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
using System.Diagnostics;
using System.IO;
using s3pi.Interfaces;
using System.Windows.Forms;

namespace ObjectCloner {
    static class Diagnostics
    {
        static bool popups = false;
        public static bool Popups { get { return popups; } set { popups = value; } }
        public static void Show(string value, string title = "")
        {
            string msg = value.Trim('\n');
            if (msg == "") return;
            if (title == "")
            {
                Debug.WriteLine(msg);
                if (popups) CopyableMessageBox.Show(msg);
            }
            else
            {
                Debug.WriteLine(String.Format("{0}: {1}", title, msg));
                if (popups) CopyableMessageBox.Show(msg, title);
            }
            if (logging) Log(value);
        }

        static bool logging = false;
        static StreamWriter logFile = null;
        static void OpenLogFile()
        {
            string filename = String.Format(Path.GetTempPath() + "s3oc-{0}.log", DateTime.UtcNow.ToString("s").Replace(":", ""));
            logFile = new StreamWriter(new FileStream(filename, FileMode.Create));
        }
        public static bool Logging
        {
            get { return logging; }
            set
            {
                logging = value;
                if (!logging) { if (logFile != null) { logFile.Close(); logFile = null; } }
            }
        }
        public static void Log(string value)
        {
            string msg = value.Trim('\n');
            if (msg == "") return;
            if (!popups) Debug.WriteLine(msg);
            if (logging) { if (logFile == null) OpenLogFile(); logFile.WriteLine("{0}: {1}", DateTime.UtcNow.ToString("s"), msg); logFile.Flush(); }
        }
    }
}