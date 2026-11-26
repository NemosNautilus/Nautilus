using Nautilus.Properties;
using Nautilus.x360;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Color = System.Drawing.Color;

namespace Nautilus
{
    public partial class FileIndexer : Form
    {
        private readonly NemoTools Tools;
        private readonly DTAParser Parser;
        private readonly string Index;
        private readonly string config;
        private readonly List<SongIndex> Songs;
        private List<SongIndex> FilteredSongs;
        private const int LIST_SPACER = 24;
        //private const double LIST_SIZER1 = 0.505;
        //private const double LIST_SIZER2 = 0.495;
        private bool DoMaximize;
        private bool DontUpdate;
        private int ActiveSortColumn;
        public SortOrder ListSorting = SortOrder.Ascending;
        private bool CancelWorkers;
        private const string TypeToSearch = "Type to search...";
        private bool SearchForIDConflicts;
        private bool SearchForDuplicates;
        private Visualizer visualizer;
        private bool PS3Mode;

        public FileIndexer(Color ButtonBackColor, Color ButtonTextColor)
        {
            InitializeComponent();
            Tools = new NemoTools();
            Parser = new DTAParser();
            Songs = new List<SongIndex>();
            visualizer = new Visualizer(Color.FromArgb(230, 215, 0), Color.White, "");
            FilteredSongs = new List<SongIndex>();
            config = Application.StartupPath + "\\bin\\config\\indexer.config";
            LoadConfig();
            var indexFolder = Application.StartupPath + "\\bin\\indexer\\";
            if (!Directory.Exists(indexFolder))
            {
                Directory.CreateDirectory(indexFolder);
            }
            Index = indexFolder + "index.nautilus";
            var formButtons = new List<Button> {btnBuild, btnClear, btnDelete, btnNew, btnClearSearch};
            foreach (var button in formButtons)
            {
                button.BackColor = ButtonBackColor;
                button.ForeColor = ButtonTextColor;
                button.FlatAppearance.MouseOverBackColor = button.BackColor == Color.Transparent ? Color.FromArgb(127, Color.AliceBlue.R, Color.AliceBlue.G, Color.AliceBlue.B) : Tools.LightenColor(button.BackColor);
            }
        }

        private void SaveConfig()
        {
            var sw = new StreamWriter(config, false);
            sw.WriteLine("FolderCount=" + lstFolders.Items.Count);
            foreach (var item in lstFolders.Items)
            {
                sw.WriteLine(item.ToString());
            }
            sw.WriteLine("SearchSubDirs=" + chkSubDirs.Checked);
            sw.WriteLine("OpenMaximized=" + (WindowState == FormWindowState.Maximized));
            sw.WriteLine("PS3Mode=" + chkPS3.Checked);
            sw.Dispose();
        }

        private void LoadConfig()
        {
            if (!File.Exists(config)) return;
            var sr = new StreamReader(config);
            var line = sr.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
            {
                sr.Dispose();
                return;
            }
            var count = Convert.ToInt16(Regex.Match(line, @"\d+").Value);
            for (var i = 0; i < count; i++)
            {
                var folder = Tools.GetConfigString(sr.ReadLine());
                if (Directory.Exists(folder))
                {
                    lstFolders.Items.Add(folder);
                }
            }
            chkSubDirs.Checked = sr.ReadLine().Contains("True");
            DoMaximize = sr.ReadLine().Contains("True");
            try
            {
                chkPS3.Checked = sr.ReadLine().Contains("True");
            }
            catch { }
            sr.Dispose();
            CheckFolderCount();
        }

        private void CheckFolderCount()
        {
            var enabled = lstFolders.Items.Count > 0;
            btnClear.Enabled = enabled;
            btnBuild.Enabled = enabled;
            chkSubDirs.Enabled = enabled;
        }

