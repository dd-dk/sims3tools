namespace ObjectCloner.CustomControls
{
    partial class TopicRating
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
            this.tbTopic = new System.Windows.Forms.TextBox();
            this.tbRating = new System.Windows.Forms.TextBox();
            this.lbValue = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tbTopic, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tbRating, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lbValue, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(428, 28);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tbTopic
            // 
            this.tbTopic.Location = new System.Drawing.Point(3, 3);
            this.tbTopic.Name = "tbTopic";
            this.tbTopic.Size = new System.Drawing.Size(85, 20);
            this.tbTopic.TabIndex = 0;
            this.tbTopic.Validating += new System.ComponentModel.CancelEventHandler(this.tb_Validating);
            this.tbTopic.Validated += new System.EventHandler(this.tb_Validated);
            // 
            // tbRating
            // 
            this.tbRating.Location = new System.Drawing.Point(94, 3);
            this.tbRating.Name = "tbRating";
            this.tbRating.Size = new System.Drawing.Size(85, 20);
            this.tbRating.TabIndex = 0;
            this.tbRating.Validating += new System.ComponentModel.CancelEventHandler(this.tb_Validating);
            this.tbRating.Validated += new System.EventHandler(this.tb_Validated);
            // 
            // lbValue
            // 
            this.lbValue.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lbValue.AutoSize = true;
            this.lbValue.Location = new System.Drawing.Point(185, 6);
            this.lbValue.Name = "lbValue";
            this.lbValue.Size = new System.Drawing.Size(0, 13);
            this.lbValue.TabIndex = 1;
            // 
            // TopicRating
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "TopicRating";
            this.Size = new System.Drawing.Size(428, 28);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox tbTopic;
        private System.Windows.Forms.TextBox tbRating;
        private System.Windows.Forms.Label lbValue;
    }
}
