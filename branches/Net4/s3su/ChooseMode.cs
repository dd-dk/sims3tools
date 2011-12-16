using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace S3Pack
{
    public partial class ChooseMode : Form
    {
        public ChooseMode()
        {
            InitializeComponent();
        }

        public int Mode { get; private set; }

        private void btnUnpack_Click(object sender, EventArgs e)
        {
            Mode = 0;
            this.Close();
        }

        private void btnRepack_Click(object sender, EventArgs e)
        {
            Mode = 1;
            this.Close();
        }

        private void btnPack_Click(object sender, EventArgs e)
        {
            Mode = 2;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Mode = -1;
            this.Close();
        }
    }
}
