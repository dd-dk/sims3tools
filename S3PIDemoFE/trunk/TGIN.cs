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
using System.IO;
using s3pi.Extensions;
using s3pi.Interfaces;

namespace S3PIDemoFE
{
    public struct TGIN
    {
        public uint ResType;
        public uint ResGroup;
        public ulong ResInstance;
        public string ResName;

        public static implicit operator string(TGIN value)
        {
            string extn = ".dat";
            if (ExtList.Ext.ContainsKey("0x" + value.ResType.ToString("X")))
                extn = String.Join("", ExtList.Ext["0x" + value.ResType.ToString("X")].ToArray());
            else if (ExtList.Ext.ContainsKey("*"))
                extn = String.Join("", ExtList.Ext["*"].ToArray());

            return String.Format((value.ResName != null && value.ResName.Length > 0)
                    ? "S3_{0:X8}_{1:X8}_{2:X16}_{3}%%+{4}"
                    : "S3_{0:X8}_{1:X8}_{2:X16}%%+{4}"
                    , value.ResType, value.ResGroup, value.ResInstance, value.ResName == null ? "" : escapeString(value.ResName), extn);
        }

        public static implicit operator TGIN(AResourceIndexEntry value)
        {
            TGIN res = new TGIN();
            res.ResType = value.ResourceType;
            res.ResGroup = value.ResourceGroup;
            res.ResInstance = value.Instance;
            return res;
        }

        public static implicit operator TGIN(string value)
        {
            TGIN res = new TGIN();

            value = System.IO.Path.GetFileNameWithoutExtension(value);

            int i = value.ToLower().IndexOf("s3_");
            if (i == 0) value = value.Substring(3);
            i = value.IndexOf("%%+");
            if (i >= 0) value = value.Substring(0, i);

            string[] fnParts = value.Split(new char[] { '_', '-' }, 4);
            if (fnParts.Length >= 3)
            {
                try
                {
                    res.ResType = Convert.ToUInt32(fnParts[0], 16);
                    res.ResGroup = Convert.ToUInt32(fnParts[1], 16);
                    res.ResInstance = Convert.ToUInt64(fnParts[2], 16);
                }
                catch { }
                if (fnParts.Length == 4)
                    res.ResName = unescapeString(fnParts[3]);
            }

            return res;
        }

        public override string ToString() { return this; }

        private static string unescapeString(string value)
        {
            for (int i = value.IndexOf("%"); i >= 0 && i + 2 < value.Length; i = value.IndexOf("%"))
            {
                try
                {
                    string bad = value.Substring(i + 1, 2);
                    string rep = new string(new char[] { (char)Convert.ToByte(bad, 16) });
                    value = value.Replace("%" + bad, rep);
                }
                catch { break; }
            }
            return value;
        }

        private static string escapeString(string value)
        {
            foreach (char[] ary in new char[][] { Path.GetInvalidFileNameChars(), Path.GetInvalidPathChars(), new char[] { '-' } })
            {
                foreach (char c in ary)
                {
                    string bad = new string(new char[] { c });
                    string rep = String.Format("%{0:x2}", (uint)c);
                    value = value.Replace(bad, rep);
                }
            }
            return value;
        }
    }
}
