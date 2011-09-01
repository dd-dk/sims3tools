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

namespace S3Pack
{
    public static class Sims3Pack
    {
        static string magic = "TS3Pack";
        static string extension = ".Sims3Pack";
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

            System.Xml.XPath.XPathDocument xdoc = new System.Xml.XPath.XPathDocument(ms);
            System.Xml.XPath.XPathNavigator nav = xdoc.CreateNavigator();

            string filename = Path.Combine(target, nav.SelectSingleNode("/Sims3Package/DisplayName").Value + ".xml");
            if (File.Exists(filename)) File.Delete(filename);
            BinaryWriter bw = new BinaryWriter(new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write));
            bw.Write(ms.ToArray());
            bw.Close();

            System.Xml.XPath.XPathNodeIterator packagedFiles = nav.Select("/Sims3Package/PackagedFile");
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

        public static void Pack(XmlValues v)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.PreserveWhitespace = true;
            xdoc.AppendChild(xdoc.CreateXmlDeclaration("1.0", "UTF-8", String.Empty));
            xdoc.AppendChild(xdoc.CreateSignificantWhitespace("\n"));

            XmlElement xe = xdoc.CreateElement("Sims3Package");
            xe.SetAttribute("Type", v.Sims3PackType);
            xe.SetAttribute("SubType", v.SubType);
            xdoc.AppendChild(xe);

            #region All the up front gubbins
            xe = xdoc.CreateElement("ArchiveVersion");
            xe.InnerText = v.ArchiveVersion;
            xdoc.DocumentElement.AppendChild(xdoc.CreateSignificantWhitespace("\n  "));
            xdoc.DocumentElement.AppendChild(xe);

            xe = xdoc.CreateElement("CodeVersion");
            xe.InnerText = v.CodeVersion;
            xdoc.DocumentElement.AppendChild(xdoc.CreateSignificantWhitespace("\n  "));
            xdoc.DocumentElement.AppendChild(xe);

            xe = xdoc.CreateElement("GameVersion");
            xe.InnerText = v.GameVersion;
            xdoc.DocumentElement.AppendChild(xdoc.CreateSignificantWhitespace("\n  "));
            xdoc.DocumentElement.AppendChild(xe);

            xe = xdoc.CreateElement("PackageId");
            xe.InnerText = v.PackageId;
            xdoc.DocumentElement.AppendChild(xdoc.CreateSignificantWhitespace("\n  "));
            xdoc.DocumentElement.AppendChild(xe);

            xe = xdoc.CreateElement("Date");
            xe.InnerText = v.Date;
            xdoc.DocumentElement.AppendChild(xdoc.CreateSignificantWhitespace("\n  "));
            xdoc.DocumentElement.AppendChild(xe);

            xe = xdoc.CreateElement("AssetVersion");
            xe.InnerText = v.AssetVersion;
            xdoc.DocumentElement.AppendChild(xdoc.CreateSignificantWhitespace("\n  "));
            xdoc.DocumentElement.AppendChild(xe);

            xe = xdoc.CreateElement("MinReqVersion");
            xe.InnerText = v.MinReqVersion;
            xdoc.DocumentElement.AppendChild(xdoc.CreateSignificantWhitespace("\n  "));
            xdoc.DocumentElement.AppendChild(xe);

            xe = xdoc.CreateElement("DisplayName");
            xe.AppendChild(xdoc.CreateCDataSection(v.DisplayName));
            xdoc.DocumentElement.AppendChild(xdoc.CreateSignificantWhitespace("\n  "));
            xdoc.DocumentElement.AppendChild(xe);

            xe = xdoc.CreateElement("Description");
            xe.AppendChild(xdoc.CreateCDataSection(v.Description));
            xdoc.DocumentElement.AppendChild(xdoc.CreateSignificantWhitespace("\n  "));
            xdoc.DocumentElement.AppendChild(xe);

