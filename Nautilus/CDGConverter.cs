using LibVLCSharp.Shared;
using NAudio.Midi;
using Nautilus.x360;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Un4seen.Bass;

namespace Nautilus
{
    public partial class CDGConverter : Form
    {
        private readonly NemoTools Tools;
        private readonly DTAParser Parser;
        private MoggSplitter moggSplitter;
        private List<TempoEvent> TempoEvents;
        private List<TimeSignature> TimeSignatures;
        private int TicksPerQuarter;
        private List<KaraokeLyric> VocalLyrics;
        private List<KaraokeLyric> Harm1Lyrics;
        private List<KaraokeLyric> Harm2Lyrics;
        private List<KaraokeLyric> Harm3Lyrics;
        private List<MIDINote> Harm1Notes;
        private List<MIDINote> Harm2Notes;
        private List<MIDINote> Harm3Notes;
        private List<MIDINote> VocalNotes;
        private List<LyricPhrase> VocalPhrases;
        private List<LyricPhrase> Harm1Phrases;
        private List<LyricPhrase> Harm2Phrases;
        private List<LyricPhrase> Harm3Phrases;
        private List<string> inputFiles;
        private string customColor = "";
        private string backgroundColor = "";
        private string textColor1 = "";
        private string highlightColor1 = "";
        private string textColor2 = "";
        private string highlightColor2 = "";
        private string textColor3 = "";
        private string highlightColor3 = "";
        private string strokeColor = "";
        private int fileCounter = 0;
        private int successCounter = 0;
        private int failureCounter = 0;
        private List<string> failedSongs = new List<string>();
        private string vocalOption = "";
        private bool doKeepTOML;
        private bool doKeepWAV;
        private string cdgOutput = "";
        private bool doSoloVocals = true;
        private bool doHarm2 = false;
        private bool doHarm3 = false;
        private readonly string configFile;
        private PrivateFontCollection pfc = new PrivateFontCollection();
        private string ActiveFont;
        private string ActiveFontName;
        private bool doCDG = true;
        private bool doMP4 = true;
        private byte[] albumArtX;
        private int totalFrames;
        private double songDuration;
        private Image BackgroundOverride = null;
        private Image KaraokeLogo = null;
        private bool do4KResolution = false;
        private DateTime BatchTimerStart;
        private DateTime BatchTimerEnd;
        private DateTime SongTimerStart;
        private DateTime SongTimerEnd;
        private int frameRate = 30;
        private bool cancelProcess;
        private int timeGap = 5000;
        private double highlightDelay = 500;
        private bool doShowLoadingBar;
        private bool doShowAnimatedNotes;
        private const string loadingBar = "████████████████";
        private const string loadingBarXL = "████████████████████████████████";
        private string cdgMode = "delayed";
        private string videoFilter = "";
        private bool doEnableHighlightAnimation = true;
        private double syncOffsetMs = 0.0;
        private bool isLoading = false;
        private bool enablePreview = false;
        private bool enableCDGStroke = true;
        private bool enableMP4Stroke = false;
        private bool enableTitleShadows = false;
        private bool solidColorBackground = true;
        private bool staticImageBackground = false;
        private bool animatedVideoBackground = false;
        private bool displayTempo = true;
        private bool displayAlbumArt = true;
        private LibVLC _bgPreviewLibVLC;
        private MediaPlayer _bgPreviewPlayer;
        private string _customBackgroundVideoPath;
        private LibVLCSharp.WinForms.VideoView videoPreview;

        List<string> karaokeColorHexes = new List<string>
        {
            "#000000", //Black
            "#FFFFFF", //White
            "#FF0000", //Red
            "#00FF00", //Green
            "#2299FF", //Approximate DodgerBlue
            "#FFFF00", //Yellow
            "#00FFFF", //Cyan
            "#FF00FF", //Magenta
            "#888888", //Gray
            "#FF8800", //Orange
            "#FF88FF", //Pink
            "#8800FF", //Purple
            "#008888", //Teal
            "#88FF00", //Lime
            "#000088", //Navy
            "#884400", //Brown
            "#545454", //Dark Gray
        };

        public CDGConverter()
        {
            InitializeComponent();

            Tools = new NemoTools();
            Parser = new DTAParser();
            moggSplitter = new MoggSplitter();
            inputFiles = new List<string>();
            NewFile();
            configFile = Application.StartupPath + "\\bin\\config\\karaoke.config";            
            InitializeBackgroundVideoView();
            InitializeBackgroundPreview();
            HookBackgroundPreviewLoop();
        }

        private void InitializeBackgroundVideoView()
        {
            videoPreview = new LibVLCSharp.WinForms.VideoView();
            videoPreview.Name = "videoViewBackgroundPreview";
            videoPreview.Visible = false;
            videoPreview.Location = picPreview.Location;
            videoPreview.Size = picPreview.Size;
            videoPreview.Anchor = picPreview.Anchor;
            videoPreview.BackColor = Color.Black;

            this.Controls.Add(videoPreview);
            videoPreview.BringToFront();
        }

        private void InitializeBackgroundPreview()
        {
            if (_bgPreviewLibVLC == null)
            {
                Core.Initialize();
                _bgPreviewLibVLC = new LibVLC();
            }

            if (_bgPreviewPlayer == null)
            {
                _bgPreviewPlayer = new MediaPlayer(_bgPreviewLibVLC);
                _bgPreviewPlayer.Mute = true;
                videoPreview.MediaPlayer = _bgPreviewPlayer;
            }
        }

        private void DroppedPreviewVideo(string videoPath)
        {
            InitializeBackgroundPreview();

            _customBackgroundVideoPath = videoPath;

            cboBackground.SelectedIndex = 0;//black
            picPreview.Visible = false;
            videoPreview.Visible = true;
            solidColorToolStripMenuItem.Checked = false;
            staticImageBackgroundToolStripMenuItem.Checked = false;
            animatedBackgroundToolStripMenuItem.Checked = true;
            picBackground1.Image = null;
            picBackground2.Image = null;
            picBackground3.Image = null;
            animatedVideoBackground = true;
            UpdateBackgroundColors(Color.Black);

            try
            {
                var oldMedia = _bgPreviewPlayer.Media;
                _bgPreviewPlayer.Media = null;

                if (oldMedia != null)
                {
                    try { oldMedia.Dispose(); } catch { }
                }

                var media = new Media(_bgPreviewLibVLC, new Uri(videoPath));
                _bgPreviewPlayer.Media = media;
                _bgPreviewPlayer.Mute = true;
                _bgPreviewPlayer.Play();
            }
            catch (Exception ex)
            {
                Log("Error previewing background video: " + ex.Message);
            }
        }

