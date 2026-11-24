using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nautilus.x360;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Enc;
using Un4seen.Bass.AddOn.Fx;
using Un4seen.Bass.AddOn.Mix;
using Un4seen.Bass.AddOn.EncOgg;
using Path = System.IO.Path;
using System.Windows.Forms;
using NautilusFREE;
using Un4seen.Bass.AddOn.EncFlac;
using Un4seen.Bass.AddOn.EncMp3;
using Un4seen.Bass.AddOn.EncOpus;
using System.Globalization;

namespace Nautilus
{
    class MoggSplitter
    {
        private NemoTools Tools;
        private DTAParser Parser;
        private List<int> Splits;
        private int SourceStream;
        public List<string> ErrorLog;
        private nTools nautilus3;
        public int WiiRate = 22500;

        private void Initialize()
        {
            Splits = new List<int>();
            ErrorLog = new List<string>();            
        }

        public bool ExtractDecryptMogg(string CON_file)
        {
            if (nautilus3 == null)
            {
                nautilus3 = new nTools();
            }
            return ExtractDecryptMogg(CON_file, nautilus3, new DTAParser());
        }

        public bool ExtractDecryptMogg(string CON_file, nTools nautilus3, DTAParser parser)
        {
            Initialize();
            Parser = parser;
            if (nautilus3 == null)
            {
                nautilus3 = new nTools();
            }
            nautilus3.ReleaseStreamHandle();
            if (!Parser.ExtractDTA(CON_file))
            {
                ErrorLog.Add("Couldn't extract songs.dta file from that CON file");
                return false;
            }
            if (!Parser.ReadDTA(Parser.DTA) || !Parser.Songs.Any())
            {
                ErrorLog.Add("Couldn't read that songs.dta file");
                return false;
            }
            if (Parser.Songs.Count > 1)
            {
                ErrorLog.Add("This feature does not support packs, only single songs\nUse the dePACK feature in Nautilus' Quick Pack Editor to split this pack into single songs and try again");
                return false;
            }
            var internal_name = Parser.Songs[0].InternalName;
            var xCON = new STFSPackage(CON_file);
            if (!xCON.ParseSuccess)
            {
                ErrorLog.Add("Couldn't parse that CON file");
                xCON.CloseIO();
                return false;
            }
            var xMogg = xCON.GetFile("songs/" + internal_name + "/" + internal_name + ".mogg");
            if (xMogg == null)
            {
                ErrorLog.Add("Couldn't find the mogg file inside that CON file");
                xCON.CloseIO();
                return false;
            }
            var mData = xMogg.Extract();
            xCON.CloseIO();
            if (mData == null || mData.Length == 0)
            {
                ErrorLog.Add("Couldn't extract the mogg file from that CON file");
                return false;
            }
            LoadLibraries();
            var Tools = new NemoTools();                    
            if (nautilus3.DecM(mData, false, false, DecryptMode.ToMemory)) return true;
            var temp = Path.Combine(Path.GetTempPath(), "temp.mogg");
            File.WriteAllBytes(temp, mData);
            //try to decrypt as TBRB mogg
            if (Tools.BeatlesDecrypt(temp, nautilus3, true)) return true;                      
            ErrorLog.Add("Mogg file is encrypted and I could not decrypt it, can't split it");
            return false;
        }        

        private static void UnloadLibraries()
        {
            Bass.BASS_Free();
        }

        private static void LoadLibraries()
        {
            BassEnc.BASS_Encode_GetVersion();
            BassMix.BASS_Mixer_GetVersion();
            BassFx.BASS_FX_GetVersion();
            Bass.BASS_GetVersion();
        }

        public bool DownmixMogg(string CON_file, string output, MoggSplitFormat format, string stems)
        {
            return DownmixMogg(CON_file, output, format,false, 0.0, 0.0, 0.0, 0.0, 0.0, stems);
        }

