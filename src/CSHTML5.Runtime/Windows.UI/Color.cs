

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
    public partial struct Color //todo: this is supposed to inherit from IFormattable
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

        internal string INTERNAL_ToHtmlString(double opacity) // Note: we didn't use a default value for the "opacity" argument to force the caller to ask herself if it is correct to call this method or if she should call "SolidColorBrush.INTERNAL_ToHtmlString()" instead (because the latter takes into account the "Brush.Opacity" property).
        {
            return "rgba(" + R.ToString() + ", " + G.ToString() + ", " + B.ToString() + ", " + (((double)A / 255) * opacity).ToString().Replace(',', '.') + ")"; //todo: instead of calling the "Replace" method, use ToString(CultureInfo.InvariantCulture) when CSHTML5 will support it.
        }

        internal static object INTERNAL_ConvertFromString(string colorcode)
        {
            try
            {
                // Check if the color is a named color:
                if (!colorcode.StartsWith("#"))
                {
                    Colors.INTERNAL_ColorsEnum namedColor = (Colors.INTERNAL_ColorsEnum)Enum.Parse(typeof(Colors.INTERNAL_ColorsEnum), colorcode, true); // Note: "TryParse" does not seem to work in JSIL.
                    return INTERNAL_ConvertFromInt32((int)namedColor);
                }
                else
                {
                    colorcode = colorcode.Replace("#", "");
                    if (colorcode.Length == 6) // This is becaue XAML is tolerant when the user has forgot the alpha channel (eg. #DDDDDD for Gray).
                        colorcode = "FF" + colorcode;
#if !BRIDGE
                    int colorAsInt = int.Parse(colorcode.Replace("#", ""), NumberStyles.HexNumber);
#else
                    int colorAsInt;
                    if (CSHTML5.Interop.IsRunningInTheSimulator)
                    {
                        colorcode = colorcode.Replace("#", "");
                        colorAsInt = INTERNAL_BridgeWorkarounds.HexToInt_SimulatorOnly(colorcode);
                    }
                    else
                    {
                        colorAsInt = Script.Write<int>("parseInt({0}, 16);", colorcode.Replace("#", ""));
                    }
#endif
                    return INTERNAL_ConvertFromInt32(colorAsInt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid color: " + colorcode, ex);
            }
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