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
        MyProgressBar mpb;
        public Export()
        {
            InitializeComponent();
            mpb = new MyProgressBar(label1, pb);
            sfdExport.FileName = string.Format("{0}_filebase.s3asc", Program.Filename);
        }

        GenericRCOLResource rcolResource;
        public Export(Stream s)
            : this()
        {
            rcolResource = new GenericRCOLResource(0, s);
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

                if (rcolResource.ChunkEntries[0].TGIBlock.ResourceType == 0x01661233)
                {
                    this.Text = "Export MODL...";
                    var modl = rcolResource.ChunkEntries[0].RCOLBlock as MODL;
                    foreach (var lodEntry in modl.Entries)
                    {
                        if (lodEntry.ModelLodIndex.RefType != GenericRCOLResource.ReferenceType.Public
                            && lodEntry.ModelLodIndex.RefType != GenericRCOLResource.ReferenceType.Private) continue;

                        var mlod = GenericRCOLResource.ChunkReference.GetBlock(rcolResource, lodEntry.ModelLodIndex) as MLOD;

                        Export_MLOD(mlod, folder, filebase);
                    }
                }
                else if (rcolResource.ChunkEntries[0].TGIBlock.ResourceType == 0x01D10F34)
                {
                    this.Text = "Export MLOD...";
                    var mlod = rcolResource.ChunkEntries[0].RCOLBlock as MLOD;
                    Export_MLOD(mlod, folder, filebase);
                }

                fs.Close();

                Environment.ExitCode = 0;

                Application.DoEvents();
            }
            catch (Exception ex) { CopyableMessageBox.IssueException(ex); }
            finally { this.Close(); }
        }

        void Export_MLOD(MLOD mlod, string folder, string filebase)
        {
            for (int m = 0; m < mlod.Meshes.Count; m++)
            {
                string fnMesh = Path.Combine(folder, string.Format("{0}_group{1:X2}.s3ascg", filebase, m));
                using (FileStream fsMesh = new FileStream(fnMesh, FileMode.Create, FileAccess.Write))
                {
                    // need to get the MATD for this mesh, following any MTST chain
                    MATD matd = GetMATDforMesh(mlod.Meshes[m].MaterialIndex);
                    float uvScale = 1f / 32767f;
                    if (matd != null)
                    {
                        MATD.ShaderData data = (matd.Version < 0x0103 ? matd.Mtrl.SData : matd.Mtnf.SData).Find(x => x.Field == MATD.FieldType.UVScales);
                        if (data != null)
                        {
                            if (data is MATD.ElementFloat) { uvScale = (data as MATD.ElementFloat).Data; }
                            else if (data is MATD.ElementFloat2) { uvScale = (data as MATD.ElementFloat2).Data0; }
                            else if (data is MATD.ElementFloat3) { uvScale = (data as MATD.ElementFloat3).Data0; }
                            else if (data is MATD.ElementFloat4) { uvScale = (data as MATD.ElementFloat4).Data0; }
                        }
                    }

                    StreamWriter w = new StreamWriter(fsMesh);

                    if (mlod.Meshes[m].GeometryStates.Count > 0)
                    {
                        w.WriteLine(";");
                        w.WriteLine("; Extended format: GeoStates follow IBUF");
                        w.WriteLine(";");
                    }

                    VRTF vrtf = GenericRCOLResource.ChunkReference.GetBlock(rcolResource, mlod.Meshes[m].VertexFormatIndex) as VRTF;
                    bool isDefault = vrtf == null;
                    if (isDefault) vrtf = VRTF.CreateDefaultForMesh(mlod.Meshes[m]);
                    Export_VRTF(w, vrtf, isDefault);

                    Export_SKIN(w, GenericRCOLResource.ChunkReference.GetBlock(rcolResource, mlod.Meshes[m].SkinControllerIndex) as SKIN, mlod.Meshes[m]);
                    Export_VBUF(w, GenericRCOLResource.ChunkReference.GetBlock(rcolResource, mlod.Meshes[m].VertexBufferIndex) as VBUF, vrtf, uvScale, mlod.Meshes[m]);
                    Export_IBUF(w, GenericRCOLResource.ChunkReference.GetBlock(rcolResource, mlod.Meshes[m].IndexBufferIndex) as IBUF, mlod.Meshes[m]);

                    //For backward compatibility, these come after the IBUFs
                    Export_MeshGeoStates(w, vrtf, uvScale, mlod, mlod.Meshes[m]);

                    fsMesh.Close();
                }
                Application.DoEvents();
            }
        }

        MATD GetMATDforMesh(GenericRCOLResource.ChunkReference reference)
        {
            IRCOLBlock materialRef = GenericRCOLResource.ChunkReference.GetBlock(rcolResource, reference);
            if (materialRef is MATD) return materialRef as MATD;
            if (materialRef is MTST) return GetMATDforMesh((materialRef as MTST).Index);
            return null;
        }

        void Export_MeshGeoStates(StreamWriter w, VRTF vrtf, float uvScale, MLOD mlod, MLOD.Mesh mesh)
        {
            if (mesh.GeometryStates.Count <= 0) return;

            Export_GEOS(w, mesh);

            for (int g = 0; g < mesh.GeometryStates.Count; g++)
            {
                Export_VBUF(w, GenericRCOLResource.ChunkReference.GetBlock(rcolResource, mesh.VertexBufferIndex) as VBUF, vrtf, uvScale, mesh, g);
                Export_IBUF(w, GenericRCOLResource.ChunkReference.GetBlock(rcolResource, mesh.IndexBufferIndex) as IBUF, mesh, vrtf, g);
            }

            w.Flush();
        }

        void Export_VRTF(StreamWriter w, VRTF vrtf, bool isDefault)
        {
            if (isDefault) w.WriteLine(";;-marker: vrtf is default for mesh");

            w.WriteLine(string.Format("vrtf {0} {1}", vrtf.Layouts.Count, vrtf.Stride));

            mpb.Init("Export VRTF...", vrtf.Layouts.Count);
            for (int i = 0; i < vrtf.Layouts.Count; i++)
            {
                var l = vrtf.Layouts[i];
                w.WriteLine(string.Format("{0} {1} {2} {3} {4}", i, (byte)l.Usage, l.UsageIndex, (byte)l.Format, l.Offset));
                mpb.Value++;
            }
            w.Flush();
            mpb.Done();
        }

        void Export_SKIN(StreamWriter w, SKIN skin, MLOD.Mesh mesh)
        {
            if (skin == null) { w.WriteLine("; skin is null"); w.WriteLine("skin 0"); return; }

            if (!mesh.JointReferences.TrueForAll(x => skin.Bones.Exists(y => y.NameHash == x)))
            {
                w.WriteLine("; mesh.JointReferences references unknown bone.");
                w.WriteLine("skin 0");
                return;
            }
            List<uint> seen = new List<uint>();
            if (!mesh.JointReferences.TrueForAll(x => { if (seen.Contains(x)) return false; seen.Add(x); return true; }))
            {
                w.WriteLine("; mesh.JointReferences contains non-unique references.");
                w.WriteLine("skin 0");
                return;
            }
            seen = null;

            w.WriteLine(string.Format("skin {0}", skin.Bones.Count));

            mpb.Init("Export SKIN...", skin.Bones.Count);
            int i = 0;
            //Referenced bones
            foreach (var bone in skin.Bones.FindAll(x => mesh.JointReferences.Contains(x.NameHash)))
            {
                w.WriteLine(string.Format("{0} {1:X8} {2:R} {3:R} {4:R} {5:R} {6:R} {7:R} {8:R} {9:R} {10:R} {11:R} {12:R} {13:R}",
                    i++,
                    bone.NameHash,
                    bone.InverseBindPose.Right.X, bone.InverseBindPose.Right.Y, bone.InverseBindPose.Right.Z, bone.InverseBindPose.Translate.X,
                    bone.InverseBindPose.Up.X, bone.InverseBindPose.Up.Y, bone.InverseBindPose.Up.Z, bone.InverseBindPose.Translate.Y,
                    bone.InverseBindPose.Back.X, bone.InverseBindPose.Back.Y, bone.InverseBindPose.Back.Z, bone.InverseBindPose.Translate.Z));
                mpb.Value++;
            }
            //Unreferenced bones
            foreach (var bone in skin.Bones.FindAll(x => !mesh.JointReferences.Contains(x.NameHash)))
            {
                w.WriteLine(string.Format("{0} {1:X8} {2:R} {3:R} {4:R} {5:R} {6:R} {7:R} {8:R} {9:R} {10:R} {11:R} {12:R} {13:R}",
                    i++,
                    bone.NameHash,
                    bone.InverseBindPose.Right.X, bone.InverseBindPose.Right.Y, bone.InverseBindPose.Right.Z, bone.InverseBindPose.Translate.X,
                    bone.InverseBindPose.Up.X, bone.InverseBindPose.Up.Y, bone.InverseBindPose.Up.Z, bone.InverseBindPose.Translate.Y,
                    bone.InverseBindPose.Back.X, bone.InverseBindPose.Back.Y, bone.InverseBindPose.Back.Z, bone.InverseBindPose.Translate.Z));
                mpb.Value++;
            }
            w.Flush();
            mpb.Done();
        }

        void Export_VBUF(StreamWriter w, VBUF vbuf, VRTF vrtf, float uvScale, MLOD.Mesh mesh)
        {
            if (vbuf == null) { w.WriteLine("; vbuf is null"); w.WriteLine("vbuf 0"); return; }

            s3piwrappers.Vertex[] av = vbuf.GetVertices(mesh, vrtf, uvScale);

            w.WriteLine(string.Format("vbuf {0}", av.Length));
            Export_VBUF_Common(w, av, vrtf);
        }

        bool geosIsContained(MLOD.GeometryState geoState, MLOD.Mesh mesh)
        {
            return geoState.MinVertexIndex + geoState.VertexCount <= mesh.MinVertexIndex + mesh.VertexCount;
        }
        void Export_VBUF(StreamWriter w, VBUF vbuf, VRTF vrtf, float uvScale, MLOD.Mesh mesh, int geoStateIndex)
        {
            if (vbuf == null) { w.WriteLine("; vbuf is null for geoState"); w.WriteLine(string.Format("vbuf {0} 0 0", geoStateIndex)); return; }

            MLOD.GeometryState geoState = mesh.GeometryStates[geoStateIndex];
            s3piwrappers.Vertex[] av = vbuf.GetVertices(mesh, vrtf, geoState, uvScale);

            if (geosIsContained(geoState, mesh)) w.WriteLine("; vbuf is contained within main mesh");
            w.WriteLine(string.Format("vbuf {0} {1} {2}", geoStateIndex, geoState.MinVertexIndex, geoState.VertexCount));
            if (geosIsContained(geoState, mesh)) return;

            Export_VBUF_Common(w, av, vrtf);
        }

        void Export_VBUF_Common(StreamWriter w, s3piwrappers.Vertex[] av, VRTF vrtf)
        {
            mpb.Init("Export VBUF...", av.Length);
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
                    mpb.Value++;
                }
            }

            w.Flush();
            mpb.Done();
        }

        void Export_IBUF(StreamWriter w, IBUF ibuf, MLOD.Mesh mesh)
        {
            if (ibuf == null) { w.WriteLine("; ibuf is null"); w.WriteLine("ibuf 0"); return; }

            w.WriteLine(string.Format("ibuf {0}", mesh.PrimitiveCount));
            Export_IBUF_Common(w, ibuf.GetIndices(mesh), MLOD.IndexCountFromPrimitiveType(mesh.PrimitiveType), mesh.PrimitiveCount);
        }

        void Export_IBUF(StreamWriter w, IBUF ibuf, MLOD.Mesh mesh, VRTF vrtf, int geoStateIndex)
        {
            if (ibuf == null) { w.WriteLine("; ibuf is null for geoState"); w.WriteLine(string.Format("ibuf {0} 0 0", geoStateIndex)); return; }

            int sizePerPrimitive = MLOD.IndexCountFromPrimitiveType(mesh.PrimitiveType);
            MLOD.GeometryState geoState = mesh.GeometryStates[geoStateIndex];
            
            w.WriteLine(string.Format("ibuf {0} {1} {2}", geoStateIndex, geoState.StartIndex, geoState.PrimitiveCount));
            if (geoState.StartIndex + geoState.PrimitiveCount * sizePerPrimitive <= mesh.StartIndex + mesh.PrimitiveCount * sizePerPrimitive) return;

            Export_IBUF_Common(w, ibuf.GetIndices(mesh, vrtf, geoStateIndex), sizePerPrimitive, geoState.PrimitiveCount);
        }

        void Export_IBUF_Common(StreamWriter w, int[] indices, int sizePerPrimitive, int faces)
        {
            mpb.Init("Export IBUF...", faces);
            for (int i = 0; i < faces; i++)
            {
                w.Write(string.Format("{0}", i));
                for (int j = 0; j < sizePerPrimitive; j++)
                    w.Write(string.Format(" {0}", indices[i * sizePerPrimitive + j]));
                w.WriteLine();
                mpb.Value++;
            }

            w.Flush();
            mpb.Done();
        }

        void Export_GEOS(StreamWriter w, MLOD.Mesh mesh)
        {
            w.WriteLine(";");
            w.WriteLine("; Extended format: GeoStates");
            w.WriteLine(";");

            w.WriteLine(string.Format("geos {0}", mesh.GeometryStates.Count));

            for (int g = 0; g < mesh.GeometryStates.Count; g++)
            {
                MLOD.GeometryState s = mesh.GeometryStates[g];
                w.WriteLine(string.Format("{0} {1:X8}", g, s.Name));
            }
        }
    }
}
