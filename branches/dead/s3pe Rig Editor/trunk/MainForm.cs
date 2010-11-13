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
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using s3piwrappers;

namespace RigEditor
{
    public partial class MainForm : Form, s3pi.DemoPlugins.IRunHelper
    {
        const string Unparented = "(unparented)";
        const string SingleFormat = "0.00000";
        RigResource resource;
        GrannyRigData.ElementList<GrannyRigData.Bone> bones;
        string[] names;
        public MainForm()
        {
            InitializeComponent();
            lbSelect.Text = "";
            btnDelete.Enabled = false;
        }

        public MainForm(Stream s)
            : this()
        {
            try
            {
                Application.UseWaitCursor = true;
                loadRig(s);
            }
            finally { Application.UseWaitCursor = false; }
        }

        void loadRig(Stream s)
        {
            resource = new RigResource(0, s);
            try
            {
                bones = (resource.RigData as GrannyRigData).Skeleton.Bones;
            }
            catch (NullReferenceException nex)
            {
                CopyableMessageBox.IssueException(nex, "Could not run - check installation instructions.\nIs RigResource.DLL present?\n\n", "Program failed");
                Environment.Exit(1);
            }
            CreateTable();
        }

        void saveRig()
        {
            result = (byte[])resource.AsBytes.Clone();
        }

        byte[] result = null;
        public byte[] Result { get { return result; } }

        private void btnOK_Click(object sender, EventArgs e)
        {
            saveRig();
            Environment.ExitCode = 0;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Environment.ExitCode = 1;
            this.Close();
        }

        void ClearTable()
        {
            for (int i = 1; i < tlpBones.RowCount - 1; i++)
            {
                for (int j = 0; j < tlpBones.ColumnCount; j++)
                {
                    tlpBones.Controls.Remove(tlpBones.GetControlFromPosition(j, i));
                }
                tlpBones.RowStyles.Remove(tlpBones.RowStyles[1]);
            }
            tlpBones.RowCount = 2;
        }

        void CreateTable()
        {
            RefreshNames();
            bones.ForEach(bone => CreateRow(bone));
        }

        void UpdateParents()
        {
            RefreshNames();
            foreach (Control c in tlpBones.Controls)
            {
                if (c is ComboBox)
                {
                    ComboBox cb = c as ComboBox;
                    cb.SelectedIndex = -1;
                    cb.Items.Clear();
                    cb.Items.AddRange(names);
                    cb.SelectedIndex = bones[tlpBones.GetPositionFromControl(cb).Row - 1].ParentIndex + 1;
                }
            }
        }

        void RefreshNames()
        {
            names = new string[bones.Count + 1];
            names[0] = Unparented;
            for (int i = 0; i < bones.Count; i++) names[i + 1] = bones[i].Name;
        }

        /*struct DoubleEuler
        {
            public double X, Y, Z;
            public override string ToString()
            {
                return string.Format("{0}; {1}; {2}",
                    X.ToString(SingleFormat, CultureInfo.InvariantCulture),
                    Y.ToString(SingleFormat, CultureInfo.InvariantCulture),
                    Z.ToString(SingleFormat, CultureInfo.InvariantCulture));
            }
            public static implicit operator string(DoubleEuler e) { return e.ToString(); }

            struct DoubleQuad
            {
                public double X, Y, Z, W;
            }

            static DoubleEuler ToEuler(GrannyRigData.Quad q)
            {
                DoubleQuad sq;
                DoubleEuler e;
                sq.X = Math.Pow(q.X, 2D);
                sq.Y = Math.Pow(q.Y, 2D);
                sq.Z = Math.Pow(q.Z, 2D);
                sq.W = Math.Pow(q.W, 2D);
                double poleTest = q.X * q.Y + q.Z * q.W;
                if (poleTest > 0.49999)
                {
                    e.Y = 2 * Math.Atan2(q.X, q.W);
                    e.Z = Math.PI / 2;
                    e.X = 0;
                }
                else if (poleTest < -0.49999)
                {
                    e.Y = -2 * Math.Atan2(q.X, q.W);
                    e.Z = -Math.PI / 2;
                    e.X = 0;
                }
                else
                {
                    e.X = Math.Atan2(2D * q.X * q.W - 2 * q.Y * q.Z, 1 - 2 * sq.X - 2 * sq.Z);
                    e.Y = Math.Asin(2 * poleTest);
                    e.Z = Math.Atan2(2D * q.Y * q.W - 2 * q.X * q.Z, 1 - 2 * sq.Y - 2 * sq.Z);
                }
                return e;
            }
            public static implicit operator DoubleEuler(GrannyRigData.Quad q) { return ToEuler(q); }
        }/**/

