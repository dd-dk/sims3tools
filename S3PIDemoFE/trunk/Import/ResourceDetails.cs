/***************************************************************************
 *  Copyright (C) 2009 by Peter L Jones                                    *
 *  peter@users.sf.net                                                     *
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

namespace S3PIDemoFE
{
    public partial class ResourceDetails : Form
    {
        bool requestFilename = false;
        public ResourceDetails() : this(false, false) { }
        public ResourceDetails(bool useName) : this(useName, false) { }
        public ResourceDetails(bool useName, bool requestFilename)
        {
            InitializeComponent();
#if !DEBUG
            this.ckbCompress.Enabled = false;
#endif
            this.requestFilename = requestFilename;
            UseName = requestFilename && useName;
            lbFilename.Visible = tbFilename.Visible = requestFilename;
        }

        public string Filename { get { return tbFilename.Text; } set { this.tbFilename.Text = value; } }
        public TGIN TGIN { get { return this.tbFilename.Text; } }
        public uint ResourceType
        {
            get { return Convert.ToUInt32(tbType.Text, tbType.Text.StartsWith("0x") ? 16 : 10); }
            set { tbType.Text = "0x" + value.ToString("X8"); }
        }
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
        public string ResourceName { get { return tbName.Text; } set { tbName.Text = value; } }
        public bool Overwrite { get { return ckbOverwrite.Checked; } set { ckbOverwrite.Checked = value; } }
        public bool Compress { get { return ckbCompress.Checked; } set { ckbCompress.Checked = value; } }

        public bool UseName { get { return ckbUseName.Checked; } set { ckbRename.Enabled = tbName.Enabled = ckbUseName.Checked = value; } }
        public bool AllowRename { get { return ckbRename.Checked; } set { ckbRename.Checked = value; } }

        private void FillPanel()
        {
            TGIN details = this.tbFilename.Text;
            tbType.Text = "0x" + details.ResType.ToString("X8");
            tbGroup.Text = "0x" + details.ResGroup.ToString("X8");
            tbInstance.Text = "0x" + details.ResInstance.ToString("X16");
            tbName.Text = details.ResName;
        }

        private void btnOKCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = (sender as Button).DialogResult;
        }

        private void tbFilename_DoubleClick(object sender, EventArgs e)
        {
            ImportResource_Load(null, null);
        }

        private void ImportResource_Load(object sender, EventArgs e)
        {
            if (!requestFilename) return;

            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr != DialogResult.OK && Filename == "")
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }

            this.tbFilename.Text = openFileDialog1.FileName;
        }

        private void tbTGI_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            try
            {
                if (tbInstance.Equals(sender))
                    Convert.ToUInt64(tb.Text, tb.Text.StartsWith("0x") ? 16 : 10);
                else
                    Convert.ToUInt32(tb.Text, tb.Text.StartsWith("0x") ? 16 : 10);
                btnOK.Enabled = tbType.Text.Length * tbGroup.Text.Length * tbInstance.Text.Length > 0;
            }
            catch { btnOK.Enabled = false; }
        }

        private void ckbUseName_CheckedChanged(object sender, EventArgs e)
        {
            ckbRename.Enabled = tbName.Enabled = ckbUseName.Checked;
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
    }
}
