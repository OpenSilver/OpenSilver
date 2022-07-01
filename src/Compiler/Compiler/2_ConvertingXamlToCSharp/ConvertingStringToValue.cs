
/*===================================================================================
* 
*   Copyright (c) Userware (OpenSilver.net, CSHTML5.com)
*      
*   This file is part of both the OpenSilver Compiler (https://opensilver.net), which
*   is licensed under the MIT license (https://opensource.org/licenses/MIT), and the
*   CSHTML5 Compiler (http://cshtml5.com), which is dual-licensed (MIT + commercial).
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System;

namespace DotNetForHtml5.Compiler
{
    internal static class ConvertingStringToValue
    {
        public static string ConvertFromInvariantString(string type, string source)
        {
            string value = source.Trim();

            if (IsNullableType(type, out string underlyingType))
            {
                if (value == "null")
                {
                    return "null";
                }
            }
            
            string result;
            
            switch (underlyingType)
            {
                case "global::System.SByte":
                case "global::System.UInt16":
                case "global::System.UInt32":
                case "global::System.UInt64":
                    // Note: for numeric types, removing the quotation marks is sufficient
                    // (+ potential additional letter to tell the actual type because casts
                    // from int to double for example causes an exception).
                    result = value;
                    break;

                case "global::System.Decimal":
                    result = PrepareStringForDecimal(value);
                    break;

                case "global::System.Char":
                    result = PrepareStringForChar(value);
                    break;

                case "global::System.Object":
                    result = PrepareStringForString(source);
                    break;

                default:
                    // return after escaping (note: we use value and not stringValue
                    // because it can be a string that starts or ends with spaces)
                    result = CoreTypesHelper.ConvertFromInvariantStringHelper(
                        source,
                        underlyingType
                    );
                    break;
            }

            return result;
        }

        private static string PrepareStringForChar(string source)
        {
            if (source != null && source.Length == 1)
            {
                return $"'{source}'";
            }

            return "'\\0'";
        }

        private static string PrepareStringForDecimal(string source)
        {
            string value = source.ToLower();

            if (value.EndsWith("m"))
            {
                value = value.Substring(0, value.Length - 1);
            }

            if (value.EndsWith("."))
            {
                value = value.Substring(0, value.Length - 1);
            }

            if (value.Length == 0)
            {
                value = "0";
            }

            return $"{value}M";
        }

        internal static string PrepareStringForString(string source)
        {
            // "{}" is used to escape '{' (when used at the beginning of the string)
            string value = source.StartsWith("{}") ? source.Substring(2) : source;

            // Note: we use verbatim string (ie. a string that starts with "@") so
            // that the only character we have to escape is the quote (we need to
            // double it).
            return GetQuotedVerbatimString(value);
        }

        private static bool IsNullableType(string type, out string underlyingType)
        {
            if (type.StartsWith("global::System.Nullable<"))
            {
                // skips "global::System.Nullable<" and then remove 
                // the trailing '>' at the end
                underlyingType = type.Substring(24, type.Length - 25);
                return true;
            }

            underlyingType = type;
            return false;
        }

        /// <summary>
        /// Adds @" at the beginning of the string and a " at the end, and escapes the 
        /// quotation marks within by doubling them (turns the content of the string 
        /// into a Verbatim string)
        /// Transforming strings from: "stringContent with a \" in it." into: 
        /// "@\"stringContent with a \"\" in it.\""
        /// </summary>
        private static string GetQuotedVerbatimString(string s)
        {
            return "@\"" + s.Replace("\"", "\"\"") + "\"";
        }

        //Types to add to the switch (probably):
        //
        //  **Types that seem to appear often :
        //  - Geometry                              <------------ Geometry feels like it could make for quite a big improvement on performance on each call if we could make the parsing at compilation time + it is not rarely used so it would be pretty nice to have it.
        //
        //  **Types that do not seem to appear as often :
        //  - Guid ?
        //  - DateTime
        //  - TimeSpan
        //  - Alignments seem to already be dealt with (maybe it's enums in general).
    }

    internal enum ColorsEnum : int
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
        YellowGreen = unchecked((int)0xFF9ACD32)
    }

    //
    // IMPORTANT: if you add or remove entries in this Enum, you must update
    // accordingly the file "FontWeights.cs" in the Runtime project.
    //
    internal enum FontweightsEnum : ushort
    {
        Black = 900,
        Bold = 700,
        DemiBold = 600,
        ExtraBlack = 900, //note: the value should be 950 but it is not supported in html5
        ExtraBold = 800,
        ExtraLight = 200,
        Heavy = 900,
        Light = 300,
        Medium = 500,
        Normal = 400,
        Regular = 400,
        SemiBold = 600,
        SemiLight = 300, //note: the value should be 350 (I think) but it is not supported in html5
        Thin = 100,
        UltraBlack = 900, //note: the value should be 950 but it is not supported in html5
        UltraBold = 800,
        UltraLight = 200
    }
}
