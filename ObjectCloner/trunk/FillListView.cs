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
                List<TGI> seen = new List<TGI>();
                List<IPackage> seenPkgs = new List<IPackage>();
                foreach (IPackage pkg in objPkgs)
                {
                    if (seenPkgs.Contains(pkg)) continue;
                    seenPkgs.Add(pkg);

                    IList<IResourceIndexEntry> matches;
                    if (resourceType != 0)
                        matches = pkg.FindAll(new string[] { "ResourceType", }, new TypedValue[] { new TypedValue(typeof(uint), (uint)resourceType, "X"), });
                    else
                        matches = pkg.GetResourceList;

                    foreach (IResourceIndexEntry match in matches)
                    {
                        if (!Enum.IsDefined(typeof(CatalogType), match.ResourceType)) continue;
                        TGI tgi = new TGI(match);
                        if (seen.Contains(tgi)) continue;
                        seen.Add(tgi);
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

        CatalogFoundation = 0x316C78F2,
        CatalogObject = 0x319E4F1D,
        CatalogWallFloorPattern = 0x515CA4CD,
        CatalogWall = 0x9151E6BC,

        CatalogRoofStyle = 0x91EDBD3E,
        ModularResource = 0xCF9A4ACE,
        CatalogRoofPattern = 0xF1EDBD86,
    }

    public struct TGI : IEquatable<TGI>, IEqualityComparer<TGI>
    {
        public uint t;
        public uint g;
        public ulong i;
        public TGI(uint t, uint g, ulong i) { this.t = t; this.g = g; this.i = i; }

        public TGI(IResourceIndexEntry rie) : this(rie.ResourceType, rie.ResourceGroup, rie.Instance) { }

        public TGI(s3pi.Extensions.TGIN tgin) { t = tgin.ResType; g = tgin.ResGroup; i = tgin.ResInstance; }
        public static implicit operator TGI(s3pi.Extensions.TGIN tgin) { return new TGI(tgin); }
        public static implicit operator s3pi.Extensions.TGIN(TGI tgi)
        {
            s3pi.Extensions.TGIN tgin = new s3pi.Extensions.TGIN();
            tgin.ResType = tgi.t;
            tgin.ResGroup = tgi.g;
            tgin.ResInstance = tgi.i;
            return tgin;
        }

        public TGI(AResource.TGIBlock tgib) { t = tgib.ResourceType; g = tgib.ResourceGroup; i = tgib.Instance; }
        public static implicit operator TGI(AResource.TGIBlock tgib) { return new TGI(tgib); }

        public static implicit operator String(TGI value) { return value.ToString(); }
        public static implicit operator TGI(String value)
        {
            TGI res = new TGI();
            string[] v = value.Split('-');
            res.t = Convert.ToUInt32(v[0], v[0].StartsWith("0x") ? 16 : 10);
            res.g = Convert.ToUInt32(v[1], v[1].StartsWith("0x") ? 16 : 10);
            res.i = Convert.ToUInt64(v[2], v[2].StartsWith("0x") ? 16 : 10);
            return res;
        }

        public static bool operator ==(TGI a, TGI b) { return a.Equals(b); }
        public static bool operator !=(TGI a, TGI b) { return !a.Equals(b); }
        //public override bool Equals(object obj) { return this.Equals((TGI)obj); } -- no, do not want invalid cast exception
        public override int GetHashCode() { return GetHashCode(this); }
        public override string ToString() { return String.Format("0x{0:X8}-0x{1:X8}-0x{2:X16}", t, g, i); }

        #region IEquatable<TGI> Members

        public bool Equals(TGI other) { return t.Equals(other.t) && g.Equals(other.g) && i.Equals(other.i); }

        #endregion

        #region IEqualityComparer<TGI> Members

        public bool Equals(TGI x, TGI y) { return x.Equals(y); }

        public int GetHashCode(TGI obj) { return obj.t.GetHashCode() ^ obj.g.GetHashCode() ^ obj.i.GetHashCode(); }

        #endregion
    }

    public struct RIE
    {
        List<IPackage> posspkgs;
        TGI mytgi;

        IPackage package;
        AResourceIndexEntry irie;

        public RIE(List<IPackage> posspkgs, TGI tgi) : this() { this.posspkgs = posspkgs; this.mytgi = tgi; }
        public RIE(IPackage pkg, IResourceIndexEntry rie) : this(null, new TGI(rie)) { if (pkg.GetResourceList.Contains(rie)) { this.package = pkg; irie = rie as AResourceIndexEntry; } }

        public IPackage pkg { get { return package; } }
        public AResourceIndexEntry rie { get { if (pkg == null || irie == null) irie = findIRIE(); return irie; } }
        public TGI tgi { get { return mytgi; } }
        public static implicit operator AResourceIndexEntry(RIE rie) { return rie.findIRIE(); }

        AResourceIndexEntry findIRIE()
        {
            AResourceIndexEntry arie = null;
            if (package != null)
                arie = findIRIEinPkg(package);
            else for (int i = 0; arie == null && i < posspkgs.Count; i++)
                {
                    arie = findIRIEinPkg(posspkgs[i]);
                    if (arie != null) package = posspkgs[i];
                }
            return arie;
        }
        AResourceIndexEntry findIRIEinPkg(IPackage pkg)
        {
            return pkg.Find(new string[] { "ResourceType", "ResourceGroup", "Instance", },
                new TypedValue[] {
                    new TypedValue(typeof(uint), mytgi.t),
                    new TypedValue(typeof(uint), mytgi.g),
                    new TypedValue(typeof(ulong), mytgi.i),
                    }
            ) as AResourceIndexEntry;
        }

    }

    public class Item
    {
        RIE myrie;
        bool defaultWrapper;

        IResource my_ires = null;
        Exception ex = null;

        //public Item(List<IPackage> posspkgs, IResourceIndexEntry rie) : this(posspkgs, rie, false) { }
        //public Item(List<IPackage> posspkgs, IResourceIndexEntry rie, bool defaultWrapper) : this(new RIE(pkg, rie), defaultWrapper) { }
        public Item(List<IPackage> posspkgs, TGI tgi) : this(posspkgs, tgi, false) { }
        Item(List<IPackage> posspkgs, TGI tgi, bool defaultWrapper) : this(new RIE(posspkgs, tgi), defaultWrapper) { }
        public Item(RIE rie) : this(rie, false) { }
        public Item(RIE rie, bool defaultWrapper) { this.defaultWrapper = defaultWrapper; this.myrie = rie; }

        public IResource Resource
        {
            get
            {
                try
                {
                    if (my_ires == null && myrie.rie != null) my_ires = s3pi.WrapperDealer.WrapperDealer.GetResource(0, myrie.pkg, myrie.rie, defaultWrapper);
                }
                catch (Exception ex)
                {
                    this.ex = ex;
                    return null;
                }
                return my_ires;
            }
        }

        public IResourceIndexEntry ResourceIndexEntry { get { return myrie.rie; } set { this.myrie = new RIE(myrie.pkg, value); my_ires = null; } }

        public IPackage Package { get { return myrie.pkg; } }

        public TGI tgi { get { return myrie.tgi; } }

        public CatalogType CType { get { return (CatalogType)myrie.tgi.t; } }

        public Exception Exception { get { return ex; } }

        public void Commit() { myrie.pkg.ReplaceResource(myrie.rie, Resource); }
    }
}
