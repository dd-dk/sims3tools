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
using System.IO;
using s3pi.Interfaces;
using System.Threading;
using s3pi.Filetable;

namespace ObjectCloner
{
    public class SaveList
    {
        MainForm mainForm;
        SpecificResource selectedItem;
        Dictionary<string, IResourceKey> rkList;
        PathPackageTuple target = null;
        bool compress;
        bool addSTBLs;
        bool padSTBLs;
        bool zeroSTBLIID;
        bool excludeCommonResources;
        MainForm.updateProgressCallback updateProgressCB;
        StopSavingCallback stopSavingCB;
        SaveListComplete savingCompleteCB;

        public delegate bool StopSavingCallback();
        public delegate void SaveListComplete(bool complete);

        public SaveList(MainForm mainForm, SpecificResource selectedItem, Dictionary<string, IResourceKey> rkList,
            PathPackageTuple target, bool compress, bool addSTBLs, bool padSTBLs, bool zeroSTBLIID, bool excludeCommonResources,
                MainForm.updateProgressCallback updateProgressCB, StopSavingCallback stopSavingCB, SaveListComplete savingCompleteCB)
        {
            this.mainForm = mainForm;
            this.selectedItem = selectedItem;
            this.rkList = rkList;
            this.target = target;
            this.compress = compress;
            this.addSTBLs = addSTBLs;
            this.padSTBLs = padSTBLs;
            this.zeroSTBLIID = zeroSTBLIID;
            this.excludeCommonResources = excludeCommonResources;
            this.updateProgressCB = updateProgressCB;
            this.stopSavingCB = stopSavingCB;
            this.savingCompleteCB = savingCompleteCB;
        }

