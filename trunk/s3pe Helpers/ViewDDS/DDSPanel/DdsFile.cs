//------------------------------------------------------------------------------
/*
	@brief		DDS File Type Plugin for Paint.NET

	@note		Copyright (c) 2006 Dean Ashton         http://www.dmashton.co.uk

	Permission is hereby granted, free of charge, to any person obtaining
	a copy of this software and associated documentation files (the 
	"Software"), to	deal in the Software without restriction, including
	without limitation the rights to use, copy, modify, merge, publish,
	distribute, sublicense, and/or sell copies of the Software, and to 
	permit persons to whom the Software is furnished to do so, subject to 
	the following conditions:

	The above copyright notice and this permission notice shall be included
	in all copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
	OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
	MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
	IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
	CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
	TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
	SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
**/
/***************************************************************************
 *  This derivative of the DDS File Type Plugin for Paint.Net is           *
 *  distributed as part of sims3tools                                      *
 *                                                                         *
 *  Copyright (C) 2011 by Peter L Jones                                    *
 *  pljones@users.sf.net                                                   *
 ***************************************************************************/
//------------------------------------------------------------------------------

// If we want to do the alignment as per the (broken) DDS documentation, then we
// uncomment this define.. 
//#define	APPLY_PITCH_ALIGNMENT

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
//using PaintDotNet;
using System.Drawing;

namespace DdsFileTypePlugin
{
    enum DdsFileFormat
    {
        DDS_FORMAT_DXT1,
        DDS_FORMAT_DXT3,
        DDS_FORMAT_DXT5,
        DDS_FORMAT_A8R8G8B8,
        DDS_FORMAT_X8R8G8B8,
        DDS_FORMAT_A8B8G8R8,
        DDS_FORMAT_X8B8G8R8,
        DDS_FORMAT_A1R5G5B5,
        DDS_FORMAT_A4R4G4B4,
        DDS_FORMAT_R8G8B8,
        DDS_FORMAT_R5G6B5,

        DDS_FORMAT_INVALID,
    }

    class DdsPixelFormat
    {
        public enum PixelFormatFlags
        {
            DDS_FOURCC = 0x00000004,
            DDS_RGB = 0x00000040,
            DDS_RGBA = 0x00000041,
        }

        public uint m_size;
        public uint m_flags;
        public uint m_fourCC;
        public uint m_rgbBitCount;
        public uint m_rBitMask;
        public uint m_gBitMask;
        public uint m_bBitMask;
        public uint m_aBitMask;

        public uint Size()
        {
            return 8 * 4;
        }

        public void Initialise(DdsFileFormat fileFormat)
        {
            m_size = Size();
            switch (fileFormat)
            {
                case DdsFileFormat.DDS_FORMAT_DXT1:
                case DdsFileFormat.DDS_FORMAT_DXT3:
                case DdsFileFormat.DDS_FORMAT_DXT5:
                    {
                        // DXT1/DXT3/DXT5
                        m_flags = (int)PixelFormatFlags.DDS_FOURCC;
                        m_rgbBitCount = 0;
                        m_rBitMask = 0;
                        m_gBitMask = 0;
                        m_bBitMask = 0;
                        m_aBitMask = 0;
                        if (fileFormat == DdsFileFormat.DDS_FORMAT_DXT1) m_fourCC = 0x31545844;	//"DXT1"
                        if (fileFormat == DdsFileFormat.DDS_FORMAT_DXT3) m_fourCC = 0x33545844;	//"DXT1"
                        if (fileFormat == DdsFileFormat.DDS_FORMAT_DXT5) m_fourCC = 0x35545844;	//"DXT1"
                        break;
                    }

                case DdsFileFormat.DDS_FORMAT_A8R8G8B8:
                    {
                        m_flags = (int)PixelFormatFlags.DDS_RGBA;
                        m_rgbBitCount = 32;
                        m_fourCC = 0;
                        m_rBitMask = 0x00ff0000;
                        m_gBitMask = 0x0000ff00;
                        m_bBitMask = 0x000000ff;
                        m_aBitMask = 0xff000000;
                        break;
                    }

                case DdsFileFormat.DDS_FORMAT_X8R8G8B8:
                    {
                        m_flags = (int)PixelFormatFlags.DDS_RGB;
                        m_rgbBitCount = 32;
                        m_fourCC = 0;
                        m_rBitMask = 0x00ff0000;
                        m_gBitMask = 0x0000ff00;
                        m_bBitMask = 0x000000ff;
                        m_aBitMask = 0x00000000;
                        break;
                    }

                case DdsFileFormat.DDS_FORMAT_A8B8G8R8:
                    {
                        m_flags = (int)PixelFormatFlags.DDS_RGBA;
                        m_rgbBitCount = 32;
                        m_fourCC = 0;
                        m_rBitMask = 0x000000ff;
                        m_gBitMask = 0x0000ff00;
                        m_bBitMask = 0x00ff0000;
                        m_aBitMask = 0xff000000;
                        break;
                    }

                case DdsFileFormat.DDS_FORMAT_X8B8G8R8:
                    {
                        m_flags = (int)PixelFormatFlags.DDS_RGB;
                        m_rgbBitCount = 32;
                        m_fourCC = 0;
                        m_rBitMask = 0x000000ff;
                        m_gBitMask = 0x0000ff00;
                        m_bBitMask = 0x00ff0000;
                        m_aBitMask = 0x00000000;
                        break;
                    }

                case DdsFileFormat.DDS_FORMAT_A1R5G5B5:
                    {
                        m_flags = (int)PixelFormatFlags.DDS_RGBA;
                        m_rgbBitCount = 16;
                        m_fourCC = 0;
                        m_rBitMask = 0x00007c00;
                        m_gBitMask = 0x000003e0;
                        m_bBitMask = 0x0000001f;
                        m_aBitMask = 0x00008000;
                        break;
                    }

                case DdsFileFormat.DDS_FORMAT_A4R4G4B4:
                    {
                        m_flags = (int)PixelFormatFlags.DDS_RGBA;
                        m_rgbBitCount = 16;
                        m_fourCC = 0;
                        m_rBitMask = 0x00000f00;
                        m_gBitMask = 0x000000f0;
                        m_bBitMask = 0x0000000f;
                        m_aBitMask = 0x0000f000;
                        break;
                    }

                case DdsFileFormat.DDS_FORMAT_R8G8B8:
                    {
                        m_flags = (int)PixelFormatFlags.DDS_RGB;
                        m_fourCC = 0;
                        m_rgbBitCount = 24;
                        m_rBitMask = 0x00ff0000;
                        m_gBitMask = 0x0000ff00;
                        m_bBitMask = 0x000000ff;
                        m_aBitMask = 0x00000000;
                        break;
                    }

                case DdsFileFormat.DDS_FORMAT_R5G6B5:
                    {
                        m_flags = (int)PixelFormatFlags.DDS_RGB;
                        m_fourCC = 0;
                        m_rgbBitCount = 16;
                        m_rBitMask = 0x0000f800;
                        m_gBitMask = 0x000007e0;
                        m_bBitMask = 0x0000001f;
                        m_aBitMask = 0x00000000;
                        break;
                    }

                default:
                    break;
            }
        }

