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
using System.IO;
using System.Windows.Forms;

namespace S3PIDemoFE
{
    public partial class MenuBarWidget : UserControl
    {
        List<ToolStripMenuItem> tsMD, tsMB, cmsBW;

        public MenuBarWidget()
        {
            InitializeComponent();

            tsMD = new List<ToolStripMenuItem>(new ToolStripMenuItem[] {
                fileToolStripMenuItem, editToolStripMenuItem, resourceToolStripMenuItem, helpToolStripMenuItem,
            });
            tsMB = new List<ToolStripMenuItem>(new ToolStripMenuItem[] {
                //File
                newToolStripMenuItem, openToolStripMenuItem, saveToolStripMenuItem, saveAsToolStripMenuItem, saveCopyAsToolStripMenuItem, closeToolStripMenuItem,
                setMaxRecentToolStripMenuItem, bookmarkCurrentToolStripMenuItem, setMaxBookmarksToolStripMenuItem,
                exitToolStripMenuItem,
                //Edit
                editCutToolStripMenuItem, editCopyToolStripMenuItem, editPasteToolStripMenuItem,
                //Resource
                addToolStripMenuItem, resCopyToolStripMenuItem1, resPasteToolStripMenuItem1, duplicateToolStripMenuItem, replaceToolStripMenuItem,
                compressedToolStripMenuItem, deletedToolStripMenuItem, detailsToolStripMenuItem,
                fromFileToolStripMenuItem, fromPackageToolStripMenuItem, asDBCToolStripMenuItem, toFileToolStripMenuItem, toPackageToolStripMenuItem,
                //Tools
                fNVHashToolStripMenuItem, searchToolStripMenuItem,
                //Settings
                externalProgramsToolStripMenuItem, organiseBookmarksToolStripMenuItem,
                saveSettingsToolStripMenuItem,
                //Help
                contentsToolStripMenuItem, aboutToolStripMenuItem, warrantyToolStripMenuItem, licenceToolStripMenuItem,
            });
            cmsBW = new List<ToolStripMenuItem>(new ToolStripMenuItem[] {
                //BrowserWidgetContextMenuStrip
                bwcmAdd, bwcmCopy, bwcmPaste, bwcmDuplicate, bwcmReplace,
                bwcmCompressed, bwcmDeleted, bwcmDetails,
                bwcmFromFile, bwcmFromPackage, bwcmAsDBC, bwcmToFile, bwcmToPackage,
            });
            UpdateMRUList();
            UpdateBookmarks();
        }

        public enum MD
        {
            MBF,
            MBE,
            MBR,
            MBS,
            MBH,
        }

        public enum MB
        {
            MBF_new = 0, MBF_open, MBF_save, MBF_saveAs, MBF_saveCopyAs, MBF_close,
            MBF_setMaxRecent, MBF_bookmarkCurrent, MBF_setMaxBookmarks,
            MBF_exit,
            MBE_cut, MBE_copy, MBE_paste,
            MBR_add, MBR_copy, MBR_paste, MBR_duplicate, MBR_replace,
            MBR_compressed, MBR_isdeleted, MBR_details,
            MBR_importResources, MBR_importPackages, MBR_importAsDBC, MBR_exportResources, MBR_exportToPackage,
            MBT_fnvHash, MBT_search,
            MBS_externals, MBS_bookmarks,
            MBS_saveSettings,
            MBH_contents, MBH_about, MBH_warranty, MBH_licence,
        }

        public enum CMS
        {
            MBR_add = (int)MB.MBR_add, MBR_copy, MBR_paste, MBR_duplicate, MBR_replace,
            MBR_compressed, MBR_isdeleted, MBR_details,
            MBR_importResources, MBR_importPackages, MBR_importAsDBC, MBR_exportResources, MBR_exportToPackage,
        }

        bool isCMSBW(MB mn) { return (mn >= MB.MBR_add && mn < MB.MBT_fnvHash); }
        public void Enable(MD mn, bool state) { tsMD[(int)mn].Enabled = state; if (mn == MD.MBR) browserWidgetContextMenuStrip.Enabled = state; }
        public void Enable(MB mn, bool state) { tsMB[(int)mn].Enabled = state; if (isCMSBW(mn)) cmsBW[(int)mn - (int)CMS.MBR_add].Enabled = state; }
        public void Checked(MB mn, bool state)
        {
            tsMB[(int)mn].Checked = state;
            tsMB[(int)mn].CheckState = state ? CheckState.Checked : CheckState.Unchecked;
            if (isCMSBW(mn))
            {
                cmsBW[(int)mn - (int)CMS.MBR_add].Checked = state;
                cmsBW[(int)mn - (int)CMS.MBR_add].CheckState = state ? CheckState.Checked : CheckState.Unchecked;
            }
        }
        public void Indeterminate(MB mn) { tsMB[(int)mn].CheckState = CheckState.Indeterminate; if (isCMSBW(mn)) cmsBW[(int)mn - (int)CMS.MBR_add].CheckState = CheckState.Indeterminate; ; }
        public bool IsChecked(MB mn) { return tsMB[(int)mn].Checked; }

