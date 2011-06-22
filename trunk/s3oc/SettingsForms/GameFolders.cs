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
using System.Windows.Forms;
using System.IO;

namespace ObjectCloner.SettingsForms
{
    public partial class GameFolders : Form
    {
        static Dictionary<string, string> gameDirs = new Dictionary<string, string>();
        static GameFolders()
        {
            gameDirs = new Dictionary<string, string>();
            foreach (string s in ObjectCloner.Properties.Settings.Default.InstallDirs.Split(';'))
            {
                string[] p = s.Split(new char[] { '=' }, 2);
                if (S3ocSims3.byName(p[0]) != null && Directory.Exists(p[1]))
                    gameDirs.Add(p[0], p[1]);
            }
        }

        static void Save()
        {
            string value = "";
            foreach (var kvp in gameDirs)
                value += ";" + kvp.Key + "=" + kvp.Value;
            ObjectCloner.Properties.Settings.Default.InstallDirs = value.TrimStart(';');
        }

        static GameFolders _gameFolders = null;
        public static GameFolders gameFolders { get { if (_gameFolders == null) _gameFolders = new GameFolders(); return _gameFolders; } }
        public string this[S3ocSims3 key]
        {
            get { return gameDirs.ContainsKey(key.subjectName) ? gameDirs[key.subjectName] : key.hasDefaultInstallDir; }
            private set
            {
                if (safeGetFullPath(gameDirs.ContainsKey(key.subjectName) ? gameDirs[key.subjectName] : null) == safeGetFullPath(value)) return;

                if (gameDirs.ContainsKey(key.subjectName))
                {
                    if (safeGetFullPath(key.hasDefaultInstallDir) == safeGetFullPath(value))
                        gameDirs.Remove(key.subjectName);
                    else
                        gameDirs[key.subjectName] = value == null ? "" : value;
                }
                else
                    gameDirs.Add(key.subjectName, value);
                Save();
            }
        }
        static string safeGetFullPath(string value) { return value == null ? null : Path.GetFullPath(value); }

        public static bool IsEnabled(S3ocSims3 sims3) { return EPsDisabled.IsDisabled(sims3.subjectName) ? false : sims3.isSuppressed < 1; }

        public static void SetEnabled(S3ocSims3 sims3, bool value) { if (sims3.isSuppressed == 0) EPsDisabled.Disable(sims3.subjectName, !value); }


        private Dictionary<int, S3ocSims3> dS3ocSims3 = new Dictionary<int, S3ocSims3>();
        public GameFolders()
        {
            InitializeComponent();

            btnCCEdit.Enabled = ckbCustomContent.Checked = ObjectCloner.Properties.Settings.Default.CCEnabled;
            tbCCFolder.Text = ObjectCloner.Properties.Settings.Default.CustomContent;

            btnPkgEdEdit.Enabled = ckbEditor.Checked = ObjectCloner.Properties.Settings.Default.pkgEditorEnabled;
            tbEditorPath.Text = ObjectCloner.Properties.Settings.Default.pkgEditorPath;

            Size size = this.Size;
            Size sizeTLP = tlpGameFolders.Size;

            foreach (S3ocSims3 sims3 in s3ocTTL.lS3ocSims3)
            {
                dS3ocSims3.Add(tlpGameFolders.RowCount - 2, sims3);

                tlpGameFolders.RowCount++;
                tlpGameFolders.RowStyles.Insert(tlpGameFolders.RowCount - 2, new RowStyle(SizeType.AutoSize));

                Label lbGameID = new Label();
                CheckBox ckbEnabled = new CheckBox();
                TextBox tbInstFolder = new TextBox();
                Button btnEdit = new Button();

                lbGameID.Anchor = AnchorStyles.None;
                lbGameID.AutoSize = true;
                lbGameID.Text = sims3.subjectName;

                ckbEnabled.Anchor = AnchorStyles.None;
                ckbEnabled.AutoSize = true;
                ckbEnabled.Visible = sims3.isSuppressed == 0;
                ckbEnabled.Checked = IsEnabled(sims3);
                ckbEnabled.CheckedChanged += new EventHandler(ckbEnabled_CheckedChanged);

                tbInstFolder.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                tbInstFolder.Text = this[sims3] == null ? "(not set)" : this[sims3];
                tbInstFolder.ReadOnly = true;

                btnEdit.Anchor = AnchorStyles.None;
                btnEdit.AutoSize = true;
                btnEdit.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
                btnEdit.Enabled = IsEnabled(sims3);
                btnEdit.Text = "Edit";
                btnEdit.Click += new EventHandler(btnEdit_Click);

                tlpGameFolders.Controls.Add(lbGameID, 0, tlpGameFolders.RowCount - 2);
                tlpGameFolders.Controls.Add(ckbEnabled, 1, tlpGameFolders.RowCount - 2);
                tlpGameFolders.Controls.Add(tbInstFolder, 2, tlpGameFolders.RowCount - 2);
                tlpGameFolders.Controls.Add(btnEdit, 3, tlpGameFolders.RowCount - 2);
            }

            this.Size = new Size(size.Width, size.Height - sizeTLP.Height + tlpGameFolders.Size.Height);
        }

