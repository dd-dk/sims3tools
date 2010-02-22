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
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tbUserHexEditor = new System.Windows.Forms.TextBox();
            this.ckbUserHexEditor = new System.Windows.Forms.CheckBox();
            this.btnHexEditorBrowse = new System.Windows.Forms.Button();
            this.ckbHexEditorTS = new System.Windows.Forms.CheckBox();
            this.ckbQuotes = new System.Windows.Forms.CheckBox();
            this.ofdUserHexEditor = new System.Windows.Forms.OpenFileDialog();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.tlpHelpers = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tlpHelpers.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(349, 167);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(268, 167);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.tbUserHexEditor, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.ckbUserHexEditor, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnHexEditorBrowse, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.ckbHexEditorTS, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.ckbQuotes, 0, 3);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 5;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(406, 102);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // tbUserHexEditor
            // 
            this.tbUserHexEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbUserHexEditor.Location = new System.Drawing.Point(24, 27);
            this.tbUserHexEditor.Margin = new System.Windows.Forms.Padding(24, 3, 3, 3);
            this.tbUserHexEditor.Name = "tbUserHexEditor";
            this.tbUserHexEditor.ReadOnly = true;
            this.tbUserHexEditor.Size = new System.Drawing.Size(298, 20);
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
            this.btnHexEditorBrowse.Location = new System.Drawing.Point(328, 26);
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
            // ckbQuotes
            // 
            this.ckbQuotes.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ckbQuotes.AutoSize = true;
            this.ckbQuotes.Enabled = false;
            this.ckbQuotes.Location = new System.Drawing.Point(24, 78);
            this.ckbQuotes.Margin = new System.Windows.Forms.Padding(24, 3, 3, 3);
            this.ckbQuotes.Name = "ckbQuotes";
            this.ckbQuotes.Size = new System.Drawing.Size(170, 17);
            this.ckbQuotes.TabIndex = 4;
            this.ckbQuotes.Text = "Needs quotes around filename";
            this.ckbQuotes.UseVisualStyleBackColor = true;
            this.ckbQuotes.CheckedChanged += new System.EventHandler(this.ckbUserHexEditor_CheckedChanged);
            // 
            // ofdUserHexEditor
            // 
            this.ofdUserHexEditor.FileName = "*.exe";
            this.ofdUserHexEditor.Filter = "Program files|*.exe";
            this.ofdUserHexEditor.Title = "Choose your hex editor";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tlpHelpers, 0, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(412, 149);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 111);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Helpers:";
            // 
            // tlpHelpers
            // 
            this.tlpHelpers.AutoSize = true;
            this.tlpHelpers.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpHelpers.ColumnCount = 3;
            this.tlpHelpers.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpHelpers.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpHelpers.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpHelpers.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpHelpers.Controls.Add(this.label1, 1, 0);
            this.tlpHelpers.Controls.Add(this.label3, 0, 0);
            this.tlpHelpers.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpHelpers.Location = new System.Drawing.Point(3, 131);
            this.tlpHelpers.Name = "tlpHelpers";
            this.tlpHelpers.RowCount = 2;
            this.tlpHelpers.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpHelpers.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpHelpers.Size = new System.Drawing.Size(406, 13);
            this.tlpHelpers.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Disabled";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Info";
            // 
            // ExternalProgramsDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(436, 202);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "ExternalProgramsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "External Program Settings";
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tlpHelpers.ResumeLayout(false);
            this.tlpHelpers.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TextBox tbUserHexEditor;
        private System.Windows.Forms.CheckBox ckbUserHexEditor;
        private System.Windows.Forms.Button btnHexEditorBrowse;
        private System.Windows.Forms.OpenFileDialog ofdUserHexEditor;
        private System.Windows.Forms.CheckBox ckbHexEditorTS;
        private System.Windows.Forms.CheckBox ckbQuotes;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TableLayoutPanel tlpHelpers;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
    }
}