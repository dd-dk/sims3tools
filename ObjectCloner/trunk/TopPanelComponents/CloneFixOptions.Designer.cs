﻿namespace ObjectCloner.TopPanelComponents
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
            this.ckbExcludeCatalogResources = new System.Windows.Forms.CheckBox();
            this.label23 = new System.Windows.Forms.Label();
            this.ckbRenumber = new System.Windows.Forms.CheckBox();
            this.label22 = new System.Windows.Forms.Label();
            this.tlpOptions = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.tlpCustomise = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.ckbPadSTBLs = new System.Windows.Forms.CheckBox();
            this.ckbClone = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ckbDefault = new System.Windows.Forms.CheckBox();
            this.ckbThumbs = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel5.SuspendLayout();
            this.tlpOptions.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.tlpCustomise.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ckbCompress
            // 
            this.ckbCompress.AutoSize = true;
            this.ckbCompress.Checked = true;
            this.ckbCompress.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tlpOptions.SetColumnSpan(this.ckbCompress, 5);
            this.ckbCompress.Location = new System.Drawing.Point(35, 429);
            this.ckbCompress.Name = "ckbCompress";
            this.ckbCompress.Size = new System.Drawing.Size(121, 17);
            this.ckbCompress.TabIndex = 8;
            this.ckbCompress.Text = "Enable compression";
            this.ckbCompress.UseVisualStyleBackColor = false;
            // 
            // tbUniqueName
            // 
            this.tbUniqueName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpCustomise.SetColumnSpan(this.tbUniqueName, 2);
            this.tbUniqueName.Location = new System.Drawing.Point(3, 132);
            this.tbUniqueName.Name = "tbUniqueName";
            this.tbUniqueName.Size = new System.Drawing.Size(259, 20);
            this.tbUniqueName.TabIndex = 1;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.AutoSize = true;
            this.tableLayoutPanel5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel5.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.Controls.Add(this.ckbExcludeCatalogResources, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.label23, 1, 0);
            this.tableLayoutPanel5.Location = new System.Drawing.Point(27, 77);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 2;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(184, 36);
            this.tableLayoutPanel5.TabIndex = 2;
            // 
            // ckbExcludeCatalogResources
            // 
            this.ckbExcludeCatalogResources.AutoSize = true;
            this.tableLayoutPanel5.SetColumnSpan(this.ckbExcludeCatalogResources, 2);
            this.ckbExcludeCatalogResources.Location = new System.Drawing.Point(3, 16);
            this.ckbExcludeCatalogResources.Name = "ckbExcludeCatalogResources";
            this.ckbExcludeCatalogResources.Size = new System.Drawing.Size(178, 17);
            this.ckbExcludeCatalogResources.TabIndex = 1;
            this.ckbExcludeCatalogResources.Text = "Preserve Catalog Resource IIDs";
            this.ckbExcludeCatalogResources.UseVisualStyleBackColor = false;
            // 
            // label23
            // 
            this.label23.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label23.AutoSize = true;
            this.label23.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.label23.Location = new System.Drawing.Point(63, 0);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(81, 13);
            this.label23.TabIndex = 0;
            this.label23.Text = "Advanced only!";
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
            this.ckbRenumber.TabIndex = 1;
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
            this.label22.TabIndex = 0;
            this.label22.Text = "If you intend to remove any parts of the scenegraph\r\nchain in order for the objec" +
                "t to use original resouces,\r\nyou must do so before renumbering internally";
            // 
            // tlpOptions
            // 
            this.tlpOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpOptions.ColumnCount = 7;
            this.tlpOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tlpOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tlpOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpOptions.Controls.Add(this.ckbCompress, 1, 9);
            this.tlpOptions.Controls.Add(this.flowLayoutPanel1, 1, 10);
            this.tlpOptions.Controls.Add(this.ckbPadSTBLs, 1, 5);
            this.tlpOptions.Controls.Add(this.label1, 1, 1);
            this.tlpOptions.Controls.Add(this.groupBox1, 1, 3);
            this.tlpOptions.Controls.Add(this.groupBox2, 1, 7);
            this.tlpOptions.Location = new System.Drawing.Point(0, 0);
            this.tlpOptions.Name = "tlpOptions";
            this.tlpOptions.RowCount = 12;
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpOptions.Size = new System.Drawing.Size(348, 515);
            this.tlpOptions.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.tlpOptions.SetColumnSpan(this.flowLayoutPanel1, 5);
            this.flowLayoutPanel1.Controls.Add(this.btnCancel);
            this.flowLayoutPanel1.Controls.Add(this.btnStart);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(150, 452);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(162, 29);
            this.flowLayoutPanel1.TabIndex = 9;
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
            this.tlpCustomise.Size = new System.Drawing.Size(265, 155);
            this.tlpCustomise.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.tlpCustomise.SetColumnSpan(this.label2, 2);
            this.label2.Location = new System.Drawing.Point(3, 116);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(258, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Unique name (used as internal name and hash seed):";
            // 
            // ckbPadSTBLs
            // 
            this.ckbPadSTBLs.AutoSize = true;
            this.tlpOptions.SetColumnSpan(this.ckbPadSTBLs, 5);
            this.ckbPadSTBLs.Location = new System.Drawing.Point(35, 183);
            this.ckbPadSTBLs.Name = "ckbPadSTBLs";
            this.ckbPadSTBLs.Size = new System.Drawing.Size(153, 17);
            this.ckbPadSTBLs.TabIndex = 6;
            this.ckbPadSTBLs.Text = "Create missing string tables";
            this.ckbPadSTBLs.UseVisualStyleBackColor = true;
            // 
            // ckbClone
            // 
            this.ckbClone.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.ckbClone, 2);
            this.ckbClone.Location = new System.Drawing.Point(3, 3);
            this.ckbClone.Name = "ckbClone";
            this.ckbClone.Size = new System.Drawing.Size(131, 17);
            this.ckbClone.TabIndex = 2;
            this.ckbClone.Text = "Create clone package";
            this.ckbClone.UseVisualStyleBackColor = true;
            this.ckbClone.CheckedChanged += new System.EventHandler(this.ckbClone_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.tlpOptions.SetColumnSpan(this.label1, 5);
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(262, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Options";
            // 
            // ckbDefault
            // 
            this.ckbDefault.AutoSize = true;
            this.ckbDefault.Location = new System.Drawing.Point(27, 26);
            this.ckbDefault.Name = "ckbDefault";
            this.ckbDefault.Size = new System.Drawing.Size(131, 17);
            this.ckbDefault.TabIndex = 4;
            this.ckbDefault.Text = "Default resources only";
            this.ckbDefault.UseVisualStyleBackColor = true;
            // 
            // ckbThumbs
            // 
            this.ckbThumbs.AutoSize = true;
            this.ckbThumbs.Location = new System.Drawing.Point(27, 49);
            this.ckbThumbs.Name = "ckbThumbs";
            this.ckbThumbs.Size = new System.Drawing.Size(114, 17);
            this.ckbThumbs.TabIndex = 5;
            this.ckbThumbs.Text = "Include thumbnails";
            this.ckbThumbs.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpOptions.SetColumnSpan(this.groupBox1, 5);
            this.groupBox1.Controls.Add(this.tableLayoutPanel1);
            this.groupBox1.Location = new System.Drawing.Point(35, 58);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(173, 107);
            this.groupBox1.TabIndex = 13;
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
            this.tableLayoutPanel1.Controls.Add(this.ckbDefault, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.ckbThumbs, 1, 3);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 19);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(161, 69);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.AutoSize = true;
            this.groupBox2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpOptions.SetColumnSpan(this.groupBox2, 5);
            this.groupBox2.Controls.Add(this.tlpCustomise);
            this.groupBox2.Location = new System.Drawing.Point(35, 218);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(277, 193);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Make object unique";
            // 
            // CloneFixOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlpOptions);
            this.Name = "CloneFixOptions";
            this.Size = new System.Drawing.Size(348, 515);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.tlpOptions.ResumeLayout(false);
            this.tlpOptions.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.tlpCustomise.ResumeLayout(false);
            this.tlpCustomise.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox ckbCompress;
        private System.Windows.Forms.TextBox tbUniqueName;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.CheckBox ckbRenumber;
        private System.Windows.Forms.CheckBox ckbExcludeCatalogResources;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TableLayoutPanel tlpOptions;
        private System.Windows.Forms.CheckBox ckbClone;
        private System.Windows.Forms.CheckBox ckbPadSTBLs;
        private System.Windows.Forms.CheckBox ckbThumbs;
        private System.Windows.Forms.CheckBox ckbDefault;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.TableLayoutPanel tlpCustomise;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}