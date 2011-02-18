﻿namespace ObjectCloner.TopPanelComponents
{
    partial class Search
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
            this.listView1 = new System.Windows.Forms.ListView();
            this.catlgName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TagID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ContentCategoryFlags = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TGI = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tlpSearch = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.cbCatalogType = new System.Windows.Forms.ComboBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.tlpWhere = new System.Windows.Forms.TableLayoutPanel();
            this.ckbResourceName = new System.Windows.Forms.CheckBox();
            this.rb1English = new System.Windows.Forms.RadioButton();
            this.rb1All = new System.Windows.Forms.RadioButton();
            this.ckbObjectName = new System.Windows.Forms.CheckBox();
            this.ckbObjectDesc = new System.Windows.Forms.CheckBox();
            this.ckbCatalogName = new System.Windows.Forms.CheckBox();
            this.ckbCatalogDesc = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbText = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tlpSearch.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tlpWhere.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.catlgName,
            this.TagID,
            this.ContentCategoryFlags,
            this.TGI});
            this.tlpSearch.SetColumnSpan(this.listView1, 4);
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.FullRowSelect = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(0, 161);
            this.listView1.Margin = new System.Windows.Forms.Padding(0);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(391, 145);
            this.listView1.TabIndex = 7;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.ItemActivate += new System.EventHandler(this.listView1_ItemActivate);
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // catlgName
            // 
            this.catlgName.Text = "Name";
            this.catlgName.Width = 45;
            // 
            // TagID
            // 
            this.TagID.Text = "Tag";
            this.TagID.Width = 32;
            // 
            // ContentCategoryFlags
            // 
            this.ContentCategoryFlags.Text = "EP/SP";
            this.ContentCategoryFlags.Width = 59;
            // 
            // TGI
            // 
            this.TGI.Text = "Resource Key";
            this.TGI.Width = 138;
            // 
            // tlpSearch
            // 
            this.tlpSearch.ColumnCount = 4;
            this.tlpSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 2F));
            this.tlpSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 2F));
            this.tlpSearch.Controls.Add(this.tableLayoutPanel1, 2, 3);
            this.tlpSearch.Controls.Add(this.tlpWhere, 2, 2);
            this.tlpSearch.Controls.Add(this.label1, 1, 1);
            this.tlpSearch.Controls.Add(this.tbText, 2, 1);
            this.tlpSearch.Controls.Add(this.label2, 1, 2);
            this.tlpSearch.Controls.Add(this.label3, 1, 3);
            this.tlpSearch.Controls.Add(this.listView1, 0, 4);
            this.tlpSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpSearch.Location = new System.Drawing.Point(0, 0);
            this.tlpSearch.Name = "tlpSearch";
            this.tlpSearch.RowCount = 6;
            this.tlpSearch.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 2F));
            this.tlpSearch.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpSearch.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpSearch.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpSearch.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpSearch.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 2F));
            this.tlpSearch.Size = new System.Drawing.Size(391, 308);
            this.tlpSearch.TabIndex = 1;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.cbCatalogType, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnSearch, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(100, 129);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(286, 29);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // cbCatalogType
            // 
            this.cbCatalogType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cbCatalogType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCatalogType.FormattingEnabled = true;
            this.cbCatalogType.Location = new System.Drawing.Point(3, 4);
            this.cbCatalogType.Name = "cbCatalogType";
            this.cbCatalogType.Size = new System.Drawing.Size(199, 21);
            this.cbCatalogType.TabIndex = 1;
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Location = new System.Drawing.Point(208, 3);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // tlpWhere
            // 
            this.tlpWhere.AutoSize = true;
            this.tlpWhere.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpWhere.ColumnCount = 2;
            this.tlpWhere.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpWhere.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpWhere.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpWhere.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpWhere.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpWhere.Controls.Add(this.ckbResourceName, 0, 0);
            this.tlpWhere.Controls.Add(this.rb1English, 0, 3);
            this.tlpWhere.Controls.Add(this.rb1All, 1, 3);
            this.tlpWhere.Controls.Add(this.ckbObjectName, 0, 1);
            this.tlpWhere.Controls.Add(this.ckbObjectDesc, 1, 1);
            this.tlpWhere.Controls.Add(this.ckbCatalogName, 0, 2);
            this.tlpWhere.Controls.Add(this.ckbCatalogDesc, 1, 2);
            this.tlpWhere.Location = new System.Drawing.Point(100, 31);
            this.tlpWhere.Name = "tlpWhere";
            this.tlpWhere.RowCount = 4;
            this.tlpWhere.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpWhere.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpWhere.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpWhere.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpWhere.Size = new System.Drawing.Size(205, 92);
            this.tlpWhere.TabIndex = 4;
            // 
            // ckbResourceName
            // 
            this.ckbResourceName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ckbResourceName.AutoSize = true;
            this.ckbResourceName.Location = new System.Drawing.Point(3, 3);
            this.ckbResourceName.Name = "ckbResourceName";
            this.ckbResourceName.Size = new System.Drawing.Size(103, 17);
            this.ckbResourceName.TabIndex = 1;
            this.ckbResourceName.Text = "Resource Name";
            this.ckbResourceName.UseVisualStyleBackColor = true;
            this.ckbResourceName.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
            // 
            // rb1English
            // 
            this.rb1English.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.rb1English.AutoSize = true;
            this.rb1English.Checked = true;
            this.rb1English.Location = new System.Drawing.Point(3, 72);
            this.rb1English.Name = "rb1English";
            this.rb1English.Size = new System.Drawing.Size(81, 17);
            this.rb1English.TabIndex = 6;
            this.rb1English.TabStop = true;
            this.rb1English.Text = "English only";
            this.rb1English.UseVisualStyleBackColor = true;
            // 
            // rb1All
            // 
            this.rb1All.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.rb1All.AutoSize = true;
            this.rb1All.Location = new System.Drawing.Point(112, 72);
            this.rb1All.Name = "rb1All";
            this.rb1All.Size = new System.Drawing.Size(88, 17);
            this.rb1All.TabIndex = 7;
            this.rb1All.Text = "All languages";
            this.rb1All.UseVisualStyleBackColor = true;
            // 
            // ckbObjectName
            // 
            this.ckbObjectName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ckbObjectName.AutoSize = true;
            this.ckbObjectName.Location = new System.Drawing.Point(3, 26);
            this.ckbObjectName.Name = "ckbObjectName";
            this.ckbObjectName.Size = new System.Drawing.Size(88, 17);
            this.ckbObjectName.TabIndex = 2;
            this.ckbObjectName.Text = "Object Name";
            this.ckbObjectName.UseVisualStyleBackColor = true;
            this.ckbObjectName.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
            // 
            // ckbObjectDesc
            // 
            this.ckbObjectDesc.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ckbObjectDesc.AutoSize = true;
            this.ckbObjectDesc.Location = new System.Drawing.Point(112, 26);
            this.ckbObjectDesc.Name = "ckbObjectDesc";
            this.ckbObjectDesc.Size = new System.Drawing.Size(85, 17);
            this.ckbObjectDesc.TabIndex = 3;
            this.ckbObjectDesc.Text = "Object Desc";
            this.ckbObjectDesc.UseVisualStyleBackColor = true;
            this.ckbObjectDesc.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
            // 
            // ckbCatalogName
            // 
            this.ckbCatalogName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ckbCatalogName.AutoSize = true;
            this.ckbCatalogName.Location = new System.Drawing.Point(3, 49);
            this.ckbCatalogName.Name = "ckbCatalogName";
            this.ckbCatalogName.Size = new System.Drawing.Size(93, 17);
            this.ckbCatalogName.TabIndex = 4;
            this.ckbCatalogName.Text = "Catalog Name";
            this.ckbCatalogName.UseVisualStyleBackColor = true;
            this.ckbCatalogName.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
            // 
            // ckbCatalogDesc
            // 
            this.ckbCatalogDesc.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ckbCatalogDesc.AutoSize = true;
            this.ckbCatalogDesc.Location = new System.Drawing.Point(112, 49);
            this.ckbCatalogDesc.Name = "ckbCatalogDesc";
            this.ckbCatalogDesc.Size = new System.Drawing.Size(90, 17);
            this.ckbCatalogDesc.TabIndex = 5;
            this.ckbCatalogDesc.Text = "Catalog Desc";
            this.ckbCatalogDesc.UseVisualStyleBackColor = true;
            this.ckbCatalogDesc.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Search for text:";
            // 
            // tbText
            // 
            this.tbText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbText.Location = new System.Drawing.Point(100, 5);
            this.tbText.Name = "tbText";
            this.tbText.Size = new System.Drawing.Size(286, 20);
            this.tbText.TabIndex = 2;
            this.tbText.TextChanged += new System.EventHandler(this.tbText_TextChanged);
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Where to search:";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 137);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Kind of object:";
            // 
            // Search
            // 
            this.Controls.Add(this.tlpSearch);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "Search";
            this.Size = new System.Drawing.Size(391, 308);
            this.tlpSearch.ResumeLayout(false);
            this.tlpSearch.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tlpWhere.ResumeLayout(false);
            this.tlpWhere.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader catlgName;
        private System.Windows.Forms.ColumnHeader TGI;
        private System.Windows.Forms.ColumnHeader TagID;
        private System.Windows.Forms.ColumnHeader ContentCategoryFlags;
        private System.Windows.Forms.TableLayoutPanel tlpSearch;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbText;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TableLayoutPanel tlpWhere;
        private System.Windows.Forms.CheckBox ckbResourceName;
        private System.Windows.Forms.CheckBox ckbObjectName;
        private System.Windows.Forms.CheckBox ckbObjectDesc;
        private System.Windows.Forms.CheckBox ckbCatalogName;
        private System.Windows.Forms.CheckBox ckbCatalogDesc;
        private System.Windows.Forms.RadioButton rb1English;
        private System.Windows.Forms.RadioButton rb1All;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbCatalogType;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;

    }
}