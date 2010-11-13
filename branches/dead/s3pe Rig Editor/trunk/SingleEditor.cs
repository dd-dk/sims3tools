/***************************************************************************
 *  Copyright (C) 2010 by Peter L Jones                                    *
 *  pljones@users.sf.net                                                   *
 *                                                                         *
 *  This helper uses Atavera's RIG Resource and is part of s3pi.           *
 *  Atavera: http://code.google.com/u/dd764ta/                             *
 *  RIG Resource: http://code.google.com/p/s3pi-wrappers/                  *
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
using System.Data;
using System.Windows.Forms;

namespace RigEditor
{
    public partial class SingleEditor : UserControl
    {
        Single f = 0f;

        public SingleEditor()
        {
            InitializeComponent();
        }

        [Browsable(true)]
        public override string Text { get { return lbSingle.Text; } set { lbSingle.Text = value; } }

        [Browsable(true)]
        [DefaultValue(0f)]
        public Single Value
        {
            get { return f; }
            set { f = value; tbSingle.Text = f.ToString("0.00000"); }
        }

        [Browsable(true)]
        public event EventHandler ValueChanged;
        protected void OnValueChanged() { if (ValueChanged != null) ValueChanged(this, EventArgs.Empty); }

        void tbSingle_Validating(object sender, CancelEventArgs e)
        {
            TextBox tb = sender as TextBox;
            string value = tb.Text.Trim();
            Single f = 0;
            e.Cancel = !Single.TryParse(value, out f);
            if (e.Cancel) tb.SelectAll();
        }

        void tbSingle_Validated(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            string value = tb.Text.Trim();
            Value = Single.Parse(value);
            OnValueChanged();
        }
    }
}
