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
                    importBatch(importResourcesDialog.FileNames, importResourcesDialog.Title);
                else
                    importSingle(importResourcesDialog.FileName, importResourcesDialog.Title);
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
                ib.UseNames = true;
                dr = ib.ShowDialog();
                if (dr != DialogResult.OK) return;

                importPackagesCommon(ib.Batch, ib.UseNames, ib.Compress, ib.Replace ? DuplicateHandling.replace : DuplicateHandling.reject, null, importPackagesDialog.Title);
            }
            finally { this.Enabled = true; }
        }

        static List<uint> xmlList = new List<uint>(new uint[] {
            0x025C95B6, //xml: UI Layout definitions
            0x025ED6F4, //xml: Sim Outfit
            //-Anach: do not allow duplicates 0x0333406C, //xml: XML Resource (Uses include tuning constants)
            0x03B33DDF, //xml: Interaction Tuning (These are like the stuff you had in TTAB. Motive advertising and autonomy etc)
            0x044AE110, //xml: Complate Preset
            0x0604ABDA, //xml:
            //-Anach: delete these 0x73E93EEB, //xml: Package manifest
        });
        static List<uint> stblList = new List<uint>(new uint[] {
            0x220557DA, //STBL
        });
        static List<uint> nmapList = new List<uint>(new uint[] {
            0x0166038C, //NMAP
        });
        // See http://dino.drealm.info/den/denforum/index.php?topic=244.0
        static List<uint> deleteList = new List<uint>(new uint[] {
            0x73E93EEB, //xml: sims3pack manifest
            //http://dino.drealm.info/den/denforum/index.php?topic=724.0
            //-Anach: no, this is needed by CAS 0x626F60CD, //THUM: sims3pack
            //http://dino.drealm.info/den/denforum/index.php?topic=253.msg1234#msg1234
            //-Anach: no, this is needed by roofing 0x2E75C765, //ICON: sims3pack
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

                AutoSaveState autoSaveState = AutoSaveState.Always;
                if (S3PIDemoFE.Properties.Settings.Default.AskDBCAutoSave)
                    autoSaveState = AutoSaveState.Ask;
                importPackagesCommon(importPackagesDialog.FileNames, true, true, DuplicateHandling.allow, allowList, importPackagesDialog.Title, autoSaveState);

                browserWidget1.Visible = false;
                lbProgress.Text = "Doing DBC clean up...";

                Application.DoEvents();
                DateTime now = DateTime.UtcNow;
                IList<IResourceIndexEntry> lrie = dupsOnly(CurrentPackage.FindAll(x => stblList.Contains(x.ResourceType)));
                foreach (IResourceIndexEntry dup in lrie)
                {
                    IList<IResourceIndexEntry> ldups = CurrentPackage.FindAll(rie => ((IResourceKey)dup).Equals(rie));
                    IResourceIndexEntry newRie = NewResource(dup, null, DuplicateHandling.allow, true);
                    IDictionary<ulong, string> newStbl = (IDictionary<ulong, string>)s3pi.WrapperDealer.WrapperDealer.GetResource(0, CurrentPackage, newRie);
                    foreach (IResourceIndexEntry rie in ldups)
                    {
                        IDictionary<ulong, string> oldStbl = (IDictionary<ulong, string>)s3pi.WrapperDealer.WrapperDealer.GetResource(0, CurrentPackage, rie);
                        foreach (var kvp in oldStbl) if (!newStbl.ContainsKey(kvp.Key)) newStbl.Add(kvp);
                        rie.IsDeleted = true;
                        if (now.AddMilliseconds(100) < DateTime.UtcNow) { Application.DoEvents(); now = DateTime.UtcNow; }
                    }
                    CurrentPackage.ReplaceResource(newRie, (IResource)newStbl);
                    browserWidget1.Add(newRie);
                }

                // Get rid of Sims3Pack resource that sneak in
                CurrentPackage.FindAll(x =>
                {
                    if (now.AddMilliseconds(100) < DateTime.UtcNow) { Application.DoEvents(); now = DateTime.UtcNow; }
                    if (deleteList.Contains(x.ResourceType)) { x.IsDeleted = true; return false; }
                    return false;
                });

                // If there are any remaining duplicate XMLs, give up - they're too messy to fix automatically
                if (dupsOnly(CurrentPackage.FindAll(x => xmlList.Contains(x.ResourceType))).Count > 0)
                    CopyableMessageBox.Show("Manual merge of XML files required.");
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


        internal enum AutoSaveState
        {
            Never,
            Ask,
            Always,
        }
        private void importPackagesCommon(string[] packageList, bool useNames, bool compress, DuplicateHandling dups, List<uint> dupsList, string title,
            AutoSaveState autoSaveState = AutoSaveState.Ask)
        {
            bool CPuseNames = controlPanel1.UseNames;
            int CPautoState = controlPanel1.AutoOff ? 0 : controlPanel1.AutoHex ? 1 : 2;
            DateTime now = DateTime.UtcNow;

            bool autoSave = false;
            if (autoSaveState == AutoSaveState.Ask)
            {
                switch (CopyableMessageBox.Show("Auto-save current package after each package imported?", title,
                     CopyableMessageBoxButtons.YesNoCancel, CopyableMessageBoxIcon.Question))
                {
                    case 0: autoSave = true; break;
                    case 2: return;
                }
            }
            else
                autoSave = autoSaveState == AutoSaveState.Always;

            try
            {
                controlPanel1.UseNames = false;
                controlPanel1.AutoOff = true;
                browserWidget1.Visible = false;
                bool skipAll = false;

                foreach (string filename in packageList)
                {
                    if (Filename != null && Filename.Length > 0 && Path.GetFullPath(Filename).Equals(Path.GetFullPath(filename)))
                    {
                        CopyableMessageBox.Show("Skipping current package.", importPackagesDialog.Title);
                        continue;
                    }

                    lbProgress.Text = "Importing " + Path.GetFileNameWithoutExtension(filename) + "...";
                    Application.DoEvents();
                    IPackage imppkg = null;
                    try
                    {
                        imppkg = Package.OpenPackage(0, filename);
                    }
                    catch (InvalidDataException ex)
                    {
                        if (skipAll) continue;
                        int btn = CopyableMessageBox.Show(String.Format("Could not open package {0}.\n{1}", Path.GetFileName(filename), ex.Message),
                            title, CopyableMessageBoxIcon.Error, new List<string>(new string[] {
                            "Skip this", "Skip all", "Abort"}), 0, 0);
                        if (btn == 0) continue;
                        if (btn == 1) { skipAll = true; continue; }
                        break;
                    }
                    try
                    {
                        IList<IResourceIndexEntry> lrie = imppkg.GetResourceList;
                        progressBar1.Value = 0;
                        progressBar1.Maximum = lrie.Count;
                        foreach (IResourceIndexEntry rie in lrie)
                        {
                            if (rie.ResourceType == 0x0166038C)//NMAP
                            {
                                if (useNames) foreach (var kvp in s3pi.WrapperDealer.WrapperDealer.GetResource(0, imppkg, rie) as IDictionary<ulong, string>)
                                        browserWidget1.ResourceName(kvp.Key, kvp.Value, true, false);
                            }
                            else
                            {
                                DuplicateHandling dupThis = dupsList == null || dupsList.Contains(rie.ResourceType) ? dups
                                    : dups == DuplicateHandling.allow ? DuplicateHandling.replace : DuplicateHandling.reject;
                                myDataFormat impres;
                                impres.tgin = rie as AResourceIndexEntry;
                                IResource res = s3pi.WrapperDealer.WrapperDealer.GetResource(0, imppkg, rie, true);
                                impres.data = res.AsBytes;
                                importStream(impres, false, false, compress, dupThis);
                                progressBar1.Value++;
                                if (now.AddMilliseconds(100) < DateTime.UtcNow) { Application.DoEvents(); now = DateTime.UtcNow; }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        CopyableMessageBox.IssueException(ex, "Could not import all resources - aborting.\n", title);
                        break;
                    }
                    finally { progressBar1.Value = 0; Package.ClosePackage(0, imppkg); }
                    if (autoSave) if (!fileSave()) break;
                }
            }
            finally
            {
                lbProgress.Text = "";
                controlPanel1.UseNames = CPuseNames;
                switch (CPautoState)
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
                        importSingle(fileDrop[0], "Resource->Paste");
                    }
                    else
                    {
                        string[] batch = new string[fileDrop.Count];
                        for (int i = 0; i < fileDrop.Count; i++) batch[i] = fileDrop[i];
                        importBatch(batch, "Resource->Paste");
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
                    importBatch(fileDrop, "File(s)->Drop");
                else if (Directory.Exists(fileDrop[0]))
                    importBatch(fileDrop, "File(s)->Drop");
                else
                    importSingle(fileDrop[0], "File(s)->Drop");
            }
            finally { this.Enabled = true; }
        }

        static string[] asPkgExts = new string[] { ".package", ".world", ".dbc", ".nhd", };
        void importSingle(string filename, string title)
        {
            if (CurrentPackage == null)
                fileNew();

            if (new List<string>(asPkgExts).Contains(filename.Substring(filename.LastIndexOf('.'))))
            {
                try
                {
                    ImportBatch ib = new ImportBatch( new string[] { filename, }, ImportBatch.Mode.package);
                    ib.UseNames = true;
                    DialogResult dr = ib.ShowDialog();
                    if (dr != DialogResult.OK) return;

                    this.Enabled = false;
                    importPackagesCommon(new string[] { filename, }, ib.UseNames, ib.Compress, ib.Replace ? DuplicateHandling.replace : DuplicateHandling.reject, null, title);
                }
                finally { this.Enabled = true; }
            }
            else
            {

                ResourceDetails ir = new ResourceDetails(CurrentPackage.Find(_key => _key.ResourceType == 0x0166038C) != null, true);
                ir.Filename = filename;
                DialogResult dr = ir.ShowDialog();
                if (dr != DialogResult.OK) return;

                importFile(ir.Filename, ir, ir.UseName, ir.AllowRename, ir.Compress, ir.Replace ? DuplicateHandling.replace : DuplicateHandling.reject);
            }
        }

        void importSingle(myDataFormat data)
        {
            ResourceDetails ir = new ResourceDetails(CurrentPackage.Find(_key => _key.ResourceType == 0x0166038C) != null, true);
            ir.Filename = data.tgin;
            DialogResult dr = ir.ShowDialog();
            if (dr != DialogResult.OK) return;

            data.tgin = ir;
            importStream(data, ir.UseName, ir.AllowRename, ir.Compress, ir.Replace ? DuplicateHandling.replace : DuplicateHandling.reject);
        }

        string[] getFiles(string folder)
        {
            if (!Directory.Exists(folder)) return null;
            List<string> files = new List<string>();
            foreach (string dir in Directory.GetDirectories(folder))
                files.AddRange(getFiles(dir));
            files.AddRange(Directory.GetFiles(folder));
            return files.ToArray();
        }
        void importBatch(string[] batch, string title)
        {
            if (CurrentPackage == null)
                fileNew();

            List<string> foo = new List<string>();
            foreach (string bar in batch)
                if (Directory.Exists(bar)) foo.AddRange(getFiles(bar));
                else if (File.Exists(bar)) foo.Add(bar);
            batch = foo.ToArray();

            ImportBatch ib = new ImportBatch(batch);
            ib.Text = title;
            DialogResult dr = ib.ShowDialog();
            if (dr != DialogResult.OK) return;

            List<string> resList = new List<string>();
            List<string> pkgList = new List<string>();
            List<string> folders = new List<string>();

            List<string> pkgExts = new List<string>(asPkgExts);
            foreach (string s in batch)
                (pkgExts.Contains(s.Substring(s.LastIndexOf('.'))) ? pkgList : resList).Add(s);

            try
            {
                this.Enabled = false;

                if (pkgList.Count > 0)
                    importPackagesCommon(pkgList.ToArray(), ib.UseNames, ib.Compress, ib.Replace ? DuplicateHandling.replace : DuplicateHandling.reject, null, title);

                bool nmOK = true;
                foreach (string filename in resList)
                {
                    nmOK = importFile(filename, filename, nmOK && ib.UseNames, ib.Rename, ib.Compress, ib.Replace ? DuplicateHandling.replace : DuplicateHandling.reject);
                    Application.DoEvents();
                }
            }
            catch (Exception ex)
            {
                CopyableMessageBox.IssueException(ex, "Could not import all resources - aborting.\n", title);
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
            catch (Exception ex)
            {
                CopyableMessageBox.IssueException(ex, "Could not import all resources.\n", "Aborting import");
            }
            finally { this.Enabled = true; }
        }

        bool importFile(string filename, TGIN tgin, bool useName, bool rename, bool compress, DuplicateHandling dups)
        {
            IResourceKey rk = (TGIBlock)tgin;
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

            IResourceIndexEntry rie = NewResource((TGIBlock)data.tgin, new MemoryStream(data.data), dups, compress);
            if (rie != null) browserWidget1.Add(rie);
        }
    }
}