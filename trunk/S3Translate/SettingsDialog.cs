/*
 *  Copyright 2009 Jonathan Haas
 * 
 *  This file is part of s3translate.
 *  
 *  s3translate is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  s3translate is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with s3translate.  If not, see <http://www.gnu.org/licenses/>.
 *  
 *  s3translate uses the s3pi libraries by Peter L Jones (pljones@users.sf.net)
 *  For details visit http://sourceforge.net/projects/s3pi/
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using S3Translate.Properties;

namespace S3Translate
{
    public partial class SettingsDialog : Form
    {
        public SettingsDialog()
        {
            InitializeComponent();
            cmbSourceLang.DataSource = Form1.locales.AsReadOnly();
            cmbTargetLang.DataSource = Form1.locales.AsReadOnly();
            try
            {
                cmbSourceLang.SelectedIndex = Settings.Default.SourceLocale;
                cmbTargetLang.SelectedIndex = Settings.Default.UserLocale;
            }
            catch (Exception){}
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SettingsDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            Settings.Default.SourceLocale = (byte)cmbSourceLang.SelectedIndex;
            Settings.Default.UserLocale = (byte)cmbTargetLang.SelectedIndex;
            Settings.Default.Save();
        }
    }
}
