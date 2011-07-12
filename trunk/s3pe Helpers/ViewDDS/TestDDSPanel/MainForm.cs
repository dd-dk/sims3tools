/***************************************************************************
 *  Copyright (C) 2011 by Peter L Jones                                    *
 *  pljones@users.sf.net                                                   *
 *                                                                         *
 *  This file is part of the Sims 3 Package Interface (s3pi)               *
 *                                                                         *
 *  s3pi is free software: you can redistribute it and/or modify           *
 *  it under the terms of the GNU General Public License as published by   *
 *  the Free Software Foundation, either version 3 of the License, or      *
 *  (at your option) any later version.                                    *
 *                                                                         *
 *  s3pi is distributed in the hope that it will be useful,                *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of         *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the          *
 *  GNU General Public License for more details.                           *
 *                                                                         *
 *  You should have received a copy of the GNU General Public License      *
 *  along with s3pi.  If not, see <http://www.gnu.org/licenses/>.          *
 ***************************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace s3pe.DDSTool
{
    public partial class MainForm : Form
    {
        string currentDdsFile = null;
        bool? currentDdsFileIsDds = null;

        public MainForm()
        {
            InitializeComponent();
        }

        public MainForm(string filename, bool isDDS)
            : this()
        {
            currentDdsFile = filename;
            currentDdsFileIsDds = isDDS;
            reloadToolStripMenuItem_Click(null, null);
        }

        private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            reloadToolStripMenuItem.Enabled = currentDdsFileIsDds.HasValue;
            saveToolStripMenuItem.Enabled = currentDdsFileIsDds.HasValue && currentDdsFileIsDds == true;
            importAlphaToolStripMenuItem.Enabled =
                clearToolStripMenuItem.Enabled =
                saveAsToolStripMenuItem.Enabled =
                exportToolStripMenuItem.Enabled =
                ddsPanel1.ImageSize != Size.Empty;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int width = ddsPanel1.MaskSize != Size.Empty ? ddsPanel1.MaskSize.Width : 1024;
            int height = ddsPanel1.MaskSize != Size.Empty ? ddsPanel1.MaskSize.Height : 1024;

            NewDDSParameters parms = new NewDDSParameters(width, height);
            DialogResult dr = parms.ShowDialog();
            if (dr != DialogResult.OK) return;

            currentDdsFile = null;
            currentDdsFileIsDds = null;
            ddsPanel1.CreateImage(parms.Value.Red, parms.Value.Green, parms.Value.Blue, parms.Value.Alpha, parms.Value.Width, parms.Value.Height, true);
            lbImageW.Text = ddsPanel1.ImageSize.Width + "";
            lbImageH.Text = ddsPanel1.ImageSize.Height + "";
            tlpImageSize.Visible = true;
            checkMaskSize();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "*.dds";
            openFileDialog1.FilterIndex = 0;
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr != DialogResult.OK) return;

            currentDdsFile = openFileDialog1.FileName;
            currentDdsFileIsDds = true;
            openDDS(currentDdsFile);
        }

        void openDDS(string filename)
        {
            try
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    ddsPanel1.DDSLoad(fs, true);
                    fs.Close();
                }
                lbImageW.Text = ddsPanel1.ImageSize.Width + "";
                lbImageH.Text = ddsPanel1.ImageSize.Height + "";
                tlpImageSize.Visible = true;
                checkMaskSize();
            }
            catch { }
        }

        private void importImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filename = GetImageName();
            if (filename == null || filename == "" || !File.Exists(filename))
                return;

            currentDdsFile = filename;
            currentDdsFileIsDds = false;
            openImage(currentDdsFile);
        }

        void openImage(string filename)
        {
            try
            {
                ddsPanel1.Import(filename, true);
                lbImageW.Text = ddsPanel1.ImageSize.Width + "";
                lbImageH.Text = ddsPanel1.ImageSize.Height + "";
                tlpImageSize.Visible = true;
                checkMaskSize();
            }
            catch { }
        }

        private void importAlphaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ddsPanel1.ImageSize == Size.Empty) return;

            string filename = GetImageName();
            if (filename == null || filename == "" || !File.Exists(filename))
                return;

            Image alphaChannelGreyScale = Image.FromFile(filename, true);
            ddsPanel1.SetAlphaFromGreyscale(alphaChannelGreyScale);
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentDdsFileIsDds.HasValue)
            {
                if (currentDdsFileIsDds == true)
                    openDDS(currentDdsFile);
                else
                    openImage(currentDdsFile);
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentDdsFile = null;
            currentDdsFileIsDds = null;
            tlpImageSize.Visible = false;

            ddsPanel1.Clear();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!currentDdsFileIsDds.HasValue || currentDdsFileIsDds != true) return;

            saveDDS(currentDdsFile);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ddsPanel1.ImageSize == Size.Empty) return;

            saveFileDialog1.FileName = "*.dds";
            saveFileDialog1.FilterIndex = 0;
            DialogResult dr = saveFileDialog1.ShowDialog();
            if (dr != DialogResult.OK) return;

            currentDdsFile = saveFileDialog1.FileName;
            currentDdsFileIsDds = true;
            saveDDS(currentDdsFile);
        }

        void saveDDS(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                ddsPanel1.DDSSave(fs);
                fs.Close();
            }
            MessageBox.Show("Saved DDS file.", "Save DDS as...");
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ddsPanel1.ImageSize == Size.Empty) return;

            string oldFilter = saveFileDialog1.Filter;
            string oldCaption = saveFileDialog1.Title;
            try
            {
                saveFileDialog1.Filter = "Image files|*.bmp;*.jpg;*.png;*.gif|All files|*.*";
                saveFileDialog1.Title = "Save to image file";
                saveFileDialog1.FileName = "*.bmp;*.jpg;*.png;*.gif";
                saveFileDialog1.FilterIndex = 0;
                DialogResult dr = saveFileDialog1.ShowDialog();
                if (dr != DialogResult.OK) return;
            }
            finally { saveFileDialog1.Filter = oldFilter; saveFileDialog1.Title = oldCaption; }

            try
            {
                ddsPanel1.Image.Save(saveFileDialog1.FileName);
                MessageBox.Show("Saved image to file.", "Export...");
            }
            catch { }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.ExitCode = 0;
            this.Close();
        }


        private void btnHSVReset_Click(object sender, EventArgs e)
        {
            hueShift.Value = saturationShift.Value = valueShift.Value = 0;
            btnHSVShift_Click(null, null);
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
                using (DdsFileTypePlugin.DdsFile ddsfile = new DdsFileTypePlugin.DdsFile())
                {
                    ddsfile.Load(fs, false); fs.Position = 0;
                    ddsMaskCh1.DDSLoad(ddsfile);
                    ddsMaskCh2.DDSLoad(ddsfile);
                    ddsMaskCh3.DDSLoad(ddsfile);
                    ddsMaskCh4.DDSLoad(ddsfile);
                }
                
                ddsPanel1.LoadMask(fs);

                lbMaskW.Text = ddsPanel1.MaskSize.Width + "";
                lbMaskH.Text = ddsPanel1.MaskSize.Height + "";
                tlpMaskSize.Visible = true;

                fs.Close();
            }
            checkMaskSize();
        }

        private void btnResetMask_Click(object sender, EventArgs e)
        {
            ddsMaskCh1.Clear();
            ddsMaskCh2.Clear();
            ddsMaskCh3.Clear();
            ddsMaskCh4.Clear();
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

            tlpMaskSize.Visible = false;

            ddsPanel1.ClearMask();
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
                newToolStripMenuItem_Click(null, null);
            }
        }

        uint? GetColour(decimal r, decimal g, decimal b, decimal a)
        {
            return ((uint)a << 24) | ((uint)r << 16) | ((uint)g << 8) | (uint)b;
        }

        private void btnApplyRGBA_Click(object sender, EventArgs e)
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
                catch (ArgumentException) { }
        }

        private void pbCh2_DoubleClick(object sender, EventArgs e)
        {
            string filename = GetImageName();
            if (filename != null)
                try { pbCh2.Load(filename); }
                catch (ArgumentException) { }
        }

        private void pbCh3_DoubleClick(object sender, EventArgs e)
        {
            string filename = GetImageName();
            if (filename != null)
                try { pbCh3.Load(filename); }
                catch (ArgumentException) { }
        }

        private void pbCh4_DoubleClick(object sender, EventArgs e)
        {
            string filename = GetImageName();
            if (filename != null)
                try { pbCh4.Load(filename); }
                catch (ArgumentException) { }
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

        void checkMaskSize()
        {
            if (ddsPanel1.ImageSize == Size.Empty || !ddsPanel1.MaskLoaded) return;

            if (ddsPanel1.ImageSize.Width == ddsPanel1.MaskSize.Width &&
                ddsPanel1.ImageSize.Height == ddsPanel1.MaskSize.Height) return;

            DialogResult dr = MessageBox.Show(String.Format("Resize Mask ({0}x{1}) to Image ({2}x{3})?",
                ddsPanel1.MaskSize.Width, ddsPanel1.MaskSize.Height,
                ddsPanel1.ImageSize.Width, ddsPanel1.ImageSize.Height),
                "Resize Mask?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr != DialogResult.Yes) return;

            ddsPanel1.ResizeMask();

            lbMaskW.Text = ddsPanel1.MaskSize.Width + "";
            lbMaskH.Text = ddsPanel1.MaskSize.Height + "";
        }
    }
}
