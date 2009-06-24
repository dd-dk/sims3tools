﻿/***************************************************************************
 *  Copyright (C) 2009 by Peter L Jones                                    *
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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using s3pi.Interfaces;
using System.Windows.Forms.Design;

namespace S3PIDemoFE
{
    public partial class ReaderEditorPanel : UserControl
    {
        public ReaderEditorPanel()
        {
            InitializeComponent();
        }

        AApiVersionedFields owner;
        string field;
        Type type;
        //TypedValue field;
        public void SetField(AApiVersionedFields owner, string field)
        {
            type = AApiVersionedFields.GetContentFieldTypes(0, owner.GetType())[field];
            if (!(typeof(TextReader).IsAssignableFrom(type) || typeof(BinaryReader).IsAssignableFrom(type)))
                throw new InvalidCastException();
            this.owner = owner;
            this.field = field;
        }

        IWindowsFormsEditorService edSvc;
        public IWindowsFormsEditorService EdSvc { get { return edSvc; } set { edSvc = value; } }

        private void Import_TextReader()
        {
            openFileDialog1.Filter = "Text files|*.txt;*.xml|All files|*.*";
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr != DialogResult.OK) return;

            FileStream fs = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read);
            owner[field] = new TypedValue(type, new StreamReader(fs));
            fs.Close();
        }
        private void Import_BinaryReader()
        {
            openFileDialog1.Filter = "All files|*.*";
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr != DialogResult.OK) return;

            FileStream fs = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read);
            owner[field] = new TypedValue(type, new BinaryReader(fs));
            fs.Close();
        }
        private void btnImport_Click(object sender, EventArgs e)
        {
            if (typeof(TextReader).IsAssignableFrom(type)) Import_TextReader();
            if (typeof(BinaryReader).IsAssignableFrom(type)) Import_BinaryReader();
            edSvc.CloseDropDown();
        }

        private void Export_TextReader()
        {
            saveFileDialog1.Filter = "Text files|*.txt;*.xml|All files|*.*";
            DialogResult dr = saveFileDialog1.ShowDialog();
            if (dr != DialogResult.OK) return;

            FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.Create, FileAccess.Write);
            TextReader r = owner[field].Value as TextReader;
            try { ((StreamReader)r).BaseStream.Position = 0; }
            catch { }
            (new BinaryWriter(fs)).Write(r.ReadToEnd().ToCharArray());
            fs.Close();
        }
        private void Export_BinaryReader()
        {
            saveFileDialog1.Filter = "All files|*.*";
            DialogResult dr = saveFileDialog1.ShowDialog();
            if (dr != DialogResult.OK) return;

            FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.Create, FileAccess.Write);
            BinaryReader r = owner[field].Value as BinaryReader;
            (new BinaryWriter(fs)).Write(r.ReadBytes((int)r.BaseStream.Length));
            fs.Close();
        }
        private void btnExport_Click(object sender, EventArgs e)
        {
            if (typeof(TextReader).IsAssignableFrom(type)) Export_TextReader();
            if (typeof(BinaryReader).IsAssignableFrom(type)) Export_BinaryReader();
            edSvc.CloseDropDown();
        }
    }
}