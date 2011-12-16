/***************************************************************************
 *  Copyright (C) 2010 by Peter L Jones                                    *
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

namespace S3SA_DLL_ExpImp
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

            if ((!export && !import) || (export && import))
            {
                CopyableMessageBox.Show("Invalid command line.", Application.ProductName, CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Error);
                Environment.Exit(1);
            }

            args = largs.ToArray();

#if DEBUG
            if (args.Length == 0)
            {
                FileStream fs = new FileStream(Application.ExecutablePath, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                Clipboard.SetData(DataFormats.Serializable, new ScriptResource.ScriptResource(0, null) { Assembly = br }.Stream);
                br.Close();
                fs.Close();
            }
#endif

            return s3pi.Helpers.RunHelper.Run(export ? typeof(Export) : typeof(Import), args);
        }

        public static string getAssemblyName(ScriptResource.ScriptResource s3sa)
        {
            try
            {
                byte[] data = new byte[s3sa.Assembly.BaseStream.Length];
                s3sa.Assembly.BaseStream.Read(data, 0, data.Length);
                Assembly assy = Assembly.Load(data);

                return assy.FullName.Split(',')[0] + ".dll";
            }
            catch
            {
            }
            return "*.dll";
        }
    }
}
