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

        public void Import_Mesh(StreamReader r, int m, GenericRCOLResource rcolResource, MLOD mlod, IResourceKey defaultRK)
        {
            #region Import VRTF
            bool isDefaultVRTF = false;
            VRTF defaultForMesh = VRTF.CreateDefaultForMesh(mlod.Meshes[m]);

            VRTF vrtf = new VRTF(rcolResource.RequestedApiVersion, null) { Version = 2, Layouts = null, };
            r.Import_VRTF(mpb, vrtf);
            if (vrtf.Equals(defaultForMesh))
            {
                isDefaultVRTF = true;
                vrtf = null;
            }

            IResourceKey vrtfRK = GenericRCOLResource.ChunkReference.GetKey(rcolResource, mlod.Meshes[m].VertexFormatIndex);
            if (vrtfRK == null)
            {
                vrtfRK = GenericRCOLResource.ChunkReference.GetKey(rcolResource, mlod.Meshes[m].SkinControllerIndex);
                if (vrtfRK == null) vrtfRK = GenericRCOLResource.ChunkReference.GetKey(rcolResource, mlod.Meshes[m].ScaleOffsetIndex);
                if (vrtfRK == null) vrtfRK = new TGIBlock(0, null, 0, 0,
                    System.Security.Cryptography.FNV64.GetHash(DateTime.UtcNow.ToString() + defaultRK.ToString()));
                vrtfRK = new TGIBlock(0, null, vrtfRK) { ResourceType = vrtf.ResourceType, };
            }

            rcolResource.ReplaceChunk(mlod.Meshes[m], "VertexFormatIndex", vrtfRK, vrtf);

            if (vrtf == null)//need a default VRTF
                vrtf = defaultForMesh;
            #endregion

            #region Import SKIN
            SKIN skin = new SKIN(rcolResource.RequestedApiVersion, null) { Version = 1, Bones = null, };
            r.Import_SKIN(mpb, skin);

            IResourceKey skinRK = GenericRCOLResource.ChunkReference.GetKey(rcolResource, mlod.Meshes[m].SkinControllerIndex);
            if (skinRK == null)
                skinRK = new TGIBlock(0, null, vrtfRK) { ResourceType = skin.ResourceType, };

            rcolResource.ReplaceChunk(mlod.Meshes[m], "SkinControllerIndex", skinRK, skin);
            #endregion

            s3piwrappers.Vertex[] mverts = Import_VBUF_Main(r, mlod, m, vrtf, isDefaultVRTF);

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
            List<s3piwrappers.Vertex[]> lverts = Import_MeshGeoStates(r, mlod, mlod.Meshes[m], vrtf, isDefaultVRTF, ibuf);

            #region Complete VBUF
            //Save the existing SwizzleInfo chunk reference
            VBUF vbuf = GenericRCOLResource.ChunkReference.GetBlock(rcolResource, mlod.Meshes[m].VertexBufferIndex) as VBUF;
            if (vbuf == null)
                vbuf = new VBUF(rcolResource.RequestedApiVersion, null) { Version = 0x00000101, Flags = VBUF.FormatFlags.None, SwizzleInfo = new GenericRCOLResource.ChunkReference(0, null, 0), };

            List<s3piwrappers.Vertex> lvert = new List<s3piwrappers.Vertex>(mverts);
            if (lverts == null) lverts = new List<s3piwrappers.Vertex[]>();
            lverts.ForEach(x => { if (x != null) lvert.AddRange(x); });
            float[] uvScales = s3piwrappers.UVCompressor.GetUVScales(vrtf, lvert);

            vbuf.SetVertices(mlod, m, vrtf, mverts, uvScales);
            for (int i = 0; i < lverts.Count; i++) vbuf.SetVertices(mlod, mlod.Meshes[m], i, vrtf, lverts[i], uvScales);

            IResourceKey vbufRK = GenericRCOLResource.ChunkReference.GetKey(rcolResource, mlod.Meshes[m].VertexBufferIndex);
            if (vbufRK == null)//means we created the VBUF: create a RK and add it
                vbufRK = new TGIBlock(0, null, defaultRK) { ResourceType = vbuf.ResourceType, };

            rcolResource.ReplaceChunk(mlod.Meshes[m], "VertexBufferIndex", vbufRK, vbuf);
            #endregion

            rcolResource.SetUVScales(mlod.Meshes[m], uvScales);

            mlod.Meshes[m].JointReferences = CreateJointReferences(mlod.Meshes[m], mverts, lverts, skin);
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
    }
}