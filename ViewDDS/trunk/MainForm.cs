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
        DdsFileTypePlugin.DdsFile ddsFile = new DdsFileTypePlugin.DdsFile();

        public MainForm()
        {
            InitializeComponent();

            MemoryStream ms = Clipboard.GetData(DataFormats.Serializable) as MemoryStream;
            if (ms == null)
                throw new Exception("Clipboard data not a MemoryStream");

            try
            {
                Application.UseWaitCursor = true;
                ddsFile.Load(ms);
            }
            finally { Application.UseWaitCursor = false; }
        }

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
    }
}