        public void Read(BinaryReader input)
        {
            this.m_size = input.ReadUInt32();
            this.m_flags = input.ReadUInt32();
            this.m_fourCC = input.ReadUInt32();
            this.m_rgbBitCount = input.ReadUInt32();
            this.m_rBitMask = input.ReadUInt32();
            this.m_gBitMask = input.ReadUInt32();
            this.m_bBitMask = input.ReadUInt32();
            this.m_aBitMask = input.ReadUInt32();
        }

        public void Write(BinaryWriter output)
        {
            output.Write(this.m_size);
            output.Write(this.m_flags);
            output.Write(this.m_fourCC);
            output.Write(this.m_rgbBitCount);
            output.Write(this.m_rBitMask);
            output.Write(this.m_gBitMask);
            output.Write(this.m_bBitMask);
            output.Write(this.m_aBitMask);
        }
    }

    class DdsHeader
    {
        public enum HeaderFlags
        {
            DDS_HEADER_FLAGS_TEXTURE = 0x00001007,	// DDSD_CAPS | DDSD_HEIGHT | DDSD_WIDTH | DDSD_PIXELFORMAT 
            DDS_HEADER_FLAGS_MIPMAP = 0x00020000,	// DDSD_MIPMAPCOUNT
            DDS_HEADER_FLAGS_VOLUME = 0x00800000,	// DDSD_DEPTH
            DDS_HEADER_FLAGS_PITCH = 0x00000008,	// DDSD_PITCH
            DDS_HEADER_FLAGS_LINEARSIZE = 0x00080000,	// DDSD_LINEARSIZE
        }

        public enum SurfaceFlags
        {
            DDS_SURFACE_FLAGS_TEXTURE = 0x00001000,	// DDSCAPS_TEXTURE
            DDS_SURFACE_FLAGS_MIPMAP = 0x00400008,	// DDSCAPS_COMPLEX | DDSCAPS_MIPMAP
            DDS_SURFACE_FLAGS_CUBEMAP = 0x00000008,	// DDSCAPS_COMPLEX
        }

        public enum CubemapFlags
        {
            DDS_CUBEMAP_POSITIVEX = 0x00000600, // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_POSITIVEX
            DDS_CUBEMAP_NEGATIVEX = 0x00000a00, // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_NEGATIVEX
            DDS_CUBEMAP_POSITIVEY = 0x00001200, // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_POSITIVEY
            DDS_CUBEMAP_NEGATIVEY = 0x00002200, // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_NEGATIVEY
            DDS_CUBEMAP_POSITIVEZ = 0x00004200, // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_POSITIVEZ
            DDS_CUBEMAP_NEGATIVEZ = 0x00008200, // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_NEGATIVEZ

            DDS_CUBEMAP_ALLFACES = (DDS_CUBEMAP_POSITIVEX | DDS_CUBEMAP_NEGATIVEX |
                                                DDS_CUBEMAP_POSITIVEY | DDS_CUBEMAP_NEGATIVEY |
                                                DDS_CUBEMAP_POSITIVEZ | DDS_CUBEMAP_NEGATIVEZ)
        }

        public enum VolumeFlags
        {
            DDS_FLAGS_VOLUME = 0x00200000,	// DDSCAPS2_VOLUME
        }

        public DdsHeader()
        {
            m_pixelFormat = new DdsPixelFormat();
        }

        public uint Size()
        {
            return (18 * 4) + m_pixelFormat.Size() + (5 * 4);
        }

        public uint m_size;
        public uint m_headerFlags;
        public uint m_height;
        public uint m_width;
        public uint m_pitchOrLinearSize;
        public uint m_depth;
        public uint m_mipMapCount;
        public uint m_reserved1_0;
        public uint m_reserved1_1;
        public uint m_reserved1_2;
        public uint m_reserved1_3;
        public uint m_reserved1_4;
        public uint m_reserved1_5;
        public uint m_reserved1_6;
        public uint m_reserved1_7;
        public uint m_reserved1_8;
        public uint m_reserved1_9;
        public uint m_reserved1_10;
        public DdsPixelFormat m_pixelFormat;
        public uint m_surfaceFlags;
        public uint m_cubemapFlags;
        public uint m_reserved2_0;
        public uint m_reserved2_1;
        public uint m_reserved2_2;

        public void Read(System.IO.Stream input)
        {
            BinaryReader br = new BinaryReader(input);

            this.m_size = br.ReadUInt32();
            this.m_headerFlags = br.ReadUInt32();
            this.m_height = br.ReadUInt32();
            this.m_width = br.ReadUInt32();
            this.m_pitchOrLinearSize = br.ReadUInt32();
            this.m_depth = br.ReadUInt32();
            this.m_mipMapCount = br.ReadUInt32();
            this.m_reserved1_0 = br.ReadUInt32();
            this.m_reserved1_1 = br.ReadUInt32();
            this.m_reserved1_2 = br.ReadUInt32();
            this.m_reserved1_3 = br.ReadUInt32();
            this.m_reserved1_4 = br.ReadUInt32();
            this.m_reserved1_5 = br.ReadUInt32();
            this.m_reserved1_6 = br.ReadUInt32();
            this.m_reserved1_7 = br.ReadUInt32();
            this.m_reserved1_8 = br.ReadUInt32();
            this.m_reserved1_9 = br.ReadUInt32();
            this.m_reserved1_10 = br.ReadUInt32();
            this.m_pixelFormat.Read(br);
            this.m_surfaceFlags = br.ReadUInt32();
            this.m_cubemapFlags = br.ReadUInt32();
            this.m_reserved2_0 = br.ReadUInt32();
            this.m_reserved2_1 = br.ReadUInt32();
            this.m_reserved2_2 = br.ReadUInt32();

        }

