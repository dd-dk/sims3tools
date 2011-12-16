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

namespace ObjectCloner
{
    public enum CatalogType : uint
    {
        CAS_Part = 0x034AEECB,

        CatalogFence = 0x0418FE2A,
        CatalogStairs = 0x049CA4CD,
        CatalogProxyProduct = 0x04AC5D93,
        CatalogTerrainGeometryBrush = 0x04B30669,

        CatalogRailing = 0x04C58103,
        CatalogTerrainPaintBrush = 0x04ED4BB2,
        CatalogFireplace = 0x04F3CC01,
        CatalogTerrainWaterBrush = 0x060B390C,

        CatalogFountainPool = 0x0A36F07A,

        CatalogFoundation = 0x316C78F2,
        CatalogObject = 0x319E4F1D,
        CatalogWallFloorPattern = 0x515CA4CD,
        CatalogWallStyle = 0x9151E6BC,

        CatalogRoofStyle = 0x91EDBD3E,
        ModularResource = 0xCF9A4ACE,
        CatalogRoofPattern = 0xF1EDBD86,
    }
}