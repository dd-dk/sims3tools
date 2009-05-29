namespace S3PIDemoFE.Filter
{
    partial class ResourceFilterWidget
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
            this.tlpResourceInfo = new System.Windows.Forms.TableLayoutPanel();
            this.flpControls = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.lbCount = new System.Windows.Forms.Label();
            this.ckbFilter = new System.Windows.Forms.CheckBox();
            this.tlpResourceInfo.SuspendLayout();
            this.flpControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpResourceInfo
            // 
            this.tlpResourceInfo.ColumnCount = 2;
            this.tlpResourceInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpResourceInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpResourceInfo.Controls.Add(this.flpControls, 0, 0);
            this.tlpResourceInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpResourceInfo.Location = new System.Drawing.Point(0, 0);
            this.tlpResourceInfo.Name = "tlpResourceInfo";
            this.tlpResourceInfo.RowCount = 2;
            this.tlpResourceInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpResourceInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpResourceInfo.Size = new System.Drawing.Size(844, 150);
            this.tlpResourceInfo.TabIndex = 0;
            // 
            // flpControls
            // 
            this.flpControls.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.flpControls.AutoSize = true;
            this.flpControls.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpControls.Controls.Add(this.label1);
            this.flpControls.Controls.Add(this.lbCount);
            this.flpControls.Controls.Add(this.ckbFilter);
            this.flpControls.Location = new System.Drawing.Point(3, 19);
            this.flpControls.Name = "flpControls";
            this.flpControls.Size = new System.Drawing.Size(90, 36);
            this.flpControls.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Count: ";
            // 
            // lbCount
            // 
            this.lbCount.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbCount.AutoSize = true;
            this.flpControls.SetFlowBreak(this.lbCount, true);
            this.lbCount.Location = new System.Drawing.Point(41, 0);
            this.lbCount.Margin = new System.Windows.Forms.Padding(0);
            this.lbCount.Name = "lbCount";
            this.lbCount.Size = new System.Drawing.Size(49, 13);
            this.lbCount.TabIndex = 1;
            this.lbCount.Text = "nnnnnnn";
            // 
            // ckbFilter
            // 
            this.ckbFilter.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ckbFilter.AutoSize = true;
            this.ckbFilter.Location = new System.Drawing.Point(3, 16);
            this.ckbFilter.Name = "ckbFilter";
            this.ckbFilter.Size = new System.Drawing.Size(84, 17);
            this.ckbFilter.TabIndex = 0;
            this.ckbFilter.Text = "Enable Filter";
            this.ckbFilter.UseVisualStyleBackColor = true;
            this.ckbFilter.CheckedChanged += new System.EventHandler(this.ckbFilter_CheckedChanged);
            // 
            // ResourceFilterWidget
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlpResourceInfo);
            this.Name = "ResourceFilterWidget";
            this.Size = new System.Drawing.Size(844, 150);
            this.tlpResourceInfo.ResumeLayout(false);
            this.tlpResourceInfo.PerformLayout();
            this.flpControls.ResumeLayout(false);
            this.flpControls.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpResourceInfo;
        private System.Windows.Forms.CheckBox ckbFilter;
        private System.Windows.Forms.FlowLayoutPanel flpControls;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbCount;
    }
}
