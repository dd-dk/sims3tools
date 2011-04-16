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
using System.Windows.Forms;
using s3pi.Interfaces;
using s3pi.GenericRCOLResource;
using s3piwrappers;

namespace s3ascHelper
{
    public partial class Import : Form, s3pi.Helpers.IRunHelper
    {
        MyProgressBar mpb;
        public Import()
        {
            InitializeComponent();
            mpb = new MyProgressBar(label1, pb);
            ofdImport.FileName = string.Format("{0}_filebase.s3asc", Program.Filename);
        }

        GenericRCOLResource rcolResource;
        public Import(Stream s)
            : this()
        {
            rcolResource = new GenericRCOLResource(0, s);
            Application.DoEvents();
        }

        byte[] result = null;
        public byte[] Result { get { return result; } }

        private void Import_Shown(object sender, EventArgs e)
        {
            try
            {
                Application.DoEvents();

                DialogResult dr = ofdImport.ShowDialog();
                if (dr != DialogResult.OK)
                {
                    Environment.ExitCode = 1;
                    return;
                }

                string folder = Path.GetDirectoryName(ofdImport.FileName);
                string filebase = Path.GetFileNameWithoutExtension(ofdImport.FileName).Replace("_filebase", "");

                if (!File.Exists(Path.Combine(folder, string.Format("{0}_filebase.s3asc", filebase))))
                {
                    CopyableMessageBox.Show("File name must end \"_filebase.s3asc\"", "Base file not found", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Error);
                    Environment.ExitCode = 1;
                    return;
                }

                try
                {
                    // to avoid destroying the internal references it's important to preserve the order of the chunks,
                    // otherwise, all the references need identifying and updating

                    if (rcolResource.ChunkEntries[0].TGIBlock.ResourceType == 0x01661233)
                    {
                        var modl = rcolResource.ChunkEntries[0].RCOLBlock as MODL;
                        foreach (var lodEntry in modl.Entries)
                        {
                            if (lodEntry.ModelLodIndex.RefType != GenericRCOLResource.ReferenceType.Public
                                && lodEntry.ModelLodIndex.RefType != GenericRCOLResource.ReferenceType.Private) continue;

                            var mlod = GenericRCOLResource.ChunkReference.GetBlock(rcolResource, lodEntry.ModelLodIndex) as MLOD;
                            Import_MLOD(mlod, folder, filebase, GenericRCOLResource.ChunkReference.GetKey(rcolResource, lodEntry.ModelLodIndex));
                        }
                    }
                    else if (rcolResource.ChunkEntries[0].TGIBlock.ResourceType == 0x01D10F34)
                    {
                        var mlod = rcolResource.ChunkEntries[0].RCOLBlock as MLOD;
                        Import_MLOD(mlod, folder, filebase, rcolResource.ChunkEntries[0].TGIBlock);
                    }
                }
                catch (Exception ex)
                {
                    CopyableMessageBox.IssueException(ex, "Error");
                    throw ex;
                }

                Environment.ExitCode = 0;

                result = (byte[])rcolResource.AsBytes.Clone();
                Application.DoEvents();
            }
            finally { this.Close(); }
        }