        private void LoadIndex()
        {
            if (!File.Exists(Index)) return;
            Songs.Clear();
            var sr = new StreamReader(Index); 
            var line = sr.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
            {
                sr.Dispose();
                return;
            }
            if (line.Contains("NewFormat") || line.Contains("Format2026"))
            {
                var count = Convert.ToInt16(Tools.GetConfigString(sr.ReadLine()));
                for (var i = 0; i < count; i++)
                {
                    var newSong = new SongIndex();
                    newSong.Name = Tools.GetConfigString(sr.ReadLine());
                    newSong.SongID = Tools.GetConfigString(sr.ReadLine());
                    newSong.Location = Tools.GetConfigString(sr.ReadLine());                    
                    if (line.Contains("Format2026"))
                    {
                        newSong.InternalName = Tools.GetConfigString(sr.ReadLine());
                    }
                    if (!File.Exists(newSong.Location) && !Directory.Exists(newSong.Location)) continue;
                    Songs.Add(newSong);
                }
            }
            else
            {
                var count = Convert.ToInt16(Tools.GetConfigString(line));
                for (var i = 0; i < count; i++)
                {
                    var name = Tools.GetConfigString(sr.ReadLine());
                    var path = Tools.GetConfigString(sr.ReadLine());
                    if (!File.Exists(path)) continue;
                    Songs.Add(new SongIndex());
                    Songs[Songs.Count - 1].Name = name;
                    Songs[Songs.Count - 1].Location = path;
                }
            }
            sr.Dispose();
            DisplayIndexedFiles();
            SortSongs();
            EnableSearch(Songs.Any());
        }

        private void EnableSearch(bool enabled)
        {
            txtSearch.Enabled = enabled;
            radioPackages.Enabled = enabled;
            radioSongs.Enabled = enabled;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lstFolders.Items.Clear();
            CheckFolderCount();
        }

