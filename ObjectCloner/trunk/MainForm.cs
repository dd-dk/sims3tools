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
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Windows.Forms;
using s3pi.Interfaces;
using s3pi.Extensions;
using s3pi.GenericRCOLResource;
using ObjectCloner.TopPanelComponents;

namespace ObjectCloner
{
    public partial class MainForm : Form
    {
        #region Static bits
        static string myName = "s3oc";
        static Dictionary<View, MenuBarWidget.MB> viewMap;
        static List<View> viewMapKeys;
        static List<MenuBarWidget.MB> viewMapValues;

        static string[] subPageText = new string[] {
            "You should never see this",
            "Step 1: Selected OBJD",
            "Step 2: OBJD-referenced resources",
            "Step 3: VPXY",
            "Step 4: VPXY-referenced resources",
            "Step 5: Preset XML (same instance as VPXY)",
            "Step 6: MODL-referenced resources",
            "Step 7: MLOD-referenced resources",
            "Step 8: TXTC-referenced resources",
            "Step 9: Thumbnails for OBJD",
            "Step 10: Fix integrity step",
            "You should never see this",
        };

        static MainForm()
        {
            viewMap = new Dictionary<View, MenuBarWidget.MB>();
            viewMap.Add(View.Tile, MenuBarWidget.MB.MBV_tiles);
            viewMap.Add(View.LargeIcon, MenuBarWidget.MB.MBV_largeIcons);
            viewMap.Add(View.SmallIcon, MenuBarWidget.MB.MBV_smallIcons);
            viewMap.Add(View.List, MenuBarWidget.MB.MBV_list);
            viewMap.Add(View.Details, MenuBarWidget.MB.MBV_detailedList);
            viewMapKeys = new List<View>(viewMap.Keys);
            viewMapValues = new List<MenuBarWidget.MB>(viewMap.Values);
        }
        #endregion

        enum Page
        {
            None = 0,
            ObjectChooser,
            Resources,
        }
        enum SubPage
        {
            None = 0,
            Step1,//Bring in the OBJD the user selected
            Step2,//Bring in all the OBJK (or, on request, all resources in all the TGI blocks of the OBJD)
            Step3,//Bring in the VPXY pointed to by the OBJK
            Step4,//Try to get everything referenced from the VPXY (some may be not found but that's OK)
            Step5,//Bring in Preset XML (same instance as VPXY)
            Step6,//Bring in all the resources in the TGI blocks of the MODL
            Step7,//Bring in all the resources in the TGI blocks of each MLOD (disregard duplicates)
            Step8,//Bring in all the resources in the TGI blocks of each TXTC (disregard duplicates)
            LastInChain = Step8,
            Step9,//Bring in all resources from ...\The Sims 3\Thumbnails\ALLThumbnails.package that match the instance number of the OBJD
            Step10,//Fix integrity step
        }
        enum Mode
        {
            None = 0,
            Clone,
            Fix,
        }

        Mode mode = Mode.None;
        SubPage subPage = SubPage.None;
        SubPage lastSubPage = SubPage.None;
        View currentView;
        IPackage pkg = null;
        TGI clone;//0x319E4F1D
        Item objdItem;
        Item objkItem;
        TGI vpxy;
        Image replacementForThumbs;

        Dictionary<string, TGI> tgiLookup = new Dictionary<string,TGI>();

        private ObjectCloner.TopPanelComponents.ObjectChooser objectChooser;
        private ObjectCloner.TopPanelComponents.ResourceList resourceList;
        private ObjectCloner.TopPanelComponents.PleaseWait pleaseWait;
        public MainForm()
        {
            InitializeComponent();
            objectChooser = new ObjectChooser();
            objectChooser.SelectedIndexChanged += new EventHandler(objectChooser_SelectedIndexChanged);
            objectChooser.ItemActivate += new EventHandler(objectChooser_ItemActivate);
            resourceList = new ResourceList();
            pleaseWait = new PleaseWait();

            this.Text = myName;
            MainForm_LoadFormSettings();

            InitialiseTabs();
            setButtons(Page.None, SubPage.None);
        }


        private void MainForm_LoadFormSettings()
        {
            int h = ObjectCloner.Properties.Settings.Default.PersistentHeight;
            if (h == -1) h = 4 * System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height / 5;
            this.Height = h;

            int w = ObjectCloner.Properties.Settings.Default.PersistentWidth;
            if (w == -1) w = 4 * System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width / 5;
            this.Width = w;

            w = ObjectCloner.Properties.Settings.Default.Splitter1Width;
            if (w > 0 && w < 100)
                splitContainer1.SplitterDistance = this.Width * w / 100;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            View view = Enum.IsDefined(typeof(View), ObjectCloner.Properties.Settings.Default.View)
                ? viewMapKeys[ObjectCloner.Properties.Settings.Default.View]
                : View.Details;
            menuBarWidget1_MBView_Click(null, new MenuBarWidget.MBClickEventArgs(viewMap[view]));

            menuBarWidget1.Checked(MenuBarWidget.MB.MBV_icons, ObjectCloner.Properties.Settings.Default.ShowThumbs);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            AbortLoading(e.CloseReason == CloseReason.ApplicationExitCall);
            AbortFetching(e.CloseReason == CloseReason.ApplicationExitCall);
            AbortSaving(e.CloseReason == CloseReason.ApplicationExitCall);
            ClosePkg();
            if (thumbpkg != null) { s3pi.Package.Package.ClosePackage(0, thumbpkg); thumbpkg = null; }

            objectChooser.ObjectChooser_SaveSettings();

            ObjectCloner.Properties.Settings.Default.PersistentHeight = this.WindowState == FormWindowState.Normal ? this.Height : -1;
            ObjectCloner.Properties.Settings.Default.PersistentWidth = this.WindowState == FormWindowState.Normal ? this.Width : -1;
            ObjectCloner.Properties.Settings.Default.Splitter1Width = splitContainer1.SplitterDistance * 100 / this.Width;
            ObjectCloner.Properties.Settings.Default.ShowThumbs = menuBarWidget1.IsChecked(MenuBarWidget.MB.MBV_icons);
            ObjectCloner.Properties.Settings.Default.Save();
        }

