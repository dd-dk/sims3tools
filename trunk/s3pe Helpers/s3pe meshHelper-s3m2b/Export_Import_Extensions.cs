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
using System.IO;
using System.Linq;
using s3pi.Interfaces;
using s3pi.GenericRCOLResource;
using meshExpImp.ModelBlocks;

namespace meshExpImp.Helper
{
    // make this extend StreamWriter
    public static class Export_Extensions
    {
        public static void Export_VRTF(this StreamWriter w, MyProgressBar mpb, VRTF vrtf)
        {
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

        public static void Export_SKIN(this StreamWriter w, MyProgressBar mpb, SKIN skin, MLOD.Mesh mesh)
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
            //pre-20120601: foreach (var bone in skin.Bones.FindAll(x => mesh.JointReferences.Contains(x.NameHash)))
            foreach (var bone in ((IEnumerable<uint>)mesh.JointReferences).Select(hash => skin.Bones.Find(x => x.NameHash == hash)))
            {
                w.WriteLine(string.Format(
                    //"{0} {1:X8} {2:R} {3:R} {4:R} {5:R} {6:R} {7:R} {8:R} {9:R} {10:R} {11:R} {12:R} {13:R}",
                    "{0} {1:X8} {2:F6} {3:F6} {4:F6} {5:F6} {6:F6} {7:F6} {8:F6} {9:F6} {10:F6} {11:F6} {12:F6} {13:F6}",
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
                w.WriteLine(string.Format(
                    //"{0} {1:X8} {2:R} {3:R} {4:R} {5:R} {6:R} {7:R} {8:R} {9:R} {10:R} {11:R} {12:R} {13:R}",
                    "{0} {1:X8} {2:F6} {3:F6} {4:F6} {5:F6} {6:F6} {7:F6} {8:F6} {9:F6} {10:F6} {11:F6} {12:F6} {13:F6}",
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

        public static void Export_VBUF(this StreamWriter w, MyProgressBar mpb, meshExpImp.ModelBlocks.Vertex[] av, VRTF vrtf)
        {
            mpb.Init("Export VBUF...", av.Length);
            for (int i = 0; i < av.Length; i++)
            {
                meshExpImp.ModelBlocks.Vertex v = av[i];
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

        public static void Export_IBUF(this StreamWriter w, MyProgressBar mpb, int[] indices, int sizePerPrimitive, int faces)
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

        public static void Export_GEOS(this StreamWriter w, MyProgressBar mpb, MLOD.Mesh mesh)
        {
            w.WriteLine(string.Format("geos {0}", mesh.GeometryStates.Count));

            for (int g = 0; g < mesh.GeometryStates.Count; g++)
                w.WriteLine(string.Format("{0} {1:X8}", g, mesh.GeometryStates[g].Name));
        }
    }

    // make this extend StreamReader
    public static class Import_Extensions
    {
        public static void Import_VRTF(this StreamReader r, MyProgressBar mpb, VRTF vrtf)
        {
            string tagLine = r.ReadTag();
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

            if (count == 0) return;
            vrtf.Layouts = new VRTF.VertexElementLayoutList(null);

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
                byte[] values = split.ConvertAll<byte>(1);
                if (values.Length != 4)
                    throw new InvalidDataException(string.Format("'vrtf' line {0} has incorrect number of byte values {1}.", l, values.Length));

                vrtf.Layouts.Add((VRTF.ElementFormat)values[2], values[3], (VRTF.ElementUsage)values[0], values[1]);
                mpb.Value++;
            }
            mpb.Done();
        }

        public static void Import_SKIN(this StreamReader r, MyProgressBar mpb, SKIN skin)
        {
            string tagLine = r.ReadTag();
            string[] split = tagLine.Split(new char[] { ' ', }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length != 2)
                throw new InvalidDataException("Invalid tag line read for 'skin'.");
            if (split[0] != "skin")
                throw new InvalidDataException("Expected line tag 'skin' not found.");
            int count;
            if (!int.TryParse(split[1], out count))
                throw new InvalidDataException("'skin' line has invalid count.");

            if (count == 0) return;
            skin.Bones = new SKIN.BoneList(null);

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
                //if (name != mesh.JointReferences[b])
                //throw new InvalidDataException(string.Format("'skin' line {0} name 0x{1:X8} does not match mesh joint reference 0x{2:X8}.", b, name, mesh.JointReferences[b]));
                float[] values = split.ConvertAll<Single>(2);
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
        }

        public static meshExpImp.ModelBlocks.Vertex[] Import_VBUF(this StreamReader r, MyProgressBar mpb, int count, VRTF vrtf)
        {
            meshExpImp.ModelBlocks.Vertex[] vertices = new meshExpImp.ModelBlocks.Vertex[count];
            int uvLength = vrtf.Layouts.FindAll(x => x.Usage == VRTF.ElementUsage.UV).Count;

            int line = 0;

            mpb.Init("Import VBUF...", count);
            for (int v = 0; v < count; v++)
            {
                meshExpImp.ModelBlocks.Vertex vertex = new meshExpImp.ModelBlocks.Vertex();
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
                            vertex.Position = GetFloats(layout.Format, line, split.ConvertAll<Single>(2));
                            break;
                        case (byte)VRTF.ElementUsage.Normal:
                            vertex.Normal = GetFloats(layout.Format, line, split.ConvertAll<Single>(2));
                            break;
                        case (byte)VRTF.ElementUsage.UV:
                            vertex.UV[nUV++] = GetFloats(layout.Format, line, split.ConvertAll<Single>(2));
                            break;
                        case (byte)VRTF.ElementUsage.BlendIndex:
                            byte[] BlendIndices = split.ConvertAll<byte>(2);
                            if (!(BlendIndices.Length == VRTF.ByteSizeFromFormat(layout.Format) || BlendIndices.Length + 1 == VRTF.ByteSizeFromFormat(layout.Format)))
                                throw new InvalidDataException(string.Format("'vbuf' line {0} has incorrect format.", line));
                            vertex.BlendIndices = BlendIndices;
                            break;
                        case (byte)VRTF.ElementUsage.BlendWeight:
                            vertex.BlendWeights = GetFloats(layout.Format, line, split.ConvertAll<Single>(2));
                            break;
                        case (byte)VRTF.ElementUsage.Tangent:
                            vertex.Tangents = GetFloats(layout.Format, line, split.ConvertAll<Single>(2));
                            break;
                        case (byte)VRTF.ElementUsage.Colour:
                            vertex.Color = GetFloats(layout.Format, line, split.ConvertAll<Single>(2));
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

        public static Int32[] Import_IBUF(this StreamReader r, MyProgressBar mpb, int sizePerPrimitive, int count)
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
                Int32[] values = split.ConvertAll<int>(1);
                if (values.Length != sizePerPrimitive)
                    throw new InvalidDataException(string.Format("'ibuf' line {0} has incorrect number of Int32 values {1}.", i, values.Length));

                for (int j = 0; j < values.Length; j++)
                    indices[i * sizePerPrimitive + j] = values[j];

                mpb.Value++;
            }
            mpb.Done();

            return indices;
        }

        public static void Import_GEOS(this StreamReader r, MyProgressBar mpb, MLOD.Mesh mesh)
        {
            if (r.EndOfStream)
            {
                mesh.GeometryStates = new MLOD.GeometryStateList(null);
                return;
            }

            string tagLine = r.ReadTag();
            string[] split = tagLine.Split(new char[] { ' ', }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length != 2 || split[0] != "geos")
                return;
            int count;
            if (!int.TryParse(split[1], out count))
                throw new InvalidDataException("'geos' line has invalid count.");

            mpb.Init("Import MeshGeoStates...", count);

            int sizePerPrimitive = IBUF.IndexCountFromPrimitiveType(mesh.PrimitiveType);
            int lastMinVertexIndex = mesh.MinVertexIndex;
            int lastVertexCount = 0;
            int lastStartIndex = mesh.PrimitiveCount * IBUF.IndexCountFromPrimitiveType(mesh.PrimitiveType);
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

                mpb.Value = g;
            }
            mpb.Done();
        }


        static float[] GetFloats(VRTF.ElementFormat format, int line, float[] source)
        {
            float[] res = new float[VRTF.FloatCountFromFormat(format)];
            Array.Copy(source, res, Math.Min(source.Length, res.Length));
            return res;
        }

        public static string ReadTag(this StreamReader r) { string l = r.ReadLine(); while (l.StartsWith(";") /*&& !l.StartsWith(";;-marker: ")/**/ && !r.EndOfStream) l = r.ReadLine(); return l; }
    }
}