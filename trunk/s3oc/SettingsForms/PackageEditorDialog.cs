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
using System.Linq;
using System.IO;

namespace ObjectCloner
{
    public partial class PackageEditorDialog : Form
    {
        public PackageEditorDialog()
        {
            InitializeComponent();
        }

        public string PkgEditorPath
        {
            get { return tbEditorPath.Text; }
            set { tbEditorPath.Text = value; }
        }

        public bool PkgEditorEnabled
        {
            get { return ckbEditor.Checked; }
            set { btnPkgEdEdit.Enabled = ckbEditor.Checked = value; }
        }

        private void btnPkgEdEdit_Click(object sender, EventArgs e)
        {
            string path = PkgEditorPath;

            if (path != null && File.Exists(path)) { openFileDialog1.InitialDirectory = Path.GetDirectoryName(path); openFileDialog1.FileName = Path.GetFileName(path); }
            else
                try { openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles); openFileDialog1.FileName = "*.exe"; }
                catch { }
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr != DialogResult.OK) return;

            PkgEditorPath = Path.GetFullPath(openFileDialog1.FileName);
        }

        private void ckbEditor_CheckedChanged(object sender, EventArgs e)
        {
            btnPkgEdEdit.Enabled = ckbEditor.Checked;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Retry;
            Close();
        }
    }
}
