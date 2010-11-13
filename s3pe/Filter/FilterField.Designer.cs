namespace S3PIDemoFE.Filter
{
    partial class FilterField
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.ckbFilter = new System.Windows.Forms.CheckBox();
            this.tbFilter = new System.Windows.Forms.TextBox();
            this.tbValue = new System.Windows.Forms.TextBox();
            this.lbField = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.ckbFilter, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.tbFilter, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.tbValue, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.lbField, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(102, 82);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // ckbFilter
            // 
            this.ckbFilter.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbFilter.AutoSize = true;
            this.ckbFilter.Location = new System.Drawing.Point(3, 45);
            this.ckbFilter.Name = "ckbFilter";
            this.ckbFilter.Size = new System.Drawing.Size(15, 14);
            this.ckbFilter.TabIndex = 3;
            this.ckbFilter.UseVisualStyleBackColor = true;
            this.ckbFilter.CheckedChanged += new System.EventHandler(this.ckbFilter_CheckedChanged);
            // 
            // tbFilter
            // 
            this.tbFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFilter.Enabled = false;
            this.tbFilter.Location = new System.Drawing.Point(24, 42);
            this.tbFilter.Name = "tbFilter";
            this.tbFilter.Size = new System.Drawing.Size(75, 20);
            this.tbFilter.TabIndex = 4;
            this.tbFilter.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbFilter.TextChanged += new System.EventHandler(this.tbFilter_TextChanged);
            // 
            // tbValue
            // 
            this.tbValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbValue.Location = new System.Drawing.Point(24, 16);
            this.tbValue.Name = "tbValue";
            this.tbValue.ReadOnly = true;
            this.tbValue.Size = new System.Drawing.Size(75, 20);
            this.tbValue.TabIndex = 2;
            this.tbValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lbField
            // 
            this.lbField.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lbField.AutoEllipsis = true;
            this.tableLayoutPanel1.SetColumnSpan(this.lbField, 2);
            this.lbField.Location = new System.Drawing.Point(3, 0);
            this.lbField.Name = "lbField";
            this.lbField.Size = new System.Drawing.Size(96, 13);
            this.lbField.TabIndex = 1;
            this.lbField.Text = "label1";
            this.lbField.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FilterField
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FilterField";
            this.Size = new System.Drawing.Size(102, 82);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox ckbFilter;
        private System.Windows.Forms.TextBox tbFilter;
        private System.Windows.Forms.TextBox tbValue;
        private System.Windows.Forms.Label lbField;
    }
}