        string ParentName(GrannyRigData.Bone bone) { return bone.ParentIndex == -1 ? Unparented : bones[bone.ParentIndex].Name; }
        int ParentIndex(string value) { GrannyRigData.Bone bone = bones.Find(x => x.Name == value); return bone == null ? -1 : bones.IndexOf(bone); }
        void CreateRow(GrannyRigData.Bone bone)
        {
            int row = tlpBones.RowCount - 1;
            tlpBones.RowCount++;
            tlpBones.RowStyles.Insert(row, new RowStyle(SizeType.AutoSize));

            int tabIndex = row * 8;
            int col = 0;

            CheckBox ckb = new CheckBox();
            ckb.Anchor = AnchorStyles.None;
            ckb.AutoEllipsis = false;
            ckb.AutoSize = true;
            ckb.Name = "ckb" + bone.Name;
            ckb.TabIndex = tabIndex++;
            ckb.Click += new EventHandler(ckb_Click);
            tlpBones.Controls.Add(ckb, col++, row);

            TableLayoutPanel tlp = new TableLayoutPanel();
            int tlpTabIndex = 0;
            tlp.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tlp.AutoSize = true;
            tlp.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            tlp.ColumnCount = 2;
            tlp.Name = "tlp" + bone.Name;
            tlp.RowCount = 2;
            tlp.TabIndex = tabIndex++;

            tlp.Controls.Add(StringView("Bone", "Bone", true, ref tlpTabIndex), 0, 0);
            tlp.Controls.Add(StringEditor("Bone" + bone.Name, bone.Name, tbBone_Validating, tbBone_Validated, ref tlpTabIndex), 1, 0);
            tlp.Controls.Add(StringView("Parent", "Parent", true, ref tlpTabIndex), 0, 1);

            ComboBox cb = new ComboBox();
            cb.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            cb.DropDownStyle = ComboBoxStyle.DropDownList;
            cb.Items.AddRange(names);
            cb.Name = "cb" + bone.Name;
            cb.SelectedIndex = bone.ParentIndex + 1;
            cb.SelectedIndexChanged += new EventHandler(cb_SelectedIndexChanged);
            cb.TabIndex = tlpTabIndex++;
            tlp.Controls.Add(cb, 1, 1);
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            tlpBones.Controls.Add(tlp, col++, row);
            tlpBones.Controls.Add(MatrixControl("Matrix" + bone.Name, bone.InverseWorld4X4, matrix_Changed, ref tabIndex), col++, row);
            tlpBones.Controls.Add(TransformControl("Transform" + bone.Name, bone.LocalTransform, transform_Changed, ref tabIndex), col++, row);
        }

