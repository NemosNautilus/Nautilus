namespace Nautilus
{
    partial class StemsIsolator
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
            this.btnBegin = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.picPin = new System.Windows.Forms.PictureBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.audioFormatToolStrip = new System.Windows.Forms.ToolStripMenuItem();
            this.doFLAC = new System.Windows.Forms.ToolStripMenuItem();
            this.doMP3 = new System.Windows.Forms.ToolStripMenuItem();
            this.doOGG = new System.Windows.Forms.ToolStripMenuItem();
            this.doOPUS = new System.Windows.Forms.ToolStripMenuItem();
            this.doWAV = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnReset = new System.Windows.Forms.Button();
            this.grpStems = new System.Windows.Forms.GroupBox();
            this.chkBacking = new System.Windows.Forms.CheckBox();
            this.chkCrowd = new System.Windows.Forms.CheckBox();
            this.chkVocals = new System.Windows.Forms.CheckBox();
            this.chkKeys = new System.Windows.Forms.CheckBox();
            this.chkGuitar = new System.Windows.Forms.CheckBox();
            this.chkBass = new System.Windows.Forms.CheckBox();
            this.chkDrums = new System.Windows.Forms.CheckBox();
            this.txtFile = new System.Windows.Forms.TextBox();
            this.btnFile = new System.Windows.Forms.Button();
            this.picWorking = new System.Windows.Forms.PictureBox();
            this.grpMode = new System.Windows.Forms.GroupBox();
            this.chkIgnore = new System.Windows.Forms.CheckBox();
            this.radioDownmix = new System.Windows.Forms.RadioButton();
            this.radioSplit = new System.Windows.Forms.RadioButton();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPin)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.grpStems.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picWorking)).BeginInit();
            this.grpMode.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstLog
            // 
            this.lstLog.AllowDrop = true;
            this.lstLog.BackColor = System.Drawing.Color.White;
            this.lstLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstLog.ContextMenuStrip = this.contextMenuStrip1;
            this.lstLog.FormattingEnabled = true;
            this.lstLog.HorizontalScrollbar = true;
            this.lstLog.Location = new System.Drawing.Point(12, 218);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(456, 119);
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
            this.btnBegin.AllowDrop = true;
            this.btnBegin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(144)))), ((int)(((byte)(51)))));
            this.btnBegin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBegin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBegin.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBegin.ForeColor = System.Drawing.Color.White;
            this.btnBegin.Location = new System.Drawing.Point(404, 183);
            this.btnBegin.Name = "btnBegin";
            this.btnBegin.Size = new System.Drawing.Size(64, 29);
            this.btnBegin.TabIndex = 19;
            this.btnBegin.Text = "&Begin";
            this.btnBegin.UseVisualStyleBackColor = false;
            this.btnBegin.Visible = false;
            this.btnBegin.Click += new System.EventHandler(this.button1_Click);
            this.btnBegin.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.btnBegin.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // picPin
            // 
            this.picPin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picPin.BackColor = System.Drawing.Color.Transparent;
            this.picPin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picPin.Image = global::Nautilus.Properties.Resources.unpinned;
            this.picPin.Location = new System.Drawing.Point(454, 4);
            this.picPin.Name = "picPin";
            this.picPin.Size = new System.Drawing.Size(20, 20);
            this.picPin.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picPin.TabIndex = 64;
            this.picPin.TabStop = false;
            this.picPin.Tag = "unpinned";
            this.toolTip1.SetToolTip(this.picPin, "Click to pin on top");
            this.picPin.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picPin_MouseClick);
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
            this.audioFormatToolStrip,
            this.helpToolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(479, 24);
            this.menuStrip1.TabIndex = 35;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // audioFormatToolStrip
            // 
            this.audioFormatToolStrip.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.doFLAC,
            this.doMP3,
            this.doOGG,
            this.doOPUS,
            this.doWAV});
            this.audioFormatToolStrip.Name = "audioFormatToolStrip";
            this.audioFormatToolStrip.Size = new System.Drawing.Size(92, 20);
            this.audioFormatToolStrip.Text = "Audio Format";
            // 
            // doFLAC
            // 
            this.doFLAC.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.doFLAC.Name = "doFLAC";
            this.doFLAC.Size = new System.Drawing.Size(104, 22);
            this.doFLAC.Text = "FLAC";
            this.doFLAC.Click += new System.EventHandler(this.doFLAC_Click);
            // 
            // doMP3
            // 
            this.doMP3.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.doMP3.Name = "doMP3";
            this.doMP3.Size = new System.Drawing.Size(104, 22);
            this.doMP3.Text = "MP3";
            this.doMP3.Click += new System.EventHandler(this.doMP3_Click);
            // 
            // doOGG
            // 
            this.doOGG.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.doOGG.Name = "doOGG";
            this.doOGG.Size = new System.Drawing.Size(104, 22);
            this.doOGG.Text = "OGG";
            this.doOGG.Click += new System.EventHandler(this.doOGG_Click);
            // 
            // doOPUS
            // 
            this.doOPUS.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.doOPUS.Name = "doOPUS";
            this.doOPUS.Size = new System.Drawing.Size(104, 22);
            this.doOPUS.Text = "OPUS";
            this.doOPUS.Click += new System.EventHandler(this.doOPUS_Click);
            // 
            // doWAV
            // 
            this.doWAV.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.doWAV.Checked = true;
            this.doWAV.CheckState = System.Windows.Forms.CheckState.Checked;
            this.doWAV.Name = "doWAV";
            this.doWAV.Size = new System.Drawing.Size(104, 22);
            this.doWAV.Text = "WAV";
            this.doWAV.Click += new System.EventHandler(this.doWAV_Click);
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
            this.btnReset.AllowDrop = true;
            this.btnReset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(144)))), ((int)(((byte)(51)))));
            this.btnReset.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReset.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReset.ForeColor = System.Drawing.Color.White;
            this.btnReset.Location = new System.Drawing.Point(12, 183);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(64, 29);
            this.btnReset.TabIndex = 40;
            this.btnReset.Text = "&Reset";
            this.btnReset.UseVisualStyleBackColor = false;
            this.btnReset.Visible = false;
            this.btnReset.Click += new System.EventHandler(this.button3_Click);
            this.btnReset.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.btnReset.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // grpStems
            // 
            this.grpStems.Controls.Add(this.chkBacking);
            this.grpStems.Controls.Add(this.chkCrowd);
            this.grpStems.Controls.Add(this.chkVocals);
            this.grpStems.Controls.Add(this.chkKeys);
            this.grpStems.Controls.Add(this.chkGuitar);
            this.grpStems.Controls.Add(this.chkBass);
            this.grpStems.Controls.Add(this.chkDrums);
            this.grpStems.Location = new System.Drawing.Point(14, 111);
            this.grpStems.Name = "grpStems";
            this.grpStems.Size = new System.Drawing.Size(454, 63);
            this.grpStems.TabIndex = 41;
            this.grpStems.TabStop = false;
            this.grpStems.Text = "Stems to Isolate";
            this.grpStems.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.grpStems.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // chkBacking
            // 
            this.chkBacking.AutoSize = true;
            this.chkBacking.Checked = true;
            this.chkBacking.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBacking.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkBacking.Enabled = false;
            this.chkBacking.Location = new System.Drawing.Point(316, 27);
            this.chkBacking.Name = "chkBacking";
            this.chkBacking.Size = new System.Drawing.Size(65, 17);
            this.chkBacking.TabIndex = 6;
            this.chkBacking.Text = "Backing";
            this.chkBacking.UseVisualStyleBackColor = true;
            // 
            // chkCrowd
            // 
            this.chkCrowd.AutoSize = true;
            this.chkCrowd.Checked = true;
            this.chkCrowd.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCrowd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkCrowd.Enabled = false;
            this.chkCrowd.Location = new System.Drawing.Point(387, 27);
            this.chkCrowd.Name = "chkCrowd";
            this.chkCrowd.Size = new System.Drawing.Size(56, 17);
            this.chkCrowd.TabIndex = 5;
            this.chkCrowd.Text = "Crowd";
            this.chkCrowd.UseVisualStyleBackColor = true;
            // 
            // chkVocals
            // 
            this.chkVocals.AutoSize = true;
            this.chkVocals.Checked = true;
            this.chkVocals.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkVocals.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkVocals.Enabled = false;
            this.chkVocals.Location = new System.Drawing.Point(197, 27);
            this.chkVocals.Name = "chkVocals";
            this.chkVocals.Size = new System.Drawing.Size(58, 17);
            this.chkVocals.TabIndex = 4;
            this.chkVocals.Text = "Vocals";
            this.chkVocals.UseVisualStyleBackColor = true;
            // 
            // chkKeys
            // 
            this.chkKeys.AutoSize = true;
            this.chkKeys.Checked = true;
            this.chkKeys.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkKeys.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkKeys.Enabled = false;
            this.chkKeys.Location = new System.Drawing.Point(261, 27);
            this.chkKeys.Name = "chkKeys";
            this.chkKeys.Size = new System.Drawing.Size(49, 17);
            this.chkKeys.TabIndex = 3;
            this.chkKeys.Text = "Keys";
            this.chkKeys.UseVisualStyleBackColor = true;
            // 
            // chkGuitar
            // 
            this.chkGuitar.AutoSize = true;
            this.chkGuitar.Checked = true;
            this.chkGuitar.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkGuitar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkGuitar.Enabled = false;
            this.chkGuitar.Location = new System.Drawing.Point(137, 27);
            this.chkGuitar.Name = "chkGuitar";
            this.chkGuitar.Size = new System.Drawing.Size(54, 17);
            this.chkGuitar.TabIndex = 2;
            this.chkGuitar.Text = "Guitar";
            this.chkGuitar.UseVisualStyleBackColor = true;
            // 
            // chkBass
            // 
            this.chkBass.AutoSize = true;
            this.chkBass.Checked = true;
            this.chkBass.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBass.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkBass.Enabled = false;
            this.chkBass.Location = new System.Drawing.Point(82, 27);
            this.chkBass.Name = "chkBass";
            this.chkBass.Size = new System.Drawing.Size(49, 17);
            this.chkBass.TabIndex = 1;
            this.chkBass.Text = "Bass";
            this.chkBass.UseVisualStyleBackColor = true;
            // 
            // chkDrums
            // 
            this.chkDrums.AutoSize = true;
            this.chkDrums.Checked = true;
            this.chkDrums.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDrums.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkDrums.Enabled = false;
            this.chkDrums.Location = new System.Drawing.Point(20, 27);
            this.chkDrums.Name = "chkDrums";
            this.chkDrums.Size = new System.Drawing.Size(56, 17);
            this.chkDrums.TabIndex = 0;
            this.chkDrums.Text = "Drums";
            this.chkDrums.UseVisualStyleBackColor = true;
            // 
            // txtFile
            // 
            this.txtFile.AllowDrop = true;
            this.txtFile.BackColor = System.Drawing.Color.White;
            this.txtFile.ForeColor = System.Drawing.Color.Gray;
            this.txtFile.Location = new System.Drawing.Point(14, 29);
            this.txtFile.Name = "txtFile";
            this.txtFile.ReadOnly = true;
            this.txtFile.Size = new System.Drawing.Size(419, 20);
            this.txtFile.TabIndex = 13;
            this.txtFile.Text = "Select source file (or drag/drop it here)";
            this.txtFile.TextChanged += new System.EventHandler(this.txtFile_TextChanged);
            this.txtFile.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.txtFile.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // btnFile
            // 
            this.btnFile.AllowDrop = true;
            this.btnFile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(144)))), ((int)(((byte)(51)))));
            this.btnFile.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFile.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFile.ForeColor = System.Drawing.Color.White;
            this.btnFile.Location = new System.Drawing.Point(439, 27);
            this.btnFile.Name = "btnFile";
            this.btnFile.Size = new System.Drawing.Size(29, 22);
            this.btnFile.TabIndex = 14;
            this.btnFile.Text = "...";
            this.btnFile.UseVisualStyleBackColor = false;
            this.btnFile.Click += new System.EventHandler(this.btnFile_Click);
            this.btnFile.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.btnFile.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // picWorking
            // 
            this.picWorking.Image = global::Nautilus.Properties.Resources.working;
            this.picWorking.Location = new System.Drawing.Point(179, 197);
            this.picWorking.Name = "picWorking";
            this.picWorking.Size = new System.Drawing.Size(128, 15);
            this.picWorking.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picWorking.TabIndex = 57;
            this.picWorking.TabStop = false;
            this.picWorking.Visible = false;
            // 
            // grpMode
            // 
            this.grpMode.Controls.Add(this.chkIgnore);
            this.grpMode.Controls.Add(this.radioDownmix);
            this.grpMode.Controls.Add(this.radioSplit);
            this.grpMode.Location = new System.Drawing.Point(14, 55);
            this.grpMode.Name = "grpMode";
            this.grpMode.Size = new System.Drawing.Size(454, 50);
            this.grpMode.TabIndex = 58;
            this.grpMode.TabStop = false;
            this.grpMode.Text = "Mode";
            this.grpMode.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.grpMode.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // chkIgnore
            // 
            this.chkIgnore.AutoSize = true;
            this.chkIgnore.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkIgnore.Location = new System.Drawing.Point(142, 19);
            this.chkIgnore.Name = "chkIgnore";
            this.chkIgnore.Size = new System.Drawing.Size(148, 17);
            this.chkIgnore.TabIndex = 4;
            this.chkIgnore.Text = "Ignore Attenuation Values";
            this.chkIgnore.UseVisualStyleBackColor = true;
            // 
            // radioDownmix
            // 
            this.radioDownmix.AllowDrop = true;
            this.radioDownmix.AutoSize = true;
            this.radioDownmix.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioDownmix.Location = new System.Drawing.Point(321, 19);
            this.radioDownmix.Name = "radioDownmix";
            this.radioDownmix.Size = new System.Drawing.Size(112, 17);
            this.radioDownmix.TabIndex = 2;
            this.radioDownmix.Text = "Downmix to stereo";
            this.radioDownmix.UseVisualStyleBackColor = true;
            this.radioDownmix.CheckedChanged += new System.EventHandler(this.radioSplit_CheckedChanged);
            this.radioDownmix.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.radioDownmix.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // radioSplit
            // 
            this.radioSplit.AllowDrop = true;
            this.radioSplit.AutoSize = true;
            this.radioSplit.Checked = true;
            this.radioSplit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioSplit.Location = new System.Drawing.Point(20, 19);
            this.radioSplit.Name = "radioSplit";
            this.radioSplit.Size = new System.Drawing.Size(116, 17);
            this.radioSplit.TabIndex = 0;
            this.radioSplit.TabStop = true;
            this.radioSplit.Text = "Split audio to stems";
            this.radioSplit.UseVisualStyleBackColor = true;
            this.radioSplit.CheckedChanged += new System.EventHandler(this.radioSplit_CheckedChanged);
            this.radioSplit.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.radioSplit.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // StemsIsolator
            // 
            this.AllowDrop = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(479, 347);
            this.Controls.Add(this.picPin);
            this.Controls.Add(this.grpMode);
            this.Controls.Add(this.picWorking);
            this.Controls.Add(this.grpStems);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.btnBegin);
            this.Controls.Add(this.btnFile);
            this.Controls.Add(this.txtFile);
            this.Controls.Add(this.lstLog);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "StemsIsolator";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Stems Isolator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.StemsIsolator_FormClosing);
            this.Shown += new System.EventHandler(this.stemsISO_Shown);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picPin)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.grpStems.ResumeLayout(false);
            this.grpStems.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picWorking)).EndInit();
            this.grpMode.ResumeLayout(false);
            this.grpMode.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstLog;
        private System.Windows.Forms.Button btnBegin;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.GroupBox grpStems;
        private System.Windows.Forms.CheckBox chkBacking;
        private System.Windows.Forms.CheckBox chkCrowd;
        private System.Windows.Forms.CheckBox chkVocals;
        private System.Windows.Forms.CheckBox chkKeys;
        private System.Windows.Forms.CheckBox chkGuitar;
        private System.Windows.Forms.CheckBox chkBass;
        private System.Windows.Forms.CheckBox chkDrums;
        private System.Windows.Forms.TextBox txtFile;
        private System.Windows.Forms.Button btnFile;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem exportLogFileToolStripMenuItem;
        private System.Windows.Forms.PictureBox picWorking;
        private System.Windows.Forms.GroupBox grpMode;
        private System.Windows.Forms.RadioButton radioSplit;
        private System.Windows.Forms.ToolStripMenuItem audioFormatToolStrip;
        private System.Windows.Forms.ToolStripMenuItem doWAV;
        private System.Windows.Forms.ToolStripMenuItem doOGG;
        private System.Windows.Forms.RadioButton radioDownmix;
        private System.Windows.Forms.PictureBox picPin;
        private System.Windows.Forms.ToolStripMenuItem doFLAC;
        private System.Windows.Forms.ToolStripMenuItem doMP3;
        private System.Windows.Forms.ToolStripMenuItem doOPUS;
        private System.Windows.Forms.CheckBox chkIgnore;
    }
}