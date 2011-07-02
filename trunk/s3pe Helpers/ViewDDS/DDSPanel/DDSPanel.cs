using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DdsFileTypePlugin;
using System.IO;

namespace DDSPanel
{
    /// <summary>
    /// Displays and manipulates a DDS image
    /// </summary>
    public partial class DDSPanel : UserControl
    {
        #region Attributes
        bool fit = false;
        Size maxSize = new Size(Size.Empty.Width, Size.Empty.Height);
        bool supportHSV = false;

        DdsFile ddsFile = new DdsFile();
        bool loaded = false;
        DateTime now = DateTime.UtcNow;
        Image image;
        RGBHSV.HSVShift hsvShift;
        #endregion

        /// <summary>
        /// Displays and manipulates a DDS image
        /// </summary>
        public DDSPanel()
        {
            InitializeComponent();
        }


        #region Designer properties
        /// <summary>
        /// When true, the image will resize to the control bounds.
        /// </summary>
        [DefaultValue(false), Description("Set to true to have the image resize to the control bounds")]
        public bool Fit { get { return fit; } set { fit = value; pictureBox1.Image = doResize(); OnChanged(FitChanged); } }

        /// <summary>
        /// When non-zero, indicates the maximum width and height to constrain the image size.
        /// </summary>
        [Description("Set non-zero bounds to constrain the image size")]
        public Size MaxSize { get { return maxSize; } set { maxSize = value; pictureBox1.Image = doResize(); OnChanged(MaxSizeChanged); } }

        /// <summary>
        /// The "Red" checkbox Checked state.
        /// </summary>
        /// <remarks>The DDS Image is retrieved using the Channel1 to 4 and InvertCh4 values.</remarks>
        [DefaultValue(true), Description("Display channel 1 as Red")]
        public bool Channel1 { get { return ckbR.Checked; } set { ckbR.Checked = value; OnChanged(Channel1Changed); } }
        /// <summary>
        /// The "Green" checkbox Checked state.
        /// </summary>
        /// <remarks>The DDS Image is retrieved using the Channel1 to 4 and InvertCh4 values.</remarks>
        [DefaultValue(true), Description("Display channel 2 as Green")]
        public bool Channel2 { get { return ckbG.Checked; } set { ckbG.Checked = value; OnChanged(Channel2Changed); } }
        /// <summary>
        /// The "Blue" checkbox Checked state.
        /// </summary>
        /// <remarks>The DDS Image is retrieved using the Channel1 to 4 and InvertCh4 values.</remarks>
        [DefaultValue(true), Description("Display channel 3 as Blue")]
        public bool Channel3 { get { return ckbB.Checked; } set { ckbB.Checked = value; OnChanged(Channel3Changed); } }
        /// <summary>
        /// The "Alpha" checkbox Checked state.
        /// </summary>
        /// <remarks>The DDS Image is retrieved using the Channel1 to 4 and InvertCh4 values.</remarks>
        [DefaultValue(true), Description("Display channel 4 as Alpha")]
        public bool Channel4 { get { return ckbA.Checked; } set { ckbA.Checked = value; OnChanged(Channel4Changed); } }
        /// <summary>
        /// The "Invert" checkbox Checked state.
        /// </summary>
        /// <remarks>The DDS Image is retrieved using the Channel1 to 4 and InvertCh4 values.</remarks>
        [DefaultValue(false), Description("Invert channel 4 values")]
        public bool InvertCh4 { get { return ckbI.Checked; } set { ckbI.Checked = value; OnChanged(InvertCh4Changed); } }

        /// <summary>
        /// When true, the Channel Selection checkboxes will be displayed above the image.
        /// </summary>
        [DefaultValue(true), Description("Show the Channel Selection check boxes")]
        public bool ShowChannelSelector { get { return flowLayoutPanel1.Visible; } set { flowLayoutPanel1.Visible = value; pictureBox1.Image = doResize(); OnChanged(ShowChannelSelectorChanged); } }

        /// <summary>
        /// When true, enables use of HSV-related methods.
        /// </summary>
        /// <remarks>Requires an increase in stored data whilst true.</remarks>
        [DefaultValue(false), Description("Enables use of HSV-related methods.  Requires an increase in stored data whilst true.")]
        public bool SupportsHSV
        {
            get { return ddsFile.SupportsHSV; }
            set
            {
                if (ddsFile.SupportsHSV != value)
                {
                    ddsFile.SupportsHSV = value;
                    ckb_CheckedChanged(null, null);
                }
            }
        }

        /// <summary>
        /// Hue shift to be applied to the image.
        /// </summary>
        /// <remarks>Only effective when HSV processing is enabled.</remarks>
        [DefaultValue(0f), Description("Hue shift to be applied to the image, when HSV processing is enabled.")]
        public float HueShift
        {
            get { return hsvShift.h; }
            set { if (hsvShift.h != value) { hsvShift.h = value; if (SupportsHSV) ckb_CheckedChanged(null, null); } }
        }

        /// <summary>
        /// Saturation shift to be applied to the image.
        /// </summary>
        /// <remarks>Only effective when HSV processing is enabled.</remarks>
        [DefaultValue(0f), Description("Saturation shift to be applied to the image, when HSV processing is enabled.")]
        public float SaturationShift
        {
            get { return hsvShift.s; }
            set { if (hsvShift.s != value) { hsvShift.s = value; if (SupportsHSV) ckb_CheckedChanged(null, null); } }
        }

