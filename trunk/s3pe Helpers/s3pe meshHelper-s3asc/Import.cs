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
using meshExpImp.ModelBlocks;

namespace meshExpImp.Helper
{
    public class Import
    {
        MyProgressBar mpb;
        public Import(MyProgressBar pb) { mpb = pb; }


        //--


        public void Import_Mesh(StreamReader r, MLOD.Mesh mesh, GenericRCOLResource rcolResource, MLOD mlod, IResourceKey defaultRK, out meshExpImp.ModelBlocks.Vertex[] mverts)
        {
            #region Import VRTF
            bool isDefaultVRTF = false;
            VRTF defaultForMesh = VRTF.CreateDefaultForMesh(mesh);

            VRTF vrtf = new VRTF(rcolResource.RequestedApiVersion, null) { Version = 2, Layouts = null, };
            r.Import_VRTF(mpb, vrtf);

            IResourceKey vrtfRK = GenericRCOLResource.ChunkReference.GetKey(rcolResource, mesh.VertexFormatIndex);
            if (vrtfRK == null)
            {
                vrtfRK = GenericRCOLResource.ChunkReference.GetKey(rcolResource, mesh.SkinControllerIndex);
                if (vrtfRK == null) vrtfRK = GenericRCOLResource.ChunkReference.GetKey(rcolResource, mesh.ScaleOffsetIndex);
                if (vrtfRK == null) vrtfRK = new TGIBlock(0, null, 0, 0,
                    System.Security.Cryptography.FNV64.GetHash(DateTime.UtcNow.ToString() + defaultRK.ToString()));
                vrtfRK = new TGIBlock(0, null, vrtfRK) { ResourceType = vrtf.ResourceType, };
            }

            if (vrtf.Equals(defaultForMesh))
            {
                isDefaultVRTF = true;
                mesh.VertexFormatIndex = new GenericRCOLResource.ChunkReference(0, null, 0);//Clear the reference
            }
            else
                rcolResource.ReplaceChunk(mesh, "VertexFormatIndex", vrtfRK, vrtf);
            #endregion

            #region Import SKIN
            // we need to read the data in the file...
            SKIN skin = new SKIN(rcolResource.RequestedApiVersion, null) { Version = 1, Bones = null, };
            r.Import_SKIN(mpb, skin);

            // However, we do *NOT* want to update the RCOL with what we read - we are not replacing the object skeleton here
#if UNDEF
            if (skin.Bones != null)
            {
                IResourceKey skinRK = GenericRCOLResource.ChunkReference.GetKey(rcolResource, mesh.SkinControllerIndex);
                if (skinRK == null)
                    skinRK = new TGIBlock(0, null, vrtfRK) { ResourceType = skin.ResourceType, };

                rcolResource.ReplaceChunk(mesh, "SkinControllerIndex", skinRK, skin);
            }
#endif
            #endregion

            mverts = Import_VBUF_Main(r, mlod, mesh, vrtf, isDefaultVRTF);

            #region Import IBUF
            IBUF ibuf = GenericRCOLResource.ChunkReference.GetBlock(rcolResource, mesh.IndexBufferIndex) as IBUF;
            if (ibuf == null)
                ibuf = new IBUF(rcolResource.RequestedApiVersion, null) { Version = 2, Flags = IBUF.FormatFlags.DifferencedIndices, DisplayListUsage = 0, };
            Import_IBUF_Main(r, mlod, mesh, ibuf);

            IResourceKey ibufRK = GenericRCOLResource.ChunkReference.GetKey(rcolResource, mesh.IndexBufferIndex);
            if (ibufRK == null)
                ibufRK = new TGIBlock(0, null, defaultRK) { ResourceType = ibuf.ResourceType, };

            rcolResource.ReplaceChunk(mesh, "IndexBufferIndex", ibufRK, ibuf);
            #endregion

            #region Update the JointReferences
            UIntList joints = CreateJointReferences(mesh, mverts, skin);

            List<uint> added = new List<uint>(joints);
            List<uint> removed = new List<uint>();
            foreach (var j in mesh.JointReferences)
            {
                if (joints.Contains(j)) added.Remove(j);
                else removed.Add(j);
            }

            // Remove root
            removed.Remove(0xCD68F001);

            if (added.Count != 0)
            {
                mesh.JointReferences.AddRange(added);

                System.Windows.Forms.CopyableMessageBox.Show(String.Format("Mesh: 0x{0:X8}\nJointReferences with newly assigned (via BlendIndex) vertex: {1}\n({2})",
                    mesh.Name,
                    added.Count,
                    String.Join(", ", added.ConvertAll<string>(a => "0x" + a.ToString("X8")).ToArray())),
                    "Warning", System.Windows.Forms.CopyableMessageBoxButtons.OK, System.Windows.Forms.CopyableMessageBoxIcon.Warning);
            }

            if (removed.Count != 0)
            {
#if UNDEF
                // http://dino.drealm.info/den/denforum/index.php?topic=394.msg3876#msg3876
                removed.ForEach(j => mesh.JointReferences[mesh.JointReferences.IndexOf(j)] = 0);
#endif
                // However, OM felt more comfortable if there was some indication something a little odd was going on.
                System.Windows.Forms.CopyableMessageBox.Show(String.Format("Mesh: 0x{0:X8}\nJointReferences with no assigned (via BlendIndex) vertex: {1}\n({2})",
                    mesh.Name,
                    removed.Count,
                    String.Join(", ", removed.ConvertAll<string>(a => "0x" + a.ToString("X8")).ToArray())),
                    "Warning", System.Windows.Forms.CopyableMessageBoxButtons.OK, System.Windows.Forms.CopyableMessageBoxIcon.Warning);
            }
            #endregion
        }


