using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using Nautilus.Properties;
using Nautilus.x360;
using DocumentFormat.OpenXml.Office2010.Drawing;

namespace Nautilus
{
    public partial class BatchRenamer : Form
    {
        private readonly List<string> inputFiles;
        private int duplicates;
        private string newName;
        private readonly NemoTools Tools;
        private readonly DTAParser Parser;
        private bool XOnly;
        private STFSPackage xPackage;
        private int ExpertCount;
        private List<string> PhaseShiftSongs;
        private static Color mMenuBackground;
        private string songAuthor;

        public BatchRenamer(Color ButtonBackColor, Color ButtonTextColor)
        {
            InitializeComponent();
            
            Tools = new NemoTools();
            Parser = new DTAParser();
            inputFiles = new List<string>();
            mMenuBackground = menuStrip1.BackColor;
            menuStrip1.Renderer = new DarkRenderer();

            var formButtons = new List<Button> { btnRefresh, btnFolder, btnBegin };
            foreach (var button in formButtons)
            {
                button.BackColor = ButtonBackColor;
                button.ForeColor = ButtonTextColor;
                button.FlatAppearance.MouseOverBackColor = button.BackColor == Color.Transparent ? Color.FromArgb(127, Color.AliceBlue.R, Color.AliceBlue.G, Color.AliceBlue.B) : Tools.LightenColor(button.BackColor);
            }
        }

        private void SaveConfig()
        {
            var configFolder = Application.StartupPath + "\\bin\\config";
            if (!Directory.Exists(configFolder))
            {
                Directory.CreateDirectory(configFolder);
            }
            var configFile = Path.Combine(configFolder, "renamer.config");
            var sw = new StreamWriter(configFile, false);
            sw.WriteLine(renameFiles.Checked ? "True" : "False");
            sw.WriteLine(renameInternalName.Checked ? "True" : "False");
            sw.WriteLine(ezloDQMode.Checked ? "True" : "False");
            sw.WriteLine(renameTheArtistSong.Checked ? "True" : "False");
            sw.WriteLine(renameArtistTheSong.Checked ? "True" : "False");
            sw.WriteLine(renameSongTheArtist.Checked ? "True" : "False");
            sw.WriteLine(renameSongArtistThe.Checked ? "True" : "False");
            sw.WriteLine(renameYearArtist.Checked ? "True" : "False");
            sw.WriteLine(renameYearSong.Checked ? "True" : "False");
            sw.WriteLine(normalizeFeaturedArtists.Checked ? "True" : "False");
            sw.WriteLine(removeSpacesFromFileName.Checked ? "True" : "False");
            sw.WriteLine(ignoreXboxFilesystemLimitations.Checked ? "True" : "False");
            sw.WriteLine(fullNameNoAccents.Checked ? "True" : "False");
            sw.WriteLine(tryToSortFiles.Checked ? "True" : "False");
            sw.WriteLine(tryDetailedSubsorting.Checked ? "True" : "False");
            sw.WriteLine(separateXonlySongs.Checked ? "True" : "False");
            sw.WriteLine(sortByAuthor.Checked ? "True" : "False");
            sw.WriteLine(deleteOlderCopies.Checked ? "True" : "False");
            sw.WriteLine(deleteExactCopiesONLY.Checked ? "True" : "False");
            sw.Dispose();
        }

