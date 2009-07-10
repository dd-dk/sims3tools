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
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using s3pi.Interfaces;

namespace S3PIDemoFE.Tools
{
    public partial class SearchForm : Form
    {
        public SearchForm()
        {
            InitializeComponent();
            lbxHits.Items.Clear();
        }

        #region Search form signal to main app
        [Browsable(true)]
        [Category("Action")]
        [Description("Raised when the user clicks \"Go to\"")]
        public event EventHandler<GoEventArgs> Go;

        public class GoEventArgs : EventArgs
        {
            IResourceIndexEntry rie;
            public GoEventArgs(IResourceIndexEntry rie) { this.rie = rie; }
            public IResourceIndexEntry ResourceIndexEntry { get { return rie; } }
        }

        protected virtual void OnGo(object sender, GoEventArgs e) { if (Go != null) Go(sender, e); }
        #endregion

        IPackage pkg;
        public IPackage CurrentPackage { get { return pkg; } set { pkg = value; btnSearch.Enabled = (pkg != null && tbHex.Text.Length > 0); } }

        string fromRIE(IResourceIndexEntry rie) { return "0x" + rie.ResourceType.ToString("X8") + " 0x" + rie.ResourceGroup.ToString("X8") + " 0x" + rie.Instance.ToString("X16"); }

        #region Search thread
        List<IResourceIndexEntry> lrie;
        Thread searchThread;
        bool searching;
        void StartSearch()
        {
            lrie = new List<IResourceIndexEntry>();
            lbxHits.Items.Clear();
            this.SearchComplete += new EventHandler<BoolEventArgs>(SearchForm_SearchComplete);

            string[] input = tbHex.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            byte[] target = new byte[input.Length];
            for (int i = 0; i < input.Length; i++) target[i] = Convert.ToByte(input[i], 16);

            SearchThread st = new SearchThread(this, pkg, target, Add, updateProgress, stopSearch, OnSearchComplete);

            searchThread = new Thread(new ThreadStart(st.Search));
            searching = true;
            searchThread.Start();
        }

        void SearchForm_SearchComplete(object sender, SearchForm.BoolEventArgs e)
        {
            searching = false;
            while (searchThread != null && searchThread.IsAlive)
                searchThread.Join(100);
            searchThread = null;

            this.toolStripProgressBar1.Visible = false;
            this.toolStripStatusLabel1.Visible = false;

            tbHex.Enabled = true;
            btnSearch.Text = "&Search";
        }

        void AbortSearch()
        {
            if (!searching) SearchForm_SearchComplete(null, new BoolEventArgs(false));
            else searching = false;
        }

        delegate void AddCallBack(IResourceIndexEntry rie);
        void Add(IResourceIndexEntry rie)
        {
            lbxHits.Items.Add(fromRIE(rie));
            lrie.Add(rie);
        }

        class BoolEventArgs : EventArgs
        {
            public bool arg;
            public BoolEventArgs(bool arg) { this.arg = arg; }
        }
        event EventHandler<BoolEventArgs> SearchComplete;
        delegate void searchCompleteCallback(bool complete);
        void OnSearchComplete(bool complete) { if (SearchComplete != null) { SearchComplete(this, new BoolEventArgs(complete)); } }

        delegate void updateProgressCallback(bool changeText, string text, bool changeMax, int max, bool changeValue, int value);
        void updateProgress(bool changeText, string text, bool changeMax, int max, bool changeValue, int value)
        {
            if (changeText)
            {
                toolStripStatusLabel1.Visible = text.Length > 0;
                toolStripStatusLabel1.Text = text;
            }
            if (changeMax)
            {
                if (max == -1)
                    toolStripProgressBar1.Visible = false;
                else
                {
                    toolStripProgressBar1.Visible = true;
                    toolStripProgressBar1.Maximum = max;
                }
            }
            if (changeValue)
                toolStripProgressBar1.Value = value;
        }

        public delegate bool stopSearchCallback();
        private bool stopSearch() { return !searching; }

        class SearchThread
        {
            SearchForm form;
            IPackage pkg;
            byte[] pattern;
            AddCallBack addCB;
            updateProgressCallback updateProgressCB;
            stopSearchCallback stopSearchCB;
            searchCompleteCallback searchCompleteCB;

