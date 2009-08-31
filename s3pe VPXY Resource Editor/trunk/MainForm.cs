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
using System.IO;
using System.Windows.Forms;
using s3pi.Interfaces;
using s3pi.GenericRCOLResource;

namespace s3pe_VPXY_Resource_Editor
{
    public partial class MainForm : Form
    {
        const string myName = "s3pe VPXY Resource Editor";
        public MainForm()
        {
            InitializeComponent();
            this.Controls.Remove(lbCurrentPart);
            this.Controls.Remove(lbLPCurrent);
            lnud.AddRange(new NumericUpDown[]{
                nudLowerX, nudLowerY, nudLowerZ,
                nudUpperX, nudUpperY, nudUpperZ,
            });

            MemoryStream ms = Clipboard.GetData(DataFormats.Serializable) as MemoryStream;
            if (ms == null)
#if DEBUG
            {
                AResource.TGIBlock tgib = new AResource.TGIBlock(0, null, "ITG", 0x736884F1, 0, 0);
                ARCOLBlock rcol = GenericRCOLResourceHandler.CreateRCOLBlock(0, null, 0x736884F1);
                GenericRCOLResource.ChunkEntry ce = new GenericRCOLResource.ChunkEntry(0, null, tgib, rcol);
                GenericRCOLResource grr = new GenericRCOLResource(0, null);
                grr.ChunkEntries.Add(ce);
                ms = grr.Stream as MemoryStream;
            }
#else
                throw new Exception("Clipboard data not a MemoryStream");
#endif

            try
            {
                Application.UseWaitCursor = true;
                loadVPXY(ms);
            }
            finally { Application.UseWaitCursor = false; }
            btnOK.Enabled = vpxy.TGIBlocks.Count > 0;
        }

        GenericRCOLResource rcol;
        VPXY vpxy;
        List<Control> lcntl = new List<Control>();
        List<Button> lbtn = new List<Button>();
        List<TGIBlockCombo> ltbc = new List<TGIBlockCombo>();
        List<TGIBlockCombo> lLPtbc = new List<TGIBlockCombo>();
        List<NumericUpDown> lnud = new List<NumericUpDown>();
        int currentEntry = -1;
        int currentLPEntry = -1;
        bool dirty = false;

        void loadVPXY(Stream data)
        {
            try
            {
                rcol = new GenericRCOLResource(0, data);
            }
            catch { rcol = null; }
            Clipboard.Clear();
            if (rcol == null || rcol.ChunkEntries.Count != 1 || rcol.ChunkEntries[0].RCOLBlock.Tag != "VPXY")
            {
                throw new Exception("Clipboard data was invalid.");
            }
            vpxy = rcol.ChunkEntries[0].RCOLBlock as VPXY;
            if (vpxy == null)
            {
                throw new Exception("Clipboard data was invalid.");
            }

            FillPartsTLP();

            nudLowerX.Value = new Decimal(vpxy.BoundingBox[0]);
            nudLowerY.Value = new Decimal(vpxy.BoundingBox[1]);
            nudLowerZ.Value = new Decimal(vpxy.BoundingBox[2]);
            nudUpperX.Value = new Decimal(vpxy.BoundingBox[3]);
            nudUpperY.Value = new Decimal(vpxy.BoundingBox[4]);
            nudUpperZ.Value = new Decimal(vpxy.BoundingBox[5]);

            tbcFTPT.Enabled = ckbModular.Checked = vpxy.Modular;
            tbcFTPT.TGIBlocks = vpxy.TGIBlocks;
            tbcFTPT.SelectedIndex = vpxy.Modular ? (int)vpxy.FTPTIndex : -1;

            rcol.ResourceChanged += new EventHandler(rcol_ResourceChanged);
        }

        void saveVPXY()
        {
            ClearLinkedPartsTLP();
            AResource.CountedTGIBlockList ltgib = new AResource.CountedTGIBlockList(null);
            vpxy.Entries.Clear();
            uint count = 0;
            byte count00 = 0;
            for (int row = 1; row < tlpParts.RowCount - 1; row++)
            {
                Control c = tlpParts.GetControlFromPosition(1, row);
                int index = lcntl.IndexOf(c);
                if (index < 0) continue;
                if (c as TGIBlockCombo != null)
                {
                    if ((c as TGIBlockCombo).SelectedIndex < 0) continue;
                    ltgib.Add(vpxy.TGIBlocks[(c as TGIBlockCombo).SelectedIndex]);
                    vpxy.Entries.Add(new VPXY.Entry01(0, null, 1, count++));
                }
                else if (c as Button != null)
                {
                    VPXY.Entry00 e00 = c.Tag as VPXY.Entry00;
                    if (e00.TGIIndexes.Count <= 0) continue;
                    e00.EntryID = count00++;
                    foreach (VPXY.ElementUInt32 elem in e00.TGIIndexes)
                    {
                        ltgib.Add(vpxy.TGIBlocks[(int)elem.Data]);
                        elem.Data = count++;
                    }
                    vpxy.Entries.Add(e00);
                }
            }
            vpxy.TGIBlocks.Clear();
            vpxy.TGIBlocks.AddRange(ltgib);
            MemoryStream ms = new MemoryStream(rcol.AsBytes);
            Clipboard.SetData(DataFormats.Serializable, ms);
        }

