/***************************************************************************
 *  Copyright (C) 2011 by Peter L Jones                                    *
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
    public partial class Repack : Form
    {
        bool haveSource = false;
        bool haveTarget = false;
        XmlValues xv = null;

        public Repack()
        {
            InitializeComponent();
        }

        public Repack(string source)
            : this()
        {
            if (File.Exists(source))
                setSource(Path.GetFullPath(source));
        }

        private void Repack_Shown(object sender, EventArgs e)
        {
            OKforOK();
        }

        private void btnSource_Click(object sender, EventArgs e)
        {
            try { ofdSims3PackXML.InitialDirectory = haveSource ? Path.GetDirectoryName(tbSource.Text) : ""; }
            catch { ofdSims3PackXML.InitialDirectory = ""; }

            try { ofdSims3PackXML.FileName = haveSource && File.Exists(tbSource.Text) ? Path.GetFileName(tbSource.Text) : "0x*.xml"; }
            catch { ofdSims3PackXML.FileName = "0x*.xml"; }

            ofdSims3PackXML.FilterIndex = 1;
            if (ofdSims3PackXML.ShowDialog() != DialogResult.OK) return;

            setSource(Path.GetFullPath(ofdSims3PackXML.FileName));
        }

        private void btnTarget_Click(object sender, EventArgs e)
        {
            string defaultTarget = haveTarget ? tbTarget.Text : haveSource ? getTarget(tbSource.Text) : "";

            try { sfdSims3Pack.InitialDirectory = Path.GetDirectoryName(defaultTarget); }
            catch { sfdSims3Pack.InitialDirectory = ""; }

            try { sfdSims3Pack.FileName = Path.GetFileName(defaultTarget); }
            catch { sfdSims3Pack.FileName = "*.Sims3Pack"; }

            sfdSims3Pack.FilterIndex = 1;
            if (sfdSims3Pack.ShowDialog() != DialogResult.OK) return;

            tbTarget.Text = Path.GetFullPath(sfdSims3Pack.FileName);

            haveTarget = true;
            OKforOK();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            S3Pack.Sims3Pack.Pack(tbSource.Text, tbTarget.Text, xv);

            CopyableMessageBox.Show("Done!", "Sims3Pack created", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Information);

            ResetSource();
            ResetTarget();
            xv = null;
            OKforOK();
        }

        void ResetSource() { tbSource.Text = "Select..."; haveSource = false; }
        void ResetTarget() { tbTarget.Text = "Select..."; haveTarget = false; }

        void setSource(string source)
        {
            tbSource.Text = source;

            try
            {
                xv = new XmlValues(tbSource.Text);
                UpdatePackagedFiles(Path.GetDirectoryName(tbSource.Text));
            }
            catch (System.Xml.XmlException xe)
            {
                CopyableMessageBox.IssueException(xe, "There is a problem with the XML.", "s3su repack");
                ResetSource();
                return;
            }

            haveSource = true;
            OKforOK();
        }

        string getTarget(string source)
        {
            string target = Path.GetFileNameWithoutExtension(Path.GetDirectoryName(source)) + ".Sims3Pack";
            return Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(source)), target);
        }

        void OKforOK()
        {
            btnOK.Enabled = haveSource && haveTarget && xv != null;
            tbStatus.Text = btnOK.Enabled ? "Click 'Repack' to repack the Sims3Pack." : "'From Sims3Pack XML' and 'Output Sims3Pack' are required.";
        }

        void UpdatePackagedFiles(string folder)
        {
            if (xv == null)
                return;

            string PackageId = xv.GetInnerText(xv.PackageId, "PackageId", "").ToLower();

            List<PackagedFile> notFound = xv.PackagedFiles.FindAll(x => !PackagedFileExists(folder, x));
            if (notFound.Count == 0) return;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("The following files in the Sims3Pack manifest XML cannot be found:");
            foreach (PackagedFile pf in notFound)
            {
                if (pf.GetInnerText(pf.Guid, "Guid", "").ToLower().Equals(PackageId) || pf.GetInnerText(pf.Name, "Name", "").ToLower().Equals(PackageId + ".package"))
                    throw new System.Xml.XmlException("Missing file found matching outer PackageId.  Please investigate.");

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
