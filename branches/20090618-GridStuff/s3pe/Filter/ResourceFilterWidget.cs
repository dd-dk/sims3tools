/***************************************************************************
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
using System.Windows.Forms;
using s3pi.Interfaces;

namespace S3PIDemoFE.Filter
{
    public partial class ResourceFilterWidget : UserControl
    {
        IList<string> fields = null;
        IResourceIndexEntry ie = null;
        BrowserWidget bw = null;
        Dictionary<string, FilterField> values = null;

        public ResourceFilterWidget()
        {
            InitializeComponent();
            lbCount.Text = "0";
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Specifies the list of fields to display")]
        public IList<string> Fields
        {
            get { return fields; }
            set
            {
                if (fields == value) return;
                fields = value;
                SetFields();
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool FilterEnabled { get { return ckbFilter.Checked; } set { ckbFilter.Checked = value; } }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(typeof(IResourceIndexEntry), "(null)")]
        public IResourceIndexEntry IndexEntry
        {
            get { return ie; }
            set
            {
                if (ie == value) return;
                ie = value;
                foreach (string s in fields)
                    values[s].Value = ie == null ? new TypedValue(typeof(string), "") : ie[s];
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(typeof(IDictionary<string, string>), "(none)")]
        public IList<KeyValuePair<string, TypedValue>> Filter
        {
            get
            {
                if (values == null) return new List<KeyValuePair<string, TypedValue>>();
                List<KeyValuePair<string, TypedValue>> f = new List<KeyValuePair<string, TypedValue>>();
                foreach(string s in fields) if (values[s].EnableFilter) f.Add(new KeyValuePair<string, TypedValue>(s, values[s].Filter));
                return f;
            }
            set
            {
                if (values == null) return;

                foreach (string s in values.Keys)
                    values[s].EnableFilter = false;

                foreach (KeyValuePair<string, TypedValue> kvp in value)
                {
                    if (values.ContainsKey(kvp.Key))
                    {
                        values[kvp.Key].EnableFilter = true;
                        values[kvp.Key].Filter = kvp.Value;
                    }
                }
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(typeof(BrowserWidget), "(none)")]
        public BrowserWidget BrowserWidget
        {
            get { return bw; }
            set
            {
                if (bw == value) return;
                if (bw != null) bw.ListUpdated -= new EventHandler(bw_ListUpdated);
                bw = value;
                if (bw != null) bw.ListUpdated += new EventHandler(bw_ListUpdated);
            }
        }

        [Description("Raised when the value of the filter changes")]
        [Category("Property Changed")]
        public event EventHandler FilterChanged;

        void SetFields()
        {
            Dictionary<string, Type> cft = AApiVersionedFields.GetContentFieldTypes(0, typeof(IResourceIndexEntry));
            values = new Dictionary<string, FilterField>();
            tlpResourceInfo.ColumnCount = fields.Count + 1;
            tlpResourceInfo.RowCount = 1;
            tlpResourceInfo.RowStyles.Clear();
            tlpResourceInfo.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlpResourceInfo.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            tlpResourceInfo.ColumnStyles.Clear();
            tlpResourceInfo.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tlpResourceInfo.Controls.Add(flpControls, 0, 0);
            for (int i = 0; i < fields.Count; i++)
            {
                if (fields[i] == "Instance")
                    tlpResourceInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 150));
                else if (i > 5)
                    tlpResourceInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75));
                else
                    tlpResourceInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
                FilterField ff = new FilterField();
                ff.Dock = DockStyle.Fill;
                ff.Title = fields[i];
                ff.Value = new TypedValue(cft[fields[i]], "");
                ff.EnableFilter = false;
                ff.Filter = new TypedValue(cft[fields[i]], "");
                tlpResourceInfo.Controls.Add(ff, i + 1, 0);
                values.Add(fields[i], ff);
                ff.FilterChanged += new EventHandler(ff_FilterChanged);
            }
            tlpResourceInfo.PerformLayout();
        }

        protected virtual void OnFilterChanged(object sender, EventArgs e) { if (FilterChanged != null) FilterChanged(sender, e); }

        private void ckbFilter_CheckedChanged(object sender, EventArgs e)
        {
            OnFilterChanged(this, new EventArgs());
        }

        private void ff_FilterChanged(object sender, EventArgs e)
        {
            FilterField ff = sender as FilterField;
            if (ff == null) return;
            if (ckbFilter.Checked) OnFilterChanged(this, new EventArgs());
        }

        private void bw_ListUpdated(object sender, EventArgs e)
        {
            lbCount.Text = (sender as BrowserWidget).Count.ToString();
        }
    }
}
