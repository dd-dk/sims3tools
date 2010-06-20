﻿/***************************************************************************
 *  Copyright (C) 2009, 2010 by Peter L Jones                              *
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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Windows.Forms;
using s3pi.Interfaces;
using s3pi.Extensions;
using s3pi.GenericRCOLResource;
using ObjectCloner.TopPanelComponents;
using SemWeb;

namespace ObjectCloner
{
    public partial class MainForm : Form
    {
        #region Static bits
        static string myName = "s3oc";
        static bool disableCompression;
        static Dictionary<View, MenuBarWidget.MB> viewMap;
        static List<View> viewMapKeys;
        static List<MenuBarWidget.MB> viewMapValues;

        static string language_fmt = "Strings_{0}_{1:x2}{2:x14}";
        static string[] languages = new string[] {
            "ENG_US", "CHI_CN", "CHI_TW", "CZE_CZ",
            "DAN_DK", "DUT_NL", "FIN_FI", "FRE_FR",
            "GER_DE", "GRE_GR", "HUN_HU", "ITA_IT",
            "JAP_JP", "KOR_KR", "NOR_NO", "POL_PL",

            "POR_PT", "POR_BR", "RUS_RU", "SPA_ES",
            "SPA_MX", "SWE_SE", "THA_TH",
        };

        static Image defaultThumbnail =
            Image.FromFile(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Resources/defaultThumbnail.png"),
            true).GetThumbnailImage(256, 256, gtAbort, IntPtr.Zero);

        static Dictionary<string, List<string>> s3ocIni;
        static MainForm()
        {
            viewMap = new Dictionary<View, MenuBarWidget.MB>();
            viewMap.Add(View.Tile, MenuBarWidget.MB.MBV_tiles);
            viewMap.Add(View.LargeIcon, MenuBarWidget.MB.MBV_largeIcons);
            viewMap.Add(View.SmallIcon, MenuBarWidget.MB.MBV_smallIcons);
            viewMap.Add(View.List, MenuBarWidget.MB.MBV_list);
            viewMap.Add(View.Details, MenuBarWidget.MB.MBV_detailedList);
            viewMapKeys = new List<View>(viewMap.Keys);
            viewMapValues = new List<MenuBarWidget.MB>(viewMap.Values);

            LoadIni();
            LoadTTL();
            LoadEPsDisabled();
            LoadGameDirSettings();
        }

        static List<string> ePsDisabled = new List<string>();
        private static void LoadEPsDisabled() { ePsDisabled = new List<string>(ObjectCloner.Properties.Settings.Default.EPsDisabled.Split(';')); }
        private static void SaveEPsDisabled() { ObjectCloner.Properties.Settings.Default.EPsDisabled = String.Join(";", ePsDisabled.ToArray()); }

        static Dictionary<string, string> gameDirs = new Dictionary<string, string>();
        private static void LoadGameDirSettings()
        {
            gameDirs = new Dictionary<string, string>();
            foreach (string s in ObjectCloner.Properties.Settings.Default.InstallDirs.Split(';'))
            {
                string[] p = s.Split(new char[] { '=' }, 2);
                if (S3ocSims3.byName(p[0]) != null && Directory.Exists(p[1]))
                    gameDirs.Add(p[0], p[1]);
            }
        }
        private static void SaveGameDirSettings()
        {
            string value = "";
            foreach (var kvp in gameDirs)
                value += ";" + kvp.Key + "=" + kvp.Value;
            ObjectCloner.Properties.Settings.Default.InstallDirs = value.TrimStart(';');
        }

        /// <summary>
        /// This static method loads the old .ini file
        /// </summary>
        static void LoadIni()
        {
            s3ocIni = new Dictionary<string, List<string>>();
            string file = Path.Combine(Path.GetDirectoryName(typeof(MainForm).Assembly.Location), "s3oc.ini");
            if (File.Exists(file))
            {
                StreamReader sr = new StreamReader(file);
                string s;
                while ((s = sr.ReadLine()) != null)
                {
                    if (s.StartsWith("#") || s.StartsWith(";") || s.StartsWith("//") || s.Trim().Length == 0) continue;
                    string[] t = s.Split(new char[] { ':' }, 2);
                    if (t.Length != 2) continue;
                    string key = t[0].Trim().ToLower();
                    if (s3ocIni.ContainsKey(key)) continue;
                    s3ocIni.Add(key, new List<string>());
                    t = t[1].Split(',');
                    foreach (string u in t) s3ocIni[key].Add(u.Trim());
                }
                sr.Close();
            }
        }

        #region LoadTTL
        static MemoryStore s3oc_ini = new MemoryStore();
        static readonly string s3octerms = "http://sims3.drealm.info/s3octerms/1.0#";
        static readonly string RDF = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";
        //static readonly string RDFS = "http://www.w3.org/2000/01/rdf-schema#";
        static readonly Entity rdftype = RDF + "type";
        static readonly Entity rdf_first = RDF + "first";
        static readonly Entity rdf_rest = RDF + "rest";
        static readonly Entity rdf_nil = RDF + "nil";
        static readonly Entity typeResourceList = s3octerms + "ResourceList";
        static readonly Entity predHasResource = s3octerms + "hasResource";
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

        public class S3ocSims3
        {
            public string subjectName;
            public string hasName;
            public string hasLongname;
            public string hasDefaultInstallDir;
            public int hasPriority;
            public int hasRGVersion;
            public int isSuppressed; // -1: not-allowed; 0: false; else true
            public List<Entity> hasPackages = new List<Entity>();
            public List<Entity> otherStatements = new List<Entity>();

            public S3ocSims3(Entity Subject) { subjectName = Subject.Uri.Split(new char[] { '#' }, 2)[1]; }

            public bool Enabled
            {
                get
                {
                    return ePsDisabled.Contains(subjectName) ? false : isSuppressed < 1;
                }
                set
                {
                    if (isSuppressed != 0 || Enabled == value) return;
                    if (value)
                        ePsDisabled.Remove(subjectName);
                    else
                        ePsDisabled.Add(subjectName);
                    SaveEPsDisabled();
                }
            }
            public string InstallDir
            {
                get
                {
                    return gameDirs.ContainsKey(subjectName) ? gameDirs[subjectName] : hasDefaultInstallDir;
                }
                set
                {
                    if (safeGetFullPath(InstallDir) == safeGetFullPath(value)) return;

                    if (gameDirs.ContainsKey(subjectName))
                    {
                        if (safeGetFullPath(hasDefaultInstallDir) == safeGetFullPath(value))
                            gameDirs.Remove(subjectName);
                        else
                            gameDirs[subjectName] = value == null ? "" : value;
                    }
                    else
                        gameDirs.Add(subjectName, value);
                    SaveGameDirSettings();
                }
            }
            string safeGetFullPath(string value) { return value == null ? null : Path.GetFullPath(value); }

            public List<string> getPackages(string type)
            {
                if (hasPackages == null) return null;
                foreach (Entity e in hasPackages)
                {

                }
                return null;
            }

            public Resource getOtherStatement(Entity Predicate)
            {
                if (otherStatements == null) return null;
                foreach (Entity e in this.otherStatements)
                    if (e.Equals(Predicate)) return rdfFetch(s3oc_ini, s3octerms + subjectName, Predicate);
                return null;
            }

            public override string ToString() { return hasLongname; }

            public static S3ocSims3 byName(string name)
            {
                foreach (S3ocSims3 sims3 in lS3ocSims3) if (sims3.subjectName.Equals(name)) return sims3;
                return null;
            }
        }
        public static List<Dictionary<String, TypedValue>> lS3ocResourceList = new List<Dictionary<String, TypedValue>>();
        public static List<S3ocSims3> lS3ocSims3 = new List<S3ocSims3>();
        public static Dictionary<byte, string> RGVersionLookup = new Dictionary<byte, string>();

        /// <summary>
        /// This static method loads the new Turtle resource definition file
        /// </summary>
        static void LoadTTL()
        {
            string iniFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "s3oc-ini.ttl");
            s3oc_ini.Import(new N3Reader(iniFile));

            lS3ocResourceList = new List<Dictionary<String, TypedValue>>();
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
                        Literal l = rdfFetch(s3oc_ini, e, s3octerms + predicates[i]) as Literal;
                        if (l == null) continue;
                        ulong value = ulong.Parse((l.ParseValue() as String).Substring(2), System.Globalization.NumberStyles.HexNumber);
                        dict.Add(keys[i], new TypedValue(types[i], Convert.ChangeType(value, types[i]), "X"));
                    }
                    lS3ocResourceList.Add(dict);
                }
            }

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
                    if (t.Predicate.Equals(predHasDefaultInstallDir)) { sims3.hasDefaultInstallDir = getHasDefaultInstallDir(t.Object); continue; }
                    if (t.Predicate.Equals(predHasPriority)) { sims3.hasPriority = getHasPriority(t.Object); continue; }
                    if (t.Predicate.Equals(predHasRGVersion)) { sims3.hasRGVersion = getHasRGVersion(t.Object); seenHasRGVersion = true; continue; }
                    if (t.Predicate.Equals(predIsSuppressed)) { sims3.isSuppressed = getIsSuppressed(t.Object); continue; }
                    if (t.Predicate.Equals(predHasPackages)) { sims3.hasPackages.Add(t.Object as Entity); continue; }
                    if (!sims3.otherStatements.Contains(t.Predicate))
                        sims3.otherStatements.Add(t.Predicate);
                }
                if (seenHasRGVersion && sims3.hasRGVersion > 0) RGVersionLookup.Add((byte)sims3.hasRGVersion, sims3.hasName);
            }
            lS3ocSims3.Sort(reversePriority);
        }
        static int reversePriority(S3ocSims3 x, S3ocSims3 y) { return y.hasPriority.CompareTo(x.hasPriority); }

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

        static string getHasDefaultInstallDir(Resource value)
        {
            string s = getString(value);
            return (s == null || !Directory.Exists(s)) ? null : s;
        }

        static List<string> ini_fb0 { get { return iniGetPath("Objects"); } }
        static List<string> ini_fb2 { get { return iniGetPath("Images"); } }
        static List<string> ini_tmb { get { return iniGetPath("Thumbnails"); } }
        static List<string> iniGetPath(string path)
        {
            List<string> res = new List<string>();
            foreach (S3ocSims3 sims3 in lS3ocSims3)
            {
                if (!sims3.Enabled) continue;
                foreach (string p in iniGetPath(sims3, path))
                    if (File.Exists(p)) res.Add(p);
            }
            return res;
        }

        /// <summary>
        /// Return the "raw" expanded list of a particular type - or all, if path is null
        /// </summary>
        /// <param name="sims3"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<string> iniGetPath(S3ocSims3 sims3, string path)
        {
            if (!sims3.Enabled) return null;

            List<string> res = new List<string>();

            foreach (Entity e in sims3.hasPackages)
            {
                Resource r = rdfFetch(s3oc_ini, e, rdftype);
                if (path != null) if (r == null || !r.Equals((Entity)(s3octerms + path))) continue;

                Entity eList = rdfFetch(s3oc_ini, e, predPackages) as Entity;
                if (eList == null) continue;

                r = rdfFetch(s3oc_ini, e, predIn);
                string subFolder = (r != null && r as Literal != null && ((Literal)r).ParseValue().GetType().Equals(typeof(string)))
                    ? (string)((Literal)r).ParseValue() : "";

                string prefix = Path.Combine(sims3.InstallDir == null ? "" : sims3.InstallDir, subFolder);
                //if (prefix.Length > 0 && !Directory.Exists(prefix)) continue;

                r = rdfFetch(s3oc_ini, eList, rdf_first);
                while (r != null && r as Literal != null && ((Literal)r).ParseValue().GetType().Equals(typeof(string)))
                {
                    res.Add(Path.Combine(prefix, (string)((Literal)r).ParseValue()));
                    eList = rdfFetch(s3oc_ini, eList, rdf_rest) as Entity;
                    if (eList == null || eList == rdf_nil) break;
                    r = rdfFetch(s3oc_ini, eList, rdf_first);
                }
            }
            return res;
        }

        static Resource rdfFetch(Store store, Entity Subject, Entity Predicate)
        {
            foreach (Statement r in store.Select(new Statement(Subject, Predicate, null)))
                return r.Object;
            return null;
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
        #endregion
        #endregion

        enum Mode
        {
            None = 0,
            Clone,
            Fix,
        }
        Mode mode = Mode.None;

        View currentView;

        List<IPackage> objPkgs;
        List<IPackage> ddsPkgs;
        List<IPackage> tmbPkgs;

        Item selectedItem;
        Image replacementForThumbs;

        Dictionary<string, IResourceKey> rkLookup = null;

        private ObjectCloner.TopPanelComponents.ObjectChooser objectChooser;
        private ObjectCloner.TopPanelComponents.ResourceList resourceList;
        private ObjectCloner.TopPanelComponents.PleaseWait pleaseWait;
        private ObjectCloner.TopPanelComponents.CloneFixOptions cloneFixOptions;
        private ObjectCloner.TopPanelComponents.Search searchPane;

        public MainForm()
        {
            InitializeComponent();
            this.Text = myName;
            objectChooser = new ObjectChooser();
            objectChooser.SelectedIndexChanged += new EventHandler(objectChooser_SelectedIndexChanged);
            objectChooser.ItemActivate += new EventHandler(objectChooser_ItemActivate);
            resourceList = new ResourceList();
            pleaseWait = new PleaseWait();

            Diagnostics.Enabled = ObjectCloner.Properties.Settings.Default.Diagnostics;
            menuBarWidget1.Checked(MenuBarWidget.MB.MBS_diagnostics, Diagnostics.Enabled);

        }

        public MainForm(params string[] args)
            : this()
        {
            MainForm_LoadFormSettings();
            CmdLine(args);//In case of conflict, command line overrides settings
#if DEBUG
            cmdlineTest = true;
#endif

            // Settings for test mode
            if (cmdlineTest)
            {
            }

            disableCompression = false;

            SetStepText();

            InitialiseTabs(CatalogType.CatalogProxyProduct);//Use the Proxy Product as it has pretty much nothing on it
            TabEnable(false);
        }

        private void MainForm_LoadFormSettings()
        {
            int h = ObjectCloner.Properties.Settings.Default.PersistentHeight;
            if (h == -1) h = 4 * System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height / 5;
            this.Height = h;

            int w = ObjectCloner.Properties.Settings.Default.PersistentWidth;
            if (w == -1) w = 4 * System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width / 5;
            this.Width = w;

            w = ObjectCloner.Properties.Settings.Default.Splitter1Width;
            if (w > 0 && w < 100)
                splitContainer1.SplitterDistance = this.Width * w / 100;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            View view = Enum.IsDefined(typeof(View), ObjectCloner.Properties.Settings.Default.View)
                ? viewMapKeys[ObjectCloner.Properties.Settings.Default.View]
                : View.Details;
            menuBarWidget1_MBView_Click(null, new MenuBarWidget.MBClickEventArgs(viewMap[view]));

            menuBarWidget1.Checked(MenuBarWidget.MB.MBV_icons, ObjectCloner.Properties.Settings.Default.ShowThumbs);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            AbortLoading(e.CloseReason == CloseReason.ApplicationExitCall);
            AbortFetching(e.CloseReason == CloseReason.ApplicationExitCall);
            AbortSaving(e.CloseReason == CloseReason.ApplicationExitCall);
            ClosePkg();

            objectChooser.ObjectChooser_SaveSettings();

            ObjectCloner.Properties.Settings.Default.PersistentHeight = this.WindowState == FormWindowState.Normal ? this.Height : -1;
            ObjectCloner.Properties.Settings.Default.PersistentWidth = this.WindowState == FormWindowState.Normal ? this.Width : -1;
            ObjectCloner.Properties.Settings.Default.Splitter1Width = splitContainer1.SplitterDistance * 100 / this.Width;
            ObjectCloner.Properties.Settings.Default.ShowThumbs = menuBarWidget1.IsChecked(MenuBarWidget.MB.MBV_icons);
            ObjectCloner.Properties.Settings.Default.Save();
        }

        void ClosePkg()
        {
            thumb = null;
            english = null;
            haveLoaded = false;
            if (currentPackage != "")
            {
                if (objPkgs.Count > 0) s3pi.Package.Package.ClosePackage(0, objPkgs[0]);
                currentPackage = "";
                ddsPkgs = tmbPkgs = objPkgs = null;
            }
            else
            {
                if (objPkgs != null) foreach (List<IPackage> lpkg in new List<IPackage>[] { objPkgs, tmbPkgs, ddsPkgs, })
                        foreach (IPackage pkg in lpkg) s3pi.Package.Package.ClosePackage(0, pkg);
                ddsPkgs = tmbPkgs = objPkgs = null;
            }
        }

        #region Command Line
        delegate bool CmdLineCmd(ref List<string> cmdline);
        struct CmdInfo
        {
            public CmdLineCmd cmd;
            public string help;
            public CmdInfo(CmdLineCmd cmd, string help) : this() { this.cmd = cmd; this.help = help; }
        }
        Dictionary<string, CmdInfo> Options;
        void SetOptions()
        {
            Options = new Dictionary<string, CmdInfo>();
            Options.Add("test", new CmdInfo(CmdLineTest, "Enable facilities still undergoing initial testing"));
            Options.Add("help", new CmdInfo(CmdLineHelp, "Display this help"));
        }
        void CmdLine(params string[] args)
        {
            SetOptions();
            List<string> pkgs = new List<string>();
            List<string> cmdline = new List<string>(args);
            while (cmdline.Count > 0)
            {
                if (cmdline[0].StartsWith("/") || cmdline[0].StartsWith("-"))
                {
                    string option = cmdline[0].Substring(1);
                    if (Options.ContainsKey(option.ToLower()))
                    {
                        if (Options[option.ToLower()].cmd(ref cmdline))
                            Environment.Exit(0);
                    }
                    else
                    {
                        CopyableMessageBox.Show(this, "Invalid command line option: '" + option + "'",
                            myName, CopyableMessageBoxIcon.Error, new List<string>(new string[] { "OK" }), 0, 0);
                        Environment.Exit(1);
                    }
                }
                else
                    pkgs.Add(cmdline[0]);
                cmdline.RemoveAt(0);
            }
        }
        bool cmdlineTest = false;
        bool CmdLineTest(ref List<string> cmdline) { cmdlineTest = true; return false; }
        bool CmdLineHelp(ref List<string> cmdline)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("The following command line options are available:\n");
            foreach (var kvp in Options)
                sb.AppendFormat("{0}  --  {1}\n", kvp.Key, kvp.Value.help);
            sb.AppendLine("\nOptions must be prefixed with '/' or '-'");

            CopyableMessageBox.Show(this, sb.ToString(), "Command line options", CopyableMessageBoxIcon.Information, new List<string>(new string[] { "OK" }), 0, 0);
            return true;
        }
        #endregion


        string creatorName = null;
        string CreatorName
        {
            get
            {
                if (creatorName == null)
                {
                    if (ObjectCloner.Properties.Settings.Default.CreatorName == null || ObjectCloner.Properties.Settings.Default.CreatorName.Length == 0)
                        settingsUserName();
                }
                if (ObjectCloner.Properties.Settings.Default.CreatorName == null || ObjectCloner.Properties.Settings.Default.CreatorName.Length == 0)
                    creatorName = "";
                else
                    creatorName = ObjectCloner.Properties.Settings.Default.CreatorName;
                return creatorName;
            }
        }


        #region Thumbs
        public class THUM
        {
            static Dictionary<uint, uint[]> thumTypes;
            public static uint[] PNGTypes = new uint[] { 0x2E75C764, 0x2E75C765, 0x2E75C766, };
            public static ushort[] thumSizes = new ushort[] { 32, 64, 128, };
            static uint defType = 0x319E4F1D;
            static THUM()
            {
                thumTypes=new Dictionary<uint,uint[]>();
                thumTypes.Add(0x319E4F1D, new uint[] { 0x0580A2B4, 0x0580A2B5, 0x0580A2B6, }); //Catalog Object
                thumTypes.Add(0xCF9A4ACE, new uint[] { 0x00000000, 0x00000000, 0x00000000, }); //Modular Resource
                thumTypes.Add(0x0418FE2A, new uint[] { 0x2653E3C8, 0x2653E3C9, 0x2653E3CA, }); //Catalog Fence
                thumTypes.Add(0x049CA4CD, new uint[] { 0x5DE9DBA0, 0x5DE9DBA1, 0x5DE9DBA2, }); //Catalog Stairs
                thumTypes.Add(0x04AC5D93, thumTypes[0x319E4F1D]); //Catalog Proxy Product
                thumTypes.Add(0x04B30669, thumTypes[0x319E4F1D]); //Catalog Terrain Geometry Brush
                thumTypes.Add(0x04C58103, new uint[] { 0x2D4284F0, 0x2D4284F1, 0x2D4284F2, }); //Catalog Railing
                thumTypes.Add(0x04ED4BB2, new uint[] { 0x05B1B524, 0x05B1B525, 0x05B1B526, }); //Catalog Terrain Paint Brush
                thumTypes.Add(0x04F3CC01, new uint[] { 0x05B17698, 0x05B17699, 0x05B1769A, }); //Catalog Fireplace
                thumTypes.Add(0x060B390C, thumTypes[0x319E4F1D]); //Catalog Terrain Water Brush
                thumTypes.Add(0x316C78F2, thumTypes[0x319E4F1D]); //Catalog Foundation
                thumTypes.Add(0x515CA4CD, new uint[] { 0x0589DC44, 0x0589DC45, 0x0589DC46, }); //Catalog Wall/Floor Pattern
                thumTypes.Add(0x9151E6BC, new uint[] { 0x00000000, 0x00000000, 0x00000000, }); //Catalog Wall -- doesn't have any
                thumTypes.Add(0x91EDBD3E, thumTypes[0x319E4F1D]); //Catalog Roof Style
                thumTypes.Add(0xF1EDBD86, thumTypes[0x319E4F1D]); //Catalog Roof Pattern
            }
            public enum THUMSize : int
            {
                small = 0,
                medium,
                large,
                defSize = large,
            }
            List<IPackage> fb0Pkgs;//for PNGInstance
            List<IPackage> tmbPkgs;//for ALLThumbnails
            public THUM(List<IPackage> fb0Pkgs, List<IPackage> tmbPkgs) { this.fb0Pkgs = fb0Pkgs; this.tmbPkgs = tmbPkgs; }
            public Image this[ulong instance] { get { return this[instance, THUMSize.defSize, false]; } }
            public Image this[ulong instance, THUMSize size] { get { return this[instance, size, false]; } set { this[instance, size, false] = value; } }
            public Image this[ulong instance, bool isPNGInstance] { get { return this[instance, THUMSize.defSize, isPNGInstance]; } }
            public Image this[ulong instance, THUMSize size, bool isPNGInstance] { get { return this[defType, instance, size, isPNGInstance]; } set { this[defType, instance, size, isPNGInstance] = value; } }
            public Image this[uint type, ulong instance] { get { return this[type, instance, THUMSize.defSize, false]; } }
            public Image this[uint type, ulong instance, THUMSize size] { get { return this[type, instance, size, false]; } set { this[type, instance, size, false] = value; } }
            public Image this[uint type, ulong instance, THUMSize size, bool isPNGInstance]
            {
                get
                {
                    Item item = getItem(isPNGInstance ? fb0Pkgs : tmbPkgs, instance, (isPNGInstance ? PNGTypes : thumTypes[type])[(int)size]);
                    if (item != null && item.Resource != null)
                        return Image.FromStream(item.Resource.Stream);
                    return null;
                }
                set
                {
                    Item item = getItem(isPNGInstance ? fb0Pkgs : tmbPkgs, instance, (isPNGInstance ? PNGTypes : thumTypes[type])[(int)size]);
                    if (item == null || item.Resource == null)
                        throw new ArgumentException();

                    Image thumb;
                    thumb = value.GetThumbnailImage(thumSizes[(int)size], thumSizes[(int)size], gtAbort, System.IntPtr.Zero);
                    thumb.Save(item.Resource.Stream, System.Drawing.Imaging.ImageFormat.Png);
                    item.Commit();
                }
            }
            bool gtAbort() { return false; }

            public IResourceKey getRK(uint type, ulong instance, THUMSize size, bool isPNGInstance)
            {
                Item item = getItem(isPNGInstance ? fb0Pkgs : tmbPkgs, instance, (isPNGInstance ? PNGTypes : thumTypes[type])[(int)size]);
                return item == null ? RK.NULL : item.rk;
            }

            public static IResourceKey getNewRK(uint type, ulong instance, THUMSize size, bool isPNGInstance)
            {
                TGIN tgin = new TGIN();
                tgin.ResType = (isPNGInstance ? PNGTypes : thumTypes[type])[(int)size];
                tgin.ResGroup = (uint)(type == 0x515CA4CD ? 1 : 0);
                tgin.ResInstance = instance;
                return (AResourceKey)tgin;
            }

            static Item getItem(List<IPackage> pkgs, ulong instance, uint type)
            {
                if (type == 0x00000000) return null;
                foreach (IPackage pkg in pkgs)
                {
                    List<IResourceIndexEntry> lrie = new List<IResourceIndexEntry>(pkg.FindAll(new string[] { "ResourceType", "Instance" }, new TypedValue[] {
                            new TypedValue(typeof(uint), type),
                            new TypedValue(typeof(ulong), instance),
                        }));
                    lrie.Sort(byGroup);
                    foreach (IResourceIndexEntry rie in lrie)
                        if (!new List<uint>(thumTypes[0x515CA4CD]).Contains(type) || (rie.ResourceGroup & 0x00FFFFFF) > 0)
                            return new Item(new RIE(pkg, rie));
                }
                //return new Item(pkgs, new TGI(type, 0, instance));
                return new Item(pkgs, RK.NULL);
            }
            static int byGroup(IResourceIndexEntry x, IResourceIndexEntry y) { return (x.ResourceGroup & 0x00FFFFFF).CompareTo(y.ResourceGroup & 0x00FFFFFF); }
        }
        THUM thumb;
        THUM Thumb
        {
            get
            {
                if (thumb == null)
                    thumb = new THUM(objPkgs, tmbPkgs);
                return thumb;
            }
        }
        Image getImage(THUM.THUMSize size, Item item)
        {
            if (item.CType == CatalogType.ModularResource)
                return getImage(size, ItemForTGIBlock0(item));
            else
            {
                ulong png = (item.Resource != null) ? (ulong)item.Resource["CommonBlock.PngInstance"].Value : 0;
                return Thumb[item.rk.ResourceType, png != 0 ? png : item.rk.Instance, size, png != 0];
            }
        }
        Image getLargestThumbOrDefault(Item item)
        {
            Image img = getImage(THUM.THUMSize.large, item);
            if (img != null) return img;
            img = getImage(THUM.THUMSize.medium, item);
            if (img != null) return img;
            img = getImage(THUM.THUMSize.small, item);
            if (img != null) return img;
            return defaultThumbnail;
        }
        IResourceKey getImageRK(THUM.THUMSize size, Item item)
        {
            if (item.CType == CatalogType.ModularResource)
                return RK.NULL;
            else
            {
                ulong png = (item.Resource != null) ? (ulong)item.Resource["CommonBlock.PngInstance"].Value : 0;
                return Thumb.getRK(item.rk.ResourceType, png != 0 ? png : item.rk.Instance, size, png != 0);
            }
        }
        static IResourceKey getNewRK(THUM.THUMSize size, Item item)
        {
            if (item.CType == CatalogType.ModularResource)
                return RK.NULL;
            else
            {
                ulong png = (item.Resource != null) ? (ulong)item.Resource["CommonBlock.PngInstance"].Value : 0;
                return THUM.getNewRK(item.rk.ResourceType, png != 0 ? png : item.rk.Instance, size, png != 0);
            }
        }
        IResourceKey makeImage(THUM.THUMSize size, Item item)
        {
            if (item.CType == CatalogType.ModularResource)
                return RK.NULL;
            else
            {
                IResourceKey rk = getImageRK(size, item);
                if (rk == RK.NULL)
                {
                    rk = getNewRK(size, item);
                    RIE rie = new RIE(objPkgs[0], objPkgs[0].AddResource(rk, null, true));
                    Item thum = new Item(rie);
                    defaultThumbnail.GetThumbnailImage(THUM.thumSizes[(int)size], THUM.thumSizes[(int)size], gtAbort, System.IntPtr.Zero).Save(thum.Resource.Stream, System.Drawing.Imaging.ImageFormat.Png);
                    thum.Commit();
                }
                return rk;
            }
        }
        #endregion


        class STBL
        {
            List<IDictionary<ulong, string>> stbls;
            Item latest;
            public STBL(IList<IPackage> stblPkgs) : this(stblPkgs, 0x00) { }
            public STBL(IList<IPackage> stblPkgs, byte langID)
            {
                stbls = new List<IDictionary<ulong, string>>();
                foreach (IPackage pkg in stblPkgs)
                {
                    IList<IResourceIndexEntry> lrie = pkg.FindAll(new String[] { "ResourceType", }, new TypedValue[] { new TypedValue(typeof(uint), (uint)0x220557DA), });
                    foreach (IResourceIndexEntry rie in lrie)
                    {
                        if (rie.Instance >> 56 == langID)
                        {
                            if (latest == null) latest = new Item(new RIE(pkg, rie));
                            stbls.Add(new Item(new RIE(pkg, rie)).Resource as IDictionary<ulong, string>);
                            break;
                        }
                    }
                }
            }
            public string this[ulong guid]
            {
                get
                {
                    foreach (IDictionary<ulong, string> stbl in stbls)
                        if (stbl.ContainsKey(guid))
                            return stbl[guid];
                    return null;
                }
            }
            public IResourceKey rk { get { return latest == null ? RK.NULL : latest.rk; } }
        }
        STBL english;
        STBL English
        {
            get
            {
                if (english == null)
                    english = new STBL(objPkgs);
                return english;
            }
        }

        class NameMap
        {
            Item latest;
            List<IDictionary<ulong, string>> namemaps;
            public NameMap(IList<IPackage> nameMapPkgs)
            {
                namemaps = new List<IDictionary<ulong, string>>();
                foreach (IPackage pkg in nameMapPkgs)
                {
                    IList<IResourceIndexEntry> lrie = pkg.FindAll(new String[] { "ResourceType", }, new TypedValue[] { new TypedValue(typeof(uint), (uint)0x0166038C), });
                    if (lrie.Count > 0) latest = new Item(new RIE(pkg, lrie[0]));
                    foreach (IResourceIndexEntry rie in lrie)
                        namemaps.Add(new Item(new RIE(pkg, rie)).Resource as IDictionary<ulong, string>);
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
            public IResourceKey rk { get { return latest == null ? RK.NULL : latest.rk; } }
            public IDictionary<ulong, string> map { get { return latest == null ? null : (IDictionary<ulong, string>)latest.Resource; } }
            public void Commit() { latest.Commit(); }
        }
        NameMap nmap;
        NameMap NMap
        {
            get
            {
                if (nmap == null)
                    nmap = new NameMap(objPkgs);
                return nmap;
            }
        }

        Item ItemForTGIBlock0(Item item)
        {
            IResourceKey rk = ((AResource.TGIBlockList)item.Resource["TGIBlocks"].Value)[0];
            return new Item(objPkgs, rk);
        }


        #region LeftPanelComponents

        private void DisplaySearch()
        {
            searchPane.SelectedIndexChanged -= new EventHandler<Search.SelectedIndexChangedEventArgs>(searchPane_SelectedIndexChanged);
            searchPane.SelectedIndexChanged += new EventHandler<Search.SelectedIndexChangedEventArgs>(searchPane_SelectedIndexChanged);
            searchPane.ItemActivate -= new EventHandler<Search.ItemActivateEventArgs>(searchPane_ItemActivate);
            searchPane.ItemActivate += new EventHandler<Search.ItemActivateEventArgs>(searchPane_ItemActivate);

            this.AcceptButton = btnStart;
            this.CancelButton = null;

            menuBarWidget1.Enable(MenuBarWidget.MB.MBF_open, true);
            menuBarWidget1.Enable(MenuBarWidget.MD.MBC, true);
            menuBarWidget1.Enable(MenuBarWidget.MD.MBT, true);
            menuBarWidget1.Enable(MenuBarWidget.MD.MBS, true);

            lbSearch.Visible = true;
            lbUseMenu.Visible = false;
            lbSelectOptions.Visible = false;
            btnStart.Visible = true;
            btnStart.Enabled = false;

            StopWait();
            splitContainer1.Panel1.Controls.Clear();
            splitContainer1.Panel1.Controls.Add(searchPane);
            searchPane.Dock = DockStyle.Fill;
            searchPane.Focus();
        }

        bool waitingToDisplayObjects;
        private void DisplayObjectChooser()
        {
            waitingToDisplayObjects = false;
            searchPane = null;
            this.AcceptButton = btnStart;
            this.CancelButton = null;

            menuBarWidget1.Enable(MenuBarWidget.MB.MBF_open, true);
            menuBarWidget1.Enable(MenuBarWidget.MD.MBC, true);
            menuBarWidget1.Enable(MenuBarWidget.MD.MBT, true);
            menuBarWidget1.Enable(MenuBarWidget.MD.MBS, true);

            lbSearch.Visible = false;
            lbUseMenu.Visible = false;
            lbSelectOptions.Visible = false;
            btnStart.Visible = true;

            StopWait();
            splitContainer1.Panel1.Controls.Clear();
            splitContainer1.Panel1.Controls.Add(objectChooser);
            objectChooser.Dock = DockStyle.Fill;
            objectChooser.Focus();
        }

        bool hasOBJDs()
        {
            switch (selectedItem.CType)
            {
                case CatalogType.ModularResource:
                case CatalogType.CatalogFireplace:
                case CatalogType.CatalogObject:
                    return true;
            }
            return false;
        }
        private void DisplayOptions()
        {
            lbSearch.Visible = false;
            lbUseMenu.Visible = false;
            lbSelectOptions.Visible = true;
            btnStart.Visible = false;

            if (searchPane != null)
                mode = Mode.Clone;

            cloneFixOptions = new CloneFixOptions(this, mode == Mode.Clone, hasOBJDs());
            cloneFixOptions.CancelClicked += new EventHandler(cloneFixOptions_CancelClicked);
            cloneFixOptions.StartClicked += new EventHandler(cloneFixOptions_StartClicked);

            if (mode == Mode.Clone)
            {
                string prefix = CreatorName;
                prefix = (prefix != null) ? prefix + "_" : "";
                cloneFixOptions.UniqueName = prefix + (searchPane == null ?
                    objectChooser.SelectedItems[0].Text
                    : searchPane.SelectedItem.Text);
            }
            else
            {
                cloneFixOptions.UniqueName = Path.GetFileNameWithoutExtension(openPackageDialog.FileName);
            }

            StopWait();
            splitContainer1.Panel1.Controls.Clear();
            splitContainer1.Panel1.Controls.Add(cloneFixOptions);
            cloneFixOptions.Dock = DockStyle.Fill;
            cloneFixOptions.Focus();
        }

        private void DisplayNothing()
        {
            this.AcceptButton = null;
            this.CancelButton = null;

            menuBarWidget1.Enable(MenuBarWidget.MB.MBF_open, true);
            menuBarWidget1.Enable(MenuBarWidget.MD.MBC, true);
            menuBarWidget1.Enable(MenuBarWidget.MD.MBT, true);
            menuBarWidget1.Enable(MenuBarWidget.MD.MBS, true);

            lbSearch.Visible = false;
            lbUseMenu.Visible = true;
            lbSelectOptions.Visible = false;
            btnStart.Visible = false;

            StopWait();
            splitContainer1.Panel1.Controls.Clear();
            if (cloneFixOptions != null)
            {
                cloneFixOptions.Enabled = false;
                cloneFixOptions.Dock = DockStyle.Fill;
                splitContainer1.Panel1.Controls.Add(cloneFixOptions);
            }
        }

        private void DoWait() { DoWait("Please wait..."); }
        private void DoWait(string waitText)
        {
            splitContainer1.Panel1.Controls.Clear();
            splitContainer1.Panel1.Controls.Add(pleaseWait);
            pleaseWait.Dock = DockStyle.Fill;
            pleaseWait.Label = waitText;
            splitContainer1.Panel2.Enabled = false;
            TabEnable(false);
            this.Text = myName + " [busy]";
            Application.DoEvents();
        }
        private void StopWait()
        {
            splitContainer1.Panel2.Enabled = true;
            this.Text = myName;
            Application.DoEvents();
        }
        #endregion

        #region ObjectChooser
        private void objectChooser_SelectedIndexChanged(object sender, EventArgs e)
        {
            replacementForThumbs = null;// might as well be here; needed after FillTabs, really.
            rkLookup = null;//Indicate that we're not working on the same resource any more
            if (objectChooser.SelectedItems.Count == 0)
            {
                selectedItem = null;
                ClearTabs();
                btnStart.Enabled = false;
            }
            else
            {
                selectedItem = objectChooser.SelectedItems[0].Tag as Item;
                FillTabs(selectedItem);
                btnStart.Enabled = true;
            }
        }

        void objectChooser_ItemActivate(object sender, EventArgs e) { btnStart_Click(sender, EventArgs.Empty); }
        #endregion

        #region Search
        private void searchPane_SelectedIndexChanged(object sender, Search.SelectedIndexChangedEventArgs e)
        {
            replacementForThumbs = null;// might as well be here; needed after FillTabs, really.
            rkLookup = null;//Indicate that we're not working on the same resource any more
            if (e.SelectedItem == null)
            {
                selectedItem = null;
                ClearTabs();
                btnStart.Enabled = false;
            }
            else
            {
                selectedItem = e.SelectedItem;
                FillTabs(selectedItem);
                btnStart.Enabled = true;
            }
        }

        private void searchPane_ItemActivate(object sender, Search.ItemActivateEventArgs e) { btnStart_Click(sender, EventArgs.Empty); }
        #endregion

        #region CloneFixOptions
        bool isClone = false;
        bool isPadSTBLs = false;
        void cloneFixOptions_StartClicked(object sender, EventArgs e)
        {
            this.AcceptButton = null;
            this.CancelButton = null;
            disableCompression = !cloneFixOptions.IsCompress;
            isClone = cloneFixOptions.IsClone;
            isPadSTBLs = cloneFixOptions.IsPadSTBLs;

            CloneFixStart();
        }

        void CloneFixStart()
        {
            uniqueObject = null;
            if ((isClone || cloneFixOptions.IsRenumber) && UniqueObject == null)
            {
                cloneFixOptions_CancelClicked(null, null);
                return;
            }

            if (isClone)
            {
                saveFileDialog1.FileName = UniqueObject;
                if (ObjectCloner.Properties.Settings.Default.LastSaveFolder != null)
                    saveFileDialog1.InitialDirectory = ObjectCloner.Properties.Settings.Default.LastSaveFolder;
                DialogResult dr = saveFileDialog1.ShowDialog();
                if (dr != DialogResult.OK) { cloneFixOptions_CancelClicked(null, null); return; }
                ObjectCloner.Properties.Settings.Default.LastSaveFolder = Path.GetDirectoryName(saveFileDialog1.FileName);
            }

            if (fetching) { AbortFetching(false); }

            DoWait("Please wait, performing operations...");

            if (rkLookup == null)
            {
                stepList = null;
                SetStepList(selectedItem, out stepList);
                if (stepList == null)
                {
                    cloneFixOptions_CancelClicked(null, null);
                    return;
                }

                stepNum = 0;
                resourceList.Clear();
                rkLookup = new Dictionary<string, IResourceKey>();
                while (stepNum < stepList.Count)
                {
                    step = stepList[stepNum];
                    updateProgress(true, StepText[step], true, stepList.Count - 1, true, stepNum);
                    Application.DoEvents();
                    stepNum++;
                    step();
                }
            }

            if (isClone)
            {
                this.Enabled = false;
                DoWait("Please wait, creating your new package...");
                waitingForSavePackage = true;
                StartSaving();
            }
            else
            {
                DoWait("Please wait, updating your package...");
                FixIntegrity();
                StartFixing();
            }
        }

        void cloneFixOptions_CancelClicked(object sender, EventArgs e)
        {
            TabEnable(false);
            if (searchPane != null)
                DisplaySearch();
            else
                DisplayObjectChooser();
            cloneFixOptions = null;
        }
        #endregion

        #region Tabs
        CatalogType tabType = 0;
        void InitialiseTabs(CatalogType resourceType)
        {
            string appWas = this.Text;
            this.Text = myName + " [busy]";
            Application.UseWaitCursor = true;
            Application.DoEvents();
            try
            {
                if (tabType == resourceType) return;
                tabType = resourceType;

                if (resourceType == CatalogType.ModularResource) resourceType = CatalogType.CatalogObject;//Modular Resources - display OBJD0

                IResource res = s3pi.WrapperDealer.WrapperDealer.CreateNewResource(0, "0x" + ((uint)resourceType).ToString("X8"));
                InitialiseDetailsTab(res);
                this.tabControl1.Controls.Remove(this.tpFlagsRoom);
                this.tabControl1.Controls.Remove(this.tpFlagsFunc);
                this.tabControl1.Controls.Remove(this.tpFlagsBuildEtc);
                if (tabType == CatalogType.CatalogObject)
                {
                    this.tabControl1.Controls.Add(this.tpFlagsRoom);
                    this.tabControl1.Controls.Add(this.tpFlagsFunc);
                    this.tabControl1.Controls.Add(this.tpFlagsBuildEtc);
                    InitialiseFlagTabs(res);
                    InitialiseOtherTab(res);
                }
            }
            finally { this.Text = appWas; Application.UseWaitCursor = false; Application.DoEvents(); }
        }

        Dictionary<string, string> detailsFieldMap;//(Type:)Label -> fieldname
        Dictionary<string, string> detailsFieldMapReverse;//(Type:)fieldname -> Label
        void InitialiseDetailsTab(IResource catlg)
        {
            List<string> detailsTabFields = new List<string>();//fieldname
            List<string> detailsTabCommonFields = new List<string>();//fieldname
            List<string> fields = AApiVersionedFields.GetContentFields(0, catlg.GetType());
            detailsFieldMap = new Dictionary<string, string>();
            detailsFieldMapReverse = new Dictionary<string, string>();
            detailsFieldMap.Add("TerrainPaintBrushCatalogResource:Category", "Category");
            detailsFieldMap.Add("CommonBlock.Product Status", "CommonBlock.BuildBuyProductStatusFlags");
            detailsFieldMapReverse.Add("TerrainPaintBrushCatalogResource:Category", "Category");
            detailsFieldMapReverse.Add("CommonBlock.BuildBuyProductStatusFlags", "Product Status");

            while (tlpObjectDetail.RowCount > 2)
            {
                Control c = tlpObjectDetail.GetControlFromPosition(0, tlpObjectDetail.RowCount - 2);
                tlpObjectDetail.Controls.Remove(c);
                c = tlpObjectDetail.GetControlFromPosition(1, tlpObjectDetail.RowCount - 2);
                tlpObjectDetail.Controls.Remove(c);
                tlpObjectDetail.RowCount--;
            }

            while (tlpObjectCommon.RowCount > 3)
            {
                Control c = tlpObjectCommon.GetControlFromPosition(0, tlpObjectCommon.RowCount - 2);
                tlpObjectCommon.Controls.Remove(c);
                c = tlpObjectCommon.GetControlFromPosition(1, tlpObjectCommon.RowCount - 2);
                tlpObjectCommon.Controls.Remove(c);
                tlpObjectCommon.RowCount--;
            }

            foreach (string field in fields)
            {
                if (field.StartsWith("Unknown")) { }
                else if (field.EndsWith("Flags")) { }
                else if (field.EndsWith("Index"))
                {
                    //if (field.Equals("FallbackIndex"))
                        detailsTabFields.Add(field);
                }
                else if (field.Contains("Index")) { }
                else if (field.Equals("Materials")) { }
                else if (field.Equals("MTDoors")) { }
                else if (field.Equals("TGIBlocks")) { }
                else if (field.Equals("CommonBlock"))
                {
                    List<string> commonfields = AApiVersionedFields.GetContentFields(0, catlg["CommonBlock"].Type);
                    foreach (string commonfield in commonfields)
                    {
                        if (commonfield.StartsWith("Unknown")) { }
                        else if (commonfield.EndsWith("Flags") && commonfield != "BuildBuyProductStatusFlags") { }
                        else if (!commonfield.Equals("Value")) detailsTabCommonFields.Add(commonfield);
                    }
                }
                else if (!field.Equals("AsBytes") && !field.Equals("Stream") && !field.Equals("Value") &&
                    !field.Equals("Count") && !field.Equals("IsReadOnly") && !field.EndsWith("Reader"))
                    detailsTabFields.Add(field);
            }

            Dictionary<string, Type> types = AApiVersionedFields.GetContentFieldTypes(0, catlg.GetType());
            foreach (string field in detailsTabFields)
            {
                if (detailsFieldMapReverse.ContainsKey(catlg.GetType().Name + ":" + field))
                    CreateField(tlpObjectDetail, types[field], detailsFieldMapReverse[catlg.GetType().Name + ":" + field], true);
                else if ((catlg as AResource).ContentFields.Contains("TGIBlocks") && field.EndsWith("Index"))
                    CreateField(tlpObjectDetail, field);
                else
                    CreateField(tlpObjectDetail, types[field], field);
            }

            AApiVersionedFields common = catlg["CommonBlock"].Value as AApiVersionedFields;
            types = AApiVersionedFields.GetContentFieldTypes(0, catlg["CommonBlock"].Type);
            foreach (string field in detailsTabCommonFields)
            {
                if (detailsFieldMapReverse.ContainsKey("CommonBlock." + field))
                    CreateField(tlpObjectCommon, types[field], detailsFieldMapReverse["CommonBlock." + field], field != "BuildBuyProductStatusFlags");
                else
                    CreateField(tlpObjectCommon, types[field], field);
            }
        }

        struct flagField
        {
            public TableLayoutPanel tlp;
            public string field;
            public int length;
            public int offset;
            public flagField(TableLayoutPanel tlp, string field, int length, int offset) { this.tlp = tlp; this.field = field; this.length = length; this.offset = offset; }
        }
        List<flagField> flagFields;
        void InitialiseFlagTabs(IResource objd)
        {
            flagFields = new List<flagField>(new flagField[] {
                new flagField(tlpRoomSort, "RoomCategoryFlags", 32, 0),
                new flagField(tlpRoomSubLow, "RoomSubCategoryFlags", 32, 0),
                new flagField(tlpRoomSubHigh, "RoomSubCategoryFlags", 64, 32),
                new flagField(tlpFuncSort, "FunctionCategoryFlags", 32, 0),
                new flagField(tlpFuncSubLow, "FunctionSubCategoryFlags", 32, 0),
                new flagField(tlpFuncSubHigh, "FunctionSubCategoryFlags", 64, 32),
                new flagField(tlpBuildSort, "BuildCategoryFlags", 32, 0),
            });
            foreach (flagField ff in flagFields)
            {
                while (ff.tlp.RowCount > 2)
                {
                    for (int i = 0; i < ff.tlp.ColumnCount; i++)
                    {
                        Control c = ff.tlp.GetControlFromPosition(i, ff.tlp.RowCount - 2);
                        ff.tlp.Controls.Remove(c);
                    }
                    ff.tlp.RowCount--;
                }
            }
            foreach (flagField ff in flagFields)
            {
                Application.DoEvents();
                Type t = objd[ff.field].Type;
                CheckBox[] ackb = new CheckBox[ff.length - ff.offset];
                for (int i = 0; i < ackb.Length; i++) ackb[i] = new CheckBox();
                ff.tlp.RowCount = 2 + ackb.Length;

                for (int i = ff.offset; i < ff.length; i++)
                {
                    ulong value = (ulong)Math.Pow(2, i);
                    string s = (Enum)Enum.ToObject(t, value) + "";
                    if (s.Equals(value.ToString())) s = "-";
                    CreateField(ff, s, ackb, i - ff.offset);
                }

                ff.tlp.Controls.AddRange(ackb);
            }
        }

        Dictionary<string, string> otherFieldMap;
        void InitialiseOtherTab(IResource objd)
        {
            while (tlpOther.RowCount > 2)
            {
                Control c = tlpOther.GetControlFromPosition(0, tlpOther.RowCount - 2);
                tlpOther.Controls.Remove(c);
                c = tlpOther.GetControlFromPosition(1, tlpOther.RowCount - 2);
                tlpOther.Controls.Remove(c);
                tlpOther.RowCount--;
            }

            otherFieldMap = new Dictionary<string, string>();
            otherFieldMap.Add("Unknown8", "Unknown8");
            otherFieldMap.Add("Unknown9", "Unknown9");
            otherFieldMap.Add("Unknown10", "Unknown10");
            otherFieldMap.Add("Slot Placement", "SlotPlacementFlags");
            Dictionary<string, Type> types = AApiVersionedFields.GetContentFieldTypes(0, objd.GetType());
            foreach (string field in otherFieldMap.Keys)
                CreateField(tlpOther, types[otherFieldMap[field]], field, true);
        }

        static Dictionary<List<Type>, int> typeToLen = null;
        static void setTypeToLen()
        {
            typeToLen = new Dictionary<List<Type>, int>();
            typeToLen.Add(new List<Type>(new Type[] { typeof(sbyte), typeof(byte), }), 2 + 2);
            typeToLen.Add(new List<Type>(new Type[] { typeof(bool), typeof(char), typeof(short), typeof(ushort), }), 2 + 4);
            typeToLen.Add(new List<Type>(new Type[] { typeof(float), }), 8);
            typeToLen.Add(new List<Type>(new Type[] { typeof(int), typeof(uint), }), 2 + 8);
            typeToLen.Add(new List<Type>(new Type[] { typeof(double), }), 16);
            typeToLen.Add(new List<Type>(new Type[] { typeof(long), typeof(ulong), }), 2 + 16);
            typeToLen.Add(new List<Type>(new Type[] { typeof(decimal), typeof(DateTime), typeof(string), typeof(object), }), 30);
        }
        void CreateField(TableLayoutPanel target, Type t, string field) { CreateField(target, t, field, false); }
        void CreateField(TableLayoutPanel target, Type t, string field, bool validate)
        {
            if (typeof(Enum).IsAssignableFrom(t))
                t = Enum.GetUnderlyingType(t);

            if (typeToLen == null) setTypeToLen();

            foreach (List<Type> tt in typeToLen.Keys)
                if (tt.Contains(t))
                {
                    CreateField(target, field, typeToLen[tt], validate);
                    return;
                }

            CreateField(target, field, 30, validate);
        }

        void CreateField(TableLayoutPanel tlp, string name, int len, bool validate)
        {
            tlp.RowCount++;
            tlp.RowStyles.Insert(tlp.RowCount - 2, new RowStyle(SizeType.AutoSize));

            Label lb = new Label();
            lb.Anchor = AnchorStyles.Right;
            lb.AutoSize = true;
            lb.Name = "lb" + name;
            lb.TabIndex = tlp.RowCount * 2;
            lb.Text = name;
            tlp.Controls.Add(lb, 0, tlp.RowCount - 2);

            Label x = new Label();
            x.AutoSize = true;
            x.Text = "".PadLeft(len, 'X');

            TextBox tb = new TextBox();
            tb.Anchor = AnchorStyles.Left;
            tb.Name = "tb" + name;
            tb.ReadOnly = true;
            tb.Size = new Size(x.PreferredWidth + 6, x.PreferredHeight + 6);
            tb.TabIndex = tlp.RowCount * 2 + 1;
            if (validate)
            {
                tb.Validating += new CancelEventHandler(tb_Validating);
                tb.Tag = validate;
            }
            tlp.Controls.Add(tb, 1, tlp.RowCount - 2);
        }

        void CreateField(TableLayoutPanel tlp, string name)
        {
            tlp.RowCount++;
            tlp.RowStyles.Insert(tlp.RowCount - 2, new RowStyle(SizeType.AutoSize));

            Label lb = new Label();
            lb.Anchor = AnchorStyles.Right;
            lb.AutoSize = true;
            lb.Name = "lb" + name;
            lb.TabIndex = tlp.RowCount * 2;
            lb.Text = name;
            tlp.Controls.Add(lb, 0, tlp.RowCount - 2);

            TGIBlockCombo tbc = new TGIBlockCombo();
            tbc.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tbc.Name = "tbc" + name;
            tbc.Enabled = false;
            tbc.Margin = new Padding(3, 0 , 0, 0);
            tbc.ShowEdit = false;
            tbc.TabIndex = tlp.RowCount * 2 + 1;
            tbc.Width = (int)tlp.ColumnStyles[1].Width;
            tlp.Controls.Add(tbc, 1, tlp.RowCount - 2);
        }

        void tbc_SelectedIndexChanged(object sender, EventArgs e)
        {
            TGIBlockCombo tbc = sender as TGIBlockCombo;
            if (tbc == null) return;
        }

        void tb_Validating(object sender, CancelEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.Name == "tbPrice")
            {
                float res;
                e.Cancel = !Single.TryParse(tb.Text, out res);
            }
            else if (tb.Name == "tbProductStatus")
            {
                try { byte res = Convert.ToByte(tb.Text, tb.Text.StartsWith("0x") ? 16 : 10); }
                catch { e.Cancel = true; }
            }
            else
            {
                try { ulong res = Convert.ToUInt64(tb.Text, tb.Text.StartsWith("0x") ? 16 : 10); }
                catch { e.Cancel = true; }
            }
            if (e.Cancel) tb.SelectAll();
        }

        void CreateField(flagField ff, string name, CheckBox[] acbk, int i)
        {
            ff.tlp.RowStyles.Insert(i + 1, new RowStyle(SizeType.AutoSize));
            ff.tlp.SetCellPosition(acbk[i], new TableLayoutPanelCellPosition(0, i + 1));

            acbk[i].Anchor = AnchorStyles.Left;
            acbk[i].AutoSize = true;
            acbk[i].Name = "cb" + name;
            acbk[i].Text = name;
            acbk[i].TabIndex = ff.tlp.RowCount;
        }


        void ClearTabs()
        {
            string appWas = this.Text;
            this.Text = myName + " [busy]";
            Application.UseWaitCursor = true;
            Application.DoEvents();
            try
            {
                clearOverview();
                clearDetails();
                if (tabControl1.Contains(tpFlagsRoom))
                {
                    clearFlags();
                    clearOther();
                }
                TabEnable(false);
            }
            finally { this.Text = appWas; Application.UseWaitCursor = false; Application.DoEvents(); }
        }
        void clearOverview()
        {
            pictureBox1.Image = null;
            lbThumbTGI.Text = "";
            tbResourceName.Text = "";
            tbObjName.Text = "";
            tbCatlgName.Text = "";
            tbObjDesc.Text = "";
            tbCatlgDesc.Text = "";
            ckbCopyToAll.Checked = false;
            tbPrice.Text = "";
            tbProductStatus.Text = "";
        }
        void clearDetails()
        {
            foreach (Control c in tlpObjectDetail.Controls)
                if (c is TextBox) ((TextBox)c).Text = "";
                else if (c is TGIBlockCombo) ((TGIBlockCombo)c).SelectedIndex = -1;
            foreach (Control c in tlpObjectCommon.Controls)
                if (c is TextBox) ((TextBox)c).Text = "";
        }
        void clearFlags()
        {
            foreach (flagField ff in flagFields)
                foreach (Control c in ff.tlp.Controls)
                    if (c is CheckBox) ((CheckBox)c).Checked = false;
        }
        void clearOther()
        {
            foreach (Control c in tlpOther.Controls)
                if (c is TextBox) { ((TextBox)c).Text = ""; ((TextBox)c).ReadOnly = true; }
        }

        void FillTabs(Item item)
        {
            string appWas = this.Text;
            this.Text = myName + " [busy]";
            Application.UseWaitCursor = true;
            Application.DoEvents();
            try
            {
                if (item.Resource == null)
                {
                    ClearTabs();
                    return;
                }

                InitialiseTabs(item.CType);

                Item catlg = (item.CType == CatalogType.ModularResource) ? ItemForTGIBlock0(item) : item;

                fillOverview(catlg);
                fillDetails(catlg);
                if (item.CType == CatalogType.CatalogObject)
                {
                    fillFlags(catlg);
                    fillOther(catlg);
                }
            }
            finally { this.Text = appWas; Application.UseWaitCursor = false; Application.DoEvents(); }
            TabEnable(false);
        }
        void fillOverview(Item item)
        {
            pictureBox1.Image = getImage(THUM.THUMSize.large, item);
            lbThumbTGI.Text = (AResourceKey)getImageRK(THUM.THUMSize.large, item);
            btnReplThumb.Enabled = true;
            tbResourceName.Text = NMap[item.rk.Instance];
            AApiVersionedFields common = item.Resource["CommonBlock"].Value as AApiVersionedFields;
            tbObjName.Text = common["Name"].Value + "";
            tbObjDesc.Text = common["Desc"].Value + "";
            tbCatlgName.Text = English[(ulong)common["NameGUID"].Value];
            tbCatlgDesc.Text = English[(ulong)common["DescGUID"].Value];
            tbCatlgName.Enabled = true;
            tbCatlgDesc.Enabled = true;
            ckbCopyToAll.Enabled = true;
            tbPrice.Text = common["Price"].Value + "";
            tbPrice.ReadOnly = false;
            tbProductStatus.Text = "0x" + ((byte)common["BuildBuyProductStatusFlags"].Value).ToString("X2");
            tbProductStatus.ReadOnly = false;
        }
        void fillDetails(Item objd)
        {
            for (int i = 1; i < tlpObjectDetail.RowCount - 1; i++)
            {
                Label lb = (Label)tlpObjectDetail.GetControlFromPosition(0, i);

                TypedValue tv;
                if (detailsFieldMap.ContainsKey(objd.Resource.GetType().Name + ":" + lb.Text))
                    tv = objd.Resource[detailsFieldMap[objd.Resource.GetType().Name + ":" + lb.Text]];
                else
                    tv = objd.Resource[lb.Text];

                Control c = tlpObjectDetail.GetControlFromPosition(1, i);
                if (c is TextBox)
                {
                    TextBox tb = (TextBox)tlpObjectDetail.GetControlFromPosition(1, i);

                    if (typeof(Enum).IsAssignableFrom(tv.Type))
                    {
                        string[] s = ("" + tv).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        tb.Text = s[0];
                    }
                    else
                        tb.Text = tv;

                    tb.ReadOnly = tb.Tag == null;
                }
                else if (c is TGIBlockCombo)
                {
                    AResource.TGIBlockList tgiBlocks = objd.Resource["TGIBlocks"].Value as AResource.TGIBlockList;
                    TGIBlockCombo tbc = (TGIBlockCombo)tlpObjectDetail.GetControlFromPosition(1, i);
                    tbc.TGIBlocks = tgiBlocks;
                    int index =(int)(uint)tv.Value;
                    tbc.SelectedIndex = index >= 0 && index < tgiBlocks.Count ? index : -1;
                }
            }
            for (int i = 2; i < tlpObjectCommon.RowCount - 1; i++)
            {
                Label lb = (Label)tlpObjectCommon.GetControlFromPosition(0, i);
                TextBox tb = (TextBox)tlpObjectCommon.GetControlFromPosition(1, i);

                TypedValue tv;
                if (detailsFieldMap.ContainsKey("CommonBlock." + lb.Text))
                    tv = objd.Resource[detailsFieldMap["CommonBlock." + lb.Text]];
                else
                    tv = objd.Resource["CommonBlock." + lb.Text];

                if (typeof(Enum).IsAssignableFrom(tv.Type))
                {
                    string[] s = ("" + tv).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    tb.Text = s[0];
                }
                else
                    tb.Text = tv;

                tb.ReadOnly = tb.Tag == null;
            }
        }
        void fillFlags(Item objd)
        {
            foreach (flagField ff in flagFields)
            {
                ulong field = getFlags(objd.Resource as AResource, ff.field);
                for (int i = 1; i < ff.tlp.RowCount - 1; i++)
                {
                    ulong value = (ulong)Math.Pow(2, ff.offset + i - 1);
                    CheckBox cb = (CheckBox)ff.tlp.GetControlFromPosition(0, i);
                    cb.Checked = (field & value) != 0;
                }
            }
        }
        void fillOther(Item objd)
        {
            for (int i = 1; i < tlpOther.RowCount - 1; i++)
            {
                Label lb = (Label)tlpOther.GetControlFromPosition(0, i);
                TextBox tb = (TextBox)tlpOther.GetControlFromPosition(1, i);

                TypedValue tv = objd.Resource[otherFieldMap[lb.Text]];
                if (typeof(Enum).IsAssignableFrom(tv.Type))
                {
                    string[] s = ("" + tv).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    tb.Text = s[0];
                }
                else
                    tb.Text = tv;

                tb.ReadOnly = tb.Tag == null;
            }
        }

        void fillOverviewUpdateImage(Item item)
        {
            if (pictureBox1.Image == null)
            {
                pictureBox1.Image = getLargestThumbOrDefault(item).GetThumbnailImage(pictureBox1.Width, pictureBox1.Height, gtAbort, IntPtr.Zero);
                lbThumbTGI.Text = (AResourceKey)getNewRK(THUM.THUMSize.large, item);
            }
        }

        void TabEnable(bool enabled)
        {
            string appWas = this.Text;
            this.Text = myName + " [busy]";
            Application.UseWaitCursor = true;
            Application.DoEvents();
            try
            {
                tabEnableOverview(enabled);
                tabEnableDetails(enabled);
                if (tabControl1.Contains(tpFlagsRoom))
                {
                    tabEnableFlags(enabled);
                    tabEnableOther(enabled);
                }
            }
            finally { this.Text = appWas; Application.UseWaitCursor = false; Application.DoEvents(); }
        }
        void tabEnableOverview(bool enabled)
        {
            btnReplThumb.Enabled = enabled;
            tbCatlgName.Enabled = enabled;
            tbCatlgDesc.Enabled = enabled;
            ckbCopyToAll.Enabled = enabled;
            tbPrice.ReadOnly = !enabled;
            tbProductStatus.ReadOnly = !enabled;
        }
        void tabEnableDetails(bool enabled)
        {
            for (int i = 1; i < tlpObjectDetail.RowCount - 1; i++)
            {
                Control c = tlpObjectDetail.GetControlFromPosition(1, i);
                if (c.Tag != null) c.Enabled = enabled;
            }
            for (int i = 2; i < tlpObjectCommon.RowCount - 1; i++)
            {
                Control c = tlpObjectCommon.GetControlFromPosition(1, i);
                if (c.Tag != null) c.Enabled = enabled;
            }
        }
        void tabEnableFlags(bool enabled)
        {
            foreach (flagField ff in flagFields)
            {
                for (int i = 1; i < ff.tlp.RowCount - 1; i++)
                {
                    CheckBox cb = (CheckBox)ff.tlp.GetControlFromPosition(0, i);
                    cb.Enabled = enabled;
                }
            }
        }
        void tabEnableOther(bool enabled)
        {
            for (int i = 1; i < tlpOther.RowCount - 1; i++)
            {
                TextBox tb = (TextBox)tlpOther.GetControlFromPosition(1, i);
                if (tb.Tag != null) tb.Enabled = enabled;
            }
        }

        ulong getFlags(AApiVersionedFields owner, string field)
        {
            TypedValue tv = owner[field];
            object o = Convert.ChangeType(tv.Value, Enum.GetUnderlyingType(tv.Type));
            if (o.GetType().Equals(typeof(byte))) return (byte)o;
            if (o.GetType().Equals(typeof(ushort))) return (ushort)o;
            if (o.GetType().Equals(typeof(uint))) return (uint)o;
            return (ulong)o;
        }
        ulong getFlags(flagField ff)
        {
            ulong res = 0;
            for (int i = ff.offset; i < ff.length; i++)
            {
                CheckBox cb = (CheckBox)ff.tlp.GetControlFromPosition(0, i - ff.offset + 1);
                if (cb.Checked) res += ((ulong)1 << i);
            }
            return res;
        }
        void setFlags(AApiVersionedFields owner, string field, ulong value)
        {
            Type t = AApiVersionedFields.GetContentFieldTypes(0, owner.GetType())[field];
            TypedValue tv = new TypedValue(t, Enum.ToObject(t, value));
            owner[field] = tv;
        }
        #endregion


        #region Loading thread
        Thread loadThread;
        bool haveLoaded = false;
        bool loading = false;
        bool isFix = false;
        Dictionary<uint, Item> CTPTBrushIndexToPair;
        void StartLoading(CatalogType resourceType, bool setIsFix)
        {
            if (haveLoaded) return;
            if (loading) { AbortLoading(false); }
            if (fetching) { AbortFetching(false); }

            isFix = setIsFix;

            waitingToDisplayObjects = true;
            objectChooser.Items.Clear();
            CTPTBrushIndexToPair = new Dictionary<uint, Item>();

            this.LoadingComplete -= new EventHandler<BoolEventArgs>(MainForm_LoadingComplete);
            this.LoadingComplete += new EventHandler<BoolEventArgs>(MainForm_LoadingComplete);

            FillListView flv = new FillListView(this, objPkgs, ddsPkgs, tmbPkgs, resourceType
                , createListViewItem, updateProgress, stopLoading, OnLoadingComplete);

            loadThread = new Thread(new ThreadStart(flv.LoadPackage));
            loading = true;
            loadThread.Start();
        }

        void AbortLoading(bool abort)
        {
            waitingToDisplayObjects = false;
            if (abort)
            {
                loading = false;
                if (loadThread != null) loadThread.Abort();
            }
            else
            {
                if (!loading) MainForm_LoadingComplete(null, new BoolEventArgs(false));
                else loading = false;
            }
        }

        void MainForm_LoadingComplete(object sender, BoolEventArgs e)
        {
            loading = false;
            while (loadThread != null && loadThread.IsAlive)
                loadThread.Join(100);
            loadThread = null;

            this.toolStripProgressBar1.Visible = false;
            this.toolStripStatusLabel1.Visible = false;

            if (e.arg)
            {
                haveLoaded = true;

                if (waitingToDisplayObjects)
                {
                    if (!isFix || objectChooser.Items.Count != 1)
                    {
                        if (mode == Mode.Clone && menuBarWidget1.IsChecked(MenuBarWidget.MB.MBV_icons))
                        {
                            waitingForImages = true;
                            StartFetching();
                        }

                        if (objectChooser.Items.Count > 0) objectChooser.SelectedIndex = 0;
                        DisplayObjectChooser();
                    }
                    else
                        CloneFixStart();
                }
            }
            else
            {
                ClosePkg();
                DisplayNothing();
            }
        }

        Dictionary<int, int> LItoIMG32 = new Dictionary<int, int>();
        Dictionary<int, int> LItoIMG64 = new Dictionary<int, int>();

        public delegate void createListViewItemCallback(Item objd);
        void createListViewItem(Item item)
        {
            if (item.CType ==  CatalogType.CatalogTerrainPaintBrush)
            {
                byte status = (byte)item.Resource["CommonBlock.BuildBuyProductStatusFlags"].Value;
                if ((status & 0x01) == 0) // do not list
                {
                    uint brushIndex = (uint)item.Resource["BrushIndex"].Value;
                    CTPTBrushIndexToPair.Add(brushIndex - 1, item);
                    return;
                }
            }
            ListViewItem lvi = new ListViewItem();
            if (item.Resource != null)
            {
                string objdtag;
                if (item.CType == CatalogType.ModularResource) objdtag = ItemForTGIBlock0(item).Resource["CommonBlock.Name"];
                else objdtag = item.Resource["CommonBlock.Name"];
                lvi.Text = (objdtag.IndexOf(':') < 0) ? objdtag : objdtag.Substring(objdtag.LastIndexOf(':') + 1);
            }
            else
            {
                string s = item.Exception.Message;
                for (Exception ex = item.Exception.InnerException; ex != null; ex = ex.InnerException) s += "  " + ex.Message;
                lvi.Text = s;
            }
            List<string> exts;
            string tag = "";
            if (s3pi.Extensions.ExtList.Ext.TryGetValue("0x" + item.rk.ResourceType.ToString("X8"), out exts)) tag = exts[0];
            else tag = "UNKN";
            lvi.SubItems.AddRange(new string[] { tag, item.RGVsn, "" + (AResourceKey)item.rk, });
            lvi.Tag = item;
            objectChooser.Items.Add(lvi);
        }

        public delegate bool stopLoadingCallback();
        private bool stopLoading() { return !loading; }

        private event EventHandler<BoolEventArgs> LoadingComplete;
        public delegate void loadingCompleteCallback(bool complete);
        public void OnLoadingComplete(bool complete) { if (LoadingComplete != null) { LoadingComplete(this, new BoolEventArgs(complete)); } }

        #endregion

        #region Fetch Images thread
        Thread fetchThread;
        bool haveFetched = false;
        bool fetching = false;
        void StartFetching()
        {
            if (!haveLoaded) return;

            objectChooser.SmallImageList = new ImageList();
            objectChooser.SmallImageList.ImageSize = new System.Drawing.Size(32, 32);
            LItoIMG32 = new Dictionary<int, int>();
            objectChooser.LargeImageList = new ImageList();
            objectChooser.LargeImageList.ImageSize = new System.Drawing.Size(64, 64);
            LItoIMG64 = new Dictionary<int, int>();

            this.FetchingComplete += new EventHandler<BoolEventArgs>(MainForm_FetchingComplete);

            FetchImages fi = new FetchImages(this, objectChooser.Items.Count,
                getItem, getImage, setImage, updateProgress, stopFetching, OnFetchingComplete);

            fetchThread = new Thread(new ThreadStart(fi.Fetch));
            fetching = true;
            fetchThread.Start();
        }

        void AbortFetching(bool abort)
        {
            if (abort)
            {
                fetching = false;
                if (fetchThread != null) fetchThread.Abort();
            }
            else
            {
                if (!fetching) MainForm_FetchingComplete(null, new BoolEventArgs(false));
                else fetching = false;
            }
        }

        bool waitingForImages;
        void MainForm_FetchingComplete(object sender, BoolEventArgs e)
        {
            fetching = false;
            while (fetchThread != null && fetchThread.IsAlive)
                fetchThread.Join(100);
            fetchThread = null;

            this.toolStripProgressBar1.Visible = false;
            this.toolStripStatusLabel1.Visible = false;

            if (e.arg)
            {
                haveFetched = true;
            }

            if (waitingForImages)
            {
                waitingForImages = false;
            }
        }

        public delegate bool stopFetchingCallback();
        private bool stopFetching() { return !fetching; }

        public delegate Item getItemCallback(int i);
        private Item getItem(int i) { return objectChooser.Items[i].Tag as Item; }

        public delegate Image getImageCallback(THUM.THUMSize size, Item item);

        public delegate void setImageCallback(bool isSmall, int i, Image image);
        private void setImage(bool isSmall, int i, Image image)
        {
            if (isSmall)
            {
                LItoIMG32.Add(i, objectChooser.SmallImageList.Images.Count);
                objectChooser.SmallImageList.Images.Add(image);
                if (currentView != View.Tile && currentView != View.LargeIcon)
                    objectChooser.Items[i].ImageIndex = LItoIMG32[i];
            }
            else
            {
                LItoIMG64.Add(i, objectChooser.LargeImageList.Images.Count);
                objectChooser.LargeImageList.Images.Add(image);
                if (currentView == View.Tile || currentView == View.LargeIcon)
                    objectChooser.Items[i].ImageIndex = LItoIMG64[i];
            }
        }

        private event EventHandler<BoolEventArgs> FetchingComplete;
        public delegate void fetchingCompleteCallback(bool complete);
        public void OnFetchingComplete(bool complete) { if (FetchingComplete != null) { FetchingComplete(this, new BoolEventArgs(complete)); } }

        class FetchImages
        {
            MainForm mainForm;
            int count;
            getItemCallback getItemCB;
            getImageCallback getImageCB;
            setImageCallback setImageCB;
            updateProgressCallback updateProgressCB;
            stopFetchingCallback stopFetchingCB;
            fetchingCompleteCallback fetchingCompleteCB;

            int imgcnt = 0;

            public FetchImages(MainForm form, int count,
                getItemCallback getItemCB, getImageCallback getImageCB, setImageCallback setImageCB,
                updateProgressCallback updateProgressCB, stopFetchingCallback stopFetchingCB, fetchingCompleteCallback fetchingCompleteCB)
            {
                this.mainForm = form;
                this.count = count;
                this.getItemCB = getItemCB;
                this.getImageCB = getImageCB;
                this.setImageCB = setImageCB;
                this.updateProgressCB = updateProgressCB;
                this.stopFetchingCB = stopFetchingCB;
                this.fetchingCompleteCB = fetchingCompleteCB;

            }

            public void Fetch()
            {
                updateProgress(true, "Please wait, loading thumbnails...", false, -1, false, -1);

                bool complete = false;
                try
                {
                    updateProgress(false, "", true, count, true, 0);
                    int freq = Math.Max(1, count / 100);
                    for (int i = 0; i < count; i++)
                    {
                        if (stopFetching) return;

                        Item item = getItem(i);

                        if (stopFetching) return;

                        Image img = getImage(THUM.THUMSize.small, item);
                        if (img != null) setImage(true, i, img);

                        if (stopFetching) return;

                        img = getImage(THUM.THUMSize.medium, item);
                        if (img != null) setImage(false, i, img);

                        if (stopFetching) return;

                        if (i % freq == 0)
                            updateProgress(true, String.Format("Please wait, loading thumbnails... {0}%", i * 100 / count), true, count, true, i);
                    }
                    complete = true;
                }
                catch (ThreadInterruptedException) { }
                finally
                {
                    updateProgress(true, "Finished loading thumnails", true, count, true, count);
                    fetchingComplete(complete);
                }
            }

            Item getItem(int i) { Thread.Sleep(0); return (Item)(!mainForm.IsHandleCreated ? null : mainForm.Invoke(getItemCB, new object[] { i, })); }

            Image getImage(THUM.THUMSize size, Item item) { Thread.Sleep(0); return (Image)(!mainForm.IsHandleCreated ? null : mainForm.Invoke(getImageCB, new object[] { size, item, })); }

            void setImage(bool isSmall, int i, Image image) { imgcnt++; Thread.Sleep(0); if (mainForm.IsHandleCreated) mainForm.Invoke(setImageCB, new object[] { isSmall, i, image, }); }

            void updateProgress(bool changeText, string text, bool changeMax, int max, bool changeValue, int value)
            {
                Thread.Sleep(0);
                if (mainForm.IsHandleCreated) mainForm.Invoke(updateProgressCB, new object[] { changeText, text, changeMax, max, changeValue, value, });
            }

            bool stopFetching { get { Thread.Sleep(0); return !mainForm.IsHandleCreated || (bool)mainForm.Invoke(stopFetchingCB); } }

            void fetchingComplete(bool complete) { Thread.Sleep(0); if (mainForm.IsHandleCreated) mainForm.BeginInvoke(fetchingCompleteCB, new object[] { complete, }); }
        }
        #endregion

        #region Saving thread
        Thread saveThread;
        bool saving = false;
        void StartSaving()
        {
            this.SavingComplete += new EventHandler<BoolEventArgs>(MainForm_SavingComplete);

            SaveList sl = new SaveList(this,
                searchPane == null ? objectChooser.SelectedItems[0].Tag as Item
                : searchPane.SelectedItem.Tag as Item, rkLookup, objPkgs, ddsPkgs, tmbPkgs,
                saveFileDialog1.FileName, isPadSTBLs, cloneFixOptions.IsExcludeCommon ? lS3ocResourceList : null,
                updateProgress, stopSaving, OnSavingComplete);

            saveThread = new Thread(new ThreadStart(sl.SavePackage));
            saving = true;
            saveThread.Start();
        }

        void AbortSaving(bool abort)
        {
            if (abort)
            {
                saving = false;
                if (saveThread != null) saveThread.Abort();
            }
            else
            {
                if (!saving) MainForm_SavingComplete(null, new BoolEventArgs(false));
                else saving = false;
            }
        }

        bool waitingForSavePackage;
        void MainForm_SavingComplete(object sender, BoolEventArgs e)
        {
            saving = false;
            this.Enabled = true;
            while (saveThread != null && saveThread.IsAlive)
                saveThread.Join(100);
            saveThread = null;

            this.toolStripProgressBar1.Visible = false;
            this.toolStripStatusLabel1.Visible = false;

            if (waitingForSavePackage)
            {
                waitingForSavePackage = false;
                if (e.arg)
                {
                    isPadSTBLs = false;
                    isClone = false;
                    fileReOpenToFix(saveFileDialog1.FileName, selectedItem.CType);
                }
                else
                {
                    if (File.Exists(saveFileDialog1.FileName))
                        CopyableMessageBox.Show("\nSave not complete.\nPlease ensure package is not in use.\n", myName, CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Warning);
                    else
                        CopyableMessageBox.Show("\nSave not complete.\n", myName, CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Warning);
                    DisplayOptions();
                }
            }
        }

        public delegate bool stopSavingCallback();
        private bool stopSaving() { return !saving; }

        private event EventHandler<BoolEventArgs> SavingComplete;
        public delegate void savingCompleteCallback(bool complete);
        public void OnSavingComplete(bool complete) { if (SavingComplete != null) { SavingComplete(this, new BoolEventArgs(complete)); } }

        class SaveList
        {
            MainForm mainForm;
            Item selectedItem;
            Dictionary<string, IResourceKey> rkList;
            List<IPackage> objPkgs;
            List<IPackage> ddsPkgs;
            List<IPackage> tmbPkgs;
            string outputPackage;
            bool padSTBLs;
            List<Dictionary<String, TypedValue>> commonResources;
            updateProgressCallback updateProgressCB;
            stopSavingCallback stopSavingCB;
            savingCompleteCallback savingCompleteCB;
            public SaveList(MainForm form, Item catlgItem, Dictionary<string, IResourceKey> rkList, List<IPackage> objPkgs, List<IPackage> ddsPkgs, List<IPackage> tmbPkgs,
                string outputPackage, bool padSTBLs, List<Dictionary<String, TypedValue>> commonResources,
                updateProgressCallback updateProgressCB, stopSavingCallback stopSavingCB, savingCompleteCallback savingCompleteCB)
            {
                this.mainForm = form;
                this.selectedItem = catlgItem;
                this.rkList = rkList;
                this.objPkgs = objPkgs;
                this.ddsPkgs = ddsPkgs;
                this.tmbPkgs = tmbPkgs;
                this.outputPackage = outputPackage;
                this.padSTBLs = padSTBLs;
                this.commonResources = commonResources;
                this.updateProgressCB = updateProgressCB;
                this.stopSavingCB = stopSavingCB;
                this.savingCompleteCB = savingCompleteCB;
            }

            //Type: 0x00B2D882 resources are in Fullbuild2, everything else is in Fullbuild0, except thumbs
            //FullBuild0 is:
            //  ...\Gamedata\Shared\Packages\FullBuild0.package
            //Relative path to ALLThumbnails is:
            // .\..\..\..\Thumbnails\ALLThumbnails.package
            public void SavePackage()
            {
                updateProgress(true, "Creating output package...", false, -1, false, -1);
                IPackage target = s3pi.Package.Package.NewPackage(0);

                updateProgress(true, "Please wait...", false, -1, false, -1);

                bool complete = false;
                NameMap fb0nm = new NameMap(objPkgs);
                NameMap fb2nm = new NameMap(ddsPkgs);
                Item newnmap = NewResource(target, fb0nm.rk == RK.NULL ? new AResource.TGIBlock(0, null, 0x0166038C, 0, selectedItem.rk.Instance) : fb0nm.rk);
                IDictionary<ulong, string> newnamemap = (IDictionary<ulong, string>)newnmap.Resource;
                try
                {
                    int i = 0;
                    int freq = Math.Max(1, rkList.Count / 50);
                    updateProgress(true, "Cloning... 0%", true, rkList.Count, true, i);
                    string lastSaved = "nothing yet";
                    foreach (var kvp in rkList)
                    {
                        if (stopSaving) return;

                        if (!excludeResource(kvp.Value))
                        {
                            List<IPackage> lpkg = (selectedItem.rk.ResourceType != 0x04ED4BB2 && kvp.Value.ResourceType == 0x00B2D882) ? ddsPkgs : kvp.Key.EndsWith("Thumb") ? tmbPkgs : objPkgs;
                            NameMap nm = kvp.Value.ResourceType == 0x00B2D882 ? fb2nm : fb0nm;

                            Item item = new Item(new RIE(lpkg, kvp.Value), true); // use default wrapper
                            if (item.ResourceIndexEntry != null)
                            {
                                if (!stopSaving) target.AddResource(kvp.Value, item.Resource.Stream, true);
                                lastSaved = kvp.Key;
                                if (!newnamemap.ContainsKey(kvp.Value.Instance))
                                {
                                    string name = nm[kvp.Value.Instance];
                                    if (name != null)
                                        if (!stopSaving) newnamemap.Add(kvp.Value.Instance, name);
                                }
                            }
                        }

                        if (++i % freq == 0)
                            updateProgress(true, "Cloned " + lastSaved + "... " + i * 100 / rkList.Count + "%", true, rkList.Count, true, i);
                    }
                    updateProgress(true, "", true, rkList.Count, true, rkList.Count);

                    #region String tables
                    updateProgress(true, "Finding string tables...", true, 0, true, 0);

                    Item catlgItem = selectedItem;
                    if (catlgItem.CType == CatalogType.ModularResource || catlgItem.CType == CatalogType.CatalogFireplace)
                    {
                        AResource.TGIBlock tgib = ((AResource.TGIBlockList)catlgItem.Resource["TGIBlocks"].Value)[0];
                        catlgItem = new Item(objPkgs, tgib);
                    }

                    ulong nameGUID = (ulong)catlgItem.Resource["CommonBlock.NameGUID"].Value;
                    ulong descGUID = (ulong)catlgItem.Resource["CommonBlock.DescGUID"].Value;

                    i = 0;
                    freq = 1;// Math.Max(1, lrie.Count / 10);

                    updateProgress(true, "Creating string tables extracts... 0%", true, 0x17, true, i);
                    STBL english = new STBL(objPkgs);
                    while (i < 0x17)
                    {
                        if (stopSaving) return;

                        STBL lang = new STBL(objPkgs, (byte)i);
                        string name = lang[nameGUID];
                        string desc = lang[descGUID];

                        Item newstbl;
                        AResourceKey newRK;
                        if (name == null && desc == null)
                        {
                            if (!padSTBLs || english == null) goto skip;
                            name = english[nameGUID];
                            desc = english[descGUID];
                            newRK = new RK(english.rk);
                            newRK.Instance = (english.rk.Instance & 0xFFFFFFFFFFFF0000) | ((ulong)i << 56);
                        }
                        else
                        {
                            newRK = new RK(lang.rk);
                            newRK.Instance &= 0xFFFFFFFFFFFF0000;
                        }
                        newstbl = NewResource(target, newRK);

                        IDictionary<ulong, string> outstbl = (IDictionary<ulong, string>)newstbl.Resource;
                        if (!outstbl.ContainsKey(nameGUID)) outstbl.Add(nameGUID, name != null ? name : "");
                        if (!outstbl.ContainsKey(descGUID)) outstbl.Add(descGUID, desc != null ? desc : "");
                        if (!stopSaving) newstbl.Commit();

                        if (!newnamemap.ContainsKey(newRK.Instance))
                        {
                            string nmname = fb0nm[lang.rk.Instance];
                            if (nmname != null) { if (!stopSaving) newnamemap.Add(newRK.Instance, nmname); }
                            else { if (!stopSaving) newnamemap.Add(newRK.Instance, String.Format(language_fmt, languages[i], i, english.rk.Instance)); }
                        }

                    skip:
                        if (++i % freq == 0)
                            updateProgress(true, "Creating string tables extracts... " + i * 100 / 0x17 + "%", true, 0x17, true, i);
                    }
                    #endregion

                    updateProgress(true, "Committing new name map... ", true, 0, true, 0);
                    if (!stopSaving) newnmap.Commit();

                    updateProgress(true, "Saving package...", true, 0, true, 0);
                    foreach (IResourceIndexEntry ie in target.GetResourceList) ie.Compressed = (ushort)(disableCompression ? 0x0000 : 0xffff);
                    try
                    {
                        target.SaveAs(outputPackage);
                        complete = true;
                    }
                    catch { }
                }
                catch (ThreadInterruptedException) { }
                finally
                {
                    savingComplete(complete);
                }
            }

            Item NewResource(IPackage pkg, IResourceKey rk)
            {
                RIE rie = new RIE(pkg, pkg.AddResource(rk, null, true));
                if (rie.rie == null) return null;
                return new Item(rie);
            }

            void updateProgress(bool changeText, string text, bool changeMax, int max, bool changeValue, int value)
            {
                Thread.Sleep(0);
                if (mainForm.IsHandleCreated) mainForm.Invoke(updateProgressCB, new object[] { changeText, text, changeMax, max, changeValue, value, });
            }

            bool stopSaving { get { Thread.Sleep(0); return !mainForm.IsHandleCreated || (bool)mainForm.Invoke(stopSavingCB); } }

            void savingComplete(bool complete)
            {
                Thread.Sleep(0);
                if (mainForm.IsHandleCreated)
                    mainForm.BeginInvoke(savingCompleteCB, new object[] { complete, });
            }

            bool excludeResource(IResourceKey rk)
            {
                if (commonResources == null) return false;

                AResource.TGIBlock tgib = new AResource.TGIBlock(0, null, rk);
                foreach (var dict in commonResources)
                {
                    bool match = true;
                    foreach (string key in dict.Keys)
                        if (!tgib[key].Equals(dict[key])) { match = false; break; };
                    if (match) return true;
                }
                return false;
            }
        }
        #endregion

        #region Common to threads
        public class BoolEventArgs : EventArgs
        {
            public bool arg;
            public BoolEventArgs(bool arg) { this.arg = arg; }
        }

        public delegate void updateProgressCallback(bool changeText, string text, bool changeMax, int max, bool changeValue, int value);
        void updateProgress(bool changeText, string text, bool changeMax, int max, bool changeValue, int value)
        {
            if (!this.IsHandleCreated) return;
            if (changeText)
            {
                toolStripStatusLabel1.Visible = text.Length > 0;
                toolStripStatusLabel1.Text = text;
            }
            if (changeMax)
            {
                if (max == -1)
                    toolStripProgressBar1.Visible = false;
                else
                {
                    toolStripProgressBar1.Visible = true;
                    toolStripProgressBar1.Maximum = max;
                }
            }
            if (changeValue)
                toolStripProgressBar1.Value = value;
        }
        #endregion


        #region Make Unique bits

        Dictionary<IResourceKey, Item> rkToItem;
        Dictionary<ulong, ulong> oldToNew;
        string uniqueObject = null;
        string UniqueObject
        {
            get
            {
                if (uniqueObject == null)
                {
                    if (cloneFixOptions.UniqueName == null || cloneFixOptions.UniqueName.Length == 0)
                    {
                        StringInputDialog ond = new StringInputDialog();
                        ond.Caption = "Unique Resource Name";
                        ond.Prompt = "Enter unique identifier";
                        ond.Value = cloneFixOptions.UniqueName;
                        DialogResult dr = ond.ShowDialog();
                        if (dr == DialogResult.OK) uniqueObject = ond.Value;
                    }
                    else uniqueObject = cloneFixOptions.UniqueName;
                }
                return uniqueObject;
            }
        }

        ulong nameGUID, newNameGUID;
        ulong descGUID, newDescGUID;

        void StartFixing()
        {
            Dictionary<IResourceKey, Item> rkToItemAdded = new Dictionary<IResourceKey,Item>();

            Item catlgItem = (selectedItem.CType == CatalogType.ModularResource || selectedItem.CType == CatalogType.CatalogFireplace) ? ItemForTGIBlock0(selectedItem) : selectedItem;
            //oldToNew = new Dictionary<ulong, ulong>();
            try
            {
                IList<IResourceIndexEntry> lirie = objPkgs[0].GetResourceList;
                foreach(IResourceIndexEntry irie in lirie)
                {
                    Item item = new Item(new RIE(objPkgs[0], irie));
                    bool dirty = false;

                    if ((item.rk.Instance == selectedItem.rk.Instance && item.CType != CatalogType.ModularResource)//Selected CatlgItem
                        || item.CType == CatalogType.CatalogObject//all OBJDs (i.e. from MDLR or CFIR)
                        || item.CType == CatalogType.CatalogTerrainPaintBrush//all CTPTs (i.e. pair of selectedItem)
                        )
                    {
                        #region Selected CatlgItem; all OBJD (i.e. from MDLR or CFIR)
                        AHandlerElement commonBlock = ((AHandlerElement)item.Resource["CommonBlock"].Value);

                        #region Selected CatlgItem || all MDLR OBJDs || both CTPTs || 0th CFIR OBJD
                        if (item.rk.Instance == selectedItem.rk.Instance//Selected CatlgItem
                            || selectedItem.CType == CatalogType.ModularResource//all MDLR OBJDs
                            || selectedItem.CType == CatalogType.CatalogTerrainPaintBrush//both CTPTs
                            || item.rk.Instance == catlgItem.rk.Instance//0th CFIR OBJD
                            )
                        {
                            commonBlock["NameGUID"] = new TypedValue(typeof(ulong), newNameGUID);
                            commonBlock["DescGUID"] = new TypedValue(typeof(ulong), newDescGUID);
                            commonBlock["Price"] = new TypedValue(typeof(float), float.Parse(tbPrice.Text));
                            commonBlock["BuildBuyProductStatusFlags"] = new TypedValue(commonBlock["BuildBuyProductStatusFlags"].Type, Convert.ToByte(tbProductStatus.Text, tbProductStatus.Text.StartsWith("0x") ? 16 : 10));

                            if (cloneFixOptions.IsRenumber)
                            {
                                commonBlock["Name"] = new TypedValue(typeof(string), "CatalogObjects/Name:" + UniqueObject);
                                commonBlock["Desc"] = new TypedValue(typeof(string), "CatalogObjects/Description:" + UniqueObject);
                            }
                        }
                        #endregion

                        #region Selected CatlgItem; 0th OBJD from MDLR or CFIR
                        if (item.rk.Instance == selectedItem.rk.Instance || item.rk.Instance == catlgItem.rk.Instance)//Selected CatlgItem; 0th OBJD from MDLR or CFIR
                        {
                            ulong PngInstance = (ulong)commonBlock["PngInstance"].Value;
                            bool isPng = PngInstance != 0;
                            if (cloneFixOptions.IsIncludeThumbnails)
                            {
                                Image img = getLargestThumbOrDefault(item);
                                //Always output one of each size
                                foreach (THUM.THUMSize size in Enum.GetValues(typeof(THUM.THUMSize)))
                                    if (getImage(size, item) == null)
                                    {
                                        IResourceKey rk = makeImage(size, item);
                                        rkToItemAdded.Add(rk, new Item(objPkgs, rk));
                                    }
                                //Update
                                if (replacementForThumbs != null) img = replacementForThumbs;
                                ulong instance = isPng ? PngInstance : item.rk.Instance;
                                foreach (THUM.THUMSize size in Enum.GetValues(typeof(THUM.THUMSize)))
                                    Thumb[item.rk.ResourceType, instance, size, isPng] = img;
                            }

                            if (isPng && oldToNew.ContainsKey(PngInstance))
                                commonBlock["PngInstance"] = new TypedValue(typeof(ulong), oldToNew[PngInstance]);

                            for (int i = 2; i < tlpObjectDetail.RowCount - 1; i++)
                            {
                                Label lb = (Label)tlpObjectDetail.GetControlFromPosition(0, i);
                                Control c = tlpObjectDetail.GetControlFromPosition(1, i);
                                if (c.Tag == null) continue;

                                if (c is TextBox)
                                {
                                    TextBox tb = c as TextBox;
                                    TypedValue tvOld = item.Resource[detailsFieldMap[item.Resource.GetType().Name + ":" + lb.Text]];

                                    ulong u = Convert.ToUInt64(tb.Text, tb.Text.StartsWith("0x") ? 16 : 10);
                                    object val;
                                    if (typeof(Enum).IsAssignableFrom(tvOld.Type))
                                        val = Enum.ToObject(tvOld.Type, u);
                                    else
                                        val = Convert.ChangeType(u, tvOld.Type);

                                    item.Resource[detailsFieldMap[item.Resource.GetType().Name + ":" + lb.Text]] = new TypedValue(tvOld.Type, val);
                                }
                            }

                            for (int i = 2; i < tlpObjectCommon.RowCount - 1; i++)
                            {
                                Label lb = (Label)tlpObjectCommon.GetControlFromPosition(0, i);
                                TextBox tb = (TextBox)tlpObjectCommon.GetControlFromPosition(1, i);
                                if (tb.Tag == null) continue;

                                TypedValue tvOld = item.Resource[detailsFieldMap["CommonBlock." + lb.Text]];

                                ulong u = Convert.ToUInt64(tb.Text, tb.Text.StartsWith("0x") ? 16 : 10);
                                object val;
                                if (typeof(Enum).IsAssignableFrom(tvOld.Type))
                                    val = Enum.ToObject(tvOld.Type, u);
                                else
                                    val = Convert.ChangeType(u, tvOld.Type);

                                item.Resource[detailsFieldMap["CommonBlock." + lb.Text]] = new TypedValue(tvOld.Type, val);
                            }

                            #region Selected OBJD only
                            if (selectedItem.CType == CatalogType.CatalogObject)//Selected OBJD only
                            {
                                foreach (flagField ff in flagFields)
                                {
                                    ulong old = getFlags(item.Resource as AResource, ff.field);
                                    ulong mask = (ulong)0xFFFFFFFF << ff.offset;
                                    ulong res = getFlags(ff);
                                    res |= (ulong)(old & ~mask);
                                    setFlags(item.Resource as AResource, ff.field, res);
                                }

                                for (int i = 1; i < tlpOther.RowCount - 1; i++)
                                {
                                    Label lb = (Label)tlpOther.GetControlFromPosition(0, i);
                                    TextBox tb = (TextBox)tlpOther.GetControlFromPosition(1, i);
                                    if (tb.Tag == null) continue;

                                    TypedValue tvOld = item.Resource[otherFieldMap[lb.Text]];

                                    ulong u = Convert.ToUInt64(tb.Text, tb.Text.StartsWith("0x") ? 16 : 10);
                                    object val;
                                    if (typeof(Enum).IsAssignableFrom(tvOld.Type))
                                        val = Enum.ToObject(tvOld.Type, u);
                                    else
                                        val = Convert.ChangeType(u, tvOld.Type);

                                    item.Resource[otherFieldMap[lb.Text]] = new TypedValue(tvOld.Type, val);
                                }
                            }
                            #endregion
                        }
                        #endregion

                        if (cloneFixOptions.IsRenumber)
                        {
                            #region Keep brushes together
                            if (item.CType == CatalogType.CatalogTerrainPaintBrush)//Both CTPTs
                            {
                                byte status = (byte)commonBlock["BuildBuyProductStatusFlags"].Value;
                                uint brushIndex = FNV32.GetHash(UniqueObject) << 1;
                                if ((status & 0x01) != 0)
                                    item.Resource["BrushIndex"] = new TypedValue(typeof(uint), brushIndex);
                                else
                                    item.Resource["BrushIndex"] = new TypedValue(typeof(uint), brushIndex + 1);
                            }
                            #endregion

                            if (item.CType == CatalogType.CatalogObject)
                            {
                                #region Avoid renumbering Fallback TGI
                                int fallbackIndex = (int)(uint)item.Resource["FallbackIndex"].Value;
                                AResource.TGIBlockList tgiBlocks = item.Resource["TGIBlocks"].Value as AResource.TGIBlockList;
                                AResourceKey fallbackRK = new AResource.TGIBlock(0, null, tgiBlocks[fallbackIndex]);

                                UpdateRKsFromField((AResource)item.Resource);

                                tgiBlocks[fallbackIndex] = new AResource.TGIBlock(0, null, (IResourceKey)fallbackRK);
                                #endregion
                            }
                            else
                                UpdateRKsFromField((AResource)item.Resource);
                        }

                        dirty = true;
                        #endregion
                    }
                    else if (item.ResourceIndexEntry.ResourceType == 0x0166038C)
                    {
                        #region NameMap
                        IDictionary<ulong, string> nm = (IDictionary<ulong, string>)item.Resource;
                        foreach (ulong old in oldToNew.Keys)
                            if (nm.ContainsKey(old) && !nm.ContainsKey(oldToNew[old]))
                            {
                                nm.Add(oldToNew[old], nm[old]);
                                nm.Remove(old);
                                dirty = true;
                            }
                        #endregion
                    }
                    else if (item.ResourceIndexEntry.ResourceType == 0x220557DA)
                    {
                        #region STBLs
                        IDictionary<ulong, string> stbl = (IDictionary<ulong, string>)item.Resource;

                        string name = "";
                        if (stbl.ContainsKey(nameGUID)) { name = stbl[nameGUID]; stbl.Remove(nameGUID); }
                        if (ckbCopyToAll.Checked || item.rk.Instance >> 56 == 0x00) name = tbCatlgName.Text;
                        stbl.Add(newNameGUID, name);

                        string desc = "";
                        if (stbl.ContainsKey(descGUID)) { desc = stbl[descGUID]; stbl.Remove(descGUID); }
                        if (ckbCopyToAll.Checked || item.rk.Instance >> 56 == 0x00) desc = tbCatlgDesc.Text;
                        stbl.Add(newDescGUID, desc);

                        dirty = true;
                        #endregion
                    }
                    else if (item.ResourceIndexEntry.ResourceType == 0x0333406C)
                    {
                        dirty = UpdateRKsFromXML(item);
                    }
                    else
                    {
                        dirty = UpdateRKsFromField((AResource)item.Resource);
                    }

                    if (dirty) item.Commit();

                }

                #region PadSTBLs
                english = null;//reload
                if (isPadSTBLs && English != null)
                {
                    for (int i = 0; i < 0x17; i++)
                    {
                        STBL lang = new STBL(objPkgs, (byte)i);
                        if (lang.rk != RK.NULL) continue;

                        string name = English[nameGUID];
                        string desc = English[descGUID];
                        if (name == null) name = tbCatlgName.Text;
                        if (desc == null) desc = tbCatlgDesc.Text;

                        IResourceKey newRK = new RK(English.rk);
                        newRK.Instance = English.rk.Instance | ((ulong)i << 56);
                        RIE rie = new RIE(objPkgs[0], objPkgs[0].AddResource(newRK, null, true));
                        Item newstbl = new Item(rie);
                        IDictionary<ulong, string> outstbl = (IDictionary<ulong, string>)newstbl.Resource;

                        outstbl.Add(newNameGUID, name);
                        outstbl.Add(newDescGUID, desc);
                        newstbl.Commit();

                        if (NMap != null && NMap.map != null && !NMap.map.ContainsKey(newRK.Instance))
                            NMap.map.Add(newRK.Instance, String.Format(language_fmt, languages[i], i, English.rk.Instance));

                        if (!rkToItem.ContainsKey(newRK))
                            rkToItem.Add(newRK, newstbl);
                    }
                    NMap.Commit();
                }
                #endregion

                foreach (var kvp in rkToItemAdded)
                    if (!rkToItem.ContainsKey(kvp.Key)) rkToItem.Add(kvp.Key, kvp.Value);
                foreach (Item item in rkToItem.Values)
                {
                    if (item.rk == RK.NULL) { continue; }
                    else if (!oldToNew.ContainsKey(item.ResourceIndexEntry.Instance)) { continue; }
                    else item.ResourceIndexEntry.Instance = oldToNew[item.ResourceIndexEntry.Instance];
                }

                if (!disableCompression)
                    foreach (IResourceIndexEntry ie in objPkgs[0].GetResourceList) ie.Compressed = 0xffff;

                objPkgs[0].SavePackage();
            }
            finally
            {
                this.toolStripProgressBar1.Visible = false;
                this.toolStripStatusLabel1.Visible = false;
                StopWait();
            }

            ClosePkg();
            CopyableMessageBox.Show("OK", myName, CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Information);
            DisplayNothing();
        }

        int numNewInstances = 0;
        ulong CreateInstance() { numNewInstances++; return FNV64.GetHash(numNewInstances.ToString("X8") + "_" + UniqueObject + "_" + DateTime.UtcNow.ToBinary().ToString("X16")); }
        ulong CreateInstance32() { numNewInstances++; return FNV32.GetHash(numNewInstances.ToString("X8") + "_" + UniqueObject + "_" + DateTime.UtcNow.ToBinary().ToString("X16")); }

        private bool UpdateRKsFromField(AApiVersionedFields field)
        {
            bool dirty = false;

            Type t = field.GetType();
            if (typeof(IResourceKey).IsAssignableFrom(t))
            {
                IResourceKey rk = (IResourceKey)field;
                if (rk != RK.NULL && rkToItem.ContainsKey(rk) && oldToNew.ContainsKey(rk.Instance)) { rk.Instance = oldToNew[rk.Instance]; dirty = true; }
            }
            else
            {
                if (typeof(IEnumerable).IsAssignableFrom(field.GetType()))
                    dirty = UpdateRKsFromIEnumerable((IEnumerable)field) || dirty;
                dirty = UpdateRKsFromAApiVersionedFields(field) || dirty;
            }

            return dirty;
        }
        private bool UpdateRKsFromAApiVersionedFields(AApiVersionedFields field)
        {
            bool dirty = false;

            List<string> fields = field.ContentFields;
            foreach (string f in fields)
            {
                if ((new List<string>(new string[] { "Stream", "AsBytes", "Value", })).Contains(f)) continue;

                Type t = AApiVersionedFields.GetContentFieldTypes(0, field.GetType())[f];
                if (!t.IsClass || t.Equals(typeof(string)) || t.Equals(typeof(Boolset))) continue;
                if (t.IsArray && (!t.GetElementType().IsClass || t.GetElementType().Equals(typeof(string)))) continue;

                if (typeof(IEnumerable).IsAssignableFrom(t))
                    dirty = UpdateRKsFromIEnumerable((IEnumerable)field[f].Value) || dirty;
                else if (typeof(AApiVersionedFields).IsAssignableFrom(t))
                    dirty = UpdateRKsFromField((AApiVersionedFields)field[f].Value) || dirty;
            }

            return dirty;
        }
        private bool UpdateRKsFromIEnumerable(IEnumerable list)
        {
            bool dirty = false;

            if (list != null)
                foreach (object o in list)
                    if (typeof(AApiVersionedFields).IsAssignableFrom(o.GetType()))
                        dirty = UpdateRKsFromField((AApiVersionedFields)o) || dirty;
                    else if (typeof(IEnumerable).IsAssignableFrom(o.GetType()))
                        dirty = UpdateRKsFromIEnumerable((IEnumerable)o) || dirty;

            return dirty;
        }
        private bool UpdateRKsFromXML(Item item)
        {
            bool dirty = false;
            StreamReader sr = new StreamReader(item.Resource.Stream, true);
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms, sr.CurrentEncoding);
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                int i = line.IndexOf("key:");
                while (i >= 0 && i + 22 + 16 < line.Length)
                {
                    string iid = line.Substring(i + 22, 16);
                    ulong IID;
                    if (ulong.TryParse(iid, System.Globalization.NumberStyles.HexNumber, null, out IID))
                    {
                        if (oldToNew.ContainsKey(IID))
                        {
                            string newiid = oldToNew[IID].ToString("X16");
                            line = line.Replace(iid, newiid);
                            dirty = true;
                        }
                    }
                    i = line.IndexOf("key:", i + 22 + 16);
                }
                sw.WriteLine(line);
            }
            sw.Flush();
            if (dirty)
            {
                item.Resource.Stream.SetLength(0);
                item.Resource.Stream.Write(ms.ToArray(), 0, (int)ms.Length);
            }
            return dirty;
        }

        #endregion


        #region Fetch resources
        private void Add(string key, IResourceKey rk)
        {
            if (resourceList.Count % 100 == 0) Application.DoEvents();
            rkLookup.Add(key, rk);

            TGIN tgin = (AResourceKey)rk;
            tgin.ResName = key;
            resourceList.Add(tgin);
        }

        private void SlurpRKsFromRK(string key, IResourceKey rk)
        {
            Item item = new Item(objPkgs, rk);
            if (item.ResourceIndexEntry != null) SlurpRKsFromField(key, (AResource)item.Resource);
            else Diagnostics.Show(String.Format("RK {0} not found", key));
        }
        private void SlurpRKsFromField(string key, AApiVersionedFields field)
        {
            Type t = field.GetType();
            if (typeof(GenericRCOLResource.ChunkEntry).IsAssignableFrom(t)) { }
            else if (typeof(AResource.TGIBlock).IsAssignableFrom(t))
            {
                Add(key, (IResourceKey)field);
            }
            else
            {
                if (typeof(IEnumerable).IsAssignableFrom(t))
                    SlurpRKsFromIEnumerable(key, (IEnumerable)field);
                SlurpRKsFromAApiVersionedFields(key, field);
            }
        }
        private void SlurpRKsFromAApiVersionedFields(string key, AApiVersionedFields field)
        {
            List<string> fields = field.ContentFields;
            foreach (string f in fields)
            {
                if ((new List<string>(new string[] { "Stream", "AsBytes", "Value", })).Contains(f)) continue;

                Type t = AApiVersionedFields.GetContentFieldTypes(0, field.GetType())[f];
                if (!t.IsClass || t.Equals(typeof(string)) || t.Equals(typeof(Boolset))) continue;
                if (t.IsArray && (!t.GetElementType().IsClass || t.GetElementType().Equals(typeof(string)))) continue;

                if (typeof(IEnumerable).IsAssignableFrom(t))
                    SlurpRKsFromIEnumerable(key + "." + f, (IEnumerable)field[f].Value);
                else if (typeof(AApiVersionedFields).IsAssignableFrom(t))
                    SlurpRKsFromField(key + "." + f, (AApiVersionedFields)field[f].Value);
            }
        }
        private void SlurpRKsFromIEnumerable(string key, IEnumerable list)
        {
            if (list == null) return;
            int i = 0;
            foreach (object o in list)
            {
                if (typeof(AApiVersionedFields).IsAssignableFrom(o.GetType()))
                {
                    SlurpRKsFromField(key + "[" + i + "]", (AApiVersionedFields)o);
                    i++;
                }
                else if (typeof(IEnumerable).IsAssignableFrom(o.GetType()))
                {
                    SlurpRKsFromIEnumerable(key + "[" + i + "]", (IEnumerable)o);
                    i++;
                }
            }
        }
        private void SlurpRKsFromXML(string key, Item item)
        {
            int j = 0;
            StreamReader sr = new StreamReader(item.Resource.Stream, true);
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                int i = line.IndexOf("key:");
                while (i >= 0 && i + 38 < line.Length)
                {
                    TGIN field = new TGIN();
                    if (uint.TryParse(line.Substring(i + 4, 8), System.Globalization.NumberStyles.HexNumber, null, out field.ResType)
                        && uint.TryParse(line.Substring(i + 13, 8), System.Globalization.NumberStyles.HexNumber, null, out field.ResGroup)
                        && ulong.TryParse(line.Substring(i + 22, 16), System.Globalization.NumberStyles.HexNumber, null, out field.ResInstance))
                        Add(key + "[" + (j++) + "]", (AResourceKey)field);
                    i = line.IndexOf("key:", i + 38);
                }
            }
        }

        private void SlurpKindred(string key, IList<IPackage> pkgs, string[] fields, TypedValue[] values)
        {
            string s = "";
            ListIResourceKey seen = new ListIResourceKey();
            foreach (IPackage pkg in pkgs)
            {
                IList<IResourceIndexEntry> lrie = pkg.FindAll(fields, values);
                int i = 0;
                foreach (IResourceIndexEntry rie in lrie)
                {
                    if (seen.Contains(rie)) continue;
                    seen.Add(rie);
                    Add(key + "[" + i + "]", rie);
                    Item item = new Item(new RIE(objPkgs, rie));
                    if (item.Resource != null)
                        vpxyKinItems.Add(item);
                    else
                        s += String.Format("VPKY kin RK {0} not found", (IResourceKey)rie);
                    i++;
                }
            }
            Diagnostics.Show(s, "Missing VPXY Kin");
        }
        #endregion


        #region Menu Bar
        private void menuBarWidget1_MBDropDownOpening(object sender, MenuBarWidget.MBDropDownOpeningEventArgs mn)
        {
            switch (mn.mn)
            {
                case MenuBarWidget.MD.MBF: break;
                case MenuBarWidget.MD.MBC: break;
                case MenuBarWidget.MD.MBV: break;
                case MenuBarWidget.MD.MBT: break;
                case MenuBarWidget.MD.MBS: break;
                case MenuBarWidget.MD.MBH: break;
                default: break;
            }
        }

        #region File menu
        private void menuBarWidget1_MBFile_Click(object sender, MenuBarWidget.MBClickEventArgs mn)
        {
            try
            {
                this.Enabled = false;
                Application.DoEvents();
                switch (mn.mn)
                {
                    case MenuBarWidget.MB.MBF_open: fileOpen(); break;
                    case MenuBarWidget.MB.MBF_exit: fileExit(); break;
                }
            }
            finally { this.Enabled = true; }
        }

        string currentPackage = "";
        private void fileOpen()
        {
            openPackageDialog.InitialDirectory = ObjectCloner.Properties.Settings.Default.LastSaveFolder == null || ObjectCloner.Properties.Settings.Default.LastSaveFolder.Length == 0
                ? "" : ObjectCloner.Properties.Settings.Default.LastSaveFolder;
            openPackageDialog.FileName = "*.package";
            DialogResult dr = openPackageDialog.ShowDialog();
            if (dr != DialogResult.OK) return;
            ObjectCloner.Properties.Settings.Default.LastSaveFolder = Path.GetDirectoryName(openPackageDialog.FileName);

            fileReOpenToFix(openPackageDialog.FileName, 0);
        }

        void fileReOpenToFix(string filename, CatalogType type)
        {
            ClosePkg();
            IPackage pkg;
            try
            {
                pkg = s3pi.Package.Package.OpenPackage(0, filename, true);
            }
            catch (Exception ex)
            {
                CopyableMessageBox.IssueException(ex, "Could not open package:\n" + filename, "File Open");
                return;
            }

            currentCatalogType = 0;
            currentPackage = filename;
            tmbPkgs = ddsPkgs = objPkgs = new List<IPackage>(new IPackage[] { pkg, });

            mode = Mode.Fix;
            fileNewOpen(type, type != 0);
        }

        void fileNewOpen(CatalogType resourceType, bool IsFixPass)
        {
            menuBarWidget1.Enable(MenuBarWidget.MB.MBF_open, false);
            menuBarWidget1.Enable(MenuBarWidget.MD.MBC, false);
            menuBarWidget1.Enable(MenuBarWidget.MD.MBT, false);
            menuBarWidget1.Enable(MenuBarWidget.MD.MBS, false);

            DoWait("Please wait, loading object catalog...");

            if (!haveLoaded)
                StartLoading(resourceType, IsFixPass);
            else
                DisplayObjectChooser();
        }

        private void fileExit()
        {
            Application.Exit();
        }
        #endregion

        #region Cloning menu
        CatalogType currentCatalogType = 0;
        private void menuBarWidget1_MBCloning_Click(object sender, MenuBarWidget.MBClickEventArgs mn)
        {
            Application.DoEvents();
            cloneType(FromCloningMenuEntry(mn.mn));
        }
        CatalogType FromCloningMenuEntry(MenuBarWidget.MB menuEntry)
        {
            if (!Enum.IsDefined(typeof(MenuBarWidget.MB), menuEntry)) return 0;
            List<MenuBarWidget.MB> ml = new List<MenuBarWidget.MB>((MenuBarWidget.MB[])Enum.GetValues(typeof(MenuBarWidget.MB)));
            return ((CatalogType[])Enum.GetValues(typeof(CatalogType)))[ml.IndexOf(menuEntry) - ml.IndexOf(MenuBarWidget.MB.MBC_cfen)];
        }

        private void cloneType(CatalogType resourceType)
        {
            if (!CheckInstallDirs()) return;

            if (currentCatalogType != resourceType)
            {
                ClosePkg();
                setList(out objPkgs, ini_fb0);
                setList(out ddsPkgs, ini_fb2);
                setList(out tmbPkgs, ini_tmb);
                currentCatalogType = resourceType;
            }

            mode = Mode.Clone;
            fileNewOpen(resourceType, false);
        }

        private bool CheckInstallDirs()
        {
            if (!doCheckPackageLists())
            {
                CopyableMessageBox.Show("Found no packages\nPlease check your Game Folder settings.", "No objects to clone",
                    CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Stop);
                return false;
            }
            return true;
        }
        bool doCheckPackageLists() { return ini_fb0 != null && ini_fb2 != null && ini_tmb != null && ini_fb0.Count > 0; }
        void setList(out List<IPackage> pkgs, List<string> paths)
        {
            pkgs = new List<IPackage>();

            if (paths == null) return;
            foreach (string file in paths)
                if (File.Exists(file))
                    try { pkgs.Add(s3pi.Package.Package.OpenPackage(0, file)); }
                    catch { }
        }
        #endregion

        #region View menu
        private void menuBarWidget1_MBView_Click(object sender, MenuBarWidget.MBClickEventArgs mn)
        {
            try
            {
                this.Enabled = false;
                Application.DoEvents();

                switch (mn.mn)
                {
                    case MenuBarWidget.MB.MBV_tiles: viewSetView(mn.mn, LItoIMG64); break;
                    case MenuBarWidget.MB.MBV_largeIcons: viewSetView(mn.mn, LItoIMG64); break;
                    case MenuBarWidget.MB.MBV_smallIcons: viewSetView(mn.mn, LItoIMG32); break;
                    case MenuBarWidget.MB.MBV_list: viewSetView(mn.mn, LItoIMG32); break;
                    case MenuBarWidget.MB.MBV_detailedList: viewSetView(mn.mn, LItoIMG32); break;
                    case MenuBarWidget.MB.MBV_icons: viewIcons(); break;
                }
            }
            finally { this.Enabled = true; }
        }

        private void viewSetView(MenuBarWidget.MB mn, Dictionary<int, int> imageMap)
        {
            if (menuBarWidget1.IsChecked(mn)) return;
            if (viewMap.ContainsKey(currentView)) menuBarWidget1.Checked(viewMap[currentView], false);
            menuBarWidget1.Checked(mn, true);

            if (menuBarWidget1.IsChecked(MenuBarWidget.MB.MBV_icons) && (LItoIMG64.Count > 0 || LItoIMG32.Count > 0))
                for (int i = 0; i < objectChooser.Items.Count; i++)
                    objectChooser.Items[i].ImageIndex = imageMap.ContainsKey(i) ? imageMap[i] : -1;

            ObjectCloner.Properties.Settings.Default.View = viewMapValues.IndexOf(mn);
            currentView = viewMapKeys[ObjectCloner.Properties.Settings.Default.View];
            objectChooser.View = currentView;
        }

        private void viewIcons()
        {
            menuBarWidget1.Checked(MenuBarWidget.MB.MBV_icons, !menuBarWidget1.IsChecked(MenuBarWidget.MB.MBV_icons));
            if (haveLoaded)
            {
                if (menuBarWidget1.IsChecked(MenuBarWidget.MB.MBV_icons))
                {
                    if (waitingForImages) return;

                    if (haveFetched)
                    {
                        if (LItoIMG64.Count > 0 || LItoIMG32.Count > 0)
                        {
                            Dictionary<int, int> imageMap = (currentView == View.Tile || currentView == View.LargeIcon) ? LItoIMG64 : LItoIMG32;
                            for (int i = 0; i < objectChooser.Items.Count; i++)
                                objectChooser.Items[i].ImageIndex = imageMap.ContainsKey(i) ? imageMap[i] : -1;
                        }
                    }
                    else
                    {
                        waitingForImages = true;
                        StartFetching();
                    }
                }
                else
                {
                    AbortFetching(false);
                    for (int i = 0; i < objectChooser.Items.Count; i++)
                        objectChooser.Items[i].ImageIndex = -1;
                }
            }
        }
        #endregion

        #region Tools menu
        private void menuBarWidget1_MBTools_Click(object sender, MenuBarWidget.MBClickEventArgs mn)
        {
            try
            {
                this.Enabled = false;
                Application.DoEvents();
                switch (mn.mn)
                {
                    case MenuBarWidget.MB.MBT_search: toolsSearch(); break;
                }
            }
            finally { this.Enabled = true; }
        }

        private void toolsSearch()
        {
            if (!CheckInstallDirs()) return;

            ClosePkg();
            setList(out objPkgs, ini_fb0);
            setList(out ddsPkgs, ini_fb2);
            setList(out tmbPkgs, ini_tmb);
            currentCatalogType = 0;

            searchPane = new Search(objPkgs, updateProgress);

            DisplaySearch();
        }
        #endregion

        #region Settings menu
        private void menuBarWidget1_MBSettings_Click(object sender, MenuBarWidget.MBClickEventArgs mn)
        {
            try
            {
                this.Enabled = false;
                Application.DoEvents();
                switch (mn.mn)
                {
                    case MenuBarWidget.MB.MBS_sims3Folder: settingsGameFolders(); break;
                    case MenuBarWidget.MB.MBS_userName: settingsUserName(); break;
                    case MenuBarWidget.MB.MBS_diagnostics: settingsDiagnostics(); break;
                }
            }
            finally { this.Enabled = true; }
        }

        private void settingsGameFolders()
        {
            while (true)
            {
                SettingsForms.GameFolders gf = new ObjectCloner.SettingsForms.GameFolders();
                DialogResult dr = gf.ShowDialog();
                if (dr != DialogResult.OK && dr!= DialogResult.Retry) return;
                if (dr != DialogResult.Retry) break;
            }
            ClosePkg();
            currentCatalogType = 0;
            tabType = 0;
            haveLoaded = false;
            DisplayNothing();
        }

        private void settingsUserName()
        {
            StringInputDialog cn = new StringInputDialog();
            cn.Caption = "Creator name";
            cn.Prompt = "Your creator name will be used by default\nto create new object names";
            cn.Value = ObjectCloner.Properties.Settings.Default.CreatorName == null || ObjectCloner.Properties.Settings.Default.CreatorName.Length == 0
                ? Environment.UserName : ObjectCloner.Properties.Settings.Default.CreatorName;
            DialogResult dr = cn.ShowDialog();
            if (dr != DialogResult.OK) return;
            ObjectCloner.Properties.Settings.Default.CreatorName = cn.Value;
        }

        private void settingsDiagnostics()
        {
            Application.DoEvents();
            ObjectCloner.Properties.Settings.Default.Diagnostics = Diagnostics.Enabled = !Diagnostics.Enabled;
            menuBarWidget1.Checked(MenuBarWidget.MB.MBS_diagnostics, Diagnostics.Enabled);
        }
        #endregion

        #region Help menu
        private void menuBarWidget1_MBHelp_Click(object sender, MenuBarWidget.MBClickEventArgs mn)
        {
            try
            {
                this.Enabled = false;
                Application.DoEvents();
                switch (mn.mn)
                {
                    case MenuBarWidget.MB.MBH_contents: helpContents(); break;
                    case MenuBarWidget.MB.MBH_about: helpAbout(); break;
                    case MenuBarWidget.MB.MBH_warranty: helpWarranty(); break;
                    case MenuBarWidget.MB.MBH_licence: helpLicence(); break;
                }
            }
            finally { this.Enabled = true; }
        }

        private void helpContents()
        {
            string locale = System.Globalization.CultureInfo.CurrentUICulture.Name;

            string baseFolder = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "HelpFiles");
            if (Directory.Exists(Path.Combine(baseFolder, locale)))
                baseFolder = Path.Combine(baseFolder, locale);
            else if (Directory.Exists(Path.Combine(baseFolder, locale.Substring(0, 2))))
                baseFolder = Path.Combine(baseFolder, locale.Substring(0, 2));

            if (File.Exists(Path.Combine(baseFolder, "Contents.htm")))
                Help.ShowHelp(this, "file:///" + Path.Combine(baseFolder, "Contents.htm"));
            else
                helpAbout();
        }

        private void helpAbout()
        {
            string copyright = "\n" +
                this.Text + "  Copyright (C) 2009  Peter L Jones\n" +
                "\n" +
                "This program comes with ABSOLUTELY NO WARRANTY; for details see Help->Warranty.\n" +
                "\n" +
                "This is free software, and you are welcome to redistribute it\n" +
                "under certain conditions; see Help->Licence for details.\n";
            CopyableMessageBox.Show(String.Format(
                "{0}\n" +
                "Front-end Distribution: {1}\n" +
                "Library Distribution: {2}"
                , copyright
                , getVersion(typeof(MainForm), "s3oc")
                , getVersion(typeof(s3pi.Interfaces.AApiVersionedFields), "s3oc")
                ), this.Text);
        }

        private string getVersion(Type type, string p)
        {
            string s = getString(Path.Combine(Path.GetDirectoryName(type.Assembly.Location), p + "-Version.txt"));
            return s == null ? "Unknown" : s;
        }

        private string getString(string file)
        {
            if (!File.Exists(file)) return null;
            FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
            StreamReader t = new StreamReader(fs);
            return t.ReadLine();
        }

        private void helpWarranty()
        {
            CopyableMessageBox.Show("\n" +
                "Disclaimer of Warranty.\n" +
                "\n" +
                "THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW. EXCEPT WHEN OTHERWISE STATED " +
                "IN WRITING THE COPYRIGHT HOLDERS AND/OR OTHER PARTIES PROVIDE THE PROGRAM \"AS IS\" WITHOUT WARRANTY OF ANY " +
                "KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND " +
                "FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. " +
                "SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.\n" +
                "\n" +
                "\n" +
                "Limitation of Liability.\n" +
                "\n" +
                "IN NO EVENT UNLESS REQUIRED BY APPLICABLE LAW OR AGREED TO IN WRITING WILL ANY COPYRIGHT HOLDER, OR ANY OTHER " +
                "PARTY WHO MODIFIES AND/OR CONVEYS THE PROGRAM AS PERMITTED ABOVE, BE LIABLE TO YOU FOR DAMAGES, INCLUDING ANY " +
                "GENERAL, SPECIAL, INCIDENTAL OR CONSEQUENTIAL DAMAGES ARISING OUT OF THE USE OR INABILITY TO USE THE PROGRAM " +
                "(INCLUDING BUT NOT LIMITED TO LOSS OF DATA OR DATA BEING RENDERED INACCURATE OR LOSSES SUSTAINED BY YOU OR " +
                "THIRD PARTIES OR A FAILURE OF THE PROGRAM TO OPERATE WITH ANY OTHER PROGRAMS), EVEN IF SUCH HOLDER OR OTHER " +
                "PARTY HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.\n" +
                "\n",
                this.Text);

        }

        private void helpLicence()
        {
            int dr = CopyableMessageBox.Show("\n" +
                "This program is distributed under the terms of the\n" +
                "GNU General Public Licence version 3.\n" +
                "\n" +
                "If you wish to see the full text of the licence,\n" +
                "please visit http://www.fsf.org/licensing/licenses/gpl.html.\n" +
                "\n" +
                "Do you wish to visit this site now?" +
                "\n",
                this.Text,
                CopyableMessageBoxButtons.YesNo, CopyableMessageBoxIcon.Question, 1);
            if (dr != 0) return;
            Help.ShowHelp(this, "http://www.fsf.org/licensing/licenses/gpl.html");
        }
        #endregion

        #endregion

        #region Steps
        Item objkItem;
        List<Item> vpxyItems;
        List<Item> modlItems;
        List<Item> vpxyKinItems;
        
        delegate void Step();
        void None() { }
        List<Step> stepList;
        Step step;
        int stepNum;
        int lastInChain;
        void SetStepList(Item item, out List<Step> stepList)
        {
            stepList = null;

            if (item == null)
                return;

            Step lastStepInChain = None;
            stepList = new List<Step>(new Step[] { Item_addSelf, });

            switch (item.CType)
            {
                case CatalogType.CatalogObject:
                    OBJD_Steps(stepList, out lastStepInChain); break;
                case CatalogType.ModularResource:
                    MDLR_Steps(stepList, out lastStepInChain); break;
                case CatalogType.CatalogFence:
                case CatalogType.CatalogStairs:
                case CatalogType.CatalogRailing:
                case CatalogType.CatalogRoofPattern:
                    Common_Steps(stepList, out lastStepInChain); break;
                case CatalogType.CatalogTerrainPaintBrush:
                    CTPT_Steps(stepList, out lastStepInChain); break;
                case CatalogType.CatalogFireplace:
                    CFIR_Steps(stepList, out lastStepInChain); break;
                case CatalogType.CatalogWallFloorPattern:
                    CWAL_Steps(stepList, out lastStepInChain); break;
            }
            lastInChain = stepList == null ? -1 : (stepList.IndexOf(lastStepInChain) + 1);
        }

        void OBJD_Steps(List<Step> stepList, out Step lastStepInChain)
        {
            lastStepInChain = None;
            if (isClone || cloneFixOptions.IsRenumber)
            {
                stepList.AddRange(new Step[] {
                    // either OBJD_SlurpDDSes or Catlg_SlurpTGIs
                    OBJD_removeRefdOBJDs,
                    OBJD_getOBKJ,
                    // OBJD_addOBJKref if default resources only
                    OBJK_SlurpTGIs,
                    OBJK_getVPXY,
                    Catlg_addVPXYs,

                    VPXYs_SlurpRKs,
                    // VPXYs_getKinXML, VPXYs_getKinMTST if NOT default resources only
                    VPXYs_getMODLs,
                    VPXYs_getKinXML,
                    VPXYs_getKinMTST,
                    VPXYKin_SlurpRKs,

                    MODLs_SlurpRKs,
                    MODLs_SlurpMLODs,
                    MODLs_SlurpTXTCs,
                });
                lastStepInChain = MODLs_SlurpTXTCs;
                if (mode == Mode.Clone)
                {
                    stepList.Insert(stepList.IndexOf(OBJD_getOBKJ), OBJD_setFallback);
                }
                if (cloneFixOptions.IsDefaultOnly)
                {
                    stepList.Insert(stepList.IndexOf(OBJD_removeRefdOBJDs), OBJD_SlurpDDSes);
                    stepList.Insert(stepList.IndexOf(OBJK_SlurpTGIs), OBJD_addOBJKref);
                }
                else
                {
                    stepList.Insert(stepList.IndexOf(OBJD_removeRefdOBJDs), Catlg_SlurpRKs);
                }
                if (cloneFixOptions.IsIncludeThumbnails || (!isClone && cloneFixOptions.IsRenumber))
                    stepList.Add(SlurpThumbnails);
            }
            lastStepInChain = MODLs_SlurpTXTCs;
        }

        void MDLR_Steps(List<Step> stepList, out Step lastStepInChain)
        {
            stepList.AddRange(new Step[] { Item_findObjds, setupObjdStepList, Modular_Main, });
            lastStepInChain = None;
        }

        void Common_Steps(List<Step> stepList, out Step lastStepInChain)
        {
            lastStepInChain = None;
            if (isClone || cloneFixOptions.IsRenumber)
            {
                stepList.AddRange(new Step[] {
                    Catlg_getVPXY,
                    Catlg_addVPXYs,

                    VPXYs_SlurpRKs,
                    // VPXYs_getKinXML, VPXYs_getKinMTST if NOT default resources only
                    VPXYs_getMODLs,
                    VPXYs_getKinXML,
                    VPXYs_getKinMTST,
                    VPXYKin_SlurpRKs,

                    MODLs_SlurpRKs,
                    MODLs_SlurpMLODs,
                    MODLs_SlurpTXTCs,
                });
                lastStepInChain = MODLs_SlurpTXTCs;
                if (cloneFixOptions.IsDefaultOnly)
                {
                }
                else
                {
                    //stepList.Insert(stepList.IndexOf(Catlg_getVPXY), Catlg_SlurpTGIs);// Causes problems for CSTR and doesn't help for others
                }
                if (cloneFixOptions.IsIncludeThumbnails || (!isClone && cloneFixOptions.IsRenumber))
                    stepList.Add(SlurpThumbnails);
            }
        }

        void CTPT_Steps(List<Step> stepList, out Step lastStepInChain)
        {
            lastStepInChain = None;
            if (isClone || cloneFixOptions.IsRenumber)
            {
                stepList.AddRange(new Step[] {
                    CTPT_addPair,
                    CTPT_addBrushTexture,
                    //CTPT_addBrushShape if NOT default resources only
                });
                if (cloneFixOptions.IsDefaultOnly)
                {
                }
                else
                {
                    //stepList.Insert(stepList.IndexOf(Catlg_getVPXY), Catlg_SlurpTGIs);// Causes problems for CSTR and doesn't help for others
                    stepList.Insert(stepList.IndexOf(SlurpThumbnails), CTPT_addBrushShape);
                    //stepList.Insert(stepList.IndexOf(SlurpThumbnails), VPXYs_getKinXML);//No VPXYs in here
                    //stepList.Insert(stepList.IndexOf(SlurpThumbnails), VPXYs_getKinMTST);//No VPXYs in here
                }
                if (cloneFixOptions.IsIncludeThumbnails || (!isClone && cloneFixOptions.IsRenumber))
                    stepList.Add(SlurpThumbnails);
            }
        }

        void CFIR_Steps(List<Step> stepList, out Step lastStepInChain)
        {
            stepList.AddRange(new Step[] { Item_findObjds, setupObjdStepList, Modular_Main, });
            if (cloneFixOptions.IsIncludeThumbnails || (!isClone && cloneFixOptions.IsRenumber))
                stepList.Add(SlurpThumbnails);
            lastStepInChain = None;
        }

        void CWAL_Steps(List<Step> stepList, out Step lastStepInChain)
        {
            lastStepInChain = None;
            if (isClone || cloneFixOptions.IsRenumber)
            {
                stepList.AddRange(new Step[] {
                    Catlg_getVPXY,
                    Catlg_addVPXYs,

                    VPXYs_SlurpRKs,
                    // VPXYs_getKinXML, VPXYs_getKinMTST if NOT default textures only
                    VPXYs_getMODLs,
                    VPXYs_getKinXML,
                    VPXYs_getKinMTST,
                    VPXYKin_SlurpRKs,

                    MODLs_SlurpRKs,
                    MODLs_SlurpMLODs,
                    MODLs_SlurpTXTCs,
                });
                lastStepInChain = MODLs_SlurpTXTCs;
                if (cloneFixOptions.IsDefaultOnly)
                {
                }
                else
                {
                    stepList.Insert(stepList.IndexOf(Catlg_getVPXY), Catlg_SlurpRKs);
                }
                if (cloneFixOptions.IsIncludeThumbnails || (!isClone && cloneFixOptions.IsRenumber))
                    stepList.Add(CWAL_SlurpThumbnails);
            }
        }

        Dictionary<Step, string> StepText;
        void SetStepText()
        {
            StepText = new Dictionary<Step, string>();
            StepText.Add(Item_addSelf, "Add selected item");

            StepText.Add(OBJD_setFallback, "Set fallback TGI");
            StepText.Add(OBJD_removeRefdOBJDs, "Remove referenced OBJDs");
            StepText.Add(OBJD_getOBKJ, "Find OBJK");
            StepText.Add(OBJD_addOBJKref, "Add OBJK");
            StepText.Add(OBJD_SlurpDDSes, "OBJD-referenced resources");
            StepText.Add(Catlg_SlurpRKs, "Catalog object-referenced resources");
            StepText.Add(OBJK_SlurpTGIs, "OBJK-referenced resources");
            StepText.Add(OBJK_getVPXY, "Find OBJK-referenced VPXY");

            StepText.Add(Catlg_getVPXY, "Find VPXYs in the Catalog Resource TGIBlockList");

            StepText.Add(CTPT_addPair, "Add the other brush in pair");
            StepText.Add(CTPT_addBrushTexture, "Add Brush Texture");
            StepText.Add(CTPT_addBrushShape, "Add Brush Shape");

            StepText.Add(Catlg_addVPXYs, "Add VPXY resources");
            StepText.Add(VPXYs_SlurpRKs, "VPXY-referenced resources");
            StepText.Add(VPXYs_getKinXML, "Preset XML (same instance as VPXY)");
            StepText.Add(VPXYs_getKinMTST, "MTST (same instance as VPXY)");
            StepText.Add(VPXYKin_SlurpRKs, "Find Preset XML and MTST referenced resources");
            StepText.Add(VPXYs_getMODLs, "Find VPXY-referenced MODLs");
            StepText.Add(MODLs_SlurpRKs, "MODL-referenced resources");
            StepText.Add(MODLs_SlurpMLODs, "MLOD-referenced resources");
            StepText.Add(MODLs_SlurpTXTCs, "TXTC-referenced resources");
            StepText.Add(SlurpThumbnails, "Add thumbnails");
            StepText.Add(CWAL_SlurpThumbnails, "Add thumbnails");
            StepText.Add(FixIntegrity, "Fix integrity step");

            StepText.Add(Item_findObjds, "Find OBJDs from MDLR/CFIR");
            StepText.Add(setupObjdStepList, "Get the OBJD step list");
            StepText.Add(Modular_Main, "Drive the modular process");
        }

        void Item_addSelf() { Add("clone", selectedItem.rk); }
        void Catlg_SlurpRKs() { SlurpRKsFromField("clone", (AResource)selectedItem.Resource); }
        void Catlg_getVPXY()
        {
            vpxyItems = new List<Item>();
            vpxyKinItems = new List<Item>();

            string s = "";
            foreach (IResourceKey rk in (AResource.TGIBlockList)selectedItem.Resource["TGIBlocks"].Value)
            {
                if (rk.ResourceType != 0x736884F1) continue;
                Item vpxy = new Item(new RIE(objPkgs, rk));
                if (vpxy.Resource != null)
                    vpxyItems.Add(vpxy);
                else
                    s += String.Format("Catalog Resource {0} -> RK {1}: not found\n", (IResourceKey)selectedItem.ResourceIndexEntry, rk);
            }
            Diagnostics.Show(s, "Missing VPXYs");
            if (vpxyItems.Count == 0)
            {
                Diagnostics.Show(String.Format("Catalog Resource {0} has no VPXY items", (IResourceKey)selectedItem.ResourceIndexEntry), "No VPXY items");
                stepNum = lastInChain;
            }
        }
        void Catlg_addVPXYs() { for (int i = 0; i < vpxyItems.Count; i++) Add("vpxy[" + i + "]", vpxyItems[i].rk); }

        #region OBJD Steps
        void OBJD_setFallback()
        {
            if ((selectedItem.rk.ResourceGroup >> 27) > 0) return;// Only base game objects

            int fallbackIndex = (int)(uint)selectedItem.Resource["FallbackIndex"].Value;
            AResource.TGIBlockList tgiBlocks = selectedItem.Resource["TGIBlocks"].Value as AResource.TGIBlockList;
            if (tgiBlocks[fallbackIndex].Equals(RK.NULL))
            {
                selectedItem.Resource["FallbackIndex"] = new TypedValue(typeof(uint), (uint)tgiBlocks.Count, "X");
                tgiBlocks.Add("TGI", selectedItem.rk.ResourceType, selectedItem.rk.ResourceGroup, selectedItem.rk.Instance);
                selectedItem.Commit();
            }
        }
        void OBJD_getOBKJ()
        {
            uint index = (uint)selectedItem.Resource["OBJKIndex"].Value;
            IList<AResource.TGIBlock> ltgi = (IList<AResource.TGIBlock>)selectedItem.Resource["TGIBlocks"].Value;
            AResource.TGIBlock objkTGI = ltgi[(int)index];
            objkItem = new Item(objPkgs, objkTGI);
            if (objkItem == null)
            {
                Diagnostics.Show(String.Format("OBJK {0} -> OBJK {1}: not found\n", (IResourceKey)selectedItem.ResourceIndexEntry, objkTGI), "Missing OBJK");
                stepNum = lastInChain;
            }

        }
        void OBJD_addOBJKref() { Add("objk", objkItem.rk); }
        void OBJD_SlurpDDSes()
        {
            IList<AResource.TGIBlock> ltgi = (IList<AResource.TGIBlock>)selectedItem.Resource["TGIBlocks"].Value;
            int i = 0;
            foreach (AApiVersionedFields mtdoor in (IList)selectedItem.Resource["MTDoors"].Value)
            {
                Add("clone.wallmask[" + i + "]", ltgi[(int)(uint)mtdoor["WallMaskIndex"].Value]);
                i++;
            }
            Add("clone.sinkmask", ltgi[(int)(uint)selectedItem.Resource["SinkDDSIndex"].Value]);
        }
        void OBJK_SlurpTGIs() { SlurpRKsFromField("objk", (AResource)objkItem.Resource); }
        void OBJK_getVPXY()
        {
            int index = -1;
            if (((ObjKeyResource.ObjKeyResource)objkItem.Resource).ComponentData.ContainsKey("modelKey"))
                index = ((ObjKeyResource.ObjKeyResource.CDTResourceKey)((ObjKeyResource.ObjKeyResource)objkItem.Resource).ComponentData["modelKey"]).Data;

            if (index == -1)
            {
                Diagnostics.Show(String.Format("OBJK {0} has no modelKey", (IResourceKey)objkItem.ResourceIndexEntry), "Missing modelKey");
                stepNum = lastInChain;//Skip past the chain
                return;
            }

            vpxyItems = new List<Item>();
            vpxyKinItems = new List<Item>();

            string s = "";
            foreach (IResourceKey rk in (IList<AResource.TGIBlock>)objkItem.Resource["TGIBlocks"].Value)
            {
                if (rk.ResourceType != 0x736884F1) continue;
                Item vpxy = new Item(new RIE(objPkgs, rk));
                if (vpxy.ResourceIndexEntry != null && vpxy.Resource != null)
                    vpxyItems.Add(vpxy);
                else
                    s += String.Format("OBJK {0} -> RK {1}: not found\n", (IResourceKey)objkItem.ResourceIndexEntry, rk);
            }
            Diagnostics.Show(s, "Missing VPXYs");
            if (vpxyItems.Count == 0)
            {
                Diagnostics.Show(String.Format("OBJK {0} has no VPXY items", (IResourceKey)selectedItem.ResourceIndexEntry), "No VPXY items");
                stepNum = lastInChain;
            }
        }
        void OBJD_removeRefdOBJDs()
        {
            IList<AResource.TGIBlock> ltgi = (IList<AResource.TGIBlock>)selectedItem.Resource["TGIBlocks"].Value;
            foreach (AResourceKey rk in ltgi)
            {
                if (rk.ResourceType == (uint)CatalogType.CatalogObject && rk.Instance != selectedItem.rk.Instance)
                {
                    int i = new List<IResourceKey>(rkLookup.Values).IndexOf(rk);
                    if (i >= 0)
                        rkLookup.Remove(new List<string>(rkLookup.Keys)[i]);
                }
            }
        }
        #endregion

        void CTPT_addPair()
        {
            uint brushIndex = (uint)selectedItem.Resource["BrushIndex"].Value;
            if (CTPTBrushIndexToPair.ContainsKey(brushIndex))
                Add("ctpt_pair", CTPTBrushIndexToPair[brushIndex].rk);
            else
                Diagnostics.Show(String.Format("CTPT {0} BrushIndex {1} not found", selectedItem.rk, brushIndex), "No ctpt_pair item");
        }
        void CTPT_addBrushTexture() { Add("ctpt_BrushTexture", (AResource.TGIBlock)selectedItem.Resource["BrushTexture"].Value); }
        void CTPT_addBrushShape() { Add("ctpt_BrushShape", (AResource.TGIBlock)selectedItem.Resource["BrushShape"].Value); }

        void VPXYs_SlurpRKs()
        {
            for (int i = 0; i < vpxyItems.Count; i++)
            {
                VPXY vpxyChunk = ((GenericRCOLResource)vpxyItems[i].Resource).ChunkEntries[0].RCOLBlock as VPXY;
                SlurpRKsFromField("vpxy[" + i + "]", vpxyChunk);
            }
        }
        void VPXYs_getKinXML()
        {
            for (int i = 0; i < vpxyItems.Count; i++)
                SlurpKindred("vpxy[" + i + "].PresetXML", objPkgs, new string[] { "ResourceType", "Instance" },
                    new TypedValue[] { new TypedValue(typeof(uint), (uint)0x0333406C), new TypedValue(typeof(ulong), vpxyItems[i].rk.Instance) });
        }
        void VPXYs_getKinMTST()
        {
            for (int i = 0; i < vpxyItems.Count; i++)
                SlurpKindred("vpxy[" + i + "].mtst", objPkgs, new string[] { "ResourceType", "Instance" },
                    new TypedValue[] { new TypedValue(typeof(uint), (uint)0x02019972), new TypedValue(typeof(ulong), vpxyItems[i].rk.Instance) });
        }
        void VPXYKin_SlurpRKs()
        {
            int i = 0;
            foreach(Item item in vpxyKinItems)
            {
                if (item.rk.ResourceType == (uint)0x0333406C)
                {
                    SlurpRKsFromXML("PresetXML[" + i + "]", item);
                }
                else
                {
                    SlurpRKsFromField("mtst[" + i + "]", item.Resource as AApiVersionedFields);
                }
                i++;
            }
        }
        void VPXYs_getMODLs()
        {
            modlItems = new List<Item>();
            string s = "";
            for (int i = 0; i < vpxyItems.Count; i++)
            {
                GenericRCOLResource rcol = (vpxyItems[i].Resource as GenericRCOLResource);
                for (int j = 0; j < rcol.ChunkEntries.Count; j++)
                {
                    bool found = false;
                    VPXY vpxychunk = rcol.ChunkEntries[j].RCOLBlock as VPXY;
                    for (int k = 0; k < vpxychunk.TGIBlocks.Count; k++)
                    {
                        AResource.TGIBlock tgib = vpxychunk.TGIBlocks[k];
                        if (tgib.ResourceType != 0x01661233) continue;
                        Item modl = new Item(new RIE(objPkgs, tgib));
                        if (modl.Resource != null)
                        {
                            found = true;
                            modlItems.Add(modl);
                        }
                    }
                    if (!found)
                        s += String.Format("VPXY {0} (chunk {1}) has no MODL items\n", (IResourceKey)vpxyItems[i].ResourceIndexEntry, j);
                }
            }
            Diagnostics.Show(s, "No MODL items");
        }
        void MODLs_SlurpRKs() { for (int i = 0; i < modlItems.Count; i++) SlurpRKsFromField("modl[" + i + "]", (AResource)modlItems[i].Resource); }
        void MODLs_SlurpMLODs()
        {
            int k = 0;
            string s = "";
            for (int i = 0; i < modlItems.Count; i++)
            {
                bool found = false;
                GenericRCOLResource rcol = (modlItems[i].Resource as GenericRCOLResource);
                for (int j = 0; j < rcol.Resources.Count; j++)
                {
                    AResource.TGIBlock tgib = rcol.Resources[j];
                    if (tgib.ResourceType != 0x01D10F34) continue;
                    SlurpRKsFromRK("modl[" + i + "].mlod[" + k + "]", tgib);
                    k++;
                    found = true;
                }
                if (!found)
                    s += String.Format("MODL {0} has no MLOD items\n", (IResourceKey)modlItems[i].ResourceIndexEntry);
            }
            Diagnostics.Show(s, "No MLOD items");
        }
        void MODLs_SlurpTXTCs()
        {
            int k = 0;
            string s = "";
            for (int i = 0; i < modlItems.Count; i++)
            {
                bool found = false;
                GenericRCOLResource rcol = (modlItems[i].Resource as GenericRCOLResource);
                for (int j = 0; j < rcol.Resources.Count; j++)
                {
                    AResource.TGIBlock tgib = rcol.Resources[j];
                    if (tgib.ResourceType != 0x033A1435) continue;
                    SlurpRKsFromRK("modl[" + i + "].txtc[" + k + "]", tgib);
                    k++;
                    found = true;
                }
                if (!found)
                    s += String.Format("MODL {0} has no TXTC items\n", (IResourceKey)modlItems[i].ResourceIndexEntry);
            }
            Diagnostics.Show(s, "No TXTC items");
        }

        List<Item> objdList;
        void Item_findObjds()
        {
            objdList = new List<Item>();
            string s = "";
            foreach (IResourceKey rk in (AResource.TGIBlockList)selectedItem.Resource["TGIBlocks"].Value)
            {
                if (rk.ResourceType != 0x319E4F1D) continue;
                Item objd = new Item(new RIE(objPkgs, rk));
                if (objd.Resource != null)
                    objdList.Add(objd);
                else
                    s += String.Format("OBJD {0}\n", rk);
            }
            Diagnostics.Show(s, String.Format("Item {0} has missing OBJDs:", selectedItem));
        }

        List<Step> objdSteps;
        void setupObjdStepList() { SetStepList(objdList[0], out objdSteps); while (objdSteps.Contains(FixIntegrity)) objdSteps.Remove(FixIntegrity); }

        void Modular_Main()
        {
            Item realSelectedItem = selectedItem;
            int realStepNum = stepNum;
            Step mdlrStep;
            Dictionary<string, IResourceKey> MDLRrkLookup = new Dictionary<string, IResourceKey>();
            foreach (var kvp in rkLookup) MDLRrkLookup.Add(kvp.Key, kvp.Value);

            for (int i = 0; i < objdList.Count; i++)
            {
                rkLookup = new Dictionary<string, IResourceKey>();
                selectedItem = objdList[i];
                stepNum = 0;
                while (stepNum < objdSteps.Count)
                {
                    mdlrStep = objdSteps[stepNum];
                    updateProgress(true, StepText[mdlrStep], true, objdSteps.Count - 1, true, stepNum);
                    Application.DoEvents();
                    stepNum++;
                    mdlrStep();
                }
                foreach (var kvp in rkLookup) MDLRrkLookup.Add("objd[" + i + "]." + kvp.Key, kvp.Value);
            }

            selectedItem = realSelectedItem;
            stepNum = realStepNum;
            rkLookup = MDLRrkLookup;
        }

        //Thumbnails for everything but walls
        //PNGs come from :Objects; Icons come from :Images; Thumbs come from :Thumbnails.
        void SlurpThumbnails()
        {
            foreach (THUM.THUMSize size in new THUM.THUMSize[] { THUM.THUMSize.small, THUM.THUMSize.medium, THUM.THUMSize.large, })
            {
                IResourceKey rk = getImageRK(size, selectedItem);
                if (THUM.PNGTypes[(int)size] == rk.ResourceType)
                    Add(size + "PNG", rk);
                else if (selectedItem.CType == CatalogType.CatalogRoofPattern)
                    Add(size + "Icon", rk);
                else
                    Add(size + "Thumb", rk);
            }
        }
        //0x515CA4CD is very different - but they do come from :Thumbnails, at least.
        void CWAL_SlurpThumbnails()
        {
            Dictionary<THUM.THUMSize, uint> CWALThumbTypes = new Dictionary<THUM.THUMSize, uint>();
            CWALThumbTypes.Add(THUM.THUMSize.small, 0x0589DC44);
            CWALThumbTypes.Add(THUM.THUMSize.medium, 0x0589DC45);
            CWALThumbTypes.Add(THUM.THUMSize.large, 0x0589DC46);
            ListIResourceKey seen = new ListIResourceKey();
            foreach (THUM.THUMSize size in new THUM.THUMSize[] { THUM.THUMSize.small, THUM.THUMSize.medium, THUM.THUMSize.large, })
            {
                int i = 0;
                uint type = CWALThumbTypes[size];
                foreach (IPackage pkg in tmbPkgs)
                {
                    IList<IResourceIndexEntry> lrie = pkg.FindAll(new string[] { "ResourceType", "Instance" }, new TypedValue[] {
                            new TypedValue(typeof(uint), type),
                            new TypedValue(typeof(ulong), selectedItem.rk.Instance),
                        });
                    foreach (IResourceIndexEntry rie in lrie)
                    {
                        RIE Rie = new RIE(pkg, rie);
                        if (seen.Contains(Rie.rk)) continue;
                        Add(size + "[" + i++ + "]Thumb", Rie.rk);
                    }
                }
            }
        }

        //Fix integrity step
        void FixIntegrity()
        {
            // A list of the TGIs we are going to renumber and the resource that "owns" them
            rkToItem = new Dictionary<IResourceKey, Item>();

            // We need to process anything we found in the previous steps
            foreach (var kvp in rkLookup)
            {
                if (kvp.Value == RK.NULL) continue;
                if (rkToItem.ContainsKey(kvp.Value)) continue; // seen this TGI before
                Item item = new Item(objPkgs, kvp.Value);
                if (item.ResourceIndexEntry == null) continue; // TGI is not a packed resource
                rkToItem.Add(kvp.Value, item);
            }

            // We need to process STBLs
            IList<IResourceIndexEntry> lstblrie = objPkgs[0].FindAll(new String[] { "ResourceType" }, new TypedValue[] { new TypedValue(typeof(uint), (uint)0x220557DA), });
            foreach (IResourceIndexEntry rie in lstblrie)
                if (!rkToItem.ContainsKey(rie))
                    rkToItem.Add(rie, new Item(new RIE(objPkgs[0], rie)));

            // We may also need to process RCOL internal chunks and NameMaps but only if we're renumbering
            if (cloneFixOptions.IsRenumber)
            {
                //If there are internal chunk references not covered by the above, we also need to add them
                Dictionary<IResourceKey, Item> rcolChunks = new Dictionary<IResourceKey, Item>();
                foreach (var kvp in rkToItem)
                {
                    if (!typeof(GenericRCOLResource).IsAssignableFrom(kvp.Value.Resource.GetType())) continue;

                    foreach (GenericRCOLResource.ChunkEntry chunk in (kvp.Value.Resource as GenericRCOLResource).ChunkEntries)
                    {
                        if (chunk.TGIBlock == RK.NULL) continue;
                        if (rkToItem.ContainsKey(chunk.TGIBlock)) continue; // External reference and we've seen it
                        if (rcolChunks.ContainsKey(chunk.TGIBlock)) continue; // Internal reference and we've seen it
                        rcolChunks.Add(chunk.TGIBlock, kvp.Value);
                    }
                }
                foreach (var kvp in rcolChunks) rkToItem.Add(kvp.Key, kvp.Value);

                // Add newest namemap
                IList<IResourceIndexEntry> lnmaprie = objPkgs[0].FindAll(new String[] { "ResourceType" }, new TypedValue[] { new TypedValue(typeof(uint), (uint)0x0166038C), });
                foreach (IResourceIndexEntry rie in lnmaprie)
                    if (!rkToItem.ContainsKey(rie))
                        rkToItem.Add(rie, new Item(new RIE(objPkgs[0], rie)));
            }

            // A list to hold the new numbers
            oldToNew = new Dictionary<ulong, ulong>();

            if (cloneFixOptions.IsRenumber && selectedItem.CType == CatalogType.ModularResource)
                oldToNew.Add(selectedItem.rk.Instance, FNV64.GetHash(UniqueObject));//MDLR needs its IID as a specific hash value

            ulong PngInstance = 0;
            if (selectedItem.CType != CatalogType.ModularResource)
                PngInstance = (ulong)selectedItem.Resource["CommonBlock.PngInstance"].Value;

            if (cloneFixOptions.IsRenumber)
            {
                // Generate new numbers for everything we've decided to renumber

                // Renumber the PNGInstance we're referencing
                if (PngInstance != 0)
                    oldToNew.Add(PngInstance, CreateInstance());

                ulong langInst = (CreateInstance() << 8) >> 8;
                foreach (IResourceKey rk in rkToItem.Keys)
                {
                    if (!oldToNew.ContainsKey(rk.Instance))
                    {
                        if (rk.ResourceType == 0x220557DA)//STBL
                            oldToNew.Add(rk.Instance, rk.Instance & 0xFF00000000000000 | langInst);
                        else if (cloneFixOptions.Is32bitIIDs &&
                            (rk.ResourceType == (uint)CatalogType.CatalogObject || rk.ResourceType == 0x02DC343F))//OBJD&OBJK
                            oldToNew.Add(rk.Instance, CreateInstance32());
                        else
                            oldToNew.Add(rk.Instance, CreateInstance());
                    }
                }
            }

            Item catlgItem = selectedItem;
            if (selectedItem.CType == CatalogType.ModularResource)
                catlgItem = ItemForTGIBlock0(catlgItem);

            nameGUID = (ulong)catlgItem.Resource["CommonBlock.NameGUID"].Value;
            descGUID = (ulong)catlgItem.Resource["CommonBlock.DescGUID"].Value;

            if (cloneFixOptions.IsRenumber)
            {
                newNameGUID = FNV64.GetHash("CatalogObjects/Name:" + UniqueObject);
                newDescGUID = FNV64.GetHash("CatalogObjects/Description:" + UniqueObject);
            }
            else
            {
                newNameGUID = nameGUID;
                newDescGUID = descGUID;
            }


            resourceList.Clear();
            if (cloneFixOptions.IsRenumber)
            {
                foreach (var kvp in rkToItem)
                {
                    TGIN oldN = (AResourceKey)kvp.Key;
                    TGIN newN = (AResourceKey)kvp.Key;
                    newN.ResInstance = oldToNew[kvp.Key.Instance];
                    string s = String.Format("Old: {0} --> New: {1}", "" + oldN, "" + newN);
                    resourceList.Add(s);
                }

                resourceList.Add("Old NameGUID: 0x" + nameGUID.ToString("X16") + " --> New NameGUID: 0x" + newNameGUID.ToString("X16"));
                resourceList.Add("Old DescGUID: 0x" + descGUID.ToString("X16") + " --> New DescGUID: 0x" + newDescGUID.ToString("X16"));
                resourceList.Add("Old ObjName: \"" + catlgItem.Resource["CommonBlock.Name"] + "\" --> New Name: \"CatalogObjects/Name:" + UniqueObject + "\"");
                resourceList.Add("Old ObjDesc: \"" + catlgItem.Resource["CommonBlock.Desc"] + "\" --> New Desc: \"CatalogObjects/Description:" + UniqueObject + "\"");
            }

            resourceList.Add("Old CatlgName: \"" + English[nameGUID] + "\" --> New CatlgName: \"" + tbCatlgName.Text + "\"");
            resourceList.Add("Old CatlgDesc: \"" + English[descGUID] + "\" --> New CatlgDesc: \"" + tbCatlgDesc.Text + "\"");
            resourceList.Add("Old Price: " + catlgItem.Resource["CommonBlock.Price"] + " --> New Price: " + float.Parse(tbPrice.Text));
            resourceList.Add("Old Product Status: 0x" + ((byte)catlgItem.Resource["CommonBlock.BuildBuyProductStatusFlags"].Value).ToString("X2") + " --> New Product Status: " + tbProductStatus.Text);
            if (PngInstance != 0 && cloneFixOptions.IsRenumber)
                resourceList.Add("Old PngInstance: " + selectedItem.Resource["CommonBlock.PngInstance"] + " --> New PngInstance: 0x" + oldToNew[PngInstance].ToString("X16"));
        }
        #endregion

        private void btnReplThumb_Click(object sender, EventArgs e)
        {
            openThumbnailDialog.FilterIndex = 1;
            openThumbnailDialog.FileName = "*.PNG";
            DialogResult dr = openThumbnailDialog.ShowDialog();
            if (dr != DialogResult.OK) return;
            try
            {
                replacementForThumbs = Image.FromFile(openThumbnailDialog.FileName, true);
                pictureBox1.Image = replacementForThumbs.GetThumbnailImage(pictureBox1.Width, pictureBox1.Height, gtAbort, System.IntPtr.Zero);
            }
            catch (Exception ex)
            {
                CopyableMessageBox.IssueException(ex, "Could not read thumbnail:\n" + openThumbnailDialog.FileName, openThumbnailDialog.Title);
                replacementForThumbs = null;
            }
        }
        static bool gtAbort() { return false; }

        private void btnStart_Click(object sender, EventArgs e)
        {
            fillOverviewUpdateImage(selectedItem);
            TabEnable(true);
            DisplayOptions();
        }
    }

    static class Diagnostics
    {
        static bool enabled = false;
        public static bool Enabled { get { return enabled; } set { enabled = value; } }
        public static void Show(string value, string title = "")
        {
            string msg = value.Trim('\n');
            if (msg == "") return;
            if (title == "")
            {
                System.Diagnostics.Debug.WriteLine(msg);
                if (enabled) CopyableMessageBox.Show(msg);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(String.Format("{0}: {1}", title, msg));
                if (enabled) CopyableMessageBox.Show(msg, title);
            }
        }
    }
}
