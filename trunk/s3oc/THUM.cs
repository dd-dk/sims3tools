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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using s3pi.Interfaces;
using s3pi.Filetable;

namespace ObjectCloner
{
    public class THUM
    {
        static Image defaultThumbnail =
            Image.FromFile(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Resources/defaultThumbnail.png"),
            true).GetThumbnailImage(256, 256, () => false, IntPtr.Zero);
        static Dictionary<uint, uint[]> thumTypes;
        static ushort[] thumSizes = new ushort[] { 32, 64, 128, };
        static uint defType = 0x319E4F1D;
        static THUM()
        {
            thumTypes = new Dictionary<uint, uint[]>();
            thumTypes.Add(0x034AEECB, new uint[] { 0x626F60CC, 0x626F60CD, 0x626F60CE, }); //Create-a-Sim Part
            thumTypes.Add(0x319E4F1D, new uint[] { 0x0580A2B4, 0x0580A2B5, 0x0580A2B6, }); //Catalog Object
            thumTypes.Add(0xCF9A4ACE, new uint[] { 0x00000000, 0x00000000, 0x00000000, }); //Modular Resource
            thumTypes.Add(0x0418FE2A, new uint[] { 0x2653E3C8, 0x2653E3C9, 0x2653E3CA, }); //Catalog Fence
            thumTypes.Add(0x049CA4CD, new uint[] { 0x5DE9DBA0, 0x5DE9DBA1, 0x5DE9DBA2, }); //Catalog Stairs
            thumTypes.Add(0x04AC5D93, thumTypes[0x319E4F1D]); //Catalog Proxy Product
            thumTypes.Add(0x04B30669, thumTypes[0x319E4F1D]); //Catalog Terrain Geometry Brush
            thumTypes.Add(0x04C58103, new uint[] { 0x2D4284F0, 0x2D4284F1, 0x2D4284F2, }); //Catalog Railing
            thumTypes.Add(0x04ED4BB2, new uint[] { 0x05B1B524, 0x05B1B525, 0x05B1B526, }); //Catalog Terrain Paint Brush
            thumTypes.Add(0x04F3CC01, new uint[] { 0x05B17698, 0x05B17699, 0x05B1769A, }); //Catalog Fireplace
            thumTypes.Add(0x060B390C, thumTypes[0x319E4F1D]); //Catalog Terrain Water Brush
            thumTypes.Add(0x0A36F07A, thumTypes[0x319E4F1D]); //Catalog Fountain Pool
            thumTypes.Add(0x316C78F2, thumTypes[0x319E4F1D]); //Catalog Foundation
            thumTypes.Add(0x515CA4CD, new uint[] { 0x0589DC44, 0x0589DC45, 0x0589DC46, }); //Catalog Wall/Floor Pattern
            thumTypes.Add(0x9151E6BC, new uint[] { 0x00000000, 0x00000000, 0x00000000, }); //Catalog Wall -- doesn't have any
            thumTypes.Add(0x91EDBD3E, thumTypes[0x319E4F1D]); //Catalog Roof Style
            thumTypes.Add(0xF1EDBD86, thumTypes[0x319E4F1D]); //Catalog Roof Pattern
        }

        public enum THUMSize : int
        {
            small = 0,
            medium,
            large,
            defSize = large,
        }

        public static uint[] PNGTypes = new uint[] { 0x2E75C764, 0x2E75C765, 0x2E75C766, };

        public Image this[ulong instance] { get { return this[instance, THUMSize.defSize, false]; } }
        public Image this[ulong instance, THUMSize size] { get { return this[instance, size, false]; } set { this[instance, size, false] = value; } }
        public Image this[ulong instance, bool isPNGInstance] { get { return this[instance, THUMSize.defSize, isPNGInstance]; } }
        public Image this[ulong instance, THUMSize size, bool isPNGInstance] { get { return this[defType, instance, size, isPNGInstance]; } set { this[defType, instance, size, isPNGInstance] = value; } }
        public Image this[uint type, ulong instance] { get { return this[type, instance, THUMSize.defSize, false]; } }
        public Image this[uint type, ulong instance, THUMSize size] { get { return this[type, instance, size, false]; } set { this[type, instance, size, false] = value; } }
        public Image this[uint type, ulong instance, THUMSize size, bool isPNGInstance]
        {
            get
            {
                SpecificResource item = getRK(type, instance, size, isPNGInstance);
                if (item != null && item.Resource != null)
                    return Image.FromStream(item.Resource.Stream);
                return null;
            }
            set
            {
                SpecificResource item = getItem(isPNGInstance ? FileTable.GameContent : FileTable.Thumbnails, instance, (isPNGInstance ? PNGTypes : thumTypes[type])[(int)size]);
                if (item == null || item.Resource == null)
                    throw new ArgumentException();

                Image thumb;
                thumb = value.GetThumbnailImage(thumSizes[(int)size], thumSizes[(int)size], () => false, System.IntPtr.Zero);
                thumb.Save(item.Resource.Stream, System.Drawing.Imaging.ImageFormat.Png);
                item.Commit();
            }
        }
        static SpecificResource getRK(uint type, ulong instance, THUMSize size, bool isPNGInstance)
        {
            return getItem(isPNGInstance ? FileTable.GameContent : FileTable.Thumbnails, instance, (isPNGInstance ? PNGTypes : thumTypes[type])[(int)size]);
        }
        static SpecificResource getItem(List<PathPackageTuple> ppts, ulong instance, uint type)
        {
            if (ppts == null) return null;
            if (type == 0x00000000) return null;
            foreach (var ppt in ppts)
            {
                List<IResourceIndexEntry> lsr = ppt.Package.FindAll(rie => rie.ResourceType == type && rie.Instance == instance);
                lsr.Sort((x, y) => (x.ResourceGroup & 0x07FFFFFF).CompareTo(y.ResourceGroup & 0x07FFFFFF));
                foreach (var rie in lsr)
                    if (!thumTypes[0x515CA4CD].Contains(type) || (rie.ResourceGroup & 0x00FFFFFF) > 0)
                        return new SpecificResource(ppt, rie);
            }
            return null;
        }

