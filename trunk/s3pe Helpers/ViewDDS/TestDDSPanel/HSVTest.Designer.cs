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
            this.btnOpenImage = new System.Windows.Forms.Button();
            this.btnOpenMask = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.numMaskCh1Hue = new System.Windows.Forms.NumericUpDown();
            this.numMaskCh1Saturation = new System.Windows.Forms.NumericUpDown();
            this.numMaskCh1Value = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.btnResetMask = new System.Windows.Forms.Button();
            this.ddsPanel1 = new DDSPanel.DDSPanel();
            ((System.ComponentModel.ISupportInitialize)(this.hueShift)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.saturationShift)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.valueShift)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaskCh1Hue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaskCh1Saturation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaskCh1Value)).BeginInit();
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
            this.hueShift.Location = new System.Drawing.Point(45, 3);
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
            this.hueShift.Size = new System.Drawing.Size(83, 20);
            this.hueShift.TabIndex = 1;
            this.hueShift.ValueChanged += new System.EventHandler(this.ned_ValueChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.hueShift, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.saturationShift, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.valueShift, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(2, 9);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(131, 79);
            this.tableLayoutPanel1.TabIndex = 2;
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
            this.saturationShift.Location = new System.Drawing.Point(45, 29);
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
            this.saturationShift.Size = new System.Drawing.Size(83, 20);
            this.saturationShift.TabIndex = 1;
            this.saturationShift.ValueChanged += new System.EventHandler(this.ned_ValueChanged);
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
            this.valueShift.Location = new System.Drawing.Point(45, 55);
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
            this.valueShift.Size = new System.Drawing.Size(83, 20);
            this.valueShift.TabIndex = 1;
            this.valueShift.ValueChanged += new System.EventHandler(this.ned_ValueChanged);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "HShift";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "SShift";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "VShift";
            // 
            // btnOpenImage
            // 
            this.btnOpenImage.Location = new System.Drawing.Point(12, 94);
            this.btnOpenImage.Name = "btnOpenImage";
            this.btnOpenImage.Size = new System.Drawing.Size(75, 23);
            this.btnOpenImage.TabIndex = 3;
            this.btnOpenImage.Text = "Base...";
            this.btnOpenImage.UseVisualStyleBackColor = true;
            this.btnOpenImage.Click += new System.EventHandler(this.btnOpenImage_Click);
            // 
            // btnOpenMask
            // 
            this.btnOpenMask.Location = new System.Drawing.Point(12, 123);
            this.btnOpenMask.Name = "btnOpenMask";
            this.btnOpenMask.Size = new System.Drawing.Size(75, 23);
            this.btnOpenMask.TabIndex = 3;
            this.btnOpenMask.Text = "Mask...";
            this.btnOpenMask.UseVisualStyleBackColor = true;
            this.btnOpenMask.Click += new System.EventHandler(this.btnOpenMask_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "dds";
            this.openFileDialog1.FileName = "*.dds";
            this.openFileDialog1.Filter = "DDS Images|*.dds|All files|*.*";
            this.openFileDialog1.Title = "Choose a DDS Image";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.numMaskCh1Hue, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.numMaskCh1Saturation, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.numMaskCh1Value, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.label4, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label5, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.label6, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.label7, 0, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(2, 152);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(131, 92);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // numMaskCh1Hue
            // 
            this.numMaskCh1Hue.Anchor = System.Windows.Forms.AnchorStyles.None;
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
            this.numMaskCh1Hue.Size = new System.Drawing.Size(83, 20);
            this.numMaskCh1Hue.TabIndex = 1;
            this.numMaskCh1Hue.ValueChanged += new System.EventHandler(this.numMask_ValueChanged);
            // 
            // numMaskCh1Saturation
            // 
            this.numMaskCh1Saturation.Anchor = System.Windows.Forms.AnchorStyles.None;
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
            this.numMaskCh1Saturation.Size = new System.Drawing.Size(83, 20);
            this.numMaskCh1Saturation.TabIndex = 1;
            this.numMaskCh1Saturation.ValueChanged += new System.EventHandler(this.numMask_ValueChanged);
            // 
            // numMaskCh1Value
            // 
            this.numMaskCh1Value.Anchor = System.Windows.Forms.AnchorStyles.None;
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
            this.numMaskCh1Value.Size = new System.Drawing.Size(83, 20);
            this.numMaskCh1Value.TabIndex = 1;
            this.numMaskCh1Value.ValueChanged += new System.EventHandler(this.numMask_ValueChanged);
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "HShift";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 45);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "SShift";
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 72);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "VShift";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.tableLayoutPanel2.SetColumnSpan(this.label7, 2);
            this.label7.Location = new System.Drawing.Point(3, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(50, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Ch1 Shift";
            // 
            // btnResetMask
            // 
            this.btnResetMask.Location = new System.Drawing.Point(12, 250);
            this.btnResetMask.Name = "btnResetMask";
            this.btnResetMask.Size = new System.Drawing.Size(75, 23);
            this.btnResetMask.TabIndex = 3;
            this.btnResetMask.Text = "Reset";
            this.btnResetMask.UseVisualStyleBackColor = true;
            this.btnResetMask.Click += new System.EventHandler(this.btnResetMask_Click);
            // 
            // ddsPanel1
            // 
            this.ddsPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ddsPanel1.Location = new System.Drawing.Point(141, 4);
            this.ddsPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.ddsPanel1.MaxSize = new System.Drawing.Size(0, 0);
            this.ddsPanel1.Name = "ddsPanel1";
            this.ddsPanel1.Size = new System.Drawing.Size(417, 451);
            this.ddsPanel1.TabIndex = 0;
            // 
            // HSVTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(567, 465);
            this.Controls.Add(this.btnResetMask);
            this.Controls.Add(this.btnOpenMask);
            this.Controls.Add(this.btnOpenImage);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.ddsPanel1);
            this.Name = "HSVTest";
            this.Text = "HSVTest";
            ((System.ComponentModel.ISupportInitialize)(this.hueShift)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.saturationShift)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.valueShift)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaskCh1Hue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaskCh1Saturation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaskCh1Value)).EndInit();
            this.ResumeLayout(false);

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
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.NumericUpDown numMaskCh1Hue;
        private System.Windows.Forms.NumericUpDown numMaskCh1Saturation;
        private System.Windows.Forms.NumericUpDown numMaskCh1Value;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnResetMask;
    }
}