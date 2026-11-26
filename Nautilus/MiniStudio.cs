using System;
using System.Drawing;
using System.Windows.Forms;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Enc;
using System.IO;
using System.Diagnostics;
using Un4seen.Bass.Misc;
using Un4seen.Bass.AddOn.Mix;

namespace Nautilus
{
    public partial class MiniStudio : Form
    {
        private readonly Bitmap switch_on;
        private readonly Bitmap switch_off;
        private readonly Bitmap volume_bg;
        private readonly Bitmap volume_slider;
        private readonly NemoTools Tools;
        private const int BassBuffer = 1000;
        private double songLength;
        private double songPosition;
        private int BassStream;
        private int BassMixer;
        private string inputFile;
        private string outputFile;
        private const string AppName = "Mini Studio";
        private bool SaveToFile;
        private bool wasPlaying;
        private int defaultVolume;
        private float newVolume = 1.0f;

        public MiniStudio()
        {
            InitializeComponent();
            Tools = new NemoTools();
            switch_on = (Bitmap)Tools.NemoLoadImage(Application.StartupPath + "\\res\\switch_on.png");
            switch_off = (Bitmap)Tools.NemoLoadImage(Application.StartupPath + "\\res\\switch_off.png");
            volume_bg = (Bitmap)Tools.NemoLoadImage(Application.StartupPath + "\\res\\vol_bg.png");
            volume_slider = (Bitmap)Tools.NemoLoadImage(Application.StartupPath + "\\res\\vol_slider.png");
        }


        private void EnableDisableChorus()
        {
            if (switchChorus.Tag.Equals("1"))
            {
                switchChorus.Image = switch_on;
                grpChorus.Enabled = true;
            }
            else
            {
                switchChorus.Image = switch_off;
                grpChorus.Enabled = false;
            }
        }

        private void switchChorus_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) { return; }