        void Import_MLOD(MLOD mlod, string folder, string filebase, IResourceKey mlodRK)
        {
            int m = 0;
            while (true)
            {
                string fnMesh = Path.Combine(folder, string.Format("{0}_group{1:X2}.s3ascg", filebase, m));
                if (!File.Exists(fnMesh)) break;

                using (FileStream fsMesh = new FileStream(fnMesh, FileMode.Open, FileAccess.Read))
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

                    StreamReader r = new StreamReader(fsMesh);

                    #region Import VRTF
                    bool isDefaultVRTF = false;
                    VRTF defaultForMesh = VRTF.CreateDefaultForMesh(mlod.Meshes[m]);
                    VRTF vrtf = Import_VRTF(r);
                    if (vrtf.Equals(defaultForMesh))
                    {
                        isDefaultVRTF = true;
                        vrtf = null;
                    }
                    IResourceKey vrtfRK = GenericRCOLResource.ChunkReference.GetKey(rcolResource, mlod.Meshes[m].VertexFormatIndex);
                    if (vrtfRK == null && vrtf != null)
                    {
                        vrtfRK = GenericRCOLResource.ChunkReference.GetKey(rcolResource, mlod.Meshes[m].SkinControllerIndex);
                        if (vrtfRK == null) vrtfRK = GenericRCOLResource.ChunkReference.GetKey(rcolResource, mlod.Meshes[m].ScaleOffsetIndex);
                        if (vrtfRK == null) vrtfRK = new TGIBlock(0, null, 0, 0,
                            System.Security.Cryptography.FNV64.GetHash(DateTime.UtcNow.ToString() + fnMesh));
                        vrtfRK = new TGIBlock(0, null, vrtfRK) { ResourceType = vrtf.ResourceType, };
                    }
                    ReplaceChunk(mlod.Meshes[m], "VertexFormatIndex", vrtfRK, vrtf);
                    if (vrtf == null)//need a default VRTF
                        vrtf = defaultForMesh;
                    #endregion

                    #region Import SKIN
                    SKIN skin = Import_SKIN(r, mlod.Meshes[m]);
                    IResourceKey skinRK = GenericRCOLResource.ChunkReference.GetKey(rcolResource, mlod.Meshes[m].SkinControllerIndex);
                    if (skinRK == null && skin != null)//no RK but imported a SKIN, so generate key
                    {
                        skinRK = new TGIBlock(0, null, vrtfRK) { ResourceType = skin.ResourceType, };
                    }
                    ReplaceChunk(mlod.Meshes[m], "SkinControllerIndex", skinRK, skin);
                    #endregion

                    #region Import VBUF
                    //Save the existing SwizzleInfo chunk reference
                    VBUF vbuf = GenericRCOLResource.ChunkReference.GetBlock(rcolResource, mlod.Meshes[m].VertexBufferIndex) as VBUF;
                    if (vbuf == null)
                        vbuf = new VBUF(rcolResource.RequestedApiVersion, null) { Version = 0x00000101, Flags = VBUF.FormatFlags.None, SwizzleInfo = new GenericRCOLResource.ChunkReference(0, null, 0), };
                    Import_VBUF(r, mlod, m, vrtf, isDefaultVRTF, uvScale, vbuf);
                    IResourceKey vbufRK = GenericRCOLResource.ChunkReference.GetKey(rcolResource, mlod.Meshes[m].VertexBufferIndex);
                    if (vbufRK == null)//means we created the VBUF: create a RK and add it
                    {
                        vbufRK = new TGIBlock(0, null, mlodRK) { ResourceType = vbuf.ResourceType, };
                        ReplaceChunk(mlod.Meshes[m], "VertexBufferIndex", vbufRK, vbuf);
                    }
                    #endregion

                    #region Import IBUF
                    IBUF ibuf = GenericRCOLResource.ChunkReference.GetBlock(rcolResource, mlod.Meshes[m].IndexBufferIndex) as IBUF;
                    if (ibuf == null)
                        ibuf = new IBUF(rcolResource.RequestedApiVersion, null) { Version = 2, Flags = IBUF.FormatFlags.DifferencedIndices, DisplayListUsage = 0, };
                    Import_IBUF(r, mlod, mlod.Meshes[m], ibuf);
                    IResourceKey ibufRK = GenericRCOLResource.ChunkReference.GetKey(rcolResource, mlod.Meshes[m].IndexBufferIndex);
                    if (ibufRK == null)//means we created the IBUF: create a RK and add it
                    {
                        ibufRK = new TGIBlock(0, null, mlodRK) { ResourceType = ibuf.ResourceType, };
                        ReplaceChunk(mlod.Meshes[m], "IndexBufferIndex", ibufRK, ibuf);
                    }
                    #endregion

                    Import_MeshGeoStates(r, mlod, mlod.Meshes[m], vrtf, isDefaultVRTF, uvScale, vbuf, ibuf);

                    fsMesh.Close();
                    m++;
                }
            }
        }

