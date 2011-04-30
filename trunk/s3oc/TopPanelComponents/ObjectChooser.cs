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
using System.Threading;
using System.Windows.Forms;
using s3pi.Interfaces;

namespace ObjectCloner.TopPanelComponents
{
    public partial class ObjectChooser : UserControl
    {

        ListViewColumnSorter lvwColumnSorter;

        MainForm.updateProgressCallback updateProgressCB;
        MainForm.listViewAddCallBack listViewAddCB;
        public CatalogType ResourceType { get; private set; }
        bool isFix;

        public ObjectChooser()
        {
            InitializeComponent();
            lvwColumnSorter = new ListViewColumnSorter();
            this.listView1.ListViewItemSorter = lvwColumnSorter;
            ObjectChooser_LoadListViewSettings();
            this.Load += new EventHandler(ObjectChooser_Load);
        }

        public ObjectChooser(MainForm.updateProgressCallback updateProgressCB, MainForm.listViewAddCallBack listViewAddCB, CatalogType resourceType, bool isFix)
            : this()
        {
            ResourceType = resourceType;
            this.updateProgressCB = updateProgressCB;
            this.listViewAddCB = listViewAddCB;
            this.isFix = isFix;
        }

        void ObjectChooser_Load(object sender, EventArgs e)
        {
            if (filling) { AbortFilling(false); }//this can never happen
            StartFilling();
        }

        void ObjectChooser_LoadListViewSettings()
        {
            string cw = ObjectCloner.Properties.Settings.Default.ColumnWidths;
            string[] cws = cw == null ? new string[] { } : cw.Split(':');

            int w;
            listView1.Columns[0].Width = cws.Length > 0 && int.TryParse(cws[0], out w) && w > 0 ? w : (this.listView1.Width - 32) / 3;
            listView1.Columns[1].Width = cws.Length > 1 && int.TryParse(cws[1], out w) && w > 0 ? w : (this.listView1.Width - 32) / 6;
            listView1.Columns[2].Width = cws.Length > 2 && int.TryParse(cws[2], out w) && w > 0 ? w : (this.listView1.Width - 32) / 6;
            listView1.Columns[3].Width = cws.Length > 3 && int.TryParse(cws[3], out w) && w > 0 ? w : (this.listView1.Width - 32) / 3;
        }
        public void ObjectChooser_SaveListViewSettings()
        {
            lvwColumnSorter.ListViewColumnSorter_SaveSettings();
            ObjectCloner.Properties.Settings.Default.ColumnWidths = string.Format("{0}:{1}:{2}:{3}"
                , listView1.Columns[0].Width
                , listView1.Columns[1].Width
                , listView1.Columns[2].Width
                , listView1.Columns[3].Width
                );
        }


        public ListViewItem SelectedItem { get { return listView1.SelectedItems.Count == 1 ? listView1.SelectedItems[0] : null; } set { listView1.SelectedItems.Clear(); if (listView1.Items.Contains(value)) try { value.Selected = true; } catch { } } }

        #region Occurs whenever the 'SelectedIndex' for the Object Chooser changes
        [Browsable(true)]
        [Category("Action")]
        [Description("Occurs whenever the 'SelectedIndex' for the Object Chooser changes")]
        public event EventHandler<MainForm.SelectedIndexChangedEventArgs> SelectedIndexChanged;
        protected virtual void OnSelectedIndexChanged(object sender, MainForm.SelectedIndexChangedEventArgs e) { if (SelectedIndexChanged != null) SelectedIndexChanged(sender, e); }
        #endregion

        #region Occurs when an item is actived
        [Browsable(true)]
        [Category("Action")]
        [Description("Occurs when an item is actived")]
        public event EventHandler<MainForm.ItemActivateEventArgs> ItemActivate;
        protected virtual void OnItemActivate(object sender, MainForm.ItemActivateEventArgs e) { if (ItemActivate != null) ItemActivate(sender, e); }
        #endregion

        private void listView1_SelectedIndexChanged(object sender, EventArgs e) { OnSelectedIndexChanged(sender, new MainForm.SelectedIndexChangedEventArgs(sender as ListView)); }
        private void listView1_ItemActivate(object sender, EventArgs e) { OnItemActivate(sender, new MainForm.ItemActivateEventArgs(sender as ListView)); }

        private void omCopyRK_Click(object sender, EventArgs e)
        {
            if (SelectedItem != null && SelectedItem.Tag as SpecificResource != null)
                Clipboard.SetText((SelectedItem.Tag as SpecificResource).ResourceIndexEntry + "");
        }

