using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Nautilus.Properties;
using Nautilus.x360;
using SearchOption = System.IO.SearchOption;
using System.Drawing;
using NAudio.Midi;
using System.Drawing.Imaging;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Mix;
using Un4seen.Bass.AddOn.Enc;
using Un4seen.Bass.AddOn.EncOgg;
using NautilusFREE;

namespace Nautilus
{
    public partial class PS3Converter : Form
    {
        private static List<string> inputFiles;
        private DateTime endTime;
        private DateTime startTime;
        private readonly string PS3Folder;
        private readonly MainForm xMainForm;
        private readonly string rar;
        private string SongName;
        private string SongArtist;
        private string internalName;
        private readonly NemoTools Tools;
        private readonly MoggSplitter Splitter;
        private readonly DTAParser Parser;
        private readonly string bin;
        private readonly string MergedSongsFolder;
        private readonly string AllSongsFolder;
        private readonly string ToMergeFolder;
        private readonly string MergeDTA;
        private readonly string CONFolder;        
        private readonly string DataFolder;
        private readonly nTools nautilus3;
        private int fixCounter;
        private int fixSuccess;
        private string fixFolder;
        private int fixIgnore;
        private List<string> conFiles;
        private const int BassBuffer = 1000;
        private int BassStream;
        private int BassMixer;
        private int currentChannel;
        private bool isLinosSpecial;

