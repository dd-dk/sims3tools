namespace DDSPanel
{
    partial class DDSPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.ckbR = new System.Windows.Forms.CheckBox();
            this.ckbG = new System.Windows.Forms.CheckBox();
            this.ckbB = new System.Windows.Forms.CheckBox();
            this.ckbA = new System.Windows.Forms.CheckBox();
            this.ckbI = new System.Windows.Forms.CheckBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(379, 157);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Location = new System.Drawing.Point(0, 29);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(379, 128);
            this.panel1.TabIndex = 2;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.Control;
            this.pictureBox1.BackgroundImage = global::DDSPanel.Properties.Resources.checkerboard;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(128, 128);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Controls.Add(this.ckbR);
            this.flowLayoutPanel1.Controls.Add(this.ckbG);
            this.flowLayoutPanel1.Controls.Add(this.ckbB);
            this.flowLayoutPanel1.Controls.Add(this.ckbA);
            this.flowLayoutPanel1.Controls.Add(this.ckbI);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 3);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(379, 23);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Enable channels:";
            // 
            // ckbR
            // 
            this.ckbR.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbR.AutoSize = true;
            this.ckbR.Checked = true;
            this.ckbR.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbR.Location = new System.Drawing.Point(98, 3);
            this.ckbR.Name = "ckbR";
            this.ckbR.Size = new System.Drawing.Size(46, 17);
            this.ckbR.TabIndex = 0;
            this.ckbR.Text = "Red";
            this.ckbR.UseVisualStyleBackColor = true;
            this.ckbR.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
            // 
            // ckbG
            // 
            this.ckbG.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbG.AutoSize = true;
            this.ckbG.Checked = true;
            this.ckbG.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbG.Location = new System.Drawing.Point(150, 3);
            this.ckbG.Name = "ckbG";
            this.ckbG.Size = new System.Drawing.Size(55, 17);
            this.ckbG.TabIndex = 1;
            this.ckbG.Text = "Green";
            this.ckbG.UseVisualStyleBackColor = true;
            this.ckbG.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
            // 
            // ckbB
            // 
            this.ckbB.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbB.AutoSize = true;
            this.ckbB.Checked = true;
            this.ckbB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbB.Location = new System.Drawing.Point(211, 3);
            this.ckbB.Name = "ckbB";
            this.ckbB.Size = new System.Drawing.Size(47, 17);
            this.ckbB.TabIndex = 2;
            this.ckbB.Text = "Blue";
            this.ckbB.UseVisualStyleBackColor = true;
            this.ckbB.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
            // 
            // ckbA
            // 
            this.ckbA.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbA.AutoSize = true;
            this.ckbA.Checked = true;
            this.ckbA.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbA.Location = new System.Drawing.Point(264, 3);
            this.ckbA.Name = "ckbA";
            this.ckbA.Size = new System.Drawing.Size(53, 17);
            this.ckbA.TabIndex = 3;
            this.ckbA.Text = "Alpha";
            this.ckbA.UseVisualStyleBackColor = true;
            this.ckbA.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
            // 
            // ckbI
            // 
            this.ckbI.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbI.AutoSize = true;
            this.ckbI.Location = new System.Drawing.Point(323, 3);
            this.ckbI.Name = "ckbI";
            this.ckbI.Size = new System.Drawing.Size(53, 17);
            this.ckbI.TabIndex = 5;
            this.ckbI.Text = "Invert";
            this.ckbI.UseVisualStyleBackColor = true;
            this.ckbI.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // DDSPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "DDSPanel";
            this.Size = new System.Drawing.Size(379, 157);
            this.Resize += new System.EventHandler(this.DDSPanel_Resize);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox ckbR;
        private System.Windows.Forms.CheckBox ckbG;
        private System.Windows.Forms.CheckBox ckbB;
        private System.Windows.Forms.CheckBox ckbA;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.CheckBox ckbI;
    }
}
