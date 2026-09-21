using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using Nautilus.Properties;
using Nautilus.x360;
using System.Text;

namespace Nautilus
{
    public partial class BatchProcessor : Form
    {
        private readonly List<string> inputFiles;
        private readonly NemoTools Tools;
        private readonly DTAParser Parser;
        private STFSPackage xPackage;
        private static Color mMenuBackground;
        
        public BatchProcessor(Color ButtonBackColor, Color ButtonTextColor)
        {
            InitializeComponent();

            Tools = new NemoTools();
            Parser = new DTAParser();
            inputFiles = new List<string>();
            mMenuBackground = menuStrip1.BackColor;
            menuStrip1.Renderer = new DarkRenderer();

            var formButtons = new List<Button> { btnRefresh, btnFolder, btnBegin };
            foreach (var button in formButtons)
            {
                button.BackColor = ButtonBackColor;
                button.ForeColor = ButtonTextColor;
                button.FlatAppearance.MouseOverBackColor = button.BackColor == Color.Transparent ? Color.FromArgb(127, Color.AliceBlue.R, Color.AliceBlue.G, Color.AliceBlue.B) : Tools.LightenColor(button.BackColor);
            }
        }

        private void btnFolder_Click(object sender, EventArgs e)
        {            
            var tFolder = txtFolder.Text;
            var ofd = new FolderPicker
            {
                InputPath = tFolder,
                Title = "Select folder where your source files are",
            };
            if (ofd.ShowDialog(IntPtr.Zero) == true)
            {
                txtFolder.Text = ofd.ResultPath;
                Tools.CurrentFolder = txtFolder.Text;
            }
            else
            {
                txtFolder.Text = tFolder;
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

        private void txtFolder_TextChanged(object sender, EventArgs e)
        {
            if (picWorking.Visible) return;
            inputFiles.Clear();
            if (string.IsNullOrWhiteSpace(txtFolder.Text))
            {
                btnRefresh.Visible = false;
            }
            btnRefresh.Visible = true;
            inputFiles.Clear();

            if (txtFolder.Text != "")
            {
                Tools.CurrentFolder = txtFolder.Text;
                Log("");
                Log("Reading input directory ... hang on");
                EnableDisable(false);
                folderScanner.RunWorkerAsync();
            }
            else
            {
                btnBegin.Visible = false;
                btnRefresh.Visible = false;
                txtFolder.Focus();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            var tFolder = txtFolder.Text;
            txtFolder.Text = "";
            txtFolder.Text = tFolder;
        }

        private void btnBegin_Click(object sender, EventArgs e)
        {
            if (btnBegin.Text == "Cancel")
            {
                FileProcessor.CancelAsync();
                Log("User cancelled process...stopping as soon as possible");
                btnBegin.Enabled = false;
                return;
            }
            if (!chkAuthor.Checked && !chkOrigin.Checked && !chkOverrideAuthor.Checked && !chkVocalGender.Checked && !chkSongID.Checked && !chkOverrideGameID.Checked && !chkAddYear.Checked && !chkDefAutID.Checked && !chkFamilyFriendly.Checked)
            {
                MessageBox.Show("No options selected, nothing to do", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Log("No options selected, nothing to do");
                return;
            }
            if (chkOrigin.Checked && txtOrigin.Text.Trim().Length == 0)
            {
                MessageBox.Show("Game origin can't be blank!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Log("Game origin can't be blank!");
                return;
            }
            EnableDisable(false);
            btnBegin.Text = "Cancel";
            toolTip1.SetToolTip(btnBegin, "Click to cancel process");
            FileProcessor.RunWorkerAsync();
        }

        private void HandleDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
        }

        private void HandleDragDrop(object sender, DragEventArgs e)
        {
            if (picWorking.Visible) return;
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            inputFiles.Clear();
            var dtaFiles = 0;
            foreach (var file in files)
            {
                var ext = Path.GetExtension(file).ToLowerInvariant();
                if (ext == ".dta")
                {
                    dtaFiles++;
                    inputFiles.Add(file);
                } 
            }
            if (dtaFiles > 0)
            {
                EnableDisable(false);
                btnBegin.Text = "Cancel";
                toolTip1.SetToolTip(btnBegin, "Click to cancel process");
                FileProcessor.RunWorkerAsync();
                return;
            }
            txtFolder.Text = Path.GetDirectoryName(files[0]);
            Tools.CurrentFolder = txtFolder.Text;
        }

        private void exportLogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.ExportLog(Text, lstLog.Items);
        }

        private void DTAProcessor_Shown(object sender, EventArgs e)
        {
            Log("Welcome to " + Text);
            Log("Drag and drop the CON / LIVE file(s) here");
            Log("Or click 'Change Input Folder' to select the files");
            Log("Ready to begin");
            cboVocalGender.SelectedIndex = 0;
        }

        private void EnableDisable(bool enabled)
        {
            btnFolder.Enabled = enabled;
            btnRefresh.Enabled = enabled;
            txtFolder.Enabled = enabled;
            picWorking.Visible = !enabled;            
            chkAuthor.Enabled = enabled;
            chkOrigin.Enabled = enabled;
            chkOverrideAuthor.Enabled = enabled;
            chkVocalGender.Enabled = enabled;
            chkFamilyFriendly.Enabled = enabled;
            chkRecursive.Enabled = enabled;
            menuStrip1.Enabled = enabled;
            chkSongID.Enabled = enabled;
            chkDefAutID.Enabled = enabled;
            lstLog.Cursor = enabled ? Cursors.Default : Cursors.WaitCursor;
            Cursor = lstLog.Cursor;
            chkFallback.Enabled = enabled;
            if (!enabled)
            {
                txtFallback.Enabled = false;
            }
        }

        private void DTAProcessor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!picWorking.Visible) return;
            MessageBox.Show("Please wait until the current process finishes", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            e.Cancel = true;
        }

        private bool ModifyDTA()
        {
            return chkAuthor.Checked || chkOrigin.Checked || chkOverrideAuthor.Checked || chkVocalGender.Checked || chkSongID.Checked || chkAddYear.Checked || chkDefAutID.Checked || chkFamilyFriendly.Checked;
        }

        private void FileProcessor_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            
            foreach (var file in inputFiles.Where(File.Exists).TakeWhile(file => !FileProcessor.CancellationPending))
            {
                var tempFolder = Application.StartupPath + "\\temp\\";
                if (!Directory.Exists(tempFolder))
                {
                    Directory.CreateDirectory(tempFolder);
                }

                try
                {
                    switch (Path.GetExtension(file).ToLowerInvariant())
                    {                        
                        case ".dta":
                            SeparateDTA(file);
                            break;
                        default:
                            if (VariousFunctions.ReadFileType(file) == XboxFileType.STFS)
                            {
                                try
                                {
                                    Log("Opening file '" + Path.GetFileName(file) + "'");
                                    xPackage = new STFSPackage(file);
                                    if (!xPackage.ParseSuccess)
                                    {
                                        Log("Error opening file '" + Path.GetFileName(file) + "'. Skipping");
                                        xPackage.CloseIO();
                                    }
                                    else
                                    {
                                        if (ModifyDTA())
                                        {
                                            var xFile = xPackage.GetFile("songs/songs.dta");
                                            var xDTA = xFile.Extract();
                                            if (xDTA.Length == 0)
                                            {
                                                Log("Error extracting the DTA file, skipping this file");
                                                xPackage.CloseIO();
                                                continue;
                                            }
                                            if (!Parser.ReadDTA(xDTA) || !Parser.Songs.Any())
                                            {
                                                Log("Error parsing the DTA file, skipping this file");
                                                xPackage.CloseIO();
                                                continue;
                                            }
                                            Log("Extracting " + xFile.Name);
                                            var currDTA = tempFolder + xFile.Name;
                                            Tools.DeleteFile(currDTA);
                                            if (!xFile.ExtractToFile(currDTA))
                                            {
                                                Log("Error extracting DTA file " + xFile.Name);
                                                Enabled = true;
                                                xPackage.CloseIO();
                                                continue;
                                            }
                                            if (File.Exists(currDTA))
                                            {
                                                Log("DTA file extracted successfully");
                                            }
                                            else
                                            {
                                                Log("Could not extract file DTA file");
                                            }
                                            var newDTA = tempFolder + "new.DTA";
                                            if (File.Exists(newDTA))
                                            {
                                                Tools.DeleteFile(newDTA);
                                            }

                                            var didStems = false;
                                            var changedAuthor = false;
                                            var songTitle = Parser.Songs[0].Name;
                                            var yearReleased = Parser.Songs[0].YearReleased;
                                            var encoding = Parser.DetectEncoding(File.ReadAllBytes(currDTA));
                                            var sr = new StreamReader(currDTA, encoding);
                                            var sw = new StreamWriter(newDTA, false, new UTF8Encoding(false, false));
                                            var line = "";
                                            while (!sr.EndOfStream)
                                            {
                                                line = sr.ReadLine();
                                                if (line.Contains("latin1"))
                                                {
                                                    sw.WriteLine(line.Replace("latin1", "utf8"));
                                                    line = sr.ReadLine();
                                                }
                                                if (line.ToLowerInvariant().Contains("game_origin") && chkOrigin.Checked && txtOrigin.Text.Length > 0)
                                                {
                                                    var origin = "";
                                                    if (chkFallback.Checked && txtFallback.Text.Length > 0)
                                                    {
                                                        origin = "#ifdef CUSTOMSOURCE '" + txtOrigin.Text + "' #else '" + txtFallback.Text + "' #endif";
                                                    }
                                                    else
                                                    {
                                                        origin = "'" + txtOrigin.Text + "'";
                                                    }
                                                    line = "   ('game_origin' " + origin + ")";
                                                    sw.WriteLine(line);
                                                    line = sr.ReadLine();
                                                }
                                                if (line.ToLowerInvariant().Contains("rating") && chkFamilyFriendly.Checked)
                                                {
                                                    line = "   ('rating' 1)";
                                                    sw.WriteLine(line);
                                                    line = sr.ReadLine();
                                                }
                                                if ((line.Contains("subgenre") || line.Contains("sub_genre")) && chkSubgenre.Checked)
                                                {
                                                    line = sr.ReadLine(); //skip it
                                                }
                                                else if ((line.Contains("\"" + songTitle + "\"") || line.Contains("'" + songTitle + "'")) && yearReleased > 0 && chkAddYear.Checked)
                                                {
                                                    if (line.Contains("(" + yearReleased + ")")) //in case of being ran through multiple times...god knows why
                                                    {
                                                        line = line.Replace("(" + yearReleased + ")", "");
                                                    }
                                                    line = line.Replace(songTitle, "(" + yearReleased + ") " + songTitle);
                                                }
                                                else if (line.Contains("song_id") && !line.Contains(";ORIG_ID=") && !line.Trim().StartsWith(";") && chkSongID.Checked)
                                                {
                                                    if (!Parser.IsNumericID(line) || chkForceNumericOverride.Checked || ((chkDefAutID.Checked && Parser.GetSongID(line).StartsWith("1000") && Parser.GetSongID(line).Length == 10))) //only if not already a numeric ID or forced or it has the default 1000 MAGMA C3 Author ID
                                                    {
                                                        var origID = Parser.GetSongID(line);
                                                        sw.WriteLine(";ORIG_ID=" + origID);
                                                        var corrector = new SongIDCorrector();
                                                        line = "   ('song_id' " + corrector.ShortnameToSongID(Parser.Songs[0].ShortName) + ")";
                                                        sw.WriteLine(line);
                                                        line = sr.ReadLine();
                                                    }
                                                }
                                            vocal_gender:
                                                if (line.ToLowerInvariant().Contains("vocal_gender") && chkVocalGender.Checked)
                                                {
                                                    line = "   ('vocal_gender' '" + cboVocalGender.Text.ToLowerInvariant() + "')";
                                                    sw.WriteLine(line);
                                                    line = sr.ReadLine();
                                                }
                                                if (line.ToLowerInvariant().Contains("genre") && !line.ToLowerInvariant().Contains("sub_genre"))
                                                {
                                                    sw.WriteLine(line);
                                                    if (chkOverrideAuthor.Checked && txtAuthor.Text.Length > 0)
                                                    {
                                                        sw.WriteLine("   (author \"" + txtAuthor.Text + "\")");
                                                        changedAuthor = true;
                                                    }
                                                    else if (Parser.Songs[0].ChartAuthor.Length == 0)
                                                    {
                                                        Log("Could not find author information in that DTA file, skipping this step");
                                                    }
                                                    else if (chkAuthor.Checked)
                                                    {
                                                        sw.WriteLine("   (author \"" + Parser.Songs[0].ChartAuthor.Replace(";", " ") + "\")");
                                                    }
                                                    line = sr.ReadLine();
                                                    if (line.ToLowerInvariant().Contains("vocal_gender"))
                                                    {
                                                        goto vocal_gender;
                                                    }
                                                    if (line.Contains("(author") && changedAuthor)
                                                    {
                                                        sw.WriteLine("   ;ORIG_AUTHOR=" + line.Replace("(author ", "").Replace(")", "").Trim());
                                                    }
                                                    else
                                                    {
                                                        sw.WriteLine(line);
                                                    }
                                                    line = sr.ReadLine();
                                                    if (line.ToLowerInvariant().Contains("vocal_gender"))
                                                    {
                                                        goto vocal_gender;
                                                    }
                                                }
                                                else if (line.Contains(";Karaoke=1") && chkDIYStems.Checked && !didStems)
                                                {
                                                    sw.WriteLine(";DIYStems=1");
                                                    line = ";Karaoke=0";
                                                    didStems = true;
                                                }
                                                else if (line.Contains(";Multitrack=1") && chkDIYStems.Checked && !didStems)
                                                {
                                                    sw.WriteLine(";DIYStems=1");
                                                    line = ";Multitrack=0";
                                                    didStems = true;
                                                }
                                                else if (line.Contains(";DIYStems") && chkDIYStems.Checked && didStems)
                                                {
                                                    line = ""; //avoid a duplicate entry
                                                }
                                                else if (line.ToLowerInvariant().Contains("'author'") && !line.Contains(")"))
                                                {
                                                    sw.WriteLine(line);
                                                    sw.WriteLine(txtAuthor.Text);
                                                    line = sr.ReadLine();
                                                }
                                                if (!string.IsNullOrEmpty(line))
                                                {
                                                    sw.WriteLine(line);
                                                }
                                            }
                                            sr.Dispose();
                                            sw.Close();
                                            Log("DTA file modified, putting it back in the CON file");
                                            if (!xFile.Replace(newDTA))
                                            {
                                                Log("Failed to replace modified DTA into CON file");
                                                xPackage.CloseIO();
                                                continue;
                                            }
                                            else
                                            {
                                                Log("Replaced modified DTA in CON file successfully");
                                            }
                                            Tools.DeleteFolder(tempFolder, true);
                                        }

                                        //sign file
                                        var success = true;
                                        xPackage.Header.ThisType = PackageType.SavedGame;
                                        xPackage.Header.MakeAnonymous();
                                        if (chkOverrideGameID.Checked)
                                        {
                                            if (gameRB1.Checked)
                                            {
                                                xPackage.Header.TitleID = 0x45410829;
                                                xPackage.Header.Title_Package = "Rock Band 1";
                                                xPackage.Header.ContentImage = Resources.RB1;
                                            }
                                            else if (gameRB2.Checked)
                                            {
                                                xPackage.Header.TitleID = 0x45410869;
                                                xPackage.Header.Title_Package = "Rock Band 2";
                                                xPackage.Header.ContentImage = Resources.RB2;
                                            }
                                            else if (gameRB3.Checked)
                                            {
                                                xPackage.Header.TitleID = 0x45410914;
                                                xPackage.Header.Title_Package = "Rock Band 3";
                                                xPackage.Header.ContentImage = Resources.RB3;
                                            }

                                            Log("Changed game ID and content image to " + xPackage.Header.Title_Package);
                                        }
                                        try
                                        {
                                            Log("Rebuilding CON file ... this might take a little while");
                                            var signature = new RSAParams(Application.StartupPath + "\\bin\\KV.bin");
                                            xPackage.RebuildPackage(signature);
                                            xPackage.FlushPackage(signature);
                                            xPackage.CloseIO();
                                        }
                                        catch
                                        {
                                            Log("Something went wrong with trying to rebuild CON file");
                                            success = false;
                                        }
                                        if (success)
                                        {
                                            Log("Trying to unlock CON file");
                                            if (Tools.UnlockCON(file))
                                            {
                                                Log("Unlocked CON file successfully");
                                            }
                                            else
                                            {
                                                Log("Error unlocking CON file");
                                                success = false;
                                            }
                                        }
                                        if (success)
                                        {
                                            Log("Trying to sign CON file");
                                            if (Tools.SignCON(file))
                                            {
                                                Log("CON file signed successfully");
                                            }
                                            else
                                            {
                                                Log("Error signing CON file");
                                                Log("If you just extracted CON file, this is a known bug");
                                                Log("Close this form, open the song again, and try again.");
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Log("There was an error processing file '" + Path.GetFileName(file) + "'. Skipping");
                                    Log("The error says: " + ex.Message);
                                }
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Log("There was a problem accessing that file");
                    Log("The error says: " + ex.Message);
                }
            }
        }

        private void FileProcessor_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            FinishWorkers();
        }

        private void FinishWorkers()
        {
            EnableDisable(true);
            Log("Done.");
            toolTip1.SetToolTip(btnBegin, "Click to begin renaming process");
            btnBegin.Text = "&Begin";
            btnBegin.Enabled = true;
            btnBegin.Visible = false;
        }

        private void folderScanner_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                var inFiles = new string[] { };
                txtFolder.Invoke(new MethodInvoker(() => inFiles = Directory.GetFiles(txtFolder.Text,".", chkRecursive.Checked? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)));                
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
            }
            catch (Exception ex)
            {
                Log("There was an error: " + ex.Message);
            }
        }

        private void folderScanner_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (!inputFiles.Any())
            {
                Log("Did not find any CON files ... try a different directory");
                Log("You can also drag and drop CON files here");
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
            EnableDisable(true);
            txtFolder.Focus();
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

        private void separateDTA_Click(object sender, EventArgs e)
        {
            inputFiles.Clear();
            var ofd = new OpenFileDialog() { Title = "Select DTA file", DefaultExt = ".dta", Multiselect = true, Filter = "DTA Files (*.dta)|" };
            ofd.ShowDialog();

            var dtaFiles = 0;
            foreach (var file in ofd.FileNames)
            {
                var ext = Path.GetExtension(file.ToLowerInvariant());
                if (ext == ".dta")
                {
                    dtaFiles++;
                    inputFiles.Add(file);
                }
            }
            ofd.Dispose();
            if (dtaFiles > 0)
            {
                EnableDisable(false);
                btnBegin.Text = "Cancel";
                toolTip1.SetToolTip(btnBegin, "Click to cancel process");                
                FileProcessor.RunWorkerAsync();
                return;
            }

            if (ofd.FileNames.Count() == 0)
            {
                Log("No DTA file opened");
            }
        }

        private void SeparateDTA(string packDTA)
        {
            Log("Opened DTA file \"" + Path.GetFileName(packDTA) + "\"");
            if (!Parser.ReadDTA(File.ReadAllBytes(packDTA)))
            {
                Log("Failed to parse DTA file \"" + Path.GetFileName(packDTA) + "\"");
            }
            Log("DTA file \"" + Path.GetFileName(packDTA) + "\" contains " + Parser.Songs.Count + " component DTA file(s)");
            var dtaFolder = Path.GetDirectoryName(packDTA) + "\\dta_files\\";
            if (!Directory.Exists(dtaFolder))
            {
                Directory.CreateDirectory(dtaFolder);
            }
            foreach (var song in Parser.Songs)
            {
                var newDTA = dtaFolder + Tools.CleanString(song.Artist + " - " + song.Name + ".dta",false,true);
                if (File.Exists(newDTA))
                {
                    Log("Duplicate DTA file found: " + newDTA);
                    Tools.DeleteFile(newDTA);
                }
                var sw = new StreamWriter(newDTA, false, new UTF8Encoding(false, false));
                foreach (var line in song.DTALines)
                {
                    if (line.Contains("latin1"))
                    {
                        sw.WriteLine(line.Replace("latin1", "utf8"));
                    }
                    else
                    {
                        sw.WriteLine(line);
                    }
                }
                sw.Dispose();
            }
            var count = Directory.GetFiles(dtaFolder).Count();
            if (count == 0)
            {
                Log("Failed to separate DTA files");
            }
            else
            {
                Log("Separated " + count + " DTA file(s)");
            }
        }              

        private void chkOrigin_CheckedChanged(object sender, EventArgs e)
        {
            txtOrigin.Enabled = chkOrigin.Checked;
            chkFallback.Enabled = chkOrigin.Checked;
            if (!chkOrigin.Checked)
            {
                txtFallback.Enabled = false;
            }
        }

        private void chkOverrideAuthor_CheckedChanged(object sender, EventArgs e)
        {
            txtAuthor.Enabled = chkOverrideAuthor.Checked;
        }

        private void chkVocalGender_CheckedChanged(object sender, EventArgs e)
        {
            cboVocalGender.Enabled = chkVocalGender.Checked;
        }

        private void chkFallback_CheckedChanged(object sender, EventArgs e)
        {
            txtFallback.Enabled = chkFallback.Checked;
        }

        private void chkOverrideGameID_CheckedChanged(object sender, EventArgs e)
        {
            gameRB1.Enabled = chkOverrideGameID.Checked;
            gameRB2.Enabled = chkOverrideGameID.Checked;
            gameRB3.Enabled = chkOverrideGameID.Checked;
        }

        private void sortSongsByDTALanguageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFolder.Text))
            {
                MessageBox.Show("Please select a folder that contains your song files first, then click this menu button again", Text, MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }
            if (inputFiles.Count == 0)
            {
                MessageBox.Show("No valid input files found", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            EnableDisable(false);
            List<string> languages = new List<string>();            
            var folders = txtFolder.Text + "\\";

            foreach (var file in inputFiles)
            {
                if (VariousFunctions.ReadFileType(file) != XboxFileType.STFS)
                {
                    continue;
                }
                var xDTA = Parser.ExtractDTA(file);
                if (!xDTA) continue;
                if (Parser.Songs.Count != 1) continue; //no packs or errored files
                var language = Parser.Songs[0].Languages.Replace(",", "|");
                if (string.IsNullOrEmpty(language))
                {
                    Log("File '" + Path.GetFileName(file) + "' does not have a language tag, skipping");
                    continue;
                }
                if (!languages.Contains(language))
                {
                    languages.Add(language);
                }
                Log("File '" + Path.GetFileName(file) + "' is tagged with language(s): " + language);
                var newFolder = folders + language.Replace("|", "") + "\\";
                if (!Directory.Exists(newFolder))
                {
                    Directory.CreateDirectory(newFolder);
                }
                if (Tools.MoveFile(file, newFolder + Path.GetFileName(file)))
                {
                    Log("Sorted successfully");
                }
                else
                {
                    Log("Failed to sort");
                }                
            }
            if (languages.Count > 0)
            {
                Log("Found songs with the following language tags:");
                foreach (var language in languages)
                {
                    Log(language);
                }
            }
            else
            {
                Log("Could not find any language tags for the songs that were processed");
            }

            EnableDisable(true);
        }
    }
}
