﻿/***************************************************************************
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
        static Size size = new Size();

        bool main = false;
        bool ignoreResize = false;

        public NewGridForm()
        {
            ignoreResize = true;
            InitializeComponent();
            ignoreResize = false;
            this.Icon = ((System.Drawing.Icon)(new ComponentResourceManager(typeof(MainForm)).GetObject("$this.Icon")));
            splitContainer1.Panel1Collapsed = true;
        }

        private NewGridForm(bool main)
            : this()
        {
            this.main = main;
            flpMainButtons.Visible = main;
            btnClose.Visible = !main;
            if (main)
            {
                this.AcceptButton = btnOK;
                this.CancelButton = btnCancel;
                //this.StartPosition = FormStartPosition.CenterParent;
                //Because WindowsDefaultLocation is so dumb, it's distracting to start centred then cascade.
                //Until a better method comes along, always cascade.
                this.StartPosition = FormStartPosition.WindowsDefaultLocation;
                size = S3PIDemoFE.Properties.Settings.Default.GridSize;
                if (size.Width == -1 && size.Height == -1)
                    size = new Size(DefaultSize.Width, DefaultSize.Height);
            }
            else
            {
                this.AcceptButton = this.CancelButton = btnClose;
                this.StartPosition = FormStartPosition.WindowsDefaultLocation;
            }
            Size = new Size(size.Width, size.Height);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (!ignoreResize) size = new Size(Width, Height);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (main) S3PIDemoFE.Properties.Settings.Default.GridSize = size;
        }

        public NewGridForm(AApiVersionedFields field, bool main) : this(main) { FieldList = null; s3PIPropertyGrid1.s3piObject = field; }
        public NewGridForm(AApiVersionedFields field) : this(field, false) { }
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
                    contextMenuStrip1.Items.Clear();
                    splitContainer1.Panel1Collapsed = false;
                    tlpAddDelete.Visible = true;
                    if (value.GetType().BaseType.IsGenericType && value.GetType().BaseType.GetGenericArguments()[0].IsAbstract)
                    {
                        btnAdd.Visible = SelectTypes(value.GetType().BaseType.GetGenericArguments()[0]);
                    }
                    else
                    {
                        btnAdd.Visible = true;
                    }
                    fillListBox(-1);
                }
            }
        }
        void fillListBox(int selectedIndex)
        {
            listBox1.SuspendLayout();
            listBox1.Items.Clear();
            string fmt = "[{0:X" + fieldList.Count.ToString("X").Length + "}] {1}";
            for (int i = 0; i < fieldList.Count; i++)
                listBox1.Items.Add(String.Format(fmt, i, fieldList[i].GetType().Name));
            if (selectedIndex == -1) selectedIndex = 0;
            listBox1.SelectedIndex = fieldList.Count > selectedIndex ? selectedIndex : fieldList.Count - 1;
            listBox1.ResumeLayout();
        }

        bool SelectTypes(Type abstractType)
        {
            contextMenuStrip1.ItemClicked -= new ToolStripItemClickedEventHandler(contextMenuStrip1_ItemClicked);
            contextMenuStrip1.ItemClicked += new ToolStripItemClickedEventHandler(contextMenuStrip1_ItemClicked);
            Type declaringType = abstractType.DeclaringType;
            foreach (Type type in declaringType.GetNestedTypes())
            {
                if (!type.IsSubclassOf(abstractType)) continue;
                object[] attrs = type.GetCustomAttributes(typeof(ConstructorParametersAttribute), true);
                if (attrs == null || attrs.Length == 0) continue;

                ToolStripItem tsi = new ToolStripMenuItem(type.Name);
                tsi.Tag = type;
                contextMenuStrip1.Items.Add(tsi);
            }
            return contextMenuStrip1.Items.Count > 0;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            s3PIPropertyGrid1.s3piObject = (AHandlerElement)(listBox1.SelectedIndex >= 0 ? fieldList[listBox1.SelectedIndex] : null);
            btnCopy.Enabled = btnDelete.Enabled = listBox1.SelectedIndex >= 0;
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            int selectedIndex = listBox1.SelectedIndex;
            AHandlerElement selectedElement = (AHandlerElement)(listBox1.SelectedIndex >= 0 ? fieldList[listBox1.SelectedIndex] : null);
            if (selectedElement != null)
                try
                {
                    if (fieldList.Add(selectedElement))
                        selectedIndex = fieldList.Count - 1;
                    else
                        CopyableMessageBox.Show("Copy failed", this.Text, CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MainForm.IssueException(ex, "Copy failed");
                }
            fillListBox(selectedIndex);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (contextMenuStrip1.Items.Count == 0) simple_btnAdd_Click(sender, e);
            else contextMenuStrip1.Show((Control)sender, new Point(3, 3));
        }

        void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            Type type = e.ClickedItem.Tag as Type;
            if (type == null) return;

            int selectedIndex = listBox1.SelectedIndex;
            try
            {
                fieldList.Add((type.GetCustomAttributes(typeof(ConstructorParametersAttribute), true)[0] as ConstructorParametersAttribute).parameters);
                selectedIndex = fieldList.Count - 1;
            }
            catch (Exception ex)
            {
                MainForm.IssueException(ex, "");
            }
            fillListBox(selectedIndex);
        }

        private void simple_btnAdd_Click(object sender, EventArgs e)
        {
            int selectedIndex = listBox1.SelectedIndex;
            try
            {
                fieldList.Add();
                selectedIndex = fieldList.Count - 1;
            }
            catch(Exception ex)
            {
                MainForm.IssueException(ex, "");
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

        // added for http://dino.drealm.info/develforums/s3pi/index.php?topic=685.0
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