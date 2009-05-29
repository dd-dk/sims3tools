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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace S3PIDemoFE
{
    public partial class MenuBarWidget : UserControl
    {
        List<ToolStripMenuItem> tsMD, tsMB;

        public MenuBarWidget()
        {
            InitializeComponent();
            tsMD = new List<ToolStripMenuItem>(new ToolStripMenuItem[]{
                fileToolStripMenuItem, editToolStripMenuItem, resourceToolStripMenuItem, helpToolStripMenuItem,
            });
            tsMB = new List<ToolStripMenuItem>(new ToolStripMenuItem[] {
                //File
                newToolStripMenuItem, openToolStripMenuItem, saveToolStripMenuItem, saveAsToolStripMenuItem,
                saveCopyAsToolStripMenuItem, closeToolStripMenuItem, importToolStripMenuItem, exportToolStripMenuItem,
                exitToolStripMenuItem,
                //Edit
                cutToolStripMenuItem, copyToolStripMenuItem, pasteToolStripMenuItem,
                //Resource
                addToolStripMenuItem, detailsToolStripMenuItem, compressedToolStripMenuItem,
                //Help
                aboutToolStripMenuItem, warrantyToolStripMenuItem, licenceToolStripMenuItem,
            });
        }

        public enum MD
        {
            MBF,
            MBE,
            MBR,
            MBH,
        }

        public enum MB
        {
            MBF_new = 0, MBF_open, MBF_save, MBF_saveAs,
            MBF_saveCopyAs, MBF_close, MBF_import, MBF_export,
            MBF_exit,
            MBE_cut, MBE_copy, MBE_paste,
            MBR_add, MBR_details, MBR_compressed,
            MBH_about, MBH_warranty, MBH_licence,
        }

        public void Enable(MD mn, bool state) { tsMD[(int)mn].Enabled = state; }
        public void Enable(MB mn, bool state) { tsMB[(int)mn].Enabled = state; }
        public void Checked(MB mn, bool state) { tsMB[(int)mn].Checked = state; }
        public bool IsChecked(MB mn) { return tsMB[(int)mn].Checked; }

        public class MBDropDownOpeningEventArgs : EventArgs { public readonly MD mn; public MBDropDownOpeningEventArgs(MD mn) { this.mn = mn; } }
        public delegate void MBDropDownOpeningEventHandler(object sender, MBDropDownOpeningEventArgs mn);
        public event MBDropDownOpeningEventHandler MBDropDownOpening;
        protected void OnMBDropDownOpening(object sender, MD mn) { if (MBDropDownOpening != null) MBDropDownOpening(sender, new MBDropDownOpeningEventArgs(mn)); }
        private void tsMD_DropDownOpening(object sender, EventArgs e) { OnMBDropDownOpening(sender, (MD)tsMD.IndexOf(sender as ToolStripMenuItem)); }

        public class MBClickEventArgs : EventArgs { public readonly MB mn; public MBClickEventArgs(MB mn) { this.mn = mn; } }
        public delegate void MBClickEventHandler(object sender, MBClickEventArgs mn);

        public event MBClickEventHandler MBFile_Click;
        protected void OnMBFile_Click(object sender, MB mn) { if (MBFile_Click != null) MBFile_Click(sender, new MBClickEventArgs(mn)); }
        private void tsMBF_Click(object sender, EventArgs e) { OnMBFile_Click(sender, (MB)tsMB.IndexOf(sender as ToolStripMenuItem)); }

        public event MBClickEventHandler MBEdit_Click;
        protected void OnMBEdit_Click(object sender, MB mn) { if (MBEdit_Click != null) MBEdit_Click(sender, new MBClickEventArgs(mn)); }
        private void tsMBE_Click(object sender, EventArgs e) { OnMBEdit_Click(sender, (MB)tsMB.IndexOf(sender as ToolStripMenuItem)); }

        public event MBClickEventHandler MBResource_Click;
        protected void OnMBResource_Click(object sender, MB mn) { if (MBResource_Click != null) MBResource_Click(sender, new MBClickEventArgs(mn)); }
        private void tsMBR_Click(object sender, EventArgs e) { OnMBResource_Click(sender, (MB)tsMB.IndexOf(sender as ToolStripMenuItem)); }

        public event MBClickEventHandler MBHelp_Click;
        protected void OnMBHelp_Click(object sender, MB mn) { if (MBHelp_Click != null) MBHelp_Click(sender, new MBClickEventArgs(mn)); }
        private void tsMBH_Click(object sender, EventArgs e) { OnMBHelp_Click(sender, (MB)tsMB.IndexOf(sender as ToolStripMenuItem)); }
    }
}
