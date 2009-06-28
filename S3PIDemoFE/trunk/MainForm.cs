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
            lbProgress.Text = "";
            MainForm_LoadSettings();

            this.Text = myName;

            browserWidget1.Sortable = controlPanel1.Sort;
            browserWidget1.DisplayResourceNames = controlPanel1.UseNames;

            browserWidget1.Fields = new List<string>(fields.ToArray());
            List<string> filterFields = new List<string>(fields.ToArray());
            filterFields.Remove("Chunkoffset");
            filterFields.Remove("Filesize");
            filterFields.Remove("Memsize");
            resourceFilterWidget1.Fields = filterFields;

            packageInfoWidget1.Fields = packageInfoFields1.Fields;
            this.PackageFilenameChanged += new EventHandler(MainForm_PackageFilenameChanged);
            this.PackageChanged += new EventHandler(MainForm_PackageChanged);

            this.SaveSettings += new EventHandler(MainForm_SaveSettings);
            this.SaveSettings += new EventHandler(browserWidget1.BrowserWidget_SaveSettings);
            this.SaveSettings += new EventHandler(controlPanel1.ControlPanel_SaveSettings);
            this.SaveSettings += new EventHandler(hexWidget1.HexWidget_SaveSettings);
        }

        void MainForm_LoadSettings()
        {
            int h = S3PIDemoFE.Properties.Settings.Default.PersistentHeight;
            if (h == -1) h = 4 * System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height / 5;
            this.Height = h;

            int w = S3PIDemoFE.Properties.Settings.Default.PersistentWidth;
            if (w == -1) w = 4 * System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width / 5;
            this.Width = w;

            FormWindowState s =
                Enum.IsDefined(typeof(FormWindowState), S3PIDemoFE.Properties.Settings.Default.FormWindowState)
                ? (FormWindowState)S3PIDemoFE.Properties.Settings.Default.FormWindowState
                : FormWindowState.Normal;
            this.WindowState = s;
        }

        void MainForm_SaveSettings(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                S3PIDemoFE.Properties.Settings.Default.PersistentHeight = this.Height;
                S3PIDemoFE.Properties.Settings.Default.PersistentWidth = this.Width;
            }
            else
            {
                S3PIDemoFE.Properties.Settings.Default.PersistentHeight = -1;
                S3PIDemoFE.Properties.Settings.Default.PersistentWidth = -1;
            }
            S3PIDemoFE.Properties.Settings.Default.FormWindowState = (int)this.WindowState;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Enabled = false;
            Filename = "";
            if (CurrentPackage != null) { e.Cancel = true; this.Enabled = true; return; }

            OnSaveSettings(this, new EventArgs());
            S3PIDemoFE.Properties.Settings.Default.Save();
        }

        private void MainForm_PackageFilenameChanged(object sender, EventArgs e)
        {
            if (Filename.Length > 0 && File.Exists(Filename))
            {
                menuBarWidget1.AddRecentFile(Filename);
                string s = Filename;
                if (s.Length > 128)
                {
                    s = System.IO.Path.GetDirectoryName(s);
                    s = s.Substring(Math.Max(0, s.Length - 40));
                    s = "..." + System.IO.Path.Combine(s, System.IO.Path.GetFileName(Filename));
                }
                this.Text = String.Format("{0}: {1}", myName, s);
                CurrentPackage = Package.OpenPackage(0, Filename);
            }
            else
            {
                this.Text = myName;
            }
        }

        private void MainForm_PackageChanged(object sender, EventArgs e)
        {
            browserWidget1.Package = packageInfoWidget1.Package = CurrentPackage;
            hexWidget1.Resource = null;
            menuBarWidget1.Enable(MenuBarWidget.MB.MBF_saveAs, CurrentPackage != null);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBF_saveCopyAs, CurrentPackage != null);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBF_importResources, CurrentPackage != null);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBF_importPackages, CurrentPackage != null);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBF_close, CurrentPackage != null);
            //menuBarWidget1.Enable(MenuBarWidget.MD.MBE, CurrentPackage != null);
            menuBarWidget1.Enable(MenuBarWidget.MD.MBR, CurrentPackage != null);
            resourceDropDownOpening();
        }

        public event EventHandler SaveSettings;
        protected virtual void OnSaveSettings(object sender, EventArgs e) { if (SaveSettings != null) SaveSettings(sender, e); }


        #region Package Filename
        string filename;
        string Filename
        {
            get { return filename; }
            set
            {
                if (filename != "" && filename == value) return;

                CurrentPackage = null;
                if (CurrentPackage != null) return;

                filename = value;
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
            set
            {
                menuBarWidget1.Enable(MenuBarWidget.MB.MBF_save, value);
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
                    /*int res = CopyableMessageBox.Show("Current package has unsaved changes.\nSave now?",
                        myName, CopyableMessageBoxButtons.YesNoCancel, CopyableMessageBoxIcon.Warning, 2);/**///Causes error on Application.Exit();
                    DialogResult drx = MessageBox.Show("Current package has unsaved changes.\nSave now?", myName, MessageBoxButtons.YesNoCancel);
                    int res = drx == DialogResult.Yes ? 0 : drx == DialogResult.No ? 1 : 2;
                    if (res == 2) return;
                    if (res == 0) if (!fileSave()) return;
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
                    case MenuBarWidget.MB.MBF_importResources: fileImport(); break;
                    case MenuBarWidget.MB.MBF_importPackages: fileImportPackages(); break;
                    case MenuBarWidget.MB.MBF_exportResources: fileExport(); break;
                    case MenuBarWidget.MB.MBF_exportToPackage: fileExportToPackage(); break;
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
            openFileDialog1.FileName = "";
            openFileDialog1.FilterIndex = 1;
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr != DialogResult.OK) return;

            Filename = openFileDialog1.FileName;
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
                Filename = saveAsFileDialog.FileName;
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

        // For "fileImport()", see Import/Import.cs
        // For "fileImportPackages()", see Import/Import.cs

        private void UpdateNameMap(ulong instance, string resourceName, bool create, bool replace)
        {
            IResourceIndexEntry rie = CurrentPackage.Find(new string[] { "ResourceType" }, new TypedValue[] { new TypedValue(typeof(uint), (uint)0x0166038C) });
            if (rie == null && create)
            {
                rie = CurrentPackage.AddResource(0x0166038C, 0, 0, null, false);
                if (rie != null) browserWidget1.Add(rie);
            }
            if (rie == null) return;

            IDictionary<ulong, string> nmap = s3pi.WrapperDealer.WrapperDealer.GetResource(0, CurrentPackage, rie, false) as IDictionary<ulong, string>;
            if (nmap == null) return;

            if (nmap.ContainsKey(instance))
            {
                if (replace) nmap[instance] = resourceName;
            }
            else
                nmap.Add(instance, resourceName);
            CurrentPackage.ReplaceResource(rie, (IResource)nmap);
            IsPackageDirty = true;
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

            IResource res = s3pi.WrapperDealer.WrapperDealer.GetResource(0, CurrentPackage, rie, false);
            package.ReplaceResource(rie, res); // Commit new resource to package
            IsPackageDirty = true;

            return rie;
        }

        private void fileExport()
        {
            if (browserWidget1.SelectedResources.Count > 1) { exportBatch(); return; }

            if (browserWidget1.SelectedResource as AResourceIndexEntry == null) return;
            TGIN tgin = browserWidget1.SelectedResource as AResourceIndexEntry;
            tgin.ResName = resourceName;

            exportFileDialog.FileName = tgin;

            DialogResult dr = exportFileDialog.ShowDialog();
            if (dr != DialogResult.OK) return;

            Application.UseWaitCursor = true;
            Application.DoEvents();
            try { exportFile(browserWidget1.SelectedResource, exportFileDialog.FileName); }
            finally { Application.UseWaitCursor = false; Application.DoEvents(); }
        }

        private void exportBatch()
        {
            DialogResult dr = exportBatchTarget.ShowDialog();
            if (dr != DialogResult.OK) return;

            Application.UseWaitCursor = true;
            Application.DoEvents();
            try
            {
                foreach (IResourceIndexEntry rie in browserWidget1.SelectedResources)
                {
                    if (rie as AResourceIndexEntry == null) continue;
                    TGIN tgin = rie as AResourceIndexEntry;
                    tgin.ResName = browserWidget1.ResourceName(rie);
                    exportFile(rie, Path.Combine(exportBatchTarget.SelectedPath, tgin));
                }
            }
            finally { Application.UseWaitCursor = false; Application.DoEvents(); }
        }

        private void exportFile(IResourceIndexEntry rie, string filename)
        {
            IResource res = s3pi.WrapperDealer.WrapperDealer.GetResource(0, CurrentPackage, rie, true);
            Stream s = res.Stream;
            s.Position = 0;
            if (s.Length != rie.Memsize) CopyableMessageBox.Show(String.Format("Resource stream has {0} bytes; index entry says {1}.", s.Length, rie.Memsize));
            BinaryWriter w = new BinaryWriter(new FileStream(filename, FileMode.Create));
            w.Write((new BinaryReader(s)).ReadBytes((int)s.Length));
            w.Close();
        }

        private void fileExportToPackage()
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
                    CopyableMessageBox.Show("Could not create package:\n" + ex.Message, "Export to package", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Error);
                    return;
                }
            }

            bool replace = false;
            try
            {
                target = Package.OpenPackage(0, exportToPackageDialog.FileName);

                if (!isNew)
                {
                    int res = CopyableMessageBox.Show(
                        "Do you want to replace any duplicate resources in the target package discovered during export?",
                        "Export to package", CopyableMessageBoxIcon.Question, new List<string>(new string[] { "Re&place", "Re&ject", "&Abandon" }), 1, 2);
                    if (res == 2) return;
                    replace = res == 0;
                }

            }
            catch (Exception ex)
            {
                CopyableMessageBox.Show("Could not open package:\n" + ex.Message, "Export to package", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Error);
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

            IResource srcres = s3pi.WrapperDealer.WrapperDealer.GetResource(0, CurrentPackage, srcrie, true);
            tgtpkg.ReplaceResource(rie, srcres);
        }

        private void menuBarWidget1_MRUClick(object sender, MenuBarWidget.MRUClickEventArgs filename)
        {
            Filename = filename.filename;
        }

        private void fileExit()
        {
            Application.Exit();
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
                }
            }
            finally { /*this.Enabled = true;/**/ }
        }

        private void resourceDropDownOpening()
        {
            menuBarWidget1.Enable(MenuBarWidget.MB.MBR_paste, CurrentPackage != null &&
                (
                Clipboard.ContainsData(myDataFormatSingleFile)
                || Clipboard.ContainsData(myDataFormatBatch)
                || Clipboard.ContainsFileDropList()
                //|| Clipboard.ContainsText()
                )
            );

            CheckState res = CompressedCheckState();
            if (res == CheckState.Indeterminate)
                menuBarWidget1.Indeterminate(MenuBarWidget.MB.MBR_compressed);
            else
                menuBarWidget1.Checked(MenuBarWidget.MB.MBR_compressed, res == CheckState.Checked);

            res = IsDeletedCheckState();
            if (res == CheckState.Indeterminate)
                menuBarWidget1.Indeterminate(MenuBarWidget.MB.MBR_isdeleted);
            else
                menuBarWidget1.Checked(MenuBarWidget.MB.MBR_isdeleted, res == CheckState.Checked);
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
                    d.data = s3pi.WrapperDealer.WrapperDealer.GetResource(0, CurrentPackage, browserWidget1.SelectedResource, false).AsBytes;

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
                        d.data = s3pi.WrapperDealer.WrapperDealer.GetResource(0, CurrentPackage, rie, false).AsBytes;
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

            IResource res = s3pi.WrapperDealer.WrapperDealer.GetResource(0, CurrentPackage, rie, false);
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
            Help.ShowHelp(this, "file:///" + Path.Combine(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "HelpFiles"), "Contents.htm"));
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
            resource = browserWidget1.SelectedResource == null
                ? null
                : s3pi.WrapperDealer.WrapperDealer.GetResource(0, CurrentPackage, browserWidget1.SelectedResource, controlPanel1.ValueEnabled);

            if (resource != null) resource.ResourceChanged += new EventHandler(resource_ResourceChanged);

            resourceIsDirty = controlPanel1.CommitEnabled = false;

            controlPanel1.HexEnabled = (resource != null);
            if (resource != null && !controlPanel1.HexOnly)
            {
                if (resource.ContentFields.Contains("Value"))
                {
                    Type t = AApiVersionedFields.GetContentFieldTypes(0, resource.GetType())["Value"];
                    controlPanel1.ValueEnabled = typeof(string).IsAssignableFrom(t) || typeof(Image).IsAssignableFrom(t);
                }

                List<string> lf = resource.ContentFields;
                foreach (string f in (new string[] { "Stream", "AsBytes", "Value" }))
                    if (lf.Contains(f)) lf.Remove(f);
                controlPanel1.GridEnabled = lf.Count > 0;

                s3pi.DemoPlugins.DemoPlugins.Config = S3PIDemoFE.Properties.Settings.Default.DemoPluginsConfig;
                plug = new s3pi.DemoPlugins.DemoPlugins(browserWidget1.SelectedResource, resource);
                controlPanel1.ViewerEnabled = plug.HasViewer;
                controlPanel1.EditorEnabled = plug.HasEditor;
            }
            else
            {
                plug = null;
                controlPanel1.ValueEnabled = controlPanel1.GridEnabled = controlPanel1.ViewerEnabled = controlPanel1.EditorEnabled = false;
            }

            menuBarWidget1.Enable(MenuBarWidget.MB.MBF_exportResources, resource != null || browserWidget1.SelectedResources.Count > 0);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBF_exportToPackage, resource != null || browserWidget1.SelectedResources.Count > 0);
            //menuBarWidget1.Enable(MenuBarWidget.MB.MBE_cut, resource != null);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBR_copy, resource != null || browserWidget1.SelectedResources.Count > 0);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBR_duplicate, resource != null);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBR_compressed, resource != null || browserWidget1.SelectedResources.Count > 0);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBR_isdeleted, resource != null || browserWidget1.SelectedResources.Count > 0);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBR_details, resource != null);

            resourceFilterWidget1.IndexEntry = browserWidget1.SelectedResource;
            hexWidget1.Resource = (controlPanel1.HexEnabled && controlPanel1.AutoHex && resource != null) ? resource : null;
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

        private void controlPanel1_HexClick(object sender, EventArgs e)
        {
            try
            {
                this.Enabled = false;
                hexWidget1.Resource = resource == null ? null : resource;
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

        private void controlPanel1_ValueClick(object sender, EventArgs e)
        {
            try
            {
                this.Enabled = false;
                Application.DoEvents();

                string title = this.Text;
                if (resourceName != null && resourceName.Length > 0) title += " - " + resourceName;

                TypedValue v = resource["Value"];
                if (typeof(String).IsAssignableFrom(v.Type))
                {
                    Form f = new Form();
                    RichTextBox rtf = new RichTextBox();
                    rtf.Text = (string)v.Value;
                    rtf.Font = new Font("DejaVu Sans Mono", 8);
                    rtf.Size = new Size(this.Width - (this.Width / 5), this.Height - (this.Height / 5));
                    rtf.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                    rtf.ReadOnly = true;
                    f.Size = new Size(rtf.Width + 12, rtf.Height + 36);
                    f.Text = title;
                    f.StartPosition = FormStartPosition.CenterParent;
                    f.Controls.Add(rtf);
                    f.ShowDialog();
                }
                else if (typeof(Image).IsAssignableFrom(v.Type))
                {
                    Form f = new Form();
                    PictureBox pb = new PictureBox();
                    pb.Image = (Image)v.Value;
                    pb.Size = ((Image)v.Value).Size;
                    f.AutoSize = true;
                    f.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                    f.SizeGripStyle = SizeGripStyle.Hide;
                    f.Text = title;
                    f.StartPosition = FormStartPosition.CenterParent;
                    f.Controls.Add(pb);
                    f.ShowDialog();
                }

            }
            finally { this.Enabled = true; }
        }

        private void controlPanel1_GridClick(object sender, EventArgs e)
        {
            try
            {
                this.Enabled = false;
#if true//DEBUG
                (new NewGridForm(resource)).ShowDialog();
#else
                (new GridForm(resource)).ShowDialog();
#endif
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
            finally { this.Enabled = true; }
        }
        #endregion
    }
}
