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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using s3pi.Interfaces;
using s3pi.Package;
using s3pi.Extensions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace S3PIDemoFE
{
    public partial class MainForm : Form
    {
        static List<string> fields = null;
        static MainForm()
        {
            fields = new List<string>();
            foreach (string s in AApiVersionedFields.GetContentFields(0, typeof(AResourceIndexEntry)))
                if (!s.Contains("Stream"))
                    fields.Add(s);
        }

        const string myName = "s3pe";
        public MainForm()
        {
            InitializeComponent();
            this.Text = myName;
            
            this.lbProgress.Text = "";

            browserWidget1.Fields = new List<string>(fields.ToArray());
            browserWidget1.ContextMenuStrip = menuBarWidget1.browserWidgetContextMenuStrip;

            List<string> filterFields = new List<string>(fields.ToArray());
            filterFields.Remove("Chunkoffset");
            filterFields.Remove("Filesize");
            filterFields.Remove("Memsize");
            resourceFilterWidget1.BrowserWidget = browserWidget1;
            resourceFilterWidget1.Fields = filterFields;

            packageInfoWidget1.Fields = packageInfoFields1.Fields;
            this.PackageFilenameChanged += new EventHandler(MainForm_PackageFilenameChanged);
            this.PackageChanged += new EventHandler(MainForm_PackageChanged);

            this.SaveSettings += new EventHandler(MainForm_SaveSettings);
            this.SaveSettings += new EventHandler(browserWidget1.BrowserWidget_SaveSettings);
            this.SaveSettings += new EventHandler(controlPanel1.ControlPanel_SaveSettings);
            //this.SaveSettings += new EventHandler(hexWidget1.HexWidget_SaveSettings);
        }

        public MainForm(params string[] args)
            :this()
        {
            MainForm_LoadFormSettings();
            CmdLine(args);//In case of conflict, command line overrides settings

            // Settings for test mode
            if (cmdlineTest)
            {
            }

        }

        void MainForm_LoadFormSettings()
        {
            int h = S3PIDemoFE.Properties.Settings.Default.PersistentHeight;
            if (h == -1) h = 4 * System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height / 5;

            int w = S3PIDemoFE.Properties.Settings.Default.PersistentWidth;
            if (w == -1) w = 4 * System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width / 5;
            this.ClientSize = new Size(w, h);

            FormWindowState s =
                Enum.IsDefined(typeof(FormWindowState), S3PIDemoFE.Properties.Settings.Default.FormWindowState)
                ? (FormWindowState)S3PIDemoFE.Properties.Settings.Default.FormWindowState
                : FormWindowState.Normal;
            this.WindowState = s;

            int s1 = S3PIDemoFE.Properties.Settings.Default.Splitter1Position;
            if (s1 >= splitContainer1.Panel1MinSize)
                splitContainer1.SplitterDistance = s1;

            int s2 = S3PIDemoFE.Properties.Settings.Default.Splitter2Position;
            if (s2 > splitContainer2.Panel1MinSize)
                splitContainer2.SplitterDistance = s2;
        }

        void MainForm_SaveSettings(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                S3PIDemoFE.Properties.Settings.Default.PersistentHeight = this.ClientSize.Height;
                S3PIDemoFE.Properties.Settings.Default.PersistentWidth = this.ClientSize.Width;
            }
            else
            {
                S3PIDemoFE.Properties.Settings.Default.PersistentHeight = -1;
                S3PIDemoFE.Properties.Settings.Default.PersistentWidth = -1;
            }
            S3PIDemoFE.Properties.Settings.Default.FormWindowState = (int)this.WindowState;
            S3PIDemoFE.Properties.Settings.Default.Splitter1Position = splitContainer1.SplitterDistance;
            S3PIDemoFE.Properties.Settings.Default.Splitter2Position = splitContainer2.SplitterDistance;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Enabled = false;
            Filename = "";
            if (CurrentPackage != null) { e.Cancel = true; this.Enabled = true; return; }

            saveSettings();
        }

        private void MainForm_PackageFilenameChanged(object sender, EventArgs e)
        {
            if (Filename.Length > 0 && File.Exists(Filename))
            {
                try
                {
                    CurrentPackage = Package.OpenPackage(0, Filename, ReadWrite);
                    menuBarWidget1.AddRecentFile((ReadWrite ? "1:" : "0:") + Filename);
                    string s = Filename;
                    if (s.Length > 128)
                    {
                        s = System.IO.Path.GetDirectoryName(s);
                        s = s.Substring(Math.Max(0, s.Length - 40));
                        s = "..." + System.IO.Path.Combine(s, System.IO.Path.GetFileName(Filename));
                    }
                    this.Text = String.Format("{0}: [{1}] {2}", myName, ReadWrite ? "RW" : "RO", s);
                }
                catch (Exception ex)
                {
                    string s = "Could not open package:\n" + Filename + "\n";
                    for (Exception inex = ex; inex != null; inex = inex.InnerException) s += "\n" + inex.Message;
                    for (Exception inex = ex; inex != null; inex = inex.InnerException) s += "\n----\nStack trace:\n" + inex.StackTrace;

                    CopyableMessageBox.Show(this, s,
                        myName, CopyableMessageBoxIcon.Error, new List<string>(new string[] { "OK" }), 0, 0);
                    Filename = "";
                }
            }
            else
            {
                this.Text = myName;
            }
        }

        private void MainForm_PackageChanged(object sender, EventArgs e)
        {
            browserWidget1.Package = packageInfoWidget1.Package = CurrentPackage;
            pnAuto.Controls.Clear();
            menuBarWidget1.Enable(MenuBarWidget.MB.MBF_saveAs, CurrentPackage != null);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBF_saveCopyAs, CurrentPackage != null);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBR_importResources, CurrentPackage != null);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBR_importPackages, CurrentPackage != null);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBF_close, CurrentPackage != null);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBF_bookmarkCurrent, CurrentPackage != null);
            menuBarWidget1.Enable(MenuBarWidget.MD.MBR, CurrentPackage != null);
            menuBarWidget1.EnableCMSBW(CurrentPackage != null);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBT_search, CurrentPackage != null);
            resourceDropDownOpening();
        }

        public event EventHandler SaveSettings;
        protected virtual void OnSaveSettings(object sender, EventArgs e) { if (SaveSettings != null) SaveSettings(sender, e); }

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
            Options.Add("import", new CmdInfo(CmdLineImport, "Import a batch of files into a new package"));
            Options.Add("help", new CmdInfo(CmdLineHelp, "Display this help"));
        }
        void CmdLine(params string[] args)
        {
            SetOptions();
            List<string> pkgs = new List<string>();
            List<string> cmdline = new List<string>(args);
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
                            myName, CopyableMessageBoxIcon.Error, new List<string>(new string[] { "OK" }), 0, 0);
                        Environment.Exit(1);
                    }
                }
                else
                {
                    if (pkgs.Count == 0)
                        pkgs.Add(option);
                    else
                    {
                        CopyableMessageBox.Show(this, "Can only accept one package on command line",
                            myName, CopyableMessageBoxIcon.Error, new List<string>(new string[] { "OK" }), 0, 0);
                        Environment.Exit(1);
                    }
                }
            }

            foreach (string pkg in pkgs)
            {
                if (!File.Exists(pkg))
                {
                    CopyableMessageBox.Show(this, "File not found:\n" + pkg,
                        myName, CopyableMessageBoxIcon.Error, new List<string>(new string[] { "OK" }), 0, 0);
                    Environment.Exit(1);
                }
                Filename = pkg;
            }
        }
        bool cmdlineTest = false;
        bool CmdLineTest(ref List<string> cmdline) { cmdlineTest = true; return false; }
        bool CmdLineImport(ref List<string> cmdline)
        {
            if (cmdline.Count < 1)
            {
                CopyableMessageBox.Show(this, "-import requires one or more files",
                    myName, CopyableMessageBoxIcon.Error, new List<string>(new string[] { "OK" }), 0, 0);
                Environment.Exit(1);
            }
            List<string> batch = new List<string>();
            while (cmdline.Count > 0 && cmdline[0][0] != '/' && cmdline[0][0] != '-')
            {
                batch.Add(cmdline[0]);
                cmdline.RemoveAt(0);
            }

            fileNew();
            this.Show();
            importBatch(batch.ToArray());
            return false;
        }
        bool CmdLineHelp(ref List<string> cmdline)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("The following command line options are available:\n");
            foreach (var kvp in Options)
                sb.AppendFormat("{0}  --  {1}\n", kvp.Key, kvp.Value.help);
            sb.AppendLine("\nOptions must be prefixed with '/' or '-'\n\n");
            sb.AppendLine("A fully-qualified package name can also be supplied on the command line.");

            CopyableMessageBox.Show(this, sb.ToString(), "Command line options", CopyableMessageBoxIcon.Information, new List<string>(new string[] { "OK" }), 0, 0);
            return true;
        }
        #endregion


        #region Package Filename
        string filename;
        bool ReadWrite { get { return filename != null && filename.Length > 1 && filename.StartsWith("1:"); } }
        string Filename
        {
            get { return filename == null || filename.Length < 2 ? "" : filename.Substring(2); }
            set
            {
                if (filename != "" && filename == value) return;

                CurrentPackage = null;
                if (CurrentPackage != null) return;

                filename = value;
                if (!filename.StartsWith("0:") && !filename.StartsWith("1:"))
                    filename = "1:" + filename;

                OnPackageFilenameChanged(this, new EventArgs());
            }
        }
        public event EventHandler PackageFilenameChanged;
        protected virtual void OnPackageFilenameChanged(object sender, EventArgs e) { if (PackageFilenameChanged != null) PackageFilenameChanged(sender, e); }
        #endregion

        #region Current Package
        bool isPackageDirty = false;
        bool IsPackageDirty
        {
            get { return ReadWrite && isPackageDirty; }
            set
            {
                menuBarWidget1.Enable(MenuBarWidget.MB.MBF_save, ReadWrite && value);
                if (isPackageDirty == value) return;
                isPackageDirty = value;
            }
        }
        IPackage package = null;
        IPackage CurrentPackage
        {
            get { return package; }
            set
            {
                if (package == value) return;

                browserWidget1.SelectedResource = null;
                if (browserWidget1.SelectedResource != null) return;

                if (isPackageDirty)
                {
                    int res = CopyableMessageBox.Show("Current package has unsaved changes.\nSave now?",
                        myName, CopyableMessageBoxButtons.YesNoCancel, CopyableMessageBoxIcon.Warning, 2);/**///Causes error on Application.Exit();... so use this.Close();
                    if (res == 2) return;
                    if (res == 0)
                    {
                        if (ReadWrite) { if (!fileSave()) return; }
                        else { if (!fileSaveAs()) return; }
                    }
                    IsPackageDirty = false;
                }
                if (package != null) Package.ClosePackage(0, package);
                
                package = value;
                OnPackageChanged(this, new EventArgs());
            }
        }
        public event EventHandler PackageChanged;
        protected virtual void OnPackageChanged(object sender, EventArgs e) { if (PackageChanged != null) PackageChanged(sender, e); }
        #endregion

        #region Current Resource
        string resourceName = "";
        s3pi.DemoPlugins.DemoPlugins plug = null;

        Exception resException = null;
        IResource resource = null;
        bool resourceIsDirty = false;
        void resource_ResourceChanged(object sender, EventArgs e)
        {
            controlPanel1.CommitEnabled = resourceIsDirty = true;
        }
        #endregion

        #region Menu Bar
        private void menuBarWidget1_MBDropDownOpening(object sender, MenuBarWidget.MBDropDownOpeningEventArgs mn)
        {
            switch (mn.mn)
            {
                case MenuBarWidget.MD.MBF: break;
                case MenuBarWidget.MD.MBE: editDropDownOpening(); break;
                case MenuBarWidget.MD.MBR: resourceDropDownOpening(); break;
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
                    case MenuBarWidget.MB.MBF_save: fileSave(); break;
                    case MenuBarWidget.MB.MBF_saveAs: fileSaveAs(); break;
                    case MenuBarWidget.MB.MBF_saveCopyAs: fileSaveCopyAs(); break;
                    case MenuBarWidget.MB.MBF_close: fileClose(); break;
                    case MenuBarWidget.MB.MBF_setMaxRecent: fileSetMaxRecent(); break;
                    case MenuBarWidget.MB.MBF_bookmarkCurrent: fileBookmarkCurrent(); break;
                    case MenuBarWidget.MB.MBF_setMaxBookmarks: fileSetMaxBookmarks(); break;
                    case MenuBarWidget.MB.MBF_exit: fileExit(); break;
                }
            }
            finally { this.Enabled = true; }
        }

        private void fileNew()
        {
            Filename = "";
            CurrentPackage = Package.NewPackage(0);
            IsPackageDirty = true;
        }

        private void fileOpen()
        {
            openFileDialog1.FileName = "*.package";
            openFileDialog1.FilterIndex = 1;
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr != DialogResult.OK) return;

            Filename = (openFileDialog1.ReadOnlyChecked ? "0:" : "1:") + openFileDialog1.FileName;
        }

        private bool fileSave()
        {
            if (CurrentPackage == null) return false;

            if (Filename == null || Filename.Length == 0) return fileSaveAs();

            Application.UseWaitCursor = true;
            Application.DoEvents();
            try
            {
                CurrentPackage.SavePackage();
                IsPackageDirty = false;
            }
            finally { Application.UseWaitCursor = false; }
            return true;
        }

        private bool fileSaveAs()
        {
            if (CurrentPackage == null) return false;

            saveAsFileDialog.FileName = "";
            DialogResult dr = saveAsFileDialog.ShowDialog();
            if (dr != DialogResult.OK) return false;

            if (Filename != null && Filename.Length > 0 && Path.GetFullPath(saveAsFileDialog.FileName).Equals(Path.GetFullPath(Filename)))
                return fileSave();

            Application.UseWaitCursor = true;
            Application.DoEvents();
            try
            {
                CurrentPackage.SaveAs(saveAsFileDialog.FileName);
                IsPackageDirty = false;
                Filename = "1:" + saveAsFileDialog.FileName;
            }
            finally { Application.UseWaitCursor = false; }
            return true;
        }

        private void fileSaveCopyAs()
        {
            if (CurrentPackage == null) return;

            saveAsFileDialog.FileName = "";
            DialogResult dr = saveAsFileDialog.ShowDialog();
            if (dr != DialogResult.OK) return;

            Application.UseWaitCursor = true;
            Application.DoEvents();
            try { CurrentPackage.SaveAs(saveAsFileDialog.FileName); }
            finally { Application.UseWaitCursor = false; }
        }

        private void fileClose()
        {
            Filename = "";
        }

        private void menuBarWidget1_MRUClick(object sender, MenuBarWidget.MRUClickEventArgs filename)
        {
            Filename = filename.filename;
        }

        private void fileSetMaxRecent()
        {
            GetNumberDialog gnd = new GetNumberDialog("Max number of files:", "Recent Files list", 0, 9,
                S3PIDemoFE.Properties.Settings.Default.MRUListSize);
            DialogResult dr = gnd.ShowDialog();
            if (dr != DialogResult.OK) return;
            S3PIDemoFE.Properties.Settings.Default.MRUListSize = (short)gnd.Value;
        }

        private void menuBarWidget1_BookmarkClick(object sender, MenuBarWidget.BookmarkClickEventArgs filename)
        {
            Filename = filename.filename;
        }

        private void fileBookmarkCurrent()
        {
            menuBarWidget1.AddBookmark((ReadWrite ? "1:" : "0:") + Filename);
        }

        private void fileSetMaxBookmarks()
        {
            GetNumberDialog gnd = new GetNumberDialog("Max number of files:", "Bookmark list", 0, 9,
                S3PIDemoFE.Properties.Settings.Default.BookmarkSize);
            DialogResult dr = gnd.ShowDialog();
            if (dr != DialogResult.OK) return;
            S3PIDemoFE.Properties.Settings.Default.BookmarkSize = (short)gnd.Value;
        }

        private void fileExit()
        {
            this.Close();
        }
        #endregion

        #region Edit menu
        private void editDropDownOpening() { }

        private void menuBarWidget1_MBEdit_Click(object sender, MenuBarWidget.MBClickEventArgs mn)
        {
            try
            {
                this.Enabled = false;
                Application.DoEvents();
                switch (mn.mn)
                {
                    case MenuBarWidget.MB.MBE_cut: editCut(); break;
                    case MenuBarWidget.MB.MBE_copy: editCopy(); break;
                    case MenuBarWidget.MB.MBE_paste: editPaste(); break;
                }
            }
            finally { this.Enabled = true; }
        }

        private void editCut() { }

        private void editCopy() { }

        private void editPaste() { }
        #endregion

        #region Resource menu
        private void menuBarWidget1_MBResource_Click(object sender, MenuBarWidget.MBClickEventArgs mn)
        {
            try
            {
                //this.Enabled = false;
                Application.DoEvents();
                switch (mn.mn)
                {
                    case MenuBarWidget.MB.MBR_add: resourceAdd(); break;
                    case MenuBarWidget.MB.MBR_copy: resourceCopy(); break;
                    case MenuBarWidget.MB.MBR_paste: resourcePaste(); break;
                    case MenuBarWidget.MB.MBR_duplicate: resourceDuplicate(); break;
                    case MenuBarWidget.MB.MBR_compressed: resourceCompressed(); break;
                    case MenuBarWidget.MB.MBR_isdeleted: resourceIsDeleted(); break;
                    case MenuBarWidget.MB.MBR_details: resourceDetails(); break;
                    case MenuBarWidget.MB.MBR_replace: resourceReplace(); break;
                    case MenuBarWidget.MB.MBR_importResources: resourceImport(); break;
                    case MenuBarWidget.MB.MBR_importPackages: resourceImportPackages(); break;
                    case MenuBarWidget.MB.MBR_exportResources: resourceExport(); break;
                    case MenuBarWidget.MB.MBR_exportToPackage: resourceExportToPackage(); break;
                }
            }
            finally { /*this.Enabled = true;/**/ }
        }

        private void resourceDropDownOpening()
        {
            bool state;
            state = CurrentPackage != null &&
                (
                Clipboard.ContainsData(myDataFormatSingleFile)
                || Clipboard.ContainsData(myDataFormatBatch)
                || Clipboard.ContainsFileDropList()
                //|| Clipboard.ContainsText()
                );
            menuBarWidget1.Enable(MenuBarWidget.MB.MBR_paste, state);
            menuBarWidget1.Enable(MenuBarWidget.CMS.MBR_paste, state);

            CheckState res = CompressedCheckState();
            if (res == CheckState.Indeterminate)
            {
                menuBarWidget1.Indeterminate(MenuBarWidget.MB.MBR_compressed);
                menuBarWidget1.Indeterminate(MenuBarWidget.CMS.MBR_compressed);
            }
            else
            {
                menuBarWidget1.Checked(MenuBarWidget.MB.MBR_compressed, res == CheckState.Checked);
                menuBarWidget1.Checked(MenuBarWidget.CMS.MBR_compressed, res == CheckState.Checked);
            }

            res = IsDeletedCheckState();
            if (res == CheckState.Indeterminate)
            {
                menuBarWidget1.Indeterminate(MenuBarWidget.MB.MBR_isdeleted);
                menuBarWidget1.Indeterminate(MenuBarWidget.CMS.MBR_isdeleted);
            }
            else
            {
                menuBarWidget1.Checked(MenuBarWidget.MB.MBR_isdeleted, res == CheckState.Checked);
                menuBarWidget1.Checked(MenuBarWidget.CMS.MBR_isdeleted, res == CheckState.Checked);
            }
        }
        private CheckState CompressedCheckState()
        {
            if (browserWidget1.SelectedResources.Count == 0)
                return CheckState.Unchecked;
            else if (browserWidget1.SelectedResources.Count == 1)
                return browserWidget1.SelectedResource.Compressed != 0 ? CheckState.Checked : CheckState.Unchecked;

            int state = 0;
            foreach (IResourceIndexEntry rie in browserWidget1.SelectedResources) if (rie.Compressed != 0) state++;
            if (state == 0 || state == browserWidget1.SelectedResources.Count)
                return state == browserWidget1.SelectedResources.Count ? CheckState.Checked : CheckState.Unchecked;

            return CheckState.Indeterminate;
        }
        private CheckState IsDeletedCheckState()
        {
            if (browserWidget1.SelectedResources.Count == 0)
                return CheckState.Unchecked;
            else if (browserWidget1.SelectedResources.Count == 1)
                return browserWidget1.SelectedResource.IsDeleted ? CheckState.Checked : CheckState.Unchecked;

            int state = 0;
            foreach (IResourceIndexEntry rie in browserWidget1.SelectedResources) if (rie.IsDeleted) state++;
            if (state == 0 || state == browserWidget1.SelectedResources.Count)
                return state == browserWidget1.SelectedResources.Count ? CheckState.Checked : CheckState.Unchecked;
            
            return CheckState.Indeterminate;
        }

        private void resourceAdd()
        {
            ResourceDetails ir = new ResourceDetails(CurrentPackage.Find(new string[] { "ResourceType" }, new TypedValue[] { new TypedValue(typeof(uint), (uint)0x0166038C) }) != null, false);
            DialogResult dr = ir.ShowDialog();
            if (dr != DialogResult.OK) return;

            if (ir.UseName && ir.ResourceName != null && ir.ResourceName.Length > 0)
                UpdateNameMap(ir.Instance, ir.ResourceName, true, ir.AllowRename);

            IResourceIndexEntry rie = NewResource(ir.ResourceType, ir.ResourceGroup, ir.Instance, null, ir.Replace, ir.Compress);
            browserWidget1.Add(rie);
        }

        //private void resourceCut() { resourceCopy(); if (browserWidget1.SelectedResource != null) package.DeleteResource(browserWidget1.SelectedResource); }

        private void resourceCopy()
        {
            if (browserWidget1.SelectedResources.Count == 0) return;

            Application.UseWaitCursor = true;
            Application.DoEvents();
            try
            {
                if (browserWidget1.SelectedResources.Count == 1)
                {
                    myDataFormat d = new myDataFormat();
                    d.tgin = browserWidget1.SelectedResource as AResourceIndexEntry;
                    d.tgin.ResName = resourceName;
                    d.data = s3pi.WrapperDealer.WrapperDealer.GetResource(0, CurrentPackage, browserWidget1.SelectedResource, true).AsBytes;//Don't need wrapper

                    IFormatter formatter = new BinaryFormatter();
                    MemoryStream ms = new MemoryStream();
                    formatter.Serialize(ms, d);
                    DataFormats.Format f = DataFormats.GetFormat(myDataFormatSingleFile);
                    Clipboard.SetData(myDataFormatSingleFile, ms);
                }
                else
                {
                    List<myDataFormat> l = new List<myDataFormat>();
                    foreach (IResourceIndexEntry rie in browserWidget1.SelectedResources)
                    {
                        myDataFormat d = new myDataFormat();
                        d.tgin = rie as AResourceIndexEntry;
                        d.tgin.ResName = browserWidget1.ResourceName(rie);
                        d.data = s3pi.WrapperDealer.WrapperDealer.GetResource(0, CurrentPackage, rie, true).AsBytes;//Don't need wrapper
                        l.Add(d);
                    }

                    IFormatter formatter = new BinaryFormatter();
                    MemoryStream ms = new MemoryStream();
                    formatter.Serialize(ms, l);
                    DataFormats.Format f = DataFormats.GetFormat(myDataFormatBatch);
                    Clipboard.SetData(myDataFormatBatch, ms);
                }
            }
            finally { Application.UseWaitCursor = false; Application.DoEvents(); }
        }

        // For "resourcePaste()" see Import/Import.cs

        private void resourceDuplicate()
        {
            if (resource == null) return;
            byte[] buffer = resource.AsBytes;
            MemoryStream ms = new MemoryStream();
            ms.Write(buffer, 0, buffer.Length);

            IResourceIndexEntry rie = CurrentPackage.AddResource(
                browserWidget1.SelectedResource.ResourceType, browserWidget1.SelectedResource.ResourceGroup, browserWidget1.SelectedResource.Instance,
                ms, false);
            rie.Compressed = browserWidget1.SelectedResource.Compressed;

            IResource res = s3pi.WrapperDealer.WrapperDealer.GetResource(0, CurrentPackage, rie, true);//Don't need wrapper
            package.ReplaceResource(rie, res); // Commit new resource to package
            IsPackageDirty = true;

            browserWidget1.Add(rie);
        }

        private void resourceCompressed()
        {
            ushort target = 0xffff;
            if (CompressedCheckState() == CheckState.Checked) target = 0;
            foreach (IResourceIndexEntry rie in browserWidget1.SelectedResources)
            {
                IsPackageDirty = !isPackageDirty || rie.Compressed != target;
                rie.Compressed = target;
            }
        }

        private void resourceIsDeleted()
        {
            bool target = true;
            if (IsDeletedCheckState() == CheckState.Checked) target = false;
            foreach (IResourceIndexEntry rie in browserWidget1.SelectedResources)
            {
                IsPackageDirty = !isPackageDirty || rie.IsDeleted != target;
                rie.IsDeleted = target;
            }
        }

        private void resourceDetails()
        {
            if (browserWidget1.SelectedResource == null) return;

            ResourceDetails ir = new ResourceDetails(resourceName != null && resourceName.Length > 0, false);
            ir.ResourceType = browserWidget1.SelectedResource.ResourceType;
            ir.ResourceGroup = browserWidget1.SelectedResource.ResourceGroup;
            ir.Instance = browserWidget1.SelectedResource.Instance;
            ir.Compress = browserWidget1.SelectedResource.Compressed != 0;
            if (ir.UseName) ir.ResourceName = resourceName;
            
            DialogResult dr = ir.ShowDialog();
            if (dr != DialogResult.OK) return;

            if (ir.UseName && ir.ResourceName != null && ir.ResourceName.Length > 0)
                UpdateNameMap(ir.Instance, ir.ResourceName, true, ir.AllowRename);

            browserWidget1.SelectedResource.ResourceType = ir.ResourceType;
            browserWidget1.SelectedResource.ResourceGroup = ir.ResourceGroup;
            browserWidget1.SelectedResource.Instance = ir.Instance;
            browserWidget1.SelectedResource.Compressed = (ushort)(ir.Compress ? 0xffff : 0);
            IsPackageDirty = true;
        }

        private void resourceReplace()
        {
            if (browserWidget1.SelectedResource as AResourceIndexEntry == null) return;

            TGIN tgin = browserWidget1.SelectedResource as AResourceIndexEntry;

            List<string> ext;
            string resType = "0x" + tgin.ResType.ToString("X8");
            if (s3pi.Extensions.ExtList.Ext.ContainsKey(resType)) ext = s3pi.Extensions.ExtList.Ext[resType];
            else ext = s3pi.Extensions.ExtList.Ext["*"];

            replaceResourceDialog.Filter = ext[0] + " by type|S3_" + tgin.ResType.ToString("X8") + "*.*" +
                "|" + ext[0] + " by ext|*" + ext[ext.Count - 1] +
                "|All files|*.*";
            int i = S3PIDemoFE.Properties.Settings.Default.ResourceReplaceFilterIndex;
            replaceResourceDialog.FilterIndex = (i >= 0 && i < 3) ? S3PIDemoFE.Properties.Settings.Default.ResourceReplaceFilterIndex + 1 : 1;
            replaceResourceDialog.FileName = replaceResourceDialog.Filter.Split('|')[i * 2 + 1];
            DialogResult dr = replaceResourceDialog.ShowDialog();
            if (dr != DialogResult.OK) return;
            S3PIDemoFE.Properties.Settings.Default.ResourceReplaceFilterIndex = replaceResourceDialog.FilterIndex - 1;

            BinaryReader br;
            try
            {
                br = new BinaryReader(new FileStream(replaceResourceDialog.FileName, FileMode.Open));
            }
            catch (Exception ex)
            {
                string s = "Could not open file:\n" + replaceResourceDialog.FileName + ".  No changes made.\n";
                for (Exception inex = ex; inex != null; inex = inex.InnerException) s += "\n" + inex.Message;
                for (Exception inex = ex; inex != null; inex = inex.InnerException) s += "\n----\nStack trace:\n" + inex.StackTrace;

                CopyableMessageBox.Show(this, s,
                    myName, CopyableMessageBoxIcon.Error, new List<string>(new string[] { "OK" }), 0, 0);

                return;
            }

            resource.Stream.Position = 0;
            resource.Stream.SetLength(br.BaseStream.Length);
            resource.Stream.Write(br.ReadBytes((int)br.BaseStream.Length), 0, (int)br.BaseStream.Length);

            package.ReplaceResource(browserWidget1.SelectedResource, resource);
            resourceIsDirty = controlPanel1.CommitEnabled = false;
            IsPackageDirty = true;
        }

        // For "resourceImport()", see Import/Import.cs
        // For "resourceImportPackages()", see Import/Import.cs

        private bool UpdateNameMap(ulong instance, string resourceName, bool create, bool replace)
        {
            IResourceIndexEntry rie = CurrentPackage.Find(new string[] { "ResourceType" }, new TypedValue[] { new TypedValue(typeof(uint), (uint)0x0166038C) });
            if (rie == null && create)
            {
                rie = CurrentPackage.AddResource(0x0166038C, 0, 0, null, false);
                if (rie != null) browserWidget1.Add(rie);
            }
            if (rie == null) return false;

            try
            {
                IDictionary<ulong, string> nmap = s3pi.WrapperDealer.WrapperDealer.GetResource(0, CurrentPackage, rie, false) as IDictionary<ulong, string>;
                if (nmap == null) return false;

                if (nmap.ContainsKey(instance))
                {
                    if (replace) nmap[instance] = resourceName;
                }
                else
                    nmap.Add(instance, resourceName);
                CurrentPackage.ReplaceResource(rie, (IResource)nmap);
                IsPackageDirty = true;
            }
            catch (Exception ex)
            {
                string s = "Resource names cannot be added.  Other than that, you should be fine.  Carry on.";
                s += String.Format("\n\nError reading _KEY {0:X8}:{1:X8}:{2:X16}", rie.ResourceType, rie.ResourceGroup, rie.Instance);
                for (Exception inex = ex; inex != null; inex = inex.InnerException) s += "\n" + inex.Message;
                for (Exception inex = ex; inex != null; inex = inex.InnerException) s += "\n----\nStack trace:\n" + inex.StackTrace;
                CopyableMessageBox.Show(s, "s3pe", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private IResourceIndexEntry NewResource(uint type, uint group, ulong instance, MemoryStream ms, bool replace, bool compress)
        {
            IResourceIndexEntry rie = CurrentPackage.Find(new string[] { "ResourceType", "ResourceGroup", "Instance" },
                new TypedValue[] { new TypedValue(type.GetType(), type), new TypedValue(group.GetType(), group), new TypedValue(instance.GetType(), instance), });
            if (rie != null)
            {
                if (!replace) return null;
                CurrentPackage.DeleteResource(rie);
            }

            rie = CurrentPackage.AddResource(type, group, instance, ms, true);
            if (rie == null) return null;

            rie.Compressed = (ushort)(compress ? 0xffff : 0);

            IsPackageDirty = true;

            return rie;
        }

        private void resourceExport()
        {
            if (browserWidget1.SelectedResources.Count > 1) { exportBatch(); return; }

            if (browserWidget1.SelectedResource as AResourceIndexEntry == null) return;
            TGIN tgin = browserWidget1.SelectedResource as AResourceIndexEntry;
            tgin.ResName = resourceName;

            exportFileDialog.FileName = tgin;
            exportFileDialog.InitialDirectory = S3PIDemoFE.Properties.Settings.Default.LastExportPath;

            DialogResult dr = exportFileDialog.ShowDialog();
            if (dr != DialogResult.OK) return;
            S3PIDemoFE.Properties.Settings.Default.LastExportPath = Path.GetDirectoryName(exportFileDialog.FileName);

            Application.UseWaitCursor = true;
            Application.DoEvents();
            try { exportFile(browserWidget1.SelectedResource, exportFileDialog.FileName); }
            finally { Application.UseWaitCursor = false; Application.DoEvents(); }
        }

        private void exportBatch()
        {
            exportBatchTarget.SelectedPath = S3PIDemoFE.Properties.Settings.Default.LastExportPath;
            DialogResult dr = exportBatchTarget.ShowDialog();
            if (dr != DialogResult.OK) return;
            S3PIDemoFE.Properties.Settings.Default.LastExportPath = exportBatchTarget.SelectedPath;

            Application.UseWaitCursor = true;
            Application.DoEvents();
            bool overwriteAll = false;
            bool skipAll = false;
            try
            {
                foreach (IResourceIndexEntry rie in browserWidget1.SelectedResources)
                {
                    if (rie as AResourceIndexEntry == null) continue;
                    TGIN tgin = rie as AResourceIndexEntry;
                    tgin.ResName = browserWidget1.ResourceName(rie);
                    string file = Path.Combine(exportBatchTarget.SelectedPath, tgin);
                    if (File.Exists(file))
                    {
                        if (skipAll) continue;
                        if (!overwriteAll)
                        {
                            Application.UseWaitCursor = false;
                            int i = CopyableMessageBox.Show("Overwrite file?\n" + file, myName, CopyableMessageBoxIcon.Question,
                                new List<string>(new string[] { "&No", "N&o to all", "&Yes", "Y&es to all", "&Abandon", }), 0, 4);
                            if (i == 0) continue;
                            if (i == 1) { skipAll = true; continue; }
                            if (i == 3) overwriteAll = true;
                            if (i == 4) return;
                        }
                        Application.UseWaitCursor = true;
                    }
                    exportFile(rie, file);
                }
            }
            finally { Application.UseWaitCursor = false; Application.DoEvents(); }
        }

        private void exportFile(IResourceIndexEntry rie, string filename)
        {
            IResource res = s3pi.WrapperDealer.WrapperDealer.GetResource(0, CurrentPackage, rie, true);//Don't need wrapper
            Stream s = res.Stream;
            s.Position = 0;
            if (s.Length != rie.Memsize) CopyableMessageBox.Show(String.Format("Resource stream has {0} bytes; index entry says {1}.", s.Length, rie.Memsize));
            BinaryWriter w = new BinaryWriter(new FileStream(filename, FileMode.Create));
            w.Write((new BinaryReader(s)).ReadBytes((int)s.Length));
            w.Close();
        }

        private void resourceExportToPackage()
        {
            DialogResult dr = exportToPackageDialog.ShowDialog();
            if (dr != DialogResult.OK) return;

            if (Filename != null && Filename.Length > 0 && Path.GetFullPath(Filename).Equals(Path.GetFullPath(exportToPackageDialog.FileName)))
            {
                CopyableMessageBox.Show("Target must not be same as source.", "Export to package", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Error);
                return;
            }

            bool isNew = false;
            IPackage target;
            if (!File.Exists(exportToPackageDialog.FileName))
            {
                try
                {
                    target = Package.NewPackage(0);
                    target.SaveAs(exportToPackageDialog.FileName);
                    Package.ClosePackage(0, target);
                    isNew = true;
                }
                catch (Exception ex)
                {
                    string s = "Export cannot begin.  Could not create target package:\n" + exportToPackageDialog.FileName + "\n";
                    for (Exception inex = ex; inex != null; inex = inex.InnerException) s += "\n" + inex.Message;
                    for (Exception inex = ex; inex != null; inex = inex.InnerException) s += "\n----\nStack trace:\n" + inex.StackTrace;
                    CopyableMessageBox.Show(s, "Export to package", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Error);
                    return;
                }
            }

            bool replace = false;
            try
            {
                target = Package.OpenPackage(0, exportToPackageDialog.FileName, true);

                if (!isNew)
                {
                    int res = CopyableMessageBox.Show(
                        "Do you want to replace any duplicate resources in the target package discovered during export?",
                        "Export to package", CopyableMessageBoxIcon.Question, new List<string>(new string[] { "Re&ject", "Re&place", "&Abandon" }), 0, 2);
                    if (res == 2) return;
                    replace = res == 0;
                }

            }
            catch (Exception ex)
            {
                string s = "Export cannot begin.  Could not open target package:\n" + exportToPackageDialog.FileName + "\n";
                for (Exception inex = ex; inex != null; inex = inex.InnerException) s += "\n" + inex.Message;
                for (Exception inex = ex; inex != null; inex = inex.InnerException) s += "\n----\nStack trace:\n" + inex.StackTrace;
                CopyableMessageBox.Show(s, "Export to package", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Error);
                return;
            }

            try
            {
                Application.UseWaitCursor = true;
                lbProgress.Text = "Exporting to " + Path.GetFileNameWithoutExtension(exportToPackageDialog.FileName) + "...";
                Application.DoEvents();


                progressBar1.Value = 0;
                progressBar1.Maximum = browserWidget1.SelectedResources.Count;
                foreach (IResourceIndexEntry rie in browserWidget1.SelectedResources)
                {
                    exportResourceToPackage(target, rie, replace);
                    progressBar1.Value++;
                    if (progressBar1.Value % 100 == 0)
                        Application.DoEvents();
                }
                progressBar1.Value = 0;

                lbProgress.Text = "Saving...";
                Application.DoEvents();
                target.SavePackage();
                Package.ClosePackage(0, target);
            }
            finally { Package.ClosePackage(0, target); lbProgress.Text = ""; Application.UseWaitCursor = false; Application.DoEvents(); }
        }

        private void exportResourceToPackage(IPackage tgtpkg, IResourceIndexEntry srcrie, bool replace)
        {
            IResourceIndexEntry rie = tgtpkg.Find(new string[] { "ResourceType", "ResourceGroup", "Instance" },
                new TypedValue[] {
                            new TypedValue(srcrie.ResourceType.GetType(), srcrie.ResourceType),
                            new TypedValue(srcrie.ResourceGroup.GetType(), srcrie.ResourceGroup),
                            new TypedValue(srcrie.Instance.GetType(), srcrie.Instance),
                        });
            if (rie != null)
            {
                if (!replace) return;
                tgtpkg.DeleteResource(rie);
            }

            rie = tgtpkg.AddResource(srcrie.ResourceType, srcrie.ResourceGroup, srcrie.Instance, null, true);
            if (rie == null) return;
            rie.Compressed = srcrie.Compressed;

            IResource srcres = s3pi.WrapperDealer.WrapperDealer.GetResource(0, CurrentPackage, srcrie, true);//Don't need wrapper
            tgtpkg.ReplaceResource(rie, srcres);
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
                    case MenuBarWidget.MB.MBT_fnvHash: toolsFNV(); break;
                    case MenuBarWidget.MB.MBT_search: toolsSearch(); break;
                }
            }
            finally { this.Enabled = true; }
        }

        private void toolsFNV()
        {
            Tools.FNVHashDialog fnvForm = new S3PIDemoFE.Tools.FNVHashDialog();
            fnvForm.Show();
        }

        private void toolsSearch()
        {
            Tools.SearchForm searchForm = new S3PIDemoFE.Tools.SearchForm();
            searchForm.Width = this.Width * 4 / 5;
            searchForm.Height = this.Height * 4 / 5;
            searchForm.CurrentPackage = CurrentPackage;
            searchForm.Go += new EventHandler<S3PIDemoFE.Tools.SearchForm.GoEventArgs>(searchForm_Go);
            searchForm.Show();
        }

        void searchForm_Go(object sender, S3PIDemoFE.Tools.SearchForm.GoEventArgs e)
        {
            browserWidget1.SelectedResource = e.ResourceIndexEntry;
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
                    case MenuBarWidget.MB.MBS_externals: settingsExternalPrograms(); break;
                    case MenuBarWidget.MB.MBS_bookmarks: settingsOrganiseBookmarks(); break;
                    case MenuBarWidget.MB.MBS_saveSettings: saveSettings(); break;
                }
            }
            finally { this.Enabled = true; }
        }

        private void settingsExternalPrograms()
        {
            Settings.ExternalProgramsDialog epd = new S3PIDemoFE.Settings.ExternalProgramsDialog();
            if (S3PIDemoFE.Properties.Settings.Default.UserHelpersTxt != null && S3PIDemoFE.Properties.Settings.Default.UserHelpersTxt.Length > 0)
            {
                epd.HasUserHelpersTxt = true;
                epd.UserHelpersTxt = S3PIDemoFE.Properties.Settings.Default.UserHelpersTxt;
            }
            else
            {
                epd.HasUserHelpersTxt = false;
                epd.UserHelpersTxt = "";
            }
            if (S3PIDemoFE.Properties.Settings.Default.HexEditorCmd != null && S3PIDemoFE.Properties.Settings.Default.HexEditorCmd.Length > 0)
            {
                epd.HasUserHexEditor = true;
                epd.UserHexEditor = S3PIDemoFE.Properties.Settings.Default.HexEditorCmd;
                epd.HexEditorIgnoreTS = S3PIDemoFE.Properties.Settings.Default.HexEditorIgnoreTS;
                epd.HexEditorWantsQuotes = S3PIDemoFE.Properties.Settings.Default.HexEditorWantsQuotes;
            }
            else
            {
                epd.HasUserHexEditor = false;
                epd.UserHexEditor = "";
                epd.HexEditorIgnoreTS = false;
            }
            DialogResult dr = epd.ShowDialog();
            if (dr != DialogResult.OK) return;
            if (epd.HasUserHelpersTxt && epd.UserHelpersTxt.Length > 0 && File.Exists(epd.UserHelpersTxt))
            {
                S3PIDemoFE.Properties.Settings.Default.UserHelpersTxt = epd.UserHelpersTxt;
            }
            else
            {
                S3PIDemoFE.Properties.Settings.Default.UserHelpersTxt = null;
            }
            if (epd.HasUserHexEditor && epd.UserHexEditor.Length > 0 && File.Exists(epd.UserHexEditor))
            {
                S3PIDemoFE.Properties.Settings.Default.HexEditorCmd = epd.UserHexEditor;
                S3PIDemoFE.Properties.Settings.Default.HexEditorIgnoreTS = epd.HexEditorIgnoreTS;
                S3PIDemoFE.Properties.Settings.Default.HexEditorWantsQuotes = epd.HexEditorWantsQuotes;
            }
            else
            {
                S3PIDemoFE.Properties.Settings.Default.HexEditorCmd = null;
                S3PIDemoFE.Properties.Settings.Default.HexEditorIgnoreTS = false;
                S3PIDemoFE.Properties.Settings.Default.HexEditorWantsQuotes = false;
            }

            if (browserWidget1.SelectedResource != null)
            {
                s3pi.DemoPlugins.DemoPlugins.Config = S3PIDemoFE.Properties.Settings.Default.UserHelpersTxt;
                plug = new s3pi.DemoPlugins.DemoPlugins(browserWidget1.SelectedResource, resource);
                controlPanel1.ViewerEnabled = plug.HasViewer;
                controlPanel1.EditorEnabled = plug.HasEditor;
                controlPanel1.HexEditEnabled = S3PIDemoFE.Properties.Settings.Default.HexEditorCmd != null
                    && S3PIDemoFE.Properties.Settings.Default.HexEditorCmd.Length > 0;
            }
        }

        private void settingsOrganiseBookmarks()
        {
            Settings.OrganiseBookmarksDialog obd = new S3PIDemoFE.Settings.OrganiseBookmarksDialog();
            obd.ShowDialog();
            menuBarWidget1.UpdateBookmarks();
        }

        private void saveSettings()
        {
            OnSaveSettings(this, new EventArgs());
            S3PIDemoFE.Properties.Settings.Default.Save();
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

            Help.ShowHelp(this, "file:///" + Path.Combine(baseFolder, "Contents.htm"));
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
                "{0}\n"+
                "Front-end Distribution: {1}\n" +
                "Library Distribution: {2}"
                , copyright
                , getVersion(typeof(MainForm), "s3pe")
                , getVersion(typeof(s3pi.Interfaces.AApiVersionedFields), "s3pe")
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
                "THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW. EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR OTHER PARTIES PROVIDE THE PROGRAM â€œAS ISâ€� WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.\n" +
                "\n" +
                "\n" +
                "Limitation of Liability.\n" +
                "\n" +
                "IN NO EVENT UNLESS REQUIRED BY APPLICABLE LAW OR AGREED TO IN WRITING WILL ANY COPYRIGHT HOLDER, OR ANY OTHER PARTY WHO MODIFIES AND/OR CONVEYS THE PROGRAM AS PERMITTED ABOVE, BE LIABLE TO YOU FOR DAMAGES, INCLUDING ANY GENERAL, SPECIAL, INCIDENTAL OR CONSEQUENTIAL DAMAGES ARISING OUT OF THE USE OR INABILITY TO USE THE PROGRAM (INCLUDING BUT NOT LIMITED TO LOSS OF DATA OR DATA BEING RENDERED INACCURATE OR LOSSES SUSTAINED BY YOU OR THIRD PARTIES OR A FAILURE OF THE PROGRAM TO OPERATE WITH ANY OTHER PROGRAMS), EVEN IF SUCH HOLDER OR OTHER PARTY HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.\n" +
                "\n",
                this.Text);

        }

        private void helpLicence()
        {
            int dr = CopyableMessageBox.Show("\n" +
                "This program is distributed under the terms of the\nGNU General Public Licence version 3.\n" +
                "\n" +
                "If you wish to see the full text of the licence,\nplease visit http://www.fsf.org/licensing/licenses/gpl.html.\n" +
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

        #region Browser Widget
        private void browserWidget1_SelectedResourceChanging(object sender, BrowserWidget.ResourceChangingEventArgs e)
        {
            if (resource == null) return;

            resource.ResourceChanged -= new EventHandler(resource_ResourceChanged);
            if (resourceIsDirty)
            {
                int dr = CopyableMessageBox.Show(
                    String.Format("Commit changes to {0}?",
                        e.name.Length > 0
                        ? e.name
                        : String.Format("TGI {0:X8}-{1:X8}-{2:X16}", browserWidget1.SelectedResource.ResourceType, browserWidget1.SelectedResource.ResourceGroup, browserWidget1.SelectedResource.Instance)
                    ), this.Text, CopyableMessageBoxButtons.YesNoCancel, CopyableMessageBoxIcon.Question, 1);
                if (dr == 2)
                {
                    e.Cancel = true;
                    return;
                }
                if (dr != 1)
                    controlPanel1_CommitClick(null, null);
            }
        }

        private void browserWidget1_SelectedResourceChanged(object sender, BrowserWidget.ResourceChangedEventArgs e)
        {
            resourceName = e.name;
            resource = null;
            resException = null;
            if (browserWidget1.SelectedResource != null)
            {
                try
                {
                    resource = s3pi.WrapperDealer.WrapperDealer.GetResource(0, CurrentPackage, browserWidget1.SelectedResource, controlPanel1.HexOnly);
                }
                catch(Exception ex)
                {
                    resException = ex;
                }
            }

            if (resource != null) resource.ResourceChanged += new EventHandler(resource_ResourceChanged);

            resourceIsDirty = controlPanel1.CommitEnabled = false;

            controlPanel1.HexEnabled = (resource != null);
            controlPanel1_AutoChanged(null, null);
            if (resource != null)
            {
                if (!controlPanel1.HexOnly)
                {
                    if (resource.ContentFields.Contains("Value"))
                    {
                        Type t = AApiVersionedFields.GetContentFieldTypes(0, resource.GetType())["Value"];
                        controlPanel1.ValueEnabled = typeof(String).IsAssignableFrom(t) || typeof(Image).IsAssignableFrom(t);
                    }
                    else controlPanel1.ValueEnabled = false;

                    List<string> lf = resource.ContentFields;
                    foreach (string f in (new string[] { "Stream", "AsBytes", "Value" }))
                        if (lf.Contains(f)) lf.Remove(f);
                    controlPanel1.GridEnabled = lf.Count > 0;
                }

                s3pi.DemoPlugins.DemoPlugins.Config = S3PIDemoFE.Properties.Settings.Default.UserHelpersTxt;
                plug = new s3pi.DemoPlugins.DemoPlugins(browserWidget1.SelectedResource, resource);
                controlPanel1.ViewerEnabled = plug.HasViewer;
                controlPanel1.EditorEnabled = plug.HasEditor;

                controlPanel1.HexEditEnabled = S3PIDemoFE.Properties.Settings.Default.HexEditorCmd != null
                    && S3PIDemoFE.Properties.Settings.Default.HexEditorCmd.Length > 0;
            }
            else
            {
                plug = null;
                controlPanel1.ValueEnabled = controlPanel1.GridEnabled =
                    controlPanel1.ViewerEnabled = controlPanel1.EditorEnabled = controlPanel1.HexEditEnabled = false;
            }

            bool selectedItems = resource != null || browserWidget1.SelectedResources.Count > 0; // one or more
            menuBarWidget1.Enable(MenuBarWidget.MB.MBR_exportResources, selectedItems);
            menuBarWidget1.Enable(MenuBarWidget.CMS.MBR_exportResources, selectedItems);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBR_exportToPackage, selectedItems);
            menuBarWidget1.Enable(MenuBarWidget.CMS.MBR_exportToPackage, selectedItems);
            //menuBarWidget1.Enable(MenuBarWidget.MB.MBE_cut, resource != null);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBR_copy, selectedItems);
            menuBarWidget1.Enable(MenuBarWidget.CMS.MBR_copy, selectedItems);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBR_duplicate, resource != null);
            menuBarWidget1.Enable(MenuBarWidget.CMS.MBR_duplicate, resource != null);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBR_replace, resource != null);
            menuBarWidget1.Enable(MenuBarWidget.CMS.MBR_replace, resource != null);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBR_compressed, selectedItems);
            menuBarWidget1.Enable(MenuBarWidget.CMS.MBR_compressed, selectedItems);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBR_isdeleted, selectedItems);
            menuBarWidget1.Enable(MenuBarWidget.CMS.MBR_isdeleted, selectedItems);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBR_details, resource != null);
            menuBarWidget1.Enable(MenuBarWidget.CMS.MBR_details, resource != null);

            resourceFilterWidget1.IndexEntry = browserWidget1.SelectedResource;
        }

        private void browserWidget1_DragOver(object sender, DragEventArgs e)
        {
            if (package == null) return;
            if ((new List<string>(e.Data.GetFormats())).Contains("FileDrop"))
                e.Effect = DragDropEffects.Copy;
        }

        // For browserWidget1_DragDrop(), see Import/Import.cs

        private void browserWidget1_ItemActivate(object sender, EventArgs e)
        {
            resourceDetails();
        }
        #endregion

        #region Resource Filter Widget
        private void resourceFilterWidget1_FilterChanged(object sender, EventArgs e)
        {
            try
            {
                this.Enabled = false;
                browserWidget1.Filter = resourceFilterWidget1.FilterEnabled ? resourceFilterWidget1.Filter : null;
            }
            finally { this.Enabled = true; }
        }
        #endregion

        #region Control Panel Widget
        private void controlPanel1_SortChanged(object sender, EventArgs e)
        {
            try
            {
                this.Enabled = false;
                browserWidget1.Sortable = controlPanel1.Sort;
            }
            finally { this.Enabled = true; }
        }

        private void controlPanel1_HexOnlyChanged(object sender, EventArgs e)
        {
            Application.DoEvents();
            IResourceIndexEntry rie = browserWidget1.SelectedResource;
            if (rie != null)
            {
                browserWidget1.SelectedResource = null;
                browserWidget1.SelectedResource = rie;
            }
        }

        private void controlPanel1_AutoChanged(object sender, EventArgs e)
        {
            pnAuto.SuspendLayout();
            pnAuto.Controls.Clear();
            if (resException != null)
            {
                IResourceIndexEntry rie = browserWidget1.SelectedResource;
                string s = "";
                if (rie != null) s += String.Format("Error reading resource {0:X8}:{1:X8}:{2:X16}", rie.ResourceType, rie.ResourceGroup, rie.Instance);
                for (Exception inex = resException; inex != null; inex = inex.InnerException) s += "\n" + inex.Message;
                for (Exception inex = resException; inex != null; inex = inex.InnerException) s += "\n----\nStack trace:\n" + inex.StackTrace;
                TextBox tb = new TextBox();
                tb.Dock = DockStyle.Fill;
                tb.Multiline = true;
                tb.ReadOnly = true;
                tb.Text = s;
                pnAuto.Controls.Add(tb);
            }
            else if (resource != null)
            {
                if (controlPanel1.AutoOff) { }
                else if (controlPanel1.AutoHex)
                {
                    HexWidget hw = new HexWidget();
                    hw.Dock = DockStyle.Fill;
                    hw.Resource = resource;
                    pnAuto.Controls.Add(hw);
                }
                else if (!controlPanel1.HexOnly && controlPanel1.AutoValue)
                {
                    Control c = getValueControl();
                    if (c != null)
                    {
                        if (c.GetType().Equals(typeof(RichTextBox)))
                        {
                            c.Dock = DockStyle.Fill;
                        }
                        else if (c.GetType().Equals(typeof(PictureBox)))
                        {
                        }
                        pnAuto.Controls.Add(c);
                    }
                }
            }
            pnAuto.ResumeLayout();
        }

        private void controlPanel1_HexClick(object sender, EventArgs e)
        {
            try
            {
                this.Enabled = false;
                Application.DoEvents();

                Form f = new Form();
                HexWidget hw = new HexWidget();

                f.SuspendLayout();
                f.Controls.Add(hw);
                f.Icon = this.Icon;

                hw.Dock = DockStyle.Fill;
                hw.Resource = resource == null ? null : resource;

                f.ClientSize = new Size(this.ClientSize.Width - (this.ClientSize.Width / 5), this.ClientSize.Height - (this.ClientSize.Height / 5));
                f.Text = this.Text + ((resourceName != null && resourceName.Length > 0) ? " - " + resourceName : "");
                f.StartPosition = FormStartPosition.CenterParent;

                f.ResumeLayout();
                f.Show(this);
            }
            finally { this.Enabled = true; }
        }

        private void controlPanel1_ValueClick(object sender, EventArgs e)
        {
            try
            {
                this.Enabled = false;
                Application.DoEvents();

                Control c = getValueControl();
                if (c == null) return;

                Form f = new Form();
                f.SuspendLayout();
                f.Controls.Add(c);
                f.Icon = this.Icon;

                f.Text = this.Text + ((resourceName != null && resourceName.Length > 0) ? " - " + resourceName : "");

                if (c.GetType().Equals(typeof(RichTextBox)))
                {
                    c.Dock = DockStyle.Fill;
                    f.ClientSize = new Size(this.ClientSize.Width - (this.ClientSize.Width / 5), this.ClientSize.Height - (this.ClientSize.Height / 5));
                }
                else if (c.GetType().Equals(typeof(PictureBox)))
                {
                    f.AutoSize = true;
                    f.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                    f.SizeGripStyle = SizeGripStyle.Hide;
                }

                f.StartPosition = FormStartPosition.CenterParent;
                f.ResumeLayout();
                f.Show(this);
            }
            finally { this.Enabled = true; }
        }
        Control getValueControl()
        {
            if (!AApiVersionedFields.GetContentFields(0, resource.GetType()).Contains("Value")) return null;
            Type t = AApiVersionedFields.GetContentFieldTypes(0, resource.GetType())["Value"];
            if (typeof(String).IsAssignableFrom(t))
            {
                RichTextBox rtb = new RichTextBox();
                rtb.Text = "" + resource["Value"];
                rtb.Font = new Font("DejaVu Sans Mono", 8);
                rtb.Size = new Size(this.Width - (this.Width / 5), this.Height - (this.Height / 5));
                rtb.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                rtb.ReadOnly = true;
                return rtb;
            }
            else if (typeof(Image).IsAssignableFrom(t))
            {
                PictureBox pb = new PictureBox();
                pb.Image = (Image)resource["Value"].Value;
                pb.Size = pb.Image.Size;
                return pb;
            }
            return null;
        }

        private void controlPanel1_GridClick(object sender, EventArgs e)
        {
            try
            {
                this.Enabled = false;
                DialogResult dr = (new NewGridForm(resource as AApiVersionedFields, true)).ShowDialog();
                if (dr != DialogResult.OK)
                {
                    resourceIsDirty = false;
                    IResourceIndexEntry rie = browserWidget1.SelectedResource;
                    browserWidget1.SelectedResource = null;
                    browserWidget1.SelectedResource = rie;
                }
                else
                {
                    controlPanel1_CommitClick(null, null);
                }
            }
            finally { this.Enabled = true; }
        }

        private void controlPanel1_UseNamesChanged(object sender, EventArgs e)
        {
            try
            {
                this.Enabled = false;
                browserWidget1.DisplayResourceNames = controlPanel1.UseNames;
            }
            finally { this.Enabled = true; }
        }

        private void controlPanel1_UseTagsChanged(object sender, EventArgs e)
        {
            try
            {
                this.Enabled = false;
                browserWidget1.DisplayResourceTags = controlPanel1.UseTags;
            }
            finally { this.Enabled = true; }
        }

        private void controlPanel1_CommitClick(object sender, EventArgs e)
        {
            if (resource == null) return;
            if (package == null) return;
            package.ReplaceResource(browserWidget1.SelectedResource, resource);
            resourceIsDirty = controlPanel1.CommitEnabled = false;
            IsPackageDirty = true;
        }

        private void controlPanel1_ViewerClick(object sender, EventArgs e)
        {
            try
            {
                this.Enabled = false;
                Application.DoEvents();

                Clipboard.SetData(DataFormats.Serializable, resource.Stream);

                plug.View(resource);

                this.Activate();
                Application.DoEvents();
            }
            finally { this.Enabled = true; }
        }

        private void controlPanel1_EditorClick(object sender, EventArgs e)
        {
            try
            {
                this.Enabled = false;
                Application.DoEvents();

                Clipboard.SetData(DataFormats.Serializable, resource.Stream);

                bool res = plug.Edit(resource);

                afterEdit(res);
            }
            finally { this.Enabled = true; }
        }

        private void controlPanel1_HexEditClick(object sender, EventArgs e)
        {
            try
            {
                this.Enabled = false;
                Application.DoEvents();

                bool res = s3pi.DemoPlugins.DemoPlugins.Edit(browserWidget1.SelectedResource, resource,
                    S3PIDemoFE.Properties.Settings.Default.HexEditorCmd,
                    S3PIDemoFE.Properties.Settings.Default.HexEditorWantsQuotes,
                    S3PIDemoFE.Properties.Settings.Default.HexEditorIgnoreTS);

                afterEdit(res);
            }
            finally { this.Enabled = true; }
        }

        void afterEdit(bool res)
        {
            this.Activate();
            Application.DoEvents();

            if (res && Clipboard.ContainsData(DataFormats.Serializable))
            {
                int dr = CopyableMessageBox.Show("Resource has been updated.  Commit changes?", "Commit changes?",
                    CopyableMessageBoxButtons.YesNo, CopyableMessageBoxIcon.Question, 0);

                if (dr != 0) return;

                MemoryStream ms = Clipboard.GetData(DataFormats.Serializable) as MemoryStream;
                IResourceIndexEntry rie = NewResource(
                    browserWidget1.SelectedResource.ResourceType, browserWidget1.SelectedResource.ResourceGroup, browserWidget1.SelectedResource.Instance,
                    ms, true, browserWidget1.SelectedResource.Compressed != 0);
                if (rie != null) browserWidget1.Add(rie);
            }
        }
        #endregion
    }
}
