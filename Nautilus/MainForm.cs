using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.Windows.Forms;
using Nautilus.Properties;
using Nautilus.x360;
using DevComponents.DotNetBar;
using System.Drawing;
using Cursors = System.Windows.Forms.Cursors;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
using Path = System.IO.Path;
using Point = System.Drawing.Point;
using System.Media;
using System.Globalization;

namespace Nautilus
{
    public partial class MainForm : Office2007Form
    {
        public List<string> Files = new List<string>();
        public Form activeForm;
        public bool arguments;
        private readonly NemoTools Tools;
        private bool Dragging;
        private int mouseX;
        private int mouseY;
        private readonly List<Form> activeForms;
        private readonly string config;
        private Button CurrentButton;
        private readonly List<MyButton> FormButtons;
        private const int form_width = 730;
        private const int form_height = 483;
        private string bg_image = "";
        private Color ActiveBackground;
        private readonly Random Randomizer;
        private bool IncreaseColor;
        private int ActiveRGB;
        public bool isLocked;
        private static Color mMenuBackground;
        private bool showMessage;
        private const int ButtonMinLeft = 2;
        private int ButtonMaxLeft;
        private const int ButtonMinTop = 2;
        private int ButtonMaxTop;
        private const int VerticalJump = 63;
        private const int HorizontalJump = 142;
        private bool MovedButton;
        private bool IsClickingButton;
        private List<int> ButtonColumns;
        private List<int> ButtonRows;
        private readonly Panel HelperLineLeft = new Panel();
        private readonly Panel HelperLineTop = new Panel();
        private readonly Panel lineLeft = new Panel();
        private readonly Panel lineRight = new Panel();
        private readonly Panel lineTop = new Panel();
        private readonly Panel lineBottom = new Panel();
        private readonly string AppName;
        private Image flappy_a;
        private Image flappy_b;

        //flappy game code
        private int gravity_speed = 5;
        private int move_speed = 4;
        private int jump_height = 70;
        private int flying_speed = 7;
        private int column_distance = 400;
        private int forgiveness = 10;
        private Color deadColor = Color.Black;
        private int jump_goal;
        private readonly string flappy_folder;
        private readonly List<PictureBox> columns_a;
        private readonly List<PictureBox> columns_b;
        private bool isPlaying;
        private readonly int center;
        private const int floor = 400;
        private readonly Random randomizer;
        private int CurrentScore;
        private bool isDead;
        private bool isPassingColumn;        
        private readonly string flappyConfig;
        private SoundPlayer WavPlayer;
        private bool GotPoint;
        private int flappyMouseX;
        private int flappyMouseY;
        private bool isPlayingFlappy;
        const int FlappyLeft = 44;
        const int FlappyTop = 383;
        private string GameName;

        public MainForm()
        {
            CheckForIllegalCrossThreadCalls = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            InitializeComponent();
            AppName = Text;
            Tools = new NemoTools();            
            activeForms = new List<Form>();
            ButtonColumns = new List<int>();
            ButtonRows = new List<int>();
            mMenuBackground = contextMenuStrip1.BackColor;
            contextMenuStrip1.Renderer = new DarkRenderer();
            contextMenuStrip2.Renderer = new DarkRenderer();
            VariousFunctions.DeleteTempFiles();
            Randomizer = new Random();
            var buttons = new List<Button>
            {
                btnRBtoUSB, btnPackCreator, btnQuickPackEditor, btnQuickDTAEditor, btnCONCreator, btnCONExplorer,
                btnVisualizer, btnMIDICleaner, btnSongAnalyzer, btnAudioAnalyzer, btnVolumeNormalizer, btnSaveFileImageEditor, btnScores,
                btnSetlistManager, btnBatchExtractor, btnBatchRenamer, btnBatchProcessor, btnEventManager, btnFileIndexer, btnCharEditor,
                btnAdvancedArtConverter, btnCONConverter, btnWiiConverter, btnPS3Converter, btnPhaseShiftConverter, btnRBAEditor, btnMiloMod,
                btnUpgradeBundler, btnStemsIsolator, btnBatchCryptor, btnMoggMaker, btnStudio, btnAudioConverter, btnCDG, btnUltraStarConverter
            };
            FormButtons = new List<MyButton>();
            foreach (var mybutton in buttons.Select(button => new MyButton
            {
                Button = button,
                DefaultLocation = button.Location,
            }))
            {
                FormButtons.Add(mybutton);
            }
            HelperLineLeft = new Panel { Parent = this, Visible = false, Width = 1, Height = Height, Top = 0, BackColor = Color.White };
            HelperLineTop = new Panel { Parent = this, Visible = false, Width = Width, Height = 1, Left = 0, BackColor = Color.White };
            lineLeft = new Panel { Name = "lineLeft", Parent = this, Visible = false, Width = 1, Height = 100, BackColor = Color.Black };
            lineRight = new Panel { Name = "lineRight", Parent = this, Visible = false, Width = 1, Height = 100, BackColor = Color.Black };
            lineLeft.BringToFront();
            lineRight.BringToFront();
            lineTop = new Panel { Name = "lineTop", Parent = this, Visible = false, Width = 100, Height = 1, BackColor = Color.Black };
            lineBottom = new Panel { Name = "lineBottom", Parent = this, Visible = false, Width = 100, Height = 1, BackColor = Color.Black };
            lineTop.BringToFront();
            lineBottom.BringToFront();
            try
            {
                var sw = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Nemo\\Nautilus", false);
                sw.WriteLine(Application.StartupPath + "\\Nautilus.exe");
                sw.Dispose();
            }
            catch (Exception)
            {}
            config = Application.StartupPath + "\\bin\\config\\main500.config";
            if (!Directory.Exists(Application.StartupPath + "\\bin\\config\\"))
            {
                Directory.CreateDirectory(Application.StartupPath + "\\bin\\config\\");
            }

            //flappy code
            flappy_folder = Application.StartupPath + "\\res\\flappy\\";

            randomizer = new Random();
            columns_a = new List<PictureBox> { columnA1, columnA2, columnA3, columnA4, columnA5, columnA6 };
            columns_b = new List<PictureBox> { columnB1, columnB2, columnB3, columnB4, columnB5, columnB6 };
             
            picBackground.Location = new Point(0, 0);
            picBackground.Size = Size;
            picBackground.SizeMode = PictureBoxSizeMode.StretchImage;
            picBackground.SendToBack();

            lblScore.Left = (Width - lblScore.Width) / 2;
            center = (Height - flappy.Height) / 2;
            flappy.Visible = true;
            lblScore.Visible = false;
            lblScore.Parent = picBackground;

            WavPlayer = new SoundPlayer();

            flappy_a = Tools.NemoLoadImage(flappy_folder + "flappy_a.png");
            flappy_b = Tools.NemoLoadImage(flappy_folder + "flappy_b.png");
            flappy.Image = flappy_a;
            flappy.Visible = flappy.Image != null;

            flappyConfig = flappy_folder + "flappy.config";
            if (File.Exists(flappyConfig))
            {
                LoadFlappyConfig();
            }                        
        }

