using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Mix;
using Un4seen.Bass.AddOn.Opus;
using Un4seen.Bass.AddOn.Enc;
using Un4seen.Bass.AddOn.EncOgg;
using Un4seen.Bass.AddOn.Flac;
using System.Globalization;
using Nautilus.Properties;
using NautilusFREE;

namespace Nautilus
{
    public partial class MoggMaker : Form
    {
        private readonly nTools nautilus3;
        private readonly NemoTools Tools;
        private readonly List<string> AudioFiles;
        private bool isOpus;
        private bool isFlac;
        private int BassMixer;
        private int BassStream;
        private const int BassBuffer = 1000;
        private bool hasRealAudio;
        private string makerFolder;
        private string moggFile;
        private string oggFile;
        private int audioChannels;
        private int overrideFrequency;
        private bool BASS_INIT;
        private readonly List<int> BassStreams;
        private double LongestAudio;
        private string encodingQuality;

        public MoggMaker(Color ButtonBackColor, Color ButtonTextColor)
        {
            InitializeComponent();

            AudioFiles = new List<string>();
            Tools = new NemoTools();
            nautilus3 = new nTools();
            BassStreams = new List<int>();
            btnBegin.BackColor = ButtonBackColor;
            btnClear.BackColor = ButtonBackColor;
            btnSilent.BackColor = ButtonBackColor;
            btnBegin.ForeColor = ButtonTextColor;
            btnClear.ForeColor = ButtonTextColor;
            btnSilent.ForeColor = ButtonTextColor;

            InitializeBASS();

            makerFolder = Application.StartupPath + "\\moggmaker\\";
            if (!Directory.Exists(makerFolder))
            {
                Directory.CreateDirectory(makerFolder);
            }
            moggFile = makerFolder + "output.mogg";
            oggFile = makerFolder + "temp.ogg";            
        }

