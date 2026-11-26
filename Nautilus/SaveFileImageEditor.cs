using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using Nautilus.Properties;
using Nautilus.x360;

namespace Nautilus
{
    public partial class SaveFileImageEditor : Form
    {
        private readonly NemoTools Tools;
        private string ImageFolder;
        private readonly string EditorFolder;
        private bool isPS3;
        private bool isWii;
        private string console;
        private readonly string wimgt;
        private string UserSaveFile;

        public SaveFileImageEditor(Color ButtonBackColor, Color ButtonTextColor)
        {
            InitializeComponent();
            
            wimgt = Application.StartupPath + "\\bin\\wimgt.exe";
            EditorFolder = Application.StartupPath + "\\editor\\";
            if (!Directory.Exists(EditorFolder))
            {
                Directory.CreateDirectory(EditorFolder);
            }
            Tools = new NemoTools();

            for (var i = 1; i < 6; i++)
            {
                if (File.Exists(Application.StartupPath + "\\res\\bg" + i + ".png"))
                {
                    cboBackgrounds.Items.Add("Background " + i);
                }
            }
            cboBackgrounds.SelectedIndex = 0;

            picCharacter.AllowDrop = true;
            picArt.AllowDrop = true;

            var formButtons = new List<Button> { btnExportArt, btnExportChar, btnReplaceArt, btnReplaceChar, btnRename };
            foreach (var button in formButtons)
            {
                button.BackColor = ButtonBackColor;
                button.ForeColor = ButtonTextColor;
                button.FlatAppearance.MouseOverBackColor = button.BackColor == Color.Transparent ? Color.FromArgb(127, Color.AliceBlue.R, Color.AliceBlue.G, Color.AliceBlue.B) : Tools.LightenColor(button.BackColor);
            }

            Tools.CurrentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            console = "";
            Tools.SaveFileCharNames = new List<string>();
            Log("Welcome to the Save File Image Editor");
            Log("Click 'File' -> 'Open file' to select a save game file");
            Log("Or drag and drop it anywhere on this form");
            Log("Ready");
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
            if (Path.GetExtension(files[0]).ToLowerInvariant() != ".dat" && VariousFunctions.ReadFileType(files[0]) != XboxFileType.STFS) return;
            if (!CanClose()) return;
            ExtractFromSaveFile(files[0]);
        }