        private void LoadFlappyConfig()
        {
            if (!File.Exists(flappyConfig)) return;

            var sr = new StreamReader(flappyConfig);
            try
            {
                GameName = Tools.GetConfigString(sr.ReadLine());
                //replace - to prevent cheating by using a negative value for gravity, etc
                gravity_speed = Convert.ToInt16(Tools.GetConfigString(sr.ReadLine().Replace("-", "")));
                move_speed = Convert.ToInt16(Tools.GetConfigString(sr.ReadLine().Replace("-", "")));
                jump_height = Convert.ToInt16(Tools.GetConfigString(sr.ReadLine().Replace("-", "")));
                flying_speed = Convert.ToInt16(Tools.GetConfigString(sr.ReadLine().Replace("-", "")));
                column_distance = Convert.ToInt16(Tools.GetConfigString(sr.ReadLine().Replace("-", "")));
                forgiveness = Convert.ToInt16(Tools.GetConfigString(sr.ReadLine().Replace("-", "")));
                deadColor = ColorTranslator.FromHtml(Tools.GetConfigString(sr.ReadLine()));
            }
            catch (Exception)
            {
                MessageBox.Show("The configuration file was not formatted correctly and I deleted it\n\nIf you're going to modify this program follow the format of the one I will create when you close me down",
                    Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                sr.Dispose();
                Tools.DeleteFile(flappyConfig);
                return;
            }
            sr.Dispose();
        }

        private void SaveFlappyConfig()
        {
            try
            {
                var sw = new StreamWriter(flappyConfig, false);
                sw.WriteLine("GameName=" + GameName);
                sw.WriteLine("GravitySpeed=" + gravity_speed);
                sw.WriteLine("HorizontalSpeed=" + move_speed);
                sw.WriteLine("JumpHeight=" + jump_height);
                sw.WriteLine("VerticalSpeed=" + flying_speed);
                sw.WriteLine("ColumnDistance=" + column_distance);
                sw.WriteLine("Forgiveness=" + forgiveness);
                sw.WriteLine("DeadColor=#" + deadColor.A.ToString("X2") + deadColor.R.ToString("X2") + deadColor.G.ToString("X2") + deadColor.B.ToString("X2"));
                sw.Dispose();
            }
            catch
            {};
        }

        private void PlaySound(string sound)
        {
            var soundfile = flappy_folder + sound + ".wav";

            if (!File.Exists(soundfile) || (sound == "flap" && GotPoint)) return;

            WavPlayer = new SoundPlayer(soundfile);
            WavPlayer.Play();

            GotPoint = sound == "point";
            pointTmr.Enabled = GotPoint;
        }

        private void SaveConfig()
        {
            var sw = new StreamWriter(config, false);
            sw.WriteLine("Width=" + (WindowState == FormWindowState.Minimized ? RestoreBounds.Width : Width));
            sw.WriteLine("Height=" + (WindowState == FormWindowState.Minimized ? RestoreBounds.Height : Height));
            sw.WriteLine("BackColor=#" + GetColorHex(BackColor));
            sw.WriteLine("BackgroundImage=" + bg_image);
            var i = 0;
            foreach (var button in FormButtons)
            {
                i++;
                sw.WriteLine("Button" + i + "TextColor=#" + GetColorHex(button.Button.ForeColor));
                sw.WriteLine("Button" + i + "BackColor=#" + GetColorHex(button.Button.BackColor));
                sw.WriteLine("Button" + i + "LocX=" + button.Button.Left);
                sw.WriteLine("Button" + i + "LocY=" + button.Button.Top);
                sw.WriteLine("Button" + i + "Visible=" + button.Button.Visible);
            }
            sw.WriteLine("BorderlessForm=" + borderlessForm.Checked);
            sw.WriteLine("ActiveColor=" + activeColorToolStripMenuItem.Checked);
            sw.WriteLine("WarnAboutAdminMode=" + administratorModeWarning.Checked);
            sw.WriteLine("ShowMario=" + flappy.Visible);
            sw.Dispose();
        }

        private static string GetColorHex(Color color)
        {
            return color.A.ToString("X2") + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {            
            if (isLocked)
            {
                e.Cancel = true;
                MessageBox.Show("You must unlock the program first", "Locked", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            WavPlayer?.Stop();
            //resetEverythingToolStripMenuItem_Click(sender, e);
            SaveConfig();
            SaveFlappyConfig();
            VariousFunctions.DeleteTempFiles();
            Tools.DeleteFolder(Application.StartupPath + "\\videoprep_input\\", true);
            Tools.DeleteFolder(Application.StartupPath + "\\videoprep_temp\\", true);
            Tools.DeleteFolder(Application.StartupPath + "\\wiiprep_input\\");
            Tools.DeleteFolder(Application.StartupPath + "\\visualizer\\", true);
            Tools.DeleteFolder(Application.StartupPath + "\\packex\\", true);
            //Tools.DeleteFolder(Application.StartupPath + "\\input\\", true);
            Tools.DeleteFolder(Application.StartupPath + "\\extracted\\", true);
            Tools.DeleteFolder(Application.StartupPath + "\\quickpackeditor\\", true);
            Tools.DeleteFolder(Application.StartupPath + "\\temp\\",true);
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[]) e.Data.GetData(DataFormats.FileDrop);
            Tools.CurrentFolder = Path.GetDirectoryName(files[0]);
            try
            {
                var counter = 0;
                foreach (var toLoad in files)
                {
                    if (counter == 10)
                    {
                        MessageBox.Show("You already opened 10 other song files.\nTo make sure I don't burn down your computer,\nI'm going to stop loading any more.\nFeel free to load 10 more when you're done\n with the 10 you have open.",
                            "Too many files!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    try
                    {
                        if (VariousFunctions.ReadFileType(toLoad) == XboxFileType.STFS)
                        {
                            var xExplorer = new CONExplorer(btnCONCreator.BackColor, btnCONCreator.ForeColor);
                            xExplorer.LoadCON(toLoad);
                            activeForm = xExplorer;
                            activeForms.Add(activeForm);
                            xExplorer.Show();
                            counter++;
                        }
                        else switch (Path.GetExtension(toLoad.ToLowerInvariant()))
                        {
                            case ".dta":
                                RunQuickPackEditor(toLoad, "");
                                break;
                            case ".mid":
                                RunMIDICleaner(toLoad);
                                break;
                            case ".mogg":
                            case ".ogg":
                            case ".wav":
                                RunAudioAnalyzer(toLoad);
                                break;
                            case ".setlist":
                                var newManager = new SetlistManager(btnSetlistManager.BackColor,btnSetlistManager.ForeColor,toLoad);
                                activeForm = newManager;
                                activeForms.Add(activeForm);
                                newManager.Show();
                                break;
                            case ".png":
                            case ".gif":
                            case ".jpeg":
                            case ".jpg":
                            case ".bmp":
                                bg_image = toLoad;
                                LoadBackground();
                                break;
                            case ".png_xbox":
                            case ".png_ps3":
                            case ".png_wii":
                            case ".dds":
                            case ".tpl":
                                var newAlbum = new AdvancedArtConverter(Path.GetDirectoryName(toLoad),btnAdvancedArtConverter.BackColor,btnAdvancedArtConverter.ForeColor);
                                activeForm = newAlbum;
                                activeForms.Add(activeForm);
                                newAlbum.Show();
                                break;
                            case ".config":
                                if (toLoad.EndsWith("main.config", StringComparison.Ordinal))
                                {
                                    LoadConfig(toLoad);
                                }
                                break;
                            case ".rba":
                            case ".rbs":
                                RunRBAEditor(toLoad);
                                break;
                            default:
                                MessageBox.Show("That's not a valid file to drag and drop here ... try again","Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("There was an error accessing that file\nThe error says:\n" + ex.Message, Text,MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error opening that file!\nThe most likely cause: this file is already open!\n\nIn case that's not it, here's the error:\n\n" +
                    ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void LoadBackground()
        {
            if (string.IsNullOrWhiteSpace(bg_image) || !File.Exists(bg_image))
            {
                switch (bg_image)
                {
                    case "default":
                        BackgroundImage = Resources.bg;
                        break;
                    case "default_old":
                        BackgroundImage = Resources.bg3;
                        break;
                    default:
                        BackgroundImage = null;
                        break;
                }
                return;
            }
            timer1.Enabled = false;
            activeColorToolStripMenuItem.Checked = false;
            TransparencyKey = Color.Empty;
            BackgroundImage = Tools.NemoLoadImage(bg_image);
            ResizeBorderLines();
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
        }
        
        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (MovedButton) return;
            var xCreate = new CONCreator(btnCONCreator.BackColor, btnCONCreator.ForeColor);
            activeForm = xCreate;
            activeForms.Add(activeForm);
            xCreate.Show();
        }

        private void btnPackager_Click(object sender, EventArgs e)
        {
            if (MovedButton) return;
            var newPackager = new PackCreator(this, btnPackCreator.BackColor, btnPackCreator.ForeColor);
            activeForm = newPackager;
            activeForms.Add(activeForm);
            newPackager.Show();
        }

        private void btnArt_Click(object sender, EventArgs e)
        {
            if (MovedButton) return;
            var newAlbum = new AdvancedArtConverter("", btnAdvancedArtConverter.BackColor, btnAdvancedArtConverter.ForeColor);
            activeForm = newAlbum;
            activeForms.Add(activeForm);
            newAlbum.Show();
        }

        private void btnVisualizer_Click(object sender, EventArgs e)
        {
            if (MovedButton) return;
            RunVisualizer("");
        }

        private void RunVisualizer(string con_file)
        {
            //clear temp folder if it exists
            Tools.DeleteFolder(Application.StartupPath + "\\visualizer\\", true);
            var xVisualizer = new Visualizer(btnVisualizer.BackColor, btnVisualizer.ForeColor, con_file);
            activeForm = xVisualizer;
            activeForms.Add(activeForm);
            xVisualizer.Show();
        }

        private void MainForm_Click(object sender, EventArgs e)
        {
            if (activeForm == null) return;
            activeForm.WindowState = FormWindowState.Normal;
            activeForm.Focus();
        }        

        private void LoadConfig(string file = "")
        {
            if (!File.Exists(config) && string.IsNullOrWhiteSpace(file)) return;
            int width;
            int height;
            bool activebgcolor;
            var sr = new StreamReader(string.IsNullOrWhiteSpace(file) ? config : file);
            try
            {
                width = Convert.ToInt16(Tools.GetConfigString(sr.ReadLine()));
                height = Convert.ToInt16(Tools.GetConfigString(sr.ReadLine()));                
                var line = sr.ReadLine();
                if (line.Contains("#"))
                {
                    BackColor = ColorTranslator.FromHtml(Tools.GetConfigString(line));
                }
                bg_image = Tools.GetConfigString(sr.ReadLine());
                foreach (var button in FormButtons)
                {
                    button.Button.ForeColor = ColorTranslator.FromHtml(Tools.GetConfigString(sr.ReadLine()));
                    button.Button.BackColor = ColorTranslator.FromHtml(Tools.GetConfigString(sr.ReadLine()));
                    button.Button.FlatAppearance.MouseOverBackColor = button.Button.BackColor == Color.Transparent ? Color.FromArgb(127, Color.AliceBlue.R, Color.AliceBlue.G, Color.AliceBlue.B) : Tools.LightenColor(button.Button.BackColor);
                    button.Button.Left = Convert.ToInt16(Tools.GetConfigString(sr.ReadLine()));
                    button.Button.Top = Convert.ToInt16(Tools.GetConfigString(sr.ReadLine()));
                    button.Button.Visible = sr.ReadLine().Contains("True");
                }         
                borderlessForm.Checked = sr.ReadLine().Contains("True"); //borderless form?
                activebgcolor = sr.ReadLine().Contains("True");
                administratorModeWarning.Checked = sr.ReadLine().Contains("True");
                flappy.Visible = sr.ReadLine().Contains("True");
            }
            catch (Exception)
            {
                sr.Dispose();
                Tools.DeleteFile(config);
                resetEverythingToolStrip.PerformClick();
                return;
            }
            sr.Dispose();
            ChangeFormBorder(borderlessForm.Checked);
            Height = height;
            Width = width;
            Left = (Screen.PrimaryScreen.WorkingArea.Width - Width) / 2;
            Top = (Screen.PrimaryScreen.WorkingArea.Height - Height) / 2;
            if (activebgcolor)
            {
                activeColorToolStripMenuItem.PerformClick();
                return;
            }
            LoadBackground();
            TransparencyKey = Color.Empty;
            if (flappy.Visible)
            {
                flappyTmr.Enabled = true;
            }
            if (BackColor == Color.Black)
            {
                flappy.Image = flappy_b;
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            LoadConfig();
            if (administratorModeWarning.Checked && IsAdministratorMode())
            {
                MessageBox.Show("You are running " + Text + " in Administrator Mode!\nIn this mode, Windows disables drag/drop functionality and you will lose " +
                                " many of the features added for your convenience\nIt's recommended you do not run " + Text + " in Administrator Mode in future use", 
                                Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            if (Application.StartupPath.Contains("Program Files"))
            {
                MessageBox.Show("You are running " + Text + " from Program Files!\nThis can cause permission problems and some of the features will not work correctly or at all " +
                    "\nIt's recommended you move " + Text + " to a different folder for future use", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            updater.RunWorkerAsync();
            //btnScores.Visible = File.Exists(Application.StartupPath + "\\bin\\RB3SaveScoresViewer.exe");
            //btnMiloMod.Visible = File.Exists(Application.StartupPath + "\\bin\\MiloMod.exe");
            //btnCharEditor.Visible = File.Exists(Application.StartupPath + "\\bin\\CharEditor.exe");
            flappyTmr.Enabled = true;

            /*var k50 = Application.StartupPath + "\\bin\\50k.c3";
            var gif = Application.StartupPath + "\\res\\50k.gif";
            if (!File.Exists(k50) && File.Exists(gif))
            {
                foreach (var button in FormButtons)
                {
                    button.Button.Visible = false;
                }
                flappy.Visible = false;
                picBackground.Image = Image.FromFile(gif);
                gifTmr.Enabled = true;
                File.Create(k50);
            }*/
            if (Tools.IsRunningOnWine() && !File.Exists(Application.StartupPath + "\\bin\\wine"))
            {
                var wine = new Wine();
                wine.Show();
            }
        }

        private void btnExtractor_Click(object sender, EventArgs e)
        {
            if (MovedButton) return;
            var newExtractor = new BatchExtractor(btnBatchExtractor.BackColor, btnBatchExtractor.ForeColor);
            activeForm = newExtractor;
            activeForms.Add(activeForm);
            newExtractor.Show();
        }        
        
        private void btnWiiPrep_Click(object sender, EventArgs e)
        {
            if (MovedButton) return;
            var newWiiPrep = new WiiConverter(btnWiiConverter.BackColor, btnWiiConverter.ForeColor, "");
            activeForm = newWiiPrep;
            activeForms.Add(activeForm);
            newWiiPrep.Show();
        }

        private void btnPhaseShift_Click(object sender, EventArgs e)
        {
            if (MovedButton) return;
            var newPhaseShiftPrep = new PhaseShiftConverter(btnPhaseShiftConverter.BackColor, btnPhaseShiftConverter.ForeColor);
            activeForm = newPhaseShiftPrep;
            activeForms.Add(activeForm);
            newPhaseShiftPrep.Show();
        }

        private void MainForm_Move(object sender, EventArgs e)
        {
            if (activeForm == null) return;
            try
            {
                Dragging = true;
                activeForm.Left = Left + ((Width - activeForm.Width)/2);
                activeForm.Top = Top + ((Height - activeForm.Height)/2);
                activeForm.Focus();
            }
            catch (Exception)
            {
                activeForm = null;
            }
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            if (!Dragging) return;
            MainForm_Click(null, null);
            Dragging = false;
        }

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (isPlayingFlappy)
            {
                if (e.Button == MouseButtons.Left && isDead)
                {
                    Cursor = Cursors.NoMove2D;
                    mouseX = MousePosition.X;
                    mouseY = MousePosition.Y;
                }
                if (isDead) return;
                doPlay();
                return;
            }
            if (e.Button != MouseButtons.Left) return;
            Cursor = Cursors.NoMove2D;
            mouseX = MousePosition.X;
            mouseY = MousePosition.Y;
        }

        private void doPlay()
        {
            if (!isPlaying)
            {
                checkCollision.Enabled = true;
                move.Enabled = true;
            }

            PlaySound("flap");
            isPlaying = true;
            gravity.Enabled = false;
            jump_goal = flappy.Top - jump_height;
            jump.Enabled = true;
        }

        private void LoadFlappyGraphics()
        {
            try
            {
                //NemoLoadImage is a safer way to load the image into memory and let go of the resource
                //But it turns the image into a static bitmap, so for animated gifs we'll use the Image.FromFile instead
                picBackground.Image = File.Exists(flappy_folder + "bg.gif") ? Image.FromFile(flappy_folder + "bg.gif") : Tools.NemoLoadImage(flappy_folder + "bg.png");
                var bGraphics = picBackground.CreateGraphics();
                bGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                bGraphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;

                flappy.BackColor = Color.Transparent;
                flappy.Parent = picBackground;

                columns_a[0].Image = File.Exists(flappy_folder + "purple.gif") ? Image.FromFile(flappy_folder + "purple.gif") : Tools.NemoLoadImage(flappy_folder + "purple.png");
                var ca0Graphics = columns_a[0].CreateGraphics();
                ca0Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                ca0Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                columns_a[1].Image = File.Exists(flappy_folder + "orange.gif") ? Image.FromFile(flappy_folder + "orange.gif") : Tools.NemoLoadImage(flappy_folder + "orange.png");
                var ca1Graphics = columns_a[1].CreateGraphics();
                ca1Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                ca1Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                columns_a[2].Image = File.Exists(flappy_folder + "blue.gif") ? Image.FromFile(flappy_folder + "blue.gif") : Tools.NemoLoadImage(flappy_folder + "blue.png");
                var ca2Graphics = columns_a[2].CreateGraphics();
                ca2Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                ca2Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                columns_a[3].Image = File.Exists(flappy_folder + "yellow.gif") ? Image.FromFile(flappy_folder + "yellow.gif") : Tools.NemoLoadImage(flappy_folder + "yellow.png");
                var ca3Graphics = columns_a[3].CreateGraphics();
                ca3Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                ca3Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                columns_a[4].Image = File.Exists(flappy_folder + "red.gif") ? Image.FromFile(flappy_folder + "red.gif") : Tools.NemoLoadImage(flappy_folder + "red.png");
                var ca4Graphics = columns_a[4].CreateGraphics();
                ca4Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                ca4Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                columns_a[5].Image = File.Exists(flappy_folder + "green.gif") ? Image.FromFile(flappy_folder + "green.gif") : Tools.NemoLoadImage(flappy_folder + "green.png");
                var ca5Graphics = columns_a[5].CreateGraphics();
                ca5Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                ca5Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;

                columns_b[0].Image = columns_a[0].Image;
                var cb0Graphics = columns_b[0].CreateGraphics();
                cb0Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                cb0Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                columns_b[1].Image = columns_a[1].Image;
                var cb1Graphics = columns_b[1].CreateGraphics();
                cb1Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                cb1Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                columns_b[2].Image = columns_a[2].Image;
                var cb2Graphics = columns_b[2].CreateGraphics();
                cb2Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                cb2Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                columns_b[3].Image = columns_a[3].Image;
                var cb3Graphics = columns_b[3].CreateGraphics();
                cb3Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                cb3Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                columns_b[4].Image = columns_a[4].Image;
                var cb4Graphics = columns_b[4].CreateGraphics();
                cb4Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                cb4Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                columns_b[5].Image = columns_a[5].Image;
                var cb5Graphics = columns_b[5].CreateGraphics();
                cb5Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                cb5Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;

                GoldStarA.Image = File.Exists(flappy_folder + "star.gif") ? Image.FromFile(flappy_folder + "star.gif") : Tools.NemoLoadImage(flappy_folder + "star.png");
                GoldStarA.BackColor = Color.Transparent;
                GoldStarA.Parent = picBackground;
                GoldStarA.BringToFront();
                var gsaGraphics = GoldStarA.CreateGraphics();
                gsaGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                gsaGraphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                GoldStarB.Image = GoldStarA.Image;
                GoldStarB.BackColor = Color.Transparent;
                GoldStarB.Parent = picBackground;
                GoldStarB.BringToFront();
                var gsbGraphics = GoldStarB.CreateGraphics();
                gsbGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                gsbGraphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;

                foreach (var column in columns_a)
                {
                    column.BackColor = Color.Transparent;
                    column.BringToFront();
                    column.Parent = picBackground;
                }
                foreach (var column in columns_b)
                {
                    column.BackColor = Color.Transparent;
                    column.BringToFront();
                    column.Parent = picBackground;
                }

                foreach (var button in FormButtons)
                {
                    button.Button.Parent = picBackground;
                }

                flappy.BringToFront();
                lblScore.BringToFront();

                var fGraphics = flappy.CreateGraphics();
                fGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                fGraphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error loading image resources\n\n" + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MainForm_MouseUp(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Default;
        }

        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (Cursor != Cursors.NoMove2D) return;
            if (MousePosition.X != mouseX)
            {
                if (MousePosition.X > mouseX)
                {
                    Left = Left + (MousePosition.X - mouseX);
                }
                else if (MousePosition.X < mouseX)
                {
                    Left = Left - (mouseX - MousePosition.X);
                }
                mouseX = MousePosition.X;
            }
            if (MousePosition.Y == mouseY) return;
            if (MousePosition.Y > mouseY)
            {
                Top = Top + (MousePosition.Y - mouseY);
            }
            else if (MousePosition.Y < mouseY)
            {
                Top = Top - (mouseY - MousePosition.Y);
            }
            mouseY = MousePosition.Y;
        }

        private void btnMIDI_Click(object sender, EventArgs e)
        {
            if (MovedButton) return;
            RunMIDICleaner("");
        }

        private void RunMIDICleaner(string file)
        {
            var newMIDICleaner = new MIDICleaner(file, btnMIDICleaner.BackColor, btnMIDICleaner.ForeColor);
            activeForm = newMIDICleaner;
            activeForms.Add(activeForm);
            newMIDICleaner.Show();
        }

        private void btnQuickDTA_Click(object sender, EventArgs e)
        {
            if (MovedButton) return;
            RunQuickDTAEditor("");
        }

        private void RunQuickDTAEditor(string file)
        {
            var newQuickDTA = new QuickDTAEditor(file);
            activeForm = newQuickDTA;
            activeForms.Add(activeForm);
            newQuickDTA.Show();
        }

        private void btnQuickPack_Click(object sender, EventArgs e)
        {
            if (MovedButton) return;
            RunQuickPackEditor("", "");
        }

        private void RunQuickPackEditor(string dta, string pack)
        {
            var newQuickPack = new QuickPackEditor(this, btnQuickPackEditor.BackColor, btnQuickPackEditor.ForeColor, dta, pack);
            activeForm = newQuickPack;
            activeForms.Add(activeForm);
            newQuickPack.Show();
        }

        private void btnSetlist_Click(object sender, EventArgs e)
        {
            if (MovedButton) return;
            var newSetlistManager = new SetlistManager(btnSetlistManager.BackColor, btnSetlistManager.ForeColor, "", this);
            activeForm = newSetlistManager;
            activeForms.Add(activeForm);
            newSetlistManager.Show();
        }

        private void btnPS3_Click(object sender, EventArgs e)
        {
            if (MovedButton) return;
            if (!File.Exists(Application.StartupPath + "\\bin\\nemoedat.exe"))
            {
                MessageBox.Show("Required file 'nemoedat.exe' is missing!\nDownload it and place it in the \\bin\\ directory, then try to run " +
                                "this tool again", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var newPS3Prep = new PS3Converter(this,  btnPS3Converter.BackColor, btnPS3Converter.ForeColor);
            activeForm = newPS3Prep;
            activeForms.Add(activeForm);
            newPS3Prep.Show();
        }

        private void defaultColor_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            activeColorToolStripMenuItem.Checked = false;
            BackColor = Color.LightBlue;
        }

        private void customColor_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            activeColorToolStripMenuItem.Checked = false;
            BackColor = ColorPicker(BackColor, false);
        }

        private Color ColorPicker(Color initialcolor, bool isBackColor)
        {
            var alpha = isBackColor ? 200 : 255;
            colorDialog1.Color = initialcolor;
            colorDialog1.CustomColors = new[] { ColorTranslator.ToOle(initialcolor), ColorTranslator.ToOle(Color.FromArgb(alpha, 34, 169, 31)), ColorTranslator.ToOle(Color.FromArgb(alpha, 197, 34, 35)), ColorTranslator.ToOle(Color.FromArgb(alpha, 230, 215, 0)), ColorTranslator.ToOle(Color.FromArgb(alpha, 37, 89, 201)), ColorTranslator.ToOle(Color.FromArgb(alpha, 240, 104, 4)) };
            colorDialog1.SolidColorOnly = false;
            colorDialog1.AnyColor = true;
            colorDialog1.FullOpen = true;
            colorDialog1.AllowFullOpen = true;
            return colorDialog1.ShowDialog() == DialogResult.OK ? Color.FromArgb(alpha, colorDialog1.Color.R, colorDialog1.Color.G, colorDialog1.Color.B) : initialcolor;
        }

        private void customColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentButton.BackColor = ColorPicker(CurrentButton.BackColor, true);
            CurrentButton.FlatAppearance.MouseOverBackColor = Tools.LightenColor(CurrentButton.BackColor);
        }

        private static Control MenuSource(object sender)
        {
            // Retrieve the ContextMenuStrip that called the function
            var owner = sender as ContextMenuStrip;
            // Get the control that is displaying this context menu
            return owner.SourceControl;
        }

        private void customColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CurrentButton.ForeColor = ColorPicker(CurrentButton.ForeColor, false);
        }

        private void defaultColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CurrentButton.ForeColor = Color.Black;//Color.White;
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CurrentButton = (Button)MenuSource(sender);
            CurrentButton.BringToFront();
            moveLeft.Enabled = CurrentButton.Left != ButtonMinLeft;
            moveRight.Enabled = CurrentButton.Left != ButtonMaxLeft;
            moveUp.Enabled = CurrentButton.Top != ButtonMinTop;
            moveDown.Enabled = CurrentButton.Top != ButtonMaxTop;
        }

        private void defaultColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var greens = new List<Button> { btnRBtoUSB, btnPackCreator, btnQuickPackEditor, btnQuickDTAEditor, btnCONCreator, btnCONExplorer };
            var reds = new List<Button> { btnVisualizer, btnSongAnalyzer, btnAudioAnalyzer, btnMIDICleaner, btnMoggMaker, btnAudioConverter, btnVolumeNormalizer };
            var yellows = new List<Button> { btnSetlistManager, btnBatchExtractor, btnBatchRenamer, btnBatchProcessor, btnEventManager, btnFileIndexer, btnSaveFileImageEditor };
            var blues = new List<Button> { btnAdvancedArtConverter, btnCONConverter, btnWiiConverter, btnPS3Converter, btnPhaseShiftConverter, btnRBAEditor, btnCDG, btnUltraStarConverter };
            var oranges = new List<Button> { btnUpgradeBundler, btnStemsIsolator, btnBatchCryptor, btnStudio, btnCharEditor, btnMiloMod, btnScores };

            if (greens.Contains(CurrentButton))
            {
                CurrentButton.BackColor = Color.FromArgb(200, 90, 195, 73);
            }
            else if (reds.Contains(CurrentButton))
            {
                CurrentButton.BackColor = Color.FromArgb(200, 208, 84, 86);
            }
            else if (yellows.Contains(CurrentButton))
            {
                CurrentButton.BackColor = Color.FromArgb(200, 251, 211, 0);
            }
            else if (blues.Contains(CurrentButton))
            {
                CurrentButton.BackColor = Color.FromArgb(200, 95, 129, 216);
            }
            else if (oranges.Contains(CurrentButton))
            {
                CurrentButton.BackColor = Color.FromArgb(200, 238, 144, 51);
            }
            //btnSettings.BackColor = Color.FromArgb(200, 151, 54, 189);
            CurrentButton.FlatAppearance.MouseOverBackColor = Tools.LightenColor(CurrentButton.BackColor);
        }
        
        private void ChangeFormBorder(bool borderless)
        {
            FormBorderStyle = borderless ? FormBorderStyle.None : FormBorderStyle.Sizable;//FormBorderStyle.FixedSingle;
            borderlessForm.Checked = borderless;
            ResizeBorderLines();
        }

        private void ResizeBorderLines()
        {
            var visible = true; //FormBorderStyle == FormBorderStyle.None;
            var lines = new List<Panel> { lineTop, lineBottom, lineLeft, lineRight };
            const int line_size = 4;
            foreach (var line in lines)
            {
                line.Visible = visible;
                switch (line.Name)
                {
                    case "lineBottom":
                    case "lineTop":
                        line.Width = Width;
                        line.Height = line_size;
                        line.Left = 0;
                        break;
                    case "lineLeft":
                    case "lineRight":
                        line.Height = Height;
                        line.Top = 0;
                        line.Width = line_size;
                        break;
                }
            }
            var vspacer = FormBorderStyle == FormBorderStyle.None ? 0 : 39;
            var hspacer = FormBorderStyle == FormBorderStyle.None ? 0 : 16;
            lineBottom.Top = Height - line_size - vspacer;
            lineTop.Top = 0;
            lineLeft.Left = 0;
            lineRight.Left = Width - line_size - hspacer;
        }

        private void resetEverythingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetEverything(false);
        }

        private void ResetEverything(bool doNew)
        {
            timer1.Enabled = false;
            Text = AppName;
            activeColorToolStripMenuItem.Checked = false;
            BackColor = doNew? Color.LightBlue : Color.Black; //need to decide on what backcolor for new look
            BackgroundImage = null;
            bg_image = null;          
            flappy.Visible = true;
            flappy.Image = doNew ? flappy_a : flappy_b; 
            ChangeFormBorder(false);
            Width = form_width;
            Height = form_height;
            TransparencyKey = Color.Empty;
            alwaysOnTop.Checked = false;
            TopMost = false;
            foreach (var button in FormButtons)
            {
                button.Button.Location = button.DefaultLocation;
                button.Button.ForeColor = Color.Black;
                CurrentButton = button.Button;
                if (doNew)
                {
                    allButtonsTransparencyToolStripMenuItem_Click(null, null);
                }
                else
                {
                    defaultColorToolStripMenuItem_Click(null, null);
                }
            }
            FormBorderStyle = FormBorderStyle.Sizable;// FormBorderStyle.FixedSingle;
            Lost(true);
            picBackground.Image = null;
            flappyTmr.Enabled = true;
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BackgroundImage = null;
            bg_image = "";
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.gif;*.png",
                Title = "Select an image",
                InitialDirectory = Tools.CurrentFolder
            };
            if (ofd.ShowDialog() != DialogResult.OK) return;
            bg_image = ofd.FileName;
            LoadBackground();
        }

        private void allButtonsDefaultColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var button in FormButtons)
            {
                button.Button.ForeColor = Color.Black;//Color.White;
            }
        }

        private void allButtonsCustomColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var color = ColorPicker(CurrentButton.ForeColor, false);
            if (color == Color.Empty) return; //user canceled
            foreach (var button in FormButtons)
            {
                button.Button.ForeColor = color;
            }
        }

        private void allButtonsCustomColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var color = ColorPicker(CurrentButton.BackColor, true);
            if (color == Color.Empty) return; //user canceled
            foreach (var button in FormButtons)
            {
                button.Button.BackColor = color;
                button.Button.FlatAppearance.MouseOverBackColor = Tools.LightenColor(button.Button.BackColor);
            }
        }

        private void allButtonsDefaultColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            foreach (var button in FormButtons)
            {
                CurrentButton = button.Button;
                defaultColorToolStripMenuItem_Click(sender, e);
                CurrentButton.FlatAppearance.MouseOverBackColor = Tools.LightenColor(CurrentButton.BackColor);
            }
        }

        private void thisButtonTransparencyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentButton.BackColor = Color.Transparent;
            CurrentButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(127, Color.AliceBlue.R, Color.AliceBlue.G, Color.AliceBlue.B);
        }

        private void allButtonsTransparencyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var button in FormButtons)
            {
                button.Button.BackColor = Color.Transparent;
                button.Button.FlatAppearance.MouseOverBackColor = Color.FromArgb(127, Color.AliceBlue.R, Color.AliceBlue.G, Color.AliceBlue.B);
            }
        }               

