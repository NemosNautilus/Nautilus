namespace Nautilus
{
    partial class CONExplorer
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
            this.contextMenuStrip3 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exportLogFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabInformation = new System.Windows.Forms.TabPage();
            this.cboFormat = new System.Windows.Forms.ComboBox();
            this.picPin = new System.Windows.Forms.PictureBox();
            this.lblIDTop = new System.Windows.Forms.Label();
            this.btnChange = new System.Windows.Forms.Button();
            this.lblSongID = new System.Windows.Forms.Label();
            this.picWorking = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cboGameID = new System.Windows.Forms.ComboBox();
            this.btnVisualize = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnExtract = new System.Windows.Forms.Button();
            this.chkAutoExtract = new System.Windows.Forms.CheckBox();
            this.txtDescription = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.txtTitle = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.picPackage = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.extractImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.picContent = new System.Windows.Forms.PictureBox();
            this.tabContents = new System.Windows.Forms.TabPage();
            this.folderTree = new DevComponents.AdvTree.AdvTree();
            this.contextMenuStrip4 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuRenameFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.node1 = new DevComponents.AdvTree.Node();
            this.nodeConnector1 = new DevComponents.AdvTree.NodeConnector();
            this.elementStyle1 = new DevComponents.DotNetBar.ElementStyle();
            this.elementStyle2 = new DevComponents.DotNetBar.ElementStyle();
            this.elementStyle3 = new DevComponents.DotNetBar.ElementStyle();
            this.elementStyle4 = new DevComponents.DotNetBar.ElementStyle();
            this.elementStyle5 = new DevComponents.DotNetBar.ElementStyle();
            this.elementStyle6 = new DevComponents.DotNetBar.ElementStyle();
            this.fileList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.extractFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replaceFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.injectSelectedFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRenameFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDeleteFile = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.addNewFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnDTA = new System.Windows.Forms.Button();
            this.btnMOGG = new System.Windows.Forms.Button();
            this.btnMIDI = new System.Windows.Forms.Button();
            this.btnPNG = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.btnMILO = new System.Windows.Forms.Button();
            this.contextMenuStrip3.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabInformation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picWorking)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPackage)).BeginInit();
            this.contextMenuStrip2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picContent)).BeginInit();
            this.tabContents.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.folderTree)).BeginInit();
            this.contextMenuStrip4.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstLog
            // 
            this.lstLog.ContextMenuStrip = this.contextMenuStrip3;
            this.lstLog.FormattingEnabled = true;
            this.lstLog.Location = new System.Drawing.Point(5, 375);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(407, 56);
            this.lstLog.TabIndex = 46;
            // 
            // contextMenuStrip3
            // 
            this.contextMenuStrip3.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.contextMenuStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportLogFileToolStripMenuItem});
            this.contextMenuStrip3.Name = "contextMenuStrip1";
            this.contextMenuStrip3.ShowImageMargin = false;
            this.contextMenuStrip3.Size = new System.Drawing.Size(122, 26);
            // 
            // exportLogFileToolStripMenuItem
            // 
            this.exportLogFileToolStripMenuItem.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.exportLogFileToolStripMenuItem.Name = "exportLogFileToolStripMenuItem";
            this.exportLogFileToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.exportLogFileToolStripMenuItem.Text = "Export log file";
            this.exportLogFileToolStripMenuItem.Click += new System.EventHandler(this.exportLogFileToolStripMenuItem_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.AllowDrop = true;
            this.tabControl1.Controls.Add(this.tabInformation);
            this.tabControl1.Controls.Add(this.tabContents);
            this.tabControl1.Location = new System.Drawing.Point(5, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(407, 332);
            this.tabControl1.TabIndex = 47;
            // 
            // tabInformation
            // 
            this.tabInformation.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.tabInformation.Controls.Add(this.cboFormat);
            this.tabInformation.Controls.Add(this.picPin);
            this.tabInformation.Controls.Add(this.lblIDTop);
            this.tabInformation.Controls.Add(this.btnChange);
            this.tabInformation.Controls.Add(this.lblSongID);
            this.tabInformation.Controls.Add(this.picWorking);
            this.tabInformation.Controls.Add(this.label4);
            this.tabInformation.Controls.Add(this.label3);
            this.tabInformation.Controls.Add(this.label2);
            this.tabInformation.Controls.Add(this.label1);
            this.tabInformation.Controls.Add(this.cboGameID);
            this.tabInformation.Controls.Add(this.btnVisualize);
            this.tabInformation.Controls.Add(this.btnSave);
            this.tabInformation.Controls.Add(this.btnExtract);
            this.tabInformation.Controls.Add(this.chkAutoExtract);
            this.tabInformation.Controls.Add(this.txtDescription);
            this.tabInformation.Controls.Add(this.txtTitle);
            this.tabInformation.Controls.Add(this.picPackage);
            this.tabInformation.Controls.Add(this.picContent);
            this.tabInformation.Location = new System.Drawing.Point(4, 22);
            this.tabInformation.Name = "tabInformation";
            this.tabInformation.Padding = new System.Windows.Forms.Padding(3);
            this.tabInformation.Size = new System.Drawing.Size(399, 306);
            this.tabInformation.TabIndex = 0;
            this.tabInformation.Text = "Information";
            // 
            // cboFormat
            // 
            this.cboFormat.Enabled = false;
            this.cboFormat.FormattingEnabled = true;
            this.cboFormat.Items.AddRange(new object[] {
            "SavedGame (CON)",
            "MarketPlace (LIVE)",
            "Installer (TU)",
            "Other"});
            this.cboFormat.Location = new System.Drawing.Point(71, 279);
            this.cboFormat.Name = "cboFormat";
            this.cboFormat.Size = new System.Drawing.Size(160, 21);
            this.cboFormat.TabIndex = 75;
            // 
            // picPin
            // 
            this.picPin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picPin.BackColor = System.Drawing.Color.Transparent;
            this.picPin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picPin.Image = global::Nautilus.Properties.Resources.unpinned;
            this.picPin.Location = new System.Drawing.Point(375, 4);
            this.picPin.Name = "picPin";
            this.picPin.Size = new System.Drawing.Size(20, 20);
            this.picPin.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picPin.TabIndex = 74;
            this.picPin.TabStop = false;
            this.picPin.Tag = "unpinned";
            this.toolTip1.SetToolTip(this.picPin, "Click to pin on top");
            this.picPin.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picPin_MouseClick);
            // 
            // lblIDTop
            // 
            this.lblIDTop.Location = new System.Drawing.Point(241, 4);
            this.lblIDTop.Name = "lblIDTop";
            this.lblIDTop.Size = new System.Drawing.Size(152, 13);
            this.lblIDTop.TabIndex = 66;
            this.lblIDTop.Text = "Song ID:";
            this.lblIDTop.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblIDTop.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lblIDTop_MouseClick);
            // 
            // btnChange
            // 
            this.btnChange.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(169)))), ((int)(((byte)(31)))));
            this.btnChange.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnChange.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChange.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnChange.ForeColor = System.Drawing.Color.White;
            this.btnChange.Location = new System.Drawing.Point(275, 39);
            this.btnChange.Name = "btnChange";
            this.btnChange.Size = new System.Drawing.Size(81, 24);
            this.btnChange.TabIndex = 68;
            this.btnChange.Text = "&Change ID";
            this.toolTip1.SetToolTip(this.btnChange, "Click to change the song ID");
            this.btnChange.UseVisualStyleBackColor = false;
            this.btnChange.Visible = false;
            this.btnChange.Click += new System.EventHandler(this.btnChange_Click);
            // 
            // lblSongID
            // 
            this.lblSongID.Location = new System.Drawing.Point(238, 19);
            this.lblSongID.Name = "lblSongID";
            this.lblSongID.Size = new System.Drawing.Size(155, 19);
            this.lblSongID.TabIndex = 67;
            this.lblSongID.Text = "N/A";
            this.lblSongID.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblSongID.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lblIDTop_MouseClick);
            // 
            // picWorking
            // 
            this.picWorking.Image = global::Nautilus.Properties.Resources.working;
            this.picWorking.Location = new System.Drawing.Point(262, 281);
            this.picWorking.Name = "picWorking";
            this.picWorking.Size = new System.Drawing.Size(128, 15);
            this.picWorking.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picWorking.TabIndex = 65;
            this.picWorking.TabStop = false;
            this.picWorking.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(12, 278);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 19);
            this.label4.TabIndex = 64;
            this.label4.Text = "Format:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(17, 148);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(137, 19);
            this.label3.TabIndex = 63;
            this.label3.Text = "Package Description:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(17, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 19);
            this.label2.TabIndex = 62;
            this.label2.Text = "Package Title:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(17, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 19);
            this.label1.TabIndex = 61;
            this.label1.Text = "Game:";
            // 
            // cboGameID
            // 
            this.cboGameID.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cboGameID.FormattingEnabled = true;
            this.cboGameID.Items.AddRange(new object[] {
            "Rock Band",
            "Rock Band 2",
            "The Beatles: Rock Band",
            "LEGO Rock Band",
            "Green Day: Rock Band",
            "Rock Band 3",
            "Guitar Hero II",
            "Guitar Hero III",
            "Guitar Hero: World Tour",
            "Guitar Hero 5",
            "Guitar Hero: Warriors of Rock",
            "Dance Central",
            "Dance Central 2",
            "Dance Central 3",
            "DJ Hero",
            "DJ Hero 2",
            "Rock Band Blitz"});
            this.cboGameID.Location = new System.Drawing.Point(71, 21);
            this.cboGameID.Name = "cboGameID";
            this.cboGameID.Size = new System.Drawing.Size(160, 21);
            this.cboGameID.TabIndex = 60;
            this.cboGameID.SelectedIndexChanged += new System.EventHandler(this.cboGameID_SelectedIndexChanged);
            // 
            // btnVisualize
            // 
            this.btnVisualize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(169)))), ((int)(((byte)(31)))));
            this.btnVisualize.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVisualize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVisualize.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVisualize.ForeColor = System.Drawing.Color.White;
            this.btnVisualize.Location = new System.Drawing.Point(312, 242);
            this.btnVisualize.Name = "btnVisualize";
            this.btnVisualize.Size = new System.Drawing.Size(75, 25);
            this.btnVisualize.TabIndex = 59;
            this.btnVisualize.Text = "&Visualize!";
            this.btnVisualize.UseVisualStyleBackColor = false;
            this.btnVisualize.Click += new System.EventHandler(this.btnVisualize_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(169)))), ((int)(((byte)(31)))));
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(245, 242);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(55, 25);
            this.btnSave.TabIndex = 58;
            this.btnSave.Text = "&Save";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.buttonX9_Click);
            // 
            // btnExtract
            // 
            this.btnExtract.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(169)))), ((int)(((byte)(31)))));
            this.btnExtract.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExtract.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExtract.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExtract.ForeColor = System.Drawing.Color.White;
            this.btnExtract.Location = new System.Drawing.Point(17, 242);
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Size = new System.Drawing.Size(116, 25);
            this.btnExtract.TabIndex = 57;
            this.btnExtract.Text = "&Extract Package";
            this.btnExtract.UseVisualStyleBackColor = false;
            this.btnExtract.Click += new System.EventHandler(this.buttonX8_Click_1);
            // 
            // chkAutoExtract
            // 
            this.chkAutoExtract.AutoSize = true;
            this.chkAutoExtract.BackColor = System.Drawing.Color.Transparent;
            this.chkAutoExtract.Checked = true;
            this.chkAutoExtract.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoExtract.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkAutoExtract.Location = new System.Drawing.Point(141, 250);
            this.chkAutoExtract.Name = "chkAutoExtract";
            this.chkAutoExtract.Size = new System.Drawing.Size(84, 17);
            this.chkAutoExtract.TabIndex = 48;
            this.chkAutoExtract.Text = "Auto-Extract";
            this.chkAutoExtract.UseVisualStyleBackColor = false;
            // 
            // txtDescription
            // 
            // 
            // 
            // 
            this.txtDescription.Border.Class = "TextBoxBorder";
            this.txtDescription.Location = new System.Drawing.Point(17, 170);
            this.txtDescription.MaxLength = 80;
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDescription.Size = new System.Drawing.Size(283, 64);
            this.txtDescription.TabIndex = 47;
            this.txtDescription.TextChanged += new System.EventHandler(this.textBoxX3_TextChanged);
            this.txtDescription.DoubleClick += new System.EventHandler(this.textBoxX3_DoubleClick);
            this.txtDescription.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxX3_KeyDown);
            // 
            // txtTitle
            // 
            // 
            // 
            // 
            this.txtTitle.Border.Class = "TextBoxBorder";
            this.txtTitle.Location = new System.Drawing.Point(17, 81);
            this.txtTitle.MaxLength = 80;
            this.txtTitle.Multiline = true;
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtTitle.Size = new System.Drawing.Size(283, 64);
            this.txtTitle.TabIndex = 45;
            this.txtTitle.TextChanged += new System.EventHandler(this.textBoxX2_TextChanged);
            this.txtTitle.DoubleClick += new System.EventHandler(this.textBoxX2_DoubleClick);
            this.txtTitle.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxX2_KeyDown);
            // 
            // picPackage
            // 
            this.picPackage.BackColor = System.Drawing.Color.Transparent;
            this.picPackage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picPackage.ContextMenuStrip = this.contextMenuStrip2;
            this.picPackage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picPackage.Location = new System.Drawing.Point(317, 81);
            this.picPackage.Name = "picPackage";
            this.picPackage.Size = new System.Drawing.Size(64, 64);
            this.picPackage.TabIndex = 46;
            this.picPackage.TabStop = false;
            this.picPackage.DragDrop += new System.Windows.Forms.DragEventHandler(this.PackageImage_DragDrop);
            this.picPackage.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            this.picPackage.MouseClick += new System.Windows.Forms.MouseEventHandler(this.PackageImage_MouseClick);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extractImageToolStripMenuItem});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.ShowImageMargin = false;
            this.contextMenuStrip2.Size = new System.Drawing.Size(121, 26);
            this.contextMenuStrip2.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip2_Opening);
            // 
            // extractImageToolStripMenuItem
            // 
            this.extractImageToolStripMenuItem.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.extractImageToolStripMenuItem.Name = "extractImageToolStripMenuItem";
            this.extractImageToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.extractImageToolStripMenuItem.Text = "Extract image";
            this.extractImageToolStripMenuItem.Click += new System.EventHandler(this.extractImageToolStripMenuItem_Click);
            // 
            // picContent
            // 
            this.picContent.BackColor = System.Drawing.Color.Transparent;
            this.picContent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picContent.ContextMenuStrip = this.contextMenuStrip2;
            this.picContent.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picContent.Location = new System.Drawing.Point(317, 170);
            this.picContent.Name = "picContent";
            this.picContent.Size = new System.Drawing.Size(64, 64);
            this.picContent.TabIndex = 44;
            this.picContent.TabStop = false;
            this.picContent.DragDrop += new System.Windows.Forms.DragEventHandler(this.ContentImage_DragDrop);
            this.picContent.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            this.picContent.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ContentImage_MouseClick);
            // 
            // tabContents
            // 
            this.tabContents.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.tabContents.Controls.Add(this.folderTree);
            this.tabContents.Controls.Add(this.fileList);
            this.tabContents.Location = new System.Drawing.Point(4, 22);
            this.tabContents.Name = "tabContents";
            this.tabContents.Padding = new System.Windows.Forms.Padding(3);
            this.tabContents.Size = new System.Drawing.Size(399, 306);
            this.tabContents.TabIndex = 1;
            this.tabContents.Text = "Contents";
            // 
            // folderTree
            // 
            this.folderTree.AccessibleRole = System.Windows.Forms.AccessibleRole.Outline;
            this.folderTree.AllowDrop = true;
            this.folderTree.AllowUserToResizeColumns = false;
            this.folderTree.BackColor = System.Drawing.Color.White;
            // 
            // 
            // 
            this.folderTree.BackgroundStyle.Class = "TreeBorderKey";
            this.folderTree.ContextMenuStrip = this.contextMenuStrip4;
            this.folderTree.DragDropEnabled = false;
            this.folderTree.Location = new System.Drawing.Point(6, 6);
            this.folderTree.Name = "folderTree";
            this.folderTree.Nodes.AddRange(new DevComponents.AdvTree.Node[] {
            this.node1});
            this.folderTree.NodesConnector = this.nodeConnector1;
            this.folderTree.NodeStyle = this.elementStyle1;
            this.folderTree.PathSeparator = ";";
            this.folderTree.Size = new System.Drawing.Size(386, 162);
            this.folderTree.Styles.Add(this.elementStyle1);
            this.folderTree.Styles.Add(this.elementStyle2);
            this.folderTree.Styles.Add(this.elementStyle3);
            this.folderTree.Styles.Add(this.elementStyle4);
            this.folderTree.Styles.Add(this.elementStyle5);
            this.folderTree.Styles.Add(this.elementStyle6);
            this.folderTree.TabIndex = 47;
            this.folderTree.Text = "advTree1";
            this.folderTree.Click += new System.EventHandler(this.advTree1_Click);
            // 
            // contextMenuStrip4
            // 
            this.contextMenuStrip4.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuRenameFolder});
            this.contextMenuStrip4.Name = "contextMenuStrip4";
            this.contextMenuStrip4.Size = new System.Drawing.Size(152, 26);
            this.contextMenuStrip4.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip4_Opening);
            // 
            // menuRenameFolder
            // 
            this.menuRenameFolder.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.menuRenameFolder.Name = "menuRenameFolder";
            this.menuRenameFolder.Size = new System.Drawing.Size(151, 22);
            this.menuRenameFolder.Text = "Rename folder";
            this.menuRenameFolder.Click += new System.EventHandler(this.menuRenameFolder_Click);
            // 
            // node1
            // 
            this.node1.Expanded = true;
            this.node1.Name = "node1";
            this.node1.Text = "Root";
            // 
            // nodeConnector1
            // 
            this.nodeConnector1.LineColor = System.Drawing.SystemColors.ControlText;
            // 
            // elementStyle1
            // 
            this.elementStyle1.Name = "elementStyle1";
            this.elementStyle1.TextColor = System.Drawing.SystemColors.ControlText;
            // 
            // elementStyle2
            // 
            this.elementStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(230)))), ((int)(((byte)(247)))));
            this.elementStyle2.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(168)))), ((int)(((byte)(228)))));
            this.elementStyle2.BackColorGradientAngle = 90;
            this.elementStyle2.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.elementStyle2.BorderBottomWidth = 1;
            this.elementStyle2.BorderColor = System.Drawing.Color.DarkGray;
            this.elementStyle2.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.elementStyle2.BorderLeftWidth = 1;
            this.elementStyle2.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.elementStyle2.BorderRightWidth = 1;
            this.elementStyle2.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.elementStyle2.BorderTopWidth = 1;
            this.elementStyle2.CornerDiameter = 4;
            this.elementStyle2.Description = "Blue";
            this.elementStyle2.Name = "elementStyle2";
            this.elementStyle2.PaddingBottom = 1;
            this.elementStyle2.PaddingLeft = 1;
            this.elementStyle2.PaddingRight = 1;
            this.elementStyle2.PaddingTop = 1;
            this.elementStyle2.TextColor = System.Drawing.Color.Black;
            // 
            // elementStyle3
            // 
            this.elementStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.elementStyle3.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(224)))), ((int)(((byte)(252)))));
            this.elementStyle3.BackColorGradientAngle = 90;
            this.elementStyle3.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.elementStyle3.BorderBottomWidth = 1;
            this.elementStyle3.BorderColor = System.Drawing.Color.DarkGray;
            this.elementStyle3.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.elementStyle3.BorderLeftWidth = 1;
            this.elementStyle3.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.elementStyle3.BorderRightWidth = 1;
            this.elementStyle3.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.elementStyle3.BorderTopWidth = 1;
            this.elementStyle3.CornerDiameter = 4;
            this.elementStyle3.Description = "BlueLight";
            this.elementStyle3.Name = "elementStyle3";
            this.elementStyle3.PaddingBottom = 1;
            this.elementStyle3.PaddingLeft = 1;
            this.elementStyle3.PaddingRight = 1;
            this.elementStyle3.PaddingTop = 1;
            this.elementStyle3.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(84)))), ((int)(((byte)(115)))));
            // 
            // elementStyle4
            // 
            this.elementStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.elementStyle4.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(224)))), ((int)(((byte)(252)))));
            this.elementStyle4.BackColorGradientAngle = 90;
            this.elementStyle4.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.elementStyle4.BorderBottomWidth = 1;
            this.elementStyle4.BorderColor = System.Drawing.Color.DarkGray;
            this.elementStyle4.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.elementStyle4.BorderLeftWidth = 1;
            this.elementStyle4.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.elementStyle4.BorderRightWidth = 1;
            this.elementStyle4.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.elementStyle4.BorderTopWidth = 1;
            this.elementStyle4.CornerDiameter = 4;
            this.elementStyle4.Description = "BlueLight";
            this.elementStyle4.Name = "elementStyle4";
            this.elementStyle4.PaddingBottom = 1;
            this.elementStyle4.PaddingLeft = 1;
            this.elementStyle4.PaddingRight = 1;
            this.elementStyle4.PaddingTop = 1;
            this.elementStyle4.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(84)))), ((int)(((byte)(115)))));
            // 
            // elementStyle5
            // 
            this.elementStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.elementStyle5.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(224)))), ((int)(((byte)(252)))));
            this.elementStyle5.BackColorGradientAngle = 90;
            this.elementStyle5.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.elementStyle5.BorderBottomWidth = 1;
            this.elementStyle5.BorderColor = System.Drawing.Color.DarkGray;
            this.elementStyle5.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.elementStyle5.BorderLeftWidth = 1;
            this.elementStyle5.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.elementStyle5.BorderRightWidth = 1;
            this.elementStyle5.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.elementStyle5.BorderTopWidth = 1;
            this.elementStyle5.CornerDiameter = 4;
            this.elementStyle5.Description = "BlueLight";
            this.elementStyle5.Name = "elementStyle5";
            this.elementStyle5.PaddingBottom = 1;
            this.elementStyle5.PaddingLeft = 1;
            this.elementStyle5.PaddingRight = 1;
            this.elementStyle5.PaddingTop = 1;
            this.elementStyle5.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(84)))), ((int)(((byte)(115)))));
            // 
            // elementStyle6
            // 
            this.elementStyle6.BackColor = System.Drawing.Color.White;
            this.elementStyle6.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(240)))));
            this.elementStyle6.BackColorGradientAngle = 90;
            this.elementStyle6.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.elementStyle6.BorderBottomWidth = 1;
            this.elementStyle6.BorderColor = System.Drawing.Color.DarkGray;
            this.elementStyle6.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.elementStyle6.BorderLeftWidth = 1;
            this.elementStyle6.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.elementStyle6.BorderRightWidth = 1;
            this.elementStyle6.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.elementStyle6.BorderTopWidth = 1;
            this.elementStyle6.CornerDiameter = 4;
            this.elementStyle6.Description = "Gray";
            this.elementStyle6.Name = "elementStyle6";
            this.elementStyle6.PaddingBottom = 1;
            this.elementStyle6.PaddingLeft = 1;
            this.elementStyle6.PaddingRight = 1;
            this.elementStyle6.PaddingTop = 1;
            this.elementStyle6.TextColor = System.Drawing.Color.Black;
            // 
            // fileList
            // 
            this.fileList.AllowDrop = true;
            this.fileList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.fileList.ContextMenuStrip = this.contextMenuStrip1;
            this.fileList.FullRowSelect = true;
            this.fileList.GridLines = true;
            this.fileList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.fileList.HideSelection = false;
            this.fileList.Location = new System.Drawing.Point(6, 174);
            this.fileList.Name = "fileList";
            this.fileList.Size = new System.Drawing.Size(386, 126);
            this.fileList.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.fileList.TabIndex = 46;
            this.fileList.UseCompatibleStateImageBehavior = false;
            this.fileList.View = System.Windows.Forms.View.Details;
            this.fileList.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.listView1_ItemDrag);
            this.fileList.DragDrop += new System.Windows.Forms.DragEventHandler(this.listView1_DragDrop);
            this.fileList.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 281;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Size";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader2.Width = 100;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extractFileToolStripMenuItem,
            this.replaceFileToolStripMenuItem,
            this.injectSelectedFileToolStripMenuItem,
            this.menuRenameFile,
            this.menuDeleteFile,
            this.toolStripMenuItem1,
            this.addNewFilesToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.ShowImageMargin = false;
            this.contextMenuStrip1.Size = new System.Drawing.Size(158, 142);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // extractFileToolStripMenuItem
            // 
            this.extractFileToolStripMenuItem.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.extractFileToolStripMenuItem.Name = "extractFileToolStripMenuItem";
            this.extractFileToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.extractFileToolStripMenuItem.Text = "Extract selected file";
            this.extractFileToolStripMenuItem.Click += new System.EventHandler(this.extractFileToolStripMenuItem_Click);
            // 
            // replaceFileToolStripMenuItem
            // 
            this.replaceFileToolStripMenuItem.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.replaceFileToolStripMenuItem.Name = "replaceFileToolStripMenuItem";
            this.replaceFileToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.replaceFileToolStripMenuItem.Text = "Replace selected file";
            this.replaceFileToolStripMenuItem.Click += new System.EventHandler(this.replaceFileToolStripMenuItem_Click);
            // 
            // injectSelectedFileToolStripMenuItem
            // 
            this.injectSelectedFileToolStripMenuItem.Name = "injectSelectedFileToolStripMenuItem";
            this.injectSelectedFileToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.injectSelectedFileToolStripMenuItem.Text = "Inject selected file";
            this.injectSelectedFileToolStripMenuItem.Click += new System.EventHandler(this.injectSelectedFileToolStripMenuItem_Click);
            // 
            // menuRenameFile
            // 
            this.menuRenameFile.Name = "menuRenameFile";
            this.menuRenameFile.Size = new System.Drawing.Size(157, 22);
            this.menuRenameFile.Text = "Rename selected file";
            this.menuRenameFile.Click += new System.EventHandler(this.menuRenameFile_Click);
            // 
            // menuDeleteFile
            // 
            this.menuDeleteFile.Name = "menuDeleteFile";
            this.menuDeleteFile.Size = new System.Drawing.Size(157, 22);
            this.menuDeleteFile.Text = "Delete selected file";
            this.menuDeleteFile.Click += new System.EventHandler(this.menuDeleteFile_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(154, 6);
            // 
            // addNewFilesToolStripMenuItem
            // 
            this.addNewFilesToolStripMenuItem.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.addNewFilesToolStripMenuItem.Name = "addNewFilesToolStripMenuItem";
            this.addNewFilesToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.addNewFilesToolStripMenuItem.Text = "Add new file(s)";
            this.addNewFilesToolStripMenuItem.Click += new System.EventHandler(this.addNewFilesToolStripMenuItem_Click);
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
            // btnDTA
            // 
            this.btnDTA.BackColor = System.Drawing.Color.LightGray;
            this.btnDTA.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDTA.Enabled = false;
            this.btnDTA.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDTA.Location = new System.Drawing.Point(86, 347);
            this.btnDTA.Name = "btnDTA";
            this.btnDTA.Size = new System.Drawing.Size(60, 23);
            this.btnDTA.TabIndex = 69;
            this.btnDTA.Text = "DTA";
            this.btnDTA.UseVisualStyleBackColor = false;
            this.btnDTA.Click += new System.EventHandler(this.btnDTA_Click);
            this.btnDTA.DragDrop += new System.Windows.Forms.DragEventHandler(this.btnDTA_DragDrop);
            this.btnDTA.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // btnMOGG
            // 
            this.btnMOGG.BackColor = System.Drawing.Color.LightGray;
            this.btnMOGG.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMOGG.Enabled = false;
            this.btnMOGG.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMOGG.Location = new System.Drawing.Point(284, 347);
            this.btnMOGG.Name = "btnMOGG";
            this.btnMOGG.Size = new System.Drawing.Size(60, 23);
            this.btnMOGG.TabIndex = 70;
            this.btnMOGG.Text = "MOGG";
            this.btnMOGG.UseVisualStyleBackColor = false;
            this.btnMOGG.Click += new System.EventHandler(this.btnMOGG_Click);
            this.btnMOGG.DragDrop += new System.Windows.Forms.DragEventHandler(this.btnMOGG_DragDrop);
            this.btnMOGG.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // btnMIDI
            // 
            this.btnMIDI.BackColor = System.Drawing.Color.LightGray;
            this.btnMIDI.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMIDI.Enabled = false;
            this.btnMIDI.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMIDI.Location = new System.Drawing.Point(152, 347);
            this.btnMIDI.Name = "btnMIDI";
            this.btnMIDI.Size = new System.Drawing.Size(60, 23);
            this.btnMIDI.TabIndex = 71;
            this.btnMIDI.Text = "MIDI";
            this.btnMIDI.UseVisualStyleBackColor = false;
            this.btnMIDI.Click += new System.EventHandler(this.btnMIDI_Click);
            this.btnMIDI.DragDrop += new System.Windows.Forms.DragEventHandler(this.btnMIDI_DragDrop);
            this.btnMIDI.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // btnPNG
            // 
            this.btnPNG.BackColor = System.Drawing.Color.LightGray;
            this.btnPNG.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPNG.Enabled = false;
            this.btnPNG.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPNG.Location = new System.Drawing.Point(218, 347);
            this.btnPNG.Name = "btnPNG";
            this.btnPNG.Size = new System.Drawing.Size(60, 23);
            this.btnPNG.TabIndex = 72;
            this.btnPNG.Text = "PNG";
            this.btnPNG.UseVisualStyleBackColor = false;
            this.btnPNG.Click += new System.EventHandler(this.btnPNG_Click);
            this.btnPNG.DragDrop += new System.Windows.Forms.DragEventHandler(this.btnPNG_DragDrop);
            this.btnPNG.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 346);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 26);
            this.label5.TabIndex = 73;
            this.label5.Text = "Click to copy\r\nThen paste it!";
            // 
            // btnMILO
            // 
            this.btnMILO.BackColor = System.Drawing.Color.LightGray;
            this.btnMILO.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMILO.Enabled = false;
            this.btnMILO.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMILO.Location = new System.Drawing.Point(350, 347);
            this.btnMILO.Name = "btnMILO";
            this.btnMILO.Size = new System.Drawing.Size(60, 23);
            this.btnMILO.TabIndex = 74;
            this.btnMILO.Text = "MILO";
            this.btnMILO.UseVisualStyleBackColor = false;
            this.btnMILO.Click += new System.EventHandler(this.btnMILO_Click);
            this.btnMILO.DragDrop += new System.Windows.Forms.DragEventHandler(this.btnMILO_DragDrop);
            this.btnMILO.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // CONExplorer
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(417, 438);
            this.Controls.Add(this.btnMILO);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnPNG);
            this.Controls.Add(this.btnMIDI);
            this.Controls.Add(this.btnMOGG);
            this.Controls.Add(this.btnDTA);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.lstLog);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "CONExplorer";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CON Explorer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SongExplorer_FormClosing);
            this.Shown += new System.EventHandler(this.CONExplorer_Shown);
            this.contextMenuStrip3.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabInformation.ResumeLayout(false);
            this.tabInformation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picWorking)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPackage)).EndInit();
            this.contextMenuStrip2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picContent)).EndInit();
            this.tabContents.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.folderTree)).EndInit();
            this.contextMenuStrip4.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstLog;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabInformation;
        private System.Windows.Forms.TabPage tabContents;
        private System.Windows.Forms.ComboBox cboGameID;
        private System.Windows.Forms.Button btnVisualize;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnExtract;
        private System.Windows.Forms.CheckBox chkAutoExtract;
        private DevComponents.DotNetBar.Controls.TextBoxX txtDescription;
        private DevComponents.DotNetBar.Controls.TextBoxX txtTitle;
        private System.Windows.Forms.PictureBox picPackage;
        private System.Windows.Forms.PictureBox picContent;
        private DevComponents.AdvTree.AdvTree folderTree;
        private DevComponents.AdvTree.Node node1;
        private DevComponents.AdvTree.NodeConnector nodeConnector1;
        private DevComponents.DotNetBar.ElementStyle elementStyle1;
        private DevComponents.DotNetBar.ElementStyle elementStyle2;
        private DevComponents.DotNetBar.ElementStyle elementStyle3;
        private DevComponents.DotNetBar.ElementStyle elementStyle4;
        private DevComponents.DotNetBar.ElementStyle elementStyle5;
        private DevComponents.DotNetBar.ElementStyle elementStyle6;
        public System.Windows.Forms.ListView fileList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem extractFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem replaceFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem addNewFilesToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem extractImageToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip3;
        private System.Windows.Forms.ToolStripMenuItem exportLogFileToolStripMenuItem;
        private System.Windows.Forms.PictureBox picWorking;
        private System.Windows.Forms.Label lblSongID;
        private System.Windows.Forms.Label lblIDTop;
        private System.Windows.Forms.Button btnDTA;
        private System.Windows.Forms.Button btnMOGG;
        private System.Windows.Forms.Button btnMIDI;
        private System.Windows.Forms.Button btnPNG;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnChange;
        private System.Windows.Forms.PictureBox picPin;
        private System.Windows.Forms.Button btnMILO;
        private System.Windows.Forms.ToolStripMenuItem injectSelectedFileToolStripMenuItem;
        private System.Windows.Forms.ComboBox cboFormat;
        private System.Windows.Forms.ToolStripMenuItem menuDeleteFile;
        private System.Windows.Forms.ToolStripMenuItem menuRenameFile;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip4;
        private System.Windows.Forms.ToolStripMenuItem menuRenameFolder;
    }
}