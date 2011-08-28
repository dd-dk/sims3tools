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

namespace ObjectCloner.SettingsForms
{
    public class EPsDisabled
    {
        static List<string> ePsDisabled = new List<string>();
        static EPsDisabled() { ePsDisabled = new List<string>(ObjectCloner.Properties.Settings.Default.EPsDisabled.Split(';')); }

        static void Save() { ObjectCloner.Properties.Settings.Default.EPsDisabled = String.Join(";", ePsDisabled.ToArray()); }

        public static bool IsDisabled(string value) { return ePsDisabled.Contains(value); }
        public static void Disable(string value, bool disabled)
        {
            if (IsDisabled(value) == disabled) return;
            if (disabled)
                ePsDisabled.Add(value);
            else
                ePsDisabled.Remove(value);
            Save();
        }
    }
}
