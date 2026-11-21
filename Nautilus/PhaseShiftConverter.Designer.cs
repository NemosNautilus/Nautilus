namespace Nautilus
{
    partial class PhaseShiftConverter
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
            this.btnBegin = new System.Windows.Forms.Button();
            this.picPin = new System.Windows.Forms.PictureBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.decodeYARG = new System.Windows.Forms.ToolStripMenuItem();
            this.packToYargSong = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.decodeSNG = new System.Windows.Forms.ToolStripMenuItem();
            this.packCloneHeroFoldersToSng = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useguitaroggForNonmultitrackSongs = new System.Windows.Forms.ToolStripMenuItem();
            this.useSubgenreInsteadOfGenreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cleanUppngxboxFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.dontAddHopoThreshold = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnReset = new System.Windows.Forms.Button();
            this.txtFolder = new System.Windows.Forms.TextBox();
            this.btnFolder = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.chkRAR = new System.Windows.Forms.CheckBox();
            this.picWorking = new System.Windows.Forms.PictureBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.radioSeparate = new System.Windows.Forms.RadioButton();
            this.grpMogg = new System.Windows.Forms.GroupBox();
            this.domainQuality = new System.Windows.Forms.DomainUpDown();
            this.lblQuality = new System.Windows.Forms.Label();
            this.radioDownmix = new System.Windows.Forms.RadioButton();
            this.markAsC3 = new System.Windows.Forms.CheckBox();
            this.markAsRV = new System.Windows.Forms.CheckBox();
            this.markAsCustom = new System.Windows.Forms.CheckBox();
            this.txtCustomLabel = new System.Windows.Forms.TextBox();
            this.grpFormat = new System.Windows.Forms.GroupBox();
            this.radioOpus = new System.Windows.Forms.RadioButton();
            this.radioOgg = new System.Windows.Forms.RadioButton();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPin)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picWorking)).BeginInit();
            this.grpMogg.SuspendLayout();
            this.grpFormat.SuspendLayout();
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
            this.lstLog.Location = new System.Drawing.Point(12, 174);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(568, 171);
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
            // btnBegin
            // 
            this.btnBegin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBegin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(129)))), ((int)(((byte)(216)))));
            this.btnBegin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBegin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBegin.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBegin.ForeColor = System.Drawing.Color.White;
            this.btnBegin.Location = new System.Drawing.Point(516, 139);
            this.btnBegin.Name = "btnBegin";
            this.btnBegin.Size = new System.Drawing.Size(64, 29);
            this.btnBegin.TabIndex = 51;
            this.btnBegin.Text = "&Begin";
            this.toolTip1.SetToolTip(this.btnBegin, "Click to begin");
            this.btnBegin.UseVisualStyleBackColor = false;
            this.btnBegin.Visible = false;
            this.btnBegin.Click += new System.EventHandler(this.btnBegin_Click);
            // 
            // picPin
            // 
            this.picPin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picPin.BackColor = System.Drawing.Color.Transparent;
            this.picPin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picPin.Image = global::Nautilus.Properties.Resources.unpinned;
            this.picPin.Location = new System.Drawing.Point(567, 4);
            this.picPin.Name = "picPin";
            this.picPin.Size = new System.Drawing.Size(20, 20);
            this.picPin.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picPin.TabIndex = 71;
            this.picPin.TabStop = false;
            this.picPin.Tag = "unpinned";
            this.toolTip1.SetToolTip(this.picPin, "Click to pin on top");
            this.picPin.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picPin_MouseClick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolsToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.helpToolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(592, 24);
            this.menuStrip1.TabIndex = 35;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.decodeYARG,
            this.packToYargSong,
            this.toolStripMenuItem2,
            this.decodeSNG,
            this.packCloneHeroFoldersToSng});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // decodeYARG
            // 
            this.decodeYARG.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.decodeYARG.Name = "decodeYARG";
            this.decodeYARG.Size = new System.Drawing.Size(326, 22);
            this.decodeYARG.Text = "Extract YARG yargsong files to folders";
            this.decodeYARG.Click += new System.EventHandler(this.decodeYARG_Click);
            // 
            // packToYargSong
            // 
            this.packToYargSong.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.packToYargSong.Name = "packToYargSong";
            this.packToYargSong.Size = new System.Drawing.Size(326, 22);
            this.packToYargSong.Text = "Pack Clone Hero folders to YARG yargsong files ";
            this.packToYargSong.Click += new System.EventHandler(this.packToYargSong_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(323, 6);
            // 
            // decodeSNG
            // 
            this.decodeSNG.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.decodeSNG.Name = "decodeSNG";
            this.decodeSNG.Size = new System.Drawing.Size(326, 22);
            this.decodeSNG.Text = "Extract Clone Hero sng files to folders";
            this.decodeSNG.Click += new System.EventHandler(this.decodeSNG_Click);
            // 
            // packCloneHeroFoldersToSng
            // 
            this.packCloneHeroFoldersToSng.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.packCloneHeroFoldersToSng.Name = "packCloneHeroFoldersToSng";
            this.packCloneHeroFoldersToSng.Size = new System.Drawing.Size(326, 22);
            this.packCloneHeroFoldersToSng.Text = "Pack Clone Hero folders to sng files";
            this.packCloneHeroFoldersToSng.Click += new System.EventHandler(this.packCloneHeroFoldersToSng_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.useguitaroggForNonmultitrackSongs,
            this.useSubgenreInsteadOfGenreToolStripMenuItem,
            this.cleanUppngxboxFiles,
            this.dontAddHopoThreshold});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // useguitaroggForNonmultitrackSongs
            // 
            this.useguitaroggForNonmultitrackSongs.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.useguitaroggForNonmultitrackSongs.CheckOnClick = true;
            this.useguitaroggForNonmultitrackSongs.Name = "useguitaroggForNonmultitrackSongs";
            this.useguitaroggForNonmultitrackSongs.Size = new System.Drawing.Size(406, 22);
            this.useguitaroggForNonmultitrackSongs.Text = "Use \'guitar.ogg\' for non-multitrack songs";
            // 
            // useSubgenreInsteadOfGenreToolStripMenuItem
            // 
            this.useSubgenreInsteadOfGenreToolStripMenuItem.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.useSubgenreInsteadOfGenreToolStripMenuItem.CheckOnClick = true;
            this.useSubgenreInsteadOfGenreToolStripMenuItem.Name = "useSubgenreInsteadOfGenreToolStripMenuItem";
            this.useSubgenreInsteadOfGenreToolStripMenuItem.Size = new System.Drawing.Size(406, 22);
            this.useSubgenreInsteadOfGenreToolStripMenuItem.Text = "Use subgenre instead of genre";
            // 
            // cleanUppngxboxFiles
            // 
            this.cleanUppngxboxFiles.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.cleanUppngxboxFiles.Checked = true;
            this.cleanUppngxboxFiles.CheckOnClick = true;
            this.cleanUppngxboxFiles.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cleanUppngxboxFiles.Name = "cleanUppngxboxFiles";
            this.cleanUppngxboxFiles.Size = new System.Drawing.Size(406, 22);
            this.cleanUppngxboxFiles.Text = "Clean up *.png_xbox files";
            // 
            // dontAddHopoThreshold
            // 
            this.dontAddHopoThreshold.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.dontAddHopoThreshold.Checked = true;
            this.dontAddHopoThreshold.CheckOnClick = true;
            this.dontAddHopoThreshold.CheckState = System.Windows.Forms.CheckState.Checked;
            this.dontAddHopoThreshold.Name = "dontAddHopoThreshold";
            this.dontAddHopoThreshold.Size = new System.Drawing.Size(406, 22);
            this.dontAddHopoThreshold.Text = "Don\'t add hopo threshold to .ini file (Clone Hero compatibility)";
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
            // btnReset
            // 
            this.btnReset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(129)))), ((int)(((byte)(216)))));
            this.btnReset.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReset.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReset.ForeColor = System.Drawing.Color.White;
            this.btnReset.Location = new System.Drawing.Point(12, 139);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(100, 29);
            this.btnReset.TabIndex = 54;
            this.btnReset.Text = "&Reset";
            this.btnReset.UseVisualStyleBackColor = false;
            this.btnReset.Visible = false;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // txtFolder
            // 
            this.txtFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFolder.BackColor = System.Drawing.Color.White;
            this.txtFolder.Location = new System.Drawing.Point(12, 60);
            this.txtFolder.Name = "txtFolder";
            this.txtFolder.ReadOnly = true;
            this.txtFolder.Size = new System.Drawing.Size(568, 20);
            this.txtFolder.TabIndex = 48;
            this.txtFolder.TextChanged += new System.EventHandler(this.txtFolder_TextChanged);
            this.txtFolder.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.txtFolder.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // btnFolder
            // 
            this.btnFolder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(129)))), ((int)(((byte)(216)))));
            this.btnFolder.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFolder.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFolder.ForeColor = System.Drawing.Color.White;
            this.btnFolder.Location = new System.Drawing.Point(12, 27);
            this.btnFolder.Name = "btnFolder";
            this.btnFolder.Size = new System.Drawing.Size(134, 30);
            this.btnFolder.TabIndex = 49;
            this.btnFolder.Text = "Change &Input Folder";
            this.btnFolder.UseVisualStyleBackColor = false;
            this.btnFolder.Click += new System.EventHandler(this.btnFolder_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(129)))), ((int)(((byte)(216)))));
            this.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.Location = new System.Drawing.Point(480, 27);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(100, 30);
            this.btnRefresh.TabIndex = 50;
            this.btnRefresh.Text = "Refresh Folder";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Visible = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // chkRAR
            // 
            this.chkRAR.AutoSize = true;
            this.chkRAR.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkRAR.Location = new System.Drawing.Point(166, 40);
            this.chkRAR.Name = "chkRAR";
            this.chkRAR.Size = new System.Drawing.Size(189, 17);
            this.chkRAR.TabIndex = 55;
            this.chkRAR.Text = "Create RAR archive for each song";
            this.chkRAR.UseVisualStyleBackColor = true;
            // 
            // picWorking
            // 
            this.picWorking.Image = global::Nautilus.Properties.Resources.working;
            this.picWorking.Location = new System.Drawing.Point(232, 19);
            this.picWorking.Name = "picWorking";
            this.picWorking.Size = new System.Drawing.Size(128, 15);
            this.picWorking.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picWorking.TabIndex = 62;
            this.picWorking.TabStop = false;
            this.picWorking.Visible = false;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // radioSeparate
            // 
            this.radioSeparate.AutoSize = true;
            this.radioSeparate.Checked = true;
            this.radioSeparate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioSeparate.Location = new System.Drawing.Point(9, 20);
            this.radioSeparate.Name = "radioSeparate";
            this.radioSeparate.Size = new System.Drawing.Size(110, 17);
            this.radioSeparate.TabIndex = 63;
            this.radioSeparate.TabStop = true;
            this.radioSeparate.Text = "Separate to stems";
            this.radioSeparate.UseVisualStyleBackColor = true;
            this.radioSeparate.CheckedChanged += new System.EventHandler(this.radioSeparate_CheckedChanged);
            // 
            // grpMogg
            // 
            this.grpMogg.Controls.Add(this.domainQuality);
            this.grpMogg.Controls.Add(this.lblQuality);
            this.grpMogg.Controls.Add(this.radioDownmix);
            this.grpMogg.Controls.Add(this.radioSeparate);
            this.grpMogg.Location = new System.Drawing.Point(12, 86);
            this.grpMogg.Name = "grpMogg";
            this.grpMogg.Size = new System.Drawing.Size(243, 47);
            this.grpMogg.TabIndex = 69;
            this.grpMogg.TabStop = false;
            this.grpMogg.Text = "Mogg options:";
            // 
            // domainQuality
            // 
            this.domainQuality.Items.Add("10");
            this.domainQuality.Items.Add("9");
            this.domainQuality.Items.Add("8");
            this.domainQuality.Items.Add("7");
            this.domainQuality.Items.Add("6");
            this.domainQuality.Items.Add("5");
            this.domainQuality.Items.Add("4");
            this.domainQuality.Items.Add("3");
            this.domainQuality.Items.Add("2");
            this.domainQuality.Items.Add("1");
            this.domainQuality.Location = new System.Drawing.Point(312, 19);
            this.domainQuality.Name = "domainQuality";
            this.domainQuality.ReadOnly = true;
            this.domainQuality.Size = new System.Drawing.Size(36, 20);
            this.domainQuality.TabIndex = 71;
            this.domainQuality.Text = "5";
            this.domainQuality.Visible = false;
            // 
            // lblQuality
            // 
            this.lblQuality.AutoSize = true;
            this.lblQuality.Location = new System.Drawing.Point(271, 22);
            this.lblQuality.Name = "lblQuality";
            this.lblQuality.Size = new System.Drawing.Size(42, 13);
            this.lblQuality.TabIndex = 70;
            this.lblQuality.Text = "Quality:";
            this.lblQuality.Visible = false;
            // 
            // radioDownmix
            // 
            this.radioDownmix.AutoSize = true;
            this.radioDownmix.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioDownmix.Location = new System.Drawing.Point(125, 20);
            this.radioDownmix.Name = "radioDownmix";
            this.radioDownmix.Size = new System.Drawing.Size(112, 17);
            this.radioDownmix.TabIndex = 69;
            this.radioDownmix.Text = "Downmix to stereo";
            this.radioDownmix.UseVisualStyleBackColor = true;
            // 
            // markAsC3
            // 
            this.markAsC3.AutoSize = true;
            this.markAsC3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.markAsC3.Location = new System.Drawing.Point(141, 151);
            this.markAsC3.Name = "markAsC3";
            this.markAsC3.Size = new System.Drawing.Size(80, 17);
            this.markAsC3.TabIndex = 72;
            this.markAsC3.Text = "Mark as C3";
            this.markAsC3.UseVisualStyleBackColor = true;
            this.markAsC3.CheckedChanged += new System.EventHandler(this.markAsC3_CheckedChanged);
            // 
            // markAsRV
            // 
            this.markAsRV.AutoSize = true;
            this.markAsRV.Cursor = System.Windows.Forms.Cursors.Hand;
            this.markAsRV.Location = new System.Drawing.Point(227, 151);
            this.markAsRV.Name = "markAsRV";
            this.markAsRV.Size = new System.Drawing.Size(82, 17);
            this.markAsRV.TabIndex = 73;
            this.markAsRV.Text = "Mark as RV";
            this.markAsRV.UseVisualStyleBackColor = true;
            this.markAsRV.CheckedChanged += new System.EventHandler(this.markAsRV_CheckedChanged);
            // 
            // markAsCustom
            // 
            this.markAsCustom.AutoSize = true;
            this.markAsCustom.Cursor = System.Windows.Forms.Cursors.Hand;
            this.markAsCustom.Location = new System.Drawing.Point(315, 151);
            this.markAsCustom.Name = "markAsCustom";
            this.markAsCustom.Size = new System.Drawing.Size(67, 17);
            this.markAsCustom.TabIndex = 74;
            this.markAsCustom.Text = "Mark as:";
            this.markAsCustom.UseVisualStyleBackColor = true;
            this.markAsCustom.CheckedChanged += new System.EventHandler(this.markAsCustom_CheckedChanged);
            // 
            // txtCustomLabel
            // 
            this.txtCustomLabel.Enabled = false;
            this.txtCustomLabel.Location = new System.Drawing.Point(383, 148);
            this.txtCustomLabel.Name = "txtCustomLabel";
            this.txtCustomLabel.Size = new System.Drawing.Size(100, 20);
            this.txtCustomLabel.TabIndex = 75;
            // 
            // grpFormat
            // 
            this.grpFormat.Controls.Add(this.radioOpus);
            this.grpFormat.Controls.Add(this.radioOgg);
            this.grpFormat.Location = new System.Drawing.Point(266, 86);
            this.grpFormat.Name = "grpFormat";
            this.grpFormat.Size = new System.Drawing.Size(116, 47);
            this.grpFormat.TabIndex = 76;
            this.grpFormat.TabStop = false;
            this.grpFormat.Text = "Audio format:";
            // 
            // radioOpus
            // 
            this.radioOpus.AutoSize = true;
            this.radioOpus.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioOpus.Location = new System.Drawing.Point(60, 20);
            this.radioOpus.Name = "radioOpus";
            this.radioOpus.Size = new System.Drawing.Size(50, 17);
            this.radioOpus.TabIndex = 69;
            this.radioOpus.Text = "Opus";
            this.radioOpus.UseVisualStyleBackColor = true;
            // 
            // radioOgg
            // 
            this.radioOgg.AutoSize = true;
            this.radioOgg.Checked = true;
            this.radioOgg.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioOgg.Location = new System.Drawing.Point(9, 20);
            this.radioOgg.Name = "radioOgg";
            this.radioOgg.Size = new System.Drawing.Size(45, 17);
            this.radioOgg.TabIndex = 63;
            this.radioOgg.TabStop = true;
            this.radioOgg.Text = "Ogg";
            this.radioOgg.UseVisualStyleBackColor = true;
            // 
            // PhaseShiftConverter
            // 
            this.AllowDrop = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(592, 356);
            this.Controls.Add(this.grpFormat);
            this.Controls.Add(this.txtCustomLabel);
            this.Controls.Add(this.markAsCustom);
            this.Controls.Add(this.markAsRV);
            this.Controls.Add(this.markAsC3);
            this.Controls.Add(this.picPin);
            this.Controls.Add(this.grpMogg);
            this.Controls.Add(this.picWorking);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.txtFolder);
            this.Controls.Add(this.btnFolder);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnBegin);
            this.Controls.Add(this.chkRAR);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.lstLog);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.Name = "PhaseShiftConverter";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Clone Hero Converter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PhaseShiftPrep_FormClosing);
            this.Shown += new System.EventHandler(this.PhaseShiftPrep_Shown);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            this.Resize += new System.EventHandler(this.PhaseShiftPrep_Resize);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picPin)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picWorking)).EndInit();
            this.grpMogg.ResumeLayout(false);
            this.grpMogg.PerformLayout();
            this.grpFormat.ResumeLayout(false);
            this.grpFormat.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstLog;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem exportLogFileToolStripMenuItem;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.TextBox txtFolder;
        private System.Windows.Forms.Button btnFolder;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnBegin;
        private System.Windows.Forms.CheckBox chkRAR;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem useSubgenreInsteadOfGenreToolStripMenuItem;
        private System.Windows.Forms.PictureBox picWorking;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.RadioButton radioSeparate;
        private System.Windows.Forms.ToolStripMenuItem useguitaroggForNonmultitrackSongs;
        private System.Windows.Forms.GroupBox grpMogg;
        private System.Windows.Forms.RadioButton radioDownmix;
        private System.Windows.Forms.PictureBox picPin;
        private System.Windows.Forms.ToolStripMenuItem cleanUppngxboxFiles;
        private System.Windows.Forms.DomainUpDown domainQuality;
        private System.Windows.Forms.Label lblQuality;
        private System.Windows.Forms.ToolStripMenuItem dontAddHopoThreshold;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem decodeSNG;
        private System.Windows.Forms.CheckBox markAsC3;
        private System.Windows.Forms.CheckBox markAsRV;
        private System.Windows.Forms.CheckBox markAsCustom;
        private System.Windows.Forms.TextBox txtCustomLabel;
        private System.Windows.Forms.ToolStripMenuItem decodeYARG;
        private System.Windows.Forms.GroupBox grpFormat;
        private System.Windows.Forms.RadioButton radioOpus;
        private System.Windows.Forms.RadioButton radioOgg;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem packCloneHeroFoldersToSng;
        private System.Windows.Forms.ToolStripMenuItem packToYargSong;
    }
}