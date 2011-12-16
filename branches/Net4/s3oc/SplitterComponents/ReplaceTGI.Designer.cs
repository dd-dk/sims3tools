namespace ObjectCloner.SplitterComponents
{
    partial class ReplaceTGI
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
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tlpFromTGIValues = new System.Windows.Forms.TableLayoutPanel();
            this.rtContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.rtgiCopyRK = new System.Windows.Forms.ToolStripMenuItem();
            this.rtgiPasteRK = new System.Windows.Forms.ToolStripMenuItem();
            this.label3 = new System.Windows.Forms.Label();
            this.tbFromInstance = new System.Windows.Forms.TextBox();
            this.tbFromResourceGroup = new System.Windows.Forms.TextBox();
            this.cbFromResourceType = new System.Windows.Forms.ResourceTypeCombo();
            this.ckbAnyResourceType = new System.Windows.Forms.CheckBox();
            this.ckbAnyResourceGroup = new System.Windows.Forms.CheckBox();
            this.ckbAnyInstance = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tlpToTGIValues = new System.Windows.Forms.TableLayoutPanel();
            this.label6 = new System.Windows.Forms.Label();
            this.tbToInstance = new System.Windows.Forms.TextBox();
            this.tbToResourceGroup = new System.Windows.Forms.TextBox();
            this.cbToResourceType = new System.Windows.Forms.ResourceTypeCombo();
            this.ckbKeepResourceType = new System.Windows.Forms.CheckBox();
            this.ckbKeepResourceGroup = new System.Windows.Forms.CheckBox();
            this.ckbKeepInstance = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.rtbResults = new System.Windows.Forms.RichTextBox();
            this.btnReplace = new System.Windows.Forms.Button();
            this.tlpCancelSave = new System.Windows.Forms.TableLayoutPanel();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.tlpFromTGIValues.SuspendLayout();
            this.rtContextMenu.SuspendLayout();
            this.tlpToTGIValues.SuspendLayout();
            this.tlpCancelSave.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.tlpFromTGIValues, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.tlpToTGIValues, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.label9, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.rtbResults, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.btnReplace, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.tlpCancelSave, 0, 6);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 8;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 6F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 6F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(643, 307);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tlpFromTGIValues
            // 
            this.tlpFromTGIValues.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tlpFromTGIValues.AutoSize = true;
            this.tlpFromTGIValues.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpFromTGIValues.ColumnCount = 3;
            this.tlpFromTGIValues.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpFromTGIValues.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpFromTGIValues.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tlpFromTGIValues.ContextMenuStrip = this.rtContextMenu;
            this.tlpFromTGIValues.Controls.Add(this.label3, 0, 0);
            this.tlpFromTGIValues.Controls.Add(this.tbFromInstance, 2, 2);
            this.tlpFromTGIValues.Controls.Add(this.tbFromResourceGroup, 2, 1);
            this.tlpFromTGIValues.Controls.Add(this.cbFromResourceType, 2, 0);
            this.tlpFromTGIValues.Controls.Add(this.ckbAnyResourceType, 1, 0);
            this.tlpFromTGIValues.Controls.Add(this.ckbAnyResourceGroup, 1, 1);
            this.tlpFromTGIValues.Controls.Add(this.ckbAnyInstance, 1, 2);
            this.tlpFromTGIValues.Controls.Add(this.label4, 0, 1);
            this.tlpFromTGIValues.Controls.Add(this.label5, 0, 2);
            this.tlpFromTGIValues.Location = new System.Drawing.Point(3, 22);
            this.tlpFromTGIValues.Name = "tlpFromTGIValues";
            this.tlpFromTGIValues.RowCount = 4;
            this.tlpFromTGIValues.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpFromTGIValues.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpFromTGIValues.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpFromTGIValues.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpFromTGIValues.Size = new System.Drawing.Size(304, 79);
            this.tlpFromTGIValues.TabIndex = 2;
            // 
            // rtContextMenu
            // 
            this.rtContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rtgiCopyRK,
            this.rtgiPasteRK});
            this.rtContextMenu.Name = "rtContextMenu";
            this.rtContextMenu.Size = new System.Drawing.Size(206, 48);
            this.rtContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.rtContextMenu_Opening);
            // 
            // rtgiCopyRK
            // 
            this.rtgiCopyRK.Name = "rtgiCopyRK";
            this.rtgiCopyRK.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.rtgiCopyRK.Size = new System.Drawing.Size(205, 22);
            this.rtgiCopyRK.Text = "&Copy ResourceKey";
            this.rtgiCopyRK.Click += new System.EventHandler(this.rtgiCopyRK_Click);
            // 
            // rtgiPasteRK
            // 
            this.rtgiPasteRK.Name = "rtgiPasteRK";
            this.rtgiPasteRK.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.rtgiPasteRK.Size = new System.Drawing.Size(205, 22);
            this.rtgiPasteRK.Text = "&Paste ResourceKey";
            this.rtgiPasteRK.Click += new System.EventHandler(this.rtgiPasteRK_Click);
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Type";
            // 
            // tbFromInstance
            // 
            this.tbFromInstance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFromInstance.Location = new System.Drawing.Point(107, 56);
            this.tbFromInstance.Name = "tbFromInstance";
            this.tbFromInstance.Size = new System.Drawing.Size(194, 20);
            this.tbFromInstance.TabIndex = 8;
            this.tbFromInstance.Text = "0xWWWWWWWWWWWWWWWW";
            this.tbFromInstance.Validating += new System.ComponentModel.CancelEventHandler(this.tbInstance_Validating);
            // 
            // tbFromResourceGroup
            // 
            this.tbFromResourceGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFromResourceGroup.Location = new System.Drawing.Point(107, 30);
            this.tbFromResourceGroup.Name = "tbFromResourceGroup";
            this.tbFromResourceGroup.Size = new System.Drawing.Size(194, 20);
            this.tbFromResourceGroup.TabIndex = 5;
            this.tbFromResourceGroup.Text = "0xWWWWWWWW";
            this.tbFromResourceGroup.Validating += new System.ComponentModel.CancelEventHandler(this.tbResourceGroup_Validating);
            // 
            // cbFromResourceType
            // 
            this.cbFromResourceType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cbFromResourceType.AutoSize = true;
            this.cbFromResourceType.Location = new System.Drawing.Point(107, 3);
            this.cbFromResourceType.Name = "cbFromResourceType";
            this.cbFromResourceType.Size = new System.Drawing.Size(194, 21);
            this.cbFromResourceType.TabIndex = 2;
            this.cbFromResourceType.Value = ((uint)(0u));
            this.cbFromResourceType.ValueChanged += new System.EventHandler(this.cbResourceType_ValueChanged);
            // 
            // ckbAnyResourceType
            // 
            this.ckbAnyResourceType.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbAnyResourceType.AutoSize = true;
            this.ckbAnyResourceType.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ckbAnyResourceType.Location = new System.Drawing.Point(57, 5);
            this.ckbAnyResourceType.Name = "ckbAnyResourceType";
            this.ckbAnyResourceType.Size = new System.Drawing.Size(44, 17);
            this.ckbAnyResourceType.TabIndex = 1;
            this.ckbAnyResourceType.Text = "Any";
            this.ckbAnyResourceType.UseVisualStyleBackColor = true;
            this.ckbAnyResourceType.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
            // 
            // ckbAnyResourceGroup
            // 
            this.ckbAnyResourceGroup.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbAnyResourceGroup.AutoSize = true;
            this.ckbAnyResourceGroup.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ckbAnyResourceGroup.Location = new System.Drawing.Point(57, 31);
            this.ckbAnyResourceGroup.Name = "ckbAnyResourceGroup";
            this.ckbAnyResourceGroup.Size = new System.Drawing.Size(44, 17);
            this.ckbAnyResourceGroup.TabIndex = 4;
            this.ckbAnyResourceGroup.Text = "Any";
            this.ckbAnyResourceGroup.UseVisualStyleBackColor = true;
            this.ckbAnyResourceGroup.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
            // 
            // ckbAnyInstance
            // 
            this.ckbAnyInstance.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbAnyInstance.AutoSize = true;
            this.ckbAnyInstance.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ckbAnyInstance.Location = new System.Drawing.Point(57, 57);
            this.ckbAnyInstance.Name = "ckbAnyInstance";
            this.ckbAnyInstance.Size = new System.Drawing.Size(44, 17);
            this.ckbAnyInstance.TabIndex = 7;
            this.ckbAnyInstance.Text = "Any";
            this.ckbAnyInstance.UseVisualStyleBackColor = true;
            this.ckbAnyInstance.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 33);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Group";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 59);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Instance";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(126, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Search for ResourceKey:";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(325, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(139, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Replace with ResourceKey:";
            // 
            // tlpToTGIValues
            // 
            this.tlpToTGIValues.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tlpToTGIValues.AutoSize = true;
            this.tlpToTGIValues.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpToTGIValues.ColumnCount = 3;
            this.tlpToTGIValues.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpToTGIValues.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpToTGIValues.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tlpToTGIValues.ContextMenuStrip = this.rtContextMenu;
            this.tlpToTGIValues.Controls.Add(this.label6, 0, 0);
            this.tlpToTGIValues.Controls.Add(this.tbToInstance, 2, 2);
            this.tlpToTGIValues.Controls.Add(this.tbToResourceGroup, 2, 1);
            this.tlpToTGIValues.Controls.Add(this.cbToResourceType, 2, 0);
            this.tlpToTGIValues.Controls.Add(this.ckbKeepResourceType, 1, 0);
            this.tlpToTGIValues.Controls.Add(this.ckbKeepResourceGroup, 1, 1);
            this.tlpToTGIValues.Controls.Add(this.ckbKeepInstance, 1, 2);
            this.tlpToTGIValues.Controls.Add(this.label7, 0, 1);
            this.tlpToTGIValues.Controls.Add(this.label8, 0, 2);
            this.tlpToTGIValues.Location = new System.Drawing.Point(325, 22);
            this.tlpToTGIValues.Name = "tlpToTGIValues";
            this.tlpToTGIValues.RowCount = 4;
            this.tlpToTGIValues.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpToTGIValues.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpToTGIValues.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpToTGIValues.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpToTGIValues.Size = new System.Drawing.Size(311, 79);
            this.tlpToTGIValues.TabIndex = 4;
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(20, 7);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(31, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Type";
            // 
            // tbToInstance
            // 
            this.tbToInstance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbToInstance.Location = new System.Drawing.Point(114, 56);
            this.tbToInstance.Name = "tbToInstance";
            this.tbToInstance.Size = new System.Drawing.Size(194, 20);
            this.tbToInstance.TabIndex = 8;
            this.tbToInstance.Text = "0xWWWWWWWWWWWWWWWW";
            this.tbToInstance.Validating += new System.ComponentModel.CancelEventHandler(this.tbInstance_Validating);
            // 
            // tbToResourceGroup
            // 
            this.tbToResourceGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbToResourceGroup.Location = new System.Drawing.Point(114, 30);
            this.tbToResourceGroup.Name = "tbToResourceGroup";
            this.tbToResourceGroup.Size = new System.Drawing.Size(194, 20);
            this.tbToResourceGroup.TabIndex = 5;
            this.tbToResourceGroup.Text = "0xWWWWWWWW";
            this.tbToResourceGroup.Validating += new System.ComponentModel.CancelEventHandler(this.tbResourceGroup_Validating);
            // 
            // cbToResourceType
            // 
            this.cbToResourceType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cbToResourceType.AutoSize = true;
            this.cbToResourceType.Location = new System.Drawing.Point(114, 3);
            this.cbToResourceType.Name = "cbToResourceType";
            this.cbToResourceType.Size = new System.Drawing.Size(194, 21);
            this.cbToResourceType.TabIndex = 2;
            this.cbToResourceType.Value = ((uint)(0u));
            this.cbToResourceType.ValueChanged += new System.EventHandler(this.cbResourceType_ValueChanged);
            // 
            // ckbKeepResourceType
            // 
            this.ckbKeepResourceType.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbKeepResourceType.AutoSize = true;
            this.ckbKeepResourceType.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ckbKeepResourceType.Location = new System.Drawing.Point(57, 5);
            this.ckbKeepResourceType.Name = "ckbKeepResourceType";
            this.ckbKeepResourceType.Size = new System.Drawing.Size(51, 17);
            this.ckbKeepResourceType.TabIndex = 1;
            this.ckbKeepResourceType.Text = "Keep";
            this.ckbKeepResourceType.UseVisualStyleBackColor = true;
            this.ckbKeepResourceType.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
            // 
            // ckbKeepResourceGroup
            // 
            this.ckbKeepResourceGroup.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbKeepResourceGroup.AutoSize = true;
            this.ckbKeepResourceGroup.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ckbKeepResourceGroup.Location = new System.Drawing.Point(57, 31);
            this.ckbKeepResourceGroup.Name = "ckbKeepResourceGroup";
            this.ckbKeepResourceGroup.Size = new System.Drawing.Size(51, 17);
            this.ckbKeepResourceGroup.TabIndex = 4;
            this.ckbKeepResourceGroup.Text = "Keep";
            this.ckbKeepResourceGroup.UseVisualStyleBackColor = true;
            this.ckbKeepResourceGroup.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
            // 
            // ckbKeepInstance
            // 
            this.ckbKeepInstance.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbKeepInstance.AutoSize = true;
            this.ckbKeepInstance.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ckbKeepInstance.Location = new System.Drawing.Point(57, 57);
            this.ckbKeepInstance.Name = "ckbKeepInstance";
            this.ckbKeepInstance.Size = new System.Drawing.Size(51, 17);
            this.ckbKeepInstance.TabIndex = 7;
            this.ckbKeepInstance.Text = "Keep";
            this.ckbKeepInstance.UseVisualStyleBackColor = true;
            this.ckbKeepInstance.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 33);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(36, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Group";
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 59);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(48, 13);
            this.label8.TabIndex = 6;
            this.label8.Text = "Instance";
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 133);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(45, 13);
            this.label9.TabIndex = 6;
            this.label9.Text = "Results:";
            // 
            // rtbResults
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.rtbResults, 4);
            this.rtbResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbResults.Location = new System.Drawing.Point(3, 149);
            this.rtbResults.Name = "rtbResults";
            this.rtbResults.ReadOnly = true;
            this.rtbResults.Size = new System.Drawing.Size(637, 120);
            this.rtbResults.TabIndex = 7;
            this.rtbResults.Text = "";
            // 
            // btnReplace
            // 
            this.btnReplace.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnReplace.Location = new System.Drawing.Point(3, 107);
            this.btnReplace.Name = "btnReplace";
            this.btnReplace.Size = new System.Drawing.Size(75, 23);
            this.btnReplace.TabIndex = 5;
            this.btnReplace.Text = "&Replace";
            this.btnReplace.UseVisualStyleBackColor = true;
            this.btnReplace.Click += new System.EventHandler(this.btnReplace_Click);
            // 
            // tlpCancelSave
            // 
            this.tlpCancelSave.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tlpCancelSave.AutoSize = true;
            this.tlpCancelSave.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpCancelSave.ColumnCount = 2;
            this.tlpCancelSave.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpCancelSave.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpCancelSave.Controls.Add(this.btnSave, 1, 0);
            this.tlpCancelSave.Controls.Add(this.btnCancel, 0, 0);
            this.tlpCancelSave.Location = new System.Drawing.Point(0, 272);
            this.tlpCancelSave.Margin = new System.Windows.Forms.Padding(0);
            this.tlpCancelSave.Name = "tlpCancelSave";
            this.tlpCancelSave.RowCount = 1;
            this.tlpCancelSave.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpCancelSave.Size = new System.Drawing.Size(162, 29);
            this.tlpCancelSave.TabIndex = 8;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSave.Location = new System.Drawing.Point(84, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "&Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancel.Location = new System.Drawing.Point(3, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "C&ancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ReplaceTGI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ReplaceTGI";
            this.Size = new System.Drawing.Size(643, 307);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tlpFromTGIValues.ResumeLayout(false);
            this.tlpFromTGIValues.PerformLayout();
            this.rtContextMenu.ResumeLayout(false);
            this.tlpToTGIValues.ResumeLayout(false);
            this.tlpToTGIValues.PerformLayout();
            this.tlpCancelSave.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnReplace;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TableLayoutPanel tlpFromTGIValues;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbFromInstance;
        private System.Windows.Forms.TextBox tbFromResourceGroup;
        private System.Windows.Forms.ResourceTypeCombo cbFromResourceType;
        private System.Windows.Forms.CheckBox ckbAnyResourceType;
        private System.Windows.Forms.CheckBox ckbAnyResourceGroup;
        private System.Windows.Forms.CheckBox ckbAnyInstance;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TableLayoutPanel tlpToTGIValues;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbToInstance;
        private System.Windows.Forms.TextBox tbToResourceGroup;
        private System.Windows.Forms.ResourceTypeCombo cbToResourceType;
        private System.Windows.Forms.CheckBox ckbKeepResourceType;
        private System.Windows.Forms.CheckBox ckbKeepResourceGroup;
        private System.Windows.Forms.CheckBox ckbKeepInstance;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.RichTextBox rtbResults;
        private System.Windows.Forms.ContextMenuStrip rtContextMenu;
        private System.Windows.Forms.ToolStripMenuItem rtgiCopyRK;
        private System.Windows.Forms.ToolStripMenuItem rtgiPasteRK;
        private System.Windows.Forms.TableLayoutPanel tlpCancelSave;
        private System.Windows.Forms.Button btnCancel;
    }
}
