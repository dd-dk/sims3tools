namespace ObjectCloner.SplitterComponents
{
    partial class CloneFixOptions
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
            this.ckbCompress = new System.Windows.Forms.CheckBox();
            this.tbUniqueName = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.ckb32bitIIDs = new System.Windows.Forms.CheckBox();
            this.label23 = new System.Windows.Forms.Label();
            this.ckbKeepSTBLIIDs = new System.Windows.Forms.CheckBox();
            this.ckbRenumber = new System.Windows.Forms.CheckBox();
            this.label22 = new System.Windows.Forms.Label();
            this.tlpOptions = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.ckbThumbs = new System.Windows.Forms.CheckBox();
            this.ckbPadSTBLs = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.ckbClone = new System.Windows.Forms.CheckBox();
            this.ckbDeepClone = new System.Windows.Forms.CheckBox();
            this.ckbExclCommon = new System.Windows.Forms.CheckBox();
            this.ckbIncludePresets = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tlpCustomise = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.ckbRepair = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel5.SuspendLayout();
            this.tlpOptions.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tlpCustomise.SuspendLayout();
            this.SuspendLayout();
            // 
            // ckbCompress
            // 
            this.ckbCompress.AutoSize = true;
            this.ckbCompress.Checked = true;
            this.ckbCompress.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tlpOptions.SetColumnSpan(this.ckbCompress, 2);
            this.ckbCompress.Location = new System.Drawing.Point(35, 511);
            this.ckbCompress.Name = "ckbCompress";
            this.ckbCompress.Size = new System.Drawing.Size(121, 17);
            this.ckbCompress.TabIndex = 6;
            this.ckbCompress.Text = "Enable compression";
            this.ckbCompress.UseVisualStyleBackColor = false;
            // 
            // tbUniqueName
            // 
            this.tbUniqueName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpCustomise.SetColumnSpan(this.tbUniqueName, 2);
            this.tbUniqueName.Location = new System.Drawing.Point(3, 155);
            this.tbUniqueName.Name = "tbUniqueName";
            this.tbUniqueName.Size = new System.Drawing.Size(259, 20);
            this.tbUniqueName.TabIndex = 5;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.AutoSize = true;
            this.tableLayoutPanel5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel5.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.Controls.Add(this.ckb32bitIIDs, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.label23, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.ckbKeepSTBLIIDs, 0, 2);
            this.tableLayoutPanel5.Location = new System.Drawing.Point(27, 77);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 3;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.Size = new System.Drawing.Size(162, 59);
            this.tableLayoutPanel5.TabIndex = 3;
            // 
            // ckb32bitIIDs
            // 
            this.ckb32bitIIDs.AutoSize = true;
            this.tableLayoutPanel5.SetColumnSpan(this.ckb32bitIIDs, 2);
            this.ckb32bitIIDs.Location = new System.Drawing.Point(3, 16);
            this.ckb32bitIIDs.Name = "ckb32bitIIDs";
            this.ckb32bitIIDs.Size = new System.Drawing.Size(156, 17);
            this.ckb32bitIIDs.TabIndex = 2;
            this.ckb32bitIIDs.Text = "Use 32bit OBJD/OBJK IIDs";
            this.ckb32bitIIDs.UseVisualStyleBackColor = false;
            // 
            // label23
            // 
            this.label23.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label23.AutoSize = true;
            this.label23.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.label23.Location = new System.Drawing.Point(52, 0);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(81, 13);
            this.label23.TabIndex = 1;
            this.label23.Text = "Advanced only!";
            // 
            // ckbKeepSTBLIIDs
            // 
            this.ckbKeepSTBLIIDs.AutoSize = true;
            this.tableLayoutPanel5.SetColumnSpan(this.ckbKeepSTBLIIDs, 2);
            this.ckbKeepSTBLIIDs.Location = new System.Drawing.Point(3, 39);
            this.ckbKeepSTBLIIDs.Name = "ckbKeepSTBLIIDs";
            this.ckbKeepSTBLIIDs.Size = new System.Drawing.Size(139, 17);
            this.ckbKeepSTBLIIDs.TabIndex = 3;
            this.ckbKeepSTBLIIDs.Text = "Keep original STBL IIDs";
            this.ckbKeepSTBLIIDs.UseVisualStyleBackColor = false;
            // 
            // ckbRenumber
            // 
            this.ckbRenumber.AutoSize = true;
            this.ckbRenumber.Checked = true;
            this.ckbRenumber.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tlpCustomise.SetColumnSpan(this.ckbRenumber, 2);
            this.ckbRenumber.Location = new System.Drawing.Point(3, 54);
            this.ckbRenumber.Name = "ckbRenumber";
            this.ckbRenumber.Size = new System.Drawing.Size(159, 17);
            this.ckbRenumber.TabIndex = 2;
            this.ckbRenumber.Text = "Renumber/rename internally";
            this.ckbRenumber.UseVisualStyleBackColor = true;
            this.ckbRenumber.CheckedChanged += new System.EventHandler(this.ckbRenumber_CheckedChanged);
            // 
            // label22
            // 
            this.label22.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label22.AutoSize = true;
            this.tlpCustomise.SetColumnSpan(this.label22, 2);
            this.label22.ForeColor = System.Drawing.Color.Red;
            this.label22.Location = new System.Drawing.Point(3, 3);
            this.label22.Margin = new System.Windows.Forms.Padding(3);
            this.label22.Name = "label22";
            this.label22.Padding = new System.Windows.Forms.Padding(3);
            this.label22.Size = new System.Drawing.Size(259, 45);
            this.label22.TabIndex = 1;
            this.label22.Text = "If you intend to remove any parts of the scenegraph\r\nchain in order for the objec" +
                "t to use original resouces,\r\nyou must do so before renumbering internally";
            // 
            // tlpOptions
            // 
            this.tlpOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpOptions.ColumnCount = 4;
            this.tlpOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tlpOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpOptions.Controls.Add(this.ckbCompress, 1, 11);
            this.tlpOptions.Controls.Add(this.flowLayoutPanel1, 1, 12);
            this.tlpOptions.Controls.Add(this.ckbThumbs, 1, 7);
            this.tlpOptions.Controls.Add(this.ckbPadSTBLs, 1, 6);
            this.tlpOptions.Controls.Add(this.label1, 1, 1);
            this.tlpOptions.Controls.Add(this.groupBox1, 1, 3);
            this.tlpOptions.Controls.Add(this.groupBox2, 1, 9);
            this.tlpOptions.Controls.Add(this.ckbRepair, 1, 5);
            this.tlpOptions.Location = new System.Drawing.Point(0, 0);
            this.tlpOptions.Name = "tlpOptions";
            this.tlpOptions.RowCount = 14;
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpOptions.Size = new System.Drawing.Size(348, 587);
            this.tlpOptions.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.tlpOptions.SetColumnSpan(this.flowLayoutPanel1, 2);
            this.flowLayoutPanel1.Controls.Add(this.btnCancel);
            this.flowLayoutPanel1.Controls.Add(this.btnStart);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(150, 534);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(162, 29);
            this.flowLayoutPanel1.TabIndex = 7;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(3, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "C&ancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(84, 3);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Sta&rt";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // ckbThumbs
            // 
            this.ckbThumbs.AutoSize = true;
            this.tlpOptions.SetColumnSpan(this.ckbThumbs, 2);
            this.ckbThumbs.Location = new System.Drawing.Point(35, 242);
            this.ckbThumbs.Name = "ckbThumbs";
            this.ckbThumbs.Size = new System.Drawing.Size(114, 17);
            this.ckbThumbs.TabIndex = 4;
            this.ckbThumbs.Text = "Include thumbnails";
            this.ckbThumbs.UseVisualStyleBackColor = true;
            // 
            // ckbPadSTBLs
            // 
            this.ckbPadSTBLs.AutoSize = true;
            this.tlpOptions.SetColumnSpan(this.ckbPadSTBLs, 2);
            this.ckbPadSTBLs.Location = new System.Drawing.Point(35, 219);
            this.ckbPadSTBLs.Name = "ckbPadSTBLs";
            this.ckbPadSTBLs.Size = new System.Drawing.Size(153, 17);
            this.ckbPadSTBLs.TabIndex = 3;
            this.ckbPadSTBLs.Text = "Create missing string tables";
            this.ckbPadSTBLs.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.tlpOptions.SetColumnSpan(this.label1, 2);
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(262, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Options";
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpOptions.SetColumnSpan(this.groupBox1, 2);
            this.groupBox1.Controls.Add(this.tableLayoutPanel1);
            this.groupBox1.Location = new System.Drawing.Point(35, 48);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(199, 130);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Make clone";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.ckbClone, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ckbDeepClone, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.ckbExclCommon, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.ckbIncludePresets, 1, 3);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 19);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(187, 92);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // ckbClone
            // 
            this.ckbClone.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.ckbClone, 2);
            this.ckbClone.Location = new System.Drawing.Point(3, 3);
            this.ckbClone.Name = "ckbClone";
            this.ckbClone.Size = new System.Drawing.Size(131, 17);
            this.ckbClone.TabIndex = 1;
            this.ckbClone.Text = "Create clone package";
            this.ckbClone.UseVisualStyleBackColor = true;
            this.ckbClone.CheckedChanged += new System.EventHandler(this.ckbClone_CheckedChanged);
            // 
            // ckbDeepClone
            // 
            this.ckbDeepClone.AutoSize = true;
            this.ckbDeepClone.Location = new System.Drawing.Point(27, 26);
            this.ckbDeepClone.Name = "ckbDeepClone";
            this.ckbDeepClone.Size = new System.Drawing.Size(81, 17);
            this.ckbDeepClone.TabIndex = 2;
            this.ckbDeepClone.Text = "Deep clone";
            this.ckbDeepClone.UseVisualStyleBackColor = true;
            // 
            // ckbExclCommon
            // 
            this.ckbExclCommon.AutoSize = true;
            this.ckbExclCommon.Location = new System.Drawing.Point(27, 49);
            this.ckbExclCommon.Name = "ckbExclCommon";
            this.ckbExclCommon.Size = new System.Drawing.Size(157, 17);
            this.ckbExclCommon.TabIndex = 3;
            this.ckbExclCommon.Text = "Exclude Common Resource";
            this.ckbExclCommon.UseVisualStyleBackColor = true;
            this.ckbExclCommon.Visible = false;
            // 
            // ckbIncludePresets
            // 
            this.ckbIncludePresets.AutoSize = true;
            this.ckbIncludePresets.Location = new System.Drawing.Point(27, 72);
            this.ckbIncludePresets.Name = "ckbIncludePresets";
            this.ckbIncludePresets.Size = new System.Drawing.Size(131, 17);
            this.ckbIncludePresets.TabIndex = 3;
            this.ckbIncludePresets.Text = "Include Preset Images";
            this.ckbIncludePresets.UseVisualStyleBackColor = true;
            this.ckbIncludePresets.Visible = false;
            // 
            // groupBox2
            // 
            this.groupBox2.AutoSize = true;
            this.groupBox2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpOptions.SetColumnSpan(this.groupBox2, 2);
            this.groupBox2.Controls.Add(this.tlpCustomise);
            this.groupBox2.Location = new System.Drawing.Point(35, 277);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(277, 216);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Make object unique";
            // 
            // tlpCustomise
            // 
            this.tlpCustomise.AutoSize = true;
            this.tlpCustomise.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpCustomise.ColumnCount = 2;
            this.tlpCustomise.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tlpCustomise.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpCustomise.Controls.Add(this.tableLayoutPanel5, 1, 2);
            this.tlpCustomise.Controls.Add(this.label22, 0, 0);
            this.tlpCustomise.Controls.Add(this.label2, 0, 3);
            this.tlpCustomise.Controls.Add(this.tbUniqueName, 0, 4);
            this.tlpCustomise.Controls.Add(this.ckbRenumber, 0, 1);
            this.tlpCustomise.Location = new System.Drawing.Point(6, 19);
            this.tlpCustomise.Name = "tlpCustomise";
            this.tlpCustomise.RowCount = 5;
            this.tlpCustomise.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpCustomise.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpCustomise.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpCustomise.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpCustomise.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpCustomise.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpCustomise.Size = new System.Drawing.Size(265, 178);
            this.tlpCustomise.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.tlpCustomise.SetColumnSpan(this.label2, 2);
            this.label2.Location = new System.Drawing.Point(3, 139);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(258, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Unique name (used as internal name and hash seed):";
            // 
            // ckbRepair
            // 
            this.ckbRepair.AutoSize = true;
            this.tlpOptions.SetColumnSpan(this.ckbRepair, 2);
            this.ckbRepair.Location = new System.Drawing.Point(35, 196);
            this.ckbRepair.Name = "ckbRepair";
            this.ckbRepair.Size = new System.Drawing.Size(132, 17);
            this.ckbRepair.TabIndex = 3;
            this.ckbRepair.Text = "Find missing resources";
            this.ckbRepair.UseVisualStyleBackColor = true;
            // 
            // CloneFixOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlpOptions);
            this.Name = "CloneFixOptions";
            this.Size = new System.Drawing.Size(348, 587);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.tlpOptions.ResumeLayout(false);
            this.tlpOptions.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tlpCustomise.ResumeLayout(false);
            this.tlpCustomise.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox ckbCompress;
        private System.Windows.Forms.TextBox tbUniqueName;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.CheckBox ckbRenumber;
        private System.Windows.Forms.CheckBox ckb32bitIIDs;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TableLayoutPanel tlpOptions;
        private System.Windows.Forms.CheckBox ckbClone;
        private System.Windows.Forms.CheckBox ckbPadSTBLs;
        private System.Windows.Forms.CheckBox ckbThumbs;
        private System.Windows.Forms.CheckBox ckbDeepClone;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.TableLayoutPanel tlpCustomise;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox ckbExclCommon;
        private System.Windows.Forms.CheckBox ckbIncludePresets;
        private System.Windows.Forms.CheckBox ckbKeepSTBLIIDs;
        private System.Windows.Forms.CheckBox ckbRepair;
    }
}
