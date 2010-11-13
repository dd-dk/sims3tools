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
    public partial class ResourceList : UserControl, ICollection<string>
    {
        public ResourceList()
        {
            InitializeComponent();
            listBox1.Font = new Font(FontFamily.GenericMonospace, listBox1.Font.Size);
        }

        public string Page
        {
            get { return label1.Text; }
            set { label1.Text = value; }
        }

        #region ICollection<string> Members

        public void Add(string item) { listBox1.Items.Add(item); }

        public void Clear() { listBox1.Items.Clear(); }

        public bool Contains(string item) { return listBox1.Items.Contains(item); }

        public void CopyTo(string[] array, int arrayIndex) { listBox1.Items.CopyTo(array, arrayIndex); }

        public int Count { get { return listBox1.Items.Count; } }

        public bool IsReadOnly { get { return listBox1.Items.IsReadOnly; } }

        public bool Remove(string item) { if (!Contains(item)) return false; listBox1.Items.Remove(item); return true; }

        #endregion

        #region IEnumerable<string> Members

        public IEnumerator<string> GetEnumerator() { return (IEnumerator<string>)listBox1.Items.GetEnumerator(); }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return listBox1.Items.GetEnumerator(); }

        #endregion

        public event EventHandler SelectedIndexChanged;
        protected void OnSelectedIndexChanged(object sender, EventArgs e) { if (SelectedIndexChanged != null) SelectedIndexChanged(sender, e); }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) { OnSelectedIndexChanged(sender, e); }

        public int SelectedIndex
        {
            get { return listBox1.SelectedIndex; }
            set { listBox1.SelectedIndex = value; }
        }

        public string SelectedItem
        {
            get { return (string)listBox1.SelectedItem; }
        }
    }
}
