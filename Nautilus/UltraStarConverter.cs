using Nautilus.Properties;
using Nautilus.x360;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Un4seen.Bass;

namespace Nautilus
{
    public partial class UltraStarConverter : Form
    {

        private static string inputDir;
        private static List<string> inputFiles;
        private DateTime endTime;
        private DateTime startTime;
        private string UltraStarFolder;
        private readonly PhaseShiftSong Song;
        private readonly NemoTools Tools;
        private readonly DTAParser Parser;
        private MIDIStuff MIDITools;

        public UltraStarConverter()
        {
            InitializeComponent();

            Song = new PhaseShiftSong();
            Tools = new NemoTools();
            Parser = new DTAParser();
            MIDITools = new MIDIStuff();

            inputFiles = new List<string>();
        }

        private void btnFolder_Click(object sender, EventArgs e)
        {
            //if user selects new folder, assign that value
            //if user cancels or selects same folder, this forces the text_changed event to run again
            var tFolder = txtFolder.Text;
            var ofd = new FolderPicker
            {
                InputPath = tFolder,
                Title = "Select folder where your source files are",
            };
            if (ofd.ShowDialog(IntPtr.Zero) == true)
            {
                txtFolder.Text = ofd.ResultPath;
                Tools.CurrentFolder = txtFolder.Text;
            }
            else
            {
                txtFolder.Text = tFolder;
            }
        }

        private void Log(string message)
        {
            if (lstLog.InvokeRequired)
            {
                lstLog.Invoke(new MethodInvoker(() => lstLog.Items.Add(message)));
                lstLog.Invoke(new MethodInvoker(() => lstLog.SelectedIndex = lstLog.Items.Count - 1));
            }
            else
            {
                lstLog.Items.Add(message);
                lstLog.SelectedIndex = lstLog.Items.Count - 1;
            }
        }

        private void txtFolder_TextChanged(object sender, EventArgs e)
        {
            if (picWorking.Visible) return;
            inputFiles.Clear();

            if (string.IsNullOrWhiteSpace(txtFolder.Text))
            {
                btnRefresh.Visible = false;
            }
            btnRefresh.Visible = true;

            if (txtFolder.Text != "")
            {
                Tools.CurrentFolder = txtFolder.Text;
                Log("");
                Log("Reading input directory ... hang on");

                try
                {
                    var inFiles = Directory.GetFiles(txtFolder.Text);
                    foreach (var file in inFiles)
                    {
                        try
                        {
                            if (VariousFunctions.ReadFileType(file) == XboxFileType.STFS)
                            {
                                inputFiles.Add(file);
                            }
                            else if (Path.GetExtension(file).Equals(".mid"))
                            {
                                inputFiles.Add(file);
                            }
                        }
                        catch (Exception ex)
                        {
                            if (Path.GetExtension(file) != "") continue;
                            Log("There was a problem accessing file " + Path.GetFileName(file));
                            Log("The error says: " + ex.Message);
                        }
                    }
                    if (!inputFiles.Any())
                    {
                        Log("Did not find any CON or MIDI files ... try a different directory");
                        Log("You can also drag and drop CON or MIDI files here");
                        Log("Ready");
                        btnBegin.Visible = false;
                        btnRefresh.Visible = true;
                    }
                    else
                    {
                        Log("Found " + inputFiles.Count + " CON or MIDI " + (inputFiles.Count > 1 ? "files" : "file"));
                        Log("Ready to begin");
                        btnBegin.Visible = true;
                        btnRefresh.Visible = true;
                    }
                }
                catch (Exception ex)
                {
                    Log("There was an error: " + ex.Message);
                }
            }
            else
            {
                btnBegin.Visible = false;
                btnRefresh.Visible = false;
            }
            txtFolder.Focus();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Log("Resetting...");
            inputFiles.Clear();
            EnableDisable(true);
            btnBegin.Visible = true;
            btnBegin.Enabled = true;
            btnReset.Visible = false;
            btnFolder.Enabled = true;
            btnRefresh.Enabled = true;
            btnRefresh.PerformClick();
        }

