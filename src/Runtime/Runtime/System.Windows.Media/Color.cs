
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System;
using System.Globalization;
using System.Text;
using OpenSilver.Internal;

#if !MIGRATION
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI
#endif
{
    /// <summary>
    /// Describes a color in terms of alpha, red, green, and blue channels.
    /// </summary>
    public struct Color : IFormattable
    {
        internal static Color INTERNAL_ConvertFromInt32(int argb)
        {
            Color c1 = new Color();

            c1.sRgbColor.a = (byte)((argb >> 0x18) & 0xff);
            c1.sRgbColor.r = (byte)((argb >> 0x10) & 0xff);
            c1.sRgbColor.g = (byte)((argb >> 8) & 0xff);
            c1.sRgbColor.b = (byte)(argb & 0xff);

            return c1;
        }

        ///<summary>
        /// FromScRgb
        ///</summary>
        public static Color FromScRgb(float a, float r, float g, float b)
        {
            Color c1 = new Color();

            if (a < 0.0f)
            {
                a = 0.0f;
            }
            else if (a > 1.0f)
            {
                a = 1.0f;
            }

            c1.sRgbColor.a = (byte)((a * 255.0f) + 0.5f);
            c1.sRgbColor.r = ScRgbTosRgb(r);
            c1.sRgbColor.g = ScRgbTosRgb(g);
            c1.sRgbColor.b = ScRgbTosRgb(b);

            return c1;
        }

        /// <summary>
        /// Creates a new <see cref="Color"/> structure by using the specified sRGB
        /// alpha channel and color channel values.
        /// </summary>
        /// <param name="a">
        /// The alpha channel, <see cref="A"/>, of the new color. The value
        /// must be between 0 and 255.
        /// </param>
        /// <param name="r">
        /// The red channel, <see cref="R"/>, of the new color. The value must
        /// be between 0 and 255.
        /// </param>
        /// <param name="g">
        /// The green channel, <see cref="G"/>, of the new color. The value
        /// must be between 0 and 255.
        /// </param>
        /// <param name="b">
        /// The blue channel, <see cref="B"/>, of the new color. The value must
        /// be between 0 and 255.
        /// </param>
        /// <returns>
        /// A <see cref="Color"/> structure with the specified values.
        /// </returns>
        public static Color FromArgb(byte a, byte r, byte g, byte b)
        {
            Color c1 = new Color();
     
            c1.sRgbColor.a = a;
            c1.sRgbColor.r = r;
            c1.sRgbColor.g = g;
            c1.sRgbColor.b = b;

            return c1;
        }

        ///<summary>
        /// Color - sRgb legacy interface, assumes Rgb values are sRgb
        ///</summary>
        public static Color FromRgb(byte r, byte g, byte b)
        {
            Color c1 = Color.FromArgb(0xff, r, g, b);
            return c1;
        }

        /// <summary>
        /// Gets a hash code for the current <see cref="Color"/> structure.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="Color"/> structure.
        /// </returns>
        public override int GetHashCode()
        {
            return this.sRgbColor.GetHashCode();
        }

        /// <summary>
        /// Creates a string representation of the color using the ARGB channels in hex notation.
        /// </summary>
        /// <returns>
        /// The string representation of the color.
        /// </returns>
        public override string ToString()
        {
            // Delegate to the internal method which implements all ToString calls.
            return ConvertToString(null, null);
        }

        /// <summary>
        /// Creates a string representation of the color by using the ARGB channels and the
        /// specified format provider.
        /// </summary>
        /// <param name="provider">
        /// Culture-specific formatting information.
        /// </param>
        /// <returns>
        /// The string representation of the color.
        /// </returns>
        public string ToString(IFormatProvider provider)
        {
            // Delegate to the internal method which implements all ToString calls.
            return ConvertToString(null, provider);
        }

        /// <summary>
        /// Creates a string representation of this object based on the format string
        /// and IFormatProvider passed in.
        /// If the provider is null, the CurrentCulture is used.
        /// See the documentation for IFormattable for more information.
        /// </summary>
        /// <returns>
        /// A string representation of this object.
        /// </returns>
        string IFormattable.ToString(string format, IFormatProvider provider)
        {
            // Delegate to the internal method which implements all ToString calls.
            return ConvertToString(format, provider);
        }

        /// <summary>
        /// Creates a string representation of this object based on the format string 
        /// and IFormatProvider passed in.  
        /// If the provider is null, the CurrentCulture is used.
        /// See the documentation for IFormattable for more information.
        /// </summary>
        /// <returns>
        /// A string representation of this object.
        /// </returns>
        internal string ConvertToString(string format, IFormatProvider provider)
        {
            StringBuilder sb = new StringBuilder();

            if (format == null)
            {
                sb.Append(string.Format(provider, "#{0:X2}", this.sRgbColor.a));
                sb.Append(string.Format(provider, "{0:X2}", this.sRgbColor.r));
                sb.Append(string.Format(provider, "{0:X2}", this.sRgbColor.g));
                sb.Append(string.Format(provider, "{0:X2}", this.sRgbColor.b));
            }
            else
            {
                // Helper to get the numeric list separator for a given culture.
                char separator = TokenizerHelper.GetNumericListSeparator(provider);

                sb.Append(
                    string.Format(
                        provider,
                        "sc#{1:" + format + "}{0} {2:" + format + "}{0} {3:" + format + "}{0} {4:" + format + "}",
                        separator, sRgbColor.a, sRgbColor.r, sRgbColor.g, sRgbColor.b
                    )
                );
            }

            return sb.ToString();
        }

        /// <summary>
        /// Tests whether the specified <see cref="Color"/> structure is identical
        /// to the current color.
        /// </summary>
        /// <param name="color">
        /// The <see cref="Color"/> structure to compare to the current <see cref="Color"/>
        /// structure.
        /// </param>
        /// <returns>
        /// true if the specified <see cref="Color"/> structure is identical to the
        /// current <see cref="Color"/> structure; otherwise, false.
        /// </returns>
        public bool Equals(Color color)
        {
            return this == color;
        }

        /// <summary>
        /// Tests whether the specified object is a <see cref="Color"/> structure
        /// and is equivalent to the current color.
        /// </summary>
        /// <param name="o">
        /// The object to compare to the current <see cref="Color"/> structure.
        /// </param>
        /// <returns>
        /// true if the specified object is a <see cref="Color"/> structure and is
        /// identical to the current <see cref="Color"/> structure; otherwise, false.
        /// </returns>
        public override bool Equals(object o)
        {
            if (o is Color)
            {
                Color color = (Color)o;

                return (this == color);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Tests whether two <see cref="Color"/> structures are identical.
        /// </summary>
        /// <param name="color1">
        /// The first <see cref="Color"/> structure to compare.
        /// </param>
        /// <param name="color2">
        /// The second <see cref="Color"/> structure to compare.
        /// </param>
        /// <returns>
        /// true if color1 and color2 are exactly identical; otherwise, false.
        /// </returns>
        public static bool operator ==(Color color1, Color color2)
        {
            if (color1.sRgbColor.r != color2.sRgbColor.r)
            {
                return false;
            }

            if (color1.sRgbColor.g != color2.sRgbColor.g)
            {
                return false;
            }

            if (color1.sRgbColor.b != color2.sRgbColor.b)
            {
                return false;
            }

            if (color1.sRgbColor.a != color2.sRgbColor.a)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tests whether two <see cref="Color"/> structures are not identical.
        /// </summary>
        /// <param name="color1">
        /// The first <see cref="Color"/> structure to compare.
        /// </param>
        /// <param name="color2">
        /// The second <see cref="Color"/> structure to compare.
        /// </param>
        /// <returns>
        /// true if color1 and color2 are not equal; otherwise, false.
        /// </returns>
        public static bool operator !=(Color color1, Color color2)
        {
            return (!(color1 == color2));
        }

        /// <summary>
        /// Gets or sets the sRGB alpha channel value of the color.
        /// </summary>
        /// <returns>
        /// The sRGB alpha channel value of the color, as a value between 0 and 255.
        /// </returns>
        public byte A
        {
            get
            {
                return sRgbColor.a;
            }
            set
            {
                sRgbColor.a = value;
            }
        }

        /// <summary>
        /// Gets or sets the sRGB blue channel value of the color.
        /// </summary>
        /// <returns>
        /// The sRGB blue channel value, as a value between 0 and 255.
        /// </returns>
        public byte R
        {
            get
            {
                return sRgbColor.r;
            }
            set
            {
                sRgbColor.r = value;
            }
        }

        /// <summary>
        /// Gets or sets the sRGB green channel value of the color.
        /// </summary>
        /// <returns>
        /// The sRGB green channel value, as a value between 0 and 255.
        /// </returns>
        public byte G
        {
            get
            {
                return sRgbColor.g;
            }
            set
            {
                sRgbColor.g = value;
            }
        }

        /// <summary>
        /// Gets or sets the sRGB red channel value of the color.
        /// </summary>
        /// <returns>
        /// The sRGB red channel value, as a value between 0 and 255.
        /// </returns>
        public byte B
        {
            get
            {
                return sRgbColor.b;
            }
            set
            {
                sRgbColor.b = value;
            }
        }

        internal string INTERNAL_ToHtmlStringForVelocity()
        {
            return $"#{R.ToInvariantString("X2")}{G.ToInvariantString("X2")}{B.ToInvariantString("X2")}";
        }

        internal string INTERNAL_ToHtmlString(double opacity)
        {
            return $"rgba({R.ToInvariantString()}, {G.ToInvariantString()}, {B.ToInvariantString()}, {(opacity * A / 255d).ToInvariantString()})";
        }

        internal static Color INTERNAL_ConvertFromString(string color)
        {
            return Parsers.ParseColor(color, null);
        }

        public static explicit operator Brush(Color color)
        {
            /********************************************************
            This method is here to support the following use:

                    <ObjectAnimationUsingKeyFrames Duration="0:0:0.3" Storyboard.TargetProperty="Fill" Storyboard.TargetName="path">
                        <ObjectAnimationUsingKeyFrames.KeyFrames>
                            <DiscreteObjectKeyFrame KeyTime="0" Value="{StaticResource ThemeHighlightedColor}"/>
                        </ObjectAnimationUsingKeyFrames.KeyFrames>
                    </ObjectAnimationUsingKeyFrames>

            If we didn't have this method, we would get an invalid cast exception at the "Getter" of the following property in the class "Shape":

                    public Brush Fill
                    {
                        get { return (Brush)GetValue(FillProperty); }
                        set { SetValue(FillProperty, value); }
                    }
            ********************************************************/

            return new SolidColorBrush(color);
        }

        ///<summary>
        /// private helper function to set context values from a color value with a set context and ScRgb values
        ///</summary>
        ///
        private static byte ScRgbTosRgb(float val)
        {
            if (!(val > 0.0))       // Handles NaN case too
            {
                return (0);
            }
            else if (val <= 0.0031308)
            {
                return ((byte)((255.0f * val * 12.92f) + 0.5f));
            }
            else if (val < 1.0)
            {
                return ((byte)((255.0f * ((1.055f * (float)Math.Pow((double)val, (1.0 / 2.4))) - 0.055f)) + 0.5f));
            }
            else
            {
                return (255);
            }
        }

        private struct MILColor
        {
            public byte a, r, g, b;
        }

        private MILColor sRgbColor;
    }
}
