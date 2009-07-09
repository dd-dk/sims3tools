﻿/***************************************************************************
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
using System.IO;
using System.Threading;
using System.Windows.Forms;
using s3pi.Interfaces;
using s3pi.Extensions;
using ObjectCloner.TopPanelComponents;

namespace ObjectCloner
{
    public partial class MainForm : Form
    {
        #region Static bits
        static string myName = "s3pi Object Cloner";
        static Dictionary<View, MenuBarWidget.MB> viewMap;
        static List<View> viewMapKeys;
        static List<MenuBarWidget.MB> viewMapValues;

        static string[] subPageText = new string[] {
            "You should never see this",
            "Step 1: Selected OBJD",
            "Step 2: OBJD-referenced resources",
            "Step 3: VPXY",
            "Step 4: VPXY-referenced resources",
            "Step 5: VPXY kindred resources (same instance)",
            "Step 6: MODL-referenced resources",
            "Step 7: MLOD-referenced resources",
            "Step 8: All thumbnails for OBJD",
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
            Choose,
            Listing,
        }
        enum SubPage
        {
            None = 0,
            Step1,//Bring in the OBJD the user selected
            Step2,//Bring in all the resources in all the TGI blocks of the OBJD
            Step3,//Bring in the VPXY pointed to by the OBJK
            Step4,//Try to get everything referenced from the VPXY (some may be not found but that's OK)
            Step5,//Bring in any resources with the same instance as the VPXY
            Step6,//Bring in all the resources in the TGI blocks of the MODL
            Step7,//Bring in all the resources in the TGI blocks of each MLOD (disregard duplicates)
            Step8,//Bring in all resources from ...\The Sims 3\Thumbnails\ALLThumbnails.package that match the instance number of the OBJD
            Last = 8,
        }

        SubPage subPage = SubPage.None;
        View currentView;
        IPackage pkg = null;
        IPackage thumbpkg = null;
        TGI clone;//0x319E4F1D
        RES resObjd;
        RES resObjk;
        TGI vpxy;

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
            AbortLoading();
            AbortFetching();
            AbortSaving();
            if (pkg != null) s3pi.Package.Package.ClosePackage(0, pkg);
            if (thumbpkg != null) s3pi.Package.Package.ClosePackage(0, thumbpkg);

            objectChooser.ObjectChooser_SaveSettings();

            ObjectCloner.Properties.Settings.Default.PersistentHeight = this.WindowState == FormWindowState.Normal ? this.Height : -1;
            ObjectCloner.Properties.Settings.Default.PersistentWidth = this.WindowState == FormWindowState.Normal ? this.Width : -1;
            ObjectCloner.Properties.Settings.Default.Splitter1Width = splitContainer1.SplitterDistance * 100 / this.Width;
            ObjectCloner.Properties.Settings.Default.ShowThumbs = menuBarWidget1.IsChecked(MenuBarWidget.MB.MBV_icons);
            ObjectCloner.Properties.Settings.Default.Save();
        }

        string packageFile;
        bool setPackageFile()
        {
            packageFile = ObjectCloner.Properties.Settings.Default.FullBuild0Location;
            openFileDialog1.FileName = packageFile;
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr != DialogResult.OK) return false;
            ObjectCloner.Properties.Settings.Default.FullBuild0Location = openFileDialog1.FileName;
            packageFile = openFileDialog1.FileName;
            return true;
        }

        void setThumbPackage()
        {
            if (thumbpkg == null)
            {
                string folder = Path.GetDirectoryName(packageFile);
                thumbpkg = s3pi.Package.Package.OpenPackage(0, Path.GetFullPath(Path.Combine(folder, @"../../../Thumbnails/ALLThumbnails.package")));
            }
        }

        #region TopPanelComponents
        private void DisplayObjectChooser()
        {
            waitingToDisplayResources = false;
            splitContainer1.Panel1.Controls.Clear();
            splitContainer1.Panel1.Controls.Add(objectChooser);
            objectChooser.Dock = DockStyle.Fill;
            setButtons(Page.Choose, subPage);
        }

        bool waitingToDisplayResources;
        private void DisplayResources()
        {
            resourceList.Page = subPageText[(int)subPage];
            splitContainer1.Panel1.Controls.Clear();
            splitContainer1.Panel1.Controls.Add(resourceList);
            resourceList.Dock = DockStyle.Fill;
            waitingToDisplayResources = false;
            setButtons(Page.Listing, subPage);
            resourceList.Focus();
        }

        private void DoWait()
        {
            splitContainer1.Panel1.Controls.Clear();
            splitContainer1.Panel1.Controls.Add(pleaseWait);
            pleaseWait.Dock = DockStyle.Fill;
            Application.DoEvents();
        }
        #endregion


        #region Loading thread
        Thread loadThread;
        bool haveLoaded = false;
        bool loading = false;
        void StartLoading()
        {
            this.LoadingComplete += new EventHandler<BoolEventArgs>(MainForm_LoadingComplete);

            FillListView flv = new FillListView(this, packageFile, createListViewItem, updateProgress, stopLoading, OnLoadingComplete);

            loadThread = new Thread(new ThreadStart(flv.LoadPackage));
            loading = true;
            loadThread.Start();
        }

        void AbortLoading()
        {
            waitingForSavePackage = false;
            if (!loading) MainForm_LoadingComplete(null, new BoolEventArgs(false));
            else loading = false;
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

                if (waitingToDisplayResources)
                {
                    if (menuBarWidget1.IsChecked(MenuBarWidget.MB.MBV_icons))
                    {
                        waitingForImages = true;
                        StartFetching();
                    }
                    
                    DisplayObjectChooser();
                }

                menuBarWidget1.Enable(MenuBarWidget.MD.MBV, true);
                menuBarWidget1_MBView_Click(null, new MenuBarWidget.MBClickEventArgs(viewMap[currentView]));
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
        public delegate void loadingCompleteCallback(IPackage pkg, bool complete);
        public void OnLoadingComplete(IPackage pkg, bool complete) { this.pkg = pkg; if (LoadingComplete != null) { LoadingComplete(this, new BoolEventArgs(complete)); } }

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

            setThumbPackage();
            FetchImages fi = new FetchImages(this, objectChooser.Items.Count, thumbpkg,
                getItem, setImage, updateProgress, stopFetching, OnFetchingComplete);

            fetchThread = new Thread(new ThreadStart(fi.Fetch));
            fetching = true;
            fetchThread.Start();
        }

        void AbortFetching()
        {
            if (!fetching) MainForm_FetchingComplete(null, new BoolEventArgs(false));
            else fetching = false;
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
                            TGI img32 = objd.tgi; img32.t = 0x0580A2B4; RES img32_res = new RES(new RIE(thumbpkg, img32));
                            if (((RIE)img32_res).rie != null) setImage(true, i, getImage(img32_res));
                        }

                        if (stopFetching) return;

                        img = getImage(0x2e75c765, objd);
                        if (img != null) setImage(false, i, img);
                        else
                        {
                            TGI img64 = objd.tgi; img64.t = 0x0580A2B5; RES img64_res = new RES(new RIE(thumbpkg, img64));
                            if (((RIE)img64_res).rie != null) setImage(false, i, getImage(img64_res));
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

            Image getImage(RES imgres) { return Image.FromStream(imgres.res.Stream); }

            Image getImage(uint type, Item objd)
            {
                ulong png = (ulong)((AHandlerElement)objd.Resource["CommonBlock"].Value)["PngInstance"].Value;
                if (png != 0)
                {
                    IPackage pkg = objd.res.pkg;

                    TGI tgi = new TGI(type, 0, png);
                    RES res = new RES(new RIE(pkg, tgi));
                    if (((RIE)res).rie != null) return getImage(res);
                }
                return null;
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

            setThumbPackage();
            SaveList sl = new SaveList(this, objectChooser.SelectedItems[0].Tag as Item, tgiLookup, packageFile, pkg, thumbpkg, saveFileDialog1.FileName,
                updateProgress, stopSaving, OnSavingComplete);

            saveThread = new Thread(new ThreadStart(sl.SavePackage));
            saving = true;
            saveThread.Start();
        }

        void AbortSaving()
        {
            if (!saving) MainForm_SavingComplete(null, new BoolEventArgs(false));
            else saving = false;
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
                Item fb0nmap = new Item(new RIE(pkgfb0, pkgfb0.Find(new String[] { "ResourceType" }, new TypedValue[] { new TypedValue(typeof(uint), (uint)0x0166038C), })));
                Item newnmap = NewResource(target, fb0nmap.tgi);
                IDictionary<ulong, string> fb0namemap = (IDictionary<ulong, string>)fb0nmap.Resource;
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
                        RES res = new RES(new RIE(pkg, kvp.Value), true); // use default wrapper
                        if (((RIE)res).rie != null)
                        {
                            if (!stopSaving) target.AddResource(kvp.Value.t, kvp.Value.g, kvp.Value.i, res.res.Stream, true);
                            lastSaved = kvp.Key;
                            if (fb0namemap.ContainsKey(kvp.Value.i) && !newnamemap.ContainsKey(kvp.Value.i))
                                if (!stopSaving) newnamemap.Add(kvp.Value.i, fb0namemap[kvp.Value.i]);
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

                        Item fb0stbl = new Item(new RIE(pkgfb0, rie));
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


        #region ObjectChooser
        private void objectChooser_SelectedIndexChanged(object sender, EventArgs e)
        {
            subPage = SubPage.None;
            resourceList.Clear();
            tgiLookup.Clear();
            if (objectChooser.SelectedItems.Count == 0) ClearTabs();
            else FillTabs(objectChooser.SelectedItems[0].Tag as Item);
            setButtons(Page.Choose, subPage);
        }

        void objectChooser_ItemActivate(object sender, EventArgs e)
        {
            btnClone_Click(sender, e);
        }
        #endregion

        #region Details view
        List<string> overviewTabFields = new List<string>();
        List<string> overviewTabCommonFields = new List<string>();
        List<string> flagsTabFields = new List<string>();
        void InitialiseTabs()
        {
            IResource res = s3pi.WrapperDealer.WrapperDealer.CreateNewResource(0, "0x319E4F1D");
            List<string> fields = AApiVersionedFields.GetContentFields(0, res.GetType());
            foreach (string field in fields)
            {
                if (field.StartsWith("Unknown")) continue;
                if (field.EndsWith("Flags")) flagsTabFields.Add(field);
                else if (field.Equals("Materials")) { }
                else if (field.Equals("MTDoors")) { }
                else if (field.Equals("TGIBlocks")) { }
                else if (field.Equals("CommonBlock"))
                {
                    List<string> commonfields = AApiVersionedFields.GetContentFields(0, res["CommonBlock"].Type);
                    foreach (string commonfield in commonfields)
                    {
                        if (commonfield.StartsWith("Unknown")) continue;
                        if (!commonfield.Equals("Value")) overviewTabCommonFields.Add(commonfield);
                    }
                }
                else if (!field.Equals("AsBytes") && !field.Equals("Stream") && !field.Equals("Value") &&
                    !field.Equals("Count") && !field.Equals("IsReadOnly") && !field.EndsWith("Reader")) overviewTabFields.Add(field);
            }
            InitialiseOverviewTab(res);
        }

        void InitialiseOverviewTab(IResource objd)
        {
            Dictionary<string, Type> types = AApiVersionedFields.GetContentFieldTypes(0, objd.GetType());
            foreach (string field in overviewTabFields)
            {
                CreateField(tlpOverviewMain, types[field], objd as AResource, field);
            }

            AApiVersionedFields common = objd["CommonBlock"].Value as AApiVersionedFields;
            types = AApiVersionedFields.GetContentFieldTypes(0, objd["CommonBlock"].Type);
            foreach (string field in overviewTabCommonFields)
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
            foreach (Control c in tlpOverviewMain.Controls)
                if (c is TextBox) ((TextBox)c).Text = "";
            foreach (Control c in tlpOverviewCommon.Controls)
                if (c is TextBox) ((TextBox)c).Text = "";
        }
        void FillTabs(Item objd)
        {
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
        #endregion

        #region Menu Bar
        private void menuBarWidget1_MBDropDownOpening(object sender, MenuBarWidget.MBDropDownOpeningEventArgs mn)
        {
            switch (mn.mn)
            {
                case MenuBarWidget.MD.MBF: break;
                case MenuBarWidget.MD.MBV: break;
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
                    case MenuBarWidget.MB.MBF_exit: fileExit(); break;
                }
            }
            finally { this.Enabled = true; }
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
                    AbortFetching();
                    for (int i = 0; i < objectChooser.Items.Count; i++)
                        objectChooser.Items[i].ImageIndex = -1;
                }
            }
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
                , getVersion(typeof(MainForm), "ObjectCloner")
                , getVersion(typeof(s3pi.Interfaces.AApiVersionedFields), "ObjectCloner")
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

            Color btnChooseBackColor = Color.FromKnownColor(KnownColor.Control);
            Color btnCloneBackColor = Color.FromKnownColor(KnownColor.Control);
            Color btnNextBackColor = Color.FromKnownColor(KnownColor.Control);
            Color btnSaveBackColor = Color.FromKnownColor(KnownColor.Control);

            switch (p)
            {
                case Page.None:
                    btnChooseBackColor = Color.FromKnownColor(KnownColor.ControlLightLight);
                    this.AcceptButton = btnChoose;
                    btnChoose.Enabled = true;
                    break;
                case Page.Choose:
                    btnChoose.Enabled = false;
                    break;
                case Page.Listing:
                    btnChoose.Enabled = true;
                    break;
            }

            if (objectChooser.SelectedItems.Count > 0)
            {
                btnClone.Enabled = true;
                if (s == SubPage.None)
                {
                    btnCloneBackColor = Color.FromKnownColor(KnownColor.ControlLightLight);
                    this.AcceptButton = btnClone;
                    btnNext.Enabled = false;
                }
                else
                {
                    if (s != SubPage.Last)
                    {
                        btnNextBackColor = Color.FromKnownColor(KnownColor.ControlLightLight);
                        this.AcceptButton = btnNext;
                        btnNext.Enabled = true;
                    }
                    else
                    {
                        btnNext.Enabled = false;
                        this.AcceptButton = btnSave;
                        btnSaveBackColor = Color.FromKnownColor(KnownColor.ControlLightLight);
                    }
                }
            }
            else
            {
                btnClone.Enabled = false;
                btnNext.Enabled = false;
            }
            tlpButtons.Enabled = true;

            btnChoose.BackColor = btnChooseBackColor;
            btnClone.BackColor = btnCloneBackColor;
            btnNext.BackColor = btnNextBackColor;
            btnSave.BackColor = btnSaveBackColor;
        }

        private void btnChoose_Click(object sender, EventArgs e)
        {
            if (!haveLoaded && !setPackageFile()) return;
            btnChoose.Enabled = false;

            DoWait();
            waitingToDisplayResources = true;
            Application.DoEvents();
            if (!haveLoaded)
                StartLoading();
            else
                DisplayObjectChooser();
        }

        private void btnClone_Click(object sender, EventArgs e)
        {
            tlpButtons.Enabled = false;
            waitingToDisplayResources = true;
            resourceList.Clear();
            tgiLookup.Clear();

            subPage = SubPage.Step1;
            DoWait();
            Step1();
            DisplayResources();
        }
        //Bring in the OBJD the user selected
        void Step1()
        {
            clone = objectChooser.SelectedItems[0].SubItems[1].Text;
            Add("clone", clone);
            resObjd = new RES(new RIE(pkg, clone));
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            tlpButtons.Enabled = false;

            DoWait();
            subPage = (SubPage)((int)subPage) + 1;
            switch (subPage)
            {
                case SubPage.Step2: Step2(); break;
                case SubPage.Step3: Step3(); break;
                case SubPage.Step4: Step4(); break;
                case SubPage.Step5: Step5(); break;
                case SubPage.Step6: Step6(); break;
                case SubPage.Step7: Step7(); break;
                case SubPage.Step8: Step8(); break;
            }
            DisplayResources();
        }
        //Bring in all the resources in all the TGI blocks of the OBJD
        void Step2()
        {
            SlurpTGIsFromField("clone", resObjd.res);
        }
        //Bring in the VPXY pointed to by the OBJK
        void Step3()
        {
            uint index = (uint)resObjd.res["OBJKIndex"].Value;
            TGI objk = tgiLookup["clone[TGIBlock][" + index + "]"];
            SlurpTGIsFromTGI("clone_objk", objk);
            resObjk = new RES(new RIE(pkg, objk));
        }
        //Try to get everything referenced from the VPXY (some may be not found but that's OK), including 
        void Step4()
        {
            int index = -1;
            TypedValue tv = resObjk.res["Keys"];
            foreach (AHandlerElement element in (System.Collections.IEnumerable)tv.Value)
            {
                if (((string)element["EntryName"].Value).Equals("modelKey") && ((byte)element["ControlCode"].Value).Equals(2))
                    index = (int)element["CcIndex"].Value;
            }
            if (index == -1)
            {
                subPage = SubPage.Step7;//can't do steps 5-7 without vpxy but don't crash
                return;
            }
            vpxy = tgiLookup["clone_objk.TGIBlocks[" + index + "]"];
            SlurpTGIsFromTGI("clone_vpxy", vpxy);
        }
        //Bring in any resources with the same instance as the VPXY
        void Step5() { SlurpVPXYKin(vpxy.i); }
        //Bring in all the resources in the TGI blocks of the MODL
        void Step6()
        {
            int i = 0;
            List<string> keys = new List<string>(tgiLookup.Keys);
            foreach (string key in keys)
                if (key.StartsWith("clone_vpxy[ChunkEntry][0].RCOLBlock.TGIBlocks["))
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
        //Bring in all the resources in the TGI blocks of each MLOD (disregard duplicates)
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

        //Bring in all resources from ...\The Sims 3\Thumbnails\ALLThumbnails.package that match the instance number of the OBJD
        void Step8()
        {
            SlurpThumbnails(clone.i);
        }

        //Bring in all resources from ...\The Sims 3\Thumbnails\ALLThumbnails.package that match the instance number of the OBJD
        private void btnSave_Click(object sender, EventArgs e)
        {
            waitingForSavePackage = true;
            this.Enabled = false;
            try
            {
                if (ObjectCloner.Properties.Settings.Default.LastSaveFolder != null)
                    saveFileDialog1.InitialDirectory = ObjectCloner.Properties.Settings.Default.LastSaveFolder;
                saveFileDialog1.FileName = objectChooser.SelectedItems[0].Text;
                DialogResult dr = saveFileDialog1.ShowDialog();
                if (dr != DialogResult.OK) return;
                ObjectCloner.Properties.Settings.Default.LastSaveFolder = Path.GetDirectoryName(saveFileDialog1.FileName);

                tlpButtons.Enabled = false;
                StartSaving();
            }
            finally { this.Enabled = true; }
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

        private void SlurpTGIsFromTGI(string key, TGI tgi) { RIE rie = new RIE(pkg, tgi); if (rie.rie != null) SlurpTGIsFromField(key, new RES(rie)); }
        private void SlurpTGIsFromField(string key, AApiVersionedFields field)
        {
            Type t = field.GetType();
            if (typeof(AResource.TGIBlock).IsAssignableFrom(t))
            {
                Add(key, "" + (AResource.TGIBlock)field);
            }
            else
            {
                if (typeof(System.Collections.IEnumerable).IsAssignableFrom(t))
                {
                    System.Collections.IEnumerable ienum = (System.Collections.IEnumerable)field;
                    int i = 0;
                    foreach (object o in ienum)
                    {
                        if (typeof(AApiVersionedFields).IsAssignableFrom(o.GetType()))
                        {
                            SlurpTGIsFromField(key + "[" + o.GetType().Name + "][" + i + "]", (AApiVersionedFields)o);
                            i++;
                        }
                    }
                }
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
                if (typeof(AResource.TGIBlockList).IsAssignableFrom(t))
                {
                    AResource.TGIBlockList list = (AResource.TGIBlockList)field[f].Value;
                    int i = 0;
                    foreach (AResource.TGIBlock value in list)
                    {
                        Add(key + "." + f + "[" + i + "]", "" + value);
                        i++;
                    }
                }
                else if (typeof(AResource.CountedTGIBlockList).IsAssignableFrom(t))
                {
                    AResource.CountedTGIBlockList list = (AResource.CountedTGIBlockList)field[f].Value;
                    int i = 0;
                    foreach (AResource.TGIBlock value in list)
                    {
                        Add(key + "." + f + "[" + i + "]", "" + value);
                        i++;
                    }
                }
                else if (typeof(AApiVersionedFields).IsAssignableFrom(t))
                    SlurpTGIsFromField(key + "." + f, (AApiVersionedFields)field[f].Value);
            }
        }

        //FullBuild0 is:
        //  ...\Gamedata\Shared\Packages\FullBuild0.package
        //Relative path to ALLThumbnails is:
        // .\..\..\..\Thumbnails\ALLThumbnails.package
        private void SlurpThumbnails(ulong instance) { setThumbPackage(); SlurpKindred("thumb", thumbpkg, instance); }
        private void SlurpVPXYKin(ulong instance) { setThumbPackage(); SlurpKindred("vpxykin", pkg, instance); }

        private void SlurpKindred(string key, IPackage pkg, ulong inst)
        {
            IList<IResourceIndexEntry> lrie = pkg.FindAll(new string[] { "Instance" }, new TypedValue[] { new TypedValue(typeof(ulong), inst), });
            int i = 0;
            foreach (IResourceIndexEntry rie in lrie)
            {
                Add(key + "[" + i + "]", new TGI(rie));
                i++;
            }
        }
        #endregion
    }
}
