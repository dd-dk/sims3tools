/***************************************************************************
 *  Copyright (C) 2009 by Peter L Jones                                    *
 *  pljones@users.sf.net                                                   *
 *                                                                         *
 *  This file is part of the Sims 3 Package Interface (s3pi)               *
 *                                                                         *
 *  s3pi is free software: you can redistribute it and/or modify           *
 *  it under the terms of the GNU General Public License as published by   *
 *  the Free Software Foundation, either version 3 of the License, or      *
 *  (at your option) any later version.                                    *
 *                                                                         *
 *  s3pi is distributed in the hope that it will be useful,                *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of         *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the          *
 *  GNU General Public License for more details.                           *
 *                                                                         *
 *  You should have received a copy of the GNU General Public License      *
 *  along with s3pi.  If not, see <http://www.gnu.org/licenses/>.          *
 ***************************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using s3pi.Interfaces;

namespace S3PIDemoFE
{
    public partial class BrowserWidget : UserControl
    {
        #region Attributes
        IList<string> fields = null;
        ProgressBar pb = null;
        IPackage pkg = null;
        ListViewColumnSorter lvwColumnSorter;
        string sortColumn = "Chunkoffset";
        bool displayResourceNames = false;
        IList<KeyValuePair<string, TypedValue>> filter = null;

        //used internally
        Dictionary<IResourceIndexEntry, ListViewItem> lookup = null;
        IList<IResourceIndexEntry> resourceList = null;
        IResource nameMap = null;
        ListViewItem selectedResource = null;
        bool internalchg = false;
        #endregion

        public BrowserWidget()
        {
            InitializeComponent();
            lvwColumnSorter = new ListViewColumnSorter();
            lookup = new Dictionary<IResourceIndexEntry, ListViewItem>();
            OnListUpdated(this, new EventArgs());
        }

        #region Methods
        public void Add(IResourceIndexEntry rie)
        {
            ListViewItem lvi = CreateItem(rie);
            if (lvi == null) return;

            listView1.BeginUpdate();
            listView1.Items.Add(lvi);
            lookup.Add(rie, lvi);
            SelectedResource = rie;
            listView1.EndUpdate();

            if (rie.ResourceType == 0x0166038C) { nameMap = null; setNameMap(true); }
        }

        public string ResourceName(IResourceIndexEntry rie)
        {
            if (!displayResourceNames) return "";
            if (nameMap == null) return "";
            IDictionary<ulong, string> nmap = (IDictionary<ulong, string>)nameMap;
            return (nmap.ContainsKey(rie.Instance)) ? nmap[rie.Instance] : "";
        }
        #endregion

        #region Properties
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Count of items in the widget")]
        public int Count { get { return listView1.Items.Count; } }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Specifies the list of fields to display")]
        public IList<string> Fields
        {
            get { return fields; }
            set
            {
                if (fields == value) return;
                fields = value;
                SetColumns();
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("The progress bar is used when loading the displayed list")]
        public ProgressBar ProgressBar { get { return pb; } set { pb = value; } }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("The current package")]
        public IPackage Package
        {
            get { return null; }
            set
            {
                if (pkg == value) return;
                if (pkg != null)
                    pkg.ResourceIndexInvalidated -= new EventHandler(pkg_ResourceIndexInvalidated);
                pkg = value;
                pkg_ResourceIndexInvalidated(null, null);
                if (pkg != null)
                    pkg.ResourceIndexInvalidated += new EventHandler(pkg_ResourceIndexInvalidated);
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("When true, clicking a column heading will sort on that column")]
        public bool Sortable
        {
            get { return this.listView1.ListViewItemSorter != null; }
            set
            {
                listView1.BeginUpdate();
                Application.UseWaitCursor = true;
                Application.DoEvents();
                try
                {
                    this.listView1.ListViewItemSorter = value ? lvwColumnSorter : null;
                    listView1.Sorting = value ? SortOrder.Ascending : SortOrder.None;
                    listView1.HeaderStyle = value ? ColumnHeaderStyle.Clickable : ColumnHeaderStyle.Nonclickable;
                }
                finally { Application.UseWaitCursor = false; Application.DoEvents(); listView1.EndUpdate(); }
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue("Chunkoffset")]
        [Description("Specifies the column to sort on by default")]
        public string SortColumn
        {
            get { return sortColumn; }
            set
            {
                if (sortColumn == value) return;
                if (fields.IndexOf(sortColumn) >= 0)
                {
                    sortColumn = value;
                    ColumnClickEventArgs e = new ColumnClickEventArgs(fields.IndexOf(sortColumn));
                    listView1_ColumnClick(this, e);
                }
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("Set to true to display resource names found in the package")]
        public bool DisplayResourceNames
        {
            get { return displayResourceNames; }
            set
            {
                if (displayResourceNames == value) return;
                displayResourceNames = value;
                if (pkg == null) return;

                if (displayResourceNames) setNameMap(true);
                else listView1.Columns[0].Width = 0;
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("The single resource selected")]
        public IResourceIndexEntry SelectedResource
        {
            get { return selectedResource == null ? null : selectedResource.Tag as IResourceIndexEntry; }
            set
            {
                ListViewItem lvi = value != null && lookup != null && lookup.ContainsKey(value) ? lookup[value] : null;
                if (listView1.SelectedItems.Count == 0 && value == null) return;
                if (listView1.SelectedItems.Count == 1 && listView1.SelectedItems[0] == value) return;

                internalchg = true;
                listView1.SelectedItems.Clear();
                if (lvi != null) listView1.SelectedIndices.Add(listView1.Items.IndexOf(lvi));
                if (listView1.SelectedItems.Count == 1) listView1.SelectedItems[0].EnsureVisible();
                internalchg = false;
                listView1_SelectedIndexChanged(null, null);
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("The list of resources selected")]
        public IList<IResourceIndexEntry> SelectedResources
        {
            get
            {
                List<IResourceIndexEntry> res = new List<IResourceIndexEntry>();
                foreach (ListViewItem lvi in listView1.SelectedItems)
                    if (lvi.Tag as IResourceIndexEntry != null) res.Add(lvi.Tag as IResourceIndexEntry);
                return res;
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Specify how to filter the package resource list")]
        public IList<KeyValuePair<string, TypedValue>> Filter
        {
            get { return filter; }
            set
            {
                if (filter == null && value == null) return;
                filter = value;
                pkg_ResourceIndexInvalidated(null, null);
            }
        }
        #endregion

        #region Events
        [Browsable(true)]
        [Category("Property Changed")]
        [Description("Raised when the list content is updated")]
        public event EventHandler ListUpdated;

        [Browsable(true)]
        [Category("Property Changed")]
        [Description("Raised when an item is activated")]
        public event EventHandler ItemActivate;

        [Browsable(true)]
        [Category("Property Changed")]
        [Description("Raised before the selection changes")]
        public event EventHandler<ResourceChangingEventArgs> SelectedResourceChanging;

        [Browsable(true)]
        [Category("Property Changed")]
        [Description("Raised when the selection changes")]
        public event EventHandler<ResourceChangedEventArgs> SelectedResourceChanged;

        [Browsable(true)]
        [Category("Property Changed")]
        [Description("Raised when the user presses DELETE with a resource selected")]
        public event EventHandler SelectedResourceDeleted;
        #endregion

        #region Sub-classes
        /// <summary>
        /// Passes the name of the currently select resource and allows handlers to cancel the change
        /// </summary>
        public class ResourceChangingEventArgs : EventArgs
        {
            public readonly string name;
            public bool Cancel = false;
            public ResourceChangingEventArgs(ListViewItem lvi) { name = lvi == null ? "" : lvi.Text; }
        }

        /// <summary>
        /// Passes the name of the newly selected resource
        /// </summary>
        public class ResourceChangedEventArgs : EventArgs
        {
            public readonly string name;
            public ResourceChangedEventArgs(ListViewItem lvi) { name = lvi == null ? "" : lvi.Text; }
        }
        #endregion


        void SetColumns()
        {
            listView1.Columns.Clear();
            if (fields == null) return;
            // Create an instance of a ListView column sorter and assign it 
            // to the ListView control.
            //

            ColumnHeader[] ach = new ColumnHeader[fields.Count + 1];
            ach[0] = new ColumnHeader();
            ach[0].DisplayIndex = 0;
            ach[0].Text = ach[0].Name = "Name";
            ach[0].Width = displayResourceNames ? 80 : 0;
            listView1.Columns.Add(ach[0]);
            for (int i = 1; i < ach.Length; i++)
            {
                ach[i] = new ColumnHeader();
                ach[i].DisplayIndex = i;
                ach[i].Text = ach[i].Name = fields[i - 1].Replace("Resource", "");
                ach[i].Width = 80;
                listView1.Columns.Add(ach[i]);
            }
        }

        void UpdateList()
        {
            listView1.Columns[0].Width = displayResourceNames && nameMap != null ? 80 : 0;
            IResourceIndexEntry sie = (listView1.SelectedItems.Count == 0 ? null : listView1.SelectedItems[0].Tag) as IResourceIndexEntry;
            System.Collections.IComparer cmp = this.listView1.ListViewItemSorter;

            listView1.BeginUpdate();
            Application.UseWaitCursor = true;
            Application.DoEvents();
            this.listView1.ListViewItemSorter = null;
            try
            {
                listView1.Items.Clear();
                lookup = new Dictionary<IResourceIndexEntry, ListViewItem>();
                if (resourceList == null) return;

                if (pb != null)
                {
                    pb.Maximum = resourceList.Count / 100;
                    pb.Value = 0;
                }
                try
                {
                    int i = 0;
                    foreach (IResourceIndexEntry ie in resourceList)
                    {
                        try
                        {
                            ListViewItem lvi = CreateItem(ie);
                            if (lvi == null) continue;
                            listView1.Items.Add(lvi);
                            lookup.Add(ie, lvi);
                        }
                        finally
                        {
                            i++;
                            if (i % 100 == 0) { if (pb != null) pb.Value++; Application.DoEvents(); }
                        }
                    }
                }
                finally { if (pb != null) pb.Value = 0; }
            }
            finally
            {
                this.listView1.ListViewItemSorter = cmp;
                Application.UseWaitCursor = false;
                Application.DoEvents();
                if (sie != null && lookup.ContainsKey(sie))
                {
                    listView1.SelectedIndices.Add(listView1.Items.IndexOf(lookup[sie]));
                    if (listView1.SelectedIndices.Count > 0)
                        listView1.SelectedItems[0].EnsureVisible();
                }
                SelectedResource = sie;
                listView1.EndUpdate();
                OnListUpdated(this, new EventArgs());
            }
        }

        ListViewItem CreateItem(IResourceIndexEntry ie)
        {
            ListViewItem lvi = new ListViewItem();
            if (ie.IsDeleted) lvi.Font = new Font(lvi.Font, FontStyle.Strikeout);
            for (int j = 0; j < fields.Count + 1; j++)
            {
                if (j == 0)
                    lvi.Text = ResourceName(ie);
                else
                    lvi.SubItems.Add(ie[fields[j - 1]] + "");
            }
            lvi.Tag = ie;
            (ie as AResourceIndexEntry).ResourceIndexEntryChanged -= new EventHandler(BrowserWidget_ResourceIndexEntryChanged);
            (ie as AResourceIndexEntry).ResourceIndexEntryChanged += new EventHandler(BrowserWidget_ResourceIndexEntryChanged);
            return lvi;
        }

        void setNameMap(bool updateNames)
        {
            if (displayResourceNames && pkg != null)
            {
                if (nameMap == null)
                {
                    IResourceIndexEntry rie = pkg.Find(new string[] { "ResourceType" }, new TypedValue[] { new TypedValue(typeof(uint), (uint)0x0166038C) });
                    if (rie != null) nameMap = s3pi.WrapperDealer.WrapperDealer.GetResource(0, pkg, rie);
                    if (nameMap == null) return;
                    nameMap.ResourceChanged += new EventHandler(nameMap_ResourceChanged);
                }
                if (updateNames) nameMap_ResourceChanged(null, null);
            }
        }


        void ILKvpToListList(IList<KeyValuePair<string, TypedValue>> ilkvp, out List<string> keys, out List<TypedValue> values)
        {
            keys = new List<string>();
            values = new List<TypedValue>();
            if (ilkvp != null)
                foreach (KeyValuePair<string, TypedValue> kvp in ilkvp) { keys.Add(kvp.Key); values.Add(kvp.Value); }
        }


        protected virtual void OnListUpdated(object sender, EventArgs e) { if (ListUpdated != null) ListUpdated(sender, e); }

        protected virtual void OnItemActivate(object sender, EventArgs e) { if (ItemActivate != null) ItemActivate(sender, e); }

        protected virtual void OnSelectedResourceChanging(object sender, ResourceChangingEventArgs e) { if (SelectedResourceChanging != null) SelectedResourceChanging(sender, e); }

        protected virtual void OnSelectedResourceChanged(object sender, ResourceChangedEventArgs e) { if (SelectedResourceChanged != null) SelectedResourceChanged(sender, e); }

        protected virtual void OnSelectedResourceDeleted(object sender, EventArgs e) { if (SelectedResourceDeleted != null) SelectedResourceDeleted(sender, e); }


        private void listView1_ItemActivate(object sender, EventArgs e) { OnItemActivate(sender, e); }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (internalchg) return;

            if (selectedResource != null)
            {
                ResourceChangingEventArgs rcea = new ResourceChangingEventArgs(selectedResource);
                OnSelectedResourceChanging(this, rcea);
                if (rcea.Cancel)
                {
                    internalchg = true;
                    listView1.SelectedItems.Clear();
                    listView1.SelectedIndices.Add(listView1.Items.IndexOf(selectedResource));
                    listView1.SelectedItems[0].EnsureVisible();
                    internalchg = false;
                    return;
                }
            }

            selectedResource = (listView1.SelectedItems.Count == 1) ? listView1.SelectedItems[0] : null;

            OnSelectedResourceChanged(this, new ResourceChangedEventArgs(selectedResource));
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            listView1.BeginUpdate();
            Application.UseWaitCursor = true;
            Application.DoEvents();
            try
            {
                this.listView1.Sort();
            }
            finally { Application.UseWaitCursor = false; Application.DoEvents(); listView1.EndUpdate(); }
            if (listView1.SelectedIndices.Count > 0)
                listView1.SelectedItems[0].EnsureVisible();
        }

        private void listView1_KeyUp(object sender, KeyEventArgs e)
        {
            if (listView1.SelectedIndices.Count == 0) return;
            switch (e.KeyCode)
            {
                case System.Windows.Forms.Keys.Delete:
                    e.Handled = true;
                    OnSelectedResourceDeleted(this, new EventArgs());
                    break;
            }
        }

        private void BrowserWidget_ResourceIndexEntryChanged(object sender, EventArgs e)
        {
            IResourceIndexEntry rie = sender as IResourceIndexEntry;
            if (rie == null) return;
            if (!lookup.ContainsKey(rie)) return;

            int i = listView1.Items.IndexOf(lookup[rie]);
            bool isSelected = listView1.SelectedIndices.Contains(i);
            bool isFocused = listView1.Items[i].Focused;

            internalchg = true;
            listView1.Items[i] = CreateItem(rie);
            listView1.Items[i].Selected = isSelected;
            listView1.Items[i].Focused = isFocused;
            internalchg = false;

            lookup[rie] = listView1.Items[i];
            if (rie.ResourceType == 0x0166038C) { nameMap = null; setNameMap(true); }
        }

        private void nameMap_ResourceChanged(object sender, EventArgs e)
        {
            if (!displayResourceNames) return;
            UpdateList();
#if this_was_faster_it_would_get_used_but_unbelievably_it_is_not
            bool hasNames = false;
            if (pb != null)
            {
                pb.Maximum = lookup.Count / 100;
                pb.Value = 0;
            }
            try
            {
                int i = 0;
                foreach (KeyValuePair<IResourceIndexEntry,ListViewItem> kvp in lookup)
                {
                    try
                    {
                        kvp.Value.Text = ResourceName(kvp.Key);
                        hasNames = hasNames || kvp.Value.Text.Length > 0;
                    }
                    finally
                    {
                        i++;
                        if (i % 100 == 0) { if (pb != null) pb.Value++; Application.DoEvents(); }
                    }
                }
            }
            finally { if (pb != null) pb.Value = 0; }
            listView1.Columns[0].Width = hasNames ? 80 : 0;
#endif
        }

        private void pkg_ResourceIndexInvalidated(object sender, EventArgs e)
        {
            nameMap = null;
            setNameMap(false);

            List<string> keys;
            List<TypedValue> values;
            ILKvpToListList(filter, out keys, out values);

            resourceList = pkg == null ? null : filter != null ? pkg.FindAll(keys.ToArray(), values.ToArray()) : pkg.GetResourceList;

            UpdateList();
            if (resourceList == null) return;

            listView1.BeginUpdate();
            if (listView1.Items.Count > 0)
                for (int i = 1; i < listView1.Columns.Count; i++)
                    listView1.Columns[i].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            listView1.EndUpdate();
        }
    }
}
