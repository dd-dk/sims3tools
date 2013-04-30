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
            this.btnAddString = new System.Windows.Forms.Button();
            this.btnDelString = new System.Windows.Forms.Button();
            this.btnSetToAll = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtInstance = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label11 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.btnStringToTarget = new System.Windows.Forms.Button();
            this.btnStringToAll = new System.Windows.Forms.Button();
            this.btnSetToTarget = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.tableLayoutPanel10 = new System.Windows.Forms.TableLayoutPanel();
            this.label6 = new System.Windows.Forms.Label();
            this.lstSTBLs = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label9 = new System.Windows.Forms.Label();
            this.btnMergeSets = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbTargetLang = new System.Windows.Forms.ComboBox();
            this.cmbSourceLang = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel_SetPicking = new System.Windows.Forms.TableLayoutPanel();
            this.label16 = new System.Windows.Forms.Label();
            this.combobox_StringSet = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel_RightInge = new System.Windows.Forms.TableLayoutPanel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel9 = new System.Windows.Forms.TableLayoutPanel();
            this.lstStrings = new System.Windows.Forms.ListView();
            this.chSource = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chTarget = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chInstance = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.txtSource = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
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
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.importPackageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importSTBLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportLanguageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.tableLayoutPanel_MainInge.SuspendLayout();
            this.tableLayoutPanel_LeftInge.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.tableLayoutPanel10.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel_SetPicking.SuspendLayout();
            this.tableLayoutPanel_RightInge.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel9.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            this.tlpFind.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
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
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(905, 692);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // toolStripContainer1.LeftToolStripPanel
            // 
            this.toolStripContainer1.LeftToolStripPanel.Enabled = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            // 
            // toolStripContainer1.RightToolStripPanel
            // 
            this.toolStripContainer1.RightToolStripPanel.Enabled = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(905, 716);
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
            this.tableLayoutPanel_MainInge.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel_MainInge.Name = "tableLayoutPanel_MainInge";
            this.tableLayoutPanel_MainInge.RowCount = 1;
            this.tableLayoutPanel_MainInge.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_MainInge.Size = new System.Drawing.Size(905, 692);
            this.tableLayoutPanel_MainInge.TabIndex = 1;
            // 
            // tableLayoutPanel_LeftInge
            // 
            this.tableLayoutPanel_LeftInge.AutoSize = true;
            this.tableLayoutPanel_LeftInge.ColumnCount = 1;
            this.tableLayoutPanel_LeftInge.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_LeftInge.Controls.Add(this.tableLayoutPanel6, 0, 3);
            this.tableLayoutPanel_LeftInge.Controls.Add(this.tableLayoutPanel10, 0, 1);
            this.tableLayoutPanel_LeftInge.Controls.Add(this.tableLayoutPanel1, 0, 2);
            this.tableLayoutPanel_LeftInge.Controls.Add(this.tableLayoutPanel_SetPicking, 0, 0);
            this.tableLayoutPanel_LeftInge.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_LeftInge.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutPanel_LeftInge.Margin = new System.Windows.Forms.Padding(2, 2, 2, 24);
            this.tableLayoutPanel_LeftInge.Name = "tableLayoutPanel_LeftInge";
            this.tableLayoutPanel_LeftInge.RowCount = 5;
            this.tableLayoutPanel_LeftInge.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 49F));
            this.tableLayoutPanel_LeftInge.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_LeftInge.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_LeftInge.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_LeftInge.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_LeftInge.Size = new System.Drawing.Size(1078, 666);
            this.tableLayoutPanel_LeftInge.TabIndex = 0;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.AutoSize = true;
            this.tableLayoutPanel6.ColumnCount = 2;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Controls.Add(this.btnAddString, 1, 2);
            this.tableLayoutPanel6.Controls.Add(this.btnDelString, 1, 1);
            this.tableLayoutPanel6.Controls.Add(this.btnSetToAll, 1, 8);
            this.tableLayoutPanel6.Controls.Add(this.label8, 0, 7);
            this.tableLayoutPanel6.Controls.Add(this.label7, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.txtInstance, 0, 1);
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel3, 0, 2);
            this.tableLayoutPanel6.Controls.Add(this.label11, 1, 3);
            this.tableLayoutPanel6.Controls.Add(this.label15, 1, 10);
            this.tableLayoutPanel6.Controls.Add(this.label10, 0, 4);
            this.tableLayoutPanel6.Controls.Add(this.btnStringToTarget, 1, 4);
            this.tableLayoutPanel6.Controls.Add(this.btnStringToAll, 1, 5);
            this.tableLayoutPanel6.Controls.Add(this.btnSetToTarget, 1, 7);
            this.tableLayoutPanel6.Controls.Add(this.label13, 1, 6);
            this.tableLayoutPanel6.Controls.Add(this.label12, 1, 9);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(3, 318);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 11;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(1072, 275);
            this.tableLayoutPanel6.TabIndex = 2;
            // 
            // btnAddString
            // 
            this.btnAddString.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddString.AutoSize = true;
            this.btnAddString.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAddString.Enabled = false;
            this.btnAddString.Location = new System.Drawing.Point(152, 45);
            this.btnAddString.Name = "btnAddString";
            this.btnAddString.Size = new System.Drawing.Size(917, 23);
            this.btnAddString.TabIndex = 1;
            this.btnAddString.Text = "Add string...";
            this.btnAddString.UseVisualStyleBackColor = true;
            this.btnAddString.Click += new System.EventHandler(this.btnAddString_Click);
            // 
            // btnDelString
            // 
            this.btnDelString.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelString.AutoSize = true;
            this.btnDelString.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnDelString.Enabled = false;
            this.btnDelString.Location = new System.Drawing.Point(152, 16);
            this.btnDelString.Name = "btnDelString";
            this.btnDelString.Size = new System.Drawing.Size(917, 23);
            this.btnDelString.TabIndex = 0;
            this.btnDelString.Text = "Delete string";
            this.btnDelString.UseVisualStyleBackColor = true;
            this.btnDelString.Click += new System.EventHandler(this.btnDelString_Click);
            // 
            // btnSetToAll
            // 
            this.btnSetToAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSetToAll.AutoSize = true;
            this.btnSetToAll.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSetToAll.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnSetToAll.Enabled = false;
            this.btnSetToAll.Location = new System.Drawing.Point(152, 201);
            this.btnSetToAll.Name = "btnSetToAll";
            this.btnSetToAll.Size = new System.Drawing.Size(917, 23);
            this.btnSetToAll.TabIndex = 2;
            this.btnSetToAll.Text = "All";
            this.btnSetToAll.UseVisualStyleBackColor = true;
            this.btnSetToAll.Click += new System.EventHandler(this.btnSetToAll_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label8.Location = new System.Drawing.Point(3, 169);
            this.label8.Name = "label8";
            this.tableLayoutPanel6.SetRowSpan(this.label8, 2);
            this.label8.Size = new System.Drawing.Size(143, 58);
            this.label8.TabIndex = 0;
            this.label8.Text = "Copy all Source Lang strings\r\nin selected String Set to:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(51, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Instance:";
            // 
            // txtInstance
            // 
            this.txtInstance.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtInstance.Location = new System.Drawing.Point(3, 16);
            this.txtInstance.MinimumSize = new System.Drawing.Size(4, 25);
            this.txtInstance.Name = "txtInstance";
            this.txtInstance.ReadOnly = true;
            this.txtInstance.Size = new System.Drawing.Size(143, 20);
            this.txtInstance.TabIndex = 1;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 56);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(149, 0);
            this.tableLayoutPanel3.TabIndex = 2;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(151, 71);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.MinimumSize = new System.Drawing.Size(22, 24);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(22, 24);
            this.label11.TabIndex = 8;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(151, 251);
            this.label15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label15.MinimumSize = new System.Drawing.Size(22, 24);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(22, 24);
            this.label15.TabIndex = 12;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label10.Location = new System.Drawing.Point(3, 91);
            this.label10.Name = "label10";
            this.tableLayoutPanel6.SetRowSpan(this.label10, 2);
            this.label10.Size = new System.Drawing.Size(143, 58);
            this.label10.TabIndex = 5;
            this.label10.Text = "Copy Source string to:";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnStringToTarget
            // 
            this.btnStringToTarget.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStringToTarget.Enabled = false;
            this.btnStringToTarget.Location = new System.Drawing.Point(152, 94);
            this.btnStringToTarget.Name = "btnStringToTarget";
            this.btnStringToTarget.Size = new System.Drawing.Size(917, 23);
            this.btnStringToTarget.TabIndex = 6;
            this.btnStringToTarget.Text = "Target";
            this.btnStringToTarget.UseVisualStyleBackColor = true;
            this.btnStringToTarget.Click += new System.EventHandler(this.btnStringToTarget_Click);
            // 
            // btnStringToAll
            // 
            this.btnStringToAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStringToAll.Enabled = false;
            this.btnStringToAll.Location = new System.Drawing.Point(152, 123);
            this.btnStringToAll.Name = "btnStringToAll";
            this.btnStringToAll.Size = new System.Drawing.Size(917, 23);
            this.btnStringToAll.TabIndex = 7;
            this.btnStringToAll.Text = "All";
            this.btnStringToAll.UseVisualStyleBackColor = true;
            this.btnStringToAll.Click += new System.EventHandler(this.btnStringToAll_Click);
            // 
            // btnSetToTarget
            // 
            this.btnSetToTarget.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSetToTarget.AutoSize = true;
            this.btnSetToTarget.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSetToTarget.Enabled = false;
            this.btnSetToTarget.Location = new System.Drawing.Point(152, 172);
            this.btnSetToTarget.Name = "btnSetToTarget";
            this.btnSetToTarget.Size = new System.Drawing.Size(917, 23);
            this.btnSetToTarget.TabIndex = 1;
            this.btnSetToTarget.Text = "Ta&rget";
            this.btnSetToTarget.UseVisualStyleBackColor = true;
            this.btnSetToTarget.Click += new System.EventHandler(this.btnSetToTarget_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(151, 149);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label13.MinimumSize = new System.Drawing.Size(22, 24);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(22, 24);
            this.label13.TabIndex = 10;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(151, 227);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label12.MinimumSize = new System.Drawing.Size(22, 24);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(22, 24);
            this.label12.TabIndex = 9;
            // 
            // tableLayoutPanel10
            // 
            this.tableLayoutPanel10.AutoSize = true;
            this.tableLayoutPanel10.ColumnCount = 2;
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel10.Controls.Add(this.label6, 0, 0);
            this.tableLayoutPanel10.Controls.Add(this.lstSTBLs, 0, 1);
            this.tableLayoutPanel10.Controls.Add(this.tableLayoutPanel2, 0, 2);
            this.tableLayoutPanel10.Controls.Add(this.label14, 0, 4);
            this.tableLayoutPanel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel10.Location = new System.Drawing.Point(3, 52);
            this.tableLayoutPanel10.Margin = new System.Windows.Forms.Padding(3, 3, 3, 24);
            this.tableLayoutPanel10.MaximumSize = new System.Drawing.Size(0, 162);
            this.tableLayoutPanel10.Name = "tableLayoutPanel10";
            this.tableLayoutPanel10.RowCount = 5;
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel10.Size = new System.Drawing.Size(1072, 162);
            this.tableLayoutPanel10.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(114, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Se&lect String Set: OLD";
            // 
            // lstSTBLs
            // 
            this.lstSTBLs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.tableLayoutPanel10.SetColumnSpan(this.lstSTBLs, 2);
            this.lstSTBLs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstSTBLs.FullRowSelect = true;
            this.lstSTBLs.GridLines = true;
            this.lstSTBLs.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lstSTBLs.HideSelection = false;
            this.lstSTBLs.Location = new System.Drawing.Point(3, 16);
            this.lstSTBLs.MultiSelect = false;
            this.lstSTBLs.Name = "lstSTBLs";
            this.lstSTBLs.Size = new System.Drawing.Size(1066, 86);
            this.lstSTBLs.TabIndex = 1;
            this.mainToolTip.SetToolTip(this.lstSTBLs, "Where there are multiple STBL resources\r\nin the package, they will be listed here" +
        "\r\n(omitting the language identifier byte)");
            this.lstSTBLs.UseCompatibleStateImageBehavior = false;
            this.lstSTBLs.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Instance";
            this.columnHeader1.Width = 187;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel10.SetColumnSpan(this.tableLayoutPanel2, 2);
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.label9, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.btnMergeSets, 1, 3);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 108);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1066, 37);
            this.tableLayoutPanel2.TabIndex = 3;
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 9);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(115, 26);
            this.label9.TabIndex = 3;
            this.label9.Text = "Combine all String Sets\r\ninto a single Set:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnMergeSets
            // 
            this.btnMergeSets.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMergeSets.AutoSize = true;
            this.btnMergeSets.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnMergeSets.Enabled = false;
            this.btnMergeSets.Location = new System.Drawing.Point(124, 11);
            this.btnMergeSets.Name = "btnMergeSets";
            this.btnMergeSets.Size = new System.Drawing.Size(939, 23);
            this.btnMergeSets.TabIndex = 4;
            this.btnMergeSets.Text = "Mer&ge";
            this.mainToolTip.SetToolTip(this.btnMergeSets, "Where there are multiple STBL resources\r\nin the package, they will be merged into" +
        " a\r\nsingle new resource\r\n");
            this.btnMergeSets.UseVisualStyleBackColor = true;
            this.btnMergeSets.Click += new System.EventHandler(this.btnMergeSets_Click);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tableLayoutPanel10.SetColumnSpan(this.label14, 2);
            this.label14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label14.Location = new System.Drawing.Point(2, 160);
            this.label14.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label14.MaximumSize = new System.Drawing.Size(0, 2);
            this.label14.MinimumSize = new System.Drawing.Size(75, 2);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(1068, 2);
            this.label14.TabIndex = 4;
            this.label14.Text = "label14";
            this.label14.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.cmbTargetLang, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.cmbSourceLang, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 241);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 3, 3, 20);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1072, 54);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "&Target Lang:";
            // 
            // cmbTargetLang
            // 
            this.cmbTargetLang.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbTargetLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTargetLang.FormattingEnabled = true;
            this.cmbTargetLang.Location = new System.Drawing.Point(77, 30);
            this.cmbTargetLang.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.cmbTargetLang.Name = "cmbTargetLang";
            this.cmbTargetLang.Size = new System.Drawing.Size(135, 21);
            this.cmbTargetLang.TabIndex = 3;
            this.mainToolTip.SetToolTip(this.cmbTargetLang, "Select the language you want to create translations in");
            // 
            // cmbSourceLang
            // 
            this.cmbSourceLang.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSourceLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSourceLang.FormattingEnabled = true;
            this.cmbSourceLang.Location = new System.Drawing.Point(77, 3);
            this.cmbSourceLang.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.cmbSourceLang.Name = "cmbSourceLang";
            this.cmbSourceLang.Size = new System.Drawing.Size(135, 21);
            this.cmbSourceLang.TabIndex = 1;
            this.mainToolTip.SetToolTip(this.cmbSourceLang, "Select the language to use as comparison\r\nand base (where the target does not exi" +
        "st)");
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "&Source Lang:";
            // 
            // tableLayoutPanel_SetPicking
            // 
            this.tableLayoutPanel_SetPicking.ColumnCount = 1;
            this.tableLayoutPanel_SetPicking.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_SetPicking.Controls.Add(this.label16, 0, 0);
            this.tableLayoutPanel_SetPicking.Controls.Add(this.combobox_StringSet, 0, 1);
            this.tableLayoutPanel_SetPicking.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_SetPicking.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutPanel_SetPicking.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel_SetPicking.Name = "tableLayoutPanel_SetPicking";
            this.tableLayoutPanel_SetPicking.RowCount = 2;
            this.tableLayoutPanel_SetPicking.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_SetPicking.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_SetPicking.Size = new System.Drawing.Size(1074, 45);
            this.tableLayoutPanel_SetPicking.TabIndex = 3;
            // 
            // label16
            // 
            this.label16.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(3, 4);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(89, 13);
            this.label16.TabIndex = 1;
            this.label16.Text = "Se&lect String Set:";
            // 
            // combobox_StringSet
            // 
            this.combobox_StringSet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.combobox_StringSet.FormattingEnabled = true;
            this.combobox_StringSet.Location = new System.Drawing.Point(2, 24);
            this.combobox_StringSet.Margin = new System.Windows.Forms.Padding(2);
            this.combobox_StringSet.Name = "combobox_StringSet";
            this.combobox_StringSet.Size = new System.Drawing.Size(1070, 21);
            this.combobox_StringSet.TabIndex = 0;
            // 
            // tableLayoutPanel_RightInge
            // 
            this.tableLayoutPanel_RightInge.ColumnCount = 1;
            this.tableLayoutPanel_RightInge.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_RightInge.Controls.Add(this.splitContainer1, 0, 1);
            this.tableLayoutPanel_RightInge.Controls.Add(this.tlpFind, 0, 0);
            this.tableLayoutPanel_RightInge.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_RightInge.Location = new System.Drawing.Point(1084, 2);
            this.tableLayoutPanel_RightInge.Margin = new System.Windows.Forms.Padding(2, 2, 2, 24);
            this.tableLayoutPanel_RightInge.Name = "tableLayoutPanel_RightInge";
            this.tableLayoutPanel_RightInge.RowCount = 2;
            this.tableLayoutPanel_RightInge.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_RightInge.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_RightInge.Size = new System.Drawing.Size(1, 666);
            this.tableLayoutPanel_RightInge.TabIndex = 1;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 83);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel9);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel5);
            this.splitContainer1.Size = new System.Drawing.Size(1, 580);
            this.splitContainer1.SplitterDistance = 309;
            this.splitContainer1.TabIndex = 0;
            // 
            // tableLayoutPanel9
            // 
            this.tableLayoutPanel9.ColumnCount = 1;
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tableLayoutPanel9.Controls.Add(this.lstStrings, 0, 1);
            this.tableLayoutPanel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel9.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel9.Name = "tableLayoutPanel9";
            this.tableLayoutPanel9.RowCount = 2;
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel9.Size = new System.Drawing.Size(1, 309);
            this.tableLayoutPanel9.TabIndex = 0;
            // 
            // lstStrings
            // 
            this.lstStrings.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chSource,
            this.chTarget,
            this.chInstance});
            this.lstStrings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstStrings.FullRowSelect = true;
            this.lstStrings.GridLines = true;
            this.lstStrings.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lstStrings.HideSelection = false;
            this.lstStrings.Location = new System.Drawing.Point(3, 3);
            this.lstStrings.Name = "lstStrings";
            this.lstStrings.Size = new System.Drawing.Size(1, 303);
            this.lstStrings.TabIndex = 3;
            this.lstStrings.UseCompatibleStateImageBehavior = false;
            this.lstStrings.View = System.Windows.Forms.View.Details;
            // 
            // chSource
            // 
            this.chSource.Text = "Source";
            this.chSource.Width = 256;
            // 
            // chTarget
            // 
            this.chTarget.Text = "Target";
            this.chTarget.Width = 252;
            // 
            // chInstance
            // 
            this.chInstance.Text = "Instance";
            this.chInstance.Width = 141;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel5.Controls.Add(this.splitContainer2, 0, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(1, 267);
            this.tableLayoutPanel5.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.tableLayoutPanel7);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tableLayoutPanel8);
            this.splitContainer2.Size = new System.Drawing.Size(1, 261);
            this.splitContainer2.SplitterDistance = 108;
            this.splitContainer2.TabIndex = 0;
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.ColumnCount = 1;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.Controls.Add(this.label4, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.txtSource, 0, 1);
            this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel7.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 2;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(108, 261);
            this.tableLayoutPanel7.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 26);
            this.label4.TabIndex = 0;
            this.label4.Text = "Source string reference:";
            // 
            // txtSource
            // 
            this.txtSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSource.Location = new System.Drawing.Point(3, 29);
            this.txtSource.Multiline = true;
            this.txtSource.Name = "txtSource";
            this.txtSource.ReadOnly = true;
            this.txtSource.Size = new System.Drawing.Size(102, 229);
            this.txtSource.TabIndex = 1;
            this.mainToolTip.SetToolTip(this.txtSource, "Source Text");
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.ColumnCount = 1;
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel8.Controls.Add(this.txtTarget, 0, 1);
            this.tableLayoutPanel8.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel8.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 2;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel8.Size = new System.Drawing.Size(111, 261);
            this.tableLayoutPanel8.TabIndex = 1;
            // 
            // txtTarget
            // 
            this.txtTarget.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTarget.Enabled = false;
            this.txtTarget.Location = new System.Drawing.Point(3, 16);
            this.txtTarget.Multiline = true;
            this.txtTarget.Name = "txtTarget";
            this.txtTarget.Size = new System.Drawing.Size(105, 242);
            this.txtTarget.TabIndex = 1;
            this.mainToolTip.SetToolTip(this.txtTarget, "Target Text");
            this.txtTarget.TextChanged += new System.EventHandler(this.txtTarget_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 13);
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
            this.tlpFind.Controls.Add(this.btnCommit, 2, 0);
            this.tlpFind.Controls.Add(this.ckbAutoCommit, 2, 1);
            this.tlpFind.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpFind.Enabled = false;
            this.tlpFind.Location = new System.Drawing.Point(3, 3);
            this.tlpFind.Name = "tlpFind";
            this.tlpFind.RowCount = 3;
            this.tlpFind.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpFind.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpFind.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tlpFind.Size = new System.Drawing.Size(1, 74);
            this.tlpFind.TabIndex = 2;
            // 
            // txtFind
            // 
            this.txtFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFind.Location = new System.Drawing.Point(39, 4);
            this.txtFind.Margin = new System.Windows.Forms.Padding(3, 3, 30, 3);
            this.txtFind.Name = "txtFind";
            this.txtFind.Size = new System.Drawing.Size(1, 20);
            this.txtFind.TabIndex = 1;
            this.mainToolTip.SetToolTip(this.txtFind, "Text entered will be searched for in\r\nsource, target and instance number");
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 8);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(30, 13);
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
            this.tableLayoutPanel4.Location = new System.Drawing.Point(39, 32);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(1, 23);
            this.tableLayoutPanel4.TabIndex = 2;
            // 
            // btnFindNext
            // 
            this.btnFindNext.AutoSize = true;
            this.btnFindNext.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnFindNext.Location = new System.Drawing.Point(45, 0);
            this.btnFindNext.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnFindNext.Name = "btnFindNext";
            this.btnFindNext.Size = new System.Drawing.Size(39, 23);
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
            this.btnFindFirst.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.btnFindFirst.Name = "btnFindFirst";
            this.btnFindFirst.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.btnFindFirst.Size = new System.Drawing.Size(39, 23);
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
            this.btnCommit.Location = new System.Drawing.Point(-77, 3);
            this.btnCommit.Name = "btnCommit";
            this.btnCommit.Padding = new System.Windows.Forms.Padding(12, 0, 12, 0);
            this.btnCommit.Size = new System.Drawing.Size(75, 23);
            this.btnCommit.TabIndex = 4;
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
            this.ckbAutoCommit.Location = new System.Drawing.Point(-87, 35);
            this.ckbAutoCommit.Name = "ckbAutoCommit";
            this.ckbAutoCommit.Size = new System.Drawing.Size(85, 17);
            this.ckbAutoCommit.TabIndex = 3;
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
            this.menuStrip1.Size = new System.Drawing.Size(905, 24);
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
            this.toolStripMenuItem2,
            this.importPackageToolStripMenuItem,
            this.importSTBLToolStripMenuItem,
            this.exportLanguageToolStripMenuItem,
            this.toolStripMenuItem3,
            this.preferencesToolStripMenuItem,
            this.toolStripSeparator1,
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "&File";
            this.fileToolStripMenuItem.DropDownOpening += new System.EventHandler(this.fileToolStripMenuItem_DropDownOpening);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.openToolStripMenuItem.Text = "&Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.closeToolStripMenuItem.Text = "&Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(179, 6);
            // 
            // savePackageToolStripMenuItem
            // 
            this.savePackageToolStripMenuItem.Name = "savePackageToolStripMenuItem";
            this.savePackageToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.savePackageToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.savePackageToolStripMenuItem.Text = "&Save Package";
            this.savePackageToolStripMenuItem.Click += new System.EventHandler(this.savePackageToolStripMenuItem_Click);
            // 
            // savePackageAsToolStripMenuItem
            // 
            this.savePackageAsToolStripMenuItem.Name = "savePackageAsToolStripMenuItem";
            this.savePackageAsToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.savePackageAsToolStripMenuItem.Text = "Save Package &As...";
            this.savePackageAsToolStripMenuItem.Click += new System.EventHandler(this.savePackageAsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(179, 6);
            // 
            // importPackageToolStripMenuItem
            // 
            this.importPackageToolStripMenuItem.Name = "importPackageToolStripMenuItem";
            this.importPackageToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.importPackageToolStripMenuItem.Text = "&Import Package...";
            this.importPackageToolStripMenuItem.Click += new System.EventHandler(this.importPackageToolStripMenuItem_Click);
            // 
            // importSTBLToolStripMenuItem
            // 
            this.importSTBLToolStripMenuItem.Name = "importSTBLToolStripMenuItem";
            this.importSTBLToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.importSTBLToolStripMenuItem.Text = "I&mport STBL...";
            this.importSTBLToolStripMenuItem.Click += new System.EventHandler(this.importSTBLToolStripMenuItem_Click);
            // 
            // exportLanguageToolStripMenuItem
            // 
            this.exportLanguageToolStripMenuItem.Name = "exportLanguageToolStripMenuItem";
            this.exportLanguageToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.exportLanguageToolStripMenuItem.Text = "&Export Language...";
            this.exportLanguageToolStripMenuItem.Click += new System.EventHandler(this.exportLanguageToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(179, 6);
            // 
            // preferencesToolStripMenuItem
            // 
            this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            this.preferencesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.preferencesToolStripMenuItem.Text = "&Preferences...";
            this.preferencesToolStripMenuItem.Click += new System.EventHandler(this.preferencesToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(179, 6);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
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
            this.sTBLsToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.sTBLsToolStripMenuItem.Text = "STBLs";
            // 
            // createNewSetToolStripMenuItem
            // 
            this.createNewSetToolStripMenuItem.Name = "createNewSetToolStripMenuItem";
            this.createNewSetToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.createNewSetToolStripMenuItem.Text = "Create new set";
            // 
            // deleteSetToolStripMenuItem
            // 
            this.deleteSetToolStripMenuItem.Name = "deleteSetToolStripMenuItem";
            this.deleteSetToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.deleteSetToolStripMenuItem.Text = "Delete selected set";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(220, 6);
            // 
            // importFromPackageToolStripMenuItem
            // 
            this.importFromPackageToolStripMenuItem.Name = "importFromPackageToolStripMenuItem";
            this.importFromPackageToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.importFromPackageToolStripMenuItem.Text = "Import STBLs from package";
            this.importFromPackageToolStripMenuItem.Click += new System.EventHandler(this.importPackageToolStripMenuItem_Click);
            // 
            // importFromSTBLFileToolStripMenuItem
            // 
            this.importFromSTBLFileToolStripMenuItem.Name = "importFromSTBLFileToolStripMenuItem";
            this.importFromSTBLFileToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.importFromSTBLFileToolStripMenuItem.Text = "Import from .STBL file/s";
            this.importFromSTBLFileToolStripMenuItem.Click += new System.EventHandler(this.importSTBLToolStripMenuItem_Click);
            // 
            // exportToPackageToolStripMenuItem
            // 
            this.exportToPackageToolStripMenuItem.Name = "exportToPackageToolStripMenuItem";
            this.exportToPackageToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.exportToPackageToolStripMenuItem.Text = "Export selected set to package";
            // 
            // exportToSTBLFileToolStripMenuItem
            // 
            this.exportToSTBLFileToolStripMenuItem.Name = "exportToSTBLFileToolStripMenuItem";
            this.exportToSTBLFileToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.exportToSTBLFileToolStripMenuItem.Text = "Export to .STBL file/s";
            this.exportToSTBLFileToolStripMenuItem.Click += new System.EventHandler(this.exportLanguageToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(220, 6);
            // 
            // mergeAllSetsToolStripMenuItem
            // 
            this.mergeAllSetsToolStripMenuItem.Enabled = false;
            this.mergeAllSetsToolStripMenuItem.Name = "mergeAllSetsToolStripMenuItem";
            this.mergeAllSetsToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.mergeAllSetsToolStripMenuItem.Text = "Merge all sets";
            this.mergeAllSetsToolStripMenuItem.Click += new System.EventHandler(this.btnMergeSets_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contentsToolStripMenuItem,
            this.licenceToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // contentsToolStripMenuItem
            // 
            this.contentsToolStripMenuItem.Name = "contentsToolStripMenuItem";
            this.contentsToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.contentsToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.contentsToolStripMenuItem.Text = "&Contents...";
            this.contentsToolStripMenuItem.Click += new System.EventHandler(this.contentsToolStripMenuItem_Click);
            // 
            // licenceToolStripMenuItem
            // 
            this.licenceToolStripMenuItem.Name = "licenceToolStripMenuItem";
            this.licenceToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.licenceToolStripMenuItem.Text = "&Licence...";
            this.licenceToolStripMenuItem.Click += new System.EventHandler(this.licenceToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.aboutToolStripMenuItem.Text = "&About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.prg});
            this.statusStrip1.Location = new System.Drawing.Point(0, 694);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(905, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // prg
            // 
            this.prg.Name = "prg";
            this.prg.Size = new System.Drawing.Size(100, 16);
            this.prg.Visible = false;
            // 
            // Form1
            // 
            this.AcceptButton = this.btnCommit;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnSetToAll;
            this.ClientSize = new System.Drawing.Size(905, 716);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStripContainer1);
            this.Name = "Form1";
            this.Text = "Sims 3 Translate";
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
            this.tableLayoutPanel10.ResumeLayout(false);
            this.tableLayoutPanel10.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel_SetPicking.ResumeLayout(false);
            this.tableLayoutPanel_SetPicking.PerformLayout();
            this.tableLayoutPanel_RightInge.ResumeLayout(false);
            this.tableLayoutPanel_RightInge.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel9.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel7.PerformLayout();
            this.tableLayoutPanel8.ResumeLayout(false);
            this.tableLayoutPanel8.PerformLayout();
            this.tlpFind.ResumeLayout(false);
            this.tlpFind.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
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
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem exportLanguageToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView lstSTBLs;
        private System.Windows.Forms.ColumnHeader columnHeader1;
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
        private System.Windows.Forms.ToolStripMenuItem importPackageToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar prg;
        private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Button btnSetToAll;
        private System.Windows.Forms.Button btnSetToTarget;
        private System.Windows.Forms.Button btnMergeSets;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnFindNext;
        private System.Windows.Forms.TextBox txtFind;
        private System.Windows.Forms.Button btnFindFirst;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel9;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tlpFind;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel10;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtInstance;
        private System.Windows.Forms.Button btnDelString;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button btnAddString;
        private System.Windows.Forms.ToolStripMenuItem importSTBLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem contentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem licenceToolStripMenuItem;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Button btnStringToTarget;
        private System.Windows.Forms.Button btnStringToAll;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_MainInge;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_LeftInge;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_RightInge;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_SetPicking;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ComboBox combobox_StringSet;
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

    }
}

