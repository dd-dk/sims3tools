/*
 *  Copyright 2009 Jonathan Haas
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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using s3pi.Interfaces;
using s3pi.Package;
using s3pi.WrapperDealer;
using System.IO;
using S3Translate.Properties;
using System.Security.Cryptography;
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
        string fileName { get { return _fname; } set { if (_fname != value) { _fname = value; OnFileNameChanged();  } } }

        IPackage _currentPackage;
        EventHandler CurrentPackageChanged;
        void OnCurrentPackageChanged() { if (CurrentPackageChanged != null) CurrentPackageChanged(this, EventArgs.Empty); }
        IPackage currentPackage { get { return _currentPackage; } set { if (_currentPackage != value) { _currentPackage = value; OnCurrentPackageChanged(); } } }

        int _selectedIndex = -1;
        EventHandler SelectedIndexChanged;
        void OnSelectedIndexChanged() { if (SelectedIndexChanged != null) SelectedIndexChanged(this, EventArgs.Empty); }
        int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (_selectedIndex == value) return;
                AskCommit();
                _selectedIndex = value;
                OnSelectedIndexChanged();
            }
        }

        /*private bool _langDirty;
        EventHandler LangIsDirtyChanged;
        void OnLangIsDirtyChanged() { if (LangIsDirtyChanged != null) LangIsDirtyChanged(this, EventArgs.Empty); }
        private bool langIsDirty { get { return _langDirty; } set { if (_langDirty != value) { _langDirty = value; OnLangIsDirtyChanged(); } } }/**/

        private bool _pkgDirty;
        private bool pkgIsDirty { get { return _pkgDirty; } set { _pkgDirty = value; SetText(); } }

        private bool _txtDirty;
        private bool txtIsDirty { get { return _txtDirty; } set { _txtDirty = value; SetText(); btnCommit.Enabled = _txtDirty; } }


        private const string packageFilter = "Sims 3 Package File (*.package)|*.package|All Files|*.*";
        private const string stblFilter = "Sims 3 String Table (*.stbl)|*.stbl|All Files|*.*";

        const uint STBL = 0x220557DA;
        private List<ListViewItem> foundItems = null;
        Dictionary<ulong, Dictionary<ulong, StblResource.StblResource>> StringTables = new Dictionary<ulong, Dictionary<ulong, StblResource.StblResource>>();


        public Form1()
        {
            InitializeComponent();
            cmbSourceLang.DataSource = locales.AsReadOnly();
            cmbTargetLang.DataSource = locales.AsReadOnly();
            try
            {
                cmbSourceLang.SelectedIndex = Settings.Default.SourceLocale;
                cmbTargetLang.SelectedIndex = Settings.Default.UserLocale;
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

            lstSTBLs.ItemSelectionChanged += new ListViewItemSelectionChangedEventHandler((sender, e) => ReloadStrings());
            cmbSourceLang.SelectedIndexChanged += new EventHandler((sender, e) => { lstStrings.Columns[0].Text = cmbSourceLang.Text; ReloadStrings(); });
            cmbTargetLang.SelectedIndexChanged += new EventHandler((sender, e) => { lstStrings.Columns[1].Text = cmbTargetLang.Text; ReloadStrings(); });

            lstStrings.ItemSelectionChanged += new ListViewItemSelectionChangedEventHandler((sender, e) =>
                SelectedIndex = lstStrings.SelectedIndices.Count == 1 ? lstStrings.SelectedIndices[0] : -1);
            SelectedIndexChanged += new EventHandler((sender, e) => { SelectStrings(); });

            btnCommit.Click += new EventHandler((sender, e) => { CommitText(); });

            cmbSourceLang.SelectedIndex = Settings.Default.SourceLocale;
            cmbTargetLang.SelectedIndex = Settings.Default.UserLocale;
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
            closeToolStripMenuItem.Enabled = exportLanguageToolStripMenuItem.Enabled = savePackageAsToolStripMenuItem.Enabled = savePackageToolStripMenuItem.Enabled = importLanguagetoolStripMenuItem.Enabled = isopen;
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

        private void importLanguageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Not implemented yet...", "Import Language");
        }

        private void exportLanguageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstSTBLs.SelectedItems.Count == 1)
            {
                var ef = new Export();
                if (ef.ShowDialog() == DialogResult.OK)
                {
                    var fd = new FolderBrowserDialog();
                    if (fd.ShowDialog() == DialogResult.OK)
                    {
                        var stblgroup = (KeyValuePair<ulong, Dictionary<ulong, StblResource.StblResource>>)lstSTBLs.SelectedItems[0].Tag;
                        foreach (int item in ef.checkList.CheckedIndices)
                        {
                            ulong STBLinstance = stblgroup.Key | (((ulong)item) << 56);
                            var targetSTBL = StringTables[stblgroup.Key][(uint)item];

                            using (var fs = new FileStream(Path.Combine(fd.SelectedPath,
                                String.Format("S3_{0:X8}_00000000_{1:X16}_{2}.stbl", STBL, STBLinstance, locales[item])
                                ), FileMode.CreateNew))
                            {
                                var sw = new BinaryWriter(fs);
                                sw.Write(targetSTBL.AsBytes);
                                sw.Close();
                            }
                        }
                    }
                }
            }
            else
                MessageBox.Show("Please select a STBL instance");
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

        #region Designer-bound event handlers
        private void txtTarget_TextChanged(object sender, EventArgs e)
        {
            if (inChangeHandler) return;

            if (ckbAutoCommit.Checked)
                CommitText();
            else
                txtIsDirty = true;
        }

        private void btnRevertLang_Click(object sender, EventArgs e)
        {
            if (sender != null)
                if (MessageBox.Show("This will revert all " + locales[cmbTargetLang.SelectedIndex]
                + " texts to the " + locales[cmbSourceLang.SelectedIndex] + " defaults.\n\nContinue?",
                "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                    return;

            var stblgroup = (KeyValuePair<ulong, Dictionary<ulong, StblResource.StblResource>>)lstSTBLs.SelectedItems[0].Tag;
            if (!StringTables[stblgroup.Key].ContainsKey((uint)cmbSourceLang.SelectedIndex))
            {
                int rlocale = (int)StringTables[stblgroup.Key].Keys.GetEnumerator().Current;
                if (MessageBox.Show("The current source language doesn't exist in this string table. Use \"" + locales[rlocale] + "\" instead ?", "Language missing", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    cmbSourceLang.SelectedIndex = rlocale;
                    lstStrings.Items.Clear();
                }
                else
                    return;
            }

            MemoryStream ms = new MemoryStream(StringTables[stblgroup.Key][(uint)cmbSourceLang.SelectedIndex].AsBytes, true);
            StblResource.StblResource targetSTBL = new StblResource.StblResource(0, ms);

            ulong STBLinstance = stblgroup.Key | ((ulong)cmbTargetLang.SelectedIndex << 56);
            IResourceIndexEntry rie = _currentPackage.Find(x => x.ResourceType == STBL && x.Instance == STBLinstance);
            if (rie != null)
                _currentPackage.ReplaceResource(rie, targetSTBL);
            else
                _currentPackage.AddResource(new RK(STBL, 0, STBLinstance), targetSTBL.Stream, false);
            StringTables[stblgroup.Key][(uint)cmbTargetLang.SelectedIndex] = targetSTBL;
            pkgIsDirty = true;
            ReloadStrings();
            SelectStrings();
        }

        private void btnRevertAll_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will revert all(!) texts in all(!) languages to the " + locales[cmbSourceLang.SelectedIndex] + " defaults. Continue?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                return;
            var a = cmbTargetLang.SelectedIndex;
            for (var i = 0; i < locales.Count; i++)
            {
                cmbTargetLang.SelectedIndex = i;
                btnRevertLang_Click(null, null);
            }
            cmbTargetLang.SelectedIndex = a;
        }

        private void btnMergeLangSTLBs_Click(object sender, EventArgs e)
        {
            var r = fileName + DateTime.Now.ToString();
            var instance = FNV64.GetHash(r) & 0x00FFFFFFFFFFFFFF;
            for (ulong lang = 0; lang < 23; lang++)
            {
                var stbl = new StblResource.StblResource(0,null);
                foreach (var tables in StringTables.Values)
                {
                    if (tables.ContainsKey(lang))
                    {
                        foreach (var s in tables[lang])
                        {
                            stbl.Add(s.Key, s.Value);
                        }
                    }
                }
                _currentPackage.AddResource(new RK(STBL, 0, instance | (lang << 56)), stbl.Stream, false);
            }
            foreach (var res in _currentPackage.GetResourceList)
            {
                if (res.ResourceType == STBL && (res.Instance & 0xFFFFFFFFFFFFFF) != instance)
                    _currentPackage.DeleteResource(res);
            }
            ReloadStringTables();
            ReloadStrings();
            SelectStrings();
        }

        private void txtFind_TextChanged(object sender, EventArgs e)
        {

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

        private void btnFindFirst_Click(object sender, EventArgs e)
        {
            findText(txtFind.Text);
        }

        private void ckbAutoCommit_CheckedChanged(object sender, EventArgs e)
        {
            if (ckbAutoCommit.Checked) btnCommit.Enabled = false;
        }
        #endregion

        void AskCommit()
        {
            if (txtIsDirty)
            {
                var r = MessageBox.Show("Commit changes to target?", "Text has changed",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (r == DialogResult.Yes)
                    CommitText();
                else
                    SelectStrings();
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
            SelectedIndex = -1;
            lstStrings.Items.Clear();
            lstSTBLs.SelectedIndices.Clear();
            lstSTBLs.Items.Clear();
            StringTables.Clear();
            if (currentPackage == null) return;

            foreach (var res in currentPackage.GetResourceList)
            {
                if (res.ResourceType == STBL && !res.IsDeleted)
                {
                    ulong instance = res.Instance & 0xFFFFFFFFFFFFFF;
                    if (!StringTables.ContainsKey(instance))
                        StringTables.Add(instance, new Dictionary<ulong, StblResource.StblResource>());
                    var stream = ((APackage)currentPackage).GetResource(res);
                    StringTables[instance].Add(res.Instance >> 56, new StblResource.StblResource(0, stream));
                }
            }
            foreach (var stbl in StringTables)
                lstSTBLs.Items.Add(CreateLVISTBL(stbl));
            if (lstSTBLs.Items.Count > 0)
                lstSTBLs.SelectedIndices.Add(0);

            btnMergeLangSTBLs.Enabled = (lstSTBLs.Items.Count > 1);
        }

        ListViewItem CreateLVISTBL(KeyValuePair<ulong, Dictionary<ulong, StblResource.StblResource>> stbl)
        {
            return new ListViewItem(new string[] { FormatHex(stbl.Key, true) }) { Tag = stbl };
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

                var stblgroup = (KeyValuePair<ulong, Dictionary<ulong, StblResource.StblResource>>)lstSTBLs.SelectedItems[0].Tag;
                prg.Visible = true;
                if (!StringTables[stblgroup.Key].ContainsKey((uint)cmbSourceLang.SelectedIndex))
                {
                    int rlocale = (int)StringTables[stblgroup.Key].Keys.GetEnumerator().Current;
                    if (MessageBox.Show("The current source language doesn't exist in this string table. Use \"" + locales[rlocale] + "\" instead ?", "Language missing", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    {
                        cmbSourceLang.SelectedIndex = rlocale;
                        lstStrings.Items.Clear();
                    }
                    else
                        return;
                }
                if (!StringTables[stblgroup.Key].ContainsKey((uint)cmbTargetLang.SelectedIndex))
                {
                    if (MessageBox.Show("The current target language doesn't exist in this string table. Create it?", "Language missing", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    {
                        MemoryStream ms = new MemoryStream(StringTables[stblgroup.Key][(uint)cmbSourceLang.SelectedIndex].AsBytes, true);
                        _currentPackage.AddResource(new RK(STBL, 0, stblgroup.Key | ((ulong)cmbTargetLang.SelectedIndex << 56)), ms, true);
                        StringTables[stblgroup.Key].Add((ulong)cmbTargetLang.SelectedIndex, new StblResource.StblResource(0, ms));
                    }
                    else
                        return;
                }
                var sourceSTBL = StringTables[stblgroup.Key][(uint)cmbSourceLang.SelectedIndex];
                var targetSTBL = StringTables[stblgroup.Key][(uint)cmbTargetLang.SelectedIndex];
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

        class RK : IResourceKey
        {
            public RK(uint rt, uint rg, ulong i) { ResourceType = rt; ResourceGroup = rg; Instance = i; }
            public uint ResourceType { get; set; }
            public uint ResourceGroup { get; set; }
            public ulong Instance { get; set; }
            public bool Equals(IResourceKey x, IResourceKey y) { throw new NotImplementedException(); }
            public int GetHashCode(IResourceKey obj) { throw new NotImplementedException(); }
            public bool Equals(IResourceKey other) { throw new NotImplementedException(); }
            public int CompareTo(IResourceKey other) { throw new NotImplementedException(); }
        }

        bool inChangeHandler = false;
        private void SelectStrings()
        {
            try
            {
                inChangeHandler = true;
                if (SelectedIndex == -1)
                {
                    txtSource.Text = txtTarget.Text = txtInstance.Text = "";
                    txtTarget.Enabled = false;
                    txtInstance.Tag = null;
                    return;
                }
                else
                {
                    txtSource.Text = lstStrings.SelectedItems[0].SubItems[0].Text;
                    txtTarget.Text = lstStrings.SelectedItems[0].SubItems[1].Text;
                    txtTarget.Enabled = true;
                    txtInstance.Text = lstStrings.SelectedItems[0].SubItems[2].Text;
                    txtInstance.Tag = lstStrings.SelectedItems[0].Tag;
                }
                txtIsDirty = false;
            }
            finally { inChangeHandler = false; }
        }

        private void CommitText()
        {
            if (inChangeHandler) return;

            var stblgroup = (KeyValuePair<ulong, Dictionary<ulong, StblResource.StblResource>>)lstSTBLs.SelectedItems[0].Tag;
            var targetSTBL = StringTables[stblgroup.Key][(uint)cmbTargetLang.SelectedIndex];
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
                lstStrings.Items[SelectedIndex].SubItems[0].Text = text;
            }
            lstStrings.Items[SelectedIndex].SubItems[1].Text = text;

            ulong STBLinstance = stblgroup.Key | ((ulong)cmbTargetLang.SelectedIndex << 56);
            IResourceIndexEntry rie = _currentPackage.Find(x => x.ResourceType == STBL && x.Instance == STBLinstance);
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
                    if(!foundItems.Contains(i))
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
