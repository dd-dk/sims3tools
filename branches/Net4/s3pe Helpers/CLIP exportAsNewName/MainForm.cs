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
using System.Windows.Forms;
using System.IO;
using s3pi.Helpers;

namespace CLIPexportAsNewName
{
    public partial class MainForm : Form, IRunHelper
    {
        public MainForm()
        {
            InitializeComponent();
            tbClipName.Text = Program.CurrentName;
            tbClipName_TextChanged(null, null);
        }

        byte[] data = null;
        public MainForm(Stream s) : this() { data = new BinaryReader(s).ReadBytes((int)s.Length); }

        public byte[] Result { get { return null; } }

        private void tbClipName_TextChanged(object sender, EventArgs e) { tbIID.Text = string.Format("0x{0:X16}", IID(tbClipName.Text)); }

        private void btnCancel_Click(object sender, EventArgs e) { this.Close(); }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                folderBrowserDialog1.SelectedPath = CLIPexportAsNewName.Properties.Settings.Default.LastExportFolder;
                DialogResult dr = folderBrowserDialog1.ShowDialog();
                if (dr != System.Windows.Forms.DialogResult.OK) return;

                CLIPexportAsNewName.Properties.Settings.Default.LastExportFolder = folderBrowserDialog1.SelectedPath;
                CLIPexportAsNewName.Properties.Settings.Default.Save();

                string filename = Path.Combine(folderBrowserDialog1.SelectedPath, String.Format("S3_{0:X8}_{1:X8}_{2:X16}_{3}%%+CLIP.animation",
                    0x6B20C4F3, 0, IID(tbClipName.Text), tbClipName.Text));

                if (File.Exists(filename) && CopyableMessageBox.Show(String.Format("File '{0}' exists.\n\nReplace?", Path.GetFileName(filename)), "File exists",
                         CopyableMessageBoxButtons.YesNo, CopyableMessageBoxIcon.Question, 1, 1) != 0) return;

                using (BinaryWriter w = new BinaryWriter(new FileStream(filename, FileMode.Create)))
                {
                    w.Write(data);
                    w.Close();
                }

                Environment.ExitCode = 0;
            }
            finally { this.Close(); }
        }

        static char[] ao = new char[] { 'a', 'o', };
        ulong IID(string text)
        {
            ulong mask = 0;

            string[] split = tbClipName.Text.Split(new char[] { '_', }, 2);

            string value = tbClipName.Text;
            if (split.Length > 1)
            {
                if (split[0].Length == 1)
                {
                    if (ao.Contains<char>(split[0][0])) { }
                    else { value = string.Join("_", new string[] { "a", split[1], }); mask = (ulong)(0x8000 | Mask(split[0][0]) << 8); }
                }
                else if (split[0].Length == 3 && split[0][1] == '2')
                {
                    if (ao.Contains<char>(split[0][0]) && ao.Contains<char>(split[0][2])) { }
                    else
                    {
                        value = string.Join("_", new string[] { (split[0][0] == 'o' ? "o" : "a") + "2" + (split[0][2] == 'o' ? "o" : "a"), split[1], });
                        mask = (ulong)(0x8000 | Mask(split[0][0]) << 8 | Mask(split[0][2]));
                    }
                }
            }

            ulong iid = System.Security.Cryptography.FNV64.GetHash(value);
            iid &= 0x7FFFFFFFFFFFFFFF;
            iid ^= mask << 48;

            return iid;
        }

        byte Mask(char age)
        {
            switch (age)
            {
                case 'b': return 0x01;
                case 'p': return 0x02;
                case 'c': return 0x03;
                case 't': return 0x04;
                case 'h': return 0x05;
                case 'e': return 0x06;
                default: return 0x00;
            }
        }
    }

    static class Extensions
    {
        public static bool Contains<T>(this IEnumerable<T> haystack, T needle) { foreach (var x in haystack) if (needle.Equals(x)) return true; return false; }
    }
}
