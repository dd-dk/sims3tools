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

namespace S3PIDemoFE
{
    public partial class NewGridForm : Form
    {
        public NewGridForm()
        {
            InitializeComponent();
            this.Icon = ((System.Drawing.Icon)(new ComponentResourceManager(typeof(MainForm)).GetObject("$this.Icon")));
            splitContainer1.Panel1Collapsed = true;
            listBox1.Visible = false;
        }

        private NewGridForm(bool main)
            : this()
        {
            flpMainButtons.Visible = main;
            btnClose.Visible = !main;
            if (main)
            {
                this.AcceptButton = btnOK;
                this.CancelButton = btnCancel;
            }
            else
                this.AcceptButton = this.CancelButton = btnClose;
        }

        public NewGridForm(AApiVersionedFields field, bool main) : this(main) { s3PIPropertyGrid1.s3piObject = field; }
        public NewGridForm(AApiVersionedFields field) : this(false) { s3PIPropertyGrid1.s3piObject = field; }

        public NewGridForm(IList<AApiVersionedFields> list) : this(false) { FieldList = list; }

        IList<AApiVersionedFields> fieldList;
        public IList<AApiVersionedFields> FieldList
        {
            get { return fieldList; }
            set
            {
                this.fieldList = value;
                if (value == null)
                {
                    splitContainer1.Panel1Collapsed = true;
                    listBox1.Visible = false;
                    s3PIPropertyGrid1.s3piObject = null;
                }
                else
                {
                    splitContainer1.Panel1Collapsed = false;
                    listBox1.Items.Clear();
                    for (int i = 0; i < fieldList.Count; i++)
                        listBox1.Items.Add("[" + i + "] " + fieldList[i].GetType().Name);
                    listBox1.Visible = true;
                    listBox1.SelectedIndex = fieldList.Count > 0 ? 0 : -1;
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            s3PIPropertyGrid1.s3piObject = listBox1.SelectedIndex >= 0 ? fieldList[listBox1.SelectedIndex] : null;
        }
    }
}
