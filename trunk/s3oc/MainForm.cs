/***************************************************************************
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
        static bool LangSearch = false;
        static bool disableCompression = false;
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

            string prefix = null;
            foreach (Statement s in s3oc_ini.Select(new Statement(subjectWin64, rdftype, typeMicrosoftOS)))
            {
                foreach (Statement t in s3oc_ini.Select(new Statement(s.Subject, predHasProgramFiles, null)))
                {
                    if (Directory.Exists(getString(t.Object)))
                        prefix = getString(t.Object);
                }
            }
            if (prefix == null)
                foreach (Statement s in s3oc_ini.Select(new Statement(subjectWin32, rdftype, typeMicrosoftOS)))
                {
                    foreach (Statement t in s3oc_ini.Select(new Statement(s.Subject, predHasProgramFiles, null)))
                    {
                        if (Directory.Exists(getString(t.Object)))
                            prefix = getString(t.Object);
                    }
                }
            if (prefix == null)
                prefix = "/";

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

        static string getHasDefaultInstallDir(string prefix, Resource value)
        {
            string s = getString(value);
            return (s == null || !Directory.Exists(prefix + s)) ? null : prefix + s;
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
            FromGame,
            FromUser,
        }
        Mode mode = Mode.None;

        View currentView;

        List<IPackage> objPkgs;
        List<IPackage> ddsPkgs;
        List<IPackage> tmbPkgs;
        List<string> objPaths;
        List<string> ddsPaths;
        List<string> tmbPaths;

        Item selectedItem;
        Image replacementForThumbs;

        Dictionary<string, IResourceKey> rkLookup = null;

        private ObjectCloner.TopPanelComponents.ObjectChooser objectChooser;
        private ObjectCloner.TopPanelComponents.ResourceList resourceList;
        private ObjectCloner.TopPanelComponents.PleaseWait pleaseWait;
        private ObjectCloner.TopPanelComponents.CloneFixOptions cloneFixOptions;
        private ObjectCloner.TopPanelComponents.Search searchPane;
        private ObjectCloner.TopPanelComponents.TGISearch tgiSearchPane;

        public MainForm()
        {
            InitializeComponent();

            this.Text = myName;
            objectChooser = new ObjectChooser();
            objectChooser.SelectedIndexChanged += new EventHandler(objectChooser_SelectedIndexChanged);
            objectChooser.ItemActivate += new EventHandler(objectChooser_ItemActivate);
            resourceList = new ResourceList();
            pleaseWait = new PleaseWait();

            MainForm_LoadFormSettings();

            LangSearch = ObjectCloner.Properties.Settings.Default.LangSearch;
            menuBarWidget1.Checked(MenuBarWidget.MB.MBS_langSearch, LangSearch);

            menuBarWidget1.Checked(MenuBarWidget.MB.MBS_advanced, ObjectCloner.Properties.Settings.Default.AdvanceCloning);

            Diagnostics.Enabled = ObjectCloner.Properties.Settings.Default.Diagnostics;
            menuBarWidget1.Checked(MenuBarWidget.MB.MBS_diagnostics, Diagnostics.Enabled);

            SetStepText();

            InitialiseTabs(CatalogType.CatalogProxyProduct);//Use the Proxy Product as it has pretty much nothing on it
            TabEnable(false);
        }

        public MainForm(params string[] args)
            : this()
        {
            CmdLine(args);

            // Settings for test mode
            if (cmdlineTest)
            {
            }
        }

        private void MainForm_LoadFormSettings()
        {
            int h = ObjectCloner.Properties.Settings.Default.PersistentHeight;
            if (h == -1) h = 4 * System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height / 5;
            this.Height = h;

            int w = ObjectCloner.Properties.Settings.Default.PersistentWidth;
            if (w == -1) w = 4 * System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width / 5;
            this.Width = w;

            Point xy = ObjectCloner.Properties.Settings.Default.PersistentLocation;
            if (xy.X == -1 && xy.Y == -1)
                this.StartPosition = FormStartPosition.CenterScreen;
            else
            {
                this.StartPosition = FormStartPosition.Manual;
                this.Location = xy;
            }

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

            ObjectCloner.Properties.Settings.Default.PersistentLocation = this.WindowState == FormWindowState.Normal ? this.Location : new Point(-1, -1);
            ObjectCloner.Properties.Settings.Default.PersistentHeight = this.WindowState == FormWindowState.Normal ? this.Height : -1;
            ObjectCloner.Properties.Settings.Default.PersistentWidth = this.WindowState == FormWindowState.Normal ? this.Width : -1;
            ObjectCloner.Properties.Settings.Default.Splitter1Width = splitContainer1.SplitterDistance * 100 / this.Width;
            ObjectCloner.Properties.Settings.Default.ShowThumbs = menuBarWidget1.IsChecked(MenuBarWidget.MB.MBV_icons);
            ObjectCloner.Properties.Settings.Default.Save();
        }

        void ClosePkg()
        {
            thumb = null;
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
                thumTypes.Add(0x0A36F07A, thumTypes[0x319E4F1D]); //Catalog Fountain Pool
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
                return item == null ? RK.NULL : item.RequestedRK;
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
                    List<IResourceIndexEntry> lrie = new List<IResourceIndexEntry>(pkg.FindAll(rie => rie.ResourceType == type && rie.Instance == instance));
                    lrie.Sort(byGroup);
                    foreach (IResourceIndexEntry rie in lrie)
                        if (!new List<uint>(thumTypes[0x515CA4CD]).Contains(type) || (rie.ResourceGroup & 0x00FFFFFF) > 0)
                            return new Item(new RIE(pkg, rie));
                }
                //return new Item(pkgs, new TGI(type, 0, instance));
                return new Item(pkgs, RK.NULL);
            }
            static int byGroup(IResourceIndexEntry x, IResourceIndexEntry y) { return (x.ResourceGroup & 0x07FFFFFF).CompareTo(y.ResourceGroup & 0x07FFFFFF); }
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
                return Thumb[item.RequestedRK.ResourceType, png != 0 ? png : item.RequestedRK.Instance, size, png != 0];
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
                return Thumb.getRK(item.RequestedRK.ResourceType, png != 0 ? png : item.RequestedRK.Instance, size, png != 0);
            }
        }
        static IResourceKey getNewRK(THUM.THUMSize size, Item item)
        {
            if (item.CType == CatalogType.ModularResource)
                return RK.NULL;
            else
            {
                ulong png = (item.Resource != null) ? (ulong)item.Resource["CommonBlock.PngInstance"].Value : 0;
                return THUM.getNewRK(item.RequestedRK.ResourceType, png != 0 ? png : item.RequestedRK.Instance, size, png != 0);
            }
        }
        IResourceKey makeImage(THUM.THUMSize size, Item item)
        {
            if (item.CType == CatalogType.ModularResource)
                return RK.NULL;
            else
            {
                IResourceKey rk = getImageRK(size, item);
                if (rk.Equals(RK.NULL))
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


        delegate void Callback(byte lang);
        static Item findStblFor(IList<IPackage> objPkgs, ulong guid, Callback callBack = null)
        {
            for (byte i = 0; i < (LangSearch ? 0x17 : 0x01); i++) { if (callBack != null) callBack(i); Item res = findStblFor(objPkgs, guid, i); if (res != null) return res; }
            return null;
        }

        static Item findStblFor(IList<IPackage> objPkgs, ulong guid, byte lang)
        {
            foreach (var pkg in objPkgs)
            {
                foreach (var rie in pkg.FindAll(x => x.ResourceType == 0x220557DA && x.Instance >> 56 == lang))
                {
                    Item item = new Item(new RIE(pkg, rie));
                    if (item.SpecificRK == null) continue;
                    if (item.Resource == null) continue;
                    IDictionary<ulong, string> id = item.Resource as IDictionary<ulong, string>;
                    if (id == null) continue;
                    if (id.ContainsKey(guid)) return item;
                }
            }
            return null;
        }

        static string StblLookup(IList<IPackage> objPkgs, ulong guid, int lang = -1, Callback callBack = null)
        {
            Item stbl;
            if (lang < 0 || lang >= 0x17) stbl = findStblFor(objPkgs, guid, callBack);
            else stbl = findStblFor(objPkgs, guid, (byte)lang);
            return stbl == null ? null : (stbl.Resource as IDictionary<ulong, string>)[guid];
        }

        public class NameMap
        {
            Item latest;
            List<IDictionary<ulong, string>> namemaps;
            public NameMap(IList<IPackage> nameMapPkgs)
            {
                namemaps = new List<IDictionary<ulong, string>>();
                foreach (IPackage pkg in nameMapPkgs)
                {
                    IList<IResourceIndexEntry> lrie = pkg.FindAll(rie => rie.ResourceType == 0x0166038C);
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
            public IResourceKey rk { get { return latest == null ? RK.NULL : latest.RequestedRK; } }
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
            IResourceKey rk = ((TGIBlockList)item.Resource["TGIBlocks"].Value)[0];
            return new Item(objPkgs, rk);
        }


        #region LeftPanelComponents

        private void DisplayTGISearch()
        {
            /*
            tgiSearchPane.SelectedIndexChanged -= new EventHandler<Search.SelectedIndexChangedEventArgs>(tgiSearchPane_SelectedIndexChanged);
            tgiSearchPane.SelectedIndexChanged += new EventHandler<Search.SelectedIndexChangedEventArgs>(tgiSearchPane_SelectedIndexChanged);
            tgiSearchPane.ItemActivate -= new EventHandler<Search.ItemActivateEventArgs>(tgiSearchPane_ItemActivate);
            tgiSearchPane.ItemActivate += new EventHandler<Search.ItemActivateEventArgs>(tgiSearchPane_ItemActivate);
            /**/

            this.AcceptButton = btnStart;
            this.CancelButton = null;

            menuBarWidget1.Enable(MenuBarWidget.MB.MBF_open, true);
            menuBarWidget1.Enable(MenuBarWidget.MD.MBC, true);
            menuBarWidget1.Enable(MenuBarWidget.MD.MBT, true);
            menuBarWidget1.Enable(MenuBarWidget.MD.MBS, true);

            lbTGISearch.Visible = true;
            lbSearch.Visible = false;
            lbUseMenu.Visible = false;
            lbSelectOptions.Visible = false;
            btnStart.Visible = true;
            btnStart.Enabled = false;

            StopWait();
            splitContainer1.Panel1.Controls.Clear();
            splitContainer1.Panel1.Controls.Add(tgiSearchPane);
            tgiSearchPane.Dock = DockStyle.Fill;
            tgiSearchPane.Focus();
        }

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

            lbTGISearch.Visible = false;
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

            lbTGISearch.Visible = false;
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
            lbTGISearch.Visible = false;
            lbSearch.Visible = false;
            lbUseMenu.Visible = false;
            lbSelectOptions.Visible = true;
            btnStart.Visible = false;

            if (searchPane != null)
                mode = Mode.FromGame;

            cloneFixOptions = new CloneFixOptions(this, mode == Mode.FromGame, hasOBJDs());
            cloneFixOptions.CancelClicked += new EventHandler(cloneFixOptions_CancelClicked);
            cloneFixOptions.StartClicked += new EventHandler(cloneFixOptions_StartClicked);

            if (mode == Mode.FromGame)
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

            lbTGISearch.Visible = false;
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
        bool isCreateNewPackage = false;
        bool isPadSTBLs = false;
        void cloneFixOptions_StartClicked(object sender, EventArgs e)
        {
            this.AcceptButton = null;
            this.CancelButton = null;
            disableCompression = !cloneFixOptions.IsCompress;
            isCreateNewPackage = cloneFixOptions.IsClone;
            isPadSTBLs = cloneFixOptions.IsPadSTBLs;

            if (isCreateNewPackage) CloneStart();
            else FixStart();
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

        void CloneStart()
        {
            uniqueObject = null;
            if (UniqueObject == null)
            {
                cloneFixOptions_CancelClicked(null, null);
                return;
            }

            saveFileDialog1.FileName = UniqueObject;
            if (ObjectCloner.Properties.Settings.Default.LastSaveFolder != null)
                saveFileDialog1.InitialDirectory = ObjectCloner.Properties.Settings.Default.LastSaveFolder;
            DialogResult dr = saveFileDialog1.ShowDialog();
            if (dr != DialogResult.OK)
            {
                cloneFixOptions_CancelClicked(null, null);
                return;
            }
            ObjectCloner.Properties.Settings.Default.LastSaveFolder = Path.GetDirectoryName(saveFileDialog1.FileName);

            StartSaving();
        }

        void FixStart()
        {
            uniqueObject = null;
            if (cloneFixOptions.IsRenumber && UniqueObject == null)
            {
                cloneFixOptions_CancelClicked(null, null);
                return;
            }

            StartFixing();
        }

        void RunRKLookup()
        {
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
        }
        #endregion

        #region Tabs
        CatalogType tabType = 0;

        static Dictionary<string, string> fieldToLabelMap;//(Type:)fieldname -> Label
        static Dictionary<string, string> labelToFieldMap;//(Type:)Label -> fieldname
        static Dictionary<string, string> otherFieldMap;
        static List<flagField> flagFields;
        static Dictionary<List<Type>, int> typeToLen = null;
        void InitialiseMaps()
        {
            if (fieldToLabelMap == null)
            {
                fieldToLabelMap = new Dictionary<string, string>();
                fieldToLabelMap.Add("Common:FireType", "Fire Type");
                fieldToLabelMap.Add("Common:IsStealable", "Stealable");
                fieldToLabelMap.Add("Common:IsReposessable", "Reposessable");
                fieldToLabelMap.Add("Common:IsPlaceableOnRoof", "Placeable On Roof");
                fieldToLabelMap.Add("Common:IsVisibleInWorldBuilder", "Visible In World Builder");
                fieldToLabelMap.Add("ObjectCatalogResource:MoodletGiven", "Moodlet Given");
                fieldToLabelMap.Add("ObjectCatalogResource:MoodletScore", "Moodlet Score");
                fieldToLabelMap.Add("ObjectCatalogResource:TopicRatings", "Topic/Ratings");
                fieldToLabelMap.Add("TerrainPaintBrushCatalogResource:Category", "Category");
            }
            if (labelToFieldMap == null)
            {
                labelToFieldMap = new Dictionary<string, string>();
                labelToFieldMap.Add("Common:Fire Type", "FireType");
                labelToFieldMap.Add("Common:Stealable", "IsStealable");
                labelToFieldMap.Add("Common:Reposessable", "IsReposessable");
                labelToFieldMap.Add("Common:Placeable On Roof", "IsPlaceableOnRoof");
                labelToFieldMap.Add("Common:Visible In World Builder", "IsVisibleInWorldBuilder");
                labelToFieldMap.Add("ObjectCatalogResource:Moodlet Given", "MoodletGiven");
                labelToFieldMap.Add("ObjectCatalogResource:Moodlet Score", "MoodletScore");
                labelToFieldMap.Add("ObjectCatalogResource:Topic/Ratings", "TopicRatings");
                labelToFieldMap.Add("TerrainPaintBrushCatalogResource:Category", "Category");
            }
            if (otherFieldMap == null)
            {
                otherFieldMap = new Dictionary<string, string>();
            }
            if (flagFields == null)
            {
                flagFields = new List<flagField>(new flagField[] {
                    new flagField(tlpUnknown8, "ObjectTypeFlags", 32, 0),
                    new flagField(tlpUnknown9, "WallPlacementFlags", 32, 0),
                    new flagField(tlpUnknown10, "MovementFlags", 32, 0),
                    new flagField(tlpRoomSort, "RoomCategoryFlags", 32, 0),
                    new flagField(tlpRoomSubLow, "RoomSubCategoryFlags", 32, 0),
                    new flagField(tlpRoomSubHigh, "RoomSubCategoryFlags", 64, 32),
                    new flagField(tlpFuncSort, "FunctionCategoryFlags", 32, 0),
                    new flagField(tlpFuncSubLow, "FunctionSubCategoryFlags", 32, 0),
                    new flagField(tlpFuncSubHigh, "FunctionSubCategoryFlags", 64, 32),
                    new flagField(tlpBuildSort, "BuildCategoryFlags", 32, 0),
                });
            }
            if (typeToLen == null)
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
        }

        struct flagField
        {
            public TableLayoutPanel tlp;
            public string field;
            public int length;
            public int offset;
            public flagField(TableLayoutPanel tlp, string field, int length, int offset) { this.tlp = tlp; this.field = field; this.length = length; this.offset = offset; }
        }
        struct ResField
        {
            public IContentFields res;
            public string field;
            public ResField(IContentFields res, string field) { this.res = res; this.field = field; }
        }
        ResField mapResField(Dictionary<string, string> map, ResField resField)
        {
            ResField result;
            if (map.ContainsKey("Common:" + resField.field))
                result.res = resField.res["CommonBlock"].Value as AApiVersionedFields;
            else
                result.res = resField.res;
            string lookup = result.res.GetType().Name + ":" + resField.field;
            result.field = map.ContainsKey(lookup) ? map[lookup]
                : map.ContainsKey(resField.field) ? map[resField.field]
                : resField.field;
            return result;
        }
        ResField fieldToLabel(ResField field) { return mapResField(fieldToLabelMap, field); }
        ResField labelToField(ResField label) { return mapResField(labelToFieldMap, label); }

        string GetTGIBlockListContentField(Type t, string field)
        {
            System.Reflection.PropertyInfo pi = t.GetProperty(field);
            foreach (Attribute attr in pi.GetCustomAttributes(typeof(TGIBlockListContentFieldAttribute), true))
                return (attr as TGIBlockListContentFieldAttribute).TGIBlockListContentField;
            return null;
        }
        bool HasTGIBlockListContentFieldAttribute(Type t, string field) { return GetTGIBlockListContentField(t, field) != null; }

        int getFieldLength(Type t)
        {
            InitialiseMaps();

            foreach (List<Type> tt in typeToLen.Keys)
                if (tt.Contains(typeof(Enum).IsAssignableFrom(t) ? Enum.GetUnderlyingType(t) : t))
                    return typeToLen[tt];
            return 30;
        }

        void IterateTLP(TableLayoutPanel tlp, Action<Label, Control> action)
        {
            for (int i = 1; i < tlp.RowCount - 1; i++)
                action((Label)tlp.GetControlFromPosition(0, i), tlp.GetControlFromPosition(1, i));
        }

        void InitialiseTabs(CatalogType resourceType)
        {
            if (tabType == resourceType) return;

            string appWas = this.Text;
            this.Text = myName + " [busy]";
            Application.UseWaitCursor = true;
            Application.DoEvents();
            try
            {
                tabType = resourceType;

                if (resourceType == CatalogType.ModularResource) resourceType = CatalogType.CatalogObject;//Modular Resources - display OBJD0

                IResource res = s3pi.WrapperDealer.WrapperDealer.CreateNewResource(0, "0x" + ((uint)resourceType).ToString("X8"));
                this.tabControl1.Controls.Remove(this.tpDetail);
                if (tabType == CatalogType.CatalogObject || tabType == CatalogType.CatalogTerrainPaintBrush)
                {
                    this.tabControl1.Controls.Add(this.tpDetail);
                    InitialiseDetailsTab(res);
                }
                this.tabControl1.Controls.Remove(this.tpFlagsRoom);
                this.tabControl1.Controls.Remove(this.tpFlagsFunc);
                this.tabControl1.Controls.Remove(this.tpFlagsBuild);
                this.tabControl1.Controls.Remove(this.tpFlagsMisc);
                if (tabType == CatalogType.CatalogObject)
                {
                    this.tabControl1.Controls.Add(this.tpFlagsRoom);
                    this.tabControl1.Controls.Add(this.tpFlagsFunc);
                    this.tabControl1.Controls.Add(this.tpFlagsBuild);
                    this.tabControl1.Controls.Add(this.tpFlagsMisc);
                    InitialiseFlagTabs(res);
                    if (tlpOther.Visible)
                        InitialiseOtherTab(res);
                }
            }
            finally { this.Text = appWas; Application.UseWaitCursor = false; Application.DoEvents(); }
        }
        void InitialiseDetailsTab(IResource catlg) { InitialiseMaps(); InitialiseTabTLP(catlg, tlpObjectDetail, labelToFieldMap.Values); }
        void InitialiseOtherTab(IResource objd) { InitialiseMaps(); InitialiseTabTLP(objd, tlpOther, otherFieldMap.Keys); }
        void InitialiseFlagTabs(IResource objd)
        {
            InitialiseMaps();
            flagFields.ForEach(ff => InitialiseTLP(ff.tlp));

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

        void InitialiseTLP(TableLayoutPanel tlp)
        {
            while (tlp.RowCount > 2)
            {
                for (int i = 0; i < tlp.ColumnCount; i++)
                {
                    Control c = tlp.GetControlFromPosition(i, tlp.RowCount - 2);
                    tlp.Controls.Remove(c);
                }
                tlp.RowCount--;
            }
        }
        void InitialiseTabTLP(IResource res, TableLayoutPanel tlp, IEnumerable<String> fields)
        {
            InitialiseTLP(tlp);

            foreach (string field in fields)
            {
                ResField resField = new ResField(res, field);
                ResField resLabel = fieldToLabel(resField);
                if (AApiVersionedFields.GetContentFields(0, resLabel.res.GetType()).Contains(field))
                    CreateField(tlp, resField, true);
            }
        }
        void CreateField(TableLayoutPanel target, ResField resField) { CreateField(target, resField, false); }
        void CreateField(TableLayoutPanel target, ResField resField, bool validate)
        {
            ResField resLabel = fieldToLabel(resField);
            Type type = AApiVersionedFields.GetContentFieldTypes(0, resLabel.res.GetType())[resField.field];

            target.RowCount++;
            target.RowStyles.Insert(target.RowCount - 2, new RowStyle(SizeType.AutoSize));

            Label lb = new Label();
            lb.Anchor = AnchorStyles.Right;
            lb.AutoSize = true;
            lb.Name = "lb" + resField.field;
            lb.TabIndex = target.RowCount * 2;
            lb.Text = resLabel.field;
            target.Controls.Add(lb, 0, target.RowCount - 2);

            if (HasTGIBlockListContentFieldAttribute(resLabel.res.GetType(), resField.field))
            {
                TGIBlockCombo tbc = new TGIBlockCombo();
                tbc.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                tbc.Name = "tbc" + resField.field;
                tbc.Enabled = false;
                tbc.Margin = new Padding(3, 0, 0, 0);
                tbc.ShowEdit = false;
                tbc.TabIndex = target.RowCount * 2 + 1;
                tbc.Tag = true;
                tbc.Width = (int)target.ColumnStyles[1].Width;
                target.Controls.Add(tbc, 1, target.RowCount - 2);
            }
            else if (resField.field.Equals("TopicRatings"))
            {
                CustomControls.TopicRatings tr = new CustomControls.TopicRatings();
                tr.Anchor = AnchorStyles.Left;
                tr.Name = "tr" + resField.field;
                tr.ReadOnly = true;
                tr.TabIndex = target.RowCount * 2 + 1;
                if (validate) tr.Tag = true;
                target.Controls.Add(tr, 1, target.RowCount - 2);
            }
            else if (typeof(Enum).IsAssignableFrom(type))
            {
                CustomControls.EnumTextBox etb = new CustomControls.EnumTextBox();
                etb.EnumType = type;
                etb.Anchor = AnchorStyles.Left;
                etb.Name = "etb" + resField.field;
                etb.ReadOnly = true;
                etb.TabIndex = target.RowCount * 2 + 1;
                if (validate) etb.Tag = true;
                target.Controls.Add(etb, 1, target.RowCount - 2);
            }
            else if (typeof(Boolean).Equals(type))
            {
                CheckBox ckb = new CheckBox();
                ckb.Anchor = AnchorStyles.Left;
                ckb.AutoCheck = false;
                ckb.Name = "ckb" + resField.field;
                ckb.TabIndex = target.RowCount * 2 + 1;
                if (validate) ckb.Tag = true;
                target.Controls.Add(ckb, 1, target.RowCount - 2);
            }
            else
            {
                Label x = new Label();
                x.AutoSize = true;
                x.Text = "".PadLeft(getFieldLength(type), 'X');

                TextBox tb = new TextBox();
                tb.Anchor = AnchorStyles.Left;
                tb.Name = "tb" + resField.field;
                tb.ReadOnly = true;
                tb.Size = new Size(x.PreferredWidth + 6, x.PreferredHeight + 6);
                tb.TabIndex = target.RowCount * 2 + 1;
                if (validate)
                {
                    tb.Validating += new CancelEventHandler(tb_Validating);
                    tb.Tag = true;
                }
                target.Controls.Add(tb, 1, target.RowCount - 2);
            }
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
                if (tabControl1.Contains(tpDetail))
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
            tbNameGUID.Text = "";
            tbCatlgName.Text = "";
            tbObjDesc.Text = "";
            tbDescGUID.Text = "";
            tbCatlgDesc.Text = "";
            ckbCopyToAll.Checked = false;
            tbPrice.Text = "";
            tbProductStatus.Text = "";
            tbPackage.Text = "";
        }
        void clearDetails() { IterateTLP(tlpObjectDetail, clearTLP); }
        void clearOther() { IterateTLP(tlpOther, clearTLP); }
        void clearFlags()
        {
            foreach (flagField ff in flagFields)
                foreach (Control c in ff.tlp.Controls)
                    if (c is CheckBox) ((CheckBox)c).Checked = false;
        }
        void clearTLP(Label l, Control c)
        {
            if (c is TGIBlockCombo) { TGIBlockCombo tbc = c as TGIBlockCombo; tbc.SelectedIndex = -1; }
            else if (c is CustomControls.TopicRatings) { CustomControls.TopicRatings tr = c as CustomControls.TopicRatings; tr.Clear(); tr.ReadOnly = true; }
            else if (c is CustomControls.EnumTextBox) { CustomControls.EnumTextBox etb = c as CustomControls.EnumTextBox; etb.Clear(); etb.ReadOnly = true; }
            else if (c is CheckBox) { CheckBox ckb = c as CheckBox; ckb.AutoCheck = ckb.Checked = false; }
            else if (c is TextBox) { TextBox tb = c as TextBox; tb.Text = ""; tb.ReadOnly = true; }
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
                if (tabControl1.Contains(tpDetail))
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

        string itemName = null;
        string itemDesc = null;
        void fillOverview(Item item)
        {
            AApiVersionedFields common = item.Resource["CommonBlock"].Value as AApiVersionedFields;
            pictureBox1.Image = getImage(THUM.THUMSize.large, item);
            lbThumbTGI.Text = (AResourceKey)getImageRK(THUM.THUMSize.large, item);
            tbResourceName.Text = NMap[item.RequestedRK.Instance];
            tbObjName.Text = common["Name"].Value + "";
            tbNameGUID.Text = common["NameGUID"] + "";

            try
            {
                updateProgress(true, "Looking up NameGUID...", true, 0x17, true, 0);
                Application.DoEvents();
                itemName = StblLookup(objPkgs, (ulong)common["NameGUID"].Value, -1, x => { updateProgress(false, "", false, 0, true, x); Application.DoEvents(); });
                tbCatlgName.Text = itemName == null ? "" : itemName;
            }
            finally { updateProgress(true, "", true, -1, false, 0); Application.DoEvents(); }

            tbObjDesc.Text = common["Desc"].Value + "";
            tbDescGUID.Text = common["DescGUID"] + "";

            try
            {
                updateProgress(true, "Looking up DescGUID...", true, 0x17, true, 0);
                Application.DoEvents();
                itemDesc = StblLookup(objPkgs, (ulong)common["DescGUID"].Value, -1, x => { updateProgress(false, "", false, 0, true, x); Application.DoEvents(); });
                tbCatlgDesc.Text = itemDesc == null ? "" : itemDesc;
            }
            finally { updateProgress(true, "", true, -1, false, 0); Application.DoEvents(); }

            tbPrice.Text = common["Price"].Value + "";
            tbProductStatus.Text = "0x" + ((byte)common["BuildBuyProductStatusFlags"].Value).ToString("X2");
            tbPackage.Text = objPaths[objPkgs.IndexOf(item.Package)];
        }
        void fillDetails(Item item) { IterateTLP(tlpObjectDetail, (l, c) => fillControl(item, l, c)); }
        void fillOther(Item item) { IterateTLP(tlpOther, (l, c) => fillControl(item, l, c)); }
        void fillFlags(Item item)
        {
            foreach (flagField ff in flagFields)
            {
                ulong field = getFlags(item.Resource as AResource, ff.field);
                for (int i = 1; i < ff.tlp.RowCount - 1; i++)
                {
                    ulong value = (ulong)Math.Pow(2, ff.offset + i - 1);
                    CheckBox cb = (CheckBox)ff.tlp.GetControlFromPosition(0, i);
                    cb.Checked = (field & value) != 0;
                }
            }
        }

        void fillControl(Item item, Label lb, Control c)
        {
            ResField resField = labelToField(new ResField(item.Resource, lb.Text));

            if (!resField.res.ContentFields.Contains(resField.field)) return;//Leave field empty and readonly if not present

            TypedValue tv = resField.res[resField.field];

            if (c is TGIBlockCombo)
            {
                TGIBlockList tgiBlocks = item.Resource["TGIBlocks"].Value as TGIBlockList;
                TGIBlockCombo tbc = c as TGIBlockCombo;
                tbc.TGIBlocks = tgiBlocks;
                int index = (int)(uint)tv.Value;
                tbc.SelectedIndex = index >= 0 && index < tgiBlocks.Count ? index : -1;
            }
            else if (c is CustomControls.TopicRatings)
            {
                CustomControls.TopicRatings tr = c as CustomControls.TopicRatings;
                tr.GetType().GetProperty("Value").SetValue(tr, tv.Value, new object[] { });
                tr.ReadOnly = false;
            }
            else if (c is CustomControls.EnumTextBox)
            {
                CustomControls.EnumTextBox etb = c as CustomControls.EnumTextBox;
                etb.EnumType = tv.Type;
                etb.Value = Convert.ToUInt64(tv.Value);
                etb.ReadOnly = false;
            }
            else if (c is CheckBox)
            {
                CheckBox ckb = c as CheckBox;
                ckb.Checked = (Boolean)tv.Value;
                ckb.AutoCheck = true;
            }
            else if (c is TextBox)
            {
                TextBox tb = c as TextBox;
                tb.Text = tv;
                tb.ReadOnly = c.Tag == null;
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
                if (tabControl1.Contains(tpDetail))
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
            tbCatlgName.ReadOnly = !enabled || itemName == null;
            tbCatlgDesc.ReadOnly = !enabled || itemDesc == null;
            ckbCopyToAll.Enabled = enabled;
            tbPrice.ReadOnly = !enabled;
            tbProductStatus.ReadOnly = !enabled;
            tbCatlgName.BackColor = tbPrice.BackColor;
            tbCatlgDesc.BackColor = tbPrice.BackColor;
        }
        void tabEnableDetails(bool enabled) { IterateTLP(tlpObjectDetail, (l, c) => { if (c.Tag != null) c.Enabled = enabled; }); }
        void tabEnableOther(bool enabled) { IterateTLP(tlpOther, (l, c) => { if (c.Tag != null) c.Enabled = enabled; }); }
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

        void tb_Validating(object sender, CancelEventArgs e)
        {
            TextBox tb = sender as TextBox;
            Type t = tb.Tag as Type;
            if (t == null) return;

            try { object val = tb_Value(tb, t); }
            catch { e.Cancel = true; }

            if (e.Cancel) tb.SelectAll();
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

        void UpdateItem(Item item, Label lb, Control c)
        {
            if (c.Tag == null) return;//skip if read only
            ResField resField = labelToField(new ResField(item.Resource, lb.Text));

            if (!resField.res.ContentFields.Contains(resField.field)) return;//skip if not present

            TypedValue tvOld = resField.res[resField.field];

            if (c is TGIBlockCombo)
            {
                resField.res[resField.field] = new TypedValue(tvOld.Type, Convert.ChangeType((c as TGIBlockCombo).SelectedIndex, tvOld.Type), "X");
            }
            else if (c is CustomControls.TopicRatings)
            {
                resField.res[resField.field] = new TypedValue(tvOld.Type, tr_Value(c as CustomControls.TopicRatings), "X");
            }
            else if (c is CustomControls.EnumTextBox)
            {
                resField.res[resField.field] = new TypedValue(tvOld.Type, etb_Value(c as CustomControls.EnumTextBox), "X");
            }
            else if (c is CheckBox)
            {
                resField.res[resField.field] = new TypedValue(tvOld.Type, (c as CheckBox).Checked, "X");
            }
            else if (c is TextBox)
            {
                resField.res[resField.field] = new TypedValue(tvOld.Type, tb_Value(c as TextBox, tvOld.Type), "X");
            }
        }
        object tb_Value(TextBox tb, Type t)
        {
            if (t == typeof(Single) || t == typeof(Boolean) || !tb.Text.StartsWith("0x"))
                return Convert.ChangeType(tb.Text, t);
            else
                return Convert.ChangeType(UInt64.Parse(tb.Text.Substring(2), System.Globalization.NumberStyles.HexNumber), t);
        }
        object etb_Value(CustomControls.EnumTextBox etb) { return Enum.ToObject(etb.EnumType, etb.Value); }
        object tr_Value(CustomControls.TopicRatings tr) { return tr.Value; }
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
        Dictionary<UInt64, Item> CTPTBrushIndexToPair;
        void StartLoading(CatalogType resourceType, bool IsFixPass)
        {
            if (haveLoaded) return;
            if (loading) { AbortLoading(false); }
            if (fetching) { AbortFetching(false); }

            isFix = IsFixPass;

            waitingToDisplayObjects = true;
            objectChooser.Items.Clear();
            CTPTBrushIndexToPair = new Dictionary<UInt64, Item>();

            this.LoadingComplete -= new EventHandler<BoolEventArgs>(MainForm_LoadingComplete);
            this.LoadingComplete += new EventHandler<BoolEventArgs>(MainForm_LoadingComplete);

            FillListView flv = new FillListView(this, objPkgs, ddsPkgs, tmbPkgs, resourceType
                , createListViewItem, updateProgress, stopLoading, OnLoadingComplete);

            DoWait("Please wait, loading object catalog...");

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
                        if (mode == Mode.FromGame && menuBarWidget1.IsChecked(MenuBarWidget.MB.MBV_icons))
                        {
                            waitingForImages = true;
                            StartFetching();
                        }

                        if (objectChooser.Items.Count > 0) objectChooser.SelectedIndex = 0;
                        DisplayObjectChooser();
                    }
                    else
                        FixStart();
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
                    //int brushIndex = ("" + item.Resource["BrushTexture"]).GetHashCode();
                    UInt64 brushIndex = (UInt64)item.Resource["CommonBlock.NameGUID"].Value;
                    if (!CTPTBrushIndexToPair.ContainsKey(brushIndex))//Try to leave one behind...
                    {
                        CTPTBrushIndexToPair.Add(brushIndex, item);
                        return;
                    }
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
            if (s3pi.Extensions.ExtList.Ext.TryGetValue("0x" + item.RequestedRK.ResourceType.ToString("X8"), out exts)) tag = exts[0];
            else tag = "UNKN";
            lvi.SubItems.AddRange(new string[] { tag, item.RGVsn, "" + (AResourceKey)item.RequestedRK, });
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
            this.Enabled = false;
            DoWait("Please wait, creating your new package...");
            waitingForSavePackage = true;
            this.SavingComplete += new EventHandler<BoolEventArgs>(MainForm_SavingComplete);

            RunRKLookup();

            SaveList sl = new SaveList(this,
                searchPane == null ? objectChooser.SelectedItems[0].Tag as Item
                : searchPane.SelectedItem.Tag as Item, rkLookup, objPkgs, ddsPkgs, tmbPkgs,
                saveFileDialog1.FileName, isPadSTBLs, mode == Mode.FromGame, cloneFixOptions.IsExcludeCommon ? lS3ocResourceList : null,
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
                    isCreateNewPackage = false;
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
            bool zeroSTBLIID;
            List<Dictionary<String, TypedValue>> commonResources;
            updateProgressCallback updateProgressCB;
            stopSavingCallback stopSavingCB;
            savingCompleteCallback savingCompleteCB;
            public SaveList(MainForm form, Item catlgItem, Dictionary<string, IResourceKey> rkList, List<IPackage> objPkgs, List<IPackage> ddsPkgs, List<IPackage> tmbPkgs,
                string outputPackage, bool padSTBLs, bool zeroSTBLIID, List<Dictionary<String, TypedValue>> commonResources,
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
                this.zeroSTBLIID = zeroSTBLIID;
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
                Item newnmap = NewResource(target, fb0nm.rk == RK.NULL ? new TGIBlock(0, null, 0x0166038C, 0, selectedItem.RequestedRK.Instance) : fb0nm.rk);
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
                            List<IPackage> lpkg = (selectedItem.RequestedRK.ResourceType != 0x04ED4BB2 && kvp.Value.ResourceType == 0x00B2D882) ? ddsPkgs : kvp.Key.EndsWith("Thumb") ? tmbPkgs : objPkgs;
                            NameMap nm = kvp.Value.ResourceType == 0x00B2D882 ? fb2nm : fb0nm;

                            Item item = new Item(new RIE(lpkg, kvp.Value), true); // use default wrapper
                            if (item.SpecificRK != null)
                            {
                                if (!stopSaving) target.AddResource(item.SpecificRK, item.Resource.Stream, true);
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
                        TGIBlock tgib = ((TGIBlockList)catlgItem.Resource["TGIBlocks"].Value)[0];
                        catlgItem = new Item(objPkgs, tgib);
                    }

                    ulong nameGUID = (ulong)catlgItem.Resource["CommonBlock.NameGUID"].Value;
                    ulong descGUID = (ulong)catlgItem.Resource["CommonBlock.DescGUID"].Value;

                    Item stbl = findStblFor(nameGUID);
                    if (stbl == null)
                        stbl = findStblFor(descGUID);

                    if (stbl == null)
                        updateProgress(true, "No string tables found!", true, 0, false, 0);
                    else
                    {
                        ulong stblInstance = stbl.RequestedRK.Instance & (ulong)(zeroSTBLIID ? 0x00FFFFFFFFFF0000 : 0x00FFFFFFFFFFFFFF);
                        i = 0;
                        freq = 1;// Math.Max(1, lrie.Count / 10);
                        updateProgress(true, "Creating string tables extracts... 0%", true, 0x17, true, i);

                        while (i < 0x17)
                        {
                            if (stopSaving) return;
                            try
                            {
                                Item oldName = findStblFor(nameGUID, (byte)i);
                                Item oldDesc = findStblFor(descGUID, (byte)i);
                                if (oldName == null && oldDesc == null && !padSTBLs) continue;

                                Item newstbl = NewResource(target, new TGIBlock(0, null, 0x220557DA, stbl.RequestedRK.ResourceGroup, ((ulong)i << 56) | stblInstance));
                                IDictionary<ulong, string> outstbl = newstbl.Resource as IDictionary<ulong, string>;
                                if (oldName == null) oldName = findStblFor(nameGUID);
                                if (!outstbl.ContainsKey(nameGUID)) outstbl.Add(nameGUID, oldName == null ? "" : (oldName.Resource as IDictionary<ulong, string>)[nameGUID]);
                                if (oldDesc == null) oldDesc = findStblFor(descGUID);
                                if (!outstbl.ContainsKey(descGUID)) outstbl.Add(descGUID, oldDesc == null ? "" : (oldDesc.Resource as IDictionary<ulong, string>)[descGUID]);
                                if (!stopSaving) newstbl.Commit();

                                if (!stopSaving) newnamemap.Add(newstbl.RequestedRK.Instance, String.Format(language_fmt, languages[i], i, stblInstance));
                            }
                            finally
                            {
                                if (++i % freq == 0)
                                    updateProgress(true, "Creating string tables extracts... " + i * 100 / 0x17 + "%", true, 0x17, true, i);
                            }
                        }
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

            private Item findStblFor(ulong guid)
            {
                byte limit = (byte)(ObjectCloner.Properties.Settings.Default.LangSearch ? 0x17 : 0x01);
                try
                {
                    updateProgress(false, "", true, limit, true, 0);
                    for (byte i = 0; i < limit; i++) { updateProgress(false, "", false, -1, true, i); Item res = findStblFor(guid, i); if (res != null) return res; }
                    return null;
                }
                finally { updateProgress(false, "", false, 0, true, limit); }
            }

            private Item findStblFor(ulong guid, byte lang)
            {
                foreach (var pkg in objPkgs)
                {
                    foreach (var rie in pkg.FindAll(x => x.ResourceType == 0x220557DA && x.Instance >> 56 == lang))
                    {
                        Item item = new Item(new RIE(pkg, rie));
                        if (item.SpecificRK == null) continue;
                        if (item.Resource == null) continue;
                        IDictionary<ulong, string> id = item.Resource as IDictionary<ulong, string>;
                        if (id == null) continue;
                        if (id.ContainsKey(guid)) return item;
                    }
                }
                return null;
            }

            Item NewResource(IPackage pkg, IResourceKey rk)
            {
                RIE rie = new RIE(pkg, pkg.AddResource(rk, null, true));
                if (rie.SpecificRK == null) return null;
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

                TGIBlock tgib = new TGIBlock(0, null, rk);
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

        Dictionary<IResourceKey, Item> rkToItem;
        Dictionary<ulong, ulong> oldToNew;
        ulong nameGUID, newNameGUID;
        ulong descGUID, newDescGUID;
        void GenerateNewIIDs()
        {
            // A list of the TGIs we are going to renumber and the resource that "owns" them
            rkToItem = new Dictionary<IResourceKey, Item>();

            // We need to process anything we found in the previous steps
            foreach (var kvp in rkLookup)
            {
                if (kvp.Value == RK.NULL) continue;
                if (rkToItem.ContainsKey(kvp.Value)) continue; // seen this TGI before
                Item item = new Item(objPkgs, kvp.Value);
                if (item.SpecificRK == null) continue; // TGI is not a packed resource
                rkToItem.Add(kvp.Value, item);
            }

            // We need to process STBLs
            IList<IResourceIndexEntry> lstblrie = objPkgs[0].FindAll(rie => rie.ResourceType == 0x220557DA);
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
                IList<IResourceIndexEntry> lnmaprie = objPkgs[0].FindAll(rie => rie.ResourceType == 0x0166038C);
                foreach (IResourceIndexEntry rie in lnmaprie)
                    if (!rkToItem.ContainsKey(rie))
                        rkToItem.Add(rie, new Item(new RIE(objPkgs[0], rie)));
            }

            // A list to hold the new numbers
            oldToNew = new Dictionary<ulong, ulong>();

            if (cloneFixOptions.IsRenumber && selectedItem.CType == CatalogType.ModularResource)
                oldToNew.Add(selectedItem.SpecificRK.Instance, FNV64.GetHash(UniqueObject));//MDLR needs its IID as a specific hash value

            ulong PngInstance = 0;
            if (selectedItem.CType != CatalogType.ModularResource)
                PngInstance = (ulong)selectedItem.Resource["CommonBlock.PngInstance"].Value;

            if (cloneFixOptions.IsRenumber)
            {
                // Generate new numbers for everything we've decided to renumber

                // Renumber the PNGInstance we're referencing
                if (PngInstance != 0)
                    oldToNew.Add(PngInstance, CreateInstance());

                Dictionary<ulong, ulong> langMap = new Dictionary<ulong, ulong>();
                foreach (var kvp in rkToItem)
                    if (!oldToNew.ContainsKey(kvp.Value.SpecificRK.Instance))//Only generate a new IID once per resource in the package
                    {
                        IResourceKey rk = kvp.Value.SpecificRK;
                        if (rk.ResourceType == 0x220557DA)//STBL
                        {
                            if (!langMap.ContainsKey(rk.Instance & 0x00FFFFFFFFFFFFFF))
                                langMap.Add(rk.Instance & 0x00FFFFFFFFFFFFFF, CreateInstance() & 0x00FFFFFFFFFFFFFF);
                            oldToNew.Add(rk.Instance, rk.Instance & 0xFF00000000000000 | langMap[rk.Instance & 0x00FFFFFFFFFFFFFF]);
                        }
                        else if (cloneFixOptions.Is32bitIIDs &&
                                (rk.ResourceType == (uint)CatalogType.CatalogObject || rk.ResourceType == 0x02DC343F))//OBJD&OBJK
                            oldToNew.Add(rk.Instance, CreateInstance32());
                        else
                            oldToNew.Add(rk.Instance, CreateInstance());
                    }
                foreach (IResourceKey rk in rkToItem.Keys)//Requested RK
                    if (!oldToNew.ContainsKey(rk.Instance))//Find those references we don't have new IIDs for
                    {
                        if (rk.ResourceType == 0x736884F1 && rk.Instance >> 32 == 0)//Either it's a request for a VPXY using version...
                            oldToNew.Add(rk.Instance, oldToNew[rkToItem[rk].SpecificRK.Instance]);//So add the new number for that resource
                        else//Or it's an RCOL chunk that needs a new IID...
                            oldToNew.Add(rk.Instance, CreateInstance());//So renumber it
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

            if (itemName != null) resourceList.Add("Old CatlgName: \"" + itemName + "\" --> New CatlgName: \"" + tbCatlgName.Text + "\"");
            if (itemDesc != null) resourceList.Add("Old CatlgDesc: \"" + itemDesc + "\" --> New CatlgDesc: \"" + tbCatlgDesc.Text + "\"");
            resourceList.Add("Old Price: " + catlgItem.Resource["CommonBlock.Price"] + " --> New Price: " + float.Parse(tbPrice.Text));
            resourceList.Add("Old Product Status: 0x" + ((byte)catlgItem.Resource["CommonBlock.BuildBuyProductStatusFlags"].Value).ToString("X2") + " --> New Product Status: " + tbProductStatus.Text);
            if (PngInstance != 0 && cloneFixOptions.IsRenumber)
                resourceList.Add("Old PngInstance: " + selectedItem.Resource["CommonBlock.PngInstance"] + " --> New PngInstance: 0x" + oldToNew[PngInstance].ToString("X16"));
        }

        int numNewInstances = 0;
        ulong CreateInstance() { numNewInstances++; return FNV64.GetHash(numNewInstances.ToString("X8") + "_" + UniqueObject + "_" + DateTime.UtcNow.ToBinary().ToString("X16")); }
        ulong CreateInstance32() { numNewInstances++; return FNV32.GetHash(numNewInstances.ToString("X8") + "_" + UniqueObject + "_" + DateTime.UtcNow.ToBinary().ToString("X16")); }

        void StartFixing()
        {
            DoWait("Please wait, updating your package...");

            RunRKLookup();
            GenerateNewIIDs();

            Dictionary<IResourceKey, Item> rkToItemAdded = new Dictionary<IResourceKey, Item>();

            Item catlgItem = (selectedItem.CType == CatalogType.ModularResource || selectedItem.CType == CatalogType.CatalogFireplace) ? ItemForTGIBlock0(selectedItem) : selectedItem;
            //oldToNew = new Dictionary<ulong, ulong>();
            try
            {
                //Need to take a copy of the ResourceList as it can get modified, which messes up the enumerator
                IList<IResourceIndexEntry> lirie = new List<IResourceIndexEntry>(objPkgs[0].GetResourceList);
                foreach(IResourceIndexEntry irie in lirie)
                {
                    Item item = new Item(new RIE(objPkgs[0], irie));
                    bool dirty = false;

                    if ((item.RequestedRK.Equals(selectedItem.RequestedRK) && item.CType != CatalogType.ModularResource)//Selected CatlgItem
                        || item.CType == CatalogType.CatalogObject//all OBJDs (i.e. from MDLR or CFIR)
                        || item.CType == CatalogType.CatalogTerrainPaintBrush//all CTPTs (i.e. pair of selectedItem)
                        )
                    {
                        #region Selected CatlgItem; all OBJD (i.e. from MDLR or CFIR)
                        AHandlerElement commonBlock = ((AHandlerElement)item.Resource["CommonBlock"].Value);

                        #region Selected CatlgItem || all MDLR OBJDs || both CTPTs || 0th CFIR OBJD
                        if (item.RequestedRK.Equals(selectedItem.RequestedRK)//Selected CatlgItem
                            || selectedItem.CType == CatalogType.ModularResource//all MDLR OBJDs
                            || selectedItem.CType == CatalogType.CatalogTerrainPaintBrush//both CTPTs
                            || (selectedItem.CType == CatalogType.CatalogFireplace && item.RequestedRK.Equals(catlgItem.RequestedRK))//0th CFIR OBJD
                            )
                        {
                            commonBlock["NameGUID"] = new TypedValue(typeof(ulong), newNameGUID);
                            commonBlock["DescGUID"] = new TypedValue(typeof(ulong), newDescGUID);
                            commonBlock["Price"] = new TypedValue(typeof(float), float.Parse(tbPrice.Text));

                            if (cloneFixOptions.IsRenumber)
                            {
                                commonBlock["Name"] = new TypedValue(typeof(string), "CatalogObjects/Name:" + UniqueObject);
                                commonBlock["Desc"] = new TypedValue(typeof(string), "CatalogObjects/Description:" + UniqueObject);
                            }
                        }
                        #endregion

                        if (item.RequestedRK.Equals(selectedItem.RequestedRK)//Selected CatlgItem
                            || (selectedItem.CType == CatalogType.ModularResource && item.RequestedRK.Equals(catlgItem.RequestedRK))//0th MDLR OBJD
                            //-- only selected CTPT
                            //-- none of the OBJDs for CFIR
                            )
                        {
                            commonBlock["BuildBuyProductStatusFlags"] = new TypedValue(commonBlock["BuildBuyProductStatusFlags"].Type, Convert.ToByte(tbProductStatus.Text, tbProductStatus.Text.StartsWith("0x") ? 16 : 10));
                        }

                        #region Selected CatlgItem; 0th OBJD from MDLR or CFIR
                        if (item.RequestedRK.Equals(selectedItem.RequestedRK)//Selected CatlgItem
                            || (selectedItem.CType == CatalogType.ModularResource && item.RequestedRK.Equals(catlgItem.RequestedRK))//0th MDLR OBJD
                            //-- only selected CTPT
                            || (selectedItem.CType == CatalogType.CatalogFireplace && item.RequestedRK.Equals(catlgItem.RequestedRK))//0th CFIR OBJD
                            )
                        {
                            ulong PngInstance = (ulong)commonBlock["PngInstance"].Value;
                            bool isPng = PngInstance != 0;

                            if (cloneFixOptions.IsIncludeThumbnails)
                            {
                                Image img = replacementForThumbs != null ? replacementForThumbs : getLargestThumbOrDefault(item);
                                ulong instance = isPng ? PngInstance : item.RequestedRK.Instance;

                                //Ensure one of each size exists
                                foreach (THUM.THUMSize size in new THUM.THUMSize[] { THUM.THUMSize.small, THUM.THUMSize.medium, THUM.THUMSize.large, })
                                {
                                    if (getImage(size, item) == null)
                                    {
                                        //Create missing thumbnail from default or replacement
                                        IResourceKey rk = getNewRK(size, item);
                                        RIE rie = new RIE(objPkgs[0], objPkgs[0].AddResource(rk, null, true));
                                        rkToItemAdded.Add(rk, new Item(objPkgs, rk));
                                        Thumb[item.RequestedRK.ResourceType, instance, size, isPng] = img;
                                    }
                                    else if (replacementForThumbs != null)
                                    {
                                        //Replace existing thumbnail
                                        Thumb[item.RequestedRK.ResourceType, instance, size, isPng] = replacementForThumbs;
                                    }
                                }
                            }

                            if (isPng && oldToNew.ContainsKey(PngInstance))
                                commonBlock["PngInstance"] = new TypedValue(typeof(ulong), oldToNew[PngInstance]);

                            if (tabControl1.TabPages.Contains(tpDetail))
                                IterateTLP(tlpObjectDetail, (l, c) => UpdateItem(item, l, c));

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

                                if (tlpOther.Visible)
                                {
                                    for (int i = 1; i < tlpOther.RowCount - 1; i++)
                                        IterateTLP(tlpOther, (l, c) => UpdateItem(item, l, c));
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
                                    item.Resource["CommonBlock.UISortPriority"] = new TypedValue(typeof(uint), brushIndex);
                                else
                                    item.Resource["CommonBlock.UISortPriority"] = new TypedValue(typeof(uint), brushIndex + 1);
                            }
                            #endregion

                            if (item.CType == CatalogType.CatalogObject)
                            {
                                #region Avoid renumbering Fallback TGI
                                int fallbackIndex = (int)(uint)item.Resource["FallbackIndex"].Value;
                                TGIBlockList tgiBlocks = item.Resource["TGIBlocks"].Value as TGIBlockList;
                                AResourceKey fallbackRK = new TGIBlock(0, null, tgiBlocks[fallbackIndex]);

                                UpdateRKsFromField((AResource)item.Resource);

                                tgiBlocks[fallbackIndex] = new TGIBlock(0, null, (IResourceKey)fallbackRK);
                                #endregion
                            }
                            else
                                UpdateRKsFromField((AResource)item.Resource);
                        }

                        dirty = true;
                        #endregion
                    }
                    else if (item.SpecificRK.ResourceType == 0x0166038C)
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
                    else if (item.SpecificRK.ResourceType == 0x220557DA)//STBL
                    {
                        if (itemName != null) dirty |= UpdateStbl(tbCatlgName.Text, nameGUID, item, newNameGUID);
                        if (itemDesc != null) dirty |= UpdateStbl(tbCatlgDesc.Text, descGUID, item, newDescGUID);
                    }
                    else if (item.SpecificRK.ResourceType == 0x0333406C)
                    {
                        dirty = UpdateRKsFromXML(item);
                    }
                    else
                    {
                        dirty = UpdateRKsFromField((AResource)item.Resource);
                    }

                    if (dirty) item.Commit();

                }

                if (isPadSTBLs)
                {
                    if (itemName != null) PadStbls(tbCatlgName.Text, nameGUID, newNameGUID);
                    if (itemDesc != null) PadStbls(tbCatlgDesc.Text, descGUID, newDescGUID);
                    NMap.Commit();
                }

                foreach (var kvp in rkToItemAdded)
                    if (!rkToItem.ContainsKey(kvp.Key)) rkToItem.Add(kvp.Key, kvp.Value);
                foreach (Item item in rkToItem.Values)
                {
                    if (item.RequestedRK == RK.NULL) { continue; }
                    else if (!oldToNew.ContainsKey(item.SpecificRK.Instance)) { continue; }
                    else item.SpecificRK.Instance = oldToNew[item.SpecificRK.Instance];
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
                if (!t.IsClass || t.Equals(typeof(string))) continue;
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

        private bool UpdateStbl(string value, ulong guid, Item item, ulong newGuid)
        {
            Item srcStbl = findStblFor(objPkgs, guid, (byte)(item.SpecificRK.Instance >> 56));

            Application.DoEvents();
            if (srcStbl != null)
            {
                if ((item.SpecificRK.Instance & 0x00FFFFFFFFFFFFFF) == (srcStbl.SpecificRK.Instance & 0x00FFFFFFFFFFFFFF))
                {
                    IDictionary<ulong, string> stbl = (IDictionary<ulong, string>)item.Resource;

                    string text = "";
                    if (stbl.ContainsKey(guid)) { text = stbl[guid]; stbl.Remove(guid); }
                    if (ckbCopyToAll.Checked || item.SpecificRK.Instance >> 56 == 0x00) text = value;
                    if (text != "") stbl.Add(newGuid, text);

                    return true;
                }
            }
            return false;
        }

        private void PadStbls(string value, ulong guid, ulong newGuid)
        {
            try
            {
                updateProgress(true, "Padding STBLs...", true, 0x17, true, 0);
                Application.DoEvents();
                Item oldStbl = findStblFor(objPkgs, guid, x => { updateProgress(true, "Padding STBL...", false, 0, true, x); Application.DoEvents(); });
                updateProgress(true, "Padding STBL...", true, -1, false, 0);
                Application.DoEvents();
                if (oldStbl != null)
                {
                    IResourceKey newRK = new RK(oldStbl.SpecificRK);
                    for (byte lang = 0; lang < 0x17; lang++)
                        if (findStblFor(objPkgs, guid, lang) == null)
                        {
                            newRK.Instance = (newRK.Instance & 0x00FFFFFFFFFFFFFF) | ((ulong)lang << 56);
                            Item newstbl = new Item(objPkgs, newRK);
                            if (newstbl.SpecificRK == null)
                            {
                                RIE rie = new RIE(objPkgs[0], objPkgs[0].AddResource(newRK, null, true));
                                newstbl = new Item(rie);
                            }
                            IDictionary<ulong, string> outstbl = (IDictionary<ulong, string>)newstbl.Resource;
                            outstbl.Add(newGuid, value);
                            newstbl.Commit();

                            if (NMap != null && NMap.map != null && !NMap.map.ContainsKey(newRK.Instance))
                                NMap.map.Add(newRK.Instance, String.Format(language_fmt, languages[lang], lang, newRK.Instance & 0x00FFFFFFFFFFFFFF));

                            if (!rkToItem.ContainsKey(newRK))
                                rkToItem.Add(newRK, newstbl);
                        }
                }
            }
            finally { updateProgress(true, "", false, 0, false, 0); Application.DoEvents(); }
        }
        #endregion


        #region Fetch resources
        private void Add(string key, IResourceKey referencedRK)
        {
            if (resourceList.Count % 100 == 0) Application.DoEvents();
            rkLookup.Add(key, referencedRK);

            TGIN tgin = (AResourceKey)referencedRK;
            tgin.ResName = key;
            resourceList.Add(tgin);
        }

        private void SlurpRKsFromRK(string key, IResourceKey rk)
        {
            Item item = new Item(objPkgs, rk);
            if (item.SpecificRK != null) SlurpRKsFromField(key, (AResource)item.Resource);
            else Diagnostics.Show(String.Format("RK {0} not found", key));
        }
        private void SlurpRKsFromField(string key, AApiVersionedFields field)
        {
            Type t = field.GetType();
            if (typeof(GenericRCOLResource.ChunkEntry).IsAssignableFrom(t)) { }
            else if (typeof(TGIBlock).IsAssignableFrom(t))
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
                if (!t.IsClass || t.Equals(typeof(string))) continue;
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

        private void SlurpKindred(string key, IList<IPackage> pkgs, Predicate<IResourceIndexEntry> Match)
        {
            List<IResourceIndexEntry> seen = new List<IResourceIndexEntry>();
            foreach (IPackage pkg in pkgs)
            {
                IList<IResourceIndexEntry> lrie = pkg.FindAll(Match);
                int i = 0;
                foreach (IResourceIndexEntry rie in lrie)
                {
                    if (seen.Exists(new RK(rie).Equals)) continue;
                    seen.Add(rie);
                    Add(key + "[" + i + "]", rie);
                    i++;
                }
            }
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

            mode = Mode.FromUser;
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
            tmbPaths = ddsPaths = objPaths = new List<string>(new string[] { filename, });

            fileNewOpen(type, type != 0);
        }

        void fileNewOpen(CatalogType resourceType, bool IsFixPass)
        {
            menuBarWidget1.Enable(MenuBarWidget.MB.MBF_open, false);
            menuBarWidget1.Enable(MenuBarWidget.MD.MBC, false);
            menuBarWidget1.Enable(MenuBarWidget.MD.MBT, false);
            menuBarWidget1.Enable(MenuBarWidget.MD.MBS, false);

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
                setList(out objPkgs, out objPaths, ini_fb0);
                setList(out ddsPkgs, out ddsPaths, ini_fb2);
                setList(out tmbPkgs, out tmbPaths, ini_tmb);
                currentCatalogType = resourceType;
            }

            mode = Mode.FromGame;
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
        void setList(out List<IPackage> pkgs, out List<string> outPaths, List<string> inPaths)
        {
            pkgs = new List<IPackage>();
            outPaths = new List<string>();

            if (inPaths == null) return;
            foreach (string file in inPaths)
                if (File.Exists(file))
                    try { pkgs.Add(s3pi.Package.Package.OpenPackage(0, file)); outPaths.Add(file); }
                    catch { }
        }

        //Inge (15-01-2011): "Only ever looking *.package for custom content"
        //static List<string> pkgPatterns = new List<string>(new string[] { "*.package", "*.dbc", "*.world", "*.nhd", });
        static List<string> pkgPatterns = new List<string>(new string[] { "*.package", });
        void setCCList(out List<IPackage> pkgs, out List<string> outPaths, string ccPath)
        {
            pkgs = new List<IPackage>();
            outPaths = new List<string>();

            if (ccPath == null || !Directory.Exists(ccPath)) return;

            //Depth-first search
            foreach (var dir in Directory.GetDirectories(ccPath))
            {
                List<IPackage> dirPkgs;
                List<string> dirPaths;
                setCCList(out dirPkgs, out dirPaths, dir);
                if (dirPkgs != null && dirPkgs.Count > 0)
                {
                    pkgs.AddRange(dirPkgs);
                    outPaths.AddRange(dirPaths);
                }
            }
            foreach(string pattern in pkgPatterns)
                foreach (var path in Directory.GetFiles(ccPath, pattern))
                    try
                    {
                        IPackage pkg = s3pi.Package.Package.OpenPackage(0, path);
                        pkgs.Add(pkg);
                        outPaths.Add(path);
                    }
                    catch(InvalidDataException) { }
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
                    case MenuBarWidget.MB.MBT_tgiSearch: toolsTGISearch(); break;
                }
            }
            finally { this.Enabled = true; }
        }

        private void toolsSearch()
        {
            if (!CheckInstallDirs()) return;

            ClosePkg();
            setList(out objPkgs, out objPaths, ini_fb0);
            setList(out ddsPkgs, out ddsPaths, ini_fb2);
            setList(out tmbPkgs, out tmbPaths, ini_tmb);
            currentCatalogType = 0;

            searchPane = new Search(objPkgs, updateProgress);

            DisplaySearch();
        }

        private void toolsTGISearch()
        {
            if (!CheckInstallDirs()) return;

            ClosePkg();
            setList(out objPkgs, out objPaths, ini_fb0);
            setList(out ddsPkgs, out ddsPaths, ini_fb2);
            setList(out tmbPkgs, out tmbPaths, ini_tmb);
            currentCatalogType = 0;

            List<List<IPackage>> pkgsList = new List<List<IPackage>>(new List<IPackage>[] { objPkgs, ddsPkgs, tmbPkgs, });
            List<List<string>> pathsList = new List<List<string>>(new List<string>[] { objPaths, ddsPaths, tmbPaths, });
            if (ObjectCloner.Properties.Settings.Default.CCEnabled)
            {
                List<IPackage> ccPkgs;
                List<string> ccPaths;
                setCCList(out ccPkgs, out ccPaths, ObjectCloner.Properties.Settings.Default.CustomContent);
                pkgsList.Add(ccPkgs);
                pathsList.Add(ccPaths);
            }
            tgiSearchPane = new TGISearch(pkgsList, pathsList, updateProgress, NMap);

            DisplayTGISearch();
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
                    case MenuBarWidget.MB.MBS_langSearch: settingsLangSearch(); break;
                    case MenuBarWidget.MB.MBS_updates: settingsAutomaticUpdates(); break;
                    case MenuBarWidget.MB.MBS_advanced: settingsAdvancedCloning(); break;
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

        private void settingsLangSearch()
        {
            Application.DoEvents();
            ObjectCloner.Properties.Settings.Default.LangSearch = LangSearch = !LangSearch;
            menuBarWidget1.Checked(MenuBarWidget.MB.MBS_langSearch, LangSearch);
        }

        private void settingsAutomaticUpdates()
        {
            AutoUpdate.Checker.AutoUpdateChoice = !menuBarWidget1.IsChecked(MenuBarWidget.MB.MBS_updates);
        }

        private void settingsAdvancedCloning()
        {
            Application.DoEvents();
            ObjectCloner.Properties.Settings.Default.AdvanceCloning = !ObjectCloner.Properties.Settings.Default.AdvanceCloning;
            menuBarWidget1.Checked(MenuBarWidget.MB.MBS_advanced, ObjectCloner.Properties.Settings.Default.AdvanceCloning);
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
                    case MenuBarWidget.MB.MBH_update: helpUpdate(); break;
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

        private void helpUpdate()
        {
            bool msgDisplayed = AutoUpdate.Checker.GetUpdate(false);
            if (!msgDisplayed)
                CopyableMessageBox.Show("Your " + Application.ProductName + " is up to date", this.Text,
                    CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Information);
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
                case CatalogType.CatalogProxyProduct:
                case CatalogType.CatalogFountainPool:
                case CatalogType.CatalogFoundation:
                case CatalogType.CatalogWallStyle:
                case CatalogType.CatalogRoofStyle:
                    ThumbnailsOnly_Steps(stepList, out lastStepInChain); break;
                case CatalogType.CatalogTerrainGeometryBrush:
                case CatalogType.CatalogTerrainWaterBrush:
                    brush_Steps(stepList, out lastStepInChain); break;
                case CatalogType.CatalogTerrainPaintBrush:
                    CTPT_Steps(stepList, out lastStepInChain); break;

                case CatalogType.CatalogFence:
                case CatalogType.CatalogStairs:
                case CatalogType.CatalogRailing:
                case CatalogType.CatalogRoofPattern:
                    CatlgHasVPXY_Steps(stepList, out lastStepInChain); break;
                case CatalogType.CatalogObject:
                    OBJD_Steps(stepList, out lastStepInChain); break;
                case CatalogType.CatalogWallFloorPattern:
                    CWAL_Steps(stepList, out lastStepInChain); break;

                case CatalogType.CatalogFireplace:
                    CFIR_Steps(stepList, out lastStepInChain); break;
                case CatalogType.ModularResource:
                    MDLR_Steps(stepList, out lastStepInChain); break;
            }
            lastInChain = stepList == null ? -1 : (stepList.IndexOf(lastStepInChain) + 1);
        }

        void ThumbnailsOnly_Steps(List<Step> stepList, out Step lastStepInChain)
        {
            lastStepInChain = None;
            if (isCreateNewPackage || cloneFixOptions.IsRenumber)
            {
                stepList.AddRange(new Step[] {
                });
                if (cloneFixOptions.IsIncludeThumbnails || (!isCreateNewPackage && cloneFixOptions.IsRenumber))
                    stepList.Add(SlurpThumbnails);
            }
        }

        void brush_Steps(List<Step> stepList, out Step lastStepInChain)
        {
            ThumbnailsOnly_Steps(stepList, out lastStepInChain);
            if (isCreateNewPackage || cloneFixOptions.IsRenumber)
            {
                if (cloneFixOptions.IsDefaultOnly)
                {
                }
                else
                {
                    stepList.Insert(0, brush_addBrushShape);
                }
            }
        }

        void CTPT_Steps(List<Step> stepList, out Step lastStepInChain)
        {
            brush_Steps(stepList, out lastStepInChain);
            if (isCreateNewPackage || cloneFixOptions.IsRenumber)
            {
                stepList.InsertRange(0, new Step[] {
                    CTPT_addPair,
                    CTPT_addBrushTexture,
                });
            }
        }

        void CatlgHasVPXY_Steps(List<Step> stepList, out Step lastStepInChain)
        {
            lastStepInChain = None;
            if (isCreateNewPackage || cloneFixOptions.IsRenumber)
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
                    stepList.Insert(stepList.IndexOf(Catlg_getVPXY), Catlg_SlurpRKs);
                    stepList.Insert(stepList.IndexOf(Catlg_getVPXY), Catlg_removeRefdCatlgs);
                }
                if (cloneFixOptions.IsIncludeThumbnails || (!isCreateNewPackage && cloneFixOptions.IsRenumber))
                    stepList.Add(SlurpThumbnails);
            }
        }

        void OBJD_Steps(List<Step> stepList, out Step lastStepInChain)
        {
            CatlgHasVPXY_Steps(stepList, out lastStepInChain);
            if (isCreateNewPackage || cloneFixOptions.IsRenumber)
            {
                stepList.Remove(Catlg_getVPXY);
                stepList.InsertRange(0, new Step[] {
                    // OBJD_setFallback if cloning from game
                    OBJD_getOBJK,
                    // OBJD_addOBJKref and OBJD_SlurpDDSes if default resources only
                    OBJK_SlurpRKs,
                    OBJK_getSPT2,
                    OBJK_getVPXY,
                });
                if (!isFix && mode == Mode.FromGame)
                {
                    stepList.Insert(0, OBJD_setFallback);
                }
                if (cloneFixOptions.IsDefaultOnly)
                {
                    stepList.InsertRange(stepList.IndexOf(OBJK_SlurpRKs), new Step[] {
                        OBJD_addOBJKref,
                        OBJD_SlurpDDSes,
                    });
                }
                else
                {
                }
            }
        }

        void CWAL_Steps(List<Step> stepList, out Step lastStepInChain)
        {
            CatlgHasVPXY_Steps(stepList, out lastStepInChain);
            if (isCreateNewPackage || cloneFixOptions.IsRenumber)
            {
                if (cloneFixOptions.IsIncludeThumbnails || (!isCreateNewPackage && cloneFixOptions.IsRenumber))
                {
                    stepList.Remove(SlurpThumbnails);
                    stepList.Add(CWAL_SlurpThumbnails);
                }
            }
        }

        void CFIR_Steps(List<Step> stepList, out Step lastStepInChain)
        {
            stepList.AddRange(new Step[] { Item_findObjds, setupObjdStepList, Modular_Main, });
            if (cloneFixOptions.IsIncludeThumbnails || (!isCreateNewPackage && cloneFixOptions.IsRenumber))
                stepList.Add(SlurpThumbnails);//For the CFIR itself
            lastStepInChain = None;
        }

        void MDLR_Steps(List<Step> stepList, out Step lastStepInChain)
        {
            stepList.AddRange(new Step[] { Item_findObjds, setupObjdStepList, Modular_Main, });
            lastStepInChain = None;
        }

        Dictionary<Step, string> StepText;
        void SetStepText()
        {
            StepText = new Dictionary<Step, string>();
            StepText.Add(Item_addSelf, "Add selected item");
            StepText.Add(Catlg_SlurpRKs, "Catalog object-referenced resources");
            StepText.Add(Catlg_removeRefdCatlgs, "Remove referenced CatalogResources");
            StepText.Add(Catlg_getVPXY, "Find VPXYs in the Catalog Resource TGIBlockList");

            StepText.Add(OBJD_setFallback, "Set fallback TGI");
            StepText.Add(OBJD_getOBJK, "Find OBJK");
            StepText.Add(OBJD_addOBJKref, "Add OBJK");
            StepText.Add(OBJD_SlurpDDSes, "OBJD-referenced DDSes");
            StepText.Add(OBJK_SlurpRKs, "OBJK-referenced resources");
            StepText.Add(OBJK_getSPT2, "Find OBJK-referenced SPT2");
            StepText.Add(OBJK_getVPXY, "Find OBJK-referenced VPXY");

            StepText.Add(CTPT_addPair, "Add the other brush in pair");
            StepText.Add(CTPT_addBrushTexture, "Add Brush Texture");
            StepText.Add(brush_addBrushShape, "Add Brush Shape");

            StepText.Add(Catlg_addVPXYs, "Add VPXY resources");
            StepText.Add(VPXYs_SlurpRKs, "VPXY-referenced resources");
            StepText.Add(VPXYs_getKinXML, "Preset XML (same instance as VPXY)");
            StepText.Add(VPXYs_getKinMTST, "MTST (same instance as VPXY)");
            StepText.Add(VPXYKin_SlurpRKs, "Find Preset XML and MTST referenced resources");
            StepText.Add(VPXYs_getMODLs, "Find VPXY-referenced MODLs");
            StepText.Add(MODLs_SlurpRKs, "MODL-referenced resources");
            StepText.Add(MODLs_SlurpMLODs, "MLOD-referenced resources");
            StepText.Add(MODLs_SlurpTXTCs, "TXTC-referenced resources");

            StepText.Add(Item_findObjds, "Find OBJDs from MDLR/CFIR");
            StepText.Add(setupObjdStepList, "Get the OBJD step list");
            StepText.Add(Modular_Main, "Drive the modular process");

            StepText.Add(SlurpThumbnails, "Add thumbnails");
            StepText.Add(CWAL_SlurpThumbnails, "Add thumbnails");
        }

        void Item_addSelf() { Add("clone", selectedItem.RequestedRK); }
        void Catlg_SlurpRKs() { SlurpRKsFromField("clone", (AResource)selectedItem.Resource); }

        // Any interdependency between catalog resource is handled like CFIR (for complex ones) or CTPT (for simple ones)
        void Catlg_removeRefdCatlgs()
        {
            IList<TGIBlock> ltgi = (IList<TGIBlock>)selectedItem.Resource["TGIBlocks"].Value;
            foreach (AResourceKey rk in ltgi)
            {
                if (Enum.IsDefined(typeof(CatalogType), rk.ResourceType) && rk.Instance != selectedItem.RequestedRK.Instance)
                {
                    int i = new List<IResourceKey>(rkLookup.Values).IndexOf(rk);
                    if (i >= 0)
                        rkLookup.Remove(new List<string>(rkLookup.Keys)[i]);
                }
            }
        }

        void Catlg_getVPXY()
        {
            vpxyItems = new List<Item>();
            vpxyKinItems = new List<Item>();

            string s = "";
            foreach (IResourceKey rk in (TGIBlockList)selectedItem.Resource["TGIBlocks"].Value)
            {
                if (rk.ResourceType != 0x736884F1) continue;
                Item vpxy = new Item(new RIE(objPkgs, rk));
                if (vpxy.Resource != null)
                    vpxyItems.Add(vpxy);
                else
                    s += String.Format("Catalog Resource {0} -> RK {1}: not found\n", (IResourceKey)selectedItem.SpecificRK, rk);
            }
            Diagnostics.Show(s, "Missing VPXYs");
            if (vpxyItems.Count == 0)
            {
                Diagnostics.Show(String.Format("Catalog Resource {0} has no VPXY items", (IResourceKey)selectedItem.SpecificRK), "No VPXY items");
                stepNum = lastInChain;
            }
        }

        #region OBJD Steps
        void OBJD_setFallback()
        {
            if ((selectedItem.RequestedRK.ResourceGroup >> 27) > 0) return;// Only base game objects

            int fallbackIndex = (int)(uint)selectedItem.Resource["FallbackIndex"].Value;
            TGIBlockList tgiBlocks = selectedItem.Resource["TGIBlocks"].Value as TGIBlockList;
            if (tgiBlocks[fallbackIndex].Equals(RK.NULL))
            {
                selectedItem.Resource["FallbackIndex"] = new TypedValue(typeof(uint), (uint)tgiBlocks.Count, "X");
                tgiBlocks.Add("TGI", selectedItem.RequestedRK.ResourceType, selectedItem.RequestedRK.ResourceGroup, selectedItem.RequestedRK.Instance);
                selectedItem.Commit();
            }
        }
        void OBJD_getOBJK()
        {
            uint index = (uint)selectedItem.Resource["OBJKIndex"].Value;
            IList<TGIBlock> ltgi = (IList<TGIBlock>)selectedItem.Resource["TGIBlocks"].Value;
            TGIBlock objkTGI = ltgi[(int)index];
            objkItem = new Item(objPkgs, objkTGI);
            if (objkItem == null)
            {
                Diagnostics.Show(String.Format("OBJK {0} -> OBJK {1}: not found\n", (IResourceKey)selectedItem.SpecificRK, objkTGI), "Missing OBJK");
                stepNum = lastInChain;
            }

        }
        void OBJD_addOBJKref() { Add("objk", objkItem.RequestedRK); }
        void OBJD_SlurpDDSes()
        {
            IList<TGIBlock> ltgi = (IList<TGIBlock>)selectedItem.Resource["TGIBlocks"].Value;
            int i = 0;
            foreach (AApiVersionedFields mtdoor in (IList)selectedItem.Resource["MTDoors"].Value)
            {
                Add("clone.wallmask[" + i + "]", ltgi[(int)(uint)mtdoor["WallMaskIndex"].Value]);
                i++;
            }
            Add("clone.sinkmask", ltgi[(int)(uint)selectedItem.Resource["SurfaceCutoutDDSIndex"].Value]);
            if (selectedItem.Resource.ContentFields.Contains("FloorCutoutDDSIndex"))
                Add("clone.tubmask", ltgi[(int)(uint)selectedItem.Resource["FloorCutoutDDSIndex"].Value]);
        }
        void OBJK_SlurpRKs() { SlurpRKsFromField("objk", (AResource)objkItem.Resource); }
        void OBJK_getSPT2()
        {
            if (((ObjKeyResource.ObjKeyResource)objkItem.Resource).Components.FindAll(x => x.Element == ObjKeyResource.ObjKeyResource.Component.Tree).Count == 0) return;

            List<Item> spt2Items = new List<Item>();

            string s = "";
            TGIBlockList tgibl = ((ObjKeyResource.ObjKeyResource)objkItem.Resource).TGIBlocks;
            foreach (var rk in tgibl.FindAll(x => x.ResourceType == 0x00B552EA))//_SPT
            {
                Item spt2 = new Item(new RIE(objPkgs, new RK(rk) { ResourceType = 0x021D7E8C }));//SPT2
                if (spt2.SpecificRK != null && spt2.Resource != null)
                    spt2Items.Add(spt2);//SPT2
                else
                    s += String.Format("OBJK {0} -> _SPT -> SPT2 {1}: not found\n", (IResourceKey)objkItem.SpecificRK, spt2.RequestedRK);
            }
            Diagnostics.Show(s, "Missing SPT2s");
            if (spt2Items.Count == 0)
            {
                Diagnostics.Show(String.Format("OBJK {0} with a Tree Component has no SPT2 items", (IResourceKey)selectedItem.SpecificRK), "No SPT2 items");
            }

            //add SPT2s
            for (int i = 0; i < spt2Items.Count; i++) Add("spt2[" + i + "]", spt2Items[i].RequestedRK);
        }
        void OBJK_getVPXY()
        {
            int index = -1;
            if (((ObjKeyResource.ObjKeyResource)objkItem.Resource).ComponentData.ContainsKey("modelKey"))
                index = ((ObjKeyResource.ObjKeyResource.CDTResourceKey)((ObjKeyResource.ObjKeyResource)objkItem.Resource).ComponentData["modelKey"]).Data;

            if (index == -1)
            {
                Diagnostics.Show(String.Format("OBJK {0} has no modelKey", (IResourceKey)objkItem.SpecificRK), "Missing modelKey");
                stepNum = lastInChain;//Skip past the chain
                return;
            }

            vpxyItems = new List<Item>();
            vpxyKinItems = new List<Item>();

            string s = "";
            TGIBlockList tgibl = ((ObjKeyResource.ObjKeyResource)objkItem.Resource).TGIBlocks;
            foreach (IResourceKey rk in tgibl.FindAll(x => x.ResourceType == 0x736884F1))//VPXY
            {
                Item vpxy = new Item(new RIE(objPkgs, rk));
                if (vpxy.SpecificRK != null && vpxy.Resource != null)
                    vpxyItems.Add(vpxy);
                else
                    s += String.Format("OBJK {0} -> RK {1}: not found\n", (IResourceKey)objkItem.SpecificRK, rk);
            }
            Diagnostics.Show(s, "Missing VPXYs");
            if (vpxyItems.Count == 0)
            {
                Diagnostics.Show(String.Format("OBJK {0} has no VPXY items", (IResourceKey)selectedItem.SpecificRK), "No VPXY items");
                stepNum = lastInChain;
            }
        }
        #endregion

        void CTPT_addPair()
        {
            //int brushIndex = ("" + selectedItem.Resource["BrushTexture"]).GetHashCode();
            UInt64 brushIndex = (UInt64)selectedItem.Resource["CommonBlock.NameGUID"].Value;
            if (CTPTBrushIndexToPair.ContainsKey(brushIndex))
                Add("ctpt_pair", CTPTBrushIndexToPair[brushIndex].RequestedRK);
            else
                Diagnostics.Show(String.Format("CTPT {0} BrushIndex {1} not found", selectedItem.RequestedRK, brushIndex), "No ctpt_pair item");
        }
        void CTPT_addBrushTexture() { Add("ctpt_BrushTexture", (TGIBlock)selectedItem.Resource["BrushTexture"].Value); }
        void brush_addBrushShape() { Add("brush_ProfileTexture", (TGIBlock)selectedItem.Resource["ProfileTexture"].Value); }

        void Catlg_addVPXYs() { for (int i = 0; i < vpxyItems.Count; i++) Add("vpxy[" + i + "]", vpxyItems[i].RequestedRK); }
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
                SlurpKindred("vpxy[" + i + "].PresetXML", objPkgs, rie => rie.ResourceType == 0x0333406C && rie.Instance == vpxyItems[i].RequestedRK.Instance);
        }
        void VPXYs_getKinMTST()
        {
            for (int i = 0; i < vpxyItems.Count; i++)
                SlurpKindred("vpxy[" + i + "].mtst", objPkgs, rie => rie.ResourceType == 0x02019972 && rie.Instance == vpxyItems[i].RequestedRK.Instance);
        }
        void VPXYKin_SlurpRKs()
        {
            int i = 0;
            foreach(Item item in vpxyKinItems)
            {
                if (item.RequestedRK.ResourceType == (uint)0x0333406C)
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
                        TGIBlock tgib = vpxychunk.TGIBlocks[k];
                        if (tgib.ResourceType != 0x01661233) continue;
                        Item modl = new Item(new RIE(objPkgs, tgib));
                        if (modl.Resource != null)
                        {
                            found = true;
                            modlItems.Add(modl);
                        }
                    }
                    if (!found)
                        s += String.Format("VPXY {0} (chunk {1}) has no MODL items\n", (IResourceKey)vpxyItems[i].SpecificRK, j);
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
                    TGIBlock tgib = rcol.Resources[j];
                    if (tgib.ResourceType != 0x01D10F34) continue;
                    SlurpRKsFromRK("modl[" + i + "].mlod[" + k + "]", tgib);
                    k++;
                    found = true;
                }
                if (!found)
                    s += String.Format("MODL {0} has no MLOD items\n", (IResourceKey)modlItems[i].SpecificRK);
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
                    TGIBlock tgib = rcol.Resources[j];
                    if (tgib.ResourceType != 0x033A1435) continue;
                    SlurpRKsFromRK("modl[" + i + "].txtc[" + k + "]", tgib);
                    k++;
                    found = true;
                }
                if (!found)
                    s += String.Format("MODL {0} has no TXTC items\n", (IResourceKey)modlItems[i].SpecificRK);
            }
            Diagnostics.Show(s, "No TXTC items");
        }


        List<Item> objdList;
        void Item_findObjds()
        {
            objdList = new List<Item>();
            string s = "";
            foreach (IResourceKey rk in (TGIBlockList)selectedItem.Resource["TGIBlocks"].Value)
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
        void setupObjdStepList() { if (objdList.Count > 0) SetStepList(objdList[0], out objdSteps); }

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
            List<IResourceIndexEntry> seen = new List<IResourceIndexEntry>();
            foreach (THUM.THUMSize size in new THUM.THUMSize[] { THUM.THUMSize.small, THUM.THUMSize.medium, THUM.THUMSize.large, })
            {
                int i = 0;
                uint type = CWALThumbTypes[size];
                foreach (IPackage pkg in tmbPkgs)
                {
                    IList<IResourceIndexEntry> lrie = pkg.FindAll(rie => rie.ResourceType == type && rie.Instance == selectedItem.RequestedRK.Instance);
                    foreach (IResourceIndexEntry rie in lrie)
                    {
                        RIE Rie = new RIE(pkg, rie);
                        if (seen.Exists(Rie.RequestedRK.Equals)) continue;
                        Add(size + "[" + i++ + "]Thumb", Rie.RequestedRK);
                    }
                }
            }
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
