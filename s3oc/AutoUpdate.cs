/***************************************************************************
 *  Copyright (C) 2010 by Peter L Jones                                    *
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
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace AutoUpdate
{
    public class Version
    {
        static String timestamp;
        static Version()
        {
            String version_txt = Path.Combine(Path.GetDirectoryName(typeof(Version).Assembly.Location), Application.ProductName + "-Version.txt");
            System.IO.StreamReader sr = new StreamReader(version_txt);
            String line1 = sr.ReadLine();
            sr.Close();

            timestamp = line1.Trim();
        }

        public static String CurrentVersion { get { return timestamp; } }
    }
    public class UpdateInfo
    {
        Dictionary<string, string> pgmUpdate = new Dictionary<string, string>();
        public String AvailableVersion { get { return pgmUpdate["Version"]; } }
        public String UpdateURL { get { return pgmUpdate["UpdateURL"]; } }
        public String Message { get { return pgmUpdate["Message"]; } }
        public bool Reset { get { bool res = false; return pgmUpdate.ContainsKey("Reset") && bool.TryParse(pgmUpdate["Reset"], out res) ? res : false; } }

        public UpdateInfo(String url)
        {
            if (url.ToLower().EndsWith(".xml"))
            {
                try
                {
                    XmlReaderSettings xrs = new XmlReaderSettings();
                    xrs.CloseInput = true;
                    xrs.IgnoreComments = true;
                    xrs.IgnoreProcessingInstructions = true;
                    xrs.IgnoreWhitespace = true;
                    xrs.ProhibitDtd = false;
                    xrs.ValidationType = ValidationType.None;
                    XmlReader xr = XmlReader.Create(url, xrs);

                    xr.MoveToContent();
                    if (!xr.Name.Equals(Application.ProductName + "Update"))
                        xr.Skip();

                    while (xr.Read())
                        if (xr.MoveToContent() == XmlNodeType.Element)
                            pgmUpdate.Add(xr.Name, xr.ReadString());
                    xr.Close();
                }
                catch (XmlException xe) { throw new System.Net.WebException("Invalid Update Info file found:\n" + xe.Message); }

                if (!pgmUpdate.ContainsKey("Version") || !pgmUpdate.ContainsKey("UpdateURL") || !pgmUpdate.ContainsKey("Message"))
                    throw new System.Net.WebException("Invalid Update Info file found:\nData content invalid.");
            }
            else
            {
                TextReader tr = new StreamReader(new System.Net.WebClient().OpenRead(url));
                string line1 = tr.ReadLine().Trim();
                bool reset = false;
                if (bool.TryParse(line1, out reset))
                {
                    pgmUpdate.Add("Reset", reset ? Boolean.TrueString : Boolean.FalseString);
                    pgmUpdate.Add("Version", tr.ReadLine().Trim());
                }
                else
                {
                    pgmUpdate.Add("Version", line1);
                }
                pgmUpdate.Add("UpdateURL", tr.ReadLine().Trim());
                pgmUpdate.Add("Message", tr.ReadToEnd().Trim());
                tr.Close();
            }
        }
    }
    public class Checker
    {
        static ObjectCloner.Properties.Settings pgmSettings = ObjectCloner.Properties.Settings.Default;

        static Checker()
        {
            // Should only be set to "AskMe" the first time through (although it might have been reset by the user)
            if (pgmSettings.AutoUpdateChoice == 0) // AskMe
            {
                int dr = CopyableMessageBox.Show(
                    Application.ProductName + " is under development.\n"
                    + "It is recommended you allow automated update checking\n"
                    + "(no more than once per day, when the program is run).\n\n"
                    + "Do you want " + Application.ProductName + " to check for updates automatically?"
                    , Application.ProductName + " AutoUpdate Setting"
                    , CopyableMessageBoxButtons.YesNo, CopyableMessageBoxIcon.Question, -1, 1
                );
                if (dr == 0)
                    AutoUpdateChoice = true; // Daily
                else
                {
                    AutoUpdateChoice = false; // Manual
                    CopyableMessageBox.Show("You can enable AutoUpdate checking under the Settings Menu.\n" +
                        "Manual update checking is under the Help Menu."
                        , Application.ProductName + " AutoUpdate Setting"
                        , CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Information
                    );
                }
                pgmSettings.Save();
                OnAutoUpdateChoice_Changed();
            }
        }

        public static void Daily()
        {
#if DEBUGUPDATE
            GetUpdate(true);
#else
            if ((pgmSettings.AutoUpdateChoice == 1)
                && (DateTime.UtcNow.Date != pgmSettings.AULastUpdateTS.Date))
            {
                GetUpdate(true);
                pgmSettings.AULastUpdateTS = DateTime.UtcNow; // Only the automated check updates this setting
                pgmSettings.Save();
            }
#endif
        }

        public static bool GetUpdate(bool autoCheck)
        {
            UpdateInfo ui = null;
            string ini = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), Application.ProductName + "Update.ini");
            if (!File.Exists(ini))
            {
                CopyableMessageBox.Show(
                    "Problem checking for update" + (autoCheck ? " - will try again later" : "") + "\n"
                    + ini + " - not found"
                    , Application.ProductName + " AutoUpdate"
                    , CopyableMessageBoxButtons.OK
                    , CopyableMessageBoxIcon.Error);
                return true;
            }
            try
            {
                string url = new StreamReader(ini).ReadLine();
                if (url == null)
                    throw new IOException(ini + " - failed to read url");
                url = url.Trim();
                try
                {
                    StartSplash();
                    try { ui = new UpdateInfo(url); }
                    finally { StopSplash(); }
                }
                catch (System.Net.WebException we)
                {
                    if (we != null)
                    {
                        CopyableMessageBox.Show(
                            "Problem checking for update" + (autoCheck ? " - will try again later" : "") + "\n"
                            + (we.Response != null ? "\nURL: " + we.Response.ResponseUri : "")
                            + "\n" + we.Message
                            , Application.ProductName + " AutoUpdate"
                            , CopyableMessageBoxButtons.OK
                            , CopyableMessageBoxIcon.Error);
                        return true;
                    }
                }
            }
            catch (IOException ioe)
            {
                CopyableMessageBox.Show(
                    "Problem checking for update" + (autoCheck ? " - will try again later" : "") + "\n"
                    + ioe.Message
                    , Application.ProductName + " AutoUpdate"
                    , CopyableMessageBoxButtons.OK
                    , CopyableMessageBoxIcon.Error);
                return true;
            }

            if (UpdateApplicable(ui, autoCheck))
            {
                int dr = CopyableMessageBox.Show(
                    String.Format("{0}\n{3}\n\nCurrent version: {1}\nAvailable version: {2}",
                    ui.Message, Version.CurrentVersion, ui.AvailableVersion, ui.UpdateURL)
                    , Application.ProductName + " update available"
                    , CopyableMessageBoxIcon.Question
                    , new List<string>(new string[] { "&Visit link", "&Later", "&Skip version", }), 1, 2
                    );

                switch (dr)
                {
                    case 0: System.Diagnostics.Process.Start(ui.UpdateURL); break;
                    case 2: pgmSettings.AULastIgnoredVsn = ui.AvailableVersion; pgmSettings.Save(); break;
                }
                return true;
            }
            return false;
        }
        static void StartSplash() { }
        static void StopSplash() { }

        private static bool UpdateApplicable(UpdateInfo ui, bool autoCheck)
        {
            if (ui.AvailableVersion.CompareTo(Version.CurrentVersion) <= 0)
                return false;

            if (ui.Reset && ui.AvailableVersion.CompareTo(pgmSettings.AULastIgnoredVsn) != 0)
                pgmSettings.AULastIgnoredVsn = Version.CurrentVersion;

            if (autoCheck && ui.AvailableVersion.CompareTo(pgmSettings.AULastIgnoredVsn) <= 0)
                return false;

            return true;
        }

        public static event EventHandler AutoUpdateChoice_Changed;
        protected static void OnAutoUpdateChoice_Changed() { if (AutoUpdateChoice_Changed != null) AutoUpdateChoice_Changed(pgmSettings, EventArgs.Empty); }
        public static bool AutoUpdateChoice { get { return pgmSettings.AutoUpdateChoice == 1; } set { pgmSettings.AutoUpdateChoice = value ? 1 : 2; OnAutoUpdateChoice_Changed(); } }
    }
}
