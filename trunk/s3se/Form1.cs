/***************************************************************************
 *  Copyright (C) 2013 by Peter L Jones                                    *
 *  pljones@users.sf.net                                                   *
 *                                                                         *
 *  This file is part of s3se.                                             *
 *                                                                         *
 *  s3se is free software: you can redistribute it and/or modify           *
 *  it under the terms of the GNU General Public License as published by   *
 *  the Free Software Foundation, either version 3 of the License, or      *
 *  (at your option) any later version.                                    *
 *                                                                         *
 *  s3tse is distributed in the hope that it will be useful,               *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of         *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the          *
 *  GNU General Public License for more details.                           *
 *                                                                         *
 *  You should have received a copy of the GNU General Public License      *
 *  along with s3translate.  If not, see <http://www.gnu.org/licenses/>.   *
 *                                                                         *
 *  s3translate was created by Jonathan Haas.                              *
 *  s3se was derived from s3translate by Peter L Jones.                    *
 ***************************************************************************/

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
using s3se.Properties;

namespace s3se
{
    public partial class Form1 : Form
    {
        private const string myName = "s3se";

        private const string packageFilter = "Sims 3 Packages|*.package|All Files|*.*";
        private const string stblFilter = "Exported String Tables|S3_220557DA_*.stbl|Any stbl (*.stbl)|*.stbl|All Files|*.*";
        private const string nraasFilter = "Nraas Packer format|StringTableStrings_*.txt|Any text file (*.txt)|*.txt|All Files|*.*";

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
        public static List<String> nraas_locales = new List<String> {
            "ENG_US",
            "CHI_CN", //ZHO_CH?
            "CHO_TW", //ZHO_TW?
            "CZE_CZ", //CES_CZ?
            "DAN_DK",
            "DUT_NL", //NLD_NL?
            "FIN_FI",
            "FRE_FR", //FRA_FR?
            "GER_DE", //DEU_DE?
            "GRE_GR", //ELL_GR?
            "HUN_HU",
            "ITA_IT",
            "JPN_JA",
            "KOR_KR",
            "NOR_NO",
            "POL_PL",
            "POR_PT",
            "POR_BR",
            "RUS_RU",
            "SPA_ES",
            "SPA_MX",
            "SWE_SE",
            "THA_TH"
        };
        #endregion

        delegate void _KEYRemoveInstance(ulong instance);
        delegate void _KEYAddInstance(ulong instance, string name);
        delegate void _KEYCommitNameMaps();

        _KEYRemoveInstance removeInstance;
        _KEYAddInstance addInstance;
        _KEYCommitNameMaps commitNameMaps;

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
        private bool txtIsDirty { get { return _txtDirty; } set { _txtDirty = value; SetFormTitle(); button_AbandonEdit.Enabled = btnCommit.Enabled = _txtDirty; } }

        private int _sourceLang;
        private int sourceLang { get { return _sourceLang; } set { _sourceLang = cmbSourceLang.SelectedIndex = value; } }
        private int _targetLang;
        private int targetLang { get { return _targetLang; } set { AskCommit(); _targetLang = cmbTargetLang.SelectedIndex = value; } }

        KeyValuePair<IResourceKey, Dictionary<int, StblResource.StblResource>> _STBLGroupKey;
        EventHandler STBLGroupKeyChanged;
        void OnSTBLGroupKeyChanged() { if (STBLGroupKeyChanged != null) STBLGroupKeyChanged(this, EventArgs.Empty); }
        KeyValuePair<IResourceKey, Dictionary<int, StblResource.StblResource>> STBLGroupKey
        {
            get
            {
                return _STBLGroupKey;
            }
            set
            {
                if (_STBLGroupKey.Equals(value))
                    return;
                _STBLGroupKey = value;
                OnSTBLGroupKeyChanged();
            }
        }


        private List<ListViewItem> foundItems = null;

        // Dictionary<STBLGroupKey, Dictionary<lang, stbl>>
        Dictionary<IResourceKey, Dictionary<int, StblResource.StblResource>> StringTables = new Dictionary<IResourceKey, Dictionary<int, StblResource.StblResource>>(RK.Default);

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
            }
            catch (Exception)
            {
                new SettingsDialog().ShowDialog();
            }
            if (Settings.Default.LastOpenPackageFolder == "")
            {
                Settings.Default.LastOpenPackageFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                Settings.Default.Save();
            }
            if (Settings.Default.LastImportFileFolder == "")
            {
                Settings.Default.LastImportFileFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                Settings.Default.Save();
            }

            FileNameChanged += new EventHandler((sender, e) => SetFormTitle());

            CurrentPackageChanged += new EventHandler((sender, e) => packageChangeHandler());

            cmbSetPicker.SelectedIndexChanged += new EventHandler((sender, e) =>
            {
                lstStrings.SelectedIndices.Clear();

                if (cmbSetPicker.SelectedIndex < 0)
                    STBLGroupKey = default(KeyValuePair<IResourceKey, Dictionary<int, StblResource.StblResource>>);
                else
                {
                    ulong stblGroupKeyInstance = Convert.ToUInt64(cmbSetPicker.SelectedItem.ToString().Substring(4), 16);
                    STBLGroupKey = StringTables.Where(x => x.Key.Instance == stblGroupKeyInstance).Single();
                }
            });

            STBLGroupKeyChanged += new EventHandler((sender, e) =>
            {
                btnAddString.Enabled = cmbSetPicker.SelectedIndex >= 0;
                ReloadStrings();
            });

            sourceLang = Settings.Default.SourceLocale;
            lstStrings.Columns[1].Text = "Source: " + cmbSourceLang.Text;
            cmbSourceLang.SelectedIndexChanged += new EventHandler((sender, e) =>
            {
                sourceLang = cmbSourceLang.SelectedIndex;
                lstStrings.Columns[1].Text = "Source: " + cmbSourceLang.Text;
                ReloadStrings();
            });

            targetLang = Settings.Default.UserLocale;
            lstStrings.Columns[2].Text = "Target: " + cmbTargetLang.Text;
            cmbTargetLang.SelectedIndexChanged += new EventHandler((sender, e) =>
            {
                targetLang = cmbTargetLang.SelectedIndex;
                lstStrings.Columns[2].Text = "Target: " + cmbTargetLang.Text;
                ReloadStrings();
            });

            lstStrings.ItemSelectionChanged += new ListViewItemSelectionChangedEventHandler((sender, e) =>
            {
                AskCommit();
                SelectStrings();
            });