        //Type: 0x00B2D882 resources are in Fullbuild2, everything else is in Fullbuild0, except thumbs
        //FullBuild0 is:
        //  ...\Gamedata\Shared\Packages\FullBuild0.package
        //Relative path to ALLThumbnails is:
        // .\..\..\..\Thumbnails\ALLThumbnails.package
        public void SavePackage()
        {
            bool complete = false;

            updateProgress(true, "Please wait...", false, -1, false, -1);

            NameMap fb0nm = new NameMap(FileTable.GameContent);
            NameMap ddsnm = new NameMap(FileTable.DDSImages);

            SpecificResource newnmap = target.Find(rie => rie.ResourceType == 0x0166038C);
            if (newnmap == null)
                newnmap = target.AddResource(fb0nm.rk == RK.NULL ? new TGIBlock(0, null, 0x0166038C, 0, selectedItem.RequestedRK.Instance) : fb0nm.rk);
            IDictionary<ulong, string> newnamemap = (IDictionary<ulong, string>)newnmap.Resource;

            #region Skip everything we already have or don't want and de-duplicate
            Dictionary<string, IResourceKey> wrkList = new Dictionary<string, IResourceKey>();
            List<ulong> missingNameMapEntries = new List<ulong>();
            foreach (var kvp in rkList)
            {
                if (target.Find(rie => rie.Equals(kvp.Value)) != null || (excludeCommonResources && Exclusions.Contains(kvp.Value)))
                {
                    if (!newnamemap.ContainsKey(kvp.Value.Instance) && !missingNameMapEntries.Contains(kvp.Value.Instance)) missingNameMapEntries.Add(kvp.Value.Instance);
                    Diagnostics.Log(String.Format(target.Find(rie => rie.Equals(kvp.Value)) != null ? "{1} ({0}) already exists" : "{0} ({1}) is excluded", kvp.Key, kvp.Value));
                    continue;
                }
                wrkList.Add(kvp.Key, kvp.Value);
            }
            rkList = wrkList;
            wrkList = new Dictionary<string, IResourceKey>();
            List<IResourceKey> seen = new List<IResourceKey>();
            foreach (var kvp in rkList)
            {
                if (seen.Contains(kvp.Value))
                {
                    Diagnostics.Log(String.Format("Duplicate RK for {1} ({0})", kvp.Key, kvp.Value));
                    continue;
                }
                wrkList.Add(kvp.Key, kvp.Value);
                seen.Add(kvp.Value);
            }
            rkList = wrkList;
            #endregion

            #region Repair the name map with stuff we won't do later
            updateProgress(true, "Adding missing name map entries...", false, -1, false, -1);
            foreach (ulong key in missingNameMapEntries)
            {
                if (fb0nm.map.ContainsKey(key))
                    newnamemap.Add(key, fb0nm.map[key]);
                else if (ddsnm.map.ContainsKey(key))
                    newnamemap.Add(key, ddsnm.map[key]);
            }
            #endregion

            try
            {
                #region Update target package with required resources
                int i = 0;
                int freq = Math.Max(1, rkList.Count / 50);
                updateProgress(true, "Seeking resources... 0%", true, rkList.Count, true, i);
                string lastSaved = "nothing yet";

                foreach (var kvp in rkList)
                {
                    List<PathPackageTuple> lppt =
                        (selectedItem.RequestedRK.ResourceType != 0x04ED4BB2 && kvp.Value.ResourceType == 0x00B2D882) ? FileTable.DDSImages
                        : kvp.Key.EndsWith("Thumb") ? FileTable.Thumbnails
                        : FileTable.GameContent;
                    NameMap nm = kvp.Value.ResourceType == 0x00B2D882 ? ddsnm : fb0nm;

                    if (stopSaving) return;
                    SpecificResource item = new SpecificResource(lppt, kvp.Value, true); // use default wrapper

                    if (item.ResourceIndexEntry != null)
                    {
                        if (stopSaving) return;
                        target.AddResource(item.ResourceIndexEntry, item.Resource.Stream);
                        Diagnostics.Log(String.Format("Added {0} ({1})", kvp.Key, item.LongName));
                        lastSaved = kvp.Key;

                        if (!newnamemap.ContainsKey(kvp.Value.Instance))
                        {
                            string name = nm[kvp.Value.Instance];
                            if (name != null)
                            {
                                if (stopSaving) return;
                                newnamemap.Add(kvp.Value.Instance, name);
                                Diagnostics.Log(String.Format("Added {0} -> \"{1}\" to name map", kvp.Value.Instance, name));
                            }
                        }
                    }
                    else
                        Diagnostics.Log(String.Format("Failed to find {0}", kvp.Key));

                    if (++i % freq == 0)
                        updateProgress(true, "Added " + lastSaved + "... " + i * 100 / rkList.Count + "%", true, rkList.Count, true, i);
                }
                updateProgress(true, "", true, rkList.Count, true, rkList.Count);
                #endregion

                if (stopSaving) return;
                if (addSTBLs)
                    AddSTBLs(newnamemap);

                if (stopSaving) return;
                updateProgress(true, "Committing new name map... ", true, 0, true, 0);
                newnmap.Commit();
                Diagnostics.Log("Committed new name map...");

                if (stopSaving) return;
                updateProgress(true, "Compressing... ", true, 0, true, 0);
                target.Package.GetResourceList.ForEach(rie => rie.Compressed = (ushort)(compress ? 0xffff : 0x0000));
                updateProgress(true, "", true, 0, true, 0);

                complete = true;
            }
            catch (ThreadAbortException) { }
            finally
            {
                savingComplete(complete);
            }
        }

