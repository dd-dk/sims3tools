namespace RigEditor
{
    partial class QuadEditor
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
            this.singleEditor1 = new RigEditor.SingleEditor();
            this.singleEditor2 = new RigEditor.SingleEditor();
            this.singleEditor3 = new RigEditor.SingleEditor();
            this.singleEditor4 = new RigEditor.SingleEditor();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.singleEditor1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.singleEditor2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.singleEditor3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.singleEditor4, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(150, 109);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // singleEditor1
            // 
            this.singleEditor1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.singleEditor1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.singleEditor1.Location = new System.Drawing.Point(3, 3);
            this.singleEditor1.Name = "singleEditor1";
            this.singleEditor1.Size = new System.Drawing.Size(144, 21);
            this.singleEditor1.TabIndex = 0;
            this.singleEditor1.Text = "X";
            this.singleEditor1.ValueChanged += new System.EventHandler(this.singleEditor1_ValueChanged);
            // 
            // singleEditor2
            // 
            this.singleEditor2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.singleEditor2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.singleEditor2.Location = new System.Drawing.Point(3, 30);
            this.singleEditor2.Name = "singleEditor2";
            this.singleEditor2.Size = new System.Drawing.Size(144, 21);
            this.singleEditor2.TabIndex = 1;
            this.singleEditor2.Text = "Y";
            this.singleEditor2.ValueChanged += new System.EventHandler(this.singleEditor2_ValueChanged);
            // 
            // singleEditor3
            // 
            this.singleEditor3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.singleEditor3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.singleEditor3.Location = new System.Drawing.Point(3, 57);
            this.singleEditor3.Name = "singleEditor3";
            this.singleEditor3.Size = new System.Drawing.Size(144, 21);
            this.singleEditor3.TabIndex = 2;
            this.singleEditor3.Text = "Z";
            this.singleEditor3.ValueChanged += new System.EventHandler(this.singleEditor3_ValueChanged);
            // 
            // singleEditor4
            // 
            this.singleEditor4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.singleEditor4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.singleEditor4.Location = new System.Drawing.Point(3, 84);
            this.singleEditor4.Name = "singleEditor4";
            this.singleEditor4.Size = new System.Drawing.Size(144, 22);
            this.singleEditor4.TabIndex = 3;
            this.singleEditor4.Text = "W";
            this.singleEditor4.ValueChanged += new System.EventHandler(this.singleEditor4_ValueChanged);
            // 
            // QuadEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "QuadEditor";
            this.Size = new System.Drawing.Size(150, 109);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private SingleEditor singleEditor1;
        private SingleEditor singleEditor2;
        private SingleEditor singleEditor3;
        private SingleEditor singleEditor4;
    }
}
