using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Nautilus.Properties;
using Nautilus.x360;
using DevComponents.AdvTree;
using DevComponents.DotNetBar;
using System.Drawing;
using NautilusFREE;

namespace Nautilus
{
    public partial class CONCreator : Office2007Form
    {
        private static string sFolder;
        private readonly CreateSTFS xSession;
        private string xOut;
        private RSAParams signature;
        private readonly NemoTools Tools;
        private string Description;
        private string Title;
        private int GameIndex;
        private readonly List<string> moggs;
        private readonly nTools nautilus3;
 
        public CONCreator(Color ButtonBackColor, Color ButtonTextColor)
        {
            InitializeComponent();

            Tools = new NemoTools();
            var y = new List<PackageType>();
            nautilus3 = new nTools();

            picContent.AllowDrop = true;
            picPackage.AllowDrop = true;

            btnCreate.BackColor = ButtonBackColor;
            btnCreate.ForeColor = ButtonTextColor;
            btnCreate.FlatAppearance.MouseOverBackColor = btnCreate.BackColor == Color.Transparent ? Color.FromArgb(127, Color.AliceBlue.R, Color.AliceBlue.G, Color.AliceBlue.B) : Tools.LightenColor(btnCreate.BackColor);
            
            moggs = new List<string>();
            xSession = new CreateSTFS();
            var x = (PackageType[])Enum.GetValues(typeof(PackageType));
            y.AddRange(x);
            node1.DataKey = (ushort)0xFFFF;
            node1.NodeClick += xReturn_NodeClick;
            folderTree.SelectedIndex = 0;    
            
            cboGameID.SelectedIndex = 5;
            toolTip1.SetToolTip(picContent, "Click here to select the Content Image (visible in here)");
            toolTip1.SetToolTip(picPackage, "Click here to select the Package Image (visible in the Xbox dashboard)");
            toolTip1.SetToolTip(btnCreate, "Click here to create the song package");
            toolTip1.SetToolTip(radioCON, "Click here for use with retail consoles");
            toolTip1.SetToolTip(radioLIVE, "Click here for use with modded consoles");
            toolTip1.SetToolTip(txtDisplay, "Enter a title for your pack (visible in the Xbox dashboard)");
            toolTip1.SetToolTip(txtDescription, "Enter a description for your pack (visible in here)");
            toolTip1.SetToolTip(folderTree, "Add folders here");
            toolTip1.SetToolTip(fileList, "Add files here");
            toolTip1.SetToolTip(groupBox1, "Choose the format for your pack - default is CON");
            AddFolder("songs", 0);
        }

