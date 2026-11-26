using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Nautilus.Properties;
using Nautilus.x360;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Mix;
using Un4seen.Bass.Misc;
using Un4seen.Bass.AddOn.Enc;
using Un4seen.Bass.AddOn.Opus;
using Un4seen.Bass.AddOn.EncMp3;
using Encoder = System.Drawing.Imaging.Encoder;
using Un4seen.Bass.AddOn.EncOgg;
using Un4seen.Bass.AddOn.EncOpus;
using Un4seen.Bass.AddOn.EncFlac;
using Nautilus.Texture;
using NautilusFREE;
using Nautilus.LibForge.SongData;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;
using LibForge.Midi;
using MidiCS;
using SCEllSharp.NPDRM;

namespace Nautilus
{
    public partial class Visualizer : Form
    {
        private static int intVocals;
        private string AlbumArt;
        private string imgLink;
        public string sCON;
        private bool reset;
        private PictureBox pictureFrom;
        private int clicks;
        private readonly Color DefaultColorDark = Color.FromArgb(54, 54, 54);
        private readonly Color DefaultColorLight = Color.FromArgb(208, 208, 208);
        private readonly Color ChartRed = Color.FromArgb(255, 0, 0);
        private readonly Color ChartGreen = Color.FromArgb(0, 255, 0);
        private Color songColor;
        private Color artistColor;
        private Color timeColor;
        private Color albumColor;
        private Color yearColor;
        private Color genreColor;
        private bool bRhythm;
        private int mouseX;
        private int mouseY;
        private string author;
        public string origAuthor;
        private Bitmap bitmap;
        private readonly string inputFile = "";
        private Point logoLocation;
        private readonly NemoTools Tools;
        public DTAParser Parser;
        private string Rating;
        private Color RatingColor;
        private bool loading;
        private decimal song1;
        private decimal song2;
        private decimal artist1;
        private decimal artist2;
        public bool UseOverlay;
        private bool isXOnly;
        public string ThemeName;
        private string ActiveFont;
        private string CustomFontName;
        private bool ProKeysEnabled = true;
        private readonly string config;
        private readonly List<string> FilesToDelete;
        private bool PlayDrums;
        private bool PlayBass;
        private bool PlayGuitar;
        private bool PlayKeys;
        private bool PlayVocals;
        private bool PlayCrowd;
        private bool PlayBacking;
        private double PlaybackSeconds;
        private string ImageOut;
        public bool isRunningShortcut;
        private int BassMixer;
        private int BassStream;
        private const int BassBuffer = 1000;
        private const double FadeTime = 3.0;
        public double VolumeLevel = 12.5;
        private string UserProfile = "";
        private float songX = 272f;
        private float songY = 33f;
        private float artistX = 272f;
        private float artistY = 75f;
        private float yearY = 112f;
        private float yearX = 518f;
        private const float albumX = 277f;
        private const float albumY = 112f;
        private const float genreX = 278f;
        private float genreY = 130f;
        private const float TimeLeft = 538f;
        private const float TimeTop = 32f;
        private const float ratingX = 518f;
        private const float ratingY = 130f;
        private Image RESOURCE_PRO_BASS;
        private Image RESOURCE_PRO_GUITAR;
        private Image RESOURCE_PRO_KEYS;
        private Image RESOURCE_DIFF_NOPART;
        private Image RESOURCE_DIFF_0;
        private Image RESOURCE_DIFF_1;
        private Image RESOURCE_DIFF_2;
        private Image RESOURCE_DIFF_3;
        private Image RESOURCE_DIFF_4;
        private Image RESOURCE_DIFF_5;
        private Image RESOURCE_DIFF_6;
        private Image RESOURCE_HARM2;
        private Image RESOURCE_HARM3;
        private Image RESOURCE_2X;
        private Image RESOURCE_ALBUM_ART;
        private Image RESOURCE_THEME;
        private Image RESOURCE_AUTHOR_LOGO;
        private Image RESOURCE_ICON1;
        private Image RESOURCE_ICON2;
        private readonly nTools nautilus3;
        private bool isTBRBDLC;
        private bool isGDRBDLC;
        private int maxHeight = 778;
        private int minHeight = 682;
        private readonly List<int> BassStreams;
        private string[] opusFiles;
        private string[] oggFiles;
        private string[] mp3Files;
        private string[] wavFiles;
        private string[] cltFiles;
        private bool isM4A;
        private bool isOpus;
        private bool isOgg;
        private bool isMP3;
        private bool isWAV;
        private bool isFNF;
        private bool BASS_INIT;
        private bool isRS2014;
        private string GHWTDE_INI_PATH;
        private string GHWTDE_EXT_PATH;
        private string XML_PATH;
        private string XMA_PATH;
        private string XMA_EXT_PATH;
        private bool isBandFuse;
        private bool isYARG;
        private bool isGHWTDE;
       
        private readonly MIDIStuff MIDITools;
        private double songLength;
        private readonly string tempFile;
        private readonly NemoFnFParser fnfParser;
        private string m4aFilePath;
        private bool volSlide;
        private static readonly HttpClient httpClient = new HttpClient();
        private string songArtist;
        private string songAlbum;
        private Image rb3Version;
        private Image expertOnly;
        private Image catEMH;
        private LibVLC _libVLC;
        private MediaPlayer _mediaPlayer;
        private VideoView videoView;
        private bool videoIsFullScreen = false;

        public Visualizer(Color ButtonBackColor, Color ButtonTextColor, string inFile)
        {
            InitializeComponent();
            Core.Initialize();
            this.SetStyle(ControlStyles.UserMouse, true);

            Core.Initialize();

            var options = new[]
            {
            "--vout=d3d11", // Ensure Direct3D 11 is used if available
            "--no-audio", // Disable audio processing
            "--no-sub-autodetect-file", // Skip subtitle loading
            "--no-video-title-show" // Hide overlay text on videos
        };

            _libVLC = new LibVLC(options);
            _mediaPlayer = new MediaPlayer(_libVLC);
            _mediaPlayer.Volume = 0; //always muted                     

            videoView = new VideoView
            {
                Width = 256,
                Height = 256,
                MediaPlayer = _mediaPlayer,
                Dock = DockStyle.Fill
            };

            this.Controls.Add(videoView);

            intVocals = 1;
            inputFile = inFile;
            config = Application.StartupPath + "\\bin\\config\\visualizer.config";
            Tools = new NemoTools();
            nautilus3 = new nTools();
            Parser = new DTAParser();
            MIDITools = new MIDIStuff();
            fnfParser = new NemoFnFParser();
            picLogo.AllowDrop = true;
            FilesToDelete = new List<string>();
            BassStreams = new List<int>();
            opusFiles = new string[20];
            oggFiles = new string[20];
            mp3Files = new string[20];
            wavFiles = new string[20];
            cltFiles = new string[20];
            var author_logo = Application.StartupPath + "\\author.png";
            tempFile = Application.StartupPath + "\\bin\\temp";
            if (File.Exists(author_logo))
            {
                GetLogo(author_logo);
            }            
            var formButtons = new List<Button>{btnLoad,btnClear,btnSave};
            foreach (var button in formButtons)
            {
                button.BackColor = ButtonBackColor;
                button.ForeColor = ButtonTextColor;
                button.FlatAppearance.MouseOverBackColor = button.BackColor == Color.Transparent ? Color.FromArgb(127, Color.AliceBlue.R, Color.AliceBlue.G, Color.AliceBlue.B) : Tools.LightenColor(button.BackColor);
            } 
            loadImages();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x210 && m.WParam.ToInt32() == 513)
            {
                var cursorPosition = videoView.PointToClient(Cursor.Position);

                if (cursorPosition.X >= 0 && cursorPosition.X < videoView.Width &&
                    cursorPosition.Y >= 0 && cursorPosition.Y < videoView.Height)
                {
                    videoIsFullScreen = !videoIsFullScreen;
                    if (videoIsFullScreen)
                    {
                        videoView.Parent = picVisualizer;
                        videoView.Width = picVisualizer.Width;
                        videoView.Height = picVisualizer.Height;
                    }
                    else
                    {
                        videoView.Parent = picAlbumArt;
                        videoView.Width = picAlbumArt.Width;
                        videoView.Height = picAlbumArt.Height;
                    }
                    videoView.BringToFront();
                }
            }

            base.WndProc(ref m);
        }

