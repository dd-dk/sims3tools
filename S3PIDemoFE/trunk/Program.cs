/***************************************************************************
 *  Copyright (C) 2009 by Peter L Jones                                    *
 *  peter@users.sf.net                                                     *
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
using System.Windows.Forms;

namespace S3PIDemoFE
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Settings();
#if DEBUG
            Application.Run(new MainForm());
#else
            try
            {
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                Form f = new Form();
                RichTextBox rtf = new RichTextBox();
                rtf.Font = new System.Drawing.Font("DejaVu Sans Mono", 8);
                rtf.Size = new System.Drawing.Size(640 - (640 / 5), 480 - (480 / 5));
                rtf.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                rtf.ReadOnly = true;
                f.Size = new System.Drawing.Size(rtf.Width + 12, rtf.Height + 36);
                //f.SizeGripStyle = SizeGripStyle.Hide;
                f.Text = "S3PIDemoFE";
                f.StartPosition = FormStartPosition.CenterParent;
                f.Controls.Add(rtf);
                string s = ex.ToString();
                if (ex.InnerException != null)
                    s += "\n\n-- Inner Exception --\n" + ex.InnerException.ToString();
                rtf.Text = s;
                f.ShowDialog();
                return 1;
            }
#endif
            return 0;
        }

        static void Settings()
        {
            if( Properties.Settings.Default.UpgradeRequired ) {
                // Bulk migrate settings from previous version
                try
                {
                    Properties.Settings.Default.Upgrade();
                    Properties.Settings.Default.Reload();
                }
                catch (System.Configuration.ConfigurationException)
                {
                    // Any problems, overwrite with current!
                    Properties.Settings.Default.Reset();
                }

                // Prevent further upgrades
                Properties.Settings.Default.UpgradeRequired = false;
                Properties.Settings.Default.Save();
            }
        }
    }
}