        MATD GetMATDforMesh(GenericRCOLResource.ChunkReference reference)
        {
            IRCOLBlock materialRef = GenericRCOLResource.ChunkReference.GetBlock(rcolResource, reference);
            if (materialRef is MATD) return materialRef as MATD;
            if (materialRef is MTST) return GetMATDforMesh((materialRef as MTST).Index);
            return null;
        }

        void Import_MeshGeoStates(StreamReader r, MLOD mlod, MLOD.Mesh mesh, VRTF vrtf, bool isDefaultVRTF, float uvScale, VBUF vbuf, IBUF ibuf)
        {
            MLOD.GeometryStateList oldGeos = new MLOD.GeometryStateList(null, mesh.GeometryStates);
            Import_GEOS(r, mesh);
            if (mesh.GeometryStates.Count <= 0) return;

            for (int g = 0; g < mesh.GeometryStates.Count; g++)
            {
                Import_VBUF(r, mlod, mesh, g, vrtf, isDefaultVRTF, uvScale, vbuf);
                Import_IBUF(r, mlod, mesh, g, vrtf, ibuf);
            }
        }

        //find the chunk, replace the chunk, perhaps create or remove the reference
        void ReplaceChunk(MLOD.Mesh mesh, string field, IResourceKey rk, ARCOLBlock block)
        {
            ARCOLBlock current = GenericRCOLResource.ChunkReference.GetBlock(rcolResource, (GenericRCOLResource.ChunkReference)mesh[field].Value) as ARCOLBlock;
            if (block != null)
            {
                if (current != null) // replacing is easy
                {
                    if (current.Tag != block.Tag)
                        throw new Exception(string.Format("mesh field {0} is '{1}' but replacement is '{2}'.", field, current.Tag, block.Tag));
                    current.Data = block.Data;
                    block = current;
                }
                else // adding is okay
                {
                    rcolResource.ChunkEntries.Add(rk, block);
                    mesh[field] = new TypedValue(typeof(GenericRCOLResource.ChunkReference), GenericRCOLResource.ChunkReference.CreateReference(rcolResource, rk), "X");
                }
            }
            else // deleting is not allowed - we can only null the reference, not remove the chunk
            {
                mesh[field] = new TypedValue(typeof(GenericRCOLResource.ChunkReference), new GenericRCOLResource.ChunkReference(0, null, 0), "X");
            }
        }

        VRTF Import_VRTF(StreamReader r)
        {
            VRTF vrtf = new VRTF(rcolResource.RequestedApiVersion, null) { Version = 2, Layouts = new VRTF.VertexElementLayoutList(null), };

            string tagLine = ReadLine(r);
            string[] split;
            split = tagLine.Split(new char[] { ' ', }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length != 3)
                throw new InvalidDataException("Invalid tag line read for 'vrtf'.");
            if (split[0] != "vrtf")
                throw new InvalidDataException("Expected line tag 'vrtf' not found.");
            int count;
            if (!int.TryParse(split[1], out count))
                throw new InvalidDataException("'vrtf' line has invalid count.");
            int stride;
            if (!int.TryParse(split[2], out stride))
                throw new InvalidDataException("'vrtf' line has invalid stride.");

            if (count == 0) return null;//permissable for there to be no VRTF

            vrtf.Stride = stride;

            mpb.Init("Import VRTF...", count);
            for (int l = 0; l < count; l++)
            {
                split = r.ReadLine().Split(new char[] { ' ', }, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length != 5)
                    throw new InvalidDataException(string.Format("'vrtf' line {0} has invalid format.", l));
                int index;
                if (!int.TryParse(split[0], out index))
                    throw new InvalidDataException(string.Format("'vrtf' line {0} has invalid line index.", l));
                if (index != l)
                    throw new InvalidDataException(string.Format("'vrtf' line {0} has incorrect line index value {1}.", l, index));
                byte[] values = split.Cast<byte>(1);
                if (values.Length != 4)
                    throw new InvalidDataException(string.Format("'vrtf' line {0} has incorrect number of byte values {1}.", l, values.Length));

                vrtf.Layouts.Add((VRTF.ElementFormat)values[2], values[3], (VRTF.ElementUsage)values[0], values[1]);
                mpb.Value++;
            }
            mpb.Done();

            return vrtf;
        }

