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
    public static class FileTable
    {
        public static PathPackageTuple Current { get; set; }

        public static bool UseCustomContent { get; set; }
        static List<PathPackageTuple> cc = null;
        public static List<PathPackageTuple> CustomContent { get { if (cc == null) cc = ccGetList(ObjectCloner.Properties.Settings.Default.CustomContent); return cc; } }
        //Inge (15-01-2011): "Only ever look in *.package for custom content"
        //static List<string> pkgPatterns = new List<string>(new string[] { "*.package", "*.dbc", "*.world", "*.nhd", });
        static List<string> CCpkgPatterns = new List<string>(new string[] { "*.package", });
        static List<PathPackageTuple> ccGetList(string ccPath)
        {
            Diagnostics.Log(String.Format("setCCList scanning {0}", ccPath));
            List<PathPackageTuple> ppts = new List<PathPackageTuple>();

            if (ccPath == null || !Directory.Exists(ccPath)) return ppts;

            //Depth-first search
            foreach (var dir in Directory.GetDirectories(ccPath))
            {
                List<PathPackageTuple> dirPPTs = ccGetList(dir);
                if (dirPPTs != null && dirPPTs.Count > 0)
                {
                    ppts.AddRange(dirPPTs);
                }
            }
            foreach (string pattern in CCpkgPatterns)
                foreach (var path in Directory.GetFiles(ccPath, pattern))
                    try
                    {
                        ppts.Add(new PathPackageTuple(path));
                    }
                    catch (InvalidDataException) { }
            return ppts;
        }

        public static bool AppendFileTable { get; set; }
        static FT _fb0 = new FT("Objects");
        static FT _dds = new FT("Images");
        static FT _tmb = new FT("Thumbnails");
        public static List<PathPackageTuple> fb0 { get { return getList(_fb0.ppt); } }
        public static List<PathPackageTuple> dds { get { return getList(_dds.ppt); } }
        public static List<PathPackageTuple> tmb { get { return getList(_tmb.ppt); } }

        static List<PathPackageTuple> getList(List<PathPackageTuple> which)
        {
            List<PathPackageTuple> res = new List<PathPackageTuple>();
            if (Current != null) { res.Add(Current); }
            if (UseCustomContent) { res.AddRange(CustomContent); }
            if (AppendFileTable) { res.AddRange(which); }
            return res.Count == 0 ? null : res;
        }


        public static void Reset()
        {
            if (Current != null) { s3pi.Package.Package.ClosePackage(0, Current.Package); Current = null; }
            UseCustomContent = false;
            cc = null;
            AppendFileTable = false;
            if (_fb0 != null) _fb0.Reset();
            if (_dds != null) _dds.Reset();
            if (_tmb != null) _tmb.Reset();
            _fb0 = new FT("Objects");
            _dds = new FT("Images");
            _tmb = new FT("Thumbnails");
        }

        public static bool IsOK { get { return fb0 != null && dds != null && tmb != null && fb0.Count > 0; } }

        public static bool IsEqual(List<PathPackageTuple> ppt1, List<PathPackageTuple> ppt2)
        {
            if (ppt1 == null)
                return ppt2 == null;
            else if (ppt2 == null)
                return ppt1 == null;
            if (ppt1.Count != ppt2.Count) return false;
            for (int i = 0; i < ppt1.Count; i++) if (ppt1[i] != ppt2[i]) return false;
            return true;
        }
    }

    class FT
    {
        string iniPath;
        public FT(string iniPath) { this.iniPath = iniPath; }

        List<string> _ini = null;
        List<string> ini { get { if (_ini == null) _ini = iniGetPath(iniPath); return _ini; } }

        List<PathPackageTuple> _ppt = null;
        public List<PathPackageTuple> ppt { get { if (_ppt == null) _ppt = ini.ConvertAll<PathPackageTuple>(path => new PathPackageTuple(path)); return _ppt; } }

        public void Reset() { if (_ppt != null) { _ppt.ForEach(x => s3pi.Package.Package.ClosePackage(0, x.Package)); _ppt = null; } }

        static List<string> iniGetPath(string path)
        {
            List<string> res = new List<string>();
            foreach (S3ocSims3 sims3 in s3ocTTL.lS3ocSims3)
            {
                if (!SettingsForms.GameFolders.IsEnabled(sims3)) continue;
                foreach (string p in s3ocTTL.GetPath(SettingsForms.GameFolders.gameFolders[sims3], sims3, path))
                    if (File.Exists(p)) res.Add(p);
            }
            return res;
        }
    }
}
