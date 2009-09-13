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
                TGIBlockCombo c = tlpParts.GetControlFromPosition(2, row) as TGIBlockCombo;
                if (ltbc.IndexOf(c) < 0) continue;
                if (c.SelectedIndex < 0) continue;
                ltgib.Add(vpxy.TGIBlocks[c.SelectedIndex]);
                vpxy.Entries.Add(new VPXY.Entry01(0, null, 1, count++));
                if (c.Tag != null)
                {
                    VPXY.Entry00 e00 = c.Tag as VPXY.Entry00;
                    if (e00.TGIIndexes.Count <= 0) continue;
                    e00.EntryID = count00++;
                    foreach (VPXY.ElementUInt32 elem in e00.TGIIndexes)
                    {
                        ltgib.Add(vpxy.TGIBlocks[(int)elem.Data]);
                        elem.Data = count++;
                    }
                    if (e00.TGIIndexes.Count > 0)
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
            for (int i = 0; i < vpxy.Entries.Count; i++)
            {
                if (vpxy.Entries[i] as VPXY.Entry00 != null)
                {
                    ltbc[ltbc.Count - 1].Tag = vpxy.Entries[i] as VPXY.Entry00;
                }
                else if (vpxy.Entries[i] as VPXY.Entry01 != null)
                {
                    VPXY.Entry01 e01 = vpxy.Entries[i] as VPXY.Entry01;
                    AddTableRowTBC(tlpParts, i, (int)e01.TGIIndex, ref tabindex);
                }
            }
            tlpParts.ResumeLayout();
        }
        void RenumberTLP()
        {
            tlpParts.SuspendLayout();
            int count = 0;
            int row = 1;
            for (int i = 0; i < vpxy.Entries.Count; i++)
            {
                if (vpxy.Entries[i] as VPXY.Entry00 != null)
                {
                    count += (vpxy.Entries[i] as VPXY.Entry00).TGIIndexes.Count;
                }
                else if (vpxy.Entries[i] as VPXY.Entry01 != null)
                {
                    Label lb = tlpParts.GetControlFromPosition(0, row++) as Label;
                    lb.Text = count.ToString();
                    count++;
                }
            }
            tlpParts.ResumeLayout();
        }
        void ClearLinkedPartsTLP()
        {
            if (currentLPEntry != -1 && currentEntry != -1)
            {
                VPXY.Entry00 e00 = ltbc[currentEntry].Tag as VPXY.Entry00;
                e00.TGIIndexes.Clear();
                for (int row = 1; row < tlpLinkedParts.RowCount - 1; row++)
                {
                    TGIBlockCombo c = tlpLinkedParts.GetControlFromPosition(2, row) as TGIBlockCombo;
                    if (lLPtbc.IndexOf(c) < 0) continue;
                    if (c.SelectedIndex < 0) continue;
                    e00.TGIIndexes.Add(new VPXY.ElementUInt32(0, null, (uint)c.SelectedIndex));
                }
                if (e00.TGIIndexes.Count == 0)
                {
                    vpxy.Entries.Remove(e00);
                    ltbc[currentEntry].Tag = null;
                    RenumberTLP();
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
        void FillLinkedPartsTLP(int offset, VPXY.Entry00 entry)
        {
            tlpLinkedParts.SuspendLayout();
            ClearLinkedPartsTLP();
            lLPtbc = new List<TGIBlockCombo>();
            int tabindex = 1;
            for (int i = 0; i < entry.TGIIndexes.Count; i++)
            {
                AddTableRowTBC(tlpLinkedParts, offset + 1 + i, (int)entry.TGIIndexes[i].Data, ref tabindex);
            }
            tlpLPControls.Enabled = tlpLinkedParts.Enabled = true;
            tlpLinkedParts.ResumeLayout();
        }
        void AddTableRowTBC(TableLayoutPanel tlp, int entry, int index, ref int tabindex)
        {
            tlp.RowCount++;
            tlp.RowStyles.Insert(tlp.RowCount - 2, new RowStyle(SizeType.AutoSize));

            Label lb = new Label();
            TGIBlockCombo tbc = new TGIBlockCombo(vpxy.TGIBlocks, index);

            lb.Anchor = AnchorStyles.None;
            lb.AutoSize = true;
            lb.BorderStyle = BorderStyle.Fixed3D;
            lb.FlatStyle = FlatStyle.Standard;
            lb.Margin = new Padding(0);
            lb.Name = "lbEntry" + tabindex;
            lb.TabIndex++;
            lb.Text = entry.ToString();
            lb.Tag = tbc;
            lb.Click += new EventHandler(lb_Click);
            tlp.Controls.Add(lb, 0, tlp.RowCount - 2);

            tbc.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tbc.Name = "tbc" + tabindex;
            tbc.TabIndex = tabindex++;
            tbc.Enter += new EventHandler(tbc_Enter);
            tbc.SelectedIndexChanged += new EventHandler(tbc_SelectedIndexChanged);
            tlp.Controls.Add(tbc, 2, tlp.RowCount - 2);

            if (tlp == tlpParts)
                ltbc.Add(tbc);
            else
                lLPtbc.Add(tbc);

            tbc.TGIBlockListChanged += new EventHandler(tbg_TGIBlockListChanged);
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
                    if (tbc.SelectedIndex == -1 && vpxy.TGIBlocks.Count > 0) tbc.SelectedIndex = vpxy.TGIBlocks.Count - 1;
                }
            if (lLPtbc != null)
                foreach (TGIBlockCombo tbc in lLPtbc)
                {
                    tbc.Refresh();
                    if (tbc.SelectedIndex == -1 && vpxy.TGIBlocks.Count > 0) tbc.SelectedIndex = vpxy.TGIBlocks.Count - 1;
                }
            if (vpxy.Modular)
            {
                tbcFTPT.Refresh();
                if (tbcFTPT.SelectedIndex == -1 && vpxy.TGIBlocks.Count > 0) tbcFTPT.SelectedIndex = vpxy.TGIBlocks.Count - 1;
            }

            btnOK.Enabled = vpxy.TGIBlocks.Count > 0;
        }

        void lb_Click(object sender, EventArgs e) { (((Label)sender).Tag as TGIBlockCombo).Focus(); }

        void tbc_Enter(object sender, EventArgs e)
        {
            TGIBlockCombo tbc = sender as TGIBlockCombo;

            if (ltbc.Contains(tbc))
            {
                ClearLinkedPartsTLP();//before currentEntry changes
                currentEntry = ltbc.IndexOf(tbc);
                tlpParts.Controls.Add(lbCurrentPart, 1, currentEntry + 1);

                if (tbc.Tag != null)
                {
                    btnAddLinked.Enabled = false;
                    int ec = int.Parse(((Label)tlpParts.GetControlFromPosition(0, tlpParts.GetCellPosition(ltbc[currentEntry]).Row)).Text);
                    FillLinkedPartsTLP(ec, tbc.Tag as VPXY.Entry00);
                }
                else
                    btnAddLinked.Enabled = true;
            }
            else
            {
                currentLPEntry = lLPtbc.IndexOf(tbc);
                tlpLinkedParts.Controls.Add(lbLPCurrent, 1, currentLPEntry + 1);
            }
        }

        void tbc_SelectedIndexChanged(object sender, EventArgs e)
        {
            TGIBlockCombo tbc = sender as TGIBlockCombo;

            if (ltbc.Contains(tbc))
            {
                int i = int.Parse(((Label)tlpParts.GetControlFromPosition(0, tlpParts.GetCellPosition(tbc).Row)).Text);
                VPXY.Entry01 e01 = vpxy.Entries[i] as VPXY.Entry01;
                e01.TGIIndex = (tbc.SelectedIndex >= 0) ? (uint)tbc.SelectedIndex : 0;
            }
            else
            {
                if (currentEntry == -1) return;
                VPXY.Entry00 e00 = ltbc[currentEntry].Tag as VPXY.Entry00;
                int i = lLPtbc.IndexOf(tbc);
                e00.TGIIndexes[i].Data = (tbc.SelectedIndex >= 0) ? (uint)tbc.SelectedIndex : 0;
            }
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
            if (currentEntry < 1 || ltbc.Count < 2) return;

            int ec = int.Parse(((Label)tlpParts.GetControlFromPosition(0, tlpParts.GetCellPosition(ltbc[currentEntry]).Row)).Text);
            VPXY.Entry entry = vpxy.Entries[ec];
            vpxy.Entries.RemoveAt(ec);
            if (ltbc[currentEntry].Tag != null) vpxy.Entries.RemoveAt(ec);
            vpxy.Entries.Insert(ec - 1, entry);
            if (ltbc[currentEntry].Tag != null) vpxy.Entries.Insert(ec - 1, ltbc[currentEntry].Tag as VPXY.Entry00);

            TGIBlockCombo c1 = tlpParts.GetControlFromPosition(2, currentEntry + 1) as TGIBlockCombo;//this control
            TGIBlockCombo c2 = tlpParts.GetControlFromPosition(2, currentEntry) as TGIBlockCombo;//the one above to swap with
            tlpParts.Controls.Remove(c1);//leaves currentEntry + 1 free
            tlpParts.Controls.Add(c2, 2, currentEntry + 1);//leaves currentEntry free
            tlpParts.Controls.Add(c1, 2, currentEntry);
            c2.TabIndex--;
            c1.TabIndex++;

            c1 = ltbc[ec];
            ltbc.RemoveAt(ec);
            ltbc.Insert(ec - 1, c1);

            currentEntry--;
            tlpParts.Controls.Add(lbCurrentPart, 1, currentEntry + 1);
            RenumberTLP();
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            if (currentEntry == ltbc.Count - 1 || ltbc.Count < 2) return;

            int ec = int.Parse(((Label)tlpParts.GetControlFromPosition(0, tlpParts.GetCellPosition(ltbc[currentEntry]).Row)).Text);
            VPXY.Entry entry = vpxy.Entries[ec];
            vpxy.Entries.RemoveAt(ec);
            if (ltbc[currentEntry].Tag != null) vpxy.Entries.RemoveAt(ec);
            vpxy.Entries.Insert(currentEntry + 1, entry);
            if (ltbc[currentEntry].Tag != null) vpxy.Entries.Insert(ec + 2, ltbc[currentEntry].Tag as VPXY.Entry00);

            TGIBlockCombo c1 = tlpParts.GetControlFromPosition(2, currentEntry + 1) as TGIBlockCombo;//this control
            TGIBlockCombo c2 = tlpParts.GetControlFromPosition(2, currentEntry + 2) as TGIBlockCombo;//the one below to swap with
            tlpParts.Controls.Remove(c1);//leaves currentEntry + 1 free
            tlpParts.Controls.Add(c2, 2, currentEntry + 1);//leaves currentEntry + 2 free
            tlpParts.Controls.Add(c1, 2, currentEntry + 2);
            c2.TabIndex++;
            c1.TabIndex--;

            c1 = ltbc[ec];
            ltbc.RemoveAt(ec);
            ltbc.Insert(ec + 1, c1);

            currentEntry++;
            tlpParts.Controls.Add(lbCurrentPart, 1, currentEntry + 1);
            RenumberTLP();
        }

        private void btnAddPart_Click(object sender, EventArgs e)
        {
            int ec = vpxy.Entries.Count;
            int tabindex = tlpParts.RowCount;
            vpxy.Entries.Add(new VPXY.Entry01(0, null, 1, 0));
            AddTableRowTBC(tlpParts, ec, -1, ref tabindex);
            tbc_Enter(ltbc[ltbc.Count - 1], EventArgs.Empty);
        }

        private void btnAddLinked_Click(object sender, EventArgs e)
        {
            int ec = int.Parse(((Label)tlpParts.GetControlFromPosition(0, tlpParts.GetCellPosition(ltbc[currentEntry]).Row)).Text);
            VPXY.Entry00 e00 = new VPXY.Entry00(0, null, 0, (byte)vpxy.Entries.Count, new List<VPXY.ElementUInt32>());
            vpxy.Entries.Insert(ec + 1, e00);
            ltbc[ec].Tag = e00;

            RenumberTLP();
            tbc_Enter(ltbc[currentEntry], EventArgs.Empty);
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (currentEntry == -1) return;
            ClearLinkedPartsTLP();
            tlpParts.Controls.Remove(lbCurrentPart);

            int ec = int.Parse(((Label)tlpParts.GetControlFromPosition(0, tlpParts.GetCellPosition(ltbc[currentEntry]).Row)).Text);
            VPXY.Entry entry = vpxy.Entries[ec];
            vpxy.Entries.RemoveAt(ec);
            if (ltbc[currentEntry].Tag != null) vpxy.Entries.RemoveAt(ec);

            Control c1 = tlpParts.GetControlFromPosition(0, currentEntry + 1);
            tlpParts.Controls.Remove(c1);
            c1 = tlpParts.GetControlFromPosition(2, currentEntry + 1);
            tlpParts.Controls.Remove(c1);
            ltbc.RemoveAt(ec);

            for (int i = currentEntry; i < ltbc.Count - 1; i++)
            {
                c1 = tlpParts.GetControlFromPosition(2, i + 2);
                c1.TabIndex--;
                tlpParts.Controls.Add(c1, 1, i + 1);
            }
            tlpParts.RowCount--;

            currentEntry = -1;
            RenumberTLP();
        }

        private void btnLPUp_Click(object sender, EventArgs e)
        {
            if (currentEntry == -1) return;
            VPXY.Entry00 e00 = ltbc[currentEntry].Tag as VPXY.Entry00;
            if (currentLPEntry < 1 || e00.TGIIndexes.Count < 2) return;

            VPXY.ElementUInt32 element = e00.TGIIndexes[currentLPEntry];
            e00.TGIIndexes.RemoveAt(currentLPEntry);
            e00.TGIIndexes.Insert(currentLPEntry - 1, element);
            Control c1 = tlpLinkedParts.GetControlFromPosition(2, currentLPEntry + 1);//this control
            Control c2 = tlpLinkedParts.GetControlFromPosition(2, currentLPEntry);//the one above to swap with
            tlpLinkedParts.Controls.Remove(c1);//leaves currentEntry + 1 free
            tlpLinkedParts.Controls.Add(c2, 2, currentLPEntry + 1);//leaves currentEntry free
            tlpLinkedParts.Controls.Add(c1, 2, currentLPEntry);
            int i = lLPtbc.IndexOf(c1 as TGIBlockCombo);
            lLPtbc.RemoveAt(i);
            lLPtbc.Insert(i - 1, c1 as TGIBlockCombo);
            currentLPEntry--;
            tlpLinkedParts.Controls.Add(lbLPCurrent, 1, currentLPEntry + 1);
            RenumberTLP();
        }

        private void btnLPDown_Click(object sender, EventArgs e)
        {
            if (currentEntry == -1) return;
            VPXY.Entry00 e00 = ltbc[currentEntry].Tag as VPXY.Entry00;
            if (currentLPEntry == e00.TGIIndexes.Count - 1 || e00.TGIIndexes.Count < 2) return;

            VPXY.ElementUInt32 element = e00.TGIIndexes[currentLPEntry];
            e00.TGIIndexes.RemoveAt(currentLPEntry);
            e00.TGIIndexes.Insert(currentLPEntry + 1, element);
            Control c1 = tlpLinkedParts.GetControlFromPosition(2, currentLPEntry + 1);//this control
            Control c2 = tlpLinkedParts.GetControlFromPosition(2, currentLPEntry + 2);//the one below to swap with
            tlpLinkedParts.Controls.Remove(c1);//leaves currentEntry + 1 free
            tlpLinkedParts.Controls.Add(c2, 2, currentLPEntry + 1);//leaves currentEntry + 2 free
            tlpLinkedParts.Controls.Add(c1, 2, currentLPEntry + 2);
            int i = lLPtbc.IndexOf(c1 as TGIBlockCombo);
            lLPtbc.RemoveAt(i);
            lLPtbc.Insert(i + 1, c1 as TGIBlockCombo);
            currentLPEntry++;
            tlpLinkedParts.Controls.Add(lbLPCurrent, 1, currentLPEntry + 1);
            RenumberTLP();
        }

        private void btnLPAdd_Click(object sender, EventArgs e)
        {
            int ec = int.Parse(((Label)tlpParts.GetControlFromPosition(0, tlpParts.GetCellPosition(ltbc[currentEntry]).Row)).Text);
            int tabindex = tlpLinkedParts.RowCount;
            VPXY.Entry00 e00 = ltbc[currentEntry].Tag as VPXY.Entry00;
            e00.TGIIndexes.Add();
            AddTableRowTBC(tlpLinkedParts, ec + e00.TGIIndexes.Count, -1, ref tabindex);
            tbc_Enter(lLPtbc[lLPtbc.Count - 1], EventArgs.Empty);
            RenumberTLP();
        }

        private void btnLPDel_Click(object sender, EventArgs e)
        {
            if (currentLPEntry == -1) return;
            tlpLinkedParts.Controls.Remove(lbLPCurrent);

            VPXY.Entry00 e00 = ltbc[currentEntry].Tag as VPXY.Entry00;
            e00.TGIIndexes.RemoveAt(currentLPEntry);

            Control c1 = tlpLinkedParts.GetControlFromPosition(0, currentLPEntry + 1);
            tlpLinkedParts.Controls.Remove(c1);
            c1 = tlpLinkedParts.GetControlFromPosition(2, currentLPEntry + 1);
            tlpLinkedParts.Controls.Remove(c1);
            lLPtbc.Remove(c1 as TGIBlockCombo);
            for (int i = currentLPEntry; i < e00.TGIIndexes.Count; i++)
            {
                c1 = tlpLinkedParts.GetControlFromPosition(2, i + 2);
                tlpLinkedParts.Controls.Add(c1, 1, i + 1);
            }
            tlpLinkedParts.RowCount--;
            currentLPEntry = -1;
            RenumberTLP();
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

        private void btnEditTGIs_Click(object sender, EventArgs e)
        {
            DialogResult dr = TGIBlockListEditor.Show(this, vpxy.TGIBlocks);
            if (dr != DialogResult.OK) return;
            tbg_TGIBlockListChanged(sender, e);
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