        public void Write(Stream output)
        {
            BinaryWriter writer = new BinaryWriter(output);
            writer.Write(this.m_size);
            writer.Write(this.m_headerFlags);
            writer.Write(this.m_height);
            writer.Write(this.m_width);
            writer.Write(this.m_pitchOrLinearSize);
            writer.Write(this.m_depth);
            writer.Write(this.m_mipMapCount);
            writer.Write(this.m_reserved1_0);
            writer.Write(this.m_reserved1_1);
            writer.Write(this.m_reserved1_2);
            writer.Write(this.m_reserved1_3);
            writer.Write(this.m_reserved1_4);
            writer.Write(this.m_reserved1_5);
            writer.Write(this.m_reserved1_6);
            writer.Write(this.m_reserved1_7);
            writer.Write(this.m_reserved1_8);
            writer.Write(this.m_reserved1_9);
            writer.Write(this.m_reserved1_10);
            this.m_pixelFormat.Write(writer);
            writer.Write(this.m_surfaceFlags);
            writer.Write(this.m_cubemapFlags);
            writer.Write(this.m_reserved2_0);
            writer.Write(this.m_reserved2_1);
            writer.Write(this.m_reserved2_2);
        }
    }

    /// <summary>
    /// Represents an image encoded using one of the supported DDS mechanisms.
    /// </summary>
    public class DdsFile : IDisposable
    {
        /// <summary>
        /// Loads the data from an image encoded using one of the supported DDS mechanisms.
        /// If <paramref name="supportHSV"/> is true, also creates an HSVa-encoded version of the image.
        /// </summary>
        /// <param name="input">A <see cref="System.IO.Stream"/> containing the DDS-encoded image.</param>
        /// <param name="supportHSV">When true, create an HSVa-encoded version of the image.</param>
        public void Load(System.IO.Stream input, bool supportHSV)
        {
            // Read the DDS tag. If it's not right, then bail.. 
            uint ddsTag = new BinaryReader(input).ReadUInt32();
            if (ddsTag != 0x20534444)
                throw new FormatException("File does not appear to be a DDS image");

            // Read everything in.. for now assume it worked like a charm..
            m_header.Read(input);

            if ((m_header.m_pixelFormat.m_flags & (int)DdsPixelFormat.PixelFormatFlags.DDS_FOURCC) != 0)
            {
                #region We can use squish
                int squishFlags = 0;

                switch (m_header.m_pixelFormat.m_fourCC)
                {
                    case 0x31545844:
                        squishFlags = (int)DdsSquish.SquishFlags.kDxt1;
                        break;

                    case 0x33545844:
                        squishFlags = (int)DdsSquish.SquishFlags.kDxt3;
                        break;

                    case 0x35545844:
                        squishFlags = (int)DdsSquish.SquishFlags.kDxt5;
                        break;

                    default:
                        throw new FormatException("File is not a squish-supported DDS format");
                }

                // Compute size of compressed block area
                int blockCount = ((GetWidth() + 3) / 4) * ((GetHeight() + 3) / 4);
                int blockSize = ((squishFlags & (int)DdsSquish.SquishFlags.kDxt1) != 0) ? 8 : 16;

                // Allocate room for compressed blocks, and read data into it.
                byte[] compressedBlocks = new byte[blockCount * blockSize];
                input.Read(compressedBlocks, 0, compressedBlocks.GetLength(0));

                // Now decompress..
                m_pixelData = DdsSquish.DecompressImage(compressedBlocks, GetWidth(), GetHeight(), squishFlags);
                #endregion
            }
            else
            {
                #region It's a non-squishable one, so try manual methods...
                #region ...determine which...
                // We can only deal with the non-DXT formats we know about..  this is a bit of a mess..
                // Sorry..
                DdsFileFormat fileFormat = DdsFileFormat.DDS_FORMAT_INVALID;
                pixelSet setPixel = null;

                if ((m_header.m_pixelFormat.m_flags == (int)DdsPixelFormat.PixelFormatFlags.DDS_RGBA) &&
                        (m_header.m_pixelFormat.m_rgbBitCount == 32) &&
                        (m_header.m_pixelFormat.m_rBitMask == 0x00ff0000) && (m_header.m_pixelFormat.m_gBitMask == 0x0000ff00) &&
                        (m_header.m_pixelFormat.m_bBitMask == 0x000000ff) && (m_header.m_pixelFormat.m_aBitMask == 0xff000000))
                {
                    fileFormat = DdsFileFormat.DDS_FORMAT_A8R8G8B8;
                    setPixel = pixelSetDDS_FORMAT_A8R8G8B8;
                }
                else if ((m_header.m_pixelFormat.m_flags == (int)DdsPixelFormat.PixelFormatFlags.DDS_RGB) &&
                            (m_header.m_pixelFormat.m_rgbBitCount == 32) &&
                            (m_header.m_pixelFormat.m_rBitMask == 0x00ff0000) && (m_header.m_pixelFormat.m_gBitMask == 0x0000ff00) &&
                            (m_header.m_pixelFormat.m_bBitMask == 0x000000ff) && (m_header.m_pixelFormat.m_aBitMask == 0x00000000))
                {
                    fileFormat = DdsFileFormat.DDS_FORMAT_X8R8G8B8;
                    setPixel = pixelSetDDS_FORMAT_X8R8G8B8;
                }
                else if ((m_header.m_pixelFormat.m_flags == (int)DdsPixelFormat.PixelFormatFlags.DDS_RGBA) &&
                            (m_header.m_pixelFormat.m_rgbBitCount == 32) &&
                            (m_header.m_pixelFormat.m_rBitMask == 0x000000ff) && (m_header.m_pixelFormat.m_gBitMask == 0x0000ff00) &&
                            (m_header.m_pixelFormat.m_bBitMask == 0x00ff0000) && (m_header.m_pixelFormat.m_aBitMask == 0xff000000))
                {
                    fileFormat = DdsFileFormat.DDS_FORMAT_A8B8G8R8;
                    setPixel = pixelSetDDS_FORMAT_A8B8G8R8;
                }
                else if ((m_header.m_pixelFormat.m_flags == (int)DdsPixelFormat.PixelFormatFlags.DDS_RGB) &&
                            (m_header.m_pixelFormat.m_rgbBitCount == 32) &&
                            (m_header.m_pixelFormat.m_rBitMask == 0x000000ff) && (m_header.m_pixelFormat.m_gBitMask == 0x0000ff00) &&
                            (m_header.m_pixelFormat.m_bBitMask == 0x00ff0000) && (m_header.m_pixelFormat.m_aBitMask == 0x00000000))
                {
                    fileFormat = DdsFileFormat.DDS_FORMAT_X8B8G8R8;
                    setPixel = pixelSetDDS_FORMAT_X8B8G8R8;
                }
                else if ((m_header.m_pixelFormat.m_flags == (int)DdsPixelFormat.PixelFormatFlags.DDS_RGBA) &&
                            (m_header.m_pixelFormat.m_rgbBitCount == 16) &&
                            (m_header.m_pixelFormat.m_rBitMask == 0x00007c00) && (m_header.m_pixelFormat.m_gBitMask == 0x000003e0) &&
                            (m_header.m_pixelFormat.m_bBitMask == 0x0000001f) && (m_header.m_pixelFormat.m_aBitMask == 0x00008000))
                {
                    fileFormat = DdsFileFormat.DDS_FORMAT_A1R5G5B5;
                    setPixel = pixelSetDDS_FORMAT_A1R5G5B5;
                }
                else if ((m_header.m_pixelFormat.m_flags == (int)DdsPixelFormat.PixelFormatFlags.DDS_RGBA) &&
                            (m_header.m_pixelFormat.m_rgbBitCount == 16) &&
                            (m_header.m_pixelFormat.m_rBitMask == 0x00000f00) && (m_header.m_pixelFormat.m_gBitMask == 0x000000f0) &&
                            (m_header.m_pixelFormat.m_bBitMask == 0x0000000f) && (m_header.m_pixelFormat.m_aBitMask == 0x0000f000))
                {
                    fileFormat = DdsFileFormat.DDS_FORMAT_A4R4G4B4;
                    setPixel = pixelSetDDS_FORMAT_A4R4G4B4;
                }
                else if ((m_header.m_pixelFormat.m_flags == (int)DdsPixelFormat.PixelFormatFlags.DDS_RGB) &&
                            (m_header.m_pixelFormat.m_rgbBitCount == 24) &&
                            (m_header.m_pixelFormat.m_rBitMask == 0x00ff0000) && (m_header.m_pixelFormat.m_gBitMask == 0x0000ff00) &&
                            (m_header.m_pixelFormat.m_bBitMask == 0x000000ff) && (m_header.m_pixelFormat.m_aBitMask == 0x00000000))
                {
                    fileFormat = DdsFileFormat.DDS_FORMAT_R8G8B8;
                    setPixel = pixelSetDDS_FORMAT_R8G8B8;
                }
                else if ((m_header.m_pixelFormat.m_flags == (int)DdsPixelFormat.PixelFormatFlags.DDS_RGB) &&
                            (m_header.m_pixelFormat.m_rgbBitCount == 16) &&
                            (m_header.m_pixelFormat.m_rBitMask == 0x0000f800) && (m_header.m_pixelFormat.m_gBitMask == 0x000007e0) &&
                            (m_header.m_pixelFormat.m_bBitMask == 0x0000001f) && (m_header.m_pixelFormat.m_aBitMask == 0x00000000))
                {
                    fileFormat = DdsFileFormat.DDS_FORMAT_R5G6B5;
                    setPixel = pixelSetDDS_FORMAT_R5G6B5;
                }

                // If fileFormat is still invalid, then it's an unsupported format.
                if (fileFormat == DdsFileFormat.DDS_FORMAT_INVALID || setPixel == null)
                    throw new FormatException("File is not a supported non-DXT DDS format");
                #endregion

                #region pixel size and row pitch
                // Size of a source pixel, in bytes
                int srcPixelSize = ((int)m_header.m_pixelFormat.m_rgbBitCount / 8);

                // We need the pitch for a row, so we can allocate enough memory for the load.
                int rowPitch = 0;

                if ((m_header.m_headerFlags & (int)DdsHeader.HeaderFlags.DDS_HEADER_FLAGS_PITCH) != 0)
                {
                    // Pitch specified.. so we can use directly
                    rowPitch = (int)m_header.m_pitchOrLinearSize;
                }
                else if ((m_header.m_headerFlags & (int)DdsHeader.HeaderFlags.DDS_HEADER_FLAGS_LINEARSIZE) != 0)
                {
                    // Linear size specified.. compute row pitch. Of course, this should never happen
                    // as linear size is *supposed* to be for compressed textures. But Microsoft don't 
                    // always play by the rules when it comes to DDS output. 
                    rowPitch = (int)m_header.m_pitchOrLinearSize / (int)m_header.m_height;
                }
                else
                {
                    // Another case of Microsoft not obeying their standard is the 'Convert to..' shell extension
                    // that ships in the DirectX SDK. Seems to always leave flags empty..so no indication of pitch
                    // or linear size. And - to cap it all off - they leave pitchOrLinearSize as *zero*. Zero??? If
                    // we get this bizarre set of inputs, we just go 'screw it' and compute row pitch ourselves, 
                    // making sure we DWORD align it (if that code path is enabled).
                    rowPitch = ((int)m_header.m_width * srcPixelSize);

#if	APPLY_PITCH_ALIGNMENT
					rowPitch = ( ( ( int )rowPitch + 3 ) & ( ~3 ) );
#endif	// APPLY_PITCH_ALIGNMENT
                }

                //				System.Diagnostics.Debug.WriteLine( "Image width : " + m_header.m_width + ", rowPitch = " + rowPitch );
                #endregion

                // Ok.. now, we need to allocate room for the bytes to read in from.. it's rowPitch bytes * height
                byte[] readPixelData = new byte[rowPitch * m_header.m_height];
                input.Read(readPixelData, 0, readPixelData.GetLength(0));

                // We now need space for the real pixel data.. that's width * height * 4..
                m_pixelData = new byte[m_header.m_width * m_header.m_height * 4];

                #region fill the pixel data array
                // And now we have the arduous task of filling that up with stuff..
                for (int destY = 0; destY < (int)m_header.m_height; destY++)
                {
                    for (int destX = 0; destX < (int)m_header.m_width; destX++)
                    {
                        // Compute source pixel offset
                        int srcPixelOffset = (destY * rowPitch) + (destX * srcPixelSize);

                        // Read our pixel
                        uint pixelColour = 0;
                        uint pixelRed = 0;
                        uint pixelGreen = 0;
                        uint pixelBlue = 0;
                        uint pixelAlpha = 0;

                        // Build our pixel colour as a DWORD	
                        for (int loop = 0; loop < srcPixelSize; loop++)
                        {
                            pixelColour |= (uint)(readPixelData[srcPixelOffset + loop] << (8 * loop));
                        }

                        // delegate takes care of calculation, set when determining format
                        setPixel(pixelColour, out pixelRed, out pixelGreen, out pixelBlue, out pixelAlpha);

                        // Write the colours away..
                        int destPixelOffset = (destY * (int)m_header.m_width * 4) + (destX * 4);
                        m_pixelData[destPixelOffset + 0] = (byte)pixelRed;
                        m_pixelData[destPixelOffset + 1] = (byte)pixelGreen;
                        m_pixelData[destPixelOffset + 2] = (byte)pixelBlue;
                        m_pixelData[destPixelOffset + 3] = (byte)pixelAlpha;
                    }
                }
                #endregion
                #endregion
            }

            if (supportHSV)
                hsvData = RGBHSV.ColorRGBA.ConvertToColorHSVAArray(m_pixelData);
        }
        #region Supported non-DXT conversion methods
        delegate void pixelSet(uint pixelColour, out uint pixelRed, out uint pixelGreen, out uint pixelBlue, out uint pixelAlpha);
        void pixelSetDDS_FORMAT_A8R8G8B8(uint pixelColour, out uint pixelRed, out uint pixelGreen, out uint pixelBlue, out uint pixelAlpha)
        {
            pixelAlpha = (pixelColour >> 24) & 0xff;
            pixelRed = (pixelColour >> 16) & 0xff;
            pixelGreen = (pixelColour >> 8) & 0xff;
            pixelBlue = (pixelColour >> 0) & 0xff;
        }
        void pixelSetDDS_FORMAT_X8R8G8B8(uint pixelColour, out uint pixelRed, out uint pixelGreen, out uint pixelBlue, out uint pixelAlpha)
        {
            pixelAlpha = 0xff;
            pixelRed = (pixelColour >> 16) & 0xff;
            pixelGreen = (pixelColour >> 8) & 0xff;
            pixelBlue = (pixelColour >> 0) & 0xff;
        }
        void pixelSetDDS_FORMAT_A8B8G8R8(uint pixelColour, out uint pixelRed, out uint pixelGreen, out uint pixelBlue, out uint pixelAlpha)
        {
            pixelAlpha = (pixelColour >> 24) & 0xff;
            pixelRed = (pixelColour >> 0) & 0xff;
            pixelGreen = (pixelColour >> 8) & 0xff;
            pixelBlue = (pixelColour >> 16) & 0xff;
        }
        void pixelSetDDS_FORMAT_X8B8G8R8(uint pixelColour, out uint pixelRed, out uint pixelGreen, out uint pixelBlue, out uint pixelAlpha)
        {
            pixelAlpha = 0xff;
            pixelRed = (pixelColour >> 0) & 0xff;
            pixelGreen = (pixelColour >> 8) & 0xff;
            pixelBlue = (pixelColour >> 16) & 0xff;
        }
        void pixelSetDDS_FORMAT_A1R5G5B5(uint pixelColour, out uint pixelRed, out uint pixelGreen, out uint pixelBlue, out uint pixelAlpha)
        {
            pixelAlpha = (pixelColour >> 15) * 0xff;
            pixelRed = (pixelColour >> 10) & 0x1f;
            pixelGreen = (pixelColour >> 5) & 0x1f;
            pixelBlue = (pixelColour >> 0) & 0x1f;

            pixelRed = (pixelRed << 3) | (pixelRed >> 2);
            pixelGreen = (pixelGreen << 3) | (pixelGreen >> 2);
            pixelBlue = (pixelBlue << 3) | (pixelBlue >> 2);
        }
        void pixelSetDDS_FORMAT_A4R4G4B4(uint pixelColour, out uint pixelRed, out uint pixelGreen, out uint pixelBlue, out uint pixelAlpha)
        {
            pixelAlpha = (pixelColour >> 12) & 0xff;
            pixelRed = (pixelColour >> 8) & 0x0f;
            pixelGreen = (pixelColour >> 4) & 0x0f;
            pixelBlue = (pixelColour >> 0) & 0x0f;

            pixelAlpha = (pixelAlpha << 4) | (pixelAlpha >> 0);
            pixelRed = (pixelRed << 4) | (pixelRed >> 0);
            pixelGreen = (pixelGreen << 4) | (pixelGreen >> 0);
            pixelBlue = (pixelBlue << 4) | (pixelBlue >> 0);
        }
        void pixelSetDDS_FORMAT_R8G8B8(uint pixelColour, out uint pixelRed, out uint pixelGreen, out uint pixelBlue, out uint pixelAlpha)
        {
            pixelAlpha = 0xff;
            pixelRed = (pixelColour >> 16) & 0xff;
            pixelGreen = (pixelColour >> 8) & 0xff;
            pixelBlue = (pixelColour >> 0) & 0xff;
        }
        void pixelSetDDS_FORMAT_R5G6B5(uint pixelColour, out uint pixelRed, out uint pixelGreen, out uint pixelBlue, out uint pixelAlpha)
        {
            pixelAlpha = 0xff;
            pixelRed = (pixelColour >> 11) & 0x1f;
            pixelGreen = (pixelColour >> 5) & 0x3f;
            pixelBlue = (pixelColour >> 0) & 0x1f;

            pixelRed = (pixelRed << 3) | (pixelRed >> 2);
            pixelGreen = (pixelGreen << 2) | (pixelGreen >> 4);
            pixelBlue = (pixelBlue << 3) | (pixelBlue >> 2);
        }
        #endregion

