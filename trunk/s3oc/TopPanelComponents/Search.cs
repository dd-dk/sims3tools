/***************************************************************************
 *  Copyright (C) 2010 by Peter L Jones                                    *
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
    public partial class Search : UserControl
    {
        #region Static bits
        static string[] ctNames = new string[]
        {
            "Any",

            "Fence",
            "Stairs",
            "Proxy Product",
            "Terrain Geometry Brush",

            "Railing",
            "Terrain Paint Brush",
            "Fireplace",
            "Terrain Water Brush",

            "Fountain / Pool",

            "Foundation",
            "Normal Object",
            "Wall/Floor Pattern",
            "Wall Style",

            "Roof Style",
            "Modular Resource",
            "Roof Pattern",
        };
        #endregion

        List<IPackage> objPkgs;
        MainForm.updateProgressCallback updateProgressCB;

        public Search(List<IPackage> objPkgs, MainForm.updateProgressCallback updateProgressCB)
        {
            InitializeComponent();
            Search_LoadListViewSettings();
            cbCatalogType.Items.AddRange(ctNames);
            cbCatalogType.SelectedIndex = 0;
            btnSearch.Enabled = false;

            this.objPkgs = objPkgs;
            this.updateProgressCB = updateProgressCB;
        }

        public void Search_LoadListViewSettings()
        {
            string cw = ObjectCloner.Properties.Settings.Default.ColumnWidths;
            string[] cws = cw == null ? new string[] { } : cw.Split(':');

            int w;
            listView1.Columns[0].Width = cws.Length > 0 && int.TryParse(cws[0], out w) && w > 0 ? w : (this.listView1.Width - 32) / 3;
            listView1.Columns[1].Width = cws.Length > 1 && int.TryParse(cws[1], out w) && w > 0 ? w : (this.listView1.Width - 32) / 6;
            listView1.Columns[2].Width = cws.Length > 2 && int.TryParse(cws[2], out w) && w > 0 ? w : (this.listView1.Width - 32) / 6;
            listView1.Columns[3].Width = cws.Length > 3 && int.TryParse(cws[3], out w) && w > 0 ? w : (this.listView1.Width - 32) / 3;/**/
        }
        public void Search_SaveSettings()
        {
            /*ObjectCloner.Properties.Settings.Default.ColumnWidths = string.Format("{0}:{1}:{2}:{3}"
                , listView1.Columns[0].Width
                , listView1.Columns[1].Width
                , listView1.Columns[2].Width
                , listView1.Columns[3].Width
                );/**/
        }

        static Item ItemForTGIBlock0(List<IPackage> pkgs, Item item)
        {
            IResourceKey rk = ((TGIBlockList)item.Resource["TGIBlocks"].Value)[0];
            return new Item(pkgs, rk);
        }

        #region Search thread
        bool searching;
        Thread searchThread;
        void StartSearch()
        {
            listView1.Items.Clear();

            SearchThread st = new SearchThread(this, objPkgs, Add, updateProgressCB, stopSearch, OnSearchComplete);
            SearchComplete += new EventHandler<MainForm.BoolEventArgs>(Search_SearchComplete);

            searchThread = new Thread(new ThreadStart(st.Search));
            searching = true;
            searchThread.Start();
        }

        void Search_SearchComplete(object sender, MainForm.BoolEventArgs e)
        {
            searching = false;
            while (searchThread != null && searchThread.IsAlive)
                searchThread.Join(100);
            searchThread = null;

            updateProgressCB(true, "", true, -1, false, 0);

            listView1.Enabled = tbText.Enabled = tlpWhere.Enabled = cbCatalogType.Enabled = true;
            btnSearch.Text = "&Search";
        }

        void AbortSearch()
        {
            if (!searching) Search_SearchComplete(null, new MainForm.BoolEventArgs(false));
            else searching = false;
        }

        delegate void AddCallBack(Item item);
        void Add(Item item)
        {
            if (!this.IsHandleCreated) return;

            if (item.CType == CatalogType.CatalogTerrainPaintBrush && ((byte)item.Resource["CommonBlock.BuildBuyProductStatusFlags"].Value & 0x01) == 0)
                return; // do not list

            Item ctlg = item.RequestedRK.ResourceType == (uint)CatalogType.ModularResource ? ItemForTGIBlock0(objPkgs, item) : item;
            string name;
            if (ctlg.Resource != null)
            {
                name = ctlg.Resource["CommonBlock.Name"];
                name = (name.IndexOf(':') < 0) ? name : name.Substring(name.LastIndexOf(':') + 1);
            }
            else
            {
                name = ctlg.Exception.Message;
                for (Exception ex = ctlg.Exception.InnerException; ex != null; ex = ex.InnerException) name += "  " + ex.Message;
            }

            List<string> exts;
            string tag = "";
            if (s3pi.Extensions.ExtList.Ext.TryGetValue("0x" + item.RequestedRK.ResourceType.ToString("X8"), out exts)) tag = exts[0];
            else tag = "UNKN";

            listView1.Items.Add(new ListViewItem(new string[] {
                name, tag, item.RGVsn, "" + (AResourceKey)item.RequestedRK,
            }) { Tag = item });
        }

        event EventHandler<MainForm.BoolEventArgs> SearchComplete;
        delegate void searchCompleteCallback(bool complete);
        void OnSearchComplete(bool complete) { if (SearchComplete != null) { SearchComplete(this, new MainForm.BoolEventArgs(complete)); } }

        public delegate bool stopSearchCallback();
        private bool stopSearch() { return !searching; }

        class SearchThread
        {
            struct Criteria
            {
                public string text;
                public bool resourceName;
                public bool objectName;
                public bool objectDesc;
                public bool catalogName;
                public bool catalogDesc;
                public bool allLanguages; // false for English only
                public CatalogType catalogType;
            }
            Criteria criteria;
            Control control;
            List<IPackage> objPkgs;
            AddCallBack addCB;
            MainForm.updateProgressCallback updateProgressCB;
            stopSearchCallback stopSearchCB;
            searchCompleteCallback searchCompleteCB;

            IDictionary<ulong, string> nameMap = null;
            List<IDictionary<ulong, string>> stbls = null;

            public SearchThread(Search searchPane, List<IPackage> objPkgs,
                AddCallBack addCB, MainForm.updateProgressCallback updateProgressCB, stopSearchCallback stopSearchCB, searchCompleteCallback searchCompleteCB)
            {
                this.control = searchPane;
                this.criteria.text = searchPane.tbText.Text.Trim().ToLowerInvariant();
                this.criteria.resourceName = searchPane.ckbResourceName.Checked;
                this.criteria.objectName = searchPane.ckbObjectName.Checked;
                this.criteria.objectDesc = searchPane.ckbObjectDesc.Checked;
                this.criteria.catalogName = searchPane.ckbCatalogName.Checked;
                this.criteria.catalogDesc = searchPane.ckbCatalogDesc.Checked;
                this.criteria.allLanguages = !searchPane.rb1English.Checked;
                this.criteria.allLanguages = searchPane.rb1All.Checked;
                this.criteria.catalogType = searchPane.SelectedCatalogType;
                this.objPkgs = objPkgs;
                this.addCB = addCB;
                this.updateProgressCB = updateProgressCB;
                this.stopSearchCB = stopSearchCB;
                this.searchCompleteCB = searchCompleteCB;
            }

            public void Search()
            {
                List<IResourceKey> seen = new List<IResourceKey>();
                bool complete = false;
                int hits = 0;
                try
                {
                    for (int p = 0; p < objPkgs.Count; p++)
                    {
                        IPackage pkg = objPkgs[p];

                        updateProgress(true, String.Format("Retrieving name map for package {0} of {1}...", p + 1, objPkgs.Count), true, -1, false, 0);
                        IList<IResourceIndexEntry> lrie = pkg.FindAll(rie => rie.ResourceType == 0x0166038C);
                        foreach (IResourceIndexEntry rie in lrie)
                        {
                            nameMap = new Item(new RIE(pkg, rie)).Resource as IDictionary<ulong, string>;
                            if (stopSearch) return;
                        }

                        stbls = new List<IDictionary<ulong, string>>();
                        updateProgress(true, String.Format("Retrieving string tables for package {0} of {1}...", p + 1, objPkgs.Count), true, -1, false, 0);
                        lrie = pkg.FindAll(rie => rie.ResourceType == 0x220557DA);
                        if (criteria.allLanguages)
                        {
                            foreach (IResourceIndexEntry rie in lrie)
                            {
                                stbls.Add(new Item(new RIE(pkg, rie)).Resource as IDictionary<ulong, string>);
                                if (stopSearch) return;
                            }
                        }
                        else
                        {
                            foreach (IResourceIndexEntry rie in lrie)
                            {
                                if (rie.Instance >> 56 == 0x00)
                                    stbls.Add(new Item(new RIE(pkg, rie)).Resource as IDictionary<ulong, string>);
                                if (stopSearch) return;
                            }
                        }

                        updateProgress(true, String.Format("Retrieving resource list {0} of {1}...", p + 1, objPkgs.Count), true, -1, false, 0);
                        lrie = Find(pkg);
                        if (stopSearch) return;

                        updateProgress(true, "Starting search... 0%", true, lrie.Count, true, 0);
                        int freq = Math.Max(1, lrie.Count / 100);

                        for (int i = 0; i < lrie.Count; i++)
                        {
                            if (stopSearch) return;
                            if (!seen.Exists(new RK(lrie[i]).Equals))
                            {
                                seen.Add(lrie[i]);
                                Item item = new Item(new RIE(pkg, lrie[i]));
                                if (Match(lrie[i].ResourceType == (uint)CatalogType.ModularResource ? ItemForTGIBlock0(objPkgs, item) : item))
                                {
                                    Add(item);
                                    hits++;
                                }
                            }
                            if (i % freq == 0)
                                updateProgress(true, String.Format("[Hits {0}] Searching... {1}%", hits, i * 100 / lrie.Count), false, -1, true, i);
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

            IList<IResourceIndexEntry> Find(IPackage pkg)
            {
                if (criteria.catalogType == 0)
                    return pkg.FindAll(rie => Enum.IsDefined(typeof(CatalogType), rie.ResourceType));
                else
                    return pkg.FindAll(rie => criteria.catalogType == (CatalogType)rie.ResourceType);
            }

            bool Match(Item item)
            {
                if (criteria.resourceName && MatchResourceName(item)) return true;
                if (criteria.objectName && MatchObjectName(item)) return true;
                if (criteria.objectDesc && MatchObjectDesc(item)) return true;
                if (criteria.catalogName && MatchCatalogName(item)) return true;
                if (criteria.catalogDesc && MatchCatalogDesc(item)) return true;
                return false;
            }
            bool MatchResourceName(Item item) { return nameMap != null && nameMap.ContainsKey(item.RequestedRK.Instance) && nameMap[item.RequestedRK.Instance].Trim().ToLowerInvariant().Contains(criteria.text); }
            bool MatchObjectName(Item item) { return ((string)item.Resource["CommonBlock.Name"].Value).Trim().ToLowerInvariant().Contains(criteria.text); }
            bool MatchObjectDesc(Item item) { return ((string)item.Resource["CommonBlock.Desc"].Value).Trim().ToLowerInvariant().Contains(criteria.text); }
            bool MatchCatalogName(Item item) { return MatchStbl((ulong)item.Resource["CommonBlock.NameGUID"].Value); }
            bool MatchCatalogDesc(Item item) { return MatchStbl((ulong)item.Resource["CommonBlock.DescGUID"].Value); }
            bool MatchStbl(ulong guid) { foreach (var stbl in stbls) if (stbl.ContainsKey(guid) && stbl[guid].Trim().ToLowerInvariant().Contains(criteria.text)) return true; return false; }

            #region Callbacks
            void Add(Item item) { Thread.Sleep(0); if (control.IsHandleCreated) control.Invoke(addCB, new object[] { item, }); }

            void updateProgress(bool changeText, string text, bool changeMax, int max, bool changeValue, int value) { Thread.Sleep(0); if (control.IsHandleCreated) control.Invoke(updateProgressCB, new object[] { changeText, text, changeMax, max, changeValue, value, }); }

            bool stopSearch { get { Thread.Sleep(0); return !control.IsHandleCreated || (bool)control.Invoke(stopSearchCB); } }

            void searchComplete(bool complete) { Thread.Sleep(0); if (control.IsHandleCreated) control.BeginInvoke(searchCompleteCB, new object[] { complete, }); }
            #endregion
        }
        #endregion


        bool haveCriteria()
        {
            return tbText.Text.Length > 0 &&
                (ckbResourceName.Checked || ckbCatalogDesc.Checked || ckbCatalogName.Checked || ckbObjectDesc.Checked || ckbObjectName.Checked);
        }

        public CatalogType SelectedCatalogType { get { return cbCatalogType.SelectedIndex > 0 ? ((CatalogType[])Enum.GetValues(typeof(CatalogType)))[cbCatalogType.SelectedIndex - 1] : 0; } }

        public ListView.ListViewItemCollection Items { get { return listView1.Items; } }

        public ListView.SelectedListViewItemCollection SelectedItems { get { return listView1.SelectedItems; } }

        public ListViewItem SelectedItem { get { return listView1.SelectedItems.Count == 1 ? listView1.SelectedItems[0] : null; } set { listView1.SelectedItems.Clear(); if (listView1.Items.Contains(value)) value.Selected = true; } }

        public int SelectedIndex { get { return listView1.SelectedIndices.Count == 1 ? listView1.SelectedIndices[0] : -1; } set { listView1.SelectedIndices.Clear(); if (value >= 0 && value < listView1.Items.Count)listView1.SelectedIndices.Add(value); } }


        public abstract class ItemEventArgs : EventArgs
        {
            ListViewItem selectedItem = null;
            public ItemEventArgs(ListView lv) { selectedItem = lv.SelectedItems.Count == 0 ? null : lv.SelectedItems[0]; }
            public Item SelectedItem { get { return selectedItem == null ? null : selectedItem.Tag as Item; } }
        }

        #region Occurs when an item is actived
        [Browsable(true)]
        [Category("Action")]
        [Description("Occurs when an item is actived")]
        public event EventHandler<ItemActivateEventArgs> ItemActivate;
        public class ItemActivateEventArgs : ItemEventArgs { public ItemActivateEventArgs(ListView lv) : base(lv) { } }
        protected virtual void OnItemActivate(object sender, ItemActivateEventArgs e) { if (ItemActivate != null) ItemActivate(sender, e); }
        #endregion

        #region Occurs whenever the 'SelectedIndex' for the Search Results changes
        [Browsable(true)]
        [Category("Action")]
        [Description("Occurs whenever the 'SelectedIndex' for the Search Results changes")]
        public event EventHandler<SelectedIndexChangedEventArgs> SelectedIndexChanged;
        public class SelectedIndexChangedEventArgs : ItemEventArgs { public SelectedIndexChangedEventArgs(ListView lv) : base(lv) { } }
        protected virtual void OnSelectedIndexChanged(object sender, SelectedIndexChangedEventArgs e) { if (SelectedIndexChanged != null) SelectedIndexChanged(sender, e); }
        #endregion



        private void listView1_ItemActivate(object sender, EventArgs e) { OnItemActivate(sender, new ItemActivateEventArgs(sender as ListView)); }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e) { OnSelectedIndexChanged(sender, new SelectedIndexChangedEventArgs(sender as ListView)); }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (searching)
                AbortSearch();
            else
            {
                listView1.Enabled = tbText.Enabled = tlpWhere.Enabled = cbCatalogType.Enabled = false;
                btnSearch.Text = "&Stop";
                StartSearch();
            }
        }

        private void ckb_CheckedChanged(object sender, EventArgs e)
        {
            btnSearch.Enabled = haveCriteria();
        }

        private void tbText_TextChanged(object sender, EventArgs e)
        {
            btnSearch.Enabled = haveCriteria();
        }

        private void scmCopyRK_Click(object sender, EventArgs e)
        {
            if (SelectedItem != null && SelectedItem.Tag as Item != null)
                Clipboard.SetText((SelectedItem.Tag as Item).SpecificRK + "");
        }

        private void searchContextMenu_Opening(object sender, CancelEventArgs e)
        {
            scmCopyRK.Enabled = SelectedIndexChanged != null;
        }
    }
}
