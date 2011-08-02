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

        /// <summary>
        /// Stores the content of a folder (<paramref name="source"/>), as a Sims3Pack file in a given output folder (<paramref name="target"/>).
        /// </summary>
        /// <param name="source">Folder containing files to place in Sims3Pack</param>
        /// <param name="target">Folder to contain Sims3Pack file</param>
        public static void Pack(string source, string target)
        {
            if (!Directory.Exists(source))
                throw new DirectoryNotFoundException(String.Format("Directory not found: {0}", source));
            if (!Directory.Exists(target))
                throw new DirectoryNotFoundException(String.Format("Directory not found: {0}", target));

            System.Xml.XmlDocument xdoc = new XmlDocument();
            string xmlName = "";
            foreach (string xmlFile in Directory.GetFiles(source, "*.xml"))
            {
                try
                {
                    xdoc.Load(xmlFile);
                    if (!(xdoc.FirstChild is XmlDeclaration) || xdoc.SelectSingleNode("/Sims3Package") == null)
                        continue;
                    xmlName = xmlFile;
                    break;
                }
                catch { }
            }
            if (xmlName == "")
                throw new FileNotFoundException(String.Format("No Sims3Pack manifest file found in {0}.", source));

            XmlNode packageId = xdoc.SelectSingleNode("/Sims3Package/PackageId");
            if (packageId != null)
            {
                string[] packages = Directory.GetFiles(source, "*.package");
                if (packages.Length == 0)
                    throw new FileNotFoundException(String.Format("No package files found in {0}.", source));
                else if (packages.Length > 1)
                    throw new FileNotFoundException(String.Format("Multiple package files found in {0}.", source));

                if (packageId.InnerText != Path.GetFileNameWithoutExtension(packages[0]))
                    throw new InvalidDataException(String.Format("Package ID is {0}; package name should match but found {1}",
                        packageId.InnerText, Path.GetFileNameWithoutExtension(packages[0])));
            }

            long Offset = 0;
            string outfile = Path.Combine(target, xdoc.FirstChild.NextSibling.SelectSingleNode("DisplayName").InnerText + extension);
            using (Stream sw = new FileStream(outfile, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                BinaryWriter bw = new BinaryWriter(sw);

                try
                {
                    // Write the header
                    bw.Write((int)magic.Length);
                    bw.Write(magic.ToCharArray());
                    bw.Write(unknown1);

                    // First pass: calculate the data for the XML document, as we write that first.
                    foreach (XmlNode node in xdoc.SelectNodes("/Sims3Package/PackagedFile"))
                    {
                        string filename = Path.Combine(source, node.SelectSingleNode("Name").InnerText);
                        if (!File.Exists(filename))
                            throw new FileNotFoundException("File referenced in manifest not found", filename);

                        using (Stream sr = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            try
                            {
                                UInt64 Crc = System.Security.Cryptography.Sims3PackCRC.CalculateCRC(sr);

                                node.SelectSingleNode("Length").InnerText = sr.Length + "";
                                node.SelectSingleNode("Offset").InnerText = Offset + "";
                                node.SelectSingleNode("Crc").InnerText = Crc.ToString("x");

                                Offset += sr.Length;
                            }
                            catch (Exception e) { throw e; }
                            finally { sr.Close(); }
                        }
                    }

                    // Write the XML
                    MemoryStream ms = new MemoryStream();
                    xdoc.Save(ms);
                    bw.Write((int)ms.Length);
                    bw.Write(ms.ToArray());

                    // Second pass: copy the data into the Sims3Pack
                    foreach (XmlNode node in xdoc.SelectNodes("/Sims3Package/PackagedFile"))
                    {
                        string filename = Path.Combine(source, node.SelectSingleNode("Name").InnerText);
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
    }
}
