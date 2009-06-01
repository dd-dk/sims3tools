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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace S3PIDemoFE
{
    public partial class ControlPanel : UserControl
    {
        public ControlPanel()
        {
            InitializeComponent();
        }

        #region Sort checkbox
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        [Description("The state of the Sort checkbox")]
        public bool Sort { get { return ckbSortable.Checked; } set { ckbSortable.Checked = value; } }

        [Browsable(true)]
        [Category("Property Changed")]
        [Description("Occurs when the Sort checkbox changes")]
        public event EventHandler SortChanged;
        protected virtual void OnSortChanged(object sender, EventArgs e) { if (SortChanged != null) SortChanged(sender, e); }
        private void ckbSortable_CheckedChanged(object sender, EventArgs e) { OnSortChanged(sender, e); }
        #endregion

        #region Hex button
        [Browsable(true)]
        [Category("Behavior")]
        [Description("The state of the Hex button")]
        public bool HexEnabled { get { return btnHex.Enabled; } set { btnHex.Enabled = value; } }

        [Browsable(true)]
        [Category("Action")]
        [DefaultValue(false)]
        [Description("Occurs when the Hex button is clicked")]
        public event EventHandler HexClick;
        protected virtual void OnHexClick(object sender, EventArgs e) { if (HexClick != null) HexClick(sender, e); }
        private void btnHex_Click(object sender, EventArgs e) { OnHexClick(sender, e); }
        #endregion

        #region AutoHex checkbox
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("The state of the AutoHex checkbox")]
        public bool AutoHex { get { return ckbAutoHex.Checked; } set { ckbAutoHex.Checked = value; } }

        [Browsable(true)]
        [Category("Property Changed")]
        [Description("Occurs when the AutoHex checkbox changes")]
        public event EventHandler AutoHexChanged;
        protected virtual void OnAutoHexChanged(object sender, EventArgs e) { if (AutoHexChanged != null) AutoHexChanged(sender, e); }
        private void ckbAutoHex_CheckedChanged(object sender, EventArgs e) { OnAutoHexChanged(sender, e); }
        #endregion

        #region HexOnly checkbox
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("The state of the HexOnly checkbox")]
        public bool HexOnly { get { return ckbNoUnWrap.Checked; } set { ckbNoUnWrap.Checked = value; } }

        [Browsable(true)]
        [Category("Property Changed")]
        [Description("Occurs when the HexOnly checkbox changes")]
        public event EventHandler HexOnlyChanged;
        protected virtual void OnHexOnlyChanged(object sender, EventArgs e) { if (HexOnlyChanged != null) HexOnlyChanged(sender, e); }
        private void ckbNoUnWrap_CheckedChanged(object sender, EventArgs e) { OnHexOnlyChanged(sender, e); }
        #endregion

        #region Unwrapped button
        [Browsable(true)]
        [Category("Behavior")]
        [Description("The state of the Unwrapped button")]
        public bool UnwrappedEnabled { get { return btnView.Enabled; } set { btnView.Enabled = value; } }

        [Browsable(true)]
        [Category("Action")]
        [DefaultValue(false)]
        [Description("Occurs when the Unwrapped button is clicked")]
        public event EventHandler UnwrappedClick;
        protected virtual void OnUnwrappedClick(object sender, EventArgs e) { if (UnwrappedClick != null) UnwrappedClick(sender, e); }
        private void btnView_Click(object sender, EventArgs e) { OnUnwrappedClick(sender, e); }
        #endregion

        #region Edit button
        [Browsable(true)]
        [Category("Behavior")]
        [Description("The state of the Edit button")]
        public bool EditEnabled { get { return btnEdit.Enabled; } set { btnEdit.Enabled = value; } }

        [Browsable(true)]
        [Category("Action")]
        [DefaultValue(false)]
        [Description("Occurs when the Edit button is clicked")]
        public event EventHandler EditClick;
        protected virtual void OnEditClick(object sender, EventArgs e) { if (EditClick != null) EditClick(sender, e); }
        private void btnEdit_Click(object sender, EventArgs e) { OnEditClick(sender, e); }
        #endregion

        #region UseNames checkbox
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("The state of the UseNames checkbox")]
        public bool UseNames { get { return ckbUseNames.Checked; } set { ckbUseNames.Checked = value; } }

        [Browsable(true)]
        [Category("Property Changed")]
        [Description("Occurs when the UseNames checkbox changes")]
        public event EventHandler UseNamesChanged;
        protected virtual void OnUseNamesChanged(object sender, EventArgs e) { if (UseNamesChanged != null) UseNamesChanged(sender, e); }
        private void ckbUseNames_CheckedChanged(object sender, EventArgs e) { OnUseNamesChanged(sender, e); }
        #endregion

        #region Commit button
        [Browsable(true)]
        [Category("Behavior")]
        [Description("The state of the Commit button")]
        public bool CommitEnabled { get { return btnCommit.Enabled; } set { btnCommit.Enabled = value; } }

        [Browsable(true)]
        [Category("Action")]
        [DefaultValue(false)]
        [Description("Occurs when the Commit button is clicked")]
        public event EventHandler CommitClick;
        protected virtual void OnCommitClick(object sender, EventArgs e) { if (CommitClick != null) CommitClick(sender, e); }
        private void btnCommit_Click(object sender, EventArgs e) { OnCommitClick(sender, e); }
        #endregion
    }
}