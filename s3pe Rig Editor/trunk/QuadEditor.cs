/***************************************************************************
 *  Copyright (C) 2010 by Peter L Jones                                    *
 *  pljones@users.sf.net                                                   *
 *                                                                         *
 *  This helper uses Atavera's RIG Resource and is part of s3pi.           *
 *  Atavera: http://code.google.com/u/dd764ta/                             *
 *  RIG Resource: http://code.google.com/p/s3pi-wrappers/                  *
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
using System.Data;
using System.Windows.Forms;
using s3piwrappers;

namespace RigEditor
{
    public partial class QuadEditor : UserControl
    {
        GrannyRigData.Quad value = new GrannyRigData.Quad(0, null);
        public QuadEditor()
        {
            InitializeComponent();
        }

        [Browsable(false)]
        public override string Text { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        [DefaultValue("X")]
        public string Text1 { get { return singleEditor1.Text; } set { singleEditor1.Text = value; } }
        [DefaultValue("Y")]
        public string Text2 { get { return singleEditor2.Text; } set { singleEditor2.Text = value; } }
        [DefaultValue("Z")]
        public string Text3 { get { return singleEditor3.Text; } set { singleEditor3.Text = value; } }
        [DefaultValue("W")]
        public string Text4 { get { return singleEditor4.Text; } set { singleEditor4.Text = value; } }

        public GrannyRigData.Quad Value
        {
            get { return value; }
            set
            {
                this.value = new GrannyRigData.Quad(0, null, value);
                singleEditor1.Value = value.X;
                singleEditor2.Value = value.Y;
                singleEditor3.Value = value.Z;
                singleEditor4.Value = value.W;
            }
        }

        public event EventHandler ValueChanged;
        protected void OnValueChanged() { if (ValueChanged != null) ValueChanged(this, EventArgs.Empty); }

        private void singleEditor1_ValueChanged(object sender, EventArgs e) { value.X = (sender as SingleEditor).Value; OnValueChanged(); }
        private void singleEditor2_ValueChanged(object sender, EventArgs e) { value.Y = (sender as SingleEditor).Value; OnValueChanged(); }
        private void singleEditor3_ValueChanged(object sender, EventArgs e) { value.Z = (sender as SingleEditor).Value; OnValueChanged(); }
        private void singleEditor4_ValueChanged(object sender, EventArgs e) { value.W = (sender as SingleEditor).Value; OnValueChanged(); }
    }
}
