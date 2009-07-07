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
                int freq = lrie.Count / 20;
                updateProgress(true, "Please wait, loading objects...", true, lrie.Count, true, i);
                foreach (IResourceIndexEntry rie in lrie)
                {
                    if (stopLoading) return;

                    createListViewItem(new Item(pkg, rie));

                    if (++i % freq == 0)
                        updateProgress(false, "", true, lrie.Count, true, i);
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
        public static implicit operator TGI(s3pi.Extensions.TGIN tgin) { return new TGI(tgin.ResType, tgin.ResGroup, tgin.ResInstance); }
    }

    public struct RIE
    {
        IPackage package;
        TGI mytgi;

        IResourceIndexEntry irie;

        public RIE(IPackage pkg, TGI tgi) : this() { this.package = pkg; this.mytgi = tgi; }
        public RIE(IPackage pkg, IResourceIndexEntry rie) : this() { this.package = pkg; this.mytgi = new TGI(rie); if (pkg.GetResourceList.Contains(rie)) irie = rie; }
        public IPackage pkg { get { return package; } }
        public AResourceIndexEntry rie { get { if (irie == null) irie = (AResourceIndexEntry)this; return irie as AResourceIndexEntry; } }
        public TGI tgi { get { return mytgi; } }
        public static implicit operator AResourceIndexEntry(RIE rie)
        {
            return rie.package.Find(new string[] { "ResourceType", "ResourceGroup", "Instance", },
                new TypedValue[] {
                    new TypedValue(typeof(uint), rie.mytgi.t),
                    new TypedValue(typeof(uint), rie.mytgi.g),
                    new TypedValue(typeof(ulong), rie.mytgi.i),
                    }
                ) as AResourceIndexEntry;
        }
    }

    public struct RES
    {
        RIE ie;
        public RES(RIE rie) : this() { this.ie = rie; }
        public IPackage pkg { get { return ie.pkg; } }
        public AResource res { get { return (AResource)this; } }
        public static implicit operator AResource(RES res) { return s3pi.WrapperDealer.WrapperDealer.GetResource(0, res.ie.pkg, res.ie.rie) as AResource; }
        public static implicit operator RIE(RES res) { return res.ie; }
    }

    public class Item
    {
        RES myres;
        IResource objd_res = null;

        public Item(IPackage pkg, IResourceIndexEntry rie) { this.myres = new RES(new RIE(pkg, rie)); }

        public IResource ObjD
        {
            get
            {
                if (((RIE)myres).rie == null) return null;
                if (objd_res == null) objd_res = myres.res;
                return objd_res;
            }
        }

        public TGI tgi { get { return ((RIE)myres).tgi; } }

        public RES res { get { return myres; } }
    }
}