        private void LoadConfig()
        {
            var configFolder = Application.StartupPath + "\\bin\\config";
            if (!Directory.Exists(configFolder)) return;            
            var configFile = Path.Combine(configFolder, "renamer.config");
            if (!File.Exists(configFile)) return;

            var sr = new StreamReader(configFile);
            try
            {
                renameFiles.Checked = sr.ReadLine().Contains("True");
                renameInternalName.Checked = sr.ReadLine().Contains("True");
                ezloDQMode.Checked = sr.ReadLine().Contains("True");
                renameTheArtistSong.Checked = sr.ReadLine().Contains("True");
                renameArtistTheSong.Checked = sr.ReadLine().Contains("True");
                renameSongTheArtist.Checked = sr.ReadLine().Contains("True");
                renameSongArtistThe.Checked = sr.ReadLine().Contains("True");
                renameYearArtist.Checked = sr.ReadLine().Contains("True");
                renameYearSong.Checked = sr.ReadLine().Contains("True");
                normalizeFeaturedArtists.Checked = sr.ReadLine().Contains("True");
                removeSpacesFromFileName.Checked = sr.ReadLine().Contains("True");
                ignoreXboxFilesystemLimitations.Checked = sr.ReadLine().Contains("True");
                fullNameNoAccents.Checked = sr.ReadLine().Contains("True");
                tryToSortFiles.Checked = sr.ReadLine().Contains("True");
                tryDetailedSubsorting.Checked = sr.ReadLine().Contains("True");
                separateXonlySongs.Checked = sr.ReadLine().Contains("True");
                sortByAuthor.Checked = sr.ReadLine().Contains("True");
                deleteOlderCopies.Checked = sr.ReadLine().Contains("True");
                deleteExactCopiesONLY.Checked = sr.ReadLine().Contains("True");
            }
            catch
            {
                sr.Dispose();
                File.Delete(configFile);
                return;
            }            
            sr.Dispose();
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
            inputFiles.Clear();

            if (txtFolder.Text != "")
            {
                Tools.CurrentFolder = txtFolder.Text;
                Log("");
                Log("Reading input directory ... hang on");
                EnableDisable(false);
                folderScanner.RunWorkerAsync();
            }
            else
            {
                btnBegin.Visible = false;
                btnRefresh.Visible = false;
                txtFolder.Focus();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            var tFolder = txtFolder.Text;
            txtFolder.Text = "";
            txtFolder.Text = tFolder;
        }

        private string doDuplicates(string file)
        {
            var dupCounter = 1;
            
            if (!File.Exists(file)) return file;

            //if already a duplicate it'll end up in (x)
            if (file.EndsWith(")", StringComparison.Ordinal))
            {
                if (file.Substring(file.Length - 3, 1) == "(")  //(x)
                {
                    dupCounter = Convert.ToInt16(file.Substring(file.Length - 2, 1));
                    newName = newName.Substring(0, newName.Length - 3);
                }
                else if (file.Substring(file.Length - 4, 1) == "(") //(xx)
                {
                    dupCounter = Convert.ToInt16(file.Substring(file.Length - 3, 2));
                    newName = newName.Substring(0, newName.Length - 4);
                }
            }

            dupCounter++;
            if (File.Exists(file))
            {
                newName = newName + " (" + dupCounter + ")";
            }
            //this recursive call is on purpose to do on a loop until no more duplicates are found. leave it
            return doDuplicates(Path.GetDirectoryName(file) + "\\" + newName);
        }

        private string SortSubFolders(string display, string description, uint ID, string origin)
        {
            if (!Directory.Exists(txtFolder.Text + "\\" + origin))
            {
                Directory.CreateDirectory(txtFolder.Text + "\\" + origin);
            }
            var sort = origin + "\\";
            var game = "";
            var disp = display.ToLowerInvariant();
            var desc = description.ToLowerInvariant();
            if (!tryDetailedSubsorting.Checked)
            {
                return sort;
            }

            switch (ID)
            {
                case 0x45410829:
                    game = "\\rb1";
                    break;
                case 0x45410869:
                    game = "\\rb2";
                    break;
            }

            if (game != "")
            {
                if (!Directory.Exists(txtFolder.Text + "\\" + origin + game))
                {
                    Directory.CreateDirectory(txtFolder.Text + "\\" + origin + game);
                }
                sort = origin + game + "\\";
            }

            if (disp.Contains("pack") || desc.Contains("pack"))
            {
                if (!Directory.Exists(txtFolder.Text + "\\" + origin + game + "\\packs"))
                {
                    Directory.CreateDirectory(txtFolder.Text + "\\" + origin + game + "\\packs");
                }
                sort = origin + game + "\\packs\\";
            }
            else if (disp.Contains("album") || desc.Contains("album"))
            {
                if (!Directory.Exists(txtFolder.Text + "\\" + origin + game + "\\albums"))
                {
                    Directory.CreateDirectory(txtFolder.Text + "\\" + origin + game + "\\albums");
                }
                sort = origin + game + "\\albums\\";
            }
            else if (disp.Contains("export") || desc.Contains("export"))
            {
                if (!Directory.Exists(txtFolder.Text + "\\" + origin + game + "\\exports"))
                {
                    Directory.CreateDirectory(txtFolder.Text + "\\" + origin + game + "\\exports");
                }
                sort = origin + game + "\\exports\\";
            }

            return sort;
        }

        private string SortFiles(string display, string description, uint ID, string songName)
        {
            var sort = "";
            var song = "";
            var disp = display.ToLowerInvariant();
            var desc = description.ToLowerInvariant();
            var name = songName.ToLowerInvariant();

            if (!tryToSortFiles.Checked || (string.IsNullOrWhiteSpace(display) && string.IsNullOrWhiteSpace(name)))
            {
                return sort;
            }

            if (description.Contains("-"))
            {
                if (description.Contains("--"))
                {
                    song = description.Substring(0, description.IndexOf("--", StringComparison.Ordinal));
                }
                else if (description.Contains(" - "))
                {
                    song = description.Substring(0, description.IndexOf(" - ", StringComparison.Ordinal));
                }
                else
                {
                    song = description.Substring(0, description.IndexOf("-", StringComparison.Ordinal));
                }
                song = song.Trim().Replace("\"", "");
            }

            if (disp.Contains("pro upgrade") || desc.Contains("pro upgrade") || desc.Contains("(pro)") || desc.Contains("(upgrade)") || disp.Contains("(pro)") || disp.Contains("(upgrade)"))
            {
                sort = SortSubFolders(display, description, ID, "pro_upgrades");
            }
            else if (disp.Contains("beatles") || desc.Contains("beatles") || name.Contains("beatles"))
            {
                sort = SortSubFolders(display, description, ID, "beatles");
            }
            else if (disp.Contains("ghtorb3") || desc.Contains("ghtorb3") || desc.Contains("t=405473") || desc.Contains(("tiny.cc/ghtorb"))) //t=405473 = ghtorb3 thread on xbox360iso, older songs
            {
                sort = SortSubFolders(display, description, ID, "ghtorb3");
            }
            else if (disp.Contains("pony") || desc.Contains("pony") || name.Contains("pony") || disp.Contains("ponies") || desc.Contains("ponies") || name.Contains("ponies") || disp.Contains("brony") || desc.Contains("brony") || name.Contains("brony") || disp.Contains("bronies") || desc.Contains("bronies") || name.Contains("bronies"))
            {
                sort = SortSubFolders(display, description, ID, "pony_rock");
            }
            else if (desc.Contains("rockband.com") || description.Contains("RockBand") || description.Contains("Rock Band") || description.Contains("Includes \""))
            {
                sort = SortSubFolders(display, description, ID, "hmx_dlc");
            }
            else if (disp.Contains("pack") || disp.Contains("album") || disp.Contains("challenge"))
            {
                if (desc.Contains("includes"))
                {
                    sort = SortSubFolders(display, description, ID, "hmx_dlc");
                }
            }
            else if (disp.Contains("c3") || desc.Contains("c3") || disp.Contains("customscreators") || desc.Contains("customscreators") || disp.Contains("customs creators") || desc.Contains("customs creators"))
            {
                sort = SortSubFolders(display, description, ID, "c3_dlc");
            }
            else if (ID == 0x45410829) //RB1 - no RBN at this point
            {
                sort = SortSubFolders(display, description, ID, "hmx_dlc");
            }
            else if (song == display)
            {
                sort = SortSubFolders(display, description, ID, "rbn");
            }
            else if (ID == 0x45410869) //RB2 - most likely DLC
            {
                sort = SortSubFolders(display, description, ID, "hmx_dlc");
            }

            return sort;
        }

        private string arrangeName(string song, string artist, int year, string internalName)
        {
            var arranged = "";
            if (ezloDQMode.Checked)
            {
                arranged = internalName + " (" + artist + " - " + song + ")";
            }
            else if (renameYearArtist.Checked && year > 0)
            {
                arranged = "(" + year + ") " + artist + " - " + song;
            }
            else if (renameYearSong.Checked && year > 0)
            {
                arranged = "(" + year + ") " + song + " - " + artist;
            }
            else
            {
                if (renameTheArtistSong.Checked)
                {
                    arranged = artist + " - " + song;
                }
                else if (renameArtistTheSong.Checked)
                {
                    if (artist.Length > 6)
                    {
                        if (artist.Substring(0, 4) == "The ")
                        {
                            arranged = artist.Substring(4, artist.Length - 4) + ", The - " + song;
                        }
                        else
                        {
                            arranged = artist + " - " + song;
                        }
                    }
                    else
                    {
                        arranged = artist + " - " + song;
                    }
                }
                else if (renameSongTheArtist.Checked)
                {
                    arranged = song + " - " + artist;
                }
                else if (renameSongArtistThe.Checked)
                {
                    if (artist.Length > 6)
                    {
                        if (artist.Substring(0, 4) == "The ")
                        {
                            arranged = song + " - " + artist.Substring(4, artist.Length - 4) + ", The";
                        }
                        else
                        {
                            arranged = song + " - " + artist;
                        }
                    }
                    else
                    {
                        arranged = song + " - " + artist;
                    }
                }
            }            
            return arranged;
        }

        private string renameBasic(string disp, string desc, string file)
        {
            string rename;
            var megasusMod = false;
            var song = disp;
            if (song != "")
            {
                song = Tools.CleanString(song, true);

                //if Description has the same string as Title_Display, then it's most likely official content
                //this will try to get the name of the artist, so we end up with Artist - Song, and not just Song
                if (desc.ToLowerInvariant().Contains(disp.ToLowerInvariant()) ||
                    desc.ToLowerInvariant().Contains(song.ToLowerInvariant()))
                {
                    if (desc.Contains("-"))
                    {
                        if (desc.Contains("--"))
                        {
                            song = desc.Substring(0,
                                                  desc.IndexOf("--", StringComparison.Ordinal));
                        }
                        else if (desc.Contains(" - "))
                        {
                            song = desc.Substring(0,
                                                  desc.IndexOf(" - ", StringComparison.Ordinal));
                        }
                        else
                        {
                            song = desc.Substring(0, desc.IndexOf("-", StringComparison.Ordinal));
                        }
                        song = Tools.CleanString(song, true);
                    }
                    string artist;
                    //this is for songs like Megasus - Megasus
                    if (
                        desc.ToLowerInvariant()
                            .Contains(song.ToLowerInvariant() + "--" + song.ToLowerInvariant()) ||
                        desc.ToLowerInvariant()
                            .Contains(song.ToLowerInvariant() + " -- " + song.ToLowerInvariant()) ||
                        desc.ToLowerInvariant()
                            .Contains(song.ToLowerInvariant() + " - " + song.ToLowerInvariant()) ||
                        desc.ToLowerInvariant()
                            .Contains(song.ToLowerInvariant() + "-" + song.ToLowerInvariant()))
                    {
                        artist = song;
                        megasusMod = true;
                    }
                    else
                    {
                        var temp = desc.ToLowerInvariant().Replace(song.ToLowerInvariant(), "");
                        var index = desc.ToLowerInvariant()
                                        .IndexOf(temp, StringComparison.Ordinal);
                        artist = desc.Substring(index, temp.Length);
                    }
                    if (artist != "")
                    {
                        var indexDash = 0;
                        artist = artist.Replace("is a cover of a song made famous by", "-");
                        artist = artist.Replace("as made famous by", "-");
                        artist = artist.Replace("by ", "-");
                        artist = artist.Replace("---", "-");
                        artist = artist.Replace("--", "-");
                        artist = Tools.CleanString(artist, false);
                        if (artist.Contains("-"))
                        {
                            indexDash = artist.IndexOf("-", StringComparison.Ordinal) + 1;
                        }
                        var indexPeriod = 0;
                        if (artist.ToLowerInvariant().Contains(". song"))
                        {
                            indexPeriod = artist.IndexOf(". song", indexDash,StringComparison.InvariantCultureIgnoreCase);
                        }
                        else if (artist.ToLowerInvariant().Contains(".song"))
                        {
                            indexPeriod = artist.IndexOf(".song", indexDash,StringComparison.InvariantCultureIgnoreCase);
                        }
                        else if (artist.ToLowerInvariant().Contains(". this"))
                        {
                            indexPeriod = artist.IndexOf(". this", indexDash,StringComparison.InvariantCultureIgnoreCase);
                        }
                        else if (artist.ToLowerInvariant().Contains(".this"))
                        {
                            indexPeriod = artist.IndexOf(".this", indexDash,StringComparison.InvariantCultureIgnoreCase);
                        }
                        else if (artist.ToLowerInvariant().Contains(". for"))
                        {
                            indexPeriod = artist.IndexOf(". for", indexDash,StringComparison.InvariantCultureIgnoreCase);
                        }
                        else if (artist.ToLowerInvariant().Contains(".for"))
                        {
                            indexPeriod = artist.IndexOf(".for", indexDash,StringComparison.InvariantCultureIgnoreCase);
                        }
                        else if (artist.ToLowerInvariant().Contains(". credit"))
                        {
                            indexPeriod = artist.IndexOf(". credit", indexDash,StringComparison.InvariantCultureIgnoreCase);
                        }
                        else if (artist.ToLowerInvariant().Contains(".credit"))
                        {
                            indexPeriod = artist.IndexOf(".credit", indexDash,StringComparison.InvariantCultureIgnoreCase);
                        }
                        if (indexDash > 0)
                        {
                            if (indexPeriod > 0)
                            {
                                artist = artist.Substring(indexDash, indexPeriod - indexDash);
                            }
                        }
                        else
                        {
                            artist = "";
                        }
                        if (artist != "")
                        {
                            artist = Tools.CleanString(artist, true);

                            rename = arrangeName(song, artist, 0, "");
                        }
                        else
                        {
                            rename = song;
                        }
                    }
                    else
                    {
                        rename = song;
                    }
                }
                else
                {
                    rename = song;
                }
            }
            else
            {
                rename = song;
            }
            if (rename.Contains("-") && !rename.Contains(" -"))
            {
                rename = rename.Replace("-", " -");
            }
            if (rename.Contains("-") && !rename.Contains("- "))
            {
                rename = rename.Replace("-", "- ");
            }
            if (megasusMod)
            {
                rename = arrangeName(song, song, 0, "");
            }
            rename = rename.Replace(".", "").Trim();
            if (string.IsNullOrWhiteSpace(rename))
            {
                Log("Internal name for file '" + Path.GetFileName(file) + "' is blank!");
                Log("Trying to figure out a better name for '" + Path.GetFileName(file) + "'");
                var name = Path.GetFileName(file);
                if (name != null)
                {
                    if (name.Contains("-") && !name.Contains(" -"))
                    {
                        name = name.Replace("-", " -");
                    }
                    if (name.Contains("-") && !name.Contains("- "))
                    {
                        name = name.Replace("-", "- ");
                    }
                }
                if (name != null)
                {
                    rename = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name);
                }
                if (name != "" && name != rename && name != null)
                {
                    Log("Found a better name! Renaming '" + Path.GetFileName(file) + "' to '" + name + "'");
                }
                else
                {
                    Log("No luck");
                }
            }
            if (replaceSpacesWithUnderscores.Checked)
            {
                rename = rename.Replace(" ", "_");
            }
            else if (removeSpacesFromFileName.Checked)
            {
                rename = rename.Replace(" ", "");
            }
            return rename.TrimStart().TrimEnd('?', '.', ',', ' ');
        }

