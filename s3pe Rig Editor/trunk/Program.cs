/***************************************************************************
 *  Copyright (C) 2010 by http://code.google.com/u/dd764ta/                *
 *                                                                         *
 *  Copyright (C) 2010 by Peter L Jones                                    *
 *  pljones@users.sf.net                                                   *
 *                                                                         *
 *  This helper is based on Atavera's RIG Exporter and is part of s3pi.    *
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
using System.Windows.Forms;
using s3pi.DemoPlugins;

namespace RigEditor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(params string[] args)
        {
#if DEBUG
            if (args.Length == 0)
                Clipboard.SetData(DataFormats.Serializable, new s3piwrappers.RigResource(0, null).Stream);
#endif
            return RunHelper.Run(typeof(MainForm), args);
        }
    }
}
