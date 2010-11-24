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

        private const string packageFilter = "Sims 3 Package File (*.package)|*.package|All Files|*.*";
        private const string stblFilter = "Sims 3 String Table (*.stbl)|*.stbl|All Files|*.*";

        private List<ListViewItem> foundItems = null;

        IPackage _currentPackage;

        Dictionary<ulong, Dictionary<ulong, StblResource.StblResource>> StringTables = new Dictionary<ulong, Dictionary<ulong, StblResource.StblResource>>();

        const uint STBL = 0x220557DA;

        private string _fname;
        private bool _dirty;

        private string fileName
        {
            get
            {
                return _fname;
            }
            set
            {
                _fname = value;
                Text = "Sims 3 Translate - " + _fname;
                isDirty = false;
            }
        }

        private bool isDirty
        {
            get
            {
                return _dirty;
            }
            set
            {
                _dirty = value;
                Text = "Sims 3 Translate - " + _fname;
                if (_dirty)
                    Text += " (unsaved)";
            }
        }

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
            cmbSourceLang.SelectedIndex = Settings.Default.SourceLocale;
            cmbTargetLang.SelectedIndex = Settings.Default.UserLocale;
            lstSTBLs.ItemSelectionChanged += new ListViewItemSelectionChangedEventHandler((sender, e) => { ReloadSTBLs(); SelectStrings(); });
            lstStrings.ItemSelectionChanged += new ListViewItemSelectionChangedEventHandler((sender, e) => { SelectStrings(); });
            cmbSourceLang.SelectedIndexChanged += new EventHandler((sender, e) => { RefreshLanguages(); ReloadSTBLs(); });
            cmbTargetLang.SelectedIndexChanged += new EventHandler((sender, e) => { RefreshLanguages(); ReloadSTBLs(); });
            btnCommit.Click += new EventHandler((sender, e) => { CommitText(); });
            RefreshLanguages();

        }

        private void RefreshLanguages()
        {
            lstStrings.Columns[1].Text = cmbTargetLang.Text;
            lstStrings.Columns[2].Text = cmbSourceLang.Text;
        }

        private void splitContainer2_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void saveAllLanguagesToolStripMenuItem_Click(object sender, EventArgs e)
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

                            using (var fs = new FileStream(Path.Combine(fd.SelectedPath, "S3_" + STBL.ToString("X").PadLeft(8, '0') + "_00000000_" + STBLinstance.ToString("X").PadLeft(16, '0') + "_" + locales[(int)item] + ".stbl"), FileMode.CreateNew))
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

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = packageFilter;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //new();
                LoadFile(ofd.FileName);
            }
        }

        private void LoadFile(string path)
        {
            fileName = path;
            ClosePackage();
            _currentPackage = Package.OpenPackage(0, path, true);
            RelistSTBLs();
            if (StringTables.Count == 0)
            {
                MessageBox.Show("There are no STBLs in the current package. It is not translateable using this tool.");
                ClosePackage();
                return;
            }
        }

        private void RelistSTBLs()
        {
            StringTables.Clear();
            foreach (var res in _currentPackage.GetResourceList)
            {
                if (res.ResourceType == STBL && !res.IsDeleted)
                {
                    ulong instance = res.Instance & 0xFFFFFFFFFFFFFF;
                    if (!StringTables.ContainsKey(instance))
                        StringTables.Add(instance, new Dictionary<ulong, StblResource.StblResource>());
                    var stream = ((APackage)_currentPackage).GetResource(res);
                    StringTables[instance].Add(res.Instance >> 56, new StblResource.StblResource(0, stream));
                }
            }
            lstSTBLs.Items.Clear();
            foreach (var stbl in StringTables)
            {
                ListViewItem i = new ListViewItem(FormatHex(stbl.Key, true));
                i.Tag = stbl;
                lstSTBLs.Items.Add(i);
            }
            if (lstSTBLs.Items.Count == 1)
                lstSTBLs.SelectedIndices.Add(0);
        }

        private void ClosePackage()
        {
            if (isDirty)
            {
                var r = MessageBox.Show("Save changes before closing file?", "Close Package", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                if (r == DialogResult.Yes)
                    _currentPackage.SavePackage();
                if (r == DialogResult.Cancel)
                    return;
            }
            if (_currentPackage != null)
                Package.ClosePackage(0, _currentPackage);
            StringTables.Clear();
            lstSTBLs.Items.Clear();
            lstStrings.Items.Clear();
            _currentPackage = null;
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

        private void ReloadSTBLs()
        {
            try
            {
                lstStrings.Items.Clear();
                SelectStrings();
                if (lstSTBLs.SelectedItems.Count != 1)
                    return;
                var stblgroup = (KeyValuePair<ulong, Dictionary<ulong, StblResource.StblResource>>)lstSTBLs.SelectedItems[0].Tag;
                lstStrings.BeginUpdate();
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
                        _currentPackage.AddResource(STBL, 0, stblgroup.Key | ((ulong)cmbTargetLang.SelectedIndex << 56), ms, true);
                        StringTables[stblgroup.Key].Add((ulong)cmbTargetLang.SelectedIndex, new StblResource.StblResource(0, ms));
                    }
                    else
                        return;
                }
                var targetSTBL = StringTables[stblgroup.Key][(uint)cmbTargetLang.SelectedIndex];
                var sourceSTBL = StringTables[stblgroup.Key][(uint)cmbSourceLang.SelectedIndex];
                prg.Maximum = sourceSTBL.Keys.Count;
                foreach (var item in sourceSTBL.Keys)
                {
                    try
                    {
                        lstStrings.Items.Add(new ListViewItem(FormatHex(item)) { Tag = item });
                    }
                    catch (Exception)
                    { }
                    prg.Value = lstStrings.Items.Count;
                    prg.Invalidate();
                    statusStrip1.Update();
                }
                foreach (var item in targetSTBL.Keys)
                {
                    if (!sourceSTBL.ContainsKey(item))
                        lstStrings.Items.Add(new ListViewItem(FormatHex(item)) { Tag = item });
                }
                prg.Maximum = lstStrings.Items.Count;
                prg.Value = 0;
                foreach (ListViewItem item in lstStrings.Items)
                {
                    item.SubItems.Add("");
                    item.SubItems.Add("");
                    item.UseItemStyleForSubItems = false;
                    if (targetSTBL.ContainsKey((ulong)item.Tag))
                    {
                        item.SubItems[1].Text = targetSTBL[(ulong)item.Tag];
                        //item.SubItems[1].Font = new Font(item.SubItems[1].Font, FontStyle.Italic);
                    }
                    if (sourceSTBL.ContainsKey((ulong)item.Tag))
                    {
                        item.SubItems[2].Text = sourceSTBL[(ulong)item.Tag];
                    }
                    prg.Value++;
                    prg.Invalidate();
                    statusStrip1.Update();
                }
            }
            finally
            {
                lstStrings.EndUpdate();
                prg.Visible = false;
            }
        }

        private void SelectStrings()
        {
            if (lstStrings.SelectedItems.Count == 0)
            {
                txtSource.Text = "";
                txtTarget.Text = "";
                return;
            }
            txtInstance.Text = lstStrings.SelectedItems[0].SubItems[0].Text;
            txtInstance.Tag = lstStrings.SelectedItems[0].Tag;
            txtTarget.Text = lstStrings.SelectedItems[0].SubItems[1].Text;
            txtSource.Text = lstStrings.SelectedItems[0].SubItems[2].Text;
        }

        private void CommitText()
        {
            if (lstStrings.SelectedItems.Count == 1)
            {
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
                lstStrings.SelectedItems[0].SubItems[1].Text = text;
                if (cmbTargetLang.SelectedIndex == cmbSourceLang.SelectedIndex)
                {
                    lstStrings.SelectedItems[0].SubItems[2].Text = text;
                }
                ulong STBLinstance = stblgroup.Key | ((ulong)cmbTargetLang.SelectedIndex << 56);
                IResourceIndexEntry rie = _currentPackage.Find(new string[] { "ResourceType", "Instance" },
                        new TypedValue[] {
                            new TypedValue(typeof(uint), STBL),
                            new TypedValue(typeof(ulong), STBLinstance),
                        });
                _currentPackage.ReplaceResource(rie, targetSTBL);
                isDirty = true;
            }

        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClosePackage();
            if(_currentPackage == null)
                Application.Exit();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClosePackage();
        }

        private void savePackageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _currentPackage.SavePackage();
            isDirty = false;
        }

        private void savePackageAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = packageFilter;
            sfd.FileName = fileName;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                _currentPackage.SaveAs(sfd.FileName);
                fileName = sfd.FileName;
            }
        }

        private void txtTarget_TextChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                CommitText();
            }
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            new SettingsDialog().ShowDialog();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = packageFilter;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //new();
                LoadFile(ofd.FileName);
            }
        }

        private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            bool isopen = (_currentPackage != null);
            closeToolStripMenuItem.Enabled = saveAllLanguagesToolStripMenuItem.Enabled = savePackageAsToolStripMenuItem.Enabled = savePackageToolStripMenuItem.Enabled = toolStripMenuItem4.Enabled = isopen;
            savePackageToolStripMenuItem.Enabled = savePackageToolStripMenuItem.Enabled && isDirty;
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Not implemented yet...");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (sender == button1 && (MessageBox.Show("This will revert all " + locales[cmbTargetLang.SelectedIndex] + " texts to the " + locales[cmbSourceLang.SelectedIndex] + " defaults. Continue?","Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No))
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
            IResourceIndexEntry rie = _currentPackage.Find(new string[] { "ResourceType", "Instance" },
                    new TypedValue[] {
                            new TypedValue(typeof(uint), STBL),
                            new TypedValue(typeof(ulong), STBLinstance),
                        });
            if(rie != null)
                _currentPackage.ReplaceResource(rie, targetSTBL);
            else
                _currentPackage.AddResource(STBL, 0 ,STBLinstance, targetSTBL.Stream,false);
            StringTables[stblgroup.Key][(uint)cmbTargetLang.SelectedIndex] = targetSTBL;
            isDirty = true;
            ReloadSTBLs();
            SelectStrings();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will revert all(!) texts in all(!) languages to the " + locales[cmbSourceLang.SelectedIndex] + " defaults. Continue?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                return;
            var a = cmbTargetLang.SelectedIndex;
            for (var i = 0; i < locales.Count; i++)
            {
                cmbTargetLang.SelectedIndex = i;
                button1_Click(null, null);
            }
            cmbTargetLang.SelectedIndex = a;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var r = fileName + DateTime.Now.ToString();
            var instance = FNV64.GetHash(r) & 0xFFFFFFFFFFFFFF;
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
                _currentPackage.AddResource(STBL, 0, instance | (lang << 56), stbl.Stream, false);
            }
            foreach (var res in _currentPackage.GetResourceList)
            {
                if (res.ResourceType == STBL && (res.Instance & 0xFFFFFFFFFFFFFF) != instance)
                    _currentPackage.DeleteResource(res);
            }
            RelistSTBLs();
            ReloadSTBLs();
            SelectStrings();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

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
                if (i.SubItems[0].Text.Contains(FormatHex(FNV64.GetHash(text))))
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

        private void button4_Click(object sender, EventArgs e)
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

        private void button5_Click(object sender, EventArgs e)
        {
            findText(textBox1.Text);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            ClosePackage();
            if (_currentPackage != null)
                e.Cancel = true;
        }
    }
}
