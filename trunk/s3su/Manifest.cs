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
using System.Linq;
using System.Xml;
using s3pi.Filetable;
using s3pi.Interfaces;

namespace S3Pack
{
    public class Manifest : AResource
    {
        PathPackageTuple ppt = null;
        string packageid = null;
        string packagetype = null;
        bool createMissingThumb;

        XmlDocument Document = null;
        XmlElement KeyList = null;
        XmlElement NumOfThumbs = null;
        int numofthumbs = 0;

        public static void UpdatePackage(string path, string packageid, string packagetype, bool createMissingThumb)
        {
            PathPackageTuple ppt = new PathPackageTuple(path, true);
            FileTable.Current = ppt;
            ppt.FindAll(rie => rie.ResourceType == 0x73E93EEB).ForEach(x => x.ResourceIndexEntry.IsDeleted = true);

            Manifest mf = new Manifest();
            mf.ppt = ppt;
            mf.packageid = packageid;
            mf.packagetype = packagetype;
            mf.createMissingThumb = createMissingThumb;

            var r = ppt.Package.AddResource(new TGIBlock(0, null, 0x73E93EEB, 0, 0), null, false);
            ppt.Package.ReplaceResource(r, mf);
            ppt.Package.SavePackage();
            s3pi.Package.Package.ClosePackage(0, ppt.Package);

            FileTable.Current = null;
        }

        Manifest() : base(0, null) { }

        protected override Stream UnParse()
        {
            if (ppt == null)
                throw new InvalidOperationException("PathPackageTuple must not be null.");

            if (packageid == null)
                throw new InvalidOperationException("PackageId must not be null.");

            if (packagetype == null)
                throw new InvalidOperationException("Type of package must not be null.");

            Document = new XmlDocument();
            XmlElement root = Document.CreateElement("manifest");
            Document.AppendChild(root);

            root.SetAttribute("packagesubtype", "00000000");
            root.SetAttribute("packagetype", packagetype);
            root.SetAttribute("version", "3");
            root.SetAttribute("true", "false");

            root.AppendChild(CreateElement("gameversion", "0.0.0.11195"));
            root.AppendChild(CreateElement("packagedate", DateTime.UtcNow.ToString("MM/dd/yyyy")));
            root.AppendChild(CreateElement("assetversion", "0"));
            root.AppendChild(CreateElement("mingamever", "1.0.0.0"));
            root.AppendChild(Document.CreateElement("handler"));
            root.AppendChild(CreateDefaultDependencyList());
            root.AppendChild(CreateElement("packageid", packageid));

            root.AppendChild(CreateNumOfThumbs());

            KeyList = Document.CreateElement("keylist");
            ppt.Package.GetResourceList.ForEach(Add);
            root.AppendChild(KeyList);

            if (createMissingThumb && numofthumbs == 0)
            {
                RK newRK;
                System.Drawing.Image newIcon;
                SpecificResource srKey = FindLargestThumb();
                if (srKey != null)
                {
                    newRK = new RK(srKey.ResourceIndexEntry) { ResourceType = 0x2E75C765, };
                    newIcon = System.Drawing.Image.FromStream(srKey.Resource.Stream);
                }
                else
                {
                    srKey = ppt.Find(x => x.ResourceType == 0x0166038C);
                    if (srKey != null)
                        newRK = new RK(srKey.ResourceIndexEntry) { ResourceType = 0x2E75C765, };
                    else
                    {
                        newRK = new RK(RK.NULL)
                        {
                            ResourceType = 0x2E75C765,
                            ResourceGroup = 0,
                            Instance = System.Security.Cryptography.FNV64.GetHash(Path.GetFileNameWithoutExtension(ppt.Path))
                        };
                    }
                    newIcon = S3Pack.Properties.Resources.defaultIcon;
                }
                if (newIcon.Width != 256)
                    newIcon = newIcon.GetThumbnailImage(256, 256, () => false, System.IntPtr.Zero);
                MemoryStream ms = new MemoryStream();
                newIcon.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                Add(ppt.AddResource(newRK, ms).ResourceIndexEntry);
            }

            root.AppendChild(CreateLanguageElementWithChild("localizednames", "localizedname", "packagetitle"));
            root.AppendChild(CreateElement("packagetitle", ""));
            root.AppendChild(CreateLanguageElementWithChild("localizeddescriptions", "localizeddescription", "packagedesc"));
            root.AppendChild(CreateElement("packagedesc", ""));

            byte[] res;
            MemoryStream msXml = new MemoryStream();
            using (XmlWriter xw = XmlWriter.Create(msXml, new XmlWriterSettings()
            {
                CloseOutput = false,
                Encoding = System.Text.UTF8Encoding.UTF8,
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\n",
            }))
            {
                Document.Save(xw);
                xw.Flush();
                xw.Close();
                res = msXml.ToArray();
            }
            return msXml;
        }