        /// <summary>
        /// Creates an image of a single colour, specified by the byte parameters,
        /// with the size given by the int parameters.
        /// If <paramref name="supportHSV"/> is true, also creates an HSVa-encoded version of the image.
        /// </summary>
        /// <param name="r">Amount of red per pixel.</param>
        /// <param name="g">Amount of green per pixel.</param>
        /// <param name="b">Amount of blue per pixel.</param>
        /// <param name="a">Amount of alpha per pixel.</param>
        /// <param name="width">Width of image.</param>
        /// <param name="height">Height of image.</param>
        /// <param name="supportHSV">When true, create an HSVa-encoded version of the image.</param>
        public void CreateImage(byte r, byte g, byte b, byte a, int width, int height, bool supportHSV)
        {
            m_header = new DdsHeader();
            m_header.m_width = (uint)width;
            m_header.m_height = (uint)height;
            m_header.m_pixelFormat.Initialise(DdsFileFormat.DDS_FORMAT_DXT1);
            m_pixelData = new byte[width * height * 4];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    setPixelRGBA(m_pixelData, pixelOffset(x, y, width), r, g, b, a);

            if (supportHSV)
                hsvData = RGBHSV.ColorRGBA.ConvertToColorHSVAArray(m_pixelData);
        }

        /// <summary>
        /// Creates an image of a single colour, specified by the uint parameter
        /// (low byte is "blue", then "green", then "red" then high byte is "alpha"),
        /// with the size given by the int parameters.
        /// If <paramref name="supportHSV"/> is true, also creates an HSVa-encoded version of the image.
        /// </summary>
        /// <param name="argb">Colour of image (low byte is "blue", then "green", then "red" then high byte is "alpha").</param>
        /// <param name="width">Width of image.</param>
        /// <param name="height">Height of image.</param>
        /// <param name="supportHSV">When true, create an HSVa-encoded version of the image.</param>
        public void CreateImage(uint argb, int width, int height, bool supportHSV)
        {
            CreateImage((byte)((argb >> 16) & 0xff), (byte)((argb >> 8) & 0xff), (byte)(argb & 0xff), (byte)((argb >> 24) & 0xff), width, height, supportHSV);
        }

