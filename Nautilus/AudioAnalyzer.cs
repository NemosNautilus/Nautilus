using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Nautilus.Properties;
using Nautilus.x360;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Mix;
using Un4seen.Bass.Misc;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;
using NautilusFREE;
using Un4seen.Bass.AddOn.Enc;

namespace Nautilus
{
    public partial class AudioAnalyzer : Form
    {
        private string InputFile;
        private readonly NemoTools Tools;
        private readonly DTAParser Parser;
        private List<string> InputFiles;
        private readonly List<Panel> ChannelPanels;
        private readonly List<Label> ChannelLabels; 
        private int WaveWidth;
        private int WaveHeight;
        private const int BUFFER = 20000;
        private const int HOR_SPACER = 14;
        private const int VER_SPACER = 42;
        private Size PanelSize;
        private static Color mMenuBackground;
        private string ImgToUpload;
        private string ImgURL;
        private readonly nTools nautilus3;
        private bool isFNF;

        public AudioAnalyzer(string file)
        {
            InitializeComponent();
            InputFile = file;
            InputFiles = new List<string>();
            ChannelPanels = new List<Panel>();
            ChannelLabels = new List<Label>();
            Tools = new NemoTools();
            Parser = new DTAParser();
            PanelSize = panelBackground.Size;
            mMenuBackground = menuStrip1.BackColor;
            menuStrip1.Renderer = new DarkRenderer();
            nautilus3 = new nTools();
            isFNF = false;
        }

        private void HandleDragDrop(object sender, DragEventArgs e)
        {
            InputFiles = ((string[]) e.Data.GetData(DataFormats.FileDrop)).ToList();
            picWorking.Visible = true;
            menuStrip1.Enabled = false;
            PreparetoDraw();
            backgroundWorker1.RunWorkerAsync();
        }

        private void PreparetoDraw()
        {
            if (showLegend.Checked)
            {
                panelWave.Location = new Point(HOR_SPACER, VER_SPACER);
                panelWave.Size = new Size(panelBackground.Width - (HOR_SPACER*2), panelBackground.Height - (VER_SPACER*2));
                lblStart.Visible = true;
                lblLength.Visible = true;
                lblFileInfo.Visible = true;
                lblFileName.Visible = true;
            }
            else
            {
                panelWave.Location = new Point(-1, -1);
                panelWave.Size = panelBackground.Size;
                lblStart.Visible = false;
                lblLength.Visible = false;
                lblFileInfo.Visible = false;
                lblFileName.Visible = false;
            }
            PanelSize = panelBackground.Size;
            WaveWidth = panelWave.Width;
            WaveHeight = panelWave.Height;
            MaximizeBox = false;
        }

