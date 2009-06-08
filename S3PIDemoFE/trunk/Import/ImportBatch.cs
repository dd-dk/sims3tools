﻿/***************************************************************************
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
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace S3PIDemoFE
{
    public partial class ImportBatch : Form
    {
        private string[] batch = null;
        public ImportBatch()
        {
            InitializeComponent();
        }

        public ImportBatch(string[] fileDrop) : this() { addDrop(fileDrop); }

        public string[] Batch { get { return (string[])batch.Clone(); } }

        public bool Overwrite { get { return ckbOverwrite.Checked; } }

        public bool Compress { get { return ckbCompress.Checked; } }

        public bool UseNames { get { return ckbUseName.Checked; } }

        public bool Rename { get { return ckbRename.Checked; } }

        void addDrop(string[] fileDrop)
        {
            batch = (string[])fileDrop.Clone();
            lbFiles.Items.Clear();
            lbFiles.Items.AddRange(fileDrop);
        }

        private void ImportBatch_DragOver(object sender, DragEventArgs e)
        {
            if ((new List<string>(e.Data.GetFormats())).Contains("FileDrop"))
                e.Effect = DragDropEffects.Copy;
        }

        private void ImportBatch_DragDrop(object sender, DragEventArgs e)
        {
            string[] fileDrop = e.Data.GetData("FileDrop") as String[];
            if (fileDrop == null || fileDrop.Length == 0) return;
            addDrop(fileDrop);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
