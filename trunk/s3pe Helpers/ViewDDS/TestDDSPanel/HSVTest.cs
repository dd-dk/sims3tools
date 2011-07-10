using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace TestDDSPanel
{
    public partial class HSVTest : Form
    {
        public HSVTest()
        {
            InitializeComponent();
        }

        private void btnOpenImage_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "*.dds";
            openFileDialog1.FilterIndex = 0;
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr != DialogResult.OK) return;

            using (FileStream fs = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read))
            {
                ddsPanel1.DDSLoad(fs, true);
                fs.Close();
            }
        }

        private void btnCreateImage_Click(object sender, EventArgs e)
        {
            int width = ddsPanel1.MaskSize != Size.Empty ? ddsPanel1.MaskSize.Width : 128;
            int height = ddsPanel1.MaskSize != Size.Empty ? ddsPanel1.MaskSize.Height : 128;
            ddsPanel1.CreateImage((byte)nudRed.Value, (byte)nudGreen.Value, (byte)nudBlue.Value, (byte)nudAlpha.Value, width, height, true);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ddsPanel1.Clear();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ddsPanel1.ImageSize == Size.Empty) return;

            DialogResult dr = saveFileDialog1.ShowDialog();
            if (dr != DialogResult.OK) return;

            using (FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.Create, FileAccess.Write))
            {
                ddsPanel1.DDSSave(fs);
                fs.Close();
            }
            MessageBox.Show("Saved DDS file.", "Save DDS as...");
        }

        private void btnHSVShift_Click(object sender, EventArgs e)
        {
            ddsPanel1.HSVShift(hueShift.Value, saturationShift.Value, valueShift.Value);
        }

        private void btnOpenMask_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "*.dds";
            openFileDialog1.FilterIndex = 0;
            string caption = openFileDialog1.Title;
            try
            {
                openFileDialog1.Title = "Select DDS Image to use as a mask";
                DialogResult dr = openFileDialog1.ShowDialog();
                if (dr != DialogResult.OK) return;
            }
            finally { openFileDialog1.Title = caption; }

            using (FileStream fs = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read))
            {
                ddsMask.DDSLoad(fs);
                fs.Position = 0;
                ddsPanel1.LoadMask(fs);
                fs.Close();
            }
        }

        private void btnResetMask_Click(object sender, EventArgs e)
        {
            ddsMask.Clear();
            numMaskCh1Hue.Value = numMaskCh1Saturation.Value = numMaskCh1Value.Value =
                numMaskCh2Hue.Value = numMaskCh2Saturation.Value = numMaskCh2Value.Value =
                numMaskCh3Hue.Value = numMaskCh3Saturation.Value = numMaskCh3Value.Value =
                numMaskCh4Hue.Value = numMaskCh4Saturation.Value = numMaskCh4Value.Value =
                0;
            nudCh1Red.Value = nudCh1Green.Value = nudCh1Blue.Value =
                nudCh2Red.Value = nudCh2Green.Value = nudCh2Blue.Value =
                nudCh3Red.Value = nudCh3Green.Value = nudCh3Blue.Value =
                nudCh4Red.Value = nudCh4Green.Value = nudCh4Blue.Value =
                0;
            nudCh1Alpha.Value = nudCh2Alpha.Value = nudCh3Alpha.Value = nudCh4Alpha.Value =
                255;
            ddsPanel1.ClearMask();
        }

        private void ddsMask_DoubleClick(object sender, EventArgs e)
        {
            btnOpenMask_Click(sender, EventArgs.Empty);
        }

        private void btnApplyShift_Click(object sender, EventArgs e)
        {
            ddsPanel1.ApplyHSVShift(
                ckbNoCh1.Checked ? RGBHSV.HSVShift.Empty : new RGBHSV.HSVShift { h = (float)numMaskCh1Hue.Value, s = (float)numMaskCh1Saturation.Value, v = (float)numMaskCh1Value.Value, },
                ckbNoCh2.Checked ? RGBHSV.HSVShift.Empty : new RGBHSV.HSVShift { h = (float)numMaskCh2Hue.Value, s = (float)numMaskCh2Saturation.Value, v = (float)numMaskCh2Value.Value, },
                ckbNoCh3.Checked ? RGBHSV.HSVShift.Empty : new RGBHSV.HSVShift { h = (float)numMaskCh3Hue.Value, s = (float)numMaskCh3Saturation.Value, v = (float)numMaskCh3Value.Value, },
                ckbNoCh4.Checked ? RGBHSV.HSVShift.Empty : new RGBHSV.HSVShift { h = (float)numMaskCh4Hue.Value, s = (float)numMaskCh4Saturation.Value, v = (float)numMaskCh4Value.Value, },
                ckbBlend.Checked);

            if (ddsPanel1.ImageSize.Width != ddsPanel1.MaskSize.Width || ddsPanel1.ImageSize.Height != ddsPanel1.MaskSize.Height)
            {
                btnCreateImage_Click(null, null);
            }
        }

        uint? GetColour(decimal r, decimal g, decimal b, decimal a)
        {
            return ((uint)a << 24) | ((uint)r << 16) | ((uint)g << 8) | (uint)b;
        }

        private void btnApplyColour_Click(object sender, EventArgs e)
        {
            ddsPanel1.ApplyColours(
                ckbNoCh1.Checked ? null : GetColour(nudCh1Red.Value, nudCh1Green.Value, nudCh1Blue.Value, nudCh1Alpha.Value),
                ckbNoCh2.Checked ? null : GetColour(nudCh2Red.Value, nudCh2Green.Value, nudCh2Blue.Value, nudCh2Alpha.Value),
                ckbNoCh3.Checked ? null : GetColour(nudCh3Red.Value, nudCh3Green.Value, nudCh3Blue.Value, nudCh3Alpha.Value),
                ckbNoCh4.Checked ? null : GetColour(nudCh4Red.Value, nudCh4Green.Value, nudCh4Blue.Value, nudCh4Alpha.Value)
                );
        }

        private void llCh1pb_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                pbCh1_DoubleClick(sender, EventArgs.Empty);
        }

        private void llCh2pb_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                pbCh2_DoubleClick(sender, EventArgs.Empty);
        }

        private void llCh3pb_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                pbCh3_DoubleClick(sender, EventArgs.Empty);
        }

        private void llCh4pb_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                pbCh4_DoubleClick(sender, EventArgs.Empty);
        }

        private void pbCh1_DoubleClick(object sender, EventArgs e)
        {
            string filename = GetImageName();
            if (filename != null)
                try { pbCh1.Load(filename); }
                catch (ArgumentException ex) { }
        }

        private void pbCh2_DoubleClick(object sender, EventArgs e)
        {
            string filename = GetImageName();
            if (filename != null)
                try { pbCh2.Load(filename); }
                catch (ArgumentException ex) { }
        }

        private void pbCh3_DoubleClick(object sender, EventArgs e)
        {
            string filename = GetImageName();
            if (filename != null)
                try { pbCh3.Load(filename); }
                catch (ArgumentException ex) { }
        }

        private void pbCh4_DoubleClick(object sender, EventArgs e)
        {
            string filename = GetImageName();
            if (filename != null)
                try { pbCh4.Load(filename); }
                catch (ArgumentException ex) { }
        }

        string GetImageName()
        {
            string oldFilter = openFileDialog1.Filter;
            string oldCaption = openFileDialog1.Title;
            try
            {
                openFileDialog1.Title = "Choose an image file";
                openFileDialog1.FileName = "*.bmp;*.jpg;*.png;*.gif";
                openFileDialog1.FilterIndex = 0;
                openFileDialog1.Filter = "Image files|*.bmp;*.jpg;*.png;*.gif|All files|*.*";
                DialogResult dr = openFileDialog1.ShowDialog();
                if (dr != DialogResult.OK) return null;
            }
            finally { openFileDialog1.Filter = oldFilter; openFileDialog1.Title = oldCaption; }
            return openFileDialog1.FileName;
        }

        private void btnApplyImage_Click(object sender, EventArgs e)
        {
            ddsPanel1.ApplyImage(
                ckbNoCh1.Checked ? null : pbCh1.Image,
                ckbNoCh2.Checked ? null : pbCh2.Image,
                ckbNoCh3.Checked ? null : pbCh3.Image,
                ckbNoCh4.Checked ? null : pbCh4.Image
                );
        }
    }
}
