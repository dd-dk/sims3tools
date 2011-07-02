using System;
using System.Collections.Generic;

namespace RGBHSV
{
    /// <summary>
    /// Implement ColorRGBA class and conversion to ColorHSVA
    /// </summary>
    /// <remarks>See <a href="http://www.academictutorials.com/graphics/graphics-the-hsl-color-model.asp">this paper</a>
    /// for where this code originates.
    /// Note that I've changed disagreements between scaling by 255 or 256 to always use 255.  This means
    /// floats range from 0.0f to 1.0f and bytes from 0 to 255.  I hope.</remarks>
    public struct ColorRGBA
    {
        /// <summary>
        /// r for red, g for green, b for blue, a for alpha
        /// </summary>
        public byte r, g, b, a;
        /// <summary>
        /// Create a new <see cref="ColorRGBA"/> from an existing <see cref="ColorHSVA"/> value.
        /// </summary>
        /// <param name="hsva">An existing <see cref="ColorHSVA"/> value.</param>
        public ColorRGBA(ColorHSVA hsva) : this(hsva.ToColorRGBA()) { }
        /// <summary>
        /// Create a new <see cref="ColorRGBA"/> from an existing <see cref="ColorRGBA"/> value.
        /// </summary>
        /// <param name="rgba">An existing <see cref="ColorRGBA"/> value.</param>
        public ColorRGBA(ColorRGBA rgba) : this(rgba.r, rgba.g, rgba.b, rgba.a) { }
        /// <summary>
        /// Create a new <see cref="ColorRGBA"/> from a byte-array <paramref name="data"/>,
        /// containing four-byte sequences, r-g-b-a, using the values at the given <paramref name="offset"/>.
        /// </summary>
        /// <param name="data">A byte-array containing four-byte sequences, r-g-b-a.</param>
        /// <param name="offset">Position in <paramref name="data"/> from which to take <see cref="ColorRGBA"/> values.</param>
        public ColorRGBA(byte[] data, int offset) : this(data[offset + 0], data[offset + 1], data[offset + 2], data[offset + 3]) { }
        /// <summary>
        /// Create a new <see cref="ColorRGBA"/> from the given values.
        /// </summary>
        /// <param name="r">Red.</param>
        /// <param name="g">Blue.</param>
        /// <param name="b">Green.</param>
        /// <param name="a">Alpha.</param>
        public ColorRGBA(byte r, byte g, byte b, byte a) { this.r = r; this.g = g; this.b = b; this.a = a; }
        /// <summary>
        /// Converts the current <see cref="ColorRGBA"/> value to a <see cref="ColorHSVA"/> quantity.
        /// </summary>
        /// <returns>Equivalent <see cref="ColorHSVA"/> quantity.</returns>
        public ColorHSVA ToColorHSVA()
        {
            float r, g, b, h, s, v;

            r = this.r / 255.0f;
            g = this.g / 255.0f;
            b = this.b / 255.0f;

            float maxColor = Math.Max(r, Math.Max(g, b));
            float minColor = Math.Min(r, Math.Min(g, b));
            v = maxColor;

            s = v == 0 ? 0 : (maxColor - minColor) / maxColor;
            if (s == 0) h = 0;
            else
            {
                if (r == maxColor) h = (g - b) / (maxColor - minColor);
                else if (g == maxColor) h = 2.0f + (b - r) / (maxColor - minColor);
                else h = 4.0f + (r - g) / (maxColor - minColor);
                h /= 6.0f; //to bring it to a number between 0 and 1
                if (h < 0) h++;
            }

            return new ColorHSVA
            {
                h = (byte)Math.Round(h * 255.0),
                s = (byte)Math.Round(s * 255.0),
                v = (byte)Math.Round(v * 255.0),
                a = this.a,
            };
        }
        /// <summary>
        /// Converts the provided value to a <see cref="ColorHSVA"/> quantity.
        /// </summary>
        /// <param name="value"><see cref="ColorRGBA"/> quantity.</param>
        /// <returns>Equivalent <see cref="ColorHSVA"/> quantity.</returns>
        public static explicit operator ColorHSVA(ColorRGBA value) { return value.ToColorHSVA(); }

        /// <summary>
        /// Determine if <paramref name="obj"/> is a <see cref="ColorRGBA"/> equal
        /// to the current value.
        /// </summary>
        /// <param name="obj">Value to test for equality.</param>
        /// <returns>True if <paramref name="obj"/> is a <see cref="ColorRGBA"/> equal
        /// to the current value; otherwise false.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is ColorRGBA)) return false;
            ColorRGBA other = (ColorRGBA)obj;
            return r == other.r && g == other.g && b == other.b && a == other.a;
        }
        /// <summary>
        /// Returns the hashcode for this instance.
        /// </summary>
        /// <returns>The hashcode for this instance.</returns>
        public override int GetHashCode()
        {
            return r.GetHashCode() & g.GetHashCode() & b.GetHashCode() & a.GetHashCode();
        }

