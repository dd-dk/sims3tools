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
using s3pi.Interfaces;

namespace ObjectCloner
{
    public class NameMap
    {
        SpecificResource latest;
        List<IDictionary<ulong, string>> namemaps;
        public NameMap(List<PathPackageTuple> nameMapPPTs)
        {
            namemaps = new List<IDictionary<ulong, string>>();
            if (nameMapPPTs == null) return;
            nameMapPPTs.ForEach(ppt => namemaps.AddRange(ppt
                    .FindAll(rie => rie.ResourceType == 0x0166038C)
                    .ConvertAll<IDictionary<ulong, string>>(sr =>
                    {
                        if (sr.Resource as IDictionary<ulong, string> == null) Diagnostics.Log(String.Format("Package {0} ResourceIndexEntry {1} is not a NameMap (accoring to s3pi).", ppt.Path, sr.ResourceIndexEntry));
                        else if (latest == null) latest = sr;
                        return sr.Resource as IDictionary<ulong, string>;
                    })
                    .FindAll(nm => nm != null)
            ));
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
                    nmap = new NameMap(FileTable.fb0);
                return nmap;
            }
        }
        public static bool IsOK { get { return NMap != null && NMap.map != null && NMap.map.Count > 0; } }
    }
}