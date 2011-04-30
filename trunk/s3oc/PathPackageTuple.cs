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

namespace ObjectCloner
{
    public class PathPackageTuple
    {
        public string Path { get; private set; }
        public IPackage Package { get; private set; }

        public PathPackageTuple(string path, bool readwrite = false) { Path = path; Package = s3pi.Package.Package.OpenPackage(0, path, readwrite); }

        public SpecificResource AddResource(IResourceKey rk, Stream stream = null)
        {
            IResourceIndexEntry rie = Package.AddResource(rk, stream, true);
            if (rie == null) return null;
            return new SpecificResource(this, rie);
        }

        public SpecificResource Find(Predicate<IResourceIndexEntry> match)
        {
            IResourceIndexEntry rie = Package.Find(match);
            return rie == null ? null : new SpecificResource(this, rie);
        }

        public List<SpecificResource> FindAll(Predicate<IResourceIndexEntry> match)
        {
            return Package.FindAll(match).ConvertAll<SpecificResource>(rie => new SpecificResource(this, rie));
        }
    }
}
