namespace Nautilus
{
    partial class PS3Converter
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
            this.chkMerge = new System.Windows.Forms.CheckBox();
            this.chkSongID = new System.Windows.Forms.CheckBox();
            this.btnBegin = new System.Windows.Forms.Button();
            this.picPin = new System.Windows.Forms.PictureBox();
            this.picOnyx = new System.Windows.Forms.PictureBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pS3Fixer = new System.Windows.Forms.ToolStripMenuItem();
            this.batchFixLoopingSongs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.downmixAndEncode = new System.Windows.Forms.ToolStripMenuItem();
            this.patchMoggForPS3UseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mergeSongsToolStrip = new System.Windows.Forms.ToolStripMenuItem();
            this.managePackDTAFile = new System.Windows.Forms.ToolStripMenuItem();
            this.pS3ToCONToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.songInFolderFormat = new System.Windows.Forms.ToolStripMenuItem();
            this.pkgToCON = new System.Windows.Forms.ToolStripMenuItem();
            this.numericIDOptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.batchChangeIDsToNumeric = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mIDIToEDATEncryptionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.type1 = new System.Windows.Forms.ToolStripMenuItem();
            this.type2 = new System.Windows.Forms.ToolStripMenuItem();
            this.wait2Seconds = new System.Windows.Forms.ToolStripMenuItem();
            this.wait5Seconds = new System.Windows.Forms.ToolStripMenuItem();
            this.wait10Seconds = new System.Windows.Forms.ToolStripMenuItem();
            this.type3 = new System.Windows.Forms.ToolStripMenuItem();
            this.encryptReplacementMIDI = new System.Windows.Forms.ToolStripMenuItem();
            this.encryptReplacementMogg = new System.Windows.Forms.ToolStripMenuItem();
            this.regionOptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.regionNTSC = new System.Windows.Forms.ToolStripMenuItem();
            this.regionPAL = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnReset = new System.Windows.Forms.Button();
            this.txtFolder = new System.Windows.Forms.TextBox();
            this.btnFolder = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.chkRAR = new System.Windows.Forms.CheckBox();
            this.picWorking = new System.Windows.Forms.PictureBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorker3 = new System.ComponentModel.BackgroundWorker();
            this.btnOnyx = new System.Windows.Forms.Button();
            this.backgroundWorker4 = new System.ComponentModel.BackgroundWorker();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picOnyx)).BeginInit();
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
            this.lstLog.Location = new System.Drawing.Point(12, 144);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(568, 223);
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
            // chkMerge
            // 
            this.chkMerge.AutoSize = true;
            this.chkMerge.Checked = true;
            this.chkMerge.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMerge.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkMerge.Enabled = false;
            this.chkMerge.Location = new System.Drawing.Point(218, 86);
            this.chkMerge.Name = "chkMerge";
            this.chkMerge.Size = new System.Drawing.Size(170, 17);
            this.chkMerge.TabIndex = 63;
            this.chkMerge.Text = "Merge new songs with existing";
            this.toolTip1.SetToolTip(this.chkMerge, "Merge converted songs with your existing customs in the Merged Songs folder");
            this.chkMerge.UseVisualStyleBackColor = true;
            this.chkMerge.CheckedChanged += new System.EventHandler(this.chkMerge_CheckedChanged);
            // 
            // chkSongID
            // 
            this.chkSongID.AutoSize = true;
            this.chkSongID.Checked = true;
            this.chkSongID.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSongID.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkSongID.Location = new System.Drawing.Point(401, 86);
            this.chkSongID.Name = "chkSongID";
            this.chkSongID.Size = new System.Drawing.Size(184, 17);
            this.chkSongID.TabIndex = 64;
            this.chkSongID.Text = "Change song ID to numeric value";
            this.toolTip1.SetToolTip(this.chkSongID, "Change song ID to unique numeric value when converting");
            this.chkSongID.UseVisualStyleBackColor = true;
            // 
            // btnBegin
            // 
            this.btnBegin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBegin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(129)))), ((int)(((byte)(216)))));
            this.btnBegin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBegin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBegin.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBegin.ForeColor = System.Drawing.Color.White;
            this.btnBegin.Location = new System.Drawing.Point(516, 109);
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
            this.picPin.Location = new System.Drawing.Point(567, 4);
            this.picPin.Name = "picPin";
            this.picPin.Size = new System.Drawing.Size(20, 20);
            this.picPin.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picPin.TabIndex = 65;
            this.picPin.TabStop = false;
            this.picPin.Tag = "unpinned";
            this.toolTip1.SetToolTip(this.picPin, "Click to pin on top");
            this.picPin.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picPin_MouseClick);
            // 
            // picOnyx
            // 
            this.picOnyx.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picOnyx.Image = global::Nautilus.Properties.Resources.icon;
            this.picOnyx.InitialImage = global::Nautilus.Properties.Resources.icon;
            this.picOnyx.Location = new System.Drawing.Point(218, 23);
            this.picOnyx.Name = "picOnyx";
            this.picOnyx.Size = new System.Drawing.Size(36, 36);
            this.picOnyx.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picOnyx.TabIndex = 66;
            this.picOnyx.TabStop = false;
            this.toolTip1.SetToolTip(this.picOnyx, "CLICK HERE!");
            this.picOnyx.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picOnyx_MouseClick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolsToolStripMenuItem,
            this.pS3ToCONToolStripMenuItem,
            this.numericIDOptionsToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.regionOptionsToolStripMenuItem,
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
            this.pS3Fixer,
            this.batchFixLoopingSongs,
            this.toolStripMenuItem2,
            this.downmixAndEncode,
            this.patchMoggForPS3UseToolStripMenuItem,
            this.toolStripMenuItem1,
            this.mergeSongsToolStrip,
            this.managePackDTAFile});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // pS3Fixer
            // 
            this.pS3Fixer.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.pS3Fixer.Name = "pS3Fixer";
            this.pS3Fixer.Size = new System.Drawing.Size(263, 22);
            this.pS3Fixer.Text = "One-Click Batch PS3 Fixer";
            this.pS3Fixer.Click += new System.EventHandler(this.pS3Fixer_Click);
            // 
            // batchFixLoopingSongs
            // 
            this.batchFixLoopingSongs.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.batchFixLoopingSongs.Name = "batchFixLoopingSongs";
            this.batchFixLoopingSongs.Size = new System.Drawing.Size(263, 22);
            this.batchFixLoopingSongs.Text = "Batch fix looping songs";
            this.batchFixLoopingSongs.Click += new System.EventHandler(this.batchFixLoopingSongs_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(260, 6);
            this.toolStripMenuItem2.Paint += new System.Windows.Forms.PaintEventHandler(this.mnuToolStripSeparator_Custom_Paint);
            // 
            // downmixAndEncode
            // 
            this.downmixAndEncode.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.downmixAndEncode.Name = "downmixAndEncode";
            this.downmixAndEncode.Size = new System.Drawing.Size(263, 22);
            this.downmixAndEncode.Text = "Downmix and encode with quality 3";
            this.downmixAndEncode.Click += new System.EventHandler(this.downmixAndEncode_Click);
            // 
            // patchMoggForPS3UseToolStripMenuItem
            // 
            this.patchMoggForPS3UseToolStripMenuItem.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.patchMoggForPS3UseToolStripMenuItem.Name = "patchMoggForPS3UseToolStripMenuItem";
            this.patchMoggForPS3UseToolStripMenuItem.Size = new System.Drawing.Size(263, 22);
            this.patchMoggForPS3UseToolStripMenuItem.Text = "Patch mogg for PS3 use";
            this.patchMoggForPS3UseToolStripMenuItem.Click += new System.EventHandler(this.patchMoggForPS3UseToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(260, 6);
            this.toolStripMenuItem1.Paint += new System.Windows.Forms.PaintEventHandler(this.mnuToolStripSeparator_Custom_Paint);
            // 
            // mergeSongsToolStrip
            // 
            this.mergeSongsToolStrip.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.mergeSongsToolStrip.Name = "mergeSongsToolStrip";
            this.mergeSongsToolStrip.Size = new System.Drawing.Size(263, 22);
            this.mergeSongsToolStrip.Text = "Merge songs";
            this.mergeSongsToolStrip.Click += new System.EventHandler(this.mergeSongsToolStrip_Click);
            // 
            // managePackDTAFile
            // 
            this.managePackDTAFile.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.managePackDTAFile.Name = "managePackDTAFile";
            this.managePackDTAFile.Size = new System.Drawing.Size(263, 22);
            this.managePackDTAFile.Text = "Manage pack DTA file";
            this.managePackDTAFile.Click += new System.EventHandler(this.managePackDTAFile_Click);
            // 
            // pS3ToCONToolStripMenuItem
            // 
            this.pS3ToCONToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.songInFolderFormat,
            this.pkgToCON});
            this.pS3ToCONToolStripMenuItem.Name = "pS3ToCONToolStripMenuItem";
            this.pS3ToCONToolStripMenuItem.Size = new System.Drawing.Size(81, 20);
            this.pS3ToCONToolStripMenuItem.Text = "PS3 to CON";
            // 
            // songInFolderFormat
            // 
            this.songInFolderFormat.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.songInFolderFormat.Name = "songInFolderFormat";
            this.songInFolderFormat.Size = new System.Drawing.Size(288, 22);
            this.songInFolderFormat.Text = "Batch convert PS3 song folder(s) to CON";
            this.songInFolderFormat.Click += new System.EventHandler(this.songInFolderFormat_Click);
            // 
            // pkgToCON
            // 
            this.pkgToCON.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.pkgToCON.Name = "pkgToCON";
            this.pkgToCON.Size = new System.Drawing.Size(288, 22);
            this.pkgToCON.Text = "Batch convert PS3 PKG file(s) to CON";
            this.pkgToCON.Click += new System.EventHandler(this.pkgToCON_Click);
            // 
            // numericIDOptionsToolStripMenuItem
            // 
            this.numericIDOptionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.batchChangeIDsToNumeric});
            this.numericIDOptionsToolStripMenuItem.Name = "numericIDOptionsToolStripMenuItem";
            this.numericIDOptionsToolStripMenuItem.Size = new System.Drawing.Size(105, 20);
            this.numericIDOptionsToolStripMenuItem.Text = "Song ID Options";
            // 
            // batchChangeIDsToNumeric
            // 
            this.batchChangeIDsToNumeric.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.batchChangeIDsToNumeric.Name = "batchChangeIDsToNumeric";
            this.batchChangeIDsToNumeric.Size = new System.Drawing.Size(192, 22);
            this.batchChangeIDsToNumeric.Text = "Batch correct song IDs";
            this.batchChangeIDsToNumeric.Click += new System.EventHandler(this.batchChangeIDsToNumeric_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mIDIToEDATEncryptionToolStripMenuItem,
            this.encryptReplacementMIDI,
            this.encryptReplacementMogg});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(121, 20);
            this.optionsToolStripMenuItem.Text = "Encryption Options";
            this.optionsToolStripMenuItem.Visible = false;
            // 
            // mIDIToEDATEncryptionToolStripMenuItem
            // 
            this.mIDIToEDATEncryptionToolStripMenuItem.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.mIDIToEDATEncryptionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.type1,
            this.type2,
            this.type3});
            this.mIDIToEDATEncryptionToolStripMenuItem.Name = "mIDIToEDATEncryptionToolStripMenuItem";
            this.mIDIToEDATEncryptionToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.mIDIToEDATEncryptionToolStripMenuItem.Text = "MIDI to EDAT encryption type";
            // 
            // type1
            // 
            this.type1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.type1.Checked = true;
            this.type1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.type1.Name = "type1";
            this.type1.Size = new System.Drawing.Size(210, 22);
            this.type1.Text = "Type 1 (edattool.exe)";
            this.type1.Click += new System.EventHandler(this.type1defaultToolStripMenuItem_Click);
            // 
            // type2
            // 
            this.type2.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.type2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.wait2Seconds,
            this.wait5Seconds,
            this.wait10Seconds});
            this.type2.Name = "type2";
            this.type2.Size = new System.Drawing.Size(210, 22);
            this.type2.Text = "Type 2 (rebuilder.exe)";
            this.type2.Click += new System.EventHandler(this.type2_Click);
            // 
            // wait2Seconds
            // 
            this.wait2Seconds.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.wait2Seconds.Checked = true;
            this.wait2Seconds.CheckState = System.Windows.Forms.CheckState.Checked;
            this.wait2Seconds.Name = "wait2Seconds";
            this.wait2Seconds.Size = new System.Drawing.Size(180, 22);
            this.wait2Seconds.Text = "Wait 2 seconds";
            this.wait2Seconds.Click += new System.EventHandler(this.wait2Seconds_Click);
            // 
            // wait5Seconds
            // 
            this.wait5Seconds.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.wait5Seconds.Name = "wait5Seconds";
            this.wait5Seconds.Size = new System.Drawing.Size(180, 22);
            this.wait5Seconds.Text = "Wait 5 seconds";
            this.wait5Seconds.Click += new System.EventHandler(this.wait5Seconds_Click);
            // 
            // wait10Seconds
            // 
            this.wait10Seconds.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.wait10Seconds.Name = "wait10Seconds";
            this.wait10Seconds.Size = new System.Drawing.Size(180, 22);
            this.wait10Seconds.Text = "Wait 10 seconds";
            this.wait10Seconds.Click += new System.EventHandler(this.wait10Seconds_Click);
            // 
            // type3
            // 
            this.type3.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.type3.Name = "type3";
            this.type3.Size = new System.Drawing.Size(210, 22);
            this.type3.Text = "Type 3 (make_npdata.exe)";
            this.type3.Click += new System.EventHandler(this.type3_Click);
            // 
            // encryptReplacementMIDI
            // 
            this.encryptReplacementMIDI.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.encryptReplacementMIDI.Name = "encryptReplacementMIDI";
            this.encryptReplacementMIDI.Size = new System.Drawing.Size(243, 22);
            this.encryptReplacementMIDI.Text = "Encrypt replacement MIDI file(s)";
            this.encryptReplacementMIDI.Click += new System.EventHandler(this.encryptReplacementMIDI_Click);
            // 
            // encryptReplacementMogg
            // 
            this.encryptReplacementMogg.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.encryptReplacementMogg.Name = "encryptReplacementMogg";
            this.encryptReplacementMogg.Size = new System.Drawing.Size(243, 22);
            this.encryptReplacementMogg.Text = "Encrypt replacement mogg file";
            this.encryptReplacementMogg.Click += new System.EventHandler(this.encryptReplacementMogg_Click);
            // 
            // regionOptionsToolStripMenuItem
            // 
            this.regionOptionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.regionNTSC,
            this.regionPAL});
            this.regionOptionsToolStripMenuItem.Name = "regionOptionsToolStripMenuItem";
            this.regionOptionsToolStripMenuItem.Size = new System.Drawing.Size(101, 20);
            this.regionOptionsToolStripMenuItem.Text = "Region Options";
            this.regionOptionsToolStripMenuItem.Visible = false;
            // 
            // regionNTSC
            // 
            this.regionNTSC.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.regionNTSC.Checked = true;
            this.regionNTSC.CheckState = System.Windows.Forms.CheckState.Checked;
            this.regionNTSC.Name = "regionNTSC";
            this.regionNTSC.Size = new System.Drawing.Size(104, 22);
            this.regionNTSC.Text = "NTSC";
            this.regionNTSC.Click += new System.EventHandler(this.regionNTSC_Click);
            // 
            // regionPAL
            // 
            this.regionPAL.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.regionPAL.Name = "regionPAL";
            this.regionPAL.Size = new System.Drawing.Size(104, 22);
            this.regionPAL.Text = "PAL";
            this.regionPAL.Click += new System.EventHandler(this.regionPAL_Click);
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
            this.btnReset.Location = new System.Drawing.Point(12, 109);
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
            this.chkRAR.Location = new System.Drawing.Point(12, 86);
            this.chkRAR.Name = "chkRAR";
            this.chkRAR.Size = new System.Drawing.Size(189, 17);
            this.chkRAR.TabIndex = 55;
            this.chkRAR.Text = "Create RAR archive for each song";
            this.chkRAR.UseVisualStyleBackColor = true;
            // 
            // picWorking
            // 
            this.picWorking.Image = global::Nautilus.Properties.Resources.working;
            this.picWorking.Location = new System.Drawing.Point(240, 123);
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
            // btnOnyx
            // 
            this.btnOnyx.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(129)))), ((int)(((byte)(216)))));
            this.btnOnyx.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOnyx.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOnyx.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOnyx.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOnyx.ForeColor = System.Drawing.Color.White;
            this.btnOnyx.Location = new System.Drawing.Point(255, 27);
            this.btnOnyx.Name = "btnOnyx";
            this.btnOnyx.Size = new System.Drawing.Size(134, 30);
            this.btnOnyx.TabIndex = 67;
            this.btnOnyx.Text = "Click Here For Ony&x";
            this.btnOnyx.UseVisualStyleBackColor = false;
            this.btnOnyx.Click += new System.EventHandler(this.btnOnyx_Click);
            // 
            // backgroundWorker4
            // 
            this.backgroundWorker4.WorkerReportsProgress = true;
            this.backgroundWorker4.WorkerSupportsCancellation = true;
            this.backgroundWorker4.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker4_DoWork);
            this.backgroundWorker4.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker4_RunWorkerCompleted);
            // 
            // PS3Converter
            // 
            this.AllowDrop = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(592, 379);
            this.Controls.Add(this.btnOnyx);
            this.Controls.Add(this.picOnyx);
            this.Controls.Add(this.picPin);
            this.Controls.Add(this.chkSongID);
            this.Controls.Add(this.chkMerge);
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
            this.Name = "PS3Converter";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PS3 Converter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PS3Converter_FormClosing);
            this.Shown += new System.EventHandler(this.PS3Prep_Shown);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            this.Resize += new System.EventHandler(this.PS3Prep_Resize);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picPin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picOnyx)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picWorking)).EndInit();
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
        private System.Windows.Forms.PictureBox picWorking;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.CheckBox chkMerge;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mergeSongsToolStrip;
        private System.Windows.Forms.ToolStripMenuItem managePackDTAFile;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mIDIToEDATEncryptionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem type1;
        private System.Windows.Forms.ToolStripMenuItem type2;
        private System.Windows.Forms.ToolStripMenuItem wait2Seconds;
        private System.Windows.Forms.ToolStripMenuItem wait5Seconds;
        private System.Windows.Forms.ToolStripMenuItem wait10Seconds;
        private System.Windows.Forms.ToolStripMenuItem encryptReplacementMogg;
        private System.Windows.Forms.ToolStripMenuItem encryptReplacementMIDI;
        private System.Windows.Forms.CheckBox chkSongID;
        private System.Windows.Forms.ToolStripMenuItem numericIDOptionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem batchChangeIDsToNumeric;
        private System.Windows.Forms.ToolStripMenuItem regionOptionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem regionNTSC;
        private System.Windows.Forms.ToolStripMenuItem regionPAL;
        private System.Windows.Forms.ToolStripMenuItem type3;
        private System.Windows.Forms.PictureBox picPin;
        private System.Windows.Forms.ToolStripMenuItem downmixAndEncode;
        private System.Windows.Forms.ToolStripMenuItem pS3ToCONToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem songInFolderFormat;
        private System.Windows.Forms.ToolStripMenuItem pkgToCON;
        private System.Windows.Forms.ToolStripMenuItem patchMoggForPS3UseToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem pS3Fixer;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
        private System.ComponentModel.BackgroundWorker backgroundWorker3;
        private System.Windows.Forms.PictureBox picOnyx;
        private System.Windows.Forms.Button btnOnyx;
        private System.ComponentModel.BackgroundWorker backgroundWorker4;
        private System.Windows.Forms.ToolStripMenuItem batchFixLoopingSongs;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
    }
}