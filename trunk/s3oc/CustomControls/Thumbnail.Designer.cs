namespace ObjectCloner.CustomControls
{
    partial class Thumbnail
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
            this.lbThumbnailRK = new System.Windows.Forms.Label();
            this.btnImportThumb = new System.Windows.Forms.Button();
            this.pbThumbnail = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnExportThumb = new System.Windows.Forms.Button();
            this.openThumbnailDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveThumbnailDialog = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.pbThumbnail)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbThumbnailRK
            // 
            this.lbThumbnailRK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lbThumbnailRK.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.lbThumbnailRK, 3);
            this.lbThumbnailRK.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbThumbnailRK.Location = new System.Drawing.Point(131, 72);
            this.lbThumbnailRK.Name = "lbThumbnailRK";
            this.lbThumbnailRK.Size = new System.Drawing.Size(247, 13);
            this.lbThumbnailRK.TabIndex = 3;
            this.lbThumbnailRK.Text = "0x00000000-0x00000000-0x0000000000000000";
            // 
            // btnImportThumb
            // 
            this.btnImportThumb.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnImportThumb.Enabled = false;
            this.btnImportThumb.Location = new System.Drawing.Point(212, 46);
            this.btnImportThumb.Name = "btnImportThumb";
            this.btnImportThumb.Size = new System.Drawing.Size(75, 23);
            this.btnImportThumb.TabIndex = 2;
            this.btnImportThumb.Text = "Replace...";
            this.btnImportThumb.UseVisualStyleBackColor = true;
            this.btnImportThumb.Click += new System.EventHandler(this.btnImportThumb_Click);
            // 
            // pbThumbnail
            // 
            this.pbThumbnail.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pbThumbnail.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pbThumbnail.ContextMenuStrip = this.contextMenuStrip1;
            this.pbThumbnail.Location = new System.Drawing.Point(0, 0);
            this.pbThumbnail.Margin = new System.Windows.Forms.Padding(0);
            this.pbThumbnail.Name = "pbThumbnail";
            this.tableLayoutPanel1.SetRowSpan(this.pbThumbnail, 4);
            this.pbThumbnail.Size = new System.Drawing.Size(128, 128);
            this.pbThumbnail.TabIndex = 0;
            this.pbThumbnail.TabStop = false;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.replaceToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(125, 48);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.saveToolStripMenuItem.Text = "E&xport...";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // replaceToolStripMenuItem
            // 
            this.replaceToolStripMenuItem.Name = "replaceToolStripMenuItem";
            this.replaceToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.replaceToolStripMenuItem.Text = "Rep&lace...";
            this.replaceToolStripMenuItem.Click += new System.EventHandler(this.replaceToolStripMenuItem_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.btnImportThumb, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.lbThumbnailRK, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.pbThumbnail, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnExportThumb, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(381, 128);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // btnExportThumb
            // 
            this.btnExportThumb.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnExportThumb.Enabled = false;
            this.btnExportThumb.Location = new System.Drawing.Point(131, 46);
            this.btnExportThumb.Name = "btnExportThumb";
            this.btnExportThumb.Size = new System.Drawing.Size(75, 23);
            this.btnExportThumb.TabIndex = 1;
            this.btnExportThumb.Text = "Export...";
            this.btnExportThumb.UseVisualStyleBackColor = true;
            this.btnExportThumb.Click += new System.EventHandler(this.btnExportThumb_Click);
            // 
            // openThumbnailDialog
            // 
            this.openThumbnailDialog.FileName = "*.PNG";
            this.openThumbnailDialog.Filter = "Thumbnails|*.PNG|All files|*.*";
            this.openThumbnailDialog.Title = "Select thumbnail";
            // 
            // saveThumbnailDialog
            // 
            this.saveThumbnailDialog.FileName = "*.PNG";
            this.saveThumbnailDialog.Filter = "Thumbnails|*.PNG|All files|*.*";
            this.saveThumbnailDialog.Title = "Save thumbnail";
            // 
            // Thumbnail
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Thumbnail";
            this.Size = new System.Drawing.Size(381, 128);
            ((System.ComponentModel.ISupportInitialize)(this.pbThumbnail)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbThumbnailRK;
        private System.Windows.Forms.Button btnImportThumb;
        private System.Windows.Forms.PictureBox pbThumbnail;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnExportThumb;
        private System.Windows.Forms.OpenFileDialog openThumbnailDialog;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem replaceToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveThumbnailDialog;
    }
}
