using System;
using System.Collections.Generic;
using System.Windows.Forms;
using s3pi.Interfaces;
using s3pi.GenericRCOLResource;

namespace s3pe_VPXY_Resource_Editor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(params string[] args)
        {
#if DEBUG
            if (args.Length == 0)
            {
                AResource.TGIBlock tgib = new AResource.TGIBlock(0, null, "ITG", 0x736884F1, 0, 0);
                ARCOLBlock rcol = GenericRCOLResourceHandler.CreateRCOLBlock(0, null, 0x736884F1);
                GenericRCOLResource.ChunkEntry ce = new GenericRCOLResource.ChunkEntry(0, null, tgib, rcol);
                GenericRCOLResource grr = new GenericRCOLResource(0, null);
                grr.ChunkEntries.Add(ce);
                Clipboard.SetData(DataFormats.Serializable, grr.Stream);
            }
#endif
            return s3pi.DemoPlugins.RunHelper.Run(typeof(MainForm), args);
        }
    }
}