            if (switchChorus.Tag.Equals("0"))
            {
                switchChorus.Tag = "1";
            }
            else
            {
                switchChorus.Tag = "0";
            }
            EnableDisableChorus();
        }

        private void EnableDisableEcho()
        {
            if (switchEcho.Tag.Equals("1"))
            {
                switchEcho.Image = switch_on;
                grpEcho.Enabled = true;
            }
            else
            {
                switchEcho.Image = switch_off;
                grpEcho.Enabled = false;
            }
        }

        private void EnableDisableGargle()
        {
            if (switchGargle.Tag.Equals("1"))
            {
                switchGargle.Image = switch_on;
                grpGargle.Enabled = true;
            }
            else
            {
                switchGargle.Image = switch_off;
                grpGargle.Enabled = false;
            }
        }

        private void switchEcho_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) { return; }

            if (switchEcho.Tag.Equals("0"))
            {
                switchEcho.Tag = "1";
            }
            else
            {
                switchEcho.Tag = "0";
            }
            EnableDisableEcho();
        }

        private void EnableDisableReverb()
        {
            if (switchReverb.Tag.Equals("1"))
            {
                switchReverb.Image = switch_on;
                grpReverb.Enabled = true;
            }
            else
            {
                switchReverb.Image = switch_off;
                grpReverb.Enabled = false;
            }
        }

        private void EnableDisableCompression()
        {
            if (switchCompression.Tag.Equals("1"))
            {
                switchCompression.Image = switch_on;
                grpCompression.Enabled = true;
            }
            else
            {
                switchCompression.Image = switch_off;
                grpCompression.Enabled = false;
            }
        }

        private void EnableDisableDistortion()
        {
            if (switchDistortion.Tag.Equals("1"))
            {
                switchDistortion.Image = switch_on;
                grpDistortion.Enabled = true;
            }
            else
            {
                switchDistortion.Image = switch_off;
                grpDistortion.Enabled = false;
            }
        }

        private void EnableDisableFlanger()
        {
            if (switchFlanger.Tag.Equals("1"))
            {
                switchFlanger.Image = switch_on;
                grpFlanger.Enabled = true;
            }
            else
            {
                switchFlanger.Image = switch_off;
                grpFlanger.Enabled = false;
            }
        }

        private void switchReverb_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) { return; }

            if (switchReverb.Tag.Equals("0"))
            {
                switchReverb.Tag = "1";
            }
            else
            {
                switchReverb.Tag = "0";
            }
            EnableDisableReverb();
        }

        private int GetVolumeLocation(int yPos)
        {
            if (yPos == 0)
            {
                return picVolBackground.Top + (picVolBackground.Height / 2) - (picVolSlider.Height / 2);
            }
            else
            {
                return picVolBackground.Top - yPos - (picVolSlider.Height / 2);
            }
        }

        private void Vocalizer_Shown(object sender, EventArgs e)
        {            
            picWorking.Left = (Width - picWorking.Width) / 2;

            picVolBackground.Image = volume_bg;            
            picVolSlider.Image = volume_slider;
            defaultVolume = GetVolumeLocation(0);
            picVolSlider.Top = defaultVolume;

            switchChorus.Image = switch_off;
            switchEcho.Image = switch_off;
            switchReverb.Image = switch_off;
            switchCompression.Image = switch_off;
            switchDistortion.Image = switch_off;
            switchFlanger.Image = switch_off;
            switchGargle.Image = switch_off;

            switchChorus.Tag = "0";
            switchEcho.Tag = "0";
            switchReverb.Tag = "0";
            switchCompression.Tag = "0";
            switchDistortion.Tag = "0";
            switchFlanger.Tag = "0";
            switchGargle.Tag = "0";

            if (!InitializeBASS())
            {
                this.Close();
            }
        }

        private bool InitializeBASS()
        {
            //initialize BASS.NET
            if (!Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, Handle))
            {
                MessageBox.Show("Error initializing BASS.NET\n" + Bass.BASS_ErrorGetCode() + "\n\nCan't do anything until this is resolved", AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_BUFFER, BassBuffer);
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD, 50);
            return true;
        }

        private void Vocalizer_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
        }

        private void Vocalizer_DragDrop(object sender, DragEventArgs e)
        {
            if (effectsWorker.IsBusy) return;

            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (!File.Exists(files[0])) return;
            if (picWorking.Visible)
            {
                MessageBox.Show("Please finish the current task first", AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            var ext = Path.GetExtension(files[0]);
            switch (ext)
            {
                case ".mp3":
                case ".ogg":
                case ".wav":
                    break;
                case ".fx":
                    ImportPreset(files[0]);
                    break;
                default:
                    MessageBox.Show("Unsupported input file\n\nOnly .mp3, .ogg and .wav are supported", AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
            }

            BassStream = Bass.BASS_StreamCreateFile(files[0], 0, File.ReadAllBytes(files[0]).Length, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);
            if (BassStream == 0)
            {
                MessageBox.Show("Error processing that file:\n\n" + Bass.BASS_ErrorGetCode().ToString(), AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Bass.BASS_StreamFree(BassStream);
                return;
            }

            var channel_info = Bass.BASS_ChannelGetInfo(BassStream);
            var channels = channel_info.chans;
            if (channels > 2)
            {
                MessageBox.Show("Unsupported number of input channels\nOnly mono or stereo tracks allowed", AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Bass.BASS_StreamFree(BassStream);
                return;
            }
            Bass.BASS_StreamFree(BassStream);

            //good to go
            inputFile = files[0];
            songPosition = 0;
            outputFile = Path.GetDirectoryName(inputFile) + "\\" + Path.GetFileNameWithoutExtension(inputFile) + ".wav";
            if (inputFile == outputFile)
            {
                outputFile = outputFile.Replace(".wav", "_edit.wav");
            }
            Text = AppName + " - " + inputFile;

            SaveToFile = false;
            EnableDisable(false);
            effectsWorker.RunWorkerAsync();
        }

        private string getConfigValue(string line)
        {
            var index = line.IndexOf("=", 0) + 1;
            return line.Substring(index, line.Length - index);
        }

        private void ImportPreset(string preset)
        {
            var sr = new StreamReader(preset);
            try
            {
                sr.ReadLine();//skip first line

                switchChorus.Tag = getConfigValue(sr.ReadLine());
                chorusDelay.Value = Convert.ToDecimal(getConfigValue(sr.ReadLine()));
                chorusDepth.Value = Convert.ToDecimal(getConfigValue(sr.ReadLine()));
                chorusFeedback.Value = Convert.ToDecimal(getConfigValue(sr.ReadLine()));
                chorusFrequency.Value = Convert.ToDecimal(getConfigValue(sr.ReadLine()));
                chorusWetDry.Value = Convert.ToDecimal(getConfigValue(sr.ReadLine()));
                chorusPhase.Value = Convert.ToDecimal(getConfigValue(sr.ReadLine()));
                chorusWaveForm.Value = Convert.ToDecimal(getConfigValue(sr.ReadLine()));

                switchEcho.Tag = getConfigValue(sr.ReadLine());
                echoFeedback.Value = Convert.ToDecimal(getConfigValue(sr.ReadLine()));
                echoLeftDelay.Value = Convert.ToDecimal(getConfigValue(sr.ReadLine()));
                echoRightDelay.Value = Convert.ToDecimal(getConfigValue(sr.ReadLine()));
                EchoWetDry.Value = Convert.ToDecimal(getConfigValue(sr.ReadLine()));
                echoPanDelay.Checked = getConfigValue(sr.ReadLine()).Contains("1");

                switchReverb.Tag = getConfigValue(sr.ReadLine());
                reverbRatio.Value = Convert.ToDecimal(getConfigValue(sr.ReadLine()));
                reverbGain.Value = Convert.ToDecimal(getConfigValue(sr.ReadLine()));
                reverbMix.Value = Convert.ToDecimal(getConfigValue(sr.ReadLine()));
                reverbTime.Value = Convert.ToDecimal(getConfigValue(sr.ReadLine()));

                switchFlanger.Tag = getConfigValue(sr.ReadLine());
                flangerDelay.Value = Convert.ToDecimal(getConfigValue(sr.ReadLine()));
                flangerDepth.Value = Convert.ToDecimal(getConfigValue(sr.ReadLine()));
                flangerFeedback.Value = Convert.ToDecimal(getConfigValue(sr.ReadLine()));
                flangerFrequency.Value = Convert.ToDecimal(getConfigValue(sr.ReadLine()));
                flangerWetDry.Value = Convert.ToDecimal(getConfigValue(sr.ReadLine()));
                flangerPhase.Value = Convert.ToDecimal(getConfigValue(sr.ReadLine()));
                flangerWaveForm.Value = Convert.ToDecimal(getConfigValue(sr.ReadLine()));

                switchDistortion.Tag = getConfigValue(sr.ReadLine());
                distortionEdge.Value = Convert.ToDecimal(getConfigValue(sr.ReadLine()));
                distortionGain.Value = Convert.ToDecimal(getConfigValue(sr.ReadLine()));
                distortionBandwith.Value = Convert.ToDecimal(getConfigValue(sr.ReadLine()));
                distortionFrequency.Value = Convert.ToDecimal(getConfigValue(sr.ReadLine()));
                distortionCutoff.Value = Convert.ToDecimal(getConfigValue(sr.ReadLine()));

                switchCompression.Tag = getConfigValue(sr.ReadLine());
                compressionAttack.Value = Convert.ToDecimal(getConfigValue(sr.ReadLine()));
                compressionGain.Value = Convert.ToDecimal(getConfigValue(sr.ReadLine()));
                compressionPredelay.Value = Convert.ToDecimal(getConfigValue(sr.ReadLine()));
                compressionRatio.Value = Convert.ToDecimal(getConfigValue(sr.ReadLine()));
                compressionRelease.Value = Convert.ToDecimal(getConfigValue(sr.ReadLine()));
                compressionThreshold.Value = Convert.ToDecimal(getConfigValue(sr.ReadLine()));

                switchGargle.Tag = getConfigValue(sr.ReadLine());
                gargleRate.Value = Convert.ToDecimal(getConfigValue(sr.ReadLine()));
                gargleWaveShape.Value = Convert.ToDecimal(getConfigValue(sr.ReadLine()));
            }
            catch (Exception ex)
            {
                sr.Dispose();
                MessageBox.Show("Error importing that preset:\n" + ex.Message, AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            sr.Dispose();

            EnableDisableChorus();
            EnableDisableEcho();
            EnableDisableReverb();
            EnableDisableCompression();
            EnableDisableDistortion();
            EnableDisableFlanger();
            EnableDisableGargle();
        }

        private void ExportPreset(string file)
        {
            var sw = new StreamWriter(file, false);
            sw.WriteLine(Text + " Effects Preset - DO NOT MODIFY MANUALLY");

            sw.WriteLine("ChorusEnabled=" + switchChorus.Tag);
            sw.WriteLine("ChorusDelay=" + chorusDelay.Value);
            sw.WriteLine("ChorusDepth=" + chorusDepth.Value);
            sw.WriteLine("ChorusFeedback=" + chorusFeedback.Value);
            sw.WriteLine("ChorusFrequency=" + chorusFrequency.Value);
            sw.WriteLine("ChorusWetDry=" + chorusWetDry.Value);
            sw.WriteLine("ChorusPhase=" + chorusPhase.Value);
            sw.WriteLine("ChorusWaveForm=" + chorusWaveForm.Value);

            sw.WriteLine("EchoEnabled=" + switchEcho.Tag);
            sw.WriteLine("EchoFeedback=" + echoFeedback.Value);
            sw.WriteLine("EchoLeftDelay=" + echoLeftDelay.Value);
            sw.WriteLine("EchoRightDelay=" + echoRightDelay.Value);
            sw.WriteLine("EchoWetDry=" + EchoWetDry.Value);
            sw.WriteLine("EchoPanDelay=" + (echoPanDelay.Checked ? 1 : 0));

            sw.WriteLine("ReverbEnabled=" + switchReverb.Tag);
            sw.WriteLine("ReverbRatio=" + reverbRatio.Value);
            sw.WriteLine("ReverbGain=" + reverbGain.Value);
            sw.WriteLine("ReverbMix=" + reverbMix.Value);
            sw.WriteLine("ReverbTime=" + reverbTime.Value);

            sw.WriteLine("FlangerEnabled=" + switchFlanger.Tag);
            sw.WriteLine("FlangerDelay=" + flangerDelay.Value);
            sw.WriteLine("FlangerDepth=" + flangerDepth.Value);
            sw.WriteLine("FlangerFeedback=" + flangerFeedback.Value);
            sw.WriteLine("FlangerFrequency=" + flangerFrequency.Value);
            sw.WriteLine("FlangerWetDry=" + flangerWetDry.Value);
            sw.WriteLine("FlangerPhase=" + flangerPhase.Value);
            sw.WriteLine("FlangerWaveForm=" + flangerWaveForm.Value);

            sw.WriteLine("DistortionEnabled=" + switchDistortion.Tag);
            sw.WriteLine("DistortionEdge=" + distortionEdge.Value);
            sw.WriteLine("DistortionGain=" + distortionGain.Value);
            sw.WriteLine("DistortionBandwidth=" + distortionBandwith.Value);
            sw.WriteLine("DistortionFrequency=" + distortionFrequency.Value);
            sw.WriteLine("DistortionCutoff=" + distortionCutoff.Value);

            sw.WriteLine("CompressionEnabled=" + switchCompression.Tag);
            sw.WriteLine("CompressionAttack=" + compressionAttack.Value);
            sw.WriteLine("CompressionGain=" + compressionGain.Value);
            sw.WriteLine("CompressionPredelay=" + compressionPredelay.Value);
            sw.WriteLine("CompressionRatio=" + compressionRatio.Value);
            sw.WriteLine("CompressionRelease=" + compressionRelease.Value);
            sw.WriteLine("CompressionThreshold=" + compressionThreshold.Value);

            sw.WriteLine("GargleEnabled=" + switchGargle.Tag);
            sw.WriteLine("GargleRate=" + gargleRate.Value);
            sw.WriteLine("GargleWaveShape=" + gargleWaveShape.Value);

            sw.Dispose();
        }

        private void ApplyEffects()
        {
            float fChorusDelay = (float)(chorusDelay.Value);
            float fChorusDepth = (float)(chorusDepth.Value);
            float fChorusFeedback = (float)(chorusFeedback.Value);
            float fChorusFrequency = (float)(chorusFrequency.Value);
            float fChorusWetDry = (float)(chorusWetDry.Value);
            int iChorusPhase = (int)(chorusPhase.Value);
            int iChorusWaveForm = (int)(chorusWaveForm.Value);

            float fEchoFeedback = (float)(echoFeedback.Value);
            float fEchoLeftDelay = (float)(echoLeftDelay.Value);
            float fEchoRightDelay = (float)(echoRightDelay.Value);
            float fEchoWetDry = (float)(EchoWetDry.Value);
            bool bEchoPanDelay = echoPanDelay.Checked;

            float fReverbRatio = (float)(reverbRatio.Value);
            float fReverbGain = (float)(reverbGain.Value);
            float fReverbMix = (float)(reverbMix.Value);
            float fReverbTime = (float)(reverbTime.Value);

            float fFlangerDelay = (float)(flangerDelay.Value);
            float fFlangerDepth = (float)(flangerDepth.Value);
            float fFlangerFeedback = (float)(flangerFeedback.Value);
            float fFlangerFrequency = (float)(flangerFrequency.Value);
            float fFlangerWetDry = (float)(flangerWetDry.Value);
            int iFlangerPhase = (int)(flangerPhase.Value);
            int iFlangerWaveForm = (int)(flangerWaveForm.Value);

            float fDistortionEdge = (float)(distortionEdge.Value);
            float fDistortionGain = (float)(distortionGain.Value);
            float fDistortionBandwith = (float)(distortionBandwith.Value);
            float fDistortionFrequency = (float)(distortionFrequency.Value);
            float fDistortionCutofF = (float)(distortionCutoff.Value);

            float fCompressionAttack = (float)(compressionAttack.Value);
            float fCompressionGain = (float)(compressionGain.Value);
            float fCompressionPredelay = (float)(compressionPredelay.Value);
            float fCompressionRatio = (float)(compressionRatio.Value);
            float fCompressionRelease = (float)(compressionRelease.Value);
            float fCompressionThreshold = (float)(compressionThreshold.Value);

            int iGargleRate = (int)(gargleRate.Value);
            int iGargleWaveShape = (int)(gargleWaveShape.Value);

            int fxChorusHandle;
            int fxEchoHandle;
            int fxReverbHandle;
            int fxFlangerHandle;
            int fxDistortionHandle;
            int fxCompressionHandle;
            int fxGargleHandle;
            int fxVolumeHandle;

            //free previous streams just in case
            Bass.BASS_StreamFree(BassStream);
            Bass.BASS_StreamFree(BassMixer);

            BassStream = Bass.BASS_StreamCreateFile(inputFile, 0, File.ReadAllBytes(inputFile).Length, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);
            var len = Bass.BASS_ChannelGetLength(BassStream);
            songLength = Bass.BASS_ChannelBytes2Seconds(BassStream, len); //the total song length
            var channel_info = Bass.BASS_ChannelGetInfo(BassStream);
            // create a stereo mixer with same frequency rate as the input file
            if (SaveToFile)
            {
                BassMixer = BassMix.BASS_Mixer_StreamCreate(channel_info.freq, channel_info.chans, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_MIXER_END);
            }
            else
            {
                BassMixer = BassMix.BASS_Mixer_StreamCreate(channel_info.freq, channel_info.chans, BASSFlag.BASS_MIXER_END);
            }         
            BassMix.BASS_Mixer_StreamAddChannel(BassMixer, BassStream, BASSFlag.BASS_MIXER_MATRIX);
                       
            SongData song = new SongData();
            song.ChannelsDrums = channel_info.chans;//assign to drums, who cares
            song.ChannelsDrumsStart = 0;
            song.OriginalAttenuationValues = "";
            song.AttenuationValues = "";
            song.PanningValues = "";

            var splitter = new MoggSplitter();
            var matrix = splitter.GetChannelMatrix(song, channel_info.chans, "drums", channel_info.chans, Path.GetExtension(inputFile) == ".ogg");
            BassMix.BASS_Mixer_ChannelSetMatrix(BassStream, matrix);         

            if (!SaveToFile) //go back to prior position
            {
                BassMix.BASS_Mixer_ChannelSetPosition(BassStream, Bass.BASS_ChannelSeconds2Bytes(BassStream, songPosition));
            }
                        
            BASS_FX_VOLUME_PARAM volume = new BASS_FX_VOLUME_PARAM(newVolume, 1, 0, 0);
            fxVolumeHandle = Bass.BASS_ChannelSetFX(BassStream, BASSFXType.BASS_FX_VOLUME, 0);
            Bass.BASS_FXSetParameters(fxVolumeHandle, volume);

            if (switchChorus.Tag.Equals("1"))
            {
                BASS_DX8_CHORUS chorus = new BASS_DX8_CHORUS(fChorusWetDry, fChorusDepth, fChorusFeedback, fChorusFrequency, iChorusWaveForm, fChorusDelay, BASSFXPhase.BASS_FX_PHASE_ZERO);
                fxChorusHandle = Bass.BASS_ChannelSetFX(BassStream, BASSFXType.BASS_FX_DX8_CHORUS, 1);
            }

            if (switchEcho.Tag.Equals("1"))
            {
                BASS_DX8_ECHO echo = new BASS_DX8_ECHO(fEchoWetDry, fEchoFeedback, fEchoLeftDelay, fEchoRightDelay, bEchoPanDelay);
                fxEchoHandle = Bass.BASS_ChannelSetFX(BassStream, BASSFXType.BASS_FX_DX8_ECHO, 2);
            }

            if (switchReverb.Tag.Equals("1"))
            {
                BASS_DX8_REVERB reverb = new BASS_DX8_REVERB(fReverbGain, fReverbMix, fReverbTime, fReverbRatio);
                fxReverbHandle = Bass.BASS_ChannelSetFX(BassStream, BASSFXType.BASS_FX_DX8_REVERB, 3);
            }

            if (switchFlanger.Tag.Equals("1"))
            {
                BASS_DX8_FLANGER flanger = new BASS_DX8_FLANGER(fFlangerWetDry, fFlangerDepth, fFlangerFeedback, fFlangerFrequency, iFlangerWaveForm, fFlangerDelay, BASSFXPhase.BASS_FX_PHASE_ZERO);
                fxFlangerHandle = Bass.BASS_ChannelSetFX(BassStream, BASSFXType.BASS_FX_DX8_FLANGER, 4);
            }

            if (switchDistortion.Tag.Equals("1"))
            {
                BASS_DX8_DISTORTION distortion = new BASS_DX8_DISTORTION(fDistortionGain, fDistortionEdge, fDistortionFrequency, fDistortionBandwith, fDistortionCutofF);
                fxDistortionHandle = Bass.BASS_ChannelSetFX(BassStream, BASSFXType.BASS_FX_DX8_DISTORTION, 5);
            }

            if (switchCompression.Tag.Equals("1"))
            {
                BASS_DX8_COMPRESSOR compression = new BASS_DX8_COMPRESSOR(fCompressionGain, fCompressionAttack, fCompressionRelease, fCompressionThreshold, fCompressionRatio, fCompressionPredelay);
                fxCompressionHandle = Bass.BASS_ChannelSetFX(BassStream, BASSFXType.BASS_FX_DX8_COMPRESSOR, 6);
            }

            if (switchGargle.Tag.Equals("1"))
            {
                BASS_DX8_GARGLE gargle = new BASS_DX8_GARGLE(iGargleRate, iGargleWaveShape);
                fxGargleHandle = Bass.BASS_ChannelSetFX(BassStream, BASSFXType.BASS_FX_DX8_GARGLE, 7);
            }            

            if (SaveToFile)
            {
                int encoder = BassEnc.BASS_Encode_Start(BassMixer, outputFile, BASSEncode.BASS_ENCODE_PCM | BASSEncode.BASS_ENCODE_AUTOFREE, null, IntPtr.Zero);
                while (true)
                {
                    var buffer = new byte[20000];
                    var c = Bass.BASS_ChannelGetData(BassMixer, buffer, buffer.Length);
                    if (c <= 0) break;
                }
                BassEnc.BASS_Encode_Stop(encoder);                

                if (File.Exists(outputFile))
                {
                    Process.Start("explorer.exe", "/select, \"" + outputFile + "\"");
                }
            }
        }

        private void Vocalizer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!picWorking.Visible)
            {
                Bass.BASS_Free();
                return;
            }
            MessageBox.Show("Please wait until the current process finishes", AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            e.Cancel = true;
        }

        private void EnableDisable(bool enable)
        {            
            picWorking.Visible = !enable;
            btnExport.Enabled = enable;
            btnImport.Enabled = enable;

            if (!enable)
            {                
                btnPause.Enabled = false;
                btnPlay.Enabled = false;
                btnStop.Enabled = false;
            }
        }

        private void effectsWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            ApplyEffects();
        }

        private void effectsWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            EnableDisable(true);
            btnApply.Enabled = true;            
            if (SaveToFile)
            {
                MessageBox.Show("Exporting completed", AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            btnSave.Enabled = true;
            btnPlay.Enabled = true;
            btnPause.Enabled = Bass.BASS_ChannelIsActive(BassMixer) == BASSActive.BASS_ACTIVE_PLAYING;
            btnStop.Enabled = btnPause.Enabled;
            if (wasPlaying)
            {
                btnPlay.PerformClick();
            }
        }

        private void switchCompression_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) { return; }

            if (switchCompression.Tag.Equals("0"))
            {
                switchCompression.Tag = "1";
            }
            else
            {
                switchCompression.Tag = "0";
            }
            EnableDisableCompression();
        }

        private void switchDistortion_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) { return; }

            if (switchDistortion.Tag.Equals("0"))
            {
                switchDistortion.Tag = "1";
            }
            else
            {
                switchDistortion.Tag = "0";
            }
            EnableDisableDistortion();
        }

        private void switchFlanger_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) { return; }

            if (switchFlanger.Tag.Equals("0"))
            {
                switchFlanger.Tag = "1";
            }
            else
            {
                switchFlanger.Tag = "0";
            }
            EnableDisableFlanger();
        }

        private void switchGargle_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) { return; }

            if (switchGargle.Tag.Equals("0"))
            {
                switchGargle.Tag = "1";
            }
            else
            {
                switchGargle.Tag = "0";
            }
            EnableDisableGargle();
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "Effects Preset (*.fx)|*.fx",
                Title = "Select the effects preset file to import",
                InitialDirectory = Tools.CurrentFolder
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Tools.CurrentFolder = Path.GetDirectoryName(ofd.FileName);
                ImportPreset(ofd.FileName);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog
            {
                Filter = "Effects Preset (*.fx)|*.fx",
                Title = "Select where to export your effects preset file",
                InitialDirectory = Tools.CurrentFolder,
                FileName = "myEffectsPreset.fx"
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Tools.CurrentFolder = Path.GetDirectoryName(sfd.FileName);
                ExportPreset(sfd.FileName);
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (effectsWorker.IsBusy) return;
            SaveToFile = false;
            EnableDisable(false);            
            effectsWorker.RunWorkerAsync();
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (BassStream == 0 || BassMixer == 0)
            {
                MessageBox.Show("Something went wrong, I can't find an audio stream to play", AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            BassMix.BASS_Mixer_ChannelSetPosition(BassStream, Bass.BASS_ChannelSeconds2Bytes(BassStream, songPosition));
            if (Bass.BASS_ChannelIsActive(BassMixer) != BASSActive.BASS_ACTIVE_PAUSED)
            {
                Bass.BASS_ChannelPlay(BassMixer, true);
            }
            else
            {
                Bass.BASS_ChannelPlay(BassMixer, false);
            }
            playbackTimer.Enabled = true;
            btnPlay.Enabled = false;
            btnPause.Enabled = true;
            btnStop.Enabled = true;
            picSpect.Visible = true;
            btnSave.Enabled = false;
            btnImport.Enabled = false;
            btnExport.Enabled = false;
            wasPlaying = true;
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            Bass.BASS_ChannelPause(BassMixer);
            btnPause.Enabled = false;
            btnPlay.Enabled = true;
            btnSave.Enabled = false;
            btnImport.Enabled = false;
            btnExport.Enabled = false;
            wasPlaying = false;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            Bass.BASS_ChannelStop(BassMixer);
            btnStop.Enabled = false;
            btnPause.Enabled = false;
            btnPlay.Enabled = true;
            picSpect.Visible = false;
            playbackTimer.Enabled = false;
            btnApply.Enabled = true;
            btnSave.Enabled = true;
            btnImport.Enabled = true;
            btnExport.Enabled = true;
            Text = AppName + " - " + inputFile;
            wasPlaying = false;
            songPosition = 0;
        }

        private readonly Visuals Spectrum = new Visuals(); // visuals class instance
        private void DrawSpectrum()
        {
            var width = picSpect.Width;
            var height = picSpect.Height;
            var spect = Spectrum.CreateSpectrum(BassMixer, width, height, Color.Green, Color.Red, BackColor, false, false, true); ;
            picSpect.Image = spect;
        }

        private void playbackTimer_Tick(object sender, EventArgs e)
        {
            if (Bass.BASS_ChannelIsActive(BassMixer) == BASSActive.BASS_ACTIVE_PLAYING)
            {
                DrawSpectrum();

                var pos = Bass.BASS_ChannelGetPosition(BassStream); // position in bytes
                songPosition = Bass.BASS_ChannelBytes2Seconds(BassStream, pos); // the elapsed time length
                var position = GetTime(songPosition);
                var length = GetTime(songLength);

                Text = AppName + " - " + inputFile + " " + position + "/" + length;
            }           
        }

        private string GetTime(double seconds)
        {
            string time;
            if (seconds >= 3600)
            {
                var hours = (int)(seconds / 3600);
                var mins = (int)(seconds - (hours * 3600));
                var secs = (int)(seconds - (mins * 60));
                time = hours + ":" + (mins < 10 ? "0" : "") + mins + ":" + (seconds < 10 ? "0" : "") + seconds;
            }
            else if (seconds >= 60)
            {
                var mins = (int)(seconds / 60);
                var secs = (int)(seconds - (mins * 60));
                time = mins + ":" + (secs < 10 ? "0" : "") + secs;
            }
            else
            {
                time = "0:" + (seconds < 10 ? "0" : "") + (int)seconds;
            }
            return time;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveToFile = true;
            EnableDisable(false);
            effectsWorker.RunWorkerAsync();
        }

        private void picVolSlider_MouseUp(object sender, MouseEventArgs e)
        {
            picVolSlider.Cursor = Cursors.Hand;
            if (picVolSlider.Top < picVolBackground.Top)
            {
                picVolSlider.Top = picVolBackground.Top;
            }
            var bottom = picVolBackground.Top + picVolBackground.Height - picVolSlider.Height;
            if (picVolSlider.Top > bottom)
            {
                picVolSlider.Top = bottom;
            }
            ApplyVolumeChange();
        }

        private void ApplyVolumeChange()
        {
            if (effectsWorker.IsBusy || (Bass.BASS_ChannelIsActive(BassMixer) != BASSActive.BASS_ACTIVE_PLAYING && Bass.BASS_ChannelIsActive(BassMixer) != BASSActive.BASS_ACTIVE_PAUSED))
            {
                return;
            }        
            
            newVolume = (float)Math.Round((double)2f + (picVolSlider.Top - (float)picVolBackground.Top) / (picVolBackground.Height - ((float)picVolSlider.Height )) * -2f, 3);
            
            SaveToFile = false;
            EnableDisable(false);
            effectsWorker.RunWorkerAsync();
        }

        private void picVolSlider_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            picVolSlider.Cursor = Cursors.NoMoveVert;

            if (picVolSlider.Top < picVolBackground.Top)
            {
                picVolSlider.Top = picVolBackground.Top;
            }
            var bottom = picVolBackground.Top + picVolBackground.Height - picVolSlider.Height;
            if (picVolSlider.Top > bottom)
            {
                picVolSlider.Top = bottom;
            }                
        }

        private void picVolSlider_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
                        
            picVolSlider.Top = PointToClient(MousePosition).Y;
            if (picVolSlider.Top < picVolBackground.Top)
            {
                picVolSlider.Top = picVolBackground.Top;
            }
            var bottom = picVolBackground.Top + picVolBackground.Height - picVolSlider.Height;
            if (picVolSlider.Top > bottom)
            {
                picVolSlider.Top = bottom;
            }            
        }

        private void picVolSlider_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) { return; }

            picVolSlider.Top = defaultVolume;
            ApplyVolumeChange();
        }

        private void MiniStudio_KeyDown(object sender, KeyEventArgs e)
        {
            if (picWorking.Visible) return;

            picVolSlider.Focus();

            switch (e.KeyCode)
            {
                case Keys.Oemplus:
                case Keys.VolumeUp:
                    if (picVolSlider.Top > picVolBackground.Top)
                    {
                        picVolSlider.Top--;
                    }
                    break;
                case Keys.OemMinus:
                case Keys.VolumeDown:
                    if (picVolSlider.Top < picVolBackground.Top + picVolBackground.Height - picVolSlider.Height)
                    {
                        picVolSlider.Top++;
                    }
                    break;
                case Keys.Space:
                    if (btnPlay.Enabled && Bass.BASS_ChannelIsActive(BassMixer) != BASSActive.BASS_ACTIVE_PLAYING)
                    {
                        btnPlay.PerformClick();
                    }
                    else if (btnPause.Enabled && Bass.BASS_ChannelIsActive(BassMixer) == BASSActive.BASS_ACTIVE_PLAYING)
                    {
                        btnPause.PerformClick();
                    }
                    break;
            }
        }

        private void MiniStudio_KeyUp(object sender, KeyEventArgs e)
        {
            ApplyVolumeChange();
        }
    } 
}

