namespace ObjectCloner.TopPanelComponents
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
            this.listView1 = new System.Windows.Forms.ListView();
            this.catlgName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TagID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ContentCategoryFlags = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TGI = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Path = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tgiSearchContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tgisCopyRK = new System.Windows.Forms.ToolStripMenuItem();
            this.tgisPasteRK = new System.Windows.Forms.ToolStripMenuItem();
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
            this.ckbUseCC = new System.Windows.Forms.CheckBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.tlpSearch.SuspendLayout();
            this.tgiSearchContextMenu.SuspendLayout();
            this.tlpTGIValues.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpSearch
            // 
            this.tlpSearch.ColumnCount = 4;
            this.tlpSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpSearch.Controls.Add(this.listView1, 0, 3);
            this.tlpSearch.Controls.Add(this.label1, 0, 0);
            this.tlpSearch.Controls.Add(this.tlpTGIValues, 1, 0);
            this.tlpSearch.Controls.Add(this.ckbUseCC, 1, 2);
            this.tlpSearch.Controls.Add(this.btnSearch, 2, 2);
            this.tlpSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpSearch.Location = new System.Drawing.Point(0, 0);
            this.tlpSearch.Name = "tlpSearch";
            this.tlpSearch.RowCount = 4;
            this.tlpSearch.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpSearch.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpSearch.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpSearch.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpSearch.Size = new System.Drawing.Size(800, 232);
            this.tlpSearch.TabIndex = 0;
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
            this.listView1.ContextMenuStrip = this.tgiSearchContextMenu;
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.FullRowSelect = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(0, 118);
            this.listView1.Margin = new System.Windows.Forms.Padding(0);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(800, 114);
            this.listView1.TabIndex = 4;
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
            // tgiSearchContextMenu
            // 
            this.tgiSearchContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tgisCopyRK,
            this.tgisPasteRK});
            this.tgiSearchContextMenu.Name = "tgiSearchContextMenu";
            this.tgiSearchContextMenu.Size = new System.Drawing.Size(209, 70);
            this.tgiSearchContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.tgiSearchContextMenu_Opening);
            // 
            // tgisCopyRK
            // 
            this.tgisCopyRK.Enabled = false;
            this.tgisCopyRK.Name = "tgisCopyRK";
            this.tgisCopyRK.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.tgisCopyRK.Size = new System.Drawing.Size(208, 22);
            this.tgisCopyRK.Text = "&Copy ResourceKey";
            this.tgisCopyRK.Click += new System.EventHandler(this.tgisCopyRK_Click);
            // 
            // tgisPasteRK
            // 
            this.tgisPasteRK.Enabled = false;
            this.tgisPasteRK.Name = "tgisPasteRK";
            this.tgisPasteRK.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.tgisPasteRK.Size = new System.Drawing.Size(208, 22);
            this.tgisPasteRK.Text = "&Paste ResourceKey";
            this.tgisPasteRK.Click += new System.EventHandler(this.tgisPasteRK_Click);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.label1.Size = new System.Drawing.Size(59, 19);
            this.label1.TabIndex = 0;
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
            this.tlpSearch.SetRowSpan(this.tlpTGIValues, 2);
            this.tlpTGIValues.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpTGIValues.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpTGIValues.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpTGIValues.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpTGIValues.Size = new System.Drawing.Size(333, 83);
            this.tlpTGIValues.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Type";
            // 
            // tbInstance
            // 
            this.tbInstance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbInstance.Location = new System.Drawing.Point(107, 56);
            this.tbInstance.Name = "tbInstance";
            this.tbInstance.Size = new System.Drawing.Size(223, 20);
            this.tbInstance.TabIndex = 8;
            this.tbInstance.Text = "0xWWWWWWWWWWWWWWWW";
            this.tbInstance.Validating += new System.ComponentModel.CancelEventHandler(this.tbInstance_Validating);
            // 
            // tbResourceGroup
            // 
            this.tbResourceGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbResourceGroup.Location = new System.Drawing.Point(107, 30);
            this.tbResourceGroup.Name = "tbResourceGroup";
            this.tbResourceGroup.Size = new System.Drawing.Size(223, 20);
            this.tbResourceGroup.TabIndex = 5;
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
            this.cbResourceType.TabIndex = 2;
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
            this.ckbResourceType.TabIndex = 1;
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
            this.ckbResourceGroup.TabIndex = 4;
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
            this.ckbInstance.TabIndex = 7;
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
            this.label3.TabIndex = 3;
            this.label3.Text = "Group";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 59);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Instance";
            // 
            // ckbUseCC
            // 
            this.ckbUseCC.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ckbUseCC.AutoSize = true;
            this.ckbUseCC.Location = new System.Drawing.Point(68, 95);
            this.ckbUseCC.Name = "ckbUseCC";
            this.ckbUseCC.Size = new System.Drawing.Size(137, 17);
            this.ckbUseCC.TabIndex = 2;
            this.ckbUseCC.Text = "Include custom content";
            this.ckbUseCC.UseVisualStyleBackColor = true;
            this.ckbUseCC.CheckedChanged += new System.EventHandler(this.ckbInstance_CheckedChanged);
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSearch.Location = new System.Drawing.Point(407, 92);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 3;
            this.btnSearch.Text = "&Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
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
            this.tgiSearchContextMenu.ResumeLayout(false);
            this.tlpTGIValues.ResumeLayout(false);
            this.tlpTGIValues.PerformLayout();
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
        private System.Windows.Forms.ContextMenuStrip tgiSearchContextMenu;
        private System.Windows.Forms.ToolStripMenuItem tgisPasteRK;
        private System.Windows.Forms.ToolStripMenuItem tgisCopyRK;
        private System.Windows.Forms.CheckBox ckbUseCC;
    }
}
