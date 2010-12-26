namespace S3SA_DLL_ExpImp
{
    partial class Export
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
            this.sfdExport = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // sfdExport
            // 
            this.sfdExport.DefaultExt = "dll";
            this.sfdExport.FileName = "*.dll";
            this.sfdExport.Filter = ".Net Assemblies|*.dll|All files|*.*";
            this.sfdExport.Title = "Export .Net Assembly";
            // 
            // Export
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Name = "Export";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Export_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SaveFileDialog sfdExport;
    }
}