        void AddSTBLs(IDictionary<ulong, string> newnamemap)
        {
            #region Determine whether anything should be done
            SpecificResource catlgItem = selectedItem.CType() == CatalogType.ModularResource
                ? MainForm.ItemForTGIBlock0(selectedItem)
                : selectedItem;

            if (catlgItem.ResourceIndexEntry == null)
            {
                Diagnostics.Log("Modular Resource but could not find OBJD zero.");
                updateProgress(true, "Modular Resource but could not find OBJD zero.", true, 0, false, 0);
                return;
            }

            ulong nameGUID = (ulong)catlgItem.Resource["CommonBlock.NameGUID"].Value;
            ulong descGUID = (ulong)catlgItem.Resource["CommonBlock.DescGUID"].Value;

            if (nameGUID == STBLHandler.FNV64Blank && descGUID == STBLHandler.FNV64Blank)
            {
                Diagnostics.Log(String.Format("Both Name (0x{0:X16}) and Desc (0x{1:X16}) GUIDs are FNV64Blank.", nameGUID, descGUID));
                updateProgress(true, "Both Name and Desc GUIDs are FNV64Blank.", true, 0, false, 0);
                return;
            }

            Diagnostics.Log("Finding string tables...");
            updateProgress(true, "Finding string tables...", true, 0, true, 0);

            SpecificResource nameStbl = STBLHandler.findStblFor(nameGUID);
            SpecificResource descStbl = STBLHandler.findStblFor(descGUID);

            if (nameStbl == null && descStbl == null)
            {
                Diagnostics.Log(String.Format("Neither Name (0x{0:X16}) nor Desc (0x{1:X16}) GUID found.", nameGUID, descGUID));
                updateProgress(true, "No string tables found!", true, 0, false, 0);
                return;
            }

            /*
             * Decided to leave this to the code in the loop below
             * if ((nameStbl != null && nameStbl.PathPackage == target) && (descStbl != null && descStbl.PathPackage == target) && !padSTBLs)
            {
                Diagnostics.Log(String.Format("Both Name (0x{0:X16}) and Desc (0x{1:X16}) exist in {2} and not padding.", nameGUID, descGUID, target.Path));
                updateProgress(true, "Both Name and Desc GUIDs already present.", true, 0, false, 0);
                return;
            }/**/
            #endregion

            IResourceKey oldRK = (nameStbl != null ? nameStbl : descStbl).RequestedRK;
            uint stblGroup = oldRK.ResourceGroup;
            ulong stblInstance = oldRK.Instance & (ulong)(zeroSTBLIID ? 0x00FFFFFFFFFF0000 : 0x00FFFFFFFFFFFFFF);

            byte lang = 0;
            int freq = 1;// Math.Max(1, lrie.Count / 10);
            Diagnostics.Log("Creating string tables extracts...");
            updateProgress(true, "Creating string tables extracts... 0%", true, 0x17, true, lang);

            while (lang < 0x17)
            {
                if (stopSaving) return;
                try
                {
                    STBLHandler.AddSTBLToNameMap(newnamemap, lang, ((ulong)lang << 56) | stblInstance);

                    SpecificResource oldName = STBLHandler.findStblFor(nameGUID, (byte)lang);
                    SpecificResource oldDesc = STBLHandler.findStblFor(descGUID, (byte)lang);

                    // Nothing to copy from and not padding
                    if (oldName == null && oldDesc == null && !padSTBLs) continue;
                    // *Both* found in target for this language
                    if ((oldName != null && oldName.PathPackage == target) && (oldDesc != null && oldDesc.PathPackage == target)) continue;

                    IResourceKey newRK = new TGIBlock(0, null, 0x220557DA, stblGroup, ((ulong)lang << 56) | stblInstance);
                    SpecificResource newstbl = target.Find(rie => rie.Equals(newRK));
                    if (newstbl == null)
                        newstbl = target.AddResource(newRK);
                    IDictionary<ulong, string> outstbl = newstbl.Resource as IDictionary<ulong, string>;

                    AddSTBL(outstbl, nameGUID, oldName, nameStbl);
                    AddSTBL(outstbl, descGUID, oldDesc, descStbl);

                    if (stopSaving) return;
                    newstbl.Commit();
                }
                finally
                {
                    if (++lang % freq == 0)
                        updateProgress(true, "Creating string tables extracts... " + lang * 100 / 0x17 + "%", true, 0x17, true, lang);
                }
            }
        }

        void AddSTBL(IDictionary<ulong, string> outstbl, ulong guid, SpecificResource oldStbl, SpecificResource fbStbl)
        {
            if (oldStbl == null && !padSTBLs) return;
            if (outstbl.ContainsKey(guid)) return;

            if (oldStbl == null) oldStbl = fbStbl;
            IDictionary<ulong, string> oldDict = oldStbl != null ? oldStbl.Resource as IDictionary<ulong, string> : null;
            string name = oldDict != null && oldDict.ContainsKey(guid) ? oldDict[guid] : "";

            outstbl.Add(guid, name);
        }

        void updateProgress(bool changeText, string text, bool changeMax, int max, bool changeValue, int value)
        {
            Thread.Sleep(0);
            if (mainForm.IsHandleCreated) mainForm.Invoke(updateProgressCB, new object[] { changeText, text, changeMax, max, changeValue, value, });
        }

        bool stopSaving { get { Thread.Sleep(0); return !mainForm.IsHandleCreated || (bool)mainForm.Invoke(stopSavingCB); } }

        void savingComplete(bool complete)
        {
            Thread.Sleep(0);
            if (mainForm.IsHandleCreated)
                mainForm.BeginInvoke(savingCompleteCB, new object[] { complete, });
        }
    }
}
