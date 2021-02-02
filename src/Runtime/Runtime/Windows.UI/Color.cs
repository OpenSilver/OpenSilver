

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


using CSHTML5.Internal;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Markup;
using DotNetForHtml5.Core;
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
#if FOR_DESIGN_TIME
    [TypeConverter(typeof(ColorConverter))]
#endif
    /// <summary>
    /// Describes a color in terms of alpha, red, green, and blue channels.
    /// </summary>
    [SupportsDirectContentViaTypeFromStringConverters]
#if WORKINPROGRESS
    public partial struct Color : IFormattable
#else
    public partial struct Color //todo: this is supposed to inherit from IFormattable
#endif
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

        static Color()
        {
            TypeFromStringConverters.RegisterConverter(typeof(Color), INTERNAL_ConvertFromString);
        }


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

        internal string INTERNAL_ToHtmlString(double opacity) // Note: we didn't use a default value for the "opacity" argument to force the caller to ask herself if it is correct to call this method or if she should call "SolidColorBrush.INTERNAL_ToHtmlString()" instead (because the latter takes into account the "Brush.Opacity" property).
        {
            return "rgba(" + R.ToString() + ", " + G.ToString() + ", " + B.ToString() + ", " + (((double)A / 255) * opacity).ToString().Replace(',', '.') + ")"; //todo: instead of calling the "Replace" method, use ToString(CultureInfo.InvariantCulture) when CSHTML5 will support it.
        }

        internal static object INTERNAL_ConvertFromString(string colorString)
        {
            string trimmedString = colorString.Trim();
            if (!string.IsNullOrEmpty(trimmedString) && (trimmedString[0] == '#'))
            {
                string tokens = trimmedString.Substring(1);
                if (tokens.Length == 6) // This is becaue XAML is tolerant when the user has forgot the alpha channel (eg. #DDDDDD for Gray).
                    tokens = "FF" + tokens;

#if NETSTANDARD
                int color;
                if (int.TryParse(tokens, NumberStyles.HexNumber, NumberFormatInfo.CurrentInfo, out color))
                {
                    return INTERNAL_ConvertFromInt32(color);
                }
#else // BRIDGE
                int color;
                if (CSHTML5.Interop.IsRunningInTheSimulator)
                {
                    color = INTERNAL_BridgeWorkarounds.HexToInt_SimulatorOnly(tokens);
                }
                else
                {
                    color = Script.Write<int>("parseInt({0}, 16);", tokens);
                }

                return INTERNAL_ConvertFromInt32(color);
#endif
            }
            else if (trimmedString != null && trimmedString.StartsWith("sc#", StringComparison.Ordinal))
            {
                string tokens = trimmedString.Substring(3);

                char[] separators = new char[1] { ',' };
                string[] words = tokens.Split(separators);
                float[] values = new float[4];
                for (int i = 0; i < 3; i++)
                {                    
                    values[i] = Convert.ToSingle(words[i]);
                }
                if (words.Length == 4)
                {
                    values[3] = Convert.ToSingle(words[3]);
                    return Color.FromScRgb(values[0], values[1], values[2], values[3]);
                }
                else
                {
                    return Color.FromScRgb(1.0f, values[0], values[1], values[2]);
                }
            }
            else
            {
                // Check if the color is a named color
                Colors.INTERNAL_ColorsEnum namedColor;
                if (Enum.TryParse(trimmedString, true, out namedColor))
                {
                    return INTERNAL_ConvertFromInt32((int)namedColor);
                }
            }
            throw new Exception(string.Format("Invalid color: {0}", colorString));
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
            return "#" + R.ToString("X2") + G.ToString("X2") + B.ToString("X2"); //todo: make sure the ToString uses an invariant culture so that the users do not end up having commas instead of dots for non integers.

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
            return "#" + A.ToString("X2") + R.ToString("X2") + G.ToString("X2") + B.ToString("X2");
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
                Color color = (Color)o;

                return (this == color);
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
            return (!(color1 == color2));
        }
    }
}