/***************************************************************************
 *  Copyright (C) 2013 by Peter L Jones                                    *
 *  pljones@users.sf.net                                                   *
 *                                                                         *
 *  This file is part of s3se.                                             *
 *                                                                         *
 *  s3se is free software: you can redistribute it and/or modify           *
 *  it under the terms of the GNU General Public License as published by   *
 *  the Free Software Foundation, either version 3 of the License, or      *
 *  (at your option) any later version.                                    *
 *                                                                         *
 *  s3tse is distributed in the hope that it will be useful,               *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of         *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the          *
 *  GNU General Public License for more details.                           *
 *                                                                         *
 *  You should have received a copy of the GNU General Public License      *
 *  along with s3translate.  If not, see <http://www.gnu.org/licenses/>.   *
 *                                                                         *
 *  s3translate was created by Jonathan Haas.                              *
 *  s3se was derived from s3translate by Peter L Jones.                    *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using s3se.Properties;

namespace s3se
{
    public partial class Export : Form
    {
        public Export()
        {
            InitializeComponent();
            checkList.Items.AddRange(Form1.locales.ToArray());
            checkList.SetItemChecked(Settings.Default.UserLocale, true);
        }
    }
}
