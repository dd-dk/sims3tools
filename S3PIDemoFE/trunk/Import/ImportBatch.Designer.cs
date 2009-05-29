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
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(314, 225);
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
            this.btnCancel.Location = new System.Drawing.Point(233, 225);
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
            this.lbFiles.Location = new System.Drawing.Point(12, 35);
            this.lbFiles.Name = "lbFiles";
            this.lbFiles.Size = new System.Drawing.Size(377, 184);
            this.lbFiles.TabIndex = 3;
            this.lbFiles.DragOver += new System.Windows.Forms.DragEventHandler(this.ImportBatch_DragOver);
            this.lbFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.ImportBatch_DragDrop);
            // 
            // ckbUseName
            // 
            this.ckbUseName.AutoSize = true;
            this.ckbUseName.Location = new System.Drawing.Point(12, 12);
            this.ckbUseName.Name = "ckbUseName";
            this.ckbUseName.Size = new System.Drawing.Size(118, 17);
            this.ckbUseName.TabIndex = 4;
            this.ckbUseName.Text = "Use resource name";
            this.ckbUseName.UseVisualStyleBackColor = true;
            // 
            // ckbRename
            // 
            this.ckbRename.AutoSize = true;
            this.ckbRename.Location = new System.Drawing.Point(136, 12);
            this.ckbRename.Name = "ckbRename";
            this.ckbRename.Size = new System.Drawing.Size(112, 17);
            this.ckbRename.TabIndex = 5;
            this.ckbRename.Text = "Rename if present";
            this.ckbRename.UseVisualStyleBackColor = true;
            // 
            // ImportBatch
            // 
            this.AcceptButton = this.btnOK;
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(401, 260);
            this.Controls.Add(this.ckbUseName);
            this.Controls.Add(this.ckbRename);
            this.Controls.Add(this.lbFiles);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "ImportBatch";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Import Files";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.ImportBatch_DragDrop);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.ImportBatch_DragOver);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ListBox lbFiles;
        private System.Windows.Forms.CheckBox ckbUseName;
        private System.Windows.Forms.CheckBox ckbRename;
    }
}