        /// <summary>
        /// Creates an image from a given <seealso cref="Image"/>.
        /// If <paramref name="supportHSV"/> is true, also creates an HSVa-encoded version of the image.
        /// </summary>
        /// <param name="image"><seealso cref="Image"/> from which to extract image pixel.</param>
        /// <param name="supportHSV">When true, create an HSVa-encoded version of the image.</param>
        public void CreateImage(Image image, bool supportHSV)
        {
            CreateImage(new Bitmap(image), supportHSV);
        }

        /// <summary>
        /// Creates an image from a given <seealso cref="Bitmap"/>.
        /// If <paramref name="supportHSV"/> is true, also creates an HSVa-encoded version of the image.
        /// </summary>
        /// <param name="image"><seealso cref="Bitmap"/> from which to extract image pixel.</param>
        /// <param name="supportHSV">When true, create an HSVa-encoded version of the image.</param>
        public void CreateImage(Bitmap image, bool supportHSV)
        {
            m_header = new DdsHeader();
            m_header.m_width = (uint)image.Width;
            m_header.m_height = (uint)image.Height;
            m_header.m_pixelFormat.Initialise(DdsFileFormat.DDS_FORMAT_DXT1);
            m_pixelData = new byte[image.Width * image.Height * 4];
            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                    setPixelARGB(m_pixelData, pixelOffset(x, y, image.Width), (uint)image.GetPixel(x, y).ToArgb());

            if (supportHSV)
                hsvData = RGBHSV.ColorRGBA.ConvertToColorHSVAArray(m_pixelData);
        }

