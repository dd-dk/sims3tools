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
using s3pi.Filetable;

namespace ObjectCloner.SplitterComponents
{
    public partial class ReplaceTGI : UserControl
    {
        Dictionary<CheckBox, Control> enableControl = new Dictionary<CheckBox, Control>();

        public ReplaceTGI()
        {
            InitializeComponent();

            cbFromResourceType.Value = 0;
            tbFromResourceGroup.Text = "0x00000000";
            tbFromInstance.Text = "0x0000000000000000";
            cbToResourceType.Value = 0;
            tbToResourceGroup.Text = "0x00000000";
            tbToInstance.Text = "0x0000000000000000";

            rtbResults.Text = "";
            btnReplace.Enabled = allowReplace();
            CriteriaEnabled = true;
            SaveEnabled = false;

            enableControl.Add(ckbAnyResourceType, cbFromResourceType);
            enableControl.Add(ckbAnyResourceGroup, tbFromResourceGroup);
            enableControl.Add(ckbAnyInstance, tbFromInstance);
            enableControl.Add(ckbKeepResourceType, cbToResourceType);
            enableControl.Add(ckbKeepResourceGroup, tbToResourceGroup);
            enableControl.Add(ckbKeepInstance, tbToInstance);
        }

        private void rtgiCopyRK_Click(object sender, EventArgs e)
        {
            ToolStripDropDownItem tsmi = sender as ToolStripDropDownItem;
            if (tsmi == null || tsmi.Owner as ContextMenuStrip == null) return;
            if ((tsmi.Owner as ContextMenuStrip).SourceControl == tlpFromTGIValues)
            {
                Clipboard.SetText(FromCriteria.ResourceKey + "");
            }
            else
            {
                Clipboard.SetText(ToCriteria.GetValueOrDefault(FromCriteria.ResourceKey) + "");
            }
        }

        private void rtgiPasteRK_Click(object sender, EventArgs e)
        {
            IResourceKey rk;
            if (RK.TryParse(Clipboard.GetText(), out rk))
            {
                ToolStripDropDownItem tsmi = sender as ToolStripDropDownItem;
                if (tsmi == null || tsmi.Owner as ContextMenuStrip == null) return;
                if ((tsmi.Owner as ContextMenuStrip).SourceControl == tlpFromTGIValues)
                {
                    cbFromResourceType.Value = rk.ResourceType;
                    tbFromResourceGroup.Text = "0x" + rk.ResourceGroup.ToString("X8");
                    tbFromInstance.Text = "0x" + rk.Instance.ToString("X16");
                }
                else
                {
                    cbToResourceType.Value = rk.ResourceType;
                    tbToResourceGroup.Text = "0x" + rk.ResourceGroup.ToString("X8");
                    tbToInstance.Text = "0x" + rk.Instance.ToString("X16");
                }
            }
        }

        private void rtContextMenu_Opening(object sender, CancelEventArgs e)
        {
            IResourceKey rk;
            rtgiPasteRK.Enabled = Clipboard.ContainsText() && RK.TryParse(Clipboard.GetText(), out rk);
        }

        private void tbResourceGroup_Validating(object sender, CancelEventArgs e)
        {
            TextBox tb = sender as TextBox;
            string s = tb.Text.ToLower();
            uint group;
            if (s.StartsWith("0x"))
            {
                if (!uint.TryParse(s.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out group)) e.Cancel = true;
            }
            else
            {
                if (!uint.TryParse(s, out group)) e.Cancel = true;
            }
            if (!e.Cancel) btnReplace.Enabled = allowReplace();
        }

        private void tbInstance_Validating(object sender, CancelEventArgs e)
        {
            TextBox tb = sender as TextBox;
            string s = tb.Text.ToLower();
            ulong instance;
            if (s.StartsWith("0x"))
            {
                if (!ulong.TryParse(s.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out instance)) e.Cancel = true;
            }
            else
            {
                if (!ulong.TryParse(s, out instance)) e.Cancel = true;
            }
            if (!e.Cancel) btnReplace.Enabled = allowReplace();
        }

        private void cbResourceType_ValueChanged(object sender, EventArgs e)
        {
            btnReplace.Enabled = allowReplace();
        }

        private void ckb_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox ckb = sender as CheckBox;
            enableControl[ckb].Enabled = !ckb.Checked;
            btnReplace.Enabled = allowReplace();
        }

        private void btnReplace_Click(object sender, EventArgs e) { if (ReplaceClicked != null) ReplaceClicked(this, EventArgs.Empty); }

        private void btnSave_Click(object sender, EventArgs e) { if (SaveClicked != null) SaveClicked(this, EventArgs.Empty); }

        private void btnCancel_Click(object sender, EventArgs e) { if (CancelClicked != null) CancelClicked(this, EventArgs.Empty); }

        uint uintParse(string value)
        {
            if (value.Substring(0, 2).ToLower().Equals("0x"))
            {
                return uint.Parse(value.Substring(2), System.Globalization.NumberStyles.HexNumber, null);
            }
            else
            {
                return uint.Parse(value);
            }
        }
        ulong ulongParse(string value)
        {
            if (value.Substring(0, 2).ToLower().Equals("0x"))
            {
                return ulong.Parse(value.Substring(2), System.Globalization.NumberStyles.HexNumber, null);
            }
            else
            {
                return ulong.Parse(value);
            }
        }

