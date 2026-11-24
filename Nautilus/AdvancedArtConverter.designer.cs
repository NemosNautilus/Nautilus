namespace Nautilus
{
    partial class AdvancedArtConverter
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
            this.txtFolder = new System.Windows.Forms.TextBox();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyFolderPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteFolderPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnFolder = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.lstLog = new System.Windows.Forms.ListBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exportLogFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.chkCleanUpWii = new System.Windows.Forms.CheckBox();
            this.btnToWii = new System.Windows.Forms.Button();
            this.btnFromWii = new System.Windows.Forms.Button();
            this.btnToXbox = new System.Windows.Forms.Button();
            this.btnFromXbox = new System.Windows.Forms.Button();
            this.chkCleanUpXbox = new System.Windows.Forms.CheckBox();
            this.btnToPS3 = new System.Windows.Forms.Button();
            this.btnFromPS3 = new System.Windows.Forms.Button();
            this.chkCleanUpPS3 = new System.Windows.Forms.CheckBox();
            this.picPin = new System.Windows.Forms.PictureBox();
            this.btnToPS4 = new System.Windows.Forms.Button();
            this.btnFromPS4 = new System.Windows.Forms.Button();
            this.chkCleanUpPS4 = new System.Windows.Forms.CheckBox();
            this.chkRemoveKeep = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabXbox = new System.Windows.Forms.TabPage();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.chkXboxPNG = new System.Windows.Forms.CheckBox();
            this.chkXboxJPG = new System.Windows.Forms.CheckBox();
            this.chkXboxBMP = new System.Windows.Forms.CheckBox();
            this.tabWii = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chkWiiPNG = new System.Windows.Forms.CheckBox();
            this.chkWiiJPG = new System.Windows.Forms.CheckBox();
            this.chkWiiBMP = new System.Windows.Forms.CheckBox();
            this.tabPS3 = new System.Windows.Forms.TabPage();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.chkPS3PNG = new System.Windows.Forms.CheckBox();
            this.chkPS3JPG = new System.Windows.Forms.CheckBox();
            this.chkPS3BMP = new System.Windows.Forms.CheckBox();
            this.tabPS4 = new System.Windows.Forms.TabPage();
            this.panel7 = new System.Windows.Forms.Panel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.chkPS4PNG = new System.Windows.Forms.CheckBox();
            this.chkPS4JPG = new System.Windows.Forms.CheckBox();
            this.chkPS4BMP = new System.Windows.Forms.CheckBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.advancedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textureSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.x256 = new System.Windows.Forms.ToolStripMenuItem();
            this.x512 = new System.Windows.Forms.ToolStripMenuItem();
            this.x1024 = new System.Windows.Forms.ToolStripMenuItem();
            this.x2048 = new System.Windows.Forms.ToolStripMenuItem();
            this.textureTypeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textureDXT1 = new System.Windows.Forms.ToolStripMenuItem();
            this.textureDXT5 = new System.Windows.Forms.ToolStripMenuItem();
            this.keepDDSFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.picWorking = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip2.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPin)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabXbox.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.tabWii.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabPS3.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel6.SuspendLayout();
            this.tabPS4.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel8.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picWorking)).BeginInit();
            this.SuspendLayout();
            // 
            // txtFolder
            // 
            this.txtFolder.AllowDrop = true;
            this.txtFolder.BackColor = System.Drawing.Color.White;
            this.txtFolder.ContextMenuStrip = this.contextMenuStrip2;
            this.txtFolder.Location = new System.Drawing.Point(4, 59);
            this.txtFolder.Name = "txtFolder";
            this.txtFolder.ReadOnly = true;
            this.txtFolder.Size = new System.Drawing.Size(261, 20);
            this.txtFolder.TabIndex = 5;
            this.txtFolder.TextChanged += new System.EventHandler(this.txtFolder_TextChanged);
            this.txtFolder.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.txtFolder.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyFolderPathToolStripMenuItem,
            this.pasteFolderPathToolStripMenuItem});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.ShowImageMargin = false;
            this.contextMenuStrip2.Size = new System.Drawing.Size(139, 48);
            this.contextMenuStrip2.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip2_Opening);
            // 
            // copyFolderPathToolStripMenuItem
            // 
            this.copyFolderPathToolStripMenuItem.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.copyFolderPathToolStripMenuItem.Name = "copyFolderPathToolStripMenuItem";
            this.copyFolderPathToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.copyFolderPathToolStripMenuItem.Text = "Copy folder path";
            this.copyFolderPathToolStripMenuItem.Click += new System.EventHandler(this.copyFolderPathToolStripMenuItem_Click);
            // 
            // pasteFolderPathToolStripMenuItem
            // 
            this.pasteFolderPathToolStripMenuItem.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.pasteFolderPathToolStripMenuItem.Name = "pasteFolderPathToolStripMenuItem";
            this.pasteFolderPathToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.pasteFolderPathToolStripMenuItem.Text = "Paste folder path";
            this.pasteFolderPathToolStripMenuItem.Click += new System.EventHandler(this.pasteFolderPathToolStripMenuItem_Click);
            // 
            // btnFolder
            // 
            this.btnFolder.AllowDrop = true;
            this.btnFolder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(129)))), ((int)(((byte)(216)))));
            this.btnFolder.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFolder.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFolder.ForeColor = System.Drawing.Color.White;
            this.btnFolder.Location = new System.Drawing.Point(4, 26);
            this.btnFolder.Name = "btnFolder";
            this.btnFolder.Size = new System.Drawing.Size(135, 30);
            this.btnFolder.TabIndex = 6;
            this.btnFolder.Text = "Change &Input Folder";
            this.toolTip1.SetToolTip(this.btnFolder, "Click to change directory");
            this.btnFolder.UseVisualStyleBackColor = false;
            this.btnFolder.Click += new System.EventHandler(this.btnFolder_Click);
            this.btnFolder.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.btnFolder.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // lstLog
            // 
            this.lstLog.AllowDrop = true;
            this.lstLog.BackColor = System.Drawing.Color.White;
            this.lstLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstLog.ContextMenuStrip = this.contextMenuStrip1;
            this.lstLog.FormattingEnabled = true;
            this.lstLog.HorizontalScrollbar = true;
            this.lstLog.Location = new System.Drawing.Point(8, 317);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(261, 145);
            this.lstLog.TabIndex = 1;
            this.toolTip1.SetToolTip(this.lstLog, "Application log");
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
            // btnRefresh
            // 
            this.btnRefresh.AllowDrop = true;
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(129)))), ((int)(((byte)(216)))));
            this.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.Location = new System.Drawing.Point(164, 26);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(101, 30);
            this.btnRefresh.TabIndex = 11;
            this.btnRefresh.Text = "&Refresh Folder";
            this.toolTip1.SetToolTip(this.btnRefresh, "Click to change directory");
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            this.btnRefresh.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.btnRefresh.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // chkCleanUpWii
            // 
            this.chkCleanUpWii.AutoSize = true;
            this.chkCleanUpWii.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkCleanUpWii.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkCleanUpWii.ForeColor = System.Drawing.Color.Black;
            this.chkCleanUpWii.Location = new System.Drawing.Point(6, 134);
            this.chkCleanUpWii.Name = "chkCleanUpWii";
            this.chkCleanUpWii.Size = new System.Drawing.Size(224, 23);
            this.chkCleanUpWii.TabIndex = 12;
            this.chkCleanUpWii.Text = "Delete originals after converting?";
            this.toolTip1.SetToolTip(this.chkCleanUpWii, "Select this option to delete the original png_xbox file(s)");
            this.chkCleanUpWii.UseVisualStyleBackColor = true;
            this.chkCleanUpWii.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.chkCleanUpWii.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // btnToWii
            // 
            this.btnToWii.AllowDrop = true;
            this.btnToWii.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(129)))), ((int)(((byte)(216)))));
            this.btnToWii.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnToWii.Enabled = false;
            this.btnToWii.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToWii.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnToWii.ForeColor = System.Drawing.Color.White;
            this.btnToWii.Location = new System.Drawing.Point(6, 5);
            this.btnToWii.Name = "btnToWii";
            this.btnToWii.Size = new System.Drawing.Size(224, 30);
            this.btnToWii.TabIndex = 13;
            this.btnToWii.Text = "Convert all input files to *.png_wii";
            this.toolTip1.SetToolTip(this.btnToWii, "Click to convert file(s) from Rock Band format");
            this.btnToWii.UseVisualStyleBackColor = false;
            this.btnToWii.Click += new System.EventHandler(this.btnToWii_Click);
            this.btnToWii.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.btnToWii.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // btnFromWii
            // 
            this.btnFromWii.AllowDrop = true;
            this.btnFromWii.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(129)))), ((int)(((byte)(216)))));
            this.btnFromWii.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFromWii.Enabled = false;
            this.btnFromWii.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFromWii.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFromWii.ForeColor = System.Drawing.Color.White;
            this.btnFromWii.Location = new System.Drawing.Point(15, 6);
            this.btnFromWii.Name = "btnFromWii";
            this.btnFromWii.Size = new System.Drawing.Size(208, 30);
            this.btnFromWii.TabIndex = 21;
            this.btnFromWii.Text = "Convert all Wii files";
            this.toolTip1.SetToolTip(this.btnFromWii, "Click to convert file(s) from Rock Band format");
            this.btnFromWii.UseVisualStyleBackColor = false;
            this.btnFromWii.EnabledChanged += new System.EventHandler(this.btnFromWii_EnabledChanged);
            this.btnFromWii.Click += new System.EventHandler(this.btnFromWii_Click);
            this.btnFromWii.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.btnFromWii.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // btnToXbox
            // 
            this.btnToXbox.AllowDrop = true;
            this.btnToXbox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(129)))), ((int)(((byte)(216)))));
            this.btnToXbox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnToXbox.Enabled = false;
            this.btnToXbox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToXbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnToXbox.ForeColor = System.Drawing.Color.White;
            this.btnToXbox.Location = new System.Drawing.Point(6, 5);
            this.btnToXbox.Name = "btnToXbox";
            this.btnToXbox.Size = new System.Drawing.Size(224, 30);
            this.btnToXbox.TabIndex = 13;
            this.btnToXbox.Text = "Convert all input files to *.png_xbox";
            this.toolTip1.SetToolTip(this.btnToXbox, "Click to convert file(s) from Rock Band format");
            this.btnToXbox.UseVisualStyleBackColor = false;
            this.btnToXbox.Click += new System.EventHandler(this.btnToXbox_Click);
            this.btnToXbox.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.btnToXbox.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // btnFromXbox
            // 
            this.btnFromXbox.AllowDrop = true;
            this.btnFromXbox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(129)))), ((int)(((byte)(216)))));
            this.btnFromXbox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFromXbox.Enabled = false;
            this.btnFromXbox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFromXbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFromXbox.ForeColor = System.Drawing.Color.White;
            this.btnFromXbox.Location = new System.Drawing.Point(15, 6);
            this.btnFromXbox.Name = "btnFromXbox";
            this.btnFromXbox.Size = new System.Drawing.Size(208, 30);
            this.btnFromXbox.TabIndex = 21;
            this.btnFromXbox.Text = "Convert all Xbox 360 files";
            this.toolTip1.SetToolTip(this.btnFromXbox, "Click to convert file(s) from Rock Band format");
            this.btnFromXbox.UseVisualStyleBackColor = false;
            this.btnFromXbox.EnabledChanged += new System.EventHandler(this.btnFromXbox_EnabledChanged);
            this.btnFromXbox.Click += new System.EventHandler(this.btnFromXbox_Click);
            this.btnFromXbox.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.btnFromXbox.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // chkCleanUpXbox
            // 
            this.chkCleanUpXbox.AllowDrop = true;
            this.chkCleanUpXbox.AutoSize = true;
            this.chkCleanUpXbox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkCleanUpXbox.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkCleanUpXbox.ForeColor = System.Drawing.Color.Black;
            this.chkCleanUpXbox.Location = new System.Drawing.Point(6, 165);
            this.chkCleanUpXbox.Name = "chkCleanUpXbox";
            this.chkCleanUpXbox.Size = new System.Drawing.Size(217, 23);
            this.chkCleanUpXbox.TabIndex = 19;
            this.chkCleanUpXbox.Text = "Delete originals after converting";
            this.toolTip1.SetToolTip(this.chkCleanUpXbox, "Select this option to delete the original png_xbox file(s)");
            this.chkCleanUpXbox.UseVisualStyleBackColor = true;
            this.chkCleanUpXbox.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.chkCleanUpXbox.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // btnToPS3
            // 
            this.btnToPS3.AllowDrop = true;
            this.btnToPS3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(129)))), ((int)(((byte)(216)))));
            this.btnToPS3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnToPS3.Enabled = false;
            this.btnToPS3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToPS3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnToPS3.ForeColor = System.Drawing.Color.White;
            this.btnToPS3.Location = new System.Drawing.Point(6, 5);
            this.btnToPS3.Name = "btnToPS3";
            this.btnToPS3.Size = new System.Drawing.Size(224, 30);
            this.btnToPS3.TabIndex = 13;
            this.btnToPS3.Text = "Convert all input files to *.png_ps3";
            this.toolTip1.SetToolTip(this.btnToPS3, "Click to convert file(s) from Rock Band format");
            this.btnToPS3.UseVisualStyleBackColor = false;
            this.btnToPS3.Click += new System.EventHandler(this.btnToPS3_Click);
            this.btnToPS3.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.btnToPS3.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // btnFromPS3
            // 
            this.btnFromPS3.AllowDrop = true;
            this.btnFromPS3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(129)))), ((int)(((byte)(216)))));
            this.btnFromPS3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFromPS3.Enabled = false;
            this.btnFromPS3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFromPS3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFromPS3.ForeColor = System.Drawing.Color.White;
            this.btnFromPS3.Location = new System.Drawing.Point(15, 6);
            this.btnFromPS3.Name = "btnFromPS3";
            this.btnFromPS3.Size = new System.Drawing.Size(208, 30);
            this.btnFromPS3.TabIndex = 21;
            this.btnFromPS3.Text = "Convert all PS3 files";
            this.toolTip1.SetToolTip(this.btnFromPS3, "Click to convert file(s) from Rock Band format");
            this.btnFromPS3.UseVisualStyleBackColor = false;
            this.btnFromPS3.EnabledChanged += new System.EventHandler(this.btnFromPS3_EnabledChanged);
            this.btnFromPS3.Click += new System.EventHandler(this.btnFromPS3_Click);
            this.btnFromPS3.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.btnFromPS3.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // chkCleanUpPS3
            // 
            this.chkCleanUpPS3.AllowDrop = true;
            this.chkCleanUpPS3.AutoSize = true;
            this.chkCleanUpPS3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkCleanUpPS3.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkCleanUpPS3.ForeColor = System.Drawing.Color.Black;
            this.chkCleanUpPS3.Location = new System.Drawing.Point(6, 134);
            this.chkCleanUpPS3.Name = "chkCleanUpPS3";
            this.chkCleanUpPS3.Size = new System.Drawing.Size(224, 23);
            this.chkCleanUpPS3.TabIndex = 22;
            this.chkCleanUpPS3.Text = "Delete originals after converting?";
            this.toolTip1.SetToolTip(this.chkCleanUpPS3, "Select this option to delete the original png_xbox file(s)");
            this.chkCleanUpPS3.UseVisualStyleBackColor = true;
            this.chkCleanUpPS3.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.chkCleanUpPS3.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // picPin
            // 
            this.picPin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picPin.BackColor = System.Drawing.Color.Transparent;
            this.picPin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picPin.Image = global::Nautilus.Properties.Resources.unpinned;
            this.picPin.Location = new System.Drawing.Point(245, 3);
            this.picPin.Name = "picPin";
            this.picPin.Size = new System.Drawing.Size(20, 20);
            this.picPin.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picPin.TabIndex = 68;
            this.picPin.TabStop = false;
            this.picPin.Tag = "unpinned";
            this.toolTip1.SetToolTip(this.picPin, "Click to pin on top");
            this.picPin.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picPin_MouseClick);
            // 
            // btnToPS4
            // 
            this.btnToPS4.AllowDrop = true;
            this.btnToPS4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(129)))), ((int)(((byte)(216)))));
            this.btnToPS4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnToPS4.Enabled = false;
            this.btnToPS4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToPS4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnToPS4.ForeColor = System.Drawing.Color.White;
            this.btnToPS4.Location = new System.Drawing.Point(6, 5);
            this.btnToPS4.Name = "btnToPS4";
            this.btnToPS4.Size = new System.Drawing.Size(224, 30);
            this.btnToPS4.TabIndex = 13;
            this.btnToPS4.Text = "Convert all input files to *.png_ps4";
            this.toolTip1.SetToolTip(this.btnToPS4, "Click to convert file(s) from Rock Band format");
            this.btnToPS4.UseVisualStyleBackColor = false;
            this.btnToPS4.Click += new System.EventHandler(this.btnToPS4_Click);
            // 
            // btnFromPS4
            // 
            this.btnFromPS4.AllowDrop = true;
            this.btnFromPS4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(129)))), ((int)(((byte)(216)))));
            this.btnFromPS4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFromPS4.Enabled = false;
            this.btnFromPS4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFromPS4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFromPS4.ForeColor = System.Drawing.Color.White;
            this.btnFromPS4.Location = new System.Drawing.Point(15, 6);
            this.btnFromPS4.Name = "btnFromPS4";
            this.btnFromPS4.Size = new System.Drawing.Size(208, 30);
            this.btnFromPS4.TabIndex = 21;
            this.btnFromPS4.Text = "Convert all PS4 files";
            this.toolTip1.SetToolTip(this.btnFromPS4, "Click to convert file(s) from Rock Band format");
            this.btnFromPS4.UseVisualStyleBackColor = false;
            this.btnFromPS4.EnabledChanged += new System.EventHandler(this.btnFromPS4_EnabledChanged);
            this.btnFromPS4.Click += new System.EventHandler(this.btnFromPS4_Click);
            // 
            // chkCleanUpPS4
            // 
            this.chkCleanUpPS4.AllowDrop = true;
            this.chkCleanUpPS4.AutoSize = true;
            this.chkCleanUpPS4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkCleanUpPS4.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkCleanUpPS4.ForeColor = System.Drawing.Color.Black;
            this.chkCleanUpPS4.Location = new System.Drawing.Point(6, 134);
            this.chkCleanUpPS4.Name = "chkCleanUpPS4";
            this.chkCleanUpPS4.Size = new System.Drawing.Size(224, 23);
            this.chkCleanUpPS4.TabIndex = 25;
            this.chkCleanUpPS4.Text = "Delete originals after converting?";
            this.toolTip1.SetToolTip(this.chkCleanUpPS4, "Select this option to delete the original png_xbox file(s)");
            this.chkCleanUpPS4.UseVisualStyleBackColor = true;
            // 
            // chkRemoveKeep
            // 
            this.chkRemoveKeep.AllowDrop = true;
            this.chkRemoveKeep.AutoSize = true;
            this.chkRemoveKeep.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkRemoveKeep.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkRemoveKeep.ForeColor = System.Drawing.Color.Black;
            this.chkRemoveKeep.Location = new System.Drawing.Point(6, 136);
            this.chkRemoveKeep.Name = "chkRemoveKeep";
            this.chkRemoveKeep.Size = new System.Drawing.Size(215, 23);
            this.chkRemoveKeep.TabIndex = 22;
            this.chkRemoveKeep.Text = "Remove _keep from file names";
            this.toolTip1.SetToolTip(this.chkRemoveKeep, "Select this option to remove _keep from the resulting file names");
            this.chkRemoveKeep.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.AllowDrop = true;
            this.tabControl1.Controls.Add(this.tabXbox);
            this.tabControl1.Controls.Add(this.tabWii);
            this.tabControl1.Controls.Add(this.tabPS3);
            this.tabControl1.Controls.Add(this.tabPS4);
            this.tabControl1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tabControl1.ItemSize = new System.Drawing.Size(200, 25);
            this.tabControl1.Location = new System.Drawing.Point(4, 86);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(261, 226);
            this.tabControl1.TabIndex = 10;
            this.tabControl1.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.tabControl1.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // tabXbox
            // 
            this.tabXbox.AllowDrop = true;
            this.tabXbox.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.tabXbox.Controls.Add(this.chkRemoveKeep);
            this.tabXbox.Controls.Add(this.panel3);
            this.tabXbox.Controls.Add(this.panel4);
            this.tabXbox.Controls.Add(this.chkCleanUpXbox);
            this.tabXbox.Cursor = System.Windows.Forms.Cursors.Default;
            this.tabXbox.Location = new System.Drawing.Point(4, 29);
            this.tabXbox.Name = "tabXbox";
            this.tabXbox.Padding = new System.Windows.Forms.Padding(3);
            this.tabXbox.Size = new System.Drawing.Size(253, 193);
            this.tabXbox.TabIndex = 0;
            this.tabXbox.Text = "Xbox 360";
            this.tabXbox.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.tabXbox.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // panel3
            // 
            this.panel3.AllowDrop = true;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.btnToXbox);
            this.panel3.Location = new System.Drawing.Point(6, 6);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(240, 44);
            this.panel3.TabIndex = 21;
            this.panel3.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.panel3.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // panel4
            // 
            this.panel4.AllowDrop = true;
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.btnFromXbox);
            this.panel4.Controls.Add(this.chkXboxPNG);
            this.panel4.Controls.Add(this.chkXboxJPG);
            this.panel4.Controls.Add(this.chkXboxBMP);
            this.panel4.Location = new System.Drawing.Point(6, 56);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(240, 74);
            this.panel4.TabIndex = 20;
            this.panel4.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.panel4.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // chkXboxPNG
            // 
            this.chkXboxPNG.AllowDrop = true;
            this.chkXboxPNG.AutoSize = true;
            this.chkXboxPNG.Checked = true;
            this.chkXboxPNG.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkXboxPNG.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkXboxPNG.Enabled = false;
            this.chkXboxPNG.Location = new System.Drawing.Point(33, 45);
            this.chkXboxPNG.Name = "chkXboxPNG";
            this.chkXboxPNG.Size = new System.Drawing.Size(51, 17);
            this.chkXboxPNG.TabIndex = 20;
            this.chkXboxPNG.Text = "*.png";
            this.chkXboxPNG.UseVisualStyleBackColor = true;
            this.chkXboxPNG.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.chkXboxPNG.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // chkXboxJPG
            // 
            this.chkXboxJPG.AllowDrop = true;
            this.chkXboxJPG.AutoSize = true;
            this.chkXboxJPG.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkXboxJPG.Enabled = false;
            this.chkXboxJPG.Location = new System.Drawing.Point(92, 45);
            this.chkXboxJPG.Name = "chkXboxJPG";
            this.chkXboxJPG.Size = new System.Drawing.Size(47, 17);
            this.chkXboxJPG.TabIndex = 19;
            this.chkXboxJPG.Text = "*.jpg";
            this.chkXboxJPG.UseVisualStyleBackColor = true;
            this.chkXboxJPG.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.chkXboxJPG.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // chkXboxBMP
            // 
            this.chkXboxBMP.AllowDrop = true;
            this.chkXboxBMP.AutoSize = true;
            this.chkXboxBMP.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkXboxBMP.Enabled = false;
            this.chkXboxBMP.Location = new System.Drawing.Point(149, 45);
            this.chkXboxBMP.Name = "chkXboxBMP";
            this.chkXboxBMP.Size = new System.Drawing.Size(53, 17);
            this.chkXboxBMP.TabIndex = 18;
            this.chkXboxBMP.Text = "*.bmp";
            this.chkXboxBMP.UseVisualStyleBackColor = true;
            this.chkXboxBMP.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.chkXboxBMP.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // tabWii
            // 
            this.tabWii.AllowDrop = true;
            this.tabWii.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.tabWii.Controls.Add(this.panel2);
            this.tabWii.Controls.Add(this.panel1);
            this.tabWii.Controls.Add(this.chkCleanUpWii);
            this.tabWii.Cursor = System.Windows.Forms.Cursors.Default;
            this.tabWii.Location = new System.Drawing.Point(4, 29);
            this.tabWii.Name = "tabWii";
            this.tabWii.Padding = new System.Windows.Forms.Padding(3);
            this.tabWii.Size = new System.Drawing.Size(253, 193);
            this.tabWii.TabIndex = 2;
            this.tabWii.Text = "Wii";
            this.tabWii.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.tabWii.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // panel2
            // 
            this.panel2.AllowDrop = true;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.btnToWii);
            this.panel2.Location = new System.Drawing.Point(6, 6);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(240, 44);
            this.panel2.TabIndex = 18;
            this.panel2.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.panel2.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // panel1
            // 
            this.panel1.AllowDrop = true;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnFromWii);
            this.panel1.Controls.Add(this.chkWiiPNG);
            this.panel1.Controls.Add(this.chkWiiJPG);
            this.panel1.Controls.Add(this.chkWiiBMP);
            this.panel1.Location = new System.Drawing.Point(6, 56);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(240, 74);
            this.panel1.TabIndex = 17;
            this.panel1.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.panel1.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // chkWiiPNG
            // 
            this.chkWiiPNG.AllowDrop = true;
            this.chkWiiPNG.AutoSize = true;
            this.chkWiiPNG.Checked = true;
            this.chkWiiPNG.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkWiiPNG.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkWiiPNG.Enabled = false;
            this.chkWiiPNG.Location = new System.Drawing.Point(33, 45);
            this.chkWiiPNG.Name = "chkWiiPNG";
            this.chkWiiPNG.Size = new System.Drawing.Size(51, 17);
            this.chkWiiPNG.TabIndex = 20;
            this.chkWiiPNG.Text = "*.png";
            this.chkWiiPNG.UseVisualStyleBackColor = true;
            this.chkWiiPNG.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.chkWiiPNG.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // chkWiiJPG
            // 
            this.chkWiiJPG.AllowDrop = true;
            this.chkWiiJPG.AutoSize = true;
            this.chkWiiJPG.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkWiiJPG.Enabled = false;
            this.chkWiiJPG.Location = new System.Drawing.Point(92, 45);
            this.chkWiiJPG.Name = "chkWiiJPG";
            this.chkWiiJPG.Size = new System.Drawing.Size(47, 17);
            this.chkWiiJPG.TabIndex = 19;
            this.chkWiiJPG.Text = "*.jpg";
            this.chkWiiJPG.UseVisualStyleBackColor = true;
            this.chkWiiJPG.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.chkWiiJPG.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // chkWiiBMP
            // 
            this.chkWiiBMP.AllowDrop = true;
            this.chkWiiBMP.AutoSize = true;
            this.chkWiiBMP.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkWiiBMP.Enabled = false;
            this.chkWiiBMP.Location = new System.Drawing.Point(149, 45);
            this.chkWiiBMP.Name = "chkWiiBMP";
            this.chkWiiBMP.Size = new System.Drawing.Size(53, 17);
            this.chkWiiBMP.TabIndex = 18;
            this.chkWiiBMP.Text = "*.bmp";
            this.chkWiiBMP.UseVisualStyleBackColor = true;
            this.chkWiiBMP.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.chkWiiBMP.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // tabPS3
            // 
            this.tabPS3.AllowDrop = true;
            this.tabPS3.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.tabPS3.Controls.Add(this.panel5);
            this.tabPS3.Controls.Add(this.panel6);
            this.tabPS3.Controls.Add(this.chkCleanUpPS3);
            this.tabPS3.Cursor = System.Windows.Forms.Cursors.Default;
            this.tabPS3.Location = new System.Drawing.Point(4, 29);
            this.tabPS3.Name = "tabPS3";
            this.tabPS3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPS3.Size = new System.Drawing.Size(253, 193);
            this.tabPS3.TabIndex = 3;
            this.tabPS3.Text = "PS3";
            this.tabPS3.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.tabPS3.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // panel5
            // 
            this.panel5.AllowDrop = true;
            this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel5.Controls.Add(this.btnToPS3);
            this.panel5.Location = new System.Drawing.Point(6, 6);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(240, 44);
            this.panel5.TabIndex = 24;
            this.panel5.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.panel5.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // panel6
            // 
            this.panel6.AllowDrop = true;
            this.panel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel6.Controls.Add(this.btnFromPS3);
            this.panel6.Controls.Add(this.chkPS3PNG);
            this.panel6.Controls.Add(this.chkPS3JPG);
            this.panel6.Controls.Add(this.chkPS3BMP);
            this.panel6.Location = new System.Drawing.Point(6, 56);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(240, 74);
            this.panel6.TabIndex = 23;
            this.panel6.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.panel6.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // chkPS3PNG
            // 
            this.chkPS3PNG.AllowDrop = true;
            this.chkPS3PNG.AutoSize = true;
            this.chkPS3PNG.Checked = true;
            this.chkPS3PNG.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPS3PNG.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkPS3PNG.Enabled = false;
            this.chkPS3PNG.Location = new System.Drawing.Point(33, 45);
            this.chkPS3PNG.Name = "chkPS3PNG";
            this.chkPS3PNG.Size = new System.Drawing.Size(51, 17);
            this.chkPS3PNG.TabIndex = 20;
            this.chkPS3PNG.Text = "*.png";
            this.chkPS3PNG.UseVisualStyleBackColor = true;
            this.chkPS3PNG.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.chkPS3PNG.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // chkPS3JPG
            // 
            this.chkPS3JPG.AllowDrop = true;
            this.chkPS3JPG.AutoSize = true;
            this.chkPS3JPG.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkPS3JPG.Enabled = false;
            this.chkPS3JPG.Location = new System.Drawing.Point(92, 45);
            this.chkPS3JPG.Name = "chkPS3JPG";
            this.chkPS3JPG.Size = new System.Drawing.Size(47, 17);
            this.chkPS3JPG.TabIndex = 19;
            this.chkPS3JPG.Text = "*.jpg";
            this.chkPS3JPG.UseVisualStyleBackColor = true;
            this.chkPS3JPG.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.chkPS3JPG.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // chkPS3BMP
            // 
            this.chkPS3BMP.AllowDrop = true;
            this.chkPS3BMP.AutoSize = true;
            this.chkPS3BMP.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkPS3BMP.Enabled = false;
            this.chkPS3BMP.Location = new System.Drawing.Point(149, 45);
            this.chkPS3BMP.Name = "chkPS3BMP";
            this.chkPS3BMP.Size = new System.Drawing.Size(53, 17);
            this.chkPS3BMP.TabIndex = 18;
            this.chkPS3BMP.Text = "*.bmp";
            this.chkPS3BMP.UseVisualStyleBackColor = true;
            this.chkPS3BMP.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.chkPS3BMP.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // tabPS4
            // 
            this.tabPS4.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.tabPS4.Controls.Add(this.panel7);
            this.tabPS4.Controls.Add(this.panel8);
            this.tabPS4.Controls.Add(this.chkCleanUpPS4);
            this.tabPS4.Cursor = System.Windows.Forms.Cursors.Default;
            this.tabPS4.Location = new System.Drawing.Point(4, 29);
            this.tabPS4.Name = "tabPS4";
            this.tabPS4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPS4.Size = new System.Drawing.Size(253, 193);
            this.tabPS4.TabIndex = 4;
            this.tabPS4.Text = "PS4";
            // 
            // panel7
            // 
            this.panel7.AllowDrop = true;
            this.panel7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel7.Controls.Add(this.btnToPS4);
            this.panel7.Location = new System.Drawing.Point(6, 6);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(240, 44);
            this.panel7.TabIndex = 27;
            // 
            // panel8
            // 
            this.panel8.AllowDrop = true;
            this.panel8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel8.Controls.Add(this.btnFromPS4);
            this.panel8.Controls.Add(this.chkPS4PNG);
            this.panel8.Controls.Add(this.chkPS4JPG);
            this.panel8.Controls.Add(this.chkPS4BMP);
            this.panel8.Location = new System.Drawing.Point(6, 56);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(240, 74);
            this.panel8.TabIndex = 26;
            // 
            // chkPS4PNG
            // 
            this.chkPS4PNG.AllowDrop = true;
            this.chkPS4PNG.AutoSize = true;
            this.chkPS4PNG.Checked = true;
            this.chkPS4PNG.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPS4PNG.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkPS4PNG.Enabled = false;
            this.chkPS4PNG.Location = new System.Drawing.Point(33, 45);
            this.chkPS4PNG.Name = "chkPS4PNG";
            this.chkPS4PNG.Size = new System.Drawing.Size(51, 17);
            this.chkPS4PNG.TabIndex = 20;
            this.chkPS4PNG.Text = "*.png";
            this.chkPS4PNG.UseVisualStyleBackColor = true;
            // 
            // chkPS4JPG
            // 
            this.chkPS4JPG.AllowDrop = true;
            this.chkPS4JPG.AutoSize = true;
            this.chkPS4JPG.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkPS4JPG.Enabled = false;
            this.chkPS4JPG.Location = new System.Drawing.Point(92, 45);
            this.chkPS4JPG.Name = "chkPS4JPG";
            this.chkPS4JPG.Size = new System.Drawing.Size(47, 17);
            this.chkPS4JPG.TabIndex = 19;
            this.chkPS4JPG.Text = "*.jpg";
            this.chkPS4JPG.UseVisualStyleBackColor = true;
            // 
            // chkPS4BMP
            // 
            this.chkPS4BMP.AllowDrop = true;
            this.chkPS4BMP.AutoSize = true;
            this.chkPS4BMP.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkPS4BMP.Enabled = false;
            this.chkPS4BMP.Location = new System.Drawing.Point(149, 45);
            this.chkPS4BMP.Name = "chkPS4BMP";
            this.chkPS4BMP.Size = new System.Drawing.Size(53, 17);
            this.chkPS4BMP.TabIndex = 18;
            this.chkPS4BMP.Text = "*.bmp";
            this.chkPS4BMP.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.advancedToolStripMenuItem,
            this.helpToolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(271, 24);
            this.menuStrip1.TabIndex = 12;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // advancedToolStripMenuItem
            // 
            this.advancedToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.textureSizeToolStripMenuItem,
            this.textureTypeToolStripMenuItem,
            this.keepDDSFiles});
            this.advancedToolStripMenuItem.Name = "advancedToolStripMenuItem";
            this.advancedToolStripMenuItem.Size = new System.Drawing.Size(72, 20);
            this.advancedToolStripMenuItem.Text = "Advanced";
            // 
            // textureSizeToolStripMenuItem
            // 
            this.textureSizeToolStripMenuItem.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.textureSizeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.x256,
            this.x512,
            this.x1024,
            this.x2048});
            this.textureSizeToolStripMenuItem.Name = "textureSizeToolStripMenuItem";
            this.textureSizeToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.textureSizeToolStripMenuItem.Text = "Texture size";
            // 
            // x256
            // 
            this.x256.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.x256.Name = "x256";
            this.x256.Size = new System.Drawing.Size(163, 22);
            this.x256.Text = "256x256";
            this.x256.Click += new System.EventHandler(this.x256ToolStripMenuItem_Click);
            // 
            // x512
            // 
            this.x512.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.x512.Checked = true;
            this.x512.CheckState = System.Windows.Forms.CheckState.Checked;
            this.x512.Name = "x512";
            this.x512.Size = new System.Drawing.Size(163, 22);
            this.x512.Text = "512x512 (default)";
            this.x512.Click += new System.EventHandler(this.x512defaultToolStripMenuItem_Click);
            // 
            // x1024
            // 
            this.x1024.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.x1024.Name = "x1024";
            this.x1024.Size = new System.Drawing.Size(163, 22);
            this.x1024.Text = "1024x1024";
            this.x1024.Click += new System.EventHandler(this.x1024ToolStripMenuItem_Click);
            // 
            // x2048
            // 
            this.x2048.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.x2048.Name = "x2048";
            this.x2048.Size = new System.Drawing.Size(163, 22);
            this.x2048.Text = "2048x2048";
            this.x2048.Click += new System.EventHandler(this.x2048muricaToolStrip_Click);
            // 
            // textureTypeToolStripMenuItem
            // 
            this.textureTypeToolStripMenuItem.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.textureTypeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.textureDXT1,
            this.textureDXT5});
            this.textureTypeToolStripMenuItem.Name = "textureTypeToolStripMenuItem";
            this.textureTypeToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.textureTypeToolStripMenuItem.Text = "Texture type";
            // 
            // textureDXT1
            // 
            this.textureDXT1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.textureDXT1.Name = "textureDXT1";
            this.textureDXT1.Size = new System.Drawing.Size(222, 22);
            this.textureDXT1.Text = "DXT1 (smaller file size)";
            this.textureDXT1.Click += new System.EventHandler(this.textureDXT1_Click);
            // 
            // textureDXT5
            // 
            this.textureDXT5.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.textureDXT5.Checked = true;
            this.textureDXT5.CheckState = System.Windows.Forms.CheckState.Checked;
            this.textureDXT5.Name = "textureDXT5";
            this.textureDXT5.Size = new System.Drawing.Size(222, 22);
            this.textureDXT5.Text = "DXT5 (higher image quality)";
            this.textureDXT5.Click += new System.EventHandler(this.textureDXT5_Click);
            // 
            // keepDDSFiles
            // 
            this.keepDDSFiles.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.keepDDSFiles.CheckOnClick = true;
            this.keepDDSFiles.Name = "keepDDSFiles";
            this.keepDDSFiles.Size = new System.Drawing.Size(215, 22);
            this.keepDDSFiles.Text = "Don\'t delete .dds / .tpl files";
            this.keepDDSFiles.Click += new System.EventHandler(this.keepDDSFilesToolStripMenuItem_Click);
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
            // picWorking
            // 
            this.picWorking.Image = global::Nautilus.Properties.Resources.working;
            this.picWorking.Location = new System.Drawing.Point(128, 5);
            this.picWorking.Name = "picWorking";
            this.picWorking.Size = new System.Drawing.Size(110, 15);
            this.picWorking.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picWorking.TabIndex = 67;
            this.picWorking.TabStop = false;
            this.picWorking.Visible = false;
            // 
            // AdvancedArtConverter
            // 
            this.AllowDrop = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(271, 464);
            this.Controls.Add(this.picPin);
            this.Controls.Add(this.picWorking);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnFolder);
            this.Controls.Add(this.txtFolder);
            this.Controls.Add(this.lstLog);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AdvancedArtConverter";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Advanced Art Converter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AlbumConvert_FormClosing);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            this.contextMenuStrip2.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picPin)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabXbox.ResumeLayout(false);
            this.tabXbox.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.tabWii.ResumeLayout(false);
            this.tabWii.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabPS3.ResumeLayout(false);
            this.tabPS3.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.tabPS4.ResumeLayout(false);
            this.tabPS4.PerformLayout();
            this.panel7.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picWorking)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtFolder;
        private System.Windows.Forms.Button btnFolder;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ListBox lstLog;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabXbox;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem exportLogFileToolStripMenuItem;
        private System.Windows.Forms.TabPage tabWii;
        private System.Windows.Forms.CheckBox chkCleanUpWii;
        private System.Windows.Forms.Button btnToWii;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnToXbox;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button btnFromXbox;
        private System.Windows.Forms.CheckBox chkXboxPNG;
        private System.Windows.Forms.CheckBox chkXboxJPG;
        private System.Windows.Forms.CheckBox chkXboxBMP;
        private System.Windows.Forms.CheckBox chkCleanUpXbox;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnFromWii;
        private System.Windows.Forms.CheckBox chkWiiPNG;
        private System.Windows.Forms.CheckBox chkWiiJPG;
        private System.Windows.Forms.CheckBox chkWiiBMP;
        private System.Windows.Forms.TabPage tabPS3;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Button btnToPS3;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Button btnFromPS3;
        private System.Windows.Forms.CheckBox chkPS3PNG;
        private System.Windows.Forms.CheckBox chkPS3JPG;
        private System.Windows.Forms.CheckBox chkPS3BMP;
        private System.Windows.Forms.CheckBox chkCleanUpPS3;
        private System.Windows.Forms.ToolStripMenuItem advancedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem textureSizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem x256;
        private System.Windows.Forms.ToolStripMenuItem x512;
        private System.Windows.Forms.ToolStripMenuItem x1024;
        private System.Windows.Forms.ToolStripMenuItem keepDDSFiles;
        private System.Windows.Forms.ToolStripMenuItem x2048;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem copyFolderPathToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteFolderPathToolStripMenuItem;
        private System.Windows.Forms.PictureBox picWorking;
        private System.Windows.Forms.PictureBox picPin;
        private System.Windows.Forms.ToolStripMenuItem textureTypeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem textureDXT1;
        private System.Windows.Forms.ToolStripMenuItem textureDXT5;
        private System.Windows.Forms.TabPage tabPS4;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Button btnToPS4;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Button btnFromPS4;
        private System.Windows.Forms.CheckBox chkPS4PNG;
        private System.Windows.Forms.CheckBox chkPS4JPG;
        private System.Windows.Forms.CheckBox chkPS4BMP;
        private System.Windows.Forms.CheckBox chkCleanUpPS4;
        private System.Windows.Forms.CheckBox chkRemoveKeep;
    }
}

