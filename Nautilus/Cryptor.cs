using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using Nautilus.Properties;
using Nautilus.x360;
using Application = System.Windows.Forms.Application;
using Button = System.Windows.Forms.Button;
using NautilusFREE;

namespace Nautilus
{
    public partial class Cryptor : Form
    {
        private List<string> moggFiles;
        private List<string> YargFiles;
        private readonly List<string> CONFiles;
        private readonly NemoTools Tools;
        private readonly DTAParser Parser;
        private DateTime startTime;
        private DateTime endTime;
        private readonly nTools nautilus3;

        public Cryptor(Color ButtonBackColor, Color ButtonTextColor)
        {
            InitializeComponent();
            Tools = new NemoTools();
            Parser = new DTAParser();
            nautilus3 = new nTools();
            var formButtons = new List<Button> { btnRefresh, btnFolder, btnBegin };
            foreach (var button in formButtons)
            {
                button.BackColor = ButtonBackColor;
                button.ForeColor = ButtonTextColor;
                button.FlatAppearance.MouseOverBackColor = button.BackColor == Color.Transparent ? Color.FromArgb(127, Color.AliceBlue.R, Color.AliceBlue.G, Color.AliceBlue.B) : Tools.LightenColor(button.BackColor);
            }
            CONFiles = new List<string>();
            moggFiles = new List<string>();
            YargFiles = new List<string>();
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
            moggFiles.Clear();
            CONFiles.Clear();
            if (string.IsNullOrWhiteSpace(txtFolder.Text))
            {
                btnRefresh.Visible = false;
            }
            btnRefresh.Visible = true;
            if (txtFolder.Text != "")
            {
                Log("");
                Log("Reading input directory ... hang on");
                try
                {
                    moggFiles = Directory.GetFiles(txtFolder.Text, "*.mogg").ToList();
                    YargFiles = Directory.GetFiles(txtFolder.Text, "*.yarg_mogg").ToList();
                    var allfiles = Directory.GetFiles(txtFolder.Text);
                    foreach (var file in allfiles.Where(file => Path.GetExtension(file) != ".lnk").Where(file => VariousFunctions.ReadFileType(file) == XboxFileType.STFS))
                    {
                        CONFiles.Add(file);
                    }
                    if (!moggFiles.Any() && !CONFiles.Any() && !YargFiles.Any())
                    {
                        Log("Did not find any mogg or CON files ... try a different directory");
                        Log("You can also drag and drop mogg or CON files here");
                        Log("Ready");
                        btnBegin.Visible = false;
                    }
                    else
                    {
                        Log("Found " + moggFiles.Count()  + " mogg " + (moggFiles.Count() == 1 ? "file" : "files"));
                        Log("Found " + YargFiles.Count() + " YARG mogg " + (moggFiles.Count() == 1 ? "file" : "files"));
                        Log("Found " + CONFiles.Count() + " CON " + (CONFiles.Count() == 1 ? "file" : "files"));
                        Log("Ready to begin");
                        btnBegin.Visible = true;
                    }
                    btnRefresh.Visible = true;
                }
                catch (Exception ex)
                {
                    Log("There was an error: " + ex.Message);
                }
            }
            else
            {
                btnRefresh.Visible = false;
                btnBegin.Visible = false;
            }
        }
        
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            var tFolder = txtFolder.Text;
            txtFolder.Text = "";
            txtFolder.Text = tFolder;
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
        
        private void exportLogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.ExportLog(Text, lstLog.Items);
        }

        private void Cryptor_Shown(object sender, EventArgs e)
        {
            Log("Welcome to " + Text);
            Log("Drag and drop mogg or CON files here");
            Log("Or click 'Change Input Folder' to select the files");
            Log("Ready");
        }
        
