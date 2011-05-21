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
using s3pi.Interfaces;
using s3pi.GenericRCOLResource;
using s3piwrappers;

namespace s3ascHelper
{
    public static class MODL_MLOD_Extensions
    {
        public static GenericRCOLResource.ChunkReference GetMLODChunkRefforMODL(this GenericRCOLResource rcolResource)
        {
            var lodEntry = (rcolResource.ChunkEntries[0].RCOLBlock as MODL).Entries
                .Find(l => l.ModelLodIndex.RefType == GenericRCOLResource.ReferenceType.Public
                    || l.ModelLodIndex.RefType == GenericRCOLResource.ReferenceType.Private);
            return lodEntry == null ? null : lodEntry.ModelLodIndex;
        }

        //find the chunk, replace the chunk, perhaps create or remove the reference
        public static void ReplaceChunk(this GenericRCOLResource rcolResource, MLOD.Mesh mesh, string field, IResourceKey rk, ARCOLBlock block)
        {
            ARCOLBlock current = GenericRCOLResource.ChunkReference.GetBlock(rcolResource, (GenericRCOLResource.ChunkReference)mesh[field].Value) as ARCOLBlock;
            if (block != null)
            {
                if (current != null) // replacing is easy
                {
                    if (current.Tag != block.Tag)
                        throw new Exception(string.Format("mesh field {0} is '{1}' but replacement is '{2}'.", field, current.Tag, block.Tag));
                    // ...not entirely sure if these are required...
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

        public static float[] GetUVScales(this GenericRCOLResource rcolResource, MLOD.Mesh mesh)
        {
            MATD matd = GetMATDforMesh(rcolResource, mesh.MaterialIndex);
            if (matd != null)
            {
                MATD.ShaderData data = (matd.Version < 0x0103 ? matd.Mtrl.SData : matd.Mtnf.SData).Find(x => x.Field == MATD.FieldType.UVScales);
                if (data != null)
                {
                    if (data is MATD.ElementFloat3) { MATD.ElementFloat3 e = data as MATD.ElementFloat3; return new float[] { e.Data0, e.Data1, e.Data2, }; }
                    else throw new InvalidOperationException(String.Format("Found UVScales of type '{0}'; expected 'ElementFloat3'.", data.GetType().Name));
                }
            }
            return new float[] { 1f / 32767f, };
        }

        public static MATD GetMATDforMesh(this GenericRCOLResource rcolResource, GenericRCOLResource.ChunkReference reference)
        {
            IRCOLBlock materialRef = GenericRCOLResource.ChunkReference.GetBlock(rcolResource, reference);
            if (materialRef is MATD) return materialRef as MATD;
            if (materialRef is MTST) return GetMATDforMesh(rcolResource, (materialRef as MTST).Index);
            return null;
        }
    }
}
