﻿/***************************************************************************
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
using System.Reflection;
using System.Windows.Forms;

namespace meshExpImp.Helper
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(params string[] args)
        {
            List<string> largs = new List<string>(args);

            bool export = largs.Contains("/export");
            if (export) largs.Remove("/export");

            bool import = largs.Contains("/import");
            if (import) largs.Remove("/import");

            if ((export && import) || (!export && !import))
            {
                CopyableMessageBox.Show("Missing /export or /import on command line.", Application.ProductName, CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Error);
                Environment.Exit(1);
            }

            args = largs.ToArray();

            Filename = (args.Length > 0 ? Path.GetFileNameWithoutExtension(args[args.Length - 1]) : "*");

#if DEBUG
            if (args.Length == 0)
            {
                FileStream fs = new FileStream(Application.ExecutablePath, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                s3pi.GenericRCOLResource.GenericRCOLResource modl = new s3pi.GenericRCOLResource.GenericRCOLResource(0, null);
                modl.ChunkEntries.Add(typeof(meshExpImp.ModelBlocks.MODL));
                Clipboard.SetData(DataFormats.Serializable, modl.Stream);
                br.Close();
                fs.Close();
            }
#endif

            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            return s3pi.Helpers.RunHelper.Run(export ? typeof(ExportForm) : typeof(ImportForm), args);
        }

        public static string Filename { get; private set; }
        public static string GetShortName()
        {
            //S3_01661233_00000001_0000000000D0F4DF_doorSingleTransomContemporary%%+MODL.model
            string[] split = Program.Filename.Split(new char[] { '_', }, 5);
            if (split.Length == 5)
            {
                string[] latter = split[4].Split(new char[] { '%', }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (latter.Length == 2)
                    return string.Format("grp{0}_{1}{2}", split[2], latter[0], Path.GetExtension(Program.Filename));
            }
            return Filename;
        }
    }
}