            public SearchThread(SearchForm form, IPackage pkg, byte[] pattern,
                AddCallBack addCB, updateProgressCallback updateProgressCB, stopSearchCallback stopSearchCB, searchCompleteCallback searchCompleteCB)
            {
                this.form = form;
                this.pkg = pkg;
                this.pattern = (byte[])pattern.Clone();
                this.addCB = addCB;
                this.updateProgressCB = updateProgressCB;
                this.stopSearchCB = stopSearchCB;
                this.searchCompleteCB = searchCompleteCB;
            }

            public void Search()
            {
                bool complete = false;
                try
                {
                    updateProgress(true, "Retrieving resource index...", true, 0, true, 0);
                    IList<IResourceIndexEntry> lrie = pkg.GetResourceList;
                    if (stopSearch) return;

                    updateProgress(true, "Starting search... 0%", true, lrie.Count, true, 0);
                    int freq = Math.Max(1, lrie.Count / 100);

                    int hits = 0;
                    for (int i = 0; i < lrie.Count; i++)
                    {
                        if (stopSearch) return;

                        if (search(lrie[i]))
                        {
                            Add(lrie[i]);
                            hits++;
                        }

                        if (i % freq == 0)
                            updateProgress(true, String.Format("[Hits {0}] Searching... {1}%", hits, i * 100 / lrie.Count), false, -1, true, i);
                    }
                    complete = true;
                }
                catch (ThreadInterruptedException) { }
                finally
                {
                    updateProgress(true, "Search ended", true, 0, true, 0);
                    searchComplete(complete);
                }
            }

            bool search(IResourceIndexEntry rie)
            {
                IResource res = s3pi.WrapperDealer.WrapperDealer.GetResource(0, pkg, rie, true);// we're searching bytes
                byte[] resBytes = res.AsBytes;
                for (int i = 0; i < resBytes.Length - pattern.Length; i++)
                {
                    for (int j = 0; j < pattern.Length; j++)
                        if (resBytes[i + j] != pattern[j]) goto fail;
                    return true;
                fail: { }
                }
                return false;
            }

            void Add(IResourceIndexEntry rie) { Thread.Sleep(0); if (form.IsHandleCreated) form.Invoke(addCB, new object[] { rie, }); }

            void updateProgress(bool changeText, string text, bool changeMax, int max, bool changeValue, int value)
            {
                Thread.Sleep(0);
                if (form.IsHandleCreated) form.Invoke(updateProgressCB, new object[] { changeText, text, changeMax, max, changeValue, value, });
            }

            bool stopSearch { get { Thread.Sleep(0); return !form.IsHandleCreated || (bool)form.Invoke(stopSearchCB); } }

            void searchComplete(bool complete) { Thread.Sleep(0); if (form.IsHandleCreated) form.BeginInvoke(searchCompleteCB, new object[] { complete, }); }
        }
        #endregion

        #region Form control event handlers
        static string valid = "0123456789abcdefABCDEF ";
        private void tbHex_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar >= 32 && e.KeyChar < 0x7f && valid.IndexOf(e.KeyChar) < 0) e.Handled = true;
        }

        Regex rx = new Regex(@"^[0-9A-F][0-9A-F]?(\s+([0-9A-F][0-9A-F]?)?)*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private void tbHex_TextChanged(object sender, EventArgs e)
        {
            if (!rx.IsMatch(tbHex.Text))
            {
                if (tbHex.SelectionStart > 0) tbHex.SelectionStart--;
                tbHex.SelectionLength = Math.Min(1, tbHex.Text.Length);
                if (tbHex.SelectionLength > 0) tbHex.SelectedText = "";
            }
            else btnSearch.Enabled = tbHex.Text.Length > 0;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (searching)
                AbortSearch();
            else
            {
                tbHex.Enabled = false;
                btnSearch.Text = "&Stop";
                StartSearch();
            }
        }

        private void lbxHits_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnGo.Enabled = lbxHits.SelectedItems.Count == 1;
            btnCopy.Enabled = lbxHits.SelectedItems.Count > 0;
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            string s = "";
            foreach (int i in lbxHits.SelectedIndices) s += fromRIE(lrie[i]) + "\n";
            //s = s.TrimEnd('\n');
            Clipboard.SetText(s);
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            OnGo(this, new GoEventArgs(lrie[lbxHits.SelectedIndex]));
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}
