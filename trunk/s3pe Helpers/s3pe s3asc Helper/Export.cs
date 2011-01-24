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
using s3piwrappers;

namespace s3ascHelper
{
    public partial class Export : Form, s3pi.Helpers.IRunHelper
    {
        public Export()
        {
            InitializeComponent();
            sfdExport.FileName = string.Format("{0}_filebase.s3asc", Program.Filename);
            sfdExport.Filter = Program.Filter;
        }

        GenericRCOLResource modlResource;
        public Export(Stream s)
            : this()
        {
            modlResource = new GenericRCOLResource(0, s);
            Application.DoEvents();
        }

        public byte[] Result { get { return null; } }

        private void Export_Shown(object sender, EventArgs e)
        {
            try
            {
                DialogResult dr = sfdExport.ShowDialog();
                if (dr != DialogResult.OK)
                {
                    Environment.ExitCode = 1;
                    return;
                }

                string folder = Path.GetDirectoryName(sfdExport.FileName);
                string filebase = Path.GetFileNameWithoutExtension(sfdExport.FileName).Replace("_filebase", "");

                FileStream fs = new FileStream(Path.Combine(folder, string.Format("{0}_filebase.s3asc", filebase)), FileMode.Create, FileAccess.Write);

                foreach (var chunk in modlResource.ChunkEntries.FindAll(x => x.RCOLBlock.ResourceType == 0x01661233))
                {
                    var modl = chunk.RCOLBlock as MODL;
                    foreach (var lodEntry in modl.Entries)
                    {
                        if (lodEntry.ModelLodIndex.RefType != GenericRCOLResource.ReferenceType.Public
                            && lodEntry.ModelLodIndex.RefType != GenericRCOLResource.ReferenceType.Private) continue;

                        var mlod = GenericRCOLResource.ChunkReference.GetBlock(modlResource, lodEntry.ModelLodIndex) as MLOD;


                        for (int m = 0; m < mlod.Meshes.Count; m++)
                        {
                            string fnMesh = Path.Combine(folder, string.Format("{0}_group{1:X2}.s3ascg", filebase, m));
                            using (FileStream fsMesh = new FileStream(fnMesh, FileMode.Create, FileAccess.Write))
                            {
                                StreamWriter w = new StreamWriter(fsMesh);

                                if (mlod.Meshes[m].GeometryStates.Count > 0)
                                {
                                    w.WriteLine(";");
                                    w.WriteLine("; Extended format: GeoStates follow IBUF");
                                    w.WriteLine(";");
                                }

                                VRTF vrtf = GenericRCOLResource.ChunkReference.GetBlock(modlResource, mlod.Meshes[m].VertexFormatIndex) as VRTF;
                                if (vrtf != null) Export_VRTF(w, vrtf);
                                else vrtf = VRTF.CreateDefaultForMesh(mlod.Meshes[m]);

                                Export_SKIN(w, GenericRCOLResource.ChunkReference.GetBlock(modlResource, mlod.Meshes[m].SkinControllerIndex) as SKIN, mlod.Meshes[m]);
                                Export_VBUF(w, GenericRCOLResource.ChunkReference.GetBlock(modlResource, mlod.Meshes[m].VertexBufferIndex) as VBUF, vrtf, mlod.Meshes[m]);
                                Export_IBUF(w, GenericRCOLResource.ChunkReference.GetBlock(modlResource, mlod.Meshes[m].IndexBufferIndex) as IBUF, mlod.Meshes[m]);

                                //For backward compatibility, these come after the IBUFs
                                Export_MeshGeoStates(w, mlod.Meshes[m]);

                                fsMesh.Close();
                            }
                            Application.DoEvents();
                        }
                    }
                }

                fs.Close();

                Environment.ExitCode = 0;

                Application.DoEvents();
            }
            catch (Exception ex) { CopyableMessageBox.IssueException(ex); }
            finally { this.Close(); }
        }

        void Export_VRTF(StreamWriter w, VRTF vrtf)
        {
            if (vrtf == null) { w.WriteLine("; vrtf is null"); w.WriteLine("vrtf 0 0"); return; }

            w.WriteLine(string.Format("vrtf {0} {1}", vrtf.Layouts.Count, vrtf.Stride));

            DateTime wait = DateTime.UtcNow;
            this.label1.Text = "Import VRTF...";
            this.pb.Value = 0;
            this.pb.Maximum = vrtf.Layouts.Count;
            for (int i = 0; i < vrtf.Layouts.Count; i++)
            {
                var l = vrtf.Layouts[i];
                w.WriteLine(string.Format("{0} {1} {2} {3} {4}", i, (byte)l.Usage, l.UsageIndex, (byte)l.Format, l.Offset));
                if (wait < DateTime.UtcNow) { this.pb.Value = i; wait = DateTime.UtcNow.AddSeconds(0.1); Application.DoEvents(); }
            }
            w.Flush();
        }

