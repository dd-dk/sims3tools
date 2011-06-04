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

        private ObjectCloner.TopPanelComponents.ObjectChooser objectChooser;
        private ObjectCloner.TopPanelComponents.PleaseWait pleaseWait;
        private ObjectCloner.TopPanelComponents.CloneFixOptions cloneFixOptions;
        private ObjectCloner.TopPanelComponents.Search searchPane;
        private ObjectCloner.TopPanelComponents.TGISearch tgiSearchPane;

        public MainForm()
        {
            InitializeComponent();

            this.Text = myName;
            pleaseWait = new PleaseWait();

            MainForm_LoadFormSettings();

            menuBarWidget1.Checked(MenuBarWidget.MB.MBS_langSearch, STBLHandler.LangSearch);

            menuBarWidget1.Checked(MenuBarWidget.MB.MBS_advanced, ObjectCloner.Properties.Settings.Default.AdvanceCloning);

            Diagnostics.Popups = ObjectCloner.Properties.Settings.Default.Diagnostics;
            menuBarWidget1.Checked(MenuBarWidget.MB.MBS_popups, Diagnostics.Popups);

            SetStepText();

            InitialiseTabs(CatalogType.CatalogProxyProduct);//Use the Proxy Product as it has pretty much nothing on it
            TabEnable(false);
        }

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
            if (w > 0 && w < 100)
                splitContainer1.SplitterDistance = this.Width * w / 100;
        }

        static bool formClosing = false;
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Diagnostics.Log("MainForm_FormClosing CloseReason: " + e.CloseReason);
            formClosing = true;
            AbortSaving(true);
            Abort(true);
            //CloseCurrent();//needed?

            ObjectCloner.Properties.Settings.Default.PersistentLocation = this.WindowState == FormWindowState.Normal ? this.Location : new Point(-1, -1);
            ObjectCloner.Properties.Settings.Default.PersistentHeight = this.WindowState == FormWindowState.Normal ? this.Height : -1;
            ObjectCloner.Properties.Settings.Default.PersistentWidth = this.WindowState == FormWindowState.Normal ? this.Width : -1;
            ObjectCloner.Properties.Settings.Default.Splitter1Width = splitContainer1.SplitterDistance * 100 / this.Width;
            ObjectCloner.Properties.Settings.Default.Save();
        }

        void Abort(bool abort)
        {
            Diagnostics.Log("Abort abort:" + abort);

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

        public static void CloseCurrent()
        {
            Diagnostics.Log("ClosePkg");

            if (FileTable.Current != null)
            {
                s3pi.Package.Package.ClosePackage(0, FileTable.Current.Package);
                FileTable.Current = null;
                Reset();// pick up new NameMap / STBLs next time needed
            }
        }

        public static void SetUseCC(bool value)
        {
            if (FileTable.UseCustomContent != value)
            {
                FileTable.UseCustomContent = value;
                Reset();
            }
        }

        public static void SetAppendEA(bool value)
        {
            if (FileTable.AppendFileTable != value)
            {
                FileTable.AppendFileTable = value;
                Reset();
            }
        }

        static void Reset()
        {
            NameMap.Reset();
            STBLHandler.Reset();
        }

        void InitialiseFileTable()
        {
            Abort(false);
            CloseCurrent();
            SetUseCC(false);
            SetAppendEA(true);
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
            Options.Add("help", new CmdInfo(CmdLineHelp, "Display this help"));
        }
        void CmdLine(params string[] args)
        {
            SetOptions();
            List<string> pkgs = new List<string>();
            List<string> cmdline = new List<string>(args);
            while (cmdline.Count > 0)
            {
                if (cmdline[0].StartsWith("/") || cmdline[0].StartsWith("-"))
                {
                    string option = cmdline[0].Substring(1);
                    if (Options.ContainsKey(option.ToLower()))
                    {
                        if (Options[option.ToLower()].cmd(ref cmdline))
                            Environment.Exit(0);
                    }
                    else
                    {
                        CopyableMessageBox.Show(this, "Invalid command line option: '" + option + "'",
                            myName, CopyableMessageBoxIcon.Error, new List<string>(new string[] { "OK" }), 0, 0);
                        Environment.Exit(1);
                    }
                }
                else
                    pkgs.Add(cmdline[0]);
                cmdline.RemoveAt(0);
            }
        }
        bool cmdlineTest = false;
        bool CmdLineTest(ref List<string> cmdline) { cmdlineTest = true; return false; }
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
            return new SpecificResource(FileTable.fb0, rk);
        }

        #region LeftPanelComponents
        public abstract class ItemEventArgs : EventArgs
        {
            ListViewItem selectedItem = null;
            public ItemEventArgs(ListView lv) { selectedItem = lv.SelectedItems.Count == 0 ? null : lv.SelectedItems[0]; }
            public SpecificResource SelectedItem { get { return selectedItem == null ? null : selectedItem.Tag as SpecificResource; } }
        }
        public class ItemActivateEventArgs : ItemEventArgs { public ItemActivateEventArgs(ListView lv) : base(lv) { } }
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
                toolStripProgressBar1.Value = value;
        }

        Dictionary<UInt64, SpecificResource> CTPTBrushIndexToPair;
        public delegate void listViewAddCallBack(SpecificResource sr, ListView listView);
        public void ListViewAdd(SpecificResource sr, ListView listView)
        {
            #region CatalogTerrainPaintBrush pair
            if (sr.CType == CatalogType.CatalogTerrainPaintBrush)
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

            string name;
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

            List<string> exts;
            string tag = "UNKN";
            if (s3pi.Extensions.ExtList.Ext.TryGetValue("0x" + sr.ResourceIndexEntry.ResourceType.ToString("X8"), out exts)) tag = exts[0];

            listView.Items.Add(new ListViewItem(new string[] {
                name, tag, sr.RGVsn, "" + (AResourceKey)sr.ResourceIndexEntry,
                sr.PathPackage.Path + " (" + sr.PPSource + ")",
            }) { Tag = sr });
        }

        private void DisplayListView(Control listView)
        {
            this.AcceptButton = btnStart;
            this.CancelButton = null;

            menuBarWidget1.Enable(MenuBarWidget.MB.MBF_open, true);
            menuBarWidget1.Enable(MenuBarWidget.MD.MBC, true);
            menuBarWidget1.Enable(MenuBarWidget.MD.MBT, true);
            menuBarWidget1.Enable(MenuBarWidget.MD.MBS, true);

            lbTGISearch.Visible = false;
            lbSearch.Visible = listView == searchPane;
            lbUseMenu.Visible = false;
            lbSelectOptions.Visible = false;
            btnStart.Visible = true;
            btnStart.Enabled = false;

            StopWait();
            splitContainer1.Panel1.Controls.Clear();
            splitContainer1.Panel1.Controls.Add(listView);
            listView.Dock = DockStyle.Fill;
            listView.Focus();
        }

        private void listView_SelectedIndexChanged(object sender, SelectedIndexChangedEventArgs e)
        {
            if (isFix) return;

            replacementForThumbs = null;// might as well be here; needed after FillTabs, really.
            rkLookup = null;//Indicate that we're not working on the same resource any more
            if (formClosing || e.SelectedItem == null)
            {
                selectedItem = null;
                ClearTabs();
                btnStart.Enabled = false;
            }
            else
            {
                selectedItem = e.SelectedItem;
                FillTabs(selectedItem);
                btnStart.Enabled = true;
            }
        }
        private void listView_ItemActivate(object sender, ItemActivateEventArgs e) { btnStart_Click(sender, EventArgs.Empty); }

        private void DisplaySearch()
        {
            Diagnostics.Log("DisplaySearch");

            isFix = false;
            DisplayListView(searchPane);
        }

        private void DisplayObjectChooser()
        {
            Diagnostics.Log("DisplayObjectChooser");

            DoWait("Setting up folders...");
            if (!CheckInstallDirs(splitContainer1.Panel1)) return;

            DisplayListView(objectChooser);
        }

        private void DisplayTGISearch()
        {
            Diagnostics.Log("DisplayTGISearch");

            this.AcceptButton = btnStart;
            this.CancelButton = null;

            menuBarWidget1.Enable(MenuBarWidget.MB.MBF_open, true);
            menuBarWidget1.Enable(MenuBarWidget.MD.MBC, true);
            menuBarWidget1.Enable(MenuBarWidget.MD.MBT, true);
            menuBarWidget1.Enable(MenuBarWidget.MD.MBS, true);

            lbTGISearch.Visible = true;
            lbSearch.Visible = false;
            lbUseMenu.Visible = false;
            lbSelectOptions.Visible = false;
            btnStart.Visible = false;

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
            switch (selectedItem.CType)
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
            lbTGISearch.Visible = false;
            lbSearch.Visible = false;
            lbUseMenu.Visible = false;
            lbSelectOptions.Visible = true;
            btnStart.Visible = false;

            if (searchPane != null)
                mode = Mode.FromGame;

            cloneFixOptions = new CloneFixOptions(this, mode == Mode.FromGame, hasOBJDs(), itemName == null || itemDesc == null);
            cloneFixOptions.CancelClicked += new EventHandler(cloneFixOptions_CancelClicked);
            cloneFixOptions.StartClicked += new EventHandler(cloneFixOptions_StartClicked);

            if (mode == Mode.FromGame)
            {
                string prefix = CreatorName;
                prefix = (prefix != null) ? prefix + "_" : "";
                cloneFixOptions.UniqueName = prefix + (searchPane == null ?
                    objectChooser.SelectedItem.Text
                    : searchPane.SelectedItem.Text);
            }
            else
            {
                cloneFixOptions.UniqueName = Path.GetFileNameWithoutExtension(openPackageDialog.FileName);
            }

            StopWait();
            splitContainer1.Panel1.Controls.Clear();
            splitContainer1.Panel1.Controls.Add(cloneFixOptions);
            cloneFixOptions.Dock = DockStyle.Fill;
            cloneFixOptions.Focus();
        }

        private void DisplayNothing()
        {
            Diagnostics.Log("DisplayNothing");
            this.AcceptButton = null;
            this.CancelButton = null;

            menuBarWidget1.Enable(MenuBarWidget.MB.MBF_open, true);
            menuBarWidget1.Enable(MenuBarWidget.MD.MBC, true);
            menuBarWidget1.Enable(MenuBarWidget.MD.MBT, true);
            menuBarWidget1.Enable(MenuBarWidget.MD.MBS, true);

            lbTGISearch.Visible = false;
            lbSearch.Visible = false;
            lbUseMenu.Visible = true;
            lbSelectOptions.Visible = false;
            btnStart.Visible = false;

            StopWait();
            TabEnable(false);
            splitContainer1.Panel1.Controls.Clear();
            if (cloneFixOptions != null)
            {
                cloneFixOptions.Enabled = false;
                cloneFixOptions.Dock = DockStyle.Fill;
                splitContainer1.Panel1.Controls.Add(cloneFixOptions);
            }
        }

        private void DoWait(string waitText = "Please wait...")
        {
            Diagnostics.Log(waitText);

            splitContainer1.Panel1.Controls.Clear();
            splitContainer1.Panel1.Controls.Add(pleaseWait);
            pleaseWait.Dock = DockStyle.Fill;
            pleaseWait.Label = waitText;
            splitContainer1.Panel2.Enabled = false;
            TabEnable(false);
            this.Text = myName + " [busy]";
            Application.DoEvents();
        }
        private void StopWait()
        {
            if (pleaseWait != null && pleaseWait.Label != "") { Diagnostics.Log("StopWait: " + pleaseWait.Label); pleaseWait.Label = ""; }
            else Diagnostics.Log("StopWait");

            splitContainer1.Panel2.Enabled = true;
            this.Text = myName;
            Application.DoEvents();
        }
        #endregion

        #region CloneFixOptions
        bool isCreateNewPackage = false;
        bool isDeepClone = false;
        bool isPadSTBLs = false;
        bool disableCompression = false;
        void cloneFixOptions_StartClicked(object sender, EventArgs e)
        {
            Diagnostics.Log("cloneFixOptions_StartClicked");
            this.AcceptButton = null;
            this.CancelButton = null;
            disableCompression = !cloneFixOptions.IsCompress;
            isCreateNewPackage = cloneFixOptions.IsClone;
            isDeepClone = isFix || cloneFixOptions.IsDeepClone;
            isPadSTBLs = cloneFixOptions.IsPadSTBLs;

            if (cloneFixOptions.IsRepair)
                SetAppendEA(true);

            if (isCreateNewPackage) CloneStart();
            else FixStart();
        }

        void cloneFixOptions_CancelClicked(object sender, EventArgs e)
        {
            Diagnostics.Log("cloneFixOptions_CancelClicked");
            TabEnable(false);
            if (searchPane != null)
                DisplaySearch();
            else
                DisplayObjectChooser();
            cloneFixOptions = null;
        }

        void CloneStart()
        {
            Diagnostics.Log("CloneStart");
            uniqueObject = null;
            if (UniqueObject == null)
            {
                cloneFixOptions_CancelClicked(null, null);
                return;
            }

            saveFileDialog1.FileName = UniqueObject;
            if (ObjectCloner.Properties.Settings.Default.LastSaveFolder != null)
                saveFileDialog1.InitialDirectory = ObjectCloner.Properties.Settings.Default.LastSaveFolder;
            DialogResult dr = saveFileDialog1.ShowDialog();
            if (dr != DialogResult.OK)
            {
                cloneFixOptions_CancelClicked(null, null);
                return;
            }
            ObjectCloner.Properties.Settings.Default.LastSaveFolder = Path.GetDirectoryName(saveFileDialog1.FileName);
            Diagnostics.Log("CloneStart: " + saveFileDialog1.FileName);

            RunRKLookup();
            StartSaving();
        }

        void FixStart()
        {
            Diagnostics.Log("FixStart");
            uniqueObject = null;
            if (cloneFixOptions.IsRenumber && UniqueObject == null)
            {
                cloneFixOptions_CancelClicked(null, null);
                return;
            }

            RunRKLookup();
            if (cloneFixOptions.IsRepair)
                RunRepair();//then StartFixing
            else
                StartFixing();
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
                    new flagField(tlpUnknown8, "ObjectTypeFlags", 32, 0),
                    new flagField(tlpUnknown9, "WallPlacementFlags", 32, 0),
                    new flagField(tlpUnknown10, "MovementFlags", 32, 0),
                    new flagField(tlpRoomSort, "RoomCategoryFlags", 32, 0),
                    new flagField(tlpRoomSubLow, "RoomSubCategoryFlags", 32, 0),
                    new flagField(tlpRoomSubHigh, "RoomSubCategoryFlags", 64, 32),
                    new flagField(tlpFuncSort, "FunctionCategoryFlags", 32, 0),
                    new flagField(tlpFuncSubLow, "FunctionSubCategoryFlags", 32, 0),
                    new flagField(tlpFuncSubHigh, "FunctionSubCategoryFlags", 64, 32),
                    new flagField(tlpBuildSort, "BuildCategoryFlags", 32, 0),
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
            if (tabType == resourceType) return;

            string appWas = this.Text;
            this.Text = myName + " [busy]";
            Application.UseWaitCursor = true;
            Application.DoEvents();
            try
            {
                tabType = resourceType;

                if (resourceType == CatalogType.ModularResource) resourceType = CatalogType.CatalogObject;//Modular Resources - display OBJD0

                IResource res = s3pi.WrapperDealer.WrapperDealer.CreateNewResource(0, "0x" + ((uint)resourceType).ToString("X8"));
                this.tabControl1.Controls.Remove(this.tpDetail);
                if (tabType == CatalogType.CatalogObject || tabType == CatalogType.CatalogTerrainPaintBrush)
                {
                    this.tabControl1.Controls.Add(this.tpDetail);
                    InitialiseDetailsTab(res);
                }
                this.tabControl1.Controls.Remove(this.tpFlagsRoom);
                this.tabControl1.Controls.Remove(this.tpFlagsFunc);
                this.tabControl1.Controls.Remove(this.tpFlagsBuild);
                this.tabControl1.Controls.Remove(this.tpFlagsMisc);
                if (tabType == CatalogType.CatalogObject)
                {
                    this.tabControl1.Controls.Add(this.tpFlagsRoom);
                    this.tabControl1.Controls.Add(this.tpFlagsFunc);
                    this.tabControl1.Controls.Add(this.tpFlagsBuild);
                    this.tabControl1.Controls.Add(this.tpFlagsMisc);
                    InitialiseFlagTabs(res);
                    if (tlpOther.Visible)
                        InitialiseOtherTab(res);
                }
            }
            finally { this.Text = appWas; Application.UseWaitCursor = false; Application.DoEvents(); }
        }
        void InitialiseDetailsTab(IResource catlg) { InitialiseMaps(); InitialiseTabTLP(catlg, tlpObjectDetail, labelToFieldMap.Values); }
        void InitialiseOtherTab(IResource objd) { InitialiseMaps(); InitialiseTabTLP(objd, tlpOther, otherFieldMap.Keys); }

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
            string appWas = this.Text;
            this.Text = myName + " [busy]";
            Application.UseWaitCursor = true;
            Application.DoEvents();
            try
            {
                clearOverview();
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
            pictureBox1.Image = null;
            lbThumbTGI.Text = "";
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
            string appWas = this.Text;
            this.Text = myName + " [busy]";
            Application.UseWaitCursor = true;
            Application.DoEvents();
            try
            {
                if (formClosing || item.Resource == null)
                {
                    ClearTabs();
                    return;
                }

                InitialiseTabs(item.CType);
                if (formClosing) return;

                SpecificResource catlg = (item.CType == CatalogType.ModularResource) ? ItemForTGIBlock0(item) : item;

                if (catlg.ResourceIndexEntry != null)
                {
                    fillOverview(catlg);
                    if (formClosing) return;
                    if (tabControl1.Contains(tpDetail))
                        fillDetails(catlg);
                    if (formClosing) return;
                    if (item.CType == CatalogType.CatalogObject)
                    {
                        fillFlags(catlg);
                        if (formClosing) return;
                        fillOther(catlg);
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
            lbThumbTGI.Font = new Font(lbThumbTGI.Font, FontStyle.Regular);
            tbObjName.Font = tbNameGUID.Font =
            tbCatlgName.Font = tbObjDesc.Font = tbDescGUID.Font =
            tbCatlgDesc.Font = tbPrice.Font = tbProductStatus.Font =
                new Font(tbObjName.Font, FontStyle.Regular);

            AApiVersionedFields common = item.Resource["CommonBlock"].Value as AApiVersionedFields;
            if (formClosing) return; else pictureBox1.Image = THUM.getImage(THUM.THUMSize.large, item);
            if (formClosing) return; else lbThumbTGI.Text = (AResourceKey)THUM.getImageRK(THUM.THUMSize.large, item);
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
            lbThumbTGI.Text = tbObjName.Text = tbNameGUID.Text =
            tbCatlgName.Text = tbObjDesc.Text = tbDescGUID.Text =
            tbCatlgDesc.Text = tbPrice.Text = tbProductStatus.Text =
                noData;
            lbThumbTGI.Font = new Font(lbThumbTGI.Font, FontStyle.Italic);
            tbObjName.Font = tbNameGUID.Font =
            tbCatlgName.Font = tbObjDesc.Font = tbDescGUID.Font =
            tbCatlgDesc.Font = tbPrice.Font = tbProductStatus.Font =
                new Font(tbObjName.Font, FontStyle.Italic);
        }

        void fillDetails(SpecificResource item) { IterateTLP(tlpObjectDetail, (l, c) => fillControl(item, l, c)); }
        void fillOther(SpecificResource item) { IterateTLP(tlpOther, (l, c) => fillControl(item, l, c)); }
        void fillFlags(SpecificResource item)
        {
            foreach (flagField ff in flagFields)
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
            if (pictureBox1.Image == null)
            {
                pictureBox1.Image = THUM.getLargestThumbOrDefault(item).GetThumbnailImage(pictureBox1.Width, pictureBox1.Height, gtAbort, IntPtr.Zero);
                lbThumbTGI.Text = (AResourceKey)THUM.getNewRK(THUM.THUMSize.large, item);
            }
        }

        void TabEnable(bool enabled)
        {
            string appWas = this.Text;
            this.Text = myName + " [busy]";
            Application.UseWaitCursor = true;
            Application.DoEvents();
            try
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
            finally { this.Text = appWas; Application.UseWaitCursor = false; Application.DoEvents(); }
        }
        void tabEnableOverview(bool enabled)
        {
            enabled &= !((string)tpMain.Tag == noData);
            btnReplThumb.Enabled = enabled;
            tbCatlgName.ReadOnly = !enabled;// || itemName == null;
            tbCatlgDesc.ReadOnly = !enabled;// || itemDesc == null;
            ckbCopyToAll.Enabled = enabled;
            tbPrice.ReadOnly = !enabled;
            tbProductStatus.ReadOnly = !enabled;
            tbCatlgName.BackColor = tbCatlgName.ReadOnly ? SystemColors.Control : SystemColors.Window;
            tbCatlgDesc.BackColor = tbCatlgDesc.ReadOnly ? SystemColors.Control : SystemColors.Window;
        }
        void tabEnableDetails(bool enabled) { IterateTLP(tlpObjectDetail, (l, c) => { if (c.Tag != null) c.Enabled = enabled; }); }
        void tabEnableOther(bool enabled) { IterateTLP(tlpOther, (l, c) => { if (c.Tag != null) c.Enabled = enabled; }); }
        void tabEnableFlags(bool enabled)
        {
            foreach (flagField ff in flagFields)
            {
                for (int i = 1; i < ff.tlp.RowCount - 1; i++)
                {
                    CheckBox cb = (CheckBox)ff.tlp.GetControlFromPosition(0, i);
                    cb.Enabled = enabled;
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
                case MenuBarWidget.MD.MBF: break;
                case MenuBarWidget.MD.MBC: break;
                case MenuBarWidget.MD.MBT: break;
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
                    case MenuBarWidget.MB.MBF_exit: fileExit(); break;
                }
            }
            finally { this.Enabled = true; }
        }

        private void fileOpen()
        {
            Diagnostics.Log("fileOpen");

            openPackageDialog.InitialDirectory = ObjectCloner.Properties.Settings.Default.LastSaveFolder == null || ObjectCloner.Properties.Settings.Default.LastSaveFolder.Length == 0
                ? "" : ObjectCloner.Properties.Settings.Default.LastSaveFolder;
            openPackageDialog.FileName = "*.package";
            DialogResult dr = openPackageDialog.ShowDialog();
            if (dr != DialogResult.OK) return;
            ObjectCloner.Properties.Settings.Default.LastSaveFolder = Path.GetDirectoryName(openPackageDialog.FileName);

            mode = Mode.FromUser;

            fileReOpenToFix(openPackageDialog.FileName, 0);
        }

        void fileReOpenToFix(string filename, CatalogType type)
        {
            Diagnostics.Log(String.Format("fileReOpenToFix filename: {0}, type: {1}", filename, type));

            Abort(true);
            CloseCurrent();//not needed?
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

            LoadObjectChooser(type, type != 0);
        }

        private void fileExit()
        {
            Diagnostics.Log("fileExit");
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

                mode = Mode.FromGame;

                InitialiseFileTable();

                LoadObjectChooser(FromCloningMenuEntry(mn.mn), false);
            }
            finally { this.Enabled = true; }
        }
        CatalogType FromCloningMenuEntry(MenuBarWidget.MB menuEntry)
        {
            if (!Enum.IsDefined(typeof(MenuBarWidget.MB), menuEntry)) return 0;
            List<MenuBarWidget.MB> ml = new List<MenuBarWidget.MB>((MenuBarWidget.MB[])Enum.GetValues(typeof(MenuBarWidget.MB)));
            return ((CatalogType[])Enum.GetValues(typeof(CatalogType)))[ml.IndexOf(menuEntry) - ml.IndexOf(MenuBarWidget.MB.MBC_cfen)];
        }
        #endregion

        bool isFix = false;
        void LoadObjectChooser(CatalogType resourceType, bool IsFixPass)
        {
            Diagnostics.Log(String.Format("LoadObjectChooser resourceType: {0}, IsFixPass: {1}", resourceType, IsFixPass));

            isFix = IsFixPass;

            CTPTBrushIndexToPair = new Dictionary<ulong, SpecificResource>();
            objectChooser = new ObjectChooser(updateProgress, ListViewAdd, resourceType, isFix);
            if (isFix)
            {
                objectChooser.SelectedIndexChanged += new EventHandler<SelectedIndexChangedEventArgs>((sender, e) => selectedItem = e.SelectedItem);
                objectChooser.ItemActivate += new EventHandler<ItemActivateEventArgs>((sender, e) => FixStart());
            }
            else
            {
                ClearTabs();
                objectChooser.SelectedIndexChanged += new EventHandler<SelectedIndexChangedEventArgs>(listView_SelectedIndexChanged);
                objectChooser.ItemActivate += new EventHandler<ItemActivateEventArgs>(listView_ItemActivate);
            }

            DisplayObjectChooser();
        }

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
                }
            }
            finally { this.Enabled = true; }
        }

        private void toolsSearch()
        {
            ClearTabs();
            InitialiseFileTable();

            CTPTBrushIndexToPair = new Dictionary<ulong, SpecificResource>();
            searchPane = new Search(updateProgress, ListViewAdd);
            searchPane.SelectedIndexChanged += new EventHandler<SelectedIndexChangedEventArgs>(listView_SelectedIndexChanged);
            searchPane.ItemActivate += new EventHandler<ItemActivateEventArgs>(listView_ItemActivate);

            DisplaySearch();
        }

        private void toolsTGISearch()
        {
            ClearTabs();
            InitialiseFileTable();

            tgiSearchPane = new TGISearch(updateProgress);

            DisplayTGISearch();
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
            tabType = 0;
            Reset();//needed!
            Abort(true);
            DisplayNothing();

            while (true)
            {
                SettingsForms.GameFolders gf = new ObjectCloner.SettingsForms.GameFolders();
                DialogResult dr = gf.ShowDialog();
                if (dr != DialogResult.OK && dr != DialogResult.Retry) return;
                if (dr != DialogResult.Retry) break;
            }
            FileTable.Reset();//needed!
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

        private void settingsLangSearch()
        {
            Application.DoEvents();
            ObjectCloner.Properties.Settings.Default.LangSearch = !STBLHandler.LangSearch;
            menuBarWidget1.Checked(MenuBarWidget.MB.MBS_langSearch, STBLHandler.LangSearch);
        }

        private void settingsAutomaticUpdates()
        {
            AutoUpdate.Checker.AutoUpdateChoice = !menuBarWidget1.IsChecked(MenuBarWidget.MB.MBS_updates);
        }

        private void settingsAdvancedCloning()
        {
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

        private void helpUpdate()
        {
            bool msgDisplayed = AutoUpdate.Checker.GetUpdate(false);
            if (!msgDisplayed)
                CopyableMessageBox.Show("Your " + Application.ProductName + " is up to date", this.Text,
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

        public static bool CheckInstallDirs(Control control)
        {
            Diagnostics.Log("CheckInstallDirs");
            try
            {
                PleaseWait.DoWait(control, "Setting up folders...");
                if (!FileTable.IsOK)
                {
                    CopyableMessageBox.Show("Found no packages\nPlease check your Game Folder settings.", "No objects to clone",
                        CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Stop);
                    return false;
                }
                bool loadedSTBLs = STBLHandler.IsOK;
                bool loadedNMaps = NameMap.IsOK;
                return true;
            }
            finally { PleaseWait.StopWait(control); Application.DoEvents(); }
        }
        #endregion

        private void btnReplThumb_Click(object sender, EventArgs e)
        {
            openThumbnailDialog.FilterIndex = 1;
            openThumbnailDialog.FileName = "*.PNG";
            DialogResult dr = openThumbnailDialog.ShowDialog();
            if (dr != DialogResult.OK) return;
            try
            {
                replacementForThumbs = Image.FromFile(openThumbnailDialog.FileName, true);
                pictureBox1.Image = replacementForThumbs.GetThumbnailImage(pictureBox1.Width, pictureBox1.Height, gtAbort, System.IntPtr.Zero);
            }
            catch (Exception ex)
            {
                CopyableMessageBox.IssueException(ex, "Could not read thumbnail:\n" + openThumbnailDialog.FileName, openThumbnailDialog.Title);
                replacementForThumbs = null;
            }
        }
        static bool gtAbort() { return false; }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Diagnostics.Log("btnStart_Click");
            fillOverviewUpdateImage(selectedItem);
            TabEnable(true);
            DisplayOptions();
        }


        #region Fetch resources
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
                    cloneFixOptions_CancelClicked(null, null);
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

        private void SlurpRKsFromRK(string key, IResourceKey rk)
        {
            SpecificResource item = new SpecificResource(FileTable.fb0, rk);
            if (item.ResourceIndexEntry != null) SlurpRKsFromField(key, (AResource)item.Resource);
            else Diagnostics.Show(String.Format("RK {0} not found", key));
        }
        private void SlurpRKsFromField(string key, AApiVersionedFields field)
        {
            Type t = field.GetType();
            if (typeof(GenericRCOLResource.ChunkEntry).IsAssignableFrom(t)) { }
            else if (typeof(TGIBlock).IsAssignableFrom(t))
            {
                Add(key, (IResourceKey)field);
            }
            else
            {
                if (typeof(IEnumerable).IsAssignableFrom(t))
                    SlurpRKsFromIEnumerable(key, (IEnumerable)field);
                SlurpRKsFromAApiVersionedFields(key, field);
            }
        }
        private void SlurpRKsFromAApiVersionedFields(string key, AApiVersionedFields field)
        {
            List<string> fields = field.ContentFields;
            foreach (string f in fields)
            {
                if ((new List<string>(new string[] { "Stream", "AsBytes", "Value", })).Contains(f)) continue;

                Type t = AApiVersionedFields.GetContentFieldTypes(0, field.GetType())[f];
                if (!t.IsClass || t.Equals(typeof(string))) continue;
                if (t.IsArray && (!t.GetElementType().IsClass || t.GetElementType().Equals(typeof(string)))) continue;

                if (typeof(IEnumerable).IsAssignableFrom(t))
                    SlurpRKsFromIEnumerable(key + "." + f, (IEnumerable)field[f].Value);
                else if (typeof(AApiVersionedFields).IsAssignableFrom(t))
                    SlurpRKsFromField(key + "." + f, (AApiVersionedFields)field[f].Value);
            }
        }
        private void SlurpRKsFromIEnumerable(string key, IEnumerable list)
        {
            if (list == null) return;
            int i = 0;
            foreach (object o in list)
            {
                if (typeof(AApiVersionedFields).IsAssignableFrom(o.GetType()))
                {
                    SlurpRKsFromField(key + "[" + i + "]", (AApiVersionedFields)o);
                    i++;
                }
                else if (typeof(IEnumerable).IsAssignableFrom(o.GetType()))
                {
                    SlurpRKsFromIEnumerable(key + "[" + i + "]", (IEnumerable)o);
                    i++;
                }
            }
        }
        private void SlurpRKsFromXML(string key, SpecificResource item)
        {
            int j = 0;
            StreamReader sr = new StreamReader(item.Resource.Stream, true);
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                int i = line.IndexOf("key:");
                while (i >= 0 && i + 38 < line.Length)
                {
                    TGIN field = new TGIN();
                    if (uint.TryParse(line.Substring(i + 4, 8), System.Globalization.NumberStyles.HexNumber, null, out field.ResType)
                        && uint.TryParse(line.Substring(i + 13, 8), System.Globalization.NumberStyles.HexNumber, null, out field.ResGroup)
                        && ulong.TryParse(line.Substring(i + 22, 16), System.Globalization.NumberStyles.HexNumber, null, out field.ResInstance))
                        Add(key + "[" + (j++) + "]", (AResourceKey)field);
                    i = line.IndexOf("key:", i + 38);
                }
            }
        }

        private void SlurpKindred(string key, Predicate<IResourceIndexEntry> Match)
        {
            List<SpecificResource> seen = new List<SpecificResource>();
            int i = 0;
            foreach (var ppt in FileTable.fb0)
                foreach (var sr in ppt.FindAll(x => Match(x) && !seen.Exists(s => new RK(x).Equals(s.ResourceIndexEntry))))
                {
                    seen.Add(sr);
                    Add(key + "[" + i + "]", sr.ResourceIndexEntry);
                    i++;
                }
        }
        #endregion

        #region Steps
        SpecificResource objkItem;
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

            switch (item.CType)
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
            }
            lastInChain = stepList == null ? -1 : (stepList.IndexOf(lastStepInChain) + 1);
        }

        bool NotJustSelf() { return isCreateNewPackage || cloneFixOptions.IsRepair || cloneFixOptions.IsRenumber; }
        bool WantThumbs() { return cloneFixOptions.IsIncludeThumbnails || (!(isCreateNewPackage || cloneFixOptions.IsRepair) && cloneFixOptions.IsRenumber); }

        void ThumbnailsOnly_Steps(List<Step> stepList, out Step lastStepInChain)
        {
            Diagnostics.Log("ThumbnailsOnly_Steps");
            lastStepInChain = None;
            if (NotJustSelf())
            {
                stepList.AddRange(new Step[] {
                });
                if (WantThumbs())
                    stepList.Add(SlurpThumbnails);
            }
        }

        void brush_Steps(List<Step> stepList, out Step lastStepInChain)
        {
            Diagnostics.Log("brush_Steps");
            ThumbnailsOnly_Steps(stepList, out lastStepInChain);
            if (NotJustSelf())
            {
                if (isDeepClone)
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
            if (NotJustSelf())
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
            if (NotJustSelf())
            {
                stepList.AddRange(new Step[] {
                    Catlg_getVPXYs,
                    Catlg_addVPXYs,

                    VPXYs_SlurpRKs,
                    VPXYs_getMODLs,
                    VPXYs_getKinMTST,
                    VPXYs_getKinXML,
                    VPXYKinMTST_SlurpRKs,

                    MODLs_SlurpRKs,
                    MODLs_SlurpMLODs,
                });
                lastStepInChain = MODLs_SlurpMLODs;
                if (isDeepClone)
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
                }
                if (WantThumbs())
                    stepList.Add(SlurpThumbnails);
            }
        }

        void OBJD_Steps(List<Step> stepList, out Step lastStepInChain)
        {
            Diagnostics.Log("OBJD_Steps");
            CatlgHasVPXY_Steps(stepList, out lastStepInChain);
            if (NotJustSelf())
            {
                stepList.Remove(Catlg_getVPXYs);
                stepList.InsertRange(stepList.IndexOf(Catlg_addVPXYs), new Step[] {
                    // OBJD_setFallback if cloning from game
                    OBJD_getOBJK,
                    // OBJD_addOBJKref and OBJD_SlurpDDSes if default resources only
                    OBJK_SlurpRKs,
                    OBJK_getSPT2,
                    OBJK_getVPXYs,
                });
                if (!isFix && mode == Mode.FromGame)
                {
                    stepList.Insert(stepList.IndexOf(OBJD_getOBJK), OBJD_setFallback);
                }
                if (isDeepClone)
                {
                }
                else
                {
                    stepList.InsertRange(stepList.IndexOf(OBJK_SlurpRKs), new Step[] {
                        OBJD_addOBJKref,
                        Catlg_IncludePresets,
                        OBJD_SlurpDDSes,
                    });
                }
            }
        }

        void CWAL_Steps(List<Step> stepList, out Step lastStepInChain)
        {
            Diagnostics.Log("CWAL_Steps");
            CatlgHasVPXY_Steps(stepList, out lastStepInChain);
            if (NotJustSelf())
            {
                if (isDeepClone)
                {
                }
                else
                {
                    stepList.InsertRange(stepList.IndexOf(Catlg_getVPXYs), new Step[] {
                        Catlg_IncludePresets,
                    });
                }
                if (WantThumbs())
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
            if (WantThumbs())
                stepList.Add(SlurpThumbnails);//For the CFIR itself
            lastStepInChain = None;
        }

        void MDLR_Steps(List<Step> stepList, out Step lastStepInChain)
        {
            Diagnostics.Log("MDLR_Steps");
            stepList.AddRange(new Step[] { Item_findObjds, setupObjdStepList, Modular_Main, });
            lastStepInChain = None;
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
            StepText.Add(Catlg_IncludePresets, "Include Preset Images");
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
            StepText.Add(MODLs_SlurpTXTCs, "TXTC-referenced resources");

            StepText.Add(Item_findObjds, "Find OBJDs from MDLR/CFIR");
            StepText.Add(setupObjdStepList, "Get the OBJD step list");
            StepText.Add(Modular_Main, "Drive the modular process");

            StepText.Add(SlurpThumbnails, "Add thumbnails");
            StepText.Add(CWAL_SlurpThumbnails, "Add thumbnails");
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
                SpecificResource vpxy = new SpecificResource(FileTable.fb0, rk);
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
            objkItem = new SpecificResource(FileTable.fb0, objkTGI);
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
                SpecificResource spt2 = new SpecificResource(FileTable.fb0, new RK(rk) { ResourceType = 0x021D7E8C });//SPT2
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
                SpecificResource vpxy = new SpecificResource(FileTable.fb0, rk);
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
        void VPXYs_getKinMTST() { Diagnostics.Log("VPXYs_getKinMTST"); VPXYs_getKin("VPXYs_getKinMTST", 0x02019972, "mtst"); }
        void VPXYs_getKinXML() { Diagnostics.Log("VPXYs_getKinXML"); VPXYs_getKin("VPXYs_getKinXML", 0x0333406C, "PresetXML"); }
        void VPXYKinMTST_SlurpRKs() { Diagnostics.Log("VPXYKinMTST_SlurpRKs"); VPXYKin_SlurpRKs(0x02019972, "mtst"); }
        void VPXYKinXML_SlurpRKs() { Diagnostics.Log("VPXYKinXML_SlurpRKs"); VPXYKin_SlurpRKs(0x0333406C, "PresetXML"); }
        bool IsVPXYKin(IResourceKey rk, uint type) { return rk.ResourceType == type && vpxyItems.Exists(item => item.ResourceIndexEntry.Instance == rk.Instance); }
        void VPXYs_getKin(string log, uint type, string suffix)
        {
            for (int i = 0; i < vpxyItems.Count; i++)
            {
                Diagnostics.Log(String.Format(log + ": 0x{0:X8}-*-0x{1:X16}", type, vpxyItems[i].ResourceIndexEntry.Instance));
                SlurpKindred("vpxy[" + i + "]." + suffix, rie => IsVPXYKin(rie, type));
            }
        }
        void VPXYKin_SlurpRKs(uint type, string prefix)
        {
            int i = 0;
            foreach (var ppt in FileTable.fb0)
                foreach (var item in ppt.FindAll(rie => IsVPXYKin(rie, type)).FindAll(sr => sr.Resource != null))
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
                        SpecificResource modl = new SpecificResource(FileTable.fb0, tgib);
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

        void MODLs_SlurpRKs() { Diagnostics.Log("MODLs_SlurpRKs"); for (int i = 0; i < modlItems.Count; i++) SlurpRKsFromField("modl[" + i + "]", (AResource)modlItems[i].Resource); }
        void MODLs_SlurpMLODs()
        {
            Diagnostics.Log("MODLs_SlurpMLODs");
            int k = 0;
            string s = "";
            for (int i = 0; i < modlItems.Count; i++)
            {
                bool found = false;
                GenericRCOLResource rcol = (modlItems[i].Resource as GenericRCOLResource);
                for (int j = 0; j < rcol.Resources.Count; j++)
                {
                    TGIBlock tgib = rcol.Resources[j];
                    if (tgib.ResourceType != 0x01D10F34) continue;
                    SlurpRKsFromRK("modl[" + i + "].mlod[" + k + "]", tgib);
                    k++;
                    found = true;
                }
                if (!found)
                    s += String.Format("MODL {0} has no MLOD items\n", (IResourceKey)modlItems[i].ResourceIndexEntry);
            }
            Diagnostics.Show(s, "No MLOD items");
        }

        //This slurps all the RKs out of the TXTCs so the references get pulled in
        void MODLs_SlurpTXTCs()
        {
            Diagnostics.Log("MODLs_SlurpMLODs");
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


        List<SpecificResource> objdList;
        void Item_findObjds()
        {
            Diagnostics.Log("Item_findObjds");
            objdList = new List<SpecificResource>();
            string s = "";
            foreach (IResourceKey rk in (TGIBlockList)selectedItem.Resource["TGIBlocks"].Value)
            {
                if (rk.ResourceType != 0x319E4F1D) continue;
                SpecificResource objd = new SpecificResource(FileTable.fb0, rk);
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
                else if (selectedItem.CType == CatalogType.CatalogRoofPattern)
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
                foreach (var ppt in FileTable.tmb)
                    foreach (var sr in ppt.FindAll(rie => rie.ResourceType == type && rie.Instance == selectedItem.ResourceIndexEntry.Instance))
                    {
                        if (seen.Exists(sr.ResourceIndexEntry.Equals)) continue;
                        Add(size + "[" + i++ + "]Thumb", sr.RequestedRK);
                    }
            }
        }
        #endregion

        #region Make Unique bits
        string uniqueObject = null;
        string UniqueObject
        {
            get
            {
                if (uniqueObject == null)
                {
                    if (cloneFixOptions.UniqueName == null || cloneFixOptions.UniqueName.Length == 0)
                    {
                        StringInputDialog ond = new StringInputDialog();
                        ond.Caption = "Unique Resource Name";
                        ond.Prompt = "Enter unique identifier";
                        ond.Value = cloneFixOptions.UniqueName;
                        DialogResult dr = ond.ShowDialog();
                        if (dr == DialogResult.OK) uniqueObject = ond.Value;
                    }
                    else uniqueObject = cloneFixOptions.UniqueName;
                }
                return uniqueObject;
            }
        }

        Dictionary<IResourceKey, SpecificResource> rkToItem;
        Dictionary<ulong, ulong> oldToNew;
        ulong nameGUID, newNameGUID;
        ulong descGUID, newDescGUID;
        void GenerateNewIIDs()
        {
            Diagnostics.Log("GenerateNewIIDs");
            // A list of the TGIs we are going to renumber and the resource that "owns" them
            rkToItem = new Dictionary<IResourceKey, SpecificResource>();

            // We need to process anything we found in the previous steps
            foreach (var kvp in rkLookup)
            {
                if (kvp.Value == RK.NULL) continue;
                if (rkToItem.ContainsKey(kvp.Value)) continue; // seen this TGI before
                SpecificResource item = new SpecificResource(FileTable.fb0, kvp.Value);
                if (item.ResourceIndexEntry == null) continue; // TGI is not a packed resource
                rkToItem.Add(kvp.Value, item);
            }

            // We need to process STBLs
            foreach (var sr in FileTable.Current.FindAll(rie => rie.ResourceType == 0x220557DA && !rkToItem.ContainsKey(rie)))
                rkToItem.Add(sr.ResourceIndexEntry, sr);

            // We may also need to process RCOL internal chunks and NameMaps but only if we're renumbering
            if (cloneFixOptions.IsRenumber)
            {
                //If there are internal chunk references not covered by the above, we also need to add them
                Dictionary<IResourceKey, SpecificResource> rcolChunks = new Dictionary<IResourceKey, SpecificResource>();
                foreach (var kvp in rkToItem)
                {
                    if (!typeof(GenericRCOLResource).IsAssignableFrom(kvp.Value.Resource.GetType())) continue;

                    foreach (GenericRCOLResource.ChunkEntry chunk in (kvp.Value.Resource as GenericRCOLResource).ChunkEntries)
                    {
                        if (chunk.TGIBlock == RK.NULL) continue;
                        if (rkToItem.ContainsKey(chunk.TGIBlock)) continue; // External reference and we've seen it
                        if (rcolChunks.ContainsKey(chunk.TGIBlock)) continue; // Internal reference and we've seen it
                        rcolChunks.Add(chunk.TGIBlock, kvp.Value);
                    }
                }
                foreach (var kvp in rcolChunks) rkToItem.Add(kvp.Key, kvp.Value);

                // Add newest namemap
                FileTable.Current.FindAll(rie => rie.ResourceType == 0x0166038C && !rkToItem.ContainsKey(rie))
                    .ForEach(sr => rkToItem.Add(sr.ResourceIndexEntry, sr));
            }

            // A list to hold the new numbers
            oldToNew = new Dictionary<ulong, ulong>();

            if (cloneFixOptions.IsRenumber && selectedItem.CType == CatalogType.ModularResource)
                oldToNew.Add(selectedItem.ResourceIndexEntry.Instance, FNV64.GetHash(UniqueObject));//MDLR needs its IID as a specific hash value

            ulong PngInstance = 0;
            if (selectedItem.CType != CatalogType.ModularResource)
                PngInstance = (ulong)selectedItem.Resource["CommonBlock.PngInstance"].Value;

            if (cloneFixOptions.IsRenumber || !cloneFixOptions.IsKeepSTBLIIDs)
            {
                // Generate new numbers for everything we've decided to renumber

                // Renumber the PNGInstance we're referencing
                if (cloneFixOptions.IsRenumber && PngInstance != 0)
                    oldToNew.Add(PngInstance, CreateInstance());

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
                        else if (cloneFixOptions.IsRenumber)
                        {
                            if (cloneFixOptions.Is32bitIIDs &&
                                (rk.ResourceType == (uint)CatalogType.CatalogObject || rk.ResourceType == 0x02DC343F))//OBJD&OBJK
                                oldToNew.Add(rk.Instance, CreateInstance32());
                            else
                                oldToNew.Add(rk.Instance, CreateInstance());
                        }
                    }
                if (cloneFixOptions.IsRenumber)
                    foreach (IResourceKey rk in rkToItem.Keys)//Requested RK
                        if (!oldToNew.ContainsKey(rk.Instance))//Find those references we don't have new IIDs for
                        {
                            if (rk.ResourceType == 0x736884F1 && rk.Instance >> 32 == 0)//Either it's a request for a VPXY using version...
                                oldToNew.Add(rk.Instance, oldToNew[rkToItem[rk].ResourceIndexEntry.Instance]);//So add the new number for that resource
                            else//Or it's an RCOL chunk that needs a new IID...
                                oldToNew.Add(rk.Instance, CreateInstance());//So renumber it
                        }
            }

            SpecificResource catlgItem = selectedItem.CType == CatalogType.ModularResource
                ? ItemForTGIBlock0(selectedItem)
                : selectedItem;

            if (catlgItem.ResourceIndexEntry != null)
            {
                nameGUID = (ulong)catlgItem.Resource["CommonBlock.NameGUID"].Value;
                descGUID = (ulong)catlgItem.Resource["CommonBlock.DescGUID"].Value;

                if (cloneFixOptions.IsRenumber)
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
            GenerateNewIIDs();

            DoWait("Please wait, updating your package...");

            Dictionary<IResourceKey, SpecificResource> rkToItemAdded = new Dictionary<IResourceKey, SpecificResource>();

            SpecificResource catlgRes = (selectedItem.CType == CatalogType.ModularResource || selectedItem.CType == CatalogType.CatalogFireplace)
                ? ItemForTGIBlock0(selectedItem) : selectedItem;
            try
            {
                //Need to take a copy of the ResourceList as it can get modified, which messes up the enumerator
                List<SpecificResource> lsr =
                    new List<SpecificResource>(FileTable.Current.Package.GetResourceList.ConvertAll<SpecificResource>(rie => new SpecificResource(FileTable.Current, rie)));

                foreach (SpecificResource item in lsr)
                {
                    bool dirty = false;

                    if ((item.RequestedRK.Equals(selectedItem.RequestedRK) && item.CType != CatalogType.ModularResource)//Selected CatlgItem
                        || item.CType == CatalogType.CatalogObject//all OBJDs (i.e. from MDLR or CFIR)
                        || item.CType == CatalogType.CatalogTerrainPaintBrush//all CTPTs (i.e. pair of selectedItem)
                        )
                    {
                        #region Selected CatlgItem; all OBJD (i.e. from MDLR or CFIR)
                        AHandlerElement commonBlock = ((AHandlerElement)item.Resource["CommonBlock"].Value);

                        #region Selected CatlgItem || all MDLR OBJDs || both CTPTs || 0th CFIR OBJD
                        if (item.RequestedRK.Equals(selectedItem.RequestedRK)//Selected CatlgItem
                            || selectedItem.CType == CatalogType.ModularResource//all MDLR OBJDs
                            || selectedItem.CType == CatalogType.CatalogTerrainPaintBrush//both CTPTs
                            || (selectedItem.CType == CatalogType.CatalogFireplace && item.RequestedRK.Equals(catlgRes.RequestedRK))//0th CFIR OBJD
                            )
                        {
                            commonBlock["NameGUID"] = new TypedValue(typeof(ulong), newNameGUID);
                            commonBlock["DescGUID"] = new TypedValue(typeof(ulong), newDescGUID);
                            commonBlock["Price"] = new TypedValue(typeof(float), float.Parse(tbPrice.Text));

                            if (cloneFixOptions.IsRenumber)
                            {
                                commonBlock["Name"] = new TypedValue(typeof(string), "CatalogObjects/Name:" + UniqueObject);
                                commonBlock["Desc"] = new TypedValue(typeof(string), "CatalogObjects/Description:" + UniqueObject);
                            }
                        }
                        #endregion

                        if (item.RequestedRK.Equals(selectedItem.RequestedRK)//Selected CatlgItem
                            || (selectedItem.CType == CatalogType.ModularResource && item.RequestedRK.Equals(catlgRes.RequestedRK))//0th MDLR OBJD
                            //-- only selected CTPT
                            //-- none of the OBJDs for CFIR
                            )
                        {
                            commonBlock["BuildBuyProductStatusFlags"] = new TypedValue(commonBlock["BuildBuyProductStatusFlags"].Type, Convert.ToByte(tbProductStatus.Text, tbProductStatus.Text.StartsWith("0x") ? 16 : 10));
                        }

                        #region Selected CatlgItem; 0th OBJD from MDLR or CFIR
                        if (item.RequestedRK.Equals(selectedItem.RequestedRK)//Selected CatlgItem
                            || (selectedItem.CType == CatalogType.ModularResource && item.RequestedRK.Equals(catlgRes.RequestedRK))//0th MDLR OBJD
                            //-- only selected CTPT
                            || (selectedItem.CType == CatalogType.CatalogFireplace && item.RequestedRK.Equals(catlgRes.RequestedRK))//0th CFIR OBJD
                            )
                        {
                            ulong PngInstance = (ulong)commonBlock["PngInstance"].Value;
                            bool isPng = PngInstance != 0;

                            if (cloneFixOptions.IsIncludeThumbnails)
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
                            if (selectedItem.CType == CatalogType.CatalogObject)//Selected OBJD only
                            {
                                foreach (flagField ff in flagFields)
                                {
                                    ulong old = getFlags(item.Resource as AResource, ff.field);
                                    ulong mask = (ulong)0xFFFFFFFF << ff.offset;
                                    ulong res = getFlags(ff);
                                    res |= (ulong)(old & ~mask);
                                    setFlags(item.Resource as AResource, ff.field, res);
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

                        if (cloneFixOptions.IsRenumber)
                        {
                            #region Keep brushes together
                            if (item.CType == CatalogType.CatalogTerrainPaintBrush)//Both CTPTs
                            {
                                byte status = (byte)commonBlock["BuildBuyProductStatusFlags"].Value;
                                uint brushIndex = FNV32.GetHash(UniqueObject) << 1;
                                if ((status & 0x01) != 0)
                                    item.Resource["CommonBlock.UISortPriority"] = new TypedValue(typeof(uint), brushIndex);
                                else
                                    item.Resource["CommonBlock.UISortPriority"] = new TypedValue(typeof(uint), brushIndex + 1);
                            }
                            #endregion

                            if (item.CType == CatalogType.CatalogObject)
                            {
                                #region Avoid renumbering Fallback TGI
                                int fallbackIndex = (int)(uint)item.Resource["FallbackIndex"].Value;
                                TGIBlockList tgiBlocks = item.Resource["TGIBlocks"].Value as TGIBlockList;
                                AResourceKey fallbackRK = new TGIBlock(0, null, tgiBlocks[fallbackIndex]);

                                UpdateRKsFromField((AResource)item.Resource);

                                tgiBlocks[fallbackIndex] = new TGIBlock(0, null, (IResourceKey)fallbackRK);
                                #endregion
                            }
                            else
                                UpdateRKsFromField((AResource)item.Resource);
                        }

                        dirty = true;
                        #endregion
                    }
                    else if (item.ResourceIndexEntry.ResourceType == 0x0166038C)
                    {
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
                    }
                    else if (item.ResourceIndexEntry.ResourceType == 0x220557DA)//STBL
                    {
                        dirty |= UpdateStbl(tbCatlgName.Text, nameGUID, item, newNameGUID);
                        dirty |= UpdateStbl(tbCatlgDesc.Text, descGUID, item, newDescGUID);
                    }
                    else if (item.ResourceIndexEntry.ResourceType == 0x0333406C)
                    {
                        dirty = UpdateRKsFromXML(item);
                    }
                    else
                    {
                        dirty = UpdateRKsFromField((AResource)item.Resource);
                    }

                    if (dirty) item.Commit();

                }

                if (isPadSTBLs)
                {
                    PadStbls(tbCatlgName.Text, nameGUID, newNameGUID);
                    PadStbls(tbCatlgDesc.Text, descGUID, newDescGUID);
                    NameMap.NMap.Commit();
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
                StopWait();
            }

            CloseCurrent();//needed
            Diagnostics.Log("...done..!");
            DisplayNothing();
            CopyableMessageBox.Show("OK", myName, CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Information);
        }

        private bool UpdateRKsFromField(AApiVersionedFields field)
        {
            bool dirty = false;

            Type t = field.GetType();
            if (typeof(IResourceKey).IsAssignableFrom(t))
            {
                IResourceKey rk = (IResourceKey)field;
                if (rk != RK.NULL && rkToItem.ContainsKey(rk) && oldToNew.ContainsKey(rk.Instance)) { rk.Instance = oldToNew[rk.Instance]; dirty = true; }
            }
            else
            {
                if (typeof(IEnumerable).IsAssignableFrom(field.GetType()))
                    dirty = UpdateRKsFromIEnumerable((IEnumerable)field) || dirty;
                dirty = UpdateRKsFromAApiVersionedFields(field) || dirty;
            }

            return dirty;
        }
        private bool UpdateRKsFromAApiVersionedFields(AApiVersionedFields field)
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
                    dirty = UpdateRKsFromIEnumerable((IEnumerable)field[f].Value) || dirty;
                else if (typeof(AApiVersionedFields).IsAssignableFrom(t))
                    dirty = UpdateRKsFromField((AApiVersionedFields)field[f].Value) || dirty;
            }

            return dirty;
        }
        private bool UpdateRKsFromIEnumerable(IEnumerable list)
        {
            bool dirty = false;

            if (list != null)
                foreach (object o in list)
                    if (typeof(AApiVersionedFields).IsAssignableFrom(o.GetType()))
                        dirty = UpdateRKsFromField((AApiVersionedFields)o) || dirty;
                    else if (typeof(IEnumerable).IsAssignableFrom(o.GetType()))
                        dirty = UpdateRKsFromIEnumerable((IEnumerable)o) || dirty;

            return dirty;
        }
        private bool UpdateRKsFromXML(SpecificResource item)
        {
            bool dirty = false;
            StreamReader sr = new StreamReader(item.Resource.Stream, true);
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms, sr.CurrentEncoding);
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                int i = line.IndexOf("key:");
                while (i >= 0 && i + 22 + 16 < line.Length)
                {
                    string iid = line.Substring(i + 22, 16);
                    ulong IID;
                    if (ulong.TryParse(iid, System.Globalization.NumberStyles.HexNumber, null, out IID))
                    {
                        if (oldToNew.ContainsKey(IID))
                        {
                            string newiid = oldToNew[IID].ToString("X16");
                            line = line.Replace(iid, newiid);
                            dirty = true;
                        }
                    }
                    i = line.IndexOf("key:", i + 22 + 16);
                }
                sw.WriteLine(line);
            }
            sw.Flush();
            if (dirty)
            {
                item.Resource.Stream.SetLength(0);
                item.Resource.Stream.Write(ms.ToArray(), 0, (int)ms.Length);
            }
            return dirty;
        }

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
                new List<IResourceIndexEntry>(FileTable.Current.Package.FindAll(x => x.ResourceType == 0x220557DA))
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
            SpecificResource newstbl = new SpecificResource(FileTable.fb0, newRK);
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

        #region Saving bits
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
                target, disableCompression, isPadSTBLs, false/*mode == Mode.FromGame/**/, null, //cloneFixOptions.IsExcludeCommon ? lS3ocResourceList : null,
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
                    fileReOpenToFix(target.Path, selectedItem.CType);
                }
                else
                {
                    if (File.Exists(target.Path))
                        CopyableMessageBox.Show("\nSave not complete.\nPlease ensure package is not in use.\n", myName, CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Warning);
                    else
                        CopyableMessageBox.Show("\nSave not complete.\n", myName, CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Warning);
                    DisplayOptions();
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
                FileTable.Current, disableCompression, isPadSTBLs, false/*mode == Mode.FromGame/**/, null, //cloneFixOptions.IsExcludeCommon ? lS3ocResourceList : null,
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
    }

    static class Extensions
    {
        public static bool Contains(this IEnumerable<string> haystack, string needle) { foreach (var x in haystack) if (x.Equals(needle)) return true; return false; }
    }
}
