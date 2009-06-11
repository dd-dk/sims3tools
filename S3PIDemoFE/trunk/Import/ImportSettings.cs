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
using System.IO;
using System.Windows.Forms;

namespace S3PIDemoFE.Import
{
    public partial class ImportSettings : FlowLayoutPanel
    {
        public ImportSettings()
        {
            InitializeComponent();
            ckbRename.Enabled = ckbUseName.Checked;
        }

        #region Properties
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool Overwrite { get { return ckbOverwrite.Checked; } set { ckbOverwrite.Checked = value; } }
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool Compress { get { return ckbCompress.Checked; } set { ckbCompress.Checked = value; } }
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool UseName { get { return ckbUseName.Checked; } set { ckbUseName.Checked = value; } }
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool AllowRename { get { return ckbRename.Checked; } set { ckbRename.Checked = value; } }
        #endregion

        #region Events
        [Category("Property Changed")]
        [Description("Raised when the Overwrite checkbox changes state")]
        public event EventHandler OverwriteCheckedChanged;

        [Category("Property Changed")]
        [Description("Raised when the Compress checkbox changes state")]
        public event EventHandler CompressCheckedChanged;

        [Category("Property Changed")]
        [Description("Raised when the UseName checkbox changes state")]
        public event EventHandler UseNameCheckedChanged;

        [Category("Property Changed")]
        [Description("Raised when the AllowRename checkbox changes state")]
        public event EventHandler AllowRenameCheckedChanged;
        #endregion

        protected virtual void OnOverwriteCheckedChanged(object sender, EventArgs e) { if (OverwriteCheckedChanged != null) OverwriteCheckedChanged(sender, e); }
        protected virtual void OnCompressCheckedChanged(object sender, EventArgs e) { if (CompressCheckedChanged != null) CompressCheckedChanged(sender, e); }
        protected virtual void OnUseNameCheckedChanged(object sender, EventArgs e) { if (UseNameCheckedChanged != null) UseNameCheckedChanged(sender, e); }
        protected virtual void OnAllowRenameCheckedChanged(object sender, EventArgs e) { if (AllowRenameCheckedChanged != null) AllowRenameCheckedChanged(sender, e); }

        private void ckbOverwrite_CheckedChanged(object sender, EventArgs e) { OnOverwriteCheckedChanged(sender, e); }
        private void ckbCompress_CheckedChanged(object sender, EventArgs e) { OnCompressCheckedChanged(sender, e); }
        private void ckbUseName_CheckedChanged(object sender, EventArgs e) { ckbRename.Enabled = ckbUseName.Checked; OnUseNameCheckedChanged(sender, e); }
        private void ckbRename_CheckedChanged(object sender, EventArgs e) { OnAllowRenameCheckedChanged(sender, e); }
    }
}
