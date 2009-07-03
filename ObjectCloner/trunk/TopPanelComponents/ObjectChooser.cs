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

namespace ObjectCloner.TopPanelComponents
{
    public partial class ObjectChooser : UserControl
    {
        public ObjectChooser()
        {
            InitializeComponent();
            ObjectChooser_LoadListViewSettings();
        }

        public void ObjectChooser_LoadListViewSettings()
        {
            string cw = ObjectCloner.Properties.Settings.Default.ColumnWidths;
            string[] cws = cw == null ? new string[] { } : cw.Split(':');

            int w;
            listView1.Columns[0].Width = cws.Length > 0 && int.TryParse(cws[0], out w) && w > 0 ? w : (this.listView1.Width - 32) / 3;
            listView1.Columns[1].Width = 0;
            listView1.Columns[2].Width = cws.Length > 2 && int.TryParse(cws[2], out w) && w > 0 ? w : (this.listView1.Width - 32) / 3;
            listView1.Columns[3].Width = cws.Length > 3 && int.TryParse(cws[3], out w) && w > 0 ? w : (this.listView1.Width - 32) / 3;
        }
        public void ObjectChooser_SaveSettings()
        {
            ObjectCloner.Properties.Settings.Default.ColumnWidths = string.Format("{0}:-1:{2}:{3}"
                , listView1.Columns[0].Width
                , listView1.Columns[1].Width
                , listView1.Columns[2].Width
                , listView1.Columns[3].Width
                );
        }

        public event EventHandler SelectedIndexChanged;
        protected void OnSelectedIndexChanged(object sender, EventArgs e) { if (SelectedIndexChanged != null)SelectedIndexChanged(sender, e); }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e) { OnSelectedIndexChanged(sender, e); }

        public View View
        {
            get { return listView1.View; }
            set { listView1.View = value; }
        }

        public ImageList SmallImageList
        {
            get { return listView1.SmallImageList; }
            set { listView1.SmallImageList = value; }
        }

        public ImageList LargeImageList
        {
            get { return listView1.LargeImageList; }
            set { listView1.LargeImageList = value; }
        }

        public ListView.ListViewItemCollection Items
        {
            get { return listView1.Items; }
        }

        public ListView.SelectedListViewItemCollection SelectedItems
        {
            get { return listView1.SelectedItems; }
        }
    }
}
