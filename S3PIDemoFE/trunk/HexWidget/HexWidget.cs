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
using System.Windows.Forms;
using System.IO;
using s3pi.Interfaces;

namespace S3PIDemoFE
{
    public partial class HexWidget : UserControl
    {
        DataGridViewCellStyle asHexStyle = new DataGridViewCellStyle();
        DataGridViewCellStyle asFixedStyle = new DataGridViewCellStyle();
        Stream stream = null;
        public HexWidget()
        {
            Font fixedwidth = new Font("DejaVu Sans Mono", 8);
            if (fixedwidth == null) fixedwidth = new Font("Lucida Console", 8);

            asHexStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            asHexStyle.Font = fixedwidth;
            asHexStyle.Padding = new Padding(0);

            asFixedStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            asFixedStyle.Font = fixedwidth;
            asFixedStyle.Padding = new Padding(0);

            InitializeComponent();

            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.DefaultCellStyle = asHexStyle;
            dataGridView1.ColumnCount = 18;
            SetHeaders();
        }

        [Browsable(true)]
        public Stream Stream
        {
            get { return stream; }
            set
            {
                if (stream == value) return;
                stream = value;
                Refill();
            }
        }

        [Browsable(true)]
        [DefaultValue(16)]
        public int Rowsize
        {
            get { return dataGridView1.ColumnCount - 2; }
            set
            {
                if (value == Rowsize) return;

                dataGridView1.ColumnCount = value + 2;
                SetHeaders();
                Refill();
            }
        }

        bool filling = false;
        void SetHeaders()
        {
            filling = true;
            try
            {
                dataGridView1.Columns[0].Name = "offset";
                dataGridView1.Columns[0].ValueType = typeof(Int32);
                dataGridView1.Columns[0].HeaderText = "Offset";
                dataGridView1.Columns[0].ReadOnly = true;
                dataGridView1.Columns[0].Width = 0x47;

                for (int i = 1; i < dataGridView1.ColumnCount - 1; i++)
                {
                    dataGridView1.Columns[i].Name = "col" + (i - 1);
                    dataGridView1.Columns[i].ValueType = typeof(byte);
                    dataGridView1.Columns[i].HeaderText = ((byte)i - 1).ToString("X").PadLeft(2, '0');
                    dataGridView1.Columns[i].Width = 0x18;
                }

                dataGridView1.Columns[dataGridView1.ColumnCount - 1].Name = "text";
                dataGridView1.Columns[dataGridView1.ColumnCount - 1].ValueType = typeof(string);
                dataGridView1.Columns[dataGridView1.ColumnCount - 1].HeaderText = "As text";
                dataGridView1.Columns[dataGridView1.ColumnCount - 1].ReadOnly = true;
                dataGridView1.Columns[dataGridView1.ColumnCount - 1].DefaultCellStyle = asFixedStyle;
                dataGridView1.Columns[dataGridView1.ColumnCount - 1].Width = 0xe8;
                dataGridView1.Columns[dataGridView1.ColumnCount - 1].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            finally { filling = false; }
        }

        private void Refill()
        {
            filling = true;
            try
            {
                dataGridView1.Rows.Clear();
                if (stream == null) return;
                stream.Position = 0;

                BinaryReader r = new BinaryReader(stream);
                for (int oset = 0; oset < stream.Length; )
                {
                    List<object> lrow = new List<object>();

                    lrow.Add(oset.ToString("X").PadLeft(8, '0'));

                    byte[] bytes = r.ReadBytes((int)Math.Min(Rowsize, stream.Length - oset));
                    foreach (byte b in bytes) lrow.Add(b.ToString("X").PadLeft(2, '0'));
                    while (lrow.Count < dataGridView1.ColumnCount - 1) lrow.Add("");

                    string text = "";
                    for (int i = 0; i < bytes.Length; i++) text += (bytes[i] >= 32 && bytes[i] < 127) ? ((char)bytes[i]).ToString() : ".";
                    lrow.Add(text);

                    dataGridView1.Rows.Add(lrow.ToArray());

                    oset += bytes.Length;
                }
            }
            finally { filling = false; }
        }

        public event EventHandler HexChanged;
        [Browsable(true)]
        [Description("Raised when a value in the byte stream is changed.")]
        protected virtual void OnHexChanged(object sender, EventArgs e) { if (HexChanged != null) HexChanged(sender, e); }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (filling) return;
            if (e.ColumnIndex == 0 || e.ColumnIndex == Rowsize - 1) return;
            stream.Seek(e.ColumnIndex - 1 + e.RowIndex * Rowsize, SeekOrigin.Begin);
            long p = stream.Position;
            byte b = (new BinaryReader(stream)).ReadByte(), newb = Convert.ToByte(dataGridView1[e.ColumnIndex, e.RowIndex].FormattedValue as string, 16);
            if (b != newb)
            {
                stream.Position = p;
                stream.WriteByte(newb);
                OnHexChanged(this, new EventArgs());
            }
        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (filling) return;
            if (e.ColumnIndex == 0 || e.ColumnIndex == dataGridView1.ColumnCount - 1 || e.ColumnIndex - 1 + this.Rowsize * e.RowIndex >= stream.Length) return;
            try { Convert.ToByte(e.FormattedValue as string, 16); }
            catch { e.Cancel = true; }
        }
    }
}
