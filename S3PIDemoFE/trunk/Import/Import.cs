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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using s3pi.Interfaces;
using s3pi.Package;
using s3pi.Extensions;

namespace S3PIDemoFE
{
    partial class MainForm
    {
        private void fileImport()
        {
            DialogResult dr = fileImportDialog.ShowDialog();
            if (dr != DialogResult.OK) return;

            if (fileImportDialog.FileNames.Length > 1)
                importBatch(fileImportDialog.FileNames);
            else
                importSingle(fileImportDialog.FileName);
        }

        private void browserWidget1_DragDrop(object sender, DragEventArgs e)
        {
            string[] fileDrop = e.Data.GetData("FileDrop") as String[];
            if (fileDrop == null || fileDrop.Length == 0) return;

            Application.DoEvents();
            if (fileDrop.Length > 1)
                importBatch(fileDrop);
            else
                importSingle(fileDrop[0]);
        }

        void importSingle(string filename)
        {
            ResourceDetails ir = new ResourceDetails(CurrentPackage.Find(new string[] { "ResourceType" }, new TypedValue[] { new TypedValue(typeof(uint), (uint)0x0166038C) }) != null, true);
            ir.Filename = fileImportDialog.FileName;
            DialogResult dr = ir.ShowDialog();
            if (dr != DialogResult.OK) return;

            TGIN tgin = new TGIN();
            tgin.ResType = ir.ResourceType;
            tgin.ResGroup = ir.ResourceGroup;
            tgin.ResInstance = ir.Instance;
            tgin.ResName = ir.ResourceName;
            importFile(ir.Filename, tgin, ir.UseName, ir.AllowRename, ir.Compress, ir.Replace);
        }

        void importBatch(string[] batch)
        {
            ImportBatch ib = new ImportBatch(batch);
            DialogResult dr = ib.ShowDialog();
            if (dr != DialogResult.OK) return;

            try
            {
                this.Enabled = false;
                foreach (string filename in batch)
                {
                    importFile(filename, filename, ib.UseNames, ib.Rename, ib.Compress, ib.Replace);
                    Application.DoEvents();
                }
            }
            finally { this.Enabled = true; }
        }

        void importFile(string filename, TGIN tgin, bool useName, bool rename, bool compress, bool replace)
        {
            if (useName && tgin.ResName != null && tgin.ResName.Length > 0)
                UpdateNameMap(tgin.ResInstance, tgin.ResName, true, rename);

            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);
            BinaryReader r = new BinaryReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            w.Write(r.ReadBytes((int)r.BaseStream.Length));
            r.Close();
            w.Flush();

            IResourceIndexEntry rie = NewResource(tgin.ResType, tgin.ResGroup, tgin.ResInstance, ms, replace, compress);
            if (rie != null) browserWidget1.Add(rie);
        }
    }
}