        public class MBDropDownOpeningEventArgs : EventArgs { public readonly MD mn; public MBDropDownOpeningEventArgs(MD mn) { this.mn = mn; } }
        public delegate void MBDropDownOpeningEventHandler(object sender, MBDropDownOpeningEventArgs mn);
        public event MBDropDownOpeningEventHandler MBDropDownOpening;
        protected void OnMBDropDownOpening(object sender, MD mn) { if (MBDropDownOpening != null) MBDropDownOpening(sender, new MBDropDownOpeningEventArgs(mn)); }
        private void tsMD_DropDownOpening(object sender, EventArgs e) { OnMBDropDownOpening(sender, (MD)tsMD.IndexOf(sender as ToolStripMenuItem)); }
        private void cmsBW_Opening(object sender, CancelEventArgs e) { OnMBDropDownOpening(sender, MD.MBR); }

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

        public event MBClickEventHandler MBTools_Click;
        protected void OnMBTools_Click(object sender, MB mn) { if (MBTools_Click != null) MBTools_Click(sender, new MBClickEventArgs(mn)); }
        private void tsMBT_Click(object sender, EventArgs e) { OnMBTools_Click(sender, (MB)tsMB.IndexOf(sender as ToolStripMenuItem)); }

        public event MBClickEventHandler MBSettings_Click;
        protected void OnMBSettings_Click(object sender, MB mn) { if (MBSettings_Click != null) MBSettings_Click(sender, new MBClickEventArgs(mn)); }
        private void tsMBS_Click(object sender, EventArgs e) { OnMBSettings_Click(sender, (MB)tsMB.IndexOf(sender as ToolStripMenuItem)); }

        public event MBClickEventHandler MBHelp_Click;
        protected void OnMBHelp_Click(object sender, MB mn) { if (MBHelp_Click != null) MBHelp_Click(sender, new MBClickEventArgs(mn)); }
        private void tsMBH_Click(object sender, EventArgs e) { OnMBHelp_Click(sender, (MB)tsMB.IndexOf(sender as ToolStripMenuItem)); }

        private void tsCMSBW_Click(object sender, EventArgs e) { OnMBResource_Click(sender, (MB)(cmsBW.IndexOf(sender as ToolStripMenuItem)) + (int)MB.MBR_add); }





        public void AddRecentFile(string value)
        {
            if (S3PIDemoFE.Properties.Settings.Default.MRUList == null)
                S3PIDemoFE.Properties.Settings.Default.MRUList = new System.Collections.Specialized.StringCollection();
            if (S3PIDemoFE.Properties.Settings.Default.MRUList.Contains(value))
                S3PIDemoFE.Properties.Settings.Default.MRUList.Remove(value);
            S3PIDemoFE.Properties.Settings.Default.MRUList.Insert(0, value);
            while (S3PIDemoFE.Properties.Settings.Default.MRUList.Count > S3PIDemoFE.Properties.Settings.Default.MRUListSize)
                S3PIDemoFE.Properties.Settings.Default.MRUList.RemoveAt(S3PIDemoFE.Properties.Settings.Default.MRUList.Count - 1);
            UpdateMRUList();
        }
        void UpdateMRUList()
        {
            this.mRUListToolStripMenuItem.DropDownItems.Clear();
            this.mRUListToolStripMenuItem.DropDownItems.Add(toolStripSeparator6);
            this.mRUListToolStripMenuItem.DropDownItems.Add(setMaxRecentToolStripMenuItem);

            int i = 0;
            if (S3PIDemoFE.Properties.Settings.Default.MRUList != null)
                foreach (string f in S3PIDemoFE.Properties.Settings.Default.MRUList)
                {
                    string s = makeName(f);

                    ToolStripMenuItem tsmiMRUListEntry = new ToolStripMenuItem();
                    tsmiMRUListEntry.Name = "tsmiRecent" + i;
                    tsmiMRUListEntry.ShortcutKeys = (Keys)(Keys.Control | ((Keys)(49 + i)));
                    tsmiMRUListEntry.Text = string.Format("&{0}. {1}", i + 1, s);
                    tsmiMRUListEntry.Click += new System.EventHandler(tsMRU_Click);
                    mRUListToolStripMenuItem.DropDownItems.Insert(i, tsmiMRUListEntry);
                    i++;
                }
        }

