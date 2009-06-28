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
using System.Drawing;
using System.Windows.Forms;

namespace S3PIDemoFE
{
    public partial class CopyableMessageBox : Form
    {
        public static int Show(string message)
        {
            return Show(message, Application.OpenForms.Count > 0 ? Application.OpenForms[0].Text : "",
                CopyableMessageBoxIcon.Information, new List<string>(new string[] { "OK" }), -1, -1);
        }
        public static int Show(string message, string caption)
        {
            return Show(message, caption,
                CopyableMessageBoxIcon.Information, new List<string>(new string[] { "OK" }), -1, -1);
        }
        public static int Show(string message, string caption, CopyableMessageBoxButtons buttons)
        {
            return Show(message, caption,
                CopyableMessageBoxIcon.Information, enumToList(buttons), -1, -1);
        }
        public static int Show(string message, string caption, CopyableMessageBoxButtons buttons, CopyableMessageBoxIcon icon)
        {
            return Show(message, caption, icon, enumToList(buttons), -1, -1);
        }
        public static int Show(string message, string caption, CopyableMessageBoxButtons buttons, CopyableMessageBoxIcon icon, int defBtn)
        {
            return Show(message, caption, icon, enumToList(buttons), defBtn, -1);
        }
        public static int Show(string message, string caption, CopyableMessageBoxButtons buttons, CopyableMessageBoxIcon icon, int defBtn, int cncBtn)
        {
            return Show(message, caption, icon, enumToList(buttons), defBtn, cncBtn);
        }

        private static IList<string> enumToList(CopyableMessageBoxButtons buttons)
        {
            switch (buttons)
            {
                case CopyableMessageBoxButtons.OKCancel: return new List<string>(new string[] { "&OK", "&Cancel", });
                case CopyableMessageBoxButtons.AbortRetryIgnore: return new List<string>(new string[] { "&Abort", "&Retry", "&Ignore", });
                case CopyableMessageBoxButtons.YesNoCancel: return new List<string>(new string[] { "&Yes", "&No", "&Cancel", });
                case CopyableMessageBoxButtons.YesNo: return new List<string>(new string[] { "&Yes", "&No", });
                case CopyableMessageBoxButtons.OK:
                default: return new List<string>(new string[] { "&OK", });
            }
        }

        public static int Show(string message, string caption, CopyableMessageBoxIcon icon, IList<string> buttons, int defBtn, int cncBtn)
        {
            return Show(Application.OpenForms[0], message, caption, icon, buttons, defBtn, cncBtn);
        }

        public static int Show(IWin32Window owner, string message, string caption, CopyableMessageBoxIcon icon, IList<string> buttons, int defBtn, int cncBtn)
        {
            CopyableMessageBox cmb = new CopyableMessageBox(message, caption, icon, buttons, defBtn, cncBtn);
            cmb.theButton = null;
            cmb.ShowDialog(owner);
            return (cmb.theButton != null) ? buttons.IndexOf(cmb.theButton.Text) : -1;
        }


        private CopyableMessageBox()
        {
            InitializeComponent();
        }

        private CopyableMessageBox(string message, string caption, CopyableMessageBoxIcon icon, IList<string> buttons, int defBtn, int cncBtn)
            : this()
        {
            if (buttons.Count < 1)
                throw new ArgumentLengthException("At least one button text must be supplied");

            this.SuspendLayout();
            this.Text = caption;


            Label lb = new Label();
            lb.MaximumSize = new Size((int)(Application.OpenForms[0].Height * .8) - 90, (int)(Application.OpenForms[0].Width * .8) - 112);
            lb.AutoSize = true;
            lb.Text = message;

            string[] lines = message.Split('\n');
            this.tbMessage.Lines = lines;

            Size tbSize = new Size(lb.PreferredHeight, lb.PreferredWidth);
            this.Size = new Size(tbSize.Height + 90, tbSize.Width + 112);
            
            CreateButtons(buttons, defBtn, cncBtn);

            switch (icon)
            {
                case CopyableMessageBoxIcon.Information:
                    lbIcon.Visible = true; lbIcon.Text = "i"; lbIcon.ForeColor = Color.Blue; lbIcon.Font = new Font(lbIcon.Font, FontStyle.Italic); break;
                case CopyableMessageBoxIcon.Question:
                    lbIcon.Visible = true; lbIcon.Text = "?"; lbIcon.ForeColor = Color.Green; lbIcon.Font = new Font(lbIcon.Font, FontStyle.Regular); break;
                case CopyableMessageBoxIcon.Warning:
                    lbIcon.Visible = true; lbIcon.Text = "!"; lbIcon.ForeColor = Color.Brown; lbIcon.Font = new Font(lbIcon.Font, FontStyle.Bold); break;
                case CopyableMessageBoxIcon.Error:
                    lbIcon.Visible = true; lbIcon.Text = "X"; lbIcon.ForeColor = Color.Red; lbIcon.Font = new Font(lbIcon.Font, FontStyle.Bold); break;
                case CopyableMessageBoxIcon.None:
                default:
                    lbIcon.Visible = false; break;
            }

            this.ResumeLayout();
        }

        void CreateButtons(IList<string> buttons, int defBtn, int cncBtn)
        {
            if (defBtn == -1) defBtn = 0;
            if (cncBtn == -1) cncBtn = buttons.Count - 1;
            flpButtons.SuspendLayout();
            flpButtons.Controls.Clear();
            for (int i = buttons.Count; i > 0; i--)
            {
                Button btn = CreateButton("button" + i, i, buttons[i - 1]);
                flpButtons.Controls.Add(btn);
                if (i == defBtn - 1) this.AcceptButton = btn;
                if (i == cncBtn - 1) this.CancelButton = btn;
            }
            flpButtons.ResumeLayout();
        }

        Button CreateButton(string Name, int TabIndex, string Text)
        {
            Button newButton = new Button();
            newButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            newButton.Name = Name;
            newButton.Size = new System.Drawing.Size(75, 23);
            newButton.TabIndex = TabIndex;
            newButton.Text = Text;
            newButton.UseVisualStyleBackColor = true;
            newButton.Click += new System.EventHandler(this.button_Click);
            return newButton;
        }

        Button theButton = null;
        private void button_Click(object sender, EventArgs e)
        {
            theButton = sender as Button;
            this.Close();
        }
    }

    public enum CopyableMessageBoxIcon
    {
        None,
        Information,
        Question,
        Warning,
        Error,
    }

    public enum CopyableMessageBoxButtons
    {
        OK,
        OKCancel,
        AbortRetryIgnore,
        YesNoCancel,
        YesNo,
        RetryCancel,
    }

}
