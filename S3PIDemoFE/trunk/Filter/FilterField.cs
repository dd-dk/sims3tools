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
using s3pi.Interfaces;

namespace S3PIDemoFE.Filter
{
    public partial class FilterField : UserControl
    {
        TypedValue tvValue = null;
        TypedValue tvFilter = null;

        public FilterField()
        {
            InitializeComponent();
        }


        [Category("Appearance")]
        [Description("Indicate whether the filter field checkbox is checked")]
        public bool Checked { get { return ckbFilter.Checked; } set { ckbFilter.Checked = value; } }

        [Category("Appearance")]
        [Description("Value to filter for")]
        public TypedValue Filter
        {
            get { return tvFilter; }
            set
            {
                if (value == null) throw new ArgumentNullException();
                tvFilter = value;
                tbApplied.Text = (tvFilter.Value == null) ? "*" : tvFilter;
            }
        }

        [Category("Appearance")]
        [Description("Title for the field")]
        public string Title { get { return lbField.Text; } set { lbField.Text = value; } }

        [Category("Appearance")]
        [Description("Value being entered")]
        public TypedValue Value
        {
            get { return tvValue; }
            set
            {
                if (value == null) throw new ArgumentNullException();
                tvValue = value;
                tbEntry.Text = (tvValue.Value == null) ? "" : tvValue;
            }
        }

        /// <summary>
        /// Set Value from Filter
        /// </summary>
        [Description("Set Value from Filter")]
        public void Revise() { Value = Filter; }

        /// <summary>
        /// Set Filter from Value taking Checked into account
        /// </summary>
        [Description("Set Filter from Value taking Checked into account")]
        public void Set() { Filter = Checked ? Value : Filter = new TypedValue(tvFilter.Type, null, "X"); }



        private void tbEntry_Leave(object sender, EventArgs e)
        {
            try
            {
                if (tbEntry.Text.Length == 0) { tvValue = new TypedValue(tvValue.Type, null, "X"); return; }
                TypedValue tv = new TypedValue(tvValue.Type, Convert.ChangeType(Convert.ToUInt64(tbEntry.Text, tbEntry.Text.StartsWith("0x") ? 16 : 10), tvValue.Type), "X");
                tvValue = tv;
            }
            catch (System.FormatException) { Value = tvValue; tbApplied.SelectAll(); }
            catch (System.InvalidCastException) { Value = tvValue; tbApplied.SelectAll(); }
        }
    }
}
