namespace RigEditor
{
    partial class SingleEditor
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
            this.lbSingle = new System.Windows.Forms.Label();
            this.tbSingle = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbSingle
            // 
            this.lbSingle.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbSingle.AutoSize = true;
            this.lbSingle.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSingle.Location = new System.Drawing.Point(3, 6);
            this.lbSingle.Name = "lbSingle";
            this.lbSingle.Size = new System.Drawing.Size(49, 15);
            this.lbSingle.TabIndex = 0;
            this.lbSingle.Text = "label1";
            // 
            // tbSingle
            // 
            this.tbSingle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSingle.Location = new System.Drawing.Point(58, 4);
            this.tbSingle.Name = "tbSingle";
            this.tbSingle.Size = new System.Drawing.Size(61, 20);
            this.tbSingle.TabIndex = 1;
            this.tbSingle.Text = "0.00000";
            this.tbSingle.Validating += new System.ComponentModel.CancelEventHandler(this.tbSingle_Validating);
            this.tbSingle.Validated += new System.EventHandler(this.tbSingle_Validated);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.lbSingle, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tbSingle, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(122, 28);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // SingleEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SingleEditor";
            this.Size = new System.Drawing.Size(122, 28);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbSingle;
        private System.Windows.Forms.TextBox tbSingle;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