        private void loadImages()
        {
            try
            {
                picVisualizer.Image = Tools.NemoLoadImage(Application.StartupPath + "\\res\\background.png");
                RESOURCE_PRO_BASS = Tools.NemoLoadImage(Application.StartupPath + "\\res\\pBass.png");
                RESOURCE_PRO_GUITAR = Tools.NemoLoadImage(Application.StartupPath + "\\res\\pGuitar.png");
                RESOURCE_PRO_KEYS = Tools.NemoLoadImage(Application.StartupPath + "\\res\\pKeys.png");
                RESOURCE_DIFF_0 = Tools.NemoLoadImage(Application.StartupPath + "\\res\\diff0.png");
                RESOURCE_DIFF_1 = Tools.NemoLoadImage(Application.StartupPath + "\\res\\diff1.png");
                RESOURCE_DIFF_2 = Tools.NemoLoadImage(Application.StartupPath + "\\res\\diff2.png");
                RESOURCE_DIFF_3 = Tools.NemoLoadImage(Application.StartupPath + "\\res\\diff3.png");
                RESOURCE_DIFF_4 = Tools.NemoLoadImage(Application.StartupPath + "\\res\\diff4.png");
                RESOURCE_DIFF_5 = Tools.NemoLoadImage(Application.StartupPath + "\\res\\diff5.png");
                RESOURCE_DIFF_6 = Tools.NemoLoadImage(Application.StartupPath + "\\res\\diff6.png");
                RESOURCE_DIFF_NOPART = Tools.NemoLoadImage(Application.StartupPath + "\\res\\nopart.png");
                RESOURCE_HARM2 = Tools.NemoLoadImage(Application.StartupPath + "\\res\\harm2.png");
                RESOURCE_HARM3 = Tools.NemoLoadImage(Application.StartupPath + "\\res\\harm3.png");
                RESOURCE_2X = Tools.NemoLoadImage(Application.StartupPath + "\\res\\2x.png");
                picMulti.Image = Tools.NemoLoadImage(Application.StartupPath + "\\res\\multi.png");
                picKaraoke.Image = Tools.NemoLoadImage(Application.StartupPath + "\\res\\karaoke.png");
                picConvert1.Image = Tools.NemoLoadImage(Application.StartupPath + "\\res\\convert.png");
                catEMH = Tools.NemoLoadImage(Application.StartupPath + "\\res\\cat.png");
                expertOnly = Tools.NemoLoadImage(Application.StartupPath + "\\res\\xonly.png");
                rb3Version = Tools.NemoLoadImage(Application.StartupPath + "\\res\\rb3.png");
                picRBass.Image = Tools.NemoLoadImage(Application.StartupPath + "\\res\\rbass.png");
                picRKeys.Image = Tools.NemoLoadImage(Application.StartupPath + "\\res\\rkeys.png");
                picDIYStems.Image = Tools.NemoLoadImage(Application.StartupPath + "\\res\\diy_multitrack.png");
                picPartial.Image = Tools.NemoLoadImage(Application.StartupPath + "\\res\\partial_multitrack.png");
                picUnpitched.Image = Tools.NemoLoadImage(Application.StartupPath + "\\res\\unpitched_vocals.png");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading one or more of the resource images:\n" + ex.Message, "Visualizer", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private bool isFontAvailable(string[] FontNames, bool isMyriad = false)
        {
            try
            {
                using (InstalledFontCollection fonts = new InstalledFontCollection())
                {
                    foreach (var fontName in FontNames)
                    {
                        if (fonts.Families.Any(f => f.Name.Equals(fontName, StringComparison.OrdinalIgnoreCase)))
                        {
                            if (isMyriad) ActiveFont = fontName;
                            return true;
                        }
                    }
                }
            }
            catch { }
            return false;
        }

        public void ReadMidi(string midiFile)
        {
            MIDITools.Initialize(true);
            MIDITools.ReadMIDIFile(midiFile, 0, true);
            try
            {
                if (MIDITools.LyricsVocals.Lyrics.Any() || MIDITools.PhrasesVocals.Phrases.Any())
                {
                    lblLyrics.Visible = true;
                }
                else
                {
                    lblLyrics.Visible = false;
                }
            }
            catch
            {
                lblLyrics.Visible = false;
            }
            if (isXOnly) return;
            if (Tools.DoesMidiHaveEMH(midiFile, ProKeysEnabled)) return;
            if ((picIcon1.Image != null && picIcon2.Image != null) || isXOnly || Tools.MIDI_ERROR_MESSAGE.ToLowerInvariant().Contains("could not load midi file")) return;
            sendIcon(expertOnly);
            if (MessageBox.Show("Marked as expert only based on MIDI file\n\nClick OK to see more details, click Cancel to go back",
                    Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
            {
                MessageBox.Show(Tools.MIDI_ERROR_MESSAGE, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            isXOnly = true;
        }

        private void AdjustImageParent(Control image, Control parent)
        {
            var pos = PointToScreen(image.Location);
            pos = parent.PointToClient(pos);
            image.Parent = parent;
            image.Location = pos;
            image.BackColor = Color.Transparent;
        }

        private void doImages()
        {
            //the following mess of code is necessary to get the transparency to show the image and not the form
            //separated here so I can just hide this whole bunch of crap

            AdjustImageParent(proGuitar, picVisualizer);
            AdjustImageParent(proBass, picVisualizer);
            AdjustImageParent(pic2x, picVisualizer);
            AdjustImageParent(proKeys, picVisualizer);
            AdjustImageParent(picHarm, picVisualizer);
            AdjustImageParent(diffGuitar, picVisualizer);
            AdjustImageParent(diffBass, picVisualizer);
            AdjustImageParent(diffDrums, picVisualizer);
            AdjustImageParent(diffVocals, picVisualizer);
            AdjustImageParent(diffKeys, picVisualizer);
            AdjustImageParent(picIcon1, picVisualizer);
            AdjustImageParent(picIcon2, picVisualizer);
            AdjustImageParent(picAlbumArt, picVisualizer);           
            AdjustImageParent(picWorking, picAlbumArt);
            AdjustImageParent(picLogo, picAlbumArt);

            picSlider.Parent = picLine;
            picSlider.Location = new Point(0,0);

            picWorking.BringToFront();
            picLogo.BringToFront();            
            logoLocation = picLogo.Location;
                        
            videoView.Parent = picAlbumArt;
            videoView.Left = 0;
            videoView.Top = 0;
            videoView.BringToFront();

            loadDefaults();
        }

        private void loadDefaults()
        {
            reset = true;
            videoView.Visible = false;
            _mediaPlayer.Stop();
            _mediaPlayer.Media = null;
            isTBRBDLC = false;
            isGDRBDLC = false;
            isBandFuse = false;
            isGHWTDE = false;
            isRS2014 = false;
            MIDITools.Initialize(false);
            lblLyrics.Invalidate();
            lblLyrics.Visible = false;
            StopPlayback();            
            picPlayPause.Image = Tools.NemoLoadImage(Application.StartupPath + "\\res\\play\\play.png");
            picPlayPause.Tag = "play";
            Height = minHeight;
            picPlayPause.Cursor = Cursors.WaitCursor;
            picStop.Cursor = Cursors.WaitCursor;
            UpdateTime();
            isXOnly = false;
            loading = false;
            txtSong.Visible = true;
            txtSong1.Visible = false;
            txtSong2.Visible = false;
            txtArtist.Visible = true;
            txtArtist1.Visible = false;
            txtArtist2.Visible = false;
            picIcon1.Image = null;
            picIcon2.Image = null;
            txtSong.Text = null;
            txtSong1.Text = null;
            txtSong2.Text = null;
            txtArtist.Text = null;
            txtArtist1.Text = null;
            txtArtist2.Text = null;
            txtTime.Text = null;
            txtAlbum.Text = null;
            txtTrack.Text = null;
            txtYear.Text = null;
            txtYear2.Text = null;
            txtGenre.Text = null;
            txtSubGenre.Text = null;
            chkGenre.Checked = true;
            chkSubGenre.Checked = false;
            SplitJoinArtist.Text = "SPLIT";
            SplitJoinSong.Text = "SPLIT";
            UseOverlay = false;
            diffGuitar.Tag = -1;
            diffBass.Tag = -1;
            diffDrums.Tag = -1;
            diffKeys.Tag = -1;
            diffVocals.Tag = -1;
            proKeys.Tag = 0;
            proGuitar.Tag = 0;
            proBass.Tag = 0;
            pic2x.Tag = 0;
            intVocals = 1;
            toolTip1.SetToolTip(diffGuitar, "No Part");
            toolTip1.SetToolTip(diffBass, "No Part");
            toolTip1.SetToolTip(diffDrums, "No Part");
            toolTip1.SetToolTip(diffVocals, "No Part");
            toolTip1.SetToolTip(diffKeys, "No Part");
            opusFiles = new string[20];
            oggFiles = new string[20];
            foreach (var wav in wavFiles)
            {
                Tools.DeleteFile(wav);
            }
            wavFiles = new string[20];
            mp3Files = new string[20];
            cltFiles = new string[20];
            PlaybackSeconds = 0.00;
            isOpus = false;
            isOgg = false;
            isWAV = false;
            isRS2014 = false;
            isM4A = false;
            RESOURCE_ALBUM_ART = null;
            picAlbumArt.Image = null;
            if (string.IsNullOrWhiteSpace(UserProfile))
            {
                numFontName.Value = 20;
                numFontArtist.Value = 20;
                numFontTime.Value = 18;
                songColor = DefaultColorDark;
                barSong.BackColor = songColor;
                artistColor = DefaultColorLight;
                barArtist.BackColor = artistColor;
                timeColor = DefaultColorDark;
                barTime.BackColor = timeColor;
                albumColor = DefaultColorLight;
                barAlbum.BackColor = albumColor;
                yearColor = DefaultColorLight;
                barYear.BackColor = yearColor;
                genreColor = DefaultColorLight;
                barGenre.BackColor = genreColor;
            }
            else
            {
                loadProfile();
            }
            picVisualizer.Cursor = Cursors.Default;
            Cursor = Cursors.Default;
            songX = 272f;
            songY = 33f;
            yearY = 112f;
            artistX = 272f;
            artistY = 75f;
            genreY = 130f;
            lblTop.Text = "";
            lblBottom.Text = "";
            lblBottom.Cursor = Cursors.Default;
            lblBottom.ForeColor = Color.Black;
            origAuthor = "";
            picWorking.Visible = false;
            songArtist = "";
            songAlbum = "";
            song1 = 0;
            song2 = 0;
            artist1 = 0;
            artist2 = 0;
            cboRating.SelectedIndex = 3;
            bRhythm = false;
            try
            {
                picPlayPause.Image = Tools.NemoLoadImage(Application.StartupPath + "\\res\\play\\play.png");
                picStop.Image = Tools.NemoLoadImage(Application.StartupPath + "\\res\\play\\stop.png");
                picLine.Image = Tools.NemoLoadImage(Application.StartupPath + "\\res\\play\\line.png");
                picSlider.Image = Tools.NemoLoadImage(Application.StartupPath + "\\res\\play\\slider.png");
                picVolume.Image = Tools.NemoLoadImage(Application.StartupPath + "\\res\\play\\speaker.png");
            }
            catch (Exception)
            {
                MessageBox.Show("There was an error loading the playback images, make sure you haven't deleted any files", Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }            
            RESOURCE_ICON1 = null;
            RESOURCE_ICON2 = null;
            Tools.DeleteFile(tempFile);
            videoView.Visible = false;
            picLastFM.Visible = true;
            toolTip1.SetToolTip(lblBottom, "");
            reset = false;
            picVisualizer.Invalidate();
        }

        public void getImage(String file)
        {            
            if (!Directory.Exists(Application.StartupPath + "\\visualizer\\"))
            {
                Directory.CreateDirectory(Application.StartupPath + "\\visualizer\\");
            }
            var ext = "";
            var imgMoved = false;
            try
            {
                //if not passed a string path for the image
                //show dialog box to find one
                if (string.IsNullOrWhiteSpace(file))
                {
                    var openFileDialog1 = new OpenFileDialog
                        {
                            Filter = "Image Files|*.bmp;*.tif;*.dds;*.jpg;*.jpeg;*.gif;*.png;*.png_xbox;*.png_ps3;*.png_ps4;*.png_wii;*.tpl",
                            Title = "Select an album art image",
                            InitialDirectory = Tools.CurrentFolder
                        };
                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        AlbumArt = openFileDialog1.FileName;
                        Tools.CurrentFolder = Path.GetDirectoryName(openFileDialog1.FileName);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    //if file is not blank, then use that for the image
                    ext = Path.GetExtension(file.ToLowerInvariant());
                    var exts = new List<string> { ".jpg", ".bmp", ".tif", ".dds", ".gif", ".tpl", ".png", ".jpeg", ".png_xbox", ".png_ps3", ".png_ps4", ".png_wii" };
                    var isImage = exts.Contains(ext);

                    if (isImage)
                    {
                        AlbumArt = file;
                    }
                    else
                    {
                        return;
                    }
                }
                if (string.IsNullOrWhiteSpace(AlbumArt)) return;
                var newArt = Application.StartupPath + "\\visualizer\\" + Path.GetFileNameWithoutExtension(AlbumArt) + ".png";
                switch (ext)
                {
                    case ".png_ps4":
                        using (var fileStream = new FileStream(AlbumArt, FileMode.Open, FileAccess.Read))
                        {
                            var converter = new TextureConverter();
                            RESOURCE_ALBUM_ART = converter.ToBitmap(TextureReader.ReadStream(fileStream), 0);
                        }
                        imgMoved = true;
                        break;
                    case ".dds":
                    case ".png_ps3":
                    case ".png_xbox":
                        if (Tools.ConvertRBImage(AlbumArt, newArt, "png"))
                        {
                            RESOURCE_ALBUM_ART = Tools.NemoLoadImage(newArt);
                        }
                        imgMoved = true;
                        break;
                    case ".tpl":
                    case ".png_wii":
                        if (Tools.ConvertWiiImage(AlbumArt, newArt, "png"))
                        {
                            RESOURCE_ALBUM_ART = Tools.NemoLoadImage(newArt);
                        }
                        imgMoved = true;
                        break;
                    default:
                        RESOURCE_ALBUM_ART = Tools.NemoLoadImage(AlbumArt);
                        newArt = AlbumArt;
                        break;
                }
                picAlbumArt.Tag = imgMoved? AlbumArt : newArt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error:\n" + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            picVisualizer.Invalidate();
        }
        
        private void ChangeDifficulty_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            var instrument = (Control)sender;
            var currDiff = Tools.GetDiffTag(instrument);
            var popup = new DifficultySelector(Cursor.Position, currDiff);
            popup.ShowDialog();
            instrument.Tag = popup.Difficulty;
            toolTip1.SetToolTip(instrument, popup.Tier);
            popup.Dispose();
            picVisualizer.Invalidate();
        }

        private Image GetDifficultyImage(Control instrument)
        {
           try
            {
                var difficulty = Convert.ToInt16(instrument.Tag);
                switch (difficulty)
                {
                    case 1:
                        return RESOURCE_DIFF_0;
                    case 2:
                        return RESOURCE_DIFF_1;
                    case 3:
                        return RESOURCE_DIFF_2;
                    case 4:
                        return RESOURCE_DIFF_3;
                    case 5:
                        return RESOURCE_DIFF_4;
                    case 6:
                        return RESOURCE_DIFF_5;
                    case 7:
                        return RESOURCE_DIFF_6;
                    default:
                        return RESOURCE_DIFF_NOPART;
                }
            }
            catch (Exception)
            {}
            return RESOURCE_DIFF_NOPART;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Tools.DeleteFolder(Application.StartupPath + "\\visualizer\\", true);
            sCON = "";
            loadDefaults();
            txtSong.Focus();
            picVisualizer.Invalidate();
        }

        private void HandleDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
        }

        private void LoadYARGPS3Folder(string folder)
        {
            var dta = Directory.GetFiles(folder, "songs.dta", SearchOption.AllDirectories);
            var png = Directory.GetFiles(folder, "*.png_xbox", SearchOption.AllDirectories);
            var png_ps3 = Directory.GetFiles(folder, "*.png_ps3", SearchOption.AllDirectories);
            var ymogg = Directory.GetFiles(folder, "*.yarg_mogg", SearchOption.AllDirectories);
            var mogg = Directory.GetFiles(folder, "*.mogg", SearchOption.AllDirectories);
            
            if (dta == null || dta.Count() == 0)
            {
                MessageBox.Show("Assumed YARG/PS3 folder structure but no songs.dta file found\n\nTIP: If you're trying to play a Clone Hero song, drag/drop the song.ini file", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if ((png == null || png.Count() == 0) && (png_ps3 == null || png_ps3.Count() == 0))
            {
                MessageBox.Show("Assumed YARG/PS3 folder structure but no *.png_xbox or *.png_ps3 file found\n\nTIP: If you're trying to play a Clone Hero song, drag/drop the song.ini file", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            if ((ymogg == null || ymogg.Count() == 0) && (mogg == null || mogg.Count() == 0))
            {
                MessageBox.Show("Assumed YARG/PS3 folder structure but no *.yarg_mogg or *.mogg file found\n\nTIP: If you're trying to play a Clone Hero song, drag/drop the song.ini file", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (!Parser.ReadDTA(File.ReadAllBytes(dta[0])) || !Parser.Songs.Any())
            {
                MessageBox.Show("Something went wrong reading that songs.dta file", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (Parser.Songs.Count > 1)
            {
                MessageBox.Show("It looks like this is a pack...\nWhat were you expecting me to do with this?\nTry a single song, please",
                        Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            loadDefaults();
            if (png != null && png.Count() > 0)
            {
                getImage(png[0]);
            }
            else if (png_ps3 != null && png_ps3.Count() > 0)
            {
                getImage(png_ps3[0]);
            }
            loadDTA();
            if (ymogg.Count() > 0)
            {
                if (!nautilus3.DecY(ymogg[0], DecryptMode.ToMemory))
                {
                    MessageBox.Show("Failed to decrypt *.yarg_mogg file, can't play audio", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                isYARG = true;
            }
            else if (mogg.Count() > 0)
            {                
                if (!nautilus3.DecM(File.ReadAllBytes(mogg[0]), false, false, DecryptMode.ToMemory))
                {
                    MessageBox.Show("Failed to decrypt *.mogg file, can't play audio", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                return;
            }            
            var edat = Directory.GetFiles(folder, "*.edat", SearchOption.AllDirectories);
            if (edat.Any())
            {
                var rap = GetRapPath(edat[0], Application.StartupPath + "\\bin\\raps\\");
                if (!string.IsNullOrEmpty(rap)) //retail encryption
                {
                    if (!string.IsNullOrEmpty(rap)) //retail encryption
                    {
                        Tools.DecryptRetailEdat(edat[0], edat[0].Replace(".edat", ""), rap);
                    }
                }
                else //Onyx and Nautilus encryption
                {
                    if (!Tools.DecryptCustomEdat(edat[0], edat[0].Replace(".edat", ""), Tools.GetKLIC(folder))) //might fail if it's a C3 folder???
                    {
                        //try again but with C3 fixed klic
                        Tools.DecryptCustomEdat(edat[0], edat[0].Replace(".edat", ""));
                    }
                }
            }
            var midi = Directory.GetFiles(folder, "*.mid", SearchOption.AllDirectories);
            if (midi.Any())
            {
                ReadMidi(midi[0]);
            }
            //extract audio file for previewing
            Height = maxHeight;
            ProcessAudio();
        }

        string GetRapPath(string edatPath, string rapFolder)
        {
            using (FileStream fs = File.OpenRead(edatPath))
            {
                NPD npd = NPD.ReadNew(fs);

                if (npd.Magic != 0x4E504400)
                    return null;

                string contentId = npd.ContentID.TrimEnd('\0').Trim();

                string rapPath = Path.Combine(
                    rapFolder,
                    contentId + ".rap");

                return File.Exists(rapPath) ? rapPath : null;
            }
        }

        private void PlayFnFSong(string m4a)
        {
            loadDefaults();
            picWorking.Visible = true;
            m4aFilePath = m4a;

            var folder = Path.GetDirectoryName(m4a);
            var ini = Directory.GetFiles(folder, "song.ini");
            var fnf = Directory.GetFiles(folder, "song.fnf");
            if (ini.Count() > 0)
            {
                Parser.ReadINIFile(ini[0]);
                loadDTA();
            }
            else if (fnf.Count() > 0)
            {
                Parser.ReadINIFile(fnf[0]);
                loadDTA();
            }
            else
            {
                MessageBox.Show("Assumed Fortnite Festival structure but did not find metadata. Aborting.", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                picWorking.Visible = false;
                return;
            }

            var artPNG = Directory.GetFiles(folder, "album.png");
            var artJPG = Directory.GetFiles(folder, "album.jpg");
            if (artPNG.Count() > 0)
            {
                getImage(artPNG[0]);
            }
            else if (artJPG.Count() > 0)
            {
                getImage(artJPG[0]);
            }

            InitBass();
            fnfWorker.RunWorkerAsync();
        }

        private async void Visualizer_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[]) e.Data.GetData(DataFormats.FileDrop);
            Tools.CurrentFolder = Path.GetDirectoryName(files[0]);
            await HandleDragDrop(files[0]);
        }

        private async Task HandleDragDrop(string file)
        {
            if (string.IsNullOrEmpty(file)) return;

            bool isFolder = File.GetAttributes(file).HasFlag(FileAttributes.Directory);
            if (isFolder)
            {
                //let's check if it's Fortnite Festival structure with more than just the preview.m4a file
                var m4aFiles = Directory.GetFiles(file, "*.m4a", SearchOption.TopDirectoryOnly);
                foreach (var m4a in m4aFiles)
                {
                    if (!Path.GetFileName(m4a).Equals("preview.m4a"))
                    {
                        PlayFnFSong(m4a);
                        return;
                    }
                    MessageBox.Show("Assumed Fortnite Festival structure but did not find .m4a audio file, aborting.", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                //otherwise assume Yarg/PS3 folder structure
                LoadYARGPS3Folder(file);
                return;
            }

            var ext = Path.GetExtension(file).ToLowerInvariant();
            var exts = new List<string> { ".jpg", ".bmp", ".tif", ".dds", ".gif", ".tpl", ".png", ".jpeg", ".png_xbox", ".png_ps3", ".png_ps4", ".png_wii" };
            var isImage = exts.Contains(ext);

            if (isImage)
            {
                getImage(file);
            }
            else
            {
                try
                {
                    if (VariousFunctions.ReadFileType(file) == XboxFileType.STFS)
                    {
                        var package = new STFSPackage(file);
                        if (!package.ParseSuccess) return;
                        if (package.Header.TitleID == (uint)1514538961)
                        {
                            package.CloseIO();
                            ExtractPowerGig(file);
                            return;
                        }
                        else if (package.Header.TitleID == (uint)1296435155)
                        {
                            package.CloseIO();
                            ExtractBandFuse(file);
                            return;
                        }
                        else if (package.Header.TitleID == 0x41560844)
                        {
                            package.CloseIO();
                            ExtractPlayDJHero(file, false);
                            return;
                        }
                        else if (package.Header.TitleID == 0x4156087F)
                        {
                            package.CloseIO();
                            ExtractPlayDJHero(file, true);
                            return;
                        }
                        else //assume Harmonix game
                        {
                            package.CloseIO();
                            loadDefaults();
                            loadCON(file);
                        }
                    }
                    else if (Path.GetExtension(file.ToLowerInvariant()) == ".pkg")
                    {
                        ExtractPKG(file);
                    }
                    else if (Path.GetExtension(file.ToLowerInvariant()) == ".sng")
                    {
                        await ExtractSNG(file);
                    }
                    else if (Path.GetExtension(file.ToLowerInvariant()) == ".yargsong")
                    {
                        await ExtractYARG(file);
                    }
                    else if (Path.GetExtension(file.ToLowerInvariant()) == ".psarc")
                    {
                        ExtractPsArc(file);
                    }
                    else if (Path.GetExtension(file.ToLowerInvariant()) == ".xml")
                    {
                        ExtractXMA(file);
                    }
                    else if (Path.GetExtension(file.ToLowerInvariant()) == ".fnf")
                    {
                        PlayFNFFolder(file);
                    }
                    else if (Path.GetExtension(file.ToLowerInvariant()) == ".m4a")
                    {
                        PlayFnFSong(file);
                    }
                    else if (Path.GetExtension(file.ToLowerInvariant()) == ".songdta_ps4")
                    {
                        loadPS4Files(file);
                    }
                    else if (Path.GetFileName(file).ToLowerInvariant() == "song.ini")
                    {
                        //check if it's GHWT:DE format
                        if (Directory.Exists(Path.GetDirectoryName(file) + "\\Content\\"))
                        {
                            GHWTDE_INI_PATH = file;
                            PlayGHWTDEFolder();
                        }
                        else
                        {
                            PlayCHFolder(Path.GetDirectoryName(file));
                        }
                    }
                    else if (Path.GetFileName(file).ToLowerInvariant() == "songs.dta")
                    {
                        if (!Parser.ReadDTA(File.ReadAllBytes(file)) || !Parser.Songs.Any())
                        {
                            MessageBox.Show("Something went wrong reading that songs.dta file", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        if (Parser.Songs.Count > 1)
                        {
                            MessageBox.Show("It looks like this is a pack...\nWhat were you expecting me to do with this?\nTry a single song, please",
                                    Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                        loadDefaults();
                        loadDTA();
                    }
                    else
                    {
                        MessageBox.Show("That's not a valid file\nTry again", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("There was an error accessing that file\nThe error says:\n" + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ExtractPlayDJHero(string file, bool doDJHero2)
        {
            loadDefaults();
            picWorking.Visible = true;

            var path = Application.StartupPath + "\\visualizer\\djhero\\";
            Tools.DeleteFolder(path, true);
            Directory.CreateDirectory(path);

            var xPackage = new STFSPackage(file);
            if (!xPackage.ParseSuccess)
            {
                MessageBox.Show("There was an error parsing that file\nTry again", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                picWorking.Visible = false;
                return;
            }

            var trackListing = path + "tracklisting.xml";
            var setListing = path + "setlisting.xml";
            var manifest = path + "manifest.xml";
            var img = path + "albumart.img";
            var far = path + "albumart.far";
            var fsb = path + "audio.fsb";
            var xmaPath = path + "\\audio\\";
            string artPath = "";
            string dlcFolder = "";
            List<DJHeroTrack> tracks = new List<DJHeroTrack>();
            var album = xPackage.Header.Title_Display;

            try
            {
                //extract manifest file first
                var xFile = xPackage.GetFile("DLC.MANIFEST");
                if (xFile != null)
                {
                    if (!xFile.ExtractToFile(manifest))
                    {
                        MessageBox.Show("Error extracting the Manifest file\nCan't play that file", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        xPackage.CloseIO();
                        picWorking.Visible = false;
                        return;
                    }
                }

                using (StreamReader reader = new StreamReader(manifest))
                {
                    string pattern = @"<FolderLocation>DLC/([^/]+)/";
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        // Check if the line contains <FolderLocation>
                        if (line.Contains("<FolderLocation>"))
                        {
                            // Match the line with the regex pattern
                            Match match = Regex.Match(line, pattern);
                            if (match.Success)
                            {
                                dlcFolder = match.Groups[1].Value; // Group 1 contains the "e000002" part
                                break; // Exit the loop as we've found the value
                            }
                        }
                    }
                }

                //extract set listing XML file (DJ Hero 1)
                if (!doDJHero2)
                {
                    xFile = xPackage.GetFile("DLC/" + dlcFolder + "/SetListing.xml");
                    if (xFile != null)
                    {
                        if (!xFile.ExtractToFile(setListing))
                        {
                            MessageBox.Show("Error extracting the SetListing.xml file", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            using (var reader = new StreamReader(setListing, Encoding.GetEncoding("iso-8859-1")))
                            {
                                XDocument doc = XDocument.Load(reader);

                                // Extract the value of <ArtName> which is the path to the album art file
                                artPath = doc.Descendants("ArtName").FirstOrDefault()?.Value;
                            }
                        }
                    }
                }

                //extract track listing XML file
                xFile = xPackage.GetFile("DLC/" + dlcFolder + "/TrackListing.xml");
                if (xFile != null)
                {
                    if (!xFile.ExtractToFile(trackListing))
                    {
                        MessageBox.Show("Error extracting the TrackListing.xml file\nCan't play that file", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        xPackage.CloseIO();
                        picWorking.Visible = false;
                        return;
                    }
                }
                                
                using (var reader = new StreamReader(trackListing, Encoding.GetEncoding("iso-8859-1")))
                {
                    XDocument doc = XDocument.Load(reader);
                    
                    tracks = doc.Descendants("Track").Select(track => new DJHeroTrack
                    {
                        IDTag = track.Element("IDTag")?.Value,
                        FolderLocation = track.Element("FolderLocation")?.Value,
                        MixArtists = track.Elements("MixArtist")
                                              .Select(artist => artist.Value.Replace("STR_DLC_", "").Replace("STR_", "").Replace("ARTIST_", "").Replace("Artist_", ""))
                                              .ToList(),
                        MixNames = track.Elements("MixName")
                                             .Select(name => name.Value.Replace("STR_DLC_", "").Replace("STR_", "").Replace("TRACK_", "").Replace("Track_", ""))
                                             .ToList()
                    })
                        .Where(item => item.IDTag != null && item.FolderLocation != null)
                        .ToList();
                }

                if (!string.IsNullOrEmpty(artPath) && !doDJHero2)
                {
                    //extract album art IMG file
                    xFile = xPackage.GetFile(artPath + ".img");
                    if (xFile != null)
                    {
                        if (!xFile.ExtractToFile(img))
                        {
                            MessageBox.Show("Error extracting the album art file", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            var djherotools = new DJHeroIMGTools();
                            if (!djherotools.ImgToDDS(img))
                            {
                                MessageBox.Show("Error converting the album art file", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                getImage(img.Replace(".img", ".dds"));
                            }
                        }
                    }
                }                

                var selector = new DJHeroTrackSelector(tracks);
                selector.ShowDialog();
                var trackIndex = selector.SelectedIndex;

                if (trackIndex == -1)
                {
                    MessageBox.Show("Error selecting track to play, can't continue", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    xPackage.CloseIO();
                    picWorking.Visible = false;
                    return;
                }

                /*if (doDJHero2) //apparently this all works, but there's no IMG files in the DLC FAR files, so ignore for now.
                {
                    //extract album art FAR file
                    var x = tracks[trackIndex].FolderLocation.Replace(dlcFolder, dlcFolder + "/TrackPacks") + "/Male.FAR";
                    xFile = xPackage.GetFile(x);
                    if (xFile != null)
                    {
                        if (!xFile.ExtractToFile(far))
                        {
                            MessageBox.Show("Error extracting the album art file", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            // Create an IFile instance from the input file
                            IFile farFile = new LocalFile(null, far);
                            // Check if it's a valid FSAR archive
                            if (FSARPackage.IsFSAR(farFile) != PackageTestResult.YES)
                            {
                                MessageBox.Show("Error extracting the album art file", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                // Load the archive
                                using (FSARPackage fsarPackage = FSARPackage.FromFile(farFile))
                                {
                                    // Retrieve all files from the archive
                                    var allFiles = fsarPackage.GetAllFiles<FSARFile>();

                                    foreach (var f in allFiles)
                                    {
                                        // Get file data
                                        byte[] fileData = f.GetBytes();

                                        // Construct output path
                                        string relativePath = f.Name.Replace('/', Path.DirectorySeparatorChar);
                                        string outputPath = Path.Combine(path, relativePath);

                                        // Ensure the directory exists
                                        string directory = Path.GetDirectoryName(outputPath);
                                        if (!Directory.Exists(directory))
                                        {
                                            Directory.CreateDirectory(directory);
                                        }

                                        // Write the file
                                        File.WriteAllBytes(outputPath, fileData);
                                    }
                                }
                            }
                        }
                    }
                }*/

                //extract audio file
                var audioPath = doDJHero2 ? tracks[trackIndex].FolderLocation + "/DJ.fsb" : tracks[trackIndex].FolderLocation + "/SinglePlayer/Medium/AudioTrack_Main.fsb";
                xFile = xPackage.GetFile(audioPath);
                if (xFile != null)
                {
                    if (!xFile.ExtractToFile(fsb))
                    {
                        MessageBox.Show("Error extracting the audio file, can't continue", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        xPackage.CloseIO();
                        picWorking.Visible = false;
                        return;
                    }
                }
                xPackage.CloseIO();

                if (!Tools.XMASH(fsb))
                {
                    MessageBox.Show("Error extracting XMA files from the FSB container, can't continue", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    picWorking.Visible = false;
                    return;
                }

                var XMAs = Directory.GetFiles(path, "*.xma", SearchOption.TopDirectoryOnly);
                if (XMAs.Count() == 0)
                {
                    MessageBox.Show("Error extracting XMA files from the FSB container, can't continue", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    picWorking.Visible = false;
                    return;
                }

                foreach (var xma in XMAs)
                {
                    if (!Tools.toWAV(xma))
                    {
                        MessageBox.Show("Failed to convert XMA file to WAV - can't play this song", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        Tools.DeleteFile(path + "\\towav.exe");
                        picWorking.Visible = false;
                        return;
                    }
                }
                Tools.DeleteFile(path + "\\xmash.exe");
                Tools.DeleteFile(path + "\\towav.exe");

                foreach (var xma in XMAs)
                {
                    Tools.DeleteFile(xma);
                }

                var WAVs = Directory.GetFiles(path, "*.wav", SearchOption.TopDirectoryOnly);
                foreach (var wav in WAVs)
                {
                    if (wav.Contains("1_1.wav"))
                    {
                        File.Move(wav, path + "vocals.wav");
                    }
                    else if (wav.Contains("1_2.wav"))
                    {
                        File.Move(wav, path + "backing.wav");
                    }
                    else if (wav.Contains("1_3.wav"))
                    {
                        File.Move(wav, path + "crowd.wav");
                    }
                }                               

                wavFiles = Directory.GetFiles(path, "*.wav");
                if (wavFiles.Any())
                {
                    isWAV = true;
                    isMP3 = false;
                    isOgg = false;
                    isOpus = false;
                    isM4A = false;

                    //the metadata doesn't contain song data, let's get it from the mp3 files
                    InitBass();
                    var stream = Bass.BASS_StreamCreateFile(wavFiles[0], 0L, File.ReadAllBytes(wavFiles[0]).Length, BASSFlag.BASS_SAMPLE_FLOAT);
                    var len = Bass.BASS_ChannelGetLength(stream);
                    var totaltime = Bass.BASS_ChannelBytes2Seconds(stream, len); // the total time length
                    Parser.Songs = new List<SongData>
                    {
                        new SongData()
                    };
                    Parser.Songs[0].Album = album;
                    Parser.Songs[0].Artist = string.Join(", ", tracks[trackIndex].MixArtists);
                    Parser.Songs[0].Name = string.Join(", ", tracks[trackIndex].MixNames);
                    Parser.Songs[0].Length = (int)(totaltime * 1000);
                    lblSongLength.Text = Parser.GetSongDuration(Parser.Songs[0].Length.ToString(CultureInfo.InvariantCulture));
                    txtTime.Text = lblSongLength.Text;
                    txtAlbum.Text = Parser.Songs[0].Album;
                    txtArtist.Text = Parser.Songs[0].Artist;
                    txtSong.Text = Parser.Songs[0].Name;


                    Height = maxHeight;
                    picPlayPause.Cursor = Cursors.Hand;
                    picStop.Cursor = Cursors.Hand;
                    StartPlayback();
                }
                else
                {
                    MessageBox.Show("Failed to extract audio files from the XMA file, can't play this song", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    picWorking.Visible = false;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an unexpected error\nThe error says: " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            xPackage.CloseIO();
            picWorking.Visible = false;
        }

        private void ExtractPlayDJHero2(string file)
        {
            loadDefaults();
            picWorking.Visible = true;

            var path = Application.StartupPath + "\\visualizer\\djhero\\";
            Tools.DeleteFolder(path, true);
            Directory.CreateDirectory(path);

            var xPackage = new STFSPackage(file);
            if (!xPackage.ParseSuccess)
            {
                MessageBox.Show("There was an error parsing that file\nTry again", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                picWorking.Visible = false;
                return;
            }

            var xml = path + "setlisting.xml";
            var manifest = path + "manifest.xml";
            var img = path + "albumart.img";
            var fsb = path + "audio.fsb";
            var xmaPath = path + "\\audio\\";
            string dlcFolder = "";
            List<DJHeroTrack> tracks = new List<DJHeroTrack>();
            var album = xPackage.Header.Title_Display;

            try
            {
                //extract manifest file first
                var xFile = xPackage.GetFile("DLC.MANIFEST");
                if (xFile != null)
                {
                    if (!xFile.ExtractToFile(manifest))
                    {
                        MessageBox.Show("Error extracting the Manifest file\nCan't play that file", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        xPackage.CloseIO();
                        picWorking.Visible = false;
                        return;
                    }
                }

                using (StreamReader reader = new StreamReader(manifest))
                {
                    string pattern = @"<FolderLocation>DLC/([^/]+)/";
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        // Check if the line contains <FolderLocation>
                        if (line.Contains("<FolderLocation>"))
                        {
                            // Match the line with the regex pattern
                            Match match = Regex.Match(line, pattern);
                            if (match.Success)
                            {
                                dlcFolder = match.Groups[1].Value; // Group 1 contains the "e000002" part
                                break; // Exit the loop as we've found the value
                            }
                        }
                    }
                }

                //extract set listing XML file
                xFile = xPackage.GetFile("DLC/" + dlcFolder + "/TrackListing.xml");
                if (xFile != null)
                {
                    if (!xFile.ExtractToFile(xml))
                    {
                        MessageBox.Show("Error extracting the TrackListing.xml file\nCan't play that file", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        xPackage.CloseIO();
                        picWorking.Visible = false;
                        return;
                    }
                }

                using (var reader = new StreamReader(xml, Encoding.GetEncoding("iso-8859-1")))
                {
                    // Load the XML file
                    XDocument xmlDoc = XDocument.Load(reader);

                    // Parse and extract data
                    tracks = xmlDoc.Descendants("Track").Select(track => new DJHeroTrack
                    {
                        IDTag = track.Element("IDTag")?.Value,
                        FolderLocation = track.Element("FolderLocation")?.Value,
                        MixArtists = track.Elements("MixArtist")
                                              .Select(artist => artist.Value.Replace("STR_DLC_", string.Empty))
                                              .ToList(),
                        MixNames = track.Elements("MixName")
                                             .Select(name => name.Value.Replace("STR_DLC_", string.Empty))
                                             .ToList()
                    })
                        .Where(item => item.IDTag != null && item.FolderLocation != null)
                        .ToList();
                }

                var selector = new DJHeroTrackSelector(tracks);
                selector.ShowDialog();
                var trackIndex = selector.SelectedIndex;

                if (trackIndex == -1)
                {
                    MessageBox.Show("Error selecting track to play, can't continue", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    xPackage.CloseIO();
                    picWorking.Visible = false;
                    return;
                }

                //extract audio file
                xFile = xPackage.GetFile(tracks[trackIndex].FolderLocation + "/DJ.fsb");
                if (xFile != null)
                {
                    if (!xFile.ExtractToFile(fsb))
                    {
                        MessageBox.Show("Error extracting the audio file, can't continue", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        xPackage.CloseIO();
                        picWorking.Visible = false;
                        return;
                    }
                }
                xPackage.CloseIO();

                if (!Tools.XMASH(fsb))
                {
                    MessageBox.Show("Error extracting XMA files from the FSB container, can't continue", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    picWorking.Visible = false;
                    return;
                }

                var XMAs = Directory.GetFiles(path, "*.xma", SearchOption.TopDirectoryOnly);
                if (XMAs.Count() == 0)
                {
                    MessageBox.Show("Error extracting XMA files from the FSB container, can't continue", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    picWorking.Visible = false;
                    return;
                }

                foreach (var xma in XMAs)
                {
                    if (!Tools.toWAV(xma))
                    {
                        MessageBox.Show("Failed to convert XMA file to WAV - can't play this song", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        Tools.DeleteFile(path + "\\towav.exe");
                        picWorking.Visible = false;
                        return;
                    }
                }
                Tools.DeleteFile(path + "\\xmash.exe");

                foreach (var xma in XMAs)
                {
                    Tools.DeleteFile(xma);
                }

                var WAVs = Directory.GetFiles(path, "*.wav", SearchOption.TopDirectoryOnly);
                foreach (var wav in WAVs)
                {
                    if (wav.Contains("1_1.wav"))
                    {
                        File.Move(wav, path + "vocals.wav");
                    }
                    else if (wav.Contains("1_2.wav"))
                    {
                        File.Move(wav, path + "backing.wav");
                    }
                    else if (wav.Contains("1_3.wav"))
                    {
                        File.Move(wav, path + "crowd.wav");
                    }
                }

                wavFiles = Directory.GetFiles(path, "*.wav");
                if (wavFiles.Any())
                {
                    isWAV = true;
                    isMP3 = false;
                    isOgg = false;
                    isOpus = false;
                    isM4A = false;

                    //the metadata doesn't contain song data, let's get it from the mp3 files
                    InitBass();
                    var stream = Bass.BASS_StreamCreateFile(wavFiles[0], 0L, File.ReadAllBytes(wavFiles[0]).Length, BASSFlag.BASS_SAMPLE_FLOAT);
                    var len = Bass.BASS_ChannelGetLength(stream);
                    var totaltime = Bass.BASS_ChannelBytes2Seconds(stream, len); // the total time length
                    Parser.Songs = new List<SongData>
                    {
                        new SongData()
                    };
                    Parser.Songs[0].Album = album;
                    Parser.Songs[0].Artist = string.Join(", ", tracks[trackIndex].MixArtists);
                    Parser.Songs[0].Name = string.Join(", ", tracks[trackIndex].MixNames);
                    Parser.Songs[0].Length = (int)(totaltime * 1000);
                    lblSongLength.Text = Parser.GetSongDuration(Parser.Songs[0].Length.ToString(CultureInfo.InvariantCulture));
                    txtTime.Text = lblSongLength.Text;
                    txtAlbum.Text = Parser.Songs[0].Album;
                    txtArtist.Text = Parser.Songs[0].Artist;
                    txtSong.Text = Parser.Songs[0].Name;

                    Height = maxHeight;
                    picPlayPause.Cursor = Cursors.Hand;
                    picStop.Cursor = Cursors.Hand;
                    StartPlayback();
                }
                else
                {
                    MessageBox.Show("Failed to extract audio files from the XMA file, can't play this song", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    picWorking.Visible = false;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an unexpected error\nThe error says: " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            xPackage.CloseIO();
            picWorking.Visible = false;
        }
          
        private void loadPS4Files(string file)
        {
            loadDefaults();

            var dtaBytes = File.ReadAllBytes(file);
            using (MemoryStream ms = new MemoryStream(dtaBytes))
            {
                var ps4Data = new SongDataReader(ms);
                var songData = ps4Data.Read();

                Parser = new DTAParser();
                Parser.Songs = new List<SongData>();
                var song = new SongData();
                song.SongId = (int)songData.SongId;
                song.GameVersion = songData.Version;
                song.PreviewStart = (int)songData.PreviewStart;
                song.PreviewEnd = (int)songData.PreviewEnd;
                song.Name = songData.Name;
                song.Artist = songData.Artist;
                song.Album = songData.AlbumName;
                song.YearReleased = songData.AlbumYear;
                song.TrackNumber = songData.AlbumTrackNumber;
                song.Genre = Parser.doGenre(songData.Genre, false);
                song.RawGenre = songData.Genre;
                song.Length = (int)songData.SongLength;
                song.GuitarDiff = Parser.GuitarDiff((int)songData.GuitarRank);
                song.BassDiff = Parser.BassDiff((int)songData.BassRank);
                song.DrumsDiff = Parser.DrumDiff((int)songData.DrumRank);
                song.VocalsDiff = Parser.VocalsDiff((int)songData.VocalsRank);
                song.BandDiff = Parser.BandDiff((int)songData.BandRank);
                song.KeysDiff = Parser.KeysDiff((int)songData.KeysRank);
                song.ProKeysDiff = Parser.ProKeysDiff((int)songData.RealKeysRank);
                song.Master = !songData.Cover;
                song.VocalParts = songData.VocalParts;
                song.ShortName = songData.Shortname;
                song.Source = songData.GameOrigin;          
                Parser.Songs.Add(song);
            }

            loadDTA();

            var art = file.Replace(".songdta_ps4", ".png_ps4");
            if (File.Exists(art))
            {
                getImage(art);
            }

            var rb4midi = file.Replace(".songdta_ps4", ".rbmid_ps4");
            var rb3midi = rb4midi.Replace(".rbmid_ps4", ".mid");
            if (File.Exists(rb4midi))
            {
                using (var inStream = File.OpenRead(rb4midi))
                {
                    var rb4mid = RBMidReader.ReadStream(inStream);

                    if (rb4mid != null)
                    {
                        var rb3mid = RBMidConverter.ToMid(rb4mid);

                        if (rb3mid != null)
                        {
                            using (var outStream = File.Create(rb3midi))
                            {
                                MidiFileWriter.WriteSMF(rb3mid, outStream);
                            }
                        }
                    }                    
                }

                if (File.Exists(rb3midi))
                {
                    ReadMidi(rb3midi);
                }
            }

            var mogg = file.Replace(".songdta_ps4", ".mogg");
            var moggDTA = mogg.Replace(".mogg", ".mogg.dta");

            if (!File.Exists(mogg) || !File.Exists(moggDTA)) return;

            var sr = new StreamReader(moggDTA);
            try
            {
                while (sr.Peek() >= 0)
                {
                    var line = sr.ReadLine();
                    if (line.Contains("(tracks"))
                    {
                        var o = 0;
                        var c = 0;
                        while (!string.IsNullOrEmpty(line.Trim()))
                        {
                            line = sr.ReadLine();
                            o += line.Count(a => a == '(');
                            c += line.Count(a => a == ')');
                            if (o == c) break;

                            if (line.ToLowerInvariant().Contains("drum"))
                            {
                                if (!line.Contains(")"))
                                {                                    
                                    line = sr.ReadLine();
                                    if (string.IsNullOrEmpty(line.Trim()))
                                    {
                                        line = sr.ReadLine();
                                    }
                                }
                                Parser.Songs[0].ChannelsDrums += Parser.getChannels(line, "drum");
                                Parser.Songs[0].ChannelsDrumsStart = 0;
                                o += line.Count(a => a == '(');
                                c += line.Count(a => a == ')');
                            }
                            else if (line.ToLowerInvariant().Contains("bass"))
                            {
                                if (!line.Contains(")"))
                                {                                   
                                    line = sr.ReadLine();
                                    if (string.IsNullOrEmpty(line.Trim()))
                                    {
                                        line = sr.ReadLine();
                                    }
                                }
                                Parser.Songs[0].ChannelsBass = Parser.getChannels(line, "bass");
                                Parser.Songs[0].ChannelsBassStart = Parser.getChannelsStart(line, "bass");
                                o += line.Count(a => a == '(');
                                c += line.Count(a => a == ')');
                            }
                            else if (line.ToLowerInvariant().Contains("guitar"))
                            {
                                if (!line.Contains(")"))
                                {
                                    line = sr.ReadLine();
                                    if (string.IsNullOrEmpty(line.Trim()))
                                    {
                                        line = sr.ReadLine();
                                    }
                                }
                                Parser.Songs[0].ChannelsGuitar = Parser.getChannels(line, "guitar");
                                Parser.Songs[0].ChannelsGuitarStart = Parser.getChannelsStart(line, "guitar");
                                o += line.Count(a => a == '(');
                                c += line.Count(a => a == ')');
                            }                            
                            else if (line.ToLowerInvariant().Contains("vocals"))
                            {
                                if (!line.Contains(")"))
                                {
                                    line = sr.ReadLine();
                                    if (string.IsNullOrEmpty(line.Trim()))
                                    {
                                        line = sr.ReadLine();
                                    }
                                }
                                Parser.Songs[0].ChannelsVocals = Parser.getChannels(line, "vocals");
                                Parser.Songs[0].ChannelsVocalsStart = Parser.getChannelsStart(line, "vocals");
                                o += line.Count(a => a == '(');
                                c += line.Count(a => a == ')');
                            }
                            else if (line.ToLowerInvariant().Contains("fake"))
                            {
                                if (!line.Contains(")"))
                                {
                                    line = sr.ReadLine();
                                    if (string.IsNullOrEmpty(line.Trim()))
                                    {
                                        line = sr.ReadLine();
                                    }
                                }                                
                                o += line.Count(a => a == '(');
                                c += line.Count(a => a == ')');
                            }
                        }
                    }
                    else if (line.Contains("pans"))
                    {
                        if (!line.Contains(")"))
                        {
                            line = sr.ReadLine();
                        }
                        Parser.Songs[0].PanningValues = line.Replace("(", "").Replace(")", "").Replace("'", "").Replace("pans", "");
                    }
                    else if (line.Contains("vols"))
                    {
                        if (!line.Contains(")"))
                        {
                            line = sr.ReadLine();
                        }
                        Parser.Songs[0].AttenuationValues = line.Replace("(", "").Replace(")", "").Replace("'", "").Replace("vols", "");
                        Parser.Songs[0].OriginalAttenuationValues = Parser.Songs[0].AttenuationValues;
                    }                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error processing that .mogg.dta file:\n\n" + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                sr.Dispose();
                return;
            }
            sr.Dispose();

            if (!nautilus3.DecM(File.ReadAllBytes(mogg), false, false, DecryptMode.ToMemory)) return;
            Height = maxHeight;
            picPlayPause.Cursor = Cursors.Hand;
            picStop.Cursor = Cursors.Hand;
            updatePlaybackInstruments();
            StartPlayback();
        }

        private void ExtractBandFuse(string file)
        {
            loadDefaults();

            var visualizer = Application.StartupPath + "\\visualizer\\";
            Tools.DeleteFolder(visualizer, true);//clean up before starting with new song
            Directory.CreateDirectory(visualizer);
            var extracted_path = visualizer + "temp\\";
            if (!Directory.Exists(extracted_path))
            {
                Directory.CreateDirectory(extracted_path);
            }
           
            var songFuse = Application.StartupPath + "\\bin\\songfuse.exe";
            if (!File.Exists(songFuse))
            {
                MessageBox.Show("Could not find songfuse.exe in the \\bin\\ folder, can't continue", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            var package = new STFSPackage(file);            
            if (!package.ExtractPayload(extracted_path, true, false))
            {
                MessageBox.Show("Failed to extract file contents, can't play this song", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                package.CloseIO();
                return;
            }
            var header = package.Header.Description;
            package.CloseIO();
           
            isBandFuse = true;
            var index = header.IndexOf(" - ");

            Parser = new DTAParser();
            SongData song = new SongData();
            song.Name = header.Substring(0, index).Trim();
            song.Artist = header.Substring(index + 3, header.Length - (index + 3)).Trim();
            song.ChartAuthor = "BandFuse";
            txtArtist.Text = song.Artist;
            songArtist = song.Artist;
            txtSong.Text = song.Name;
            Parser.Songs = new List<SongData> { song };
            
            var art = extracted_path + "album.png";
            var xpr = Directory.GetFiles(extracted_path, "*.xpr", SearchOption.AllDirectories);
            if (xpr.Count() > 0)
            {
                if (!Tools.ConvertBandFuse("texture", xpr[0], art))
                {
                    MessageBox.Show("Failed to convert album art texture", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    getImage(art);
                }
            }

            cltFiles = Directory.GetFiles(extracted_path, "*.clt", SearchOption.AllDirectories);
            if (!cltFiles.Any())
            {
                MessageBox.Show("No audio files found to decrypt, can't play this song", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                isBandFuse = false;
                return;
            }

            picWorking.Visible = true;
            bandFuseWorker.RunWorkerAsync();
        }

        private void PlayFNFFolder(string filename)
        {
            loadDefaults();
            Parser.ReadFNFFile(filename);
            loadDTA();

            var default_art = Application.StartupPath + "\\res\\fnf.png";
            var albumPNG = Path.GetDirectoryName(filename) + "\\album.png";
            var albumJPG = Path.GetDirectoryName(filename) + "\\album.jpg";
            if (File.Exists(albumPNG))
            {
                getImage(albumPNG);
            }
            else if (File.Exists(albumJPG))
            {
                getImage(albumJPG);
            }
            else if (File.Exists(default_art))
            {
                getImage(default_art);
            }
            Invalidate();

            opusFiles = Directory.GetFiles(Path.GetDirectoryName(filename), "*.opus", SearchOption.TopDirectoryOnly);
            oggFiles = Directory.GetFiles(Path.GetDirectoryName(filename), "*.ogg", SearchOption.TopDirectoryOnly);
            if (!opusFiles.Any() && !oggFiles.Any())
            {
                MessageBox.Show("Did not find any audio files in that folder, can't play that song", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            isFNF = true;
            isMP3 = false;
            isOpus = opusFiles.Any();
            isWAV = false;
            isOgg = oggFiles.Any() && !isOpus;
            isM4A = false;
            Height = maxHeight;
            picPlayPause.Cursor = Cursors.Hand;
            picStop.Cursor = Cursors.Hand;
            StartPlayback();
        }

        private void ExtractPowerGig(string file)
        {
            loadDefaults();           

            var visualizer = Application.StartupPath + "\\visualizer\\";
            if (!Directory.Exists(visualizer))
            {
                Directory.CreateDirectory(visualizer);
            }
            var extracted_path = visualizer + "temp\\";
            if (!Directory.Exists(extracted_path))
            {
                Directory.CreateDirectory(extracted_path);
            }
            var onyx_dir = Application.StartupPath + "\\bin\\onyx\\";
            if (!Directory.Exists(onyx_dir))
            {
                Directory.CreateDirectory(onyx_dir);
                if (MessageBox.Show("These Power Gig files are encrypted!\nIn order to extract and decrypt the contents we need to rely on the more powerful tool Onyx\nPlease download 'Onyx CLI' (the command line interface version) and copy the contents into the onyx subfolder I created in the \\bin\\ folder, then click OK to proceed", Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) != DialogResult.OK)
                {
                    return;
                }
            }
            var onyx = onyx_dir + "onyx.exe";
            if (!File.Exists(onyx))
            {
                MessageBox.Show("Could not find Onyx.exe in the \\bin\\onyx\\ folder, can't continue", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            var package = new STFSPackage(file);
            if (!package.ExtractPayload(extracted_path, true, false))
            {
                MessageBox.Show("Failed to extract file contents, can't play this song", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                package.CloseIO();
                return;
            }
            package.CloseIO();

            var root_folder = extracted_path + "Root";
            var data_file = root_folder + "\\Data.hdr.e.2";

            var XML = Tools.ExtractWithOnyx(data_file, root_folder);
            if (string.IsNullOrEmpty(XML))
            {
                MessageBox.Show("Failed to extract and decrypt the contents, can't play this song", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
                        
            //resulting XML file is compressed, need to decompress it
            if (!Tools.DecompressXML(XML))
            {
                MessageBox.Show("Failed to decompress the XML file, can't play this song", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            
            ExtractXMA(XML);
        }        

        private void ExtractXMA(string xml)
        {

            XML_PATH = xml;
            var XMAs = Directory.GetFiles(Path.GetDirectoryName(XML_PATH), "*.xma", SearchOption.TopDirectoryOnly);
            var ogXMA = XMAs[0];//XML_PATH.Replace(".xml", "_all.xma");

            loadDefaults();
            Parser.ReadXMLFile(XML_PATH);
            loadDTA();

            var midi = XML_PATH.Replace(".xml", ".mid");
            if (File.Exists(midi))
            {
                ReadMidi(midi);
            }
            var visualizer = Application.StartupPath + "\\visualizer\\";
            if (!Directory.Exists(visualizer))
            {
                Directory.CreateDirectory(visualizer);
            }
            XMA_EXT_PATH = visualizer + (string.IsNullOrEmpty(Parser.Songs[0].InternalName) ? "temp" : Parser.Songs[0].InternalName);
            if (!Directory.Exists(XMA_EXT_PATH))
            {
                Directory.CreateDirectory(XMA_EXT_PATH);
            }
            XMA_PATH = XMA_EXT_PATH + "\\" + Path.GetFileNameWithoutExtension(XML_PATH) + "_all.xma";

            var default_art = Application.StartupPath + "\\res\\powergig.jpg";
            var albumPNG = Path.GetDirectoryName(XML_PATH) + "\\album.png";
            var albumJPG = Path.GetDirectoryName(XML_PATH) + "\\album.jpg";
            if (File.Exists(albumPNG))
            {
                getImage(albumPNG);
            }
            else if (File.Exists(albumJPG))
            {
                getImage(albumJPG);
            }
            else if (File.Exists(default_art))
            {
                getImage(default_art);
            }
            Invalidate();

            if (!File.Exists(ogXMA))
            {
                MessageBox.Show("Expected XMA file '" + Path.GetFileName(ogXMA) + "' not found, can't play this song", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            File.Copy(ogXMA, XMA_PATH, true);            

            picWorking.Visible = true;
            powerGigWorker.RunWorkerAsync();
        }

        private void PlayGHWTDEFolder()
        {
            loadDefaults();
            Parser.ReadGHWTDEFile(GHWTDE_INI_PATH);
            loadDTA();
            isGHWTDE = true;

            var default_art = Application.StartupPath + "\\res\\ghwtde.png";
            var albumPNG = Path.GetDirectoryName(GHWTDE_INI_PATH) + "\\Content\\album.png";
            var albumJPG = Path.GetDirectoryName(GHWTDE_INI_PATH) + "\\Content\\album.jpg";

            if (File.Exists(albumPNG))
            {
                getImage(albumPNG);
            }
            else if (File.Exists(albumJPG))
            {
                getImage(albumJPG);
            }
            else if (File.Exists(default_art))
            {
                getImage(default_art);
            }
            Invalidate();

            var visualizer = Application.StartupPath + "\\visualizer\\";
            if (!Directory.Exists(visualizer))
            {
                Directory.CreateDirectory(visualizer);
            }
            GHWTDE_EXT_PATH = visualizer + (string.IsNullOrEmpty(Parser.Songs[0].InternalName) ? "temp" : Parser.Songs[0].InternalName);
            if (!Directory.Exists(GHWTDE_EXT_PATH))
            {
                Directory.CreateDirectory(GHWTDE_EXT_PATH);
            }

            picWorking.Visible = true;
            ghwtdeWorker.RunWorkerAsync();            
        }        

        private void ExtractFSBAudio(string fsb, long offset, int spacer, string mp3)
        {
            byte[] mp3_data;
            const int frame = 384;//size of each frame of audio
            
            using (var ms = new MemoryStream(File.ReadAllBytes(fsb)))
            {
                using (var br = new BinaryReader(ms))
                {
                    br.BaseStream.Seek(offset, SeekOrigin.Begin);
                    while (br.BaseStream.Position < br.BaseStream.Length)
                    {
                        mp3_data = br.ReadBytes(frame);
                        br.BaseStream.Seek(spacer, SeekOrigin.Current);

                        using (var bw = new BinaryWriter(new FileStream(mp3, FileMode.Append)))
                        {
                            bw.Write(mp3_data);
                        }
                    }
                }
            }
        }

        private void loadCON(string con)
        {
            var albumart = "";
            var midi = "";
            Tools.DeleteFolder(Application.StartupPath + "\\visualizer", true);
            Directory.CreateDirectory(Application.StartupPath + "\\visualizer");
            sCON = con;
            loadDefaults();
            picWorking.Visible = true;
            if (con != "")
            {
                if (!Parser.ExtractDTA(con))
                {
                    MessageBox.Show("Something went wrong extracting the songs.dta file", Text,MessageBoxButtons.OK, MessageBoxIcon.Error);
                    picWorking.Visible = false;
                    return;
                }
                if (!Parser.ReadDTA(Parser.DTA) || !Parser.Songs.Any())
                {
                    MessageBox.Show("Something went wrong reading the songs.dta file", Text,MessageBoxButtons.OK, MessageBoxIcon.Error);
                    picWorking.Visible = false;
                    return;
                }
                if (Parser.Songs.Count > 1)
                {
                    MessageBox.Show("It looks like this is a pack...\nWhat were you expecting me to do with this?\nTry a single song, please",Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    picWorking.Visible = false;
                    return;
                }
                loadDTA();
                var xPackage = new STFSPackage(con);
                if (!xPackage.ParseSuccess)
                {
                    MessageBox.Show("There was an error parsing that file\nTry again", Text,MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                isTBRBDLC = xPackage.Header.TitleID == 0x454108B1 || xPackage.Header.TitleID == (uint)4294838225 || xPackage.Header.Description.ToLowerInvariant().Contains("beatles") ||
                xPackage.Header.Description.ToLowerInvariant().Contains("tbrb") || xPackage.Header.Title_Display.ToLowerInvariant().Contains("beatles") ||
                xPackage.Header.Title_Display.ToLowerInvariant().Contains("tbrb") || xPackage.Header.Title_Package.ToLowerInvariant().Contains("beatles") ||
                xPackage.Header.Title_Package.ToLowerInvariant().Contains("tbrb") || Parser.Songs[0].Source.Equals("beatles");
                isGDRBDLC = xPackage.Header.TitleID == 0x454108E7;
                if (isTBRBDLC && string.IsNullOrEmpty(Parser.Songs[0].Artist.Trim()))
                {
                    txtArtist.Text = "The Beatles";
                    songArtist = "The Beatles";
                }
                if (isGDRBDLC && string.IsNullOrEmpty(Parser.Songs[0].Artist.Trim()))
                {
                    txtArtist.Text = "Green Day";
                    songArtist = "Green Day";
                }
                var songname = Parser.Songs[0].InternalName;
                //set file path for later use in saving image
                try
                {
                    var xFile = xPackage.GetFile("songs/" + songname + "/" + songname + ".mid");
                    if (xFile != null)
                    {
                        midi = Application.StartupPath + "\\visualizer\\" + songname + ".mid";
                        Tools.DeleteFile(midi);
                        if (!xFile.ExtractToFile(midi))
                        {
                            midi = "";
                        }
                    }
                    xFile = xPackage.GetFile("songs/" + songname + "/gen/" + songname + "_keep.png_xbox");
                    if (xFile != null)
                    {
                        albumart = Application.StartupPath + "\\visualizer\\" + songname + "_keep.png_xbox";
                        Tools.DeleteFile(albumart);
                        if (!xFile.ExtractToFile(albumart))
                        {
                            albumart = "";
                        }
                    }
                    /*
                    var HMX_Sources = new List<string>
                    {
                        "rb1","acdc","rb2","rb3","rb1_dlc","rb2_dlc","rb3_dlc","rb4","rb4_dlc",
                        "greenday","gdrb","lego","beatles","tbrb"
                    };
                    //check if the song is a GHtoRB3 or a HMX song
                    if (xPackage.Header.Description.ToLowerInvariant().Contains("ghtorb3") ||
                        xPackage.Header.Description.ToLowerInvariant().Contains("tiny.cc/ghtorb") ||
                        xPackage.Header.Description.ToLowerInvariant().Contains("t=405473"))
                    {
                        origAuthor = "GHtoRB3";
                    }
                    else if (xPackage.Header.Description.ToLowerInvariant().Contains("rockband.com") || HMX_Sources.Contains(Parser.Songs[0].Source))
                    {
                        origAuthor = "Harmonix";
                    }
                    if (xPackage.Header.Description.Contains(xPackage.Header.Title_Display.Replace("\"", "")) &&
                        !xPackage.Header.Description.ToLowerInvariant().Contains("rockband.com") &&
                        !xPackage.Header.Description.ToLowerInvariant().Contains("ghtorb3") &&
                        !xPackage.Header.Description.ToLowerInvariant().Contains("tiny.cc/ghtorb") &&
                        !xPackage.Header.Description.ToLowerInvariant().Contains("t=405473")  &&
                        !xPackage.Header.Description.ToLowerInvariant().Contains("depacked with"))
                    {
                        origAuthor = "Rock Band Network";
                    }
                    if (xPackage.Header.Description.Contains("discord.gg/XM9gexj"))
                    {
                        origAuthor = "TBRB CDLC";
                    }*/
                    xPackage.CloseIO();
                    doShowAuthor();
                    
                    //extract audio file for previewing
                    Height = maxHeight;
                    audioProcessor.RunWorkerAsync();
                    if (albumart != "")
                    {
                        //grab the album art
                        getImage(albumart);
                    }
                    if (midi != "")
                    {
                        ReadMidi(midi);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("There was an error:\n" + ex.Message, Text,MessageBoxButtons.OK, MessageBoxIcon.Error);
                    try
                    {
                        xPackage.CloseIO();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("There was an error:\n" + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            picWorking.Visible = false;
        }

        private void doShowAuthor()
        {
            lblBottom.Cursor = Cursors.Default;
            lblTop.Text = "Author:";
            if (!string.IsNullOrWhiteSpace(origAuthor))
            {
                lblBottom.Text = RemoveCloneHeroColor(origAuthor);
                lblBottom.ForeColor = GetCloneHeroColor(origAuthor);
            }
            else if (!string.IsNullOrWhiteSpace(author))
            {
                lblBottom.Text = RemoveCloneHeroColor(author);
                if (!string.IsNullOrEmpty(lblBottom.Text) && !lblBottom.Text.Contains(","))
                {
                    lblBottom.Cursor = Cursors.Hand;
                    toolTip1.SetToolTip(lblBottom, $"Click here to search for more {lblBottom.Text} songs");
                }
                lblBottom.ForeColor = GetCloneHeroColor(author);
            }
            else if (Parser.Songs[0].Source.Equals("beatles"))
            {
                lblBottom.Text = "Harmonix"; //all TBRB game songs are attributable to Harmonix
            }
            else
            {
                lblBottom.Text = "Unknown";
            }
        }
        
        private string RemoveCloneHeroColor(string author)
        {
            try
            {
                int startIndex = author.IndexOf('>') + 1;
                int endIndex = author.LastIndexOf('<');
                return author.Substring(startIndex, endIndex - startIndex);
            }
            catch
            {
                try
                {
                    int startIndex = author.IndexOf('<');
                    int endIndex = author.LastIndexOf('>') + 1;
                    var color = author.Substring(startIndex, endIndex - startIndex);
                    return author.Replace(color, "").Trim();
                }
                catch
                {
                    return author;
                }
            }            
        }

        private Color GetCloneHeroColor(string author)
        {
            try
            {
                int colorStartIndex = author.IndexOf('=') + 1;
                int colorEndIndex = author.IndexOf('>');
                string colorValue = author.Substring(colorStartIndex, colorEndIndex - colorStartIndex);

                Color color;
                try
                {
                    color = ColorTranslator.FromHtml(colorValue); // Convert from hex
                }
                catch
                {
                    color = Color.Black; // Default to black if invalid
                }

                return color;
            }
            catch
            {
                return Color.Black;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //reset links in case this is a re-upload
            lblTop.Text = radioImgur.Checked ? "Uploading..." : "Saving...";
            lblBottom.Text = "";
            imgLink = "";
            if (RESOURCE_AUTHOR_LOGO == null)
            {
                picLogo.BorderStyle = BorderStyle.None;
            }
            picLastFM.Visible = false;
            Application.DoEvents();
            
            //capture image
            using (bitmap = new Bitmap(picVisualizer.Width, picVisualizer.Height))
            {
                string xOut;
                               
                //NEW METHOD - seems to scale well
                picVisualizer.DrawToBitmap(bitmap, picVisualizer.ClientRectangle);

                picLastFM.Visible = true;//restore after grabbing screenshot

                if (radioLocal.Checked)
                {
                    //prepare to prompt user for save location and extension
                    var fileOutput = new SaveFileDialog
                        {
                            Filter = "JPG Image|*.jpg|PNG Image|*.png|Bitmap Image|*.bmp",
                            Title = "Where should I save the visual to?",
                            FileName = "visual_",
                            InitialDirectory = Application.StartupPath + "\\visualizer\\"
                        };
                    //if sCon has a value, i.e. we're working with a con
                    //prepopulate the fields for the user
                    if (!string.IsNullOrWhiteSpace(sCON))
                    {
                        fileOutput.InitialDirectory = Path.GetDirectoryName(sCON);
                        fileOutput.FileName = Path.GetFileNameWithoutExtension(sCON) + "_visual";
                    }
                    //show dialog, get final file name and extension
                    if (fileOutput.ShowDialog() == DialogResult.OK)
                    {
                        xOut = fileOutput.FileName;
                        Tools.CurrentFolder = Path.GetDirectoryName(xOut);
                    }
                    else
                    {
                        xOut = null;
                    }
                    if (xOut == null)
                    {
                        //re-enable the outlines for the Icon Slots
                        picIcon1.BorderStyle = BorderStyle.FixedSingle;
                        picIcon2.BorderStyle = BorderStyle.FixedSingle;
                        doShowAuthor();
                        return;
                    }
                }
                else
                {
                    xOut = Application.StartupPath + "\\bin\\temp.jpg";
                }
                ImageOut = xOut;
                //this is so image quality is higher than the default
                var myEncoder = Encoder.Quality;
                var myEncoderParameters = new EncoderParameters(1);
                var myEncoderParameter = new EncoderParameter(myEncoder, 100L);
                myEncoderParameters.Param[0] = myEncoderParameter;
                //set encoder based on user choice
                var imgFormat = Path.GetExtension(xOut);
                ImageCodecInfo myImageCodecInfo;
                switch (imgFormat)
                {
                    case "bmp":
                        myImageCodecInfo = Tools.GetEncoderInfo("image/bmp");
                        break;
                    case "png":
                        myImageCodecInfo = Tools.GetEncoderInfo("image/png");
                        break;
                    default:
                        myImageCodecInfo = Tools.GetEncoderInfo("image/jpeg");
                        break;
                }
                //save image
                bitmap.Save(xOut, myImageCodecInfo, myEncoderParameters);
                if (RESOURCE_AUTHOR_LOGO == null)
                {
                    picLogo.BorderStyle = BorderStyle.Fixed3D;
                }
                if (!radioImgur.Checked)
                {
                    if (File.Exists(ImageOut))
                    {
                        lblTop.Text = "Saved successfully";
                        lblBottom.Text = "Click to view";
                        toolTip1.SetToolTip(lblBottom, "Click here to view your saved image");
                        lblBottom.Cursor = Cursors.Hand;
                        imgLink = ImageOut;
                    }
                    else
                    {
                        lblTop.Text = "Saving failed!";
                    }
                    return;
                }
                picWorking.Visible = true;
                Application.DoEvents();
                uploader.RunWorkerAsync();
            }
        }

        private void Visualizer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!picWorking.Visible)
            {
                StopPlayback();
                //let's not leave over any files by mistake
                Tools.DeleteFile(Path.GetTempPath() + "o");
                Tools.DeleteFile(Path.GetTempPath() + "m");
                Tools.DeleteFile(tempFile);
                foreach (var file in FilesToDelete)
                {
                    Tools.DeleteFile(file);
                }
                Tools.DeleteFolder(Application.StartupPath + "\\visualizer\\", true);
                SaveConfig();
                try
                {
                    if (audioProcessor.IsBusy)
                    {
                        audioProcessor.CancelAsync();
                    }
                }
                catch (Exception)
                {}

                if (isRunningShortcut)
                {
                    Environment.Exit(0);
                }
                return;
            }
            MessageBox.Show("Please wait until the current process finishes", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            e.Cancel = true;
        }
        
        private void txtSong_TextChanged(object sender, EventArgs e)
        {
            picVisualizer.Invalidate();
            if (reset || loading) return;
            txtSong.Text = Tools.FixFeaturedArtist(txtSong.Text);
            if (string.IsNullOrWhiteSpace(txtSong.Text)) return;
            CheckSongName(txtSong);
            MeasureSong(txtSong);
            picVisualizer.Invalidate();
        }

        private void MeasureAlbum()
        {
            /*//create font variable for measuring string size
            var f = new Font(ActiveFont, 11, FontStyle.Bold);
            SizeF name = TextRenderer.MeasureText(txtAlbum.Text.Trim(), f);
            var maxSize = txtYear2.Text.Trim().Length > 0 ? 160 : 200;
            if (name.Width <= maxSize) return;
            if (string.IsNullOrWhiteSpace(txtAlbum.Text.Trim())) return;
            var maxLength = txtYear2.Text.Trim().Length > 0 ? 19 : 24;
            txtAlbum.Text = txtAlbum.Text.Trim().Substring(0, maxLength) + "...";
            txtAlbum.SelectionStart = txtAlbum.Text.Length;
            picVisualizer.Invalidate();
            */
            //disable this, let the user arrange the size or not
        }

        private void MeasureSong(Control tb)
        {
            var song = tb.Text;

            //create font variable for measuring string size
            var f = new Font(ActiveFont, 20, FontStyle.Bold);

            SizeF name = TextRenderer.MeasureText(song, f);

            //don't want it bigger than point 20
            float fSize;
            if (name.Width > 200) //preset size reserved for the song name
            {
                var factor = 205/name.Width; //size + 5, just because it works well
                fSize = (20*factor);
            }
            else
            {
                fSize = 20;
            }

            if ((Decimal) fSize < numFontName.Minimum)
            {
                fSize = (float) numFontName.Minimum;
            }
            if ((Decimal) fSize > numFontName.Maximum)
            {
                fSize = (float) numFontName.Maximum;
            }

            if (tb == txtSong1 || tb == txtSong2)
            {
                if (((Decimal) fSize > song1 && song1 > 0) || ((Decimal) fSize > song2 && song2 > 0))
                {
                    return;
                }
            }

            if (tb == txtSong1)
            {
                song1 = (Decimal) fSize;
            }
            else if (tb == txtSong2)
            {
                song2 = (Decimal) fSize;
            }
            numFontName.Value = (Decimal)fSize;
            picVisualizer.Invalidate();
            if (numFontName.Value >= 10 || !txtSong.Visible) return;
            SplitJoinSong_Click(null, null);
        }

        private void CheckSongName(TextBoxBase box)
        {
            if (loading) return;
            var message = "";
            var songName = box.Text;

            for (var i = 1; i < 14; i++)
            {
                switch (i)
                {
                    case 1:
                        message = "rhythm version";
                        break;
                    case 2:
                        message = "rhytm version";
                        break;
                    case 3:
                        message = "rythm version";
                        break;
                    case 4:
                        message = "rytm version";
                        break;
                    case 5:
                        message = "2x bass pedal";
                        break;
                    case 6:
                        message = "2x bass";
                        break;
                    case 7:
                        message = "2x pedal";
                        break;
                    case 8:
                        message = "rb3 version";
                        break;
                    case 9:
                        message = "rb3version";
                        break;
                    case 10:
                        message = "rhythm guitar version";
                        break;
                    case 11:
                        message = "rhytm guitar version";
                        break;
                    case 12:
                        message = "rythm guitar version";
                        break;
                    case 13:
                        message = "rytm guitar version";
                        break;
                }

                if (!box.Text.ToLowerInvariant().Contains(message)) continue;
                switch (i)
                {
                    case 13:
                    case 12:
                    case 11:
                    case 10:
                    case 4:
                    case 3:
                    case 2:
                    case 1:
                        if (!bRhythm)
                        {
                            sendIcon(picRKeys.Image);
                            bRhythm = true;
                        }
                        break;
                    case 7:
                    case 6:
                    case 5:
                        pic2x.Image = Tools.NemoLoadImage(Application.StartupPath + "\\res\\2x.png");
                        break;
                    case 9:
                    case 8:
                        sendIcon(rb3Version);
                        break;
                    default:
                        return;
                }
                songName = songName.ToLowerInvariant().Replace(message, "");
                songName = songName.ToLowerInvariant().Replace("()", "").Trim();
                songName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(songName);
                box.Text = songName;
                box.SelectionStart = box.Text.Length;
            }
            picVisualizer.Invalidate();
        }

        private void txtArtist_TextChanged(object sender, EventArgs e)
        {
            picVisualizer.Invalidate();
            if (reset || loading) return;
            txtArtist2.Text = Tools.FixFeaturedArtist(txtArtist2.Text);
            if (txtArtist.Text != "")
            {
                MeasureArtist(txtArtist);
            }
            picVisualizer.Invalidate();
        }

        private void MeasureArtist(Control tb)
        {
            var artist = tb.Text;

            //create font variable for measuring string size
            var f = new Font(ActiveFont, 20, FontStyle.Bold);

            SizeF name = TextRenderer.MeasureText(artist, f);

            //don't want it bigger than point 20
            float fSize;
            if (name.Width > 230) //preset size reserved for the artist name
            {
                var factor = 230/name.Width; //size + 5, just because it works well
                fSize = (20*factor);
            }
            else
            {
                fSize = 20;
            }
            if ((Decimal) fSize < numFontArtist.Minimum)
            {
                fSize = (float) numFontArtist.Minimum;
            }
            if ((Decimal) fSize > numFontArtist.Maximum)
            {
                fSize = (float) numFontArtist.Maximum;
            }

            if (tb == txtArtist1 || tb == txtArtist2)
            {
                if (((Decimal) fSize > artist1 && artist1 > 0) || ((Decimal) fSize > artist2 && artist2 > 0))
                {
                    return;
                }
            }

            if (tb == txtArtist1)
            {
                artist1 = (Decimal) fSize;
            }
            else if (tb == txtArtist2)
            {
                artist2 = (Decimal) fSize;
            }
            numFontArtist.Value = (Decimal)fSize;
            picVisualizer.Invalidate();
            if (numFontArtist.Value >= 10 || !txtArtist.Visible) return;
            SplitJoinArtist_Click(null, null);
        }

        private void txtAlbum_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAlbum.Text))
            {
                genreY = albumY;
                picGenreDown.Visible = true;
                picGenreUp.Visible = true;
            }
            else
            {
                genreY = 130f;
                picGenreDown.Visible = false;
                picGenreUp.Visible = false;
            }
            picVisualizer.Invalidate();
            if (reset) return;
            MeasureAlbum();
        }
        
        private void txtYear_TextChanged(object sender, EventArgs e)
        {
            picVisualizer.Invalidate();
            if (reset) return;
            MeasureAlbum();
        }
        
        private void updatePlaybackInstruments()
        {
            PlayDrums = Parser.Songs[0].ChannelsDrums > 0;
            PlayBass = Parser.Songs[0].ChannelsBass > 0;
            PlayGuitar = Parser.Songs[0].ChannelsGuitar > 0;
            PlayKeys = Parser.Songs[0].ChannelsKeys > 0;
            PlayVocals = Parser.Songs[0].ChannelsVocals > 0;
            PlayCrowd = Parser.Songs[0].ChannelsCrowd > 0;
            PlayBacking = true;// should be always present per specifications
            try
            {
                var folder = Application.StartupPath + "\\res\\play\\";
                picPlayDrums.Image = Tools.NemoLoadImage(folder + (PlayDrums ? "drums" : "nodrums") + ".png");
                picPlayDrums.Enabled = PlayDrums;
                picPlayBass.Image = Tools.NemoLoadImage(folder + (PlayBass ? "bass" : "nobass") + ".png");
                picPlayBass.Enabled = PlayBass;
                picPlayGuitar.Image = Tools.NemoLoadImage(folder + (PlayGuitar ? "guitar" : "noguitar") + ".png");
                picPlayGuitar.Enabled = PlayGuitar;
                picPlayKeys.Image = Tools.NemoLoadImage(folder + (PlayKeys ? "keys" : "nokeys") + ".png");
                picPlayKeys.Enabled = PlayKeys;
                picPlayVocals.Image = Tools.NemoLoadImage(folder + (PlayVocals ? "vocals" : "novocals") + ".png");
                picPlayVocals.Enabled = PlayVocals;
                picPlayCrowd.Image = Tools.NemoLoadImage(folder + (PlayCrowd ? "crowd" : "nocrowd") + ".png");
                picPlayCrowd.Enabled = PlayCrowd;
                picPlayBacking.Image = Tools.NemoLoadImage(folder + (PlayBacking ? "backing" : "nobacking") + ".png");
                picPlayBacking.Enabled = PlayBacking;
            }
            catch (Exception)
            {
                MessageBox.Show("There was an error loading the playback images, make sure you haven't deleted any files", Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        public void loadDTA()
        {
            updatePlaybackInstruments();
            PlaybackSeconds = Parser.Songs[0].PreviewStart == 0 || picPreview.Tag.ToString() == "song" ? 0 : (double)Parser.Songs[0].PreviewStart / 1000;
            lblSongLength.Text = Parser.GetSongDuration(Parser.Songs[0].Length.ToString(CultureInfo.InvariantCulture));
            UpdateTime();            
            try
            {
                origAuthor = "";
                author = "";
                loading = true;
                txtSong.Text = string.IsNullOrWhiteSpace(Parser.Songs[0].OverrideName) ? Parser.Songs[0].Name : Parser.Songs[0].OverrideName;
                var artist = Parser.Songs[0].Artist.Trim();
                if (isTBRBDLC && string.IsNullOrEmpty(artist) || Parser.Songs[0].Source.Equals("beatles"))
                {
                    artist = "The Beatles";
                }
                else if (isGDRBDLC && string.IsNullOrEmpty(artist))
                {
                    artist = "Green Day";
                }
                txtArtist.Text = artist;
                songArtist = artist;
                songAlbum = Parser.Songs[0].Album;
                txtAlbum.Text = Parser.Songs[0].Album;
                txtTrack.Text = Parser.Songs[0].TrackNumber <= 0 ? "" : Parser.Songs[0].TrackNumber.ToString(CultureInfo.InvariantCulture);
                txtYear.Text = Parser.Songs[0].YearReleased == 0 ? "" : Parser.Songs[0].YearReleased.ToString(CultureInfo.InvariantCulture);
                txtYear2.Text = Parser.Songs[0].YearRecorded == 0 ? "" : Parser.Songs[0].YearRecorded.ToString(CultureInfo.InvariantCulture);
                var vocal_parts = Parser.Songs[0].VocalParts;
                txtTime.Text = Parser.Songs[0].Length == 0 ? "" : Parser.GetSongDuration(Parser.Songs[0].Length.ToString(CultureInfo.InvariantCulture));
                diffDrums.Tag = Parser.Songs[0].DrumsDiff;
                if (Parser.Songs[0].BassDiff > 0)
                {
                    diffBass.Tag = Parser.Songs[0].BassDiff;
                } 
                else if (Parser.Songs[0].RhythmBass || Parser.Songs[0].RhythmDiff > 0)
                {
                    diffBass.Tag =  Parser.Songs[0].RhythmDiff;
                }
                else
                {
                    diffBass.Tag = 0;
                }
                diffGuitar.Tag = Parser.Songs[0].GuitarDiff;
                diffVocals.Tag = Parser.Songs[0].VocalsDiff;
                diffKeys.Tag = Parser.Songs[0].KeysDiff;
                if (Parser.Songs[0].ProBassDiff > 0 || isRS2014)
                {
                    if (!isRS2014)
                    {
                        diffBass.Tag = Parser.Songs[0].ProBassDiff;
                    }                    
                    proBass.Tag = 1;
                }
                else
                {
                    proBass.Tag = 0;
                }
                if (Parser.Songs[0].ProGuitarDiff > 0 || isRS2014)
                {
                    if (!isRS2014)
                    {
                        diffGuitar.Tag = Parser.Songs[0].ProGuitarDiff;
                    }
                    proGuitar.Tag = 1;
                }
                else
                {
                    proGuitar.Tag = 0;
                }
                if (Parser.Songs[0].ProKeysDiff > 0)
                {
                    diffKeys.Tag = Parser.Songs[0].ProKeysDiff;
                    proKeys.Tag = 1;
                }
                else
                {
                    proKeys.Tag = 0;
                    ProKeysEnabled = false;
                }
                txtGenre.Text = Parser.Songs[0].Genre;
                txtSubGenre.Text = Parser.Songs[0].SubGenre == null ? "" : Parser.Songs[0].SubGenre;
                if (txtSubGenre.Text != txtGenre.Text && txtSubGenre.Text != "Other")
                {
                    chkSubGenre.Checked = true;
                }
                try
                {
                    cboRating.SelectedIndex = Parser.Songs[0].Rating - 1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("There was an error:\n" + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                author = Parser.Songs[0].ChartAuthor == null ? "" : Parser.Songs[0].ChartAuthor.Replace("&", "&&"); //fix for Nunchuk & Sygenysis

                if (Parser.Songs[0].DisableProKeys)
                {
                    proKeys.Image = null;
                }
                if (Parser.Songs[0].RhythmBass)
                {
                    bRhythm = true;
                    sendIcon(picRBass.Image);
                }
                if (Parser.Songs[0].RhythmKeys && !bRhythm)
                {
                    bRhythm = true;
                    sendIcon(picRKeys.Image);
                }
                pic2x.Tag = Parser.Songs[0].DoubleBass ? 1 : 0;
                if (Parser.Songs[0].Karaoke)
                {
                    sendIcon(picKaraoke.Image);
                }
                if (Parser.Songs[0].Multitrack)
                {
                    sendIcon(picMulti.Image);
                }
                if (Parser.Songs[0].Convert)
                {
                    if (picIcon1.Image == null || picIcon2.Image == null)
                    {
                        sendIcon(picConvert1.Image);
                    }
                }
                if (Parser.Songs[0].DIYStems)
                {
                    sendIcon(picDIYStems.Image);
                }
                if (Parser.Songs[0].PartialMultitrack)
                {
                    sendIcon(picPartial.Image);
                }
                if (Parser.Songs[0].UnpitchedVocals)
                {
                    sendIcon(picUnpitched.Image);
                }
                if (Parser.Songs[0].RB3Version)
                {
                    sendIcon(rb3Version);
                }
                if (Parser.Songs[0].CATemh)
                {
                    if ((picIcon1.Image == null || picIcon2.Image == null) && !isXOnly)
                    {
                        sendIcon(catEMH);
                    }
                }
                if (Parser.Songs[0].ExpertOnly)
                {
                    if ((picIcon1.Image == null || picIcon2.Image == null) && !isXOnly)
                    {
                        sendIcon(expertOnly);
                    }
                    isXOnly = true;
                }
                loading = false;
                CheckSongName(txtSong);
                if (txtSong.Text != "")
                {
                    MeasureSong(txtSong);
                }
                MeasureArtist(txtArtist);
                switch (vocal_parts)
                {
                    case 2:
                        intVocals = 2;
                        break;
                    case 3:
                        intVocals = 3;
                        break;
                    default:
                        intVocals = 1;
                        break;
                }               
                if (string.IsNullOrWhiteSpace(txtAlbum.Text))
                {
                    genreY = albumY;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was a problem Visualizing that file\nError: " + ex.Message, "Visualizer",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            doShowAuthor();
            picVisualizer.Invalidate();
        }
        
        private async void button1_Click(object sender, EventArgs e)
        {
            var openFileDialog1 = new OpenFileDialog
                {
                    Title = "Open song file",
                    InitialDirectory = Tools.CurrentFolder
                };
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
            var file = openFileDialog1.FileName;

            try
            {
                var ext = Path.GetExtension(file).ToLowerInvariant();
                var exts = new List<string> { ".jpg", ".bmp", ".tif", ".dds", ".gif", ".tpl", ".png", ".jpeg", ".png_xbox", ".png_ps3", ".png_ps4", ".png_wii" };
                var isImage = exts.Contains(ext);

                if (isImage)
                {
                    getImage(file);
                }
                else if (VariousFunctions.ReadFileType(file) == XboxFileType.STFS)
                {
                    var package = new STFSPackage(file);
                    if (!package.ParseSuccess) return;
                    Tools.CurrentFolder = Path.GetDirectoryName(file);
                    if (package.Header.TitleID == (uint)1514538961)
                    {
                        package.CloseIO();
                        ExtractPowerGig(file);
                        return;
                    }
                    else if (package.Header.TitleID == (uint)1296435155)
                    {
                        package.CloseIO();
                        ExtractBandFuse(file);
                        return;
                    }
                    else //assume Harmonix game
                    {
                        package.CloseIO();
                        loadDefaults();
                        loadCON(file);
                    }                   
                }
                else if (Path.GetFileName(file).ToLowerInvariant() == "songs.dta")
                {
                    if (!Parser.ReadDTA(File.ReadAllBytes(file)) || !Parser.Songs.Any())
                    {
                        MessageBox.Show("Something went wrong reading that songs.dta file", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    loadDefaults();
                    loadDTA();
                }
                else if (Path.GetExtension(file.ToLowerInvariant()) == ".pkg")
                {
                    ExtractPKG(file);
                }
                else if (Path.GetExtension(file.ToLowerInvariant()) == ".sng")
                {
                    await ExtractSNG(file);
                }
                else if (Path.GetExtension(file.ToLowerInvariant()) == ".yargsong")
                {
                    await ExtractYARG(file);
                }
                else if (Path.GetExtension(file.ToLowerInvariant()) == ".xml")
                {
                    ExtractXMA(file);
                }
                else if (Path.GetExtension(file.ToLowerInvariant()) == ".psarc")
                {
                    ExtractPsArc(file);
                }
                else if (Path.GetFileName(file).ToLowerInvariant() == "song.ini")
                {
                    PlayCHFolder(Path.GetDirectoryName(file));
                }
                else
                {
                    MessageBox.Show("That's not a song file ... try again", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error accessing that file\nThe error says:\n" + ex.Message, Text,MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var message = Tools.ReadHelpFile("vi");
            var help = new HelpForm(Text + " - Help", message);
            help.ShowDialog();
        }

        private void sendImage(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            clicks++;
            if (clicks == 2)
            {
                clicks = 0;
                if (pictureFrom == sender)
                {
                    sendIcon(((PictureBox)sender).Image);
                    return;
                }
            }
            pictureFrom = ((PictureBox)(sender));
            var image = pictureFrom == picIcon1 ? RESOURCE_ICON1 : (pictureFrom == picIcon2 ? RESOURCE_ICON2 : ((PictureBox)(sender)).Image);
            if (image == null) return;
            DoDragDrop(image, DragDropEffects.Move);
            picVisualizer.Invalidate();
        }

        private void picIcon1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Bitmap))
                e.Effect = DragDropEffects.Move;
        }

        private void picIcon2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Bitmap))
                e.Effect = DragDropEffects.Move;
        }

        private void receiveImage(object sender, DragEventArgs e)
        {
            try
            {
                var icon = (Bitmap) e.Data.GetData(DataFormats.Bitmap);
                if (pictureFrom == picIcon1)
                {
                    RESOURCE_ICON1 = RESOURCE_ICON2;
                }
                else if (pictureFrom == picIcon2)
                {
                    RESOURCE_ICON2 = RESOURCE_ICON1;
                }
                if (sender == picIcon1)
                {
                    RESOURCE_ICON1 = icon;
                }
                else
                {
                    RESOURCE_ICON2 = icon;
                }
                clicks = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error:\n" + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            picVisualizer.Invalidate();
        }

        private void sendIcon(Image icon)
        {
            var bmp = icon;
            if (bmp == null) return;
            if (RESOURCE_ICON1 == null)
            {
                RESOURCE_ICON1 = bmp;
            }
            else
            {
                if (RESOURCE_ICON2 != null)
                {
                    RESOURCE_ICON1 = RESOURCE_ICON2;
                }
                RESOURCE_ICON2 = bmp;
            }
            picVisualizer.Invalidate();
        }
        
        private void lblGenre_Click(object sender, EventArgs e)
        {
            chkGenre.Checked = !chkGenre.Checked;
        }

        private void lblSubGenre_Click(object sender, EventArgs e)
        {
            chkSubGenre.Checked = !chkSubGenre.Checked;
        }
        
        private void ChangeColor(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            colorDialog1.Color = ((PictureBox)(sender)).BackColor;
            colorDialog1.CustomColors = new[] { ColorTranslator.ToOle(DefaultColorDark), ColorTranslator.ToOle(DefaultColorLight) };
            colorDialog1.SolidColorOnly = true;
            colorDialog1.ShowDialog();
            ((PictureBox)(sender)).BackColor = colorDialog1.Color;
            switch (((PictureBox)(sender)).Name)
            {
                case "barSong":
                    songColor = colorDialog1.Color;
                    break;
                case "barArtist":
                    artistColor = colorDialog1.Color;
                    break;
                case "barTime":
                    timeColor = colorDialog1.Color;
                    break;
                case "barAlbum":
                    albumColor = colorDialog1.Color;
                    break;
                case "barYear":
                    yearColor = colorDialog1.Color;
                    break;
                case "barGenre":
                    genreColor = colorDialog1.Color;
                    break;
            }
            picVisualizer.Invalidate();
        }
        
        private void songJoystick_MouseMove(object sender, MouseEventArgs e)
        {
            if (songJoystick.Cursor != Cursors.NoMove2D) return;
            if (MousePosition.X != mouseX)
            {
                if (MousePosition.X > mouseX)
                {
                    songX = songX + (MousePosition.X - mouseX);
                }
                else if (MousePosition.X < mouseX)
                {
                    songX = songX - (mouseX - MousePosition.X);
                }
                mouseX = MousePosition.X;
            }
            picVisualizer.Invalidate();
            if (MousePosition.Y == mouseY) return;
            if (MousePosition.Y > mouseY)
            {
                songY = songY + (MousePosition.Y - mouseY);
            }
            else if (MousePosition.Y < mouseY)
            {
                songY = songY - (mouseY - MousePosition.Y);
            }
            mouseY = MousePosition.Y;
            picVisualizer.Invalidate();
        }
        
        private void artistJoystick_MouseMove(object sender, MouseEventArgs e)
        {
            if (artistJoystick.Cursor != Cursors.NoMove2D) return;
            if (MousePosition.X != mouseX)
            {
                if (MousePosition.X > mouseX)
                {
                    artistX = artistX + (MousePosition.X - mouseX);
                }
                else if (MousePosition.X < mouseX)
                {
                    artistX = artistX - (mouseX - MousePosition.X);
                }
                mouseX = MousePosition.X;
            }
            picVisualizer.Invalidate();
            if (MousePosition.Y == mouseY) return;
            if (MousePosition.Y > mouseY)
            {
                artistY = artistY + (MousePosition.Y - mouseY);
            }
            else if (MousePosition.Y < mouseY)
            {
                artistY = artistY - (mouseY - MousePosition.Y);
            }
            mouseY = MousePosition.Y;
            picVisualizer.Invalidate();
        }

        private void saveProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog
                {
                    InitialDirectory = Tools.CurrentFolder,
                    Filter = "Visualizer Profile|*.vis",
                    Title = "Save Visualizer Profile",
                };
            sfd.ShowDialog();
            if (string.IsNullOrWhiteSpace(sfd.FileName)) return;
            UserProfile = sfd.FileName + ".vis";
            if (string.IsNullOrWhiteSpace(UserProfile)) return;
            try
            {
                if (File.Exists(UserProfile))
                {
                    if (MessageBox.Show("A profile with the name " + Path.GetFileName(sfd.FileName) + " already exists\nDo you want to overwrite it?", "File already exists!",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes)
                    {
                        saveProfileToolStripMenuItem.PerformClick();
                        return;
                    }
                }
                var sw = new StreamWriter(UserProfile, false, Encoding.Default);
                sw.WriteLine("//Visualizer Profile");
                sw.WriteLine("//DO NOT EDIT MANUALLY");
                sw.WriteLine("songColor=#" + songColor.R.ToString("X2") + songColor.G.ToString("X2") + songColor.B.ToString("X2"));
                sw.WriteLine("artistColor=#" + artistColor.R.ToString("X2") + artistColor.G.ToString("X2") + artistColor.B.ToString("X2"));
                sw.WriteLine("timeColor=#" + timeColor.R.ToString("X2") + timeColor.G.ToString("X2") + timeColor.B.ToString("X2"));
                sw.WriteLine("albumColor=#" + albumColor.R.ToString("X2") + albumColor.G.ToString("X2") + albumColor.B.ToString("X2"));
                sw.WriteLine("yearColor=#" + yearColor.R.ToString("X2") + yearColor.G.ToString("X2") + yearColor.B.ToString("X2"));
                sw.WriteLine("genreColor=#" + genreColor.R.ToString("X2") + genreColor.G.ToString("X2") + genreColor.B.ToString("X2"));
                sw.WriteLine("songFontSize=" + numFontName.Value);
                sw.WriteLine("artistFontSize=" + numFontArtist.Value);
                sw.WriteLine("timeFontSize=" + numFontTime.Value);
                sw.WriteLine("authorLogoX=" + picLogo.Left);
                sw.WriteLine("authorLogoY=" + picLogo.Top);
                sw.Dispose();
                Tools.CurrentFolder = Path.GetDirectoryName(UserProfile);
                MessageBox.Show("Visualizer Profile saved successfully!", "Success", MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error saving your Visualizer Profile\nThe error I got was:\n\n" + ex.Message,"Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void loadProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserProfile = "";
            try
            {
                var ofd = new OpenFileDialog
                    {
                        InitialDirectory = Tools.CurrentFolder,
                        Filter = "Visualizer Profile|*.vis",
                        Title = "Open Visualizer Profile"
                    };
                ofd.ShowDialog();
                if (string.IsNullOrWhiteSpace(ofd.FileName)) return;
                if (string.IsNullOrWhiteSpace(ofd.FileName)) return;
                UserProfile = ofd.FileName;
                loadProfile();
                Tools.CurrentFolder = Path.GetDirectoryName(UserProfile);
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error loading the Visualizer Profile\nThe error I got was\n\n" + ex.Message, "Error!",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void loadProfile()
        {
            if (!File.Exists(UserProfile)) return;
            var sr = new StreamReader(UserProfile, Encoding.Default);
            try
            {
                sr.ReadLine();//skip header
                sr.ReadLine();//skip header
                songColor = ColorTranslator.FromHtml(Tools.GetConfigString(sr.ReadLine()));
                artistColor = ColorTranslator.FromHtml(Tools.GetConfigString(sr.ReadLine()));
                timeColor = ColorTranslator.FromHtml(Tools.GetConfigString(sr.ReadLine()));
                albumColor = ColorTranslator.FromHtml(Tools.GetConfigString(sr.ReadLine()));
                yearColor = ColorTranslator.FromHtml(Tools.GetConfigString(sr.ReadLine()));
                genreColor = ColorTranslator.FromHtml(Tools.GetConfigString(sr.ReadLine()));
                numFontName.Value = Convert.ToDecimal(Tools.GetConfigString(sr.ReadLine()));
                numFontArtist.Value = Convert.ToDecimal(Tools.GetConfigString(sr.ReadLine()));
                numFontTime.Value = Convert.ToDecimal(Tools.GetConfigString(sr.ReadLine()));
                picLogo.Left = Convert.ToInt16(Tools.GetConfigString(sr.ReadLine()));
                picLogo.Top = Convert.ToInt16(Tools.GetConfigString(sr.ReadLine()));
            }
            catch (Exception)
            {}
            sr.Dispose();
            barSong.BackColor = songColor;
            barArtist.BackColor = artistColor;
            barTime.BackColor = timeColor;
            barYear.BackColor = yearColor;
            barAlbum.BackColor = albumColor;
            barGenre.BackColor = genreColor;
            picVisualizer.Invalidate();
        }

        private void txtSong1_TextChanged(object sender, EventArgs e)
        {
            picVisualizer.Invalidate();
            if (reset || loading) return;
            txtSong1.Text = Tools.FixFeaturedArtist(txtSong1.Text);
            if (string.IsNullOrWhiteSpace(txtSong1.Text))
            {
                txtSong2.Text = "";
                txtSong2.Enabled = false;
            }
            else
            {
                txtSong2.Enabled = true;
            }
            if (txtSong1.Text.Length > txtSong2.Text.Length)
            {
                if (string.IsNullOrWhiteSpace(txtSong1.Text)) return;
                CheckSongName(txtSong1);
                MeasureSong(txtSong1);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(txtSong2.Text)) return;
                CheckSongName(txtSong2);
                MeasureSong(txtSong2);
            }
            picVisualizer.Invalidate();
        }

        private void txtSong2_TextChanged(object sender, EventArgs e)
        {
            picVisualizer.Invalidate();
            if (reset || loading) return;
            txtSong2.Text = Tools.FixFeaturedArtist(txtSong2.Text);
            if (txtSong2.Text.Length > txtSong1.Text.Length)
            {
                if (string.IsNullOrWhiteSpace(txtSong2.Text)) return;
                CheckSongName(txtSong2);
                MeasureSong(txtSong2);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(txtSong1.Text)) return;
                CheckSongName(txtSong1);
                MeasureSong(txtSong1);
            }
            picVisualizer.Invalidate();
        }

        private void txtArtist1_TextChanged(object sender, EventArgs e)
        {
            picVisualizer.Invalidate();
            if (reset || loading) return;
            txtArtist1.Text = Tools.FixFeaturedArtist(txtArtist1.Text);
            if (string.IsNullOrWhiteSpace(txtArtist1.Text))
            {
                txtArtist2.Text = "";
                txtArtist2.Enabled = false;
            }
            else
            {
                txtArtist2.Enabled = true;
            }
            if (txtArtist1.Text.Length > txtArtist2.Text.Length && !string.IsNullOrWhiteSpace(txtArtist1.Text))
            {
                MeasureArtist(txtArtist1);
            }
            else if (!string.IsNullOrWhiteSpace(txtArtist2.Text))
            {
                MeasureArtist(txtArtist2);
            }
            picVisualizer.Invalidate();
        }

        private void txtArtist2_TextChanged(object sender, EventArgs e)
        {
            picVisualizer.Invalidate();
            if (reset || loading) return;
            txtArtist2.Text = Tools.FixFeaturedArtist(txtArtist2.Text);
            if (txtArtist2.Text.Length > txtArtist1.Text.Length && !string.IsNullOrWhiteSpace(txtArtist2.Text))
            {
                MeasureArtist(txtArtist2);
            }
            else if (!string.IsNullOrWhiteSpace(txtArtist1.Text))
            {
                MeasureArtist(txtArtist1);
            }
            picVisualizer.Invalidate();
        }

        private void SplitJoinSong_Click(object sender, EventArgs e)
        {
            if (SplitJoinSong.Text == "SPLIT")
            {
                if (string.IsNullOrWhiteSpace(txtSong.Text)) return;
                int divider;
                if (txtSong.Text.Contains("("))
                {
                    divider = txtSong.Text.IndexOf("(", StringComparison.Ordinal) - 1;
                }
                else if (txtSong.Text.Contains("feat"))
                {
                    divider = txtSong.Text.IndexOf("feat", StringComparison.Ordinal) - 1;
                }
                else if (txtSong.Text.Contains("ft."))
                {
                    divider = txtSong.Text.IndexOf("ft.", StringComparison.Ordinal) - 1;
                }
                else
                {
                    divider = txtSong.Text.Length/2;
                    divider = txtSong.Text.IndexOf(" ", divider, StringComparison.Ordinal);
                }
                if (divider <= 0)
                {
                    divider = txtSong.Text.IndexOf(" ", StringComparison.Ordinal);
                }
                if (divider <= 0)
                {
                    MessageBox.Show("What do you want me to split?\nI can't find a space in the song name!",
                                    Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                txtSong1.Visible = true;
                txtSong2.Visible = true;
                txtSong.Visible = false;
                txtSong1.Text = txtSong.Text.Substring(0, divider).Trim();
                txtSong2.Text = txtSong.Text.Substring(divider + 1, txtSong.Text.Length - divider - 1).Trim();
                txtSong.Text = "";
                txtSong2.Focus();
                txtSong2.SelectionStart = txtSong2.Text.Length;
                SplitJoinSong.Text = "JOIN";
            }
            else
            {
                txtSong1.Visible = false;
                txtSong2.Visible = false;
                txtSong.Visible = true;
                txtSong.Text = txtSong1.Text.Trim() + " " + txtSong2.Text.Trim();
                txtSong1.Text = "";
                txtSong2.Text = "";
                song1 = 0;
                song2 = 0;
                txtSong.Focus();
                txtSong.SelectionStart = txtSong.Text.Length;
                SplitJoinSong.Text = "SPLIT";
            }
            picVisualizer.Invalidate();
        }

        private void SplitJoinArtist_Click(object sender, EventArgs e)
        {
            if (SplitJoinArtist.Text == "SPLIT")
            {
                if (string.IsNullOrWhiteSpace(txtArtist.Text)) return;
                int divider;
                if (txtArtist.Text.Contains("("))
                {
                    divider = txtArtist.Text.IndexOf("(", StringComparison.Ordinal) - 1;
                }
                else if (txtArtist.Text.Contains("feat"))
                {
                    divider = txtArtist.Text.IndexOf("feat", StringComparison.Ordinal) - 1;
                }
                else if (txtArtist.Text.Contains("ft."))
                {
                    divider = txtArtist.Text.IndexOf("ft.", StringComparison.Ordinal) - 1;
                }
                else
                {
                    divider = txtArtist.Text.Length/2;
                    divider = txtArtist.Text.IndexOf(" ", divider, StringComparison.Ordinal);
                }
                if (divider <= 0)
                {
                    divider = txtArtist.Text.IndexOf(" ", StringComparison.Ordinal);
                }
                if (divider <= 0)
                {
                    MessageBox.Show("What do you want me to split?\nI can't find a space in the artist/band name!",
                                    Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                txtArtist1.Visible = true;
                txtArtist2.Visible = true;
                txtArtist.Visible = false;
                txtArtist1.Text = txtArtist.Text.Substring(0, divider).Trim();
                txtArtist2.Text = txtArtist.Text.Substring(divider + 1, txtArtist.Text.Length - divider - 1).Trim();
                txtArtist.Text = "";
                txtArtist2.Focus();
                txtArtist2.SelectionStart = txtArtist2.Text.Length;
                SplitJoinArtist.Text = "JOIN";
            }
            else
            {
                txtArtist1.Visible = false;
                txtArtist2.Visible = false;
                txtArtist.Visible = true;
                txtArtist.Text = txtArtist1.Text.Trim() + " " + txtArtist2.Text.Trim();
                txtArtist1.Text = "";
                txtArtist2.Text = "";
                artist1 = 0;
                artist2 = 0;
                txtArtist.Focus();
                txtArtist.SelectionStart = txtArtist.Text.Length;
                SplitJoinArtist.Text = "SPLIT";
            }
            picVisualizer.Invalidate();
        }

        private void SplitJoinSong_TextChanged(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(SplitJoinSong,SplitJoinSong.Text == "SPLIT"? "Click here to split the name of the song into two lines": "Click here to join the two lines into one");
        }

        private void SplitJoinArtist_TextChanged(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(SplitJoinArtist,SplitJoinArtist.Text == "SPLIT"? "Click here to split the artist / band name into two lines": "Click here to join the two lines into one");
        }

        private void CheckLoadFonts()
        {
            calibriToolStrip.Enabled = isFontAvailable(new string[] { "Calibri" });
            tahomaToolStrip.Enabled = isFontAvailable(new string[] { "Tahoma" });
            timesNewRomanToolStrip.Enabled = isFontAvailable(new string[] { "Times New Roman" });
            verdanaToolStrip.Enabled = isFontAvailable(new string[] { "Verdana" });
            segoeUIToolStrip.Enabled = isFontAvailable(new string[] { "Segoe UI" });
            myriadProToolStrip.Enabled = isFontAvailable(new string[] { "Myriad Pro", "Myriad Pro Bold", "MyriadPro-Bold" }, true);

            if (File.Exists(Application.StartupPath + "\\res\\font.txt"))
            {
                var sr = new StreamReader(Application.StartupPath + "\\res\\font.txt");
                var fontName = Tools.GetConfigString(sr.ReadLine()).Replace("\"","");
                sr.Dispose();

                if (isFontAvailable(new string[] { fontName }))
                {
                    customFontToolStrip.Text = "Custom Font: " + fontName;
                    customFontToolStrip.Visible = true;
                    customFontToolStrip.Checked = true;
                    CustomFontName = fontName;
                }
                else
                {
                    MessageBox.Show("Found custom font file font.txt and it's asking for font '" + fontName +
                        "'\nbut Windows is telling me that font is not installed on your system\nPlease make sure the font name is spelled correctly and that it is actually installed on your machine",
                        Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }

            switch (ActiveFont)
            {
                case "Myriad Pro":
                case "Myriad Pro Bold":
                case "MyriadPro-Bold":
                    myriadProToolStrip.Checked = true;
                    break;
                case "Calibri":
                    calibriToolStrip.Checked = true;
                    break;
                case "Tahoma":
                    tahomaToolStrip.Checked = true;
                    break;
                case "Times New Roman":
                    timesNewRomanToolStrip.Checked = true;
                    break;
                case "Verdana":
                    verdanaToolStrip.Checked = true;
                    break;
                case "Segoe UI":
                    segoeUIToolStrip.Checked = true;
                    break;
                default:
                    calibriToolStrip.Checked = true;
                    break;
            }            
        }

        private void LoadConfig()
        {
            if (!File.Exists(config)) return;
            var preview = true;
            var loop = true;
            var autoplay = false;
            var sr = new StreamReader(config);
            try
            {
                radioImgur.Checked = sr.ReadLine().Contains("True");
                radioLocal.Checked = sr.ReadLine().Contains("True") && !radioImgur.Checked;
                preview = sr.ReadLine().Contains("True");
                loop = sr.ReadLine().Contains("True");
                autoplay = sr.ReadLine().Contains("True");
                VolumeLevel = Convert.ToDouble(Tools.GetConfigString(sr.ReadLine()));
                sr.ReadLine();//no longer used
                UserProfile = Tools.GetConfigString(sr.ReadLine());
                autoloadLastProfile.Checked = sr.ReadLine().Contains("True");
                ActiveFont = Tools.GetConfigString(sr.ReadLine());
            }
            catch (Exception)
            {}
            sr.Dispose();
            if (preview)
            {
                picPreview.Image = Tools.NemoLoadImage(Application.StartupPath + "\\res\\play\\dopreview.png");
            }
            else
            {
                UpdateInfoPreview();
            }
            if (loop)
            {
                picLoop.Image = Tools.NemoLoadImage(Application.StartupPath + "\\res\\play\\loop.png");
            }
            else
            {
                UpdateInfoLoop();
            }
            if (autoplay)
            {
                picAutoPlay.Image = Tools.NemoLoadImage(Application.StartupPath + "\\res\\play\\autoplay.png");
            }
            else
            {
                UpdateInfoAutoPlay();
            }
            if (autoloadLastProfile.Checked && !string.IsNullOrWhiteSpace(UserProfile) && File.Exists(UserProfile))
            {
                loadProfile();
            }
        }

        private void SaveConfig()
        {
            var sw = new StreamWriter(config, false);
            sw.WriteLine("SaveToImgur=" + radioImgur.Checked);
            sw.WriteLine("SaveLocally=" + radioLocal.Checked);
            sw.WriteLine("PlayPreviewOnly=" + (picPreview.Tag.ToString() == "preview"));
            sw.WriteLine("LoopPlayback=" + (picLoop.Tag.ToString() == "loop"));
            sw.WriteLine("AutoPlay=" + (picAutoPlay.Tag.ToString() == "autoplay"));
            sw.WriteLine("VolumeLevel=" + VolumeLevel);
            sw.WriteLine("SpectrumID=0");
            sw.WriteLine("LastUsedProfile=" + (autoloadLastProfile.Checked ? UserProfile : ""));
            sw.WriteLine("AutoLoadProfile=" + autoloadLastProfile.Checked);
            sw.WriteLine("ActiveFont=" + ActiveFont);
            sw.Dispose();
        }

        private async void Visualizer_Shown(object sender, EventArgs e)
        {
            doImages();
            LoadConfig();
            Application.DoEvents();
            CheckLoadFonts();

            if (!string.IsNullOrEmpty(inputFile))
            {
                await HandleDragDrop(inputFile);
            }

            var themes_folder = Application.StartupPath + "\\res\\vis_themes\\";
            switch (DateTime.Now.Month)
            {
                case 1:
                    if (DateTime.Now.Day < 6 && File.Exists(themes_folder + "newyears_button.png"))
                    {
                        ThemeName = "newyears";
                    }
                    break;
                case 2:
                    if (DateTime.Now.Day > 19) return;
                    if (File.Exists(themes_folder + "love_button.png"))
                    {
                        ThemeName = "love";
                    }
                    break;
                case 3:
                    if (DateTime.Now.Day > 22) return;
                    if (File.Exists(themes_folder + "stpaddy_button.png"))
                    {
                        ThemeName = "stpaddy";
                    }
                    break;
                case 5:
                    if (DateTime.Now.Day > 22) return;
                    if (File.Exists(themes_folder + "norway_button.png"))
                    {
                        ThemeName = "norway";
                    }
                    break;
                case 6:
                    if (DateTime.Now.Day > 20 && File.Exists(themes_folder + "freedom_button.png"))
                    {
                        ThemeName = "freedom";
                    }
                    else if (File.Exists(themes_folder + "summer_button.png"))
                    {
                        ThemeName = "summer";
                    }
                    break;
                case 7:
                    if (DateTime.Now.Day > 9 && File.Exists(themes_folder + "summer_button.png"))
                    {
                        ThemeName = "summer";
                    }
                    else if (File.Exists(themes_folder + "freedom_button.png"))
                    {
                        ThemeName = "freedom";
                    }
                    break;
                case 8:
                    if (File.Exists(themes_folder + "freedom_button.png"))
                    {
                        ThemeName = "freedom";
                    }
                    break;
                case 10:
                    if (File.Exists(themes_folder + "spooky_button.png"))
                    {
                        ThemeName = "spooky";
                    }
                    break;
                case 11:
                    if (DateTime.Now.Day < 6 && File.Exists(themes_folder + "spooky_button.png"))
                    {
                        ThemeName = "spooky";
                    }
                    break;
                case 12:
                    if (DateTime.Now.Day < 25 && File.Exists(themes_folder + "xmas_button.png"))
                    {
                        ThemeName = "xmas";
                    }
                    else if (File.Exists(themes_folder + "newyears_button.png"))
                    {
                        ThemeName = "newyears";
                    }
                    break;
            }
            picVisualizer.Invalidate();

            if (!string.IsNullOrWhiteSpace(ThemeName))
            {
                try
                {
                    picShowTheme.Image = Tools.NemoLoadImage(themes_folder + ThemeName + "_button.png");
                    picShowTheme.Visible = true;
                    UseOverlay = true;
                    picVisualizer.Invalidate();
                }
                catch (Exception)
                {
                    MessageBox.Show("Error loading theme button image + " + ThemeName +
                        "_button.png\nMake sure the files are named correctly and the files are in the res\\vis_themes\\ directory", Text,
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    ThemeName = "";
                }
            }            
        }

        private void GetLogo(string image_path)
        {
            if ((!image_path.Contains(".jpg") && !image_path.Contains(".bmp") && !image_path.Contains(".png") &&
                 !image_path.Contains(".jpeg")) || image_path.Contains(".png_xbox") || image_path.Contains(".png_wii"))
                return;
            picLogo.BorderStyle = BorderStyle.None;
            RESOURCE_AUTHOR_LOGO = Tools.NemoLoadImage(image_path);
            Tools.CurrentFolder = Path.GetDirectoryName(image_path);
            picVisualizer.Invalidate();
        }

        private void picLogo_MouseMove(object sender, MouseEventArgs e)
        {
            if (picLogo.Cursor != Cursors.NoMove2D) return;
            if (MousePosition.X != mouseX)
            {
                if (MousePosition.X > mouseX)
                {
                    picLogo.Left = picLogo.Left + (MousePosition.X - mouseX);
                }
                else if (MousePosition.X < mouseX)
                {
                    picLogo.Left = picLogo.Left - (mouseX - MousePosition.X);
                }
                mouseX = MousePosition.X;
            }
            if (MousePosition.Y != mouseY)
            {
                if (MousePosition.Y > mouseY)
                {
                    picLogo.Top = picLogo.Top + (MousePosition.Y - mouseY);
                }
                else if (MousePosition.Y < mouseY)
                {
                    picLogo.Top = picLogo.Top - (mouseY - MousePosition.Y);
                }
                mouseY = MousePosition.Y;
            }
            if (picLogo.Top < (-1 * picLogo.Height) + 5)
            {
                picLogo.Top = (-1 * picLogo.Height) + 5;
            }
            else if (picLogo.Top > picAlbumArt.Height - 5)
            {
                picLogo.Top = picAlbumArt.Height - 5;
            }
            if (picLogo.Left < (-1 * picLogo.Width) + 5)
            {
                picLogo.Left = (-1 * picLogo.Width) + 5;
            }
            else if (picLogo.Left > picAlbumArt.Width - 5)
            {
                picLogo.Left = picAlbumArt.Width - 5;
            }
            picVisualizer.Invalidate();
        }

        private void clearLogoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RESOURCE_AUTHOR_LOGO = null;
            picLogo.BorderStyle = BorderStyle.Fixed3D;
            picLogo.Location = logoLocation;
            picVisualizer.Invalidate();
        }
        
        private void txtYear2_TextChanged(object sender, EventArgs e)
        {
            picVisualizer.Invalidate();
            if (reset) return;
            MeasureAlbum();
        }

        private void TextBoxSelectAll(object sender, EventArgs e)
        {
            var tb = (TextBox) sender;
            tb.SelectAll();
        }
        
        private void picLogo_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[]) e.Data.GetData(DataFormats.FileDrop);
            GetLogo(files[0]);
        }

        private void cboRating_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cboRating.SelectedIndex + 1)
            {
                case 1:
                    Rating = "FF";
                    RatingColor = Color.LimeGreen;
                    break;
                case 2:
                    Rating = "SR";
                    RatingColor = Color.Yellow;
                    break;
                case 3:
                    Rating = "M";
                    RatingColor = Color.Red;
                    break;
                case 4:
                    Rating = "";
                    break;
            }            
            picVisualizer.Invalidate();
        }

        private void chkRating_CheckedChanged(object sender, EventArgs e)
        {
            cboRating.Enabled = chkRating.Checked;            
            picVisualizer.Invalidate();
        }
        
        private void picImage_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            getImage("");
        }

        private void picLock_MouseClick(object sender, MouseEventArgs e)
        {
            var selector = new ThemeSelector(this);
            selector.ShowDialog();
        }

        public void UpdateTheme()
        {
            UseOverlay = true;
            picShowTheme.Image = Tools.NemoLoadImage(Application.StartupPath + "\\res\\vis_themes\\" + ThemeName + "_button.png");
            RESOURCE_THEME = Tools.NemoLoadImage(Application.StartupPath + "\\res\\vis_themes\\" + ThemeName + "_overlay.png");
            picShowTheme.Visible = true;
            picVisualizer.Invalidate();
        }

        private void picSpecial_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            RESOURCE_THEME = Tools.NemoLoadImage(Application.StartupPath + "\\res\\vis_themes\\" + ThemeName + "_overlay.png");
            UseOverlay = !UseOverlay;
            picVisualizer.Invalidate();
        }
                        
        private void proGuitar_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            proGuitar.Tag = proGuitar.Tag.ToString() == "1" ? proGuitar.Tag = "0" : proGuitar.Tag = "1";
            picVisualizer.Invalidate();
        }

        private void proBass_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            proBass.Tag = proBass.Tag.ToString() == "1" ? proBass.Tag = "0" : proBass.Tag = "1";
            picVisualizer.Invalidate();
        }

        private void pic2x_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            pic2x.Tag = pic2x.Tag.ToString() == "1" ? pic2x.Tag = "0" : pic2x.Tag = "1";
            picVisualizer.Invalidate();
        }

        private void picHarm_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            switch (intVocals)
            {
                case 1:
                    intVocals = 2;
                    break;
                case 2:
                    intVocals = 3;
                    break;
                default:
                    intVocals = 1;
                    break;
            }
            picVisualizer.Invalidate();
        }

        private void proKeys_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            proKeys.Tag = proKeys.Tag.ToString() == "1" ? proKeys.Tag = "0" : proKeys.Tag = "1";
            picVisualizer.Invalidate();
        }
        
        private void picGenreDown_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            genreY = 130f;
            picVisualizer.Invalidate();
        }

        private void picGenreUp_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (string.IsNullOrWhiteSpace(txtAlbum.Text))
            {
                genreY = albumY;
            }
            picVisualizer.Invalidate();
        }

        private void artistJoystick_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (string.IsNullOrWhiteSpace(txtArtist.Text) && string.IsNullOrWhiteSpace(txtArtist1.Text) && string.IsNullOrWhiteSpace(txtArtist2.Text)) return;
            mouseX = MousePosition.X;
            mouseY = MousePosition.Y;
            if (artistJoystick.Cursor == Cursors.Hand)
            {
                artistJoystick.Image = null;
                artistJoystick.Cursor = Cursors.NoMove2D;
            }
            else if (artistJoystick.Cursor == Cursors.NoMove2D)
            {
                artistJoystick.Cursor = Cursors.Hand;
                artistJoystick.Image = Resources.moveall;
            }
        }

        private void songJoystick_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (string.IsNullOrWhiteSpace(txtSong.Text) && string.IsNullOrWhiteSpace(txtSong1.Text) && string.IsNullOrWhiteSpace(txtSong2.Text)) return;
            mouseX = MousePosition.X;
            mouseY = MousePosition.Y;
            if (songJoystick.Cursor == Cursors.Hand)
            {
                songJoystick.Image = null;
                songJoystick.Cursor = Cursors.NoMove2D;
            }
            else if (songJoystick.Cursor == Cursors.NoMove2D)
            {
                songJoystick.Cursor = Cursors.Hand;
                songJoystick.Image = Resources.moveall;
            }
        }             
        
        private void UncheckAllFonts(ToolStripMenuItem menu)
        {
            myriadProToolStrip.Checked = false;
            calibriToolStrip.Checked = false;
            tahomaToolStrip.Checked = false;
            timesNewRomanToolStrip.Checked = false;
            customFontToolStrip.Checked = false;
            verdanaToolStrip.Checked = false;
            segoeUIToolStrip.Checked = false;
            menu.Checked = true;
            picVisualizer.Invalidate();
        }

        private void calibriToolStrip_Click(object sender, EventArgs e)
        {
            ActiveFont = "Calibri";
            UncheckAllFonts((ToolStripMenuItem)sender);
        }

        private void tahomaToolStrip_Click(object sender, EventArgs e)
        {
            ActiveFont = "Tahoma";
            UncheckAllFonts((ToolStripMenuItem)sender);
        }

        private void timesNewRomanToolStrip_Click(object sender, EventArgs e)
        {
            ActiveFont = "Times New Roman";
            UncheckAllFonts((ToolStripMenuItem)sender);
        }

        private void customFontToolStrip_Click(object sender, EventArgs e)
        {
            ActiveFont = CustomFontName;
            UncheckAllFonts((ToolStripMenuItem)sender);
        }

        private void myriadProToolStrip_Click(object sender, EventArgs e)
        {
            if (isFontAvailable(new string[] { "Myriad Pro", "Myriad Pro Bold", "MyriadPro-Bold" }, true))
            {
                UncheckAllFonts((ToolStripMenuItem)sender);
            }
            else
            {
                if (MessageBox.Show("Myriad Pro Bold is our default font, but Windows is telling me it's not installed on your system\nClick 'OK' to open the /res folder, then install 'nautilus.otf' and restart Nautilus\nClick 'Cancel' to go back and choose a different font",
                        Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                {
                    Process.Start(Application.StartupPath + "\\res\\");
                }
            }
        }

        private void StopPlayback(bool Pause = false)
        {
            try
            {
                PlaybackTimer.Enabled = false;
                if (Pause)
                {
                    if (!Bass.BASS_ChannelPause(BassMixer))
                    {
                        MessageBox.Show("Error pausing playback\n" + Bass.BASS_ErrorGetCode());
                    }
                    if (autoplayMusicVideos.Checked && _mediaPlayer.IsPlaying)
                    {
                        _mediaPlayer.Pause();
                    }
                }
                else
                {
                    StopBASS();
                    if (autoplayMusicVideos.Checked && _mediaPlayer.IsPlaying)
                    {
                        _mediaPlayer.Stop();
                    }
                }
            }
            catch (Exception)
            {}
            picPlayPause.Image = Tools.NemoLoadImage(Application.StartupPath + "\\res\\play\\play.png");
            picPlayPause.Tag = "play";
        }

        private void StopBASS()
        {
            try
            {
                Bass.BASS_ChannelStop(BassMixer);
                Bass.BASS_StreamFree(BassMixer);
                Bass.BASS_Free();
                Bass.BASS_StreamFree(BassStream);
                foreach (var stream in BassStreams)
                {
                    Bass.BASS_StreamFree(stream);
                }
                BASS_INIT = false;
            }
            catch (Exception)
            { }
            picSpect.Image = null;
        }

        private void SetPlayLocation(double time, bool setVideo = true)
        {
            if (time < 0)
            {
                time = 0.0;
            }
            if (isOpus || isOgg || isMP3 || isWAV)
            {
                foreach (var stream in BassStreams)
                {
                    try
                    {
                        BassMix.BASS_Mixer_ChannelSetPosition(stream, Bass.BASS_ChannelSeconds2Bytes(stream, time));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error setting play location: " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                try
                {
                    BassMix.BASS_Mixer_ChannelSetPosition(BassStream, Bass.BASS_ChannelSeconds2Bytes(BassStream, time));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error setting play location: " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }               
            }
            if (!setVideo) return;
            if (autoplayMusicVideos.Checked && _mediaPlayer.Media != null && _mediaPlayer.IsSeekable)
            {
                _mediaPlayer.Time = (long)(time * 1000) + Parser.Songs[0].VideoStartTime;
            }
        }

        private string GetStemsToPlay()
        {
            var stems = "";
            if (PlayDrums)
            {
                stems += "drums|";
            }
            if (PlayBass)
            {
                stems += "bass|";
            }
            if (PlayGuitar)
            {
                stems += "guitar|";
            }
            if (PlayVocals)
            {
                stems += "vocals|";
            }
            if (PlayKeys)
            {
                stems += "keys|";
            }
            if (PlayBacking)
            {
                stems += "backing|";
            }
            if (PlayCrowd)
            {
                stems += "crowd";
            }
            return stems;
        }

        private void InitBass()
        {
            if (BASS_INIT) return;
            //initialize BASS.NET
            if (!Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, Handle))
            {
                if (Bass.BASS_ErrorGetCode() == BASSError.BASS_ERROR_ALREADY)
                {
                    BASS_INIT = true;
                    return;
                }
                MessageBox.Show("Error initializing BASS.NET\n" + Bass.BASS_ErrorGetCode());
                return;
            }
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_BUFFER, BassBuffer);
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD, 50);
            BASS_INIT = true;
        }

        private void StartPlayback(bool ignorePlaybackInstruments = false)
        {
            InitBass();

            if (!isOpus && !isOgg && !isMP3 && !isWAV)
            {
                if (!isM4A && nautilus3.PlayingSongOggData.Count() == 0)
                {
                    MessageBox.Show("Couldn't play back that audio file, sorry", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    StopPlayback();
                    return;
                }

                if (isM4A)
                {
                    BassStream = Bass.BASS_StreamCreateFile(tempFile, 0L, File.ReadAllBytes(tempFile).Length, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);
                    if (BassStream == 0)
                    {
                        MessageBox.Show("That is not a valid .m4a input file", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        File.Delete(tempFile);
                        Bass.BASS_ChannelFree(BassStream);
                        return;
                    }
                    
                    var len = Bass.BASS_ChannelGetLength(BassStream);
                    var totaltime = Bass.BASS_ChannelBytes2Seconds(BassStream, len); // the total time length
                    songLength = (int)(totaltime * 1000);
                    lblSongLength.Text = Parser.GetSongDuration(songLength.ToString(CultureInfo.InvariantCulture));
                    txtTime.Text = lblSongLength.Text;
                }
                else
                {
                    BassStream = Bass.BASS_StreamCreateFile(nautilus3.GetOggStreamIntPtr(), 0L, nautilus3.PlayingSongOggData.Length, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);
                }
                var channel_info = Bass.BASS_ChannelGetInfo(BassStream);
                if (channel_info == null && !isM4A)
                {
                    MessageBox.Show("Error getting stream info\n\nBASS status: " + Bass.BASS_ErrorGetCode(), Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                // create a stereo mixer with same frequency rate as the input file
                if (exportYARGAudio.Checked && isYARG)
                {
                    BassMixer = BassMix.BASS_Mixer_StreamCreate(channel_info.freq, 2, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_MIXER_END);
                }
                else if (exportFNFAudio.Checked && isM4A)
                {
                    BassMixer = BassMix.BASS_Mixer_StreamCreate(channel_info.freq, 2, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_MIXER_END);
                }
                else if (isM4A)
                {
                    BassMixer = BassMix.BASS_Mixer_StreamCreate(48000, 2, BASSFlag.BASS_MIXER_END);
                }
                else
                {
                    BassMixer = BassMix.BASS_Mixer_StreamCreate(channel_info.freq, 2, BASSFlag.BASS_MIXER_END);
                }
                BassMix.BASS_Mixer_StreamAddChannel(BassMixer, BassStream, BASSFlag.BASS_MIXER_MATRIX);

                var matrix = new float[2, isM4A ? 10 : channel_info.chans];
                var splitter = new MoggSplitter();
                matrix = splitter.GetChannelMatrix(Parser.Songs[0], isM4A ? 10 : channel_info.chans, GetStemsToPlay());
                BassMix.BASS_Mixer_ChannelSetMatrix(BassStream, matrix);                
            }                   
            else
            {
                PrepMixerCH(ignorePlaybackInstruments);
            }

            if ((exportYARGAudio.Checked && isYARG) || (exportFNFAudio.Checked && isM4A))
            {
                SaveAudioToFile(true);
                return;
            }

            if ((isMP3 && exportGHWTDEAudio.Checked) || (isWAV && exportPowerGigAudio.Checked && !isBandFuse) || (isBandFuse && exportBandFuseAudio.Checked))
            {
                SaveAudioToFile();
                return;
            }

            //apply volume correction to entire track
            SetPlayLocation(PlaybackSeconds, false);
            var track_vol = (float)Utils.DBToLevel(Convert.ToDouble(-1 * VolumeLevel), 1.0);
            if (picPreview.Tag.ToString() == "preview")
            {
                double previewStart = isM4A || isBandFuse || isGHWTDE || isRS2014 ? 30.0 : (double)(Parser.Songs[0].PreviewStart / 1000);                
                if (PlaybackSeconds >= previewStart && PlaybackSeconds <= previewStart + FadeTime && !volSlide)
                {
                    Bass.BASS_ChannelSetAttribute(BassMixer, BASSAttribute.BASS_ATTRIB_VOL, 0);
                    Bass.BASS_ChannelSlideAttribute(BassMixer, BASSAttribute.BASS_ATTRIB_VOL, track_vol, (int)(FadeTime * 1000));
                    volSlide = true;
                }
            }         
            else
            {
                volSlide = false;
            }
            if (!volSlide)
            {
                Bass.BASS_ChannelSetAttribute(BassMixer, BASSAttribute.BASS_ATTRIB_VOL, track_vol);
            }
                        
            SetPlayLocation(PlaybackSeconds, false);
            UpdateTime();
            //start mix playback
            Bass.BASS_ChannelPlay(BassMixer, false);
            PlayVideo();
            PlaybackTimer.Enabled = true;
            picPlayPause.Image = Tools.NemoLoadImage(Application.StartupPath + "\\res\\play\\pause.png");
            picPlayPause.Tag = "pause";
        }               

        private void PlayVideo()
        {
            if (autoplayMusicVideos.Checked && _mediaPlayer.Media != null)
            {
                _mediaPlayer.Play();
                if (_mediaPlayer.IsSeekable)
                {
                    _mediaPlayer.Time = (long)(PlaybackSeconds * 1000) + Parser.Songs[0].VideoStartTime;
                }
            }
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(sCON) || Parser.Songs == null) return;
            var Splitter = new MoggSplitter();
            try
            {
                if (!Splitter.ExtractDecryptMogg(sCON, nautilus3, Parser))
                {
                    nautilus3.PlayingSongOggData = null;
                }
            }
            catch (Exception)
            {
                nautilus3.PlayingSongOggData = null;
            }                        
        }

        private void SaveAudioToFile(bool isMOGG = false)
        {            
            int encoder = 0;
            var fileOutput = new SaveFileDialog
            {
                Filter = "FLAC Audio (*.flac)|*.flac|MP3 Audio (*.mp3)|*.mp3|OGG Audio (*.ogg)|*.ogg|OPUS Audio (*.opus)|*.opus|WAV Audio (*.wav)|.*wav",
                Title = "Where should I save the audio file to?",
                FileName = Parser.Songs[0].Artist + " - " + Parser.Songs[0].Name,
                InitialDirectory = Environment.CurrentDirectory,
            };           
            
            if (fileOutput.ShowDialog() == DialogResult.OK)
            {
                if (isMOGG)
                {
                    BassMix.BASS_Mixer_ChannelSetPosition(BassStream, Bass.BASS_ChannelSeconds2Bytes(BassStream, 0));
                }
                else
                {
                    foreach (var stream in BassStreams)
                    {
                        BassMix.BASS_Mixer_ChannelSetPosition(stream, Bass.BASS_ChannelSeconds2Bytes(stream, 0));
                    }
                }
                string arg = "";                
                switch (fileOutput.FilterIndex)//starts count at 1 not 0
                {
                    default:
                    case 1://Flac:
                        arg = "--compression-level-5 --fast -T \"TITLE=" + Parser.Songs[0].Name + "\" -T \"ARTIST=" + Parser.Songs[0].Artist + "\" -T \"GENRE=" + Parser.Songs[0].Genre + "\" -T \"YEAR=" + Parser.Songs[0].YearReleased + "\" -T \"TRACKNUMBER=" + Parser.Songs[0].TrackNumber + "\" -T \"ALBUM=" + Parser.Songs[0].Album + "\" --picture \"" + picAlbumArt.Tag + "\" -T \"COMMENT=Made by Nemo\"";
                        encoder = BassEnc_Flac.BASS_Encode_FLAC_StartFile(BassMixer, arg, BASSEncode.BASS_ENCODE_DEFAULT | BASSEncode.BASS_ENCODE_AUTOFREE, fileOutput.FileName);
                        break;
                    case 2://MP3:
                        arg = "-b 320 --add-id3v2 --ignore-tag-errors --tt \"" + Parser.Songs[0].Name + "\" --ta \"" + Parser.Songs[0].Artist + "\" --ty " + Parser.Songs[0].YearReleased + " --tg \"" + Parser.Songs[0].Genre + "\" --tl \"" + Parser.Songs[0].Album + "\" --tn \"" + Parser.Songs[0].TrackNumber + "\" --ti \"" + picAlbumArt.Tag + "\" --tc \"Made by Nemo\"";
                        encoder = BassEnc_Mp3.BASS_Encode_MP3_StartFile(BassMixer, arg, BASSEncode.BASS_UNICODE | BASSEncode.BASS_ENCODE_AUTOFREE, fileOutput.FileName);
                        break;
                    case 3://Ogg:
                        arg = "-q 5 -t \"" + Parser.Songs[0].Name + "\" -a \"" + Parser.Songs[0].Artist + "\" -G \"" + Parser.Songs[0].Genre + "\" -d \"" + Parser.Songs[0].YearReleased + "\" -I \"" + Parser.Songs[0].Album + "\" -N \"" + Parser.Songs[0].TrackNumber + "\" --picture \"" + picAlbumArt.Tag + "\" -c \"COMMENT=Made by Nemo\"";
                        encoder = BassEnc_Ogg.BASS_Encode_OGG_StartFile(BassMixer, arg, BASSEncode.BASS_ENCODE_AUTOFREE, fileOutput.FileName);
                        break;
                    case 4://Opus:
                        arg = "--vbr --music --title \"" + Parser.Songs[0].Name + "\" --artist \"" + Parser.Songs[0].Artist + "\" --album \"" + Parser.Songs[0].Album + "\" --genre \"" + Parser.Songs[0].Genre + "\" --date \"" + Parser.Songs[0].YearReleased + "\" --tracknumber \"" + Parser.Songs[0].TrackNumber + "\" --picture \"" + picAlbumArt.Tag + "\" --comment COMMENT=\"Made by Nemo\"";
                        encoder = BassEnc_Opus.BASS_Encode_OPUS_StartFile(BassMixer, arg, BASSEncode.BASS_ENCODE_DEFAULT | BASSEncode.BASS_ENCODE_AUTOFREE, fileOutput.FileName);
                        break;
                    case 5://WAV:
                        encoder = BassEnc.BASS_Encode_Start(BassMixer, fileOutput.FileName, BASSEncode.BASS_ENCODE_PCM | BASSEncode.BASS_ENCODE_AUTOFREE, null, IntPtr.Zero);
                        break;
                }
                while (true)
                {
                    var buffer = new byte[20000];
                    var c = Bass.BASS_ChannelGetData(BassMixer, buffer, buffer.Length);
                    if (c <= 0) break;
                }
                BassEnc.BASS_Encode_Stop(encoder);
                if (File.Exists(fileOutput.FileName))
                {
                    Process.Start("explorer.exe", "/select, \"" + fileOutput.FileName + "\"");
                }
                Environment.CurrentDirectory = Path.GetDirectoryName(fileOutput.FileName);

                loadDefaults();
            }
        }
       
        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            ProcessAudio();
        }

        private void ProcessAudio()
        {
            //analyze mogg file for length if not present in DTA
            if (Parser.Songs[0].Length == 0 && nautilus3.PlayingSongOggData != null && nautilus3.PlayingSongOggData.Count() > 0)
            {
                try
                {
                    InitBass();
                    var stream = Bass.BASS_StreamCreateFile(nautilus3.GetOggStreamIntPtr(), 0L, nautilus3.PlayingSongOggData.Length, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);
                    var len = Bass.BASS_ChannelGetLength(stream);
                    var totaltime = Bass.BASS_ChannelBytes2Seconds(stream, len); // the total time length
                    Parser.Songs[0].Length = (int)(totaltime * 1000);
                    lblSongLength.Text = Parser.GetSongDuration(Parser.Songs[0].Length.ToString(CultureInfo.InvariantCulture));
                    txtTime.Text = lblSongLength.Text;
                    StopBASS();
                }
                catch (Exception)
                { }
            }
            if (nautilus3.PlayingSongOggData != null && nautilus3.PlayingSongOggData.Count() > 0)
            {
                picPlayPause.Cursor = Cursors.Hand;
                picStop.Cursor = Cursors.Hand;
                if (picAutoPlay.Tag.ToString() != "autoplay" || audioProcessor.CancellationPending) return;
                PlaybackSeconds = Parser.Songs[0].PreviewStart == 0 || picPreview.Tag.ToString() == "song" ? 0 : (double)Parser.Songs[0].PreviewStart / 1000;
                StartPlayback();
            }
            else
            {
                picPlayPause.Cursor = Cursors.No;
                picStop.Cursor = Cursors.No;
            }
        }

        private void ChangePlaybackInstruments(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            var pic = (PictureBox) sender;
            var enabled = true;
            var instrument = pic.Tag.ToString();
            switch (instrument)
            {
                case "drums":
                    PlayDrums = !PlayDrums;
                    enabled = PlayDrums;
                    break;
                case "bass":
                    PlayBass = !PlayBass;
                    enabled = PlayBass;
                    break;
                case "guitar":
                    PlayGuitar = !PlayGuitar;
                    enabled = PlayGuitar;
                    break;
                case "vocals":
                    PlayVocals = !PlayVocals;
                    enabled = PlayVocals;
                    break;
                case "keys":
                    PlayKeys = !PlayKeys;
                    enabled = PlayKeys;
                    break;
                case "crowd":
                    PlayCrowd = !PlayCrowd;
                    enabled = PlayCrowd;
                    break;
                case "backing":
                    PlayBacking = !PlayBacking;
                    enabled = PlayBacking;
                    break;
            }
            var image = Application.StartupPath + "\\res\\play\\" + (enabled ? "" : "no") + instrument + ".png";
            try
            {
                pic.Image = Tools.NemoLoadImage(image);
            }
            catch (Exception)
            {
                MessageBox.Show("There was an error loading the playback image:\n" + image + "\n\nMake sure you haven't deleted any files", Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            toolTip1.SetToolTip(pic, (enabled? "Disable" : "Enable") + " " + pic.Tag + " track");
            if (picPlayPause.Tag.ToString() != "pause") return;

            StopPlayback();            
            StartPlayback(isOpus || isOgg || isMP3 || isWAV);
        }
        
        private void picPlayPause_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (picPlayPause.Cursor == Cursors.No)
            {
                MessageBox.Show("Can't play audio for this song, most likely it is encrypted", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (picPlayPause.Cursor == Cursors.WaitCursor)
            {
                MessageBox.Show("Please wait while I extract the audio from the CON file", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (!PlayDrums && !PlayBass && !PlayGuitar && !PlayKeys && !PlayVocals && !PlayCrowd && !PlayBacking)
            {
                MessageBox.Show("Enable at least one track to play", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            try
            {
                switch (Bass.BASS_ChannelIsActive(BassMixer))
                {
                    case BASSActive.BASS_ACTIVE_PLAYING:
                        StopPlayback(true);
                        UpdateTime();
                        break;
                    case BASSActive.BASS_ACTIVE_PAUSED:
                        Bass.BASS_ChannelPlay(BassMixer, false);
                        var state = _mediaPlayer.State;
                        if (state == VLCState.Paused)
                        {
                            PlayVideo();
                        }
                        PlaybackTimer.Enabled = true;
                        picPlayPause.Image = Tools.NemoLoadImage(Application.StartupPath + "\\res\\play\\pause.png");
                        picPlayPause.Tag = "pause";
                        toolTip1.SetToolTip(picPlayPause, "Pause");
                        break;
                    default:
                        StartPlayback();                        
                        break;
                }
            }
            catch (Exception)
            {
                StartPlayback();
            }
        }

        private void picStop_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (picPlayPause.Cursor == Cursors.No || picPlayPause.Cursor == Cursors.WaitCursor) return;
            StopPlayback();
            PlaybackSeconds = Parser.Songs[0].PreviewStart == 0 || picPreview.Tag.ToString() == "song" ? 0 : (double)Parser.Songs[0].PreviewStart / 1000;
            UpdateTime();
        }
        
        private void picLoop_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            UpdateInfoLoop();
        }

        private void UpdateInfoLoop()
        {
            if (picLoop.Tag.ToString() == "loop")
            {
                picLoop.Tag = "noloop";
                picLoop.Image = Tools.NemoLoadImage(Application.StartupPath + "\\res\\play\\loop_off.png");
                toolTip1.SetToolTip(picLoop, "Enable looping of playback");
            }
            else
            {
                picLoop.Tag = "loop";
                picLoop.Image = Tools.NemoLoadImage(Application.StartupPath + "\\res\\play\\loop.png");
                toolTip1.SetToolTip(picLoop, "Disable looping of playback");
            }
        }

        private void picAutoPlay_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            UpdateInfoAutoPlay();
        }

        private void UpdateInfoAutoPlay()
        {
            if (picAutoPlay.Tag.ToString() == "autoplay")
            {
                picAutoPlay.Tag = "noautoplay";
                picAutoPlay.Image = Tools.NemoLoadImage(Application.StartupPath + "\\res\\play\\autoplay_off.png");
                toolTip1.SetToolTip(picAutoPlay, "Enable auto-play");
            }
            else
            {
                picAutoPlay.Tag = "autoplay";
                picAutoPlay.Image = Tools.NemoLoadImage(Application.StartupPath + "\\res\\play\\autoplay.png");
                toolTip1.SetToolTip(picAutoPlay, "Disable auto-play");
            }
        }

        private void picPreview_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            UpdateInfoPreview();
            try
            {
                PlaybackSeconds = Parser.Songs[0].PreviewStart == 0 || picPreview.Tag.ToString() == "song" ? 0 : (double)Parser.Songs[0].PreviewStart / 1000;
                SetPlayLocation(PlaybackSeconds);
                UpdateTime();
            }
            catch (Exception)
            {
                PlaybackSeconds = 0;
            }
        }

        private void UpdateInfoPreview()
        {
            if (picPreview.Tag.ToString() == "preview")
            {
                picPreview.Tag = "song";
                picPreview.Image = Tools.NemoLoadImage(Application.StartupPath + "\\res\\play\\dosong.png");
                toolTip1.SetToolTip(picPreview, "Click to play in-game preview");
            }
            else
            {
                picPreview.Tag = "preview";
                picPreview.Image = Tools.NemoLoadImage(Application.StartupPath + "\\res\\play\\dopreview.png");
                toolTip1.SetToolTip(picPreview, "Click to play entire song");
            }
        }

        private void UpdateTime()
        {
            string time;
            if (PlaybackSeconds >= 3600)
            {
                var hours = (int)(PlaybackSeconds / 3600);
                var minutes = (int)(PlaybackSeconds - (hours * 3600));
                var seconds = (int)(PlaybackSeconds - (minutes * 60));
                time = hours + ":" + (minutes < 10 ? "0" : "") + minutes + ":" + (seconds < 10 ? "0" : "") + seconds;
            }
            else if (PlaybackSeconds >= 60)
            {
                var minutes = (int)(PlaybackSeconds / 60);
                var seconds = (int)(PlaybackSeconds - (minutes * 60));
                time = minutes + ":" + (seconds < 10 ? "0" : "") + seconds;
            }
            else
            {
                time = "0:" + (PlaybackSeconds < 10 ? "0" : "") + (int)PlaybackSeconds;
            }
            if (lblTime.InvokeRequired)
            {
                lblTime.Invoke(new MethodInvoker(() => lblTime.Text = time));
            }
            else
            {
                lblTime.Text = time;
            }
            if (picSlider.Cursor == Cursors.NoMoveHoriz || reset) return;
            var percent = PlaybackSeconds / ((double) (isM4A ? songLength : Parser.Songs[0].Length) / 1000);
            if (picSlider.InvokeRequired)
            {
                picSlider.Invoke(new MethodInvoker(() => picSlider.Left = (int)((picLine.Width - picSlider.Width) * percent)));
            }
            else
            {
                picSlider.Left = (int)((picLine.Width - picSlider.Width) * percent);
            }
        }

        private void picSlider_MouseDown(object sender, MouseEventArgs e)
        {
            picSlider.Cursor = Cursors.NoMoveHoriz;
            mouseX = MousePosition.X;
        }

        private void picSlider_MouseUp(object sender, MouseEventArgs e)
        {
            picSlider.Cursor = Cursors.Hand;
            ManualTimeSelection();
        }

        private void ManualTimeSelection()
        {
            picPreview.Tag = "song";
            picPreview.Image = Tools.NemoLoadImage(Application.StartupPath + "\\res\\play\\dosong.png");
            if (Bass.BASS_ChannelIsActive(BassMixer) == BASSActive.BASS_ACTIVE_PAUSED ||
                Bass.BASS_ChannelIsActive(BassMixer) == BASSActive.BASS_ACTIVE_PLAYING)
            {
                SetPlayLocation(PlaybackSeconds);
            }
        }

        private void picSlider_MouseMove(object sender, MouseEventArgs e)
        {
            if (picSlider.Cursor != Cursors.NoMoveHoriz) return;
            if (MousePosition.X != mouseX)
            {
                if (MousePosition.X > mouseX)
                {
                    picSlider.Left = picSlider.Left + (MousePosition.X - mouseX);
                }
                else if (MousePosition.X < mouseX)
                {
                    picSlider.Left = picSlider.Left - (mouseX - MousePosition.X);
                }
                mouseX = MousePosition.X;
            }
            if (picSlider.Left < 0)
            {
                picSlider.Left = 0;
            }
            else if (picSlider.Left > picLine.Width - picSlider.Width)
            {
                picSlider.Left = picLine.Width - picSlider.Width;
            }
            PlaybackSeconds = (int)(((double)(isM4A ? songLength : Parser.Songs[0].Length) / 1000) * ((double)picSlider.Left / (picLine.Width - picSlider.Width)));
            SetPlayLocation(PlaybackSeconds);
            UpdateTime();
        }

        private void picLine_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            PlaybackSeconds = (int)(((double)(isM4A ? songLength : Parser.Songs[0].Length) / 1000) * ((double)(e.X - (picSlider.Width / 2)) / (picLine.Width - picSlider.Width)));
            ManualTimeSelection(); 
            UpdateTime();
        }

        private void backgroundWorker2_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            imgLink = Tools.UploadToImgur(ImageOut);
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            toolTip1.SetToolTip(lblBottom, "");
            lblBottom.Cursor = Cursors.Default;
            if (!radioLocal.Checked)
            {
                Tools.DeleteFile(ImageOut);
            }
            picWorking.Visible = false;
            lblTop.Text = "";
            if (string.IsNullOrWhiteSpace(imgLink) && File.Exists(ImageOut))
            {
                lblTop.Text = "Saved successfully";
                lblBottom.Text = "Click to view";
                imgLink = ImageOut;
                lblBottom.Cursor = Cursors.Hand;
                return;
            }
            if (string.IsNullOrWhiteSpace(imgLink) && radioImgur.Checked)
            {
                lblTop.Text = "Uploading failed!";
                lblBottom.Text = "Try again later...";
                return;
            }
            lblTop.Text = "Uploaded successfully";
            lblBottom.Text = "Click to view";
            toolTip1.SetToolTip(lblBottom, "Click here to view your uploaded image");
            lblBottom.Cursor = Cursors.Hand;
            Clipboard.SetText(imgLink);
        }

        private void lblRating_Click(object sender, EventArgs e)
        {
            chkRating.Checked = !chkRating.Checked;
        }

        private void PlaybackTimer_Tick(object sender, EventArgs e)
        {            
            try
            {
                double previewStart = isM4A || isBandFuse || isGHWTDE || isRS2014 ? 30.0 : (double)(Parser.Songs[0].PreviewStart / 1000);
                if (Bass.BASS_ChannelIsActive(BassMixer) == BASSActive.BASS_ACTIVE_PLAYING)
                {
                    // the stream is still playing...
                    var pos = Bass.BASS_ChannelGetPosition(BassStream); // position in bytes
                    PlaybackSeconds = Bass.BASS_ChannelBytes2Seconds(BassStream, pos); // the elapsed time length
                    UpdateTime();
                    DrawSpectrum();
                    lblLyrics.Invalidate();
                    var track_vol = (float)Utils.DBToLevel(Convert.ToDouble(-1 * VolumeLevel), 1.0);
                    if (picPreview.Tag.ToString() != "preview")
                    {
                        if (!volSlide)
                        {
                            Bass.BASS_ChannelSetAttribute(BassMixer, BASSAttribute.BASS_ATTRIB_VOL, track_vol);
                        }
                        return;
                    }                                                       
                    if (PlaybackSeconds >= previewStart + FadeTime)
                    {
                        volSlide = false;
                    }
                    var previewEnd = previewStart + 30.0;
                    if (PlaybackSeconds >= (previewEnd - FadeTime) && PlaybackSeconds < previewEnd && !volSlide)                    
                    {
                        Bass.BASS_ChannelSlideAttribute(BassMixer, BASSAttribute.BASS_ATTRIB_VOL, 0, (int)(FadeTime * 1000));
                        volSlide = true;
                    }
                    if (PlaybackSeconds < previewEnd) return;
                }
                else
                {
                    PlaybackTimer.Enabled = false;                  
                }
                StopPlayback();
                PlaybackSeconds = previewStart == 0 || picPreview.Tag.ToString() == "song" ? 0 : previewStart;
                if (picLoop.Tag.ToString() != "loop") return;
                StartPlayback();                
            }
            catch (Exception)
            { }
        }
        
        private readonly Visuals Spectrum = new Visuals(); // visuals class instance
        private void DrawSpectrum()
        {
            var width = picSpect.Width;
            var height = picSpect.Height;
            var spect = Spectrum.CreateSpectrum(BassMixer, width, height, ChartGreen, ChartRed, panelPlay.BackColor, false, false, true);
            picSpect.Image = spect;
        }

        public void UpdateVolume(double volume)
        {
            if (Bass.BASS_ChannelIsActive(BassMixer) != BASSActive.BASS_ACTIVE_PAUSED &&
                Bass.BASS_ChannelIsActive(BassMixer) != BASSActive.BASS_ACTIVE_PLAYING)
            {
                return;
            }
            var track_vol = (float)Utils.DBToLevel(Convert.ToDouble(-1 * volume), 1.0);
            Bass.BASS_ChannelSetAttribute(BassMixer, BASSAttribute.BASS_ATTRIB_VOL, track_vol);
        }

        private void picVolume_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            var Volume = new Volume(this, Cursor.Position);
            Volume.Show();
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

        private void picVisualizer_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                var g = e.Graphics;
                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                //always align vertically, horizontal alignment varies by field
                var stringFormat = new StringFormat
                {
                    LineAlignment = StringAlignment.Center
                };

                //draw album art
                if (RESOURCE_ALBUM_ART != null)
                {
                    g.DrawImage(RESOURCE_ALBUM_ART, picAlbumArt.Left, picAlbumArt.Top, picAlbumArt.Width, picAlbumArt.Height);
                }

                //draw user logo
                if (RESOURCE_AUTHOR_LOGO != null)
                {
                    g.DrawImage(RESOURCE_AUTHOR_LOGO, picLogo.Left, picLogo.Top, picLogo.Width, picLogo.Height);
                }
                
                //draw song name
                if (!string.IsNullOrWhiteSpace(txtSong.Text))
                {
                    stringFormat.Alignment = StringAlignment.Near;
                    using (var f = new Font(ActiveFont, (float) numFontName.Value, FontStyle.Bold))
                    {
                        g.DrawString(txtSong.Text, f, new SolidBrush(songColor), songX, songY, stringFormat);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(txtSong1.Text))
                {
                    stringFormat.Alignment = StringAlignment.Near;
                    using (var f = new Font(ActiveFont, (float) numFontName.Value, FontStyle.Bold))
                    {
                        g.DrawString(txtSong1.Text, f, new SolidBrush(songColor), songX, songY - 12, stringFormat);
                        g.DrawString(txtSong2.Text, f, new SolidBrush(songColor), songX, songY + 8, stringFormat);
                    }
                }

                //draw song time
                if (!string.IsNullOrWhiteSpace(txtTime.Text))
                {
                    //limit string to 5 characters i.e 1:23:45
                    var time = txtTime.Text.Length > 7 ? txtTime.Text.Substring(0, 6) : txtTime.Text;
                    stringFormat.Alignment = StringAlignment.Far;
                    using (var f = new Font(ActiveFont, (float) numFontTime.Value, FontStyle.Bold))
                    {
                        g.DrawString(time, f, new SolidBrush(timeColor), TimeLeft, TimeTop, stringFormat);
                    }
                }

                //draw artist name
                if (!string.IsNullOrWhiteSpace(txtArtist.Text))
                {
                    stringFormat.Alignment = StringAlignment.Near;
                    using (var f = new Font(ActiveFont, (float) numFontArtist.Value, FontStyle.Bold))
                    {
                        g.DrawString(txtArtist.Text, f, new SolidBrush(artistColor), artistX, artistY, stringFormat);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(txtArtist1.Text))
                {
                    stringFormat.Alignment = StringAlignment.Near;
                    using (var f = new Font(ActiveFont, (float)numFontArtist.Value, FontStyle.Bold))
                    {
                        g.DrawString(txtArtist1.Text, f, new SolidBrush(artistColor), artistX, artistY - 10, stringFormat);
                        g.DrawString(txtArtist2.Text, f, new SolidBrush(artistColor), artistX, artistY + 10, stringFormat);
                    }
                }

                //redraw album name
                if (!string.IsNullOrWhiteSpace(txtAlbum.Text))
                {
                    var track = "";
                    if (txtTrack.Text.Trim().Length > 0 && txtTrack.Text.Trim() != "0" && chkTrack.Checked)
                    {
                        track = " (Track #" + txtTrack.Text.Trim() + ")";
                    }
                    stringFormat.Alignment = StringAlignment.Near;
                    using (var f = new Font(ActiveFont, 11, FontStyle.Bold))
                    {
                        g.DrawString(txtAlbum.Text + track, f, new SolidBrush(albumColor), albumX, albumY, stringFormat);
                    }
                }

                var genre = "";
                var ColorforGenre = DefaultColorLight;
                if (chkGenre.Checked)
                {
                    genre = chkSubGenre.Checked && !string.IsNullOrWhiteSpace(txtSubGenre.Text) ? txtGenre.Text + " (" + txtSubGenre.Text + ")" : txtGenre.Text;
                    ColorforGenre = genreColor;
                }
                else if (chkSubGenre.Checked)
                {
                    genre = txtSubGenre.Text;
                    ColorforGenre = genreColor;
                }

                //draw genre
                if (!string.IsNullOrWhiteSpace(genre))
                {
                    stringFormat.Alignment = StringAlignment.Near;
                    using (var f = new Font(ActiveFont, 11, FontStyle.Bold))
                    {
                        g.DrawString(genre, f, new SolidBrush(ColorforGenre), genreX, genreY, stringFormat);
                    }
                }

                //draw Rating
                if (!string.IsNullOrWhiteSpace(Rating) && cboRating.Enabled)
                {
                    stringFormat.Alignment = StringAlignment.Far;
                    using (var f = new Font(ActiveFont, 11, FontStyle.Bold))
                    {
                        g.DrawString(Rating, f, new SolidBrush(RatingColor), ratingX, ratingY, stringFormat);
                    }
                }

                //draw year
                if (!string.IsNullOrWhiteSpace(txtYear.Text))
                {
                    stringFormat.Alignment = StringAlignment.Far;
                    //limit year to 4 digits, i.e 2013
                    string year;
                    string year2;
                    var year1 = txtYear.Text.Length > 4 ? txtYear.Text.Substring(0, 3) : txtYear.Text;
                    if (string.IsNullOrWhiteSpace(txtYear2.Text))
                    {
                        year = year1;
                    }
                    else if (txtYear2.Text.Length > 4)
                    {
                        year2 = txtYear2.Text.Substring(0, 3);
                        year = year1 + " (" + year2 + ")";
                    }
                    else
                    {
                        year2 = txtYear2.Text;
                        year = year1 + " (" + year2 + ")";
                    }
                    using (var f = new Font(ActiveFont, 11, FontStyle.Bold))
                    {
                        g.DrawString(year, f, new SolidBrush(yearColor), yearX, !string.IsNullOrWhiteSpace(Rating) && cboRating.Enabled ? 112f : yearY, stringFormat);
                    }
                }
                
                g.DrawImage(GetDifficultyImage(diffGuitar), diffGuitar.Left, diffGuitar.Top, diffGuitar.Width, diffGuitar.Height);
                g.DrawImage(GetDifficultyImage(diffBass), diffBass.Left, diffBass.Top, diffBass.Width, diffBass.Height);
                g.DrawImage(GetDifficultyImage(diffDrums), diffDrums.Left, diffDrums.Top, diffDrums.Width, diffDrums.Height);
                g.DrawImage(GetDifficultyImage(diffKeys), diffKeys.Left, diffKeys.Top, diffKeys.Width, diffKeys.Height);
                g.DrawImage(GetDifficultyImage(diffVocals), diffVocals.Left, diffVocals.Top, diffVocals.Width, diffVocals.Height);
                if (proGuitar.Tag.ToString() == "1")
                {
                    g.DrawImage(RESOURCE_PRO_GUITAR, proGuitar.Left, proGuitar.Top, proGuitar.Width, proGuitar.Height);
                }
                if (proBass.Tag.ToString() == "1")
                {
                    g.DrawImage(RESOURCE_PRO_BASS, proBass.Left, proBass.Top, proBass.Width, proBass.Height);
                }
                if (proKeys.Tag.ToString() == "1")
                {
                    g.DrawImage(RESOURCE_PRO_KEYS, proKeys.Left, proKeys.Top, proKeys.Width, proKeys.Height);
                }
                if (pic2x.Tag.ToString() == "1")
                {
                    g.DrawImage(RESOURCE_2X, pic2x.Left, pic2x.Top, pic2x.Width, pic2x.Height);
                }
                if (intVocals > 1)
                {
                    g.DrawImage(intVocals == 2 ? RESOURCE_HARM2 : RESOURCE_HARM3, picHarm.Left, picHarm.Top, picHarm.Width, picHarm.Height);
                }                

                //draw icons
                if (RESOURCE_ICON1 != null)
                {
                    g.DrawImage(RESOURCE_ICON1, picIcon1.Left, picIcon1.Top, picIcon1.Width, picIcon1.Height);
                }
                if (RESOURCE_ICON2 != null)
                {
                    g.DrawImage(RESOURCE_ICON2, picIcon2.Left, picIcon2.Top, picIcon2.Width, picIcon2.Height);
                }

                if (!UseOverlay || string.IsNullOrWhiteSpace(ThemeName) || RESOURCE_THEME == null) return;
                e.Graphics.DrawImage(RESOURCE_THEME, 0, 0, RESOURCE_THEME.Width, RESOURCE_THEME.Height);
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error:\n" + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ControlChanged(object sender, EventArgs e)
        {
            picVisualizer.Invalidate();
        }
        
        private void lblBottom_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || lblBottom.Cursor != Cursors.Hand) return;
            if (!string.IsNullOrEmpty(imgLink))
            {
                Process.Start(imgLink);
            }
            if (!string.IsNullOrEmpty(lblBottom.Text) && !lblBottom.Text.Contains(","))
            {
                Process.Start($"https://rhythmverse.co/songfiles/author/{lblBottom.Text.ToLowerInvariant()}");
            }
        }

        private void picIcon1_MouseEnter(object sender, EventArgs e)
        {
            var box = (PictureBox) sender;
            var enabled = (sender == picIcon1 && RESOURCE_ICON1 != null) || (sender == picIcon2 && RESOURCE_ICON2 != null);
            toolTip1.SetToolTip(box, enabled ? "Right-click to hide the icons" : "");
            box.Cursor = enabled ? Cursors.Hand : Cursors.Default;
        }

        private void picLogo_MouseUp(object sender, MouseEventArgs e)
        {
            picLogo.Cursor = Cursors.Hand;
            if (RESOURCE_AUTHOR_LOGO != null)
            {
                picVisualizer.Invalidate();
                return;
            }
            var ofd = new OpenFileDialog
            {
                Title = "Select your logo",
                Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png",
                InitialDirectory = Tools.CurrentFolder
            };
            ofd.ShowDialog();
            if (ofd.FileName != "" && File.Exists(ofd.FileName))
            {
                GetLogo(ofd.FileName);
            }
            picVisualizer.Invalidate();
        }

        private void picLogo_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || RESOURCE_AUTHOR_LOGO == null) return;
            mouseX = MousePosition.X;
            mouseY = MousePosition.Y;
            picLogo.Cursor = Cursors.NoMove2D;
        }

        private void txtGenre_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtGenre.Text))
            {
                chkGenre.Checked = true;
            }
            ControlChanged(sender, e);
        }

        private void txtSubGenre_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtSubGenre.Text))
            {
                chkSubGenre.Checked = true;
            }
            ControlChanged(sender, e);
        }
        
        private void txtTime_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTime.Text)) return;
            try
            {
                Convert.ToDateTime(txtTime.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("That's not a valid duration value", "Visualizer", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtTime.Text = "";
                txtTime.Focus();
            }
        }

        private void toolStripClearIcon_Click(object sender, EventArgs e)
        {
            RESOURCE_ICON1 = null;
            RESOURCE_ICON2 = null;
            picVisualizer.Invalidate();
        }

        private bool PrepMixerCH(bool ignorePlaybackInstruments)
        {
            BassStreams.Clear();
            try
            {
                InitBass();
                var audioFile = isOpus ? opusFiles[0] : (isMP3 ? mp3Files[0] : (isWAV ? wavFiles[0] : oggFiles[0]));
                if (isOpus)
                {
                    BassStream = BassOpus.BASS_OPUS_StreamCreateFile(audioFile, 0L, File.ReadAllBytes(audioFile).Length, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);
                }
                else //ogg and wav and mp3
                {
                    BassStream = Bass.BASS_StreamCreateFile(audioFile, 0L, File.ReadAllBytes(audioFile).Length, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);
                }

                // create a decoder for the audio file(s)
                var channel_info = Bass.BASS_ChannelGetInfo(BassStream);

                // create a stereo mixer with same frequency rate as the input file
                if ((isMP3 && exportGHWTDEAudio.Checked) || (isWAV && exportPowerGigAudio.Checked && !isBandFuse) || (isBandFuse && exportBandFuseAudio.Checked))
                {
                    BassMixer = BassMix.BASS_Mixer_StreamCreate(channel_info.freq, 2, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_MIXER_END);
                }
                else
                {
                    BassMixer = BassMix.BASS_Mixer_StreamCreate(channel_info.freq, 2, BASSFlag.BASS_MIXER_END);
                }

                var folder = Path.GetDirectoryName(audioFile) + "\\";
                var ext = isOpus ? "opus" : (isMP3 ? "mp3" : (isWAV ? "wav" : "ogg"));
                var drums = folder + "drums." + ext;
                var drums1 = folder + "drums_1." + ext;
                var drums2 = folder + "drums_2." + ext;
                var drums3 = folder + "drums_3." + ext;
                var drums4 = folder + "drums_4." + ext;
                var bass = folder + "bass." + ext;
                var rhythm = folder + "rhythm." + ext;
                var guitar = folder + "guitar." + ext;
                var guitar1 = folder + "guitar_1." + ext;
                var guitar2 = folder + "guitar_2." + ext;
                var keys = folder + "keys." + ext;
                var vocals = folder + "vocals." + ext;
                var vocals1 = folder + "vocals_1." + ext;
                var vocals2 = folder + "vocals_2." + ext;
                var backing = folder + "backing." + ext;
                var song = folder + "song." + ext;
                var crowd = folder + "crowd." + ext;

                if (File.Exists(drums) || File.Exists(drums1) || File.Exists(drums2) || File.Exists(drums3) || File.Exists(drums4) || isFNF)
                {
                    Parser.Songs[0].ChannelsDrums = 2; //don't matter as long as it's more than 0 to enable it
                    Parser.Songs[0].ChannelsDrumsStart = 0;
                }
                if (File.Exists(bass) || File.Exists(rhythm) || isFNF)
                {
                    Parser.Songs[0].ChannelsBass = 2;
                    Parser.Songs[0].ChannelsBassStart = 2;
                }
                if (File.Exists(guitar) || File.Exists(guitar1) || File.Exists(guitar2) || isFNF)
                {
                    Parser.Songs[0].ChannelsGuitar = 2;
                    Parser.Songs[0].ChannelsGuitarStart = 4;
                }
                if (File.Exists(keys))
                {
                    Parser.Songs[0].ChannelsKeys = 2;
                }
                if (File.Exists(vocals) || File.Exists(vocals1) || File.Exists(vocals2) || isFNF)
                {
                    Parser.Songs[0].ChannelsVocals = 2;
                    Parser.Songs[0].ChannelsVocalsStart = 6;
                    Parser.Songs[0].ChannelsTotal = channel_info.chans;
                }
                if (File.Exists(crowd))
                {
                    Parser.Songs[0].ChannelsCrowd = 2;
                }

                if (!ignorePlaybackInstruments)
                {
                    updatePlaybackInstruments();
                }                              

                Parser.Songs[0].OriginalAttenuationValues = "";
                Parser.Songs[0].AttenuationValues = "";
                Parser.Songs[0].PanningValues = "";
                
                if (PlayDrums)
                {
                    if (File.Exists(drums))
                    {
                        AddAudioToMixer(drums);
                    }
                    else
                    {
                        var split_drums = new List<string> { drums1, drums2, drums3, drums4 };
                        foreach (var drum in split_drums.Where(File.Exists))
                        {
                            AddAudioToMixer(drum);
                        }
                    }
                }
                if (PlayBass)
                {                    
                    if (File.Exists(bass))
                    {
                        AddAudioToMixer(bass);
                    }
                    else if (File.Exists(rhythm))
                    {
                        AddAudioToMixer(rhythm);
                    }
                }
                if (PlayGuitar && (isOpus ? opusFiles.Count() > 1 : (isWAV ? wavFiles.Count() > 1 : oggFiles.Count() > 1)))
                {
                    if (File.Exists(guitar))
                    {
                        AddAudioToMixer(guitar);
                    }
                    else
                    {
                        var split_guitar = new List<string> { guitar1, guitar2 };
                        foreach (var gtr in split_guitar.Where(File.Exists))
                        {
                            AddAudioToMixer(gtr);
                        }
                    }
                    if (File.Exists(rhythm) && !File.Exists(bass))
                    {
                        AddAudioToMixer(rhythm);
                    }
                }
                if (PlayKeys && File.Exists(keys))
                {
                    AddAudioToMixer(keys);
                }
                if (PlayVocals)
                {
                    if (File.Exists(vocals))
                    {
                        AddAudioToMixer(vocals);
                    }
                    else
                    {
                        var split_vocals = new List<string> { vocals1, vocals2 };
                        foreach (var vocal in split_vocals.Where(File.Exists))
                        {
                            AddAudioToMixer(vocal);
                        }
                    }
                }
                if (PlayBacking)
                {
                    if (File.Exists(backing))
                    {
                        AddAudioToMixer(backing);
                    }
                    else if (File.Exists(song))
                    {
                        AddAudioToMixer(song);
                    }
                    else if ((isOpus ? opusFiles.Count() == 1 : oggFiles.Count() == 1) && audioFile == guitar)
                    {
                        AddAudioToMixer(guitar);
                    }
                }
                if (PlayCrowd && File.Exists(crowd))
                {
                    AddAudioToMixer(crowd);
                }
            }
            catch (Exception ex)
            {
                var error = $"Error preparing CH mixer\nError:\n{ex.Message}\nStack Trace:\n{ex.StackTrace}";
                Clipboard.SetText(error);
                MessageBox.Show(error, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }            
            return true;
        }

        private void AddAudioToMixer(string audioFile)
        {
            if (isOpus)
            {
                BassStream = BassOpus.BASS_OPUS_StreamCreateFile(audioFile, 0L, File.ReadAllBytes(audioFile).Length, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);
            }
            else
            {
                BassStream = Bass.BASS_StreamCreateFile(audioFile, 0L, File.ReadAllBytes(audioFile).Length, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);
            }
            var stream_info = Bass.BASS_ChannelGetInfo(BassStream);
            if (stream_info.chans == 0) return;
            BassMix.BASS_Mixer_StreamAddChannel(BassMixer, BassStream, BASSFlag.BASS_MIXER_MATRIX);
            BassStreams.Add(BassStream);

            if (isFNF)
            {//get and apply channel matrix
                var splitter = new MoggSplitter();
                var matrix = splitter.GetChannelMatrix(Parser.Songs[0], stream_info.chans, GetStemsToPlay());
                BassMix.BASS_Mixer_ChannelSetMatrix(BassStream, matrix);
            }
            
            int stream;
            if (isOpus)
            {
                stream = BassOpus.BASS_OPUS_StreamCreateFile(audioFile, 0L, File.ReadAllBytes(audioFile).Length, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);
            }
            else //ogg and wav and mp3
            {
                stream = Bass.BASS_StreamCreateFile(audioFile, 0L, File.ReadAllBytes(audioFile).Length, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);
            }
            var len = Bass.BASS_ChannelGetLength(stream);
            var totaltime = Bass.BASS_ChannelBytes2Seconds(stream, len); //the total time length
            Parser.Songs[0].Length = (int)(totaltime * 1000);
            lblSongLength.Text = Parser.GetSongDuration(Parser.Songs[0].Length.ToString(CultureInfo.InvariantCulture));
            txtTime.Text = lblSongLength.Text;
        }

        private void ExtractPsArc(string file)
        {
            if (file.Contains("_p_") || file.Contains("_p."))
            {
                MessageBox.Show("Your file has a file name that contains '_p' and that confuses the program Visualizer relies on to unpack .psarc files\n\nPlease remove that from the file name and try again", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            var outFolder = Application.StartupPath + "\\visualizer";
            if (Directory.Exists(outFolder))
            {
                Tools.DeleteFolder(outFolder, true);
            }
            Directory.CreateDirectory(outFolder);

            var rsFolder = outFolder + "\\" + Path.GetFileNameWithoutExtension(file) + "_psarc_RS2014_Pc";

            if (!Tools.ExtractPsArc(file, outFolder, rsFolder))
            {
                MessageBox.Show("Failed to process that PsArc file, can't Visualize", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            PlayARCFolder(rsFolder);
        }

        private void PlayARCFolder(string folder)
        {
            var audioFolder = folder + "\\audio\\"; //windows\\";
            var OggFiles = Directory.GetFiles(audioFolder, "*.ogg", SearchOption.AllDirectories);
            var artFolder = folder + "\\gfxassets\\album_art\\";            
            var pngFiles = Directory.GetFiles(artFolder, "*.png");
            var ddsFiles = Directory.GetFiles(artFolder, "*.dds");
            var manifestFolder = folder + "\\manifests\\";
            var metadataFiles = Directory.GetFiles(manifestFolder, "*.hsan", SearchOption.AllDirectories);
            var sngFolder = folder + "\\songs\\bin\\"; //generic\\";
            var sngFiles = Directory.GetFiles(sngFolder, "*.sng");
                        
            loadDefaults();

            bool hasRhythm = false;
            bool hasVocals = false;

            if (pngFiles.Count() > 0)
            {
                getImage(pngFiles[0]);
            }
            else if (ddsFiles.Count() > 0)
            {
                foreach (var file in ddsFiles)
                {
                    if (file.Contains("256"))
                    {
                        getImage(file);
                        break;
                    }
                }
            }

            if (metadataFiles.Count() > 0)
            {
                Parser.ReadHSANFile(metadataFiles[0]);                
            }

            if (sngFiles.Count() > 0)
            {
                foreach (var sng in sngFiles)
                {
                    if (sng.Contains("_rhythm.sng"))
                    {
                        hasRhythm = true;
                    }
                    else if (sng.Contains("_vocals.sng"))
                    {
                        hasVocals = true;
                    }
                }
            }
            
            if (hasVocals)
            {
                Parser.Songs[0].VocalsDiff = 1;
            }
            else if (hasRhythm)
            {                
                Parser.Songs[0].RhythmBass = true;
            }

            isRS2014 = true;
            loadDTA();

            var bigOgg = "";
            var sortedOggs = from f in OggFiles orderby new FileInfo(f).Length ascending select f;
            foreach (var sorted in sortedOggs)
            {
                bigOgg = sorted.ToString();
            }
            var song = Path.GetDirectoryName(audioFolder) + "\\song.ogg";
            Tools.MoveFile(bigOgg, song);
            oggFiles[0] = song;

            isMP3 = false;
            isOpus = false;
            isWAV = false;
            isOgg = true;
            isM4A = false;
            Height = maxHeight;
            picPlayPause.Cursor = Cursors.Hand;
            picStop.Cursor = Cursors.Hand;
            PlaybackSeconds = picPreview.Tag.ToString() == "preview" ? 30.0 : 0.0;
            StartPlayback();           
        }

        private async Task ExtractYARG(string file)
        {
            var outFolder = Application.StartupPath + "\\visualizer\\extracted" + "\\" + Path.GetFileNameWithoutExtension(file).Trim();
            Tools.DeleteFolder(outFolder, true);
            Directory.CreateDirectory(outFolder);
            var success = await Tools.DecryptExtractYARGSONG(file, outFolder);
            if (!success)
            {
                MessageBox.Show("Failed to decode and extract that YARG file, can't Visualize", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            PlayCHFolder(outFolder);
        }

        private async Task ExtractSNG(string file)
        {
            var outFolder = Application.StartupPath + "\\visualizer\\extracted" + Path.GetFileNameWithoutExtension(file).Trim();
            Tools.DeleteFolder(outFolder, true);
            Directory.CreateDirectory(outFolder);

            await Tools.ExtractSNG(file, outFolder);
            if (!Directory.Exists(outFolder) || !Directory.GetFiles(outFolder, "*.*", SearchOption.TopDirectoryOnly).Any())
            {
                MessageBox.Show("Failed to decode and extract that SNG file, can't Visualize", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }            
            PlayCHFolder(outFolder);                       
        }

        private void TryPlayCHVideo(string folder)
        {
            if (!autoplayMusicVideos.Checked) return;
            try
            {
                var validFileNames = new List<string>() { "background.avi", "video.mp4", "video.webm", "bg.mp4", "bg.webm" };
                var files = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories);
                if (!files.Any()) return;
                var _mediaFile = "";
                foreach (var file in files)
                {
                    if (validFileNames.Contains(Path.GetFileName(file)))
                    {
                        _mediaFile = file;
                        break;
                    }
                }
                if (string.IsNullOrEmpty(_mediaFile)) return;

                var media = new Media(_libVLC, _mediaFile, FromType.FromPath);
                if (media == null) return;

                _mediaPlayer.Media = media;
                _mediaPlayer.Play(); //has to start playing to be seekable
                if (_mediaPlayer.IsSeekable)
                {
                    _mediaPlayer.Time = (long)(PlaybackSeconds * 1000) + Parser.Songs[0].VideoStartTime;
                }
                videoView.Visible = true;
                picLastFM.Visible = false;
            }
            catch
            {
                videoView.Visible = false;
                picLastFM.Visible = true;
            }
        }

        private void PlayCHFolder(string outFolder)
        {
            loadDefaults();
            var ini = Directory.GetFiles(outFolder, "song.ini");
            if (ini.Count() > 0)
            {
                Parser.ReadINIFile(ini[0]);
                loadDTA();
            }
            var artPNG = Directory.GetFiles(outFolder, "album.png");
            var artJPG = Directory.GetFiles(outFolder, "album.jpg");
            if (artPNG.Count() > 0)
            {
                getImage(artPNG[0]);
            }
            else if (artJPG.Count() > 0)
            {
                getImage(artJPG[0]);
            }
            var midi = Directory.GetFiles(outFolder, "notes.mid");
            if (midi.Count() > 0)
            {
                ReadMidi(midi[0]);
            }
            else
            {
                var chart = Directory.GetFiles(outFolder, "*.chart");
                if (chart.Count() > 0)
                {
                    var chartMid = ChartToRockBandMidi.ConvertChartToMidi(chart[0]);
                    var chartMidPath = outFolder + "\\notes.mid";
                    NAudio.Midi.MidiFile.Export(chartMidPath, chartMid);
                    if (File.Exists(chartMidPath))
                    {
                        ReadMidi(chartMidPath);
                    }
                }
            }    
            TryPlayCHVideo(outFolder);
            opusFiles = Directory.GetFiles(outFolder, "*.opus");
            oggFiles = Directory.GetFiles(outFolder, "*.ogg");
            if (opusFiles.Count() > 0)
            {
                isOpus = true;
                isOgg = false;
                isWAV = false;
                isMP3 = false;
                isM4A = false;
                if (opusFiles.Count() > 1)
                {                    
                    Height = maxHeight;
                    picPlayPause.Cursor = Cursors.Hand;
                    picStop.Cursor = Cursors.Hand;
                    StartPlayback();
                    return;
                }
                Height = maxHeight;
                picPlayPause.Cursor = Cursors.Hand;
                picStop.Cursor = Cursors.Hand;
                StartPlayback();
            }
            else if (oggFiles.Count() > 0)
            {
                isOpus = false;
                isOgg = true;
                isWAV = false;
                isMP3 = false;
                isM4A = false;
                if (oggFiles.Count() > 1)
                {
                    Height = maxHeight;
                    picPlayPause.Cursor = Cursors.Hand;
                    picStop.Cursor = Cursors.Hand;
                    StartPlayback();
                    return;
                }
                Height = maxHeight;
                picPlayPause.Cursor = Cursors.Hand;
                picStop.Cursor = Cursors.Hand;
                StartPlayback();
            }
            else
            {
                MessageBox.Show("No .opus or .ogg audio files found, can't play the song", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void ExtractPKG(string file)
        {
            var folder = Application.StartupPath + "\\visualizer\\";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var outFolder = folder + "PKG";
            Tools.DeleteFolder(outFolder, true);
            loadDefaults();

            byte[] dtaBytes = Tools.ReadSongsDTAFromPKG(file);
            if (dtaBytes != null)
            {
                Parser.ReadDTA(dtaBytes);
            }
            if (Parser.Songs.Count > 1)
            {
                MessageBox.Show("It looks like this is a pack...\nWhat were you expecting me to do with this?\nTry a single song, please",
                        Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Tools.DeleteFolder(outFolder, true);
                return;
            }
            if (!Tools.ExtractPS3PKG(file, outFolder))
            {
                MessageBox.Show("Failed to process that PKG file, can't Visualize", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            var DTA = Directory.GetFiles(outFolder, "songs.dta", SearchOption.AllDirectories);            
            var internalName = Parser.Songs[0].InternalName;
            var PNG_PS3 = Directory.GetFiles(outFolder, internalName + "_keep.png_ps3", SearchOption.AllDirectories);
            var MOGG = Directory.GetFiles(outFolder, internalName + ".mogg", SearchOption.AllDirectories);            
            loadDTA();
            getImage(PNG_PS3[0]);            
            if (!nautilus3.DecM(File.ReadAllBytes(MOGG[0]), false, false, DecryptMode.ToMemory))
            {
                MessageBox.Show("Failed to decrypt *.mogg file, can't play audio", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var edat = Directory.GetFiles(outFolder, "*.edat", SearchOption.AllDirectories);           
            if (edat.Any())
            {
                var rap = GetRapPath(edat[0], Application.StartupPath + "\\bin\\raps\\");
                if (!string.IsNullOrEmpty(rap)) //retail encryption
                {
                    Tools.DecryptRetailEdat(edat[0], edat[0].Replace(".edat", ""), rap);
                }
                else //Onyx and Nautilus encryption
                {
                    if (!Tools.DecryptCustomEdat(edat[0], edat[0].Replace(".edat", ""), Tools.GetKLIC(folder))) //might fail if it's a C3 folder???
                    {
                        //try again but with C3 fixed klic
                        Tools.DecryptCustomEdat(edat[0], edat[0].Replace(".edat", ""));
                    }
                }
            }
            var midi = Directory.GetFiles(outFolder, "*.mid", SearchOption.AllDirectories);
            if (midi.Any())
            {
                ReadMidi(midi[0]);
            }
            //extract audio file for previewing
            Height = maxHeight;
            ProcessAudio();
        }

        private void ghwtdeWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            var fsb1 = Path.GetDirectoryName(GHWTDE_INI_PATH) + "\\Content\\MUSIC\\" + Parser.Songs[0].InternalName + "_1.fsb.xen";
            var fsb2 = Path.GetDirectoryName(GHWTDE_INI_PATH) + "\\Content\\MUSIC\\" + Parser.Songs[0].InternalName + "_2.fsb.xen";
            var fsb3 = Path.GetDirectoryName(GHWTDE_INI_PATH) + "\\Content\\MUSIC\\" + Parser.Songs[0].InternalName + "_3.fsb.xen";

            var path = Application.StartupPath + "\\visualizer\\temp\\";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var decFSB1 = path + Path.GetFileName(fsb1);
            var decFSB2 = path + Path.GetFileName(fsb2);
            var decFSB3 = path + Path.GetFileName(fsb3);

            if (Tools.fsbIsEncrypted(fsb1))
            {
                var dec = Tools.DecryptFSBFile(fsb1);
                File.WriteAllBytes(decFSB1, dec);
                fsb1 = decFSB1;
            }
            if (File.Exists(decFSB1) && Tools.fsbIsEncrypted(decFSB1))
            {
                MessageBox.Show("File '" + Path.GetFileName(fsb1) + "' is encrypted and I failed to decrypt it\nCan't continue", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Tools.DeleteFile(decFSB1);
                return;
            }

            if (Tools.fsbIsEncrypted(fsb2))
            {
                var dec = Tools.DecryptFSBFile(fsb2);
                File.WriteAllBytes(decFSB2, dec);
                fsb2 = decFSB2;
            }
            if (File.Exists(decFSB2) && Tools.fsbIsEncrypted(decFSB2))
            {
                MessageBox.Show("File '" + Path.GetFileName(fsb2) + "' is encrypted and I failed to decrypt it\nCan't continue", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Tools.DeleteFile(decFSB2);
                return;
            }

            if (Tools.fsbIsEncrypted(fsb3))
            {
                var dec = Tools.DecryptFSBFile(fsb3);
                File.WriteAllBytes(decFSB3, dec);
                fsb3 = decFSB3;
            }
            if (File.Exists(decFSB3) && Tools.fsbIsEncrypted(decFSB3))
            {
                MessageBox.Show("File '" + Path.GetFileName(fsb3) + "' is encrypted and I failed to decrypt it\nCan't continue", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Tools.DeleteFile(decFSB3);
                return;
            }

            //extract drum tracks
            const int frame = 384;//size of each frame of audio
            const int spacer1 = 1152;//the spacer from one track past the other three
            const int spacer2 = 768;//the spacer from one track past the other two
            const int spacer3 = 384;//the spacer from one track past the other

            var kick_audio = GHWTDE_EXT_PATH + "\\drums_1.mp3";
            var snare_audio = GHWTDE_EXT_PATH + "\\drums_2.mp3";
            var cymbal_audio = GHWTDE_EXT_PATH + "\\drums_3.mp3";
            var tom_audio = GHWTDE_EXT_PATH + "\\drums_4.mp3";
            var guitar_audio = GHWTDE_EXT_PATH + "\\guitar.mp3";
            var bass_audio = GHWTDE_EXT_PATH + "\\bass.mp3";
            var vocals_audio = GHWTDE_EXT_PATH + "\\vocals.mp3";
            var backing_audio = GHWTDE_EXT_PATH + "\\song.mp3";
            var crowd_audio = GHWTDE_EXT_PATH + "\\crowd.mp3";

            var kick_offset = 0x80;
            var snare_offset = kick_offset + frame;
            var cymbal_offset = snare_offset + frame;
            var tom_offset = cymbal_offset + frame;

            ExtractFSBAudio(fsb1, kick_offset, spacer1, kick_audio);
            ExtractFSBAudio(fsb1, snare_offset, spacer1, snare_audio);
            ExtractFSBAudio(fsb1, cymbal_offset, spacer1, cymbal_audio);
            ExtractFSBAudio(fsb1, tom_offset, spacer1, tom_audio);

            //extract guitar bass vocals
            var guitar_offset = 0x80;
            var bass_offset = guitar_offset + frame;
            var vocals_offset = bass_offset + frame;

            ExtractFSBAudio(fsb2, guitar_offset, spacer2, guitar_audio);
            ExtractFSBAudio(fsb2, bass_offset, spacer2, bass_audio);
            ExtractFSBAudio(fsb2, vocals_offset, spacer2, vocals_audio);

            //extract backing and crowd
            var backing_offset = 0x80;
            var crowd_offset = backing_offset + frame;

            ExtractFSBAudio(fsb3, backing_offset, spacer3, backing_audio);
            ExtractFSBAudio(fsb3, crowd_offset, spacer3, crowd_audio);
        }

        private void ghwtdeWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            picWorking.Visible = false;

            mp3Files = Directory.GetFiles(GHWTDE_EXT_PATH, "*.mp3");
            if (mp3Files.Any())
            {
                isMP3 = true;
                isOgg = false;
                isOpus = false;
                isWAV = false;
                isM4A = false;

                //the metadata doesn't contain song data, let's get it from the mp3 files
                InitBass();
                var stream = Bass.BASS_StreamCreateFile(mp3Files[0], 0L, File.ReadAllBytes(mp3Files[0]).Length, BASSFlag.BASS_SAMPLE_FLOAT);
                var len = Bass.BASS_ChannelGetLength(stream);
                var totaltime = Bass.BASS_ChannelBytes2Seconds(stream, len); // the total time length
                Parser.Songs[0].Length = (int)(totaltime * 1000);
                lblSongLength.Text = Parser.GetSongDuration(Parser.Songs[0].Length.ToString(CultureInfo.InvariantCulture));
                txtTime.Text = lblSongLength.Text;                             

                if (allowAccessToGHWTDE.Checked)
                {
                    Process.Start(GHWTDE_EXT_PATH);
                }

                Height = maxHeight;
                picPlayPause.Cursor = Cursors.Hand;
                picStop.Cursor = Cursors.Hand;
                PlaybackSeconds = picPreview.Tag.ToString() == "preview" ? 30.0 : 0.0;
                StartPlayback();
            }
            else
            {
                MessageBox.Show("Failed to extract audio files from FSB files, can't play this song", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void exportGHWTDEAudio_Click(object sender, EventArgs e)
        {
            ShowSongSaveMessage((ToolStripMenuItem)(sender), "GHWT:DE");
        }

        private void allowAccessToGHWTDE_Click(object sender, EventArgs e)
        {
            if (allowAccessToGHWTDE.Checked)
            {
                MessageBox.Show("With this option enabled, Visualizer will open the temporary folder where the multitrack .mp3 files are located\nCopy them to another folder before you close Visualizer or they will be deleted!", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async void DownloadAlbumArt()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string apiKey = "6b579e4522f9846c88cf88b6a44564f5";
            string url = $"http://ws.audioscrobbler.com/2.0/?method=album.getinfo&api_key={apiKey}&artist={Uri.EscapeDataString(songArtist)}&album={Uri.EscapeDataString(songAlbum)}&format=json";

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                JObject albumInfo = JObject.Parse(jsonResponse);

                // Extract the album art URL (e.g., the largest image)
                var images = albumInfo["album"]?["image"];
                string albumArtUrl = null;

                if (images != null)
                {
                    // Choose the largest image
                    foreach (var image in images)
                    {
                        if (image?["size"]?.ToString() == "mega")
                        {
                            albumArtUrl = image?["#text"]?.ToString();
                            break;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(albumArtUrl))
                {
                    try
                    {
                        using (HttpClient client = new HttpClient())
                        using (response = await client.GetAsync(albumArtUrl))
                        {
                            response.EnsureSuccessStatusCode();
                            using (var stream = await response.Content.ReadAsStreamAsync())
                            {
                                picAlbumArt.Image = Image.FromStream(stream);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading image: {ex.Message}");
                    }

                }
                else
                {
                    MessageBox.Show("No album art found on last.fm");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error downloading album art: {ex.Message}");
            }
        }

        private void powerGigWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (!Tools.XMASH(XMA_PATH))
            {
                MessageBox.Show("Failed to extract the audio streams from '" + Path.GetFileName(XMA_PATH) + "' - can't play this song", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Tools.DeleteFile(XMA_EXT_PATH + "\\xmash.exe");
                return;
            }
            Tools.DeleteFile(XMA_EXT_PATH + "\\xmash.exe");
            Tools.DeleteFile(XMA_PATH);

            var XMAs = Directory.GetFiles(XMA_EXT_PATH, "*.xma", SearchOption.TopDirectoryOnly);
            if (XMAs.Count() == 0)
            {
                MessageBox.Show("Failed to extract the audio streams from '" + Path.GetFileName(XMA_PATH) + "' - can't play this song", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            foreach (var xma in XMAs)
            {
                if (!Tools.toWAV(xma))
                {
                    MessageBox.Show("Failed to convert XMA file to WAV - can't play this song", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Tools.DeleteFile(XMA_EXT_PATH + "\\towav.exe");
                    return;
                }
            }
            Tools.DeleteFile(XMA_EXT_PATH + "\\towav.exe");

            foreach (var xma in XMAs)
            {
                Tools.DeleteFile(xma);
            }

            //rename the files based on assumed order
            wavFiles = Directory.GetFiles(XMA_EXT_PATH, "*.wav");
            for (var i = wavFiles.Count(); i >= 0; i--)
            {
                if (i == wavFiles.Count() - 1)
                {
                    File.Move(wavFiles[i], XMA_EXT_PATH + "\\song.wav");
                }
                else if (i == wavFiles.Count() - 2)
                {
                    File.Move(wavFiles[i], XMA_EXT_PATH + "\\guitar.wav");
                }
                else if (i == wavFiles.Count() - 3)
                {
                    File.Move(wavFiles[i], XMA_EXT_PATH + "\\vocals.wav");
                }
                else if (i <= wavFiles.Count() - 4)
                {                    
                    switch (i) 
                    {
                        case 0:
                            File.Move(wavFiles[i], XMA_EXT_PATH + "\\drums_1.wav");
                            return;
                        case 1:
                            File.Move(wavFiles[i - 1], XMA_EXT_PATH + "\\drums_1.wav");
                            File.Move(wavFiles[i], XMA_EXT_PATH + "\\drums_2.wav");
                            return;
                        case 2:
                            File.Move(wavFiles[i - 3], XMA_EXT_PATH + "\\drums_1.wav");
                            File.Move(wavFiles[i - 1], XMA_EXT_PATH + "\\drums_2.wav");
                            File.Move(wavFiles[i], XMA_EXT_PATH + "\\drums_3.wav");
                            return;
                        case 3:
                            File.Move(wavFiles[i - 3], XMA_EXT_PATH + "\\drums_1.wav");
                            File.Move(wavFiles[i - 2], XMA_EXT_PATH + "\\drums_2.wav");
                            File.Move(wavFiles[i - 1], XMA_EXT_PATH + "\\drums_3.wav");
                            File.Move(wavFiles[i], XMA_EXT_PATH + "\\drums_4.wav");
                            return;
                    }
                }
            }
        }

        private void powerGigWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            picWorking.Visible = false;

            wavFiles = Directory.GetFiles(XMA_EXT_PATH, "*.wav");
            if (wavFiles.Any())
            {
                isWAV = true;
                isMP3 = false;
                isOgg = false;
                isOpus = false;
                isM4A = false;

                //the metadata doesn't contain song data, let's get it from the mp3 files
                InitBass();
                var stream = Bass.BASS_StreamCreateFile(wavFiles[0], 0L, File.ReadAllBytes(wavFiles[0]).Length, BASSFlag.BASS_SAMPLE_FLOAT);
                var len = Bass.BASS_ChannelGetLength(stream);
                var totaltime = Bass.BASS_ChannelBytes2Seconds(stream, len); // the total time length
                Parser.Songs[0].Length = (int)(totaltime * 1000);
                lblSongLength.Text = Parser.GetSongDuration(Parser.Songs[0].Length.ToString(CultureInfo.InvariantCulture));
                txtTime.Text = lblSongLength.Text;

                if (allowAccessToPowerGig.Checked)
                {
                    Process.Start(XMA_EXT_PATH);
                }

                Height = maxHeight;
                picPlayPause.Cursor = Cursors.Hand;
                picStop.Cursor = Cursors.Hand;
                StartPlayback();
            }
            else
            {
                MessageBox.Show("Failed to extract audio files from the XMA file, can't play this song", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void exportPowerGigAudio_Click(object sender, EventArgs e)
        {
            ShowSongSaveMessage((ToolStripMenuItem)(sender), "Power Gig");
        }

        private void allowAccessToPowerGig_Click(object sender, EventArgs e)
        {
            if (allowAccessToPowerGig.Checked)
            {
                MessageBox.Show("With this option enabled, Visualizer will open the temporary folder where the multitrack .wav files are located\nCopy them to another folder before you close Visualizer or they will be deleted!", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void bandFuseWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            var visualizer = Application.StartupPath + "\\visualizer\\";
            var extracted_path = visualizer + "temp\\";

            var didBacking = false;
            var didBass = false;
            var didDrums = false;
            var didGuitar1 = false;
            var didGuitar2 = false;
            var didVocals = false;

            foreach (var clt in cltFiles)
            {
                if (clt.EndsWith("\\back\\audio.clt") && !didBacking)
                {
                    Tools.ConvertBandFuse("audio", clt, extracted_path + "backing.wav");
                    didBacking = true;
                }
                else if (clt.EndsWith("\\bass\\audio.clt") && !didBass)
                {
                    Tools.ConvertBandFuse("audio", clt, extracted_path + "bass.wav");
                    didBass = true;
                }
                else if (clt.EndsWith("\\drums\\audio.clt") && !didDrums)
                {
                    Tools.ConvertBandFuse("audio", clt, extracted_path + "drums.wav");
                    didDrums = true;
                }
                else if (clt.EndsWith("\\gtr1\\audio.clt") && !didGuitar1)
                {
                    Tools.ConvertBandFuse("audio", clt, extracted_path + "guitar_1.wav");
                    didGuitar1 = true;
                }
                else if (clt.EndsWith("\\gtr2\\audio.clt") && !didGuitar2)
                {
                    Tools.ConvertBandFuse("audio", clt, extracted_path + "guitar_2.wav");
                    didGuitar2 = true;
                }
                else if (clt.EndsWith("\\vox\\audio.clt") && !didVocals)
                {
                    Tools.ConvertBandFuse("audio", clt, extracted_path + "vocals.wav");
                    didVocals = true;
                }
            }

            wavFiles = Directory.GetFiles(extracted_path, "*.wav", SearchOption.TopDirectoryOnly);            
        }

        private void bandFuseWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            var visualizer = Application.StartupPath + "\\visualizer\\";
            var extracted_path = visualizer + "temp\\";

            picWorking.Visible = false;
            if (!wavFiles.Any())
            {
                MessageBox.Show("Failed to decrypt the CLT audio, can't play this song", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            isWAV = true;

            if (allowAccessToBandFuse.Checked)
            {
                Process.Start(extracted_path);
            }

            Height = maxHeight;
            picPlayPause.Cursor = Cursors.Hand;
            picStop.Cursor = Cursors.Hand;
            PlaybackSeconds = picPreview.Tag.ToString() == "preview" ? 30.0 : 0.0;
            StartPlayback();
        }

        private void exportBandFuseAudio_Click(object sender, EventArgs e)
        {
            ShowSongSaveMessage((ToolStripMenuItem)(sender), "BandFuse");
        }

        private void allowAccessToBandFuse_Click(object sender, EventArgs e)
        {
            if (allowAccessToBandFuse.Checked)
            {
                MessageBox.Show("With this option enabled, Visualizer will open the temporary folder where the multitrack .wav files are located\nCopy them to another folder before you close Visualizer or they will be deleted!", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }                

        private void exportYARGAudio_Click(object sender, EventArgs e)
        {
            ShowSongSaveMessage((ToolStripMenuItem)(sender), "YARG");
        }

        private void mnuToolStripSeparator_Custom_Paint(Object sender, PaintEventArgs e)
        {
            ToolStripSeparator sep = (ToolStripSeparator)sender;

            e.Graphics.FillRectangle(new SolidBrush(Color.FromName("GradientInactiveCaption")), 0, 0, sep.Width, sep.Height);

            e.Graphics.DrawLine(new Pen(Color.White), 30, sep.Height / 2, sep.Width - 4, sep.Height / 2);

        }

        private void txtTrack_TextChanged(object sender, EventArgs e)
        {
            picVisualizer.Invalidate();
            if (reset) return;
            MeasureAlbum();
        }

        private void lblLyrics_Paint(object sender, PaintEventArgs e)
        {
            if (MIDITools == null) return;
            if (MIDITools.PhrasesVocals == null) return;
            if (!MIDITools.PhrasesVocals.Phrases.Any()) return;
            if (MIDITools.LyricsVocals == null) return;
            if (!MIDITools.LyricsVocals.Lyrics.Any()) return;
            var phrases = MIDITools.PhrasesVocals.Phrases;

            var lyrics = MIDITools.LyricsVocals.Lyrics;
            if (lyricsFixed.Checked)
            {
                DrawLyricsStatic(phrases, lblLyrics, Color.White, e.Graphics);
            }
            else if (lyricsScrolling.Checked)
            {
                DrawLyricsScrolling(lyrics, lblLyrics, Color.White, e.Graphics);
            }
            else
            {
                DrawLyricsKaraoke(phrases, lyrics, lblLyrics, lyricColor, e.Graphics);
            }
        }

        private readonly Color lyricColor = Color.Yellow;
        private readonly Color LabelBackgroundColor = Color.FromArgb(127, 40, 40, 40);
        private void DrawLyricsKaraoke(IEnumerable<LyricPhrase> phrases, IEnumerable<Lyric> lyrics, Control label, Color color, Graphics graphics)
        {
            var time = GetCorrectedTime();
            var doWholeWordsLyrics = useWholeWords.Checked;
            label.Text = "";
            using (var pen = new SolidBrush(LabelBackgroundColor))
            {
                graphics.FillRectangle(pen, label.ClientRectangle);
            }
            LyricPhrase line = null;
            foreach (var lyric in phrases.TakeWhile(lyric => lyric.PhraseStart <= time).Where(lyric => lyric.PhraseEnd >= time))
            {
                line = lyric;
            }
            if (line == null || string.IsNullOrEmpty(line.PhraseText)) return;
            var measure = TextRenderer.MeasureText(ProcessLine(line.PhraseText, doWholeWordsLyrics), label.Font);
            var left = (label.Width - measure.Width) / 2;
            TextRenderer.DrawText(graphics, ProcessLine(line.PhraseText, doWholeWordsLyrics), label.Font, new Point(left, 0), Color.White);
            var line2 = lyrics.Where(lyr => !(lyr.Start < line.PhraseStart)).TakeWhile(lyr => !(lyr.Start > time)).Aggregate("", (current, lyr) => current + " " + lyr.Text);
            if (string.IsNullOrEmpty(line2)) return;
            TextRenderer.DrawText(graphics, ProcessLine(line2, doWholeWordsLyrics), label.Font, new Point(left, 0), color);
        }

        private void DrawLyricsStatic(IEnumerable<LyricPhrase> phrases, Control label, Color color, Graphics graphics)
        {
            var time = GetCorrectedTime();
            var doWholeWordsLyrics = useWholeWords.Checked;
            label.Text = "";
            using (var pen = new SolidBrush(LabelBackgroundColor))
            {
                graphics.FillRectangle(pen, label.ClientRectangle);
            }
            LyricPhrase phrase = null;
            foreach (var lyric in phrases.TakeWhile(lyric => lyric.PhraseStart <= time).Where(lyric => lyric.PhraseEnd >= time))
            {
                phrase = lyric;
            }
            string line;
            try
            {
                line = phrase == null || string.IsNullOrEmpty(phrase.PhraseText.Trim()) ? GetMusicNotes() : ProcessLine(phrase.PhraseText, doWholeWordsLyrics);
            }
            catch (Exception)
            {
                line = GetMusicNotes();
            }
            var measure = TextRenderer.MeasureText(ProcessLine(line, doWholeWordsLyrics), label.Font);
            var left = (label.Width - measure.Width) / 2;
            TextRenderer.DrawText(graphics, line, label.Font, new Point(left, 0), color);
        }

        private void DrawLyricsScrolling(List<Lyric> lyrics, Control label, System.Drawing.Color color, Graphics graphics)
        {           
            var time = GetCorrectedTime();
            var playbackWindow = 3.0;
            label.Text = "";
            using (var pen = new SolidBrush(LabelBackgroundColor))
            {
                graphics.FillRectangle(pen, label.ClientRectangle);
            }
            for (var i = 0; i < lyrics.Count(); i++)
            {
                if (lyrics[i].Start < time) continue;
                if (lyrics[i].Start > time + playbackWindow) return;
                var left = (int)(((lyrics[i].Start - time) / playbackWindow) * label.Width);
                TextRenderer.DrawText(graphics, ProcessLine(lyrics[i].Text, true), label.Font, new Point(left, 0), color);
            }
        }

        private double GetCorrectedTime()
        {
            return PlaybackSeconds - ((double)BassBuffer / 1000) - ((double)Parser.Songs[0].PSDelay / 1000);
        }

        private string GetMusicNotes()
        {
            //"♫ ♫ ♫ ♫"
            var quarter = (int)((PlaybackSeconds - (int)PlaybackSeconds) * 100);
            string notes;
            if (quarter >= 0 && quarter < 25)
            {
                notes = "♫";
            }
            else if (quarter >= 25 && quarter < 50)
            {
                notes = "♫ ♫";
            }
            else if (quarter >= 50 && quarter < 75)
            {
                notes = "♫ ♫ ♫";
            }
            else
            {
                notes = "♫ ♫ ♫ ♫";
            }
            return notes;
        }

        private static string ProcessLine(string line, bool clean)
        {
            if (line == null) return "";
            string newline;
            if (clean)
            {
                newline = line.Replace("$", "");
                newline = newline.Replace("#", "");
                newline = newline.Replace("^", "");
                newline = newline.Replace("- + ", "");
                newline = newline.Replace("+- ", "");
                newline = newline.Replace("- ", "");
                newline = newline.Replace(" + ", " ");
                newline = newline.Replace(" +", "");
                newline = newline.Replace("+ ", "");
                newline = newline.Replace("+-", "");
                newline = newline.Replace("=", "-");
                newline = newline.Replace("§", " ");// Replace("§", "‿");
                newline = newline.Replace("- ", "-").Trim();
                if (newline.EndsWith("+", StringComparison.Ordinal))
                {
                    newline = newline.Substring(0, newline.Length - 1).Trim();
                }
                if (newline.EndsWith("-", StringComparison.Ordinal))
                {
                    newline = newline.Substring(0, newline.Length - 1);
                }
            }
            else
            {
                newline = line;
            }
            return newline.Replace("/", "").Replace("§", " ").Trim();
        }

        private void lyricsKaraoke_Click(object sender, EventArgs e)
        {
            lyricsKaraoke.Checked = true;
            lyricsFixed.Checked = false;
            lyricsScrolling.Checked = false;
        }

        private void lyricsScrolling_Click(object sender, EventArgs e)
        {
            lyricsKaraoke.Checked = false;
            lyricsFixed.Checked = false;
            lyricsScrolling.Checked = true;
        }

        private void lyricsFixed_Click(object sender, EventArgs e)
        {
            lyricsKaraoke.Checked = false;
            lyricsFixed.Checked = true;
            lyricsScrolling.Checked = false;
        }

        private void useWholeWords_Click(object sender, EventArgs e)
        {
            useWholeWords.Checked = true;
            useSyllables.Checked = false;
        }

        private void useSyllables_Click(object sender, EventArgs e)
        {
            useSyllables.Checked = true;
            useWholeWords.Checked = false;
        }

        private void exportFNFAudio_Click(object sender, EventArgs e)
        {
            ShowSongSaveMessage((ToolStripMenuItem)(sender), "Fortnite Festival");
        }

        private void ShowSongSaveMessage(ToolStripMenuItem menu, string game)
        {
            if (!menu.Checked) return;
            MessageBox.Show("With this option enabled, Visualizer won't play back the audio of your " + game + " song\nOnce it exports it to an audio file it will stop\nTo get started, drag/drop your song again\n\nIf you want to be able to play the songs instead, you must disable this option", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void fnfWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            BassStream = fnfParser.m4aToBassStream(m4aFilePath, 10);//always 10 channels, no preview allowed here
            if (BassStream == 0)
            {
                MessageBox.Show("File '" + m4aFilePath + "' is not a valid input file", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            //this next bit is an ugly hack but temporary until Ian @ BASS implements a better solution
            //writes the raw opus data to a temporary wav file (fastest encoder) and then reads it back in the StartPlayback function
            BassEnc.BASS_Encode_Start(BassStream, tempFile, BASSEncode.BASS_ENCODE_PCM | BASSEncode.BASS_ENCODE_AUTOFREE, null, IntPtr.Zero);
            while (true)
            {
                var buffer = new byte[20000];
                var c = Bass.BASS_ChannelGetData(BassStream, buffer, buffer.Length);
                if (c <= 0) break;
            }
            Bass.BASS_ChannelFree(BassStream);

            isOpus = false;
            isOgg = false;
            isWAV = false;
            isMP3 = false;
            isM4A = true;            

            Parser.Songs[0].ChannelsDrums = 2;
            Parser.Songs[0].ChannelsBassStart = 0;
            Parser.Songs[0].ChannelsBass = 2;
            Parser.Songs[0].ChannelsBassStart = 2;
            Parser.Songs[0].ChannelsGuitar = 2;
            Parser.Songs[0].ChannelsGuitarStart = 4;
            Parser.Songs[0].ChannelsVocals = 2;
            Parser.Songs[0].ChannelsVocalsStart = 6;
            Parser.Songs[0].ChannelsTotal = 10;
            Parser.Songs[0].OriginalAttenuationValues = "";
            Parser.Songs[0].AttenuationValues = "";
            Parser.Songs[0].PanningValues = "";
            Parser.Songs[0].PreviewStart = 30000;
        }

        private void fnfWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            Height = maxHeight;
            picPlayPause.Cursor = Cursors.Hand;
            picStop.Cursor = Cursors.Hand;
            picWorking.Visible = false;
            PlaybackSeconds = picPreview.Tag.ToString() == "preview" ? 30.0 : 0.0;
            updatePlaybackInstruments();
            StartPlayback();
        }

        private void segoeUIToolStrip_Click(object sender, EventArgs e)
        {
            ActiveFont = "Segoe UI";
            UncheckAllFonts((ToolStripMenuItem)sender);
        }

        private void verdanaToolStrip_Click(object sender, EventArgs e)
        {
            ActiveFont = "Verdana";
            UncheckAllFonts((ToolStripMenuItem)sender);
        }

        private void yearJoystick_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (string.IsNullOrWhiteSpace(txtYear.Text)) return;
            mouseX = MousePosition.X;
            mouseY = MousePosition.Y;
            if (yearJoystick.Cursor == Cursors.Hand)
            {
                yearJoystick.Image = null;
                yearJoystick.Cursor = Cursors.NoMove2D;
            }
            else if (yearJoystick.Cursor == Cursors.NoMove2D)
            {
                yearJoystick.Cursor = Cursors.Hand;
                yearJoystick.Image = Resources.moveall;
            }
        }

        private void yearJoystick_MouseMove(object sender, MouseEventArgs e)
        {
            if (yearJoystick.Cursor != Cursors.NoMove2D) return;
            if (MousePosition.X != mouseX)
            {
                if (MousePosition.X > mouseX)
                {
                    yearX = yearX + (MousePosition.X - mouseX);
                }
                else if (MousePosition.X < mouseX)
                {
                    yearX = yearX - (mouseX - MousePosition.X);
                }
                mouseX = MousePosition.X;
            }
            picVisualizer.Invalidate();
            if (MousePosition.Y == mouseY) return;
            if (MousePosition.Y > mouseY)
            {
                yearY = yearY + (MousePosition.Y - mouseY);
            }
            else if (MousePosition.Y < mouseY)
            {
                yearY = yearY - (mouseY - MousePosition.Y);
            }
            mouseY = MousePosition.Y;
            picVisualizer.Invalidate();
        }

        private void picLastFM_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if ((string.IsNullOrEmpty(songArtist) && string.IsNullOrEmpty(txtArtist.Text)) || (string.IsNullOrEmpty(songAlbum) && string.IsNullOrEmpty(txtAlbum.Text)))
            {
                MessageBox.Show("Last.fm requires Artist and Album name", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrEmpty(songArtist) && !string.IsNullOrEmpty(txtArtist.Text))
            {
                songArtist = txtArtist.Text;
            }
            if (string.IsNullOrEmpty(songAlbum) && !string.IsNullOrEmpty(txtAlbum.Text))
            {
                songAlbum = txtAlbum.Text;
            }
            DownloadAlbumArt();
        }

        private void exportAlbumArtFile_Click(object sender, EventArgs e)
        {
            if (picAlbumArt.Image == null)
            {
                MessageBox.Show("There is no image to export", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            var sfd = new SaveFileDialog
            {
                InitialDirectory = Tools.CurrentFolder,
                Filter = "PNG Image|*.png",
                Title = "Export album art file...",
            };
            sfd.ShowDialog();
            if (string.IsNullOrWhiteSpace(sfd.FileName)) return;
            using (var memoryStream = new MemoryStream())
            {
                // Save the image to the memory stream as PNG (no GDI+ Save called explicitly)
                Bitmap bitmap = new Bitmap(picAlbumArt.Image);
                bitmap.Save(memoryStream, ImageFormat.Png);

                // Write the memory stream to a file
                File.WriteAllBytes(sfd.FileName, memoryStream.ToArray());
            }
        }

        private void autoplayMusicVideos_Click(object sender, EventArgs e)
        {
            if (!autoplayMusicVideos.Checked)
            {
                if (_mediaPlayer.IsPlaying)
                {
                    _mediaPlayer.Stop();
                }
                videoView.Visible = false;
            }
        }
    }   
}

public class DJHeroTrack
{
    public string IDTag { get; set; }
    public string FolderLocation { get; set; }
    public List<string> MixArtists { get; set; }
    public List<string> MixNames { get; set; }
}