        private void HookBackgroundPreviewLoop()
        {
            _bgPreviewPlayer.EndReached += (s, e) =>
            {
                try
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        if (_bgPreviewPlayer != null && !string.IsNullOrEmpty(_customBackgroundVideoPath))
                        {
                            _bgPreviewPlayer.Stop();

                            using (var media = new Media(_bgPreviewLibVLC, new Uri(_customBackgroundVideoPath)))
                            {
                                _bgPreviewPlayer.Media = media;
                            }

                            _bgPreviewPlayer.Play();
                        }
                    }));
                }
                catch { }
            };
        }

        private void NewFile()
        {
            albumArtX = new byte[0]; 
            VocalLyrics = new List<KaraokeLyric>();
            VocalNotes = new List<MIDINote>();
            VocalPhrases = new List<LyricPhrase>();
            Harm1Phrases = new List<LyricPhrase>();
            Harm2Phrases = new List<LyricPhrase>();
            Harm3Phrases = new List<LyricPhrase>();
            Harm1Lyrics = new List<KaraokeLyric>();
            Harm2Lyrics = new List<KaraokeLyric>();
            Harm3Lyrics = new List<KaraokeLyric>();            
            Harm1Notes = new List<MIDINote>();
            Harm2Notes = new List<MIDINote>();
            Harm3Notes = new List<MIDINote>();
        }
        
        private void SaveConfig()
        {
            var sw = new StreamWriter(configFile, false);
            sw.WriteLine("BackgroundColorIndex=" + cboBackground.SelectedIndex.ToString());
            sw.WriteLine("Text1BaseColorIndex=" + cboTextColor1.SelectedIndex.ToString());
            sw.WriteLine("Text1HighlightColorIndex=" + cboTextHighlight1.SelectedIndex.ToString());
            sw.WriteLine("Text2BaseColorIndex=" + cboTextColor2.SelectedIndex.ToString());
            sw.WriteLine("Text2HighlightColorIndex=" + cboTextHighlight2.SelectedIndex.ToString());
            sw.WriteLine("Text3BaseColorIndex=" + cboTextColor3.SelectedIndex.ToString());
            sw.WriteLine("Text3HighlightColorIndex=" + cboTextHighlight3.SelectedIndex.ToString());
            sw.WriteLine("FontIndex=" + cboFont.SelectedIndex.ToString());
            sw.WriteLine("StaticImageBackground=" + (staticImageBackgroundToolStripMenuItem.Checked ? "True" : "False"));
            sw.WriteLine("RemoveVocals=" + (radioRemove.Checked ? "True" : "False"));
            sw.WriteLine("KeepVocals=" + (radioKeep.Checked ? "True" : "False"));
            sw.WriteLine("KeepTOML=" + (keeptomlFileToolStripMenuItem.Checked ? "True" : "False"));
            sw.WriteLine("KeepWAV=" + (keepwavFileToolStripMenuItem.Checked ? "True" : "False"));
            sw.WriteLine("SoloVocals=" + (radioSoloVocals.Checked ? "True" : "False"));
            sw.WriteLine("Harmonies2=" + (radioHarm2.Checked ? "True" : "False"));
            sw.WriteLine("Harmonies3=" + (radioHarm3.Checked ? "True" : "False"));
            sw.WriteLine("DoCDG+MP3=" + (chkCDG.Checked ? "True" : "False"));
            sw.WriteLine("DoMP4=" + (chkMP4.Checked ? "True" : "False"));
            sw.WriteLine("Do1080P=" + (HighRes.Checked ? "True" : "False"));
            sw.WriteLine("Do4K=" + (HigherRes.Checked ? "True" : "False"));
            sw.WriteLine("HighlightDelay25=" + (quarterSecondDelay.Checked ? "True" : "False"));
            sw.WriteLine("HighlightDelay50=" + (halfSecondDelay.Checked ? "True" : "False"));
            sw.WriteLine("HighlightDelay75=" + (threeQuarterSecondDelay.Checked ? "True" : "False"));
            sw.WriteLine("HighlightDelay100=" + (wholeSecondDelay.Checked ? "True" : "False"));
            sw.WriteLine("HighlightDelay125=" + (OneTwentyFiveDelay.Checked ? "True" : "False"));
            sw.WriteLine("HighlightDelay150=" + (OneFiftyDelay.Checked ? "True" : "False"));
            sw.WriteLine("HighlightDelay175=" + (OneSeventyFiveDelay.Checked ? "True" : "False"));
            sw.WriteLine("HighlightDelay200=" + (TwoSecondDelay.Checked ? "True" : "False"));
            sw.WriteLine("DoLoadingBars=" + (enableLoadingBar.Checked ? "True" : "False"));
            sw.WriteLine("LoadingBarGap3=" + (loadingBar3Secs.Checked ? "True" : "False"));
            sw.WriteLine("LoadingBarGap5=" + (loadingBar5Secs.Checked ? "True" : "False"));
            sw.WriteLine("LoadingBarGap10=" + (loadingBar10Secs.Checked ? "True" : "False"));
            sw.WriteLine("DoAnimatedNotes=" + (enableAnimatedNotes.Checked ? "True" : "False"));
            sw.WriteLine("CDGClearModePage=" + (pageMode.Checked ? "True" : "False"));
            sw.WriteLine("CDGClearModeDelayed=" + (delayedMode.Checked ? "True" : "False"));
            sw.WriteLine("CDGClearModeEager=" + (eagerMode.Checked ? "True" : "False"));
            sw.WriteLine("FPS15=" + (FifteenFPS.Checked ? "True" : "False"));
            sw.WriteLine("FPS30=" + (ThirtyFPS.Checked ? "True" : "False"));
            sw.WriteLine("FPS60=" + (SixtyFPS.Checked ? "True" : "False"));
            sw.WriteLine("DoHighlightLeadAnimation=" + (enableHighlightAnimation.Checked ? "True" : "False"));
            sw.WriteLine("AudioVideoOffset=" + syncOffsetMs);
            sw.WriteLine("StrokeColorIndex=" + cboStroke.SelectedIndex.ToString());
            sw.WriteLine("CustomColor=" + customColor);
            sw.WriteLine("EnableRenderingPreview=" + (showRenderingPreview.Checked ? "True" : "False"));
            sw.WriteLine("EnableCDGStroke=" + (enableCDGStrokeToolStripMenuItem.Checked ? "True" : "False"));
            sw.WriteLine("EnableMP4Stroke=" + (enableMP4StrokeToolStripMenuItem.Checked ? "True" : "False"));
            sw.WriteLine("EnableTitleCardShadows=" + (enableMP4TitleCardShadows.Checked ? "True" : "False"));
            sw.WriteLine("SolidColorBackground=" + (solidColorToolStripMenuItem.Checked ? "True" : "False"));
            sw.WriteLine("AnimatedBackground=" + (animatedBackgroundToolStripMenuItem.Checked ? "True" : "False"));
            sw.WriteLine("NoHighlightDelay=" + (noDelayToolStripMenuItem.Checked ? "True" : "False"));
            sw.WriteLine("DisplayTempoOnTitleCard=" + (displayTempoOnTitleCard.Checked ? "True" : "False"));
            sw.WriteLine("DisplayAlbumArtOnTitleCard=" + (displayAlbumArtOnTitleCard.Checked ? "True" : "False"));
            sw.Dispose();
        }

        private void LoadConfig()
        {
            if (!File.Exists(configFile)) return;
            isLoading = true;
            var sr = new StreamReader(configFile);
            try
            {
                cboBackground.SelectedIndex = Convert.ToInt16(Tools.GetConfigString(sr.ReadLine()));
                cboTextColor1.SelectedIndex = Convert.ToInt16(Tools.GetConfigString(sr.ReadLine()));
                cboTextHighlight1.SelectedIndex = Convert.ToInt16(Tools.GetConfigString(sr.ReadLine()));
                cboTextColor2.SelectedIndex = Convert.ToInt16(Tools.GetConfigString(sr.ReadLine()));
                cboTextHighlight2.SelectedIndex = Convert.ToInt16(Tools.GetConfigString(sr.ReadLine()));
                cboTextColor3.SelectedIndex = Convert.ToInt16(Tools.GetConfigString(sr.ReadLine()));
                cboTextHighlight3.SelectedIndex = Convert.ToInt16(Tools.GetConfigString(sr.ReadLine()));
                cboFont.SelectedIndex = Convert.ToInt16(Tools.GetConfigString(sr.ReadLine()));
                staticImageBackgroundToolStripMenuItem.Checked = sr.ReadLine().Contains("True");
                radioRemove.Checked = sr.ReadLine().Contains("True");
                radioKeep.Checked = sr.ReadLine().Contains("True");
                keeptomlFileToolStripMenuItem.Checked = sr.ReadLine().Contains("True");
                keepwavFileToolStripMenuItem.Checked = sr.ReadLine().Contains("True");
                radioSoloVocals.Checked = sr.ReadLine().Contains("True");
                radioHarm2.Checked = sr.ReadLine().Contains("True");
                radioHarm3.Checked = sr.ReadLine().Contains("True");
                chkCDG.Checked = sr.ReadLine().Contains("True");
                chkMP4.Checked = sr.ReadLine().Contains("True");  
                HighRes.Checked = sr.ReadLine().Contains("True");
                HigherRes.Checked = sr.ReadLine().Contains("True");
                quarterSecondDelay.Checked = sr.ReadLine().Contains("True");
                halfSecondDelay.Checked = sr.ReadLine().Contains("True");
                threeQuarterSecondDelay.Checked = sr.ReadLine().Contains("True");
                wholeSecondDelay.Checked = sr.ReadLine().Contains("True");
                OneTwentyFiveDelay.Checked = sr.ReadLine().Contains("True");
                OneFiftyDelay.Checked = sr.ReadLine().Contains("True");
                OneSeventyFiveDelay.Checked = sr.ReadLine().Contains("True");
                TwoSecondDelay.Checked = sr.ReadLine().Contains("True");
                enableLoadingBar.Checked = sr.ReadLine().Contains("True");
                loadingBar3Secs.Checked = sr.ReadLine().Contains("True");
                loadingBar5Secs.Checked = sr.ReadLine().Contains("True");
                loadingBar10Secs.Checked = sr.ReadLine().Contains("True");
                enableAnimatedNotes.Checked = sr.ReadLine().Contains("True");
                pageMode.Checked = sr.ReadLine().Contains("True");
                delayedMode.Checked = sr.ReadLine().Contains("True");
                eagerMode.Checked = sr.ReadLine().Contains("True");
                FifteenFPS.Checked = sr.ReadLine().Contains("True");
                ThirtyFPS.Checked = sr.ReadLine().Contains("True");
                SixtyFPS.Checked = sr.ReadLine().Contains("True");
                enableHighlightAnimation.Checked = sr.ReadLine().Contains("True");
                syncOffsetMs = Convert.ToDouble(Tools.GetConfigString(sr.ReadLine()));
                cboStroke.SelectedIndex = Convert.ToInt16(Tools.GetConfigString(sr.ReadLine()));
                customColor = Tools.GetConfigString(sr.ReadLine());
                showRenderingPreview.Checked = sr.ReadLine().Contains("True");
                enableCDGStrokeToolStripMenuItem.Checked = sr.ReadLine().Contains("True");
                enableMP4StrokeToolStripMenuItem.Checked = sr.ReadLine().Contains("True");
                enableMP4TitleCardShadows.Checked = sr.ReadLine().Contains("True");
                solidColorToolStripMenuItem.Checked = sr.ReadLine().Contains("True");
                animatedBackgroundToolStripMenuItem.Checked = sr.ReadLine().Contains("True");
                noDelayToolStripMenuItem.Checked = sr.ReadLine().Contains("True");
                displayTempoOnTitleCard.Checked = sr.ReadLine().Contains("True");
                displayAlbumArtOnTitleCard.Checked = sr.ReadLine().Contains("True");
                UpdateTextParents();
                ModeSanityCheck();
                if (cboBackground.SelectedIndex == cboBackground.Items.Count - 1)
                {
                    if (string.IsNullOrEmpty(customColor))
                    {
                        cboBackground.SelectedIndex = 0;
                    }
                    else
                    {
                        var backColor = ColorTranslator.FromHtml(customColor);
                        UpdateBackgroundColors(backColor);
                    }
                }
                if (staticImageBackgroundToolStripMenuItem.Checked)
                {
                    var defaultImagePath = Application.StartupPath + "\\bin\\images\\background.png";
                    if (File.Exists(defaultImagePath))
                    {
                        picPreview.Image = Image.FromFile(defaultImagePath);
                        picBackground1.Image = picPreview.Image;
                        picBackground2.Image = picPreview.Image;
                        picBackground3.Image = picPreview.Image;
                    }
                }
                else if (animatedBackgroundToolStripMenuItem.Checked)
                {
                    var defaultVideoPath = Application.StartupPath + "\\bin\\images\\background.mp4";
                    if (File.Exists(defaultVideoPath))
                    {
                        DroppedPreviewVideo(defaultVideoPath);
                    }
                }
            }
            catch 
            {
                sr.Dispose();
                //Tools.DeleteFile(configFile);
                isLoading = false;
                return;
            }
            sr.Dispose();
            isLoading = false;
        }

        private void CDGConverter_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
        }

        private bool droppedImage = false;
        private void CDGConverter_DragDrop(object sender, DragEventArgs e)
        {
            if (picWorking.Visible || backgroundWorker1.IsBusy)
            {
                MessageBox.Show("Please let the current queue finish", "Busy", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            var ext = Path.GetExtension(files[0]);
            switch (ext)
            {
                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".bmp":
                    BackgroundOverride = Image.FromFile(files[0]);
                    solidColorToolStripMenuItem.Checked = false;
                    animatedBackgroundToolStripMenuItem.Checked = false;
                    staticImageBackgroundToolStripMenuItem.Checked = true;
                    droppedImage = true;
                    picPreview.Image = BackgroundOverride;
                    picBackground1.Image = picPreview.Image;
                    picBackground2.Image = picPreview.Image;
                    picBackground3.Image = picPreview.Image;
                    videoPreview.Visible = false;
                    picPreview.Visible = true;
                    animatedVideoBackground = false;
                    return;
                case ".mp4":
                case ".mov":
                case ".mkv":
                case ".avi":
                case ".wmv":
                case ".webm":
                    DroppedPreviewVideo(files[0]);
                    return;                    
                case ".toml":
                    Log("Received TOML file '" + files[0] + "' ... converting to CDG+MP3");
                    EnableDisable(false);
                    cdgOutput = "";
                    Application.DoEvents();
                    CreateCDG(files[0]);
                    Log("Completed manual TOML to CDG+MP3 attempt... see below:");
                    //Log(cdgOutput);               
                    EnableDisable(true);
                    cdgOutput = "";
                    return;
            }
            if (VariousFunctions.ReadFileType(files[0]) != XboxFileType.STFS)
            {
                Log("Received invalid file '" + files[0] + "' ... can't proceed");
                MessageBox.Show("That's not a valid file to drop here", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            inputFiles = files.ToList();
            if (cboBackground.SelectedIndex < cboBackground.Items.Count - 1)
            {
                backgroundColor = karaokeColorHexes[cboBackground.SelectedIndex];
            }
            else
            {
                backgroundColor = customColor;
            }            
            strokeColor = karaokeColorHexes[cboStroke.SelectedIndex];
            textColor1 = karaokeColorHexes[cboTextColor1.SelectedIndex];
            highlightColor1 = karaokeColorHexes[cboTextHighlight1.SelectedIndex];
            textColor2 = karaokeColorHexes[cboTextColor2.SelectedIndex];
            highlightColor2 = karaokeColorHexes[cboTextHighlight2.SelectedIndex];
            textColor3 = karaokeColorHexes[cboTextColor3.SelectedIndex];
            highlightColor3 = karaokeColorHexes[cboTextHighlight3.SelectedIndex];
            strokeColor = karaokeColorHexes[cboStroke.SelectedIndex];
            doKeepTOML = keeptomlFileToolStripMenuItem.Checked;
            doSoloVocals = radioSoloVocals.Checked;
            doHarm2 = radioHarm2.Checked;
            doHarm3 = radioHarm3.Checked;
            ActiveFont = cboFont.Text.Split('|')[1].Trim();
            ActiveFontName = cboFont.Text.Split('|')[0].Trim();
            doCDG = chkCDG.Checked;
            doMP4 = chkMP4.Checked;
            doKeepWAV = keepwavFileToolStripMenuItem.Checked;
            do4KResolution = HigherRes.Checked;
            enablePreview = showRenderingPreview.Checked;
            enableCDGStroke = enableCDGStrokeToolStripMenuItem.Checked;
            enableMP4Stroke = enableMP4StrokeToolStripMenuItem.Checked;
            enableTitleShadows = enableMP4TitleCardShadows.Checked;
            var remove = "drums|bass|guitar|keys|backing|NOcrowd";
            var keep = "allstems|NOcrowd";
            vocalOption = radioRemove.Checked ? remove : keep;
            solidColorBackground = solidColorToolStripMenuItem.Checked;
            staticImageBackground = staticImageBackgroundToolStripMenuItem.Checked;
            animatedVideoBackground = animatedBackgroundToolStripMenuItem.Checked;
            if (!droppedImage || BackgroundOverride == null)
            {
                try
                {
                    BackgroundOverride = Image.FromFile(Application.StartupPath + "\\bin\\images\\background.png");
                }
                catch
                {
                    BackgroundOverride = null;
                }
            }
            try
            {
                KaraokeLogo = Image.FromFile(Application.StartupPath + "\\bin\\images\\logo.png");
            }
            catch
            {
                KaraokeLogo = null;
            }
            if (noDelayToolStripMenuItem.Checked)
            {
                highlightDelay = 0;
            }
            else if (quarterSecondDelay.Checked)
            {
                highlightDelay = 250;
            }
            else if (halfSecondDelay.Checked)
            {
                highlightDelay = 500;
            }
            else if (threeQuarterSecondDelay.Checked)
            {
                highlightDelay = 750;
            }
            else if (wholeSecondDelay.Checked)
            {
                highlightDelay = 1000;
            }
            else if (OneTwentyFiveDelay.Checked)
            {
                highlightDelay = 1250;
            }
            else if (OneFiftyDelay.Checked)
            {
                highlightDelay = 1500;
            }
            else if (OneSeventyFiveDelay.Checked)
            {
                highlightDelay = 1750;
            }
            else if (TwoSecondDelay.Checked)
            {
                highlightDelay = 2000;
            }
            else
            {
                highlightDelay = 1500; //default to 1.5 seconds
            }
            doShowLoadingBar = enableLoadingBar.Checked;
            doShowAnimatedNotes = enableAnimatedNotes.Checked;
            if (loadingBar3Secs.Checked)
            {
                timeGap = 3000;
            }
            else if (loadingBar5Secs.Checked)
            {
                timeGap = 5000;
            }
            else if (loadingBar10Secs.Checked)
            {
                timeGap = 10000;
            }
            else
            {
                timeGap = 5000; //default to 5 seconds
            }
            if (FifteenFPS.Checked)
            {
                frameRate = 15;
                videoFilter = " -filter:v \"fps=30\""; //interpolation filter
            }
            else if (ThirtyFPS.Checked)
            {
                frameRate = 30;
                videoFilter = "";
            }
            else if (SixtyFPS.Checked)
            {
                frameRate = 60;
                videoFilter = "";
            }
            else
            {
                frameRate = 30;
                videoFilter = "";
            }
            if (pageMode.Checked)
            {
                cdgMode = "page";
            }
            else if (delayedMode.Checked)
            {
                cdgMode = "delayed";
            }
            else if (eagerMode.Checked)
            {
                cdgMode = "eager";
            }
            else
            {
                cdgMode = "delayed"; //default to this
            }
            doEnableHighlightAnimation = enableHighlightAnimation.Checked;
            displayTempo = displayTempoOnTitleCard.Checked;
            displayAlbumArt = displayAlbumArtOnTitleCard.Checked;
            EnableDisable(false);
            BatchTimerStart = DateTime.Now;
            Log("Batch processing start time is " + BatchTimerStart.ToString("hh:mm:ss tt"));
            backgroundWorker1.RunWorkerAsync();
        }

        private void EnableDisable(bool enable)
        {
            picWorking.Visible = !enable;
            menuStrip1.Enabled = enable;
            grpFormat.Enabled = enable;
            grpVocalParts.Enabled = enable;
            grpAudioOptions.Enabled = enable;
            grpHarm1.Enabled = enable;
            grpHarm2.Enabled = enable;
            grpHarm3.Enabled = enable;
            cboBackground.Enabled = enable;            
            cboFont.Enabled = enable;
            lblFontQuestion.Enabled = enable;
            btnCancel.Visible = !enable;
            cboStroke.Enabled = enable;
            lblStrokeQuestion.Enabled = enable;
        }

        private string CleanString(string str)
        {
            return str.Replace("#", "").Replace("^", "").Replace("\"", "").Replace("§", "‿").Replace(",", "").Replace("$", "");
        }

        private void BuildTimeSignatureList(MidiEventCollection midi)
        {
            TimeSignatures = new List<TimeSignature>();
            foreach (var ev in midi[0])
            {
                if (ev.CommandCode != MidiCommandCode.MetaEvent) continue;
                var meta = ev as MetaEvent;
                if (meta?.MetaEventType != MetaEventType.TimeSignature) continue;

                var ts = meta as TimeSignatureEvent;
                if (ts == null) continue;

                // The actual denominator is 2 raised to the power of denominator exponent
                int numerator = ts.Numerator;
                int denominator = (int)Math.Pow(2, ts.Denominator);

                var timeSig = new TimeSignature
                {
                    AbsoluteTime = ev.AbsoluteTime,
                    Numerator = numerator,
                    Denominator = denominator
                };

                TimeSignatures.Add(timeSig);
            }
        }
        
        private void BuildTempoList(MidiEventCollection midi)
        {
            TempoEvents = new List<TempoEvent>();

            double currentBpm = 120.0;
            double realTimeMs = 0.0;
            int relDeltaTicks = 0;

            foreach (var ev in midi[0])
            {
                relDeltaTicks += ev.DeltaTime;

                if (ev.CommandCode != MidiCommandCode.MetaEvent) continue;

                var meta = ev as MetaEvent;
                if (meta?.MetaEventType != MetaEventType.SetTempo) continue;

                var tempoEvent = meta as NAudio.Midi.TempoEvent;
                if (tempoEvent == null) continue;

                // Convert ticks to ms using current tempo
                double deltaMs = relDeltaTicks / (double)TicksPerQuarter * (60000.0 / currentBpm);
                realTimeMs += deltaMs;

                // Convert microseconds/quarter note to BPM
                currentBpm = 60000000.0 / tempoEvent.MicrosecondsPerQuarterNote;
                relDeltaTicks = 0;

                TempoEvents.Add(new TempoEvent
                {
                    AbsoluteTime = ev.AbsoluteTime,
                    RealTime = realTimeMs,
                    BPM = currentBpm
                });
            }
        }

        private void ProcessFile(string file)
        {
            NewFile();
            Parser.ExtractDTA(file);
            Parser.ReadDTA(Parser.DTA);

            var message = "";
            if (Parser.Songs.Count > 1)
            {
                message = "This is a pack, dePACK first then try again, stopping";
                Log(message);
                MessageBox.Show(message,"Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (!Parser.Songs.Any())
            {
                message = "Couldn't find any songs in that file, stopping";
                Log(message);
                MessageBox.Show(message,"Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (Parser.Songs[0].VocalParts == 0)
            {
                message = "That song has no vocals, stopping";
                Log(message);
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            var title = CleanString(Parser.Songs[0].Artist).Replace("feat.", "ft.").Replace("featuring", "ft.") + " - " + CleanString(Parser.Songs[0].Name).Replace("feat.", "ft.").Replace("featuring", "ft.");
            Log("Song " + fileCounter + " is '" + title + "'");

            var folder = Path.GetDirectoryName(file) + "\\Karaoke\\" + title.Replace("?", "") + "\\";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            var wav = folder + title.Replace("?", "") + ".wav";
            var mp3 = folder + title.Replace("?", "") + ".mp3";
            var cdg = folder + title.Replace("?", "") + ".cdg";
            var toml = folder + title.Replace("?", "") + ".toml";
            var mp4 = folder + title.Replace("?", "") + "-" + (do4KResolution ? "4K" : "1080P") + frameRate + ".mp4";
            
            string selectedFont;
            if (cboFont.InvokeRequired)
            {
                selectedFont = (string)cboFont.Invoke(new Func<string>(() => cboFont.Text));
            }
            else
            {
                selectedFont = cboFont.Text;
            }
            ActiveFontName = selectedFont.Split('|')[0].Trim();

            Log("Active Font Name = " + ActiveFontName);
            if (!moggSplitter.DownmixMogg(file, wav, MoggSplitter.MoggSplitFormat.WAV, vocalOption))
            {
                message = "Failed to extract and downmix the audio, stopping";
                Log(message);
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            Log("Extracted and downmixed audio successfully");
                          
            var xPackage = new STFSPackage(file);
            if (!xPackage.ParseSuccess)
            {
                message = "Could not parse that file, stopping";
                Log(message);
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var xFile = xPackage.GetFile("songs/" + Parser.Songs[0].InternalName + "/" + Parser.Songs[0].InternalName + ".mid");
            if (xFile == null)
            {
                message = "Could not find MIDI file, stopping";
                Log(message);
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                xPackage.CloseIO();
                return;
            }
            var midi = Path.GetTempPath() + Parser.Songs[0].InternalName + ".mid";
            Tools.DeleteFile(midi);
            if (!xFile.ExtractToFile(midi))
            {
                message = "Could not extract MIDI file, stopping";
                Log(message);
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                xPackage.CloseIO();
                return;
            }
            if (displayAlbumArt)
            {
                xFile = xPackage.GetFile("songs/" + Parser.Songs[0].InternalName + "/gen/" + Parser.Songs[0].InternalName + "_keep.png_xbox");
                if (xFile == null)
                {
                    message = "No album art found in that file";
                    Log(message);
                    albumArtX = new byte[0];
                }
                else
                {
                    albumArtX = xFile.Extract();
                    message = "Extracted album art successfully";
                    Log(message);
                }
            }
            xPackage.CloseIO();
                        
            var MIDIFile = Tools.NemoLoadMIDI(midi);
            if (MIDIFile == null)
            {
                message = "Unable to load MIDI file '" + Path.GetFileName(midi) + "', stopping";
                Log(message);
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }                

            TicksPerQuarter = MIDIFile.DeltaTicksPerQuarterNote;
            Log("Ticks per quarter: " + TicksPerQuarter);
            BuildTempoList(MIDIFile.Events);
            Log("Found " + TempoEvents.Count() + " tempo events");
            BuildTimeSignatureList(MIDIFile.Events);
            Log("Found " + TimeSignatures.Count() + " time signatures");

            for (var i = 0; i < MIDIFile.Events.Tracks; i++)
            {
                var trackname = MIDIFile.Events[i][0].ToString();
                if (trackname.Contains("PART VOCALS"))
                {
                    GetPhraseMarkers(MIDIFile.Events[i], 0);
                    AnalyzeVocals(MIDIFile.Events[i], 0);
                    Log("Found " + VocalPhrases.Count() + " phrases in PART VOCALS chart");
                    Log("Found " + VocalLyrics.Count() + " lyrics in PART VOCALS chart");
                    foreach (var phrase in VocalPhrases)
                    {
                        var matchingLyrics = VocalLyrics
                            .Where(lyr => lyr.Start >= phrase.PhraseStart && lyr.Start < phrase.PhraseEnd)
                            .OrderBy(lyr => lyr.Start).Select(lyr => lyr.Lyric).ToList();
                        phrase.PhraseText = string.Join(" ", matchingLyrics);
                    }
                }
                else if (trackname.Contains("HARM1"))
                {
                    GetPhraseMarkers(MIDIFile.Events[i], 1);
                    AnalyzeVocals(MIDIFile.Events[i], 1);
                    Log("Found " + Harm1Phrases.Count() + " phrases in HARM1 chart");
                    Log("Found " + Harm1Lyrics.Count() + " lyrics in HARM1 chart");
                    foreach (var phrase in Harm1Phrases)
                    {
                        var matchingLyrics = Harm1Lyrics
                            .Where(lyr => lyr.Start >= phrase.PhraseStart && lyr.Start < phrase.PhraseEnd)
                            .OrderBy(lyr => lyr.Start).Select(lyr => lyr.Lyric).ToList();
                        phrase.PhraseText = string.Join(" ", matchingLyrics);
                    }
                }
                else if (trackname.Contains("HARM2"))
                {
                    GetPhraseMarkers(MIDIFile.Events[i], 2);
                    AnalyzeVocals(MIDIFile.Events[i], 2);
                    Log("Found " + Harm2Phrases.Count() + " phrases in HARM2 chart");
                    Log("Found " + Harm2Lyrics.Count() + " lyrics in HARM2 chart");
                    foreach (var phrase in Harm2Phrases)
                    {
                        var matchingLyrics = Harm2Lyrics
                            .Where(lyr => lyr.Start >= phrase.PhraseStart && lyr.Start < phrase.PhraseEnd)
                            .OrderBy(lyr => lyr.Start).Select(lyr => lyr.Lyric).ToList();
                        phrase.PhraseText = string.Join(" ", matchingLyrics);
                    }
                }
                else if (trackname.Contains("HARM3"))
                {                    
                    AnalyzeVocals(MIDIFile.Events[i], 3);
                    Log("Found " + Harm3Lyrics.Count() + " lyrics in HARM3 chart");
                    foreach (var phrase in Harm3Phrases)
                    {
                        var matchingLyrics = Harm3Lyrics
                            .Where(lyr => lyr.Start >= phrase.PhraseStart && lyr.Start < phrase.PhraseEnd)
                            .OrderBy(lyr => lyr.Start).Select(lyr => lyr.Lyric).ToList();
                        phrase.PhraseText = string.Join(" ", matchingLyrics);
                    }
                }
            }

            foreach (var lyric in VocalLyrics)
            {
                foreach (var note in VocalNotes)
                {
                    if (note.Ticks <= lyric.Ticks && note.Ticks >= lyric.Ticks)
                    {
                        lyric.End = note.NoteEnd;
                        break;
                    }
                }
            }
            
            if (Parser.Songs[0].VocalParts > 1)
            {
                foreach (var lyric in Harm1Lyrics)
                {
                    foreach (var note in Harm1Notes)
                    {
                        if (note.Ticks <= lyric.Ticks && note.Ticks >= lyric.Ticks)
                        {
                            lyric.End = note.NoteEnd;
                            break;
                        }
                    }
                }               

                foreach (var lyric in Harm2Lyrics)
                {
                    foreach (var note in Harm2Notes)
                    {
                        if (note.Ticks <= lyric.Ticks && note.Ticks >= lyric.Ticks)
                        {
                            lyric.End = note.NoteEnd;
                            break;
                        }
                    }
                }                

                foreach (var lyric in Harm3Lyrics)
                {
                    foreach (var note in Harm3Notes)
                    {
                        if (note.Ticks <= lyric.Ticks && note.Ticks >= lyric.Ticks)
                        {
                            lyric.End = note.NoteEnd;
                            break;
                        }
                    }
                }                
            }

            try
            {
                try
                {
                    Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
                }
                catch { }
                int stream = Bass.BASS_StreamCreateFile(wav, 0L, 0L, BASSFlag.BASS_STREAM_DECODE);
                songDuration = Bass.BASS_ChannelBytes2Seconds(stream, Bass.BASS_ChannelGetLength(stream));
                Bass.BASS_StreamFree(stream);
                totalFrames = (int)Math.Ceiling(songDuration * frameRate); //detected audio length * {frameRate} fps
                Log("Based on audio file, song duration is " + (int)(songDuration + 0.5) + " seconds");                
            }
            catch
            {
                message = "Can't detect audio length, can't render video!";
                Log(message);
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (doCDG)
            {
                WriteTOML(toml, wav, cdg);
                CreateCDG(toml);
            }
            var art = folder + "art.jpg";
            if (doMP4)
            {
                var x = folder + "art.png_xbox";                
                if (albumArtX != null && albumArtX.Length > 0 && displayAlbumArt)
                {
                    File.WriteAllBytes(x, albumArtX);
                    if (!Tools.ConvertRBImage(x, art, "jpg", true))
                    {
                        Log("Failed to convert album art or album art not found");
                    }
                    Log("Converted album art successfully");
                }
                try
                {
                    DoModernKaraoke(folder, art, wav, mp4);
                }
                catch (Exception ex)
                {
                    message = "Error creating MP4 karaoke file:\n\n" + ex.Message.ToString() + "\n\n" + ex.StackTrace.ToString();
                    Log(message);
                    MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if (!doKeepWAV)
            {
                Tools.DeleteFile(wav);
            }
            if (!doKeepTOML)
            {
                Tools.DeleteFile(toml);
            }
            Tools.DeleteFile(art);
            if ((File.Exists(mp3) && File.Exists(cdg) && doCDG) || (File.Exists(mp4) && doMP4))
            {
                successCounter++;
            }
            else
            {
                failureCounter++;
                failedSongs.Add(Parser.Songs[0].Artist + " - " + Parser.Songs[0].Name);
            }
        }
        
        private string EscapeTomlString(string input)
        {
            if (input.Contains('"'))
                return "\"\"\"" + input + "\"\"\""; // wrap in triple quotes
            return "\"" + input + "\"";
        }

        private void Log(string message)
        {
            var messages = message.Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (lstLog.InvokeRequired)
            {
                foreach (var line in messages)
                {
                    lstLog.Invoke(new MethodInvoker(() => lstLog.Items.Add(line)));
                    lstLog.Invoke(new MethodInvoker(() => lstLog.SelectedIndex = lstLog.Items.Count - 1));
                }
            }
            else
            {
                foreach (var line in messages)
                {
                    lstLog.Items.Add(line);
                    lstLog.SelectedIndex = lstLog.Items.Count - 1;
                }
            }
        }

        public void InsertLoadingBars(ref List<TimedLyricLine> lines, List<TimedLyricLine> lyricsVocals, List<TimedLyricLine> lyricsHarm2, int vocal_part, int vocal_parts)
        {
            const int paddingCs = 100; // 1 second            
            var loadingLines = new List<(int InsertIndex, List<TimedLyricLine> Lines)>();

            // Gather all actual lyric timings
            var lyricTimings = lyricsVocals.Concat(lyricsHarm2).SelectMany(line => line.StartTimes
            .Zip(line.EndTimes, (start, end) => new { Start = start, End = end })).OrderBy(t => t.Start).ToList();

            if (lyricTimings.Count == 0)
                return; // no sung content at all
                        
            // Handle gaps between sung syllables
            for (int i = 0; i < lyricTimings.Count() - 1; i++)
            {
                int endCurrent = lyricTimings[i].End;
                int startNext = lyricTimings[i + 1].Start;
                int gap = startNext - endCurrent;

                if (gap >= timeGap * 0.1)
                {
                    int start = endCurrent + paddingCs;
                    int end = startNext - paddingCs;

                    // Find insertion index in lines
                    int insertIndex = lines.FindIndex(l =>
                        l.StartTimes.Count > 0 && l.StartTimes[0] > start);

                    if (insertIndex == -1)
                        insertIndex = lines.Count;

                    loadingLines.Add((insertIndex, BuildLoadingBlock(start, end, vocal_part, vocal_parts)));
                }
            }

            int lastEnd = lyricTimings.Last().End;
            int trackEnd = (int)(songDuration * 100);
            int outroEnd = trackEnd - (int)(timeGap * 0.1);
            var outroGap = outroEnd - lastEnd;

            if (outroGap >= (timeGap * 0.1))
            {
                int start = lastEnd + paddingCs;
                int end = outroEnd;

                int insertIndex = lines.Count; // append at the end
                loadingLines.Add((insertIndex, BuildLoadingBlock(start, end, vocal_part, vocal_parts)));
            }

            // Insert all loading bar blocks in reverse to preserve indices
            foreach (var (insertIndex, barLines) in loadingLines.OrderByDescending(l => l.InsertIndex))
            {
                lines.InsertRange(insertIndex, barLines);
            }
        }
               
        private static List<TimedLyricLine> BuildLoadingBlock(int start, int end, int vocal_part, int vocal_parts)
        {
            var lines = new List<TimedLyricLine>();                          

            if (vocal_parts == 1 || (vocal_parts > 1 && vocal_part == 2))
            {
                // Loading bar line with timing
                lines.Add(new TimedLyricLine
                {
                    Syllables = new List<string> { loadingBar, "/" },
                    StartTimes = new List<int> { start, end },
                    EndTimes = new List<int> { -1, -1 }, //this is just a placeholder
                    FormattedLine = $"{loadingBar}/"
                });
            }
            
            return lines;
        }

        private void WriteTOML(string toml, string wav, string cdg)
        {
            Log("Writing TOML file '" + toml + "'");
            Log("Active Font: " + ActiveFont);
            var vocal_parts = 1;
            if (doHarm2 && Harm2Lyrics.Any())
            {
                vocal_parts = 2;
            }
            else if (doHarm3)
            {
                if (Harm3Lyrics.Any())
                {
                    vocal_parts = 3;
                }
                else if (Harm2Lyrics.Any())
                {
                    vocal_parts = 2; //fallback to 2 part harmonies when the song doesn't have harm3
                }
            }
            int harm1Top = vocal_parts == 1 ? 5 : (Harm3Lyrics.Any() ? 1 : 2);
            int harm1Parts = vocal_parts == 1 ? 4 : (vocal_parts == 3 ? 2 : 3);
            int harm2Top = vocal_parts == 2 ? 11 : 7;
            int harm2Parts = vocal_parts == 2 ? 3 : 2;
            const int harm3Top = 13;
            const int harm3Parts = 2;
            var fontPath = Application.StartupPath + "\\bin\\fonts\\" + ActiveFont;
            var bpm = (int)(AverageBPM() + 0.5);
            Log("Tempo: " + bpm + " BPM");

            var sw = new StreamWriter(toml, false);
            sw.WriteLine("title = " + EscapeTomlString("\"" + Parser.Songs[0].Name.Replace("feat.", "ft.").Replace("featuring", "ft.") + "\""));
            var artist = $"artist = '''{Parser.Songs[0].Artist.Replace("feat.", "ft.").Replace("featuring", "ft.")}";
            if (displayTempo)
            {
                artist = artist + $"\n\nTempo: {bpm} BPM'''";
            }
            else
            {
                artist = artist + "'''";
            }
            sw.WriteLine(artist);
            sw.WriteLine("file = \"" + wav.Replace("\\", "\\\\") + "\"");
            sw.WriteLine("outname = \"" + cdg.Replace("\\", "\\\\") + "\"");
            sw.WriteLine("");
            sw.WriteLine("clear_mode = \"" + cdgMode + "\"");
            sw.WriteLine("");
            sw.WriteLine("background = \"" + backgroundColor + "\"");
            sw.WriteLine("border = \"" + backgroundColor + "\""); //match the background for now
            sw.WriteLine("highlight_bandwidth = 1");
            sw.WriteLine("draw_bandwidth = 1");
            sw.WriteLine("stroke_width = " + (enableCDGStroke ? "1" : "0"));
            sw.WriteLine("");
            sw.WriteLine("font = '" + ActiveFont + "'");
            sw.WriteLine("font_size = 18"); //hardcoded
            sw.WriteLine("");                                       
            sw.WriteLine("[[singers]]"); //always must have atleast one, this is lead/harm1
            sw.WriteLine("active_fill = \"" + highlightColor1 + "\"");
            sw.WriteLine("active_stroke = \"" + (enableCDGStroke ? strokeColor : highlightColor1) + "\""); 
            sw.WriteLine("inactive_fill = \"" + textColor1 + "\""); 
            sw.WriteLine("inactive_stroke = \"" + (enableCDGStroke ? strokeColor : textColor1) + "\"");
            if ((doHarm2 || doHarm3) && Harm2Lyrics.Any())
            {
                sw.WriteLine("");
                sw.WriteLine("[[singers]]"); //harm2
                sw.WriteLine("active_fill = \"" + highlightColor2 + "\"");
                sw.WriteLine("active_stroke = \"" + (enableCDGStroke ? strokeColor : highlightColor2) + "\"");
                sw.WriteLine("inactive_fill = \"" + textColor2 + "\"");
                sw.WriteLine("inactive_stroke = \"" + (enableCDGStroke ? strokeColor : textColor2) + "\"");
            }
            if (doHarm3 && Harm3Lyrics.Any())
            {
                sw.WriteLine("");
                sw.WriteLine("[[singers]]"); //harm3
                sw.WriteLine("active_fill = \"" + highlightColor3 + "\"");
                sw.WriteLine("active_stroke = \"" + (enableCDGStroke ? strokeColor : highlightColor3) + "\"");
                sw.WriteLine("inactive_fill = \"" + textColor3 + "\""); 
                sw.WriteLine("inactive_stroke = \"" + (enableCDGStroke ? strokeColor : textColor3) + "\"");
            }
            sw.WriteLine("");
            sw.WriteLine("[[lyrics]]"); //must always have one, this is lead/harm1
            sw.WriteLine("singer = 1");
            sw.WriteLine("sync = [");
            var lyricsVocals = GetTimedLyricsForTOML(vocal_parts > 1 ? 1 : 0);
            if (vocal_parts == 1 && doShowLoadingBar)
            {
                InsertLoadingBars(ref lyricsVocals, lyricsVocals, new List<TimedLyricLine>(), 1, vocal_parts);
            }            
            foreach (var lyricLine in lyricsVocals)
            {
                for (int i = 0; i < lyricLine.Syllables.Count; i++)
                {
                    if (lyricLine.StartTimes.Count == 0 || lyricLine.StartTimes[i] == -1) continue;
                    sw.Write(lyricLine.StartTimes[i] + ",");
                    if (i == lyricLine.Syllables.Count - 1)
                    {
                        sw.Write("\n");//break it up per phrase
                    }
                }
            }
            sw.Write("\n");
            sw.WriteLine("]");
            sw.WriteLine("row = " + harm1Top);
            sw.WriteLine("line_tile_height = 2");
            sw.WriteLine("lines_per_page = " + harm1Parts);
            sw.WriteLine("text = '''");
            foreach (var line in lyricsVocals)
            {
                sw.WriteLine(line.FormattedLine.Replace("‿", "\u00A0"));
            }
            sw.WriteLine("'''");
            if ((doHarm2 || doHarm3) && Harm2Lyrics.Any())
            {
                sw.WriteLine("");
                sw.WriteLine("[[lyrics]]");
                sw.WriteLine("singer = 2");
                sw.WriteLine("sync = [");
                var lyricsHarm2 = GetTimedLyricsForTOML(2);
                if (vocal_parts > 1 && doShowLoadingBar)
                {
                    InsertLoadingBars(ref lyricsHarm2, lyricsVocals, lyricsHarm2, 2, vocal_parts);
                }
                foreach (var lyricLine in lyricsHarm2)
                {
                    for (int i = 0; i < lyricLine.Syllables.Count; i++)
                    {
                        if (lyricLine.StartTimes.Count == 0 || lyricLine.StartTimes[i] == -1) continue;
                        sw.Write(lyricLine.StartTimes[i] + ",");
                        if (i == lyricLine.Syllables.Count - 1)
                        {
                            sw.Write("\n");//break it up per phrase
                        }
                    }
                }
                sw.WriteLine("]");
                sw.WriteLine("row = " + harm2Top);
                sw.WriteLine("line_tile_height = 2");
                sw.WriteLine("lines_per_page = " + harm2Parts);
                sw.WriteLine("text = '''");
                foreach (var line in lyricsHarm2)
                {
                    sw.WriteLine(line.FormattedLine.Replace("‿", "\u00A0"));
                }
                sw.WriteLine("'''");                
            }
            if (doHarm3 && Harm3Lyrics.Any())
            {
                sw.WriteLine("");
                sw.WriteLine("[[lyrics]]");
                sw.WriteLine("singer = 3");
                sw.WriteLine("sync = [");
                var lyricsHarm3 = GetTimedLyricsForTOML(3);
                if (vocal_parts > 1 && doShowLoadingBar)
                {
                    InsertLoadingBars(ref lyricsHarm3, lyricsVocals, lyricsHarm3, 3, vocal_parts);
                }
                foreach (var lyricLine in lyricsHarm3)
                {
                    for (int i = 0; i < lyricLine.Syllables.Count; i++)
                    {
                        if (lyricLine.StartTimes.Count == 0 || lyricLine.StartTimes[i] == -1) continue;
                        sw.Write(lyricLine.StartTimes[i] + ",");
                        if (i == lyricLine.Syllables.Count - 1)
                        {
                            sw.Write("\n");//break it up per phrase
                        }
                    }
                }
                sw.WriteLine("]");
                sw.WriteLine("row = " + harm3Top);
                sw.WriteLine("line_tile_height = 2");
                sw.WriteLine("lines_per_page = " + harm3Parts);
                sw.WriteLine("text = '''");
                foreach (var line in lyricsHarm3)
                {
                    sw.WriteLine(line.FormattedLine.Replace("‿", "\u00A0"));
                }
                sw.WriteLine("'''");
            }
            sw.Dispose();            
        }      

        private void CreateCDG(string toml)
        {
            Log("Sending TOML file to CDGMaker");
            var folderPath = Application.StartupPath + "\\bin\\";
            var exePath = Path.Combine(folderPath, "cdgmaker.exe");
            if (!File.Exists(exePath))
            {
                var message = "cdgmaker.exe is missing and I can't continue without it";
                Log(message);
                MessageBox.Show(message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var arg = "-v \"" + toml + "\"";
            var app = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = arg,
                WorkingDirectory = folderPath,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            var process = Process.Start(app);
            cdgOutput += $"Begin log for song '{Parser.Songs[0].Artist} - {Parser.Songs[0].Name}'{process.StandardError.ReadToEnd()}" +
                $"End log for song '{Parser.Songs[0].Artist} - {Parser.Songs[0].Name}'\n\n";
            process.WaitForExit();
            process.Dispose();
        }
                       
        private List<TimedLyricLine> GetTimedLyricsForTOML(int part)
        {
            var result = new List<TimedLyricLine>();
            int maxLineLength = 24;

            var allLyrics = (part == 0 ? VocalLyrics :
                            (part == 1 ? Harm1Lyrics :
                            (part == 2 ? Harm2Lyrics : Harm3Lyrics)));

            foreach (var phrase in VocalPhrases)
            {
                var phraseLyrics = allLyrics
                    .Where(l => l.Start >= phrase.PhraseStart && l.Start <= phrase.PhraseEnd).ToList();

                if (phraseLyrics.Count == 0)
                {
                    result.Add(new TimedLyricLine { Syllables = new List<string> { "~" }, FormattedLine = "~" });
                    continue;
                }

                var currentLine = new TimedLyricLine();
                int visibleCharCount = 0;
                var formattedLineBuilder = new List<string>();

                int i = 0;
                while (i < phraseLyrics.Count)
                {
                    var lyric = phraseLyrics[i];
                    string raw = lyric.Lyric.Trim().Replace("^", "").Replace("$", "").Replace("#", "");
                    
                    // Handle hold syllables
                    if (raw == "+")
                    {
                        int holdStart = (int)((lyric.Start * 0.1) - 0.5);
                        int holdEnd = (int)((lyric.End * 0.1) + 0.5);

                        currentLine.Syllables.Add("/");
                        currentLine.StartTimes.Add(holdStart);
                        currentLine.EndTimes.Add(holdEnd);
                        formattedLineBuilder.Add("/");
                        i++;
                        continue;
                    }

                    // Rebuild full word from syllables
                    var syllables = new List<(string Text, int Start, int End)>();
                    bool endsWithDash = EndsWithDash(raw);
                    string clean = CleanSyllable(raw);

                    if (!string.IsNullOrWhiteSpace(clean))                        
                        syllables.Add((clean, (int)((lyric.Start * 0.1) - 0.5), (int)((lyric.End * 0.1) + 0.5)));

                    int j = i + 1;
                    while (endsWithDash && j < phraseLyrics.Count)
                    {
                        var next = phraseLyrics[j];
                        string nextRaw = next.Lyric.Trim();
                        endsWithDash = EndsWithDash(nextRaw) || nextRaw == "+";
                        string nextClean = CleanSyllable(nextRaw);
                        if (!string.IsNullOrWhiteSpace(nextClean))                            
                            syllables.Add((nextClean, (int)((next.Start * 0.1) - 0.5), (int)((next.End * 0.1) + 0.5)));

                        j++;
                    }

                    int visibleLen = syllables.Sum(s => s.Text.Length);

                    // Wrap if too long
                    if (visibleCharCount + (visibleCharCount > 0 ? 1 : 0) + visibleLen > maxLineLength)
                    {
                        if (currentLine.Syllables.Count > 0)
                        {
                            currentLine.FormattedLine = string.Join("", formattedLineBuilder);
                            result.Add(currentLine);
                        }
                        currentLine = new TimedLyricLine();
                        formattedLineBuilder.Clear();
                        visibleCharCount = 0;
                    }

                    // Add space before word
                    if (visibleCharCount > 0)
                    {
                        currentLine.Syllables.Add(" ");
                        currentLine.StartTimes.Add(-1);
                        currentLine.EndTimes.Add(-1);
                        formattedLineBuilder.Add(" ");
                        visibleCharCount++;
                    }

                    //10 centisecond for spaces
                    for (int syl = 0; syl < currentLine.Syllables.Count; syl++)
                    {
                        if (currentLine.Syllables[syl] == " " && syl > 0)
                        {
                            currentLine.StartTimes[syl] = currentLine.StartTimes[syl - 1] + 5;
                            currentLine.EndTimes[syl] = currentLine.StartTimes[syl - 1] + 15;
                        }
                    }

                    for (int s = 0; s < syllables.Count; s++)
                    {                        
                        var syll = syllables[s];                                               

                        int highlightStart = syll.Start;
                        int highlightEnd = syll.End;

                        currentLine.Syllables.Add(syll.Text);
                        currentLine.StartTimes.Add(highlightStart);
                        currentLine.EndTimes.Add(highlightEnd);
                        formattedLineBuilder.Add(syll.Text);
                        visibleCharCount += syll.Text.Length;

                        currentLine.Syllables.Add("/");
                        currentLine.StartTimes.Add(highlightEnd);
                        currentLine.EndTimes.Add(highlightEnd);

                        if (s < syllables.Count - 1)
                            formattedLineBuilder.Add("//");
                        else
                            formattedLineBuilder.Add("/");
                    }                    

                    i = j;
                }

                if (currentLine.Syllables.Count > 0)
                {
                    currentLine.FormattedLine = string.Join("", formattedLineBuilder);
                    result.Add(currentLine);
                }
            }

            return result;
        }

        public class TimedLyricLine
        {
            public List<string> Syllables { get; set; } = new List<string>();
            public List<int> StartTimes { get; set; } = new List<int>();
            public List<int> EndTimes { get; set; } = new List<int>();

            public string FormattedLine { get; set; } = "";
        }

        private bool EndsWithDash(string s)
        {
            return s.Replace("#", "").Replace("^", "")
                    .TrimEnd('.', ',', '!', '?', '…', ';', ':')
                    .Trim()
                    .EndsWith("-");
        }

        private string CleanSyllable(string s)
        {
            return CleanString(s)
                .Replace("-", "")
                .Replace("=", "-")
                .Replace("#", "")
                .Replace("^", "")
                .Replace("$", "")
                .Replace("+", "")//.Replace("+", "^")
                .Replace("§", "‿")
                .Trim();
        }

        private double GetRealtime(long absdelta)
        {
            //code by raynebc
            var BPM = 120.0;   //As per the MIDI specification, until a tempo change is reached, 120BPM is assumed
            var reldelta = absdelta;   //The number of delta ticks between the delta time being converted and the tempo change immediately at or before it
            var time = 0.0;   //The real time position of the tempo change immediately at or before the delta time being converted
            foreach (var tempo in TempoEvents.Where(tempo => tempo.AbsoluteTime <= absdelta))
            {
                BPM = tempo.BPM;
                time = tempo.RealTime;
                reldelta = absdelta - tempo.AbsoluteTime;
            }
            time += (double)reldelta / TicksPerQuarter * (60000.0 / BPM);
            return time;
        }

        private void AnalyzeVocals(IEnumerable<MidiEvent> track, int part)
        {
            foreach (var notes in track)
            {
                switch (notes.CommandCode)
                {
                    case MidiCommandCode.MetaEvent:
                        var vocal_event = (MetaEvent)notes;
                        if ((vocal_event.MetaEventType == MetaEventType.Lyric || vocal_event.MetaEventType == MetaEventType.TextEvent) &&
                            !vocal_event.ToString().Contains("["))
                        {
                            var lyric = GetCleanMIDILyric(vocal_event.ToString());
                            if (string.IsNullOrWhiteSpace(lyric)) continue;                            
                            var k = new KaraokeLyric()
                            {
                                Lyric = lyric,
                                Ticks = vocal_event.AbsoluteTime,
                                Start = GetRealtime(vocal_event.AbsoluteTime)
                            };
                            switch (part)
                            {
                                case 1:
                                    Harm1Lyrics.Add(k);
                                    break;
                                case 2:
                                    Harm2Lyrics.Add(k);
                                    break;
                                case 3:
                                    Harm3Lyrics.Add(k);
                                    break;
                                default:
                                    VocalLyrics.Add(k);
                                    break;
                            }                            
                        }
                        break;
                    case MidiCommandCode.NoteOn:
                        var note = (NoteOnEvent)notes;
                        if (note.Velocity <= 0) continue;

                        if (note.NoteNumber <= 84 && note.NoteNumber >= 36)
                        {
                            var n = new MIDINote()
                            {
                                NoteNumber = note.NoteNumber,
                                NoteStart = GetRealtime(note.AbsoluteTime),
                                Ticks = note.AbsoluteTime,
                                NoteEnd = GetRealtime(note.AbsoluteTime + note.NoteLength)
                            };
                            switch (part)
                            {
                                case 1:
                                    Harm1Notes.Add(n);
                                    break;
                                case 2:
                                    Harm2Notes.Add(n);
                                    break;
                                case 3:
                                    Harm3Notes.Add(n);
                                    break;
                                default:
                                    VocalNotes.Add(n);
                                    break;
                            }
                        }
                        break;
                }
            }
        }

        private static string GetCleanMIDILyric(string raw_event)
        {
            try
            {
                raw_event = raw_event.Trim();
                int index;
                if (raw_event.Contains("Lyric"))
                {
                    index = raw_event.IndexOf("Lyric", StringComparison.Ordinal) + 5;
                }
                else if (raw_event.Contains("TextEvent"))
                {
                    index = raw_event.IndexOf("TextEvent", StringComparison.Ordinal) + 9;
                }
                else
                {
                    index = raw_event.LastIndexOf(" ", StringComparison.Ordinal);
                }
                if (index == -1) return "";

                var old_lyric = raw_event.Substring(index + 1, raw_event.Length - index - 1);
                return old_lyric;
            }
            catch (Exception)
            {
                return "";
            }
        }

        private void GetPhraseMarkers(IEnumerable<MidiEvent> track, int part)
        {
            var phrases = new List<LyricPhrase>();
            foreach (var notes in track)
            {
                switch (notes.CommandCode)
                {
                    case MidiCommandCode.NoteOn:
                        var note = (NoteOnEvent)notes;
                        if (note.Velocity <= 0) continue;
                        switch (note.NoteNumber)
                        {
                            case 106: //older style part vocals had both 106 and 105
                            case 105: //typical phrase marker
                                var time = GetRealtime(note.AbsoluteTime);
                                var end = GetRealtime(note.AbsoluteTime + note.NoteLength);
                                if (phrases.Any())
                                {
                                    var index = phrases.Count - 1;
                                    if (phrases[index].PhraseStart == time)
                                    {
                                        continue; //for old double-stacked markers
                                    }
                                }
                                var phrase = new LyricPhrase
                                {
                                    PhraseStart = time,
                                    PhraseEnd = end
                                };
                                phrases.Add(phrase); //new line
                                break;
                        }
                        break;
                }
            }
            switch (part)
            {
                case 0:
                default:
                    VocalPhrases = phrases;
                    break;
                case 1:
                    Harm1Phrases = phrases;
                    break;
                case 2:
                    Harm2Phrases = phrases;
                    Harm3Phrases = phrases.Select(p => new LyricPhrase
                    {
                        PhraseStart = p.PhraseStart,
                        PhraseEnd = p.PhraseEnd
                    }).ToList();
                    break;
            }            
        }                   

        private void cboBackground_SelectedIndexChanged(object sender, EventArgs e)
        {
            Color backColor;

            if (cboBackground.SelectedIndex < cboBackground.Items.Count - 1) //not the last "custom" option
            {
                backColor = GetColorFromIndex(cboBackground.SelectedIndex);                
            }
            else if (isLoading)
            {
                return;
            }
            else
            {
                colorPicker.ShowDialog();
                backColor = colorPicker.Color;
                customColor = ColorTranslator.ToHtml(backColor);
            }

            UpdateBackgroundColors(backColor);            
        }

        private void UpdateBackgroundColors(Color backColor)
        {
            picBackground1.BackColor = backColor;
            lblTextHighlight1.BackColor = Color.Transparent;// backColor;
            lblTextColor1.BackColor = Color.Transparent;// backColor;

            picBackground2.BackColor = backColor;
            lblTextHighlight2.BackColor = Color.Transparent;// backColor;
            lblTextColor2.BackColor = Color.Transparent;// backColor;

            picBackground3.BackColor = backColor;
            lblTextHighlight3.BackColor = Color.Transparent;// backColor;
            lblTextColor3.BackColor = Color.Transparent;// backColor;

            picPreview.BackColor = backColor;
        }

        private Color GetColorFromIndex(int index)
        {
            try
            {
                return ColorTranslator.FromHtml(karaokeColorHexes[index]);
            }
            catch
            {
                return Color.Black;
            }            
        }        

        private void cboFont_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFont();
        }

        private void UpdateFont()
        {
            if (!cboFont.Text.Contains(".ttf")) return;
            try
            {
                var fontFile = cboFont.Text.Split('|')[1].Trim();
                string fontPath = Path.Combine(Application.StartupPath, "bin", "fonts", fontFile);
                pfc = new PrivateFontCollection();
                pfc.AddFontFile(fontPath);               
                FontFamily ff = pfc.Families[0]; // assuming the TTF file only contains one family
                             
                var font = new Font(ff, 16f, FontStyle.Bold);
                lblTextColor1.Font = font;
                lblTextHighlight1.Font = font;
                lblTextColor2.Font = font;
                lblTextHighlight2.Font = font;
                lblTextColor3.Font = font;
                lblTextHighlight3.Font = font;
            }
            catch
            {
                MessageBox.Show("Error trying to change font, are you sure it's a valid selection?", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            cdgOutput = "";
            var log = Application.StartupPath + "\\bin\\karaoke.log.txt";
            Tools.DeleteFile(log);
            Log("Received " + inputFiles.Count() + " Rock Band song files ... beginning processing");
            foreach (var file in inputFiles)
            {
                if (cancelProcess) break;
                Log("");
                fileCounter++;
                Log("File " + fileCounter + " is '" + file + "'");
                SongTimerStart = DateTime.Now;
                Log("Song processing start time is " + SongTimerStart.ToString("hh:mm:ss tt"));
                ProcessFile(file);
                SongTimerEnd = DateTime.Now;
                Log("Song processing end time is " + SongTimerEnd.ToString("hh:mm:ss tt"));
                TimeSpan duration = SongTimerEnd - SongTimerStart;
                Log("Song processing duration: " + duration.ToString(@"hh\:mm\:ss"));
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            Log("");
            BatchTimerEnd = DateTime.Now;            
            var duration = BatchTimerEnd - BatchTimerStart;
            EnableDisable(true);
            inputFiles = new List<string>();            
            Log($"Processed {fileCounter} song file(s):");
            Log($"{successCounter} file(s) were converted successfully");
            if (failureCounter > 0)
            {
                Log($"{failureCounter} file(s) failed to convert");
                Log("The failed song file(s) are:");
                foreach (var song in failedSongs)
                {
                    Log(song);
                }
            }
            Log("Batch processing end time is " + BatchTimerEnd.ToString("hh:mm:ss tt"));
            Log("Batch processing duration: " + duration.ToString(@"hh\:mm\:ss"));
            fileCounter = 0;
            failureCounter = 0;
            successCounter = 0;
            failedSongs = new List<string>();
            cancelProcess = false;
        }

        private void LogDefaults()
        {
            lstLog.Items.Clear();
            Log("Welcome to " + Text);
            Log("Choose your options or leave the defaults (recommended)");
            Log("You can use a solid color background, a static image, or a video background");
            Log("You can use the provided static image and video backgrounds");
            Log("Or you can drag and drop any custom image or video that you want to use");
            Log("When ready, drag and drop one or multiple Rock Band song files here to get started");
        }

        private void CDGConverter_Shown(object sender, EventArgs e)
        {
            LoadConfig();
            LogDefaults();
            var folder = Application.StartupPath + "\\bin\\fonts\\";
            var fonts = Directory.GetFiles(folder, "*.ttf", SearchOption.TopDirectoryOnly);
            if (fonts.Any())
            {
                cboFont.Items.Clear();
                foreach (var font in fonts)
                {
                    pfc = new PrivateFontCollection();
                    pfc.AddFontFile(font);
                    FontFamily ff = pfc.Families[0];
                    cboFont.Items.Add(ff.Name + " | " + Path.GetFileName(font));
                }
                if (cboFont.Items.Count > 0)
                {
                    cboFont.SelectedIndex = 0;
                }
            }
            var cdgmaker = Application.StartupPath + "\\bin\\cdgmaker.exe";
            if (!File.Exists(cdgmaker))
            {
                var message = "cdgmaker.exe is required to render to CDG+MP3 format and it is missing from the \\bin folder!";
                MessageBox.Show(message, "Missing File", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Log(message);
                chkCDG.Enabled = false;
            }
            var ffmpeg = Application.StartupPath + "\\bin\\ffmpeg.exe";
            if (!File.Exists(ffmpeg))
            {
                var message = "ffmpeg.exe is required to render to MP4 format and it is missing from the \\bin folder!";
                MessageBox.Show(message, "Missing File", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Log(message);
                chkMP4.Enabled = false;
            }
            UpdateFont();
        }

        private void CDGConverter_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (picWorking.Visible)
            {
                MessageBox.Show("Please wait until the current process finishes", "Working...", MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
                e.Cancel = true;
                return;
            }
            SaveConfig();
        }

        private void cboTextColor1_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblTextColor1.ForeColor = GetColorFromIndex(cboTextColor1.SelectedIndex);
        }

        private void cboTextHighlight1_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblTextHighlight1.ForeColor = GetColorFromIndex(cboTextHighlight1.SelectedIndex);
        }

        private void cboTextColor2_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblTextColor2.ForeColor = GetColorFromIndex(cboTextColor2.SelectedIndex);
        }

        private void cboTextColor3_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblTextColor3.ForeColor = GetColorFromIndex(cboTextColor3.SelectedIndex);
        }

        private void cboTextHighlight2_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblTextHighlight2.ForeColor = GetColorFromIndex(cboTextHighlight2.SelectedIndex);
        }

        private void cboTextHighlight3_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblTextHighlight3.ForeColor = GetColorFromIndex(cboTextHighlight3.SelectedIndex);
        }

        private void lblFontQuestion_MouseClick(object sender, MouseEventArgs e)
        {
            const string message = "You can customize the font that the converter will use\n\nPut your preferred fonts in the \\bin\\fonts\\ folder and they will be displayed here\n\n"
                + "Only .ttf font files are accepted";
            MessageBox.Show(message, "Fonts", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }        

        public static (List<KaraokeLyric> line1, List<KaraokeLyric> line2)
        SplitSyllablesByPixelWidth(
            List<KaraokeLyric> phraseSyllables,
            Font font,
            Graphics g)
        {
            var line1 = new List<KaraokeLyric>();
            var line2 = new List<KaraokeLyric>();
            if (phraseSyllables == null || phraseSyllables.Count == 0) return (line1, line2);
            if (phraseSyllables.Count == 1) return (phraseSyllables, line2);

            // 1) Build "words" from syllables (merge hyphenated and sustained runs)
            var words = new List<(List<KaraokeLyric> syls, string text, int widthPx)>();

            int i = 0;
            while (i < phraseSyllables.Count)
            {
                var bucket = new List<KaraokeLyric>();
                bool keepMerging = true;

                while (i < phraseSyllables.Count && keepMerging)
                {
                    var s = phraseSyllables[i];
                    bucket.Add(s);

                    string lyric = (s.Lyric ?? "").Trim();
                    bool endsWithDash = lyric.Replace("#", "").Replace("^", "").Replace("$", "").EndsWith("-");
                    bool isSustain = lyric == "+";

                    bool nextIsSustain = (i + 1 < phraseSyllables.Count && phraseSyllables[i + 1].Lyric == "+");

                    // If current ends with "-", the word definitely continues.
                    // If current is "+", it belongs to the current word (sustain).
                    // If next is "+", we also keep merging into current word.
                    keepMerging = endsWithDash || isSustain || nextIsSustain;
                    i++;
                }

                // Visible text for the word (no + or -; replace tie with space)
                string wordText = string.Join("", bucket.Select(b =>
                    (b.Lyric ?? "").Replace("#", "").Replace("^", "").Replace("$", "").Replace("+", "").Replace("-", "").Replace("‿", " ")))
                    .Trim();

                if (wordText.Length == 0)
                {
                    // If the “word” is only sustains, skip it (still contributes to timing, not to drawing text)
                    continue;
                }

                // Measure with the SAME API we render with
                int w = TextRenderer.MeasureText(g, wordText, font).Width;

                words.Add((bucket, wordText, w));
            }

            if (words.Count == 0) return (line1, line2);
            if (words.Count == 1) { line1.AddRange(words[0].syls); return (line1, line2); }

            // 2) Compute total pixel width incl. spaces
            // Approximate a single space width with this font
            int spaceW = TextRenderer.MeasureText(g, " ", font).Width;
            int totalPx = 0;
            for (int k = 0; k < words.Count; k++)
            {
                totalPx += words[k].widthPx;
                if (k > 0) totalPx += spaceW;
            }
            int target = totalPx / 2;

            // 3) Greedy pack into line1 until we would exceed target
            int accum = 0;
            int breakIndex = words.Count; // default all on line1 if short

            for (int k = 0; k < words.Count; k++)
            {
                int add = words[k].widthPx + (k > 0 ? spaceW : 0);
                // If adding this word would push us *far* past target and we already
                // have at least one word, break before it.
                if (k > 0 && accum + add > target)
                {
                    // Optional: consider which side is visually closer to target
                    int over = (accum + add) - target;
                    int under = target - accum;
                    if (over >= under) breakIndex = k;
                    else breakIndex = k + 1;
                    break;
                }
                accum += add;
            }

            // 4) Emit syllables to lines
            for (int k = 0; k < words.Count; k++)
            {
                if (k < breakIndex) line1.AddRange(words[k].syls);
                else line2.AddRange(words[k].syls);
            }

            return (line1, line2);
        }       

        private string GetSongKey()
        {
            var key = "";
            switch (Parser.Songs[0].TonicNote)
            {
                case 0:
                    key = "C";
                    break;
                case 1:
                    key = "D♭";
                    break;
                case 2:
                    key = "D";
                    break;
                case 3:
                    key = "E♭";
                    break;
                case 4:
                    key = "E";
                    break;
                case 5:
                    key = "F";
                    break;
                case 6:
                    key = "G♭";
                    break;
                case 7:
                    key = "G";
                    break;
                case 8:
                    key = "A♭";
                    break;
                case 9:
                    key = "A";
                    break;
                case 10:
                    key = "B♭";
                    break;
                case 11:
                    key = "B";
                    break;
                default:
                    key = "";
                    break;
            }
            if (key == "") return "";
            var tonality = "";
            switch (Parser.Songs[0].Tonality)
            {
                case 0:
                    tonality = " Major";
                    break;
                case 1:
                    tonality = " Minor";
                    break;
                default:
                    tonality = "";
                    break;
            }

            return "🎵 Key: " + key + tonality;
        }

        private void DrawCenteredLine(
            Graphics g,
            string text,
            int resolutionX,
            int y,
            float maxFontSize,
            int offset = 0,
            int shadowOffsetX = 1,
            int shadowOffsetY = 1,
            int shadowBlur = 3,
            float shadowOpacity = 0.20f
            )
        {
            using (var baseFont = new Font(ActiveFontName, 16f))
            {
                float scaledFontSize = GetScaledFontSize(g, text, baseFont, maxFontSize, resolutionX - offset);

                using (var font = new Font(ActiveFontName, scaledFontSize))
                {
                    ApplyTextRenderingSettings(g);
                    
                    // measure for centering
                    var size = TextRenderer.MeasureText(g, text, font);
                    int x = (resolutionX + offset - size.Width) / 2;

                    Color textColor = ColorTranslator.FromHtml(textColor1);                                      
                    
                    if (enableTitleShadows)
                    {
                        // Create an offscreen bitmap for blur
                        using (Bitmap shadowBmp = new Bitmap(size.Width + shadowBlur * 2, size.Height + shadowBlur * 2))
                        using (Graphics shadowG = Graphics.FromImage(shadowBmp))
                        {
                            shadowG.Clear(Color.Transparent);
                            shadowG.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                            // Shadow color (same shape as text)
                            using (Brush shadowBrush = new SolidBrush(Color.FromArgb((int)(255 * shadowOpacity), 0, 0, 0)))
                            {
                                shadowG.DrawString(text, font, shadowBrush, shadowBlur, shadowBlur);
                            }

                            // Apply a simple blur approximation by redrawing the bitmap slightly offset
                            for (int dx = -shadowBlur; dx <= shadowBlur; dx++)
                            {
                                for (int dy = -shadowBlur; dy <= shadowBlur; dy++)
                                {
                                    if (dx == 0 && dy == 0) continue;
                                    float weight = 1f - (float)Math.Sqrt(dx * dx + dy * dy) / shadowBlur;
                                    if (weight <= 0) continue;

                                    using (var tempBrush = new TextureBrush(shadowBmp))
                                    {
                                        ColorMatrix cm = new ColorMatrix
                                        {
                                            Matrix33 = weight * 0.2f // blur transparency falloff
                                        };
                                        using (ImageAttributes ia = new ImageAttributes())
                                        {
                                            ia.SetColorMatrix(cm, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                                            g.DrawImage(shadowBmp,
                                                new Rectangle(x + shadowOffsetX + dx, y + shadowOffsetY + dy,
                                                              shadowBmp.Width, shadowBmp.Height),
                                                0, 0, shadowBmp.Width, shadowBmp.Height,
                                                GraphicsUnit.Pixel, ia);
                                        }
                                    }
                                }
                            }
                        }

                        using (var brush = new SolidBrush(textColor))
                        {
                            g.DrawString(text, font, brush, x, y);
                        }
                    }
                    if (enableMP4Stroke)
                    {                        
                        Color strokeCol = ColorTranslator.FromHtml(strokeColor);

                        DrawTextWithStroke(
                            g,
                            text,
                            font,
                            new Point(x, y),
                            textColor,
                            strokeCol,
                            2
                        );
                    }
                    else
                    {
                        using (var brush = new SolidBrush(textColor))
                        {
                            g.DrawString(text, font, brush, x, y);
                        }
                    }                                      
                }
            }
        }
                
        public double AverageBPM()
        {
            var total_bpm = 0.0;
            var last = 0.0;
            var bpm = 120.0;
            double difference;
            var LengthSeconds = songDuration * 1000;
            if (LengthSeconds <= 0.0)
            {
                var count = TempoEvents.Sum(tempo => tempo.BPM);
                return Math.Round(count / TempoEvents.Count, 2);
            }
            foreach (var tempo in TempoEvents)
            {
                var current = GetRealtime(tempo.AbsoluteTime);
                difference = current - last;
                last = GetRealtime(tempo.AbsoluteTime);
                if (difference <= 0.0)
                {
                    bpm = tempo.BPM;
                    continue;
                }
                total_bpm += bpm * (difference / LengthSeconds);
                bpm = tempo.BPM;
            }
            difference = LengthSeconds - last;
            total_bpm += bpm * (difference / LengthSeconds);
            if (total_bpm == 0)
            {
                total_bpm = bpm;
            }
            return Math.Round(total_bpm, 2);
        }

        void DrawAnimatedNotes(Graphics graphics, int noteCounter, int spawnFrequency, int screenWidth, int screenHeight)
        {
            string[] musicNotes = new[] { "🎵", "🎶", "♫", "♬" };
            int multiplier = do4KResolution ? 2 : 1;
            Color[] colors = new[]
            {
            Color.FromArgb(255, 255, 105, 97),   // pastel red
            Color.FromArgb(255, 97, 168, 255),   // light blue
            Color.FromArgb(255, 144, 238, 144),  // light green
            Color.FromArgb(255, 255, 222, 89),   // light yellow
            Color.FromArgb(255, 255, 179, 255),  // soft pink
            Color.FromArgb(255, 189, 255, 255),  // soft cyan
            Color.FromArgb(255, 255, 255, 255),  // white fallback
    };

            // Spawn new notes at interval
            if (noteCounter % spawnFrequency == 0)
            {
                var fontFamily = new FontFamily("Segoe UI Emoji");
                for (int i = 0; i < 5; i++) // fewer per spawn for smoother effect
                {
                    string note = musicNotes[rand.Next(musicNotes.Length)];
                    float fontSize = rand.Next(20, 40);
                    float x = rand.Next(screenWidth);
                    float y = rand.Next(screenHeight);
                    Color baseColor = colors[rand.Next(colors.Length)];
                    int alpha = rand.Next(140, 200);
                    Color finalColor = Color.FromArgb(alpha, baseColor.R, baseColor.G, baseColor.B);

                    activeNotes.Add(new AnimatedNote
                    {
                        Note = note,
                        X = x,
                        Y = y,
                        FontSize = fontSize,
                        Color = finalColor,
                        Lifetime = 3 * frameRate // ~3 seconds
                    });
                }
            }
            // Draw and update active notes
            var fontFamilyLive = new FontFamily("Segoe UI Emoji");
            for (int i = activeNotes.Count - 1; i >= 0; i--)
            {
                var n = activeNotes[i];
                using (var font = new Font(fontFamilyLive, n.FontSize * multiplier, FontStyle.Bold))
                using (var brush = new SolidBrush(n.Color))
                {
                    graphics.DrawString(n.Note, font, brush, n.X, n.Y);
                }

                n.Lifetime--;
                if (n.Lifetime <= 0)
                    activeNotes.RemoveAt(i);
            }
        }

        class AnimatedNote
        {
            public string Note;
            public float X, Y;
            public float FontSize;
            public Color Color;
            public int Lifetime; // in frames
        }

        private List<AnimatedNote> activeNotes = new List<AnimatedNote>();
        private Random rand = new Random();

        private Bitmap GetTitleCard(string artFilePath)
        {
            var AvgBPM = AverageBPM();
            int resolutionX = do4KResolution ? 3840 : 1920;
            int resolutionY = do4KResolution ? 2160 : 1080;
            int multiplier = do4KResolution ? 2 : 1;

            Bitmap coverBitmap = null;
            int coverWidth = 512 * multiplier;
            int coverHeight = 512 * multiplier;

            if (File.Exists(artFilePath) && displayAlbumArt)
            {
                using (var src = Image.FromFile(artFilePath))
                {
                    coverBitmap = new Bitmap(coverWidth, coverHeight, PixelFormat.Format32bppArgb);
                    using (Graphics g = Graphics.FromImage(coverBitmap))
                    {
                        g.DrawImage(src, 0, 0, coverWidth, coverHeight);
                    }
                }
            }

            var lineHeight = resolutionY / 11;

            var bmp = new Bitmap(resolutionX, resolutionY, PixelFormat.Format32bppArgb);

            using (var graphics = Graphics.FromImage(bmp))
            {
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

                if (animatedVideoBackground)
                {
                    graphics.Clear(Color.Transparent);
                }
                else
                {
                    graphics.Clear(ColorTranslator.FromHtml(backgroundColor));
                }

                if (staticImageBackground && BackgroundOverride != null)
                {
                    graphics.DrawImage(BackgroundOverride, 0, 0, resolutionX, resolutionY);
                }

                var title = "\"" + Parser.Songs[0].Name.Replace("&", "&&").Replace("feat.", "ft.").Replace("featuring", "ft.") + "\"";
                var artist = Parser.Songs[0].Artist.Replace("&", "&&").Replace("feat.", "ft.").Replace("featuring", "ft.");
                var album = Parser.Songs[0].Album.Replace("&", "&&");
                var bpm = AvgBPM == 0 ? "" : "Tempo: " + Math.Round(AvgBPM, 0, MidpointRounding.AwayFromZero) + " BPM";

                var parts = 1;
                if ((doHarm2 || doHarm3) && Harm2Lyrics.Any()) parts++;
                if (doHarm3 && Harm3Lyrics.Any()) parts++;

                var vocalParts = "Vocals: " + ((doHarm2 || doHarm3) && Harm2Lyrics.Any() ? parts + "-part harmony" : "Solo");
                var songKey = "";
                var genre = Parser.doGenre(Parser.Songs[0].Genre).Replace("&", "&&");
                if (!string.IsNullOrEmpty(genre))
                {
                    genre = "Genre: " + genre;
                }

                var offset = 0;

                if (coverBitmap != null && displayAlbumArt)
                {
                    int artSize = 512 * multiplier;
                    int spacer = 100 * multiplier;

                    graphics.DrawImage(coverBitmap, spacer, (resolutionY - artSize) / 2, artSize, artSize);
                    offset = artSize + (int)(1.5 * spacer);
                }

                DrawCenteredLine(graphics, title, resolutionX, lineHeight * 3, 72f * multiplier, offset);
                DrawCenteredLine(graphics, artist, resolutionX, lineHeight * 4, 60f * multiplier, offset);
                DrawCenteredLine(graphics, album, resolutionX, lineHeight * 5, 48f * multiplier, offset);

                if (!string.IsNullOrEmpty(genre))
                    DrawCenteredLine(graphics, genre, resolutionX, lineHeight * 7, 32f * multiplier, offset);

                DrawCenteredLine(graphics, vocalParts, resolutionX, (int)(lineHeight * 7.7), 32f * multiplier, offset);

                if (!string.IsNullOrEmpty(songKey))
                    DrawCenteredLine(graphics, songKey, resolutionX, (int)(lineHeight * 8.4), 32f * multiplier, offset);

                if (!string.IsNullOrEmpty(bpm) && displayTempo)
                {
                    DrawCenteredLine(graphics, bpm, resolutionX, (int)(lineHeight * (string.IsNullOrEmpty(songKey) ? 8.4 : 9.1)), 32f * multiplier, offset);
                }
            }

            coverBitmap?.Dispose();
            return bmp;
        }

        private Bitmap GetEndingCard()
        {
            int resolutionX = do4KResolution ? 3840 : 1920;
            int resolutionY = do4KResolution ? 2160 : 1080;
            int multiplier = do4KResolution ? 2 : 1;

            var lineHeight = resolutionY / 11;

            var bmp = new Bitmap(resolutionX, resolutionY, PixelFormat.Format32bppArgb);

            using (var graphics = Graphics.FromImage(bmp))
            {
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

                if (animatedVideoBackground)
                {
                    graphics.Clear(Color.Transparent);
                }
                else
                {
                    graphics.Clear(ColorTranslator.FromHtml(backgroundColor));
                }

                if (staticImageBackground && BackgroundOverride != null)
                {
                    graphics.DrawImage(BackgroundOverride, 0, 0, resolutionX, resolutionY);
                }

                Bitmap logoBitmap = null;
                int logoWidth = 0;
                int logoHeight = 0;

                if (KaraokeLogo != null)
                {
                    logoWidth = KaraokeLogo.Width * multiplier;
                    logoHeight = KaraokeLogo.Height * multiplier;

                    logoBitmap = new Bitmap(logoWidth, logoHeight, PixelFormat.Format32bppArgb);
                    using (Graphics g = Graphics.FromImage(logoBitmap))
                    {
                        g.DrawImage(KaraokeLogo, 0, 0, logoWidth, logoHeight);
                    }
                }

                int logoX = (resolutionX - logoWidth) / 2;
                int logoY = (resolutionY - logoHeight) / 2;

                graphics.DrawImage(logoBitmap, logoX, logoY, logoWidth, logoHeight);

                DrawCenteredLine(graphics, "Created with Nautilus | www.nemosnautilus.com",
                                 resolutionX, lineHeight * 10, 24f * multiplier);
            }

            return bmp;
        }

        private Bitmap loadingBarBaseBmp;
        private Bitmap loadingBarHighlightBmp;
        private Size loadingBarSize;
        private Font loadingBarFont;

        private void PrepareLoadingBarAssets(int multiplier)
        {
            DisposeLoadingBarAssets();

            loadingBarFont = new Font(ActiveFontName, 24f * multiplier);

            loadingBarSize = TextRenderer.MeasureText(loadingBarXL, loadingBarFont);

            loadingBarBaseBmp = new Bitmap(loadingBarSize.Width, loadingBarSize.Height, PixelFormat.Format32bppArgb);
            loadingBarHighlightBmp = new Bitmap(loadingBarSize.Width, loadingBarSize.Height, PixelFormat.Format32bppArgb);

            using (Graphics gBase = Graphics.FromImage(loadingBarBaseBmp))
            {
                gBase.Clear(Color.Transparent);
                TextRenderer.DrawText(
                    gBase,
                    loadingBarXL,
                    loadingBarFont,
                    new Point(0, 0),
                    ColorTranslator.FromHtml(textColor1),
                    Color.Transparent);
            }

            using (Graphics gHi = Graphics.FromImage(loadingBarHighlightBmp))
            {
                gHi.Clear(Color.Transparent);
                TextRenderer.DrawText(
                    gHi,
                    loadingBarXL,
                    loadingBarFont,
                    new Point(0, 0),
                    ColorTranslator.FromHtml(highlightColor1),
                    Color.Transparent);
            }
        }

        private void DisposeLoadingBarAssets()
        {
            if (loadingBarBaseBmp != null)
            {
                loadingBarBaseBmp.Dispose();
                loadingBarBaseBmp = null;
            }

            if (loadingBarHighlightBmp != null)
            {
                loadingBarHighlightBmp.Dispose();
                loadingBarHighlightBmp = null;
            }

            if (loadingBarFont != null)
            {
                loadingBarFont.Dispose();
                loadingBarFont = null;
            }

            loadingBarSize = Size.Empty;
        }

        private void DoModernKaraoke(string folder, string artFilePath, string audioFilePath, string outputVideoPath)
        {
            Log("Rendering using Nemo's Modern Karaoke MP4 v2 format");
            
            string ffmpegPath = Path.Combine(Application.StartupPath, "bin", "ffmpeg.exe");
            if (!File.Exists(ffmpegPath))
            {
                var message = "ffmpeg is missing from the \\bin\\ folder and I can't render to MP4 without it, stopping";
                Log(message);
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var AvgBPM = AverageBPM();
            const int spawnFrequency = 30;
            var noteCounter = spawnFrequency;
            int resolutionX = do4KResolution ? 3840 : 1920;
            int resolutionY = do4KResolution ? 2160 : 1080;            
            int multiplier = do4KResolution ? 2 : 1;
            double vertOffset = 0;

            var lyricsRawPath = Path.GetDirectoryName(outputVideoPath) + "\\lyrics_raw.mp4";
            var backgroundPath = (!string.IsNullOrEmpty(_customBackgroundVideoPath) && File.Exists(_customBackgroundVideoPath)) ? _customBackgroundVideoPath : Application.StartupPath + "\\bin\\images\\background.mp4";
            var tempVideoPath = animatedVideoBackground ?  lyricsRawPath : outputVideoPath;
            Log("Resolution: " + resolutionX + "*" + resolutionY);
            Log($"Frame rate: {frameRate} FPS");
            Log("Average BPM: " + (int)(AvgBPM + 0.5));
            Log("Rendering " + totalFrames + " frames ... stand by");


            this.Invoke(new Action(() =>
            {
                videoPreview.Visible = false;
                picPreview.Visible = true;
            }));            

            if (enablePreview)
            {
                this.Invoke(new Action(() =>
                {
                    picPreview.Image = null;
                    picPreview.Width = 288;
                    picPreview.Height = 162;
                    lblFrames.Visible = true;
                    lblFrames.Text = "Processing...";
                }));
            }

            //ffmpeg stuff
            int byteCount = resolutionX * resolutionY * 4;
            byte[] rawBuffer = new byte[byteCount];

            Process ffmpeg = new Process();
            ffmpeg.StartInfo.FileName = ffmpegPath;
            ffmpeg.StartInfo.UseShellExecute = false;
            ffmpeg.StartInfo.RedirectStandardInput = true;
            ffmpeg.StartInfo.CreateNoWindow = true;

            string bgRawPath = Path.Combine(Path.GetDirectoryName(outputVideoPath) ?? "", "bg_raw.mp4");

            if (animatedVideoBackground && File.Exists(backgroundPath))
            {              
                string prepArgs =
                    $"-y " +
                    $"-stream_loop -1 -i \"{backgroundPath}\" " +
                    $"-filter_complex \"[0:v]" +
                        $"scale={resolutionX}:{resolutionY}," +
                        $"fps={frameRate},format=yuv420p[v]\" " +
                    $"-map \"[v]\" " +
                    $"-t {songDuration.ToString(System.Globalization.CultureInfo.InvariantCulture)} " +
                    $"-c:v libx264 -preset veryfast -crf 18 -an " +
                    $"\"{bgRawPath}\"";

                using (var ffmpegPrep = new Process())
                {
                    ffmpegPrep.StartInfo.FileName = ffmpegPath;
                    ffmpegPrep.StartInfo.Arguments = prepArgs;
                    ffmpegPrep.StartInfo.UseShellExecute = false;
                    ffmpegPrep.StartInfo.RedirectStandardOutput = true;
                    ffmpegPrep.StartInfo.RedirectStandardError = true;
                    ffmpegPrep.StartInfo.CreateNoWindow = true;
                    ffmpegPrep.Start();

                    string prepStderr = ffmpegPrep.StandardError.ReadToEnd();
                    ffmpegPrep.WaitForExit();

                    if (ffmpegPrep.ExitCode != 0)
                    {
                        throw new Exception("FFmpeg background prep failed:\n" + prepStderr);
                    }
                }

                Log("Rendering composite karaoke video ... stand by");

                ffmpeg.StartInfo.Arguments =
                    $"-y " +
                    $"-i \"{bgRawPath}\" " +
                    $"-f rawvideo -pixel_format bgra -video_size {resolutionX}x{resolutionY} -framerate {frameRate} -i - " +
                    $"-i \"{audioFilePath}\" " +
                    $"-filter_complex \"[0:v][1:v]overlay=0:0:format=auto[v]\" " +
                    $"-map \"[v]\" " +
                    $"-map 2:a " +
                    $"-c:v libx264 -preset veryfast -crf 18 -pix_fmt yuv420p " +
                    $"-c:a aac -b:a 192k -shortest " +
                    $"\"{outputVideoPath}\"";
            }
            else
            {
                Log("Rendering karaoke video ... stand by");

                var videoQuality = "-crf 18 -pix_fmt yuv420p";

                ffmpeg.StartInfo.Arguments =
                    $"-y " +
                    $"-f rawvideo -pixel_format bgra -video_size {resolutionX}x{resolutionY} -framerate {frameRate} " +
                    $"-i - " +
                    $"-i \"{audioFilePath}\"{videoFilter} " +
                    $"-c:v libx264 -preset veryfast {videoQuality} " +
                    $"-c:a aac -b:a 192k -shortest \"{outputVideoPath}\"";
            }

            ffmpeg.Start();
            var ffmpegStream = ffmpeg.StandardInput.BaseStream;

            //render background for use in parallel processing
            Bitmap renderedBackground = new Bitmap(resolutionX, resolutionY, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(renderedBackground))
            {
                if (animatedVideoBackground)
                {
                    g.Clear(Color.Transparent);
                }
                else
                {
                    g.Clear(ColorTranslator.FromHtml(backgroundColor));
                }
                if (staticImageBackground && BackgroundOverride != null)
                {
                    g.DrawImage(BackgroundOverride, 0, 0, resolutionX, resolutionY);
                }
            }

            // Copy pixel data to raw byte buffer
            var bgData = new byte[resolutionX * resolutionY * 4];
            var rect = new Rectangle(0, 0, resolutionX, resolutionY);
            var bmpData = renderedBackground.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(bmpData.Scan0, bgData, 0, bgData.Length);
            renderedBackground.UnlockBits(bmpData);
            renderedBackground.Dispose();                
                                
            var prepBmp = new Bitmap(8, 8);
            var prepGraphics = Graphics.FromImage(prepBmp);
            ApplyTextRenderingSettings(prepGraphics);
            var prepBaseFont = new Font(ActiveFontName, 48f, FontStyle.Bold);
            var preparedLead = PrepareTrack(
                            prepGraphics,
                            VocalLyrics,
                            VocalPhrases,
                            prepBaseFont,
                            resolutionX,
                            textColor1,
                            highlightColor1);

            var preparedHarm2 = PrepareTrack(
                            prepGraphics,
                            Harm2Lyrics,
                            Harm2Phrases,
                            prepBaseFont,
                            resolutionX,
                            textColor2,
                            highlightColor2);
            
            var preparedHarm3 = PrepareTrack(
                            prepGraphics,
                            Harm3Lyrics,
                            Harm3Phrases,
                            prepBaseFont,
                            resolutionX,
                            textColor3,
                            highlightColor3);

            PrepareLoadingBarAssets(multiplier);

            Bitmap titleCard = GetTitleCard(artFilePath);
            Bitmap endingCard = GetEndingCard();

            using (var bmp = new Bitmap(resolutionX, resolutionY, PixelFormat.Format32bppArgb))
            {
                using (var graphics = Graphics.FromImage(bmp))
                {
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

                    for (var frame = 0; frame < totalFrames; frame++)
                    {
                        if (cancelProcess) { break; }
                        try
                        {
                            noteCounter++;
                            double frameDuration = 1.0 / frameRate;
                            double time = frame * frameDuration * 1000.0;
                            double adjustedTime = Math.Max(0, time - syncOffsetMs);

                            rect = new Rectangle(0, 0, resolutionX, resolutionY);
                            bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                            Marshal.Copy(bgData, 0, bmpData.Scan0, bgData.Length);
                            bmp.UnlockBits(bmpData);

                            graphics.ResetTransform();
                            graphics.ResetClip();                                               

                            LyricPhrase actualNextLineHarmony = null;
                            LyricPhrase actualLastLineHarmony = null;
                            LyricPhrase currentLineLead = null;
                            LyricPhrase nextLineLead = null;
                            LyricPhrase lastLineLead = null;
                            LyricPhrase actualLastLineLead = null;
                            LyricPhrase actualNextLineLead = null;
                            bool hasInlineGap = false;

                            var phrasesLead = Harm2Lyrics.Any() && (doHarm2 || doHarm3) ? Harm1Phrases : VocalPhrases;
                            var lyricsLead = Harm2Lyrics.Any() && (doHarm2 || doHarm3) ? Harm1Lyrics : VocalLyrics;

                            if (phrasesLead == null || phrasesLead.Count == 0)
                            {
                                phrasesLead = VocalPhrases;
                                lyricsLead = VocalLyrics;
                            }

                            // IMPORTANT: only phrases with displayable lyrics should participate
                            // in lead phrase pairing / preview / loading-bar logic
                            var drawablePhrasesLead = phrasesLead
                                .Where(p => HasDisplayableLyrics(p.PhraseText))
                                .ToList();

                            double previewTime = adjustedTime + highlightDelay;
                            int lastPhraseIndex = 0;
                            bool phrase2IsDup = false;

                            for (var i = lastPhraseIndex; i < drawablePhrasesLead.Count; i++)
                            {
                                i = lastPhraseIndex;
                                if (i < 0) continue;
                                if (i >= drawablePhrasesLead.Count) break;

                                lastLineLead = lastPhraseIndex > 0 ? drawablePhrasesLead[lastPhraseIndex - 1] : null;
                                var phrase1 = drawablePhrasesLead[lastPhraseIndex];
                                var phrase2 = phrase1; // for harmonies and when there's a gap, only show one phrase per page

                                var harmonies = (doHarm2 || doHarm3) && Harm2Lyrics.Any(); // there's no case of Harm3Lyrics without Harm2Lyrics

                                phrase2IsDup = false;
                                if (lastPhraseIndex < drawablePhrasesLead.Count - 1)
                                {
                                    var currentIndex = lastPhraseIndex + 1;
                                    if (drawablePhrasesLead[currentIndex].PhraseStart - phrase1.PhraseEnd < timeGap && !harmonies)
                                    {
                                        phrase2 = drawablePhrasesLead[currentIndex];
                                        currentIndex++;
                                    }
                                    else
                                    {
                                        phrase2IsDup = true;
                                    }

                                    if (phrase2.PhraseEnd <= time)
                                    {
                                        actualLastLineLead = phrase2; // whether phrase1 or phrase2 based on assignment above
                                        if (currentIndex <= drawablePhrasesLead.Count - 1)
                                        {
                                            actualNextLineLead = drawablePhrasesLead[currentIndex];
                                        }
                                    }
                                }
                                else
                                {
                                    phrase2IsDup = true; // last phrase, must be null for nextLine
                                }

                                if (previewTime >= phrase1.PhraseStart && time < phrase2.PhraseEnd)
                                {
                                    try
                                    {
                                        var gap = phrase2.PhraseStart - phrase1.PhraseEnd >= timeGap && !harmonies;
                                        if (gap)
                                        {
                                            if (hasInlineGap && time > phrase1.PhraseEnd)
                                            {
                                                currentLineLead = null;
                                                nextLineLead = phrase2;
                                            }
                                            else
                                            {
                                                currentLineLead = phrase1;
                                                nextLineLead = null;
                                                hasInlineGap = true;
                                            }
                                            vertOffset = 1.5;
                                        }
                                        else
                                        {
                                            currentLineLead = phrase1;
                                            nextLineLead = phrase2IsDup ? null : phrase2;
                                            hasInlineGap = false;
                                            vertOffset = phrase2IsDup ? 1.5 : 0.0;
                                        }
                                    }
                                    catch { }
                                    break;
                                }

                                if (harmonies || phrase2IsDup)
                                {
                                    lastPhraseIndex++;
                                }
                                else
                                {
                                    lastPhraseIndex += 2;
                                }
                            }

                            if (actualNextLineLead == null)
                            {
                                actualNextLineLead = drawablePhrasesLead.FirstOrDefault(p => p.PhraseStart > previewTime);
                            }

                            if (actualLastLineLead == null)
                            {
                                actualLastLineLead = drawablePhrasesLead.LastOrDefault(p => p.PhraseEnd <= previewTime);
                            }
                            
                            LyricPhrase currentLineHarm2 = null;
                            LyricPhrase lastLineHarm2 = null;
                            if (doHarm2 || doHarm3)
                            {
                                for (var i = 0; i < Harm2Phrases.Count(); i++)
                                {
                                    var phrase = Harm2Phrases[i];
                                    lastLineHarm2 = i > 0 ? Harm2Phrases[i - 1] : null;

                                    if (phrase.PhraseEnd <= time)
                                    {
                                        actualLastLineHarmony = Harm2Phrases[i];
                                        if (i < Harm2Phrases.Count() - 1)
                                        {
                                            actualNextLineHarmony = Harm2Phrases[i + 1];
                                        }
                                    }

                                    if (previewTime >= phrase.PhraseStart && time < phrase.PhraseEnd)
                                    {
                                        currentLineHarm2 = phrase;
                                        break;
                                    }
                                }
                                if (actualNextLineHarmony == null)
                                {
                                    actualNextLineHarmony = Harm2Phrases.FirstOrDefault(p => HasDisplayableLyrics(p.PhraseText) && p.PhraseStart > previewTime);
                                }
                                if (actualLastLineHarmony == null)
                                {
                                    actualLastLineHarmony = Harm2Phrases.LastOrDefault(p => HasDisplayableLyrics(p.PhraseText) && p.PhraseEnd <= previewTime);
                                }
                            }

                            LyricPhrase currentLineHarm3 = null;
                            LyricPhrase lastLineHarm3 = null;
                            if (doHarm3)
                            {
                                for (var i = 0; i < Harm3Phrases.Count(); i++)
                                {
                                    var phrase = Harm3Phrases[i];
                                    lastLineHarm3 = i > 0 ? Harm3Phrases[i - 1] : null;

                                    if (previewTime >= phrase.PhraseStart && time < phrase.PhraseEnd)
                                    {
                                        currentLineHarm3 = phrase;
                                        break;
                                    }
                                }
                            }

                            var lineHeight = resolutionY / 11;
                            var harm1LineTop1 = 0; ;
                            var harm1LineTop2 = 0;
                            var harm1LineTop3 = 0;
                            var harm1LineTop4 = 0;
                            var harm2LineTop1 = 0;
                            var harm2LineTop2 = 0;
                            var harm3LineTop1 = 0;
                            var harm3LineTop2 = 0;

                            if (doSoloVocals || !Harm2Lyrics.Any()) //do solo vocals
                            {
                                harm1LineTop1 = (int)(lineHeight * (2.5 + vertOffset));
                                harm1LineTop2 = (int)(lineHeight * (4.0 + vertOffset));
                                harm1LineTop3 = (int)(lineHeight * 5.5);
                                harm1LineTop4 = (int)(lineHeight * 7.0);
                            }
                            if (doHarm3 && Harm3Lyrics.Any())
                            {
                                harm1LineTop1 = lineHeight * 0;
                                harm1LineTop2 = (int)(lineHeight * 1.5);
                                harm2LineTop1 = lineHeight * 4;
                                harm2LineTop2 = (int)(lineHeight * 5.5);
                                harm3LineTop1 = lineHeight * 8;
                                harm3LineTop2 = (int)(lineHeight * 9.5);
                            }
                            else if ((doHarm2 || doHarm3) && Harm2Lyrics.Any())
                            {
                                harm1LineTop1 = lineHeight * 2;
                                harm1LineTop2 = (int)(lineHeight * 3.5);
                                harm2LineTop1 = lineHeight * 6;
                                harm2LineTop2 = (int)(lineHeight * 7.5);
                            }

                            if (time + highlightDelay < phrasesLead.First().PhraseStart)
                            {                                
                                graphics.DrawImageUnscaled(titleCard, 0, 0);
                                goto DoSaveFrame;
                            }

                            PreparedPhrase preparedLastLead = FindPreparedPhrase(preparedLead.Phrases, lastLineLead);
                            PreparedPhrase preparedCurrentLead = FindPreparedPhrase(preparedLead.Phrases, currentLineLead);
                            PreparedPhrase preparedNextLead = FindPreparedPhrase(preparedLead.Phrases, nextLineLead);

                            PreparedPhrase preparedLastHarm2 = FindPreparedPhrase(preparedHarm2.Phrases, lastLineHarm2);
                            PreparedPhrase preparedCurrentHarm2 = FindPreparedPhrase(preparedHarm2.Phrases, currentLineHarm2);

                            PreparedPhrase preparedLastHarm3 = FindPreparedPhrase(preparedHarm3.Phrases, lastLineHarm3);
                            PreparedPhrase preparedCurrentHarm3 = FindPreparedPhrase(preparedHarm3.Phrases, currentLineHarm3);

                            if (time > phrasesLead.Last().PhraseEnd)
                            {
                                graphics.DrawImageUnscaled(endingCard, 0, 0);
                                goto DoSaveFrame;
                            }                            

                            bool shouldDrawLoadingBar = false;
                            double? LastEnd = 0.0;
                            double? NextStart = 0.0;
                            double loadingGap = 0.0;
                            double wait = 0.0;

                            if (doShowLoadingBar)
                            {
                                bool leadPhraseActive =
                                    (currentLineLead != null && time >= currentLineLead.PhraseStart && time < currentLineLead.PhraseEnd) ||
                                    (nextLineLead != null && time >= nextLineLead.PhraseStart && time < nextLineLead.PhraseEnd);

                                bool harm2PhraseActive =
                                    currentLineHarm2 != null && time >= currentLineHarm2.PhraseStart && time < currentLineHarm2.PhraseEnd;

                                bool harm3PhraseActive =
                                    currentLineHarm3 != null && time >= currentLineHarm3.PhraseStart && time < currentLineHarm3.PhraseEnd;

                                bool anyPhraseActive = leadPhraseActive || harm2PhraseActive || harm3PhraseActive;

                                if (!anyPhraseActive)
                                {
                                    if (actualLastLineLead?.PhraseEnd > actualLastLineHarmony?.PhraseEnd)
                                    {
                                        LastEnd = actualLastLineLead?.PhraseEnd;
                                    }
                                    else
                                    {
                                        LastEnd = actualLastLineHarmony?.PhraseEnd;
                                    }

                                    // fallback
                                    if (LastEnd == null)
                                    {
                                        if (actualLastLineLead != null)
                                        {
                                            LastEnd = actualLastLineLead.PhraseEnd;
                                        }
                                        else if (actualLastLineHarmony != null)
                                        {
                                            LastEnd = actualLastLineHarmony.PhraseEnd;
                                        }
                                    }

                                    if (actualNextLineLead?.PhraseStart < actualNextLineHarmony?.PhraseStart)
                                    {
                                        NextStart = actualNextLineLead?.PhraseStart;
                                    }
                                    else
                                    {
                                        NextStart = actualNextLineHarmony?.PhraseStart;
                                    }

                                    // fallback
                                    if (NextStart == null)
                                    {
                                        if (actualNextLineLead != null)
                                        {
                                            NextStart = actualNextLineLead.PhraseStart;
                                        }
                                        else if (actualNextLineHarmony != null)
                                        {
                                            NextStart = actualNextLineHarmony.PhraseStart;
                                        }
                                    }

                                    if (LastEnd != null && NextStart != null)
                                    {
                                        loadingGap = NextStart.Value - LastEnd.Value;
                                        wait = NextStart.Value - previewTime;
                                        shouldDrawLoadingBar = loadingGap >= timeGap && wait > 0;
                                    }
                                }
                            }

                            bool currentLeadStillActive =
                            (currentLineLead != null &&
                            time < currentLineLead.PhraseEnd) || 
                            (nextLineLead != null &&
                            time < nextLineLead.PhraseEnd);

                            if (currentLeadStillActive)
                            {
                                shouldDrawLoadingBar = false;
                            }                            

                            if (shouldDrawLoadingBar)
                            {
                                int posX = (resolutionX - loadingBarSize.Width) / 2;
                                int posY = (resolutionY - loadingBarSize.Height) / 2;

                                graphics.DrawImageUnscaled(loadingBarBaseBmp, posX, posY);

                                double progress = 1.0 - (wait / loadingGap);
                                progress = Math.Max(0.0, Math.Min(1.0, progress));

                                int highlightWidth = (int)Math.Round(loadingBarSize.Width * progress);

                                if (highlightWidth > 0)
                                {
                                    Rectangle src = new Rectangle(0, 0, Math.Min(highlightWidth, loadingBarHighlightBmp.Width), loadingBarHighlightBmp.Height);
                                    Rectangle dest = new Rectangle(posX, posY, src.Width, src.Height);

                                    graphics.DrawImage(loadingBarHighlightBmp, dest, src, GraphicsUnit.Pixel);
                                }

                                if (wait / 1000.0 <= 5.5 && wait / 1000.0 > 1.0)
                                {
                                    string waitString = ((int)wait / 1000).ToString();
                                    DrawCenteredLine(
                                        graphics,
                                        waitString,
                                        resolutionX,
                                        (resolutionY - TextRenderer.MeasureText(waitString, new Font(ActiveFontName, 80f * multiplier)).Height) / 2,
                                        80f * multiplier);
                                }

                                if (doShowAnimatedNotes)
                                {
                                    DrawAnimatedNotes(graphics, noteCounter, spawnFrequency, resolutionX, resolutionY);
                                }

                                goto DoSaveFrame;
                            }

                            DrawPreparedLeadGroup(
                                graphics,
                                preparedLastLead,
                                preparedCurrentLead,
                                preparedNextLead,
                                adjustedTime,
                                harm1LineTop1,
                                harm1LineTop2,
                                harm1LineTop3,
                                harm1LineTop4,
                                time,
                                vertOffset
                            );

                            DrawPreparedHarmonyGroup(
                            graphics,
                            preparedLastHarm2,
                            preparedCurrentHarm2,
                            preparedLastHarm3,
                            preparedCurrentHarm3,
                            harm2LineTop1,
                            harm2LineTop2,
                            harm3LineTop1,
                            harm3LineTop2,
                            adjustedTime,
                            time
                        );

                        DoSaveFrame:
                            BitmapToBGRA32IntoBuffer(bmp, rawBuffer);
                            ffmpegStream.Write(rawBuffer, 0, byteCount);

                            if (enablePreview)
                            {
                                if (frame % 10 == 0) //show only every 10 frames
                                {
                                    try //try to display preview but screw it if any frame fails just skip it
                                    {
                                        var preview = (Bitmap)bmp.Clone();
                                        this.Invoke(new Action(() =>
                                        {
                                            picPreview.Image?.Dispose();
                                            picPreview.Image = preview;
                                            lblFrames.Text = frame + "/" + totalFrames;
                                        }));
                                    }
                                    catch { }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            var message = $"Error rendering frame: {frame} out of {totalFrames}\n\n{ex.Message}\nn\n{ex.StackTrace}";
                            Log(message);
                            //MessageBox.Show(message, "Rendering Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }                       
                    }
                    
                    bmp?.Dispose();
                    DisposeLoadingBarAssets();
                    ffmpegStream.Flush();
                    ffmpegStream.Close();
                    ffmpeg.WaitForExit();

                    this.Invoke(new Action(() =>
                    {
                        picPreview.Image?.Dispose();
                        picPreview.Image = BackgroundOverride;
                        lblFrames.Visible = false;
                        if (animatedVideoBackground)
                        {
                            picPreview.Visible = false;
                            videoPreview.Visible = true;
                            DroppedPreviewVideo(backgroundPath);
                        }
                    }));
                                        
                    Tools.DeleteFile(bgRawPath);
                    Tools.DeleteFile(artFilePath);//delete temp album art

                    if (cancelProcess)
                    {
                        Tools.DeleteFile(outputVideoPath);//ffmpeg will have produced a partial video, delete it
                    }
                }
            }                       
        }

        private void DrawPreparedHarmonyGroup(
            Graphics graphics,
            PreparedPhrase preparedLastHarm2,
            PreparedPhrase preparedCurrentHarm2,
            PreparedPhrase preparedLastHarm3,
            PreparedPhrase preparedCurrentHarm3,
            int harm2LineTop1,
            int harm2LineTop2,
            int harm3LineTop1,
            int harm3LineTop2,
            double adjustedTime,
            double time)
        {
            if (preparedCurrentHarm2 != null && (doHarm2 || doHarm3))
            {
                bool hasHarm21 = preparedCurrentHarm2.Line1 != null && preparedCurrentHarm2.Line1.HasText;
                bool hasHarm22 = preparedCurrentHarm2.Line2 != null && preparedCurrentHarm2.Line2.HasText;

                double previousHarm2Time = preparedLastHarm2?.LastLyricTime ?? 0;

                using (var harm2Font = new Font(ActiveFontName, preparedCurrentHarm2.FontSize, FontStyle.Bold))
                {
                    if (doEnableHighlightAnimation && hasHarm21)
                    {
                        DrawPreparedPhraseAnimation(
                            graphics,
                            preparedCurrentHarm2,
                            preparedCurrentHarm2.Line1,
                            harm2Font,
                            harm2LineTop1,
                            time,
                            ColorTranslator.FromHtml(highlightColor2),
                            previousHarm2Time
                        );
                    }
                }

                if (hasHarm21)
                    DrawPreparedLine(graphics, preparedCurrentHarm2.Line1, harm2LineTop1, adjustedTime);

                if (hasHarm22)
                    DrawPreparedLine(graphics, preparedCurrentHarm2.Line2, harm2LineTop2, adjustedTime);
            }

            if (preparedCurrentHarm3 != null && doHarm3)
            {
                bool hasHarm31 = preparedCurrentHarm3.Line1 != null && preparedCurrentHarm3.Line1.HasText;
                bool hasHarm32 = preparedCurrentHarm3.Line2 != null && preparedCurrentHarm3.Line2.HasText;

                double previousHarm3Time = preparedLastHarm3?.LastLyricTime ?? 0;

                using (var harm3Font = new Font(ActiveFontName, preparedCurrentHarm3.FontSize, FontStyle.Bold))
                {
                    if (doEnableHighlightAnimation && hasHarm31)
                    {
                        DrawPreparedPhraseAnimation(
                            graphics,
                            preparedCurrentHarm3,
                            preparedCurrentHarm3.Line1,
                            harm3Font,
                            harm3LineTop1,
                            time,
                            ColorTranslator.FromHtml(highlightColor3),
                            previousHarm3Time
                        );
                    }
                }

                if (hasHarm31)
                    DrawPreparedLine(graphics, preparedCurrentHarm3.Line1, harm3LineTop1, adjustedTime);

                if (hasHarm32)
                    DrawPreparedLine(graphics, preparedCurrentHarm3.Line2, harm3LineTop2, adjustedTime);
            }
        }       

        private void DrawPreparedLine(Graphics g, PreparedLine line, int y, double adjustedTime)
        {
            if (line == null || !line.HasText || line.BaseBitmap == null)
                return;

            g.DrawImage(line.BaseBitmap, line.PosX, y);

            if (line.SyllableMasks == null || line.SyllableMasks.Count == 0 || line.Syllables == null)
            {
                // fallback to old behavior if needed
                if (line.HighlightBitmap != null)
                {
                    float highlightWidth = GetHighlightedPixelWidth(line.Syllables, adjustedTime);
                    highlightWidth = Math.Max(0f, Math.Min(highlightWidth, line.TextWidth));

                    int hw = (int)highlightWidth;
                    if (hw > 0)
                    {
                        Rectangle src = new Rectangle(0, 0, hw, line.TextHeight);
                        Rectangle dest = new Rectangle(line.PosX, y, hw, line.TextHeight);
                        g.DrawImage(line.HighlightBitmap, dest, src, GraphicsUnit.Pixel);
                    }
                }
                return;
            }

            for (int i = 0; i < line.Syllables.Count && i < line.SyllableMasks.Count; i++)
            {
                MergedSyllable syllable = line.Syllables[i];
                PreparedSyllableMask mask = line.SyllableMasks[i];

                if (mask == null || !mask.HasPixels)
                    continue;

                if (adjustedTime >= syllable.End)
                {
                    g.DrawImage(mask.Bitmap, line.PosX + mask.X, y + mask.Y);
                }
                else if (adjustedTime > syllable.Start)
                {
                    double duration = syllable.End - syllable.Start;
                    double progress = duration <= 0
                        ? 1.0
                        : (adjustedTime - syllable.Start) / duration;

                    progress = Math.Max(0.0, Math.Min(1.0, progress));

                    int partialWidth = (int)Math.Ceiling(mask.Width * progress);
                    if (partialWidth > 0)
                    {
                        Rectangle src = new Rectangle(0, 0, partialWidth, mask.Height);
                        Rectangle dest = new Rectangle(line.PosX + mask.X, y + mask.Y, partialWidth, mask.Height);
                        g.DrawImage(mask.Bitmap, dest, src, GraphicsUnit.Pixel);
                    }

                    break;
                }
                else
                {
                    break;
                }
            }
        }       

        private void DrawPreparedPhraseAnimation(
            Graphics graphics,
            PreparedPhrase phrase,
            PreparedLine line,
            Font font,
            int y,
            double time,
            Color color,
            double previousLastLyricTime)
        {
            if (phrase == null || line == null || !line.HasText)
                return;

            double totalGapDuration = phrase.FirstLyricTime - previousLastLyricTime;
            double leadDuration = Math.Min(highlightDelay, totalGapDuration);

            if (leadDuration <= 0)
                return;

            DrawHighlightAnimation(
                graphics,
                font,
                phrase.FirstLyricTime,
                leadDuration,
                line.PosX,
                y,
                color,
                time
            );
        }

        private void DrawPreparedLeadGroup(
            Graphics graphics,
            PreparedPhrase preparedLastLead,
            PreparedPhrase preparedCurrentLead,
            PreparedPhrase preparedNextLead,
            double adjustedTime,
            int harm1LineTop1,
            int harm1LineTop2,
            int harm1LineTop3,
            int harm1LineTop4,
            double time,
            double vertOffset)
        {
            bool hasCurrent1 = preparedCurrentLead?.Line1 != null && preparedCurrentLead.Line1.HasText;
            bool hasCurrent2 = preparedCurrentLead?.Line2 != null && preparedCurrentLead.Line2.HasText;
            bool hasNext1 = preparedNextLead?.Line1 != null && preparedNextLead.Line1.HasText;
            bool hasNext2 = preparedNextLead?.Line2 != null && preparedNextLead.Line2.HasText;

            bool hasCurrent = hasCurrent1 || hasCurrent2;
            bool hasNext = (doSoloVocals || !Harm2Lyrics.Any()) && (hasNext1 || hasNext2);                      

            Color animColor = ColorTranslator.FromHtml(highlightColor1);

            double previousLeadTime = preparedLastLead?.LastLyricTime ?? 0;
            double previousNextLeadTime = preparedCurrentLead?.LastLyricTime ?? previousLeadTime;

            if (hasCurrent)
            {
                using (var currentFont = new Font(ActiveFontName, preparedCurrentLead.FontSize, FontStyle.Bold))
                {
                    if (doEnableHighlightAnimation && hasCurrent1)
                    {
                        DrawPreparedPhraseAnimation(
                            graphics,
                            preparedCurrentLead,
                            preparedCurrentLead.Line1,
                            currentFont,
                            harm1LineTop1,
                            time,
                            animColor,
                            previousLeadTime
                        );
                    }
                }

                if (hasCurrent1)
                    DrawPreparedLine(graphics, preparedCurrentLead.Line1, harm1LineTop1, adjustedTime);

                if (hasCurrent2)
                    DrawPreparedLine(graphics, preparedCurrentLead.Line2, harm1LineTop2, adjustedTime);
            }

            if (hasNext)
            {
                using (var nextFont = new Font(ActiveFontName, preparedNextLead.FontSize, FontStyle.Bold))
                {
                    if (doEnableHighlightAnimation && hasNext1)
                    {
                        DrawPreparedPhraseAnimation(
                            graphics,
                            preparedNextLead,
                            preparedNextLead.Line1,
                            nextFont,
                            harm1LineTop3,
                            time,
                            animColor,
                            previousNextLeadTime
                        );
                    }
                }

                if (hasNext1)
                    DrawPreparedLine(graphics, preparedNextLead.Line1, harm1LineTop3, adjustedTime);

                if (hasNext2)
                    DrawPreparedLine(graphics, preparedNextLead.Line2, harm1LineTop4, adjustedTime);
            }
        }

        private PreparedPhrase FindPreparedPhrase(List<PreparedPhrase> prepared, LyricPhrase phrase)
        {
            if (phrase == null) return null;

            return prepared.FirstOrDefault(p =>
                Math.Abs(p.PhraseStart - phrase.PhraseStart) < 0.0001 &&
                Math.Abs(p.PhraseEnd - phrase.PhraseEnd) < 0.0001);
        }    
          
        private PreparedTrack PrepareTrack(
            Graphics g,
            List<KaraokeLyric> allLyrics,
            IEnumerable<LyricPhrase> phrases,
            Font baseFont,
            int resolutionX,
            string textColor,
            string highlightColor)
        {
            var track = new PreparedTrack();

            foreach (var phrase in phrases)
            {
                var prepared = PreparePhrase(
                    g,
                    allLyrics,
                    phrase,
                    baseFont,
                    resolutionX,
                    textColor,
                    highlightColor);

                if (prepared != null && prepared.HasAnyText)
                {
                    track.Phrases.Add(prepared);
                }
            }

            return track;
        }

        private PreparedPhrase PreparePhrase(
            Graphics g,
            List<KaraokeLyric> allLyrics,
            LyricPhrase phrase,
            Font baseFont,
            int resolutionX,
            string textColor,
            string highlightColor)
        {
            var phraseSyllables = allLyrics
                .Where(s => s.End > phrase.PhraseStart && s.Start <= phrase.PhraseEnd)
                .OrderBy(s => s.Start)
                .ToList();

            if (phraseSyllables.Count == 0)
                return null;

            string rawPhraseText = string.Join(" ", phraseSyllables
                .Where(s => !string.IsNullOrWhiteSpace(s.Lyric) && s.Lyric != "+" && s.Lyric != "-")
                .Select(s => s.Lyric.Replace("‿", " ")));

            var (line1Syllables, line2Syllables) = SplitSyllablesByPixelWidth(phraseSyllables, baseFont, g);

            string line1Preview = string.Join(" ", JoinWordsForDisplay(line1Syllables));
            string line2Preview = string.Join(" ", JoinWordsForDisplay(line2Syllables));
            string widestLine = line1Preview.Length > line2Preview.Length ? line1Preview : line2Preview;

            int multiplier = do4KResolution ? 2 : 1;
            float scaledFontSize = GetScaledFontSize(g, widestLine, baseFont, 100f * multiplier, resolutionX);

            using (var displayFont = new Font(baseFont.FontFamily, scaledFontSize))
            {
                var prepared = new PreparedPhrase
                {
                    PhraseStart = phrase.PhraseStart,
                    PhraseEnd = phrase.PhraseEnd,
                    PhraseText = rawPhraseText,
                    FontSize = scaledFontSize,
                    FirstLyricTime = GetFirstLyricStart(allLyrics, phrase.PhraseStart, phrase.PhraseEnd),
                    LastLyricTime = GetLastLyricEnd(allLyrics, phrase.PhraseStart, phrase.PhraseEnd),
                    Line1 = PrepareLine(g, line1Syllables, displayFont, resolutionX, textColor, highlightColor),
                    Line2 = PrepareLine(g, line2Syllables, displayFont, resolutionX, textColor, highlightColor)
                };
                return prepared;
            }
        }

        IEnumerable<string> JoinWordsForDisplay(List<KaraokeLyric> syls)
        {
            var words = new List<string>();
            var buf = new List<KaraokeLyric>();
            for (int i = 0; i < syls.Count; i++)
            {
                buf.Add(syls[i]);
                string t = syls[i].Lyric ?? "";
                bool endWord = !(t.EndsWith("-") || t == "+" ||
                                (i + 1 < syls.Count && syls[i + 1].Lyric == "+"));
                if (endWord)
                {
                    string word = string.Join("", buf.Select(b => (b.Lyric ?? "")
                        .Replace("+", "").Replace("-", "").Replace("‿", " "))).Trim();
                    if (word.Length > 0) words.Add(word);
                    buf.Clear();
                }
            }
            if (buf.Count > 0)
            {
                string tail = string.Join("", buf.Select(b => (b.Lyric ?? "")
                    .Replace("+", "").Replace("-", "").Replace("‿", " "))).Trim();
                if (tail.Length > 0) words.Add(tail);
            }
            return words;
        }

        double GetFirstLyricStart(List<KaraokeLyric> lyrics, double phraseStart, double phraseEnd)
        {
            return lyrics
                .Where(lyr => lyr.Start >= phraseStart && lyr.Start <= phraseEnd)
                .OrderBy(lyr => lyr.Start)
                .Select(lyr => lyr.Start)
                .FirstOrDefault(); // returns 0.0 if none found

        }

        double GetLastLyricEnd(List<KaraokeLyric> lyrics, double phraseStart, double phraseEnd)
        {
            return lyrics
                .Where(lyr => lyr.End >= phraseStart && lyr.Start <= phraseEnd)
                .OrderByDescending(lyr => lyr.End)
                .Select(lyr => lyr.End)
                .LastOrDefault(); // returns 0.0 if none found
        }

        private PreparedLine PrepareLine(
            Graphics g,
            List<KaraokeLyric> originalSyllables,
            Font font,
            int resolutionX,
            string baseColor,
            string highlightColor)
        {
            var prepared = new PreparedLine();

            if (originalSyllables == null || originalSyllables.Count == 0)
                return prepared;
                        
            var merged = MergeSustainedSyllables(originalSyllables);
            string displayText = ReconstructPhraseTextFromSyllables(merged);

            if (string.IsNullOrWhiteSpace(displayText))
                return prepared;

            ApplyTextRenderingSettings(g);

            SizeF sizeF = g.MeasureString(displayText.Replace("‿", " "), font, int.MaxValue);
            int textWidth = Math.Max(1, (int)Math.Ceiling(sizeF.Width));
            int textHeight = Math.Max(1, (int)Math.Ceiling(sizeF.Height));
            int posX = (int)Math.Round((resolutionX - textWidth) / 2f);

            BuildSyllablePixelMap(merged, font, g, displayText, textWidth);

            Color baseCol = ColorTranslator.FromHtml(baseColor);
            Color highlightCol = ColorTranslator.FromHtml(highlightColor);
            Color strokeCol = ColorTranslator.FromHtml(strokeColor);

            var baseBmp = new Bitmap(textWidth, textHeight);
            var hiBmp = new Bitmap(textWidth, textHeight);
            var multiplier = do4KResolution ? 2 : 1;

            using (var gBase = Graphics.FromImage(baseBmp))
            {
                gBase.Clear(Color.Transparent);
                ApplyTextRenderingSettings(gBase);
                DrawTextWithStroke(gBase, displayText, font, new Point(0, 0), baseCol, strokeCol, 3 * multiplier);
            }

            using (var gHi = Graphics.FromImage(hiBmp))
            {
                gHi.Clear(Color.Transparent);
                ApplyTextRenderingSettings(gHi);
                DrawTextWithStroke(gHi, displayText, font, new Point(0, 0), highlightCol, strokeCol, 3 * multiplier);
            }

            prepared.SyllableMasks = BuildSyllableMasks(
                merged,
                displayText,
                font,
                textWidth,
                textHeight,
                highlightCol,
                strokeCol
            );

            prepared.Text = displayText;
            prepared.Syllables = merged;
            prepared.TextWidth = textWidth;
            prepared.TextHeight = textHeight;
            prepared.PosX = posX;
            prepared.BaseBitmap = baseBmp;
            prepared.HighlightBitmap = hiBmp;

            return prepared;
        }

        private List<PreparedSyllableMask> BuildSyllableMasks(
            List<MergedSyllable> syllables,
            string displayText,
            Font font,
            int textWidth,
            int textHeight,
            Color highlightColor,
            Color strokeColor)
        {
            var result = new List<PreparedSyllableMask>();

            if (syllables == null || syllables.Count == 0 || string.IsNullOrWhiteSpace(displayText))
                return result;

            int searchIndex = 0;
            Bitmap previousPrefix = new Bitmap(textWidth, textHeight, PixelFormat.Format32bppArgb);

            try
            {
                using (Graphics gPrev = Graphics.FromImage(previousPrefix))
                {
                    gPrev.Clear(Color.Transparent);
                }

                foreach (var syllable in syllables)
                {
                    string visible = GetVisibleTextForSyllable(syllable);

                    if (string.IsNullOrWhiteSpace(visible))
                    {
                        result.Add(new PreparedSyllableMask
                        {
                            Start = syllable.Start,
                            End = syllable.End
                        });
                        continue;
                    }

                    int idx = displayText.IndexOf(visible, searchIndex, StringComparison.Ordinal);
                    if (idx < 0)
                    {
                        result.Add(new PreparedSyllableMask
                        {
                            Start = syllable.Start,
                            End = syllable.End
                        });
                        continue;
                    }

                    int endIdx = idx + visible.Length;
                    string prefixText = displayText.Substring(0, endIdx);

                    Bitmap currentPrefix = new Bitmap(textWidth, textHeight, PixelFormat.Format32bppArgb);
                    try
                    {
                        using (Graphics gCurr = Graphics.FromImage(currentPrefix))
                        {
                            gCurr.Clear(Color.Transparent);
                            ApplyTextRenderingSettings(gCurr);
                            DrawTextWithStroke(gCurr, prefixText, font, new Point(0, 0), highlightColor, strokeColor, 3);
                        }

                        PreparedSyllableMask mask = ExtractDeltaMask(
                            previousPrefix,
                            currentPrefix,
                            syllable.Start,
                            syllable.End
                        );

                        result.Add(mask);

                        Bitmap oldPrev = previousPrefix;
                        previousPrefix = currentPrefix;
                        currentPrefix = null;
                        oldPrev.Dispose();
                    }
                    finally
                    {
                        if (currentPrefix != null)
                            currentPrefix.Dispose();
                    }

                    searchIndex = endIdx;
                }
            }
            finally
            {
                if (previousPrefix != null)
                    previousPrefix.Dispose();
            }

            return result;
        }

        private PreparedSyllableMask ExtractDeltaMask(
            Bitmap previousPrefix,
            Bitmap currentPrefix,
            double start,
            double end)
        {
            var mask = new PreparedSyllableMask
            {
                Start = start,
                End = end
            };

            Rectangle rect = new Rectangle(0, 0, currentPrefix.Width, currentPrefix.Height);

            BitmapData prevData = previousPrefix.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData currData = currentPrefix.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int minX = currentPrefix.Width;
            int minY = currentPrefix.Height;
            int maxX = -1;
            int maxY = -1;

            try
            {
                int stride = currData.Stride;
                int height = currentPrefix.Height;
                int width = currentPrefix.Width;

                unsafe
                {
                    byte* prevPtr = (byte*)prevData.Scan0;
                    byte* currPtr = (byte*)currData.Scan0;

                    for (int y = 0; y < height; y++)
                    {
                        byte* prevRow = prevPtr + (y * stride);
                        byte* currRow = currPtr + (y * stride);

                        for (int x = 0; x < width; x++)
                        {
                            int offset = x * 4;

                            byte prevA = prevRow[offset + 3];
                            byte currA = currRow[offset + 3];

                            if (currA > prevA)
                            {
                                if (x < minX) minX = x;
                                if (y < minY) minY = y;
                                if (x > maxX) maxX = x;
                                if (y > maxY) maxY = y;
                            }
                        }
                    }

                    if (maxX < minX || maxY < minY)
                    {
                        return mask;
                    }

                    int outW = maxX - minX + 1;
                    int outH = maxY - minY + 1;

                    Bitmap cropped = new Bitmap(outW, outH, PixelFormat.Format32bppArgb);
                    BitmapData outData = cropped.LockBits(
                        new Rectangle(0, 0, outW, outH),
                        ImageLockMode.WriteOnly,
                        PixelFormat.Format32bppArgb);

                    try
                    {
                        int outStride = outData.Stride;
                        byte* outPtr = (byte*)outData.Scan0;

                        for (int y = minY; y <= maxY; y++)
                        {
                            byte* prevRow = prevPtr + (y * stride);
                            byte* currRow = currPtr + (y * stride);
                            byte* outRow = outPtr + ((y - minY) * outStride);

                            for (int x = minX; x <= maxX; x++)
                            {
                                int srcOffset = x * 4;
                                int dstOffset = (x - minX) * 4;

                                byte prevA = prevRow[srcOffset + 3];
                                byte currA = currRow[srcOffset + 3];

                                int diffA = currA - prevA;
                                if (diffA < 0) diffA = 0;

                                if (diffA > 0)
                                {
                                    outRow[dstOffset + 0] = currRow[srcOffset + 0]; // B
                                    outRow[dstOffset + 1] = currRow[srcOffset + 1]; // G
                                    outRow[dstOffset + 2] = currRow[srcOffset + 2]; // R
                                    outRow[dstOffset + 3] = (byte)diffA;            // A
                                }
                            }
                        }
                    }
                    finally
                    {
                        cropped.UnlockBits(outData);
                    }

                    mask.X = minX;
                    mask.Y = minY;
                    mask.Width = outW;
                    mask.Height = outH;
                    mask.Bitmap = cropped;
                }
            }
            finally
            {
                previousPrefix.UnlockBits(prevData);
                currentPrefix.UnlockBits(currData);
            }

            return mask;
        }

        public class PreparedLine : IDisposable
        {
            public string Text { get; set; } = "";
            public List<MergedSyllable> Syllables { get; set; } = new List<MergedSyllable>();

            public int TextWidth { get; set; }
            public int TextHeight { get; set; }
            public int PosX { get; set; }

            public Bitmap BaseBitmap { get; set; }
            public Bitmap HighlightBitmap { get; set; }

            public List<PreparedSyllableMask> SyllableMasks { get; set; } = new List<PreparedSyllableMask>();

            public bool HasText
            {
                get { return !string.IsNullOrWhiteSpace(Text) && Syllables.Count > 0; }
            }

            public void Dispose()
            {
                if (BaseBitmap != null) BaseBitmap.Dispose();
                if (HighlightBitmap != null) HighlightBitmap.Dispose();

                if (SyllableMasks != null)
                {
                    foreach (var mask in SyllableMasks)
                    {
                        if (mask != null) mask.Dispose();
                    }
                    SyllableMasks.Clear();
                }
            }
        }

        public class PreparedPhrase : IDisposable
        {
            public double PhraseStart { get; set; }
            public double PhraseEnd { get; set; }

            public double FirstLyricTime { get; set; }
            public double LastLyricTime { get; set; }

            public float FontSize { get; set; }

            public PreparedLine Line1 { get; set; } = new PreparedLine();
            public PreparedLine Line2 { get; set; } = new PreparedLine();

            public string PhraseText { get; set; } = "";

            public bool HasAnyText => Line1.HasText || Line2.HasText;

            public void Dispose()
            {
                Line1.Dispose();
                Line2.Dispose();
            }
        }

        public class PreparedTrack
        {
            public List<PreparedPhrase> Phrases { get; } = new List<PreparedPhrase>();
        }

        static void BitmapToBGRA32IntoBuffer(Bitmap bmp, byte[] buffer)
        {
            if (bmp == null || buffer == null)
                throw new ArgumentNullException("Bitmap or buffer is null");

            if (bmp.Width == 0 || bmp.Height == 0)
                throw new ArgumentException("Bitmap has invalid dimensions");

            var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            var bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            try
            {
                int byteCount = Math.Abs(bmpData.Stride) * bmp.Height;

                if (byteCount != buffer.Length)
                    throw new ArgumentException($"Buffer size mismatch. Expected {byteCount}, got {buffer.Length}");

                Marshal.Copy(bmpData.Scan0, buffer, 0, byteCount);
            }
            finally
            {
                bmp.UnlockBits(bmpData);
            }
        }       

        public float GetScaledFontSize(Graphics g, string line, Font preferedFont, float maxSize, int frameWidth)
        {            
                if (string.IsNullOrEmpty(line))
                return preferedFont.Size; // Avoid divide-by-zero or nonsense scaling

            double maxWidth = frameWidth * 0.85;
            SizeF measuredSize = g.MeasureString(line, preferedFont);

            if (measuredSize.Width <= 0)
                return preferedFont.Size; // Fallback to preferred if invalid

            double scaleRatio = maxWidth / measuredSize.Width;
            double scaledSize = preferedFont.Size * scaleRatio;

            // Clamp to a reasonable range
            const float absoluteMax = 256f;
            if (scaledSize > maxSize)
                return Math.Min(maxSize, absoluteMax);
            if (scaledSize < 4f)
                return 4f; // Prevent unreadably small fonts

            return (float)scaledSize;
        }

        private void chkCDG_CheckedChanged(object sender, EventArgs e)
        {
            ModeSanityCheck();
        }

        private void ModeSanityCheck()
        {
            if (!chkCDG.Checked && !chkMP4.Checked)
            {
                MessageBox.Show("You must select at least one rendering format", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                chkMP4.Checked = true;
            }
            staticImageBackgroundToolStripMenuItem.Enabled = chkMP4.Checked;
            solidColorToolStripMenuItem.Enabled = chkMP4.Checked;
            animatedBackgroundToolStripMenuItem.Enabled = chkMP4.Checked;
            mP4FrameRateToolStripMenuItem.Enabled = chkMP4.Checked;
            mP4ResolutionToolStripMenuItem.Enabled = chkMP4.Checked;
            mP4BackgroundToolStripMenuItem.Enabled = chkMP4.Checked;
            highlightDelayToolStripMenuItem.Enabled = chkMP4.Checked;
            keeptomlFileToolStripMenuItem.Enabled = chkCDG.Checked;
            cDGMP3ModeToolStripMenuItem.Enabled = chkCDG.Checked;
            if (!chkCDG.Checked)
            {
                keeptomlFileToolStripMenuItem.Checked = false;
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var message = Tools.ReadHelpFile("cnv");
            var helper = new HelpForm(Text + " - Help", message, false, false);
            helper.ShowDialog();
        }

        private void HighRes_Click(object sender, EventArgs e)
        {
            HighRes.Checked = true;
            HigherRes.Checked = false;
        }

        private void HigherRes_Click(object sender, EventArgs e)
        {
            HigherRes.Checked = true;
            HighRes.Checked = false;
        }

        private void lblBackgroundQuestion_MouseClick(object sender, MouseEventArgs e)
        {
            const string message = "This is the background color used when not using Background Image or Background Video";
            MessageBox.Show(message, "Background", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void lblPartsQuestion_MouseClick(object sender, MouseEventArgs e)
        {
            const string message = "This determines how many parts will be drawn in the karaoke file\n\nThe converter will try to draw as many parts as you" +
                " selected, but will default to the one below that if not available (i.e. if you selected three parts but the song only has two parts," +
                " it will display two parts, and so on) - this means you can batch convert hundreds of songs by selecting three part harmonies and it will" +
                " work even with songs that have two part harmonies or only solo vocals\n\nSome songs may be too fast for the CDG+MP3 format to render " +
                "three parts - I recommend you use two parts or Solo Vocals for those songs - there should be no performance limitation when using the MP4 format";
            MessageBox.Show(message, "Vocal Parts", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void label6_MouseClick(object sender, MouseEventArgs e)
        {
            const string message = "The converter has two formats it works with, and you can select either or both\n\n" +
                "The original format I started to do this for is CDG+MP3 - this is the old karaoke format that you see in bars and pubs\n\n" +
                "This is made possible by using Josiah Winslow's CDGMaker python application wrapped into an executable after several modifications" +
                " by me\nThis system is limited in what it can do because CDG is an old system with low resolution and limited color palette - " +
                "experiment with it and see what you can do - it particularly struggles with fast songs like 'Weird Al' Yankovich's Polka Power! " +
                "with three part harmonies - I recommend you do Solo Vocals for those types of songs if you're using the CDG+MP3 format\n\n" +
                "On the other hand, the MP4 format is designed and implemented entirely by me, and it is available in 1080P and 4K resolutions" +
                " with no limitations on song speed or harmony parts beyond what Rock Band supports - this is what you would use if you're uploading " +
                "to YouTube or playing at a bar/pub that has a modern karaoke DJ system\n\nI tried to make both formats customizable so you can " +
                "generate karaoke videos in your own style";
            MessageBox.Show(message, "Format", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void lblMultitrackQuestion_MouseClick(object sender, MouseEventArgs e)
        {
            const string message = "The converter is able to mute/remove the vocal track for a true karaoke experience but only when the song " +
                "already has karaoke or multitrack audio with separated vocals\n\nThe converter cannot mute/remove vocals in a song with a single " +
                "backing track";
            MessageBox.Show(message, "Audio", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void exportLogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.ExportLog(Text, lstLog.Items);
        }

        private void clearLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogDefaults();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            cancelProcess = true;
            Log("User cancelled process ... stopping as soon as possible");
        }

        private void quarterSecondDelay_Click(object sender, EventArgs e)
        {
            CheckUncheckDelay(sender);
        }

        private void CheckUncheckDelay(object sender)
        {
            noDelayToolStripMenuItem.Checked = false;
            quarterSecondDelay.Checked = false;
            halfSecondDelay.Checked = false;
            threeQuarterSecondDelay.Checked = false;
            wholeSecondDelay.Checked = false;
            OneTwentyFiveDelay.Checked = false;
            OneFiftyDelay.Checked = false;
            OneSeventyFiveDelay.Checked = false;
            TwoSecondDelay.Checked = false;

            ((ToolStripMenuItem)(sender)).Checked = true;
        }

        private void halfSecondDelay_Click(object sender, EventArgs e)
        {
            CheckUncheckDelay(sender);
        }

        private void threeQuarterSecondDelay_Click(object sender, EventArgs e)
        {
            CheckUncheckDelay(sender);
        }

        private void wholeSecondDelay_Click(object sender, EventArgs e)
        {
            CheckUncheckDelay(sender);
        }

        private void loadingBar3Secs_Click(object sender, EventArgs e)
        {
            loadingBar3Secs.Checked = true;
            loadingBar5Secs.Checked = false;
            loadingBar10Secs.Checked = false;
        }

        private void loadingBar5Secs_Click(object sender, EventArgs e)
        {
            loadingBar3Secs.Checked = false;
            loadingBar5Secs.Checked = true;
            loadingBar10Secs.Checked = false;
        }

        private void loadingBar10Secs_Click(object sender, EventArgs e)
        {
            loadingBar3Secs.Checked = false;
            loadingBar5Secs.Checked = false;
            loadingBar10Secs.Checked = true;
        }

        private void enableLoadingBar_Click(object sender, EventArgs e)
        {
            enableAnimatedNotes.Enabled = enableLoadingBar.Checked;
            loadingBarThresholdToolStripMenuItem.Enabled = enableLoadingBar.Checked;
        }

        private void pageMode_Click(object sender, EventArgs e)
        {
            if (radioHarm2.Checked || radioHarm3.Checked)
            {
                MessageBox.Show("Page clearing mode is only available in Solo Vocals mode", "Unavailable", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                pageMode.Checked = false;
                delayedMode.Checked = true;
                eagerMode.Checked = false;
                return;
            }
            pageMode.Checked = true;
            delayedMode.Checked = false;
            eagerMode.Checked = false;
        }

        private void delayedMode_Click(object sender, EventArgs e)
        {
            pageMode.Checked = false;
            delayedMode.Checked = true;
            eagerMode.Checked = false;
        }

        private void eagerMode_Click(object sender, EventArgs e)
        {
            pageMode.Checked = false;
            delayedMode.Checked = false;
            eagerMode.Checked = true;
        }

        private void radioHarm2_CheckedChanged(object sender, EventArgs e)
        {
            if ((radioHarm2.Checked || radioHarm3.Checked) && pageMode.Checked)
            {
                pageMode.Checked = false;
                delayedMode.Checked = true;
            }
        }

        public static List<MergedSyllable> MergeSustainedSyllables(List<KaraokeLyric> input)
        {
            var merged = new List<MergedSyllable>();
            int i = 0;

            while (i < input.Count)
            {
                KaraokeLyric s = input[i];
                string text = s.Lyric;
                double start = s.Start;
                double end = s.End;

                // Handle prefix with dash (e.g., "ma-")
                if (text.EndsWith("-"))
                {
                    text = text.Substring(0, text.Length - 1);
                    i++;

                    // Merge any sustain "+" symbols
                    while (i < input.Count && input[i].Lyric == "+")
                    {
                        end = input[i].End;
                        i++;
                    }

                    // Merge the next syllable if it exists and isn't "+"
                    if (i < input.Count && input[i].Lyric != "+" && input[i].Lyric != "-")
                    {
                        text += input[i].Lyric;
                        end = input[i].End;
                        i++;
                    }

                    merged.Add(new MergedSyllable
                    {
                        Lyric = text,
                        Start = start,
                        End = end
                    });
                }
                else if (text == "+" || text == "-")
                {
                    // Ignore standalone + or - symbols
                    i++;
                }
                else
                {
                    // Normal case: single syllable
                    i++;

                    // Extend end time with any consecutive "+"
                    while (i < input.Count && input[i].Lyric == "+")
                    {
                        end = input[i].End;
                        i++;
                    }

                    merged.Add(new MergedSyllable
                    {
                        Lyric = text,
                        Start = start,
                        End = end
                    });
                }
            }

            return merged;
        }

        public class PreparedSyllableMask : IDisposable
        {
            public double Start { get; set; }
            public double End { get; set; }

            public int X { get; set; }
            public int Y { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }

            public Bitmap Bitmap { get; set; }

            public bool HasPixels
            {
                get { return Bitmap != null && Width > 0 && Height > 0; }
            }

            public void Dispose()
            {
                if (Bitmap != null)
                {
                    Bitmap.Dispose();
                    Bitmap = null;
                }
            }
        }

        public class MergedSyllable
        {
            public string Lyric { get; set; }
            public double Start { get; set; }
            public double End { get; set; }

            // Used for drawing:
            public float Width { get; set; }

            // Optional: where this syllable starts, in pixels, from the left of the line
            public float OffsetX { get; set; }
        }               

        public List<MergedSyllable> BuildSyllablePixelMap(
            List<MergedSyllable> syllables,
            Font font,
            Graphics g,
            string displayText,
            float totalTextWidth)
        {
            ApplyTextRenderingSettings(g);

            int searchIndex = 0;
            float prevPrefixWidth = 0f;

            foreach (var syllable in syllables)
            {
                string visible = GetVisibleTextForSyllable(syllable);

                if (string.IsNullOrWhiteSpace(visible))
                {
                    syllable.Width = 0f;
                    continue;
                }

                // Find this syllable's visible text in the final display string,
                // starting from where the last one left off.
                int idx = displayText.IndexOf(visible, searchIndex, StringComparison.Ordinal);
                if (idx < 0)
                {
                    // Fallback: if we can’t find it (weird cleaning / markers),
                    // at least measure it in isolation so we don't crash.
                    SizeF sizeFallback = g.MeasureString(visible, font);
                    syllable.Width = sizeFallback.Width;
                    continue;
                }

                int endIdx = idx + visible.Length;

                // Measure prefix up to the end of this syllable in the final string
                string prefixText = displayText.Substring(0, endIdx);
                SizeF prefixSize = g.MeasureString(prefixText, font);
                float prefixWidth = prefixSize.Width;

                syllable.Width = prefixWidth - prevPrefixWidth;

                prevPrefixWidth = prefixWidth;
                searchIndex = endIdx;
            }            

            // Optional but recommended: normalize widths so they sum exactly
            // to the measured total text width.
            float sumWidths = syllables.Sum(s => s.Width);
            if (sumWidths > 0.1f && Math.Abs(sumWidths - totalTextWidth) > 0.5f)
            {
                float scale = totalTextWidth / sumWidths;
                foreach (var s in syllables)
                {
                    s.Width *= scale;
                }
            }

            return syllables;
        }
                       
        public static float GetHighlightedPixelWidth(List<MergedSyllable> syllables, double currentTime)
        {
            float total = 0f;

            foreach (var s in syllables)
            {
                if (currentTime < s.Start)
                    break;

                if (currentTime >= s.End)
                {
                    total += s.Width;
                }
                else
                {
                    double progress = (currentTime - s.Start) / (s.End - s.Start);
                    progress = MathHelper.Clamp(progress, 0.0, 1.0);
                    total += (float)(progress * s.Width);
                    break;
                }
            }

            return total;
        }

        private void DrawHighlightAnimation(
            Graphics g,
            Font f,
            double lyricStart,
            double leadDuration,
            int x,
            int y,
            Color color,
            double time)
        {
            if (leadDuration <= 0) return;

            double timeUntilLyric = lyricStart - time;
            if (timeUntilLyric < 0) return;
            if (timeUntilLyric > leadDuration) return;

            int multiplier = do4KResolution ? 2 : 1;
            float travelDistance = 200f * multiplier;
            float paddingBeforeLyric = 0f;// 1f * multiplier; // keeps arrow from touching lyric text
            const string cursor = "•";

            // Measure the actual drawn size of the arrow
            SizeF cursorSize = g.MeasureString(cursor, f, PointF.Empty, StringFormat.GenericTypographic);

            // End position: arrow stops just before the lyric start
            float endX = x - cursorSize.Width - paddingBeforeLyric;

            // Start position: farther to the left
            float startX = endX - travelDistance;

            // 0.0 at animation start, 1.0 at lyric start
            double progress = 1.0 - MathHelper.Clamp(timeUntilLyric / leadDuration, 0.0, 1.0);

            float cursorX = startX + (float)((endX - startX) * progress);

            using (var brush = new SolidBrush(color))
            {
                g.DrawString(cursor, f, brush, new PointF(cursorX, y), StringFormat.GenericTypographic);
            }
        }      

        private static void ApplyTextRenderingSettings(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
        }

        private bool HasDisplayableLyrics(string phraseText)
        {
            if (string.IsNullOrWhiteSpace(phraseText))
                return false;

            string text = phraseText.Trim();

            string[] ignoredMarkers =
            {
                "[clap]",
                "[cowbell]",
                "[tambourine]",
                "[percussion]"
            };

            return !ignoredMarkers.Any(m => text.Equals(m, StringComparison.OrdinalIgnoreCase));
        }

        public string ReconstructPhraseTextFromSyllables(List<MergedSyllable> phraseSyllables)
        {
            var words = new List<string>();
            int i = 0;

            while (i < phraseSyllables.Count)
            {
                var syllable = phraseSyllables[i];
                string raw = syllable.Lyric.Trim();
                string clean = CleanSyllable(raw);
                bool endsWithDash = EndsWithDash(raw) || EndsWithDash(clean);

                string word = clean;

                int j = i + 1;
                bool extended = false;

                // Keep attaching to the word as long as we’re in a broken-up word (ending in - or +)
                while (j < phraseSyllables.Count)
                {
                    var next = phraseSyllables[j];
                    string nextRaw = next.Lyric.Trim();
                    string nextClean = CleanSyllable(nextRaw);

                    bool isSustain = nextRaw == "+";
                    bool nextEndsWithDash = EndsWithDash(nextRaw) || EndsWithDash(nextClean);

                    // If the previous ends in dash or this is a sustain, keep appending
                    if (endsWithDash || isSustain)
                    {
                        word += nextClean;
                        endsWithDash = nextEndsWithDash || isSustain;
                        j++;
                        extended = true;
                    }
                    else
                    {
                        break;
                    }
                }

                if (!string.IsNullOrWhiteSpace(word))
                    words.Add(word);

                i = extended ? j : i + 1;
            }

            return string.Join(" ", words).Replace("‿", " ");
        }

        private void DrawTextWithStroke(Graphics g, string text, Font font, Point pos, Color fill, Color stroke, int strokeWidth)
        {
            ApplyTextRenderingSettings(g);

            if (enableMP4Stroke)
            {
                using (var strokeBrush = new SolidBrush(stroke))
                {
                    // Draw radial stroke (fills circle)
                    for (int dx = -strokeWidth; dx <= strokeWidth; dx++)
                    {
                        for (int dy = -strokeWidth; dy <= strokeWidth; dy++)
                        {
                            if (dx * dx + dy * dy <= strokeWidth * strokeWidth)
                            {
                                g.DrawString(text, font, strokeBrush, pos.X + dx, pos.Y + dy);
                            }
                        }
                    }
                }
            }

            // Draw main fill
            using (var fillBrush = new SolidBrush(fill))
            {
                g.DrawString(text, font, fillBrush, pos);
            }
        }

        private string GetVisibleTextForSyllable(MergedSyllable s)
        {
            var raw = s.Lyric?.Trim() ?? string.Empty;
            var clean = CleanSyllable(raw);
            return clean.Replace("‿", " ");
        }

        private void OneTwentyFiveDelay_Click(object sender, EventArgs e)
        {
            CheckUncheckDelay(sender);
        }

        private void OneFiftyDelay_Click(object sender, EventArgs e)
        {
            CheckUncheckDelay(sender);
        }

        private void OneSeventyFiveDelay_Click(object sender, EventArgs e)
        {
            CheckUncheckDelay(sender);
        }

        private void TwoSecondDelay_Click(object sender, EventArgs e)
        {
            CheckUncheckDelay(sender);
        }

        private void FifteenFPS_Click(object sender, EventArgs e)
        {
            FifteenFPS.Checked = true;
            ThirtyFPS.Checked = false;
            SixtyFPS.Checked = false;  
        }

        private void ThirtyFPS_Click(object sender, EventArgs e)
        {
            FifteenFPS.Checked = false;
            ThirtyFPS.Checked = true;
            SixtyFPS.Checked = false;
        }

        private void SixtyFPS_Click(object sender, EventArgs e)
        {
            FifteenFPS.Checked = false;
            ThirtyFPS.Checked = false;
            SixtyFPS.Checked = true;
        }

        private void lblStrokeQuestion_MouseClick(object sender, MouseEventArgs e)
        {
            const string message = "Color of the outline for the words. Black is best.";
            MessageBox.Show(message, "Stroke", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }               

        private void enableCDGStrokeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cboStroke.Enabled = enableCDGStrokeToolStripMenuItem.Checked || enableMP4StrokeToolStripMenuItem.Checked;
        }

        private void solidColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            solidColorToolStripMenuItem.Checked = true;
            animatedBackgroundToolStripMenuItem.Checked = false;
            staticImageBackgroundToolStripMenuItem.Checked = false;

            videoPreview.Visible = false;
            picPreview.Visible = true;

            UpdateTextParents();
        }

        private void staticImageBackgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var background = Application.StartupPath + "\\bin\\images\\background.png";
            if (!File.Exists(background))
            {
                MessageBox.Show("Background image is missing", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                solidColorToolStripMenuItem.Checked = true;
                staticImageBackgroundToolStripMenuItem.Checked = false;
                animatedBackgroundToolStripMenuItem.Checked = false;
                return;
            }

            videoPreview.Visible = false;
            picPreview.Visible = true;
            cboBackground.SelectedIndex = 0;//black

            droppedImage = false;
            picPreview.Image = Image.FromFile(background);
            solidColorToolStripMenuItem.Checked = false;
            animatedBackgroundToolStripMenuItem.Checked = false;
            staticImageBackgroundToolStripMenuItem.Checked = true;

            var bg = Image.FromFile(background);
            picBackground1.Image = bg;
            lblTextHighlight1.Parent = picBackground1;
            lblTextHighlight1.BackColor = Color.Transparent;
            lblTextHighlight1.Location = new Point(0, 43);
            lblTextColor1.Parent = picBackground1;
            lblTextColor1.BackColor = Color.Transparent;
            lblTextColor1.Location = new Point(0, 12);

            picBackground2.Image = bg;
            lblTextHighlight2.Parent = picBackground2;
            lblTextHighlight2.BackColor = Color.Transparent;
            lblTextHighlight2.Location = new Point(0, 43);
            lblTextColor2.Parent = picBackground2;
            lblTextColor2.BackColor = Color.Transparent;
            lblTextColor2.Location = new Point(0, 12);

            picBackground3.Image = bg;
            lblTextHighlight3.Parent = picBackground3;
            lblTextHighlight3.BackColor = Color.Transparent;
            lblTextHighlight3.Location = new Point(0, 43);
            lblTextColor3.Parent = picBackground3;
            lblTextColor3.BackColor = Color.Transparent;
            lblTextColor3.Location = new Point(0, 12);
        }

        private void animatedBackgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var background = Application.StartupPath + "\\bin\\images\\background.mp4";
            if (!File.Exists(background))
            {
                MessageBox.Show("Background video is missing", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                solidColorToolStripMenuItem.Checked = true;
                staticImageBackgroundToolStripMenuItem.Checked = false;
                animatedBackgroundToolStripMenuItem.Checked = false;
                return;                
            }

            DroppedPreviewVideo(background);            
            UpdateTextParents();
        }

        private void UpdateTextParents()
        {         
            picBackground1.Image = null;
            if (animatedBackgroundToolStripMenuItem.Checked)
            {
                picBackground1.BackColor = Color.Black;
            }
            lblTextHighlight1.Parent = picBackground1;
            lblTextHighlight1.BackColor = Color.Transparent;
            lblTextHighlight1.Location = new Point(0, 43);
            lblTextColor1.Parent = picBackground1;
            lblTextColor1.BackColor = Color.Transparent;
            lblTextColor1.Location = new Point(0, 12);

            picBackground2.Image = null;
            if (animatedBackgroundToolStripMenuItem.Checked)
            {
                picBackground2.BackColor = Color.Black;
            }
            lblTextHighlight2.Parent = picBackground2;
            lblTextHighlight2.BackColor = Color.Transparent;
            lblTextHighlight2.Location = new Point(0, 43);
            lblTextColor2.Parent = picBackground2;
            lblTextColor2.BackColor = Color.Transparent;
            lblTextColor2.Location = new Point(0, 12);

            picBackground3.Image = null;
            if (animatedBackgroundToolStripMenuItem.Checked)
            {
                picBackground3.BackColor = Color.Black;
            }
            lblTextHighlight3.Parent = picBackground3;
            lblTextHighlight3.BackColor = Color.Transparent;
            lblTextHighlight3.Location = new Point(0, 43);
            lblTextColor3.Parent = picBackground3;
            lblTextColor3.BackColor = Color.Transparent;
            lblTextColor3.Location = new Point(0, 12);

            picPreview.Image = null;
            picPreview.BackColor = picBackground1.BackColor;
        }

        private void noDelayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckUncheckDelay(sender);
        }
    }    

    public class KaraokeLyric
    {
        public string Lyric { get; set; }
        public double Start { get; set; }
        public double End { get; set; }
        public long Ticks { get; set; }
    }

    public static class MathHelper
    {
        public static float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        public static int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        public static double Clamp(double value, double min, double max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}
