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
using s3pi.Interfaces;

namespace S3PIDemoFE.Filter
{
    public partial class FilterField : UserControl
    {
        TypedValue tvValue = null;
        TypedValue tvFilter = null;
        bool internalchg = false;

        public FilterField()
        {
            InitializeComponent();
            tbFilter.Enabled = ckbFilter.Checked;
        }


        [Category("Behavior")]
        [Description("Title for the field")]
        public string Title { get { return lbField.Text; } set { lbField.Text = value; } }

        [Category("Behavior")]
        [Description("Currently selected row value for the field")]
        public TypedValue Value { get { return tvValue; } set { tvValue = value; if (tvValue != null) tbValue.Text = tvValue; } }

        [Category("Behavior")]
        [Description("Whether to field on the field")]
        public bool EnableFilter { get { return ckbFilter.Checked; } set { ckbFilter.Checked = value; } }

        [Category("Behavior")]
        [Description("Value to filter for")]
        public TypedValue Filter { get { return tvFilter; } set { tvFilter = value; internalchg = true; if (tvFilter != null) tbFilter.Text = tvFilter; internalchg = false;} }


        [Description("Raised when the value of the filter changes")]
        public event EventHandler FilterChanged;

        
        protected virtual void OnFilterChanged(object sender, EventArgs e) { if (FilterChanged != null) FilterChanged(sender, e); }

        
        private void ckbFilter_CheckedChanged(object sender, EventArgs e)
        {
            tbFilter.Enabled = ckbFilter.Checked;
            internalchg = true;
            if (tbFilter.Enabled)
            {
                tvFilter = new TypedValue(tvValue.Type, tvValue.Value, "X");
                tbFilter.Text = tbValue.Text;
            }
            else
            {
                tbFilter.Text = "";
            }
            internalchg = false;
            OnFilterChanged(this, new EventArgs());
        }

        private void tbFilter_TextChanged(object sender, EventArgs e)
        {
            if (internalchg) return;
            try
            {
                if (tvValue.Type.IsPrimitive)
                {
                    //Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, IntPtr, UIntPtr, Char, Double, and Single
                    if (tbFilter.Text.ToLower().StartsWith("0x"))
                        tvFilter = new TypedValue(tvValue.Type, Convert.ChangeType(UInt64.Parse(tbFilter.Text.Substring(2), System.Globalization.NumberStyles.HexNumber), tvValue.Type), "X");
                    else
                        tvFilter = new TypedValue(tvValue.Type, Convert.ChangeType(Int64.Parse(tbFilter.Text, System.Globalization.NumberStyles.Number), tvValue.Type), "X");
                }
                /*else if (tvValue.Type.IsArray)
                {
                }
                else if (tvValue.Type.IsClass)
                {
                    tvFilter = new TypedValue(tvValue.Type, Convert.ChangeType(tbFilter.Text, tvValue.Type));
                }/**/
                else if (tvValue.Type.IsEnum)
                {
                    tvFilter = new TypedValue(tvValue.Type, Enum.Parse(tvValue.Type, tbFilter.Text), "X");
                }
                /*else if (tvValue.Type.IsValueType)
                {
                }/**/
                else
                    tvFilter = new TypedValue(tvValue.Type, Convert.ChangeType(tbFilter.Text, tvValue.Type), "X");
            }
            catch
            {
                tvFilter = new TypedValue(tvValue.Type, "", "X");
                tbFilter.Text = "";
            }
            OnFilterChanged(this, new EventArgs());
        }
    }
}
