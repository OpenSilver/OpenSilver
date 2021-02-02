

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



extern alias wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetForHtml5.Compiler
{
    internal static class ConvertingStringToValue
    {
        internal static string ConvertStringToValue(string elementTypeInCSharp, string valueAsString)
        {
            string result = null;
            string trimmedValueAsString = valueAsString.Trim();
            string nonNullableElementTypeInCSharp = elementTypeInCSharp;
            bool goThroughSwitch = true;
            if (elementTypeInCSharp.StartsWith("global::System.Nullable<"))
            {
                nonNullableElementTypeInCSharp = elementTypeInCSharp.Substring(24, elementTypeInCSharp.Length - 25); //24 is the length of "global::System.Nullable<" then +1 to also remove the '>' at the end.
                if (trimmedValueAsString == "null")
                {
                    result = "null";
                    goThroughSwitch = false;
                }
            }
            if (goThroughSwitch)
            {
                switch (nonNullableElementTypeInCSharp)
                {
                    //Thickness first because it seems to be the most often used type.
                    case "global::Windows.UI.Xaml.Thickness":
                    case "global::System.Windows.Thickness":
                        result = PrepareStringForThicknessOrCornerRadius(nonNullableElementTypeInCSharp, valueAsString);
                        break;
                    case "global::Windows.UI.Xaml.Media.Brush":
                    case "global::System.Windows.Media.Brush":
                        result = PrepareStringForBrush(nonNullableElementTypeInCSharp, valueAsString);
                        break;
                    case "global::Windows.UI.Color":
                    case "global::System.Windows.Media.Color":
                        result = PrepareStringForColor(nonNullableElementTypeInCSharp, valueAsString);
                        break;
                    case "global::Windows.UI.Xaml.Media.Animation.KeyTime":
                    case "global::System.Windows.Media.Animation.KeyTime":
                        result = PrepareStringForKeyTime(nonNullableElementTypeInCSharp, valueAsString);
                        break;
                    case "global::Windows.UI.Xaml.CornerRadius":
                    case "global::System.Windows.CornerRadius":
                        result = PrepareStringForThicknessOrCornerRadius(nonNullableElementTypeInCSharp, valueAsString);
                        break;
                    case "global::System.Windows.Input.Cursor":
                        result = PrepareStringForCursor(nonNullableElementTypeInCSharp, valueAsString);
                        break;
                    case "global::Windows.UI.Xaml.Duration":
                    case "global::System.Windows.Duration":
                        result = PrepareStringForDuration(nonNullableElementTypeInCSharp, valueAsString);
                        break;
                    case "global::Windows.UI.Xaml.GridLength":
                    case "global::System.Windows.GridLength":
                        result = PrepareStringForGridLength(nonNullableElementTypeInCSharp, valueAsString);
                        break;
                    case "global::Windows.UI.Xaml.PropertyPath":
                    case "global::System.Windows.PropertyPath":
                        result = PrepareStringForPropertyPath(nonNullableElementTypeInCSharp, valueAsString);
                        break;
                    case "global::Windows.Foundation.Point":
                    case "global::System.Windows.Point":
                        result = PrepareStringForPoint(nonNullableElementTypeInCSharp, valueAsString);
                        break;
                    case "global::System.Boolean":
                        result = PrepareStringForBoolean(nonNullableElementTypeInCSharp, valueAsString);
                        break;
                    case "global::System.Byte":
                    case "global::System.Int16":
                    case "global::System.Int32":
                    case "global::System.SByte":
                    case "global::System.UInt16":
                    case "global::System.UInt32":
                    case "global::System.UInt64":
                        //Note: for numeric types, removing the quotation marks is sufficient (+ potential additional letter to tell the actual type because casts from int to double for example causes an exception).
                        result = trimmedValueAsString;
                        break;
                    case "global::System.Decimal":
                        result = PrepareStringForNumericType(trimmedValueAsString, 'M');
                        break;
                    case "global::System.Int64":
                        result = PrepareStringForNumericType(trimmedValueAsString, 'L');
                        break;
                    case "global::System.Single":
                        result = PrepareStringForNumericType(trimmedValueAsString, 'F');
                        break;
                    case "global::System.Double": //Special case: doubles can be "Auto" which is translated as double.NaN
                        result = PrepareStringForDouble(nonNullableElementTypeInCSharp, valueAsString);
                        break;
                    case "global::System.Char":
                        //replace the double quotes (") with single quotes ('):
                        result = "'" + trimmedValueAsString + "'";
                        break;
                    case "global::System.String":
                    case "global::System.Object":
                        result = PrepareStringForString(nonNullableElementTypeInCSharp, valueAsString);
                        break;
                    default:
                        //return after escaping (note: we use valueAsString and not trimmedValueAsString because it can be a string that starts or ends with spaces):
                        result = string.Format("({0})global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof({0}), {1})",
                        nonNullableElementTypeInCSharp,
                        GetQuotedVerbatimString(valueAsString)); // Note: we use verbatim string (ie. a string that starts with "@") so that the only character we have to escape is the quote (we need to double it).
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// Adds @" at the beginning of the string and a " at the end, and escapes the quotation marks within by doubling them (turns the content of the string into a Verbatim string)
        /// Transforming strings from: "stringContent with a \" in it." into: "@\"stringContent with a \"\" in it.\""
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static string GetQuotedVerbatimString(string str)
        {
            return "@\"" + str.Replace("\"", "\"\"") + "\"";
        }

        static string PrepareStringForThicknessOrCornerRadius(string elementTypeInCSharp, string valueAsString)
        {
            //Note: the way to generate the code for Thickness and CornerRadius is exactly the same but the meanings are different so be careful when modifying this method. It was initially made with Thickness in mind.

            string result;
            //Now it looks like "left,top, right bottom" (we may or may not have additional spaces or commas.
            string[] values = valueAsString.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (values.Length == 1)
            {
                result = string.Format("new {0}({1})", elementTypeInCSharp, values[0]);
            }
            else
            {
                string left = values[0]; //topLeft in the case of the CornerRadius
                string right = values[0]; //bottomRight in the case of the CornerRadius
                string top = values[1]; //topRight in the case of the CornerRadius
                string bottom = values[1]; //bottomLeft in the case of the CornerRadius

                if (values.Length == 4) //Note: in the case of CornerRadius, this should always be true if we arrive here.
                {
                    right = values[2];
                    bottom = values[3];
                }
                result = string.Format("new {0}({1}, {2}, {3}, {4})", elementTypeInCSharp, left, top, right, bottom);
            }
            return result;
        }

        static string PrepareStringForGridLength(string elementTypeInCSharp, string valueAsString)
        {
            string lowercaseFullValueAsString = valueAsString.ToLower();
            string resultValueAsString;
            string resultTypeAsString;
            string globalTypeNameSpace = elementTypeInCSharp.Substring(0, elementTypeInCSharp.LastIndexOf('.')); // todo-perf: If we decide to pass isSLMigration as parameter of ConvertStringToValue, use it here with hard-coded strings rather than getting it from the string for the type.
            if (lowercaseFullValueAsString.EndsWith("*"))
            {
                string lowercaseValue = lowercaseFullValueAsString.Substring(0, lowercaseFullValueAsString.Length - 1);
                double value;
                if (lowercaseValue == "")
                {
                    resultValueAsString = "1.0";
                }
                else if (double.TryParse(lowercaseValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out value))
                {
                    resultValueAsString = lowercaseValue;
                }
                else
                    throw new wpf::System.Windows.Markup.XamlParseException("Could not generate GridLength instantiation from string: " + valueAsString);
                resultTypeAsString = globalTypeNameSpace + ".GridUnitType.Star";
            }
            else if (lowercaseFullValueAsString == "auto")
            {
                resultValueAsString = "1.0";
                resultTypeAsString = globalTypeNameSpace + ".GridUnitType.Auto";
            }
            else
            {
                double value;
                if (double.TryParse(lowercaseFullValueAsString, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out value))
                {
                    resultValueAsString = lowercaseFullValueAsString;
                    resultTypeAsString = globalTypeNameSpace + ".GridUnitType.Pixel";
                }
                else
                    throw new wpf::System.Windows.Markup.XamlParseException("Could not generate GridLength instantiation from string: " + valueAsString);
            }
            return String.Format("new {0}({1}, {2})", elementTypeInCSharp, resultValueAsString, resultTypeAsString);
        }

        static string PrepareStringForPropertyPath(string elementTypeInCSharp, string valueAsString)
        {
            return string.Format("new {0}({1})", elementTypeInCSharp, GetQuotedVerbatimString(valueAsString));
        }

        static string PrepareStringForPoint(string elementTypeInCSharp, string valueAsString)
        {
            string[] splittedString = valueAsString.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (splittedString.Length == 2)
            {
                double x = 0d;
                double y = 0d;

                bool isParseOK = double.TryParse(splittedString[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out x);
                isParseOK = isParseOK && double.TryParse(splittedString[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out y);

                if (isParseOK)
                    return string.Format("new {0}({1}, {2})", elementTypeInCSharp, x.ToString(System.Globalization.CultureInfo.InvariantCulture), y.ToString(System.Globalization.CultureInfo.InvariantCulture));
            }
            throw new wpf::System.Windows.Markup.XamlParseException("Could not generate Point instantiation from string: \"" + valueAsString + "\".");
        }

        static string PrepareStringForBoolean(string elementTypeInCSharp, string valueAsString)
        {
            return valueAsString.ToLower();
        }

        static string PrepareStringForDouble(string elementTypeInCSharp, string valueAsString)
        {
            string trimmedString = valueAsString.Trim();

            if (trimmedString.ToLower() == "auto" || trimmedString.ToLower() == "nan")
            {
                return "global::System.Double.NaN";
            }
            else if (trimmedString.ToLower() == "infinity")
            {
                return "global::System.Double.PositiveInfinity";
            }
            else if (trimmedString.ToLower() == "-infinity")
            {
                return "global::System.Double.NegativeInfinity";
            }
            else
            {
                if (trimmedString == ".")
                {
                    return "0D";
                }
                else if (trimmedString.EndsWith("."))
                {
                    return PrepareStringForNumericType(trimmedString.Substring(0, trimmedString.Length - 1), 'D');
                }
                else
                {
                    return PrepareStringForNumericType(trimmedString, 'D');
                }
            }
        }

        private static string PrepareStringForNumericType(string value, char code)
        {
            if (value[value.Length - 1] != code)
            {
                return value + code;
            }
            return value;
        }

        internal static string PrepareStringForString(string elementTypeInCSharp, string valueAsString)
        {
            string value = valueAsString.StartsWith("{}") ? valueAsString.Substring(2) : valueAsString; // "{}" is used to escape '{' (when used at the beginning of the string)
            return GetQuotedVerbatimString(value); // Note: we use verbatim string (ie. a string that starts with "@") so that the only character we have to escape is the quote (we need to double it).
        }

        static string PrepareStringForColor(string elementTypeInCSharp, string valueAsString)
        {
            string trimmedString = valueAsString.Trim();
            if (!string.IsNullOrEmpty(trimmedString) && (trimmedString[0] == '#'))
            {
                string tokens = trimmedString.Substring(1);
                if (tokens.Length == 6) // This is becaue XAML is tolerant when the user has forgot the alpha channel (eg. #DDDDDD for Gray).
                    tokens = "FF" + tokens;

                int color;
                if (int.TryParse(tokens, NumberStyles.HexNumber, NumberFormatInfo.CurrentInfo, out color))
                {
                    return string.Format("new {0}() {{ A = (byte){1}, R = (byte){2}, G = (byte){3}, B = (byte){4} }}",
                        elementTypeInCSharp,
                        (color >> 0x18) & 0xff,
                        (color >> 0x10) & 0xff,
                        (color >> 8) & 0xff,
                        color & 0xff);
                }
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
                    return string.Format("{0}.FromScRgb((float){1}, (float){2}, (float){3}, (float){4})",
                        elementTypeInCSharp,
                        values[0], values[1], values[2], values[3]);
                }
                else
                {
                    return string.Format("{0}.FromScRgb((float){1}, (float){2}, (float){3}, (float){4})",
                        elementTypeInCSharp,
                        1.0f, values[0], values[1], values[2]);
                }
            }
            else
            {
                ColorsEnum namedColor;
                if (Enum.TryParse(trimmedString, true, out namedColor))
                {
                    int color = (int)namedColor;
                    return string.Format("new {0}() {{ A = (byte){1}, R = (byte){2}, G = (byte){3}, B = (byte){4} }}",
                        elementTypeInCSharp,
                        (color >> 0x18) & 0xff,
                        (color >> 0x10) & 0xff,
                        (color >> 8) & 0xff,
                        color & 0xff);
                }
            }
            throw new wpf::System.Windows.Markup.XamlParseException(string.Format("Invalid color: {0}", valueAsString));
        }

        static string PrepareStringForBrush(string elementTypeInCSharp, string valueAsString)
        {
            string brushNamespace = elementTypeInCSharp.Substring(0, elementTypeInCSharp.LastIndexOf('.'));
            string colorNamespace = brushNamespace == "global::System.Windows.Media" ? brushNamespace : "global::Windows.UI"; //the namespace for the Color class is the same as for Brush in SL but not in WPF
            return string.Format("new {0}.SolidColorBrush({1})", brushNamespace, PrepareStringForColor(colorNamespace + ".Color", valueAsString));
        }

        static string PrepareStringForKeyTime(string elementTypeInCSharp, string valueAsString)
        {
            try
            {
                if (valueAsString == "Uniform")
                {
                    throw new wpf::System.Windows.Markup.XamlParseException("The Value \"Uniform\" for keyTime is not supported yet.");
                }
                else if (valueAsString == "Paced")
                {
                    throw new wpf::System.Windows.Markup.XamlParseException("The Value \"Paced\" for keyTime is not supported yet.");
                }
                else if (valueAsString.EndsWith("%"))
                {
                    throw new wpf::System.Windows.Markup.XamlParseException("The percentage values for keyTime are not supported yet.");
                }
                else
                {
                    TimeSpan timeSpan = TimeSpan.Parse(valueAsString);
                    return string.Format("{0}.FromTimeSpan(new global::System.TimeSpan({1}L))", elementTypeInCSharp, timeSpan.Ticks);
                }
            }
            catch (Exception ex)
            {
                throw new wpf::System.Windows.Markup.XamlParseException("Invalid KeyTime: " + valueAsString, ex);
            }
        }

        static string PrepareStringForCursor(string elementTypeInCSharp, string valueAsString)
        {
            // For cursors, it's easy: use the Cursors class that gives an easy access to the possible values, and use the string as is to serve as the accessor
            // Note: If we want to accept it as a non-case-sensitive thing (accepting hand instead of Hand), we will need to add some code to recognise the thing (I guess using System.Windows.Forms.Cursors).
            // todo: Once the different types of ways to define a cursor are implemented, this will need some changes accordingly.
            return string.Format("{0}s.{1}", elementTypeInCSharp, valueAsString);
        }

        static string PrepareStringForDuration(string elementTypeInCSharp, string valueAsString)
        {
            if (valueAsString.ToLower() == "forever")
                return elementTypeInCSharp + ".Forever";
            if (valueAsString.ToLower() == "automatic")
                return elementTypeInCSharp + ".Automatic";
            TimeSpan timeSpan = TimeSpan.Parse(valueAsString);
            return string.Format("new {0}(new global::System.TimeSpan({1}L))", elementTypeInCSharp, timeSpan.Ticks); ;
        }


        //Types added and quickly tested:
        //  - Thickness
        //  - CornerRadius
        //  - GridLength
        //  - KeyTime
        //  - Brush
        //  - Cursor

        //Types added kinda tested:
        //  - Point
        //  - Color
        //  - PropertyPath

        //Types added not tested:

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
}
