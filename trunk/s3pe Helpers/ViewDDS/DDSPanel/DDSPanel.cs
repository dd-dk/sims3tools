using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DdsFileTypePlugin;
using System.IO;

namespace DDSPanel
{
    public partial class DDSPanel : UserControl
    {

        public DDSPanel()
        {
            InitializeComponent();
        }

        bool fit = false;
        [DefaultValue(false), Description("Set to true to have the image resize to the control bounds")]
        public bool Fit { get { return fit; } set { fit = value; pictureBox1.Image = doResize(); OnChanged(FitChanged); } }

        Size maxSize = new Size(0, 0);
        [Description("Set non-zero bounds to constrain the image size")]
        public Size MaxSize { get { return maxSize; } set { maxSize = value; pictureBox1.Image = doResize(); OnChanged(MaxSizeChanged); } }

        [DefaultValue(true), Description("Display channel 1 as Red")]
        public bool Channel1 { get { return ckbR.Checked; } set { ckbR.Checked = value; OnChanged(Channel1Changed); } }
        [DefaultValue(true), Description("Display channel 2 as Green")]
        public bool Channel2 { get { return ckbG.Checked; } set { ckbG.Checked = value; OnChanged(Channel2Changed); } }
        [DefaultValue(true), Description("Display channel 3 as Blue")]
        public bool Channel3 { get { return ckbB.Checked; } set { ckbB.Checked = value; OnChanged(Channel3Changed); } }
        [DefaultValue(true), Description("Display channel 4 as Alpha")]
        public bool Channel4 { get { return ckbA.Checked; } set { ckbA.Checked = value; OnChanged(Channel4Changed); } }
        [DefaultValue(false), Description("Invert channel 4 values")]
        public bool InvertCh4 { get { return ckbI.Checked; } set { ckbI.Checked = value; OnChanged(InvertCh4Changed); } }

        [DefaultValue(true), Description("Show the Channel Selection check boxes")]
        public bool ShowChannelSelector { get { return flowLayoutPanel1.Visible; } set { flowLayoutPanel1.Visible = value; pictureBox1.Image = doResize(); OnChanged(ShowChannelSelectorChanged); } }

        [Description("Raised to indicate Fit value changed")]
        public event EventHandler FitChanged;
        [Description("Raised to indicate MaxSize value changed")]
        public event EventHandler MaxSizeChanged;
        [Description("Raised to indicate Channel1 value changed")]
        public event EventHandler Channel1Changed;
        [Description("Raised to indicate Channel2 value changed")]
        public event EventHandler Channel2Changed;
        [Description("Raised to indicate Channel3 value changed")]
        public event EventHandler Channel3Changed;
        [Description("Raised to indicate Channel4 value changed")]
        public event EventHandler Channel4Changed;
        [Description("Raised to indicate InvertCh4 value changed")]
        public event EventHandler InvertCh4Changed;
        [Description("Raised to indicate ShowChannelSelector value changed")]
        public event EventHandler ShowChannelSelectorChanged;

        protected void OnChanged(EventHandler h) { if (h != null) h(this, EventArgs.Empty); }

        DdsFile ddsFile = new DdsFile();
        bool loaded = false;
        /// <summary>
        /// Load a DDS image from a <see cref="System.IO.Stream"/>
        /// </summary>
        /// <param name="stream">The <see cref="System.IO.Stream"/> containing the DDS image to display</param>
        public void DDSLoad(Stream stream)
        {
            try
            {
                this.Enabled = false;
                Application.UseWaitCursor = true;
                ddsFile.Load(stream);
                loaded = true;
            }
            finally { this.Enabled = true; Application.UseWaitCursor = false; }
            ckb_CheckedChanged(null, null);
        }

        DateTime now = DateTime.UtcNow;

        Image image;
        private void ckb_CheckedChanged(object sender, EventArgs e)
        {
            if (!loaded) return;

            try
            {
                this.Enabled = false;
                Application.UseWaitCursor = true;
                image = ddsFile.Image(ckbR.Checked, ckbG.Checked, ckbB.Checked, ckbA.Checked, ckbI.Checked);
                pictureBox1.Image = doResize();
            }
            finally { this.Enabled = true; Application.UseWaitCursor = false; }

            if (sender != null)
            {
                if (sender == ckbR) Channel1 = ckbR.Checked;
                if (sender == ckbG) Channel2 = ckbG.Checked;
                if (sender == ckbB) Channel3 = ckbB.Checked;
                if (sender == ckbA) Channel4 = ckbA.Checked;
                if (sender == ckbI) InvertCh4 = ckbI.Checked;
            }
        }

        Image doResize()
        {
            if (!loaded) return null;

            Image targetImage = image;
            Size targetSize = targetImage.Size;

            if (Fit) targetSize = ScaleToFit(targetSize, panel1.ClientSize);

            targetSize = ScaleToFit(targetSize, maxSize);

            pictureBox1.Size = targetSize;

            if (targetImage.Size.Width > targetSize.Width || targetImage.Size.Height > targetSize.Height)
                targetImage = targetImage.GetThumbnailImage(
                    targetImage.Size.Width > targetSize.Width ? targetSize.Width : targetImage.Size.Width,
                    targetImage.Size.Height > targetSize.Height ? targetSize.Height : targetImage.Size.Height,
                    gtAbort, System.IntPtr.Zero);

            return targetImage;
        }
        static bool gtAbort() { return false; }

        Size ScaleToFit(Size from, Size constraint)
        {
            double scaleWidth = constraint.Width <= 0 ? 1 : Math.Min(from.Width, constraint.Width) / (double)from.Width;
            double scaleHeight = constraint.Height <= 0 ? 1 : Math.Min(from.Height, constraint.Height) / (double)from.Height;
            double scale = Math.Min(scaleWidth, scaleHeight);
            return new Size((int)Math.Round(from.Width * scale - 0.5), (int)Math.Round(from.Height * scale - 0.5));
        }

        private void DDSPanel_Resize(object sender, EventArgs e)
        {
            if (fit && DateTime.UtcNow > now.AddMilliseconds(25))
            {
                pictureBox1.Image = doResize();
                now = DateTime.UtcNow;
            }
            else
                timer1.Interval = 10;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            pictureBox1.Image = doResize();
        }
    }
}