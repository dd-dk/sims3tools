namespace ObjectCloner.SplitterComponents
{
    partial class TGISearch
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
            this.tlpSearch = new System.Windows.Forms.TableLayoutPanel();
            this.tlpTGIContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tgisCopyRK = new System.Windows.Forms.ToolStripMenuItem();
            this.tgisPasteRK = new System.Windows.Forms.ToolStripMenuItem();
            this.listView1 = new System.Windows.Forms.ListView();
            this.catlgName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TagID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ContentCategoryFlags = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TGI = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Path = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.lvcmCopyRK = new System.Windows.Forms.ToolStripMenuItem();
            this.lvcmActivate = new System.Windows.Forms.ToolStripMenuItem();
            this.lvcmFix = new System.Windows.Forms.ToolStripMenuItem();
            this.lvcmEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.tlpTGIValues = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.tbInstance = new System.Windows.Forms.TextBox();
            this.tbResourceGroup = new System.Windows.Forms.TextBox();
            this.cbResourceType = new System.Windows.Forms.ResourceTypeCombo();
            this.ckbResourceType = new System.Windows.Forms.CheckBox();
            this.ckbResourceGroup = new System.Windows.Forms.CheckBox();
            this.ckbInstance = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.ckbUseCC = new System.Windows.Forms.CheckBox();
            this.ckbUseEA = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tlpCount = new System.Windows.Forms.TableLayoutPanel();
            this.lbCount = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnPasteRK = new System.Windows.Forms.Button();
            this.tlpSearch.SuspendLayout();
            this.tlpTGIContextMenu.SuspendLayout();
            this.lvContextMenu.SuspendLayout();
            this.tlpTGIValues.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tlpCount.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpSearch
            // 
            this.tlpSearch.ColumnCount = 4;
            this.tlpSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpSearch.ContextMenuStrip = this.tlpTGIContextMenu;
            this.tlpSearch.Controls.Add(this.listView1, 0, 2);
            this.tlpSearch.Controls.Add(this.label1, 0, 0);
            this.tlpSearch.Controls.Add(this.tlpTGIValues, 1, 0);
            this.tlpSearch.Controls.Add(this.tableLayoutPanel1, 2, 1);
            this.tlpSearch.Controls.Add(this.tableLayoutPanel2, 1, 1);
            this.tlpSearch.Controls.Add(this.tableLayoutPanel3, 2, 0);
            this.tlpSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpSearch.Location = new System.Drawing.Point(0, 0);
            this.tlpSearch.Name = "tlpSearch";
            this.tlpSearch.RowCount = 3;
            this.tlpSearch.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpSearch.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpSearch.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpSearch.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpSearch.Size = new System.Drawing.Size(800, 232);
            this.tlpSearch.TabIndex = 0;
            // 
            // tlpTGIContextMenu
            // 
            this.tlpTGIContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tgisCopyRK,
            this.tgisPasteRK});
            this.tlpTGIContextMenu.Name = "tgiSearchContextMenu";
            this.tlpTGIContextMenu.Size = new System.Drawing.Size(206, 48);
            this.tlpTGIContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.tgiSearchContextMenu_Opening);
            // 
            // tgisCopyRK
            // 
            this.tgisCopyRK.Name = "tgisCopyRK";
            this.tgisCopyRK.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.tgisCopyRK.Size = new System.Drawing.Size(205, 22);
            this.tgisCopyRK.Text = "&Copy ResourceKey";
            this.tgisCopyRK.Click += new System.EventHandler(this.tgisCopyRK_Click);
            // 
            // tgisPasteRK
            // 
            this.tgisPasteRK.Name = "tgisPasteRK";
            this.tgisPasteRK.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.tgisPasteRK.Size = new System.Drawing.Size(205, 22);
            this.tgisPasteRK.Text = "&Paste ResourceKey";
            this.tgisPasteRK.Click += new System.EventHandler(this.tgisPasteRK_Click);
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.catlgName,
            this.TagID,
            this.ContentCategoryFlags,
            this.TGI,
            this.Path});
            this.tlpSearch.SetColumnSpan(this.listView1, 4);
            this.listView1.ContextMenuStrip = this.lvContextMenu;
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.FullRowSelect = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(0, 118);
            this.listView1.Margin = new System.Windows.Forms.Padding(0);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(800, 114);
            this.listView1.TabIndex = 5;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView1_ColumnClick);
            this.listView1.ItemActivate += new System.EventHandler(this.listView1_ItemActivate);
            // 
            // catlgName
            // 
            this.catlgName.Text = "Name";
            this.catlgName.Width = 160;
            // 
            // TagID
            // 
            this.TagID.Text = "Tag";
            this.TagID.Width = 64;
            // 
            // ContentCategoryFlags
            // 
            this.ContentCategoryFlags.Text = "EP/SP";
            this.ContentCategoryFlags.Width = 64;
            // 
            // TGI
            // 
            this.TGI.Text = "ResourceKey";
            this.TGI.Width = 192;
            // 
            // Path
            // 
            this.Path.Text = "Path";
            this.Path.Width = 296;
            // 
            // lvContextMenu
            // 
            this.lvContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lvcmCopyRK,
            this.lvcmActivate,
            this.lvcmFix,
            this.lvcmEdit});
            this.lvContextMenu.Name = "lvContextMenu";
            this.lvContextMenu.Size = new System.Drawing.Size(205, 92);
            this.lvContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.lvContextMenu_Opening);
            // 
            // lvcmCopyRK
            // 
            this.lvcmCopyRK.Name = "lvcmCopyRK";
            this.lvcmCopyRK.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.lvcmCopyRK.Size = new System.Drawing.Size(204, 22);
            this.lvcmCopyRK.Text = "&Copy ResourceKey";
            this.lvcmCopyRK.Click += new System.EventHandler(this.tgisCopyRK_Click);
            // 
            // lvcmActivate
            // 
            this.lvcmActivate.Name = "lvcmActivate";
            this.lvcmActivate.ShortcutKeyDisplayString = "Enter";
            this.lvcmActivate.Size = new System.Drawing.Size(204, 22);
            this.lvcmActivate.Text = "Resource details";
            this.lvcmActivate.Click += new System.EventHandler(this.lvActivate_Click);
            // 
            // lvcmFix
            // 
            this.lvcmFix.Name = "lvcmFix";
            this.lvcmFix.Size = new System.Drawing.Size(204, 22);
            this.lvcmFix.Text = "&Open containing package";
            this.lvcmFix.Click += new System.EventHandler(this.lvcmFix_Click);
            // 
            // lvcmEdit
            // 
            this.lvcmEdit.Name = "lvcmEdit";
            this.lvcmEdit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.lvcmEdit.Size = new System.Drawing.Size(204, 22);
            this.lvcmEdit.Text = "Export to &Editor";
            this.lvcmEdit.Click += new System.EventHandler(this.lvcmEdit_Click);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 35);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.label1.Size = new System.Drawing.Size(59, 19);
            this.label1.TabIndex = 1;
            this.label1.Text = "Search for:";
            // 
            // tlpTGIValues
            // 
            this.tlpTGIValues.ColumnCount = 3;
            this.tlpTGIValues.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpTGIValues.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpTGIValues.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 229F));
            this.tlpTGIValues.Controls.Add(this.label2, 0, 0);
            this.tlpTGIValues.Controls.Add(this.tbInstance, 2, 2);
            this.tlpTGIValues.Controls.Add(this.tbResourceGroup, 2, 1);
            this.tlpTGIValues.Controls.Add(this.cbResourceType, 2, 0);
            this.tlpTGIValues.Controls.Add(this.ckbResourceType, 1, 0);
            this.tlpTGIValues.Controls.Add(this.ckbResourceGroup, 1, 1);
            this.tlpTGIValues.Controls.Add(this.ckbInstance, 1, 2);
            this.tlpTGIValues.Controls.Add(this.label3, 0, 1);
            this.tlpTGIValues.Controls.Add(this.label4, 0, 2);
            this.tlpTGIValues.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpTGIValues.Location = new System.Drawing.Point(68, 3);
            this.tlpTGIValues.Name = "tlpTGIValues";
            this.tlpTGIValues.RowCount = 4;
            this.tlpTGIValues.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpTGIValues.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpTGIValues.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpTGIValues.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpTGIValues.Size = new System.Drawing.Size(333, 83);
            this.tlpTGIValues.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Type";
            // 
            // tbInstance
            // 
            this.tbInstance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbInstance.Location = new System.Drawing.Point(107, 56);
            this.tbInstance.Name = "tbInstance";
            this.tbInstance.Size = new System.Drawing.Size(223, 20);
            this.tbInstance.TabIndex = 9;
            this.tbInstance.Text = "0xWWWWWWWWWWWWWWWW";
            this.tbInstance.Validating += new System.ComponentModel.CancelEventHandler(this.tbInstance_Validating);
            // 
            // tbResourceGroup
            // 
            this.tbResourceGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbResourceGroup.Location = new System.Drawing.Point(107, 30);
            this.tbResourceGroup.Name = "tbResourceGroup";
            this.tbResourceGroup.Size = new System.Drawing.Size(223, 20);
            this.tbResourceGroup.TabIndex = 6;
            this.tbResourceGroup.Text = "0xWWWWWWWW";
            this.tbResourceGroup.Validating += new System.ComponentModel.CancelEventHandler(this.tbResourceGroup_Validating);
            // 
            // cbResourceType
            // 
            this.cbResourceType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cbResourceType.AutoSize = true;
            this.cbResourceType.Location = new System.Drawing.Point(107, 3);
            this.cbResourceType.Name = "cbResourceType";
            this.cbResourceType.Size = new System.Drawing.Size(223, 21);
            this.cbResourceType.TabIndex = 3;
            this.cbResourceType.Value = ((uint)(0u));
            // 
            // ckbResourceType
            // 
            this.ckbResourceType.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbResourceType.AutoSize = true;
            this.ckbResourceType.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ckbResourceType.Location = new System.Drawing.Point(57, 5);
            this.ckbResourceType.Name = "ckbResourceType";
            this.ckbResourceType.Size = new System.Drawing.Size(44, 17);
            this.ckbResourceType.TabIndex = 2;
            this.ckbResourceType.Text = "Any";
            this.ckbResourceType.UseVisualStyleBackColor = true;
            this.ckbResourceType.CheckedChanged += new System.EventHandler(this.ckbResourceType_CheckedChanged);
            // 
            // ckbResourceGroup
            // 
            this.ckbResourceGroup.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbResourceGroup.AutoSize = true;
            this.ckbResourceGroup.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ckbResourceGroup.Location = new System.Drawing.Point(57, 31);
            this.ckbResourceGroup.Name = "ckbResourceGroup";
            this.ckbResourceGroup.Size = new System.Drawing.Size(44, 17);
            this.ckbResourceGroup.TabIndex = 5;
            this.ckbResourceGroup.Text = "Any";
            this.ckbResourceGroup.UseVisualStyleBackColor = true;
            this.ckbResourceGroup.CheckedChanged += new System.EventHandler(this.ckbResourceGroup_CheckedChanged);
            // 
            // ckbInstance
            // 
            this.ckbInstance.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbInstance.AutoSize = true;
            this.ckbInstance.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ckbInstance.Location = new System.Drawing.Point(57, 57);
            this.ckbInstance.Name = "ckbInstance";
            this.ckbInstance.Size = new System.Drawing.Size(44, 17);
            this.ckbInstance.TabIndex = 8;
            this.ckbInstance.Text = "Any";
            this.ckbInstance.UseVisualStyleBackColor = true;
            this.ckbInstance.CheckedChanged += new System.EventHandler(this.ckbInstance_CheckedChanged);
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Group";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 59);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Instance";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.btnCancel, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnSearch, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(404, 89);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(162, 29);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancel.Location = new System.Drawing.Point(84, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "C&ancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSearch.Location = new System.Drawing.Point(3, 3);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 1;
            this.btnSearch.Text = "&Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.ckbUseCC, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.ckbUseEA, 0, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(65, 92);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(266, 23);
            this.tableLayoutPanel2.TabIndex = 3;
            // 
            // ckbUseCC
            // 
            this.ckbUseCC.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbUseCC.AutoSize = true;
            this.ckbUseCC.Location = new System.Drawing.Point(126, 3);
            this.ckbUseCC.Name = "ckbUseCC";
            this.ckbUseCC.Size = new System.Drawing.Size(137, 17);
            this.ckbUseCC.TabIndex = 2;
            this.ckbUseCC.Text = "Include custom content";
            this.ckbUseCC.UseVisualStyleBackColor = true;
            this.ckbUseCC.CheckedChanged += new System.EventHandler(this.ckbUse_CheckedChanged);
            // 
            // ckbUseEA
            // 
            this.ckbUseEA.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbUseEA.AutoSize = true;
            this.ckbUseEA.Location = new System.Drawing.Point(3, 3);
            this.ckbUseEA.Name = "ckbUseEA";
            this.ckbUseEA.Size = new System.Drawing.Size(117, 17);
            this.ckbUseEA.TabIndex = 1;
            this.ckbUseEA.Text = "Include EA content";
            this.ckbUseEA.UseVisualStyleBackColor = true;
            this.ckbUseEA.CheckedChanged += new System.EventHandler(this.ckbUse_CheckedChanged);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.tlpCount, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnPasteRK, 0, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(404, 0);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(162, 89);
            this.tableLayoutPanel3.TabIndex = 7;
            // 
            // tlpCount
            // 
            this.tlpCount.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.tlpCount.AutoSize = true;
            this.tlpCount.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpCount.ColumnCount = 1;
            this.tlpCount.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpCount.Controls.Add(this.lbCount, 0, 1);
            this.tlpCount.Controls.Add(this.label5, 0, 0);
            this.tlpCount.Location = new System.Drawing.Point(16, 31);
            this.tlpCount.Name = "tlpCount";
            this.tlpCount.RowCount = 2;
            this.tlpCount.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpCount.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpCount.Size = new System.Drawing.Size(129, 26);
            this.tlpCount.TabIndex = 6;
            this.tlpCount.Visible = false;
            // 
            // lbCount
            // 
            this.lbCount.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbCount.AutoSize = true;
            this.lbCount.Location = new System.Drawing.Point(57, 13);
            this.lbCount.Name = "lbCount";
            this.lbCount.Size = new System.Drawing.Size(14, 13);
            this.lbCount.TabIndex = 2;
            this.lbCount.Text = "X";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(123, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Number of entries found:";
            // 
            // btnPasteRK
            // 
            this.btnPasteRK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPasteRK.Location = new System.Drawing.Point(3, 63);
            this.btnPasteRK.Name = "btnPasteRK";
            this.btnPasteRK.Size = new System.Drawing.Size(75, 23);
            this.btnPasteRK.TabIndex = 7;
            this.btnPasteRK.Text = "&Paste RK";
            this.btnPasteRK.UseVisualStyleBackColor = true;
            this.btnPasteRK.Click += new System.EventHandler(this.btnPasteRK_Click);
            // 
            // TGISearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlpSearch);
            this.Name = "TGISearch";
            this.Size = new System.Drawing.Size(800, 232);
            this.tlpSearch.ResumeLayout(false);
            this.tlpSearch.PerformLayout();
            this.tlpTGIContextMenu.ResumeLayout(false);
            this.lvContextMenu.ResumeLayout(false);
            this.tlpTGIValues.ResumeLayout(false);
            this.tlpTGIValues.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tlpCount.ResumeLayout(false);
            this.tlpCount.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpSearch;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ResourceTypeCombo cbResourceType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox ckbResourceType;
        private System.Windows.Forms.CheckBox ckbResourceGroup;
        private System.Windows.Forms.TextBox tbResourceGroup;
        private System.Windows.Forms.CheckBox ckbInstance;
        private System.Windows.Forms.TextBox tbInstance;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader catlgName;
        private System.Windows.Forms.ColumnHeader TagID;
        private System.Windows.Forms.ColumnHeader ContentCategoryFlags;
        private System.Windows.Forms.ColumnHeader TGI;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TableLayoutPanel tlpTGIValues;
        private System.Windows.Forms.ColumnHeader Path;
        private System.Windows.Forms.ContextMenuStrip tlpTGIContextMenu;
        private System.Windows.Forms.ToolStripMenuItem tgisPasteRK;
        private System.Windows.Forms.ToolStripMenuItem tgisCopyRK;
        private System.Windows.Forms.CheckBox ckbUseCC;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.CheckBox ckbUseEA;
        private System.Windows.Forms.ContextMenuStrip lvContextMenu;
        private System.Windows.Forms.ToolStripMenuItem lvcmCopyRK;
        private System.Windows.Forms.ToolStripMenuItem lvcmActivate;
        private System.Windows.Forms.ToolStripMenuItem lvcmFix;
        private System.Windows.Forms.ToolStripMenuItem lvcmEdit;
        private System.Windows.Forms.TableLayoutPanel tlpCount;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lbCount;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button btnPasteRK;
    }
}
