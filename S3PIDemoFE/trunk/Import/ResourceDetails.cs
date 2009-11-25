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
using s3pi.Extensions;
using s3pi.Interfaces;

namespace S3PIDemoFE
{
    public partial class ResourceDetails : Form, IResourceKey
    {
        public ResourceDetails() : this(true, true) { }
        public ResourceDetails(bool useName, bool displayFilename)
        {
            InitializeComponent();
            this.Icon = ((System.Drawing.Icon)(new ComponentResourceManager(typeof(MainForm)).GetObject("$this.Icon")));
            tbName.Enabled = UseName = useName;
            lbFilename.Visible = tbFilename.Visible = displayFilename;
        }
        public ResourceDetails(bool useName, bool displayFilename, IResourceKey rk)
            :this(useName, displayFilename)
        {
            ResourceType = rk.ResourceType;
            ResourceGroup = rk.ResourceGroup;
            Instance = rk.Instance;
            EpFlags = rk.EpFlags;
        }

        #region IResourceKey Members
        public uint ResourceType { get { return cbType.Value; } set { cbType.Value = value; } }
        public uint ResourceGroup
        {
            get { return Convert.ToUInt32(tbGroup.Text, tbGroup.Text.StartsWith("0x") ? 16 : 10); }
            set { tbGroup.Text = "0x" + value.ToString("X8"); }
        }
        public ulong Instance
        {
            get { return Convert.ToUInt64(tbInstance.Text, tbInstance.Text.StartsWith("0x") ? 16 : 10); }
            set { tbInstance.Text = "0x" + value.ToString("X16"); }
        }
        public EPFlags EpFlags
        {
            get { return (EPFlags)Convert.ToByte(tbEPFlags.Text, tbEPFlags.Text.StartsWith("0x") ? 16 : 10); }
            set { tbEPFlags.Text = "0x" + ((byte)value).ToString("X2"); }
        }
        #endregion

        public string ResourceName { get { return tbName.Text; } set { tbName.Text = value; } }
        public bool Replace { get { return importSettings1.Replace; } }
        public bool Compress { get { return importSettings1.Compress; } set { importSettings1.Compress = value; } }

        public bool UseName { get { return importSettings1.UseName; } set { importSettings1.UseName = value; } }
        public bool AllowRename { get { return importSettings1.AllowRename; } set { importSettings1.AllowRename = value; } }

        public string Filename { get { return tbFilename.Text; } set { this.tbFilename.Text = value; } }
        public static implicit operator TGIN(ResourceDetails form) { return form.tbFilename.Text; }
        //public static implicit operator ResourceDetails(TGIN value) { ResourceDetails res = new ResourceDetails(); res.Filename = value; return res; }

        private void FillPanel()
        {
            TGIN details = this.tbFilename.Text;
            cbType.Value = details.ResType;
            tbGroup.Text = "0x" + (details.ResGroup & 0x00FFFFFF).ToString("X6");
            tbInstance.Text = "0x" + details.ResInstance.ToString("X16");
            tbEPFlags.Text = "0x" + (details.ResGroup >> 24).ToString("X2");
            tbName.Text = details.ResName;
        }

        private void btnOKCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = (sender as Button).DialogResult;
        }

        private void tbTGI_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.Text.Length > 0)
                try
                {
                    if (tbInstance.Equals(sender))
                        Convert.ToUInt64(tb.Text, tb.Text.StartsWith("0x") ? 16 : 10);
                    else
                        Convert.ToUInt32(tb.Text, tb.Text.StartsWith("0x") ? 16 : 10);
                    btnOK.Enabled = cbType.Valid && (tbGroup.Text.Length * tbInstance.Text.Length > 0);
                }
                catch { btnOK.Enabled = false; }
            else
                btnOK.Enabled = false;
        }

        private void ckbUseName_CheckedChanged(object sender, EventArgs e)
        {
            tbName.Enabled = importSettings1.UseName;
        }

        private void tbFilename_DragOver(object sender, DragEventArgs e)
        {
            if ((new List<string>(e.Data.GetFormats())).Contains("FileDrop"))
                e.Effect = DragDropEffects.Copy;
        }

        private void tbFilename_DragDrop(object sender, DragEventArgs e)
        {
            string[] fileDrop = e.Data.GetData("FileDrop") as String[];
            if (fileDrop != null && fileDrop.Length > 0)
                this.tbFilename.Text = fileDrop[0];
        }

        private void tbFilename_TextChanged(object sender, EventArgs e)
        {
            FillPanel();
        }

        private void cbType_ValidChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = cbType.Valid && (tbGroup.Text.Length * tbInstance.Text.Length > 0);
        }

        #region IEqualityComparer<IResourceKey> Members
        public bool Equals(IResourceKey x, IResourceKey y) { return x.Equals(y); }
        public int GetHashCode(IResourceKey obj) { return obj.GetHashCode(); }
        public override int GetHashCode() { return ResourceType.GetHashCode() ^ ResourceGroup.GetHashCode() ^ Instance.GetHashCode(); }
        #endregion

        #region IEquatable<IResourceKey> Members

        public bool Equals(IResourceKey other) { return CompareTo(other) == 0; }

        #endregion

        #region IComparable<IResourceKey> Members

        public int CompareTo(IResourceKey other)
        {
            int res = ResourceType.CompareTo(other.ResourceType); if (res != 0) return res;
            res = ResourceGroup.CompareTo(other.ResourceGroup); if (res != 0) return res;
            return Instance.CompareTo(other.Instance);
        }

        #endregion
    }
}
