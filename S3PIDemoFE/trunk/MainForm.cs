/***************************************************************************
 *  Copyright (C) 2009 by Peter L Jones                                    *
 *  peter@users.sf.net                                                     *
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

namespace S3PIDemoFE
{
    public partial class MainForm : Form
    {
        const string myName = "S3PIDemoFE";
        public MainForm()
        {
            InitializeComponent();

            this.Text = myName;

            browserWidget1.Sortable = controlPanel1.Sort;
            browserWidget1.DisplayResourceNames = controlPanel1.UseNames;
            resourceFilterWidget1.Fields = browserWidget1.Fields = resourceFields1.Fields;
            packageInfoWidget1.Fields = packageInfoFields1.Fields;
            this.PackageFilenameChanged += new EventHandler(MainForm_PackageFilenameChanged);
            this.PackageChanged += new EventHandler(MainForm_PackageChanged);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Filename = "";
            if (CurrentPackage != null) e.Cancel = true;
        }

        private void MainForm_PackageFilenameChanged(object sender, EventArgs e)
        {
            if (Filename.Length > 0 && File.Exists(Filename))
            {
                this.Text = String.Format("{0}: {1}", myName, Filename);
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
            hexWidget1.Stream = null;
            menuBarWidget1.Enable(MenuBarWidget.MB.MBF_saveAs, CurrentPackage != null);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBF_saveCopyAs, CurrentPackage != null);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBF_import, CurrentPackage != null);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBF_close, CurrentPackage != null);
            //menuBarWidget1.Enable(MenuBarWidget.MD.MBE, CurrentPackage != null);
            menuBarWidget1.Enable(MenuBarWidget.MD.MBR, CurrentPackage != null);
        }

        #region Package Filename
        string filename;
        string Filename
        {
            get { return filename; }
            set
            {
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

                if (isPackageDirty)
                {
                    DialogResult dr = MessageBox.Show("Current package has unsaved changes.\nSave now?",
                        myName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button3);
                    if (dr == DialogResult.Cancel) return;
                    if (dr == DialogResult.Yes) fileSave();
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

        IResource resource = null;
        bool resourceIsDirty = false;
        void resource_ResourceChanged(object sender, EventArgs e) { controlPanel1.CommitEnabled = resourceIsDirty = true; }
        #endregion

        #region Menu Bar
        private void menuBarWidget1_MBDropDownOpening(object sender, MenuBarWidget.MBDropDownOpeningEventArgs mn)
        {
            //this.Enabled = false;
            //Application.DoEvents();
            switch (mn.mn)
            {
                case MenuBarWidget.MD.MBF: break;
                case MenuBarWidget.MD.MBE: editDropDownOpening(); break;
                case MenuBarWidget.MD.MBR: resourceDropDownOpening(); break;
                case MenuBarWidget.MD.MBH: break;
                default: break;
            }
            //this.Enabled = true;
        }

        #region File menu
        private void menuBarWidget1_MBFile_Click(object sender, MenuBarWidget.MBClickEventArgs mn)
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
                case MenuBarWidget.MB.MBF_import: fileImport(); break;
                case MenuBarWidget.MB.MBF_export: fileExport(); break;
                case MenuBarWidget.MB.MBF_exit: fileExit(); break;
            }
            this.Enabled = true;
        }

        private void fileNew()
        {
            Filename = "";
            CurrentPackage = Package.NewPackage(0);
        }

        private void fileOpen()
        {
            openFileDialog1.FileName = "";
            openFileDialog1.FilterIndex = 1;
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr != DialogResult.OK) return;

            Filename = openFileDialog1.FileName;
        }

        private void fileSave()
        {
            if (CurrentPackage == null) return;

            if (Filename == null || Filename.Length == 0) fileSaveAs();
            else
            {
                Application.UseWaitCursor = true;
                Application.DoEvents();
                try
                {
                    CurrentPackage.SavePackage();
                    IsPackageDirty = false;
                }
                finally { Application.UseWaitCursor = false; }
            }
        }

        private void fileSaveAs()
        {
            if (CurrentPackage == null) return;

            saveAsFileDialog.FileName = "";
            DialogResult dr = saveAsFileDialog.ShowDialog();
            if (dr != DialogResult.OK) return;

            if (Filename != null && Filename.Length > 0 && Path.GetFullPath(saveAsFileDialog.FileName).Equals(Path.GetFullPath(Filename)))
            {
                fileSave();
                return;
            }

            Application.UseWaitCursor = true;
            Application.DoEvents();
            try
            {
                CurrentPackage.SaveAs(saveAsFileDialog.FileName);
                IsPackageDirty = false;
                Filename = saveAsFileDialog.FileName;
            }
            finally { Application.UseWaitCursor = false; }
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

        private void fileImport()
        {
            ResourceDetails ir = new ResourceDetails(CurrentPackage.Find(new string[] { "ResourceType" }, new TypedValue[] { new TypedValue(typeof(uint), (uint)0x0166038C) }) != null, true);
            DialogResult dr = ir.ShowDialog();
            if (dr != DialogResult.OK) return;

            TGIN tgin = new TGIN();
            tgin.ResType = ir.ResourceType;
            tgin.ResGroup = ir.ResourceGroup;
            tgin.ResInstance = ir.Instance;
            tgin.ResName = ir.ResourceName;
            importFile(ir.Filename, tgin, ir.UseName, ir.AllowRename);
        }

        void importFile(string filename, TGIN tgin, bool useName, bool rename)
        {
            MemoryStream ms = new MemoryStream();
            BinaryReader r = new BinaryReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            BinaryWriter w = new BinaryWriter(ms);
            w.Write(r.ReadBytes((int)r.BaseStream.Length));
            w.Flush();

            IResourceIndexEntry rie = CurrentPackage.AddResource(tgin.ResType, tgin.ResGroup, tgin.ResInstance, ms, false);

            browserWidget1.Add(rie);
            IsPackageDirty = true;

            if (useName && tgin.ResName != null && tgin.ResName.Length > 0)
                UpdateNameMap(tgin.ResInstance, tgin.ResName, true, rename);
        }

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
        }

        private void fileExport()
        {
            if (browserWidget1.SelectedResources.Count > 1) { exportBatch(); return; }

            TGIN tgin = browserWidget1.SelectedResource as AResourceIndexEntry;
            if (tgin == null) return;
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
                    TGIN tgin = rie as AResourceIndexEntry;
                    if (tgin == null) continue;
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
            if (s.Length != rie.Memsize) MessageBox.Show(String.Format("Resource stream has {0} bytes; index entry says {1}.", s.Length, rie.Memsize));
            BinaryWriter w = new BinaryWriter(new FileStream(filename, FileMode.Create));
            w.Write((new BinaryReader(s)).ReadBytes((int)s.Length));
            w.Close();
        }

        private void fileExit()
        {
            Application.Exit();
        }
        #endregion

        #region Edit menu
        private void editDropDownOpening()
        {
            menuBarWidget1.Enable(MenuBarWidget.MB.MBE_paste, CurrentPackage != null &&
                (
                Clipboard.ContainsData(DataFormats.Serializable)
                //|| Clipboard.ContainsFileDropList()
                //|| Clipboard.ContainsText()
                )
            );
        }

        private void menuBarWidget1_MBEdit_Click(object sender, MenuBarWidget.MBClickEventArgs mn)
        {
            this.Enabled = false;
            Application.DoEvents();
            switch (mn.mn)
            {
                case MenuBarWidget.MB.MBE_cut: editCut(); break;
                case MenuBarWidget.MB.MBE_copy: editCopy(); break;
                case MenuBarWidget.MB.MBE_paste: editPaste(); break;
            }
            this.Enabled = true;
        }

        private void editCut()
        {
            editCopy();
            if (browserWidget1.SelectedResource != null) package.DeleteResource(browserWidget1.SelectedResource);
        }

        private void editCopy()
        {
            if (resource == null) return;

            Application.UseWaitCursor = true;
            Application.DoEvents();
            try { Clipboard.SetData(DataFormats.Serializable, resource.Stream); }
            finally { Application.UseWaitCursor = false; Application.DoEvents(); }
        }

        private void editPaste()
        {
            uint type, group;
            ulong instance;
            bool compress;

            if (browserWidget1.SelectedResource == null)
            {
                ResourceDetails ir = new ResourceDetails(CurrentPackage.Find(new string[] { "ResourceType" }, new TypedValue[] { new TypedValue(typeof(uint), (uint)0x0166038C) }) != null);
                DialogResult dr = ir.ShowDialog();
                if (dr != DialogResult.OK) return;
                type = ir.ResourceType;
                group = ir.ResourceGroup;
                instance = ir.Instance;
                compress = ir.Compress;
            }
            else
            {
                type = browserWidget1.SelectedResource.ResourceType;
                group = browserWidget1.SelectedResource.ResourceGroup;
                instance = browserWidget1.SelectedResource.Instance;
                compress = browserWidget1.SelectedResource.Compressed != 0;
                package.DeleteResource(browserWidget1.SelectedResource);
            }

            MemoryStream ms = null;
            if (Clipboard.ContainsData(DataFormats.Serializable)) ms = Clipboard.GetData(DataFormats.Serializable) as MemoryStream;

            IResourceIndexEntry rie = CurrentPackage.AddResource(type, group, instance, ms, false);
            rie.Compressed = (ushort)(compress ? 0xffff : 0);

            browserWidget1.Add(rie);
            IsPackageDirty = true;
        }
        #endregion

        #region Resource menu
        private void menuBarWidget1_MBResource_Click(object sender, MenuBarWidget.MBClickEventArgs mn)
        {
            this.Enabled = false;
            Application.DoEvents();
            switch (mn.mn)
            {
                case MenuBarWidget.MB.MBR_add: resourceAdd(); break;
                case MenuBarWidget.MB.MBR_details: resourceDetails(); break;
                case MenuBarWidget.MB.MBR_compressed: resourceCompressed(); break;
            }
            this.Enabled = true;
        }

        private void resourceDropDownOpening()
        {
            if (resource != null)
                menuBarWidget1.Checked(MenuBarWidget.MB.MBR_compressed, browserWidget1.SelectedResource.Compressed != 0);
        }

        private void resourceAdd()
        {
            ResourceDetails ir = new ResourceDetails(CurrentPackage.Find(new string[] { "ResourceType" }, new TypedValue[] { new TypedValue(typeof(uint), (uint)0x0166038C) }) != null);
            DialogResult dr = ir.ShowDialog();
            if (dr != DialogResult.OK) return;

            IResourceIndexEntry rie = CurrentPackage.AddResource(ir.ResourceType, ir.ResourceGroup, ir.Instance, null, false);
            rie.Compressed = (ushort)(ir.Compress ? 0xffff : 0);
            IsPackageDirty = true;

            browserWidget1.Add(rie);
            package.ReplaceResource(browserWidget1.SelectedResource, resource); // Commit new resource to package

            if (ir.UseName && ir.ResourceName != null && ir.ResourceName.Length > 0)
                UpdateNameMap(ir.Instance, ir.ResourceName, true, ir.AllowRename);
        }

        private void resourceDetails()
        {
            if (browserWidget1.SelectedResource == null) return;

            ResourceDetails ir = new ResourceDetails(resourceName != null && resourceName.Length > 0);
            ir.ResourceType = browserWidget1.SelectedResource.ResourceType;
            ir.ResourceGroup = browserWidget1.SelectedResource.ResourceGroup;
            ir.Instance = browserWidget1.SelectedResource.Instance;
            ir.Compress = browserWidget1.SelectedResource.Compressed != 0;
            if (ir.UseName) ir.ResourceName = resourceName;
            
            DialogResult dr = ir.ShowDialog();
            if (dr != DialogResult.OK) return;

            browserWidget1.SelectedResource.ResourceType = ir.ResourceType;
            browserWidget1.SelectedResource.ResourceGroup = ir.ResourceGroup;
            browserWidget1.SelectedResource.Instance = ir.Instance;
            browserWidget1.SelectedResource.Compressed = (ushort)(ir.Compress ? 0xffff : 0);
            IsPackageDirty = true;

            if (ir.UseName && ir.ResourceName != null && ir.ResourceName.Length > 0)
                UpdateNameMap(ir.Instance, ir.ResourceName, true, ir.AllowRename);
        }

        private void resourceCompressed()
        {
            browserWidget1.SelectedResource.Compressed = (ushort)(browserWidget1.SelectedResource.Compressed == 0 ? 0xffff : 0);
            IsPackageDirty = true;
        }
        #endregion

        #region Help menu
        private void menuBarWidget1_MBHelp_Click(object sender, MenuBarWidget.MBClickEventArgs mn)
        {
            this.Enabled = false;
            Application.DoEvents();
            switch (mn.mn)
            {
                case MenuBarWidget.MB.MBH_about: helpAbout(); break;
                case MenuBarWidget.MB.MBH_warranty: helpWarranty(); break;
                case MenuBarWidget.MB.MBH_licence: helpLicence(); break;
            }
            this.Enabled = true;
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
            MessageBox.Show(String.Format(
                "{0}\n"+
                "Front-end Distribution: {1}\n" +
                "Library Distribution: {2}"
                , copyright
                , getVersion(typeof(MainForm), "S3PIDemoFE")
                , getVersion(typeof(s3pi.Interfaces.AApiVersionedFields), "s3pi")
                ), this.Text, MessageBoxButtons.OK);
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
            MessageBox.Show("\n" +
                "Disclaimer of Warranty.\n" +
                "\n" +
                "THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW. EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR OTHER PARTIES PROVIDE THE PROGRAM â€œAS ISâ€� WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.\n" +
                "\n" +
                "\n" +
                "Limitation of Liability.\n" +
                "\n" +
                "IN NO EVENT UNLESS REQUIRED BY APPLICABLE LAW OR AGREED TO IN WRITING WILL ANY COPYRIGHT HOLDER, OR ANY OTHER PARTY WHO MODIFIES AND/OR CONVEYS THE PROGRAM AS PERMITTED ABOVE, BE LIABLE TO YOU FOR DAMAGES, INCLUDING ANY GENERAL, SPECIAL, INCIDENTAL OR CONSEQUENTIAL DAMAGES ARISING OUT OF THE USE OR INABILITY TO USE THE PROGRAM (INCLUDING BUT NOT LIMITED TO LOSS OF DATA OR DATA BEING RENDERED INACCURATE OR LOSSES SUSTAINED BY YOU OR THIRD PARTIES OR A FAILURE OF THE PROGRAM TO OPERATE WITH ANY OTHER PROGRAMS), EVEN IF SUCH HOLDER OR OTHER PARTY HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.\n" +
                "\n",
                this.Text,
                MessageBoxButtons.OK);

        }

        private void helpLicence()
        {
            DialogResult dr = MessageBox.Show("\n" +
                "This program is distributed under the terms of the\nGNU General Public Licence version 3.\n" +
                "\n" +
                "If you wish to see the full text of the licence,\nplease visit http://www.fsf.org/licensing/licenses/gpl.html.\n" +
                "\n" +
                "Do you wish to visit this site now?" +
                "\n",
                this.Text,
                MessageBoxButtons.YesNo);
            if (dr != DialogResult.Yes) return;
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
                DialogResult dr = MessageBox.Show(
                    String.Format("Save changes to {0}?",
                        e.name.Length > 0
                        ? e.name
                        : String.Format("TGI {0:X8}-{1:X8}-{2:X16}", browserWidget1.SelectedResource.ResourceType, browserWidget1.SelectedResource.ResourceGroup, browserWidget1.SelectedResource.Instance)
                    ), this.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
                if (dr == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                if (dr != DialogResult.No)
                    controlPanel1_CommitClick(null, null);
            }
        }

        private void browserWidget1_SelectedResourceChanged(object sender, BrowserWidget.ResourceChangedEventArgs e)
        {
            resourceName = e.name;
            resource = browserWidget1.SelectedResource == null
                ? null
                : s3pi.WrapperDealer.WrapperDealer.GetResource(0, CurrentPackage, browserWidget1.SelectedResource, controlPanel1.UnwrappedEnabled);

            if (resource != null) resource.ResourceChanged += new EventHandler(resource_ResourceChanged);

            resourceIsDirty = controlPanel1.CommitEnabled = false;

            controlPanel1.HexEnabled = (resource != null);
            if (resource != null)
            {
                List<string> lf = resource.ContentFields;
                foreach (string f in (new string[] { "Stream", "AsBytes", "Value" }))
                    if (lf.Contains(f)) lf.Remove(f);
                controlPanel1.DataGridEnabled = lf.Count > 0;
            }
            else controlPanel1.DataGridEnabled = false;

            menuBarWidget1.Enable(MenuBarWidget.MB.MBF_export, resource != null || browserWidget1.SelectedResources.Count > 0);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBE_cut, resource != null);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBE_copy, resource != null);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBR_compressed, resource != null);
            resourceFilterWidget1.IndexEntry = browserWidget1.SelectedResource;
            controlPanel1.UnwrappedEnabled = !controlPanel1.HexOnly && resource != null && resource.ContentFields.Contains("Value");
            hexWidget1.Stream = (controlPanel1.HexEnabled && controlPanel1.AutoHex && resource != null) ? resource.Stream : null;
        }

        private void browserWidget1_SelectedResourceDeleted(object sender, EventArgs e)
        {
            if (browserWidget1.SelectedResource != null)
            {
                package.DeleteResource(browserWidget1.SelectedResource);
                IsPackageDirty = true;
            }
        }

        private void browserWidget1_DragOver(object sender, DragEventArgs e)
        {
            if (package == null) return;
            if ((new List<string>(e.Data.GetFormats())).Contains("FileDrop"))
                e.Effect = DragDropEffects.Copy;
        }

        private void browserWidget1_DragDrop(object sender, DragEventArgs e)
        {
            string[] fileDrop = e.Data.GetData("FileDrop") as String[];
            if (fileDrop == null || fileDrop.Length == 0) return;

            ImportBatch ib = new ImportBatch(fileDrop);
            DialogResult dr = ib.ShowDialog();
            if (dr != DialogResult.OK) return;

            foreach (string filename in ib.Batch)
                importFile(filename, filename, ib.UseNames, ib.Rename);
        }

        private void browserWidget1_ItemActivate(object sender, EventArgs e)
        {
            resourceDetails();
        }
        #endregion

        #region Resource Filter Widget
        private void resourceFilterWidget1_FilterChanged(object sender, EventArgs e)
        {
            this.Enabled = false;
            browserWidget1.Filter = resourceFilterWidget1.FilterEnabled ? resourceFilterWidget1.Filter : null;
            this.Enabled = true;
        }
        #endregion

        #region Control Panel Widget
        private void controlPanel1_SortChanged(object sender, EventArgs e)
        {
            this.Enabled = false;
            browserWidget1.Sortable = controlPanel1.Sort;
            this.Enabled = true;
        }

        private void controlPanel1_HexClick(object sender, EventArgs e)
        {
            this.Enabled = false;
            hexWidget1.Stream = resource == null ? null : resource.Stream;
            this.Enabled = true;
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

        private void controlPanel1_UnwrappedClick(object sender, EventArgs e)
        {
            this.Enabled = false;

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

            this.Enabled = true;
        }

        private void controlPanel1_DataGridClick(object sender, EventArgs e)
        {
            (new DataSheet(resource)).ShowDialog();
        }

        private void controlPanel1_UseNamesChanged(object sender, EventArgs e)
        {
            this.Enabled = false;
            browserWidget1.DisplayResourceNames = controlPanel1.UseNames;
            this.Enabled = true;
        }

        private void controlPanel1_CommitClick(object sender, EventArgs e)
        {
            if (resource == null) return;
            if (package == null) return;
            package.ReplaceResource(browserWidget1.SelectedResource, resource);
            resourceIsDirty = controlPanel1.CommitEnabled = false;
            IsPackageDirty = true;
        }
        #endregion
    }
}
