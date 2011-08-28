namespace ObjectCloner.SettingsForms
{
    partial class GameFolders
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
            this.tlpGameFolders = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.btnReset = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label8 = new System.Windows.Forms.Label();
            this.ckbCustomContent = new System.Windows.Forms.CheckBox();
            this.tbCCFolder = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.ckbEditor = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.tbEditorPath = new System.Windows.Forms.TextBox();
            this.btnCCEdit = new System.Windows.Forms.Button();
            this.btnPkgEdEdit = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tlpGameFolders.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpGameFolders
            // 
            this.tlpGameFolders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpGameFolders.AutoSize = true;
            this.tlpGameFolders.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpGameFolders.ColumnCount = 4;
            this.tlpGameFolders.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpGameFolders.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpGameFolders.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpGameFolders.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpGameFolders.Controls.Add(this.label1, 0, 0);
            this.tlpGameFolders.Controls.Add(this.label2, 1, 0);
            this.tlpGameFolders.Controls.Add(this.label3, 2, 0);
            this.tlpGameFolders.Controls.Add(this.label4, 3, 0);
            this.tlpGameFolders.Location = new System.Drawing.Point(12, 145);
            this.tlpGameFolders.Name = "tlpGameFolders";
            this.tlpGameFolders.RowCount = 2;
            this.tlpGameFolders.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpGameFolders.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpGameFolders.Size = new System.Drawing.Size(604, 13);
            this.tlpGameFolders.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Game ID";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(66, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Enabled";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(125, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Install Folder";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(572, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Edit";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnClose.Location = new System.Drawing.Point(541, 168);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.Description = "Select the folder where your Sims3 game is installed.";
            this.folderBrowserDialog1.ShowNewFolderButton = false;
            // 
            // btnReset
            // 
            this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReset.Location = new System.Drawing.Point(460, 168);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 2;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label8, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.ckbCustomContent, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.tbCCFolder, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.label6, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.label7, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.label9, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label10, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ckbEditor, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label11, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.tbEditorPath, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnCCEdit, 3, 4);
            this.tableLayoutPanel1.Controls.Add(this.btnPkgEdEdit, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.label12, 3, 3);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(604, 126);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(569, 13);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "Edit";
            // 
            // ckbCustomContent
            // 
            this.ckbCustomContent.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbCustomContent.AutoSize = true;
            this.ckbCustomContent.Location = new System.Drawing.Point(85, 100);
            this.ckbCustomContent.Name = "ckbCustomContent";
            this.ckbCustomContent.Size = new System.Drawing.Size(15, 14);
            this.ckbCustomContent.TabIndex = 1;
            this.ckbCustomContent.UseVisualStyleBackColor = true;
            this.ckbCustomContent.CheckedChanged += new System.EventHandler(this.ckbCustomContent_CheckedChanged);
            // 
            // tbCCFolder
            // 
            this.tbCCFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCCFolder.Location = new System.Drawing.Point(125, 97);
            this.tbCCFolder.Name = "tbCCFolder";
            this.tbCCFolder.ReadOnly = true;
            this.tbCCFolder.Size = new System.Drawing.Size(435, 20);
            this.tbCCFolder.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(66, 80);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Enabled";
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(125, 80);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(80, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Install Folder";
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(66, 13);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "Enabled";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(6, 67);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(51, 26);
            this.label5.TabIndex = 0;
            this.label5.Text = "Custom\r\nContent";
            // 
            // label10
            // 
            this.label10.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(3, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(57, 26);
            this.label10.TabIndex = 0;
            this.label10.Text = "Package\r\nEditor";
            // 
            // ckbEditor
            // 
            this.ckbEditor.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbEditor.AutoSize = true;
            this.ckbEditor.Location = new System.Drawing.Point(85, 33);
            this.ckbEditor.Name = "ckbEditor";
            this.ckbEditor.Size = new System.Drawing.Size(15, 14);
            this.ckbEditor.TabIndex = 1;
            this.ckbEditor.UseVisualStyleBackColor = true;
            this.ckbEditor.CheckedChanged += new System.EventHandler(this.ckbEditor_CheckedChanged);
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(125, 13);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(83, 13);
            this.label11.TabIndex = 0;
            this.label11.Text = "Program Path";
            // 
            // tbEditorPath
            // 
            this.tbEditorPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbEditorPath.Location = new System.Drawing.Point(125, 30);
            this.tbEditorPath.Name = "tbEditorPath";
            this.tbEditorPath.ReadOnly = true;
            this.tbEditorPath.Size = new System.Drawing.Size(435, 20);
            this.tbEditorPath.TabIndex = 3;
            // 
            // btnCCEdit
            // 
            this.btnCCEdit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCCEdit.AutoSize = true;
            this.btnCCEdit.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCCEdit.Location = new System.Drawing.Point(566, 96);
            this.btnCCEdit.Name = "btnCCEdit";
            this.btnCCEdit.Size = new System.Drawing.Size(35, 23);
            this.btnCCEdit.TabIndex = 2;
            this.btnCCEdit.Text = "Edit";
            this.btnCCEdit.UseVisualStyleBackColor = true;
            this.btnCCEdit.Click += new System.EventHandler(this.btnCCEdit_Click);
            // 
            // btnPkgEdEdit
            // 
            this.btnPkgEdEdit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnPkgEdEdit.AutoSize = true;
            this.btnPkgEdEdit.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnPkgEdEdit.Location = new System.Drawing.Point(566, 29);
            this.btnPkgEdEdit.Name = "btnPkgEdEdit";
            this.btnPkgEdEdit.Size = new System.Drawing.Size(35, 23);
            this.btnPkgEdEdit.TabIndex = 2;
            this.btnPkgEdEdit.Text = "Edit";
            this.btnPkgEdEdit.UseVisualStyleBackColor = true;
            this.btnPkgEdEdit.Click += new System.EventHandler(this.btnPkgEdEdit_Click);
            // 
            // label12
            // 
            this.label12.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(569, 80);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(29, 13);
            this.label12.TabIndex = 0;
            this.label12.Text = "Edit";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "exe";
            this.openFileDialog1.FileName = "*.exe";
            this.openFileDialog1.Filter = "Program files|*.exe|All files|*.*";
            this.openFileDialog1.Title = "Locate package editor";
            // 
            // GameFolders
            // 
            this.AcceptButton = this.btnClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(628, 203);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.tlpGameFolders);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "GameFolders";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Game Folders";
            this.tlpGameFolders.ResumeLayout(false);
            this.tlpGameFolders.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpGameFolders;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox ckbCustomContent;
        private System.Windows.Forms.Button btnCCEdit;
        private System.Windows.Forms.TextBox tbCCFolder;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox ckbEditor;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox tbEditorPath;
        private System.Windows.Forms.Button btnPkgEdEdit;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}