            txtTarget.TextChanged += new EventHandler((sender, e) => { if (inChangeHandler) return; txtIsDirty = true; });
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!AskSavePackage()) { e.Cancel = true; return; }

            ClosePackage();
        }

        #region File menu
        private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            closeToolStripMenuItem.Enabled =
                savePackageAsToolStripMenuItem.Enabled =
                _currentPackage != null;
            savePackageToolStripMenuItem.Enabled = _currentPackage != null && pkgIsDirty;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog() { Filter = packageFilter, InitialDirectory = Settings.Default.LastOpenPackageFolder, };
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            if (!AskSavePackage())
                return;

            OpenPackage(ofd.FileName);
            Settings.Default.LastOpenPackageFolder = Path.GetDirectoryName(ofd.FileName);
            Settings.Default.Save();
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
            var sfd = new SaveFileDialog() { Filter = packageFilter, InitialDirectory = Settings.Default.LastOpenPackageFolder, FileName = fileName, };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                AskCommit();

                _currentPackage.SaveAs(sfd.FileName);
                fileName = sfd.FileName;
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

        #region STBL menu
        private void sTBLsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            createNewSetToolStripMenuItem.Enabled =
                importFromPackageToolStripMenuItem.Enabled =
                importFromSTBLFileToolStripMenuItem.Enabled =
                importFromNraasPackerFormatToolStripMenuItem.Enabled =
                _currentPackage != null;

            deleteSetToolStripMenuItem.Enabled =
                exportToPackageToolStripMenuItem.Enabled =
                exportToSTBLFileToolStripMenuItem.Enabled =
                exportToNraasPackerFormatToolStripMenuItem.Enabled =
                cmbSetPicker.SelectedIndex >= 0;

            mergeAllSetsToolStripMenuItem.Enabled = cmbSetPicker.Items.Count > 1;
        }

        private void createNewSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AskCommit();

            var instance = FNV64.GetHash(fileName + DateTime.Now.ToBinary().ToString());
            instance = (instance >> 8) ^ (instance & 0xFF);

            uint group = 0;
            foreach (var rk in StringTables.Keys)
                if (rk.ResourceGroup > group)
                    group = rk.ResourceGroup;

            var stblGroupKeyRK = new RK(STBL, group, instance);

            for (int lang = 0; lang < locales.Count; lang++)
            {
                var stbl = new StblResource.StblResource(0, null);

                var rk = stblGroupKeyToRK(stblGroupKeyRK, lang);
                _currentPackage.AddResource(rk, stbl.Stream, true);
                addInstance(rk.Instance, NameMapName(rk));
            }

            commitNameMaps();

            pkgIsDirty = true;

            ReloadStringTables();
            int i = 0;
            foreach (var key in StringTables.Keys)
            {
                if (key.Instance == instance)
                    break;
                i++;
            }
            cmbSetPicker.SelectedIndex = i;

            ReloadStrings();
        }

        private void deleteSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                "This will delete all strings in all languages for the selected string set.\n\nContinue?",
                "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2
                ) == DialogResult.No)
                return;

            foreach (var res in currentPackage.FindAll(x =>
                x.ResourceType == STBL &&
                x.ResourceGroup == STBLGroupKey.Key.ResourceGroup &&
                (x.Instance & 0x00FFFFFFFFFFFFFF) == STBLGroupKey.Key.Instance))
            {
                _currentPackage.DeleteResource(res);
                removeInstance(res.Instance);
            }

            commitNameMaps();

            pkgIsDirty = true;

            ReloadStringTables();
        }

        private void importPackageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Get the file name
            var ofd = new OpenFileDialog() { Title = "Import From Package", Filter = packageFilter, InitialDirectory = Settings.Default.LastImportFileFolder, };
            if (ofd.ShowDialog() != DialogResult.OK) return;
            if (ofd.FileName == fileName)
            {
                MessageBox.Show("That is the current package.", myName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var newSTBLs = new List<Tuple<String, IResourceKey, IDictionary<ulong, string>>>();

            #region Parse the input

            // Find out what we have got to deal with
            try
            {
                IPackage importFrom = Package.OpenPackage(0, ofd.FileName);

                newSTBLs = importFrom
                    .FindAll(x => x.ResourceType == STBL)
                    .Select(x =>
                    {
                        return Tuple.Create(
                            NameMapName(x),
                            (IResourceKey)new RK(x.ResourceType, x.ResourceGroup, x.Instance),
                            (IDictionary<ulong, string>)new StblResource.StblResource(0, ((APackage)importFrom).GetResource(x))
                        );
                    })
                    .ToList();

                if (newSTBLs.Count == 0)
                {
                    MessageBox.Show("The selected package contains no STBLs.", "Import", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error whilst trying to import from " + ofd.FileName + ".\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            #endregion

            ImportNewSTBLList(newSTBLs);
            Settings.Default.LastImportFileFolder = Path.GetDirectoryName(ofd.FileName);
            Settings.Default.Save();
        }

        private void importSTBLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportFromFile(stblFilter, _getFilesSTBL, _parseSTBL);
        }

        private void importFromNraasPackerFormatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportFromFile(nraasFilter, _getFilesNRaas, _parseNRaas);
        }

        private void exportToPackageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IPackage targetPackage;

            #region Get the filename
            var sfd = new SaveFileDialog()
            {
                Title = "Export To Package",
                Filter = packageFilter,
                InitialDirectory = Settings.Default.LastImportFileFolder,
                FileName = Path.GetFileNameWithoutExtension(fileName) + cmbSetPicker.Text.Substring(3),
            };
            if (sfd.ShowDialog() != DialogResult.OK)
                return;
            #endregion

            #region Get the package
            try
            {
                if (File.Exists(sfd.FileName))
                {
                    targetPackage = Package.OpenPackage(0, sfd.FileName, true);

                    #region Check for duplicates
                    var lrie = currentPackage.FindAll(x => x.ResourceType == STBL);
                    if (targetPackage.FindAll(x =>
                    {
                        if (x.ResourceType != STBL) return false;
                        foreach (var rie in lrie) if (x.Equals(rie)) return true;
                        return false;
                    }).Count > 0)
                    {
                        MessageBox.Show("Duplicate resources detected.",
                            "Export", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    #endregion
                }
                else
                {
                    targetPackage = Package.NewPackage(0);
                    targetPackage.SaveAs(sfd.FileName);
                    targetPackage = Package.OpenPackage(0, sfd.FileName, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not open package for export.\nPackage: " + sfd.FileName + "\nError:\n" + ex.Message,
                    "Export", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            #endregion

            #region Handle the NameMap/_KEY bits
            _KEYAddInstance targetAddInstance;
            _KEYCommitNameMaps targetCommitNameMaps;

            // Do we actually have anything to deal with?
            // Set up a list of the useful NMAP/_KEY rks/resources
            var keys = targetPackage.FindAll(x => x.ResourceType == 0x0166038C)// NMAP (or _KEY if you prefer)
                .Select(rk =>
                {
                    // So, this resource key in the index, can we get a _KEY from it?
                    try
                    {
                        return new KeyValuePair<IResourceIndexEntry, NameMapResource.NameMapResource>(rk,
                            new NameMapResource.NameMapResource(0, ((APackage)targetPackage).GetResource(rk)));
                    }
                    catch { return new KeyValuePair<IResourceIndexEntry, NameMapResource.NameMapResource>(null, null); }
                }).Where(x => x.Key != null).ToList();

            if (keys.Count > 0)
            {
                // We'll just use the first file for adding (if there is one) - and handily, it knows how.
                // Note: this *will* throw an exception of the key exists
                targetAddInstance = keys[0].Value.Add;

                // Just assume they need cleaning...
                targetCommitNameMaps = () => keys.ForEach(kvp => targetPackage.ReplaceResource(kvp.Key, kvp.Value));
            }
            else
            {
                // There is no _KEY at all, so we do nothing
                targetAddInstance = (i, value) => { };
                targetCommitNameMaps = () => { };
            }
            #endregion

            AskCommit();

            foreach (var rk in currentPackage
                .FindAll(x => x.ResourceType == STBL && (x.Instance & 0x00FFFFFFFFFFFFFF) == STBLGroupKey.Key.Instance))
            {
                var stbl = new StblResource.StblResource(0, ((APackage)currentPackage).GetResource(rk));

                targetPackage.AddResource(rk, stbl.Stream, true);
                targetAddInstance(rk.Instance, NameMapName(rk));
            }

            targetCommitNameMaps();
            targetPackage.SavePackage();
            Settings.Default.LastImportFileFolder = Path.GetDirectoryName(sfd.FileName);
            Settings.Default.Save();
        }

        private void exportLanguageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ef = new Export() { Text = "Export (S3 stbl)" };
            if (ef.ShowDialog() != DialogResult.OK) return;

            var fd = new FolderBrowserDialog() { Description = "Export (S3 stbl)", SelectedPath = Settings.Default.LastImportFileFolder, };
            if (fd.ShowDialog() != DialogResult.OK) return;

            AskCommit();

            bool overwrite = false;
            foreach (int lang in ef.checkList.CheckedIndices)
            {
                var fn = Path.Combine(fd.SelectedPath, STBLName(stblGroupKeyToRK(STBLGroupKey.Key, lang)));
                if (!overwrite && File.Exists(fn))
                {
                    if (MessageBox.Show("Proceeding will overwrite existing files in the selected folder.\n\nContinue?",
                        "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != System.Windows.Forms.DialogResult.Yes)
                        return;
                    overwrite = true;
                }
                using (var fs = new FileStream(fn, FileMode.CreateNew))
                {
                    new BinaryWriter(fs).Write(StringTables[STBLGroupKey.Key][lang].AsBytes);
                    fs.Close();
                }
            }

            Settings.Default.LastImportFileFolder = fd.SelectedPath;
            Settings.Default.Save();
        }

        private void exportToNraasPackerFormatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ef = new Export() { Text = "Export (Nraas Packer)", };
            if (ef.ShowDialog() != DialogResult.OK) return;

            var fd = new FolderBrowserDialog() { Description = "Export (Nraas Packer)", SelectedPath = Settings.Default.LastImportFileFolder, };
            if (fd.ShowDialog() != DialogResult.OK) return;

            AskCommit();

            bool overwrite = false;
            foreach (int lang in ef.checkList.CheckedIndices)
            {
                //StringTableStrings_ENG_US_000f16b00ba8342f.txt
                var fn = Path.Combine(fd.SelectedPath, NRaasName(stblGroupKeyToRK(STBLGroupKey.Key, lang)));
                if (!overwrite && File.Exists(fn))
                {
                    if (MessageBox.Show("Proceeding will overwrite existing files in the selected folder.\n\nContinue?",
                        "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != System.Windows.Forms.DialogResult.Yes)
                        return;
                    overwrite = true;
                }
                using (var fs = new FileStream(fn, FileMode.Create))
                using (var w = new StreamWriter(fs, System.Text.Encoding.Unicode))
                {
                    foreach (var kvp in STBLGroupKey.Value[lang])
                    {
                        w.WriteLine(String.Format("<KEY>0x{0:X16}</KEY>", kvp.Key));
                        w.WriteLine(String.Format("<STR>{0}</STR>", kvp.Value));
                    }
                }
            }

            Settings.Default.LastImportFileFolder = fd.SelectedPath;
            Settings.Default.Save();
        }

        private void mergeAllSetsToolStripMenuItem_Click(object sender, EventArgs e)
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

            AskCommit();

            var instance = FNV64.GetHash(fileName + DateTime.Now.ToBinary().ToString());
            instance = (instance >> 8) ^ (instance & 0xFF);

            uint group = 0;
            foreach (var rk in StringTables.Keys) if (rk.ResourceGroup > group) group = rk.ResourceGroup;

            var stblGroupKeyRK = new RK(STBL, group, instance);

            for (int lang = 0; lang < locales.Count; lang++)
            {
                var stbl = new StblResource.StblResource(0, null);
                foreach (var lr in StringTables.Values.Where(x => x.ContainsKey(lang)))
                {
                    foreach (var s in lr[lang])
                        stbl.Add(s.Key, s.Value);
                }

                var rk = stblGroupKeyToRK(stblGroupKeyRK, lang);
                _currentPackage.AddResource(rk, stbl.Stream, true);
                addInstance(rk.Instance, NameMapName(rk));
            }

            foreach (var res in currentPackage.FindAll(x => x.ResourceType == STBL && (x.ResourceGroup != group || (x.Instance & 0x00FFFFFFFFFFFFFF) != instance)))
            {
                _currentPackage.DeleteResource(res);
                removeInstance(res.Instance);// res.Instance is futile, uh, possibly unused that is.
            }

            commitNameMaps();

            pkgIsDirty = true;

            ReloadStringTables();
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
                ), myName);
        }
        #endregion

        #region Form controls
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
            var guid = (ulong)tbGUID.Tag;

            foreach (var locale in StringTables[STBLGroupKey.Key].Keys)
            {
                var stbl = StringTables[STBLGroupKey.Key][locale];
                stbl.Remove(guid);
                commitLocaleInSet(locale, STBLGroupKey.Key, stbl);
            }

            pkgIsDirty = true;
            ReloadStrings();
        }

        private void btnChangeGUID_Click(object sender, EventArgs e)
        {
            EditAddGUID((ulong)tbGUID.Tag);
        }

        private void btnAddString_Click(object sender, EventArgs e)
        {
            EditAddGUID(0);
        }

        private void btnCommit_Click(object sender, EventArgs e)
        {
            CommitText();
            lstStrings.Focus();
        }

        private void button_AbandonEdit_Click(object sender, EventArgs e)
        {
            AbandonText();
            lstStrings.Focus();
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
            lstStrings.SelectedItems[0].SubItems[2].Text = lstStrings.SelectedItems[0].SubItems[1].Text;
            try
            {
                inChangeHandler = true;
                txtTarget.Text = lstStrings.SelectedItems[0].SubItems[1].Text;
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
            lstStrings.SelectedItems[0].SubItems[2].Text = lstStrings.SelectedItems[0].SubItems[1].Text;
            try
            {
                inChangeHandler = true;
                txtTarget.Text = lstStrings.SelectedItems[0].SubItems[1].Text;
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

" + myName + @" is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published
by the Free Software Foundation, either version 3 of the License,
or (at your option) any later version.

" + myName + @" is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU General Public License for more details.

You should have received a copy of the GNU General Public
License along with " + myName + @".
If not, see <http://www.gnu.org/licenses/>.

" + myName + @" uses the s3pi libraries by Peter L Jones (pljones@users.sf.net)
For details visit http://sourceforge.net/projects/s3pi/" + (accept ? @"

Do you accept this licence?" : ""),
                    "Licence", btns, icon, defBtn);

            return accept && res == DialogResult.Yes;
        }

        void packageChangeHandler()
        {
            _pkgDirty = _txtDirty = false;
            SetFormTitle();
            ReloadStringTables();
            tlpFind.Enabled = currentPackage != null;

            /*
                * Every time we remove a resource from the package, we should check to see if it is still in use.
                * If not, we should remove it from the _KEY resource, if one exists.
                * The following fancy foot work gets us set up ready to rock'n'roll...
            /**/

            // Do we actually have anything to deal with?
            // Set up a list of the useful NMAP/_KEY rks/resources
            var keys = (_currentPackage == null ? new List<IResourceIndexEntry>() : _currentPackage.FindAll(x => x.ResourceType == 0x0166038C))// NMAP (or _KEY if you prefer)
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
        }

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
            try
            {
                currentPackage = Package.OpenPackage(0, path, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not open package.\nPackage: " + path + "\nError:\n" + ex.Message,
                    "Open", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (StringTables.Count == 0)
            {
                MessageBox.Show("There are no STBLs in the selected package.", "Open", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ClosePackage()
        {
            if (currentPackage != null)
                Package.ClosePackage(0, _currentPackage);

            currentPackage = null;
        }

        private void ReloadStringTables()
        {
            // Remember where in the list we were... but do not hold references...
            ulong currentSTBLGroupIID = STBLGroupKey.Key == null ? 0 : STBLGroupKey.Key.Instance;

            lstStrings.SelectedIndices.Clear();
            lstStrings.Items.Clear();
            cmbSetPicker.SelectedIndex = -1;
            cmbSetPicker.Items.Clear();
            StringTables.Clear();

            if (currentPackage == null)
                return;


            var stbls = currentPackage
                .FindAll(x => x.ResourceType == STBL)
                .Select(x => new Tuple<IResourceKey, int>(RKTostblGroupKey(x), getLanguage(x.Instance)))
                .OrderBy(x => x.Item1.Instance)
            ;

            var stblGroupKeys = stbls
                .Select(x => x.Item1)
                .Distinct(RK.Default);

            foreach (var stblGroupKey in stblGroupKeys)
            {
                StringTables.Add(stblGroupKey, new Dictionary<int, StblResource.StblResource>());
                cmbSetPicker.Items.Add("0x__" + stblGroupKey.Instance.ToString("X14"));

                var languages = stbls
                    .Where(x => x.Item1.Equals(stblGroupKey))
                    .Select(x => x.Item2);

                foreach (var language in languages)
                {
                    var res = currentPackage
                        .Find(x => x.ResourceType == STBL && x.Instance == stblGroupKeyToRK(stblGroupKey, language).Instance);
                    StringTables[stblGroupKey].Add(language, new StblResource.StblResource(0, ((APackage)currentPackage).GetResource(res)));
                }
            }

            if (cmbSetPicker.Items.Count > 0)
            {
                var oldSTBLGroupIndex = currentSTBLGroupIID == 0 ? -1 : StringTables.Keys.Select(rk => rk.Instance).ToList().IndexOf(currentSTBLGroupIID);
                cmbSetPicker.SelectedIndex = oldSTBLGroupIndex == -1 ? 0 : oldSTBLGroupIndex;
            }
        }

        private void ReloadStrings()
        {
            int i = lstStrings.SelectedIndices.Count > 0 ? lstStrings.SelectedIndices[0] : -1;
            try
            {
                lstStrings.BeginUpdate();
                lstStrings.SelectedIndices.Clear();
                lstStrings.Items.Clear();
                label_SourceLanguageMissing.Visible =
                    label_TargetLanguageMissing.Visible =
                    btnSetToTarget.Enabled =
                    btnSetToAll.Enabled =
                    false;

                if (cmbSetPicker.SelectedIndex == -1)
                    return;

                if (!StringTables[STBLGroupKey.Key].ContainsKey(sourceLang))
                {
                    int rlocale = StringTables[STBLGroupKey.Key].Keys.DefaultIfEmpty(-1).First();

                    // Give up! (This should never happen)
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
                if (lstStrings.Items.Count > 0)
                {
                    lstStrings.SelectedIndices.Add(i >= 0 && i < lstStrings.Items.Count ? i : 0);
                }
                if (cmbSetPicker.SelectedIndex != -1)
                {
                    btnSetToTarget.Enabled =
                        btnSetToAll.Enabled =
                        StringTables[STBLGroupKey.Key].ContainsKey(sourceLang);
                    label_SourceLanguageMissing.Visible =
                        !StringTables[STBLGroupKey.Key].ContainsKey(sourceLang);
                    label_TargetLanguageMissing.Visible =
                        !StringTables[STBLGroupKey.Key].ContainsKey(targetLang);
                }
            }
        }

        ListViewItem CreateLVIStrings(StblResource.StblResource src, StblResource.StblResource tgt, ulong item)
        {
            return new ListViewItem(new string[] {
                "0x" + item.ToString("X16"),
                src.ContainsKey(item) ? src[item] : "",
                tgt.ContainsKey(item) ? tgt[item] : ""
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
            var guid = (ulong)tbGUID.Tag;

            if (targetSTBL.ContainsKey(guid))
            {
                if (targetSTBL[guid] == text)
                    return;
                targetSTBL[guid] = text;
            }
            else
            {
                targetSTBL.Add(guid, text);
            }
            _currentPackage.ReplaceResource(rie, targetSTBL);
        }

        void EditAddGUID(ulong orig)
        {
            AddEditGUID ag = new AddEditGUID(orig);
            if (ag.ShowDialog() != DialogResult.OK)
                return;

            var guid = ag.GUID;
            if (guid == 0 || guid == orig)
                return;

            // Check for duplicate before changing anything
            if (StringTables[STBLGroupKey.Key].Any(kvp => kvp.Value.ContainsKey(guid)))
            {
                MessageBox.Show(String.Format("GUID 0x{0:X16} already present.", guid), "Cannot add GUID", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Now we are happy to start updating
            if (orig == 0)
                lstStrings.SelectedIndices.Clear();

            foreach (var locale in StringTables[STBLGroupKey.Key].Keys)
            {
                var stbl = StringTables[STBLGroupKey.Key][locale];
                var value = "";
                if (orig != 0)
                {
                    value = StringTables[STBLGroupKey.Key][locale][orig];
                    stbl.Remove(orig);
                }
                stbl.Add(guid, value);
                commitLocaleInSet(locale, STBLGroupKey.Key, stbl);
            }

            pkgIsDirty = true;
            ReloadStrings();

            // If we added one, select it
            if (orig == 0)
            {
                lstStrings.SelectedIndices.Clear();
                var res = lstStrings.Items.Cast<ListViewItem>().Where(x => x.SubItems[0].Text == "0x" + guid.ToString("X16")).SingleOrDefault();
                if (res != null)
                {
                    res.Selected = true;
                    res.EnsureVisible();
                }
            }
        }

        bool inChangeHandler = false;
        private void SelectStrings()
        {
            try
            {
                inChangeHandler = true;
                if (lstStrings.SelectedItems.Count != 1)
                {
                    tbGUID.Text = "";
                    txtSource.Text = txtTarget.Text = "";
                    btnStringToTarget.Enabled = btnStringToAll.Enabled =
                        btnDelString.Enabled = btnChangeGUID.Enabled =
                        txtTarget.Enabled = false;
                    tbGUID.Tag = null;
                    return;
                }
                else
                {
                    tbGUID.Text = lstStrings.SelectedItems[0].SubItems[0].Text;
                    txtSource.Text = lstStrings.SelectedItems[0].SubItems[1].Text;
                    txtTarget.Text = lstStrings.SelectedItems[0].SubItems[2].Text;
                    btnStringToTarget.Enabled = btnStringToAll.Enabled =
                        btnDelString.Enabled = btnChangeGUID.Enabled =
                        txtTarget.Enabled = true;
                    tbGUID.Tag = lstStrings.SelectedItems[0].Tag;
                }
                btnSetToTarget.Enabled = btnSetToAll.Enabled = cmbSetPicker.SelectedIndex >= 0;
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
                    AbandonText();
            }
        }

        private void CommitText()
        {
            if (inChangeHandler) return;
            if (STBLGroupKey.Key == null)
                return;

            var targetSTBL = StringTables[STBLGroupKey.Key][targetLang];
            var text = txtTarget.Text;
            var guid = (ulong)tbGUID.Tag;

            if (targetSTBL.ContainsKey(guid))
            {
                if (targetSTBL[guid] == text)
                    return;
                targetSTBL[guid] = text;
            }
            else
            {
                targetSTBL.Add(guid, text);
            }

            foreach (var item in lstStrings.Items.Cast<ListViewItem>().Where(x => ((ulong)x.Tag) == guid))
            {
                item.SubItems[2].Text = text;
                if (sourceLang == targetLang)
                    item.SubItems[1].Text = text;
            }

            if (sourceLang == targetLang)
                if (lstStrings.SelectedIndices.Count > 0)
                {
                    txtSource.Text = text;
                }

            commitLocaleInSet(targetLang, STBLGroupKey.Key, targetSTBL);

            pkgIsDirty = true;
            txtIsDirty = false;
        }

        private void AbandonText()
        {
            txtIsDirty = false;
            SelectStrings();
        }

        private void commitLocaleInSet(int lang, IResourceKey stblGroupKey, IResource targetSTBL)
        {
            IResourceKey rk = stblGroupKeyToRK(stblGroupKey, lang);
            IResourceIndexEntry rie = _currentPackage.Find(x => x.Equals(rk));
            _currentPackage.ReplaceResource(rie, targetSTBL);
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
                if (i.SubItems[0].Text.Contains("0x" + FNV64.GetHash(text).ToString("X16")))
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
            }
            lstStrings.SelectedItems.Clear();
            if (foundItems.Count == 0)
                return;

            foundItems[0].Selected = true;
            lstStrings.EnsureVisible(foundItems[0].Index);
        }

        void ImportFromFile(string filter, _ImportGetFiles getFiles, _ImportParse parser)
        {
            // Get the file name(s)
            var ofd = new OpenFileDialog() { Title = "Import", Filter = filter, InitialDirectory = Settings.Default.LastImportFileFolder, };
            if (ofd.ShowDialog() != DialogResult.OK) return;

            List<Tuple<String, IResourceKey, IDictionary<ulong, string>>> newSTBLs;

            try
            {
                newSTBLs = getFiles(ofd.FileName)// Get RK from chosen filename and add other languages
                    .Select(x => parser(x.Item1, x.Item2))// Parse each input file
                    .ToList() // Force execution inside an exception handler scope
                    ;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error whilst trying to import:\n\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (newSTBLs.Count == 0)
            {
                MessageBox.Show("No strings were found.", "Import", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ImportNewSTBLList(newSTBLs);
            Settings.Default.LastImportFileFolder = Path.GetDirectoryName(ofd.FileName);
            Settings.Default.Save();
        }

        void ImportNewSTBLList(List<Tuple<String, IResourceKey, IDictionary<ulong, string>>> newSTBLList)
        {
            // Name the nameless Tuple Items any place in an anonymous class
            var newSTBLs = newSTBLList.Select(x => new { name = x.Item1, rk = x.Item2, stbl = x.Item3, });

            // Where do the new GUIDs come from
            var newGUIDStblGroupKeyRKs = newSTBLs
                .SelectMany(x => x.stbl
                    .Select(y => new { guid = y.Key, stblGroupKeyRK = RKTostblGroupKey(x.rk), })
                )
                .Select(x => new guidRK(x.guid, x.stblGroupKeyRK))
                .Distinct(guidRK.Default)
                .Select(x => new { guid = x.guid, stblGroupKeyRK = x.rk, })
                ;

            // Where do the existing GUIDs come from
            var oldGUIDStblGroupKeyRKs = StringTables
                .SelectMany(x => x.Value
                    .SelectMany(y => y.Value
                        .Select(z => new { guid = z.Key, stblGroupKeyRK = x.Key, })
                    )
                )
                .Select(x => new guidRK(x.guid, x.stblGroupKeyRK))
                .Distinct(guidRK.Default)
                .Select(x => new { guid = x.guid, stblGroupKeyRK = x.rk, })
            ;

            #region Check for duplicate GUIDs
            // Do any new GUIDs already exist?
            var dupGUIDStblGroupKeyRKs = newGUIDStblGroupKeyRKs
                .Where(x => oldGUIDStblGroupKeyRKs
                    .Any(y => y.guid == x.guid)
                )
                .SelectMany(x => oldGUIDStblGroupKeyRKs
                    .Where(y => y.guid == x.guid)
                    .Select(y => new { guid = x.guid, newGUIDStblGroupKeyRK = x.stblGroupKeyRK, oldGUIDStblGroupKeyRK = y.stblGroupKeyRK, })
                )
            ;

            // Do any new GUIDs exist with a different stblGroupKeyRK?
            var conflictGUIDStblGroupKeyRKs = dupGUIDStblGroupKeyRKs
                .Where(x => !x.oldGUIDStblGroupKeyRK.Equals(x.newGUIDStblGroupKeyRK))
            ;

            if (conflictGUIDStblGroupKeyRKs.FirstOrDefault() != null)
            {
                MessageBox.Show("One or more GUIDs being imported exists in more than one of the existing String Sets.  " +
                    "This conflict cannot be resolved automatically.\n\n" +
                    "Please manually ensure a GUID only exists in one String Set by deleting from one and editing the other.  The GUIDs/String Sets are:\n\n  " +
                    String.Join("\n  ", conflictGUIDStblGroupKeyRKs
                        .Select(x => x.guid)
                        .Distinct()
                        .Select(x => new
                        {
                            guid = x,
                            stringSets = conflictGUIDStblGroupKeyRKs
                                .Where(y => y.guid == x)
                                .Select(y => y.oldGUIDStblGroupKeyRK)
                        })
                        .Select(x => String.Format("GUID: {0:X16} in String Sets {1}.",
                            x.guid, String.Join(", ", x.stringSets.Select(y => "0x__" + y.Instance.ToString("X14")))))),
                    "Import", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            if (dupGUIDStblGroupKeyRKs.FirstOrDefault() != null)
            {
                if (MessageBox.Show("Duplicate String GUIDs exist.  If you continue, existing values will be overwritten.\n\nContinue?",
                    "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    return;
            }
            #endregion

            // OK, go ahead
            lstStrings.SelectedIndices.Clear();

            #region Make sure we have one string per GUID per language

            // All GUIDs we are touching across all StblGroups affected
            var mergeGUIDStblGroupKeyRKs = oldGUIDStblGroupKeyRKs
                .Where(x => newGUIDStblGroupKeyRKs.Any(y => y.stblGroupKeyRK.Equals(x.stblGroupKeyRK)))
                .Select(x => new guidRK(x.guid, x.stblGroupKeyRK))
                .Union(newGUIDStblGroupKeyRKs.Select(x => new guidRK(x.guid, x.stblGroupKeyRK)), guidRK.Default)
                .Select(x => new { guid = x.guid, stblGroupKeyRK = x.rk, })
            ;

            // Get the language list
            var langs = newSTBLs
                .Select(x => getLanguage(x.rk.Instance))
                .Distinct()
                .Union(StringTables
                    .Where(x => mergeGUIDStblGroupKeyRKs
                        .Any(y => y.stblGroupKeyRK.Equals(x.Key))
                    )
                    .SelectMany(x => x.Value.Keys)
                    .Distinct()
                )
            ;

            // Create the default entries
            var defaultStringTables = langs
                .SelectMany(x => mergeGUIDStblGroupKeyRKs
                    .Select(y => new
                        {
                            stblGroupKeyRK = y.stblGroupKeyRK,
                            lang = x,
                            guid = y.guid,
                            value = String.Format("0x{0:X16}: {1}", y.guid, locales[x]),
                        }
                    )
                )

            ;

            // Overwrite defaults with old values, if present
            var oldValues = defaultStringTables
                .Select(x =>
                {
                    if (!StringTables.ContainsKey(x.stblGroupKeyRK))
                        return x;

                    var stblGroup = StringTables[x.stblGroupKeyRK];
                    if (!stblGroup.ContainsKey(x.lang))
                        return x;

                    var stbl = stblGroup[x.lang];
                    if (!stbl.ContainsKey(x.guid))
                        return x;

                    return new { stblGroupKeyRK = x.stblGroupKeyRK, lang = x.lang, guid = x.guid, value = stbl[x.guid], };
                }
                )
            ;

            // Overwrite old or default values with new values, if present
            var newStringTables = oldValues
                .Select(x =>
                {
                    var stbl = newSTBLs
                        .Where(y => y.rk.Equals(stblGroupKeyToRK(x.stblGroupKeyRK, x.lang)))
                        .Select(y => y.stbl)
                        .SingleOrDefault();

                    var value = (stbl == null || !stbl.ContainsKey(x.guid))
                        ? x.value
                        : stbl[x.guid];

                    // This should now look like StringTables, right?
                    return new { rk = x.stblGroupKeyRK, dict = new { lang = x.lang, stbl = new { guid = x.guid, value = value, } } };
                }
                )
            ;
            #endregion

            #region Write the values to the current package
            foreach (var stblGroupKeyRK in newStringTables
                .Select(x => x.rk)
                .Distinct(RK.Default)
                .OrderBy(x => x)
                )
            {
                foreach (var lang in newStringTables
                    .Select(x => x.dict.lang)
                    .Distinct()
                    .OrderBy(x => x)
                    )
                {
                    var targetSTBL = new StblResource.StblResource(0, null);
                    var rk = stblGroupKeyToRK(stblGroupKeyRK, lang);
                    var name = newSTBLs
                        .Where(x => x.rk.Equals(rk))
                        .Select(x => x.name)
                        .DefaultIfEmpty(NameMapName(rk))
                        .Single();

                    var newSTBL = newStringTables
                        .Where(x => x.rk.Equals(stblGroupKeyRK))
                        .Select(x => x.dict);

                    var newStrings = newSTBL
                        .Where(x => x.lang == lang)
                        .Select(x => x.stbl)
                        ;

                    // Create this language
                    foreach (var kvp in newStrings)
                        targetSTBL.Add(kvp.guid, kvp.value);

                    if (!StringTables.ContainsKey(stblGroupKeyRK) || !StringTables[stblGroupKeyRK].ContainsKey(lang))
                    {
                        // We're creating a new resource
                        _currentPackage.AddResource(rk, targetSTBL.Stream, true);
                        addInstance(rk.Instance, name);
                    }
                    else
                    {
                        // We already have the resource, so replace it
                        IResourceIndexEntry rie = _currentPackage.Find(x => rk.Equals(x));
                        _currentPackage.ReplaceResource(rie, targetSTBL);
                    }
                }
            }
            #endregion

            pkgIsDirty = true;
            ReloadStringTables();
        }

        delegate IEnumerable<Tuple<String, IResourceKey>> _ImportGetFiles(String f);

        delegate Tuple<String, IResourceKey, IDictionary<ulong, string>> _ImportParse(String f, IResourceKey rk);

        IEnumerable<Tuple<String, IResourceKey>> _getFilesSTBL(String f)
        {
            // We default to adding to the currently selected STBLGroup and target language
            uint g = STBLGroupKey.Key == null ? StringTables.Keys.Select(x => x.ResourceGroup).DefaultIfEmpty().Max() : STBLGroupKey.Key.ResourceGroup;
            ulong i = (STBLGroupKey.Key == null ? FNV64.GetHash(fileName + DateTime.Now.ToBinary().ToString()) >> 8 : STBLGroupKey.Key.Instance) | (ulong)targetLang << 56;
            IResourceKey rk = new RK(STBL, g, i);

            string[] fn = Path.GetFileName(f).Split('_');
            if (fn.Length < 4
                || fn[0] != "S3"
                || fn[1].Length != 8
                || fn[1] != STBL.ToString("X8")
                || fn[2].Length != 8
                || !uint.TryParse(fn[2], System.Globalization.NumberStyles.HexNumber, null, out g)
                || fn[3].Length != 16
                || !ulong.TryParse(fn[3], System.Globalization.NumberStyles.HexNumber, null, out i)
                || getLanguage(i) > locales.Count
                )
            {
                if (MessageBox.Show("File name is not in expected format.  " +
                    "Content will be imported into the target language in the current string set.\n\nContinue?",
                    "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    yield break;
                yield return Tuple.Create(f, rk);
            }
            else
            {
                // Everything looked good, so use the values (type is okay)
                rk.ResourceGroup = g;
                rk.Instance = i;

                // Ask about slurping languages
                if (MessageBox.Show("Attempt to import files for all languages?", "Import", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    rk.Instance = i & 0x00FFFFFFFFFFFFFF;
                    for (int l = 0; l < locales.Count; l++)
                    {
                        // Generate the instance per locale
                        var langRK = stblGroupKeyToRK(rk, l);

                        var stbl = Directory.GetFiles(Path.GetDirectoryName(f), String.Format("S3_{0:X8}_{1:X8}_{2:X16}*.stbl", STBL, langRK.ResourceGroup, langRK.Instance))
                            .SingleOrDefault();
                        if (stbl != null)
                        {
                            yield return Tuple.Create(stbl, langRK);
                        }
                    }
                }
                else
                    yield return Tuple.Create(f, rk);
            }
        }

        IEnumerable<Tuple<String, IResourceKey>> _getFilesNRaas(String f)
        {
            // We default to adding to the currently selected STBLGroup and target language
            // If there isn't a currently selected STBLGroup, create one
            uint g = STBLGroupKey.Key == null ? StringTables.Keys.Select(x => x.ResourceGroup).DefaultIfEmpty().Max() : STBLGroupKey.Key.ResourceGroup;
            ulong i = (STBLGroupKey.Key == null ? FNV64.GetHash(fileName + DateTime.Now.ToBinary().ToString()) >> 8 : STBLGroupKey.Key.Instance) | (ulong)targetLang << 56;
            IResourceKey rk = new RK(STBL, g, i);

            string[] fn = Path.GetFileNameWithoutExtension(f).Split('_');
            //StringTableStrings_ENG_US_000f16b00ba8342f.txt
            if (fn.Length < 4
                || fn[0] != "StringTableStrings"
                || !nraas_locales.Contains(fn[1] + "_" + fn[2])
                || !ulong.TryParse(fn[3], System.Globalization.NumberStyles.HexNumber, null, out i)
                || getLanguage(i) > nraas_locales.Count
                )
            {
                if (MessageBox.Show("File name is not in Nraas Packer format.  " +
                    "Content will be imported into the target language in the current string set.\n\nContinue?",
                    "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    yield break;
                yield return Tuple.Create(f, rk);
            }
            else
            {
                // Everything looked good, so use the values
                rk.Instance = i;

                // Ask about slurping languages
                if (MessageBox.Show("Attempt to import files for all languages?", "Import", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    rk.Instance = i & 0x00FFFFFFFFFFFFFF;
                    for (int l = 0; l < nraas_locales.Count; l++)
                    {
                        // Generate the instance per locale
                        var langRK = stblGroupKeyToRK(rk, l);

                        f = Path.Combine(Path.GetDirectoryName(f), NRaasName(langRK));
                        if (File.Exists(f))
                        {
                            yield return Tuple.Create(f, langRK);
                        }
                    }
                }
                else
                    yield return Tuple.Create(f, rk);
            }
        }

        Tuple<String, IResourceKey, IDictionary<ulong, string>> _parseSTBL(String f, IResourceKey rk)
        {
            using (FileStream fs = new FileStream(f, FileMode.Open, FileAccess.Read))
            {
                var name = Path.GetFileNameWithoutExtension(f);
                var prefix = new RK(0, 0, 0).ToString();
                name = name.Replace(String.Format("S3_{0:X8}_{1:X8}_{2:X16}", rk.ResourceType, rk.ResourceGroup, rk.Instance), "").TrimStart('_');
                int i = name.IndexOf("%%+");
                if (i >= 0)
                    name = name.Substring(0, i);
                if (name == "")
                    name = NameMapName(rk);
                return Tuple.Create(name, rk, (IDictionary<ulong, string>)new StblResource.StblResource(0, fs));
            }
        }

        Tuple<String, IResourceKey, IDictionary<ulong, string>> _parseNRaas(String f, IResourceKey rk)
        {
            // Extend any exception with the file name and rethrow
            try
            {
                using (FileStream fs = new FileStream(f, FileMode.Open, FileAccess.Read))
                {
                    return Tuple.Create(Path.GetFileNameWithoutExtension(f), rk, (IDictionary<ulong, string>)fs.ReadKeyStr().checkDuplicates().ToDictionary(x => x.Item1, x => x.Item2));
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message + "\n\nFile not imported: " + f, ex); }
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

        class RK : IResourceKey
        {
            public RK(uint rt, uint rg, ulong i) { ResourceType = rt; ResourceGroup = rg; Instance = i; }
            public static RK Default { get { return new RK(0, 0, 0); } }
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

        IResourceKey RKTostblGroupKey(IResourceKey rk)
        {
            return new RK(rk.ResourceType, rk.ResourceGroup, rk.Instance & 0x00FFFFFFFFFFFFFF);
        }

        int getLanguage(ulong instance)
        {
            return (int)(instance >> 56);
        }

        String NameMapName(IResourceKey rk)
        {
            return String.Format("Strings_{0}_{1:x16}", locales[getLanguage(rk.Instance)].Replace(' ', '_'), rk.Instance);
        }

        String STBLName(IResourceKey rk)
        {
            return String.Format("S3_{0:X8}_{1:X8}_{2:X16}_{3}%%+STBL.stbl", STBL, rk.ResourceGroup, rk.Instance, NameMapName(rk));
        }

        String NRaasName(IResourceKey rk)
        {
            //StringTableStrings_ENG_US_000f16b00ba8342f.txt
            return String.Format("StringTableStrings_{0}_{1:x16}.txt", nraas_locales[getLanguage(rk.Instance)], rk.Instance);
        }


        class guidRK : Tuple<ulong, IResourceKey>, IEqualityComparer<guidRK>, IEquatable<guidRK>, IComparable<guidRK>
        {
            static guidRK _defaultGuidRK = new guidRK(0, new RK(0, 0, 0));
            public static guidRK Default { get { return _defaultGuidRK; } }

            public guidRK(ulong guid, IResourceKey rk) : base(guid, rk) { }
            public ulong guid { get { return Item1; } }
            public IResourceKey rk { get { return Item2; } }

            public bool Equals(guidRK x, guidRK y) { return x.CompareTo(y) == 0; }
            public bool Equals(guidRK other) { return guid.Equals(other.guid) && rk.Equals(other.rk); }
            public int GetHashCode(guidRK obj) { return guid.GetHashCode() ^ rk.GetHashCode(); }

            public int CompareTo(guidRK other)
            {
                var res = guid.CompareTo(other.guid);
                if (res != 0) return res;
                return rk.CompareTo(other.rk);
            }
        }
    }

    static class Extensions
    {
        public static IEnumerable<Tuple<ulong, string>> ReadKeyStr(this FileStream fs)
        {
            // This was originally Unicode.  That gets weird Chinese symbols on some files.  So we'll use UTF-8.
            using (LineReader sr = new LineReader(fs))
            {
                string _key;
                while ((_key = sr.ReadNonBlankLine()) != null)
                {
                    // Read the guid okay
                    _key = _key.Trim();
                    if (!_key.StartsWith("<KEY>") || !_key.EndsWith("</KEY>"))
                    {
                        throw new InvalidDataException("Expected KEY delimiters missing at line " + sr.Line + ".");
                    }
                    var start = _key.IndexOf("<KEY>") + 5;
                    var length = _key.LastIndexOf("</KEY>") - start;
                    _key = _key.Substring(start, length);

                    ulong guid = 0;
                    if (!_key.StartsWith("0x") || !ulong.TryParse(_key.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out guid))
                        guid = System.Security.Cryptography.FNV64.GetHash(_key);

                    // Read the string
                    string _str = sr.ReadNonBlankLine();
                    if (_str == null)
                    {
                        throw new InvalidDataException("Missing STR at line " + sr.Line + ".");
                    }
                    if (!_str.TrimStart().StartsWith("<STR>"))
                    {
                        throw new InvalidDataException("Expected STR start delimiter missing at line " + sr.Line + ".");
                    }

                    System.Text.StringBuilder value = new System.Text.StringBuilder();
                    start = _str.IndexOf("<STR>") + 5;
                    value.AppendLine(_str.Substring(start));

                    while (!_str.TrimEnd().EndsWith("</STR>") && (_str = sr.ReadLine()) != null)
                    {
                        value.AppendLine(_str);
                    }
                    if (_str == null || !_str.TrimEnd().EndsWith("</STR>"))
                    {
                        throw new InvalidDataException("Expected STR end delimiter missing at 0x" + sr.BaseStream.Position.ToString("X") + ".");
                    }
                    _str = value.ToString().TrimEnd();

                    yield return Tuple.Create(guid, _str.Substring(0, _str.LastIndexOf("</STR>")));
                }
            }
        }

        public static string ReadNonBlankLine(this LineReader lr)
        {
            foreach (var line in lr)
            {
                if (line != "")
                    return line;
            }

            return null;
        }

        // This is simply to blow up helpfully if there are duplicates
        public static IEnumerable<Tuple<ulong, string>> checkDuplicates(this IEnumerable<Tuple<ulong, string>> _nraasFile)
        {
            var seen = new List<ulong>();
            var dups = new List<ulong>();
            foreach (var item in _nraasFile)
                if (!seen.Contains(item.Item1))
                {
                    seen.Add(item.Item1);
                    yield return item;
                }
                else
                    dups.Add(item.Item1);

            if (dups.Count > 0)
            {
                string values = String.Join(", ", dups.Distinct().Select(i => "0x" + i.ToString("X")));
                throw new InvalidDataException("The following hashed key values appear more than once:\n" + values + "\n\nPlease resolve duplicates and try again.");
            }
        }
    }

    class LineReader : StreamReader, IEnumerable<string>
    {
        class LineEnumerator : IEnumerator<string>
        {
            StreamReader StreamReader { get; set; }
            int _line = 0;
            string _current = null;

            public LineEnumerator(StreamReader r) { StreamReader = r; }

            public int Line { get { return _line; } }

            public string Current { get { return _current; } }

            /// <summary>
            /// No dispose here - we only wrap the stream reader passed in.
            /// </summary>
            public void Dispose() { }

            object System.Collections.IEnumerator.Current { get { return this.Current; } }

            public bool MoveNext()
            {
                if (StreamReader.EndOfStream)
                    return false;

                _current = StreamReader.ReadLine();
                _line++;
                return true;
            }

            public void Reset() { throw new NotSupportedException(); }
        }

        LineEnumerator _enumerator { get; set; }

        public LineReader(Stream s) : base(s, System.Text.Encoding.UTF8) { _enumerator = new LineEnumerator(this); }

        public IEnumerator<string> GetEnumerator() { return _enumerator; }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return _enumerator; }

        public int Line { get { return _enumerator.Line; } }
    }
}