        public bool DownmixMogg(string CON_file, string output, MoggSplitFormat format, bool doWii = false, double start = 0.0, double length = 0.0, double fadeIn = 0.0, double fadeOut = 0.0, double volume = 0.0, string stems = "allstems")
        {
            if (!ExtractDecryptMogg(CON_file)) return false;
            try
            {
                if (!InitBass()) return false;
                var BassStream = Bass.BASS_StreamCreateFile(nautilus3.GetOggStreamIntPtr(), 0, nautilus3.PlayingSongOggData.Length, BASSFlag.BASS_STREAM_DECODE);
                var channel_info = Bass.BASS_ChannelGetInfo(BassStream);
                var BassMixer = BassMix.BASS_Mixer_StreamCreate(doWii ? WiiRate : channel_info.freq, 2, BASSFlag.BASS_MIXER_END | BASSFlag.BASS_MIXER_NOSPEAKER | BASSFlag.BASS_STREAM_DECODE);
                if (doWii)
                {
                    BassMix.BASS_Mixer_StreamAddChannelEx(BassMixer, BassStream, BASSFlag.BASS_MIXER_MATRIX, 0, Bass.BASS_ChannelSeconds2Bytes(BassMixer, length));
                    var track_vol = (float)Utils.DBToLevel(Convert.ToDouble(volume), 1.0);
                    Bass.BASS_ChannelSetPosition(BassStream, Bass.BASS_ChannelSeconds2Bytes(BassStream, start));
                    BASS_MIXER_NODE[] nodes = 
                    {
                        new BASS_MIXER_NODE(0, 0),
                        new BASS_MIXER_NODE(Bass.BASS_ChannelSeconds2Bytes(BassMixer, fadeIn), track_vol),
                        new BASS_MIXER_NODE(Bass.BASS_ChannelSeconds2Bytes(BassMixer, length - fadeOut), track_vol),
                        new BASS_MIXER_NODE(Bass.BASS_ChannelSeconds2Bytes(BassMixer, length), 0)
                    };
                    BassMix.BASS_Mixer_ChannelSetEnvelope(BassStream, BASSMIXEnvelope.BASS_MIXER_ENV_VOL, nodes, nodes.Count());
                }
                else
                {
                    BassMix.BASS_Mixer_StreamAddChannel(BassMixer, BassStream, BASSFlag.BASS_MIXER_MATRIX);
                }
                var matrix = GetChannelMatrix(Parser.Songs[0], channel_info.chans, stems);
                BassMix.BASS_Mixer_ChannelSetMatrix(BassStream, matrix);
                var output_file = output;
                if (string.IsNullOrWhiteSpace(output))
                {
                    output_file = Path.GetDirectoryName(CON_file) + "\\" + Parser.Songs[0].InternalName + GetExtensionFromAudioFormat(format);
                }
                var arg = "";
                switch (format)
                {
                    case MoggSplitFormat.FLAC:
                        arg = "--compression-level-5 --fast -T \"COMMENT=Made by Nemo\"";
                        BassEnc_Flac.BASS_Encode_FLAC_StartFile(BassMixer, arg, BASSEncode.BASS_ENCODE_AUTOFREE, output_file);
                        break;
                    case MoggSplitFormat.OPUS:
                        arg = "--vbr --music --comment COMMENT=\"Made by Nemo\"";
                        BassEnc_Opus.BASS_Encode_OPUS_StartFile(BassMixer, arg, BASSEncode.BASS_ENCODE_DEFAULT | BASSEncode.BASS_ENCODE_AUTOFREE, output_file);
                        break;
                    case MoggSplitFormat.OGG:
                        arg = "-q 5 -c \"COMMENT=Made by Nemo\"";
                        BassEnc_Ogg.BASS_Encode_OGG_StartFile(BassMixer, arg, BASSEncode.BASS_ENCODE_AUTOFREE, output_file);
                        break;
                    case MoggSplitFormat.MP3:
                        arg = "-b 320 --add-id3v2 --ignore-tag-errors --tc \"Made by Nemo\"";
                        BassEnc_Mp3.BASS_Encode_MP3_StartFile(BassMixer, arg, BASSEncode.BASS_UNICODE | BASSEncode.BASS_ENCODE_AUTOFREE, output_file);
                        break;
                    default:
                    case MoggSplitFormat.WAV:
                        BassEnc.BASS_Encode_Start(BassMixer, output_file, BASSEncode.BASS_ENCODE_PCM | BASSEncode.BASS_ENCODE_AUTOFREE, null, IntPtr.Zero);
                        break;
                }
                while (true)
                {
                    var buffer = new byte[20000];
                    var c = Bass.BASS_ChannelGetData(BassMixer, buffer, buffer.Length);
                    if (c <= 0) break;
                }
                UnloadLibraries();
                nautilus3.ReleaseStreamHandle();
                return File.Exists(output_file);
            }
            catch (Exception ex)
            {
                ErrorLog.Add("Error downmixing mogg file:");
                ErrorLog.Add(ex.Message);
                UnloadLibraries();
                nautilus3.ReleaseStreamHandle();
                return false;
            }
        }

        private string GetExtensionFromAudioFormat(MoggSplitFormat format)
        {
            var extension = ".";

            switch (format)
            {
                case MoggSplitFormat.FLAC:
                    extension += "flac";
                    break;
                case MoggSplitFormat.MP3:
                    extension += "mp3";
                    break;
                case MoggSplitFormat.OGG:
                    extension += "ogg";
                    break;
                case MoggSplitFormat.OPUS:
                    extension += "opus";
                    break;
                case MoggSplitFormat.WAV:
                default:
                    extension += "wav";
                    break;
            }

            return extension;
        }
        public bool ReEncodeMogg(SongData song, string mogg, int quality, bool encrypt = true)
        {
            var nautilus3 = new nTools();
            var Tools = new NemoTools();            
            if (!nautilus3.DecM(File.ReadAllBytes(mogg), false, false, DecryptMode.ToMemory)) return false;
            if (!InitBass()) return false;

            try
            {
                var BassStream = Bass.BASS_StreamCreateFile(nautilus3.GetOggStreamIntPtr(), 0, nautilus3.PlayingSongOggData.Length, BASSFlag.BASS_STREAM_DECODE);
                var channel_info = Bass.BASS_ChannelGetInfo(BassStream);
                var BassMixer = BassMix.BASS_Mixer_StreamCreate(channel_info.freq, song.ChannelsTotal, BASSFlag.BASS_MIXER_END | BASSFlag.BASS_MIXER_NOSPEAKER | BASSFlag.BASS_STREAM_DECODE);
                BassMix.BASS_Mixer_StreamAddChannel(BassMixer, BassStream, BASSFlag.BASS_STREAM_AUTOFREE);// BASSFlag.BASS_MIXER_MATRIX);

                var ogg = mogg.Replace(".mogg", ".ogg");
                Tools.DeleteFile(ogg);

                BassEnc_Ogg.BASS_Encode_OGG_StartFile(BassMixer, "-q " + quality, BASSEncode.BASS_ENCODE_AUTOFREE, ogg);

                while (true)
                {
                    var buffer = new byte[20000];
                    var c = Bass.BASS_ChannelGetData(BassMixer, buffer, buffer.Length);
                    if (c <= 0) break;
                }

                UnloadLibraries();
                nautilus3.ReleaseStreamHandle();

                if (File.Exists(ogg))
                {                    
                    Tools.MakeMogg(ogg, mogg);
                }
                else
                {
                    return false;
                }

                Tools.DeleteFile(ogg);

                if (encrypt)
                {
                    return nautilus3.EncM(File.ReadAllBytes(mogg), mogg);
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error re-encoding mogg file:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);                
                UnloadLibraries();
                nautilus3.ReleaseStreamHandle();
                return false;
            }
        }