        private DevComponents.AdvTree.Node EnsurePackageFolder(string folderPath)
        {
            var parts = folderPath.Split(
                new[] { '/' },
                StringSplitOptions.RemoveEmptyEntries);

            var parent = folderTree.Nodes[0];
            string currentPath = "";

            foreach (var part in parts)
            {
                currentPath = string.IsNullOrEmpty(currentPath)
                    ? part
                    : currentPath + "/" + part;

                var entry = xSession.GetFolder(currentPath);

                if (entry == null)
                {
                    if (!xSession.AddFolder(currentPath))
                    {
                        throw new IOException(
                            "Could not create folder: " + currentPath);
                    }

                    entry = xSession.GetFolder(currentPath);
                }

                if (entry == null)
                {
                    throw new IOException(
                        "Could not retrieve folder: " + currentPath);
                }

                DevComponents.AdvTree.Node existingNode = null;

                foreach (DevComponents.AdvTree.Node node in parent.Nodes)
                {
                    var tag = node.Tag as CFolderEntry;

                    if (tag != null &&
                        string.Equals(tag.Path, currentPath,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        existingNode = node;
                        break;
                    }
                }

                if (existingNode == null)
                {
                    existingNode = GetFoldNode(entry);
                    parent.Nodes.Add(existingNode);
                }

                parent = existingNode;
            }

            return parent;
        }

        private bool ImportPackageFile(string file, string folder)
        {
            if (!File.Exists(file))
                return false;

            string xPath = folder + "/" + Path.GetFileName(file);

            if (xSession.GetFile(xPath) != null)
                return false;

            if (Path.GetExtension(file).Equals(
                ".mogg", StringComparison.OrdinalIgnoreCase))
            {
                nautilus3.WriteOutData(
                    nautilus3.DeObfM(File.ReadAllBytes(file)), file);

                moggs.Add(file);
            }

            if (!xSession.AddFile(file, xPath))
            {
                throw new IOException(
                    "Could not add file: " + xPath);
            }

            btnCreate.Enabled = true;
            return true;
        }

        private void ImportPackageFolder(
    string sourceFolder, string destinationFolder)
        {
            EnsurePackageFolder(destinationFolder);

            foreach (var file in Directory.GetFiles(sourceFolder))
            {
                ImportPackageFile(file, destinationFolder);
            }

            foreach (var subfolder in Directory.GetDirectories(sourceFolder))
            {
                string subfolderName = Path.GetFileName(subfolder);

                ImportPackageFolder(
                    subfolder,
                    destinationFolder + "/" + subfolderName);
            }
        }

        private void AddFolder(string folder, int node_depth = 0)
        {
            if (xSession.GetFolder(folder) != null)
            {
                MessageBox.Show("There is already a folder with the name '" + folder + "'\nYou can't have multiple folders with the same name,\ntry deleting the existing folder first",
                    Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);                
                return;
            }

            string xPath;
            if (folderTree.SelectedNode != folderTree.Nodes[0])
            {
                xPath = ((CFolderEntry)folderTree.SelectedNode.Tag).Path + "/" + folder;
            }
            else
            {
                xPath = folder;
            }

            if (!xSession.AddFolder(xPath))
            {
                return;
            }

            folderTree.SelectedNode.Nodes.Add(GetFoldNode(xSession.GetFolder(xPath)));
            folderTree.SelectedNode.ExpandAll();
            switch (node_depth)
            {
                case 1: //main song folder
                    folderTree.SelectedNode = folderTree.Nodes[0].Nodes[0].LastNode; 
                    break;
                case 2: //gen folder
                    folderTree.SelectedNode = folderTree.Nodes[0].Nodes[0].Nodes[0].LastNode;
                    break;
                default: //songs.dta folder
                    folderTree.SelectedNode = folderTree.Nodes[0].LastNode;  
                    break;
            }                 
        }
        
        private void addFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog {Multiselect = true, Title = "Choose files to add"};
            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            foreach (var file in ofd.FileNames)
            {
                GetFiles(file);
                Tools.CurrentFolder = Path.GetDirectoryName(file);
            }
        }