        ModelBlocks.Vertex[] Import_VBUF_Main(StreamReader r, MLOD mlod, MLOD.Mesh mesh, VRTF vrtf, bool isDefaultVRTF)
        {
            string tagLine = r.ReadTag();
            string[] split = tagLine.Split(new char[] { ' ', }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length != 2)
                throw new InvalidDataException("Invalid tag line read for 'vbuf'.");
            if (split[0] != "vbuf")
                throw new InvalidDataException("Expected line tag 'vbuf' not found.");
            int count;
            if (!int.TryParse(split[1], out count))
                throw new InvalidDataException("'vbuf' line has invalid count.");

            //Wes's MilkShape plug-in sends back the first line in all subsequent lines of a dropShadow.
            return r.Import_VBUF(mpb, count, vrtf, (mesh.Flags & MeshFlags.ShadowCaster) == 0);
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

            ibuf.SetIndices(mlod, mesh, r.Import_IBUF(mpb, IBUF.IndexCountFromPrimitiveType(mesh.PrimitiveType), count));
        }

        UIntList CreateJointReferences(MLOD.Mesh mesh, ModelBlocks.Vertex[] mverts, SKIN skin)
        {
            if (skin == null || skin.Bones == null) return new UIntList(null);

            int maxReference = -1;

            foreach (var vert in mverts)
                if (vert.BlendIndices != null)
                    foreach (var reference in vert.BlendIndices)
                        if ((sbyte)reference > maxReference) maxReference = reference;

            return maxReference > -1 ? new UIntList(null, skin.Bones.GetRange(0, maxReference + 1).ConvertAll<uint>(x => x.NameHash)) : new UIntList(null);
        }


        //--