        void ClearPartsTLP()
        {
            for (int i = 0; i < tlpParts.Controls.Count; i++)
                if (tlpParts.Controls[i] as Label == null)
                    tlpParts.Controls.Remove(tlpParts.Controls[i--]);
            while (tlpParts.RowStyles.Count > 2)
                tlpParts.RowStyles.RemoveAt(1);
            tlpParts.RowCount = 2;
        }
        void FillPartsTLP()
        {
            tlpParts.SuspendLayout();
            ClearPartsTLP();
            int tabindex = 1;
            foreach(VPXY.Entry entry in vpxy.Entries)
            {
                if (entry as VPXY.Entry00 != null)
                {
                    VPXY.Entry00 e00 = entry as VPXY.Entry00;
                    int[] indexes = new int[e00.TGIIndexes.Count];
                    for (int i = 0; i < indexes.Length; i++) indexes[i] = (int)e00.TGIIndexes[i].Data;
                    AddTableRowLinked(tlpParts, ref tabindex);
                }
                else if (entry as VPXY.Entry01 != null)
                {
                    VPXY.Entry01 e01 = entry as VPXY.Entry01;
                    AddTableRowTBC(tlpParts, (int)e01.TGIIndex, ref tabindex);
                }
            }
            tlpParts.ResumeLayout();
        }
        void AddTableRowLinked(TableLayoutPanel tlp, ref int tabindex)
        {
            tlp.RowCount++;
            tlp.RowStyles.Insert(tlp.RowCount - 2, new RowStyle(SizeType.AutoSize));

            Button btn = new Button();
            btn.Anchor = AnchorStyles.Left;
            btn.AutoSize = true;
            btn.Margin = new Padding(0);
            btn.Name = "btnLinked" + tabindex;
            btn.TabIndex = tabindex++;
            btn.Text = "Linked Parts...";
            tlp.Controls.Add(btn, 1, tlp.RowCount - 2);

            btn.Tag = vpxy.Entries[lcntl.Count];

            lbtn.Add(btn);
            lcntl.Add(btn);
            btn.Click += new EventHandler(btn_Click);
        }
        void AddTableRowTBC(TableLayoutPanel tlp, int index, ref int tabindex)
        {
            tlp.RowCount++;
            tlp.RowStyles.Insert(tlp.RowCount - 2, new RowStyle(SizeType.AutoSize));

            TGIBlockCombo tbc = new TGIBlockCombo(vpxy.TGIBlocks, index);
            tbc.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tbc.Name = "tbc" + tabindex;
            tbc.TabIndex = tabindex++;
            tbc.Enter += new EventHandler(tbc_Enter);
            tbc.SelectedIndexChanged += new EventHandler(tbc_SelectedIndexChanged);
            tlp.Controls.Add(tbc, 1, tlp.RowCount - 2);

            if (tlp == tlpParts)
            {
                ltbc.Add(tbc);
                lcntl.Add(tbc);
            }
            else
                lLPtbc.Add(tbc);

            tbc.TGIBlockListChanged += new EventHandler(tbg_TGIBlockListChanged);
        }
        void ClearLinkedPartsTLP()
        {
            if (currentLPEntry != -1 && currentEntry != -1)
            {
                VPXY.Entry00 e00 = lcntl[currentEntry].Tag as VPXY.Entry00;
                e00.TGIIndexes.Clear();
                foreach (Control c in tlpLinkedParts.Controls)
                {
                    if (c as TGIBlockCombo == null) continue;
                    if (lLPtbc.IndexOf(c as TGIBlockCombo) < 0) continue;
                    e00.TGIIndexes.Add(new VPXY.ElementUInt32(0, null, (uint)(c as TGIBlockCombo).SelectedIndex));
                }
            }
            currentLPEntry = -1;
            lLPtbc = null;
            for (int i = 0; i < tlpLinkedParts.Controls.Count; i++)
                if (!tlpLinkedParts.Controls[i].Equals(lbLPTitle))
                    tlpLinkedParts.Controls.Remove(tlpLinkedParts.Controls[i--]);
            while (tlpLinkedParts.RowStyles.Count > 2)
                tlpLinkedParts.RowStyles.RemoveAt(1);
            tlpLinkedParts.RowCount = 2;
            tlpLPControls.Enabled = tlpLinkedParts.Enabled = false;
        }
        void FillLinkedPartsTLP(VPXY.Entry00 entry)
        {
            tlpLinkedParts.SuspendLayout();
            ClearLinkedPartsTLP();
            lLPtbc = new List<TGIBlockCombo>();
            int tabindex = 1;
            foreach (var x in entry.TGIIndexes)
            {
                AddTableRowTBC(tlpLinkedParts, (int)x.Data, ref tabindex);
            }
            tlpLPControls.Enabled = tlpLinkedParts.Enabled = true;
            tlpLinkedParts.ResumeLayout();
        }