        public bool SplitMogg(string CON_file, string output_folder, string StemsToSplit, MoggSplitFormat format, bool IgnoreAttenuationValues = false)
        {
            return ExtractDecryptMogg(CON_file) && DoSplitMogg(output_folder, StemsToSplit, format, IgnoreAttenuationValues);
        }

        public enum MoggSplitFormat
        {
            FLAC, MP3, OGG, OPUS, WAV
        }

        private bool InitBass()
        {
            try
            {
                if (!Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
                {
                    if (Bass.BASS_ErrorGetCode().ToString().Equals("BASS_ERROR_ALREADY"))
                    {
                        return true;
                    }
                    ErrorLog.Add("Error initializing BASS.NET");
                    ErrorLog.Add(Bass.BASS_ErrorGetCode().ToString());
                    ErrorLog.Add("Can't process that mogg file");
                    nautilus3.ReleaseStreamHandle();
                    return false;
                }
                Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_BUFFER, 20000);              
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.Add("Error initializing BASS.NET");
                ErrorLog.Add(ex.Message);
                return false;
            }
        }

        public int[] ArrangeStreamChannels(int totalChannels, bool isOgg)
        {            
            var channels = new int[totalChannels];            
            if (isOgg)
            {
                switch (totalChannels)
                {
                    case 3:
                        channels[0] = 0;
                        channels[1] = 2;
                        channels[2] = 1;
                        break;
                    case 5:
                        channels[0] = 0;
                        channels[1] = 2;
                        channels[2] = 1;
                        channels[3] = 3;
                        channels[4] = 4;
                        break;
                    case 6:
                        channels[0] = 0;
                        channels[1] = 2;
                        channels[2] = 1;
                        channels[3] = 4;
                        channels[4] = 5;
                        channels[5] = 3;
                        break;
                    case 7:
                        channels[0] = 0;
                        channels[1] = 2;
                        channels[2] = 1;
                        channels[3] = 5;
                        channels[4] = 6;
                        channels[5] = 4;
                        channels[6] = 3;
                        break;
                    case 8:
                        channels[0] = 0;
                        channels[1] = 2;
                        channels[2] = 1;
                        channels[3] = 6;
                        channels[4] = 4;
                        channels[5] = 7;
                        channels[6] = 5;
                        channels[7] = 3;
                        break;
                    default:
                        goto DoAllChannels;
                }
                return channels;
            }
            DoAllChannels:
            for (var i = 0; i < totalChannels; i++)
            {
                channels[i] = i;
            }
            return channels;
        }

