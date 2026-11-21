namespace Nautilus
{
    partial class BatchRenamer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnFolder = new System.Windows.Forms.Button();
            this.txtFolder = new System.Windows.Forms.TextBox();
            this.lstLog = new System.Windows.Forms.ListBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exportLogFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnBegin = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.picPin = new System.Windows.Forms.PictureBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.renamingOptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.renameInternalName = new System.Windows.Forms.ToolStripMenuItem();
            this.ezloDQMode = new System.Windows.Forms.ToolStripMenuItem();
            this.renameTheArtistSong = new System.Windows.Forms.ToolStripMenuItem();
            this.renameArtistTheSong = new System.Windows.Forms.ToolStripMenuItem();
            this.renameSongTheArtist = new System.Windows.Forms.ToolStripMenuItem();
            this.renameSongArtistThe = new System.Windows.Forms.ToolStripMenuItem();
            this.renameYearArtist = new System.Windows.Forms.ToolStripMenuItem();
            this.renameYearSong = new System.Windows.Forms.ToolStripMenuItem();
            this.normalizeFeaturedArtists = new System.Windows.Forms.ToolStripMenuItem();
            this.removeSpacesFromFileName = new System.Windows.Forms.ToolStripMenuItem();
            this.replaceSpacesWithUnderscores = new System.Windows.Forms.ToolStripMenuItem();
            this.ignoreXboxFilesystemLimitations = new System.Windows.Forms.ToolStripMenuItem();
            this.fullNameNoAccents = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.batchRenamePhaseShift = new System.Windows.Forms.ToolStripMenuItem();
            this.sortingOptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tryToSortFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.tryDetailedSubsorting = new System.Windows.Forms.ToolStripMenuItem();
            this.separateXonlySongs = new System.Windows.Forms.ToolStripMenuItem();
            this.sortByAuthor = new System.Windows.Forms.ToolStripMenuItem();
            this.otherOptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteOlderCopies = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteExactCopiesONLY = new System.Windows.Forms.ToolStripMenuItem();
            this.FileRenamer = new System.ComponentModel.BackgroundWorker();
            this.picWorking = new System.Windows.Forms.PictureBox();
            this.folderScanner = new System.ComponentModel.BackgroundWorker();
            this.PhaseShiftRenamer = new System.ComponentModel.BackgroundWorker();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPin)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picWorking)).BeginInit();
            this.SuspendLayout();
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(251)))), ((int)(((byte)(211)))), ((int)(((byte)(0)))));
            this.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefresh.ForeColor = System.Drawing.Color.Black;
            this.btnRefresh.Location = new System.Drawing.Point(488, 32);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(114, 30);
            this.btnRefresh.TabIndex = 18;
            this.btnRefresh.Text = "&Refresh Folder";
            this.toolTip1.SetToolTip(this.btnRefresh, "Click to rescan the input folder");
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Visible = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            this.btnRefresh.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.btnRefresh.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // btnFolder
            // 
            this.btnFolder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(251)))), ((int)(((byte)(211)))), ((int)(((byte)(0)))));
            this.btnFolder.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFolder.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFolder.ForeColor = System.Drawing.Color.Black;
            this.btnFolder.Location = new System.Drawing.Point(12, 32);
            this.btnFolder.Name = "btnFolder";
            this.btnFolder.Size = new System.Drawing.Size(137, 30);
            this.btnFolder.TabIndex = 17;
            this.btnFolder.Text = "Select &Input Folder";
            this.toolTip1.SetToolTip(this.btnFolder, "Click to select the folder where your files are");
            this.btnFolder.UseVisualStyleBackColor = false;
            this.btnFolder.Click += new System.EventHandler(this.btnFolder_Click);
            this.btnFolder.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.btnFolder.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // txtFolder
            // 
            this.txtFolder.BackColor = System.Drawing.Color.White;
            this.txtFolder.Location = new System.Drawing.Point(12, 68);
            this.txtFolder.Name = "txtFolder";
            this.txtFolder.ReadOnly = true;
            this.txtFolder.Size = new System.Drawing.Size(590, 20);
            this.txtFolder.TabIndex = 16;
            this.txtFolder.TextChanged += new System.EventHandler(this.txtFolder_TextChanged);
            this.txtFolder.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.txtFolder.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // lstLog
            // 
            this.lstLog.AllowDrop = true;
            this.lstLog.BackColor = System.Drawing.Color.White;
            this.lstLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstLog.ContextMenuStrip = this.contextMenuStrip1;
            this.lstLog.FormattingEnabled = true;
            this.lstLog.HorizontalScrollbar = true;
            this.lstLog.Location = new System.Drawing.Point(12, 94);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(590, 210);
            this.lstLog.TabIndex = 19;
            this.lstLog.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.lstLog.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportLogFileToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.ShowImageMargin = false;
            this.contextMenuStrip1.Size = new System.Drawing.Size(122, 26);
            // 
            // exportLogFileToolStripMenuItem
            // 
            this.exportLogFileToolStripMenuItem.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.exportLogFileToolStripMenuItem.Name = "exportLogFileToolStripMenuItem";
            this.exportLogFileToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.exportLogFileToolStripMenuItem.Text = "Export log file";
            this.exportLogFileToolStripMenuItem.Click += new System.EventHandler(this.exportLogFileToolStripMenuItem_Click);
            // 
            // btnBegin
            // 
            this.btnBegin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(251)))), ((int)(((byte)(211)))), ((int)(((byte)(0)))));
            this.btnBegin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBegin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBegin.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBegin.ForeColor = System.Drawing.Color.Black;
            this.btnBegin.Location = new System.Drawing.Point(543, 310);
            this.btnBegin.Name = "btnBegin";
            this.btnBegin.Size = new System.Drawing.Size(59, 30);
            this.btnBegin.TabIndex = 20;
            this.btnBegin.Text = "&Begin";
            this.toolTip1.SetToolTip(this.btnBegin, "Click to begin renaming process");
            this.btnBegin.UseVisualStyleBackColor = false;
            this.btnBegin.Visible = false;
            this.btnBegin.Click += new System.EventHandler(this.btnBegin_Click);
            this.btnBegin.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.btnBegin.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // picPin
            // 
            this.picPin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picPin.BackColor = System.Drawing.Color.Transparent;
            this.picPin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picPin.Image = global::Nautilus.Properties.Resources.unpinned;
            this.picPin.Location = new System.Drawing.Point(588, 6);
            this.picPin.Name = "picPin";
            this.picPin.Size = new System.Drawing.Size(20, 20);
            this.picPin.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picPin.TabIndex = 64;
            this.picPin.TabStop = false;
            this.picPin.Tag = "unpinned";
            this.toolTip1.SetToolTip(this.picPin, "Click to pin on top");
            this.picPin.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picPin_MouseClick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renamingOptionsToolStripMenuItem,
            this.sortingOptionsToolStripMenuItem,
            this.otherOptionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(614, 24);
            this.menuStrip1.TabIndex = 25;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // renamingOptionsToolStripMenuItem
            // 
            this.renamingOptionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renameFiles,
            this.normalizeFeaturedArtists,
            this.removeSpacesFromFileName,
            this.replaceSpacesWithUnderscores,
            this.ignoreXboxFilesystemLimitations,
            this.fullNameNoAccents,
            this.toolStripMenuItem1,
            this.batchRenamePhaseShift});
            this.renamingOptionsToolStripMenuItem.Name = "renamingOptionsToolStripMenuItem";
            this.renamingOptionsToolStripMenuItem.Size = new System.Drawing.Size(118, 20);
            this.renamingOptionsToolStripMenuItem.Text = "&Renaming Options";
            // 
            // renameFiles
            // 
            this.renameFiles.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.renameFiles.Checked = true;
            this.renameFiles.CheckOnClick = true;
            this.renameFiles.CheckState = System.Windows.Forms.CheckState.Checked;
            this.renameFiles.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renameInternalName,
            this.ezloDQMode,
            this.renameTheArtistSong,
            this.renameArtistTheSong,
            this.renameSongTheArtist,
            this.renameSongArtistThe,
            this.renameYearArtist,
            this.renameYearSong});
            this.renameFiles.Name = "renameFiles";
            this.renameFiles.Size = new System.Drawing.Size(299, 22);
            this.renameFiles.Text = "Rename files";
            this.renameFiles.Click += new System.EventHandler(this.renameFilesToolStripMenuItem_Click);
            // 
            // renameInternalName
            // 
            this.renameInternalName.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.renameInternalName.Name = "renameInternalName";
            this.renameInternalName.Size = new System.Drawing.Size(246, 22);
            this.renameInternalName.Text = "InternalName";
            this.renameInternalName.Click += new System.EventHandler(this.internalName_Click);
            // 
            // ezloDQMode
            // 
            this.ezloDQMode.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ezloDQMode.Name = "ezloDQMode";
            this.ezloDQMode.Size = new System.Drawing.Size(246, 22);
            this.ezloDQMode.Text = "InternalName (The Artist - Song)";
            this.ezloDQMode.Click += new System.EventHandler(this.internalName_Click);
            // 
            // renameTheArtistSong
            // 
            this.renameTheArtistSong.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.renameTheArtistSong.Checked = true;
            this.renameTheArtistSong.CheckState = System.Windows.Forms.CheckState.Checked;
            this.renameTheArtistSong.Name = "renameTheArtistSong";
            this.renameTheArtistSong.Size = new System.Drawing.Size(246, 22);
            this.renameTheArtistSong.Text = "The Artist - Song";
            this.renameTheArtistSong.Click += new System.EventHandler(this.internalName_Click);
            // 
            // renameArtistTheSong
            // 
            this.renameArtistTheSong.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.renameArtistTheSong.Name = "renameArtistTheSong";
            this.renameArtistTheSong.Size = new System.Drawing.Size(246, 22);
            this.renameArtistTheSong.Text = "Artist, The - Song";
            this.renameArtistTheSong.Click += new System.EventHandler(this.internalName_Click);
            // 
            // renameSongTheArtist
            // 
            this.renameSongTheArtist.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.renameSongTheArtist.Name = "renameSongTheArtist";
            this.renameSongTheArtist.Size = new System.Drawing.Size(246, 22);
            this.renameSongTheArtist.Text = "Song - The Artist";
            this.renameSongTheArtist.Click += new System.EventHandler(this.internalName_Click);
            // 
            // renameSongArtistThe
            // 
            this.renameSongArtistThe.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.renameSongArtistThe.Name = "renameSongArtistThe";
            this.renameSongArtistThe.Size = new System.Drawing.Size(246, 22);
            this.renameSongArtistThe.Text = "Song - Artist, The";
            this.renameSongArtistThe.Click += new System.EventHandler(this.internalName_Click);
            // 
            // renameYearArtist
            // 
            this.renameYearArtist.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.renameYearArtist.Name = "renameYearArtist";
            this.renameYearArtist.Size = new System.Drawing.Size(246, 22);
            this.renameYearArtist.Text = "(Year) The Artist - Song";
            this.renameYearArtist.Click += new System.EventHandler(this.internalName_Click);
            // 
            // renameYearSong
            // 
            this.renameYearSong.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.renameYearSong.Name = "renameYearSong";
            this.renameYearSong.Size = new System.Drawing.Size(246, 22);
            this.renameYearSong.Text = "(Year) Song - The Artist";
            this.renameYearSong.Click += new System.EventHandler(this.internalName_Click);
            // 
            // normalizeFeaturedArtists
            // 
            this.normalizeFeaturedArtists.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.normalizeFeaturedArtists.Checked = true;
            this.normalizeFeaturedArtists.CheckOnClick = true;
            this.normalizeFeaturedArtists.CheckState = System.Windows.Forms.CheckState.Checked;
            this.normalizeFeaturedArtists.Name = "normalizeFeaturedArtists";
            this.normalizeFeaturedArtists.Size = new System.Drawing.Size(299, 22);
            this.normalizeFeaturedArtists.Text = "Normalize featured artists to \'ft.\'";
            // 
            // removeSpacesFromFileName
            // 
            this.removeSpacesFromFileName.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.removeSpacesFromFileName.CheckOnClick = true;
            this.removeSpacesFromFileName.Name = "removeSpacesFromFileName";
            this.removeSpacesFromFileName.Size = new System.Drawing.Size(299, 22);
            this.removeSpacesFromFileName.Text = "Remove spaces from file names";
            this.removeSpacesFromFileName.Click += new System.EventHandler(this.removeSpacesFromFileNameToolStripMenuItem_Click);
            // 
            // replaceSpacesWithUnderscores
            // 
            this.replaceSpacesWithUnderscores.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.replaceSpacesWithUnderscores.CheckOnClick = true;
            this.replaceSpacesWithUnderscores.Name = "replaceSpacesWithUnderscores";
            this.replaceSpacesWithUnderscores.Size = new System.Drawing.Size(299, 22);
            this.replaceSpacesWithUnderscores.Text = "Replace spaces with underscores";
            this.replaceSpacesWithUnderscores.Click += new System.EventHandler(this.replaceSpacesWithUnderscoresToolStripMenuItem_Click);
            // 
            // ignoreXboxFilesystemLimitations
            // 
            this.ignoreXboxFilesystemLimitations.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ignoreXboxFilesystemLimitations.CheckOnClick = true;
            this.ignoreXboxFilesystemLimitations.Name = "ignoreXboxFilesystemLimitations";
            this.ignoreXboxFilesystemLimitations.Size = new System.Drawing.Size(299, 22);
            this.ignoreXboxFilesystemLimitations.Text = "Ignore Xbox file system limitations";
            this.ignoreXboxFilesystemLimitations.Click += new System.EventHandler(this.ignoreXboxFilesystemLimitations_Click);
            // 
            // fullNameNoAccents
            // 
            this.fullNameNoAccents.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.fullNameNoAccents.CheckOnClick = true;
            this.fullNameNoAccents.Name = "fullNameNoAccents";
            this.fullNameNoAccents.Size = new System.Drawing.Size(299, 22);
            this.fullNameNoAccents.Text = "Full name, no accents or special characters";
            this.fullNameNoAccents.Click += new System.EventHandler(this.fullNameNoAccents_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(296, 6);
            // 
            // batchRenamePhaseShift
            // 
            this.batchRenamePhaseShift.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.batchRenamePhaseShift.Name = "batchRenamePhaseShift";
            this.batchRenamePhaseShift.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.batchRenamePhaseShift.Size = new System.Drawing.Size(299, 22);
            this.batchRenamePhaseShift.Text = "Batch rename Phase Shift folders";
            this.batchRenamePhaseShift.Click += new System.EventHandler(this.batchRenamePhaseShift_Click);
            // 
            // sortingOptionsToolStripMenuItem
            // 
            this.sortingOptionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tryToSortFiles,
            this.tryDetailedSubsorting,
            this.sortByAuthor});
            this.sortingOptionsToolStripMenuItem.Name = "sortingOptionsToolStripMenuItem";
            this.sortingOptionsToolStripMenuItem.Size = new System.Drawing.Size(102, 20);
            this.sortingOptionsToolStripMenuItem.Text = "&Sorting Options";
            // 
            // tryToSortFiles
            // 
            this.tryToSortFiles.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.tryToSortFiles.CheckOnClick = true;
            this.tryToSortFiles.Name = "tryToSortFiles";
            this.tryToSortFiles.Size = new System.Drawing.Size(215, 22);
            this.tryToSortFiles.Text = "Try to sort files by source";
            this.tryToSortFiles.Click += new System.EventHandler(this.sortFilesToolStripMenuItem_Click);
            // 
            // tryDetailedSubsorting
            // 
            this.tryDetailedSubsorting.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.tryDetailedSubsorting.CheckOnClick = true;
            this.tryDetailedSubsorting.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.separateXonlySongs});
            this.tryDetailedSubsorting.Enabled = false;
            this.tryDetailedSubsorting.Name = "tryDetailedSubsorting";
            this.tryDetailedSubsorting.Size = new System.Drawing.Size(215, 22);
            this.tryDetailedSubsorting.Text = "Try detailed sub-sorting";
            this.tryDetailedSubsorting.Click += new System.EventHandler(this.tryDetailedSubsortingToolStripMenuItem_Click);
            // 
            // separateXonlySongs
            // 
            this.separateXonlySongs.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.separateXonlySongs.Checked = true;
            this.separateXonlySongs.CheckOnClick = true;
            this.separateXonlySongs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.separateXonlySongs.Enabled = false;
            this.separateXonlySongs.Name = "separateXonlySongs";
            this.separateXonlySongs.Size = new System.Drawing.Size(191, 22);
            this.separateXonlySongs.Text = "Separate X-only songs";
            // 
            // sortByAuthor
            // 
            this.sortByAuthor.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.sortByAuthor.CheckOnClick = true;
            this.sortByAuthor.Name = "sortByAuthor";
            this.sortByAuthor.Size = new System.Drawing.Size(215, 22);
            this.sortByAuthor.Text = "Try to sort songs by author";
            this.sortByAuthor.Click += new System.EventHandler(this.sortByAuthor_Click);
            // 
            // otherOptionsToolStripMenuItem
            // 
            this.otherOptionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteOlderCopies,
            this.deleteExactCopiesONLY});
            this.otherOptionsToolStripMenuItem.Name = "otherOptionsToolStripMenuItem";
            this.otherOptionsToolStripMenuItem.Size = new System.Drawing.Size(94, 20);
            this.otherOptionsToolStripMenuItem.Text = "&Other Options";
            // 
            // deleteOlderCopies
            // 
            this.deleteOlderCopies.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.deleteOlderCopies.CheckOnClick = true;
            this.deleteOlderCopies.Name = "deleteOlderCopies";
            this.deleteOlderCopies.Size = new System.Drawing.Size(207, 22);
            this.deleteOlderCopies.Text = "Delete older copies";
            this.deleteOlderCopies.Click += new System.EventHandler(this.deleteOlderCopiesToolStripMenuItem_Click);
            // 
            // deleteExactCopiesONLY
            // 
            this.deleteExactCopiesONLY.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.deleteExactCopiesONLY.CheckOnClick = true;
            this.deleteExactCopiesONLY.Name = "deleteExactCopiesONLY";
            this.deleteExactCopiesONLY.Size = new System.Drawing.Size(207, 22);
            this.deleteExactCopiesONLY.Text = "Delete exact copies ONLY";
            this.deleteExactCopiesONLY.Click += new System.EventHandler(this.deleteExactCopiesONLYToolStripMenuItem_Click);
            // 
            // FileRenamer
            // 
            this.FileRenamer.WorkerReportsProgress = true;
            this.FileRenamer.WorkerSupportsCancellation = true;
            this.FileRenamer.DoWork += new System.ComponentModel.DoWorkEventHandler(this.FileRenamer_DoWork);
            this.FileRenamer.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.FileRenamer_RunWorkerCompleted);
            // 
            // picWorking
            // 
            this.picWorking.Image = global::Nautilus.Properties.Resources.working;
            this.picWorking.Location = new System.Drawing.Point(252, 47);
            this.picWorking.Name = "picWorking";
            this.picWorking.Size = new System.Drawing.Size(128, 15);
            this.picWorking.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picWorking.TabIndex = 60;
            this.picWorking.TabStop = false;
            this.picWorking.Visible = false;
            // 
            // folderScanner
            // 
            this.folderScanner.WorkerReportsProgress = true;
            this.folderScanner.WorkerSupportsCancellation = true;
            this.folderScanner.DoWork += new System.ComponentModel.DoWorkEventHandler(this.folderScanner_DoWork);
            this.folderScanner.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.folderScanner_RunWorkerCompleted);
            // 
            // PhaseShiftRenamer
            // 
            this.PhaseShiftRenamer.WorkerReportsProgress = true;
            this.PhaseShiftRenamer.WorkerSupportsCancellation = true;
            this.PhaseShiftRenamer.DoWork += new System.ComponentModel.DoWorkEventHandler(this.PhaseShiftRenamer_DoWork);
            this.PhaseShiftRenamer.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.PhaseShiftRenamer_RunWorkerCompleted);
            // 
            // BatchRenamer
            // 
            this.AllowDrop = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(614, 349);
            this.Controls.Add(this.picPin);
            this.Controls.Add(this.picWorking);
            this.Controls.Add(this.btnBegin);
            this.Controls.Add(this.lstLog);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnFolder);
            this.Controls.Add(this.txtFolder);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "BatchRenamer";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Batch Renamer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BatchRenamer_FormClosing);
            this.Shown += new System.EventHandler(this.BatchRenamer_Shown);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picPin)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picWorking)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnFolder;
        private System.Windows.Forms.TextBox txtFolder;
        private System.Windows.Forms.ListBox lstLog;
        private System.Windows.Forms.Button btnBegin;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem renamingOptionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameFiles;
        private System.Windows.Forms.ToolStripMenuItem replaceSpacesWithUnderscores;
        private System.Windows.Forms.ToolStripMenuItem sortingOptionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tryToSortFiles;
        private System.Windows.Forms.ToolStripMenuItem tryDetailedSubsorting;
        private System.Windows.Forms.ToolStripMenuItem otherOptionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteOlderCopies;
        private System.Windows.Forms.ToolStripMenuItem deleteExactCopiesONLY;
        private System.Windows.Forms.ToolStripMenuItem renameTheArtistSong;
        private System.Windows.Forms.ToolStripMenuItem renameArtistTheSong;
        private System.Windows.Forms.ToolStripMenuItem renameSongTheArtist;
        private System.Windows.Forms.ToolStripMenuItem renameSongArtistThe;
        private System.Windows.Forms.ToolStripMenuItem removeSpacesFromFileName;
        private System.Windows.Forms.ToolStripMenuItem ignoreXboxFilesystemLimitations;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem exportLogFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem separateXonlySongs;
        private System.Windows.Forms.ToolStripMenuItem normalizeFeaturedArtists;
        private System.ComponentModel.BackgroundWorker FileRenamer;
        private System.Windows.Forms.PictureBox picWorking;
        private System.ComponentModel.BackgroundWorker folderScanner;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem batchRenamePhaseShift;
        private System.ComponentModel.BackgroundWorker PhaseShiftRenamer;
        private System.Windows.Forms.PictureBox picPin;
        private System.Windows.Forms.ToolStripMenuItem renameInternalName;
        private System.Windows.Forms.ToolStripMenuItem renameYearArtist;
        private System.Windows.Forms.ToolStripMenuItem renameYearSong;
        private System.Windows.Forms.ToolStripMenuItem sortByAuthor;
        private System.Windows.Forms.ToolStripMenuItem ezloDQMode;
        private System.Windows.Forms.ToolStripMenuItem fullNameNoAccents;
    }
}