        void rcol_ResourceChanged(object sender, EventArgs e)
        {
            dirty = true;
        }

        void tbg_TGIBlockListChanged(object sender, EventArgs e)
        {
            if (ltbc != null)
                foreach (TGIBlockCombo tbc in ltbc)
                {
                    tbc.Refresh();
                    if (tbc.SelectedIndex == -1 && vpxy.TGIBlocks.Count > 0) tbc.SelectedIndex = 0;
                }
            if (lLPtbc != null)
                foreach (TGIBlockCombo tbc in lLPtbc)
                {
                    tbc.Refresh();
                    if (tbc.SelectedIndex == -1 && vpxy.TGIBlocks.Count > 0) tbc.SelectedIndex = 0;
                }
            if (vpxy.Modular)
            {
                tbcFTPT.Refresh();
                if (tbcFTPT.SelectedIndex == -1 && vpxy.TGIBlocks.Count > 0) tbcFTPT.SelectedIndex = 0;
            }

            btnOK.Enabled = vpxy.TGIBlocks.Count > 0;
        }

        void tbc_Enter(object sender, EventArgs e)
        {
            TGIBlockCombo tbc = sender as TGIBlockCombo;

            if (ltbc.Contains(tbc))
            {
                ClearLinkedPartsTLP();//before currentEntry changes
                currentEntry = lcntl.IndexOf(tbc);
                tlpParts.Controls.Add(lbCurrentPart, 0, currentEntry + 1);
            }
            else
            {
                currentLPEntry = lLPtbc.IndexOf(tbc);
                tlpLinkedParts.Controls.Add(lbLPCurrent, 0, currentLPEntry + 1);
            }
        }

        void tbc_SelectedIndexChanged(object sender, EventArgs e)
        {
            TGIBlockCombo tbc = sender as TGIBlockCombo;

            if (ltbc.Contains(tbc))
            {
                int i = lcntl.IndexOf(tbc);
                VPXY.Entry01 e01 = vpxy.Entries[i] as VPXY.Entry01;
                e01.TGIIndex = (tbc.SelectedIndex >= 0) ? (uint)tbc.SelectedIndex : 0;
            }
            else
            {
                if (currentEntry == -1) return;
                VPXY.Entry00 e00 = vpxy.Entries[currentEntry] as VPXY.Entry00;
                int i = lLPtbc.IndexOf(tbc);
                e00.TGIIndexes[i].Data = (tbc.SelectedIndex >= 0) ? (uint)tbc.SelectedIndex : 0;
            }
        }

        void btn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            currentEntry = lcntl.IndexOf(btn);
            tlpParts.Controls.Add(lbCurrentPart, 0, currentEntry + 1);
            FillLinkedPartsTLP(btn.Tag as VPXY.Entry00);
        }

