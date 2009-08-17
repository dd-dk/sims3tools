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
    public partial class FindAsYouType : UserControl
    {
        ListView listToSearch;

        public FindAsYouType()
        {
            InitializeComponent();
        }

        public FindAsYouType(ListView lv)
            : this()
        {
            listToSearch = lv;
        }

        public void SendKeyDown(KeyEventArgs value)
        {
            if (value.KeyCode == Keys.Enter) { btnNext_Click(this, EventArgs.Empty); value.Handled = true; }
            else if (value.KeyCode == Keys.Escape) { btnCancel_Click(this, EventArgs.Empty); value.Handled = true; }
            else if (value.KeyCode == Keys.Back) tbSearch.Text = tbSearch.Text.Substring(0, tbSearch.Text.Length - 1);
            else if (!Char.IsControl((char)value.KeyCode)) { tbSearch.Text += (char)value.KeyCode; value.Handled = true; }
        }

        public event EventHandler Close;


        private void Search(bool forwards)
        {
            int index = forwards ? listToSearch.SelectedIndices[0] + 1 : 0;
            while (index < listToSearch.Items.Count)
            {
                if (listToSearch.Items[index].Text.ToLower().Contains(tbSearch.Text.ToLower()))
                {
                    listToSearch.Items[index].Selected = true;
                    break;
                }
                index++;
            }
        }


        private void tbSearch_TextChanged(object sender, EventArgs e)
        {
            if (listToSearch == null || tbSearch.Text.Length == 0) { btnCancel_Click(sender, e); return; }
            this.Visible = true;
            if (listToSearch.SelectedItems.Count == 0 ||
                !listToSearch.SelectedItems[0].Text.Contains(tbSearch.Text)) Search(false);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (listToSearch == null || tbSearch.Text.Length == 0) return;
            Search(listToSearch.SelectedItems.Count == 1);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (Close != null) Close(this, EventArgs.Empty);
        }
    }
}