        private void InitializeBASS()
        {
            if (!BASS_INIT)
            {
                //initialize BASS.NET
                if (!Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, Handle))
                {
                    MessageBox.Show("Error initializing BASS.NET\n" + Bass.BASS_ErrorGetCode());
                    return;
                }
                Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_BUFFER, BassBuffer);
                Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD, 50);
                BASS_INIT = true;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lstAudio.Items.Clear();
            AudioFiles.Clear();
            hasRealAudio = false;
            audioChannels = 0;
            lstLog.Items.Clear();
            Log("Welcome to " + Text);
            LongestAudio = 0.0;
            UpdateInfoLabel();
        }

        private void exportLogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.ExportLog(Text, lstLog.Items);
        }

        private void lstAudio_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
        }

        private void lstAudio_DragDrop(object sender, DragEventArgs e)
        {
            if (picWorking.Visible) return;
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (isValidFile(files[0]))
            {
                var counter = lstAudio.Items.Count + 1;
                lstAudio.Items.Add(counter + ". " + Path.GetFileName(files[0]) + " (" + GetAudioDetails(files[0]) + ")");
                Log("Audio file '" + files[0] + "' received");
                AudioFiles.Add(files[0]);
                hasRealAudio = true;
                UpdateInfoLabel();
            }
            else
            {
                MessageBox.Show("Invalid file", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
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

        public string GetAudioDetails(string file)
        {
            CheckFileType(file);

            try
            {
                if (isOpus)
                {
                    BassStream = BassOpus.BASS_OPUS_StreamCreateFile(file, 0L, File.ReadAllBytes(file).Length, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);
                }
                else if (isFlac)
                {
                    BassStream = BassFlac.BASS_FLAC_StreamCreateFile(file, 0L, File.ReadAllBytes(file).Length, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);
                }
                else
                {
                    BassStream = Bass.BASS_StreamCreateFile(file, 0L, File.ReadAllBytes(file).Length, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);
                }
            }
            catch
            {
                return "ERROR";
            }

            if (BassStream == 0)
            {
                MessageBox.Show("Error opening that file:\n" + Bass.BASS_ErrorGetCode().ToString(), Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return "ERROR";
            }
                        
            var channel_info = Bass.BASS_ChannelGetInfo(BassStream);
            var channels = channel_info.chans;
            var freq = channel_info.freq;
            audioChannels += channels;

            long length = Bass.BASS_ChannelGetLength(BassStream);
            var duration = Math.Round(Bass.BASS_ChannelBytes2Seconds(BassStream, length), 2);

            if (duration > LongestAudio)
            {
                LongestAudio = duration;
                UpdateInfoLabel();
            }
            return channels + (channels == 1 ? " channel" : " channels") + " - " + freq + "Hz - " + CalculateLength(duration);
        }

        private void UpdateInfoLabel()
        {
            lblInfo.Text = "";
            if (audioChannels > 0) 
            {
                lblInfo.Text = "( " + audioChannels + " audio " + (audioChannels == 1 ? "channel" : "channels");
                if (LongestAudio > 0)
                {
                    lblInfo.Text += " - " + CalculateLength(LongestAudio) + " )";
                }
                else
                {
                    lblInfo.Text += " )";
                }
            }
        }

        private string CalculateLength(double time)
        {            
            var Parser = new DTAParser();
            var minutes = Parser.GetSongDuration((time * 1000).ToString(CultureInfo.InvariantCulture));
            return minutes;
        }

        private bool isValidFile(string file)
        {
            switch (Path.GetExtension(file).ToLowerInvariant())
            {
                case ".aiff":
                case ".mp3":
                case ".ogg":
                case ".wav":
                case ".m4a":
                case ".opus":
                case ".flac":
                case ".aac":
                    return true;
                default:
                    return false;
            }
        }

        private void btnBegin_Click(object sender, EventArgs e)
        {
            if (lstAudio.Items.Count == 0 || AudioFiles.Count == 0 || !hasRealAudio)
            {
                MessageBox.Show("No audio files added, nothing to do", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (audioChannels == 6)
            {
                var result = MessageBox.Show("This will result in a 6 channel Mogg file, which will cause the infamous '5.1 bug' due to how Ogg Vorbis encoding works.\n\nI suggest you add another track, even if it's a Silent Track to pad the channels.\n\nAre you sure you want to continue?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (result != DialogResult.Yes)
                {
                    return;
                }
            }

            EnableDisable(false);
            Log("Creating multi-channel ogg file from input files");
            InitializeBASS();
            backgroundWorker1.RunWorkerAsync();
        }

        private void MoggMaker_Shown(object sender, EventArgs e)
        {
            cboQuality.SelectedIndex = 4;

            Log("Welcome to " + Text);
        }

        private void btnSilent_Click(object sender, EventArgs e)
        {
            var counter = lstAudio.Items.Count + 1;
            lstAudio.Items.Add(counter + ". Silent Track (2 channels)");
            AudioFiles.Add("Silent Track");
            audioChannels += 2;
            UpdateInfoLabel();
        }

        private void CheckFileType(string file)
        {
            isOpus = Path.GetExtension(file).ToLowerInvariant() == ".opus";
            isFlac = Path.GetExtension(file).ToLowerInvariant() == ".flac";
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {            
            Tools.DeleteFile(moggFile);
            Tools.DeleteFile(oggFile);                      

            var audioFile = "";
            foreach (var file in AudioFiles)
            {
                if (file != "Silent Track")
                {
                    audioFile = file;
                    CheckFileType(file);
                    break;
                }
            }

            if (isOpus)
            {
                BassStream = BassOpus.BASS_OPUS_StreamCreateFile(audioFile, 0L, File.ReadAllBytes(audioFile).Length, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);
            }
            else if (isFlac)
            {
                BassStream = BassFlac.BASS_FLAC_StreamCreateFile(audioFile, 0L, File.ReadAllBytes(audioFile).Length, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);
            }
            else
            {
                BassStream = Bass.BASS_StreamCreateFile(audioFile, 0L, File.ReadAllBytes(audioFile).Length, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);
            }
            var stream_info = Bass.BASS_ChannelGetInfo(BassStream);
            BassMixer = BassMix.BASS_Mixer_StreamCreate(chkOverride.Checked ? overrideFrequency : stream_info.freq, audioChannels, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_MIXER_END);

            var currentChannel = 0;
            foreach (var file in AudioFiles)
            {
                var isSilent = file == "Silent Track";
                CheckFileType(isSilent ? audioFile : file);

                if (isOpus)
                {
                    BassStream = BassOpus.BASS_OPUS_StreamCreateFile(isSilent? audioFile : file, 0L, File.ReadAllBytes(isSilent ? audioFile : file).Length, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);
                }
                else if (isFlac)
                {
                    BassStream = BassFlac.BASS_FLAC_StreamCreateFile(isSilent ? audioFile : file, 0L, File.ReadAllBytes(isSilent ? audioFile : file).Length, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);
                }
                else
                {
                    BassStream = Bass.BASS_StreamCreateFile(isSilent ? audioFile : file, 0L, File.ReadAllBytes(isSilent ? audioFile : file).Length, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);
                }
                stream_info = Bass.BASS_ChannelGetInfo(BassStream);
                if (stream_info.chans == 0) continue;
                BassMix.BASS_Mixer_StreamAddChannel(BassMixer, BassStream, BASSFlag.BASS_MIXER_MATRIX);
                //var matrix = GetMatrix(currentChannel, stream_info.chans, isSilent);
                var matrix = GetMatrix(currentChannel, stream_info.chans, audioChannels, isSilent);
                BassMix.BASS_Mixer_ChannelSetMatrix(BassStream, matrix);
                currentChannel += isSilent? 2 : stream_info.chans;
                BassStreams.Add(BassStream);
            }
            
            BassEnc_Ogg.BASS_Encode_OGG_StartFile(BassMixer, "-q " + encodingQuality, BASSEncode.BASS_ENCODE_AUTOFREE, oggFile);
            while (true)
            {
                var buffer = new byte[20000];
                var c = Bass.BASS_ChannelGetData(BassMixer, buffer, buffer.Length);
                if (c < 0) break;
            }

            ClearBASS();

            if (File.Exists(oggFile))
            {
                Log("Success");

                Log("Adding Mogg header");
                if (Tools.MakeMogg(oggFile, moggFile))
                {
                    Log("Success");
                }
                else
                {
                    Log("Failed");
                }
            }
        }       

        private void ClearBASS()
        {
            Bass.BASS_ChannelStop(BassMixer);
            Bass.BASS_StreamFree(BassMixer);
            foreach (var stream in BassStreams)
            {
                Bass.BASS_StreamFree(stream);
            }
            Bass.BASS_Free();
            BASS_INIT = false;
        }

        /*private float[,] GetMatrix(int currentChannel, int channelCount, bool isSilent)
        {
            var matrix = new float[audioChannels, channelCount];
            var vol = isSilent ? (float)0.0 : (float)1.0;
            if (channelCount == 1)
            {
                matrix[currentChannel, 0] = vol;
            }
            else
            {
                matrix[currentChannel, 0] = vol;
                matrix[currentChannel + 1, 1] = vol;
            }
            return matrix;
        }*/

        private float[,] GetMatrix(int logicalStartChannel, int sourceChannelCount, int totalOutputChannels, bool isSilent)
        {
            var matrix = new float[totalOutputChannels, sourceChannelCount];
            var vol = isSilent ? 0.0f : 1.0f;

            var splitter = new MoggSplitter();
            var map = splitter.ArrangeStreamChannels(totalOutputChannels, true);

            for (int src = 0; src < sourceChannelCount; src++)
            {
                int logicalDest = logicalStartChannel + src;
                if (logicalDest >= totalOutputChannels)
                    break;

                int physicalDest = map[logicalDest];
                matrix[physicalDest, src] = vol;
            }

            return matrix;
        }

        private void EnableDisable(bool enabled)
        {
            btnSilent.Enabled = enabled;
            btnBegin.Enabled = enabled;
            btnClear.Enabled = enabled;
            picWorking.Visible = !enabled;
            chkEncrypt.Enabled = enabled;
            chkAnalyzer.Enabled = enabled;
            chkOverride.Enabled = enabled;
            lstLog.Enabled = enabled;
            lstAudio.Enabled = enabled;
            lstLog.Cursor = enabled ? Cursors.Default : Cursors.WaitCursor;
            lstAudio.Cursor = lstLog.Cursor;
            Cursor = lstLog.Cursor;
            cboQuality.Enabled = enabled;
        }

        private void MoggMaker_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!picWorking.Visible)
            {
                ClearBASS();
                Tools.DeleteFolder(makerFolder, true);
                return;
            }
            MessageBox.Show("Please wait until the current process finishes", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            e.Cancel = true;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            EnableDisable(true);
            Tools.DeleteFile(oggFile);

            if (!File.Exists(moggFile))
            {
                Log("Process failed, see the log file");
                return;
            }

            if (chkEncrypt.Checked)
            {
                Log("Encrypting Mogg file");
                if (nautilus3.EncM(File.ReadAllBytes(moggFile),moggFile))
                {
                    Log("Success");
                }
                else
                {
                    Log("Failed");
                }
            }
            var sfd = new SaveFileDialog
            {
                Filter = "Mogg Files|*.mogg",
                Title = "Where should I save the Mogg file to?",
                FileName = "output",
                InitialDirectory = Environment.CurrentDirectory,
            };

            if (sfd.ShowDialog() != DialogResult.OK)
            {
                Log("User cancelled process");
                Tools.DeleteFile(moggFile);
                return;
            }

            Tools.MoveFile(moggFile, sfd.FileName);
            if (!File.Exists(sfd.FileName))
            {
                Log("Error");
                return;
            }
                        
            if (chkAnalyzer.Checked)
            {
                Log("Sending Mogg file to Audio Analyzer");
                var analyzer = new AudioAnalyzer(sfd.FileName);
                analyzer.Show();
            }
            Log("Finished");
        }

        private void chkOverride_CheckedChanged(object sender, EventArgs e)
        {
            cboFreq.Enabled = chkOverride.Checked;
            if (cboFreq.Enabled && cboFreq.SelectedIndex < 0)
            {
                cboFreq.SelectedIndex = 3;
            }
        }

        private void cboFreq_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboFreq.SelectedIndex >= 0)
            {
                overrideFrequency = Convert.ToInt32(cboFreq.Items[cboFreq.SelectedIndex]);
            }
        }

        private void cboQuality_SelectedIndexChanged(object sender, EventArgs e)
        {
            encodingQuality = cboQuality.Items[cboQuality.SelectedIndex].ToString();
        }

        private void lstAudio_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    MoveAudio(MovementType.Up);
                    break;
                case Keys.Down:
                    MoveAudio(MovementType.Down);
                    break;
                case Keys.Delete:
                    DeleteAudio();
                    break;
                default:
                    return;
            }            
        }

        private void DeleteAudio()
        {
            if (lstAudio.SelectedIndex == -1) return;
            var audio = lstAudio.Items[lstAudio.SelectedIndex].ToString();
            var channels = audio.Contains("1 channel") ? 1 : 2;
            AudioFiles.RemoveAt(lstAudio.SelectedIndex);        
            lstAudio.Items.RemoveAt(lstAudio.SelectedIndex);
            audioChannels -= channels;
            UpdateInfoLabel();
        }

        private enum MovementType
        {
            Up, Down
        }

        private void MoveAudio(MovementType movement)
        {
            if (lstAudio.SelectedIndex == -1) return;
            if (movement == MovementType.Up && lstAudio.SelectedIndex == 0) return;
            if (movement == MovementType.Down && lstAudio.SelectedIndex == lstAudio.Items.Count - 1) return;

            var oldIndex = lstAudio.SelectedIndex;
            var newIndex = movement == MovementType.Up ? lstAudio.SelectedIndex - 1 : lstAudio.SelectedIndex + 1;

            var oldFilePath = AudioFiles[oldIndex];
            var oldFileName = lstAudio.Items[oldIndex].ToString();
            var newFilePath = AudioFiles[newIndex];
            var newFileName = lstAudio.Items[newIndex].ToString();

            AudioFiles[oldIndex] = newFilePath;
            lstAudio.Items[oldIndex] = newFileName;
            AudioFiles[newIndex] = oldFilePath;
            lstAudio.Items[newIndex] = oldFileName;            
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
    }
}