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
            mask.Position = 0;
            ddsPanel1.ApplyMask(mask,
                new RGBHSV.HSVShift { h = (float)numMaskCh1Hue.Value, s = (float)numMaskCh1Saturation.Value, v = (float)numMaskCh1Value.Value, },
                RGBHSV.HSVShift.Empty, RGBHSV.HSVShift.Empty, RGBHSV.HSVShift.Empty);
        }

        private void btnResetMask_Click(object sender, EventArgs e)
        {
            ddsPanel1.ClearMask();
            numMaskCh1Hue.Value = numMaskCh1Saturation.Value = numMaskCh1Value.Value = 0;
        }

        private void numMask_ValueChanged(object sender, EventArgs e)
        {
            if (mask == null) return;
            mask.Position = 0;
            ddsPanel1.ApplyMask(mask,
                new RGBHSV.HSVShift { h = (float)numMaskCh1Hue.Value, s = (float)numMaskCh1Saturation.Value, v = (float)numMaskCh1Value.Value, },
                RGBHSV.HSVShift.Empty, RGBHSV.HSVShift.Empty, RGBHSV.HSVShift.Empty);
        }
    }
}
