namespace RigEditor
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tlpBones = new System.Windows.Forms.TableLayoutPanel();
            this.lbSelect = new System.Windows.Forms.Label();
            this.lbBone = new System.Windows.Forms.Label();
            this.lbMatrix = new System.Windows.Forms.Label();
            this.lbTransform = new System.Windows.Forms.Label();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tlpBones.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(693, 744);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "Save";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(612, 744);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Abandon";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tlpBones
            // 
            this.tlpBones.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpBones.AutoSize = true;
            this.tlpBones.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpBones.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset;
            this.tlpBones.ColumnCount = 4;
            this.tlpBones.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpBones.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpBones.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpBones.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpBones.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpBones.Controls.Add(this.lbSelect, 0, 0);
            this.tlpBones.Controls.Add(this.lbBone, 1, 0);
            this.tlpBones.Controls.Add(this.lbMatrix, 2, 0);
            this.tlpBones.Controls.Add(this.lbTransform, 3, 0);
            this.tlpBones.Location = new System.Drawing.Point(0, 0);
            this.tlpBones.Margin = new System.Windows.Forms.Padding(0);
            this.tlpBones.Name = "tlpBones";
            this.tlpBones.RowCount = 2;
            this.tlpBones.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpBones.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpBones.Size = new System.Drawing.Size(755, 25);
            this.tlpBones.TabIndex = 2;
            // 
            // lbSelect
            // 
            this.lbSelect.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbSelect.AutoSize = true;
            this.lbSelect.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSelect.Location = new System.Drawing.Point(5, 2);
            this.lbSelect.Name = "lbSelect";
            this.lbSelect.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.lbSelect.Size = new System.Drawing.Size(41, 19);
            this.lbSelect.TabIndex = 0;
            this.lbSelect.Text = "label1";
            // 
            // lbBone
            // 
            this.lbBone.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbBone.AutoSize = true;
            this.lbBone.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbBone.Location = new System.Drawing.Point(321, 2);
            this.lbBone.Name = "lbBone";
            this.lbBone.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.lbBone.Size = new System.Drawing.Size(42, 19);
            this.lbBone.TabIndex = 0;
            this.lbBone.Text = "Bones";
            // 
            // lbMatrix
            // 
            this.lbMatrix.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbMatrix.AutoSize = true;
            this.lbMatrix.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMatrix.Location = new System.Drawing.Point(638, 2);
            this.lbMatrix.Name = "lbMatrix";
            this.lbMatrix.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.lbMatrix.Size = new System.Drawing.Size(41, 19);
            this.lbMatrix.TabIndex = 1;
            this.lbMatrix.Text = "Matrix";
            // 
            // lbTransform
            // 
            this.lbTransform.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbTransform.AutoSize = true;
            this.lbTransform.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTransform.Location = new System.Drawing.Point(687, 2);
            this.lbTransform.Name = "lbTransform";
            this.lbTransform.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.lbTransform.Size = new System.Drawing.Size(63, 19);
            this.lbTransform.TabIndex = 1;
            this.lbTransform.Text = "Transform";
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDelete.Location = new System.Drawing.Point(12, 744);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(104, 23);
            this.btnDelete.TabIndex = 3;
            this.btnDelete.Text = "Delete Selected";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAdd.Location = new System.Drawing.Point(122, 744);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(104, 23);
            this.btnAdd.TabIndex = 4;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.tlpBones);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(756, 726);
            this.panel1.TabIndex = 5;
            // 
            // MainForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(780, 779);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Rig Bone Editor";
            this.tlpBones.ResumeLayout(false);
            this.tlpBones.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TableLayoutPanel tlpBones;
        private System.Windows.Forms.Label lbBone;
        private System.Windows.Forms.Label lbSelect;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lbMatrix;
        private System.Windows.Forms.Label lbTransform;

    }
}