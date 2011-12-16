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
using System.Linq;
using System.IO;
using s3pi.Interfaces;
using s3pi.Filetable;

namespace ObjectCloner
{
    public class NameMap
    {
        SpecificResource latest = null;
        List<IDictionary<ulong, string>> namemaps;
        public NameMap(List<PathPackageTuple> nameMapPPTs)
        {
            namemaps = new List<IDictionary<ulong, string>>();
            if (nameMapPPTs == null) return;
            foreach (var sr in nameMapPPTs.SelectMany(ppt => ppt.FindAll(rie => rie.ResourceType == 0x0166038C)))
            {
                IDictionary<ulong, string> nmp = sr.Resource as IDictionary<ulong, string>;
                if (nmp == null) continue;
                if (latest == null) latest = sr;
                namemaps.Add(nmp);
            }
        }
        public string this[ulong instance]
        {
            get
            {
                foreach (IDictionary<ulong, string> namemap in namemaps)
                    if (namemap.ContainsKey(instance))
                        return namemap[instance];
                return null;
            }
        }
        public IResourceKey rk { get { return latest == null ? RK.NULL : latest.RequestedRK; } }
        public IDictionary<ulong, string> map { get { return latest == null ? null : (IDictionary<ulong, string>)latest.Resource; } }
        public void Commit() { latest.Commit(); }

        static NameMap nmap;
        public static void Reset() { nmap = null; }
        public static NameMap NMap
        {
            get
            {
                if (nmap == null)
                    nmap = new NameMap(FileTable.GameContent);
                return nmap;
            }
        }
        public static bool IsOK { get { return NMap != null && NMap.map != null && NMap.map.Count > 0; } }
    }
}