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
        public Import()
        {
            InitializeComponent();
            ofdImport.FileName = string.Format("{0}_filebase.s3asc", Program.Filename);
            ofdImport.Filter = Program.Filter;
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
                    StreamReader r = new StreamReader(fsMesh);

                    VRTF vrtf = Import_VRTF(r);
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
                        vrtf = VRTF.CreateDefaultForMesh(mlod.Meshes[m]);

                    SKIN skin = Import_SKIN(r, mlod.Meshes[m]);
                    IResourceKey skinRK = GenericRCOLResource.ChunkReference.GetKey(rcolResource, mlod.Meshes[m].SkinControllerIndex);
                    if (skinRK == null && skin != null)//no RK but imported a SKIN, so generate key
                    {
                        skinRK = new TGIBlock(0, null, vrtfRK) { ResourceType = skin.ResourceType, };
                    }
                    ReplaceChunk(mlod.Meshes[m], "SkinControllerIndex", skinRK, skin);

                    //Save the existing SwizzleInfo chunk reference
                    GenericRCOLResource.ChunkReference swizRef = new GenericRCOLResource.ChunkReference(0, null, 0);
                    VBUF vbufTemp = GenericRCOLResource.ChunkReference.GetBlock(rcolResource, mlod.Meshes[m].VertexBufferIndex) as VBUF;
                    if (vbufTemp != null)
                        swizRef = new GenericRCOLResource.ChunkReference(0, null, vbufTemp.SwizzleInfo);

                    VBUF vbuf = Import_VBUF(r, vrtf, mlod.Meshes[m]);
                    IResourceKey vbufRK = GenericRCOLResource.ChunkReference.GetKey(rcolResource, mlod.Meshes[m].VertexBufferIndex);
                    if (vbufRK == null && vbuf != null)//no RK but imported a VBUF, so generate key
                    {
                        vbufRK = new TGIBlock(0, null, mlodRK) { ResourceType = vbuf.ResourceType, };
                    }
                    Update_SwizzleInfo(swizRef, vrtf, vbuf, m);
                    ReplaceChunk(mlod.Meshes[m], "VertexBufferIndex", vbufRK, vbuf);

                    IBUF ibuf = Import_IBUF(r, mlod.Meshes[m]);
                    IResourceKey ibufRK = GenericRCOLResource.ChunkReference.GetKey(rcolResource, mlod.Meshes[m].IndexBufferIndex);
                    if (ibufRK == null && ibuf != null)//no RK but imported a IBUF, so generate key
                    {
                        ibufRK = new TGIBlock(0, null, mlodRK) { ResourceType = ibuf.ResourceType, };
                    }
                    ReplaceChunk(mlod.Meshes[m], "IndexBufferIndex", ibufRK, ibuf);

                    Import_MeshGeoStates(r, mlod.Meshes[m]);

                    fsMesh.Close();
                    m++;
                }
            }
        }

        //find the chunk, replace the chunk, perhaps create or remove the reference
        void ReplaceChunk(MLOD.Mesh mesh, string field, IResourceKey rk, ARCOLBlock block)
        {
            ARCOLBlock current = GenericRCOLResource.ChunkReference.GetBlock(rcolResource, (GenericRCOLResource.ChunkReference)mesh[field].Value) as ARCOLBlock;
            if (current != null && block != null) // replacing is easy
            {
                if (current.Tag != block.Tag)
                    throw new Exception(string.Format("mesh field {0} is '{1}' but replacement is '{2}'.", field, current.Tag, block.Tag));
                current.Data = block.Data;
                block = current;
            }
            else if (current == null) // adding is okay
            {
                rcolResource.ChunkEntries.Add(rk, block);
                mesh[field] = new TypedValue(typeof(GenericRCOLResource.ChunkReference), GenericRCOLResource.ChunkReference.CreateReference(rcolResource, rk), "X");
            }
            else // deleting is not allowed - we can only null the reference, not remove the chunk
            {
                mesh[field] = new TypedValue(typeof(GenericRCOLResource.ChunkReference), new GenericRCOLResource.ChunkReference(0, null, 0), "X");
            }
        }

        void Update_SwizzleInfo(GenericRCOLResource.ChunkReference swizRef, VRTF vrtf, VBUF vbuf, int m)
        {
            vbuf.SwizzleInfo = new GenericRCOLResource.ChunkReference(0, null, swizRef);
            /*
            VBSI swiz = new VBSI(0, null) { Data = ((ARCOLBlock)GenericRCOLResource.ChunkReference.GetBlock(modlResource, swizRef)).Data };
            /**/
        }

        VRTF Import_VRTF(StreamReader r)
        {
            VRTF vrtf = new VRTF(rcolResource.RequestedApiVersion, null) { Version = 2, Layouts = new VRTF.VertexElementLayoutList(null), };

            string tagLine = ReadLine(r);
            string[] split = tagLine.Split(new char[] { ' ', }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length!=3)
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

            DateTime wait = DateTime.UtcNow.AddSeconds(0.1);
            this.label1.Text = "Import VRTF...";
            this.pb.Value = 0;
            this.pb.Maximum = count;
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
                byte[] values = StringToArray<Byte>.Convert(split, 1);
                if (values.Length != 4)
                    throw new InvalidDataException(string.Format("'vrtf' line {0} has incorrect number of byte values {1}.", l, values.Length));

                vrtf.Layouts.Add((VRTF.ElementFormat)values[2], values[3], (VRTF.ElementUsage)values[0], values[1]);
                if (wait < DateTime.UtcNow) { this.pb.Value = l; wait = DateTime.UtcNow.AddSeconds(0.1); Application.DoEvents(); }
            }

            return vrtf;
        }

        SKIN Import_SKIN(StreamReader r, MLOD.Mesh mesh)
        {
            SKIN skin = new SKIN(rcolResource.RequestedApiVersion, null) { Version = 0, Bones = new SKIN.BoneList(null), };
            mesh.JointReferences = new UIntList(null);

            string tagLine = ReadLine(r);
            string[] split = tagLine.Split(new char[] { ' ', }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length != 2)
                throw new InvalidDataException("Invalid tag line read for 'skin'.");
            if (split[0] != "skin")
                throw new InvalidDataException("Expected line tag 'skin' not found.");
            int count;
            if (!int.TryParse(split[1], out count))
                throw new InvalidDataException("'skin' line has invalid count.");

            DateTime wait = DateTime.UtcNow;
            this.label1.Text = "Import SKIN...";
            this.pb.Value = 0;
            this.pb.Maximum = count;
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
                float[] values = StringToArray<Single>.Convert(split, 2);
                if (values.Length != 12)
                    throw new InvalidDataException(string.Format("'skin' line {0} has incorrect number of float values {1}.", b, values.Length));

                SKIN.Bone bone = new SKIN.Bone(0, null) { NameHash = name, InverseBindPose = new Matrix43(0, null), };
                bone.InverseBindPose.Right.X = values[0]; bone.InverseBindPose.Right.Y = values[1]; bone.InverseBindPose.Right.Z = values[2]; bone.InverseBindPose.Translate.X = values[3];
                bone.InverseBindPose.Up.X = values[4]; bone.InverseBindPose.Up.Y = values[5]; bone.InverseBindPose.Up.Z = values[6]; bone.InverseBindPose.Translate.Y = values[7];
                bone.InverseBindPose.Back.X = values[8]; bone.InverseBindPose.Back.Y = values[9]; bone.InverseBindPose.Back.Z = values[10]; bone.InverseBindPose.Translate.Z = values[11];

                mesh.JointReferences.Add(name);
                skin.Bones.Add(bone);
                if (wait.AddSeconds(0.1) < DateTime.UtcNow) { this.pb.Value = b; wait = DateTime.UtcNow; Application.DoEvents(); }
            }

            return skin;
        }

        VBUF Import_VBUF(StreamReader r, VRTF vrtf, MLOD.Mesh mesh)
        {
            VBUF vbuf = new VBUF(rcolResource.RequestedApiVersion, null) { Version = 0x00000101, Flags = VBUF.FormatFlags.None, SwizzleInfo = new GenericRCOLResource.ChunkReference(0, null, 0), };

            string tagLine = ReadLine(r);
            string[] split = tagLine.Split(new char[] { ' ', }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length != 2)
                throw new InvalidDataException("Invalid tag line read for 'vbuf'.");
            if (split[0] != "vbuf")
                throw new InvalidDataException("Expected line tag 'vbuf' not found.");
            int count;
            if (!int.TryParse(split[1], out count))
                throw new InvalidDataException("'vbuf' line has invalid count.");

            if (vrtf == null) vrtf = mesh.IsShadowCaster ? VRTF.CreateDefaultForSunShadow() : VRTF.CreateDefaultForDropShadow();
            s3piwrappers.Vertex[] vertices = new s3piwrappers.Vertex[count];
            int uvLength = vrtf.Layouts.FindAll(x => x.Usage == VRTF.ElementUsage.UV).Count;

            int line = 0;
            DateTime wait = DateTime.UtcNow;

            this.label1.Text = "Import VBUF...";
            this.pb.Value = 0;
            this.pb.Maximum = count;
            for (int v = 0; v < count; v++)
            {
                s3piwrappers.Vertex vertex = new s3piwrappers.Vertex();
                int nUV = 0;
                vertex.UV = new float[uvLength][];

                foreach (var layout in vrtf.Layouts)
                {
                    split = r.ReadLine().Split(new char[] { ' ', }, StringSplitOptions.RemoveEmptyEntries);
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
                            float[] Position = StringToArray<Single>.Convert(split, 2);
                            if (Position.Length != VRTF.FloatCountFromFormat(layout.Format))
                                throw new InvalidDataException(string.Format("'vbuf' line {0} has incorrect format.", line));
                            vertex.Position = Position;
                            break;
                        case (byte)VRTF.ElementUsage.Normal:
                            float[] Normal = StringToArray<Single>.Convert(split, 2);
                            if (Normal.Length != VRTF.FloatCountFromFormat(layout.Format))
                                throw new InvalidDataException(string.Format("'vbuf' line {0} has incorrect format.", line));
                            vertex.Normal = Normal;
                            break;
                        case (byte)VRTF.ElementUsage.UV:
                            float[] UV = StringToArray<Single>.Convert(split, 2);
                            if (UV.Length != VRTF.FloatCountFromFormat(layout.Format))
                                throw new InvalidDataException(string.Format("'vbuf' line {0} has incorrect format.", line));
                            vertex.UV[nUV++] = UV;
                            break;
                        case (byte)VRTF.ElementUsage.BlendIndex:
                            byte[] BlendIndex = StringToArray<Byte>.Convert(split, 2);
                            if (BlendIndex.Length != VRTF.ByteSizeFromFormat(layout.Format))
                                throw new InvalidDataException(string.Format("'vbuf' line {0} has incorrect format.", line));
                            break;
                        case (byte)VRTF.ElementUsage.BlendWeight:
                            float[] BlendWeight = StringToArray<Single>.Convert(split, 2);
                            if (BlendWeight.Length != VRTF.FloatCountFromFormat(layout.Format))
                                throw new InvalidDataException(string.Format("'vbuf' line {0} has incorrect format.", line));
                            break;
                        case (byte)VRTF.ElementUsage.Tangent:
                            float[] Tangent = StringToArray<Single>.Convert(split, 2);
                            if (Tangent.Length != VRTF.FloatCountFromFormat(layout.Format))
                                throw new InvalidDataException(string.Format("'vbuf' line {0} has incorrect format.", line));
                            break;
                        case (byte)VRTF.ElementUsage.Colour:
                            float[] Colour = StringToArray<Single>.Convert(split, 2);
                            if (Colour.Length != VRTF.FloatCountFromFormat(layout.Format))
                                throw new InvalidDataException(string.Format("'vbuf' line {0} has incorrect format.", line));
                            break;
                    }
                }
                vertices[v] = vertex;
                if (nUV != uvLength)
                    throw new InvalidDataException(string.Format("'vbuf' vertex {0} read {1} UV lines, expected {2}.", v, nUV, uvLength));
                if (wait.AddSeconds(0.1) < DateTime.UtcNow) { this.pb.Value = v; wait = DateTime.UtcNow; Application.DoEvents(); }
            }

            vbuf.SetVertices(vrtf, vertices);

            return vbuf;
        }

        IBUF Import_IBUF(StreamReader r, MLOD.Mesh mesh)
        {
            IBUF ibuf = new IBUF(rcolResource.RequestedApiVersion, null) { Version = 2, Flags = IBUF.FormatFlags.DifferencedIndices, DisplayListUsage = 0, };

            string tagLine = ReadLine(r);
            string[] split = tagLine.Split(new char[] { ' ', }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length != 2)
                throw new InvalidDataException("Invalid tag line read for 'ibuf'.");
            if (split[0] != "ibuf")
                throw new InvalidDataException("Expected line tag 'ibuf' not found.");
            int count;
            if (!int.TryParse(split[1], out count))
                throw new InvalidDataException("'ibuf' line has invalid count.");

            Int32[] indices = new Int32[count * 3];

            DateTime wait = DateTime.UtcNow;
            this.label1.Text = "Import IBUF...";
            this.pb.Value = 0;
            this.pb.Maximum = count;
            for (int i = 0; i < count; i++)
            {
                split = r.ReadLine().Split(new char[] { ' ', }, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length != 4)
                    throw new InvalidDataException(string.Format("'ibuf' line {0} has invalid format.", i));
                int index;
                if (!int.TryParse(split[0], out index))
                    throw new InvalidDataException(string.Format("'ibuf' line {0} has invalid line index.", i));
                if (index != i)
                    throw new InvalidDataException(string.Format("'ibuf' line {0} has incorrect line index value {1}.", i, index));
                Int32[] values = StringToArray<Int32>.Convert(split, 1);
                if (values.Length != 3)
                    throw new InvalidDataException(string.Format("'ibuf' line {0} has incorrect number of Int32 values {1}.", i, values.Length));

                for (int j = 0; j < values.Length; j++)
                    indices[i * 3 + j] = values[j];

                if (wait.AddSeconds(0.1) < DateTime.UtcNow) { this.pb.Value = i; wait = DateTime.UtcNow; Application.DoEvents(); }
            }

            ibuf.Buffer = indices;

            return ibuf;
        }

        void Import_MeshGeoStates(StreamReader r, MLOD.Mesh mesh)
        {
            mesh.GeometryStates = new MLOD.GeometryStateList(null);

            if (r.EndOfStream) return;

            string tagLine = ReadLine(r);
            string[] split = tagLine.Split(new char[] { ' ', }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length < 1 || split[0] != "geos")
                return;
            int count;
            if (!int.TryParse(split[1], out count))
                throw new InvalidDataException("'geos' line has invalid count.");

            DateTime wait = DateTime.UtcNow;
            this.label1.Text = "Import MeshGeoStates...";
            this.pb.Value = 0;
            this.pb.Maximum = count;
            for (int g = 0; g < count; g++)
            {
                split = r.ReadLine().Split(new char[] { ' ', }, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length != 6)
                    throw new InvalidDataException(string.Format("'geos' line {0} has invalid format.", g));
                int index;
                if (!int.TryParse(split[0], out index))
                    throw new InvalidDataException(string.Format("'geos' line {0} has invalid line index.", g));
                if (index != g)
                    throw new InvalidDataException(string.Format("'geos' line {0} has incorrect line index value {1}.", g, index));
                uint name;
                if (!uint.TryParse(split[1], System.Globalization.NumberStyles.HexNumber, null, out name))
                    throw new InvalidDataException(string.Format("'geos' line {0} has invalid name hash.", g));
                Int32[] values = StringToArray<Int32>.Convert(split, 2);
                if (values.Length != 4)
                    throw new InvalidDataException(string.Format("'geos' line {0} has incorrect number of Int32 values {1}.", g, values.Length));

                mesh.GeometryStates.Add(new MLOD.GeometryState(0, null)
                {
                    Name = name,
                    StartIndex = values[0],
                    MinVertexIndex = values[1],
                    VertexCount = values[2],
                    PrimitiveCount = values[3],
                });

                if (wait.AddSeconds(0.1) < DateTime.UtcNow) { this.pb.Value = g; wait = DateTime.UtcNow; Application.DoEvents(); }
            }
        }

        static class StringToArray<T>
        {
            public static T[] Convert(string[] s, int pos)
            {
                System.Reflection.MethodInfo mi = typeof(T).GetMethod("TryParse", new Type[] { typeof(string), typeof(T).MakeByRefType(), });
                if (mi == null)
                    throw new InvalidOperationException("No TryParse for " + typeof(T).Name);

                T[] res = new T[s.Length - pos];
                for (int i = pos; i < s.Length; i++)
                {
                    // because simple types are not reference types, this doesn't work except as the test
                    if (!(bool)mi.Invoke(null, new object[] { s[i], res[i - pos], }))
                        throw new InvalidDataException("Invalid value");
                    // so need to invoke Parse to get the value back
                    res[i - pos] = (T)typeof(T).InvokeMember("Parse", System.Reflection.BindingFlags.InvokeMethod, null, null, new object[] { s[i], });
                }
                return res;
            }
        }

        string ReadLine(StreamReader r) { string l = r.ReadLine(); while (l.StartsWith(";") && !r.EndOfStream) l = r.ReadLine(); return l; }
    }
}
