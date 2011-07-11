namespace TestDDSPanel
{
    partial class HSVTest
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.hueShift = new System.Windows.Forms.NumericUpDown();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.saturationShift = new System.Windows.Forms.NumericUpDown();
            this.valueShift = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.btnHSVShift = new System.Windows.Forms.Button();
            this.btnOpenImage = new System.Windows.Forms.Button();
            this.btnOpenMask = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.numMaskCh1Hue = new System.Windows.Forms.NumericUpDown();
            this.numMaskCh1Saturation = new System.Windows.Forms.NumericUpDown();
            this.numMaskCh1Value = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.btnResetMask = new System.Windows.Forms.Button();
            this.ckbBlend = new System.Windows.Forms.CheckBox();
            this.numMaskCh2Hue = new System.Windows.Forms.NumericUpDown();
            this.numMaskCh2Saturation = new System.Windows.Forms.NumericUpDown();
            this.numMaskCh2Value = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.btnApplyShift = new System.Windows.Forms.Button();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.nudRed = new System.Windows.Forms.NumericUpDown();
            this.nudGreen = new System.Windows.Forms.NumericUpDown();
            this.nudBlue = new System.Windows.Forms.NumericUpDown();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.nudAlpha = new System.Windows.Forms.NumericUpDown();
            this.btnSetColour = new System.Windows.Forms.Button();
            this.nudCh1Red = new System.Windows.Forms.NumericUpDown();
            this.nudCh1Green = new System.Windows.Forms.NumericUpDown();
            this.nudCh1Blue = new System.Windows.Forms.NumericUpDown();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.nudCh1Alpha = new System.Windows.Forms.NumericUpDown();
            this.nudCh2Red = new System.Windows.Forms.NumericUpDown();
            this.nudCh2Green = new System.Windows.Forms.NumericUpDown();
            this.nudCh2Blue = new System.Windows.Forms.NumericUpDown();
            this.nudCh2Alpha = new System.Windows.Forms.NumericUpDown();
            this.btnApplyColour = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel11 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel9 = new System.Windows.Forms.TableLayoutPanel();
            this.btnApplyImage = new System.Windows.Forms.Button();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.llCh2pb = new System.Windows.Forms.LinkLabel();
            this.pbCh2 = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.pbCh1 = new System.Windows.Forms.PictureBox();
            this.llCh1pb = new System.Windows.Forms.LinkLabel();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.numMaskCh3Hue = new System.Windows.Forms.NumericUpDown();
            this.numMaskCh4Hue = new System.Windows.Forms.NumericUpDown();
            this.numMaskCh3Saturation = new System.Windows.Forms.NumericUpDown();
            this.numMaskCh4Saturation = new System.Windows.Forms.NumericUpDown();
            this.numMaskCh3Value = new System.Windows.Forms.NumericUpDown();
            this.numMaskCh4Value = new System.Windows.Forms.NumericUpDown();
            this.nudCh3Red = new System.Windows.Forms.NumericUpDown();
            this.nudCh4Red = new System.Windows.Forms.NumericUpDown();
            this.nudCh3Green = new System.Windows.Forms.NumericUpDown();
            this.nudCh4Green = new System.Windows.Forms.NumericUpDown();
            this.nudCh3Blue = new System.Windows.Forms.NumericUpDown();
            this.nudCh4Blue = new System.Windows.Forms.NumericUpDown();
            this.nudCh3Alpha = new System.Windows.Forms.NumericUpDown();
            this.nudCh4Alpha = new System.Windows.Forms.NumericUpDown();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.pbCh3 = new System.Windows.Forms.PictureBox();
            this.llCh3pb = new System.Windows.Forms.LinkLabel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.pbCh4 = new System.Windows.Forms.PictureBox();
            this.llCh4pb = new System.Windows.Forms.LinkLabel();
            this.label10 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.ckbNoCh1 = new System.Windows.Forms.CheckBox();
            this.ckbNoCh2 = new System.Windows.Forms.CheckBox();
            this.ckbNoCh3 = new System.Windows.Forms.CheckBox();
            this.ckbNoCh4 = new System.Windows.Forms.CheckBox();
            this.ddsMask = new DDSPanel.DDSPanel();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.btnSave = new System.Windows.Forms.Button();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel10 = new System.Windows.Forms.TableLayoutPanel();
            this.btnExport = new System.Windows.Forms.Button();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.btnImport = new System.Windows.Forms.Button();
            this.ddsPanel1 = new DDSPanel.DDSPanel();
            ((System.ComponentModel.ISupportInitialize)(this.hueShift)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.saturationShift)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.valueShift)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaskCh1Hue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaskCh1Saturation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaskCh1Value)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaskCh2Hue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaskCh2Saturation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaskCh2Value)).BeginInit();
            this.tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGreen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBlue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAlpha)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCh1Red)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCh1Green)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCh1Blue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCh1Alpha)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCh2Red)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCh2Green)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCh2Blue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCh2Alpha)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel11.SuspendLayout();
            this.tableLayoutPanel9.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbCh2)).BeginInit();
            this.tableLayoutPanel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbCh1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaskCh3Hue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaskCh4Hue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaskCh3Saturation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaskCh4Saturation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaskCh3Value)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaskCh4Value)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCh3Red)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCh4Red)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCh3Green)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCh4Green)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCh3Blue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCh4Blue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCh3Alpha)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCh4Alpha)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbCh3)).BeginInit();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbCh4)).BeginInit();
            this.tableLayoutPanel5.SuspendLayout();
            this.tableLayoutPanel10.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // hueShift
            // 
            this.hueShift.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.hueShift.DecimalPlaces = 6;
            this.hueShift.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.hueShift.Location = new System.Drawing.Point(39, 16);
            this.hueShift.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.hueShift.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.hueShift.Name = "hueShift";
            this.hueShift.Size = new System.Drawing.Size(72, 20);
            this.hueShift.TabIndex = 2;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.hueShift, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.saturationShift, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.valueShift, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label17, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(282, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(114, 91);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // saturationShift
            // 
            this.saturationShift.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.saturationShift.DecimalPlaces = 6;
            this.saturationShift.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.saturationShift.Location = new System.Drawing.Point(39, 42);
            this.saturationShift.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.saturationShift.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.saturationShift.Name = "saturationShift";
            this.saturationShift.Size = new System.Drawing.Size(72, 20);
            this.saturationShift.TabIndex = 4;
            // 
            // valueShift
            // 
            this.valueShift.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.valueShift.DecimalPlaces = 6;
            this.valueShift.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.valueShift.Location = new System.Drawing.Point(39, 68);
            this.valueShift.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.valueShift.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.valueShift.Name = "valueShift";
            this.valueShift.Size = new System.Drawing.Size(72, 20);
            this.valueShift.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 19);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "HShift";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1, 45);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "SShift";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(1, 71);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "VShift";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label17, 2);
            this.label17.Location = new System.Drawing.Point(3, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(70, 13);
            this.label17.TabIndex = 0;
            this.label17.Text = "Overall shifts:";
            // 
            // btnHSVShift
            // 
            this.btnHSVShift.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnHSVShift.Location = new System.Drawing.Point(301, 120);
            this.btnHSVShift.Name = "btnHSVShift";
            this.btnHSVShift.Size = new System.Drawing.Size(75, 23);
            this.btnHSVShift.TabIndex = 7;
            this.btnHSVShift.Text = "Apply";
            this.btnHSVShift.UseVisualStyleBackColor = true;
            this.btnHSVShift.Click += new System.EventHandler(this.btnHSVShift_Click);
            // 
            // btnOpenImage
            // 
            this.btnOpenImage.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnOpenImage.Location = new System.Drawing.Point(3, 3);
            this.btnOpenImage.Name = "btnOpenImage";
            this.btnOpenImage.Size = new System.Drawing.Size(75, 23);
            this.btnOpenImage.TabIndex = 0;
            this.btnOpenImage.Text = "Load DDS...";
            this.btnOpenImage.UseVisualStyleBackColor = true;
            this.btnOpenImage.Click += new System.EventHandler(this.btnOpenImage_Click);
            // 
            // btnOpenMask
            // 
            this.btnOpenMask.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnOpenMask.Location = new System.Drawing.Point(3, 12);
            this.btnOpenMask.Name = "btnOpenMask";
            this.btnOpenMask.Size = new System.Drawing.Size(75, 23);
            this.btnOpenMask.TabIndex = 0;
            this.btnOpenMask.Text = "Load Mask";
            this.btnOpenMask.UseVisualStyleBackColor = true;
            this.btnOpenMask.Click += new System.EventHandler(this.btnOpenMask_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "dds";
            this.openFileDialog1.FileName = "*.dds";
            this.openFileDialog1.Filter = "DDS Images|*.dds|All files|*.*";
            this.openFileDialog1.Title = "Select a DDS Image to work on";
            // 
            // numMaskCh1Hue
            // 
            this.numMaskCh1Hue.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.numMaskCh1Hue.DecimalPlaces = 6;
            this.numMaskCh1Hue.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numMaskCh1Hue.Location = new System.Drawing.Point(45, 16);
            this.numMaskCh1Hue.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMaskCh1Hue.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.numMaskCh1Hue.Name = "numMaskCh1Hue";
            this.numMaskCh1Hue.Size = new System.Drawing.Size(72, 20);
            this.numMaskCh1Hue.TabIndex = 2;
            // 
            // numMaskCh1Saturation
            // 
            this.numMaskCh1Saturation.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.numMaskCh1Saturation.DecimalPlaces = 6;
            this.numMaskCh1Saturation.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numMaskCh1Saturation.Location = new System.Drawing.Point(45, 42);
            this.numMaskCh1Saturation.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMaskCh1Saturation.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.numMaskCh1Saturation.Name = "numMaskCh1Saturation";
            this.numMaskCh1Saturation.Size = new System.Drawing.Size(72, 20);
            this.numMaskCh1Saturation.TabIndex = 4;
            // 
            // numMaskCh1Value
            // 
            this.numMaskCh1Value.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.numMaskCh1Value.DecimalPlaces = 6;
            this.numMaskCh1Value.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numMaskCh1Value.Location = new System.Drawing.Point(45, 68);
            this.numMaskCh1Value.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMaskCh1Value.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.numMaskCh1Value.Name = "numMaskCh1Value";
            this.numMaskCh1Value.Size = new System.Drawing.Size(72, 20);
            this.numMaskCh1Value.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 19);
            this.label4.Margin = new System.Windows.Forms.Padding(0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "HShift";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 45);
            this.label5.Margin = new System.Windows.Forms.Padding(0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "SShift";
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 71);
            this.label6.Margin = new System.Windows.Forms.Padding(0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "VShift";
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(53, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Channel 1";
            // 
            // btnResetMask
            // 
            this.btnResetMask.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnResetMask.Location = new System.Drawing.Point(84, 12);
            this.btnResetMask.Name = "btnResetMask";
            this.btnResetMask.Size = new System.Drawing.Size(75, 23);
            this.btnResetMask.TabIndex = 1;
            this.btnResetMask.Text = "Reset";
            this.btnResetMask.UseVisualStyleBackColor = true;
            this.btnResetMask.Click += new System.EventHandler(this.btnResetMask_Click);
            // 
            // ckbBlend
            // 
            this.ckbBlend.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ckbBlend.AutoSize = true;
            this.tableLayoutPanel11.SetColumnSpan(this.ckbBlend, 3);
            this.ckbBlend.Location = new System.Drawing.Point(3, 51);
            this.ckbBlend.Name = "ckbBlend";
            this.ckbBlend.Size = new System.Drawing.Size(142, 17);
            this.ckbBlend.TabIndex = 3;
            this.ckbBlend.Text = "Blending Mask (for HSV)";
            this.ckbBlend.UseVisualStyleBackColor = true;
            // 
            // numMaskCh2Hue
            // 
            this.numMaskCh2Hue.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.numMaskCh2Hue.DecimalPlaces = 6;
            this.numMaskCh2Hue.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numMaskCh2Hue.Location = new System.Drawing.Point(123, 16);
            this.numMaskCh2Hue.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMaskCh2Hue.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.numMaskCh2Hue.Name = "numMaskCh2Hue";
            this.numMaskCh2Hue.Size = new System.Drawing.Size(72, 20);
            this.numMaskCh2Hue.TabIndex = 2;
            // 
            // numMaskCh2Saturation
            // 
            this.numMaskCh2Saturation.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.numMaskCh2Saturation.DecimalPlaces = 6;
            this.numMaskCh2Saturation.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numMaskCh2Saturation.Location = new System.Drawing.Point(123, 42);
            this.numMaskCh2Saturation.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMaskCh2Saturation.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.numMaskCh2Saturation.Name = "numMaskCh2Saturation";
            this.numMaskCh2Saturation.Size = new System.Drawing.Size(72, 20);
            this.numMaskCh2Saturation.TabIndex = 4;
            // 
            // numMaskCh2Value
            // 
            this.numMaskCh2Value.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.numMaskCh2Value.DecimalPlaces = 6;
            this.numMaskCh2Value.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numMaskCh2Value.Location = new System.Drawing.Point(123, 68);
            this.numMaskCh2Value.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMaskCh2Value.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.numMaskCh2Value.Name = "numMaskCh2Value";
            this.numMaskCh2Value.Size = new System.Drawing.Size(72, 20);
            this.numMaskCh2Value.TabIndex = 6;
            // 
            // label11
            // 
            this.label11.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(131, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(55, 13);
            this.label11.TabIndex = 0;
            this.label11.Text = "Channel 2";
            // 
            // btnApplyShift
            // 
            this.btnApplyShift.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnApplyShift.Location = new System.Drawing.Point(357, 40);
            this.btnApplyShift.Name = "btnApplyShift";
            this.tableLayoutPanel9.SetRowSpan(this.btnApplyShift, 3);
            this.btnApplyShift.Size = new System.Drawing.Size(75, 23);
            this.btnApplyShift.TabIndex = 3;
            this.btnApplyShift.Text = "ApplyHSV";
            this.btnApplyShift.UseVisualStyleBackColor = true;
            this.btnApplyShift.Click += new System.EventHandler(this.btnApplyShift_Click);
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.AutoSize = true;
            this.tableLayoutPanel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.nudRed, 1, 1);
            this.tableLayoutPanel4.Controls.Add(this.nudGreen, 1, 2);
            this.tableLayoutPanel4.Controls.Add(this.nudBlue, 1, 3);
            this.tableLayoutPanel4.Controls.Add(this.label12, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.label13, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.label14, 0, 3);
            this.tableLayoutPanel4.Controls.Add(this.label15, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.label16, 0, 4);
            this.tableLayoutPanel4.Controls.Add(this.nudAlpha, 1, 4);
            this.tableLayoutPanel4.Location = new System.Drawing.Point(81, 0);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 5;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.Size = new System.Drawing.Size(100, 117);
            this.tableLayoutPanel4.TabIndex = 0;
            // 
            // nudRed
            // 
            this.nudRed.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.nudRed.Location = new System.Drawing.Point(39, 16);
            this.nudRed.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudRed.Name = "nudRed";
            this.nudRed.Size = new System.Drawing.Size(58, 20);
            this.nudRed.TabIndex = 2;
            // 
            // nudGreen
            // 
            this.nudGreen.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.nudGreen.Location = new System.Drawing.Point(39, 42);
            this.nudGreen.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudGreen.Name = "nudGreen";
            this.nudGreen.Size = new System.Drawing.Size(58, 20);
            this.nudGreen.TabIndex = 4;
            // 
            // nudBlue
            // 
            this.nudBlue.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.nudBlue.Location = new System.Drawing.Point(39, 68);
            this.nudBlue.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudBlue.Name = "nudBlue";
            this.nudBlue.Size = new System.Drawing.Size(58, 20);
            this.nudBlue.TabIndex = 6;
            // 
            // label12
            // 
            this.label12.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(9, 19);
            this.label12.Margin = new System.Windows.Forms.Padding(0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(27, 13);
            this.label12.TabIndex = 1;
            this.label12.Text = "Red";
            // 
            // label13
            // 
            this.label13.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(0, 45);
            this.label13.Margin = new System.Windows.Forms.Padding(0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(36, 13);
            this.label13.TabIndex = 3;
            this.label13.Text = "Green";
            // 
            // label14
            // 
            this.label14.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(8, 71);
            this.label14.Margin = new System.Windows.Forms.Padding(0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(28, 13);
            this.label14.TabIndex = 5;
            this.label14.Text = "Blue";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.tableLayoutPanel4.SetColumnSpan(this.label15, 2);
            this.label15.Location = new System.Drawing.Point(3, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(91, 13);
            this.label15.TabIndex = 0;
            this.label15.Text = "Solid colour DDS:";
            // 
            // label16
            // 
            this.label16.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(2, 97);
            this.label16.Margin = new System.Windows.Forms.Padding(0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(34, 13);
            this.label16.TabIndex = 7;
            this.label16.Text = "Alpha";
            // 
            // nudAlpha
            // 
            this.nudAlpha.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.nudAlpha.Location = new System.Drawing.Point(39, 94);
            this.nudAlpha.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudAlpha.Name = "nudAlpha";
            this.nudAlpha.Size = new System.Drawing.Size(58, 20);
            this.nudAlpha.TabIndex = 8;
            this.nudAlpha.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            // 
            // btnSetColour
            // 
            this.btnSetColour.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSetColour.Location = new System.Drawing.Point(93, 120);
            this.btnSetColour.Name = "btnSetColour";
            this.btnSetColour.Size = new System.Drawing.Size(75, 23);
            this.btnSetColour.TabIndex = 9;
            this.btnSetColour.Text = "Create";
            this.btnSetColour.UseVisualStyleBackColor = true;
            this.btnSetColour.Click += new System.EventHandler(this.btnCreateImage_Click);
            // 
            // nudCh1Red
            // 
            this.nudCh1Red.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.nudCh1Red.Location = new System.Drawing.Point(65, 94);
            this.nudCh1Red.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudCh1Red.Name = "nudCh1Red";
            this.nudCh1Red.Size = new System.Drawing.Size(52, 20);
            this.nudCh1Red.TabIndex = 2;
            // 
            // nudCh1Green
            // 
            this.nudCh1Green.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.nudCh1Green.Location = new System.Drawing.Point(65, 120);
            this.nudCh1Green.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudCh1Green.Name = "nudCh1Green";
            this.nudCh1Green.Size = new System.Drawing.Size(52, 20);
            this.nudCh1Green.TabIndex = 4;
            // 
            // nudCh1Blue
            // 
            this.nudCh1Blue.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.nudCh1Blue.Location = new System.Drawing.Point(65, 146);
            this.nudCh1Blue.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudCh1Blue.Name = "nudCh1Blue";
            this.nudCh1Blue.Size = new System.Drawing.Size(52, 20);
            this.nudCh1Blue.TabIndex = 6;
            // 
            // label18
            // 
            this.label18.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(15, 97);
            this.label18.Margin = new System.Windows.Forms.Padding(0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(27, 13);
            this.label18.TabIndex = 1;
            this.label18.Text = "Red";
            // 
            // label19
            // 
            this.label19.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(6, 123);
            this.label19.Margin = new System.Windows.Forms.Padding(0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(36, 13);
            this.label19.TabIndex = 3;
            this.label19.Text = "Green";
            // 
            // label20
            // 
            this.label20.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(14, 149);
            this.label20.Margin = new System.Windows.Forms.Padding(0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(28, 13);
            this.label20.TabIndex = 5;
            this.label20.Text = "Blue";
            // 
            // label22
            // 
            this.label22.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(8, 175);
            this.label22.Margin = new System.Windows.Forms.Padding(0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(34, 13);
            this.label22.TabIndex = 7;
            this.label22.Text = "Alpha";
            // 
            // nudCh1Alpha
            // 
            this.nudCh1Alpha.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.nudCh1Alpha.Location = new System.Drawing.Point(65, 172);
            this.nudCh1Alpha.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudCh1Alpha.Name = "nudCh1Alpha";
            this.nudCh1Alpha.Size = new System.Drawing.Size(52, 20);
            this.nudCh1Alpha.TabIndex = 8;
            this.nudCh1Alpha.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            // 
            // nudCh2Red
            // 
            this.nudCh2Red.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.nudCh2Red.Location = new System.Drawing.Point(143, 94);
            this.nudCh2Red.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudCh2Red.Name = "nudCh2Red";
            this.nudCh2Red.Size = new System.Drawing.Size(52, 20);
            this.nudCh2Red.TabIndex = 2;
            // 
            // nudCh2Green
            // 
            this.nudCh2Green.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.nudCh2Green.Location = new System.Drawing.Point(143, 120);
            this.nudCh2Green.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudCh2Green.Name = "nudCh2Green";
            this.nudCh2Green.Size = new System.Drawing.Size(52, 20);
            this.nudCh2Green.TabIndex = 4;
            // 
            // nudCh2Blue
            // 
            this.nudCh2Blue.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.nudCh2Blue.Location = new System.Drawing.Point(143, 146);
            this.nudCh2Blue.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudCh2Blue.Name = "nudCh2Blue";
            this.nudCh2Blue.Size = new System.Drawing.Size(52, 20);
            this.nudCh2Blue.TabIndex = 6;
            // 
            // nudCh2Alpha
            // 
            this.nudCh2Alpha.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.nudCh2Alpha.Location = new System.Drawing.Point(143, 172);
            this.nudCh2Alpha.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudCh2Alpha.Name = "nudCh2Alpha";
            this.nudCh2Alpha.Size = new System.Drawing.Size(52, 20);
            this.nudCh2Alpha.TabIndex = 8;
            this.nudCh2Alpha.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            // 
            // btnApplyColour
            // 
            this.btnApplyColour.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnApplyColour.Location = new System.Drawing.Point(357, 131);
            this.btnApplyColour.Name = "btnApplyColour";
            this.tableLayoutPanel9.SetRowSpan(this.btnApplyColour, 4);
            this.btnApplyColour.Size = new System.Drawing.Size(75, 23);
            this.btnApplyColour.TabIndex = 3;
            this.btnApplyColour.Text = "ApplyRGBA";
            this.btnApplyColour.UseVisualStyleBackColor = true;
            this.btnApplyColour.Click += new System.EventHandler(this.btnApplyColour_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClear.Location = new System.Drawing.Point(184, 120);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 1;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.Controls.Add(this.tableLayoutPanel11);
            this.groupBox1.Location = new System.Drawing.Point(3, 159);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(447, 388);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Mask tools";
            // 
            // tableLayoutPanel11
            // 
            this.tableLayoutPanel11.AutoSize = true;
            this.tableLayoutPanel11.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel11.ColumnCount = 3;
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel11.Controls.Add(this.tableLayoutPanel9, 0, 2);
            this.tableLayoutPanel11.Controls.Add(this.ckbBlend, 0, 1);
            this.tableLayoutPanel11.Controls.Add(this.btnOpenMask, 0, 0);
            this.tableLayoutPanel11.Controls.Add(this.ddsMask, 2, 0);
            this.tableLayoutPanel11.Controls.Add(this.btnResetMask, 1, 0);
            this.tableLayoutPanel11.Location = new System.Drawing.Point(6, 19);
            this.tableLayoutPanel11.Name = "tableLayoutPanel11";
            this.tableLayoutPanel11.RowCount = 3;
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel11.Size = new System.Drawing.Size(435, 350);
            this.tableLayoutPanel11.TabIndex = 7;
            // 
            // tableLayoutPanel9
            // 
            this.tableLayoutPanel9.AutoSize = true;
            this.tableLayoutPanel9.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel9.ColumnCount = 6;
            this.tableLayoutPanel11.SetColumnSpan(this.tableLayoutPanel9, 3);
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel9.Controls.Add(this.nudCh2Blue, 2, 6);
            this.tableLayoutPanel9.Controls.Add(this.btnApplyImage, 5, 8);
            this.tableLayoutPanel9.Controls.Add(this.nudCh2Green, 2, 5);
            this.tableLayoutPanel9.Controls.Add(this.btnApplyColour, 5, 4);
            this.tableLayoutPanel9.Controls.Add(this.tableLayoutPanel8, 2, 8);
            this.tableLayoutPanel9.Controls.Add(this.nudCh2Red, 2, 4);
            this.tableLayoutPanel9.Controls.Add(this.tableLayoutPanel7, 1, 8);
            this.tableLayoutPanel9.Controls.Add(this.nudCh1Alpha, 1, 7);
            this.tableLayoutPanel9.Controls.Add(this.nudCh2Alpha, 2, 7);
            this.tableLayoutPanel9.Controls.Add(this.nudCh1Blue, 1, 6);
            this.tableLayoutPanel9.Controls.Add(this.nudCh1Red, 1, 4);
            this.tableLayoutPanel9.Controls.Add(this.numMaskCh2Value, 2, 3);
            this.tableLayoutPanel9.Controls.Add(this.numMaskCh2Saturation, 2, 2);
            this.tableLayoutPanel9.Controls.Add(this.numMaskCh2Hue, 2, 1);
            this.tableLayoutPanel9.Controls.Add(this.label22, 0, 7);
            this.tableLayoutPanel9.Controls.Add(this.label20, 0, 6);
            this.tableLayoutPanel9.Controls.Add(this.label19, 0, 5);
            this.tableLayoutPanel9.Controls.Add(this.label18, 0, 4);
            this.tableLayoutPanel9.Controls.Add(this.numMaskCh1Value, 1, 3);
            this.tableLayoutPanel9.Controls.Add(this.numMaskCh1Hue, 1, 1);
            this.tableLayoutPanel9.Controls.Add(this.btnApplyShift, 5, 1);
            this.tableLayoutPanel9.Controls.Add(this.label7, 1, 0);
            this.tableLayoutPanel9.Controls.Add(this.label11, 2, 0);
            this.tableLayoutPanel9.Controls.Add(this.label4, 0, 1);
            this.tableLayoutPanel9.Controls.Add(this.label6, 0, 3);
            this.tableLayoutPanel9.Controls.Add(this.label5, 0, 2);
            this.tableLayoutPanel9.Controls.Add(this.numMaskCh1Saturation, 1, 2);
            this.tableLayoutPanel9.Controls.Add(this.nudCh1Green, 1, 5);
            this.tableLayoutPanel9.Controls.Add(this.label8, 3, 0);
            this.tableLayoutPanel9.Controls.Add(this.label9, 4, 0);
            this.tableLayoutPanel9.Controls.Add(this.numMaskCh3Hue, 3, 1);
            this.tableLayoutPanel9.Controls.Add(this.numMaskCh4Hue, 4, 1);
            this.tableLayoutPanel9.Controls.Add(this.numMaskCh3Saturation, 3, 2);
            this.tableLayoutPanel9.Controls.Add(this.numMaskCh4Saturation, 4, 2);
            this.tableLayoutPanel9.Controls.Add(this.numMaskCh3Value, 3, 3);
            this.tableLayoutPanel9.Controls.Add(this.numMaskCh4Value, 4, 3);
            this.tableLayoutPanel9.Controls.Add(this.nudCh3Red, 3, 4);
            this.tableLayoutPanel9.Controls.Add(this.nudCh4Red, 4, 4);
            this.tableLayoutPanel9.Controls.Add(this.nudCh3Green, 3, 5);
            this.tableLayoutPanel9.Controls.Add(this.nudCh4Green, 4, 5);
            this.tableLayoutPanel9.Controls.Add(this.nudCh3Blue, 3, 6);
            this.tableLayoutPanel9.Controls.Add(this.nudCh4Blue, 4, 6);
            this.tableLayoutPanel9.Controls.Add(this.nudCh3Alpha, 3, 7);
            this.tableLayoutPanel9.Controls.Add(this.nudCh4Alpha, 4, 7);
            this.tableLayoutPanel9.Controls.Add(this.tableLayoutPanel2, 3, 8);
            this.tableLayoutPanel9.Controls.Add(this.tableLayoutPanel3, 4, 8);
            this.tableLayoutPanel9.Controls.Add(this.label10, 0, 8);
            this.tableLayoutPanel9.Controls.Add(this.label21, 0, 9);
            this.tableLayoutPanel9.Controls.Add(this.ckbNoCh1, 1, 9);
            this.tableLayoutPanel9.Controls.Add(this.ckbNoCh2, 2, 9);
            this.tableLayoutPanel9.Controls.Add(this.ckbNoCh3, 3, 9);
            this.tableLayoutPanel9.Controls.Add(this.ckbNoCh4, 4, 9);
            this.tableLayoutPanel9.Location = new System.Drawing.Point(0, 71);
            this.tableLayoutPanel9.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel9.Name = "tableLayoutPanel9";
            this.tableLayoutPanel9.RowCount = 10;
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel9.Size = new System.Drawing.Size(435, 279);
            this.tableLayoutPanel9.TabIndex = 9;
            // 
            // btnApplyImage
            // 
            this.btnApplyImage.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnApplyImage.Location = new System.Drawing.Point(357, 210);
            this.btnApplyImage.Name = "btnApplyImage";
            this.btnApplyImage.Size = new System.Drawing.Size(75, 23);
            this.btnApplyImage.TabIndex = 4;
            this.btnApplyImage.Text = "ApplyImages";
            this.btnApplyImage.UseVisualStyleBackColor = true;
            this.btnApplyImage.Click += new System.EventHandler(this.btnApplyImage_Click);
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tableLayoutPanel8.AutoSize = true;
            this.tableLayoutPanel8.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel8.ColumnCount = 2;
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel8.Controls.Add(this.llCh2pb, 0, 0);
            this.tableLayoutPanel8.Controls.Add(this.pbCh2, 1, 0);
            this.tableLayoutPanel8.Location = new System.Drawing.Point(125, 198);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 1;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel8.Size = new System.Drawing.Size(68, 48);
            this.tableLayoutPanel8.TabIndex = 8;
            // 
            // llCh2pb
            // 
            this.llCh2pb.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.llCh2pb.AutoSize = true;
            this.llCh2pb.Location = new System.Drawing.Point(0, 17);
            this.llCh2pb.Margin = new System.Windows.Forms.Padding(0);
            this.llCh2pb.Name = "llCh2pb";
            this.llCh2pb.Size = new System.Drawing.Size(20, 13);
            this.llCh2pb.TabIndex = 2;
            this.llCh2pb.TabStop = true;
            this.llCh2pb.Text = "C2";
            this.llCh2pb.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llCh2pb_LinkClicked);
            // 
            // pbCh2
            // 
            this.pbCh2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pbCh2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pbCh2.Location = new System.Drawing.Point(20, 0);
            this.pbCh2.Margin = new System.Windows.Forms.Padding(0);
            this.pbCh2.Name = "pbCh2";
            this.pbCh2.Size = new System.Drawing.Size(48, 48);
            this.pbCh2.TabIndex = 10;
            this.pbCh2.TabStop = false;
            this.pbCh2.DoubleClick += new System.EventHandler(this.pbCh2_DoubleClick);
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tableLayoutPanel7.AutoSize = true;
            this.tableLayoutPanel7.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel7.ColumnCount = 2;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel7.Controls.Add(this.pbCh1, 1, 0);
            this.tableLayoutPanel7.Controls.Add(this.llCh1pb, 0, 0);
            this.tableLayoutPanel7.Location = new System.Drawing.Point(47, 198);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 1;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel7.Size = new System.Drawing.Size(68, 48);
            this.tableLayoutPanel7.TabIndex = 7;
            // 
            // pbCh1
            // 
            this.pbCh1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pbCh1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pbCh1.Location = new System.Drawing.Point(20, 0);
            this.pbCh1.Margin = new System.Windows.Forms.Padding(0);
            this.pbCh1.Name = "pbCh1";
            this.pbCh1.Size = new System.Drawing.Size(48, 48);
            this.pbCh1.TabIndex = 10;
            this.pbCh1.TabStop = false;
            this.pbCh1.DoubleClick += new System.EventHandler(this.pbCh1_DoubleClick);
            // 
            // llCh1pb
            // 
            this.llCh1pb.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.llCh1pb.AutoSize = true;
            this.llCh1pb.Location = new System.Drawing.Point(0, 17);
            this.llCh1pb.Margin = new System.Windows.Forms.Padding(0);
            this.llCh1pb.Name = "llCh1pb";
            this.llCh1pb.Size = new System.Drawing.Size(20, 13);
            this.llCh1pb.TabIndex = 0;
            this.llCh1pb.TabStop = true;
            this.llCh1pb.Text = "C1";
            this.llCh1pb.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llCh1pb_LinkClicked);
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(209, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(55, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "Channel 3";
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(287, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(55, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "Channel 4";
            // 
            // numMaskCh3Hue
            // 
            this.numMaskCh3Hue.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.numMaskCh3Hue.DecimalPlaces = 6;
            this.numMaskCh3Hue.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numMaskCh3Hue.Location = new System.Drawing.Point(201, 16);
            this.numMaskCh3Hue.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMaskCh3Hue.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.numMaskCh3Hue.Name = "numMaskCh3Hue";
            this.numMaskCh3Hue.Size = new System.Drawing.Size(72, 20);
            this.numMaskCh3Hue.TabIndex = 2;
            // 
            // numMaskCh4Hue
            // 
            this.numMaskCh4Hue.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.numMaskCh4Hue.DecimalPlaces = 6;
            this.numMaskCh4Hue.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numMaskCh4Hue.Location = new System.Drawing.Point(279, 16);
            this.numMaskCh4Hue.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMaskCh4Hue.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.numMaskCh4Hue.Name = "numMaskCh4Hue";
            this.numMaskCh4Hue.Size = new System.Drawing.Size(72, 20);
            this.numMaskCh4Hue.TabIndex = 2;
            // 
            // numMaskCh3Saturation
            // 
            this.numMaskCh3Saturation.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.numMaskCh3Saturation.DecimalPlaces = 6;
            this.numMaskCh3Saturation.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numMaskCh3Saturation.Location = new System.Drawing.Point(201, 42);
            this.numMaskCh3Saturation.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMaskCh3Saturation.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.numMaskCh3Saturation.Name = "numMaskCh3Saturation";
            this.numMaskCh3Saturation.Size = new System.Drawing.Size(72, 20);
            this.numMaskCh3Saturation.TabIndex = 4;
            // 
            // numMaskCh4Saturation
            // 
            this.numMaskCh4Saturation.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.numMaskCh4Saturation.DecimalPlaces = 6;
            this.numMaskCh4Saturation.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numMaskCh4Saturation.Location = new System.Drawing.Point(279, 42);
            this.numMaskCh4Saturation.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMaskCh4Saturation.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.numMaskCh4Saturation.Name = "numMaskCh4Saturation";
            this.numMaskCh4Saturation.Size = new System.Drawing.Size(72, 20);
            this.numMaskCh4Saturation.TabIndex = 4;
            // 
            // numMaskCh3Value
            // 
            this.numMaskCh3Value.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.numMaskCh3Value.DecimalPlaces = 6;
            this.numMaskCh3Value.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numMaskCh3Value.Location = new System.Drawing.Point(201, 68);
            this.numMaskCh3Value.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMaskCh3Value.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.numMaskCh3Value.Name = "numMaskCh3Value";
            this.numMaskCh3Value.Size = new System.Drawing.Size(72, 20);
            this.numMaskCh3Value.TabIndex = 6;
            // 
            // numMaskCh4Value
            // 
            this.numMaskCh4Value.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.numMaskCh4Value.DecimalPlaces = 6;
            this.numMaskCh4Value.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numMaskCh4Value.Location = new System.Drawing.Point(279, 68);
            this.numMaskCh4Value.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMaskCh4Value.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.numMaskCh4Value.Name = "numMaskCh4Value";
            this.numMaskCh4Value.Size = new System.Drawing.Size(72, 20);
            this.numMaskCh4Value.TabIndex = 6;
            // 
            // nudCh3Red
            // 
            this.nudCh3Red.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.nudCh3Red.Location = new System.Drawing.Point(221, 94);
            this.nudCh3Red.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudCh3Red.Name = "nudCh3Red";
            this.nudCh3Red.Size = new System.Drawing.Size(52, 20);
            this.nudCh3Red.TabIndex = 2;
            // 
            // nudCh4Red
            // 
            this.nudCh4Red.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.nudCh4Red.Location = new System.Drawing.Point(299, 94);
            this.nudCh4Red.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudCh4Red.Name = "nudCh4Red";
            this.nudCh4Red.Size = new System.Drawing.Size(52, 20);
            this.nudCh4Red.TabIndex = 2;
            // 
            // nudCh3Green
            // 
            this.nudCh3Green.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.nudCh3Green.Location = new System.Drawing.Point(221, 120);
            this.nudCh3Green.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudCh3Green.Name = "nudCh3Green";
            this.nudCh3Green.Size = new System.Drawing.Size(52, 20);
            this.nudCh3Green.TabIndex = 4;
            // 
            // nudCh4Green
            // 
            this.nudCh4Green.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.nudCh4Green.Location = new System.Drawing.Point(299, 120);
            this.nudCh4Green.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudCh4Green.Name = "nudCh4Green";
            this.nudCh4Green.Size = new System.Drawing.Size(52, 20);
            this.nudCh4Green.TabIndex = 4;
            // 
            // nudCh3Blue
            // 
            this.nudCh3Blue.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.nudCh3Blue.Location = new System.Drawing.Point(221, 146);
            this.nudCh3Blue.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudCh3Blue.Name = "nudCh3Blue";
            this.nudCh3Blue.Size = new System.Drawing.Size(52, 20);
            this.nudCh3Blue.TabIndex = 6;
            // 
            // nudCh4Blue
            // 
            this.nudCh4Blue.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.nudCh4Blue.Location = new System.Drawing.Point(299, 146);
            this.nudCh4Blue.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudCh4Blue.Name = "nudCh4Blue";
            this.nudCh4Blue.Size = new System.Drawing.Size(52, 20);
            this.nudCh4Blue.TabIndex = 6;
            // 
            // nudCh3Alpha
            // 
            this.nudCh3Alpha.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.nudCh3Alpha.Location = new System.Drawing.Point(221, 172);
            this.nudCh3Alpha.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudCh3Alpha.Name = "nudCh3Alpha";
            this.nudCh3Alpha.Size = new System.Drawing.Size(52, 20);
            this.nudCh3Alpha.TabIndex = 8;
            this.nudCh3Alpha.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            // 
            // nudCh4Alpha
            // 
            this.nudCh4Alpha.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.nudCh4Alpha.Location = new System.Drawing.Point(299, 172);
            this.nudCh4Alpha.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudCh4Alpha.Name = "nudCh4Alpha";
            this.nudCh4Alpha.Size = new System.Drawing.Size(52, 20);
            this.nudCh4Alpha.TabIndex = 8;
            this.nudCh4Alpha.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.pbCh3, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.llCh3pb, 0, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(203, 198);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(68, 48);
            this.tableLayoutPanel2.TabIndex = 7;
            // 
            // pbCh3
            // 
            this.pbCh3.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pbCh3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pbCh3.Location = new System.Drawing.Point(20, 0);
            this.pbCh3.Margin = new System.Windows.Forms.Padding(0);
            this.pbCh3.Name = "pbCh3";
            this.pbCh3.Size = new System.Drawing.Size(48, 48);
            this.pbCh3.TabIndex = 10;
            this.pbCh3.TabStop = false;
            this.pbCh3.DoubleClick += new System.EventHandler(this.pbCh3_DoubleClick);
            // 
            // llCh3pb
            // 
            this.llCh3pb.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.llCh3pb.AutoSize = true;
            this.llCh3pb.Location = new System.Drawing.Point(0, 17);
            this.llCh3pb.Margin = new System.Windows.Forms.Padding(0);
            this.llCh3pb.Name = "llCh3pb";
            this.llCh3pb.Size = new System.Drawing.Size(20, 13);
            this.llCh3pb.TabIndex = 0;
            this.llCh3pb.TabStop = true;
            this.llCh3pb.Text = "C3";
            this.llCh3pb.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llCh3pb_LinkClicked);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(this.pbCh4, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.llCh4pb, 0, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(281, 198);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(68, 48);
            this.tableLayoutPanel3.TabIndex = 7;
            // 
            // pbCh4
            // 
            this.pbCh4.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pbCh4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pbCh4.Location = new System.Drawing.Point(20, 0);
            this.pbCh4.Margin = new System.Windows.Forms.Padding(0);
            this.pbCh4.Name = "pbCh4";
            this.pbCh4.Size = new System.Drawing.Size(48, 48);
            this.pbCh4.TabIndex = 10;
            this.pbCh4.TabStop = false;
            this.pbCh4.DoubleClick += new System.EventHandler(this.pbCh4_DoubleClick);
            // 
            // llCh4pb
            // 
            this.llCh4pb.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.llCh4pb.AutoSize = true;
            this.llCh4pb.Location = new System.Drawing.Point(0, 17);
            this.llCh4pb.Margin = new System.Windows.Forms.Padding(0);
            this.llCh4pb.Name = "llCh4pb";
            this.llCh4pb.Size = new System.Drawing.Size(20, 13);
            this.llCh4pb.TabIndex = 0;
            this.llCh4pb.TabStop = true;
            this.llCh4pb.Text = "C4";
            this.llCh4pb.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llCh4pb_LinkClicked);
            // 
            // label10
            // 
            this.label10.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(1, 215);
            this.label10.Margin = new System.Windows.Forms.Padding(0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(41, 13);
            this.label10.TabIndex = 7;
            this.label10.Text = "Images";
            // 
            // label21
            // 
            this.label21.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(0, 257);
            this.label21.Margin = new System.Windows.Forms.Padding(0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(42, 13);
            this.label21.TabIndex = 7;
            this.label21.Text = "Disable";
            // 
            // ckbNoCh1
            // 
            this.ckbNoCh1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbNoCh1.AutoSize = true;
            this.ckbNoCh1.Location = new System.Drawing.Point(73, 257);
            this.ckbNoCh1.Name = "ckbNoCh1";
            this.ckbNoCh1.Size = new System.Drawing.Size(15, 14);
            this.ckbNoCh1.TabIndex = 9;
            this.ckbNoCh1.UseVisualStyleBackColor = true;
            // 
            // ckbNoCh2
            // 
            this.ckbNoCh2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbNoCh2.AutoSize = true;
            this.ckbNoCh2.Location = new System.Drawing.Point(151, 257);
            this.ckbNoCh2.Name = "ckbNoCh2";
            this.ckbNoCh2.Size = new System.Drawing.Size(15, 14);
            this.ckbNoCh2.TabIndex = 9;
            this.ckbNoCh2.UseVisualStyleBackColor = true;
            // 
            // ckbNoCh3
            // 
            this.ckbNoCh3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbNoCh3.AutoSize = true;
            this.ckbNoCh3.Location = new System.Drawing.Point(229, 257);
            this.ckbNoCh3.Name = "ckbNoCh3";
            this.ckbNoCh3.Size = new System.Drawing.Size(15, 14);
            this.ckbNoCh3.TabIndex = 9;
            this.ckbNoCh3.UseVisualStyleBackColor = true;
            // 
            // ckbNoCh4
            // 
            this.ckbNoCh4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbNoCh4.AutoSize = true;
            this.ckbNoCh4.Location = new System.Drawing.Point(307, 257);
            this.ckbNoCh4.Name = "ckbNoCh4";
            this.ckbNoCh4.Size = new System.Drawing.Size(15, 14);
            this.ckbNoCh4.TabIndex = 9;
            this.ckbNoCh4.UseVisualStyleBackColor = true;
            // 
            // ddsMask
            // 
            this.ddsMask.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ddsMask.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.ddsMask.Location = new System.Drawing.Point(162, 0);
            this.ddsMask.Margin = new System.Windows.Forms.Padding(0);
            this.ddsMask.MaxSize = new System.Drawing.Size(48, 48);
            this.ddsMask.Name = "ddsMask";
            this.ddsMask.ShowChannelSelector = false;
            this.ddsMask.Size = new System.Drawing.Size(48, 48);
            this.ddsMask.TabIndex = 2;
            this.ddsMask.TabStop = false;
            this.ddsMask.DoubleClick += new System.EventHandler(this.ddsMask_DoubleClick);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "dds";
            this.saveFileDialog1.FileName = "*.dds";
            this.saveFileDialog1.Filter = "DDS Images|*.dds|All files|*.*";
            this.saveFileDialog1.Title = "Save DDS as...";
            // 
            // btnSave
            // 
            this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSave.Location = new System.Drawing.Point(3, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save DDS...";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.AutoSize = true;
            this.tableLayoutPanel5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel5.ColumnCount = 5;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel6, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.btnHSVShift, 4, 1);
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel1, 4, 0);
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel4, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.btnSetColour, 1, 1);
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel10, 2, 0);
            this.tableLayoutPanel5.Controls.Add(this.btnClear, 2, 1);
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 9);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 2;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(396, 146);
            this.tableLayoutPanel5.TabIndex = 8;
            // 
            // tableLayoutPanel10
            // 
            this.tableLayoutPanel10.AutoSize = true;
            this.tableLayoutPanel10.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel10.ColumnCount = 1;
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel10.Controls.Add(this.btnExport, 0, 1);
            this.tableLayoutPanel10.Controls.Add(this.btnSave, 0, 0);
            this.tableLayoutPanel10.Location = new System.Drawing.Point(181, 0);
            this.tableLayoutPanel10.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel10.Name = "tableLayoutPanel10";
            this.tableLayoutPanel10.RowCount = 2;
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel10.Size = new System.Drawing.Size(81, 58);
            this.tableLayoutPanel10.TabIndex = 9;
            // 
            // btnExport
            // 
            this.btnExport.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnExport.Location = new System.Drawing.Point(3, 32);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 3;
            this.btnExport.Text = "Export...";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.tableLayoutPanel6.AutoSize = true;
            this.tableLayoutPanel6.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel6.ColumnCount = 1;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.Controls.Add(this.btnOpenImage, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.btnImport, 0, 1);
            this.tableLayoutPanel6.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel6.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 2;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(81, 58);
            this.tableLayoutPanel6.TabIndex = 9;
            // 
            // btnImport
            // 
            this.btnImport.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnImport.Location = new System.Drawing.Point(3, 32);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(75, 23);
            this.btnImport.TabIndex = 1;
            this.btnImport.Text = "Import...";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // ddsPanel1
            // 
            this.ddsPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ddsPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ddsPanel1.Fit = true;
            this.ddsPanel1.Location = new System.Drawing.Point(453, 5);
            this.ddsPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.ddsPanel1.MaxSize = new System.Drawing.Size(0, 0);
            this.ddsPanel1.Name = "ddsPanel1";
            this.ddsPanel1.Size = new System.Drawing.Size(513, 542);
            this.ddsPanel1.TabIndex = 5;
            // 
            // HSVTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(975, 557);
            this.Controls.Add(this.tableLayoutPanel5);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ddsPanel1);
            this.Name = "HSVTest";
            this.Text = "DDSPanel Test Tool";
            ((System.ComponentModel.ISupportInitialize)(this.hueShift)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.saturationShift)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.valueShift)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaskCh1Hue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaskCh1Saturation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaskCh1Value)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaskCh2Hue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaskCh2Saturation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaskCh2Value)).EndInit();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGreen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBlue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAlpha)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCh1Red)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCh1Green)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCh1Blue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCh1Alpha)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCh2Red)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCh2Green)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCh2Blue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCh2Alpha)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tableLayoutPanel11.ResumeLayout(false);
            this.tableLayoutPanel11.PerformLayout();
            this.tableLayoutPanel9.ResumeLayout(false);
            this.tableLayoutPanel9.PerformLayout();
            this.tableLayoutPanel8.ResumeLayout(false);
            this.tableLayoutPanel8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbCh2)).EndInit();
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbCh1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaskCh3Hue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaskCh4Hue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaskCh3Saturation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaskCh4Saturation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaskCh3Value)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaskCh4Value)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCh3Red)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCh4Red)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCh3Green)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCh4Green)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCh3Blue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCh4Blue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCh3Alpha)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCh4Alpha)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbCh3)).EndInit();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbCh4)).EndInit();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.tableLayoutPanel10.ResumeLayout(false);
            this.tableLayoutPanel6.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DDSPanel.DDSPanel ddsPanel1;
        private System.Windows.Forms.NumericUpDown hueShift;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.NumericUpDown saturationShift;
        private System.Windows.Forms.NumericUpDown valueShift;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnOpenImage;
        private System.Windows.Forms.Button btnOpenMask;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.NumericUpDown numMaskCh1Hue;
        private System.Windows.Forms.NumericUpDown numMaskCh1Saturation;
        private System.Windows.Forms.NumericUpDown numMaskCh1Value;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnResetMask;
        private System.Windows.Forms.CheckBox ckbBlend;
        private System.Windows.Forms.NumericUpDown numMaskCh2Hue;
        private System.Windows.Forms.NumericUpDown numMaskCh2Saturation;
        private System.Windows.Forms.NumericUpDown numMaskCh2Value;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnApplyShift;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.NumericUpDown nudRed;
        private System.Windows.Forms.NumericUpDown nudGreen;
        private System.Windows.Forms.NumericUpDown nudBlue;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btnSetColour;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.NumericUpDown nudAlpha;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.NumericUpDown nudCh1Red;
        private System.Windows.Forms.NumericUpDown nudCh1Green;
        private System.Windows.Forms.NumericUpDown nudCh1Blue;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.NumericUpDown nudCh1Alpha;
        private System.Windows.Forms.NumericUpDown nudCh2Red;
        private System.Windows.Forms.NumericUpDown nudCh2Green;
        private System.Windows.Forms.NumericUpDown nudCh2Blue;
        private System.Windows.Forms.NumericUpDown nudCh2Alpha;
        private System.Windows.Forms.Button btnApplyColour;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.GroupBox groupBox1;
        private DDSPanel.DDSPanel ddsMask;
        private System.Windows.Forms.Button btnHSVShift;
        private System.Windows.Forms.PictureBox pbCh1;
        private System.Windows.Forms.Button btnApplyImage;
        private System.Windows.Forms.PictureBox pbCh2;
        private System.Windows.Forms.LinkLabel llCh1pb;
        private System.Windows.Forms.LinkLabel llCh2pb;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel11;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel9;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel8;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown numMaskCh3Hue;
        private System.Windows.Forms.NumericUpDown numMaskCh4Hue;
        private System.Windows.Forms.NumericUpDown numMaskCh3Saturation;
        private System.Windows.Forms.NumericUpDown numMaskCh4Saturation;
        private System.Windows.Forms.NumericUpDown numMaskCh3Value;
        private System.Windows.Forms.NumericUpDown numMaskCh4Value;
        private System.Windows.Forms.NumericUpDown nudCh3Red;
        private System.Windows.Forms.NumericUpDown nudCh4Red;
        private System.Windows.Forms.NumericUpDown nudCh3Green;
        private System.Windows.Forms.NumericUpDown nudCh4Green;
        private System.Windows.Forms.NumericUpDown nudCh3Blue;
        private System.Windows.Forms.NumericUpDown nudCh4Blue;
        private System.Windows.Forms.NumericUpDown nudCh3Alpha;
        private System.Windows.Forms.NumericUpDown nudCh4Alpha;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.PictureBox pbCh3;
        private System.Windows.Forms.LinkLabel llCh3pb;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.PictureBox pbCh4;
        private System.Windows.Forms.LinkLabel llCh4pb;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.CheckBox ckbNoCh1;
        private System.Windows.Forms.CheckBox ckbNoCh2;
        private System.Windows.Forms.CheckBox ckbNoCh3;
        private System.Windows.Forms.CheckBox ckbNoCh4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel10;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.Button btnImport;
    }
}