namespace Nautilus
{
    partial class ProUpgradeBundler
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProUpgradeBundler));
            this.btnBundle = new System.Windows.Forms.Button();
            this.lstLog = new System.Windows.Forms.ListBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exportLogFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnReset = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.batchBundle = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trackOverwritingOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.overwriteExistingTrack = new System.Windows.Forms.ToolStripMenuItem();
            this.onlyAddNewTracks = new System.Windows.Forms.ToolStripMenuItem();
            this.ignoreNoninstrumentTracks = new System.Windows.Forms.ToolStripMenuItem();
            this.ChangeGameID = new System.Windows.Forms.ToolStripMenuItem();
            this.showSongIDPrompt = new System.Windows.Forms.ToolStripMenuItem();
            this.useUpgradeID = new System.Windows.Forms.ToolStripMenuItem();
            this.cleanUpAfterBundlingFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.picWorking = new System.Windows.Forms.PictureBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.lstSong = new System.Windows.Forms.ListBox();
            this.lblOrig = new System.Windows.Forms.Label();
            this.lstUpgrades = new System.Windows.Forms.ListBox();
            this.lblUpgrades = new System.Windows.Forms.Label();
            this.picPin = new System.Windows.Forms.PictureBox();
            this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            this.contextMenuStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picWorking)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPin)).BeginInit();
            this.SuspendLayout();
            // 
            // btnBundle
            // 
            this.btnBundle.AllowDrop = true;
            this.btnBundle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(144)))), ((int)(((byte)(51)))));
            this.btnBundle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBundle.Enabled = false;
            this.btnBundle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBundle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBundle.ForeColor = System.Drawing.Color.White;
            this.btnBundle.Location = new System.Drawing.Point(470, 234);
            this.btnBundle.Name = "btnBundle";
            this.btnBundle.Size = new System.Drawing.Size(69, 26);
            this.btnBundle.TabIndex = 4;
            this.btnBundle.Text = "&Bundle!";
            this.btnBundle.UseVisualStyleBackColor = false;
            this.btnBundle.Click += new System.EventHandler(this.btnBundle_Click);
            // 
            // lstLog
            // 
            this.lstLog.AllowDrop = true;
            this.lstLog.BackColor = System.Drawing.Color.White;
            this.lstLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstLog.ContextMenuStrip = this.contextMenuStrip1;
            this.lstLog.FormattingEnabled = true;
            this.lstLog.HorizontalScrollbar = true;
            this.lstLog.Location = new System.Drawing.Point(12, 270);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(527, 119);
            this.lstLog.TabIndex = 13;
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
            // btnReset
            // 
            this.btnReset.AllowDrop = true;
            this.btnReset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(144)))), ((int)(((byte)(51)))));
            this.btnReset.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReset.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReset.ForeColor = System.Drawing.Color.White;
            this.btnReset.Location = new System.Drawing.Point(12, 234);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(69, 26);
            this.btnReset.TabIndex = 14;
            this.btnReset.Text = "&Reset";
            this.btnReset.UseVisualStyleBackColor = false;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.batchBundle,
            this.optionsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(551, 24);
            this.menuStrip1.TabIndex = 16;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // batchBundle
            // 
            this.batchBundle.Name = "batchBundle";
            this.batchBundle.Size = new System.Drawing.Size(89, 20);
            this.batchBundle.Text = "Batch Bundle";
            this.batchBundle.Click += new System.EventHandler(this.batchBundle_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.trackOverwritingOptions,
            this.ignoreNoninstrumentTracks,
            this.ChangeGameID,
            this.showSongIDPrompt,
            this.useUpgradeID,
            this.cleanUpAfterBundlingFiles});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // trackOverwritingOptions
            // 
            this.trackOverwritingOptions.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.trackOverwritingOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.overwriteExistingTrack,
            this.onlyAddNewTracks});
            this.trackOverwritingOptions.Name = "trackOverwritingOptions";
            this.trackOverwritingOptions.Size = new System.Drawing.Size(302, 22);
            this.trackOverwritingOptions.Text = "Track overwriting options";
            // 
            // overwriteExistingTrack
            // 
            this.overwriteExistingTrack.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.overwriteExistingTrack.Checked = true;
            this.overwriteExistingTrack.CheckState = System.Windows.Forms.CheckState.Checked;
            this.overwriteExistingTrack.Name = "overwriteExistingTrack";
            this.overwriteExistingTrack.Size = new System.Drawing.Size(322, 22);
            this.overwriteExistingTrack.Text = "Ovewrite existing track if also found in upgrade";
            this.overwriteExistingTrack.Click += new System.EventHandler(this.overwriteExistingTrack_Click);
            // 
            // onlyAddNewTracks
            // 
            this.onlyAddNewTracks.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.onlyAddNewTracks.Name = "onlyAddNewTracks";
            this.onlyAddNewTracks.Size = new System.Drawing.Size(322, 22);
            this.onlyAddNewTracks.Text = "Only add new tracks, don\'t overwrite existing";
            this.onlyAddNewTracks.Click += new System.EventHandler(this.onlyAddNewTracks_Click);
            // 
            // ignoreNoninstrumentTracks
            // 
            this.ignoreNoninstrumentTracks.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ignoreNoninstrumentTracks.Checked = true;
            this.ignoreNoninstrumentTracks.CheckOnClick = true;
            this.ignoreNoninstrumentTracks.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ignoreNoninstrumentTracks.Name = "ignoreNoninstrumentTracks";
            this.ignoreNoninstrumentTracks.Size = new System.Drawing.Size(302, 22);
            this.ignoreNoninstrumentTracks.Text = "Ignore non-instrument tracks";
            // 
            // ChangeGameID
            // 
            this.ChangeGameID.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ChangeGameID.Checked = true;
            this.ChangeGameID.CheckOnClick = true;
            this.ChangeGameID.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChangeGameID.Name = "ChangeGameID";
            this.ChangeGameID.Size = new System.Drawing.Size(302, 22);
            this.ChangeGameID.Text = "Change game ID on bundled file to RB3";
            // 
            // showSongIDPrompt
            // 
            this.showSongIDPrompt.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.showSongIDPrompt.Checked = true;
            this.showSongIDPrompt.CheckOnClick = true;
            this.showSongIDPrompt.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showSongIDPrompt.Name = "showSongIDPrompt";
            this.showSongIDPrompt.Size = new System.Drawing.Size(302, 22);
            this.showSongIDPrompt.Text = "Show new song ID prompt before bundling";
            // 
            // useUpgradeID
            // 
            this.useUpgradeID.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.useUpgradeID.Checked = true;
            this.useUpgradeID.CheckOnClick = true;
            this.useUpgradeID.CheckState = System.Windows.Forms.CheckState.Checked;
            this.useUpgradeID.Name = "useUpgradeID";
            this.useUpgradeID.Size = new System.Drawing.Size(302, 22);
            this.useUpgradeID.Text = "Default to upgrade ID over original song ID";
            // 
            // cleanUpAfterBundlingFiles
            // 
            this.cleanUpAfterBundlingFiles.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.cleanUpAfterBundlingFiles.Checked = true;
            this.cleanUpAfterBundlingFiles.CheckOnClick = true;
            this.cleanUpAfterBundlingFiles.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cleanUpAfterBundlingFiles.Name = "cleanUpAfterBundlingFiles";
            this.cleanUpAfterBundlingFiles.Size = new System.Drawing.Size(302, 22);
            this.cleanUpAfterBundlingFiles.Text = "Clean up after bundling files";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // picWorking
            // 
            this.picWorking.Image = global::Nautilus.Properties.Resources.working;
            this.picWorking.Location = new System.Drawing.Point(218, 242);
            this.picWorking.Name = "picWorking";
            this.picWorking.Size = new System.Drawing.Size(128, 15);
            this.picWorking.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picWorking.TabIndex = 58;
            this.picWorking.TabStop = false;
            this.picWorking.Visible = false;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // lstSong
            // 
            this.lstSong.AllowDrop = true;
            this.lstSong.FormattingEnabled = true;
            this.lstSong.Items.AddRange(new object[] {
            "No CON or MIDI loaded..."});
            this.lstSong.Location = new System.Drawing.Point(12, 53);
            this.lstSong.Name = "lstSong";
            this.lstSong.Size = new System.Drawing.Size(260, 173);
            this.lstSong.TabIndex = 59;
            this.lstSong.DragDrop += new System.Windows.Forms.DragEventHandler(this.lstSong_DragDrop);
            this.lstSong.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // lblOrig
            // 
            this.lblOrig.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOrig.ForeColor = System.Drawing.Color.RoyalBlue;
            this.lblOrig.Location = new System.Drawing.Point(12, 28);
            this.lblOrig.Name = "lblOrig";
            this.lblOrig.Size = new System.Drawing.Size(260, 18);
            this.lblOrig.TabIndex = 60;
            this.lblOrig.Text = "Drop original CON or MIDI here";
            this.lblOrig.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lstUpgrades
            // 
            this.lstUpgrades.AllowDrop = true;
            this.lstUpgrades.FormattingEnabled = true;
            this.lstUpgrades.Items.AddRange(new object[] {
            "No upgrades loaded..."});
            this.lstUpgrades.Location = new System.Drawing.Point(279, 53);
            this.lstUpgrades.Name = "lstUpgrades";
            this.lstUpgrades.Size = new System.Drawing.Size(260, 173);
            this.lstUpgrades.TabIndex = 61;
            this.lstUpgrades.DragDrop += new System.Windows.Forms.DragEventHandler(this.lstUpgrades_DragDrop);
            this.lstUpgrades.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // lblUpgrades
            // 
            this.lblUpgrades.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUpgrades.ForeColor = System.Drawing.Color.RoyalBlue;
            this.lblUpgrades.Location = new System.Drawing.Point(279, 28);
            this.lblUpgrades.Name = "lblUpgrades";
            this.lblUpgrades.Size = new System.Drawing.Size(260, 18);
            this.lblUpgrades.TabIndex = 62;
            this.lblUpgrades.Text = "Drop upgrades files here";
            this.lblUpgrades.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // picPin
            // 
            this.picPin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picPin.BackColor = System.Drawing.Color.Transparent;
            this.picPin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picPin.Image = ((System.Drawing.Image)(resources.GetObject("picPin.Image")));
            this.picPin.Location = new System.Drawing.Point(525, 6);
            this.picPin.Name = "picPin";
            this.picPin.Size = new System.Drawing.Size(20, 20);
            this.picPin.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picPin.TabIndex = 64;
            this.picPin.TabStop = false;
            this.picPin.Tag = "unpinned";
            this.picPin.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picPin_MouseClick);
            // 
            // backgroundWorker2
            // 
            this.backgroundWorker2.WorkerReportsProgress = true;
            this.backgroundWorker2.WorkerSupportsCancellation = true;
            this.backgroundWorker2.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker2_DoWork);
            this.backgroundWorker2.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker2_RunWorkerCompleted);
            // 
            // ProUpgradeBundler
            // 
            this.AllowDrop = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(551, 399);
            this.Controls.Add(this.picPin);
            this.Controls.Add(this.lblUpgrades);
            this.Controls.Add(this.lstUpgrades);
            this.Controls.Add(this.lblOrig);
            this.Controls.Add(this.lstSong);
            this.Controls.Add(this.picWorking);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.lstLog);
            this.Controls.Add(this.btnBundle);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "ProUpgradeBundler";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Upgrade Bundler";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProUpgradeBundler_FormClosing);
            this.Shown += new System.EventHandler(this.ProBundler_Shown);
            this.contextMenuStrip1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picWorking)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPin)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnBundle;
        private System.Windows.Forms.ListBox lstLog;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem exportLogFileToolStripMenuItem;
        private System.Windows.Forms.PictureBox picWorking;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ListBox lstSong;
        private System.Windows.Forms.Label lblOrig;
        private System.Windows.Forms.ListBox lstUpgrades;
        private System.Windows.Forms.Label lblUpgrades;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cleanUpAfterBundlingFiles;
        private System.Windows.Forms.ToolStripMenuItem trackOverwritingOptions;
        private System.Windows.Forms.ToolStripMenuItem overwriteExistingTrack;
        private System.Windows.Forms.ToolStripMenuItem onlyAddNewTracks;
        private System.Windows.Forms.ToolStripMenuItem ignoreNoninstrumentTracks;
        private System.Windows.Forms.ToolStripMenuItem ChangeGameID;
        private System.Windows.Forms.PictureBox picPin;
        private System.Windows.Forms.ToolStripMenuItem showSongIDPrompt;
        private System.Windows.Forms.ToolStripMenuItem useUpgradeID;
        private System.Windows.Forms.ToolStripMenuItem batchBundle;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
    }
}