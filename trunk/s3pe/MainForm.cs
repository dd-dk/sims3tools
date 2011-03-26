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
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using s3pi.Interfaces;
using s3pi.Package;
using s3pi.Extensions;
using System.Text;

namespace S3PIDemoFE
{
    public partial class MainForm : Form
    {
        static List<string> fields = AApiVersionedFields.GetContentFields(0, typeof(AResourceIndexEntry));
        static List<string> unwantedFields = new List<string>(new string[] {
            "Stream",
        });
        static List<string> unwantedFilterFields = new List<string>(new string[] {
            "Chunkoffset", "Filesize", "Memsize", "Unknown2",
        });
        static List<string> ddsResources = new List<string>(new string[] {
            "0x00B2D882", "0x8FFB80F6",
        });
        static string myName;
        static string tempName;
        static MainForm()
        {
            myName = Path.GetFileNameWithoutExtension(Application.ExecutablePath);
            tempName = "s3pe-" + System.Security.Cryptography.FNV64.GetHash(DateTime.UtcNow.ToString("O")).ToString("X16") + "-";
            foreach (string s in unwantedFields) fields.Remove(s);
            //fields.Sort(byElementPriority);

            List<KeyValuePair<string, Type>> typeMap = new List<KeyValuePair<string, Type>>(s3pi.WrapperDealer.WrapperDealer.TypeMap);
            s3pi.WrapperDealer.WrapperDealer.Disabled.Clear();
            if (S3PIDemoFE.Properties.Settings.Default.DisabledWrappers != null)
                foreach (var v in S3PIDemoFE.Properties.Settings.Default.DisabledWrappers)
                {
                    string[] kv = v.Trim().Split(new char[] { ':', },2);
                    KeyValuePair<string, Type> kvp = typeMap.Find(x => x.Key == kv[0] && x.Value.FullName == kv[1]);
                    if (!kvp.Equals(default(KeyValuePair<string, Type>)))
                        s3pi.WrapperDealer.WrapperDealer.Disabled.Add(kvp);
                }
        }
        static int byElementPriority(string x, string y)
        {
            int xPrio = int.MaxValue;
            int yPrio = int.MaxValue;
            object[] xCA = typeof(AResourceIndexEntry).GetProperty(x).GetCustomAttributes(typeof(ElementPriorityAttribute), true);
            object[] yCA = typeof(AResourceIndexEntry).GetProperty(y).GetCustomAttributes(typeof(ElementPriorityAttribute), true);
            foreach (ElementPriorityAttribute o in xCA) { xPrio = o.Priority; break; }
            foreach (ElementPriorityAttribute o in yCA) { yPrio = o.Priority; break; }
            return xPrio.CompareTo(yPrio);
        }

        public MainForm()
        {
            InitializeComponent();

            this.Text = myName;
            
            this.lbProgress.Text = "";

            browserWidget1.Fields = new List<string>(fields.ToArray());
            browserWidget1.ContextMenuStrip = menuBarWidget1.browserWidgetContextMenuStrip;

            List<string> filterFields = new List<string>(fields);
            foreach (string f in unwantedFilterFields)
                filterFields.Remove(f);
            filterFields.Insert(0, "Tag");
            filterFields.Insert(0, "Name");
            resourceFilterWidget1.BrowserWidget = browserWidget1;
            resourceFilterWidget1.Fields = filterFields;

            packageInfoWidget1.Fields = packageInfoFields1.Fields;
            this.PackageFilenameChanged += new EventHandler(MainForm_PackageFilenameChanged);
            this.PackageChanged += new EventHandler(MainForm_PackageChanged);

            this.SaveSettings += new EventHandler(MainForm_SaveSettings);
            this.SaveSettings += new EventHandler(browserWidget1.BrowserWidget_SaveSettings);
            this.SaveSettings += new EventHandler(controlPanel1.ControlPanel_SaveSettings);
            //this.SaveSettings += new EventHandler(hexWidget1.HexWidget_SaveSettings);

            MainForm_LoadFormSettings();
        }

        public MainForm(params string[] args)
            :this()
        {
            CmdLine(args);

            // Settings for test mode
            if (cmdlineTest)
            {
            }
        }

