using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using NautilusFREE;

namespace Nautilus
{
    public partial class Worker : Form
    {
        private readonly string WorkingFile;
        private readonly NemoTools Tools;
        private bool WorkerSuccess;
        private readonly nTools nautilus3;

        public Worker(string file)
        {
            InitializeComponent();
            Tools = new NemoTools();
            nautilus3 = new nTools();
            WorkingFile = file;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Opacity = Opacity - 5;
            Application.DoEvents();
            if (Opacity == 0)
            {
                Environment.Exit(0);
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            var nautilus3 = new nTools();
            var ogg = WorkingFile.Substring(0, WorkingFile.Length - 5) + ".ogg";            
            WorkerSuccess = nautilus3.DecM(File.ReadAllBytes(WorkingFile), false, false, DecryptMode.ToFile, ogg);
        }

        private void Worker_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!picWorking.Visible)
            {
                return;
            }
            MessageBox.Show("Please wait until the current process finishes", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            e.Cancel = true;
        }

        private void Worker_Shown(object sender, EventArgs e)
        {
            if (!File.Exists(WorkingFile))
            {
                lblStatus.Text = "File not found";
                Application.DoEvents();
                Thread.Sleep(1000);
                timer1.Enabled = true;
                return;
            }            
            picWorking.Visible = true;
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            picWorking.Visible = false;
            lblStatus.Text = WorkerSuccess ? "Success" : "Failed";
            Application.DoEvents();
            Thread.Sleep(1000);
            timer1.Enabled = true;
        }
    }
}
