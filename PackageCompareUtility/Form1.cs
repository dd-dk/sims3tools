/***************************************************************************
 *  Copyright (C) 2012 by Peter L Jones                                    *
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
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using s3pi.Interfaces;
using s3pi.Extensions;

namespace PackageCompareUtility
{
    public partial class Form1 : Form
    {
        const string myName = "Package Compare Utility";

        public Form1()
        {
            InitializeComponent();
        }

        public Form1(params string[] args)
            : this()
        {
            CmdLine(args);
            btnGo.Enabled = File.Exists(tbPkgA.Text) && File.Exists(tbPkgB.Text);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            int r = CopyableMessageBox.Show("Delete temp files?", myName, CopyableMessageBoxButtons.YesNoCancel, CopyableMessageBoxIcon.Question);
            if (r == 2)
            {
                e.Cancel = true;
                return;
            }
            if (r == 1)
                return;
            CleanTemp();
        }

        // --- File

        private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            var mruLeft = Properties.Settings.Default.MRULeft;
            var mruRight = Properties.Settings.Default.MRURight;

            recentComparesToolStripMenuItem.DropDownItems.Clear();
            if (mruLeft == null || mruRight == null || mruLeft.Count != mruRight.Count)
            {
                recentComparesToolStripMenuItem.Enabled = false;
                return;
            }

            recentComparesToolStripMenuItem.Enabled = true;
            int j = 1;
            for (int i = 0; i < mruLeft.Count; i++)
            {
                if (File.Exists(mruLeft[i]) && File.Exists(mruRight[i]))
                {
                    ToolStripMenuItem mi = new ToolStripMenuItem();
                    mi.Text = "&" + j.ToString() + " " + Path.GetFileNameWithoutExtension(mruLeft[i]) + "  -v-  " + Path.GetFileNameWithoutExtension(mruRight[i]);
                    mi.ShortcutKeys = (Keys)((int)Keys.Control | (48 + j));
                    mi.Tag = new Int32?(i);
                    mi.Click += new EventHandler(mi_Click);
                    recentComparesToolStripMenuItem.DropDownItems.Add(mi);
                    j++;
                }
            }
        }

        void mi_Click(object sender, EventArgs e)
        {
            Int32? index = (sender as ToolStripMenuItem).Tag as Int32?;
            if (index == null) return;

            tbPkgA.Text = Properties.Settings.Default.MRULeft[index.Value];
            tbPkgB.Text = Properties.Settings.Default.MRURight[index.Value];

            btnGo_Click(this, EventArgs.Empty);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) { Application.Exit(); }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            string[] names = openFileDialog1.FileNames;
            if (names.Length > 2)
            {
                CopyableMessageBox.Show(this, "Please select one or two packages at a time.",
                    myName, CopyableMessageBoxIcon.Error, new List<string>(new string[] { "OK" }), 0, 0);
                e.Cancel = true;
                return;
            }
        }

        private void btnBrowseLeft_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = Path.GetFileName(tbPkgA.Text);
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr != DialogResult.OK)
                return;

            tbPkgA.Text = Path.GetFullPath(openFileDialog1.FileName);
            btnGo.Enabled = File.Exists(tbPkgA.Text) && File.Exists(tbPkgB.Text);
        }

        private void btnBrowseRight_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = Path.GetFileName(tbPkgB.Text);
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr != DialogResult.OK)
                return;

            tbPkgB.Text = Path.GetFullPath(openFileDialog1.FileName);
            btnGo.Enabled = File.Exists(tbPkgA.Text) && File.Exists(tbPkgB.Text);
        }

        private void btnLogFile_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(tbLogFile.Text);
            DialogResult dr = saveFileDialog1.ShowDialog();
            if (dr != DialogResult.OK)
                return;

            tbLogFile.Text = Path.GetFullPath(saveFileDialog1.FileName);
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            tbLogFile.Text = "";
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            List<string> pkgFiles = new List<string>();
            pkgFiles = new List<string>();
            pkgFiles.AddRange((new string[] { tbPkgA.Text, tbPkgB.Text, }).Where(x => !pkgFiles.Contains(Path.GetFullPath(x))).ToArray());

            IPackage pkgA = null;
            IPackage pkgB = null;
            try
            {
                pkgA = s3pi.Package.Package.OpenPackage(0, pkgFiles[0]);
                pkgB = s3pi.Package.Package.OpenPackage(0, pkgFiles[1]);
                UpdateMRU(pkgFiles[0], pkgFiles[1]);
            }
            catch (Exception ex)
            {
                CopyableMessageBox.IssueException(ex);

                if (pkgB != null)
                    try { s3pi.Package.Package.ClosePackage(0, pkgB); }
                    catch { }

                if (pkgA != null)
                    try { s3pi.Package.Package.ClosePackage(0, pkgA); }
                    catch { }

                return;
            }

            Compare(pkgFiles[0], pkgA, pkgFiles[1], pkgB, ckbNames.Checked);
        }

        // --- Help

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string copyright = "\n" +
                myName + "  Copyright (C) 2012  Peter L Jones\n" +
                "\n" +
                "This program comes with ABSOLUTELY NO WARRANTY; for details see Help->Warranty.\n" +
                "\n" +
                "This is free software, and you are welcome to redistribute it\n" +
                "under certain conditions; see Help->Licence for details.\n" +
                "\n" +
                "Please see Acknowledgements.txt for acknowledgements and\n" +
                "licence details of libraries used.\n";
            CopyableMessageBox.Show(String.Format(
                "{0}\n" +
                "Front-end Distribution: {1}\n" +
                "Library Distribution: {2}"
                , copyright
                , AutoUpdate.Version.CurrentVersion
                , AutoUpdate.Version.LibraryVersion
                ), myName);
        }

        private void warrantyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyableMessageBox.Show("\n" +
                "Disclaimer of Warranty.\n" +
                "\n" +
                "THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW.\n" +
                "EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR OTHER\n" +
                "PARTIES PROVIDE THE PROGRAM \"AS IS\" WITHOUT WARRANTY OF ANY KIND,\n" +
                "EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO,\n" +
                "THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.\n" +
                "THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU.\n" +
                "SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.\n" +
                "\n" +
                "\n" +
                "Limitation of Liability.\n" +
                "\n" +
                "IN NO EVENT UNLESS REQUIRED BY APPLICABLE LAW OR AGREED TO IN WRITING WILL ANY COPYRIGHT HOLDER,\n" +
                "OR ANY OTHER PARTY WHO MODIFIES AND/OR CONVEYS THE PROGRAM AS PERMITTED ABOVE,\n" +
                "BE LIABLE TO YOU FOR DAMAGES, INCLUDING ANY GENERAL, SPECIAL, INCIDENTAL OR CONSEQUENTIAL DAMAGES\n" +
                "ARISING OUT OF THE USE OR INABILITY TO USE THE PROGRAM\n" +
                "(INCLUDING BUT NOT LIMITED TO LOSS OF DATA OR DATA BEING RENDERED INACCURATE\n" +
                "OR LOSSES SUSTAINED BY YOU OR THIRD PARTIES OR A FAILURE OF THE PROGRAM TO OPERATE WITH ANY OTHER PROGRAMS),\n" +
                "EVEN IF SUCH HOLDER OR OTHER PARTY HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.\n" +
                "\n",
                myName);

        }

        private void licenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int dr = CopyableMessageBox.Show("\n" +
                "This program is distributed under the terms of the\nGNU General Public Licence version 3.\n" +
                "\n" +
                "If you wish to see the full text of the licence,\nplease visit http://www.fsf.org/licensing/licenses/gpl.html.\n" +
                "\n" +
                "Do you wish to visit this site now?" +
                "\n",
                myName,
                CopyableMessageBoxButtons.YesNo, CopyableMessageBoxIcon.Question, 1);
            if (dr != 0) return;
            Help.ShowHelp(this, "http://www.fsf.org/licensing/licenses/gpl.html");
        }

        private void checkForUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool msgDisplayed = AutoUpdate.Checker.GetUpdate(false);
            if (!msgDisplayed)
                CopyableMessageBox.Show("Your " + System.Configuration.PortableSettingsProvider.ExecutableName + " is up to date", myName,
                    CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Information);
        }

        // ---

        public void CmdLine(params string[] args)
        {
            var mruLeft = Properties.Settings.Default.MRULeft;
            var mruRight = Properties.Settings.Default.MRURight;
            if (mruLeft != null && mruRight != null)
            {
                tbPkgA.Text = mruLeft[0];
                tbPkgB.Text = mruRight[0];
            }

            List<string> cmdline = new List<string>(args);
            List<string> pkgs = new List<string>();
            while (cmdline.Count > 0)
            {
                string option = cmdline[0];
                cmdline.RemoveAt(0);
                if (!File.Exists(option))
                {
                    CopyableMessageBox.Show(this, "File not found:\n" + option,
                        myName, CopyableMessageBoxIcon.Error, new List<string>(new string[] { "OK" }), 0, 0);
                    Environment.Exit(1);
                }
                pkgs.Add(Path.GetFullPath(option));
            }
            if (pkgs.Count > 2)
            {
                CopyableMessageBox.Show(this, "Please select one or two packages at a time.",
                    myName, CopyableMessageBoxIcon.Error, new List<string>(new string[] { "OK" }), 0, 0);
                Environment.Exit(1);
            }


            if (pkgs.Count > 0) tbPkgA.Text = pkgs[0];
            if (pkgs.Count > 1) tbPkgB.Text = pkgs[1];

            btnGo.Enabled = File.Exists(tbPkgA.Text) && File.Exists(tbPkgB.Text);
        }

        struct Row
        {
            public IResourceKey leftRK;
            public string result;
            public IResourceKey rightRK;
        }

        void Compare(string pkgA, IPackage left, string pkgB, IPackage right, bool names)
        {
            StringWriter rkEqualMatch = new StringWriter();
            StringWriter rkEqualDiff = new StringWriter();
            StringWriter rkLeftOnly = new StringWriter();
            StringWriter rkRightOnly = new StringWriter();

            Dictionary<ulong, string> bothMaps = new Dictionary<ulong, string>();
            if (names)
            {
                foreach (var pkg in new[] { left, right })
                {
                    foreach (var rk in pkg.FindAll(x => x.ResourceType == 0x0166038C))
                    {
                        IDictionary<ulong, string> nmap = s3pi.WrapperDealer.WrapperDealer.GetResource(0, pkg, rk) as NameMapResource.NameMapResource;
                        foreach (var kvp in nmap)
                            if (!bothMaps.ContainsKey(kvp.Key))
                                bothMaps.Add(kvp.Key, kvp.Value);
                    }
                }
            }

            foreach (var row in Comparer(left, right))
            {
                if (row.leftRK != null)
                {
                    string key = ExtList.Ext[row.leftRK.ResourceType][0] + "-" + row.leftRK + (names && bothMaps.ContainsKey(row.leftRK.Instance) ? ": " + bothMaps[row.leftRK.Instance] : "");
                    if (row.rightRK != null)
                        (row.result == "=" ? rkEqualMatch : rkEqualDiff).WriteLine(key);
                    else
                        rkLeftOnly.WriteLine(key);
                }
                else
                    rkRightOnly.WriteLine(ExtList.Ext[row.rightRK.ResourceType][0] + "-" + row.rightRK + (names && bothMaps.ContainsKey(row.rightRK.Instance) ? ": " + bothMaps[row.rightRK.Instance] : ""));
            }
            rkEqualMatch.Close();
            rkEqualDiff.Close();
            rkLeftOnly.Close();
            rkRightOnly.Close();

            StreamWriter w;
            string filename;
            if (tbLogFile.Text.Length == 0)
            {
                filename = GetTempName();
                w = new StreamWriter(filename);
            }
            else
            {
                filename = tbLogFile.Text;
                Stream s = new FileStream(tbLogFile.Text, File.Exists(tbLogFile.Text) ? FileMode.Append : FileMode.CreateNew, FileAccess.Write);
                w = new StreamWriter(s);

                w.WriteLine();
                w.WriteLine();
                w.WriteLine("================================================================");
                w.WriteLine(DateTime.Now.ToString("F"));
                w.WriteLine("================================================================");
                w.WriteLine();
            }

            w.WriteLine("File 1: " + pkgA);
            w.WriteLine("File 2: " + pkgB);
            w.WriteLine();

            foreach (var line in rtbComment.Lines)
                w.WriteLine(line);
            if (rtbComment.TextLength > 0)
                w.WriteLine();

            bool noDiffs = true;

            if (rkLeftOnly.GetStringBuilder().ToString().Length > 0)
            {
                w.WriteLine("\n\nIn File1 (but not File2):");
                w.WriteLine(rkLeftOnly.GetStringBuilder().ToString());
                noDiffs = false;
            }

            if (rkRightOnly.GetStringBuilder().ToString().Length > 0)
            {
                w.WriteLine("\n\nIn File2 (but not File1):");
                w.WriteLine(rkRightOnly.GetStringBuilder().ToString());
                noDiffs = false;
            }

            if (rkEqualDiff.GetStringBuilder().ToString().Length > 0)
            {
                w.WriteLine("\n\nIn both (but different):");
                w.WriteLine(rkEqualDiff.GetStringBuilder().ToString());
                noDiffs = false;
            }

            if (noDiffs)
            {
                w.WriteLine("\n\nNo differences found.");
            }

            w.Close();

            if (tbLogFile.Text.Length == 0)
                Execute(filename);
            else
                CopyableMessageBox.Show("Done!", myName, CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Information);
        }

        IEnumerable<Row> Comparer(IPackage pkgA, IPackage pkgB)
        {
            List<IResourceIndexEntry> leftRKs = pkgA.FindAll(x => true), rightRKs = pkgB.FindAll(x => true);
            int left = 0;
            int right = 0;

            leftRKs.Sort();
            rightRKs.Sort();

            while (true)
            {
                Row row = new Row();
                if (left < leftRKs.Count)
                {
                    if (right < rightRKs.Count)
                    {
                        // both exist
                        switch (leftRKs[left].CompareTo(rightRKs[right]))
                        {
                            case -1:
                                // left RK < right RK
                                row.leftRK = leftRKs[left];
                                row.result = "<";
                                left++;
                                break;
                            case 0:
                                // same RK
                                row.leftRK = leftRKs[left];
                                row.rightRK = rightRKs[right];
                                row.result = Compare(pkgA, pkgB, leftRKs[left]);//seeing as they're the same
                                left++;
                                right++;
                                break;
                            case 1:
                                // left RK > right RK
                                row.rightRK = rightRKs[right];
                                row.result = ">";
                                right++;
                                break;
                        }
                    }
                    else
                    {
                        // left but no right
                        row.leftRK = leftRKs[left];
                        row.result = "<";
                        left++;
                    }
                }
                else
                {
                    if (right < rightRKs.Count)
                    {
                        // right but no left
                        row.rightRK = rightRKs[right];
                        row.result = ">";
                        right++;
                    }
                    else
                    {
                        // we're done
                        yield break;
                    }
                }

                yield return row;
            }
        }

        string Compare(IPackage pkgA, IPackage pkgB, IResourceKey rk)
        {
            IResource left = s3pi.WrapperDealer.WrapperDealer.GetResource(0, pkgA, pkgA.Find(x => x.CompareTo(rk) == 0), true);
            IResource right = s3pi.WrapperDealer.WrapperDealer.GetResource(0, pkgB, pkgB.Find(x => x.CompareTo(rk) == 0), true);

            return left.AsBytes.CompareTo(right.AsBytes) == 0 ? "=" : "!";
        }

        void UpdateMRU(string pkgA, string pkgB)
        {
            int maxMRU = 4;

            var mruLeftOld = Properties.Settings.Default.MRULeft;
            var mruRightOld = Properties.Settings.Default.MRURight;

            var mruLeftNew = new System.Collections.Specialized.StringCollection();
            var mruRightNew = new System.Collections.Specialized.StringCollection();

            mruLeftNew.Add(pkgA);
            mruRightNew.Add(pkgB);

            int i = 0;
            if (mruLeftOld != null && mruRightOld != null && mruLeftOld.Count == mruRightOld.Count)
            {
                while (i < mruLeftOld.Count && mruLeftNew.Count < maxMRU)
                {
                    if (mruLeftOld[i] != pkgA || mruRightOld[i] != pkgB)
                    {
                        if (File.Exists(mruLeftOld[i]) && File.Exists(mruRightOld[i]))
                        {
                            mruLeftNew.Add(mruLeftOld[i]);
                            mruRightNew.Add(mruRightOld[i]);
                        }
                    }
                    i++;
                }
            }

            Properties.Settings.Default.MRULeft = mruLeftNew;
            Properties.Settings.Default.MRURight = mruRightNew;
            Properties.Settings.Default.Save();
        }

        static void Execute(string command)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();

            p.StartInfo.FileName = command;
            p.StartInfo.UseShellExecute = true;

            try { p.Start(); }
            catch (Exception ex)
            {
                CopyableMessageBox.IssueException(ex, "Launch failed");
                return;
            }
        }

        string GetTempName()
        {
            return Path.GetTempPath() + "/" + myName + "-" + System.Diagnostics.Process.GetCurrentProcess().Id + "-" + Guid.NewGuid().ToString("N") + ".txt";
        }

        void CleanTemp()
        {
            foreach (var file in Directory.GetFiles(Path.GetTempPath(), myName + "-*.txt"))
                File.Delete(file);
        }
    }
}
