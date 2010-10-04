using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RigEditor
{
    public partial class UInt32Editor : UserControl
    {
        UInt32 u = 0;

        public UInt32Editor()
        {
            InitializeComponent();
        }

        [Browsable(true)]
        public override string Text { get { return lbUInt32.Text; } set { lbUInt32.Text = value; } }

        [Browsable(true)]
        [DefaultValue((uint)0)]
        public UInt32 Value
        {
            get { return u; }
            set { u = value; tbUInt32.Text = "0x" + u.ToString("X8"); }
        }

        [Browsable(true)]
        public event EventHandler ValueChanged;
        protected void OnValueChanged() { if (ValueChanged != null) ValueChanged(this, EventArgs.Empty); }

        void tbSingle_Validating(object sender, CancelEventArgs e)
        {
            TextBox tb = sender as TextBox;
            string value = tb.Text.Trim();
            UInt32 f = 0;
            if (value.StartsWith("0x"))
                e.Cancel = !UInt32.TryParse(value.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out f);
            else
                e.Cancel = !UInt32.TryParse(value, out f);
            if (e.Cancel) tb.SelectAll();
        }

        void tbSingle_Validated(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            string value = tb.Text.Trim();
            if (value.StartsWith("0x"))
                Value = UInt32.Parse(value.Substring(2), System.Globalization.NumberStyles.HexNumber, null);
            else
                Value = UInt32.Parse(value);
            OnValueChanged();
        }
    }
}
