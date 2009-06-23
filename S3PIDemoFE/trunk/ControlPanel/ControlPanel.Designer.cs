namespace S3PIDemoFE
{
    partial class ControlPanel
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
            this.ckbSortable = new System.Windows.Forms.CheckBox();
            this.btnHex = new System.Windows.Forms.Button();
            this.ckbAutoHex = new System.Windows.Forms.CheckBox();
            this.btnValue = new System.Windows.Forms.Button();
            this.ckbNoUnWrap = new System.Windows.Forms.CheckBox();
            this.ckbUseNames = new System.Windows.Forms.CheckBox();
            this.btnCommit = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnGrid = new System.Windows.Forms.Button();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnViewer = new System.Windows.Forms.Button();
            this.btnEditor = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ckbSortable
            // 
            this.ckbSortable.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbSortable.AutoSize = true;
            this.ckbSortable.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ckbSortable.Location = new System.Drawing.Point(0, 5);
            this.ckbSortable.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.ckbSortable.Name = "ckbSortable";
            this.ckbSortable.Size = new System.Drawing.Size(45, 17);
            this.ckbSortable.TabIndex = 1;
            this.ckbSortable.Text = "Sort";
            this.ckbSortable.UseVisualStyleBackColor = true;
            this.ckbSortable.CheckedChanged += new System.EventHandler(this.ckbSortable_CheckedChanged);
            // 
            // btnHex
            // 
            this.btnHex.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnHex.Enabled = false;
            this.btnHex.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnHex.Location = new System.Drawing.Point(57, 2);
            this.btnHex.Margin = new System.Windows.Forms.Padding(0);
            this.btnHex.Name = "btnHex";
            this.btnHex.Size = new System.Drawing.Size(50, 23);
            this.btnHex.TabIndex = 2;
            this.btnHex.Text = "&Hex";
            this.btnHex.UseVisualStyleBackColor = true;
            this.btnHex.Click += new System.EventHandler(this.btnHex_Click);
            // 
            // ckbAutoHex
            // 
            this.ckbAutoHex.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbAutoHex.AutoSize = true;
            this.ckbAutoHex.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ckbAutoHex.Location = new System.Drawing.Point(110, 5);
            this.ckbAutoHex.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.ckbAutoHex.Name = "ckbAutoHex";
            this.ckbAutoHex.Size = new System.Drawing.Size(70, 17);
            this.ckbAutoHex.TabIndex = 3;
            this.ckbAutoHex.Text = "Auto Hex";
            this.ckbAutoHex.UseVisualStyleBackColor = true;
            this.ckbAutoHex.CheckedChanged += new System.EventHandler(this.ckbAutoHex_CheckedChanged);
            // 
            // btnValue
            // 
            this.btnValue.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnValue.Enabled = false;
            this.btnValue.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnValue.Location = new System.Drawing.Point(264, 2);
            this.btnValue.Margin = new System.Windows.Forms.Padding(0);
            this.btnValue.Name = "btnValue";
            this.btnValue.Size = new System.Drawing.Size(50, 23);
            this.btnValue.TabIndex = 5;
            this.btnValue.Text = "Value";
            this.btnValue.UseVisualStyleBackColor = true;
            this.btnValue.Click += new System.EventHandler(this.btnValue_Click);
            // 
            // ckbNoUnWrap
            // 
            this.ckbNoUnWrap.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbNoUnWrap.AutoSize = true;
            this.ckbNoUnWrap.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ckbNoUnWrap.Location = new System.Drawing.Point(183, 5);
            this.ckbNoUnWrap.Margin = new System.Windows.Forms.Padding(3, 0, 12, 0);
            this.ckbNoUnWrap.Name = "ckbNoUnWrap";
            this.ckbNoUnWrap.Size = new System.Drawing.Size(69, 17);
            this.ckbNoUnWrap.TabIndex = 4;
            this.ckbNoUnWrap.Text = "Hex Only";
            this.ckbNoUnWrap.UseVisualStyleBackColor = true;
            this.ckbNoUnWrap.CheckedChanged += new System.EventHandler(this.ckbNoUnWrap_CheckedChanged);
            // 
            // ckbUseNames
            // 
            this.ckbUseNames.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbUseNames.AutoSize = true;
            this.ckbUseNames.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ckbUseNames.Location = new System.Drawing.Point(382, 5);
            this.ckbUseNames.Margin = new System.Windows.Forms.Padding(12, 0, 12, 0);
            this.ckbUseNames.Name = "ckbUseNames";
            this.ckbUseNames.Size = new System.Drawing.Size(138, 17);
            this.ckbUseNames.TabIndex = 7;
            this.ckbUseNames.Text = "Display resource names";
            this.ckbUseNames.UseVisualStyleBackColor = true;
            this.ckbUseNames.CheckedChanged += new System.EventHandler(this.ckbUseNames_CheckedChanged);
            // 
            // btnCommit
            // 
            this.btnCommit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCommit.Enabled = false;
            this.btnCommit.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCommit.Location = new System.Drawing.Point(836, 2);
            this.btnCommit.Margin = new System.Windows.Forms.Padding(0);
            this.btnCommit.Name = "btnCommit";
            this.btnCommit.Size = new System.Drawing.Size(75, 23);
            this.btnCommit.TabIndex = 9;
            this.btnCommit.Text = "&Commit";
            this.btnCommit.UseVisualStyleBackColor = true;
            this.btnCommit.Click += new System.EventHandler(this.btnCommit_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 11;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 6F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.ckbSortable, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnCommit, 10, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnValue, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.ckbUseNames, 7, 0);
            this.tableLayoutPanel1.Controls.Add(this.ckbAutoHex, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnHex, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.ckbNoUnWrap, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnGrid, 6, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 8, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(911, 27);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // btnGrid
            // 
            this.btnGrid.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnGrid.Enabled = false;
            this.btnGrid.Location = new System.Drawing.Point(320, 2);
            this.btnGrid.Margin = new System.Windows.Forms.Padding(0);
            this.btnGrid.Name = "btnGrid";
            this.btnGrid.Size = new System.Drawing.Size(50, 23);
            this.btnGrid.TabIndex = 6;
            this.btnGrid.Text = "Grid";
            this.btnGrid.UseVisualStyleBackColor = true;
            this.btnGrid.Click += new System.EventHandler(this.btnGrid_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 6F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.btnViewer, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnEditor, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(532, 1);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(165, 25);
            this.tableLayoutPanel2.TabIndex = 8;
            // 
            // btnViewer
            // 
            this.btnViewer.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnViewer.Enabled = false;
            this.btnViewer.Location = new System.Drawing.Point(56, 1);
            this.btnViewer.Margin = new System.Windows.Forms.Padding(0);
            this.btnViewer.Name = "btnViewer";
            this.btnViewer.Size = new System.Drawing.Size(50, 23);
            this.btnViewer.TabIndex = 1;
            this.btnViewer.Text = "Viewer";
            this.btnViewer.UseVisualStyleBackColor = true;
            this.btnViewer.Click += new System.EventHandler(this.btnViewer_Click);
            // 
            // btnEditor
            // 
            this.btnEditor.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnEditor.Enabled = false;
            this.btnEditor.Location = new System.Drawing.Point(114, 1);
            this.btnEditor.Margin = new System.Windows.Forms.Padding(0);
            this.btnEditor.Name = "btnEditor";
            this.btnEditor.Size = new System.Drawing.Size(50, 23);
            this.btnEditor.TabIndex = 2;
            this.btnEditor.Text = "Editor";
            this.btnEditor.UseVisualStyleBackColor = true;
            this.btnEditor.Click += new System.EventHandler(this.btnEditor_Click);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "External:";
            // 
            // ControlPanel
            // 
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ControlPanel";
            this.Size = new System.Drawing.Size(911, 27);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

          }

        #endregion

        private System.Windows.Forms.CheckBox ckbSortable;
        private System.Windows.Forms.Button btnHex;
        private System.Windows.Forms.CheckBox ckbAutoHex;
        private System.Windows.Forms.Button btnValue;
        private System.Windows.Forms.CheckBox ckbNoUnWrap;
        private System.Windows.Forms.CheckBox ckbUseNames;
        private System.Windows.Forms.Button btnCommit;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnGrid;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button btnViewer;
        private System.Windows.Forms.Button btnEditor;
        private System.Windows.Forms.Label label1;
    }
}

