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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace S3PIDemoFE
{
    partial class MainForm
    {
        const string myDataFormatSingleFile = "x-application/s3pe.singleFile";
        const string myDataFormatBatch = "x-application/s3pe.batch";

        [Serializable]
        public struct myDataFormat
        {
            public TGIN tgin;
            public byte[] data;
        }

        private void fileImport()
        {
            bool useNames = controlPanel1.UseNames;
            try
            {
                browserWidget1.Visible = false;
                controlPanel1.UseNames = false;
                this.Enabled = false;
                DialogResult dr = importResourcesDialog.ShowDialog();
                if (dr != DialogResult.OK) return;

                if (importResourcesDialog.FileNames.Length > 1)
                    importBatch(importResourcesDialog.FileNames);
                else
                    importSingle(importResourcesDialog.FileName);
            }
            finally { controlPanel1.UseNames = useNames; browserWidget1.Visible = true; this.Enabled = true; }
        }

        private void fileImportPackages()
        {
            try
            {
                this.Enabled = false;
                DialogResult dr = importPackagesDialog.ShowDialog();
                if (dr != DialogResult.OK) return;

                ImportBatch ib = new ImportBatch(importPackagesDialog.FileNames, ImportBatch.Mode.package);
                dr = ib.ShowDialog();
                if (dr != DialogResult.OK) return;

                bool useNames = controlPanel1.UseNames;
                browserWidget1.Visible = false;
                controlPanel1.UseNames = false;
                foreach (string filename in ib.Batch)
                {
                    if (Filename != null && Filename.Length > 0 && Path.GetFullPath(Filename).Equals(Path.GetFullPath(filename)))
                    {
                        CopyableMessageBox.Show("Skipping current package.", importPackagesDialog.Title);
                        continue;
                    }

                    lbProgress.Text = "Importing " + Path.GetFileNameWithoutExtension(filename) + "...";
                    Application.DoEvents();
                    IPackage imppkg = Package.OpenPackage(0, filename);
                    try
                    {
                        IList<IResourceIndexEntry> lrie = imppkg.GetResourceList;
                        progressBar1.Value = 0;
                        progressBar1.Maximum = lrie.Count;
                        foreach (IResourceIndexEntry rie in lrie)
                        {
                            myDataFormat impres;
                            impres.tgin = rie as AResourceIndexEntry;
                            IResource res = s3pi.WrapperDealer.WrapperDealer.GetResource(0, imppkg, rie, true);
                            impres.data = res.AsBytes;
                            importStream(impres, false, false, ib.Compress, ib.Replace);
                            progressBar1.Value++;
                            if (progressBar1.Value % 100 == 0)
                                Application.DoEvents();
                        }
                    }
                    finally { progressBar1.Value = 0; Package.ClosePackage(0, imppkg); }
                }
            }
            finally { lbProgress.Text = ""; Application.DoEvents(); controlPanel1.UseNames = true; browserWidget1.Visible = true; this.Enabled = true; }
        }

        private void resourcePaste()
        {
            try
            {
                this.Enabled = false;
                if (Clipboard.ContainsData(myDataFormatSingleFile))
                {
                    IFormatter formatter = new BinaryFormatter();
                    MemoryStream ms = Clipboard.GetData(myDataFormatSingleFile) as MemoryStream;
                    myDataFormat d = (myDataFormat)formatter.Deserialize(ms);
                    ms.Close();

                    importSingle(d);
                }
                else if (Clipboard.ContainsData(myDataFormatBatch))
                {
                    IFormatter formatter = new BinaryFormatter();
                    MemoryStream ms = Clipboard.GetData(myDataFormatBatch) as MemoryStream;
                    List<myDataFormat> l = (List<myDataFormat>)formatter.Deserialize(ms);
                    ms.Close();

                    importBatch(l);
                }
                else if (Clipboard.ContainsFileDropList())
                {
                    System.Collections.Specialized.StringCollection fileDrop = Clipboard.GetFileDropList();
                    if (fileDrop == null || fileDrop.Count == 0) return;

                    if (fileDrop.Count == 1)
                    {
                        importSingle(fileDrop[0]);
                    }
                    else
                    {
                        string[] batch = new string[fileDrop.Count];
                        for (int i = 0; i < fileDrop.Count; i++) batch[i] = fileDrop[i];
                        importBatch(batch);
                    }
                }
            }
            finally { this.Enabled = true; }
        }

        private void browserWidget1_DragDrop(object sender, DragEventArgs e)
        {
            string[] fileDrop = e.Data.GetData("FileDrop") as String[];
            if (fileDrop == null || fileDrop.Length == 0) return;

            Application.DoEvents();
            try
            {
                this.Enabled = false;
                if (fileDrop.Length > 1)
                    importBatch(fileDrop);
                else
                    importSingle(fileDrop[0]);
            }
            finally { this.Enabled = true; }
        }

        void importSingle(string filename)
        {
            ResourceDetails ir = new ResourceDetails(CurrentPackage.Find(new string[] { "ResourceType" }, new TypedValue[] { new TypedValue(typeof(uint), (uint)0x0166038C) }) != null, true);
            ir.Filename = filename;
            DialogResult dr = ir.ShowDialog();
            if (dr != DialogResult.OK) return;

            TGIN tgin = new TGIN();
            tgin.ResType = ir.ResourceType;
            tgin.ResGroup = ir.ResourceGroup;
            tgin.ResInstance = ir.Instance;
            tgin.ResName = ir.ResourceName;
            importFile(ir.Filename, tgin, ir.UseName, ir.AllowRename, ir.Compress, ir.Replace);
        }

        void importSingle(myDataFormat data)
        {
            ResourceDetails ir = new ResourceDetails(CurrentPackage.Find(new string[] { "ResourceType" }, new TypedValue[] { new TypedValue(typeof(uint), (uint)0x0166038C) }) != null, true);
            ir.Filename = data.tgin;
            DialogResult dr = ir.ShowDialog();
            if (dr != DialogResult.OK) return;

            data.tgin.ResType = ir.ResourceType;
            data.tgin.ResGroup = ir.ResourceGroup;
            data.tgin.ResInstance = ir.Instance;
            data.tgin.ResName = ir.ResourceName;
            importStream(data, ir.UseName, ir.AllowRename, ir.Compress, ir.Replace);
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

        void importBatch(IList<myDataFormat> ldata)
        {
            ImportBatch ib = new ImportBatch(ldata);
            DialogResult dr = ib.ShowDialog();
            if (dr != DialogResult.OK) return;

            List<myDataFormat> output = new List<myDataFormat>();
            foreach (string b in ib.Batch)
            {
                foreach(myDataFormat data in ldata)
                    if (data.tgin == b) { output.Add(data); goto next; }
            next: { }
            }

            if (output.Count == 0) return;
            if (output.Count == 1) { importSingle(output[0]); return; }

            try
            {
                this.Enabled = false;
                foreach (myDataFormat data in output)
                {
                    importStream(data, ib.UseNames, ib.Rename, ib.Compress, ib.Replace);
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

        void importStream(myDataFormat data, bool useName, bool rename, bool compress, bool replace)
        {
            if (useName && data.tgin.ResName != null && data.tgin.ResName.Length > 0)
                UpdateNameMap(data.tgin.ResInstance, data.tgin.ResName, true, rename);

            IResourceIndexEntry rie = NewResource(data.tgin.ResType, data.tgin.ResGroup, data.tgin.ResInstance,
                new MemoryStream(data.data), replace, compress);
            if (rie != null) browserWidget1.Add(rie);
        }
    }
}