        private void GetFiles(string file, string folder = "")
        {
            if (!File.Exists(file)) return;

            if (string.IsNullOrEmpty(folder))
            {
                folder = folderTree.SelectedNode.Text;
            }

            var xPath = "";
            try
            {
                xPath = ((CFolderEntry)folderTree.SelectedNode.Tag).Path + "/" + Path.GetFileName(file);
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error on the GetFiles function\nError says:\n\n" + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            if (xSession.GetFile(xPath) == null)
            {
                if (Path.GetExtension(file) == ".mogg")
                {
                    nautilus3.WriteOutData(nautilus3.DeObfM(File.ReadAllBytes(file)), file);
                    moggs.Add(file);
                }

                if (xSession.AddFile(file, xPath))
                {
                    GetSelFiles((CFolderEntry) folderTree.SelectedNode.Tag);
                    btnCreate.Enabled = true;
                    return;
                }

                var ent = xSession.GetFile(xPath);
                var xItem = new ListViewItem(ent.Name) {Tag = ent};
                fileList.Items.Add(xItem);
                GetSelFiles((CFolderEntry) folderTree.SelectedNode.Tag);
            }
            else
            {
                MessageBox.Show("File with name '" + Path.GetFileName(file) +
                    "' already exists\nTry deleting the existing file first", Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void GetSelFiles(CFolderEntry folder)
        {
            if (folder == null) return;           
            folderTree.Enabled = fileList.Enabled = false;
            var x = folder.GetFiles();
            fileList.Items.Clear();
            fileList.Items.AddRange(x.Select(y => new ListViewItem(y.Name) {Tag = y}).ToArray());
            folderTree.Enabled = fileList.Enabled = true;
        }

        private Node GetFoldNode(CItemEntry x)
        {
            var xReturn = new Node {Text = x.Name, Tag = x, ContextMenu = contextMenuStrip3};
            xReturn.NodeClick += xReturn_NodeClick;
            return xReturn;
        }

        private void xReturn_NodeClick(object sender, EventArgs e)
        {
            var x = (Node) sender;
            if (folderTree.Nodes[0] != x)
            {
                GetSelFiles((CFolderEntry) x.Tag);
            }
            else
            {
                GetSelFiles(xSession.RootPath);
            }
        }

        private void addFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sUsed = new StringCollection();
            foreach (Node node in folderTree.SelectedNode.Nodes)
            {
                sUsed.Add(node.Text);
            }
            var newName = "";
            while (string.IsNullOrWhiteSpace(newName) || sUsed.Contains(newName))
            {
                var popup = new PasswordUnlocker();
                popup.Renamer(); //change settings for renaming
                popup.ShowDialog();
                newName = popup.EnteredText;
                popup.Dispose();
                if (string.IsNullOrWhiteSpace(newName)) return;
                if (sUsed.Contains(newName))
                {
                    MessageBox.Show("That folder name is already used, try a different name", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            AddFolder(newName);
            GetSelFiles((CFolderEntry)(folderTree.FindNodeByText(Path.GetFileName(newName))).Tag);
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            addFileToolStripMenuItem.Enabled = folderTree.SelectedNode != null;
            deleteToolStripMenuItem.Enabled = fileList.SelectedIndices.Count > 0;
        }

        private void deleteFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this folder and its contents?", "Warning!",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes) return;
            var x = (CFolderEntry) folderTree.SelectedNode.Tag;
            var folderexists = xSession.GetFolder(x.Path);

            if (folderexists == null) return;
            if (!xSession.DeleteFolder(x.Path))
            {
                MessageBox.Show("didn't work");
            }
            folderTree.SelectedNode.Remove();
            folderTree.SelectedIndex = 0;

            GetSelFiles(x);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileList.SelectedIndices.Count == 0)
            {
                return;
            }

            for (var i = 0; i < fileList.SelectedItems.Count;i++ )
            {
                var x = (CFileEntry)fileList.SelectedItems[i].Tag;
                xSession.DeleteFile(x.Path);
                fileList.Items.Remove(fileList.SelectedItems[0]);
                deleteToolStripMenuItem_Click(sender,e);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(sFolder))
            {
                sFolder = Directory.GetCurrentDirectory();
            }
            var filename = txtDisplay.Text.Replace(" ", "").Replace(",", "").Replace("!", "").Replace("?", "").Replace("+", "").Replace(">", "")
                .Replace("<", "").Replace("\\", "").Replace("/", ""); 
            var fileOutput = new SaveFileDialog { FileName = filename, InitialDirectory = sFolder };
            if (fileOutput.ShowDialog() == DialogResult.OK)
            {
                xOut = fileOutput.FileName;
            }
            if (xOut == null) return;
            Tools.CurrentFolder = Path.GetDirectoryName(xOut);
            Description = txtDescription.Text;
            Title = txtDisplay.Text;
            GameIndex = cboGameID.SelectedIndex;
            backgroundWorker1.RunWorkerAsync();
        }
        
        private void GetImage(String file, PictureBox box)
        {
            if (picWorking.Visible) return;
            try
            {
                string contentImage = null;
                //if not passed a string path for the image
                //show dialog box to find one
                if (string.IsNullOrWhiteSpace(file))
                {
                    var openFileDialog1 = new OpenFileDialog
                        {
                            Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png",
                            Title = "Select an image",
                            InitialDirectory = Application.StartupPath + "\\res\\thumbs"
                        };
                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        contentImage = openFileDialog1.FileName;
                        Tools.CurrentFolder = Path.GetDirectoryName(openFileDialog1.FileName);
                    }
                }
                else
                {
                    //if file is not blank, then use that for the image
                    if ((file.Contains(".jpg") || file.Contains(".bmp") ||
                        file.Contains(".png") || file.Contains(".jpeg")) && !file.Contains(".png_xbox") && !file.Contains(".png_wii"))
                    {
                        contentImage = file;
                    }
                    else
                    {
                        return;
                    }
                }
                if (string.IsNullOrWhiteSpace(contentImage)) return;
                var thumbnail = Tools.NemoLoadImage(contentImage);
                if (thumbnail.Width == 64 && thumbnail.Height == 64)
                {
                    box.Image = thumbnail;
                    return;
                }
                var newimage = Path.GetTempPath() + Path.GetFileNameWithoutExtension(contentImage) + ".png";
                Tools.ResizeImage(contentImage, 64, "png", newimage);
                if (File.Exists(newimage))
                {
                    box.Image = Tools.NemoLoadImage(newimage);
                }
                Tools.DeleteFile(newimage);
            }
            catch (Exception iSuck)
            {
                MessageBox.Show("Error: " + iSuck.Message);
            }
        }
        
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            xSession.HeaderData.ContentImageBinary = picContent.Image.ImageToBytes(ImageFormat.Png);
            xSession.HeaderData.PackageImageBinary = picPackage.Image.ImageToBytes(ImageFormat.Png);
            var title = "";
            switch (GameIndex)
            {
                case 0:
                    xSession.HeaderData.TitleID = 0x45410829;
                    title = "Rock Band 1";
                    break;
                case 1:
                    xSession.HeaderData.TitleID = 0x45410869;
                    title = "Rock Band 2";
                    break;
                case 2:
                    xSession.HeaderData.TitleID = 0x454108B1;
                    title = "The Beatles: Rock Band";
                    break;
                case 3:
                    xSession.HeaderData.TitleID = 0x575207F0;
                    title = "LEGO Rock Band";
                    break;
                case 4:
                    xSession.HeaderData.TitleID = 0x454108E7;
                    title = "Green Day: Rock Band";
                    break;
                case 5:
                    xSession.HeaderData.TitleID = 0x45410914;
                    title = "Rock Band 3";
                    break;
                case 6 :
                    xSession.HeaderData.TitleID = 0x415607E7;
                    title = "Guitar Hero II";
                    break;
                case 7:
                    xSession.HeaderData.TitleID = 0x415607F7;
                    title = "Guitar Hero III";
                    break;
                case 8:
                    xSession.HeaderData.TitleID = 0x4156081A;
                    title = "Guitar Hero: World Tour";
                    break;
                case 9:
                    xSession.HeaderData.TitleID = 0x41560840;
                    title = "Guitar Hero 5";
                    break;
                case 10:
                    xSession.HeaderData.TitleID = 0x41560883;
                    title = "Guitar Hero: Warriors of Rock";
                    break;
                case 11:
                    xSession.HeaderData.TitleID = 0x545607D3;                    
                    title = "Dance Central";
                    break;
                case 12:
                    xSession.HeaderData.TitleID = 0x373307D2;
                    title = "Dance Central 2";
                    break;
                case 13:
                    xSession.HeaderData.TitleID = 0x373307D9;
                    title = "Dance Central 3";
                    break;
                case 14:
                    xSession.HeaderData.TitleID = 0x5841122D;
                    title = "Rock Band Blitz";
                    break;
            }
            xSession.HeaderData.Publisher = "";
            xSession.HeaderData.Title_Package = title;
            xSession.HeaderData.SetLanguage(Languages.English);
            xSession.HeaderData.Title_Display = Title;
            xSession.HeaderData.Description = Description;
            xSession.STFSType = STFSType.Type0;
            xSession.HeaderData.MakeAnonymous();
            xSession.HeaderData.ThisType = radioCON.Checked ? PackageType.SavedGame : PackageType.MarketPlace;
            signature = radioCON.Checked ? new RSAParams(Application.StartupPath + "\\bin\\KV.bin") : new RSAParams(StrongSigned.LIVE);
            var xy = new STFSPackage(xSession, signature, xOut);
            xy.CloseIO();
            //now open and unlock
            if (!Tools.UnlockCON(xOut))
            {
                MessageBox.Show("There was an error unlocking CON file\nCan't finish", Text, MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            //convert to CON if button checked, if not, leave as LIVE
            if (!radioCON.Checked) return;
            if (!Tools.SignCON(xOut))
            {
                MessageBox.Show("There was an error signing CON file\nCan't finish", Text, MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            picWorking.Visible = false;            
            var xExplorer = new CONExplorer(Color.FromArgb(34, 169, 31), Color.White);
            xExplorer.LoadCON(xOut);
            Close();
            xExplorer.Show();
        }
        
        private void cboGameID_SelectedIndexChanged(object sender, EventArgs e)
        {
            radioCON.Checked = true;
            switch (cboGameID.SelectedIndex)
            {
                case 0:
                    xSession.HeaderData.TitleID = 0x45410829; //RB1
                    picContent.Image = Resources.RB1;
                    break;
                case 1:
                    xSession.HeaderData.TitleID = 0x45410869; //RB2
                    picContent.Image = Resources.RB2;
                    break;
                case 2:
                    xSession.HeaderData.TitleID = 0x454108B1; //TBRB
                    picContent.Image = Resources.TBRB;
                    radioLIVE.Checked = true;
                    break;
                case 3:
                    xSession.HeaderData.TitleID = 0x575207F0; //LRB
                    picContent.Image = Resources.RBL;
                    break;
                case 4:
                    xSession.HeaderData.TitleID = 0x454108E7; //GDRB
                    picContent.Image = Resources.GDRB;
                    radioLIVE.Checked = true;
                    break;
                case 5:
                    xSession.HeaderData.TitleID = 0x45410914; //RB3
                    picContent.Image = Resources.RB3;
                    break;
                case 6:
                    xSession.HeaderData.TitleID = 0x415607E7; //GHII
                    picContent.Image = Resources.GH2;
                    radioLIVE.Checked = true;
                    break;
                case 7:
                    xSession.HeaderData.TitleID = 0x415607F7; //GHIII
                    picContent.Image = Resources.GH3;
                    radioLIVE.Checked = true;
                    break;
                case 8:
                    xSession.HeaderData.TitleID = 0x4156081A; //GH4WT
                    picContent.Image = Resources.GH4WT;
                    radioLIVE.Checked = true;
                    break;
                case 9:
                    xSession.HeaderData.TitleID = 0x41560840; //GH5
                    picContent.Image = Resources.GH5;
                    radioLIVE.Checked = true;
                    break;
                case 10:
                    xSession.HeaderData.TitleID = 0x41560840; //GHWOR
                    picContent.Image = Resources.GH6WOR;
                    radioLIVE.Checked = true;
                    break;
                case 11:
                    xSession.HeaderData.TitleID = 0x545607D3; //Dance Central
                    picContent.Image = Resources.dc1;
                    radioLIVE.Checked = true;
                    break;
                case 12:
                    xSession.HeaderData.TitleID = 0x373307D2; //Dance Central 2
                    picContent.Image = Resources.dc2;
                    radioLIVE.Checked = true;
                    break;
                case 13:
                    xSession.HeaderData.TitleID = 0x373307D9; //Dance Central 3
                    picContent.Image = Resources.dc3;
                    radioLIVE.Checked = true;
                    break;
                case 14:
                    xSession.HeaderData.TitleID = 0x5841122D; //Rock Band Blitz
                    picContent.Image = Resources.RBB;
                    break;

            }
            picPackage.Image = picContent.Image;
            xSession.HeaderData.ContentImageBinary = picContent.Image.ImageToBytes(ImageFormat.Png);
            xSession.HeaderData.PackageImageBinary = picPackage.Image.ImageToBytes(ImageFormat.Png);
        }              

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            if (picWorking.Visible) return;
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            Tools.CurrentFolder = Path.GetDirectoryName(files[0]);
            foreach (var file in files)
            {
                GetFiles(file);
            }
        }

        private void HandleDragEnter(object sender, DragEventArgs e)
        {
            e.Effect = !picWorking.Visible &&
               e.Data.GetDataPresent(DataFormats.FileDrop)
                ? DragDropEffects.Copy
                : DragDropEffects.None;
        }

        private void advTree1_DragDrop(object sender, DragEventArgs e)
        {
            if (picWorking.Visible)
                return;

            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            var folders = e.Data.GetData(DataFormats.FileDrop) as string[];

            if (folders == null || folders.Length == 0)
                return;

            folderTree.BeginUpdate();

            try
            {
                foreach (var folder in folders)
                {
                    if (!Directory.Exists(folder))
                        continue;

                    string shortName = new DirectoryInfo(folder).Name;
                    string destination = "songs/" + shortName;

                    if (xSession.GetFolder(destination) != null)
                    {
                        MessageBox.Show(
                            "There is already a folder with the name '" +
                            shortName + "'.\n\n" +
                            "Try deleting the existing folder first.",
                            Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);

                        continue;
                    }

                    EnsurePackageFolder("songs");

                    // Import songs.dta if one exists in the parent folder
                    // and the package doesn't already contain one.

                    string parent = Path.GetDirectoryName(
                        folder.TrimEnd('\\', '/'));

                    if (!string.IsNullOrEmpty(parent))
                    {
                        string dta = Path.Combine(parent, "songs.dta");

                        if (File.Exists(dta))
                        {
                            ImportPackageFile(dta, "songs");
                        }
                    }

                    // Import the entire song folder recursively.

                    ImportPackageFolder(folder, destination);

                    folderTree.SelectedNode =
                        EnsurePackageFolder(destination);

                    folderTree.SelectedNode.ExpandAll();

                    GetSelFiles(
                        (CFolderEntry)folderTree.SelectedNode.Tag);

                    btnCreate.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error importing folder:\n\n" + ex.Message,
                    Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                folderTree.EndUpdate();
            }
        }

        private void txtDisplay_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.A)
                txtDisplay.SelectAll();
        }

        private void txtDisplay_DoubleClick(object sender, EventArgs e)
        {
            txtDisplay.SelectAll();
        }

        private void txtDescription_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.A)
                txtDescription.SelectAll();
        }

        private void txtDescription_DoubleClick(object sender, EventArgs e)
        {
            txtDescription.SelectAll();
        }

        private void pictureBox1_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            GetImage(files[0], picContent);
        }

        private void pictureBox2_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            GetImage(files[0], picPackage);
        }

        private void pictureBox2_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                GetImage("", picPackage);
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                GetImage("", picContent);
            }
        }

        private void CONCreator_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!picWorking.Visible) return;
            MessageBox.Show("Please wait until the current process finishes", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            e.Cancel = true;
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

        private void folderTree_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = !picWorking.Visible &&
               e.Data.GetDataPresent(DataFormats.FileDrop)
        ? DragDropEffects.Copy
        : DragDropEffects.None;
        }
    }    
}