        private void ProcessInputFile(string file)
        {
            isFNF = false;
            Parser.Songs = null;
            try
            {

                if (VariousFunctions.ReadFileType(file) == XboxFileType.STFS)
                {
                    var Splitter = new MoggSplitter();
                    if (Splitter.ExtractDecryptMogg(file, nautilus3, Parser))
                    {
                        InputFile = file;
                        DrawWaveForm(0);
                    }
                    else
                    {
                        var msg = Splitter.ErrorLog.Aggregate("Couldn't process the audio in CON file '" + file + "'\nSee the error log below:", (current, log) => current + "\n" + log);
                        MessageBox.Show(msg, Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                else if (Path.GetExtension(file).ToLowerInvariant() == ".mogg")
                {                    
                    if (nautilus3.DecM(File.ReadAllBytes(file), false, false, DecryptMode.ToMemory))
                    {
                        InputFile = file;
                        DrawWaveForm(0);
                        return;
                    }
                    MessageBox.Show("Mogg file '" + file + "' is encrypted with an encryption I don't understand, and I couldn't process it", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else if (Path.GetExtension(file).ToLowerInvariant() == ".pkg")
                {
                    InputFile = file;
                    ExtractPKG(file);
                }
                else
                {
                    switch (Path.GetExtension(file).ToLowerInvariant())
                    {
                        case ".m4a":
                            var fnfParser = new NemoFnFParser();
                            var fnfStream = fnfParser.m4aToBassStream(file, Path.GetFileName(file).Equals("preview.m4a") ? 2 : 10);
                            if (fnfStream == 0)
                            {
                                MessageBox.Show("File '" + file + "' is not a valid input file", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);                                
                            }
                            else
                            {
                                isFNF = true;
                                InputFile = file;
                                Parser.Songs = new List<SongData>();
                                var temp = Application.StartupPath + "\\bin\\temp";
                                BassEnc.BASS_Encode_Start(fnfStream, temp, BASSEncode.BASS_ENCODE_PCM | BASSEncode.BASS_ENCODE_AUTOFREE, null, IntPtr.Zero);
                                while (true)
                                {
                                    var buffer = new byte[20000];
                                    var c = Bass.BASS_ChannelGetData(fnfStream, buffer, buffer.Length);
                                    if (c <= 0) break;
                                }
                                Bass.BASS_StreamFree(fnfStream);
                                fnfStream = Bass.BASS_StreamCreateFile(temp, 0L, File.ReadAllBytes(temp).Length, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);
                                if (fnfStream == 0)
                                {
                                    MessageBox.Show("File '" + file + "' is not a valid input file", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    break;
                                }
                                File.Delete(temp);
                                DrawWaveForm(fnfStream);
                                Bass.BASS_ChannelFree(fnfStream);                                
                            }
                            break;
                        case ".ogg":
                        case ".wav":
                            InputFile = file;
                            Parser.Songs = new List<SongData>();
                            nautilus3.PlayingSongOggData = File.ReadAllBytes(file);
                            DrawWaveForm(0);
                            break;
                        case ".yarg_mogg":
                            if (nautilus3.DecY(file, DecryptMode.ToMemory))
                            {
                                InputFile = file;
                                DrawWaveForm(0);
                                return;
                            }
                            MessageBox.Show("YARG mogg file '" + file + "' is encrypted and I couldn't process it", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            break;
                        default:
                            MessageBox.Show("File '" + file + "' is not a valid input file", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearPanels()
        {
            foreach (var panel in ChannelPanels)
            {
                try
                {
                    panel.Invoke(new MethodInvoker(() => panel.Dispose()));
                }
                catch (Exception)
                {}
            }
            ChannelPanels.Clear();
            panelWave.BackgroundImage = null;
            panelBackground.BackgroundImage = null;
            lblFileInfo.Invoke(new MethodInvoker(() => lblFileInfo.Text = ""));
            lblFileName.Invoke(new MethodInvoker(() => lblFileName.Text = ""));
            lblStart.Invoke(new MethodInvoker(() => lblStart.Text = ""));
            lblLength.Invoke(new MethodInvoker(() => lblLength.Text = ""));
        }

        private void ClearLabels()
        {
            foreach (var label in ChannelLabels)
            {
                try
                {
                    label.Invoke(new MethodInvoker(() => label.Dispose()));
                }
                catch (Exception)
                { }
            }
            ChannelLabels.Clear();
        }

        private void GetTrackNames(out List<string> TrackNames, out List<bool> TrackIsStereo)
        {
            TrackNames = new List<string>();
            TrackIsStereo = new List<bool>();
            if (Parser.Songs == null && !isFNF) return;
            if (isFNF) //fixed song structure
            {
                TrackNames.Add("Drums");
                TrackIsStereo.Add(true);
                TrackNames.Add("Bass");
                TrackIsStereo.Add(true);
                TrackNames.Add("Guitar");
                TrackIsStereo.Add(true);
                TrackNames.Add("Vocals");
                TrackIsStereo.Add(true);
                TrackNames.Add("Backing");
                TrackIsStereo.Add(true);
                return;
            }
            switch (Parser.Songs[0].ChannelsDrums)
            {
                case 2:
                    TrackNames.Add("Drums");
                    TrackIsStereo.Add(true);
                    break;
                case 3:
                    TrackNames.Add("Drums (Kick)");
                    TrackIsStereo.Add(false);
                    TrackNames.Add("Drums (Kit)");
                    TrackIsStereo.Add(true);
                    break;
                case 4:
                    TrackNames.Add("Drums (Kick)");
                    TrackIsStereo.Add(false);
                    TrackNames.Add("Drums (Snare)");
                    TrackIsStereo.Add(false);
                    TrackNames.Add("Drums (Kit)");
                    TrackIsStereo.Add(true);
                    break;
                case 5:
                    TrackNames.Add("Drums (Kick)");
                    TrackIsStereo.Add(false);
                    TrackNames.Add("Drums (Snare)");
                    TrackIsStereo.Add(true);
                    TrackNames.Add("Drums (Kit)");
                    TrackIsStereo.Add(true);
                    break;
                case 6:
                    TrackNames.Add("Drums (Kick)");
                    TrackIsStereo.Add(true);
                    TrackNames.Add("Drums (Snare)");
                    TrackIsStereo.Add(true);
                    TrackNames.Add("Drums (Kit)");
                    TrackIsStereo.Add(true);
                    break;
            }
            switch (Parser.Songs[0].ChannelsBass)
            {
                case 1:
                case 2:
                    TrackNames.Add("Bass");
                    TrackIsStereo.Add(Parser.Songs[0].ChannelsBass == 2);
                    break;
            }
            switch (Parser.Songs[0].ChannelsGuitar)
            {
                case 1:
                case 2:
                    TrackNames.Add("Guitar");
                    TrackIsStereo.Add(Parser.Songs[0].ChannelsGuitar == 2);
                    break;
            }
            if (Parser.Songs[0].ChannelsVocals > 0 && Parser.Songs[0].ChannelsVocalsStart < Parser.Songs[0].ChannelsKeysStart) //vocals are first
            {
                switch (Parser.Songs[0].ChannelsVocals)
                {
                    case 1:
                    case 2:
                        TrackNames.Add("Vocals");
                        TrackIsStereo.Add(Parser.Songs[0].ChannelsVocals == 2);
                        break;
                }
                switch (Parser.Songs[0].ChannelsKeys)
                {
                    case 1:
                    case 2:
                        TrackNames.Add("Keys");
                        TrackIsStereo.Add(Parser.Songs[0].ChannelsKeys == 2);
                        break;
                }
            }
            else if (Parser.Songs[0].ChannelsKeys > 0)//keys are first
            {
                switch (Parser.Songs[0].ChannelsKeys)
                {
                    case 1:
                    case 2:
                        TrackNames.Add("Keys");
                        TrackIsStereo.Add(Parser.Songs[0].ChannelsKeys == 2);
                        break;
                }
                switch (Parser.Songs[0].ChannelsVocals)
                {
                    case 1:
                    case 2:
                        TrackNames.Add("Vocals");
                        TrackIsStereo.Add(Parser.Songs[0].ChannelsVocals == 2);
                        break;
                }                
            }   
            else if (Parser.Songs[0].ChannelsVocals > 0)//there are no keys, vocals are first
            {
                switch (Parser.Songs[0].ChannelsVocals)
                {
                    case 1:
                    case 2:
                        TrackNames.Add("Vocals");
                        TrackIsStereo.Add(Parser.Songs[0].ChannelsVocals == 2);
                        break;
                }
            }
            //a little hacky hack
            if (Parser.Songs[0].Source.Equals("beatles"))
            {
                Parser.Songs[0].ChannelsTotal = Parser.Songs[0].ChannelsBass + Parser.Songs[0].ChannelsDrums + Parser.Songs[0].ChannelsGuitar + Parser.Songs[0].ChannelsKeys + Parser.Songs[0].ChannelsVocals + Parser.Songs[0].ChannelsCrowd + 2;
            }
            var backing = Parser.Songs[0].ChannelsTotal - Parser.Songs[0].ChannelsDrums - Parser.Songs[0].ChannelsBass - Parser.Songs[0].ChannelsGuitar - Parser.Songs[0].ChannelsVocals - Parser.Songs[0].ChannelsKeys - Parser.Songs[0].ChannelsCrowd;
                
            if (Parser.Songs[0].ChannelsCrowd == 0 && backing > 0)
            {
                TrackNames.Add("Backing");
                TrackIsStereo.Add(backing == 2);
            }
            else if (Parser.Songs[0].ChannelsCrowd > 0)
            {
                if (Parser.Songs[0].ChannelsCrowdStart + Parser.Songs[0].ChannelsCrowd == Parser.Songs[0].ChannelsTotal) //crowd channels are last
                {
                    if (backing > 0)
                    {
                        TrackNames.Add("Backing");
                        TrackIsStereo.Add(backing == 2);
                    }
                    TrackNames.Add("Crowd");
                    TrackIsStereo.Add(Parser.Songs[0].ChannelsCrowd == 2);
                }
                else
                {
                    TrackNames.Add("Crowd"); //backing channels are last
                    TrackIsStereo.Add(Parser.Songs[0].ChannelsCrowd == 2);
                    if (backing > 0)
                    {
                        TrackNames.Add("Backing");
                        TrackIsStereo.Add(backing == 2);
                    }
                }
            }           
        }

        private static WaveForm GetNewWaveForm(bool isStereo)
        {
            var ColorBackground = Color.FromArgb(192, 192, 192);
            var ColorWaveForm = Color.FromArgb(50, 50, 200);
            var ColorLine = Color.FromArgb(50, 50, 200);
            var WaveImage = new WaveForm
            {
                FrameResolution = 0.01f,
                CallbackFrequency = 2000,
                ColorBackground = ColorBackground,
                ColorLeft = ColorWaveForm,
                ColorRight = ColorWaveForm,
                ColorMiddleLeft = ColorLine,
                ColorMiddleRight = ColorLine,
                DrawWaveForm = isStereo ? WaveForm.WAVEFORMDRAWTYPE.Stereo : WaveForm.WAVEFORMDRAWTYPE.Mono,                
            };
            return WaveImage;
        }

        private void DrawWaveForm(int BassStream)
        {
            ClearPanels();
            ClearLabels();
            
            var graphics = panelBackground.CreateGraphics();
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;

            try
            {
                lblFileName.Invoke(new MethodInvoker(() => lblFileName.Text = "Analysis of audio file: " + (Parser.Songs == null || Parser.Songs.Count == 0 ? Path.GetFileNameWithoutExtension(InputFile) : Parser.Songs[0].InternalName + ".mogg")));
            }
            catch 
            {
                lblFileName.Invoke(new MethodInvoker(() => lblFileName.Text = "Analysis of audio file: " + Path.GetFileNameWithoutExtension(InputFile) + ".mogg"));
            }
            if (BassStream == 0) //always except for Fortnite Festival .m4a files
            {
                BassStream = Bass.BASS_StreamCreateFile(nautilus3.GetOggStreamIntPtr(), 0L, nautilus3.PlayingSongOggData.Length, BASSFlag.BASS_STREAM_DECODE);
            }
            if (BassStream == 0)
            {
                MessageBox.Show("Error processing audio stream:\n" + Bass.BASS_ErrorGetCode(), Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            var length = Bass.BASS_ChannelGetLength(BassStream);
            var duration = Math.Round(Bass.BASS_ChannelBytes2Seconds(BassStream, length), 2);
            var audio_info = Bass.BASS_ChannelGetInfo(BassStream);
            string size;
            long dataLength = 0;
            if (isFNF)
            {
                dataLength = File.ReadAllBytes(InputFile).Length;
            }
            else
            {
                dataLength = nautilus3.PlayingSongOggData.Length;
            }
            if (dataLength >= 1048576)
            {
                size = Math.Round((double)dataLength / 1048576, 2) + " MB";
            }
            else
            {
                size = Math.Round((double)dataLength / 1024, 2) + " KB";
            }
            var minutes = Parser.GetSongDuration((duration * 1000).ToString(CultureInfo.InvariantCulture));
            lblStart.Invoke(new MethodInvoker(() => lblStart.Text = "0:00"));
            lblLength.Invoke(new MethodInvoker(() => lblLength.Text = minutes));
            var info = "Channels: " + audio_info.chans + "   |   Sample rate: " + audio_info.freq + " Hz   |   Length: " + duration + " seconds ("
                    + minutes + ")   |   File size: " + dataLength + " bytes (" + size + ")";
            lblFileInfo.Invoke(new MethodInvoker(() => lblFileInfo.Text = info));
            WaveForm WaveImage;
            switch (audio_info.chans)
            {
                case 1:
                    WaveImage = GetNewWaveForm(false);
                    if (!WaveImage.RenderStart(BassStream, false, true))
                    {
                        MessageBox.Show("Error rendering audio stream:\n" + Bass.BASS_ErrorGetCode(), Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        ClearPanels();
                        return;
                    }
                    panelWave.BackgroundImage = WaveImage.CreateBitmap(WaveWidth, WaveHeight, -1, -1, highQualityDrawing.Checked);
                    break;
                case 2:
                    WaveImage = GetNewWaveForm(true);   
                    if (!WaveImage.RenderStart(BassStream, false, true))
                    {
                        MessageBox.Show("Error rendering audio stream:\n" + Bass.BASS_ErrorGetCode(), Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        ClearPanels();
                        return;
                    }
                    panelWave.BackgroundImage = WaveImage.CreateBitmap(WaveWidth, WaveHeight, -1, -1, highQualityDrawing.Checked);
                    break;
                default:
                    try
                    {
                        List<string> TrackNames;
                        List<bool> TrackIsStereo;
                        var splitter = new MoggSplitter();
                        var isOgg = false;
                        if (Path.GetExtension(InputFile).ToLowerInvariant() == ".ogg" || Path.GetExtension(InputFile).ToLowerInvariant() == ".mogg" || VariousFunctions.ReadFileType(InputFile) == XboxFileType.STFS)
                        {
                            isOgg = true;
                        }
                        var ArrangedChannels = splitter.ArrangeStreamChannels(audio_info.chans, isOgg);
                        GetTrackNames(out TrackNames, out TrackIsStereo);
                        var height = WaveHeight / audio_info.chans;
                        var top = 0;
                        var index = 0;
                        var maxCount = TrackNames.Any() ? TrackNames.Count : audio_info.chans;
                        for (var i = 0; i < maxCount; i++)
                        {
                            var multiplier = TrackIsStereo.Any() && TrackIsStereo[i] ? 2 : 1;
                            var panel = new Panel();
                            Invoke(new MethodInvoker(delegate { panel.Parent = panelWave; }));
                            panel.Invoke(new MethodInvoker(() => panel.Left = -1));
                            panel.Invoke(new MethodInvoker(() => panel.Top = top - 1));
                            panel.Invoke(new MethodInvoker(() => panel.Width = WaveWidth + 2));
                            panel.Invoke(new MethodInvoker(() => panel.Height = (height * multiplier) + 1));
                            panel.Invoke(new MethodInvoker(() => panel.BackgroundImageLayout = ImageLayout.Stretch));
                            panel.Invoke(new MethodInvoker(() => panel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right));
                            if (outlineAudioTracks.Checked && i < maxCount - 1)
                            {
                                panel.Invoke(new MethodInvoker(() => panel.BorderStyle = BorderStyle.FixedSingle));
                            }
                            var map = TrackIsStereo.Any() && TrackIsStereo[i] ? new[] { ArrangedChannels[index], ArrangedChannels[index + 1], -1 } : 
                                new[]{ ArrangedChannels[index], -1 };
                            var channel_stream = BassMix.BASS_Split_StreamCreate(BassStream, BASSFlag.BASS_STREAM_DECODE, map);
                            WaveImage = GetNewWaveForm(TrackIsStereo.Any() && TrackIsStereo[i]);
                            if (!WaveImage.RenderStart(channel_stream, false, true))
                            {
                                MessageBox.Show("Error rendering audio stream:\n" + Bass.BASS_ErrorGetCode(), Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                ClearPanels();
                                ClearLabels();
                                return;
                            }
                            var endFrame = WaveImage.Position2Frames((length / audio_info.chans)*multiplier);
                            panel.BackgroundImage = WaveImage.CreateBitmap(WaveWidth, height * multiplier, -1, endFrame, highQualityDrawing.Checked);
                            var font = new Font("Times New Roman", 10f, FontStyle.Bold);
                            var label = new Label();
                            Invoke(new MethodInvoker(delegate { label.Parent = panel; }));
                            label.Invoke(new MethodInvoker(() => label.Visible = labelAudioChannels.Checked));
                            label.Invoke(new MethodInvoker(() => label.Location = new Point(3, 3)));
                            label.Invoke(new MethodInvoker(() => label.BackColor = Color.Transparent));
                            label.Invoke(new MethodInvoker(() => label.ForeColor = Color.White));
                            label.Invoke(new MethodInvoker(() => label.Font = font));
                            label.Invoke(new MethodInvoker(() => label.Text = TrackNames.Count > 0 ? TrackNames[i] : "chan. " + i));
                            ChannelLabels.Add(label);
                            Bass.BASS_StreamFree(channel_stream);
                            ChannelPanels.Add(panel);
                            top += (height*multiplier);
                            index += multiplier;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error rendering audio stream:\n" + ex.Message + "\n\n" + Bass.BASS_ErrorGetCode(), Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        ClearPanels();
                        ClearLabels();
                        return;
                    }
                    break;
            }
            Bass.BASS_StreamFree(BassStream);
            if (InputFiles.Count <= 1) return;
            var file = Path.GetDirectoryName(InputFile) + "\\" + Tools.CleanString(Path.GetFileNameWithoutExtension(InputFile), false) + ".jpg";
            TakeScreenshot(file);
        }

        private void AudioAnalyzer_Shown(object sender, EventArgs e)
        {
            if (!Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, Handle))
            {
                if (!Bass.BASS_ErrorGetCode().ToString().Contains("ALREADY"))
                {
                    MessageBox.Show("Error initializing BASS.NET\n" + Bass.BASS_ErrorGetCode());
                    return;
                }
            }
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_BUFFER, BUFFER);
            Application.DoEvents();
            if (string.IsNullOrWhiteSpace(InputFile) || !File.Exists(InputFile)) return;
            InputFiles.Add(InputFile);
            picWorking.Visible = true;
            menuStrip1.Enabled = false;
            PreparetoDraw();
            backgroundWorker1.RunWorkerAsync();
        }

        private void HandleDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            foreach (var file in InputFiles)
            {
                ProcessInputFile(file);
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            picWorking.Visible = false;
            menuStrip1.Enabled = true;
            MaximizeBox = true;
        }

        private void AudioAnalyzer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!picWorking.Visible && !backgroundWorker1.IsBusy && !backgroundWorker2.IsBusy)
            {
                Bass.BASS_Free();
                Tools.DeleteFolder(Application.StartupPath + "\\audio\\", true);                
                return;                
            }
            MessageBox.Show("Please wait until the current process finishes", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            e.Cancel = true;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void uploadToImgurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy || backgroundWorker2.IsBusy) return;
            if (string.IsNullOrWhiteSpace(InputFile))
            {
                MessageBox.Show("Nothing to save", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            ImgToUpload = Path.GetTempPath() + "temp.jpg";
            if (!TakeScreenshot(ImgToUpload))
            {
                MessageBox.Show("Failed to save visual, please try again", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            menuStrip1.Enabled = false;
            picWorking.Visible = true;
            backgroundWorker2.RunWorkerAsync();
        }

        private bool TakeScreenshot(string path)
        {
            using (var bitmap = new Bitmap(PanelSize.Width, PanelSize.Height))
            {
                //this method doesn't scale when using display/font scaling - needs fixing
                var location = new Point(0, 0);
                Invoke(new MethodInvoker(delegate { location = PointToScreen(panelBackground.Location); }));
                var g = Graphics.FromImage(bitmap);
                g.CopyFromScreen(location, new Point(0, 0), PanelSize, CopyPixelOperation.SourceCopy);
                
                var myEncoder = Encoder.Quality;
                var myEncoderParameters = new EncoderParameters(1);
                var myEncoderParameter = new EncoderParameter(myEncoder, 100L);
                myEncoderParameters.Param[0] = myEncoderParameter;
                var myImageCodecInfo = Tools.GetEncoderInfo("image/jpeg");
                Tools.DeleteFile(path);
                bitmap.Save(path, myImageCodecInfo, myEncoderParameters);
            }
            return File.Exists(path);
        }
        
        private void saveToFile_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy || backgroundWorker2.IsBusy) return;
            if (string.IsNullOrWhiteSpace(InputFile))
            {
                MessageBox.Show("Nothing to save", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            var file = Path.GetDirectoryName(InputFile) + "\\" + Tools.CleanString(Path.GetFileNameWithoutExtension(InputFile), false) + ".jpg";
            if (TakeScreenshot(file))
            {
                if (MessageBox.Show("Saved visual successfully\nClick OK to open", Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) != DialogResult.OK)
                {
                    return;
                }
                Process.Start(file);
                return;
            }
            MessageBox.Show("Failed to save visual, please try again", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void labelEachAudioChannel_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy || backgroundWorker2.IsBusy)
            {
                MessageBox.Show("Wait for the process to finish", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                labelAudioChannels.Checked = !labelAudioChannels.Checked;
                return;
            }
            if (!ChannelLabels.Any()) return;
            foreach (var label in ChannelLabels)
            {
                label.Visible = labelAudioChannels.Checked;
            }
        }

        private void AudioAnalyzer_Resize(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy || backgroundWorker2.IsBusy) return;
            PreparetoDraw();
            if (!InputFiles.Any())
            {
                MaximizeBox = true;
                return;
            }
            picWorking.Visible = true;
            menuStrip1.Enabled = false;
            backgroundWorker1.RunWorkerAsync();
        }

        private void showLegend_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy || backgroundWorker2.IsBusy)
            {
                MessageBox.Show("Wait for the process to finish", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                showLegend.Checked = !showLegend.Checked;
                return;
            }
            PreparetoDraw();
            if (!InputFiles.Any())
            {
                MaximizeBox = true;
                return;
            }
            picWorking.Visible = true;
            menuStrip1.Enabled = false;
            backgroundWorker1.RunWorkerAsync();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var message = Tools.ReadHelpFile("aa");
            var help = new HelpForm(Text + " - Help", message);
            help.ShowDialog();
        }

        private sealed class DarkRenderer : ToolStripProfessionalRenderer
        {
            public DarkRenderer() : base(new DarkColors()) { }
        }

        private sealed class DarkColors : ProfessionalColorTable
        {
            public override Color ImageMarginGradientBegin
            {
                get { return mMenuBackground; }
            }
            public override Color ImageMarginGradientEnd
            {
                get { return mMenuBackground; }
            }
            public override Color ImageMarginGradientMiddle
            {
                get { return mMenuBackground; }
            }
            public override Color ToolStripDropDownBackground
            {
                get { return mMenuBackground; }
            }
        }

        private void backgroundWorker2_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            ImgURL = Tools.UploadToImgur(ImgToUpload);
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            menuStrip1.Enabled = true;
            picWorking.Visible = false;
            if (string.IsNullOrWhiteSpace(ImgURL))
            {
                MessageBox.Show("Failed to upload to Imgur, please try again", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            Clipboard.SetText(ImgURL);
            if (MessageBox.Show("Uploaded to Imgur successfully\nClick OK to open link in browser", Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) != DialogResult.OK)
            {
                return;
            }
            Process.Start(ImgURL);
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

        private void ExtractPKG(string file)
        {
            var folder = Application.StartupPath + "\\audio\\";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var shortPath = Path.GetFileNameWithoutExtension(file).Replace(" ", "").Replace("-", "").Replace("_", "").Trim();
            if (shortPath.Length > 10) //to avoid very lengthy PKG file names, which can cause issues with long paths on Windows
            {
                shortPath = shortPath.Substring(0, 10) + "_ex";
            }
            var outFolder = folder + shortPath;
            Tools.DeleteFolder(outFolder, true);
            if (!Tools.ExtractPS3PKG(file, outFolder))
            {
                MessageBox.Show("Failed to process that PKG file, can't analyze", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            var DTA = Directory.GetFiles(outFolder, "songs.dta", SearchOption.AllDirectories);
            if (DTA.Count() == 0)
            {
                MessageBox.Show("No songs.dta file found, can't analyze", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            Parser.ReadDTA(File.ReadAllBytes(DTA[0]));
            var internalName = Parser.Songs[0].InternalName;
            var MOGG = Directory.GetFiles(outFolder, internalName + ".mogg", SearchOption.AllDirectories);            
            if (!nautilus3.DecM(File.ReadAllBytes(MOGG[0]), false, false, DecryptMode.ToMemory))
            {
                MessageBox.Show("Failed to decrypt mogg file, can't analyze", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            DrawWaveForm(0);
        }
    }
}
