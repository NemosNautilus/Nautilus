using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Nautilus.Properties;
using Nautilus.x360;
using DevComponents.AdvTree;
using Color = System.Drawing.Color;
using NautilusFREE;

namespace Nautilus
{
    public partial class CONExplorer : Form
    {
        private Boolean FailedSign;
        private string outFolder;
        private string folder;
        private bool starting;
        private bool internalDrop;
        private bool Changes;
        private readonly NemoTools Tools;
        private readonly DTAParser Parser;
        private FileEntry activeDTA;
        private FileEntry activeMOGG;
        private FileEntry activePNG;
        private FileEntry activeMIDI;
        private FileEntry activeUPG;
        private FileEntry activeMILO;
        private readonly bool isRunningShortcut;
        private static Color mMenuBackground;
        private nTools nautilus3;
        private bool doInject;
        private PackageType filePackageType;
        const string AppName = "CON Explorer";
        private string internalName;
        private string shortName;

        public CONExplorer(Color ButtonBackColor, Color ButtonTextColor, bool runningshortcut = false)
        {
            CheckForIllegalCrossThreadCalls = true;
            InitializeComponent();
            InitializeSearchBox();
            mMenuBackground = contextMenuStrip1.BackColor;
            contextMenuStrip1.Renderer = new DarkRenderer();
            nautilus3 = new nTools();

            picContent.AllowDrop = true;
            picPackage.AllowDrop = true;
            btnDTA.AllowDrop = true;
            btnMIDI.AllowDrop = true;
            btnMOGG.AllowDrop = true;
            btnPNG.AllowDrop = true;
            btnMILO.AllowDrop = true;

            Tools = new NemoTools();
            Parser = new DTAParser();
            isRunningShortcut = runningshortcut;

            var formButtons = new List<Button> { btnExtract,btnSave,btnVisualize, btnChange };
            foreach (var button in formButtons)
            {
                button.BackColor = ButtonBackColor;
                button.ForeColor = ButtonTextColor;
                button.FlatAppearance.MouseOverBackColor = button.BackColor == Color.Transparent ? Color.FromArgb(127, Color.AliceBlue.R, Color.AliceBlue.G, Color.AliceBlue.B) : Tools.LightenColor(button.BackColor);
            }

            toolTip1.SetToolTip(picContent, "Click here to select the Content Image (visible in here)");
            toolTip1.SetToolTip(picPackage, "Click here to select the Package Image (visible in the Xbox dashboard)");            
            toolTip1.SetToolTip(btnExtract, "Click to extract the whole CON file");
            toolTip1.SetToolTip(btnSave, "Click to save changes to file");
            toolTip1.SetToolTip(txtTitle, "Package Title (optional)");
            toolTip1.SetToolTip(txtDescription, "Package Description (optional)");
            toolTip1.SetToolTip(btnVisualize, "Click to open in Visualizer");
        }

        private STFSPackage xPackage { get; set; }

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

        private TextBox txtSearch;
        private Node lastClickedNode;

        private void InitializeSearchBox()
        {
            txtSearch = new TextBox
            {
                Location = new Point(6, 6),
                Size = new Size(281, 20),
                Name = "txtSearch",
                TabIndex = 46
            };
            cboExtension = new ComboBox
            {
                Location = new Point(292, 5),
                Size = new Size(100, 21),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Name = "cboExtension",
                TabIndex = 47
            };
            cboExtension.Items.Add(AllExtensions);
            cboExtension.SelectedIndex = 0;
            cboExtension.SelectedIndexChanged += (s, e) => RunSearch();
            tabContents.Controls.Add(cboExtension);

            //checkboxes mark files for the copy list; checks persist across searches/folders
            fileList.CheckBoxes = true;
            fileList.ItemChecked += (s, e) =>
            {
                if (suppressCheck) return;
                var f = e.Item.Tag as FileEntry;
                if (f == null) return;
                if (e.Item.Checked) { if (!copyList.Contains(f)) copyList.Add(f); }
                else copyList.Remove(f);
            };
            toolTip1.SetToolTip(cboExtension, "Filter by file extension");

            //draw a greyed-out Information tab when it's disabled (multi-file mode)
            tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl1.DrawItem += TabControl1_DrawItem;
            txtSearch.TextChanged += (s, e) => QueueSearch();
            txtSearch.KeyDown += (s, e) =>
            {
                if (e.KeyCode != Keys.Escape) return;
                ClearFilters();
                xReturn_NodeClick(lastClickedNode ?? node1, null);
                e.SuppressKeyPress = true;
            };
            tabContents.Controls.Add(txtSearch);
            folderTree.Location = new Point(6, 30);
            folderTree.Size = new Size(386, 138);
            SetSearchCue();
            MakeResizable();
        }

        private void MakeResizable()
        {
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            MinimumSize = Size; //designer size is the smallest layout that fits

            const AnchorStyles all = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl1.Anchor = all;
            lstLog.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            foreach (Control c in new Control[] { label5, btnDTA, btnMIDI, btnPNG, btnMOGG, btnMILO })
            {
                c.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            }

            //contents tab: tree takes ~55% of the height, file list the rest
            txtSearch.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cboExtension.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            folderTree.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            fileList.Anchor = all;
            tabContents.Resize += (s, e) => LayoutContents();

            //Name column fills the width left over by Size
            fileList.Resize += (s, e) =>
            {
                var w = fileList.ClientSize.Width - columnHeader2.Width;
                if (w > 50) columnHeader1.Width = w;
            };
        }

        private void LayoutContents()
        {
            const int gap = 6;
            var top = folderTree.Top;
            var available = tabContents.ClientSize.Height - top - gap * 2;
            if (available < 100) return;
            var treeHeight = available * 55 / 100;
            folderTree.Height = treeHeight;
            fileList.Top = top + treeHeight + gap;
            fileList.Height = tabContents.ClientSize.Height - fileList.Top - gap;
        }

        private const string AllExtensions = "All types";
        private const string NoExtension = "(none)";
        private ComboBox cboExtension;