        /// <summary>
        /// Creates the HSVA data values corresponding to an RGBA pixel data array.
        /// </summary>
        /// <param name="pixelDataRGBA">An RGBA pixel data array.</param>
        /// <returns>HSVA data values corresponding to <paramref name="pixelDataRGBA"/>.</returns>
        public static ColorHSVA[] ConvertToColorHSVAArray(byte[] pixelDataRGBA)
        {
            ColorHSVA[] hsvaData = new ColorHSVA[pixelDataRGBA.Length / 4];
            for (int offset = 0; offset < pixelDataRGBA.Length; offset += 4)
                hsvaData[offset / 4] = new ColorRGBA(pixelDataRGBA, offset).ToColorHSVA();
            return hsvaData;
        }

        /// <summary>
        /// Creates a byte array corresponding to an HSVA image.
        /// </summary>
        /// <param name="image">An HSVA image.</param>
        /// <param name="hsvShift">An optional <see cref="HSVShift"/> to apply.</param>
        /// <returns>a byte array corresponding to <paramref name="image"/>.</returns>
        public static byte[] ConvertToByteArray(ColorHSVA[] image, HSVShift hsvShift = new HSVShift())
        {
            byte[] pixelDataRGBA = new byte[image.Length * 4];
            for (int offset = 0; offset / 4 < image.Length; offset += 4)
            {
                ColorRGBA rgba = hsvShift.IsEmpty ? image[offset / 4].ToColorRGBA() : image[offset / 4].HSVShift(hsvShift).ToColorRGBA();
                pixelDataRGBA[offset + 0] = rgba.r;
                pixelDataRGBA[offset + 1] = rgba.g;
                pixelDataRGBA[offset + 2] = rgba.b;
                pixelDataRGBA[offset + 3] = rgba.a;
            }
            return pixelDataRGBA;
        }
    }

    /// <summary>
    /// Implement ColorHSVA class and conversion to ColorRGBA
    /// </summary>
    /// <remarks>See <a href="http://www.academictutorials.com/graphics/graphics-the-hsl-color-model.asp">this paper</a>
    /// for where this code originates.
    /// Note that I've changed disagreements between scaling by 255 or 256 to always use 255.  This means
    /// floats range from 0.0f to 1.0f and bytes from 0 to 255.  I hope.</remarks>
    public struct ColorHSVA
    {
        /// <summary>
        /// h for hue, s for saturation, v for value, a for alpha
        /// </summary>
        /// <remarks><see cref="a"/> is only used for conversion from and to <see cref="ColorRGBA"/>.</remarks>
        public byte h, s, v, a;
        /// <summary>
        /// Create a new <see cref="ColorHSVA"/> from an existing <see cref="ColorRGBA"/> value.
        /// </summary>
        /// <param name="rgba">An existing <see cref="ColorRGBA"/> value.</param>
        public ColorHSVA(ColorRGBA rgba) : this(rgba.ToColorHSVA()) { }
        /// <summary>
        /// Create a new <see cref="ColorHSVA"/> from an existing <see cref="ColorHSVA"/> value.
        /// </summary>
        /// <param name="hsva">An existing <see cref="ColorHSVA"/> value.</param>
        public ColorHSVA(ColorHSVA hsva) : this(hsva.h, hsva.s, hsva.v, hsva.a) { }
        /// <summary>
        /// Create a new <see cref="ColorHSVA"/> from the given values.
        /// </summary>
        /// <param name="h">Hue.</param>
        /// <param name="s">Saturation.</param>
        /// <param name="v">Value.</param>
        /// <param name="a">Alpha.</param>
        public ColorHSVA(byte h, byte s, byte v, byte a) { this.h = h; this.s = s; this.v = v; this.a = a; }
        /// <summary>
        /// Converts the provided value to a <see cref="ColorRGBA"/> quantity.
        /// </summary>
        /// <returns>Equivalent <see cref="ColorRGBA"/> quantity.</returns>
        public ColorRGBA ToColorRGBA()
        {
            float r = 0, g = 0, b = 0, h, s, v;
            h = this.h / 255.0f;
            s = this.s / 255.0f;
            v = this.v / 255.0f;

            if (s == 0) r = g = b = v;//Grey
            else
            {
                float f, p, q, t;
                int i;
                h *= 6; //to bring hue to a number between 0 and 6, better for the calculations
                i = (int)(Math.Floor(h));  //e.g. 2.7 becomes 2 and 3.01 becomes 3 or 4.9999 becomes 4
                f = h - i;  //the fractional part of h
                p = v * (1 - s);
                q = v * (1 - (s * f));
                t = v * (1 - (s * (1 - f)));
                switch (i)
                {
                    case 0: r = v; g = t; b = p; break;
                    case 1: r = q; g = v; b = p; break;
                    case 2: r = p; g = v; b = t; break;
                    case 3: r = p; g = q; b = v; break;
                    case 4: r = t; g = p; b = v; break;
                    case 5: r = v; g = p; b = q; break;
                }
            }

            return new ColorRGBA
            {
                r = (byte)Math.Round(r * 255.0f),
                g = (byte)Math.Round(g * 255.0f),
                b = (byte)Math.Round(b * 255.0f),
                a = this.a,
            };
        }
        /// <summary>
        /// Converts the current <see cref="ColorHSVA"/> value to a <see cref="ColorRGBA"/> quantity.
        /// </summary>
        /// <param name="value"><see cref="ColorHSVA"/> quantity.</param>
        /// <returns>Equivalent <see cref="ColorRGBA"/> quantity.</returns>
        public static explicit operator ColorRGBA(ColorHSVA value) { return value.ToColorRGBA(); }

        /// <summary>
        /// Return the current <see cref="ColorHSVA"/> adjusted by a given <see cref="HSVShift"/> value.
        /// </summary>
        /// <param name="hsvShift">The <see cref="HSVShift"/> value by which to adjust this <see cref="ColorHSVA"/>.</param>
        /// <returns>The current <see cref="ColorHSVA"/> adjusted by a given <see cref="HSVShift"/> value.</returns>
        public ColorHSVA HSVShift(HSVShift hsvShift)
        {
            float h = this.h / 255.0f + hsvShift.h;
            if (h < 0) h += (float)Math.Floor(Math.Abs(h)) + 1;
            if (h > 1) h -= (float)Math.Floor(Math.Abs(h)) + 1;
            return new ColorHSVA
            {
                h = (byte)Math.Round(h * 255.0f),
                s = (byte)Math.Round(Math.Max(0, Math.Min(1, this.s / 255.0f + hsvShift.s)) * 255.0f),
                v = (byte)Math.Round(Math.Max(0, Math.Min(1, this.v / 255.0f + hsvShift.v)) * 255.0f),
                a = this.a,
            };
        }

        /// <summary>
        /// Determine if <paramref name="obj"/> is a <see cref="ColorHSVA"/> equal
        /// to the current value.
        /// </summary>
        /// <param name="obj">Value to test for equality.</param>
        /// <returns>True if <paramref name="obj"/> is a <see cref="ColorHSVA"/> equal
        /// to the current value; otherwise false.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is ColorHSVA)) return false;
            ColorHSVA other = (ColorHSVA)obj;
            return h == other.h && s == other.s && v == other.v && a == other.a;
        }
        /// <summary>
        /// Returns the hashcode for this instance.
        /// </summary>
        /// <returns>The hashcode for this instance.</returns>
        public override int GetHashCode()
        {
            return h.GetHashCode() & s.GetHashCode() & v.GetHashCode() & a.GetHashCode();
        }
    }

    /// <summary>
    /// Describes a hue, saturation and value shift to be applied to an image
    /// </summary>
    public struct HSVShift
    {
        /// <summary>
        /// Hue, Saturation and Value shift amounts.
        /// </summary>
        public float h, s, v;
        /// <summary>
        /// Returns true if <paramref name="obj"/> is a <see cref="HSVShift"/> with the same value as this instance.
        /// </summary>
        /// <param name="obj">An object to compare.</param>
        /// <returns>True if <paramref name="obj"/> is a <see cref="HSVShift"/> with the same value as this instance; otherwise false.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is HSVShift)) return false;
            HSVShift other = (HSVShift)obj;
            return h == other.h && s == other.s && v == other.v;
        }
        /// <summary>
        /// Returns the hashcode for this instance.
        /// </summary>
        /// <returns>The hashcode for this instance.</returns>
        public override int GetHashCode()
        {
            return h.GetHashCode() ^ s.GetHashCode() ^ v.GetHashCode();
        }

        /// <summary>
        /// True if this HSVShift is non-zero.
        /// </summary>
        public bool IsEmpty { get { return h == 0 && s == 0 && v == 0; } }

        static HSVShift _Empty = new HSVShift();
        /// <summary>
        /// An empty HSVShift
        /// </summary>
        public static HSVShift Empty { get { return _Empty; } }
    }
}