            xdoc.DocumentElement.AppendChild(xdoc.CreateSignificantWhitespace("\n  "));
            xdoc.DocumentElement.AppendChild(xdoc.CreateElement("LocalizedNames"));

            xdoc.DocumentElement.AppendChild(xdoc.CreateSignificantWhitespace("\n  "));
            xdoc.DocumentElement.AppendChild(xdoc.CreateElement("LocalizedDescriptions"));
            #endregion

            long Offset = 0;
            using (Stream sw = new FileStream(v.Target, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                BinaryWriter bw = new BinaryWriter(sw);

                try
                {
                    #region Package - first pass: XML
                    XmlElement pf = xdoc.CreateElement("PackagedFile");
                    
                    xe = xdoc.CreateElement("Name");
                    xe.InnerText = v.PackageId + ".package";
                    pf.AppendChild(xdoc.CreateSignificantWhitespace("\n    "));
                    pf.AppendChild(xe);

                    xe = xdoc.CreateElement("Guid");
                    xe.InnerText = v.PackageId;
                    pf.AppendChild(xdoc.CreateSignificantWhitespace("\n    "));
                    pf.AppendChild(xe);

                    xe = xdoc.CreateElement("ContentType");
                    xe.InnerText = v.Sims3PackType;
                    pf.AppendChild(xdoc.CreateSignificantWhitespace("\n    "));
                    pf.AppendChild(xe);

                    xe = xdoc.CreateElement("EPFlags");
                    xe.InnerText = v.EPFlags;
                    pf.AppendChild(xdoc.CreateSignificantWhitespace("\n    "));
                    pf.AppendChild(xe);

                    XmlElement mt = xdoc.CreateElement("metatags");
                    if (v.Thumbnail != null)
                    {
                        xe = xdoc.CreateElement("numOfThumbs");
                        xe.InnerText = "1";
                        mt.AppendChild(xdoc.CreateSignificantWhitespace("\n      "));
                        mt.AppendChild(xe);
                    }
                    pf.AppendChild(xdoc.CreateSignificantWhitespace("\n    "));
                    pf.AppendChild(mt);

                    using (Stream sr = new FileStream(v.Package, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        try
                        {
                            xe = xdoc.CreateElement("Length");
                            xe.InnerText = sr.Length + "";
                            pf.AppendChild(xdoc.CreateSignificantWhitespace("\n    "));
                            pf.AppendChild(xe);

                            xe = xdoc.CreateElement("Offset");
                            xe.InnerText = Offset + "";
                            pf.AppendChild(xdoc.CreateSignificantWhitespace("\n    "));
                            pf.AppendChild(xe);

                            UInt64 Crc = System.Security.Cryptography.Sims3PackCRC.CalculateCRC(sr);
                            xe = xdoc.CreateElement("Crc");
                            xe.InnerText = Crc.ToString("x");
                            pf.AppendChild(xdoc.CreateSignificantWhitespace("\n    "));
                            pf.AppendChild(xe);

                            Offset += sr.Length;
                        }
                        catch (Exception e) { throw e; }
                        finally { sr.Close(); }
                    }

                    pf.AppendChild(xdoc.CreateSignificantWhitespace("\n  "));
                    xdoc.DocumentElement.AppendChild(xdoc.CreateSignificantWhitespace("\n  "));
                    xdoc.DocumentElement.AppendChild(pf);
                    #endregion

                    if (v.Thumbnail != null)
                    {
                        #region Thumbnail - first pass: XML
                        pf = xdoc.CreateElement("PackagedFile");

                        xe = xdoc.CreateElement("Name");
                        xe.InnerText = Path.GetFileName(v.Thumbnail);
                        pf.AppendChild(xdoc.CreateSignificantWhitespace("\n    "));
                        pf.AppendChild(xe);

                        xe = xdoc.CreateElement("Guid");
                        xe.InnerText = "0x0000000000000000";
                        pf.AppendChild(xdoc.CreateSignificantWhitespace("\n    "));
                        pf.AppendChild(xe);

                        xe = xdoc.CreateElement("ContentType");
                        xe.InnerText = "unknown";
                        pf.AppendChild(xdoc.CreateSignificantWhitespace("\n    "));
                        pf.AppendChild(xe);

                        xe = xdoc.CreateElement("EPFlags");
                        xe.InnerText = v.EPFlags;
                        pf.AppendChild(xdoc.CreateSignificantWhitespace("\n    "));
                        pf.AppendChild(xe);

                        pf.AppendChild(xdoc.CreateSignificantWhitespace("\n    "));
                        pf.AppendChild(xdoc.CreateElement("metatags"));

                        using (Stream sr = new FileStream(v.Thumbnail, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            try
                            {
                                xe = xdoc.CreateElement("Length");
                                xe.InnerText = sr.Length + "";
                                pf.AppendChild(xdoc.CreateSignificantWhitespace("\n    "));
                                pf.AppendChild(xe);

                                xe = xdoc.CreateElement("Offset");
                                xe.InnerText = Offset + "";
                                pf.AppendChild(xdoc.CreateSignificantWhitespace("\n    "));
                                pf.AppendChild(xe);

                                UInt64 Crc = System.Security.Cryptography.Sims3PackCRC.CalculateCRC(sr);
                                xe = xdoc.CreateElement("Crc");
                                xe.InnerText = Crc.ToString("x");
                                pf.AppendChild(xdoc.CreateSignificantWhitespace("\n    "));
                                pf.AppendChild(xe);

                                Offset += sr.Length;
                            }
                            catch (Exception e) { throw e; }
                            finally { sr.Close(); }
                        }

                        pf.AppendChild(xdoc.CreateSignificantWhitespace("\n  "));
                        xdoc.DocumentElement.AppendChild(xdoc.CreateSignificantWhitespace("\n  "));
                        xdoc.DocumentElement.AppendChild(pf);
                        #endregion
                    }
                    xdoc.DocumentElement.AppendChild(xdoc.CreateSignificantWhitespace("\n"));

                    // Write the header
                    bw.Write((int)magic.Length);
                    bw.Write(magic.ToCharArray());
                    bw.Write(unknown1);

                    // Write the XML
                    MemoryStream ms = new MemoryStream();
                    xdoc.Save(ms);
                    bw.Write((int)ms.Length);
                    bw.Write(ms.ToArray());

                    #region Package - second pass: data
                    using (Stream sr = new FileStream(v.Package, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        try { sw.Write(new BinaryReader(sr).ReadBytes((int)sr.Length), 0, (int)sr.Length); }
                        catch (Exception e) { throw e; }
                        finally { sr.Close(); }
                    }
                    #endregion

                    if (v.Thumbnail != null)
                    {
                        #region Thumbnail - second pass: data
                        using (Stream sr = new FileStream(v.Thumbnail, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            try { sw.Write(new BinaryReader(sr).ReadBytes((int)sr.Length), 0, (int)sr.Length); }
                            catch (Exception e) { throw e; }
                            finally { sr.Close(); }
                        }
                        #endregion
                    }
                }
                catch (Exception e) { throw e; }
                finally { sw.Close(); }
            }
        }
    }

    public class XmlValues
    {
        public string Package { get; set; }
        public string CreatorName { get; set; }
        public string Title { get; set; }
        public string Target { get; set; }
        public string Sims3PackType { get; set; }
        public string SubType { get; set; }
        public string ArchiveVersion { get; set; }
        public string CodeVersion { get; set; }
        public string GameVersion { get; set; }
        public string PackageId { get; set; }
        public string Date { get; set; }
        public string AssetVersion { get; set; }
        public string MinReqVersion { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string EPFlags { get; set; }
        public string Thumbnail { get; set; }
    }
}
