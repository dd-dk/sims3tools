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
using System.IO;
using System.Threading;
using System.Windows.Forms;
using s3pi.Interfaces;
using ObjectCloner.TopPanelComponents;


//Type: 0x00B2D882 resources are in Fullbuild2, everything else is in Fullbuild0


namespace ObjectCloner
{
    public partial class MainForm : Form
    {
        #region Static bits
        static string myName = "s3pi Object Cloner";
        static Dictionary<View, MenuBarWidget.MB> viewMap;
        static List<View> viewMapKeys;
        static List<MenuBarWidget.MB> viewMapValues;

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
            Step5,//Bring in all the resources in the TGI blocks of the MODL
            Step6,//Bring in all the resources in the TGI blocks of each MLOD (disregard duplicates)
            Step7,//Bring in all resources from ...\The Sims 3\Thumbnails\ALLThumbnails.package that match the instance number of the OBJD
            Last = 7,
        }

        SubPage subPage = SubPage.None;
        View currentView;
        IPackage pkg = null;

        private bool haveLoaded = false;
        private ObjectCloner.TopPanelComponents.ObjectChooser objectChooser;
        private ObjectCloner.TopPanelComponents.ResourceList resourceList;
        public MainForm()
        {
            InitializeComponent();
            objectChooser = new ObjectChooser();
            objectChooser.SelectedIndexChanged += new EventHandler(objectChooser_SelectedIndexChanged);
            resourceList = new ResourceList();

            this.Text = myName;
            menuBarWidget1.Enable(MenuBarWidget.MD.MBV, false);
            MainForm_LoadFormSettings();

            objectChooser.SmallImageList = new ImageList();
            objectChooser.SmallImageList.ImageSize = new System.Drawing.Size(32, 32);
            objectChooser.LargeImageList = new ImageList();
            objectChooser.LargeImageList.ImageSize = new System.Drawing.Size(64, 64);

            InitialiseTabs();
            setButtons(Page.None, SubPage.None);
        }