        SKIN Import_SKIN(StreamReader r, MLOD.Mesh mesh)
        {
            SKIN skin = new SKIN(rcolResource.RequestedApiVersion, null) { Version = 1, Bones = new SKIN.BoneList(null), };

            string tagLine = ReadLine(r);
            string[] split = tagLine.Split(new char[] { ' ', }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length != 2)
                throw new InvalidDataException("Invalid tag line read for 'skin'.");
            if (split[0] != "skin")
                throw new InvalidDataException("Expected line tag 'skin' not found.");
            int count;
            if (!int.TryParse(split[1], out count))
                throw new InvalidDataException("'skin' line has invalid count.");

            if (count == 0) return null;

            mpb.Init("Import SKIN...", count);
            for (int b = 0; b < count; b++)
            {
                split = r.ReadLine().Split(new char[] { ' ', }, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length != 14)
                    throw new InvalidDataException(string.Format("'skin' line {0} has invalid format.", b));
                int index;
                if (!int.TryParse(split[0], out index))
                    throw new InvalidDataException(string.Format("'skin' line {0} has invalid line index.", b));
                if (index != b)
                    throw new InvalidDataException(string.Format("'vrtf' line {0} has incorrect line index value {1}.", b, index));
                uint name;
                if (!uint.TryParse(split[1], System.Globalization.NumberStyles.HexNumber, null, out name))
                    throw new InvalidDataException(string.Format("'skin' line {0} has invalid name hash.", b));
                if (name != mesh.JointReferences[b])
                    throw new InvalidDataException(string.Format("'skin' line {0} name 0x{1:X8} does not match mesh joint reference 0x{2:X8}.", b, name, mesh.JointReferences[b]));
                float[] values = split.Cast<Single>(2);
                if (values.Length != 12)
                    throw new InvalidDataException(string.Format("'skin' line {0} has incorrect number of float values {1}.", b, values.Length));

                SKIN.Bone bone = new SKIN.Bone(0, null) { NameHash = name, InverseBindPose = new Matrix43(0, null), };
                bone.InverseBindPose.Right.X = values[0]; bone.InverseBindPose.Right.Y = values[1]; bone.InverseBindPose.Right.Z = values[2]; bone.InverseBindPose.Translate.X = values[3];
                bone.InverseBindPose.Up.X = values[4]; bone.InverseBindPose.Up.Y = values[5]; bone.InverseBindPose.Up.Z = values[6]; bone.InverseBindPose.Translate.Y = values[7];
                bone.InverseBindPose.Back.X = values[8]; bone.InverseBindPose.Back.Y = values[9]; bone.InverseBindPose.Back.Z = values[10]; bone.InverseBindPose.Translate.Z = values[11];

                skin.Bones.Add(bone);
                mpb.Value++;
            }
            mpb.Done();

            return skin;
        }

