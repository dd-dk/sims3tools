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
using s3pi.Interfaces;
using s3pi.GenericRCOLResource;
using s3piwrappers;

namespace s3ascHelper
{
    public class Import
    {
        MyProgressBar mpb;
        public Import(MyProgressBar pb) { mpb = pb; }


        //--


        public void Import_Mesh(StreamReader r, int m, GenericRCOLResource rcolResource, MLOD mlod, IResourceKey defaultRK, out s3piwrappers.Vertex[] mverts, out List<s3piwrappers.Vertex[]> lverts)
        {
            #region Import VRTF
            bool isDefaultVRTF = false;
            VRTF defaultForMesh = VRTF.CreateDefaultForMesh(mlod.Meshes[m]);

            VRTF vrtf = new VRTF(rcolResource.RequestedApiVersion, null) { Version = 2, Layouts = null, };
            r.Import_VRTF(mpb, vrtf);

            IResourceKey vrtfRK = GenericRCOLResource.ChunkReference.GetKey(rcolResource, mlod.Meshes[m].VertexFormatIndex);
            if (vrtfRK == null)
            {
                vrtfRK = GenericRCOLResource.ChunkReference.GetKey(rcolResource, mlod.Meshes[m].SkinControllerIndex);
                if (vrtfRK == null) vrtfRK = GenericRCOLResource.ChunkReference.GetKey(rcolResource, mlod.Meshes[m].ScaleOffsetIndex);
                if (vrtfRK == null) vrtfRK = new TGIBlock(0, null, 0, 0,
                    System.Security.Cryptography.FNV64.GetHash(DateTime.UtcNow.ToString() + defaultRK.ToString()));
                vrtfRK = new TGIBlock(0, null, vrtfRK) { ResourceType = vrtf.ResourceType, };
            }

            if (vrtf.Equals(defaultForMesh))
            {
                isDefaultVRTF = true;
                mlod.Meshes[m].VertexFormatIndex = new GenericRCOLResource.ChunkReference(0, null, 0);//Clear the reference
            }
            else
                rcolResource.ReplaceChunk(mlod.Meshes[m], "VertexFormatIndex", vrtfRK, vrtf);
            #endregion

            #region Import SKIN
            SKIN skin = new SKIN(rcolResource.RequestedApiVersion, null) { Version = 1, Bones = null, };
            r.Import_SKIN(mpb, skin);

            if (skin.Bones != null)
            {
                IResourceKey skinRK = GenericRCOLResource.ChunkReference.GetKey(rcolResource, mlod.Meshes[m].SkinControllerIndex);
                if (skinRK == null)
                    skinRK = new TGIBlock(0, null, vrtfRK) { ResourceType = skin.ResourceType, };

                rcolResource.ReplaceChunk(mlod.Meshes[m], "SkinControllerIndex", skinRK, skin);
            }
            #endregion

            mverts = Import_VBUF_Main(r, mlod, m, vrtf, isDefaultVRTF);

            #region Import IBUF
            IBUF ibuf = GenericRCOLResource.ChunkReference.GetBlock(rcolResource, mlod.Meshes[m].IndexBufferIndex) as IBUF;
            if (ibuf == null)
                ibuf = new IBUF(rcolResource.RequestedApiVersion, null) { Version = 2, Flags = IBUF.FormatFlags.DifferencedIndices, DisplayListUsage = 0, };
            Import_IBUF_Main(r, mlod, mlod.Meshes[m], ibuf);

            IResourceKey ibufRK = GenericRCOLResource.ChunkReference.GetKey(rcolResource, mlod.Meshes[m].IndexBufferIndex);
            if (ibufRK == null)
                ibufRK = new TGIBlock(0, null, defaultRK) { ResourceType = ibuf.ResourceType, };

            rcolResource.ReplaceChunk(mlod.Meshes[m], "IndexBufferIndex", ibufRK, ibuf);
            #endregion

            // This reads both VBUF Vertex[]s and the ibufs; but the ibufs just go straigh in quite happily
            lverts = Import_MeshGeoStates(r, mlod, mlod.Meshes[m], vrtf, isDefaultVRTF, ibuf);

            mlod.Meshes[m].JointReferences = CreateJointReferences(mlod.Meshes[m], mverts, lverts ?? new List<s3piwrappers.Vertex[]>(), skin);
        }


        s3piwrappers.Vertex[] Import_VBUF_Main(StreamReader r, MLOD mlod, int meshIndex, VRTF vrtf, bool isDefaultVRTF)
        {
            MLOD.Mesh mesh = mlod.Meshes[meshIndex];

            string tagLine = r.ReadTag();
            string[] split = tagLine.Split(new char[] { ' ', }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length != 2)
                throw new InvalidDataException("Invalid tag line read for 'vbuf'.");
            if (split[0] != "vbuf")
                throw new InvalidDataException("Expected line tag 'vbuf' not found.");
            int count;
            if (!int.TryParse(split[1], out count))
                throw new InvalidDataException("'vbuf' line has invalid count.");

            return r.Import_VBUF(mpb, count, vrtf);
        }