        private string renameAdvanced(byte[] xDTA)
        {
            if (!Parser.ReadDTA(xDTA) || !Parser.Songs.Any()) return "";
            var name = Tools.CleanString(Parser.Songs[0].Name, false, ignoreXboxFilesystemLimitations.Checked);
            var artist = Tools.CleanString(Parser.Songs[0].Artist, false, ignoreXboxFilesystemLimitations.Checked);
            var year = Parser.Songs[0].YearReleased;
            var internalName = Parser.Songs[0].InternalName;
            if (renameInternalName.Checked) return internalName;
            if (!string.IsNullOrWhiteSpace(internalName) && !XOnly && xPackage.ParseSuccess)
            {
                var midi = "";
                var xfile = xPackage.GetFile("songs/" + internalName + "/" + internalName + ".mid");
                if (xfile != null)
                {
                    midi = Path.GetTempPath() + internalName + ".mid";
                    Tools.DeleteFile(midi);
                    if (!xfile.ExtractToFile(midi))
                    {
                        midi = "";
                    }
                }
                if (midi != "")
                {
                    XOnly = !Tools.DoesMidiHaveEMH(midi);
                }
                Tools.DeleteFile(midi);
            }
            string rename;
            if (Parser.Songs.Count == 1) //if single song, packs will have values > 1
            {
                rename = arrangeName(name, artist, year, internalName);
                if (replaceSpacesWithUnderscores.Checked)
                {
                    rename = rename.Replace(" ", "_");
                }
                else if (removeSpacesFromFileName.Checked)
                {
                    rename = rename.Replace(" ", "");
                }
            }
            else
            {
                rename = ""; //this will use the basic renaming since this is most likely a pack
            }
            return rename.TrimStart().TrimEnd('?', '.', ',', ' ');
        }

