/*
 *  Copyright 2009 Jonathan Haas
 * 
 *  This file is part of s3translate.
 *  
 *  s3translate is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  s3translate is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with s3translate.  If not, see <http://www.gnu.org/licenses/>.
 *  
 *  s3translate uses the s3pi libraries by Peter L Jones (pljones@users.sf.net)
 *  For details visit http://sourceforge.net/projects/s3pi/
*/

namespace S3Translate
{
    partial class Form1
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.tableLayoutPanel_MainInge = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel_LeftInge = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.btnChangeGUID = new System.Windows.Forms.Button();
            this.btnAddString = new System.Windows.Forms.Button();
            this.btnSetToAll = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tbGUID = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.btnStringToTarget = new System.Windows.Forms.Button();
            this.btnStringToAll = new System.Windows.Forms.Button();
            this.btnSetToTarget = new System.Windows.Forms.Button();
            this.btnDelString = new System.Windows.Forms.Button();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel_SourceTarget = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbTargetLang = new System.Windows.Forms.ComboBox();
            this.cmbSourceLang = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel_SetPicking = new System.Windows.Forms.TableLayoutPanel();
            this.label16 = new System.Windows.Forms.Label();
            this.cmbSetPicker = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel_RightInge = new System.Windows.Forms.TableLayoutPanel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel9 = new System.Windows.Forms.TableLayoutPanel();
            this.lstStrings = new System.Windows.Forms.ListView();
            this.chInstance = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chSource = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chTarget = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tlp_EditAndRef = new System.Windows.Forms.TableLayoutPanel();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.tlp_SourceRef = new System.Windows.Forms.TableLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.txtSource = new System.Windows.Forms.TextBox();
            this.tlp_TargetEdit = new System.Windows.Forms.TableLayoutPanel();
            this.txtTarget = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tlpFind = new System.Windows.Forms.TableLayoutPanel();
            this.txtFind = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.btnFindNext = new System.Windows.Forms.Button();
            this.btnFindFirst = new System.Windows.Forms.Button();
            this.btnCommit = new System.Windows.Forms.Button();
            this.ckbAutoCommit = new System.Windows.Forms.CheckBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.savePackageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.savePackageAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sTBLsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createNewSetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteSetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.importFromPackageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importFromSTBLFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToPackageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToSTBLFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mergeAllSetsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.licenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.prg = new System.Windows.Forms.ToolStripProgressBar();
            this.tlp_CommitCancel = new System.Windows.Forms.TableLayoutPanel();
            this.button_AbandonEdit = new System.Windows.Forms.Button();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.tableLayoutPanel_MainInge.SuspendLayout();
            this.tableLayoutPanel_LeftInge.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel_SourceTarget.SuspendLayout();
            this.tableLayoutPanel_SetPicking.SuspendLayout();
            this.tableLayoutPanel_RightInge.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel9.SuspendLayout();
            this.tlp_EditAndRef.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tlp_SourceRef.SuspendLayout();
            this.tlp_TargetEdit.SuspendLayout();
            this.tlpFind.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.tlp_CommitCancel.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.Enabled = false;
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.tableLayoutPanel_MainInge);
            this.toolStripContainer1.ContentPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1187, 722);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // toolStripContainer1.LeftToolStripPanel
            // 
            this.toolStripContainer1.LeftToolStripPanel.Enabled = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.toolStripContainer1.Name = "toolStripContainer1";
            // 
            // toolStripContainer1.RightToolStripPanel
            // 
            this.toolStripContainer1.RightToolStripPanel.Enabled = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(1187, 749);
            this.toolStripContainer1.TabIndex = 0;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.menuStrip1);
            // 
            // tableLayoutPanel_MainInge
            // 
            this.tableLayoutPanel_MainInge.ColumnCount = 2;
            this.tableLayoutPanel_MainInge.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel_MainInge.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_MainInge.Controls.Add(this.tableLayoutPanel_LeftInge, 0, 0);
            this.tableLayoutPanel_MainInge.Controls.Add(this.tableLayoutPanel_RightInge, 1, 0);
            this.tableLayoutPanel_MainInge.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_MainInge.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel_MainInge.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tableLayoutPanel_MainInge.Name = "tableLayoutPanel_MainInge";
            this.tableLayoutPanel_MainInge.RowCount = 2;
            this.tableLayoutPanel_MainInge.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_MainInge.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel_MainInge.Size = new System.Drawing.Size(1187, 722);
            this.tableLayoutPanel_MainInge.TabIndex = 0;
            // 
            // tableLayoutPanel_LeftInge
            // 
            this.tableLayoutPanel_LeftInge.AutoSize = true;
            this.tableLayoutPanel_LeftInge.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel_LeftInge.ColumnCount = 1;
            this.tableLayoutPanel_LeftInge.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_LeftInge.Controls.Add(this.tableLayoutPanel6, 0, 1);
            this.tableLayoutPanel_LeftInge.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel_LeftInge.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel_LeftInge.Name = "tableLayoutPanel_LeftInge";
            this.tableLayoutPanel_LeftInge.RowCount = 4;
            this.tableLayoutPanel_LeftInge.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_LeftInge.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_LeftInge.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_LeftInge.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_LeftInge.Size = new System.Drawing.Size(431, 457);
            this.tableLayoutPanel_LeftInge.TabIndex = 0;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.AutoSize = true;
            this.tableLayoutPanel6.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel6.ColumnCount = 2;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel6.Controls.Add(this.btnChangeGUID, 1, 1);
            this.tableLayoutPanel6.Controls.Add(this.btnAddString, 1, 2);
            this.tableLayoutPanel6.Controls.Add(this.btnSetToAll, 1, 8);
            this.tableLayoutPanel6.Controls.Add(this.label8, 0, 7);
            this.tableLayoutPanel6.Controls.Add(this.label7, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.tbGUID, 0, 1);
            this.tableLayoutPanel6.Controls.Add(this.label10, 0, 4);
            this.tableLayoutPanel6.Controls.Add(this.btnStringToTarget, 1, 4);
            this.tableLayoutPanel6.Controls.Add(this.btnStringToAll, 1, 5);
            this.tableLayoutPanel6.Controls.Add(this.btnSetToTarget, 1, 7);
            this.tableLayoutPanel6.Controls.Add(this.btnDelString, 1, 0);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(4, 176);
            this.tableLayoutPanel6.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 10;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(423, 277);
            this.tableLayoutPanel6.TabIndex = 1;
            // 
            // btnChangeGUID
            // 
            this.btnChangeGUID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChangeGUID.AutoSize = true;
            this.btnChangeGUID.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnChangeGUID.Enabled = false;
            this.btnChangeGUID.Location = new System.Drawing.Point(204, 39);
            this.btnChangeGUID.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnChangeGUID.Name = "btnChangeGUID";
            this.btnChangeGUID.Size = new System.Drawing.Size(215, 27);
            this.btnChangeGUID.TabIndex = 3;
            this.btnChangeGUID.Text = "&Edit GUID...";
            this.btnChangeGUID.UseVisualStyleBackColor = true;
            this.btnChangeGUID.Click += new System.EventHandler(this.btnChangeGUID_Click);
            // 
            // btnAddString
            // 
            this.btnAddString.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddString.AutoSize = true;
            this.btnAddString.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAddString.Enabled = false;
            this.btnAddString.Location = new System.Drawing.Point(204, 74);
            this.btnAddString.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnAddString.Name = "btnAddString";
            this.btnAddString.Size = new System.Drawing.Size(215, 27);
            this.btnAddString.TabIndex = 4;
            this.btnAddString.Text = "&Add string...";
            this.btnAddString.UseVisualStyleBackColor = true;
            this.btnAddString.Click += new System.EventHandler(this.btnAddString_Click);
            // 
            // btnSetToAll
            // 
            this.btnSetToAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSetToAll.AutoSize = true;
            this.btnSetToAll.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSetToAll.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnSetToAll.Enabled = false;
            this.btnSetToAll.Location = new System.Drawing.Point(204, 246);
            this.btnSetToAll.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSetToAll.Name = "btnSetToAll";
            this.btnSetToAll.Size = new System.Drawing.Size(215, 27);
            this.btnSetToAll.TabIndex = 10;
            this.btnSetToAll.Text = "All  &4";
            this.btnSetToAll.UseVisualStyleBackColor = true;
            this.btnSetToAll.Click += new System.EventHandler(this.btnSetToAll_Click);
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 225);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.tableLayoutPanel6.SetRowSpan(this.label8, 2);
            this.label8.Size = new System.Drawing.Size(189, 34);
            this.label8.TabIndex = 8;
            this.label8.Text = "Copy all Source Lang strings\r\nin selected String Set to:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(0, 18);
            this.label7.Margin = new System.Windows.Forms.Padding(0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(146, 17);
            this.label7.TabIndex = 0;
            this.label7.Text = "Selected String GUID:";
            // 
            // tbGUID
            // 
            this.tbGUID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbGUID.Location = new System.Drawing.Point(4, 41);
            this.tbGUID.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbGUID.Name = "tbGUID";
            this.tbGUID.ReadOnly = true;
            this.tbGUID.Size = new System.Drawing.Size(192, 22);
            this.tbGUID.TabIndex = 1;
            // 
            // label10
            // 
            this.label10.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(48, 142);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.tableLayoutPanel6.SetRowSpan(this.label10, 2);
            this.label10.Size = new System.Drawing.Size(148, 17);
            this.label10.TabIndex = 5;
            this.label10.Text = "Copy Source string to:";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnStringToTarget
            // 
            this.btnStringToTarget.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStringToTarget.Enabled = false;
            this.btnStringToTarget.Location = new System.Drawing.Point(204, 119);
            this.btnStringToTarget.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnStringToTarget.Name = "btnStringToTarget";
            this.btnStringToTarget.Size = new System.Drawing.Size(215, 28);
            this.btnStringToTarget.TabIndex = 6;
            this.btnStringToTarget.Text = "Target  &1";
            this.btnStringToTarget.UseVisualStyleBackColor = true;
            this.btnStringToTarget.Click += new System.EventHandler(this.btnStringToTarget_Click);
            // 
            // btnStringToAll
            // 
            this.btnStringToAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStringToAll.Enabled = false;
            this.btnStringToAll.Location = new System.Drawing.Point(204, 155);
            this.btnStringToAll.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnStringToAll.Name = "btnStringToAll";
            this.btnStringToAll.Size = new System.Drawing.Size(215, 28);
            this.btnStringToAll.TabIndex = 7;
            this.btnStringToAll.Text = "All  &2";
            this.btnStringToAll.UseVisualStyleBackColor = true;
            this.btnStringToAll.Click += new System.EventHandler(this.btnStringToAll_Click);
            // 
            // btnSetToTarget
            // 
            this.btnSetToTarget.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSetToTarget.AutoSize = true;
            this.btnSetToTarget.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSetToTarget.Enabled = false;
            this.btnSetToTarget.Location = new System.Drawing.Point(204, 211);
            this.btnSetToTarget.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSetToTarget.Name = "btnSetToTarget";
            this.btnSetToTarget.Size = new System.Drawing.Size(215, 27);
            this.btnSetToTarget.TabIndex = 9;
            this.btnSetToTarget.Text = "Target  &3";
            this.btnSetToTarget.UseVisualStyleBackColor = true;
            this.btnSetToTarget.Click += new System.EventHandler(this.btnSetToTarget_Click);
            // 
            // btnDelString
            // 
            this.btnDelString.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelString.AutoSize = true;
            this.btnDelString.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnDelString.Enabled = false;
            this.btnDelString.Location = new System.Drawing.Point(204, 4);
            this.btnDelString.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnDelString.Name = "btnDelString";
            this.btnDelString.Size = new System.Drawing.Size(215, 27);
            this.btnDelString.TabIndex = 2;
            this.btnDelString.Text = "&Remove string";
            this.btnDelString.UseVisualStyleBackColor = true;
            this.btnDelString.Click += new System.EventHandler(this.btnDelString_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel_SourceTarget, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel_SetPicking, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 2);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(425, 168);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // tableLayoutPanel_SourceTarget
            // 
            this.tableLayoutPanel_SourceTarget.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel_SourceTarget.AutoSize = true;
            this.tableLayoutPanel_SourceTarget.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel_SourceTarget.ColumnCount = 2;
            this.tableLayoutPanel_SourceTarget.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel_SourceTarget.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_SourceTarget.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel_SourceTarget.Controls.Add(this.cmbTargetLang, 1, 1);
            this.tableLayoutPanel_SourceTarget.Controls.Add(this.cmbSourceLang, 1, 0);
            this.tableLayoutPanel_SourceTarget.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel_SourceTarget.Location = new System.Drawing.Point(4, 100);
            this.tableLayoutPanel_SourceTarget.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tableLayoutPanel_SourceTarget.Name = "tableLayoutPanel_SourceTarget";
            this.tableLayoutPanel_SourceTarget.RowCount = 2;
            this.tableLayoutPanel_SourceTarget.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_SourceTarget.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_SourceTarget.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel_SourceTarget.Size = new System.Drawing.Size(377, 64);
            this.tableLayoutPanel_SourceTarget.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 39);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "&Target Lang:";
            // 
            // cmbTargetLang
            // 
            this.cmbTargetLang.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbTargetLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTargetLang.FormattingEnabled = true;
            this.cmbTargetLang.Location = new System.Drawing.Point(101, 36);
            this.cmbTargetLang.Margin = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.cmbTargetLang.Name = "cmbTargetLang";
            this.cmbTargetLang.Size = new System.Drawing.Size(276, 24);
            this.cmbTargetLang.TabIndex = 3;
            this.mainToolTip.SetToolTip(this.cmbTargetLang, "Select the language you want to create translations in");
            // 
            // cmbSourceLang
            // 
            this.cmbSourceLang.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSourceLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSourceLang.FormattingEnabled = true;
            this.cmbSourceLang.Location = new System.Drawing.Point(101, 4);
            this.cmbSourceLang.Margin = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.cmbSourceLang.Name = "cmbSourceLang";
            this.cmbSourceLang.Size = new System.Drawing.Size(276, 24);
            this.cmbSourceLang.TabIndex = 1;
            this.mainToolTip.SetToolTip(this.cmbSourceLang, "Select the language to use as comparison\r\nand base (where the target does not exi" +
        "st)");
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 7);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 17);
            this.label2.TabIndex = 0;
            this.label2.Text = "&Source Lang:";
            // 
            // tableLayoutPanel_SetPicking
            // 
            this.tableLayoutPanel_SetPicking.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel_SetPicking.AutoSize = true;
            this.tableLayoutPanel_SetPicking.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel_SetPicking.ColumnCount = 1;
            this.tableLayoutPanel_SetPicking.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_SetPicking.Controls.Add(this.label16, 0, 0);
            this.tableLayoutPanel_SetPicking.Controls.Add(this.cmbSetPicker, 0, 1);
            this.tableLayoutPanel_SetPicking.Location = new System.Drawing.Point(3, 2);
            this.tableLayoutPanel_SetPicking.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tableLayoutPanel_SetPicking.Name = "tableLayoutPanel_SetPicking";
            this.tableLayoutPanel_SetPicking.RowCount = 3;
            this.tableLayoutPanel_SetPicking.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_SetPicking.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_SetPicking.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_SetPicking.Size = new System.Drawing.Size(379, 92);
            this.tableLayoutPanel_SetPicking.TabIndex = 1;
            // 
            // label16
            // 
            this.label16.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(4, 10);
            this.label16.Margin = new System.Windows.Forms.Padding(4, 10, 4, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(117, 17);
            this.label16.TabIndex = 0;
            this.label16.Text = "Se&lect Strin&g Set:";
            // 
            // cmbSetPicker
            // 
            this.cmbSetPicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSetPicker.FormattingEnabled = true;
            this.cmbSetPicker.Location = new System.Drawing.Point(3, 29);
            this.cmbSetPicker.Margin = new System.Windows.Forms.Padding(3, 2, 3, 39);
            this.cmbSetPicker.Name = "cmbSetPicker";
            this.cmbSetPicker.Size = new System.Drawing.Size(373, 24);
            this.cmbSetPicker.TabIndex = 1;
            // 
            // tableLayoutPanel_RightInge
            // 
            this.tableLayoutPanel_RightInge.ColumnCount = 1;
            this.tableLayoutPanel_RightInge.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_RightInge.Controls.Add(this.splitContainer1, 0, 1);
            this.tableLayoutPanel_RightInge.Controls.Add(this.tlpFind, 0, 0);
            this.tableLayoutPanel_RightInge.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_RightInge.Location = new System.Drawing.Point(440, 3);
            this.tableLayoutPanel_RightInge.Name = "tableLayoutPanel_RightInge";
            this.tableLayoutPanel_RightInge.RowCount = 2;
            this.tableLayoutPanel_RightInge.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_RightInge.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_RightInge.Size = new System.Drawing.Size(744, 696);
            this.tableLayoutPanel_RightInge.TabIndex = 1;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(4, 97);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel9);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tlp_EditAndRef);
            this.splitContainer1.Size = new System.Drawing.Size(736, 595);
            this.splitContainer1.SplitterDistance = 313;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 1;
            // 
            // tableLayoutPanel9
            // 
            this.tableLayoutPanel9.ColumnCount = 1;
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel9.Controls.Add(this.lstStrings, 0, 1);
            this.tableLayoutPanel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel9.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel9.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tableLayoutPanel9.Name = "tableLayoutPanel9";
            this.tableLayoutPanel9.RowCount = 2;
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel9.Size = new System.Drawing.Size(736, 313);
            this.tableLayoutPanel9.TabIndex = 0;
            // 
            // lstStrings
            // 
            this.lstStrings.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chInstance,
            this.chSource,
            this.chTarget});
            this.lstStrings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstStrings.FullRowSelect = true;
            this.lstStrings.GridLines = true;
            this.lstStrings.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lstStrings.HideSelection = false;
            this.lstStrings.Location = new System.Drawing.Point(4, 4);
            this.lstStrings.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lstStrings.Name = "lstStrings";
            this.lstStrings.Size = new System.Drawing.Size(728, 305);
            this.lstStrings.TabIndex = 0;
            this.lstStrings.UseCompatibleStateImageBehavior = false;
            this.lstStrings.View = System.Windows.Forms.View.Details;
            // 
            // chInstance
            // 
            this.chInstance.Text = "String GUID";
            this.chInstance.Width = 164;
            // 
            // chSource
            // 
            this.chSource.Text = "Source String";
            this.chSource.Width = 203;
            // 
            // chTarget
            // 
            this.chTarget.Text = "Target String";
            this.chTarget.Width = 256;
            // 
            // tlp_EditAndRef
            // 
            this.tlp_EditAndRef.ColumnCount = 2;
            this.tlp_EditAndRef.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_EditAndRef.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlp_EditAndRef.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tlp_EditAndRef.Controls.Add(this.splitContainer2, 0, 0);
            this.tlp_EditAndRef.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_EditAndRef.Location = new System.Drawing.Point(0, 0);
            this.tlp_EditAndRef.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tlp_EditAndRef.Name = "tlp_EditAndRef";
            this.tlp_EditAndRef.RowCount = 1;
            this.tlp_EditAndRef.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_EditAndRef.Size = new System.Drawing.Size(736, 277);
            this.tlp_EditAndRef.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(4, 4);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.tlp_TargetEdit);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tlp_SourceRef);
            this.splitContainer2.Size = new System.Drawing.Size(728, 269);
            this.splitContainer2.SplitterDistance = 351;
            this.splitContainer2.SplitterWidth = 5;
            this.splitContainer2.TabIndex = 1;
            // 
            // tlp_SourceRef
            // 
            this.tlp_SourceRef.ColumnCount = 1;
            this.tlp_SourceRef.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_SourceRef.Controls.Add(this.label4, 0, 0);
            this.tlp_SourceRef.Controls.Add(this.txtSource, 0, 1);
            this.tlp_SourceRef.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_SourceRef.Location = new System.Drawing.Point(0, 0);
            this.tlp_SourceRef.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tlp_SourceRef.Name = "tlp_SourceRef";
            this.tlp_SourceRef.RowCount = 2;
            this.tlp_SourceRef.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlp_SourceRef.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_SourceRef.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlp_SourceRef.Size = new System.Drawing.Size(372, 269);
            this.tlp_SourceRef.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 0);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(161, 17);
            this.label4.TabIndex = 0;
            this.label4.Text = "Source string reference:";
            // 
            // txtSource
            // 
            this.txtSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSource.Location = new System.Drawing.Point(4, 21);
            this.txtSource.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtSource.Multiline = true;
            this.txtSource.Name = "txtSource";
            this.txtSource.ReadOnly = true;
            this.txtSource.Size = new System.Drawing.Size(364, 244);
            this.txtSource.TabIndex = 1;
            this.mainToolTip.SetToolTip(this.txtSource, "Source Text");
            // 
            // tlp_TargetEdit
            // 
            this.tlp_TargetEdit.ColumnCount = 1;
            this.tlp_TargetEdit.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_TargetEdit.Controls.Add(this.txtTarget, 0, 1);
            this.tlp_TargetEdit.Controls.Add(this.label3, 0, 0);
            this.tlp_TargetEdit.Controls.Add(this.tlp_CommitCancel, 0, 2);
            this.tlp_TargetEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_TargetEdit.Location = new System.Drawing.Point(0, 0);
            this.tlp_TargetEdit.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tlp_TargetEdit.Name = "tlp_TargetEdit";
            this.tlp_TargetEdit.RowCount = 3;
            this.tlp_TargetEdit.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlp_TargetEdit.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_TargetEdit.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlp_TargetEdit.Size = new System.Drawing.Size(351, 269);
            this.tlp_TargetEdit.TabIndex = 1;
            // 
            // txtTarget
            // 
            this.txtTarget.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTarget.Enabled = false;
            this.txtTarget.Location = new System.Drawing.Point(4, 21);
            this.txtTarget.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtTarget.Multiline = true;
            this.txtTarget.Name = "txtTarget";
            this.txtTarget.Size = new System.Drawing.Size(343, 203);
            this.txtTarget.TabIndex = 1;
            this.mainToolTip.SetToolTip(this.txtTarget, "Target Text");
            this.txtTarget.TextChanged += new System.EventHandler(this.txtTarget_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 0);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(116, 17);
            this.label3.TabIndex = 0;
            this.label3.Text = "Edit target string:";
            // 
            // tlpFind
            // 
            this.tlpFind.AutoSize = true;
            this.tlpFind.ColumnCount = 3;
            this.tlpFind.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpFind.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpFind.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpFind.Controls.Add(this.txtFind, 1, 0);
            this.tlpFind.Controls.Add(this.label5, 0, 0);
            this.tlpFind.Controls.Add(this.tableLayoutPanel4, 1, 1);
            this.tlpFind.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpFind.Enabled = false;
            this.tlpFind.Location = new System.Drawing.Point(4, 4);
            this.tlpFind.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tlpFind.Name = "tlpFind";
            this.tlpFind.RowCount = 3;
            this.tlpFind.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpFind.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpFind.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpFind.Size = new System.Drawing.Size(736, 85);
            this.tlpFind.TabIndex = 0;
            // 
            // txtFind
            // 
            this.txtFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFind.Location = new System.Drawing.Point(51, 4);
            this.txtFind.Margin = new System.Windows.Forms.Padding(4, 4, 40, 4);
            this.txtFind.Name = "txtFind";
            this.txtFind.Size = new System.Drawing.Size(645, 22);
            this.txtFind.TabIndex = 1;
            this.mainToolTip.SetToolTip(this.txtFind, "Text entered will be searched for in\r\nsource, target and instance number");
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 6);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(39, 17);
            this.label5.TabIndex = 0;
            this.label5.Text = "F&ind:";
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.AutoSize = true;
            this.tableLayoutPanel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel4.ColumnCount = 3;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.btnFindNext, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.btnFindFirst, 0, 0);
            this.tableLayoutPanel4.Location = new System.Drawing.Point(51, 34);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(105, 27);
            this.tableLayoutPanel4.TabIndex = 2;
            // 
            // btnFindNext
            // 
            this.btnFindNext.AutoSize = true;
            this.btnFindNext.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnFindNext.Location = new System.Drawing.Point(55, 0);
            this.btnFindNext.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.btnFindNext.Name = "btnFindNext";
            this.btnFindNext.Size = new System.Drawing.Size(46, 27);
            this.btnFindNext.TabIndex = 1;
            this.btnFindNext.Text = "&Next";
            this.btnFindNext.UseVisualStyleBackColor = true;
            this.btnFindNext.Click += new System.EventHandler(this.btnFindNext_Click);
            // 
            // btnFindFirst
            // 
            this.btnFindFirst.AutoSize = true;
            this.btnFindFirst.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnFindFirst.Location = new System.Drawing.Point(0, 0);
            this.btnFindFirst.Margin = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.btnFindFirst.Name = "btnFindFirst";
            this.btnFindFirst.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.btnFindFirst.Size = new System.Drawing.Size(47, 27);
            this.btnFindFirst.TabIndex = 0;
            this.btnFindFirst.Text = "Fin&d";
            this.btnFindFirst.UseVisualStyleBackColor = true;
            this.btnFindFirst.Click += new System.EventHandler(this.btnFindFirst_Click);
            // 
            // btnCommit
            // 
            this.btnCommit.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnCommit.AutoSize = true;
            this.btnCommit.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCommit.Enabled = false;
            this.btnCommit.Location = new System.Drawing.Point(121, 4);
            this.btnCommit.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCommit.Name = "btnCommit";
            this.btnCommit.Padding = new System.Windows.Forms.Padding(16, 0, 16, 0);
            this.btnCommit.Size = new System.Drawing.Size(96, 27);
            this.btnCommit.TabIndex = 3;
            this.btnCommit.Text = "&Commit";
            this.mainToolTip.SetToolTip(this.btnCommit, "Write changes to Target string");
            this.btnCommit.UseVisualStyleBackColor = true;
            // 
            // ckbAutoCommit
            // 
            this.ckbAutoCommit.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.ckbAutoCommit.AutoSize = true;
            this.ckbAutoCommit.Checked = true;
            this.ckbAutoCommit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbAutoCommit.Location = new System.Drawing.Point(4, 7);
            this.ckbAutoCommit.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ckbAutoCommit.Name = "ckbAutoCommit";
            this.ckbAutoCommit.Size = new System.Drawing.Size(109, 21);
            this.ckbAutoCommit.TabIndex = 4;
            this.ckbAutoCommit.Text = "A&uto Commit";
            this.ckbAutoCommit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mainToolTip.SetToolTip(this.ckbAutoCommit, "When unchecked, any changes made to the target text\r\nbox require \"Commit\" to be c" +
        "licked to write back to the\r\nTarget string itself.");
            this.ckbAutoCommit.UseVisualStyleBackColor = true;
            this.ckbAutoCommit.CheckedChanged += new System.EventHandler(this.ckbAutoCommit_CheckedChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.sTBLsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1187, 27);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.toolStripMenuItem1,
            this.savePackageToolStripMenuItem,
            this.savePackageAsToolStripMenuItem,
            this.toolStripMenuItem3,
            this.preferencesToolStripMenuItem,
            this.toolStripSeparator1,
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(47, 23);
            this.fileToolStripMenuItem.Text = "&File";
            this.fileToolStripMenuItem.DropDownOpening += new System.EventHandler(this.fileToolStripMenuItem_DropDownOpening);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(238, 24);
            this.openToolStripMenuItem.Text = "&Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(238, 24);
            this.closeToolStripMenuItem.Text = "&Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(235, 6);
            // 
            // savePackageToolStripMenuItem
            // 
            this.savePackageToolStripMenuItem.Name = "savePackageToolStripMenuItem";
            this.savePackageToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.savePackageToolStripMenuItem.Size = new System.Drawing.Size(238, 24);
            this.savePackageToolStripMenuItem.Text = "&Save Package";
            this.savePackageToolStripMenuItem.Click += new System.EventHandler(this.savePackageToolStripMenuItem_Click);
            // 
            // savePackageAsToolStripMenuItem
            // 
            this.savePackageAsToolStripMenuItem.Name = "savePackageAsToolStripMenuItem";
            this.savePackageAsToolStripMenuItem.Size = new System.Drawing.Size(238, 24);
            this.savePackageAsToolStripMenuItem.Text = "Save Package &As...";
            this.savePackageAsToolStripMenuItem.Click += new System.EventHandler(this.savePackageAsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(235, 6);
            // 
            // preferencesToolStripMenuItem
            // 
            this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            this.preferencesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(238, 24);
            this.preferencesToolStripMenuItem.Text = "&Preferences...";
            this.preferencesToolStripMenuItem.Click += new System.EventHandler(this.preferencesToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(235, 6);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(238, 24);
            this.quitToolStripMenuItem.Text = "&Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            // 
            // sTBLsToolStripMenuItem
            // 
            this.sTBLsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createNewSetToolStripMenuItem,
            this.deleteSetToolStripMenuItem,
            this.toolStripSeparator3,
            this.importFromPackageToolStripMenuItem,
            this.importFromSTBLFileToolStripMenuItem,
            this.exportToPackageToolStripMenuItem,
            this.exportToSTBLFileToolStripMenuItem,
            this.toolStripSeparator2,
            this.mergeAllSetsToolStripMenuItem});
            this.sTBLsToolStripMenuItem.Name = "sTBLsToolStripMenuItem";
            this.sTBLsToolStripMenuItem.Size = new System.Drawing.Size(69, 23);
            this.sTBLsToolStripMenuItem.Text = "STB&Ls";
            this.sTBLsToolStripMenuItem.DropDownOpening += new System.EventHandler(this.sTBLsToolStripMenuItem_DropDownOpening);
            // 
            // createNewSetToolStripMenuItem
            // 
            this.createNewSetToolStripMenuItem.Name = "createNewSetToolStripMenuItem";
            this.createNewSetToolStripMenuItem.Size = new System.Drawing.Size(300, 24);
            this.createNewSetToolStripMenuItem.Text = "&Create new set";
            // 
            // deleteSetToolStripMenuItem
            // 
            this.deleteSetToolStripMenuItem.Name = "deleteSetToolStripMenuItem";
            this.deleteSetToolStripMenuItem.Size = new System.Drawing.Size(300, 24);
            this.deleteSetToolStripMenuItem.Text = "&Delete selected set";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(297, 6);
            // 
            // importFromPackageToolStripMenuItem
            // 
            this.importFromPackageToolStripMenuItem.Name = "importFromPackageToolStripMenuItem";
            this.importFromPackageToolStripMenuItem.Size = new System.Drawing.Size(300, 24);
            this.importFromPackageToolStripMenuItem.Text = "&Import STBLs from package";
            this.importFromPackageToolStripMenuItem.Click += new System.EventHandler(this.importPackageToolStripMenuItem_Click);
            // 
            // importFromSTBLFileToolStripMenuItem
            // 
            this.importFromSTBLFileToolStripMenuItem.Name = "importFromSTBLFileToolStripMenuItem";
            this.importFromSTBLFileToolStripMenuItem.Size = new System.Drawing.Size(300, 24);
            this.importFromSTBLFileToolStripMenuItem.Text = "Import &from .STBL file/s";
            this.importFromSTBLFileToolStripMenuItem.Click += new System.EventHandler(this.importSTBLToolStripMenuItem_Click);
            // 
            // exportToPackageToolStripMenuItem
            // 
            this.exportToPackageToolStripMenuItem.Name = "exportToPackageToolStripMenuItem";
            this.exportToPackageToolStripMenuItem.Size = new System.Drawing.Size(300, 24);
            this.exportToPackageToolStripMenuItem.Text = "&Export selected set to package";
            this.exportToPackageToolStripMenuItem.Click += new System.EventHandler(this.exportToPackageToolStripMenuItem_Click);
            // 
            // exportToSTBLFileToolStripMenuItem
            // 
            this.exportToSTBLFileToolStripMenuItem.Name = "exportToSTBLFileToolStripMenuItem";
            this.exportToSTBLFileToolStripMenuItem.Size = new System.Drawing.Size(300, 24);
            this.exportToSTBLFileToolStripMenuItem.Text = "Export &to .STBL file/s";
            this.exportToSTBLFileToolStripMenuItem.Click += new System.EventHandler(this.exportLanguageToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(297, 6);
            // 
            // mergeAllSetsToolStripMenuItem
            // 
            this.mergeAllSetsToolStripMenuItem.Name = "mergeAllSetsToolStripMenuItem";
            this.mergeAllSetsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
            this.mergeAllSetsToolStripMenuItem.Size = new System.Drawing.Size(300, 24);
            this.mergeAllSetsToolStripMenuItem.Text = "&Merge all sets";
            this.mergeAllSetsToolStripMenuItem.Click += new System.EventHandler(this.btnMergeSets_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contentsToolStripMenuItem,
            this.licenceToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(53, 23);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // contentsToolStripMenuItem
            // 
            this.contentsToolStripMenuItem.Name = "contentsToolStripMenuItem";
            this.contentsToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.contentsToolStripMenuItem.Size = new System.Drawing.Size(185, 24);
            this.contentsToolStripMenuItem.Text = "&Contents...";
            this.contentsToolStripMenuItem.Click += new System.EventHandler(this.contentsToolStripMenuItem_Click);
            // 
            // licenceToolStripMenuItem
            // 
            this.licenceToolStripMenuItem.Name = "licenceToolStripMenuItem";
            this.licenceToolStripMenuItem.Size = new System.Drawing.Size(185, 24);
            this.licenceToolStripMenuItem.Text = "&Licence...";
            this.licenceToolStripMenuItem.Click += new System.EventHandler(this.licenceToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(185, 24);
            this.aboutToolStripMenuItem.Text = "&About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.prg});
            this.statusStrip1.Location = new System.Drawing.Point(0, 727);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1187, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // prg
            // 
            this.prg.Name = "prg";
            this.prg.Size = new System.Drawing.Size(133, 20);
            this.prg.Visible = false;
            // 
            // tlp_CommitCancel
            // 
            this.tlp_CommitCancel.AutoSize = true;
            this.tlp_CommitCancel.ColumnCount = 3;
            this.tlp_CommitCancel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlp_CommitCancel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlp_CommitCancel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlp_CommitCancel.Controls.Add(this.button_AbandonEdit, 2, 0);
            this.tlp_CommitCancel.Controls.Add(this.ckbAutoCommit, 0, 0);
            this.tlp_CommitCancel.Controls.Add(this.btnCommit, 1, 0);
            this.tlp_CommitCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.tlp_CommitCancel.Location = new System.Drawing.Point(12, 231);
            this.tlp_CommitCancel.Name = "tlp_CommitCancel";
            this.tlp_CommitCancel.RowCount = 1;
            this.tlp_CommitCancel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_CommitCancel.Size = new System.Drawing.Size(336, 35);
            this.tlp_CommitCancel.TabIndex = 2;
            // 
            // button_AbandonEdit
            // 
            this.button_AbandonEdit.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.button_AbandonEdit.AutoSize = true;
            this.button_AbandonEdit.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button_AbandonEdit.Enabled = false;
            this.button_AbandonEdit.Location = new System.Drawing.Point(225, 4);
            this.button_AbandonEdit.Margin = new System.Windows.Forms.Padding(4);
            this.button_AbandonEdit.Name = "button_AbandonEdit";
            this.button_AbandonEdit.Padding = new System.Windows.Forms.Padding(16, 0, 16, 0);
            this.button_AbandonEdit.Size = new System.Drawing.Size(107, 27);
            this.button_AbandonEdit.TabIndex = 4;
            this.button_AbandonEdit.Text = "A&bandon";
            this.mainToolTip.SetToolTip(this.button_AbandonEdit, "Write changes to Target string");
            this.button_AbandonEdit.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AcceptButton = this.btnCommit;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnSetToAll;
            this.ClientSize = new System.Drawing.Size(1187, 749);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStripContainer1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Form1";
            this.Text = "s3se";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.tableLayoutPanel_MainInge.ResumeLayout(false);
            this.tableLayoutPanel_MainInge.PerformLayout();
            this.tableLayoutPanel_LeftInge.ResumeLayout(false);
            this.tableLayoutPanel_LeftInge.PerformLayout();
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel_SourceTarget.ResumeLayout(false);
            this.tableLayoutPanel_SourceTarget.PerformLayout();
            this.tableLayoutPanel_SetPicking.ResumeLayout(false);
            this.tableLayoutPanel_SetPicking.PerformLayout();
            this.tableLayoutPanel_RightInge.ResumeLayout(false);
            this.tableLayoutPanel_RightInge.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel9.ResumeLayout(false);
            this.tlp_EditAndRef.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.tlp_SourceRef.ResumeLayout(false);
            this.tlp_SourceRef.PerformLayout();
            this.tlp_TargetEdit.ResumeLayout(false);
            this.tlp_TargetEdit.PerformLayout();
            this.tlpFind.ResumeLayout(false);
            this.tlpFind.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tlp_CommitCancel.ResumeLayout(false);
            this.tlp_CommitCancel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem savePackageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem savePackageAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbSourceLang;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbTargetLang;
        private System.Windows.Forms.ListView lstStrings;
        private System.Windows.Forms.ColumnHeader chTarget;
        private System.Windows.Forms.ColumnHeader chSource;
        private System.Windows.Forms.TextBox txtTarget;
        private System.Windows.Forms.TextBox txtSource;
        private System.Windows.Forms.ToolTip mainToolTip;
        private System.Windows.Forms.ColumnHeader chInstance;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox ckbAutoCommit;
        private System.Windows.Forms.Button btnCommit;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar prg;
        private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnFindNext;
        private System.Windows.Forms.TextBox txtFind;
        private System.Windows.Forms.Button btnFindFirst;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel9;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_SourceTarget;
        private System.Windows.Forms.TableLayoutPanel tlpFind;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TableLayoutPanel tlp_EditAndRef;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TableLayoutPanel tlp_SourceRef;
        private System.Windows.Forms.TableLayoutPanel tlp_TargetEdit;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem contentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem licenceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_MainInge;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_LeftInge;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_RightInge;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_SetPicking;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ComboBox cmbSetPicker;
        private System.Windows.Forms.ToolStripMenuItem sTBLsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createNewSetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteSetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importFromPackageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importFromSTBLFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToPackageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToSTBLFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem mergeAllSetsToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.Button btnAddString;
        private System.Windows.Forms.Button btnDelString;
        private System.Windows.Forms.Button btnSetToAll;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbGUID;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btnStringToTarget;
        private System.Windows.Forms.Button btnStringToAll;
        private System.Windows.Forms.Button btnSetToTarget;
        private System.Windows.Forms.Button btnChangeGUID;
        private System.Windows.Forms.TableLayoutPanel tlp_CommitCancel;
        private System.Windows.Forms.Button button_AbandonEdit;

    }
}

