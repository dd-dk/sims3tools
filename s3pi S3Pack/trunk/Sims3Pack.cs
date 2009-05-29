﻿/***************************************************************************
 *  Copyright (C) 2009 by Peter L Jones                                    *
 *  peter@users.sf.net                                                     *
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
        static ushort unknown1 = 0;

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

            magic = BitConverter.ToString(br.ReadBytes(br.ReadInt32()));
            unknown1 = br.ReadUInt16();
            MemoryStream ms = new MemoryStream(br.ReadBytes(br.ReadInt32()));

            long basePos = fs.Position;

            XmlReaderSettings xrs = new XmlReaderSettings();
            xrs.IgnoreComments = true;
            xrs.IgnoreProcessingInstructions = true;
            xrs.IgnoreWhitespace = true;
            XmlReader xr = XmlReader.Create(ms, xrs);
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(xr);
            System.Xml.XPath.XPathNavigator nav = xdoc.CreateNavigator();

            string filename = Path.Combine(target, nav.SelectSingleNode("/Sims3Package/DisplayName").Value + ".xml");
            if (File.Exists(filename)) File.Delete(filename);
            BinaryWriter bw = new BinaryWriter(new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write));
            ms.Position = 0;
            bw.Write(ms.ToArray());
            bw.Close();

            System.Xml.XPath.XPathNodeIterator packagedFiles = nav.Select("/Sims3Package/PackagedFile");
            while(packagedFiles.MoveNext())
            {
                filename = Path.Combine(target, packagedFiles.Current.SelectSingleNode("Name").Value);
                if (File.Exists(filename)) File.Delete(filename);
                bw = new BinaryWriter(new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write));
                fs.Position = basePos + Convert.ToInt64(packagedFiles.Current.SelectSingleNode("Offset").Value);
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

            /*string[] pathElements = source.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar });

            string DisplayName = pathElements[pathElements.Length - 1];

            string filename = Path.Combine(target, DisplayName + ".Sims3Pack");

            if (File.Exists(filename)) File.Delete(filename);
            FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fs);
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Encoding = System.Text.Encoding.UTF8;
            xws.Indent = true;
            xws.OmitXmlDeclaration = false;/**/
        }
    }
}