        private void borderlessFormToolStrip_Click(object sender, EventArgs e)
        {
            ChangeFormBorder(borderlessForm.Checked);
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && FormBorderStyle == FormBorderStyle.None)
            {
                Close();
            }
        }
       
        private void MainForm_Resize(object sender, EventArgs e)
        {
            ResizeBorderLines();
            if (borderlessForm.Checked)
            {
                ResizeBorderLines();
            }
            ButtonMaxLeft = Width - 2 - btnVisualizer.Width;
            ButtonMaxTop = Height - 2 - btnVisualizer.Height;
            HelperLineLeft.Height = Height;
            HelperLineTop.Width = Width;
            picBackground.Size = Size;
        }
        
        private void contextMenuStrip2_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {            
            checkForUpdates.Visible = !updater.IsBusy;
            changeBackgroundColor.Enabled = BackgroundImage == null;
            clearToolStripMenuItem.Enabled = BackgroundImage != null;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        private void btnSaveFileImageViewer_Click(object sender, EventArgs e)
        {
            if (MovedButton) return;
            var newViewer = new SaveFileImageEditor(btnSaveFileImageEditor.BackColor, btnSaveFileImageEditor.ForeColor);
            activeForm = newViewer;
            activeForms.Add(activeForm);
            newViewer.Show();
        }

        private void btnRBAConverter_Click(object sender, EventArgs e)
        {
            if (MovedButton) return;
            var newRBAConverter = new RBAConverter(btnCONConverter.BackColor, btnCONConverter.ForeColor);
            activeForm = newRBAConverter;
            activeForms.Add(activeForm);
            newRBAConverter.Show();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var colors = new List<int> {ActiveBackground.R, ActiveBackground.G, ActiveBackground.B};
            const int add = 5;
            const int max = 255;
            const int min = 0;
            if (IncreaseColor)
            {
                if (colors[ActiveRGB] < max)
                {
                    colors[ActiveRGB] += add;
                }
                else
                {
                    IncreaseColor = Randomizer.Next(0, 9999) % 2 != 0;
                    ActiveRGB = Randomizer.Next(0, colors.Count);
                    return;
                }
            }
            else
            {
                if (colors[ActiveRGB] > min)
                {
                    colors[ActiveRGB] -= add;
                }
                else
                {
                    IncreaseColor = Randomizer.Next(0, 9999) % 2 != 0;
                    ActiveRGB = Randomizer.Next(0, colors.Count);
                    return;
                }
            }
            for (var c = 0; c < colors.Count; c++)
            {
                if (colors[c] < min)
                {
                    colors[c] = min;
                }
                else if (colors[c] > max)
                {
                    colors[c] = max;
                }
            }
            ActiveBackground = Color.FromArgb(colors[0], colors[1], colors[2]);
            BackColor = ActiveBackground;
        }

        private void activeColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ActiveBackground = BackColor;
            timer1.Enabled = true;
        }
        