        private bool DoSplitMogg(string folder, string StemsToSplit, MoggSplitFormat format, bool IgnoreAttenuationValues = false)
        {
            int quality = 5;//just hard code it
            var ext = "ogg";
            if (format == MoggSplitFormat.WAV)
            {
                ext = "wav";
            }
            else if (format == MoggSplitFormat.OPUS)
            {
                ext = "opus";
            }
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            var drums = folder + "drums." + ext;
            var drums1 = folder + "drums_1." + ext;
            var drums2 = folder + "drums_2." + ext;
            var drums3 = folder + "drums_3." + ext;
            var bass = folder + "bass." + ext;
            var rhythm = folder + "rhythm." + ext;
            var guitar = folder + "guitar." + ext;
            var keys = folder + "keys." + ext;
            var vocals = folder + "vocals." + ext;
            var backing = folder + "backing." + ext;
            var song = folder + "song." + ext;
            var crowd = folder + "crowd." + ext;
            var tracks = new List<string> { drums, drums1, drums2, drums3, bass, guitar, keys, vocals, backing, crowd };
            Tools = new NemoTools();
            foreach (var track in tracks)
            {
                Tools.DeleteFile(track);
            }
            try
            {
                if (!InitBass()) return false;
                SourceStream = Bass.BASS_StreamCreateFile(nautilus3.GetOggStreamIntPtr(), 0, nautilus3.PlayingSongOggData.Length, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_MIXER_NOSPEAKER);
                var info = Bass.BASS_ChannelGetInfo(SourceStream);
                var ArrangedChannels = ArrangeStreamChannels(info.chans, true);
                var isSlave = false;
                if (Parser.Songs[0].ChannelsDrums > 0 && (StemsToSplit.Contains("allstems") || StemsToSplit.Contains("drums")))
                {
                    switch (Parser.Songs[0].ChannelsDrums)
                    {
                        case 2:
                            PrepareChannelsToSplit(0, ArrangedChannels, 2, IgnoreAttenuationValues? 1.0f : GetStemVolume(0), drums, format, quality, false);
                            break;
                        case 3:
                            PrepareChannelsToSplit(0, ArrangedChannels, 1, IgnoreAttenuationValues ? 1.0f : GetStemVolume(0), drums1, format, quality, false);
                            PrepareChannelsToSplit(1, ArrangedChannels, 2, IgnoreAttenuationValues ? 1.0f : GetStemVolume(1), drums2, format, quality);
                            break;
                        case 4:
                            PrepareChannelsToSplit(0, ArrangedChannels, 1, IgnoreAttenuationValues ? 1.0f : GetStemVolume(0), drums1, format, quality, false);
                            PrepareChannelsToSplit(1, ArrangedChannels, 1, IgnoreAttenuationValues ? 1.0f : GetStemVolume(1), drums2, format, quality);
                            PrepareChannelsToSplit(2, ArrangedChannels, 2, IgnoreAttenuationValues ? 1.0f : GetStemVolume(2), drums3, format, quality);
                            break;
                        case 5:
                            PrepareChannelsToSplit(0, ArrangedChannels, 1, IgnoreAttenuationValues ? 1.0f : GetStemVolume(0), drums1, format, quality, false);
                            PrepareChannelsToSplit(1, ArrangedChannels, 2, IgnoreAttenuationValues ? 1.0f : GetStemVolume(1), drums2, format, quality);
                            PrepareChannelsToSplit(3, ArrangedChannels, 2, IgnoreAttenuationValues ? 1.0f : GetStemVolume(3), drums3, format, quality);
                            break;
                        case 6:
                            PrepareChannelsToSplit(0, ArrangedChannels, 2, IgnoreAttenuationValues ? 1.0f : GetStemVolume(0), drums1, format, quality, false);
                            PrepareChannelsToSplit(2, ArrangedChannels, 2, IgnoreAttenuationValues ? 1.0f : GetStemVolume(2), drums2, format, quality);
                            PrepareChannelsToSplit(4, ArrangedChannels, 2, IgnoreAttenuationValues ? 1.0f : GetStemVolume(4), drums3, format, quality);
                            break;
                    }
                    isSlave = true;
                }
                if (Parser.Songs[0].ChannelsBass > 0 && (StemsToSplit.Contains("allstems") || StemsToSplit.Contains("bass") || StemsToSplit.Contains("rhythm")))
                {
                    PrepareChannelsToSplit(Parser.Songs[0].ChannelsBassStart, ArrangedChannels, Parser.Songs[0].ChannelsBass, IgnoreAttenuationValues ? 1.0f : GetStemVolume(Parser.Songs[0].ChannelsBassStart), StemsToSplit.Contains("rhythm") ? rhythm : bass, format, quality, isSlave);
                    isSlave = true;
                }
                if (Parser.Songs[0].ChannelsGuitar > 0 && (StemsToSplit.Contains("allstems") || StemsToSplit.Contains("guitar")))
                {
                    PrepareChannelsToSplit(Parser.Songs[0].ChannelsGuitarStart, ArrangedChannels, Parser.Songs[0].ChannelsGuitar, IgnoreAttenuationValues ? 1.0f : GetStemVolume(Parser.Songs[0].ChannelsGuitarStart), guitar, format, quality, isSlave);
                    isSlave = true;
                }
                if (Parser.Songs[0].ChannelsVocals > 0 && (StemsToSplit.Contains("allstems") || StemsToSplit.Contains("vocals")))
                {
                    PrepareChannelsToSplit(Parser.Songs[0].ChannelsVocalsStart, ArrangedChannels, Parser.Songs[0].ChannelsVocals, IgnoreAttenuationValues ? 1.0f : GetStemVolume(Parser.Songs[0].ChannelsVocalsStart), vocals, format, quality, isSlave);
                    isSlave = true;
                }
                if (Parser.Songs[0].ChannelsKeys > 0 && (StemsToSplit.Contains("allstems") || StemsToSplit.Contains("keys")))
                {
                    PrepareChannelsToSplit(Parser.Songs[0].ChannelsKeysStart, ArrangedChannels, Parser.Songs[0].ChannelsKeys, IgnoreAttenuationValues ? 1.0f : GetStemVolume(Parser.Songs[0].ChannelsKeysStart), keys, format, quality, isSlave);
                    isSlave = true;
                }                
                if (Parser.Songs[0].ChannelsCrowd > 0 && (StemsToSplit.Contains("allstems") || StemsToSplit.Contains("crowd")))
                {
                    PrepareChannelsToSplit(Parser.Songs[0].ChannelsCrowdStart, ArrangedChannels, Parser.Songs[0].ChannelsCrowd, IgnoreAttenuationValues ? 1.0f : GetStemVolume(Parser.Songs[0].ChannelsCrowdStart), crowd, format, quality, isSlave);
                }
                if ((StemsToSplit.Contains("allstems") || StemsToSplit.Contains("backing") || StemsToSplit.Contains("song")))
                {
                    //a little hacky hack
                    if (Parser.Songs[0].Source.Equals("beatles"))
                    {
                        Parser.Songs[0].ChannelsTotal = Parser.Songs[0].ChannelsBass + Parser.Songs[0].ChannelsDrums + Parser.Songs[0].ChannelsGuitar + Parser.Songs[0].ChannelsKeys + Parser.Songs[0].ChannelsVocals + Parser.Songs[0].ChannelsCrowd + 2;
                    }
                    var back = Parser.Songs[0].ChannelsTotal - Parser.Songs[0].ChannelsBass - Parser.Songs[0].ChannelsDrums - Parser.Songs[0].ChannelsGuitar - Parser.Songs[0].ChannelsKeys - Parser.Songs[0].ChannelsVocals - Parser.Songs[0].ChannelsCrowd;
                    if (back > 0) //backing not required
                    {
                        if (Parser.Songs[0].ChannelsCrowdStart + Parser.Songs[0].ChannelsCrowd == Parser.Songs[0].ChannelsTotal) //crowd channels are last
                        {
                            PrepareChannelsToSplit(Parser.Songs[0].ChannelsCrowdStart - back, ArrangedChannels, back, IgnoreAttenuationValues ? 1.0f : GetStemVolume(Parser.Songs[0].ChannelsCrowdStart - back), StemsToSplit.Contains("song") ? song : backing, format, quality, isSlave);
                        }
                        else //backing channels are last
                        {
                            PrepareChannelsToSplit(Parser.Songs[0].ChannelsTotal - back, ArrangedChannels, back, IgnoreAttenuationValues ? 1.0f : GetStemVolume(Parser.Songs[0].ChannelsTotal - back), StemsToSplit.Contains("song") ? song : backing, format, quality, isSlave);
                        }
                        isSlave = true;
                    }
                }
                         
                while (true)
                {
                    var buffer = new byte[20000];
                    var c = Bass.BASS_ChannelGetData(Splits[0], buffer, buffer.Length);
                    if (c <= 0) break;
                    for (var i = 1; i < Splits.Count; i++)
                    {
                        while (Bass.BASS_ChannelGetData(Splits[i], buffer, buffer.Length) > 0){}
                    }
                }
                foreach (var split in Splits)
                {
                    Bass.BASS_StreamFree(split);
                }
                UnloadLibraries();
                nautilus3.ReleaseStreamHandle();
            }
            catch (Exception ex)
            {
                ErrorLog.Add("Error splitting mogg file:");
                ErrorLog.Add(ex.Message);
                foreach (var split in Splits)
                {
                    Bass.BASS_StreamFree(split);
                }
                UnloadLibraries();
                nautilus3.ReleaseStreamHandle();
                return false;
            }
            return true;
        }

