using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace ViewDDS
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(params string[] args)
        {
            bool useClipboard = false;
            bool useFile = false;
            List<string> files = new List<string>();

            foreach (string s in args)
            {
                string p = s;
                if (p.StartsWith("/") || p.StartsWith("-"))
                {
                    if ("clipboard".StartsWith(p.Substring(1).ToLower()))
                        useClipboard = true;
                }
                else
                    files.Add(s);
            }
            useFile = !useClipboard && files.Count > 0;

            if (!useClipboard && !useFile)
            {
                MessageBox.Show("Either pass \"/Clipboard\" switch or a filename", "Fail", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return 1;
            }
            if (useClipboard && !Clipboard.ContainsData(DataFormats.Serializable))
            {
                MessageBox.Show("No suitable data on clipboard", "Fail", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return 2;
            }
            if (useFile)
            {
                MessageBox.Show("Only pass one file at a time", "Fail", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                if (files.Count > 1) return 3;
                MemoryStream ms = new MemoryStream();
                FileStream fs = new FileStream(files[0], FileMode.Open, FileAccess.Read);
                (new BinaryWriter(ms)).Write((new BinaryReader(fs)).ReadBytes((int)fs.Length));
                fs.Close();
                ms.Flush();
                Clipboard.SetData(DataFormats.Serializable, ms.GetBuffer());
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                for (Exception inex = ex.InnerException; inex != null; inex = ex.InnerException) s += "\n" + inex.Message;
                    MessageBox.Show(s, "Fail", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return -1;
            }

            return 0;
        }
    }
}
