namespace Nautilus
{
    partial class RBtoUSB
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openUSB = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeDriveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameDrive = new System.Windows.Forms.ToolStripMenuItem();
            this.copyTU4ToDrive = new System.Windows.Forms.ToolStripMenuItem();
            this.copyTU5ToDriveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkForMisplacedFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.grabSongMetadata = new System.Windows.Forms.ToolStripMenuItem();
            this.autoopenLastUsedDrive = new System.Windows.Forms.ToolStripMenuItem();
            this.showGridlines = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.filterFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filterAll = new System.Windows.Forms.ToolStripMenuItem();
            this.filterCON = new System.Windows.Forms.ToolStripMenuItem();
            this.filterLIVE = new System.Windows.Forms.ToolStripMenuItem();
            this.filterMisc = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleInformationColumnsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.togglePackage = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleFileType = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleFileSize = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleModifiedDate = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleFileName = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleSongArtist = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleSongTitle = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleSongID = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleInternalName = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.resetColumns = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.resetFormSize = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lstFiles = new System.Windows.Forms.ListView();
            this.colPackageName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colFileSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colFileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colArtist = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSong = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSongID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colInternalName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addMagmaFile = new System.Windows.Forms.ToolStripMenuItem();
            this.extractFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lineSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.findFileInFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SendToCONExplorer = new System.Windows.Forms.ToolStripMenuItem();
            this.SendToVisualizer = new System.Windows.Forms.ToolStripMenuItem();
            this.SendToMIDICleaner = new System.Windows.Forms.ToolStripMenuItem();
            this.SendToSongAnalyzer = new System.Windows.Forms.ToolStripMenuItem();
            this.SendToAudioAnalyzer = new System.Windows.Forms.ToolStripMenuItem();
            this.SendToSetlistManager = new System.Windows.Forms.ToolStripMenuItem();
            this.SendToQuickPackEditor = new System.Windows.Forms.ToolStripMenuItem();
            this.exportListToCSV = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.refreshToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.panelInfo = new System.Windows.Forms.Panel();
            this.lblDriveUsed = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblDriveFiles = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblDriveSize = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblDriveName = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblDriveLetter = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.picWorking = new System.Windows.Forms.PictureBox();
            this.driveLoader = new System.ComponentModel.BackgroundWorker();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.picPin = new System.Windows.Forms.PictureBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.fileLoader = new System.ComponentModel.BackgroundWorker();
            this.fileDeleter = new System.ComponentModel.BackgroundWorker();
            this.fileExtractor = new System.ComponentModel.BackgroundWorker();
            this.lblLog = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.menuStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.panelInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picWorking)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPin)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(984, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openUSB,
            this.refreshToolStripMenuItem,
            this.closeDriveToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openUSB
            // 
            this.openUSB.BackColor = System.Drawing.Color.WhiteSmoke;
            this.openUSB.Name = "openUSB";
            this.openUSB.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openUSB.Size = new System.Drawing.Size(208, 22);
            this.openUSB.Text = "Open &USB drive...";
            this.openUSB.Click += new System.EventHandler(this.openUSB_Click);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.BackColor = System.Drawing.Color.WhiteSmoke;
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // closeDriveToolStripMenuItem
            // 
            this.closeDriveToolStripMenuItem.BackColor = System.Drawing.Color.WhiteSmoke;
            this.closeDriveToolStripMenuItem.Name = "closeDriveToolStripMenuItem";
            this.closeDriveToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.closeDriveToolStripMenuItem.Text = "Close drive";
            this.closeDriveToolStripMenuItem.Click += new System.EventHandler(this.closeDriveToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.BackColor = System.Drawing.Color.WhiteSmoke;
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.findFileToolStripMenuItem,
            this.renameDrive,
            this.copyTU4ToDrive,
            this.copyTU5ToDriveToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // findFileToolStripMenuItem
            // 
            this.findFileToolStripMenuItem.BackColor = System.Drawing.Color.WhiteSmoke;
            this.findFileToolStripMenuItem.Name = "findFileToolStripMenuItem";
            this.findFileToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.findFileToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.findFileToolStripMenuItem.Text = "Search...";
            this.findFileToolStripMenuItem.Click += new System.EventHandler(this.findFileToolStripMenuItem_Click);
            // 
            // renameDrive
            // 
            this.renameDrive.BackColor = System.Drawing.Color.WhiteSmoke;
            this.renameDrive.Name = "renameDrive";
            this.renameDrive.Size = new System.Drawing.Size(169, 22);
            this.renameDrive.Text = "Rename drive";
            this.renameDrive.Click += new System.EventHandler(this.renameDrive_Click);
            // 
            // copyTU4ToDrive
            // 
            this.copyTU4ToDrive.BackColor = System.Drawing.Color.WhiteSmoke;
            this.copyTU4ToDrive.Name = "copyTU4ToDrive";
            this.copyTU4ToDrive.Size = new System.Drawing.Size(169, 22);
            this.copyTU4ToDrive.Text = "Copy TU4 to drive";
            this.copyTU4ToDrive.Click += new System.EventHandler(this.copyTU4ToDrive_Click);
            // 
            // copyTU5ToDriveToolStripMenuItem
            // 
            this.copyTU5ToDriveToolStripMenuItem.Name = "copyTU5ToDriveToolStripMenuItem";
            this.copyTU5ToDriveToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.copyTU5ToDriveToolStripMenuItem.Text = "Copy TU5 to drive";
            this.copyTU5ToDriveToolStripMenuItem.Click += new System.EventHandler(this.copyTU5ToDriveToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkForMisplacedFiles,
            this.grabSongMetadata,
            this.autoopenLastUsedDrive,
            this.showGridlines,
            this.toolStripMenuItem4,
            this.filterFilesToolStripMenuItem,
            this.toggleInformationColumnsToolStripMenuItem,
            this.toolStripMenuItem3,
            this.resetFormSize});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "&Options";
            // 
            // checkForMisplacedFiles
            // 
            this.checkForMisplacedFiles.BackColor = System.Drawing.Color.WhiteSmoke;
            this.checkForMisplacedFiles.Checked = true;
            this.checkForMisplacedFiles.CheckOnClick = true;
            this.checkForMisplacedFiles.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkForMisplacedFiles.Name = "checkForMisplacedFiles";
            this.checkForMisplacedFiles.Size = new System.Drawing.Size(226, 22);
            this.checkForMisplacedFiles.Text = "Check for misplaced files";
            // 
            // grabSongMetadata
            // 
            this.grabSongMetadata.BackColor = System.Drawing.Color.WhiteSmoke;
            this.grabSongMetadata.Checked = true;
            this.grabSongMetadata.CheckOnClick = true;
            this.grabSongMetadata.CheckState = System.Windows.Forms.CheckState.Checked;
            this.grabSongMetadata.Name = "grabSongMetadata";
            this.grabSongMetadata.Size = new System.Drawing.Size(226, 22);
            this.grabSongMetadata.Text = "Grab song metadata (slower)";
            this.grabSongMetadata.Click += new System.EventHandler(this.grabSongMetadata_Click);
            // 
            // autoopenLastUsedDrive
            // 
            this.autoopenLastUsedDrive.BackColor = System.Drawing.Color.WhiteSmoke;
            this.autoopenLastUsedDrive.CheckOnClick = true;
            this.autoopenLastUsedDrive.Name = "autoopenLastUsedDrive";
            this.autoopenLastUsedDrive.Size = new System.Drawing.Size(226, 22);
            this.autoopenLastUsedDrive.Text = "Auto-open last used drive";
            // 
            // showGridlines
            // 
            this.showGridlines.BackColor = System.Drawing.Color.WhiteSmoke;
            this.showGridlines.CheckOnClick = true;
            this.showGridlines.Name = "showGridlines";
            this.showGridlines.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.showGridlines.Size = new System.Drawing.Size(226, 22);
            this.showGridlines.Text = "Show gridlines";
            this.showGridlines.Click += new System.EventHandler(this.showGridlines_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(223, 6);
            // 
            // filterFilesToolStripMenuItem
            // 
            this.filterFilesToolStripMenuItem.BackColor = System.Drawing.Color.WhiteSmoke;
            this.filterFilesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filterAll,
            this.filterCON,
            this.filterLIVE,
            this.filterMisc});
            this.filterFilesToolStripMenuItem.Name = "filterFilesToolStripMenuItem";
            this.filterFilesToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            this.filterFilesToolStripMenuItem.Text = "Filter files";
            // 
            // filterAll
            // 
            this.filterAll.BackColor = System.Drawing.Color.WhiteSmoke;
            this.filterAll.Checked = true;
            this.filterAll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.filterAll.Name = "filterAll";
            this.filterAll.Size = new System.Drawing.Size(152, 22);
            this.filterAll.Text = "All files";
            this.filterAll.Click += new System.EventHandler(this.filterAll_Click);
            // 
            // filterCON
            // 
            this.filterCON.BackColor = System.Drawing.Color.WhiteSmoke;
            this.filterCON.Name = "filterCON";
            this.filterCON.Size = new System.Drawing.Size(152, 22);
            this.filterCON.Text = "Only CON files";
            this.filterCON.Click += new System.EventHandler(this.filterAll_Click);
            // 
            // filterLIVE
            // 
            this.filterLIVE.BackColor = System.Drawing.Color.WhiteSmoke;
            this.filterLIVE.Name = "filterLIVE";
            this.filterLIVE.Size = new System.Drawing.Size(152, 22);
            this.filterLIVE.Text = "Only LIVE files";
            this.filterLIVE.Click += new System.EventHandler(this.filterAll_Click);
            // 
            // filterMisc
            // 
            this.filterMisc.BackColor = System.Drawing.Color.WhiteSmoke;
            this.filterMisc.Name = "filterMisc";
            this.filterMisc.Size = new System.Drawing.Size(152, 22);
            this.filterMisc.Text = "Only Misc files";
            this.filterMisc.Click += new System.EventHandler(this.filterAll_Click);
            // 
            // toggleInformationColumnsToolStripMenuItem
            // 
            this.toggleInformationColumnsToolStripMenuItem.BackColor = System.Drawing.Color.WhiteSmoke;
            this.toggleInformationColumnsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.togglePackage,
            this.toggleFileType,
            this.toggleFileSize,
            this.toggleModifiedDate,
            this.toggleFileName,
            this.toggleSongArtist,
            this.toggleSongTitle,
            this.toggleSongID,
            this.toggleInternalName,
            this.toolStripMenuItem2,
            this.resetColumns});
            this.toggleInformationColumnsToolStripMenuItem.Name = "toggleInformationColumnsToolStripMenuItem";
            this.toggleInformationColumnsToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            this.toggleInformationColumnsToolStripMenuItem.Text = "Toggle information columns";
            // 
            // togglePackage
            // 
            this.togglePackage.BackColor = System.Drawing.Color.WhiteSmoke;
            this.togglePackage.Checked = true;
            this.togglePackage.CheckOnClick = true;
            this.togglePackage.CheckState = System.Windows.Forms.CheckState.Checked;
            this.togglePackage.Name = "togglePackage";
            this.togglePackage.Size = new System.Drawing.Size(153, 22);
            this.togglePackage.Text = "Package Name";
            this.togglePackage.Click += new System.EventHandler(this.UpdateToggles);
            // 
            // toggleFileType
            // 
            this.toggleFileType.BackColor = System.Drawing.Color.WhiteSmoke;
            this.toggleFileType.Checked = true;
            this.toggleFileType.CheckOnClick = true;
            this.toggleFileType.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toggleFileType.Name = "toggleFileType";
            this.toggleFileType.Size = new System.Drawing.Size(153, 22);
            this.toggleFileType.Text = "File Type";
            this.toggleFileType.Click += new System.EventHandler(this.UpdateToggles);
            // 
            // toggleFileSize
            // 
            this.toggleFileSize.BackColor = System.Drawing.Color.WhiteSmoke;
            this.toggleFileSize.Checked = true;
            this.toggleFileSize.CheckOnClick = true;
            this.toggleFileSize.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toggleFileSize.Name = "toggleFileSize";
            this.toggleFileSize.Size = new System.Drawing.Size(153, 22);
            this.toggleFileSize.Text = "File Size";
            this.toggleFileSize.Click += new System.EventHandler(this.UpdateToggles);
            // 
            // toggleModifiedDate
            // 
            this.toggleModifiedDate.BackColor = System.Drawing.Color.WhiteSmoke;
            this.toggleModifiedDate.Checked = true;
            this.toggleModifiedDate.CheckOnClick = true;
            this.toggleModifiedDate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toggleModifiedDate.Name = "toggleModifiedDate";
            this.toggleModifiedDate.Size = new System.Drawing.Size(153, 22);
            this.toggleModifiedDate.Text = "Modified Date";
            this.toggleModifiedDate.Click += new System.EventHandler(this.UpdateToggles);
            // 
            // toggleFileName
            // 
            this.toggleFileName.BackColor = System.Drawing.Color.WhiteSmoke;
            this.toggleFileName.Checked = true;
            this.toggleFileName.CheckOnClick = true;
            this.toggleFileName.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toggleFileName.Name = "toggleFileName";
            this.toggleFileName.Size = new System.Drawing.Size(153, 22);
            this.toggleFileName.Text = "File Name";
            this.toggleFileName.Click += new System.EventHandler(this.UpdateToggles);
            // 
            // toggleSongArtist
            // 
            this.toggleSongArtist.BackColor = System.Drawing.Color.WhiteSmoke;
            this.toggleSongArtist.Checked = true;
            this.toggleSongArtist.CheckOnClick = true;
            this.toggleSongArtist.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toggleSongArtist.Name = "toggleSongArtist";
            this.toggleSongArtist.Size = new System.Drawing.Size(153, 22);
            this.toggleSongArtist.Text = "Song Artist";
            this.toggleSongArtist.Click += new System.EventHandler(this.UpdateToggles);
            // 
            // toggleSongTitle
            // 
            this.toggleSongTitle.BackColor = System.Drawing.Color.WhiteSmoke;
            this.toggleSongTitle.Checked = true;
            this.toggleSongTitle.CheckOnClick = true;
            this.toggleSongTitle.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toggleSongTitle.Name = "toggleSongTitle";
            this.toggleSongTitle.Size = new System.Drawing.Size(153, 22);
            this.toggleSongTitle.Text = "Song Title";
            this.toggleSongTitle.Click += new System.EventHandler(this.UpdateToggles);
            // 
            // toggleSongID
            // 
            this.toggleSongID.BackColor = System.Drawing.Color.WhiteSmoke;
            this.toggleSongID.Checked = true;
            this.toggleSongID.CheckOnClick = true;
            this.toggleSongID.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toggleSongID.Name = "toggleSongID";
            this.toggleSongID.Size = new System.Drawing.Size(153, 22);
            this.toggleSongID.Text = "Song ID";
            this.toggleSongID.Click += new System.EventHandler(this.UpdateToggles);
            // 
            // toggleInternalName
            // 
            this.toggleInternalName.BackColor = System.Drawing.Color.WhiteSmoke;
            this.toggleInternalName.Checked = true;
            this.toggleInternalName.CheckOnClick = true;
            this.toggleInternalName.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toggleInternalName.Name = "toggleInternalName";
            this.toggleInternalName.Size = new System.Drawing.Size(153, 22);
            this.toggleInternalName.Text = "Internal Name";
            this.toggleInternalName.Click += new System.EventHandler(this.UpdateToggles);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(150, 6);
            // 
            // resetColumns
            // 
            this.resetColumns.BackColor = System.Drawing.Color.WhiteSmoke;
            this.resetColumns.Name = "resetColumns";
            this.resetColumns.Size = new System.Drawing.Size(153, 22);
            this.resetColumns.Text = "Reset columns";
            this.resetColumns.Click += new System.EventHandler(this.resetColumns_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(223, 6);
            // 
            // resetFormSize
            // 
            this.resetFormSize.BackColor = System.Drawing.Color.WhiteSmoke;
            this.resetFormSize.Name = "resetFormSize";
            this.resetFormSize.Size = new System.Drawing.Size(226, 22);
            this.resetFormSize.Text = "Reset RBtoUSB form";
            this.resetFormSize.Click += new System.EventHandler(this.resetFormSize_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // lstFiles
            // 
            this.lstFiles.AllowColumnReorder = true;
            this.lstFiles.AllowDrop = true;
            this.lstFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstFiles.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.lstFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colPackageName,
            this.colType,
            this.colFileSize,
            this.colDate,
            this.colFileName,
            this.colArtist,
            this.colSong,
            this.colSongID,
            this.colInternalName});
            this.lstFiles.ContextMenuStrip = this.contextMenuStrip1;
            this.lstFiles.FullRowSelect = true;
            this.lstFiles.HideSelection = false;
            this.lstFiles.Location = new System.Drawing.Point(-1, 24);
            this.lstFiles.Name = "lstFiles";
            this.lstFiles.Size = new System.Drawing.Size(986, 592);
            this.lstFiles.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lstFiles.TabIndex = 1;
            this.lstFiles.UseCompatibleStateImageBehavior = false;
            this.lstFiles.View = System.Windows.Forms.View.Details;
            this.lstFiles.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lstFiles_ColumnClick);
            this.lstFiles.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.lstFiles_ItemDrag);
            this.lstFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.lstFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            this.lstFiles.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lstFiles_KeyUp);
            this.lstFiles.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstFiles_MouseDoubleClick);
            // 
            // colPackageName
            // 
            this.colPackageName.Text = "Package Name";
            this.colPackageName.Width = 310;
            // 
            // colType
            // 
            this.colType.Text = "File Type";
            this.colType.Width = 80;
            // 
            // colFileSize
            // 
            this.colFileSize.Tag = "FileSize";
            this.colFileSize.Text = "File Size";
            this.colFileSize.Width = 120;
            // 
            // colDate
            // 
            this.colDate.Tag = "DateTime";
            this.colDate.Text = "Modified Date";
            this.colDate.Width = 130;
            // 
            // colFileName
            // 
            this.colFileName.Text = "File Name";
            this.colFileName.Width = 310;
            // 
            // colArtist
            // 
            this.colArtist.Text = "Song Artist";
            this.colArtist.Width = 200;
            // 
            // colSong
            // 
            this.colSong.Text = "Song Title";
            this.colSong.Width = 200;
            // 
            // colSongID
            // 
            this.colSongID.Text = "Song ID";
            this.colSongID.Width = 100;
            // 
            // colInternalName
            // 
            this.colInternalName.Text = "Internal Name";
            this.colInternalName.Width = 100;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addFileToolStripMenuItem,
            this.addMagmaFile,
            this.extractFileToolStripMenuItem,
            this.deleteFileToolStripMenuItem,
            this.lineSeparator,
            this.findFileInFolderToolStripMenuItem,
            this.sendToToolStripMenuItem,
            this.exportListToCSV,
            this.toolStripMenuItem5,
            this.refreshToolStripMenuItem1});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.ShowImageMargin = false;
            this.contextMenuStrip1.Size = new System.Drawing.Size(197, 192);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // addFileToolStripMenuItem
            // 
            this.addFileToolStripMenuItem.BackColor = System.Drawing.Color.WhiteSmoke;
            this.addFileToolStripMenuItem.Name = "addFileToolStripMenuItem";
            this.addFileToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.addFileToolStripMenuItem.Text = "Add files...";
            this.addFileToolStripMenuItem.Click += new System.EventHandler(this.addFileToolStripMenuItem_Click);
            // 
            // addMagmaFile
            // 
            this.addMagmaFile.BackColor = System.Drawing.Color.WhiteSmoke;
            this.addMagmaFile.Name = "addMagmaFile";
            this.addMagmaFile.Size = new System.Drawing.Size(196, 22);
            this.addMagmaFile.Text = "Add file(s) from Magma: C3";
            this.addMagmaFile.Click += new System.EventHandler(this.addMagmaFile_Click);
            // 
            // extractFileToolStripMenuItem
            // 
            this.extractFileToolStripMenuItem.BackColor = System.Drawing.Color.WhiteSmoke;
            this.extractFileToolStripMenuItem.Name = "extractFileToolStripMenuItem";
            this.extractFileToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.extractFileToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.extractFileToolStripMenuItem.Text = "Extract selected file";
            this.extractFileToolStripMenuItem.Click += new System.EventHandler(this.extractFileToolStripMenuItem_Click);
            // 
            // deleteFileToolStripMenuItem
            // 
            this.deleteFileToolStripMenuItem.BackColor = System.Drawing.Color.WhiteSmoke;
            this.deleteFileToolStripMenuItem.Name = "deleteFileToolStripMenuItem";
            this.deleteFileToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.deleteFileToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.deleteFileToolStripMenuItem.Text = "Delete selected file";
            this.deleteFileToolStripMenuItem.Click += new System.EventHandler(this.deleteFileToolStripMenuItem_Click);
            // 
            // lineSeparator
            // 
            this.lineSeparator.Name = "lineSeparator";
            this.lineSeparator.Size = new System.Drawing.Size(193, 6);
            // 
            // findFileInFolderToolStripMenuItem
            // 
            this.findFileInFolderToolStripMenuItem.BackColor = System.Drawing.Color.WhiteSmoke;
            this.findFileInFolderToolStripMenuItem.Name = "findFileInFolderToolStripMenuItem";
            this.findFileInFolderToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.findFileInFolderToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.findFileInFolderToolStripMenuItem.Text = "Locate file in drive";
            this.findFileInFolderToolStripMenuItem.Click += new System.EventHandler(this.findFileInFolderToolStripMenuItem_Click);
            // 
            // sendToToolStripMenuItem
            // 
            this.sendToToolStripMenuItem.BackColor = System.Drawing.Color.WhiteSmoke;
            this.sendToToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SendToCONExplorer,
            this.SendToVisualizer,
            this.SendToMIDICleaner,
            this.SendToSongAnalyzer,
            this.SendToAudioAnalyzer,
            this.SendToSetlistManager,
            this.SendToQuickPackEditor});
            this.sendToToolStripMenuItem.Name = "sendToToolStripMenuItem";
            this.sendToToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.sendToToolStripMenuItem.Text = "Send to...";
            // 
            // SendToCONExplorer
            // 
            this.SendToCONExplorer.BackColor = System.Drawing.Color.WhiteSmoke;
            this.SendToCONExplorer.Name = "SendToCONExplorer";
            this.SendToCONExplorer.Size = new System.Drawing.Size(167, 22);
            this.SendToCONExplorer.Text = "CON Explorer";
            this.SendToCONExplorer.Click += new System.EventHandler(this.SendToCONExplorer_Click);
            // 
            // SendToVisualizer
            // 
            this.SendToVisualizer.BackColor = System.Drawing.Color.WhiteSmoke;
            this.SendToVisualizer.Name = "SendToVisualizer";
            this.SendToVisualizer.Size = new System.Drawing.Size(167, 22);
            this.SendToVisualizer.Text = "Visualizer";
            this.SendToVisualizer.Click += new System.EventHandler(this.SendToVisualizer_Click);
            // 
            // SendToMIDICleaner
            // 
            this.SendToMIDICleaner.BackColor = System.Drawing.Color.WhiteSmoke;
            this.SendToMIDICleaner.Name = "SendToMIDICleaner";
            this.SendToMIDICleaner.Size = new System.Drawing.Size(167, 22);
            this.SendToMIDICleaner.Text = "MIDI Cleaner";
            this.SendToMIDICleaner.Click += new System.EventHandler(this.SendToMIDICleaner_Click);
            // 
            // SendToSongAnalyzer
            // 
            this.SendToSongAnalyzer.BackColor = System.Drawing.Color.WhiteSmoke;
            this.SendToSongAnalyzer.Name = "SendToSongAnalyzer";
            this.SendToSongAnalyzer.Size = new System.Drawing.Size(167, 22);
            this.SendToSongAnalyzer.Text = "Song Analyzer";
            this.SendToSongAnalyzer.Click += new System.EventHandler(this.SendToSongAnalyzer_Click);
            // 
            // SendToAudioAnalyzer
            // 
            this.SendToAudioAnalyzer.BackColor = System.Drawing.Color.WhiteSmoke;
            this.SendToAudioAnalyzer.Name = "SendToAudioAnalyzer";
            this.SendToAudioAnalyzer.Size = new System.Drawing.Size(167, 22);
            this.SendToAudioAnalyzer.Text = "Audio Analyzer";
            this.SendToAudioAnalyzer.Click += new System.EventHandler(this.SendToAudioAnalyzer_Click);
            // 
            // SendToSetlistManager
            // 
            this.SendToSetlistManager.BackColor = System.Drawing.Color.WhiteSmoke;
            this.SendToSetlistManager.Name = "SendToSetlistManager";
            this.SendToSetlistManager.Size = new System.Drawing.Size(167, 22);
            this.SendToSetlistManager.Text = "Setlist Manager";
            this.SendToSetlistManager.Click += new System.EventHandler(this.SendToSetlistManager_Click);
            // 
            // SendToQuickPackEditor
            // 
            this.SendToQuickPackEditor.BackColor = System.Drawing.Color.WhiteSmoke;
            this.SendToQuickPackEditor.Name = "SendToQuickPackEditor";
            this.SendToQuickPackEditor.Size = new System.Drawing.Size(167, 22);
            this.SendToQuickPackEditor.Text = "Quick Pack Editor";
            this.SendToQuickPackEditor.Click += new System.EventHandler(this.SendToQuickPackEditor_Click);
            // 
            // exportListToCSV
            // 
            this.exportListToCSV.BackColor = System.Drawing.Color.WhiteSmoke;
            this.exportListToCSV.Name = "exportListToCSV";
            this.exportListToCSV.Size = new System.Drawing.Size(196, 22);
            this.exportListToCSV.Text = "Export list to CSV";
            this.exportListToCSV.Click += new System.EventHandler(this.exportListToCSV_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(193, 6);
            // 
            // refreshToolStripMenuItem1
            // 
            this.refreshToolStripMenuItem1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.refreshToolStripMenuItem1.Name = "refreshToolStripMenuItem1";
            this.refreshToolStripMenuItem1.Size = new System.Drawing.Size(196, 22);
            this.refreshToolStripMenuItem1.Text = "Refresh...";
            this.refreshToolStripMenuItem1.Click += new System.EventHandler(this.refreshToolStripMenuItem1_Click);
            // 
            // panelInfo
            // 
            this.panelInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelInfo.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelInfo.Controls.Add(this.lblDriveUsed);
            this.panelInfo.Controls.Add(this.label6);
            this.panelInfo.Controls.Add(this.lblDriveFiles);
            this.panelInfo.Controls.Add(this.label5);
            this.panelInfo.Controls.Add(this.lblDriveSize);
            this.panelInfo.Controls.Add(this.label3);
            this.panelInfo.Controls.Add(this.lblDriveName);
            this.panelInfo.Controls.Add(this.label2);
            this.panelInfo.Controls.Add(this.lblDriveLetter);
            this.panelInfo.Controls.Add(this.label1);
            this.panelInfo.Location = new System.Drawing.Point(-1, 636);
            this.panelInfo.Name = "panelInfo";
            this.panelInfo.Size = new System.Drawing.Size(986, 27);
            this.panelInfo.TabIndex = 2;
            // 
            // lblDriveUsed
            // 
            this.lblDriveUsed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDriveUsed.BackColor = System.Drawing.Color.Transparent;
            this.lblDriveUsed.Location = new System.Drawing.Point(685, 6);
            this.lblDriveUsed.Name = "lblDriveUsed";
            this.lblDriveUsed.Size = new System.Drawing.Size(97, 13);
            this.lblDriveUsed.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(648, 6);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(31, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Free:";
            // 
            // lblDriveFiles
            // 
            this.lblDriveFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDriveFiles.BackColor = System.Drawing.Color.Transparent;
            this.lblDriveFiles.Location = new System.Drawing.Point(880, 6);
            this.lblDriveFiles.Name = "lblDriveFiles";
            this.lblDriveFiles.Size = new System.Drawing.Size(97, 13);
            this.lblDriveFiles.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(825, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "RB Files:";
            // 
            // lblDriveSize
            // 
            this.lblDriveSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDriveSize.BackColor = System.Drawing.Color.Transparent;
            this.lblDriveSize.Location = new System.Drawing.Point(502, 6);
            this.lblDriveSize.Name = "lblDriveSize";
            this.lblDriveSize.Size = new System.Drawing.Size(97, 13);
            this.lblDriveSize.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(466, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Size:";
            // 
            // lblDriveName
            // 
            this.lblDriveName.AutoEllipsis = true;
            this.lblDriveName.AutoSize = true;
            this.lblDriveName.BackColor = System.Drawing.Color.Transparent;
            this.lblDriveName.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblDriveName.Location = new System.Drawing.Point(151, 6);
            this.lblDriveName.Name = "lblDriveName";
            this.lblDriveName.Size = new System.Drawing.Size(0, 13);
            this.lblDriveName.TabIndex = 3;
            this.toolTip1.SetToolTip(this.lblDriveName, "Click to change");
            this.lblDriveName.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lblDriveName_MouseClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(107, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Name:";
            // 
            // lblDriveLetter
            // 
            this.lblDriveLetter.BackColor = System.Drawing.Color.Transparent;
            this.lblDriveLetter.Location = new System.Drawing.Point(44, 6);
            this.lblDriveLetter.Name = "lblDriveLetter";
            this.lblDriveLetter.Size = new System.Drawing.Size(25, 13);
            this.lblDriveLetter.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Drive:";
            // 
            // picWorking
            // 
            this.picWorking.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picWorking.Image = global::Nautilus.Properties.Resources.working;
            this.picWorking.Location = new System.Drawing.Point(827, 4);
            this.picWorking.Name = "picWorking";
            this.picWorking.Size = new System.Drawing.Size(128, 15);
            this.picWorking.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picWorking.TabIndex = 59;
            this.picWorking.TabStop = false;
            this.picWorking.Visible = false;
            this.picWorking.VisibleChanged += new System.EventHandler(this.picWorking_VisibleChanged);
            // 
            // driveLoader
            // 
            this.driveLoader.WorkerReportsProgress = true;
            this.driveLoader.WorkerSupportsCancellation = true;
            this.driveLoader.DoWork += new System.ComponentModel.DoWorkEventHandler(this.driveLoader_DoWork);
            this.driveLoader.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.driveLoader_RunWorkerCompleted);
            // 
            // picPin
            // 
            this.picPin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picPin.BackColor = System.Drawing.Color.Transparent;
            this.picPin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picPin.Image = global::Nautilus.Properties.Resources.unpinned;
            this.picPin.Location = new System.Drawing.Point(961, 2);
            this.picPin.Name = "picPin";
            this.picPin.Size = new System.Drawing.Size(20, 20);
            this.picPin.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picPin.TabIndex = 64;
            this.picPin.TabStop = false;
            this.picPin.Tag = "unpinned";
            this.toolTip1.SetToolTip(this.picPin, "Click to pin on top");
            this.picPin.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picPin_MouseClick);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BackColor = System.Drawing.Color.Red;
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(909, 614);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 66;
            this.btnCancel.Text = "Cancel";
            this.toolTip1.SetToolTip(this.btnCancel, "Click to cancel operation");
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // fileLoader
            // 
            this.fileLoader.WorkerReportsProgress = true;
            this.fileLoader.WorkerSupportsCancellation = true;
            this.fileLoader.DoWork += new System.ComponentModel.DoWorkEventHandler(this.fileLoader_DoWork);
            this.fileLoader.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.fileLoader_RunWorkerCompleted);
            // 
            // fileDeleter
            // 
            this.fileDeleter.WorkerReportsProgress = true;
            this.fileDeleter.WorkerSupportsCancellation = true;
            this.fileDeleter.DoWork += new System.ComponentModel.DoWorkEventHandler(this.fileDeleter_DoWork);
            this.fileDeleter.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.fileDeleter_RunWorkerCompleted);
            // 
            // fileExtractor
            // 
            this.fileExtractor.DoWork += new System.ComponentModel.DoWorkEventHandler(this.fileExtractor_DoWork);
            this.fileExtractor.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.fileExtractor_RunWorkerCompleted);
            // 
            // lblLog
            // 
            this.lblLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLog.BackColor = System.Drawing.Color.WhiteSmoke;
            this.lblLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblLog.Location = new System.Drawing.Point(-1, 615);
            this.lblLog.Name = "lblLog";
            this.lblLog.Size = new System.Drawing.Size(986, 22);
            this.lblLog.TabIndex = 60;
            this.lblLog.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(699, 4);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(256, 15);
            this.progressBar.TabIndex = 65;
            this.progressBar.Visible = false;
            this.progressBar.VisibleChanged += new System.EventHandler(this.progressBar_VisibleChanged);
            // 
            // RBtoUSB
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(984, 661);
            this.Controls.Add(this.picWorking);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.picPin);
            this.Controls.Add(this.lblLog);
            this.Controls.Add(this.panelInfo);
            this.Controls.Add(this.lstFiles);
            this.Controls.Add(this.menuStrip1);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "RBtoUSB";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "RBtoUSB";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.USBnator_FormClosing);
            this.Shown += new System.EventHandler(this.USBnator_Shown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.USBnator_KeyUp);
            this.Resize += new System.EventHandler(this.USBnator_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.panelInfo.ResumeLayout(false);
            this.panelInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picWorking)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPin)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openUSB;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader colFileName;
        private System.Windows.Forms.ColumnHeader colPackageName;
        private System.Windows.Forms.ColumnHeader colFileSize;
        private System.Windows.Forms.ColumnHeader colDate;
        private System.Windows.Forms.Panel panelInfo;
        private System.Windows.Forms.Label lblDriveUsed;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblDriveFiles;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblDriveSize;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblDriveName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblDriveLetter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox picWorking;
        private System.ComponentModel.BackgroundWorker driveLoader;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ColumnHeader colType;
        private System.Windows.Forms.ToolStripMenuItem checkForMisplacedFiles;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem addFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extractFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator lineSeparator;
        private System.Windows.Forms.ToolStripMenuItem findFileInFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sendToToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SendToCONExplorer;
        private System.Windows.Forms.ToolStripMenuItem SendToSongAnalyzer;
        private System.Windows.Forms.ToolStripMenuItem SendToVisualizer;
        private System.Windows.Forms.ToolStripMenuItem SendToMIDICleaner;
        private System.Windows.Forms.ToolStripMenuItem renameDrive;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker fileLoader;
        private System.Windows.Forms.ToolStripMenuItem copyTU4ToDrive;
        private System.ComponentModel.BackgroundWorker fileDeleter;
        private System.ComponentModel.BackgroundWorker fileExtractor;
        //private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem SendToAudioAnalyzer;
        public System.Windows.Forms.ListView lstFiles;
        private System.Windows.Forms.ToolStripMenuItem SendToSetlistManager;
        private System.Windows.Forms.ToolStripMenuItem showGridlines;
        private System.Windows.Forms.ToolStripMenuItem findFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoopenLastUsedDrive;
        private System.Windows.Forms.ToolStripMenuItem addMagmaFile;
        private System.Windows.Forms.ColumnHeader colArtist;
        private System.Windows.Forms.ColumnHeader colSong;
        private System.Windows.Forms.ColumnHeader colSongID;
        private System.Windows.Forms.ToolStripMenuItem resetFormSize;
        private System.Windows.Forms.ToolStripMenuItem toggleInformationColumnsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem togglePackage;
        private System.Windows.Forms.ToolStripMenuItem toggleFileType;
        private System.Windows.Forms.ToolStripMenuItem toggleFileSize;
        private System.Windows.Forms.ToolStripMenuItem toggleModifiedDate;
        private System.Windows.Forms.ToolStripMenuItem toggleFileName;
        private System.Windows.Forms.ToolStripMenuItem toggleSongArtist;
        private System.Windows.Forms.ToolStripMenuItem toggleSongTitle;
        private System.Windows.Forms.ToolStripMenuItem toggleSongID;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem resetColumns;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem SendToQuickPackEditor;
        private System.Windows.Forms.ToolStripMenuItem filterFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filterAll;
        private System.Windows.Forms.ToolStripMenuItem filterCON;
        private System.Windows.Forms.ToolStripMenuItem filterLIVE;
        private System.Windows.Forms.ToolStripMenuItem filterMisc;
        private System.Windows.Forms.Label lblLog;
        private System.Windows.Forms.ToolStripMenuItem closeDriveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem grabSongMetadata;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem toggleInternalName;
        private System.Windows.Forms.ColumnHeader colInternalName;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem1;
        private System.Windows.Forms.PictureBox picPin;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ToolStripMenuItem exportListToCSV;
        private System.Windows.Forms.ToolStripMenuItem copyTU5ToDriveToolStripMenuItem;
    }
}