        private void EnableDisable(bool enabled)
        {
            btnFolder.Enabled = enabled;
            btnRefresh.Enabled = enabled;
            txtFolder.Enabled = enabled;
            picWorking.Visible = !enabled;
            lstLog.Cursor = enabled ? Cursors.Default : Cursors.WaitCursor;
            Cursor = lstLog.Cursor;
            grpFormat.Enabled = enabled;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            var tFolder = txtFolder.Text;
            txtFolder.Text = "";
            txtFolder.Text = tFolder;
        }

        private void picPin_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            switch (picPin.Tag.ToString())
            {
                case "pinned":
                    picPin.Image = Resources.unpinned;
                    picPin.Tag = "unpinned";
                    break;
                case "unpinned":
                    picPin.Image = Resources.pinned;
                    picPin.Tag = "pinned";
                    break;
            }
            TopMost = picPin.Tag.ToString() == "pinned";
        }

        private void btnBegin_Click(object sender, EventArgs e)
        {
            if (btnBegin.Text == "Cancel")
            {
                backgroundWorker1.CancelAsync();
                Log("User cancelled process...stopping as soon as possible");
                btnBegin.Enabled = false;
                return;
            }

            startTime = DateTime.Now;
            UltraStarFolder = txtFolder.Text + "\\UltraStar\\";
            Tools.CurrentFolder = txtFolder.Text;
            EnableDisable(false);

            if (!Directory.Exists(UltraStarFolder))
            {
                Directory.CreateDirectory(UltraStarFolder);
            }

            try
            {
                var files = Directory.GetFiles(txtFolder.Text);
                if (files.Count() != 0)
                {
                    btnBegin.Text = "Cancel";
                    backgroundWorker1.RunWorkerAsync();
                }
                else
                {
                    Log("No files found ... there's nothing to do");
                    EnableDisable(true);
                }
            }
            catch (Exception ex)
            {
                Log("Error retrieving files to process");
                Log("The error says:" + ex.Message);
                EnableDisable(true);
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Log("Done!");
            endTime = DateTime.Now;
            var timeDiff = endTime - startTime;
            Log("Process took " + timeDiff.Minutes + (timeDiff.Minutes == 1 ? " minute" : " minutes") + " and " + (timeDiff.Minutes == 0 && timeDiff.Seconds == 0 ? "1 second" : timeDiff.Seconds + " seconds"));
            Log("Click 'Reset' to start again or just close me down");           
            btnReset.Enabled = true;
            btnReset.Visible = true;
            picWorking.Visible = false;
            lstLog.Cursor = Cursors.Default;
            Cursor = lstLog.Cursor;
            btnBegin.Text = "&Begin";
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (ProcessFiles()) return;
            Log("There was an error processing the files ... stopping here");
        }

        private bool ProcessFiles()
        {
            var counter = 0;
            var success = 0;
            var isMidi = false;
            var isCON = false;
            foreach (var file in inputFiles.Where(File.Exists).TakeWhile(file => !backgroundWorker1.CancellationPending))
            {
                try
                {
                    isMidi = Path.GetExtension(file).Equals(".mid");
                    isCON = VariousFunctions.ReadFileType(file) == XboxFileType.STFS;
                    if (!isCON && !isMidi) continue;
                    Song.NewSong();
                    
                    try
                    {                        
                        counter++;
                        if (isCON)
                        {
                            Parser.ExtractDTA(file);
                            Parser.ReadDTA(Parser.DTA);
                            if (string.IsNullOrEmpty(Parser.Songs[0].Artist) && (Parser.Songs[0].Source.Equals("beatles")))
                            {
                                Parser.Songs[0].Artist = "The Beatles";
                            }
                            if (Parser.Songs.Count > 1)
                            {
                                Log("File " + Path.GetFileName(file) + " is a pack, try dePACKing first, skipping...");
                                continue;
                            }
                            if (!Parser.Songs.Any())
                            {
                                Log("There was an error processing the songs.dta file");
                                continue;
                            }

                            Log("Loaded and processed songs.dta file for song #" + counter + " successfully");
                            Log("Song #" + counter + " is " + Parser.Songs[0].Artist + " - " + Parser.Songs[0].Name);
                        }
                        else
                        {
                            Log("Processing MIDI file '" + Path.GetFileName(file) + "'");
                        }                         
                        if (isMidi)
                        {
                            var localFolder = Path.GetDirectoryName(file) + "\\";
                            var ini = localFolder + "song.ini";
                            if (File.Exists(ini))
                            {
                                Parser.ReadINIFile(ini);
                                Log("Loaded and processed song.ini file for song #" + counter + " successfully");
                                Log("Song #" + counter + " is " + Parser.Songs[0].Artist + " - " + Parser.Songs[0].Name);
                            }
                            else
                            {
                                Log("No matching song.ini file found - can't proceed");
                                continue;
                            }
                            MIDITools.Initialize(true);
                            MIDITools.ReadMIDIFile(file, Parser.Songs[0].HopoThreshold, true);
                            if (MIDITools.MIDI_Chart.Vocals.ChartedNotes.Count() == 0)
                            {
                                Log("This song does not appear to have vocals charted, can't proceed");
                                continue;
                            }
                            var art = localFolder + "album.png";
                            if (File.Exists(art))
                            {
                                Log("Found album art - converting to correct format");
                                var jpgFile = localFolder + Parser.Songs[0].Artist.Replace("\"", "") + " - " + 
                                    Parser.Songs[0].Name.Replace("\"", "") + " [CO].jpg";
                                ConvertPngToJpg(art, jpgFile);
                            }
                            WriteTXTFile(localFolder, file, true);
                        }
                        else
                        {
                            if (!Directory.Exists(UltraStarFolder))
                            {
                                Directory.CreateDirectory(UltraStarFolder);
                            }
                            var songFolder = UltraStarFolder + Tools.CleanString(Parser.Songs[0].Artist.Replace("\"", "") + " - " + Parser.Songs[0].Name.Replace("\"", ""), false) + "\\";
                            if (!Directory.Exists(songFolder))
                            {
                                Directory.CreateDirectory(songFolder);
                            }
                            var internalName = Parser.Songs[0].InternalName;
                            var xPackage = new STFSPackage(file);
                            if (!xPackage.ParseSuccess)
                            {
                                Log("Failed to parse '" + Path.GetFileName(file) + "'");
                                Log("Skipping this file");
                                continue;
                            }
                            var xArt = xPackage.GetFile("songs/" + internalName + "/gen/" + internalName + "_keep.png_xbox");
                            if (xArt != null)
                            {
                                var newArt = songFolder + "album.png_xbox";
                                if (xArt.ExtractToFile(newArt))
                                {
                                    Log("Extracted album art file " + internalName + "_keep.png_xbox successfully");
                                    fromXbox(newArt);
                                }
                                else
                                {
                                    Log("There was a problem extracting the album art file");
                                }
                            }
                            else
                            {
                                Log("WARNING: Did not find album art file in that CON file");
                            }
                            var newMIDI = songFolder + "notes.mid";
                            var xMIDI = xPackage.GetFile("songs/" + internalName + "/" + internalName + ".mid");
                            if (xMIDI != null)
                            {
                                if (xMIDI.ExtractToFile(newMIDI))
                                {
                                    Log("Extracted MIDI file " + internalName + ".mid successfully");
                                    MIDITools.Initialize(true);
                                    MIDITools.ReadMIDIFile(newMIDI, Parser.Songs[0].HopoThreshold, true);
                                    if (MIDITools.MIDI_Chart.Vocals.ChartedNotes.Count() == 0)
                                    {
                                        Log("This song does not appear to have vocals charted, can't proceed");
                                        xPackage.CloseIO();
                                        continue;
                                    }
                                }
                                else
                                {
                                    Log("There was a problem extracting the MIDI file");
                                    Log("Skipping this song...");
                                    xPackage.CloseIO();
                                    continue;
                                }
                            }
                            else
                            {
                                Log("ERROR: Did not find a MIDI file in that CON file!");
                                Log("Skipping this song...");
                                xPackage.CloseIO();
                                continue;
                            }
                            var xMOGG = xPackage.GetFile("songs/" + internalName + "/" + internalName + ".mogg");
                            if (xMOGG != null)
                            {
                                xPackage.CloseIO();
                                DownMixAudio(file, songFolder, Parser.Songs[0].Artist + " - " + Parser.Songs[0].Name);
                            }
                            else
                            {
                                Log("ERROR: Did not find an audio file in that CON file!");
                                Log("Skipping this song...");
                                xPackage.CloseIO();
                                continue;
                            }
                            if (!Directory.Exists(songFolder))
                            {
                                Directory.CreateDirectory(songFolder);
                            }
                            WriteTXTFile(songFolder, newMIDI, false);
                            Tools.DeleteFile(newMIDI);
                        }                        
                        success++;                        
                    }
                    catch (Exception ex)
                    {
                        Log("There was an error: " + ex.Message);
                        Log("Attempting to continue with the next file");
                    }
                }
                catch (Exception ex)
                {
                    Log("There was a problem accessing that file");
                    Log("The error says: " + ex.Message);
                }
            }
            Log("Successfully processed " + success + " of " + counter + " files");
            return true;
        }

        public sealed class UltraStarNote
        {
            public double StartMs { get; set; }
            public double EndMs { get; set; }
            public int MidiPitch { get; set; }
            public string Lyric { get; set; } = "";
            public bool Golden { get; set; }
            public bool Freestyle { get; set; }
            public bool Rap { get; set; }
        }

        public sealed class UltraStarPhrase
        {
            public double StartMs { get; set; }
            public double EndMs { get; set; }
            public List<UltraStarNote> Notes { get; } = new List<UltraStarNote>();
        }

        private void WriteTXTFile(string path, string midiPath, bool isMidi)
        {            
            double ultraStarBpm = Math.Round(MIDITools.AverageBPM() * 4.0, 3);
            double beatMs = 15000.0 / ultraStarBpm;

            var ext = isMidi ? ".ogg" : (radioMP3.Checked ? ".mp3" : (radioWAV.Checked ? ".wav" : ".ogg"));
            var utf8NoBom = new UTF8Encoding(false);

            var phrases = new List<UltraStarPhrase>();

            foreach (var phraseMarker in MIDITools.PhrasesVocals.Phrases)
            {
                var phrase = new UltraStarPhrase
                {
                    StartMs = phraseMarker.PhraseStart * 1000.0,
                    EndMs = phraseMarker.PhraseEnd * 1000.0
                };

                foreach (var note in MIDITools.MIDI_Chart.Vocals.ChartedNotes)
                {
                    if (note.NoteStart < phraseMarker.PhraseStart)
                        continue;

                    if (note.NoteStart >= phraseMarker.PhraseEnd)
                        continue;

                    string lyricText = FindLyricForNote(
                        note.NoteStart,
                        MIDITools.LyricsVocals.Lyrics);

                    if (string.IsNullOrWhiteSpace(lyricText))
                        continue;

                    phrase.Notes.Add(new UltraStarNote
                    {
                        StartMs = note.NoteStart * 1000.0,
                        EndMs = note.NoteEnd * 1000.0,
                        MidiPitch = note.NoteNumber,
                        Lyric = lyricText,
                        Rap = lyricText.Contains("#") || lyricText.Contains("^")
                    });
                }

                if (phrase.Notes.Count > 0)
                    phrases.Add(phrase);
            }

            var allNotes = phrases
                .SelectMany(p => p.Notes)
                .Where(n => !string.IsNullOrWhiteSpace(n.Lyric))
                .OrderBy(n => n.StartMs)
                .ToList();

            if (allNotes.Count == 0)
                return;

            // Store the first vocal timing in GAP, then write notes relative to that.
            double gapMs = allNotes[0].StartMs;

            string artist = Parser.Songs[0].Artist.Replace("\"", "");
            string title = Parser.Songs[0].Name.Replace("\"", "");

            string txtPath = Path.Combine(path, (Tools.CleanString(artist + " - " + title, false) + ".txt"));

            Log("Writing TXT file:");
            Log(txtPath);
            using (var sw = new StreamWriter(txtPath, false, utf8NoBom))
            {
                sw.WriteLine("#VERSION:1.1.0");
                sw.WriteLine("#TITLE:" + Parser.Songs[0].Name);
                sw.WriteLine("#ARTIST:" + Parser.Songs[0].Artist);
                if (isMidi)
                {
                    sw.WriteLine("#AUDIO:song.ogg");
                }
                else
                {
                    sw.WriteLine("#AUDIO:" + artist + " - " + title + ext);
                }
                if (IsInstrumental)
                {
                    sw.WriteLine("#VOCALS:" + artist + " - " + title + " [VOC]" + ext);
                    sw.WriteLine("#INSTRUMENTAL:" + artist + " - " + title + " [INSTR]" + ext);
                }

                string cover = artist + " - " + title + " [CO].jpg";
                if (File.Exists(Path.Combine(path, cover)))
                {
                    sw.WriteLine("#COVER:" + cover);
                }

                sw.WriteLine("#BPM:" + ultraStarBpm.ToString("0.###", CultureInfo.InvariantCulture));
                sw.WriteLine("#GAP:" + gapMs.ToString("0", CultureInfo.InvariantCulture));

                if (!string.IsNullOrWhiteSpace(Parser.Songs[0].Genre))
                {
                    sw.WriteLine("#GENRE:" + Parser.Songs[0].Genre);
                }

                if (Parser.Songs[0].YearReleased > 0)
                {
                    sw.WriteLine("#YEAR:" + Parser.Songs[0].YearReleased);
                }

                if (!string.IsNullOrWhiteSpace(Parser.Songs[0].Languages))
                {
                    sw.WriteLine("#LANGUAGE:" + Parser.Songs[0].Languages);
                }

                double previewStartSec = Parser.Songs[0].PreviewStart / 1000.0;
                sw.WriteLine("#PREVIEWSTART:" + previewStartSec.ToString("0.###", CultureInfo.InvariantCulture));

                if (!string.IsNullOrWhiteSpace(Parser.Songs[0].ChartAuthor))
                {
                    sw.WriteLine("#CREATOR:" + Parser.Songs[0].ChartAuthor);
                }

                sw.WriteLine("#COMMENT:Created with Nautilus");

                foreach (var phrase in phrases.OrderBy(p => p.StartMs))
                {
                    var notes = phrase.Notes
                        .Where(n => !string.IsNullOrWhiteSpace(n.Lyric))
                        .OrderBy(n => n.StartMs)
                        .ToList();

                    bool nextSyllableContinuesWord = false;

                    for (int i = 0; i < notes.Count; i++)
                    {
                        var note = notes[i];

                        string rawLyric = note.Lyric ?? "";

                        string lyric = PrepareUltraStarLyric(
                            rawLyric,
                            isFirstSyllableInPhrase: i == 0,
                            continuationFromPrevious: nextSyllableContinuesWord);

                        // This sets the state for the NEXT syllable.
                        nextSyllableContinuesWord = ContinuesIntoNextSyllable(rawLyric);

                        if (string.IsNullOrEmpty(lyric))
                            continue;

                        int startBeat = (int)Math.Round((note.StartMs - gapMs) / beatMs);
                        int lengthBeat = Math.Max(1, (int)Math.Round((note.EndMs - note.StartMs) / beatMs));
                        int pitch = note.MidiPitch - 60;

                        string type = note.Golden ? "*" : note.Freestyle ? "F" : note.Rap ? "R" : ":";

                        sw.WriteLine(type + " " + startBeat + " " + lengthBeat + " " + pitch + " " + lyric);
                    }

                    int phraseEndBeat = (int)Math.Round((phrase.EndMs - gapMs) / beatMs);

                    if (notes.Count > 0)
                    {
                        int lastNoteStartBeat = (int)Math.Round((notes[notes.Count - 1].StartMs - gapMs) / beatMs);
                        phraseEndBeat = Math.Max(phraseEndBeat, lastNoteStartBeat + 1);
                    }

                    sw.WriteLine("- " + phraseEndBeat);
                }

                sw.WriteLine("E");
            }
            Log("Finished");
        }

        private static bool ContinuesIntoNextSyllable(string rawLyric)
        {
            if (string.IsNullOrEmpty(rawLyric))
                return false;

            string t = rawLyric.TrimEnd().Replace("#", "").Replace("^", "");

            return t.EndsWith("-") ||
                   t.EndsWith("=") ||
                   t.EndsWith("+");
        }

        private static string PrepareUltraStarLyric(
            string rawLyric,
            bool isFirstSyllableInPhrase,
            bool continuationFromPrevious)
        {
            if (string.IsNullOrEmpty(rawLyric))
                return "";

            string lyric = rawLyric.Replace("\r", "").Replace("\n", "");

            // Remove Rock Band control/scoring markers.
            lyric = lyric.Replace("#", "");
            lyric = lyric.Replace("^", "");
            lyric = lyric.Replace("*", "");
            lyric = lyric.Replace("%", "");
            lyric = lyric.Replace("$", "");
            lyric = lyric.Replace("§", " ");

            // Preserve the fact that "-" meant continuation, but don't display it.
            lyric = lyric.TrimEnd();

            if (lyric.EndsWith("-") || lyric.EndsWith("="))
                lyric = lyric.Substring(0, lyric.Length - 1);

            lyric = lyric.Replace("+", "~"); //preserve the slide
            lyric = lyric.Replace("_", " ");
            lyric = lyric.Replace("/", "");

            if (string.IsNullOrEmpty(lyric))
                return "";

            // First syllable in a phrase should not start with a visible space.
            if (isFirstSyllableInPhrase)
                return lyric.TrimStart();

            // If previous syllable ended with "-", this is the rest of the same word.
            // Example:
            // tri-
            // umph
            // should become "tri" + "umph", not "tri umph".
            if (continuationFromPrevious)
                return lyric.TrimStart();

            // If the original lyric already had a leading space, keep it.
            if (lyric.StartsWith(" "))
                return lyric;

            // Otherwise add a leading space for a new word.
            return " " + lyric.TrimStart();
        }

        private static string FindLyricForNote(
            double noteStartSeconds,
            List<Lyric> lyrics,
            double toleranceSeconds = 0.025)
        {
            var match = lyrics
                .Select(l => new
                {
                    Lyric = l,
                    Delta = Math.Abs((double)l.Start - noteStartSeconds)
                })
                .Where(x => x.Delta <= toleranceSeconds)
                .OrderBy(x => x.Delta)
                .FirstOrDefault();

            if (match == null)
                return "";

            return match.Lyric.Text ?? "";
        }

        public sealed class TempoPoint
        {
            public long Tick { get; set; }
            public int MicrosecondsPerQuarter { get; set; }
        }

        public static double MidiTickToMilliseconds(
            long tick,
            List<TempoPoint> tempos,
            int ppq)
        {
            if (tempos == null || tempos.Count == 0)
            {
                tempos = new List<TempoPoint>
        {
            new TempoPoint { Tick = 0, MicrosecondsPerQuarter = 500000 }
        };
            }

            tempos = tempos
                .OrderBy(t => t.Tick)
                .ToList();

            double microseconds = 0;
            long previousTick = tempos[0].Tick;
            int previousMpq = tempos[0].MicrosecondsPerQuarter;

            if (previousTick > 0)
            {
                previousTick = 0;
            }

            foreach (var tempo in tempos.Skip(1))
            {
                if (tempo.Tick >= tick)
                    break;

                long deltaTicks = tempo.Tick - previousTick;
                microseconds += deltaTicks * (previousMpq / (double)ppq);

                previousTick = tempo.Tick;
                previousMpq = tempo.MicrosecondsPerQuarter;
            }

            microseconds += (tick - previousTick) * (previousMpq / (double)ppq);

            return microseconds / 1000.0;
        }

        public static void ConvertPngToJpg(string pngPath, string jpgPath)
        {
            using (Image image = Image.FromFile(pngPath))
            {
                image.Save(jpgPath, ImageFormat.Jpeg);
            }
        }

        private void fromXbox(string image)
        {
            var jpgFile = UltraStarFolder + (Tools.CleanString(Parser.Songs[0].Artist.Replace("\"", "") + " - " + Parser.Songs[0].Name.Replace("\"", ""), false) + "\\" + Tools.CleanString(Parser.Songs[0].Artist.Replace("\"", "") + " - " + Parser.Songs[0].Name.Replace("\"", ""), false) + " [CO].jpg");
            try
            {
                Log(Tools.ConvertRBImage(image, jpgFile, "jpg", true) ? "Converted album art file successfully" : "There was an error when converting the album art file");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong when trying to convert the album art file\nfrom the native Rock Band format\nSorry\n\nThe message says: " + ex.Message,
                    Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Log("There was an error when converting the album art file");
                Log("The error says: " + ex.Message);
            }
        }

        private bool BassInit = false;
        private bool IsInstrumental = false;
        private void DownMixAudio(string CON, string folder, string fileName)
        {
            folder = folder.Replace("\"", "");
            fileName = Tools.CleanString(fileName.Replace("\"", ""), false);

            var ext = radioMP3.Checked ? ".mp3" : (radioWAV.Checked ? ".wav" : ".ogg");
            if (backgroundWorker1.CancellationPending) return;

            var mix = folder + fileName + ext;
            var instrumental = folder + fileName + " [INSTR]" + ext;
            var vocals = folder + fileName + " [VOC]" + ext;

            Log("Downmixing audio file to stereo mix:");
            Log(mix);
            var splitter = new MoggSplitter();            
            var createdMix = splitter.DownmixMogg(CON, mix, radioMP3.Checked ? MoggSplitter.MoggSplitFormat.MP3 : (radioWAV.Checked ? MoggSplitter.MoggSplitFormat.WAV : MoggSplitter.MoggSplitFormat.OGG), "allstems");
            foreach (var error in splitter.ErrorLog)
            {
                Log(error);
            }
            splitter.ErrorLog.Clear();
            Log(createdMix && File.Exists(mix) ? "Success" : "Failed");

            Log("Creating instrumental track:");
            Log(instrumental);            
            var createdInstrumental = splitter.DownmixMogg(CON, instrumental, radioMP3.Checked ? MoggSplitter.MoggSplitFormat.MP3 : (radioWAV.Checked ? MoggSplitter.MoggSplitFormat.WAV : MoggSplitter.MoggSplitFormat.OGG), "drums|bass|guitar|keys|backing");
            foreach (var error in splitter.ErrorLog)
            {
                Log(error);
            }
            splitter.ErrorLog.Clear();
            Log(createdInstrumental && File.Exists(instrumental) ? "Success" : "Failed");

            Log("Extracting vocals track:");
            Log(vocals);
            var createdVocals = splitter.DownmixMogg(CON, vocals, radioMP3.Checked ? MoggSplitter.MoggSplitFormat.MP3 : (radioWAV.Checked ? MoggSplitter.MoggSplitFormat.WAV : MoggSplitter.MoggSplitFormat.OGG), "vocals|NOcrowd");
            foreach (var error in splitter.ErrorLog)
            {
                Log(error);
            }
            Log(createdVocals && File.Exists(vocals) ? "Success" : "Failed");

            Log("Analyzing vocals track to ensure audio is present");
            if (!BassInit)
            {
                Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
                BassInit = true;
            }
            int BassStream = Bass.BASS_StreamCreateFile(vocals, 0L, 0L, BASSFlag.BASS_STREAM_DECODE);
            if (BassStream == 0)
            {
                Log("Failed to read the vocals audio file");
                Log("BASS ERROR: " + Bass.BASS_ErrorGetCode());                
            }
            var level = new float[1];
            while (Bass.BASS_ChannelGetLevel(BassStream, level, 1, BASSLevel.BASS_LEVEL_MONO))
            {
                if (level[0] != 0) break;
            }
            Bass.BASS_StreamFree(BassStream);
            BassInit = false;
            if (level[0] == 0)
            {
                Log("Vocals audio file is empty, this is not a multitrack file");
                IsInstrumental = false;
                return;
            }
            Log("Vocals audio file reports content, good to go");
            IsInstrumental = true;
        }

        private void lstLog_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
        }

        private void lstLog_DragDrop(object sender, DragEventArgs e)
        {
            if (picWorking.Visible) return;
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (btnReset.Visible)
            {
                btnReset.PerformClick();
            }
            if (VariousFunctions.ReadFileType(files[0]) == XboxFileType.STFS || Path.GetExtension(files[0]) == ".mid")
            {
                txtFolder.Text = Path.GetDirectoryName(files[0]);
                Tools.CurrentFolder = txtFolder.Text;
            }            
            else
            {
                MessageBox.Show("That's not a valid file to drop here", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void UltraStarConverter_Shown(object sender, EventArgs e)
        {
            Log("Welcome to " + Text);
            Log("Drag and drop the Rock Band CON or MIDI file(s) to be converted here");
            Log("Or click 'Change Input Folder' to select the folder containing the files");
            Log("Ready to begin");
            txtFolder.Text = inputDir;
        }

        private void exportLogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.ExportLog(Text, lstLog.Items);
        }
    }
}
