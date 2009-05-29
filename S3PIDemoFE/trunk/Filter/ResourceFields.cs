/***************************************************************************
 *  Copyright (C) 2009 by Peter L Jones                                    *
 *  peter@users.sf.net                                                     *
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
using s3pi.Interfaces;

namespace S3PIDemoFE.Filter
{
    public partial class ResourceFields : Component
    {
        public ResourceFields() { }

        static List<string> fields = null;
        static ResourceFields()
        {
            fields = new List<string>();
            foreach (string s in AApiVersionedFields.GetContentFields(0,typeof(AResourceIndexEntry)))
                if (!s.Contains("Stream"))
                    fields.Add(s);
        }

        [Browsable(true)]
        [Description("The list of known fields on a Resource object")]
        public IList<string> Fields
        {
            get { return fields; }
            //set { }
        }
    }
}