        public static IResourceKey getImageRK(THUMSize size, SpecificResource item)
        {
            if (item.RequestedRK.CType() == CatalogType.ModularResource)
            {
                return RK.NULL;
            }
            else if (item.RequestedRK.CType() == CatalogType.CAS_Part)
            {
                SpecificResource sr = getRK(item.RequestedRK.ResourceType, item.RequestedRK.Instance, size, false);
                return sr == null ? RK.NULL : sr.RequestedRK;
            }
            else
            {
                ulong png = (item.Resource != null) ? (ulong)item.Resource["CommonBlock.PngInstance"].Value : 0;
                SpecificResource sr = getRK(item.RequestedRK.ResourceType, png != 0 ? png : item.RequestedRK.Instance, size, png != 0);
                return sr == null ? RK.NULL : sr.RequestedRK;
            }
        }
        public static SpecificResource getTHUM(THUMSize size, SpecificResource item)
        {
            if (item.RequestedRK.CType() == CatalogType.ModularResource)
            {
                return null;
            }
            else if (item.RequestedRK.CType() == CatalogType.CAS_Part)
            {
                return getRK(item.RequestedRK.ResourceType, item.RequestedRK.Instance, size, false);
            }
            else
            {
                ulong png = (item.Resource != null) ? (ulong)item.Resource["CommonBlock.PngInstance"].Value : 0;
                return getRK(item.RequestedRK.ResourceType, png != 0 ? png : item.RequestedRK.Instance, size, png != 0);
            }
        }

        public static IResourceKey getNewRK(THUMSize size, SpecificResource item)
        {
            if (item.RequestedRK.CType() == CatalogType.ModularResource)
            {
                return RK.NULL;
            }
            else if (item.RequestedRK.CType() == CatalogType.CAS_Part)
            {
                return getNewRK(item.RequestedRK.ResourceType, item.RequestedRK.Instance, size, false);
            }
            else
            {
                ulong png = (item.Resource != null) ? (ulong)item.Resource["CommonBlock.PngInstance"].Value : 0;
                return getNewRK(item.RequestedRK.ResourceType, png != 0 ? png : item.RequestedRK.Instance, size, png != 0);
            }
        }
        static IResourceKey getNewRK(uint type, ulong instance, THUMSize size, bool isPNGInstance)
        {
            return new RK(RK.NULL)
            {
                ResourceType = (isPNGInstance ? PNGTypes : thumTypes[type])[(int)size],
                ResourceGroup = (uint)(type == 0x515CA4CD ? 1 : 0),
                Instance = instance,
            };
        }
        public static Image getLargestThumbOrDefault(SpecificResource item)
        {
            Image img = getImage(THUMSize.large, item);
            if (img != null) return img;
            img = getImage(THUMSize.medium, item);
            if (img != null) return img;
            img = getImage(THUMSize.small, item);
            if (img != null) return img;
            return defaultThumbnail;
        }
        public static Image getImage(THUMSize size, SpecificResource item)
        {
            if (item.RequestedRK.CType() == CatalogType.ModularResource)
            {
                return getImage(size, MainForm.ItemForTGIBlock0(item));
            }
            else if (item.RequestedRK.CType() == CatalogType.CAS_Part)
            {
                return Thumb[item.RequestedRK.ResourceType, item.RequestedRK.Instance, size, false];
            }
            else
            {
                ulong png = (item.Resource != null) ? (ulong)item.Resource["CommonBlock.PngInstance"].Value : 0;
                return Thumb[item.RequestedRK.ResourceType, png != 0 ? png : item.RequestedRK.Instance, size, png != 0];
            }
        }

        static THUM thumb;
        public static THUM Thumb
        {
            get
            {
                if (thumb == null)
                    thumb = new THUM();
                return thumb;
            }
        }

        /*public static IResourceKey makeImage(THUMSize size, SpecificResource item)
        {
            if (item.CType() == CatalogType.ModularResource)
                return RK.NULL;
            else
            {
                IResourceKey rk = getImageRK(size, item);
                if (rk.Equals(RK.NULL))
                {
                    rk = getNewRK(size, item);
                    SpecificResource thum = FileTable.Current.AddResource(rk);
                    defaultThumbnail.GetThumbnailImage(thumSizes[(int)size], thumSizes[(int)size], gtAbort, System.IntPtr.Zero)
                        .Save(thum.Resource.Stream, System.Drawing.Imaging.ImageFormat.Png);
                    thum.Commit();
                }
                return rk;
            }
        }/**/
    }
}