        void Import_VBUF(StreamReader r, MLOD mlod, int meshIndex, VRTF vrtf, bool isDefaultVRTF, float uvScale, VBUF vbuf)
        {
            MLOD.Mesh mesh = mlod.Meshes[meshIndex];

            string tagLine = ReadLine(r);
            string[] split = tagLine.Split(new char[] { ' ', }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length != 2)
                throw new InvalidDataException("Invalid tag line read for 'vbuf'.");
            if (split[0] != "vbuf")
                throw new InvalidDataException("Expected line tag 'vbuf' not found.");
            int count;
            if (!int.TryParse(split[1], out count))
                throw new InvalidDataException("'vbuf' line has invalid count.");

            s3piwrappers.Vertex[] vertices = Import_VBUF_Common(r, mesh, count, vrtf, isDefaultVRTF);
            mpb.Init("Import VBUF...", 0);
            vbuf.SetVertices(mlod, meshIndex, vrtf, vertices, uvScale);
            mpb.Done();
        }

        void Import_VBUF(StreamReader r, MLOD mlod, MLOD.Mesh mesh, int geoStateIndex, VRTF vrtf, bool isDefaultVRTF, float uvScale, VBUF vbuf)
        {
            //w.WriteLine(string.Format("vbuf {0} {1} {2}", geoStateIndex, mesh.GeometryStates[geoStateIndex].MinVertexIndex, mesh.GeometryStates[geoStateIndex].VertexCount));
            string tagLine = ReadLine(r);
            string[] split = tagLine.Split(new char[] { ' ', }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length != 4)
                throw new InvalidDataException(string.Format("Invalid tag line read for geoState {0} 'vbuf'.", geoStateIndex));
            if (split[0] != "vbuf")
                throw new InvalidDataException("Expected line tag 'vbuf' not found.");
            int lineIndex;
            if (!int.TryParse(split[1], out lineIndex))
                throw new InvalidDataException(string.Format("geoState {0} 'vbuf' line has invalid geoStateIndex.", geoStateIndex));
            if (lineIndex != geoStateIndex)
                throw new InvalidDataException(string.Format("geoState {0} 'vbuf' line has incorrect geoStateIndex value {1}.", geoStateIndex, lineIndex));
            int minVertexIndex;
            if (!int.TryParse(split[2], out minVertexIndex))
                throw new InvalidDataException(string.Format("geoState {0} 'vbuf' line has invalid MinVertexIndex.", geoStateIndex));
            int vertexCount;
            if (!int.TryParse(split[3], out vertexCount))
                throw new InvalidDataException(string.Format("geoState {0} 'vbuf' line has invalid VertexCount.", geoStateIndex));

            if (minVertexIndex + vertexCount <= mesh.MinVertexIndex + mesh.VertexCount)
            {
                mesh.GeometryStates[geoStateIndex].MinVertexIndex = minVertexIndex;
                mesh.GeometryStates[geoStateIndex].VertexCount = vertexCount;
                return;
            }

            if (minVertexIndex != mesh.GeometryStates[geoStateIndex].MinVertexIndex)
                throw new InvalidDataException(string.Format("geoState {0} 'vbuf' line has unexpected MinVertexIndex {1}; expected {2}.", geoStateIndex, minVertexIndex, mesh.GeometryStates[geoStateIndex].MinVertexIndex));
            s3piwrappers.Vertex[] vertices = Import_VBUF_Common(r, mesh, vertexCount, vrtf, isDefaultVRTF);
            mpb.Init("Import VBUF...", 0);
            vbuf.SetVertices(mlod, mesh, geoStateIndex, vrtf, vertices, uvScale);
            mpb.Done();
        }

