using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using s3pi.Interfaces;

namespace ObjectCloner.CustomControls
{
    public partial class Thumbnail : UserControl
    {
        public Thumbnail()
        {
            InitializeComponent();
        }

        private Image currentImage = null;
        public Image Image
        {
            get { return currentImage; }
            set
            {
                if (currentImage != value)
                {
                    currentImage = value;
                    if (currentImage != null)
                    {
                        pbThumbnail.Image = MainForm.ResizeImage(currentImage, pbThumbnail);
                        saveToolStripMenuItem.Enabled = btnExportThumb.Enabled = true;
                    }
                    else
                    {
                        pbThumbnail.Image = null;
                        saveToolStripMenuItem.Enabled = btnExportThumb.Enabled = false;
                    }
                    IsReplaced = false;
                }
            }
        }

        private IResourceKey resourceKey;
        public IResourceKey ResourceKey
        {
            get { return resourceKey; }
            set
            {
                if (resourceKey != value)
                {
                    resourceKey = value;
                    lbThumbnailRK.Text = "" + resourceKey;
                    replaceToolStripMenuItem.Enabled = btnImportThumb.Enabled = resourceKey.ResourceType != 0;
                }
            }
        }

        public bool IsReplaced
        {
            get;
            set;
        }

        private void btnExportThumb_Click(object sender, EventArgs e)
        {
            saveThumbnailDialog.FileName = resourceKey + ".png";
            DialogResult dr = saveThumbnailDialog.ShowDialog();
            if (dr != DialogResult.OK)
                return;

            exportThumb(System.IO.Path.GetFullPath(saveThumbnailDialog.FileName));
        }

        private void btnImportThumb_Click(object sender, EventArgs e)
        {
            Image rep = getReplacementForThumbs();
            if (rep != null)
            {
                pbThumbnail.Image = currentImage = MainForm.ResizeImage(rep, pbThumbnail);
                IsReplaced = true;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e) { btnExportThumb_Click(sender, e); }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e) { btnImportThumb_Click(sender, e); }

        //--

        Image getReplacementForThumbs()
        {
            openThumbnailDialog.FilterIndex = 1;
            openThumbnailDialog.FileName = "*.PNG";
            DialogResult dr = openThumbnailDialog.ShowDialog();
            if (dr != DialogResult.OK) return null;
            try
            {
                return Image.FromFile(openThumbnailDialog.FileName, true);
            }
            catch (Exception ex)
            {
                CopyableMessageBox.IssueException(ex, "Could not read thumbnail:\n" + openThumbnailDialog.FileName, openThumbnailDialog.Title);
                return null;
            }
        }

        void exportThumb(string filename)
        {
            currentImage.Save(filename);
        }
    }
}
