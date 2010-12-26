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
using System.Windows.Forms;
using System.IO;
using s3pi.Interfaces;

namespace S3PIDemoFE
{
    public partial class HexWidget : UserControl
    {
        int rowLength = 16;
        IResource res;

        public HexWidget()
        {
            InitializeComponent();
            this.richTextBox1.Font = new Font(FontFamily.GenericMonospace, 10 * (rowLength > 16 ? 0.85f : 1f));
            this.Enabled = false;
        }

        private void HexWidget_Load(object sender, EventArgs e)
        {
            HexWidget_LoadSettings();
        }

        void HexWidget_LoadSettings()
        {
            Rowsize = S3PIDemoFE.Properties.Settings.Default.HexRowsize;
        }
        public void HexWidget_SaveSettings(object sender, EventArgs e)
        {
            S3PIDemoFE.Properties.Settings.Default.HexRowsize = Rowsize;
        }

        public IResource Resource { get { return res; } set { if (res != value) { res = value; Refill(); } } }

        [DefaultValue(16)]
        public int Rowsize
        {
            get { return rowLength; }
            set
            {
                if (rowLength == value) return;
                rowLength = value;
                this.richTextBox1.Font = new Font(FontFamily.GenericMonospace, 10 * (rowLength > 16 ? 0.85f : 1f));
                Refill();
            }
        }

        private void Refill()
        {
            try
            {
                this.richTextBox1.Clear();
                if (res == null) return;

                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                this.Enabled = false;
                byte[] b = res.AsBytes;
                int padLength = 8;// +b.Length.ToString("X").Length;
                string rowFmt = "X" + padLength;

                sb.Append("".PadLeft(padLength + 2));
                for (int col = 0; col < rowLength; col++) sb.Append(col.ToString("X2") + " ");
                sb.AppendLine();
                sb.Append("".PadLeft(padLength + 2));
                for (int col = 0; col < rowLength; col++) sb.Append("---");
                sb.AppendLine();

                for (int row = 0; row < b.Length; row += rowLength)
                {
                    sb.Append(row.ToString(rowFmt) + ": ");

                    int col = 0;
                    for (; col < rowLength && row + col < b.Length; col++) sb.Append(b[row + col].ToString("X2") + " ");
                    for (; col < rowLength; col++) sb.Append("   ");

                    sb.Append(" : ");
                    for (col = 0; col < rowLength && row + col < b.Length; col++)
                        sb.Append(b[row + col] < 0x20 || b[row + col] > 0x7e ? '.' : (char)b[row + col]);

                    sb.AppendLine();
                }
                this.richTextBox1.Text = sb.ToString();
            }
            finally { this.Enabled = res != null; }
        }
    }
}
