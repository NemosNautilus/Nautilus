using Nautilus.Properties;
using Nautilus.x360;
using NautilusFREE;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Enc;
using Un4seen.Bass.AddOn.EncOgg;
using Un4seen.Bass.AddOn.Mix;

namespace Nautilus
{
    partial class VolumeNormalizer : Form
    {
        private static string inputDir;
        private static string tempFile;
        private static List<string> inputFiles;
        private DateTime endTime;
        private DateTime startTime;
        private readonly NemoTools Tools;
        private nTools nautilus3;
        private readonly DTAParser Parser;
        private MoggSplitter moggSplitter;
        private string attenuationValues;

        private bool Debug = true;

        private SHA1CryptoServiceProvider sha1 = new System.Security.Cryptography.SHA1CryptoServiceProvider();
        private STFSPackage xPackage;

        private bool BassInit = false;
        private int BassStream;
        private int BassMixer;
        BASS_CHANNELINFO channel_info;

        private bool hasOriginalValues = false;
        private bool originalAudioFound = false;
        private bool audioModified = false;
        private string audioHash = "";

        private double volumeOffset = 0.0;
        private double dBAverage = 0.0;

        // Our target loudness that we want. Normally -6.4 dB.
        // This is always overridden when starting a job to the value of numTargetValue (default -2.0).
        private double targetDB = -6.4;
        // The 83 RB3 songs average at a reported 2.36dB.

        // The threshold below which no normalization is performed. If the absolute value of the difference between the current average volume and the target volume is less than this threshold, no changes are made.
        private double thresholdDB = 0.4;




        public VolumeNormalizer(Color ButtonBackColor, Color ButtonTextColor)
        {
            InitializeComponent();

            //initialize
            Tools = new NemoTools();
            Parser = new DTAParser();
            nautilus3 = new nTools();
            moggSplitter = new MoggSplitter();

            inputFiles = new List<string>();
            inputDir = Application.StartupPath;
            tempFile = Application.StartupPath + "\\bin\\temp";

            if (!Directory.Exists(inputDir))
            {
                Directory.CreateDirectory(inputDir);
            }

            var formButtons = new List<Button> { btnReset, btnRefresh, btnFolder, btnBegin };
            foreach (var button in formButtons)
            {
                button.BackColor = ButtonBackColor;
                button.ForeColor = ButtonTextColor;
                button.FlatAppearance.MouseOverBackColor = button.BackColor == Color.Transparent ? Color.FromArgb(127, Color.AliceBlue.R, Color.AliceBlue.G, Color.AliceBlue.B) : Tools.LightenColor(button.BackColor);
            }

            toolTip1.SetToolTip(btnBegin, "Click to begin process");
            toolTip1.SetToolTip(btnFolder, "Click to select the input folder");
            toolTip1.SetToolTip(btnRefresh, "Click to refresh if the contents of the folder have changed");
            toolTip1.SetToolTip(txtFolder, "This is the working directory");
            toolTip1.SetToolTip(lstLog, "This is the application log. Right click to export");
            toolTip1.SetToolTip(numThresholdValue, "Songs that are within this amount of the target volume will not be changed");
            toolTip1.SetToolTip(chkAlbumMode, "Calculate the average adjustment needed to bring the average volume level of all songs to the target volume, then apply that adjustment to all songs.\n\nThis helps maintain the dynamic range of a collection of songs, such as an album, when applying a volume offset.");
        }

        private void Log(string message)
        {
            if (lstLog.InvokeRequired)
            {
                lstLog.Invoke(new MethodInvoker(() => lstLog.Items.Add(message)));
                lstLog.Invoke(new MethodInvoker(() => lstLog.SelectedIndex = lstLog.Items.Count - 1));
            }
            else
            {
                lstLog.Items.Add(message);
                lstLog.SelectedIndex = lstLog.Items.Count - 1;
            }
        }

        private void DebugLog(string message)
        {
            if (Debug)
            {
                Log(message);
            }
        }