        void setPixelRGBA(byte[] pixelData, int offset, byte r, byte g, byte b, byte a)
        {
            pixelData[offset + 0] = r;
            pixelData[offset + 1] = g;
            pixelData[offset + 2] = b;
            pixelData[offset + 3] = a;
        }

        void setPixelARGB(byte[] pixelData, int offset, uint argb)
        {
            setPixelRGBA(pixelData, offset, (byte)((argb >> 16) & 0xff), (byte)((argb >> 8) & 0xff), (byte)(argb & 0xff), (byte)((argb >> 24) & 0xff));
        }

        private void copyPixel(byte[] outPixelData, int offset, byte[] inPixelData)
        {
            for (int i = 0; i < 3; i++)
            {
                outPixelData[offset + i] = inPixelData[offset + i];
            }
        }

        int pixelOffset(int x, int y, int width) { return (y * width + x) * 4; }

        ///  <summary>
        ///  Saves the current image using one of the supported DDS mechanisms.
        ///  </summary>
        ///  <param name="output">A <see cref="T:System.IO.Stream"/> to receive the DDS-encoded image.</param>
        ///  <param name="m_flags">Optional; when null (or omitted), uses the value from the loaded image; currently only 0x04 is supported.</param>
        ///  <param name="m_fourCC">Optional; when null (or omitted), uses the value from the loaded image; FourCC tags DXT1, DXT3 and DXT5 are supported.</param>
        public void Save(Stream output, uint? m_flags = null, uint? m_fourCC = null)
        {
            byte[] buffer2;
            byte[] pixelData = this.GetPixelData();
            if (!m_flags.HasValue) m_flags =  this.m_header.m_pixelFormat.m_flags;
            if (!m_fourCC.HasValue) m_fourCC = this.m_header.m_pixelFormat.m_fourCC;

            if ((m_flags & (int)DdsPixelFormat.PixelFormatFlags.DDS_FOURCC) == 0)
                throw new FormatException("Non-squish compression is not currently supported");

            int flags = 0;
            uint valueOrDefault = m_fourCC.GetValueOrDefault();
            if (m_fourCC.HasValue)
            {
                switch (valueOrDefault)
                {
                    case 0x31545844:
                        flags = (int)DdsSquish.SquishFlags.kDxt1;
                        break;

                    case 0x33545844:
                        flags = (int)DdsSquish.SquishFlags.kDxt3;
                        break;

                    case 0x35545844:
                        flags = (int)DdsSquish.SquishFlags.kDxt5;
                        break;
                    default:
                        throw new FormatException("File is not a squish-supported DDS format");
                }
            }

            buffer2 = DdsSquish.CompressImage(pixelData, this.GetWidth(), this.GetHeight(), flags);
            new BinaryWriter(output).Write((uint)0x20534444);
            this.m_header.Write(output);
            output.Write(buffer2, 0, buffer2.Length);
        }

