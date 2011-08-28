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
using SemWeb;

namespace ObjectCloner
{
    public class s3ocTTL
    {
        static readonly string s3octerms = "http://sims3.drealm.info/s3octerms/1.0#";
        static MemoryStore s3oc_ini = new MemoryStore();
        #region Define entities
        static readonly string RDF = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";
        //static readonly string RDFS = "http://www.w3.org/2000/01/rdf-schema#";
        static readonly Entity rdftype = RDF + "type";
        static readonly Entity rdf_first = RDF + "first";
        static readonly Entity rdf_rest = RDF + "rest";
        static readonly Entity rdf_nil = RDF + "nil";
        static readonly Entity typeResourceList = s3octerms + "ResourceList";
        static readonly Entity predHasResource = s3octerms + "hasResource";
        static readonly Entity subjectWin64 = s3octerms + "Win64";
        static readonly Entity subjectWin32 = s3octerms + "Win32";
        static readonly Entity typeMicrosoftOS = s3octerms + "MicrosoftOS";
        static readonly Entity predHasProgramFiles = s3octerms + "hasProgramFiles";
        static readonly Entity typeSims3 = s3octerms + "Sims3";
        static readonly Entity predHasName = s3octerms + "hasName";
        static readonly Entity predHasLongname = s3octerms + "hasLongname";
        static readonly Entity predHasDefaultInstallDir = s3octerms + "hasDefaultInstallDir";
        static readonly Entity predHasPriority = s3octerms + "hasPriority";
        static readonly Entity predIsSuppressed = s3octerms + "isSuppressed";
        static readonly Entity predHasPackages = s3octerms + "hasPackages";
        static readonly Entity predHasRGVersion = s3octerms + "hasRGVersion";
        static readonly Entity predIn = s3octerms + "in";
        static readonly Entity predPackages = s3octerms + "packages";
        #endregion

        public static List<S3ocSims3> lS3ocSims3 = new List<S3ocSims3>();

        public static Dictionary<byte, string> RGVersionLookup = new Dictionary<byte, string>();

        static s3ocTTL()
        {
            string iniFile = Path.Combine(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), "s3oc-ini.ttl");
            s3oc_ini.Import(new N3Reader(iniFile));

            lS3ocSims3 = new List<S3ocSims3>();
            foreach (Statement s in s3oc_ini.Select(new Statement(null, rdftype, typeSims3)))
            {
                S3ocSims3 sims3 = new S3ocSims3(s.Subject);
                lS3ocSims3.Add(sims3);
                bool seenHasRGVersion = false;

                foreach (Statement t in s3oc_ini.Select(new Statement(s.Subject, null, null)))
                {
                    if (t.Predicate.Equals(predHasName)) { sims3.hasName = getString(t.Object); continue; }
                    if (t.Predicate.Equals(predHasLongname)) { sims3.hasLongname = getString(t.Object); continue; }
                    if (t.Predicate.Equals(predHasDefaultInstallDir)) { sims3.hasDefaultInstallDir = getHasDefaultInstallDir(prefix, t.Object); continue; }
                    if (t.Predicate.Equals(predHasPriority)) { sims3.hasPriority = getHasPriority(t.Object); continue; }
                    if (t.Predicate.Equals(predHasRGVersion)) { sims3.hasRGVersion = getHasRGVersion(t.Object); seenHasRGVersion = true; continue; }
                    if (t.Predicate.Equals(predIsSuppressed)) { sims3.isSuppressed = getIsSuppressed(t.Object); continue; }
                    if (t.Predicate.Equals(predHasPackages)) { sims3.hasPackages.Add(t.Object as Entity); continue; }
                    if (!sims3.otherStatements.Contains(t.Predicate))
                        sims3.otherStatements.Add(t.Predicate);
                }
                if (seenHasRGVersion && sims3.hasRGVersion > 0) RGVersionLookup.Add((byte)sims3.hasRGVersion, sims3.hasName);
            }
            lS3ocSims3.Sort((x, y) => y.hasPriority.CompareTo(x.hasPriority));
        }

        static string getString(Resource value)
        {
            if (value as Literal == null) return null;
            object o = ((Literal)value).ParseValue();
            if (!o.GetType().Equals(typeof(string))) return null;
            return (!o.GetType().Equals(typeof(string))) ? null : (string)o;
        }

        static int getIsSuppressed(Resource value)
        {
            if (value as Literal == null) return 0;
            object o = ((Literal)value).ParseValue();
            if (!o.GetType().Equals(typeof(string))) return -1;
            string s = (string)o;
            if (s.Equals("not-allowed")) return -1;
            if (s.Equals("false")) return 0;
            return 1;
        }

        static int getHasPriority(Resource value)
        {
            if (value as Literal == null) return 0;
            object o = ((Literal)value).ParseValue();
            if (!o.GetType().Equals(typeof(Decimal))) return 0;
            return Convert.ToInt32((Decimal)o);
        }

        static int getHasRGVersion(Resource value)
        {
            if (value as Literal == null) return 0;
            object o = ((Literal)value).ParseValue();
            if (!o.GetType().Equals(typeof(Decimal))) return 0;
            return Convert.ToInt32((Decimal)o) & 0x1F;
        }

        static string getHasDefaultInstallDir(string prefix, Resource value)
        {
            string s = getString(value);
            return (s == null || !Directory.Exists(prefix + s)) ? null : prefix + s;
        }

