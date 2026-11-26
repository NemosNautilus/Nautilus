using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Nautilus.Properties;
using Nautilus.x360;
using Microsoft.VisualBasic;
using Application = System.Windows.Forms.Application;
using Point = System.Drawing.Point;

namespace Nautilus
{
    public partial class RBtoUSB : Form
    {
        public string DriveLetter;
        private const string Profile = "0000000000000000\\"; //no profile
        private const string RB1_PATH = "45410829\\";
        private const string RB2_PATH = "45410869\\";
        private const string RB3_PATH = "45410914\\";
        private const string CON_PATH = "00000001\\";
        private const string LIVE_PATH = "00000002\\";
        private const string TU_PATH = "000B0000\\";
        private const string RootFolder = "Content\\";
        private const string NameFile = "name.txt";
        private const long MB = 1048576;
        private const long GB = 1073741824;
        private const long TB = 1099511627776;
        private static Color mMenuBackground;
        private readonly NemoTools Tools;
        private string GamePath;
        private string[] FilesToAdd;
        private readonly List<string> FilesToDelete;
        private readonly List<string> FilesToExtract;
        private readonly string BIN;
        private string ExtractFolder;
        private bool isDragDrop;
        public SortOrder ListSorting;
        private int ActiveSortColumn;
        private string SearchTerm;
        private readonly string config;
        private List<string> MagmaFiles;
        private readonly List<ToolStripMenuItem> Toggles;
        private readonly int[] ColumnWidths;
        private readonly int[] StaticColumnWidths;
        private readonly List<string> AddedFiles;
        private readonly List<int> ToDelete;
        private bool OverWriteFiles;
        private bool UserCancelled;

        public RBtoUSB()
        {
            InitializeComponent();
            Tools = new NemoTools();
            mMenuBackground = menuStrip1.BackColor;
            menuStrip1.Renderer = new DarkRenderer();
            contextMenuStrip1.Renderer = new DarkRenderer();
            BIN = Application.StartupPath + "\\bin\\";
            copyTU4ToDrive.Visible = File.Exists(BIN + "tu4");
            FilesToDelete = new List<string>();
            FilesToExtract = new List<string>();
            MagmaFiles = new List<string>();
            AddedFiles = new List<string>();
            ToDelete = new List<int>();
            config = BIN + "config\\usb.config";
            Toggles = new List<ToolStripMenuItem> { togglePackage, toggleFileType, toggleFileSize, toggleModifiedDate, toggleFileName, toggleSongArtist, toggleSongTitle, toggleSongID, toggleInternalName };
            ColumnWidths = new int[lstFiles.Columns.Count];
            StaticColumnWidths = new int[lstFiles.Columns.Count];
            for (var i = 0; i < lstFiles.Columns.Count; i++)
            {
                StaticColumnWidths[i] = lstFiles.Columns[i].Width;
            }
            SetDoubleBuffered(lstFiles, true);
            LoadConfig();
        }

        public static void SetDoubleBuffered(Control control, bool enable)
        {
            var doubleBufferPropertyInfo = control.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            doubleBufferPropertyInfo.SetValue(control, enable, null);
        }

        private void LoadConfig()
        {
            if (!File.Exists(config))
            {
                CenterToScreen();
                return;
            }
            var x = -1;
            var y = -1;
            var sr = new StreamReader(config);
            try
            {
                DriveLetter = Tools.GetConfigString(sr.ReadLine());
                checkForMisplacedFiles.Checked = sr.ReadLine().Contains("True");
                grabSongMetadata.Checked = sr.ReadLine().Contains("True");
                autoopenLastUsedDrive.Checked = sr.ReadLine().Contains("True");
                showGridlines.Checked = sr.ReadLine().Contains("True");
                Width = Convert.ToInt16(Tools.GetConfigString(sr.ReadLine()));
                Height = Convert.ToInt16(Tools.GetConfigString(sr.ReadLine()));
                x = Convert.ToInt16(Tools.GetConfigString(sr.ReadLine()));
                y = Convert.ToInt16(Tools.GetConfigString(sr.ReadLine()));
                if (sr.ReadLine().Contains("True"))
                {
                    WindowState = FormWindowState.Maximized;
                }
                var count = Convert.ToInt16(Tools.GetConfigString(sr.ReadLine()));
                for (var i = 0; i < count; i++)
                {
                    lstFiles.Columns[i].Width = Convert.ToInt16(Tools.GetConfigString(sr.ReadLine()));
                }
                for (var i = 0; i < count; i++)
                {
                    lstFiles.Columns[i].DisplayIndex = Convert.ToInt16(Tools.GetConfigString(sr.ReadLine()));
                }
                for (var i = 0; i < count; i++)
                {
                    Toggles[i].Checked = sr.ReadLine().Contains("True");
                }
            }
            catch (Exception)
            {}
            if (WindowState != FormWindowState.Maximized && x > -1 && y > -1)
            {
                Location = new Point(x,y);
            }
            else
            {
                CenterToScreen();
            }
            sr.Dispose();
            lstFiles.GridLines = showGridlines.Checked;
        }

        private void SaveConfig()
        {
            var sw = new StreamWriter(config, false);
            sw.WriteLine("LastUsedDrive=" + DriveLetter);
            sw.WriteLine("CheckMisplacedFiles=" + checkForMisplacedFiles.Checked);
            sw.WriteLine("GrabMetadta=" + grabSongMetadata.Checked);
            sw.WriteLine("AutoOpen=" + autoopenLastUsedDrive.Checked);
            sw.WriteLine("ShowGridlines=" + showGridlines.Checked);
            sw.WriteLine("FormWidth=" + Width);
            sw.WriteLine("FormHeight=" + Height);
            sw.WriteLine("PosX=" + Left);
            sw.WriteLine("PosY=" + Top);
            sw.WriteLine("IsMaximized=" + (WindowState == FormWindowState.Maximized));
            sw.WriteLine("InfoColumns=" + lstFiles.Columns.Count);
            for (var i = 0; i < lstFiles.Columns.Count; i++)
            {
                sw.WriteLine("Column" + (i + 1) + "Width=" + lstFiles.Columns[i].Width);
            }
            for (var i = 0; i < lstFiles.Columns.Count; i++)
            {
                sw.WriteLine("Column" + (i + 1) + "Index=" + lstFiles.Columns[i].DisplayIndex);
            }
            for (var i = 0; i < lstFiles.Columns.Count; i++)
            {
                sw.WriteLine("Toggle" + (i + 1) + "=" + Toggles[i].Checked);
            }
            sw.Dispose();
        }

        private void openUSB_Click(object sender, EventArgs e)
        {
            var letter = DriveLetter;
            var selector = new DriveSelector(this) {Left = Cursor.Position.X, Top = Cursor.Position.Y};
            selector.ShowDialog();
            if (string.IsNullOrWhiteSpace(DriveLetter) || DriveLetter == letter) return;
            OpenDrive();
        }

        private void CloseDrive()
        {
            if (string.IsNullOrWhiteSpace(DriveLetter)) return;
            lstFiles.Items.Clear();
            lblDriveLetter.Text = "";
            lblDriveName.Text = "";
            lblDriveSize.Text = "";
            lblDriveUsed.Text = "";
            lblDriveFiles.Text = "";
            GamePath = "";
            SearchTerm = "";
            FilesToAdd = new string[] {};
            FilesToDelete.Clear();
            FilesToExtract.Clear();
            ExtractFolder = "";
            Log("");
        }