        s3piwrappers.Vertex[] Import_VBUF_Common(StreamReader r, MLOD.Mesh mesh, int count, VRTF vrtf, bool isDefaultVRTF)
        {
            s3piwrappers.Vertex[] vertices = new s3piwrappers.Vertex[count];
            int uvLength = vrtf.Layouts.FindAll(x => x.Usage == VRTF.ElementUsage.UV).Count;

            int line = 0;

            mpb.Init("Import VBUF...", count);
            for (int v = 0; v < count; v++)
            {
                s3piwrappers.Vertex vertex = new s3piwrappers.Vertex();
                int nUV = 0;
                vertex.UV = new float[uvLength][];

                foreach (var layout in vrtf.Layouts)
                {
                    string[] split = r.ReadLine().Split(new char[] { ' ', }, StringSplitOptions.RemoveEmptyEntries);
                    if (split.Length < 2)
                        throw new InvalidDataException(string.Format("'vbuf' line {0} has invalid format.", line));
                    int index;
                    if (!int.TryParse(split[0], out index))
                        throw new InvalidDataException(string.Format("'vbuf' line {0} has invalid line index.", line));
                    if (index != v)
                        throw new InvalidDataException(string.Format("'vbuf' line {0} has incorrect line index value {1}.", line, index));
                    byte usage;
                    if (!byte.TryParse(split[1], out usage))
                        throw new InvalidDataException(string.Format("'vbuf' line {0} has invalid line index.", v));
                    if (usage != (byte)layout.Usage)
                        throw new InvalidDataException(string.Format("'vbuf' line {0} has incorrect line Usage value {1}.", line, usage));

                    switch (usage)
                    {
                        case (byte)VRTF.ElementUsage.Position:
                            vertex.Position = GetFloats(layout.Format, isDefaultVRTF, line, split.Cast<Single>(2));
                            break;
                        case (byte)VRTF.ElementUsage.Normal:
                            vertex.Normal = GetFloats(layout.Format, isDefaultVRTF, line, split.Cast<Single>(2));
                            break;
                        case (byte)VRTF.ElementUsage.UV:
                            vertex.UV[nUV++] = GetFloats(layout.Format, isDefaultVRTF, line, split.Cast<Single>(2));
                            break;
                        case (byte)VRTF.ElementUsage.BlendIndex:
                            byte[] BlendIndices = split.Cast<byte>(2);
                            if (!(BlendIndices.Length == VRTF.ByteSizeFromFormat(layout.Format) || (isDefaultVRTF && BlendIndices.Length + 1 == VRTF.ByteSizeFromFormat(layout.Format))))
                                throw new InvalidDataException(string.Format("'vbuf' line {0} has incorrect format.", line));
                            vertex.BlendIndices = BlendIndices;
                            break;
                        case (byte)VRTF.ElementUsage.BlendWeight:
                            vertex.BlendWeights = GetFloats(layout.Format, isDefaultVRTF, line, split.Cast<Single>(2));
                            break;
                        case (byte)VRTF.ElementUsage.Tangent:
                            vertex.Tangents = GetFloats(layout.Format, isDefaultVRTF, line, split.Cast<Single>(2));
                            break;
                        case (byte)VRTF.ElementUsage.Colour:
                            vertex.Color = GetFloats(layout.Format, isDefaultVRTF, line, split.Cast<Single>(2));
                            break;
                    }
                    line++;
                }
                vertices[v] = vertex;
                if (nUV != uvLength)
                    throw new InvalidDataException(string.Format("'vbuf' vertex {0} read {1} UV lines, expected {2}.", v, nUV, uvLength));
                mpb.Value++;
            }
            mpb.Done();

            return vertices;
        }

        void Import_IBUF(StreamReader r, MLOD mlod, MLOD.Mesh mesh, IBUF ibuf)
        {
            string tagLine = ReadLine(r);
            string[] split = tagLine.Split(new char[] { ' ', }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length != 2)
                throw new InvalidDataException("Invalid tag line read for 'ibuf'.");
            if (split[0] != "ibuf")
                throw new InvalidDataException("Expected line tag 'ibuf' not found.");
            int count;
            if (!int.TryParse(split[1], out count))
                throw new InvalidDataException("'ibuf' line has invalid count.");

            Int32[] indices = Import_IBUF_Common(r, MLOD.IndexCountFromPrimitiveType(mesh.PrimitiveType), count);
            ibuf.SetIndices(mlod, mesh, indices);
        }

