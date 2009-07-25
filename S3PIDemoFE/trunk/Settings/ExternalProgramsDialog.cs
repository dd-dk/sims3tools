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
using System.Windows.Forms;

namespace S3PIDemoFE.Settings
{
    public partial class ExternalProgramsDialog : Form
    {
        public ExternalProgramsDialog()
        {
            InitializeComponent();
        }

        public bool HasUserHelpersTxt { get { return ckbOverrideHelpers.Checked; } set { ckbOverrideHelpers.Checked = value; } }

        public string UserHelpersTxt { get { return tbUserHelpersTxt.Text; } set { tbUserHelpersTxt.Text = value; } }

        private void ckbOverrideHelpers_CheckedChanged(object sender, EventArgs e) { btnHelpersBrowse.Enabled = ckbOverrideHelpers.Checked; }

        private void btnHelpersBrowse_Click(object sender, EventArgs e)
        {
            DialogResult dr = ofdUserHelpersTxt.ShowDialog();
            if (dr != DialogResult.OK) return;
            tbUserHelpersTxt.Text = ofdUserHelpersTxt.FileName;
        }

        public bool HasUserHexEditor { get { return ckbUserHexEditor.Checked; } set { ckbUserHexEditor.Checked = value; } }

        public string UserHexEditor { get { return tbUserHexEditor.Text; } set { tbUserHexEditor.Text = value; } }

        public bool HexEditorIgnoreTS { get { return ckbHexEditorTS.Checked; } set { ckbHexEditorTS.Checked = value; } }
        
        public bool HexEditorWantsQuotes { get { return ckbQuotes.Checked; } set { ckbQuotes.Checked = value; } }

        private void ckbUserHexEditor_CheckedChanged(object sender, EventArgs e) { ckbQuotes.Enabled = ckbHexEditorTS.Enabled = btnHexEditorBrowse.Enabled = ckbUserHexEditor.Checked; }

        private void btnHexEditorBrowse_Click(object sender, EventArgs e)
        {
            DialogResult dr = ofdUserHexEditor.ShowDialog();
            if (dr != DialogResult.OK) return;
            tbUserHexEditor.Text = ofdUserHexEditor.FileName;
        }
    }
}
