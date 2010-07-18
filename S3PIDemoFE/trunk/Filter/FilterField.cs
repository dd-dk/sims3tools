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
using System.Text.RegularExpressions;
using System.Windows.Forms;
using s3pi.Interfaces;

namespace S3PIDemoFE.Filter
{
    public partial class FilterField : UserControl
    {
        Regex rxFilter = null;
        Regex rxValue = null;

        public FilterField()
        {
            InitializeComponent();
        }

        [Category("Appearance")]
        [Description("Indicate whether the filter field checkbox is checked")]
        public bool Checked { get { return ckbFilter.Checked; } set { ckbFilter.Checked = value; } }

        [Category("Appearance")]
        [Description("Value to filter for")]
        public Regex Filter
        {
            get { return rxFilter; }
            set
            {
                if (value == null) throw new ArgumentNullException();
                rxFilter = value;
                tbApplied.Text = (rxFilter == null) ? "*" : rxFilter.ToString().TrimStart('^').TrimEnd('$');
                if (tbApplied.Text == ".*") tbApplied.Text = "*";
            }
        }

        [Category("Appearance")]
        [Description("Title for the field")]
        public string Title { get { return lbField.Text; } set { lbField.Text = value; } }

        [Category("Appearance")]
        [Description("Value being entered")]
        public Regex Value
        {
            get { return rxValue; }
            set
            {
                if (value == null) throw new ArgumentNullException();
                rxValue = value;
                tbEntry.Text = (rxValue == null) ? "" : rxValue.ToString().TrimStart('^').TrimEnd('$');
                if (tbEntry.Text == ".*") tbEntry.Text = "";
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
        public void Set() { Filter = Checked ? Value : new Regex(".*"); }



        private void tbEntry_Leave(object sender, EventArgs e)
        {
            try
            {
                if (tbEntry.Text.Length == 0) { rxValue = new Regex(""); return; }
                Value = new Regex("^" + tbEntry.Text.TrimStart('^').TrimEnd('$') + "$", RegexOptions.IgnoreCase);
            }
            catch (System.ArgumentException) { Value = rxValue; tbEntry.SelectAll(); }
        }
    }
}
