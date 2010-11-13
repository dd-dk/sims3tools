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

namespace S3Pack
{
    public partial class Pack : Form
    {
        bool haveSource = false;
        bool haveTarget = false;
        public Pack()
        {
            InitializeComponent();
        }

        private void Pack_Shown(object sender, EventArgs e)
        {
            btnSource_Click(null, null);
            btnTarget_Click(null, null);
        }

        private void btnSource_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = tbSource.Text;
            if (folderBrowserDialog1.ShowDialog() != DialogResult.OK) return;
            haveSource = true;
            btnOK.Enabled = haveSource && haveTarget;
            tbSource.Text = folderBrowserDialog1.SelectedPath;
            tbStatus.Text = btnOK.Enabled ? "Click 'Pack...' to create a Sims3Pack." : "";
        }

        private void btnTarget_Click(object sender, EventArgs e)
        {
            folderBrowserDialog2.SelectedPath = tbTarget.Text;
            if (folderBrowserDialog2.ShowDialog() != DialogResult.OK) return;
            haveTarget = true;
            btnOK.Enabled = haveSource && haveTarget;
            tbTarget.Text = folderBrowserDialog2.SelectedPath;
            tbStatus.Text = btnOK.Enabled ? "Click 'Pack...' to create a Sims3Pack." : "";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            S3Pack.Sims3Pack.Pack(folderBrowserDialog1.SelectedPath, folderBrowserDialog2.SelectedPath);
            tbSource.Text = tbTarget.Text = "";
            btnOK.Enabled = haveSource = haveTarget = false;
            tbStatus.Text = "Done.  Select another Sims3Pack and target folder, or Exit.";
            //MainForm_Shown(null, null);
        }
    }
}
