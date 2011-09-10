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
        XmlValues xv = null;

        public Pack()
        {
            InitializeComponent();
        }

        private void Pack_Shown(object sender, EventArgs e)
        {
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

            string filename = Path.GetFileNameWithoutExtension(ofdSelectPackage.FileName);
            string[] split = filename.Split(new char[] { '_', }, 2);
            if (split.Length == 2)
            {
                if (tbCreatorName.Text.Length == 0) tbCreatorName.Text = split[0];
                if (tbTitle.Text.Length == 0) tbTitle.Text = split[1];
                if (tbDisplayName.Text.Length == 0) tbDisplayName.Text = split[1];
            }
            else
                if (tbCreatorName.Text.Length == 0) tbCreatorName.Text = Environment.UserName;

            try
            {
                xv = XmlValues.GetXmlValues(tbSource.Text);
                if (xv != null)
                {
                    cbType.Text = xv.GetAttributeValue(xv.Sims3PackType, "Type", "");
                    tbSubType.Text = xv.GetAttributeValue(xv.SubType, "SubType", "0x00000000");

                    tbArchiveVersion.Text = xv.GetInnerText(xv.ArchiveVersion, "ArchiveVersion", "1.4");
                    tbCodeVersion.Text = xv.GetInnerText(xv.CodeVersion, "CodeVersion", "0.0.0.1");
                    tbGameVersion.Text = xv.GetInnerText(xv.GameVersion, "GameVersion", "0.0.0.0");

                    if (xv.PackageId != null)
                        tbPackageId.Text = xv.PackageId.InnerText;
                    else
                    {
                        PackagedFile pf = xv.PackagedFiles.Find(x => x.Guid.InnerText != "0x0000000000000000");
                        if (pf != null)
                            tbPackageId.Text = pf.Guid.InnerText;
                        else
                        {
                            tbPackageId.Text = Path.GetFileNameWithoutExtension(tbSource.Text);
                            tbPackageId.Text = "0x" + NewGUID().ToString("n");
                        }
                        xv.SetInnerText("PackageId", tbPackageId.Text);
                    }

                    //Date gets overwritten always
                    tbAssetVersion.Text = xv.GetInnerText(xv.AssetVersion, "AssetVersion", "0");
                    tbMinReqVersion.Text = xv.GetInnerText(xv.MinReqVersion, "MinReqVersion", "1.0.0.0");

                    tbDisplayName.Text = xv.GetInnerText(xv.DisplayName, "DisplayName", tbTitle.Text);
                    tbDescription.Text = xv.GetInnerText(xv.Description, "Description", "");

                    //ckbOWLocalDesc.Checked = xv.LocalisedNames != null && xv.LocalisedNames.TrueForAll(el => el.InnerText == xv.DisplayName.InnerText);
                    //ckbOWLocalName.Checked = xv.LocalisedDescs != null && xv.LocalisedDescs.TrueForAll(el => el.InnerText == xv.Description.InnerText);

                    //tbEPFlags.Text = CommonEPFlags() ?? "0x00000000";

                    if (tbTitle.Text.Length == 0) tbTitle.Text = tbDisplayName.Text;

                    UpdatePackagedFiles(Path.GetDirectoryName(ofdSelectPackage.FileName));
                }
            }
            catch (System.Xml.XmlException xe)
            {
                CopyableMessageBox.IssueException(xe, "There is a problem with the XML found.", "XML error");
                tbSource.Text = "";
                haveSource = false;
            }

            OKforOK();
        }

        private void btnTarget_Click(object sender, EventArgs e)
        {
            if (tbTarget.Text == "")
                tbTarget.Text = (tbSource.Text != "" ? Directory.GetParent(Path.GetDirectoryName(tbSource.Text)) + "\\" : "") +
                    tbCreatorName.Text.Replace(" ", "") + "_" + tbTitle.Text.Replace(" ", "");

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

        private void cbType_Leave(object sender, EventArgs e)
        {
            OKforOK();
        }

        private void btnNewGUID_Click(object sender, EventArgs e)
        {
            tbPackageId.Text = "0x" + NewGUID().ToString("n");
            OKforOK();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (xv == null)
                xv = XmlValues.GetXmlValues(tbSource.Text);
            if (xv == null)
                xv = new XmlValues();

            xv.SetAttributeValue("Type", cbType.Text);
            xv.SetAttributeValue("SubType", tbSubType.Text);

            xv.SetInnerText("ArchiveVersion", tbArchiveVersion.Text);
            xv.SetInnerText("CodeVersion", tbCodeVersion.Text);
            xv.SetInnerText("GameVersion", tbGameVersion.Text);
            xv.SetInnerText("DisplayName", tbDisplayName.Text);
            xv.SetInnerText("Description", tbDescription.Text);
            xv.SetInnerText("PackageId", tbPackageId.Text);
            xv.SetInnerText("Date", tbDate.Text);
            xv.SetInnerText("AssetVersion", tbAssetVersion.Text);
            xv.SetInnerText("MinReqVersion", tbMinReqVersion.Text);

            if (tbSource.Text.ToLower().EndsWith(".package"))
            {
                //Ensure a minimal entry for the package exists but don't override existing values
                PackagedFile pf = xv.PackagedFiles.Find(p => p.Guid.InnerText.ToLower().Equals(tbPackageId.Text.ToLower()));
                if (pf == null)
                {
                    pf = xv.CreatePackagedFile(tbPackageId.Text + ".package");
                    pf.GetInnerText(pf.Guid, "Guid", tbPackageId.Text);
                    pf.GetInnerText(pf.ContentType, "ContentType", cbType.Text);
                    pf.GetInnerText(pf.EPFlags, "EPFlags", "0x00000000");
                }
            }

            //if (ckbOWLocalDesc.Checked) xv.LocalisedNames.IsEmpty = true;
            //if (ckbOWLocalName.Checked) xv.LocalisedDescs.IsEmpty = true;

            S3Pack.Sims3Pack.Pack(ofdSelectPackage.FileName, sfdSims3Pack.FileName, xv);

            CopyableMessageBox.Show("Done!", "Sims3Pack created", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Information);

            tbSource.Text = tbCreatorName.Text = tbTitle.Text =
                tbTarget.Text = cbType.Text = tbPackageId.Text =
                tbDisplayName.Text = tbDescription.Text = "";
            tbSubType.Text = "0x00000000";

            OKforOK();
        }

        void OKforOK()
        {
            tbDate.Text = DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss");
            btnOK.Enabled = haveSource && haveTarget && cbType.Text.Length > 0 && tbPackageId.Text.Length > 0;
            tbStatus.Text = btnOK.Enabled ? "Click 'Pack...' to create a Sims3Pack." : "Source, Target, Type and PackageId are required.";
        }

        Guid NewGUID()
        {
            Guid guid = new Guid();
            if (tbPackageId.Text.ToLower().StartsWith("0x"))
                guid = new Guid(tbPackageId.Text.Substring(2));

            ulong fnv64a = 0, fnv64b = 0;
            string guidStr = guid.ToString("n");
            fnv64a = tbCreatorName.Text.Length > 0
                ? System.Security.Cryptography.FNV64.GetHash(tbCreatorName.Text)
                : ulong.Parse(guidStr.Substring(0, 16), System.Globalization.NumberStyles.HexNumber);
            fnv64b = tbTitle.Text.Length > 0
                ? System.Security.Cryptography.FNV64.GetHash(tbTitle.Text)
                : ulong.Parse(guidStr.Substring(16), System.Globalization.NumberStyles.HexNumber);

            guid = new Guid(fnv64a.ToString("x16") + fnv64b.ToString("x16"));

            return guid;
        }

        string CommonEPFlags() { return null; }

        void UpdatePackagedFiles(string folder)
        {
            List<PackagedFile> notFound = xv.PackagedFiles.FindAll(x => !PackagedFileExists(folder, x));
            if (notFound.Count == 0) return;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("The following files in the Sims3Pack manifest XML cannot be found:");
            foreach (PackagedFile pf in notFound)
            {
                sb.AppendLine(pf.Name.InnerText);
                xv.RemovePackagedFile(pf.Name.InnerText);
            }
            CopyableMessageBox.Show(sb.ToString(), "Missing files", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Warning);
        }

        bool PackagedFileExists(string folder, PackagedFile pf)
        {
            return pf.Name != null &&
                pf.Name.InnerText.Length != 0 &&
                File.Exists(Path.Combine(folder, Path.GetFileName(pf.Name.InnerText)));
        }
    }
}