        private void btnAnalyzer_Click(object sender, EventArgs e)
        {
            if (MovedButton) return;
            RunSongAnalyzer("");
        }

        private void RunSongAnalyzer(string file)
        {
            var newAnalyzer = new SongAnalyzer(file);
            activeForm = newAnalyzer;
            activeForms.Add(activeForm);
            newAnalyzer.Show();
        }

        private void RunAudioAnalyzer(string file)
        {
            var analyzer = new AudioAnalyzer(file);
            activeForm = analyzer;
            activeForms.Add(activeForm);
            analyzer.Show();
        }

        private void btnIndexer_Click(object sender, EventArgs e)
        {
            if (MovedButton) return;
            var newIndexer = new FileIndexer(btnFileIndexer.BackColor, btnFileIndexer.ForeColor);
            activeForm = newIndexer;
            activeForms.Add(activeForm);
            newIndexer.Show();
        }

        private void btnEvent_Click(object sender, EventArgs e)
        {
            if (MovedButton) return;
            var newEventManager = new EventManager(this);
            activeForm = newEventManager;
            activeForms.Add(activeForm);
            newEventManager.Show();
        }

        private void btnWii_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files[0].ToLowerInvariant().EndsWith(".bin", StringComparison.Ordinal))
            {
                Tools.CurrentFolder = Path.GetDirectoryName(files[0]);
                Tools.ProcessBINFile(files[0], Text);
            }
            else
            {
                MainForm_DragDrop(sender, e);
            }
        }

        private void btnVisualizer_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (VariousFunctions.ReadFileType(files[0]) == XboxFileType.STFS)
            {
                RunVisualizer(files[0]);
            }
            else
            {
                MainForm_DragDrop(sender, e);
            }
        }

        private void btnCleaner_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files[0].ToLowerInvariant().EndsWith(".mid", StringComparison.Ordinal) || (VariousFunctions.ReadFileType(files[0]) == XboxFileType.STFS))
            {
                RunMIDICleaner(files[0]);
            }
            else
            {
                MainForm_DragDrop(sender, e);
            }
        }

        private void btnAnalyzer_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files[0].ToLowerInvariant().EndsWith(".mid", StringComparison.Ordinal) || (VariousFunctions.ReadFileType(files[0]) == XboxFileType.STFS))
            {
                RunSongAnalyzer(files[0]);
            }
            else
            {
                MainForm_DragDrop(sender, e);
            }
        }

        private void btnQuickPack_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files[0].ToLowerInvariant().EndsWith(".dta", StringComparison.Ordinal))
            {
                RunQuickPackEditor(files[0], "");
            }
            else if (VariousFunctions.ReadFileType(files[0]) == XboxFileType.STFS)
            {
                RunQuickPackEditor("", files[0]);
            }
            else
            {
                MainForm_DragDrop(sender, e);
            }
        }

        private void btnQuickDTA_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (VariousFunctions.ReadFileType(files[0]) == XboxFileType.STFS)
            {
                RunQuickDTAEditor(files[0]);
            }
            else
            {
                MainForm_DragDrop(sender, e);
            }
        }
        
        private static string GetAppVersion()
        {
            var vers = Assembly.GetExecutingAssembly().GetName().Version;
            return "v" + String.Format("{0}.{1}.{2}", vers.Major, vers.Minor, vers.Build);
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
        
        private void btnUSB_Click(object sender, EventArgs e)
        {
            if (MovedButton) return;
            var newUSB = new RBtoUSB();
            activeForm = newUSB;
            activeForms.Add(activeForm);
            newUSB.Show();
        }

        private void btnStems_Click(object sender, EventArgs e)
        {
            if (MovedButton) return;            
            var newStems = new StemsIsolator(btnStemsIsolator.BackColor, btnStemsIsolator.ForeColor);
            activeForm = newStems;
            activeForms.Add(activeForm);
            newStems.Show();
        }

        private void btnCryptor_Click(object sender, EventArgs e)
        {
            if (MovedButton) return;            
            var cryptor = new Cryptor(btnBatchCryptor.BackColor, btnBatchCryptor.ForeColor);
            activeForm = cryptor;
            activeForms.Add(activeForm);
            cryptor.Show();
        }

        private void updater_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            var path = Application.StartupPath + "\\bin\\update.txt";
            Tools.DeleteFile(path);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;
            using (var client = new WebClient())
            {
                try
                {
                    client.DownloadFile("https://nemosnautilus.com/nautilus/update.txt", path);
                }
                catch (Exception)
                { }
            }
        }         

        private void updater_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            var path = Application.StartupPath + "\\bin\\update.txt";
            if (!File.Exists(path))
            {
                if (showMessage)
                {
                    MessageBox.Show("Unable to check for updates, try again later", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                return;
            }
            var thisVersion = GetAppVersion();
            var newVersion = "v";
            string appName;
            string releaseDate;
            string link;
            var changeLog = new List<string>();
            var sr = new StreamReader(path);
            try
            {
                var line = sr.ReadLine();
                if (line.ToLowerInvariant().Contains("html"))
                {
                    if (showMessage)
                    {
                        MessageBox.Show("Unable to check for updates, try again later", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    sr.Dispose();
                    return;
                }
                appName = Tools.GetConfigString(line);
                newVersion += Tools.GetConfigString(sr.ReadLine());
                releaseDate = Tools.GetConfigString(sr.ReadLine());
                link = Tools.GetConfigString(sr.ReadLine());
                sr.ReadLine();//ignore Change Log header
                while (sr.Peek() >= 0)
                {
                    changeLog.Add(sr.ReadLine());
                }
            }
            catch (Exception ex)
            {
                if (showMessage)
                {
                    MessageBox.Show("Error parsing update file:\n" + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                sr.Dispose();
                return;
            }
            sr.Dispose();
            Tools.DeleteFile(path);
            if (thisVersion.Equals(newVersion))
            {
                if (showMessage)
                {
                    MessageBox.Show("You have the latest version", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return;
            }
            var newInt = Convert.ToInt16(newVersion.Replace("v", "").Replace(".", "").Trim());
            var thisInt = Convert.ToInt16(thisVersion.Replace("v", "").Replace(".", "").Trim());
            if (newInt <= thisInt)
            {
                if (showMessage)
                {
                    MessageBox.Show("You have a newer version (" + thisVersion + ") than what's on the server (" + newVersion + ")\nNo update needed!", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return;
            }
            var updaterForm = new Updater();
            updaterForm.SetInfo(Text, thisVersion, appName, newVersion, releaseDate, link, changeLog);
            updaterForm.ShowDialog();
        }
        
        private void alwaysOnTop_Click(object sender, EventArgs e)
        {
            TopMost = alwaysOnTop.Checked;
        }

        private void btnAudio_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            var exts = new List<string> {".mogg", ".wav", ".ogg"};
            if (exts.Contains(Path.GetExtension(files[0].ToLowerInvariant())) || VariousFunctions.ReadFileType(files[0]) == XboxFileType.STFS)
            {
                RunAudioAnalyzer(files[0]);
            }
            else
            {
                MainForm_DragDrop(sender, e);
            }
        }

        private void btnAudio_Click(object sender, EventArgs e)
        {
            if (MovedButton) return;
            RunAudioAnalyzer("");
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            if (MovedButton) return;
            contextMenuStrip2.Show(Cursor.Position.X, Cursor.Position.Y);
        }        

        private void resetLocations_Click(object sender, EventArgs e)
        {
            foreach (var button in FormButtons)
            {
                button.Button.Location = button.DefaultLocation;
            }
        }

        private void btnBatchRenamer_Click(object sender, EventArgs e)
        {
            if (MovedButton) return;
            var newRenamer = new BatchRenamer(btnBatchRenamer.BackColor, btnBatchRenamer.ForeColor);
            activeForm = newRenamer;
            activeForms.Add(activeForm);
            newRenamer.Show();
        }

        private void btnUpgradeBundler_Click(object sender, EventArgs e)
        {
            if (MovedButton) return;
            var bundler = new ProUpgradeBundler(btnUpgradeBundler.BackColor, btnUpgradeBundler.ForeColor);
            activeForm = bundler;
            activeForms.Add(activeForm);
            bundler.Show();
        }

        private void hideButton_Click(object sender, EventArgs e)
        {
            CurrentButton.Visible = false;
        }

        private void moveUp_Click(object sender, EventArgs e)
        {
            CurrentButton.Top -= VerticalJump;
        }

        private void moveDown_Click(object sender, EventArgs e)
        {
            CurrentButton.Top += VerticalJump;
        }

        private void moveLeft_Click(object sender, EventArgs e)
        {
            CurrentButton.Left -= HorizontalJump;
        }

        private void moveRight_Click(object sender, EventArgs e)
        {
            CurrentButton.Left += HorizontalJump;
        }
        
        private void Buttons_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            IsClickingButton = true;
            MovedButton = false;
            CurrentButton = (Button)sender;
            timer2.Enabled = true;
        }

        private void Buttons_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            timer2.Enabled = false;
            IsClickingButton = false;
            CurrentButton = (Button)sender;
            CurrentButton.Cursor = Cursors.Hand;
            CurrentButton = null;
            HelperLineLeft.Visible = false;
            HelperLineTop.Visible = false;
        }
        
        private void Buttons_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (CurrentButton == null) return;
            if (CurrentButton.Cursor != Cursors.NoMove2D) return;
            CurrentButton.Left = PointToClient(MousePosition).X - (CurrentButton.Width / 2);
            CurrentButton.Top = PointToClient(MousePosition).Y - (CurrentButton.Height / 2);
            MovedButton = true;
            HelperLineLeft.Left = CurrentButton.Left;
            HelperLineTop.Top = CurrentButton.Top;
            HelperLineLeft.Visible = ButtonColumns.Contains(CurrentButton.Left);
            HelperLineTop.Visible = ButtonRows.Contains(CurrentButton.Top);
            mouseX = MousePosition.X;
            mouseY = MousePosition.Y;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Enabled = false;
            if (!IsClickingButton)
            {
                MovedButton = false;
                return;
            }
            CurrentButton.Cursor = Cursors.NoMove2D;
            MovedButton = true;
            CurrentButton.BringToFront();
            HelperLineLeft.BringToFront();
            HelperLineTop.BringToFront();
            mouseX = MousePosition.X;
            mouseY = MousePosition.Y;
            IsClickingButton = false;
            ButtonColumns = new List<int>();
            ButtonRows = new List<int>();
            foreach (var button in FormButtons.Where(button => button.Button != CurrentButton))
            {
                if (!ButtonColumns.Contains(button.Button.Left))
                {
                    ButtonColumns.Add(button.Button.Left);
                }
                if (!ButtonRows.Contains(button.Button.Top))
                {
                    ButtonRows.Add(button.Button.Top);
                }
            }
        }

        public static bool IsAdministratorMode()
        {
            try
            {
                return (new WindowsPrincipal(WindowsIdentity.GetCurrent())).IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void aboutTool_Click(object sender, EventArgs e)
        {
            var version = GetAppVersion();
            var message = Tools.ReadHelpFile("about");
            MessageBox.Show(Text + " " + version + "\nDeveloped by Nemo\n© Nemo's Nautilus, 2013-2025\n\n" + message, "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void viewChangeLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var readme = Application.StartupPath + "\\nautilus_changelog.txt";
            if (!File.Exists(readme))
            {
                MessageBox.Show("Changelog is missing - don't delete any files that come with this program!", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            Process.Start(readme);
        }

        private void checkForUpdates_Click(object sender, EventArgs e)
        {
            showMessage = true;
            updater.RunWorkerAsync();
        } 

        private void newDefaultImage_Click(object sender, EventArgs e)
        {
            bg_image = "default";
            LoadBackground();
        }

        private void oldDefaultImage_Click(object sender, EventArgs e)
        {
            bg_image = "default_old";
            LoadBackground();
        }

        private void btnVolumeNormalizer_Click(object sender, EventArgs e)
        {
            var newVolumeNormalizerPrep = new VolumeNormalizer(btnVolumeNormalizer.BackColor, btnVolumeNormalizer.ForeColor);
            activeForm = newVolumeNormalizerPrep;
            activeForms.Add(activeForm);
            newVolumeNormalizerPrep.Show();
        }

        private void btnCONExplorer_Click(object sender, EventArgs e)
        {
            if (MovedButton) return;
            var ofd = new OpenFileDialog { Multiselect = true, Title = "Choose CON/LIVE file to open..." };
            if (ofd.ShowDialog() != DialogResult.OK) return;

            if (VariousFunctions.ReadFileType(ofd.FileName) == XboxFileType.STFS)
            {
                var xExplorer = new CONExplorer(btnCONCreator.BackColor, btnCONCreator.ForeColor);
                xExplorer.LoadCON(ofd.FileName);
                activeForm = xExplorer;
                activeForms.Add(activeForm);
                xExplorer.Show();
            }
            else
            {
                MessageBox.Show("That's not a valid file...", "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnBatchProcessor_Click(object sender, EventArgs e)
        {
            if (MovedButton) return;
            var newBatchProcessor = new BatchProcessor(btnBatchRenamer.BackColor, btnBatchRenamer.ForeColor);
            activeForm = newBatchProcessor;
            activeForms.Add(activeForm);
            newBatchProcessor.Show();
        }

        private void btnRBAEditor_Click(object sender, EventArgs e)
        {
            if (MovedButton) return;
            RunRBAEditor("");
        }

        private void btnRBAEditor_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            RunRBAEditor(files[0]);
        } 

        private void RunRBAEditor(string file)
        {
            var newRBAEditor = new RBAEditor(btnCONConverter.BackColor, btnCONConverter.ForeColor, file);
            activeForm = newRBAEditor;
            activeForms.Add(activeForm);
            newRBAEditor.Show();
        }

        private void btnScores_Click(object sender, EventArgs e)
        {
            var file = Application.StartupPath + "\\bin\\RB3SaveScoresViewer.exe";
            if (!File.Exists(file))
            {
                MessageBox.Show(Path.GetFileName(file) + " is missing, can't launch...", "Missing File", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            MessageBox.Show("RB3 Save Scores Viewer was created and is still maintained by StackOverflow0x\nHe was kind enough to volunteer this great tool to join Nautilus\nMake sure to thank him!", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            Process.Start(file);
        }

        private void btnMiloMod_Click(object sender, EventArgs e)
        {
            var file = Application.StartupPath + "\\bin\\MiloMod.exe";
            if (!File.Exists(file))
            {
                MessageBox.Show(Path.GetFileName(file) + " is missing, can't launch...", "Missing File", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            MessageBox.Show("MiloMod was created and is still maintained by StackOverflow0x\nHe was kind enough to volunteer this great tool to join Nautilus\nMake sure to thank him!", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            Process.Start(file);
        }

        private void gravity_Tick(object sender, EventArgs e)
        {
            if (flappy.Top + gravity_speed >= floor)
            {
                Lost();
                return;
            }
            flappy.Top = flappy.Top + gravity_speed;            
        }

        private void Lost(bool silent = false)
        {
            if (!isDead && !silent) //if lost by hitting ground, otherwise already done when hit the column
            {
                flappy.BackColor = deadColor;
                PlaySound("dead");
            }
            Cursor = Cursors.Default;
            isDead = true;
            gravity.Enabled = false;
            move.Enabled = false;
            isPlaying = false;
            flappy.Top = floor;
            if (!silent)
            {
                MessageBox.Show("You scored " + CurrentScore + " point(s)\nThanks for playing!", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            isPlayingFlappy = false;
            CurrentScore = 0;
            GoldStarA.Visible = false;
            GoldStarB.Visible = false;
            flappy.Left = FlappyLeft;
            flappy.Top = FlappyTop;
            ResetColumns();
            flappy.BackColor = Color.Transparent;
        }

        private void ResetFlappy()
        {
            flappy.Top = center;
            flappy.Left = (Width - flappy.Width) / 2;
            ResetColumns();
            isDead = false;
            isPlayingFlappy = true;
            flappy.Image = flappy_b;
            Text = GameName;
        }

        private void jump_Tick(object sender, EventArgs e)
        {
            if (flappy.Top - flying_speed <= 0 || flappy.Top - flying_speed <= jump_goal)
            {
                jump.Enabled = false;
                gravity.Enabled = true;
            }
            flappy.Top = flappy.Top - flying_speed;
        }

        private void move_Tick(object sender, EventArgs e)
        {
            MoveColumn(columns_a, columnA1.Left - move_speed);
            MoveColumn(columns_b, columnB1.Left - move_speed);
            GoldStarA.Left = GoldStarA.Left - move_speed;
            GoldStarB.Left = GoldStarB.Left - move_speed;

            if (GoldStarA.Left < 0 - GoldStarA.Width)
            {
                GoldStarA.Left = columnA1.Left + (column_distance - GoldStarA.Width) / 2 + 15;
                MoveStar(GoldStarA);
            }
            else if (GoldStarB.Left < 0 - GoldStarB.Width)
            {
                GoldStarB.Left = columnB1.Left + (column_distance - GoldStarB.Width) / 2 + 15;
                MoveStar(GoldStarB);
            }
            if (columnA1.Left < 0 - columnA1.Width)
            {
                MoveColumn(columns_a, columnB1.Left + column_distance);
                HideBoxes(columns_a);
            }
            else if (columnB1.Left < 0 - columnB1.Width)
            {
                MoveColumn(columns_b, columnA1.Left + column_distance);
                HideBoxes(columns_b);
            }
        }
        private void MoveStar(Control star)
        {
            var bottom = Height - (GoldStarA.Height * 2);
            var rnd = new Random();
            star.Top = rnd.Next(GoldStarA.Height, bottom);
            star.Visible = true;
        }

        private void HideBoxes(IList<PictureBox> columns)
        {
            foreach (var column in columns)
            {
                column.Visible = true; // restore all of them
            }

            var hide = randomizer.Next(1, 5); //can't have the top-most or bottom-most be values
            columns[hide].Visible = false;
            if (hide + 1 >= 5)
            {
                columns[hide - 1].Visible = false;
            }
            else
            {
                columns[hide + 1].Visible = false;
            }
        }

        private static void MoveColumn(IEnumerable<PictureBox> columns, int pos)
        {
            foreach (var column in columns)
            {
                column.Left = pos;
            }
        }

        private void showScore_Tick(object sender, EventArgs e)
        {
            lblScore.Visible = false;
            showScore.Enabled = false;
        }

        private void checkCollision_Tick(object sender, EventArgs e)
        {
            List<PictureBox> columns;

            var leftBorder = flappy.Left - GoldStarA.Width + forgiveness;
            var rightBorder = flappy.Left + flappy.Width - forgiveness;
            var topBorder = flappy.Top - GoldStarA.Height + forgiveness;
            var bottomBorder = flappy.Top + flappy.Height - forgiveness;

            var isGoldStar = false;
            var star = GoldStarA;
            if (GoldStarA.Left > leftBorder && GoldStarA.Left < rightBorder && GoldStarA.Top > topBorder &&
                GoldStarA.Top < bottomBorder && GoldStarA.Visible)
            {
                isGoldStar = true;
            }
            else if (GoldStarB.Left > leftBorder && GoldStarB.Left < rightBorder && GoldStarB.Top > topBorder &&
                GoldStarB.Top < bottomBorder && GoldStarB.Visible)
            {
                isGoldStar = true;
                star = GoldStarB;
            }
            if (isGoldStar)
            {
                DoScore();
                star.Visible = false;
            }

            leftBorder = flappy.Left - columnA1.Width + forgiveness;
            topBorder = flappy.Top - columnA1.Height + forgiveness;

            if (columnA1.Left > leftBorder && columnA1.Left < rightBorder)
            {
                isPassingColumn = true;
                columns = columns_a;
            }
            else if (columnB1.Left > leftBorder && columnB1.Left < rightBorder)
            {
                isPassingColumn = true;
                columns = columns_b;
            }
            else
            {
                if (!isPassingColumn) return;
                isPassingColumn = false;
                DoScore();
                return;
            }

            if (!columns.Any(column => column.Top > topBorder && column.Top < bottomBorder && column.Visible)) return;
            PlaySound("dead");
            isPassingColumn = false;
            move.Enabled = false;
            isDead = true;
            flappy.BackColor = deadColor;
            checkCollision.Enabled = false;
            GoldStarA.Visible = false;
            GoldStarB.Visible = false;
        }

        private void pointTmr_Tick(object sender, EventArgs e)
        {
            GotPoint = false;
            pointTmr.Enabled = false;
        }

        private void DoScore()
        {
            CurrentScore++;
            if (CurrentScore <= 0) return;
            PlaySound("point");
            lblScore.Text = CurrentScore.ToString(CultureInfo.InvariantCulture);
            lblScore.Visible = true;
            showScore.Enabled = true;
        }

        private void ResetColumns()
        {
            MoveColumn(columns_a, Width);
            MoveColumn(columns_b, Width + column_distance);

            HideBoxes(columns_a);
            HideBoxes(columns_b);
        }

        private void picBackground_MouseUp(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Default;
        }

        private void picBackground_MouseMove(object sender, MouseEventArgs e)
        {
            if (Cursor != Cursors.NoMove2D) return;
            if (MousePosition.X != mouseX)
            {
                if (MousePosition.X > flappyMouseX)
                {
                    Left = Left + (MousePosition.X - flappyMouseX);
                }
                else if (MousePosition.X < flappyMouseX)
                {
                    Left = Left - (flappyMouseX - MousePosition.X);
                }
                flappyMouseX = MousePosition.X;
            }

            if (MousePosition.Y == flappyMouseY) return;
            if (MousePosition.Y > flappyMouseY)
            {
                Top = Top + (MousePosition.Y - flappyMouseY);
            }
            else if (MousePosition.Y < flappyMouseY)
            {
                Top = Top - (flappyMouseY - MousePosition.Y);
            }
            flappyMouseY = MousePosition.Y;
        }

        private void flappy_Click(object sender, EventArgs e)
        {
            resetEverythingToolStripMenuItem_Click(sender, e);
            FormBorderStyle = FormBorderStyle.Sizable;//FormBorderStyle.FixedSingle;
            LoadFlappyGraphics();
            allButtonsTransparencyToolStripMenuItem_Click(sender, e);
            ResetColumns();
            ResetFlappy();
            flappyTmr.Enabled = false;
        }

        private void picBackground_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (isPlayingFlappy)
            {
                if (e.Button == MouseButtons.Left && isDead)
                {
                    Cursor = Cursors.NoMove2D;
                    mouseX = MousePosition.X;
                    mouseY = MousePosition.Y;
                }
                if (isDead) return;
                doPlay();
                return;
            }
        }

        private void btnCharEditor_Click(object sender, EventArgs e)
        {
            var file = Application.StartupPath + "\\bin\\CharEditor.exe";
            if (!File.Exists(file))
            {
                MessageBox.Show(Path.GetFileName(file) + " is missing, can't launch...", "Missing File", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            MessageBox.Show("RB3 Character Editor was created by Lord Zedd\\TehBanStick\nHe was kind enough to agree to let his tool become part of Nautilus\nMake sure to thank him!", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            Process.Start(file);
        }

        private void btnMoggMaker_Click(object sender, EventArgs e)
        {
            if (MovedButton) return;
            var maker = new MoggMaker(btnMoggMaker.BackColor, btnMoggMaker.ForeColor);
            activeForm = maker;
            activeForms.Add(maker);
            maker.Show();
        }

        private void gifTmr_Tick(object sender, EventArgs e)
        {
            gifTmr.Enabled = false;
            //MessageBox.Show("Thank you, for everything.\n\n- Nemo", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            ResetEverything(true);
        }

        private void cLIHelp_Click(object sender, EventArgs e)
        {
            var help = Application.StartupPath + "\\bin\\help\\help";
            var message = File.Exists(help) ? File.ReadAllText(help) : "Help file missing!";
            var form = new HelpForm("Help", message, false, true);
            form.ShowDialog();
        }

        private void btnStudio_Click(object sender, EventArgs e)
        {
            if (MovedButton) return;
            var studio = new MiniStudio();
            activeForm = studio;
            activeForms.Add(studio);
            studio.Show();
        }

        private bool slideRight = true;
        private void flappyTmr_Tick(object sender, EventArgs e)
        {
            if (slideRight)
            {
                flappy.Left += 2;
            }
            else
            {
                flappy.Left -= 2;
            }
            var left = btnVolumeNormalizer.Left;
            if (!btnVolumeNormalizer.Visible)
            {
                left = btnSaveFileImageEditor.Left;
                if (!btnSaveFileImageEditor.Visible)
                {
                    left = btnRBAEditor.Left;
                    if (!btnRBAEditor.Visible)
                    {
                        left = btnBatchCryptor.Left;
                        if (!btnBatchCryptor.Visible)
                        {
                            left = Width - (flappy.Width/2);
                        }
                    }                    
                }
            }            
            if (flappy.Left + flappy.Width >= left - 2)
            {
                slideRight = false;
            }
            if (flappy.Left <= picSettings.Left + picSettings.Width)
            {
                slideRight = true;
            }
        }

        private void showHideMario_Click(object sender, EventArgs e)
        {
            flappy.Visible = !flappy.Visible;
            flappyTmr.Enabled = flappy.Visible;
        }

        private void resetEverythingNew_Click(object sender, EventArgs e)
        {
            ResetEverything(true);
        }

        private void resetVisibility_Click(object sender, EventArgs e)
        {
            foreach (var button in FormButtons)
            {
                if (button.Button == btnSettings) continue; //keep it hidden
                button.Button.Visible = true;
            }
        }

        private void picSettings_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            btnSettings_Click(sender, e);
        }

        private void btnAudioConverter_Click(object sender, EventArgs e)
        {
            if (MovedButton) return;
            var converter = new AudioConverter();
            activeForm = converter;
            activeForms.Add(converter);
            converter.Show();
        }

        private void btnCDG_Click(object sender, EventArgs e)
        {
            if (MovedButton) return;
            var cdg = new CDGConverter();
            activeForm = cdg;
            activeForms.Add(activeForm);
            cdg.Show();
        }

        private void btnUltraStarConverter_Click(object sender, EventArgs e)
        {
            if (MovedButton) return;
            var ultraStar = new UltraStarConverter();
            activeForm = ultraStar;
            activeForms.Add(activeForm);
            ultraStar.Show();
        }
    }

    public class MyButton
    {
        public Button Button { get; set; }
        public Point DefaultLocation { get; set; }
    }
}