﻿/***************************************************************************
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

namespace ObjectCloner
{
    public partial class MenuBarWidget : MenuStrip
    {
        List<ToolStripMenuItem> tsMD, tsMB;

        public MenuBarWidget()
        {
            InitializeComponent();
            tsMD = new List<ToolStripMenuItem>(new ToolStripMenuItem[]{
                fileToolStripMenuItem, cloningToolStripMenuItem, viewToolStripMenuItem, settingsToolStripMenuItem, helpToolStripMenuItem,
            });
            tsMB = new List<ToolStripMenuItem>(new ToolStripMenuItem[] {
                //File
                openToolStripMenuItem, exitToolStripMenuItem,
                //Cloning -- These must be in order of enum CatalogType by value:
                cfenToolStripMenuItem, cstrToolStripMenuItem, cprxToolStripMenuItem, cttlToolStripMenuItem,
                cralToolStripMenuItem, ctptToolStripMenuItem, cfirToolStripMenuItem, cwatToolStripMenuItem,
                cfndToolStripMenuItem, objdToolStripMenuItem, cwalToolStripMenuItem, cwstToolStripMenuItem,
                crstToolStripMenuItem, mdlrToolStripMenuItem, crmtToolStripMenuItem,
                //View
                tilesToolStripMenuItem, largeIconsToolStripMenuItem, smallIconsToolStripMenuItem, listToolStripMenuItem, detailedListToolStripMenuItem,
                iconsToolStripMenuItem,
                //Tools
                searchToolStripMenuItem,
                //Settings
                gameFoldersToolStripMenuItem, userNameToopStripMenuItem,
                //Help
                contentsToolStripMenuItem, aboutToolStripMenuItem, warrantyToolStripMenuItem, licenceToolStripMenuItem,
            });
        }

        public enum MD
        {
            MBF,
            MBC,
            MBV,
            MBT,
            MBS,
            MBH,
        }

        public enum MB
        {
            MBF_open = 0, MBF_exit,
            MBC_cfen, MBC_cstr, MBC_cprx, MBC_cttl,
            MBC_cral, MBC_ctpt, MBC_cfir, MBC_cwat,
            MBC_cfnd, MBC_objd, MBC_cwal, MBC_cwst,
            MBC_crst, MBC_mdlr, MBC_crmt,
            MBV_tiles, MBV_largeIcons, MBV_smallIcons, MBV_list, MBV_detailedList,
            MBV_icons,
            MBT_search,
            MBS_sims3Folder, MBS_userName,
            MBH_contents, MBH_about, MBH_warranty, MBH_licence,
        }

        public void Enable(MD mn, bool state) { tsMD[(int)mn].Enabled = state; }
        public void Enable(MB mn, bool state) { tsMB[(int)mn].Enabled = state; }
        public void Checked(MB mn, bool state) { tsMB[(int)mn].Checked = state; tsMB[(int)mn].CheckState = state ? CheckState.Checked : CheckState.Unchecked; }
        public void Indeterminate(MB mn) { tsMB[(int)mn].CheckState = CheckState.Indeterminate; }
        public bool IsChecked(MB mn) { return tsMB[(int)mn].Checked; }

        public class MBDropDownOpeningEventArgs : EventArgs { public readonly MD mn; public MBDropDownOpeningEventArgs(MD mn) { this.mn = mn; } }
        public delegate void MBDropDownOpeningEventHandler(object sender, MBDropDownOpeningEventArgs mn);
        public event MBDropDownOpeningEventHandler MBDropDownOpening;
        protected void OnMBDropDownOpening(object sender, MD mn) { if (MBDropDownOpening != null) MBDropDownOpening(sender, new MBDropDownOpeningEventArgs(mn)); }
        private void tsMD_DropDownOpening(object sender, EventArgs e) { OnMBDropDownOpening(sender, (MD)tsMD.IndexOf(sender as ToolStripMenuItem)); }

        public class MBClickEventArgs : EventArgs { public readonly MB mn; public MBClickEventArgs(MB mn) { this.mn = mn; } }
        public delegate void MBClickEventHandler(object sender, MBClickEventArgs mn);

        [Category("Action")]
        public event MBClickEventHandler MBFile_Click;
        protected void OnMBFile_Click(object sender, MB mn) { if (MBFile_Click != null) MBFile_Click(sender, new MBClickEventArgs(mn)); }
        private void tsMBF_Click(object sender, EventArgs e) { OnMBFile_Click(sender, (MB)tsMB.IndexOf(sender as ToolStripMenuItem)); }

        [Category("Action")]
        public event MBClickEventHandler MBCloning_Click;
        protected void OnMBCloning_Click(object sender, MB mn) { if (MBCloning_Click != null) MBCloning_Click(sender, new MBClickEventArgs(mn)); }
        private void tsMBC_Click(object sender, EventArgs e) { OnMBCloning_Click(sender, (MB)tsMB.IndexOf(sender as ToolStripMenuItem)); }

        [Category("Action")]
        public event MBClickEventHandler MBView_Click;
        protected void OnMBView_Click(object sender, MB mn) { if (MBView_Click != null) MBView_Click(sender, new MBClickEventArgs(mn)); }
        private void tsMBV_Click(object sender, EventArgs e) { OnMBView_Click(sender, (MB)tsMB.IndexOf(sender as ToolStripMenuItem)); }

        [Category("Action")]
        public event MBClickEventHandler MBTools_Click;
        protected void OnMBTools_Click(object sender, MB mn) { if (MBTools_Click != null) MBTools_Click(sender, new MBClickEventArgs(mn)); }
        private void tsMBT_Click(object sender, EventArgs e) { OnMBTools_Click(sender, (MB)tsMB.IndexOf(sender as ToolStripMenuItem)); }

        [Category("Action")]
        public event MBClickEventHandler MBSettings_Click;
        protected void OnMBSettings_Click(object sender, MB mn) { if (MBSettings_Click != null) MBSettings_Click(sender, new MBClickEventArgs(mn)); }
        private void tsMBS_Click(object sender, EventArgs e) { OnMBSettings_Click(sender, (MB)tsMB.IndexOf(sender as ToolStripMenuItem)); }

        [Category("Action")]
        public event MBClickEventHandler MBHelp_Click;
        protected void OnMBHelp_Click(object sender, MB mn) { if (MBHelp_Click != null) MBHelp_Click(sender, new MBClickEventArgs(mn)); }
        private void tsMBH_Click(object sender, EventArgs e) { OnMBHelp_Click(sender, (MB)tsMB.IndexOf(sender as ToolStripMenuItem)); }
    }
}