        void Export_SKIN(StreamWriter w, SKIN skin, MLOD.Mesh mesh)
        {
            if (skin == null) { w.WriteLine("; skin is null"); w.WriteLine("skin 0"); return; }

            if (skin.Bones.Count != mesh.JointReferences.Count) { w.WriteLine("; skin.Bones.Count != mesh.JointReferences.Count"); w.WriteLine("skin 0"); return; }

            w.WriteLine(string.Format("skin {0}", skin.Bones.Count));

            DateTime wait = DateTime.UtcNow;
            this.label1.Text = "Import VRTF...";
            this.pb.Value = 0;
            this.pb.Maximum = skin.Bones.Count;
            for (int i = 0; i < skin.Bones.Count; i++)
            {
                var bone = skin.Bones[mesh.JointReferences[i]];
                if (bone == null)
                {
                    w.WriteLine(string.Format("; Bone for Joint 0x{0:X8} not found", mesh.JointReferences[i]));
                    w.WriteLine(string.Format("{0} {1:X8} 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0", i, bone.NameHash));
                    continue;
                }
                w.WriteLine(string.Format("{0} {1:X8} {2:F6} {3:F6} {4:F6} {5:F6} {6:F6} {7:F6} {8:F6} {9:F6} {10:F6} {11:F6} {12:F6} {13:F6}",
                    i,
                    bone.NameHash,
                    bone.InverseBindPose.Right.X, bone.InverseBindPose.Right.Y, bone.InverseBindPose.Right.Z, bone.InverseBindPose.Translate.X,
                    bone.InverseBindPose.Up.X, bone.InverseBindPose.Up.Y, bone.InverseBindPose.Up.Z, bone.InverseBindPose.Translate.Y,
                    bone.InverseBindPose.Back.X, bone.InverseBindPose.Back.Y, bone.InverseBindPose.Back.Z, bone.InverseBindPose.Translate.Z));
                if (wait < DateTime.UtcNow) { this.pb.Value = i; wait = DateTime.UtcNow.AddSeconds(0.1); Application.DoEvents(); }
            }
            w.Flush();
        }

        void Export_VBUF(StreamWriter w, VBUF vbuf, VRTF vrtf, MLOD.Mesh mesh)
        {
            if (vbuf == null) { w.WriteLine("; vbuf is null"); w.WriteLine("vbuf 0"); return; }

            s3piwrappers.Vertex[] av = vbuf.GetVertices(mesh, vrtf);

            w.WriteLine(string.Format("vbuf {0}", av.Length));

            DateTime wait = DateTime.UtcNow;
            this.label1.Text = "Import VRTF...";
            this.pb.Value = 0;
            this.pb.Maximum = av.Length;
            for (int i = 0; i < av.Length; i++)
            {
                s3piwrappers.Vertex v = av[i];
                int nUV = 0;
                foreach (var layout in vrtf.Layouts)
                {
                    w.Write(string.Format("{0} {1}", i, (byte)layout.Usage));
                    switch (layout.Usage)
                    {
                        case VRTF.ElementUsage.Position:
                            if (v.Position != null) foreach (float f in v.Position) w.Write(string.Format(" {0:F6}", f));
                            else w.Write(" Position is null.");
                            break;
                        case VRTF.ElementUsage.Normal:
                            if (v.Normal != null) foreach (float f in v.Normal) w.Write(string.Format(" {0:F6}", f));
                            else w.Write(" Normal is null.");
                            break;
                        case VRTF.ElementUsage.UV:
                            if (v.UV != null) foreach (float f in v.UV[nUV]) w.Write(string.Format(" {0:F6}", f));
                            else w.Write(string.Format(" UV[{0}] is null.", nUV));
                            nUV++;
                            break;
                        case VRTF.ElementUsage.BlendIndex:
                            if (v.BlendIndices != null) foreach (byte b in v.BlendIndices) w.Write(string.Format(" {0}", b));
                            else w.Write(" BlendIndices is null.");
                            break;
                        case VRTF.ElementUsage.BlendWeight:
                            if (v.BlendWeights != null) foreach (float f in v.BlendWeights) w.Write(string.Format(" {0:F6}", f));
                            else w.Write(" BlendWeight is null.");
                            break;
                        case VRTF.ElementUsage.Tangent:
                            if (v.Tangents != null) foreach (float f in v.Tangents) w.Write(string.Format(" {0:F6}", f));
                            else w.Write(" Tangents is null.");
                            break;
                        case VRTF.ElementUsage.Colour:
                            if (v.Color != null) foreach (float f in v.Color) w.Write(string.Format(" {0:F6}", f));
                            else w.Write(" Colour is null.");
                            break;
                    }
                    w.WriteLine();
                    if (wait < DateTime.UtcNow) { this.pb.Value = i; wait = DateTime.UtcNow.AddSeconds(0.1); Application.DoEvents(); }
                }
            }

            w.Flush();
        }

        void Export_IBUF(StreamWriter w, IBUF ibuf, MLOD.Mesh mesh)
        {
            if (ibuf == null) { w.WriteLine("; ibuf is null"); w.WriteLine("ibuf 0"); return; }

            Int32[] indices = ibuf.GetIndices(mesh);

            int faces = indices.Length / 3;
            w.WriteLine(string.Format("ibuf {0}", faces));

            DateTime wait = DateTime.UtcNow;
            this.label1.Text = "Import VRTF...";
            this.pb.Value = 0;
            this.pb.Maximum = faces;
            for (int i = 0; i < faces; i++)
            {
                w.WriteLine(string.Format("{0} {1} {2} {3}", i, indices[i * 3 + 0], indices[i * 3 + 1], indices[i * 3 + 2]));
                if (wait < DateTime.UtcNow) { this.pb.Value = i; wait = DateTime.UtcNow.AddSeconds(0.1); Application.DoEvents(); }
            }

            w.Flush();
        }

        void Export_MeshGeoStates(StreamWriter w, MLOD.Mesh mesh)
        {
            if (mesh.GeometryStates.Count <= 0) return;

            w.WriteLine(";");
            w.WriteLine("; Extended format: GeoStates");
            w.WriteLine(";");

            w.WriteLine(string.Format("geos {0}", mesh.GeometryStates.Count));

            for (int g = 0; g < mesh.GeometryStates.Count; g++)
            {
                MLOD.GeometryState s = mesh.GeometryStates[g];
                w.WriteLine(string.Format("{0} {1:X8} {2} {3} {4} {5}", g, s.Name, s.StartIndex, s.MinVertexIndex, s.VertexCount, s.PrimitiveCount));
            }

            w.Flush();
        }
    }
}
