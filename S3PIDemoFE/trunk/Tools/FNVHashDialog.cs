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
using System.Windows.Forms;

namespace S3PIDemoFE.Tools
{
    // From the Sims3 wiki http://www.sims2wiki.info/wiki.php?title=FNV
    public partial class FNVHashDialog : Form
    {
        public FNVHashDialog()
        {
            InitializeComponent();
        }

        byte[] fromString(string text)
        {
            byte[] res = new byte[text.Length];
            for (int i = 0; i < text.Length; i++) res[i] = (byte)text[i];
            return res;
        }
        private UInt32 hash32(string text) { return (uint)fnv(0x01000193, 0x811C9DC5, fromString(text.ToLower())); }
        private UInt64 hash64(string text) { return fnv(0x00000100000001B3, 0xCBF29CE484222325, fromString(text.ToLower())); ; }

        private UInt64 fnv(ulong prime, ulong offset, byte[] data)
        {
            ulong hash = offset;
            foreach (byte b in data)
            {
                hash *= prime;
                hash ^= b;
            }
            return hash;
        }

        private void btnCalc_Click(object sender, EventArgs e)
        {
            tbFNV32.Text = "0x" + hash32(tbInput.Text).ToString("X8");
            tbFNV64.Text = "0x" + hash64(tbInput.Text).ToString("X16");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