        private void TabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            var page = tabControl1.TabPages[e.Index];
            var selected = e.Index == tabControl1.SelectedIndex;
            var back = selected ? page.BackColor : SystemColors.Control;
            using (var brush = new SolidBrush(back))
            {
                e.Graphics.FillRectangle(brush, e.Bounds);
            }
            var color = page.Enabled ? SystemColors.ControlText : SystemColors.GrayText;
            TextRenderer.DrawText(e.Graphics, page.Text, tabControl1.Font, e.Bounds, color,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        private static string GetExtension(string name)
        {
            var ext = Path.GetExtension(name);
            return string.IsNullOrEmpty(ext) ? NoExtension : ext.ToLowerInvariant();
        }

        private void PopulateExtensions()
        {
            var current = cboExtension.SelectedItem as string ?? AllExtensions;
            var exts = GetSearchIndex().Select(x => x.Extension).Distinct()
                .OrderBy(x => x == NoExtension ? 1 : 0).ThenBy(x => x).ToList();
            cboExtension.BeginUpdate();
            cboExtension.Items.Clear();
            cboExtension.Items.Add(AllExtensions);
            foreach (var ext in exts) cboExtension.Items.Add(ext);
            var idx = cboExtension.Items.IndexOf(current);
            suppressSearch = true;
            cboExtension.SelectedIndex = idx < 0 ? 0 : idx;
            suppressSearch = false;
            cboExtension.EndUpdate();
        }

        private bool suppressSearch;
        private bool suppressCheck;
        private ToolStripMenuItem menuCopyFile, menuAddToCopyList, menuCopyList, menuClearCopyList;

        private bool IsFiltering
        {
            get
            {
                return txtSearch != null && (txtSearch.TextLength > 0 ||
                       (cboExtension.SelectedItem as string ?? AllExtensions) != AllExtensions);
            }
        }

        private void ClearFilters()
        {
            suppressSearch = true;
            txtSearch.Clear();
            cboExtension.SelectedIndex = 0;
            suppressSearch = false;
            if (searchTimer != null) searchTimer.Stop();
        }

        private void SetSearchCue()
        {
            //native placeholder text
            const int EM_SETCUEBANNER = 0x1501;
            if (txtSearch.IsHandleCreated)
                SendMessage(txtSearch.Handle, EM_SETCUEBANNER, (IntPtr)1, "Search files and folders...");
            else
                txtSearch.HandleCreated += (s, e) => SetSearchCue();
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);

        private const int MaxSearchResults = 500;
        private Timer searchTimer;
        private List<SearchEntry> searchIndex;

        private sealed class SearchEntry
        {
            public string Path;
            public string Size;
            public FileEntry File;
            public string Extension;
        }

        private void QueueSearch()
        {
            //debounce: wait until the user pauses typing
            if (suppressSearch) return;
            if (searchTimer == null)
            {
                searchTimer = new Timer { Interval = 250 };
                searchTimer.Tick += (s, e) => { searchTimer.Stop(); RunSearch(); };
            }
            searchTimer.Stop();
            searchTimer.Start();
        }

        /// <summary>
        /// Flat in-memory list of every file path, built once per load (or after edits).
        /// Uses the package's directory lists directly; GetSubFiles/GetSubFolders scan the
        /// whole list per call, which made the recursive search quadratic.
        /// </summary>
        private List<SearchEntry> GetSearchIndex()
        {
            if (searchIndex != null) return searchIndex;
            searchIndex = new List<SearchEntry>();
            var packages = IsMultiMode ? multiPackages.OrderBy(p => Path.GetFileName(p.FileNameLong)).ToList()
                                       : new List<STFSPackage> { xPackage };
            foreach (var pkg in packages)
            {
                var prefix = IsMultiMode ? Path.GetFileName(pkg.FileNameLong) + ": " : "";
                var folders = new Dictionary<ushort, FolderEntry>();
                foreach (var f in pkg.xFolderDirectory) folders[f.EntryID] = f;
                var folderPaths = new Dictionary<ushort, string>();
                Func<ushort, string> pathOf = null;
                pathOf = id =>
                {
                    string p;
                    if (folderPaths.TryGetValue(id, out p)) return p;
                    FolderEntry f;
                    p = folders.TryGetValue(id, out f) && f.FolderPointer != id
                        ? pathOf(f.FolderPointer) + f.Name + "/"
                        : ""; //root
                    folderPaths[id] = p;
                    return p;
                };
                var entries = pkg.xFileDirectory
                    .Select(f => new SearchEntry { Path = prefix + pathOf(f.FolderPointer) + f.Name, Size = FormatSize(f.Size), File = f, Extension = GetExtension(f.Name) })
                    .OrderBy(x => x.Path, StringComparer.OrdinalIgnoreCase);
                searchIndex.AddRange(entries);
            }
            return searchIndex;
        }

        private void RunSearch()
        {
            if (searchTimer != null) searchTimer.Stop();
            if (suppressSearch) return;
            if (xPackage == null || !xPackage.ParseSuccess) return;
            var term = txtSearch.Text.Trim();
            var ext = cboExtension.SelectedItem as string;
            var anyExt = ext == null || ext == AllExtensions;
            if (term.Length == 0 && anyExt)
            {
                xReturn_NodeClick(lastClickedNode ?? node1, null);
                return;
            }
            var index = GetSearchIndex();
            var checkedSet = new HashSet<FileEntry>(copyList);
            var items = new List<ListViewItem>(MaxSearchResults + copyList.Count);

            //checked files stay listed (on top) across searches; unchecked ones drop off on the next search
            var byFile = new Dictionary<FileEntry, SearchEntry>();
            foreach (var x in index) byFile[x.File] = x;
            foreach (var f in copyList)
            {
                SearchEntry entry;
                if (!byFile.TryGetValue(f, out entry)) continue;
                var item = new ListViewItem(entry.Path) { Checked = true, Tag = f };
                item.SubItems.Add(entry.Size);
                items.Add(item);
            }

            var matches = index.Where(x =>
                !checkedSet.Contains(x.File) &&
                (anyExt || x.Extension == ext) &&
                (term.Length == 0 || x.Path.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0));
            var total = 0;
            var shown = 0;
            foreach (var m in matches)
            {
                total++;
                if (shown >= MaxSearchResults) continue;
                var item = new ListViewItem(m.Path) { Tag = m.File };
                item.SubItems.Add(m.Size);
                items.Add(item);
                shown++;
            }
            fileList.BeginUpdate();
            fileList.Items.Clear();
            suppressCheck = true;
            fileList.Items.AddRange(items.ToArray());
            suppressCheck = false;
            fileList.EndUpdate();
            if (total > MaxSearchResults)
            {
                Log("Showing first " + MaxSearchResults + " of " + total + " matches, refine your search");
            }
        }

        private static string FormatSize(long bytes)
        {
            if (bytes < 1024) return "1 KB";
            if (bytes < 1048576) return decimal.Round((decimal)bytes / 1024, 2) + " KB";
            return decimal.Round((decimal)bytes / 1048576, 2) + " MB";
        }

        private void SetNodes()
        {
            if (txtSearch != null) ClearFilters();
            searchIndex = null;
            lastClickedNode = null;
            node1.Nodes.Clear();
            fileList.Items.Clear();
            node1.DataKey = xPackage.RootDirectory;
            var x = xPackage.RootDirectory.GetSubFolders();
            foreach (var y in x)
            {
                node1.Nodes.Add(GetNode(y));
            }
            xReturn_NodeClick(node1, null);
            PopulateExtensions();
        }

        private Node GetNode(ItemEntry x)
        {
            var xReturn = new Node { Text = x.Name, DataKey = x };
            xReturn.NodeClick += xReturn_NodeClick;
            return xReturn;
        }

        private void xReturn_NodeClick(object sender, EventArgs e)
        {
            if (!xPackage.ParseSuccess)
            {
                MessageBox.Show("There was an error.\nTry closing and reopening the file.", AppName,
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            var xsender = (Node)sender;
            if (IsMultiMode && xsender == node1)
            {
                //root lists the loaded packages only
                lastClickedNode = node1;
                if (IsFiltering)
                {
                    if (e == null) { RunSearch(); return; }
                    ClearFilters();
                }
                fileList.Items.Clear();
                return;
            }
            var x = (FolderEntry)xsender.DataKey;
            if (IsMultiMode && x != null)
            {
                xPackage = x.xPackage; //operate on the package this folder belongs to
            }
            var xFiles = x.GetSubFiles();
            var xFolders = x.GetSubFolders();

            if (x == null || xFiles == null || xFolders == null)
            {
                return;
            }
            lastClickedNode = xsender;
            if (IsFiltering)
            {
                if (e == null)
                {
                    RunSearch(); //refresh after add/replace/delete while searching
                    return;
                }
                ClearFilters(); //user clicked a folder: drop the search and show it
            }
            fileList.Items.Clear();

            foreach (var y in xFiles)
            {
                var z = new ListViewItem(y.Name);
                z.SubItems.Add(FormatSize(y.Size));
                z.Tag = y;
                z.Checked = copyList.Contains(y);
                suppressCheck = true;
                fileList.Items.Add(z);
                suppressCheck = false;
            }
            xsender.Nodes.Clear();

            //sort subfolders
            var xfolders = xFolders.Select(f => f.Name).ToList();
            xfolders.Sort();

            foreach (var f in xfolders)
            {
                for (var i = 0; i < xfolders.Count; i++)
                {
                    if (xFolders[i].Name != f) continue;
                    var node = GetNode(xFolders[i]);
                    node.Tag = xFolders[i];
                    xsender.Nodes.Add(node);
                    //xsender.Nodes.Add(GetNode(xFolders[i]));
                    //xsender.Nodes[i].Tag = xFolders[i];
                }
            }
        }

        private List<STFSPackage> multiPackages;
        private bool IsMultiMode { get { return multiPackages != null; } }

        /// <summary>
        /// Loads several CON/LIVE files into one read-only window: each package becomes a
        /// node under Root in the Contents tab, and the Information tab is disabled.
        /// </summary>
        public void LoadMultipleCON(IList<string> files)
        {
            if (files.Count == 1)
            {
                LoadCON(files[0]);
                return;
            }
            starting = true;
            multiPackages = new List<STFSPackage>();
            tabControl1.Enabled = false;
            foreach (var file in files)
            {
                STFSPackage pkg;
                try
                {
                    pkg = new STFSPackage(file);
                }
                catch (Exception)
                {
                    pkg = null;
                }
                if (pkg == null || !pkg.ParseSuccess)
                {
                    Log("Error opening " + Path.GetFileName(file));
                    if (pkg != null) pkg.CloseIO();
                    continue;
                }
                multiPackages.Add(pkg);
                Log("Opened " + Path.GetFileName(file));
            }
            if (multiPackages.Count == 0)
            {
                Log("No files could be opened");
                multiPackages = null;
                return;
            }
            xPackage = multiPackages[0];

            node1.NodeClick += xReturn_NodeClick;
            node1.Nodes.Clear();
            fileList.Items.Clear();
            node1.DataKey = null;
            foreach (var pkg in multiPackages.OrderBy(p => Path.GetFileName(p.FileNameLong)))
            {
                var pkgNode = new Node { Text = Path.GetFileName(pkg.FileNameLong), DataKey = pkg.RootDirectory };
                pkgNode.NodeClick += xReturn_NodeClick;
                node1.Nodes.Add(pkgNode);
            }
            node1.Expand();
            PopulateExtensions();

            //Information tab only makes sense for a single package
            tabInformation.Enabled = false;
            tabControl1.SelectedTab = tabContents;
            tabControl1.Invalidate();
            tabControl1.Selecting += (s, ev) =>
            {
                if (ev.TabPage == tabInformation) ev.Cancel = true;
            };
            menuRenameFolder.Enabled = false;

            Text = multiPackages.Count + " files loaded";
            Log("Loaded " + multiPackages.Count + " files (read-only)");
            Log("Ready");
            tabControl1.Enabled = true;
            starting = false;
        }

        public void LoadCON(string file)
        {
            starting = true;
            if (VariousFunctions.ReadFileType(file) != XboxFileType.STFS)
            {
                MessageBox.Show("That's not a valid file ... try again.", AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            xPackage = new STFSPackage(file);
            if (!xPackage.ParseSuccess)
            {
                Log("Error opening file");
                return;
            }
            
            tabControl1.Enabled = false;
            Log("File opened successfully");
            Log("Processing");

            //populate treeview and listview with package contents
            node1.NodeClick += xReturn_NodeClick;
            SetNodes();
            node1.SetChecked(true, eTreeAction.Expand);
            Log("Finished processing the contents");
            Log("Ready");

            //populate main form with package info
            folder = file;
            Text = Path.GetFileName(file);
            toolTip1.SetToolTip(chkAutoExtract, "File will be extracted to " + file + "_extracted\\");
            txtTitle.Text = Tools.FixBadChars(xPackage.Header.Title_Display);
            txtDescription.Text = Tools.FixBadChars(xPackage.Header.Description);
            picContent.Image = xPackage.Header.ContentImage;
            picPackage.Image = xPackage.Header.PackageImage;

            switch (xPackage.Header.TitleID)
            {
                case 0x45410829:
                    cboGameID.SelectedIndex = 0; //RB1
                    break;
                case 0x45410869:
                    cboGameID.SelectedIndex = 1; //RB2
                    break;
                case 0x454108B1:
                case (uint)4294838225:
                    cboGameID.SelectedIndex = 2; //TBRB
                    break;
                case 0x575207F0:
                    cboGameID.SelectedIndex = 3; //LRB
                    break;
                case 0x454108E7:
                    cboGameID.SelectedIndex = 4; //GDRB
                    break;
                case 0x45410914:
                    cboGameID.SelectedIndex = 5; //RB3
                    break;
                case 0x415607E7:
                    cboGameID.SelectedIndex = 6; //GH2
                    break;
                case 0x415607F7:
                    cboGameID.SelectedIndex = 7; //GH3
                    break;
                case 0x4156081A:
                    cboGameID.SelectedIndex = 8; //GH4WT
                    break;
                case 0x41560840:
                    cboGameID.SelectedIndex = 9; //GH5
                    break;
                case 0x41560883:
                    cboGameID.SelectedIndex = 10; //GH6WOR
                    break;
                case 0x545607D3:
                    cboGameID.SelectedIndex = 11; //Dance Central 1
                    break;
                case 0x373307D2:
                    cboGameID.SelectedIndex = 12; //Dance Central 2
                    break;
                case 0x373307D9:
                    cboGameID.SelectedIndex = 13; //Dance Central 3
                    break;
                case 0x41560844:
                    cboGameID.SelectedIndex = 14; //DJ Hero
                    break;
                case 0x4156087F:
                    cboGameID.SelectedIndex = 15; //DJ Hero 2
                    break;
                case 0x5841122D:
                    cboGameID.SelectedIndex = 16; //Rock Band Blitz
                    break;
                default:
                    cboGameID.Items.Add("Unknown - " + xPackage.Header.TitleID);
                    cboGameID.SelectedIndex = 16;
                    break;
            }

            filePackageType = xPackage.Header.ThisType;
            switch (filePackageType)
            {
                case PackageType.SavedGame:
                    cboFormat.SelectedIndex = 0;
                    break;
                case PackageType.MarketPlace:
                    cboFormat.SelectedIndex = 1;
                    break;
                case PackageType.Installer:
                    cboFormat.SelectedIndex = 2;
                    break;
                default:
                    cboFormat.SelectedIndex = 3;
                    cboFormat.Items[3] = "Other - " + filePackageType.ToString();
                    break;
            }            
                        
            tabControl1.Enabled = true;
            starting = false;
            ShowChanges(false);
        }

        private void extractFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileList.SelectedIndices.Count == 0)
            {
                return;
            }

            var savedir = Path.GetDirectoryName(xPackage.FileNameLong);
            for (var i = 0; i < fileList.SelectedItems.Count; i++)
            {
                var xent = (FileEntry)fileList.SelectedItems[i].Tag;
                if (xent == null)
                {
                    Log("Error extracting " + xent.Name);
                    Enabled = true;
                    return;
                }

                var extension = Path.GetExtension(xent.Name);
                if (extension != "")
                {
                    extension = extension.Substring(1).ToUpper() + " Files|*" + extension;
                }

                var sfd = new SaveFileDialog
                {
                    Filter = extension,
                    InitialDirectory = savedir,
                    FileName = Path.GetFileNameWithoutExtension(xent.Name)
                };
                if (sfd.ShowDialog() != DialogResult.OK || string.IsNullOrWhiteSpace(sfd.FileName))
                {
                    Log("Extracting cancelled by user");
                    Enabled = true;
                    return;
                }
                savedir = Path.GetDirectoryName(sfd.FileName);
                Tools.CurrentFolder = savedir;
                Enabled = false;
                Log("Extracting " + xent.Name);
                if (!xent.ExtractToFile(sfd.FileName))
                {
                    Log("Error extracting " + xent.Name);
                    Enabled = true;
                    return;
                }                
                
                Log("File " + xent.Name + " extracted successfully");
            }
            Enabled = true;
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            extractFileToolStripMenuItem.Text = "Extract selected file";
            if (menuCopyFile == null)
            {
                menuCopyFile = new ToolStripMenuItem { BackColor = extractFileToolStripMenuItem.BackColor };
                menuCopyFile.Click += (s, ev) => CopySelectedFiles();
                var back = extractFileToolStripMenuItem.BackColor;
                menuAddToCopyList = new ToolStripMenuItem("Check selected") { BackColor = back };
                menuAddToCopyList.Click += (s, ev) => CheckSelected(true);
                menuCopyList = new ToolStripMenuItem { BackColor = back };
                menuCopyList.Click += (s, ev) => CopyFiles(copyList.ToList());
                menuClearCopyList = new ToolStripMenuItem("Uncheck all") { BackColor = back };
                menuClearCopyList.Click += (s, ev) => ClearCopyList();
                var at = contextMenuStrip1.Items.IndexOf(extractFileToolStripMenuItem) + 1;
                contextMenuStrip1.Items.Insert(at++, menuCopyFile);
                contextMenuStrip1.Items.Insert(at++, menuAddToCopyList);
                contextMenuStrip1.Items.Insert(at++, menuCopyList);
                contextMenuStrip1.Items.Insert(at, menuClearCopyList);
            }
            menuCopyFile.Enabled = fileList.SelectedIndices.Count > 0;
            menuAddToCopyList.Enabled = fileList.SelectedIndices.Count > 0;
            menuAddToCopyList.Visible = fileList.SelectedIndices.Count > 1;
            menuCopyList.Text = "Copy checked files (" + copyList.Count + ")";
            menuCopyList.Visible = copyList.Count > 0;
            menuClearCopyList.Visible = copyList.Count > 0;
            menuCopyFile.Text = fileList.SelectedIndices.Count > 1 ? "Copy selected files" : "Copy";
            if (IsMultiMode)
            {
                //read-only: only extraction is allowed
                replaceFileToolStripMenuItem.Visible = false;
                injectSelectedFileToolStripMenuItem.Visible = false;
                menuRenameFile.Visible = false;
                menuDeleteFile.Visible = false;
                toolStripMenuItem1.Visible = false;
                addNewFilesToolStripMenuItem.Visible = false;
                extractFileToolStripMenuItem.Enabled = fileList.SelectedIndices.Count > 0;
                if (fileList.SelectedIndices.Count > 1) extractFileToolStripMenuItem.Text = "Extract selected files";
                return;
            }

            if (fileList.SelectedIndices.Count == 0)
            {
                extractFileToolStripMenuItem.Enabled = false;
                replaceFileToolStripMenuItem.Enabled = false;
                menuRenameFile.Enabled = false;
                return;
            }

            extractFileToolStripMenuItem.Enabled = true;
            replaceFileToolStripMenuItem.Enabled = true;
            menuRenameFile.Enabled = true;

            if (fileList.SelectedIndices.Count == 1)
            {
                replaceFileToolStripMenuItem.Visible = true;
                menuRenameFile.Visible = true;
            }
            else if (fileList.SelectedIndices.Count > 1)
            {
                extractFileToolStripMenuItem.Text = "Extract selected files";
                replaceFileToolStripMenuItem.Visible = false;
                menuDeleteFile.Visible = false;
                menuRenameFile.Visible = false;
            }
        }

        private void EnableDisable(bool enabled)
        {
            txtTitle.Enabled = enabled;
            picPackage.Enabled = enabled;
            picContent.Enabled = enabled;
            txtDescription.Enabled = enabled;
            btnExtract.Enabled = enabled;
            btnSave.Enabled = enabled;
            chkAutoExtract.Enabled = enabled;
            btnVisualize.Enabled = enabled;
            cboGameID.Enabled = enabled;
            contextMenuStrip1.Enabled = enabled;
            contextMenuStrip2.Enabled = enabled;
            label4.Visible = enabled;            
            folderTree.Enabled = enabled;
            fileList.Enabled = enabled;
            picWorking.Visible = !enabled;
        }

        private void replaceFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doInject = false;
            InjectReplaceFile();
        }

        private void InjectReplaceFile()
        {
            if (fileList.SelectedIndices.Count == 0)
            {
                return;
            }
            var xEnt = (FileEntry)fileList.SelectedItems[0].Tag;
            var extension = Path.GetExtension(fileList.SelectedItems[0].Text);
            if (extension != null)
            {
                var filterFiles = extension.Substring(1).ToUpper() + " File|*." + extension.Substring(1);
                var file = VariousFunctions.GetUserFileLocale("Open a File", filterFiles, true);
                if (file == null)
                {
                    return;
                }
                Log((doInject ? "Injecting" : "Replacing")+ " file " + xEnt.Name);
                var obf_file = Path.GetTempPath() + "m";
                if (Path.GetExtension(file) == ".mogg")
                {
                    Tools.DeleteFile(obf_file);
                    File.Copy(file, obf_file);
                    nautilus3.WriteOutData(nautilus3.DeObfM(File.ReadAllBytes(obf_file)), obf_file);
                }
                else
                {
                    obf_file = file;
                }
                if (doInject)
                {
                    if (!xEnt.Inject(obf_file))
                    {
                        Log("Error injecting file " + xEnt.Name);
                        return;
                    }
                }
                else
                {
                    if (!xEnt.Replace(obf_file))
                    {
                        Log("Error replacing file " + xEnt.Name);
                        return;
                    }
                }
                Tools.CurrentFolder = Path.GetDirectoryName(file);
            }
            Log((doInject ? "Injected" : "Replaced") + " file " + xEnt.Name + " successfully");
            ShowChanges(true);
        }

        private void ShowChanges(bool show)
        {
            Changes = show;
            if (show && txtSearch != null)
            {
                searchIndex = null; //contents changed, rebuild index and extension list
                PopulateExtensions();
            }
            Text =Text.Replace("*", "").Trim();
            if (show)
            {
                Text = "* " + Text + " *";
            }
        }
        
        private void DeleteFile()
        {
            try
            {
                var x = (FileEntry)fileList.SelectedItems[0].Tag;
                xPackage.xDeleteEntry(x);
                copyList.Remove(x);
                Log("Deleted file " + x.Name + " successfully");
                fileList.SelectedItems[0].Remove();
                ShowChanges(true);
            }
            catch
            {
                Log("Error deleting that file");
            }
        }

        private void AddFiles(IEnumerable<string> files)
        {
            var existing = false;
            foreach (var file in files)
            {
                if (string.IsNullOrWhiteSpace(file))
                {
                    return;
                }
                for (var i = 0; i < fileList.Items.Count; i++)
                {
                    if (!String.Equals(fileList.Items[i].Text, Path.GetFileName(file), StringComparison.InvariantCultureIgnoreCase)) continue;
                    MessageBox.Show("There is already a file with the name " + Path.GetFileName(file) +
                        "!\nIf you want to overwrite that file, right click on it and choose\n'Replace selected file' or 'Inject selected file'",
                        AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    existing = true;
                }

                if (existing) continue;
                if (Path.GetExtension(file) == ".mogg")
                {
                    nautilus3.WriteOutData(nautilus3.DeObfM(File.ReadAllBytes(file)), file);
                }
                var x = new DJsIO(file, DJFileMode.Open, true);
                if (!x.Accessed)
                {
                    return;
                }
                xPackage.MakeFile(x.FileNameShort, x, ((FolderEntry)folderTree.SelectedNode.DataKey).EntryID, AddType.NoOverWrite);
                x.Dispose();
                xReturn_NodeClick(folderTree.SelectedNode, null);
                Log("Added file " + x.FileNameShort + " successfully");
                ShowChanges(true);
            }
        }

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            if (picWorking.Visible) return;
            if (IsMultiMode)
            {
                internalDrop = false;
                return;
            }
            if (internalDrop)
            {
                internalDrop = false;
                return;
            }

            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            Tools.CurrentFolder = Path.GetDirectoryName(files[0]);
            
            if (fileList.SelectedIndices.Count == 0)
            {
                AddFiles(files);
                return;
            }

            if (Path.GetExtension(fileList.SelectedItems[0].Text) == Path.GetExtension(files[0]))
            {
                if (MessageBox.Show("Replace '" + fileList.SelectedItems[0].Text + "' with '" + Path.GetFileName(files[0]) + "'?",
                    "Replace file?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                Log("Replacing file " + fileList.SelectedItems[0].Text);
                var xent = (FileEntry)fileList.SelectedItems[0].Tag;

                var obf_file = Path.GetTempPath() + "m";
                if (Path.GetExtension(files[0]) == ".mogg")
                {
                    Tools.DeleteFile(obf_file);
                    File.Copy(files[0], obf_file);
                    nautilus3.WriteOutData(nautilus3.DeObfM(File.ReadAllBytes(obf_file)), obf_file);
                }
                else
                {
                    obf_file = files[0];
                }
                if (!xent.Replace(obf_file))
                {
                    Log("Error replacing file " + fileList.SelectedItems[0].Text);
                    return;
                }
                Log("Replaced file " + fileList.SelectedItems[0].Text + " successfully");
                ShowChanges(true);
            }
            else
            {
                MessageBox.Show("Can't replace a '" + Path.GetExtension(fileList.SelectedItems[0].Text) + "' file with a '" + Path.GetExtension(files[0]) + "' file!", AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void textBoxX2_TextChanged(object sender, EventArgs e)
        {
            xPackage.Header.Title_Display = txtTitle.Text;
            ShowChanges(true);
        }

        private void textBoxX3_TextChanged(object sender, EventArgs e)
        {            
            xPackage.Header.Description = txtDescription.Text;
            ShowChanges(true);
        }

        private void getImage(String file, PictureBox box)
        {
            if (picWorking.Visible) return;

            try
            {
                string contentImage;

                //if not passed a string path for the image
                //show dialog box to find one
                if (string.IsNullOrWhiteSpace(file))
                {
                    var openFileDialog1 = new OpenFileDialog
                    {
                        Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png",
                        Title = "Select a Package image",
                        InitialDirectory = Application.StartupPath + "\\res\\thumbs"
                    };
                    if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
                    contentImage = openFileDialog1.FileName;
                    Tools.CurrentFolder = Path.GetDirectoryName(openFileDialog1.FileName);
                }
                else
                {
                    //if file is not blank, then use that for the image

                    if ((file.Contains(".jpg") || file.Contains(".bmp") ||
                        file.Contains(".png") || file.Contains(".jpeg")) && !file.Contains(".png_xbox") && !file.Contains(".png_wii"))
                    {
                        contentImage = file;
                    }
                    else
                    {
                        Log("That's not a valid image file");
                        return;
                    }
                }

                if (string.IsNullOrWhiteSpace(contentImage)) return;

                var thumbnail = Tools.NemoLoadImage(contentImage);
                if (thumbnail.Width == 64 && thumbnail.Height == 64)
                {
                    box.Image = thumbnail;
                    Log(box.Name.Replace("pic", "") + " Image changed successfully");
                    xPackage.Header.ContentImageBinary = picContent.Image.ImageToBytes(ImageFormat.Png);
                    xPackage.Header.PackageImageBinary = picPackage.Image.ImageToBytes(ImageFormat.Png);
                    ShowChanges(true);
                    return;
                }
                
                var newimage = Path.GetTempPath() + Path.GetFileNameWithoutExtension(contentImage) + ".png";
                Tools.ResizeImage(contentImage, 64, "png", newimage);

                if (File.Exists(newimage))
                {
                    box.Image = Tools.NemoLoadImage(newimage);
                    xPackage.Header.ContentImageBinary = picContent.Image.ImageToBytes(ImageFormat.Png);
                    xPackage.Header.PackageImageBinary = picPackage.Image.ImageToBytes(ImageFormat.Png);
                }
                Tools.DeleteFile(newimage);
                Log(box.Name.Replace("pic", "") + " Image changed successfully");
                ShowChanges(true);
            }
            catch
            {
                Log("Error loading image ... try again.");
            }
        }
        
        private void advTree1_Click(object sender, EventArgs e)
        {
            if (folderTree.SelectedNode != null)
            {
                folderTree.SelectedNode.Expand();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            var success = true;
            xPackage.Header.ThisType = filePackageType;

            if (filePackageType == PackageType.SavedGame)
            {
                xPackage.Header.MakeAnonymous();
            }

            if (filePackageType == PackageType.SavedGame || filePackageType == PackageType.MarketPlace)
            {
                try
                {
                    Log("Rebuilding file ... this might take a little while");
                    var signature = filePackageType == PackageType.SavedGame ? new RSAParams(Application.StartupPath + "\\bin\\KV.bin") : new RSAParams(StrongSigned.LIVE);
                    xPackage.RebuildPackage(signature);
                    xPackage.FlushPackage(signature);
                    xPackage.CloseIO();
                }
                catch
                {
                    Log("Something went wrong with trying to rebuild the file");
                    success = false;
                }
            }
            xPackage.CloseIO();

            if (success && (filePackageType == PackageType.SavedGame || filePackageType == PackageType.MarketPlace))
            {
                Log("Trying to unlock the file");
                if (Tools.UnlockCON(folder))
                {
                    Log("Unlocked file successfully");
                }
                else
                {
                    Log("Error unlocking file");
                    success = false;
                }
            }

            if (success)
            {                
                if (filePackageType != PackageType.SavedGame) return;
                Log("Trying to sign the file");
                if (Tools.SignCON(folder))
                {
                    Log("File signed successfully");
                }
                else
                {
                    Log("Error signing file");
                    Log("If you just extracted the file, this is a known bug");
                    Log("Close this form, open the song again, and try again");
                    FailedSign = true;
                }
            }
            else
            {
                FailedSign = true;
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!FailedSign)
            {
                Log("Successfully rebuilt and resigned file");
                ShowChanges(false);
            }
            else
            {
                Log("Process failed ... please close this window down and try again");
            }
            picWorking.Visible = false;
            
            if (FailedSign) return;
            var xExplorer = new CONExplorer(btnSave.BackColor, btnSave.ForeColor);
            xExplorer.lstLog.Items.AddRange(lstLog.Items);
            xExplorer.LoadCON(folder);
            Close();
            xExplorer.Show();
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (xPackage.ExtractPayload(outFolder, true, true))
                {                
                    Log("File extracted successfully to:");
                    Log(outFolder);
                }
                else
                {
                    Log("Error extracting file!");
                }
            }
            catch
            {
                Log("Error extracting file!");
            }
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            EnableDisable(true);
            Log("Ready");
        }

        private void btnVisualize_Click(object sender, EventArgs e)
        {
            if (Text.Contains("*"))
            {
                MessageBox.Show("You have unsaved changes, save first and then try again", AppName, MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                return;
            }
            if (txtTitle.Text.Contains("Rock Band 3 Song Cache") || (filePackageType != PackageType.SavedGame && filePackageType != PackageType.MarketPlace))
            {
                MessageBox.Show("Can't Visualize that file", AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            xPackage.CloseIO();
            var xVisualizer = new Visualizer(Color.FromArgb(230, 215, 0), Color.White, folder)
            {
                isRunningShortcut = isRunningShortcut
            };
            xVisualizer.Show();
            if (isRunningShortcut)
            {
                Hide();
            }
            else
            {
                Dispose();
            }
        }
        
        private void SongExplorer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (picWorking.Visible)
            {
                MessageBox.Show("Please wait until the current process finishes", AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Cancel = true;
                return;
            }
            if (Changes)
            {
                if (MessageBox.Show("You have unsaved changes\nIf you exit now nothing will be changed in the file\nAre you sure you want to exit?",
                        AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
            }

            if (IsMultiMode)
            {
                foreach (var pkg in multiPackages) pkg.CloseIO();
            }
            else if (xPackage != null)
            {
                xPackage.CloseIO();
            }
            Tools.DeleteFolder(Application.StartupPath + "\\packex\\", true);
            Tools.DeleteFolder(Application.StartupPath + "\\temp\\", true);
        }

        private void buttonX8_Click_1(object sender, EventArgs e)
        {
            if (chkAutoExtract.Checked)
            {
                outFolder = folder + "_extracted\\";
            }
            else
            {
                var ofd = new FolderPicker
                {
                    Title = "Select folder to extract file to",
                };
                if (ofd.ShowDialog(IntPtr.Zero) == true)
                {
                    outFolder = ofd.ResultPath + "\\" + Path.GetFileNameWithoutExtension(folder) + "_extracted\\";
                    Tools.CurrentFolder = outFolder;
                }
                else
                {
                    return;
                }
            }
            Log("Extracting file");
            EnableDisable(false);
            backgroundWorker2.RunWorkerAsync();
        }

        private void buttonX9_Click(object sender, EventArgs e)
        {
            EnableDisable(false);
            backgroundWorker1.RunWorkerAsync();
        }
        
        private void cboGameID_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (starting || cboGameID.SelectedIndex == -1) return;
            var title = "";
            switch (cboGameID.SelectedIndex)
            {
                case 0:
                    xPackage.Header.TitleID = 0x45410829;
                    picContent.Image = Resources.RB1;
                    title = "Rock Band 1";
                    break;
                case 1:
                    xPackage.Header.TitleID = 0x45410869;
                    picContent.Image = Resources.RB2;
                    title = "Rock Band 2";
                    break;
                case 2:
                    xPackage.Header.TitleID = 0x454108B1;
                    picContent.Image = Resources.TBRB;
                    title = "The Beatles: Rock Band";
                    break;
                case 3:
                    xPackage.Header.TitleID = 0x575207F0;
                    picContent.Image = Resources.RBL;
                    title = "LEGO Rock Band";
                    break;
                case 4:
                    xPackage.Header.TitleID = 0x454108E7;
                    picContent.Image = Resources.GDRB;
                    title = "Green Day: Rock Band";
                    break;
                case 5:
                    xPackage.Header.TitleID = 0x45410914;
                    picContent.Image = Resources.RB3;
                    title = "Rock Band 3";
                    break;
                case 6:
                    xPackage.Header.TitleID = 0x415607E7;
                    picContent.Image = Resources.GH2;
                    title = "Guitar Hero II";
                    break;
                case 7:
                    xPackage.Header.TitleID = 0x415607F7;
                    picContent.Image = Resources.GH3;
                    title = "Guitar Hero III";
                    break;
                case 8:
                    xPackage.Header.TitleID = 0x4156081A;
                    picContent.Image = Resources.GH4WT;
                    title = "Guitar Hero: World Tour";
                    break;
                case 9:
                    xPackage.Header.TitleID = 0x41560840;
                    picContent.Image = Resources.GH5;
                    title = "Guitar Hero 5";
                    break;
                case 10:
                    xPackage.Header.TitleID = 0x41560883;
                    picContent.Image = Resources.GH6WOR;
                    title = "Guitar Hero: Warriors of Rock";
                    break;
                case 11:
                    xPackage.Header.TitleID = 0x545607D3;
                    picContent.Image = Resources.dc1;
                    title = "Dance Central";
                    break;
                case 12:
                    xPackage.Header.TitleID = 0x373307D2;
                    picContent.Image = Resources.dc2;
                    title = "Dance Central 2";
                    break;
                case 13:
                    xPackage.Header.TitleID = 0x373307D9;
                    picContent.Image = Resources.dc3;
                    title = "Dance Central 3";
                    break;
                case 14:
                    xPackage.Header.TitleID = 0x41560844;
                    picContent.Image = Resources.djh1;
                    title = "DJ Hero";
                    break;
                case 15:
                    xPackage.Header.TitleID = 0x4156087F;
                    picContent.Image = Resources.djh2;
                    title = "DJ Hero 2";
                    break;
                case 16:
                    xPackage.Header.TitleID = 0x5841122D;
                    picContent.Image = Resources.RBB;
                    title = "Rock Band Blitz";
                    break;
            }
            xPackage.Header.Title_Package = title; 
            Log("Game changed to " + xPackage.Header.Title_Package);
            xPackage.Header.ContentImageBinary = picContent.Image.ImageToBytes(ImageFormat.Png);
            ShowChanges(true);
        }

        private void listView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (fileList.SelectedItems.Count == 0)
            {
                return;
            }
            
            var files = new List<string>();
            var ext_folder = Application.StartupPath + "\\temp\\";
            if (!Directory.Exists(ext_folder))
            {
                Directory.CreateDirectory(ext_folder);
            }

            for (var i = 0; i < fileList.SelectedItems.Count; i++)
            {
                var xent = (FileEntry)fileList.SelectedItems[i].Tag;
                Enabled = false;
                Log("Extracting " + xent.Name);
                var ext_file = ext_folder + xent.Name;
                Tools.DeleteFile(ext_file);
                if (!xent.ExtractToFile(ext_file))
                {
                    Log("Error extracting " + xent.Name);
                    Enabled = true;
                    return;
                }
                files.Add(ext_file);
            }
            Enabled = true;

            try
            {
                internalDrop = true;
                DoDragDrop(new DataObject(DataFormats.FileDrop, files.ToArray()), DragDropEffects.Copy);
                
                foreach (var file in files)
                {
                    if (internalDrop)
                    {
                        Log("File " + Path.GetFileName(file) + " extracted successfully");
                    }
                    else
                    {
                        Log("Cancelled extracting file " + Path.GetFileName(file));
                    }
                    //try to clean any leftover temp files
                    Tools.DeleteFile(file);
                }
            }
            catch (Exception ex)
            {
                Log("There was an error extracting the selected file(s)!");
                Log("The error says:");
                Log(ex.Message);
            }

            internalDrop = false;
        }

        private void addNewFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Title = "Select file(s) to add",
                Multiselect = true,
                InitialDirectory = Path.GetDirectoryName(xPackage.FileNameLong)
            };

            if (ofd.ShowDialog() != DialogResult.Cancel)
            {
                AddFiles(ofd.FileNames);
            }
        }

        private void extractImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var pic = "Package Image";
            var picturebox = picPackage;

            var x = tabControl1.PointToClient(new Point(MousePosition.X, MousePosition.Y)).X;
            var y = tabControl1.PointToClient(new Point(MousePosition.X, MousePosition.Y)).Y;

            if (y > picContent.Location.Y && y < picContent.Location.Y + picContent.Height)
            {
                if (x > picContent.Location.X & x < picContent.Location.X + picContent.Width)
                {
                    pic = "Content Image";
                    picturebox = picContent;
                }
            }
            
            var sfd = new SaveFileDialog
                {
                    FileName = Text.Replace(" ","") + "_" + pic.Replace(" ",""),
                    Filter = "PNG File (*.png)|*.png",
                    InitialDirectory = Tools.CurrentFolder,
                    Title = "Choose where to save the " + pic + " to"
                };

            sfd.ShowDialog();

            if (sfd.FileName != "")
            {
                var png = picturebox.Image;
                png.Save(sfd.FileName,ImageFormat.Png);
            }
            
            Tools.CurrentFolder = Path.GetDirectoryName(sfd.FileName);
            

            if (File.Exists(sfd.FileName))
            {
                Log(pic + " extracted successfully");
            }
            else
            {
                Log("Looks like extracting " + pic + " failed. Try again");
            }
        }

        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {
            var pic = "";
            var x = tabControl1.PointToClient(new Point(MousePosition.X, MousePosition.Y)).X;
            var y = tabControl1.PointToClient(new Point(MousePosition.X, MousePosition.Y)).Y;

            if (y > picPackage.Location.Y && y < picPackage.Location.Y + picPackage.Height)
            {
                if (x > picPackage.Location.X && x < picPackage.Location.X + picPackage.Width)
                {
                    pic = "Package Image";
                }
            }

            if (y > picContent.Location.Y && y < picContent.Location.Y + picContent.Height)
            {
                if (x > picContent.Location.X & x < picContent.Location.X + picContent.Width)
                {
                    pic = "Content Image";
                }
            }
            
            extractImageToolStripMenuItem.Text = "Extract " + pic;
        }

        private void textBoxX2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.A)
                txtTitle.SelectAll();
        }

        private void textBoxX3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.A)
                txtDescription.SelectAll();
        }

        private void textBoxX2_DoubleClick(object sender, EventArgs e)
        {
            txtTitle.SelectAll();
        }

        private void textBoxX3_DoubleClick(object sender, EventArgs e)
        {
            txtDescription.SelectAll();
        }

        private void exportLogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.ExportLog(Text, lstLog.Items);
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            ShowChanges(true);
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            ShowChanges(true);
        }

        private void PackageImage_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            getImage(files[0], picPackage);
        }

        private void ContentImage_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            getImage(files[0], picContent);
        }

