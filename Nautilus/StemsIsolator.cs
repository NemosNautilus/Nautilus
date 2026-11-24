using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Nautilus.Properties;
using Nautilus.x360;
using Un4seen.Bass.AddOn.Mix;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Enc;
using Un4seen.Bass.AddOn.EncFlac;
using Un4seen.Bass.AddOn.EncMp3;
using Un4seen.Bass.AddOn.EncOgg;
using Un4seen.Bass.AddOn.EncOpus;
using Un4seen.Bass.AddOn.Fx;

namespace Nautilus
{
    public partial class StemsIsolator : Form
    {
        private int ChannelsTotal;
        private int ChannelsDrums;
        private int ChannelsBass;
        private int ChannelsGuitar;
        private int ChannelsVocals;
        private int ChannelsKeys;
        private int ChannelsBacking;
        private int ChannelsCrowd;
        private readonly NemoTools Tools;
        private readonly DTAParser Parser;
        private string StemsToIsolate;
        private string AudioType = "WAV";
        private bool BASS_INIT = false;
        private MoggSplitter.MoggSplitFormat AudioFormat = MoggSplitter.MoggSplitFormat.WAV;
        private int BassStream;
        private int BassMixer;
        private List<int> Splits;

        public StemsIsolator(Color ButtonBackColor, Color ButtonTextColor)
        {
            InitializeComponent();
            
            Tools = new NemoTools();
            Parser = new DTAParser();
            grpStems.AllowDrop = true;
            Splits = new List<int>();

            toolTip1.SetToolTip(btnBegin, "Click to begin process");
            toolTip1.SetToolTip(btnFile, "Click to select the source file");
            toolTip1.SetToolTip(txtFile, "This is the source file");
            toolTip1.SetToolTip(lstLog, "This is the application log. Right click to export");

            var formButtons = new List<Button> { btnFile,btnReset,btnBegin };
            foreach (var button in formButtons)
            {
                button.BackColor = ButtonBackColor;
                button.ForeColor = ButtonTextColor;
                button.FlatAppearance.MouseOverBackColor = button.BackColor == Color.Transparent ? Color.FromArgb(127, Color.AliceBlue.R, Color.AliceBlue.G, Color.AliceBlue.B) : Tools.LightenColor(button.BackColor);
            }
            txtFile.Focus();
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

        private void EnableDisable(bool enabled)
        {
            picWorking.Visible = !enabled;
            btnBegin.Visible = enabled;
            btnFile.Enabled = enabled;            
            btnReset.Visible = enabled;
            grpStems.Enabled = enabled;// && !radioDownmix.Checked;
            EnableDisableStems();
            chkIgnore.Enabled = radioSplit.Checked;
            txtFile.Enabled = enabled;
            menuStrip1.Enabled = enabled;
            grpMode.Enabled = enabled;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StemsToIsolate = GetStemsToIsolate();
            if (!radioDownmix.Checked && string.IsNullOrWhiteSpace(StemsToIsolate))
            {
                MessageBox.Show("Select at least one stem to isolate", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            try
            {
                EnableDisable(false);
                //start animation and send to background worker
                picWorking.Visible = true;
                backgroundWorker1.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                Log("There was an error: " + ex.Message);
                EnableDisable(true);
            }
        }

        private void HandleDragDrop(object sender, DragEventArgs e)
        {
            if (picWorking.Visible) return;

            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            try
            {
                if (files[0].EndsWith(".m4a"))
                {
                    txtFile.Text = files[0];
                }
                else if (VariousFunctions.ReadFileType(files[0]) != XboxFileType.STFS)
                {
                    MessageBox.Show("That's not a valid file to drop here", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                txtFile.Text = files[0];
                Tools.CurrentFolder = txtFile.Text;
            }
            catch (Exception ex)
            {
                Log("There was a problem accessing that file");
                Log("The error says: " + ex.Message);
            }
        }

        private void HandleDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
        }

        private string GetStemsToIsolate()
        {
            var stems = "";
            if (chkDrums.Enabled && chkDrums.Checked)
            {
                stems += "drums|";
            }
            if (chkBass.Enabled && chkBass.Checked)
            {
                stems += "bass|";
            }
            if (chkGuitar.Enabled && chkGuitar.Checked)
            {
                stems += "guitar|";
            }
            if (chkKeys.Enabled && chkKeys.Checked)
            {
                stems += "keys|";
            }
            if (chkVocals.Enabled && chkVocals.Checked)
            {
                stems += "vocals|";
            }
            if (chkBacking.Enabled && chkBacking.Checked)
            {
                stems += "backing|";
            }
            if (chkCrowd.Enabled && chkCrowd.Checked)
            {
                stems += "crowd";
            }
            return stems;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (txtFile.Text.EndsWith(".m4a"))
            {
                if (radioSplit.Checked)
                {
                    Log("Trying to separate .m4a file into its component stems");
                }
                else
                {
                    Log("Trying to downmix .m4a file to stereo");
                }
                ProcessFnFFile(txtFile.Text);
                return;
            }
            var Splitter = new MoggSplitter();
            var folder = txtFile.Text + " Stems\\";
            if (radioSplit.Checked)
            {
                Log("Trying to separate mogg file into its component stems");
                var split = Splitter.SplitMogg(txtFile.Text, folder, StemsToIsolate, AudioFormat, chkIgnore.Checked);
                foreach (var error in Splitter.ErrorLog)
                {
                    Log(error);
                }
                Log(split ? "Process completed successfully!" : "Something went wrong along the way, sorry!");
                return;
            }
            if (radioDownmix.Checked)
            {
                Log("Trying to downmix mogg file to a stereo file");
                var downmixed = Splitter.DownmixMogg(txtFile.Text, "", AudioFormat, chkCrowd.Checked ? "allstems" : "allstems|NOcrowd");
                foreach (var error in Splitter.ErrorLog)
                {
                    Log(error);
                }
                Log(downmixed ? "Process completed successfully!" : "Something went wrong along the way, sorry!");
                return;
            }            
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            picWorking.Visible = false;
            btnReset.Visible = true;    
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var message = Tools.ReadHelpFile("si");
            var help = new HelpForm(Text + " - Help", message);
            help.ShowDialog();
        }

        private void reset(bool finished)
        {
            if (finished)
            {
                Log("Resetting");
            }
            EnableDisable(true);
            ChannelsTotal = 0;
            ChannelsDrums = 0;
            ChannelsBass = 0;
            ChannelsGuitar = 0;
            ChannelsVocals = 0;
            ChannelsKeys = 0;
            ChannelsBacking = 0;
            ChannelsCrowd = 0;
            btnBegin.Visible = false;
            chkBacking.Enabled = false;
            chkBass.Enabled = false;
            chkGuitar.Enabled = false;
            chkDrums.Enabled = false;
            chkKeys.Enabled = false;
            chkVocals.Enabled = false;
            chkCrowd.Enabled = false;
            if (!finished) return;
            var file = txtFile.Text;
            txtFile.Text = "Select source file (or drag/drop it here)";
            Log("Ready");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            reset(true);
        }
                
        private void ProcessFnFFile(string file)
        {
            if (!InitBass()) return;

            var fnfParser = new NemoFnFParser();
            BassStream = fnfParser.m4aToBassStream(file, 10);

            if (BassStream == 0)
            {
                Log("Error processing that M4A file, is this a Fortnite Festival file?");
                return;
            }          

            var stream_info = Bass.BASS_ChannelGetInfo(BassStream);
            int inputChannels = stream_info.chans;
            int outputChannels = 2;
            BassMixer = BassMix.BASS_Mixer_StreamCreate(stream_info.freq, outputChannels, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_MIXER_END);
            BassMix.BASS_Mixer_StreamAddChannel(BassMixer, BassStream, BASSFlag.BASS_MIXER_MATRIX);

            var matrix = new float[outputChannels, inputChannels];
            var vol = (1.0f / inputChannels) * 3f;  //arbitrary math so the mix doesn't clip but doesn't sound too flat
            for (var i = 0; i < inputChannels - 1; i++)
            {
                matrix[0, i] = vol;
                matrix[1, i + 1] = vol;
            }
            BassMix.BASS_Mixer_ChannelSetMatrix(BassStream, matrix);

            var outputFile = Path.GetDirectoryName(file) + "\\" + Path.GetFileNameWithoutExtension(file) + "." + AudioType.ToLowerInvariant();
            var arg = "";
            if (radioSplit.Checked)
            {
                var folder = Path.GetDirectoryName(file) + "\\stems\\";
                if (!string.IsNullOrEmpty(folder) && !Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                var drums = folder + "\\drums." + AudioType.ToLowerInvariant();
                var bass = folder + "\\bass." + AudioType.ToLowerInvariant();
                var guitar = folder + "\\guitar." + AudioType.ToLowerInvariant();
                var vocals = folder + "\\vocals." + AudioType.ToLowerInvariant();
                var backing = folder + "\\backing." + AudioType.ToLowerInvariant();

                var tracks = new List<string> { drums, bass, guitar, vocals, backing };
                foreach (var track in tracks)
                {
                    File.Delete(track);
                }
                var ArrangedChannels = ArrangeStreamChannels(10);
                PrepareChannelsToSplit(0, ArrangedChannels, 2, (float)1.0, drums, false);

                PrepareChannelsToSplit(2, ArrangedChannels, 2, (float)1.0, bass, true);

                PrepareChannelsToSplit(4, ArrangedChannels, 2, (float)1.0, guitar, true);

                PrepareChannelsToSplit(6, ArrangedChannels, 2, (float)1.0, vocals, true);

                PrepareChannelsToSplit(8, ArrangedChannels, 2, (float)1.0, backing, true);

                while (true)
                {
                    var buffer = new byte[20000];
                    var c = Bass.BASS_ChannelGetData(Splits[0], buffer, buffer.Length);
                    if (c <= 0) break;
                    for (var i = 1; i < Splits.Count; i++)
                    {
                        while (Bass.BASS_ChannelGetData(Splits[i], buffer, buffer.Length) > 0) { }
                    }
                }
                foreach (var split in Splits)
                {
                    Bass.BASS_StreamFree(split);
                }
                Splits.Clear();

                ReleaseBass();

                if (File.Exists(drums))
                {
                    Log("Success");
                }
                else
                {
                    Log("Failed");
                }
                EnableDisable(true);
            }
            else
            {
                switch (AudioFormat)
                {
                    case MoggSplitter.MoggSplitFormat.FLAC:
                        arg = "--compression-level-5 --fast -T \"COMMENT=Made by Nemo\"";
                        BassEnc_Flac.BASS_Encode_FLAC_StartFile(BassMixer, arg, BASSEncode.BASS_ENCODE_AUTOFREE, outputFile);
                        break;
                    case MoggSplitter.MoggSplitFormat.OPUS:
                        arg = "--vbr --music --comment COMMENT=\"Made by Nemo\"";
                        BassEnc_Opus.BASS_Encode_OPUS_StartFile(BassMixer, arg, BASSEncode.BASS_ENCODE_DEFAULT | BASSEncode.BASS_ENCODE_AUTOFREE, outputFile);
                        break;
                    case MoggSplitter.MoggSplitFormat.OGG:
                        arg = "-q 5 -c \"COMMENT=Made by Nemo\"";
                        BassEnc_Ogg.BASS_Encode_OGG_StartFile(BassMixer, arg, BASSEncode.BASS_ENCODE_AUTOFREE, outputFile);
                        break;
                    case MoggSplitter.MoggSplitFormat.MP3:
                        arg = "-b 320 --add-id3v2 --ignore-tag-errors --tc \"Made by Nemo\"";
                        BassEnc_Mp3.BASS_Encode_MP3_StartFile(BassMixer, arg, BASSEncode.BASS_UNICODE | BASSEncode.BASS_ENCODE_AUTOFREE, outputFile);
                        break;
                    default:
                    case MoggSplitter.MoggSplitFormat.WAV:
                        BassEnc.BASS_Encode_Start(BassMixer, outputFile, BASSEncode.BASS_ENCODE_PCM | BASSEncode.BASS_ENCODE_AUTOFREE, null, IntPtr.Zero);
                        break;
                }

                Log("Beginning encoding process to stereo file");
                while (true)
                {
                    var buffer = new byte[20000];
                    var c = Bass.BASS_ChannelGetData(BassMixer, buffer, buffer.Length);
                    if (c <= 0) break;
                }

                ReleaseBass();
                if (File.Exists(outputFile))
                {
                    Log("Success");
                }
                else
                {
                    Log("Failed");
                }
                EnableDisable(true);
            }
        }

        private void PrepareChannelsToSplit(int index, IList<int> ArrangedChannels, int channels, float vol, string file, bool slave = true)
        {
            var channel_map = new int[channels == 2 ? 3 : 2];
            channel_map[0] = ArrangedChannels[index];
            channel_map[1] = channels == 2 ? ArrangedChannels[index + 1] : -1;
            if (channels == 2)
            {
                channel_map[2] = -1;
            }
            var flags = slave ? BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SPLIT_SLAVE : BASSFlag.BASS_STREAM_DECODE;
            var BassMixer = BassMix.BASS_Split_StreamCreate(BassStream, flags, channel_map);
            var volumeFX = Bass.BASS_ChannelSetFX(BassMixer, BASSFXType.BASS_FX_BFX_VOLUME, 0);
            var volume = new BASS_BFX_VOLUME { lChannel = 0, fVolume = vol };
            Bass.BASS_FXSetParameters(volumeFX, volume);
            Splits.Add(BassMixer);
            var arg = "";
            switch (AudioFormat)
            {
                case MoggSplitter.MoggSplitFormat.FLAC:
                    arg = "--compression-level-5 --fast -T \"COMMENT=Made by Nemo\"";
                    BassEnc_Flac.BASS_Encode_FLAC_StartFile(BassMixer, arg, BASSEncode.BASS_ENCODE_AUTOFREE, file);
                    break;
                case MoggSplitter.MoggSplitFormat.OPUS:
                    arg = "--vbr --music --comment COMMENT=\"Made by Nemo\"";
                    BassEnc_Opus.BASS_Encode_OPUS_StartFile(BassMixer, arg, BASSEncode.BASS_ENCODE_DEFAULT | BASSEncode.BASS_ENCODE_AUTOFREE, file);
                    break;
                case MoggSplitter.MoggSplitFormat.OGG:
                    arg = "-q 5 -c \"COMMENT=Made by Nemo\"";
                    BassEnc_Ogg.BASS_Encode_OGG_StartFile(BassMixer, arg, BASSEncode.BASS_ENCODE_AUTOFREE, file);
                    break;
                case MoggSplitter.MoggSplitFormat.MP3:
                    arg = "-b 320 --add-id3v2 --ignore-tag-errors --tc \"Made by Nemo\"";
                    BassEnc_Mp3.BASS_Encode_MP3_StartFile(BassMixer, arg, BASSEncode.BASS_UNICODE | BASSEncode.BASS_ENCODE_AUTOFREE, file);
                    break;
                default:
                case MoggSplitter.MoggSplitFormat.WAV:
                    BassEnc.BASS_Encode_Start(BassMixer, file, BASSEncode.BASS_ENCODE_PCM | BASSEncode.BASS_ENCODE_AUTOFREE, null, IntPtr.Zero);
                    break;
            }
        }

        public int[] ArrangeStreamChannels(int totalChannels)
        {
            var channels = new int[totalChannels];
            for (var i = 0; i < totalChannels; i++)
            {
                channels[i] = i;
            }
            return channels;
        }

        private bool InitBass()
        {
            if (BASS_INIT) return true;
            //initialize BASS.NET
            if (!Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
            {
                if (Bass.BASS_ErrorGetCode() == BASSError.BASS_ERROR_ALREADY)
                {
                    BASS_INIT = true;
                    return true;
                }
                MessageBox.Show("Error initializing BASS.NET\n" + Bass.BASS_ErrorGetCode());
                return false;
            }
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_BUFFER, 20000);
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD, 50);
            BASS_INIT = true;
            return true;
        }

        private void ReleaseBass()
        {
            Bass.BASS_ChannelStop(BassMixer);
            Bass.BASS_StreamFree(BassMixer);
            Bass.BASS_ChannelStop(BassStream);
            Bass.BASS_StreamFree(BassStream);
            Bass.BASS_Free();
            BASS_INIT = false;
        }

        private void btnFile_Click(object sender, EventArgs e)
        {
            //if user selects new folder, assign that value
            //if user cancels or selects same folder, this forces the text_changed event to run again
            var tFile = txtFile.Text;
            var ofd = new OpenFileDialog
                {
                    InitialDirectory = Tools.CurrentFolder,
                    Title = "Select the source CON/LIVE/M4A file"
                };
            txtFile.Text = "";
            ofd.ShowDialog();
            if (!string.IsNullOrWhiteSpace(ofd.FileName) && File.Exists(ofd.FileName))
            {
                try
                {
                    if (Path.GetExtension(ofd.FileName)==".m4a")
                    {                        
                        txtFile.Text = ofd.FileName;
                    }
                    else if (VariousFunctions.ReadFileType(ofd.FileName) == XboxFileType.STFS)
                    {
                        Log("File is a valid CON file");
                        txtFile.Text = ofd.FileName;
                        Tools.CurrentFolder = Path.GetDirectoryName(txtFile.Text);
                    }
                    else
                    {
                        MessageBox.Show("That is not a valid file!\nTry again", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        txtFile.Text = tFile;
                    }
                }
                catch (Exception ex)
                {
                    Log("There was a problem accessing that file");
                    Log("The error says: " + ex.Message);
                }
            }
            else
            {
                txtFile.Text = tFile;
            }
        }
    
        private bool readDTA(byte[] xDTA)
        {
            try
            {
                if (!Parser.ReadDTA(xDTA))
                {
                    return false;
                }
                if (Parser.Songs.Count > 1)
                {
                    Log("Can't work with packs for now ... please try a single song");
                    return false;
                }
                if (Parser.Songs[0].Source.Equals("beatles"))
                {
                    Parser.Songs[0].Artist = "The Beatles";
                }                
                ChannelsTotal = Parser.Songs[0].ChannelsTotal;                
                ChannelsBass = Parser.Songs[0].ChannelsBass;
                chkBass.Enabled = ChannelsBass > 0;
                ChannelsDrums = Parser.Songs[0].ChannelsDrums;
                chkDrums.Enabled = ChannelsDrums > 0;
                ChannelsGuitar = Parser.Songs[0].ChannelsGuitar;
                chkGuitar.Enabled = ChannelsGuitar > 0;
                ChannelsKeys = Parser.Songs[0].ChannelsKeys;
                chkKeys.Enabled = ChannelsKeys > 0; 
                ChannelsVocals = Parser.Songs[0].ChannelsVocals;
                chkVocals.Enabled = ChannelsVocals > 0;
                ChannelsCrowd = Parser.Songs[0].ChannelsCrowd;
                chkCrowd.Enabled = ChannelsCrowd > 0;

                if (Parser.Songs[0].Source.Equals("beatles"))
                {
                    ChannelsTotal = ChannelsBass + ChannelsDrums + ChannelsGuitar + ChannelsKeys + ChannelsVocals + ChannelsCrowd + 2;
                }
                if (ChannelsTotal > (ChannelsBass + ChannelsDrums + ChannelsGuitar + ChannelsKeys + ChannelsVocals + ChannelsCrowd))
                {
                    ChannelsBacking = ChannelsTotal - ChannelsBass - ChannelsDrums - ChannelsGuitar - ChannelsKeys - ChannelsVocals - ChannelsCrowd;
                    chkBacking.Enabled = true;
                }
                //for debugging
                if (chkDrums.Enabled) Log("Drums have " + ChannelsDrums + (ChannelsDrums == 1 ? " channel" : " channels"));
                if (chkBass.Enabled) Log("Bass has " + ChannelsBass + (ChannelsBass == 1 ? " channel" : " channels"));
                if (chkGuitar.Enabled) Log("Guitar has " + ChannelsGuitar + (ChannelsGuitar == 1 ? " channel" : " channels"));
                if (chkVocals.Enabled) Log("Vocals have " + ChannelsVocals + (ChannelsVocals == 1 ? " channel" : " channels"));
                if (chkKeys.Enabled) Log("Keys have " + ChannelsKeys + (ChannelsKeys == 1 ? " channel" : " channels"));
                if (chkCrowd.Enabled) Log("Crowd has " + ChannelsCrowd + (ChannelsCrowd == 1 ? " channel" : " channels"));
                if (chkBacking.Enabled) Log("Backing has " + ChannelsBacking + (ChannelsBacking == 1 ? " channel" : " channels"));
                Log("Total channels: " + ChannelsTotal);

                if (!chkDrums.Enabled) chkDrums.Checked = false;
                if (!chkBass.Enabled) chkBass.Checked = false;
                if (!chkGuitar.Enabled) chkGuitar.Checked = false;
                if (!chkVocals.Enabled) chkVocals.Checked = false;
                if (!chkKeys.Enabled) chkKeys.Checked = false;
                if (!chkCrowd.Enabled) chkCrowd.Checked = false;
                if (!chkBacking.Enabled) chkBacking.Checked = false;

                return true;
            }
            catch (Exception ex)
            {
                Log("There was an error processing the songs.dta file!");
                Log("The error says " + ex.Message);
                return false;
            }        
        }

        private void txtFile_TextChanged(object sender, EventArgs e)
        {
            if (picWorking.Visible) return;
            if (!File.Exists(txtFile.Text))
            {
                txtFile.Text = "Select source file (or drag/drop it here)";
                return;
            }
            if (txtFile.Text == "Select source file (or drag/drop it here)")
            {
                txtFile.ForeColor = Color.Gray;
                return;
            }
            txtFile.ForeColor = Color.Black;
            if (txtFile.Text != "")
            {                
                reset(false);
                if (txtFile.Text.EndsWith(".m4a"))
                {
                    Log("Analyzing file ... hang on");
                    Log("File is a Fortnite Festival audio file");
                    btnBegin.Visible = true;
                    btnReset.Visible = true;
                    radioDownmix.Checked = true;
                    EnableDisableStems();
                    return;
                }
                Log("Analyzing file ... hang on");
                var xPackage = new STFSPackage(txtFile.Text);
                if (!xPackage.ParseSuccess)
                {
                    Log("Failed to open '" + Path.GetFileName(txtFile.Text) + "'");
                    return;
                }                
                xPackage.CloseIO();
                if (Parser.ExtractDTA(txtFile.Text))
                {
                    Log("Extracted songs.dta file successfully ... processing");
                }
                if (!readDTA(Parser.DTA))
                {
                    Log("Processing songs.dta failed");
                    return;
                }
                Log("DTA file processed successfully");
                btnBegin.Visible = true;
                btnReset.Visible = true;
            }
            else
            {
                btnBegin.Visible = false;
                btnReset.Visible = false;
            }
        }
        
        private void exportLogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.ExportLog(Text, lstLog.Items);
        }              

        private void stemsISO_Shown(object sender, EventArgs e)
        {
            Log("Welcome to " + Text);
            Log("Drag and drop the CON / LIVE / M4A file here");
            Log("Or click on the button to select the file");
            Log("Ready to begin");
        }

        private void StemsIsolator_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!picWorking.Visible) return;
            MessageBox.Show("Please wait until the current process finishes", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            e.Cancel = true;
        }

        private void radioSplit_CheckedChanged(object sender, EventArgs e)
        {
            chkIgnore.Enabled = radioSplit.Checked;
            EnableDisableStems();
        }

        private void EnableDisableStems()
        {
            if (txtFile.Text.Length <= 0) return;
            var isM4A = txtFile.Text.EndsWith(".m4a");
            chkDrums.Enabled = !radioDownmix.Checked && (ChannelsDrums > 0 || isM4A);
            chkBass.Enabled = !radioDownmix.Checked && (ChannelsBass > 0 || isM4A);
            chkVocals.Enabled = !radioDownmix.Checked && (ChannelsVocals > 0 || isM4A);
            chkKeys.Enabled = !radioDownmix.Checked && ChannelsKeys > 0 && !isM4A;
            chkGuitar.Enabled = !radioDownmix.Checked && (ChannelsGuitar > 0 || isM4A);
            chkBacking.Enabled = !radioDownmix.Checked && (ChannelsBacking > 0 || isM4A);
            chkCrowd.Enabled = ChannelsCrowd > 0 && !isM4A;
        }

        private void doWAV_Click(object sender, EventArgs e)
        {
            doWAV.Checked = true;
            doOGG.Checked = false;
            doFLAC.Checked = false;
            doOPUS.Checked = false;
            doMP3.Checked = false;
            AudioType = "WAV";
            AudioFormat = MoggSplitter.MoggSplitFormat.WAV;
        }

        private void doOGG_Click(object sender, EventArgs e)
        {
            doWAV.Checked = false;
            doOGG.Checked = true;
            doFLAC.Checked = false;
            doOPUS.Checked = false;
            doMP3.Checked = false;
            AudioType = "OGG";
            AudioFormat = MoggSplitter.MoggSplitFormat.OGG;
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

        private void doFLAC_Click(object sender, EventArgs e)
        {
            doWAV.Checked = false;
            doOGG.Checked = false;
            doFLAC.Checked = true;
            doOPUS.Checked = false;
            doMP3.Checked = false;
            AudioType = "FLAC";
            AudioFormat = MoggSplitter.MoggSplitFormat.FLAC;
        }

        private void doMP3_Click(object sender, EventArgs e)
        {
            doWAV.Checked = false;
            doOGG.Checked = false;
            doFLAC.Checked = false;
            doOPUS.Checked = false;
            doMP3.Checked = true;
            AudioType = "MP3";
            AudioFormat = MoggSplitter.MoggSplitFormat.MP3;
        }

        private void doOPUS_Click(object sender, EventArgs e)
        {
            doWAV.Checked = false;
            doOGG.Checked = false;
            doFLAC.Checked = false;
            doOPUS.Checked = true;
            doMP3.Checked = false;
            AudioType = "OPUS";
            AudioFormat = MoggSplitter.MoggSplitFormat.OPUS;
        }
    }
}