        private void btnTGIEditor_Click(object sender, EventArgs e)
        {
            AResource.CountedTGIBlockList tgiBlocksCopy = new AResource.CountedTGIBlockList(null, vpxy.TGIBlocks);
            DialogResult dr = TGIBlockListEditor.Show(tgiBlocksCopy);
            if (dr != DialogResult.OK) return;
            vpxy.TGIBlocks.Clear();
            vpxy.TGIBlocks.AddRange(tgiBlocksCopy.ToArray());
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            if (currentEntry < 1 || vpxy.Entries.Count < 2) return;

            VPXY.Entry entry = vpxy.Entries[currentEntry];
            vpxy.Entries.RemoveAt(currentEntry);
            vpxy.Entries.Insert(currentEntry - 1, entry);
            Control c1 = tlpParts.GetControlFromPosition(1, currentEntry + 1);//this control
            Control c2 = tlpParts.GetControlFromPosition(1, currentEntry);//the one above to swap with
            tlpParts.Controls.Remove(c1);//leaves currentEntry + 1 free
            tlpParts.Controls.Add(c2, 1, currentEntry + 1);//leaves currentEntry free
            tlpParts.Controls.Add(c1, 1, currentEntry);
            int i = lcntl.IndexOf(c1);
            lcntl.Remove(c1);
            lcntl.Insert(i - 1, c1);
            currentEntry--;
            tlpParts.Controls.Add(lbCurrentPart, 0, currentEntry + 1);
            lbtn = new List<Button>(); foreach (Control c in lcntl) if (c as Button != null) lbtn.Add(c as Button);
            ltbc = new List<TGIBlockCombo>(); foreach (Control c in lcntl) if (c as TGIBlockCombo != null) ltbc.Add(c as TGIBlockCombo);
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            if (currentEntry == vpxy.Entries.Count - 1 || vpxy.Entries.Count < 2) return;

            VPXY.Entry entry = vpxy.Entries[currentEntry];
            vpxy.Entries.RemoveAt(currentEntry);
            vpxy.Entries.Insert(currentEntry + 1, entry);
            Control c1 = tlpParts.GetControlFromPosition(1, currentEntry + 1);//this control
            Control c2 = tlpParts.GetControlFromPosition(1, currentEntry + 2);//the one below to swap with
            tlpParts.Controls.Remove(c1);//leaves currentEntry + 1 free
            tlpParts.Controls.Add(c2, 1, currentEntry + 1);//leaves currentEntry + 2 free
            tlpParts.Controls.Add(c1, 1, currentEntry + 2);
            int i = lcntl.IndexOf(c1);
            lcntl.Remove(c1);
            lcntl.Insert(i + 1, c1);
            currentEntry++;
            tlpParts.Controls.Add(lbCurrentPart, 0, currentEntry + 1);
            lbtn = new List<Button>(); foreach (Control c in lcntl) if (c as Button != null) lbtn.Add(c as Button);
            ltbc = new List<TGIBlockCombo>(); foreach (Control c in lcntl) if (c as TGIBlockCombo != null) ltbc.Add(c as TGIBlockCombo);
        }

