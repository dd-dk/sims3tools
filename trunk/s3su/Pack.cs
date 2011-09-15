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
        bool haveSource = false;
        bool haveTarget = false;
        bool havePackageId = false;

        public Pack()
        {
            InitializeComponent();
        }

        public Pack(string source)
            : this()
        {
            if (File.Exists(source))
                setSource(Path.GetFullPath(source));
        }

        private void Pack_Shown(object sender, EventArgs e)
        {
            OKforOK();
        }

        private void btnSource_Click(object sender, EventArgs e)
        {
            try { ofdSelectPackage.InitialDirectory = haveSource ? Path.GetDirectoryName(tbSource.Text) : ""; }
            catch { ofdSelectPackage.InitialDirectory = ""; }

            try { ofdSelectPackage.FileName = haveSource && File.Exists(tbSource.Text) ? Path.GetFileName(tbSource.Text) : "*.package"; }
            catch { ofdSelectPackage.FileName = "*.package"; }

            ofdSelectPackage.FilterIndex = 1;
            if (ofdSelectPackage.ShowDialog() != DialogResult.OK) return;

            setSource(Path.GetFullPath(ofdSelectPackage.FileName));
        }

        private void btnTarget_Click(object sender, EventArgs e)
        {
            string defaultTarget = haveSource ? Path.Combine(Path.GetDirectoryName(tbSource.Text), Path.GetFileNameWithoutExtension(tbSource.Text) + ".Sims3Pack") : "";

            try { sfdSims3Pack.InitialDirectory = haveTarget ? Path.GetDirectoryName(tbTarget.Text) : haveSource ? Path.GetDirectoryName(defaultTarget) : ""; }
            catch { sfdSims3Pack.InitialDirectory = ""; }

            try { sfdSims3Pack.FileName = haveTarget ? Path.GetFileName(tbTarget.Text) : haveSource ? Path.GetFileName(defaultTarget) : "*.Sims3Pack"; }
            catch { sfdSims3Pack.FileName = "*.Sims3Pack"; }

            sfdSims3Pack.FilterIndex = 1;
            if (sfdSims3Pack.ShowDialog() != DialogResult.OK) return;

            tbTarget.Text = Path.GetFullPath(sfdSims3Pack.FileName);
            haveTarget = true;

            OKforOK();
        }

        private void cbType_Leave(object sender, EventArgs e)
        {
            OKforOK();
        }

        private void btnUseSource_Click(object sender, EventArgs e)
        {
            if (haveSource)
            {
                Guid? guid = TryParse(Path.GetFileNameWithoutExtension(tbSource.Text));
                if (guid.HasValue)
                {
                    tbPackageId.Text = "0x" + guid.Value.ToString("n");
                    havePackageId = true;
                }
                else
                    ResetPackageId();
            }

            btnUseSource.Enabled = false;
            OKforOK();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            tbPackageId.Text = "0x" + Guid.NewGuid().ToString("n");
            havePackageId = true;

            btnUseSource.Enabled = haveSource && TryParse(Path.GetFileNameWithoutExtension(tbSource.Text)) != null;
            OKforOK();
        }

        private void ckbCreateManifest_CheckedChanged(object sender, EventArgs e)
        {
            ckbCreateMissingIcon.Checked = ckbCreateMissingIcon.Enabled = ckbCreateManifest.Checked;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string source = tbSource.Text;
            bool deleteSource = false;
            try
            {
                XmlValues xv = new XmlValues();

                xv.SetAttributeValue("Type", tbType.Text);
                xv.SetAttributeValue("SubType", tbSubType.Text);

                xv.SetInnerText("ArchiveVersion", tbArchiveVersion.Text);
                xv.SetInnerText("CodeVersion", tbCodeVersion.Text);
                xv.SetInnerText("GameVersion", tbGameVersion.Text);
                xv.SetInnerText("DisplayName", tbDisplayName.Text);
                xv.SetInnerText("Description", tbDescription.Text);
                xv.SetInnerText("PackageId", tbPackageId.Text);
                //Date gets set by packer
                xv.SetInnerText("AssetVersion", tbAssetVersion.Text);
                xv.SetInnerText("MinReqVersion", tbMinReqVersion.Text);

                if (ckbCreateManifest.Checked)
                {
                    source = CopyToTemp(source, tbPackageId.Text);
                    deleteSource = true;
                    try
                    {
                        Manifest.UpdatePackage(source, tbPackageId.Text, tbType.Text, tbSubType.Text, tbDisplayName.Text, tbDescription.Text, ckbCreateMissingIcon.Checked);
                    }
                    catch (ApplicationException aex)
                    {
                        CopyableMessageBox.Show(aex.Message, "s3su", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Warning);
                        return;
                    }
                }

                //Ensure a minimal entry for the package exists but don't override existing values
                PackagedFile pf = xv.PackagedFiles.Find(p => p.Guid.InnerText.ToLower().Equals(tbPackageId.Text.ToLower()));
                if (pf == null)
                {
                    pf = xv.CreatePackagedFile(tbPackageId.Text + ".package");
                    pf.GetInnerText(pf.Guid, "Guid", tbPackageId.Text);
                    pf.GetInnerText(pf.ContentType, "ContentType", tbType.Text);
                    pf.GetInnerText(pf.EPFlags, "EPFlags", "0x00000000");
                }

                S3Pack.Sims3Pack.Pack(source, tbTarget.Text, xv);

                CopyableMessageBox.Show("Done!", "Sims3Pack created", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Information);
            }
            finally
            {
                if (deleteSource)
                    File.Delete(source);

                ResetSource();
                ResetTarget();
                ResetPackageId();

                tbDisplayName.Text = tbDescription.Text = "";
                tbSubType.Text = "0x00000000";

                ckbCreateManifest.Enabled = false;
                ckbCreateManifest.Checked = ckbCreateManifest.Enabled;

                OKforOK();
            }
        }

        string CopyToTemp(string from, string pkgid)
        {
            string target = Path.Combine(Path.GetTempPath(), pkgid + ".package");
            File.Copy(from, target, true);
            return target;
        }

        void setSource(string source)
        {
            tbSource.Text = source;
            haveSource = true;

            try
            {
                var pkg = s3pi.Package.Package.OpenPackage(0, source);
                s3pi.Package.Package.ClosePackage(0, pkg);
            }
            catch (InvalidDataException ex)
            {
                CopyableMessageBox.IssueException(ex, "There is a problem with the package.", "s3su pack");
                ResetSource();
                return;
            }

            btnUseSource.Enabled = TryParse(Path.GetFileNameWithoutExtension(tbSource.Text)) != null;

            OKforOK();
        }

        void OKforOK()
        {
            btnOK.Enabled = haveSource && haveTarget && havePackageId;
            tbStatus.Text = btnOK.Enabled ?
                "Click 'Pack' to create the Sims3Pack." :
                "'From package', 'Output Sims3Pack' and 'PackageId' are required.";
        }

        void ResetSource() { tbSource.Text = "Select..."; haveSource = false; btnUseSource.Enabled = false; ResetPackageId(); }
        void ResetTarget() { tbTarget.Text = "Select..."; haveTarget = false; }
        void ResetPackageId() { tbPackageId.Text = ""; havePackageId = false; }

        Guid? TryParse(string value)
        {
            if (value.ToLower().StartsWith("0x") && value.Length == 34)
                try
                {
                    Guid guid = new Guid(value.Substring(2));
                    return guid;
                }
                catch { }
            return null;
        }
    }
}
