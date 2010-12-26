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
        private Dictionary<int, MainForm.S3ocSims3> dS3ocSims3 = new Dictionary<int, MainForm.S3ocSims3>();
        public GameFolders()
        {
            InitializeComponent();

            Size size = this.Size;
            Size sizeTLP = tlpGameFolders.Size;

            foreach (MainForm.S3ocSims3 sims3 in MainForm.lS3ocSims3)
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
                ckbEnabled.Checked = sims3.Enabled;
                ckbEnabled.CheckedChanged += new EventHandler(ckbEnabled_CheckedChanged);

                tbInstFolder.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                tbInstFolder.Text = (sims3.InstallDir == null) ? "(not set)" : sims3.InstallDir;
                tbInstFolder.ReadOnly = true;

                btnEdit.Anchor = AnchorStyles.None;
                btnEdit.AutoSize = true;
                btnEdit.Enabled = sims3.Enabled;
                btnEdit.Text = "Edit";
                btnEdit.Click += new EventHandler(btnEdit_Click);

                tlpGameFolders.Controls.Add(lbGameID, 0, tlpGameFolders.RowCount - 2);
                tlpGameFolders.Controls.Add(ckbEnabled, 1, tlpGameFolders.RowCount - 2);
                tlpGameFolders.Controls.Add(tbInstFolder, 2, tlpGameFolders.RowCount - 2);
                tlpGameFolders.Controls.Add(btnEdit, 3, tlpGameFolders.RowCount - 2);
            }

            this.Size = new Size(size.Width, size.Height - sizeTLP.Height + tlpGameFolders.Size.Height);
        }

        MainForm.S3ocSims3 S3ocSims3FromControl(Control c)
        {
            int row = tlpGameFolders.GetCellPosition(c).Row - 1;
            if (!dS3ocSims3.ContainsKey(row)) return null;
            return dS3ocSims3[row];
        }

        void ckbEnabled_CheckedChanged(object sender, EventArgs e)
        {
            MainForm.S3ocSims3 sims3 = S3ocSims3FromControl((Control)sender);
            if (sims3 == null) return;

            sims3.Enabled = !sims3.Enabled;

            Button btn = tlpGameFolders.GetControlFromPosition(3, tlpGameFolders.GetCellPosition((Control)sender).Row) as Button;
            if (btn != null) btn.Enabled = sims3.Enabled;
        }

        void btnEdit_Click(object sender, EventArgs e)
        {
            MainForm.S3ocSims3 sims3 = S3ocSims3FromControl((Control)sender);
            if (sims3 == null) return;

            folderBrowserDialog1.SelectedPath = sims3.InstallDir == null ? "" : sims3.InstallDir;
            DialogResult dr = folderBrowserDialog1.ShowDialog();
            if (dr != DialogResult.OK) return;

            sims3.InstallDir = folderBrowserDialog1.SelectedPath;

            TextBox tb = tlpGameFolders.GetControlFromPosition(2, tlpGameFolders.GetCellPosition((Control)sender).Row) as TextBox;
            tb.Text = (sims3.InstallDir == null) ? "(not set)" : sims3.InstallDir;
            tb.BackColor = SystemColors.Control;

            List<string> files = MainForm.iniGetPath(sims3, null);
            foreach(string file in files)
                if (!File.Exists(file))
                {
                    CopyableMessageBox.Show("One or more expected packages in the path you chose was not found.", "File not found",
                        CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Warning);
                    tb.BackColor = SystemColors.ControlDark;
                    break;
                }

        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            foreach (MainForm.S3ocSims3 sims3 in MainForm.lS3ocSims3)
            {
                sims3.Enabled = sims3.isSuppressed == 0;
                sims3.InstallDir = sims3.hasDefaultInstallDir;
            }
            DialogResult = DialogResult.Retry;
            Close();
        }
    }
}
