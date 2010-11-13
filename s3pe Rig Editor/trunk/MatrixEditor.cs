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
    public partial class MatrixEditor : UserControl
    {
        GrannyRigData.Matrix4x4 value = new GrannyRigData.Matrix4x4(0, null);

        public MatrixEditor()
        {
            InitializeComponent();
        }

        public GrannyRigData.Matrix4x4 Value
        {
            get { return value; }
            set
            {
                this.value = new GrannyRigData.Matrix4x4(0, null, value);
                quadEditor1.Value = value.M0;
                quadEditor2.Value = value.M1;
                quadEditor3.Value = value.M2;
                quadEditor4.Value = value.M3;
            }
        }

        public event EventHandler ValueChanged;
        protected void OnValueChanged() { if (ValueChanged != null) ValueChanged(this, EventArgs.Empty); }

        private void quadEditor_ValueChanged(object sender, EventArgs e)
        {
            QuadEditor qe = sender as QuadEditor;
            int row = tableLayoutPanel1.GetPositionFromControl(qe).Row;
            int col = tableLayoutPanel1.GetPositionFromControl(qe).Column;
            int m0123 = (row - 1) * 2 + col;

            switch (m0123)
            {
                case 0: value.M0 = qe.Value; break;
                case 1: value.M1 = qe.Value; break;
                case 2: value.M2 = qe.Value; break;
                case 3: value.M3 = qe.Value; break;
            }
            OnValueChanged();
        }
    }
}
