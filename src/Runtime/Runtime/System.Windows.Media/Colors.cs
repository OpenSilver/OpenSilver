
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

namespace System.Windows.Media
{
    /// <summary>
    /// Implements a set of predefined colors. See Color for usage information.
    /// </summary>
    public static class Colors
    {
        internal enum KnownColor : uint
        {
            AliceBlue = 0xFFF0F8FF,
            AntiqueWhite = 0xFFFAEBD7,
            Aqua = 0xFF00FFFF,
            Aquamarine = 0xFF7FFFD4,
            Azure = 0xFFF0FFFF,
            Beige = 0xFFF5F5DC,
            Bisque = 0xFFFFE4C4,
            Black = 0xFF000000,
            BlanchedAlmond = 0xFFFFEBCD,
            Blue = 0xFF0000FF,
            BlueViolet = 0xFF8A2BE2,
            Brown = 0xFFA52A2A,
            BurlyWood = 0xFFDEB887,
            CadetBlue = 0xFF5F9EA0,
            Chartreuse = 0xFF7FFF00,
            Chocolate = 0xFFD2691E,
            Coral = 0xFFFF7F50,
            CornflowerBlue = 0xFF6495ED,
            Cornsilk = 0xFFFFF8DC,
            Crimson = 0xFFDC143C,
            Cyan = 0xFF00FFFF,
            DarkBlue = 0xFF00008B,
            DarkCyan = 0xFF008B8B,
            DarkGoldenrod = 0xFFB8860B,
            DarkGray = 0xFFA9A9A9,
            DarkGreen = 0xFF006400,
            DarkKhaki = 0xFFBDB76B,
            DarkMagenta = 0xFF8B008B,
            DarkOliveGreen = 0xFF556B2F,
            DarkOrange = 0xFFFF8C00,
            DarkOrchid = 0xFF9932CC,
            DarkRed = 0xFF8B0000,
            DarkSalmon = 0xFFE9967A,
            DarkSeaGreen = 0xFF8FBC8F,
            DarkSlateBlue = 0xFF483D8B,
            DarkSlateGray = 0xFF2F4F4F,
            DarkTurquoise = 0xFF00CED1,
            DarkViolet = 0xFF9400D3,
            DeepPink = 0xFFFF1493,
            DeepSkyBlue = 0xFF00BFFF,
            DimGray = 0xFF696969,
            DodgerBlue = 0xFF1E90FF,
            Firebrick = 0xFFB22222,
            FloralWhite = 0xFFFFFAF0,
            ForestGreen = 0xFF228B22,
            Fuchsia = 0xFFFF00FF,
            Gainsboro = 0xFFDCDCDC,
            GhostWhite = 0xFFF8F8FF,
            Gold = 0xFFFFD700,
            Goldenrod = 0xFFDAA520,
            Gray = 0xFF808080,
            Green = 0xFF008000,
            GreenYellow = 0xFFADFF2F,
            Honeydew = 0xFFF0FFF0,
            HotPink = 0xFFFF69B4,
            IndianRed = 0xFFCD5C5C,
            Indigo = 0xFF4B0082,
            Ivory = 0xFFFFFFF0,
            Khaki = 0xFFF0E68C,
            Lavender = 0xFFE6E6FA,
            LavenderBlush = 0xFFFFF0F5,
            LawnGreen = 0xFF7CFC00,
            LemonChiffon = 0xFFFFFACD,
            LightBlue = 0xFFADD8E6,
            LightCoral = 0xFFF08080,
            LightCyan = 0xFFE0FFFF,
            LightGoldenrodYellow = 0xFFFAFAD2,
            LightGreen = 0xFF90EE90,
            LightGray = 0xFFD3D3D3,
            LightPink = 0xFFFFB6C1,
            LightSalmon = 0xFFFFA07A,
            LightSeaGreen = 0xFF20B2AA,
            LightSkyBlue = 0xFF87CEFA,
            LightSlateGray = 0xFF778899,
            LightSteelBlue = 0xFFB0C4DE,
            LightYellow = 0xFFFFFFE0,
            Lime = 0xFF00FF00,
            LimeGreen = 0xFF32CD32,
            Linen = 0xFFFAF0E6,
            Magenta = 0xFFFF00FF,
            Maroon = 0xFF800000,
            MediumAquamarine = 0xFF66CDAA,
            MediumBlue = 0xFF0000CD,
            MediumOrchid = 0xFFBA55D3,
            MediumPurple = 0xFF9370DB,
            MediumSeaGreen = 0xFF3CB371,
            MediumSlateBlue = 0xFF7B68EE,
            MediumSpringGreen = 0xFF00FA9A,
            MediumTurquoise = 0xFF48D1CC,
            MediumVioletRed = 0xFFC71585,
            MidnightBlue = 0xFF191970,
            MintCream = 0xFFF5FFFA,
            MistyRose = 0xFFFFE4E1,
            Moccasin = 0xFFFFE4B5,
            NavajoWhite = 0xFFFFDEAD,
            Navy = 0xFF000080,
            OldLace = 0xFFFDF5E6,
            Olive = 0xFF808000,
            OliveDrab = 0xFF6B8E23,
            Orange = 0xFFFFA500,
            OrangeRed = 0xFFFF4500,
            Orchid = 0xFFDA70D6,
            PaleGoldenrod = 0xFFEEE8AA,
            PaleGreen = 0xFF98FB98,
            PaleTurquoise = 0xFFAFEEEE,
            PaleVioletRed = 0xFFDB7093,
            PapayaWhip = 0xFFFFEFD5,
            PeachPuff = 0xFFFFDAB9,
            Peru = 0xFFCD853F,
            Pink = 0xFFFFC0CB,
            Plum = 0xFFDDA0DD,
            PowderBlue = 0xFFB0E0E6,
            Purple = 0xFF800080,
            Red = 0xFFFF0000,
            RosyBrown = 0xFFBC8F8F,
            RoyalBlue = 0xFF4169E1,
            SaddleBrown = 0xFF8B4513,
            Salmon = 0xFFFA8072,
            SandyBrown = 0xFFF4A460,
            SeaGreen = 0xFF2E8B57,
            SeaShell = 0xFFFFF5EE,
            Sienna = 0xFFA0522D,
            Silver = 0xFFC0C0C0,
            SkyBlue = 0xFF87CEEB,
            SlateBlue = 0xFF6A5ACD,
            SlateGray = 0xFF708090,
            Snow = 0xFFFFFAFA,
            SpringGreen = 0xFF00FF7F,
            SteelBlue = 0xFF4682B4,
            Tan = 0xFFD2B48C,
            Teal = 0xFF008080,
            Thistle = 0xFFD8BFD8,
            Tomato = 0xFFFF6347,
            Transparent = 0x00FFFFFF,
            Turquoise = 0xFF40E0D0,
            Violet = 0xFFEE82EE,
            Wheat = 0xFFF5DEB3,
            White = 0xFFFFFFFF,
            WhiteSmoke = 0xFFF5F5F5,
            Yellow = 0xFFFFFF00,
            YellowGreen = 0xFF9ACD32,
        }