        /// <summary>
        /// Converts the DDS encoded image into a <see cref="System.Drawing.Image"/> representation,
        /// subject to the provided parameters.
        /// </summary>
        /// <param name="red">When true, the red channel of the DDS contributes to the red pixels of the returned image.</param>
        /// <param name="green">When true, the green channel of the DDS contributes to the green pixels of the returned image.</param>
        /// <param name="blue">When true, the blue channel of the DDS contributes to the blue pixels of the returned image.</param>
        /// <param name="alpha">When true, the alpha channel of the DDS contributes to the pixel transparency in the returned image.</param>
        /// <param name="invert">When true, the alpha channel of the DDS is inverted.</param>
        /// <returns>A <see cref="System.Drawing.Image"/> representation of the DDS encoded image in the loaded <see cref="System.IO.Stream"/>.</returns>
        /// <seealso cref="Load"/>
        public Image Image(bool red = true, bool green = true, bool blue = true, bool alpha = false, bool invert = false)
        {
            Bitmap bitmap = new Bitmap(this.GetWidth(), this.GetHeight(), System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            byte[] readPixelData = this.GetPixelData();

            for (int y = 0; y < this.GetHeight(); y++)
            {
                for (int x = 0; x < this.GetWidth(); x++)
                {
                    int readPixelOffset = pixelOffset(x, y, this.GetWidth());

                    int cred = 0;
                    int cgreen = 0;
                    int cblue = 0;
                    int calpha = 0;

                    if (red) { cred = readPixelData[readPixelOffset + 0]; }
                    if (green) { cgreen = readPixelData[readPixelOffset + 1]; }
                    if (blue) { cblue = readPixelData[readPixelOffset + 2]; }
                    if (alpha)
                    {
                        calpha = readPixelData[readPixelOffset + 3];
                        // Inverse the alpha
                        if (invert)
                            calpha = (255 - calpha);
                    }

                    if (alpha)
                    {
                        bitmap.SetPixel(x, y, Color.FromArgb(calpha, cred, cgreen, cblue));
                    }
                    else
                    {
                        bitmap.SetPixel(x, y, Color.FromArgb(cred, cgreen, cblue));
                    }
                }
            }

            return bitmap;
        }

        bool maskInEffect = false;

        /// <summary>
        /// Clears a previously-applied HSVShift mask.
        /// </summary>
        public void ClearMask()
        {
            if (!SupportsHSV || !maskInEffect) return;
            hsvData = RGBHSV.ColorRGBA.ConvertToColorHSVAArray(m_pixelData);
            maskInEffect = false;
        }

        /// <summary>
        /// Apply <see cref="RGBHSV.HSVShift"/> values to this DDS image based on the
        /// channels in the <paramref name="mask"/>.
        /// </summary>
        /// <param name="mask">A DDS image file, each colourway acting as a mask channel.</param>
        /// <param name="ch1Shift">A shift to apply to the image when the first channel of the mask is active.</param>
        /// <param name="ch2Shift">A shift to apply to the image when the second channel of the mask is active.</param>
        /// <param name="ch3Shift">A shift to apply to the image when the third channel of the mask is active.</param>
        /// <param name="ch4Shift">A shift to apply to the image when the fourth channel of the mask is active.</param>
        public void MaskedHSVShift(DdsFile mask, RGBHSV.HSVShift ch1Shift, RGBHSV.HSVShift ch2Shift, RGBHSV.HSVShift ch3Shift, RGBHSV.HSVShift ch4Shift)
        {
            if (!SupportsHSV) return;

            maskInEffect = maskInEffect || !ch1Shift.IsEmpty || !ch2Shift.IsEmpty || !ch3Shift.IsEmpty || !ch4Shift.IsEmpty;

            if (!maskInEffect) return;

            if (!ch1Shift.IsEmpty)
                for (int i = 0; i < hsvData.Length; i++)
                    if (mask.m_pixelData[i * 4 + 0] != 0) hsvData[i] = hsvData[i].HSVShift(ch1Shift);

            if (!ch2Shift.IsEmpty)
                for (int i = 0; i < hsvData.Length; i++)
                    if (mask.m_pixelData[i * 4 + 1] != 0) hsvData[i] = hsvData[i].HSVShift(ch2Shift);

            if (!ch3Shift.IsEmpty)
                for (int i = 0; i < hsvData.Length; i++)
                    if (mask.m_pixelData[i * 4 + 2] != 0) hsvData[i] = hsvData[i].HSVShift(ch3Shift);

            if (!ch4Shift.IsEmpty)
                for (int i = 0; i < hsvData.Length; i++)
                    if (mask.m_pixelData[i * 4 + 3] != 0) hsvData[i] = hsvData[i].HSVShift(ch4Shift);
        }

        /// <summary>
        /// Apply <see cref="RGBHSV.HSVShift"/> values to this DDS image based on the
        /// channels in the <paramref name="mask"/>.
        /// Each channel of the mask acts independently, in order "R", "G", "B", "A".
        /// </summary>
        /// <param name="mask">A DDS image file, each colourway acting as a mask channel.</param>
        /// <param name="ch1Shift">A shift to apply to the image when the first channel of the mask is active.</param>
        /// <param name="ch2Shift">A shift to apply to the image when the second channel of the mask is active.</param>
        /// <param name="ch3Shift">A shift to apply to the image when the third channel of the mask is active.</param>
        /// <param name="ch4Shift">A shift to apply to the image when the fourth channel of the mask is active.</param>
        public void MaskedHSVShiftNoBlend(DdsFile mask, RGBHSV.HSVShift ch1Shift, RGBHSV.HSVShift ch2Shift, RGBHSV.HSVShift ch3Shift, RGBHSV.HSVShift ch4Shift)
        {
            if (!SupportsHSV) return;

            maskInEffect = maskInEffect || !ch1Shift.IsEmpty || !ch2Shift.IsEmpty || !ch3Shift.IsEmpty || !ch4Shift.IsEmpty;

            if (!maskInEffect) return;

            RGBHSV.ColorHSVA[] result = new RGBHSV.ColorHSVA[hsvData.Length];
            Array.Copy(hsvData, 0, result, 0, result.Length);

            if (!ch1Shift.IsEmpty)
                for (int i = 0; i < hsvData.Length; i++)
                    if (mask.m_pixelData[i * 4 + 0] != 0) result[i] = hsvData[i].HSVShift(ch1Shift);

            if (!ch2Shift.IsEmpty)
                for (int i = 0; i < hsvData.Length; i++)
                    if (mask.m_pixelData[i * 4 + 1] != 0) result[i] = hsvData[i].HSVShift(ch2Shift);

            if (!ch3Shift.IsEmpty)
                for (int i = 0; i < hsvData.Length; i++)
                    if (mask.m_pixelData[i * 4 + 2] != 0) result[i] = hsvData[i].HSVShift(ch3Shift);

            if (!ch4Shift.IsEmpty)
                for (int i = 0; i < hsvData.Length; i++)
                    if (mask.m_pixelData[i * 4 + 3] != 0) result[i] = hsvData[i].HSVShift(ch4Shift);

            hsvData = result;
        }

        /// <summary>
        /// Set the colour of the image based on the channels in the <paramref name="mask"/>.
        /// </summary>
        /// <param name="mask">The <see cref="System.IO.Stream"/> containing the DDS image to use as a mask.</param>
        /// <param name="ch1Colour">(Nullable) ARGB colour to the image when the first channel of the mask is active.</param>
        /// <param name="ch2Colour">(Nullable) ARGB colour to the image when the second channel of the mask is active.</param>
        /// <param name="ch3Colour">(Nullable) ARGB colour to the image when the third channel of the mask is active.</param>
        /// <param name="ch4Colour">(Nullable) ARGB colour to the image when the fourth channel of the mask is active.</param>
        public void MaskedSetColour(DdsFile mask, uint? ch1Colour, uint? ch2Colour, uint? ch3Colour, uint? ch4Colour)
        {
            byte[] readPixelData = GetPixelData();

            if (ch1Colour.HasValue)
                for (int i = 0; i < hsvData.Length; i++)
                    if (mask.m_pixelData[i * 4 + 0] != 0) setPixelARGB(readPixelData, i * 4, ch1Colour.Value);

            if (ch2Colour.HasValue)
                for (int i = 0; i < hsvData.Length; i++)
                    if (mask.m_pixelData[i * 4 + 1] != 0) setPixelARGB(readPixelData, i * 4, ch2Colour.Value);

            if (ch3Colour.HasValue)
                for (int i = 0; i < hsvData.Length; i++)
                    if (mask.m_pixelData[i * 4 + 2] != 0) setPixelARGB(readPixelData, i * 4, ch3Colour.Value);

            if (ch4Colour.HasValue)
                for (int i = 0; i < hsvData.Length; i++)
                    if (mask.m_pixelData[i * 4 + 3] != 0) setPixelARGB(readPixelData, i * 4, ch4Colour.Value);

            if (SupportsHSV) hsvData = RGBHSV.ColorRGBA.ConvertToColorHSVAArray(readPixelData);
            m_pixelData = readPixelData;
        }

        /// <summary>
        /// Use the <paramref name="mask"/> to apply the DDS image supplied.
        /// </summary>
        /// <param name="mask">Determines to which areas each DDS image is applied.</param>
        /// <param name="ch1DdsFile">DDS image applied to <paramref name="mask"/> channel 1 area.</param>
        /// <param name="ch2DdsFile">DDS image applied to <paramref name="mask"/> channel 2 area.</param>
        /// <param name="ch3DdsFile">DDS image applied to <paramref name="mask"/> channel 3 area.</param>
        /// <param name="ch4DdsFile">DDS image applied to <paramref name="mask"/> channel 4 area.</param>
        public void MaskedApplyImage(DdsFile mask, DdsFile ch1DdsFile, DdsFile ch2DdsFile, DdsFile ch3DdsFile, DdsFile ch4DdsFile)
		{
            byte[] readPixelData = GetPixelData();

            for (int y = 0; y < mask.GetHeight(); y++)
                for (int x = 0; x < mask.GetWidth(); x++)
                {
                    int offset = ((y * mask.GetWidth()) * 4) + (x * 4);
                    if ((ch1DdsFile != null) && (mask.m_pixelData[offset + 0] != 0)) copyPixel(readPixelData, offset, ch1DdsFile.m_pixelData);
                    if ((ch2DdsFile != null) && (mask.m_pixelData[offset + 1] != 0)) copyPixel(readPixelData, offset, ch2DdsFile.m_pixelData);
                    if ((ch3DdsFile != null) && (mask.m_pixelData[offset + 2] != 0)) copyPixel(readPixelData, offset, ch3DdsFile.m_pixelData);
                    if ((ch4DdsFile != null) && (mask.m_pixelData[offset + 3] != 0)) copyPixel(readPixelData, offset, ch4DdsFile.m_pixelData);
                }

            if (SupportsHSV) hsvData = RGBHSV.ColorRGBA.ConvertToColorHSVAArray(readPixelData);
            m_pixelData = readPixelData;
        }

        /// <summary>
        /// Use the <paramref name="mask"/> to apply the supplied images.
        /// </summary>
        /// <param name="mask">Determines to which areas each image is applied.</param>
        /// <param name="ch1Image">Image applied to <paramref name="mask"/> channel 1 area.</param>
        /// <param name="ch2Image">Image applied to <paramref name="mask"/> channel 2 area.</param>
        /// <param name="ch3Image">Image applied to <paramref name="mask"/> channel 3 area.</param>
        /// <param name="ch4Image">Image applied to <paramref name="mask"/> channel 4 area.</param>
        public void MaskedApplyImage(DdsFile mask, Image ch1Image, Image ch2Image, Image ch3Image, Image ch4Image)
		{
            this.MaskedApplyImage(mask,
                (ch1Image == null) ? null : new Bitmap(ch1Image),
                (ch2Image == null) ? null : new Bitmap(ch2Image),
                (ch3Image == null) ? null : new Bitmap(ch3Image),
                (ch4Image == null) ? null : new Bitmap(ch4Image));
        }

        /// <summary>
        /// Use the <paramref name="mask"/> to apply the supplied bitmaps.
        /// </summary>
        /// <param name="mask">Determines to which areas each image is applied.</param>
        /// <param name="ch1Bitmap">Bitmap applied to <paramref name="mask"/> channel 1 area.</param>
        /// <param name="ch2Bitmap">Bitmap applied to <paramref name="mask"/> channel 2 area.</param>
        /// <param name="ch3Bitmap">Bitmap applied to <paramref name="mask"/> channel 3 area.</param>
        /// <param name="ch4Bitmap">Bitmap applied to <paramref name="mask"/> channel 4 area.</param>
        public void MaskedApplyImage(DdsFile mask, Bitmap ch1Bitmap, Bitmap ch2Bitmap, Bitmap ch3Bitmap, Bitmap ch4Bitmap)
		{
            byte[] readPixelData = GetPixelData();

            for (int y = 0; y < mask.GetHeight(); y++)
                for (int x = 0; x < mask.GetWidth(); x++)
                {
                    int offset = ((y * mask.GetWidth()) * 4) + (x * 4);

                    if ((ch1Bitmap != null) && (mask.m_pixelData[offset + 0] != 0))
                        this.setPixelARGB(readPixelData, offset, (uint)ch1Bitmap.GetPixel(x % ch1Bitmap.Width, y % ch1Bitmap.Height).ToArgb());

                    if ((ch2Bitmap != null) && (mask.m_pixelData[offset + 1] != 0))
                        this.setPixelARGB(readPixelData, offset, (uint)ch2Bitmap.GetPixel(x % ch2Bitmap.Width, y % ch2Bitmap.Height).ToArgb());

                    if ((ch3Bitmap != null) && (mask.m_pixelData[offset + 2] != 0))
                        this.setPixelARGB(readPixelData, offset, (uint)ch3Bitmap.GetPixel(x % ch3Bitmap.Width, y % ch3Bitmap.Height).ToArgb());

                    if ((ch4Bitmap != null) && (mask.m_pixelData[offset + 3] != 0))
                        this.setPixelARGB(readPixelData, offset, (uint)ch4Bitmap.GetPixel(x % ch4Bitmap.Width, y % ch4Bitmap.Height).ToArgb());
                }

            if (SupportsHSV) hsvData = RGBHSV.ColorRGBA.ConvertToColorHSVAArray(readPixelData);
            m_pixelData = readPixelData;
        }

        int GetWidth()
        {
            return (int)m_header.m_width;
        }

        int GetHeight()
        {
            return (int)m_header.m_height;
        }

        byte[] GetPixelData()
        {
            return SupportsHSV && (!hsvShift.IsEmpty || maskInEffect)
                ? RGBHSV.ColorRGBA.ConvertToByteArray(hsvData, hsvShift)
                : m_pixelData
            ;
        }

        // Loaded DDS header (also uses storage for save)
        DdsHeader m_header = new DdsHeader();

        // Pixel data
        byte[] m_pixelData;

        // HSVa data
        RGBHSV.ColorHSVA[] hsvData = null;

        RGBHSV.HSVShift hsvShift;
        /// <summary>
        /// The HSVShift applied to the current image, when supported.
        /// </summary>
        /// <seealso cref="SupportsHSV"/>
        public RGBHSV.HSVShift HSVShift { get { return hsvShift; } set { hsvShift = value; } }

        /// <summary>
        /// The image size.
        /// </summary>
        public Size Size
        {
            get { return new Size(GetWidth(), GetHeight()); }
        }

        /// <summary>
        /// True if the image is prepared to process HSV requests.
        /// </summary>
        public bool SupportsHSV
        {
            get { return hsvData != null; }
            set { if (value != SupportsHSV) { hsvData = value ? RGBHSV.ColorRGBA.ConvertToColorHSVAArray(m_pixelData) : null; } }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            maskInEffect = false;
            hsvData = null;
            m_pixelData = null;
        }
    }
}