        Control StringView(string name, string value, bool bold, ref int tabIndex)
        {
            Label lb = new Label();
            lb.Anchor = AnchorStyles.Right;
            lb.AutoSize = true;
            if (bold) lb.Font = new System.Drawing.Font(lb.Font, FontStyle.Bold);
            lb.Name = "lb" + name;
            lb.Margin = new System.Windows.Forms.Padding(3);
            lb.TabIndex = tabIndex++;
            lb.Text = value;
            return lb;
        }
        Control StringEditor(string name, string value, CancelEventHandler validator, EventHandler validated, ref int tabIndex)
        {
            TextBox tb = new TextBox();
            tb.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tb.Name = "tb" + name;
            tb.TabIndex = tabIndex++;
            tb.Text = value;
            if (validator != null) tb.Validating += validator;
            if (validated != null) tb.Validated += validated;
            return tb;
        }
        Control MatrixControl(string name, GrannyRigData.Matrix4x4 value, EventHandler changed, ref int tabIndex)
        {
            MatrixEditor me = new MatrixEditor();
            me.Anchor = AnchorStyles.None;
            me.Name = "me" + name;
            me.Text = "";
            me.Value = value;
            if (changed != null) me.ValueChanged += new EventHandler(changed);
            return me;
        }
        Control TransformControl(string name, GrannyRigData.Transform value, EventHandler changed, ref int tabIndex)
        {
            TransformEditor tf = new TransformEditor();
            tf.Anchor = AnchorStyles.None;
            tf.Name = "tf" + name;
            tf.Text = "";
            tf.Value = value;
            if (changed != null) tf.ValueChanged += new EventHandler(changed);
            return tf;
        }

        void ckb_Click(object sender, EventArgs e)
        {
            bool enabled = (sender as CheckBox).Checked;
            foreach(Control c in tlpBones.Controls)
                if (c is CheckBox && (c as CheckBox).Checked) { enabled = true; break; }
            btnDelete.Enabled = enabled;
        }

        void cb_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            int index = tlpBones.GetPositionFromControl(cb).Row - 1;
            if (cb.SelectedIndex == -1 || bones[index].ParentIndex == cb.SelectedIndex - 1) return;
            if (index == cb.SelectedIndex - 1) cb.SelectedIndex = bones[index].ParentIndex + 1;
            bones[index].ParentIndex = cb.SelectedIndex - 1;
        }

        void tbBone_Validating(object sender, CancelEventArgs e)
        {
            TextBox tb = sender as TextBox;
            string value = tb.Text.Trim();
            int index = tlpBones.GetPositionFromControl(tb).Row - 1;
            if (value.Equals(bones[index].Name)) return;

            e.Cancel = value.Contains(" ") || bones.Find(bone => bone.Name.Equals(value)) != null;
            if (e.Cancel) tb.SelectAll();
        }

        void tbBone_Validated(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            string value = tb.Text.Trim();
            int index = tlpBones.GetPositionFromControl(tb).Row - 1;
            if (value.Equals(bones[index].Name)) return;

            tb.Text = bones[index].Name = value;
            UpdateParents();
        }

        void matrix_Changed(object sender, EventArgs e)
        {
            MatrixEditor me = sender as MatrixEditor;
            int index = tlpBones.GetPositionFromControl(me).Row - 1;

            bones[index].InverseWorld4X4 = me.Value;
        }

        void transform_Changed(object sender, EventArgs e)
        {
            TransformEditor tf = sender as TransformEditor;
            int index = tlpBones.GetPositionFromControl(tf).Row - 1;

            bones[index].LocalTransform = tf.Value;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            btnDelete.Enabled = false;

            List<GrannyRigData.Bone> toBeDeleted = new List<GrannyRigData.Bone>();
            for (int i = 1; i < tlpBones.RowCount - 1; i++)
            {
                if ((tlpBones.GetControlFromPosition(0, i) as CheckBox).Checked)
                    toBeDeleted.Add(bones[i - 1]);
            }

            foreach(var bone in bones)
            {
                GrannyRigData.Bone parent = bone.ParentIndex == -1 ? null : bones[bone.ParentIndex];
                while(parent != null && toBeDeleted.Contains(parent))
                    parent = parent.ParentIndex == -1 ? null : bones[parent.ParentIndex];
                bone.ParentIndex = parent == null ? -1 : bones.IndexOf(parent);
            }
            toBeDeleted.ForEach(bone => bones.Remove(bone));

            tlpBones.SuspendLayout();
            ClearTable();
            CreateTable();
            tlpBones.ResumeLayout();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            GrannyRigData.Bone bone = new GrannyRigData.Bone(0, null);
            bone.Name = "Bone" + (tlpBones.RowCount - 1).ToString();
            bones.Add(bone);
            CreateRow(bone);
            UpdateParents();
        }
    }
}
