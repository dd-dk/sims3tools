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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using s3pi.Interfaces;

namespace ObjectCloner.SplitterComponents
{
    public partial class FixIntegrityResults : UserControl
    {
        public FixIntegrityResults()
        {
            InitializeComponent();
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Description("Contains the text to display.")]
        public new string Text
        {
            get { return richTextBox1.Text; }
            set { richTextBox1.Text = value; }
        }

        public event EventHandler SaveClicked;
        public event EventHandler CancelClicked;

        private void btnSave_Click(object sender, EventArgs e) { if (SaveClicked != null) SaveClicked(this, EventArgs.Empty); }

        private void btnCancel_Click(object sender, EventArgs e) { if (CancelClicked != null) CancelClicked(this, EventArgs.Empty); }
    }
}