        private void PrepareChannelsToSplit(int index, IList<int> ArrangedChannels, int channels, float vol, string file, MoggSplitFormat format, int quality, bool slave = true)
        {
            var channel_map = new int[channels == 2 ? 3 : 2];
            channel_map[0] = ArrangedChannels[index];
            channel_map[1] = channels == 2 ? ArrangedChannels[index + 1] : -1;
            if (channels == 2)
            {
                channel_map[2] = -1;
            }
            var flags = slave ? BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SPLIT_SLAVE | BASSFlag.BASS_MIXER_NOSPEAKER : BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_MIXER_NOSPEAKER;
            var out_stream = BassMix.BASS_Split_StreamCreate(SourceStream, flags, channel_map);
            var volumeFX = Bass.BASS_ChannelSetFX(out_stream, BASSFXType.BASS_FX_BFX_VOLUME, 0);
            var volume = new BASS_BFX_VOLUME {lChannel = 0, fVolume = vol};
            Bass.BASS_FXSetParameters(volumeFX, volume);
            Splits.Add(out_stream);
            if (format == MoggSplitFormat.OGG)
            {
                BassEnc_Ogg.BASS_Encode_OGG_StartFile(out_stream, "-q " + quality + " \" -c \"COMMENT=Made by Nemo\"", BASSEncode.BASS_ENCODE_AUTOFREE, file);
            }
            else if (format == MoggSplitFormat.OPUS)
            {
                BassEnc_Opus.BASS_Encode_OPUS_StartFile(out_stream, "--vbr --music --comment COMMENT=\"Made by Nemo\"", BASSEncode.BASS_ENCODE_DEFAULT | BASSEncode.BASS_ENCODE_AUTOFREE, file);
            }
            else
            {
                BassEnc.BASS_Encode_Start(out_stream, file, BASSEncode.BASS_ENCODE_PCM | BASSEncode.BASS_ENCODE_AUTOFREE, null, IntPtr.Zero);
            }         
        }

        private float GetStemVolume(int curr_channel)
        {
            const double max_dB = 1.0;
            var volumes = Parser.Songs[0].OriginalAttenuationValues.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
            float vol;
            try
            {
                vol = (float)Utils.DBToLevel(Convert.ToDouble(volumes[curr_channel], CultureInfo.InvariantCulture), max_dB);
            }
            catch (Exception)
            {
                vol = (float)1.0;
            }
            return vol;
        }