        private void HandleDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
        }

        private void ContentImage_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                getImage("", picContent);
            }
        }

        private void PackageImage_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                getImage("", picPackage);
            }
        }
        
        private void CONExplorer_Shown(object sender, EventArgs e)
        {
            if (IsMultiMode || xPackage == null) return;
            var xfile = xPackage.GetFile("songs/songs.dta") ?? xPackage.GetFile("songs_upgrades/upgrades.dta");
            if (xfile == null) return;

            var isUpgrade = false;
            
            if (!Parser.ExtractDTA(xPackage, false))
            {
                isUpgrade = Parser.ExtractDTA(xPackage, false, true);
                if (!isUpgrade) return;
            }
            if (!Parser.ReadDTA(Parser.DTA) || !Parser.Songs.Any())
            {
                return;
            }

            activeUPG = xPackage.GetFile("songs_upgrades/upgrades.dta");
            activeDTA = xPackage.GetFile("songs/songs.dta");
            if (!isUpgrade)
            {
                shortName = Parser.Songs[0].ShortName;
                internalName = Parser.Songs[0].InternalName;
                if (!string.IsNullOrWhiteSpace(internalName) && Parser.Songs.Count == 1)
                {
                    activeMIDI = xPackage.GetFile("songs/" + internalName + "/" + internalName + ".mid");
                    activeMOGG = xPackage.GetFile("songs/" + internalName + "/" + internalName + ".mogg");
                    activePNG = xPackage.GetFile("songs/" + internalName + "/gen/" + internalName + "_keep.png_xbox");
                    activeMILO = xPackage.GetFile("songs/" + internalName + "/gen/" + internalName + ".milo_xbox");
                }
            }
            else
            {
                internalName = Parser.Songs[0].MIDIFile;
                activeMIDI = xPackage.GetFile(internalName);
            }

            btnDTA.Enabled = activeDTA != null || activeUPG != null;
            btnMIDI.Enabled = activeMIDI != null && Parser.Songs.Count < 2;
            btnMOGG.Enabled = activeMOGG != null && Parser.Songs.Count < 2;
            btnPNG.Enabled = activePNG != null && Parser.Songs.Count < 2;
            btnMILO.Enabled = activeMILO != null && Parser.Songs.Count < 2;

            btnDTA.BackColor = btnDTA.Enabled ? Color.LightYellow : Color.LightGray;
            btnMIDI.BackColor = btnMIDI.Enabled ? Color.LightYellow : Color.LightGray;
            btnMOGG.BackColor = btnMOGG.Enabled ? Color.LightYellow : Color.LightGray;
            btnPNG.BackColor = btnPNG.Enabled ? Color.LightYellow : Color.LightGray;
            btnMILO.BackColor = btnMILO.Enabled ? Color.LightYellow : Color.LightGray;

            btnDTA.Text = activeDTA != null && activeUPG != null ? "DTAs" : "DTA";
            
            if (Parser.Songs.Count > 1)
            {
                lblSongID.Text = "N/A - PACK";
                btnVisualize.Visible = false;
                btnChange.Visible = false;
                return;
            }
            if (Parser.Songs[0].SongId == 0 || string.IsNullOrWhiteSpace(Parser.Songs[0].SongIdString))
            {
                lblSongID.Text = Parser.Songs[0].HasSongIDError ? "SONG ID MISSING" : "NOT PRESENT IN DTA";
                btnVisualize.Visible = !isUpgrade; //Parser.Songs[0].HasSongIDError && 
                btnChange.Visible = !isUpgrade;
                return;
            }
            lblSongID.Text = Parser.Songs[0].SongIdString;
            lblSongID.Cursor = Cursors.Hand;
            lblIDTop.Cursor = lblSongID.Cursor;
            toolTip1.SetToolTip(lblSongID, "Click to copy song ID to clipboard");
            toolTip1.SetToolTip(lblIDTop, "Click to copy song ID to clipboard");
            btnVisualize.Visible = btnVisualize.Visible && !isUpgrade && !txtTitle.Text.Contains("Rock Band 3 Song Cache");
            btnChange.Visible = !isUpgrade;
        }

        private void lblIDTop_MouseClick(object sender, MouseEventArgs e)
        {
            var fails = new List<string> {"SONG ID MISSING", "NOT PRESENT IN DTA", "N/A", "N/A - PACK"};
            if (e.Button != MouseButtons.Left || fails.Contains(lblSongID.Text) || string.IsNullOrWhiteSpace(lblSongID.Text)) return;
            Clipboard.SetText(lblSongID.Text);
            MessageBox.Show("Song ID was copied to your clipboard", AppName, MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        private void ExtractFile(FileEntry xent, FileEntry xent2 = null)
        {
            var ext_folder = Path.Combine(Application.StartupPath, "temp");
            if (!Directory.Exists(ext_folder))
            {
                Directory.CreateDirectory(ext_folder);
            }

            Enabled = false;
            Log("Extracting " + xent.Name);
            var ext_file = new List<string> { Path.Combine(ext_folder, xent.Name) };
            Tools.DeleteFile(ext_file[0]);

            if (!xent.ExtractToFile(ext_file[0]))
            {
                Log("Error extracting " + xent.Name);
                Enabled = true;
                return;
            }

            if (xent2 != null)
            {
                Log("Extracting " + xent2.Name);
                string secondFilePath = Path.Combine(ext_folder, xent2.Name);
                ext_file.Add(secondFilePath);
                Tools.DeleteFile(secondFilePath);

                if (!xent2.ExtractToFile(secondFilePath))
                {
                    Log("Error extracting " + xent2.Name);
                    ext_file.Remove(secondFilePath);
                }
            }

            Enabled = true;

            if (File.Exists(ext_file[0]))
            {
                if (Tools.IsRunningOnWine())
                {
                    // Running under WINE - use Save File Dialog instead of clipboard
                    foreach (var filePath in ext_file)
                    {
                        SaveFileUsingDialog(filePath);
                    }
                }
                else
                {
                    // Normal Windows behavior - copy to clipboard
                    var moveEffect = new byte[] { 2, 0, 0, 0 };
                    var dropEffect = new MemoryStream();
                    dropEffect.Write(moveEffect, 0, moveEffect.Length);

                    var data = new DataObject();
                    var fileDropList = new StringCollection();
                    fileDropList.AddRange(ext_file.ToArray());

                    data.SetFileDropList(fileDropList);
                    data.SetData("Preferred DropEffect", dropEffect);

                    Clipboard.Clear();
                    Clipboard.SetDataObject(data, true);

                    Log("File " + Path.GetFileName(ext_file[0]) + " extracted successfully");
                    if (ext_file.Count > 1)
                    {
                        Log("File " + Path.GetFileName(ext_file[1]) + " extracted successfully");
                    }
                    Log(ext_file.Count > 1 ? "Files are in your clipboard, paste them anywhere" : "File is in your clipboard, paste it anywhere");
                }
            }
            else
            {
                Log("Could not extract file " + Path.GetFileName(ext_file[0]));
            }
        }        

        /// <summary>
        /// Extracts the selected list entries to temp and puts them on the clipboard as files,
        /// so they can be pasted in Explorer (same as the DTA/MIDI/MOGG buttons).
        /// </summary>
        private List<FileEntry> SelectedEntries()
        {
            return fileList.SelectedItems.Cast<ListViewItem>().Select(i => i.Tag as FileEntry).Where(f => f != null).ToList();
        }

        private void CopySelectedFiles()
        {
            CopyFiles(SelectedEntries());
        }

        //files collected across searches/folders, copied together
        private readonly List<FileEntry> copyList = new List<FileEntry>();

        private void CheckSelected(bool check)
        {
            foreach (ListViewItem item in fileList.SelectedItems) item.Checked = check; //ItemChecked updates copyList
        }

        private void ClearCopyList()
        {
            copyList.Clear();
            suppressCheck = true;
            foreach (ListViewItem item in fileList.Items) item.Checked = false;
            suppressCheck = false;
            Log("Copy list cleared");
        }

        private void CopyFiles(List<FileEntry> entries)
        {
            if (entries.Count == 0) return;

            var ext_folder = Path.Combine(Application.StartupPath, "temp", "copy");
            Tools.DeleteFolder(ext_folder, true);
            Directory.CreateDirectory(ext_folder);

            Enabled = false;
            var paths = new List<string>();
            foreach (var xent in entries)
            {
                //same name from different packages/folders: put each in its own subfolder
                var folder = ext_folder;
                var n = 1;
                while (File.Exists(Path.Combine(folder, xent.Name)))
                {
                    folder = Path.Combine(ext_folder, (n++).ToString());
                }
                Directory.CreateDirectory(folder);
                var path = Path.Combine(folder, xent.Name);
                Log("Extracting " + xent.Name);
                if (!xent.ExtractToFile(path) || !File.Exists(path))
                {
                    Log("Error extracting " + xent.Name);
                    continue;
                }
                paths.Add(path);
            }
            Enabled = true;
            if (paths.Count == 0) return;

            if (Tools.IsRunningOnWine())
            {
                foreach (var p in paths) SaveFileUsingDialog(p);
                return;
            }
            var data = new DataObject();
            var dropList = new StringCollection();
            dropList.AddRange(paths.ToArray());
            data.SetFileDropList(dropList);
            data.SetData("Preferred DropEffect", new MemoryStream(new byte[] { 1, 0, 0, 0 })); //copy
            Clipboard.Clear();
            Clipboard.SetDataObject(data, true);
            Log(paths.Count > 1 ? paths.Count + " files are in your clipboard, paste them anywhere" : "File is in your clipboard, paste it anywhere");
        }

        /// <summary>
        /// Prompts the user with a Save File Dialog and copies the extracted file to the chosen location.
        /// </summary>
        private void SaveFileUsingDialog(string sourcePath)
        {
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.FileName = Path.GetFileName(sourcePath); // Prepopulate filename
                saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                saveDialog.Title = "Save Extracted File";
                saveDialog.Filter = "All Files|*.*";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.Copy(sourcePath, saveDialog.FileName.TrimEnd('.', ' '), true); //avoid reported issues of trailing periods
                        Log($"File saved successfully: {saveDialog.FileName}");
                    }
                    catch (Exception ex)
                    {
                        Log($"Error saving file: {ex.Message}");
                    }
                }
                else
                {
                    Log("File save cancelled.");
                }
            }
        }

        private void ReplaceFile(FileEntry xent, string file)
        {
            if (Path.GetExtension(xent.Name) == Path.GetExtension(file))
            {
                if (MessageBox.Show("Replace '" + xent.Name + "' with '" + Path.GetFileName(file) + "'?",
                    "Replace file?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                Log("Replacing file " + xent.Name);
                    
                var obf_file = Path.GetTempPath() + "m";
                if (Path.GetExtension(file) == ".mogg")
                {
                    Tools.DeleteFile(obf_file);
                    File.Copy(file, obf_file);
                    nautilus3.WriteOutData(nautilus3.DeObfM(File.ReadAllBytes(obf_file)), obf_file);
                }
                else
                {
                    obf_file = file;
                }
                if (!xent.Replace(obf_file))
                {
                    Log("Error replacing file " + xent.Name);
                    return;
                }
                Log("Replaced file " + xent.Name + " successfully");
                ShowChanges(true);
            }
            else
            {
                MessageBox.Show("Can't replace a '" + Path.GetExtension(xent.Name) + "' file with a '" + Path.GetExtension(file) + "' file", AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnDTA_Click(object sender, EventArgs e)
        {
            if (activeDTA != null && activeUPG != null)
            {
                ExtractFile(activeDTA, activeUPG);
            }
            else 
            {
                if (activeDTA != null)
                {
                    ExtractFile(activeDTA);
                }
                if (activeUPG != null)
                {
                    ExtractFile(activeUPG);
                }
            }
        }

        private void btnMIDI_Click(object sender, EventArgs e)
        {
            ExtractFile(activeMIDI);
        }

        private void btnPNG_Click(object sender, EventArgs e)
        {
            ExtractFile(activePNG);
        }

        private void btnMOGG_Click(object sender, EventArgs e)
        {
            ExtractFile(activeMOGG);
        }

        private void btnDTA_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            switch (Path.GetFileName(files[0]).ToLowerInvariant())
            {
                case "songs.dta":
                    ReplaceFile(activeDTA, files[0]);
                    break;
                case "upgrades.dta":
                    ReplaceFile(activeUPG, files[0]);
                    break;
                default:
                    MessageBox.Show("Only songs.dta or upgrades.dta files can be replaced this way", AppName,
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    break;
            }
        }

        private void btnMIDI_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            ReplaceFile(activeMIDI, files[0]);
        }

        private void btnPNG_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            ReplaceFile(activePNG, files[0]);
        }

        private void btnMOGG_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            ReplaceFile(activeMOGG, files[0]);
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            var fails = new List<string> { "SONG ID MISSING", "NOT PRESENT IN DTA", "N/A", "N/A - PACK" };
            if (fails.Contains(lblSongID.Text))
            {
                MessageBox.Show("Not possible with this file", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            var popup = new PasswordUnlocker(lblSongID.Text, shortName);
            popup.IDChanger();
            popup.ShowDialog();
            var newID = popup.EnteredText;
            popup.Dispose();
            if (string.IsNullOrWhiteSpace(newID) || newID.Trim() == lblSongID.Text) return;
            var dta = Path.GetTempPath() + "dta";
            Tools.DeleteFile(dta);
            if (!activeDTA.ExtractToFile(dta))
            {
                Log("Error extracting DTA file...failed to change song ID!");
                return;
            }
            Tools.ReplaceSongID(dta, newID, "", Parser.Songs[0].SongIdString);
            if (!activeDTA.Replace(dta))
            {
                Tools.DeleteFile(dta);
                Log("Error replacing DTA file...failed to change song ID!");
                return;
            }
            Tools.DeleteFile(dta);
            lblSongID.Text = newID;
            Log("Changed song ID to '" + newID + "' successfully");
            btnSave.PerformClick();
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

        private void btnMILO_Click(object sender, EventArgs e)
        {
            ExtractFile(activeMILO);
        }

        private void btnMILO_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            ReplaceFile(activeMILO, files[0]);
        }

        private void injectSelectedFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doInject = true;
            InjectReplaceFile();
        }

        private void menuDeleteFile_Click(object sender, EventArgs e)
        {
            DeleteFile();
        }

        private void menuRenameFile_Click(object sender, EventArgs e)
        {
            RenameFile(); 
        }

        private void RenameFile()
        {
            try
            {
                var x = (FileEntry)fileList.SelectedItems[0].Tag;
                var currentName = x.Name;
                var popup = new PasswordUnlocker(currentName);
                popup.Renamer();
                popup.ShowDialog();
                var newName = popup.EnteredText;
                popup.Dispose();
                if (currentName == newName) return;
                if (string.IsNullOrEmpty(newName))
                {
                    MessageBox.Show("New name can't be blank", "CON Explorer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                x.Name = newName;
                fileList.SelectedItems[0].Text = newName;
                Log("Renamed file '" + currentName + "' to '" + newName + "' successfully");
                Log("Don't forget to save your changes");
                ShowChanges(true);
            }
            catch
            {
                Log("Error renaming that file");
            }
        }

        private void contextMenuStrip4_Opening(object sender, CancelEventArgs e)
        {
            menuRenameFolder.Visible = folderTree.SelectedNode != null && !IsMultiMode;
        }

        private void menuRenameFolder_Click(object sender, EventArgs e)
        {
            if (folderTree.SelectedNode == null) return;
            try
            {
                var f = (FolderEntry)folderTree.SelectedNode.Tag;
                var folder = folderTree.SelectedNode;
                var currentName = f.Name;
                var popup = new PasswordUnlocker(currentName);
                popup.Renamer();
                popup.ShowDialog();
                var newName = popup.EnteredText;
                popup.Dispose();
                if (currentName == newName) return;
                if (string.IsNullOrEmpty(newName))
                {
                    MessageBox.Show("New name can't be blank", "CON Explorer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                folder.Text = newName;
                folder.Name = newName;
                f.Name = newName;               
                Log("Renamed folder '" + currentName + "' to '" + newName + "' successfully");
                Log("Don't forget to save your changes");
                ShowChanges(true);
            }
            catch
            {
                Log("Error renaming that folder");
            }
        }
    }
}