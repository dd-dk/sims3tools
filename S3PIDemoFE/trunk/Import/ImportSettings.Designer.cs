namespace S3PIDemoFE.Import
{
    partial class ImportSettings
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
            this.ckbOverwrite = new System.Windows.Forms.CheckBox();
            this.ckbCompress = new System.Windows.Forms.CheckBox();
            this.ckbUseName = new System.Windows.Forms.CheckBox();
            this.ckbRename = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // ckbOverwrite
            // 
            this.ckbOverwrite.AutoSize = true;
            this.ckbOverwrite.Location = new System.Drawing.Point(3, 3);
            this.ckbOverwrite.Name = "ckbOverwrite";
            this.ckbOverwrite.Size = new System.Drawing.Size(122, 17);
            this.ckbOverwrite.TabIndex = 1;
            this.ckbOverwrite.Text = "Overwrite duplicates";
            this.ckbOverwrite.UseVisualStyleBackColor = true;
            this.ckbOverwrite.CheckedChanged += new System.EventHandler(this.ckbOverwrite_CheckedChanged);
            // 
            // ckbCompress
            // 
            this.ckbCompress.AutoSize = true;
            this.SetFlowBreak(this.ckbCompress, true);
            this.ckbCompress.Location = new System.Drawing.Point(131, 3);
            this.ckbCompress.Name = "ckbCompress";
            this.ckbCompress.Size = new System.Drawing.Size(72, 17);
            this.ckbCompress.TabIndex = 2;
            this.ckbCompress.Text = "Compress";
            this.ckbCompress.UseVisualStyleBackColor = true;
            this.ckbCompress.CheckedChanged += new System.EventHandler(this.ckbCompress_CheckedChanged);
            // 
            // ckbUseName
            // 
            this.ckbUseName.AutoSize = true;
            this.ckbUseName.Location = new System.Drawing.Point(3, 26);
            this.ckbUseName.Name = "ckbUseName";
            this.ckbUseName.Size = new System.Drawing.Size(118, 17);
            this.ckbUseName.TabIndex = 3;
            this.ckbUseName.Text = "Use resource name";
            this.ckbUseName.UseVisualStyleBackColor = true;
            this.ckbUseName.CheckedChanged += new System.EventHandler(this.ckbUseName_CheckedChanged);
            // 
            // ckbRename
            // 
            this.ckbRename.AutoSize = true;
            this.ckbRename.Location = new System.Drawing.Point(127, 26);
            this.ckbRename.Name = "ckbRename";
            this.ckbRename.Size = new System.Drawing.Size(112, 17);
            this.ckbRename.TabIndex = 4;
            this.ckbRename.Text = "Rename if present";
            this.ckbRename.UseVisualStyleBackColor = true;
            this.ckbRename.CheckedChanged += new System.EventHandler(this.ckbRename_CheckedChanged);
            // 
            // ImportSettings
            // 
            this.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.ckbOverwrite);
            this.Controls.Add(this.ckbCompress);
            this.Controls.Add(this.ckbUseName);
            this.Controls.Add(this.ckbRename);
            this.Size = new System.Drawing.Size(242, 46);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox ckbOverwrite;
        private System.Windows.Forms.CheckBox ckbCompress;
        private System.Windows.Forms.CheckBox ckbUseName;
        private System.Windows.Forms.CheckBox ckbRename;
    }
}