        public float[,] GetDownmixChannelMatrix(SongData song, int inputChans, int outputChans, bool deleteCrowd = true)
        {
            //initialize matrix
            //matrix must be float[output_channels, input_channels]
            var matrix = new float[outputChans, inputChans];
            var ArrangedChannels = ArrangeStreamChannels(inputChans, true);

            var drums = 0;
            if (song.ChannelsDrums > 0)
            {
                drums = song.ChannelsDrums - 2;
                //for drums it's a bit tricky because of the possible combinations
                switch (song.ChannelsDrums)
                {
                    case 2:
                        //stereo kit
                        matrix = DoDownmixMatrixPanning(song, matrix, ArrangedChannels, 2, 0, 0, true);
                        break;
                    case 3:
                        //mono kick
                        matrix = DoDownmixMatrixPanning(song, matrix, ArrangedChannels, 1, 0, 0, true);
                        //stereo kit
                        matrix = DoDownmixMatrixPanning(song, matrix, ArrangedChannels, 2, 1, 0, true);
                        break;
                    case 4:
                        //mono kick
                        matrix = DoDownmixMatrixPanning(song, matrix, ArrangedChannels, 1, 0, 0, true);
                        //mono snare
                        matrix = DoDownmixMatrixPanning(song, matrix, ArrangedChannels, 1, 1, 0, true);
                        //stereo kit
                        matrix = DoDownmixMatrixPanning(song, matrix, ArrangedChannels, 2, 2, 0, true);
                        break;
                    case 5:
                        //mono kick
                        matrix = DoDownmixMatrixPanning(song, matrix, ArrangedChannels, 1, 0, 0, true);
                        //stereo snare
                        matrix = DoDownmixMatrixPanning(song, matrix, ArrangedChannels, 2, 1, 0, true);
                        //stereo kit
                        matrix = DoDownmixMatrixPanning(song, matrix, ArrangedChannels, 2, 3, 0, true);
                        break;
                    case 6:
                        //stereo kick
                        matrix = DoDownmixMatrixPanning(song, matrix, ArrangedChannels, 2, 0, 0, true);
                        //stereo snare
                        matrix = DoDownmixMatrixPanning(song, matrix, ArrangedChannels, 2, 2, 0, true);
                        //stereo kit
                        matrix = DoDownmixMatrixPanning(song, matrix, ArrangedChannels, 2, 4, 0, true);
                        break;
                }
            }
            if (song.ChannelsBass > 0)
            {
                matrix = DoDownmixMatrixPanning(song, matrix, ArrangedChannels, song.ChannelsBass, song.ChannelsBassStart, song.ChannelsBassStart - drums, false);
            }
            if (song.ChannelsGuitar > 0)
            {
                matrix = DoDownmixMatrixPanning(song, matrix, ArrangedChannels, song.ChannelsGuitar, song.ChannelsGuitarStart, song.ChannelsGuitarStart - drums, false);
            }
            if (song.ChannelsVocals > 0)
            {
                matrix = DoDownmixMatrixPanning(song, matrix, ArrangedChannels, song.ChannelsVocals, song.ChannelsVocalsStart, song.ChannelsVocalsStart - drums, false);
            }
            if (song.ChannelsKeys > 0)
            {
                matrix = DoDownmixMatrixPanning(song, matrix, ArrangedChannels, song.ChannelsKeys, song.ChannelsKeysStart, song.ChannelsKeysStart - drums, false);
            }
            if (song.ChannelsCrowd > 0 && !deleteCrowd)
            {
                matrix = DoDownmixMatrixPanning(song, matrix, ArrangedChannels, song.ChannelsCrowd, song.ChannelsCrowdStart, song.ChannelsCrowdStart - drums, false);
            }            
            var backing = song.ChannelsTotal - song.ChannelsBass - song.ChannelsDrums - song.ChannelsGuitar - song.ChannelsKeys - song.ChannelsVocals - song.ChannelsCrowd;
            if (backing > 0) //backing not required 
            {
                if (song.ChannelsCrowdStart + song.ChannelsCrowd == song.ChannelsTotal) //crowd channels are last
                {
                    matrix = DoDownmixMatrixPanning(song, matrix, ArrangedChannels, backing, song.ChannelsCrowdStart - backing, song.ChannelsCrowdStart - backing - drums, false);                       
                }
                else
                {
                    matrix = DoDownmixMatrixPanning(song, matrix, ArrangedChannels, backing, song.ChannelsTotal - backing, song.ChannelsTotal - backing - drums, false);
                }
            }
            return matrix;
        }

