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

namespace ObjectCloner.TopPanelComponents
{
    public partial class CloneFixOptions : UserControl
    {
        bool allow32bitIIDs = false;

        public CloneFixOptions()
        {
            InitializeComponent();
        }

        public CloneFixOptions(Form form, bool mustClone, bool allow32bitIIDs, bool shouldPad)
            : this()
        {
            if (mustClone)
            {
                ckbClone.Enabled = false;
                ckbClone.Checked = true;
            }
            ckbClone_CheckedChanged(null, EventArgs.Empty);

            ckbRenumber.Checked = mustClone;
            this.allow32bitIIDs = allow32bitIIDs;
            ckbRenumber_CheckedChanged(null, EventArgs.Empty);

            ckbPadSTBLs.Checked = shouldPad;

            form.AcceptButton = btnStart;
            form.CancelButton = btnCancel;
        }

        public string UniqueName { get { return tbUniqueName.Text; } set { tbUniqueName.Text = value; } }
        public bool IsClone { get { return ckbClone.Checked; } }
        public bool IsDeepClone { get { return ckbDeepClone.Checked; } }
        //public bool IsExcludeCommon { get { return ckbExclCommon.Checked; } }
        //public bool IsIncludePresets { get { return ckbIncludePresets.Checked; } }
        public bool IsPadSTBLs { get { return ckbPadSTBLs.Checked; } }
        public bool IsIncludeThumbnails { get { return ckbThumbs.Checked; } }
        public bool IsRenumber { get { return ckbRenumber.Checked; } }
        public bool Is32bitIIDs { get { return ckb32bitIIDs.Checked; } }
        public bool IsKeepSTBLIIDs { get { return ckbKeepSTBLIIDs.Checked; } }
        public bool IsCompress { get { return ckbCompress.Checked; } }

        public event EventHandler CancelClicked;
        public event EventHandler StartClicked;

        private void ckbClone_CheckedChanged(object sender, EventArgs e)
        {
            ckbDeepClone.Enabled = !ckbClone.Enabled && ckbClone.Checked;
            if (!ckbDeepClone.Enabled) ckbDeepClone.Checked = false;
        }

        private void ckbRenumber_CheckedChanged(object sender, EventArgs e)
        {
            tbUniqueName.Enabled = ckbRenumber.Checked;
            ckb32bitIIDs.Enabled = ckbRenumber.Checked && allow32bitIIDs;
            ckb32bitIIDs.Checked = false;
            ckbKeepSTBLIIDs.Enabled = !ckbRenumber.Checked;
            ckbKeepSTBLIIDs.Checked = false;
        }

        private void btnCancel_Click(object sender, EventArgs e) { if (CancelClicked != null) CancelClicked(this, EventArgs.Empty); }

        private void btnStart_Click(object sender, EventArgs e) { if (StartClicked != null) StartClicked(this, EventArgs.Empty); }
    }
}
