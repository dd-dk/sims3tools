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
            this.btnView = new System.Windows.Forms.Button();
            this.ckbNoUnWrap = new System.Windows.Forms.CheckBox();
            this.ckbUseNames = new System.Windows.Forms.CheckBox();
            this.btnCommit = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnDataGrid = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ckbSortable
            // 
            this.ckbSortable.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbSortable.AutoSize = true;
            this.ckbSortable.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ckbSortable.Location = new System.Drawing.Point(0, 5);
            this.ckbSortable.Margin = new System.Windows.Forms.Padding(0);
            this.ckbSortable.Name = "ckbSortable";
            this.ckbSortable.Size = new System.Drawing.Size(45, 17);
            this.ckbSortable.TabIndex = 2;
            this.ckbSortable.Text = "Sort";
            this.ckbSortable.UseVisualStyleBackColor = true;
            this.ckbSortable.CheckedChanged += new System.EventHandler(this.ckbSortable_CheckedChanged);
            // 
            // btnHex
            // 
            this.btnHex.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnHex.Enabled = false;
            this.btnHex.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnHex.Location = new System.Drawing.Point(45, 2);
            this.btnHex.Margin = new System.Windows.Forms.Padding(0);
            this.btnHex.Name = "btnHex";
            this.btnHex.Size = new System.Drawing.Size(75, 23);
            this.btnHex.TabIndex = 3;
            this.btnHex.Text = "&Hex";
            this.btnHex.UseVisualStyleBackColor = true;
            this.btnHex.Click += new System.EventHandler(this.btnHex_Click);
            // 
            // ckbAutoHex
            // 
            this.ckbAutoHex.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbAutoHex.AutoSize = true;
            this.ckbAutoHex.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ckbAutoHex.Location = new System.Drawing.Point(123, 5);
            this.ckbAutoHex.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.ckbAutoHex.Name = "ckbAutoHex";
            this.ckbAutoHex.Size = new System.Drawing.Size(70, 17);
            this.ckbAutoHex.TabIndex = 4;
            this.ckbAutoHex.Text = "Auto Hex";
            this.ckbAutoHex.UseVisualStyleBackColor = true;
            this.ckbAutoHex.CheckedChanged += new System.EventHandler(this.ckbAutoHex_CheckedChanged);
            // 
            // btnView
            // 
            this.btnView.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnView.Enabled = false;
            this.btnView.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnView.Location = new System.Drawing.Point(271, 2);
            this.btnView.Margin = new System.Windows.Forms.Padding(0);
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(75, 23);
            this.btnView.TabIndex = 5;
            this.btnView.Text = "&Unwrapped";
            this.btnView.UseVisualStyleBackColor = true;
            this.btnView.Click += new System.EventHandler(this.btnView_Click);
            // 
            // ckbNoUnWrap
            // 
            this.ckbNoUnWrap.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbNoUnWrap.AutoSize = true;
            this.ckbNoUnWrap.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ckbNoUnWrap.Location = new System.Drawing.Point(199, 5);
            this.ckbNoUnWrap.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.ckbNoUnWrap.Name = "ckbNoUnWrap";
            this.ckbNoUnWrap.Size = new System.Drawing.Size(69, 17);
            this.ckbNoUnWrap.TabIndex = 6;
            this.ckbNoUnWrap.Text = "Hex Only";
            this.ckbNoUnWrap.UseVisualStyleBackColor = true;
            this.ckbNoUnWrap.CheckedChanged += new System.EventHandler(this.ckbNoUnWrap_CheckedChanged);
            // 
            // ckbUseNames
            // 
            this.ckbUseNames.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbUseNames.AutoSize = true;
            this.ckbUseNames.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ckbUseNames.Location = new System.Drawing.Point(453, 5);
            this.ckbUseNames.Margin = new System.Windows.Forms.Padding(12, 0, 12, 0);
            this.ckbUseNames.Name = "ckbUseNames";
            this.ckbUseNames.Size = new System.Drawing.Size(138, 17);
            this.ckbUseNames.TabIndex = 6;
            this.ckbUseNames.Text = "Display resource names";
            this.ckbUseNames.UseVisualStyleBackColor = true;
            this.ckbUseNames.CheckedChanged += new System.EventHandler(this.ckbUseNames_CheckedChanged);
            // 
            // btnCommit
            // 
            this.btnCommit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCommit.Enabled = false;
            this.btnCommit.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCommit.Location = new System.Drawing.Point(647, 2);
            this.btnCommit.Margin = new System.Windows.Forms.Padding(0);
            this.btnCommit.Name = "btnCommit";
            this.btnCommit.Size = new System.Drawing.Size(75, 23);
            this.btnCommit.TabIndex = 7;
            this.btnCommit.Text = "&Commit";
            this.btnCommit.UseVisualStyleBackColor = true;
            this.btnCommit.Click += new System.EventHandler(this.btnCommit_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 10;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.ckbSortable, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnCommit, 9, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnView, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.ckbUseNames, 7, 0);
            this.tableLayoutPanel1.Controls.Add(this.ckbAutoHex, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnHex, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.ckbNoUnWrap, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnDataGrid, 6, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(722, 27);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // btnDataGrid
            // 
            this.btnDataGrid.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnDataGrid.Enabled = false;
            this.btnDataGrid.Location = new System.Drawing.Point(366, 2);
            this.btnDataGrid.Margin = new System.Windows.Forms.Padding(0);
            this.btnDataGrid.Name = "btnDataGrid";
            this.btnDataGrid.Size = new System.Drawing.Size(75, 23);
            this.btnDataGrid.TabIndex = 8;
            this.btnDataGrid.Text = "Data &Grid";
            this.btnDataGrid.UseVisualStyleBackColor = true;
            this.btnDataGrid.Click += new System.EventHandler(this.btnDataGrid_Click);
            // 
            // ControlPanel
            // 
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ControlPanel";
            this.Size = new System.Drawing.Size(722, 27);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

          }

        #endregion

        private System.Windows.Forms.CheckBox ckbSortable;
        private System.Windows.Forms.Button btnHex;
        private System.Windows.Forms.CheckBox ckbAutoHex;
        private System.Windows.Forms.Button btnView;
        private System.Windows.Forms.CheckBox ckbNoUnWrap;
        private System.Windows.Forms.CheckBox ckbUseNames;
        private System.Windows.Forms.Button btnCommit;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnDataGrid;
    }
}