        private void ocContextMenu_Opening(object sender, CancelEventArgs e)
        {
            omCopyRK.Enabled = SelectedItem != null;
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
                    compareResult = (listviewX.Tag as SpecificResource).RequestedRK.CompareTo((listviewY.Tag as SpecificResource).RequestedRK);
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

        #region FillThread
        bool filling = false;
        Thread fillThread;

        void StartFilling()
        {
            Diagnostics.Log(String.Format("StartFilling ResourceType: {0}", ResourceType));
            listView1.Visible = false;
            if (!isFix)
            {
                PleaseWait.DoWait(this, "Please wait, loading object catalog...");
            }
            listView1.Items.Clear();

            FillThread ft = new FillThread(this, ResourceType, Add, updateProgressCB, stopFilling, OnFillingComplete);
            FillingComplete += new EventHandler<MainForm.BoolEventArgs>(ObjectChooser_FillingComplete);

            fillThread = new Thread(new ThreadStart(ft.Fill));
            filling = true;
            fillThread.Start();
        }

        void ObjectChooser_FillingComplete(object sender, MainForm.BoolEventArgs e)
        {
            Diagnostics.Log(String.Format("ObjectChooser_FillingComplete {0}", e.arg));
            filling = false;
            while (fillThread != null && fillThread.IsAlive)
                fillThread.Join(100);
            fillThread = null;

            updateProgressCB(true, "", true, -1, false, 0);
            if (!isFix)
            {
                PleaseWait.StopWait(this);
            }
            listView1.Visible = !isFix;



            if (e.arg)
            {
                if (listView1.Items.Count > 0) { SelectedItem = listView1.Items[0]; if (!isFix) { SelectedItem.EnsureVisible(); listView1.Focus(); } }
                if (isFix && listView1.Items.Count == 1) listView1_ItemActivate(listView1, EventArgs.Empty);
            }
        }

        public void AbortFilling(bool abort)
        {
            Diagnostics.Log(String.Format("AbortFilling {0}", abort));
            if (abort)
            {
                if (fillThread != null) fillThread.Abort();
                filling = false;
                while (fillThread != null && fillThread.IsAlive)
                    fillThread.Join(100);
                fillThread = null;
            }
            else
            {
                if (!filling) ObjectChooser_FillingComplete(null, new MainForm.BoolEventArgs(false));
                else filling = false;
            }
        }

        delegate void AddCallBack(SpecificResource item);
        void Add(SpecificResource item)
        {
            if (!this.IsHandleCreated || this.IsDisposed) return;
            listViewAddCB(item, listView1);
        }

        public event EventHandler<MainForm.BoolEventArgs> FillingComplete;
        public delegate void fillingCompleteCallback(bool complete);
        public void OnFillingComplete(bool complete) { if (FillingComplete != null) { FillingComplete(this, new MainForm.BoolEventArgs(complete)); } }

        public delegate bool stopFillingCallback();
        private bool stopFilling() { return !filling; }

        class FillThread
        {

            CatalogType resourceType;
            Control control;
            AddCallBack addCB;
            MainForm.updateProgressCallback updateProgressCB;
            stopFillingCallback stopFillingCB;
            fillingCompleteCallback fillingCompleteCB;

            public FillThread(Control objectChooser, CatalogType resourceType
                , AddCallBack createListViewItemCB
                , MainForm.updateProgressCallback updateProgressCB
                , stopFillingCallback stopFillingCB
                , fillingCompleteCallback fillingCompleteCB
                )
            {
                this.control = objectChooser;
                this.resourceType = resourceType;
                this.addCB = createListViewItemCB;
                this.updateProgressCB = updateProgressCB;
                this.stopFillingCB = stopFillingCB;
                this.fillingCompleteCB = fillingCompleteCB;
            }


            public void Fill()
            {
                bool complete = false;
                bool abort = false;
                try
                {
                    updateProgress(true, "Please wait, searching for objects...", true, -1, false, 0);

                    List<SpecificResource> lres = new List<SpecificResource>();
                    updateProgress(true, "Please wait, finding objects... 0%", true, FileTable.fb0.Count, true, 0);
                    int i = 0;
                    foreach (PathPackageTuple ppt in FileTable.fb0)
                    {
                        if (stopFilling) return;

                        List<SpecificResource> matches;
                        if (resourceType != 0)
                            matches = ppt.FindAll(rie => rie.ResourceType == (uint)resourceType && !lres.Exists(sr => rie.Instance == sr.ResourceIndexEntry.Instance));
                        else
                            matches = ppt.FindAll(rie => Enum.IsDefined(typeof(CatalogType), rie.ResourceType) && !lres.Exists(sr => rie.Instance == sr.ResourceIndexEntry.Instance));
                        if (stopFilling) return;

                        lres.AddRange(matches);
                        updateProgress(true, "Please wait, finding objects... " + i * 100 / FileTable.fb0.Count + "%", false, -1, true, i);
                    }

                    updateProgress(true, "Please wait, loading objects... 0%", true, lres.Count, true, 0);
                    int freq = Math.Max(1, lres.Count / 50);
                    i = 0;
                    foreach (SpecificResource res in lres)
                    {
                        if (stopFilling) return;

                        Add(res);

                        if (++i % freq == 0)
                            updateProgress(true, "Please wait, loading objects... " + i * 100 / lres.Count + "%", false, -1, true, i);
                    }
                    complete = true;
                }
                catch (ThreadAbortException) { abort = true; Thread.Sleep(0); }
                finally
                {
                    if (!abort)
                    {
                        updateProgress(true, "Search ended", true, -1, false, 0);
                        fillingComplete(complete);
                    }
                }
            }

            #region Callbacks
            void Add(SpecificResource sr) { Thread.Sleep(0); if (control.IsHandleCreated && !control.IsDisposed) control.Invoke(addCB, new object[] { sr, }); }

            void updateProgress(bool changeText, string text, bool changeMax, int max, bool changeValue, int value) { Thread.Sleep(0); if (control.IsHandleCreated && !control.IsDisposed) control.Invoke(updateProgressCB, new object[] { changeText, text, changeMax, max, changeValue, value, }); }

            bool stopFilling { get { Thread.Sleep(0); return !(control.IsHandleCreated && !control.IsDisposed) || (bool)control.Invoke(stopFillingCB); } }

            void fillingComplete(bool complete) { Thread.Sleep(0); if (control.IsHandleCreated && !control.IsDisposed) control.BeginInvoke(fillingCompleteCB, new object[] { complete, }); }
            #endregion
        }
        #endregion
    }
}