        void MainForm_LoadFormSettings()
        {
            FormWindowState s =
                Enum.IsDefined(typeof(FormWindowState), S3PIDemoFE.Properties.Settings.Default.FormWindowState)
                ? (FormWindowState)S3PIDemoFE.Properties.Settings.Default.FormWindowState
                : FormWindowState.Minimized;

            int defaultWidth = 4 * System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width / 5;
            int defaultHeight = 4 * System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height / 5;
            this.ClientSize = new Size(defaultWidth, defaultHeight);//needed to correctly work out the following
            int defaultSplitterDistance1 = splitContainer1.ClientSize.Height - (splitContainer1.Panel2MinSize + splitContainer1.SplitterWidth + 4);
            int defaultSplitterDistance2 = (int)(splitContainer2.ClientSize.Width / 2);

            if (s == FormWindowState.Minimized)
            {
                this.ClientSize = new Size(defaultWidth, defaultHeight);
                splitContainer1.SplitterDistance = defaultSplitterDistance1;
                splitContainer2.SplitterDistance = defaultSplitterDistance2;
                this.StartPosition = FormStartPosition.CenterScreen;
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                // these mustn't be negative

                int w = S3PIDemoFE.Properties.Settings.Default.PersistentWidth;
                int h = S3PIDemoFE.Properties.Settings.Default.PersistentHeight;
                this.ClientSize = new Size(w < 0 ? defaultWidth : w, h < 0 ? defaultHeight : h);

                int s1 = S3PIDemoFE.Properties.Settings.Default.Splitter1Position;
                splitContainer1.SplitterDistance = s1 < 0 ? defaultSplitterDistance1 : s1;

                int s2 = S3PIDemoFE.Properties.Settings.Default.Splitter2Position;
                splitContainer2.SplitterDistance = s2 < 0 ? defaultSplitterDistance2 : s2;

                // everything else assumed valid -- any problems, use the iconise/exit/run trick to fix
                
                this.StartPosition = FormStartPosition.Manual;
                this.Location = S3PIDemoFE.Properties.Settings.Default.PersistentLocation;
                this.WindowState = s;
            }
        }

