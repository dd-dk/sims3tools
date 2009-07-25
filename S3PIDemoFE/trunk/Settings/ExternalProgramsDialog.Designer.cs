namespace S3PIDemoFE.Settings
{
    partial class ExternalProgramsDialog
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.ckbOverrideHelpers = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tbUserHelpersTxt = new System.Windows.Forms.TextBox();
            this.btnHelpersBrowse = new System.Windows.Forms.Button();
            this.ofdUserHelpersTxt = new System.Windows.Forms.OpenFileDialog();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tbUserHexEditor = new System.Windows.Forms.TextBox();
            this.ckbUserHexEditor = new System.Windows.Forms.CheckBox();
            this.btnHexEditorBrowse = new System.Windows.Forms.Button();
            this.ckbHexEditorTS = new System.Windows.Forms.CheckBox();
            this.ofdUserHexEditor = new System.Windows.Forms.OpenFileDialog();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(377, 157);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(296, 157);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // ckbOverrideHelpers
            // 
            this.ckbOverrideHelpers.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ckbOverrideHelpers.AutoSize = true;
            this.ckbOverrideHelpers.Location = new System.Drawing.Point(3, 3);
            this.ckbOverrideHelpers.Name = "ckbOverrideHelpers";
            this.ckbOverrideHelpers.Size = new System.Drawing.Size(144, 17);
            this.ckbOverrideHelpers.TabIndex = 1;
            this.ckbOverrideHelpers.Text = "Use your own Helpers.txt";
            this.ckbOverrideHelpers.UseVisualStyleBackColor = true;
            this.ckbOverrideHelpers.CheckedChanged += new System.EventHandler(this.ckbOverrideHelpers_CheckedChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.tbUserHelpersTxt, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.ckbOverrideHelpers, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnHelpersBrowse, 1, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(440, 52);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // tbUserHelpersTxt
            // 
            this.tbUserHelpersTxt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbUserHelpersTxt.Location = new System.Drawing.Point(24, 27);
            this.tbUserHelpersTxt.Margin = new System.Windows.Forms.Padding(24, 3, 3, 3);
            this.tbUserHelpersTxt.Name = "tbUserHelpersTxt";
            this.tbUserHelpersTxt.ReadOnly = true;
            this.tbUserHelpersTxt.Size = new System.Drawing.Size(332, 20);
            this.tbUserHelpersTxt.TabIndex = 2;
            // 
            // btnHelpersBrowse
            // 
            this.btnHelpersBrowse.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnHelpersBrowse.Enabled = false;
            this.btnHelpersBrowse.Location = new System.Drawing.Point(362, 26);
            this.btnHelpersBrowse.Name = "btnHelpersBrowse";
            this.btnHelpersBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnHelpersBrowse.TabIndex = 3;
            this.btnHelpersBrowse.Text = "Browse...";
            this.btnHelpersBrowse.UseVisualStyleBackColor = true;
            this.btnHelpersBrowse.Click += new System.EventHandler(this.btnHelpersBrowse_Click);
            // 
            // ofdUserHelpersTxt
            // 
            this.ofdUserHelpersTxt.FileName = "*.txt";
            this.ofdUserHelpersTxt.Filter = "Text files|*.txt|All files|*.*";
            this.ofdUserHelpersTxt.Title = "Select your Helpers.txt file";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.tbUserHexEditor, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.ckbUserHexEditor, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnHexEditorBrowse, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.ckbHexEditorTS, 0, 2);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(12, 70);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(440, 78);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // tbUserHexEditor
            // 
            this.tbUserHexEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbUserHexEditor.Location = new System.Drawing.Point(24, 27);
            this.tbUserHexEditor.Margin = new System.Windows.Forms.Padding(24, 3, 3, 3);
            this.tbUserHexEditor.Name = "tbUserHexEditor";
            this.tbUserHexEditor.ReadOnly = true;
            this.tbUserHexEditor.Size = new System.Drawing.Size(332, 20);
            this.tbUserHexEditor.TabIndex = 2;
            // 
            // ckbUserHexEditor
            // 
            this.ckbUserHexEditor.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ckbUserHexEditor.AutoSize = true;
            this.ckbUserHexEditor.Location = new System.Drawing.Point(3, 3);
            this.ckbUserHexEditor.Name = "ckbUserHexEditor";
            this.ckbUserHexEditor.Size = new System.Drawing.Size(149, 17);
            this.ckbUserHexEditor.TabIndex = 1;
            this.ckbUserHexEditor.Text = "Use an external hex editor";
            this.ckbUserHexEditor.UseVisualStyleBackColor = true;
            this.ckbUserHexEditor.CheckedChanged += new System.EventHandler(this.ckbUserHexEditor_CheckedChanged);
            // 
            // btnHexEditorBrowse
            // 
            this.btnHexEditorBrowse.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnHexEditorBrowse.Enabled = false;
            this.btnHexEditorBrowse.Location = new System.Drawing.Point(362, 26);
            this.btnHexEditorBrowse.Name = "btnHexEditorBrowse";
            this.btnHexEditorBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnHexEditorBrowse.TabIndex = 3;
            this.btnHexEditorBrowse.Text = "Browse...";
            this.btnHexEditorBrowse.UseVisualStyleBackColor = true;
            this.btnHexEditorBrowse.Click += new System.EventHandler(this.btnHexEditorBrowse_Click);
            // 
            // ckbHexEditorTS
            // 
            this.ckbHexEditorTS.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ckbHexEditorTS.AutoSize = true;
            this.ckbHexEditorTS.Enabled = false;
            this.ckbHexEditorTS.Location = new System.Drawing.Point(24, 55);
            this.ckbHexEditorTS.Margin = new System.Windows.Forms.Padding(24, 3, 3, 3);
            this.ckbHexEditorTS.Name = "ckbHexEditorTS";
            this.ckbHexEditorTS.Size = new System.Drawing.Size(143, 17);
            this.ckbHexEditorTS.TabIndex = 4;
            this.ckbHexEditorTS.Text = "Does not update file time";
            this.ckbHexEditorTS.UseVisualStyleBackColor = true;
            this.ckbHexEditorTS.CheckedChanged += new System.EventHandler(this.ckbUserHexEditor_CheckedChanged);
            // 
            // ofdUserHexEditor
            // 
            this.ofdUserHexEditor.FileName = "*.exe";
            this.ofdUserHexEditor.Filter = "Program files|*.exe";
            this.ofdUserHexEditor.Title = "Choose your hex editor";
            // 
            // ExternalProgramsDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(464, 192);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "ExternalProgramsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "External Program Settings";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox ckbOverrideHelpers;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox tbUserHelpersTxt;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnHelpersBrowse;
        private System.Windows.Forms.OpenFileDialog ofdUserHelpersTxt;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TextBox tbUserHexEditor;
        private System.Windows.Forms.CheckBox ckbUserHexEditor;
        private System.Windows.Forms.Button btnHexEditorBrowse;
        private System.Windows.Forms.OpenFileDialog ofdUserHexEditor;
        private System.Windows.Forms.CheckBox ckbHexEditorTS;
    }
}