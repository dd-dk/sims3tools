using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ViewDDS
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        public MainForm(Stream ms)
            : this()
        {
            try
            {
                Application.UseWaitCursor = true;
                ddsFile.Load(ms);
            }
            finally { Application.UseWaitCursor = false; }
        }

        DdsFileTypePlugin.DdsFile ddsFile = new DdsFileTypePlugin.DdsFile();

        private void ckb_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                this.Enabled = false;
                Application.UseWaitCursor = true;
                pictureBox1.Image = ddsFile.Image(ckbR.Checked, ckbG.Checked, ckbB.Checked, ckbA.Checked);
                pictureBox1.Size = pictureBox1.Image.Size;
            }
            finally { this.Enabled = true; Application.UseWaitCursor = false; }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            ckb_CheckedChanged(null, null);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void licenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, "file:///" + Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "ViewDDS-Licence.htm"));
        }
    }
}
