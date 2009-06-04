namespace S3PIDemoFE
{
    partial class ImportBatch
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
            this.lbFiles = new System.Windows.Forms.ListBox();
            this.ckbUseName = new System.Windows.Forms.CheckBox();
            this.ckbRename = new System.Windows.Forms.CheckBox();
            this.ckbCompress = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.ckbOverwrite = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(193, 181);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "Import";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(112, 181);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lbFiles
            // 
            this.lbFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbFiles.FormattingEnabled = true;
            this.lbFiles.IntegralHeight = false;
            this.lbFiles.Location = new System.Drawing.Point(3, 55);
            this.lbFiles.Name = "lbFiles";
            this.lbFiles.Size = new System.Drawing.Size(250, 105);
            this.lbFiles.TabIndex = 3;
            this.lbFiles.DragOver += new System.Windows.Forms.DragEventHandler(this.ImportBatch_DragOver);
            this.lbFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.ImportBatch_DragDrop);
            // 
            // ckbUseName
            // 
            this.ckbUseName.AutoSize = true;
            this.ckbUseName.Location = new System.Drawing.Point(3, 26);
            this.ckbUseName.Name = "ckbUseName";
            this.ckbUseName.Size = new System.Drawing.Size(118, 17);
            this.ckbUseName.TabIndex = 4;
            this.ckbUseName.Text = "Use resource name";
            this.ckbUseName.UseVisualStyleBackColor = true;
            // 
            // ckbRename
            // 
            this.ckbRename.AutoSize = true;
            this.ckbRename.Location = new System.Drawing.Point(127, 26);
            this.ckbRename.Name = "ckbRename";
            this.ckbRename.Size = new System.Drawing.Size(112, 17);
            this.ckbRename.TabIndex = 5;
            this.ckbRename.Text = "Rename if present";
            this.ckbRename.UseVisualStyleBackColor = true;
            // 
            // ckbCompress
            // 
            this.ckbCompress.AutoSize = true;
            this.flowLayoutPanel1.SetFlowBreak(this.ckbCompress, true);
            this.ckbCompress.Location = new System.Drawing.Point(131, 3);
            this.ckbCompress.Name = "ckbCompress";
            this.ckbCompress.Size = new System.Drawing.Size(72, 17);
            this.ckbCompress.TabIndex = 6;
            this.ckbCompress.Text = "Compress";
            this.ckbCompress.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.lbFiles, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(256, 163);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.ckbOverwrite);
            this.flowLayoutPanel1.Controls.Add(this.ckbCompress);
            this.flowLayoutPanel1.Controls.Add(this.ckbUseName);
            this.flowLayoutPanel1.Controls.Add(this.ckbRename);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(242, 46);
            this.flowLayoutPanel1.TabIndex = 4;
            // 
            // ckbOverwrite
            // 
            this.ckbOverwrite.AutoSize = true;
            this.ckbOverwrite.Location = new System.Drawing.Point(3, 3);
            this.ckbOverwrite.Name = "ckbOverwrite";
            this.ckbOverwrite.Size = new System.Drawing.Size(122, 17);
            this.ckbOverwrite.TabIndex = 7;
            this.ckbOverwrite.Text = "Overwrite duplicates";
            this.ckbOverwrite.UseVisualStyleBackColor = true;
            // 
            // ImportBatch
            // 
            this.AcceptButton = this.btnOK;
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(280, 216);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "ImportBatch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Import Files";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.ImportBatch_DragDrop);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.ImportBatch_DragOver);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ListBox lbFiles;
        private System.Windows.Forms.CheckBox ckbUseName;
        private System.Windows.Forms.CheckBox ckbRename;
        private System.Windows.Forms.CheckBox ckbCompress;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.CheckBox ckbOverwrite;
    }
}