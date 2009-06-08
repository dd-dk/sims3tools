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
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using s3pi.Interfaces;

namespace S3PIDemoFE
{
    public partial class GridForm : Form
    {
        interface exporterResult { } // just to differentiate delegate signatures - return null
        interface importerResult { } // just to differentiate delegate signatures - return null
        delegate exporterResult Exporter(IResource resource, string field);
        delegate importerResult Importer(IResource resource, string field);
        delegate Control ControlCreator(IResource resource, string field);

        struct Row
        {
            public IResource resource;
            public Label fieldName;
            public Control value;
            public Button btnExporter;
            public Button btnImporter;
            public Exporter exporter;
            public Importer importer;

            public Row(IResource resource, string fieldName, Control value, Exporter exporter, Importer importer)
                : this(resource, fieldName, value, exporter, importer, "Export...", "Import...") { }

            public Row(IResource resource, string fieldName, Control value, Exporter exporter, Importer importer, string btnExporterText, string btnImporterText)
                : this(resource, fieldName, value)
            {
                if (exporter != null)
                {
                    this.exporter = exporter;
                    btnExporter = new Button();
                    btnExporter.Anchor = System.Windows.Forms.AnchorStyles.None;
                    btnExporter.Text = btnExporterText;
                    btnExporter.Click += new EventHandler(btnExporter_Click);
                }

                if (importer != null)
                {
                    this.importer = importer;
                    btnImporter = new Button();
                    btnImporter.Anchor = System.Windows.Forms.AnchorStyles.None;
                    btnImporter.Text = btnImporterText;
                    btnImporter.Click += new EventHandler(btnImporter_Click);
                }
            }

            public Row(IResource resource, string fieldNameText, Control value)
            {
                this.resource = resource;

                this.fieldName = new Label();
                this.fieldName.Anchor = System.Windows.Forms.AnchorStyles.Right;
                this.fieldName.AutoSize = true;
                this.fieldName.Text = fieldNameText;

                this.value = value;

                this.btnExporter = null;
                this.btnImporter = null;
                this.exporter = null;
                this.importer = null;
            }

            void btnExporter_Click(object sender, EventArgs e) { if (exporter != null) exporter(resource, fieldName.Text); }
            void btnImporter_Click(object sender, EventArgs e) { if (importer != null) importer(resource, fieldName.Text); }
        }

        Dictionary<Type, Exporter> typeExporterMap = new Dictionary<Type, Exporter>();
        Dictionary<Type, Importer> typeImporterMap = new Dictionary<Type, Importer>();
        Dictionary<Type, ControlCreator> typeControlCreatorMap = new Dictionary<Type, ControlCreator>();

        public GridForm()
        {
            InitializeComponent();
            typeExporterMap.Add(typeof(TextReader), exportTextReader);
            typeImporterMap.Add(typeof(TextReader), importTextReader);
            typeExporterMap.Add(typeof(BinaryReader), exportBinaryReader);
            typeImporterMap.Add(typeof(BinaryReader), importBinaryReader);
            typeControlCreatorMap.Add(typeof(String), textboxCreator);
            typeControlCreatorMap.Add(typeof(bool), labelCreator);
            typeControlCreatorMap.Add(typeof(int), labelCreator);
            typeControlCreatorMap.Add(typeof(uint), labelCreator);
            typeControlCreatorMap.Add(typeof(ushort), labelCreator);
            typeControlCreatorMap.Add(typeof(byte), labelCreator);
        }

        public GridForm(IResource resource) : this()
        {
            tableLayoutPanel1.RowCount = 0;
            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.RowStyles.Clear();
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(label2, 1, 0);
            tableLayoutPanel1.RowStyles.Insert(0, new RowStyle(SizeType.AutoSize));

            List<Row> ldsr = new List<Row>();
            Dictionary<string, Type> dst = AResource.GetContentFieldTypes(0, resource.GetType());
            
            foreach (string f in resource.ContentFields)
            {
                if (f.Equals("Stream") || f.Equals("AsBytes") || f.Equals("Value")) continue;

                Type t = dst[f];
                ControlCreator cntl = typeControlCreatorMap.ContainsKey(t) ? typeControlCreatorMap[t] : typenameCreator;
                Exporter exp = typeExporterMap.ContainsKey(t) ? typeExporterMap[t] : null;
                Importer imp = typeImporterMap.ContainsKey(t) ? typeImporterMap[t] : null;
                
                if (cntl == null && exp == null && imp == null) continue;

                Row dsr = new Row(resource, f, cntl == null ? null : cntl(resource, f), exp, imp);
                ldsr.Add(dsr);
            }

            foreach (Row dsr in ldsr)
            {
                int row = tableLayoutPanel1.RowCount - 1;
                tableLayoutPanel1.RowCount++;

                tableLayoutPanel1.RowStyles.Insert(row, new RowStyle(SizeType.AutoSize));
                tableLayoutPanel1.Controls.Add(dsr.fieldName, 0, row);
                if (dsr.value != null)
                    tableLayoutPanel1.Controls.Add(dsr.value, 1, row);
                if (dsr.exporter != null)
                    tableLayoutPanel1.Controls.Add(dsr.btnExporter, 2, row);
                if (dsr.importer != null)
                    tableLayoutPanel1.Controls.Add(dsr.btnImporter, 3, row);
            }
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        }

