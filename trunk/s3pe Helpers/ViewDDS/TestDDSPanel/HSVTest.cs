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

        private void btnSetColour_Click(object sender, EventArgs e)
        {
            int width = ddsPanel1.MaskSize != Size.Empty ? ddsPanel1.MaskSize.Width : 128;
            int height = ddsPanel1.MaskSize != Size.Empty ? ddsPanel1.MaskSize.Height : 128;
            ddsPanel1.SetColour((byte)nudRed.Value, (byte)nudGreen.Value, (byte)nudBlue.Value, (byte)nudAlpha.Value, width, height, true);
        }

        private void btnOpenImage_Click(object sender, EventArgs e)
        {
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr != DialogResult.OK) return;

            FileStream fs = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read);
            ddsPanel1.DDSLoad(fs, true);
        }

        private void ned_ValueChanged(object sender, EventArgs e)
        {
            ddsPanel1.HSVShift(hueShift.Value, saturationShift.Value, valueShift.Value);
        }

        private void btnOpenMask_Click(object sender, EventArgs e)
        {
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr != DialogResult.OK) return;

            ddsPanel1.LoadMask(new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read));
        }

        private void btnResetMask_Click(object sender, EventArgs e)
        {
            ddsPanel1.ClearMask();
            numMaskCh1Hue.Value = numMaskCh1Saturation.Value = numMaskCh1Value.Value =
                numMaskCh2Hue.Value = numMaskCh2Saturation.Value = numMaskCh2Value.Value = 0;
            btnApplyShift_Click(null, null);
        }

        private void btnApplyShift_Click(object sender, EventArgs e)
        {
            ddsPanel1.ApplyHSVShift(
                new RGBHSV.HSVShift { h = (float)numMaskCh1Hue.Value, s = (float)numMaskCh1Saturation.Value, v = (float)numMaskCh1Value.Value, },
                new RGBHSV.HSVShift { h = (float)numMaskCh2Hue.Value, s = (float)numMaskCh2Saturation.Value, v = (float)numMaskCh2Value.Value, },
                RGBHSV.HSVShift.Empty, RGBHSV.HSVShift.Empty, ckbBlend.Checked);

            if (ddsPanel1.ImageSize.Width != ddsPanel1.MaskSize.Width || ddsPanel1.ImageSize.Height != ddsPanel1.MaskSize.Height)
            {
                btnSetColour_Click(null, null);
            }
        }

        uint GetColour(decimal r, decimal g, decimal b, decimal a)
        {
            return ((uint)a << 24) | ((uint)r << 16) | ((uint)g << 8) | (uint)b;
        }

        private void btnApplyColour_Click(object sender, EventArgs e)
        {
            ddsPanel1.ApplyColours(GetColour(nudCh1Red.Value, nudCh1Green.Value, nudCh1Blue.Value, nudCh1Alpha.Value),
                GetColour(nudCh2Red.Value, nudCh2Green.Value, nudCh2Blue.Value, nudCh2Alpha.Value),
                null, null);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ddsPanel1.Clear();
        }
    }
}
