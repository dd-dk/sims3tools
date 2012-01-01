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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using s3pi.Interfaces;
using s3pi.GenericRCOLResource;
using meshExpImp.ModelBlocks;

namespace meshExpImp.Helper
{
    public partial class ExportForm : Form, s3pi.Helpers.IRunHelper
    {
        public ExportForm()
        {
            InitializeComponent();

            sfdExport.Filter = "s3m2b base files|*_filebase.s3m2b|All files|*.*";
            sfdExport.FileName = string.Format("{0}_filebase.s3m2b",
                Program.GetShortName());
        }

        Stream stream;
        public ExportForm(Stream s)
            : this()
        {
            stream = s;
            Application.DoEvents();
        }


        public byte[] Result { get { return null; } }

        private void Export_Shown(object sender, EventArgs e)
        {
            try
            {
                sfdExport.Title += " -- Test version " + typeof(ExportForm).Assembly.GetName().Version.ToString();
                DialogResult dr = sfdExport.ShowDialog();
                if (dr != DialogResult.OK)
                {
                    Environment.ExitCode = 1;
                    return;
                }

                string folder = Path.GetDirectoryName(sfdExport.FileName);
                string filebase = Path.GetFileNameWithoutExtension(sfdExport.FileName).Replace("_filebase", "");

                try
                {
                    using (FileStream fs = new FileStream(Path.Combine(folder, string.Format("{0}_filebase.s3m2b", filebase)), FileMode.Create, FileAccess.Write))
                    {
                        fs.Close();
                    }

                    GenericRCOLResource rcolResource = new GenericRCOLResource(0, stream);
                    MLOD mlod = null;

                    if (rcolResource.ChunkEntries[0].TGIBlock.ResourceType == 0x01661233)//MODL
                    {
                        this.Text = "Export MODL...";
                        var lodRef = rcolResource.GetMLODChunkRefforMODL();
                        if (lodRef == null)
                        {
                            CopyableMessageBox.Show("MODL (0x01661233) with no MLOD (0x01D10F34).",
                                "Invalid MODL resource", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Error);
                            Environment.ExitCode = 1;
                            return;
                        }


                        mlod = GenericRCOLResource.ChunkReference.GetBlock(rcolResource, lodRef) as MLOD;
                    }
                    else if (rcolResource.ChunkEntries[0].TGIBlock.ResourceType == 0x01D10F34)//MLOD
                    {
                        this.Text = "Export MLOD...";

                        mlod = rcolResource.ChunkEntries[0].RCOLBlock as MLOD;
                    }
                    else
                    {
                        CopyableMessageBox.Show("RCOL resource must be MODL (0x01661233) or MLOD (0x01D10F34).",
                            "Invalid RCOL resource", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Error);
                        Environment.ExitCode = 1;
                        return;
                    }

                    Export export = new Export(new MyProgressBar(label1, pb));

                    for (int m = 0; m < mlod.Meshes.Count; m++)
                    {
                        string fnMesh = Path.Combine(folder, string.Format("{0}_group{1:X2}.s3m2bg", filebase, m));

                        using (FileStream fsMesh = new FileStream(fnMesh, FileMode.Create, FileAccess.Write))
                        {
                            export.Export_MLOD(new StreamWriter(fsMesh), rcolResource, mlod, mlod.Meshes[m]);
                            fsMesh.Close();
                        }
                    }

                    Environment.ExitCode = 0;
                }
                catch (Exception ex)
                {
                    CopyableMessageBox.IssueException(ex, "Error processing " + Program.Filename);
                    throw ex;
                }
            }
            finally { this.Close(); }
        }
    }
}