        private bool CanClose()
        {
            if (!Text.Contains("*"))
            {
                return true;
            }
            return MessageBox.Show("You have unsaved changes\nAre you sure you want to do that?",Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.No;
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

        private void ClearAll()
        {
            cboCharacter.Enabled = false;
            cboArt.Enabled = false;
            picArt.Image = null;
            picCharacter.Image = null;
            cboCharacter.Items.Clear();
            cboArt.Items.Clear();
            cboCharacter.Text = "";
            cboCharacter.SelectedIndex = -1;
            cboArt.Text = "";
            cboArt.SelectedIndex = -1;
            exportAllImagesToolStripMenuItem.Enabled = false;
            saveChangesToFileToolStripMenuItem.Enabled = false;
            closeFileToolStripMenuItem.Enabled = false;
            btnExportArt.Enabled = false;
            btnExportChar.Enabled = false;
            btnReplaceArt.Enabled = false;
            btnReplaceChar.Enabled = false;
            txtBand.Enabled = false;
            picCharacter.BackgroundImage = null;
            cboBackgrounds.SelectedIndex = 0;
            toolTip1.SetToolTip(picCharacter, "Your character images will be displayed here");
            toolTip1.SetToolTip(picArt, "Your art images will be displayed here");
            NeedsToSave(false);
            Text = Text.Replace("*","").Trim();
            isWii = false;
            isPS3 = false;
            console = "";
            Tools.SaveFileBandName = "";
            Tools.SaveFileCharNames.Clear();
            txtBand.Text = "";
            lblFileName.Text = "";
            lblConsole.Text = "";
            lblCharacter.Text = "";
            lblArt.Text = "";
        }

        private void ExtractFromSaveFile(string file)
        {
            ClearAll();
            isWorking(true);
            var savefile = file;
            var isXbox = Path.GetFileName(file).ToLowerInvariant().Contains("xbox") || Path.GetFileName(file) == "band3";
            isWii = Path.GetFileName(file).ToLowerInvariant().Contains("wii");
            isPS3 = Path.GetFileName(file).ToLowerInvariant().Contains("ps3");
            Tools.CurrentFolder = Path.GetDirectoryName(file);
            UserSaveFile = file;

            if (VariousFunctions.ReadFileType(file) == XboxFileType.STFS)
            {
                isXbox = true;
                isPS3 = false;
                isWii = false;

                Log("Received save CON file, extracting save.dat file");
                var package = new STFSPackage(file);
                if (!package.ParseSuccess)
                {
                    isWorking(false);
                    package.CloseIO();
                    Log("Couldn't read that CON file, aborting");
                    Log("Ready");
                    return;
                }
                var xent = package.GetFile("save.dat");
                if (xent == null)
                {
                    isWorking(false);
                    package.CloseIO();
                    Log("Couldn't find save.dat in that CON file, aborting");
                    Log("Ready");
                    return;
                }
                var tempdat = EditorFolder + "\\temp.dat";
                Tools.DeleteFile(tempdat);
                if (!xent.ExtractToFile(tempdat))
                {
                    isWorking(false);
                    package.CloseIO();
                    Log("Couldn't extract save.dat from that CON file, aborting");
                    Log("Ready");
                    return;
                }
                savefile = tempdat;
                package.CloseIO();
            }
            Log("Received save file, extracting images...");

            ImageFolder = EditorFolder + Path.GetFileNameWithoutExtension(file) + "_extracted\\";
            Tools.DeleteFolder(ImageFolder,true);
            if (!Directory.Exists(ImageFolder))
            {
                Directory.CreateDirectory(ImageFolder);
            }

            var success = false;
            if (isXbox)
            {
                success = Tools.ExtractSaveImages(savefile, EditorFolder + Path.GetFileNameWithoutExtension(file));
            }
            else if (isWii)
            {
                success = Tools.ExtractWiiSaveImages(savefile, ImageFolder);// EditorFolder + Path.GetFileNameWithoutExtension(file));
            }
            else if (isPS3)
            {
                var offset = offsetFix.Checked? 1 : 0;
                success = Tools.ExtractSaveImages(savefile, EditorFolder + Path.GetFileNameWithoutExtension(file), true, offset);
            }
            else
            {
                switch (MessageBox.Show("Click Yes if this is a *decrypted* PS3 save file\nClick No if this is a Wii save file\n" +
                    "Xbox 360 users must use the 'band3' save file\nPress Cancel to go back",
                            Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case DialogResult.Yes:
                        isPS3 = true;
                        isWii = false;
                        success = Tools.ExtractSaveImages(savefile, EditorFolder + Path.GetFileNameWithoutExtension(file), true); // EditorFolder + Path.GetFileNameWithoutExtension(file), true);
                        break;
                    case DialogResult.No:
                        isWii = true;
                        isPS3 = false;
                        success = Tools.ExtractWiiSaveImages(savefile, ImageFolder); // EditorFolder + Path.GetFileNameWithoutExtension(file));
                        break;
                    case DialogResult.Cancel:
                        Log("Extracting cancelled by user");
                        isWorking(false);
                        Log("Ready");
                        return;
                }
            }
            
            if (!success)
            {
                Log("Extracting from save game file failed");
                isWorking(false);
                Log("Ready");
                return;
            }
            Log("Extracted images from save file successfully");

            if (savefile != file) //i.e. extracted from con
            {
                Tools.DeleteFile(savefile);
            }

            lblFileName.Text = Path.GetFileName(file);
            lblConsole.Text = isPS3 ? "Playstation 3" : (isWii ? "Wii" : "Xbox 360");
            ChangeImagePadding();
            LoadExtractedImages();
            exportAllImagesToolStripMenuItem.Enabled = true;
            closeFileToolStripMenuItem.Enabled = true;

            txtBand.Enabled = true;
            txtBand.Text = !string.IsNullOrWhiteSpace(Tools.SaveFileBandName) ? Tools.SaveFileBandName : "Unknown";
            txtBand.ReadOnly = isWii;

            isWorking(false);
            Log("Ready");
        }

        private void LoadExtractedImages()
        {
            console = isPS3 ? "ps3" : (isWii ? "wii" : "xbox");
            var files = Directory.GetFiles(ImageFolder, "*.png_" + console, SearchOption.TopDirectoryOnly).ToList();
               
            if (!files.Any()) return;

            var art_counter = 0;
            var char_counter = 0;
            foreach (var file in files)
            {
                if (file.Contains("character"))
                {
                    char_counter++;
                }
                else if (file.Contains("art"))
                {
                    art_counter++;
                }
            }

            if (Tools.SaveFileCharNames.Any())
            {
                for (var i = 0; i < Tools.SaveFileCharNames.Count; i++)
                {
                    var file = ImageFolder + "character_" + (i + 1) + ".png_" + console;
                    if (File.Exists(file))
                    {
                        cboCharacter.Items.Add(Tools.SaveFileCharNames[i].Trim() != "" ? Tools.SaveFileCharNames[i] : "character_" + (i + 1));
                    }
                }
            }
            else
            {
                for (var i = 0; i < char_counter; i++)
                {
                    var file = ImageFolder + "character_" + (i + 1) + ".png_" + console;
                    var name = Path.GetFileNameWithoutExtension(file);
                    if (File.Exists(file) && !string.IsNullOrWhiteSpace(name))
                    {
                        cboCharacter.Items.Add(name);
                    }
                }
            }

            for (var i = 0; i < art_counter; i++)
            {
                var file = ImageFolder + "art_" + (i + 1) + ".png_" + console;
                var name = Path.GetFileNameWithoutExtension(file);
                if (File.Exists(file) && !string.IsNullOrWhiteSpace(name))
                {
                    cboArt.Items.Add(name);
                }
            }

            if (cboCharacter.Items.Count > 0)
            {
                cboCharacter.Enabled = true;
                cboCharacter.SelectedIndex = 0;
                toolTip1.SetToolTip(picCharacter, "Drag and drop an image here to replace this character image");
            }
            if (cboArt.Items.Count > 0)
            {
                cboArt.Enabled = true;
                cboArt.SelectedIndex = 0;
                toolTip1.SetToolTip(picArt, "Drag and drop an image here to replace this art image");
            }

            lblCharacter.Text = cboCharacter.Items.Count.ToString(CultureInfo.InvariantCulture);
            lblArt.Text = cboArt.Items.Count.ToString(CultureInfo.InvariantCulture);
            Log("Loaded " + cboCharacter.Items.Count + " character " + (cboCharacter.Items.Count == 1 ? "image" : "images"));
            Log("Loaded " + cboArt.Items.Count + " art " + (cboArt.Items.Count == 1 ? "image" : "images"));
        }

        private void openSaveGameFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
                {
                    Title = "Select Rock Band 3 save game file",
                    Multiselect = false,
                    InitialDirectory = Tools.CurrentFolder,
                    Filter = "X360 Save File|band3|PS3 Save File|*.dat|Wii Save File|*.dat",
                };

            ofd.ShowDialog();
            if (string.IsNullOrWhiteSpace(ofd.FileName)) return;
            Tools.CurrentFolder = Path.GetDirectoryName(ofd.FileName);
            if (Path.GetExtension(ofd.FileName) != ".dat" && VariousFunctions.ReadFileType(ofd.FileName) != XboxFileType.STFS)
            {
                MessageBox.Show("That's not a valid file to open here", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            ExtractFromSaveFile(ofd.FileName);
        }

        private void cboCharacter_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnExportChar.Enabled = cboCharacter.SelectedIndex > -1;
            btnReplaceChar.Enabled = cboCharacter.SelectedIndex > -1 && !isWii;

            if (cboCharacter.SelectedIndex < 0) return;

            var image = ImageFolder + "character_" + (cboCharacter.SelectedIndex + 1) + ".png";
            picCharacter.Image = Tools.NemoLoadImage(image);
        }

        private void cboArt_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnExportArt.Enabled = cboArt.SelectedIndex > -1;
            btnReplaceArt.Enabled = cboArt.SelectedIndex > -1 && !isWii;

            if (cboArt.SelectedIndex < 0) return;

            var image = ImageFolder + "art_" + (cboArt.SelectedIndex + 1) + ".png";
            picArt.Image = Tools.NemoLoadImage(image);
        }

        private void exportAllImagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ofd = new FolderPicker
            {
                InputPath = Tools.CurrentFolder,
                Title = "Select folder to export images to",
            };
            if (ofd.ShowDialog(IntPtr.Zero) != true) return;
            Tools.CurrentFolder = ofd.ResultPath;
            Log("Exporting all images...");

            for (var i = 0; i < cboCharacter.Items.Count; i++)
            {
                var old_file = ImageFolder + "character_" + (i + 1) + ".png";
                var new_file = Tools.CurrentFolder + "\\" + Path.GetFileName(old_file);
                try
                {
                    Tools.DeleteFile(new_file);
                    File.Copy(old_file, new_file);
                }
                catch (Exception ex)
                {
                    Log("Error exporting file " + Path.GetFileName(new_file));
                    Log("Error says: " + ex.Message);
                }
            }
            for (var i = 0; i < cboArt.Items.Count; i++)
            {
                var old_file = ImageFolder + "art_" + (i + 1) + ".png";
                var new_file = Tools.CurrentFolder + "\\" + Path.GetFileName(old_file);
                try
                {
                    Tools.DeleteFile(new_file);
                    File.Copy(old_file, new_file);
                }
                catch (Exception ex)
                {
                    Log("Error exporting file " + Path.GetFileName(new_file));
                    Log("Error says: " + ex.Message);
                }
            }
            Log("Exporting complete");
            Log("Ready");
        }

        private void exportLogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.ExportLog(Text, lstLog.Items);
        }

