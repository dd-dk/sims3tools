namespace ObjectCloner.TopPanelComponents
{
    partial class ObjectChooser
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
            this.catlgName = new System.Windows.Forms.ColumnHeader();
            this.TagID = new System.Windows.Forms.ColumnHeader();
            this.ContentCategoryFlags = new System.Windows.Forms.ColumnHeader();
            this.TGI = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.catlgName,
            this.TagID,
            this.ContentCategoryFlags,
            this.TGI});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.FullRowSelect = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(852, 150);
            this.listView1.TabIndex = 1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.ItemActivate += new System.EventHandler(this.listView1_ItemActivate);
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            this.listView1.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView1_ColumnClick);
            // 
            // catlgName
            // 
            this.catlgName.Text = "Name";
            this.catlgName.Width = 275;
            // 
            // TagID
            // 
            this.TagID.Text = "Tag";
            // 
            // ContentCategoryFlags
            // 
            this.ContentCategoryFlags.Text = "Cont. Cat.";
            this.ContentCategoryFlags.Width = 77;
            // 
            // TGI
            // 
            this.TGI.Text = "Resource Key";
            this.TGI.Width = 246;
            // 
            // ObjectChooser
            // 
            this.Controls.Add(this.listView1);
            this.Name = "ObjectChooser";
            this.Size = new System.Drawing.Size(852, 150);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader catlgName;
        private System.Windows.Forms.ColumnHeader TGI;
        private System.Windows.Forms.ColumnHeader TagID;
        private System.Windows.Forms.ColumnHeader ContentCategoryFlags;

    }
}
