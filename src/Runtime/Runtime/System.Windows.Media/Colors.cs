
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
        internal enum INTERNAL_ColorsEnum : int
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
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.AliceBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the AntiqueWhite named color.
        /// </summary>
        public static Color AntiqueWhite
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.AntiqueWhite);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Aqua named color.
        /// </summary>
        public static Color Aqua
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Aqua);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Aquamarine named color.
        /// </summary>
        public static Color Aquamarine
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Aquamarine);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Azure named color.
        /// </summary>
        public static Color Azure
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Azure);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Beige named color.
        /// </summary>
        public static Color Beige
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Beige);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Bisque named color.
        /// </summary>
        public static Color Bisque
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Bisque);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Black named color.
        /// </summary>
        public static Color Black
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Black);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the BlanchedAlmond named color.
        /// </summary>
        public static Color BlanchedAlmond
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.BlanchedAlmond);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Blue named color.
        /// </summary>
        public static Color Blue
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Blue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the BlueViolet named color.
        /// </summary>
        public static Color BlueViolet
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.BlueViolet);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Brown named color.
        /// </summary>
        public static Color Brown
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Brown);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the BurlyWood named color.
        /// </summary>
        public static Color BurlyWood
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.BurlyWood);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the CadetBlue named color.
        /// </summary>
        public static Color CadetBlue
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.CadetBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Chartreuse named color.
        /// </summary>
        public static Color Chartreuse
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Chartreuse);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Chocolate named color.
        /// </summary>
        public static Color Chocolate
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Chocolate);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Coral named color.
        /// </summary>
        public static Color Coral
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Coral);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the CornflowerBlue named color.
        /// </summary>
        public static Color CornflowerBlue
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.CornflowerBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Cornsilk named color.
        /// </summary>
        public static Color Cornsilk
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Cornsilk);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Crimson named color.
        /// </summary>
        public static Color Crimson
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Crimson);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Cyan named color.
        /// </summary>
        public static Color Cyan
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Cyan);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkBlue named color.
        /// </summary>
        public static Color DarkBlue
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.DarkBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkCyan named color.
        /// </summary>
        public static Color DarkCyan
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.DarkCyan);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkGoldenrod named color.
        /// </summary>
        public static Color DarkGoldenrod
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.DarkGoldenrod);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkGray named color.
        /// </summary>
        public static Color DarkGray
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.DarkGray);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkGreen named color.
        /// </summary>
        public static Color DarkGreen
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.DarkGreen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkKhaki named color.
        /// </summary>
        public static Color DarkKhaki
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.DarkKhaki);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkMagenta named color.
        /// </summary>
        public static Color DarkMagenta
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.DarkMagenta);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkOliveGreen named color.
        /// </summary>
        public static Color DarkOliveGreen
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.DarkOliveGreen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkOrange named color.
        /// </summary>
        public static Color DarkOrange
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.DarkOrange);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkOrchid named color.
        /// </summary>
        public static Color DarkOrchid
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.DarkOrchid);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkRed named color.
        /// </summary>
        public static Color DarkRed
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.DarkRed);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkSalmon named color.
        /// </summary>
        public static Color DarkSalmon
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.DarkSalmon);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkSeaGreen named color.
        /// </summary>
        public static Color DarkSeaGreen
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.DarkSeaGreen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkSlateBlue named color.
        /// </summary>
        public static Color DarkSlateBlue
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.DarkSlateBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkSlateGray named color.
        /// </summary>
        public static Color DarkSlateGray
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.DarkSlateGray);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkTurquoise named color.
        /// </summary>
        public static Color DarkTurquoise
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.DarkTurquoise);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DarkViolet named color.
        /// </summary>
        public static Color DarkViolet
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.DarkViolet);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DeepPink named color.
        /// </summary>
        public static Color DeepPink
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.DeepPink);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DeepSkyBlue named color.
        /// </summary>
        public static Color DeepSkyBlue
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.DeepSkyBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DimGray named color.
        /// </summary>
        public static Color DimGray
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.DimGray);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the DodgerBlue named color.
        /// </summary>
        public static Color DodgerBlue
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.DodgerBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Firebrick named color.
        /// </summary>
        public static Color Firebrick
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Firebrick);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the FloralWhite named color.
        /// </summary>
        public static Color FloralWhite
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.FloralWhite);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the ForestGreen named color.
        /// </summary>
        public static Color ForestGreen
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.ForestGreen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Fuchsia named color.
        /// </summary>
        public static Color Fuchsia
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Fuchsia);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Gainsboro named color.
        /// </summary>
        public static Color Gainsboro
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Gainsboro);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the GhostWhite named color.
        /// </summary>
        public static Color GhostWhite
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.GhostWhite);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Gold named color.
        /// </summary>
        public static Color Gold
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Gold);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Goldenrod named color.
        /// </summary>
        public static Color Goldenrod
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Goldenrod);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Gray named color.
        /// </summary>
        public static Color Gray
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Gray);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Green named color.
        /// </summary>
        public static Color Green
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Green);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the GreenYellow named color.
        /// </summary>
        public static Color GreenYellow
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.GreenYellow);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Honeydew named color.
        /// </summary>
        public static Color Honeydew
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Honeydew);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the HotPink named color.
        /// </summary>
        public static Color HotPink
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.HotPink);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the IndianRed named color.
        /// </summary>
        public static Color IndianRed
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.IndianRed);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Indigo named color.
        /// </summary>
        public static Color Indigo
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Indigo);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Ivory named color.
        /// </summary>
        public static Color Ivory
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Ivory);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Khaki named color.
        /// </summary>
        public static Color Khaki
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Khaki);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Lavender named color.
        /// </summary>
        public static Color Lavender
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Lavender);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LavenderBlush named color.
        /// </summary>
        public static Color LavenderBlush
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.LavenderBlush);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LawnGreen named color.
        /// </summary>
        public static Color LawnGreen
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.LawnGreen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LemonChiffon named color.
        /// </summary>
        public static Color LemonChiffon
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.LemonChiffon);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LightBlue named color.
        /// </summary>
        public static Color LightBlue
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.LightBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LightCoral named color.
        /// </summary>
        public static Color LightCoral
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.LightCoral);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LightCyan named color.
        /// </summary>
        public static Color LightCyan
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.LightCyan);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LightGoldenrodYellow named color.
        /// </summary>
        public static Color LightGoldenrodYellow
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.LightGoldenrodYellow);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LightGray named color.
        /// </summary>
        public static Color LightGray
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.LightGray);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LightGreen named color.
        /// </summary>
        public static Color LightGreen
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.LightGreen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LightPink named color.
        /// </summary>
        public static Color LightPink
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.LightPink);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LightSalmon named color.
        /// </summary>
        public static Color LightSalmon
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.LightSalmon);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LightSeaGreen named color.
        /// </summary>
        public static Color LightSeaGreen
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.LightSeaGreen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LightSkyBlue named color.
        /// </summary>
        public static Color LightSkyBlue
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.LightSkyBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LightSlateGray named color.
        /// </summary>
        public static Color LightSlateGray
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.LightSlateGray);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LightSteelBlue named color.
        /// </summary>
        public static Color LightSteelBlue
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.LightSteelBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LightYellow named color.
        /// </summary>
        public static Color LightYellow
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.LightYellow);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Lime named color.
        /// </summary>
        public static Color Lime
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Lime);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the LimeGreen named color.
        /// </summary>
        public static Color LimeGreen
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.LimeGreen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Linen named color.
        /// </summary>
        public static Color Linen
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Linen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Magenta named color.
        /// </summary>
        public static Color Magenta
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Magenta);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Maroon named color.
        /// </summary>
        public static Color Maroon
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Maroon);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the MediumAquamarine named color.
        /// </summary>
        public static Color MediumAquamarine
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.MediumAquamarine);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the MediumBlue named color.
        /// </summary>
        public static Color MediumBlue
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.MediumBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the MediumOrchid named color.
        /// </summary>
        public static Color MediumOrchid
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.MediumOrchid);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the MediumPurple named color.
        /// </summary>
        public static Color MediumPurple
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.MediumPurple);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the MediumSeaGreen named color.
        /// </summary>
        public static Color MediumSeaGreen
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.MediumSeaGreen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the MediumSlateBlue named color.
        /// </summary>
        public static Color MediumSlateBlue
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.MediumSlateBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the MediumSpringGreen named color.
        /// </summary>
        public static Color MediumSpringGreen
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.MediumSpringGreen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the MediumTurquoise named color.
        /// </summary>
        public static Color MediumTurquoise
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.MediumTurquoise);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the MediumVioletRed named color.
        /// </summary>
        public static Color MediumVioletRed
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.MediumVioletRed);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the MidnightBlue named color.
        /// </summary>
        public static Color MidnightBlue
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.MidnightBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the MintCream named color.
        /// </summary>
        public static Color MintCream
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.MintCream);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the MistyRose named color.
        /// </summary>
        public static Color MistyRose
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.MistyRose);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Moccasin named color.
        /// </summary>
        public static Color Moccasin
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Moccasin);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the NavajoWhite named color.
        /// </summary>
        public static Color NavajoWhite
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.NavajoWhite);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Navy named color.
        /// </summary>
        public static Color Navy
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Navy);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the OldLace named color.
        /// </summary>
        public static Color OldLace
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.OldLace);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Olive named color.
        /// </summary>
        public static Color Olive
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Olive);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the OliveDrab named color.
        /// </summary>
        public static Color OliveDrab
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.OliveDrab);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Orange named color.
        /// </summary>
        public static Color Orange
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Orange);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the OrangeRed named color.
        /// </summary>
        public static Color OrangeRed
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.OrangeRed);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Orchid named color.
        /// </summary>
        public static Color Orchid
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Orchid);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the PaleGoldenrod named color.
        /// </summary>
        public static Color PaleGoldenrod
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.PaleGoldenrod);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the PaleGreen named color.
        /// </summary>
        public static Color PaleGreen
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.PaleGreen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the PaleTurquoise named color.
        /// </summary>
        public static Color PaleTurquoise
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.PaleTurquoise);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the PaleVioletRed named color.
        /// </summary>
        public static Color PaleVioletRed
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.PaleVioletRed);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the PapayaWhip named color.
        /// </summary>
        public static Color PapayaWhip
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.PapayaWhip);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the PeachPuff named color.
        /// </summary>
        public static Color PeachPuff
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.PeachPuff);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Peru named color.
        /// </summary>
        public static Color Peru
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Peru);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Pink named color.
        /// </summary>
        public static Color Pink
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Pink);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Plum named color.
        /// </summary>
        public static Color Plum
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Plum);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the PowderBlue named color.
        /// </summary>
        public static Color PowderBlue
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.PowderBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Purple named color.
        /// </summary>
        public static Color Purple
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Purple);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Red named color.
        /// </summary>
        public static Color Red
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Red);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the RosyBrown named color.
        /// </summary>
        public static Color RosyBrown
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.RosyBrown);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the RoyalBlue named color.
        /// </summary>
        public static Color RoyalBlue
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.RoyalBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the SaddleBrown named color.
        /// </summary>
        public static Color SaddleBrown
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.SaddleBrown);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Salmon named color.
        /// </summary>
        public static Color Salmon
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Salmon);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the SandyBrown named color.
        /// </summary>
        public static Color SandyBrown
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.SandyBrown);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the SeaGreen named color.
        /// </summary>
        public static Color SeaGreen
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.SeaGreen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the SeaShell named color.
        /// </summary>
        public static Color SeaShell
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.SeaShell);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Sienna named color.
        /// </summary>
        public static Color Sienna
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Sienna);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Silver named color.
        /// </summary>
        public static Color Silver
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Silver);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the SkyBlue named color.
        /// </summary>
        public static Color SkyBlue
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.SkyBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the SlateBlue named color.
        /// </summary>
        public static Color SlateBlue
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.SlateBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the SlateGray named color.
        /// </summary>
        public static Color SlateGray
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.SlateGray);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Snow named color.
        /// </summary>
        public static Color Snow
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Snow);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the SpringGreen named color.
        /// </summary>
        public static Color SpringGreen
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.SpringGreen);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the SteelBlue named color.
        /// </summary>
        public static Color SteelBlue
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.SteelBlue);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Tan named color.
        /// </summary>
        public static Color Tan
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Tan);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Teal named color.
        /// </summary>
        public static Color Teal
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Teal);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Thistle named color.
        /// </summary>
        public static Color Thistle
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Thistle);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Tomato named color.
        /// </summary>
        public static Color Tomato
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Tomato);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Transparent named color.
        /// </summary>
        public static Color Transparent
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Transparent);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Turquoise named color.
        /// </summary>
        public static Color Turquoise
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Turquoise);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Violet named color.
        /// </summary>
        public static Color Violet
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Violet);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Wheat named color.
        /// </summary>
        public static Color Wheat
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Wheat);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the White named color.
        /// </summary>
        public static Color White
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.White);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the WhiteSmoke named color.
        /// </summary>
        public static Color WhiteSmoke
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.WhiteSmoke);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the Yellow named color.
        /// </summary>
        public static Color Yellow
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.Yellow);
            }
        }
 
        /// <summary>
        /// Gets the color value that represents the YellowGreen named color.
        /// </summary>
        public static Color YellowGreen
        {
            get
            {
                return Color.INTERNAL_ConvertFromInt32((int)INTERNAL_ColorsEnum.YellowGreen);
            }
        }
    }
}