        private void btnExportChar_Click(object sender, EventArgs e)
        {
            var ofd = new FolderPicker
            {
                InputPath = Tools.CurrentFolder,
                Title = "Select folder to export image to",
            };
            if (ofd.ShowDialog(IntPtr.Zero) != true) return;
            Tools.CurrentFolder = ofd.ResultPath;
            Log("Exporting image for character '" + cboCharacter.Items[cboCharacter.SelectedIndex] + "'");

            var old_file = ImageFolder + "character_" + (cboCharacter.SelectedIndex + 1) + ".png";
            var new_file = Tools.CurrentFolder + "\\" + cboCharacter.Items[cboCharacter.SelectedIndex].ToString().Replace("\"", "") + ".png";
            try
            {
                Tools.DeleteFile(new_file);
                File.Copy(old_file,new_file);
                Log("Exported image for character '" + cboCharacter.Items[cboCharacter.SelectedIndex] + "' successfully");
            }
            catch (Exception ex)
            {
                Log("Exporting image for character '" + cboCharacter.Items[cboCharacter.SelectedIndex] + "' failed");
                Log("Error says: " + ex.Message);
            }
            Log("Ready");
        }

        private void btnExportArt_Click(object sender, EventArgs e)
        {
            var ofd = new FolderPicker
            {
                InputPath = Tools.CurrentFolder,
                Title = "Select folder to export image to",
            };
            if (ofd.ShowDialog(IntPtr.Zero) != true) return;
            Tools.CurrentFolder = ofd.ResultPath;
            Log("Exporting art image '" + cboArt.Items[cboArt.SelectedIndex] + "'");

            var old_file = ImageFolder + cboArt.Items[cboArt.SelectedIndex].ToString().Replace("\"","") + ".png";
            var new_file = Tools.CurrentFolder + "\\" + Path.GetFileName(old_file);
            try
            {
                Tools.DeleteFile(new_file);
                File.Copy(old_file, new_file);
                Log("Exported art image '" + cboArt.Items[cboArt.SelectedIndex] + "' successfully");
            }
            catch (Exception ex)
            {
                Log("Exporting art image '" + cboArt.Items[cboArt.SelectedIndex] + "' failed");
                Log("Error says: " + ex.Message);
            }
            Log("Ready");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnReplaceChar_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
                {
                    Title = "Select replacement character image",
                    Filter = "PNG Image|*.png",
                    InitialDirectory = Tools.CurrentFolder,
                    Multiselect = false
                };
            if (ofd.ShowDialog() != DialogResult.OK || string.IsNullOrWhiteSpace(ofd.FileName) || !File.Exists(ofd.FileName)) return;
            Tools.CurrentFolder = Path.GetDirectoryName(ofd.FileName);
            ReplaceCharacterImage(ofd.FileName);
        }

