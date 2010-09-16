/***************************************************************************
 *  Copyright (C) 2010 by Peter L Jones                                    *
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
using CatalogResource;

namespace ObjectCloner.CustomControls
{
    public partial class TopicRatings : UserControl
    {
        ObjectCatalogResource.TopicRating[] topicRatings = null;
        List<EnumTextBox> letb = null;
        List<TextBox> ltb = null;
        bool readOnly = false;

        public TopicRatings()
        {
            InitializeComponent();
            letb = new List<EnumTextBox>(new EnumTextBox[] { etbTopic0, etbTopic1, etbTopic2, etbTopic3, etbTopic4, });
            ltb = new List<TextBox>(new TextBox[] { tbRating0, tbRating1, tbRating2, tbRating3, tbRating4, });
            for (int i = 0; i < letb.Count; i++)
                letb[i].EnumType = typeof(ObjectCatalogResource.TopicCategory);
            Clear();
        }

        public void Clear()
        {
            Value = new ObjectCatalogResource(0, null).TopicRatings;
            for (int i = 0; i < topicRatings.Length; i++) letb[i].ReadOnly = ltb[i].ReadOnly = true;
        }

        public void Enable(bool enabled)
        {
            foreach (Control c in letb) c.Enabled = enabled;
            foreach (Control c in ltb) c.Enabled = enabled;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ObjectCatalogResource.TopicRating[] Value
        {
            get { return (ObjectCatalogResource.TopicRating[])topicRatings.Clone(); }
            set
            {
                if ((topicRatings != null && AResource.ArrayCompare(topicRatings, value)) || topicRatings == value) return;

                topicRatings = (ObjectCatalogResource.TopicRating[])value.Clone();
                for (int i = 0; i < topicRatings.Length; i++)
                {
                    letb[i].Value = Convert.ToUInt64(topicRatings[i].Topic);
                    ltb[i].Text = "" + topicRatings[i].Rating;// decimal and allowed to be negative
                }
                OnValueChanged(this, EventArgs.Empty);
            }
        }

        [Browsable(true)]
        [DefaultValue(false)]
        public bool ReadOnly
        {
            get { return readOnly; }
            set
            {
                if (readOnly = value) return;
                readOnly = value;
                for (int i = 0; i < topicRatings.Length; i++)
                    letb[i].ReadOnly = ltb[i].ReadOnly = readOnly;
            }
        }

        private void etbTopic_ValueChanged(object sender, EventArgs e)
        {
            int i = letb.IndexOf(sender as EnumTextBox);
            topicRatings[i].Topic = (ObjectCatalogResource.TopicCategory)letb[i].Value;
            OnValueChanged(this, EventArgs.Empty);
        }

        private void tbRating_Validating(object sender, CancelEventArgs e)
        {
            TextBox tb = sender as TextBox;

            try { Get_Value(sender as TextBox); }
            catch { e.Cancel = true; }

            if (e.Cancel) tb.SelectAll();
        }

        private void tbRating_Validated(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            int i = ltb.IndexOf(tb);
            topicRatings[ltb.IndexOf(tb)].Rating = Get_Value(tb);
            OnValueChanged(this, EventArgs.Empty);
        }

        int Get_Value(TextBox tb)
        {
            if (!tb.Text.StartsWith("0x"))
                return Int32.Parse(tb.Text);
            else
                return Int32.Parse(tb.Text.Substring(2), System.Globalization.NumberStyles.HexNumber);
        }

        public event EventHandler ValueChanged;
        protected void OnValueChanged(object sender, EventArgs e) { if (ValueChanged != null)ValueChanged(sender, e); }
    }
}
