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
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
//using PaintDotNet;

namespace DdsFileTypePlugin
{
	internal sealed class DdsSquish
	{
		public enum SquishFlags
		{
			kDxt1						= ( 1 << 0 ),		// Use DXT1 compression.
			kDxt3						= ( 1 << 1 ),		// Use DXT3 compression.
			kDxt5						= ( 1 << 2 ), 		// Use DXT5 compression.
		
			kColourClusterFit			= ( 1 << 3 ),		// Use a slow but high quality colour compressor (the default).
			kColourRangeFit				= ( 1 << 4 ),		// Use a fast but low quality colour compressor.

			kColourMetricPerceptual		= ( 1 << 5 ),		// Use a perceptual metric for colour error (the default).
			kColourMetricUniform		= ( 1 << 6 ),		// Use a uniform metric for colour error.
	
			kWeightColourByAlpha		= ( 1 << 7 ),		// Weight the colour by alpha during cluster fit (disabled by default).

			kColourIterativeClusterFit	= ( 1 << 8 ),		// Use a very slow but very high quality colour compressor.
		}

		private	static bool	Is64Bit()
		{
			return ( Marshal.SizeOf( IntPtr.Zero ) == 8 ); 
		}

		private sealed class SquishInterface_32
		{
			[DllImport("squishinterface_Win32.dll")]
			internal static extern unsafe void SquishCompressImage( byte* rgba, int width, int height, byte* blocks, int flags );
			[DllImport("squishinterface_Win32.dll")]
			internal static	extern unsafe void SquishDecompressImage( byte* rgba, int width, int height, byte* blocks, int flags );
		}
   
		private sealed class SquishInterface_64
		{
			[DllImport("squishinterface_x64.dll")]
			internal static extern unsafe void SquishCompressImage( byte* rgba, int width, int height, byte* blocks, int flags );
			[DllImport("squishinterface_x64.dll")]
			internal static	extern unsafe void SquishDecompressImage( byte* rgba, int width, int height, byte* blocks, int flags );
		}
	
		private static unsafe void	CallDecompressImage( byte[] rgba, int width, int height, byte[] blocks, int flags )
		{
			fixed ( byte* pRGBA = rgba )
			{
				fixed ( byte* pBlocks = blocks )
				{
					if ( Is64Bit() )
						SquishInterface_64.SquishDecompressImage( pRGBA, width, height, pBlocks, flags );
					else
						SquishInterface_32.SquishDecompressImage( pRGBA, width, height, pBlocks, flags );
				}
			}
		}

		// ---------------------------------------------------------------------------------------
		//	DecompressImage
		// ---------------------------------------------------------------------------------------
		//
		//	Params
		//		inputSurface	:	Source byte array containing DXT block data
		//		width			:	Width of image in pixels
		//		height			:	Height of image in pixels
		//		flags			:	Flags for squish decompression control
		//
		//	Return	
		//		byte[]			:	Array of bytes containing decompressed blocks
		//
		// ---------------------------------------------------------------------------------------

		internal static byte[] DecompressImage( byte[] blocks, int width, int height, int flags)
		{
			// Allocate room for decompressed output
			byte[]	pixelOutput	= new byte[ width * height * 4 ];

			// Invoke squish::DecompressImage() with the required parameters
			CallDecompressImage( pixelOutput, width, height, blocks, flags );

			// Return our pixel data to caller..
			return pixelOutput;
		}
	}
}
