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
        public CloneFixOptions()
        {
            InitializeComponent();
            ckbExcludeCatalogResources.Enabled = ckbFix.Checked;
            ckbFix.Checked = tlpCustomise.Enabled;
            tlpCustomise.Enabled = ckbCustomise.Checked;
            ckbThumbs.Enabled = ckbDefault.Enabled = ckbClone.Checked;
        }

        public CloneFixOptions(Form form, bool mustClone)
            : this()
        {
            if (mustClone)
            {
                ckbClone.Checked = true;
                ckbClone.Enabled = false;
            }
            form.AcceptButton = btnStart;
        }

        public string UniqueName { get { return tbUniqueName.Text; } set { tbUniqueName.Text = value; } }
        public bool IsClone { get { return ckbClone.Checked; } set { ckbClone.Checked = value; } }
        public bool IsDefaultOnly { get { return ckbDefault.Checked; } }
        public bool IsIncludeThumbnails { get { return ckbThumbs.Checked; } }
        public bool IsPadSTBLs { get { return ckbPadSTBLs.Checked; } }
        public bool IsFix { get { return ckbFix.Checked; } }
        public bool IsExcludeCatalogResources { get { return ckbExcludeCatalogResources.Checked; } }
        public bool IsCompress { get { return ckbCompress.Checked; } }

        public event EventHandler CancelClicked;
        public event EventHandler StartClicked;

        private void tbUniqueName_TextChanged(object sender, EventArgs e)
        {
            btnStart.Enabled = tbUniqueName.Text.Length > 0 && (ckbClone.Checked || ckbCustomise.Checked);
        }

        private void ckbClone_CheckedChanged(object sender, EventArgs e)
        {
            ckbThumbs.Enabled = ckbDefault.Enabled = ckbClone.Checked;
            ckbDefault.Checked = ckbClone.Checked;
            ckbThumbs.Checked = false;
            btnStart.Enabled = tbUniqueName.Text.Length > 0 && (ckbClone.Checked || ckbCustomise.Checked);
        }

        private void ckbCustomise_CheckedChanged(object sender, EventArgs e)
        {
            tlpCustomise.Enabled = ckbCustomise.Checked;
            btnStart.Enabled = tbUniqueName.Text.Length > 0 && (ckbClone.Checked || ckbCustomise.Checked);
        }

        private void tlpCustomise_EnabledChanged(object sender, EventArgs e)
        {
            ckbFix.Checked = tlpCustomise.Enabled;
        }

        private void ckbFix_CheckedChanged(object sender, EventArgs e)
        {
            ckbExcludeCatalogResources.Enabled = ckbFix.Checked;
            ckbExcludeCatalogResources.Checked = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (CancelClicked != null) CancelClicked(this, EventArgs.Empty);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (StartClicked != null) StartClicked(this, EventArgs.Empty);
        }
    }
}