        private void NeedsToSave(bool needs)
        {
            Text = Text.Replace("*", "").Trim();
            if (needs)
            {
                 Text = "* " + Text + " *";
            }
            saveChangesToFileToolStripMenuItem.Enabled = needs;
        }

        private void ReplaceCharacterImage(string image)
        {
            Tools.CurrentFolder = Path.GetDirectoryName(image);
            console = isPS3 ? "ps3" : (isWii ? "wii" : "xbox");
            var xfile = ImageFolder + "character_" + (cboCharacter.SelectedIndex + 1) + ".png_" + console;
            var file = ImageFolder + "character_" + (cboCharacter.SelectedIndex + 1) + ".png";
            var backup = xfile + ".bak";
            File.Copy(xfile, backup);

            Tools.isSaveFileCharacter = true;
            Tools.isSaveFileArt = false;
            Tools.TextureSize = isWii? 256 : 512;
            Tools.isVerticalTexture = true;
            Tools.useDXT5 = true;

            if (isWii)
            {
                Tools.ConvertImagetoWii(wimgt, image, xfile);
            }
            else
            {
                Tools.ConvertImagetoRB(image, xfile, false, isPS3);
            }
            
            if (File.Exists(xfile))
            {
                Tools.DeleteFile(file);
                if (isWii)
                {
                    Tools.ConvertWiiImage(xfile, image, file);
                }
                else
                {
                    Tools.ConvertRBImage(xfile, file);
                }
                picCharacter.Image = Tools.NemoLoadImage(file);
                Log("Replaced image for character '" + cboCharacter.Items[cboCharacter.SelectedIndex] + "' successfully");
                NeedsToSave(true);
                Log("Ready");
                Tools.DeleteFile(backup);
                return;
            }
            Log("Replacing image for character '" + cboCharacter.Items[cboCharacter.SelectedIndex] + "' failed");

            //set everything back
            File.Move(backup, xfile);
            Tools.DeleteFile(file);
            if (isWii)
            {
                Tools.ConvertImagetoWii(wimgt, image, xfile);
            }
            else
            {
                Tools.ConvertImagetoRB(image, xfile, false, isPS3);
            }
            ChangeImagePadding();
            picCharacter.Image = Tools.NemoLoadImage(file);
        }

