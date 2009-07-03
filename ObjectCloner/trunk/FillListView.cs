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
        string packageFile;
        MainForm.createListViewItemCallback createListViewItemCB;
        MainForm.updateProgressCallback updateProgressCB;
        MainForm.stopLoadingCallback stopLoadingCB;
        MainForm.loadingCompleteCallback loadingCompleteCB;

        public FillListView(Form mainForm, string packageFile
            , MainForm.createListViewItemCallback createListViewItemCB
            , MainForm.updateProgressCallback updateProgressCB
            , MainForm.stopLoadingCallback stopLoadingCB
            , MainForm.loadingCompleteCallback loadingCompleteCB
            )
        {
            this.mainForm = mainForm;
            this.packageFile = packageFile;
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

        void loadingComplete(IPackage pkg, bool complete)
        {
            Thread.Sleep(0);
            if (mainForm.IsHandleCreated)
                mainForm.BeginInvoke(loadingCompleteCB, new object[] { pkg, complete, });
        }


        public void LoadPackage()
        {
            IPackage pkg = null;
            bool complete = false;
            try
            {
                updateProgress(true, "Please wait, opening package...", true, -1, false, 0);
                pkg = s3pi.Package.Package.OpenPackage(0, packageFile);
                if (pkg == null) return;

                updateProgress(true, "Please wait, searching for objects...", true, -1, false, 0);
                IList<IResourceIndexEntry> lrie = pkg.FindAll(new string[] { "ResourceType", },
                    new TypedValue[] {
                        new TypedValue(typeof(uint), (uint)0x319E4F1D, "X"),
                    });


                int i = 0;
                updateProgress(true, "Please wait, loading objects...", true, lrie.Count, true, i);
                foreach (IResourceIndexEntry rie in lrie)
                {
                    if (stopLoading) break;

                    Item item = new Item(pkg, rie);

                    /* This is the wrong thumbnail
                    ulong inst = (ulong)((AApiVersionedFields)item.ObjD["CommonBlock"].Value)["PngInstance"].Value;
                    if (inst != 0)
                    {
                        updateProgress(true, "Please wait, loading images: 0x" + inst.ToString("X16"), false, -1, false, 0);
                        item.FindImages(pkg);
                        item.LoadImages(pkg);
                        updateProgress(true, "Please wait, loading objects...", false, -1, true, i);
                    }/**/
                    createListViewItem(item);

                    if (++i % 100 == 0)
                        updateProgress(false, "", false, -1, true, i);
                }
                complete = true;
            }
            catch (ThreadInterruptedException) { }
            finally
            {
                loadingComplete(pkg, complete);
            }
        }
    }


    public struct TGI
    {
        public uint t;
        public uint g;
        public ulong i;
        public TGI(uint t, uint g, ulong i) { this.t = t; this.g = g; this.i = i; }
        public TGI(IResourceIndexEntry rie) { t = rie.ResourceType; g = rie.ResourceGroup; i = rie.Instance; }
        public static implicit operator String(TGI value)
        {
            return String.Format("0x{0:X8}-0x{1:X8}-0x{2:X16}", value.t, value.g, value.i);
        }
        public static implicit operator TGI(String value)
        {
            TGI res = new TGI();
            string[] v = value.Split('-');
            res.t = Convert.ToUInt32(v[0], v[0].StartsWith("0x") ? 16 : 10);
            res.g = Convert.ToUInt32(v[1], v[1].StartsWith("0x") ? 16 : 10);
            res.i = Convert.ToUInt64(v[2], v[2].StartsWith("0x") ? 16 : 10);
            return res;
        }
    }

    public struct RIE
    {
        IPackage pkg;
        TGI tgi;
        public RIE(IPackage pkg, TGI tgi) : this() { this.pkg = pkg; this.tgi = tgi; }
        public IResourceIndexEntry rie
        {
            get
            {
                IResourceIndexEntry res = pkg.Find(new string[] { "ResourceType", "ResourceGroup", "Instance", },
                    new TypedValue[] {
                    new TypedValue(typeof(uint), tgi.t),
                    new TypedValue(typeof(uint), tgi.g),
                    new TypedValue(typeof(ulong), tgi.i),
                }
                    );
                return res;
            }
        }
    }

    public class Item : IEquatable<Item>, IEqualityComparer<Item>
    {
        IPackage pkg;
        IResourceIndexEntry objd;
        IResourceIndexEntry objd_thum32_png;
        IResourceIndexEntry objd_thum64_png;
        IResourceIndexEntry objd_thum128_png;

        IResource objd_res;

        Image thum32_png;
        Image thum64_png;
        Image thum128_png;

        private Item(IPackage pkg, string value)
        {
            this.pkg = pkg;

            string[] ries = value.Split(new char[] { ':' }, 5);
            objd = (new RIE(pkg, ries[0])).rie;
            objd_thum32_png = (new RIE(pkg, ries[1])).rie;
            objd_thum64_png = (new RIE(pkg, ries[2])).rie;
            objd_thum128_png = (new RIE(pkg, ries[3])).rie;
        }

        public Item(IPackage pkg, IResourceIndexEntry objd)
        {
            this.pkg = pkg;
            this.objd = objd;
        }

        public static KeyValuePair<int, Item> FromString(IPackage pkg, string value)
        {
            string[] keyvalue = value.Split(new char[] { ':' }, 2);
            int key = Int32.Parse(keyvalue[0]);
            return new KeyValuePair<int, Item>(key, new Item(pkg, keyvalue[1]));
        }

        public static implicit operator string(Item value)
        {
            return value.objd +
                ":" + value.objd_thum32_png +
                ":" + value.objd_thum64_png +
                ":" + value.objd_thum128_png
                ;
        }

        public IResource ObjD
        {
            get
            {
                if (objd == null) return null;
                if (objd_res == null)
                {
                    objd_res = s3pi.WrapperDealer.WrapperDealer.GetResource(0, pkg, objd);
                    if (objd_res == null) return null;
                }
                return objd_res;
            }
        }
        public IResourceIndexEntry rieObjD { get { return objd; } }

        public Image Thum32 { get { return thum32_png; } }
        public Image Thum64 { get { return thum64_png; } }
        public Image Thum128 { get { return thum128_png; } }

        public void LoadImages(IPackage pkg)
        {
            thum32_png = LoadImage(pkg, objd_thum32_png);
            thum64_png = LoadImage(pkg, objd_thum64_png);
            thum128_png = LoadImage(pkg, objd_thum128_png);
        }

        Image LoadImage(IPackage pkg, IResourceIndexEntry rie)
        {
            if (rie == null || rie.Instance == 0) return null;
            IResource res_img = s3pi.WrapperDealer.WrapperDealer.GetResource(0, pkg, rie);
            return res_img["Value"].Value as Image;
        }

        public void FindImages(IPackage pkg)
        {
            if (objd_thum32_png != null && objd_thum64_png != null && objd_thum128_png != null) return;

            if (ObjD == null) return;
            ulong instance = (ulong)((AApiVersionedFields)ObjD["CommonBlock"].Value)["PngInstance"].Value;

            /*0x2E75C764 small; 0x2E75C765 medium; 0x2E75C766 large*/
            if (objd_thum32_png == null) objd_thum32_png = (new RIE(pkg, new TGI(0x2E75C764, objd.ResourceGroup, instance))).rie;
            if (objd_thum64_png == null) objd_thum64_png = (new RIE(pkg, new TGI(0x2E75C765, objd.ResourceGroup, instance))).rie;
            if (objd_thum128_png == null) objd_thum128_png = (new RIE(pkg, new TGI(0x2E75C766, objd.ResourceGroup, instance))).rie;
        }

        #region IEquatable<CacheItem> Members

        public bool Equals(Item other) { return this.pkg.Equals(other.pkg) && this.objd.Equals(other.objd); }

        #endregion

        #region IEqualityComparer<CacheItem> Members

        public bool Equals(Item x, Item y) { return x.Equals(y); }

        public int GetHashCode(Item obj) { return pkg.GetHashCode() ^ objd.GetHashCode(); }

        #endregion
    }
}
