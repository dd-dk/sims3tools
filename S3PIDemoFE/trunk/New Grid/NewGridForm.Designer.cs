namespace S3PIDemoFE
{
    partial class NewGridForm
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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.btnOK = new System.Windows.Forms.Button();
            this.s3PIPropertyGrid1 = new S3PIDemoFE.S3PIPropertyGrid();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Title = "Import...";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Title = "Export...";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(219, 303);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "Close";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // s3PIPropertyGrid1
            // 
            this.s3PIPropertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.s3PIPropertyGrid1.HelpVisible = false;
            this.s3PIPropertyGrid1.Location = new System.Drawing.Point(12, 12);
            this.s3PIPropertyGrid1.Name = "s3PIPropertyGrid1";
            this.s3PIPropertyGrid1.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.s3PIPropertyGrid1.s3piObject = null;
            this.s3PIPropertyGrid1.Size = new System.Drawing.Size(282, 284);
            this.s3PIPropertyGrid1.TabIndex = 1;
            this.s3PIPropertyGrid1.ToolbarVisible = false;
            // 
            // GridForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(306, 338);
            this.Controls.Add(this.s3PIPropertyGrid1);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "GridForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Data Grid";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button btnOK;
        private S3PIPropertyGrid s3PIPropertyGrid1;
    }
}