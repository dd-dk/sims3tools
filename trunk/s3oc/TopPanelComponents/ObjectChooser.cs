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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ObjectCloner.TopPanelComponents
{
    public partial class ObjectChooser : UserControl
    {
        ListViewColumnSorter lvwColumnSorter;
        public ObjectChooser()
        {
            InitializeComponent();
            lvwColumnSorter = new ListViewColumnSorter();
            this.listView1.ListViewItemSorter = lvwColumnSorter;
            ObjectChooser_LoadListViewSettings();
        }

        public void ObjectChooser_LoadListViewSettings()
        {
            string cw = ObjectCloner.Properties.Settings.Default.ColumnWidths;
            string[] cws = cw == null ? new string[] { } : cw.Split(':');

            int w;
            listView1.Columns[0].Width = cws.Length > 0 && int.TryParse(cws[0], out w) && w > 0 ? w : (this.listView1.Width - 32) / 3;
            listView1.Columns[1].Width = cws.Length > 1 && int.TryParse(cws[1], out w) && w > 0 ? w : (this.listView1.Width - 32) / 6;
            listView1.Columns[2].Width = cws.Length > 2 && int.TryParse(cws[2], out w) && w > 0 ? w : (this.listView1.Width - 32) / 6;
            listView1.Columns[3].Width = cws.Length > 3 && int.TryParse(cws[3], out w) && w > 0 ? w : (this.listView1.Width - 32) / 3;
        }
        public void ObjectChooser_SaveSettings()
        {
            lvwColumnSorter.ListViewColumnSorter_SaveSettings();
            ObjectCloner.Properties.Settings.Default.ColumnWidths = string.Format("{0}:{1}:{2}:{3}"
                , listView1.Columns[0].Width
                , listView1.Columns[1].Width
                , listView1.Columns[2].Width
                , listView1.Columns[3].Width
                );
        }

        #region ListViewColumnSorter
        /// <summary>
        /// This class is an implementation of the 'IComparer' interface.
        /// </summary>
        public class ListViewColumnSorter : IComparer
        {
            /// <summary>
            /// Specifies the column to be sorted
            /// </summary>
            private int ColumnToSort;
            /// <summary>
            /// Specifies the order in which to sort (i.e. 'Ascending').
            /// </summary>
            private SortOrder OrderOfSort;
            /// <summary>
            /// Case insensitive comparer object
            /// </summary>
            private CaseInsensitiveComparer ObjectCompare;

            /// <summary>
            /// Class constructor.  Initializes various elements
            /// </summary>
            public ListViewColumnSorter()
            {
                // Initialize the CaseInsensitiveComparer object
                ObjectCompare = new CaseInsensitiveComparer();

                ListViewColumnSorter_LoadSettings();
            }

            void ListViewColumnSorter_LoadSettings()
            {
                ColumnToSort = ObjectCloner.Properties.Settings.Default.ColumnToSort;

                OrderOfSort = Enum.IsDefined(typeof(SortOrder), ObjectCloner.Properties.Settings.Default.SortOrder)
                    ? (SortOrder)ObjectCloner.Properties.Settings.Default.SortOrder
                    : SortOrder.None;
            }

            public void ListViewColumnSorter_SaveSettings()
            {
                ObjectCloner.Properties.Settings.Default.ColumnToSort = ColumnToSort;
                ObjectCloner.Properties.Settings.Default.SortOrder = (int)OrderOfSort;
            }

            /// <summary>
            /// This method is inherited from the IComparer interface.  It compares the two objects passed using a case insensitive comparison.
            /// </summary>
            /// <param name="x">First object to be compared</param>
            /// <param name="y">Second object to be compared</param>
            /// <returns>The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'</returns>
            public int Compare(object x, object y)
            {
                if (ColumnToSort < 0 || ColumnToSort > ((ListViewItem)x).SubItems.Count) return 0;

                int compareResult = 0;
                ListViewItem listviewX, listviewY;

                // Cast the objects to be compared to ListViewItem objects
                listviewX = (ListViewItem)x;
                listviewY = (ListViewItem)y;

                // Compare the two items
                if (ColumnToSort == 3)
                    compareResult = (listviewX.Tag as Item).RequestedRK.Compare((listviewY.Tag as Item).RequestedRK);
                else
                    compareResult = ObjectCompare.Compare(listviewX.SubItems[ColumnToSort].Text, listviewY.SubItems[ColumnToSort].Text);

                // Calculate correct return value based on object comparison
                if (OrderOfSort == SortOrder.Ascending)
                {
                    // Ascending sort is selected, return normal result of compare operation
                    return compareResult;
                }
                else if (OrderOfSort == SortOrder.Descending)
                {
                    // Descending sort is selected, return negative result of compare operation
                    return (-compareResult);
                }
                else
                {
                    // Return '0' to indicate they are equal
                    return 0;
                }
            }

            /// <summary>
            /// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
            /// </summary>
            public int SortColumn
            {
                set
                {
                    ColumnToSort = value;
                }
                get
                {
                    return ColumnToSort;
                }
            }

            /// <summary>
            /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
            /// </summary>
            public SortOrder Order
            {
                set
                {
                    OrderOfSort = value;
                }
                get
                {
                    return OrderOfSort;
                }
            }

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
            this.listView1.Sort();

            if (listView1.SelectedIndices.Count > 0)
                listView1.SelectedItems[0].EnsureVisible();
        }
        #endregion

        public event EventHandler SelectedIndexChanged;
        protected void OnSelectedIndexChanged(object sender, EventArgs e) { if (SelectedIndexChanged != null) SelectedIndexChanged(sender, e); }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e) { OnSelectedIndexChanged(sender, e); }

        public View View
        {
            get { return listView1.View; }
            set { listView1.View = value; }
        }

        public ImageList SmallImageList
        {
            get { return listView1.SmallImageList; }
            set { listView1.SmallImageList = value; }
        }

        public ImageList LargeImageList
        {
            get { return listView1.LargeImageList; }
            set { listView1.LargeImageList = value; }
        }

        public ListView.ListViewItemCollection Items
        {
            get { return listView1.Items; }
        }

        public ListView.SelectedListViewItemCollection SelectedItems
        {
            get { return listView1.SelectedItems; }
        }

        public ListViewItem SelectedItem { get { return listView1.SelectedItems.Count == 1 ? listView1.SelectedItems[0] : null; } set { listView1.SelectedItems.Clear(); if (listView1.Items.Contains(value)) value.Selected = true; } }

        public int SelectedIndex { get { return listView1.SelectedIndices.Count == 1 ? listView1.SelectedIndices[0] : -1; } set { listView1.SelectedIndices.Clear(); if (value >= 0 && value < listView1.Items.Count)listView1.SelectedIndices.Add(value); } }

        public event EventHandler ItemActivate;
        protected void OnItemActivate(object sender, EventArgs e) { if (ItemActivate != null) ItemActivate(sender, e); }
        private void listView1_ItemActivate(object sender, EventArgs e) { OnItemActivate(sender, e); }

        private void omCopyRK_Click(object sender, EventArgs e)
        {
            if (SelectedItem != null && SelectedItem.Tag as Item != null)
                Clipboard.SetText((SelectedItem.Tag as Item).SpecificRK + "");
        }

        private void ocContextMenu_Opening(object sender, CancelEventArgs e)
        {
            omCopyRK.Enabled = SelectedItem != null;
        }
    }
}
