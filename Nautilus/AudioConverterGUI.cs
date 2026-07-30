using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Nautilus.Properties;
using Nautilus.x360;

namespace Nautilus
{
    public partial class AudioConverter : Form
    {
        readonly string AudioConverterPath;
        bool isCON = false;

        public AudioConverter()
        {
            InitializeComponent();
            AudioConverterPath = Application.StartupPath + "\\bin\\AudioConverter.exe";
            if (!File.Exists(AudioConverterPath))
            {
                DoErrorMessage();
                Close();
            }
        }

        private void DoErrorMessage()
        {
            MessageBox.Show("This is an interface for the CLI application Audio Converter by Nemo\n\nIt appears that AudioConverter.exe is missing from the bin folder\n\nI can't work without it\n\nClick OK to exit", "Missing File", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnOgg_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
        }

        private bool ValidateFile(string file)
        {
            var valid_input = new List<string>() { ".aac", ".bik", ".flac", ".m4a", ".mogg", ".mp3", ".ogg", ".opus", ".wav", ".wma", ".yarg_mogg" };
            var input_ext = Path.GetExtension(file).ToLowerInvariant();

            if (!valid_input.Contains(input_ext))
            {
                if (VariousFunctions.ReadFileType(file) != XboxFileType.STFS)
                {
                    var top = TopMost;
                    TopMost = false;
                    MessageBox.Show("That's not a valid input file, try again.\n\nValid input files are:\n.aac | .bik | .flac | .m4a | .mogg | .mp3 | .ogg | .opus | .wav |  .wma | .yarg_mogg\nCON files are also supported", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    TopMost = top;
                    return false;
                }
                else
                {
                    isCON = true;
                }
            }
            return true;
        }

        private void SendToConverter(List<string> files, string format, string argument)
        {
            isCON = false;
            foreach (var file in files)
            {
                if (!ValidateFile(file))
                {
                    continue;
                }
                var path = Application.StartupPath + "\\bin\\";
                if (!File.Exists(AudioConverterPath))
                {
                    DoErrorMessage();
                    continue;
                }
                lblWorking.Visible = true;
                Application.DoEvents();
                var arg = "-\"" + file + "\" -" + format + argument;                
                var app = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    FileName = AudioConverterPath,
                    Arguments = arg,
                    WorkingDirectory = path
                };
                var process = Process.Start(app);
                do
                {
                    //
                } while (!process.HasExited);
                process.Dispose();
                lblWorking.Visible = false;
                Application.DoEvents();
                                
                var folder = Path.GetDirectoryName(file) + "\\";
                var output = folder + (isCON ? Path.GetFileName(file) : Path.GetFileNameWithoutExtension(file)) + "." + format;
                if (File.Exists(output))
                {
                    lblSuccess.Visible = true;
                }
                else
                {
                    lblFailed.Visible = true;
                }
            }
            timer1.Enabled = true;
        }

        private void btnOgg_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            SendToConverter(files.ToList(), "ogg", " -" + qualityOgg.Value.ToString());
        }

        private void btnOpus_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            SendToConverter(files.ToList(), "opus", " -" + qualityOpus.Value.ToString());
        }

        private void btnMP3_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            SendToConverter(files.ToList(), "mp3", " -" + qualityMp3.Value.ToString());
        }

        private void btnFlac_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            SendToConverter(files.ToList(), "flac", " -" + qualityFlac.Value.ToString());
        }

        private void btnWav_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            SendToConverter(files.ToList(), "wav", "");
        }

        private void btnAbout_Click(object sender, System.EventArgs e)
        {
            MessageBox.Show("Audio Converter by Nemo, 2024-2026", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void timer1_Tick(object sender, System.EventArgs e)
        {
            lblSuccess.Visible = false;
            lblFailed.Visible = false;
            timer1.Enabled = false;
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

        private void btnBink_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            SendToConverter(files.ToList(), "mogg", " -" + qualityBink.Value.ToString());
        }
    }
}