        internal static string MatchColor(string colorString, out bool isKnownColor, out bool isNumericColor, out bool isScRgbColor)
        {
            string trimmedString = colorString.Trim();

            if (((trimmedString.Length == 4) ||
                (trimmedString.Length == 5) ||
                (trimmedString.Length == 7) ||
                (trimmedString.Length == 9)) &&
                (trimmedString[0] == '#'))
            {
                isNumericColor = true;
                isScRgbColor = false;
                isKnownColor = false;
            }
            else if (trimmedString.StartsWith("sc#", StringComparison.Ordinal))
            {
                isNumericColor = false;
                isScRgbColor = true;
                isKnownColor = false;
            }
            else
            {
                isNumericColor = false;
                isScRgbColor = false;
                isKnownColor = true;
            }

            return trimmedString;
        }

        /// <summary>
        /// Gets the color value that represents the AliceBlue named color.
        /// </summary>
        public static Color AliceBlue
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.AliceBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the AntiqueWhite named color.
        /// </summary>
        public static Color AntiqueWhite
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.AntiqueWhite);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Aqua named color.
        /// </summary>
        public static Color Aqua
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Aqua);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Aquamarine named color.
        /// </summary>
        public static Color Aquamarine
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Aquamarine);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Azure named color.
        /// </summary>
        public static Color Azure
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Azure);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Beige named color.
        /// </summary>
        public static Color Beige
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Beige);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Bisque named color.
        /// </summary>
        public static Color Bisque
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Bisque);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Black named color.
        /// </summary>
        public static Color Black
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Black);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the BlanchedAlmond named color.
        /// </summary>
        public static Color BlanchedAlmond
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.BlanchedAlmond);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Blue named color.
        /// </summary>
        public static Color Blue
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Blue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the BlueViolet named color.
        /// </summary>
        public static Color BlueViolet
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.BlueViolet);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Brown named color.
        /// </summary>
        public static Color Brown
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Brown);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the BurlyWood named color.
        /// </summary>
        public static Color BurlyWood
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.BurlyWood);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the CadetBlue named color.
        /// </summary>
        public static Color CadetBlue
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.CadetBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Chartreuse named color.
        /// </summary>
        public static Color Chartreuse
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Chartreuse);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Chocolate named color.
        /// </summary>
        public static Color Chocolate
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Chocolate);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Coral named color.
        /// </summary>
        public static Color Coral
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Coral);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the CornflowerBlue named color.
        /// </summary>
        public static Color CornflowerBlue
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.CornflowerBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Cornsilk named color.
        /// </summary>
        public static Color Cornsilk
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Cornsilk);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Crimson named color.
        /// </summary>
        public static Color Crimson
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Crimson);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Cyan named color.
        /// </summary>
        public static Color Cyan
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Cyan);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkBlue named color.
        /// </summary>
        public static Color DarkBlue
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DarkBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkCyan named color.
        /// </summary>
        public static Color DarkCyan
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DarkCyan);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkGoldenrod named color.
        /// </summary>
        public static Color DarkGoldenrod
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DarkGoldenrod);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkGray named color.
        /// </summary>
        public static Color DarkGray
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DarkGray);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkGreen named color.
        /// </summary>
        public static Color DarkGreen
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DarkGreen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkKhaki named color.
        /// </summary>
        public static Color DarkKhaki
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DarkKhaki);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkMagenta named color.
        /// </summary>
        public static Color DarkMagenta
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DarkMagenta);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkOliveGreen named color.
        /// </summary>
        public static Color DarkOliveGreen
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DarkOliveGreen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkOrange named color.
        /// </summary>
        public static Color DarkOrange
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DarkOrange);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkOrchid named color.
        /// </summary>
        public static Color DarkOrchid
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DarkOrchid);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkRed named color.
        /// </summary>
        public static Color DarkRed
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DarkRed);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkSalmon named color.
        /// </summary>
        public static Color DarkSalmon
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DarkSalmon);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkSeaGreen named color.
        /// </summary>
        public static Color DarkSeaGreen
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DarkSeaGreen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkSlateBlue named color.
        /// </summary>
        public static Color DarkSlateBlue
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DarkSlateBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkSlateGray named color.
        /// </summary>
        public static Color DarkSlateGray
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DarkSlateGray);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkTurquoise named color.
        /// </summary>
        public static Color DarkTurquoise
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DarkTurquoise);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkViolet named color.
        /// </summary>
        public static Color DarkViolet
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DarkViolet);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DeepPink named color.
        /// </summary>
        public static Color DeepPink
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DeepPink);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DeepSkyBlue named color.
        /// </summary>
        public static Color DeepSkyBlue
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DeepSkyBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DimGray named color.
        /// </summary>
        public static Color DimGray
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DimGray);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DodgerBlue named color.
        /// </summary>
        public static Color DodgerBlue
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DodgerBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Firebrick named color.
        /// </summary>
        public static Color Firebrick
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Firebrick);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the FloralWhite named color.
        /// </summary>
        public static Color FloralWhite
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.FloralWhite);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the ForestGreen named color.
        /// </summary>
        public static Color ForestGreen
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.ForestGreen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Fuchsia named color.
        /// </summary>
        public static Color Fuchsia
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Fuchsia);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Gainsboro named color.
        /// </summary>
        public static Color Gainsboro
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Gainsboro);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the GhostWhite named color.
        /// </summary>
        public static Color GhostWhite
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.GhostWhite);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Gold named color.
        /// </summary>
        public static Color Gold
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Gold);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Goldenrod named color.
        /// </summary>
        public static Color Goldenrod
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Goldenrod);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Gray named color.
        /// </summary>
        public static Color Gray
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Gray);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Green named color.
        /// </summary>
        public static Color Green
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Green);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the GreenYellow named color.
        /// </summary>
        public static Color GreenYellow
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.GreenYellow);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Honeydew named color.
        /// </summary>
        public static Color Honeydew
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Honeydew);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the HotPink named color.
        /// </summary>
        public static Color HotPink
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.HotPink);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the IndianRed named color.
        /// </summary>
        public static Color IndianRed
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.IndianRed);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Indigo named color.
        /// </summary>
        public static Color Indigo
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Indigo);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Ivory named color.
        /// </summary>
        public static Color Ivory
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Ivory);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Khaki named color.
        /// </summary>
        public static Color Khaki
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Khaki);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Lavender named color.
        /// </summary>
        public static Color Lavender
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Lavender);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LavenderBlush named color.
        /// </summary>
        public static Color LavenderBlush
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.LavenderBlush);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LawnGreen named color.
        /// </summary>
        public static Color LawnGreen
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.LawnGreen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LemonChiffon named color.
        /// </summary>
        public static Color LemonChiffon
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.LemonChiffon);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LightBlue named color.
        /// </summary>
        public static Color LightBlue
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.LightBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LightCoral named color.
        /// </summary>
        public static Color LightCoral
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.LightCoral);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LightCyan named color.
        /// </summary>
        public static Color LightCyan
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.LightCyan);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LightGoldenrodYellow named color.
        /// </summary>
        public static Color LightGoldenrodYellow
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.LightGoldenrodYellow);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LightGray named color.
        /// </summary>
        public static Color LightGray
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.LightGray);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LightGreen named color.
        /// </summary>
        public static Color LightGreen
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.LightGreen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LightPink named color.
        /// </summary>
        public static Color LightPink
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.LightPink);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LightSalmon named color.
        /// </summary>
        public static Color LightSalmon
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.LightSalmon);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LightSeaGreen named color.
        /// </summary>
        public static Color LightSeaGreen
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.LightSeaGreen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LightSkyBlue named color.
        /// </summary>
        public static Color LightSkyBlue
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.LightSkyBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LightSlateGray named color.
        /// </summary>
        public static Color LightSlateGray
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.LightSlateGray);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LightSteelBlue named color.
        /// </summary>
        public static Color LightSteelBlue
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.LightSteelBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LightYellow named color.
        /// </summary>
        public static Color LightYellow
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.LightYellow);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Lime named color.
        /// </summary>
        public static Color Lime
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Lime);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LimeGreen named color.
        /// </summary>
        public static Color LimeGreen
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.LimeGreen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Linen named color.
        /// </summary>
        public static Color Linen
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Linen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Magenta named color.
        /// </summary>
        public static Color Magenta
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Magenta);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Maroon named color.
        /// </summary>
        public static Color Maroon
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Maroon);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the MediumAquamarine named color.
        /// </summary>
        public static Color MediumAquamarine
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.MediumAquamarine);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the MediumBlue named color.
        /// </summary>
        public static Color MediumBlue
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.MediumBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the MediumOrchid named color.
        /// </summary>
        public static Color MediumOrchid
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.MediumOrchid);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the MediumPurple named color.
        /// </summary>
        public static Color MediumPurple
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.MediumPurple);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the MediumSeaGreen named color.
        /// </summary>
        public static Color MediumSeaGreen
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.MediumSeaGreen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the MediumSlateBlue named color.
        /// </summary>
        public static Color MediumSlateBlue
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.MediumSlateBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the MediumSpringGreen named color.
        /// </summary>
        public static Color MediumSpringGreen
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.MediumSpringGreen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the MediumTurquoise named color.
        /// </summary>
        public static Color MediumTurquoise
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.MediumTurquoise);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the MediumVioletRed named color.
        /// </summary>
        public static Color MediumVioletRed
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.MediumVioletRed);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the MidnightBlue named color.
        /// </summary>
        public static Color MidnightBlue
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.MidnightBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the MintCream named color.
        /// </summary>
        public static Color MintCream
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.MintCream);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the MistyRose named color.
        /// </summary>
        public static Color MistyRose
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.MistyRose);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Moccasin named color.
        /// </summary>
        public static Color Moccasin
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Moccasin);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the NavajoWhite named color.
        /// </summary>
        public static Color NavajoWhite
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.NavajoWhite);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Navy named color.
        /// </summary>
        public static Color Navy
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Navy);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the OldLace named color.
        /// </summary>
        public static Color OldLace
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.OldLace);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Olive named color.
        /// </summary>
        public static Color Olive
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Olive);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the OliveDrab named color.
        /// </summary>
        public static Color OliveDrab
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.OliveDrab);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Orange named color.
        /// </summary>
        public static Color Orange
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Orange);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the OrangeRed named color.
        /// </summary>
        public static Color OrangeRed
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.OrangeRed);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Orchid named color.
        /// </summary>
        public static Color Orchid
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Orchid);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the PaleGoldenrod named color.
        /// </summary>
        public static Color PaleGoldenrod
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.PaleGoldenrod);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the PaleGreen named color.
        /// </summary>
        public static Color PaleGreen
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.PaleGreen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the PaleTurquoise named color.
        /// </summary>
        public static Color PaleTurquoise
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.PaleTurquoise);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the PaleVioletRed named color.
        /// </summary>
        public static Color PaleVioletRed
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.PaleVioletRed);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the PapayaWhip named color.
        /// </summary>
        public static Color PapayaWhip
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.PapayaWhip);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the PeachPuff named color.
        /// </summary>
        public static Color PeachPuff
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.PeachPuff);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Peru named color.
        /// </summary>
        public static Color Peru
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Peru);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Pink named color.
        /// </summary>
        public static Color Pink
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Pink);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Plum named color.
        /// </summary>
        public static Color Plum
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Plum);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the PowderBlue named color.
        /// </summary>
        public static Color PowderBlue
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.PowderBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Purple named color.
        /// </summary>
        public static Color Purple
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Purple);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Red named color.
        /// </summary>
        public static Color Red
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Red);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the RosyBrown named color.
        /// </summary>
        public static Color RosyBrown
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.RosyBrown);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the RoyalBlue named color.
        /// </summary>
        public static Color RoyalBlue
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.RoyalBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the SaddleBrown named color.
        /// </summary>
        public static Color SaddleBrown
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.SaddleBrown);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Salmon named color.
        /// </summary>
        public static Color Salmon
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Salmon);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the SandyBrown named color.
        /// </summary>
        public static Color SandyBrown
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.SandyBrown);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the SeaGreen named color.
        /// </summary>
        public static Color SeaGreen
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.SeaGreen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the SeaShell named color.
        /// </summary>
        public static Color SeaShell
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.SeaShell);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Sienna named color.
        /// </summary>
        public static Color Sienna
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Sienna);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Silver named color.
        /// </summary>
        public static Color Silver
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Silver);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the SkyBlue named color.
        /// </summary>
        public static Color SkyBlue
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.SkyBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the SlateBlue named color.
        /// </summary>
        public static Color SlateBlue
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.SlateBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the SlateGray named color.
        /// </summary>
        public static Color SlateGray
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.SlateGray);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Snow named color.
        /// </summary>
        public static Color Snow
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Snow);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the SpringGreen named color.
        /// </summary>
        public static Color SpringGreen
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.SpringGreen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the SteelBlue named color.
        /// </summary>
        public static Color SteelBlue
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.SteelBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Tan named color.
        /// </summary>
        public static Color Tan
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Tan);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Teal named color.
        /// </summary>
        public static Color Teal
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Teal);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Thistle named color.
        /// </summary>
        public static Color Thistle
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Thistle);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Tomato named color.
        /// </summary>
        public static Color Tomato
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Tomato);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Transparent named color.
        /// </summary>
        public static Color Transparent
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Transparent);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Turquoise named color.
        /// </summary>
        public static Color Turquoise
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Turquoise);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Violet named color.
        /// </summary>
        public static Color Violet
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Violet);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Wheat named color.
        /// </summary>
        public static Color Wheat
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Wheat);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the White named color.
        /// </summary>
        public static Color White
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.White);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the WhiteSmoke named color.
        /// </summary>
        public static Color WhiteSmoke
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.WhiteSmoke);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Yellow named color.
        /// </summary>
        public static Color Yellow
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Yellow);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the YellowGreen named color.
        /// </summary>
        public static Color YellowGreen
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.YellowGreen);
            }
        }
    }
}