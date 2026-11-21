namespace Nautilus
{
    partial class UltraStarConverter
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
            this.btnReset = new System.Windows.Forms.Button();
            this.txtFolder = new System.Windows.Forms.TextBox();
            this.btnFolder = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnBegin = new System.Windows.Forms.Button();
            this.lstLog = new System.Windows.Forms.ListBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exportLogFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.picWorking = new System.Windows.Forms.PictureBox();
            this.picPin = new System.Windows.Forms.PictureBox();
            this.grpFormat = new System.Windows.Forms.GroupBox();
            this.radioOGG = new System.Windows.Forms.RadioButton();
            this.radioWAV = new System.Windows.Forms.RadioButton();
            this.radioMP3 = new System.Windows.Forms.RadioButton();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picWorking)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPin)).BeginInit();
            this.grpFormat.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnReset
            // 
            this.btnReset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(129)))), ((int)(((byte)(216)))));
            this.btnReset.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReset.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReset.ForeColor = System.Drawing.Color.White;
            this.btnReset.Location = new System.Drawing.Point(12, 124);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(100, 29);
            this.btnReset.TabIndex = 60;
            this.btnReset.Text = "&Reset";
            this.btnReset.UseVisualStyleBackColor = false;
            this.btnReset.Visible = false;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // txtFolder
            // 
            this.txtFolder.AllowDrop = true;
            this.txtFolder.BackColor = System.Drawing.Color.White;
            this.txtFolder.Location = new System.Drawing.Point(12, 45);
            this.txtFolder.Name = "txtFolder";
            this.txtFolder.ReadOnly = true;
            this.txtFolder.Size = new System.Drawing.Size(568, 20);
            this.txtFolder.TabIndex = 56;
            this.txtFolder.TextChanged += new System.EventHandler(this.txtFolder_TextChanged);
            this.txtFolder.DragDrop += new System.Windows.Forms.DragEventHandler(this.lstLog_DragDrop);
            this.txtFolder.DragEnter += new System.Windows.Forms.DragEventHandler(this.lstLog_DragEnter);
            // 
            // btnFolder
            // 
            this.btnFolder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(129)))), ((int)(((byte)(216)))));
            this.btnFolder.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFolder.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFolder.ForeColor = System.Drawing.Color.White;
            this.btnFolder.Location = new System.Drawing.Point(12, 12);
            this.btnFolder.Name = "btnFolder";
            this.btnFolder.Size = new System.Drawing.Size(134, 30);
            this.btnFolder.TabIndex = 57;
            this.btnFolder.Text = "Change &Input Folder";
            this.btnFolder.UseVisualStyleBackColor = false;
            this.btnFolder.Click += new System.EventHandler(this.btnFolder_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(129)))), ((int)(((byte)(216)))));
            this.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.Location = new System.Drawing.Point(480, 12);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(100, 30);
            this.btnRefresh.TabIndex = 58;
            this.btnRefresh.Text = "Refresh Folder";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Visible = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnBegin
            // 
            this.btnBegin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(129)))), ((int)(((byte)(216)))));
            this.btnBegin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBegin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBegin.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBegin.ForeColor = System.Drawing.Color.White;
            this.btnBegin.Location = new System.Drawing.Point(516, 124);
            this.btnBegin.Name = "btnBegin";
            this.btnBegin.Size = new System.Drawing.Size(64, 29);
            this.btnBegin.TabIndex = 59;
            this.btnBegin.Text = "&Begin";
            this.btnBegin.UseVisualStyleBackColor = false;
            this.btnBegin.Visible = false;
            this.btnBegin.Click += new System.EventHandler(this.btnBegin_Click);
            // 
            // lstLog
            // 
            this.lstLog.AllowDrop = true;
            this.lstLog.BackColor = System.Drawing.Color.White;
            this.lstLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstLog.ContextMenuStrip = this.contextMenuStrip1;
            this.lstLog.FormattingEnabled = true;
            this.lstLog.HorizontalScrollbar = true;
            this.lstLog.Location = new System.Drawing.Point(12, 159);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(568, 171);
            this.lstLog.TabIndex = 55;
            this.lstLog.DragDrop += new System.Windows.Forms.DragEventHandler(this.lstLog_DragDrop);
            this.lstLog.DragEnter += new System.Windows.Forms.DragEventHandler(this.lstLog_DragEnter);
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
            // picWorking
            // 
            this.picWorking.Image = global::Nautilus.Properties.Resources.working;
            this.picWorking.Location = new System.Drawing.Point(245, 24);
            this.picWorking.Name = "picWorking";
            this.picWorking.Size = new System.Drawing.Size(128, 15);
            this.picWorking.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picWorking.TabIndex = 63;
            this.picWorking.TabStop = false;
            this.picWorking.Visible = false;
            // 
            // picPin
            // 
            this.picPin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picPin.BackColor = System.Drawing.Color.Transparent;
            this.picPin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picPin.Image = global::Nautilus.Properties.Resources.unpinned;
            this.picPin.Location = new System.Drawing.Point(558, 71);
            this.picPin.Name = "picPin";
            this.picPin.Size = new System.Drawing.Size(20, 20);
            this.picPin.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picPin.TabIndex = 72;
            this.picPin.TabStop = false;
            this.picPin.Tag = "unpinned";
            this.picPin.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picPin_MouseClick);
            // 
            // grpFormat
            // 
            this.grpFormat.Controls.Add(this.radioOGG);
            this.grpFormat.Controls.Add(this.radioWAV);
            this.grpFormat.Controls.Add(this.radioMP3);
            this.grpFormat.Location = new System.Drawing.Point(12, 77);
            this.grpFormat.Name = "grpFormat";
            this.grpFormat.Size = new System.Drawing.Size(221, 41);
            this.grpFormat.TabIndex = 73;
            this.grpFormat.TabStop = false;
            this.grpFormat.Text = "Audio Format";
            // 
            // radioOGG
            // 
            this.radioOGG.AutoSize = true;
            this.radioOGG.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioOGG.Location = new System.Drawing.Point(161, 18);
            this.radioOGG.Name = "radioOGG";
            this.radioOGG.Size = new System.Drawing.Size(49, 17);
            this.radioOGG.TabIndex = 2;
            this.radioOGG.Text = "OGG";
            this.radioOGG.UseVisualStyleBackColor = true;
            // 
            // radioWAV
            // 
            this.radioWAV.AutoSize = true;
            this.radioWAV.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioWAV.Location = new System.Drawing.Point(87, 18);
            this.radioWAV.Name = "radioWAV";
            this.radioWAV.Size = new System.Drawing.Size(50, 17);
            this.radioWAV.TabIndex = 1;
            this.radioWAV.Text = "WAV";
            this.radioWAV.UseVisualStyleBackColor = true;
            // 
            // radioMP3
            // 
            this.radioMP3.AutoSize = true;
            this.radioMP3.Checked = true;
            this.radioMP3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioMP3.Location = new System.Drawing.Point(15, 18);
            this.radioMP3.Name = "radioMP3";
            this.radioMP3.Size = new System.Drawing.Size(47, 17);
            this.radioMP3.TabIndex = 0;
            this.radioMP3.TabStop = true;
            this.radioMP3.Text = "MP3";
            this.radioMP3.UseVisualStyleBackColor = true;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // UltraStarConverter
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(590, 335);
            this.Controls.Add(this.grpFormat);
            this.Controls.Add(this.picPin);
            this.Controls.Add(this.picWorking);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.txtFolder);
            this.Controls.Add(this.btnFolder);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnBegin);
            this.Controls.Add(this.lstLog);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "UltraStarConverter";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "UltraStar Converter";
            this.Shown += new System.EventHandler(this.UltraStarConverter_Shown);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.lstLog_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.lstLog_DragEnter);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picWorking)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPin)).EndInit();
            this.grpFormat.ResumeLayout(false);
            this.grpFormat.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.TextBox txtFolder;
        private System.Windows.Forms.Button btnFolder;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnBegin;
        private System.Windows.Forms.ListBox lstLog;
        private System.Windows.Forms.PictureBox picWorking;
        private System.Windows.Forms.PictureBox picPin;
        private System.Windows.Forms.GroupBox grpFormat;
        private System.Windows.Forms.RadioButton radioOGG;
        private System.Windows.Forms.RadioButton radioWAV;
        private System.Windows.Forms.RadioButton radioMP3;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem exportLogFileToolStripMenuItem;
    }
}