        void MainForm_SaveSettings(object sender, EventArgs e)
        {
            S3PIDemoFE.Properties.Settings.Default.FormWindowState = (int)this.WindowState;
            S3PIDemoFE.Properties.Settings.Default.PersistentHeight = this.ClientSize.Height;
            S3PIDemoFE.Properties.Settings.Default.PersistentWidth = this.ClientSize.Width;
            S3PIDemoFE.Properties.Settings.Default.PersistentLocation = this.Location;
            S3PIDemoFE.Properties.Settings.Default.Splitter1Position = splitContainer1.SplitterDistance;
            S3PIDemoFE.Properties.Settings.Default.Splitter2Position = splitContainer2.SplitterDistance;

            S3PIDemoFE.Properties.Settings.Default.DisabledWrappers = new System.Collections.Specialized.StringCollection();
            foreach (var kvp in s3pi.WrapperDealer.WrapperDealer.Disabled)
                S3PIDemoFE.Properties.Settings.Default.DisabledWrappers.Add(kvp.Key + ":" + kvp.Value.FullName + "\n");
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Enabled = false;
            Filename = "";
            if (CurrentPackage != null) { e.Cancel = true; this.Enabled = true; return; }

            saveSettings();

            cleanUpTemp();
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
                    IssueException(ex, "Could not open package:\n" + Filename);
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
            bool enable = CurrentPackage != null;
            menuBarWidget1.Enable(MenuBarWidget.MB.MBF_saveAs, enable);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBF_saveCopyAs, enable);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBF_close, enable);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBF_bookmarkCurrent, enable);
            menuBarWidget1.Enable(MenuBarWidget.MD.MBE, enable);
            menuBarWidget1.Enable(MenuBarWidget.MD.MBR, enable);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBR_add, enable);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBT_search, enable);
            editDropDownOpening();
            resourceDropDownOpening();
        }

        public event EventHandler SaveSettings;
        protected virtual void OnSaveSettings(object sender, EventArgs e) { if (SaveSettings != null) SaveSettings(sender, e); }


        internal static void IssueException(Exception ex, string prefix)
        {
            CopyableMessageBox.IssueException(ex,
                String.Format("{0}\nFront-end Distribution: {1}\nLibrary Distribution: {2}",
                prefix, getVersion(typeof(MainForm), "s3pe"), getVersion(typeof(s3pi.Interfaces.AApiVersionedFields), "s3pe")),
                myName);
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

            this.Show();
            importBatch(batch.ToArray(), "-import");
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
                //http://dino.drealm.info/develforums/s3pi/index.php?topic=781.0
                //if (filename != "" && filename == value) return;

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
        s3pi.Helpers.HelperManager helpers = null;

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
            openFileDialog1.FileName = "";
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
            saveAsFileDialog.FilterIndex = 1;
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
            saveAsFileDialog.FilterIndex = 1;
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
        private void editDropDownOpening()
        {
            Application.DoEvents();
            bool enable = resException == null && resource != null;
            menuBarWidget1.Enable(MenuBarWidget.MB.MBE_copy, enable
                && ((pnAuto.Controls[0] is RichTextBox && (pnAuto.Controls[0] as RichTextBox).SelectedText.Length > 0)
                || (pnAuto.Controls[0] is HexWidget && (pnAuto.Controls[0] as HexWidget).SelectedText.Length > 0))
                );
            menuBarWidget1.Enable(MenuBarWidget.MB.MBE_float, enable);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBE_ote, enable);
        }

        private void menuBarWidget1_MBEdit_Click(object sender, MenuBarWidget.MBClickEventArgs mn)
        {
            try
            {
                this.Enabled = false;
                Application.DoEvents();
                switch (mn.mn)
                {
                    case MenuBarWidget.MB.MBE_copy: editCopy(); break;
                    case MenuBarWidget.MB.MBE_float: editFloat(); break;
                    case MenuBarWidget.MB.MBE_ote: editOTE(); break;
                }
            }
            finally { this.Enabled = true; }
        }

        private void editCopy()
        {
            if (pnAuto.Controls.Count != 1) return;
            string selectedText = "";
            if (pnAuto.Controls[0] is RichTextBox) selectedText = (pnAuto.Controls[0] as RichTextBox).SelectedText;
            else if (pnAuto.Controls[0] is HexWidget) selectedText = (pnAuto.Controls[0] as HexWidget).SelectedText;
            else return;
            if (selectedText.Length == 0) return;

            System.Text.StringBuilder s = new StringBuilder();
            TextReader t = new StringReader(selectedText);
            for (var line = t.ReadLine(); line != null; line = t.ReadLine()) s.AppendLine(line);
            Clipboard.SetText(s.ToString(), TextDataFormat.UnicodeText);
        }

        private void editFloat()
        {
            if (!controlPanel1.HexOnly && controlPanel1.AutoValue) controlPanel1_ValueClick(null, EventArgs.Empty);
            else controlPanel1_HexClick(null, EventArgs.Empty);
        }

        private void editOTE()
        {
            if (pnAuto.Controls.Count != 1) return;

            string text = "";
            if (pnAuto.Controls[0] is RichTextBox) { text = (pnAuto.Controls[0] as RichTextBox).Text; }
            else if (pnAuto.Controls[0] is HexWidget) { text = (pnAuto.Controls[0] as HexWidget).Text; }
            else return;

            System.Text.StringBuilder s = new StringBuilder();
            TextReader t = new StringReader(text);
            for (var line = t.ReadLine(); line != null; line = t.ReadLine()) s.AppendLine(line);
            text = s.ToString();

            UTF8Encoding utf8 = new UTF8Encoding();
            UnicodeEncoding unicode = new UnicodeEncoding();
            byte[] utf8Bytes = Encoding.Convert(unicode, utf8, unicode.GetBytes(text));

            string command = S3PIDemoFE.Properties.Settings.Default.TextEditorCmd;
            string filename = String.Format("{0}{1}{2}.txt", Path.GetTempPath(), tempName, System.Security.Cryptography.FNV64.GetHash(DateTime.UtcNow.ToString("O")).ToString("X16"));
            using (BinaryWriter w = new BinaryWriter(new FileStream(filename, FileMode.Create), Encoding.UTF8))
            {
                w.Write(utf8Bytes);
                w.Close();
            }
            File.SetAttributes(filename, FileAttributes.ReadOnly | FileAttributes.Temporary);

            System.Diagnostics.Process p = new System.Diagnostics.Process();

            p.StartInfo.FileName = command;
            p.StartInfo.Arguments = filename;
            p.StartInfo.UseShellExecute = false;
            p.Exited += new EventHandler(p_Exited);
            p.EnableRaisingEvents = true;

            try { p.Start(); }
            catch (Exception ex)
            {
                CopyableMessageBox.IssueException(ex, String.Format("Application failed to start:\n{0}\n{1}", command, filename), "Launch failed");
                File.SetAttributes(filename, FileAttributes.Normal);
                File.Delete(filename);
                return;
            }
        }

        void p_Exited(object sender, EventArgs e)
        {
            System.Diagnostics.Process p = sender as System.Diagnostics.Process;
            File.SetAttributes(p.StartInfo.Arguments, FileAttributes.Normal);
            File.Delete(p.StartInfo.Arguments);

            MakeFormVisible();
        }

        void cleanUpTemp()
        {
            foreach (var file in Directory.GetFiles(Path.GetTempPath(), String.Format("{0}*.txt", tempName)))
                try {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }
                catch { }
        }
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
                    case MenuBarWidget.MB.MBR_importAsDBC: resourceImportAsDBC(); break;
                    case MenuBarWidget.MB.MBR_exportResources: resourceExport(); break;
                    case MenuBarWidget.MB.MBR_exportToPackage: resourceExportToPackage(); break;
                    case MenuBarWidget.MB.MBR_hexEditor: resourceHexEdit(); break;
                    case MenuBarWidget.MB.MBR_textEditor: resourceTextEdit(); break;
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

            CheckState res = CompressedCheckState();
            if (res == CheckState.Indeterminate)
            {
                menuBarWidget1.Indeterminate(MenuBarWidget.MB.MBR_compressed);
            }
            else
            {
                menuBarWidget1.Checked(MenuBarWidget.MB.MBR_compressed, res == CheckState.Checked);
            }

            res = IsDeletedCheckState();
            if (res == CheckState.Indeterminate)
            {
                menuBarWidget1.Indeterminate(MenuBarWidget.MB.MBR_isdeleted);
            }
            else
            {
                menuBarWidget1.Checked(MenuBarWidget.MB.MBR_isdeleted, res == CheckState.Checked);
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
            ResourceDetails ir = new ResourceDetails(CurrentPackage.Find(x => x.ResourceType == 0x0166038C) != null, false);
            DialogResult dr = ir.ShowDialog();
            if (dr != DialogResult.OK) return;

            IResourceIndexEntry rie = NewResource(ir, null, ir.Replace ? DuplicateHandling.replace : DuplicateHandling.reject, ir.Compress);
            if (rie == null) return;

            browserWidget1.Add(rie);
            package.ReplaceResource(rie, resource);//Ensure there's an actual resource in the package

            if (ir.UseName && ir.ResourceName != null && ir.ResourceName.Length > 0)
                browserWidget1.ResourceName(ir.Instance, ir.ResourceName, true, ir.AllowRename);
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

            IResourceIndexEntry rie = CurrentPackage.AddResource(browserWidget1.SelectedResource, ms, false);
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

            ResourceDetails ir = new ResourceDetails(resourceName != null && resourceName.Length > 0, false, browserWidget1.SelectedResource);
            ir.Compress = browserWidget1.SelectedResource.Compressed != 0;
            if (ir.UseName) ir.ResourceName = resourceName;
            
            DialogResult dr = ir.ShowDialog();
            if (dr != DialogResult.OK) return;

            browserWidget1.ResourceKey = ir;
            browserWidget1.SelectedResource.Compressed = (ushort)(ir.Compress ? 0xffff : 0);

            if (ir.UseName && ir.ResourceName != null && ir.ResourceName.Length > 0)
                browserWidget1.ResourceName(ir.Instance, ir.ResourceName, true, ir.AllowRename);

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
                IssueException(ex, "Could not open file:\n" + replaceResourceDialog.FileName + ".\nNo changes made.");
                return;
            }

            // Reload the resource we just replaced as there's no way to get a changed trigger from it
            SuspendLayout();
            IResourceIndexEntry rie = browserWidget1.SelectedResource;
            IResource res = s3pi.WrapperDealer.WrapperDealer.GetResource(0, CurrentPackage, rie, true);
            browserWidget1.SelectedResource = null;

            res.Stream.Position = 0;
            res.Stream.SetLength(br.BaseStream.Length);
            res.Stream.Write(br.ReadBytes((int)br.BaseStream.Length), 0, (int)br.BaseStream.Length);
            br.Close();

            package.ReplaceResource(rie, res);
            resourceIsDirty = controlPanel1.CommitEnabled = false;
            IsPackageDirty = true;

            browserWidget1.SelectedResource = rie;
            ResumeLayout();
        }

        // For "resourceImport()", see Import/Import.cs
        // For "resourceImportPackages()", see Import/Import.cs

        /// <summary>
        /// How to handle duplicate resources when adding to a package
        /// </summary>
        enum DuplicateHandling
        {
            /// <summary>
            /// Refuse to create the request resource
            /// </summary>
            reject,
            /// <summary>
            /// Delete any conflicting resource
            /// </summary>
            replace,
            /// <summary>
            /// Ignore any conflicting resource
            /// </summary>
            allow,
        }
        private IResourceIndexEntry NewResource(IResourceKey rk, MemoryStream ms, DuplicateHandling dups, bool compress)
        {
            IResourceIndexEntry rie = CurrentPackage.Find(x => rk.Equals(x));
            if (rie != null)
            {
                if (dups == DuplicateHandling.reject) return null;
                if (dups == DuplicateHandling.replace) CurrentPackage.DeleteResource(rie);
            }

            rie = CurrentPackage.AddResource(rk, ms, dups != DuplicateHandling.allow);
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
                    IssueException(ex, "Export cannot begin.  Could not create target package:\n" + exportToPackageDialog.FileName);
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
                IssueException(ex, "Export cannot begin.  Could not open target package:\n" + exportToPackageDialog.FileName);
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
            IResourceIndexEntry rie = tgtpkg.Find(x => ((IResourceKey)srcrie).Equals(x));
            if (rie != null)
            {
                if (!replace) return;
                tgtpkg.DeleteResource(rie);
            }

            rie = tgtpkg.AddResource(srcrie, null, true);
            if (rie == null) return;
            rie.Compressed = srcrie.Compressed;

            IResource srcres = s3pi.WrapperDealer.WrapperDealer.GetResource(0, CurrentPackage, srcrie, true);//Don't need wrapper
            tgtpkg.ReplaceResource(rie, srcres);
        }

        private void resourceHexEdit()
        {
            if (resource == null) return;
            HexEdit(browserWidget1.SelectedResource, resource);
        }

        private void resourceTextEdit()
        {
            if (resource == null) return;
            TextEdit(browserWidget1.SelectedResource, resource);
        }
        #endregion

        private void menuBarWidget1_HelperClick(object sender, MenuBarWidget.HelperClickEventArgs helper)
        {
            try
            {
                this.Enabled = false;
                Application.DoEvents();
                do_HelperClick(helper.helper);
            }
            finally { this.Enabled = true; }
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
                    case MenuBarWidget.MB.MBS_updates: settingsAutomaticUpdates(); break;
                    case MenuBarWidget.MB.MBS_bookmarks: settingsOrganiseBookmarks(); break;
                    case MenuBarWidget.MB.MBS_externals: settingsExternalPrograms(); break;
                    case MenuBarWidget.MB.MBS_wrappers: settingsManageWrappers(); break;
                    case MenuBarWidget.MB.MBS_previewDDS: settingsEnableDDSPreview(); break;
                    case MenuBarWidget.MB.MBS_saveSettings: saveSettings(); break;
                }
            }
            finally { this.Enabled = true; }
        }

        private void settingsAutomaticUpdates()
        {
            AutoUpdate.Checker.AutoUpdateChoice = !menuBarWidget1.IsChecked(MenuBarWidget.MB.MBS_updates);
        }

        private void settingsOrganiseBookmarks()
        {
            Settings.OrganiseBookmarksDialog obd = new S3PIDemoFE.Settings.OrganiseBookmarksDialog();
            obd.ShowDialog();
            menuBarWidget1.UpdateBookmarks();
        }

        bool hasHexEditor { get { return S3PIDemoFE.Properties.Settings.Default.HexEditorCmd != null && S3PIDemoFE.Properties.Settings.Default.HexEditorCmd.Length > 0; } }
        bool hasTextEditor { get { return S3PIDemoFE.Properties.Settings.Default.TextEditorCmd != null && S3PIDemoFE.Properties.Settings.Default.TextEditorCmd.Length > 0; } }
        private void settingsExternalPrograms()
        {
            Settings.ExternalProgramsDialog epd = new S3PIDemoFE.Settings.ExternalProgramsDialog();
            if (hasHexEditor)
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
                epd.HexEditorWantsQuotes = false;
            }
            if (hasTextEditor)
            {
                epd.HasUserTextEditor = true;
                epd.UserTextEditor = S3PIDemoFE.Properties.Settings.Default.TextEditorCmd;
                epd.TextEditorIgnoreTS = S3PIDemoFE.Properties.Settings.Default.TextEditorIgnoreTS;
                epd.TextEditorWantsQuotes = S3PIDemoFE.Properties.Settings.Default.TextEditorWantsQuotes;
            }
            else
            {
                epd.HasUserTextEditor = false;
                epd.UserTextEditor = "";
                epd.TextEditorIgnoreTS = false;
                epd.TextEditorWantsQuotes = false;
            }
            if (S3PIDemoFE.Properties.Settings.Default.DisabledHelpers == null)
                S3PIDemoFE.Properties.Settings.Default.DisabledHelpers = new System.Collections.Specialized.StringCollection();

            string[] disabledHelpers = new string[S3PIDemoFE.Properties.Settings.Default.DisabledHelpers.Count];
            S3PIDemoFE.Properties.Settings.Default.DisabledHelpers.CopyTo(disabledHelpers, 0);
            epd.DisabledHelpers = disabledHelpers;
            DialogResult dr = epd.ShowDialog();
            if (dr != DialogResult.OK) return;
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
            if (epd.HasUserTextEditor && epd.UserTextEditor.Length > 0 && File.Exists(epd.UserTextEditor))
            {
                S3PIDemoFE.Properties.Settings.Default.TextEditorCmd = epd.UserTextEditor;
                S3PIDemoFE.Properties.Settings.Default.TextEditorIgnoreTS = epd.TextEditorIgnoreTS;
                S3PIDemoFE.Properties.Settings.Default.TextEditorWantsQuotes = epd.TextEditorWantsQuotes;
            }
            else
            {
                S3PIDemoFE.Properties.Settings.Default.TextEditorCmd = null;
                S3PIDemoFE.Properties.Settings.Default.TextEditorIgnoreTS = false;
                S3PIDemoFE.Properties.Settings.Default.TextEditorWantsQuotes = false;
            }
            disabledHelpers = epd.DisabledHelpers;
            if (disabledHelpers.Length == 0)
                S3PIDemoFE.Properties.Settings.Default.DisabledHelpers = null;
            else
            {
                S3PIDemoFE.Properties.Settings.Default.DisabledHelpers = new System.Collections.Specialized.StringCollection();
                S3PIDemoFE.Properties.Settings.Default.DisabledHelpers.AddRange(epd.DisabledHelpers);
            }
            if (browserWidget1.SelectedResource != null && resource != null) { setHexEditor(); setTextEditor(); setHelpers(); }
        }

        private void settingsManageWrappers()
        {
            new Settings.ManageWrappersDialog().ShowDialog();
            IResourceIndexEntry rie = browserWidget1.SelectedResource;
            browserWidget1.SelectedResource = null;
            browserWidget1.SelectedResource = rie;
        }

        bool ddsEnableWarningIssued = false;
        private void settingsEnableDDSPreview()
        {
            if (!S3PIDemoFE.Properties.Settings.Default.EnableDDSPreview && !ddsEnableWarningIssued)
            {
                ddsEnableWarningIssued = true;
                if (CopyableMessageBox.Show("The DDS Preview feature is in early testing.\n" + "Please save your work frequently if you enable it.\n" +
                    "\nClick OK to continue or Cancel to leave the feature disabled.", "Enable DDS Preview",
                    CopyableMessageBoxButtons.OKCancel, CopyableMessageBoxIcon.Warning) != 0) return;
            }
            S3PIDemoFE.Properties.Settings.Default.EnableDDSPreview = !menuBarWidget1.IsChecked(MenuBarWidget.MB.MBS_previewDDS);
            menuBarWidget1.Checked(MenuBarWidget.MB.MBS_previewDDS, S3PIDemoFE.Properties.Settings.Default.EnableDDSPreview);
            if (S3PIDemoFE.Properties.Settings.Default.EnableDDSPreview)
            {
                IResourceIndexEntry rie = browserWidget1.SelectedResource;
                browserWidget1.SelectedResource = null;
                browserWidget1.SelectedResource = rie;
            }
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
                "under certain conditions; see Help->Licence for details.\n" +
                "\n" +
                "Please see Acknowledgements.txt and Acknowledgements-s3pe.txt for details of libraries used.\n";
            CopyableMessageBox.Show(String.Format(
                "{0}\n"+
                "Front-end Distribution: {1}\n" +
                "Library Distribution: {2}"
                , copyright
                , getVersion(typeof(MainForm), "s3pe")
                , getVersion(typeof(s3pi.Interfaces.AApiVersionedFields), "s3pe")
                ), this.Text);
        }

        private static string getVersion(Type type, string p)
        {
            string s = getString(Path.Combine(Path.GetDirectoryName(type.Assembly.Location), p + "-Version.txt"));
            return s == null ? "Unknown" : s;
        }

        private static string getString(string file)
        {
            if (!File.Exists(file)) return null;
            FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
            StreamReader t = new StreamReader(fs);
            string res = t.ReadLine();
            fs.Close();
            return res;
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

        bool ddsFailedWarningIssued = false;
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

            controlPanel1_AutoChanged(null, null);
            if (resource != null)
            {
                controlPanel1.HexEnabled = true;
                controlPanel1.ValueEnabled = hasValueContentField();
                controlPanel1.GridEnabled = resource.ContentFields.Find(x => !x.Equals("AsBytes") && !x.Equals("Stream") && !x.Equals("Value")) != null;
                setHexEditor();
                setTextEditor();
                setHelpers();
            }
            else
            {
                controlPanel1.HexEnabled = controlPanel1.ValueEnabled = controlPanel1.GridEnabled =
                    controlPanel1.Helper1Enabled = controlPanel1.Helper2Enabled = controlPanel1.HexEditEnabled = false;
                menuBarWidget1.Enable(MenuBarWidget.MB.MBR_hexEditor, false);
                menuBarWidget1.Enable(MenuBarWidget.MB.MBR_textEditor, false);
                helpers = null;
                menuBarWidget1.ClearHelpers();
            }

            bool selectedItems = resource != null || browserWidget1.SelectedResources.Count > 0; // one or more
            menuBarWidget1.Enable(MenuBarWidget.MB.MBR_exportResources, selectedItems);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBR_exportToPackage, selectedItems);
            //menuBarWidget1.Enable(MenuBarWidget.MB.MBE_cut, resource != null);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBR_copy, selectedItems);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBR_duplicate, resource != null);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBR_replace, resource != null);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBR_compressed, selectedItems);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBR_isdeleted, selectedItems);
            menuBarWidget1.Enable(MenuBarWidget.MB.MBR_details, resource != null);

            resourceFilterWidget1.IndexEntry = browserWidget1.SelectedResource;
        }

        bool hasValueContentField()
        {
            if (ddsResources.Contains(browserWidget1.SelectedResource["ResourceType"] + "") && S3PIDemoFE.Properties.Settings.Default.EnableDDSPreview)
                return true;
            //if (!AApiVersionedFields.GetContentFields(0, resource.GetType()).Contains("Value")) return false;
            //-prefer to use the per-resource ContentFields property:
            if (!resource.ContentFields.Contains("Value")) return false;
            Type t = AApiVersionedFields.GetContentFieldTypes(0, resource.GetType())["Value"];
            if (typeof(String).IsAssignableFrom(t) || typeof(Image).IsAssignableFrom(t)) return true;
            return false;
        }

        private void browserWidget1_DragOver(object sender, DragEventArgs e)
        {
            //if (package == null) return;
            if ((new List<string>(e.Data.GetFormats())).Contains("FileDrop"))
                e.Effect = DragDropEffects.Copy;
        }

        // For browserWidget1_DragDrop(), see Import/Import.cs

        private void browserWidget1_ItemActivate(object sender, EventArgs e)
        {
            resourceDetails();
        }

        private void browserWidget1_DeletePressed(object sender, EventArgs e)
        {
            resourceIsDeleted();
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
            pnAutoCleanUp();
            if (resException != null)
            {
                IResourceIndexEntry rie = browserWidget1.SelectedResource;
                string s = "";
                if (rie != null) s += String.Format("Error reading resource {0:X8}:{1:X8}:{2:X16}", rie.ResourceType, rie.ResourceGroup, rie.Instance);
                s += String.Format("\r\nFront-end Distribution: {0}\r\nLibrary Distribution: {1}\r\n",
                    getVersion(typeof(MainForm), "s3pe"), getVersion(typeof(s3pi.Interfaces.AApiVersionedFields), "s3pe"));
                for (Exception inex = resException; inex != null; inex = inex.InnerException)
                {
                    s += "\r\nSource: " + inex.Source;
                    s += "\r\nAssembly: " + inex.TargetSite.DeclaringType.Assembly.FullName;
                    s += "\r\n" + inex.Message;
                    s += "\r\n----\r\nStack trace:\r\n" + inex.StackTrace + "\r\n----\r\n";
                }
                TextBox tb = new TextBox();
                tb.Dock = DockStyle.Fill;
                tb.Multiline = true;
                tb.ReadOnly = true;
                tb.ScrollBars = ScrollBars.Vertical;
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
                    hw.ContextMenuStrip = menuBarWidget1.textPreviewContextMenuStrip;
                    pnAuto.Controls.Add(hw);
                }
                else if (!controlPanel1.HexOnly && controlPanel1.AutoValue && hasValueContentField())
                {
                    Control c = getValueControl();
                    c.ContextMenuStrip = menuBarWidget1.textPreviewContextMenuStrip;
                    if (c != null)
                        pnAuto.Controls.Add(c);
                }
            }
            pnAuto.ResumeLayout();
        }

        private void pnAutoCleanUp()
        {
            if (pnAuto.Controls.Count == 0) return;

            foreach (Control c in pnAuto.Controls)
                c.Dispose();
            pnAuto.Controls.Clear();
            GC.Collect();
            GC.WaitForPendingFinalizers();
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
                f.FormClosed += new FormClosedEventHandler(f_FormClosed);
                f.Show(this);
            }
            finally { this.Enabled = true; }
        }

        void f_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!(sender as Form).IsDisposed) (sender as Form).Dispose();
        }

        class DDSControl
        {
            static bool channel1 = true, channel2 = true, channel3 = true, channel4 = true;
            DDSPanel.DDSPanel control;
            public DDSControl()
            {
                control = new DDSPanel.DDSPanel() { Fit = true, Channel1 = channel1, Channel2 = channel2, Channel3 = channel3, Channel4 = channel4 };
                control.Channel1Changed += new EventHandler(control_Channel1Changed);
                control.Channel2Changed += new EventHandler(control_Channel2Changed);
                control.Channel3Changed += new EventHandler(control_Channel3Changed);
                control.Channel4Changed += new EventHandler(control_Channel4Changed);
            }
            void control_Channel1Changed(object sender, EventArgs e) { channel1 = control.Channel1; }
            void control_Channel2Changed(object sender, EventArgs e) { channel2 = control.Channel2; }
            void control_Channel3Changed(object sender, EventArgs e) { channel3 = control.Channel3; }
            void control_Channel4Changed(object sender, EventArgs e) { channel4 = control.Channel4; }

            public DDSPanel.DDSPanel Control { get { return control; } }
        }
        Control getValueControl()
        {
            Control res = null;
            if (ddsResources.Contains(browserWidget1.SelectedResource["ResourceType"] + "") && S3PIDemoFE.Properties.Settings.Default.EnableDDSPreview)
            {
                try
                {
                    DDSControl dds = new DDSControl();
                    dds.Control.DDSLoad(resource.Stream);
                    res = (Control)dds.Control;
                }
                catch(Exception e)
                {
                    if (!ddsFailedWarningIssued)
                    {
                        ddsFailedWarningIssued = true;
                        ddsEnableWarningIssued = false;
                        CopyableMessageBox.IssueException(e, "DDS Preview failed:\n" + e.Message + "\n\nDDS Preview has been disabled\n"
                            , "Cannot preview DDS");
                        S3PIDemoFE.Properties.Settings.Default.EnableDDSPreview = false;
                        menuBarWidget1.Checked(MenuBarWidget.MB.MBS_previewDDS, S3PIDemoFE.Properties.Settings.Default.EnableDDSPreview);
                    }
                    return null;
                }
            }
            else if (hasValueContentField())
            {
                Type t = AApiVersionedFields.GetContentFieldTypes(0, resource.GetType())["Value"];
                if (typeof(String).IsAssignableFrom(t))
                {
                    RichTextBox rtb = new RichTextBox();
                    rtb.Text = "" + resource["Value"];
                    rtb.Font = new Font(FontFamily.GenericMonospace, 8);
                    rtb.Size = new Size(this.Width - (this.Width / 5), this.Height - (this.Height / 5));
                    rtb.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                    rtb.ReadOnly = true;
                    res = rtb;
                }
                else if (typeof(Image).IsAssignableFrom(t))
                {
                    PictureBox pb = new PictureBox();
                    pb.Image = (Image)resource["Value"].Value;
                    pb.Size = pb.Image.Size;
                    res = pb;
                }
            }
            if (res != null) res.Dock = DockStyle.Fill;
            return res;
        }


        private void controlPanel1_GridClick(object sender, EventArgs e)
        {
            try
            {
                this.Enabled = false;
                DialogResult dr = (new NewGridForm(resource as AApiVersionedFields, true)).ShowDialog();
                if (dr != DialogResult.OK)
                    resourceIsDirty = false;
                else
                    controlPanel1_CommitClick(null, null);
                IResourceIndexEntry rie = browserWidget1.SelectedResource;
                browserWidget1.SelectedResource = null;
                browserWidget1.SelectedResource = rie;
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

        private void controlPanel1_HexEditClick(object sender, EventArgs e) { HexEdit(browserWidget1.SelectedResource, resource); }
        #endregion

        #region Helpers
        void setHexEditor() { menuBarWidget1.Enable(MenuBarWidget.MB.MBR_hexEditor, hasHexEditor); controlPanel1.HexEditEnabled = hasHexEditor; }

        void setTextEditor() { menuBarWidget1.Enable(MenuBarWidget.MB.MBR_textEditor, hasTextEditor); }

        void setHelpers()
        {
            helpers = new s3pi.Helpers.HelperManager(browserWidget1.SelectedResource, resource, browserWidget1.ResourceName(browserWidget1.SelectedResource));
            if (S3PIDemoFE.Properties.Settings.Default.DisabledHelpers != null)
                foreach (string id in S3PIDemoFE.Properties.Settings.Default.DisabledHelpers)
                {
                    List<s3pi.Helpers.HelperManager.Helper> disabled = new List<s3pi.Helpers.HelperManager.Helper>();
                    foreach (var helper in helpers) if (helper.id == id) disabled.Add(helper);
                    foreach (var helper in disabled) helpers.Remove(helper);
                }
            controlPanel1.Helper1Enabled = helpers.Count > 0;
            controlPanel1.Helper1Label = helpers.Count > 0 && helpers[0].label.Length > 0 ? helpers[0].label : "Helper1";
            controlPanel1.Helper1Tip = helpers.Count > 0 && helpers[0].desc.Length > 0 ? helpers[0].desc : "";

            controlPanel1.Helper2Enabled = helpers.Count > 1;
            controlPanel1.Helper2Label = helpers.Count > 1 && helpers[1].label.Length > 0 ? helpers[1].label : "Helper2";
            controlPanel1.Helper1Tip = helpers.Count > 1 && helpers[1].desc.Length > 0 ? helpers[1].desc : "";

            menuBarWidget1.SetHelpers(helpers);
        }

        private void controlPanel1_Helper1Click(object sender, EventArgs e)
        {
            do_HelperClick(0);
        }

        private void controlPanel1_Helper2Click(object sender, EventArgs e)
        {
            do_HelperClick(1);
        }

        void do_HelperClick(int i)
        {
            try
            {
                this.Enabled = false;
                Application.DoEvents();

                MemoryStream ms = helpers.execHelper(i);
                MakeFormVisible();
                Application.DoEvents();

                if (!helpers[i].isReadOnly) afterEdit(ms);
            }
            finally { this.Enabled = true; }
        }

        void TextEdit(IResourceKey key, IResource res)
        {
            try
            {
                this.Enabled = false;
                Application.DoEvents();

                MemoryStream ms = s3pi.Helpers.HelperManager.Edit(key, res,
                    S3PIDemoFE.Properties.Settings.Default.TextEditorCmd,
                    S3PIDemoFE.Properties.Settings.Default.TextEditorWantsQuotes,
                    S3PIDemoFE.Properties.Settings.Default.TextEditorIgnoreTS);

                MakeFormVisible();
                Application.DoEvents();

                afterEdit(ms);
            }
            finally { this.Enabled = true; }
        }

        void HexEdit(IResourceKey key, IResource res)
        {
            try
            {
                this.Enabled = false;
                Application.DoEvents();

                MemoryStream ms = s3pi.Helpers.HelperManager.Edit(key, res,
                    S3PIDemoFE.Properties.Settings.Default.HexEditorCmd,
                    S3PIDemoFE.Properties.Settings.Default.HexEditorWantsQuotes,
                    S3PIDemoFE.Properties.Settings.Default.HexEditorIgnoreTS);

                MakeFormVisible();
                Application.DoEvents();

                afterEdit(ms);
            }
            finally { this.Enabled = true; }
        }

        void afterEdit(MemoryStream ms)
        {
            if (ms != null)
            {
                int dr = CopyableMessageBox.Show("Resource has been updated.  Commit changes?", "Commit changes?",
                    CopyableMessageBoxButtons.YesNo, CopyableMessageBoxIcon.Question, 0);

                if (dr != 0) return;

                IResourceIndexEntry rie = NewResource(browserWidget1.SelectedResource, ms, DuplicateHandling.replace, browserWidget1.SelectedResource.Compressed != 0);
                if (rie != null) browserWidget1.Add(rie);
            }
        }

        void MakeFormVisible() { try { ForceFocus.Focus(this); } catch { } }
        #endregion
    }
}