        /// <summary>
        /// Value shift to be applied to the image.
        /// </summary>
        /// <remarks>Only effective when HSV processing is enabled.</remarks>
        [DefaultValue(0f), Description("Value shift to be applied to the image, when HSV processing is enabled.")]
        public float ValueShift
        {
            get { return hsvShift.v; }
            set { if (hsvShift.v != value) { hsvShift.v = value; if (SupportsHSV) ckb_CheckedChanged(null, null); } }
        }
        #endregion

        #region Events
        /// <summary>
        /// Raised to indicate Fit value changed.
        /// </summary>
        [Description("Raised to indicate Fit value changed")]
        public event EventHandler FitChanged;
        /// <summary>
        /// Raised to indicate MaxSize value changed.
        /// </summary>
        [Description("Raised to indicate MaxSize value changed")]
        public event EventHandler MaxSizeChanged;
        /// <summary>
        /// Raised to indicate Channel1 value changed.
        /// </summary>
        [Description("Raised to indicate Channel1 value changed")]
        public event EventHandler Channel1Changed;
        /// <summary>
        /// Raised to indicate Channel2 value changed.
        /// </summary>
        [Description("Raised to indicate Channel2 value changed")]
        public event EventHandler Channel2Changed;
        /// <summary>
        /// Raised to indicate Channel3 value changed.
        /// </summary>
        [Description("Raised to indicate Channel3 value changed")]
        public event EventHandler Channel3Changed;
        /// <summary>
        /// Raised to indicate Channel4 value changed.
        /// </summary>
        [Description("Raised to indicate Channel4 value changed")]
        public event EventHandler Channel4Changed;
        /// <summary>
        /// Raised to indicate InvertCh4 value changed.
        /// </summary>
        [Description("Raised to indicate InvertCh4 value changed")]
        public event EventHandler InvertCh4Changed;
        /// <summary>
        /// Raised to indicate ShowChannelSelector value changed.
        /// </summary>
        [Description("Raised to indicate ShowChannelSelector value changed")]
        public event EventHandler ShowChannelSelectorChanged;

        void OnChanged(EventHandler h) { if (h != null) h(this, EventArgs.Empty); }
        #endregion

        #region Methods
        /// <summary>
        /// Load a DDS image from a <see cref="System.IO.Stream"/>
        /// </summary>
        /// <param name="stream">The <see cref="System.IO.Stream"/> containing the DDS image to display</param>
        /// <param name="supportHSV">Optional; when true, HSV operations will be supported on the image.</param>
        public void DDSLoad(Stream stream, bool supportHSV = false)
        {
            try
            {
                this.Enabled = false;
                Application.UseWaitCursor = true;
                ddsFile.Load(stream, supportHSV);
                loaded = true;
            }
            finally { this.Enabled = true; Application.UseWaitCursor = false; }
            this.supportHSV = supportHSV;
            ckb_CheckedChanged(null, null);
        }

        /// <summary>
        /// Apply a hue, saturation and value shift to the image.
        /// </summary>
        /// <param name="h">Hue shift, default 0</param>
        /// <param name="s">Saturation shift, default 0</param>
        /// <param name="v">Value shift, default 0</param>
        public void HSVShift(decimal h = 0, decimal s = 0, decimal v = 0)
        {
            hsvShift = new RGBHSV.HSVShift { h = (float)h, s = (float)s, v = (float)v, };
            if (SupportsHSV) ckb_CheckedChanged(null, null);
        }

        /// <summary>
        /// Apply <see cref="RGBHSV.HSVShift"/> values to the image, based on the
        /// channels in the <paramref name="mask"/>.
        /// </summary>
        /// <param name="mask">The <see cref="System.IO.Stream"/> containing the DDS image to use as a mask.</param>
        /// <param name="ch1Shift">A shift to apply to the image when the first channel of the mask is active.</param>
        /// <param name="ch2Shift">A shift to apply to the image when the second channel of the mask is active.</param>
        /// <param name="ch3Shift">A shift to apply to the image when the third channel of the mask is active.</param>
        /// <param name="ch4Shift">A shift to apply to the image when the fourth channel of the mask is active.</param>
        /// <param name="blend">When true, each channel's shift adds; when false, each channel's shift overrides.</param>
        public void ApplyMask(Stream mask, RGBHSV.HSVShift ch1Shift, RGBHSV.HSVShift ch2Shift, RGBHSV.HSVShift ch3Shift, RGBHSV.HSVShift ch4Shift, bool blend)
        {
            if (!SupportsHSV) return;
            DdsFile ddsMask = new DdsFile();
            ddsMask.Load(mask, false);//only want the pixmap data
            if (blend)
                ddsFile.SetMask(ddsMask, ch1Shift, ch2Shift, ch3Shift, ch4Shift);
            else
                ddsFile.SetMaskNoBlend(ddsMask, ch1Shift, ch2Shift, ch3Shift, ch4Shift);

            ckb_CheckedChanged(null, null);
        }

        /// <summary>
        /// Removes any previously applied masked shifts
        /// </summary>
        public void ClearMask()
        {
            if (!SupportsHSV) return;
            ddsFile.ClearMask();
            ckb_CheckedChanged(null, null);
        }
        #endregion

        #region Implementation
        private void ckb_CheckedChanged(object sender, EventArgs e)
        {
            if (!loaded) return;

            try
            {
                this.Enabled = false;
                Application.UseWaitCursor = true;
                ddsFile.HSVShift = hsvShift;
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
        #endregion
    }
}