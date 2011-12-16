/***************************************************************************
 *  Copyright (C) 2011 by Peter L Jones                                    *
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

namespace s3pe.DDSTool
{
    public partial class NewDDSParameters : Form
    {
        public NewDDSParameters()
        {
            InitializeComponent();
        }

        public NewDDSParameters(int width, int height) : this()
        {
            nudWidth.Value = width;
            nudHeight.Value = height;
        }

        public Result Value
        {
            get
            {
                return new Result
                {
                    Red = (byte)nudRed.Value,
                    Green = (byte)nudGreen.Value,
                    Blue = (byte)nudBlue.Value,
                    Alpha = (byte)nudAlpha.Value,
                    Width = (int)nudWidth.Value,
                    Height = (int)nudHeight.Value,
                    DialogResult = this.DialogResult,
                };
            }
        }

        public struct Result
        {
            public byte Red;
            public byte Green;
            public byte Blue;
            public byte Alpha;
            public int Width;
            public int Height;
            public DialogResult DialogResult;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