        string fullBuild0Path = null;
        string FullBuild0Path
        {
            get
            {
                if (fullBuild0Path == null)
                {
                    if (ObjectCloner.Properties.Settings.Default.Sims3Folder == null || ObjectCloner.Properties.Settings.Default.Sims3Folder.Length == 0
                        || !Directory.Exists(ObjectCloner.Properties.Settings.Default.Sims3Folder))
                        settingsSims3Folder();
                    if (ObjectCloner.Properties.Settings.Default.Sims3Folder == null || ObjectCloner.Properties.Settings.Default.Sims3Folder.Length == 0
                        || !Directory.Exists(ObjectCloner.Properties.Settings.Default.Sims3Folder))
                        return null;
                }
                fullBuild0Path = Path.Combine(ObjectCloner.Properties.Settings.Default.Sims3Folder, @"GameData\Shared\Packages\FullBuild0.package");
                return fullBuild0Path;
            }
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

        IPackage thumbpkg = null;
        IPackage ThumbPkg
        {
            get
            {
                if (thumbpkg == null)
                    thumbpkg = s3pi.Package.Package.OpenPackage(0, Path.Combine(ObjectCloner.Properties.Settings.Default.Sims3Folder, @"Thumbnails/ALLThumbnails.package"));
                return thumbpkg;
            }
        }

        IDictionary<ulong, string> english = null;
        IDictionary<ulong, string> English
        {
            get
            {
                if (pkg == null) return null;
                if (english == null)
                {
                    IList<IResourceIndexEntry> lrie = pkg.FindAll(new String[] { "ResourceType", }, new TypedValue[] { new TypedValue(typeof(uint), (uint)0x220557DA), });
                    foreach (IResourceIndexEntry rie in lrie)
                        if (rie.Instance >> 56 == 0x00)
                        {
                            english = new Item(pkg, rie).Resource as IDictionary<ulong, string>;
                            break;
                        }
                }
                return english;
            }
        }

        void ClosePkg()
        {
            if (pkg == null) return;
            s3pi.Package.Package.ClosePackage(0, pkg);
            pkg = null;
            haveLoaded = false;
            loadedPackage = "";
            english = null;
        }

        #region TopPanelComponents
        bool waitingToDisplayObjects;
        private void DisplayObjectChooser()
        {
            waitingToDisplayObjects = false;

            menuBarWidget1.Enable(MenuBarWidget.MB.MBF_new, mode == Mode.Fix); // don't need to re-do FB0
            menuBarWidget1.Enable(MenuBarWidget.MB.MBF_open, true); // do need to allow changing other packages
            setButtons(Page.ObjectChooser, subPage);

            splitContainer1.Panel1.Controls.Clear();
            splitContainer1.Panel1.Controls.Add(objectChooser);
            objectChooser.Dock = DockStyle.Fill;
            objectChooser.Focus();
        }

        private void DisplayResources()
        {
            setButtons(Page.Resources, subPage);

            resourceList.Page = subPageText[(int)subPage];
            splitContainer1.Panel1.Controls.Clear();
            splitContainer1.Panel1.Controls.Add(resourceList);
            resourceList.Dock = DockStyle.Fill;
            resourceList.Focus();
        }

        private void DoWait() { DoWait("Please wait..."); }
        private void DoWait(string waitText)
        {
            splitContainer1.Panel1.Controls.Clear();
            splitContainer1.Panel1.Controls.Add(pleaseWait);
            pleaseWait.Dock = DockStyle.Fill;
            pleaseWait.Label = waitText;
            Application.DoEvents();
        }
        #endregion

        #region ObjectChooser
        private void objectChooser_SelectedIndexChanged(object sender, EventArgs e)
        {
            subPage = SubPage.None;
            resourceList.Clear();
            tgiLookup.Clear();
            replacementForThumbs = null;
            if (objectChooser.SelectedItems.Count == 0) ClearTabs();
            else FillTabs(objectChooser.SelectedItems[0].Tag as Item);
            setButtons(Page.ObjectChooser, subPage);
        }

        void objectChooser_ItemActivate(object sender, EventArgs e)
        {
            btnStart_Click(sender, e);
        }
        #endregion

        #region Tabs
        List<string> detailsTabFields = new List<string>();
        List<string> detailsTabCommonFields = new List<string>();
        List<string> flagsTabFields = new List<string>();
        void InitialiseTabs()
        {
            IResource res = s3pi.WrapperDealer.WrapperDealer.CreateNewResource(0, "0x319E4F1D");
            List<string> fields = AApiVersionedFields.GetContentFields(0, res.GetType());
            foreach (string field in fields)
            {
                if (field.StartsWith("Unknown")) continue;
                if (field.EndsWith("Flags")) flagsTabFields.Add(field);
                else if (field.EndsWith("Index")) { }//ignore
                else if (field.Equals("Materials")) { }
                else if (field.Equals("MTDoors")) { }
                else if (field.Equals("TGIBlocks")) { }
                else if (field.Equals("CommonBlock"))
                {
                    List<string> commonfields = AApiVersionedFields.GetContentFields(0, res["CommonBlock"].Type);
                    foreach (string commonfield in commonfields)
                    {
                        if (commonfield.StartsWith("Unknown")) continue;
                        if (!commonfield.Equals("Value")) detailsTabCommonFields.Add(commonfield);
                    }
                }
                else if (!field.Equals("AsBytes") && !field.Equals("Stream") && !field.Equals("Value") &&
                    !field.Equals("Count") && !field.Equals("IsReadOnly") && !field.EndsWith("Reader")) detailsTabFields.Add(field);
            }
            pictureBox1.Image = null;
            tbObjName.Text = "";
            tbCatlgName.Text = "";
            tbObjDesc.Text = "";
            tbCatlgDesc.Text = "";
            InitialiseDetailsTab(res);
        }
        void InitialiseDetailsTab(IResource objd)
        {
            Dictionary<string, Type> types = AApiVersionedFields.GetContentFieldTypes(0, objd.GetType());
            foreach (string field in detailsTabFields)
            {
                CreateField(tlpOverviewMain, types[field], objd as AResource, field);
            }

            AApiVersionedFields common = objd["CommonBlock"].Value as AApiVersionedFields;
            types = AApiVersionedFields.GetContentFieldTypes(0, objd["CommonBlock"].Type);
            foreach (string field in detailsTabCommonFields)
            {
                CreateField(tlpOverviewCommon, types[field], common, field);
            }
        }

        void CreateField(TableLayoutPanel target, Type t, AApiVersionedFields obj, string field)
        {
            if (typeof(IConvertible).IsAssignableFrom(t))
            {
                switch (((IConvertible)obj[field].Value).GetTypeCode())
                {
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                        CreateField(target, field, 2 + 2);
                        break;
                    case TypeCode.Boolean:
                    case TypeCode.Char:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                        CreateField(target, field, 2 + 4);
                        break;
                    case TypeCode.Single:
                    case TypeCode.DBNull:
                    case TypeCode.Empty:
                        CreateField(target, field, 8);
                        break;
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                        CreateField(target, field, 2 + 8);
                        break;
                    case TypeCode.Double:
                        CreateField(target, field, 16);
                        break;
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                        CreateField(target, field, 2 + 16);
                        break;
                    case TypeCode.Decimal:
                    case TypeCode.DateTime:
                    case TypeCode.String:
                    case TypeCode.Object:
                        CreateField(target, field, 30);
                        break;
                }
            }
            else
                CreateField(target, field, 30);
        }

        void CreateField(TableLayoutPanel tlp, string name, int len)
        {
            tlp.RowCount++;
            tlp.RowStyles.Insert(tlp.RowCount - 2, new RowStyle(SizeType.AutoSize));

            Label lb = new Label();
            lb.Anchor = AnchorStyles.Right;
            lb.AutoSize = true;
            lb.Name = "lb" + name;
            lb.TabIndex = 1;
            lb.Text = name;
            tlp.Controls.Add(lb, 0, tlp.RowCount - 2);

            Label x = new Label();
            x.AutoSize = true;
            x.Text = "".PadLeft(len, 'X');

            TextBox tb = new TextBox();
            tb.Anchor = AnchorStyles.Left;
            tb.Name = "tb" + name;
            tb.ReadOnly = true;
            tb.Size = new Size(x.PreferredWidth + 6, x.PreferredHeight + 6);
            tb.TabIndex = 2;
            //tb.Text = x.Text;
            tlp.Controls.Add(tb, 1, tlp.RowCount - 2);
        }

        void ClearTabs()
        {
            pictureBox1.Image = null;
            tbObjName.Text = "";
            tbCatlgName.Text = "";
            tbObjDesc.Text = "";
            tbCatlgDesc.Text = "";
            tbPrice.Text = "";
            tbCatlgName.Enabled = false;
            tbCatlgDesc.Enabled = false;
            ckbCopyToAll.Enabled = false;
            tbPrice.ReadOnly = true;
            foreach (Control c in tlpOverviewMain.Controls)
                if (c is TextBox) ((TextBox)c).Text = "";
            foreach (Control c in tlpOverviewCommon.Controls)
                if (c is TextBox) ((TextBox)c).Text = "";
        }
        void FillTabs(Item objd)
        {
            pictureBox1.Image = GetLargeThumb(objd);
            btnReplThumb.Enabled = mode == Mode.Fix;
            AApiVersionedFields common = objd.Resource["CommonBlock"].Value as AApiVersionedFields;
            tbObjName.Text = common["Name"].Value + "";
            tbObjDesc.Text = common["Desc"].Value + "";
            tbCatlgName.Text = GetSTBLValue((ulong)common["NameGUID"].Value);
            tbCatlgDesc.Text = GetSTBLValue((ulong)common["DescGUID"].Value);
            tbPrice.Text = common["Price"].Value + "";
            tbCatlgName.Enabled = tbCatlgDesc.Enabled = ckbCopyToAll.Enabled = mode == Mode.Fix;
            tbPrice.ReadOnly = mode == Mode.Clone;

            for (int i = 1; i < tlpOverviewMain.RowCount - 1; i++)
            {
                Label lb = (Label)tlpOverviewMain.GetControlFromPosition(0, i);
                TextBox tb = (TextBox)tlpOverviewMain.GetControlFromPosition(1, i);

                TypedValue tv = objd.Resource[lb.Text];
                tb.Text = tv;
            }
            for (int i = 2; i < tlpOverviewCommon.RowCount - 1; i++)
            {
                Label lb = (Label)tlpOverviewCommon.GetControlFromPosition(0, i);
                TextBox tb = (TextBox)tlpOverviewCommon.GetControlFromPosition(1, i);

                TypedValue tv = ((AApiVersionedFields)objd.Resource["CommonBlock"].Value)[lb.Text];
                tb.Text = tv;
            }
        }

        public Image GetLargeThumb(Item objd)
        {
            Image res = getImage(0x2e75c766, objd);
            if (res == null)
            {
                TGI img = objd.tgi; img.t = 0x0580A2B6; Item pngItem = new Item(mode == Mode.Clone ? ThumbPkg : objd.Package, img);
                if (pngItem.ResourceIndexEntry != null) res = getImage(pngItem);
            }
            return res;
        }

        public static Image getImage(Item pngItem) { return Image.FromStream(pngItem.Resource.Stream); }

        public static Image getImage(uint type, Item objd)
        {
            ulong png = (ulong)((AHandlerElement)objd.Resource["CommonBlock"].Value)["PngInstance"].Value;
            if (png != 0)
            {
                Item pngItem = new Item(objd.Package, new TGI(type, 0, png));
                if (pngItem.ResourceIndexEntry != null) return getImage(pngItem);
            }
            return null;
        }

        string GetSTBLValue(ulong guid)
        {
            if (English != null && English.ContainsKey(guid)) return English[guid];
            return "";
        }
        #endregion


        #region Loading thread
        Thread loadThread;
        bool haveLoaded = false;
        bool loading = false;
        string loadedPackage;
        void StartLoading(string path)
        {
            if (haveLoaded && loadedPackage == path) return;
            if (loading) { AbortLoading(false); }

            waitingToDisplayObjects = true;
            haveLoaded = false;
            loadedPackage = path;
            objectChooser.Items.Clear();

            this.LoadingComplete += new EventHandler<BoolEventArgs>(MainForm_LoadingComplete);

            FillListView flv = new FillListView(this, path, pkg, createListViewItem, updateProgress, stopLoading, OnLoadingComplete);

            loadThread = new Thread(new ThreadStart(flv.LoadPackage));
            loading = true;
            loadThread.Start();
        }

        void AbortLoading(bool abort)
        {
            waitingToDisplayObjects = false;
            if (abort)
            {
                loading = false;
                if (loadThread != null) loadThread.Abort();
            }
            else
            {
                if (!loading) MainForm_LoadingComplete(null, new BoolEventArgs(false));
                else loading = false;
            }
        }

        void MainForm_LoadingComplete(object sender, BoolEventArgs e)
        {
            loading = false;
            while (loadThread != null && loadThread.IsAlive)
                loadThread.Join(100);
            loadThread = null;

            this.toolStripProgressBar1.Visible = false;
            this.toolStripStatusLabel1.Visible = false;

            if (e.arg)
            {
                haveLoaded = true;

                if (waitingToDisplayObjects)
                {
                    if (mode == Mode.Clone && menuBarWidget1.IsChecked(MenuBarWidget.MB.MBV_icons))
                    {
                        waitingForImages = true;
                        StartFetching();
                    }

                    if (objectChooser.Items.Count > 0) objectChooser.SelectedIndex = 0;

                    DisplayObjectChooser();
                }
            }
            else
            {
                ClosePkg();
            }
        }

        Dictionary<int, int> LItoIMG32 = new Dictionary<int, int>();
        Dictionary<int, int> LItoIMG64 = new Dictionary<int, int>();

        public delegate void createListViewItemCallback(Item objd);
        void createListViewItem(Item objd)
        {
            ListViewItem lvi = new ListViewItem();
            string objdtag = ((AApiVersionedFields)objd.Resource["CommonBlock"].Value)["Name"];
            lvi.Text = (objdtag.IndexOf(':') < 0) ? objdtag : objdtag.Substring(objdtag.LastIndexOf(':') + 1);
            lvi.SubItems.AddRange(new string[] { objd.tgi, });
            lvi.Tag = objd;
            objectChooser.Items.Add(lvi);
        }

        public delegate bool stopLoadingCallback();
        private bool stopLoading() { return !loading; }

        private event EventHandler<BoolEventArgs> LoadingComplete;
        public delegate void loadingCompleteCallback(bool complete);
        public void OnLoadingComplete(bool complete) { if (LoadingComplete != null) { LoadingComplete(this, new BoolEventArgs(complete)); } }

        #endregion

        #region Fetch Images thread
        Thread fetchThread;
        bool haveFetched = false;
        bool fetching = false;
        void StartFetching()
        {
            if (!haveLoaded) return;

            objectChooser.SmallImageList = new ImageList();
            objectChooser.SmallImageList.ImageSize = new System.Drawing.Size(32, 32);
            LItoIMG32 = new Dictionary<int, int>();
            objectChooser.LargeImageList = new ImageList();
            objectChooser.LargeImageList.ImageSize = new System.Drawing.Size(64, 64);
            LItoIMG64 = new Dictionary<int, int>();

            this.FetchingComplete += new EventHandler<BoolEventArgs>(MainForm_FetchingComplete);

            FetchImages fi = new FetchImages(this, objectChooser.Items.Count, ThumbPkg,
                getItem, setImage, updateProgress, stopFetching, OnFetchingComplete);

            fetchThread = new Thread(new ThreadStart(fi.Fetch));
            fetching = true;
            fetchThread.Start();
        }

        void AbortFetching(bool abort)
        {
            if (abort)
            {
                fetching = false;
                if (fetchThread != null) fetchThread.Abort();
            }
            else
            {
                if (!fetching) MainForm_FetchingComplete(null, new BoolEventArgs(false));
                else fetching = false;
            }
        }

        bool waitingForImages;
        void MainForm_FetchingComplete(object sender, BoolEventArgs e)
        {
            fetching = false;
            while (fetchThread != null && fetchThread.IsAlive)
                fetchThread.Join(100);
            fetchThread = null;

            this.toolStripProgressBar1.Visible = false;
            this.toolStripStatusLabel1.Visible = false;

            if (e.arg)
            {
                haveFetched = true;
            }

            if (waitingForImages)
            {
                waitingForImages = false;
            }
        }

        public delegate bool stopFetchingCallback();
        private bool stopFetching() { return !fetching; }

        public delegate Item getItemCallback(int i);
        private Item getItem(int i) { return objectChooser.Items[i].Tag as Item; }

        public delegate void setImageCallback(bool smallImage, int i, Image image);
        private void setImage(bool smallImage, int i, Image image)
        {
            if (smallImage)
            {
                LItoIMG32.Add(i, objectChooser.SmallImageList.Images.Count);
                objectChooser.SmallImageList.Images.Add(image);
                if (currentView != View.Tile && currentView != View.LargeIcon)
                    objectChooser.Items[i].ImageIndex = LItoIMG32[i];
            }
            else
            {
                LItoIMG64.Add(i, objectChooser.LargeImageList.Images.Count);
                objectChooser.LargeImageList.Images.Add(image);
                if (currentView == View.Tile || currentView == View.LargeIcon)
                    objectChooser.Items[i].ImageIndex = LItoIMG64[i];
            }
        }

        private event EventHandler<BoolEventArgs> FetchingComplete;
        public delegate void fetchingCompleteCallback(bool complete);
        public void OnFetchingComplete(bool complete) { if (FetchingComplete != null) { FetchingComplete(this, new BoolEventArgs(complete)); } }

        class FetchImages
        {
            MainForm mainForm;
            int count;
            IPackage thumbpkg;
            getItemCallback getItemCB;
            setImageCallback setImageCB;
            updateProgressCallback updateProgressCB;
            stopFetchingCallback stopFetchingCB;
            fetchingCompleteCallback fetchingCompleteCB;

            int imgcnt = 0;

            public FetchImages(MainForm form, int count, IPackage thumbpkg,
                getItemCallback getItemCB, setImageCallback setImageCB,
                updateProgressCallback updateProgressCB, stopFetchingCallback stopFetchingCB, fetchingCompleteCallback fetchingCompleteCB)
            {
                this.mainForm = form;
                this.count = count;
                this.thumbpkg = thumbpkg;
                this.getItemCB = getItemCB;
                this.setImageCB = setImageCB;
                this.updateProgressCB = updateProgressCB;
                this.stopFetchingCB = stopFetchingCB;
                this.fetchingCompleteCB = fetchingCompleteCB;

            }

            public void Fetch()
            {
                updateProgress(true, "Please wait, loading thumbnails...", false, -1, false, -1);

                bool complete = false;
                try
                {
                    updateProgress(false, "", true, count, true, 0);
                    int freq = count / 100;
                    for (int i = 0; i < count; i++)
                    {
                        if (stopFetching) return;

                        Item objd = getItem(i);

                        if (stopFetching) return;

                        Image img = getImage(0x2e75c764, objd);
                        if (img != null) setImage(true, i, img);
                        else
                        {
                            TGI img32 = objd.tgi; img32.t = 0x0580A2B4; Item pngItem = new Item(thumbpkg, img32);
                            if (pngItem.ResourceIndexEntry != null) setImage(true, i, getImage(pngItem));
                        }

                        if (stopFetching) return;

                        img = getImage(0x2e75c765, objd);
                        if (img != null) setImage(false, i, img);
                        else
                        {
                            TGI img64 = objd.tgi; img64.t = 0x0580A2B5; Item pngItem = new Item(thumbpkg, img64);
                            if (pngItem.ResourceIndexEntry != null) setImage(false, i, getImage(pngItem));
                        }

                        if (stopFetching) return;

                        if (i % freq == 0)
                            updateProgress(true, String.Format("Please wait, loading thumbnails... {0}%", i * 100 / count), true, count, true, i);
                    }
                    complete = true;
                }
                catch (ThreadInterruptedException) { }
                finally
                {
                    updateProgress(true, "Finished loading thumnails", true, count, true, count);
                    fetchingComplete(complete);
                }
            }

            Item getItem(int i) { Thread.Sleep(0); return (Item)(!mainForm.IsHandleCreated ? null : mainForm.Invoke(getItemCB, new object[] { i, })); }

            void setImage(bool flag, int i, Image image) { imgcnt++; Thread.Sleep(0); if (mainForm.IsHandleCreated) mainForm.Invoke(setImageCB, new object[] { flag, i, image, }); }

            void updateProgress(bool changeText, string text, bool changeMax, int max, bool changeValue, int value)
            {
                Thread.Sleep(0);
                if (mainForm.IsHandleCreated) mainForm.Invoke(updateProgressCB, new object[] { changeText, text, changeMax, max, changeValue, value, });
            }

            bool stopFetching { get { Thread.Sleep(0); return !mainForm.IsHandleCreated || (bool)mainForm.Invoke(stopFetchingCB); } }

            void fetchingComplete(bool complete) { Thread.Sleep(0); if (mainForm.IsHandleCreated) mainForm.BeginInvoke(fetchingCompleteCB, new object[] { complete, }); }
        }
        #endregion

        #region Saving thread
        Thread saveThread;
        bool saving = false;
        void StartSaving()
        {
            this.SavingComplete += new EventHandler<BoolEventArgs>(MainForm_SavingComplete);

            SaveList sl = new SaveList(this, objectChooser.SelectedItems[0].Tag as Item, tgiLookup, FullBuild0Path, pkg, ThumbPkg, saveFileDialog1.FileName,
                updateProgress, stopSaving, OnSavingComplete);

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

        bool waitingForSavePackage;
        void MainForm_SavingComplete(object sender, BoolEventArgs e)
        {
            saving = false;
            tlpButtons.Enabled = true;
            while (saveThread != null && saveThread.IsAlive)
                saveThread.Join(100);
            saveThread = null;

            this.toolStripProgressBar1.Visible = false;
            this.toolStripStatusLabel1.Visible = false;

            if (waitingForSavePackage)
            {
                waitingForSavePackage = false;
                if (e.arg)
                    CopyableMessageBox.Show("OK", myName, CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Information);
                else
                    CopyableMessageBox.Show("Save not complete", myName, CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Warning);
                DisplayResources();
            }
        }

        public delegate bool stopSavingCallback();
        private bool stopSaving() { return !saving; }

        private event EventHandler<BoolEventArgs> SavingComplete;
        public delegate void savingCompleteCallback(bool complete);
        public void OnSavingComplete(bool complete) { if (SavingComplete != null) { SavingComplete(this, new BoolEventArgs(complete)); } }

        class SaveList
        {
            MainForm mainForm;
            Item objd;
            Dictionary<string, TGI> tgiList;
            string fullBuild0;
            IPackage pkgfb0;
            IPackage thumbpkg;
            string outputPackage;
            updateProgressCallback updateProgressCB;
            stopSavingCallback stopSavingCB;
            savingCompleteCallback savingCompleteCB;
            public SaveList(MainForm form, Item objd, Dictionary<string, TGI> tgiList, string fullBuild0, IPackage pkgfb0, IPackage thumbpkg, string outputPackage,
                updateProgressCallback updateProgressCB, stopSavingCallback stopSavingCB, savingCompleteCallback savingCompleteCB)
            {
                this.mainForm = form;
                this.objd = objd;
                this.tgiList = tgiList;
                this.fullBuild0 = fullBuild0;
                this.outputPackage = outputPackage;
                this.pkgfb0 = pkgfb0;
                this.thumbpkg = thumbpkg;
                this.updateProgressCB = updateProgressCB;
                this.stopSavingCB = stopSavingCB;
                this.savingCompleteCB = savingCompleteCB;
            }

            //Type: 0x00B2D882 resources are in Fullbuild2, everything else is in Fullbuild0, except thumbs
            //FullBuild0 is:
            //  ...\Gamedata\Shared\Packages\FullBuild0.package
            //Relative path to ALLThumbnails is:
            // .\..\..\..\Thumbnails\ALLThumbnails.package
            public void SavePackage()
            {
                updateProgress(true, "Creating output package...", false, -1, false, -1);
                IPackage target = s3pi.Package.Package.NewPackage(0);

                string folder = Path.GetDirectoryName(fullBuild0);

                updateProgress(true, "Opening FullBuild2...", false, -1, false, -1);
                IPackage pkgfb2 = s3pi.Package.Package.OpenPackage(0, Path.Combine(folder, "FullBuild2.package"));

                updateProgress(true, "Please wait...", false, -1, false, -1);

                bool complete = false;
                Item fb0nmap = new Item(pkgfb0, pkgfb0.Find(new String[] { "ResourceType" }, new TypedValue[] { new TypedValue(typeof(uint), (uint)0x0166038C), }));
                Item fb2nmap = new Item(pkgfb2, pkgfb2.Find(new String[] { "ResourceType" }, new TypedValue[] { new TypedValue(typeof(uint), (uint)0x0166038C), }));
                Item newnmap = NewResource(target, fb0nmap.tgi);
                IDictionary<ulong, string> fb0namemap = (IDictionary<ulong, string>)fb0nmap.Resource;
                IDictionary<ulong, string> fb2namemap = (IDictionary<ulong, string>)fb2nmap.Resource;
                IDictionary<ulong, string> thumbnamemap = new Dictionary<ulong, string>();//There isn't one
                IDictionary<ulong, string> newnamemap = (IDictionary<ulong, string>)newnmap.Resource;
                try
                {
                    int i = 0;
                    int freq = Math.Max(1, tgiList.Count / 50);
                    updateProgress(true, "Saving... 0%", true, tgiList.Count, true, i);
                    string lastSaved = "nothing yet";
                    foreach (var kvp in tgiList)
                    {
                        if (stopSaving) return;

                        IPackage pkg = kvp.Value.t == 0x00B2D882 ? pkgfb2 : kvp.Key.StartsWith("thumb[") ? thumbpkg : pkgfb0;
                        IDictionary<ulong, string> nm = kvp.Value.t == 0x00B2D882 ? fb2namemap : kvp.Key.StartsWith("thumb[") ? thumbnamemap : fb0namemap;

                        Item item = new Item(pkg, kvp.Value, true); // use default wrapper
                        if (item.ResourceIndexEntry != null)
                        {
                            if (!stopSaving) target.AddResource(kvp.Value.t, kvp.Value.g, kvp.Value.i, item.Resource.Stream, true);
                            lastSaved = kvp.Key;
                            if (nm.ContainsKey(kvp.Value.i) && !newnamemap.ContainsKey(kvp.Value.i))
                                if (!stopSaving) newnamemap.Add(kvp.Value.i, nm[kvp.Value.i]);
                        }

                        if (++i % freq == 0)
                            updateProgress(true, "Saved " + lastSaved + "... " + i * 100 / tgiList.Count + "%", true, tgiList.Count, true, i);
                    }
                    updateProgress(true, "", true, tgiList.Count, true, tgiList.Count);

                    updateProgress(true, "Finding string tables...", true, 0, true, 0);
                    ulong nameGUID = (ulong)((AHandlerElement)objd.Resource["CommonBlock"].Value)["NameGUID"].Value;
                    ulong descGUID = (ulong)((AHandlerElement)objd.Resource["CommonBlock"].Value)["DescGUID"].Value;
                    IList<IResourceIndexEntry> lrie = pkgfb0.FindAll(new String[] { "ResourceType" }, new TypedValue[] { new TypedValue(typeof(uint), (uint)0x220557DA), });
                    i = 0;
                    freq = 1;// Math.Max(1, lrie.Count / 10);

                    updateProgress(true, "Creating string tables extracts... 0%", true, lrie.Count, true, i);
                    foreach (IResourceIndexEntry rie in lrie)
                    {
                        if (stopSaving) return;

                        Item fb0stbl = new Item(pkgfb0, rie);
                        Item newstbl = NewResource(target, fb0stbl.tgi);
                        IDictionary<ulong, string> instbl = (IDictionary<ulong, string>)fb0stbl.Resource;
                        IDictionary<ulong, string> outstbl = (IDictionary<ulong, string>)newstbl.Resource;

                        if (fb0namemap.ContainsKey(rie.Instance) && !newnamemap.ContainsKey(rie.Instance))
                            if (!stopSaving) newnamemap.Add(rie.Instance, fb0namemap[rie.Instance]);

                        if (instbl.ContainsKey(nameGUID)) outstbl.Add(nameGUID, instbl[nameGUID]);
                        if (instbl.ContainsKey(descGUID)) outstbl.Add(descGUID, instbl[descGUID]);
                        if (!stopSaving) newstbl.Commit();

                        if (++i % freq == 0)
                            updateProgress(true, "Creating string tables extracts... " + i * 100 / lrie.Count + "%", true, lrie.Count, true, i);
                    }
                    updateProgress(true, "Committing new name map... ", true, 0, true, 0);
                    if (!stopSaving) newnmap.Commit();

                    updateProgress(true, "", true, 0, true, 0);
                    complete = true;
                }
                catch (ThreadInterruptedException) { }
                finally
                {
                    s3pi.Package.Package.ClosePackage(0, pkgfb2);

                    target.SaveAs(outputPackage);
                    s3pi.Package.Package.ClosePackage(0, target);

                    savingComplete(complete);
                }
            }

            Item NewResource(IPackage pkg, TGI tgi)
            {
                RIE rie = new RIE(pkg, pkg.AddResource(tgi.t, tgi.g, tgi.i, null, true));
                if (rie.rie == null) return null;
                return new Item(rie);
            }

            void updateProgress(bool changeText, string text, bool changeMax, int max, bool changeValue, int value)
            {
                Thread.Sleep(0);
                if (mainForm.IsHandleCreated) mainForm.Invoke(updateProgressCB, new object[] { changeText, text, changeMax, max, changeValue, value, });
            }

            bool stopSaving { get { Thread.Sleep(0); return !mainForm.IsHandleCreated || (bool)mainForm.Invoke(stopSavingCB); } }

            void savingComplete(bool complete)
            {
                Thread.Sleep(0);
                if (mainForm.IsHandleCreated)
                    mainForm.BeginInvoke(savingCompleteCB, new object[] { complete, });
            }
        }
        #endregion

        #region Common to threads
        public class BoolEventArgs : EventArgs
        {
            public bool arg;
            public BoolEventArgs(bool arg) { this.arg = arg; }
        }

        public delegate void updateProgressCallback(bool changeText, string text, bool changeMax, int max, bool changeValue, int value);
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
        #endregion


        #region Make Unique bits

        Dictionary<TGI, Item> tgiToItem;
        Dictionary<ulong, ulong> oldToNew;
        string uniqueObject = null;
        string UniqueObject
        {
            get
            {
                if (uniqueObject == null)
                {
                    StringInputDialog ond = new StringInputDialog();
                    ond.Caption = "Make Object Unique";
                    ond.Prompt = "Enter unique object identifier";
                    ond.Value = Path.GetFileNameWithoutExtension(openPackageDialog.FileName);
                    DialogResult dr = ond.ShowDialog();
                    if (dr == DialogResult.OK) uniqueObject = ond.Value;
                }
                return uniqueObject;
            }
        }

        ulong nameGUID, newNameGUID;
        ulong descGUID, newDescGUID;

        void StartFixing()
        {
            //oldToNew = new Dictionary<ulong, ulong>();
            //tgiToItem - TGIs we're interested in and the Item they refer to
            try
            {
                foreach (Item item in tgiToItem.Values)
                {
                    bool dirty = false;

                    if (item.ResourceIndexEntry.ResourceType == 0x319E4F1D) //OBJD needs more than just the TGIs done
                    {
                        AHandlerElement commonBlock = ((AHandlerElement)item.Resource["CommonBlock"].Value);

                        commonBlock["NameGUID"] = new TypedValue(typeof(ulong), newNameGUID);
                        commonBlock["DescGUID"] = new TypedValue(typeof(ulong), newDescGUID);
                        commonBlock["Name"] = new TypedValue(typeof(string), "CatalogObjects/Name:" + UniqueObject);
                        commonBlock["Desc"] = new TypedValue(typeof(string), "CatalogObjects/Description:" + UniqueObject);
                        commonBlock["Price"] = new TypedValue(typeof(float), float.Parse(tbPrice.Text));

                        if (!ckbCatlgDetails.Checked)
                            UpdateTGIsFromField((AResource)item.Resource);

                        dirty = true;
                    }
                    else if (item.ResourceIndexEntry.ResourceType == 0x0166038C)
                    {
                        IDictionary<ulong, string> nm = (IDictionary<ulong, string>)item.Resource;
                        foreach (ulong old in oldToNew.Keys)
                            if (nm.ContainsKey(old) && !nm.ContainsKey(oldToNew[old]))
                            {
                                nm.Add(oldToNew[old], nm[old]);
                                nm.Remove(old);
                                dirty = true;
                            }
                    }
                    else if (item.ResourceIndexEntry.ResourceType == 0x220557DA)
                    {
                        IDictionary<ulong, string> stbl = (IDictionary<ulong, string>)item.Resource;

                        string name = "";
                        if (stbl.ContainsKey(nameGUID)) { name = stbl[nameGUID]; stbl.Remove(nameGUID); }
                        if (ckbCopyToAll.Checked || item.tgi.i >> 56 == 0x00) name = tbCatlgName.Text;
                        stbl.Add(oldToNew.ContainsKey(nameGUID) ? oldToNew[nameGUID] : nameGUID, name);

                        string desc = "";
                        if (stbl.ContainsKey(descGUID)) { desc = stbl[descGUID]; stbl.Remove(descGUID); }
                        if (ckbCopyToAll.Checked || item.tgi.i >> 56 == 0x00) desc = tbCatlgDesc.Text;
                        stbl.Add(oldToNew.ContainsKey(descGUID) ? oldToNew[descGUID] : descGUID, desc);

                        dirty = true;
                    }
                    else
                    {
                        dirty = UpdateTGIsFromField((AResource)item.Resource);
                    }

                    if (dirty) item.Commit();

                }

                if (replacementForThumbs != null)
                {
                    IList<IResourceIndexEntry> lrie = pkg.FindAll(new string[] { "Instance", }, new TypedValue[] { new TypedValue(typeof(ulong), objdItem.tgi.i), });
                    foreach (IResourceIndexEntry rie in lrie)
                    {
                        switch (rie.ResourceType)
                        {
                            case 0x2E75C766:
                            case 0x0580A2B6:
                                setThumb(replacementForThumbs, rie, 128);
                                break;
                            case 0x2E75C765:
                            case 0x0580A2B5:
                                setThumb(replacementForThumbs, rie, 64);
                                break;
                            case 0x2E75C764:
                            case 0x0580A2B4:
                                setThumb(replacementForThumbs, rie, 32);
                                break;
                            default: break;
                        }
                    }
                }

                foreach (Item item in tgiToItem.Values)
                    if (item.tgi != new TGI(0, 0, 0) && oldToNew.ContainsKey(item.ResourceIndexEntry.Instance))
                        item.ResourceIndexEntry.Instance = oldToNew[item.ResourceIndexEntry.Instance];

                pkg.SavePackage();
            }
            finally
            {
                splitContainer1.Panel1.Controls.Clear();
                tlpButtons.Enabled = true;
                objectChooser.Items.Clear();
                ClearTabs();
                ClosePkg();
                subPage = SubPage.None;
                setButtons(Page.None, SubPage.None);
            }

            CopyableMessageBox.Show("OK", myName, CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Information);
        }

        bool gtAbort() { return false; }
        void setThumb(Image src, IResourceIndexEntry rie, int size)
        {
            Item item = new Item(pkg, rie);
            Image thumb;
            thumb = replacementForThumbs.GetThumbnailImage(size, size, gtAbort, System.IntPtr.Zero);
            thumb.Save(item.Resource.Stream, System.Drawing.Imaging.ImageFormat.Png);
            pkg.ReplaceResource(rie, item.Resource);
        }

        int numNewInstances = 0;
        ulong CreateInstance() { numNewInstances++; return FNV64.GetHash(numNewInstances.ToString("X8") + "_" + UniqueObject + "_" + DateTime.UtcNow.ToBinary().ToString("X16")); }

        private bool UpdateTGIsFromField(AApiVersionedFields field)
        {
            bool dirty = false;

            Type t = field.GetType();
            if (typeof(AResource.TGIBlock).IsAssignableFrom(t))
            {
                AResource.TGIBlock tgib = (AResource.TGIBlock)field;
                if (tgib != new TGI(0, 0, 0) && tgiToItem.ContainsKey(tgib) && oldToNew.ContainsKey(tgib.Instance)) { tgib.Instance = oldToNew[tgib.Instance]; dirty = true; }
            }
            else
            {
                if (typeof(IEnumerable).IsAssignableFrom(field.GetType()))
                    dirty = UpdateTGIsFromIEnumerable((IEnumerable)field) || dirty;
                dirty = UpdateTGIsFromAApiVersionedFields(field) || dirty;
            }

            return dirty;
        }
        private bool UpdateTGIsFromAApiVersionedFields(AApiVersionedFields field)
        {
            bool dirty = false;

            List<string> fields = field.ContentFields;
            foreach (string f in fields)
            {
                if ((new List<string>(new string[] { "Stream", "AsBytes", "Value", })).Contains(f)) continue;

                Type t = AApiVersionedFields.GetContentFieldTypes(0, field.GetType())[f];
                if (!t.IsClass || t.Equals(typeof(string)) || t.Equals(typeof(Boolset))) continue;
                if (t.IsArray && (!t.GetElementType().IsClass || t.GetElementType().Equals(typeof(string)))) continue;

                if (typeof(IEnumerable).IsAssignableFrom(t))
                    dirty = UpdateTGIsFromIEnumerable((IEnumerable)field[f].Value) || dirty;
                else if (typeof(AApiVersionedFields).IsAssignableFrom(t))
                    dirty = UpdateTGIsFromField((AApiVersionedFields)field[f].Value) || dirty;
            }

            return dirty;
        }
        private bool UpdateTGIsFromIEnumerable(IEnumerable list)
        {
            bool dirty = false;

            if (list != null)
                foreach (object o in list)
                    if (typeof(AApiVersionedFields).IsAssignableFrom(o.GetType()))
                        dirty = UpdateTGIsFromField((AApiVersionedFields)o) || dirty;
                    else if (typeof(IEnumerable).IsAssignableFrom(o.GetType()))
                        dirty = UpdateTGIsFromIEnumerable((IEnumerable)o) || dirty;

            return dirty;
        }

        #endregion


        #region Fetch resources
        private void Add(string key, TGI tgi)
        {
            if (resourceList.Count % 100 == 0) Application.DoEvents();
            tgiLookup.Add(key, tgi);

            TGIN tgin = new TGIN();
            tgin.ResType = tgi.t;
            tgin.ResGroup = tgi.g;
            tgin.ResInstance = tgi.i;
            tgin.ResName = key;
            resourceList.Add(tgin);
        }

        private void SlurpTGIsFromTGI(string key, TGI tgi) { Item item = new Item(pkg, tgi); if (item.ResourceIndexEntry != null) SlurpTGIsFromField(key, (AResource)item.Resource); }
        private void SlurpTGIsFromField(string key, AApiVersionedFields field)
        {
            Type t = field.GetType();
            if (typeof(AResource.TGIBlock).IsAssignableFrom(t))
            {
                Add(key, "" + (AResource.TGIBlock)field);
            }
            else
            {
                if (typeof(IEnumerable).IsAssignableFrom(t))
                    SlurpTGIsFromIEnumerable(key, (IEnumerable)field);
                SlurpTGIsFromAApiVersionedFields(key, field);
            }
        }
        private void SlurpTGIsFromAApiVersionedFields(string key, AApiVersionedFields field)
        {
            List<string> fields = field.ContentFields;
            foreach (string f in fields)
            {
                if ((new List<string>(new string[] { "Stream", "AsBytes", "Value", })).Contains(f)) continue;

                Type t = AApiVersionedFields.GetContentFieldTypes(0, field.GetType())[f];
                if (!t.IsClass || t.Equals(typeof(string)) || t.Equals(typeof(Boolset))) continue;
                if (t.IsArray && (!t.GetElementType().IsClass || t.GetElementType().Equals(typeof(string)))) continue;

                if (typeof(IEnumerable).IsAssignableFrom(t))
                    SlurpTGIsFromIEnumerable(key + "." + f, (IEnumerable)field[f].Value);
                else if (typeof(AApiVersionedFields).IsAssignableFrom(t))
                    SlurpTGIsFromField(key + "." + f, (AApiVersionedFields)field[f].Value);
            }
        }
        private void SlurpTGIsFromIEnumerable(string key, IEnumerable list)
        {
            if (list == null) return;
            int i = 0;
            foreach (object o in list)
            {
                if (typeof(AApiVersionedFields).IsAssignableFrom(o.GetType()))
                {
                    SlurpTGIsFromField(key + "[" + i + "]", (AApiVersionedFields)o);
                    i++;
                }
                else if (typeof(IEnumerable).IsAssignableFrom(o.GetType()))
                {
                    SlurpTGIsFromIEnumerable(key + "[" + i + "]", (IEnumerable)o);
                    i++;
                }
            }
        }

        //FullBuild0 is:
        //  ...\Gamedata\Shared\Packages\FullBuild0.package
        //Relative path to ALLThumbnails is:
        // .\..\..\..\Thumbnails\ALLThumbnails.package
        private void SlurpThumbnails(ulong instance)
        {
            string[] fields = new string[ckbDefault.Checked ? 2 : 1];
            TypedValue[] values = new TypedValue[ckbDefault.Checked ? 2 : 1];
            fields[0] = "Instance";
            values[0] = new TypedValue(typeof(ulong), instance);
            if (ckbDefault.Checked)
            {
                fields[1] = "ResourceGroup";
                values[1] = new TypedValue(typeof(uint), (uint)0);
            }

            SlurpKindred("thumb", mode == Mode.Clone ? ThumbPkg : pkg, fields, values);
        }

        private void SlurpVPXYKin(ulong instance)
        {
            SlurpKindred("vpxykin", pkg, new string[] { "ResourceType", "Instance" },
                new TypedValue[] { new TypedValue(typeof(uint), (uint)0x0333406C), new TypedValue(typeof(ulong), instance) });
        }

        private void SlurpKindred(string key, IPackage pkg, string[] fields, TypedValue[] values)
        {
            IList<IResourceIndexEntry> lrie = pkg.FindAll(fields, values);
            int i = 0;
            foreach (IResourceIndexEntry rie in lrie)
            {
                Add(key + "[" + i + "]", new TGI(rie));
                i++;
            }
        }
        #endregion


        #region Menu Bar
        private void menuBarWidget1_MBDropDownOpening(object sender, MenuBarWidget.MBDropDownOpeningEventArgs mn)
        {
            switch (mn.mn)
            {
                case MenuBarWidget.MD.MBF: break;
                case MenuBarWidget.MD.MBV: break;
                case MenuBarWidget.MD.MBS: break;
                case MenuBarWidget.MD.MBH: break;
                default: break;
            }
        }

        #region File menu
        private void menuBarWidget1_MBFile_Click(object sender, MenuBarWidget.MBClickEventArgs mn)
        {
            try
            {
                this.Enabled = false;
                Application.DoEvents();
                switch (mn.mn)
                {
                    case MenuBarWidget.MB.MBF_new: fileNew(); break;
                    case MenuBarWidget.MB.MBF_open: fileOpen(); break;
                    case MenuBarWidget.MB.MBF_exit: fileExit(); break;
                }
            }
            finally { this.Enabled = true; }
        }

        private void fileNew()
        {
            if (FullBuild0Path == null) return;
            mode = Mode.Clone;
            lastSubPage = SubPage.Step9;
            fileNewOpen(FullBuild0Path);
        }

        private void fileOpen()
        {
            openPackageDialog.InitialDirectory = ObjectCloner.Properties.Settings.Default.LastSaveFolder == null || ObjectCloner.Properties.Settings.Default.LastSaveFolder.Length == 0
                ? "" : ObjectCloner.Properties.Settings.Default.LastSaveFolder;
            openPackageDialog.FileName = "*.package";
            DialogResult dr = openPackageDialog.ShowDialog();
            if (dr != DialogResult.OK) return;
            ObjectCloner.Properties.Settings.Default.LastSaveFolder = Path.GetDirectoryName(openPackageDialog.FileName);
            try
            {
                IPackage target = s3pi.Package.Package.OpenPackage(0, openPackageDialog.FileName, true);
                s3pi.Package.Package.ClosePackage(0, target);
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                for (Exception inex = ex.InnerException; inex != null; inex = inex.InnerException) s += "\n" + inex.Message;
                CopyableMessageBox.Show("Could not open package " + openPackageDialog.FileName + "\n\n" + s, "File Open", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Error);
            }

            mode = Mode.Fix;
            lastSubPage = SubPage.Step10;

            fileNewOpen(openPackageDialog.FileName);
        }

        void fileNewOpen(string pkgName)
        {
            ClosePkg();
            try { pkg = s3pi.Package.Package.OpenPackage(0, pkgName, mode == Mode.Fix); }
            catch { pkg = null; }
            if (pkg == null)
            {
                CopyableMessageBox.Show(@"Failed to open """ + pkgName + @"""", "File open error", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Error);
                return;
            }

            menuBarWidget1.Enable(MenuBarWidget.MB.MBF_new, false);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBF_open, false);
            ckbDefault.Enabled = mode == Mode.Clone;
            ckbDefault.Checked = mode == Mode.Clone;
            ckbNoOBJD.Enabled = mode == Mode.Fix;
            ckbNoOBJD.Checked = false;
            ckbCatlgDetails.Enabled = mode == Mode.Fix;
            ckbCatlgDetails.Checked = false;

            DoWait("Please wait, loading object catalog...");
            Application.DoEvents();
            if (!haveLoaded || loadedPackage == null || loadedPackage != pkgName)
                StartLoading(pkgName);
            else
                DisplayObjectChooser();
        }

        private void fileExit()
        {
            Application.Exit();
        }
        #endregion

        #region View menu
        private void menuBarWidget1_MBView_Click(object sender, MenuBarWidget.MBClickEventArgs mn)
        {
            try
            {
                this.Enabled = false;
                Application.DoEvents();

                switch (mn.mn)
                {
                    case MenuBarWidget.MB.MBV_tiles: viewSetView(mn.mn, LItoIMG64); break;
                    case MenuBarWidget.MB.MBV_largeIcons: viewSetView(mn.mn, LItoIMG64); break;
                    case MenuBarWidget.MB.MBV_smallIcons: viewSetView(mn.mn, LItoIMG32); break;
                    case MenuBarWidget.MB.MBV_list: viewSetView(mn.mn, LItoIMG32); break;
                    case MenuBarWidget.MB.MBV_detailedList: viewSetView(mn.mn, LItoIMG32); break;
                    case MenuBarWidget.MB.MBV_icons: viewIcons(); break;
                }
            }
            finally { this.Enabled = true; }
        }

        private void viewSetView(MenuBarWidget.MB mn, Dictionary<int, int> imageMap)
        {
            if (menuBarWidget1.IsChecked(mn)) return;
            if (viewMap.ContainsKey(currentView)) menuBarWidget1.Checked(viewMap[currentView], false);
            menuBarWidget1.Checked(mn, true);

            if (menuBarWidget1.IsChecked(MenuBarWidget.MB.MBV_icons) && (LItoIMG64.Count > 0 || LItoIMG32.Count > 0))
                for (int i = 0; i < objectChooser.Items.Count; i++)
                    objectChooser.Items[i].ImageIndex = imageMap.ContainsKey(i) ? imageMap[i] : -1;

            ObjectCloner.Properties.Settings.Default.View = viewMapValues.IndexOf(mn);
            currentView = viewMapKeys[ObjectCloner.Properties.Settings.Default.View];
            objectChooser.View = currentView;
        }

        private void viewIcons()
        {
            menuBarWidget1.Checked(MenuBarWidget.MB.MBV_icons, !menuBarWidget1.IsChecked(MenuBarWidget.MB.MBV_icons));
            if (haveLoaded)
            {
                if (menuBarWidget1.IsChecked(MenuBarWidget.MB.MBV_icons))
                {
                    if (waitingForImages) return;

                    if (haveFetched)
                    {
                        if (LItoIMG64.Count > 0 || LItoIMG32.Count > 0)
                        {
                            Dictionary<int, int> imageMap = (currentView == View.Tile || currentView == View.LargeIcon) ? LItoIMG64 : LItoIMG32;
                            for (int i = 0; i < objectChooser.Items.Count; i++)
                                objectChooser.Items[i].ImageIndex = imageMap.ContainsKey(i) ? imageMap[i] : -1;
                        }
                    }
                    else
                    {
                        waitingForImages = true;
                        StartFetching();
                    }
                }
                else
                {
                    AbortFetching(false);
                    for (int i = 0; i < objectChooser.Items.Count; i++)
                        objectChooser.Items[i].ImageIndex = -1;
                }
            }
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
                    case MenuBarWidget.MB.MBS_sims3Folder: settingsSims3Folder(); break;
                    case MenuBarWidget.MB.MBS_userName: settingsUserName(); break;
                }
            }
            finally { this.Enabled = true; }
        }

        private void settingsSims3Folder()
        {
            folderBrowserDialog1.SelectedPath = ObjectCloner.Properties.Settings.Default.Sims3Folder == null || ObjectCloner.Properties.Settings.Default.Sims3Folder.Length == 0
                ? "" : ObjectCloner.Properties.Settings.Default.Sims3Folder;
            while (true)
            {
                DialogResult dr = folderBrowserDialog1.ShowDialog();
                if (dr != DialogResult.OK) return;

                string path = Path.Combine(folderBrowserDialog1.SelectedPath, @"GameData\Shared\Packages\FullBuild0.package");
                if (File.Exists(path)) break;

                if (CopyableMessageBox.Show(@"Cannot find ""GameData\Shared\Packages\FullBuild0.package""" + "\nin \"" + folderBrowserDialog1.SelectedPath + @"""",
                    "Select Sims3 Install Folder", CopyableMessageBoxButtons.OKCancel, CopyableMessageBoxIcon.Error) != 0) return;
            }
            ObjectCloner.Properties.Settings.Default.Sims3Folder = folderBrowserDialog1.SelectedPath;
        }

        private void settingsUserName()
        {
            StringInputDialog cn = new StringInputDialog();
            cn.Caption = "Creator name";
            cn.Prompt = "Your creator name will be used by default\nto create new object names";
            cn.Value = ObjectCloner.Properties.Settings.Default.CreatorName == null || ObjectCloner.Properties.Settings.Default.CreatorName.Length == 0
                ? Environment.UserName : ObjectCloner.Properties.Settings.Default.CreatorName;
            DialogResult dr = cn.ShowDialog();
            if (dr != DialogResult.OK) return;
            ObjectCloner.Properties.Settings.Default.CreatorName = cn.Value;
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
                    case MenuBarWidget.MB.MBH_warranty: helpWarranty(); break;
                    case MenuBarWidget.MB.MBH_licence: helpLicence(); break;
                }
            }
            finally { this.Enabled = true; }
        }

        private void helpContents()
        {
            if (File.Exists(Path.Combine(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "HelpFiles"), "Contents.htm")))
                Help.ShowHelp(this, "file:///" + Path.Combine(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "HelpFiles"), "Contents.htm"));
            else
                helpAbout();
        }

        private void helpAbout()
        {
            string copyright = "\n" +
                this.Text + "  Copyright (C) 2009  Peter L Jones\n" +
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
                , getVersion(typeof(MainForm), "s3oc")
                , getVersion(typeof(s3pi.Interfaces.AApiVersionedFields), "s3oc")
                ), this.Text);
        }

        private string getVersion(Type type, string p)
        {
            string s = getString(Path.Combine(Path.GetDirectoryName(type.Assembly.Location), p + "-Version.txt"));
            return s == null ? "Unknown" : s;
        }

        private string getString(string file)
        {
            if (!File.Exists(file)) return null;
            FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
            StreamReader t = new StreamReader(fs);
            return t.ReadLine();
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

        #endregion

        #region Buttons
        void setButtons(Page p, SubPage s)
        {
            Application.DoEvents();

            bool flag = p != Page.Resources || subPage < SubPage.Step2;
            ckbDefault.Enabled = mode == Mode.Clone && flag;
            ckbNoOBJD.Enabled = mode == Mode.Fix && !ckbCatlgDetails.Checked && flag;
            ckbCatlgDetails.Enabled = mode == Mode.Fix && flag;

            Color btnListBackColor = Color.FromKnownColor(KnownColor.Control);
            Color btnStartBackColor = Color.FromKnownColor(KnownColor.Control);
            Color btnNextBackColor = Color.FromKnownColor(KnownColor.Control);
            Color btnCommitBackColor = Color.FromKnownColor(KnownColor.Control);

            float price;
            btnSave.Enabled = p == Page.Resources && float.TryParse(tbPrice.Text, out price);
            switch (p)
            {
                case Page.None:
                    btnListBackColor = Color.FromKnownColor(KnownColor.ControlLightLight);
                    this.AcceptButton = btnList;
                    btnList.Enabled = haveLoaded;
                    break;
                case Page.ObjectChooser:
                    btnList.Enabled = false;
                    break;
                case Page.Resources:
                    btnList.Enabled = haveLoaded;
                    break;
            }

            if (objectChooser.SelectedItems.Count > 0)
            {
                btnStart.Enabled = true;
                if (s == SubPage.None)
                {
                    btnStartBackColor = Color.FromKnownColor(KnownColor.ControlLightLight);
                    this.AcceptButton = btnStart;
                    btnNext.Enabled = false;
                }
                else
                {
                    if (s != lastSubPage)
                    {
                        btnNextBackColor = Color.FromKnownColor(KnownColor.ControlLightLight);
                        this.AcceptButton = btnNext;
                        btnNext.Enabled = true;
                    }
                    else
                    {
                        btnNext.Enabled = false;
                        this.AcceptButton = btnSave;
                        btnCommitBackColor = Color.FromKnownColor(KnownColor.ControlLightLight);
                    }
                }
            }
            else
            {
                btnStart.Enabled = false;
                btnNext.Enabled = false;
            }

            btnList.BackColor = btnListBackColor;
            btnStart.BackColor = btnStartBackColor;
            btnNext.BackColor = btnNextBackColor;
            btnSave.BackColor = btnCommitBackColor;

            tlpButtons.Enabled = pkg != null;
        }

        private void btnList_Click(object sender, EventArgs e)
        {
            DisplayObjectChooser();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            tlpButtons.Enabled = false;
            resourceList.Clear();
            tgiLookup.Clear();
            if (mode == Mode.Fix)
            {
                uniqueObject = null;
                if (UniqueObject == null)
                {
                    DisplayObjectChooser();
                    return;
                }
            }

            subPage = SubPage.Step1;
            //DoWait("Please wait, retrieving Object...");
            DoWait("Please wait, performing operations...");
            Step1();
            DisplayResources();
        }
        //Bring in the OBJD the user selected
        void Step1()
        {
            clone = objectChooser.SelectedItems[0].SubItems[1].Text;
            Add("clone", clone);
            objdItem = new Item(pkg, clone);

            while (subPage != lastSubPage)
            {
                updateProgress(true, subPageText[(int)subPage], true, (int)lastSubPage, true, (int)subPage);
                Application.DoEvents();
                btnNext_Click(null, null);
            }

            this.toolStripProgressBar1.Visible = false;
            this.toolStripStatusLabel1.Visible = false;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            //tlpButtons.Enabled = false;

            subPage = (SubPage)((int)subPage) + 1;

            if (ckbCatlgDetails.Checked) subPage = lastSubPage;//Skip to end
            if (ckbDefault.Checked && subPage == SubPage.Step5) subPage = SubPage.Step6;//Skip it

            //DoWait("Coming next: " + subPageText[(int)subPage]);
            switch (subPage)
            {
                case SubPage.Step2: Step2(); break;
                case SubPage.Step3: Step3(); break;
                case SubPage.Step4: Step4(); break;
                case SubPage.Step5: Step5(); break;
                case SubPage.Step6: Step6(); break;
                case SubPage.Step7: Step7(); break;
                case SubPage.Step8: Step8(); break;
                case SubPage.Step9: Step9(); break;
                case SubPage.Step10: Step10(); break;
            }
            //DisplayResources();
        }
        //Bring in all the OBJK (or, on request, all resources in all the TGI blocks of the OBJD)
        void Step2()
        {
            uint index = (uint)objdItem.Resource["OBJKIndex"].Value;
            IList<AResource.TGIBlock> ltgi = (IList<AResource.TGIBlock>)objdItem.Resource["TGIBlocks"].Value;
            AResource.TGIBlock objkTGI = ltgi[(int)index];
            objkItem = new Item(pkg, objkTGI);

            if (ckbDefault.Checked)
                Add("clone_objk", objkTGI);
            else
                SlurpTGIsFromField("clone", (AResource)objdItem.Resource);
        }
        //Bring in the VPXY pointed to by the OBJK -- actually brings in all referenced resources
        void Step3()
        {
            if (objkItem.Resource == null) subPage = SubPage.LastInChain;//Skip past the chain
            else SlurpTGIsFromField("clone_objk", (AResource)objkItem.Resource);
        }
        //Try to get everything referenced from the VPXY (some may be not found but that's OK)
        void Step4()
        {
            int index = -1;
            IEnumerable keys = (IEnumerable)objkItem.Resource["Keys"].Value;
            foreach (AHandlerElement element in keys)
                if (((string)element["EntryName"].Value).Equals("modelKey")) { index = (int)element["CcIndex"].Value; break; }

            if (index == -1)
            {
                subPage = SubPage.LastInChain;//Skip past the chain
                return;
            }

            vpxy = tgiLookup["clone_objk.TGIBlocks[" + index + "]"];
            SlurpTGIsFromTGI("clone_vpxy", vpxy);
        }
        //Bring in Preset XML (same instance as VPXY)
        void Step5() { SlurpVPXYKin(vpxy.i); }
        //Bring in all the resources in the TGI blocks of each MODL (ref'd from VPXY)
        void Step6()
        {
            int i = 0;
            List<string> keys = new List<string>(tgiLookup.Keys);
            foreach (string key in keys)
                if (key.StartsWith("clone_vpxy.ChunkEntries[0].RCOLBlock.TGIBlocks["))
                {
                    RIE rie = new RIE(pkg, tgiLookup[key]);
                    if (rie.rie != null)
                    {
                        if (rie.rie.ResourceType != 0x01661233) continue;
                        SlurpTGIsFromTGI("clone_modl[" + i + "]", tgiLookup[key]);
                        i++;
                    }
                }
        }
        //Bring in all the resources in the TGI blocks of each MLOD (ref'd from MODL)
        void Step7()
        {
            int i = 0;
            List<string> keys = new List<string>(tgiLookup.Keys);
            foreach (string key in keys)
                if (key.StartsWith("clone_modl[") && (key.Contains("].Resources[") || key.Contains("].TGIBlocks[")))
                {
                    RIE rie = new RIE(pkg, tgiLookup[key]);
                    if (rie.rie != null)
                    {
                        if (rie.rie.ResourceType != 0x01D10F34) continue;
                        SlurpTGIsFromTGI("clone_mlod[" + i + "]", tgiLookup[key]);
                        i++;
                    }
                }
        }
        //Bring in all the resources in the TGI blocks of each TXTC (ref'd from MODL)
        void Step8()
        {
            int i = 0;
            List<string> keys = new List<string>(tgiLookup.Keys);
            foreach (string key in keys)
                if (key.StartsWith("clone_modl[") && (key.Contains("].Resources[") || key.Contains("].TGIBlocks[")))
                {
                    RIE rie = new RIE(pkg, tgiLookup[key]);
                    if (rie.rie != null)
                    {
                        if (rie.rie.ResourceType != 0x033A1435) continue;
                        SlurpTGIsFromTGI("clone_txtc[" + i + "]", tgiLookup[key]);
                        i++;
                    }
                }
        }

        //Bring in all resources from ...\The Sims 3\Thumbnails\ALLThumbnails.package that match the instance number of the OBJD
        void Step9() { SlurpThumbnails(clone.i); }

        //Fix integrity step
        void Step10()
        {
            //tgiLookup -- List of resources for cloned object, excludes name map and stbls

            if (!ckbCatlgDetails.Checked)//No point adding NMAP
            {
                IList<IResourceIndexEntry> lnmaprie = pkg.FindAll(new String[] { "ResourceType" }, new TypedValue[] { new TypedValue(typeof(uint), (uint)0x0166038C), });
                foreach (IResourceIndexEntry rie in lnmaprie)
                {
                    TGI tgi = new TGI(rie);
                    tgiLookup.Add("namemap", tgi);
                }
            }

            IList<IResourceIndexEntry> lstblrie = pkg.FindAll(new String[] { "ResourceType" }, new TypedValue[] { new TypedValue(typeof(uint), (uint)0x220557DA), });
            foreach (IResourceIndexEntry rie in lstblrie)
            {
                TGI tgi = new TGI(rie);
                tgiLookup.Add("lang_" + (tgi.i >> 56).ToString("X2"), tgi);
            }

            //tgiLookup -- List of resources for cloned object, *includes* name map and stbls now -- may include references to things not in package

            oldToNew = new Dictionary<ulong, ulong>();

            // Prevent OBJD and related resources getting renumbered
            if (ckbNoOBJD.Checked || ckbCatlgDetails.Checked)
                oldToNew.Add(objdItem.tgi.i, objdItem.tgi.i);

            //Prevent anything getting renumbered
            if (!ckbCatlgDetails.Checked)
            {
                ulong langInst = FNV64.GetHash("StringTable:" + UniqueObject) >> 8;
                foreach (TGI tgi in tgiLookup.Values)
                {
                    if (!oldToNew.ContainsKey(tgi.i))
                    {
                        if (tgi.t == 0x220557DA)//STBL
                            oldToNew.Add(tgi.i, tgi.i & 0xFF00000000000000 | langInst);
                        else
                            oldToNew.Add(tgi.i, CreateInstance());
                    }
                }
            }


            tgiToItem = new Dictionary<TGI, Item>();

            foreach (var kvp in tgiLookup)
            {
                if (kvp.Value == new TGI(0, 0, 0)) continue;
                Item item = new Item(pkg, kvp.Value);
                if (item.ResourceIndexEntry == null) continue; // Not a packed resource
                if (tgiToItem.ContainsKey(kvp.Value)) continue; // seen this TGI before
                tgiToItem.Add(kvp.Value, item);

                if (typeof(GenericRCOLResource).IsAssignableFrom(item.Resource.GetType()))
                {
                    // Add chunks we've identified an interest in and can find in the tgiLookup lookup
                    foreach (var chunk in (GenericRCOLResource.ChunkEntryList)item.Resource["ChunkEntries"].Value)
                    {
                        TGI tgi = new TGI(chunk.TGIBlock);
                        if (tgiLookup.ContainsValue(tgi) && !tgiToItem.ContainsKey(tgi)) tgiToItem.Add(tgi, item);
                    }
                }
            }

            //tgiToItem -- List of tgis for cloned object, *includes* name map and stbls now -- should only include references to things in package
            //rcols -- those tgiToItem values that refer to an RCOL resource


            nameGUID = (ulong)((AApiVersionedFields)objdItem.Resource["CommonBlock"].Value)["NameGUID"].Value;
            descGUID = (ulong)((AApiVersionedFields)objdItem.Resource["CommonBlock"].Value)["DescGUID"].Value;

            newNameGUID = FNV64.GetHash("CatalogObjects/Name:" + UniqueObject);
            newDescGUID = FNV64.GetHash("CatalogObjects/Description:" + UniqueObject);


            resourceList.Clear();
            if (!ckbCatlgDetails.Checked) foreach (var kvp in tgiToItem)
                {
                    string s = String.Format("Old: {0} --> New: {1}", "" + (TGIN)kvp.Key, "" + (TGIN)new TGI(kvp.Key.t, kvp.Key.g, oldToNew[kvp.Key.i]));
                    resourceList.Add(s);
                }

            resourceList.Add("Old NameGUID: 0x" + nameGUID.ToString("X16") + " --> New NameGUID: 0x" + newNameGUID.ToString("X16"));
            resourceList.Add("Old DescGUID: 0x" + descGUID.ToString("X16") + " --> New DescGUID: 0x" + newDescGUID.ToString("X16"));
            resourceList.Add("Old ObjName: \"" + ((AApiVersionedFields)objdItem.Resource["CommonBlock"].Value)["Name"] + "\" --> New Name: \"CatalogObjects/Name:" + UniqueObject + "\"");
            resourceList.Add("Old ObjDesc: \"" + ((AApiVersionedFields)objdItem.Resource["CommonBlock"].Value)["Desc"] + "\" --> New Desc: \"CatalogObjects/Description:" + UniqueObject + "\"");
            resourceList.Add("Old CatlgName: \"" + GetSTBLValue(nameGUID) + "\" --> New CatlgName: \"" + tbCatlgName.Text + "\"");
            resourceList.Add("Old CatlgDesc: \"" + GetSTBLValue(descGUID) + "\" --> New CatlgDesc: \"" + tbCatlgDesc.Text + "\"");
            resourceList.Add("Old Price: " + ((AApiVersionedFields)objdItem.Resource["CommonBlock"].Value)["Price"] + " --> New Price: " + float.Parse(tbPrice.Text));
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            try
            {
                if (mode == Mode.Clone)
                {
                    string prefix = CreatorName;
                    prefix = (prefix != null) ? prefix + "_" : "";
                    if (ObjectCloner.Properties.Settings.Default.LastSaveFolder != null)
                        saveFileDialog1.InitialDirectory = ObjectCloner.Properties.Settings.Default.LastSaveFolder;
                    saveFileDialog1.FileName = objectChooser.SelectedItems.Count > 0 ? prefix + objectChooser.SelectedItems[0].Text : "";
                    DialogResult dr = saveFileDialog1.ShowDialog();
                    if (dr != DialogResult.OK) return;
                    ObjectCloner.Properties.Settings.Default.LastSaveFolder = Path.GetDirectoryName(saveFileDialog1.FileName);

                    tlpButtons.Enabled = false;
                    DoWait("Please wait, creating your new package...");
                    waitingForSavePackage = true;
                    StartSaving();
                }
                else
                {
                    tlpButtons.Enabled = false;
                    DoWait("Please wait, updating your package...");
                    StartFixing();
                }
            }
            finally { ckbNoOBJD.Checked = false; this.Enabled = true; }
        }
        #endregion

        private void ckbCatlgDetails_CheckedChanged(object sender, EventArgs e)
        {
            if (!ckbCatlgDetails.Enabled) return;
            ckbNoOBJD.Enabled = !ckbCatlgDetails.Checked;
        }

        private void btnReplThumb_Click(object sender, EventArgs e)
        {
            openThumbnailDialog.FilterIndex = 1;
            openThumbnailDialog.FileName = "*.PNG";
            DialogResult dr = openThumbnailDialog.ShowDialog();
            if (dr != DialogResult.OK) return;
            try
            {
                replacementForThumbs = Image.FromFile(openThumbnailDialog.FileName, true);
                pictureBox1.Image = replacementForThumbs.GetThumbnailImage(128, 128, gtAbort, System.IntPtr.Zero);
            }
            catch (Exception ex)
            {
                CopyableMessageBox.Show("Could not read " + openThumbnailDialog.FileName + ":\n" + ex.Message, openThumbnailDialog.Title,
                    CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Error);
                replacementForThumbs = null;
            }
        }
    }
}