        static uint[] smallThumbs = new uint[] {
            0x0580A2B4, 0x0589DC44, 0x05B17698,
            0x05B1B524, 0x2653E3C8, 0x2D4284F0,
            0x5DE9DBA0, 0x626F60CC,
        };
        static uint[] mediumThumbs = new uint[] {
            0x0580A2B5, 0x0589DC45, 0x05B17699,
            0x05B1B525, 0x2653E3C9, 0x2D4284F1,
            0x5DE9DBA1, 0x626F60CD,
        };
        static uint[] largeThumbs = new uint[] {
            0x0580A2B6, 0x0589DC46, 0x05B1769A,
            0x05B1B526, 0x2653E3CA, 0x2D4284F2,
            0x5DE9DBA2, 0x626F60CE,
        };
        SpecificResource FindLargestThumb()
        {
            return ppt.Find(x => largeThumbs.Contains(x.ResourceType)) ??
                ppt.Find(x => mediumThumbs.Contains(x.ResourceType)) ??
                ppt.Find(x => smallThumbs.Contains(x.ResourceType));
        }

        public override int RecommendedApiVersion { get { return 1; } }

        XmlElement CreateElement(string name, string innerText)
        {
            XmlElement elem = Document.CreateElement(name);
            elem.InnerText = innerText;
            return elem;
        }

        XmlElement CreateDefaultDependencyList()
        {
            XmlElement dependencylist = Document.CreateElement("dependencylist");
            dependencylist.AppendChild(CreateElement("packageid", "0x050cffe800000000050cffe800000000"));
            dependencylist.AppendChild(CreateElement("packageid", packageid));
            return dependencylist;
        }

        XmlElement CreateNumOfThumbs()
        {
            NumOfThumbs = CreateElement("numofthumbs", "" + numofthumbs);
            XmlElement metatags = Document.CreateElement("metatags");
            metatags.AppendChild(NumOfThumbs);
            return metatags;
        }

        XmlElement CreateLanguageElementWithChild(string root, string child, string childValue)
        {
            XmlElement outer = Document.CreateElement(root);
            XmlElement enUS = Document.CreateElement(child);
            enUS.SetAttribute("language", "en-US");
            enUS.AppendChild(Document.CreateCDataSection(childValue));
            outer.AppendChild(enUS);
            return outer;
        }

        static uint[] icons = new uint[] { 0x2E75C764, 0x2E75C765, 0x2E75C766, 0x2E75C767, };
        void Add(IResourceIndexEntry rie)
        {
            XmlElement key = CreateElement("reskey", string.Format("1:{0:x8}:{1:x8}:{2:x16}", rie.ResourceType, rie.ResourceGroup, rie.Instance));
            KeyList.AppendChild(key);
            if (icons.Contains(rie.ResourceType))
            {
                NumOfThumbs.InnerText = "" + ++numofthumbs;
                Document.DocumentElement.AppendChild(CreateElement("thumbnail", string.Format("{0:x8}:{1:x8}:{2:x16}", rie.ResourceType, rie.ResourceGroup, rie.Instance)));
            }
        }
    }
}
