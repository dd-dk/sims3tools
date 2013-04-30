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


        private const string myName = "Sims 3 Translate";
        private const string packageFilter = "Sims 3 Packages|*.package|All Files|*.*";
        private const string stblFilter = "Exported String Tables|S3_220557DA_*.stbl|Any stbl (*.stbl)|*.stbl|All Files|*.*";
        private const uint STBL = 0x220557DA;

        string _fname;
        EventHandler FileNameChanged;
        void OnFileNameChanged() { if (FileNameChanged != null) FileNameChanged(this, EventArgs.Empty); }
        string fileName { get { return _fname; } set { if (_fname != value) { _fname = value; OnFileNameChanged(); } } }

        IPackage _currentPackage;
        EventHandler CurrentPackageChanged;
        void OnCurrentPackageChanged() { if (CurrentPackageChanged != null) CurrentPackageChanged(this, EventArgs.Empty); }
        IPackage currentPackage { get { return _currentPackage; } set { if (_currentPackage != value) { _currentPackage = value; OnCurrentPackageChanged(); } } }

        private bool _pkgDirty;
        private bool pkgIsDirty { get { return _pkgDirty; } set { _pkgDirty = value; SetFormTitle(); } }

        private bool _txtDirty;
        private bool txtIsDirty { get { return _txtDirty; } set { _txtDirty = value; SetFormTitle(); btnCommit.Enabled = _txtDirty; } }

        private int _sourceLang;
        private int sourceLang { get { return _sourceLang; } set { _sourceLang = cmbSourceLang.SelectedIndex = value; } }
        private int _targetLang;
        private int targetLang { get { return _targetLang; } set { AskCommit(); _targetLang = cmbTargetLang.SelectedIndex = value; } }

        KeyValuePair<IResourceKey, Dictionary<int, StblResource.StblResource>> STBLGroupKey
        {
            get
            {
                if (comboBox_SetPicker.SelectedIndex < 0)
                    return default(KeyValuePair<IResourceKey, Dictionary<int, StblResource.StblResource>>);

                ulong stblGroupKeyInstance = Convert.ToUInt64(comboBox_SetPicker.SelectedItem.ToString().Substring(4), 16);
                var stblgroup = StringTables.Where(x => x.Key.Instance == stblGroupKeyInstance).First();
                return stblgroup;
            }
        }


        private List<ListViewItem> foundItems = null;

        // Dictionary<STBLGroupKey, Dictionary<lang, stbl>>
        Dictionary<IResourceKey, Dictionary<int, StblResource.StblResource>> StringTables = new Dictionary<IResourceKey, Dictionary<int, StblResource.StblResource>>(new STBLGroupKeyEqualityComparer());
        class STBLGroupKeyEqualityComparer : IEqualityComparer<IResourceKey>
        {
            public bool Equals(IResourceKey x, IResourceKey y) { return x.Instance == y.Instance; }
            public int GetHashCode(IResourceKey obj) { return obj.Instance.GetHashCode(); }
        }

        public Form1()
        {
            AcceptLicence();
            InitializeComponent();

            cmbSourceLang.DataSource = locales.AsReadOnly();
            cmbTargetLang.DataSource = locales.AsReadOnly();
            try
            {
                sourceLang = Settings.Default.SourceLocale;
                targetLang = Settings.Default.UserLocale;
                ckbAutoCommit.Checked = Settings.Default.AutoCommit;
            }
            catch (Exception)
            {
                new SettingsDialog().ShowDialog();
            }

            ClosePackage();
            
            FileNameChanged += new EventHandler((sender, e) => SetFormTitle());

            CurrentPackageChanged += new EventHandler((sender, e) =>
            {
                _pkgDirty = _txtDirty = false;
                SetFormTitle();
                ReloadStringTables();
                btnSetToAll.Enabled = btnSetToTarget.Enabled = tlpFind.Enabled = currentPackage != null;
            });

            comboBox_SetPicker.SelectedIndexChanged += new EventHandler((sender, e) => { AskCommit(); btnAddString.Enabled = comboBox_SetPicker.SelectedIndex >= 0; ReloadStrings(); });

            sourceLang = Settings.Default.SourceLocale;
            lstStrings.Columns[0].Text = "Source: " + cmbSourceLang.Text;
            cmbSourceLang.SelectedIndexChanged += new EventHandler((sender, e) => { sourceLang = cmbSourceLang.SelectedIndex; lstStrings.Columns[0].Text = "Source: " + cmbSourceLang.Text; ReloadStrings(); });

            targetLang = Settings.Default.UserLocale;
            lstStrings.Columns[1].Text = "Target: " + cmbTargetLang.Text;
            cmbTargetLang.SelectedIndexChanged += new EventHandler((sender, e) => { targetLang = cmbTargetLang.SelectedIndex; lstStrings.Columns[1].Text = "Target: " + cmbTargetLang.Text; ReloadStrings(); });

            lstStrings.ItemSelectionChanged += new ListViewItemSelectionChangedEventHandler((sender, e) => { AskCommit(); SelectStrings(); });

            btnCommit.Click += new EventHandler((sender, e) => { CommitText(); });
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

            foreach (int lang in ef.checkList.CheckedIndices)
            {
                using (var fs = new FileStream(Path.Combine(fd.SelectedPath,
                    String.Format("S3_{0:X8}_{1:X8}_{2:X2}{3:X14}_{4}.stbl", STBLGroupKey.Key.ResourceType, STBLGroupKey.Key.ResourceGroup, lang, STBLGroupKey.Key.Instance, locales[lang])
                    ), FileMode.CreateNew))
                {
                    new BinaryWriter(fs).Write(StringTables[STBLGroupKey.Key][lang].AsBytes);
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

        private void btnSetToTarget_Click(object sender, EventArgs e)
        {
            if (sourceLang == targetLang)
                AskCommit();
            txtIsDirty = false;

            if (MessageBox.Show(
                "This will copy all " + locales[sourceLang] + " strings to " + locales[targetLang] + ".\n\nContinue?",
                "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2
                ) == DialogResult.No)
                return;

            int source = _getSource(STBLGroupKey.Key, sourceLang);
            if (source == -1)
                return;

            _setToTarget(STBLGroupKey.Key, source, targetLang);

            pkgIsDirty = true;

            ReloadStrings();
            SelectStrings();
        }

        private void btnSetToAll_Click(object sender, EventArgs e)
        {
            if (sourceLang == targetLang)
                AskCommit();
            txtIsDirty = false;

            if (MessageBox.Show(
                "This will copy all " + locales[sourceLang] + " strings to all other languages.\n\nContinue?",
                "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2
                ) == DialogResult.No)
                return;

            int source = _getSource(STBLGroupKey.Key, sourceLang);
            if (source == -1)
                return;

            for (var i = 0; i < locales.Count; i++)
                _setToTarget(STBLGroupKey.Key, source, i);

            pkgIsDirty = true;

            ReloadStrings();
            SelectStrings();
        }

        delegate void _KEYRemoveInstance(ulong instance);
        delegate void _KEYAddInstance(ulong instance, string name);
        delegate void _KEYCommitNameMaps();
        
        private void btnMergeSets_Click(object sender, EventArgs e)
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
             * Every time we remove a resource from the package, we should check to see if it is still in use.
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
            AskCommit();

            findText(txtFind.Text);
        }

        private void btnFindNext_Click(object sender, EventArgs e)
        {
            AskCommit();

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
            AskCommit();

            var instance = (ulong)txtInstance.Tag;

            for (var i = 0; i < locales.Count; i++)
                if (StringTables[STBLGroupKey.Key].ContainsKey(i) && StringTables[STBLGroupKey.Key][i].ContainsKey(instance))
                    StringTables[STBLGroupKey.Key][i].Remove(instance);

            pkgIsDirty = true;
            ReloadStrings();
            SelectStrings();
        }

        private void btnAddString_Click(object sender, EventArgs e)
        {
            AskCommit();

            AddInstance ag = new AddInstance();
            if (ag.ShowDialog() != DialogResult.OK) return;

            ulong guid = ag.Instance;

            #region Check for duplicate
            for (var i = 0; i < locales.Count; i++)
                if (StringTables[STBLGroupKey.Key].ContainsKey(i))
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
                if (StringTables[STBLGroupKey.Key].ContainsKey(i))
                    StringTables[STBLGroupKey.Key][i].Add(new KeyValuePair<ulong, string>(guid, ""));

            pkgIsDirty = true;
            ReloadStrings();
            SelectStrings();
        }

        private void ckbAutoCommit_CheckedChanged(object sender, EventArgs e)
        {
            if (ckbAutoCommit.Checked)
            {
                btnCommit.Enabled = false;
                if (txtIsDirty)
                    CommitText();
            }

            Settings.Default.AutoCommit = ckbAutoCommit.Checked;
            Settings.Default.Save();
        }

        private void btnStringToTarget_Click(object sender, EventArgs e)
        {
            AskCommit();
            txtIsDirty = false;

            int source = _getSource(STBLGroupKey.Key, sourceLang);
            if (source == -1)
                return;

            _stringToTarget(STBLGroupKey.Key, source, targetLang);

            pkgIsDirty = true;
            lstStrings.SelectedItems[0].SubItems[1].Text = lstStrings.SelectedItems[0].SubItems[0].Text;
            try
            {
                inChangeHandler = true;
                txtTarget.Text = lstStrings.SelectedItems[0].SubItems[0].Text;
            }
            finally { inChangeHandler = false; }
        }

        private void btnStringToAll_Click(object sender, EventArgs e)
        {
            AskCommit();
            txtIsDirty = false;

            if (MessageBox.Show(
                "This will copy the selected " + locales[sourceLang] + " string to all other languages.\n\nContinue?",
                "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2
                ) == DialogResult.No)
                return;

            int source = _getSource(STBLGroupKey.Key, sourceLang);
            if (source == -1)
                return;

            for (var i = 0; i < locales.Count; i++)
                _stringToTarget(STBLGroupKey.Key, source, i);

            pkgIsDirty = true;
            lstStrings.SelectedItems[0].SubItems[1].Text = lstStrings.SelectedItems[0].SubItems[0].Text;
            try
            {
                inChangeHandler = true;
                txtTarget.Text = lstStrings.SelectedItems[0].SubItems[0].Text;
            }
            finally { inChangeHandler = false; }
        }
        #endregion

        void AcceptLicence()
        {
            if (!Settings.Default.LicenceAccepted)
            {
                if (!ShowLicence(true))
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
            var btns = accept ? MessageBoxButtons.YesNo : MessageBoxButtons.OK;
            var icon = accept ? MessageBoxIcon.Question : MessageBoxIcon.Information;
            var defBtn = accept ? MessageBoxDefaultButton.Button2 : MessageBoxDefaultButton.Button1;

            var res = MessageBox.Show(@"
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
                    "Licence", btns, icon, defBtn);

            return accept && res == DialogResult.Yes;
        }

        #region class Version
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
        #endregion

        void SetFormTitle()
        {
            Text = String.Format("{0}{1}{2}{3}"
                , myName
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
                var r = MessageBox.Show("Save changes before closing file?", "Save Package", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
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
            ClosePackage();

            fileName = path;
            currentPackage = Package.OpenPackage(0, path, true);

            closeToolStripMenuItem.Enabled =
                savePackageToolStripMenuItem.Enabled =
                savePackageAsToolStripMenuItem.Enabled =
                true;

            importPackageToolStripMenuItem.Enabled =
                importSTBLToolStripMenuItem.Enabled =
                exportLanguageToolStripMenuItem.Enabled =
                true;

            importFromPackageToolStripMenuItem.Enabled =
                importFromSTBLFileToolStripMenuItem.Enabled =
                exportToPackageToolStripMenuItem.Enabled =
                exportToSTBLFileToolStripMenuItem.Enabled =
                true;

            if (StringTables.Count == 0)
            {
                MessageBox.Show("There are no STBLs in the chosen package. It is not translateable using this tool.");
                ClosePackage();
                fileName = "";
                return;
            }
        }

        private void ClosePackage()
        {
            if (currentPackage != null)
                Package.ClosePackage(0, _currentPackage);

            currentPackage = null;
            closeToolStripMenuItem.Enabled =
                savePackageToolStripMenuItem.Enabled =
                savePackageAsToolStripMenuItem.Enabled =
                false;

            importPackageToolStripMenuItem.Enabled =
                importSTBLToolStripMenuItem.Enabled =
                exportLanguageToolStripMenuItem.Enabled =
                false;

            importFromPackageToolStripMenuItem.Enabled =
                importFromSTBLFileToolStripMenuItem.Enabled =
                exportToPackageToolStripMenuItem.Enabled =
                exportToSTBLFileToolStripMenuItem.Enabled =
                false;
        }


        class TupleComparer<T, U> : IComparer<Tuple<T, U>>
            where T : IComparable<T>
            where U : IComparable<U>
        {
            public int Compare(Tuple<T, U> x, Tuple<T, U> y)
            {
                var res = x.Item1.CompareTo(y.Item1);
                return res == 0 ? x.Item2.CompareTo(y.Item2) : res;
            }
        }

        private void ReloadStringTables()
        {
            lstStrings.SelectedIndices.Clear();
            lstStrings.Items.Clear();
            comboBox_SetPicker.SelectedIndex = -1;
            comboBox_SetPicker.Items.Clear();
            StringTables.Clear();
            mergeAllSetsToolStripMenuItem.Enabled = false;

            if (currentPackage == null)
                return;


            var stbls = currentPackage
                .FindAll(x => x.ResourceType == STBL)
                .Select(x => new Tuple<IResourceKey, int>(RKTostblGroupKey(x), (int)(x.Instance >> 56)))
                .OrderBy(x => x, new TupleComparer<IResourceKey, int>())
            ;

            var stblGroupKeys = stbls
                .Select(x => x.Item1)
                .Distinct(new STBLGroupKeyEqualityComparer());

            foreach (var stblGroupKey in stblGroupKeys)
            {
                StringTables.Add(stblGroupKey, new Dictionary<int, StblResource.StblResource>());
                comboBox_SetPicker.Items.Add("0x__" + stblGroupKey.Instance.ToString("X14"));

                var languages = stbls
                    .Where(x => x.Item1.Equals(stblGroupKey))
                    .Select(x => x.Item2);

                foreach (var language in languages)
                {
                    var res = currentPackage
                        .Find(x => x.ResourceType == STBL && x.Instance == (ulong)(stblGroupKey.Instance | ((ulong)language << 56)));
                    StringTables[stblGroupKey].Add(language, new StblResource.StblResource(0, ((APackage)currentPackage).GetResource(res)));
                }
            }

            mergeAllSetsToolStripMenuItem.Enabled = (comboBox_SetPicker.Items.Count > 1);

            if (comboBox_SetPicker.Items.Count > 0)
                comboBox_SetPicker.SelectedIndex = 0;
        }

        private void ReloadStrings()
        {
            int i = lstStrings.SelectedIndices.Count > 0 ? lstStrings.SelectedIndices[0] : -1;
            try
            {
                lstStrings.BeginUpdate();
                lstStrings.SelectedIndices.Clear();
                lstStrings.Items.Clear();

                if (comboBox_SetPicker.SelectedIndex == -1)
                    return;

                if (!StringTables[STBLGroupKey.Key].ContainsKey(sourceLang))
                {
                    int rlocale = StringTables[STBLGroupKey.Key].Keys.GetEnumerator().Current;

                    // Give up!
                    if (rlocale == sourceLang)
                        return;

                    if (MessageBox.Show(
                        string.Format("The current source language, {0}, does not exist in this string table.\n\n" +
                        "Use {1} instead?", locales[sourceLang], locales[rlocale]),
                        "Language missing", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    {
                        sourceLang = rlocale;// we get called!
                        return;
                    }
                    else
                        return;
                }


                if (!StringTables[STBLGroupKey.Key].ContainsKey(targetLang))
                {
                    if (MessageBox.Show(
                        string.Format("The current target language, {0}, does not exist in this string table.\n\n" +
                        "Create it?", locales[targetLang]),
                        "Language missing", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    {
                        MemoryStream ms = new MemoryStream(StringTables[STBLGroupKey.Key][sourceLang].AsBytes, true);
                        _currentPackage.AddResource(stblGroupKeyToRK(STBLGroupKey.Key, targetLang), ms, true);
                        StringTables[STBLGroupKey.Key].Add(targetLang, new StblResource.StblResource(0, ms));
                    }
                    else
                        return;
                }

                var sourceSTBL = StringTables[STBLGroupKey.Key][sourceLang];
                var targetSTBL = StringTables[STBLGroupKey.Key][targetLang];

                prg.Visible = true;
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
                if (lstStrings.Items.Count > 0) lstStrings.SelectedIndices.Add(i >= 0 && i < lstStrings.Items.Count ? i : 0);
            }
        }

        ListViewItem CreateLVIStrings(StblResource.StblResource src, StblResource.StblResource tgt, ulong item)
        {
            return new ListViewItem(new string[] {
                src.ContainsKey(item) ? src[item] : "",
                tgt.ContainsKey(item) ? tgt[item] : "",
                "0x" + item.ToString("X16")
            }) { Tag = item, UseItemStyleForSubItems = false };
        }

        int _getSource(IResourceKey stblgroupKey, int source)
        {
            if (!StringTables[stblgroupKey].ContainsKey(source))
            {
                int rlocale = StringTables[stblgroupKey].Keys.GetEnumerator().Current;//wtf?
                if (MessageBox.Show(
                    string.Format("Source language {0} does not exist in this string table.\n\n" +
                    "Use {1} instead?", locales[source], locales[rlocale]),
                    "Language missing", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    return rlocale;
                }
                else
                    return -1;
            }
            return source;
        }

        void _setToTarget(IResourceKey stblgroupKey, int source, int target)
        {
            if (source == target)
                return;

            MemoryStream ms = new MemoryStream(StringTables[stblgroupKey][source].AsBytes, true);
            StblResource.StblResource targetSTBL = new StblResource.StblResource(0, ms);

            IResourceKey rk = stblGroupKeyToRK(stblgroupKey, target);
            IResourceIndexEntry rie = _currentPackage.Find(x => x.Equals(rk));
            if (rie != null)
                _currentPackage.ReplaceResource(rie, targetSTBL);
            else
            {
                if (MessageBox.Show(
                    locales[target] + " does not exist.\n\nCreate it?",
                    "Create " + locales[target], MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1
                    ) != DialogResult.Yes)
                    return;

                _currentPackage.AddResource(rk, targetSTBL.Stream, true);
            }
            StringTables[stblgroupKey][target] = targetSTBL;
        }

        void _stringToTarget(IResourceKey stblgroupKey, int source, int target)
        {
            if (source == target)
                return;

            IResourceKey rk = stblGroupKeyToRK(stblgroupKey, target);
            IResourceIndexEntry rie = _currentPackage.Find(x => x.Equals(rk));
            StblResource.StblResource targetSTBL;
            if (rie == null)
            {
                if (MessageBox.Show(
                    locales[target] + " does not exist.\n\nCreate it?",
                    "Create " + locales[target], MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1
                    ) != DialogResult.Yes)
                    return;

                targetSTBL = new StblResource.StblResource(0, null);
                StringTables[stblgroupKey][target] = targetSTBL;
                rie = _currentPackage.AddResource(rk, targetSTBL.Stream, true);
            }
            else
            {
                targetSTBL = StringTables[stblgroupKey][target];
            }

            var text = txtSource.Text;
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
            _currentPackage.ReplaceResource(rie, targetSTBL);
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
            public override string ToString() { return String.Format("0x{0:X8}-0x{1:X8}-0x{2:X16}", ResourceType, ResourceGroup, Instance); }
        }

        IResourceKey stblGroupKeyToRK(IResourceKey stblGroupKey, int lang)
        {
            return new RK(stblGroupKey.ResourceType, stblGroupKey.ResourceGroup, stblGroupKey.Instance | ((ulong)lang << 56));
        }

        IResourceKey RKTostblGroupKey(IResourceKey stblGroupKey)
        {
            return new RK(stblGroupKey.ResourceType, stblGroupKey.ResourceGroup, stblGroupKey.Instance & 0x00FFFFFFFFFFFFFF);
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
                    btnStringToTarget.Enabled = btnStringToAll.Enabled = btnDelString.Enabled = txtTarget.Enabled = false;
                    txtInstance.Tag = null;
                    return;
                }
                else
                {
                    txtSource.Text = lstStrings.SelectedItems[0].SubItems[0].Text;
                    txtTarget.Text = lstStrings.SelectedItems[0].SubItems[1].Text;
                    btnStringToTarget.Enabled = btnStringToAll.Enabled = btnDelString.Enabled = txtTarget.Enabled = true;
                    txtInstance.Text = lstStrings.SelectedItems[0].SubItems[2].Text;
                    txtInstance.Tag = lstStrings.SelectedItems[0].Tag;
                }
            }
            finally { inChangeHandler = false; }
        }

        private void AskCommit()
        {
            if (txtIsDirty)
            {
                var r = MessageBox.Show("Commit changes to " + locales[targetLang] + "?", "Text has changed",
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

        private void CommitText()
        {
            if (inChangeHandler) return;

            var targetSTBL = StringTables[STBLGroupKey.Key][targetLang];
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

            foreach (var item in lstStrings.Items.Cast<ListViewItem>().Where(x => ((ulong)x.Tag) == instance))
            {
                item.SubItems[1].Text = text;
                if (sourceLang == targetLang)
                    item.SubItems[0].Text = text;
            }

            if (sourceLang == targetLang)
                if (lstStrings.SelectedIndices.Count > 0)
                {
                    txtSource.Text = text;
                }

            IResourceKey rk = stblGroupKeyToRK(STBLGroupKey.Key, targetLang);
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
                if (i.SubItems[2].Text.Contains("0x" + FNV64.GetHash(text).ToString("X16")))
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
