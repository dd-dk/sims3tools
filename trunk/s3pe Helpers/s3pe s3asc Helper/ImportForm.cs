﻿/***************************************************************************
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
using System.Windows.Forms;
using s3pi.Interfaces;
using s3pi.GenericRCOLResource;
using s3piwrappers;

namespace s3ascHelper
{
    public partial class ImportForm : Form, s3pi.Helpers.IRunHelper
    {
        public ImportForm()
        {
            InitializeComponent();

            ofdImport.Filter = Program.GetFilter();
            ofdImport.FileName = string.Format("{0}_filebase.{1}",
                Program.UseFormat == Program.Format.s3m2b ? Program.GetShortName() : Program.Filename,
                Program.GetExtension());
        }

        Stream stream;
        public ImportForm(Stream s)
            : this()
        {
            stream = s;
            Application.DoEvents();
        }

        byte[] result = null;
        public byte[] Result { get { return result; } }

        private void Import_Shown(object sender, EventArgs e)
        {
            try
            {
                DialogResult dr = ofdImport.ShowDialog();
                if (dr != DialogResult.OK)
                {
                    Environment.ExitCode = 1;
                    return;
                }

                string folder = Path.GetDirectoryName(ofdImport.FileName);
                string filebase = Path.GetFileNameWithoutExtension(ofdImport.FileName).Replace("_filebase", "");

                if (!File.Exists(Path.Combine(folder, string.Format("{0}_filebase.{1}", filebase, Program.GetExtension()))))
                {
                    CopyableMessageBox.Show("File name must end \"_filebase." + Program.GetExtension() + "\"",
                        "Base file not found", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Error);
                    Environment.ExitCode = 1;
                    return;
                }

                bool updateBBs = false;
                int q = System.Windows.Forms.CopyableMessageBox.Show("Update mesh bounding boxes?", Application.ProductName, CopyableMessageBoxButtons.YesNoCancel, CopyableMessageBoxIcon.Question);
                if (q == 0)
                    updateBBs = true;
                else if (q == 2)
                {
                    Environment.ExitCode = 1;
                    return;
                }

                try
                {
                    GenericRCOLResource rcolResource = new GenericRCOLResource(0, stream);
                    MLOD mlod = null;
                    IResourceKey rk = null;

                    if (rcolResource.ChunkEntries[0].TGIBlock.ResourceType == 0x01661233)//MODL
                    {
                        this.Text = "Import MODL...";
                        var lodRef = rcolResource.GetMLODChunkRefforMODL();
                        if (lodRef == null)
                        {
                            CopyableMessageBox.Show("MODL (0x01661233) with no MLOD (0x01D10F34).",
                                "Invalid MODL resource", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Error);
                            Environment.ExitCode = 1;
                            return;
                        }

                        rk = GenericRCOLResource.ChunkReference.GetKey(rcolResource, lodRef);
                        mlod = GenericRCOLResource.ChunkReference.GetBlock(rcolResource, lodRef) as MLOD;
                    }
                    else if (rcolResource.ChunkEntries[0].TGIBlock.ResourceType == 0x01D10F34)//MLOD
                    {
                        this.Text = "Import MLOD...";
                        rk = rcolResource.ChunkEntries[0].TGIBlock;
                        mlod = rcolResource.ChunkEntries[0].RCOLBlock as MLOD;
                    }
                    else
                    {
                        CopyableMessageBox.Show("RCOL resource must be MODL (0x01661233) or MLOD (0x01D10F34).",
                            "Invalid RCOL resource", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Error);
                        Environment.ExitCode = 1;
                        return;
                    }

                    Import import = new Import(new MyProgressBar(label1, pb));

                    int m = 0;
                    List<s3piwrappers.Vertex[]> lmverts = new List<s3piwrappers.Vertex[]>();
                    List<List<s3piwrappers.Vertex[]>> llverts = new List<List<s3piwrappers.Vertex[]>>();
                    while (true)
                    {
                        string fnMesh = Path.Combine(folder, string.Format("{0}_group{1:X2}.{2}g", filebase, m, Program.GetExtension()));
                        if (!File.Exists(fnMesh)) break;

                        using (FileStream fsMesh = new FileStream(fnMesh, FileMode.Open, FileAccess.Read))
                        {
                            s3piwrappers.Vertex[] mverts;
                            List<s3piwrappers.Vertex[]> lverts;
                            import.Import_Mesh(new StreamReader(fsMesh), mlod.Meshes[m++], rcolResource, mlod, rk, out mverts, out lverts);
                            lmverts.Add(mverts);
                            llverts.Add(lverts);
                            fsMesh.Close();
                        }
                    }

                    import.VertsToVBUFs(rcolResource, mlod, rk, lmverts, llverts, updateBBs);

                    result = (byte[])rcolResource.AsBytes.Clone();

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