        public PS3Converter(MainForm xParent, Color ButtonBackColor, Color ButtonTextColor)
        {
            xMainForm = xParent;
            InitializeComponent();
            Tools = new NemoTools();
            Parser = new DTAParser();
            Splitter = new MoggSplitter();
            inputFiles = new List<string>();
            conFiles = new List<string>();
            PS3Folder = Application.StartupPath + "\\ps3\\";
            MergedSongsFolder = PS3Folder + "Merged Songs\\";
            AllSongsFolder = PS3Folder + "All Songs\\";
            ToMergeFolder = PS3Folder + "Songs to Merge\\";
            CONFolder = PS3Folder + "CONs\\";
            MergeDTA = MergedSongsFolder + "songs.dta";
            DataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\nautilus\\";
            bin = Application.StartupPath + "\\bin\\";
            nautilus3 = new nTools();
            LoadConfig();
            if (!Directory.Exists(MergedSongsFolder))
            {
                Directory.CreateDirectory(MergedSongsFolder);
            }
            if (!Directory.Exists(AllSongsFolder))
            {
                Directory.CreateDirectory(AllSongsFolder);
            }
            if (!Directory.Exists(ToMergeFolder))
            {
                Directory.CreateDirectory(ToMergeFolder);
            }
            if (!Directory.Exists(CONFolder))
            {
                Directory.CreateDirectory(CONFolder);
            }
            if (!File.Exists(MergeDTA))
            {
                //create blank file
                File.Create(MergeDTA);
            }
            chkMerge.Enabled = File.Exists(MergeDTA);
            mergeSongsToolStrip.Enabled = chkMerge.Enabled;
            managePackDTAFile.Enabled = chkMerge.Enabled;
            var formButtons = new List<Button> { btnReset, btnRefresh,btnFolder,btnBegin, btnOnyx};
            foreach (var button in formButtons)
            {
                button.BackColor = ButtonBackColor;
                button.ForeColor = ButtonTextColor;
                button.FlatAppearance.MouseOverBackColor = button.BackColor == Color.Transparent ? Color.FromArgb(127, Color.AliceBlue.R, Color.AliceBlue.G, Color.AliceBlue.B) : Tools.LightenColor(button.BackColor);
            }
            rar = bin + "rar.exe";
            if (!File.Exists(rar))
            {
                MessageBox.Show("Can't find rar.exe ... I won't be able to create RAR files for your songs without it",
                                "Missing Executable", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                chkRAR.Checked = false;
                chkRAR.Enabled = false;
            }
            toolTip1.SetToolTip(btnBegin, "Click to begin process");
            toolTip1.SetToolTip(btnFolder, "Click to select the input folder");
            toolTip1.SetToolTip(btnRefresh, "Click to refresh if the contents of the folder have changed");
            toolTip1.SetToolTip(txtFolder, "This is the working directory");
            toolTip1.SetToolTip(lstLog, "This is the application log. Right click to export");
        }

        private void LoadConfig()
        {
            var config = Application.StartupPath + "\\bin\\config\\ps3.config";
            var backup = DataFolder + Path.GetFileName(config);
            if (!File.Exists(config) && File.Exists(backup))
            {
                if (MessageBox.Show("It looks like you don't have a PS3 configuration file in the /bin folder, but I found a backup. Do you want me to " +
                                    "restore that?\n\n[RECOMMENDED]\nClick Yes to restore and continue from the last numeric ID you used\n\n" +
                                    "[NOT RECOMMENDED]\nClick No to ignore and start over - LIKELY TO CAUSE ID CONFLICTS!", Text, MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    File.Copy(backup, config);
                    
                }
            }
            if (File.Exists(config))
            {
                var sr = new StreamReader(config);
                try
                {
                    var line = sr.ReadLine();
                    if (line.Contains("CurrentSongID")) //old config file, delete it
                    {
                        sr.Dispose();
                        Tools.DeleteFile(config);
                        return;
                    }
                    sr.ReadLine();
                    sr.ReadLine();
                    regionNTSC.Checked = sr.ReadLine().Contains("True");
                    regionPAL.Checked = !regionNTSC.Checked;
                    var type = Convert.ToInt16(Tools.GetConfigString(sr.ReadLine()));
                    type1.Checked = false;
                    type2.Checked = false;
                    type3.Checked = false;
                    switch (type)
                    {
                        default:
                            type1.Checked = true;
                            break;
                        case 2:
                            type2.Checked = true;
                            break;
                        case 3:
                            type3.Checked = true;
                            break;
                    }
                    var wait = Convert.ToInt16(Tools.GetConfigString(sr.ReadLine()));
                    wait2Seconds.Checked = false;
                    wait5Seconds.Checked = false;
                    wait10Seconds.Checked = false;
                    switch (wait)
                    {
                        default:
                            wait2Seconds.Checked = true;
                            break;
                        case 5:
                            wait5Seconds.Checked = true;
                            break;
                        case 10:
                            wait10Seconds.Checked = true;
                            break;
                    }
                }
                catch (Exception)
                {
                    regionNTSC.Checked = true;
                    type1.Checked = true;
                    wait2Seconds.Checked = true;
                }
                sr.Dispose();
            }
        }             

        private void SaveConfig()
        {
            var config = Application.StartupPath + "\\bin\\config\\ps3.config";
            var backup = DataFolder + Path.GetFileName(config);

            var sw = new StreamWriter(config, false);
            try
            {
                sw.WriteLine("SongIDPrefix=" + 0);
                sw.WriteLine("AuthorID=" + 0);
                sw.WriteLine("CurrentSongNumber=" + 0);
                sw.WriteLine("UseNTSC=" + regionNTSC.Checked);
                int type;
                if (type1.Checked)
                {
                    type = 1;
                }
                else if (type2.Checked)
                {
                    type = 2;
                }
                else
                {
                    type = 3;
                }
                sw.WriteLine("EncryptionType=" + type);
                int wait;
                if (wait2Seconds.Checked)
                {
                    wait = 2;
                }
                else if (wait5Seconds.Checked)
                {
                    wait = 5;
                }
                else
                {
                    wait = 10;
                }
                sw.WriteLine("Type2Wait=" + wait);
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error saving the song ID configuration file\nThis is what happened:\n\n" + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            sw.Dispose();

            try
            {
                Tools.DeleteFile(backup);
                File.Copy(config, backup);
            }
            catch (Exception)
            {}
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
            chkMerge.Enabled = File.Exists(MergeDTA);
            mergeSongsToolStrip.Enabled = chkMerge.Enabled;
            managePackDTAFile.Enabled = chkMerge.Enabled;
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
                        Log("Did not find any CON files ... try a different directory");
                        Log("You can also drag and drop CON files here");
                        Log("Ready");
                        btnBegin.Visible = false;
                        btnRefresh.Visible = true;
                    }
                    else
                    {
                        Log("Found " + inputFiles.Count + " CON " + (inputFiles.Count > 1 ? "files" : "file"));
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
        
        public bool loadDTA(uint TitleID)
        {
            try
            {
                internalName = Parser.Songs[0].InternalName;
                SongName = Parser.Songs[0].Name;
                SongArtist = Parser.Songs[0].Artist;
                if (string.IsNullOrEmpty(SongArtist) && (TitleID == 0x454108B1 || TitleID == (uint)4294838225)) //for TBRB customs
                {
                    SongArtist = "The Beatles";
                }
                return (SongName != "" && SongArtist != "" && internalName != "");
            }
            catch (Exception ex)
            {
                Log("There was an error processing that songs.dta file");
                Log("The error says: " + ex.Message);
                return false;
            }
        }
        
        private bool ProcessFiles()
        {
            var counter = 0;
            var success = 0;
            internalName = "";
            SongName = "";
            SongArtist = "";
            var internalFolder = "";
            foreach (var file in inputFiles.Where(File.Exists).TakeWhile(file => !backgroundWorker1.CancellationPending))
            {
                try
                {
                    if (VariousFunctions.ReadFileType(file) != XboxFileType.STFS) continue;
                    try
                    {
                        counter++;
                        Parser.ExtractDTA(file);
                        Parser.ReadDTA(Parser.DTA);
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
                        var xPackage = new STFSPackage(file);
                        if (!xPackage.ParseSuccess)
                        {
                            Log("Failed to parse '" + Path.GetFileName(file) + "'");
                            Log("Skipping this file");
                            continue;
                        }
                        if (loadDTA(xPackage.Header.TitleID))
                        {
                            Log("Loaded and processed songs.dta file for song #" + counter + " successfully");
                            Log("Song #" + counter + " is " + SongArtist + " - " + SongName);
                        }
                        else
                        {
                            xPackage.CloseIO();
                            return false;
                        }
                        var songExtracted = PS3Folder + "temp\\" + Path.GetFileName(file).Replace(" ","") + "_extracted\\";
                        var origExFolder = songExtracted;
                        var songFolder = chkMerge.Checked && chkMerge.Enabled ? MergedSongsFolder : AllSongsFolder + Tools.CleanString(SongArtist,false) + " - " + Tools.CleanString(SongName, false) + "\\";
                        internalFolder = songFolder + CleanString(internalName) + "\\";
                        var genFolder = internalFolder + "gen\\";
                        Tools.DeleteFolder(songExtracted,true);
                        xPackage.ExtractPayload(songExtracted, true, false);
                        xPackage.CloseIO();
                        if (!Directory.Exists(genFolder))
                        {
                            Directory.CreateDirectory(genFolder);
                        }
                        songExtracted += "Root\\songs\\";
                        var midi = songExtracted + internalName + "\\" + internalName + ".mid";
                        if (File.Exists(midi))
                        {
                            var newMIDI = internalFolder + CleanString(Path.GetFileName(midi));
                            Tools.DeleteFile(newMIDI);
                            if (Tools.MoveFile(midi, newMIDI))
                            {
                                Log("Extracted MIDI file " + Path.GetFileName(newMIDI) + " successfully");
                                if (type1.Checked)
                                {
                                    MakeEdat("edat1", newMIDI);
                                }
                                else if (type2.Checked)
                                {
                                    MakeEdat("edat2", newMIDI);
                                }
                                else
                                {
                                    MakeEdat("edat3", newMIDI);
                                }
                            }
                            else
                            {
                                Log("There was a problem extracting the MIDI file");
                            }
                        }
                        else
                        {
                            Log("WARNING: Could not find MIDI file");
                        }
                        var dtaOut = songExtracted + "songs.dta";
                        if (File.Exists(dtaOut))
                        {
                            try
                            {
                                var newDTA = songFolder + "songs.dta";
                                var sr = new StreamReader(dtaOut);
                                var sw = new StreamWriter(newDTA, chkMerge.Checked && chkMerge.Enabled);
                                var done = false;
                                while (sr.Peek() > 0)
                                {
                                    var line = sr.ReadLine();
                                    if (line.Trim() == "(" && !done)
                                    {
                                        sw.WriteLine(line);
                                        line = "   '" + CleanString(sr.ReadLine()) + "'"; //clean shortname
                                        done = true;
                                    }
                                    if (line.Contains("rating") && line.Contains("4"))
                                    {
                                        line = line.Replace("4", "2"); //change Not Rated to Supervision Recommended as needed
                                    }
                                    else if (line.Contains("songs/"))
                                    {
                                        //PS3 is case sensitive, files below are changed, need to reflect here
                                        //this also removes unsupported characters that create conflict
                                        line = line.Replace(Parser.Songs[0].InternalName, CleanString(Parser.Songs[0].InternalName)).ToLowerInvariant();
                                    }
                                    else if (line.Contains("song_id") && chkSongID.Checked)
                                    {
                                        if (!Parser.IsNumericID(line) || ((Parser.GetSongID(line).StartsWith("1000") && Parser.GetSongID(line).Length == 10))) //only if not already a numeric ID or it has the default 1000 MAGMA C3 Author ID
                                        {
                                            var origID = Parser.GetSongID(line);
                                            line = ";ORIG_ID=" + origID;
                                            sw.WriteLine(line);
                                            var corrector = new SongIDCorrector();
                                            line = "   ('song_id' " + corrector.ShortnameToSongID(Parser.Songs[0].ShortName) + ")";
                                        }
                                    }
                                    if (line.Trim() != "")
                                    {
                                        sw.WriteLine(line);
                                    }
                                }
                                sr.Dispose();
                                sw.Dispose();
                                Log("Extracted and modified songs.dta file successfully");
                            }
                            catch (Exception ex)
                            {
                                Log("WARNING: There was an error extracting and modifying the songs.dta file");
                                Log("Error says: " + ex.Message);
                            }
                        }
                        else
                        {
                            Log("WARNING: Could not find a songs.dta file");
                        }
                        var png = songExtracted + internalName + "\\gen\\" + internalName + "_keep.png_xbox";
                        if (File.Exists(png))
                        {
                            var newPNG = genFolder + CleanString(Path.GetFileName(png));
                            Tools.DeleteFile(newPNG);
                            if (Tools.MoveFile(png, newPNG))
                            {
                                Log("Extracted album art file " + Path.GetFileName(newPNG) + " successfully");
                                Log(Tools.ConvertXboxtoPS3(newPNG, newPNG, true)
                                    ? "Converted album art from png_xbox to png_ps3 successfully"
                                    : "There was a problem converting the album art to png_ps3 format");
                            }
                            else
                            {
                                Log("There was a problem extracting the album art file");
                            }
                        }
                        else
                        {
                            Log("WARNING: Could not find album art file");
                        }
                        var mogg = songExtracted + internalName + "\\" + internalName + ".mogg";
                        if (File.Exists(mogg))
                        {
                            var newMogg = internalFolder + CleanString(Path.GetFileName(mogg));
                            Tools.DeleteFile(newMogg);
                            if (Tools.MoveFile(mogg, newMogg))
                            {
                                Log("Extracted mogg file " + Path.GetFileName(newMogg) + " successfully");
                                EncryptMogg(newMogg);
                            }
                            else
                            {
                                Log("There was a problem extracting the mogg file");
                            }
                        }
                        else
                        {
                            Log("WARNING: Could not find mogg file");
                        }
                        var milo = songExtracted + internalName + "\\gen\\" + internalName + ".milo_xbox";
                        if (File.Exists(milo))
                        {
                            var newMILO = genFolder + CleanString(Path.GetFileNameWithoutExtension(milo)) + ".milo_ps3";
                            Tools.DeleteFile(newMILO);
                            if (Tools.MoveFile(milo, newMILO))
                            {
                                Log("Extracted milo file " + Path.GetFileName(newMILO) + " successfully");
                            }
                            else
                            {
                                Log("There was a problem extracting the milo file");
                            }
                        }
                        else
                        {
                            Log("WARNING: Could not find milo file");
                        }
                        Tools.DeleteFolder(PS3Folder + "temp\\", true);
                        success++;
                        //in case of stragglers
                        Tools.DeleteFile(internalFolder + "c.exe");
                        if (!chkRAR.Checked || (chkMerge.Checked && chkMerge.Enabled) || backgroundWorker1.CancellationPending) continue;
                        var archive = Path.GetFileName(file);
                        archive = archive.Replace(" ", "").Replace("-", "_").Replace("\\","").Replace("'", "").Replace(",", "").Replace("_rb3con","");
                        archive = Tools.CleanString(archive, false);
                        archive = txtFolder.Text + "\\" + archive + "_ps3.rar";
                        var arg = "a -m5 -r -ep1 \"" + archive + "\" \"" + songFolder + "\"";
                        Log("Creating RAR archive");
                        //in case of stragglers
                        Tools.DeleteFile(internalFolder + "c.exe");
                        Log(Tools.CreateRAR(rar, archive, arg) ? "Created RAR archive successfully" : "RAR archive creation failed");
                    }
                    catch (Exception ex)
                    {
                        //in case of stragglers
                        Tools.DeleteFile(internalFolder + "c.exe");
                        Log("There was an error: " + ex.Message);
                        Log("Attempting to continue with the next file");
                    }
                }
                catch (Exception ex)
                {
                    //in case of stragglers
                    Tools.DeleteFile(internalFolder + "c.exe");
                    Log("There was a problem accessing that file");
                    Log("The error says: " + ex.Message);
                }
            }
            //sometimes c.exe is left behind, let's make sure to delete them
            Tools.DeleteFile(internalFolder + "c.exe");
            Log("Successfully processed " + success + " of " + counter + " files");
            return true;
        }

        private static string CleanString(string line)
        {
            var bad_chars = new List<string>
            {
                "!","@","#","$","%","^","&","*","(",")","<",">","?","//","\\","~","`",",", "'"
            };
            line = bad_chars.Aggregate(line, (current, bad) => current.Replace(bad, ""));
            return line.ToLowerInvariant().Trim();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            var tFolder = txtFolder.Text;
            txtFolder.Text = "";
            txtFolder.Text = tFolder;
        }
        
        private void MakeEdat(string format, string file)
        {
            if (!File.Exists(file)) return;
            Log("Encrypting MIDI file '" + Path.GetFileName(file) + "' to EDAT");
            var region = regionNTSC.Checked ? "regionNTSC" : "regionPAL";
            var wait = wait2Seconds.Checked ? "2000" : (wait5Seconds.Checked ? "5000" : "10000");
            var arg = format + " " + region + " " + wait + " \"" + file + "\"";
            var startInfo = new ProcessStartInfo
            {
                FileName = bin + "nemoedat.exe",
                Arguments = arg,
                UseShellExecute = false
            };
            var process = Process.Start(startInfo);
            do
            {//
            } while (!process.HasExited);
            process.Dispose();
            var edat = file + ".edat";
            if (File.Exists(edat))
            {
                Log("Encrypted MIDI file to EDAT successfully");
            }
            else
            {
                Log("Failed to encrypt MIDI file " + Path.GetFileName(file) + " ... try again");
            }
        }
        
        private void HandleDragDrop(object sender, DragEventArgs e)
        {
            MessageBox.Show("This converter is permanently disabled. Use Onyx.", "Deprecated", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
            if (picWorking.Visible) return;
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (btnReset.Visible)
            {
                btnReset.PerformClick();
            }
            if (VariousFunctions.ReadFileType(files[0]) == XboxFileType.STFS)
            {
                txtFolder.Text = Path.GetDirectoryName(files[0]);
                Tools.CurrentFolder = txtFolder.Text;
            }
            else switch (Path.GetExtension(files[0]).ToLowerInvariant())
            {
                case ".mogg":
                    if (MessageBox.Show("Do you want to encrypt mogg file '" + Path.GetFileName(files[0]) + "'?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        EncryptMogg(files[0]);
                    }
                    break;
                case ".mid":
                    if (MessageBox.Show("Do you want to encrypt MIDI file '" + Path.GetFileName(files[0]) + "'?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        EncryptMIDI(files[0]);
                    }
                    break;
                case ".pkg":
                    var pkgFiles = new List<string>();
                    foreach (var file in files.Where(pkg => Path.GetExtension(pkg) == ".pkg"))
                    {
                        pkgFiles.Add(file);
                    }
                    ExtractPKGFiles(pkgFiles, Path.GetDirectoryName(files[0]));
                    break;
                default:
                    MessageBox.Show("That's not a valid file to drop here", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    break;
            }
        }
        
        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This converter is permanently disabled. Use Onyx.", "Deprecated", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
            var message = Tools.ReadHelpFile("ps3");
            var help = new HelpForm(Text + " - Help", message);
            help.ShowDialog();
        }

        private void exportLogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.ExportLog(Text, lstLog.Items);
        }
        
        private void btnReset_Click(object sender, EventArgs e)
        {
            Log("Resetting...");
            inputFiles.Clear();
            btnReset.Visible = false;
            btnBegin.Visible = true;
            btnBegin.Enabled = true;
            EnableDisable(true);
            SongName = "";
            SongArtist = "";
            internalName = "";
            btnRefresh.PerformClick();
        }

        private void EnableDisable(bool enabled)
        {
            btnFolder.Enabled = enabled;
            btnRefresh.Enabled = enabled;
            picWorking.Visible = !enabled;
            menuStrip1.Enabled = enabled;
            chkRAR.Enabled = enabled;
            chkMerge.Enabled = enabled;
            chkSongID.Enabled = enabled;
            txtFolder.Enabled = enabled;
            lstLog.Cursor = enabled ? Cursors.Default : Cursors.WaitCursor;
            Cursor = lstLog.Cursor;
            btnBegin.Visible = true;
            btnBegin.Enabled = true;
            btnOnyx.Enabled = enabled;
            picOnyx.Enabled = enabled;
            if (!enabled)
            {
                btnBegin.Text = "Cancel";
                toolTip1.SetToolTip(btnBegin, "Click here to cancel");
            }
            else
            {
                btnBegin.Text = "Begin";
                toolTip1.SetToolTip(btnBegin, "Click here to begin");
            }
        }
                
        private void btnBegin_Click(object sender, EventArgs e)
        {       
            if (btnBegin.Text == "Cancel")
            {
                backgroundWorker1.CancelAsync();
                backgroundWorker2.CancelAsync();
                backgroundWorker3.CancelAsync();
                Log("User cancelled process...stopping as soon as possible");
                btnBegin.Enabled = false;
                return;
            }
            startTime = DateTime.Now;
            EnableDisable(false);
            Tools.CurrentFolder = txtFolder.Text;
            try
            {
                var files = Directory.GetFiles(txtFolder.Text);
                if (files.Count() != 0)
                {
                    btnBegin.Text = "Cancel";
                    toolTip1.SetToolTip(btnBegin, "Click to cancel process");
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
        
        private void HandleDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
        }
        
        private void PS3Prep_Resize(object sender, EventArgs e)
        {
            btnRefresh.Left = txtFolder.Left + txtFolder.Width - btnRefresh.Width;
            btnBegin.Left = txtFolder.Left + txtFolder.Width - btnBegin.Width;
            picWorking.Left = (Width / 2) - (picWorking.Width / 2);
        }
        
        private void PS3Prep_Shown(object sender, EventArgs e)
        {            
            Log("Welcome to " + Text);
            Log("");
            Log(Text + " is now deprecated and is only left as part of Nautilus for legacy purposes and a few useful tools");
            Log("The tool suite Onyx Music Game Toolkit does a significantly much better job at creating PS3 customs and it is");
            Log("highly recommended that you use that tool instead");
            Log("Click the Onyx button to visit the Onyx Music Game Toolkit Github repository");
            Log("");
            Log("Drag and drop the CON / LIVE file(s) to be converted here");
            Log("Or click 'Change Input Folder' to select the files");
            Log("Ready to begin");
            txtFolder.Text = CONFolder;            
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            Log("Done!");
            endTime = DateTime.Now;
            var timeDiff = endTime - startTime;
            Log("Process took " + timeDiff.Minutes + (timeDiff.Minutes == 1 ? " minute" : " minutes") + " and " + (timeDiff.Minutes == 0 && timeDiff.Seconds == 0 ? "1 second" : timeDiff.Seconds + " seconds"));
            Log("Click 'Reset' to start again or just close me down");
            //clear up any leftover png_xbox files
            var xbox_files = Directory.GetFiles(txtFolder.Text, "*.png_xbox", SearchOption.AllDirectories);
            foreach (var xbox_file in xbox_files)
            {
                Tools.DeleteFile(xbox_file);
            }
            picWorking.Visible = false;
            btnReset.Visible = true;
            btnReset.Enabled = true;
            lstLog.Cursor = Cursors.Default;
            Cursor = lstLog.Cursor;
            toolTip1.SetToolTip(btnBegin, "Click to begin");
            btnBegin.Text = "&Begin";
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            isLinosSpecial = false;
            if (ProcessFiles()) return;
            Log("There was an error processing the files ... stopping here");
        }

        private void chkMerge_CheckedChanged(object sender, EventArgs e)
        {
            chkRAR.Enabled = !chkMerge.Checked || !chkMerge.Enabled;
        }

        private void mergeSongsToolStrip_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will merge songs that have already been converted to PS3 format with your existing songs and combine their songs.dta files\n\nTo start, copy all the songs you want to merge to the 'Songs to Merge' folder\n\nPress OK when you're ready to start", "Merge Songs Tool", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) != DialogResult.OK)
            {
                return;
            }
            Log("Merging songs...");
            try
            {
                var counter = 0;
                var DTAs = Directory.GetFiles(ToMergeFolder, "songs.dta", SearchOption.AllDirectories);
                if (!DTAs.Any())
                {
                    Log("No songs found in 'Songs to Merge' directory");
                    return;
                }
                var folders = DTAs.Select(Path.GetDirectoryName).ToList();
                if (!folders.Any())
                {
                    Log("No songs found in 'Songs to Merge' directory");
                    return;
                }
                foreach (var song in folders)
                {
                    var skip = false;
                    var dirs = Directory.GetDirectories(song);
                    foreach (var dir in dirs)
                    {
                        var newdir = MergedSongsFolder + dir.Replace(song, "") + "\\";
                        if (Directory.Exists(newdir))
                        {
                            Log("Song folder '" + dir.Replace(song + "\\", "") + "' already exists in 'Merged Songs' folder, skipping...");
                            skip = true;
                            continue;
                        }
                        Directory.CreateDirectory(newdir + "gen\\");
                        var files = Directory.GetFiles(dir);
                        foreach (var file in files)
                        {
                            Tools.MoveFile(file, newdir + Path.GetFileName(file));
                        }
                        files = Directory.GetFiles(dir + "\\gen\\");
                        foreach (var file in files)
                        {
                            Tools.MoveFile(file, newdir + "gen\\" + Path.GetFileName(file));
                        }
                    }
                    if (skip) continue;
                    var sr = new StreamReader(song + "\\songs.dta");
                    var sw = new StreamWriter(MergeDTA, true);
                    sw.Write(sr.ReadToEnd());
                    sr.Dispose();
                    sw.Dispose();
                    counter++;
                    Tools.DeleteFolder(song, true);
                }
                Log(counter == 0 ? "No songs merged" : "Merged " + counter + " " + (counter > 1 ? "songs" : "song") + " successfully");
                if (!chkSongID.Checked) return;
                Log("A total of " + counter + " song IDs were merged into the existing DTA file");
                Log("Beginning batch replacing of song IDs with unique numeric values");
                doIDReplace(MergeDTA, true);
            }
            catch (Exception ex)
            {
                Log("Failed to merge songs");
                Log("Error says: " + ex.Message);
            }
        }

        private void managePackDTAFile_Click(object sender, EventArgs e)
        {
            Log("Sending songs.dta file to Quick Pack Editor...");
            var newQuickPack = new QuickPackEditor(xMainForm, Color.FromArgb(200, 90, 195, 73), Color.White, MergeDTA);
            newQuickPack.ShowDialog();
            if (!File.Exists(MergedSongsFolder + "deleted.txt"))
            {
                Log("Quick Pack Editor closed: no songs were removed from the songs.dta file");
                return;
            }
            var deleted = new List<string>();
            var sr = new StreamReader(MergedSongsFolder + "deleted.txt");
            while (sr.Peek() > -1)
            {
                deleted.Add(Tools.GetConfigString(sr.ReadLine()));
            }
            sr.Dispose();
            Tools.DeleteFile(MergedSongsFolder + "deleted.txt");
            Log("Quick Pack Editor closed: " + deleted.Count + (deleted.Count > 1 ? " songs were" : " song was") + " removed from the songs.dta file");
            var message = "The following " + (deleted.Count > 1 ? "songs were" : "song was") + " removed from the songs.dta file:\n";
            message = deleted.Aggregate(message, (current, line) => current + "\n" + line);
            message = message + "\n\nDo you want to remove the corresponding song " + (deleted.Count > 1 ? "folders" : "folder") + "?";
            if (MessageBox.Show(message, Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                Log("No song folders deleted");
                return;
            }
            foreach (var folder in from line in deleted let index = line.IndexOf("::", StringComparison.Ordinal) select line.Substring(index + 2, line.Length - index - 2).Trim())
            {
                Log("Sending folder '" + folder + "' from 'Merged Songs' directory to Recycle Bin");
                Tools.SendtoTrash(MergedSongsFolder + folder, true);
            }
            Log("Deleted " + deleted.Count + " song " + (deleted.Count > 1 ? "folders" : "folder"));
        }        

        private void PS3Converter_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!picWorking.Visible)
            {
                SaveConfig();
                return;
            }
            MessageBox.Show("Please wait until the current process finishes", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            e.Cancel = true;
        }

        private void type1defaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            type1.Checked = true;
            type2.Checked = false;
            type3.Checked = false;
        }

        private void type2_Click(object sender, EventArgs e)
        {
            type1.Checked = false;
            type2.Checked = true;
            type3.Checked = false;
        }

        private void wait2Seconds_Click(object sender, EventArgs e)
        {
            wait2Seconds.Checked = true;
            wait5Seconds.Checked = false;
            wait10Seconds.Checked = false;
        }

        private void wait5Seconds_Click(object sender, EventArgs e)
        {
            wait2Seconds.Checked = false;
            wait5Seconds.Checked = true;
            wait10Seconds.Checked = false;
        }

        private void wait10Seconds_Click(object sender, EventArgs e)
        {
            wait2Seconds.Checked = false;
            wait5Seconds.Checked = false;
            wait10Seconds.Checked = true;
        }

        private void encryptReplacementMogg_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "MOGG Audio File (*.mogg)|*.mogg",
                InitialDirectory = Environment.CurrentDirectory,
                Title = "Select MOGG file to encrypt",
                Multiselect = false
            };
            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            EncryptMogg(ofd.FileName);
        }

        private void EncryptMogg(string mogg)
        {
            CryptVersion version;
            using (var fs = File.OpenRead(mogg))
            {
                using (var br = new BinaryReader(fs))
                {
                    version = (CryptVersion)br.ReadInt32();
                }
            }
            //byte[] bytes = null;
            var mData = File.ReadAllBytes(mogg);
            if (version != CryptVersion.x0A)
            {
                Log("Mogg file '" + Path.GetFileName(mogg) + "' is already encrypted");
                return;
            }
            if (nautilus3.EncM(File.ReadAllBytes(mogg), mogg))
            {
                Log("Mogg file '" + Path.GetFileName(mogg) + "' was encrypted successfully");
            }
            else
            {
                Log("Failed to encrypt mogg file '" + Path.GetFileName(mogg) + "'");
            }
        }

        private void encryptReplacementMIDI_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "MIDI File (*.mid)|*.mid",
                InitialDirectory = Environment.CurrentDirectory,
                Title = "Select MIDI file(s) to encrypt",
                Multiselect = true
            };
            if (ofd.ShowDialog() != DialogResult.OK) return;
            foreach (var file in ofd.FileNames)
            {
                EncryptMIDI(file, false);
            }
        }

        private void EncryptMIDI(string file, bool message = true)
        {
            //need to make sure file path is lowercase for PS3
            var temp = Path.GetDirectoryName(file) + "\\" + Path.GetFileNameWithoutExtension(file) +"_temp.mid";
            Tools.DeleteFile(temp);
            Tools.MoveFile(file, temp);
            var midi = Path.GetDirectoryName(file) + "\\" + Path.GetFileName(file).ToLowerInvariant().Replace(" ", "").Replace("-", "").Replace("'", "").Replace("(", "").Replace(")", "");
            Tools.DeleteFile(midi);
            Tools.MoveFile(temp, midi);
            if (type1.Checked)
            {
                MakeEdat("edat1", midi);
            }
            else if (type2.Checked)
            {
                MakeEdat("edat2", midi);
            }
            else
            {
                MakeEdat("edat3", midi);
            }
            var edat = midi + ".edat";
            if (File.Exists(edat))
            {
                if (message)
                {
                    MessageBox.Show("Encrypted MIDI to EDAT successfully\nNew file can be found at:\n" + edat, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                Log("Encrypted MIDI '" + Path.GetFileName(file) + "' to EDAT '" + Path.GetFileName(edat) + "' successfully");
            }
            else
            {
                if (message)
                {
                    MessageBox.Show("Failed to encrypt MIDI file '" + Path.GetFileName(file) + "'", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                Log("Failed to encrypt MIDI file '" + Path.GetFileName(file) + "'");
            }
        }
        
        private void batchChangeIDsToNumeric_Click(object sender, EventArgs e)
        {            
            var ofd = new OpenFileDialog
            {
                InitialDirectory = MergedSongsFolder,
                Title = "Select DTA file to edit",
                Multiselect = false,
                Filter = "DTA Files(*.dta)|*.dta"
            };
            if (ofd.ShowDialog() != DialogResult.OK) return;
            if (string.IsNullOrWhiteSpace(ofd.FileName)) return;
            doIDReplace(ofd.FileName);
        }
        
        private bool doIDReplace(string dta, bool hide_message = false)
        {
            var counter = 0;
            try
            {
                var sr = new StreamReader(dta);
                var lines = new List<string>();
                while (sr.Peek() > 0)
                {
                    lines.Add(sr.ReadLine());
                }
                sr.Dispose();
                var sw = new StreamWriter(dta, false);
                foreach (var line in lines)
                {
                    var newline = line;
                    if (line.Contains("song_id") && !line.Contains(";ORIG_ID=") && !line.TrimStart().StartsWith(";"))
                    {
                        var origID = Parser.GetSongID(line);
                        if (!Parser.IsNumericID(line) || (Parser.GetSongID(line).StartsWith("1000") && Parser.GetSongID(line).Length == 10)) //only if not already a numeric ID or it has the default 1000 MAGMA C3 Author ID
                        {                            
                            newline = ";ORIG_ID=" + origID;
                            sw.WriteLine(newline);
                            var corrector = new SongIDCorrector();
                            var newID = corrector.ShortnameToSongID(Parser.Songs[0].ShortName);
                            newline = "   ('song_id' " + newID + ")";
                            Log($"Song has an incorrect ID: {origID} - replacing with a correct ID: {newID}");
                            counter++;                            
                        }
                        else
                        {
                            Log($"This song already has a correct ID: {origID} - leaving the song ID alone");
                        }
                    }
                    if (newline.Trim() != "")
                    {
                        sw.WriteLine(newline);
                    }
                }
                sw.Dispose();
                if (!hide_message)
                {
                    MessageBox.Show("Process completed without any errors\nReplaced IDs for " + counter + (counter == 1 ? " song" : " songs") + "\n\nOnly custom songs without numeric IDs and default author IDs were edited", Text, MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error editing that DTA file\nThe error says: " + ex.Message, Text,
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            if (counter == 0)
            {
                Log("No song IDs were replaced with unique numeric values");
                return false;
            }
            else
            {
                Log("Successfully replaced " + counter + " song IDs with unique numeric values");
                return true;
            }
        }      

        private void regionNTSC_Click(object sender, EventArgs e)
        {
            regionNTSC.Checked = true;
            regionPAL.Checked = false;
        }

        private void regionPAL_Click(object sender, EventArgs e)
        {
            regionNTSC.Checked = false;
            regionPAL.Checked = true;
        }

        private void type3_Click(object sender, EventArgs e)
        {
            type1.Checked = false;
            type2.Checked = false;
            type3.Checked = true;
        }

        private void AddAudioToMixer(string audioFile, int audioChannels)
        {
            BassStream = Bass.BASS_StreamCreateFile(audioFile, 0L, File.ReadAllBytes(audioFile).Length, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);
            var stream_info = Bass.BASS_ChannelGetInfo(BassStream);
            if (stream_info.chans == 0) return;
            BassMix.BASS_Mixer_StreamAddChannel(BassMixer, BassStream, BASSFlag.BASS_MIXER_MATRIX);
            var matrix = GetMatrix(currentChannel, stream_info.chans, audioChannels);
            BassMix.BASS_Mixer_ChannelSetMatrix(BassStream, matrix);
            currentChannel += stream_info.chans;
        }

        private float[,] GetMatrix(int currentChannel, int channelCount, int audioChannels)
        {
            var matrix = new float[audioChannels, channelCount];
            var vol = (float)1.0;
            if (channelCount == 1)
            {
                matrix[currentChannel, 0] = vol;
            }
            else
            {
                matrix[currentChannel, 0] = vol;
                matrix[currentChannel + 1, 1] = vol;
            }
            return matrix;
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

        private void downmixAndEncode_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This tool will accept any number of Xbox 360 CON files, will extract the mogg file, will downmix the drum tracks in the mogg file to stereo to ensure you have a maximum of 16 total channels, will reencode the mogg file using quality 3 to remove stuttering and other issues in game, will modify the songs.dta file to reflect the changes to the mogg file, will modify the MIDI file to reflect the correct drum mix events, and will repackage the CON files for you. You then should feed these edited CON files to Onyx Music Game Toolkit to create PKG files that you can play on your PS3. Good luck!", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            var ofd = new FolderPicker
            {
                Title = "Select the folder where your CON files are",
                InputPath = Environment.CurrentDirectory,
            };            
            if (ofd.ShowDialog(IntPtr.Zero) != true || string.IsNullOrEmpty(ofd.ResultPath)) return;
            fixFolder = ofd.ResultPath;

            conFiles = new List<string>();
            var inFiles = Directory.GetFiles(fixFolder);
            foreach (var file in inFiles)
            {
                if (VariousFunctions.ReadFileType(file) == XboxFileType.STFS)
                {
                    conFiles.Add(file);
                }
            }
            Log("");
            Log("Found " + conFiles.Count + " CON " + (conFiles.Count == 1 ? "file" : "files"));
            fixCounter = 0;
            fixSuccess = 0;

            EnableDisable(false);
            backgroundWorker3.RunWorkerAsync();             
        }

        private bool FixCONForPS3(string folderPath, string conFilePath)
        {
            this.internalName = "";
            SongName = "";
            SongArtist = "";
            bool editedMogg = false;

            try
            {
                if (VariousFunctions.ReadFileType(conFilePath) != XboxFileType.STFS) return false;
                uint TitleID;
                var con = new STFSPackage(conFilePath);
                if (con.ParseSuccess)
                {
                    TitleID = con.Header.TitleID;
                }
                else
                {
                    TitleID = 0;
                }
                con.CloseIO();
                try
                {                    
                    Parser.ExtractDTA(conFilePath);
                    Parser.ReadDTA(Parser.DTA);
                    if (Parser.Songs.Count > 1)
                    {
                        Log("File " + Path.GetFileName(conFilePath) + " is a pack, try dePACKing first, skipping...");
                        return false;
                    }
                    if (!Parser.Songs.Any())
                    {
                        Log("There was an error loading the songs.dta file, skipping this file...");
                        return false;
                    }
                    if (loadDTA(TitleID))
                    {
                        Log("Loaded songs.dta file for song '" + SongArtist + " - " + SongName + "' successfully");
                        var scenario = "";
                        if (Parser.Songs[0].ChannelsTotal <= 12)
                        {
                            scenario = "Scenario 1";
                        }
                        else if (Parser.Songs[0].ChannelsTotal > 12 & Parser.Songs[0].ChannelsTotal <= 16)
                        {
                            scenario = "Scenario 2";
                        }
                        else
                        {
                            scenario = "Scenario 3";
                        }
                        Log("Song has " + Parser.Songs[0].ChannelsTotal + " audio channels: " + scenario);
                    }
                    else
                    {
                        return false;
                    }
                }
                catch { }
            }
            catch { }
            //make a backup before editing the CON just in case
            var backup = conFilePath + "_backup";
            Tools.DeleteFile(backup);
            File.Copy(conFilePath, backup);
            var xCON = new STFSPackage(conFilePath);
            if (!xCON.ParseSuccess)
            {
                Log("Couldn't parse file " + Path.GetFileName(conFilePath) + " ... skipping");
                xCON.CloseIO();
                return false;
            }
            var internalName = Parser.Songs[0].InternalName;
            var xMogg = xCON.GetFile("songs/" + internalName + "/" + internalName + ".mogg");
            if (xMogg == null)
            {
                Log("Couldn't find mogg file inside " + Path.GetFileName(conFilePath) + " ... skipping");
                xCON.CloseIO();
                return false;
            }
            var mogg = folderPath + "\\" + internalName + ".mogg";
            Tools.DeleteFile(mogg);
            if (backgroundWorker1.CancellationPending || backgroundWorker2.CancellationPending)
            {
                xCON.CloseIO();
                return false;
            }
            xMogg.ExtractToFile(mogg);
            if (!File.Exists(mogg))
            {
                Log("Couldn't extract mogg file from " + Path.GetFileName(conFilePath) + " ... skipping");
                xCON.CloseIO();
                return false;
            }

            var xDTA = xCON.GetFile("songs/songs.dta");
            var xMIDI = xCON.GetFile("songs/" + internalName + "/" + internalName + ".mid");
            string dta = "";            
            string midi = "";

            if (xDTA == null)
            {
                Log("Couldn't find songs.dta file inside " + Path.GetFileName(conFilePath) + " ... skipping");
                xCON.CloseIO();
                return false;
            }
            dta = folderPath + "\\songs.dta";
            Tools.DeleteFile(dta);
            xDTA.ExtractToFile(dta);
            if (!File.Exists(dta))
            {
                Log("Couldn't extract songs.dta file from " + Path.GetFileName(conFilePath) + " ... skipping");
                xCON.CloseIO();
                Tools.DeleteFile(mogg);
                return false;
            }

            if (Parser.Songs[0].ChannelsTotal > 16 || !isLinosSpecial)
            {               
                if (xMIDI == null)
                {
                    Log("Couldn't find MIDI file inside " + Path.GetFileName(conFilePath) + " ... skipping");
                    xCON.CloseIO();
                    return false;
                }
                midi = folderPath + "\\" + internalName + ".mid";
                Tools.DeleteFile(midi);
                if (backgroundWorker1.CancellationPending || backgroundWorker2.CancellationPending)
                {
                    xCON.CloseIO();
                    return false;
                }
                xMIDI.ExtractToFile(midi);
                if (!File.Exists(midi))
                {
                    Log("Couldn't extract MIDI file from " + Path.GetFileName(conFilePath) + " ... skipping");
                    xCON.CloseIO();
                    return false;
                }

                if (Parser.Songs[0].ChannelsDrums > 2)
                {
                    Log("Song has " + Parser.Songs[0].ChannelsDrums + " drum channels, will try to downmix");
                }

                Log("Reading mogg file...");                
                var dec = nautilus3.DecM(File.ReadAllBytes(mogg), false, false, DecryptMode.ToMemory);
                if (!dec)
                {
                    Log("This mogg file is encrypted and I can't work with it");
                    xCON.CloseIO();
                    Tools.DeleteFile(dta);
                    Tools.DeleteFile(mogg);
                    Tools.DeleteFile(midi);
                    return false;
                }
                Log("Read mogg file successfully, will attempt to auto downmix");

                var channels = 0;
                if (Parser.Songs[0].ChannelsDrums > 0)
                {
                    channels = 2;
                }
                channels += Parser.Songs[0].ChannelsBass;
                channels += Parser.Songs[0].ChannelsGuitar;
                channels += Parser.Songs[0].ChannelsVocals;
                channels += Parser.Songs[0].ChannelsKeys;
                var backing = Parser.Songs[0].ChannelsTotal - Parser.Songs[0].ChannelsDrums - Parser.Songs[0].ChannelsBass - Parser.Songs[0].ChannelsGuitar - Parser.Songs[0].ChannelsVocals - Parser.Songs[0].ChannelsKeys - Parser.Songs[0].ChannelsCrowd;
                channels += backing;
                channels += Parser.Songs[0].ChannelsCrowd;

                Control.CheckForIllegalCrossThreadCalls = false;
                var splitter = new MoggSplitter();
                var downmixed = splitter.DoMoggDownmix(nautilus3, Parser, mogg, channels, false);
                if (downmixed)
                {
                    Log("Downmixed mogg file to ogg file successfully");
                }
                else
                {
                    Log("Failed to downmix mogg file to ogg file");
                    xCON.CloseIO();
                    Tools.DeleteFile(dta);
                    Tools.DeleteFile(mogg);
                    Tools.DeleteFile(midi);
                    return false;
                }                
                var ogg = mogg.Replace(".mogg", ".ogg");
                Log("Adding mogg header to ogg file...");
                if (Tools.MakeMogg(ogg, mogg))
                {
                    Log("Success");
                    Tools.DeleteFile(ogg);
                }
                else
                {
                    Log("Failed");
                    xCON.CloseIO();
                    Tools.DeleteFile(dta);
                    Tools.DeleteFile(mogg);
                    Tools.DeleteFile(midi);
                    return false;
                }
                Log("Encrypting mogg file (required for PS3 use)");
                if (nautilus3.EncM(File.ReadAllBytes(mogg), mogg))
                {
                    Log("Success");
                }
                else
                {
                    Log("Failed");
                    xCON.CloseIO();
                    Tools.DeleteFile(mogg);
                    Tools.DeleteFile(dta);
                    Tools.DeleteFile(midi);
                    return false;
                }
                Log("Downmixed mogg file from " + Parser.Songs[0].ChannelsTotal + " channels to " + channels + " channels");
                Log("Encoded using ogg quality 3");
                editedMogg = true;

                Log("Editing DTA file to reflect changes to mogg file");

                var drumsDiff = Parser.Songs[0].ChannelsDrums - 2;
                var pans = "-1.0 1.0 ";
                var vols = "";
                var cores = "";
                var track_count = "2 ";
                for (var i = 0; i < channels; i++)
                {
                    vols += "0.0 ";
                    cores += "-1 ";
                }
                if (Parser.Songs[0].ChannelsBass > 0)
                {
                    pans += Parser.Songs[0].ChannelsBass == 1 ? "0.0 " : "-1.0 1.0 ";
                }
                track_count += Parser.Songs[0].ChannelsBass.ToString() + " ";
                if (Parser.Songs[0].ChannelsGuitar > 0)
                {
                    pans += Parser.Songs[0].ChannelsGuitar == 1 ? "0.0 " : "-1.0 1.0 ";
                }
                track_count += Parser.Songs[0].ChannelsGuitar.ToString() + " ";
                if (Parser.Songs[0].ChannelsVocals > 0)
                {
                    pans += Parser.Songs[0].ChannelsVocals == 1 ? "0.0 " : "-1.0 1.0 ";
                }
                track_count += Parser.Songs[0].ChannelsVocals.ToString() + " ";
                if (Parser.Songs[0].ChannelsKeys > 0)
                {
                    pans += Parser.Songs[0].ChannelsKeys == 1 ? "0.0 " : "-1.0 1.0 ";
                }
                track_count += Parser.Songs[0].ChannelsKeys.ToString() + " ";
                if (backing > 0)
                {
                    pans += backing == 1 ? "0.0 " : "-1.0 1.0 ";
                }
                track_count += backing.ToString() + " ";
                if (Parser.Songs[0].ChannelsCrowd > 0)
                {
                    pans += Parser.Songs[0].ChannelsCrowd == 1 ? "0.0 " : "-1.0 1.0 ";
                    track_count += Parser.Songs[0].ChannelsCrowd.ToString() + " ";
                }
                var didDrums = false;
                var didBass = false;
                var didGuitar = false;
                var didVocals = false;
                var didKeys = false;
                var pastRank = false;
                var finalDTA = dta.Replace("songs.dta", "_final_songs.dta");
                var encoding = Parser.DetectEncoding(dta);
                var sr = new StreamReader(dta, encoding);
                var sw = new StreamWriter(finalDTA, false, new UTF8Encoding(false, false));
                try
                {
                    while (sr.Peek() >= 0)
                    {
                        var line = sr.ReadLine();
                        if (line.Contains("'rank'"))
                        {
                            pastRank = true;
                        }
                        if (line.Contains("crowd_channels"))
                        {
                            line = "     (crowd_channels " + (Parser.Songs[0].ChannelsCrowd == 1 ? (Parser.Songs[0].ChannelsCrowdStart - drumsDiff).ToString() : (Parser.Songs[0].ChannelsCrowdStart - drumsDiff).ToString() + " " + (Parser.Songs[0].ChannelsCrowdStart - drumsDiff + 1).ToString()) + ")";
                        }
                        if (line.Contains("'tracks_count'"))
                        {
                            sw.WriteLine(line);
                            sr.ReadLine();
                            sw.WriteLine("         (" + track_count.TrimEnd() + ")");
                            continue;
                        }
                        if (line.Contains("pans") && !line.Contains("'pans'"))
                        {
                            line = "     (pans        (" + pans.TrimEnd() + "))";
                        }
                        else if (line.Contains("'pans'"))
                        {
                            sw.WriteLine(line);
                            sr.ReadLine();
                            sw.WriteLine("         (" + pans.TrimEnd() + ")");
                            continue;
                        }                        
                        if (line.Contains("vols") && !line.Contains("'vols'"))
                        {
                            line = "     (vols        (" + vols.TrimEnd() + "))";
                        }
                        else if (line.Contains("'vols'"))
                        {
                            sw.WriteLine(line);
                            sr.ReadLine();
                            sw.WriteLine("         (" + vols.TrimEnd() + ")");
                            continue;
                        }
                        if (line.Contains("cores") && !line.Contains("'cores'"))
                        {
                            line = "     (cores        (" + cores.TrimEnd() + "))";
                        }
                        else if (line.Contains("'cores'"))
                        {
                            sw.WriteLine(line);
                            sr.ReadLine();
                            sw.WriteLine("         (" + cores.TrimEnd() + ")");
                            continue;
                        }
                        if (line.Contains("(drum") && !didDrums && !pastRank)
                        {
                            line = "         ((drum (0 1))";
                            didDrums = true;
                        }
                        else if (line.Contains("'drum'") && !didDrums && !pastRank)
                        {
                            sw.WriteLine(line);
                            sr.ReadLine();
                            line = "               (0 1)";
                            didDrums = true;
                        }
                        if (line.Contains("(bass") && !didBass && !pastRank)
                        {
                            line = "         (bass " + (Parser.Songs[0].ChannelsBass == 1 ? (Parser.Songs[0].ChannelsBassStart - drumsDiff).ToString() : (Parser.Songs[0].ChannelsBassStart - drumsDiff).ToString() + " " + (Parser.Songs[0].ChannelsBassStart - drumsDiff + 1).ToString()) + ")";
                            didBass = true;
                        }
                        else if (line.Contains("'bass'") && !didBass && !pastRank)
                        {
                            if (!line.Contains(")"))
                            {
                                sw.WriteLine(line);
                                sr.ReadLine();
                                line = "               (" + (Parser.Songs[0].ChannelsBass == 1 ? (Parser.Songs[0].ChannelsBassStart - drumsDiff).ToString() : (Parser.Songs[0].ChannelsBassStart - drumsDiff).ToString() + " " + (Parser.Songs[0].ChannelsBassStart - drumsDiff + 1).ToString()) + ")";
                            }
                            else
                            {
                                line = "               ('bass' " + (Parser.Songs[0].ChannelsBass == 1 ? (Parser.Songs[0].ChannelsBassStart - drumsDiff).ToString() : (Parser.Songs[0].ChannelsBassStart - drumsDiff).ToString() + " " + (Parser.Songs[0].ChannelsBassStart - drumsDiff + 1).ToString()) + ")";
                            }
                            didBass = true;
                        }
                        if (line.Contains("(guitar") && !didGuitar && !pastRank)
                        {
                            line = "         (guitar " + (Parser.Songs[0].ChannelsGuitar == 1 ? (Parser.Songs[0].ChannelsGuitarStart - drumsDiff).ToString() : (Parser.Songs[0].ChannelsGuitarStart - drumsDiff).ToString() + " " + (Parser.Songs[0].ChannelsGuitarStart - drumsDiff + 1).ToString()) + ")";
                            didGuitar = true;
                        }
                        else if (line.Contains("'guitar'") && !didGuitar && !pastRank)
                        {
                            if (!line.Contains(")"))
                            {
                                sw.WriteLine(line);
                                sr.ReadLine();
                                line = "               (" + (Parser.Songs[0].ChannelsGuitar == 1 ? (Parser.Songs[0].ChannelsGuitarStart - drumsDiff).ToString() : (Parser.Songs[0].ChannelsGuitarStart - drumsDiff).ToString() + " " + (Parser.Songs[0].ChannelsGuitarStart - drumsDiff + 1).ToString()) + ")";
                            }
                            else
                            {
                                line = "               ('guitar' " + (Parser.Songs[0].ChannelsGuitar == 1 ? (Parser.Songs[0].ChannelsGuitarStart - drumsDiff).ToString() : (Parser.Songs[0].ChannelsGuitarStart - drumsDiff).ToString() + " " + (Parser.Songs[0].ChannelsGuitarStart - drumsDiff + 1).ToString()) + ")";

                            }
                            didGuitar = true;
                        }
                        if (line.Contains("(vocals") && !didVocals && !pastRank)
                        {
                            line = "         (vocals " + (Parser.Songs[0].ChannelsVocals == 1 ? (Parser.Songs[0].ChannelsVocalsStart - drumsDiff).ToString() : (Parser.Songs[0].ChannelsVocalsStart - drumsDiff).ToString() + " " + (Parser.Songs[0].ChannelsVocalsStart - drumsDiff + 1).ToString()) + ")";
                            didVocals = true;
                        }
                        else if (line.Contains("'vocals'") && !didVocals && !pastRank)
                        {
                            if (!line.Contains(")"))
                            {
                                sw.WriteLine(line);
                                sr.ReadLine();
                                line = "               (" + (Parser.Songs[0].ChannelsVocals == 1 ? (Parser.Songs[0].ChannelsVocalsStart - drumsDiff).ToString() : (Parser.Songs[0].ChannelsVocalsStart - drumsDiff).ToString() + " " + (Parser.Songs[0].ChannelsVocalsStart - drumsDiff + 1).ToString()) + ")";
                            }
                            else
                            {
                                line = "               ('vocals' " + (Parser.Songs[0].ChannelsVocals == 1 ? (Parser.Songs[0].ChannelsVocalsStart - drumsDiff).ToString() : (Parser.Songs[0].ChannelsVocalsStart - drumsDiff).ToString() + " " + (Parser.Songs[0].ChannelsVocalsStart - drumsDiff + 1).ToString()) + ")";

                            }
                            didVocals = true;
                        }
                        if (line.Contains("(keys") && !didKeys && !pastRank)
                        {
                            line = "         (keys " + (Parser.Songs[0].ChannelsKeys == 1 ? (Parser.Songs[0].ChannelsKeysStart - drumsDiff).ToString() : (Parser.Songs[0].ChannelsKeysStart - drumsDiff).ToString() + " " + (Parser.Songs[0].ChannelsKeysStart - drumsDiff + 1).ToString()) + ")";
                            didKeys = true;
                        }
                        else if (line.Contains("'keys'") && !didKeys && !pastRank)
                        {
                            if (!line.Contains(")"))
                            {
                                sw.WriteLine(line);
                                sr.ReadLine();
                                line = "               (" + (Parser.Songs[0].ChannelsKeys == 1 ? (Parser.Songs[0].ChannelsKeysStart - drumsDiff).ToString() : (Parser.Songs[0].ChannelsKeysStart - drumsDiff).ToString() + " " + (Parser.Songs[0].ChannelsKeysStart - drumsDiff + 1).ToString()) + ")";
                            }
                            else
                            {
                                line = "               ('keys' " + (Parser.Songs[0].ChannelsKeys == 1 ? (Parser.Songs[0].ChannelsKeysStart - drumsDiff).ToString() : (Parser.Songs[0].ChannelsKeysStart - drumsDiff).ToString() + " " + (Parser.Songs[0].ChannelsKeysStart - drumsDiff + 1).ToString()) + ")";
                            }
                            didKeys = true;
                        }
                        sw.WriteLine(line);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error processing DTA file:\n\n" + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                sr.Dispose();
                sw.Dispose();
                Tools.MoveFile(finalDTA, dta);
                Log("Done");

                Log("Editing MIDI file to reflect changes to drum tracks");
                var mid = Tools.NemoLoadMIDI(midi);
                if (mid == null)
                {
                    Log("Failed to read the MIDI file, skipping this file...");
                    Tools.DeleteFile(mogg);
                    Tools.DeleteFile(dta);
                    Tools.DeleteFile(midi);
                    xCON.CloseIO();
                    return false;
                }

                var to_remove = new List<MidiEvent>();
                var to_add = new List<MidiEvent>();

                for (var i = 0; i < mid.Events.Tracks; i++)
                {
                    var track = mid.Events[i][0].ToString();
                    if (!track.Contains("PART DRUMS")) continue;

                    foreach (var drum_events in mid.Events[i])
                    {
                        if (drum_events.CommandCode == MidiCommandCode.MetaEvent && drum_events.ToString().Contains("[mix") && drum_events.ToString().Contains("drums"))
                        {
                            var drum_event = (TextEvent)drum_events;
                            var disco = "";
                            if (drum_event.Text.Contains("d]"))//disco flip
                            {
                                disco = "d";
                            }
                            if (drum_event.Text.Contains("[mix 0 drums"))
                            {
                                to_add.Add(new TextEvent("[mix 0 drums0" + disco + "]", MetaEventType.TextEvent, drum_events.AbsoluteTime));
                            }
                            else if (drum_event.Text.Contains("[mix 1 drums"))
                            {
                                to_add.Add(new TextEvent("[mix 1 drums0" + disco + "]", MetaEventType.TextEvent, drum_events.AbsoluteTime));
                            }
                            else if (drum_event.Text.Contains("[mix 2 drums"))
                            {
                                to_add.Add(new TextEvent("[mix 2 drums0" + disco + "]", MetaEventType.TextEvent, drum_events.AbsoluteTime));
                            }
                            else if (drum_event.Text.Contains("[mix 3 drums"))
                            {
                                to_add.Add(new TextEvent("[mix 3 drums0" + disco + "]", MetaEventType.TextEvent, drum_events.AbsoluteTime));
                            }
                            to_remove.Add(drum_events);
                        }
                    }
                    foreach (var remove in to_remove)
                    {
                        mid.Events[i].Remove(remove);
                    }
                    foreach (var add in to_add)
                    {
                        mid.Events[i].Add(add);
                    }
                }
                try
                {
                    MidiFile.Export(midi, mid.Events);
                }
                catch (Exception ex)
                {
                    Log("Error editing MIDI file:");
                    Log(ex.Message);
                    Log("Skipping this file...");
                    Tools.DeleteFile(mogg);
                    Tools.DeleteFile(dta);
                    Tools.DeleteFile(midi);
                    xCON.CloseIO();
                    return false;
                }
                Log("Done");
            }
            else if ((Parser.Songs[0].ChannelsTotal > 12 & Parser.Songs[0].ChannelsTotal <= 16) && isLinosSpecial)
            {
                Log("Will attempt to re-encode mogg file using quality 3");
                if (!Splitter.ReEncodeMogg(Parser.Songs[0], mogg, 3))
                {
                    Log("Failed to re-encode mogg file, skipping this file...");
                    Tools.DeleteFile(mogg);
                    Tools.DeleteFile(dta);
                    Tools.DeleteFile(midi);
                    xCON.CloseIO();
                    return false;
                }
                Log("Mogg file re-encoded successfully");
                editedMogg = true;
            }
            else if (Parser.Songs[0].ChannelsTotal <= 12 && isLinosSpecial)
            {
                if (!CheckApplyPS3MoggPatch(mogg, true))
                {
                    Log("Not applying PS3 Mogg Patch...");
                    editedMogg = false;
                    fixIgnore++;
                }
                else
                {
                    editedMogg = true;
                }
            }

            doIDReplace(dta, true);

            if (editedMogg)
            {
                Log("Repackaging CON file...");

                if (xMogg.Replace(mogg))
                {
                    Log("Repackaged mogg file successfully");
                    Tools.DeleteFile(mogg);
                }
                else
                {
                    Log("Failed to repackage mogg file, skipping this file...");
                    Tools.DeleteFile(mogg);
                    Tools.DeleteFile(dta);
                    Tools.DeleteFile(midi);
                    xCON.CloseIO();
                    return false;
                }
            }
                        
            if (xDTA.Replace(dta))
            {
                Log("Repackaged songs.dta file successfully");
                Tools.DeleteFile(dta);
            }
            else
            {
                Log("Failed to repackage songs.dta file, skipping this file...");
                Tools.DeleteFile(dta);
                Tools.DeleteFile(midi);
                xCON.CloseIO();
                return false;
            }

            if (Parser.Songs[0].ChannelsTotal > 16 || !isLinosSpecial)
            {             
                if (xMIDI.Replace(midi))
                {
                    Log("Repackaged MIDI file successfully");
                    Tools.DeleteFile(midi);
                }
                else
                {
                    Log("Failed to repackage MIDI file, skipping this file...");
                    Tools.DeleteFile(midi);
                    xCON.CloseIO();
                    return false;
                }
            }

            Log("Finished repackaging CON file, signing...");
            xCON.Header.ThisType = PackageType.SavedGame;
            xCON.Header.MakeAnonymous();
            var signature = new RSAParams(Application.StartupPath + "\\bin\\KV.bin");
            xCON.RebuildPackage(signature);
            xCON.FlushPackage(signature);
            xCON.CloseIO();
            Tools.DeleteFile(mogg);
            Tools.DeleteFile(dta);
            Tools.DeleteFile(midi);
            var bOK = Tools.UnlockCON(conFilePath);
            if (bOK)
            {
                bOK = Tools.SignCON(conFilePath);
            }
            if (bOK)
            {
                Log("Success");
                Tools.DeleteFile(backup);
                return true;
            }
            else
            {
                Log("Failed");
                Log("Backup file can be found at: " + backup);
                Tools.DeleteFile(conFilePath);
                return false ;
            }       
    }
        private void pkgToCON_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This will batch decrypt, extract and convert PS3 PKG files to usable Xbox 360 CON files for use with Rock Band 3", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            var ofd = new FolderPicker
            {
                InputPath = Environment.CurrentDirectory,
                Title = "Select the folder where your PKG files are",
            };            
            if (ofd.ShowDialog(IntPtr.Zero) != true || string.IsNullOrEmpty(ofd.ResultPath)) return;
            var folder = ofd.ResultPath;

            var pkgFiles = new List<string>();
            var inFiles = Directory.GetFiles(folder);
            foreach (var file in inFiles.Where(pkg => Path.GetExtension(pkg) == ".pkg"))
            {
                pkgFiles.Add(file);
            }
            ExtractPKGFiles(pkgFiles, folder);
        }

        private void ExtractPKGFiles(List<string> pkgFiles, string folder)
        {
            Log("");
            Log("Found " + pkgFiles.Count + " PKG files");
            var counter = 0;
            var success = 0;

            List<string> folders = new List<string>();
            List<string> klics = new List<string>();

            foreach (var pkg in pkgFiles)
            {
                var shortPath = Path.GetFileNameWithoutExtension(pkg).Replace(" ", "").Replace("-", "").Replace("_", "").Trim();
                if (shortPath.Length > 10) //to avoid very lengthy PKG file names, which can cause issues with long paths on Windows
                {
                    shortPath = shortPath.Substring(0, 10) + "_ex";
                }
                var outFolder = folder + shortPath;
                Tools.DeleteFolder(outFolder, true);
                Log("Extracting PKG file '" + Path.GetFileName(pkg) + "'");
                counter++;
                if (!Tools.ExtractPS3PKG(pkg, outFolder))
                {
                    Log("Failed");
                    continue;
                }
                Log("Success");
                success++;
                folders.Add(outFolder);
            }
            Log("Extracted " + success + " PKG files out of " + counter + " files attempted");
            ProcessPS3Folders(folders, klics, true, true);
        }

        private void ProcessPS3Folders(List<string> folders, List<string> klics = null, bool isPKG = false, bool cleanUp = false)
        {
            var counter = 0;
            var success = 0;

            foreach (var outFolder in folders)
            {
                counter++;
                Log("Converting PKG file #" + counter);
                var DTA = Directory.GetFiles(outFolder, "songs.dta", SearchOption.AllDirectories);
                if (DTA.Count() == 0)
                {
                    Log("No songs.dta file found, skipping...");
                    continue;
                }
                Log("Found songs.dta file");
                Parser.ReadDTA(File.ReadAllBytes(DTA[0]));
                for (var i = 0; i < Parser.Songs.Count; i++)
                {                    
                    var newDTA = DTA[0].Replace(".dta", i + ".dta");
                    if (Parser.Songs.Count > 0)
                    {                        
                        var sw = new StreamWriter(newDTA, false);
                        foreach (var line in Parser.Songs[i].DTALines)
                        {
                            sw.WriteLine(line);
                        }
                        sw.Close();
                    }
                    Log("Song #" + (i + 1) + " is '" + Parser.Songs[i].Artist + " - " + Parser.Songs[i].Name + "'");
                    var internalName = Parser.Songs[i].InternalName;
                    var EDAT = Directory.GetFiles(outFolder, internalName + ".mid.edat", SearchOption.AllDirectories);
                    if (EDAT.Count() == 0)
                    {
                        Log("No .mid.edat file found, skipping...");
                        continue;
                    }
                    Log("Found .mid.edat file");
                    var MIDI = EDAT[0].Replace(".mid.edat", ".mid");
                    var PNG_PS3 = Directory.GetFiles(outFolder, internalName + "_keep.png_ps3", SearchOption.AllDirectories);
                    if (PNG_PS3.Count() == 0)
                    {
                        Log("No .png_ps3 file found, skipping...");
                        continue;
                    }
                    Log("Found .png_ps3 file");
                    if (!Tools.ConvertPS3toXbox(PNG_PS3[0], PNG_PS3[0], false))
                    {
                        Log("Failed to convert .png_ps3 to .png_xbox, skipping...");
                        continue;
                    }
                    var PNG_XBOX = PNG_PS3[0].Replace(".png_ps3", ".png_xbox");
                    if (File.Exists(PNG_XBOX))
                    {
                        Log("Converted .png_ps3 file to .png_xbox successfully");
                    }
                    var MILO_PS3 = Directory.GetFiles(outFolder, internalName + ".milo_ps3", SearchOption.AllDirectories);
                    if (MILO_PS3.Count() == 0)
                    {
                        Log("No .milo_ps3 file found, skipping...");
                        continue;
                    }
                    var MILO_XBOX = MILO_PS3[0].Replace(".milo_ps3", ".milo_xbox");
                    Tools.DeleteFile(MILO_XBOX);
                    File.Copy(MILO_PS3[0], MILO_XBOX);
                    if (File.Exists(MILO_XBOX))
                    {
                        Log("Renamed .milo_ps3 to .milo_xbox successfully");
                    }
                    var MOGG = Directory.GetFiles(outFolder, internalName + ".mogg", SearchOption.AllDirectories);
                    if (MOGG.Count() == 0)
                    {
                        Log("No .mogg file found, skipping...");
                        continue;
                    }
                    var wasAlreadyEncrypted = nautilus3.MoggIsEncrypted(File.ReadAllBytes(MOGG[0]));
                    if (wasAlreadyEncrypted)
                    {
                        Log("Mogg is PS3 encrypted...will add PS3 encrypted Mogg to CON");
                    }

                    Log("Decrypting EDAT file");
                    if (isPKG)
                    {
                        if (!Tools.DecryptCustomEdat(EDAT[0], MIDI, klics[counter - 1]))
                        {
                            Log("Failed to decrypt EDAT file...skipping");
                            continue;
                        }
                    }
                    else
                    {
                        if (!Tools.DecryptCustomEdat(EDAT[0], MIDI))
                        {
                            Log("Failed to decrypt EDAT file...skipping");
                            continue;
                        }
                    }

                    Log("Decrypted EDAT to MIDI successfully");
                    Log("Ready to build CON file");

                    var xCON = Path.GetDirectoryName(outFolder) + "\\" + internalName + "_rb3con";
                    Tools.DeleteFile(xCON);
                    var xSession = new CreateSTFS { HeaderData = { TitleID = 0x45410914 } };
                    xSession.HeaderData.Title_Package = "Rock Band 3";
                    xSession.HeaderData.SetLanguage(Languages.English);
                    xSession.HeaderData.Publisher = "";
                    xSession.STFSType = STFSType.Type0;
                    xSession.HeaderData.ThisType = PackageType.SavedGame;
                    xSession.HeaderData.PackageImageBinary = Resources.RB3.ImageToBytes(ImageFormat.Png);
                    xSession.HeaderData.ContentImageBinary = Resources.RB3.ImageToBytes(ImageFormat.Png);
                    xSession.HeaderData.MakeAnonymous();
                    xSession.AddFolder("songs");
                    xSession.AddFolder("songs/" + internalName);
                    xSession.AddFolder("songs/" + internalName + "/gen");
                    if (!xSession.AddFile(newDTA, "songs/songs.dta"))
                    {
                        Log("ERROR: Could not add songs.dta file to CON, skipping...");
                        continue;
                    }
                    Log("Added songs.dta file to CON successfully");
                    if (!xSession.AddFile(MIDI, "songs/" + internalName + "/" + internalName + ".mid"))
                    {
                        Log("ERROR: Could not add MIDI file to CON, skipping...");
                        continue;
                    }
                    Log("Added MIDI file to CON successfully");
                    if (!xSession.AddFile(MOGG[0], "songs/" + internalName + "/" + internalName + ".mogg"))
                    {
                        Log("ERROR: Could not add mogg file to CON, skipping...");
                        continue;
                    }
                    Log("Added mogg file to CON successfully");
                    if (!xSession.AddFile(MILO_XBOX, "songs/" + internalName + "/gen/" + internalName + ".milo_xbox"))
                    {
                        Log("ERROR: Could not add *.milo_xbox file to CON, skipping...");
                        continue;
                    }
                    Log("Added *.milo_xbox file to CON successfully");
                    if (!xSession.AddFile(PNG_XBOX, "songs/" + internalName + "/gen/" + internalName + "_keep.png_xbox"))
                    {
                        Log("ERROR: Could not add album art (*.png_xbox) to CON, skipping...");
                        continue;
                    }

                    Log("Added album art (*.png_xbox) file to CON successfully");
                    var signature = new RSAParams(Application.StartupPath + "\\bin\\KV.bin");
                    Log("Finalizing CON creation");
                    try
                    {
                        xSession.HeaderData.Title_Display = Parser.Songs[i].Artist + " - " + Parser.Songs[i].Name;
                        xSession.HeaderData.Description = "Converted to CON using " + Text;
                        var xy = new STFSPackage(xSession, signature, xCON);
                        xy.CloseIO();
                    }
                    catch (Exception ex)
                    {
                        Log("Error finalizing CON creation:");
                        Log(ex.Message);
                    }
                    Log("Unlocking CON file");
                    if (Tools.UnlockCON(xCON))
                    {
                        Log("Success");
                    }
                    else
                    {
                        Log("Failed, skipping this file...");
                        Tools.DeleteFile(xCON);
                        continue;
                    }
                    Log("Signing CON file");
                    if (Tools.SignCON(xCON))
                    {
                        Log("Success");
                    }
                    else
                    {
                        Log("Failed, skipping this file...");
                        Tools.DeleteFile(xCON);
                        continue;
                    }                    
                    Log("Work completed on this file");
                    success++;
                }
                if (cleanUp)
                {
                    Log("Cleaning up working folder...");
                    Tools.DeleteFolder(outFolder, true);
                }
            }
            Log("Successfully created " + success + " Xbox 360 CON files out of " + counter + " PS3 " + (isPKG ? "PKG files" : "folders"));
        }              
                
        private void songInFolderFormat_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This will TRY to convert PS3 folder files to usable Xbox 360 CON files for use with Rock Band 3", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            var ofd = new FolderPicker
            {
                InputPath = Environment.CurrentDirectory,
                Title = "Select the main folder where your PS3 folders are",
            };            
            if (ofd.ShowDialog(IntPtr.Zero) != true || string.IsNullOrEmpty(ofd.ResultPath)) return;
            var folder = ofd.ResultPath;

            var response = MessageBox.Show("Is this a collection of C3 PS3 folders or RPCS3 folders from Onyx PKG files?\nClick Yes for C3 folders\nClick No for RPCS3 folders", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (response == DialogResult.Yes)
            {
                var entries = Directory.GetDirectories(folder);
                List<string> folders = new List<string>();
                foreach (var entry in entries)
                {
                    folders.Add(entry);
                }
                ProcessPS3Folders(folders);
            }
            else
            {
                List<string> folders = new List<string>();
                List<string> klics = new List<string>();

                var entries = Directory.GetDirectories(folder);
                foreach (var entry in entries)
                {
                    folders.Add(entry);                                       
                    var folder_value = Path.GetFileName(entry);
                    var unhashed_klic = "Ih38rtW1ng3r" + folder_value + "10025250";
                    var klic = Tools.CreateMD5(unhashed_klic);
                    klics.Add(klic);
                }
                ProcessPS3Folders(folders, klics, true, false);                
            }            
        }

        private void patchMoggForPS3UseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "MOGG Audio File (*.mogg)|*.mogg",
                InitialDirectory = Environment.CurrentDirectory,
                Title = "Select mogg file to patch",
                Multiselect = false
            };
            if (ofd.ShowDialog() != DialogResult.OK) return;
            if (!File.Exists(ofd.FileName)) return;
            CheckApplyPS3MoggPatch(ofd.FileName, false);
        }

        private bool CheckApplyPS3MoggPatch(string inMogg, bool log)
        {
            if (nautilus3.MoggIsObfuscated(File.ReadAllBytes(inMogg)))
            {
                //deobfuscate silently
                var mogg = nautilus3.DeObfM(File.ReadAllBytes(inMogg));
                File.WriteAllBytes(inMogg, mogg);
            }
            var encrypted = nautilus3.MoggIsEncrypted(File.ReadAllBytes(inMogg));
            if (!encrypted)
            {
                if (log)
                {
                    Log("That mogg file is not encrypted, will encrypt (automatically applies PS3 patch)");
                }
                else
                {
                    MessageBox.Show("That mogg file is not encrypted, will encrypt (automatically applies PS3 patch)", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                nautilus3.EncM(File.ReadAllBytes(inMogg), inMogg);
                return true;
            }
            var patched = nautilus3.MoggIsAlreadyPatched(File.ReadAllBytes(inMogg));
            if (patched)
            {
                if (log)
                {
                    Log("That mogg file was already patched, no need to patch it again");
                }
                else
                {
                    MessageBox.Show("That mogg file was already patched, no need to patch it again", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return true;
            }
            
            //only patch 0x0C and 0x0D moggs
            var encByte = new int();
            using (var fs = File.OpenRead(inMogg))
            {
                using (var br = new BinaryReader(fs))
                {
                    encByte = br.ReadInt32();
                    fs.Close();
                    br.Close();
                }
            }
            if (encByte == 0x0C || encByte == 0x0D)
            {
                var success = nautilus3.PatchMoggForPS3Use(inMogg);
                if (success)
                {
                    if (log)
                    {
                        Log("Patched mogg file successfully");
                    }
                    else
                    {
                        MessageBox.Show("Patched mogg file successfully", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    return true;
                }
                else
                {
                    if (log)
                    {
                        Log("Failed to patch mogg file");
                    }
                    else
                    {
                        MessageBox.Show("Failed to patch mogg file", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return false;
                }
            }
            else
            {
                if (log)
                {
                    Log("No need to patch that mogg file");
                }
                else
                {
                    MessageBox.Show("No need to patch that mogg file", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return false;
            }            
        }

        private void pS3Fixer_Click(object sender, EventArgs e)
        {
            MessageBox.Show("\"The Linos Special\"\n\nThis tool will check for the total amount of audio channels and do the following:\n\nScenario 1: If equal to or less than 12 channels, will only apply the PS3 mogg patch (if necessary)\n\nScenario 2: If over 12 channels but less than or equal to 16 channels, will re-encode the mogg with quality 3 and apply the PS3 mogg patch (if necessary)\n\nScenario 3: If more than 16 channels will downmix the drums, will re-encode the mogg with quality 3 and apply the PS3 mogg patch (if necessary)\n\nAs requested, it will now also fix alphanumeric and default song_ids under all scenarios", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            var ofd = new FolderPicker
            {
                Title = "Select the folder where your CON files are",
                InputPath = Environment.CurrentDirectory,
            };
            if (ofd.ShowDialog(IntPtr.Zero) != true || string.IsNullOrEmpty(ofd.ResultPath)) return;
            fixFolder = ofd.ResultPath;
            
            conFiles = new List<string>(); //reset to avoid issues if this is ran multiple times
            var inFiles = Directory.GetFiles(fixFolder);
            foreach (var file in inFiles)
            {
                if (VariousFunctions.ReadFileType(file) == XboxFileType.STFS)
                {
                    conFiles.Add(file);
                }
            }
            Log("");
            Log("Found " + conFiles.Count + " CON " + (conFiles.Count == 1 ? "file" : "files"));
                        
            fixCounter = 0;
            fixSuccess = 0;
            fixIgnore = 0;

            EnableDisable(false);
            startTime = DateTime.Now;
            backgroundWorker2.RunWorkerAsync();            
        }

        private void backgroundWorker2_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            isLinosSpecial = true;
            foreach (var CON in conFiles.Where(File.Exists).TakeWhile(file => !backgroundWorker2.CancellationPending))
            {
                fixCounter++;
                if (FixCONForPS3(fixFolder, CON))
                {
                    fixSuccess++;
                }
            }
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            endTime = DateTime.Now;
            EnableDisable(true);
            Log("Successfully processed " + fixSuccess + (fixSuccess == 1 ? " file" : " files") + " out of " + fixCounter + (fixCounter == 1 ? " file" : " files") + " attempted");
            if (!isLinosSpecial)
            {
                Log("Ignored " + fixIgnore + " CON " + (fixIgnore == 1 ? "file" : "files") + " that didn't need fixing");
            }
            var timeDiff = endTime - startTime;
            Log("Process took " + timeDiff.Minutes + (timeDiff.Minutes == 1 ? " minute" : " minutes") + " and " + (timeDiff.Minutes == 0 && timeDiff.Seconds == 0 ? "1 second" : timeDiff.Seconds + " seconds"));
            Log("Done");
        }

        private void backgroundWorker3_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            isLinosSpecial = false;
            foreach (var CON in conFiles.Where(File.Exists).TakeWhile(file => !backgroundWorker3.CancellationPending))
            {
                fixCounter++;
                if (FixCONForPS3(fixFolder, CON))
                {
                    fixSuccess++;
                }
            }            
        }

        private void backgroundWorker3_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            EnableDisable(true);
            Log("Successfully downmixed " + fixSuccess + (fixSuccess == 1 ? " file" : " files") + " out of " + fixCounter + (fixCounter == 1 ?  " file" : " files") + " attempted");
        }

        private void picOnyx_MouseClick(object sender, MouseEventArgs e)
        {
            doOnyxMessage();
        }

        private void doOnyxMessage()
        {
            var result = MessageBox.Show(Text + " is now deprecated and is only left as part of Nautilus for legacy purposes\nThe tool suite Onyx Music Game Toolkit does a significantly much better job at creating PS3 customs and it is highly recommended that you use that tool\nClick OK to visit the Onyx Music Game Toolkit Github repository\nClick Cancel to continue with " + Text, Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            if (result == DialogResult.OK)
            {
                Process.Start("https://github.com/mtolly/onyxite-customs/releases");
                Environment.Exit(0);
            }
        }

        private void btnOnyx_Click(object sender, EventArgs e)
        {
            doOnyxMessage();
        }

        private void backgroundWorker4_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            foreach (var loopingSong in conFiles)
            {
                fixCounter++;
                Log("");
                Log("Beginning process to fix looping file '" + Path.GetFileName(loopingSong) + "'");
                Log("Please be patient, this will take a while");

                var splitter = new MoggSplitter();
                var path = Path.GetTempPath();

                if (!splitter.SplitMogg(loopingSong, path, "allstems", MoggSplitter.MoggSplitFormat.WAV, true))
                {
                    Log("Failed to separate the audio stems, can't proceed");                    
                    continue;
                }
                var oggs = Directory.GetFiles(path, "*.wav");
                if (!oggs.Any())
                {
                    Log("Failed to separate the audio stems, can't proceed");
                    continue;
                }

                Log("Split mogg file into " + oggs.Count() + " stems");

                Parser.ExtractDTA(loopingSong);
                var channels = Parser.Songs[0].ChannelsTotal;

                Log("Songs.dta file reports the mogg file should have a total of " + channels + " channels");

                Log("Initializing BASS.NET to begin rebuilding the mogg file");
                //initialize BASS.NET
                if (!Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
                {
                    Log("Error initializing BASS.NET\n" + Bass.BASS_ErrorGetCode());
                    continue;
                }
                Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_BUFFER, BassBuffer);
                Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD, 50);

                BassStream = Bass.BASS_StreamCreateFile(oggs[0], 0L, File.ReadAllBytes(oggs[0]).Length, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);

                // create a decoder for the audio file(s)
                var channel_info = Bass.BASS_ChannelGetInfo(BassStream);

                // create a mixer with same frequency rate as the input file
                BassMixer = BassMix.BASS_Mixer_StreamCreate(channel_info.freq, channels, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_MIXER_END);

                var folder = Path.GetDirectoryName(oggs[0]) + "\\";                
                var drums = folder + "drums.wav";
                var drums1 = folder + "drums_1.wav";
                var drums2 = folder + "drums_2.wav";
                var drums3 = folder + "drums_3.wav";
                var bass = folder + "bass.wav";
                var guitar = folder + "guitar.wav";
                var keys = folder + "keys.wav";
                var vocals = folder + "vocals.wav";
                var backing = folder + "backing.wav";
                var crowd = folder + "crowd.wav";

                if (File.Exists(drums))
                {
                    Log("Adding drums stem to audio stream");
                    AddAudioToMixer(drums, channels);
                }
                else
                {
                    var split_drums = new List<string> { drums1, drums2, drums3 };
                    foreach (var drum in split_drums.Where(File.Exists))
                    {
                        Log("Adding drums stem to audio stream");
                        AddAudioToMixer(drum, channels);
                    }
                }
                if (File.Exists(bass))
                {
                    Log("Adding bass stem to audio stream");
                    AddAudioToMixer(bass, channels);
                }
                if (File.Exists(guitar))
                {
                    Log("Adding guitar stem to audio stream");
                    AddAudioToMixer(guitar, channels);
                }               
                if (File.Exists(vocals))
                {
                    Log("Adding vocals stem to audio stream");
                    AddAudioToMixer(vocals, channels);
                }
                if (File.Exists(keys))
                {
                    Log("Adding keys stem to audio stream");
                    AddAudioToMixer(keys, channels);
                }
                if (File.Exists(backing))
                {
                    Log("Adding backing stem to audio stream");
                    AddAudioToMixer(backing, channels);
                }
                if (File.Exists(crowd))
                {
                    Log("Adding crowd stem to audio stream");
                    AddAudioToMixer(crowd, channels);
                }

                Log("Beginning encoding process");
                var ogg = Path.GetTempFileName();
                var encoder = BassEnc_Ogg.BASS_Encode_OGG_StartFile(BassMixer, "-q 3", BASSEncode.BASS_ENCODE_AUTOFREE, ogg);

                while (true)
                {
                    var buffer = new byte[20000];
                    var c = Bass.BASS_ChannelGetData(BassMixer, buffer, buffer.Length);
                    if (c <= 0) break;
                }

                BassEnc.BASS_Encode_Stop(encoder);
                Bass.BASS_Free();
                Bass.BASS_PluginFree(encoder);
                BassStream = 0;
                BassMixer = 0;
                currentChannel = 0;

                foreach (var file in oggs)
                {
                    Tools.DeleteFile(file);
                }

                if (File.Exists(ogg))
                {
                    Log("Created ogg file successfully");
                }
                else
                {
                    Log("Failed to create mogg file");
                    continue;
                }

                var mogg = Path.GetTempFileName();
                Log("Adding mogg header");
                if (!Tools.MakeMogg(ogg, mogg))
                {
                    Log("Failed to add mogg header");
                    continue;
                }
                else
                {
                    Log("Success");
                }

                Tools.DeleteFile(ogg);

                Log("Encrypting new mogg file");
                if (!nautilus3.EncM(File.ReadAllBytes(mogg), mogg))
                {
                    Log("Failed to encrypt mogg file");
                    continue;
                }
                else
                {
                    Log("Success");
                }

                Log("Replacing mogg in CON file with new one");
                var xCON = new STFSPackage(loopingSong);
                if (!xCON.ParseSuccess)
                {
                    Log("Couldn't parse file " + Path.GetFileName(loopingSong) + " ... skipping");
                    xCON.CloseIO();
                    continue;
                }
                var internalName = Parser.Songs[0].InternalName;
                var xMogg = xCON.GetFile("songs/" + internalName + "/" + internalName + ".mogg");
                if (xMogg == null)
                {
                    Log("Couldn't find mogg file inside " + Path.GetFileName(loopingSong) + " ... skipping");
                    xCON.CloseIO();
                    continue;
                }
                if (!xMogg.Replace(mogg))
                {
                    Log("Failed to replace the mogg file");
                    xCON.CloseIO();
                    continue;
                }
                
                Log("Saving modified CON file");
                xCON.Header.ThisType = PackageType.SavedGame;
                xCON.Header.MakeAnonymous();
                var signature = new RSAParams(Application.StartupPath + "\\bin\\KV.bin");
                xCON.RebuildPackage(signature);
                xCON.FlushPackage(signature);
                xCON.CloseIO();
                Tools.DeleteFile(mogg);
                var bOK = Tools.UnlockCON(loopingSong);
                if (bOK)
                {
                    bOK = Tools.SignCON(loopingSong);
                }
                if (bOK)
                {
                    Log("Success");
                    fixSuccess++;
                }
                else
                {
                    Log("Failed");                    
                }              
            }            
        }

        private void backgroundWorker4_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            endTime = DateTime.Now;
            EnableDisable(true);
            Log("Successfully processed " + fixSuccess + (fixSuccess == 1 ? " file" : " files") + " out of " + fixCounter + (fixCounter == 1 ? " file" : " files") + " attempted");
            var timeDiff = endTime - startTime;
            Log("Process took " + timeDiff.Minutes + (timeDiff.Minutes == 1 ? " minute" : " minutes") + " and " + (timeDiff.Minutes == 0 && timeDiff.Seconds == 0 ? "1 second" : timeDiff.Seconds + " seconds"));
            Log("Done");            
        }

        private void batchFixLoopingSongs_Click(object sender, EventArgs e)
        {
            var answer = MessageBox.Show("Please note that to fix the looping problem, I will have to break down and reconstruct the mogg file inside the CON file\nThis may result in very small audio quality loss but unfortunately no other method is known to fix this issue", Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (answer != DialogResult.OK) return;

            var ofd = new FolderPicker
            {
                Title = "Select the folder where your CON files are",
                InputPath = Environment.CurrentDirectory,
            };            
            if (ofd.ShowDialog(IntPtr.Zero) != true || string.IsNullOrEmpty(ofd.ResultPath)) return;
            fixFolder = ofd.ResultPath;

            conFiles = new List<string>(); //reset to avoid issues if this is ran multiple times
            var inFiles = Directory.GetFiles(fixFolder);
            foreach (var file in inFiles)
            {
                if (VariousFunctions.ReadFileType(file) == XboxFileType.STFS)
                {
                    conFiles.Add(file);
                }
            }
            Log("");
            Log("Found " + conFiles.Count + " CON " + (conFiles.Count == 1 ? "file" : "files"));

            fixCounter = 0;
            fixSuccess = 0;
            fixIgnore = 0;                             

            EnableDisable(false);
            startTime = DateTime.Now;
            backgroundWorker4.RunWorkerAsync();
        }

        private void mnuToolStripSeparator_Custom_Paint(Object sender, PaintEventArgs e)
        {
            ToolStripSeparator sep = (ToolStripSeparator)sender;

            e.Graphics.FillRectangle(new SolidBrush(Color.FromName("GradientInactiveCaption")), 0, 0, sep.Width, sep.Height);

            e.Graphics.DrawLine(new Pen(Color.White), 30, sep.Height / 2, sep.Width - 4, sep.Height / 2);

        }
    }
}