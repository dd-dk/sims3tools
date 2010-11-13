namespace RigEditor
{
    partial class TransformEditor
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lbShear0 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.uInt32Editor1 = new RigEditor.UInt32Editor();
            this.quadEditor1 = new RigEditor.QuadEditor();
            this.tripleEditor1 = new RigEditor.TripleEditor();
            this.tripleEditor2 = new RigEditor.TripleEditor();
            this.tripleEditor3 = new RigEditor.TripleEditor();
            this.tripleEditor4 = new RigEditor.TripleEditor();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.lbShear0, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label5, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label6, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.uInt32Editor1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.quadEditor1, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.tripleEditor1, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.tripleEditor2, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.tripleEditor3, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.tripleEditor4, 2, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(266, 272);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(22, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(3);
            this.label1.Size = new System.Drawing.Size(43, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "Flags";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(94, 0);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(3);
            this.label2.Size = new System.Drawing.Size(75, 19);
            this.label2.TabIndex = 0;
            this.label2.Text = "Orientation";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(192, 0);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Padding = new System.Windows.Forms.Padding(3);
            this.label3.Size = new System.Drawing.Size(58, 19);
            this.label3.TabIndex = 0;
            this.label3.Text = "Position";
            // 
            // lbShear0
            // 
            this.lbShear0.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbShear0.AutoSize = true;
            this.lbShear0.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbShear0.Location = new System.Drawing.Point(1, 136);
            this.lbShear0.Margin = new System.Windows.Forms.Padding(0);
            this.lbShear0.Name = "lbShear0";
            this.lbShear0.Padding = new System.Windows.Forms.Padding(3);
            this.lbShear0.Size = new System.Drawing.Size(85, 19);
            this.lbShear0.TabIndex = 0;
            this.lbShear0.Text = "ScaleShear0";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(89, 136);
            this.label5.Margin = new System.Windows.Forms.Padding(0);
            this.label5.Name = "label5";
            this.label5.Padding = new System.Windows.Forms.Padding(3);
            this.label5.Size = new System.Drawing.Size(85, 19);
            this.label5.TabIndex = 0;
            this.label5.Text = "ScaleShear1";
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(178, 136);
            this.label6.Margin = new System.Windows.Forms.Padding(0);
            this.label6.Name = "label6";
            this.label6.Padding = new System.Windows.Forms.Padding(3);
            this.label6.Size = new System.Drawing.Size(85, 19);
            this.label6.TabIndex = 0;
            this.label6.Text = "ScaleShear2";
            // 
            // uInt32Editor1
            // 
            this.uInt32Editor1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.uInt32Editor1.Location = new System.Drawing.Point(3, 65);
            this.uInt32Editor1.Name = "uInt32Editor1";
            this.uInt32Editor1.Size = new System.Drawing.Size(82, 26);
            this.uInt32Editor1.TabIndex = 1;
            this.uInt32Editor1.Value = ((uint)(0u));
            this.uInt32Editor1.ValueChanged += new System.EventHandler(this.flags_ValueChanged);
            // 
            // quadEditor1
            // 
            this.quadEditor1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.quadEditor1.Location = new System.Drawing.Point(91, 23);
            this.quadEditor1.Name = "quadEditor1";
            this.quadEditor1.Size = new System.Drawing.Size(82, 110);
            this.quadEditor1.TabIndex = 2;
            this.quadEditor1.ValueChanged += new System.EventHandler(this.quadEditor1_ValueChanged);
            // 
            // tripleEditor1
            // 
            this.tripleEditor1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tripleEditor1.Location = new System.Drawing.Point(179, 36);
            this.tripleEditor1.Name = "tripleEditor1";
            this.tripleEditor1.Size = new System.Drawing.Size(84, 83);
            this.tripleEditor1.TabIndex = 3;
            this.tripleEditor1.ValueChanged += new System.EventHandler(this.pos_ValueChanged);
            // 
            // tripleEditor2
            // 
            this.tripleEditor2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tripleEditor2.Location = new System.Drawing.Point(3, 172);
            this.tripleEditor2.Name = "tripleEditor2";
            this.tripleEditor2.Size = new System.Drawing.Size(82, 83);
            this.tripleEditor2.TabIndex = 3;
            this.tripleEditor2.ValueChanged += new System.EventHandler(this.sh_ValueChanged);
            // 
            // tripleEditor3
            // 
            this.tripleEditor3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tripleEditor3.Location = new System.Drawing.Point(91, 172);
            this.tripleEditor3.Name = "tripleEditor3";
            this.tripleEditor3.Size = new System.Drawing.Size(82, 83);
            this.tripleEditor3.TabIndex = 3;
            this.tripleEditor3.ValueChanged += new System.EventHandler(this.sh_ValueChanged);
            // 
            // tripleEditor4
            // 
            this.tripleEditor4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tripleEditor4.Location = new System.Drawing.Point(179, 172);
            this.tripleEditor4.Name = "tripleEditor4";
            this.tripleEditor4.Size = new System.Drawing.Size(84, 83);
            this.tripleEditor4.TabIndex = 3;
            this.tripleEditor4.ValueChanged += new System.EventHandler(this.sh_ValueChanged);
            // 
            // TransformEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "TransformEditor";
            this.Size = new System.Drawing.Size(266, 272);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbShear0;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private UInt32Editor uInt32Editor1;
        private QuadEditor quadEditor1;
        private TripleEditor tripleEditor1;
        private TripleEditor tripleEditor2;
        private TripleEditor tripleEditor3;
        private TripleEditor tripleEditor4;
    }
}