        void Import_IBUF_Main(StreamReader r, MLOD mlod, MLOD.Mesh mesh, IBUF ibuf)
        {
            string tagLine = r.ReadTag();
            string[] split = tagLine.Split(new char[] { ' ', }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length != 2)
                throw new InvalidDataException("Invalid tag line read for 'ibuf'.");
            if (split[0] != "ibuf")
                throw new InvalidDataException("Expected line tag 'ibuf' not found.");
            int count;
            if (!int.TryParse(split[1], out count))
                throw new InvalidDataException("'ibuf' line has invalid count.");

            ibuf.SetIndices(mlod, mesh, r.Import_IBUF(mpb, MLOD.IndexCountFromPrimitiveType(mesh.PrimitiveType), count));
        }

        List<s3piwrappers.Vertex[]> Import_MeshGeoStates(StreamReader r, MLOD mlod, MLOD.Mesh mesh, VRTF vrtf, bool isDefaultVRTF, IBUF ibuf)
        {
            MLOD.GeometryStateList oldGeos = new MLOD.GeometryStateList(null, mesh.GeometryStates);
            r.Import_GEOS(mpb, mesh);
            if (mesh.GeometryStates.Count <= 0) return null;

            List<s3piwrappers.Vertex[]> lverts = new List<s3piwrappers.Vertex[]>();
            for (int g = 0; g < mesh.GeometryStates.Count; g++)
            {
                lverts.Add(Import_VBUF_Geos(r, mlod, mesh, g, vrtf, isDefaultVRTF));
                Import_IBUF_Geos(r, mlod, mesh, g, ibuf);
            }
            return lverts;
        }

        UIntList CreateJointReferences(MLOD.Mesh mesh, s3piwrappers.Vertex[] mverts, List<s3piwrappers.Vertex[]> lverts, SKIN skin)
        {
            int maxReference = -1;

            lverts.Insert(0, mverts);
            foreach (var vertices in lverts)
                foreach (var vert in vertices)
                    if (vert.BlendIndices != null)
                        foreach (var reference in vert.BlendIndices)
                            if (reference > maxReference) maxReference = reference;
            lverts.Remove(mverts);

            return maxReference > -1 ? new UIntList(null, skin.Bones.GetRange(0, maxReference + 1).ConvertAll<uint>(x => x.NameHash)) : new UIntList(null);
        }


        s3piwrappers.Vertex[] Import_VBUF_Geos(StreamReader r, MLOD mlod, MLOD.Mesh mesh, int geoStateIndex, VRTF vrtf, bool isDefaultVRTF)
        {
            //w.WriteLine(string.Format("vbuf {0} {1} {2}", geoStateIndex, mesh.GeometryStates[geoStateIndex].MinVertexIndex, mesh.GeometryStates[geoStateIndex].VertexCount));
            string tagLine = r.ReadTag();
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
                return null;
            }

            if (minVertexIndex != mesh.GeometryStates[geoStateIndex].MinVertexIndex)
                throw new InvalidDataException(string.Format("geoState {0} 'vbuf' line has unexpected MinVertexIndex {1}; expected {2}.", geoStateIndex, minVertexIndex, mesh.GeometryStates[geoStateIndex].MinVertexIndex));
            return r.Import_VBUF(mpb, vertexCount, vrtf);
        }

        void Import_IBUF_Geos(StreamReader r, MLOD mlod, MLOD.Mesh mesh, int geoStateIndex, IBUF ibuf)
        {
            //w.WriteLine(string.Format("ibuf {0} {1} {2}", geoStateIndex, mesh.GeometryStates[geoStateIndex].StartIndex, mesh.GeometryStates[geoStateIndex].PrimitiveCount));
            string tagLine = r.ReadTag();
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
            ibuf.SetIndices(mlod, mesh, geoStateIndex, r.Import_IBUF(mpb, MLOD.IndexCountFromPrimitiveType(mesh.PrimitiveType), primitiveCount));
        }


        //--


