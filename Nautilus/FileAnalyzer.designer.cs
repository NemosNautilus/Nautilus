namespace Nautilus
{
    partial class SongAnalyzer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SongAnalyzer));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openMIDIOrCONFile = new System.Windows.Forms.ToolStripMenuItem();
            this.runAnalysisAgain = new System.Windows.Forms.ToolStripMenuItem();
            this.resetToolStrip = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStrip = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.calculateNPS = new System.Windows.Forms.ToolStripMenuItem();
            this.calculateDensity = new System.Windows.Forms.ToolStripMenuItem();
            this.breakDownInstruments = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.analyzeMoggFileInCONs = new System.Windows.Forms.ToolStripMenuItem();
            this.analyzePngxboxFile = new System.Windows.Forms.ToolStripMenuItem();
            this.exportLyricsToolStrip = new System.Windows.Forms.ToolStripMenuItem();
            this.exportPartVocals = new System.Windows.Forms.ToolStripMenuItem();
            this.exportHarmonies = new System.Windows.Forms.ToolStripMenuItem();
            this.exportingOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.leaveWordsSeparated = new System.Windows.Forms.ToolStripMenuItem();
            this.joinSyllables = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.searchForOccurrenceOfLyricToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lyricPhraseSearchOptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.onlyListSongs = new System.Windows.Forms.ToolStripMenuItem();
            this.showLyricsToolStrip = new System.Windows.Forms.ToolStripMenuItem();
            this.ignoreHarmonies = new System.Windows.Forms.ToolStripMenuItem();
            this.displayPhraseTiming = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.batchAnalyzeForUnpitched = new System.Windows.Forms.ToolStripMenuItem();
            this.separateUnpitchedSongs = new System.Windows.Forms.ToolStripMenuItem();
            this.moggAnalysis = new System.Windows.Forms.ToolStripMenuItem();
            this.batchAnalyzeMoggFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.openAudioAnalyzer = new System.Windows.Forms.ToolStripMenuItem();
            this.proDrumsAnalysis = new System.Windows.Forms.ToolStripMenuItem();
            this.batchAnalyzeForMissingProDrums = new System.Windows.Forms.ToolStripMenuItem();
            this.separateFilesThatAreMissingProDrums = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.batchAnalyzeForMissingTomMarkers = new System.Windows.Forms.ToolStripMenuItem();
            this.separateFilesThatAreMissingTomMarkers = new System.Windows.Forms.ToolStripMenuItem();
            this.euroMixToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkForNoDrumFills = new System.Windows.Forms.ToolStripMenuItem();
            this.checkForNoOverdrive = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exportToTextFile = new System.Windows.Forms.ToolStripMenuItem();
            this.resetToolStripMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.lstStats = new System.Windows.Forms.ListView();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.picloading = new System.Windows.Forms.PictureBox();
            this.chkMisc = new System.Windows.Forms.CheckBox();
            this.chkDrums = new System.Windows.Forms.CheckBox();
            this.chkBass = new System.Windows.Forms.CheckBox();
            this.chkGuitar = new System.Windows.Forms.CheckBox();
            this.chkKeys = new System.Windows.Forms.CheckBox();
            this.chkProKeys = new System.Windows.Forms.CheckBox();
            this.chkVocals = new System.Windows.Forms.CheckBox();
            this.chkHarms = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnCancel = new System.Windows.Forms.Button();
            this.picPin = new System.Windows.Forms.PictureBox();
            this.chkProBass = new System.Windows.Forms.CheckBox();
            this.chkProGuitar = new System.Windows.Forms.CheckBox();
            this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorker3 = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorker4 = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorker5 = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorker6 = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorker7 = new System.ComponentModel.BackgroundWorker();
            this.menuStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picloading)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPin)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.exportLyricsToolStrip,
            this.moggAnalysis,
            this.proDrumsAnalysis,
            this.euroMixToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(615, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openMIDIOrCONFile,
            this.runAnalysisAgain,
            this.resetToolStrip,
            this.exitToolStrip});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            this.fileToolStripMenuItem.Click += new System.EventHandler(this.fileToolStripMenuItem_Click);
            // 
            // openMIDIOrCONFile
            // 
            this.openMIDIOrCONFile.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.openMIDIOrCONFile.Name = "openMIDIOrCONFile";
            this.openMIDIOrCONFile.Size = new System.Drawing.Size(190, 22);
            this.openMIDIOrCONFile.Text = "&Open song file";
            this.openMIDIOrCONFile.Click += new System.EventHandler(this.openMIDIOrCONFileToolStrip_Click);
            // 
            // runAnalysisAgain
            // 
            this.runAnalysisAgain.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.runAnalysisAgain.Enabled = false;
            this.runAnalysisAgain.Name = "runAnalysisAgain";
            this.runAnalysisAgain.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.runAnalysisAgain.Size = new System.Drawing.Size(190, 22);
            this.runAnalysisAgain.Text = "Run analysis again";
            this.runAnalysisAgain.Click += new System.EventHandler(this.runAnalysisAgain_Click);
            // 
            // resetToolStrip
            // 
            this.resetToolStrip.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.resetToolStrip.Name = "resetToolStrip";
            this.resetToolStrip.Size = new System.Drawing.Size(190, 22);
            this.resetToolStrip.Text = "&Reset";
            this.resetToolStrip.Click += new System.EventHandler(this.resetToolStrip_Click);
            // 
            // exitToolStrip
            // 
            this.exitToolStrip.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.exitToolStrip.Name = "exitToolStrip";
            this.exitToolStrip.Size = new System.Drawing.Size(190, 22);
            this.exitToolStrip.Text = "E&xit";
            this.exitToolStrip.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.calculateNPS,
            this.calculateDensity,
            this.breakDownInstruments,
            this.toolStripMenuItem2,
            this.analyzeMoggFileInCONs,
            this.analyzePngxboxFile});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "&Options";
            // 
            // calculateNPS
            // 
            this.calculateNPS.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.calculateNPS.Checked = true;
            this.calculateNPS.CheckOnClick = true;
            this.calculateNPS.CheckState = System.Windows.Forms.CheckState.Checked;
            this.calculateNPS.Name = "calculateNPS";
            this.calculateNPS.Size = new System.Drawing.Size(250, 22);
            this.calculateNPS.Text = "Calculate NPS / NPM";
            // 
            // calculateDensity
            // 
            this.calculateDensity.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.calculateDensity.CheckOnClick = true;
            this.calculateDensity.Name = "calculateDensity";
            this.calculateDensity.Size = new System.Drawing.Size(250, 22);
            this.calculateDensity.Text = "Calculate Measure / Note Density";
            // 
            // breakDownInstruments
            // 
            this.breakDownInstruments.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.breakDownInstruments.CheckOnClick = true;
            this.breakDownInstruments.Name = "breakDownInstruments";
            this.breakDownInstruments.Size = new System.Drawing.Size(250, 22);
            this.breakDownInstruments.Text = "Break down instruments by note";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(247, 6);
            // 
            // analyzeMoggFileInCONs
            // 
            this.analyzeMoggFileInCONs.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.analyzeMoggFileInCONs.Checked = true;
            this.analyzeMoggFileInCONs.CheckOnClick = true;
            this.analyzeMoggFileInCONs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.analyzeMoggFileInCONs.Name = "analyzeMoggFileInCONs";
            this.analyzeMoggFileInCONs.Size = new System.Drawing.Size(250, 22);
            this.analyzeMoggFileInCONs.Text = "Analyze mogg file in CONs";
            // 
            // analyzePngxboxFile
            // 
            this.analyzePngxboxFile.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.analyzePngxboxFile.Checked = true;
            this.analyzePngxboxFile.CheckOnClick = true;
            this.analyzePngxboxFile.CheckState = System.Windows.Forms.CheckState.Checked;
            this.analyzePngxboxFile.Name = "analyzePngxboxFile";
            this.analyzePngxboxFile.Size = new System.Drawing.Size(250, 22);
            this.analyzePngxboxFile.Text = "Analyze png_xbox file in CONs";
            // 
            // exportLyricsToolStrip
            // 
            this.exportLyricsToolStrip.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportPartVocals,
            this.exportHarmonies,
            this.exportingOptions,
            this.toolStripMenuItem1,
            this.searchForOccurrenceOfLyricToolStripMenuItem,
            this.lyricPhraseSearchOptionsToolStripMenuItem,
            this.toolStripMenuItem3,
            this.batchAnalyzeForUnpitched,
            this.separateUnpitchedSongs});
            this.exportLyricsToolStrip.Name = "exportLyricsToolStrip";
            this.exportLyricsToolStrip.Size = new System.Drawing.Size(86, 20);
            this.exportLyricsToolStrip.Text = "Lyrics/Vocals";
            // 
            // exportPartVocals
            // 
            this.exportPartVocals.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.exportPartVocals.Enabled = false;
            this.exportPartVocals.Name = "exportPartVocals";
            this.exportPartVocals.Size = new System.Drawing.Size(272, 22);
            this.exportPartVocals.Text = "Export lead vocals only";
            this.exportPartVocals.Click += new System.EventHandler(this.exportPartVocals_Click);
            // 
            // exportHarmonies
            // 
            this.exportHarmonies.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.exportHarmonies.Enabled = false;
            this.exportHarmonies.Name = "exportHarmonies";
            this.exportHarmonies.Size = new System.Drawing.Size(272, 22);
            this.exportHarmonies.Text = "Export vocals && harmonies";
            this.exportHarmonies.Click += new System.EventHandler(this.exportHarmonies_Click);
            // 
            // exportingOptions
            // 
            this.exportingOptions.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.exportingOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.leaveWordsSeparated,
            this.joinSyllables});
            this.exportingOptions.Name = "exportingOptions";
            this.exportingOptions.Size = new System.Drawing.Size(272, 22);
            this.exportingOptions.Text = "Exporting options";
            // 
            // leaveWordsSeparated
            // 
            this.leaveWordsSeparated.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.leaveWordsSeparated.Checked = true;
            this.leaveWordsSeparated.CheckState = System.Windows.Forms.CheckState.Checked;
            this.leaveWordsSeparated.Name = "leaveWordsSeparated";
            this.leaveWordsSeparated.Size = new System.Drawing.Size(256, 22);
            this.leaveWordsSeparated.Text = "Leave words separated by syllables";
            this.leaveWordsSeparated.Click += new System.EventHandler(this.leaveWordsSeparated_Click);
            // 
            // joinSyllables
            // 
            this.joinSyllables.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.joinSyllables.Name = "joinSyllables";
            this.joinSyllables.Size = new System.Drawing.Size(256, 22);
            this.joinSyllables.Text = "Join syllables into whole words";
            this.joinSyllables.Click += new System.EventHandler(this.joinSyllables_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(269, 6);
            // 
            // searchForOccurrenceOfLyricToolStripMenuItem
            // 
            this.searchForOccurrenceOfLyricToolStripMenuItem.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.searchForOccurrenceOfLyricToolStripMenuItem.Name = "searchForOccurrenceOfLyricToolStripMenuItem";
            this.searchForOccurrenceOfLyricToolStripMenuItem.Size = new System.Drawing.Size(272, 22);
            this.searchForOccurrenceOfLyricToolStripMenuItem.Text = "Search for lyric phrase";
            this.searchForOccurrenceOfLyricToolStripMenuItem.Click += new System.EventHandler(this.searchForOccurrenceOfLyricToolStripMenuItem_Click);
            // 
            // lyricPhraseSearchOptionsToolStripMenuItem
            // 
            this.lyricPhraseSearchOptionsToolStripMenuItem.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.lyricPhraseSearchOptionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.onlyListSongs,
            this.showLyricsToolStrip,
            this.ignoreHarmonies,
            this.displayPhraseTiming});
            this.lyricPhraseSearchOptionsToolStripMenuItem.Name = "lyricPhraseSearchOptionsToolStripMenuItem";
            this.lyricPhraseSearchOptionsToolStripMenuItem.Size = new System.Drawing.Size(272, 22);
            this.lyricPhraseSearchOptionsToolStripMenuItem.Text = "Lyric phrase search options";
            // 
            // onlyListSongs
            // 
            this.onlyListSongs.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.onlyListSongs.CheckOnClick = true;
            this.onlyListSongs.Name = "onlyListSongs";
            this.onlyListSongs.Size = new System.Drawing.Size(188, 22);
            this.onlyListSongs.Text = "Only list songs";
            this.onlyListSongs.Click += new System.EventHandler(this.onlyListSongsToolStripMenuItem_Click);
            // 
            // showLyricsToolStrip
            // 
            this.showLyricsToolStrip.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.showLyricsToolStrip.Checked = true;
            this.showLyricsToolStrip.CheckOnClick = true;
            this.showLyricsToolStrip.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showLyricsToolStrip.Name = "showLyricsToolStrip";
            this.showLyricsToolStrip.Size = new System.Drawing.Size(188, 22);
            this.showLyricsToolStrip.Text = "Show lyrics";
            this.showLyricsToolStrip.Click += new System.EventHandler(this.showLyricsToolStripMenuItem_Click);
            // 
            // ignoreHarmonies
            // 
            this.ignoreHarmonies.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ignoreHarmonies.CheckOnClick = true;
            this.ignoreHarmonies.Name = "ignoreHarmonies";
            this.ignoreHarmonies.Size = new System.Drawing.Size(188, 22);
            this.ignoreHarmonies.Text = "Ignore harmonies";
            // 
            // displayPhraseTiming
            // 
            this.displayPhraseTiming.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.displayPhraseTiming.CheckOnClick = true;
            this.displayPhraseTiming.Name = "displayPhraseTiming";
            this.displayPhraseTiming.Size = new System.Drawing.Size(188, 22);
            this.displayPhraseTiming.Text = "Display phrase timing";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(269, 6);
            // 
            // batchAnalyzeForUnpitched
            // 
            this.batchAnalyzeForUnpitched.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.batchAnalyzeForUnpitched.Name = "batchAnalyzeForUnpitched";
            this.batchAnalyzeForUnpitched.Size = new System.Drawing.Size(272, 22);
            this.batchAnalyzeForUnpitched.Text = "Batch analyze for unpitched vocals";
            this.batchAnalyzeForUnpitched.Click += new System.EventHandler(this.batchAnalyzeForUnpitched_Click);
            // 
            // separateUnpitchedSongs
            // 
            this.separateUnpitchedSongs.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.separateUnpitchedSongs.Checked = true;
            this.separateUnpitchedSongs.CheckOnClick = true;
            this.separateUnpitchedSongs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.separateUnpitchedSongs.Name = "separateUnpitchedSongs";
            this.separateUnpitchedSongs.Size = new System.Drawing.Size(272, 22);
            this.separateUnpitchedSongs.Text = "Separate songs with unpitched vocals";
            // 
            // moggAnalysis
            // 
            this.moggAnalysis.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.batchAnalyzeMoggFiles,
            this.openAudioAnalyzer});
            this.moggAnalysis.Name = "moggAnalysis";
            this.moggAnalysis.Size = new System.Drawing.Size(56, 20);
            this.moggAnalysis.Text = "Moggs";
            // 
            // batchAnalyzeMoggFiles
            // 
            this.batchAnalyzeMoggFiles.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.batchAnalyzeMoggFiles.Name = "batchAnalyzeMoggFiles";
            this.batchAnalyzeMoggFiles.Size = new System.Drawing.Size(223, 22);
            this.batchAnalyzeMoggFiles.Text = "Batch analyze mogg files";
            this.batchAnalyzeMoggFiles.Click += new System.EventHandler(this.batchAnalyzeMoggFilesToolStrip_Click);
            // 
            // openAudioAnalyzer
            // 
            this.openAudioAnalyzer.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.openAudioAnalyzer.Name = "openAudioAnalyzer";
            this.openAudioAnalyzer.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.V)));
            this.openAudioAnalyzer.Size = new System.Drawing.Size(223, 22);
            this.openAudioAnalyzer.Text = "Visualize mogg file(s)";
            this.openAudioAnalyzer.Click += new System.EventHandler(this.openAudioAnalyzer_Click);
            // 
            // proDrumsAnalysis
            // 
            this.proDrumsAnalysis.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.batchAnalyzeForMissingProDrums,
            this.separateFilesThatAreMissingProDrums,
            this.toolStripMenuItem4,
            this.batchAnalyzeForMissingTomMarkers,
            this.separateFilesThatAreMissingTomMarkers});
            this.proDrumsAnalysis.Name = "proDrumsAnalysis";
            this.proDrumsAnalysis.Size = new System.Drawing.Size(75, 20);
            this.proDrumsAnalysis.Text = "Pro Drums";
            // 
            // batchAnalyzeForMissingProDrums
            // 
            this.batchAnalyzeForMissingProDrums.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.batchAnalyzeForMissingProDrums.Name = "batchAnalyzeForMissingProDrums";
            this.batchAnalyzeForMissingProDrums.Size = new System.Drawing.Size(300, 22);
            this.batchAnalyzeForMissingProDrums.Text = "Batch analyze for missing Pro Drums";
            this.batchAnalyzeForMissingProDrums.Click += new System.EventHandler(this.batchAnalyzeForMissingProDrums_Click);
            // 
            // separateFilesThatAreMissingProDrums
            // 
            this.separateFilesThatAreMissingProDrums.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.separateFilesThatAreMissingProDrums.Checked = true;
            this.separateFilesThatAreMissingProDrums.CheckOnClick = true;
            this.separateFilesThatAreMissingProDrums.CheckState = System.Windows.Forms.CheckState.Checked;
            this.separateFilesThatAreMissingProDrums.Name = "separateFilesThatAreMissingProDrums";
            this.separateFilesThatAreMissingProDrums.Size = new System.Drawing.Size(300, 22);
            this.separateFilesThatAreMissingProDrums.Text = "Separate files that are missing Pro Drums";
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(297, 6);
            // 
            // batchAnalyzeForMissingTomMarkers
            // 
            this.batchAnalyzeForMissingTomMarkers.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.batchAnalyzeForMissingTomMarkers.Name = "batchAnalyzeForMissingTomMarkers";
            this.batchAnalyzeForMissingTomMarkers.Size = new System.Drawing.Size(300, 22);
            this.batchAnalyzeForMissingTomMarkers.Text = "Batch analyze for missing tom markers";
            this.batchAnalyzeForMissingTomMarkers.Click += new System.EventHandler(this.batchAnalyzeForMissingTomMarkers_Click);
            // 
            // separateFilesThatAreMissingTomMarkers
            // 
            this.separateFilesThatAreMissingTomMarkers.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.separateFilesThatAreMissingTomMarkers.Checked = true;
            this.separateFilesThatAreMissingTomMarkers.CheckOnClick = true;
            this.separateFilesThatAreMissingTomMarkers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.separateFilesThatAreMissingTomMarkers.Name = "separateFilesThatAreMissingTomMarkers";
            this.separateFilesThatAreMissingTomMarkers.Size = new System.Drawing.Size(300, 22);
            this.separateFilesThatAreMissingTomMarkers.Text = "Separate files that are missing tom markers";
            // 
            // euroMixToolStripMenuItem
            // 
            this.euroMixToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkForNoDrumFills,
            this.checkForNoOverdrive});
            this.euroMixToolStripMenuItem.Name = "euroMixToolStripMenuItem";
            this.euroMixToolStripMenuItem.Size = new System.Drawing.Size(104, 20);
            this.euroMixToolStripMenuItem.Text = "EuroMIX Special";
            // 
            // checkForNoDrumFills
            // 
            this.checkForNoDrumFills.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.checkForNoDrumFills.Name = "checkForNoDrumFills";
            this.checkForNoDrumFills.Size = new System.Drawing.Size(195, 22);
            this.checkForNoDrumFills.Text = "Check for no drum fills";
            this.checkForNoDrumFills.Click += new System.EventHandler(this.checkForNoDrumFills_Click);
            // 
            // checkForNoOverdrive
            // 
            this.checkForNoOverdrive.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.checkForNoOverdrive.Name = "checkForNoOverdrive";
            this.checkForNoOverdrive.Size = new System.Drawing.Size(195, 22);
            this.checkForNoOverdrive.Text = "Check for no overdrive";
            this.checkForNoOverdrive.Click += new System.EventHandler(this.checkForNoOverdrive_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportToTextFile,
            this.resetToolStripMenu});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.ShowImageMargin = false;
            this.contextMenuStrip1.Size = new System.Drawing.Size(138, 48);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // exportToTextFile
            // 
            this.exportToTextFile.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.exportToTextFile.Name = "exportToTextFile";
            this.exportToTextFile.Size = new System.Drawing.Size(137, 22);
            this.exportToTextFile.Text = "Export to text file";
            this.exportToTextFile.Click += new System.EventHandler(this.exportToTextFileToolStripMenuItem_Click);
            // 
            // resetToolStripMenu
            // 
            this.resetToolStripMenu.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.resetToolStripMenu.Name = "resetToolStripMenu";
            this.resetToolStripMenu.Size = new System.Drawing.Size(137, 22);
            this.resetToolStripMenu.Text = "Reset";
            this.resetToolStripMenu.Click += new System.EventHandler(this.resetToolStripMenuItem1_Click);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // lstStats
            // 
            this.lstStats.AllowDrop = true;
            this.lstStats.BackgroundImageTiled = true;
            this.lstStats.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colValue});
            this.lstStats.ContextMenuStrip = this.contextMenuStrip1;
            this.lstStats.FullRowSelect = true;
            this.lstStats.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lstStats.HideSelection = false;
            this.lstStats.Location = new System.Drawing.Point(12, 27);
            this.lstStats.Name = "lstStats";
            this.lstStats.Size = new System.Drawing.Size(591, 491);
            this.lstStats.TabIndex = 31;
            this.toolTip1.SetToolTip(this.lstStats, "Right click to export when analysis is completed");
            this.lstStats.UseCompatibleStateImageBehavior = false;
            this.lstStats.View = System.Windows.Forms.View.Details;
            this.lstStats.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.lstStats.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // colName
            // 
            this.colName.Text = "Name";
            this.colName.Width = 374;
            // 
            // colValue
            // 
            this.colValue.Text = "Value";
            this.colValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.colValue.Width = 190;
            // 
            // picloading
            // 
            this.picloading.BackColor = System.Drawing.Color.White;
            this.picloading.ContextMenuStrip = this.contextMenuStrip1;
            this.picloading.Image = ((System.Drawing.Image)(resources.GetObject("picloading.Image")));
            this.picloading.Location = new System.Drawing.Point(252, 211);
            this.picloading.Name = "picloading";
            this.picloading.Size = new System.Drawing.Size(100, 100);
            this.picloading.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picloading.TabIndex = 30;
            this.picloading.TabStop = false;
            this.picloading.UseWaitCursor = true;
            this.picloading.Visible = false;
            this.picloading.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.picloading.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            // 
            // chkMisc
            // 
            this.chkMisc.AutoSize = true;
            this.chkMisc.Checked = true;
            this.chkMisc.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMisc.Location = new System.Drawing.Point(12, 524);
            this.chkMisc.Name = "chkMisc";
            this.chkMisc.Size = new System.Drawing.Size(48, 17);
            this.chkMisc.TabIndex = 32;
            this.chkMisc.Text = "Misc";
            this.toolTip1.SetToolTip(this.chkMisc, "Enable to show this information, disable to hide");
            this.chkMisc.UseVisualStyleBackColor = true;
            // 
            // chkDrums
            // 
            this.chkDrums.AutoSize = true;
            this.chkDrums.Checked = true;
            this.chkDrums.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDrums.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkDrums.Location = new System.Drawing.Point(62, 524);
            this.chkDrums.Name = "chkDrums";
            this.chkDrums.Size = new System.Drawing.Size(56, 17);
            this.chkDrums.TabIndex = 33;
            this.chkDrums.Text = "Drums";
            this.toolTip1.SetToolTip(this.chkDrums, "Enable to show this information, disable to hide");
            this.chkDrums.UseVisualStyleBackColor = true;
            // 
            // chkBass
            // 
            this.chkBass.AutoSize = true;
            this.chkBass.Checked = true;
            this.chkBass.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBass.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkBass.Location = new System.Drawing.Point(120, 524);
            this.chkBass.Name = "chkBass";
            this.chkBass.Size = new System.Drawing.Size(49, 17);
            this.chkBass.TabIndex = 34;
            this.chkBass.Text = "Bass";
            this.toolTip1.SetToolTip(this.chkBass, "Enable to show this information, disable to hide");
            this.chkBass.UseVisualStyleBackColor = true;
            // 
            // chkGuitar
            // 
            this.chkGuitar.AutoSize = true;
            this.chkGuitar.Checked = true;
            this.chkGuitar.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkGuitar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkGuitar.Location = new System.Drawing.Point(241, 524);
            this.chkGuitar.Name = "chkGuitar";
            this.chkGuitar.Size = new System.Drawing.Size(54, 17);
            this.chkGuitar.TabIndex = 35;
            this.chkGuitar.Text = "Guitar";
            this.toolTip1.SetToolTip(this.chkGuitar, "Enable to show this information, disable to hide");
            this.chkGuitar.UseVisualStyleBackColor = true;
            // 
            // chkKeys
            // 
            this.chkKeys.AutoSize = true;
            this.chkKeys.Checked = true;
            this.chkKeys.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkKeys.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkKeys.Location = new System.Drawing.Point(490, 524);
            this.chkKeys.Name = "chkKeys";
            this.chkKeys.Size = new System.Drawing.Size(49, 17);
            this.chkKeys.TabIndex = 36;
            this.chkKeys.Text = "Keys";
            this.toolTip1.SetToolTip(this.chkKeys, "Enable to show this information, disable to hide");
            this.chkKeys.UseVisualStyleBackColor = true;
            // 
            // chkProKeys
            // 
            this.chkProKeys.AutoSize = true;
            this.chkProKeys.Checked = true;
            this.chkProKeys.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkProKeys.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkProKeys.Location = new System.Drawing.Point(541, 524);
            this.chkProKeys.Name = "chkProKeys";
            this.chkProKeys.Size = new System.Drawing.Size(68, 17);
            this.chkProKeys.TabIndex = 37;
            this.chkProKeys.Text = "Pro-Keys";
            this.toolTip1.SetToolTip(this.chkProKeys, "Enable to show this information, disable to hide");
            this.chkProKeys.UseVisualStyleBackColor = true;
            // 
            // chkVocals
            // 
            this.chkVocals.AutoSize = true;
            this.chkVocals.Checked = true;
            this.chkVocals.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkVocals.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkVocals.Location = new System.Drawing.Point(372, 524);
            this.chkVocals.Name = "chkVocals";
            this.chkVocals.Size = new System.Drawing.Size(58, 17);
            this.chkVocals.TabIndex = 38;
            this.chkVocals.Text = "Vocals";
            this.toolTip1.SetToolTip(this.chkVocals, "Enable to show this information, disable to hide");
            this.chkVocals.UseVisualStyleBackColor = true;
            this.chkVocals.CheckedChanged += new System.EventHandler(this.chkVocals_CheckedChanged);
            // 
            // chkHarms
            // 
            this.chkHarms.AutoSize = true;
            this.chkHarms.Checked = true;
            this.chkHarms.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkHarms.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkHarms.Location = new System.Drawing.Point(432, 524);
            this.chkHarms.Name = "chkHarms";
            this.chkHarms.Size = new System.Drawing.Size(56, 17);
            this.chkHarms.TabIndex = 39;
            this.chkHarms.Text = "Harms";
            this.toolTip1.SetToolTip(this.chkHarms, "Enable to show this information, disable to hide");
            this.chkHarms.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.Location = new System.Drawing.Point(555, 27);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(48, 23);
            this.btnCancel.TabIndex = 40;
            this.btnCancel.Text = "Cancel";
            this.toolTip1.SetToolTip(this.btnCancel, "Click to cancel process");
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // picPin
            // 
            this.picPin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picPin.BackColor = System.Drawing.Color.Transparent;
            this.picPin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picPin.Image = global::Nautilus.Properties.Resources.unpinned;
            this.picPin.Location = new System.Drawing.Point(590, 4);
            this.picPin.Name = "picPin";
            this.picPin.Size = new System.Drawing.Size(20, 20);
            this.picPin.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picPin.TabIndex = 64;
            this.picPin.TabStop = false;
            this.picPin.Tag = "unpinned";
            this.toolTip1.SetToolTip(this.picPin, "Click to pin on top");
            this.picPin.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picPin_MouseClick);
            // 
            // chkProBass
            // 
            this.chkProBass.AutoSize = true;
            this.chkProBass.Checked = true;
            this.chkProBass.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkProBass.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkProBass.Location = new System.Drawing.Point(171, 524);
            this.chkProBass.Name = "chkProBass";
            this.chkProBass.Size = new System.Drawing.Size(68, 17);
            this.chkProBass.TabIndex = 65;
            this.chkProBass.Text = "Pro-Bass";
            this.toolTip1.SetToolTip(this.chkProBass, "Enable to show this information, disable to hide");
            this.chkProBass.UseVisualStyleBackColor = true;
            // 
            // chkProGuitar
            // 
            this.chkProGuitar.AutoSize = true;
            this.chkProGuitar.Checked = true;
            this.chkProGuitar.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkProGuitar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkProGuitar.Location = new System.Drawing.Point(297, 524);
            this.chkProGuitar.Name = "chkProGuitar";
            this.chkProGuitar.Size = new System.Drawing.Size(73, 17);
            this.chkProGuitar.TabIndex = 66;
            this.chkProGuitar.Text = "Pro-Guitar";
            this.toolTip1.SetToolTip(this.chkProGuitar, "Enable to show this information, disable to hide");
            this.chkProGuitar.UseVisualStyleBackColor = true;
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
            // 
            // backgroundWorker4
            // 
            this.backgroundWorker4.WorkerReportsProgress = true;
            this.backgroundWorker4.WorkerSupportsCancellation = true;
            this.backgroundWorker4.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker4_DoWork);
            this.backgroundWorker4.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker4_RunWorkerCompleted);
            // 
            // backgroundWorker5
            // 
            this.backgroundWorker5.WorkerReportsProgress = true;
            this.backgroundWorker5.WorkerSupportsCancellation = true;
            this.backgroundWorker5.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker5_DoWork);
            this.backgroundWorker5.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker5_RunWorkerCompleted);
            // 
            // backgroundWorker6
            // 
            this.backgroundWorker6.WorkerReportsProgress = true;
            this.backgroundWorker6.WorkerSupportsCancellation = true;
            this.backgroundWorker6.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker6_DoWork);
            this.backgroundWorker6.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker6_RunWorkerCompleted);
            // 
            // backgroundWorker7
            // 
            this.backgroundWorker7.WorkerReportsProgress = true;
            this.backgroundWorker7.WorkerSupportsCancellation = true;
            this.backgroundWorker7.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker7_DoWork);
            this.backgroundWorker7.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker7_RunWorkerCompleted);
            // 
            // SongAnalyzer
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(615, 550);
            this.Controls.Add(this.chkProGuitar);
            this.Controls.Add(this.chkProBass);
            this.Controls.Add(this.picPin);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.chkHarms);
            this.Controls.Add(this.chkVocals);
            this.Controls.Add(this.chkProKeys);
            this.Controls.Add(this.chkKeys);
            this.Controls.Add(this.chkGuitar);
            this.Controls.Add(this.chkBass);
            this.Controls.Add(this.chkDrums);
            this.Controls.Add(this.chkMisc);
            this.Controls.Add(this.picloading);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.lstStats);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "SongAnalyzer";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "File Analyzer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MIDIAnalyzer_FormClosing);
            this.Shown += new System.EventHandler(this.MIDIAnalyzer_Shown);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.HandleDragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.HandleDragEnter);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picloading)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPin)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.PictureBox picloading;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openMIDIOrCONFile;
        private System.Windows.Forms.ToolStripMenuItem resetToolStrip;
        private System.Windows.Forms.ToolStripMenuItem exitToolStrip;
        private System.Windows.Forms.ListView lstStats;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colValue;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem exportToTextFile;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenu;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem breakDownInstruments;
        private System.Windows.Forms.ToolStripMenuItem calculateNPS;
        private System.Windows.Forms.ToolStripMenuItem calculateDensity;
        private System.Windows.Forms.ToolStripMenuItem runAnalysisAgain;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox chkMisc;
        private System.Windows.Forms.CheckBox chkDrums;
        private System.Windows.Forms.CheckBox chkBass;
        private System.Windows.Forms.CheckBox chkGuitar;
        private System.Windows.Forms.CheckBox chkKeys;
        private System.Windows.Forms.CheckBox chkProKeys;
        private System.Windows.Forms.CheckBox chkVocals;
        private System.Windows.Forms.CheckBox chkHarms;
        private System.Windows.Forms.ToolStripMenuItem exportLyricsToolStrip;
        private System.Windows.Forms.ToolStripMenuItem exportPartVocals;
        private System.Windows.Forms.ToolStripMenuItem exportHarmonies;
        private System.Windows.Forms.ToolStripMenuItem exportingOptions;
        private System.Windows.Forms.ToolStripMenuItem leaveWordsSeparated;
        private System.Windows.Forms.ToolStripMenuItem joinSyllables;
        private System.Windows.Forms.ToolStripMenuItem moggAnalysis;
        private System.Windows.Forms.ToolStripMenuItem batchAnalyzeMoggFiles;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
        private System.ComponentModel.BackgroundWorker backgroundWorker3;
        private System.Windows.Forms.ToolStripMenuItem proDrumsAnalysis;
        private System.Windows.Forms.ToolStripMenuItem batchAnalyzeForMissingProDrums;
        private System.Windows.Forms.ToolStripMenuItem separateFilesThatAreMissingProDrums;
        private System.ComponentModel.BackgroundWorker backgroundWorker4;
        private System.Windows.Forms.ToolStripMenuItem analyzeMoggFileInCONs;
        private System.Windows.Forms.ToolStripMenuItem searchForOccurrenceOfLyricToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker backgroundWorker5;
        private System.Windows.Forms.ToolStripMenuItem lyricPhraseSearchOptionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem onlyListSongs;
        private System.Windows.Forms.ToolStripMenuItem showLyricsToolStrip;
        private System.Windows.Forms.ToolStripMenuItem ignoreHarmonies;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ToolStripMenuItem analyzePngxboxFile;
        private System.Windows.Forms.ToolStripMenuItem displayPhraseTiming;
        private System.Windows.Forms.ToolStripMenuItem openAudioAnalyzer;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.PictureBox picPin;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem batchAnalyzeForUnpitched;
        private System.Windows.Forms.ToolStripMenuItem separateUnpitchedSongs;
        private System.Windows.Forms.CheckBox chkProBass;
        private System.Windows.Forms.CheckBox chkProGuitar;
        private System.Windows.Forms.ToolStripMenuItem euroMixToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkForNoDrumFills;
        private System.Windows.Forms.ToolStripMenuItem checkForNoOverdrive;
        private System.ComponentModel.BackgroundWorker backgroundWorker6;
        private System.ComponentModel.BackgroundWorker backgroundWorker7;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem batchAnalyzeForMissingTomMarkers;
        private System.Windows.Forms.ToolStripMenuItem separateFilesThatAreMissingTomMarkers;
    }
}