        private bool UpdateDriveName(string name)
        {
            try
            {
                var sw = new StreamWriter(DriveLetter + NameFile, false, Encoding.Unicode);
                sw.Write(name);
                sw.Dispose();
                lblDriveName.Text = name;
                var drive = GetDrive(DriveLetter);
                if (string.IsNullOrWhiteSpace(drive.VolumeLabel))
                {
                    drive.VolumeLabel = "RB3_CUSTOMS";
                }
                Log("Changed drive name to '" + name + "'");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating drive name file\n" + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private static DriveInfo GetDrive(string drive_letter)
        {
            return DriveInfo.GetDrives().FirstOrDefault(drive => drive.Name == drive_letter);
        }

        private void OpenDrive()
        {
            Log("Loading drive " + DriveLetter);
            CloseDrive();
            if (GetDrive(DriveLetter).DriveType == DriveType.CDRom)
            {
                MessageBox.Show("You selected a disc drive, please select your Xbox USB drive", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (Directory.Exists(DriveLetter + "Xbox360") || Directory.Exists(DriveLetter + "Xbox 360"))
            {
                MessageBox.Show("The drive you selected appears to be formatted using the old Xbox 360 specific format - I can't work with that!\n\n" + 
                                "If the drive shows as being full even though you think it has free space, that's the old formatting system at work.\n\n" + 
                                "Please read the Help document and follow instructions - format your drive using FAT32 in Windows or " +
                                "using your updated Xbox 360.", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            var success = true;
            if (!File.Exists(DriveLetter + NameFile))
            {
                success = UpdateDriveName("My RB3 Customs Drive");
            }
            else
            {
                try
                {
                    var sr = new StreamReader(DriveLetter + NameFile);
                    lblDriveName.Text = sr.ReadLine();
                    sr.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error reading drive name\n" + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    success = false;
                }
            }
            if (!success) return;
            var drive = GetDrive(DriveLetter);
            GamePath = DriveLetter + RootFolder + Profile + RB3_PATH;
            lblDriveLetter.Text = DriveLetter;
            lblDriveSize.Text = GetFormattedSize(drive.TotalSize);
            lblDriveUsed.Text = GetFormattedSize(drive.AvailableFreeSpace);
            if (!CheckCreateDirectories())
            {
                CloseDrive();
                return;
            }
            EnableDisable(false);
            lstFiles.ListViewItemSorter = null; //necessary to prevent auto-sorting as items are added
            lstFiles.Sorting = SortOrder.None;
            driveLoader.RunWorkerAsync();
        }

        private void EnableDisable(bool enabled)
        {
            menuStrip1.Invoke(new MethodInvoker(() => menuStrip1.Enabled = enabled));
            lstFiles.Invoke(new MethodInvoker(() => lstFiles.Enabled = enabled));
            panelInfo.Invoke(new MethodInvoker(() => panelInfo.Enabled = enabled));
            btnCancel.Invoke(new MethodInvoker(() => btnCancel.Visible = false));
            picWorking.Invoke(new MethodInvoker(() => picWorking.Visible = !enabled));
            progressBar.Invoke(new MethodInvoker(() => progressBar.Visible = false));
            progressBar.Invoke(new MethodInvoker(() => progressBar.Value = 0));
            UserCancelled = false;
        }

        private bool CheckCreateDirectories()
        {
            try
            {
                if (!Directory.Exists(DriveLetter + RootFolder))
                {
                    var di = Directory.CreateDirectory(DriveLetter + RootFolder);
                    di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
                }
                var directories = new List<string> { GamePath.Replace(RB3_PATH, RB1_PATH) + CON_PATH, GamePath.Replace(RB3_PATH, RB2_PATH) + CON_PATH, 
                    GamePath + CON_PATH, GamePath.Replace(RB3_PATH, RB1_PATH) + LIVE_PATH, GamePath.Replace(RB3_PATH, RB2_PATH) + LIVE_PATH, GamePath + LIVE_PATH, 
                    GamePath + TU_PATH };
                foreach (var directory in directories.Where(directory => !Directory.Exists(directory)))
                {
                    Directory.CreateDirectory(directory);
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating required folders\n" + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private static string GetFormattedSize(long bytes)
        {
            if (bytes > TB)
            {
                return Math.Round((double)bytes/TB, 2) + " TB";
            }
            if (bytes > GB)
            {
                return Math.Round((double)bytes / GB, 2) + " GB";
            }
            if (bytes > MB)
            {
                return Math.Round((double)bytes / MB, 2) + " MB";
            }
            return Math.Round((double)bytes / 1024, 2) + " KB";
        }

        private enum FileType
        {
            CON, LIVE, TU
        }

        private static string GetFileTypeString(FileType type)
        {
            string isType;
            switch (type)
            {
                case FileType.CON:
                    isType = "CON";
                    break;
                case FileType.LIVE:
                    isType = "LIVE";
                    break;
                default:
                    isType = "Title Update";
                    break;
            }
            return isType;
        }

        private string DoMisplacedFile(string file, FileType isType, FileType shouldBe, string path)
        {
            var msg = "File '" + Path.GetFileName(file) + "' is a " + GetFileTypeString(shouldBe) + " file but it's placed in the directory for " + 
                GetFileTypeString(isType) + " files\nDo you want me to move it to the correct folder?";
            if (MessageBox.Show(msg, "Misplaced File", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return file;
            Log("Relocating misplaced file: '" + Path.GetFileName(file) + "'");
            return CopyFile(file, path + Path.GetFileName(file), false, true) ? path + Path.GetFileName(file) : file;
        }

        private bool CopyFile(string from, string to, bool extract = false, bool move = false, bool isBatch = false, bool isDuplicate = false)
        {
            //Log(counter + "Adding file '" + Path.GetFileName(from) + "'");
            if (!File.Exists(from)) 
            {
                if (!isBatch)
                {
                    MessageBox.Show("Something happened and I can't find file '" + Path.GetFileName(from) + "' to copy it", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                Log("");
                return false;
            }
            try
            {
                var drive = GetDrive(Path.GetPathRoot(to));
                var fileInfo = new FileInfo(from);
                if (fileInfo.Length > drive.AvailableFreeSpace)
                {
                    MessageBox.Show("There is not enough free space to copy file '" + Path.GetFileName(from) + "'\nThe file is " +
                        GetFormattedSize(fileInfo.Length) + " and the destination only has " + GetFormattedSize(drive.AvailableFreeSpace) +
                        " free\nIf there are other files to transfer, I will try to copy those next","Insufficient Space", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Log("");
                    return false;
                }
            }
            catch (Exception)
            {}
            if (File.Exists(to))
            {
                if (MessageBox.Show("File '" + Path.GetFileName(to) + "' already exists.\nOverwrite?", "File Exists", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    Log("");
                    return false;
                }
                isDuplicate = true;
                Tools.DeleteFile(to);
            }
            try
            {
                if (move)
                {
                    if (!Tools.MoveFile(from, to))
                    {
                        Log("");
                        return false;
                    }
                }
                else
                {
                    if (!CopyFileEx(from, to))
                    {
                        Log("");
                        return false;
                    }
                }
                if (isDuplicate)
                {
                    Log("");
                    return true;
                }
                if (extract) return true;
                AnalyzeFiles(new List<string> {to});
                AddedFiles.Add(to);
                return true;
            }
            catch (Exception ex)
            {
                if (!isBatch)
                {
                    MessageBox.Show("Error copying file '" + Path.GetFileName(from) + "'\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                Log("");
                return false;
            }
        }

        private static bool IsMiscFile(string package)
        {
            var misc_files = new List<string> { "Rock Band Song Cache", "Rock Band 2 Song Cache", "Rock Band 3 Song Cache", 
                "Rock Band Audio/Video Settings", "Rock Band 2 Audio/Video Settings", "Rock Band 3 Audio/Video Settings", 
                "Rock Band Save Data", "Rock Band 2", "Rock Band 3" };
            return misc_files.Contains(package);
        }

        private void AnalyzeFiles(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                if (UserCancelled) return;
                try
                {
                    FileType type;
                    if (file.Contains(CON_PATH))
                    {
                        type = FileType.CON;
                    }
                    else if (file.Contains(LIVE_PATH))
                    {
                        type = FileType.LIVE;
                    }
                    else
                    {
                        type = FileType.TU;
                    }
                    var final_file = file;
                    string package;
                    string fileType;
                    var artist = "";
                    var song = "";
                    var songid = "";
                    var internalname = "";
                    var xFile = new STFSPackage(file);
                    if (xFile.ParseSuccess)
                    {
                        package = xFile.Header.Title_Display;
                        var xFileType = xFile.Header.ThisType;
                        xFile.CloseIO();
                        var getDTA = false;
                        switch (xFileType)
                        {
                            case PackageType.SavedGame:
                                if (IsMiscFile(package) && !filterMisc.Checked && !filterAll.Checked) continue;
                                if (!filterCON.Checked && !filterAll.Checked && !IsMiscFile(package)) continue;
                                getDTA = true;
                                fileType = GetFileTypeString(FileType.CON);
                                if (type != FileType.CON && checkForMisplacedFiles.Checked)
                                {
                                    final_file = DoMisplacedFile(file, type, FileType.CON, GamePath + CON_PATH);
                                }
                                break;
                            case PackageType.MarketPlace:
                                if (!filterLIVE.Checked && !filterAll.Checked) continue;
                                getDTA = true;
                                fileType = GetFileTypeString(FileType.LIVE);
                                if (type != FileType.LIVE && checkForMisplacedFiles.Checked)
                                {
                                    final_file = DoMisplacedFile(file, type, FileType.LIVE, GamePath + LIVE_PATH);
                                }
                                break;
                            case PackageType.Installer:
                                if (!filterMisc.Checked && !filterAll.Checked) continue;
                                artist = "N/A";
                                song = "N/A";
                                songid = "N/A";
                                internalname = "N/A";
                                fileType = GetFileTypeString(FileType.TU);
                                if (type != FileType.TU && checkForMisplacedFiles.Checked)
                                {
                                    final_file = DoMisplacedFile(file, type, FileType.TU, GamePath + TU_PATH);
                                }
                                break;
                            default:
                                if (!filterMisc.Checked && !filterAll.Checked) continue;
                                artist = "N/A";
                                song = "N/A";
                                songid = "N/A";
                                internalname = "N/A";
                                fileType = "Unknown";
                                break;
                        }
                        if (IsMiscFile(package))
                        {
                            artist = "N/A";
                            song = "N/A";
                            songid = "N/A";
                            internalname = "N/A";
                            getDTA = false;
                        }
                        if (getDTA && grabSongMetadata.Checked)
                        {
                            try
                            {
                                var Parser = new DTAParser();
                                if (Parser.ExtractDTA(file) && Parser.ReadDTA(Parser.DTA))
                                {
                                    if (Parser.Songs == null || !Parser.Songs.Any())
                                    {
                                        artist = "";
                                        song = "";
                                        songid = "";
                                        internalname = "";
                                    }
                                    else if (Parser.Songs.Count > 1)
                                    {
                                        artist = "N/A - Pack";
                                        song = "N/A - Pack";
                                        songid = "N/A - Pack";
                                        internalname = "N/A - Pack";
                                    }
                                    else
                                    {
                                        artist = Parser.Songs[0].Artist;
                                        song = Parser.Songs[0].Name;
                                        songid = Parser.Songs[0].SongIdString;
                                        internalname = Parser.Songs[0].InternalName;
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                artist = "";
                                song = "";
                                songid = "";
                                internalname = "";
                            }
                        }
                    }
                    else
                    {
                        package = "Unable to read package";
                        fileType = "Unknown";
                    }
                    var entry = new ListViewItem(package);
                    entry.SubItems.Add(fileType);
                    var fileInfo = new FileInfo(final_file);
                    entry.SubItems.Add(GetFormattedSize(fileInfo.Length));
                    entry.SubItems.Add(fileInfo.LastWriteTime.ToShortDateString() + " " + fileInfo.LastWriteTime.ToShortTimeString());
                    entry.SubItems.Add(Path.GetFileName(final_file));
                    entry.SubItems.Add(artist);
                    entry.SubItems.Add(song);
                    entry.SubItems.Add(songid);
                    entry.SubItems.Add(internalname);
                    entry.Tag = final_file;
                    lstFiles.Invoke(new MethodInvoker(() => lstFiles.Items.Add(entry)));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error reading file '" + Path.GetFileName(file) + "'!\n" + ex.Message, "Error", MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
        }
        
        private void lblDriveName_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            DoRenameDrive();
        }

        private void DoRenameDrive()
        {
            if (string.IsNullOrWhiteSpace(lblDriveLetter.Text))
            {
                MessageBox.Show("Open a drive first", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            var popup = new PasswordUnlocker(lblDriveName.Text);
            popup.Renamer(); //change settings for renaming
            popup.ShowDialog();
            var newname = popup.EnteredText;
            popup.Dispose();
            if (string.IsNullOrWhiteSpace(newname)) return;
            UpdateDriveName(newname);
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (lstFiles.Items.Count == 0 && !MagmaFiles.Any())
            {
                e.Cancel = true;
                return;
            }
            var visible = lstFiles.SelectedItems.Count > 0;
            extractFileToolStripMenuItem.Visible = visible;
            deleteFileToolStripMenuItem.Visible = visible;
            visible = lstFiles.SelectedItems.Count == 1;
            findFileInFolderToolStripMenuItem.Visible = visible;
            lineSeparator.Visible = visible;
            visible = lstFiles.SelectedItems.Count > 0 && !IsMiscFile(lstFiles.SelectedItems[0].SubItems[0].Text);
            SendToAudioAnalyzer.Visible = visible && lstFiles.SelectedItems[0].SubItems[7].Text != "N/A - Pack";
            SendToVisualizer.Visible = visible && lstFiles.SelectedItems[0].SubItems[7].Text != "N/A - Pack";
            SendToMIDICleaner.Visible = visible && lstFiles.SelectedItems[0].SubItems[7].Text != "N/A - Pack";
            SendToSongAnalyzer.Visible = visible && lstFiles.SelectedItems[0].SubItems[7].Text != "N/A - Pack";
            extractFileToolStripMenuItem.Text = lstFiles.SelectedItems.Count > 1 ? "Extract selected files" : "Extract selected file";
            deleteFileToolStripMenuItem.Text = lstFiles.SelectedItems.Count > 1 ? "Delete selected files" : "Delete selected file";
            sendToToolStripMenuItem.Visible = lstFiles.SelectedItems.Count == 1 && lstFiles.SelectedItems[0].SubItems[1].Text != "Title Update";
            SendToSetlistManager.Visible = lstFiles.SelectedItems.Count > 0 && lstFiles.SelectedItems[0].SubItems[0].Text == "Rock Band 3 Song Cache";
            SendToQuickPackEditor.Visible = lstFiles.SelectedItems.Count == 1 && lstFiles.SelectedItems[0].SubItems[7].Text == "N/A - Pack";
            addMagmaFile.Visible = MagmaFiles.Any();
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

        private void findFileInFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstFiles.SelectedItems.Count == 0) return;
            Process.Start("explorer.exe", "/select," + lstFiles.SelectedItems[0].Tag);
        }

        private void SendToCONExplorer_Click(object sender, EventArgs e)
        {
            Log("Sent file to CON Explorer");
            var handler = new CONExplorer(Color.FromArgb(34, 169, 31), Color.White);
            handler.LoadCON(lstFiles.SelectedItems[0].Tag.ToString());
            handler.Show();
            Log("");
        }

        private void SendToVisualizer_Click(object sender, EventArgs e)
        {
            Log("Sent file to Visualizer");
            var handler = new Visualizer(Color.FromArgb(230, 215, 0), Color.White, lstFiles.SelectedItems[0].Tag.ToString());
            handler.Show();
            Log("");
        }

        private void SendToSongAnalyzer_Click(object sender, EventArgs e)
        {
            Log("Sent file to Song Analyzer");
            var handler = new SongAnalyzer(lstFiles.SelectedItems[0].Tag.ToString());
            handler.Show();
            Log("");
        }

        private void SendToMIDICleaner_Click(object sender, EventArgs e)
        {
            Log("Sent file to MIDI Cleaner");
            var file = lstFiles.SelectedItems[0].Tag.ToString();
            var modified = File.GetLastWriteTimeUtc(file);
            var handler = new MIDICleaner(lstFiles.SelectedItems[0].Tag.ToString(), Color.FromArgb(230, 215, 0), Color.White);
            handler.Show();
            Log("");
            if (modified != File.GetLastWriteTimeUtc(file)) //file was modified
            {
                OpenDrive();
            }
        }
        
        private void renameDrive_Click(object sender, EventArgs e)
        {
            DoRenameDrive();
        }

        private void addFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FilesToAdd = new string[]{};
            var ofd = new OpenFileDialog
            {
                Title = "Select Rock Band file(s)",
                InitialDirectory = Environment.CurrentDirectory,
                Multiselect = true
            };
            if (ofd.ShowDialog() != DialogResult.OK || !ofd.FileNames.Any()) return;
            Environment.CurrentDirectory = Path.GetDirectoryName(ofd.FileNames[0]);
            FilesToAdd = ofd.FileNames;
            EnableDisable(false);
            fileLoader.RunWorkerAsync();
        }

        private void AddFiles()
        {
            Log("Adding " + FilesToAdd.Count() + (FilesToAdd.Count() == 1 ? " file" : " files"));
            OverWriteFiles = false;
            var InvalidFiles = new List<string>();
            var ValidFiles = new List<string>();
            var newPaths = new List<string>();
            foreach (var file in FilesToAdd)
            {
                if (UserCancelled) return;
                switch (Path.GetExtension(file).ToLowerInvariant())
                {
                    case ".rar":
                    case ".zip":
                    case ".7z":
                        var files = ExtractFiles(file);
                        if (!files.Any())
                        {
                            if (FilesToAdd.Count() == 1)
                            {
                                MessageBox.Show("File '" + Path.GetFileName(file) + "' is not a valid file", "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                            InvalidFiles.Add(file);
                        }
                        else
                        {
                            foreach (var f in files)
                            {
                                ProcessFile(f, ref ValidFiles, ref newPaths, FilesToAdd.Count() == 1);
                            }
                        }
                        break;
                    case ".exe":
                    case ".db":
                    case ".msi":
                    case ".ini":
                    case ".wav":
                    case ".mp3":
                    case ".flac":
                    case ".mkv":
                    case ".mp4":
                    case ".mpg":
                    case ".avi":
                    case ".wma":
                    case ".mogg":
                    case ".ogg":
                    case ".rbproj":
                    case ".pdf":
                    case ".txt":
                    case ".doc":
                    case ".docx":
                    case ".dll":
                    case ".bin":
                    case ".png":
                    case ".png_xbox":
                    case ".png_wii":
                    case ".png_ps3":
                    case ".gif":
                    case ".jpg":
                    case ".bmp":
                    case ".milo_xbox":
                    case ".milo_ps3":
                    case ".milo_wii":
                    case ".dta":
                    case ".log":
                    case ".csv":
                    case ".header":
                        if (FilesToAdd.Count() == 1)
                        {
                            MessageBox.Show("File '" + Path.GetFileName(file) + "' is not a valid file", "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        InvalidFiles.Add(file);
                        Log("");
                        break;
                    default:
                        ProcessFile(file, ref ValidFiles, ref newPaths, FilesToAdd.Count() == 1);
                        break;
                }
            }

            if (!ValidFiles.Any())
            {
                MessageBox.Show("No valid Rock Band files to copy", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Log("");
                return;
            }
            if (ValidFiles.Count != newPaths.Count)
            {
                MessageBox.Show("Something went wrong in analyzing where the files need to be copied to, sorry", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (InvalidFiles.Any())
            {
                MessageBox.Show("There " + (InvalidFiles.Count == 1 ? "was " : "were ") + InvalidFiles.Count + " invalid " + (InvalidFiles.Count == 1 ? "file " : "files ") + 
                    "you tried to copy to the drive and I ignored them\n\nPlease only use valid Rock Band files", "Invalid Files", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            var DuplicateFiles = newPaths.Where(File.Exists).ToList();
            if (DuplicateFiles.Any())
            {
                var message = "You're trying to copy " + DuplicateFiles.Count + (DuplicateFiles.Count == 1 ? " file" : " files") +
                              " that already " + (DuplicateFiles.Count == 1 ? "exists" : "exist") + " in the drive\nDo you want to overwrite " + (DuplicateFiles.Count == 1 ? 
                              "that file" : "those files") + "?\n\nClick 'Yes' to copy new files and overwrite existing files\nClick 'No' to only copy new files and ignore existing " +
                              "files\nClick 'Cancel' to stop this process without copying anything";
                var result = MessageBox.Show(message, "Overwrite Files?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                switch (result)
                {
                    case DialogResult.Yes:
                        OverWriteFiles = true;
                        break;
                    case DialogResult.No:
                        OverWriteFiles = false;
                        break;
                    default:
                        return;
                }
            }
            CopyFilesToDrive(ValidFiles, newPaths);
        }

        private void ProcessFile(string file, ref List<string> ValidFiles, ref List<string> newPaths, bool doMessage)
        {
            Log("Processing file '" + Path.GetFileName(file) + "'");
            if (VariousFunctions.ReadFileType(file) != XboxFileType.STFS)
            {
                if (doMessage)
                {
                    MessageBox.Show("File '" + Path.GetFileName(file) + "' is not a valid Rock Band file", "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                return;
            }
            var xFile = new STFSPackage(file);
            if (!xFile.ParseSuccess)
            {
                if (doMessage)
                {
                    MessageBox.Show("Couldn't parse file '" + Path.GetFileName(file) + "' - I did NOT add it to your files", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                return;
            }
            var profile = xFile.Header.ProfileID.ToString("X").ToUpperInvariant() + "\\";
            var game_path = xFile.Header.TitleID.ToString("X").ToUpperInvariant() + "\\";
            var path = GamePath;
            switch (xFile.Header.ThisType)
            {
                case PackageType.SavedGame:
                    path += CON_PATH + ShortenFileName(file);
                    break;
                case PackageType.MarketPlace:
                    path += LIVE_PATH + ShortenFileName(file);
                    break;
                case PackageType.Installer:
                    path += TU_PATH + ShortenFileName(file);
                    break;
                default:
                    if (doMessage)
                    {
                        MessageBox.Show("File '" + Path.GetFileName(file) + "' is not a valid Rock Band file", "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    xFile.CloseIO();
                    return;
            }
            if (profile != Profile && xFile.Header.ProfileID != 0)
            {
                path = path.Replace(Profile, profile);
                var new_dir = Path.GetDirectoryName(path) + "\\";
                if (!Directory.Exists(new_dir))
                {
                    Directory.CreateDirectory(new_dir);
                }
            }
            if (game_path != RB3_PATH)
            {
                path = path.Replace(RB3_PATH, game_path);
                var new_dir = Path.GetDirectoryName(path) + "\\";
                if (!Directory.Exists(new_dir))
                {
                    Directory.CreateDirectory(new_dir);
                }
            }
            xFile.CloseIO();
            ValidFiles.Add(file);
            newPaths.Add(path);
        }

        private static string ShortenFileName(string file)
        {
            var name = Path.GetFileName(file);
            return name.Length > 42 ? name.Substring(0, 42) : name;
        }

        private void CopyFilesToDrive(IList<string> currentPaths, IList<string> newPaths)
        {
            picWorking.Invoke(new MethodInvoker(() => picWorking.Visible = false));
            progressBar.Invoke(new MethodInvoker(() => progressBar.Visible = true));
            var added = 0;
            for (var i = 0; i < currentPaths.Count; i++)
            {
                if (UserCancelled) break;
                if (!File.Exists(currentPaths[i])) continue;
                var isDuplicate = false;
                if (File.Exists(newPaths[i]))
                {
                    if (OverWriteFiles)
                    {
                        isDuplicate = true;
                        Tools.DeleteFile(newPaths[i]);
                    }
                    else
                    {
                        continue;
                    }
                }
                progressBar.BeginInvoke(new Action(() =>
                {
                    toolTip1.SetToolTip(progressBar, "Copying file " + (i + 1) + " of " + currentPaths.Count);
                }));
                Log("Copying file " + (i + 1) + " of " + currentPaths.Count + ": '" + Path.GetFileName(currentPaths[i] + "'")); 
                if (CopyFile(currentPaths[i], newPaths[i], false, false, OverWriteFiles, isDuplicate))
                {
                    Log("Added file '" + Path.GetFileName(currentPaths[i]) + "'");
                    added++;
                }
                else
                {
                    Log("");
                }
                lblDriveUsed.Invoke(new MethodInvoker(() => lblDriveUsed.Text = GetFormattedSize(GetDrive(DriveLetter).AvailableFreeSpace)));
                lblDriveUsed.Invoke(new MethodInvoker(() => lblDriveUsed.Refresh()));
            }
            if (added == 0)
            {
                Log("No files added");
            }
            else
            {
                Log("Added " + added + " " + (added == 1 ? "file" : "files"));
            }
        }

        private List<string> ExtractFiles(string file)
        {
            var exe = BIN + "7z.exe";
            var dll = BIN + "7z.dll";
            var files = new List<string>();
            if (!File.Exists(exe) || !File.Exists(dll))
            {
                MessageBox.Show("7Zip files are missing from the \\bin\\ folder so I can't extract file '" + Path.GetFileName(file) +
                    "'\nThis file will not be added to your drive", "Missing Files", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return files;
            }
            try
            {
                Log("Extracting file from archive '" + Path.GetFileName(file) + "'");
                var xFolder = Path.GetDirectoryName(file) + "\\" + Path.GetFileNameWithoutExtension(file) + " extracted\\";
                if (!Directory.Exists(xFolder))
                {
                    Directory.CreateDirectory(xFolder);
                }
                var startInfo = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    FileName = exe,
                    Arguments = "x -o\"" + xFolder + "\" \"" + file + "\"",
                    WorkingDirectory = Application.StartupPath + "\\bin\\"
                };
                var process = Process.Start(startInfo);
                do
                {
                    //wait
                } while (!process.HasExited);
                Application.DoEvents();
                process.Dispose();
                files.AddRange(Directory.GetFiles(xFolder, "*", SearchOption.TopDirectoryOnly).Where(f => VariousFunctions.ReadFileType(f) == XboxFileType.STFS));
                MessageBox.Show("You tried to add archive file '" + Path.GetFileName(file) + "' but that's not a valid format!\nI was able to extract the contents of the file for you and found " +
                    files.Count + " possible Rock Band files that I'm going to now try to add to your drive\n\nFor better results, extract all .RAR, .ZIP and .7Z files before trying to add them to your drive",
                    "Extracted Files", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Log("Archive contained " + files.Count + (files.Count == 1 ? "file" : "files"));
                return files;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error extracting contents of file '" + Path.GetFileName(file) + "'\n" + ex.Message,"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<string>();
            }
        }

        private void HandleDragDrop(object sender, DragEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(lblDriveLetter.Text))
            {
                MessageBox.Show("Open a drive first", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (isDragDrop)
            {
                isDragDrop = false;
                return;
            }
            FilesToAdd = ((string[])e.Data.GetData(DataFormats.FileDrop));
            EnableDisable(false);
            fileLoader.RunWorkerAsync();
        }

        private void HandleDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
        }
        
        private void copyTU4ToDrive_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(lblDriveLetter.Text))
            {
                MessageBox.Show("Open a drive first", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (!File.Exists(BIN + "tu4"))
            {
                MessageBox.Show("TU4 file is missing from \\bin\\ folder", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Log("Copying file: 'Title Update 4'");
            if (!CopyFile(BIN + "tu4", GamePath + TU_PATH + "tu00000001_00000000")) return;
            EnableDisable(true);
            lblDriveFiles.Text = lstFiles.Items.Count.ToString(CultureInfo.InvariantCulture);
        }

        private void USBnator_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!picWorking.Visible && !progressBar.Visible && !driveLoader.IsBusy && !fileLoader.IsBusy && !fileDeleter.IsBusy)
            {
                SaveConfig();
                return;
            }
            MessageBox.Show("Please wait for the current process to finish", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            e.Cancel = true;
        }

        private void deleteFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var count = lstFiles.SelectedItems.Count;
            if (count == 0) return;
            if (MessageBox.Show("Are you sure you want to delete the selected " + (count == 1 ? "file" : "files") + "?", "Confirm Deletion",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }
            FilesToDelete.Clear();
            ToDelete.Clear();
            for (var i = 0; i < lstFiles.SelectedItems.Count; i++)
            {
                FilesToDelete.Add(lstFiles.SelectedItems[i].Tag.ToString());
                ToDelete.Add(lstFiles.SelectedItems[i].Index);
            }
            if (!FilesToDelete.Any()) return;
            Log("Deleting " + FilesToDelete.Count + (FilesToDelete.Count == 1 ? " file" : " files"));
            EnableDisable(false);
            fileDeleter.RunWorkerAsync();
        }
        
        private void extractFileToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            var ofd = new FolderPicker
            {
                Title = "Select folder to extract file(s) to",
                InputPath = Environment.CurrentDirectory,
            };
            if (ofd.ShowDialog(IntPtr.Zero) != true) return;
            ExtractFolder = ofd.ResultPath;
            GetFilesToExtract();
            EnableDisable(false);
            fileExtractor.RunWorkerAsync();
        }

        private void GetFilesToExtract()
        {
            FilesToExtract.Clear();
            for (var i = 0; i < lstFiles.SelectedItems.Count; i++)
            {
                FilesToExtract.Add(lstFiles.SelectedItems[i].Tag.ToString());
            }
        }
        
        private void ExtractFiles()
        {
            for (var i = 0; i < FilesToExtract.Count; i++)
            {
                var file = FilesToExtract[i];
                Log("Extracting file " + (i + 1) + " of " + FilesToExtract.Count + ": '" + Path.GetFileName(file) + "'");
                CopyFile(file, ExtractFolder + "\\" + Path.GetFileName(file), true);
            }
        }
        
        private void lstFiles_ItemDrag(object sender, ItemDragEventArgs e)
        {
            GetFilesToExtract();
            isDragDrop = true;
            DoDragDrop(new DataObject(DataFormats.FileDrop, FilesToExtract.ToArray()), DragDropEffects.Copy);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(lblDriveLetter.Text))
            {
                MessageBox.Show("Open a drive first", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            OpenDrive();
        }

        private void SendToAudioAnalyzer_Click(object sender, EventArgs e)
        {
            Log("Sent file to Audio Analyzer");
            var handler = new AudioAnalyzer(lstFiles.SelectedItems[0].Tag.ToString());
            handler.Show();
            Log("");
        }

        private void RemoveDuplicateEntries()
        {
            var entries = new List<string>();
            var to_remove = new List<int>();
            for (var i = 0; i < lstFiles.Items.Count; i++)
            {
                var file = lstFiles.Items[i].Tag.ToString();
                if (entries.Contains(file))
                {
                    to_remove.Add(i);
                }
                else
                {
                    entries.Add(file);
                }
            }
            if (!to_remove.Any()) return;
            to_remove.Sort();
            to_remove.Reverse();
            foreach (var remove in to_remove)
            {
                lstFiles.Items.RemoveAt(remove);
            }
        }

        private void driveLoader_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            var conRB3 = 0;
            var conRB2 = 0;
            var conRB1 = 0;
            var liveRB3 = 0;
            var liveRB2 = 0;
            var liveRB1 = 0;
            var tu = 0;
            var save = 0;
            try
            {
                var liveFiles = Directory.GetFiles(GamePath.Replace(RB3_PATH, RB1_PATH) + LIVE_PATH, "*", SearchOption.TopDirectoryOnly);
                AnalyzeFiles(liveFiles);
                liveRB1 = liveFiles.Count();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching drive for RB1 LIVE files\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            try
            {
                var liveFiles = Directory.GetFiles(GamePath.Replace(RB3_PATH, RB2_PATH) + LIVE_PATH, "*", SearchOption.TopDirectoryOnly);
                AnalyzeFiles(liveFiles);
                liveRB2 = liveFiles.Count();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching drive for RB2 LIVE files\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            try
            {
                var liveFiles = Directory.GetFiles(GamePath + LIVE_PATH, "*", SearchOption.TopDirectoryOnly);
                AnalyzeFiles(liveFiles);
                liveRB3 = liveFiles.Count();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching drive for RB3 LIVE files\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            try
            {
                var conFiles = Directory.GetFiles(GamePath.Replace(RB3_PATH, RB1_PATH) + CON_PATH, "*", SearchOption.TopDirectoryOnly);
                AnalyzeFiles(conFiles);
                conRB1 = conFiles.Count();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching drive for RB1 CON files\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            try
            {
                var conFiles = Directory.GetFiles(GamePath.Replace(RB3_PATH, RB2_PATH) + CON_PATH, "*", SearchOption.TopDirectoryOnly);
                AnalyzeFiles(conFiles);
                conRB2 = conFiles.Count();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching drive for RB2 CON files\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            try
            {
                var conFiles = Directory.GetFiles(GamePath + CON_PATH, "*", SearchOption.TopDirectoryOnly);
                AnalyzeFiles(conFiles);
                conRB3 = conFiles.Count();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching drive for RB3 CON files\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            
            try
            {
                var tuFiles = Directory.GetFiles(GamePath + TU_PATH, "*", SearchOption.TopDirectoryOnly);
                AnalyzeFiles(tuFiles);
                tu = tuFiles.Count();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching drive for Title Update files\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            try
            {
                var saveFiles = Directory.GetFiles(DriveLetter, "band3", SearchOption.AllDirectories);//rb3
                AnalyzeFiles(saveFiles);
                save = saveFiles.Count();
                saveFiles = Directory.GetFiles(DriveLetter, "band", SearchOption.AllDirectories);//rb1 and rb2
                AnalyzeFiles(saveFiles);
                save += saveFiles.Count();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching drive for RB save game files\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            lblDriveFiles.Invoke(new MethodInvoker(() => lblDriveFiles.Text = (conRB1 + conRB2 + conRB3 + liveRB1 + liveRB2 + liveRB3 + tu + save).ToString(CultureInfo.InvariantCulture)));
        }

        private void driveLoader_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            RemoveDuplicateEntries();
            Log("Loaded " + lstFiles.Items.Count + (lstFiles.Items.Count == 1 ? " file" : " files"));
            try
            {
                var count = Convert.ToInt16(lblDriveFiles.Text);
                if (lstFiles.Items.Count < count)
                {
                    lblDriveFiles.Text = lstFiles.Items.Count + " (" + lblDriveFiles.Text + ")";
                }
            }
            catch (Exception)
            {}
            lstFiles.Sort();
            EnableDisable(true);
        }

        private void fileLoader_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            AddFiles();
        }

        private void fileLoader_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            FilesToAdd = new string[] { };
            if (!AddedFiles.Any())
            {
                EnableDisable(true);
                lblDriveFiles.Text = lstFiles.Items.Count.ToString(CultureInfo.InvariantCulture);
                return;
            }
            AddedFiles.Clear();
            EnableDisable(true);
            lblDriveFiles.Text = lstFiles.Items.Count.ToString(CultureInfo.InvariantCulture);
        }

        private void fileDeleter_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            picWorking.Invoke(new MethodInvoker(() => picWorking.Visible = false));
            progressBar.Invoke(new MethodInvoker(() => progressBar.Visible = true));
            for (var i = 0; i < FilesToDelete.Count; i++)
            {
                var file = FilesToDelete[i];
                Log("[" + (i + 1) + "/" + FilesToDelete.Count + "] Deleting file '" + Path.GetFileName(file) + "'");
                Tools.DeleteFile(file);
                progressBar.Invoke(new MethodInvoker(() => progressBar.Value = (int)((i + 1)*100.0/FilesToDelete.Count)));
                lblDriveUsed.Invoke(new MethodInvoker(() => lblDriveUsed.Text = GetFormattedSize(GetDrive(DriveLetter).AvailableFreeSpace)));
                lblDriveUsed.Invoke(new MethodInvoker(() => lblDriveUsed.Refresh()));
            }
        }

        private void fileDeleter_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            Log("");
            FilesToDelete.Clear();
            ToDelete.Sort();
            ToDelete.Reverse();
            foreach (var delete in ToDelete)
            {
                lstFiles.Items.RemoveAt(delete);
            }
            Log("Deleted " + ToDelete.Count + (ToDelete.Count == 1? " file" : " files"));
            lblDriveFiles.Text = lstFiles.Items.Count.ToString(CultureInfo.InvariantCulture);
            ToDelete.Clear();
            EnableDisable(true);
        }

        private void fileExtractor_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            ExtractFiles();
        }

        private void fileExtractor_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            Log("Extracted " + FilesToExtract.Count + " " + (FilesToExtract.Count == 1 ? "file" : "files"));
            FilesToExtract.Clear();
            ExtractFolder = "";
            EnableDisable(true);
        }

        private void lstFiles_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (lstFiles.SelectedItems.Count == 1 && lstFiles.SelectedItems[0].SubItems[1].Text != "Title Update")
            {
                SendToCONExplorer_Click(null,null);
            }
        }

        private void lstFiles_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column != ActiveSortColumn)
            {
                ListSorting = SortOrder.Ascending;
            }
            else
            {
                ListSorting = ListSorting == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            }
            ActiveSortColumn = e.Column;
            SortSongs();
        }

        private void SortSongs()
        {
            lstFiles.ListViewItemSorter = new ListViewItemComparer(lstFiles, ActiveSortColumn, ListSorting);
            lstFiles.Sort();
        }

        private void SendToSetlistManager_Click(object sender, EventArgs e)
        {
            Log("Sent file to Setlist Manager");
            var handler = new SetlistManager(Color.FromArgb(197, 34, 35), Color.White, lstFiles.SelectedItems[0].Tag.ToString());
            handler.ShowDialog();
            Log("");
        }

        private void showGridlines_Click(object sender, EventArgs e)
        {
            lstFiles.GridLines = showGridlines.Checked;
        }

        private void findFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(lblDriveLetter.Text))
            {
                MessageBox.Show("Open a drive first", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            const string message = "Enter search term (not case sensitive):";
            SearchTerm = Interaction.InputBox(message, Text).ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(SearchTerm)) return;
            DoSearch(0);
        }

        private void DoSearch(int index)
        {
            for (var i = index; i < lstFiles.Items.Count; i++)
            {
                var name = lstFiles.Items[i].SubItems[0].Text.ToLowerInvariant();
                var file = lstFiles.Items[i].SubItems[4].Text.ToLowerInvariant();
                var artist = lstFiles.Items[i].SubItems[5].Text.ToLowerInvariant();
                var song = lstFiles.Items[i].SubItems[6].Text.ToLowerInvariant();
                var songid = lstFiles.Items[i].SubItems[7].Text.ToLowerInvariant();
                var internalname = lstFiles.Items[i].SubItems[8].Text.ToLowerInvariant();
                if (!name.Contains(SearchTerm) && !file.Contains(SearchTerm) && !artist.Contains(SearchTerm) && !song.Contains(SearchTerm) && 
                    !songid.Contains(SearchTerm) && !internalname.Contains(SearchTerm))  continue;
                lstFiles.SelectedItems.Clear();
                lstFiles.Items[i].Selected = true;
                lstFiles.Items[i].EnsureVisible();
                return;
            }
            if (index != 0)
            {
                DoSearch(0);
            }
        }

        private void USBnator_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData != Keys.F3) return;
            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                findFileToolStripMenuItem_Click(null,null);
            }
            else
            {
                DoSearch(lstFiles.SelectedItems.Count == 0 ? 0 : lstFiles.SelectedItems[0].Index + 1);
            }
        }

        private void USBnator_Shown(object sender, EventArgs e)
        {
            Application.DoEvents();
            for (var i = 0; i < lstFiles.Columns.Count; i ++)
            {
                ColumnWidths[i] = lstFiles.Columns[i].Width;
            }
            toggleSongArtist.Enabled = grabSongMetadata.Checked;
            toggleSongTitle.Enabled = grabSongMetadata.Checked;
            toggleSongID.Enabled = grabSongMetadata.Checked;
            toggleInternalName.Enabled = grabSongMetadata.Checked;
            if (!grabSongMetadata.Checked)
            {
                lstFiles.Columns[5].Width = 0;
                lstFiles.Columns[6].Width = 0;
                lstFiles.Columns[7].Width = 0;
                lstFiles.Columns[8].Width = 0;
            }
            if (Clipboard.GetText().Contains("MagmaFile:"))
            {
                var files = Clipboard.GetText().Replace("MagmaFile:", "");
                {
                    if (files.Contains("|"))
                    {
                        MagmaFiles = files.Split(new[] {"|"}, StringSplitOptions.RemoveEmptyEntries).ToList();
                    }
                    else
                    {
                        MagmaFiles.Add(files);
                    }
                }
                var names = MagmaFiles.Count == 2 ? ("Files '" + Path.GetFileName(MagmaFiles[0]) + "' and '" + Path.GetFileName(MagmaFiles[1]) + "'")
                    : "File '" + Path.GetFileName(MagmaFiles[0]) + "'";
                MessageBox.Show(names + " received from Magma: C3 Roks Edition\nAfter opening your drive, right-click and choose 'Add Magma file(s)' to add " 
                    + (MagmaFiles.Count == 1? "it" : "them") + " to your customs", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                Clipboard.SetText(" ");
            }
            if (autoopenLastUsedDrive.Checked && !string.IsNullOrWhiteSpace(DriveLetter) && Directory.Exists(DriveLetter))
            {
                OpenDrive();
            }
            else
            {
                DriveLetter = "";
            }
        }

        private void addMagmaFile_Click(object sender, EventArgs e)
        {
            Log("Adding " + (MagmaFiles.Count == 1 ? "file" : "files") + " received from Magma: C3 Roks Edition");
            FilesToAdd = MagmaFiles.ToArray();
            MagmaFiles.Clear();
            EnableDisable(false);
            fileLoader.RunWorkerAsync();
        }

        private void resetFormSize_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
            Width = 1000;
            Height = 700;
            CenterToScreen();
        }

        private void UpdateToggles(object sender, EventArgs e)
        {
            var toggle = (ToolStripMenuItem) sender;
            for (var i = 0; i < Toggles.Count; i++)
            {
                if (Toggles[i] != toggle) continue;
                lstFiles.Columns[i].Width = toggle.Checked ? ColumnWidths[i] : 0;
            }
        }

        private void resetColumns_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < lstFiles.Columns.Count; i++)
            {
                lstFiles.Columns[i].DisplayIndex = i;
                lstFiles.Columns[i].Width = StaticColumnWidths[i];
                ColumnWidths[i] = StaticColumnWidths[i];
                Toggles[i].Checked = true;
                Toggles[i].Enabled = true;
            }
            grabSongMetadata.Checked = true;
        }

        private void SendToQuickPackEditor_Click(object sender, EventArgs e)
        {
            Log("Sent file to Quick Pack Editor");
            var file = lstFiles.SelectedItems[0].Tag.ToString();
            var handler = new QuickPackEditor(null, Color.FromArgb(34, 169, 31), Color.White, "", file);
            handler.ShowDialog();
            Log("");
            var extracted = Path.GetDirectoryName(file) + "\\dePACKed files\\";
            if (!Directory.Exists(extracted)) return;
            var files = Directory.GetFiles(extracted, "*.*", SearchOption.AllDirectories);
            if (!files.Any())
            {
                Tools.DeleteFolder(extracted, true);
                return;
            }
            foreach (var f in files)
            {
                CopyFile(f, GamePath + CON_PATH + Path.GetFileName(f), false, true);
            }
            Tools.DeleteFolder(extracted, true);
            OpenDrive();
        }

        private void filterAll_Click(object sender, EventArgs e)
        {
            filterAll.Checked = false;
            filterCON.Checked = false;
            filterLIVE.Checked = false;
            filterMisc.Checked = false;
            ((ToolStripMenuItem) sender).Checked = true;
            if (string.IsNullOrWhiteSpace(lblDriveLetter.Text))
            {
                return;
            }
            OpenDrive();
        }

        private void USBnator_Resize(object sender, EventArgs e)
        {
            picWorking.Left = (Width - picWorking.Width)/2;
        }

        private void Log(string text)
        {
            lblLog.Invoke(new MethodInvoker(() => lblLog.Text = text));
            lblLog.Invoke(new MethodInvoker(() => lblLog.Refresh()));
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var message = Tools.ReadHelpFile("usb");
            var help = new HelpForm(Text + " - Help", message, true);
            help.ShowDialog();
        }

        private void closeDriveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseDrive();
            DriveLetter = "";
        }

        private void grabSongMetadata_Click(object sender, EventArgs e)
        {
            lstFiles.Columns[5].Width = grabSongMetadata.Checked && toggleSongArtist.Checked ? ColumnWidths[5] : 0;
            lstFiles.Columns[6].Width = grabSongMetadata.Checked && toggleSongTitle.Checked ? ColumnWidths[6] : 0;
            lstFiles.Columns[7].Width = grabSongMetadata.Checked && toggleSongID.Checked ? ColumnWidths[7] : 0;
            lstFiles.Columns[8].Width = grabSongMetadata.Checked && toggleSongID.Checked ? ColumnWidths[8] : 0;
            toggleSongArtist.Enabled = grabSongMetadata.Checked;
            toggleSongTitle.Enabled = grabSongMetadata.Checked;
            toggleSongID.Enabled = grabSongMetadata.Checked;
            toggleInternalName.Enabled = grabSongMetadata.Checked;
            if (string.IsNullOrWhiteSpace(lblDriveLetter.Text))
            {
                return;
            }
            OpenDrive();
        }

        private void refreshToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            refreshToolStripMenuItem_Click(sender, e);
        }

        private void lstFiles_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.A || !ModifierKeys.HasFlag(Keys.Control)) return;
            for (var i = 0; i < lstFiles.Items.Count; i++)
            {
                lstFiles.Items[i].Selected = true;
            }
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

        private bool CopyFileEx(string SourceFile, string DestFile)
        {
            var buffer = new byte[1024*1024]; // 1MB buffer
            try
            {
                using (var source = new FileStream(SourceFile, FileMode.Open, FileAccess.Read))
                {
                    var fileLength = source.Length;
                    using (var dest = new FileStream(DestFile, FileMode.CreateNew, FileAccess.Write))
                    {
                        long totalBytes = 0;
                        int currentBlockSize;
                        while ((currentBlockSize = source.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            totalBytes += currentBlockSize;
                            var percentage = totalBytes*100.0/fileLength;
                            dest.Write(buffer, 0, currentBlockSize);
                            progressBar.Invoke(new MethodInvoker(() => progressBar.Value = (int) percentage));
                            if (!UserCancelled) continue;
                            dest.Dispose();
                            Tools.DeleteFile(DestFile);
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error copying file '" + Path.GetFileName(SourceFile) + "'\nError says: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Tools.DeleteFile(DestFile);
                return false;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            UserCancelled = true;
            Log("Cancelling operation...");
        }

        private void progressBar_VisibleChanged(object sender, EventArgs e)
        {
            if (progressBar.Visible)
            {
                btnCancel.Visible = true;
            }
        }

        private void picWorking_VisibleChanged(object sender, EventArgs e)
        {
            if (picWorking.Visible)
            {
                btnCancel.Visible = true;
            }
        }

        private void exportListToCSV_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog
            {
                Title = Text,
                InitialDirectory = Tools.CurrentFolder,
                OverwritePrompt = true,
                AddExtension = true,
                Filter = "CSV Files (*.csv)|*csv"
            };
            if (sfd.ShowDialog() != DialogResult.OK || string.IsNullOrWhiteSpace(sfd.FileName)) return;
            Tools.CurrentFolder = Environment.CurrentDirectory;
            var filename = sfd.FileName;
            if (string.IsNullOrWhiteSpace(Path.GetExtension(filename)))
            {
                filename = filename + ".csv";
            }

            var sw = new StreamWriter(filename, false);
            try
            {
                var line = "";
                for (var i = 0; i < lstFiles.Columns.Count; i++)
                {
                    line = line + "\"" + lstFiles.Columns[i].Text + "\",";
                }
                sw.WriteLine(line);

                for (var x = 0; x < lstFiles.Items.Count; x++)
                {
                    line = "";
                    for (var i = 0; i < lstFiles.Columns.Count; i++)
                    {
                        line = line + "\"" + lstFiles.Items[x].SubItems[i].Text + "\",";
                    }
                    sw.WriteLine(line);
                }
                sw.Dispose();

                MessageBox.Show("Exported to CSV successfully", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                sw.Dispose();
                MessageBox.Show("Exporting to CSV failed\nThe error says: " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void copyTU5ToDriveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(lblDriveLetter.Text))
            {
                MessageBox.Show("Open a drive first", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (!File.Exists(BIN + "tu5"))
            {
                MessageBox.Show("TU5 file is missing from \\bin\\ folder", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Log("Copying file: 'Title Update 5'");
            if (!CopyFile(BIN + "tu5", GamePath + TU_PATH + "tu00000001_00000000")) return;
            EnableDisable(true);
            lblDriveFiles.Text = lstFiles.Items.Count.ToString(CultureInfo.InvariantCulture);
        }
    }
}
