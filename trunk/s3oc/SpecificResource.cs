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

namespace ObjectCloner
{
    public class SpecificResource : SpecificIndexEntry
    {
        public bool DefaultWrapper { get; private set; }
        public Exception Exception { get; private set; }
        IResource resource = null;
        public IResource Resource
        {
            get
            {
                try
                {
                    if (resource == null && ResourceIndexEntry != null) resource = s3pi.WrapperDealer.WrapperDealer.GetResource(0, PathPackage.Package, ResourceIndexEntry, DefaultWrapper);
                }
                catch (Exception ex)
                {
                    this.Exception = ex;
                    return null;
                }
                return resource;
            }
        }


        public SpecificResource(PathPackageTuple ppt, IResourceIndexEntry specificRK, bool defaultWrapper = false) : base(ppt, specificRK) { DefaultWrapper = defaultWrapper; }
        public SpecificResource(List<PathPackageTuple> searchList, IResourceKey requestedRK, bool defaultWrapper = false) : base(searchList, requestedRK) { DefaultWrapper = defaultWrapper; }


        public void Commit() { PathPackage.Package.ReplaceResource(ResourceIndexEntry, Resource); }
    }

    public class SpecificIndexEntry
    {
        public string PPSource { get; private set; }
        public PathPackageTuple PathPackage { get; private set; }
        public IResourceIndexEntry ResourceIndexEntry
        {
            get
            {
                if (specificIndexEntry == null && searchList != null) specificIndexEntry = findRK();
                return specificIndexEntry;
            }
        }
        public RK RequestedRK { get; private set; }
        public string LongName
        {
            get
            {
                string key = "0x" + specificIndexEntry.ResourceType.ToString("X8");
                List<string> exts = s3pi.Extensions.ExtList.Ext.ContainsKey(key) ? s3pi.Extensions.ExtList.Ext[key] : new List<string>();
                return String.Format("{0}-{1} from {2}", exts.Count > 0 ? exts[0] : "UNKN", specificIndexEntry, PathPackage.Path);
            }
        }

        public CatalogType CType { get { return (CatalogType)RequestedRK.ResourceType; } }
        public string RGVsn
        {
            get
            {
                if (ResourceIndexEntry == null) return "";
                byte rgVersion = (byte)(ResourceIndexEntry.ResourceGroup >> 27);
                if (rgVersion == 0) return "";
                if (!s3ocTTL.RGVersionLookup.ContainsKey(rgVersion)) return "Unk";
                return s3ocTTL.RGVersionLookup[rgVersion];
            }
        }

        private IResourceIndexEntry specificIndexEntry;
        private List<PathPackageTuple> searchList;

        public SpecificIndexEntry(PathPackageTuple ppt, IResourceIndexEntry resourceIndexEntry)
        {
            if (FileTable.Current == ppt) PPSource = "current";
            else if (FileTable.UseCustomContent && FileTable.CustomContent.Contains(ppt)) PPSource = "cc";
            else if (FileTable.fb0.Contains(ppt)) PPSource = "fb0";
            else if (FileTable.dds.Contains(ppt)) PPSource = "dds";
            else if (FileTable.tmb.Contains(ppt)) PPSource = "tmb";
            else PPSource = "unk";

            if (ppt.Package.Find(rie => rie.Equals(resourceIndexEntry)) == null) // Avoid recursive call to ppt.Find()
                throw new ArgumentException("Package does not contain specified ResourceIndexEntry", "resourceIndexEntry");
            this.RequestedRK = new RK(resourceIndexEntry);
            this.PathPackage = ppt;

            this.specificIndexEntry = resourceIndexEntry;
            this.searchList = null;
        }
        public SpecificIndexEntry(List<PathPackageTuple> searchList, IResourceKey requestedRK)
        {
            if (FileTable.IsEqual(searchList, FileTable.fb0)) PPSource = "fb0";
            else if (FileTable.IsEqual(searchList, FileTable.dds)) PPSource = "dds";
            else if (FileTable.IsEqual(searchList, FileTable.tmb)) PPSource = "tmb";
            else throw new ArgumentOutOfRangeException("searchList", "Search list must be one of fb0, dds or tmb FileTable properties.");
            this.PathPackage = null;
            this.RequestedRK = new RK(requestedRK);

            this.specificIndexEntry = null;
            this.searchList = searchList;
        }

        IResourceIndexEntry findRK()
        {
            foreach (var ppt in searchList)
            {
                var rie = ppt.Package.Find(RequestedRK.Equals);
                if (rie != null)
                {
                    PathPackage = ppt;
                    return rie;
                }
            }
            return null;
        }
    }
}
