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
using System.Drawing.Design;
using System.Globalization;

namespace S3PIDemoFE
{

    //http://msdn.microsoft.com/en-us/library/aa302334.aspx

    public class Profile
    {
        short mRUListSize = S3PIDemoFE.Properties.Settings.Default.MRUListSize;
        bool overrideExtensions = S3PIDemoFE.Properties.Settings.Default.OverrideExtensions;
        System.Collections.Specialized.StringCollection lOverrideExtension = S3PIDemoFE.Properties.Settings.Default.ExtensionsConfig;

        System.Windows.Forms.PropertyGrid pg;
        public Profile(System.Windows.Forms.PropertyGrid pg) { this.pg = pg; }

        public void Commit()
        {
            S3PIDemoFE.Properties.Settings.Default.MRUListSize = mRUListSize;
            S3PIDemoFE.Properties.Settings.Default.OverrideExtensions = overrideExtensions;
            S3PIDemoFE.Properties.Settings.Default.ExtensionsConfig = lOverrideExtension;
        }


        [Category("Files")]
        [DefaultValue(4)]
        [Description("Set the maximum number of recent files to remember.  Valid values are 0 to 9.")]
        [DisplayName("Limit of recent files")]
        public short MaxRecentFiles
        {
            get { return mRUListSize; }
            set { if (value >= 0 && value <= 9) mRUListSize = value; }
        }

        [Category("Resources")]
        [DefaultValue(false)]
        [Description("By default, S3PIDemoFE uses a common standard approach to naming files.  " +
            "Setting this to true allows you to specify an alternative set of suffixes to be applied to files based on their resource type.")]
        [DisplayName("Override standard extensions?")]
        public bool OverrideStandardExtensions
        {
            get { return overrideExtensions; }
            set
            {
                overrideExtensions = value;
                pg.Refresh();
            }
        }

        [Category("Resources")]
        [DefaultValue(null)]
        [Description("Specify the overrides to standard extensions.")]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [DisplayName("Overrides to standard extensions")]
        [Editor(typeof(OverrideExtensionEditor), typeof(UITypeEditor))]
        public System.Collections.Specialized.StringCollection OverrideExtensions { get { return lOverrideExtension; } set { lOverrideExtension = value; } }

        private bool ShouldSerializeMaxRecentFiles() { return MaxRecentFiles != 4; }
        private bool ShouldSerializeOverrideStandardExtensions() { return OverrideStandardExtensions; }
        private bool ShouldSerializeOverrideExtensions() { return OverrideStandardExtensions && (OverrideExtensions != null && OverrideExtensions.Count > 0); }


        public class OverrideExtensionEditor : UITypeEditor
        {
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                //just in case
                if (context.Instance as Profile == null) return base.GetEditStyle(context);

                return ((Profile)context.Instance).OverrideStandardExtensions ? UITypeEditorEditStyle.Modal : UITypeEditorEditStyle.None;
            }

            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                //return value;
                return base.EditValue(context, provider, value);
            }
        }
    }
}