        public float[,] DoDownmixMatrixPanning(SongData song, float[,] in_matrix, IList<int> ArrangedChannels, int inst_channels, int curr_channel, int new_channel, bool isDrums)
        {
            //by default matrix values will be 0 = 0 volume
            //if nothing is assigned here, it stays at 0 so that channel won't be played
            //otherwise we assign a volume level based on the dta volumes

            //initialize output matrix based on input matrix, just in case something fails there's something going out
            var matrix = in_matrix;

            //split attenuation and panning info from DTA file for index access
            var volumes = song.OriginalAttenuationValues.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
            var pans = song.PanningValues.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

            //BASS.NET lets us specify maximum volume when converting dB to Level
            //in case we want to change this later, it's only one value to change
            const double max_dB = 1.0;

            //technically we could do each channel, but Magma only allows us to specify volume per track, 
            //so both channels should have same volume, let's save a tiny bit of processing power
            float vol;
            try
            {
                vol = (float)Utils.DBToLevel(Convert.ToDouble(volumes[curr_channel], CultureInfo.InvariantCulture), max_dB);
            }
            catch (Exception)
            {
                vol = (float)1.0;
            }

            //assign volume level to channels in the matrix
            if (inst_channels == 2) //is it a stereo track
            {
                try
                {
                    //assign current channel (left) to left channel
                    matrix[isDrums ? 0 : new_channel, ArrangedChannels[curr_channel]] = vol;
                }
                catch (Exception)
                { }
                try
                {
                    //assign next channel (right) to the right channel
                    matrix[isDrums ? 1 : new_channel + 1, ArrangedChannels[curr_channel + 1]] = vol;
                }
                catch (Exception)
                { }
            }
            else
            {
                //it's a mono track
                double pan;
                try
                {
                    pan = Convert.ToDouble(pans[curr_channel], CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    pan = 0.0; // in case there's an error above, it gets centered
                }

                matrix[isDrums ? 0 : new_channel, ArrangedChannels[curr_channel]] = vol;
            }
            return matrix;
        }

        public float[,] GetChannelMatrix(SongData song, int inputChans, string stems, int outputChans = 2, bool isOgg = true)
        {
            //initialize matrix
            //matrix must be float[output_channels, input_channels]
            var matrix = new float[outputChans, inputChans];
            var ArrangedChannels = ArrangeStreamChannels(inputChans, isOgg);
            if (song.ChannelsDrums > 0 && (stems.Contains("drums") || stems.Contains("allstems")))
            {
                //for drums it's a bit tricky because of the possible combinations
                switch (song.ChannelsDrums)
                {
                    case 2:
                        //stereo kit
                        matrix = DoMatrixPanning(song, matrix, ArrangedChannels, 2, 0);
                        break;
                    case 3:
                        //mono kick
                        matrix = DoMatrixPanning(song, matrix, ArrangedChannels, 1, 0);
                        //stereo kit
                        matrix = DoMatrixPanning(song, matrix, ArrangedChannels, 2, 1);
                        break;
                    case 4:
                        //mono kick
                        matrix = DoMatrixPanning(song, matrix, ArrangedChannels, 1, 0);
                        //mono snare
                        matrix = DoMatrixPanning(song, matrix, ArrangedChannels, 1, 1);
                        //stereo kit
                        matrix = DoMatrixPanning(song, matrix, ArrangedChannels, 2, 2);
                        break;
                    case 5:
                        //mono kick
                        matrix = DoMatrixPanning(song, matrix, ArrangedChannels, 1, 0);
                        //stereo snare
                        matrix = DoMatrixPanning(song, matrix, ArrangedChannels, 2, 1);
                        //stereo kit
                        matrix = DoMatrixPanning(song, matrix, ArrangedChannels, 2, 3);
                        break;
                    case 6:
                        //stereo kick
                        matrix = DoMatrixPanning(song, matrix, ArrangedChannels, 2, 0);
                        //stereo snare
                        matrix = DoMatrixPanning(song, matrix, ArrangedChannels, 2, 2);
                        //stereo kit
                        matrix = DoMatrixPanning(song, matrix, ArrangedChannels, 2, 4);
                        break;
                }
            }
            //var channel = song.ChannelsDrums;
            if (song.ChannelsBass > 0 && (stems.Contains("bass") || stems.Contains("allstems")))
            {
                matrix = DoMatrixPanning(song, matrix, ArrangedChannels, song.ChannelsBass, song.ChannelsBassStart);//channel);
            }
            //channel = channel + song.ChannelsBass;
            if (song.ChannelsGuitar > 0 && (stems.Contains("guitar") || stems.Contains("allstems")))
            {
                matrix = DoMatrixPanning(song, matrix, ArrangedChannels, song.ChannelsGuitar, song.ChannelsGuitarStart);//channel);
            }
            //channel = channel + song.ChannelsGuitar;
            if (song.ChannelsVocals > 0 && (stems.Contains("vocals") || stems.Contains("allstems")))
            {
                matrix = DoMatrixPanning(song, matrix, ArrangedChannels, song.ChannelsVocals, song.ChannelsVocalsStart);//channel);
            }
            //channel = channel + song.ChannelsVocals;
            if (song.ChannelsKeys > 0 && (stems.Contains("keys") || stems.Contains("allstems")))
            {
                matrix = DoMatrixPanning(song, matrix, ArrangedChannels, song.ChannelsKeys, song.ChannelsKeysStart);//channel);
            }
            //channel = channel + song.ChannelsKeys;
            if (song.ChannelsCrowd > 0 && !stems.Contains("NOcrowd") && (stems.Contains("crowd") || stems.Contains("allstems")))
            {
                matrix = DoMatrixPanning(song, matrix, ArrangedChannels, song.ChannelsCrowd, song.ChannelsCrowdStart);//channel);
            }
            //channel = channel + song.ChannelsCrowd;
            if ((stems.Contains("backing") || stems.Contains("allstems"))) //song.ChannelsBacking > 0 &&  ---- should always be enabled per specifications
            {
                var backing = song.ChannelsTotal - song.ChannelsBass - song.ChannelsDrums - song.ChannelsGuitar - song.ChannelsKeys - song.ChannelsVocals - song.ChannelsCrowd;
                if (backing > 0) //backing not required 
                {
                    if (song.ChannelsCrowdStart + song.ChannelsCrowd == song.ChannelsTotal) //crowd channels are last
                    {
                        matrix = DoMatrixPanning(song, matrix, ArrangedChannels, backing, song.ChannelsCrowdStart - backing);//channel);                        
                    }
                    else
                    {
                        matrix = DoMatrixPanning(song, matrix, ArrangedChannels, backing, song.ChannelsTotal - backing);//channel);
                    }
                }
            }
            return matrix;
        }

        private static float[,] DoMatrixPanning(SongData song, float[,] in_matrix, IList<int> ArrangedChannels, int inst_channels, int curr_channel)
        {
            //by default matrix values will be 0 = 0 volume
            //if nothing is assigned here, it stays at 0 so that channel won't be played
            //otherwise we assign a volume level based on the dta volumes

            //initialize output matrix based on input matrix, just in case something fails there's something going out
            var matrix = in_matrix;

            //split attenuation and panning info from DTA file for index access
            string[] volumes = new string[song.ChannelsTotal];
            try
            {
                volumes = song.OriginalAttenuationValues.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
            }
            catch { }
            string[] pans = new string[song.ChannelsTotal];
            try
            {
                pans = song.PanningValues.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
            }
            catch { }

            //BASS.NET lets us specify maximum volume when converting dB to Level
            //in case we want to change this later, it's only one value to change
            const double max_dB = 1.0;

            //technically we could do each channel, but Magma only allows us to specify volume per track, 
            //so both channels should have same volume, let's save a tiny bit of processing power
            float vol;
            try
            {
                vol = (float)Utils.DBToLevel(Convert.ToDouble(volumes[curr_channel], CultureInfo.InvariantCulture), max_dB);
            }
            catch (Exception)
            {
                vol = (float)1.0;
            }

            //assign volume level to channels in the matrix
            if (inst_channels == 2) //is it a stereo track
            {
                try
                {
                    //assign current channel (left) to left channel
                    matrix[0, ArrangedChannels[curr_channel]] = vol;
                }
                catch (Exception)
                { }
                try
                {
                    //assign next channel (right) to the right channel
                    matrix[1, ArrangedChannels[curr_channel + 1]] = vol;
                }
                catch (Exception)
                { }
            }
            else
            {
                //it's a mono track, let's assign based on the panning value
                double pan;
                try
                {
                    pan = Convert.ToDouble(pans[curr_channel], CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    pan = 0.0; // in case there's an error above, it gets centered
                }

                if (pan <= 0) //centered or left, assign it to the left channel
                {
                    matrix[0, ArrangedChannels[curr_channel]] = vol;
                }
                if (pan >= 0) //centered or right, assign it to the right channel
                {
                    matrix[1, ArrangedChannels[curr_channel]] = vol;
                }
            }
            return matrix;
        }

        public bool DoMoggDownmix(nTools nautilus3, DTAParser Parser, string mogg, int channels, bool DeleteCrowd)
        {
            //const int BassBuffer = 1000;
            //initialize BASS.NET
            //Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_BUFFER, BassBuffer);
            //Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD, 50);

            if (!InitBass()) return false;

            // create a decoder for the OGG file
            var BassStream = Bass.BASS_StreamCreateFile(nautilus3.GetOggStreamIntPtr(), 0L, nautilus3.PlayingSongOggData.Length, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);
            var channel_info = Bass.BASS_ChannelGetInfo(BassStream);

            // create a mixer with same frequency rate as the input file
            var BassMixer = BassMix.BASS_Mixer_StreamCreate(channel_info.freq, channels, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_MIXER_NOSPEAKER | BASSFlag.BASS_MIXER_END);
            BassMix.BASS_Mixer_StreamAddChannel(BassMixer, BassStream, BASSFlag.BASS_MIXER_MATRIX);

            //get and apply channel matrix
            var matrix = GetDownmixChannelMatrix(Parser.Songs[0], channel_info.chans, channels, DeleteCrowd);
            BassMix.BASS_Mixer_ChannelSetMatrix(BassStream, matrix);

            var ogg = mogg.Replace(".mogg", ".ogg");
            var Tools = new NemoTools();
            Tools.DeleteFile(ogg);
            
            BassEnc_Ogg.BASS_Encode_OGG_StartFile(BassMixer, "-q 3", BASSEncode.BASS_ENCODE_AUTOFREE, ogg);
            while (true)
            {
                var buffer = new byte[20000];
                var c = Bass.BASS_ChannelGetData(BassMixer, buffer, buffer.Length);
                if (c < 0) break;
            }

            Bass.BASS_ChannelStop(BassMixer);
            Bass.BASS_StreamFree(BassMixer);
            UnloadLibraries();
            nautilus3.ReleaseStreamHandle();

            return File.Exists(ogg);
        }
    }
}