        void Import_IBUF(StreamReader r, MLOD mlod, MLOD.Mesh mesh, int geoStateIndex, VRTF vrtf, IBUF ibuf)
        {
            //w.WriteLine(string.Format("ibuf {0} {1} {2}", geoStateIndex, mesh.GeometryStates[geoStateIndex].StartIndex, mesh.GeometryStates[geoStateIndex].PrimitiveCount));
            string tagLine = ReadLine(r);
            string[] split = tagLine.Split(new char[] { ' ', }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length != 4)
                throw new InvalidDataException("Invalid tag line read for 'ibuf'.");
            if (split[0] != "ibuf")
                throw new InvalidDataException("Expected line tag 'ibuf' not found.");
            int lineIndex;
            if (!int.TryParse(split[1], out lineIndex))
                throw new InvalidDataException(string.Format("geoState {0} 'ibuf' line has invalid geoStateIndex.", geoStateIndex));
            if (lineIndex != geoStateIndex)
                throw new InvalidDataException(string.Format("geoState {0} 'ibuf' line has incorrect geoStateIndex value {1}.", geoStateIndex, lineIndex));
            int startIndex;
            if (!int.TryParse(split[2], out startIndex))
                throw new InvalidDataException(string.Format("geoState {0} 'ibuf' line has invalid StartIndex.", geoStateIndex));
            int primitiveCount;
            if (!int.TryParse(split[3], out primitiveCount))
                throw new InvalidDataException(string.Format("geoState {0} 'ibuf' line has invalid PrimitiveCount.", geoStateIndex));

            int sizePerPrimitive = MLOD.IndexCountFromPrimitiveType(mesh.PrimitiveType);
            if (startIndex + primitiveCount * sizePerPrimitive <= mesh.StartIndex + mesh.PrimitiveCount * sizePerPrimitive)
            {
                mesh.GeometryStates[geoStateIndex].StartIndex = startIndex;
                mesh.GeometryStates[geoStateIndex].PrimitiveCount = primitiveCount;
                return;
            }

            if (startIndex != mesh.GeometryStates[geoStateIndex].StartIndex)
                throw new InvalidDataException(string.Format("geoState {0} 'ibuf' line has unexpected StartIndex {1}; expected {2}.", geoStateIndex, startIndex, mesh.GeometryStates[geoStateIndex].StartIndex));
            Int32[] indices = Import_IBUF_Common(r, MLOD.IndexCountFromPrimitiveType(mesh.PrimitiveType), primitiveCount);
            ibuf.SetIndices(mlod, mesh, geoStateIndex, indices);
        }

        Int32[] Import_IBUF_Common(StreamReader r, int sizePerPrimitive, int count)
        {
            Int32[] indices = new Int32[count * sizePerPrimitive];

            mpb.Init("Import IBUF...", count);
            for (int i = 0; i < count; i++)
            {
                string[] split = r.ReadLine().Split(new char[] { ' ', }, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length != 4)
                    throw new InvalidDataException(string.Format("'ibuf' line {0} has invalid format.", i));
                int index;
                if (!int.TryParse(split[0], out index))
                    throw new InvalidDataException(string.Format("'ibuf' line {0} has invalid line index.", i));
                if (index != i)
                    throw new InvalidDataException(string.Format("'ibuf' line {0} has incorrect line index value {1}.", i, index));
                Int32[] values = split.Cast<int>(1);
                if (values.Length != sizePerPrimitive)
                    throw new InvalidDataException(string.Format("'ibuf' line {0} has incorrect number of Int32 values {1}.", i, values.Length));

                for (int j = 0; j < values.Length; j++)
                    indices[i * sizePerPrimitive + j] = values[j];

                mpb.Value++;
            }
            mpb.Done();

            return indices;
        }

