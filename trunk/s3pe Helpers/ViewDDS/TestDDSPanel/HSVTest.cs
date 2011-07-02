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
            ddsPanel1.Fit = true;
        }

        private void ned_ValueChanged(object sender, EventArgs e)
        {
            ddsPanel1.HSVShift(hueShift.Value, saturationShift.Value, valueShift.Value);
        }

        private void btnOpenImage_Click(object sender, EventArgs e)
        {
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr != DialogResult.OK) return;

            FileStream fs = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read);
            ddsPanel1.DDSLoad(fs, true);
        }

        Stream mask = null;
        private void btnOpenMask_Click(object sender, EventArgs e)
        {
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr != DialogResult.OK) return;

            mask = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read);
        }

        private void btnResetMask_Click(object sender, EventArgs e)
        {
            ddsPanel1.ClearMask();
            numMaskCh1Hue.Value = numMaskCh1Saturation.Value = numMaskCh1Value.Value =
                numMaskCh2Hue.Value = numMaskCh2Saturation.Value = numMaskCh2Value.Value = 0;
            btnApply_Click(null, null);
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (mask == null) return;

            mask.Position = 0;
            ddsPanel1.ApplyMask(mask,
                new RGBHSV.HSVShift { h = (float)numMaskCh1Hue.Value, s = (float)numMaskCh1Saturation.Value, v = (float)numMaskCh1Value.Value, },
                new RGBHSV.HSVShift { h = (float)numMaskCh2Hue.Value, s = (float)numMaskCh2Saturation.Value, v = (float)numMaskCh2Value.Value, },
                RGBHSV.HSVShift.Empty, RGBHSV.HSVShift.Empty, ckbBlend.Checked);
        }
    }
}
