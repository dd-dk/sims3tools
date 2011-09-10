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
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace S3Pack
{
    public static class Sims3Pack
    {
        static string magic = "TS3Pack";
        static ushort unknown1 = 0x0101;

        /// <summary>
        /// Extracts the content of a Sims3Pack file (<paramref name="source"/>) into a folder (<paramref name="target"/>).
        /// </summary>
        /// <param name="source">A valid Sims3Pack file</param>
        /// <param name="target">An existing folder</param>
        public static void Unpack(string source, string target)
        {
            if (!File.Exists(source))
                throw new FileNotFoundException("File not found", source);
            if (!Directory.Exists(target))
                throw new DirectoryNotFoundException(String.Format("Directory not found: {0}", target));

            string basename = Path.GetFileNameWithoutExtension(source);
            if (!File.Exists(Path.Combine(target, basename)))
                target = Path.Combine(target, basename);
            if (!Directory.Exists(target))
                Directory.CreateDirectory(target);

            FileStream fs = new FileStream(source, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);

            magic = new string(br.ReadChars(br.ReadInt32()));
            unknown1 = br.ReadUInt16();
            MemoryStream ms = new MemoryStream(br.ReadBytes(br.ReadInt32()));

            long basePos = fs.Position;

            XPathDocument xdoc = new XPathDocument(ms);
            XPathNavigator nav = xdoc.CreateNavigator();

            string filename;
            XPathNavigator pkgId = nav.SelectSingleNode("/Sims3Package/PackageId");
            if (pkgId != null)
                filename = pkgId.Value;
            else
            {
                pkgId = nav.SelectSingleNode("/Sims3Package/PackagedFile[Guid!=\"0x0000000000000000\"]/Guid");
                if (pkgId != null)
                    filename = pkgId.Value;
                else
                    filename = Path.GetFileNameWithoutExtension(source);
            }
            filename = Path.Combine(target, filename + ".xml");

            if (File.Exists(filename)) File.Delete(filename);
            BinaryWriter bw = new BinaryWriter(new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write));
            bw.Write(ms.ToArray());
            bw.Close();

            XPathNodeIterator packagedFiles = nav.Select("/Sims3Package/PackagedFile");
            while (packagedFiles.MoveNext())
            {
                fs.Position = basePos + Convert.ToInt64(packagedFiles.Current.SelectSingleNode("Offset").Value);
                filename = Path.Combine(target, packagedFiles.Current.SelectSingleNode("Name").Value);
                if (File.Exists(filename)) File.Delete(filename);
                bw = new BinaryWriter(new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write));
                bw.Write(br.ReadBytes(Convert.ToInt32(packagedFiles.Current.SelectSingleNode("Length").Value)));
                bw.Close();
            }
        }

        public static void Pack(string Package, string Target, XmlValues v)
        {
            long Offset = 0;
            // Update the XML
            foreach (PackagedFile pf in v.PackagedFiles)
            {
                string filename = GetFilename(pf.Name, v.PackageId, Package);
                using (Stream sr = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    try
                    {
                        pf.SetInnerText("Length", sr.Length + "");
                        pf.SetInnerText("Offset", Offset + "");

                        UInt64 Crc = System.Security.Cryptography.Sims3PackCRC.CalculateCRC(sr);
                        pf.SetInnerText("Crc", Crc.ToString("x"));

                        Offset += sr.Length;
                    }
                    finally { sr.Close(); }
                }
            }

            using (Stream sw = new FileStream(Target, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                BinaryWriter bw = new BinaryWriter(sw);

                try
                {
                    // Write the header
                    bw.Write((int)magic.Length);
                    bw.Write(magic.ToCharArray());
                    bw.Write(unknown1);

                    // Write the XML
                    using (MemoryStream ms = new MemoryStream())
                    using (XmlWriter xw = XmlWriter.Create(ms, new XmlWriterSettings() { CloseOutput = false, Indent = true, IndentChars = "  ", }))
                    {
                        v.Sims3PackType.OwnerDocument.WriteContentTo(xw);
                        xw.Flush();
                        xw.Close();
                        byte[] xml = ms.ToArray();
                        bw.Write((int)xml.Length);
                        bw.Write(xml);
                        ms.Close();
                    }

                    // Write all the packaged files
                    foreach (PackagedFile pf in v.PackagedFiles)
                    {
                        string filename = GetFilename(pf.Name, v.PackageId, Package);
                        using (Stream sr = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            try { sw.Write(new BinaryReader(sr).ReadBytes((int)sr.Length), 0, (int)sr.Length); }
                            catch (Exception e) { throw e; }
                            finally { sr.Close(); }
                        }
                    }
                }
                catch (Exception e) { throw e; }
                finally { sw.Close(); }
            }
        }

        static string GetFilename(XmlElement name, XmlElement pkgId, string pkg)
        {
            string filename = Path.Combine(Path.GetDirectoryName(pkg), name.InnerText);

            if (
                pkg.ToLower().EndsWith(".package") && //user selected a package
                name.InnerText.ToLower() == pkgId.InnerText.ToLower() + ".package" && //we're looking for the main package
                !File.Exists(filename) //but can't find it
                ) return pkg; //return the package selected by the user

            return filename;
        }
    }

    public class PackagedFile
    {
        XmlDocument Document;
        XmlElement Self;
        public PackagedFile(XmlElement root)
        {
            Document = root.OwnerDocument;
            Self = root;

            foreach (XmlAttribute attr in root.Attributes)
                Attributes.Add(attr);

            metatags = new List<XmlElement>();
            Elements = new List<XmlElement>();
            foreach (XmlElement elem in root.ChildNodes)
            {
                switch (elem.LocalName)
                {
                    case "Name": Name = elem; break;
                    case "Length": Length = elem; break;
                    case "Offset": Offset = elem; break;
                    case "Crc": Crc = elem; break;
                    case "Guid": Guid = elem; break;
                    case "ContentType": ContentType = elem; break;
                    case "EPFlags": EPFlags = elem; break;
                    case "metatags": foreach(XmlElement tag in elem.ChildNodes) metatags.Add(tag); break;
                    default: Elements.Add(elem); break;
                }
            }
        }

        public void SetAttributeValue(string attrName, string value)
        {
            if (Document.DocumentElement.HasAttribute(attrName))
            {
                Document.DocumentElement.SetAttribute(attrName, value);
            }
            else
            {
                XmlAttribute attr = Document.CreateAttribute(attrName);
                Document.DocumentElement.Attributes.Append(attr);
                Attributes.Add(attr);
                attr.Value = value;
            }
        }
        public string GetAttributeValue(XmlAttribute attr, string attrName, string defValue)
        {
            if (attr == null)
                SetAttributeValue(attrName, defValue);
            return attr.Value;
        }

        public void SetInnerText(string elemName, string value)
        {
            XmlElement elem;
            XmlElement existing = Self.SelectSingleNode(elemName) as XmlElement;
            if (existing != null)
            {
                elem = existing;
            }
            else
            {
                elem = Document.CreateElement(elemName);
                Self.AppendChild(elem);
                switch (elem.LocalName)
                {
                    case "Name": Name = elem; break;
                    case "Length": Length = elem; break;
                    case "Offset": Offset = elem; break;
                    case "Crc": Crc = elem; break;
                    case "Guid": Guid = elem; break;
                    case "ContentType": ContentType = elem; break;
                    case "EPFlags": EPFlags = elem; break;
                    case "metatags": foreach (XmlElement tag in elem.ChildNodes) metatags.Add(tag); break;
                    default: Elements.Add(elem); break;
                }
            }
            elem.InnerText = value;
        }
        public string GetInnerText(XmlElement elem, string elemName, string defValue)
        {
            if (elem == null)
            {
                SetInnerText(elemName, defValue);
                return defValue;
            }
            return elem.InnerText;
        }

        public List<XmlAttribute> Attributes { get; set; }

        public XmlElement Name { get; set; }
        public XmlElement Length { get; set; }
        public XmlElement Offset { get; set; }
        public XmlElement Crc { get; set; }
        public XmlElement Guid { get; set; }
        public XmlElement ContentType { get; set; }
        public XmlElement EPFlags { get; set; }
        public List<XmlElement> metatags { get; set; }
        public List<XmlElement> Elements { get; set; }
    }

    public class XmlValues
    {
        public static XmlValues GetXmlValues(string filename)
        {
            string xmlFile = Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename) + ".xml");
            try { return File.Exists(xmlFile) ? new XmlValues(xmlFile) : null; }
            catch (XmlException) { return null; }
        }

        public XmlValues() : this(new StringReader(@"<Sims3Package>
  <Dependencies>
    <Dependency>0x050cffe800000000050cffe800000000</Dependency>
  </Dependencies>
  <LocalisedNames/>
  <LocalizedDescriptions/>
</Sims3Package>")) { }
        public XmlValues(string manifest) : this(new StreamReader(manifest)) { }
        public XmlValues(TextReader tr) { Parse(tr); }

        void Parse(TextReader tr)
        {
            Document = new XmlDocument();
            Document.Load(tr);
            XmlElement root = Document.DocumentElement;

            foreach (XmlAttribute attr in root.Attributes)
            {
                switch (attr.LocalName)
                {
                    case "Type": Sims3PackType = attr; break;
                    case "SubType": SubType = attr; break;
                    default: Attributes.Add(attr); break;
                }
            }

            PackagedFiles = new List<PackagedFile>();
            Elements = new List<XmlElement>();
            foreach (XmlElement elem in root.ChildNodes)
            {
                switch (elem.LocalName)
                {
                    case "ArchiveVersion": ArchiveVersion = elem; break;
                    case "CodeVersion": CodeVersion = elem; break;
                    case "GameVersion": GameVersion = elem; break;
                    case "PackageId": PackageId = elem; break;
                    case "Date": Date = elem; break;
                    case "AssetVersion": AssetVersion = elem; break;
                    case "MinReqVersion": MinReqVersion = elem; break;
                    case "DisplayName": DisplayName = elem; break;
                    case "Description": Description = elem; break;
                    case "Dependencies": Dependencies = elem; break;
                    case "LocalisedNames": LocalisedNames = elem; break;
                    case "LocalizedDescriptions": LocalizedDescriptions = elem; break;
                    case "PackagedFile": PackagedFiles.Add(new PackagedFile(elem)); break;
                    default: Elements.Add(elem); break;
                }
            }
        }

        public void SetAttributeValue(string attrName, string value)
        {
            if (Document.DocumentElement.HasAttribute(attrName))
            {
                Document.DocumentElement.SetAttribute(attrName, value);
            }
            else
            {
                XmlAttribute attr = Document.CreateAttribute(attrName);
                Document.DocumentElement.Attributes.Append(attr);
                switch (attrName)
                {
                    case "Type": Sims3PackType = attr; break;
                    case "SubType": SubType = attr; break;
                    default: Attributes.Add(attr); break;
                }
                attr.Value = value;
            }
        }
        public string GetAttributeValue(XmlAttribute attr, string attrName, string defValue)
        {
            if (attr == null)
                SetAttributeValue(attrName, defValue);
            return attr.Value;
        }

        public void SetInnerText(string elemName, string value)
        {
            XmlElement elem;
            XmlElement existing = Document.DocumentElement.SelectSingleNode(elemName) as XmlElement;
            if (existing != null)
            {
                elem = existing;
            }
            else
            {
                elem = Document.CreateElement(elemName);
                Document.DocumentElement.AppendChild(elem);
                switch (elemName)
                {
                    case "ArchiveVersion": ArchiveVersion = elem; break;
                    case "CodeVersion": CodeVersion = elem; break;
                    case "GameVersion": GameVersion = elem; break;
                    case "PackageId": PackageId = elem; break;
                    case "Date": Date = elem; break;
                    case "AssetVersion": AssetVersion = elem; break;
                    case "MinReqVersion": MinReqVersion = elem; break;
                    case "DisplayName": DisplayName = elem; break;
                    case "Description": Description = elem; break;
                    case "Dependencies": Dependencies = elem; break;
                    case "LocalisedNames": LocalisedNames = elem; break;
                    case "LocalisedDescs": LocalizedDescriptions = elem; break;
                    case "PackagedFile": PackagedFiles.Add(new PackagedFile(elem)); break;
                    default: Elements.Add(elem); break;
                }
            }
            elem.InnerText = value;
        }
        public string GetInnerText(XmlElement elem, string elemName, string defValue)
        {
            if (elem == null)
            {
                SetInnerText(elemName, defValue);
                return defValue;
            }
            return elem.InnerText;
        }

        public PackagedFile CreatePackagedFile(string name)
        {
            PackagedFile pf = PackagedFiles.Find(x => x.Name.InnerText.Equals(name));
            if (pf != null) return pf;

            XmlElement _pf = Document.CreateElement("PackagedFile");
            Document.DocumentElement.AppendChild(_pf);
            pf = new PackagedFile(_pf);
            pf.SetInnerText("Name", name);
            pf.SetInnerText("EPFlags", "0x00000000");
            _pf.AppendChild(Document.CreateElement("metatags"));
            PackagedFiles.Add(pf);

            return pf;
        }

        public XmlNode RemovePackagedFile(string name)
        {
            PackagedFile pf = PackagedFiles.Find(x => x.Name.InnerText.Equals(name));
            if (pf == null) return null;

            PackagedFiles.Remove(pf);
            return Document.DocumentElement.RemoveChild(pf.Name.ParentNode);
        }

        public XmlDocument Document { get; private set; }

        public XmlAttribute Sims3PackType { get; set; }
        public XmlAttribute SubType { get; set; }
        public List<XmlAttribute> Attributes { get; set; }

        public XmlElement ArchiveVersion { get; set; }
        public XmlElement CodeVersion { get; set; }
        public XmlElement GameVersion { get; set; }
        public XmlElement PackageId { get; set; }
        public XmlElement Date { get; set; }
        public XmlElement AssetVersion { get; set; }
        public XmlElement MinReqVersion { get; set; }
        public XmlElement DisplayName { get; set; }
        public XmlElement Description { get; set; }
        public XmlElement Dependencies { get; set; }
        public XmlElement LocalisedNames { get; set; }
        public XmlElement LocalizedDescriptions { get; set; }
        public List<XmlElement> Elements { get; set; }
        public List<PackagedFile> PackagedFiles { get; set; }
    }

    public static class Extensions
    {
        public static IEnumerable<XmlElement> FindAll(this XmlElement root, Predicate<XmlElement> Match)
        {
            List<XmlElement> res = new List<XmlElement>();
            foreach (XmlElement elem in root.ChildNodes)
                if (Match(elem)) yield return elem;
        }

        public static bool TrueForAll(this XmlElement root, Predicate<XmlElement> Match)
        {
            foreach (XmlElement elem in root.ChildNodes)
                if (!Match(elem)) return false;
            return true;
        }
    }
}
