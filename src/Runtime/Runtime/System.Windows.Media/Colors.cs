
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
        internal enum KnownColor : int
        {
            AliceBlue = unchecked((int)0xFFF0F8FF),
            AntiqueWhite = unchecked((int)0xFFFAEBD7),
            Aqua = unchecked((int)0xFF00FFFF),
            Aquamarine = unchecked((int)0xFF7FFFD4),
            Azure = unchecked((int)0xFFF0FFFF),
            Beige = unchecked((int)0xFFF5F5DC),
            Bisque = unchecked((int)0xFFFFE4C4),
            Black = unchecked((int)0xFF000000),
            BlanchedAlmond = unchecked((int)0xFFFFEBCD),
            Blue = unchecked((int)0xFF0000FF),
            BlueViolet = unchecked((int)0xFF8A2BE2),
            Brown = unchecked((int)0xFFA52A2A),
            BurlyWood = unchecked((int)0xFFDEB887),
            CadetBlue = unchecked((int)0xFF5F9EA0),
            Chartreuse = unchecked((int)0xFF7FFF00),
            Chocolate = unchecked((int)0xFFD2691E),
            Coral = unchecked((int)0xFFFF7F50),
            CornflowerBlue = unchecked((int)0xFF6495ED),
            Cornsilk = unchecked((int)0xFFFFF8DC),
            Crimson = unchecked((int)0xFFDC143C),
            Cyan = unchecked((int)0xFF00FFFF),
            DarkBlue = unchecked((int)0xFF00008B),
            DarkCyan = unchecked((int)0xFF008B8B),
            DarkGoldenrod = unchecked((int)0xFFB8860B),
            DarkGray = unchecked((int)0xFFA9A9A9),
            DarkGreen = unchecked((int)0xFF006400),
            DarkKhaki = unchecked((int)0xFFBDB76B),
            DarkMagenta = unchecked((int)0xFF8B008B),
            DarkOliveGreen = unchecked((int)0xFF556B2F),
            DarkOrange = unchecked((int)0xFFFF8C00),
            DarkOrchid = unchecked((int)0xFF9932CC),
            DarkRed = unchecked((int)0xFF8B0000),
            DarkSalmon = unchecked((int)0xFFE9967A),
            DarkSeaGreen = unchecked((int)0xFF8FBC8F),
            DarkSlateBlue = unchecked((int)0xFF483D8B),
            DarkSlateGray = unchecked((int)0xFF2F4F4F),
            DarkTurquoise = unchecked((int)0xFF00CED1),
            DarkViolet = unchecked((int)0xFF9400D3),
            DeepPink = unchecked((int)0xFFFF1493),
            DeepSkyBlue = unchecked((int)0xFF00BFFF),
            DimGray = unchecked((int)0xFF696969),
            DodgerBlue = unchecked((int)0xFF1E90FF),
            Firebrick = unchecked((int)0xFFB22222),
            FloralWhite = unchecked((int)0xFFFFFAF0),
            ForestGreen = unchecked((int)0xFF228B22),
            Fuchsia = unchecked((int)0xFFFF00FF),
            Gainsboro = unchecked((int)0xFFDCDCDC),
            GhostWhite = unchecked((int)0xFFF8F8FF),
            Gold = unchecked((int)0xFFFFD700),
            Goldenrod = unchecked((int)0xFFDAA520),
            Gray = unchecked((int)0xFF808080),
            Green = unchecked((int)0xFF008000),
            GreenYellow = unchecked((int)0xFFADFF2F),
            Honeydew = unchecked((int)0xFFF0FFF0),
            HotPink = unchecked((int)0xFFFF69B4),
            IndianRed = unchecked((int)0xFFCD5C5C),
            Indigo = unchecked((int)0xFF4B0082),
            Ivory = unchecked((int)0xFFFFFFF0),
            Khaki = unchecked((int)0xFFF0E68C),
            Lavender = unchecked((int)0xFFE6E6FA),
            LavenderBlush = unchecked((int)0xFFFFF0F5),
            LawnGreen = unchecked((int)0xFF7CFC00),
            LemonChiffon = unchecked((int)0xFFFFFACD),
            LightBlue = unchecked((int)0xFFADD8E6),
            LightCoral = unchecked((int)0xFFF08080),
            LightCyan = unchecked((int)0xFFE0FFFF),
            LightGoldenrodYellow = unchecked((int)0xFFFAFAD2),
            LightGreen = unchecked((int)0xFF90EE90),
            LightGray = unchecked((int)0xFFD3D3D3),
            LightPink = unchecked((int)0xFFFFB6C1),
            LightSalmon = unchecked((int)0xFFFFA07A),
            LightSeaGreen = unchecked((int)0xFF20B2AA),
            LightSkyBlue = unchecked((int)0xFF87CEFA),
            LightSlateGray = unchecked((int)0xFF778899),
            LightSteelBlue = unchecked((int)0xFFB0C4DE),
            LightYellow = unchecked((int)0xFFFFFFE0),
            Lime = unchecked((int)0xFF00FF00),
            LimeGreen = unchecked((int)0xFF32CD32),
            Linen = unchecked((int)0xFFFAF0E6),
            Magenta = unchecked((int)0xFFFF00FF),
            Maroon = unchecked((int)0xFF800000),
            MediumAquamarine = unchecked((int)0xFF66CDAA),
            MediumBlue = unchecked((int)0xFF0000CD),
            MediumOrchid = unchecked((int)0xFFBA55D3),
            MediumPurple = unchecked((int)0xFF9370DB),
            MediumSeaGreen = unchecked((int)0xFF3CB371),
            MediumSlateBlue = unchecked((int)0xFF7B68EE),
            MediumSpringGreen = unchecked((int)0xFF00FA9A),
            MediumTurquoise = unchecked((int)0xFF48D1CC),
            MediumVioletRed = unchecked((int)0xFFC71585),
            MidnightBlue = unchecked((int)0xFF191970),
            MintCream = unchecked((int)0xFFF5FFFA),
            MistyRose = unchecked((int)0xFFFFE4E1),
            Moccasin = unchecked((int)0xFFFFE4B5),
            NavajoWhite = unchecked((int)0xFFFFDEAD),
            Navy = unchecked((int)0xFF000080),
            OldLace = unchecked((int)0xFFFDF5E6),
            Olive = unchecked((int)0xFF808000),
            OliveDrab = unchecked((int)0xFF6B8E23),
            Orange = unchecked((int)0xFFFFA500),
            OrangeRed = unchecked((int)0xFFFF4500),
            Orchid = unchecked((int)0xFFDA70D6),
            PaleGoldenrod = unchecked((int)0xFFEEE8AA),
            PaleGreen = unchecked((int)0xFF98FB98),
            PaleTurquoise = unchecked((int)0xFFAFEEEE),
            PaleVioletRed = unchecked((int)0xFFDB7093),
            PapayaWhip = unchecked((int)0xFFFFEFD5),
            PeachPuff = unchecked((int)0xFFFFDAB9),
            Peru = unchecked((int)0xFFCD853F),
            Pink = unchecked((int)0xFFFFC0CB),
            Plum = unchecked((int)0xFFDDA0DD),
            PowderBlue = unchecked((int)0xFFB0E0E6),
            Purple = unchecked((int)0xFF800080),
            Red = unchecked((int)0xFFFF0000),
            RosyBrown = unchecked((int)0xFFBC8F8F),
            RoyalBlue = unchecked((int)0xFF4169E1),
            SaddleBrown = unchecked((int)0xFF8B4513),
            Salmon = unchecked((int)0xFFFA8072),
            SandyBrown = unchecked((int)0xFFF4A460),
            SeaGreen = unchecked((int)0xFF2E8B57),
            SeaShell = unchecked((int)0xFFFFF5EE),
            Sienna = unchecked((int)0xFFA0522D),
            Silver = unchecked((int)0xFFC0C0C0),
            SkyBlue = unchecked((int)0xFF87CEEB),
            SlateBlue = unchecked((int)0xFF6A5ACD),
            SlateGray = unchecked((int)0xFF708090),
            Snow = unchecked((int)0xFFFFFAFA),
            SpringGreen = unchecked((int)0xFF00FF7F),
            SteelBlue = unchecked((int)0xFF4682B4),
            Tan = unchecked((int)0xFFD2B48C),
            Teal = unchecked((int)0xFF008080),
            Thistle = unchecked((int)0xFFD8BFD8),
            Tomato = unchecked((int)0xFFFF6347),
            Transparent = unchecked((int)0x00FFFFFF),
            Turquoise = unchecked((int)0xFF40E0D0),
            Violet = unchecked((int)0xFFEE82EE),
            Wheat = unchecked((int)0xFFF5DEB3),
            White = unchecked((int)0xFFFFFFFF),
            WhiteSmoke = unchecked((int)0xFFF5F5F5),
            Yellow = unchecked((int)0xFFFFFF00),
            YellowGreen = unchecked((int)0xFF9ACD32),
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
                return Color.FromInt32((int)KnownColor.AliceBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the AntiqueWhite named color.
        /// </summary>
        public static Color AntiqueWhite
        {
            get
            {
                return Color.FromInt32((int)KnownColor.AntiqueWhite);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Aqua named color.
        /// </summary>
        public static Color Aqua
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Aqua);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Aquamarine named color.
        /// </summary>
        public static Color Aquamarine
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Aquamarine);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Azure named color.
        /// </summary>
        public static Color Azure
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Azure);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Beige named color.
        /// </summary>
        public static Color Beige
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Beige);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Bisque named color.
        /// </summary>
        public static Color Bisque
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Bisque);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Black named color.
        /// </summary>
        public static Color Black
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Black);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the BlanchedAlmond named color.
        /// </summary>
        public static Color BlanchedAlmond
        {
            get
            {
                return Color.FromInt32((int)KnownColor.BlanchedAlmond);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Blue named color.
        /// </summary>
        public static Color Blue
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Blue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the BlueViolet named color.
        /// </summary>
        public static Color BlueViolet
        {
            get
            {
                return Color.FromInt32((int)KnownColor.BlueViolet);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Brown named color.
        /// </summary>
        public static Color Brown
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Brown);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the BurlyWood named color.
        /// </summary>
        public static Color BurlyWood
        {
            get
            {
                return Color.FromInt32((int)KnownColor.BurlyWood);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the CadetBlue named color.
        /// </summary>
        public static Color CadetBlue
        {
            get
            {
                return Color.FromInt32((int)KnownColor.CadetBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Chartreuse named color.
        /// </summary>
        public static Color Chartreuse
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Chartreuse);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Chocolate named color.
        /// </summary>
        public static Color Chocolate
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Chocolate);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Coral named color.
        /// </summary>
        public static Color Coral
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Coral);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the CornflowerBlue named color.
        /// </summary>
        public static Color CornflowerBlue
        {
            get
            {
                return Color.FromInt32((int)KnownColor.CornflowerBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Cornsilk named color.
        /// </summary>
        public static Color Cornsilk
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Cornsilk);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Crimson named color.
        /// </summary>
        public static Color Crimson
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Crimson);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Cyan named color.
        /// </summary>
        public static Color Cyan
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Cyan);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkBlue named color.
        /// </summary>
        public static Color DarkBlue
        {
            get
            {
                return Color.FromInt32((int)KnownColor.DarkBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkCyan named color.
        /// </summary>
        public static Color DarkCyan
        {
            get
            {
                return Color.FromInt32((int)KnownColor.DarkCyan);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkGoldenrod named color.
        /// </summary>
        public static Color DarkGoldenrod
        {
            get
            {
                return Color.FromInt32((int)KnownColor.DarkGoldenrod);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkGray named color.
        /// </summary>
        public static Color DarkGray
        {
            get
            {
                return Color.FromInt32((int)KnownColor.DarkGray);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkGreen named color.
        /// </summary>
        public static Color DarkGreen
        {
            get
            {
                return Color.FromInt32((int)KnownColor.DarkGreen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkKhaki named color.
        /// </summary>
        public static Color DarkKhaki
        {
            get
            {
                return Color.FromInt32((int)KnownColor.DarkKhaki);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkMagenta named color.
        /// </summary>
        public static Color DarkMagenta
        {
            get
            {
                return Color.FromInt32((int)KnownColor.DarkMagenta);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkOliveGreen named color.
        /// </summary>
        public static Color DarkOliveGreen
        {
            get
            {
                return Color.FromInt32((int)KnownColor.DarkOliveGreen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkOrange named color.
        /// </summary>
        public static Color DarkOrange
        {
            get
            {
                return Color.FromInt32((int)KnownColor.DarkOrange);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkOrchid named color.
        /// </summary>
        public static Color DarkOrchid
        {
            get
            {
                return Color.FromInt32((int)KnownColor.DarkOrchid);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkRed named color.
        /// </summary>
        public static Color DarkRed
        {
            get
            {
                return Color.FromInt32((int)KnownColor.DarkRed);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkSalmon named color.
        /// </summary>
        public static Color DarkSalmon
        {
            get
            {
                return Color.FromInt32((int)KnownColor.DarkSalmon);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkSeaGreen named color.
        /// </summary>
        public static Color DarkSeaGreen
        {
            get
            {
                return Color.FromInt32((int)KnownColor.DarkSeaGreen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkSlateBlue named color.
        /// </summary>
        public static Color DarkSlateBlue
        {
            get
            {
                return Color.FromInt32((int)KnownColor.DarkSlateBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkSlateGray named color.
        /// </summary>
        public static Color DarkSlateGray
        {
            get
            {
                return Color.FromInt32((int)KnownColor.DarkSlateGray);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkTurquoise named color.
        /// </summary>
        public static Color DarkTurquoise
        {
            get
            {
                return Color.FromInt32((int)KnownColor.DarkTurquoise);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkViolet named color.
        /// </summary>
        public static Color DarkViolet
        {
            get
            {
                return Color.FromInt32((int)KnownColor.DarkViolet);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DeepPink named color.
        /// </summary>
        public static Color DeepPink
        {
            get
            {
                return Color.FromInt32((int)KnownColor.DeepPink);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DeepSkyBlue named color.
        /// </summary>
        public static Color DeepSkyBlue
        {
            get
            {
                return Color.FromInt32((int)KnownColor.DeepSkyBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DimGray named color.
        /// </summary>
        public static Color DimGray
        {
            get
            {
                return Color.FromInt32((int)KnownColor.DimGray);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DodgerBlue named color.
        /// </summary>
        public static Color DodgerBlue
        {
            get
            {
                return Color.FromInt32((int)KnownColor.DodgerBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Firebrick named color.
        /// </summary>
        public static Color Firebrick
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Firebrick);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the FloralWhite named color.
        /// </summary>
        public static Color FloralWhite
        {
            get
            {
                return Color.FromInt32((int)KnownColor.FloralWhite);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the ForestGreen named color.
        /// </summary>
        public static Color ForestGreen
        {
            get
            {
                return Color.FromInt32((int)KnownColor.ForestGreen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Fuchsia named color.
        /// </summary>
        public static Color Fuchsia
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Fuchsia);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Gainsboro named color.
        /// </summary>
        public static Color Gainsboro
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Gainsboro);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the GhostWhite named color.
        /// </summary>
        public static Color GhostWhite
        {
            get
            {
                return Color.FromInt32((int)KnownColor.GhostWhite);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Gold named color.
        /// </summary>
        public static Color Gold
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Gold);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Goldenrod named color.
        /// </summary>
        public static Color Goldenrod
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Goldenrod);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Gray named color.
        /// </summary>
        public static Color Gray
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Gray);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Green named color.
        /// </summary>
        public static Color Green
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Green);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the GreenYellow named color.
        /// </summary>
        public static Color GreenYellow
        {
            get
            {
                return Color.FromInt32((int)KnownColor.GreenYellow);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Honeydew named color.
        /// </summary>
        public static Color Honeydew
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Honeydew);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the HotPink named color.
        /// </summary>
        public static Color HotPink
        {
            get
            {
                return Color.FromInt32((int)KnownColor.HotPink);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the IndianRed named color.
        /// </summary>
        public static Color IndianRed
        {
            get
            {
                return Color.FromInt32((int)KnownColor.IndianRed);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Indigo named color.
        /// </summary>
        public static Color Indigo
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Indigo);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Ivory named color.
        /// </summary>
        public static Color Ivory
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Ivory);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Khaki named color.
        /// </summary>
        public static Color Khaki
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Khaki);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Lavender named color.
        /// </summary>
        public static Color Lavender
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Lavender);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LavenderBlush named color.
        /// </summary>
        public static Color LavenderBlush
        {
            get
            {
                return Color.FromInt32((int)KnownColor.LavenderBlush);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LawnGreen named color.
        /// </summary>
        public static Color LawnGreen
        {
            get
            {
                return Color.FromInt32((int)KnownColor.LawnGreen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LemonChiffon named color.
        /// </summary>
        public static Color LemonChiffon
        {
            get
            {
                return Color.FromInt32((int)KnownColor.LemonChiffon);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LightBlue named color.
        /// </summary>
        public static Color LightBlue
        {
            get
            {
                return Color.FromInt32((int)KnownColor.LightBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LightCoral named color.
        /// </summary>
        public static Color LightCoral
        {
            get
            {
                return Color.FromInt32((int)KnownColor.LightCoral);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LightCyan named color.
        /// </summary>
        public static Color LightCyan
        {
            get
            {
                return Color.FromInt32((int)KnownColor.LightCyan);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LightGoldenrodYellow named color.
        /// </summary>
        public static Color LightGoldenrodYellow
        {
            get
            {
                return Color.FromInt32((int)KnownColor.LightGoldenrodYellow);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LightGray named color.
        /// </summary>
        public static Color LightGray
        {
            get
            {
                return Color.FromInt32((int)KnownColor.LightGray);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LightGreen named color.
        /// </summary>
        public static Color LightGreen
        {
            get
            {
                return Color.FromInt32((int)KnownColor.LightGreen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LightPink named color.
        /// </summary>
        public static Color LightPink
        {
            get
            {
                return Color.FromInt32((int)KnownColor.LightPink);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LightSalmon named color.
        /// </summary>
        public static Color LightSalmon
        {
            get
            {
                return Color.FromInt32((int)KnownColor.LightSalmon);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LightSeaGreen named color.
        /// </summary>
        public static Color LightSeaGreen
        {
            get
            {
                return Color.FromInt32((int)KnownColor.LightSeaGreen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LightSkyBlue named color.
        /// </summary>
        public static Color LightSkyBlue
        {
            get
            {
                return Color.FromInt32((int)KnownColor.LightSkyBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LightSlateGray named color.
        /// </summary>
        public static Color LightSlateGray
        {
            get
            {
                return Color.FromInt32((int)KnownColor.LightSlateGray);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LightSteelBlue named color.
        /// </summary>
        public static Color LightSteelBlue
        {
            get
            {
                return Color.FromInt32((int)KnownColor.LightSteelBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LightYellow named color.
        /// </summary>
        public static Color LightYellow
        {
            get
            {
                return Color.FromInt32((int)KnownColor.LightYellow);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Lime named color.
        /// </summary>
        public static Color Lime
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Lime);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LimeGreen named color.
        /// </summary>
        public static Color LimeGreen
        {
            get
            {
                return Color.FromInt32((int)KnownColor.LimeGreen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Linen named color.
        /// </summary>
        public static Color Linen
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Linen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Magenta named color.
        /// </summary>
        public static Color Magenta
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Magenta);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Maroon named color.
        /// </summary>
        public static Color Maroon
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Maroon);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the MediumAquamarine named color.
        /// </summary>
        public static Color MediumAquamarine
        {
            get
            {
                return Color.FromInt32((int)KnownColor.MediumAquamarine);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the MediumBlue named color.
        /// </summary>
        public static Color MediumBlue
        {
            get
            {
                return Color.FromInt32((int)KnownColor.MediumBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the MediumOrchid named color.
        /// </summary>
        public static Color MediumOrchid
        {
            get
            {
                return Color.FromInt32((int)KnownColor.MediumOrchid);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the MediumPurple named color.
        /// </summary>
        public static Color MediumPurple
        {
            get
            {
                return Color.FromInt32((int)KnownColor.MediumPurple);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the MediumSeaGreen named color.
        /// </summary>
        public static Color MediumSeaGreen
        {
            get
            {
                return Color.FromInt32((int)KnownColor.MediumSeaGreen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the MediumSlateBlue named color.
        /// </summary>
        public static Color MediumSlateBlue
        {
            get
            {
                return Color.FromInt32((int)KnownColor.MediumSlateBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the MediumSpringGreen named color.
        /// </summary>
        public static Color MediumSpringGreen
        {
            get
            {
                return Color.FromInt32((int)KnownColor.MediumSpringGreen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the MediumTurquoise named color.
        /// </summary>
        public static Color MediumTurquoise
        {
            get
            {
                return Color.FromInt32((int)KnownColor.MediumTurquoise);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the MediumVioletRed named color.
        /// </summary>
        public static Color MediumVioletRed
        {
            get
            {
                return Color.FromInt32((int)KnownColor.MediumVioletRed);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the MidnightBlue named color.
        /// </summary>
        public static Color MidnightBlue
        {
            get
            {
                return Color.FromInt32((int)KnownColor.MidnightBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the MintCream named color.
        /// </summary>
        public static Color MintCream
        {
            get
            {
                return Color.FromInt32((int)KnownColor.MintCream);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the MistyRose named color.
        /// </summary>
        public static Color MistyRose
        {
            get
            {
                return Color.FromInt32((int)KnownColor.MistyRose);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Moccasin named color.
        /// </summary>
        public static Color Moccasin
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Moccasin);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the NavajoWhite named color.
        /// </summary>
        public static Color NavajoWhite
        {
            get
            {
                return Color.FromInt32((int)KnownColor.NavajoWhite);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Navy named color.
        /// </summary>
        public static Color Navy
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Navy);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the OldLace named color.
        /// </summary>
        public static Color OldLace
        {
            get
            {
                return Color.FromInt32((int)KnownColor.OldLace);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Olive named color.
        /// </summary>
        public static Color Olive
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Olive);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the OliveDrab named color.
        /// </summary>
        public static Color OliveDrab
        {
            get
            {
                return Color.FromInt32((int)KnownColor.OliveDrab);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Orange named color.
        /// </summary>
        public static Color Orange
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Orange);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the OrangeRed named color.
        /// </summary>
        public static Color OrangeRed
        {
            get
            {
                return Color.FromInt32((int)KnownColor.OrangeRed);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Orchid named color.
        /// </summary>
        public static Color Orchid
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Orchid);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the PaleGoldenrod named color.
        /// </summary>
        public static Color PaleGoldenrod
        {
            get
            {
                return Color.FromInt32((int)KnownColor.PaleGoldenrod);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the PaleGreen named color.
        /// </summary>
        public static Color PaleGreen
        {
            get
            {
                return Color.FromInt32((int)KnownColor.PaleGreen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the PaleTurquoise named color.
        /// </summary>
        public static Color PaleTurquoise
        {
            get
            {
                return Color.FromInt32((int)KnownColor.PaleTurquoise);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the PaleVioletRed named color.
        /// </summary>
        public static Color PaleVioletRed
        {
            get
            {
                return Color.FromInt32((int)KnownColor.PaleVioletRed);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the PapayaWhip named color.
        /// </summary>
        public static Color PapayaWhip
        {
            get
            {
                return Color.FromInt32((int)KnownColor.PapayaWhip);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the PeachPuff named color.
        /// </summary>
        public static Color PeachPuff
        {
            get
            {
                return Color.FromInt32((int)KnownColor.PeachPuff);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Peru named color.
        /// </summary>
        public static Color Peru
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Peru);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Pink named color.
        /// </summary>
        public static Color Pink
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Pink);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Plum named color.
        /// </summary>
        public static Color Plum
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Plum);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the PowderBlue named color.
        /// </summary>
        public static Color PowderBlue
        {
            get
            {
                return Color.FromInt32((int)KnownColor.PowderBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Purple named color.
        /// </summary>
        public static Color Purple
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Purple);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Red named color.
        /// </summary>
        public static Color Red
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Red);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the RosyBrown named color.
        /// </summary>
        public static Color RosyBrown
        {
            get
            {
                return Color.FromInt32((int)KnownColor.RosyBrown);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the RoyalBlue named color.
        /// </summary>
        public static Color RoyalBlue
        {
            get
            {
                return Color.FromInt32((int)KnownColor.RoyalBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the SaddleBrown named color.
        /// </summary>
        public static Color SaddleBrown
        {
            get
            {
                return Color.FromInt32((int)KnownColor.SaddleBrown);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Salmon named color.
        /// </summary>
        public static Color Salmon
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Salmon);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the SandyBrown named color.
        /// </summary>
        public static Color SandyBrown
        {
            get
            {
                return Color.FromInt32((int)KnownColor.SandyBrown);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the SeaGreen named color.
        /// </summary>
        public static Color SeaGreen
        {
            get
            {
                return Color.FromInt32((int)KnownColor.SeaGreen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the SeaShell named color.
        /// </summary>
        public static Color SeaShell
        {
            get
            {
                return Color.FromInt32((int)KnownColor.SeaShell);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Sienna named color.
        /// </summary>
        public static Color Sienna
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Sienna);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Silver named color.
        /// </summary>
        public static Color Silver
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Silver);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the SkyBlue named color.
        /// </summary>
        public static Color SkyBlue
        {
            get
            {
                return Color.FromInt32((int)KnownColor.SkyBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the SlateBlue named color.
        /// </summary>
        public static Color SlateBlue
        {
            get
            {
                return Color.FromInt32((int)KnownColor.SlateBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the SlateGray named color.
        /// </summary>
        public static Color SlateGray
        {
            get
            {
                return Color.FromInt32((int)KnownColor.SlateGray);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Snow named color.
        /// </summary>
        public static Color Snow
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Snow);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the SpringGreen named color.
        /// </summary>
        public static Color SpringGreen
        {
            get
            {
                return Color.FromInt32((int)KnownColor.SpringGreen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the SteelBlue named color.
        /// </summary>
        public static Color SteelBlue
        {
            get
            {
                return Color.FromInt32((int)KnownColor.SteelBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Tan named color.
        /// </summary>
        public static Color Tan
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Tan);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Teal named color.
        /// </summary>
        public static Color Teal
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Teal);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Thistle named color.
        /// </summary>
        public static Color Thistle
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Thistle);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Tomato named color.
        /// </summary>
        public static Color Tomato
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Tomato);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Transparent named color.
        /// </summary>
        public static Color Transparent
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Transparent);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Turquoise named color.
        /// </summary>
        public static Color Turquoise
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Turquoise);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Violet named color.
        /// </summary>
        public static Color Violet
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Violet);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Wheat named color.
        /// </summary>
        public static Color Wheat
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Wheat);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the White named color.
        /// </summary>
        public static Color White
        {
            get
            {
                return Color.FromInt32((int)KnownColor.White);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the WhiteSmoke named color.
        /// </summary>
        public static Color WhiteSmoke
        {
            get
            {
                return Color.FromInt32((int)KnownColor.WhiteSmoke);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Yellow named color.
        /// </summary>
        public static Color Yellow
        {
            get
            {
                return Color.FromInt32((int)KnownColor.Yellow);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the YellowGreen named color.
        /// </summary>
        public static Color YellowGreen
        {
            get
            {
                return Color.FromInt32((int)KnownColor.YellowGreen);
            }
        }
    }
}