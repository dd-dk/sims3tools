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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using s3pi.Interfaces;
using ScriptResource;

namespace S3SA_DLL_ExpImp
{
    public partial class Import : Form, s3pi.Helpers.IRunHelper
    {
        public Import()
        {
            InitializeComponent();
        }

        ScriptResource.ScriptResource s3sa;
        public Import(Stream s)
            : this()
        {
            s3sa = new ScriptResource.ScriptResource(0, s);
            Application.DoEvents();
        }

        byte[] result = null;
        public byte[] Result { get { return result; } }

        private void Import_Load(object sender, EventArgs e)
        {
            Application.DoEvents();

            ofdImport.FileName = Program.getAssemblyName(s3sa);
            DialogResult dr = ofdImport.ShowDialog();
            if (dr != DialogResult.OK)
            {
                Environment.ExitCode = 1;
            }

            FileStream fs = new FileStream(ofdImport.FileName, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            s3sa.Assembly = br;
            br.Close();
            fs.Close();

            result = (byte[])s3sa.AsBytes.Clone();

            Application.DoEvents();
            Environment.ExitCode = 0;
            this.Close();
        }
    }
}
