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
using System.IO;
using System.Windows.Forms;

namespace S3Pack
{
    public partial class Unpack : Form
    {
        bool haveSource = false;
        bool haveTarget = false;
        public Unpack()
        {
            InitializeComponent();
        }

        public Unpack(string source)
            : this()
        {
            if (File.Exists(source))
                setSource(Path.GetFullPath(source));
        }

        private void Unpack_Shown(object sender, EventArgs e)
        {
            OKforOK();
        }

        private void btnSource_Click(object sender, EventArgs e)
        {
            try { ofdSims3Pack.InitialDirectory = haveSource ? Path.GetDirectoryName(tbSource.Text) : ""; }
            catch { ofdSims3Pack.InitialDirectory = ""; }

            try { ofdSims3Pack.FileName = haveSource && File.Exists(tbSource.Text) ? Path.GetFileName(tbSource.Text) : "*.Sims3Pack"; }
            catch { ofdSims3Pack.FileName = "*.Sims3Pack"; }

            ofdSims3Pack.FilterIndex = 1;
            if (ofdSims3Pack.ShowDialog() != DialogResult.OK) return;

            setSource(Path.GetFullPath(ofdSims3Pack.FileName));
        }

        void setSource(string source)
        {
            tbSource.Text = source;
            haveSource = true;

            OKforOK();
        }

        private void btnTarget_Click(object sender, EventArgs e)
        {
            try { sfdTarget.InitialDirectory = haveTarget ? Path.GetDirectoryName(tbTarget.Text) : haveSource ? Path.GetDirectoryName(tbSource.Text) : ""; }
            catch { sfdTarget.InitialDirectory = ""; }

            sfdTarget.FileName = "Filename will be ignored";
            sfdTarget.FilterIndex = 1;
            if (sfdTarget.ShowDialog() != DialogResult.OK) return;
            tbTarget.Text = Path.GetDirectoryName(Path.GetFullPath(sfdTarget.FileName));

            haveTarget = true;
            OKforOK();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                S3Pack.Sims3Pack.Unpack(tbSource.Text, tbTarget.Text);
                CopyableMessageBox.Show("Done!", "Sims3Pack unpacked", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                CopyableMessageBox.IssueException(ex, "There was a problem unpacking.", "s3su unpack");
            }

            ResetSource();
            ResetTarget();
            OKforOK();
        }

        void ResetSource() { tbSource.Text = "Select..."; haveSource = false; }
        void ResetTarget() { tbTarget.Text = "Select..."; haveTarget = false; }

        void OKforOK()
        {
            btnOK.Enabled = haveSource && haveTarget;
            tbStatus.Text = btnOK.Enabled ? "Click 'Unpack' to unpack the Sims3Pack." : "'From Sims3Pack' and 'Output parent folder' are required.";
        }
    }
}