        private void InitAudio()
        {

            if (!BassInit && Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
            {
                BassInit = true;

                BassStream = Bass.BASS_StreamCreateFile(nautilus3.GetOggStreamIntPtr(), 0, nautilus3.PlayingSongOggData.Length, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);
                channel_info = Bass.BASS_ChannelGetInfo(BassStream);
                BassMixer = BassMix.BASS_Mixer_StreamCreate(channel_info.freq, 2, BASSFlag.BASS_MIXER_END | BASSFlag.BASS_MIXER_NOSPEAKER | BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_AC3_DOWNMIX_2);
                DebugLog("Bass init");
            }
            else
            {
                throw new Exception("Bass failed to initialize!");
            }
            
            var numValues = Parser.Songs[0].OriginalAttenuationValues.Split(new char[0], StringSplitOptions.RemoveEmptyEntries).Length;

            DebugLog($"MOGG Audio Channels Count: {channel_info.chans}");
            DebugLog($"DTA Channel Value Count: {numValues}");
            DebugLog($"Current Vols: {Parser.Songs[0].AttenuationValues}");
            DebugLog($"Original Vols: {Parser.Songs[0].OriginalAttenuationValues}");

            if (channel_info.chans != numValues)
            {
                throw new Exception("Audio file and DTA do not have matching Audio Channels, aborting.");
            }

            BassMix.BASS_Mixer_StreamAddChannel(BassMixer, BassStream, BASSFlag.BASS_MIXER_MATRIX);

            //get and apply channel matrix
            var matrix = moggSplitter.GetChannelMatrix(Parser.Songs[0], channel_info.chans, "allstems");
            BassMix.BASS_Mixer_ChannelSetMatrix(BassStream, matrix);

        }

        private void CleanUp()
        {
            xPackage.CloseIO();
            nautilus3.ReleaseStreamHandle();

            if (BassInit)
            {
                if (!Bass.BASS_Free())
                {
                    throw new Exception("Bass failed to free!");
                }
                else
                {
                    BassInit = false;
                    DebugLog("Bass freed");
                }
            }
        }

        private byte[] GetAudioFromFile()
        {
            Tools.DeleteFile(Path.GetTempPath() + "\\" + "mogg");
            Tools.DeleteFile(Path.GetTempPath() + "\\" + "ogg_adjust");
            Tools.DeleteFile(Path.GetTempPath() + "\\" + "mogg_adjust");
            Tools.DeleteFile(Path.GetTempPath() + "\\" + "mogg_enc");

            string internal_name = Parser.Songs[0].InternalName;

            byte[] moggData = null;
            originalAudioFound = false;
            audioModified = false;

            if (Parser.Songs[0].VNAudioHash != "")
            {
                string searchName = $"{Parser.Songs[0].InternalName}_{Parser.Songs[0].VNAudioHash.Substring(0, 8)}";

                var BackupLocation = Tools.CurrentFolder + "\\Backup Audio\\";
                var fileToTest = BackupLocation + $"{searchName}.moggbackup";

                // Check to see if the backup file exists inside the CON file.
                if (xPackage.GetFile("songs/" + internal_name + "/" + searchName + ".moggbackup") != null)
                {
                    originalAudioFound = true;
                    Log("Found original audio backup inside CON file.");
                    xPackage.GetFile("songs/" + internal_name + "/" + searchName + ".moggbackup").ExtractToFile((Path.GetTempPath() + "\\" + "mogg"));
                    moggData = File.ReadAllBytes( Path.GetTempPath() + "\\" + "mogg" );
                }

                // Check to see if the backup file exists externally.
                else if (File.Exists(fileToTest))
                {
                    originalAudioFound = true;
                    Log("Found original audio backup externally.");
                    File.Copy(fileToTest, (Path.GetTempPath() + "\\" + "mogg"));
                    moggData = File.ReadAllBytes(fileToTest);
                }
            }
            else
            {
                DebugLog("No Hash found, skipping backup search.");
            }

            // Validate our backup audio.
            if (originalAudioFound)
            {
                audioHash = string.Concat(sha1.ComputeHash(moggData).Select(x => x.ToString("X2")));

                if (Parser.Songs[0].VNAudioHash != "")
                {
                    if (audioHash != Parser.Songs[0].VNAudioHash)
                    {
                        Log("Audio backup does not match song, attempting to load CON audio.");
                        originalAudioFound = false;
                    }
                    else
                    {
                        if (chkRestore.Checked)
                        {
                            audioModified = true;
                        }
                    }
                }

            }

            // Check to see if the backup file exists inside the CON file.
            if (!originalAudioFound)
            {
                if (xPackage.GetFile("songs/" + internal_name + "/" + internal_name + ".mogg") != null)
                {
                    Log("Loading audio file from CON file...");
                    var mogg = xPackage.GetFile("songs/" + internal_name + "/" + internal_name + ".mogg");

                    var tempMogg = (Path.GetTempPath() + "\\" + "mogg");
                    mogg.ExtractToFile(tempMogg);

                    moggData = File.ReadAllBytes(tempMogg);

                    audioHash = string.Concat(sha1.ComputeHash(moggData).Select(x => x.ToString("X2")));

                    if (Parser.Songs[0].VNAudioHash != "")
                    {
                        if (audioHash != Parser.Songs[0].VNAudioHash)
                        {
                            throw new Exception("Audio was previously modified, no backup found, not processing this track.");
                        }
                        else
                        {
                            DebugLog("Audio Hash found, matches internal .mogg.");
                            originalAudioFound = true;
                        }
                    }
                    else
                    {
                        DebugLog("No Audio Hash found, assuming original Audio.");
                        originalAudioFound = true;
                    }

                }
                // Could not find the audio file, we cry.
                else
                {
                    throw new Exception("Could not find an audio file!");
                }
            }

            if (chkRestore.Checked)
            {
                if (!originalAudioFound)
                {
                    throw new Exception("Unable to find original audio for restoration!");
                }
                else
                {
                    return null;
                }
            }
            
            DebugLog($"Audio Hash: {audioHash}");

            // We need to attempt to decrypt the audio here, we can't work with the raw data.
            bool decryptStatus = false;                       

            if (nautilus3.DecM(moggData, false, false, DecryptMode.ToMemory)) decryptStatus = true;

            if (decryptStatus == false)
            {
                throw new Exception("Audio file is encrypted and could not be decrypted.");
            }

            return moggData;

        }

        string FormatDB(double level)
        {
            // We do this in order to not deal with *very* small numbers in our string output.
            if (level > -0.01 && level < 0.01)
            {
                return "0.0";
            }
            else
            {
                // Here we limit the length of the output string to two decimal places.
                string levelString = level.ToString("F2", CultureInfo.InvariantCulture);

                if (!levelString.Contains("."))
                {
                    levelString = $"{levelString}.0";
                }

                return levelString;
            }
        }

        private void CalculateVolumeOffset()
        {
            DebugLog($"Finding volume level to reach target of {targetDB}...");

            while (Math.Abs(dBAverage - targetDB) > thresholdDB)
            {
                var matrix = GetChannelMatrix_VolumeAdjust(Parser.Songs[0], channel_info.chans, "allstems");
                BassMix.BASS_Mixer_ChannelSetMatrix(BassStream, matrix);

                Log("CalculateVolumeOffset(): calculating average after matrix applied");
                dBAverage = CalculateVolumeAverage();
                double dBDistance = double.Parse(FormatDB(Math.Abs(dBAverage - targetDB)));

                DebugLog($"Distance to target: {dBDistance}");

                if (Math.Abs(dBAverage - targetDB) > thresholdDB)
                {
                    if (dBAverage >= targetDB)
                    {
                        volumeOffset -= dBDistance;
                    }
                    else
                    {
                        volumeOffset += dBDistance;
                    }
                }

                //Log($"{dBAverage}");

            }

            Log($"Target volume {targetDB} dB found at {volumeOffset}.");
        }

        private void ApplyOffset(string file, double calculatedOffset = -999)
        {
            bool fixedOffset = (calculatedOffset != -999);
            double attenuationOffset;
            if (calculatedOffset == -999)
            {
                attenuationOffset = dBAverage - targetDB;
            } else
            {
                attenuationOffset = -calculatedOffset;
            }


            // Strip most of the decimal places, because we don't need to be *that* precise.
            attenuationOffset = double.Parse(FormatDB(attenuationOffset), CultureInfo.InvariantCulture);
            volumeOffset = -attenuationOffset;

            Log($"Average dB of {(fixedOffset ? "album": "song MOGG")} is: {FormatDB(dBAverage)} dB, {FormatDB(attenuationOffset)} dB away from the target dB of: {targetDB} dB.");

            // Get Attenuation values
            var values = Parser.Songs[0].OriginalAttenuationValues.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
            double highestVolume = -64.0;

            DebugLog("Original Values: (checking if we need to re-render audio)");
            DebugLog($"{Parser.Songs[0].OriginalAttenuationValues}");

            // We want to know how far we can increase the volume before we need to
            // re-render the audio.
            DebugLog("highestVolume check");
            foreach (var value in values)
            {
                double parsedValue = double.Parse(value, CultureInfo.InvariantCulture);
                DebugLog($"Value: {value}, {parsedValue}");

                if (parsedValue > highestVolume)
                {
                    DebugLog($"Higher than: {highestVolume}");
                    highestVolume = parsedValue;
                }
            }

            double attenuationRange = 0 - highestVolume;
            DebugLog($"Highest Channel Volume is {highestVolume}, range is {attenuationRange}.");

            if (!fixedOffset) CalculateVolumeOffset();

            if (volumeOffset > 0)
            {
                DebugLog("Song needs to be louder...");

                if (Math.Abs(attenuationOffset) < thresholdDB)
                {
                    Log("Song is within threshold, skipping audio re-render.");
                }
                else
                {
                    if (volumeOffset < attenuationRange)
                    {
                        Log("Song is within limits, adjusting...");
                    }
                    else
                    {
                        if (radioAllowRender.Checked)
                        {
                            if (chkBackupAudio.Checked)
                            {
                                var BackupLocation = Tools.CurrentFolder + "\\Backup Audio\\";
                                var BackupFile = BackupLocation + $"{Parser.Songs[0].InternalName}_{audioHash.Substring(0, 8)}.moggbackup";

                                if (!Directory.Exists(BackupLocation))
                                {
                                    Directory.CreateDirectory(BackupLocation);
                                }

                                if (!File.Exists(BackupFile))
                                {
                                    Log("Backing up original mogg...");
                                    File.Copy(Path.GetTempPath() + "mogg", BackupFile);
                                }
                                else
                                {
                                    Log("Backup Audio already exists, skipping backup.");
                                }

                            }

                            Log("Rendering new audio file...");
                            audioModified = true;

                            Bass.BASS_ChannelSetPosition(BassStream, Bass.BASS_ChannelSeconds2Bytes(BassStream, 0));
                            Bass.BASS_ChannelSetPosition(BassMixer, Bass.BASS_ChannelSeconds2Bytes(BassMixer, 0));
                            BassMix.BASS_Mixer_ChannelSetPosition(BassMixer, Bass.BASS_ChannelSeconds2Bytes(BassMixer, 0));

                            var newVolume = (float)Utils.DBToLevel(Convert.ToDouble(volumeOffset), 1.0);

                            BASS_FX_VOLUME_PARAM volume = new BASS_FX_VOLUME_PARAM(newVolume, 1, 0, 0);
                            var fxVolumeHandle = Bass.BASS_ChannelSetFX(BassStream, BASSFXType.BASS_FX_VOLUME, 0);
                            Bass.BASS_FXSetParameters(fxVolumeHandle, volume);

                            string fileToTest = Path.GetTempPath() + "\\" + "ogg_adjust";
                            BassEnc_Ogg.BASS_Encode_OGG_StartFile(BassStream, "-q 5", BASSEncode.BASS_ENCODE_AUTOFREE, fileToTest);

                            while (true)
                            {
                                var buffer = new byte[20000];
                                var c = Bass.BASS_ChannelGetData(BassStream, buffer, buffer.Length);
                                if (c <= 0) break;
                            }

                            BassEnc.BASS_Encode_Stop(BassStream);

                            Log("Adding MOGG header...");
                            if (Tools.MakeMogg(fileToTest, Path.GetTempPath() + "\\" + "mogg_adjust"))
                            {
                                Log("Success");
                            }
                            else
                            {
                                throw new Exception("Failed");
                            }

                        }
                        else
                        {
                            Log("Audio not allowed to be re-rendered, adjusting as far as possible...");
                            volumeOffset = attenuationRange;
                        }
                    }
                }

            }
            else
            {
                DebugLog("Song needs to be quieter, applying DTA modification");
            }


            attenuationValues = "";

            DebugLog($"values Length: {values.Length}");

            foreach (var value in values)
            {
                double preFinal = double.Parse(value, CultureInfo.InvariantCulture) + volumeOffset;

                if (audioModified)
                {
                    preFinal = 0.0;
                }

                attenuationValues += FormatDB(preFinal) + " ";
                DebugLog($"Modifying original value: {value} to {FormatDB(preFinal)}");
            }

            // Trim the white space off the end of the string.
            attenuationValues = attenuationValues.Trim();

            CleanUp();
            WriteDTA();
            WriteCON(file);
        }

        private double CalculateVolumeAverage()
        {

            // How we are determining the average loudness is to grab the top percentage of
            // levels, and then averaging them out. We grab a percentage of the levels because
            // we can ignore the most quiet parts of the song, as well as some of the loudest parts.
            int topPercentToKeep = 70;
            int topPercentToStrip = 10;

            int silenceLevel = -24;

            // We will remove some of the levels from the beginning of the song
            // to compensate for the count-in of the song.
            int secondsToRemoveFromStart = 4;

            Log("Determining loudness of song...");

            var level = new float[2];
            List<double> dBLevels = new List<double>();

            Bass.BASS_ChannelSetPosition(BassStream, Bass.BASS_ChannelSeconds2Bytes(BassStream, 0));
            Bass.BASS_ChannelSetPosition(BassMixer, Bass.BASS_ChannelSeconds2Bytes(BassStream, 0));
            BassMix.BASS_Mixer_ChannelSetPosition(BassMixer, Bass.BASS_ChannelSeconds2Bytes(BassStream, 0));

            // Get the audio levels of the OGG file.
            while ( Bass.BASS_ChannelGetLevel(BassMixer, level, 1, BASSLevel.BASS_LEVEL_STEREO) )
            {
                double levelDouble = Convert.ToDouble(level.Max());

                // Translate the level to dB.
                double dblevel = (levelDouble > 0 ? 20 * Math.Log10(levelDouble) : -int.MaxValue);

                // We want to not use any section of the song that is too quiet in our checking.
                // This also has the added benefit of ignoring any silence in the song.
                if (dblevel > silenceLevel)
                {
                    dBLevels.Add(dblevel);
                }

                //DebugLog(dblevel.ToString());

            }

            //Log($"Levels: {dBLevels.Count()}");

            if (dBLevels.Count <= 0)
            {
                throw new Exception("No dB Levels found!");
            }

            // Remove the beginning of the song
            dBLevels.RemoveRange(0, secondsToRemoveFromStart);

            // Sort by volume
            dBLevels.Sort();

            // Calculate and remove the amount of levels requested.
            int countToKeep = (int)Math.Floor(dBLevels.Count() * (topPercentToKeep * 0.01));
            int countToStrip = (int)Math.Floor(dBLevels.Count() * (topPercentToStrip * 0.01));

            dBLevels.RemoveRange(0, dBLevels.Count() - countToKeep);
            dBLevels.RemoveRange(dBLevels.Count() - countToStrip, countToStrip);

            /*
            foreach (var item in dBLevels)
            {
                Log(item.ToString());
            }
            */

            double dBAverage = dBLevels.Average();

            return dBAverage;
        }

        private void WriteDTA()
        {
            if (chkAnalyzerMode.Checked == true) return;

            // We write the DTA, and patch in the changed volume numbers on the fly.
            Log("Writing changes to DTA...");

            var dta = Path.GetTempPath() + "temp_dta.txt";

            Tools.DeleteFile(dta);

            if (!Parser.WriteDTAToFile(dta))
            {
                throw new Exception("Error writing temporary DTA file!");
            }

            var dtaLines = new List<string>();
            var dtaLinesNew = new List<string>();

            using (var streamReader = new StreamReader(dta))
            {
                int indexOfVol = 100;
                int indexOfEnd = 0;
                bool valuesExist = false;
                bool hashExists = false;
                bool hasNormalizedAsAlbum = false;

                int indexToWrite = 0;
                int lastClose = 0;

                do
                {
                    dtaLines.Add(streamReader.ReadLine());
                } while (!streamReader.EndOfStream);

                // We want to check if the original values exist before we write them later.
                foreach (var item in dtaLines)
                {
                    if (item.Contains(";OriginalAttenuationValues="))
                    {
                        valuesExist = true;
                    }

                    if (item.Contains(";VolumeNormalizerAudioHash="))
                    {
                        hashExists = true;
                    }

                    if (item.Contains(";NormalizedAsAlbum="))
                    {
                        hasNormalizedAsAlbum = true;
                    }
                        
                }

                // Go through all of the lines of the DTA
                for (int i = 0; i < dtaLines.Count; i++)
                {
                    var line = dtaLines[i];

                    if (line.Contains("'vols'"))
                    {
                        if (!line.Contains("(vols"))
                        {
                            // We don't need this line, we need the next one.
                            dtaLinesNew.Add(line);
                            i++;
                            line = dtaLines[i];
                        }

                        // Find when the volume starts.
                        for (int j = 0; j < line.Length; j++)
                        {
                            if (Char.IsDigit(line[j]) && j < indexOfVol)
                            {
                                indexOfVol = j;
                            }
                        }

                        // If the first number is negative, we want that.
                        if (line[indexOfVol - 1] == '-')
                        {
                            indexOfVol--;
                        }

                        // Find the closing )
                        indexOfEnd = line.IndexOf(')');

                        // Write the modified line.
                        line = line.Substring(0, indexOfVol) + attenuationValues + line.Substring(indexOfEnd);

                    }

                    // Write line
                    dtaLinesNew.Add(line);

                    // We write our own line to the DTA file here.
                    if (line.Contains(";ExpertOnly"))
                    {
                        indexToWrite = i + 1;
                    }

                    if (line.Contains(")"))
                    {
                        if (line.Trim() == ")")
                        {
                            lastClose = i;
                        }
                    }
                }

                if (indexToWrite == 0)
                {
                    indexToWrite = lastClose;
                }

                if (!valuesExist)
                {
                    dtaLinesNew.Insert(indexToWrite, ";OriginalAttenuationValues=" + Parser.Songs[0].OriginalAttenuationValues.Trim());
                }
                if (!hashExists && audioModified)
                {
                    dtaLinesNew.Insert(indexToWrite, ";VolumeNormalizerAudioHash=" + audioHash);
                }
                if (!hasNormalizedAsAlbum)
                {
                    dtaLinesNew.Insert(indexToWrite, ";NormalizedAsAlbum=" + (chkAlbumMode.Checked ? "1" : "0"));
                } else
                {
                    int idx = dtaLinesNew.FindIndex(x => x.StartsWith(";NormalizedAsAlbum="));
                    dtaLinesNew.RemoveAt(idx);
                    dtaLinesNew.Insert(idx, ";NormalizedAsAlbum=" + (chkAlbumMode.Checked ? "1" : "0"));
                }

                    streamReader.Close();
            }

            // Actually output the file.
            var streamWriter = new StreamWriter(dta, false);
            using (streamWriter)
            {
                foreach (var line in dtaLinesNew)
                {
                    streamWriter.WriteLine(line);
                }
                streamWriter.Close();
            }
        }

        private bool ProcessFiles()
        {
            var counter = 0;
            var success = 0;
            Dictionary<string, double> dBSongLevels = new Dictionary<string, double>();

            foreach (var file in inputFiles.Where(File.Exists).TakeWhile(file => !backgroundWorker1.CancellationPending))
            {
                try
                {
                    if (VariousFunctions.ReadFileType(file) != XboxFileType.STFS) continue;

                    try
                    {

                        counter++;

                        // Make sure the file is sane before we work with it.

                        Parser.ExtractDTA(file);
                        Parser.ReadDTA(Parser.DTA);

                        if (Parser.Songs.Count > 1)
                        {
                            Log("File " + Path.GetFileName(file) + " is a pack, try dePACKing first, skipping...");
                            continue;
                        }
                        if (!Parser.Songs.Any())
                        {
                            Log("There was an error processing the songs.dta file");
                            continue;
                        }

                        xPackage = new STFSPackage(file);

                        if (!xPackage.ParseSuccess)
                        {
                            throw new Exception("Failed to parse '" + Path.GetFileName(file) + "'");
                        }

                        // Start the process.
                        Log("");
                        Log($" - Song #{counter} is: {Parser.Songs[0].Artist} - {Parser.Songs[0].Name}");

                        GetAudioFromFile();

                        if (chkRestore.Checked)
                        {
                            // Restore attenuation values to the original levels if requested
                            attenuationValues = Parser.Songs[0].OriginalAttenuationValues.Trim();

                            Log("Restoring original volume levels for the song...");

                            CleanUp();
                            WriteDTA();
                            WriteCON(file);

                            success++;
                            continue;

                        }


                        Log("ProcessFiles(): calculating MOGG average");
                        InitAudio();
                        dBAverage = CalculateVolumeAverage();


                        if (chkAnalyzerMode.Checked == false && chkAlbumMode.Checked == false)
                        {
                            ApplyOffset(file);
                        }
                        else
                        {
                            Log($"Average dB of song is: {FormatDB(dBAverage)} dB.");
                            dBSongLevels.Add(file, dBAverage);
                            CleanUp();
                        }

                        success++;

                    }
                    catch (Exception ex)
                    {
                        Log("There was an error: " + ex.Message);
                        
                        if (Bass.BASS_ErrorGetCode() != BASSError.BASS_OK)
                        {
                            Log("BASS.NET Error:\n" + Bass.BASS_ErrorGetCode());
                        }
                        if (Debug)
                        {
                            backgroundWorker1.CancelAsync();
                        }

                        CleanUp();

                    }
                }
                catch (Exception ex)
                {
                    Log("There was a problem accessing that file");
                    Log("The error says: " + ex.Message);
                    if (Bass.BASS_ErrorGetCode() != BASSError.BASS_OK)
                    {
                        Log("BASS.NET Error:\n" + Bass.BASS_ErrorGetCode());
                    }
                    CleanUp();
                }
            }

            Log("");
            Log($"Successfully processed {success} of {counter} files");

            if (success > 0 && (chkAnalyzerMode.Checked || chkAlbumMode.Checked))
            {
                Log($"Average dB of {success} files is: {FormatDB(dBSongLevels.Values.Average())} dB.");
            }

            Log("");

            if (success > 0 && (chkAlbumMode.Checked && !chkAnalyzerMode.Checked))
            {
                counter = 0;
                success = 0;
                double averageOffset = targetDB - dBSongLevels.Values.Average();
                Log($"Now applying offset of {FormatDB(averageOffset)} dB to all files...");

                foreach (var file in dBSongLevels.Keys.Where(File.Exists).TakeWhile(file => !backgroundWorker1.CancellationPending))
                {
                    try
                    {
                        counter++;
                        // this should never be true, because we already checked its validity on the first pass where we got this song's volume
                        // if it failed, it should never have had the opportunity to be added to dBSongLevels
                        if (VariousFunctions.ReadFileType(file) != XboxFileType.STFS) continue;

                        Parser.ExtractDTA(file);
                        Parser.ReadDTA(Parser.DTA);

                        Log("");
                        Log($" - Song #{counter} is: {Parser.Songs[0].Artist} - {Parser.Songs[0].Name}");

                        ApplyOffset(file, averageOffset);
                        success++;
                    }
                    catch (Exception ex)
                    {
                        Log("There was a problem accessing that file");
                        Log("The error says: " + ex.Message);
                        if (Bass.BASS_ErrorGetCode() != BASSError.BASS_OK)
                        {
                            Log("BASS.NET Error:\n" + Bass.BASS_ErrorGetCode());
                        }
                        CleanUp();
                    }
                }

                Log($"Successfully applied album volume offset to {success} of {counter} files");
            }

            CleanUp();
            return true;
        }



        #region Modified Nautilus Code

        // Here lies code that is basically identical to the original code in Nautilus, 
        // with slight tweaks that were needed for it to work the way I needed it to.

        private void WriteCON(string con)
        {
            if (chkAnalyzerMode.Checked == true) return;

            // This function is prety much the same steps as the QuickDTAEditor.
            // The only real difference is that we try to continue with other files
            // instead of aborting everything on a problem.

            Tools.CurrentFolder = Path.GetDirectoryName(con);

            var dta = Path.GetTempPath() + "temp_dta.txt";
            var backup = con + "_backup";
            string internal_name = Parser.Songs[0].InternalName;

            Tools.DeleteFile(backup);

            DebugLog("Backing up file before modifying it...");
            File.Copy(con, backup);

            // Make a new package
            var song = new STFSPackage(con);
            var xDTA = song.GetFile("/songs/songs.dta");

            if (!xDTA.Replace(dta))
            {
                Tools.DeleteFile(con);
                DebugLog(Tools.MoveFile(backup, con) ? "Restored from backup." : "Failed to restore from backup!");
                throw new Exception("Error replacing DTA file with modified one!");
            }
            Log("Replaced DTA file successfully!");

            if ( audioModified )
            {

                var xMOGG = song.GetFile($"songs/{internal_name}/{internal_name}.mogg");
                var mogg = Path.GetTempPath() + "\\" + "mogg_adjust";

                if (chkRestore.Checked)
                {
                    mogg = Path.GetTempPath() + "\\" + "mogg";
                }
                
                if (!xMOGG.Replace(mogg))
                {
                    Tools.DeleteFile(con);
                    DebugLog(Tools.MoveFile(backup, con) ? "Restored from backup." : "Failed to restore from backup!");
                    throw new Exception("Error replacing MOGG file with modified one!");
                }
                Log("Replaced MOGG file successfully!");

            }

            song.Header.MakeAnonymous();
            song.Header.ThisType = PackageType.SavedGame;

            Log("Saving changes to file...");
            RSAParams signature = new RSAParams(Application.StartupPath + "\\bin\\KV.bin");
            song.RebuildPackage(signature);
            song.FlushPackage(signature);
            song.CloseIO();

            if (!Tools.UnlockCON(con))
            {
                Tools.DeleteFile(con);
                DebugLog(Tools.MoveFile(backup, con) ? "Restored from backup." : "Failed to restore from backup!");
                throw new Exception("Failed to unlock CON file!");
            }

            if (!Tools.SignCON(con))
            {
                Tools.DeleteFile(con);
                DebugLog(Tools.MoveFile(backup, con) ? "Restored from backup." : "Failed to restore from backup!");
                throw new Exception("Failed to sign CON file!");
            }

            Tools.DeleteFile(dta);

            Log("Process completed successfully! Removing backup...");
            Tools.DeleteFile(backup);

            return;

        }

        private float[,] DoVolumeAdjust(SongData song, float[,] in_matrix, IList<int> ArrangedChannels, int inst_channels, int curr_channel)
        {
            // This is a copy of the Matrix function, changed to use a volume input.
            DebugLog("DoVolumeAdjust");

            //initialize output matrix based on input matrix, just in case something fails there's something going out
            var matrix = in_matrix;

            //split attenuation and panning info from DTA file for index access
            string[] volumes = new string[song.ChannelsTotal];
            try
            {
                volumes = song.OriginalAttenuationValues.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < volumes.Length; i++)
                {
                    volumes[i] = (double.Parse(volumes[i], CultureInfo.InvariantCulture) + volumeOffset).ToString("F", CultureInfo.InvariantCulture);
                    //DebugLog($"{i}: {volumes[i]}");
                }
                DebugLog($"Testing using DTA vals: {string.Join(" ", volumes)}");
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
            //level in this case refers to a linear scale, rather than the dB value (logarithmic)
            float vol;
            try
            {
                vol = (float)Utils.DBToLevel(Convert.ToDouble(volumes[curr_channel], CultureInfo.InvariantCulture), max_dB);
                //Log($"vol:{vol}");
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

        public float[,] GetChannelMatrix_VolumeAdjust(SongData song, int inputChans, string stems, int outputChans = 2, bool isOgg = true)
        {
            // Slight tweak to the function to call volume functions instead. 
            DebugLog("GetChannelMatrix_VolumeAdjust");

            //initialize matrix
            //matrix must be float[output_channels, input_channels]
            var matrix = new float[outputChans, inputChans];
            var ArrangedChannels = moggSplitter.ArrangeStreamChannels(inputChans, isOgg);
            if (song.ChannelsDrums > 0 && (stems.Contains("drums") || stems.Contains("allstems")))
            {
                DebugLog("Modifying drums vols");
                //for drums it's a bit tricky because of the possible combinations
                switch (song.ChannelsDrums)
                {
                    case 2:
                        //stereo kit
                        matrix = DoVolumeAdjust(song, matrix, ArrangedChannels, 2, 0);
                        break;
                    case 3:
                        //mono kick
                        matrix = DoVolumeAdjust(song, matrix, ArrangedChannels, 1, 0);
                        //stereo kit
                        matrix = DoVolumeAdjust(song, matrix, ArrangedChannels, 2, 1);
                        break;
                    case 4:
                        //mono kick
                        matrix = DoVolumeAdjust(song, matrix, ArrangedChannels, 1, 0);
                        //mono snare
                        matrix = DoVolumeAdjust(song, matrix, ArrangedChannels, 1, 1);
                        //stereo kit
                        matrix = DoVolumeAdjust(song, matrix, ArrangedChannels, 2, 2);
                        break;
                    case 5:
                        //mono kick
                        matrix = DoVolumeAdjust(song, matrix, ArrangedChannels, 1, 0);
                        //stereo snare
                        matrix = DoVolumeAdjust(song, matrix, ArrangedChannels, 2, 1);
                        //stereo kit
                        matrix = DoVolumeAdjust(song, matrix, ArrangedChannels, 2, 3);
                        break;
                    case 6:
                        //stereo kick
                        matrix = DoVolumeAdjust(song, matrix, ArrangedChannels, 2, 0);
                        //stereo snare
                        matrix = DoVolumeAdjust(song, matrix, ArrangedChannels, 2, 2);
                        //stereo kit
                        matrix = DoVolumeAdjust(song, matrix, ArrangedChannels, 2, 4);
                        break;
                }
            }
            //var channel = song.ChannelsDrums;
            if (song.ChannelsBass > 0 && (stems.Contains("bass") || stems.Contains("allstems")))
            {
                DebugLog("Modifying bass vols");
                matrix = DoVolumeAdjust(song, matrix, ArrangedChannels, song.ChannelsBass, song.ChannelsBassStart);//channel);
            }
            //channel = channel + song.ChannelsBass;
            if (song.ChannelsGuitar > 0 && (stems.Contains("guitar") || stems.Contains("allstems")))
            {
                DebugLog("Modifying guitar vols");
                matrix = DoVolumeAdjust(song, matrix, ArrangedChannels, song.ChannelsGuitar, song.ChannelsGuitarStart);//channel);
            }
            //channel = channel + song.ChannelsGuitar;
            if (song.ChannelsVocals > 0 && (stems.Contains("vocals") || stems.Contains("allstems")))
            {
                DebugLog("Modifying vocals vols");
                matrix = DoVolumeAdjust(song, matrix, ArrangedChannels, song.ChannelsVocals, song.ChannelsVocalsStart);//channel);
            }
            //channel = channel + song.ChannelsVocals;
            if (song.ChannelsKeys > 0 && (stems.Contains("keys") || stems.Contains("allstems")))
            {
                DebugLog("Modifying keys vols");
                matrix = DoVolumeAdjust(song, matrix, ArrangedChannels, song.ChannelsKeys, song.ChannelsKeysStart);//channel);
            }
            //channel = channel + song.ChannelsKeys;
            if (song.ChannelsCrowd > 0 && !stems.Contains("NOcrowd") && (stems.Contains("crowd") || stems.Contains("allstems")))
            {
                DebugLog("Modifying crowd vols");
                matrix = DoVolumeAdjust(song, matrix, ArrangedChannels, song.ChannelsCrowd, song.ChannelsCrowdStart);//channel);
            }
            //channel = channel + song.ChannelsCrowd;
            if ((stems.Contains("backing") || stems.Contains("allstems"))) //song.ChannelsBacking > 0 &&  ---- should always be enabled per specifications
            {
                var backing = song.ChannelsTotal - song.ChannelsBass - song.ChannelsDrums - song.ChannelsGuitar - song.ChannelsKeys - song.ChannelsVocals - song.ChannelsCrowd;
                if (backing > 0) //backing not required 
                {
                    DebugLog("Modifying backing vols");
                    if (song.ChannelsCrowdStart + song.ChannelsCrowd == song.ChannelsTotal) //crowd channels are last
                    {
                        matrix = DoVolumeAdjust(song, matrix, ArrangedChannels, backing, song.ChannelsCrowdStart - backing);//channel);                        
                    }
                    else
                    {
                        matrix = DoVolumeAdjust(song, matrix, ArrangedChannels, backing, song.ChannelsTotal - backing);//channel);
                    }
                }
            }
            return matrix;
        }
        #endregion

        #region Form Things

        private void txtFolder_TextChanged(object sender, EventArgs e)
        {
            if (picWorking.Visible) return;
            inputFiles.Clear();

            if (string.IsNullOrWhiteSpace(txtFolder.Text))
            {
                btnRefresh.Visible = false;
            }
            btnRefresh.Visible = true;

            if (txtFolder.Text != "")
            {
                Tools.CurrentFolder = txtFolder.Text;
                Log("");
                Log("Reading input directory ... hang on");

                try
                {
                    var inFiles = Directory.GetFiles(txtFolder.Text);
                    foreach (var file in inFiles)
                    {
                        try
                        {
                            if (VariousFunctions.ReadFileType(file) == XboxFileType.STFS)
                            {
                                inputFiles.Add(file);
                            }
                        }
                        catch (Exception ex)
                        {
                            if (Path.GetExtension(file) != "") continue;
                            Log("There was a problem accessing file " + Path.GetFileName(file));
                            Log("The error says: " + ex.Message);
                        }
                    }
                    if (!inputFiles.Any())
                    {
                        Log("Did not find any CON files ... try a different directory");
                        Log("You can also drag and drop CON files or a folder containing CON files here");
                        Log("Ready");
                        btnBegin.Visible = false;
                        btnRefresh.Visible = true;
                    }
                    else
                    {
                        Log("Found " + inputFiles.Count + " CON " + (inputFiles.Count > 1 ? "files" : "file"));
                        Log("Ready to begin");
                        btnBegin.Visible = true;
                        btnRefresh.Visible = true;
                    }
                }
                catch (Exception ex)
                {
                    Log("There was an error: " + ex.Message);
                }
            }
            else
            {
                btnBegin.Visible = false;
                btnRefresh.Visible = false;
            }
            txtFolder.Focus();
        }

        private void btnFolder_Click(object sender, EventArgs e)
        {
            //if user selects new folder, assign that value
            //if user cancels or selects same folder, this forces the text_changed event to run again
            var tFolder = txtFolder.Text;

            var folderUser = new FolderBrowserDialog
            {
                SelectedPath = txtFolder.Text,
                Description = "Select the folder where your CON files are",
            };
            txtFolder.Text = "";
            var result = folderUser.ShowDialog();
            txtFolder.Text = result == DialogResult.OK ? folderUser.SelectedPath : tFolder;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            var tFolder = txtFolder.Text;
            txtFolder.Text = "";
            txtFolder.Text = tFolder;
        }

        private void HandleDragDrop(object sender, DragEventArgs e)
        {
            if (picWorking.Visible) return;
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (btnReset.Visible)
            {
                btnReset.PerformClick();
            }
            if (Directory.Exists(files[0])) {
                txtFolder.Text = Path.GetFullPath(files[0]);
                Tools.CurrentFolder = txtFolder.Text;
            } 
            else if (VariousFunctions.ReadFileType(files[0]) == XboxFileType.STFS)
            {
                txtFolder.Text = Path.GetDirectoryName(files[0]);
                Tools.CurrentFolder = txtFolder.Text;
            }
            else
            {
                MessageBox.Show("That's not a valid file to drop here", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var message = Tools.ReadHelpFile("vn");
            var help = new HelpForm(Text + " - Help", message);
            help.ShowDialog();
        }

        private void exportLogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.ExportLog(Text, lstLog.Items);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            lstLog.Items.Clear();
            Log("Resetting...");
            inputFiles.Clear();
            EnableDisable(true);
            btnBegin.Visible = true;
            btnBegin.Enabled = true;
            btnReset.Visible = false;
            btnFolder.Enabled = true;
            chkAlbumMode.Enabled = !chkRestore.Checked;
            btnRefresh.Enabled = true;
            btnRefresh.PerformClick();
            lblResetVolEnabled.Visible = chkRestore.Checked;
        }

        private void EnableDisable(bool enabled)
        {
            btnFolder.Enabled = enabled;
            btnRefresh.Enabled = enabled;
            menuStrip1.Enabled = enabled;
            txtFolder.Enabled = enabled;
            picWorking.Visible = !enabled;
            lblResetVolEnabled.Visible = enabled && chkRestore.Checked;
            lstLog.Cursor = enabled ? Cursors.Default : Cursors.WaitCursor;
            Cursor = lstLog.Cursor;
            chkRestore.Enabled = enabled;
            radioAllowRender.Enabled = enabled;
            radioDoNotRender.Enabled = enabled;
            numTargetValue.Enabled = enabled;
            numThresholdValue.Enabled = enabled;
            if (chkRestore.Checked)
            {
                chkAlbumMode.Enabled = false;
            }
            else
            {
                chkAlbumMode.Enabled = enabled;
            }
        }

        private void btnBegin_Click(object sender, EventArgs e)
        {

            if (btnBegin.Text == "Cancel")
            {
                backgroundWorker1.CancelAsync();
                Log("User cancelled process... Stopping as soon as possible.");
                btnBegin.Enabled = false;
                return;
            }
            else
            {
                if (chkAnalyzerMode.Checked == false)
                {
                    string _tempPrepString = "This will modify all of the CON files in this folder.";

                    if (radioAllowRender.Checked)
                    {
                        _tempPrepString += "\n\nIf the volume needs to be increased, audio will be modified!";
                    }

                    if (MessageBox.Show(_tempPrepString, "Are you sure you want to proceed?",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        return;
                    }
                }
            }

            startTime = DateTime.Now;
            Tools.CurrentFolder = txtFolder.Text;
            EnableDisable(false);
            Debug = chkVerboseOutput.Checked;

            targetDB = double.Parse(numTargetValue.Value.ToString());
            thresholdDB = double.Parse(numThresholdValue.Value.ToString());

            DebugLog($"Culture: {CultureInfo.CurrentCulture}");

            try
            {
                var files = Directory.GetFiles(txtFolder.Text);
                if (files.Count() != 0)
                {
                    btnBegin.Text = "Cancel";
                    toolTip1.SetToolTip(btnBegin, "Click to cancel process");
                    backgroundWorker1.RunWorkerAsync();
                }
                else
                {
                    Log("No files found... There's nothing to do");
                    EnableDisable(true);
                }
            }
            catch (Exception ex)
            {
                Log("Error retrieving files to process");
                Log("The error says:" + ex.Message);
                EnableDisable(true);
            }
        }

        private void HandleDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
        }

        private void VolumeNormalizerPrep_Resize(object sender, EventArgs e)
        {
            btnRefresh.Left = txtFolder.Left + txtFolder.Width - btnRefresh.Width;
            btnBegin.Left = txtFolder.Left + txtFolder.Width - btnBegin.Width;
            chkAlbumMode.Left = Math.Max((int)(txtFolder.Left + (txtFolder.Width / 2) - (chkAlbumMode.Width / 2)), (int)(txtFolder.Left + btnFolder.Width + 8));
            lblResetVolEnabled.Left = txtFolder.Left + (txtFolder.Width / 2) - (lblResetVolEnabled.Width / 2);
            picWorking.Left = (Width / 2) - (picWorking.Width / 2);
        }

        private void VolumeNormalizerPrep_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!picWorking.Visible)
            {
                //Tools.DeleteFolder(Application.StartupPath + "\\phaseshift\\");
                return;
            }
            MessageBox.Show("Please wait until the current process finishes", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            e.Cancel = true;
        }

        private void VolumeNormalizerPrep_Shown(object sender, EventArgs e)
        {
            Log("Welcome to " + Text);
            Log("Drag and drop the CON / LIVE file(s) to be processed here");
            Log("Or click 'Change Input Folder' to select the files");
            Log("Ready to begin");
            txtFolder.Text = inputDir;
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (ProcessFiles()) return;
            Log("There was an error processing the files... Stopping here.");
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            Log("Done!");
            endTime = DateTime.Now;
            var timeDiff = endTime - startTime;
            Log("Process took " + timeDiff.Minutes + (timeDiff.Minutes == 1 ? " minute" : " minutes") + " and " + (timeDiff.Minutes == 0 && timeDiff.Seconds == 0 ? "1 second" : timeDiff.Seconds + " seconds"));
            Log("Click 'Reset' to start again or just close me down");

            btnReset.Enabled = true;
            btnReset.Visible = true;
            picWorking.Visible = false;
            lblResetVolEnabled.Visible = chkRestore.Checked;
            lstLog.Cursor = Cursors.Default;
            Cursor = lstLog.Cursor;
            toolTip1.SetToolTip(btnBegin, "Click to begin");
            btnBegin.Text = "&Begin";
        }

        private void picPin_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            switch (picPin.Tag.ToString())
            {
                case "pinned":
                    picPin.Image = Resources.unpinned;
                    picPin.Tag = "unpinned";
                    break;
                case "unpinned":
                    picPin.Image = Resources.pinned;
                    picPin.Tag = "pinned";
                    break;
            }
            TopMost = picPin.Tag.ToString() == "pinned";
        }

        private void chkBackupAudio_Click(object sender, EventArgs e)
        {
            if (chkBackupAudio.Checked == false)
            {
                string _tempPrepString = "This will disable backing up the audio file if audio needs to be re-rendered!";
                _tempPrepString += "\n\nThe Volume Normalizer will refuse to work on modified audio, are you sure you want to do this?";

                if (MessageBox.Show(_tempPrepString, "Volume Normalizer",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    chkBackupAudio.Checked = true;
                }
            }
        }

        private void chkRestore_Click(object sender, EventArgs e)
        {
            if (chkRestore.Checked)
            {
                chkAlbumMode.Checked = false;
                chkAlbumMode.Enabled = false;
            } else
            {
                chkAlbumMode.Enabled = true;
            }
            lblResetVolEnabled.Visible = chkRestore.Checked;
        }

        #endregion
    }

}