        private void MainForm_LoadFormSettings()
        {
            currentView = Enum.IsDefined(typeof(View), ObjectCloner.Properties.Settings.Default.View)
                ? (View)ObjectCloner.Properties.Settings.Default.View
                : View.Details;

            int h = ObjectCloner.Properties.Settings.Default.PersistentHeight;
            if (h == -1) h = 4 * System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height / 5;
            this.Height = h;

            int w = ObjectCloner.Properties.Settings.Default.PersistentWidth;
            if (w == -1) w = 4 * System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width / 5;
            this.Width = w;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            AbortLoading();
            if (pkg != null) s3pi.Package.Package.ClosePackage(0, pkg);

            objectChooser.ObjectChooser_SaveSettings();

            ObjectCloner.Properties.Settings.Default.PersistentHeight = this.WindowState == FormWindowState.Normal ? this.Height : -1;
            ObjectCloner.Properties.Settings.Default.PersistentWidth = this.WindowState == FormWindowState.Normal ? this.Width : -1;
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

        #region TopPanelComponents
        bool waitingToDisplayObjectChooser;
        private void DisplayObjectChooser()
        {
            waitingToDisplayObjectChooser = false;
            splitContainer1.Panel1.Controls.Clear();
            splitContainer1.Panel1.Controls.Add(objectChooser);
            objectChooser.Dock = DockStyle.Fill;
            setButtons(Page.Choose, subPage);
        }

        bool waitingToDisplayResources;
        private void DisplayResources()
        {
            waitingToDisplayResources = false;
            splitContainer1.Panel1.Controls.Clear();
            resourceList.Page = "" + subPage;
            splitContainer1.Panel1.Controls.Add(resourceList);
            objectChooser.Dock = DockStyle.Fill;
            setButtons(Page.Listing, subPage);
        }
        #endregion

        #region Loading thread
        Thread loadThread;
        bool loading;
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
            waitingToDisplayObjectChooser = false;
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

                if (waitingToDisplayObjectChooser) DisplayObjectChooser();

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
            string objdtag = ((AApiVersionedFields)objd.ObjD["CommonBlock"].Value)["Name"];
            lvi.Text = (objdtag.IndexOf(':') < 0) ? objdtag : objdtag.Substring(objdtag.LastIndexOf(':') + 1);
            objdtag = ((AApiVersionedFields)objd.ObjD["CommonBlock"].Value)["Desc"];
            lvi.SubItems.AddRange(new string[]{
                (objdtag.IndexOf(':') < 0) ? objdtag : objdtag.Substring(objdtag.LastIndexOf(':') + 1),
                new TGI(objd.rieObjD),
                ""
            });
            if (objd.Thum32 != null) { LItoIMG32.Add(objectChooser.Items.Count, objectChooser.SmallImageList.Images.Count); objectChooser.SmallImageList.Images.Add(objd.Thum32); }
            if (objd.Thum64 != null) { LItoIMG64.Add(objectChooser.Items.Count, objectChooser.LargeImageList.Images.Count); objectChooser.LargeImageList.Images.Add(objd.Thum64); }

            objectChooser.Items.Add(lvi);
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

        public class BoolEventArgs : EventArgs
        {
            public bool arg;
            public BoolEventArgs(bool arg) { this.arg = arg; }
        }
        private event EventHandler<BoolEventArgs> LoadingComplete;
        public delegate void loadingCompleteCallback(IPackage pkg, bool complete);
        public void OnLoadingComplete(IPackage pkg, bool complete) { this.pkg = pkg; if (LoadingComplete != null) { LoadingComplete(this, new BoolEventArgs(complete)); } }

        public delegate bool stopLoadingCallback();
        private bool stopLoading()
        {
            return !loading;
        }
        #endregion

        #region ObjectChooser
        private void objectChooser_SelectedIndexChanged(object sender, EventArgs e)
        {
            subPage = SubPage.None;
            if (objectChooser.SelectedItems.Count == 0) ClearTabs();
            else FillTabs((TGI)objectChooser.SelectedItems[0].SubItems[2].Text);
            setButtons(Page.Choose, subPage);
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

        void InitialiseOverviewTab(IResource res)
        {
            Dictionary<string, Type> types = AApiVersionedFields.GetContentFieldTypes(0, res.GetType());
            foreach (string field in overviewTabFields)
            {
                CreateField(tlpOverviewMain, types[field], res as AResource, field);
            }

            AApiVersionedFields common = res["CommonBlock"].Value as AApiVersionedFields;
            types = AApiVersionedFields.GetContentFieldTypes(0, res["CommonBlock"].Type);
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
        void FillTabs(TGI tgi)
        {
            RIE rie = new RIE(pkg, tgi);
            IResource res = s3pi.WrapperDealer.WrapperDealer.GetResource(0, pkg, rie.rie);
            for (int i = 1; i < tlpOverviewMain.RowCount - 1; i++)
            {
                Label lb = (Label)tlpOverviewMain.GetControlFromPosition(0, i);
                TextBox tb = (TextBox)tlpOverviewMain.GetControlFromPosition(1, i);

                TypedValue tv = res[lb.Text];
                tb.Text = tv;
            }
            for (int i = 2; i < tlpOverviewCommon.RowCount - 1; i++)
            {
                Label lb = (Label)tlpOverviewCommon.GetControlFromPosition(0, i);
                TextBox tb = (TextBox)tlpOverviewCommon.GetControlFromPosition(1, i);

                TypedValue tv = ((AApiVersionedFields)res["CommonBlock"].Value)[lb.Text];
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
                    //case MenuBarWidget.MB.MBV_showImage: viewShowImage(); break;
                }
            }
            finally { this.Enabled = true; }
        }

        private void viewSetView(MenuBarWidget.MB mn, Dictionary<int, int> imageMap)
        {
            if (menuBarWidget1.IsChecked(mn)) return;
            if (viewMap.ContainsKey(currentView)) menuBarWidget1.Checked(viewMap[currentView], false);
            menuBarWidget1.Checked(mn, true);

            for (int i = 0; i < objectChooser.Items.Count; i++)
                objectChooser.Items[i].ImageIndex = imageMap.ContainsKey(i) ? imageMap[i] : -1;

            ObjectCloner.Properties.Settings.Default.View = viewMapValues.IndexOf(mn);
            currentView = viewMapKeys[ObjectCloner.Properties.Settings.Default.View];
            objectChooser.View = currentView;
        }

        private void viewShowImage()
        {
            //toggle check mark
            //if checked
            //  if images not loaded: disable the menu option, start the image loader thread
            //  else run viewSetImage with the appropriate image map (hmm, may need to pull the code out)
            //else
            //  set all ImageIndex entries to -1
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
                    btnNext.Enabled = false;
                }
                else
                {
                    if (s != SubPage.Last)
                    {
                        btnNextBackColor = Color.FromKnownColor(KnownColor.ControlLightLight);
                        btnNext.Enabled = true;
                    }
                    else
                    {
                        btnNext.Enabled = false;
                        btnSaveBackColor = Color.FromKnownColor(KnownColor.ControlLightLight);
                    }
                }
            }
            else
            {
                btnClone.Enabled = false;
                btnNext.Enabled = false;
            }
            btnChoose.BackColor = btnChooseBackColor;
            btnClone.BackColor = btnCloneBackColor;
            btnNext.BackColor = btnNextBackColor;
            btnSave.BackColor = btnSaveBackColor;
        }

        private void btnChoose_Click(object sender, EventArgs e)
        {
            if (!haveLoaded && !setPackageFile()) return;
            btnChoose.Enabled = false;
            Application.DoEvents();
            if (!haveLoaded)
            {
                waitingToDisplayObjectChooser = true;
                StartLoading();
            }
            else
                DisplayObjectChooser();
        }

        private void btnClone_Click(object sender, EventArgs e)
        {
            btnClone.Enabled = false;
            waitingToDisplayResources = true;
            subPage = SubPage.Step1;
            DisplayResources();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            btnNext.Enabled = false;
            waitingToDisplayResources = true;
            subPage = (SubPage)((int)subPage) + 1;
            DisplayResources();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            try
            {
                DialogResult dr = saveFileDialog1.ShowDialog();
                if (dr != DialogResult.OK) return;
                //...
            }
            finally { this.Enabled = true; }
        }
        #endregion
    }
}
