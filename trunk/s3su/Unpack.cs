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

        private void Unpack_Shown(object sender, EventArgs e)
        {
            OKforOK();
        }

        private void btnSource_Click(object sender, EventArgs e)
        {
            ofdSims3Pack.FilterIndex = 1;
            if (ofdSims3Pack.ShowDialog() != DialogResult.OK) return;
            tbSource.Text = Path.GetFullPath(ofdSims3Pack.FileName);

            haveSource = true;
            OKforOK();
        }

        private void btnTarget_Click(object sender, EventArgs e)
        {
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
            S3Pack.Sims3Pack.Unpack(tbSource.Text, tbTarget.Text);

            CopyableMessageBox.Show("Done!", "Sims3Pack unpacked", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Information);

            tbSource.Text = tbTarget.Text = "";
            haveSource = haveTarget = false;
            OKforOK();
        }

        void OKforOK()
        {
            btnOK.Enabled = haveSource && haveTarget;
            tbStatus.Text = btnOK.Enabled ? "Click 'Unpack...' to unpack the Sims3Pack." : "Select Sim3Pack and folder to contain output folder.";
        }
    }
}
