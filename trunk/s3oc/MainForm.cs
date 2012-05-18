/***************************************************************************
 *  Copyright (C) 2009, 2010 by Peter L Jones                              *
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
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Windows.Forms;
using s3pi.Interfaces;
using s3pi.Extensions;
using s3pi.GenericRCOLResource;
using ObjectCloner.SplitterComponents;
using s3pi.Filetable;
using System.Xml.Linq;
using meshExpImp.ModelBlocks;

namespace ObjectCloner
{
    public partial class MainForm : Form
    {
        static string myName = "s3oc";

        enum Mode
        {
            None = 0,
            FromGame,
            FromUser,
        }
        Mode mode = Mode.None;

        SpecificResource selectedItem;
        Image replacementForThumbs;

        private ObjectChooser objectChooser;
        private PleaseWait pleaseWait;
        private CloneFixOptions cloneFixOptions;
        private Search searchPane;
        private TGISearch tgiSearchPane;
        private ReplaceTGI replaceTGIPane;
        private FixIntegrityResults fixIntegrityResults;

        public MainForm()
        {
            using (Splash form = new Splash("Starting s3oc..."))
            {
                form.Show();
                Application.DoEvents();

                InitializeComponent();

                this.Text = myName;
                pleaseWait = new PleaseWait();

                FileTable.CustomContentPath = ObjectCloner.Properties.Settings.Default.CustomContent;
                GameFolders.InstallDirs = ObjectCloner.Properties.Settings.Default.InstallDirs;
                GameFolders.EPsDisabled = ObjectCloner.Properties.Settings.Default.EPsDisabled;

                MainForm_LoadFormSettings();

                menuBarWidget1.Checked(MenuBarWidget.MB.MBS_langSearch, STBLHandler.LangSearch);

                menuBarWidget1.Checked(MenuBarWidget.MB.MBS_advanced, ObjectCloner.Properties.Settings.Default.AdvanceCloning);

                Diagnostics.Popups = ObjectCloner.Properties.Settings.Default.Diagnostics;
                menuBarWidget1.Checked(MenuBarWidget.MB.MBS_popups, Diagnostics.Popups);

                SetStepText();

                InitialiseTabs(CatalogType.CatalogProxyProduct);//Use the Proxy Product as it has pretty much nothing on it
                TabEnable(false);
            }
        }

        string GetMyName()
        {
            if (FileTable.Current == null) return myName;
            return myName + ": " + Path.GetFileNameWithoutExtension(FileTable.Current.Path);
        }

        string cmdLineFilename = null;
        public MainForm(params string[] args)
            : this()
        {
            CmdLine(args);

            // Settings for test mode
            if (cmdlineTest)
            {
            }
        }

        private void MainForm_LoadFormSettings()
        {
            int h = ObjectCloner.Properties.Settings.Default.PersistentHeight;
            if (h == -1) h = 4 * System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height / 5;
            this.Height = h;

            int w = ObjectCloner.Properties.Settings.Default.PersistentWidth;
            if (w == -1) w = 4 * System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width / 5;
            this.Width = w;

            Point xy = ObjectCloner.Properties.Settings.Default.PersistentLocation;
            if (xy.X == -1 && xy.Y == -1)
                this.StartPosition = FormStartPosition.CenterScreen;
            else
            {
                this.StartPosition = FormStartPosition.Manual;
                this.Location = xy;
            }

            w = ObjectCloner.Properties.Settings.Default.Splitter1Width;
            if (w > 8 && w < this.ClientSize.Width - 8)
                splitContainer1.SplitterDistance = w;
            else
                splitContainer1.SplitterDistance = this.ClientSize.Width / 2 - 2;
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            // "D:\My Documents\Sims\CreatorNameHere_ToiletExpensive_NR.package"
            if (cmdLineFilename != null)
            {
                ObjectCloner.Properties.Settings.Default.LastSaveFolder = Path.GetDirectoryName(cmdLineFilename);
                mode = Mode.FromUser;
                fileReOpenToFix(cmdLineFilename);
                cmdLineFilename = null;
            }

            // /select 0x319E4F1D-0x00000000-0x000000000000040B
            if (cmdLineSelect)
            {
                cmdLineSelect = false;
                
                InitialiseFileTable();

                if (CheckInstallDirs(null))
                {
                    int i = 0;
                    using (Splash form = new Splash("Searching..."))
                    {
                        form.Show();
                        selectedItem = FileTable.GameContent.Select(ppt =>
                        {
                            form.Message = "Searching package " + ++i + "...";
                            return ppt.Find(rie => cmdLineSelectRK.Equals(rie));
                        }).Where(x => x != null).FirstOrDefault();
                    }

                    if (selectedItem == null)
                        CopyableMessageBox.Show(this, "Resource " + cmdLineSelectRK + " not found.", "Error", CopyableMessageBoxIcon.Error, new string[] { "OK" }, 0, 0);
                    else
                    {
                        mode = selectedItem.PPSource == "current" ? Mode.FromUser : Mode.FromGame;
                        FillTabs(selectedItem);
                        btnStart_Click(this, EventArgs.Empty);
                    }
                }
            }
        }

        static bool formClosing = false;
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Diagnostics.Log("MainForm_FormClosing CloseReason: " + e.CloseReason);
            formClosing = true;
            AbortSaving(true);
            Abort(true);
            //CloseCurrent();//needed?

            foreach (string temp in Directory.EnumerateFiles(Environment.GetEnvironmentVariable("TEMP"), string.Format("s3oc_0x{0:X8}_*.package", System.Diagnostics.Process.GetCurrentProcess().Id)))
                try { File.Delete(temp); }
                catch { }//oh well

            ObjectCloner.Properties.Settings.Default.EPsDisabled = GameFolders.EPsDisabled;
            ObjectCloner.Properties.Settings.Default.InstallDirs = GameFolders.InstallDirs;

            ObjectCloner.Properties.Settings.Default.PersistentLocation = this.WindowState == FormWindowState.Normal ? this.Location : new Point(-1, -1);
            ObjectCloner.Properties.Settings.Default.PersistentHeight = this.WindowState == FormWindowState.Normal ? this.Height : -1;
            ObjectCloner.Properties.Settings.Default.PersistentWidth = this.WindowState == FormWindowState.Normal ? this.Width : -1;
            ObjectCloner.Properties.Settings.Default.Splitter1Width = splitContainer1.SplitterDistance;
            ObjectCloner.Properties.Settings.Default.Save();
        }

        void CloseCurrent()
        {
            Diagnostics.Log("CloseCurrent");

            if (FileTable.Current != null)
            {
                s3pi.Package.Package.ClosePackage(0, FileTable.Current.Package);
                FileTable.Current = null;
                Reset();// pick up new NameMap / STBLs next time needed
            }

            this.Text = GetMyName();
        }

        void InitialiseFileTable()
        {
            Abort(false);
            SetUseCC(false);
            SetAppendEA(true);
        }

        void Abort(bool abort)
        {
            Diagnostics.Log("Abort abort:" + abort);

            btnStart.Enabled = false;
            if (objectChooser != null)
            {
                objectChooser.AbortFilling(abort);
                objectChooser.ObjectChooser_SaveListViewSettings();
                objectChooser = null;
            }
            if (searchPane != null)
            {
                searchPane.AbortSearch(abort);
                searchPane = null;
            }
            if (tgiSearchPane != null)
            {
                tgiSearchPane.AbortTGISearch(abort);
                tgiSearchPane = null;
            }
        }

        public static bool SetFT(bool useCC, bool useEA, CheckInstallDirsCB checkInstallDirsCB = null, Control control = null)
        {
            Diagnostics.Log("SetFT useCC: " + useCC
                + "; useEA: " + useEA
                + "; checkInstallDirsCB set? " + (checkInstallDirsCB != null)
                + "; control set? " + (control != null));

            bool changed = false;
            if (useCC != FileTable.CustomContentEnabled)
            {
                FileTable.CustomContentEnabled = useCC;
                changed = true;
            }
            if (useEA != FileTable.FileTableEnabled)
            {
                FileTable.FileTableEnabled = useEA;
                changed = true;
            }
            if (changed)
            {
                Reset();
                if (checkInstallDirsCB != null) return checkInstallDirsCB(control);
            }
            return true;
        }

        public static void SetUseCC(bool value)
        {
            Diagnostics.Log("SetUseCC value: " + value);

            if (FileTable.CustomContentEnabled != value)
            {
                FileTable.CustomContentEnabled = value;
                Reset();
            }
        }

        public static void SetAppendEA(bool value)
        {
            Diagnostics.Log("SetAppendEA value: " + value);

            if (FileTable.FileTableEnabled != value)
            {
                FileTable.FileTableEnabled = value;
                Reset();
            }
        }

        static void Reset()
        {
            Diagnostics.Log("Reset");

            NameMap.Reset();
            STBLHandler.Reset();
        }


        #region Command Line
        delegate bool CmdLineCmd(ref List<string> cmdline);
        struct CmdInfo
        {
            public CmdLineCmd cmd;
            public string help;
            public CmdInfo(CmdLineCmd cmd, string help) : this() { this.cmd = cmd; this.help = help; }
        }
        Dictionary<string, CmdInfo> Options;
        void SetOptions()
        {
            Options = new Dictionary<string, CmdInfo>();
            Options.Add("test", new CmdInfo(CmdLineTest, "Enable facilities still undergoing initial testing"));
            Options.Add("select", new CmdInfo(CmdLineSelect, "Select the specified the resourceKey to clone"));
            Options.Add("help", new CmdInfo(CmdLineHelp, "Display this help"));
        }
        void CmdLine(params string[] args)
        {
            SetOptions();
            List<string> cmdline = new List<string>(args);
            List<string> pkgs = new List<string>();
            while (cmdline.Count > 0)
            {
                string option = cmdline[0];
                cmdline.RemoveAt(0);
                if (option.StartsWith("/") || option.StartsWith("-"))
                {
                    option = option.Substring(1);
                    if (Options.ContainsKey(option.ToLower()))
                    {
                        if (Options[option.ToLower()].cmd(ref cmdline))
                            Environment.Exit(0);
                    }
                    else
                    {
                        CopyableMessageBox.Show(this, "Invalid command line option: '" + option + "'",
                            myName, CopyableMessageBoxIcon.Error, new string[] { "OK" }, 0, 0);
                        Environment.Exit(1);
                    }
                }
                else
                {
                    if (pkgs.Count == 0)
                    {
                        if (!File.Exists(option))
                        {
                            CopyableMessageBox.Show(this, "File not found:\n" + option,
                                myName, CopyableMessageBoxIcon.Error, new string[] { "OK" }, 0, 0);
                            Environment.Exit(1);
                        }
                        pkgs.Add(option);
                        cmdLineFilename = option;
                    }
                    else
                    {
                        CopyableMessageBox.Show(this, "Can only accept one package on command line",
                            myName, CopyableMessageBoxIcon.Error, new string[] { "OK" }, 0, 0);
                        Environment.Exit(1);
                    }
                }
            }
        }
        bool cmdlineTest = false;
        bool CmdLineTest(ref List<string> cmdline) { cmdlineTest = true; return false; }
        bool cmdLineSelect = false;
        IResourceKey cmdLineSelectRK = new RK(RK.NULL);
        bool CmdLineSelect(ref List<string> cmdline)
        {
            if (cmdline.Count < 1)
            {
                CopyableMessageBox.Show(this, "Missing 'resourceKey' parameter.", "Error", CopyableMessageBoxIcon.Error, new string[] { "OK" }, 0, 0);
                return true;
            }
            if (!RK.TryParse(cmdline[0], cmdLineSelectRK))
            {
                CopyableMessageBox.Show(this, "'resourceKey' parameter in incorrect format.", "Error", CopyableMessageBoxIcon.Error, new string[] { "OK" }, 0, 0);
                return true;
            }
            if (!Enum.IsDefined(typeof(CatalogType), cmdLineSelectRK.ResourceType))
            {
                CopyableMessageBox.Show(this, "Resource Type 0x" + cmdLineSelectRK.ResourceType.ToString("X8") + " is not supported.", "Error", CopyableMessageBoxIcon.Error, new string[] { "OK" }, 0, 0);
                return true;
            }
            cmdline.RemoveAt(0);
            cmdLineSelect = true;
            return false;
        }
        bool CmdLineHelp(ref List<string> cmdline)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("The following command line options are available:\n");
            foreach (var kvp in Options)
                sb.AppendFormat("{0}  --  {1}\n", kvp.Key, kvp.Value.help);
            sb.AppendLine("\nOptions must be prefixed with '/' or '-'");

            CopyableMessageBox.Show(this, sb.ToString(), "Command line options", CopyableMessageBoxIcon.Information, new List<string>(new string[] { "OK" }), 0, 0);
            return true;
        }
        #endregion


        public static SpecificResource ItemForTGIBlock0(SpecificResource item)
        {
            IResourceKey rk = ((TGIBlockList)item.Resource["TGIBlocks"].Value)[0];
            return new SpecificResource(FileTable.GameContent, rk);
        }

        #region SplitterComponents
        public abstract class ItemEventArgs : EventArgs
        {
            ListView lv;
            public ItemEventArgs(ListView lv) { this.lv = lv; }
            public ListView ListView { get { return lv; } }
            public SpecificResource SelectedItem { get { return lv.SelectedItems.Count != 1 ? null : lv.SelectedItems[0].Tag as SpecificResource; } }
        }
        public enum Action { Clone, Fix, Export, Details }
        public class ItemActivateEventArgs : ItemEventArgs
        {
            public Action Action { get; private set; }
            public ItemActivateEventArgs(ListView lv, Action action = Action.Clone) : base(lv) { this.Action = action; }
        }
        public class SelectedIndexChangedEventArgs : ItemEventArgs { public SelectedIndexChangedEventArgs(ListView lv) : base(lv) { } }

        public class BoolEventArgs : EventArgs
        {
            public bool arg;
            public BoolEventArgs(bool arg) { this.arg = arg; }
        }

        public delegate void updateProgressCallback(bool changeText, string text, bool changeMax, int max, bool changeValue, int value);
        void updateProgress(bool changeText, string text, bool changeMax, int max, bool changeValue, int value)
        {
            if (!this.IsHandleCreated || this.IsDisposed) return;
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
            {
                if (value > toolStripProgressBar1.Maximum) { }
                else
                    toolStripProgressBar1.Value = value;
            }
        }

        Dictionary<UInt64, SpecificResource> CTPTBrushIndexToPair;
        Dictionary<uint, Dictionary<SpecificResource, ListViewItem>> ListViewCache = new Dictionary<uint, Dictionary<SpecificResource, ListViewItem>>();
        public delegate void listViewAddCallBack(SpecificResource sr, ListView listView);
        public void ListViewAdd(SpecificResource sr, ListView listView)
        {
            if (!ListViewCache.ContainsKey(sr.RequestedRK.ResourceType))
            {
                ListViewCache.Add(sr.RequestedRK.ResourceType, new Dictionary<SpecificResource, ListViewItem>());
            }
            Dictionary<SpecificResource, ListViewItem> cache = ListViewCache[sr.RequestedRK.ResourceType];
            KeyValuePair<SpecificResource, ListViewItem> kvp = cache.Where(x => x.Key.Equals(sr)).FirstOrDefault();
            if (kvp.Key != null)
            {
                if (kvp.Value.ListView != null)
                    kvp.Value.ListView.Items.Remove(kvp.Value);
                listView.Items.Add(kvp.Value);
                return;
            }

            #region CatalogTerrainPaintBrush pair
            if (sr.RequestedRK.CType() == CatalogType.CatalogTerrainPaintBrush)
            {
                byte status = (byte)sr.Resource["CommonBlock.BuildBuyProductStatusFlags"].Value;
                if ((status & 0x01) == 0) // do not list
                {
                    UInt64 brushIndex = (UInt64)sr.Resource["CommonBlock.NameGUID"].Value;
                    if (!CTPTBrushIndexToPair.ContainsKey(brushIndex))//Try to leave one behind...
                    {
                        CTPTBrushIndexToPair.Add(brushIndex, sr);
                        return;
                    }
                }
            }
            #endregion

            string name = GetResourceName(sr);

            List<string> exts;
            string tag = "UNKN";
            if (s3pi.Extensions.ExtList.Ext.TryGetValue("0x" + sr.ResourceIndexEntry.ResourceType.ToString("X8"), out exts)) tag = exts[0];

            ListViewItem lvi = new ListViewItem(new string[] {
                name, tag, sr.RGVsn, "" + (AResourceKey)sr.ResourceIndexEntry,
                sr.PathPackage.Path + " (" + sr.PPSource + ")",
            }) { Tag = sr };

            listView.Items.Add(lvi);
            //listView.EnsureVisible(listView.Items.Count - 1);

            cache.Add(sr, lvi);
        }

        string GetResourceName(SpecificResource sr)
        {
            string name = NameMap.NMap[sr.ResourceIndexEntry.Instance];
            if (name == null)
            {
                try
                {
                    if (sr.RequestedRK.ResourceType == (uint)CatalogType.CAS_Part)
                        name = sr.Resource["Unknown1"];
                    else
                    {
                        SpecificResource ctlg = sr.ResourceIndexEntry.ResourceType == (uint)CatalogType.ModularResource ? ItemForTGIBlock0(sr) : sr;
                        if (ctlg.Resource != null)
                        {
                            name = ctlg.Resource["CommonBlock.Name"];
                            name = (name.IndexOf(':') < 0) ? name : name.Substring(name.LastIndexOf(':') + 1);
                        }
                        else
                        {
                            name = ctlg.Exception.Message;
                            for (Exception ex = ctlg.Exception.InnerException; ex != null; ex = ex.InnerException) name = ex.Message + "|  " + name;
                        }
                    }
                }
                catch (Exception e)
                {
                    name = e.Message;// Don't crash...
                }
            }
            return name;
        }

        #region flowLayoutPanel1 prompt labels and buttons
        [Flags]
        enum Prompt
        {
            UseMenu = 0x01,
            CloneFix = 0x02,
            ReplaceTGI = 0x04,
            SelectOptions = 0x08,

            Search = 0x10,
            TGISearch = 0x20,
            SaveCancel = 0x40,
        }
        bool testPrompt(Prompt which, Prompt value) { return (which & value) != 0; }
        void setPrompt(Prompt which)
        {
            flowLayoutPanel1.SuspendLayout();

            lbUseMenu.Visible = testPrompt(which, Prompt.UseMenu);
            lbCloneFix.Visible = testPrompt(which, Prompt.CloneFix);
            lbReplaceTGI.Visible = testPrompt(which, Prompt.ReplaceTGI);
            lbSelectOptions.Visible = testPrompt(which, Prompt.SelectOptions);
            lbSearch.Visible = testPrompt(which, Prompt.Search);
            lbTGISearch.Visible = testPrompt(which, Prompt.TGISearch);
            lbSaveCancel.Visible = testPrompt(which, Prompt.SaveCancel);

            btnStart.Visible = testPrompt(which, Prompt.CloneFix | Prompt.Search);

            flowLayoutPanel1.ResumeLayout();
        }
        #endregion

        #region ObjectChooser, Search and TGISearch common bits
        private void listView_SelectedIndexChanged(object sender, SelectedIndexChangedEventArgs e)
        {
            if (formClosing) return;

            replacementForThumbs = null;// might as well be here; needed after FillTabs, really.
            rkLookup = null;//Indicate that we're not working on the same resource any more
            if (e.SelectedItem == null)
            {
                selectedItem = null;
                ClearTabs();
                (AcceptButton as Button).Enabled = false;
            }
            else
            {
                selectedItem = e.SelectedItem;
                FillTabs(selectedItem);
                (AcceptButton as Button).Enabled = true;
            }
        }
        private void objectChooser_ItemActivate(object sender, ItemActivateEventArgs e) { AcceptButton.PerformClick(); }

        private void searchResult_ItemActivate(object sender, ItemActivateEventArgs e)
        {
            Diagnostics.Log("searchResult_ItemActivate: " + e.Action);
            switch (e.Action)
            {
                case Action.Clone:
                    if (AcceptButton != null)
                        AcceptButton.PerformClick();
                    break;
                case Action.Fix:
                    ActionFix(e.ListView);
                    break;
                case Action.Details:
                    ActionDetails(e.ListView);
                    break;
                case Action.Export:
                    ActionExport(e.ListView);
                    break;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Diagnostics.Log("btnStart_Click");
            if (tabControl1.Contains(tpMain))
                fillOverviewUpdateImage(selectedItem);
            else if (tabControl1.Contains(tpCASP))
                fillCASPUpdateImage(selectedItem);
            TabEnable(true);
            DisplayOptions();
        }

        private void ActionFix(ListView listView1)
        {
            SpecificResource sr = listView1.SelectedItems.Count == 1 ? listView1.SelectedItems[0].Tag as SpecificResource : null;
            if (sr != null
                && sr.PPSource == "cc"
                && System.IO.File.Exists(sr.PathPackage.Path))
            {
                ObjectCloner.Properties.Settings.Default.LastSaveFolder = Path.GetDirectoryName(sr.PathPackage.Path);
                mode = Mode.FromUser;
                fileReOpenToFix(sr.PathPackage.Path);
            }
        }

        private void ActionDetails(ListView listView1)
        {
            if (listView1.SelectedItems.Count != 1) return;

            ListViewItem lvi = listView1.SelectedItems[0];

            CopyableMessageBox.Show(String.Format(
                "Name: {0}\n" +
                "Tag: {1}\n" +
                "EP/SP: {2}\n" +
                "ResourceKey: {3}\n" +
                "Path: {4}\n\n\n\n",
                lvi.SubItems[0].Text, lvi.SubItems[1].Text, lvi.SubItems[2].Text, lvi.SubItems[3].Text, lvi.SubItems[4].Text
                ),
                "Selected item");
        }

        private void ActionExport(ListView listView1)
        {
            if (listView1.SelectedItems.Count == 0)
                return;

            if (!ObjectCloner.Properties.Settings.Default.pkgEditorEnabled ||
                ObjectCloner.Properties.Settings.Default.pkgEditorPath == null ||
                !System.IO.File.Exists(ObjectCloner.Properties.Settings.Default.pkgEditorPath))
                return;

            List<SpecificResource> lsr = new List<SpecificResource>();
            foreach (ListViewItem lvi in listView1.SelectedItems)
                if (lvi.Tag as SpecificResource == null ||
                    !System.IO.File.Exists((lvi.Tag as SpecificResource).PathPackage.Path))
                    return;
                else
                    lsr.Add(lvi.Tag as SpecificResource);

            List<SpecificResource> missing = new List<SpecificResource>();
            string filename = System.IO.Path.Combine(Environment.GetEnvironmentVariable("TEMP"),
                string.Format("s3oc_0x{0:X8}_{1}.package",
                System.Diagnostics.Process.GetCurrentProcess().Id,
                Guid.NewGuid().ToString()
                ));
            bool remove = true;
            try
            {
                s3pi.Package.Package.NewPackage(0).SaveAs(filename);
                PathPackageTuple ppt = new PathPackageTuple(filename, readwrite: true);

                try
                {
                    string _keyName = Path.GetFileNameWithoutExtension(filename);
                    ulong _keyIID = FNV64.GetHash(_keyName);
                    SpecificResource _keySR = ppt.AddResource(new RK(RK.NULL) { ResourceType = 0x0166038C, Instance = _keyIID, });
                    IDictionary<ulong, string> _key = _keySR.Resource as IDictionary<ulong, string>;
                    _key.Add(_keyIID, _keyName);

                    foreach (var sr in lsr)
                    {
                        if (sr.ResourceIndexEntry == null)
                        {
                            missing.Add(sr);
                            continue;
                        }

                        string name = NameMap.NMap[sr.ResourceIndexEntry.Instance];
                        if (name != null && !_key.ContainsKey(sr.ResourceIndexEntry.Instance))
                            _key.Add(sr.ResourceIndexEntry.Instance, name);
                        ppt.AddResource(sr.ResourceIndexEntry, sr.Resource.Stream);
                    }

                    if (missing.Count != 0)
                    {
                        CopyableMessageBox.Show("Resource(s) could not be found!", "Cannot open in editor", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Error);
                        return;
                    }

                    _keySR.Commit();

                }
                catch (Exception ex)
                {
                    CopyableMessageBox.IssueException(ex, "Export failed");
                }
                finally
                {
                    ppt.Package.SavePackage();
                    s3pi.Package.Package.ClosePackage(0, ppt.Package);
                }

                string command = ObjectCloner.Properties.Settings.Default.pkgEditorPath;
                string arguments = String.Format(@"""{0}""", filename);

                System.Diagnostics.Process p = new System.Diagnostics.Process();

                p.StartInfo.FileName = command;
                p.StartInfo.Arguments = arguments;
                p.StartInfo.UseShellExecute = false;

                try { p.Start(); remove = false; }
                catch (Exception ex)
                {
                    CopyableMessageBox.IssueException(ex,
                        String.Format("Application failed to start:\n{0}\n{1}", command, arguments),
                        "Launch failed");
                }
            }
            finally
            {
                if (remove)
                {
                    File.Delete(filename);
                }
            }
        }
        #endregion

        private void DisplayListView(Control listView)
        {
            this.AcceptButton = btnStart;
            this.CancelButton = null;

            setPrompt(listView == searchPane ? Prompt.Search : Prompt.CloneFix);

            StopWait();
            splitContainer1.Panel1.Controls.Clear();
            splitContainer1.Panel1.Controls.Add(listView);
            listView.Dock = DockStyle.Fill;
            listView.Focus();
        }

        private void DisplayObjectChooser()
        {
            Diagnostics.Log("DisplayObjectChooser");

            if (!CheckInstallDirs(null)) return;

            //Ensure there's only one place for DisplayOptions to drop back to on Cancel
            searchPane = null;
            
            DisplayListView(objectChooser);
            objectChooser.GetReady();
        }

        private void DisplaySearch()
        {
            Diagnostics.Log("DisplaySearch");

            //Ensure there's only one place for DisplayOptions to drop back to on Cancel
            objectChooser = null;

            searchPane.SelectedItem = null;
            DisplayListView(searchPane);
        }

        private void DisplayTGISearch()
        {
            Diagnostics.Log("DisplayTGISearch");

            this.AcceptButton = null;
            this.CancelButton = null;

            setPrompt(Prompt.TGISearch);

            StopWait();
            splitContainer1.Panel1.Controls.Clear();
            splitContainer1.Panel1.Controls.Add(tgiSearchPane);
            tgiSearchPane.Dock = DockStyle.Fill;
            tgiSearchPane.Focus();
        }


        string creatorName = null;
        string CreatorName
        {
            get
            {
                if (creatorName == null)
                {
                    if (ObjectCloner.Properties.Settings.Default.CreatorName == null || ObjectCloner.Properties.Settings.Default.CreatorName.Length == 0)
                        settingsUserName();
                }
                if (ObjectCloner.Properties.Settings.Default.CreatorName == null || ObjectCloner.Properties.Settings.Default.CreatorName.Length == 0)
                    creatorName = "";
                else
                    creatorName = ObjectCloner.Properties.Settings.Default.CreatorName;
                return creatorName;
            }
        }
        bool hasOBJDs()
        {
            switch (selectedItem.RequestedRK.CType())
            {
                case CatalogType.ModularResource:
                case CatalogType.CatalogFireplace:
                case CatalogType.CatalogObject:
                    return true;
            }
            return false;
        }
        private void DisplayOptions()
        {
            Diagnostics.Log("DisplayOptions");

            this.AcceptButton = null;
            this.CancelButton = null;

            setPrompt(Prompt.SelectOptions);

            if (searchPane != null)
                mode = Mode.FromGame;

            cloneFixOptions = new CloneFixOptions(this, mode == Mode.FromGame, selectedItem.RequestedRK.CType() == CatalogType.CAS_Part, hasOBJDs(), itemName == null || itemDesc == null);
            cloneFixOptions.CancelClicked += new EventHandler(cloneFixOptions_CancelClicked);
            cloneFixOptions.StartClicked += new EventHandler(cloneFixOptions_StartClicked);

            if (mode == Mode.FromGame)
            {
                string prefix = CreatorName;
                string name = GetResourceName(selectedItem);
                prefix = (prefix != null) ? prefix + "_" : "";
                cloneFixOptions.UniqueName = prefix + name;
            }
            else
            {
                cloneFixOptions.UniqueName = Path.GetFileNameWithoutExtension(FileTable.Current.Path);
                string[] split = cloneFixOptions.UniqueName.Split('_');
                uint oldHash;
                if (split.Length > 1 && split[split.Length - 1].Length == 8 && uint.TryParse(split[split.Length - 1], System.Globalization.NumberStyles.HexNumber, null, out oldHash))
                    cloneFixOptions.UniqueName = string.Join("_", split, 0, split.Length - 1);
            }
            cloneFixOptions.UniqueName += "_" + FNV32.GetHash(cloneFixOptions.UniqueName + DateTime.UtcNow.ToBinary().ToString("X16")).ToString("X8");

            StopWait();
            splitContainer1.Panel1.Controls.Clear();
            splitContainer1.Panel1.Controls.Add(cloneFixOptions);
            cloneFixOptions.Dock = DockStyle.Fill;
            cloneFixOptions.Focus();
        }


        private void DisplayReplaceTGI()
        {
            Diagnostics.Log("DisplayReplaceTGI");

            setPrompt(Prompt.ReplaceTGI);
            replaceTGIPane.CriteriaEnabled = true;
            replaceTGIPane.SaveEnabled = false;

            replaceTGIPane.FromCriteria = replaceTGIPane.ToCriteria = new ReplaceTGI.Criteria();
            replaceTGIPane.Results = "";

            StopWait();
            splitContainer1.Panel1.Controls.Clear();
            splitContainer1.Panel1.Controls.Add(replaceTGIPane);
            replaceTGIPane.Dock = DockStyle.Fill;
            replaceTGIPane.Focus();
        }

        private void DisplayReplaceTGIResults()
        {
            Diagnostics.Log("DisplayReplaceTGIResults");

            setPrompt(Prompt.SaveCancel);

            replaceTGIPane.CriteriaEnabled = false;
            replaceTGIPane.SaveEnabled = true;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (string s in replacements) sb.AppendLine(s);
            replaceTGIPane.Results = sb.ToString();

            StopWait();
            splitContainer1.Panel1.Controls.Clear();
            splitContainer1.Panel1.Controls.Add(replaceTGIPane);
            replaceTGIPane.Dock = DockStyle.Fill;
            replaceTGIPane.Focus();
        }

        private void DisplayFixIntegrityResults()
        {
            Diagnostics.Log("DisplayFixIntegrityResults");

            setPrompt(Prompt.SaveCancel);

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (string s in replacements) sb.AppendLine(s);
            fixIntegrityResults.Text = sb.ToString();

            StopWait();
            splitContainer1.Panel1.Controls.Clear();
            splitContainer1.Panel1.Controls.Add(fixIntegrityResults);
            fixIntegrityResults.Dock = DockStyle.Fill;
            fixIntegrityResults.Focus();
        }


        private void DisplayNothing()
        {
            Diagnostics.Log("DisplayNothing");

            objectChooser = null;
            searchPane = null;

            this.AcceptButton = null;
            this.CancelButton = null;

            setPrompt(Prompt.UseMenu);

            StopWait();
            ClearTabs();
            //TabEnable(false);
            splitContainer1.Panel1.Controls.Clear();
            if (cloneFixOptions != null)
            {
                cloneFixOptions.Enabled = false;
                cloneFixOptions.Dock = DockStyle.Fill;
                splitContainer1.Panel1.Controls.Add(cloneFixOptions);
            }
        }

        public delegate void DoWaitCallback(string waitText = "Please wait...");
        public delegate void StopWaitCallback(Control control);
        private void DoWait(string waitText = "Please wait...")
        {
            Diagnostics.Log("DoWait: " + waitText);

            flowLayoutPanel1.Visible = false;//hide prompt

            pleaseWait.Label = waitText;
            splitContainer1.Panel2.Enabled = false;
            TabEnable(false);//?
            this.Text = GetMyName() + " [busy]" + " " + waitText;

            splitContainer1.Panel1.Controls.Clear();
            splitContainer1.Panel1.Controls.Add(pleaseWait);
            pleaseWait.Dock = DockStyle.Fill;
            Application.DoEvents();
        }
        private void StopWait(Control control)
        {
            StopWait();
            splitContainer1.Panel1.Controls.Clear();
            splitContainer1.Panel1.Controls.Add(control);
            control.Dock = DockStyle.Fill;
        }
        private void StopWait()
        {
            if (pleaseWait != null && pleaseWait.Label != "") { Diagnostics.Log("StopWait: " + pleaseWait.Label); pleaseWait.Label = ""; }
            else Diagnostics.Log("StopWait");

            flowLayoutPanel1.Visible = true;

            splitContainer1.Panel2.Enabled = true;
            this.Text = GetMyName();
            Application.DoEvents();
        }
        #endregion

        #region CloneFixOptions
        string uniqueName = null;
        bool isFix = false;
        bool isRepair = false;
        bool isCreateNewPackage = false;
        bool isDeepClone = false;
        bool isPadSTBLs = false;
        bool isIncludeThumbnails = false;
        bool isRenumber = false;
        bool is32bitIIDs = false;
        bool isKeepSTBLIIDs = false;
        bool disableCompression = false;

        bool IsDeepClone { get { return isFix || isDeepClone; } }
        bool JustSelf { get { return !isCreateNewPackage && !isRepair && !isRenumber; } }
        bool WantThumbs { get { return isIncludeThumbnails || (!(isCreateNewPackage || isRepair) && isRenumber); } }

        void cloneFixOptions_StartClicked(object sender, EventArgs e)
        {
            Diagnostics.Log("cloneFixOptions_StartClicked");

            uniqueName = cloneFixOptions.UniqueName;
            isRepair = cloneFixOptions.IsRepair;
            isCreateNewPackage = cloneFixOptions.IsClone;
            isDeepClone = cloneFixOptions.IsDeepClone;
            isPadSTBLs = cloneFixOptions.IsPadSTBLs;
            isIncludeThumbnails = cloneFixOptions.IsIncludeThumbnails;
            isRenumber = cloneFixOptions.IsRenumber;
            is32bitIIDs = cloneFixOptions.Is32bitIIDs;
            isKeepSTBLIIDs = cloneFixOptions.IsKeepSTBLIIDs;
            disableCompression = !cloneFixOptions.IsCompress;

            if (isRepair)
                SetAppendEA(true);

            if (isCreateNewPackage) CloneStart();
            else FixStart();
        }

        void cloneFixOptions_CancelClicked(object sender, EventArgs e)
        {
            Diagnostics.Log("cloneFixOptions_CancelClicked");
            //sender was null if triggered by CloneStart, FixStart or RunRKLookup (all now commented out)
            if (sender != null && !IsOkayToThrowAwayWork())
                return;

            TabEnable(false);
            if (searchPane != null)
                DisplaySearch();
            else if (objectChooser != null)
                DisplayObjectChooser();
            else
                DisplayNothing();
        }

        void CloneStart()
        {
            Diagnostics.Log("CloneStart");

            isFix = false;

            uniqueObject = null;
            if (UniqueObject == null)
                return;

            saveFileDialog1.FileName = UniqueObject;
            if (ObjectCloner.Properties.Settings.Default.LastSaveFolder != null)
                saveFileDialog1.InitialDirectory = ObjectCloner.Properties.Settings.Default.LastSaveFolder;
            DialogResult dr = saveFileDialog1.ShowDialog();
            if (dr != DialogResult.OK)
                return;

            ObjectCloner.Properties.Settings.Default.LastSaveFolder = Path.GetDirectoryName(saveFileDialog1.FileName);
            Diagnostics.Log("CloneStart: " + saveFileDialog1.FileName);

            RunRKLookup();
            if (rkLookup != null)
                StartSaving();
        }

        void FixStart()
        {
            Diagnostics.Log("FixStart");

            isFix = true;

            uniqueObject = null;
            if (isRenumber && UniqueObject == null)
                return;

            RunRKLookup();
            if (rkLookup != null)
            {
                if (isRepair)
                    RunRepair();//then StartFixing
                else
                    StartFixing();
            }
        }
        #endregion

        #region Tabs
        CatalogType tabType = 0;

        static Dictionary<string, string> fieldToLabelMap;//(Type:)fieldname -> Label
        static Dictionary<string, string> labelToFieldMap;//(Type:)Label -> fieldname
        static Dictionary<string, string> otherFieldMap;
        static List<flagField> flagFields;
        static Dictionary<List<Type>, int> typeToLen = null;
        void InitialiseMaps()
        {
            if (fieldToLabelMap == null)
            {
                fieldToLabelMap = new Dictionary<string, string>();
                fieldToLabelMap.Add("Common:FireType", "Fire Type");
                fieldToLabelMap.Add("Common:IsStealable", "Stealable");
                fieldToLabelMap.Add("Common:IsReposessable", "Reposessable");
                fieldToLabelMap.Add("Common:IsPlaceableOnRoof", "Placeable On Roof");
                fieldToLabelMap.Add("Common:IsVisibleInWorldBuilder", "Visible In World Builder");
                fieldToLabelMap.Add("ObjectCatalogResource:MoodletGiven", "Moodlet Given");
                fieldToLabelMap.Add("ObjectCatalogResource:MoodletScore", "Moodlet Score");
                fieldToLabelMap.Add("ObjectCatalogResource:TopicRatings", "Topic/Ratings");
                fieldToLabelMap.Add("TerrainPaintBrushCatalogResource:Category", "Category");
            }
            if (labelToFieldMap == null)
            {
                labelToFieldMap = new Dictionary<string, string>();
                labelToFieldMap.Add("Common:Fire Type", "FireType");
                labelToFieldMap.Add("Common:Stealable", "IsStealable");
                labelToFieldMap.Add("Common:Reposessable", "IsReposessable");
                labelToFieldMap.Add("Common:Placeable On Roof", "IsPlaceableOnRoof");
                labelToFieldMap.Add("Common:Visible In World Builder", "IsVisibleInWorldBuilder");
                labelToFieldMap.Add("ObjectCatalogResource:Moodlet Given", "MoodletGiven");
                labelToFieldMap.Add("ObjectCatalogResource:Moodlet Score", "MoodletScore");
                labelToFieldMap.Add("ObjectCatalogResource:Topic/Ratings", "TopicRatings");
                labelToFieldMap.Add("TerrainPaintBrushCatalogResource:Category", "Category");
            }
            if (otherFieldMap == null)
            {
                otherFieldMap = new Dictionary<string, string>();
            }
            if (flagFields == null)
            {
                flagFields = new List<flagField>(new flagField[] {
                    new flagField(tlpObjectTypeFlags, "ObjectTypeFlags", 32, 0),
                    new flagField(tlpObjectTypeFlags2, "ObjectTypeFlags2", 32, 0),//Version>=0x1a
                    new flagField(tlpMovementFlags, "MovementFlags", 32, 0),
                    new flagField(tlpRoomSort, "RoomCategoryFlags", 32, 0),
                    new flagField(tlpRoomSubLow, "RoomSubCategoryFlags", 32, 0),
                    new flagField(tlpRoomSubHigh, "RoomSubCategoryFlags", 64, 32),
                    new flagField(tlpFuncSort, "FunctionCategoryFlags", 32, 0),
                    new flagField(tlpFuncSubLow, "FunctionSubCategoryFlags", 32, 0),
                    new flagField(tlpFuncSubHigh, "FunctionSubCategoryFlags", 64, 32),
                    new flagField(tlpFuncSub2Low, "FunctionSubCategoryFlags2", 32, 0),//Version>=0x1c
                    new flagField(tlpFuncSub2High, "FunctionSubCategoryFlags2", 64, 32),//Version>=0x1c
                    new flagField(tlpBuildSort, "BuildCategoryFlags", 32, 0),
                    new flagField(tlpWallPlacementFlags, "WallPlacementFlags", 32, 0),
                });
            }
            if (typeToLen == null)
            {
                typeToLen = new Dictionary<List<Type>, int>();
                typeToLen.Add(new List<Type>(new Type[] { typeof(sbyte), typeof(byte), }), 2 + 2);
                typeToLen.Add(new List<Type>(new Type[] { typeof(bool), typeof(char), typeof(short), typeof(ushort), }), 2 + 4);
                typeToLen.Add(new List<Type>(new Type[] { typeof(float), }), 8);
                typeToLen.Add(new List<Type>(new Type[] { typeof(int), typeof(uint), }), 2 + 8);
                typeToLen.Add(new List<Type>(new Type[] { typeof(double), }), 16);
                typeToLen.Add(new List<Type>(new Type[] { typeof(long), typeof(ulong), }), 2 + 16);
                typeToLen.Add(new List<Type>(new Type[] { typeof(decimal), typeof(DateTime), typeof(string), typeof(object), }), 30);
            }
        }

        struct flagField
        {
            public TableLayoutPanel tlp;
            public string field;
            public int length;
            public int offset;
            public flagField(TableLayoutPanel tlp, string field, int length, int offset) { this.tlp = tlp; this.field = field; this.length = length; this.offset = offset; }
        }
        struct ResField
        {
            public IContentFields res;
            public string field;
            public ResField(IContentFields res, string field) { this.res = res; this.field = field; }
        }
        ResField mapResField(Dictionary<string, string> map, ResField resField)
        {
            ResField result;
            if (map.ContainsKey("Common:" + resField.field))
                result.res = resField.res["CommonBlock"].Value as AApiVersionedFields;
            else
                result.res = resField.res;
            string lookup = result.res.GetType().Name + ":" + resField.field;
            result.field = map.ContainsKey(lookup) ? map[lookup]
                : map.ContainsKey(resField.field) ? map[resField.field]
                : resField.field;
            return result;
        }
        ResField fieldToLabel(ResField field) { return mapResField(fieldToLabelMap, field); }
        ResField labelToField(ResField label) { return mapResField(labelToFieldMap, label); }

        string GetTGIBlockListContentField(Type t, string field)
        {
            System.Reflection.PropertyInfo pi = t.GetProperty(field);
            foreach (Attribute attr in pi.GetCustomAttributes(typeof(TGIBlockListContentFieldAttribute), true))
                return (attr as TGIBlockListContentFieldAttribute).TGIBlockListContentField;
            return null;
        }
        bool HasTGIBlockListContentFieldAttribute(Type t, string field) { return GetTGIBlockListContentField(t, field) != null; }

        int getFieldLength(Type t)
        {
            InitialiseMaps();

            foreach (List<Type> tt in typeToLen.Keys)
                if (tt.Contains(typeof(Enum).IsAssignableFrom(t) ? Enum.GetUnderlyingType(t) : t))
                    return typeToLen[tt];
            return 30;
        }

        void IterateTLP(TableLayoutPanel tlp, Action<Label, Control> action)
        {
            for (int i = 1; i < tlp.RowCount - 1; i++)
                action((Label)tlp.GetControlFromPosition(0, i), tlp.GetControlFromPosition(1, i));
        }

        void InitialiseTabs(CatalogType resourceType)
        {
            Diagnostics.Log("InitialiseTabs: " + resourceType);
            if (tabType == resourceType) return;

            string appWas = this.Text;
            this.Text = GetMyName() + " [busy] Initialising tabs...";
            Application.UseWaitCursor = true;
            Application.DoEvents();
            try
            {
                tabType = resourceType;

                if (resourceType == CatalogType.ModularResource) resourceType = CatalogType.CatalogObject;//Modular Resources - display OBJD0

                IResource res = s3pi.WrapperDealer.WrapperDealer.CreateNewResource(0, "0x" + ((uint)resourceType).ToString("X8"));
                this.tabControl1.TabPages.Clear();
                if (tabType == CatalogType.CAS_Part)
                {
                    InitialiseCASP();
                    this.tabControl1.TabPages.Add(this.tpCASP);
                }
                else
                {
                    this.tabControl1.TabPages.Add(this.tpMain);
                    if (tabType == CatalogType.CatalogObject || tabType == CatalogType.CatalogTerrainPaintBrush)
                    {
                        InitialiseDetailsTab(res);
                        this.tabControl1.TabPages.Add(this.tpDetail);
                        if (tabType == CatalogType.CatalogObject)
                        {
                            using (Splash splash = new Splash() { Message = "Initialising tabs...", })
                            {
                                splash.Show();
                                Application.DoEvents();
                                InitialiseFlagTabs(res);
                                if (tlpOther.Visible)
                                    InitialiseOtherTab(res);
                                this.tabControl1.TabPages.Add(this.tpFlagsRoom);
                                this.tabControl1.TabPages.Add(this.tpFlagsFunc1);
                                this.tabControl1.TabPages.Add(this.tpFlagsFunc2);
                                this.tabControl1.TabPages.Add(this.tpFlagsMisc1);
                                this.tabControl1.TabPages.Add(this.tpFlagsMisc2);
                            }
                        }
                    }
                }
            }
            finally { this.Text = appWas; Application.UseWaitCursor = false; Application.DoEvents(); }
        }
        void InitialiseDetailsTab(IResource catlg) { InitialiseMaps(); InitialiseTabTLP(catlg, tlpObjectDetail, labelToFieldMap.Values); }
        void InitialiseOtherTab(IResource objd) { InitialiseMaps(); InitialiseTabTLP(objd, tlpOther, otherFieldMap.Keys); }

        void InitialiseCASP()
        {
            InitialiseCASPCombo(cbCASPClothingType, typeof(CASPartResource.ClothingType));
            InitialiseCASPCombo(cbCASPTypeFlags, typeof(CASPartResource.DataTypeFlags));
            InitialiseCASPFlags(clbCASPAgeFlags, typeof(CASPartResource.AgeFlags));
            InitialiseCASPCombo(cbCASPSpeciesType, typeof(CASPartResource.SpeciesType));
            InitialiseCASPFlags(clbCASPGenderFlags, typeof(CASPartResource.GenderFlags));
            InitialiseCASPFlags(clbCASPHandednessFlags, typeof(CASPartResource.HandednessFlags));
            InitialiseCASPCombo(cbCASPRegionType, typeof(CASPartResource.RegionType));
            InitialiseClothingCategoryFlags(clbCASPCategory, typeof(CASPartResource.ClothingCategoryFlags));
        }
        void InitialiseCASPCombo(ComboBox cb, Type enumType)
        {
            cb.Items.Clear();
            cb.Items.AddRange(Enum.GetNames(enumType).Where(x => !x.StartsWith("Unknown")).ToArray());
        }
        void InitialiseCASPFlags(CheckedListBox clb, Type enumType)
        {
            clb.Items.Clear();
            clb.Items.AddRange(Enum.GetNames(enumType).Where(x => !x.StartsWith("Unknown")).ToArray());
        }
        void InitialiseClothingCategoryFlags(CheckedListBox clb, Type enumType)
        {
            clb.Items.Clear();
            clb.Items.AddRange(Enum.GetNames(enumType).Where(x => !x.StartsWith("Unknown") && !x.StartsWith("Region")).ToArray());
        }

        bool ffInitialised = false;
        void InitialiseFlagTabs(IResource objd)
        {
            if (ffInitialised) return;
            ffInitialised = true;

            InitialiseMaps();
            flagFields.ForEach(ff => InitialiseTLP(ff.tlp));

            foreach (flagField ff in flagFields)
            {
                Application.DoEvents();
                Type t = AApiVersionedFields.GetContentFieldTypes(0, objd.GetType())[ff.field];
                CheckBox[] ackb = new CheckBox[ff.length - ff.offset];
                for (int i = 0; i < ackb.Length; i++) ackb[i] = new CheckBox();
                ff.tlp.RowCount = 2 + ackb.Length;

                for (int i = ff.offset; i < ff.length; i++)
                {
                    ulong value = (ulong)Math.Pow(2, i);
                    string s = (Enum)Enum.ToObject(t, value) + "";
                    if (s.Equals(value.ToString())) s = "-";
                    CreateField(ff, s, ackb, i - ff.offset);
                }

                ff.tlp.Controls.AddRange(ackb);
            }
        }

        void InitialiseTLP(TableLayoutPanel tlp)
        {
            while (tlp.RowCount > 2)
            {
                for (int i = 0; i < tlp.ColumnCount; i++)
                {
                    Control c = tlp.GetControlFromPosition(i, tlp.RowCount - 2);
                    tlp.Controls.Remove(c);
                }
                tlp.RowCount--;
            }
        }
        void InitialiseTabTLP(IResource res, TableLayoutPanel tlp, IEnumerable<String> fields)
        {
            InitialiseTLP(tlp);

            foreach (string field in fields)
            {
                ResField resField = new ResField(res, field);
                ResField resLabel = fieldToLabel(resField);
                if (AApiVersionedFields.GetContentFields(0, resLabel.res.GetType()).Contains(field))
                    CreateField(tlp, resField, true);
            }
        }
        void CreateField(TableLayoutPanel target, ResField resField) { CreateField(target, resField, false); }
        void CreateField(TableLayoutPanel target, ResField resField, bool validate)
        {
            ResField resLabel = fieldToLabel(resField);
            Type type = AApiVersionedFields.GetContentFieldTypes(0, resLabel.res.GetType())[resField.field];

            target.RowCount++;
            target.RowStyles.Insert(target.RowCount - 2, new RowStyle(SizeType.AutoSize));

            Label lb = new Label();
            lb.Anchor = AnchorStyles.Right;
            lb.AutoSize = true;
            lb.Name = "lb" + resField.field;
            lb.TabIndex = target.RowCount * 2;
            lb.Text = resLabel.field;
            target.Controls.Add(lb, 0, target.RowCount - 2);

            if (HasTGIBlockListContentFieldAttribute(resLabel.res.GetType(), resField.field))
            {
                TGIBlockCombo tbc = new TGIBlockCombo();
                tbc.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                tbc.Name = "tbc" + resField.field;
                tbc.Enabled = false;
                tbc.Margin = new Padding(3, 0, 0, 0);
                tbc.ShowEdit = false;
                tbc.TabIndex = target.RowCount * 2 + 1;
                tbc.Tag = true;
                tbc.Width = (int)target.ColumnStyles[1].Width;
                target.Controls.Add(tbc, 1, target.RowCount - 2);
            }
            else if (resField.field.Equals("TopicRatings"))
            {
                CustomControls.TopicRatings tr = new CustomControls.TopicRatings();
                tr.Anchor = AnchorStyles.Left;
                tr.Name = "tr" + resField.field;
                tr.ReadOnly = true;
                tr.TabIndex = target.RowCount * 2 + 1;
                if (validate) tr.Tag = true;
                target.Controls.Add(tr, 1, target.RowCount - 2);
            }
            else if (typeof(Enum).IsAssignableFrom(type))
            {
                CustomControls.EnumTextBox etb = new CustomControls.EnumTextBox();
                etb.EnumType = type;
                etb.Anchor = AnchorStyles.Left;
                etb.Name = "etb" + resField.field;
                etb.ReadOnly = true;
                etb.TabIndex = target.RowCount * 2 + 1;
                if (validate) etb.Tag = true;
                target.Controls.Add(etb, 1, target.RowCount - 2);
            }
            else if (typeof(Boolean).Equals(type))
            {
                CheckBox ckb = new CheckBox();
                ckb.Anchor = AnchorStyles.Left;
                ckb.AutoCheck = false;
                ckb.Name = "ckb" + resField.field;
                ckb.TabIndex = target.RowCount * 2 + 1;
                if (validate) ckb.Tag = true;
                target.Controls.Add(ckb, 1, target.RowCount - 2);
            }
            else
            {
                Label x = new Label();
                x.AutoSize = true;
                x.Text = "".PadLeft(getFieldLength(type), 'X');

                TextBox tb = new TextBox();
                tb.Anchor = AnchorStyles.Left;
                tb.Name = "tb" + resField.field;
                tb.ReadOnly = true;
                tb.Size = new Size(x.PreferredWidth + 6, x.PreferredHeight + 6);
                tb.TabIndex = target.RowCount * 2 + 1;
                if (validate)
                {
                    tb.Validating += new CancelEventHandler(tb_Validating);
                    tb.Tag = true;
                }
                target.Controls.Add(tb, 1, target.RowCount - 2);
            }
        }
        void CreateField(flagField ff, string name, CheckBox[] acbk, int i)
        {
            ff.tlp.RowStyles.Insert(i + 1, new RowStyle(SizeType.AutoSize));
            ff.tlp.SetCellPosition(acbk[i], new TableLayoutPanelCellPosition(0, i + 1));

            acbk[i].Anchor = AnchorStyles.Left;
            acbk[i].AutoSize = true;
            acbk[i].Name = "cb" + name;
            acbk[i].Text = name;
            acbk[i].TabIndex = ff.tlp.RowCount;
        }

        void ClearTabs()
        {
            Diagnostics.Log("ClearTabs");
            string appWas = this.Text;
            this.Text = GetMyName() + " [busy] Clearing tabs...";

            Application.UseWaitCursor = true;
            Application.DoEvents();
            try
            {
                clearOverview();
                clearCASP();
                if (tabControl1.Contains(tpDetail))
                    clearDetails();
                if (tabControl1.Contains(tpFlagsRoom))
                {
                    clearFlags();
                    clearOther();
                }
                TabEnable(false);
            }
            finally { this.Text = appWas; Application.UseWaitCursor = false; Application.DoEvents(); }
        }
        void clearOverview()
        {
            pbCatlgThum.Image = null;
            lbTGICatlgThum.Text = "0x00000000-0x00000000-0x0000000000000000";
            tbResourceName.Text = "";
            tbObjName.Text = "";
            tbNameGUID.Text = "";
            tbCatlgName.Text = "";
            tbObjDesc.Text = "";
            tbDescGUID.Text = "";
            tbCatlgDesc.Text = "";
            ckbCopyToAll.Checked = false;
            tbPrice.Text = "";
            tbProductStatus.Text = "";
            tbPackage.Text = "";
        }
        void clearCASP()
        {
            pbCASPThum.Image = null;
            lbTGICASPThum.Text = "0x00000000-0x00000000-0x0000000000000000";
            tbCASPResourceName.Text = "";
            tbCASPUnknown1.Text = "";
            cbCASPClothingType.SelectedIndex = -1;
            cbCASPTypeFlags.SelectedIndex = -1;
            for (int i = 0; i < clbCASPAgeFlags.Items.Count; i++) clbCASPAgeFlags.SetItemChecked(i, false);
            cbCASPSpeciesType.SelectedIndex = -1;
            for (int i = 0; i < clbCASPGenderFlags.Items.Count; i++) clbCASPGenderFlags.SetItemChecked(i, false);
            for (int i = 0; i < clbCASPHandednessFlags.Items.Count; i++) clbCASPHandednessFlags.SetItemChecked(i, false);
            cbCASPRegionType.SelectedIndex = -1;
            for (int i = 0; i < clbCASPCategory.Items.Count; i++) clbCASPCategory.SetItemChecked(i, false);
            tbCASPUnknown4.Text = "";
            tbCASPPackage.Text = "";
        }
        void clearDetails() { IterateTLP(tlpObjectDetail, clearTLP); }
        void clearOther() { IterateTLP(tlpOther, clearTLP); }
        void clearFlags()
        {
            foreach (flagField ff in flagFields)
                foreach (Control c in ff.tlp.Controls)
                    if (c is CheckBox) ((CheckBox)c).Checked = false;
        }
        void clearTLP(Label l, Control c)
        {
            if (c is TGIBlockCombo) { TGIBlockCombo tbc = c as TGIBlockCombo; tbc.SelectedIndex = -1; }
            else if (c is CustomControls.TopicRatings) { CustomControls.TopicRatings tr = c as CustomControls.TopicRatings; tr.Clear(); tr.ReadOnly = true; }
            else if (c is CustomControls.EnumTextBox) { CustomControls.EnumTextBox etb = c as CustomControls.EnumTextBox; etb.Clear(); etb.ReadOnly = true; }
            else if (c is CheckBox) { CheckBox ckb = c as CheckBox; ckb.AutoCheck = ckb.Checked = false; }
            else if (c is TextBox) { TextBox tb = c as TextBox; tb.Text = ""; tb.ReadOnly = true; }
        }

        void FillTabs(SpecificResource item)
        {
            Diagnostics.Log("FillTabs: " + item);
            string appWas = this.Text;
            this.Text = GetMyName() + " [busy] Filling tabs...";

            Application.UseWaitCursor = true;
            Application.DoEvents();
            try
            {
                if (formClosing || item.Resource == null)
                {
                    ClearTabs();
                    return;
                }

                InitialiseTabs(item.RequestedRK.CType());
                if (formClosing) return;

                SpecificResource catlg = (item.RequestedRK.CType() == CatalogType.ModularResource) ? ItemForTGIBlock0(item) : item;

                if (catlg.ResourceIndexEntry != null)
                {
                    if (tabControl1.Contains(tpCASP))
                        fillCASPTab(catlg);
                    else
                    {
                        fillOverview(catlg);
                        if (formClosing) return;
                        if (tabControl1.Contains(tpDetail))
                            fillDetails(catlg);
                        if (formClosing) return;
                        if (item.RequestedRK.CType() == CatalogType.CatalogObject)
                        {
                            fillFlags(catlg);
                            if (formClosing) return;
                            fillOther(catlg);
                        }
                    }
                }
                else
                    fillOverviewNoTGI0(item);
            }
            finally { this.Text = appWas; Application.UseWaitCursor = false; Application.DoEvents(); }
            TabEnable(false);
        }

        string itemName = null;
        string itemDesc = null;
        void fillOverview(SpecificResource item)
        {
            lbTGICatlgThum.Font = new Font(lbTGICatlgThum.Font, FontStyle.Regular);
            tbObjName.Font = tbNameGUID.Font =
            tbCatlgName.Font = tbObjDesc.Font = tbDescGUID.Font =
            tbCatlgDesc.Font = tbPrice.Font = tbProductStatus.Font =
                new Font(tbObjName.Font, FontStyle.Regular);

            AApiVersionedFields common = item.Resource["CommonBlock"].Value as AApiVersionedFields;

            if (formClosing) return;
            SpecificResource thumSR = THUM.getTHUM(THUM.THUMSize.large, item);
            if (thumSR == null)
            {
                pbCatlgThum.Image = null;
                lbTGICatlgThum.Text = "0x00000000-0x00000000-0x0000000000000000";
            }
            else
            {
                pbCatlgThum.Image = ResizeImage(Image.FromStream(thumSR.Resource.Stream), pbCatlgThum);
                lbTGICatlgThum.Text = "" + (AResourceKey)thumSR.RequestedRK;
            }

            if (formClosing) return; else tbResourceName.Text = NameMap.NMap[item.RequestedRK.Instance];
            tbObjName.Text = common["Name"].Value + "";
            tbNameGUID.Text = common["NameGUID"] + "";

            try
            {
                if (formClosing) return; else updateProgress(true, "Looking up NameGUID...", true, 0x17, true, 0);
                Application.DoEvents();
                itemName = STBLHandler.StblLookup((ulong)common["NameGUID"].Value, -1, x => { if (formClosing) return; else updateProgress(false, "", false, 0, true, x); Application.DoEvents(); });
                tbCatlgName.Text = itemName == null ? "" : itemName;
            }
            finally { if (!formClosing) updateProgress(true, "", true, -1, false, 0); Application.DoEvents(); }

            tbObjDesc.Text = common["Desc"].Value + "";
            tbDescGUID.Text = common["DescGUID"] + "";

            try
            {
                if (formClosing) return; else updateProgress(true, "Looking up DescGUID...", true, 0x17, true, 0);
                Application.DoEvents();
                itemDesc = STBLHandler.StblLookup((ulong)common["DescGUID"].Value, -1, x => { if (formClosing) return; else updateProgress(false, "", false, 0, true, x); Application.DoEvents(); });
                tbCatlgDesc.Text = itemDesc == null ? "" : itemDesc;
            }
            finally { if (!formClosing) updateProgress(true, "", true, -1, false, 0); Application.DoEvents(); }

            tbPrice.Text = common["Price"].Value + "";
            tbProductStatus.Text = "0x" + ((byte)common["BuildBuyProductStatusFlags"].Value).ToString("X2");
            tbPackage.Text = item.PathPackage.Path;
        }
        const string noData = "(No data available)";
        void fillOverviewNoTGI0(SpecificResource item)
        {
            tbResourceName.Text = NameMap.NMap[item.RequestedRK.Instance];
            tbPackage.Text = item.PathPackage.Path;

            tpMain.Tag = noData;
            lbTGICatlgThum.Text = tbObjName.Text = tbNameGUID.Text =
            tbCatlgName.Text = tbObjDesc.Text = tbDescGUID.Text =
            tbCatlgDesc.Text = tbPrice.Text = tbProductStatus.Text =
                noData;
            lbTGICatlgThum.Font = new Font(lbTGICatlgThum.Font, FontStyle.Italic);
            tbObjName.Font = tbNameGUID.Font =
            tbCatlgName.Font = tbObjDesc.Font = tbDescGUID.Font =
            tbCatlgDesc.Font = tbPrice.Font = tbProductStatus.Font =
                new Font(tbObjName.Font, FontStyle.Italic);
        }

        void fillCASPTab(SpecificResource item)
        {
            CASPartResource.CASPartResource casp = item.Resource as CASPartResource.CASPartResource;
            if (casp == null) { clearCASP(); return; }

            if (formClosing) return;
            SpecificResource thumSR = THUM.getTHUM(THUM.THUMSize.large, item);
            if (thumSR == null)
            {
                pbCASPThum.Image = null;
                lbTGICASPThum.Text = "0x00000000-0x00000000-0x0000000000000000";
            }
            else
            {
                pbCASPThum.Image = ResizeImage(Image.FromStream(thumSR.Resource.Stream), pbCASPThum);
                lbTGICASPThum.Text = "" + (AResourceKey)thumSR.RequestedRK;
            }

            tbCASPResourceName.Text = NameMap.NMap[item.RequestedRK.Instance];
            tbCASPPackage.Text = item.PathPackage.Path;

            tbCASPUnknown1.Text = casp.Unknown1;
            tbCASPUnknown4.Text = casp.Unknown4;

            cbCASPClothingType.SelectedIndex = enumIsDefinedAndNotUnknown(casp.Clothing);
            cbCASPTypeFlags.SelectedIndex = enumIsDefinedAndNotUnknown(casp.DataType);
            fillCASPFlags(clbCASPAgeFlags, typeof(CASPartResource.AgeFlags), (uint)casp.AgeGender.Age);
            cbCASPSpeciesType.SelectedIndex = enumIsDefinedAndNotUnknown(casp.AgeGender.Species);
            fillCASPFlags(clbCASPGenderFlags, typeof(CASPartResource.GenderFlags), (uint)casp.AgeGender.Gender);
            fillCASPFlags(clbCASPHandednessFlags, typeof(CASPartResource.HandednessFlags), (uint)casp.AgeGender.Handedness);
            CASPartResource.RegionType region = CASPartResource.Extensions.GetRegionType(casp.ClothingCategory);
            cbCASPRegionType.SelectedIndex = enumIsDefinedAndNotUnknown(region);
            fillClothingCategoryFlags(clbCASPCategory, typeof(CASPartResource.ClothingCategoryFlags), (uint)casp.ClothingCategory);
        }
        static int enumIsDefinedAndNotUnknown<T>(T value)
        {
            return Enum.GetNames(typeof(T)).Where(x => !x.StartsWith("Unknown")).ToList().IndexOf(Enum.GetName(typeof(T), value));
        }
        void fillCASPFlags(CheckedListBox clb, Type enumType, uint value)
        {
            string[] names = Enum.GetNames(enumType);

            int index = 0;
            for (int bit = 0; bit < names.Length; bit++)
                if (!names[bit].StartsWith("Unknown")) clb.SetItemChecked(index++, bitset(value, bit));
        }
        void fillClothingCategoryFlags(CheckedListBox clb, Type enumType, uint value)
        {
            string[] names = Enum.GetNames(enumType);

            int index = 0;
            for (int bit = 0; bit < names.Length; bit++)
                if (!names[bit].StartsWith("Unknown") && !names[bit].StartsWith("Region")) clb.SetItemChecked(index++, bitset(value, bit));
        }
        bool bitset(uint value, int bit) { return (value & (1 << bit)) != 0; }

        void fillDetails(SpecificResource item) { IterateTLP(tlpObjectDetail, (l, c) => fillControl(item, l, c)); }
        void fillOther(SpecificResource item) { IterateTLP(tlpOther, (l, c) => fillControl(item, l, c)); }
        void fillFlags(SpecificResource item)
        {
            foreach (flagField ff in flagFields)
            {
                if (item.Resource.ContentFields.Contains(ff.field))
                {
                    ulong field = getFlags(item.Resource as AResource, ff.field);
                    for (int i = 1; i < ff.tlp.RowCount - 1; i++)
                    {
                        if (formClosing) return;
                        ulong value = (ulong)Math.Pow(2, ff.offset + i - 1);
                        CheckBox cb = (CheckBox)ff.tlp.GetControlFromPosition(0, i);
                        cb.Checked = (field & value) != 0;
                    }
                }
            }
        }

        void fillControl(SpecificResource item, Label lb, Control c)
        {
            if (formClosing) return;

            ResField resField = labelToField(new ResField(item.Resource, lb.Text));

            if (!resField.res.ContentFields.Contains(resField.field)) return;//Leave field empty and readonly if not present

            TypedValue tv = resField.res[resField.field];

            if (c is TGIBlockCombo)
            {
                TGIBlockList tgiBlocks = item.Resource["TGIBlocks"].Value as TGIBlockList;
                TGIBlockCombo tbc = c as TGIBlockCombo;
                tbc.TGIBlocks = tgiBlocks;
                int index = (int)(uint)tv.Value;
                tbc.SelectedIndex = index >= 0 && index < tgiBlocks.Count ? index : -1;
            }
            else if (c is CustomControls.TopicRatings)
            {
                CustomControls.TopicRatings tr = c as CustomControls.TopicRatings;
                tr.GetType().GetProperty("Value").SetValue(tr, tv.Value, new object[] { });
                tr.ReadOnly = false;
            }
            else if (c is CustomControls.EnumTextBox)
            {
                CustomControls.EnumTextBox etb = c as CustomControls.EnumTextBox;
                etb.EnumType = tv.Type;
                etb.Value = Convert.ToUInt64(tv.Value);
                etb.ReadOnly = false;
            }
            else if (c is CheckBox)
            {
                CheckBox ckb = c as CheckBox;
                ckb.Checked = (Boolean)tv.Value;
                ckb.AutoCheck = true;
            }
            else if (c is TextBox)
            {
                TextBox tb = c as TextBox;
                tb.Text = tv;
                tb.ReadOnly = c.Tag == null;
            }
        }

        void fillOverviewUpdateImage(SpecificResource item)
        {
            if (formClosing) return;
            if (pbCatlgThum.Image == null)
            {
                pbCatlgThum.Image = ResizeImage(THUM.getLargestThumbOrDefault(item), pbCatlgThum);
                lbTGICatlgThum.Text = (AResourceKey)THUM.getNewRK(THUM.THUMSize.large, item);
            }
        }

        void fillCASPUpdateImage(SpecificResource item)
        {
            if (formClosing) return;
            if (pbCASPThum.Image == null)
            {
                pbCASPThum.Image = ResizeImage(THUM.getLargestThumbOrDefault(item), pbCASPThum);
                lbTGICASPThum.Text = (AResourceKey)THUM.getNewRK(THUM.THUMSize.large, item);
            }
        }

        void TabEnable(bool enabled)
        {
            Diagnostics.Log((enabled ? "En" : "Dis") + "abling tabs...");
            string appWas = this.Text;
            this.Text = GetMyName() + " [busy] " + (enabled ? "En" : "Dis") + "abling tabs...";

            Application.UseWaitCursor = true;
            Application.DoEvents();
            try
            {
                if (tabControl1.Contains(tpCASP))
                    tabEnableCASP(enabled);
                else
                {
                    tabEnableOverview(enabled);
                    if (tabControl1.Contains(tpDetail))
                        tabEnableDetails(enabled);
                    if (tabControl1.Contains(tpFlagsRoom))
                    {
                        tabEnableFlags(enabled);
                        tabEnableOther(enabled);
                    }
                }
            }
            finally { this.Text = appWas; Application.UseWaitCursor = false; Application.DoEvents(); }
        }
        void tabEnableOverview(bool enabled)
        {
            enabled &= !((string)tpMain.Tag == noData);
            btnReplCatlgThum.Enabled = enabled;
            tbCatlgName.ReadOnly = !enabled;// || itemName == null;
            tbCatlgDesc.ReadOnly = !enabled;// || itemDesc == null;
            ckbCopyToAll.Enabled = enabled;
            tbPrice.ReadOnly = !enabled;
            tbProductStatus.ReadOnly = !enabled;
            tbCatlgName.BackColor = tbCatlgName.ReadOnly ? SystemColors.Control : SystemColors.Window;
            tbCatlgDesc.BackColor = tbCatlgDesc.ReadOnly ? SystemColors.Control : SystemColors.Window;
        }
        void tabEnableCASP(bool enabled)
        {
            //tbCASPResourceName.Text = "";
            btnReplCASPThum.Enabled = enabled;
            tbCASPUnknown1.ReadOnly = !enabled;
            cbCASPClothingType.Enabled = enabled;
            cbCASPTypeFlags.Enabled = enabled;
            clbCASPAgeFlags.Enabled = enabled;
            clbCASPGenderFlags.Enabled = enabled;
            cbCASPSpeciesType.Enabled = enabled;
            clbCASPHandednessFlags.Enabled = enabled;
            cbCASPRegionType.Enabled = enabled;
            clbCASPCategory.Enabled = enabled;
            tbCASPUnknown4.ReadOnly = !enabled;
            //tbCASPPackage.Text = "";
        }
        void tabEnableDetails(bool enabled) { IterateTLP(tlpObjectDetail, (l, c) => { if (c.Tag != null) c.Enabled = enabled; }); }
        void tabEnableOther(bool enabled) { IterateTLP(tlpOther, (l, c) => { if (c.Tag != null) c.Enabled = enabled; }); }
        void tabEnableFlags(bool enabled)
        {
            foreach (flagField ff in flagFields)
            {
                if (enabled && !selectedItem.Resource.ContentFields.Contains(ff.field))
                    ff.tlp.Enabled = false;
                else
                {
                    ff.tlp.Enabled = true;
                    for (int i = 1; i < ff.tlp.RowCount - 1; i++)
                    {
                        CheckBox cb = (CheckBox)ff.tlp.GetControlFromPosition(0, i);
                        cb.Enabled = enabled;
                    }
                }
            }
        }

        void tb_Validating(object sender, CancelEventArgs e)
        {
            TextBox tb = sender as TextBox;
            Type t = tb.Tag as Type;
            if (t == null) return;

            try { object val = tb_Value(tb, t); }
            catch { e.Cancel = true; }

            if (e.Cancel) tb.SelectAll();
        }

        ulong getFlags(AApiVersionedFields owner, string field)
        {
            TypedValue tv = owner[field];
            object o = Convert.ChangeType(tv.Value, Enum.GetUnderlyingType(tv.Type));
            if (o.GetType().Equals(typeof(byte))) return (byte)o;
            if (o.GetType().Equals(typeof(ushort))) return (ushort)o;
            if (o.GetType().Equals(typeof(uint))) return (uint)o;
            return (ulong)o;
        }
        ulong getFlags(flagField ff)
        {
            ulong res = 0;
            for (int i = ff.offset; i < ff.length; i++)
            {
                CheckBox cb = (CheckBox)ff.tlp.GetControlFromPosition(0, i - ff.offset + 1);
                if (cb.Checked) res += ((ulong)1 << i);
            }
            return res;
        }

        void UpdateItem(SpecificResource item, Label lb, Control c)
        {
            if (c.Tag == null) return;//skip if read only
            ResField resField = labelToField(new ResField(item.Resource, lb.Text));

            if (!resField.res.ContentFields.Contains(resField.field)) return;//skip if not present

            TypedValue tvOld = resField.res[resField.field];

            if (c is TGIBlockCombo)
            {
                resField.res[resField.field] = new TypedValue(tvOld.Type, Convert.ChangeType((c as TGIBlockCombo).SelectedIndex, tvOld.Type), "X");
            }
            else if (c is CustomControls.TopicRatings)
            {
                resField.res[resField.field] = new TypedValue(tvOld.Type, tr_Value(c as CustomControls.TopicRatings), "X");
            }
            else if (c is CustomControls.EnumTextBox)
            {
                resField.res[resField.field] = new TypedValue(tvOld.Type, etb_Value(c as CustomControls.EnumTextBox), "X");
            }
            else if (c is CheckBox)
            {
                resField.res[resField.field] = new TypedValue(tvOld.Type, (c as CheckBox).Checked, "X");
            }
            else if (c is TextBox)
            {
                resField.res[resField.field] = new TypedValue(tvOld.Type, tb_Value(c as TextBox, tvOld.Type), "X");
            }
        }
        object tb_Value(TextBox tb, Type t)
        {
            if (t == typeof(Single) || t == typeof(Boolean) || !tb.Text.StartsWith("0x"))
                return Convert.ChangeType(tb.Text, t);
            else
                return Convert.ChangeType(UInt64.Parse(tb.Text.Substring(2), System.Globalization.NumberStyles.HexNumber), t);
        }
        object etb_Value(CustomControls.EnumTextBox etb) { return Enum.ToObject(etb.EnumType, etb.Value); }
        object tr_Value(CustomControls.TopicRatings tr) { return tr.Value; }
        void setFlags(AApiVersionedFields owner, string field, ulong value)
        {
            Type t = AApiVersionedFields.GetContentFieldTypes(0, owner.GetType())[field];
            TypedValue tv = new TypedValue(t, Enum.ToObject(t, value));
            owner[field] = tv;
        }
        #endregion

        #region Menu Bar
        private void menuBarWidget1_MBDropDownOpening(object sender, MenuBarWidget.MBDropDownOpeningEventArgs mn)
        {
            Diagnostics.Log(String.Format("menuBarWidget1_MBDropDownOpening mn: {0}", mn.mn));
            switch (mn.mn)
            {
                case MenuBarWidget.MD.MBF:
                    menuBarWidget1.Enable(MenuBarWidget.MB.MBF_reload, FileTable.Current != null);
                    menuBarWidget1.Enable(MenuBarWidget.MB.MBF_close, FileTable.Current != null);
                    break;
                case MenuBarWidget.MD.MBC: break;
                case MenuBarWidget.MD.MBT:
                    menuBarWidget1.Enable(MenuBarWidget.MB.MBT_replaceTGI, FileTable.Current != null);
                    menuBarWidget1.Enable(MenuBarWidget.MB.MBT_fixIntegrity, FileTable.Current != null);
                    break;
                case MenuBarWidget.MD.MBS: break;
                case MenuBarWidget.MD.MBH: break;
                default: break;
            }
        }

        #region File menu
        private void menuBarWidget1_MBFile_Click(object sender, MenuBarWidget.MBClickEventArgs mn)
        {
            Diagnostics.Log(String.Format("menuBarWidget1_MBFile_Click mn: {0}", mn.mn));
            try
            {
                this.Enabled = false;
                Application.DoEvents();
                switch (mn.mn)
                {
                    case MenuBarWidget.MB.MBF_open: fileOpen(); break;
                    case MenuBarWidget.MB.MBF_reload: fileReloadCurrent(); break;
                    case MenuBarWidget.MB.MBF_close: fileClose(); break;
                    case MenuBarWidget.MB.MBF_exit: fileExit(); break;
                }
            }
            finally { this.Enabled = true; }
        }

        private void fileOpen()
        {
            Diagnostics.Log("fileOpen");
            if (HaveUnsavedWork())
                if (!IsOkayToThrowAwayWork())
                    return;

            openPackageDialog.InitialDirectory = ObjectCloner.Properties.Settings.Default.LastSaveFolder == null || ObjectCloner.Properties.Settings.Default.LastSaveFolder.Length == 0
                ? "" : ObjectCloner.Properties.Settings.Default.LastSaveFolder;
            openPackageDialog.FileName = "*.package";
            DialogResult dr = openPackageDialog.ShowDialog();
            if (dr != DialogResult.OK) return;
            ObjectCloner.Properties.Settings.Default.LastSaveFolder = Path.GetDirectoryName(openPackageDialog.FileName);

            mode = Mode.FromUser;

            fileReOpenToFix(openPackageDialog.FileName);
        }

        private void fileReloadCurrent()
        {
            Diagnostics.Log("fileReloadCurrent");
            if (HaveUnsavedWork())
                if (!IsOkayToThrowAwayWork())
                    return;

            ReloadCurrentPackage();
        }

        private void fileClose()
        {
            Diagnostics.Log("fileClose");
            if (HaveUnsavedWork())
                if (!IsOkayToThrowAwayWork())
                    return;

            CloseCurrent();//needed
            DisplayNothing();
        }

        private void fileExit()
        {
            Diagnostics.Log("fileExit");
            if (HaveUnsavedWork())
                if (!IsOkayToThrowAwayWork())
                    return;

            Application.Exit();
        }
        #endregion

        #region Cloning menu
        private void menuBarWidget1_MBCloning_Click(object sender, MenuBarWidget.MBClickEventArgs mn)
        {
            Diagnostics.Log(String.Format("menuBarWidget1_MBFile_Click mn: {0}", mn.mn));
            try
            {
                this.Enabled = false;
                Application.DoEvents();

                if (HaveUnsavedWork())
                    if (!IsOkayToThrowAwayWork())
                        return;

                mode = Mode.FromGame;

                CloseCurrent();//needed
                InitialiseFileTable();

                LoadObjectChooser(FromCloningMenuEntry(mn.mn));
            }
            finally { this.Enabled = true; }
        }
        CatalogType FromCloningMenuEntry(MenuBarWidget.MB menuEntry)
        {
            if (!Enum.IsDefined(typeof(MenuBarWidget.MB), menuEntry)) return 0;
            List<MenuBarWidget.MB> ml = new List<MenuBarWidget.MB>((MenuBarWidget.MB[])Enum.GetValues(typeof(MenuBarWidget.MB)));
            return ((CatalogType[])Enum.GetValues(typeof(CatalogType)))[ml.IndexOf(menuEntry) - ml.IndexOf(MenuBarWidget.MB.MBC_casp)];
        }
        #endregion

        #region Tools menu
        private void menuBarWidget1_MBTools_Click(object sender, MenuBarWidget.MBClickEventArgs mn)
        {
            try
            {
                this.Enabled = false;
                Application.DoEvents();
                switch (mn.mn)
                {
                    case MenuBarWidget.MB.MBT_search: toolsSearch(); break;
                    case MenuBarWidget.MB.MBT_tgiSearch: toolsTGISearch(); break;
                    case MenuBarWidget.MB.MBT_replaceTGI: toolsReplaceTGI(); break;
                    case MenuBarWidget.MB.MBT_fixIntegrity: toolsFixIntegrity(); break;
                }
            }
            finally { this.Enabled = true; }
        }

        private void toolsSearch()
        {
            Diagnostics.Log("toolsSearch");
            if (HaveUnsavedWork())
                if (!IsOkayToThrowAwayWork())
                    return;

            ClearTabs();
            InitialiseFileTable();

            CTPTBrushIndexToPair = new Dictionary<ulong, SpecificResource>();
            searchPane = new Search(CheckInstallDirs, updateProgress, ListViewAdd);
            searchPane.SelectedIndexChanged += new EventHandler<SelectedIndexChangedEventArgs>(listView_SelectedIndexChanged);
            searchPane.ItemActivate += new EventHandler<ItemActivateEventArgs>(searchResult_ItemActivate);
            searchPane.CancelClicked += new EventHandler((x, e) => ReloadCurrentPackage());

            DisplaySearch();
        }

        private void toolsTGISearch()
        {
            Diagnostics.Log("toolsTGISearch");
            if (HaveUnsavedWork())
                if (!IsOkayToThrowAwayWork())
                    return;

            ClearTabs();
            InitialiseFileTable();

            tgiSearchPane = new TGISearch(CheckInstallDirs, updateProgress);
            tgiSearchPane.CancelClicked += new EventHandler((x, e) => ReloadCurrentPackage());
            tgiSearchPane.ItemActivate += new EventHandler<ItemActivateEventArgs>(searchResult_ItemActivate);

            DisplayTGISearch();
        }

        private void toolsReplaceTGI()
        {
            Diagnostics.Log("toolsReplaceTGI");
            if (FileTable.Current == null) return;
            if (HaveUnsavedWork())
                if (!IsOkayToThrowAwayWork())
                    return;

            replaceTGIPane = new ReplaceTGI();
            replaceTGIPane.ReplaceClicked += new EventHandler(replaceTGIPane_ReplaceClicked);
            replaceTGIPane.SaveClicked += new EventHandler(replaceTGIPane_SaveClicked);
            replaceTGIPane.CancelClicked += new EventHandler(replaceTGIPane_CancelClicked);

            ClearTabs();
            DisplayReplaceTGI();
        }

        private void toolsFixIntegrity()
        {
            Diagnostics.Log("toolsFixIntegrity");
            if (FileTable.Current == null) return;
            if (HaveUnsavedWork())
                if (!IsOkayToThrowAwayWork())
                    return;

            ClearTabs();
            DoWait("Renumbering all resources and references...");
            StartRenumbering();

            fixIntegrityResults = new FixIntegrityResults();
            fixIntegrityResults.SaveClicked += new EventHandler(fixIntegrityResults_SaveClicked);
            fixIntegrityResults.CancelClicked += new EventHandler(fixIntegrityResults_CancelClicked);

            DisplayFixIntegrityResults();
        }
        #endregion

        #region Settings menu
        private void menuBarWidget1_MBSettings_Click(object sender, MenuBarWidget.MBClickEventArgs mn)
        {
            try
            {
                this.Enabled = false;
                Application.DoEvents();
                switch (mn.mn)
                {
                    case MenuBarWidget.MB.MBS_sims3Folder: settingsGameFolders(); break;
                    case MenuBarWidget.MB.MBS_editor: settingsPackageEditor(); break;
                    case MenuBarWidget.MB.MBS_userName: settingsUserName(); break;
                    case MenuBarWidget.MB.MBS_langSearch: settingsLangSearch(); break;
                    case MenuBarWidget.MB.MBS_updates: settingsAutomaticUpdates(); break;
                    case MenuBarWidget.MB.MBS_advanced: settingsAdvancedCloning(); break;
                    case MenuBarWidget.MB.MBS_popups: settingsPopups(); break;
                    case MenuBarWidget.MB.MBS_logging: settingsLogging(); break;
                }
            }
            finally { this.Enabled = true; }
        }

        private void settingsGameFolders()
        {
            Diagnostics.Log("settingsGameFolders");
            if (HaveUnsavedWork())
                if (!IsOkayToThrowAwayWork())
                    return;

            GameFoldersForm gf;
            while (true)
            {
                gf = new GameFoldersForm();
                gf.CustomContent = ObjectCloner.Properties.Settings.Default.CustomContent;
                gf.CCEnabled = ObjectCloner.Properties.Settings.Default.CCEnabled;
                gf.InstallDirs = ObjectCloner.Properties.Settings.Default.InstallDirs;
                gf.EPsDisabled = ObjectCloner.Properties.Settings.Default.EPsDisabled;

                DialogResult dr = gf.ShowDialog();
                if (dr != DialogResult.OK && dr != DialogResult.Retry) return;
                if (dr != DialogResult.Retry) break;
            }

            bool changed = false;
            if (ObjectCloner.Properties.Settings.Default.CustomContent != gf.CustomContent)
            {
                FileTable.CustomContentPath = ObjectCloner.Properties.Settings.Default.CustomContent = gf.CustomContent;
                changed = true;
            }
            if (ObjectCloner.Properties.Settings.Default.CCEnabled != gf.CCEnabled)
            {
                ObjectCloner.Properties.Settings.Default.CCEnabled = gf.CCEnabled;
                changed = true;
            }
            if (ObjectCloner.Properties.Settings.Default.InstallDirs != gf.InstallDirs)
            {
                GameFolders.InstallDirs = ObjectCloner.Properties.Settings.Default.InstallDirs = gf.InstallDirs;
                changed = true;
            }
            if (ObjectCloner.Properties.Settings.Default.EPsDisabled != gf.EPsDisabled)
            {
                GameFolders.EPsDisabled = ObjectCloner.Properties.Settings.Default.EPsDisabled = gf.EPsDisabled;
                changed = true;
            }

            if (changed)
            {
                Abort(true);
                tabType = 0;
                DisplayNothing();
                ObjectCloner.Properties.Settings.Default.Save();
                FileTable.Reset();//needed!
                Reset();//needed!
            }
        }

        private void settingsPackageEditor()
        {
            Diagnostics.Log("settingsPackageEditor");

            PackageEditorDialog pe;
            while (true)
            {
                pe = new PackageEditorDialog();
                pe.PkgEditorPath = ObjectCloner.Properties.Settings.Default.pkgEditorPath;
                pe.PkgEditorEnabled = ObjectCloner.Properties.Settings.Default.pkgEditorEnabled;

                DialogResult dr = pe.ShowDialog();
                if (dr != DialogResult.OK && dr != DialogResult.Retry) return;
                if (dr != DialogResult.Retry) break;
            }
            ObjectCloner.Properties.Settings.Default.pkgEditorPath = pe.PkgEditorPath;
            ObjectCloner.Properties.Settings.Default.pkgEditorEnabled = pe.PkgEditorEnabled;
        }

        private void settingsUserName()
        {
            Diagnostics.Log("settingsUserName");

            StringInputDialog cn = new StringInputDialog();
            cn.Caption = "Creator name";
            cn.Prompt = "Your creator name will be used by default\nto create new object names";
            cn.Value = ObjectCloner.Properties.Settings.Default.CreatorName == null || ObjectCloner.Properties.Settings.Default.CreatorName.Length == 0
                ? Environment.UserName : ObjectCloner.Properties.Settings.Default.CreatorName;
            DialogResult dr = cn.ShowDialog();
            if (dr != DialogResult.OK) return;
            ObjectCloner.Properties.Settings.Default.CreatorName = cn.Value;
        }

        private void settingsLangSearch()
        {
            Diagnostics.Log("settingsLangSearch");

            Application.DoEvents();
            ObjectCloner.Properties.Settings.Default.LangSearch = !STBLHandler.LangSearch;
            menuBarWidget1.Checked(MenuBarWidget.MB.MBS_langSearch, STBLHandler.LangSearch);
        }

        private void settingsAutomaticUpdates()
        {
            Diagnostics.Log("settingsAutomaticUpdates");

            AutoUpdate.Checker.AutoUpdateChoice = !menuBarWidget1.IsChecked(MenuBarWidget.MB.MBS_updates);
        }

        private void settingsAdvancedCloning()
        {
            Diagnostics.Log("settingsAdvancedCloning");

            Application.DoEvents();
            ObjectCloner.Properties.Settings.Default.AdvanceCloning = !ObjectCloner.Properties.Settings.Default.AdvanceCloning;
            menuBarWidget1.Checked(MenuBarWidget.MB.MBS_advanced, ObjectCloner.Properties.Settings.Default.AdvanceCloning);
        }

        private void settingsPopups()
        {
            Application.DoEvents();
            ObjectCloner.Properties.Settings.Default.Diagnostics = Diagnostics.Popups = !Diagnostics.Popups;
            menuBarWidget1.Checked(MenuBarWidget.MB.MBS_popups, Diagnostics.Popups);
        }

        private void settingsLogging()
        {
            Application.DoEvents();
            if (Diagnostics.Logging) Diagnostics.Show("Closing log file");
            Diagnostics.Logging = !Diagnostics.Logging;
            if (Diagnostics.Logging) Diagnostics.Show("Openned log file");
            menuBarWidget1.Checked(MenuBarWidget.MB.MBS_logging, Diagnostics.Logging);
        }
        #endregion

        #region Help menu
        private void menuBarWidget1_MBHelp_Click(object sender, MenuBarWidget.MBClickEventArgs mn)
        {
            try
            {
                this.Enabled = false;
                Application.DoEvents();
                switch (mn.mn)
                {
                    case MenuBarWidget.MB.MBH_contents: helpContents(); break;
                    case MenuBarWidget.MB.MBH_about: helpAbout(); break;
                    case MenuBarWidget.MB.MBH_update: helpUpdate(); break;
                    case MenuBarWidget.MB.MBH_warranty: helpWarranty(); break;
                    case MenuBarWidget.MB.MBH_licence: helpLicence(); break;
                }
            }
            finally { this.Enabled = true; }
        }

        private void helpContents()
        {
            string locale = System.Globalization.CultureInfo.CurrentUICulture.Name;

            string baseFolder = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "HelpFiles");
            if (Directory.Exists(Path.Combine(baseFolder, locale)))
                baseFolder = Path.Combine(baseFolder, locale);
            else if (Directory.Exists(Path.Combine(baseFolder, locale.Substring(0, 2))))
                baseFolder = Path.Combine(baseFolder, locale.Substring(0, 2));

            if (File.Exists(Path.Combine(baseFolder, "Contents.htm")))
                Help.ShowHelp(this, "file:///" + Path.Combine(baseFolder, "Contents.htm"));
            else
                helpAbout();
        }

        private void helpAbout()
        {
            string copyright = "\n" +
                myName + "  Copyright (C) 2012  Peter L Jones\n" +
                "\n" +
                "This program comes with ABSOLUTELY NO WARRANTY; for details see Help->Warranty.\n" +
                "\n" +
                "This is free software, and you are welcome to redistribute it\n" +
                "under certain conditions; see Help->Licence for details.\n";
            CopyableMessageBox.Show(String.Format(
                "{0}\n" +
                "Front-end Distribution: {1}\n" +
                "Library Distribution: {2}"
                , copyright
                , AutoUpdate.Version.CurrentVersion
                , AutoUpdate.Version.LibraryVersion
                ), myName);
        }

        private void helpUpdate()
        {
            bool msgDisplayed = AutoUpdate.Checker.GetUpdate(false);
            if (!msgDisplayed)
                CopyableMessageBox.Show("Your " + System.Configuration.PortableSettingsProvider.ExecutableName + " is up to date", myName,
                    CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Information);
        }

        private void helpWarranty()
        {
            CopyableMessageBox.Show("\n" +
                "Disclaimer of Warranty.\n" +
                "\n" +
                "THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW. EXCEPT WHEN OTHERWISE STATED " +
                "IN WRITING THE COPYRIGHT HOLDERS AND/OR OTHER PARTIES PROVIDE THE PROGRAM \"AS IS\" WITHOUT WARRANTY OF ANY " +
                "KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND " +
                "FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. " +
                "SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.\n" +
                "\n" +
                "\n" +
                "Limitation of Liability.\n" +
                "\n" +
                "IN NO EVENT UNLESS REQUIRED BY APPLICABLE LAW OR AGREED TO IN WRITING WILL ANY COPYRIGHT HOLDER, OR ANY OTHER " +
                "PARTY WHO MODIFIES AND/OR CONVEYS THE PROGRAM AS PERMITTED ABOVE, BE LIABLE TO YOU FOR DAMAGES, INCLUDING ANY " +
                "GENERAL, SPECIAL, INCIDENTAL OR CONSEQUENTIAL DAMAGES ARISING OUT OF THE USE OR INABILITY TO USE THE PROGRAM " +
                "(INCLUDING BUT NOT LIMITED TO LOSS OF DATA OR DATA BEING RENDERED INACCURATE OR LOSSES SUSTAINED BY YOU OR " +
                "THIRD PARTIES OR A FAILURE OF THE PROGRAM TO OPERATE WITH ANY OTHER PROGRAMS), EVEN IF SUCH HOLDER OR OTHER " +
                "PARTY HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.\n" +
                "\n",
                this.Text);

        }

        private void helpLicence()
        {
            int dr = CopyableMessageBox.Show("\n" +
                "This program is distributed under the terms of the\n" +
                "GNU General Public Licence version 3.\n" +
                "\n" +
                "If you wish to see the full text of the licence,\n" +
                "please visit http://www.fsf.org/licensing/licenses/gpl.html.\n" +
                "\n" +
                "Do you wish to visit this site now?" +
                "\n",
                this.Text,
                CopyableMessageBoxButtons.YesNo, CopyableMessageBoxIcon.Question, 1);
            if (dr != 0) return;
            Help.ShowHelp(this, "http://www.fsf.org/licensing/licenses/gpl.html");
        }
        #endregion


        bool HaveUnsavedWork()
        {
            return cloneFixOptions != null ||
                lbSaveCancel.Visible;// && splitContainer1.Panel1.Controls.Contains(cloneFixOptions);
        }
        bool IsOkayToThrowAwayWork()
        {
            return true;
            /*int dr = CopyableMessageBox.Show("Continuing will lose any changes.", GetMyName(), CopyableMessageBoxButtons.OKCancel, CopyableMessageBoxIcon.Warning, 1);
            if (dr != 0) return false;

            cloneFixOptions = null;
            return true;/**/
        }


        void ReloadCurrentPackage()
        {
            if (FileTable.Current != null)
            {
                ObjectCloner.Properties.Settings.Default.LastSaveFolder = Path.GetDirectoryName(FileTable.Current.Path);
                mode = Mode.FromUser;
                fileReOpenToFix(FileTable.Current.Path);
            }
            else
                DisplayNothing();
        }

        void fileReOpenToFix(string filename, IResourceKey rk = null)
        {
            Diagnostics.Log(String.Format("fileReOpenToFix filename: {0}, rk: {1}", filename, rk));

            cloneFixOptions = null;//This should be a safe and sensible place to do this...

            CloseCurrent();//needed
            Abort(true);
            SetUseCC(false);
            SetAppendEA(false);

            try
            {
                FileTable.Current = new PathPackageTuple(filename, true);
                Reset();// pick up new NameMap / STBLs next time needed
            }
            catch (Exception ex)
            {
                CopyableMessageBox.IssueException(ex, "Could not open package:\n" + filename, "File Open");
                return;
            }

            this.Text = GetMyName();

            if (rk != null)
            {
                selectedItem = FileTable.Current.Find(rie => rk.Equals(rie));
                if (selectedItem == null)
                    CopyableMessageBox.Show(this, "Resource " + rk + " not found.", "Error", CopyableMessageBoxIcon.Error, new string[] { "OK" }, 0, 0);
                else
                    FixStart();
            }
            else
                LoadObjectChooser(0);
        }

        void LoadObjectChooser(CatalogType resourceType)
        {
            Diagnostics.Log("LoadObjectChooser resourceType: " + resourceType);

            CTPTBrushIndexToPair = new Dictionary<ulong, SpecificResource>();

            ClearTabs();

            btnStart.Enabled = false;
            objectChooser = ObjectChooser.CreateObjectChooser(DoWait, StopWait, updateProgress, ListViewAdd, resourceType,
                listView_SelectedIndexChanged, objectChooser_ItemActivate);

            DisplayObjectChooser();
        }

        public delegate bool CheckInstallDirsCB(Control control);
        public bool CheckInstallDirs(Control control)
        {
            Diagnostics.Log("CheckInstallDirs");
            try
            {
                DoWait("Refreshing packages cache...");
                if (!FileTable.IsOK)
                {
                    CopyableMessageBox.Show("Found no packages\nPlease check your Game Folder settings.", "No objects to clone",
                        CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Stop);
                    return false;
                }
                DoWait("Refreshing STBL cache...");
                bool loadedSTBLs = STBLHandler.IsOK;
                DoWait("Refreshing _KEY cache...");
                bool loadedNMaps = NameMap.IsOK;
                return true;
            }
            finally { if (control != null) StopWait(control); else StopWait(); Application.DoEvents(); }
        }
        #endregion

        private void btnReplCatlgThum_Click(object sender, EventArgs e)
        {
            Image rep = getReplacementForThumbs();
            if (rep != null)
            {
                replacementForThumbs = rep;
                pbCatlgThum.Image = ResizeImage(rep, pbCatlgThum);
            }
        }

        private void pbCatlgThum_DoubleClick(object sender, EventArgs e) { btnReplCatlgThum_Click(sender, e); }

        private void btnReplCASPThum_Click(object sender, EventArgs e)
        {
            Image rep = getReplacementForThumbs();
            if (rep != null)
            {
                replacementForThumbs = rep;
                pbCASPThum.Image = ResizeImage(rep, pbCASPThum);
            }
        }

        private void pbCASPThum_DoubleClick(object sender, EventArgs e) { btnReplCASPThum_Click(sender, e); }

        Image getReplacementForThumbs()
        {
            openThumbnailDialog.FilterIndex = 1;
            openThumbnailDialog.FileName = "*.PNG";
            DialogResult dr = openThumbnailDialog.ShowDialog();
            if (dr != DialogResult.OK) return null;
            try
            {
                return Image.FromFile(openThumbnailDialog.FileName, true);
            }
            catch (Exception ex)
            {
                CopyableMessageBox.IssueException(ex, "Could not read thumbnail:\n" + openThumbnailDialog.FileName, openThumbnailDialog.Title);
                return null;
            }
        }

        Image ResizeImage(Image src, PictureBox target)
        {
            return src.GetThumbnailImage(target.Width, target.Height, () => false, System.IntPtr.Zero);
        }

        List<string> replacements = new List<string>();

        private bool ReplaceRKsInTextReader(string fn, Predicate<IResourceKey> match, Converter<IResourceKey, IResourceKey> replacer, TextReader tr, TextWriter tw)
        {
            bool dirty = false;
            int ln = 0;
            string line = tr.ReadLine();
            while (line != null)
            {
                int i = line.IndexOf("key:");
                while (i >= 0 && i + 4 + 8 + 1 + 8 + 1 + 16 < line.Length)//key:TTTTTTTT-GGGGGGGG-IIIIIIIIIIIIIIII
                {
                    string oldKey = line.Substring(i + 4, 8 + 1 + 8 + 1 + 16);
                    bool hasColons = oldKey.Contains(":");
                    string RKkey = "0x" + (hasColons ? oldKey.Replace(":", "-0x") : oldKey.Replace("-", "-0x"));//translate to s3pi format
                    IResourceKey rk;
                    if (RK.TryParse(RKkey, out rk) && match(rk))
                    {
                        string newKey = new RK(replacer(rk)).ToString().Replace("0x", "");
                        if (hasColons) newKey = newKey.Replace("-", ":");
                        line = line.Substring(0, i) + "key:" + newKey + line.Substring(i + 4 + oldKey.Length);

                        string rpt = String.Format("{0} line {1} pos {2}: Replaced {3} with {4}", fn, ln, i, oldKey, newKey);
                        replacements.Add(rpt);
                        Diagnostics.Log(rpt);
                        dirty = true;
                    }
                    i = line.IndexOf("key:", i + 4 + oldKey.Length);
                }
                tw.WriteLine(line);
                ln++;
                line = tr.ReadLine();
            }
            tw.Flush();
            return dirty;
        }
        private bool ReplaceRKsInResourceStream(SpecificResource item, Predicate<IResourceKey> match, Converter<IResourceKey, IResourceKey> replacer)
        {
            bool dirty = false;
            MemoryStream ms = new MemoryStream();//Need this to persist its resources.
            StreamReader sr = new StreamReader(item.Resource.Stream, true);
            StreamWriter sw = new StreamWriter(ms, sr.CurrentEncoding);

            dirty = ReplaceRKsInTextReader("_XML " + item.RequestedRK, match, replacer, sr, sw);
            if (dirty)
            {
                item.Resource.Stream.SetLength(0);
                item.Resource.Stream.Write(ms.ToArray(), 0, (int)ms.Length);
            }

            return dirty;
        }
        private bool ReplaceRKsInField(SpecificResource item, string fn, Predicate<IResourceKey> match, Converter<IResourceKey, IResourceKey> replacer, AApiVersionedFields field)
        {
            bool dirty = false;

            Type t = field.GetType();
            if (typeof(IResourceKey).IsAssignableFrom(t))
            {
                IResourceKey rk = (IResourceKey)field;
                if (rk != RK.NULL && match(rk))
                {
                    string oldRK = new RK(rk) + "";
                    IResourceKey newRK = replacer(rk);
                    rk.ResourceType = newRK.ResourceType;
                    rk.ResourceGroup = newRK.ResourceGroup;
                    rk.Instance = newRK.Instance;

                    string rpt = String.Format("{0} {1}: Replaced {2} with {3}", item.RequestedRK + "", fn, oldRK, new RK(newRK) + "");
                    replacements.Add(rpt);
                    Diagnostics.Log(rpt);
                    dirty = true;
                }
            }
            else
            {
                if (typeof(IEnumerable).IsAssignableFrom(field.GetType()))
                    dirty = ReplaceRKsInIEnumerable(item, fn, match, replacer, (IEnumerable)field);

                dirty = ReplaceRKsInAApiVersionedFields(item, fn, match, replacer, field) || dirty;
            }

            return dirty;
        }
        private bool ReplaceRKsInIEnumerable(SpecificResource item, string fn, Predicate<IResourceKey> match, Converter<IResourceKey, IResourceKey> replacer, IEnumerable list)
        {
            bool dirty = false;

            int i = 0;
            string fmt = fn + "[{0:X}]";
            if (list != null)
                foreach (object o in list)
                    if (typeof(AApiVersionedFields).IsAssignableFrom(o.GetType()))
                        dirty = ReplaceRKsInField(item, String.Format(fmt, i++), match, replacer, (AApiVersionedFields)o) || dirty;
                    else if (typeof(IEnumerable).IsAssignableFrom(o.GetType()))
                        dirty = ReplaceRKsInIEnumerable(item, String.Format(fmt, i++), match, replacer, (IEnumerable)o) || dirty;

            return dirty;
        }
        private bool ReplaceRKsInAApiVersionedFields(SpecificResource item, string fn, Predicate<IResourceKey> match, Converter<IResourceKey, IResourceKey> replacer, AApiVersionedFields field)
        {
            bool dirty = false;

            List<string> fields = field.ContentFields;
            foreach (string f in fields)
            {
                if ((new List<string>(new string[] { "Stream", "AsBytes", "Value", })).Contains(f)) continue;

                Type t = AApiVersionedFields.GetContentFieldTypes(0, field.GetType())[f];
                if (!t.IsClass || t.Equals(typeof(string))) continue;
                if (t.IsArray && (!t.GetElementType().IsClass || t.GetElementType().Equals(typeof(string)))) continue;

                if (typeof(IEnumerable).IsAssignableFrom(t))
                    dirty = ReplaceRKsInIEnumerable(item, fn + (fn.Length == 0 ? "" : ".") + f, match, replacer, (IEnumerable)field[f].Value) || dirty;
                else if (typeof(AApiVersionedFields).IsAssignableFrom(t))
                    dirty = ReplaceRKsInField(item, fn + (fn.Length == 0 ? "" : ".") + f, match, replacer, (AApiVersionedFields)field[f].Value) || dirty;
            }

            return dirty;
        }

        #region Fetch resources
        // This is used for both Clone and Fix
        // Fix is basically DeepClone with a slight change in OBJD
        Dictionary<string, IResourceKey> rkLookup = null;
        void RunRKLookup()
        {
            DoWait("Please wait, tracking down resources...");

            if (rkLookup == null)
            {
                stepList = null;
                SetStepList(selectedItem, out stepList);
                if (stepList == null)
                {
                    //cloneFixOptions_CancelClicked(null, null);
                    return;
                }

                stepNum = 0;
                rkLookup = new Dictionary<string, IResourceKey>();
                while (stepNum < stepList.Count)
                {
                    step = stepList[stepNum];
                    updateProgress(true, StepText[step], true, stepList.Count - 1, true, stepNum);
                    Application.DoEvents();
                    stepNum++;
                    step();
                }
            }
        }

        private void Add(string key, IResourceKey referencedRK) { Diagnostics.Log("Add " + key + " '" + referencedRK + "'");/**/ rkLookup.Add(key, referencedRK); }

        private void SlurpRKsFromRK(string key, IResourceKey rk, bool includeDDSes = true)
        {
            SpecificResource item = new SpecificResource(FileTable.GameContent, rk);
            if (item.Resource != null)
            {
                if (item.Resource.GetType().Name.EndsWith("DefaultResource"))
                    Diagnostics.Log("item " + rk + " using DefaultResource wrapper");
                SlurpRKsFromField(key, (AResource)item.Resource, includeDDSes);
            }
            else Diagnostics.Show(String.Format("RK {0} not found", key));
        }
        private void SlurpRKsFromField(string key, AApiVersionedFields field, bool includeDDSes = true)
        {
            foreach (var tuple in field
                .FindAll(key, x => x is TGIBlock || x is GenericRCOLResource.ChunkEntry)//Don't look inside ChunkEntries
                .Where(x =>
                    x.Item2 is TGIBlock//but we only actually want TGIBlocks
                        //http://dino.drealm.info/develforums/s3pi/index.php?topic=1190.0 and maybe not DDSes
                    && (includeDDSes || (!ddsResources.Contains((x.Item2 as TGIBlock).ResourceType)))))
            {
                Add(tuple.Item1, tuple.Item2 as IResourceKey);
            }
        }
        static uint[] ddsResources = new uint[] {
            0x00B2D882, 0x8FFB80F6,
        };

        private void SlurpKinRKs(string key, Predicate<IResourceIndexEntry> Match)
        {
            List<SpecificResource> seen = new List<SpecificResource>();
            int i = 0;
            foreach (var ppt in FileTable.GameContent)
                foreach (var sr in ppt.FindAll(x => Match(x) && !seen.Exists(s => new RK(x).Equals(s.ResourceIndexEntry))))
                {
                    seen.Add(sr);
                    Add(key + "[" + i + "]", sr.ResourceIndexEntry);
                    i++;
                }
        }

        /*
<preset>
<complate name="CasRgbMask" reskey="blah">
<value key="..." value="key:00B2D882:00000000:B5683B98CA67ECFC" />
</complate>
</preset>
/**/
        static string[] CasRgbMaskWantedKeys = new string[] {
             "Overlay"          
            ,"Mask"             
            ,"Transparency Map" 
            ,"Multiplier"       
            ,"Clothing Specular"
            ,"Clothing Ambient" 
            ,"Stencil A"        
            ,"Stencil B"        
            ,"Stencil C"        
            ,"Stencil D"        
            ,"Stencil E"        
            ,"Stencil F"        
            ,"Logo Texture"     
            ,"Logo 2 Texture"   
            ,"Logo 3 Texture"   
            ,
        };
        void CASP_getXMLRefdDDSes(string key, IEnumerable<XElement> elements, bool log = false)
        {
            foreach (Tuple<string, string> tuple in elements
                .Where(v => v.Attribute("value").Value.StartsWith("key:"))
                .Select(v => Tuple.Create(v.Attribute("key").Value, v.Attribute("value").Value)))
            {
                if (!CasRgbMaskWantedKeys.Contains(tuple.Item1))
                {
                    if (log)
                        Diagnostics.Log(String.Format(key + ": Skipping key '{0}', value '{1}'", tuple.Item1, tuple.Item2));
                    continue;
                }

                IResourceKey rk;
                string oldKey = tuple.Item2.Substring(4).Replace('-', ':');
                string RKkey = "0x" + oldKey.Replace(":", "-0x");//translate to s3pi format
                if (RK.TryParse(RKkey, out rk))
                    Add(key + "." + tuple.Item1, rk);
            }
        }

        #region Steps
        SpecificResource objkItem;
        SpecificResource geomItem;
        List<SpecificResource> vpxyItems;
        List<SpecificResource> modlItems;

        delegate void Step();
        List<Step> stepList;
        Step step;
        int stepNum;
        int lastInChain;
        void SetStepList(SpecificResource item, out List<Step> stepList)
        {
            stepList = null;

            if (item == null)
                return;

            Step lastStepInChain = None;
            stepList = new List<Step>(new Step[] { Item_addSelf, });

            switch (item.RequestedRK.CType())
            {
                case CatalogType.CatalogProxyProduct:
                case CatalogType.CatalogFountainPool:
                case CatalogType.CatalogFoundation:
                case CatalogType.CatalogWallStyle:
                case CatalogType.CatalogRoofStyle:
                    ThumbnailsOnly_Steps(stepList, out lastStepInChain); break;
                case CatalogType.CatalogTerrainGeometryBrush:
                case CatalogType.CatalogTerrainWaterBrush:
                    brush_Steps(stepList, out lastStepInChain); break;
                case CatalogType.CatalogTerrainPaintBrush:
                    CTPT_Steps(stepList, out lastStepInChain); break;

                case CatalogType.CatalogFence:
                case CatalogType.CatalogStairs:
                case CatalogType.CatalogRailing:
                case CatalogType.CatalogRoofPattern:
                    CatlgHasVPXY_Steps(stepList, out lastStepInChain); break;
                case CatalogType.CatalogObject:
                    OBJD_Steps(stepList, out lastStepInChain); break;
                case CatalogType.CatalogWallFloorPattern:
                    CWAL_Steps(stepList, out lastStepInChain); break;

                case CatalogType.CatalogFireplace:
                    CFIR_Steps(stepList, out lastStepInChain); break;
                case CatalogType.ModularResource:
                    MDLR_Steps(stepList, out lastStepInChain); break;

                case CatalogType.CAS_Part:
                    CASP_Steps(stepList, out lastStepInChain); break;
            }
            lastInChain = stepList == null ? -1 : (stepList.IndexOf(lastStepInChain) + 1);
        }

        void ThumbnailsOnly_Steps(List<Step> stepList, out Step lastStepInChain)
        {
            Diagnostics.Log("ThumbnailsOnly_Steps");
            lastStepInChain = None;
            if (!JustSelf)
            {
                stepList.AddRange(new Step[] {
                });
                if (WantThumbs)
                    stepList.Add(SlurpThumbnails);
            }
        }

        void brush_Steps(List<Step> stepList, out Step lastStepInChain)
        {
            Diagnostics.Log("brush_Steps");
            ThumbnailsOnly_Steps(stepList, out lastStepInChain);
            if (!JustSelf)
            {
                if (IsDeepClone)
                {
                    stepList.Insert(0, brush_addBrushShape);
                }
                else
                {
                }
            }
        }

        void CTPT_Steps(List<Step> stepList, out Step lastStepInChain)
        {
            Diagnostics.Log("CTPT_Steps");
            brush_Steps(stepList, out lastStepInChain);
            if (!JustSelf)
            {
                stepList.InsertRange(0, new Step[] {
                    CTPT_addPair,
                    CTPT_addBrushTexture,
                });
            }
        }

        void CatlgHasVPXY_Steps(List<Step> stepList, out Step lastStepInChain)
        {
            Diagnostics.Log("CatlgHasVPXY_Steps");
            lastStepInChain = None;
            if (!JustSelf)
            {
                stepList.AddRange(new Step[] {
                    //NotDC: Catlg_IncludePresets,
                    //DC:    Catlg_SlurpRKs,
                    //DC:    Catlg_removeRefdCatlgs,
                    Catlg_getVPXYs,
                    Catlg_addVPXYs,

                    VPXYs_SlurpRKs,
                    VPXYs_getMODLs,
                    //DC:    VPXYKinXML_SlurpRKs,
                    VPXYs_getKinMTST,
                    VPXYs_getKinXML,
                    VPXYKinMTST_SlurpRKs,

                    MODLs_SlurpRKs,
                    MODLs_SlurpMLODs,
                    //NotDC: MODL_GetDiffuseSpecular,
                    //DC:    MODLs_SlurpTXTCs,
                });
                if (IsDeepClone)
                {
                    stepList.InsertRange(stepList.IndexOf(Catlg_getVPXYs), new Step[] {
                        Catlg_SlurpRKs,
                        Catlg_removeRefdCatlgs,
                    });
                    stepList.InsertRange(stepList.IndexOf(VPXYs_getKinMTST), new Step[] {
                        VPXYKinXML_SlurpRKs,
                    });
                    stepList.Add(MODLs_SlurpTXTCs);
                    lastStepInChain = MODLs_SlurpTXTCs;
                }
                else
                {
                    stepList.InsertRange(stepList.IndexOf(Catlg_getVPXYs), new Step[] {
                        Catlg_IncludePresets,
                    });
                    stepList.Add(MODL_GetDiffuseSpecular);
                    lastStepInChain = MODL_GetDiffuseSpecular;
                }
                if (WantThumbs)
                    stepList.Add(SlurpThumbnails);
            }
        }

        void OBJD_Steps(List<Step> stepList, out Step lastStepInChain)
        {
            Diagnostics.Log("OBJD_Steps");
            CatlgHasVPXY_Steps(stepList, out lastStepInChain);
            if (!JustSelf)
            {
                stepList.Remove(Catlg_getVPXYs);
                stepList.InsertRange(stepList.IndexOf(Catlg_addVPXYs), new Step[] {
                    // OBJD_setFallback if cloning from game
                    OBJD_getOBJK,
                    //NotDC: OBJD_addOBJKref,
                    //NotDC: OBJD_SlurpDDSes,
                    OBJK_SlurpRKs,
                    OBJK_getSPT2,
                    OBJK_getVPXYs,
                });
                if (!isFix && mode == Mode.FromGame && isRenumber)
                {
                    stepList.Insert(stepList.IndexOf(OBJD_getOBJK), OBJD_setFallback);
                }
                if (IsDeepClone)
                {
                }
                else
                {
                    stepList.InsertRange(stepList.IndexOf(OBJK_SlurpRKs), new Step[] {
                        OBJD_addOBJKref,
                        OBJD_SlurpDDSes,
                    });
                }
            }
        }

        void CWAL_Steps(List<Step> stepList, out Step lastStepInChain)
        {
            Diagnostics.Log("CWAL_Steps");
            CatlgHasVPXY_Steps(stepList, out lastStepInChain);
            if (!JustSelf)
            {
                if (IsDeepClone)
                {
                }
                else
                {
                }
                if (WantThumbs)
                {
                    stepList.Remove(SlurpThumbnails);
                    stepList.Add(CWAL_SlurpThumbnails);
                }
            }
        }

        void CFIR_Steps(List<Step> stepList, out Step lastStepInChain)
        {
            Diagnostics.Log("CFIR_Steps");
            stepList.AddRange(new Step[] { Item_findObjds, setupObjdStepList, Modular_Main, });
            if (WantThumbs)
                stepList.Add(SlurpThumbnails);//For the CFIR itself
            lastStepInChain = None;
        }

        void MDLR_Steps(List<Step> stepList, out Step lastStepInChain)
        {
            Diagnostics.Log("MDLR_Steps");
            stepList.AddRange(new Step[] { Item_findObjds, setupObjdStepList, Modular_Main, });
            lastStepInChain = None;
        }

        void CASP_Steps(List<Step> stepList, out Step lastStepInChain)
        {
            Diagnostics.Log("CASP_Steps");
            lastStepInChain = None;
            if (!JustSelf)
            {
                if (IsDeepClone)
                {
                    stepList.AddRange(new Step[] {
                        CASP_deepClone,
                    });
                }
                else
                {
                    stepList.AddRange(new Step[] {
                        CASP_clone,
                        CASP_IncludePresets,
                        GEOM_getNormalMap,
                    });
                }
                stepList.AddRange(new Step[] {
                    CASP_SlurpKinXML,
                });
                lastStepInChain = CASP_SlurpKinXML;
                if (WantThumbs)
                    stepList.Add(SlurpThumbnails);
            }
        }

        Dictionary<Step, string> StepText;
        void SetStepText()
        {
            StepText = new Dictionary<Step, string>();
            StepText.Add(Item_addSelf, "Add selected item");
            StepText.Add(Catlg_SlurpRKs, "Catalog object-referenced resources");
            StepText.Add(Catlg_removeRefdCatlgs, "Remove referenced CatalogResources");
            StepText.Add(Catlg_getVPXYs, "Find VPXYs in the Catalog Resource TGIBlockList");

            StepText.Add(OBJD_setFallback, "Set fallback TGI");
            StepText.Add(Catlg_IncludePresets, "Include Preset Images (CatalogResources)");
            StepText.Add(OBJD_getOBJK, "Find OBJK");
            StepText.Add(OBJD_addOBJKref, "Add OBJK");
            StepText.Add(OBJD_SlurpDDSes, "OBJD-referenced DDSes");
            StepText.Add(OBJK_SlurpRKs, "OBJK-referenced resources");
            StepText.Add(OBJK_getSPT2, "Find OBJK-referenced SPT2");
            StepText.Add(OBJK_getVPXYs, "Find OBJK-referenced VPXY(s)");

            StepText.Add(CTPT_addPair, "Add the other brush in pair");
            StepText.Add(CTPT_addBrushTexture, "Add Brush Texture");
            StepText.Add(brush_addBrushShape, "Add Brush Shape");

            StepText.Add(Catlg_addVPXYs, "Add VPXY resources");
            StepText.Add(VPXYs_SlurpRKs, "VPXY-referenced resources");
            StepText.Add(VPXYs_getKinMTST, "MTST (same instance as VPXY)");
            StepText.Add(VPXYs_getKinXML, "Preset XML (same instance as VPXY)");
            StepText.Add(VPXYKinMTST_SlurpRKs, "Find MTST referenced resources");
            StepText.Add(VPXYKinXML_SlurpRKs, "Find Preset XML referenced resources");
            StepText.Add(VPXYs_getMODLs, "Find VPXY-referenced MODLs");
            StepText.Add(MODLs_SlurpRKs, "MODL-referenced resources");
            StepText.Add(MODLs_SlurpMLODs, "MLOD-referenced resources");
            StepText.Add(MODL_GetDiffuseSpecular, "MLOD-referenced diffuse and specular DDS images");
            StepText.Add(MODLs_SlurpTXTCs, "TXTC-referenced resources");

            StepText.Add(Item_findObjds, "Find OBJDs from MDLR/CFIR");
            StepText.Add(setupObjdStepList, "Get the OBJD step list");
            StepText.Add(Modular_Main, "Drive the modular process");

            StepText.Add(SlurpThumbnails, "Add thumbnails");
            StepText.Add(CWAL_SlurpThumbnails, "Add thumbnails");

            StepText.Add(CASP_deepClone, "Deep clone of resources referenced by CASP");
            StepText.Add(CASP_clone, "Clone of several resources referenced by CASP");
            StepText.Add(CASP_IncludePresets, "Include Preset Images (CASParts)");
            StepText.Add(GEOM_getNormalMap, "Add GEOM-referenced NormalMap");
            StepText.Add(CASP_SlurpKinXML, "Preset XML (same instance as CASP)");
        }

        void None() { }

        void Item_addSelf()
        {
            Diagnostics.Log(String.Format("Item_addSelf: {0}", selectedItem.LongName));
            Add("clone", selectedItem.RequestedRK);
        }
        void Catlg_SlurpRKs() { Diagnostics.Log("Catlg_SlurpRKs"); SlurpRKsFromField("clone", (AResource)selectedItem.Resource); }

        // Any interdependency between catalog resource is handled like CFIR (for complex ones) or CTPT (for simple ones)
        void Catlg_removeRefdCatlgs()
        {
            Diagnostics.Log("Catlg_removeRefdCatlgs");
            IList<TGIBlock> ltgi = (IList<TGIBlock>)selectedItem.Resource["TGIBlocks"].Value;
            foreach (AResourceKey rk in ltgi)
            {
                if (Enum.IsDefined(typeof(CatalogType), rk.ResourceType) && rk.Instance != selectedItem.RequestedRK.Instance)
                {
                    int i = new List<IResourceKey>(rkLookup.Values).IndexOf(rk);
                    if (i >= 0)
                    {
                        string key = new List<string>(rkLookup.Keys)[i];
                        Diagnostics.Log("Catlg_removeRefdCatlgs: Removed " + key + " '" + rkLookup[key] + "'");
                        rkLookup.Remove(key);
                    }
                }
            }
        }

        void Catlg_getVPXYs()
        {
            Diagnostics.Log("Catlg_getVPXYs");
            vpxyItems = new List<SpecificResource>();

            string s = "";
            foreach (IResourceKey rk in (TGIBlockList)selectedItem.Resource["TGIBlocks"].Value)
            {
                if (rk.ResourceType != 0x736884F1) continue;
                SpecificResource vpxy = new SpecificResource(FileTable.GameContent, rk);
                if (vpxy.Resource != null)
                    vpxyItems.Add(vpxy);
                else
                    s += String.Format("Catalog Resource {0} -> RK {1}: not found\n", (IResourceKey)selectedItem.ResourceIndexEntry, rk);
            }
            Diagnostics.Show(s, "Missing VPXYs");
            if (vpxyItems.Count == 0)
            {
                Diagnostics.Show(String.Format("Catalog Resource {0} has no VPXY items", (IResourceKey)selectedItem.ResourceIndexEntry), "No VPXY items");
                stepNum = lastInChain;
            }
        }

        #region OBJD Steps
        void OBJD_setFallback()
        {
            Diagnostics.Log("OBJD_setFallback");
            if ((selectedItem.RequestedRK.ResourceGroup >> 27) > 0) return;// Only base game objects

            int fallbackIndex = (int)(uint)selectedItem.Resource["FallbackIndex"].Value;
            TGIBlockList tgiBlocks = selectedItem.Resource["TGIBlocks"].Value as TGIBlockList;
            if (tgiBlocks[fallbackIndex].Equals(RK.NULL))
            {
                selectedItem.Resource["FallbackIndex"] = new TypedValue(typeof(uint), (uint)tgiBlocks.Count, "X");
                tgiBlocks.Add("TGI", selectedItem.RequestedRK.ResourceType, selectedItem.RequestedRK.ResourceGroup, selectedItem.RequestedRK.Instance);
                selectedItem.Commit();
                Diagnostics.Log("OBJD_setFallback: FallbackIndex: 0x" + (tgiBlocks.Count - 1).ToString("X2") + ", Resourcekey: " + tgiBlocks[tgiBlocks.Count - 1]);
            }
        }
        void OBJD_getOBJK()
        {
            Diagnostics.Log("OBJD_getOBJK");
            uint index = (uint)selectedItem.Resource["OBJKIndex"].Value;
            IList<TGIBlock> ltgi = (IList<TGIBlock>)selectedItem.Resource["TGIBlocks"].Value;
            TGIBlock objkTGI = ltgi[(int)index];
            objkItem = new SpecificResource(FileTable.GameContent, objkTGI);
            if (objkItem == null || objkItem.ResourceIndexEntry == null)
            {
                Diagnostics.Show(String.Format("OBJK {0} -> OBJK {1}: not found\n", (IResourceKey)selectedItem.ResourceIndexEntry, objkTGI), "Missing OBJK");
                stepNum = lastInChain;
            }
            else
            {
                Diagnostics.Log(String.Format("OBJD_getOBJK: Found {0}", objkItem.LongName));
            }
        }
        void OBJD_addOBJKref() { Diagnostics.Log("OBJD_addOBJKref"); Add("objk", objkItem.RequestedRK); }
        void OBJD_SlurpDDSes()
        {
            Diagnostics.Log("OBJD_SlurpDDSes");
            IList<TGIBlock> ltgi = (IList<TGIBlock>)selectedItem.Resource["TGIBlocks"].Value;
            int i = 0;
            foreach (AApiVersionedFields mtdoor in (IList)selectedItem.Resource["MTDoors"].Value)
            {
                Add("clone.wallmask[" + i + "]", ltgi[(int)(uint)mtdoor["WallMaskIndex"].Value]);
                i++;
            }
            Add("clone.sinkmask", ltgi[(int)(uint)selectedItem.Resource["SurfaceCutoutDDSIndex"].Value]);
            if (selectedItem.Resource.ContentFields.Contains("FloorCutoutDDSIndex"))
                Add("clone.tubmask", ltgi[(int)(uint)selectedItem.Resource["FloorCutoutDDSIndex"].Value]);
        }
        void OBJK_SlurpRKs() { Diagnostics.Log("OBJK_SlurpRKs"); SlurpRKsFromField("objk", (AResource)objkItem.Resource); }
        void OBJK_getSPT2()
        {
            Diagnostics.Log("OBJK_getSPT2");
            ObjKeyResource.ObjKeyResource objk = objkItem.Resource as ObjKeyResource.ObjKeyResource;
            if (objk.Components.FindAll(x => x.Element == ObjKeyResource.ObjKeyResource.Component.Tree).Count == 0) return;

            List<SpecificResource> spt2Items = new List<SpecificResource>();

            string s = "";
            foreach (var rk in objk.TGIBlocks.FindAll(x => x.ResourceType == 0x00B552EA))//_SPT
            {
                SpecificResource spt2 = new SpecificResource(FileTable.GameContent, new RK(rk) { ResourceType = 0x021D7E8C });//SPT2
                if (spt2.ResourceIndexEntry != null && spt2.Resource != null)
                {
                    spt2Items.Add(spt2);//SPT2
                    Diagnostics.Log(String.Format("OBJK_getSPT2: Found {0}", spt2.LongName));
                }
                else
                    s += String.Format("OBJK {0} -> _SPT -> SPT2 {1}: not found\n", (IResourceKey)objkItem.ResourceIndexEntry, spt2.RequestedRK);
            }
            Diagnostics.Show(s, "Missing SPT2s");
            if (spt2Items.Count == 0)
            {
                Diagnostics.Show(String.Format("OBJK {0} with a Tree Component has no SPT2 items", (IResourceKey)selectedItem.ResourceIndexEntry), "No SPT2 items");
            }

            //add SPT2s
            for (int i = 0; i < spt2Items.Count; i++) Add("spt2[" + i + "]", spt2Items[i].RequestedRK);
        }
        void OBJK_getVPXYs()
        {
            Diagnostics.Log("OBJK_getVPXYs");
            int index = -1;
            if (((ObjKeyResource.ObjKeyResource)objkItem.Resource).ComponentData.ContainsKey("modelKey"))
                index = ((ObjKeyResource.ObjKeyResource.CDTResourceKey)((ObjKeyResource.ObjKeyResource)objkItem.Resource).ComponentData["modelKey"]).Data;

            if (index == -1)
            {
                Diagnostics.Show(String.Format("OBJK {0} has no modelKey", (IResourceKey)objkItem.ResourceIndexEntry), "Missing modelKey");
                stepNum = lastInChain;//Skip past the chain
                return;
            }

            vpxyItems = new List<SpecificResource>();

            string s = "";
            TGIBlockList tgibl = ((ObjKeyResource.ObjKeyResource)objkItem.Resource).TGIBlocks;

            Diagnostics.Log(String.Format("OBJK_getVPXYs: modelKey {0} -> {1}", index, tgibl.Count > index ? tgibl[index] : "(error)"));

            foreach (IResourceKey rk in tgibl.FindAll(x => x.ResourceType == 0x736884F1))//VPXY
            {
                SpecificResource vpxy = new SpecificResource(FileTable.GameContent, rk);
                if (vpxy.ResourceIndexEntry != null && vpxy.Resource != null)
                {
                    vpxyItems.Add(vpxy);
                    Diagnostics.Log(String.Format("OBJK_getVPXYs: Looked up {0}: found {1}", rk, vpxy.LongName));
                }
                else
                    s += String.Format("OBJK {0} -> RK {1}: not found\n", (IResourceKey)objkItem.ResourceIndexEntry, rk);
            }
            Diagnostics.Show(s, "Missing VPXYs");
            if (vpxyItems.Count == 0)
            {
                Diagnostics.Show(String.Format("OBJK {0} has no VPXY items", (IResourceKey)selectedItem.ResourceIndexEntry), "No VPXY items");
                stepNum = lastInChain;
            }
        }
        #endregion

        void CTPT_addPair()
        {
            Diagnostics.Log("CTPT_addPair");
            //int brushIndex = ("" + selectedItem.Resource["BrushTexture"]).GetHashCode();
            UInt64 brushIndex = (UInt64)selectedItem.Resource["CommonBlock.NameGUID"].Value;
            if (CTPTBrushIndexToPair.ContainsKey(brushIndex))
                Add("ctpt_pair", CTPTBrushIndexToPair[brushIndex].RequestedRK);
            else
                Diagnostics.Show(String.Format("CTPT {0} BrushIndex {1} not found", selectedItem.RequestedRK, brushIndex), "No ctpt_pair item");
        }
        void CTPT_addBrushTexture() { Diagnostics.Log("CTPT_addBrushTexture"); Add("ctpt_BrushTexture", (TGIBlock)selectedItem.Resource["BrushTexture"].Value); }
        void brush_addBrushShape() { Diagnostics.Log("brush_addBrushShape"); Add("brush_ProfileTexture", (TGIBlock)selectedItem.Resource["ProfileTexture"].Value); }

        static string[] complateOverrideVariables = new string[] {
            "Multiplier", "Mask", "Specular", "Overlay",
            "Stencil A", "Stencil B", "Stencil C", "Stencil D",
        };
        void Catlg_IncludePresets()
        {
            Diagnostics.Log("Catlg_IncludePresets");
            if (!selectedItem.Resource.ContentFields.Contains("Materials"))
            {
                Diagnostics.Log("Catlg_IncludePresets - field not found");
                return;
            }
            int i = 0;
            System.Collections.IEnumerable materials = (System.Collections.IEnumerable)selectedItem.Resource["Materials"].Value;
            foreach (CatalogResource.CatalogResource.Material material in materials)
            {
                i++;
                foreach (var complateOverride in material.MaterialBlock.ComplateOverrides)
                {
                    if (complateOverrideVariables.Contains(complateOverride.VariableName))
                    {
                        IResourceKey rk = material.TGIBlocks[(Byte)complateOverride["TGIIndex"].Value];
                        Add("preset" + i + "_" + complateOverride.VariableName, rk);
                    }
                }
            }
        }
        void Catlg_addVPXYs() { Diagnostics.Log("Catlg_addVPXYs"); for (int i = 0; i < vpxyItems.Count; i++) Add("vpxy[" + i + "]", vpxyItems[i].RequestedRK); }

        void VPXYs_SlurpRKs()
        {
            Diagnostics.Log("VPXYs_SlurpRKs");
            for (int i = 0; i < vpxyItems.Count; i++)
            {
                VPXY vpxyChunk = ((GenericRCOLResource)vpxyItems[i].Resource).ChunkEntries[0].RCOLBlock as VPXY;
                SlurpRKsFromField("vpxy[" + i + "]", vpxyChunk);
            }
        }
        void VPXYs_getKinMTST() { Diagnostics.Log("VPXYs_getKinMTST"); VPXYs_getKinRKs("VPXYs_getKinMTST", 0x02019972, "mtst"); }
        void VPXYs_getKinXML() { Diagnostics.Log("VPXYs_getKinXML"); VPXYs_getKinRKs("VPXYs_getKinXML", 0x0333406C, "PresetXML"); }
        void VPXYKinMTST_SlurpRKs() { Diagnostics.Log("VPXYKinMTST_SlurpRKs"); VPXYKin_SlurpRKs(0x02019972, "mtst"); }
        void VPXYKinXML_SlurpRKs() { Diagnostics.Log("VPXYKinXML_SlurpRKs"); VPXYKin_SlurpRKs(0x0333406C, "PresetXML"); }
        bool IsVPXYKin(IResourceKey rk, uint type) { return rk.ResourceType == type && vpxyItems.Exists(item => item.ResourceIndexEntry.Instance == rk.Instance); }
        void VPXYs_getKinRKs(string log, uint type, string suffix)
        {
            for (int i = 0; i < vpxyItems.Count; i++)
            {
                Diagnostics.Log(String.Format(log + ": 0x{0:X8}-*-0x{1:X16}", type, vpxyItems[i].ResourceIndexEntry.Instance));
                SlurpKinRKs("vpxy[" + i + "]." + suffix, rie => IsVPXYKin(rie, type));
            }
        }
        void VPXYKin_SlurpRKs(uint type, string prefix)
        {
            int i = 0;
            foreach (var ppt in FileTable.GameContent)
                foreach (var item in ppt.FindAll(rie => IsVPXYKin(rie, type)).Where(sr => sr.Resource != null))
                    SlurpRKsFromField(prefix + "[" + i++ + "]", item.Resource as AApiVersionedFields);
        }
        void VPXYs_getMODLs()
        {
            Diagnostics.Log("VPXYs_getMODLs");
            modlItems = new List<SpecificResource>();
            string s = "";
            for (int i = 0; i < vpxyItems.Count; i++)
            {
                GenericRCOLResource rcol = (vpxyItems[i].Resource as GenericRCOLResource);
                for (int j = 0; j < rcol.ChunkEntries.Count; j++)
                {
                    bool found = false;
                    VPXY vpxychunk = rcol.ChunkEntries[j].RCOLBlock as VPXY;
                    for (int k = 0; k < vpxychunk.TGIBlocks.Count; k++)
                    {
                        TGIBlock tgib = vpxychunk.TGIBlocks[k];
                        if (tgib.ResourceType != 0x01661233) continue;
                        SpecificResource modl = new SpecificResource(FileTable.GameContent, tgib);
                        if (modl.Resource != null)
                        {
                            found = true;
                            modlItems.Add(modl);
                            Diagnostics.Log(String.Format("Found {0}", modl.LongName));
                        }
                    }
                    if (!found)
                        s += String.Format("VPXY {0} (chunk {1}) has no MODL items\n", (IResourceKey)vpxyItems[i].ResourceIndexEntry, j);
                }
            }
            Diagnostics.Show(s, "No MODL items");
        }

        void MODLs_SlurpRKs() { Diagnostics.Log("MODLs_SlurpRKs"); for (int i = 0; i < modlItems.Count; i++) SlurpRKsFromField("modl[" + i + "]", (AResource)modlItems[i].Resource, IsDeepClone); }
        void MODLs_SlurpMLODs()
        {
            Diagnostics.Log("MODLs_SlurpMLODs");
            string s = "";
            for (int i = 0; i < modlItems.Count; i++)
            {
                bool found = false;
                GenericRCOLResource rcol = (modlItems[i].Resource as GenericRCOLResource);
                int k = 0;
                for (int j = 0; j < rcol.Resources.Count; j++)
                {
                    TGIBlock tgib = rcol.Resources[j];
                    if (tgib.ResourceType != 0x01D10F34) continue;
                    SlurpRKsFromRK("modl[" + i + "].mlod[" + k + "]", tgib, IsDeepClone);
                    k++;
                    found = true;
                }
                if (!found)
                    s += String.Format("MODL {0} has no MLOD items\n", (IResourceKey)modlItems[i].ResourceIndexEntry);
            }
            Diagnostics.Show(s, "No MLOD items");
        }
        void MODL_GetDiffuseSpecular()
        {
            Diagnostics.Log("MODL_GetDiffuseSpecular");
            string s = "";
            for (int i = 0; i < modlItems.Count; i++)
            {
                GenericRCOLResource rcol = (modlItems[i].Resource as GenericRCOLResource);

                MATD_GetDiffuseSpecular("modl[" + i + "]", rcol);

                int k = 0;
                foreach (SpecificResource mlodItem in rcol.Resources
                    .Where(x => x.ResourceType == 0x01D10F34)
                    .Select(mlodRK => new SpecificResource(FileTable.GameContent, mlodRK)))
                {
                    string key = "modl[" + i + "].mlod[" + k + "]";
                    if (mlodItem.Resource != null) MATD_GetDiffuseSpecular(key, mlodItem.Resource as GenericRCOLResource);
                    else s += String.Format(key + ": RK {0} not found", mlodItem.RequestedRK);
                    k++;
                }
            }
            if (s.Length > 0)
                Diagnostics.Show(s);
        }
        void MATD_GetDiffuseSpecular(string key, GenericRCOLResource rcol)
        {
            Diagnostics.Log("MATD_GetDiffuseSpecular: " + key);
            int i = 0;
            foreach (MATD matd in rcol.ChunkEntries.Where(x => x.RCOLBlock is MATD).Select(x => x.RCOLBlock))
            {
                string myKey = key + ".matd[" + i + "]";

                MATD.ShaderDataList sdataList;
                if (matd.ContentFields.Contains("Mtnf")) sdataList = matd.Mtnf.SData;
                else sdataList = matd.Mtrl.SData;

                int j = 0;
                foreach (MATD.ElementTextureRef sdata in sdataList
                    .Where(x => x is MATD.ElementTextureRef && wantedImages.Contains(x.Field)))
                {
                    if (ddsResources.Contains(rcol.Resources[sdata.Data.TGIBlockIndex].ResourceType))
                    {
                        Add(myKey + ".ElementTextureRef[" + j + "]", rcol.Resources[sdata.Data.TGIBlockIndex]);
                        j++;
                    }
                }

                foreach (MATD.ElementTextureKey sdata in sdataList
                    .Where(x => x is MATD.ElementTextureKey && wantedImages.Contains(x.Field)))
                {
                    if (ddsResources.Contains(sdata.Data.ResourceType))
                    {
                        Add(myKey + ".ElementTextureKey[" + j + "]", sdata.Data);
                        j++;
                    }
                }

                i++;
            }
        }
        static MATD.FieldType[] wantedImages = new MATD.FieldType[] {
            MATD.FieldType.DiffuseMap,
            MATD.FieldType.SpecularMap,
        };

        //This slurps all the RKs out of the TXTCs so the references get pulled in
        void MODLs_SlurpTXTCs()
        {
            Diagnostics.Log("MODLs_SlurpTXTCs");
            int k = 0;
            string s = "";
            for (int i = 0; i < modlItems.Count; i++)
            {
                bool found = false;
                GenericRCOLResource rcol = (modlItems[i].Resource as GenericRCOLResource);
                for (int j = 0; j < rcol.Resources.Count; j++)
                {
                    TGIBlock tgib = rcol.Resources[j];
                    if (tgib.ResourceType != 0x033A1435) continue;
                    SlurpRKsFromRK("modl[" + i + "].txtc[" + k + "]", tgib);
                    k++;
                    found = true;
                }
                if (!found)
                    s += String.Format("MODL {0} has no TXTC items\n", (IResourceKey)modlItems[i].ResourceIndexEntry);
            }
            Diagnostics.Show(s, "No TXTC items");
        }

        void deepClone(Tuple<string, object> _tuple, Predicate<IResourceKey> _match)
        {
            IResourceKey _rk = _tuple.Item2 as IResourceKey;
            if (!_match(_rk)) return;
            Add(_tuple.Item1, _rk);
            SpecificResource sr = new SpecificResource(FileTable.GameContent, _rk);
            if (sr.ResourceIndexEntry == null) return;
            if (sr.Resource == null)
                throw sr.Exception;

            string key = _tuple.Item1 + ": " + sr.LongName.Substring(0, 4);
            IEnumerable<Tuple<string, object>> iet;
            if (_rk.ResourceType == 0x0333406C)//_XML
            {
                iet = (new StreamReader(sr.Resource.Stream, true)).SlurpRKs(key);
            }
            else
            {
                iet = (sr.Resource as AResource).FindAll(key, x => x is IResourceKey, (tr, k) => tr.SlurpRKs(k));
            }
            foreach (var tuple in iet)
                deepClone(tuple, _match);
        }

        void CASP_deepClone()
        {
            Diagnostics.Log("CASP_deepClone");
            CASPartResource.CASPartResource casp = selectedItem.Resource as CASPartResource.CASPartResource;
            if (casp == null) return;

            List<IResourceKey> seen = new List<IResourceKey>();
            foreach (var tuple in casp.FindAll("casp", x => x is IResourceKey, (tr, k) => tr.SlurpRKs(k)))
                deepClone(tuple, x => { if (seen.Contains(x) || x.ResourceType == 0x034AEECB) return false; seen.Add(x); return true; });
        }

        void CASP_clone()
        {
            Diagnostics.Log("CASP_clone");
            CASPartResource.CASPartResource casp = selectedItem.Resource as CASPartResource.CASPartResource;
            if (casp == null) return;

            geomItem = null;
            List<IResourceKey> seen = new List<IResourceKey>();
            foreach (var tuple in casp.FindAll("casp", x => x is IResourceKey, (tr, k) => tr.SlurpRKs(k)))
                deepClone(tuple,
                    x => {
                        if (x.ResourceType == 0x00B2D882 //_IMG
                            || x.ResourceType == 0x0333406C //_XML
                            || x.ResourceType == 0x034AEECB //CASP
                            || seen.Contains(x))
                            return false;
                        seen.Add(x);
                        if (x.ResourceType == 0x015A1849 && geomItem == null)
                            geomItem = new SpecificResource(FileTable.GameContent, x);

                        return true;
                    });
        }

        void CASP_IncludePresets()
        {
            Diagnostics.Log("CASP_IncludePresets");
            CASPartResource.CASPartResource casp = selectedItem.Resource as CASPartResource.CASPartResource;
            if (casp == null)
            {
                Diagnostics.Show("The CASP resource has disappeared.");
                return;
            }

            int i = 0;
            foreach (var preset in casp.Presets)
            {
                XDocument xdoc = XDocument.Load(System.Xml.XmlReader.Create(preset.XmlFile));
                var elements = xdoc.Descendants("value").Where(n => n.Ancestors("pattern").Count() == 0);
                CASP_getXMLRefdDDSes("casp.Presets[" + i + "]", elements, true);

                i++;
            }
        }

        void GEOM_getNormalMap()
        {
            Diagnostics.Log("GEOM_getNormalMap");
            if (geomItem == null) { Diagnostics.Show("GEOM reference not found"); return; }
            if (geomItem.Resource == null) { Diagnostics.Show("GEOM resource not found"); return; }
            if (geomItem.Resource as meshExpImp.ModelBlocks.GeometryResource == null) { Diagnostics.Show("GEOM resource is not a GeometryResource"); return; }
            meshExpImp.ModelBlocks.GeometryResource geomResource = geomItem.Resource as meshExpImp.ModelBlocks.GeometryResource;
            if (geomResource.ChunkEntries.Count != 1) { Diagnostics.Show("GeometryResource should have 1 Chunk, found " + geomResource.ChunkEntries.Count); return; }
            meshExpImp.ModelBlocks.GEOM geom = geomResource.ChunkEntries[0].RCOLBlock as meshExpImp.ModelBlocks.GEOM;
            if (geom == null) { Diagnostics.Show("GEOM RCOL block not found"); return; }
            if (geom.Shader == 0) return;
            MATD.MTNF mtnf = geom.Mtnf;
            if (mtnf == null) { Diagnostics.Show("MTNF expected but was null"); return; }
            MATD.ShaderData sd = mtnf.SData.Find(e => e.Field == MATD.FieldType.NormalMap);
            if (sd == null) { Diagnostics.Show("NormalMap not found"); return; }
            MATD.ElementTextureRef tr = sd as MATD.ElementTextureRef;
            if (tr == null) { Diagnostics.Show("NormalMap not an ElementTextureRef"); return; }
            //This will break when Data stops being a ChunkRef and turns into a plain int...
            int index = -2;
            using (MemoryStream ms = new MemoryStream())
            {
                tr.Data.UnParse(ms);
                ms.Position = 0;
                index = new BinaryReader(ms).ReadInt32();
            }
            if (index < 0) { Diagnostics.Show(String.Format("NormalMap index read {0}, expected >= 0", index)); return; }
            if (index >= geom.TGIBlocks.Count) { Diagnostics.Show(String.Format("NormalMap index read {0}, expected < {1}", index, geom.TGIBlocks.Count)); return; }
            if (rkLookup.ContainsValue(geom.TGIBlocks[index])) { Diagnostics.Show("Already seen NormalMap " + geom.TGIBlocks[index]); }
            Add("casp.GEOM.NormalMap", geom.TGIBlocks[index]);
        }

        void CASP_SlurpKinXML()
        {
            Diagnostics.Log("CASP_getKinXML");
            List<IResourceKey> seen = new List<IResourceKey>();

            int i = 0;
            foreach (var sr in FileTable.GameContent
                .SelectMany(ppt => ppt.FindAll(rie => rie.ResourceType == 0x0333406C && rie.Instance == selectedItem.RequestedRK.Instance))
                .Where(sr => { if (seen.Contains(sr.RequestedRK)) return false; seen.Add(sr.RequestedRK); return true; }))
            {
                Add("casp.KinXML[" + i + "]", sr.ResourceIndexEntry);

                XElement xdoc = XElement.Load(sr.Resource.Stream);
                var elements = xdoc.Descendants("value");
                CASP_getXMLRefdDDSes("casp.KinXML[" + i + "]", elements);

                i++;
            }
        }

        List<SpecificResource> objdList;
        void Item_findObjds()
        {
            Diagnostics.Log("Item_findObjds");
            objdList = new List<SpecificResource>();
            string s = "";
            foreach (IResourceKey rk in (TGIBlockList)selectedItem.Resource["TGIBlocks"].Value)
            {
                if (rk.ResourceType != 0x319E4F1D) continue;
                SpecificResource objd = new SpecificResource(FileTable.GameContent, rk);
                if (objd.Resource != null)
                {
                    objdList.Add(objd);
                    Diagnostics.Log(String.Format("Item_findObjds: Found {0}", objd.LongName));
                }
                else
                    s += String.Format("OBJD {0}\n", rk);
            }
            Diagnostics.Show(s, String.Format("Item {0} has missing OBJDs:", selectedItem));
        }

        List<Step> objdSteps;
        void setupObjdStepList() { Diagnostics.Log("setupObjdStepList"); if (objdList.Count > 0) SetStepList(objdList[0], out objdSteps); }

        void Modular_Main()
        {
            Diagnostics.Log("Modular_Main");
            SpecificResource realSelectedItem = selectedItem;
            int realStepNum = stepNum;
            Step mdlrStep;
            Dictionary<string, IResourceKey> MDLRrkLookup = new Dictionary<string, IResourceKey>();
            foreach (var kvp in rkLookup) MDLRrkLookup.Add(kvp.Key, kvp.Value);

            for (int i = 0; i < objdList.Count; i++)
            {
                rkLookup = new Dictionary<string, IResourceKey>();
                selectedItem = objdList[i];
                stepNum = 0;
                while (stepNum < objdSteps.Count)
                {
                    mdlrStep = objdSteps[stepNum];
                    updateProgress(true, StepText[mdlrStep], true, objdSteps.Count - 1, true, stepNum);
                    Application.DoEvents();
                    stepNum++;
                    mdlrStep();
                }
                foreach (var kvp in rkLookup) MDLRrkLookup.Add("objd[" + i + "]." + kvp.Key, kvp.Value);
            }

            selectedItem = realSelectedItem;
            stepNum = realStepNum;
            rkLookup = MDLRrkLookup;
        }

        //Thumbnails for everything but walls
        //PNGs come from :Objects; Icons come from :Images; Thumbs come from :Thumbnails.
        void SlurpThumbnails()
        {
            Diagnostics.Log("SlurpThumbnails");
            foreach (THUM.THUMSize size in new THUM.THUMSize[] { THUM.THUMSize.small, THUM.THUMSize.medium, THUM.THUMSize.large, })
            {
                IResourceKey rk = THUM.getImageRK(size, selectedItem);
                if (THUM.PNGTypes[(int)size] == rk.ResourceType)
                    Add(size + "PNG", rk);
                else if (selectedItem.RequestedRK.CType() == CatalogType.CatalogRoofPattern)
                    Add(size + "Icon", rk);
                else
                    Add(size + "Thumb", rk);
            }
        }
        //0x515CA4CD is very different - but they do come from :Thumbnails, at least.
        void CWAL_SlurpThumbnails()
        {
            Diagnostics.Log("CWAL_SlurpThumbnails");
            Dictionary<THUM.THUMSize, uint> CWALThumbTypes = new Dictionary<THUM.THUMSize, uint>();
            CWALThumbTypes.Add(THUM.THUMSize.small, 0x0589DC44);
            CWALThumbTypes.Add(THUM.THUMSize.medium, 0x0589DC45);
            CWALThumbTypes.Add(THUM.THUMSize.large, 0x0589DC46);
            List<IResourceIndexEntry> seen = new List<IResourceIndexEntry>();
            foreach (THUM.THUMSize size in new THUM.THUMSize[] { THUM.THUMSize.small, THUM.THUMSize.medium, THUM.THUMSize.large, })
            {
                int i = 0;
                uint type = CWALThumbTypes[size];
                foreach (var ppt in FileTable.Thumbnails)
                    foreach (var sr in ppt.FindAll(rie => rie.ResourceType == type && rie.Instance == selectedItem.ResourceIndexEntry.Instance))
                    {
                        if (seen.Exists(sr.ResourceIndexEntry.Equals)) continue;
                        Add(size + "[" + i++ + "]Thumb", sr.RequestedRK);
                    }
            }
        }
        #endregion
        #endregion

        #region Make Unique bits
        string uniqueObject = null;
        string UniqueObject
        {
            get
            {
                if (uniqueObject == null)
                {
                    if (uniqueName == null || uniqueName.Length == 0)
                    {
                        StringInputDialog ond = new StringInputDialog();
                        ond.Caption = "Unique Resource Name";
                        ond.Prompt = "Enter unique identifier";
                        ond.Value = CreatorName + "_" + Guid.NewGuid().ToString("N") + "_" + DateTime.UtcNow.ToBinary().ToString("X16");
                        DialogResult dr = ond.ShowDialog();
                        if (dr == DialogResult.OK) uniqueObject = ond.Value;
                    }
                    else uniqueObject = uniqueName;
                }
                return uniqueObject;
            }
        }

        // A map from RK to SpecificResource
        Dictionary<IResourceKey, SpecificResource> rkToItem;
        void MapRKtoSpecificResource()
        {
            Diagnostics.Log("MapRKtoSpecificResource");
            rkToItem = new Dictionary<IResourceKey, SpecificResource>();

            // We need to process anything we found in the previous steps
            foreach (var kvp in rkLookup)
            {
                if (kvp.Value == RK.NULL) continue;
                if (rkToItem.ContainsKey(kvp.Value)) continue; // seen this TGI before
                SpecificResource item = new SpecificResource(FileTable.GameContent, kvp.Value);
                if (item.ResourceIndexEntry == null) continue; // TGI is not a packed resource
                rkToItem.Add(kvp.Value, item);
            }

            // We need to process STBLs
            foreach (var sr in FileTable.Current.FindAll(rie => rie.ResourceType == 0x220557DA && !rkToItem.ContainsKey(rie)))
                rkToItem.Add(sr.ResourceIndexEntry, sr);

            // We may also need to process RCOL internal chunks, TXTFs in TXTCs and NameMaps -- but only if we're renumbering
            if (isRenumber)
            {
                //If there are internal chunk references not covered by the above, we also need to add them
                Dictionary<IResourceKey, SpecificResource> rcolChunks = new Dictionary<IResourceKey, SpecificResource>();
                Dictionary<IResourceKey, SpecificResource> txtfs = new Dictionary<IResourceKey, SpecificResource>();
                foreach (var kvp in rkToItem)
                {
                    GenericRCOLResource rcol = kvp.Value.Resource as GenericRCOLResource;
                    if (rcol != null) // it's a GenericRCOL
                    {
                        foreach (GenericRCOLResource.ChunkEntry chunk in rcol.ChunkEntries)
                        {
                            if (chunk.TGIBlock == RK.NULL) continue;
                            if (rkToItem.ContainsKey(chunk.TGIBlock)) continue; // External reference and we've seen it
                            if (rcolChunks.ContainsKey(chunk.TGIBlock)) continue; // Internal reference and we've seen it
                            rcolChunks.Add(chunk.TGIBlock, kvp.Value);
                        }
                    }
                    TxtcResource.TxtcResource txtc = kvp.Value.Resource as TxtcResource.TxtcResource;
                    if (txtc != null) // it's a TXTC
                    {
                        foreach (var index in txtc.Root.SuperBlocks.Select(x => x.TGIIndex)) // All the referenced TXTFs
                        {
                            if (rkToItem.ContainsKey(txtc.Root.TGIBlocks[index])) continue; // External reference should never happen...
                            if (txtfs.ContainsKey(txtc.Root.TGIBlocks[index])) continue; // Internal reference and we've seen it
                            txtfs.Add(txtc.Root.TGIBlocks[index], kvp.Value);
                        }
                    }

                }
                foreach (var kvp in rcolChunks) rkToItem.Add(kvp.Key, kvp.Value);
                foreach (var kvp in txtfs) rkToItem.Add(kvp.Key, kvp.Value);

                // Add newest namemap
                foreach (var sr in FileTable.Current.FindAll(rie => rie.ResourceType == 0x0166038C && !rkToItem.ContainsKey(rie)))
                    rkToItem.Add(sr.ResourceIndexEntry, sr);
            }
        }

        // A list to hold the new numbers
        Dictionary<ulong, ulong> oldToNew;

        void CASP_GenerateNewIIDs()
        {
            Diagnostics.Log("CASP_GenerateNewIIDs");

            oldToNew = new Dictionary<ulong, ulong>();
            if (isRenumber)
            {
                foreach (var kvp in rkToItem)
                    if (!oldToNew.ContainsKey(kvp.Value.ResourceIndexEntry.Instance))//Only generate a new IID once per resource in the package
                        oldToNew.Add(kvp.Value.ResourceIndexEntry.Instance, CreateInstance());
                foreach (IResourceKey rk in rkToItem.Keys)//Requested RK
                    if (!oldToNew.ContainsKey(rk.Instance))//Find those references we don't have new IIDs for
                    {
                        if (rk.ResourceType == 0x736884F1 && rk.Instance >> 32 == 0)//Either it's a request for a VPXY using version...
                            oldToNew.Add(rk.Instance, oldToNew[rkToItem[rk].ResourceIndexEntry.Instance]);//So add the new number for that resource
                        else//Or it's an RCOL chunk or TXTF that needs a new IID...
                            oldToNew.Add(rk.Instance, CreateInstance());//So renumber it
                    }
            }
        }

        ulong nameGUID, newNameGUID;
        ulong descGUID, newDescGUID;
        void Catlg_GenerateNewIIDs()
        {
            Diagnostics.Log("Catlg_GenerateNewIIDs");

            oldToNew = new Dictionary<ulong, ulong>();

            ulong PngInstance = 0;
            if (isRenumber)
            {
                if (selectedItem.RequestedRK.CType() == CatalogType.ModularResource)
                    oldToNew.Add(selectedItem.ResourceIndexEntry.Instance, FNV64.GetHash(UniqueObject));//MDLR needs its IID as a specific hash value
                else
                {
                    PngInstance = (ulong)selectedItem.Resource["CommonBlock.PngInstance"].Value;
                    if (PngInstance != 0)
                        oldToNew.Add(PngInstance, CreateInstance());
                }
            }

            if (isRenumber || !isKeepSTBLIIDs)
            {
                // Generate new numbers for everything we've decided to renumber

                Dictionary<ulong, ulong> langMap = new Dictionary<ulong, ulong>();
                foreach (var kvp in rkToItem)
                    if (!oldToNew.ContainsKey(kvp.Value.ResourceIndexEntry.Instance))//Only generate a new IID once per resource in the package
                    {
                        IResourceKey rk = kvp.Value.ResourceIndexEntry;
                        if (rk.ResourceType == 0x220557DA)//STBL - we got here, so we're renumbering it!
                        {
                            if (!langMap.ContainsKey(rk.Instance & 0x00FFFFFFFFFFFFFF))
                                langMap.Add(rk.Instance & 0x00FFFFFFFFFFFFFF, CreateInstance() & 0x00FFFFFFFFFFFFFF);
                            oldToNew.Add(rk.Instance, rk.Instance & 0xFF00000000000000 | langMap[rk.Instance & 0x00FFFFFFFFFFFFFF]);
                        }
                        else if (isRenumber)
                        {
                            if (is32bitIIDs &&
                                (rk.ResourceType == (uint)CatalogType.CatalogObject || rk.ResourceType == 0x02DC343F))//OBJD&OBJK
                                oldToNew.Add(rk.Instance, CreateInstance32());
                            else
                                oldToNew.Add(rk.Instance, CreateInstance());
                        }
                    }
                if (isRenumber)
                    foreach (IResourceKey rk in rkToItem.Keys)//Requested RK
                        if (!oldToNew.ContainsKey(rk.Instance))//Find those references we don't have new IIDs for
                        {
                            if (rk.ResourceType == 0x736884F1 && rk.Instance >> 32 == 0)//Either it's a request for a VPXY using version...
                                oldToNew.Add(rk.Instance, oldToNew[rkToItem[rk].ResourceIndexEntry.Instance]);//So add the new number for that resource
                            else//Or it's an RCOL chunk or TXTF that needs a new IID...
                                oldToNew.Add(rk.Instance, CreateInstance());//So renumber it
                        }
            }

            SpecificResource catlgItem = selectedItem.RequestedRK.CType() == CatalogType.ModularResource
                ? ItemForTGIBlock0(selectedItem)
                : selectedItem;

            if (catlgItem.ResourceIndexEntry != null)
            {
                nameGUID = (ulong)catlgItem.Resource["CommonBlock.NameGUID"].Value;
                descGUID = (ulong)catlgItem.Resource["CommonBlock.DescGUID"].Value;

                if (isRenumber)
                {
                    newNameGUID = FNV64.GetHash("CatalogObjects/Name:" + UniqueObject);
                    newDescGUID = FNV64.GetHash("CatalogObjects/Description:" + UniqueObject);
                }
                else
                {
                    newNameGUID = nameGUID;
                    newDescGUID = descGUID;
                }
            }
        }

        int numNewInstances = 0;
        ulong CreateInstance() { numNewInstances++; return FNV64.GetHash(numNewInstances.ToString("X8") + "_" + UniqueObject + "_" + DateTime.UtcNow.ToBinary().ToString("X16")); }
        ulong CreateInstance32() { numNewInstances++; return FNV32.GetHash(numNewInstances.ToString("X8") + "_" + UniqueObject + "_" + DateTime.UtcNow.ToBinary().ToString("X16")); }

        void StartFixing()
        {
            MapRKtoSpecificResource();
            if (selectedItem.RequestedRK.CType() == CatalogType.CAS_Part)
                CASP_GenerateNewIIDs();
            else
                Catlg_GenerateNewIIDs();

            DoWait("Please wait, updating your package...");

            Dictionary<IResourceKey, SpecificResource> rkToItemAdded = new Dictionary<IResourceKey, SpecificResource>();

            SpecificResource catlgRes = (selectedItem.RequestedRK.CType() == CatalogType.ModularResource || selectedItem.RequestedRK.CType() == CatalogType.CatalogFireplace)
                ? ItemForTGIBlock0(selectedItem) : selectedItem;
            try
            {
                this.Enabled = false;

                if (isPadSTBLs)
                {
                    PadStbls(tbCatlgName.Text, nameGUID, newNameGUID);
                    PadStbls(tbCatlgDesc.Text, descGUID, newDescGUID);
                    NameMap.NMap.Commit();
                }

                //Need to take a copy of the ResourceList as it can get modified, which messes up the enumerator
                List<SpecificResource> lsr = FileTable.Current.FindAll(rie => true).ToList();
                foreach (SpecificResource item in lsr)
                {
                    bool dirty = false;

                    if (item.ResourceIndexEntry.ResourceType == 0x0166038C)
                    {
                        Diagnostics.Log("NMAP: " + item.LongName);
                        #region NameMap
                        IDictionary<ulong, string> nm = (IDictionary<ulong, string>)item.Resource;
                        foreach (ulong old in oldToNew.Keys)
                            if (nm.ContainsKey(old) && !nm.ContainsKey(oldToNew[old]))
                            {
                                nm.Add(oldToNew[old], nm[old]);
                                nm.Remove(old);
                                dirty = true;
                            }
                        #endregion
                        Diagnostics.Log("NMAP: " + item.LongName + " is" + (dirty ? "" : " not") + " dirty.");
                    }
                    else if (item.ResourceIndexEntry.ResourceType == 0x220557DA)//STBL
                    {
                        Diagnostics.Log("STBL: " + item.LongName);
                        dirty |= UpdateStbl(tbCatlgName.Text, nameGUID, item, newNameGUID);
                        dirty |= UpdateStbl(tbCatlgDesc.Text, descGUID, item, newDescGUID);
                        Diagnostics.Log("STBL: " + item.LongName + " is" + (dirty ? "" : " not") + " dirty.");
                    }
                    else if (item.ResourceIndexEntry.ResourceType == 0x0333406C)//XML
                    {
                        Diagnostics.Log("_XML: " + item.LongName);
                        dirty = ReplaceRKsInResourceStream(item, FixMatch, IIDReplacer);
                        Diagnostics.Log("_XML: " + item.LongName + " is" + (dirty ? "" : " not") + " dirty.");
                    }
                    else if (item.RequestedRK.Equals(selectedItem.RequestedRK) && item.RequestedRK.CType() == CatalogType.CAS_Part)//Deal with CASP separately..!
                    {
                        Diagnostics.Log("CAS_Part: " + item.LongName);
                        #region CAS Part
                        CASPartResource.CASPartResource casp = item.Resource as CASPartResource.CASPartResource;
                        // put all the details from the tab page into the resource
                        //...
                        casp.Unknown1 = tbCASPUnknown1.Text;
                        casp.Unknown4 = tbCASPUnknown4.Text;

                        casp.Clothing = cbCASPClothingType.GetValue(casp.Clothing);
                        casp.DataType = cbCASPTypeFlags.GetValue(casp.DataType);
                        casp.AgeGender.Age = clbCASPAgeFlags.GetValue(casp.AgeGender.Age);
                        casp.AgeGender.Species = cbCASPSpeciesType.GetValue(casp.AgeGender.Species);
                        casp.AgeGender.Gender = clbCASPGenderFlags.GetValue(casp.AgeGender.Gender);
                        casp.AgeGender.Handedness = clbCASPHandednessFlags.GetValue(casp.AgeGender.Handedness);

                        CASPartResource.RegionType region = cbCASPRegionType.GetValue(CASPartResource.RegionType.None);
                        casp.ClothingCategory = (CASPartResource.ClothingCategoryFlags)((uint)casp.ClothingCategory & 0xE1FFFFFF);
                        casp.ClothingCategory = clbCASPCategory.GetValue(casp.ClothingCategory) | CASPartResource.Extensions.ToClothingCategoryFlags(region);

                        for (int i = 0; i < casp.Presets.Count; i++)
                        {
                            CASPartResource.CASPartResource.Preset preset = casp.Presets[i];
                            using (StringWriter sw = new StringWriter())
                            {
                                if (ReplaceRKsInTextReader("CASP " + selectedItem.RequestedRK + ".Preset[" + i + "]", FixMatch, IIDReplacer, preset.XmlFile, sw))
                                {
                                    using (StringReader sr = new StringReader(sw.GetStringBuilder().ToString()))
                                    {
                                        preset.XmlFile = sr;
                                    }
                                }
                            }
                        }
                        ReplaceRKsInField(item, "", FixMatch, IIDReplacer, casp);

                        dirty = true;
                        #endregion
                        Diagnostics.Log("CAS_Part: " + item.LongName + " is" + (dirty ? "" : " not") + " dirty.");
                    }
                    else if ((item.RequestedRK.Equals(selectedItem.RequestedRK) && item.RequestedRK.CType() != CatalogType.ModularResource)//Selected CatlgItem
                    || item.RequestedRK.CType() == CatalogType.CatalogObject//all OBJDs (i.e. from MDLR or CFIR)
                    || item.RequestedRK.CType() == CatalogType.CatalogTerrainPaintBrush//all CTPTs (i.e. pair of selectedItem)
                    )
                    {
                        Diagnostics.Log("Selected CatlgItem || any OBJD || any CTPT: " + item.LongName);
                        #region Selected CatlgItem; all OBJD (i.e. from MDLR or CFIR)
                        AHandlerElement commonBlock = ((AHandlerElement)item.Resource["CommonBlock"].Value);

                        #region Selected CatlgItem || all MDLR OBJDs || both CTPTs || 0th CFIR OBJD
                        if (item.RequestedRK.Equals(selectedItem.RequestedRK)//Selected CatlgItem
                            || selectedItem.RequestedRK.CType() == CatalogType.ModularResource//all MDLR OBJDs
                            || selectedItem.RequestedRK.CType() == CatalogType.CatalogTerrainPaintBrush//both CTPTs
                            || (selectedItem.RequestedRK.CType() == CatalogType.CatalogFireplace && item.RequestedRK.Equals(catlgRes.RequestedRK))//0th CFIR OBJD
                            )
                        {
                            commonBlock["NameGUID"] = new TypedValue(typeof(ulong), newNameGUID);
                            commonBlock["DescGUID"] = new TypedValue(typeof(ulong), newDescGUID);
                            commonBlock["Price"] = new TypedValue(typeof(float), float.Parse(tbPrice.Text));

                            if (isRenumber)
                            {
                                commonBlock["Name"] = new TypedValue(typeof(string), "CatalogObjects/Name:" + UniqueObject);
                                commonBlock["Desc"] = new TypedValue(typeof(string), "CatalogObjects/Description:" + UniqueObject);
                            }
                        }
                        #endregion

                        if (item.RequestedRK.Equals(selectedItem.RequestedRK)//Selected CatlgItem
                            || (selectedItem.RequestedRK.CType() == CatalogType.ModularResource && item.RequestedRK.Equals(catlgRes.RequestedRK))//0th MDLR OBJD
                            //-- only selected CTPT
                            //-- none of the OBJDs for CFIR
                            )
                        {
                            commonBlock["BuildBuyProductStatusFlags"] = new TypedValue(commonBlock["BuildBuyProductStatusFlags"].Type, Convert.ToByte(tbProductStatus.Text, tbProductStatus.Text.StartsWith("0x") ? 16 : 10));
                        }

                        #region Selected CatlgItem; 0th OBJD from MDLR or CFIR
                        if (item.RequestedRK.Equals(selectedItem.RequestedRK)//Selected CatlgItem
                            || (selectedItem.RequestedRK.CType() == CatalogType.ModularResource && item.RequestedRK.Equals(catlgRes.RequestedRK))//0th MDLR OBJD
                            //-- only selected CTPT
                            || (selectedItem.RequestedRK.CType() == CatalogType.CatalogFireplace && item.RequestedRK.Equals(catlgRes.RequestedRK))//0th CFIR OBJD
                            )
                        {
                            ulong PngInstance = (ulong)commonBlock["PngInstance"].Value;
                            bool isPng = PngInstance != 0;

                            if (isIncludeThumbnails)
                            {
                                Image img = replacementForThumbs != null ? replacementForThumbs : THUM.getLargestThumbOrDefault(item);
                                ulong instance = isPng ? PngInstance : item.RequestedRK.Instance;

                                //Ensure one of each size exists
                                foreach (THUM.THUMSize size in new THUM.THUMSize[] { THUM.THUMSize.small, THUM.THUMSize.medium, THUM.THUMSize.large, })
                                {
                                    if (THUM.getImage(size, item) == null)
                                    {
                                        //Create missing thumbnail from default or replacement
                                        IResourceKey rk = THUM.getNewRK(size, item);
                                        SpecificResource thum = FileTable.Current.AddResource(rk);
                                        rkToItemAdded.Add(rk, thum);
                                        THUM.Thumb[item.RequestedRK.ResourceType, instance, size, isPng] = img;
                                    }
                                    else if (replacementForThumbs != null)
                                    {
                                        //Replace existing thumbnail
                                        THUM.Thumb[item.RequestedRK.ResourceType, instance, size, isPng] = replacementForThumbs;
                                    }
                                }
                            }

                            if (isPng && oldToNew.ContainsKey(PngInstance))
                                commonBlock["PngInstance"] = new TypedValue(typeof(ulong), oldToNew[PngInstance]);

                            if (tabControl1.TabPages.Contains(tpDetail))
                                IterateTLP(tlpObjectDetail, (l, c) => UpdateItem(item, l, c));

                            #region Selected OBJD only
                            if (selectedItem.RequestedRK.CType() == CatalogType.CatalogObject)//Selected OBJD only
                            {
                                foreach (flagField ff in flagFields)
                                {
                                    if (item.Resource.ContentFields.Contains(ff.field))
                                    {
                                        ulong old = getFlags(item.Resource as AResource, ff.field);
                                        ulong mask = (ulong)0xFFFFFFFF << ff.offset;
                                        ulong res = getFlags(ff);
                                        res |= (ulong)(old & ~mask);
                                        setFlags(item.Resource as AResource, ff.field, res);
                                    }
                                }

                                if (tlpOther.Visible)
                                {
                                    for (int i = 1; i < tlpOther.RowCount - 1; i++)
                                        IterateTLP(tlpOther, (l, c) => UpdateItem(item, l, c));
                                }
                            }
                            #endregion
                        }
                        #endregion

                        if (isRenumber)
                        {
                            #region Keep brushes together
                            if (item.RequestedRK.CType() == CatalogType.CatalogTerrainPaintBrush)//Both CTPTs
                            {
                                byte status = (byte)commonBlock["BuildBuyProductStatusFlags"].Value;
                                uint brushIndex = FNV32.GetHash(UniqueObject) << 1;
                                if ((status & 0x01) != 0)
                                    item.Resource["CommonBlock.UISortPriority"] = new TypedValue(typeof(uint), brushIndex);
                                else
                                    item.Resource["CommonBlock.UISortPriority"] = new TypedValue(typeof(uint), brushIndex + 1);
                            }
                            #endregion

                            if (item.RequestedRK.CType() == CatalogType.CatalogObject)
                            {
                                #region Avoid renumbering Fallback TGI
                                int fallbackIndex = (int)(uint)item.Resource["FallbackIndex"].Value;
                                TGIBlockList tgiBlocks = item.Resource["TGIBlocks"].Value as TGIBlockList;
                                AResourceKey fallbackRK = new TGIBlock(0, null, tgiBlocks[fallbackIndex]);

                                ReplaceRKsInField(item, "", FixMatch, IIDReplacer, (AResource)item.Resource);

                                tgiBlocks[fallbackIndex] = new TGIBlock(0, null, (IResourceKey)fallbackRK);
                                #endregion
                            }
                            else
                                ReplaceRKsInField(item, "", FixMatch, IIDReplacer, (AResource)item.Resource);
                        }

                        dirty = true;
                        #endregion
                        Diagnostics.Log("Selected CatlgItem || any OBJD || any CTPT: " + item.LongName + " is" + (dirty ? "" : " not") + " dirty.");
                    }
                    else
                    {
                        Diagnostics.Log("Anything else: " + item.LongName);
                        dirty = ReplaceRKsInField(item, "", FixMatch, IIDReplacer, (AResource)item.Resource);
                        Diagnostics.Log("Anything else: " + item.LongName + " is" + (dirty ? "" : " not") + " dirty.");
                    }

                    if (dirty) item.Commit();

                }

                foreach (var kvp in rkToItemAdded)
                    if (!rkToItem.ContainsKey(kvp.Key)) rkToItem.Add(kvp.Key, kvp.Value);

                foreach (SpecificResource item in rkToItem.Values)
                {
                    if (item.RequestedRK == RK.NULL) { continue; }
                    else if (!oldToNew.ContainsKey(item.ResourceIndexEntry.Instance)) { continue; }
                    else item.ResourceIndexEntry.Instance = oldToNew[item.ResourceIndexEntry.Instance];
                }

                if (!disableCompression)
                    FileTable.Current.Package.GetResourceList.ForEach(rie => rie.Compressed = 0xffff);

                FileTable.Current.Package.SavePackage();
            }
            finally
            {
                this.toolStripProgressBar1.Visible = false;
                this.toolStripStatusLabel1.Visible = false;
                this.Enabled = true;
                StopWait();
            }

            /*int dr = CopyableMessageBox.Show("Your updated package is ready.\nDo you wish to continue working\non it in s3oc?",
                myName, CopyableMessageBoxButtons.YesNo, CopyableMessageBoxIcon.Question);

            if (dr == 0)
                ReloadCurrentPackage();
            else/**/
                CopyableMessageBox.Show("Your updated package is ready.",
                    myName, CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Information);
                fileClose();
        }

        bool FixMatch(IResourceKey rk) { return rkToItem.ContainsKey(rk) && oldToNew.ContainsKey(rk.Instance); }
        IResourceKey IIDReplacer(IResourceKey rk) { return new TGIBlock(0, null, rk) { Instance = oldToNew[rk.Instance] }; }

        private bool UpdateStbl(string value, ulong guid, SpecificResource item, ulong newGuid)
        {
            IDictionary<ulong, string> stbl = (IDictionary<ulong, string>)item.Resource;
            if (!stbl.ContainsKey(guid)) return false;

            string text = stbl[guid];
            stbl.Remove(guid);
            if (ckbCopyToAll.Checked || item.ResourceIndexEntry.Instance >> 56 == 0x00) text = value;
            if (text != "") stbl.Add(newGuid, text);

            return true;
        }

        private void PadStbls(string value, ulong guid, ulong newGuid)
        {
            try
            {
                updateProgress(true, "Padding STBLs...", true, 0x17, true, 0);
                Application.DoEvents();

                List<ulong> stbls = new List<ulong>();
                FileTable.Current.Package.FindAll(x => x.ResourceType == 0x220557DA)
                    .ConvertAll<ulong>(x => { return x.Instance & 0x00FFFFFFFFFFFFFF; })
                    .ForEach(x => { if (!stbls.Contains(x)) stbls.Add(x); });

                IResourceKey newRK = new RK(new TGIBlock(0, null, 0x220557DA, 0, stbls.Count == 0 ? CreateInstance() & 0x00FFFFFFFFFFFFFF : stbls[0]));

                for (byte lang = 0; lang < 0x17; lang++)
                {
                    if (STBLHandler.findStblFor(guid, lang) == null)
                        CreateSTBL(newRK, newGuid, value, lang);
                    updateProgress(false, "", false, -1, true, lang);
                }
            }
            finally { updateProgress(true, "", true, -1, false, 0); Application.DoEvents(); }
        }

        void CreateSTBL(IResourceKey newRK, ulong newGuid, string value, byte lang)
        {
            newRK.Instance = (newRK.Instance & 0x00FFFFFFFFFFFFFF) | ((ulong)lang << 56);
            SpecificResource newstbl = new SpecificResource(FileTable.GameContent, newRK);
            if (newstbl.ResourceIndexEntry == null) newstbl = FileTable.Current.AddResource(newRK);

            IDictionary<ulong, string> stbl = (IDictionary<ulong, string>)newstbl.Resource;

            if (!stbl.ContainsKey(newGuid)) stbl.Add(newGuid, value);

            newstbl.Commit();

            if (NameMap.NMap != null && NameMap.NMap.map != null && !NameMap.NMap.map.ContainsKey(newRK.Instance))
                STBLHandler.AddSTBLToNameMap(NameMap.NMap.map, lang, newRK.Instance);

            if (!rkToItem.ContainsKey(newRK))
                rkToItem.Add(newRK, newstbl);
        }
        #endregion

        #region Create new package bits
        Thread saveThread;
        bool saving = false;
        bool waitingForSavePackage;
        PathPackageTuple target;
        void StartSaving()
        {
            this.Enabled = false;
            waitingForSavePackage = true;
            this.SavingComplete += new EventHandler<BoolEventArgs>(MainForm_SavingComplete);

            DoWait("Please wait, creating your new package...");
            Diagnostics.Log("Creating output package...");
            updateProgress(true, "Creating output package...", false, -1, false, -1);
            s3pi.Package.Package.NewPackage(0).SaveAs(saveFileDialog1.FileName);
            target = new PathPackageTuple(saveFileDialog1.FileName, true);//need to be able to write to it...

            SaveList sl = new SaveList(this,
                selectedItem, rkLookup,
                target, disableCompression, selectedItem.RequestedRK.CType() != CatalogType.CAS_Part,
                isPadSTBLs, false/*mode == Mode.FromGame/**/, cloneFixOptions.IsExcludeCommon,
                updateProgress, () => !saving, OnSavingComplete);

            saveThread = new Thread(new ThreadStart(sl.SavePackage));
            saving = true;
            saveThread.Start();
        }

        void AbortSaving(bool abort)
        {
            if (abort)
            {
                saving = false;
                if (saveThread != null) saveThread.Abort();
            }
            else
            {
                if (!saving) MainForm_SavingComplete(null, new BoolEventArgs(false));
                else saving = false;
            }
        }

        void MainForm_SavingComplete(object sender, BoolEventArgs e)
        {
            Diagnostics.Log(String.Format("MainForm_SavingComplete arg: {0}", e.arg));
            saving = false;
            this.Enabled = true;
            while (saveThread != null && saveThread.IsAlive)
                saveThread.Join(100);
            saveThread = null;

            this.toolStripProgressBar1.Visible = false;
            this.toolStripStatusLabel1.Visible = false;

            if (waitingForSavePackage)
            {
                waitingForSavePackage = false;
                if (e.arg)
                {
                    //isPadSTBLs = false;
                    Diagnostics.Log("Writing package: " + target.Path);
                    updateProgress(true, "Writing package...", true, 0, true, 0);
                    target.Package.SavePackage();
                    s3pi.Package.Package.ClosePackage(0, target.Package);

                    isCreateNewPackage = false;
                    fileReOpenToFix(target.Path, selectedItem.RequestedRK);
                }
                else
                {
                    if (File.Exists(target.Path))
                        CopyableMessageBox.Show("\nSave not complete.\nPlease ensure package is not in use.\n", myName, CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Warning);
                    else
                        CopyableMessageBox.Show("\nSave not complete.\n", myName, CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Warning);
                    //DisplayOptions();
                    DisplayNothing();//perhaps
                }
            }
        }

        private event EventHandler<BoolEventArgs> SavingComplete;
        public void OnSavingComplete(bool complete) { if (SavingComplete != null) { SavingComplete(this, new BoolEventArgs(complete)); } }
        #endregion

        #region Repairing bits
        Thread repairThread;
        bool repairing = false;
        bool waitingForRepair;
        private event EventHandler<BoolEventArgs> RepairingComplete;
        public void OnRepairingComplete(bool complete) { if (RepairingComplete != null) { RepairingComplete(this, new BoolEventArgs(complete)); } }

        void RunRepair()
        {
            Diagnostics.Log("RunRepair");
            this.Enabled = false;
            waitingForRepair = true;
            this.RepairingComplete += new EventHandler<BoolEventArgs>(MainForm_RepairComplete);

            DoWait("Please wait, adding missing resources...");
            SaveList sl = new SaveList(this, selectedItem, rkLookup,
                FileTable.Current, disableCompression, selectedItem.RequestedRK.CType() != CatalogType.CAS_Part,
                isPadSTBLs, false/*mode == Mode.FromGame/**/, cloneFixOptions.IsExcludeCommon,
                updateProgress, () => !repairing, OnRepairingComplete);

            repairThread = new Thread(new ThreadStart(sl.SavePackage));
            repairing = true;
            repairThread.Start();
        }

        void MainForm_RepairComplete(object sender, BoolEventArgs e)
        {
            Diagnostics.Log(String.Format("MainForm_RepairComplete arg: {0}", e.arg));
            repairing = false;
            this.Enabled = true;
            while (repairThread != null && repairThread.IsAlive)
                repairThread.Join(100);
            repairThread = null;

            this.toolStripProgressBar1.Visible = false;
            this.toolStripStatusLabel1.Visible = false;

            if (waitingForRepair)
            {
                waitingForRepair = false;
                if (e.arg)
                {
                    StartFixing();
                }
                else
                {
                    CopyableMessageBox.Show("Repair not complete.\n", myName, CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Warning);
                    DisplayOptions();
                }
            }
        }
        #endregion

        #region Replace TGI bits
        void replaceTGIPane_ReplaceClicked(object sender, EventArgs e)
        {
            Diagnostics.Log("replaceTGIPane_ReplaceClicked");

            DoWait("Please wait, replacing resource keys...");

            this.AcceptButton = null;
            this.CancelButton = null;

            replaceTGIPane.Results = "";
            replacements = new List<string>();

            try
            {
                foreach (SpecificResource item in FileTable.Current.FindAll(rie => true))
                {
                    bool dirty = false;

                    if (item.ResourceIndexEntry.ResourceType == 0x0333406C)//_XML
                    {
                        dirty = ReplaceRKsInResourceStream(item, ReplaceTGIMatch, ReplaceTGIReplacer);
                    }
                    else
                    {
                        dirty = ReplaceRKsInField(item, "", ReplaceTGIMatch, ReplaceTGIReplacer, item.Resource as AApiVersionedFields);
                    }

                    if (dirty) item.Commit();
                }
            }
            finally
            {
                this.toolStripProgressBar1.Visible = false;
                this.toolStripStatusLabel1.Visible = false;
                StopWait();
            }

            DisplayReplaceTGIResults();
        }

        bool ReplaceTGIMatch(IResourceKey rk) { return replaceTGIPane.FromCriteria.Match(rk); }
        IResourceKey ReplaceTGIReplacer(IResourceKey rk) { return replaceTGIPane.ToCriteria.GetValueOrDefault(rk); }

        void replaceTGIPane_SaveClicked(object sender, EventArgs e)
        {
            Diagnostics.Log("replaceTGIPane_SaveClicked");

            DoWait();

            FileTable.Current.Package.SavePackage();
            CopyableMessageBox.Show("Package saved.", myName, CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Information);

            ReloadCurrentPackage();
        }

        void replaceTGIPane_CancelClicked(object sender, EventArgs e)
        {
            Diagnostics.Log("replaceTGIPane_CancelClicked");

            if (replaceTGIPane.SaveEnabled)
            {
                int dr = CopyableMessageBox.Show("Any unsaved changes will be lost!\n\nAre you sure?", myName, CopyableMessageBoxButtons.YesNo, CopyableMessageBoxIcon.Exclamation);
                if (dr != 0) return;
            }

            ReloadCurrentPackage();
        }
        #endregion

        #region Renumber package bits
        void StartRenumbering()
        {
            numNewInstances = 0;
            oldToNew = new Dictionary<ulong, ulong>();
            replacements = new List<string>();

            #region Renumber CatalogResources and GUIDs

            // 1. Find all (non-FNV64Blank) GUIDs (GUID->Catlg SR)
            List<ulong> guidsSeen = new List<ulong>();
            foreach (SpecificResource sr in FileTable.Current.FindAll(rie => rie.ResourceType != 0xCF9A4ACE && Enum.IsDefined(typeof(CatalogType), rie.ResourceType)))
            {
                CatalogResource.CatalogResource cr = sr.Resource as CatalogResource.CatalogResource;
                if (cr == null) continue;

                // Whilst we're here on a CatalogResource, get its new IID
                if (!oldToNew.ContainsKey(sr.ResourceIndexEntry.Instance))
                    oldToNew.Add(sr.ResourceIndexEntry.Instance, RenumberingGUIDIID());

                // Now find its GUIDs
                if (cr.CommonBlock.NameGUID != STBLHandler.FNV64Blank && !guidsSeen.Contains(cr.CommonBlock.NameGUID))
                    guidsSeen.Add(cr.CommonBlock.NameGUID);
                if (cr.CommonBlock.DescGUID != STBLHandler.FNV64Blank && !guidsSeen.Contains(cr.CommonBlock.DescGUID))
                    guidsSeen.Add(cr.CommonBlock.DescGUID);
            }

            // 2. Delete GUID->Catlg entries where STBL not found (i.e. not current package, so not eligible for renumber)
            IEnumerable<IDictionary<ulong, string>> stbls = FileTable.Current
                .FindAll(rie => rie.ResourceType == 0x220557DA)
                .Select(sr => sr.Resource as IDictionary<ulong, string>)
                .Where(stbl => stbl != null);
            guidsSeen.Where(g => !stbls.Any(stbl => stbl.ContainsKey(g)))// Old GUID not in current package
                .ToList()// Don't think it's safe updating guidsSeen whilst it's enumerating itself
                .ForEach(g => guidsSeen.Remove(g));// So remove from those we'll renumber

            // 3. Generate new GUIDs
            Dictionary<ulong, ulong> oldToNewGUID = new Dictionary<ulong, ulong>();
            guidsSeen.ForEach(g => oldToNewGUID.Add(g, RenumberingGUIDIID()));

            // 4. Update Catlg entries
            foreach (SpecificResource sr in FileTable.Current.FindAll(rie => rie.ResourceType != 0xCF9A4ACE && Enum.IsDefined(typeof(CatalogType), rie.ResourceType)))
            {
                CatalogResource.CatalogResource cr = sr.Resource as CatalogResource.CatalogResource;
                if (cr == null) continue;

                bool dirty = false;

                if (oldToNewGUID.ContainsKey(cr.CommonBlock.NameGUID))
                {
                    string rpt = String.Format("{0}: Replaced NameGUID 0x{1:X16} with 0x{2:X16} in {3}", "" + sr.ResourceIndexEntry,
                        cr.CommonBlock.NameGUID, oldToNewGUID[cr.CommonBlock.NameGUID], cr.GetType().Name);
                    replacements.Add(rpt);
                    Diagnostics.Log(rpt);
                    cr.CommonBlock.NameGUID = oldToNewGUID[cr.CommonBlock.NameGUID];
                    dirty = true;
                }
                if (oldToNewGUID.ContainsKey(cr.CommonBlock.DescGUID))
                {
                    string rpt = String.Format("{0}: Replaced DescGUID 0x{1:X16} with 0x{2:X16} in {3}", "" + sr.ResourceIndexEntry,
                        cr.CommonBlock.DescGUID, oldToNewGUID[cr.CommonBlock.DescGUID], cr.GetType().Name);
                    replacements.Add(rpt);
                    Diagnostics.Log(rpt);
                    cr.CommonBlock.DescGUID = oldToNewGUID[cr.CommonBlock.DescGUID];
                    dirty = true;
                }

                if (dirty) sr.Commit();
            }

            // 5. Generate referenced STBL IIDs and update GUIDs
            Dictionary<ulong, ulong> langMap = new Dictionary<ulong, ulong>();
            foreach (SpecificResource sr in FileTable.Current.FindAll(rie => rie.ResourceType == 0x220557DA))
            {
                IDictionary<ulong, string> stbl = sr.Resource as IDictionary<ulong, string>;
                if (stbl == null) continue;
                if (new List<ulong>(stbl.Keys).FindAll(g => guidsSeen.Contains(g)).Count == 0) continue;//Not referenced

                if (!oldToNew.ContainsKey(sr.ResourceIndexEntry.Instance))
                {
                    if (!langMap.ContainsKey(sr.ResourceIndexEntry.Instance & 0x00FFFFFFFFFFFFFF))
                        langMap.Add(sr.ResourceIndexEntry.Instance & 0x00FFFFFFFFFFFFFF, RenumberingGUIDIID() & 0x00FFFFFFFFFFFFFF);
                    oldToNew.Add(sr.ResourceIndexEntry.Instance, sr.ResourceIndexEntry.Instance & 0xFF00000000000000 | langMap[sr.ResourceIndexEntry.Instance & 0x00FFFFFFFFFFFFFF]);
                }

                bool dirty = false;

                foreach (var kvp in oldToNewGUID)
                    if (stbl.ContainsKey(kvp.Key))
                    {
                        string rpt = String.Format("{0}: Replaced GUID 0x{1:X16} with 0x{2:X16} in STBL", "" + sr.ResourceIndexEntry, kvp.Key, kvp.Value);
                        replacements.Add(rpt);
                        Diagnostics.Log(rpt);
                        stbl.Add(kvp.Value, stbl[kvp.Key]);
                        stbl.Remove(kvp.Key);
                        dirty = true;
                    }

                if (dirty) sr.Commit();
            }

            #endregion

            #region Update TGIBlocks referencing package resources

            // RenumberingMatch doesn't work for RCOL ChunkEntries, so find these first and generate new IIDs
            foreach (var rcol in FileTable.Current
                .FindAll(rie => true)
                .Select(sr => sr.Resource as GenericRCOLResource)
                .Where(rcol => rcol != null))
            {
                foreach (var ce in rcol.ChunkEntries)
                {
                    if (ce.TGIBlock.Instance == 0) continue;// do not renumber zeros
                    if (!oldToNew.ContainsKey(ce.TGIBlock.Instance))
                        oldToNew.Add(ce.TGIBlock.Instance, RenumberingGUIDIID());
                }
            }

            foreach (SpecificResource item in FileTable.Current.FindAll(rie => true))
            {
                bool dirty = false;

                // RenumberingMatch does magic
                if (item.ResourceIndexEntry.ResourceType == 0x0333406C)//_XML
                    dirty = ReplaceRKsInResourceStream(item, RenumberingMatch, IIDReplacer);
                else
                    dirty = ReplaceRKsInField(item, "", RenumberingMatch, IIDReplacer, item.Resource as AApiVersionedFields);

                if (dirty) item.Commit();
            }
            #endregion

            #region Name map and ResourceIndex

            // 1. Generate referenced _KEY IIDs
            foreach (SpecificResource sr in FileTable.Current.FindAll(rie => rie.ResourceType == 0x0166038C))
            {
                IDictionary<ulong, string> namemap = sr.Resource as IDictionary<ulong, string>;
                if (namemap == null) continue;
                if (!namemap.ContainsKey(sr.ResourceIndexEntry.Instance)// Doesn't contain its own name
                    && (new List<ulong>(namemap.Keys).FindAll(i => oldToNew.ContainsKey(i)).Count == 0))// Doesn't contain names for anything we're renumbering
                    continue;

                if (!oldToNew.ContainsKey(sr.ResourceIndexEntry.Instance))
                    oldToNew.Add(sr.ResourceIndexEntry.Instance, RenumberingGUIDIID());
            }

            // 2. Update referenced _KEYs
            foreach (SpecificResource sr in FileTable.Current.FindAll(rie => rie.ResourceType == 0x0166038C))
            {
                IDictionary<ulong, string> namemap = sr.Resource as IDictionary<ulong, string>;
                if (namemap == null) continue;

                bool dirty = false;

                foreach (var kvp in oldToNew)
                    if (namemap.ContainsKey(kvp.Key))
                    {
                        namemap.Add(kvp.Value, namemap[kvp.Key]);
                        namemap.Remove(kvp.Key);
                        string rpt = String.Format("{0}: Replaced IID 0x{1:X16} with 0x{2:X16} in _KEY", "" + sr.ResourceIndexEntry, kvp.Key, kvp.Value);
                        replacements.Add(rpt);
                        Diagnostics.Log(rpt);
                        dirty = true;
                    }

                if (dirty) sr.Commit();
            }

            // 3. Update ResourceIndexEntries
            foreach (var rie in FileTable.Current.Package.FindAll(rie => oldToNew.ContainsKey(rie.Instance)))//Only referenced resources
            {
                string rpt = String.Format("{0}: Renumbered IID to 0x{1:X16}", "" + rie, oldToNew[rie.Instance]);
                replacements.Add(rpt);
                Diagnostics.Log(rpt);
                rie.Instance = oldToNew[rie.Instance];
            }

            #endregion
        }
        bool RenumberingMatch(IResourceKey rk)
        {
            if (rk.Instance == 0) return false;// do not renumber zeros
            if (rk.ResourceType == 0xCF9A4ACE) return false; // MDLR - absolutely never ever match
            if (rk.ResourceType == 0x220557DA) return false; // STBL - dealt with separately, not a reference
            if (oldToNew.ContainsKey(rk.Instance)) return true; // Already seen this old IID

            SpecificIndexEntry sie = new SpecificIndexEntry(FileTable.GameContent, rk);
            if (sie.ResourceIndexEntry == null || sie.PathPackage != FileTable.Current) return false; // Not found in current package, so we don't renumber it here

            if (rk.ResourceType == 0x736884F1 && rk.Instance >> 32 == 0) // It's a request for a VPXY using version...
            {
                if (!oldToNew.ContainsKey(sie.ResourceIndexEntry.Instance)) // Target not yet in map
                    oldToNew.Add(sie.ResourceIndexEntry.Instance, RenumberingGUIDIID());

                // Add a reference to the new canonical IID
                oldToNew.Add(rk.Instance, oldToNew[sie.ResourceIndexEntry.Instance]);
            }
            else
            {
                oldToNew.Add(rk.Instance, RenumberingGUIDIID());
            }
            return true;
        }
        ulong RenumberingGUIDIID()
        {
            return FNV64.GetHash((++numNewInstances).ToString("X8") + "_" + CreatorName + "_" + Guid.NewGuid().ToString("N") + "_" + DateTime.UtcNow.ToBinary().ToString("X16"));
        }

        void fixIntegrityResults_SaveClicked(object sender, EventArgs e)
        {
            Diagnostics.Log("fixIntegrityResults_SaveClicked");

            DoWait();

            FileTable.Current.Package.SavePackage();
            CopyableMessageBox.Show("Package saved.", myName, CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Information);

            ReloadCurrentPackage();
        }

        void fixIntegrityResults_CancelClicked(object sender, EventArgs e)
        {
            Diagnostics.Log("fixIntegrityResults_CancelClicked");

            int dr = CopyableMessageBox.Show("Any unsaved changes will be lost!\n\nAre you sure?", myName, CopyableMessageBoxButtons.YesNo, CopyableMessageBoxIcon.Exclamation);
            if (dr != 0) return;

            ReloadCurrentPackage();
        }
        #endregion
    }

    static class Extensions
    {
        public static CatalogType CType(this IResourceKey rk) { return (CatalogType)rk.ResourceType; }

        static string[] excludes = new string[] { "Value", "Stream", "AsBytes", };
        public static IEnumerable<Tuple<string, object>> FindAll(this AApiVersionedFields resource, string key, Predicate<object> match,
            Func<TextReader, string, IEnumerable<Tuple<string, object>>> findAllTextReader = null,
            Func<BinaryReader, string, IEnumerable<Tuple<string, object>>> findAllBinaryReader = null)
        {
            if (match(resource))
                yield return Tuple.Create<string, object>(key, resource);

            else foreach (string field in resource.ContentFields.Where(x => !excludes.Contains(x)))
                {
                    TypedValue tv = resource[field];
                    string name = key + "." + field;

                    // string is enumerable but we want to treat it as a single value
                    if (typeof(string).IsAssignableFrom(tv.Type))
                    {
                        if (match(tv.Value))
                            yield return Tuple.Create<string, object>(name, tv.Value);
                    }

                    else if (typeof(IEnumerable).IsAssignableFrom(tv.Type))
                    {
                        int i = 0;
                        foreach (var e in tv.Value as IEnumerable)
                        {
                            string elem = name + "[" + (i++) + "]";
                            if (e is AApiVersionedFields)
                                foreach (var tuple in (e as AApiVersionedFields).FindAll(elem, match, findAllTextReader, findAllBinaryReader))
                                    yield return tuple;

                            else if (e is TextReader && findAllTextReader != null)
                                foreach (var tuple in findAllTextReader(e as TextReader, elem))
                                    yield return tuple;

                            else if (e is BinaryReader && findAllBinaryReader != null)
                                foreach (var tuple in findAllBinaryReader(e as BinaryReader, elem))
                                    yield return tuple;

                            else if (match(e))
                                yield return Tuple.Create<string, object>(elem, e);
                        }
                    }

                    else if (typeof(AApiVersionedFields).IsAssignableFrom(tv.Type))
                        foreach (var tuple in (tv.Value as AApiVersionedFields).FindAll(name, match, findAllTextReader, findAllBinaryReader))
                            yield return tuple;

                    else if (typeof(TextReader).IsAssignableFrom(tv.Type) && findAllTextReader != null)
                        foreach (var value in findAllTextReader(tv.Value as TextReader, name))
                            yield return value;

                    else if (typeof(BinaryReader).IsAssignableFrom(tv.Type) && findAllBinaryReader != null)
                        foreach (var tuple in findAllBinaryReader(tv.Value as BinaryReader, name))
                            yield return tuple;

                    else if (match(tv.Value))
                        yield return Tuple.Create<string, object>(name, tv.Value);
                }
        }

        public static IEnumerable<Tuple<string, object>> SlurpRKs(this TextReader tr, string key)
        {
            const int keyLen = 3 + 1 + 8 + 1 + 8 + 1 + 16;//key:TTTTTTTT:GGGGGGGG:IIIIIIIIIIIIIIII
            string line = tr.ReadLine();
            IResourceKey rk = null;
            int linePos = 0;
            int index = -1;
            int ctr = 0;

            while (line != null)
            {
                index = line.IndexOf("key:", linePos);
                while (index == -1)
                {
                    line = tr.ReadLine();
                    if (line == null)
                        yield break;

                    linePos = 0;
                    index = line.IndexOf("key:", linePos);
                }
                linePos += keyLen;
                if (linePos > line.Length)
                {
                    line = tr.ReadLine();
                    continue;
                }

                string oldKey = line.Substring(index, keyLen).Substring(4).Replace('-', ':');
                string RKkey = "0x" + oldKey.Replace(":", "-0x");//translate to s3pi format
                if (RK.TryParse(RKkey, out rk))
                    yield return Tuple.Create<string, object>(key + "[" + (ctr++) + "]", rk);
            }
        }

        public static T GetValue<T>(this CheckedListBox clb, T value)
        {
            uint uValue = Convert.ToUInt32(value);
            foreach (var typeFlag in clb.Items)
            {
                uint flag = Convert.ToUInt32(Enum.Parse(typeof(T), typeFlag + ""));
                uValue = (uValue & ~flag) | (clb.CheckedItems.Contains(typeFlag) ? flag : 0);
            }
            return (T)Enum.ToObject(typeof(T), uValue);
        }

        public static T GetValue<T>(this ComboBox cb, T value)
        {
            if (cb.SelectedIndex != -1)
                return (T)Enum.Parse(typeof(T), cb.SelectedItem + "");
            return value;
        }
    }
}
