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
    public partial class Pack : Form
    {
        bool havePng = false;
        bool haveSource = false;
        bool haveTarget = false;
        public Pack()
        {
            InitializeComponent();
        }

        private void Pack_Shown(object sender, EventArgs e)
        {
            //btnSource_Click(null, null);
            //btnTarget_Click(null, null);
            OKforOK();
        }

        private void btnSource_Click(object sender, EventArgs e)
        {
            try { ofdSelectPackage.InitialDirectory = Path.GetDirectoryName(tbSource.Text); }
            catch { ofdSelectPackage.InitialDirectory = ""; }

            try { ofdSelectPackage.FileName = File.Exists(tbSource.Text) ? Path.GetFileName(tbSource.Text) : "*.package"; }
            catch { ofdSelectPackage.FileName = "*.package"; }

            ofdSelectPackage.FilterIndex = 1;
            if (ofdSelectPackage.ShowDialog() != DialogResult.OK) return;

            haveSource = true;
            tbSource.Text = Path.GetFullPath(ofdSelectPackage.FileName);

            OKforOK();

            CreatorNameTitle_TextChanged(null, null);

            string filename = Path.GetFileNameWithoutExtension(ofdSelectPackage.FileName);
            string[] split = filename.Split(new char[] { '_', }, 2);
            if (split.Length == 2)
            {
                if (tbCreatorName.Text.Length == 0) tbCreatorName.Text = split[0];
                if (tbTitle.Text.Length == 0) tbTitle.Text = split[1];
                if (tbDisplayName.Text.Length == 0) tbDisplayName.Text = split[1];
            }
        }

        private void CreatorNameTitle_TextChanged(object sender, EventArgs e)
        {
            ulong fnv64a = 0, fnv64b = 0;
            if (tbPackageId.Text.Length == 34) // "0x" + 64bits in hex + 64bits in hex
            {
                ulong.TryParse(tbPackageId.Text.Substring(2, 16), System.Globalization.NumberStyles.HexNumber, null, out fnv64a);
                ulong.TryParse(tbPackageId.Text.Substring(18), System.Globalization.NumberStyles.HexNumber, null, out fnv64b);
            }
            if (tbCreatorName.Text.Length > 0) fnv64a = System.Security.Cryptography.FNV64.GetHash(tbCreatorName.Text);
            if (tbTitle.Text.Length > 0) fnv64b = System.Security.Cryptography.FNV64.GetHash(tbTitle.Text);

            tbPackageId.Text = string.Format("0x{0:x16}{1:x16}", fnv64a, fnv64b);
        }

        private void btnTarget_Click(object sender, EventArgs e)
        {
            try { sfdSims3Pack.InitialDirectory = Path.GetDirectoryName(tbTarget.Text); }
            catch { sfdSims3Pack.InitialDirectory = ""; }

            try { sfdSims3Pack.FileName = File.Exists(tbSource.Text) ? Path.GetFileName(tbTarget.Text) : "*.Sims3Pack"; }
            catch { sfdSims3Pack.FileName = "*.Sims3Pack"; }

            sfdSims3Pack.FilterIndex = 1;
            if (sfdSims3Pack.ShowDialog() != DialogResult.OK) return;

            haveTarget = true;
            tbTarget.Text = Path.GetFullPath(sfdSims3Pack.FileName);

            OKforOK();
        }

        private void cbType_SelectionChangeCommitted(object sender, EventArgs e)
        {
            OKforOK();
        }

        private void cbType_Leave(object sender, EventArgs e)
        {
            OKforOK();
        }

        private void btnThumbnail_Click(object sender, EventArgs e)
        {
            try { ofdThumbnail.InitialDirectory = Path.GetDirectoryName(tbThumbnail.Text); }
            catch { ofdThumbnail.InitialDirectory = ""; }

            try { ofdThumbnail.FileName = File.Exists(tbThumbnail.Text) ? Path.GetFileName(tbThumbnail.Text) : "*.png"; }
            catch { ofdThumbnail.FileName = "*.png"; }

            ofdThumbnail.FilterIndex = 1;
            if (ofdThumbnail.ShowDialog() != DialogResult.OK) return;

            havePng = true;
            tbThumbnail.Text = Path.GetFullPath(ofdThumbnail.FileName);
        }

        private void btnClearThumbnail_Click(object sender, EventArgs e)
        {
            havePng = false;
            tbThumbnail.Text = "";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            S3Pack.Sims3Pack.Pack(new XmlValues()
            {
                Package = ofdSelectPackage.FileName,
                CreatorName = tbCreatorName.Text,
                Title = tbCreatorName.Text,
                Target = sfdSims3Pack.FileName,
                Sims3PackType = cbType.Text,
                SubType = tbSubType.Text,
                ArchiveVersion = tbArchiveVersion.Text,
                CodeVersion = tbCodeVersion.Text,
                GameVersion = tbGameVersion.Text,
                PackageId = tbPackageId.Text,
                Date = tbDate.Text,
                AssetVersion = tbAssetVersion.Text,
                MinReqVersion = tbMinReqVersion.Text,
                DisplayName = tbDisplayName.Text,
                Description = tbDescription.Text,
                EPFlags = tbEPFlags.Text,
                Thumbnail = havePng ? tbThumbnail.Text : null,
            });

            CopyableMessageBox.Show("Done!", "Sims3Pack created", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Information);

            tbSource.Text = tbCreatorName.Text = tbCreatorName.Text =
                tbTarget.Text = cbType.Text = tbPackageId.Text =
                tbDisplayName.Text = tbDescription.Text = tbThumbnail.Text = "";
            tbSubType.Text = "0x00000000";
            havePng = false;

            OKforOK();
            //MainForm_Shown(null, null);
        }

        void OKforOK()
        {
            tbDate.Text = DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss");
            btnOK.Enabled = haveSource && haveTarget && cbType.Text.Length > 0;
            tbStatus.Text = btnOK.Enabled ? "Click 'Pack...' to create a Sims3Pack." : "Source, Target and Type are required.";
        }
    }
}