        private void btnBegin_Click(object sender, EventArgs e)
        {            
            if (btnBegin.Text == "Cancel")
            {
                if (backgroundWorker1.CancellationPending) return;
                backgroundWorker1.CancelAsync();
                Log("User cancelled process...stopping as soon as possible");
                return;
            }
            EnableDisable(false);
            btnBegin.Text = "Cancel";
            toolTip1.SetToolTip(btnBegin, "Click to cancel process"); 
            startTime = DateTime.Now;
            EnableDisable(false);
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            var encFolder = txtFolder.Text + "\\encrypted\\";
            var decFolder = txtFolder.Text + "\\decrypted\\";
            if (radioDecrypt.Checked && !Directory.Exists(decFolder) && radioDecrypt.Checked)
            {
                Directory.CreateDirectory(decFolder);
            }
            if (radioEncrypt.Checked && !Directory.Exists(encFolder) && radioEncrypt.Checked)
            {
                Directory.CreateDirectory(encFolder);
            }
            foreach (var mogg in moggFiles.Where(File.Exists))
            {
                if (backgroundWorker1.CancellationPending) return;
                Log((radioEncrypt.Checked ? "Encrypting" : "Decrypting") + " mogg file " + Path.GetFileName(mogg) + " ...");
                var newMogg = (radioEncrypt.Checked ? encFolder : decFolder) + Path.GetFileName(mogg);
                Tools.DeleteFile(newMogg);
                File.Copy(mogg, newMogg);
                var mData = File.ReadAllBytes(newMogg);
                if (radioEncrypt.Checked)
                {
                    if (nautilus3.MoggIsEncrypted(mData) || nautilus3.MoggIsObfuscated(mData))
                    {
                        if (nautilus3.IsC3Mogg(mData))
                        {
                            //only patch 0x0C and 0x0D moggs
                            var encByte = new int();
                            using (var fs = File.OpenRead(newMogg))
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
                                Log("File is already encrypted but needs the PS3 patch, applying...");
                                var patched = nautilus3.PatchMoggForPS3Use(newMogg);
                                Log(patched ? "Success" : "Failed");
                                if (!patched)
                                {
                                    Tools.DeleteFile(newMogg);
                                }
                            }
                            Log("No need to patch that file, skipping...");
                            Tools.DeleteFile(newMogg);
                            continue;
                        }
                        else
                        {
                            Log("File is already encrypted, skipping...");
                            Tools.DeleteFile(newMogg);
                            continue;
                        }
                    }
                    var success = nautilus3.EncM(mData, newMogg);
                    if (success && chkObfuscate.Checked)
                    {
                        nautilus3.WriteOutData(nautilus3.ObfM(File.ReadAllBytes(newMogg)), newMogg);
                    }
                    Log((success ? "Successfully encrypted" : "Failed to encrypt") + " mogg file " + Path.GetFileName(newMogg));
                    if (!success)
                    {
                        Tools.DeleteFile(newMogg);
                    }
                }
                else
                {
                    if (!nautilus3.MoggIsEncrypted(mData))
                    {
                        Log("Mogg file is already decrypted, skipping...");
                        Tools.DeleteFile(newMogg);
                        continue;
                    }
                    if (nautilus3.MoggIsObfuscated(mData))
                    {
                        Log("Mogg file is obfuscated, let me fix that...");
                        mData = nautilus3.DeObfM(mData);                        
                    }
                    var success = false;
                    success = nautilus3.DecM(mData, true, false, DecryptMode.ToFile, newMogg);
                    if (!success) //try TBRB decryption
                    {
                        success = Tools.BeatlesDecrypt(mogg, nautilus3, false, newMogg);
                    }
                    Log((success ? "Successfully decrypted" : "Failed to decrypt") + " mogg file " + Path.GetFileName(newMogg));
                    if (!success)
                    {
                        Tools.DeleteFile(newMogg);
                    }
                }
            }
            foreach (var mogg in YargFiles.Where(File.Exists))
            {
                if (backgroundWorker1.CancellationPending) return;
                if (radioEncrypt.Checked)
                {
                    Log("Encrypting of YARG files not supported");
                    break;
                }
                else
                {
                    Log("Decrypting YARG mogg file " + Path.GetFileName(mogg) + " ...");
                }
                var newMogg = decFolder + Path.GetFileName(mogg).Replace("yarg_mogg", "mogg");
                var success = nautilus3.DecY(mogg, DecryptMode.ToFile, newMogg);
                Log((success ? "Successfully decrypted" : "Failed to decrypt") + " YARG mogg file " + Path.GetFileName(mogg));                
            }
            foreach (var con in CONFiles)
            {
                if (backgroundWorker1.CancellationPending) return;
                Log((radioEncrypt.Checked ? "Encrypting" : "Decrypting") + " CON file " + Path.GetFileName(con) + " ...");
                var newCON = (radioEncrypt.Checked ? encFolder : decFolder) + Path.GetFileName(con);
                Tools.DeleteFile(newCON);
                File.Copy(con, newCON);
                if (!Parser.ExtractDTA(newCON))
                {
                    Log("Couldn't extract songs.dta file from " + Path.GetFileName(newCON) + " ... skipping");
                    Tools.DeleteFile(newCON);
                    continue;
                }
                if (!Parser.ReadDTA(Parser.DTA) || !Parser.Songs.Any())
                {
                    Log("Couldn't read that songs.dta file ... skipping");
                    Tools.DeleteFile(newCON);
                    continue;
                }
                if (Parser.Songs.Count > 1)
                {
                    Log("File " + Path.GetFileName(newCON) + " is a pack, try dePACKing first ... skipping");
                    Tools.DeleteFile(newCON);
                    continue;
                }
                var xCON = new STFSPackage(newCON);
                if (!xCON.ParseSuccess)
                {
                    Log("Couldn't parse file " + Path.GetFileName(newCON) + " ... skipping");
                    xCON.CloseIO();
                    Tools.DeleteFile(newCON);
                    continue;
                }
                var internalname = Parser.Songs[0].InternalName;
                var xFile = xCON.GetFile("songs/" + internalname + "/" + internalname + ".mogg");
                if (xFile == null)
                {
                    Log("Couldn't find mogg file inside " + Path.GetFileName(newCON) + " ... skipping");
                    xCON.CloseIO();
                    Tools.DeleteFile(newCON);
                    continue;
                }
                var newMogg = (radioEncrypt.Checked ? encFolder : decFolder) + internalname + ".mogg";
                Tools.DeleteFile(newMogg);
                if (backgroundWorker1.CancellationPending)
                {
                    xCON.CloseIO();
                    return;
                }
                var mData = xFile.Extract();
                if (mData == null || mData.Length == 0)
                {
                    Log("Couldn't extract mogg file from " + Path.GetFileName(newCON) + " ... skipping");
                    xCON.CloseIO();
                    Tools.DeleteFile(newCON);
                    continue;
                }
                if (radioEncrypt.Checked)
                {
                    if (nautilus3.MoggIsEncrypted(mData))
                    {
                        if (nautilus3.IsC3Mogg(mData))
                        {
                            //only patch 0x0C and 0x0D moggs
                            File.WriteAllBytes(newMogg, mData);
                            var encByte = new int();
                            using (var fs = File.OpenRead(newMogg))
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
                                Log("Mogg file is already encrypted but needs PS3 patch, applying...");                                
                                var patched = nautilus3.PatchMoggForPS3Use(newMogg);
                                Log(patched ? "Success" : "Failed");
                                if (!patched)
                                {
                                    xCON.CloseIO();
                                    Tools.DeleteFile(newMogg);
                                    Tools.DeleteFile(newCON);
                                    continue;
                                }
                                mData = File.ReadAllBytes(newMogg);
                            }
                            else
                            {
                                Log("No need to patch that Mogg file...");
                            }    
                        }
                        else
                        {
                            Log("Mogg file in CON " + Path.GetFileName(newCON) + " is already encrypted ... skipping");
                            xCON.CloseIO();
                            Tools.DeleteFile(newMogg);
                            Tools.DeleteFile(newCON);
                            continue;
                        }                            
                    }
                    else
                    {
                        if (!nautilus3.EncM(mData, newMogg))
                        {
                            Log("Failed to encrypt CON file " + Path.GetFileName(newCON) + " ... skipping");
                            xCON.CloseIO();
                            Tools.DeleteFile(newMogg);
                            Tools.DeleteFile(newCON);
                            continue;
                        }
                    }
                    
                }
                else
                {
                    if (!nautilus3.MoggIsEncrypted(mData))
                    {
                        Log("Mogg file in CON " + Path.GetFileName(newCON) + " is already decrypted ... skipping");
                        xCON.CloseIO();
                        Tools.DeleteFile(newMogg);
                        Tools.DeleteFile(newCON);
                        continue;
                    }                    
                    if (!nautilus3.DecM(mData, true, false, DecryptMode.ToFile, newMogg))
                    {
                        var temp = Path.Combine(Path.GetTempPath(), "temp.mogg");
                        File.WriteAllBytes(temp, mData);
                        if (!Tools.BeatlesDecrypt(temp, nautilus3, false, newMogg))
                        {
                            Log("Failed to decrypt CON file " + Path.GetFileName(newCON) + " ... skipping");
                            xCON.CloseIO();
                            Tools.DeleteFile(temp);
                            Tools.DeleteFile(newMogg);
                            Tools.DeleteFile(newCON);
                            continue;
                        }
                    }
                }
                if (backgroundWorker1.CancellationPending)
                {
                    Tools.DeleteFile(newMogg);
                    xCON.CloseIO();
                    return;
                }
                if (!xFile.Replace(newMogg))
                {
                    Log("Replacing mogg file in " + Path.GetFileName(newCON) + " failed ... skipping");
                    xCON.CloseIO();
                    Tools.DeleteFile(newMogg);
                    Tools.DeleteFile(newCON);
                    continue;
                }
                xCON.Header.ThisType = PackageType.SavedGame;
                xCON.Header.MakeAnonymous();
                var signature = new RSAParams(Application.StartupPath + "\\bin\\KV.bin");
                xCON.RebuildPackage(signature);
                xCON.FlushPackage(signature);
                xCON.CloseIO();
                Tools.DeleteFile(newMogg);
                var bOK = Tools.UnlockCON(newCON);
                if (bOK)
                {
                    bOK = Tools.SignCON(newCON);
                }
                if (bOK)
                {
                    Log((radioEncrypt.Checked ? "Encrypted" : "Decrypted") + " CON file " + Path.GetFileName(newCON) + " successfully");
                }
                else
                {
                    Log("Failed to " + (radioEncrypt.Checked ? "encrypt" : "decrypt") + " CON file " + Path.GetFileName(newCON));
                    Tools.DeleteFile(newCON);
                }
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            EnableDisable(true);
            endTime = DateTime.Now;
            var timeDiff = endTime - startTime;
            toolTip1.SetToolTip(btnBegin, "Click to begin");
            btnBegin.Text = "&Begin";
            Log("Batch " + (radioEncrypt.Checked ? "encrypting" : "decrypting") + " process completed");
            Log("Process took " + timeDiff.Minutes + (timeDiff.Minutes == 1 ? " minute" : " minutes") + " and " + (timeDiff.Minutes == 0 && timeDiff.Seconds == 0 ? "1 second" : timeDiff.Seconds + " seconds"));
            Log("The new files can be found in the '" + (radioEncrypt.Checked? "encrypted" : "decrypted") + "' folder");
            Log("Ready");
        }

        private void Cryptor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!picWorking.Visible) return;
            MessageBox.Show("Please wait until the current process finishes", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            e.Cancel = true;
        }

        private void EnableDisable(bool enable)
        {
            picWorking.Visible = !enable;
            lstLog.Cursor = enable? Cursors.Default : Cursors.WaitCursor;
            Cursor = lstLog.Cursor;
            btnFolder.Enabled = enable;
            btnRefresh.Enabled = enable;
            txtFolder.Enabled = enable;
            radioEncrypt.Enabled = enable;
            radioDecrypt.Enabled = enable;
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var message = "Both loose moggs and moggs inside of CON files " +
                "can be encrypted and decrypted\r\nThis will decrypt all types of C3 and Harmonix encrypted files\r\n\r\nPut all the files " +
                "to be encrypted or decrypted in one folder and drag and drop one of the files here\r\nSelect the right option and click " +
                "'Begin'";
            var help = new HelpForm(Text + " - Help", message);
            help.ShowDialog();
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