        bool allowReplace()
        {
            if (!criteriaEnabled) return false;

            Criteria c = FromCriteria;
            if (c.ResourceType == null && c.ResourceGroup == null && c.Instance == null) return false;
            c = ToCriteria;
            if (c.ResourceType == null && c.ResourceGroup == null && c.Instance == null) return false;

            AResourceKey from = new RK(FromCriteria.ResourceKey);
            AResourceKey to = new RK(ToCriteria.GetValueOrDefault(from));
            bool res = to.Equals(from);
            return !res;
        }

        public struct Criteria
        {
            public uint? ResourceType;
            public uint? ResourceGroup;
            public ulong? Instance;
            public Criteria(IResourceKey RK) { ResourceType = RK.ResourceType; ResourceGroup = RK.ResourceGroup; Instance = RK.Instance; }
            public IResourceKey ResourceKey { get { return GetValueOrDefault(0, 0, 0); } }
            public IResourceKey GetValueOrDefault(IResourceKey defaultRK)
            {
                return GetValueOrDefault(defaultRK.ResourceType, defaultRK.ResourceGroup, defaultRK.Instance);
            }
            public IResourceKey GetValueOrDefault(uint defaultResourceType, uint defaultResourceGroup, ulong defaultInstance)
            {
                return new TGIBlock(0, null,
                    ResourceType.GetValueOrDefault(defaultResourceType),
                    ResourceGroup.GetValueOrDefault(defaultResourceGroup),
                    Instance.GetValueOrDefault(defaultInstance));
            }
            public bool Match(IResourceKey RK)
            {
                if (ResourceType.HasValue && ResourceType.Value != RK.ResourceType) return false;
                if (ResourceGroup.HasValue && ResourceGroup.Value != RK.ResourceGroup) return false;
                if (Instance.HasValue && Instance.Value != RK.Instance) return false;
                return true;
            }
        }

        public event EventHandler ReplaceClicked;
        public event EventHandler SaveClicked;
        public event EventHandler CancelClicked;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The ResourceKey to search for, with optional wild cards.")]
        public Criteria FromCriteria
        {
            get
            {
                return new Criteria()
                {
                    ResourceType = ckbAnyResourceType.Checked ? null : (uint?)cbFromResourceType.Value,
                    ResourceGroup = ckbAnyResourceGroup.Checked ? null : (uint?)uintParse(tbFromResourceGroup.Text),
                    Instance = ckbAnyInstance.Checked ? null : (ulong?)ulongParse(tbFromInstance.Text)
                };
            }
            set
            {
                ckbAnyResourceType.Checked = !value.ResourceType.HasValue;
                cbFromResourceType.Value = value.ResourceType.GetValueOrDefault(0);
                ckbAnyResourceGroup.Checked = !value.ResourceGroup.HasValue;
                tbFromResourceGroup.Text = "0x" + value.ResourceGroup.GetValueOrDefault(0).ToString("X8");
                ckbAnyInstance.Checked = !value.Instance.HasValue;
                tbFromInstance.Text = "0x" + value.Instance.GetValueOrDefault(0).ToString("X16");
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The ResourceKey to use as a replacement, optionally keeping certain fields.")]
        public Criteria ToCriteria
        {
            get
            {
                return new Criteria()
                {
                    ResourceType = ckbKeepResourceType.Checked ? null : (uint?)cbToResourceType.Value,
                    ResourceGroup = ckbKeepResourceGroup.Checked ? null : (uint?)uintParse(tbToResourceGroup.Text),
                    Instance = ckbKeepInstance.Checked ? null : (ulong?)ulongParse(tbToInstance.Text)
                };
            }
            set
            {
                ckbKeepResourceType.Checked = !value.ResourceType.HasValue;
                cbToResourceType.Value = value.ResourceType.GetValueOrDefault(0);
                ckbKeepResourceGroup.Checked = !value.ResourceGroup.HasValue;
                tbToResourceGroup.Text = "0x" + value.ResourceGroup.GetValueOrDefault(0).ToString("X8");
                ckbKeepInstance.Checked = !value.Instance.HasValue;
                tbToInstance.Text = "0x" + value.Instance.GetValueOrDefault(0).ToString("X16");
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Description("Contains the text to display as the results of the replacement.")]
        public string Results
        {
            get { return rtbResults.Text; }
            set { rtbResults.Text = value; }
        }

        bool criteriaEnabled;
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue("")]
        [Description("When enabled, the user can enter search and replace criteria and the replace button is active.")]
        public bool CriteriaEnabled
        {
            get { return criteriaEnabled; }
            set { tlpFromTGIValues.Enabled = tlpToTGIValues.Enabled = criteriaEnabled = value; btnReplace.Enabled = allowReplace(); }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue("")]
        [Description("When enabled, cancel and save buttons are active.")]
        public bool SaveEnabled
        {
            get { return btnSave.Enabled; }
            set { btnSave.Enabled = value; }
        }
    }
}