        static string _prefix = null;
        static string prefix
        {
            get
            {
                if (_prefix == null)
                {
                    foreach (Statement s in s3oc_ini.Select(new Statement(subjectWin64, rdftype, typeMicrosoftOS)))
                    {
                        foreach (Statement t in s3oc_ini.Select(new Statement(s.Subject, predHasProgramFiles, null)))
                        {
                            if (Directory.Exists(getString(t.Object)))
                                _prefix = getString(t.Object);
                        }
                    }
                    if (_prefix == null)
                        foreach (Statement s in s3oc_ini.Select(new Statement(subjectWin32, rdftype, typeMicrosoftOS)))
                        {
                            foreach (Statement t in s3oc_ini.Select(new Statement(s.Subject, predHasProgramFiles, null)))
                            {
                                if (Directory.Exists(getString(t.Object)))
                                    _prefix = getString(t.Object);
                            }
                        }
                    if (_prefix == null)
                    {
                        try
                        {
                            _prefix = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                        }
                        catch (PlatformNotSupportedException) { _prefix = "/"; }
                    }
                }
                return _prefix;
            }
        }

        //"Common resources" list
        static List<Dictionary<String, TypedValue>> _lS3ocResourceList = null;
        public List<Dictionary<String, TypedValue>> lS3ocResourceList
        {
            get
            {
                if (_lS3ocResourceList == null)
                {
                    _lS3ocResourceList = new List<Dictionary<string, TypedValue>>();
                    foreach (Statement s in s3oc_ini.Select(new Statement(null, rdftype, typeResourceList)))
                    {
                        foreach (Statement t in s3oc_ini.Select(new Statement(s.Subject, predHasResource, null)))
                        {
                            Entity e = t.Object as Entity;
                            String[] predicates = new String[] { "T", "G", "I", };
                            String[] keys = new String[] { "ResourceType", "ResourceGroup", "Instance", };
                            Type[] types = new Type[] { typeof(uint), typeof(uint), typeof(ulong), };
                            Dictionary<String, TypedValue> dict = new Dictionary<string, TypedValue>();

                            for (int i = 0; i < predicates.Length; i++)
                            {
                                Literal l = s3oc_ini.Find(e, s3octerms + predicates[i]) as Literal;
                                if (l == null) continue;
                                ulong value = ulong.Parse((l.ParseValue() as String).Substring(2), System.Globalization.NumberStyles.HexNumber);
                                dict.Add(keys[i], new TypedValue(types[i], Convert.ChangeType(value, types[i]), "X"));
                            }
                            _lS3ocResourceList.Add(dict);
                        }
                    }
                }
                return _lS3ocResourceList;
            }
        }

        public static List<string> GetPath(string folder, S3ocSims3 sims3, string path)
        {
            List<string> res = new List<string>();

            foreach (Entity e in sims3.hasPackages)
            {
                Resource r = s3ocTTL.s3oc_ini.Find(e, rdftype);
                if (path != null) if (r == null || !r.Equals((Entity)(s3octerms + path))) continue;

                Entity eList = s3ocTTL.s3oc_ini.Find(e, predPackages) as Entity;
                if (eList == null) continue;

                r = s3ocTTL.s3oc_ini.Find(e, predIn);
                string subFolder = (r != null && r as Literal != null && ((Literal)r).ParseValue().GetType().Equals(typeof(string)))
                    ? (string)((Literal)r).ParseValue() : "";

                string gp_prefix = Path.Combine(folder == null ? "" : folder, subFolder);
                //if (gp_prefix.Length > 0 && !Directory.Exists(gp_prefix)) continue;

                r = s3ocTTL.s3oc_ini.Find(eList, rdf_first);
                while (r != null && r as Literal != null && ((Literal)r).ParseValue().GetType().Equals(typeof(string)))
                {
                    res.Add(Path.Combine(gp_prefix, (string)((Literal)r).ParseValue()));
                    eList = s3ocTTL.s3oc_ini.Find(eList, rdf_rest) as Entity;
                    if (eList == null || eList == rdf_nil) break;
                    r = s3ocTTL.s3oc_ini.Find(eList, rdf_first);
                }
            }
            return res;
        }

        public Resource GetOtherStatement(S3ocSims3 sims3, Entity Predicate)
        {
            return sims3.otherStatements != null && sims3.otherStatements.Contains(Predicate) ? s3ocTTL.s3oc_ini.Find(s3octerms + "#" + sims3.subjectName, Predicate) : null;
        }


#if DEBUG
        class StatementPrinter : StatementSink
        {
            public bool Add(Statement assertion)
            {
                Console.WriteLine(assertion.ToString());
                return true;
            }
        }
#endif
    }

    public class S3ocSims3
    {
        string uri;
        public string subjectName;
        public string hasName;
        public string hasLongname;
        public string hasDefaultInstallDir;
        public int hasPriority;
        public int hasRGVersion;
        public int isSuppressed; // -1: not-allowed; 0: false; else true
        public List<Entity> hasPackages = new List<Entity>();
        public List<Entity> otherStatements = new List<Entity>();

        public S3ocSims3(Entity Subject) { subjectName = Subject.Uri.Split(new char[] { '#' }, 2)[1]; uri = Subject.Uri; }

        public override string ToString() { return hasLongname; }

        public static S3ocSims3 byName(string name) { return s3ocTTL.lS3ocSims3.Find(sims3 => sims3.subjectName.Equals(name)); }
    }

    public static class SemWebExtensions
    {
        public static Resource Find(this Store value, Entity Subject, Entity Predicate)
        {
            foreach (Statement r in value.Select(new Statement(Subject, Predicate, null)))
                return r.Object;
            return null;
        }
        public static List<Resource> FindAll(this Store value, Entity Subject, Entity Predicate)
        {
            List<Resource> res = new List<Resource>();
            foreach (Statement r in value.Select(new Statement(Subject, Predicate, null)))
                res.Add(r.Object);
            return res;
        }
    }
}
