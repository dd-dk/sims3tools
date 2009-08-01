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
        static bool disableCompression;
        static Dictionary<View, MenuBarWidget.MB> viewMap;
        static List<View> viewMapKeys;
        static List<MenuBarWidget.MB> viewMapValues;

        static string language_fmt = "Strings_{0}_{1:X2}{2:X62}";
        static string[] languages = new string[] {
            "ENG_US", "CHI_CN", "CHI_TW", "CZE_CZ",
            "DAN_DK", "DUT_NL", "FIN_FI", "FRE_FR",
            "GER_DE", "GRE_GR", "HUN_HU", "ITA_IT",
            "JAP_JP", "KOR_KR", "NOR_NO", "POL_PL",

            "POR_PT", "POR_BR", "RUS_RU", "SPA_ES",
            "SPA_MX", "SWE_SE", "THA_TH",
        };

        static Dictionary<string, List<string>> s3ocIni;
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

            s3ocIni = new Dictionary<string, List<string>>();
            string file = Path.Combine(Path.GetDirectoryName(typeof(MainForm).Assembly.Location), "s3oc.ini");
            if (File.Exists(file))
            {
                StreamReader sr = new StreamReader(file);
                string s;
                while ((s = sr.ReadLine()) != null)
                {
                    if (s.StartsWith("#") || s.StartsWith(";") || s.StartsWith("//") || s.Trim().Length == 0) continue;
                    string[] t = s.Split(new char[] { ':' }, 2);
                    if (t.Length != 2) continue;
                    string key = t[0].Trim().ToLower();
                    if (s3ocIni.ContainsKey(key)) continue;
                    s3ocIni.Add(key, new List<string>());
                    t = t[1].Split(',');
                    foreach (string u in t) s3ocIni[key].Add(u.Trim());
                }
                sr.Close();
            }
        }
        #endregion

        enum Page
        {
            None = 0,
            ObjectChooser,
            Resources,
        }

        enum Mode
        {
            None = 0,
            Clone,
            Fix,
        }
        Mode mode = Mode.None;

        View currentView;

        List<IPackage> objPkgs;
        List<IPackage> ddsPkgs;
        List<IPackage> tmbPkgs;

        Item selectedItem;
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
            SetStepText();

            this.Text = myName;
            MainForm_LoadFormSettings();
            disableCompression = !ckbCompress.Checked;

            InitialiseTabs(catalogTypes[4]);//Use the Proxy Product as it has pretty much nothing on it
            setButtons(Page.None, None);
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

            objectChooser.ObjectChooser_SaveSettings();

            ObjectCloner.Properties.Settings.Default.PersistentHeight = this.WindowState == FormWindowState.Normal ? this.Height : -1;
            ObjectCloner.Properties.Settings.Default.PersistentWidth = this.WindowState == FormWindowState.Normal ? this.Width : -1;
            ObjectCloner.Properties.Settings.Default.Splitter1Width = splitContainer1.SplitterDistance * 100 / this.Width;
            ObjectCloner.Properties.Settings.Default.ShowThumbs = menuBarWidget1.IsChecked(MenuBarWidget.MB.MBV_icons);
            ObjectCloner.Properties.Settings.Default.Save();
        }

        void ClosePkg()
        {
            haveLoaded = false;
            thumb = null;
            english = null;
            if (currentPackage != "")
            {
                if (objPkgs.Count > 0) s3pi.Package.Package.ClosePackage(0, objPkgs[0]);
                currentPackage = "";
                ddsPkgs = tmbPkgs = objPkgs = null;
            }
            else
            {
                if (objPkgs != null) foreach (List<IPackage> lpkg in new List<IPackage>[] { objPkgs, tmbPkgs, ddsPkgs, })
                        foreach (IPackage pkg in lpkg) s3pi.Package.Package.ClosePackage(0, pkg);
                ddsPkgs = tmbPkgs = objPkgs = null;
            }
        }


        string Sims3Folder
        {
            get
            {
                if (ObjectCloner.Properties.Settings.Default.Sims3Folder == null || ObjectCloner.Properties.Settings.Default.Sims3Folder.Length == 0
                    || !Directory.Exists(ObjectCloner.Properties.Settings.Default.Sims3Folder))
                    settingsSims3Folder();
                if (ObjectCloner.Properties.Settings.Default.Sims3Folder == null || ObjectCloner.Properties.Settings.Default.Sims3Folder.Length == 0
                    || !Directory.Exists(ObjectCloner.Properties.Settings.Default.Sims3Folder))
                    return null;
                return ObjectCloner.Properties.Settings.Default.Sims3Folder;
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


        #region Thumbs
        public class THUM
        {
            static Dictionary<uint, uint[]> thumTypes;
            static uint[] PNGTypes;
            static ushort[] thumSizes;
            static uint defType = 0x319E4F1D;
            static THUM()
            {
                thumTypes=new Dictionary<uint,uint[]>();
                thumTypes.Add(0x319E4F1D, new uint[] { 0x0580A2B4, 0x0580A2B5, 0x0580A2B6, }); //Catalog Object
                thumTypes.Add(0xCF9A4ACE, new uint[] { 0x00000000, 0x00000000, 0x00000000, }); //Modular Resource
                thumTypes.Add(0x0418FE2A, new uint[] { 0x2653E3C8, 0x2653E3C9, 0x2653E3CA, }); //Catalog Fence
                thumTypes.Add(0x049CA4CD, new uint[] { 0x5DE9DBA0, 0x5DE9DBA1, 0x5DE9DBA2, }); //Catalog Stairs
                thumTypes.Add(0x04AC5D93, thumTypes[0x319E4F1D]); //Catalog Proxy Product
                thumTypes.Add(0x04B30669, thumTypes[0x319E4F1D]); //Catalog Terrain Geometry Brush
                thumTypes.Add(0x04C58103, new uint[] { 0x2D4284F0, 0x2D4284F1, 0x2D4284F2, }); //Catalog Railing
                thumTypes.Add(0x04ED4BB2, new uint[] { 0x05B1B524, 0x05B1B525, 0x05B1B526, }); //Catalog Terrain Paint Brush
                thumTypes.Add(0x04F3CC01, new uint[] { 0x05B17698, 0x05B17699, 0x05B1769A, }); //Catalog Fireplace
                thumTypes.Add(0x060B390C, thumTypes[0x319E4F1D]); //Catalog Terrain Water Brush
                thumTypes.Add(0x316C78F2, thumTypes[0x319E4F1D]); //Catalog Foundation
                thumTypes.Add(0x515CA4CD, new uint[] { 0x0589DC44, 0x0589DC45, 0x0589DC46, }); //Catalog Wall/Floor Pattern
                thumTypes.Add(0x9151E6BC, new uint[] { 0x00000000, 0x00000000, 0x00000000, }); //Catalog Wall -- doesn't have any
                thumTypes.Add(0x91EDBD3E, thumTypes[0x319E4F1D]); //Catalog Roof Style
                thumTypes.Add(0xF1EDBD86, thumTypes[0x319E4F1D]); //Catalog Roof Pattern

                PNGTypes = new uint[] { 0x2E75C764, 0x2E75C765, 0x2E75C766, };
                thumSizes = new ushort[] { 32, 64, 128, };
            }
            public enum THUMSize : int
            {
                small = 0,
                medium,
                large,
                defSize = large,
            }
            List<IPackage> fb0Pkgs;//for PNGInstance
            List<IPackage> tmbPkgs;//for ALLThumbnails
            public THUM(List<IPackage> fb0Pkgs, List<IPackage> tmbPkgs) { this.fb0Pkgs = fb0Pkgs; this.tmbPkgs = tmbPkgs; }
            public Image this[ulong instance] { get { return this[instance, THUMSize.defSize, false]; } }
            public Image this[ulong instance, THUMSize size] { get { return this[instance, size, false]; } set { this[instance, size, false] = value; } }
            public Image this[ulong instance, bool isPNGInstance] { get { return this[instance, THUMSize.defSize, isPNGInstance]; } }
            public Image this[ulong instance, THUMSize size, bool isPNGInstance] { get { return this[defType, instance, size, isPNGInstance]; } set { this[defType, instance, size, isPNGInstance] = value; } }
            public Image this[uint type, ulong instance] { get { return this[type, instance, THUMSize.defSize, false]; } }
            public Image this[uint type, ulong instance, THUMSize size] { get { return this[type, instance, size, false]; } set { this[type, instance, size, false] = value; } }
            public Image this[uint type, ulong instance, THUMSize size, bool isPNGInstance]
            {
                get
                {
                    Item item = getItem(isPNGInstance ? fb0Pkgs : tmbPkgs, instance, (isPNGInstance ? PNGTypes : thumTypes[type])[(int)size]);
                    if (item != null && item.Resource != null)
                        return Image.FromStream(item.Resource.Stream);
                    return null;
                }
                set
                {
                    Item item = getItem(isPNGInstance ? fb0Pkgs : tmbPkgs, instance, (isPNGInstance ? PNGTypes : thumTypes[type])[(int)size]);
                    if (item == null || item.Resource == null)
                        throw new ArgumentException();

                    Image thumb;
                    thumb = value.GetThumbnailImage(thumSizes[(int)size], thumSizes[(int)size], gtAbort, System.IntPtr.Zero);
                    thumb.Save(item.Resource.Stream, System.Drawing.Imaging.ImageFormat.Png);
                    item.Commit();
                }
            }
            bool gtAbort() { return false; }

            public TGI getTGI(uint type, ulong instance, THUMSize size, bool isPNGInstance)
            {
                Item item = getItem(isPNGInstance ? fb0Pkgs : tmbPkgs, instance, (isPNGInstance ? PNGTypes : thumTypes[type])[(int)size]);
                return item == null ? new TGI() : item.tgi;
            }

            static Item getItem(List<IPackage> pkgs, ulong instance, uint type)
            {
                if (type == 0x00000000) return null;
                if (new List<uint>(thumTypes[0x515CA4CD]).Contains(type))
                {
                    foreach(IPackage pkg in pkgs)
                    {
                        IList<IResourceIndexEntry> lrie = pkg.FindAll(new string[] { "ResourceType", "Instance" }, new TypedValue[] {
                            new TypedValue(typeof(uint), type),
                            new TypedValue(typeof(ulong), instance),
                        });
                        foreach (IResourceIndexEntry rie in lrie)
                            if (rie.ResourceGroup > 0)
                                return new Item(new RIE(pkg, rie));
                    }
                }
                return new Item(pkgs, new TGI(type, 0, instance));
            }
        }
        THUM thumb;
        THUM Thumb
        {
            get
            {
                if (!haveLoaded) return null;
                if (thumb == null)
                    thumb = new THUM(objPkgs, tmbPkgs);
                return thumb;
            }
        }
        Image getImage(THUM.THUMSize size, Item item)
        {
            if (item.tgi.t == catalogTypes[1])
                return getImage(size, CatlgForMdlr(item));
            else
            {
                ulong png = 0;
                if (item.Resource != null) png = (ulong)((AHandlerElement)item.Resource["CommonBlock"].Value)["PngInstance"].Value;
                Image res = png == 0 ? null : Thumb[item.tgi.t, png, size, true];
                if (res != null) return res;
                return Thumb[item.tgi.t, item.tgi.i, size];
            }
        }
        TGI getImageTGI(THUM.THUMSize size, Item item)
        {
            if (item.tgi.t == catalogTypes[1])
            {
                return new TGI();
            }
            else
            {
                ulong png = 0;
                if (item.Resource != null) png = (ulong)((AHandlerElement)item.Resource["CommonBlock"].Value)["PngInstance"].Value;
                if (png != 0 && Thumb[item.tgi.t, png, size, true] != null)
                    return Thumb.getTGI(item.tgi.t, png, size, true);
                return Thumb.getTGI(item.tgi.t, item.tgi.i, size, false);
            }
        }
        #endregion


        class STBL
        {
            List<IDictionary<ulong, string>> stbls;
            Item latest;
            public STBL(IList<IPackage> stblPkgs) : this(stblPkgs, 0x00) { }
            public STBL(IList<IPackage> stblPkgs, byte langID)
            {
                stbls = new List<IDictionary<ulong, string>>();
                foreach (IPackage pkg in stblPkgs)
                {
                    IList<IResourceIndexEntry> lrie = pkg.FindAll(new String[] { "ResourceType", }, new TypedValue[] { new TypedValue(typeof(uint), (uint)0x220557DA), });
                    foreach (IResourceIndexEntry rie in lrie)
                    {
                        if (rie.Instance >> 56 == langID)
                        {
                            if (latest == null) latest = new Item(new RIE(pkg, rie));
                            stbls.Add(new Item(new RIE(pkg, rie)).Resource as IDictionary<ulong, string>);
                            break;
                        }
                    }
                }
            }
            public string this[ulong guid]
            {
                get
                {
                    foreach (IDictionary<ulong, string> stbl in stbls)
                        if (stbl.ContainsKey(guid))
                            return stbl[guid];
                    return null;
                }
            }
            public TGI tgi { get { return latest == null ? new TGI() : latest.tgi; } }
        }
        STBL english;
        STBL English
        {
            get
            {
                if (!haveLoaded) return null;
                if (english == null)
                    english = new STBL(objPkgs);
                return english;
            }
        }

        class NameMap
        {
            Item latest;
            List<IDictionary<ulong, string>> namemaps;
            public NameMap(IList<IPackage> nameMapPkgs)
            {
                namemaps = new List<IDictionary<ulong, string>>();
                foreach (IPackage pkg in nameMapPkgs)
                {
                    IList<IResourceIndexEntry> lrie = pkg.FindAll(new String[] { "ResourceType", }, new TypedValue[] { new TypedValue(typeof(uint), (uint)0x0166038C), });
                    if (lrie.Count > 0) latest = new Item(new RIE(pkg, lrie[0]));
                    foreach (IResourceIndexEntry rie in lrie)
                        namemaps.Add(new Item(new RIE(pkg, rie)).Resource as IDictionary<ulong, string>);
                }
            }
            public string this[ulong instance]
            {
                get
                {
                    foreach (IDictionary<ulong, string> namemap in namemaps)
                        if (namemap.ContainsKey(instance))
                            return namemap[instance];
                    return null;
                }
            }
            public TGI tgi { get { return latest == null ? new TGI() : latest.tgi; } }
        }
        NameMap nmap;
        NameMap NMap
        {
            get
            {
                if (!haveLoaded) return null;
                if (nmap == null)
                    nmap = new NameMap(objPkgs);
                return nmap;
            }
        }

        Item CatlgForMdlr(Item mdlr)
        {
            AResource.TGIBlock tgib = ((AResource.TGIBlockList)mdlr.Resource["TGIBlocks"].Value)[0];
            return new Item(objPkgs, new TGI(tgib));
        }


        #region LeftPanelComponents
        bool waitingToDisplayObjects;
        private void DisplayObjectChooser()
        {
            waitingToDisplayObjects = false;

            menuBarWidget1.Enable(MenuBarWidget.MB.MBF_new, mode == Mode.Fix); // don't need to re-do FB0
            menuBarWidget1.Enable(MenuBarWidget.MB.MBF_open, true); // do need to allow changing other packages
            setButtons(Page.ObjectChooser, step);

            splitContainer1.Panel1.Controls.Clear();
            splitContainer1.Panel1.Controls.Add(objectChooser);
            objectChooser.Dock = DockStyle.Fill;
            objectChooser.Focus();
        }

        private void DisplayResources()
        {
            setButtons(Page.Resources, step);

            resourceList.Page = StepText[step];
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
            resourceList.Clear();
            tgiLookup.Clear();
            replacementForThumbs = null;
            if (objectChooser.SelectedItems.Count == 0)
            {
                selectedItem = null;
                ClearTabs();
            }
            else
            {
                selectedItem = objectChooser.SelectedItems[0].Tag as Item;
                FillTabs(selectedItem);
            }
            stepList = null;
            step = None;
            setButtons(Page.ObjectChooser, step);
        }

        void objectChooser_ItemActivate(object sender, EventArgs e)
        {
            btnStart_Click(sender, e);
        }
        #endregion

        #region Tabs
        void InitialiseTabs(uint resourceType)
        {
            if (resourceType == catalogTypes[1]) resourceType = catalogTypes[0];//Modular Resources - display OBJD0

            IResource res = s3pi.WrapperDealer.WrapperDealer.CreateNewResource(0, "0x" + resourceType.ToString("X8"));
            InitialiseDetailsTab(resourceType, res);
            this.tabControl1.Controls.Remove(this.tpFlagsRoom);
            this.tabControl1.Controls.Remove(this.tpFlagsFunc);
            this.tabControl1.Controls.Remove(this.tpFlagsBuildEtc);
            if (resourceType == catalogTypes[0])
            {
                this.tabControl1.Controls.Add(this.tpFlagsRoom);
                this.tabControl1.Controls.Add(this.tpFlagsFunc);
                this.tabControl1.Controls.Add(this.tpFlagsBuildEtc);
                InitialiseFlagTabs(res);
                InitialiseOtherTab(res);
            }
        }
        Dictionary<string, string> detailsFieldMap;
        Dictionary<string, string> detailsFieldMapReverse;
        void InitialiseDetailsTab(uint resourceType, IResource catlg)
        {
            List<string> detailsTabFields = new List<string>();
            List<string> detailsTabCommonFields = new List<string>();
            List<string> fields = AApiVersionedFields.GetContentFields(0, catlg.GetType());
            detailsFieldMap = new Dictionary<string, string>();
            detailsFieldMapReverse = new Dictionary<string, string>();
            detailsFieldMap.Add("Product Status", "CommonBlock.BuildBuyProductStatusFlags");
            detailsFieldMapReverse.Add("CommonBlock.BuildBuyProductStatusFlags", "Product Status");

            while (tlpObjectDetail.RowCount > 2)
            {
                Control c = tlpObjectDetail.GetControlFromPosition(0, tlpObjectDetail.RowCount - 2);
                tlpObjectDetail.Controls.Remove(c);
                c = tlpObjectDetail.GetControlFromPosition(1, tlpObjectDetail.RowCount - 2);
                tlpObjectDetail.Controls.Remove(c);
                tlpObjectDetail.RowCount--;
            }

            while (tlpObjectCommon.RowCount > 3)
            {
                Control c = tlpObjectCommon.GetControlFromPosition(0, tlpObjectCommon.RowCount - 2);
                tlpObjectCommon.Controls.Remove(c);
                c = tlpObjectCommon.GetControlFromPosition(1, tlpObjectCommon.RowCount - 2);
                tlpObjectCommon.Controls.Remove(c);
                tlpObjectCommon.RowCount--;
            }

            foreach (string field in fields)
            {
                if (field.StartsWith("Unknown")) { }
                else if (field.EndsWith("Flags")) { }
                else if (field.Contains("Index")) { }
                else if (field.Equals("Materials")) { }
                else if (field.Equals("MTDoors")) { }
                else if (field.Equals("TGIBlocks")) { }
                else if (field.Equals("CommonBlock"))
                {
                    List<string> commonfields = AApiVersionedFields.GetContentFields(0, catlg["CommonBlock"].Type);
                    foreach (string commonfield in commonfields)
                    {
                        if (commonfield.StartsWith("Unknown")) { }
                        else if (commonfield.EndsWith("Flags") && commonfield != "BuildBuyProductStatusFlags") { }
                        else if (!commonfield.Equals("Value")) detailsTabCommonFields.Add(commonfield);
                    }
                }
                else if (!field.Equals("AsBytes") && !field.Equals("Stream") && !field.Equals("Value") &&
                    !field.Equals("Count") && !field.Equals("IsReadOnly") && !field.EndsWith("Reader")) detailsTabFields.Add(field);
            }

            Dictionary<string, Type> types = AApiVersionedFields.GetContentFieldTypes(0, catlg.GetType());
            foreach (string field in detailsTabFields)
            {
                CreateField(tlpObjectDetail, types[field], field);
            }

            AApiVersionedFields common = catlg["CommonBlock"].Value as AApiVersionedFields;
            types = AApiVersionedFields.GetContentFieldTypes(0, catlg["CommonBlock"].Type);
            foreach (string field in detailsTabCommonFields)
            {
                if (detailsFieldMap.ContainsValue("CommonBlock." + field))
                    CreateField(tlpObjectCommon, types[field], detailsFieldMapReverse["CommonBlock." + field], true);
                else
                    CreateField(tlpObjectCommon, types[field], field);
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
        List<flagField> flagFields;
        void InitialiseFlagTabs(IResource objd)
        {
            flagFields = new List<flagField>(new flagField[] {
                new flagField(tlpRoomSort, "RoomCategoryFlags", 32, 0),
                new flagField(tlpRoomSubLow, "RoomSubCategoryFlags", 32, 0),
                new flagField(tlpRoomSubHigh, "RoomSubCategoryFlags", 64, 32),
                new flagField(tlpFuncSort, "FunctionCategoryFlags", 32, 0),
                new flagField(tlpFuncSubLow, "FunctionSubCategoryFlags", 32, 0),
                new flagField(tlpFuncSubHigh, "FunctionSubCategoryFlags", 64, 32),
                new flagField(tlpBuildSort, "BuildCategoryFlags", 32, 0),
            });
            foreach (flagField ff in flagFields)
            {
                while (ff.tlp.RowCount > 2)
                {
                    for (int i = 0; i < ff.tlp.ColumnCount; i++)
                    {
                        Control c = ff.tlp.GetControlFromPosition(i, ff.tlp.RowCount - 2);
                        ff.tlp.Controls.Remove(c);
                    }
                    ff.tlp.RowCount--;
                }
            }
            foreach (flagField ff in flagFields)
            {
                Application.DoEvents();
                Type t = objd[ff.field].Type;
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
        Dictionary<string, string> otherFieldMap;
        void InitialiseOtherTab(IResource objd)
        {
            while (tlpOther.RowCount > 2)
            {
                Control c = tlpOther.GetControlFromPosition(0, tlpOther.RowCount - 2);
                tlpOther.Controls.Remove(c);
                c = tlpOther.GetControlFromPosition(1, tlpOther.RowCount - 2);
                tlpOther.Controls.Remove(c);
                tlpOther.RowCount--;
            }

            otherFieldMap = new Dictionary<string, string>();
            otherFieldMap.Add("Unknown8", "Unknown8");
            otherFieldMap.Add("Unknown9", "Unknown9");
            otherFieldMap.Add("Unknown10", "Unknown10");
            otherFieldMap.Add("Slot Placement", "SlotPlacementFlags");
            Dictionary<string, Type> types = AApiVersionedFields.GetContentFieldTypes(0, objd.GetType());
            foreach (string field in otherFieldMap.Keys)
                CreateField(tlpOther, types[otherFieldMap[field]], field, true);
        }

        static Dictionary<List<Type>, int> typeToLen = null;
        static void setTypeToLen()
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
        void CreateField(TableLayoutPanel target, Type t, string field) { CreateField(target, t, field, false); }
        void CreateField(TableLayoutPanel target, Type t, string field, bool validate)
        {
            if (typeof(Enum).IsAssignableFrom(t))
                t = Enum.GetUnderlyingType(t);

            if (typeToLen == null) setTypeToLen();

            foreach (List<Type> tt in typeToLen.Keys)
                if (tt.Contains(t))
                {
                    CreateField(target, field, typeToLen[tt], validate);
                    return;
                }

            CreateField(target, field, 30, validate);
        }

        void CreateField(TableLayoutPanel tlp, string name, int len, bool validate)
        {
            tlp.RowCount++;
            tlp.RowStyles.Insert(tlp.RowCount - 2, new RowStyle(SizeType.AutoSize));

            Label lb = new Label();
            lb.Anchor = AnchorStyles.Right;
            lb.AutoSize = true;
            lb.Name = "lb" + name;
            lb.TabIndex = tlp.RowCount * 2;
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
            tb.TabIndex = tlp.RowCount * 2 + 1;
            if (validate)
            {
                tb.Validating += new CancelEventHandler(tb_Validating);
                tb.Tag = validate;
            }
            tlp.Controls.Add(tb, 1, tlp.RowCount - 2);
        }

        void tb_Validating(object sender, CancelEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.Name == "tbPrice")
            {
                float res;
                e.Cancel = !Single.TryParse(tb.Text, out res);
            }
            else
                try
                {
                    ulong res = Convert.ToUInt64(tb.Text, tb.Text.StartsWith("0x") ? 16 : 10);
                }
                catch { e.Cancel = true; }
            if (e.Cancel) tb.SelectAll();
        }

        void CreateField(flagField ff, string name, CheckBox[] acbk, int i)
        {
            ff.tlp.RowStyles.Insert(i + 1, new RowStyle(SizeType.AutoSize));
            ff.tlp.SetCellPosition(acbk[i], new TableLayoutPanelCellPosition(0, i + 1));

            acbk[i].Anchor = AnchorStyles.Left;
            acbk[i].AutoSize = true;
            acbk[i].Enabled = false;
            acbk[i].Name = "cb" + name;
            acbk[i].Text = name;
            acbk[i].TabIndex = ff.tlp.RowCount;
        }

        void ClearTabs()
        {
            clearOverview();
            clearDetails();
            if (tabControl1.Contains(tpFlagsRoom))
            {
                clearFlags();
                clearOther();
            }
        }
        void clearOverview()
        {
            pictureBox1.Image = null;
            lbThumbTGI.Text = "";
            btnReplThumb.Enabled = false;
            tbObjName.Text = "";
            tbCatlgName.Text = "";
            tbObjDesc.Text = "";
            tbCatlgDesc.Text = "";
            tbCatlgName.Enabled = false;
            tbCatlgDesc.Enabled = false;
            ckbCopyToAll.Checked = false;
            ckbCopyToAll.Enabled = false;
            tbPrice.Text = "";
            tbPrice.ReadOnly = true;
        }
        void clearDetails()
        {
            foreach (Control c in tlpObjectDetail.Controls)
                if (c is TextBox) ((TextBox)c).Text = "";
            foreach (Control c in tlpObjectCommon.Controls)
                if (c is TextBox) ((TextBox)c).Text = "";
        }
        void clearFlags()
        {
            foreach (flagField ff in flagFields)
                foreach (Control c in ff.tlp.Controls)
                    if (c is CheckBox) ((CheckBox)c).Checked = ((CheckBox)c).Enabled = false;
        }
        void clearOther()
        {
            foreach (Control c in tlpOther.Controls)
                if (c is TextBox) { ((TextBox)c).Text = ""; ((TextBox)c).ReadOnly = true; }
        }

        void FillTabs(Item item)
        {
            if (item.Resource == null)
            {
                ClearTabs();
                return;
            }

            if (item.tgi.t != currentCatalogType)
            {
                currentCatalogType = item.tgi.t;
                InitialiseTabs(currentCatalogType);
            }

            Item catlg = (item.tgi.t == catalogTypes[1]) ? CatlgForMdlr(item) : item;

            fillOverview(catlg);
            fillDetails(catlg);
            if (catlg.tgi.t == catalogTypes[0])
            {
                fillFlags(catlg);
                fillOther(catlg);
            }
        }
        void fillOverview(Item objd)
        {
            pictureBox1.Image = getImage(THUM.THUMSize.large, objd);
            lbThumbTGI.Text = getImageTGI(THUM.THUMSize.large, objd);
            btnReplThumb.Enabled = mode == Mode.Fix;
            AApiVersionedFields common = objd.Resource["CommonBlock"].Value as AApiVersionedFields;
            tbObjName.Text = common["Name"].Value + "";
            tbObjDesc.Text = common["Desc"].Value + "";
            tbCatlgName.Text = English[(ulong)common["NameGUID"].Value];
            tbCatlgDesc.Text = English[(ulong)common["DescGUID"].Value];
            tbPrice.Text = common["Price"].Value + "";
            tbPrice.ReadOnly = mode == Mode.Clone;
            tbCatlgName.Enabled = tbCatlgDesc.Enabled = ckbCopyToAll.Enabled = mode == Mode.Fix;
        }
        void fillDetails(Item objd)
        {
            for (int i = 1; i < tlpObjectDetail.RowCount - 1; i++)
            {
                Label lb = (Label)tlpObjectDetail.GetControlFromPosition(0, i);
                TextBox tb = (TextBox)tlpObjectDetail.GetControlFromPosition(1, i);


                TypedValue tv;
                if (detailsFieldMap.ContainsKey(lb.Text))
                    tv = objd.Resource[detailsFieldMap[lb.Text]];
                else
                    tv = objd.Resource[lb.Text];

                if (typeof(Enum).IsAssignableFrom(tv.Type))
                {
                    string[] s = ("" + tv).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    tb.Text = s[0];
                }
                else
                    tb.Text = tv;

                tb.ReadOnly = !(mode == Mode.Fix && tb.Tag != null);
            }
            for (int i = 2; i < tlpObjectCommon.RowCount - 1; i++)
            {
                Label lb = (Label)tlpObjectCommon.GetControlFromPosition(0, i);
                TextBox tb = (TextBox)tlpObjectCommon.GetControlFromPosition(1, i);

                TypedValue tv;
                if (detailsFieldMap.ContainsKey(lb.Text))
                    tv = objd.Resource[detailsFieldMap[lb.Text]];
                else
                    tv = ((AApiVersionedFields)objd.Resource["CommonBlock"].Value)[lb.Text];

                if (typeof(Enum).IsAssignableFrom(tv.Type))
                {
                    string[] s = ("" + tv).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    tb.Text = s[0];
                }
                else
                    tb.Text = tv;

                tb.ReadOnly = !(mode == Mode.Fix && tb.Tag != null);
            }
        }
        void fillFlags(Item objd)
        {
            foreach (flagField ff in flagFields)
            {
                ulong field = getFlags(objd.Resource as AResource, ff.field);
                for (int i = 1; i < ff.tlp.RowCount - 1; i++)
                {
                    ulong value = (ulong)Math.Pow(2, ff.offset + i - 1);
                    CheckBox cb = (CheckBox)ff.tlp.GetControlFromPosition(0, i);
                    cb.Checked = (field & value) != 0;
                    cb.Enabled = mode == Mode.Fix;
                }
            }
        }
        void fillOther(Item objd)
        {
            for (int i = 1; i < tlpOther.RowCount - 1; i++)
            {
                Label lb = (Label)tlpOther.GetControlFromPosition(0, i);
                TextBox tb = (TextBox)tlpOther.GetControlFromPosition(1, i);

                TypedValue tv = objd.Resource[otherFieldMap[lb.Text]];
                if (typeof(Enum).IsAssignableFrom(tv.Type))
                {
                    string[] s = ("" + tv).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    tb.Text = s[0];
                }
                else
                    tb.Text = tv;

                tb.ReadOnly = !(mode == Mode.Fix && tb.Tag != null);
            }
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
        void setFlags(AApiVersionedFields owner, string field, ulong value)
        {
            Type t = AApiVersionedFields.GetContentFieldTypes(0, owner.GetType())[field];
            TypedValue tv = new TypedValue(t, Enum.ToObject(t, value));
            owner[field] = tv;
        }
        #endregion


        #region Loading thread
        Thread loadThread;
        bool haveLoaded = false;
        bool loading = false;
        void StartLoading(uint resourceType)
        {
            if (haveLoaded) return;
            if (loading) { AbortLoading(false); }

            waitingToDisplayObjects = true;
            objectChooser.Items.Clear();

            this.LoadingComplete += new EventHandler<BoolEventArgs>(MainForm_LoadingComplete);

            FillListView flv = new FillListView(this, objPkgs, ddsPkgs, tmbPkgs, resourceType
                , createListViewItem, updateProgress, stopLoading, OnLoadingComplete);

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
        void createListViewItem(Item item)
        {
            ListViewItem lvi = new ListViewItem();
            if (item.Resource != null)
            {
                string objdtag;
                if (item.tgi.t == catalogTypes[1]) objdtag = ((AApiVersionedFields)CatlgForMdlr(item).Resource["CommonBlock"].Value)["Name"];
                else objdtag = ((AApiVersionedFields)item.Resource["CommonBlock"].Value)["Name"];
                lvi.Text = (objdtag.IndexOf(':') < 0) ? objdtag : objdtag.Substring(objdtag.LastIndexOf(':') + 1);
            }
            else
            {
                string s = item.Exception.Message;
                for (Exception ex = item.Exception.InnerException; ex != null; ex = ex.InnerException) s += "  " + ex.Message;
                lvi.Text = s;
            }
            lvi.SubItems.AddRange(new string[] { item.tgi, });
            lvi.Tag = item;
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

            FetchImages fi = new FetchImages(this, objectChooser.Items.Count,
                getItem, getImage, setImage, updateProgress, stopFetching, OnFetchingComplete);

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

        public delegate Image getImageCallback(THUM.THUMSize size, Item item);

        public delegate void setImageCallback(bool isSmall, int i, Image image);
        private void setImage(bool isSmall, int i, Image image)
        {
            if (isSmall)
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
            getItemCallback getItemCB;
            getImageCallback getImageCB;
            setImageCallback setImageCB;
            updateProgressCallback updateProgressCB;
            stopFetchingCallback stopFetchingCB;
            fetchingCompleteCallback fetchingCompleteCB;

            int imgcnt = 0;

            public FetchImages(MainForm form, int count,
                getItemCallback getItemCB, getImageCallback getImageCB, setImageCallback setImageCB,
                updateProgressCallback updateProgressCB, stopFetchingCallback stopFetchingCB, fetchingCompleteCallback fetchingCompleteCB)
            {
                this.mainForm = form;
                this.count = count;
                this.getItemCB = getItemCB;
                this.getImageCB = getImageCB;
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
                    int freq = Math.Max(1, count / 100);
                    for (int i = 0; i < count; i++)
                    {
                        if (stopFetching) return;

                        Item item = getItem(i);

                        if (stopFetching) return;

                        Image img = getImage(THUM.THUMSize.small, item);
                        if (img != null) setImage(true, i, img);

                        if (stopFetching) return;

                        img = getImage(THUM.THUMSize.medium, item);
                        if (img != null) setImage(false, i, img);

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

            Image getImage(THUM.THUMSize size, Item item) { Thread.Sleep(0); return (Image)(!mainForm.IsHandleCreated ? null : mainForm.Invoke(getImageCB, new object[] { size, item, })); }

            void setImage(bool isSmall, int i, Image image) { imgcnt++; Thread.Sleep(0); if (mainForm.IsHandleCreated) mainForm.Invoke(setImageCB, new object[] { isSmall, i, image, }); }

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

            SaveList sl = new SaveList(this, objectChooser.SelectedItems[0].Tag as Item, tgiLookup, objPkgs, ddsPkgs, tmbPkgs,
                saveFileDialog1.FileName, ckbPadSTBLs.Checked,
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
            List<IPackage> objPkgs;
            List<IPackage> ddsPkgs;
            List<IPackage> tmbPkgs;
            string outputPackage;
            bool padSTBLs;
            updateProgressCallback updateProgressCB;
            stopSavingCallback stopSavingCB;
            savingCompleteCallback savingCompleteCB;
            public SaveList(MainForm form, Item objd, Dictionary<string, TGI> tgiList, List<IPackage> objPkgs, List<IPackage> ddsPkgs, List<IPackage> tmbPkgs,
                string outputPackage, bool padSTBLs,
                updateProgressCallback updateProgressCB, stopSavingCallback stopSavingCB, savingCompleteCallback savingCompleteCB)
            {
                this.mainForm = form;
                this.objd = objd;
                this.tgiList = tgiList;
                this.objPkgs = objPkgs;
                this.ddsPkgs = ddsPkgs;
                this.tmbPkgs = tmbPkgs;
                this.outputPackage = outputPackage;
                this.padSTBLs = padSTBLs;
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

                updateProgress(true, "Please wait...", false, -1, false, -1);

                bool complete = false;
                NameMap fb0nm = new NameMap(objPkgs);
                NameMap fb2nm = new NameMap(ddsPkgs);
                Item newnmap = NewResource(target, fb0nm.tgi);
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

                        List<IPackage> lpkg = kvp.Value.t == 0x00B2D882 ? ddsPkgs : kvp.Key.EndsWith("Thumb") ? tmbPkgs : objPkgs;
                        NameMap nm = kvp.Value.t == 0x00B2D882 ? fb2nm : fb0nm;

                        Item item = new Item(new RIE(lpkg, kvp.Value), true); // use default wrapper
                        if (item.ResourceIndexEntry != null)
                        {
                            if (!stopSaving) target.AddResource(kvp.Value.t, kvp.Value.g, kvp.Value.i, item.Resource.Stream, true);
                            lastSaved = kvp.Key;
                            if (!newnamemap.ContainsKey(kvp.Value.i))
                            {
                                string name = nm[kvp.Value.i];
                                if (name != null)
                                    if (!stopSaving) newnamemap.Add(kvp.Value.i, name);
                            }
                        }

                        if (++i % freq == 0)
                            updateProgress(true, "Saved " + lastSaved + "... " + i * 100 / tgiList.Count + "%", true, tgiList.Count, true, i);
                    }
                    updateProgress(true, "", true, tgiList.Count, true, tgiList.Count);

                    updateProgress(true, "Finding string tables...", true, 0, true, 0);
                    ulong nameGUID = (ulong)((AHandlerElement)objd.Resource["CommonBlock"].Value)["NameGUID"].Value;
                    ulong descGUID = (ulong)((AHandlerElement)objd.Resource["CommonBlock"].Value)["DescGUID"].Value;

                    i = 0;
                    freq = 1;// Math.Max(1, lrie.Count / 10);

                    updateProgress(true, "Creating string tables extracts... 0%", true, 0x17, true, i);
                    STBL english = new STBL(objPkgs);
                    while (i < 0x17)
                    {
                        if (stopSaving) return;

                        STBL lang = new STBL(objPkgs, (byte)i);
                        string name = lang[nameGUID];
                        string desc = lang[descGUID];

                        Item newstbl;
                        if (name == null && desc == null)
                        {
                            if (!padSTBLs || english == null) goto skip;
                            TGI newTGI = new TGI(english.tgi.t, english.tgi.g, english.tgi.i | ((ulong)i << 56));
                            newstbl = NewResource(target, newTGI);
                            name = english[nameGUID];
                            desc = english[descGUID];
                        }
                        else
                        {
                            newstbl = NewResource(target, lang.tgi);
                        }

                        IDictionary<ulong, string> outstbl = (IDictionary<ulong, string>)newstbl.Resource;
                        if (name != null) outstbl.Add(nameGUID, name);
                        if (desc != null) outstbl.Add(descGUID, desc);
                        if (!stopSaving) newstbl.Commit();

                        if (!newnamemap.ContainsKey(lang.tgi.i))
                        {
                            string nmname = fb0nm[lang.tgi.i];
                            if (nmname != null) { if (!stopSaving) newnamemap.Add(lang.tgi.i, nmname); }
                            else { if (!stopSaving) newnamemap.Add(lang.tgi.i, String.Format(language_fmt, languages[i], i, english.tgi.i)); }
                        }

                    skip:
                        if (++i % freq == 0)
                            updateProgress(true, "Creating string tables extracts... " + i * 100 / 0x17 + "%", true, 0x17, true, i);
                    }

                    updateProgress(true, "Committing new name map... ", true, 0, true, 0);
                    if (!stopSaving) newnmap.Commit();

                    updateProgress(true, "", true, 0, true, 0);
                    complete = true;
                }
                catch (ThreadInterruptedException) { }
                finally
                {
                    if (!disableCompression)
                        foreach (IResourceIndexEntry ie in target.GetResourceList) ie.Compressed = 0xffff;
                    target.SaveAs(outputPackage);
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

                    if (item.tgi == selectedItem.tgi) // .ResourceIndexEntry.ResourceType == 0x319E4F1D) //OBJD needs more than just the TGIs done
                    {
                        AHandlerElement commonBlock = ((AHandlerElement)item.Resource["CommonBlock"].Value);

                        commonBlock["NameGUID"] = new TypedValue(typeof(ulong), newNameGUID);
                        commonBlock["DescGUID"] = new TypedValue(typeof(ulong), newDescGUID);
                        commonBlock["Name"] = new TypedValue(typeof(string), "CatalogObjects/Name:" + UniqueObject);
                        commonBlock["Desc"] = new TypedValue(typeof(string), "CatalogObjects/Description:" + UniqueObject);
                        commonBlock["Price"] = new TypedValue(typeof(float), float.Parse(tbPrice.Text));

                        ulong PngInstance = (ulong)commonBlock["PngInstance"].Value;
                        if (oldToNew.ContainsKey(PngInstance))
                            commonBlock["PngInstance"] = new TypedValue(typeof(ulong), oldToNew[PngInstance]);

                        for (int i = 2; i < tlpObjectCommon.RowCount - 1; i++)
                        {
                            Label lb = (Label)tlpObjectCommon.GetControlFromPosition(0, i);
                            TextBox tb = (TextBox)tlpObjectCommon.GetControlFromPosition(1, i);
                            if (tb.Tag == null) continue;

                            TypedValue tvOld = item.Resource[detailsFieldMap[lb.Text]];

                            ulong u = Convert.ToUInt64(tb.Text, tb.Text.StartsWith("0x") ? 16 : 10);
                            object val;
                            if (typeof(Enum).IsAssignableFrom(tvOld.Type))
                                val = Enum.ToObject(tvOld.Type, u);
                            else
                                val = Convert.ChangeType(u, tvOld.Type);

                            item.Resource[detailsFieldMap[lb.Text]] = new TypedValue(tvOld.Type, val);
                        }

                        if (item.tgi.t == catalogTypes[0])
                        {
                            foreach (flagField ff in flagFields)
                            {
                                ulong old = getFlags(item.Resource as AResource, ff.field);
                                ulong mask = (ulong)0xFFFFFFFF << ff.offset;
                                ulong res = getFlags(ff);
                                res |= (ulong)(old & ~mask);
                                setFlags(item.Resource as AResource, ff.field, res);
                            }

                            for (int i = 1; i < tlpOther.RowCount - 1; i++)
                            {
                                Label lb = (Label)tlpOther.GetControlFromPosition(0, i);
                                TextBox tb = (TextBox)tlpOther.GetControlFromPosition(1, i);
                                if (tb.Tag == null) continue;

                                TypedValue tvOld = item.Resource[otherFieldMap[lb.Text]];

                                ulong u = Convert.ToUInt64(tb.Text, tb.Text.StartsWith("0x") ? 16 : 10);
                                object val;
                                if (typeof(Enum).IsAssignableFrom(tvOld.Type))
                                    val = Enum.ToObject(tvOld.Type, u);
                                else
                                    val = Convert.ChangeType(u, tvOld.Type);

                                item.Resource[otherFieldMap[lb.Text]] = new TypedValue(tvOld.Type, val);
                            }
                        }

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
                        stbl.Add(newNameGUID, name);

                        string desc = "";
                        if (stbl.ContainsKey(descGUID)) { desc = stbl[descGUID]; stbl.Remove(descGUID); }
                        if (ckbCopyToAll.Checked || item.tgi.i >> 56 == 0x00) desc = tbCatlgDesc.Text;
                        stbl.Add(newDescGUID, desc);

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
                    foreach (THUM.THUMSize size in Enum.GetValues(typeof(THUM.THUMSize)))
                    {
                        if (Thumb[selectedItem.tgi.t, selectedItem.tgi.i, size, true] != null)
                            Thumb[selectedItem.tgi.t, selectedItem.tgi.i, size, true] = replacementForThumbs;
                        else if (Thumb[selectedItem.tgi.t, selectedItem.tgi.i, size] != null)
                            Thumb[selectedItem.tgi.t, selectedItem.tgi.i, size] = replacementForThumbs;
                    }
                }

                foreach (Item item in tgiToItem.Values)
                    if (item.tgi != new TGI(0, 0, 0) && oldToNew.ContainsKey(item.ResourceIndexEntry.Instance))
                        item.ResourceIndexEntry.Instance = oldToNew[item.ResourceIndexEntry.Instance];

                if (!disableCompression)
                    foreach (IResourceIndexEntry ie in objPkgs[0].GetResourceList) ie.Compressed = 0xffff;

                objPkgs[0].SavePackage();
            }
            finally
            {
                splitContainer1.Panel1.Controls.Clear();
                tlpButtons.Enabled = true;
                objectChooser.Items.Clear();
                ClearTabs();
                ClosePkg();
                step = None;
                setButtons(Page.None, None);
            }

            CopyableMessageBox.Show("OK", myName, CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Information);
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

        private void SlurpTGIsFromTGI(string key, TGI tgi) { Item item = new Item(objPkgs, tgi); if (item.ResourceIndexEntry != null) SlurpTGIsFromField(key, (AResource)item.Resource); }
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

        private void SlurpKindred(string key, IList<IPackage> pkgs, string[] fields, TypedValue[] values)
        {
            List<TGI> seen = new List<TGI>();
            foreach (IPackage pkg in pkgs)
            {
                IList<IResourceIndexEntry> lrie = pkg.FindAll(fields, values);
                int i = 0;
                foreach (IResourceIndexEntry rie in lrie)
                {
                    TGI tgi = new TGI(rie);
                    if (seen.Contains(tgi)) continue;
                    Add(key + "[" + i + "]", new TGI(rie));
                    seen.Add(tgi);
                    i++;
                }
                break;
            }
        }
        #endregion


        #region Menu Bar
        private void menuBarWidget1_MBDropDownOpening(object sender, MenuBarWidget.MBDropDownOpeningEventArgs mn)
        {
            switch (mn.mn)
            {
                case MenuBarWidget.MD.MBF: break;
                case MenuBarWidget.MD.MBC: break;
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
            cloneType(0x319E4F1D);
        }

        string currentPackage = "";
        private void fileOpen()
        {
            ClosePkg();
            openPackageDialog.InitialDirectory = ObjectCloner.Properties.Settings.Default.LastSaveFolder == null || ObjectCloner.Properties.Settings.Default.LastSaveFolder.Length == 0
                ? "" : ObjectCloner.Properties.Settings.Default.LastSaveFolder;
            openPackageDialog.FileName = "*.package";
            DialogResult dr = openPackageDialog.ShowDialog();
            if (dr != DialogResult.OK) return;
            haveLoaded = currentPackage == openPackageDialog.FileName;
            currentPackage = openPackageDialog.FileName;
            ObjectCloner.Properties.Settings.Default.LastSaveFolder = Path.GetDirectoryName(openPackageDialog.FileName);

            IPackage pkg;
            try
            {
                pkg = s3pi.Package.Package.OpenPackage(0, openPackageDialog.FileName, true);
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                for (Exception inex = ex.InnerException; inex != null; inex = inex.InnerException) s += "\n" + inex.Message;
                CopyableMessageBox.Show("Could not open package " + openPackageDialog.FileName + "\n\n" + s, "File Open", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Error);
                return;
            }
            tmbPkgs = ddsPkgs = objPkgs = new List<IPackage>(new IPackage[] { pkg, });

            mode = Mode.Fix;
            fileNewOpen(0);
        }

        uint currentCatalogType = 0;
        void fileNewOpen(uint resourceType)
        {
            menuBarWidget1.Enable(MenuBarWidget.MB.MBF_new, false);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBF_open, false);
            ckbDefault.Enabled = mode == Mode.Clone;
            ckbDefault.Checked = mode == Mode.Clone;
            ckbPadSTBLs.Enabled = mode == Mode.Clone;
            ckbPadSTBLs.Checked = false;
            ckbNoOBJD.Enabled = mode == Mode.Fix;
            ckbNoOBJD.Checked = false;
            ckbCatlgDetails.Enabled = mode == Mode.Fix;
            ckbCatlgDetails.Checked = false;

            DoWait("Please wait, loading object catalog...");
            Application.DoEvents();

            if (!haveLoaded)
                StartLoading(resourceType);
            else
                DisplayObjectChooser();
        }

        private void fileExit()
        {
            Application.Exit();
        }
        #endregion

        #region Cloning menu
        public static uint[] catalogTypes = new uint[] {
            0x319E4F1D, //0: Catalog Object
            0xCF9A4ACE, //1: Modular Resource
            0x0418FE2A, //2: Catalog Fence
            0x049CA4CD, //3: Catalog Stairs
            0x04AC5D93, //4: Catalog Proxy Product
            0x04B30669, //5: Catalog Terrain Geometry Brush
            0x04C58103, //6: Catalog Railing
            0x04ED4BB2, //7: Catalog Terrain Paint Brush
            0x04F3CC01, //8: Catalog Fireplace
            0x060B390C, //9: Catalog Terrain Water Brush
            0x316C78F2, //10: Catalog Foundation
            0x515CA4CD, //11: Catalog Wall/Floor Pattern
            0x9151E6BC, //12: Catalog Wall
            0x91EDBD3E, //13: Catalog Roof Style
            0xF1EDBD86, //14: Catalog Roof Pattern
        };
        private void menuBarWidget1_MBCloning_Click(object sender, MenuBarWidget.MBClickEventArgs mn)
        {
            try
            {
                this.Enabled = false;
                Application.DoEvents();
                int i = ((int)mn.mn) - ((int)MenuBarWidget.MB.MBC_objd);
                int j = new List<uint>(catalogTypes).IndexOf(currentCatalogType);
                if (i != j)
                {
                    if (j >= 0) menuBarWidget1.Checked((MenuBarWidget.MB)((int)MenuBarWidget.MB.MBC_objd + j), false);
                    menuBarWidget1.Checked((MenuBarWidget.MB)((int)MenuBarWidget.MB.MBC_objd + i), true);
                }
                if (i >= 0 && i < catalogTypes.Length)
                    cloneType(catalogTypes[i]);
            }
            finally { this.Enabled = true; }
        }

        private void cloneType(uint resourceType)
        {
            if (Sims3Folder == null) return;
            ClosePkg();

            setList(@"Gamedata\Shared\Packages\", s3ocIni.ContainsKey("fb0") ? s3ocIni["fb0"] : new List<string>(new string[] { "FullBuild0", }), out objPkgs);
            setList(@"Gamedata\Shared\Packages\", s3ocIni.ContainsKey("fb2") ? s3ocIni["fb2"] : new List<string>(new string[] { "FullBuild2", }), out ddsPkgs);
            setList(@"Thumbnails\", s3ocIni.ContainsKey("tmb") ? s3ocIni["tmb"] : new List<string>(new string[] { "ALLThumbnails", }), out tmbPkgs);

            mode = Mode.Clone;
            fileNewOpen(resourceType);
        }
        void setList(string path, IList<string> files, out List<IPackage> pkgs)
        {
            path = Path.Combine(Sims3Folder, path);
            pkgs = new List<IPackage>();
            foreach (string file in files)
                try { pkgs.Add(s3pi.Package.Package.OpenPackage(0, Path.Combine(path, file + ".package"))); }
                catch { }
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
        void setButtons(Page p, Step s)
        {
            Application.DoEvents();

            bool flag = p != Page.Resources;
            ckbPadSTBLs.Enabled = ckbDefault.Enabled = mode == Mode.Clone && flag;
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
                if (stepList == null)
                {
                    btnStartBackColor = Color.FromKnownColor(KnownColor.ControlLightLight);
                    this.AcceptButton = btnStart;
                    btnNext.Enabled = false;
                }
                else
                {
                    if (s != stepList[stepList.Count - 1])
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

            tlpButtons.Enabled = haveLoaded;
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

            SetStepList(selectedItem);
            if (stepList == null)
            {
                DisplayObjectChooser();
                return;
            }

            DoWait("Please wait, performing operations...");

            stepNum = 0;
            while (stepNum < stepList.Count)
            {
                step = stepList[stepNum];
                updateProgress(true, StepText[step], true, stepList.Count - 1, true, stepNum);
                Application.DoEvents();
                stepNum++;
                step();
            }

            this.toolStripProgressBar1.Visible = false;
            this.toolStripStatusLabel1.Visible = false;
            DisplayResources();
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

        #region Steps
        Item objkItem;
        List<Item> vpxyItems;
        List<Item> modlItems;
        
        delegate void Step();
        void None() { }
        List<Step> stepList;
        Step step;
        Step lastStepInChain;
        int stepNum;
        int lastInChain;
        void SetStepList(Item item)
        {
            lastStepInChain = None;
            stepList = null;

            if (item == null)
                return;

            switch (new List<uint>(catalogTypes).IndexOf(item.tgi.t))
            {
                case 0: OBJDSteps(); break;
                case 2:
                case 3:
                case 6:
                case 11:
                case 14:
                    Common_Steps(); break;
                case 1:
                case 8:
                    Modular_Steps(); break;
            }
            lastInChain = stepList == null ? -1 : stepList.IndexOf(lastStepInChain);
        }

        void OBJDSteps()
        {
            if (ckbCatlgDetails.Checked) // Implies we're Fixing
                stepList = new List<Step>(new Step[] { Catlg_addSelf, FixIntegrity });
            else
            {
                stepList = new List<Step>(new Step[] {
                    Catlg_addSelf,

                    OBJD_getOBKJ,
                    // OBJD_addOBJKref or OBJD_SlurpTGIs
                    OBJK_SlurpTGIs,
                    OBJK_getVPXY,

                    VPXYs_SlurpTGIs,
                    // VPXYs_getKinXML if NOT default textures only
                    VPXYs_getMODLs,
                    MODLs_SlurpTGIs,
                    MODLs_SlurpMLODs,
                    MODLs_SlurpTXTCs,
                    SlurpThumbnails,
                    // FixIntegrity if fixing
                });
                if (ckbDefault.Checked)
                {
                    stepList.Insert(stepList.IndexOf(OBJK_SlurpTGIs), OBJD_addOBJKref);
                }
                else
                {
                    stepList.Insert(stepList.IndexOf(OBJK_SlurpTGIs), OBJD_SlurpTGIs);
                    stepList.Insert(stepList.IndexOf(VPXYs_getMODLs), VPXYs_getKinXML);
                }
                if (mode == Mode.Fix) stepList.Add(FixIntegrity);
            }
            lastStepInChain = MODLs_SlurpTXTCs;
        }

        void Common_Steps()
        {
            if (ckbCatlgDetails.Checked) // Implies we're Fixing
                stepList = new List<Step>(new Step[] { Catlg_addSelf, FixIntegrity });
            else
            {
                stepList = new List<Step>(new Step[] {
                    Catlg_addSelf,

                    Catlg_getVPXY,

                    VPXYs_SlurpTGIs,
                    VPXYs_getMODLs,
                    MODLs_SlurpTGIs,
                    MODLs_SlurpMLODs,
                    MODLs_SlurpTXTCs,
                    SlurpThumbnails,
                    // FixIntegrity if fixing
                });
                if (mode == Mode.Fix) stepList.Add(FixIntegrity);
            }
            lastStepInChain = MODLs_SlurpTXTCs;
        }

        void Modular_Steps() { }

        Dictionary<Step, string> StepText;
        void SetStepText()
        {
            StepText = new Dictionary<Step, string>();
            StepText.Add(Catlg_addSelf, "Add selected item");

            StepText.Add(OBJD_getOBKJ, "Find OBJK");
            StepText.Add(OBJD_addOBJKref, "Add OBJK");
            StepText.Add(OBJD_SlurpTGIs, "OBJD-referenced resources");
            StepText.Add(OBJK_SlurpTGIs, "OBJK-referenced resources");
            StepText.Add(OBJK_getVPXY, "Find OBJK-referenced VPXY");

            StepText.Add(Catlg_getVPXY, "Find VPXYs in the Catalog Resource TGIBlockList");

            StepText.Add(VPXYs_SlurpTGIs, "VPXY-referenced resources");
            StepText.Add(VPXYs_getKinXML, "Preset XML (same instance as VPXY)");
            StepText.Add(VPXYs_getMODLs, "Find VPXY-referenced MODLs");
            StepText.Add(MODLs_SlurpTGIs, "MODL-referenced resources");
            StepText.Add(MODLs_SlurpMLODs, "MLOD-referenced resources");
            StepText.Add(MODLs_SlurpTXTCs, "TXTC-referenced resources");
            StepText.Add(SlurpThumbnails, "Add thumbnails");
            StepText.Add(FixIntegrity, "Fix integrity step");
        }

        void Catlg_addSelf() { Add("clone", selectedItem.tgi); }

        #region OBJD Steps
        void OBJD_getOBKJ()
        {
            uint index = (uint)selectedItem.Resource["OBJKIndex"].Value;
            IList<AResource.TGIBlock> ltgi = (IList<AResource.TGIBlock>)selectedItem.Resource["TGIBlocks"].Value;
            AResource.TGIBlock objkTGI = ltgi[(int)index];
            objkItem = new Item(objPkgs, objkTGI);
            if (ckbDefault.Checked && objkItem == null) stepNum = lastInChain;
        }
        void OBJD_addOBJKref() { Add("objk", objkItem.tgi); }
        void OBJD_SlurpTGIs() { SlurpTGIsFromField("clone", (AResource)selectedItem.Resource); }
        void OBJK_SlurpTGIs() { SlurpTGIsFromField("objk", (AResource)objkItem.Resource); }
        void OBJK_getVPXY()
        {
            int index = -1;
            IEnumerable keys = (IEnumerable)objkItem.Resource["Keys"].Value;
            foreach (AHandlerElement element in keys)
                if (((string)element["EntryName"].Value).Equals("modelKey")) { index = (int)element["CcIndex"].Value; break; }

            if (index == -1)
            {
                stepNum = lastInChain;//Skip past the chain
                return;
            }

            vpxyItems = new List<Item>();
            foreach (TGI tgi in (AResource.TGIBlockList)objkItem.Resource["TGIBlocks"].Value)
            {
                Item vpxy = new Item(new RIE(objPkgs, tgi));
                if (vpxy.Resource != null)
                    vpxyItems.Add(vpxy);
            }
        }
        #endregion

        void Catlg_getVPXY()
        {
            vpxyItems = new List<Item>();
            foreach (TGI tgi in (AResource.TGIBlockList)selectedItem.Resource["TGIBlocks"].Value)
            {
                if (tgi.t != 0x736884F1) continue;
                Item vpxy = new Item(new RIE(objPkgs, tgi));
                if (vpxy.Resource != null)
                    vpxyItems.Add(vpxy);
            }
        }

        void VPXYs_SlurpTGIs() { for (int i = 0; i < vpxyItems.Count; i++) SlurpTGIsFromField("vpxy[" + i + "]", (AResource)vpxyItems[i].Resource); }
        void VPXYs_getKinXML()
        {
            for (int i = 0; i < vpxyItems.Count; i++)
                SlurpKindred("vpxy[" + i + "].PresetXML", objPkgs, new string[] { "ResourceType", "Instance" },
                    new TypedValue[] { new TypedValue(typeof(uint), (uint)0x0333406C), new TypedValue(typeof(ulong), vpxyItems[i].tgi.i) });
        }
        void VPXYs_getMODLs()
        {
            modlItems = new List<Item>();
            for (int i = 0; i < vpxyItems.Count; i++)
            {
                GenericRCOLResource rcol = (vpxyItems[i].Resource as GenericRCOLResource);
                for (int j = 0; j < rcol.ChunkEntries.Count; j++)
                {
                    VPXY vpxychunk = rcol.ChunkEntries[j].RCOLBlock as VPXY;
                    for (int k = 0; k < vpxychunk.TGIBlocks.Count; k++)
                    {
                        AResource.TGIBlock tgib = vpxychunk.TGIBlocks[k];
                        if (tgib.ResourceType != 0x01661233) continue;
                        Item modl = new Item(new RIE(objPkgs, tgib));
                        if (modl.Resource != null)
                            modlItems.Add(modl);
                    }
                }
            }
        }
        void MODLs_SlurpTGIs() { for (int i = 0; i < modlItems.Count; i++) SlurpTGIsFromField("modl[" + i + "]", (AResource)modlItems[i].Resource); }
        void MODLs_SlurpMLODs()
        {
            for (int i = 0; i < modlItems.Count; i++)
            {
                GenericRCOLResource rcol = (modlItems[i].Resource as GenericRCOLResource);
                for (int j = 0; j < rcol.Resources.Count; j++)
                {
                    AResource.TGIBlock tgib = rcol.Resources[j];
                    if (tgib.ResourceType != 0x01D10F34) continue;
                    SlurpTGIsFromTGI("modl[" + i + "]" + ".mlod[" + j + "]", tgib);
                }
            }
        }
        void MODLs_SlurpTXTCs()
        {
            for (int i = 0; i < modlItems.Count; i++)
            {
                GenericRCOLResource rcol = (modlItems[i].Resource as GenericRCOLResource);
                for (int j = 0; j < rcol.Resources.Count; j++)
                {
                    AResource.TGIBlock tgib = rcol.Resources[j];
                    if (tgib.ResourceType != 0x033A1435) continue;
                    SlurpTGIsFromTGI("modl[" + i + "]" + ".txtc[" + j + "]", tgib);
                }
            }
        }

        //Bring in all resources from ...\The Sims 3\Thumbnails\ALLThumbnails.package that match the instance number of the OBJD
        //FullBuild0 is:
        //  ...\Gamedata\Shared\Packages\FullBuild0.package
        //Relative path to ALLThumbnails is:
        // .\..\..\..\Thumbnails\ALLThumbnails.package
        void SlurpThumbnails()
        {
            //0x515CA4CD is very different
                foreach (THUM.THUMSize size in new THUM.THUMSize[] { THUM.THUMSize.small, THUM.THUMSize.medium, THUM.THUMSize.large, })
                {
                    if (selectedItem.tgi.t == catalogTypes[11])
                    {
                    }
                    else
                    {
                        TGI tgi = getImageTGI(size, selectedItem);
                        if (selectedItem.tgi.t == catalogTypes[14])
                            Add(size + "Icon", tgi);
                        else
                            Add(size + "Thumb", tgi);
                    }
                }
#if UNDEF
            ulong instance = selectedItem.tgi.i;
            string[] fields = new string[ckbDefault.Checked ? 2 : 1];
            TypedValue[] values = new TypedValue[ckbDefault.Checked ? 2 : 1];
            fields[0] = "Instance";
            values[0] = new TypedValue(typeof(ulong), instance);
            if (ckbDefault.Checked)
            {
                fields[1] = "ResourceGroup";
                values[1] = new TypedValue(typeof(uint), (uint)0);
            }

            SlurpKindred("thumb", mode == Mode.Clone ? tmbPkgs : objPkgs, fields, values);
#endif
        }

        //Fix integrity step
        void FixIntegrity()
        {
            //tgiLookup -- List of resources for cloned object, excludes name map and stbls

            if (!ckbCatlgDetails.Checked)//No point adding NMAP
            {
                IList<IResourceIndexEntry> lnmaprie = objPkgs[0].FindAll(new String[] { "ResourceType" }, new TypedValue[] { new TypedValue(typeof(uint), (uint)0x0166038C), });
                foreach (IResourceIndexEntry rie in lnmaprie)
                {
                    TGI tgi = new TGI(rie);
                    tgiLookup.Add("namemap", tgi);
                }
            }

            IList<IResourceIndexEntry> lstblrie = objPkgs[0].FindAll(new String[] { "ResourceType" }, new TypedValue[] { new TypedValue(typeof(uint), (uint)0x220557DA), });
            foreach (IResourceIndexEntry rie in lstblrie)
            {
                TGI tgi = new TGI(rie);
                tgiLookup.Add("lang_" + (tgi.i >> 56).ToString("X2"), tgi);
            }

            //tgiLookup -- List of resources for cloned object, *includes* name map and stbls now -- may include references to things not in package

            oldToNew = new Dictionary<ulong, ulong>();

            // Prevent OBJD and related resources getting renumbered
            if (ckbNoOBJD.Checked || ckbCatlgDetails.Checked)
                oldToNew.Add(selectedItem.tgi.i, selectedItem.tgi.i);

            // Renumber the PNGInstance we're referencing
            ulong PngInstance = (ulong)((AApiVersionedFields)selectedItem.Resource["CommonBlock"].Value)["PngInstance"].Value;
            if (PngInstance != 0)
                oldToNew.Add(PngInstance, CreateInstance());


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
                Item item = new Item(objPkgs, kvp.Value);
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


            nameGUID = (ulong)((AApiVersionedFields)selectedItem.Resource["CommonBlock"].Value)["NameGUID"].Value;
            descGUID = (ulong)((AApiVersionedFields)selectedItem.Resource["CommonBlock"].Value)["DescGUID"].Value;

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
            resourceList.Add("Old ObjName: \"" + ((AApiVersionedFields)selectedItem.Resource["CommonBlock"].Value)["Name"] + "\" --> New Name: \"CatalogObjects/Name:" + UniqueObject + "\"");
            resourceList.Add("Old ObjDesc: \"" + ((AApiVersionedFields)selectedItem.Resource["CommonBlock"].Value)["Desc"] + "\" --> New Desc: \"CatalogObjects/Description:" + UniqueObject + "\"");
            

            resourceList.Add("Old CatlgName: \"" + English[nameGUID] + "\" --> New CatlgName: \"" + tbCatlgName.Text + "\"");
            resourceList.Add("Old CatlgDesc: \"" + English[descGUID] + "\" --> New CatlgDesc: \"" + tbCatlgDesc.Text + "\"");
            resourceList.Add("Old Price: " + ((AApiVersionedFields)selectedItem.Resource["CommonBlock"].Value)["Price"] + " --> New Price: " + float.Parse(tbPrice.Text));
            if (PngInstance != 0)
                resourceList.Add("Old PngInstance: " + ((AApiVersionedFields)selectedItem.Resource["CommonBlock"].Value)["PngInstance"] + " --> New PngInstance: 0x" + oldToNew[PngInstance].ToString("X16"));
        }
        #endregion

        private void ckbCatlgDetails_CheckedChanged(object sender, EventArgs e)
        {
            if (!ckbCatlgDetails.Enabled) return;
            ckbNoOBJD.Enabled = !ckbCatlgDetails.Checked;
        }

        private void ckbNoComp_CheckedChanged(object sender, EventArgs e)
        {
            disableCompression = !ckbCompress.Checked;
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
        bool gtAbort() { return false; }
    }
}
