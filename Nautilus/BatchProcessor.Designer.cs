namespace Nautilus
{
    partial class BatchProcessor
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
            this.FileProcessor = new System.ComponentModel.BackgroundWorker();
            this.picWorking = new System.Windows.Forms.PictureBox();
            this.folderScanner = new System.ComponentModel.BackgroundWorker();
            this.PhaseShiftRenamer = new System.ComponentModel.BackgroundWorker();
            this.chkAuthor = new System.Windows.Forms.CheckBox();
            this.chkOrigin = new System.Windows.Forms.CheckBox();
            this.txtOrigin = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.separateDTA = new System.Windows.Forms.ToolStripMenuItem();
            this.sortSongsByDTALanguageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txtAuthor = new System.Windows.Forms.TextBox();
            this.chkOverrideAuthor = new System.Windows.Forms.CheckBox();
            this.chkVocalGender = new System.Windows.Forms.CheckBox();
            this.cboVocalGender = new System.Windows.Forms.ComboBox();
            this.txtFallback = new System.Windows.Forms.TextBox();
            this.chkFallback = new System.Windows.Forms.CheckBox();
            this.chkRecursive = new System.Windows.Forms.CheckBox();
            this.chkSongID = new System.Windows.Forms.CheckBox();
            this.chkOverrideGameID = new System.Windows.Forms.CheckBox();
            this.gameRB1 = new System.Windows.Forms.RadioButton();
            this.gameRB2 = new System.Windows.Forms.RadioButton();
            this.gameRB3 = new System.Windows.Forms.RadioButton();
            this.chkAddYear = new System.Windows.Forms.CheckBox();
            this.chkFamilyFriendly = new System.Windows.Forms.CheckBox();
            this.chkDIYStems = new System.Windows.Forms.CheckBox();
            this.chkSubgenre = new System.Windows.Forms.CheckBox();
            this.chkForceNumericOverride = new System.Windows.Forms.CheckBox();
            this.chkDefAutID = new System.Windows.Forms.CheckBox();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picWorking)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(251)))), ((int)(((byte)(211)))), ((int)(((byte)(0)))));
            this.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefresh.ForeColor = System.Drawing.Color.Black;
            this.btnRefresh.Location = new System.Drawing.Point(488, 30);
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
            this.btnFolder.Location = new System.Drawing.Point(12, 30);
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
            this.txtFolder.Location = new System.Drawing.Point(12, 66);
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
            this.lstLog.Location = new System.Drawing.Point(12, 92);
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
            this.btnBegin.Location = new System.Drawing.Point(543, 458);
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
            this.picPin.Location = new System.Drawing.Point(582, 4);
            this.picPin.Name = "picPin";
            this.picPin.Size = new System.Drawing.Size(20, 20);
            this.picPin.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picPin.TabIndex = 64;
            this.picPin.TabStop = false;
            this.picPin.Tag = "unpinned";
            this.toolTip1.SetToolTip(this.picPin, "Click to pin on top");
            this.picPin.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picPin_MouseClick);
            // 
            // FileProcessor
            // 
            this.FileProcessor.WorkerReportsProgress = true;
            this.FileProcessor.WorkerSupportsCancellation = true;
            this.FileProcessor.DoWork += new System.ComponentModel.DoWorkEventHandler(this.FileProcessor_DoWork);
            this.FileProcessor.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.FileProcessor_RunWorkerCompleted);
            // 
            // picWorking
            // 
            this.picWorking.Image = global::Nautilus.Properties.Resources.working;
            this.picWorking.Location = new System.Drawing.Point(252, 45);
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
            // chkAuthor
            // 
            this.chkAuthor.AutoSize = true;
            this.chkAuthor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkAuthor.Location = new System.Drawing.Point(12, 308);
            this.chkAuthor.Name = "chkAuthor";
            this.chkAuthor.Size = new System.Drawing.Size(310, 17);
            this.chkAuthor.TabIndex = 65;
            this.chkAuthor.Text = "Move author information from Magma comment to DTA array";
            this.chkAuthor.UseVisualStyleBackColor = true;
            // 
            // chkOrigin
            // 
            this.chkOrigin.AutoSize = true;
            this.chkOrigin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkOrigin.Location = new System.Drawing.Point(12, 355);
            this.chkOrigin.Name = "chkOrigin";
            this.chkOrigin.Size = new System.Drawing.Size(224, 17);
            this.chkOrigin.TabIndex = 66;
            this.chkOrigin.Text = "Override game_origin with following value:";
            this.chkOrigin.UseVisualStyleBackColor = true;
            this.chkOrigin.CheckedChanged += new System.EventHandler(this.chkOrigin_CheckedChanged);
            // 
            // txtOrigin
            // 
            this.txtOrigin.Enabled = false;
            this.txtOrigin.Location = new System.Drawing.Point(238, 353);
            this.txtOrigin.Name = "txtOrigin";
            this.txtOrigin.Size = new System.Drawing.Size(138, 20);
            this.txtOrigin.TabIndex = 67;
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.separateDTA,
            this.sortSongsByDTALanguageToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(614, 24);
            this.menuStrip1.TabIndex = 68;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // separateDTA
            // 
            this.separateDTA.Name = "separateDTA";
            this.separateDTA.Size = new System.Drawing.Size(159, 20);
            this.separateDTA.Text = "Separate bundled DTA files";
            this.separateDTA.Click += new System.EventHandler(this.separateDTA_Click);
            // 
            // sortSongsByDTALanguageToolStripMenuItem
            // 
            this.sortSongsByDTALanguageToolStripMenuItem.Name = "sortSongsByDTALanguageToolStripMenuItem";
            this.sortSongsByDTALanguageToolStripMenuItem.Size = new System.Drawing.Size(166, 20);
            this.sortSongsByDTALanguageToolStripMenuItem.Text = "Sort songs by DTA language";
            this.sortSongsByDTALanguageToolStripMenuItem.Click += new System.EventHandler(this.sortSongsByDTALanguageToolStripMenuItem_Click);
            // 
            // txtAuthor
            // 
            this.txtAuthor.Enabled = false;
            this.txtAuthor.Location = new System.Drawing.Point(238, 328);
            this.txtAuthor.Name = "txtAuthor";
            this.txtAuthor.Size = new System.Drawing.Size(138, 20);
            this.txtAuthor.TabIndex = 70;
            // 
            // chkOverrideAuthor
            // 
            this.chkOverrideAuthor.AutoSize = true;
            this.chkOverrideAuthor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkOverrideAuthor.Location = new System.Drawing.Point(12, 330);
            this.chkOverrideAuthor.Name = "chkOverrideAuthor";
            this.chkOverrideAuthor.Size = new System.Drawing.Size(197, 17);
            this.chkOverrideAuthor.TabIndex = 69;
            this.chkOverrideAuthor.Text = "Override author with following value:";
            this.chkOverrideAuthor.UseVisualStyleBackColor = true;
            this.chkOverrideAuthor.CheckedChanged += new System.EventHandler(this.chkOverrideAuthor_CheckedChanged);
            // 
            // chkVocalGender
            // 
            this.chkVocalGender.AutoSize = true;
            this.chkVocalGender.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkVocalGender.Location = new System.Drawing.Point(12, 379);
            this.chkVocalGender.Name = "chkVocalGender";
            this.chkVocalGender.Size = new System.Drawing.Size(232, 17);
            this.chkVocalGender.TabIndex = 71;
            this.chkVocalGender.Text = "Override vocal_gender with following value:";
            this.chkVocalGender.UseVisualStyleBackColor = true;
            this.chkVocalGender.CheckedChanged += new System.EventHandler(this.chkVocalGender_CheckedChanged);
            // 
            // cboVocalGender
            // 
            this.cboVocalGender.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboVocalGender.Enabled = false;
            this.cboVocalGender.FormattingEnabled = true;
            this.cboVocalGender.Items.AddRange(new object[] {
            "Female",
            "Male"});
            this.cboVocalGender.Location = new System.Drawing.Point(255, 377);
            this.cboVocalGender.Name = "cboVocalGender";
            this.cboVocalGender.Size = new System.Drawing.Size(121, 21);
            this.cboVocalGender.TabIndex = 72;
            // 
            // txtFallback
            // 
            this.txtFallback.Enabled = false;
            this.txtFallback.Location = new System.Drawing.Point(464, 353);
            this.txtFallback.Name = "txtFallback";
            this.txtFallback.Size = new System.Drawing.Size(138, 20);
            this.txtFallback.TabIndex = 74;
            this.txtFallback.Text = "ugc_plus";
            // 
            // chkFallback
            // 
            this.chkFallback.AutoSize = true;
            this.chkFallback.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkFallback.Location = new System.Drawing.Point(396, 355);
            this.chkFallback.Name = "chkFallback";
            this.chkFallback.Size = new System.Drawing.Size(66, 17);
            this.chkFallback.TabIndex = 73;
            this.chkFallback.Text = "Fallback";
            this.chkFallback.UseVisualStyleBackColor = true;
            this.chkFallback.CheckedChanged += new System.EventHandler(this.chkFallback_CheckedChanged);
            // 
            // chkRecursive
            // 
            this.chkRecursive.AutoSize = true;
            this.chkRecursive.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkRecursive.Location = new System.Drawing.Point(396, 330);
            this.chkRecursive.Name = "chkRecursive";
            this.chkRecursive.Size = new System.Drawing.Size(129, 17);
            this.chkRecursive.TabIndex = 75;
            this.chkRecursive.Text = "Recursive searching?";
            this.chkRecursive.UseVisualStyleBackColor = true;
            // 
            // chkSongID
            // 
            this.chkSongID.AutoSize = true;
            this.chkSongID.Checked = true;
            this.chkSongID.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSongID.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkSongID.Location = new System.Drawing.Point(12, 402);
            this.chkSongID.Name = "chkSongID";
            this.chkSongID.Size = new System.Drawing.Size(344, 17);
            this.chkSongID.TabIndex = 76;
            this.chkSongID.Text = "Automatically change alphanumeric song IDs to unique numeric IDs";
            this.chkSongID.UseVisualStyleBackColor = true;
            // 
            // chkOverrideGameID
            // 
            this.chkOverrideGameID.AutoSize = true;
            this.chkOverrideGameID.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkOverrideGameID.Location = new System.Drawing.Point(12, 425);
            this.chkOverrideGameID.Name = "chkOverrideGameID";
            this.chkOverrideGameID.Size = new System.Drawing.Size(178, 17);
            this.chkOverrideGameID.TabIndex = 77;
            this.chkOverrideGameID.Text = "Override game ID with following:";
            this.chkOverrideGameID.UseVisualStyleBackColor = true;
            this.chkOverrideGameID.CheckedChanged += new System.EventHandler(this.chkOverrideGameID_CheckedChanged);
            // 
            // gameRB1
            // 
            this.gameRB1.AutoSize = true;
            this.gameRB1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.gameRB1.Enabled = false;
            this.gameRB1.Location = new System.Drawing.Point(196, 425);
            this.gameRB1.Name = "gameRB1";
            this.gameRB1.Size = new System.Drawing.Size(46, 17);
            this.gameRB1.TabIndex = 78;
            this.gameRB1.Text = "RB1";
            this.gameRB1.UseVisualStyleBackColor = true;
            // 
            // gameRB2
            // 
            this.gameRB2.AutoSize = true;
            this.gameRB2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.gameRB2.Enabled = false;
            this.gameRB2.Location = new System.Drawing.Point(248, 425);
            this.gameRB2.Name = "gameRB2";
            this.gameRB2.Size = new System.Drawing.Size(46, 17);
            this.gameRB2.TabIndex = 79;
            this.gameRB2.Text = "RB2";
            this.gameRB2.UseVisualStyleBackColor = true;
            // 
            // gameRB3
            // 
            this.gameRB3.AutoSize = true;
            this.gameRB3.Checked = true;
            this.gameRB3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.gameRB3.Enabled = false;
            this.gameRB3.Location = new System.Drawing.Point(300, 425);
            this.gameRB3.Name = "gameRB3";
            this.gameRB3.Size = new System.Drawing.Size(46, 17);
            this.gameRB3.TabIndex = 80;
            this.gameRB3.TabStop = true;
            this.gameRB3.Text = "RB3";
            this.gameRB3.UseVisualStyleBackColor = true;
            // 
            // chkAddYear
            // 
            this.chkAddYear.AutoSize = true;
            this.chkAddYear.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkAddYear.Location = new System.Drawing.Point(396, 379);
            this.chkAddYear.Name = "chkAddYear";
            this.chkAddYear.Size = new System.Drawing.Size(135, 17);
            this.chkAddYear.TabIndex = 81;
            this.chkAddYear.Text = "Add year to song name";
            this.chkAddYear.UseVisualStyleBackColor = true;
            // 
            // chkFamilyFriendly
            // 
            this.chkFamilyFriendly.AutoSize = true;
            this.chkFamilyFriendly.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkFamilyFriendly.Location = new System.Drawing.Point(12, 494);
            this.chkFamilyFriendly.Name = "chkFamilyFriendly";
            this.chkFamilyFriendly.Size = new System.Drawing.Size(175, 17);
            this.chkFamilyFriendly.TabIndex = 83;
            this.chkFamilyFriendly.Text = "Change rating to Family Friendly";
            this.chkFamilyFriendly.UseVisualStyleBackColor = true;
            // 
            // chkDIYStems
            // 
            this.chkDIYStems.AutoSize = true;
            this.chkDIYStems.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkDIYStems.Location = new System.Drawing.Point(12, 448);
            this.chkDIYStems.Name = "chkDIYStems";
            this.chkDIYStems.Size = new System.Drawing.Size(331, 17);
            this.chkDIYStems.TabIndex = 82;
            this.chkDIYStems.Text = "Override Magma\'s Karaoke or Multitrack tags with DIY Stems tag";
            this.chkDIYStems.UseVisualStyleBackColor = true;
            // 
            // chkSubgenre
            // 
            this.chkSubgenre.AutoSize = true;
            this.chkSubgenre.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkSubgenre.Location = new System.Drawing.Point(12, 471);
            this.chkSubgenre.Name = "chkSubgenre";
            this.chkSubgenre.Size = new System.Drawing.Size(177, 17);
            this.chkSubgenre.TabIndex = 83;
            this.chkSubgenre.Text = "Remove subgenre from DTA file";
            this.chkSubgenre.UseVisualStyleBackColor = true;
            // 
            // chkForceNumericOverride
            // 
            this.chkForceNumericOverride.AutoSize = true;
            this.chkForceNumericOverride.Checked = true;
            this.chkForceNumericOverride.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkForceNumericOverride.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkForceNumericOverride.Location = new System.Drawing.Point(396, 402);
            this.chkForceNumericOverride.Name = "chkForceNumericOverride";
            this.chkForceNumericOverride.Size = new System.Drawing.Size(211, 17);
            this.chkForceNumericOverride.TabIndex = 84;
            this.chkForceNumericOverride.Text = "Force overwrite even if already numeric";
            this.chkForceNumericOverride.UseVisualStyleBackColor = true;
            // 
            // chkDefAutID
            // 
            this.chkDefAutID.AutoSize = true;
            this.chkDefAutID.Checked = true;
            this.chkDefAutID.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDefAutID.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkDefAutID.Location = new System.Drawing.Point(396, 425);
            this.chkDefAutID.Name = "chkDefAutID";
            this.chkDefAutID.Size = new System.Drawing.Size(212, 17);
            this.chkDefAutID.TabIndex = 84;
            this.chkDefAutID.Text = "Automatically correct default Author IDs";
            this.chkDefAutID.UseVisualStyleBackColor = true;
            // 
            // BatchProcessor
            // 
            this.AllowDrop = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(614, 519);
            this.Controls.Add(this.chkDefAutID);
            this.Controls.Add(this.chkForceNumericOverride);
            this.Controls.Add(this.chkSubgenre);
            this.Controls.Add(this.chkDIYStems);
            this.Controls.Add(this.chkAddYear);
            this.Controls.Add(this.gameRB3);
            this.Controls.Add(this.gameRB2);
            this.Controls.Add(this.gameRB1);
            this.Controls.Add(this.chkOverrideGameID);
            this.Controls.Add(this.chkSongID);
            this.Controls.Add(this.chkRecursive);
            this.Controls.Add(this.txtFallback);
            this.Controls.Add(this.chkFallback);
            this.Controls.Add(this.cboVocalGender);
            this.Controls.Add(this.chkVocalGender);
            this.Controls.Add(this.txtAuthor);
            this.Controls.Add(this.chkOverrideAuthor);
            this.Controls.Add(this.txtOrigin);
            this.Controls.Add(this.chkOrigin);
            this.Controls.Add(this.chkFamilyFriendly);
            this.Controls.Add(this.chkAuthor);
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
            this.MaximizeBox = false;
            this.Name = "BatchProcessor";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Batch Processor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DTAProcessor_FormClosing);
            this.Shown += new System.EventHandler(this.DTAProcessor_Shown);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picPin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picWorking)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
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
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem exportLogFileToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker FileProcessor;
        private System.Windows.Forms.PictureBox picWorking;
        private System.ComponentModel.BackgroundWorker folderScanner;
        private System.ComponentModel.BackgroundWorker PhaseShiftRenamer;
        private System.Windows.Forms.PictureBox picPin;
        private System.Windows.Forms.CheckBox chkAuthor;
        private System.Windows.Forms.CheckBox chkFamilyFriendly;
        private System.Windows.Forms.CheckBox chkOrigin;
        private System.Windows.Forms.TextBox txtOrigin;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem separateDTA;
        private System.Windows.Forms.TextBox txtAuthor;
        private System.Windows.Forms.CheckBox chkOverrideAuthor;
        private System.Windows.Forms.CheckBox chkVocalGender;
        private System.Windows.Forms.ComboBox cboVocalGender;
        private System.Windows.Forms.TextBox txtFallback;
        private System.Windows.Forms.CheckBox chkFallback;
        private System.Windows.Forms.CheckBox chkRecursive;
        private System.Windows.Forms.CheckBox chkSongID;
        private System.Windows.Forms.CheckBox chkOverrideGameID;
        private System.Windows.Forms.RadioButton gameRB1;
        private System.Windows.Forms.RadioButton gameRB2;
        private System.Windows.Forms.RadioButton gameRB3;
        private System.Windows.Forms.CheckBox chkAddYear;
        private System.Windows.Forms.CheckBox chkDIYStems;
        private System.Windows.Forms.CheckBox chkSubgenre;
        private System.Windows.Forms.ToolStripMenuItem sortSongsByDTALanguageToolStripMenuItem;
        private System.Windows.Forms.CheckBox chkForceNumericOverride;
        private System.Windows.Forms.CheckBox chkDefAutID;
    }
}