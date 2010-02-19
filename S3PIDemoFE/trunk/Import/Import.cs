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

        private void resourceImport()
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

        private void resourceImportPackages()
        {
            try
            {
                this.Enabled = false;
                DialogResult dr = importPackagesDialog.ShowDialog();
                if (dr != DialogResult.OK) return;

                ImportBatch ib = new ImportBatch(importPackagesDialog.FileNames, ImportBatch.Mode.package);
                dr = ib.ShowDialog();
                if (dr != DialogResult.OK) return;

                importPackagesCommon(ib.Batch, ib.Compress, ib.Replace ? DuplicateHandling.replace : DuplicateHandling.reject, null);
            }
            finally { this.Enabled = true; }
        }

        static List<uint> xmlList = new List<uint>(new uint[] {
            0x025C95B6, //xml: UI Layout definitions
            0x025ED6F4, //xml: Sim Outfit
            0x0333406C, //xml: XML Resource (Uses include tuning constants)
            0x03B33DDF, //xml: Interaction Tuning (These are like the stuff you had in TTAB. Motive advertising and autonomy etc)
            0x044AE110, //xml: Complate Preset
            0x0604ABDA, //xml:
            0x73E93EEB, //xml: Package manifest
        });
        static List<uint> stblList = new List<uint>(new uint[] {
            0x220557DA, //STBL
        });
        static List<uint> nmapList = new List<uint>(new uint[] {
            0x0166038C, //NMAP
        });
        static List<uint> allowList = new List<uint>();
        private void resourceImportAsDBC()
        {
            if (allowList.Count == 0)
            {
                allowList.AddRange(xmlList);
                allowList.AddRange(stblList);
                allowList.AddRange(nmapList);
            }
            try
            {
                this.Enabled = false;
                DialogResult dr = importPackagesDialog.ShowDialog();
                if (dr != DialogResult.OK) return;

                importPackagesCommon(importPackagesDialog.FileNames, true, DuplicateHandling.allow, allowList);

                browserWidget1.Visible = false;
                lbProgress.Text = "Doing DBC clean up...";
                Application.DoEvents();

                #region (Don't) Fix up XML clashes
                bool needToMergeXMLs = false;
                foreach (uint xml in xmlList)
                {
                    IList<IResourceIndexEntry> lrie = dupsOnly(CurrentPackage.FindAll(new string[] { "ResourceType", "Instance", },
                        new TypedValue[] { new TypedValue(typeof(uint), xml), new TypedValue(typeof(ulong), (ulong)0), }));
                    if (lrie.Count > 0)
                    {
                        needToMergeXMLs = true;
                        break;
                    }
                }
                if (needToMergeXMLs)
                {
                    CopyableMessageBox.Show("Manual merge of XML files required.");
                }
                #endregion

                foreach (uint stbl in stblList)
                {
                    IList<IResourceIndexEntry> lrie = dupsOnly(CurrentPackage.FindAll(new string[] { "ResourceType", },
                        new TypedValue[] { new TypedValue(typeof(uint), stbl), }));
                    foreach (IResourceIndexEntry dup in lrie)
                    {
                        IList<IResourceIndexEntry> ldups = getDups(dup);
                        IResourceIndexEntry newRie = NewResource(dup, null, DuplicateHandling.allow, true);
                        IDictionary<ulong, string> newStbl = (IDictionary<ulong, string>)s3pi.WrapperDealer.WrapperDealer.GetResource(0, CurrentPackage, newRie);
                        foreach (IResourceIndexEntry rie in ldups)
                        {
                            IDictionary<ulong, string> oldStbl = (IDictionary<ulong, string>)s3pi.WrapperDealer.WrapperDealer.GetResource(0, CurrentPackage, rie);
                            foreach (var kvp in oldStbl) if (!newStbl.ContainsKey(kvp.Key)) newStbl.Add(kvp);
                            rie.IsDeleted = true;
                        }
                        CurrentPackage.ReplaceResource(newRie, (IResource)newStbl);
                        browserWidget1.Add(newRie);
                    }
                }

                foreach (uint nmap in nmapList)
                {
                    IList<IResourceIndexEntry> lrie = CurrentPackage.FindAll(new string[] { "ResourceType", },
                        new TypedValue[] { new TypedValue(typeof(uint), nmap), });
                    if (lrie.Count > 1)
                    {
                        IResourceIndexEntry newRie = NewResource(lrie[0], null, DuplicateHandling.allow, true);
                        IDictionary<ulong, string> newNmap = (IDictionary<ulong, string>)s3pi.WrapperDealer.WrapperDealer.GetResource(0, CurrentPackage, newRie);
                        foreach (IResourceIndexEntry rie in lrie)
                        {
                            IDictionary<ulong, string> oldNmap = (IDictionary<ulong, string>)s3pi.WrapperDealer.WrapperDealer.GetResource(0, CurrentPackage, rie);
                            foreach (var kvp in oldNmap) if (!newNmap.ContainsKey(kvp.Key)) newNmap.Add(kvp);
                            rie.IsDeleted = true;
                        }
                        CurrentPackage.ReplaceResource(newRie, (IResource)newNmap);
                        browserWidget1.Add(newRie);
                    }
                }
            }
            finally { browserWidget1.Visible = true; lbProgress.Text = ""; Application.DoEvents(); this.Enabled = true; }
        }
        IList<IResourceIndexEntry> dupsOnly(IList<IResourceIndexEntry> list)
        {
            List<IResourceKey> seen = new List<IResourceKey>();
            List<IResourceIndexEntry> res = new List<IResourceIndexEntry>();
            foreach (IResourceIndexEntry rie in list)
            {
                if (!seen.Contains(rie)) seen.Add(rie);
                else if (!res.Contains(rie)) res.Add(rie);
            }
            return res;
        }
        IList<IResourceIndexEntry> getDups(IResourceKey rk)
        {
            return CurrentPackage.FindAll(new string[] { "ResourceType", "ResourceGroup", "Instance", },
                new TypedValue[] {
                    new TypedValue(rk.ResourceType.GetType(), rk.ResourceType),
                    new TypedValue(rk.ResourceGroup.GetType(), rk.ResourceGroup),
                    new TypedValue(rk.Instance.GetType(), rk.Instance),
                });
        }

        private void importPackagesCommon(string[] packageList, bool compress, DuplicateHandling dups, List<uint> dupsList)
        {
            bool useNames = controlPanel1.UseNames;
            int autoState = controlPanel1.AutoOff ? 0 : controlPanel1.AutoHex ? 1 : 2;
            try
            {
                controlPanel1.UseNames = false;
                controlPanel1.AutoOff = true;
                browserWidget1.Visible = false;

                foreach (string filename in packageList)
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
                            DuplicateHandling dupThis = dupsList == null || dupsList.Contains(rie.ResourceType) ? dups
                                : dups == DuplicateHandling.allow ? DuplicateHandling.replace : DuplicateHandling.reject;
                            myDataFormat impres;
                            impres.tgin = rie as AResourceIndexEntry;
                            IResource res = s3pi.WrapperDealer.WrapperDealer.GetResource(0, imppkg, rie, true);
                            impres.data = res.AsBytes;
                            importStream(impres, false, false, compress, dupThis);
                            progressBar1.Value++;
                            if (progressBar1.Value % 100 == 0)
                                Application.DoEvents();
                        }
                    }
                    finally { progressBar1.Value = 0; Package.ClosePackage(0, imppkg); }
                }
            }
            finally
            {
                lbProgress.Text = "";
                controlPanel1.UseNames = useNames;
                switch (autoState)
                {
                    case 0: controlPanel1.AutoOff = true; break;
                    case 1: controlPanel1.AutoHex = true; break;
                    case 2: controlPanel1.AutoValue = true; break;
                }
                browserWidget1.Visible = true;
                Application.DoEvents();
            }
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

            importFile(ir.Filename, ir, ir.UseName, ir.AllowRename, ir.Compress, ir.Replace ? DuplicateHandling.replace : DuplicateHandling.reject);
        }

        void importSingle(myDataFormat data)
        {
            ResourceDetails ir = new ResourceDetails(CurrentPackage.Find(new string[] { "ResourceType" }, new TypedValue[] { new TypedValue(typeof(uint), (uint)0x0166038C) }) != null, true);
            ir.Filename = data.tgin;
            DialogResult dr = ir.ShowDialog();
            if (dr != DialogResult.OK) return;

            data.tgin = ir;
            importStream(data, ir.UseName, ir.AllowRename, ir.Compress, ir.Replace ? DuplicateHandling.replace : DuplicateHandling.reject);
        }

        void importBatch(string[] batch)
        {
            ImportBatch ib = new ImportBatch(batch);
            DialogResult dr = ib.ShowDialog();
            if (dr != DialogResult.OK) return;

            try
            {
                this.Enabled = false;
                bool nmOK = true;
                foreach (string filename in batch)
                {
                    nmOK = importFile(filename, filename, nmOK && ib.UseNames, ib.Rename, ib.Compress, ib.Replace ? DuplicateHandling.replace : DuplicateHandling.reject);
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
                foreach (myDataFormat data in ldata)
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
                    importStream(data, ib.UseNames, ib.Rename, ib.Compress, ib.Replace ? DuplicateHandling.replace : DuplicateHandling.reject);
                    Application.DoEvents();
                }
            }
            finally { this.Enabled = true; }
        }

        bool importFile(string filename, TGIN tgin, bool useName, bool rename, bool compress, DuplicateHandling dups)
        {
            IResourceKey rk = (AResource.TGIBlock)tgin;
            string resName = tgin.ResName;
            bool nmOK = true;
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);
            BinaryReader r = new BinaryReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            w.Write(r.ReadBytes((int)r.BaseStream.Length));
            r.Close();
            w.Flush();

            if (useName && resName != null && resName.Length > 0)
                nmOK = browserWidget1.ResourceName(rk.Instance, resName, true, rename);

            IResourceIndexEntry rie = NewResource(rk, ms, dups, compress);
            if (rie != null) browserWidget1.Add(rie);
            return nmOK;
        }

        void importStream(myDataFormat data, bool useName, bool rename, bool compress, DuplicateHandling dups)
        {
            if (useName && data.tgin.ResName != null && data.tgin.ResName.Length > 0)
                browserWidget1.ResourceName(data.tgin.ResInstance, data.tgin.ResName, true, rename);

            IResourceIndexEntry rie = NewResource((AResource.TGIBlock)data.tgin, new MemoryStream(data.data), dups, compress);
            if (rie != null) browserWidget1.Add(rie);
        }
    }
}
