namespace Nautilus
{
    partial class VolumeNormalizer
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
            this.chkAlbumMode = new System.Windows.Forms.CheckBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chkRestore = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.chkAnalyzerMode = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.chkBackupAudio = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.chkVerboseOutput = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnReset = new System.Windows.Forms.Button();
            this.txtFolder = new System.Windows.Forms.TextBox();
            this.btnFolder = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.picWorking = new System.Windows.Forms.PictureBox();
            this.grpMogg = new System.Windows.Forms.GroupBox();
            this.numThresholdValue = new System.Windows.Forms.NumericUpDown();
            this.lblThreshold = new System.Windows.Forms.Label();
            this.numTargetValue = new System.Windows.Forms.NumericUpDown();
            this.lblTargetVolume = new System.Windows.Forms.Label();
            this.radioDoNotRender = new System.Windows.Forms.RadioButton();
            this.radioAllowRender = new System.Windows.Forms.RadioButton();
            this.lblResetVolEnabled = new System.Windows.Forms.Label();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPin)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picWorking)).BeginInit();
            this.grpMogg.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numThresholdValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTargetValue)).BeginInit();
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
            this.lstLog.Location = new System.Drawing.Point(12, 200);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(568, 145);
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
            this.btnBegin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(89)))), ((int)(((byte)(201)))));
            this.btnBegin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBegin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBegin.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBegin.ForeColor = System.Drawing.Color.White;
            this.btnBegin.Location = new System.Drawing.Point(516, 167);
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
            // chkAlbumMode
            // 
            this.chkAlbumMode.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chkAlbumMode.AutoSize = true;
            this.chkAlbumMode.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkAlbumMode.Location = new System.Drawing.Point(213, 34);
            this.chkAlbumMode.Name = "chkAlbumMode";
            this.chkAlbumMode.Size = new System.Drawing.Size(201, 17);
            this.chkAlbumMode.TabIndex = 73;
            this.chkAlbumMode.Text = "Adjust all CONs equally (album mode)";
            this.chkAlbumMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkAlbumMode.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem,
            this.helpToolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(592, 24);
            this.menuStrip1.TabIndex = 35;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.chkRestore,
            this.toolStripSeparator1,
            this.chkAnalyzerMode,
            this.toolStripSeparator2,
            this.chkBackupAudio,
            this.toolStripSeparator3,
            this.chkVerboseOutput});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // chkRestore
            // 
            this.chkRestore.CheckOnClick = true;
            this.chkRestore.Name = "chkRestore";
            this.chkRestore.Size = new System.Drawing.Size(234, 22);
            this.chkRestore.Text = "Restore Original Volumes";
            this.chkRestore.Click += new System.EventHandler(this.chkRestore_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(231, 6);
            // 
            // chkAnalyzerMode
            // 
            this.chkAnalyzerMode.AutoToolTip = true;
            this.chkAnalyzerMode.CheckOnClick = true;
            this.chkAnalyzerMode.Name = "chkAnalyzerMode";
            this.chkAnalyzerMode.Size = new System.Drawing.Size(234, 22);
            this.chkAnalyzerMode.Text = "Analyzer Mode";
            this.chkAnalyzerMode.ToolTipText = "Only read audio levels of CONs in folder.";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(231, 6);
            // 
            // chkBackupAudio
            // 
            this.chkBackupAudio.Checked = true;
            this.chkBackupAudio.CheckOnClick = true;
            this.chkBackupAudio.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBackupAudio.Name = "chkBackupAudio";
            this.chkBackupAudio.Size = new System.Drawing.Size(234, 22);
            this.chkBackupAudio.Text = "Backup audio on modification";
            this.chkBackupAudio.Click += new System.EventHandler(this.chkBackupAudio_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(231, 6);
            // 
            // chkVerboseOutput
            // 
            this.chkVerboseOutput.CheckOnClick = true;
            this.chkVerboseOutput.Name = "chkVerboseOutput";
            this.chkVerboseOutput.Size = new System.Drawing.Size(234, 22);
            this.chkVerboseOutput.Text = "Verbose Output";
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
            this.btnReset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(89)))), ((int)(((byte)(201)))));
            this.btnReset.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReset.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReset.ForeColor = System.Drawing.Color.White;
            this.btnReset.Location = new System.Drawing.Point(12, 167);
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
            this.btnFolder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(89)))), ((int)(((byte)(201)))));
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
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(89)))), ((int)(((byte)(201)))));
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
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // picWorking
            // 
            this.picWorking.Image = global::Nautilus.Properties.Resources.working;
            this.picWorking.Location = new System.Drawing.Point(231, 179);
            this.picWorking.Name = "picWorking";
            this.picWorking.Size = new System.Drawing.Size(128, 15);
            this.picWorking.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picWorking.TabIndex = 62;
            this.picWorking.TabStop = false;
            this.picWorking.Visible = false;
            // 
            // grpMogg
            // 
            this.grpMogg.Controls.Add(this.numThresholdValue);
            this.grpMogg.Controls.Add(this.lblThreshold);
            this.grpMogg.Controls.Add(this.numTargetValue);
            this.grpMogg.Controls.Add(this.lblTargetVolume);
            this.grpMogg.Controls.Add(this.radioDoNotRender);
            this.grpMogg.Controls.Add(this.radioAllowRender);
            this.grpMogg.Location = new System.Drawing.Point(12, 86);
            this.grpMogg.Name = "grpMogg";
            this.grpMogg.Size = new System.Drawing.Size(568, 75);
            this.grpMogg.TabIndex = 72;
            this.grpMogg.TabStop = false;
            this.grpMogg.Text = "Audio (mogg) options:";
            // 
            // numThresholdValue
            // 
            this.numThresholdValue.DecimalPlaces = 1;
            this.numThresholdValue.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numThresholdValue.Location = new System.Drawing.Point(513, 45);
            this.numThresholdValue.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            65536});
            this.numThresholdValue.Name = "numThresholdValue";
            this.numThresholdValue.Size = new System.Drawing.Size(49, 20);
            this.numThresholdValue.TabIndex = 73;
            this.numThresholdValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numThresholdValue.Value = new decimal(new int[] {
            4,
            0,
            0,
            65536});
            // 
            // lblThreshold
            // 
            this.lblThreshold.AutoSize = true;
            this.lblThreshold.Location = new System.Drawing.Point(378, 47);
            this.lblThreshold.Name = "lblThreshold";
            this.lblThreshold.Size = new System.Drawing.Size(129, 13);
            this.lblThreshold.TabIndex = 72;
            this.lblThreshold.Text = "Re-render Threshold (dB):";
            // 
            // numTargetValue
            // 
            this.numTargetValue.DecimalPlaces = 1;
            this.numTargetValue.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numTargetValue.Location = new System.Drawing.Point(513, 20);
            this.numTargetValue.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numTargetValue.Minimum = new decimal(new int[] {
            80,
            0,
            0,
            -2147418112});
            this.numTargetValue.Name = "numTargetValue";
            this.numTargetValue.Size = new System.Drawing.Size(49, 20);
            this.numTargetValue.TabIndex = 71;
            this.numTargetValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numTargetValue.Value = new decimal(new int[] {
            20,
            0,
            0,
            -2147418112});
            // 
            // lblTargetVolume
            // 
            this.lblTargetVolume.AutoSize = true;
            this.lblTargetVolume.Location = new System.Drawing.Point(406, 22);
            this.lblTargetVolume.Name = "lblTargetVolume";
            this.lblTargetVolume.Size = new System.Drawing.Size(101, 13);
            this.lblTargetVolume.TabIndex = 70;
            this.lblTargetVolume.Text = "Target Volume (dB):";
            // 
            // radioDoNotRender
            // 
            this.radioDoNotRender.AutoSize = true;
            this.radioDoNotRender.Checked = true;
            this.radioDoNotRender.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioDoNotRender.Location = new System.Drawing.Point(10, 43);
            this.radioDoNotRender.Name = "radioDoNotRender";
            this.radioDoNotRender.Size = new System.Drawing.Size(124, 17);
            this.radioDoNotRender.TabIndex = 69;
            this.radioDoNotRender.TabStop = true;
            this.radioDoNotRender.Text = "Don\'t re-render audio";
            this.radioDoNotRender.UseVisualStyleBackColor = true;
            // 
            // radioAllowRender
            // 
            this.radioAllowRender.AutoSize = true;
            this.radioAllowRender.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioAllowRender.Location = new System.Drawing.Point(9, 20);
            this.radioAllowRender.Name = "radioAllowRender";
            this.radioAllowRender.Size = new System.Drawing.Size(138, 17);
            this.radioAllowRender.TabIndex = 63;
            this.radioAllowRender.Text = "Allow re-rendering audio";
            this.radioAllowRender.UseVisualStyleBackColor = true;
            // 
            // lblResetVolEnabled
            // 
            this.lblResetVolEnabled.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblResetVolEnabled.AutoSize = true;
            this.lblResetVolEnabled.ForeColor = System.Drawing.Color.Red;
            this.lblResetVolEnabled.Location = new System.Drawing.Point(213, 175);
            this.lblResetVolEnabled.Name = "lblResetVolEnabled";
            this.lblResetVolEnabled.Size = new System.Drawing.Size(166, 13);
            this.lblResetVolEnabled.TabIndex = 74;
            this.lblResetVolEnabled.Text = "Restore Original Volumes enabled";
            this.lblResetVolEnabled.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblResetVolEnabled.Visible = false;
            // 
            // VolumeNormalizer
            // 
            this.AllowDrop = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(592, 356);
            this.Controls.Add(this.chkAlbumMode);
            this.Controls.Add(this.grpMogg);
            this.Controls.Add(this.picPin);
            this.Controls.Add(this.picWorking);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.txtFolder);
            this.Controls.Add(this.btnFolder);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnBegin);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.lstLog);
            this.Controls.Add(this.lblResetVolEnabled);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(608, 395);
            this.Name = "VolumeNormalizer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Volume Normalizer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.VolumeNormalizerPrep_FormClosing);
            this.Shown += new System.EventHandler(this.VolumeNormalizerPrep_Shown);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            this.Resize += new System.EventHandler(this.VolumeNormalizerPrep_Resize);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picPin)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picWorking)).EndInit();
            this.grpMogg.ResumeLayout(false);
            this.grpMogg.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numThresholdValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTargetValue)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
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
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.PictureBox picPin;
        private System.Windows.Forms.ToolStripMenuItem chkRestore;
        private System.Windows.Forms.PictureBox picWorking;
        private System.Windows.Forms.GroupBox grpMogg;
        private System.Windows.Forms.RadioButton radioDoNotRender;
        private System.Windows.Forms.RadioButton radioAllowRender;
        private System.Windows.Forms.NumericUpDown numTargetValue;
        private System.Windows.Forms.Label lblTargetVolume;
        private System.Windows.Forms.ToolStripMenuItem chkBackupAudio;
        private System.Windows.Forms.ToolStripMenuItem chkAnalyzerMode;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.NumericUpDown numThresholdValue;
        private System.Windows.Forms.Label lblThreshold;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem chkVerboseOutput;
        public System.Windows.Forms.ListBox lstLog;
        private System.Windows.Forms.CheckBox chkAlbumMode;
        private System.Windows.Forms.Label lblResetVolEnabled;
    }
}