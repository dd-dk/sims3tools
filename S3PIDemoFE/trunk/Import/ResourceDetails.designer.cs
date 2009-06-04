namespace S3PIDemoFE
{
    partial class ResourceDetails
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbType = new System.Windows.Forms.TextBox();
            this.tbGroup = new System.Windows.Forms.TextBox();
            this.tbInstance = new System.Windows.Forms.TextBox();
            this.lbFilename = new System.Windows.Forms.Label();
            this.tbFilename = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbName = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.ckbUseName = new System.Windows.Forms.CheckBox();
            this.ckbRename = new System.Windows.Forms.CheckBox();
            this.ckbCompress = new System.Windows.Forms.CheckBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.ckbOverwrite = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point(240, 176);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOKCancel_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(159, 176);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnOKCancel_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tbType, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.tbGroup, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.tbInstance, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.lbFilename, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.tbFilename, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.tbName, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 1, 3);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(303, 158);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Instance";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Group";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Type";
            // 
            // tbType
            // 
            this.tbType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbType.Location = new System.Drawing.Point(58, 3);
            this.tbType.Name = "tbType";
            this.tbType.Size = new System.Drawing.Size(242, 20);
            this.tbType.TabIndex = 2;
            this.tbType.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbType.TextChanged += new System.EventHandler(this.tbTGI_TextChanged);
            // 
            // tbGroup
            // 
            this.tbGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbGroup.Location = new System.Drawing.Point(58, 29);
            this.tbGroup.Name = "tbGroup";
            this.tbGroup.Size = new System.Drawing.Size(242, 20);
            this.tbGroup.TabIndex = 4;
            this.tbGroup.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbGroup.TextChanged += new System.EventHandler(this.tbTGI_TextChanged);
            // 
            // tbInstance
            // 
            this.tbInstance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbInstance.Location = new System.Drawing.Point(58, 55);
            this.tbInstance.Name = "tbInstance";
            this.tbInstance.Size = new System.Drawing.Size(242, 20);
            this.tbInstance.TabIndex = 6;
            this.tbInstance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbInstance.TextChanged += new System.EventHandler(this.tbTGI_TextChanged);
            // 
            // lbFilename
            // 
            this.lbFilename.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbFilename.AutoSize = true;
            this.lbFilename.Location = new System.Drawing.Point(3, 159);
            this.lbFilename.Name = "lbFilename";
            this.lbFilename.Size = new System.Drawing.Size(49, 13);
            this.lbFilename.TabIndex = 10;
            this.lbFilename.Text = "Filename";
            // 
            // tbFilename
            // 
            this.tbFilename.AllowDrop = true;
            this.tbFilename.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFilename.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbFilename.Location = new System.Drawing.Point(58, 159);
            this.tbFilename.Name = "tbFilename";
            this.tbFilename.ReadOnly = true;
            this.tbFilename.Size = new System.Drawing.Size(242, 13);
            this.tbFilename.TabIndex = 11;
            this.tbFilename.DoubleClick += new System.EventHandler(this.tbFilename_DoubleClick);
            this.tbFilename.TextChanged += new System.EventHandler(this.tbFilename_TextChanged);
            this.tbFilename.DragDrop += new System.Windows.Forms.DragEventHandler(this.tbFilename_DragDrop);
            this.tbFilename.DragOver += new System.Windows.Forms.DragEventHandler(this.tbFilename_DragOver);
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 136);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Name";
            // 
            // tbName
            // 
            this.tbName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbName.Enabled = false;
            this.tbName.Location = new System.Drawing.Point(58, 133);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(242, 20);
            this.tbName.TabIndex = 9;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.ckbOverwrite);
            this.flowLayoutPanel1.Controls.Add(this.ckbCompress);
            this.flowLayoutPanel1.Controls.Add(this.ckbUseName);
            this.flowLayoutPanel1.Controls.Add(this.ckbRename);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(58, 81);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(242, 46);
            this.flowLayoutPanel1.TabIndex = 7;
            // 
            // ckbUseName
            // 
            this.ckbUseName.AutoSize = true;
            this.ckbUseName.Location = new System.Drawing.Point(3, 26);
            this.ckbUseName.Name = "ckbUseName";
            this.ckbUseName.Size = new System.Drawing.Size(118, 17);
            this.ckbUseName.TabIndex = 1;
            this.ckbUseName.Text = "Use resource name";
            this.ckbUseName.UseVisualStyleBackColor = true;
            this.ckbUseName.CheckedChanged += new System.EventHandler(this.ckbUseName_CheckedChanged);
            // 
            // ckbRename
            // 
            this.ckbRename.AutoSize = true;
            this.ckbRename.Enabled = false;
            this.ckbRename.Location = new System.Drawing.Point(127, 26);
            this.ckbRename.Name = "ckbRename";
            this.ckbRename.Size = new System.Drawing.Size(112, 17);
            this.ckbRename.TabIndex = 2;
            this.ckbRename.Text = "Rename if present";
            this.ckbRename.UseVisualStyleBackColor = true;
            // 
            // ckbCompress
            // 
            this.ckbCompress.AutoSize = true;
            this.flowLayoutPanel1.SetFlowBreak(this.ckbCompress, true);
            this.ckbCompress.Location = new System.Drawing.Point(126, 3);
            this.ckbCompress.Name = "ckbCompress";
            this.ckbCompress.Size = new System.Drawing.Size(72, 17);
            this.ckbCompress.TabIndex = 3;
            this.ckbCompress.Text = "Compress";
            this.ckbCompress.UseVisualStyleBackColor = true;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "Exported files (S3_*.*)|S3_*.*|All files (*.*)|*.*";
            this.openFileDialog1.Title = "Import Resource";
            // 
            // ckbOverwrite
            // 
            this.ckbOverwrite.AutoSize = true;
            this.ckbOverwrite.Location = new System.Drawing.Point(3, 3);
            this.ckbOverwrite.Name = "ckbOverwrite";
            this.ckbOverwrite.Size = new System.Drawing.Size(117, 17);
            this.ckbOverwrite.TabIndex = 4;
            this.ckbOverwrite.Text = "Overwrite duplicate";
            this.ckbOverwrite.UseVisualStyleBackColor = true;
            // 
            // ResourceDetails
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(327, 211);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "ResourceDetails";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Resource Details";
            this.Load += new System.EventHandler(this.ImportResource_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbType;
        private System.Windows.Forms.TextBox tbGroup;
        private System.Windows.Forms.TextBox tbInstance;
        private System.Windows.Forms.Label lbFilename;
        private System.Windows.Forms.TextBox tbFilename;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.CheckBox ckbUseName;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.CheckBox ckbRename;
        private System.Windows.Forms.CheckBox ckbCompress;
        private System.Windows.Forms.CheckBox ckbOverwrite;
    }
}