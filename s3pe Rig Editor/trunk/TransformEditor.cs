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
    public partial class TransformEditor : UserControl
    {
        GrannyRigData.Transform value = new GrannyRigData.Transform(0, null);

        public TransformEditor()
        {
            InitializeComponent();
            uInt32Editor1.Text = "";
        }

        public GrannyRigData.Transform Value
        {
            get { return value; }
            set
            {
                this.value = new GrannyRigData.Transform(0, null, value);
                uInt32Editor1.Value = value.Flags;
                quadEditor1.Value = value.Orientation;
                tripleEditor1.Value = value.Position;
                tripleEditor2.Value = value.ScaleShear0;
                tripleEditor3.Value = value.ScaleShear1;
                tripleEditor4.Value = value.ScaleShear2;
            }
        }

        public event EventHandler ValueChanged;
        protected void OnValueChanged() { if (ValueChanged != null) ValueChanged(this, EventArgs.Empty); }

        private void flags_ValueChanged(object sender, EventArgs e)
        {
            UInt32Editor ue = sender as UInt32Editor;
            value.Flags = ue.Value;
            OnValueChanged();
        }

        private void quadEditor1_ValueChanged(object sender, EventArgs e)
        {
            QuadEditor qe = sender as QuadEditor;
            value.Orientation = qe.Value;
            OnValueChanged();
        }

        private void pos_ValueChanged(object sender, EventArgs e)
        {
            TripleEditor te = sender as TripleEditor;
            value.Position = te.Value;
            OnValueChanged();
        }

        private void sh_ValueChanged(object sender, EventArgs e)
        {
            TripleEditor te = sender as TripleEditor;
            int ss012 = tableLayoutPanel1.GetPositionFromControl(te).Column - tableLayoutPanel1.GetPositionFromControl(lbShear0).Column;

            switch (ss012)
            {
                case 0: value.ScaleShear0 = te.Value; break;
                case 1: value.ScaleShear1 = te.Value; break;
                case 2: value.ScaleShear2 = te.Value; break;
            }
            OnValueChanged();
        }
    }
}
