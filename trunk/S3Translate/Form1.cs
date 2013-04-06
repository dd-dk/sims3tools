/*
 *  Copyright 2009 Jonathan Haas
 *  Copyright (C) 2010-2013 by Peter L Jones
 * 
 *  This file is part of s3translate.
 *  
 *  s3translate is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  s3translate is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with s3translate.  If not, see <http://www.gnu.org/licenses/>.
 *  
 *  s3translate uses the s3pi libraries by Peter L Jones (pljones@users.sf.net)
 *  For details visit http://sourceforge.net/projects/s3pi/
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;
using s3pi.Interfaces;
using s3pi.Package;
using S3Translate.Properties;

namespace S3Translate
{
    public partial class Form1 : Form
    {
        #region locales
        public static List<String> locales = new List<String> {
            "English (en-us)",
            "Chinese (zh-cn)",
            "Chinese Taiwan (zh-tw)",
            "Czech (cs-cz)",
            "Danish (da-dk)",
            "Dutch (nl-nl)",
            "Finnish (fi-fi)",
            "French (fr-fr)",
            "German (de-de)",
            "Greek (el-gr)",
            "Hungarian (hu-hu)",
            "Italian (it-it)",
            "Japanese (ja-ja)",
            "Korean (ko-kr)",
            "Norwegian (no-no)",
            "Polish (pl-pl)",
            "Portuguese (pt-pt)",
            "Portuguese Brazil (pt-br)",
            "Russian (ru-ru)",
            "Spanish (es-es)",
            "Spanish Mexico (es-mx)",
            "Swedish (sv-se)",
            "Thai (th-th)"
        };
        #endregion


        string _fname;
        EventHandler FileNameChanged;
        void OnFileNameChanged() { if (FileNameChanged != null) FileNameChanged(this, EventArgs.Empty); }
        string fileName { get { return _fname; } set { if (_fname != value) { _fname = value; OnFileNameChanged(); } } }

        IPackage _currentPackage;
        EventHandler CurrentPackageChanged;
        void OnCurrentPackageChanged() { if (CurrentPackageChanged != null) CurrentPackageChanged(this, EventArgs.Empty); }
        IPackage currentPackage { get { return _currentPackage; } set { if (_currentPackage != value) { _currentPackage = value; OnCurrentPackageChanged(); } } }

        private bool _pkgDirty;
        private bool pkgIsDirty { get { return _pkgDirty; } set { _pkgDirty = value; SetText(); } }

        private bool _txtDirty;
        private bool txtIsDirty { get { return _txtDirty; } set { _txtDirty = value; SetText(); btnCommit.Enabled = _txtDirty; } }


        private const string packageFilter = "Sims 3 Packages|*.package|All Files|*.*";
        private const string stblFilter = "Exported String Tables|S3_220557DA_*.stbl|Any stbl (*.stbl)|*.stbl|All Files|*.*";

        const uint STBL = 0x220557DA;
        private List<ListViewItem> foundItems = null;

        // Dictionary<stblIID, Dictionary<lang, stbl>>
        Dictionary<IResourceKey, Dictionary<int, StblResource.StblResource>> StringTables = new Dictionary<IResourceKey, Dictionary<int, StblResource.StblResource>>(new rkMatch());

        class rkMatch : IEqualityComparer<IResourceKey>
        {
            public bool Equals(IResourceKey x, IResourceKey y) { return x.Equals(y); }
            public int GetHashCode(IResourceKey obj) { return obj.GetHashCode(obj); }
        }

        IResourceKey stblGroupKeyToRK(IResourceKey stblGroupKey, int lang)
        {
            return new RK(stblGroupKey.ResourceType, stblGroupKey.ResourceGroup, stblGroupKey.Instance | ((ulong)lang << 56));
        }

        IResourceKey RKTostblGroupKey(IResourceKey stblGroupKey)
        {
            return new RK(stblGroupKey.ResourceType, stblGroupKey.ResourceGroup, stblGroupKey.Instance & 0x00FFFFFFFFFFFFFF);
        }

        public Form1()
        {
            AcceptLicence();
            InitializeComponent();
            cmbSourceLang.DataSource = locales.AsReadOnly();
            cmbTargetLang.DataSource = locales.AsReadOnly();
            try
            {
                cmbSourceLang.SelectedIndex = Settings.Default.SourceLocale;
                cmbTargetLang.SelectedIndex = Settings.Default.UserLocale;
                ckbAutoCommit.Checked = Settings.Default.AutoCommit;
            }
            catch (Exception)
            {
                new SettingsDialog().ShowDialog();
            }

            FileNameChanged += new EventHandler((sender, e) => SetText());

            CurrentPackageChanged += new EventHandler((sender, e) =>
            {
                _pkgDirty = _txtDirty = false;
                SetText();
                ReloadStringTables();
                btnRevertAll.Enabled = btnRevertLang.Enabled = tlpFind.Enabled = currentPackage != null;
            });

            lstSTBLs.ItemSelectionChanged += new ListViewItemSelectionChangedEventHandler((sender, e) => { btnAddString.Enabled = lstSTBLs.SelectedIndices.Count == 1; ReloadStrings(); });

            cmbSourceLang.SelectedIndex = Settings.Default.SourceLocale;
            lstStrings.Columns[0].Text = "Source: " + cmbSourceLang.Text;
            cmbSourceLang.SelectedIndexChanged += new EventHandler((sender, e) => { lstStrings.Columns[0].Text = cmbSourceLang.Text; ReloadStrings(); });

            cmbTargetLang.SelectedIndex = Settings.Default.UserLocale;
            lstStrings.Columns[1].Text = "Target: " + cmbTargetLang.Text;
            cmbTargetLang.SelectedIndexChanged += new EventHandler((sender, e) => { lstStrings.Columns[1].Text = cmbTargetLang.Text; ReloadStrings(); });

            lstStrings.ItemSelectionChanged += new ListViewItemSelectionChangedEventHandler((sender, e) => { AskCommit(); SelectStrings(); });

            btnCommit.Click += new EventHandler((sender, e) => { CommitText(); });
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            openToolStripMenuItem_Click(null, null);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!AskSavePackage()) { e.Cancel = true; return; }

            ClosePackage();
        }

        #region File menu
        private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            bool isopen = (_currentPackage != null);
            closeToolStripMenuItem.Enabled
                = exportLanguageToolStripMenuItem.Enabled
                = savePackageAsToolStripMenuItem.Enabled
                = savePackageToolStripMenuItem.Enabled
                = importPackageToolStripMenuItem.Enabled
                = importSTBLToolStripMenuItem.Enabled
                = isopen;
            savePackageToolStripMenuItem.Enabled = savePackageToolStripMenuItem.Enabled && pkgIsDirty;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!AskSavePackage()) return;

            var ofd = new OpenFileDialog() { Filter = packageFilter };
            if (ofd.ShowDialog() == DialogResult.OK) OpenPackage(ofd.FileName);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!AskSavePackage()) return;

            ClosePackage();
            fileName = "";
        }

        private void savePackageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AskCommit();
            _currentPackage.SavePackage();
            pkgIsDirty = false;
        }

        private void savePackageAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AskCommit();
            var sfd = new SaveFileDialog();
            sfd.Filter = packageFilter;
            sfd.FileName = fileName;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                _currentPackage.SaveAs(sfd.FileName);
                fileName = sfd.FileName;
            }
        }

        private void importPackageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lstStrings.SelectedIndices.Clear();

            #region Get the file name
            var ofd = new OpenFileDialog() { Filter = packageFilter };
            if (ofd.ShowDialog() != DialogResult.OK) return;
            if (ofd.FileName == fileName)
            {
                MessageBox.Show("That is the current package.", "S3Translate", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            IPackage importFrom = Package.OpenPackage(0, ofd.FileName);
            List<IResourceIndexEntry> lrie = new List<IResourceIndexEntry>(importFrom.FindAll(x => x.ResourceType == STBL));

            if (lrie.Count == 0)
            {
                MessageBox.Show("The selected package contains no STBLs.", "S3Translate", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            #endregion

            List<byte> lb = new List<byte>();
            foreach (var rie in lrie) { byte x = (byte)(rie.Instance >> 56); if (!lb.Contains(x)) lb.Add(x); }
            bool all = true;
            if (lb.Count > 1)
            {
                DialogResult dr = MessageBox.Show("Import all languages?", "Import", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.Cancel) return;
                all = dr == DialogResult.Yes;
            }

            if (!all)
            {
                var imp = new Export() { Text = "Import" };
                for (byte i = 0; i < locales.Count; i++)
                    imp.checkList.SetItemChecked(i, lb.Contains(i));

                if (imp.ShowDialog() != DialogResult.OK) return;

                lb.Clear();
                foreach (int i in imp.checkList.CheckedIndices)
                    lb.Add((byte)i);

                lrie = new List<IResourceIndexEntry>(importFrom.FindAll(x => x.ResourceType == STBL && lb.Contains((byte)(x.Instance >> 56))));
            }

            #region Check for duplicates
            if (currentPackage.FindAll(x =>
            {
                if (x.ResourceType != STBL) return false;
                foreach (var rie in lrie) if (x.Equals(rie)) return true;
                return false;
            }).Count > 0)
            {
                MessageBox.Show("Duplicate resources detected.", "S3Translate", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            #endregion

            foreach (var rie in lrie)
            {
                currentPackage.AddResource(rie, ((APackage)importFrom).GetResource(rie), true);
            }

            pkgIsDirty = true;
            ReloadStringTables();
        }

        private void importSTBLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lstStrings.SelectedIndices.Clear();

            var ofd = new OpenFileDialog() { Filter = stblFilter };
            if (ofd.ShowDialog() != DialogResult.OK) return;
            List<string> fns = new List<string>(new string[] { ofd.FileName, });

            ulong instance = FNV64.GetHash(ofd.FileName + DateTime.Now.ToBinary().ToString());
            instance = (instance >> 8) ^ (instance & 0xFF);
            RK rk = new RK(STBL, 0, instance);

            #region Get RK from chosen filename and add other languages
            string[] fn = Path.GetFileName(ofd.FileName).Split('_');
            if (fn.Length > 3 && fn[0] == "S3")
            {
                if (fn[1].Length != 8 || fn[1] != STBL.ToString("X8"))
                    if (MessageBox.Show("This does not appear to be a STBL.\n\nContinue?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

                uint g = 0;
                if (fn[2].Length != 8 || !uint.TryParse(fn[2], System.Globalization.NumberStyles.HexNumber, null, out g))
                    if (MessageBox.Show("Invalid Group number.\n\nContinue?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
                rk.ResourceGroup = g;

                ulong i = 0;
                if (fn.Length > 3 && !ulong.TryParse(fn[3], System.Globalization.NumberStyles.HexNumber, null, out i))
                {
                    if (MessageBox.Show("Invalid STBL Instance.\n\nContinue?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
                    else i = instance;
                }
                if ((int)(i >> 56) > locales.Count)
                    if (MessageBox.Show("Invalid Language ID.\n\nContinue?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
                rk.Instance = i;

                if (MessageBox.Show("Attempt to add all languages?", "Import", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string stblInstance = (rk.Instance & 0x00FFFFFFFFFFFFFF).ToString("X14");
                    foreach (var f in Directory.GetFiles(Path.GetDirectoryName(ofd.FileName), String.Format("S3_{0:X8}_{1:X8}_*.stbl", STBL, rk.ResourceGroup)))
                    {
                        string[] afn = f.Split('_');
                        if (afn[3].Substring(2).Equals(stblInstance) && !fns.Contains(f))
                            fns.Add(f);
                    }
                }
            }
            else
                if (MessageBox.Show("File name is not in export format.\n\nContinue?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            #endregion

            #region Check for duplicates
            if (fns.Count > 1)
            {
                bool found = false;
                if (currentPackage.Find(x => x.ResourceType == STBL && x.Equals(rk)) != null)
                {
                    found = true;
                }
                else
                {
                    for (int i = 1; i < fns.Count; i++)
                    {
                        string[] afn = fns[i].Split('_');
                        if (currentPackage.Find(x => x.ResourceType == STBL && x.ResourceGroup.ToString("X8").Equals(afn[2]) && x.Instance.ToString("X16").Equals(afn[3])) != null)
                        {
                            found = true;
                            break;
                        }
                    }
                }
                if (found)
                {
                    MessageBox.Show("Duplicate resources detected.", "S3Translate", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                if (currentPackage.Find(x => x.ResourceType == STBL && x.Equals(rk)) != null)
                {
                    MessageBox.Show("Duplicate resources detected.", "S3Translate", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            #endregion

            using (FileStream fs = new FileStream(fns[0], FileMode.Open, FileAccess.Read))
            {
                byte[] stbl = new byte[fs.Length];
                fs.Read(stbl, 0, (int)fs.Length);
                currentPackage.AddResource(rk, new MemoryStream(stbl), true);
                fs.Close();
            }
            for(int i = 1; i< fns.Count;i++)
            {
                string f = fns[i];
                string[] afn = f.Split('_');
                rk.ResourceGroup = uint.Parse(afn[2], System.Globalization.NumberStyles.HexNumber);
                rk.Instance = ulong.Parse(afn[3], System.Globalization.NumberStyles.HexNumber);

                using (FileStream fs = new FileStream(f, FileMode.Open, FileAccess.Read))
                {
                    byte[] stbl = new byte[fs.Length];
                    fs.Read(stbl, 0, (int)fs.Length);
                    currentPackage.AddResource(rk, new MemoryStream(stbl), true);
                    fs.Close();
                }
            }

            pkgIsDirty = true;
            ReloadStringTables();
        }

        private void exportLanguageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ef = new Export() { Text = "Export" };
            if (ef.ShowDialog() != DialogResult.OK) return;

            var fd = new FolderBrowserDialog();
            if (fd.ShowDialog() != DialogResult.OK) return;

            var stblgroup = (KeyValuePair<IResourceKey, Dictionary<int, StblResource.StblResource>>)lstSTBLs.SelectedItems[0].Tag;
            foreach (int lang in ef.checkList.CheckedIndices)
            {
                using (var fs = new FileStream(Path.Combine(fd.SelectedPath,
                    String.Format("S3_{0:X8}_{1:X8}_{2:X2}{3:X14}_{4}.stbl", stblgroup.Key.ResourceType, stblgroup.Key.ResourceGroup, lang, stblgroup.Key.Instance, locales[lang])
                    ), FileMode.CreateNew))
                {
                    new BinaryWriter(fs).Write(StringTables[stblgroup.Key][lang].AsBytes);
                    fs.Close();
                }
            }
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new SettingsDialog().ShowDialog();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!AskSavePackage()) return;

            ClosePackage();
            Application.Exit();
        }
        #endregion

        #region Help menu
        private void contentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string locale = System.Globalization.CultureInfo.CurrentUICulture.Name;

            string baseFolder = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "HelpFiles");
            if (Directory.Exists(Path.Combine(baseFolder, locale)))
                baseFolder = Path.Combine(baseFolder, locale);
            else if (Directory.Exists(Path.Combine(baseFolder, locale.Substring(0, 2))))
                baseFolder = Path.Combine(baseFolder, locale.Substring(0, 2));

            Help.ShowHelp(this, "file:///" + Path.Combine(baseFolder, "Contents.htm"));
        }

        private void licenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowLicence();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string copyright = @"
Copyright 2009 Jonathan Haas
Copyright (C) 2010-2013 by Peter L Jones

This program comes with ABSOLUTELY NO WARRANTY

This is free software, and you are welcome to redistribute it
under certain conditions; see Help->Licence for details.

";

            MessageBox.Show(String.Format(
                "{0}\n" +
                "Front-end Distribution: {1}\n" +
                "Library Distribution: {2}"
                , copyright
                , Version.CurrentVersion
                , Version.LibraryVersion
                ), "S3Translate");
        }
        #endregion

        #region Designer-bound event handlers
        private void txtTarget_TextChanged(object sender, EventArgs e)
        {
            if (inChangeHandler) return;

            if (ckbAutoCommit.Checked)
                CommitText();
            else
                txtIsDirty = true;
        }

        private void btnOverwriteLang_Click(object sender, EventArgs e)
        {
            if (sender != null)
                if (MessageBox.Show("This will overwrite all " + locales[cmbTargetLang.SelectedIndex]
                + " texts from the " + locales[cmbSourceLang.SelectedIndex] + " defaults.\n\nContinue?",
                "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                    return;

            var stblgroup = (KeyValuePair<IResourceKey, Dictionary<int, StblResource.StblResource>>)lstSTBLs.SelectedItems[0].Tag;
            if (!StringTables[stblgroup.Key].ContainsKey(cmbSourceLang.SelectedIndex))
            {
                int rlocale = StringTables[stblgroup.Key].Keys.GetEnumerator().Current;
                if (MessageBox.Show("The current source language doesn't exist in this string table. Use \"" + locales[rlocale] + "\" instead ?", "Language missing", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    cmbSourceLang.SelectedIndex = rlocale;
                    lstStrings.Items.Clear();
                }
                else
                    return;
            }

            MemoryStream ms = new MemoryStream(StringTables[stblgroup.Key][cmbSourceLang.SelectedIndex].AsBytes, true);
            StblResource.StblResource targetSTBL = new StblResource.StblResource(0, ms);

            IResourceKey rk = stblGroupKeyToRK(stblgroup.Key, cmbTargetLang.SelectedIndex);
            IResourceIndexEntry rie = _currentPackage.Find(x => x.Equals(rk));
            if (rie != null)
                _currentPackage.ReplaceResource(rie, targetSTBL);
            else
                _currentPackage.AddResource(rk, targetSTBL.Stream, true);
            StringTables[stblgroup.Key][cmbTargetLang.SelectedIndex] = targetSTBL;

            pkgIsDirty = true;
            ReloadStrings();
            SelectStrings();
        }

        private void btnOverwriteAll_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will overwrite all(!) texts in all(!) languages from the " + locales[cmbSourceLang.SelectedIndex] + " defaults. Continue?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                return;
            var a = cmbTargetLang.SelectedIndex;
            for (var i = 0; i < locales.Count; i++)
            {
                cmbTargetLang.SelectedIndex = i;
                btnOverwriteLang_Click(null, null);
            }
            cmbTargetLang.SelectedIndex = a;
        }

        delegate void _KEYRemoveInstance(ulong instance);
        delegate void _KEYAddInstance(ulong instance, string name);
        delegate void _KEYCommitNameMaps();
        
        private void btnMergeSTLBs_Click(object sender, EventArgs e)
        {
            #region Check for duplicates
            Dictionary<ulong, List<int>> allGUIDs = new Dictionary<ulong, List<int>>();
            List<ulong> duplicates = new List<ulong>();
            foreach (var slr in StringTables)
            {
                foreach (var lr in slr.Value)
                {
                    foreach (var gs in lr.Value)
                    {
                        if (allGUIDs.ContainsKey(gs.Key))
                        {
                            if (allGUIDs[gs.Key].Contains(lr.Key))
                                duplicates.Add(gs.Key);
                            else
                                allGUIDs[gs.Key].Add(lr.Key);
                        }
                        else
                            allGUIDs.Add(gs.Key, new List<int>(new int[] { lr.Key, }));
                    }
                }
            }
            if (duplicates.Count > 0)
            {
                duplicates.Sort();
                MessageBox.Show("Cannot merge - duplicate String GUIDs exist:\n\n  "
                    + String.Join("\n  ", duplicates.Select(g => "0x" + g.ToString("X16")))
                , "Merge", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            #endregion

            #region Prepare to add and remove instances to/from package name map _KEY files
            /*
             * Every time we remove a resource from the package, we should check to see if is still in use.
             * If not, we should remove it from the _KEY resource, if one exists.
             * The following fancy foot work gets us set up ready to rock'n'roll...
            /**/

            _KEYRemoveInstance removeInstance;
            _KEYAddInstance addInstance;
            _KEYCommitNameMaps commitNameMaps;

            // Do we actually have anything to deal with?
            // Set up a list of the useful NMAP/_KEY rks/resources
            var keys = _currentPackage.FindAll(x => x.ResourceType == 0x0166038C)// NMAP (or _KEY if you prefer)
                .Select(rk =>
                {
                    // So, this resource key in the index, can we get a _KEY from it?
                    try { return new KeyValuePair<IResourceIndexEntry, NameMapResource.NameMapResource>(rk, new NameMapResource.NameMapResource(0, ((APackage)_currentPackage).GetResource(rk))); }
                    catch { return new KeyValuePair<IResourceIndexEntry, NameMapResource.NameMapResource>(null, null); }
                }).Where(x => x.Key != null).ToList();

            if (keys.Count > 0)
            {
                // OK, we have one or more _KEY files, so we're going to have to keep things tidy

                removeInstance = i =>
                {
                    // First, let's see if the instance exists (remember, this is after we've marked the STBL deleted)
                    var lrie = _currentPackage.FindAll(x => x.Instance == i);
                    if (lrie.Count > 0) return; // Yes, still in use elsewhere

                    // OK, not found... now need to go through all the _KEYs deleting the entry.
                    keys.ForEach(kvp => kvp.Value.Remove(i)); // and we don't care in the least if it wasn't there
                };

                // We'll just use the first file for adding (if there is one) - and handily, it knows how.
                // Note: this *will* throw an exception of the key exists
                addInstance = keys[0].Value.Add;

                // Just assume they need cleaning...
                commitNameMaps = () => keys.ForEach(kvp => _currentPackage.ReplaceResource(kvp.Key, kvp.Value));
            }
            else
            {
                // There is no _KEY at all, so we do nothing
                removeInstance = i => { };
                addInstance = (i, value) => { };
                commitNameMaps = () => { };
            }
            #endregion

            var instance = FNV64.GetHash(fileName + DateTime.Now.ToBinary().ToString());
            instance = (instance >> 8) ^ (instance & 0xFF);

            uint group = 0;
            foreach (var rk in StringTables.Keys) if (rk.ResourceGroup > group) group = rk.ResourceGroup;

            for (int lang = 0; lang < locales.Count; lang++)
            {
                var stbl = new StblResource.StblResource(0, null);
                foreach (var lr in StringTables.Values)
                {
                    if (lr.ContainsKey(lang))
                        foreach (var s in lr[lang])
                            stbl.Add(s.Key, s.Value);
                }

                var rk = new RK(STBL, group, instance | ((ulong)lang << 56));
                _currentPackage.AddResource(rk, stbl.Stream, true);
                addInstance(rk.Instance, "Strings_" + locales[lang].Replace(' ', '_') + "_" + rk.Instance.ToString("x"));
            }

            foreach (var res in currentPackage.FindAll(x => x.ResourceType == STBL && (x.ResourceGroup != group || (x.Instance & 0x00FFFFFFFFFFFFFF) != instance)))
            {
                _currentPackage.DeleteResource(res);
                removeInstance(res.Instance);// res.Instance is futile, uh, possibly unused that is.
            }

            commitNameMaps();

            pkgIsDirty = true;

            ReloadStringTables();
            ReloadStrings();
            SelectStrings();
        }

        private void btnFindFirst_Click(object sender, EventArgs e)
        {
            findText(txtFind.Text);
        }

        private void btnFindNext_Click(object sender, EventArgs e)
        {
            if (foundItems == null || foundItems.Count == 0)
                return;
            bool q = false;
            foreach (var i in foundItems)
            {
                if (q == true)
                {
                    i.Selected = true;
                    lstStrings.EnsureVisible(i.Index);
                    return;
                }
                if (i.Selected == true)
                {
                    q = true;
                    i.Selected = false;
                }
            }
            if (foundItems.Count > 0)
                foundItems[0].Selected = true;
        }

        private void btnDelString_Click(object sender, EventArgs e)
        {
            var stblgroup = (KeyValuePair<IResourceKey, Dictionary<int, StblResource.StblResource>>)lstSTBLs.SelectedItems[0].Tag;
            var instance = (ulong)txtInstance.Tag;

            for (var i = 0; i < locales.Count; i++)
                if (StringTables[stblgroup.Key].ContainsKey(i) && StringTables[stblgroup.Key][i].ContainsKey(instance))
                    StringTables[stblgroup.Key][i].Remove(instance);

            pkgIsDirty = true;
            ReloadStrings();
            SelectStrings();
        }

        private void btnAddString_Click(object sender, EventArgs e)
        {
            AddInstance ag = new AddInstance();
            if (ag.ShowDialog() != DialogResult.OK) return;

            var stblgroup = (KeyValuePair<IResourceKey, Dictionary<int, StblResource.StblResource>>)lstSTBLs.SelectedItems[0].Tag;
            ulong guid = ag.Instance;

            #region Check for duplicate
            for (var i = 0; i < locales.Count; i++)
                if (StringTables[stblgroup.Key].ContainsKey(i))
                    {
                        for (var j = 0; j < lstStrings.Items.Count; j++)
                        {
                            if (((ulong)lstStrings.Items[j].Tag) == guid)
                            {
                                MessageBox.Show(String.Format("GUID 0x{0:X16} already present.", guid), "Cannot add GUID", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                lstStrings.SelectedIndices.Clear();
                                lstStrings.SelectedIndices.Add(j);
                                return;
                            }
                        }
                    }
            #endregion

            for (var i = 0; i < locales.Count; i++)
                if (StringTables[stblgroup.Key].ContainsKey(i))
                    StringTables[stblgroup.Key][i].Add(new KeyValuePair<ulong, string>(guid, ""));

            pkgIsDirty = true;
            ReloadStrings();
            SelectStrings();
        }

        private void ckbAutoCommit_CheckedChanged(object sender, EventArgs e)
        {
            if (ckbAutoCommit.Checked) btnCommit.Enabled = false;

            Settings.Default.AutoCommit = ckbAutoCommit.Checked;
            Settings.Default.Save();
        }
        #endregion

        void AcceptLicence()
        {
            if (!Settings.Default.LicenceAccepted)
            {
                if (ShowLicence(true))
                {
                    Environment.Exit(1);
                    return;
                }
                Settings.Default.LicenceAccepted = true;
                Settings.Default.Save();
            }
        }

        bool ShowLicence(bool accept = false)
        {
            return (MessageBox.Show(@"
Copyright 2009 Jonathan Haas
Copyright (C) 2010-2013 by Peter L Jones

s3translate is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published
by the Free Software Foundation, either version 3 of the License,
or (at your option) any later version.

s3translate is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU General Public License for more details.

You should have received a copy of the GNU General Public
License along with s3translate.
If not, see <http://www.gnu.org/licenses/>.

s3translate uses the s3pi libraries by Peter L Jones (pljones@users.sf.net)
For details visit http://sourceforge.net/projects/s3pi/" + (accept ? @"

Do you accept this licence?" : ""),
                        "Licence",
                        accept ? MessageBoxButtons.YesNo : MessageBoxButtons.OK,
                        accept ? MessageBoxIcon.Question : MessageBoxIcon.Information,
                        accept ? MessageBoxDefaultButton.Button2 : MessageBoxDefaultButton.Button1) == DialogResult.Yes || !accept);
        }

        public class Version
        {
            static String timestamp;
            public static String CurrentVersion { get { return timestamp; } }

            static String libraryTimestamp;
            public static String LibraryVersion { get { return libraryTimestamp; } }

            static Version()
            {
                timestamp = VersionFor(System.Reflection.Assembly.GetEntryAssembly());
                libraryTimestamp = VersionFor(typeof(s3pi.Interfaces.AApiVersionedFields).Assembly);
            }

            public static String VersionFor(System.Reflection.Assembly a)
            {
#if DEBUG
                string[] v = a.GetName().Version.ToString().Split('.');
                return String.Format("{0}-{1}{2}-{3}", v[0].Substring(0, 2), v[0].Substring(2), v[1], v[2]);
#else
                string version_txt = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), Path.GetFileNameWithoutExtension(Application.ExecutablePath) + "-Version.txt");
                if (!File.Exists(version_txt))
                    return "Unknown";
                using (System.IO.StreamReader sr = new StreamReader(version_txt))
                {
                    String line1 = sr.ReadLine();
                    sr.Close();
                    return line1.Trim();
                }
#endif
            }
        }

        void AskCommit()
        {
            if (txtIsDirty)
            {
                var r = MessageBox.Show("Commit changes to target?", "Text has changed",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (r == DialogResult.Yes)
                    CommitText();
                else
                {
                    txtIsDirty = false;
                    SelectStrings();
                }
            }
        }

        void SetText()
        {
            Text = String.Format("Sims 3 Translate{0}{1}{2}"
                , _fname != "" ? " - " + _fname : ""
                , _pkgDirty ? " (unsaved)" : ""
                , _txtDirty ? " (uncommitted)" : ""
                );
        }

        bool AskSavePackage()
        {
            AskCommit();
            if (pkgIsDirty)
            {
                var r = MessageBox.Show("Save changes before closing file?", "Close Package", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                if (r == DialogResult.Yes)
                {
                    currentPackage.SavePackage();
                    pkgIsDirty = false;
                }
                else if (r == DialogResult.Cancel)
                    return false;
            }
            return true;
        }

        private void OpenPackage(string path)
        {
            if (fileName == path) return;

            ClosePackage();
            currentPackage = Package.OpenPackage(0, path, true);

            if (StringTables.Count == 0)
            {
                MessageBox.Show("There are no STBLs in the chosen package. It is not translateable using this tool.");
                ClosePackage();
                fileName = "";
                return;
            }

            fileName = path;
        }

        private void ClosePackage()
        {
            if (currentPackage != null)
                Package.ClosePackage(0, _currentPackage);

            currentPackage = null;
        }

        private void ReloadStringTables()
        {
            lstStrings.SelectedIndices.Clear();
            lstStrings.Items.Clear();
            lstSTBLs.SelectedIndices.Clear();
            lstSTBLs.Items.Clear();
            StringTables.Clear();

            if (currentPackage == null) return;

            loadAllLanguagesFromPackage(currentPackage);

            btnMergeSTBLs.Enabled = (lstSTBLs.Items.Count > 1);

            if (lstSTBLs.Items.Count > 0)
                lstSTBLs.SelectedIndices.Add(0);
        }

        private void loadAllLanguagesFromPackage(IPackage package)
        {
            for (byte i = 0; i < locales.Count; i++)
                loadLanguageFromPackage(i, package);
        }

        private void loadLanguageFromPackage(byte language, IPackage package)
        {
            foreach (var res in package.FindAll(x => x.ResourceType == STBL && x.Instance >> 56 == language))
            {
                IResourceKey rk = RKTostblGroupKey(res);
                List<IResourceKey> l = new List<IResourceKey>(StringTables.Keys).FindAll(x => { bool match = x.Equals(rk); return match; });
                if (l.Count == 0)
                {
                    KeyValuePair<IResourceKey, Dictionary<int, StblResource.StblResource>> kvp
                        = new KeyValuePair<IResourceKey, Dictionary<int, StblResource.StblResource>>(rk, new Dictionary<int, StblResource.StblResource>());
                    StringTables.Add(kvp.Key, kvp.Value);
                    lstSTBLs.Items.Add(CreateLVISTBL(kvp));
                }
                StringTables[rk].Add(language, new StblResource.StblResource(0, ((APackage)package).GetResource(res)));
            }
        }

        ListViewItem CreateLVISTBL(KeyValuePair<IResourceKey, Dictionary<int, StblResource.StblResource>> stbl)
        {
            return new ListViewItem(new string[] { FormatHex(stbl.Key.Instance, true) }) { Tag = stbl };
        }

        private void ReloadStrings()
        {
            try
            {
                lstStrings.BeginUpdate();
                lstStrings.SelectedIndices.Clear();
                lstStrings.Items.Clear();
                if (lstSTBLs.SelectedItems.Count != 1)
                    return;

                var stblgroup = (KeyValuePair<IResourceKey, Dictionary<int, StblResource.StblResource>>)lstSTBLs.SelectedItems[0].Tag;
                prg.Visible = true;
                if (!StringTables[stblgroup.Key].ContainsKey(cmbSourceLang.SelectedIndex))
                {
                    int rlocale = StringTables[stblgroup.Key].Keys.GetEnumerator().Current;
                    if (MessageBox.Show("The current source language doesn't exist in this string table. Use \"" + locales[rlocale] + "\" instead ?", "Language missing", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    {
                        cmbSourceLang.SelectedIndex = rlocale;
                        lstStrings.Items.Clear();
                    }
                    else
                        return;
                }
                if (!StringTables[stblgroup.Key].ContainsKey(cmbTargetLang.SelectedIndex))
                {
                    if (MessageBox.Show("The current target language doesn't exist in this string table. Create it?", "Language missing", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    {
                        MemoryStream ms = new MemoryStream(StringTables[stblgroup.Key][cmbSourceLang.SelectedIndex].AsBytes, true);
                        _currentPackage.AddResource(stblGroupKeyToRK(stblgroup.Key, cmbTargetLang.SelectedIndex), ms, true);
                        StringTables[stblgroup.Key].Add(cmbTargetLang.SelectedIndex, new StblResource.StblResource(0, ms));
                    }
                    else
                        return;
                }
                var sourceSTBL = StringTables[stblgroup.Key][cmbSourceLang.SelectedIndex];
                var targetSTBL = StringTables[stblgroup.Key][cmbTargetLang.SelectedIndex];
                prg.Value = 0;
                prg.Maximum = sourceSTBL.Keys.Count + targetSTBL.Keys.Count;
                foreach (var item in sourceSTBL.Keys)
                {
                    try
                    {
                        lstStrings.Items.Add(CreateLVIStrings(sourceSTBL, targetSTBL, item));
                    }
                    catch (Exception)
                    { }
                    prg.Value++;
                    prg.Invalidate();
                    statusStrip1.Update();
                }
                foreach (var item in targetSTBL.Keys)
                {
                    if (!sourceSTBL.ContainsKey(item))
                        lstStrings.Items.Add(CreateLVIStrings(sourceSTBL, targetSTBL, item));
                    prg.Value++;
                    prg.Invalidate();
                    statusStrip1.Update();
                }
            }
            finally
            {
                lstStrings.EndUpdate();
                prg.Visible = false;
                if (lstStrings.Items.Count > 0) lstStrings.SelectedIndices.Add(0);
            }
        }

        ListViewItem CreateLVIStrings(StblResource.StblResource src, StblResource.StblResource tgt, ulong item)
        {
            return new ListViewItem(new string[] {
                src.ContainsKey(item) ? src[item] : "",
                tgt.ContainsKey(item) ? tgt[item] : "",
                FormatHex(item)
            }) { Tag = item, UseItemStyleForSubItems = false };
        }

        private string FormatHex(ulong a)
        {
            return "0x" + a.ToString("X").PadLeft(16, '0');
        }

        private string FormatHex(ulong a, bool skipByte)
        {
            if (skipByte)
                return "0x__" + a.ToString("X").PadLeft(16, '0').Substring(2);
            else
                return FormatHex(a);
        }

        private string FormatHex(uint a)
        {
            return "0x" + a.ToString("X").PadLeft(8, '0');
        }

        #region class RK
        class RK : IResourceKey
        {
            public RK(uint rt, uint rg, ulong i) { ResourceType = rt; ResourceGroup = rg; Instance = i; }
            public uint ResourceType { get; set; }
            public uint ResourceGroup { get; set; }
            public ulong Instance { get; set; }
            public bool Equals(IResourceKey x, IResourceKey y) { return x.Equals(y); }
            public int GetHashCode(IResourceKey obj) { return obj.ResourceType.GetHashCode() ^ obj.ResourceGroup.GetHashCode() ^ obj.Instance.GetHashCode(); }
            public bool Equals(IResourceKey other) { return this.CompareTo(other) == 0; }
            public int CompareTo(IResourceKey other)
            {
                int res = this.ResourceType.CompareTo(other.ResourceType);
                if (res == 0) res = this.ResourceGroup.CompareTo(other.ResourceGroup);
                if (res == 0) res = this.Instance.CompareTo(other.Instance);
                return res;
            }
        }
        #endregion

        bool inChangeHandler = false;
        private void SelectStrings()
        {
            try
            {
                inChangeHandler = true;
                if (lstStrings.SelectedItems.Count != 1)
                {
                    txtSource.Text = txtTarget.Text = txtInstance.Text = "";
                    btnDelString.Enabled = txtTarget.Enabled = false;
                    txtInstance.Tag = null;
                    return;
                }
                else
                {
                    txtSource.Text = lstStrings.SelectedItems[0].SubItems[0].Text;
                    txtTarget.Text = lstStrings.SelectedItems[0].SubItems[1].Text;
                    btnDelString.Enabled = txtTarget.Enabled = true;
                    txtInstance.Text = lstStrings.SelectedItems[0].SubItems[2].Text;
                    txtInstance.Tag = lstStrings.SelectedItems[0].Tag;
                }
            }
            finally { inChangeHandler = false; }
        }

        private void CommitText()
        {
            if (inChangeHandler) return;

            var stblgroup = (KeyValuePair<IResourceKey, Dictionary<int, StblResource.StblResource>>)lstSTBLs.SelectedItems[0].Tag;
            var targetSTBL = StringTables[stblgroup.Key][cmbTargetLang.SelectedIndex];
            var text = txtTarget.Text;
            var instance = (ulong)txtInstance.Tag;

            if (targetSTBL.ContainsKey(instance))
            {
                if (targetSTBL[instance] == text)
                    return;
                targetSTBL[instance] = text;
            }
            else
            {
                targetSTBL.Add(instance, text);
            }

            if (cmbSourceLang.SelectedIndex == cmbTargetLang.SelectedIndex)
            {
                lstStrings.SelectedItems[0].SubItems[0].Text = text;
            }
            lstStrings.SelectedItems[0].SubItems[1].Text = text;

            IResourceKey rk = stblGroupKeyToRK(stblgroup.Key, cmbTargetLang.SelectedIndex);
            IResourceIndexEntry rie = _currentPackage.Find(x => x.Equals(rk));
            _currentPackage.ReplaceResource(rie, targetSTBL);
            pkgIsDirty = true;
            txtIsDirty = false;
        }

        private void findText(string text)
        {
            text = text.ToLower();
            foundItems = new List<ListViewItem>();
            foreach (ListViewItem i in lstStrings.Items)
            {
                if (i.SubItems[0].Text.ToLower().Contains(text))
                {
                    if (!foundItems.Contains(i))
                        foundItems.Add(i);
                }
                if (i.SubItems[1].Text.ToLower().Contains(text))
                {
                    if (!foundItems.Contains(i))
                        foundItems.Add(i);
                }
                if (i.SubItems[2].Text.ToLower().Contains(text))
                {
                    if (!foundItems.Contains(i))
                        foundItems.Add(i);
                }
                if (i.SubItems[2].Text.Contains(FormatHex(FNV64.GetHash(text))))
                {
                    if (!foundItems.Contains(i))
                        foundItems.Add(i);
                }
            }
            lstStrings.SelectedItems.Clear();
            if (foundItems.Count == 0)
                return;

            foundItems[0].Selected = true;
            lstStrings.EnsureVisible(foundItems[0].Index);
        }
    }
}
