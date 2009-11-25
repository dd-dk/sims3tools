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
                List<IResourceKey> seen = new List<IResourceKey>();
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
                        if (seen.Contains(match)) continue;
                        seen.Add(match);
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
    }

    public struct RIE
    {
        List<IPackage> posspkgs;
        IResourceKey myrk;

        IPackage package;
        AResourceIndexEntry irie;

        public RIE(List<IPackage> posspkgs, IResourceKey rk) : this() { this.posspkgs = posspkgs; this.myrk = rk; }
        public RIE(IPackage pkg, IResourceIndexEntry rie) : this(null, (IResourceKey)rie) { if (pkg.GetResourceList.Contains(rie)) { this.package = pkg; irie = rie as AResourceIndexEntry; } }

        public IPackage pkg { get { return package; } }
        public AResourceIndexEntry rie { get { if (pkg == null || irie == null) irie = findIRIE(); return irie; } }
        public IResourceKey rk { get { return myrk; } }
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
                    new TypedValue(typeof(uint), myrk.ResourceType),
                    new TypedValue(typeof(uint), myrk.ResourceGroup),
                    new TypedValue(typeof(ulong), myrk.Instance),
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
        public Item(List<IPackage> posspkgs, IResourceKey rk) : this(posspkgs, rk, false) { }
        Item(List<IPackage> posspkgs, IResourceKey rk, bool defaultWrapper) : this(new RIE(posspkgs, rk), defaultWrapper) { }
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

        public IResourceKey rk { get { return myrie.rk; } }

        public CatalogType CType { get { return (CatalogType)myrie.rk.ResourceType; } }

        public Exception Exception { get { return ex; } }

        public void Commit() { myrie.pkg.ReplaceResource(myrie.rie, Resource); }
    }
}