        void Import_GEOS(StreamReader r, MLOD.Mesh mesh)
        {
            if (r.EndOfStream)
            {
                mesh.GeometryStates = new MLOD.GeometryStateList(null);
                return;
            }

            string tagLine = ReadLine(r);
            string[] split = tagLine.Split(new char[] { ' ', }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length != 2 || split[0] != "geos")
                return;
            int count;
            if (!int.TryParse(split[1], out count))
                throw new InvalidDataException("'geos' line has invalid count.");

            DateTime wait = DateTime.UtcNow;
            this.label1.Text = "Import MeshGeoStates...";
            this.pb.Value = 0;
            this.pb.Maximum = count;

            int sizePerPrimitive = MLOD.IndexCountFromPrimitiveType(mesh.PrimitiveType);
            int lastMinVertexIndex = mesh.MinVertexIndex;
            int lastVertexCount = 0;
            int lastStartIndex = mesh.PrimitiveCount * MLOD.IndexCountFromPrimitiveType(mesh.PrimitiveType);
            int lastPrimitiveCount = 0;
            for (int g = 0; g < count; g++)
            {
                split = r.ReadLine().Split(new char[] { ' ', }, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length != 2)
                    throw new InvalidDataException(string.Format("'geos' line {0} has invalid format.", g));
                int index;
                if (!int.TryParse(split[0], out index))
                    throw new InvalidDataException(string.Format("'geos' line {0} has invalid line index.", g));
                if (index != g)
                    throw new InvalidDataException(string.Format("'geos' line {0} has incorrect line index value {1}.", g, index));
                uint name;
                if (!uint.TryParse(split[1], System.Globalization.NumberStyles.HexNumber, null, out name))
                    throw new InvalidDataException(string.Format("'geos' line {0} has invalid name hash.", g));
                /*
                int minVertexIndex;
                if (!int.TryParse(split[2], out minVertexIndex))
                    throw new InvalidDataException(string.Format("'geos' line {0} has invalid MinVertexIndex.", g));
                int vertexCount;
                if (!int.TryParse(split[3], out vertexCount))
                    throw new InvalidDataException(string.Format("'geos' line {0} has invalid VertexCount.", g));
                /**/

                if (g < mesh.GeometryStates.Count)
                {
                    mesh.GeometryStates[g].Name = name;
                    lastMinVertexIndex = mesh.GeometryStates[g].MinVertexIndex;
                    lastVertexCount = mesh.GeometryStates[g].VertexCount;
                    lastStartIndex = mesh.GeometryStates[g].StartIndex;
                    lastPrimitiveCount = mesh.GeometryStates[g].PrimitiveCount;
                }
                else
                    mesh.GeometryStates.Add(new MLOD.GeometryState(0, null)
                    {
                        Name = name,
                        MinVertexIndex = lastMinVertexIndex + lastVertexCount,
                        VertexCount = 0,
                        StartIndex = lastStartIndex + lastPrimitiveCount * sizePerPrimitive,
                        PrimitiveCount = 0,
                    });

                if (wait.AddSeconds(0.1) < DateTime.UtcNow) { this.pb.Value = g; wait = DateTime.UtcNow; Application.DoEvents(); }
            }
        }

        static float[] GetFloats(VRTF.ElementFormat format, bool isDefaultVRTF, int line, float[] source)
        {
            if (!CheckFloatCount(format, isDefaultVRTF, source.Length))
                throw new InvalidDataException(string.Format("'vbuf' line {0} has incorrect format.", line));
            float[] res = new float[VRTF.FloatCountFromFormat(format)];
            Array.Copy(source, res, Math.Min(source.Length, res.Length));
            return res;
        }
        static bool CheckFloatCount(VRTF.ElementFormat format, bool isDefaultVRTF, int length)
        {
            return true || length == VRTF.FloatCountFromFormat(format) ||
                (isDefaultVRTF && length + 1 == VRTF.FloatCountFromFormat(format));
        }

        string ReadLine(StreamReader r) { string l = r.ReadLine(); while (l.StartsWith(";") /*&& !l.StartsWith(";;-marker: ")/**/ && !r.EndOfStream) l = r.ReadLine(); return l; }
    }
}
