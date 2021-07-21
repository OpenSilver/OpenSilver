﻿

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


using OpenSilver.Internal;
using System.ComponentModel;
using System.Globalization;
#if !MIGRATION
using Windows.UI.Xaml.Media;
#endif
#if BRIDGE
using Bridge;
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
    [TypeConverter(typeof(ColorConverter))]
    public partial struct Color : IFormattable
    {
        /// <summary>
        /// Gets or sets the sRGB alpha channel value of the color.
        /// </summary>
        public byte A { get; set; }

        /// <summary>
        /// Gets or sets the sRGB blue channel value of the color.
        /// </summary>
        public byte B { get; set; }

        /// <summary>
        /// Gets or sets the sRGB green channel value of the color.
        /// </summary>
        public byte G { get; set; }

        /// <summary>
        /// Gets or sets the sRGB red channel value of the color.
        /// </summary>
        public byte R { get; set; }

        /// <summary>
        /// Creates a new Windows.UI.Color structure by using the specified sRGB alpha
        /// channel and color channel values.
        /// </summary>
        /// <param name="a">
        /// The alpha channel, Windows.UI.Color.A, of the new color. The value must be
        /// between 0 and 255.
        /// </param>
        /// <param name="r">
        /// The red channel, Windows.UI.Color.R, of the new color. The value must be
        /// between 0 and 255.
        /// </param>
        /// <param name="g">
        /// The green channel, Windows.UI.Color.G, of the new color. The value must be
        /// between 0 and 255.
        /// </param>
        /// <param name="b">
        /// The blue channel, Windows.UI.Color.B, of the new color. The value must be
        /// between 0 and 255.
        /// </param>
        /// <returns>A Windows.UI.Color structure with the specified values.</returns>
        public static Color FromArgb(byte a, byte r, byte g, byte b)
        {
            return new Color()
            {
                A = a,
                R = r,
                G = g,
                B = b
            };
        }

        public static Color FromScRgb(float a, float r, float g, float b)
        {
            return new Color()
            {
                A = (byte)(a * 255f),
                R = ScRgbTosRgb(r),
                G = ScRgbTosRgb(g),
                B = ScRgbTosRgb(b),
            };
        }

        private static byte ScRgbTosRgb(float val)
        {
            if (!(val > 0.0))       // Handles NaN case too
            {
                return 0;
            }
            else if (val <= 0.0031308)
            {
                return (byte)((255.0f * val * 12.92f) + 0.5f);
            }
            else if (val < 1.0)
            {
                return (byte)((255.0f * ((1.055f * (float)Math.Pow(val, 1.0 / 2.4)) - 0.055f)) + 0.5f);
            }
            else
            {
                return 255;
            }
        }

        internal string INTERNAL_ToHtmlString(double opacity)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "rgba({0}, {1}, {2}, {3})",
                R, G, B, opacity * A / 255d);
        }

        internal static Color INTERNAL_ConvertFromInt32(int colorAsInt32)
        {
            return new Color()
            {
                A = (byte)((colorAsInt32 >> 0x18) & 0xff),
                R = (byte)((colorAsInt32 >> 0x10) & 0xff),
                G = (byte)((colorAsInt32 >> 8) & 0xff),
                B = (byte)(colorAsInt32 & 0xff)
            };
        }

        internal string INTERNAL_ToHtmlStringForVelocity()
        {
            return string.Format("#{0}{1}{2}", 
                R.ToInvariantString("X2"), 
                G.ToInvariantString("X2"), 
                B.ToInvariantString("X2"));
        }

        public static explicit operator Brush(Color color)  // explicit Color to Brush conversion operator
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

        public override string ToString()
        {
            return string.Format("#{0}{1}{2}{3}", 
                A.ToInvariantString("X2"), 
                R.ToInvariantString("X2"), 
                G.ToInvariantString("X2"), 
                B.ToInvariantString("X2"));
        }

        public override int GetHashCode()
        {
            return BitConverter.ToInt32(new byte[] { A, R, G, B }, 0); //A.GetHashCode() ^ R.GetHashCode() ^ G.GetHashCode() ^ B.GetHashCode();
        }

        public bool Equals(Color color)
        {
            return this == color;
        }

        public override bool Equals(object o)
        {
            if (o is Color)
            {
                var color = (Color)o;

                return this == color;
            }
            else
            {
                return false;
            }
        }

        public static bool operator ==(Color color1, Color color2)
        {
            if (color1.R != color2.R)
            {
                return false;
            }

            if (color1.G != color2.G)
            {
                return false;
            }

            if (color1.B != color2.B)
            {
                return false;
            }

            if (color1.A != color2.A)
            {
                return false;
            }

            return true;
        }

        public static bool operator !=(Color color1, Color color2)
        {
            return !(color1 == color2);
        }
    }
}