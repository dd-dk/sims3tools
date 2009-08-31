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

            int h = 4 * Application.OpenForms[0].ClientSize.Height / 5;
            int w = 4 * Application.OpenForms[0].ClientSize.Width / 5;
            this.ClientSize = new Size(w, h);
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

        public NewGridForm(AApiVersionedFields field, bool main) : this(main) { FieldList = null; s3PIPropertyGrid1.s3piObject = field; }
        public NewGridForm(AApiVersionedFields field) : this(false) { FieldList = null; s3PIPropertyGrid1.s3piObject = field; }

        public NewGridForm(IGenericAdd list) : this(false) { FieldList = list; }

        IGenericAdd fieldList;
        public IGenericAdd FieldList
        {
            get { return fieldList; }
            set
            {
                this.fieldList = value;
                listBox1.Items.Clear();

                if (value == null)
                {
                    splitContainer1.Panel1Collapsed = true;
                    tlpAddDelete.Visible = false;
                    s3PIPropertyGrid1.s3piObject = null;
                }
                else
                {
                    splitContainer1.Panel1Collapsed = false;
                    tlpAddDelete.Visible = !(value.GetType().BaseType.IsGenericType && value.GetType().BaseType.GetGenericArguments()[0].IsAbstract);
                    fillListBox(-1);
                }
            }
        }
        void fillListBox(int selectedIndex)
        {
            listBox1.SuspendLayout();
            listBox1.Items.Clear();
            for (int i = 0; i < fieldList.Count; i++)
                listBox1.Items.Add("[" + i + "] " + fieldList[i].GetType().Name);
            if (selectedIndex == -1) selectedIndex = 0;
            listBox1.SelectedIndex = fieldList.Count > selectedIndex ? selectedIndex : fieldList.Count - 1;
            listBox1.ResumeLayout();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            s3PIPropertyGrid1.s3piObject = (AHandlerElement)(listBox1.SelectedIndex >= 0 ? fieldList[listBox1.SelectedIndex] : null);
            btnDelete.Enabled = listBox1.SelectedIndex >= 0;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            int selectedIndex = listBox1.SelectedIndex;
            int count = fieldList.Count;
            try
            {
                fieldList.Add();
            }
            catch(Exception ex)
            {
                string s = "";
                for (Exception inex = ex; inex != null; inex = ex.InnerException) s += inex.Message + "\n\n";
                CopyableMessageBox.Show(s, this.Text, CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Error);
            }
            fillListBox(selectedIndex);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int selectedIndex = listBox1.SelectedIndex;
            fieldList.RemoveAt(listBox1.SelectedIndex);
            listBox1.SelectedIndex = -1;
            fillListBox(selectedIndex);
        }

        // added for http://www.simlogical.com/S3PIdevelforum/index.php?topic=685.0
        bool OKtoClose = false;
        private void btnClose_Click(object sender, EventArgs e)
        {
            OKtoClose = true;
            this.Close();
        }

        private void NewGridForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && !OKtoClose) e.Cancel = true;
        }
    }
}
