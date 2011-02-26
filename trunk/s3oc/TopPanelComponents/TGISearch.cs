/***************************************************************************
 *  Copyright (C) 2011 by Peter L Jones                                    *
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
using System.Threading;
using s3pi.Interfaces;

namespace ObjectCloner.TopPanelComponents
{
    public partial class TGISearch : UserControl
    {
        ListViewColumnSorter lvwColumnSorter;

        List<List<IPackage>> pkgLists;
        List<List<string>> pathLists;
        MainForm.updateProgressCallback updateProgressCB;
        MainForm.NameMap nameMap;

        public TGISearch()
        {
            InitializeComponent();
            lvwColumnSorter = new ListViewColumnSorter();
            listView1.ListViewItemSorter = lvwColumnSorter;

            cbResourceType.Value = 0;
            tbResourceGroup.Text = "0x00000000";
            tbInstance.Text = "0x0000000000000000";
        }

        public TGISearch(List<List<IPackage>> pkgLists, List<List<string>> pathLists, MainForm.updateProgressCallback updateProgressCB, MainForm.NameMap nameMap)
            : this()
        {
            this.pkgLists = pkgLists;
            this.pathLists = pathLists;
            this.updateProgressCB = updateProgressCB;
            this.nameMap = nameMap;
        }

        #region Search criteria event handlers
        private void ckbResourceType_CheckedChanged(object sender, EventArgs e)
        {
            cbResourceType.Enabled = !ckbResourceType.Checked;
            allowSearch();
        }

        private void ckbResourceGroup_CheckedChanged(object sender, EventArgs e)
        {
            tbResourceGroup.Enabled = !ckbResourceGroup.Checked;
            allowSearch();
        }

        private void ckbInstance_CheckedChanged(object sender, EventArgs e)
        {
            tbInstance.Enabled = !ckbInstance.Checked;
            allowSearch();
        }

        private void allowSearch()
        {
            btnSearch.Enabled = !ckbResourceType.Checked || !ckbResourceGroup.Checked || !ckbInstance.Checked;
        }

        private void tbResourceGroup_Validating(object sender, CancelEventArgs e)
        {
            string s = tbResourceGroup.Text.ToLower();
            uint group;
            if (s.StartsWith("0x"))
            {
                if (!uint.TryParse(s.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out group)) e.Cancel = true;
            }
            else
            {
                if (!uint.TryParse(s, out group)) e.Cancel = true;
            }
        }

        private void tbInstance_Validating(object sender, CancelEventArgs e)
        {
            string s = tbResourceGroup.Text.ToLower();
            ulong instance;
            if (s.StartsWith("0x"))
            {
                if (!ulong.TryParse(s.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out instance)) e.Cancel = true;
            }
            else
            {
                if (!ulong.TryParse(s, out instance)) e.Cancel = true;
            }
        }
        #endregion

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (searching)
            {
                AbortSearch();
            }
            else
            {
                tgiSearchContextMenu.Enabled = listView1.Enabled = tlpTGIValues.Enabled = false;
                btnSearch.Text = "&Stop";
                StartSearch();
            }
        }

        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 1) return;

            ListViewItem lvi = listView1.SelectedItems[0];
            
            CopyableMessageBox.Show(String.Format(
                "Name: {0}\n"+
                "Tag: {1}\n"+
                "EP/SP: {2}\n"+
                "ResourceKey: {3}\n"+
                "Path: {4}",
                lvi.SubItems[0].Text, lvi.SubItems[1].Text, lvi.SubItems[2].Text, lvi.SubItems[3].Text, lvi.SubItems[4].Text
                ),
                "Selected item");
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

                ColumnToSort = 3;

                OrderOfSort = SortOrder.None;
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

        delegate void AddCallBack(IResourceKey rk, string path);
        void Add(IResourceKey rk, string path)
        {
            if (!this.IsHandleCreated) return;

            string name = nameMap[rk.Instance];
            if (name == null) name = "";

            List<string> exts;
            string tag = "";
            if (s3pi.Extensions.ExtList.Ext.TryGetValue("0x" + rk.ResourceType.ToString("X8"), out exts)) tag = exts[0];
            else tag = "UNKN";

            listView1.Items.Add(new ListViewItem(new string[] {
                name, tag, Item.EPSP((byte)(rk.ResourceGroup >> 27)), "" + (AResourceKey)rk, path,
            }) { Tag = rk, });
        }

        Thread searchThread;
        bool searching = false;
        public delegate bool stopSearchCallback();
        private bool stopSearch() { return !searching; }

        event EventHandler<MainForm.BoolEventArgs> SearchComplete;
        delegate void searchCompleteCallback(bool complete);
        void OnSearchComplete(bool complete) { if (SearchComplete != null) { SearchComplete(this, new MainForm.BoolEventArgs(complete)); } }

        private void AbortSearch()
        {
            if (!searching) OnSearchComplete(false); // Should never happen
            else searching = false;
        }

        TGISearchThread.Criteria GetCriteria()
        {
            TGISearchThread.Criteria criteria = new TGISearchThread.Criteria();
            criteria.useResourceType = !ckbResourceType.Checked;
            if (criteria.useResourceType) criteria.resourceType = cbResourceType.Value;
            criteria.useResourceGroup = !ckbResourceGroup.Checked;
            if (criteria.useResourceGroup)
            {
                string s = tbResourceGroup.Text.ToLower();
                if (s.StartsWith("0x"))
                {
                    criteria.resourceGroup = uint.Parse(s.Substring(2), System.Globalization.NumberStyles.HexNumber, null);
                }
                else
                {
                    criteria.resourceGroup = uint.Parse(s);
                }
            }
            criteria.useInstance = !ckbInstance.Checked;
            if (criteria.useInstance)
            {
                string s = tbInstance.Text.ToLower();
                if (s.StartsWith("0x"))
                {
                    criteria.instance = ulong.Parse(s.Substring(2), System.Globalization.NumberStyles.HexNumber, null);
                }
                else
                {
                    criteria.instance = ulong.Parse(s);
                }
            }
            return criteria;
        }

        private void StartSearch()
        {
            listView1.Items.Clear();

            TGISearchThread st = new TGISearchThread(this, GetCriteria(), pkgLists, pathLists,
                Add, updateProgressCB, stopSearch, OnSearchComplete);
            SearchComplete += new EventHandler<MainForm.BoolEventArgs>(TGISearch_SearchComplete);

            searchThread = new Thread(new ThreadStart(st.Search));
            searching = true;
            searchThread.Start();
        }

        void TGISearch_SearchComplete(object sender, MainForm.BoolEventArgs e)
        {
            searching = false;
            while (searchThread != null && searchThread.IsAlive)
                searchThread.Join(100);
            searchThread = null;

            updateProgressCB(true, "", true, -1, false, 0);

            tgiSearchContextMenu.Enabled = listView1.Enabled = tlpTGIValues.Enabled = true;
            btnSearch.Text = "&Search";
        }

        class TGISearchThread
        {
            public struct Criteria
            {
                public bool useResourceType;
                public uint resourceType;
                public bool useResourceGroup;
                public uint resourceGroup;
                public bool useInstance;
                public ulong instance;
            }

            Control control;
            Criteria criteria;
            List<List<IPackage>> pkgLists;
            List<List<string>> pathLists;
            AddCallBack addCB;
            MainForm.updateProgressCallback updateProgressCB;
            stopSearchCallback stopSearchCB;
            searchCompleteCallback searchCompleteCB;
            public TGISearchThread(Control control, Criteria criteria, List<List<IPackage>> pkgLists, List<List<string>> pathLists,
                AddCallBack addCB, MainForm.updateProgressCallback updateProgressCB, stopSearchCallback stopSearchCB, searchCompleteCallback searchCompleteCB)
            {
                this.control = control;
                this.criteria = criteria;
                this.pkgLists = pkgLists;
                this.pathLists = pathLists;
                this.addCB = addCB;
                this.updateProgressCB = updateProgressCB;
                this.stopSearchCB = stopSearchCB;
                this.searchCompleteCB = searchCompleteCB;
            }

            public void Search()
            {
                bool complete = false;
                DateTime now = DateTime.UtcNow;

                try
                {
                    int searchCount = 0;
                    int searched = 0;

                    pkgLists.ForEach(pkgList => searchCount += pkgList.Count);
                    updateProgress(true, "Searching " + searchCount + " packages... 0%", true, searchCount, true, 0);

                    for (int i = 0; i < pkgLists.Count; i++)
                    {
                        List<IPackage> pkgs = pkgLists[i];
                        List<string> paths = pathLists[i];

                        for (int j = 0; j < pkgs.Count; j++)
                        {
                            IPackage pkg = pkgs[j];
                            string path = paths[j];

                            if (stopSearch) return;

                            pkg.FindAll(rie =>
                            {
                                if (stopSearch) return false;

                                bool match = true;
                                if (match && criteria.useResourceType) match = rie.ResourceType.Equals(criteria.resourceType);
                                if (match && criteria.useResourceGroup) match = rie.ResourceGroup.Equals(criteria.resourceGroup);
                                if (match && criteria.useInstance) match = rie.Instance.Equals(criteria.instance);
                                if (match)
                                {
                                    Add(rie, path);
                                }
                                return match;
                            });

                            if (now.AddMilliseconds(100) < DateTime.UtcNow)
                                updateProgress(true, "Searching " + searchCount + " packages... " + (++searched * 100 / searchCount) + "%", false, -1, true, searched);
                        }
                    }

                    complete = true;
                }
                catch (ThreadInterruptedException) { }
                finally
                {
                    updateProgress(true, "Search ended", true, -1, false, 0);
                    searchComplete(complete);
                }
            }

            #region Callbacks
            void Add(IResourceIndexEntry rie, string path) { Thread.Sleep(0); if (control.IsHandleCreated) control.Invoke(addCB, new object[] { rie, path, }); }

            void updateProgress(bool changeText, string text, bool changeMax, int max, bool changeValue, int value) { Thread.Sleep(0); if (control.IsHandleCreated) control.Invoke(updateProgressCB, new object[] { changeText, text, changeMax, max, changeValue, value, }); }

            bool stopSearch { get { Thread.Sleep(0); return !control.IsHandleCreated || (bool)control.Invoke(stopSearchCB); } }

            void searchComplete(bool complete) { Thread.Sleep(0); if (control.IsHandleCreated) control.BeginInvoke(searchCompleteCB, new object[] { complete, }); }
            #endregion
        }

        private void tgiSearchContextMenu_Opening(object sender, CancelEventArgs e)
        {
            tgisCopyRK.Enabled = listView1.SelectedItems.Count == 1 && listView1.SelectedItems[0].Tag as AResourceKey != null;

            IResourceKey rk;
            tgisPasteRK.Enabled = Clipboard.ContainsText() && RK.TryParse(Clipboard.GetText(), out rk);
        }

        private void tgisCopyRK_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems[0] != null && listView1.SelectedItems[0].Tag as AResourceKey != null)
                Clipboard.SetText((listView1.SelectedItems[0].Tag as AResourceKey) + "");
        }

        private void tgisPasteRK_Click(object sender, EventArgs e)
        {
            IResourceKey rk;
            if (RK.TryParse(Clipboard.GetText(), out rk))
            {
                cbResourceType.Value = rk.ResourceType;
                tbResourceGroup.Text = "0x" + rk.ResourceGroup.ToString("X8");
                tbInstance.Text = "0x" + rk.Instance.ToString("X16");
            }
        }
    }
}