        private void ChangeImagePadding()
        {
            picCharacter.Padding = new Padding(isWii ? 64 : 0, isWii ? 256 : 0, 0, 0);
        }

        private void ReplaceArtImage(string image)
        {
            Tools.CurrentFolder = Path.GetDirectoryName(image);
            console = isPS3 ? "ps3" : (isWii ? "wii" : "xbox");
            var xfile = ImageFolder + "art_" + (cboArt.SelectedIndex + 1) + ".png_" + console;
            var file = ImageFolder + "art_" + (cboArt.SelectedIndex + 1) + ".png";
            var backup = xfile + ".bak";
            File.Copy(xfile, backup);

            Tools.isSaveFileCharacter = false;
            Tools.isSaveFileArt = true;
            Tools.TextureSize = isWii ? 128 : 256;
            Tools.isVerticalTexture = false;
            Tools.useDXT5 = true;

            if (isWii)
            {
                Tools.ConvertImagetoWii(wimgt, image, xfile);
            }
            else
            {
               Tools.ConvertImagetoRB(image, xfile, false, isPS3); 
            }
            

            if (File.Exists(xfile))
            {
                Tools.DeleteFile(file);
                if (isWii)
                {
                    Tools.ConvertWiiImage(xfile, image, file);
                }
                else
                {
                    Tools.ConvertRBImage(xfile, file);
                }
                picArt.Image = Tools.NemoLoadImage(file);
                Log("Replaced art image '" + cboArt.Items[cboArt.SelectedIndex] + "' successfully");
                NeedsToSave(true);
                Log("Ready");
                Tools.DeleteFile(backup);
                return;
            }

            Log("Replacing art image '" + cboArt.Items[cboArt.SelectedIndex] + "' failed");

            //set everything back
            File.Move(backup, xfile);
            Tools.DeleteFile(file);
            if (isWii)
            {
                Tools.ConvertImagetoWii(wimgt, image, xfile);
            }
            else
            {
                Tools.ConvertImagetoRB(image, xfile, false, isPS3);
            }
            ChangeImagePadding();
            picArt.Image = Tools.NemoLoadImage(file);
        }

