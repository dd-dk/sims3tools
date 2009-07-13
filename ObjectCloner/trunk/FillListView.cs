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
        IPackage pkg;
        MainForm.createListViewItemCallback createListViewItemCB;
        MainForm.updateProgressCallback updateProgressCB;
        MainForm.stopLoadingCallback stopLoadingCB;
        MainForm.loadingCompleteCallback loadingCompleteCB;

        public FillListView(Form mainForm, string packageFile, IPackage pkg
            , MainForm.createListViewItemCallback createListViewItemCB
            , MainForm.updateProgressCallback updateProgressCB
            , MainForm.stopLoadingCallback stopLoadingCB
            , MainForm.loadingCompleteCallback loadingCompleteCB
            )
        {
            this.mainForm = mainForm;
            this.packageFile = packageFile;
            this.pkg = pkg;
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
                IList<IResourceIndexEntry> lrie = pkg.FindAll(new string[] { "ResourceType", },
                    new TypedValue[] {
                        new TypedValue(typeof(uint), (uint)0x319E4F1D, "X"),
                    });


                int i = 0;
                int freq = Math.Max(1, lrie.Count / 50);
                updateProgress(true, "Please wait, loading objects... 0%", true, lrie.Count, true, i);
                foreach (IResourceIndexEntry rie in lrie)
                {
                    if (stopLoading) return;

                    createListViewItem(new Item(pkg, rie));

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


    public struct TGI
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
        IPackage package;
        TGI mytgi;

        AResourceIndexEntry irie;

        public RIE(IPackage pkg, TGI tgi) : this() { this.package = pkg; this.mytgi = tgi; }
        public RIE(IPackage pkg, IResourceIndexEntry rie) : this(pkg, new TGI(rie)) { if (pkg.GetResourceList.Contains(rie)) irie = rie as AResourceIndexEntry; }

        public IPackage pkg { get { return package; } }
        public AResourceIndexEntry rie { get { if (irie == null) irie = (AResourceIndexEntry)this; return irie; } }
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

    public class Item
    {
        RIE myrie;
        bool defaultWrapper;

        IResource my_ires = null;

        public Item(IPackage pkg, IResourceIndexEntry rie) : this(pkg, rie, false) { }
        public Item(IPackage pkg, IResourceIndexEntry rie, bool defaultWrapper) : this(new RIE(pkg, rie), defaultWrapper) { }
        public Item(IPackage pkg, TGI tgi) : this(pkg, tgi, false) { }
        public Item(IPackage pkg, TGI tgi, bool defaultWrapper) : this(new RIE(pkg, tgi), defaultWrapper) { }
        public Item(RIE rie) : this(rie, false) { }
        public Item(RIE rie, bool defaultWrapper) { this.defaultWrapper = defaultWrapper; this.myrie = rie; }

        public IResource Resource
        {
            get
            {
                if (my_ires == null && myrie.rie != null) my_ires = s3pi.WrapperDealer.WrapperDealer.GetResource(0, myrie.pkg, myrie.rie, defaultWrapper);
                return my_ires;
            }
        }

        public IResourceIndexEntry ResourceIndexEntry { get { return myrie.rie; } set { this.myrie = new RIE(myrie.pkg, value); my_ires = null; } }

        public IPackage Package { get { return myrie.pkg; } }

        public TGI tgi { get { return myrie.tgi; } }

        public void Commit() { myrie.pkg.ReplaceResource(myrie.rie, Resource); }
    }
}