        private void llAddPart_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            int tabindex = tlpParts.RowCount;
            vpxy.Entries.Add(new VPXY.Entry01(0, null, 1, 0));
            AddTableRowTBC(tlpParts, -1, ref tabindex);
            tbc_Enter(ltbc[ltbc.Count - 1], EventArgs.Empty);
        }

        private void llAddLinked_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            int tabindex = tlpParts.RowCount;
            vpxy.Entries.Add(new VPXY.Entry00(0, null, 0, (byte)lbtn.Count, new List<VPXY.ElementUInt32>()));
            AddTableRowLinked(tlpParts, ref tabindex);
            btn_Click(lbtn[lbtn.Count - 1], EventArgs.Empty);
        }

        private void llDel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (currentEntry == -1) return;
            ClearLinkedPartsTLP();
            tlpParts.Controls.Remove(lbCurrentPart);

            VPXY.Entry entry = vpxy.Entries[currentEntry];
            vpxy.Entries.RemoveAt(currentEntry);

            Control c1 = tlpParts.GetControlFromPosition(1, currentEntry + 1);
            tlpParts.Controls.Remove(c1);
            lcntl.Remove(c1);
            for (int i = currentEntry; i < vpxy.Entries.Count; i++)
            {
                c1 = tlpParts.GetControlFromPosition(1, i + 2);
                tlpParts.Controls.Add(c1, 1, i + 1);
            }
            tlpParts.RowCount--;
            lbtn = new List<Button>(); foreach (Control c in lcntl) if (c as Button != null) lbtn.Add(c as Button);
            ltbc = new List<TGIBlockCombo>(); foreach (Control c in lcntl) if (c as TGIBlockCombo != null) ltbc.Add(c as TGIBlockCombo);
            currentEntry = -1;
        }

        private void btnLPUp_Click(object sender, EventArgs e)
        {
            if (currentEntry == -1) return;
            VPXY.Entry00 e00 = vpxy.Entries[currentEntry] as VPXY.Entry00;
            if (currentLPEntry < 1 || e00.TGIIndexes.Count < 2) return;

            VPXY.ElementUInt32 element = e00.TGIIndexes[currentLPEntry];
            e00.TGIIndexes.RemoveAt(currentLPEntry);
            e00.TGIIndexes.Insert(currentLPEntry - 1, element);
            Control c1 = tlpLinkedParts.GetControlFromPosition(1, currentLPEntry + 1);//this control
            Control c2 = tlpLinkedParts.GetControlFromPosition(1, currentLPEntry);//the one above to swap with
            tlpLinkedParts.Controls.Remove(c1);//leaves currentEntry + 1 free
            tlpLinkedParts.Controls.Add(c2, 1, currentLPEntry + 1);//leaves currentEntry free
            tlpLinkedParts.Controls.Add(c1, 1, currentLPEntry);
            int i = lLPtbc.IndexOf(c1 as TGIBlockCombo);
            lLPtbc.RemoveAt(i);
            lLPtbc.Insert(i - 1, c1 as TGIBlockCombo);
            currentLPEntry--;
            tlpLinkedParts.Controls.Add(lbLPCurrent, 0, currentLPEntry + 1);
        }

        private void btnLPDown_Click(object sender, EventArgs e)
        {
            if (currentEntry == -1) return;
            VPXY.Entry00 e00 = vpxy.Entries[currentEntry] as VPXY.Entry00;
            if (currentLPEntry == e00.TGIIndexes.Count - 1 || e00.TGIIndexes.Count < 2) return;

            VPXY.ElementUInt32 element = e00.TGIIndexes[currentLPEntry];
            e00.TGIIndexes.RemoveAt(currentLPEntry);
            e00.TGIIndexes.Insert(currentLPEntry + 1, element);
            Control c1 = tlpLinkedParts.GetControlFromPosition(1, currentLPEntry + 1);//this control
            Control c2 = tlpLinkedParts.GetControlFromPosition(1, currentLPEntry + 2);//the one below to swap with
            tlpLinkedParts.Controls.Remove(c1);//leaves currentEntry + 1 free
            tlpLinkedParts.Controls.Add(c2, 1, currentLPEntry + 1);//leaves currentEntry + 2 free
            tlpLinkedParts.Controls.Add(c1, 1, currentLPEntry + 2);
            int i = lLPtbc.IndexOf(c1 as TGIBlockCombo);
            lLPtbc.RemoveAt(i);
            lLPtbc.Insert(i + 1, c1 as TGIBlockCombo);
            currentLPEntry++;
            tlpLinkedParts.Controls.Add(lbLPCurrent, 0, currentLPEntry + 1);
        }

        private void llLPAdd_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            int tabindex = tlpLinkedParts.RowCount;
            (vpxy.Entries[currentEntry] as VPXY.Entry00).TGIIndexes.Add();
            AddTableRowTBC(tlpLinkedParts, -1, ref tabindex);
            tbc_Enter(lLPtbc[lLPtbc.Count - 1], EventArgs.Empty);
        }

        private void llLPDel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (currentLPEntry == -1) return;
            tlpLinkedParts.Controls.Remove(lbLPCurrent);

            VPXY.Entry00 e00 = vpxy.Entries[currentEntry] as VPXY.Entry00;
            e00.TGIIndexes.RemoveAt(currentLPEntry);

            Control c1 = tlpLinkedParts.GetControlFromPosition(1, currentLPEntry + 1);
            tlpLinkedParts.Controls.Remove(c1);
            lLPtbc.Remove(c1 as TGIBlockCombo);
            for (int i = currentLPEntry; i < e00.TGIIndexes.Count; i++)
            {
                c1 = tlpLinkedParts.GetControlFromPosition(1, i + 2);
                tlpLinkedParts.Controls.Add(c1, 1, i + 1);
            }
            tlpLinkedParts.RowCount--;
            currentLPEntry = -1;
        }

        private void nud_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown nud = sender as NumericUpDown;
            vpxy.BoundingBox[lnud.IndexOf(nud)] = Decimal.ToSingle(nud.Value);
        }

        private void ckbModular_CheckedChanged(object sender, EventArgs e)
        {
            vpxy.Modular = tbcFTPT.Enabled = ckbModular.Checked;
        }

        private void tbcFTPT_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!vpxy.Modular) return;
            vpxy.FTPTIndex = (byte)(tbcFTPT.SelectedIndex > 0 ? tbcFTPT.SelectedIndex : 0);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Environment.ExitCode = 1;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (dirty)
            {
                saveVPXY();
                Environment.ExitCode = 0;
            }
            else
                Environment.ExitCode = 1;
            this.Close();
        }
    }
}