        public void VertsToVBUFs(GenericRCOLResource rcolResource, MLOD mlod, IResourceKey defaultRK, List<s3piwrappers.Vertex[]> lmverts, List<List<s3piwrappers.Vertex[]>> llverts)
        {
            #region Find all the meshes using the same MATD
            Dictionary<MATD, List<int>> meshGroups = new Dictionary<MATD, List<int>>();
            for (int m = 0; m < mlod.Meshes.Count; m++)
            {
                MATD matd = rcolResource.GetMATDforMesh(mlod.Meshes[m].MaterialIndex);
                if (meshGroups.ContainsKey(matd)) meshGroups[matd].Add(m);
                else meshGroups.Add(matd, new List<int> { m });
            }
            #endregion

            #region Find the MaxUVs for each MATD
            Dictionary<MATD, float[]> matdMaxUVs = new Dictionary<MATD, float[]>();
            foreach (MATD key in meshGroups.Keys)
            {
                List<s3piwrappers.Vertex> verts = new List<s3piwrappers.Vertex>();
                foreach (int m in meshGroups[key])
                {
                    verts.AddRange(lmverts[m]);
                    if (llverts[m] != null) llverts[m].ForEach(l => verts.AddRange(l));
                }
                matdMaxUVs.Add(key, s3piwrappers.UVCompressor.GetMaxUVs(verts));
            }
            #endregion

            #region Set the uvScales for each MATD and cache the VRTFs
            Dictionary<MATD, float[]> matdUVScales = new Dictionary<MATD, float[]>();
            Dictionary<int, VRTF> meshVRTF = new Dictionary<int, VRTF>();
            foreach (MATD key in meshGroups.Keys)
            {
                List<float[]> lUVScales = new List<float[]>();
                foreach (int m in meshGroups[key])
                {
                    meshVRTF.Add(m, GenericRCOLResource.ChunkReference.GetBlock(rcolResource, mlod.Meshes[m].VertexFormatIndex) as VRTF ?? VRTF.CreateDefaultForMesh(mlod.Meshes[m]));
                    lUVScales.Add(s3piwrappers.UVCompressor.GetUVScales(meshVRTF[m], matdMaxUVs[key]));
                }

                for (int i = 1; i < lUVScales.Count; i++)
                    if (!lUVScales[i].Equals<float>(lUVScales[0]))
                        throw new InvalidOperationException("Different UVScales values for one MATD are not allowed.  Ensure correct MATD is referenced by Mesh.");

                matdUVScales.Add(key, lUVScales[0]);
                if (lUVScales[0] != null) SetUVScales(key, lUVScales[0]);
            }
            #endregion

            #region Create the VBUF for each mesh
            foreach (MATD key in meshGroups.Keys)
            {
                foreach (int m in meshGroups[key])
                {
                    VBUF vbuf = GenericRCOLResource.ChunkReference.GetBlock(rcolResource, mlod.Meshes[m].VertexBufferIndex) as VBUF;
                    if (vbuf == null)
                        vbuf = new VBUF(rcolResource.RequestedApiVersion, null) { Version = 0x00000101, Flags = VBUF.FormatFlags.None, SwizzleInfo = new GenericRCOLResource.ChunkReference(0, null, 0), };
                    vbuf.SetVertices(mlod, m, meshVRTF[m], lmverts[m], matdUVScales[key]);
                    if (llverts[m] != null) for (int i = 0; i < llverts[m].Count; i++) vbuf.SetVertices(mlod, mlod.Meshes[m], i, meshVRTF[m], llverts[m][i], matdUVScales[key]);

                    IResourceKey vbufRK = GenericRCOLResource.ChunkReference.GetKey(rcolResource, mlod.Meshes[m].VertexBufferIndex);
                    if (vbufRK == null)//means we created the VBUF: create a RK and add it
                        vbufRK = new TGIBlock(0, null, defaultRK) { ResourceType = vbuf.ResourceType, };

                    rcolResource.ReplaceChunk(mlod.Meshes[m], "VertexBufferIndex", vbufRK, vbuf);
                }
            }
            #endregion
        }

        void SetUVScales(MATD matd, float[] uvScales)
        {
            MATD.ShaderDataList sdList = (matd.Version < 0x0103 ? matd.Mtrl.SData : matd.Mtnf.SData);
            MATD.ShaderData data;
            switch (uvScales.Length)
            {
                case 1: data = new MATD.ElementFloat(0, null, MATD.FieldType.UVScales, uvScales[0]); break;
                case 2: data = new MATD.ElementFloat2(0, null, MATD.FieldType.UVScales, uvScales[0], uvScales[1]); break;
                case 3: data = new MATD.ElementFloat3(0, null, MATD.FieldType.UVScales, uvScales[0], uvScales[1], uvScales[2]); break;
                case 4: data = new MATD.ElementFloat4(0, null, MATD.FieldType.UVScales, uvScales[0], uvScales[1], uvScales[2], uvScales[4]); break;
                default:
                    throw new ArgumentException(String.Format("Found {0} UVScales; expected 1 to 4.", uvScales.Length));
            }
            int i = sdList.FindIndex(x => x.Field == MATD.FieldType.UVScales);
            if (i >= 0) sdList[i] = data;
            else sdList.Add(data);
        }
    }
}