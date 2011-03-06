/***************************************************************************
 *  Copyright (C) 2009, 2010 by Peter L Jones                              *
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
using System.Threading;
using System.Windows.Forms;
using s3pi.Interfaces;

namespace ObjectCloner
{
    class FillListView
    {
        Form mainForm;
        List<IPackage> objPkgs;
        List<IPackage> ddsPkgs;
        List<IPackage> tmbPkgs;
        CatalogType resourceType;
        MainForm.createListViewItemCallback createListViewItemCB;
        MainForm.updateProgressCallback updateProgressCB;
        MainForm.stopLoadingCallback stopLoadingCB;
        MainForm.loadingCompleteCallback loadingCompleteCB;

        public FillListView(Form mainForm, List<IPackage> objPkgs, List<IPackage> ddsPkgs, List<IPackage> tmbPkgs, CatalogType resourceType
            , MainForm.createListViewItemCallback createListViewItemCB
            , MainForm.updateProgressCallback updateProgressCB
            , MainForm.stopLoadingCallback stopLoadingCB
            , MainForm.loadingCompleteCallback loadingCompleteCB
            )
        {
            this.mainForm = mainForm;
            this.objPkgs = objPkgs;
            this.ddsPkgs = ddsPkgs;
            this.tmbPkgs = tmbPkgs;
            this.resourceType = resourceType;
            this.createListViewItemCB = createListViewItemCB;
            this.updateProgressCB = updateProgressCB;
            this.stopLoadingCB = stopLoadingCB;
            this.loadingCompleteCB = loadingCompleteCB;
        }

        void createListViewItem(Item objd) { Thread.Sleep(0); if (mainForm.IsHandleCreated) mainForm.Invoke(createListViewItemCB, new object[] { objd }); }

        void updateProgress(bool changeText, string text, bool changeMax, int max, bool changeValue, int value)
        {
            Thread.Sleep(0);
            if (mainForm.IsHandleCreated) mainForm.Invoke(updateProgressCB, new object[] { changeText, text, changeMax, max, changeValue, value, });
        }

        bool stopLoading { get { Thread.Sleep(0); return !mainForm.IsHandleCreated || (bool)mainForm.Invoke(stopLoadingCB); } }

        void loadingComplete(bool complete)
        {
            Thread.Sleep(0);
            if (mainForm.IsHandleCreated)
                mainForm.BeginInvoke(loadingCompleteCB, new object[] { complete, });
        }


        public void LoadPackage()
        {
            bool complete = false;
            try
            {
                updateProgress(true, "Please wait, searching for objects...", true, -1, false, 0);
                List<RIE> lrie = new List<RIE>();
                List<ulong> seen = new List<ulong>();
                List<IPackage> seenPkgs = new List<IPackage>();
                foreach (IPackage pkg in objPkgs)
                {
                    if (seenPkgs.Contains(pkg)) continue;
                    seenPkgs.Add(pkg);

                    IList<IResourceIndexEntry> matches;
                    if (resourceType != 0)
                        matches = pkg.FindAll(rie => rie.ResourceType == (uint)resourceType);
                    else
                        matches = pkg.GetResourceList;

                    foreach (IResourceIndexEntry match in matches)
                    {
                        if (!Enum.IsDefined(typeof(CatalogType), match.ResourceType)) continue;
                        if (seen.Contains(match.Instance)) continue;
                        seen.Add(match.Instance);
                        lrie.Add(new RIE(pkg, match));
                    }
                }


                int i = 0;
                int freq = Math.Max(1, lrie.Count / 50);
                updateProgress(true, "Please wait, loading objects... 0%", true, lrie.Count, true, i);
                foreach (RIE rie in lrie)
                {
                    if (stopLoading) return;

                    createListViewItem(new Item(rie));

                    if (++i % freq == 0)
                        updateProgress(true, "Please wait, loading objects... " + i * 100 / lrie.Count + "%", true, lrie.Count, true, i);
                }
                complete = true;
            }
            catch (ThreadInterruptedException) { }
            finally
            {
                loadingComplete(complete);
            }
        }
    }


    public enum CatalogType : uint
    {
        CatalogFence = 0x0418FE2A,
        CatalogStairs = 0x049CA4CD,
        CatalogProxyProduct = 0x04AC5D93,
        CatalogTerrainGeometryBrush = 0x04B30669,

        CatalogRailing = 0x04C58103,
        CatalogTerrainPaintBrush = 0x04ED4BB2,
        CatalogFireplace = 0x04F3CC01,
        CatalogTerrainWaterBrush = 0x060B390C,

        CatalogFountainPool = 0x0A36F07A,

        CatalogFoundation = 0x316C78F2,
        CatalogObject = 0x319E4F1D,
        CatalogWallFloorPattern = 0x515CA4CD,
        CatalogWallStyle = 0x9151E6BC,

        CatalogRoofStyle = 0x91EDBD3E,
        ModularResource = 0xCF9A4ACE,
        CatalogRoofPattern = 0xF1EDBD86,
    }

    /// <summary>
    /// This exists pretty much just so we have a small, concrete implementation of IResourceKey
    /// </summary>
    public class RK : AResourceKey
    {
        public RK(IResourceKey rk) : base(0, null, rk) { }

        RK() : base(0, null) { }
        static readonly RK rknull = new RK();
        public static RK NULL { get { return rknull; } }

        public override AHandlerElement Clone(EventHandler handler) { throw new NotImplementedException(); }
        public override List<string> ContentFields { get { throw new NotImplementedException(); } }
        public override int RecommendedApiVersion { get { throw new NotImplementedException(); } }

        public int Compare(IResourceKey rk)
        {
            int res = this.ResourceType.CompareTo(rk.ResourceType); if (res != 0) return res;
            res = (this.ResourceGroup & 0x07FFFFFF).CompareTo(rk.ResourceGroup & 0x07FFFFFF); if (res != 0) return res;
            return ((this.ResourceType == 0x736884F1 && this.Instance >> 32 == 0) ? this.Instance & 0x07FFFFFF : this.Instance)
                .CompareTo(((rk.ResourceType == 0x736884F1 && rk.Instance >> 32 == 0) ? rk.Instance & 0x07FFFFFF : rk.Instance));
        }

        public static IResourceKey Parse(string value)
        {
            IResourceKey result;
            if (!TryParse(value, out result)) throw new ArgumentException();
            return result;
        }
        public static bool TryParse(string value, out IResourceKey result)
        {
            result = new RK();

            string[] tgi = value.Trim().ToLower().Split('-');
            if (tgi.Length != 3) return false;
            foreach (var x in tgi) if (!x.StartsWith("0x")) return false;

            uint tg;
            if (!uint.TryParse(tgi[0].Substring(2), System.Globalization.NumberStyles.HexNumber, null, out tg)) return false;
            result.ResourceType = tg;
            if (!uint.TryParse(tgi[1].Substring(2), System.Globalization.NumberStyles.HexNumber, null, out tg)) return false;
            result.ResourceGroup = tg;

            ulong i;
            if (!ulong.TryParse(tgi[2].Substring(2), System.Globalization.NumberStyles.HexNumber, null, out i)) return false;
            result.Instance = i;

            return true;
        }

        public new bool Equals(IResourceKey rk) { return this.Compare(rk) == 0; }
    }

    public struct RIE
    {
        List<IPackage> searchList;
        RK requestedRK;

        IPackage specificPkg;
        IResourceKey specificRK;

        public RIE(List<IPackage> searchList, IResourceKey requestedRK) : this()
        {
            this.searchList = searchList;
            this.requestedRK = new RK(requestedRK);
            this.specificPkg = null;
            this.specificRK = null;
        }
        public RIE(IPackage pkg, IResourceIndexEntry specificRK) : this()
        {
            if (!pkg.GetResourceList.Contains(specificRK)) // Full RK matching
                throw new ArgumentException();
            this.searchList = null;
            this.requestedRK = new RK(specificRK);
            this.specificPkg = pkg;
            this.specificRK = specificRK;
        }

        public IPackage SpecificPkg { get { return specificPkg; } }
        public AResourceIndexEntry SpecificRK { get { if (specificPkg == null || specificRK == null) specificRK = findRK(); return specificRK as AResourceIndexEntry; } }
        public RK RequestedRK { get { return requestedRK; } }
        public static implicit operator AResourceIndexEntry(RIE rie) { return rie.findRK() as AResourceIndexEntry; }

        IResourceKey findRK()
        {
            RK rk = requestedRK;
            if (specificPkg != null) return specificPkg.Find(requestedRK.Equals);

            IResourceKey arie = null;
            for (int i = 0; arie == null && i < searchList.Count; i++)
                {
                    arie = searchList[i].Find(requestedRK.Equals);
                    if (arie != null) specificPkg = searchList[i];
                }
            return arie;
        }

        public string LongName
        {
            get
            {
                string key = "0x" + specificRK.ResourceType.ToString("X8");
                List<string> exts = s3pi.Extensions.ExtList.Ext.ContainsKey(key) ? s3pi.Extensions.ExtList.Ext[key] : new List<string>();
                return String.Format("{0}-{1} from {2}", exts.Count > 0 ? exts[0] : "UNKN", specificRK, MainForm.PathForPackage(searchList, specificPkg, true));
            }
        }
    }

    public class Item
    {
        RIE myrie;
        bool defaultWrapper;

        IResource my_ires = null;
        Exception ex = null;

        public Item(List<IPackage> posspkgs, IResourceKey rk) : this(posspkgs, rk, false) { }
        Item(List<IPackage> posspkgs, IResourceKey rk, bool defaultWrapper) : this(new RIE(posspkgs, rk), defaultWrapper) { }
        public Item(RIE rie, bool defaultWrapper = false) { this.myrie = rie; this.defaultWrapper = defaultWrapper; }

        public IResource Resource
        {
            get
            {
                try
                {
                    if (my_ires == null && myrie.SpecificRK != null) my_ires = s3pi.WrapperDealer.WrapperDealer.GetResource(0, myrie.SpecificPkg, myrie.SpecificRK, defaultWrapper);
                }
                catch (Exception ex)
                {
                    this.ex = ex;
                    return null;
                }
                return my_ires;
            }
        }

        public AResourceIndexEntry SpecificRK { get { return myrie.SpecificRK; } set { this.myrie = new RIE(myrie.SpecificPkg, value); my_ires = null; } }

        public IPackage Package { get { return myrie.SpecificPkg; } }

        public RK RequestedRK { get { return myrie.RequestedRK; } }

        public CatalogType CType { get { return (CatalogType)myrie.RequestedRK.ResourceType; } }

        public Exception Exception { get { return ex; } }

        public string RGVsn
        {
            get
            {
                if (this.SpecificRK == null) return "";
                return EPSP((byte)(this.SpecificRK.ResourceGroup >> 27));
            }
        }

        public static string EPSP(byte rgVersion)
        {
            if (rgVersion == 0) return "";
            if (!MainForm.RGVersionLookup.ContainsKey(rgVersion)) return "Unk";
            return MainForm.RGVersionLookup[rgVersion];
        }

        public void Commit() { myrie.SpecificPkg.ReplaceResource(myrie.SpecificRK, Resource); }

        public string LongName { get { return myrie.LongName; } }
    }
}
