﻿namespace S3PIDemoFE
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.browserWidget1 = new S3PIDemoFE.BrowserWidget();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.packageInfoWidget1 = new S3PIDemoFE.PackageInfo.PackageInfoWidget();
            this.hexWidget1 = new S3PIDemoFE.HexWidget();
            this.panel1 = new System.Windows.Forms.Panel();
            this.controlPanel1 = new S3PIDemoFE.ControlPanel();
            this.resourceFilterWidget1 = new S3PIDemoFE.Filter.ResourceFilterWidget();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveAsFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.exportFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.menuBarWidget1 = new S3PIDemoFE.MenuBarWidget();
            this.packageInfoFields1 = new S3PIDemoFE.PackageInfo.PackageInfoFields();
            this.resourceFields1 = new S3PIDemoFE.Filter.ResourceFields();
            this.exportBatchTarget = new System.Windows.Forms.FolderBrowserDialog();
            this.fileImportDialog = new System.Windows.Forms.OpenFileDialog();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 23);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.hexWidget1);
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Size = new System.Drawing.Size(880, 654);
            this.splitContainer1.SplitterDistance = 273;
            this.splitContainer1.TabIndex = 1;
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.browserWidget1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.packageInfoWidget1);
            this.splitContainer2.Size = new System.Drawing.Size(880, 273);
            this.splitContainer2.SplitterDistance = 607;
            this.splitContainer2.TabIndex = 0;
            // 
            // browserWidget1
            // 
            this.browserWidget1.AllowDrop = true;
            this.browserWidget1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.browserWidget1.Fields = null;
            this.browserWidget1.Filter = null;
            this.browserWidget1.Location = new System.Drawing.Point(0, 0);
            this.browserWidget1.Name = "browserWidget1";
            this.browserWidget1.Package = null;
            this.browserWidget1.ProgressBar = this.progressBar1;
            this.browserWidget1.SelectedResource = null;
            this.browserWidget1.Size = new System.Drawing.Size(603, 269);
            this.browserWidget1.Sortable = false;
            this.browserWidget1.TabIndex = 0;
            this.browserWidget1.DragOver += new System.Windows.Forms.DragEventHandler(this.browserWidget1_DragOver);
            this.browserWidget1.SelectedResourceDeleted += new System.EventHandler(this.browserWidget1_SelectedResourceDeleted);
            this.browserWidget1.DragDrop += new System.Windows.Forms.DragEventHandler(this.browserWidget1_DragDrop);
            this.browserWidget1.ItemActivate += new System.EventHandler(this.browserWidget1_ItemActivate);
            this.browserWidget1.SelectedResourceChanging += new System.EventHandler<S3PIDemoFE.BrowserWidget.ResourceChangingEventArgs>(this.browserWidget1_SelectedResourceChanging);
            this.browserWidget1.SelectedResourceChanged += new System.EventHandler<S3PIDemoFE.BrowserWidget.ResourceChangedEventArgs>(this.browserWidget1_SelectedResourceChanged);
            // 
            // progressBar1
            // 
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar1.Location = new System.Drawing.Point(0, 652);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(880, 25);
            this.progressBar1.TabIndex = 2;
            // 
            // packageInfoWidget1
            // 
            this.packageInfoWidget1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.packageInfoWidget1.Fields = null;
            this.packageInfoWidget1.Location = new System.Drawing.Point(0, 0);
            this.packageInfoWidget1.Name = "packageInfoWidget1";
            this.packageInfoWidget1.Package = null;
            this.packageInfoWidget1.Size = new System.Drawing.Size(265, 269);
            this.packageInfoWidget1.TabIndex = 0;
            // 
            // hexWidget1
            // 
            this.hexWidget1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.hexWidget1.Location = new System.Drawing.Point(0, 103);
            this.hexWidget1.Margin = new System.Windows.Forms.Padding(0);
            this.hexWidget1.Name = "hexWidget1";
            this.hexWidget1.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.hexWidget1.Rowsize = 32;
            this.hexWidget1.Size = new System.Drawing.Size(876, 242);
            this.hexWidget1.Stream = null;
            this.hexWidget1.TabIndex = 1;
            this.hexWidget1.HexChanged += new System.EventHandler(this.resource_ResourceChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.controlPanel1);
            this.panel1.Controls.Add(this.resourceFilterWidget1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(876, 103);
            this.panel1.TabIndex = 0;
            // 
            // controlPanel1
            // 
            this.controlPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.controlPanel1.Location = new System.Drawing.Point(0, 74);
            this.controlPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.controlPanel1.Name = "controlPanel1";
            this.controlPanel1.Padding = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.controlPanel1.Size = new System.Drawing.Size(876, 29);
            this.controlPanel1.TabIndex = 1;
            this.controlPanel1.CommitClick += new System.EventHandler(this.controlPanel1_CommitClick);
            this.controlPanel1.ViewerClick += new System.EventHandler(this.controlPanel1_ViewerClick);
            this.controlPanel1.HexClick += new System.EventHandler(this.controlPanel1_HexClick);
            this.controlPanel1.UseNamesChanged += new System.EventHandler(this.controlPanel1_UseNamesChanged);
            this.controlPanel1.GridClick += new System.EventHandler(this.controlPanel1_GridClick);
            this.controlPanel1.SortChanged += new System.EventHandler(this.controlPanel1_SortChanged);
            this.controlPanel1.ValueClick += new System.EventHandler(this.controlPanel1_ValueClick);
            this.controlPanel1.HexOnlyChanged += new System.EventHandler(this.controlPanel1_HexOnlyChanged);
            this.controlPanel1.EditorClick += new System.EventHandler(this.controlPanel1_EditorClick);
            // 
            // resourceFilterWidget1
            // 
            this.resourceFilterWidget1.BrowserWidget = this.browserWidget1;
            this.resourceFilterWidget1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resourceFilterWidget1.Fields = null;
            this.resourceFilterWidget1.Filter = ((System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<string, s3pi.Interfaces.TypedValue>>)(resources.GetObject("resourceFilterWidget1.Filter")));
            this.resourceFilterWidget1.Location = new System.Drawing.Point(0, 0);
            this.resourceFilterWidget1.Name = "resourceFilterWidget1";
            this.resourceFilterWidget1.Size = new System.Drawing.Size(876, 103);
            this.resourceFilterWidget1.TabIndex = 0;
            this.resourceFilterWidget1.FilterChanged += new System.EventHandler(this.resourceFilterWidget1_FilterChanged);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "*.package";
            this.openFileDialog1.Filter = "DBPF Packages|*.package;*.world|All Files|*.*";
            this.openFileDialog1.SupportMultiDottedExtensions = true;
            this.openFileDialog1.Title = "Open package";
            // 
            // saveAsFileDialog
            // 
            this.saveAsFileDialog.FileName = "*.package";
            this.saveAsFileDialog.Filter = "DBPF Packages|*.package;*.world|All Files|*.*";
            this.saveAsFileDialog.Title = "Save As";
            // 
            // exportFileDialog
            // 
            this.exportFileDialog.AddExtension = false;
            this.exportFileDialog.Filter = "Exported files (S3_*.*)|S3_*.*|All files (*.*)|*.*";
            this.exportFileDialog.Title = "Export File";
            // 
            // menuBarWidget1
            // 
            this.menuBarWidget1.Dock = System.Windows.Forms.DockStyle.Top;
            this.menuBarWidget1.Location = new System.Drawing.Point(0, 0);
            this.menuBarWidget1.Margin = new System.Windows.Forms.Padding(0);
            this.menuBarWidget1.Name = "menuBarWidget1";
            this.menuBarWidget1.Size = new System.Drawing.Size(880, 23);
            this.menuBarWidget1.TabIndex = 0;
            this.menuBarWidget1.MBHelp_Click += new S3PIDemoFE.MenuBarWidget.MBClickEventHandler(this.menuBarWidget1_MBHelp_Click);
            this.menuBarWidget1.MBResource_Click += new S3PIDemoFE.MenuBarWidget.MBClickEventHandler(this.menuBarWidget1_MBResource_Click);
            this.menuBarWidget1.MBEdit_Click += new S3PIDemoFE.MenuBarWidget.MBClickEventHandler(this.menuBarWidget1_MBEdit_Click);
            this.menuBarWidget1.MBFile_Click += new S3PIDemoFE.MenuBarWidget.MBClickEventHandler(this.menuBarWidget1_MBFile_Click);
            this.menuBarWidget1.MBDropDownOpening += new S3PIDemoFE.MenuBarWidget.MBDropDownOpeningEventHandler(this.menuBarWidget1_MBDropDownOpening);
            this.menuBarWidget1.MBSettings_Click += new S3PIDemoFE.MenuBarWidget.MBClickEventHandler(this.menuBarWidget1_MBSettings_Click);
            this.menuBarWidget1.MRUClick += new S3PIDemoFE.MenuBarWidget.MRUClickEventHandler(this.menuBarWidget1_MRUClick);
            // 
            // exportBatchTarget
            // 
            this.exportBatchTarget.Description = "Choose the folder to receive the exported resources";
            // 
            // fileImportDialog
            // 
            this.fileImportDialog.Filter = "Exported files (S3_*.*)|S3_*.*|All files (*.*)|*.*";
            this.fileImportDialog.Multiselect = true;
            this.fileImportDialog.Title = "Import Resource";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(880, 677);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuBarWidget1);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "S3PIDemoFE";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveAsFileDialog;
        private System.Windows.Forms.SaveFileDialog exportFileDialog;
        private MenuBarWidget menuBarWidget1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private BrowserWidget browserWidget1;
        private PackageInfo.PackageInfoWidget packageInfoWidget1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private PackageInfo.PackageInfoFields packageInfoFields1;
        private Filter.ResourceFields resourceFields1;
        private HexWidget hexWidget1;
        private System.Windows.Forms.Panel panel1;
        private Filter.ResourceFilterWidget resourceFilterWidget1;
        private ControlPanel controlPanel1;
        private System.Windows.Forms.FolderBrowserDialog exportBatchTarget;
        private System.Windows.Forms.OpenFileDialog fileImportDialog;
    }
}