        private void btnReplaceArt_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Title = "Select replacement art image",
                Filter = "Image Files|*.bmp;*.tif;*.dds;*.jpg;*.jpeg;*.gif;*.png",
                InitialDirectory = Tools.CurrentFolder,
                Multiselect = false
            };
            if (ofd.ShowDialog() != DialogResult.OK || string.IsNullOrWhiteSpace(ofd.FileName) || !File.Exists(ofd.FileName)) return;
            Tools.CurrentFolder = Path.GetDirectoryName(ofd.FileName);
            ReplaceArtImage(ofd.FileName);
        }

        private void SaveFileImageEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!CanClose())
            {
                e.Cancel = true;
            }
            else
            {
                Tools.DeleteFolder(EditorFolder, true);
            }
        }

        private void saveChangesToFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBand.Text))
            {
                MessageBox.Show("Band name can't be blank\nPlease enter a valid band name to continue", Text,
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtBand.Focus();
                return;
            }
            Log("Saving changes to file...");

            //make new backup
            var backup = UserSaveFile + ".bak";
            Tools.DeleteFile(backup);
            File.Copy(UserSaveFile, backup);

            var offset = offsetFix.Checked ? 1 : 0;
            if (!Tools.ReplaceSaveImages(UserSaveFile, ImageFolder, isPS3))// offset))
            {
                Log("Saving changes to file failed");
                Tools.DeleteFile(UserSaveFile);
                try
                {
                    File.Copy(backup, UserSaveFile);
                    Log("Backup was restored, nothing was lost");
                    Log("Ready");
                }
                catch (Exception ex)
                {
                    Log("Error restoring backup!");
                    Log(ex.Message);
                }
                return;
            }
            Log("Saved changes to file successfully");
            switch (console)
            {
                case "xbox":
                    Log("Just place the edited " + lblFileName.Text + " file back in your Xbox 360");
                    break;
                case "ps3":
                    Log("Make sure to encrypt the edited " + lblFileName.Text + " file before putting it back in your PS3");
                    break;
            }
            NeedsToSave(false);
            Log("Ready");
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var message = Tools.ReadHelpFile("sfie");
            var help = new HelpForm(Text + " - Help", message);
            help.ShowDialog();
        }

        private void picCharacter_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            var ext = Path.GetExtension(files[0]).ToLowerInvariant();
            if (ext == ".dat" || VariousFunctions.ReadFileType(files[0]) == XboxFileType.STFS)
            {
                if (!CanClose()) return;
                ExtractFromSaveFile(files[0]);
                return;
            }
            if (isWii)
            {
                MessageBox.Show("That feature is not available for Wii files, sorry", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (string.IsNullOrWhiteSpace(console))
            {
                return;
            }
            if (ext == ".png")
            {
                ReplaceCharacterImage(files[0]);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(console)) return;
                MessageBox.Show("You can only drop .png files here", Text, MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
            }
        }

        private void picArt_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            var ext = Path.GetExtension(files[0]).ToLowerInvariant();
            if (ext == ".dat" || VariousFunctions.ReadFileType(files[0]) == XboxFileType.STFS)
            {
                if (!CanClose()) return;
                ExtractFromSaveFile(files[0]);
                return;
            }
            if (isWii)
            {
                MessageBox.Show("That feature is not available for Wii files, sorry", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (string.IsNullOrWhiteSpace(console))
            {
                return;
            }
            if (ext == ".png" || ext == ".jpg" || ext == ".dds" || ext == ".bmp")
            {
                ReplaceArtImage(files[0]);
            }
            else
            {
                MessageBox.Show("You can only drop .png, .jpg, .dds and .bmp files here", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void closeFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CanClose()) return;
            Tools.DeleteFolder(ImageFolder, true);
            ClearAll();
        }

        private void contextMenuStrip2_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            replaceCharacterImageToolStripMenuItem.Enabled = cboCharacter.SelectedIndex > -1 && !isWii;
            exportCompositeImageToolStripMenuItem.Enabled = cboCharacter.Items.Count > 0 && cboBackgrounds.SelectedIndex > 0;
        }

        private void contextMenuStrip3_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            replaceArtImageToolStripMenuItem.Enabled = cboArt.SelectedIndex > -1 && !isWii;
        }

        private void exportCompositeImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Bitmap bitmap;
                //capture image
                var location = PointToScreen(picCharacter.Location);
                using (bitmap = new Bitmap(picCharacter.Width, picCharacter.Height))
                {
                    cboCharacter.Visible = false;
                    btnExportChar.Visible = false;
                    btnRename.Visible = false;
                    btnReplaceChar.Visible = false;
                    picCharacter.Invalidate();
                    Application.DoEvents();
                    var g = Graphics.FromImage(bitmap);
                    g.CopyFromScreen(new Point(location.X, location.Y), Point.Empty, picCharacter.Size, CopyPixelOperation.SourceCopy);
                    cboCharacter.Visible = true;
                    btnExportChar.Visible = true;
                    btnRename.Visible = true;
                    btnReplaceChar.Visible = true;

                    //this is so image quality is higher than the default
                    var myEncoder = Encoder.Quality;
                    var myEncoderParameters = new EncoderParameters(1);
                    const long encoder = 100L;
                    var myEncoderParameter = new EncoderParameter(myEncoder, encoder);
                    myEncoderParameters.Param[0] = myEncoderParameter;
                    var myImageCodecInfo = Tools.GetEncoderInfo("image/png");

                    //prepare to prompt user for save location and extension
                    var sfd = new SaveFileDialog
                    {
                        Filter = "PNG Image|*.png",
                        Title = "Where should I save the image to?",
                        FileName = cboCharacter.Items[cboCharacter.SelectedIndex] + "_composite",
                        AddExtension = true,
                        InitialDirectory = Tools.CurrentFolder
                    };

                    if (sfd.ShowDialog() != DialogResult.OK || string.IsNullOrWhiteSpace(sfd.FileName)) return;
                    Log("Exporting composite character image...");
                    Tools.DeleteFile(sfd.FileName);
                    Tools.CurrentFolder = Path.GetDirectoryName(sfd.FileName);
                    //save image
                    bitmap.Save(sfd.FileName, myImageCodecInfo, myEncoderParameters);
                }
                Log("Exported composite character image successfully");
            }
            catch (Exception ex)
            {
                Log("Exporting composite character image failed");
                Log("The error says: " + ex.Message);
            }
            Log("Ready");
        }

        private void replaceCharacterImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnReplaceChar.PerformClick();
        }

        private void replaceArtImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnReplaceArt.PerformClick();
        }

        private void cboBackgrounds_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboBackgrounds.SelectedIndex < 0) return;

            if (cboBackgrounds.SelectedIndex == 0)
            {
                picCharacter.BackgroundImage = null;
                return;
            }
            var bg = Application.StartupPath + "\\res\\bg" + cboBackgrounds.SelectedIndex + ".png";
            if (File.Exists(bg))
            {
                picCharacter.BackgroundImage = Tools.NemoLoadImage(bg);
            }
            else
            {
                cboBackgrounds.SelectedIndex = 0;
            }
        }

        private void isWorking(bool working)
        {
            var cursor = working ? Cursors.WaitCursor : Cursors.Default;

            picWorking.Visible = working;
            picCharacter.Cursor = cursor;
            picArt.Cursor = cursor;
            Cursor = cursor;
            lstLog.Cursor = cursor;
        }

        private void btnRename_Click(object sender, EventArgs e)
        {
            var name = cboCharacter.Text;
            var popup = new PasswordUnlocker(name);
            popup.Renamer(); //change settings for renaming
            popup.ShowDialog();
            var newName = popup.EnteredText;
            popup.Dispose();
            if (string.IsNullOrWhiteSpace(newName) || newName == name) return;
            if (newName.Length > 24)
            {
                newName = newName.Substring(0, 24).Trim();
            }
            cboCharacter.Items[cboCharacter.SelectedIndex] = newName;
            if (Tools.SaveFileCharNames.Count >= cboCharacter.SelectedIndex)
            {
                Tools.SaveFileCharNames[cboCharacter.SelectedIndex] = newName;
            }
            NeedsToSave(true);
        }

        private void btnReplaceChar_EnabledChanged(object sender, EventArgs e)
        {
            btnRename.Enabled = btnReplaceChar.Enabled;
        }

        private void txtBand_TextChanged(object sender, EventArgs e)
        {
            if (txtBand.Text.Trim() == Tools.SaveFileBandName) return;
            Tools.SaveFileBandName = txtBand.Text.Trim();
            NeedsToSave(true);
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
    }
}
