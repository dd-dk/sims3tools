namespace ObjectCloner.TopPanelComponents
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
            this.ckbFix = new System.Windows.Forms.CheckBox();
            this.label22 = new System.Windows.Forms.Label();
            this.tlpOptions = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tlpCustomise = new System.Windows.Forms.TableLayoutPanel();
            this.ckbPadSTBLs = new System.Windows.Forms.CheckBox();
            this.ckbThumbs = new System.Windows.Forms.CheckBox();
            this.ckbDefault = new System.Windows.Forms.CheckBox();
            this.ckbClone = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel5.SuspendLayout();
            this.tlpOptions.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tlpCustomise.SuspendLayout();
            this.SuspendLayout();
            // 
            // ckbCompress
            // 
            this.ckbCompress.AutoSize = true;
            this.ckbCompress.Checked = true;
            this.ckbCompress.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tlpOptions.SetColumnSpan(this.ckbCompress, 5);
            this.ckbCompress.Location = new System.Drawing.Point(60, 359);
            this.ckbCompress.Name = "ckbCompress";
            this.ckbCompress.Size = new System.Drawing.Size(121, 17);
            this.ckbCompress.TabIndex = 8;
            this.ckbCompress.Text = "Enable compression";
            this.ckbCompress.UseVisualStyleBackColor = false;
            // 
            // tbUniqueName
            // 
            this.tbUniqueName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpOptions.SetColumnSpan(this.tbUniqueName, 5);
            this.tbUniqueName.Location = new System.Drawing.Point(60, 88);
            this.tbUniqueName.Name = "tbUniqueName";
            this.tbUniqueName.Size = new System.Drawing.Size(327, 20);
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
            this.tableLayoutPanel5.Location = new System.Drawing.Point(48, 77);
            this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(24, 3, 3, 3);
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
            this.label23.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label23.AutoSize = true;
            this.label23.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.label23.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.Location = new System.Drawing.Point(27, 0);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(95, 13);
            this.label23.TabIndex = 0;
            this.label23.Text = "Advanced only!";
            // 
            // ckbFix
            // 
            this.ckbFix.AutoSize = true;
            this.ckbFix.Checked = true;
            this.ckbFix.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbFix.Location = new System.Drawing.Point(27, 54);
            this.ckbFix.Name = "ckbFix";
            this.ckbFix.Size = new System.Drawing.Size(194, 17);
            this.ckbFix.TabIndex = 1;
            this.ckbFix.Text = "Renumber resources (make unique)";
            this.ckbFix.UseVisualStyleBackColor = true;
            this.ckbFix.CheckedChanged += new System.EventHandler(this.ckbFix_CheckedChanged);
            // 
            // label22
            // 
            this.label22.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label22.AutoSize = true;
            this.label22.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tlpCustomise.SetColumnSpan(this.label22, 2);
            this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.Location = new System.Drawing.Point(3, 3);
            this.label22.Margin = new System.Windows.Forms.Padding(3);
            this.label22.Name = "label22";
            this.label22.Padding = new System.Windows.Forms.Padding(3);
            this.label22.Size = new System.Drawing.Size(314, 45);
            this.label22.TabIndex = 0;
            this.label22.Text = "If you intend to remove any parts of the scenegraph\r\nchain in order for the objec" +
                "t to use original resouces,\r\nyou must do so before renumbering resources";
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
            this.tlpOptions.Controls.Add(this.ckbCompress, 1, 14);
            this.tlpOptions.Controls.Add(this.flowLayoutPanel1, 1, 15);
            this.tlpOptions.Controls.Add(this.panel1, 1, 13);
            this.tlpOptions.Controls.Add(this.ckbPadSTBLs, 1, 11);
            this.tlpOptions.Controls.Add(this.ckbClone, 1, 6);
            this.tlpOptions.Controls.Add(this.label1, 1, 1);
            this.tlpOptions.Controls.Add(this.tbUniqueName, 1, 4);
            this.tlpOptions.Controls.Add(this.label2, 1, 3);
            this.tlpOptions.Controls.Add(this.ckbDefault, 2, 8);
            this.tlpOptions.Controls.Add(this.ckbThumbs, 2, 9);
            this.tlpOptions.Location = new System.Drawing.Point(0, 0);
            this.tlpOptions.Name = "tlpOptions";
            this.tlpOptions.RowCount = 17;
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 6F));
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 6F));
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 6F));
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 6F));
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpOptions.Size = new System.Drawing.Size(448, 454);
            this.tlpOptions.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.tlpOptions.SetColumnSpan(this.flowLayoutPanel1, 5);
            this.flowLayoutPanel1.Controls.Add(this.btnCancel);
            this.flowLayoutPanel1.Controls.Add(this.btnStart);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(225, 382);
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
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tlpOptions.SetColumnSpan(this.panel1, 5);
            this.panel1.Controls.Add(this.tlpCustomise);
            this.panel1.Location = new System.Drawing.Point(60, 230);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(327, 123);
            this.panel1.TabIndex = 12;
            // 
            // tlpCustomise
            // 
            this.tlpCustomise.AutoSize = true;
            this.tlpCustomise.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpCustomise.ColumnCount = 2;
            this.tlpCustomise.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tlpCustomise.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpCustomise.Controls.Add(this.tableLayoutPanel5, 1, 2);
            this.tlpCustomise.Controls.Add(this.ckbFix, 1, 1);
            this.tlpCustomise.Controls.Add(this.label22, 0, 0);
            this.tlpCustomise.Location = new System.Drawing.Point(0, 0);
            this.tlpCustomise.Name = "tlpCustomise";
            this.tlpCustomise.RowCount = 3;
            this.tlpCustomise.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpCustomise.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpCustomise.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpCustomise.Size = new System.Drawing.Size(320, 116);
            this.tlpCustomise.TabIndex = 7;
            // 
            // ckbPadSTBLs
            // 
            this.ckbPadSTBLs.AutoSize = true;
            this.tlpOptions.SetColumnSpan(this.ckbPadSTBLs, 5);
            this.ckbPadSTBLs.Location = new System.Drawing.Point(60, 201);
            this.ckbPadSTBLs.Name = "ckbPadSTBLs";
            this.ckbPadSTBLs.Size = new System.Drawing.Size(153, 17);
            this.ckbPadSTBLs.TabIndex = 6;
            this.ckbPadSTBLs.Text = "Create missing string tables";
            this.ckbPadSTBLs.UseVisualStyleBackColor = true;
            // 
            // ckbThumbs
            // 
            this.ckbThumbs.AutoSize = true;
            this.tlpOptions.SetColumnSpan(this.ckbThumbs, 4);
            this.ckbThumbs.Location = new System.Drawing.Point(84, 172);
            this.ckbThumbs.Name = "ckbThumbs";
            this.ckbThumbs.Size = new System.Drawing.Size(114, 17);
            this.ckbThumbs.TabIndex = 5;
            this.ckbThumbs.Text = "Include thumbnails";
            this.ckbThumbs.UseVisualStyleBackColor = true;
            // 
            // ckbDefault
            // 
            this.ckbDefault.AutoSize = true;
            this.tlpOptions.SetColumnSpan(this.ckbDefault, 4);
            this.ckbDefault.Location = new System.Drawing.Point(84, 149);
            this.ckbDefault.Name = "ckbDefault";
            this.ckbDefault.Size = new System.Drawing.Size(131, 17);
            this.ckbDefault.TabIndex = 4;
            this.ckbDefault.Text = "Default resources only";
            this.ckbDefault.UseVisualStyleBackColor = true;
            // 
            // ckbClone
            // 
            this.ckbClone.AutoSize = true;
            this.tlpOptions.SetColumnSpan(this.ckbClone, 2);
            this.ckbClone.Location = new System.Drawing.Point(60, 120);
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
            this.label1.Location = new System.Drawing.Point(337, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Options";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.tlpOptions.SetColumnSpan(this.label2, 5);
            this.label2.Location = new System.Drawing.Point(60, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Unique Name:";
            // 
            // CloneFixOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlpOptions);
            this.Name = "CloneFixOptions";
            this.Size = new System.Drawing.Size(448, 454);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.tlpOptions.ResumeLayout(false);
            this.tlpOptions.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tlpCustomise.ResumeLayout(false);
            this.tlpCustomise.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox ckbCompress;
        private System.Windows.Forms.TextBox tbUniqueName;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.CheckBox ckbFix;
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
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tlpCustomise;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}
