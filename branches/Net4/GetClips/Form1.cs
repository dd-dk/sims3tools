using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using s3pi.Filetable;
using s3pi.Interfaces;
using Namemap;

namespace GetClips
{
    public partial class Form1 : Form
    {
        bool stop = false;
        bool running = false;
        public Form1()
        {
            InitializeComponent();


            GameFoldersForm gff = new GameFoldersForm();
            gff.ShowInTaskbar = true;
            gff.StartPosition = FormStartPosition.CenterScreen;

            bool ccEnabled = GetClips.Properties.Settings.Default.CustomContentEnabled;
            string customContent = GetClips.Properties.Settings.Default.CustomContentPath;
            string installDirs = GetClips.Properties.Settings.Default.InstallDirs;
            string epsDisabled = GetClips.Properties.Settings.Default.EPsDisabled;

            for (DialogResult dr = DialogResult.Retry; dr == DialogResult.Retry; )
            {
                gff.CCEnabled = ccEnabled;
                gff.CustomContent = customContent;
                gff.InstallDirs = installDirs;
                gff.EPsDisabled = epsDisabled;
                dr = gff.ShowDialog();
            }
            GetClips.Properties.Settings.Default.CustomContentEnabled = FileTable.CustomContentEnabled = gff.CCEnabled;
            GetClips.Properties.Settings.Default.CustomContentPath = FileTable.CustomContentPath = gff.CustomContent;
            GetClips.Properties.Settings.Default.InstallDirs = GameFolders.InstallDirs = gff.InstallDirs;
            GetClips.Properties.Settings.Default.EPsDisabled = epsDisabled;
            GetClips.Properties.Settings.Default.Save();

            FileTable.FileTableEnabled = true;

            NameMap.Reset();
        }

        static int updateEvery = 1000;
        static int tickTime = 50;
        private void Form1_Shown(object sender, EventArgs e)
        {
            if (FileTable.GameContent == null)
            {
                richTextBox1.Text = "No game content found.";
                return;
            }

            running = true;

            List<SpecificIndexEntry> clips = new List<SpecificIndexEntry>();

            DateTime now = DateTime.UtcNow.AddMilliseconds(updateEvery);
            DateTime tick = DateTime.UtcNow.AddMilliseconds(tickTime);
            foreach (var sie in FileTable.GameContent.SelectMany(ppt => ppt.Package.FindAll(rie => rie.ResourceType == 0x6B20C4F3).Select(rie => new SpecificIndexEntry(ppt, rie))))
            {
                if (stop) { Environment.Exit(0); return; }
                if (DateTime.UtcNow > now)
                {
                    if (richTextBox1.Text.Length > 3)
                        richTextBox1.Text = "...";
                    else
                        richTextBox1.Text = "Please wait, processing... " + clips.Count + "\n" + sie.PathPackage.Path;
                    Application.DoEvents();
                    now = DateTime.UtcNow.AddMilliseconds(updateEvery);
                }
                if (DateTime.UtcNow > tick)
                {
                    Application.DoEvents();
                    tick = DateTime.UtcNow.AddMilliseconds(tickTime);
                }
                if (clips.Exists(x => x.RequestedRK.Instance == sie.RequestedRK.Instance)) continue;
                clips.Add(sie);
            }

            richTextBox1.Text = "Please wait, sorting output...";
            Application.DoEvents();
            clips.Sort((x, y) => (x.RequestedRK.Instance << 8).CompareTo(y.RequestedRK.Instance << 8));

            richTextBox1.Text = "Please wait, formatting output...";
            Application.DoEvents();
            string[] lines = new string[clips.Count];
            for (int i = 0; i < lines.Length; i++)
            {
                if (stop) { Environment.Exit(0); return; }
                if (DateTime.UtcNow > tick)
                {
                    Application.DoEvents();
                    tick = DateTime.UtcNow.AddMilliseconds(tickTime);
                }

                string name = NameMap.NMap[clips[i].RequestedRK.Instance];
                lines[i] = (name == null ? "(unknown)" : name) + "  " + clips[i].RequestedRK + "  " + clips[i].PathPackage.Path;
            }

            if (stop) { Environment.Exit(0); return; }
            richTextBox1.Lines = lines;
            running = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && running)
            {
                stop = true;
                e.Cancel = true;
            }
        }
    }
}