        private void btnBegin_Click(object sender, EventArgs e)
        {
            if (btnBegin.Text == "Cancel")
            {
                FileRenamer.CancelAsync();
                Log("User cancelled process...stopping as soon as possible");
                btnBegin.Enabled = false;
                return;
            }
            EnableDisable(false);
            btnBegin.Text = "Cancel";
            toolTip1.SetToolTip(btnBegin, "Click to cancel renaming process");
            FileRenamer.RunWorkerAsync();
        }

        private static string CleanForXbox(string input, bool trim = true)
        {
            var destfile = input;
            if (string.IsNullOrWhiteSpace(destfile)) return input;
            
            destfile = destfile.Replace("AC-DC", "ACDC");
            destfile = destfile.Replace("AC DC", "ACDC");
            destfile = destfile.Replace("P!nk", "Pink");
            destfile = destfile.Replace("'", "");
            destfile = destfile.Replace(",", "");
            destfile = destfile.Replace(";", "");
            destfile = destfile.Replace("!", "");
            destfile = destfile.Replace("¡", "");
            destfile = destfile.Replace("@", "");
            destfile = destfile.Replace("#", "");
            destfile = destfile.Replace("$", "");
            destfile = destfile.Replace("%", "");
            destfile = destfile.Replace("^", "");
            destfile = destfile.Replace("+", "");
            destfile = destfile.Replace("~", "");
            destfile = destfile.Replace("`", "");
            destfile = destfile.Replace("¿", "");
            destfile = destfile.Replace("ä", "a");
            destfile = destfile.Replace("å", "a");
            destfile = destfile.Replace("á", "a");
            destfile = destfile.Replace("á", "a");
            destfile = destfile.Replace("ë", "e");
            destfile = destfile.Replace("é", "e");
            destfile = destfile.Replace("è", "e");
            destfile = destfile.Replace("ï", "i");
            destfile = destfile.Replace("ì", "i");
            destfile = destfile.Replace("í", "i");
            destfile = destfile.Replace("ö", "o");
            destfile = destfile.Replace("ò", "o");
            destfile = destfile.Replace("ó", "o");
            destfile = destfile.Replace("ù", "u");
            destfile = destfile.Replace("ú", "u");
            destfile = destfile.Replace("ü", "u");
            destfile = destfile.Replace("ÿ", "y");
            destfile = destfile.Replace("Ä", "A");
            destfile = destfile.Replace("É", "E");
            destfile = destfile.Replace("Ê", "E");
            destfile = destfile.Replace("Ö", "O");
            destfile = destfile.Replace("Ü", "U");
            destfile = destfile.Replace("ñ", "n");
            destfile = destfile.Replace("Ñ", "N");
            if (destfile.EndsWith(".", StringComparison.Ordinal)) //can't have files like Y.M.C.A.
            {
                destfile = destfile.Substring(0, destfile.Length - 1);
            }
            if (destfile.Length > 42 && trim)
            {
                destfile = destfile.Substring(0, 42).Trim();
            }
            return destfile;
        }

