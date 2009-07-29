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
using StblResource;

namespace s3pi_STBL_Resource_Editor
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            MemoryStream ms = Clipboard.GetData(DataFormats.Serializable) as MemoryStream;
            if (ms == null)
                throw new Exception("Clipboard data not a MemoryStream");

            try
            {
                Application.UseWaitCursor = true;
                loadStbl(ms);
            }
            finally { Application.UseWaitCursor = false; }

            if (lbStrings.Items.Count > 0)
                lbStrings.SelectedIndices.Add(0);
        }

        StblResource.StblResource stbl;
        IDictionary<ulong, string> map;
        void loadStbl(Stream data)
        {
            stbl = new StblResource.StblResource(0, data);
            map = (IDictionary<ulong, string>)stbl;
            foreach (ulong key in map.Keys)
                lbStrings.Items.Add("0x" + key.ToString("X16"));
            Clipboard.Clear();
        }

        void saveStbl()
        {
            MemoryStream ms = new MemoryStream(stbl.AsBytes);
            Clipboard.SetData(DataFormats.Serializable, ms);
        }

        int currentIndex = -1;
        private void lbStrings_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (currentIndex >= 0)
            {
                map[Convert.ToUInt64(lbStrings.Items[currentIndex] + "", 16)] = rtbValue.Text;
            }
            rtbValue.Enabled = lbStrings.SelectedIndices.Count > 0;
            if (lbStrings.SelectedIndices.Count == 0)
            {
                rtbValue.Text = "";
                currentIndex = -1;
                btnDelete.Enabled = false;
            }
            else
            {
                rtbValue.Text = map[Convert.ToUInt64(lbStrings.SelectedItem + "", 16)];
                currentIndex = lbStrings.SelectedIndex;
                btnDelete.Enabled = true;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            lbStrings.SelectedIndices.Clear();
            saveStbl();
            Environment.ExitCode = 0;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Environment.ExitCode = 1;
            this.Close();
        }

        private void tbGUID_TextChanged(object sender, EventArgs e)
        {
            btnAdd.Enabled = tbGUID.Text.Length > 0;
        }

        private void tbGUID_Validating(object sender, CancelEventArgs e)
        {
            string val = tbGUID.Text.Trim();
            if (val.Length == 0) return;
            try { Convert.ToUInt64(val, val.StartsWith("0x") ? 16 : 10); }
            catch { e.Cancel = true; tbGUID.SelectAll(); }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ulong newGUID = Convert.ToUInt64(tbGUID.Text, tbGUID.Text.StartsWith("0x") ? 16 : 10);
            if (map.ContainsKey(newGUID)) { tbGUID.Focus(); tbGUID.SelectAll(); }
            else { map.Add(newGUID, ""); lbStrings.Items.Add("0x" + newGUID.ToString("X16")); lbStrings.SelectedIndex = lbStrings.Items.Count - 1; }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            ulong oldGUID = Convert.ToUInt64(lbStrings.Items[currentIndex] + "", 16);
            if (map.ContainsKey(oldGUID))
            {
                map.Remove(oldGUID);
                lbStrings.Items.RemoveAt(currentIndex);
            }
        }

        private void btnHash_Click(object sender, EventArgs e)
        {
            ulong newGUID = System.Security.Cryptography.FNV64.GetHash(rtbValue.Text);
            if (map.ContainsKey(newGUID)) { tbGUID.Text = "0x" + newGUID.ToString("X16"); tbGUID.Focus(); tbGUID.SelectAll(); }
            else { map.Add(newGUID, rtbValue.Text); lbStrings.Items.Add("0x" + newGUID.ToString("X16")); lbStrings.SelectedIndex = lbStrings.Items.Count - 1; }
        }
    }
}
