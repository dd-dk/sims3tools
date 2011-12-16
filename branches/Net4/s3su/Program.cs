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
using System.Windows.Forms;

namespace S3Pack
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(params string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool allowOption = !(new string[] { "unpack", "repack", "pack", }).Contains(Path.GetFileNameWithoutExtension(Application.ExecutablePath).ToLower());
            string filename = null;
            int value = CmdLine(allowOption, ref filename, args);
            if (value == -1)
                return -1;
            if (!allowOption && value >= 0) // option specified
            {
                CopyableMessageBox.Show("Invalid command line: '" + Environment.CommandLine + "'.", "s3su", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Error);
                return -1;
            }
            if (!allowOption) switch (Path.GetFileNameWithoutExtension(Application.ExecutablePath).ToLower())
                {
                    case "unpack": value = 0; break;
                    case "repack": value = 1; break;
                    case "pack": value = 2; break;
                    default:
                        CopyableMessageBox.Show("Invalid command line: '" + Environment.CommandLine + "'.", "s3su", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Error);
                        return -1;
                }
            else if (value < 0)
            {
                ChooseMode cm = new ChooseMode();
                cm.ShowDialog();
                if (cm.Mode == -1)
                    return 0;
                value = cm.Mode;
            }

            switch (value)
            {
                case 0: Application.Run(filename == null ? new Unpack() : new Unpack(filename)); break;
                case 1: Application.Run(filename == null ? new Repack() : new Repack(filename)); break;
                case 2: Application.Run(filename == null ? new Pack() : new Pack(filename)); break;
            }
            return 0;
        }

        static int CmdLine(bool allowOption, ref string filename, params string[] args)
        {
            List<string> files = new List<string>();
            List<string> cmdline = new List<string>(args);
            int mode = -2;
            while (cmdline.Count > 0)
            {
                string option = cmdline[0];
                cmdline.RemoveAt(0);

                if (option.StartsWith("/") || option.StartsWith("-"))
                {
                    option = option.Substring(1);
                    switch (option.ToLower())
                    {
                        case "unpack": if (allowOption) mode = 0; break;
                        case "repack": if (allowOption) mode = 1; break;
                        case "pack": if (allowOption) mode = 2; break;
                        default:
                            CopyableMessageBox.Show("Invalid command line option: '" + option + "'", "s3su", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Error);
                            return -1;
                    }
                }
                else
                {
                    if (files.Count == 0)
                    {
                        if (!File.Exists(option))
                        {
                            CopyableMessageBox.Show("File not found:\n" + option, "s3su", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Error);
                            return -1;
                        }
                        files.Add(option);
                        filename = option;
                    }
                    else
                    {
                        CopyableMessageBox.Show("Can only accept one file name argument.", "s3su", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Error);
                        return -1;
                    }
                }
            }
            if (allowOption && mode < 0 && files.Count > 0)
            {
                switch (Path.GetExtension(filename).ToLower())
                {
                    case ".sims3pack": mode = 0; break;
                    case ".xml": mode = 1; break;
                    case ".package": mode = 2; break;
                }
            }
            return mode;
        }
    }
}
