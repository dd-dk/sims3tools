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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using s3pi.Interfaces;

namespace ObjectCloner
{
    public class Exclusions
    {
        static string ExclusionsList = "ExclusionsList.txt"; // Filename in folder containing assembly containing class

        static List<MatchRK> ExcludedResources = null;

        struct MatchRK
        {
            public uint? ResourceType;
            public uint? ResourceGroup;
            public ulong? Instance;
        }

        static Exclusions()
        {
            string folder = Path.GetDirectoryName(typeof(Exclusions).Assembly.Location);
            string exclusionsList = Path.Combine(folder, ExclusionsList);
            List<MatchRK> ExcludedResources = new List<MatchRK>();

            if (!File.Exists(exclusionsList)) return;
            using (TextReader tr = new StreamReader(exclusionsList))
            {
                string line = null;
                while ((line = tr.ReadLine()) != null)
                {
                    string[] split = line.Split(new char[] { '#', ';', }, 2);
                    if (split[0].Length == 0) continue;

                    line = split[0];
                    split = line.Split(new char[] { ',', }, 4);
                    if (split.Length != 4) continue;

                    MatchRK mrk = new MatchRK();
                    uint tg = 0;
                    ulong i = 0;
                    if (split[0].Trim() == "*") mrk.ResourceType = null;
                    else if (!split[0].Trim().ToLower().StartsWith("0x") || !uint.TryParse(split[0].Trim().Substring(2), System.Globalization.NumberStyles.HexNumber, null, out tg))
                        continue;
                    mrk.ResourceType = tg;
                    if (split[1] == "*") mrk.ResourceGroup = null;
                    else if (!split[1].Trim().ToLower().StartsWith("0x") || !uint.TryParse(split[1].Trim().Substring(2), System.Globalization.NumberStyles.HexNumber, null, out tg))
                        continue;
                    mrk.ResourceGroup = tg;
                    if (split[2] == "*") mrk.Instance = null;
                    else if (!split[2].Trim().ToLower().StartsWith("0x") || !ulong.TryParse(split[2].Trim().Substring(2), System.Globalization.NumberStyles.HexNumber, null, out i))
                        continue;
                    mrk.Instance = i;
                }
                tr.Close();
            }
        }

        public static bool Contains(IResourceKey rk)
        {
            return ExcludedResources != null && ExcludedResources.Exists(ex =>
                (!ex.ResourceType.HasValue || ex.ResourceType.Value == rk.ResourceType) &&
                (!ex.ResourceGroup.HasValue || ex.ResourceGroup.Value == rk.ResourceGroup) &&
                (!ex.Instance.HasValue || ex.Instance.Value == rk.Instance)
                );
        }
    }
}