        public struct offScale
        {
            public int meshGroup { get; set; }
            public int geoState { get; set; }
            public int vertex { get; set; }
            public int nUV { get; set; }
            public float actual { get; set; }
            public float max { get; set; }
            public override string ToString()
            {
                return String.Format("MeshGroup: {0}" + (geoState >= 0 ? " (Geo: {5})" : "") + "; Vertex[{1}].UV[{2}]: {3}; Max: {4} (from UVScales)", meshGroup, vertex, nUV, actual, max, geoState);
            }
        }
        public List<offScale> VertsToVBUFs(GenericRCOLResource rcolResource, MLOD mlod, IResourceKey defaultRK, List<meshExpImp.ModelBlocks.Vertex[]> lmverts, bool updateBBs, bool updateUVs)
        {
            // List of UV elements going off scale
            List<offScale> offScales = new List<offScale>();

            // Find everything for each mesh group
            Dictionary<GenericRCOLResource.ChunkReference, List<int>> meshGroups = new Dictionary<GenericRCOLResource.ChunkReference, List<int>>();
            Dictionary<int, VRTF> meshVRTF = new Dictionary<int, VRTF>();
            Dictionary<int, float[]> meshUVScales = new Dictionary<int, float[]>();
            for (int m = 0; m < mlod.Meshes.Count; m++)
            {
                if (meshGroups.ContainsKey(mlod.Meshes[m].MaterialIndex)) meshGroups[mlod.Meshes[m].MaterialIndex].Add(m);
                else meshGroups.Add(mlod.Meshes[m].MaterialIndex, new List<int> { m });
                VRTF vrtf = GenericRCOLResource.ChunkReference.GetBlock(rcolResource, mlod.Meshes[m].VertexFormatIndex) as VRTF ?? VRTF.CreateDefaultForMesh(mlod.Meshes[m]);
                meshVRTF.Add(m, vrtf);

                if (updateUVs)
                    rcolResource.FixUVScales(mlod.Meshes[m]);
                meshUVScales.Add(m, rcolResource.GetUVScales(mlod.Meshes[m]));
            }

            // Update the VBUFs for each mesh group and set the mesh bounds whilst we're here
            foreach (var key in meshGroups.Keys)
            {
                foreach (int m in meshGroups[key])
                {
                    VBUF vbuf = GenericRCOLResource.ChunkReference.GetBlock(rcolResource, mlod.Meshes[m].VertexBufferIndex) as VBUF;
                    if (vbuf == null)
                        vbuf = new VBUF(rcolResource.RequestedApiVersion, null) { Version = 0x00000101, Flags = VBUF.FormatFlags.None, SwizzleInfo = new GenericRCOLResource.ChunkReference(0, null, 0), };

                    offScales.AddRange(getOffScales(m, -1, lmverts[m], meshUVScales[m]));
                    vbuf.SetVertices(mlod, m, meshVRTF[m], lmverts[m], meshUVScales[m]);

                    IResourceKey vbufRK = GenericRCOLResource.ChunkReference.GetKey(rcolResource, mlod.Meshes[m].VertexBufferIndex);
                    if (vbufRK == null)//means we created the VBUF: create a RK and add it
                        vbufRK = new TGIBlock(0, null, defaultRK) { ResourceType = vbuf.ResourceType, };

                    rcolResource.ReplaceChunk(mlod.Meshes[m], "VertexBufferIndex", vbufRK, vbuf);

                    if (updateBBs)
                        mlod.Meshes[m].Bounds = vbuf.GetBoundingBox(mlod.Meshes[m], meshVRTF[m]);
                }
            }

            return offScales;
        }

        private IEnumerable<offScale> getOffScales(int meshGroup, int geoState, meshExpImp.ModelBlocks.Vertex[] verts, float[] uvScales)
        {
            for (int v = 0; v < verts.Length; v++)
                foreach (float[] uvs in verts[v].UV)
                    for (int u = 0; u < uvs.Length; u++)
                    {
                        float max = (u < uvScales.Length && uvScales[u] != 0 ? uvScales[u] : uvScales[0]) * short.MaxValue;
                        if (uvs[u] > max)
                            yield return new offScale() { meshGroup = meshGroup, geoState = geoState, vertex = v, nUV = u, actual = uvs[u], max = max };
                    }
        }
    }
}