        private void lstFolders_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnDelete.Enabled = lstFolders.SelectedIndex > -1;
            PreparetoDisplay();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstFolders.SelectedIndex == -1) return;
            lstFolders.Items.RemoveAt(lstFolders.SelectedIndex);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FileIndexer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!picWorking.Visible)
            {
                SaveConfig();
                return;
            }
            MessageBox.Show("Please wait until the current process finishes", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            e.Cancel = true;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            var ofd = new FolderPicker
            {
                Title = "Select folder to add",
            };
            if (ofd.ShowDialog(IntPtr.Zero) != true) return;

            if (Directory.Exists(ofd.ResultPath) && !lstFolders.Items.Contains(ofd.ResultPath))
            {
                lstFolders.Items.Add(ofd.ResultPath);
            }
            CheckFolderCount();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var message = Tools.ReadHelpFile("fi");
            var help = new HelpForm(Text + " - Help", message);
            help.ShowDialog();
        }

        private void btnBuild_EnabledChanged(object sender, EventArgs e)
        {
            chkSubDirs.Enabled = btnBuild.Enabled;
        }

        private void btnBuild_Click(object sender, EventArgs e)
        {
            if (btnBuild.Text == "Cancel")
            {
                CancelWorkers = true;
                btnBuild.Enabled = false;
                return;
            }
            if (MessageBox.Show("This might take a while depending on how many folders you have selected and how many files are in those folders\nAre you sure you want to do this now?",
                    Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }
            txtSearch.Text = TypeToSearch;
            btnBuild.Text = "Cancel";
            lstSongs.Items.Clear();
            EnableDisable(false);
            PS3Mode = chkPS3.Checked;
            indexingWorker.RunWorkerAsync();
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
            var folder = Path.GetDirectoryName(files[0]);

            if (string.IsNullOrWhiteSpace(folder)) return;
            if (!Directory.Exists(folder) || lstFolders.Items.Contains(folder)) return;
            lstFolders.Items.Add(folder);
            CheckFolderCount();
        }

        private void EnableDisable(bool enabled)
        {
            menuStrip1.Enabled = enabled;
            contextMenuStrip1.Enabled = enabled;
            btnClear.Enabled = enabled;
            btnDelete.Enabled = enabled;
            btnNew.Enabled = enabled;
            chkSubDirs.Enabled = enabled;
            chkPS3.Enabled = enabled;
            txtSearch.Enabled = enabled;
            radioSongs.Enabled = enabled;
            radioPackages.Enabled = enabled;

            Cursor = enabled ? Cursors.Default : Cursors.WaitCursor;
            lstFolders.Cursor = Cursor;
            lstSongs.Cursor = Cursor;

            picWorking.Visible = !enabled;
            
            if (enabled) return;
            btnClearSearch.Enabled = false;
            lblWorking.Visible = true;
            lblWorking.Text = "Working...";
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            CancelWorkers = false;
            PreparetoDisplay();
        }

        private void PreparetoDisplay()
        {
            DisplayIndexedFiles();
            SortSongs();
            EnableDisable(true);
            btnBuild.Text = "Build Index";
            btnBuild.Enabled = true;
            CheckFolderCount();
            EnableSearch(Songs.Any());
        }

        private void DisplayIndexedFiles(bool doFindIDs = false, string filter = "", bool filterpackage = false)
        {
            var workingSongs = doFindIDs ? FilteredSongs : Songs;
            if (!workingSongs.Any()) return;
            var packages = new List<string>();
            lstSongs.Enabled = true;
            lstSongs.Items.Clear();
            lstSongs.ListViewItemSorter = null;
            lstSongs.BeginUpdate();
            foreach (var song in workingSongs)
            {
                if (!string.IsNullOrWhiteSpace(filter))
                {
                    if ((!filterpackage && !song.Name.ToLowerInvariant().Contains(filter)) ||
                         (filterpackage && !song.Location.ToLowerInvariant().Contains(filter))) continue;
                }
                var skip = true;
                for (var i = 0; i < lstFolders.SelectedItems.Count; i++)
                {
                    var folder = lstFolders.SelectedItems[i].ToString();
                    if (song.Location.Contains(folder))
                    {
                        skip = false;
                    }
                }
                if (lstFolders.SelectedItems.Count == 0)
                { 
                    skip = false;
                }
                if (skip) continue;
                var entry = new ListViewItem(song.Name);
                entry.SubItems.Add(song.SongID);
                entry.SubItems.Add(song.Location);
                entry.SubItems.Add(song.InternalName);
                lstSongs.Items.Add(entry);
                if (!packages.Contains(song.Location))
                {
                    packages.Add(song.Location);
                }
            }
            lstSongs.EndUpdate();
            btnBuild.Text = "Rebuild Index";
            lblWorking.Visible = true;
            CountResults(lstSongs.Items.Count, packages.Count);
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            var origins = new List<string>();
            Songs.Clear();
            foreach (var file in lstFolders.Items.Cast<object>().Select(item => item.ToString()).Where(Directory.Exists).Select(
                folder => Directory.GetFiles(folder, "*.*", chkSubDirs.Checked ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)).SelectMany(
                    files => files.Where(file => !Path.GetExtension(file).Equals(".exe") && !Path.GetExtension(file).Equals(".dll"))).TakeWhile(file => !indexingWorker.CancellationPending))
            {
                if (CancelWorkers) return;
                try
                {
                    if (PS3Mode)
                    {
                        if (Path.GetFileName(file) != "songs.dta") continue;
                        if (!Parser.ReadDTA(File.ReadAllBytes(file)) || !Parser.Songs.Any()) continue;                        
                    }
                    else
                    {
                        if (VariousFunctions.ReadFileType(file) != XboxFileType.STFS) continue;
                        if (!Parser.ExtractDTA(file)) continue;
                        if (!Parser.ReadDTA(Parser.DTA) || !Parser.Songs.Any()) continue;
                    }

                    for (var i = 0; i < Parser.Songs.Count; i++)
                    {
                        var dtaSong = Parser.Songs[i];
                        var song = new SongIndex();
                        song.Name = dtaSong.Artist + " - " + dtaSong.Name;
                        song.Location = PS3Mode ? Path.GetDirectoryName(file) + "\\" + dtaSong.InternalName + "\\": file;
                        song.SongID = dtaSong.SongIdString;
                        song.InternalName = dtaSong.InternalName;
                        Songs.Add(song);
                    }
                }
                catch (Exception)
                {}
            }
            SaveIndex();
        }

        private void SaveIndex()
        {
            if (!Songs.Any()) return;
            Tools.DeleteFile(Index);
            var sw = new StreamWriter(Index, false);
            sw.WriteLine("Format2026=True");
            sw.WriteLine("IndexedCount=" + Songs.Count);
            foreach (var song in Songs)
            {
                sw.WriteLine("SongName=" + song.Name);
                sw.WriteLine("SongID=" + song.SongID);
                sw.WriteLine("FileLocation=" + song.Location);
                sw.WriteLine("InternalName=" + song.InternalName);
            }
            sw.Dispose();
        }
        
        private void openFolderThatContainsFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var file = lstSongs.SelectedItems[0].SubItems[2].Text;
            Process.Start("explorer.exe", "/select," + file);
        }
        
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            txtSearch.ForeColor = txtSearch.Text == TypeToSearch ? Color.LightGray : Color.Black;
            if (txtSearch.Text == TypeToSearch || txtSearch.Text == "[FILTER: ID]" || DontUpdate || txtSearch.Text == "[FILTER: FILE]" || txtSearch.Text == "[FILTER: DUPLICATE_NAMES]")
            {
                DontUpdate = false;
                btnClearSearch.Enabled = false;
                return;
            }
            btnClearSearch.Enabled = true;
            DisplayIndexedFiles(false, txtSearch.Text.ToLowerInvariant(), radioPackages.Checked);
            SortSongs();
        }

        private void btnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = TypeToSearch;
            EnableSearch(true);
            DisplayIndexedFiles();
            SortSongs();
        }

        private void CountResults(int count, int packs)
        {
            if (count == 0)
            {
                lblWorking.Text = "No matches found...";
            }
            else
            {
                lblWorking.Text = "Indexed " + count + " " + (count == 1 ? "song" : "songs") + (packs == 1 ? " in 1 file" : " across " + packs + " files");
            }
        }

        private void clearIndexedFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.DeleteFile(Index);
            Songs.Clear();
            lstSongs.Items.Clear();
            EnableSearch(false);
            lblWorking.Text = "";
            btnBuild.Text = "Build Index";
            txtSearch.Text = TypeToSearch;
        }
        
        private void FileIndexer_Resize(object sender, EventArgs e)
        {
            lstSongs.Columns[0].Width = (int)((lstSongs.Width - LIST_SPACER) * 0.4);
            lstSongs.Columns[1].Width = (int)((lstSongs.Width - LIST_SPACER) * 0.15);
            lstSongs.Columns[2].Width = (int)((lstSongs.Width - LIST_SPACER) * 0.35);
        }

        private void FileIndexer_Shown(object sender, EventArgs e)
        {
            if (DoMaximize)
            {
                WindowState = FormWindowState.Maximized;
            }
            Application.DoEvents();
            LoadIndex();
            txtSearch.SelectionStart = 0;
            txtSearch.SelectionLength = 0;
            picWorking.Visible = false;
        }

        private void onlyShowOtherSongs_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "[FILTER: FILE]";
            EnableSearch(false);
            var file = lstSongs.SelectedItems[0].SubItems[2].Text.ToLowerInvariant();
            DisplayIndexedFiles(false, file, true);
            SortSongs();
            btnClearSearch.Enabled = true;
        }
        
        private void txtSearch_MouseClick(object sender, MouseEventArgs e)
        {
            if (txtSearch.Text != TypeToSearch) return;
            DontUpdate = true;
            txtSearch.Text = "";
        }

        private void txtSearch_Leave(object sender, EventArgs e)
        {
            if (txtSearch.Text.Trim() != "") return;
            txtSearch.Text = TypeToSearch;
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            contextMenuStrip1.Enabled = lstSongs.Items.Count > 0;
            var visible = lstSongs.SelectedItems.Count == 1;
            openFolder.Visible = visible;
            onlyShowOtherSongs.Visible = visible;
            exportDisplayedSongs.Visible = visible;
            sendToMenu.Visible = visible && !chkPS3.Checked;
            moveSelectedFiles.Visible = !chkPS3.Checked;
            SendToCONExplorer.Visible = !chkPS3.Checked;
            SendToMIDICleaner.Visible = !chkPS3.Checked;
            SendToSongAnalyzer.Visible = !chkPS3.Checked;
            moveSelectedFiles.Visible = !chkPS3.Checked;
        }

        private void exportDisplayedSongs_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog
            {
                Title = Text,
                InitialDirectory = Tools.CurrentFolder,
                OverwritePrompt = true,
                AddExtension = true,
                FileName = "fileindex.csv",
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
                sw.WriteLine("\"Song Name\",\"Song ID\",\"File Location\"");

                for (var i = 0; i < lstSongs.Items.Count; i++)
                {
                    sw.WriteLine("\"" + lstSongs.Items[i].SubItems[0].Text + "\",\"" + lstSongs.Items[i].SubItems[1].Text + "\",\"" + lstSongs.Items[i].SubItems[2].Text + "\"");
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

        private void SendToCONExplorer_Click(object sender, EventArgs e)
        {
            var handler = new CONExplorer(Color.FromArgb(34, 169, 31), Color.White);
            handler.LoadCON(lstSongs.SelectedItems[0].SubItems[2].Text);
            handler.Show();
        }

        private void SendToVisualizer_Click(object sender, EventArgs e)
        {
            OpenInVisualizer(lstSongs.SelectedItems[0].SubItems[2].Text);
        }

        private void OpenInVisualizer(string file)
        {
            //close old instance
            visualizer.Close();

            //launch new instance
            visualizer = new Visualizer(Color.FromArgb(230, 215, 0), Color.White, file);
            visualizer.Show();
        }

        private void SendToMIDICleaner_Click(object sender, EventArgs e)
        {
            var handler = new MIDICleaner(lstSongs.SelectedItems[0].SubItems[2].Text, Color.FromArgb(230, 215, 0), Color.White);
            handler.Show();
        }

        private void SendToSongAnalyzer_Click(object sender, EventArgs e)
        {
            var handler = new SongAnalyzer(lstSongs.SelectedItems[0].SubItems[2].Text);
            handler.Show();
        }

        private void SendToAudioAnalyzer_Click(object sender, EventArgs e)
        {
            var file = lstSongs.SelectedItems[0].SubItems[2].Text;
            if (chkPS3.Checked)
            {
                var moggs = Directory.GetFiles(file, "*.mogg", SearchOption.AllDirectories);
                if (moggs != null && moggs.Count() > 0)
                {
                    file = moggs[0];
                }
            }
            var handler = new AudioAnalyzer(file);
            handler.Show();
        }
        
        private void SendToQuickPackEditor_Click(object sender, EventArgs e)
        {
            var dta = "";
            var pack = lstSongs.SelectedItems[0].SubItems[2].Text;
            if (chkPS3.Checked)
            {
                var dtas = Directory.GetFiles(lstSongs.SelectedItems[0].SubItems[2].Text, "songs.dta", SearchOption.AllDirectories);
                if (dtas != null  && dtas.Count() > 0)
                {
                    dta = dtas[0];
                    pack = "";
                }
            }
            var handler = new QuickPackEditor(null, Color.FromArgb(34, 169, 31), Color.White, dta, pack);
            handler.ShowDialog();
        }

        private void findSongsWithoutWipeproofIDs_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This might take a while depending on how many songs you have indexed\nAre you sure you want to do this now?",
                    Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                if (txtSearch.Text.Trim() == "[FILTER: WIPE_PROOF_ID]")
                {
                    txtSearch.Text = TypeToSearch;
                }
                return;
            }
            btnBuild.Text = "Cancel";
            txtSearch.Text = "[FILTER: WIPE_PROOF_ID]";
            lstSongs.Items.Clear();
            EnableDisable(false);
            SearchForIDConflicts = false;
            SearchForDuplicates = false;
            filteringWorker.RunWorkerAsync();
        }

        private void filteringWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            FilteredSongs = new List<SongIndex>();
            if (SearchForIDConflicts)
            {
                var IDs = new List<SongIndex>();
                foreach (var song in Songs)
                {
                    if (CancelWorkers) return;
                    foreach (var ID in IDs)
                    {
                        if (CancelWorkers) return;
                        if (!song.SongID.Equals(ID.SongID) || string.IsNullOrEmpty(song.SongID) || 
                            string.IsNullOrEmpty(ID.SongID) || song.SongID.Equals("0") || ID.SongID.Equals("0")) continue;
                        if (!FilteredSongs.Contains(song))
                        {
                            FilteredSongs.Add(song);
                        }
                        if (!FilteredSongs.Contains(ID))
                        {
                            FilteredSongs.Add(ID);
                        }
                    }
                    IDs.Add(song);
                }
            }
           else
            {
                if (SearchForDuplicates)
                {
                    var IDs = new List<SongIndex>();
                    foreach (var song in Songs)
                    {
                        if (CancelWorkers) return;
                        foreach (var ID in IDs)
                        {
                            if (CancelWorkers) return;
                            if (!CleanName(song.Name).Equals(CleanName(ID.Name)) || string.IsNullOrEmpty(CleanName(song.Name)) ||
                                string.IsNullOrEmpty(CleanName(ID.Name))) continue;
                            if (!FilteredSongs.Contains(song))
                            {
                                FilteredSongs.Add(song);
                            }
                            if (!FilteredSongs.Contains(ID))
                            {
                                FilteredSongs.Add(ID);
                            }
                        }
                        IDs.Add(song);
                    }
                }
                else
                {
                    foreach (var song in Songs.Where(song => !Parser.IsNumericID(song.SongID)))
                    {
                        if (CancelWorkers) return;
                        FilteredSongs.Add(song);
                    }
                }
            }            
        }

        private string CleanName(string Name)
        {
            var name = "";

            name = Name.ToLowerInvariant();
            name = name.Replace("(rb3 version)", "");
            name = name.Replace("(2x bass pedal)", "");
            name = name.Replace("featuring", "ft.");
            name = name.Replace("feat.", "ft.");

            return name;
        }

        private void filteringWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (CancelWorkers)
            {
                txtSearch.Text = TypeToSearch;
                DisplayIndexedFiles();
            }
            else
            {
                DisplayIndexedFiles(true);
            }

            if (FilteredSongs.Count == 0)
            {
                MessageBox.Show("Search returned nothing...", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                if (SearchForDuplicates)
                {
                    var message = "Found " + FilteredSongs.Count + " duplicate songs\nSongs are sorted by name and should be grouped together in the list";
                    MessageBox.Show(message, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    var message = "Found " + FilteredSongs.Count + " song" + (FilteredSongs.Count > 1 ? "s" : "") + (SearchForIDConflicts ?
                   " with possible ID conflicts\nSongs are sorted by song ID and should be grouped together in the list" : " without a wipe-proof ID");
                    MessageBox.Show(message, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }            
            }

            btnClearSearch.Enabled = true;
            ActiveSortColumn = SearchForDuplicates? 0 : 1;//sort songs by name or by ID
            SortSongs();
            CancelWorkers = false;
            EnableDisable(true);
            btnBuild.Text = "Build Index";
            btnBuild.Enabled = true;
            CheckFolderCount();
            EnableSearch(txtSearch.Text == TypeToSearch);
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            switch (txtSearch.Text.Trim())
            {
                case "[FILTER: WIPE_PROOF_ID]":
                    findSongsWithoutWipeproofIDs.PerformClick();
                    break;
                case "[FILTER: FILE]":
                    onlyShowOtherSongs_Click(sender, e);
                    break;
                case "[FILTER: ID_CONFLICT]":
                    findSongsWithIDConflicts.PerformClick();
                    break;
            }
            btnClearSearch.Enabled = true;
        }

        private void lstSongs_ColumnClick(object sender, ColumnClickEventArgs e)
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
            if (lstSongs.Items.Count < 2) return;
            lstSongs.BeginUpdate();
            lstSongs.ListViewItemSorter = new ListViewItemComparer(lstSongs, ActiveSortColumn, ListSorting);
            lstSongs.Sort();
            lstSongs.EndUpdate();
        }

        private void findSongsWithIDConflicts_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This might take a while depending on how many songs you have indexed\nAre you sure you want to do this now?",
                    Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                if (txtSearch.Text.Trim() == "[FILTER: ID_CONFLICT]")
                {
                    txtSearch.Text = TypeToSearch;
                }
                return;
            }
            btnBuild.Text = "Cancel";
            txtSearch.Text = "[FILTER: ID_CONFLICT]";
            lstSongs.Items.Clear();
            EnableDisable(false);
            SearchForIDConflicts = true;
            SearchForDuplicates = false;
            filteringWorker.RunWorkerAsync();
        }

        private void deleteSelectedFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Delete " + lstSongs.SelectedItems.Count + " file(s)?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes) return;
            var indices = new List<int>();
            for (var i = 0; i < lstSongs.SelectedItems.Count; i++)
            {
                var file = lstSongs.SelectedItems[i].SubItems[2].Text;
                Tools.SendtoTrash(file, chkPS3.Checked);
                var dtaPath = GetDTAPath(lstSongs.SelectedItems[i].SubItems[2].Text);
                var dtaContents = Parser.ReadDTA(File.ReadAllBytes(dtaPath));
                if (Parser.Songs.Count == 1) //if single song DTA - delete, pack - deal with below
                {
                    Tools.SendtoTrash(dtaPath, false);
                }
                indices.Add(lstSongs.SelectedItems[i].Index);
            }
            if (indices.Count == 0) return;            
            indices.Reverse();
            for (var i = 0; i < indices.Count; i++ )
            {
                if (chkPS3.Checked)
                {
                    var dtaPath = GetDTAPath(lstSongs.Items[indices[i]].SubItems[2].Text);
                    var dtaSongs = Parser.ReadDTA(File.ReadAllBytes(dtaPath));
                    var sw = new StreamWriter(dtaPath, false);
                    for (var x = 0; x < Parser.Songs.Count; x++)
                    {
                        var currID = lstSongs.Items[indices[i]].SubItems[1].Text;
                        var currInternal = lstSongs.Items[indices[i]].SubItems[3].Text;
                        var newID = Parser.Songs[x].SongIdString;
                        var newInternal = Parser.Songs[x].InternalName;

                        //use combination of ID and internal name to match index entry to DTA entry
                        if ((currID + "|" + currInternal).Equals(newID + "|" + newInternal)) continue;
                        for (var j = 0; j < Parser.Songs[x].DTALines.Count; j++)
                        {
                            sw.WriteLine(Parser.Songs[x].DTALines[j]);
                        }
                    }
                    sw.Dispose();
                }
                lstSongs.Items.RemoveAt(indices[i]);
            }
        }

        private void findSongsWithDuplicateNames_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This might take a while depending on how many songs you have indexed\nAre you sure you want to do this now?",
                   Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                if (txtSearch.Text.Trim() == "[FILTER: DUPLICATE_NAMES]")
                {
                    txtSearch.Text = TypeToSearch;
                }
                return;
            }
            btnBuild.Text = "Cancel";
            txtSearch.Text = "[FILTER: DUPLICATE_NAMES]";
            lstSongs.Items.Clear();
            EnableDisable(false);
            SearchForDuplicates = true;
            filteringWorker.RunWorkerAsync();
        }

        private string GetDTAPath(string folder)
        {
            var currentDir = new DirectoryInfo(folder);
            var parentDir = currentDir.Parent.FullName;
            return parentDir + "\\songs.dta";
        }

        private void lstSongs_DoubleClick(object sender, EventArgs e)
        {
            if (!doubleclickToOpenInVisualizer.Checked) return;
            if (chkPS3.Checked)
            {
                OpenInVisualizer(GetDTAPath(lstSongs.SelectedItems[0].SubItems[2].Text));
            }
            else
            {
                OpenInVisualizer(lstSongs.SelectedItems[0].SubItems[2].Text);
            }
        }

        private void moveSelectedFiles_Click(object sender, EventArgs e)
        {
            var ofd = new FolderPicker
            {
                Title = "Select folder to move file(s) to",
            };
            if (ofd.ShowDialog(IntPtr.Zero) != true) return;
            if (!Directory.Exists(ofd.ResultPath)) return;

            var result = MessageBox.Show("Move " + lstSongs.SelectedItems.Count + " file(s)?", "Confirm Move", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes) return;

            var indices = new List<int>();
            for (var i = 0; i < lstSongs.SelectedItems.Count; i++)
            {
                var file = lstSongs.SelectedItems[i].SubItems[2].Text;
                Tools.MoveFile(file, ofd.ResultPath + "\\" + Path.GetFileName(file));
                var sourceFolder = Path.GetDirectoryName(file);
                if (string.Equals(ofd.ResultPath, sourceFolder)) continue;//skip "moving" since not necessary
                indices.Add(lstSongs.SelectedItems[i].Index);             
            }
            if (!lstFolders.Items.Contains(ofd.ResultPath))
            {
                lstFolders.Items.Add(ofd.ResultPath);
            }
            if (indices.Count == 0) return;
            indices.Reverse();
            for (var i = 0; i < indices.Count; i++)
            {
                lstSongs.Items.RemoveAt(indices[i]);
            }
            MessageBox.Show("File(s) moved ... you need to rebuild the Index now", "Need to Rebuild", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void exportToJson_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog
            {
                Title = Text,
                InitialDirectory = Tools.CurrentFolder,
                OverwritePrompt = true,
                AddExtension = true,
                FileName = "fileindex.json",
                Filter = "Json Files (*.json)|*json"
            };

            if (sfd.ShowDialog() != DialogResult.OK || string.IsNullOrWhiteSpace(sfd.FileName)) return;
            Tools.CurrentFolder = Environment.CurrentDirectory;
            var filename = sfd.FileName;
            if (string.IsNullOrEmpty(Path.GetExtension(filename)))
            {
                filename += ".json";
            }
            try
            {
                List<SongIndex> Songs = new List<SongIndex>();
                for (var i = 0; i < lstSongs.Items.Count; i++)
                {
                    var song = new SongIndex { Name = lstSongs.Items[i].SubItems[0].Text, SongID = lstSongs.Items[i].SubItems[1].Text, Location = lstSongs.Items[i].SubItems[2].Text };
                    Songs.Add(song);
                }

                var value = JsonConvert.SerializeObject(Songs, Formatting.None);
                var sw = new StreamWriter(filename, false);
                sw.WriteLine(value);
                sw.Dispose();

                if (File.Exists(filename))
                {
                    MessageBox.Show("Exported to Json successfully", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Exporting to Json failed", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting to Json:\n" + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            } 
        }
    }

    public class SongIndex
    {
        public string Name { get; set; }
        public string SongID { get; set; }
        public string Location { get; set; }
        public string InternalName { get; set; }
    }
}
