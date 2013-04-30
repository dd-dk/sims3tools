/***************************************************************************
 *  Copyright (C) 2010 by Peter L Jones                                    *
 *  pljones@users.sf.net                                                   *
 *                                                                         *
 *  This file is part of s3translate.                                      *
 *                                                                         *
 *  s3translate is free software: you can redistribute it and/or modify    *
 *  it under the terms of the GNU General Public License as published by   *
 *  the Free Software Foundation, either version 3 of the License, or      *
 *  (at your option) any later version.                                    *
 *                                                                         *
 *  s3translate is distributed in the hope that it will be useful,         *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of         *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the          *
 *  GNU General Public License for more details.                           *
 *                                                                         *
 *  You should have received a copy of the GNU General Public License      *
 *  along with s3translate.  If not, see <http://www.gnu.org/licenses/>.   *
 *                                                                         *
 *  s3translate was created by Jonathan Haas                               *
 ***************************************************************************/
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace S3Translate
{
    public partial class AddInstance : Form
    {
        public AddInstance()
        {
            InitializeComponent();
        }

        public AddInstance(ulong guid) : this()
        {
            tbInstance.Text = "0x" + guid.ToString("X16");
        }

        private void AddGUID_Shown(object sender, EventArgs e)
        {
            tbInstance.Focus();
            tbInstance.SelectAll();
        }

        private void btnFNV_Click(object sender, EventArgs e)
        {
            string toHash;
            if (textBox_StringToHash.Text.Trim().Length > 0)
            {
                toHash = textBox_StringToHash.Text;
            }
            else
            {
                toHash = Environment.UserName + DateTime.Now.ToBinary().ToString();
            }
            ulong hash = System.Security.Cryptography.FNV64.GetHash(toHash);
            tbInstance.Text = "0x" + hash.ToString("X16");
        }

        public ulong Instance
        {
            get
            {
                ulong res = 0;
                if (tbInstance.Text.ToLower().StartsWith("0x"))
                {
                    ulong.TryParse(tbInstance.Text.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out res);
                }
                else
                {
                    ulong.TryParse(tbInstance.Text, out res);
                }
                return res;
            }
        }
    }
}
