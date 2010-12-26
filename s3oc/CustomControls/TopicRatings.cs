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
        bool readOnly = false;

        public TopicRatings()
        {
            InitializeComponent();
            Clear();
        }

        void IterateTLP(Action<TopicRating> tr)
        {
            for (int i = 0; i < tlpTopicRatings.RowCount - 1; i++)
                tr((TopicRating)tlpTopicRatings.GetControlFromPosition(1, i));
        }

        public void Clear() { IterateTLP(x => x.Clear()); }

        public void Enable(bool enabled) { IterateTLP(x => x.Enabled = enabled); }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ObjectCatalogResource.TopicRating[] Value
        {
            get
            {
                List<ObjectCatalogResource.TopicRating>res = new List<ObjectCatalogResource.TopicRating>();
                IterateTLP(x => res.Add(x.Value));
                return res.ToArray();
            }
            set
            {
                if (AResource.ArrayCompare(Value, value)) return;
                int i = 0;
                IterateTLP(x => x.Value = value[i++]);
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
                IterateTLP(x => x.ReadOnly = value);
            }
        }
    }
}
