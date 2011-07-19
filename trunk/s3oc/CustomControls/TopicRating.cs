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
    public partial class TopicRating : UserControl
    {
        ObjectCatalogResource.TopicCategory topic = ObjectCatalogResource.TopicCategory.EndOfTopics;
        int rating = 0;
        bool readOnly = false;

        public TopicRating()
        {
            InitializeComponent();
        }


        [Browsable(true)]
        [DefaultValue(ObjectCatalogResource.TopicCategory.EndOfTopics)]
        public ObjectCatalogResource.TopicCategory Topic
        {
            get { return topic; }
            set
            {
                if (topic == value) return;
                topic = value;
                tbTopic.Text = "0x" + ((uint)value).ToString("X8");
                lbValue.Text = new ObjectCatalogResource.TopicRating(0, null, topic, rating).Value;
                OnValueChanged(this, EventArgs.Empty);
            }
        }

        [Browsable(true)]
        [DefaultValue((int)0)]
        public int Rating
        {
            get { return rating; }
            set
            {
                if (rating == value) return;
                rating = value;
                tbRating.Text = "0x" + value.ToString("X8");
                lbValue.Text = new ObjectCatalogResource.TopicRating(0, null, topic, rating).Value;
                OnValueChanged(this, EventArgs.Empty);
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ObjectCatalogResource.TopicRating Value
        {
            get { return new ObjectCatalogResource.TopicRating(0, null, topic, rating); }
            set
            {
                topic = value.Topic; tbTopic.Text = "0x" + ((uint)value.Topic).ToString("X8");
                rating = value.Rating; tbRating.Text = "0x" + value.Rating.ToString("X8");
                lbValue.Text = value.Value;
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
                tbRating.ReadOnly = tbTopic.ReadOnly = readOnly;
            }
        }

        public void Clear() { topic = 0; rating = 0; tbTopic.Text = ""; tbRating.Text = ""; lbValue.Text = ""; }

        public event EventHandler ValueChanged;
        protected void OnValueChanged(object sender, EventArgs e) { if (ValueChanged != null) ValueChanged(sender, e); }

        UInt64 Get_Value(string value)
        {
            if (value == "") return 0;

            if (!value.StartsWith("0x"))
                return UInt64.Parse(value);
            else
                return UInt64.Parse(value.Substring(2), System.Globalization.NumberStyles.HexNumber);
        }
        object Get_Value(Type t, TextBox tb)
        {
            if (typeof(Enum).IsAssignableFrom(t))
            {
                if (new List<string>(Enum.GetNames(t)).Contains(tb.Text))
                    return Enum.Parse(t, tb.Text);
                t = Enum.GetUnderlyingType(t);
            }
            return Convert.ChangeType(Get_Value(tb.Text), t);
        }

        private void tb_Validating(object sender, CancelEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.Text == "") return;

            try { Get_Value(tb == tbTopic ? typeof(ObjectCatalogResource.TopicCategory) : typeof(int), tb); }
            catch { e.Cancel = true; }

            if (e.Cancel) tb.SelectAll();
        }

        private void tb_Validated(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            object value = Get_Value(tb == tbTopic ? typeof(ObjectCatalogResource.TopicCategory) : typeof(int), tb);

            if (tb == tbTopic)
                Topic = (ObjectCatalogResource.TopicCategory)value;
            else
                Rating = (int)value;
        }
    }
}
