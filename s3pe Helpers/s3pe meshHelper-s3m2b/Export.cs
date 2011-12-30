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
    public class Export
    {
        MyProgressBar mpb;
        public Export(MyProgressBar pb) { mpb = pb; }

        public void Export_MLOD(StreamWriter w, GenericRCOLResource rcolResource, MLOD mlod, MLOD.Mesh mesh)
        {
            float[] uvScales = rcolResource.GetUVScales(mesh);

            if (mesh.GeometryStates.Count > 0)
            {
                w.WriteLine(";");
                w.WriteLine("; Extended format: GeoStates follow IBUF");
                w.WriteLine(";");
            }

            VRTF vrtf = GenericRCOLResource.ChunkReference.GetBlock(rcolResource, mesh.VertexFormatIndex) as VRTF;
            bool isDefault = vrtf == null;
            if (isDefault)
            {
                vrtf = VRTF.CreateDefaultForMesh(mesh);
                w.WriteLine(";;-marker: vrtf is default for mesh");
            }
            w.Export_VRTF(mpb, vrtf);

            w.Export_SKIN(mpb, GenericRCOLResource.ChunkReference.GetBlock(rcolResource, mesh.SkinControllerIndex) as SKIN, mesh);
            Export_VBUF_Main(w, GenericRCOLResource.ChunkReference.GetBlock(rcolResource, mesh.VertexBufferIndex) as VBUF, vrtf, uvScales, mesh);
            Export_IBUF_Main(w, GenericRCOLResource.ChunkReference.GetBlock(rcolResource, mesh.IndexBufferIndex) as IBUF, mesh);

            //For backward compatibility, these come after the IBUFs
            Export_MeshGeoStates(w, vrtf, uvScales, mlod, mesh,
                GenericRCOLResource.ChunkReference.GetBlock(rcolResource, mesh.VertexBufferIndex) as VBUF,
                GenericRCOLResource.ChunkReference.GetBlock(rcolResource, mesh.IndexBufferIndex) as IBUF);
        }


        void Export_VBUF_Main(StreamWriter w, VBUF vbuf, VRTF vrtf, float[] uvScales, MLOD.Mesh mesh)
        {
            if (vbuf == null) { w.WriteLine("; vbuf is null"); w.WriteLine("vbuf 0"); return; }

            meshExpImp.ModelBlocks.Vertex[] av = vbuf.GetVertices(mesh, vrtf, uvScales);

            w.WriteLine(string.Format("vbuf {0}", av.Length));
            w.Export_VBUF(mpb, av, vrtf);
        }

        void Export_IBUF_Main(StreamWriter w, IBUF ibuf, MLOD.Mesh mesh)
        {
            if (ibuf == null) { w.WriteLine("; ibuf is null"); w.WriteLine("ibuf 0"); return; }

            w.WriteLine(string.Format("ibuf {0}", mesh.PrimitiveCount));
            w.Export_IBUF(mpb, ibuf.GetIndices(mesh), MLOD.IndexCountFromPrimitiveType(mesh.PrimitiveType), mesh.PrimitiveCount);
        }

        void Export_MeshGeoStates(StreamWriter w, VRTF vrtf, float[] uvScales, MLOD mlod, MLOD.Mesh mesh, VBUF vbuf, IBUF ibuf)
        {
            if (mesh.GeometryStates.Count <= 0) return;

            w.WriteLine(";");
            w.WriteLine("; Extended format: GeoStates");
            w.WriteLine(";");

            w.Export_GEOS(mpb, mesh);

            for (int g = 0; g < mesh.GeometryStates.Count; g++)
            {
                Export_VBUF_Geos(w, vbuf, vrtf, uvScales, mesh, g);
                Export_IBUF_Geos(w, ibuf, mesh, vrtf, g);
            }

            w.Flush();
        }


        bool geosIsContained(MLOD.GeometryState geoState, MLOD.Mesh mesh)
        {
            return geoState.MinVertexIndex + geoState.VertexCount <= mesh.MinVertexIndex + mesh.VertexCount;
        }
        void Export_VBUF_Geos(StreamWriter w, VBUF vbuf, VRTF vrtf, float[] uvScales, MLOD.Mesh mesh, int geoStateIndex)
        {
            if (vbuf == null) { w.WriteLine("; vbuf is null for geoState"); w.WriteLine(string.Format("vbuf {0} 0 0", geoStateIndex)); return; }

            MLOD.GeometryState geoState = mesh.GeometryStates[geoStateIndex];
            meshExpImp.ModelBlocks.Vertex[] av = vbuf.GetVertices(mesh, vrtf, geoState, uvScales);

            if (geosIsContained(geoState, mesh)) w.WriteLine("; vbuf is contained within main mesh");
            w.WriteLine(string.Format("vbuf {0} {1} {2}", geoStateIndex, geoState.MinVertexIndex, geoState.VertexCount));
            if (geosIsContained(geoState, mesh)) return;

            w.Export_VBUF(mpb, av, vrtf);
        }

        void Export_IBUF_Geos(StreamWriter w, IBUF ibuf, MLOD.Mesh mesh, VRTF vrtf, int geoStateIndex)
        {
            if (ibuf == null) { w.WriteLine("; ibuf is null for geoState"); w.WriteLine(string.Format("ibuf {0} 0 0", geoStateIndex)); return; }

            int sizePerPrimitive = MLOD.IndexCountFromPrimitiveType(mesh.PrimitiveType);
            MLOD.GeometryState geoState = mesh.GeometryStates[geoStateIndex];

            w.WriteLine(string.Format("ibuf {0} {1} {2}", geoStateIndex, geoState.StartIndex, geoState.PrimitiveCount));
            if (geoState.StartIndex + geoState.PrimitiveCount * sizePerPrimitive <= mesh.StartIndex + mesh.PrimitiveCount * sizePerPrimitive) return;

            w.Export_IBUF(mpb, ibuf.GetIndices(mesh, vrtf, geoStateIndex), sizePerPrimitive, geoState.PrimitiveCount);
        }
    }
}