        Control textboxCreator(IResource resource, string field)
        {
            if (!resource.ContentFields.Contains(field))
                throw new InvalidOperationException();
            string s = "" + resource[field].Value;
            if (s == null)
                throw new InvalidCastException();

            TextBox res = new TextBox();

            res.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            res.BorderStyle = BorderStyle.None;
            res.ReadOnly = true;
            res.TabStop = false;
            res.Multiline = true;
            res.Text = s.Substring(0, Math.Min(s.Length, 256)) + (s.Length > 256 ? "..." : "");

            if (res.Lines.Length > 0) res.Width = Math.Max(res.Width, (int)(res.Lines[0].Length * res.Font.Size));
            if (res.Lines.Length > 0) res.Height = Math.Max(res.Height, res.Lines.Length * res.Font.Height);

            return res;
        }

        Control labelCreator(IResource resource, string field)
        {
            if (!resource.ContentFields.Contains(field))
                throw new InvalidOperationException();

            Label res = new Label();
            res.Anchor = AnchorStyles.Left;
            res.AutoSize = true;
            res.Text = "" + resource[field].Value;
            return res;
        }

        Control typenameCreator(IResource resource, string field)
        {
            if (!resource.ContentFields.Contains(field))
                throw new InvalidOperationException();

            Label res = new Label();
            res.Anchor = AnchorStyles.Left;
            res.AutoSize = true;
            res.Text = "[" + resource[field].Type.Name + "]";
            return res;
        }

        exporterResult exportTextReader(IResource resource, string field)
        {
            if (!resource.ContentFields.Contains(field))
                throw new InvalidOperationException();
            TextReader s = resource[field].Value as TextReader;
            if (s == null)
                throw new InvalidCastException();

            saveFileDialog1.Filter = "Text files|*.txt;*.xml|All files|*.*";
            DialogResult dr = saveFileDialog1.ShowDialog();
            if (dr != DialogResult.OK) return null;

            FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.Create, FileAccess.Write);
            (new BinaryWriter(fs)).Write(s.ReadToEnd().ToCharArray());
            fs.Close();

            return null;
        }

        importerResult importTextReader(IResource resource, string field)
        {
            if (!resource.ContentFields.Contains(field))
                throw new InvalidOperationException();
            TextReader s = resource[field].Value as TextReader;
            if (s == null)
                throw new InvalidCastException();

            openFileDialog1.Filter = "Text files|*.txt;*.xml|All files|*.*";
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr != DialogResult.OK) return null;

            resource[field] = new TypedValue(resource[field].Type, new StreamReader(openFileDialog1.FileName));

            this.Close();

            return null;
        }

        exporterResult exportBinaryReader(IResource resource, string field)
        {
            if (!resource.ContentFields.Contains(field))
                throw new InvalidOperationException();
            BinaryReader s = resource[field].Value as BinaryReader;
            if (s == null)
                throw new InvalidCastException();

            saveFileDialog1.Filter = "All files|*.*";
            DialogResult dr = saveFileDialog1.ShowDialog();
            if (dr != DialogResult.OK) return null;

            FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.Create, FileAccess.Write);
            (new BinaryWriter(fs)).Write(s.ReadBytes((int)s.BaseStream.Length));
            fs.Close();

            return null;
        }

        importerResult importBinaryReader(IResource resource, string field)
        {
            if (!resource.ContentFields.Contains(field))
                throw new InvalidOperationException();
            BinaryReader s = resource[field].Value as BinaryReader;
            if (s == null)
                throw new InvalidCastException();

            openFileDialog1.Filter = "All files|*.*";
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr != DialogResult.OK) return null;

            resource[field] = new TypedValue(resource[field].Type, new BinaryReader(new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read)));

            this.Close();

            return null;
        }
    }
}
