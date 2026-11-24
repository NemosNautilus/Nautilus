namespace Nautilus
{
    partial class QuickPackEditor
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
            this.lstLog = new System.Windows.Forms.ListBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exportLogFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.lstSongs = new System.Windows.Forms.ListBox();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.sortByDTAEntry = new System.Windows.Forms.ToolStripMenuItem();
            this.sortByArtistSong = new System.Windows.Forms.ToolStripMenuItem();
            this.sortBySongArtist = new System.Windows.Forms.ToolStripMenuItem();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnRestore = new System.Windows.Forms.Button();
            this.btnRePack = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.radioDTA = new System.Windows.Forms.RadioButton();
            this.radioPack = new System.Windows.Forms.RadioButton();
            this.chkBackup = new System.Windows.Forms.CheckBox();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnDePack = new System.Windows.Forms.Button();
            this.PackageImage = new System.Windows.Forms.PictureBox();
            this.ContentImage = new System.Windows.Forms.PictureBox();
            this.picPin = new System.Windows.Forms.PictureBox();
            this.btnExtract = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.openFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.batchDePACKToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setGameIDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rockBandToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rockBand2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rockBand3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dePACKOptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.usePackThumbnail = new System.Windows.Forms.ToolStripMenuItem();
            this.useSongAlbumArt = new System.Windows.Forms.ToolStripMenuItem();
            this.openFolderAfterDePACK = new System.Windows.Forms.ToolStripMenuItem();
            this.useArtistSongShortName = new System.Windows.Forms.ToolStripMenuItem();
            this.signingOptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.signAsCON = new System.Windows.Forms.ToolStripMenuItem();
            this.signAsLIVE = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.picWorking = new System.Windows.Forms.PictureBox();
            this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorker3 = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorker4 = new System.ComponentModel.BackgroundWorker();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PackageImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ContentImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPin)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picWorking)).BeginInit();
            this.SuspendLayout();
            // 
            // lstLog
            // 
            this.lstLog.AllowDrop = true;
            this.lstLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstLog.BackColor = System.Drawing.Color.White;
            this.lstLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstLog.ContextMenuStrip = this.contextMenuStrip1;
            this.lstLog.FormattingEnabled = true;
            this.lstLog.HorizontalScrollbar = true;
            this.lstLog.Location = new System.Drawing.Point(4, 397);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(563, 158);
            this.lstLog.TabIndex = 12;
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
            // lstSongs
            // 
            this.lstSongs.ContextMenuStrip = this.contextMenuStrip2;
            this.lstSongs.FormattingEnabled = true;
            this.lstSongs.Items.AddRange(new object[] {
            "Pack contents will be shown here"});
            this.lstSongs.Location = new System.Drawing.Point(4, 27);
            this.lstSongs.Name = "lstSongs";
            this.lstSongs.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstSongs.Size = new System.Drawing.Size(478, 173);
            this.lstSongs.TabIndex = 36;
            this.toolTip1.SetToolTip(this.lstSongs, "These are the songs found in your pack");
            this.lstSongs.SelectedIndexChanged += new System.EventHandler(this.lstSongs_SelectedIndexChanged);
            this.lstSongs.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.lstSongs.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sortByDTAEntry,
            this.sortByArtistSong,
            this.sortBySongArtist});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.ShowImageMargin = false;
            this.contextMenuStrip2.Size = new System.Drawing.Size(156, 70);
            this.contextMenuStrip2.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip2_Opening);
            // 
            // sortByDTAEntry
            // 
            this.sortByDTAEntry.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.sortByDTAEntry.Name = "sortByDTAEntry";
            this.sortByDTAEntry.Size = new System.Drawing.Size(155, 22);
            this.sortByDTAEntry.Text = "Sort by DTA Entry #";
            this.sortByDTAEntry.Click += new System.EventHandler(this.sortByDTAEntry_Click);
            // 
            // sortByArtistSong
            // 
            this.sortByArtistSong.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.sortByArtistSong.Name = "sortByArtistSong";
            this.sortByArtistSong.Size = new System.Drawing.Size(155, 22);
            this.sortByArtistSong.Text = "Sort by Artist - Song";
            this.sortByArtistSong.Click += new System.EventHandler(this.sortByArtistSong_Click);
            // 
            // sortBySongArtist
            // 
            this.sortBySongArtist.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.sortBySongArtist.Name = "sortBySongArtist";
            this.sortBySongArtist.Size = new System.Drawing.Size(155, 22);
            this.sortBySongArtist.Text = "Sort by Song - Artist";
            this.sortBySongArtist.Click += new System.EventHandler(this.sortBySongArtist_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(195)))), ((int)(((byte)(73)))));
            this.btnRemove.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRemove.Enabled = false;
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemove.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRemove.ForeColor = System.Drawing.Color.White;
            this.btnRemove.Location = new System.Drawing.Point(488, 112);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(79, 41);
            this.btnRemove.TabIndex = 37;
            this.btnRemove.Text = "Remove selected";
            this.toolTip1.SetToolTip(this.btnRemove, "Remove the selected song(s) from the pack");
            this.btnRemove.UseVisualStyleBackColor = false;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnRestore
            // 
            this.btnRestore.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(195)))), ((int)(((byte)(73)))));
            this.btnRestore.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRestore.Enabled = false;
            this.btnRestore.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRestore.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRestore.ForeColor = System.Drawing.Color.White;
            this.btnRestore.Location = new System.Drawing.Point(488, 159);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(79, 41);
            this.btnRestore.TabIndex = 38;
            this.btnRestore.Text = "Restore contents";
            this.toolTip1.SetToolTip(this.btnRestore, "Restore all the songs that were in the pack initially");
            this.btnRestore.UseVisualStyleBackColor = false;
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            // 
            // btnRePack
            // 
            this.btnRePack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(195)))), ((int)(((byte)(73)))));
            this.btnRePack.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRePack.Enabled = false;
            this.btnRePack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRePack.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRePack.ForeColor = System.Drawing.Color.White;
            this.btnRePack.Location = new System.Drawing.Point(488, 260);
            this.btnRePack.Name = "btnRePack";
            this.btnRePack.Size = new System.Drawing.Size(79, 27);
            this.btnRePack.TabIndex = 39;
            this.btnRePack.Text = "rePACK";
            this.toolTip1.SetToolTip(this.btnRePack, "Click to begin rePACKing process");
            this.btnRePack.UseVisualStyleBackColor = false;
            this.btnRePack.Click += new System.EventHandler(this.btnBegin_Click);
            // 
            // btnReset
            // 
            this.btnReset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(195)))), ((int)(((byte)(73)))));
            this.btnReset.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReset.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReset.ForeColor = System.Drawing.Color.White;
            this.btnReset.Location = new System.Drawing.Point(488, 351);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(79, 27);
            this.btnReset.TabIndex = 40;
            this.btnReset.Text = "Reset";
            this.toolTip1.SetToolTip(this.btnReset, "Reset this program to edit another pack");
            this.btnReset.UseVisualStyleBackColor = false;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // radioDTA
            // 
            this.radioDTA.AutoSize = true;
            this.radioDTA.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioDTA.Location = new System.Drawing.Point(12, 211);
            this.radioDTA.Name = "radioDTA";
            this.radioDTA.Size = new System.Drawing.Size(124, 17);
            this.radioDTA.TabIndex = 41;
            this.radioDTA.Text = "Edit DTA entries only";
            this.toolTip1.SetToolTip(this.radioDTA, "Choose this for faster process");
            this.radioDTA.UseVisualStyleBackColor = true;
            // 
            // radioPack
            // 
            this.radioPack.AutoSize = true;
            this.radioPack.Checked = true;
            this.radioPack.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioPack.Location = new System.Drawing.Point(158, 211);
            this.radioPack.Name = "radioPack";
            this.radioPack.Size = new System.Drawing.Size(194, 17);
            this.radioPack.TabIndex = 42;
            this.radioPack.TabStop = true;
            this.radioPack.Text = "Edit DTA entries and pack contents";
            this.toolTip1.SetToolTip(this.radioPack, "Choose this for a more accurate but slower process");
            this.radioPack.UseVisualStyleBackColor = true;
            // 
            // chkBackup
            // 
            this.chkBackup.AutoSize = true;
            this.chkBackup.Checked = true;
            this.chkBackup.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBackup.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkBackup.Location = new System.Drawing.Point(377, 212);
            this.chkBackup.Name = "chkBackup";
            this.chkBackup.Size = new System.Drawing.Size(96, 17);
            this.chkBackup.TabIndex = 43;
            this.chkBackup.Text = "Create backup";
            this.toolTip1.SetToolTip(this.chkBackup, "Leave this selected to have a backup created");
            this.chkBackup.UseVisualStyleBackColor = true;
            this.chkBackup.CheckedChanged += new System.EventHandler(this.chkBackup_CheckedChanged);
            // 
            // txtTitle
            // 
            this.txtTitle.Location = new System.Drawing.Point(4, 260);
            this.txtTitle.MaxLength = 80;
            this.txtTitle.Multiline = true;
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(424, 48);
            this.txtTitle.TabIndex = 44;
            this.toolTip1.SetToolTip(this.txtTitle, "This is what you see in your Xbox dashboard");
            this.txtTitle.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.txtTitle.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // txtDescription
            // 
            this.txtDescription.ForeColor = System.Drawing.Color.LightGray;
            this.txtDescription.Location = new System.Drawing.Point(4, 330);
            this.txtDescription.MaxLength = 80;
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(424, 48);
            this.txtDescription.TabIndex = 45;
            this.txtDescription.Text = "Edited with Nautilus";
            this.toolTip1.SetToolTip(this.txtDescription, "This is only seen in this and similar programs");
            this.txtDescription.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.txtDescription.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // btnSelect
            // 
            this.btnSelect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(195)))), ((int)(((byte)(73)))));
            this.btnSelect.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSelect.Enabled = false;
            this.btnSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelect.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelect.ForeColor = System.Drawing.Color.White;
            this.btnSelect.Location = new System.Drawing.Point(488, 27);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(79, 27);
            this.btnSelect.TabIndex = 51;
            this.btnSelect.Text = "Select All";
            this.toolTip1.SetToolTip(this.btnSelect, "Select all songs in the list");
            this.btnSelect.UseVisualStyleBackColor = false;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnClear
            // 
            this.btnClear.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(195)))), ((int)(((byte)(73)))));
            this.btnClear.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClear.Enabled = false;
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.ForeColor = System.Drawing.Color.White;
            this.btnClear.Location = new System.Drawing.Point(488, 60);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(79, 27);
            this.btnClear.TabIndex = 52;
            this.btnClear.Text = "Clear All";
            this.toolTip1.SetToolTip(this.btnClear, "Clear selections");
            this.btnClear.UseVisualStyleBackColor = false;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnDePack
            // 
            this.btnDePack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(195)))), ((int)(((byte)(73)))));
            this.btnDePack.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDePack.Enabled = false;
            this.btnDePack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDePack.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDePack.ForeColor = System.Drawing.Color.White;
            this.btnDePack.Location = new System.Drawing.Point(488, 307);
            this.btnDePack.Name = "btnDePack";
            this.btnDePack.Size = new System.Drawing.Size(79, 27);
            this.btnDePack.TabIndex = 53;
            this.btnDePack.Text = "dePACK";
            this.toolTip1.SetToolTip(this.btnDePack, "Click to begin dePACKing process");
            this.btnDePack.UseVisualStyleBackColor = false;
            this.btnDePack.Click += new System.EventHandler(this.btnDePack_Click);
            // 
            // PackageImage
            // 
            this.PackageImage.BackColor = System.Drawing.Color.Transparent;
            this.PackageImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PackageImage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.PackageImage.Location = new System.Drawing.Point(434, 260);
            this.PackageImage.Name = "PackageImage";
            this.PackageImage.Size = new System.Drawing.Size(48, 48);
            this.PackageImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PackageImage.TabIndex = 48;
            this.PackageImage.TabStop = false;
            this.toolTip1.SetToolTip(this.PackageImage, "This is the thumbnail you see on the Xbox dashboard");
            this.PackageImage.DragDrop += new System.Windows.Forms.DragEventHandler(this.PackageImage_DragDrop);
            this.PackageImage.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            this.PackageImage.MouseClick += new System.Windows.Forms.MouseEventHandler(this.PackageImage_MouseClick);
            // 
            // ContentImage
            // 
            this.ContentImage.BackColor = System.Drawing.Color.Transparent;
            this.ContentImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ContentImage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ContentImage.Location = new System.Drawing.Point(434, 330);
            this.ContentImage.Name = "ContentImage";
            this.ContentImage.Size = new System.Drawing.Size(48, 48);
            this.ContentImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ContentImage.TabIndex = 47;
            this.ContentImage.TabStop = false;
            this.toolTip1.SetToolTip(this.ContentImage, "This represents the game the content is for, only seen in this and similar progra" +
        "ms");
            this.ContentImage.DragDrop += new System.Windows.Forms.DragEventHandler(this.ContentImage_DragDrop);
            this.ContentImage.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            this.ContentImage.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ContentImage_MouseClick);
            // 
            // picPin
            // 
            this.picPin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picPin.BackColor = System.Drawing.Color.Transparent;
            this.picPin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picPin.Image = global::Nautilus.Properties.Resources.unpinned;
            this.picPin.Location = new System.Drawing.Point(547, 3);
            this.picPin.Name = "picPin";
            this.picPin.Size = new System.Drawing.Size(20, 20);
            this.picPin.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picPin.TabIndex = 64;
            this.picPin.TabStop = false;
            this.picPin.Tag = "unpinned";
            this.toolTip1.SetToolTip(this.picPin, "Click to pin on top");
            this.picPin.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picPin_MouseClick);
            // 
            // btnExtract
            // 
            this.btnExtract.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(195)))), ((int)(((byte)(73)))));
            this.btnExtract.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExtract.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExtract.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExtract.ForeColor = System.Drawing.Color.White;
            this.btnExtract.Location = new System.Drawing.Point(488, 208);
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Size = new System.Drawing.Size(79, 41);
            this.btnExtract.TabIndex = 65;
            this.btnExtract.Text = "Extract selected";
            this.toolTip1.SetToolTip(this.btnExtract, "Extract selected song");
            this.btnExtract.UseVisualStyleBackColor = false;
            this.btnExtract.Visible = false;
            this.btnExtract.Click += new System.EventHandler(this.btnExtract_Click);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFileToolStripMenuItem,
            this.batchDePACKToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.helpToolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(572, 24);
            this.menuStrip1.TabIndex = 35;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // openFileToolStripMenuItem
            // 
            this.openFileToolStripMenuItem.Name = "openFileToolStripMenuItem";
            this.openFileToolStripMenuItem.Size = new System.Drawing.Size(76, 20);
            this.openFileToolStripMenuItem.Text = "Open pack";
            this.openFileToolStripMenuItem.Click += new System.EventHandler(this.openFileToolStripMenuItem_Click);
            // 
            // batchDePACKToolStripMenuItem
            // 
            this.batchDePACKToolStripMenuItem.Name = "batchDePACKToolStripMenuItem";
            this.batchDePACKToolStripMenuItem.Size = new System.Drawing.Size(94, 20);
            this.batchDePACKToolStripMenuItem.Text = "Batch dePACK";
            this.batchDePACKToolStripMenuItem.Click += new System.EventHandler(this.batchDePACKToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setGameIDToolStripMenuItem,
            this.dePACKOptionsToolStripMenuItem,
            this.signingOptionsToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(72, 20);
            this.optionsToolStripMenuItem.Text = "Advanced";
            // 
            // setGameIDToolStripMenuItem
            // 
            this.setGameIDToolStripMenuItem.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.setGameIDToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rockBandToolStripMenuItem,
            this.rockBand2ToolStripMenuItem,
            this.rockBand3ToolStripMenuItem});
            this.setGameIDToolStripMenuItem.Name = "setGameIDToolStripMenuItem";
            this.setGameIDToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.setGameIDToolStripMenuItem.Text = "Change game ID";
            // 
            // rockBandToolStripMenuItem
            // 
            this.rockBandToolStripMenuItem.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.rockBandToolStripMenuItem.Name = "rockBandToolStripMenuItem";
            this.rockBandToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.rockBandToolStripMenuItem.Text = "Rock Band";
            this.rockBandToolStripMenuItem.Click += new System.EventHandler(this.rockBandToolStripMenuItem_Click);
            // 
            // rockBand2ToolStripMenuItem
            // 
            this.rockBand2ToolStripMenuItem.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.rockBand2ToolStripMenuItem.Name = "rockBand2ToolStripMenuItem";
            this.rockBand2ToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.rockBand2ToolStripMenuItem.Text = "Rock Band 2";
            this.rockBand2ToolStripMenuItem.Click += new System.EventHandler(this.rockBand2ToolStripMenuItem_Click);
            // 
            // rockBand3ToolStripMenuItem
            // 
            this.rockBand3ToolStripMenuItem.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.rockBand3ToolStripMenuItem.Checked = true;
            this.rockBand3ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.rockBand3ToolStripMenuItem.Name = "rockBand3ToolStripMenuItem";
            this.rockBand3ToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.rockBand3ToolStripMenuItem.Text = "Rock Band 3";
            this.rockBand3ToolStripMenuItem.Click += new System.EventHandler(this.rockBand3ToolStripMenuItem_Click);
            // 
            // dePACKOptionsToolStripMenuItem
            // 
            this.dePACKOptionsToolStripMenuItem.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.dePACKOptionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.usePackThumbnail,
            this.useSongAlbumArt,
            this.openFolderAfterDePACK,
            this.useArtistSongShortName});
            this.dePACKOptionsToolStripMenuItem.Name = "dePACKOptionsToolStripMenuItem";
            this.dePACKOptionsToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.dePACKOptionsToolStripMenuItem.Text = "dePACK options";
            // 
            // usePackThumbnail
            // 
            this.usePackThumbnail.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.usePackThumbnail.CheckOnClick = true;
            this.usePackThumbnail.Name = "usePackThumbnail";
            this.usePackThumbnail.Size = new System.Drawing.Size(226, 22);
            this.usePackThumbnail.Text = "Use pack thumbnail";
            this.usePackThumbnail.Click += new System.EventHandler(this.usePackThumbnailToolStripMenuItem_Click);
            // 
            // useSongAlbumArt
            // 
            this.useSongAlbumArt.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.useSongAlbumArt.Checked = true;
            this.useSongAlbumArt.CheckOnClick = true;
            this.useSongAlbumArt.CheckState = System.Windows.Forms.CheckState.Checked;
            this.useSongAlbumArt.Name = "useSongAlbumArt";
            this.useSongAlbumArt.Size = new System.Drawing.Size(226, 22);
            this.useSongAlbumArt.Text = "Use song album art";
            this.useSongAlbumArt.Click += new System.EventHandler(this.useSongAlbumArtToolStripMenuItem_Click);
            // 
            // openFolderAfterDePACK
            // 
            this.openFolderAfterDePACK.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.openFolderAfterDePACK.Checked = true;
            this.openFolderAfterDePACK.CheckOnClick = true;
            this.openFolderAfterDePACK.CheckState = System.Windows.Forms.CheckState.Checked;
            this.openFolderAfterDePACK.Name = "openFolderAfterDePACK";
            this.openFolderAfterDePACK.Size = new System.Drawing.Size(226, 22);
            this.openFolderAfterDePACK.Text = "Open folder after dePACKing";
            // 
            // useArtistSongShortName
            // 
            this.useArtistSongShortName.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.useArtistSongShortName.CheckOnClick = true;
            this.useArtistSongShortName.Name = "useArtistSongShortName";
            this.useArtistSongShortName.Size = new System.Drawing.Size(226, 22);
            this.useArtistSongShortName.Text = "Use ArtistSongShortName";
            // 
            // signingOptionsToolStripMenuItem
            // 
            this.signingOptionsToolStripMenuItem.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.signingOptionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.signAsCON,
            this.signAsLIVE});
            this.signingOptionsToolStripMenuItem.Name = "signingOptionsToolStripMenuItem";
            this.signingOptionsToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.signingOptionsToolStripMenuItem.Text = "Signing options";
            // 
            // signAsCON
            // 
            this.signAsCON.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.signAsCON.Checked = true;
            this.signAsCON.CheckState = System.Windows.Forms.CheckState.Checked;
            this.signAsCON.Name = "signAsCON";
            this.signAsCON.Size = new System.Drawing.Size(140, 22);
            this.signAsCON.Text = "Sign as CON";
            this.signAsCON.Click += new System.EventHandler(this.signAsCON_Click);
            // 
            // signAsLIVE
            // 
            this.signAsLIVE.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.signAsLIVE.Name = "signAsLIVE";
            this.signAsLIVE.Size = new System.Drawing.Size(140, 22);
            this.signAsLIVE.Text = "Sign as LIVE";
            this.signAsLIVE.Click += new System.EventHandler(this.signAsLIVE_Click);
            // 
            // helpToolStripMenuItem1
            // 
            this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
            this.helpToolStripMenuItem1.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem1.Text = "&Help";
            this.helpToolStripMenuItem1.Click += new System.EventHandler(this.helpToolStripMenuItem1_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 244);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 49;
            this.label1.Text = "Package Title";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 314);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 13);
            this.label2.TabIndex = 50;
            this.label2.Text = "Package Description";
            // 
            // picWorking
            // 
            this.picWorking.Image = global::Nautilus.Properties.Resources.working;
            this.picWorking.Location = new System.Drawing.Point(215, 380);
            this.picWorking.Name = "picWorking";
            this.picWorking.Size = new System.Drawing.Size(128, 15);
            this.picWorking.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picWorking.TabIndex = 55;
            this.picWorking.TabStop = false;
            this.picWorking.Visible = false;
            // 
            // backgroundWorker2
            // 
            this.backgroundWorker2.WorkerReportsProgress = true;
            this.backgroundWorker2.WorkerSupportsCancellation = true;
            this.backgroundWorker2.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker2_DoWork);
            this.backgroundWorker2.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker2_RunWorkerCompleted);
            // 
            // backgroundWorker3
            // 
            this.backgroundWorker3.WorkerReportsProgress = true;
            this.backgroundWorker3.WorkerSupportsCancellation = true;
            this.backgroundWorker3.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker3_DoWork);
            this.backgroundWorker3.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker3_RunWorkerCompleted);
            // 
            // backgroundWorker4
            // 
            this.backgroundWorker4.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker4_DoWork);
            // 
            // QuickPackEditor
            // 
            this.AllowDrop = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(572, 562);
            this.Controls.Add(this.btnExtract);
            this.Controls.Add(this.picPin);
            this.Controls.Add(this.picWorking);
            this.Controls.Add(this.btnDePack);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PackageImage);
            this.Controls.Add(this.ContentImage);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.txtTitle);
            this.Controls.Add(this.chkBackup);
            this.Controls.Add(this.radioPack);
            this.Controls.Add(this.radioDTA);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnRePack);
            this.Controls.Add(this.btnRestore);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.lstSongs);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.lstLog);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "QuickPackEditor";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Quick Pack Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.QuickPackEditor_FormClosing);
            this.Shown += new System.EventHandler(this.QuickPack_Shown);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PackageImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ContentImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPin)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picWorking)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstLog;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem exportLogFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openFileToolStripMenuItem;
        private System.Windows.Forms.ListBox lstSongs;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnRestore;
        private System.Windows.Forms.Button btnRePack;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.RadioButton radioDTA;
        private System.Windows.Forms.RadioButton radioPack;
        private System.Windows.Forms.CheckBox chkBackup;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.PictureBox ContentImage;
        private System.Windows.Forms.PictureBox PackageImage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripMenuItem setGameIDToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rockBandToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rockBand2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rockBand3ToolStripMenuItem;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnDePack;
        private System.Windows.Forms.ToolStripMenuItem dePACKOptionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem usePackThumbnail;
        private System.Windows.Forms.ToolStripMenuItem useSongAlbumArt;
        private System.Windows.Forms.ToolStripMenuItem openFolderAfterDePACK;
        private System.Windows.Forms.PictureBox picWorking;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
        private System.ComponentModel.BackgroundWorker backgroundWorker3;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem sortByDTAEntry;
        private System.Windows.Forms.ToolStripMenuItem sortByArtistSong;
        private System.Windows.Forms.ToolStripMenuItem sortBySongArtist;
        private System.Windows.Forms.ToolStripMenuItem batchDePACKToolStripMenuItem;
        private System.Windows.Forms.PictureBox picPin;
        private System.Windows.Forms.Button btnExtract;
        private System.ComponentModel.BackgroundWorker backgroundWorker4;
        private System.Windows.Forms.ToolStripMenuItem signingOptionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem signAsCON;
        private System.Windows.Forms.ToolStripMenuItem signAsLIVE;
        private System.Windows.Forms.ToolStripMenuItem useArtistSongShortName;
    }
}