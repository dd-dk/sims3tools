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
            ControlPanel_LoadSettings();
        }

        void ControlPanel_LoadSettings()
        {
            Sort = S3PIDemoFE.Properties.Settings.Default.Sort;
            AutoHex = S3PIDemoFE.Properties.Settings.Default.AutoHex;
            HexOnly = S3PIDemoFE.Properties.Settings.Default.HexOnly;
            UseNames = S3PIDemoFE.Properties.Settings.Default.HexOnly;
        }
        public void ControlPanel_SaveSettings(object sender, EventArgs e)
        {
            S3PIDemoFE.Properties.Settings.Default.Sort = Sort;
            S3PIDemoFE.Properties.Settings.Default.AutoHex = AutoHex;
            S3PIDemoFE.Properties.Settings.Default.HexOnly = HexOnly;
            S3PIDemoFE.Properties.Settings.Default.HexOnly = UseNames;
        }

        #region Sort checkbox
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
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
        [DefaultValue(false)]
        [Description("The state of the Hex button")]
        public bool HexEnabled { get { return btnHex.Enabled; } set { btnHex.Enabled = value; } }

        [Browsable(true)]
        [Category("Action")]
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

        #region Value button
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("The state of the Value button")]
        public bool ValueEnabled { get { return btnValue.Enabled; } set { btnValue.Enabled = value; } }

        [Browsable(true)]
        [Category("Action")]
        [Description("Occurs when the Value button is clicked")]
        public event EventHandler ValueClick;
        protected virtual void OnValueClick(object sender, EventArgs e) { if (ValueClick != null) ValueClick(sender, e); }
        private void btnValue_Click(object sender, EventArgs e) { OnValueClick(sender, e); }
        #endregion

        #region Grid button
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("The state of the Grid button")]
        public bool GridEnabled { get { return btnGrid.Enabled; } set { btnGrid.Enabled = value; } }

        [Browsable(true)]
        [Category("Action")]
        [Description("Occurs when the Grid button is clicked")]
        public event EventHandler GridClick;
        protected virtual void OnGridClick(object sender, EventArgs e) { if (GridClick != null) GridClick(sender, e); }
        private void btnGrid_Click(object sender, EventArgs e) { OnGridClick(sender, e); }
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

        #region Viewer button
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("The state of the Viewer button")]
        public bool ViewerEnabled { get { return btnViewer.Enabled; } set { btnViewer.Enabled = value; } }

        [Browsable(true)]
        [Category("Action")]
        [Description("Occurs when the Viewer button is clicked")]
        public event EventHandler ViewerClick;
        protected virtual void OnViewerClick(object sender, EventArgs e) { if (ViewerClick != null) ViewerClick(sender, e); }
        private void btnViewer_Click(object sender, EventArgs e) { OnViewerClick(sender, e); }
        #endregion

        #region Editor button
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("The state of the Editor button")]
        public bool EditorEnabled { get { return btnEditor.Enabled; } set { btnEditor.Enabled = value; } }

        [Browsable(true)]
        [Category("Action")]
        [Description("Occurs when the Editor button is clicked")]
        public event EventHandler EditorClick;
        protected virtual void OnEditorClick(object sender, EventArgs e) { if (EditorClick != null) EditorClick(sender, e); }
        private void btnEditor_Click(object sender, EventArgs e) { OnEditorClick(sender, e); }
        #endregion

        #region Commit button
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("The state of the Commit button")]
        public bool CommitEnabled { get { return btnCommit.Enabled; } set { btnCommit.Enabled = value; } }

        [Browsable(true)]
        [Category("Action")]
        [Description("Occurs when the Commit button is clicked")]
        public event EventHandler CommitClick;
        protected virtual void OnCommitClick(object sender, EventArgs e) { if (CommitClick != null) CommitClick(sender, e); }
        private void btnCommit_Click(object sender, EventArgs e) { OnCommitClick(sender, e); }
        #endregion
    }
}