        S3ocSims3 S3ocSims3FromControl(Control c)
        {
            int row = tlpGameFolders.GetCellPosition(c).Row - 1;
            if (!dS3ocSims3.ContainsKey(row)) return null;
            return dS3ocSims3[row];
        }

        void ckbEnabled_CheckedChanged(object sender, EventArgs e)
        {
            S3ocSims3 sims3 = S3ocSims3FromControl((Control)sender);
            if (sims3 == null) return;

            SetEnabled(sims3, !IsEnabled(sims3));

            Button btn = tlpGameFolders.GetControlFromPosition(3, tlpGameFolders.GetCellPosition((Control)sender).Row) as Button;
            if (btn != null) btn.Enabled = IsEnabled(sims3);
        }

        void btnEdit_Click(object sender, EventArgs e)
        {
            S3ocSims3 sims3 = S3ocSims3FromControl((Control)sender);
            if (sims3 == null) return;

            folderBrowserDialog1.SelectedPath = this[sims3] == null ? "" : this[sims3];
            DialogResult dr = folderBrowserDialog1.ShowDialog();
            if (dr != DialogResult.OK) return;

            this[sims3] = folderBrowserDialog1.SelectedPath;

            TextBox tb = tlpGameFolders.GetControlFromPosition(2, tlpGameFolders.GetCellPosition((Control)sender).Row) as TextBox;
            tb.Text = this[sims3] == null ? "(not set)" : this[sims3];
            tb.BackColor = SystemColors.Control;

            List<string> files = IsEnabled(sims3) ? s3ocTTL.GetPath(this[sims3], sims3, null) : null;
            foreach(string file in files)
                if (!File.Exists(file))
                {
                    CopyableMessageBox.Show("One or more expected packages in the path you chose was not found.", "File not found",
                        CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Warning);
                    tb.BackColor = SystemColors.ControlDark;
                    break;
                }

        }

        private void btnCCEdit_Click(object sender, EventArgs e)
        {
            string path = ObjectCloner.Properties.Settings.Default.CustomContent;
            if (path == null) path = Environment.GetEnvironmentVariable("USERPROFILE");
            folderBrowserDialog1.SelectedPath = path;
            DialogResult dr = folderBrowserDialog1.ShowDialog();
            if (dr != DialogResult.OK) return;

            tbCCFolder.Text = ObjectCloner.Properties.Settings.Default.CustomContent = folderBrowserDialog1.SelectedPath;
        }

        private void ckbCustomContent_CheckedChanged(object sender, EventArgs e)
        {
            btnCCEdit.Enabled = ObjectCloner.Properties.Settings.Default.CCEnabled = ckbCustomContent.Checked;
        }

        private void btnPkgEdEdit_Click(object sender, EventArgs e)
        {
            string path = ObjectCloner.Properties.Settings.Default.pkgEditorPath;
            if (path != null && File.Exists(path)) { openFileDialog1.InitialDirectory = Path.GetDirectoryName(path); openFileDialog1.FileName = Path.GetFileName(path); }
            else { openFileDialog1.InitialDirectory = Environment.GetEnvironmentVariable("PROGRAMFILES"); openFileDialog1.FileName = "*.exe"; }
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr != DialogResult.OK) return;

            tbEditorPath.Text = ObjectCloner.Properties.Settings.Default.pkgEditorPath = Path.GetFullPath(openFileDialog1.FileName);
        }

        private void ckbEditor_CheckedChanged(object sender, EventArgs e)
        {
            btnPkgEdEdit.Enabled = ObjectCloner.Properties.Settings.Default.pkgEditorEnabled = ckbEditor.Checked;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            foreach (S3ocSims3 sims3 in s3ocTTL.lS3ocSims3)
            {
                SetEnabled(sims3, sims3.isSuppressed == 0);
                this[sims3] = sims3.hasDefaultInstallDir;
            }
            DialogResult = DialogResult.Retry;
            Close();
        }
    }
}
