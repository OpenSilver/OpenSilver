
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace OpenSilver.Compiler
{
    internal sealed class SLCoreTypesConverterCS : CoreTypesConverterBase
    {
        protected override Dictionary<string, Func<string, string>> SupportedCoreTypes { get; }
            = GetSupportedCoreTypes();

        //
        // IMPORTANT: Do not modify this dictionary unless you made changes in the file
        // 'TypeConverterHelper.cs' in the Runtime project. This dictionary and the 
        // dictionary 'TypeConverterHelper.CoreTypeConverters' must stay in sync.
        //
        // ImageSource is the only type to be present in this dictionary and not in the
        // one from the Runtime. This is due to the fact that we need to support the 
        // following XAML syntax : <ImageSource>whatever/you/want</ImageSource> which
        // normally requires the ContentPropertyAttribute to be defined on the type we
        // are trying to instantiate but this is not the case for ImageSource.
        // However, ImageSource has a registered TypeConverter via TypeConverterAttribute,
        // so this is why we do not want to register it in the CoreTypeConverters
        // dictionary, as it would prevent derived types (BitmapSource and BitmapImage)
        // from finding the converter.
        //
        private static Dictionary<string, Func<string, string>> GetSupportedCoreTypes()
        {
            return new Dictionary<string, Func<string, string>>(29, StringComparer.OrdinalIgnoreCase)
            {
                ["system.windows.input.cursor"] = CoreTypesHelper.ConvertToCursor,
                ["system.windows.media.animation.keytime"] = CoreTypesHelper.ConvertToKeyTime,
                ["system.windows.media.animation.repeatbehavior"] = CoreTypesHelper.ConvertToRepeatBehavior,
                ["system.windows.media.animation.keyspline"] = CoreTypesHelper.ConvertToKeySpline,
                ["system.windows.media.brush"] = CoreTypesHelper.ConvertToBrush,
                ["system.windows.media.solidcolorbrush"] = CoreTypesHelper.ConvertToBrush,
                ["system.windows.media.color"] = CoreTypesHelper.ConvertToColor,
                ["system.windows.media.doublecollection"] = CoreTypesHelper.ConvertToDoubleCollection,
                ["system.windows.media.fontfamily"] = CoreTypesHelper.ConvertToFontFamily,
                ["system.windows.media.geometry"] = CoreTypesHelper.ConvertToGeometry,
                ["system.windows.media.pathgeometry"] = CoreTypesHelper.ConvertToPathGeometry,
                ["system.windows.media.matrix"] = CoreTypesHelper.ConvertToMatrix,
                ["system.windows.media.pointcollection"] = CoreTypesHelper.ConvertToPointCollection,
                ["system.windows.media.transform"] = CoreTypesHelper.ConvertToTransform,
                ["system.windows.media.matrixtransform"] = CoreTypesHelper.ConvertToTransform,
                ["system.windows.media.cachemode"] = CoreTypesHelper.ConvertToCacheMode,
                ["system.windows.cornerradius"] = CoreTypesHelper.ConvertToCornerRadius,
                ["system.windows.duration"] = CoreTypesHelper.ConvertToDuration,
                ["system.windows.fontweight"] = CoreTypesHelper.ConvertToFontWeight,
                ["system.windows.gridlength"] = CoreTypesHelper.ConvertToGridLength,
                ["system.windows.point"] = CoreTypesHelper.ConvertToPoint,
                ["system.windows.propertypath"] = CoreTypesHelper.ConvertToPropertyPath,
                ["system.windows.rect"] = CoreTypesHelper.ConvertToRect,
                ["system.windows.size"] = CoreTypesHelper.ConvertToSize,
                ["system.windows.thickness"] = CoreTypesHelper.ConvertToThickness,
                ["system.windows.fontstretch"] = CoreTypesHelper.ConvertToFontStretch,
                ["system.windows.fontstyle"] = CoreTypesHelper.ConvertToFontStyle,
                ["system.windows.textdecorationcollection"] = CoreTypesHelper.ConvertToTextDecorationCollection,
                ["system.windows.media.imagesource"] = CoreTypesHelper.ConvertToImageSource,
                ["system.windows.vector"] = CoreTypesHelper.ConvertToVector,
            };
        }
    }

    internal static class CoreTypesHelper
    {
        public const string RuntimeHelperClass = "global::OpenSilver.Internal.Xaml.RuntimeHelpers";
        private static readonly char[] _separators = [',', ' '];
        private static readonly char[] _repeatBehaviorConverterIterationCharacter = ['x', 'X'];

        public static string ConvertFromInvariantStringHelper(string source, string destinationType)
        {
            return $"{RuntimeHelperClass}.ConvertFromInvariantString<{destinationType}>({Escape(source)})";
        }

        internal static string ConvertToCursor(string source)
        {
            return $"global::System.Windows.Input.Cursors.{source}";
        }

        internal static string ConvertToKeyTime(string source)
        {
            string stringValue = source.Trim();

            if (stringValue == "Paced")
            {
                throw new XamlParseException("The 'System.Windows.Media.Animation.KeyTime.Paced' property is not supported yet.");
            }
            else if (stringValue.Length > 0 &&
                     stringValue[stringValue.Length - 1] == '%')
            {
                throw new XamlParseException("Percentage values for 'System.Windows.Media.Animation.KeyTime' are not supported yet.");
            }
            else if (stringValue == "Uniform")
            {
                return "global::System.Windows.Media.Animation.KeyTime.Uniform";
            }
            else
            {
                string timeSpanValue = SystemTypesHelper.CSharp.ConvertFromInvariantString(stringValue, "system.timespan");
                return $"global::System.Windows.Media.Animation.KeyTime.FromTimeSpan({timeSpanValue})";
            }
        }

        internal static string ConvertToRepeatBehavior(string source)
        {
            string stringValue = source.Trim();

            if (string.Equals(stringValue, "Forever", StringComparison.OrdinalIgnoreCase))
            {
                return "global::System.Windows.Media.Animation.RepeatBehavior.Forever";
            }
            else if (stringValue.Length > 0 &&
                     char.ToLowerInvariant(stringValue[stringValue.Length - 1]) == _repeatBehaviorConverterIterationCharacter[0])
            {
                string stringDoubleValue = stringValue.TrimEnd(_repeatBehaviorConverterIterationCharacter);

                return $"new global::System.Windows.Media.Animation.RepeatBehavior({SystemTypesHelper.CSharp.ConvertFromInvariantString(stringDoubleValue, "system.double")})";
            }

            string timeSpanValue = SystemTypesHelper.CSharp.ConvertFromInvariantString(stringValue, "system.timespan");

            return $"new global::System.Windows.Media.Animation.RepeatBehavior({timeSpanValue})";
        }

        internal static string ConvertToKeySpline(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return "new global::System.Windows.Media.Animation.KeySpline()";
            }

            string[] split = source.Split(_separators, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length == 4)
            {
                return $"new global::System.Windows.Media.Animation.KeySpline({split[0]}, {split[1]}, {split[2]}, {split[3]})";
            }

            throw GetConvertException(source, "System.Windows.Media.Animation.KeySpline");
        }

        internal static string ConvertToBrush(string source)
        {
            return $"new global::System.Windows.Media.SolidColorBrush({ConvertToColor(source)})";
        }

        internal static string ConvertToColor(string source)
        {
            const int s_zeroChar = (int)'0';
            const int s_aLower = (int)'a';
            const int s_aUpper = (int)'A';

            static string MatchColor(string colorString, out bool isKnownColor, out bool isNumericColor, out bool isScRgbColor)
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

            static int ParseHexChar(char c)
            {
                int intChar = (int)c;

                if ((intChar >= s_zeroChar) && (intChar <= (s_zeroChar + 9)))
                {
                    return (intChar - s_zeroChar);
                }

                if ((intChar >= s_aLower) && (intChar <= (s_aLower + 5)))
                {
                    return (intChar - s_aLower + 10);
                }

                if ((intChar >= s_aUpper) && (intChar <= (s_aUpper + 5)))
                {
                    return (intChar - s_aUpper + 10);
                }
                throw new FormatException("Token is not valid.");
            }

            static string ParseHexColor(string trimmedColor)
            {
                int a, r, g, b;
                a = 255;

                if (trimmedColor.Length > 7)
                {
                    a = ParseHexChar(trimmedColor[1]) * 16 + ParseHexChar(trimmedColor[2]);
                    r = ParseHexChar(trimmedColor[3]) * 16 + ParseHexChar(trimmedColor[4]);
                    g = ParseHexChar(trimmedColor[5]) * 16 + ParseHexChar(trimmedColor[6]);
                    b = ParseHexChar(trimmedColor[7]) * 16 + ParseHexChar(trimmedColor[8]);
                }
                else if (trimmedColor.Length > 5)
                {
                    r = ParseHexChar(trimmedColor[1]) * 16 + ParseHexChar(trimmedColor[2]);
                    g = ParseHexChar(trimmedColor[3]) * 16 + ParseHexChar(trimmedColor[4]);
                    b = ParseHexChar(trimmedColor[5]) * 16 + ParseHexChar(trimmedColor[6]);
                }
                else if (trimmedColor.Length > 4)
                {
                    a = ParseHexChar(trimmedColor[1]);
                    a = a + a * 16;
                    r = ParseHexChar(trimmedColor[2]);
                    r = r + r * 16;
                    g = ParseHexChar(trimmedColor[3]);
                    g = g + g * 16;
                    b = ParseHexChar(trimmedColor[4]);
                    b = b + b * 16;
                }
                else
                {
                    r = ParseHexChar(trimmedColor[1]);
                    r = r + r * 16;
                    g = ParseHexChar(trimmedColor[2]);
                    g = g + g * 16;
                    b = ParseHexChar(trimmedColor[3]);
                    b = b + b * 16;
                }

                return string.Format(
                    CultureInfo.InvariantCulture,
                    "global::System.Windows.Media.Color.FromArgb((byte){0}, (byte){1}, (byte){2}, (byte){3})",
                    a, r, g, b);
            }

            static string ParseScRgbColor(string trimmedColor)
            {
                if (!trimmedColor.StartsWith("sc#", StringComparison.Ordinal))
                {
                    throw new FormatException("Token is not valid.");
                }

                string tokens = trimmedColor.Substring(3, trimmedColor.Length - 3);

                string[] split = tokens.Split(_separators, StringSplitOptions.RemoveEmptyEntries);

                if (split.Length == 3)
                {
                    return string.Format(
                        CultureInfo.InvariantCulture,
                        "global::System.Windows.Media.Color.FromScRgb({0}F, {1}F, {2}F, {3}F)",
                        1.0f,
                        Convert.ToSingle(split[0], CultureInfo.InvariantCulture),
                        Convert.ToSingle(split[1], CultureInfo.InvariantCulture),
                        Convert.ToSingle(split[2], CultureInfo.InvariantCulture));
                }
                else if (split.Length == 4)
                {
                    return string.Format(
                        CultureInfo.InvariantCulture,
                        "global::System.Windows.Media.Color.FromScRgb({0}F, {1}F, {2}F, {3}F)",
                        Convert.ToSingle(split[0], CultureInfo.InvariantCulture),
                        Convert.ToSingle(split[1], CultureInfo.InvariantCulture),
                        Convert.ToSingle(split[2], CultureInfo.InvariantCulture),
                        Convert.ToSingle(split[3], CultureInfo.InvariantCulture));
                }

                throw new FormatException("Token is not valid.");
            }

            static string ParseColor(string colorString)
            {
                string trimmedColor = MatchColor(
                    colorString, out bool isPossibleKnowColor, out bool isNumericColor, out bool isScRgbColor);

                //Is it a number?
                if (isNumericColor)
                {
                    return ParseHexColor(trimmedColor);
                }
                else if (isScRgbColor)
                {
                    return ParseScRgbColor(trimmedColor);
                }
                else
                {
                    Debug.Assert(isPossibleKnowColor);

                    if (Enum.TryParse(trimmedColor, true, out ColorsEnum namedColor))
                    {
                        uint color = (uint)namedColor;

                        return string.Format(
                            CultureInfo.InvariantCulture,
                            "global::System.Windows.Media.Color.FromArgb((byte){0}, (byte){1}, (byte){2}, (byte){3})",
                            (color >> 0x18) & 0xff,
                            (color >> 0x10) & 0xff,
                            (color >> 8) & 0xff,
                            color & 0xff);
                    }
                }

                throw GetConvertException(colorString, "System.Windows.Media.Color");
            }

            return ParseColor(source);
        }

        internal static string ConvertToDoubleCollection(string source)
        {
            string[] split = source.Split(_separators, StringSplitOptions.RemoveEmptyEntries);

            var sb = new StringBuilder();

            sb.Append("new global::System.Windows.Media.DoubleCollection()");
            sb.Append("{");

            if (split != null && split.Length > 0)
            {
                foreach (string d in split)
                {
                    sb.Append(d).Append(", ");
                }
            }

            sb.Append("}");

            return sb.ToString();
        }

        internal static string ConvertToFontFamily(string source)
        {
            string fontName = Escape(source.Trim());

            return $"new global::System.Windows.Media.FontFamily({fontName})";
        }

        internal static string ConvertToGeometry(string source)
        {
            return ConvertFromInvariantStringHelper(source, "global::System.Windows.Media.Geometry");
        }

        internal static string ConvertToPathGeometry(string source)
        {
            return ConvertFromInvariantStringHelper(source, "global::System.Windows.Media.PathGeometry");
        }

        internal static string ConvertToMatrix(string source)
        {
            if (source == "Identity")
            {
                return "global::System.Windows.Media.Matrix.Identity";
            }

            string[] split = source.Split(_separators, StringSplitOptions.RemoveEmptyEntries);

            if (split.Length == 6)
            {
                return $"new global::System.Windows.Media.Matrix({split[0]}, {split[1]}, {split[2]}, {split[3]}, {split[4]}, {split[5]})";
            }

            throw GetConvertException(source, "System.Windows.Media.Matrix");
        }

        // "global::System.Windows.Media.PointCollection", "global::System.Windows.Point"
        internal static string ConvertToPointCollection(string source)
        {
            string[] split = source.Split(_separators, StringSplitOptions.RemoveEmptyEntries);

            // Points count needs to be an even number
            if (split.Length % 2 == 1)
            {
                throw GetConvertException(source, "System.Windows.Media.PointCollection");
            }

            var sb = new StringBuilder();

            sb.Append("new global::System.Windows.Media.PointCollection()");
            sb.Append("{");

            for (int i = 0; i < split.Length; i += 2)
            {
                sb.Append(ConvertPointHelper(split[i], split[i + 1]))
                  .Append(", ");
            }

            sb.Append("}");

            return sb.ToString();
        }

        internal static string ConvertToTransform(string source)
        {
            return $"new global::System.Windows.Media.MatrixTransform({ConvertToMatrix(source)})";
        }

        internal static string ConvertToCacheMode(string source)
        {
            if (source.Equals("BitmapCache", StringComparison.OrdinalIgnoreCase))
            {
                return "new global::System.Windows.Media.BitmapCache()";
            }

            throw GetConvertException(source, "System.Windows.Media.CacheMode");
        }

        internal static string ConvertToCornerRadius(string source)
        {
            string[] split = source.Split(_separators, StringSplitOptions.RemoveEmptyEntries);

            switch (split.Length)
            {
                case 1:
                    return $"new global::System.Windows.CornerRadius({split[0]})";

                case 4:
                    return $"new global::System.Windows.CornerRadius({split[0]}, {split[1]}, {split[2]}, {split[3]})";
            }

            throw GetConvertException(source, "System.Windows.CornerRadius");
        }

        internal static string ConvertToDuration(string source)
        {
            string stringValue = source.Trim();

            if (stringValue.Equals("Automatic", StringComparison.OrdinalIgnoreCase))
            {
                return "global::System.Windows.Duration.Automatic";
            }
            else if (stringValue.Equals("Forever", StringComparison.OrdinalIgnoreCase))
            {
                return "global::System.Windows.Duration.Forever";
            }
            else
            {
                return $"new global::System.Windows.Duration({SystemTypesHelper.CSharp.ConvertFromInvariantString(stringValue, "system.timespan")})";
            }
        }

        internal static string ConvertToFontWeight(string source)
        {
            if (Enum.TryParse(source, true, out FontWeightsCode fontCode))
            {
                return $"global::System.Windows.FontWeights.{fontCode}";
            }
            else if (ushort.TryParse(source, out ushort code))
            {
                string fontName = Enum.GetName(typeof(FontWeightsCode), code);
                if (fontName != null)
                {
                    return $"global::System.Windows.FontWeights.{fontName}";
                }
            }

            throw GetConvertException(source, "System.Windows.FontWeight");
        }

        internal static string ConvertToGridLength(string source)
        {
            static string ReadDouble(string seq, string defaultValue)
            {
                // flag used to keep track of dots in the sequence.
                // If we weet more than 1 dot, just ignore the rest of the sequence.
                bool isFloat = false;

                int i = 0;
                for (; i < seq.Length; i++)
                {
                    char c = seq[i];
                    if (c == '.')
                    {
                        if (isFloat)
                        {
                            break;
                        }

                        isFloat = true;
                        continue;
                    }
                    else if (!char.IsDigit(c))
                    {
                        break;
                    }
                }

                if (i == 0)
                {
                    return defaultValue;
                }
                else if (i == 1 && seq[0] == '.')
                {
                    return "0D";
                }

                return seq.Substring(0, i);
            }

            string value;
            string unit;

            string stringValue = source.Trim().ToLower();
            if (stringValue == "auto")
            {
                value = "1D";
                unit = "global::System.Windows.GridUnitType.Auto";
            }
            else if (stringValue.EndsWith("*"))
            {
                value = ReadDouble(stringValue, "1D");
                unit = "global::System.Windows.GridUnitType.Star";
            }
            else
            {
                value = ReadDouble(stringValue, "0D");
                unit = "global::System.Windows.GridUnitType.Pixel";
            }

            return $"new global::System.Windows.GridLength({value}, {unit})";
        }

        internal static string ConvertToPoint(string source)
        {
            string[] split = source.Split(_separators, StringSplitOptions.RemoveEmptyEntries);

            if (split.Length == 2)
            {
                return ConvertPointHelper(split[0], split[1]);
            }

            throw GetConvertException(source, "System.Windows.Point");
        }

        internal static string ConvertPointHelper(string x, string y)
        {
            return $"new global::System.Windows.Point({x}, {y})";
        }

        internal static string ConvertToPropertyPath(string source)
        {
            return $"new global::System.Windows.PropertyPath({Escape(source)})";
        }

        internal static string ConvertToRect(string source)
        {
            string[] split = source.Split(_separators, StringSplitOptions.RemoveEmptyEntries);

            if (split.Length == 4)
            {
                return $"new global::System.Windows.Rect({split[0]}, {split[1]}, {split[2]}, {split[3]})";
            }

            throw GetConvertException(source, "System.Windows.Rect");
        }

        internal static string ConvertToSize(string source)
        {
            string[] split = source.Split(_separators, StringSplitOptions.RemoveEmptyEntries);

            if (split.Length == 2)
            {
                return $"new global::System.Windows.Size({split[0]}, {split[1]})";
            }

            throw GetConvertException(source, "System.Windows.Size");
        }

        internal static string ConvertToThickness(string source)
        {
            string[] split = source.Split(_separators, StringSplitOptions.RemoveEmptyEntries);

            switch (split.Length)
            {
                case 1:
                    return $"new global::System.Windows.Thickness({split[0]})";

                case 2:
                    return $"new global::System.Windows.Thickness({split[0]}, {split[1]}, {split[0]}, {split[1]})";

                case 4:
                    return $"new global::System.Windows.Thickness({split[0]}, {split[1]}, {split[2]}, {split[3]})";
            }

            throw GetConvertException(source, "System.Windows.Thickness");
        }

        internal static string ConvertToFontStretch(string source)
        {
            string stringValue = source.Trim();
            if (stringValue.Equals("UltraCondensed", StringComparison.OrdinalIgnoreCase))
            {
                return "global::System.Windows.FontStretches.UltraCondensed";
            }
            else if (stringValue.Equals("ExtraCondensed", StringComparison.OrdinalIgnoreCase))
            {
                return "global::System.Windows.FontStretches.ExtraCondensed";
            }
            else if (stringValue.Equals("Condensed", StringComparison.OrdinalIgnoreCase))
            {
                return "global::System.Windows.FontStretches.Condensed";
            }
            else if (stringValue.Equals("SemiCondensed", StringComparison.OrdinalIgnoreCase))
            {
                return "global::System.Windows.FontStretches.SemiCondensed";
            }
            else if (stringValue.Equals("Normal", StringComparison.OrdinalIgnoreCase))
            {
                return "global::System.Windows.FontStretches.Normal";
            }
            else if (stringValue.Equals("SemiExpanded", StringComparison.OrdinalIgnoreCase))
            {
                return "global::System.Windows.FontStretches.SemiExpanded";
            }
            else if (stringValue.Equals("Expanded", StringComparison.OrdinalIgnoreCase))
            {
                return "global::System.Windows.FontStretches.Expanded";
            }
            else if (stringValue.Equals("ExtraExpanded", StringComparison.OrdinalIgnoreCase))
            {
                return "global::System.Windows.FontStretches.ExtraExpanded";
            }
            else if (stringValue.Equals("UltraExpanded", StringComparison.OrdinalIgnoreCase))
            {
                return "global::System.Windows.FontStretches.UltraExpanded";
            }

            throw GetConvertException(source, "System.Windows.FontStretch");
        }

        internal static string ConvertToFontStyle(string source)
        {
            if (source.Equals("Normal", StringComparison.OrdinalIgnoreCase))
            {
                return "global::System.Windows.FontStyles.Normal";
            }
            else if (source.Equals("Oblique", StringComparison.OrdinalIgnoreCase))
            {
                return "global::System.Windows.FontStyles.Oblique";
            }
            else if (source.Equals("Italic", StringComparison.OrdinalIgnoreCase))
            {
                return "global::System.Windows.FontStyles.Italic";
            }

            throw GetConvertException(source, "System.Windows.FontStyle");
        }

        internal static string ConvertToTextDecorationCollection(string source)
        {
            switch (source.Trim().ToLower())
            {
                case "underline":
                    return "global::System.Windows.TextDecorations.Underline";
                case "strikethrough":
                    return "global::System.Windows.TextDecorations.Strikethrough";
                case "overline":
                    return "global::System.Windows.TextDecorations.OverLine";
                //case "baseline":
                //    return $"{textDecorationsTypeFullName}.Baseline";
                case "none":
                    return "null";

                default:
                    throw GetConvertException(source, "System.Windows.TextDecorationCollection");
            }
        }

        internal static string ConvertToImageSource(string source)
        {
            string uriKind;
            if (source.Contains(":/"))
            {
                uriKind = "global::System.UriKind.Absolute";
            }
            else
            {
                uriKind = "global::System.UriKind.Relative";
            }

            return $"new global::System.Windows.Media.Imaging.BitmapImage(new global::System.Uri({Escape(source)}, {uriKind}))";
        }

        internal static string ConvertToVector(string source)
        {
            string[] split = source.Split(_separators, StringSplitOptions.RemoveEmptyEntries);

            if (split.Length == 2)
            {
                return $"new global::System.Windows.Vector({split[0]}, {split[1]})";
            }

            throw GetConvertException(source, "System.Windows.Vector");
        }

        private static Exception GetConvertException(string value, string destinationTypeFullName)
        {
            return new XamlParseException($"Cannot convert '{value}' to '{destinationTypeFullName}'.");
        }

        private static string Escape(string s)
        {
            return string.Concat("@\"", s.Replace("\"", "\"\""), "\"");
        }
    }
}
