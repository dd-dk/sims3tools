/***************************************************************************
 *  Copyright (C) 2010 by Peter L Jones                                    *
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

namespace ObjectCloner.CustomControls
{
    public partial class EnumTextBox : UserControl
    {
        Type enumType;
        ulong currentValue = 0;
        string fmt = "X";
        List<string> validNames = null;

        public EnumTextBox()
        {
            InitializeComponent();
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Type EnumType
        {
            get { return enumType; }
            set
            {
                if (enumType == value) return;

                enumType = value;
                if (enumType == null)
                {
                    fmt = "X";
                    validNames = null;
                    label1.Text = "";
                }
                else
                {
                    fmt = Enum.GetUnderlyingType(enumType).Equals(typeof(UInt64)) ? "X16" : "X8";
                    validNames = new List<string>(Enum.GetNames(enumType));

                    int len = 2 + (Enum.GetUnderlyingType(enumType).Equals(typeof(UInt64)) ? 16 : 8);
                    Label x = new Label();
                    x.AutoSize = true;
                    x.Text = "".PadLeft(len, 'X');
                    textBox1.Size = new Size(x.PreferredWidth + 6, x.PreferredHeight + 6);

                    string val = Enum.ToObject(enumType, currentValue).ToString();
                    ulong tryVal;
                    label1.Text = UInt64.TryParse(val, out tryVal) ? "" : val;
                }
                textBox1.Text = "0x" + currentValue.ToString(fmt);
            }
        }

        [Browsable(true)]
        [DefaultValue(false)]
        public bool ReadOnly { get { return textBox1.ReadOnly; } set { textBox1.ReadOnly = value; } }

        [Browsable(true)]
        [DefaultValue((byte)0)]
        public UInt64 Value
        {
            get { return currentValue; }
            set
            {
                if (currentValue == value) return;

                textBox1.Text = "0x" + value.ToString(fmt);

                if (enumType == null) label1.Text = "";
                else
                {
                    string val = Enum.ToObject(enumType, value).ToString();
                    ulong tryVal;
                    label1.Text = UInt64.TryParse(val, out tryVal) ? "" : val;
                }

                currentValue = value;
                OnValueChanged(this, EventArgs.Empty);
            }
        }

        public void Clear()
        {
            enumType = null;
            currentValue = 0;
            fmt = "X";
            validNames = null;
            label1.Text = "";
            textBox1.Text = "0x" + currentValue.ToString(fmt);
        }

        private void textBox1_Validating(object sender, CancelEventArgs e)
        {
            if (validNames.Contains(textBox1.Text)) return;
            UInt64 val;
            e.Cancel = !(textBox1.Text.StartsWith("0x")
                ? UInt64.TryParse(textBox1.Text.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out val)
                : UInt64.TryParse(textBox1.Text, out val)
            );

            if (e.Cancel) textBox1.SelectAll();
        }

        private void textBox1_Validated(object sender, EventArgs e)
        {
            Value = Get_Value(textBox1.Text);
        }

        ulong Get_Value(string value)
        {
            if (validNames.Contains(textBox1.Text))
                return Convert.ToUInt64(Enum.Parse(enumType, textBox1.Text));

            return value.StartsWith("0x")
                ? UInt64.Parse(value.Substring(2), System.Globalization.NumberStyles.HexNumber)
                : UInt64.Parse(value);
        }

        public event EventHandler ValueChanged;
        protected void OnValueChanged(object sender, EventArgs e) { if (ValueChanged != null) ValueChanged(sender, e); }
    }
}