        private void HandleDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
        }

        private void HandleDragDrop(object sender, DragEventArgs e)
        {
            if (picWorking.Visible) return;
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            txtFolder.Text = Path.GetDirectoryName(files[0]);
            Tools.CurrentFolder = txtFolder.Text;
        }
        
        private void renameFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnBegin.Enabled = renameFiles.Checked || tryToSortFiles.Checked;
            replaceSpacesWithUnderscores.Enabled = renameFiles.Checked;
            removeSpacesFromFileName.Enabled = renameFiles.Checked;
            normalizeFeaturedArtists.Enabled = renameFiles.Checked;
            ignoreXboxFilesystemLimitations.Enabled = renameFiles.Checked;
            renameTheArtistSong.Enabled = renameFiles.Checked;
            renameArtistTheSong.Enabled = renameFiles.Checked;
            renameSongTheArtist.Enabled = renameFiles.Checked;
            renameSongArtistThe.Enabled = renameFiles.Checked;
            renameYearSong.Enabled = renameFiles.Checked;
            renameYearArtist.Enabled = renameFiles.Checked;
        }

        private void replaceSpacesWithUnderscoresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (replaceSpacesWithUnderscores.Checked)
            {
                removeSpacesFromFileName.Checked = false;
            }
        }

        private void sortFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!tryToSortFiles.Checked)
            {
                tryDetailedSubsorting.Checked = false;
            }
            tryDetailedSubsorting.Enabled = tryToSortFiles.Checked;
            btnBegin.Enabled = renameFiles.Checked || tryToSortFiles.Checked;
            if (tryToSortFiles.Checked)
            {
                sortByAuthor.Checked = false;
            }
        }

        private void deleteOlderCopiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (deleteOlderCopies.Checked)
            {
                deleteExactCopiesONLY.Checked = false;
            }
        }

        private void deleteExactCopiesONLYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (deleteExactCopiesONLY.Checked)
            {
                deleteOlderCopies.Checked = false;
            }
        }

        private void tryDetailedSubsortingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            separateXonlySongs.Enabled = tryDetailedSubsorting.Checked;
        }
        
        private void removeSpacesFromFileNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (removeSpacesFromFileName.Checked)
            {
                replaceSpacesWithUnderscores.Checked = false;
            }
        }

        private void exportLogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.ExportLog(Text, lstLog.Items);
        }

        private void BatchRenamer_Shown(object sender, EventArgs e)
        {
            Log("Welcome to " + Text);
            Log("Drag and drop the CON / LIVE file(s) here");
            Log("Or click 'Change Input Folder' to select the files");
            Log("Ready to begin");
            LoadConfig();
        }
        
        private void EnableDisable(bool enabled)
        {
            menuStrip1.Enabled = enabled;
            btnFolder.Enabled = enabled;
            btnRefresh.Enabled = enabled;
            txtFolder.Enabled = enabled;
            picWorking.Visible = !enabled;
            lstLog.Cursor = enabled ? Cursors.Default : Cursors.WaitCursor;
            Cursor = lstLog.Cursor;
        }

        private void BatchRenamer_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveConfig();
            if (!picWorking.Visible) return;
            MessageBox.Show("Please wait until the current process finishes", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            e.Cancel = true;
        }

        private void FileRenamer_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            var renamed = 0;
            duplicates = 0;
            var sorted = 0;
            var deleted = 0;
            var rbafiles = 0;
            var midifiles = 0;
            ExpertCount = 0;
            var skipped = 0;

            foreach (var sourceFile in inputFiles.Where(File.Exists).TakeWhile(file => !FileRenamer.CancellationPending))
            {
                newName = "";
                XOnly = false;
                try
                {
                    switch (Path.GetExtension(sourceFile).ToLowerInvariant())
                    {                        
                        default:
                            if (VariousFunctions.ReadFileType(sourceFile) == XboxFileType.STFS)
                            {
                                try
                                {
                                    xPackage = new STFSPackage(sourceFile);
                                    if (!xPackage.ParseSuccess)
                                    {
                                        Log("Error opening file '" + Path.GetFileName(sourceFile) + "' for renaming/sorting. Skipping...");
                                        xPackage.CloseIO();
                                    }
                                    else
                                    {
                                        var origFile = new FileInfo(sourceFile);
                                        var disp = Tools.FixBadChars(xPackage.Header.Title_Display.Replace("\"", ""));
                                        var desc = Tools.FixBadChars(xPackage.Header.Description.Replace("\"", ""));
                                        var ID = xPackage.Header.TitleID;

                                        if (renameFiles.Checked)
                                        {
                                            if (Tools.DescribesPack(desc, disp))
                                            {
                                                newName = renameBasic(disp, desc, sourceFile);
                                            }
                                            else
                                            {
                                                var xFile = xPackage.GetFile("songs/songs.dta");
                                                var xDTA = xFile.Extract();
                                                if (sortByAuthor.Checked)
                                                {
                                                    Parser.ReadDTA(xDTA);
                                                    if (Parser.Songs.Count == 0)
                                                    {
                                                        Log("Error reading songs.dta file for '" + Path.GetFileName(sourceFile) + "'");
                                                        xPackage.CloseIO();
                                                        skipped++;
                                                        continue;
                                                    }
                                                    xPackage.CloseIO();
                                                    songAuthor = Tools.CleanString(Parser.Songs[0].ChartAuthor, false, true);
                                                    if (string.IsNullOrEmpty(songAuthor))
                                                    {
                                                        songAuthor = "Unknown";
                                                    }
                                                    var folder = txtFolder.Text + "\\_Sorted\\" + songAuthor + "\\";
                                                    if (!Directory.Exists(folder))
                                                    {
                                                        Directory.CreateDirectory(folder);
                                                    }
                                                    var targetFile = folder + Tools.CleanString(Path.GetFileName(sourceFile), false, true);
                                                    if (File.Exists(targetFile))
                                                    {
                                                        targetFile = targetFile + "(2)";//for duplicates
                                                        duplicates++;
                                                    }
                                                    Tools.MoveFile(sourceFile, targetFile);
                                                    Log("Sorted file '" + Path.GetFileName(sourceFile) + "' to folder '" + songAuthor + "'");
                                                    sorted++;
                                                    continue;
                                                }
                                                newName = xDTA != null && xDTA.Length > 0 ? renameAdvanced(xDTA) : renameBasic(disp, desc, sourceFile);
                                                if (string.IsNullOrWhiteSpace(newName))
                                                {
                                                    newName = renameBasic(disp, desc, sourceFile);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            newName = Path.GetFileName(sourceFile);
                                        }
                                        xPackage.CloseIO();

                                        if (string.IsNullOrWhiteSpace(newName))
                                        {
                                            Log("Couldn't get a better name for '" + Path.GetFileName(sourceFile) + "'! Skipping");
                                        }
                                        else
                                        {
                                            string newfile;

                                            var sort = SortFiles(disp, desc, ID, newName);

                                            if (normalizeFeaturedArtists.Checked)
                                            {
                                                newName = Tools.FixFeaturedArtist(newName); //adjust featured artists
                                            }

                                            if (fullNameNoAccents.Checked)
                                            {
                                                newName = CleanForXbox(newName, false);
                                                newfile = Path.GetDirectoryName(sourceFile) + "\\" + sort + newName;
                                            }
                                            else if (ignoreXboxFilesystemLimitations.Checked)
                                            {
                                                newfile = Path.GetDirectoryName(sourceFile) + "\\" + sort + newName;
                                            }
                                            else
                                            {
                                                newName = CleanForXbox(newName);
                                                newfile = Path.GetDirectoryName(sourceFile) + "\\" + sort + newName;
                                            }

                                            if (tryDetailedSubsorting.Checked && separateXonlySongs.Checked && XOnly)
                                            {
                                                var folder = Path.GetDirectoryName(newfile) + "\\Expert Only\\";
                                                var filename = Path.GetFileName(newfile);

                                                if (!Directory.Exists(folder))
                                                {
                                                    Directory.CreateDirectory(folder);
                                                }

                                                newfile = folder + filename;
                                                ExpertCount++;
                                            }

                                            if (!String.Equals(sourceFile, newfile, StringComparison.InvariantCultureIgnoreCase))
                                                //only if the old file isn't the same as the new file
                                            {
                                                //if the file names are the same, only sorting is being done, mark as not renamed
                                                if (Path.GetFileName(sourceFile) == Path.GetFileName(newfile))
                                                {
                                                    skipped++;
                                                }

                                                try
                                                {
                                                    if (Directory.Exists(newfile))
                                                    {
                                                        MessageBox.Show("The internal name for file '" + Path.GetFileName(sourceFile) + "' is '" + newName +
                                                                        "'\nIn a moment of comic-book coincidence, there is also a folder with that same name in that same directory!\nSince these are extensionless files, Windows won't allow me to name your file the same name as the folder...\nTo work around this, I added '_rb3con' to your file, so now it is\n'" +
                                                                        newName + "_rb3con'\n\nCrazy Windows huh", Text,
                                                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                        newName = newName + "_rb3con";
                                                        newfile = Path.GetDirectoryName(newfile) + "\\" + newName;
                                                    }

                                                    if (File.Exists(newfile))
                                                    {
                                                        duplicates++;

                                                        if (deleteExactCopiesONLY.Checked)
                                                        {
                                                            var newFile = new FileInfo(newfile);

                                                            if (origFile.Length == newFile.Length &&
                                                                origFile.LastWriteTime == newFile.LastWriteTime)
                                                            {
                                                                Tools.SendtoTrash(newfile);
                                                                deleted++;
                                                            }
                                                            else
                                                            {
                                                                newfile = doDuplicates(newfile);
                                                            }
                                                        }
                                                        else if (deleteOlderCopies.Checked)
                                                        {
                                                            var newFile = new FileInfo(newfile);

                                                            if (origFile.LastWriteTime > newFile.LastWriteTime)
                                                            {
                                                                Tools.SendtoTrash(newfile);
                                                                deleted++;
                                                            }
                                                            else
                                                            {
                                                                Tools.SendtoTrash(sourceFile);
                                                                deleted++;
                                                                Tools.MoveFile(newfile, sourceFile);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            newfile = doDuplicates(newfile);
                                                        }
                                                    }

                                                    if (Tools.MoveFile(sourceFile, newfile))
                                                    {
                                                        if (Path.GetFileName(sourceFile) != newName)
                                                        {
                                                            Log("File '" + Path.GetFileName(sourceFile) + "' was successfully renamed to '" + newName + "'");
                                                            renamed++;
                                                        }
                                                        if (sort != "")
                                                        {
                                                            Log("File '" + Path.GetFileName(sourceFile) + "' was sorted to folder '" + sort.Substring(0, sort.Length - 1) + "'");
                                                            sorted++;
                                                        }
                                                        if (XOnly && tryDetailedSubsorting.Checked && separateXonlySongs.Checked)
                                                        {
                                                            Log("File '" + Path.GetFileName(sourceFile) + "' was Expert-Only and was sorted to folder '" +
                                                                (Path.GetDirectoryName(newfile).Replace(Path.GetDirectoryName(sourceFile) + "\\", "")) + "'");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Log("Looks like renaming file '" + Path.GetFileName(sourceFile) + "' failed. Sorry");
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    Log("There was an error renaming file '" + Path.GetFileName(sourceFile));
                                                    Log("The error says: " + ex.Message);
                                                }
                                            }
                                            else
                                            {
                                                skipped++;
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Log("There was an error processing file '" + Path.GetFileName(sourceFile) + "'. Skipping");
                                    Log("The error says: " + ex.Message);
                                }
                            }
                            break;
                        case ".mid":
                            if (!Directory.Exists(Path.GetDirectoryName(sourceFile) + "\\midi"))
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(sourceFile) + "\\midi");
                            }
                            Tools.MoveFile(sourceFile, Path.GetDirectoryName(sourceFile) + "\\midi\\" + Path.GetFileName(sourceFile));
                            midifiles++;
                            break;
                        case ".rba":
                            if (!Directory.Exists(Path.GetDirectoryName(sourceFile) + "\\rba"))
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(sourceFile) + "\\rba");
                            }
                            Tools.MoveFile(sourceFile, Path.GetDirectoryName(sourceFile) + "\\rba\\" + Path.GetFileName(sourceFile));
                            rbafiles++;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Log("There was a problem accessing that file");
                    Log("The error says: " + ex.Message);
                }
            }

            if (sortByAuthor.Checked)
            {
                if (sorted > 0)
                {
                    Log("Successfully sorted " + sorted + (sorted > 1 ? " files" : " file"));

                    if (skipped > 0)
                    {
                        Log("Skipped sorting " + skipped + (skipped > 1 ? " files" : " file"));
                    }
                }
            }
            else if (renamed > 0)
            {
                Log("Successfully renamed " + renamed + (renamed > 1 ? " files" : " file"));

                if (skipped > 0)
                {
                    Log("Skipped renaming " + skipped + (skipped > 1 ? " files" : " file"));
                }
                var failed = inputFiles.Count - renamed - duplicates - skipped;
                if (failed > 0)
                {
                    Log("Failed to rename " + failed + (failed > 1 ? " files" : " file"));
                }
            }
            else
            {
                if (renameFiles.Checked)
                {
                    Log("No files were renamed!");
                    Log("This means your files are already named correctly");
                    Log("Or I can't figure out a better name for them");
                }
            }
            if (duplicates > 0)
            {
                if (deleted > 0)
                {
                    Log("I found and deleted " + deleted + " duplicate " + (deleted > 1 ? "files" : "file") + " (sent to Recycle Bin)");
                }
                else
                {
                    Log("I found " + duplicates + " duplicate " + (duplicates > 1 ? "files" : "file"));
                }
            }

            if (!sortByAuthor.Checked && sorted > 0)
            {
                Log("Successfully sorted " + sorted + (sorted > 1 ? " files" : "file"));
            }
            else
            {
                if (tryToSortFiles.Checked)
                {
                    Log("No files were sorted!");
                    Log("This means I couldn't figure out where to place your files, sorry!");
                }
            }
            if (ExpertCount > 0)
            {
                Log("I found and sorted " + ExpertCount + " Expert-Only " + (ExpertCount > 1 ? " files" : " file"));
            }
            if (rbafiles <= 0 && midifiles <= 0) return;
            Log("Psst");

            if (rbafiles > 0)
            {
                Log("I also found " + rbafiles + " RBA " + (rbafiles > 1 ? "files" : " file"));
                Log("I moved " + (rbafiles > 1 ? "them" : "it") + " to the 'rba' folder since I can't work with " + (rbafiles > 1 ? "them" : "it"));
            }
            if (midifiles <= 0) return;
            Log("I also found " + midifiles + " MIDI " + (midifiles > 1 ? "files" : "file"));
            Log("I moved " + (midifiles > 1 ? "them" : "it") + " to the 'midi' folder since I can't work with " + (midifiles > 1 ? "them" : "it"));
        }

        private void FileRenamer_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            FinishWorkers();
        }

        private void FinishWorkers()
        {
            EnableDisable(true);
            Log("Done.");
            toolTip1.SetToolTip(btnBegin, "Click to begin renaming process");
            btnBegin.Text = "&Begin";
            btnBegin.Enabled = true;
            btnBegin.Visible = false;
        }

        private void folderScanner_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                var inFiles = new string[]{};
                txtFolder.Invoke(new MethodInvoker(() => inFiles = Directory.GetFiles(txtFolder.Text)));
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
            }
            catch (Exception ex)
            {
                Log("There was an error: " + ex.Message);
            }
        }

        private void folderScanner_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
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
            EnableDisable(true);
            txtFolder.Focus();
        }

        private void batchRenamePhaseShift_Click(object sender, EventArgs e)
        {
            var browser = new FolderPicker
            {
                InputPath = Environment.CurrentDirectory,
                Title = "Select Phase Shift music folder",
            };
            if (browser.ShowDialog(IntPtr.Zero) != true || string.IsNullOrWhiteSpace(browser.ResultPath)) return;
            Environment.CurrentDirectory = browser.ResultPath;
            Log("Batch renaming Phase Shift folders");
            Log("Scanning for song.ini files");
            var songs = Directory.GetFiles(browser.ResultPath, "*.ini", SearchOption.AllDirectories);
            if (!songs.Any())
            {
                Log("No songs found in that folder, stopping");
                return;
            }
            PhaseShiftSongs = songs.ToList();
            EnableDisable(false);
            btnBegin.Text = "Cancel";
            toolTip1.SetToolTip(btnBegin, "Click to cancel renaming process");
            btnBegin.Visible = true;
            PhaseShiftRenamer.RunWorkerAsync();
        }

        private void PhaseShiftRenamer_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Log("Found " + PhaseShiftSongs.Count() + (PhaseShiftSongs.Count() == 1 ? " song" : " songs"));
            var renamed = 0;
            for (var i = 0; i < PhaseShiftSongs.Count(); i++)
            {
                var Artist = "";
                var Song = "";
                var sr = new StreamReader(PhaseShiftSongs[i]);
                while (sr.Peek() >= 0)
                {
                    try
                    {
                        var line = sr.ReadLine();
                        if (line.Contains("artist=") || line.Contains("artist ="))
                        {
                            Artist = Tools.GetConfigString(line);
                        }
                        else if (line.Contains("name=") || line.Contains("name ="))
                        {
                            Song = Tools.GetConfigString(line);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log("Error reading INI file: " + ex.Message);
                    }
                }
                sr.Dispose();
                if (string.IsNullOrWhiteSpace(Artist) || string.IsNullOrWhiteSpace(Song)) continue;
                Log("Song #" + (i + 1) + " is '" + Artist + " - " + Song + "'");
                var new_name = arrangeName(Song, Artist, 0, "");
                if (normalizeFeaturedArtists.Checked)
                {
                    new_name = Tools.FixFeaturedArtist(new_name);
                }
                if (replaceSpacesWithUnderscores.Checked)
                {
                    new_name = new_name.Replace(" ", "_");
                }
                else if (removeSpacesFromFileName.Checked)
                {
                    new_name = new_name.Replace(" ", "");
                }
                new_name = Tools.CleanString(new_name, false).TrimStart().TrimEnd('?', '.', ',', ' ');
                var song_folder = Path.GetDirectoryName(PhaseShiftSongs[i]);
                var root_folder = Directory.GetParent(song_folder);
                var new_folder = root_folder + "\\" + new_name;
                Log("New folder name is '" + new_name + "'");
                if (new_folder == song_folder)
                {
                    Log("Folder is already in the desired format, skipping");
                    continue;
                }
                if (Directory.Exists(new_folder))
                {
                    Log("There is already a folder with that name, appending (2) to avoid overwriting");
                    new_folder += "(2)";
                }
                try
                {
                    Log("Renaming folder from '" + song_folder.Replace(root_folder + "\\", "") + "' to '" + new_folder.Replace(root_folder + "\\", "") + "'");
                    Directory.Move(song_folder, new_folder);
                    Log("Success");
                    renamed++;
                }
                catch (Exception ex)
                {
                    Log("Failed. Error: " + ex.Message);
                }
            }
            if (renamed == 0)
            {
                Log("No folders were renamed");
            }
            else
            {
                Log("Renamed " + renamed + (renamed == 1? " folder" : " folders"));
            }
        }

        private void PhaseShiftRenamer_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            FinishWorkers();
        }

        private sealed class DarkRenderer : ToolStripProfessionalRenderer
        {
            public DarkRenderer() : base(new DarkColors()) { }
        }

        private sealed class DarkColors : ProfessionalColorTable
        {
            public override Color ImageMarginGradientBegin
            {
                get { return mMenuBackground; }
            }
            public override Color ImageMarginGradientEnd
            {
                get { return mMenuBackground; }
            }
            public override Color ImageMarginGradientMiddle
            {
                get { return mMenuBackground; }
            }
            public override Color ToolStripDropDownBackground
            {
                get { return mMenuBackground; }
            }
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

        private void internalName_Click(object sender, EventArgs e)
        {
            renameInternalName.Checked = false;
            renameTheArtistSong.Checked = false;
            renameArtistTheSong.Checked = false;
            renameSongTheArtist.Checked = false;
            renameSongArtistThe.Checked = false;
            renameYearArtist.Checked = false;
            renameYearSong.Checked = false;
            ezloDQMode.Checked = false;
            ((ToolStripMenuItem) sender).Checked = true;
        }

        private void sortByAuthor_Click(object sender, EventArgs e)
        {
            if (sortByAuthor.Checked) 
            {
                tryToSortFiles.Checked = false;
                tryDetailedSubsorting.Enabled = false;
            }
        }

        private void ignoreXboxFilesystemLimitations_Click(object sender, EventArgs e)
        {
            fullNameNoAccents.Checked = false;
        }

        private void fullNameNoAccents_Click(object sender, EventArgs e)
        {
            ignoreXboxFilesystemLimitations.Checked = false;
        }
    }
}