        public class MRUClickEventArgs : EventArgs { public readonly string filename; public MRUClickEventArgs(string filename) { this.filename = filename; } }
        public delegate void MRUClickEventHandler(object sender, MRUClickEventArgs filename);
        public event MRUClickEventHandler MRUClick;
        protected void OnMRUClick(object sender, int i) { if (MRUClick != null) MRUClick(sender, new MRUClickEventArgs(S3PIDemoFE.Properties.Settings.Default.MRUList[i])); }
        private void tsMRU_Click(object sender, EventArgs e) { OnMRUClick(sender, mRUListToolStripMenuItem.DropDownItems.IndexOf(sender as ToolStripMenuItem)); }

        public void AddBookmark(string value)
        {
            if (S3PIDemoFE.Properties.Settings.Default.Bookmarks == null)
                S3PIDemoFE.Properties.Settings.Default.Bookmarks = new System.Collections.Specialized.StringCollection();
            if (S3PIDemoFE.Properties.Settings.Default.Bookmarks.Contains(value))
                S3PIDemoFE.Properties.Settings.Default.Bookmarks.Remove(value);
            S3PIDemoFE.Properties.Settings.Default.Bookmarks.Insert(0, value);
            while (S3PIDemoFE.Properties.Settings.Default.Bookmarks.Count > S3PIDemoFE.Properties.Settings.Default.BookmarkSize)
                S3PIDemoFE.Properties.Settings.Default.Bookmarks.RemoveAt(S3PIDemoFE.Properties.Settings.Default.Bookmarks.Count - 1);
            UpdateBookmarks();
        }
        public void UpdateBookmarks()
        {
            this.bookmarkedPackagesToolStripMenuItem.DropDownItems.Clear();
            this.bookmarkedPackagesToolStripMenuItem.DropDownItems.Add(toolStripSeparator7);
            this.bookmarkedPackagesToolStripMenuItem.DropDownItems.Add(bookmarkCurrentToolStripMenuItem);
            this.bookmarkedPackagesToolStripMenuItem.DropDownItems.Add(setMaxBookmarksToolStripMenuItem);

            int i = 0;
            if (S3PIDemoFE.Properties.Settings.Default.Bookmarks != null)
                foreach (string f in S3PIDemoFE.Properties.Settings.Default.Bookmarks)
                {
                    string s = makeName(f);

                    ToolStripMenuItem tsmiBookmarkEntry = new ToolStripMenuItem();
                    tsmiBookmarkEntry.Name = "tsmiBookmark" + i;
                    tsmiBookmarkEntry.ShortcutKeys = (Keys)(Keys.Control | Keys.Shift | ((Keys)(49 + i)));
                    tsmiBookmarkEntry.Text = string.Format("&{0}. {1}", i + 1, s);
                    tsmiBookmarkEntry.Click += new System.EventHandler(tsBookmark_Click);
                    bookmarkedPackagesToolStripMenuItem.DropDownItems.Insert(i, tsmiBookmarkEntry);
                    i++;
                }
        }

        string makeName(string savedName)
        {
            bool readWrite;
            if (savedName.StartsWith("0:")) { savedName = savedName.Substring(2); readWrite = false; }
            else if (savedName.StartsWith("1:")) { savedName = savedName.Substring(2); readWrite = true; }
            else readWrite = true;
            if (savedName.Length > 100)
                savedName = "..." + savedName.Substring(Math.Max(0, savedName.Length - Math.Max(100, Path.GetFileName(savedName).Length)));
            return (readWrite ? "[RW]" : "[RO]") + ": " + savedName;
        }

        public class BookmarkClickEventArgs : EventArgs { public readonly string filename; public BookmarkClickEventArgs(string filename) { this.filename = filename; } }
        public delegate void BookmarkClickEventHandler(object sender, BookmarkClickEventArgs filename);
        public event BookmarkClickEventHandler BookmarkClick;
        protected void OnBookmarkClick(object sender, int i) { if (BookmarkClick != null) BookmarkClick(sender, new BookmarkClickEventArgs(S3PIDemoFE.Properties.Settings.Default.Bookmarks[i])); }
        private void tsBookmark_Click(object sender, EventArgs e) { OnBookmarkClick(sender, bookmarkedPackagesToolStripMenuItem.DropDownItems.IndexOf(sender as ToolStripMenuItem)); }
    }
}
