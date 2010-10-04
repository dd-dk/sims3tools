namespace RigEditor
{
    partial class UInt32Editor
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
            this.lbUInt32 = new System.Windows.Forms.Label();
            this.tbUInt32 = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.lbUInt32, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tbUInt32, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(130, 26);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // lbUInt32
            // 
            this.lbUInt32.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbUInt32.AutoSize = true;
            this.lbUInt32.Location = new System.Drawing.Point(3, 6);
            this.lbUInt32.Name = "lbUInt32";
            this.lbUInt32.Size = new System.Drawing.Size(35, 13);
            this.lbUInt32.TabIndex = 0;
            this.lbUInt32.Text = "label1";
            // 
            // tbUInt32
            // 
            this.tbUInt32.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbUInt32.Location = new System.Drawing.Point(44, 3);
            this.tbUInt32.Name = "tbUInt32";
            this.tbUInt32.Size = new System.Drawing.Size(83, 20);
            this.tbUInt32.TabIndex = 1;
            this.tbUInt32.Text = "0x00000000";
            // 
            // UInt32Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "UInt32Editor";
            this.Size = new System.Drawing.Size(130, 26);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lbUInt32;
        private System.Windows.Forms.TextBox tbUInt32;
    }
}
