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
    public class STBLHandler
    {
        public const ulong FNV64Blank = 0xCBF29CE484222325;

        public static bool LangSearch { get { return ObjectCloner.Properties.Settings.Default.LangSearch; } }

        static Dictionary<byte, List<SpecificResource>> _STBLs = null;
        public static void Reset() { _STBLs = null; }
        static Dictionary<byte, List<SpecificResource>> STBLs
        {
            get
            {
                if (_STBLs == null)
                {
                    _STBLs = new Dictionary<byte, List<SpecificResource>>();
                    if (FileTable.fb0 != null)
                        FileTable.fb0.ForEach(ppt => ppt
                            .FindAll(rie => rie.ResourceType == 0x220557DA)
                            .ForEach(sr =>
                            {
                                //note that we do not want to actually run the resource wrapper here, it's too slow
                                byte lang = (byte)(sr.ResourceIndexEntry.Instance >> 56);
                                if (!_STBLs.ContainsKey(lang)) _STBLs.Add(lang, new List<SpecificResource>());
                                _STBLs[lang].Add(sr);
                            })
                        );
                }
                return _STBLs;
            }
        }
        public static bool IsOK { get { return STBLs != null && STBLs.Count > 0; } }

        public delegate void Callback(byte lang);
        public static SpecificResource findStblFor(ulong guid, Callback callBack = null)
        {
            if (guid == FNV64Blank) return null;

            for (byte i = 0; i < (LangSearch ? 0x17 : 0x01); i++) { if (callBack != null) callBack(i); SpecificResource sr = findStblFor(guid, i); if (sr != null) return sr; }
            return null;
        }

        public static SpecificResource findStblFor(ulong guid, byte lang)
        {
            if (guid == FNV64Blank) return null;
            return STBLs.ContainsKey(lang) ? STBLs[lang].Find(sr =>
            {
                var stbl = sr.Resource as IDictionary<ulong, string>;
                if (stbl == null) return false;
                return stbl.ContainsKey(guid);
            }) : null;
        }

        public static string StblLookup(ulong guid, int lang = -1, Callback callBack = null)
        {
            if (guid == FNV64Blank) return null;

            SpecificResource sr;
            if (lang < 0 || lang >= 0x17) sr = findStblFor(guid, callBack);
            else sr = findStblFor(guid, (byte)lang);
            return sr == null ? null : (sr.Resource as IDictionary<ulong, string>)[guid];
        }

        static string language_fmt = "Strings_{0}_{1:x2}{2:x14}";
        static string[] languages = new string[] {
            "ENG_US", "CHI_CN", "CHI_TW", "CZE_CZ",
            "DAN_DK", "DUT_NL", "FIN_FI", "FRE_FR",
            "GER_DE", "GRE_GR", "HUN_HU", "ITA_IT",
            "JAP_JP", "KOR_KR", "NOR_NO", "POL_PL",

            "POR_PT", "POR_BR", "RUS_RU", "SPA_ES",
            "SPA_MX", "SWE_SE", "THA_TH",
        };

        public static void AddSTBLToNameMap(IDictionary<ulong, string> nameMap, byte lang, ulong instance)
        {
            if (nameMap == null) throw new ArgumentNullException("nameMap");
            if (nameMap.ContainsKey(instance)) return;
            string value = String.Format(language_fmt, languages[lang], lang, instance & 0x00FFFFFFFFFFFFFF);
            nameMap.Add(instance, value);
        }
    }
}