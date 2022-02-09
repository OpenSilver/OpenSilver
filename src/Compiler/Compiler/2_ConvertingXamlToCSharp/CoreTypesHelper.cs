﻿
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

namespace DotNetForHtml5.Compiler
{
    internal static class CoreTypesHelper
    {
        public const string TypeFromStringConvertersFullName = "global::DotNetForHtml5.Core.TypeFromStringConverters";

        private static Dictionary<string, Func<string, string>> SupportedCoreTypes { get; }
            = GetSupportedCoreTypes();

        public static bool IsSupportedCoreType(string typeFullName, string assemblyName)
        {
            if (IsCoreAssemblyOrNull(assemblyName))
            {
                return SupportedCoreTypes.ContainsKey(typeFullName.ToLower());
            }

            return false;
        }

        public static string ConvertFromInvariantString(string source, string typeFullName)
        {
            if (SupportedCoreTypes.TryGetValue(typeFullName.ToLower(), out var converter))
            {
                Debug.Assert(converter != null);
                return converter(source);
            }

            throw new InvalidOperationException(
                $"Cannot find a converter for type '{typeFullName}'"    
            );
        }

        public static string ConvertFromInvariantStringHelper(string source, string destinationType)
        {
            return string.Format(
                "({0}){1}.ConvertFromInvariantString(typeof({0}), {2})",
                destinationType, TypeFromStringConvertersFullName, Escape(source)
            );
        }

        private static bool IsCoreAssemblyOrNull(string assemblyName)
        {
            if (assemblyName == null ||
                assemblyName.Equals(Constants.NAME_OF_CORE_ASSEMBLY_SLMIGRATION_USING_BLAZOR, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        private static string ConvertToCursor(string source, string destinationType, string cursorsTypeFullName)
        {
            return $"{cursorsTypeFullName}.{source}";
        }

        private static string ConvertToKeyTime(string source, string destinationType)
        {
            string stringValue = source.Trim();

            if (stringValue == "Uniform" || stringValue == "Paced")
            {
                throw new XamlParseException(
                    $"The '{destinationType}.{stringValue}' property is not supported yet."
                );
            }
            else if (stringValue.Length > 0 &&
                     stringValue[stringValue.Length - 1] == '%')
            {
                throw new XamlParseException(
                    $"Percentage values for '{destinationType}' are not supported yet."
                );
            }
            else
            {
                return SystemTypesHelper.ConvertFromInvariantString(stringValue, "system.timespan");
            }
        }

        private static string ConvertToRepeatBehavior(string source, string destinationType)
        {
            const char _iterationCharacter = 'x';

            string stringValue = source.Trim().ToLowerInvariant();

            if (stringValue == "forever")
            {
                return string.Format($"{destinationType}.Forever");
            }
            else if (stringValue.Length > 0 && 
                     stringValue[stringValue.Length - 1] == _iterationCharacter)
            {
                string stringDoubleValue = stringValue.TrimEnd(_iterationCharacter);

                return $"new {destinationType}({stringDoubleValue.TrimEnd()})";
            }

            return SystemTypesHelper.ConvertFromInvariantString(stringValue, "system.timespan");
        }

        private static string ConvertToBrush(string source, string destinationType, string colorTypeName)
        {
            return $"new {destinationType}({ConvertToColor(source, colorTypeName)})";
        }

        private static string ConvertToColor(string source, string destinationType)
        {
            const int s_zeroChar = (int)'0';
            const int s_aLower = (int)'a';
            const int s_aUpper = (int)'A';

            string MatchColor(string colorString, out bool isKnownColor, out bool isNumericColor, out bool isScRgbColor)
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

            int ParseHexChar(char c)
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

            string ParseHexColor(string trimmedColor)
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
                    "{0}.FromArgb((byte){1}, (byte){2}, (byte){3}, (byte){4})",
                    destinationType, a, r, g, b
                );
            }

            string ParseScRgbColor(string trimmedColor)
            {
                if (!trimmedColor.StartsWith("sc#", StringComparison.Ordinal))
                {
                    throw new FormatException("Token is not valid.");
                }

                string tokens = trimmedColor.Substring(3, trimmedColor.Length - 3);

                char[] separator = new char[2] { ',', ' ' };
                string[] split = tokens.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                if (split.Length == 3)
                {
                    return string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}.FromScRgb({1}F, {2}F, {3}F, {4}F)",
                        destinationType,
                        1.0f,
                        Convert.ToSingle(split[0], CultureInfo.InvariantCulture),
                        Convert.ToSingle(split[1], CultureInfo.InvariantCulture),
                        Convert.ToSingle(split[2], CultureInfo.InvariantCulture)
                    );
                }
                else if (split.Length == 4)
                {
                    return string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}.FromScRgb({1}F, {2}F, {3}F, {4}F)",
                        destinationType,
                        Convert.ToSingle(split[0], CultureInfo.InvariantCulture),
                        Convert.ToSingle(split[1], CultureInfo.InvariantCulture),
                        Convert.ToSingle(split[2], CultureInfo.InvariantCulture),
                        Convert.ToSingle(split[3], CultureInfo.InvariantCulture)
                    );
                }

                throw new FormatException("Token is not valid.");
            }

            string ParseColor(string colorString)
            {
                string trimmedColor = MatchColor(
                    colorString, out bool isPossibleKnowColor, out bool isNumericColor, out bool isScRgbColor
                );

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
                        int color = (int)namedColor;

                        return string.Format(
                            CultureInfo.InvariantCulture,
                            "{0}.FromArgb((byte){1}, (byte){2}, (byte){3}, (byte){4})",
                            destinationType,
                            (color >> 0x18) & 0xff,
                            (color >> 0x10) & 0xff,
                            (color >> 8) & 0xff,
                            color & 0xff
                        );
                    }
                }

                throw GetConvertException(colorString, destinationType);
            }

            return ParseColor(source);
        }
        
        private static string ConvertToDoubleCollection(string source, string destinationType)
        {
            string[] split = source.Split(new char[2] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            var sb = new StringBuilder();

            sb.Append($"new {destinationType}()");
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

        private static string ConvertToFontFamily(string source, string destinationType)
        {
            string fontName = Escape(source.Trim());

            return $"new {destinationType}({fontName})";
        }

        private static string ConvertToGeometry(string source, string destinationType)
        {
            return ConvertFromInvariantStringHelper(source, destinationType);
        }

        private static string ConvertToMatrix(string source, string destinationType)
        {
            if (source == "Identity")
            {
                return $"{destinationType}.Identity";
            }

            string[] split = source.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (split.Length == 6)
            {
                return string.Format(
                    "new {0}({1}, {2}, {3}, {4}, {5}, {6})",
                    destinationType, split[0], split[1], split[2], split[3], split[4], split[5]
                );
            }

            throw GetConvertException(source, destinationType);
        }

        private static string ConvertToPointCollection(string source, string destinationType, string pointTypeFullName)
        {
            string[] split = source.Split(new char[2] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Points count needs to be an even number
            if (split.Length % 2 == 1)
            {
                throw GetConvertException(source, destinationType);
            }

            var sb = new StringBuilder();

            sb.Append($"new {destinationType}()");
            sb.Append("{");

            for (int i = 0; i < split.Length; i += 2)
            {
                sb.Append(ConvertPointHelper(split[i], split[i + 1], pointTypeFullName))
                  .Append(", ");
            }

            sb.Append("}");

            return sb.ToString();
        }

        private static string ConvertToTransform(string source, string destinationType, string matrixTypeFullName)
        {
            return $"new {destinationType}({ConvertToMatrix(source, matrixTypeFullName)})";
        }

        private static string ConvertToCacheMode(string source, string destinationType, string bitmapCacheTypeFullName)
        {
            if (source.Equals("BitmapCache", StringComparison.OrdinalIgnoreCase))
            {
                return $"new {bitmapCacheTypeFullName}()";
            }

            throw GetConvertException(source, destinationType);
        }

        private static string ConvertToCornerRadius(string source, string destinationType)
        {
            char[] separator = new char[2] { ',', ' ' };
            string[] split = source.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            switch (split.Length)
            {
                case 1:
                    return $"new {destinationType}({split[0]})";

                case 4:
                    return $"new {destinationType}({split[0]}, {split[1]}, {split[2]}, {split[3]})";
            }

            throw GetConvertException(source, destinationType);
        }

        private static string ConvertToDuration(string source, string destinationType)
        {
            string stringValue = source.Trim();

            if (stringValue.Equals("Automatic", StringComparison.OrdinalIgnoreCase))
            {
                return $"{destinationType}.Automatic";
            }
            else if (stringValue.Equals("Forever", StringComparison.OrdinalIgnoreCase))
            {
                return $"{destinationType}.Forever";
            }
            else
            {
                return SystemTypesHelper.ConvertFromInvariantString(stringValue, "system.timespan");
            }
        }

        private static string ConvertToFontWeight(string source, string destinationType, string fontWeightsTypeFullName)
        {
            if (Enum.TryParse(source, true, out FontweightsEnum fontCode))
            {
                return $"{fontWeightsTypeFullName}.{fontCode}";
            }
            else if (ushort.TryParse(source, out ushort code))
            {
                string fontName = Enum.GetName(typeof(FontweightsEnum), code);
                if (fontName != null)
                {
                    return $"{fontWeightsTypeFullName}.{fontName}";
                }
            }

            throw GetConvertException(source, destinationType);
        }

        private static string ConvertToGridLength(string source, string destinationType, string gridUnitTypeFullName)
        {
            string ReadDouble(string seq, string defaultValue)
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
                unit = $"{gridUnitTypeFullName}.Auto";
            }
            else if (stringValue.EndsWith("*"))
            {
                value = ReadDouble(stringValue, "1D");
                unit = $"{gridUnitTypeFullName}.Star";
            }
            else
            {
                value = ReadDouble(stringValue, "0D");
                unit = $"{gridUnitTypeFullName}.Pixel";
            }

            return $"new {destinationType}({value}, {unit})";
        }

        private static string ConvertToPoint(string source, string destinationType)
        {
            string[] split = source.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (split.Length == 2)
            {
                return ConvertPointHelper(split[0], split[1], destinationType);
            }

            throw GetConvertException(source, destinationType);
        }

        private static string ConvertPointHelper(string x, string y, string pointTypeFullName)
        {
            return $"new {pointTypeFullName}({x}, {y})";
        }

        private static string ConvertToPropertyPath(string source, string destinationType)
        {
            return $"new {destinationType}({Escape(source)})";
        }

        private static string ConvertToRect(string source, string destinationType)
        {
            string[] split = source.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (split.Length == 4)
            {
                return string.Format(
                    "new {0}({1}, {2}, {3}, {4})",
                    destinationType, split[0], split[1], split[2], split[3]
                );
            }

            throw GetConvertException(source, destinationType);
        }

        private static string ConvertToSize(string source, string destinationType)
        {
            string[] split = source.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (split.Length == 2)
            {
                return string.Format(
                    "new {0}({1}, {2})",
                    destinationType, split[0], split[1]
                );
            }

            throw GetConvertException(source, destinationType);
        }

        private static string ConvertToThickness(string source, string destinationType)
        {
            char[] separator = new char[2] { ',', ' ' };

            string[] split = source.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            switch (split.Length)
            {
                case 1:
                    return $"new {destinationType}({split[0]})";

                case 2:
                    return $"new {destinationType}({split[0]}, {split[1]}, {split[0]}, {split[1]})";

                case 4:
                    return $"new {destinationType}({split[0]}, {split[1]}, {split[2]}, {split[3]})";
            }

            throw GetConvertException(source, destinationType);
        }

        private static string ConvertToFontStretch(string source, string destinationType)
        {
            return $"new {destinationType}()";
        }

#if MIGRATION
        private static string ConvertToFontStyle(string source, string destinationType, string fontStylesFullTypeName)
        {
            if (source.Equals("Normal", StringComparison.OrdinalIgnoreCase))
            {
                return $"{fontStylesFullTypeName}.Normal";
            }
            else if (source.Equals("Oblique", StringComparison.OrdinalIgnoreCase))
            {
                return $"{fontStylesFullTypeName}.Oblique";
            }
            else if (source.Equals("Italic", StringComparison.OrdinalIgnoreCase))
            {
                return $"{fontStylesFullTypeName}.Italic";
            }

            throw GetConvertException(source, destinationType);
        }

        private static string ConvertToTextDecorationCollection(string source, string destinationType, string textDecorationsTypeFullName)
        {
            switch (source.Trim().ToLower())
            {
                case "underline":
                    return $"{textDecorationsTypeFullName}.Underline";
                case "strikethrough":
                    return $"{textDecorationsTypeFullName}.Strikethrough";
                case "overline":
                    return $"{textDecorationsTypeFullName}.OverLine";
                //case "baseline":
                //    return $"{textDecorationsTypeFullName}.Baseline";
                case "none":
                    return "null";

                default:
                    throw GetConvertException(source, destinationType);
            }
        }
#endif

        private static string ConvertToImageSource(string source, string destinationType, string bitmapImageTypeFullName)
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

            return $"new {bitmapImageTypeFullName}(new global::System.Uri({Escape(source)}, {uriKind}))";
        }

        private static Exception GetConvertException(string value, string destinationTypeFullName)
        {
            return new XamlParseException(
                $"Cannot convert '{value}' to '{destinationTypeFullName}'."
            );
        }

        private static string Escape(string s)
        {
            return string.Concat("@\"", s.Replace("\"", "\"\""), "\"");
        }

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
#if MIGRATION
            return new Dictionary<string, Func<string, string>>(27)
            {
                ["system.windows.input.cursor"] = (s => ConvertToCursor(s, "global::System.Windows.Input.Cursor", "global::System.Windows.Input.Cursors")),
                ["system.windows.media.animation.keytime"] = (s => ConvertToKeyTime(s, "global::System.Windows.Media.Animation.KeyTime")),
                ["system.windows.media.animation.repeatbehavior"] = (s => ConvertToRepeatBehavior(s, "global::System.Windows.Media.Animation.RepeatBehavior")),
                ["system.windows.media.brush"] = (s => ConvertToBrush(s, "global::System.Windows.Media.SolidColorBrush", "global::System.Windows.Media.Color")),
                ["system.windows.media.solidcolorbrush"] = (s => ConvertToBrush(s, "global::System.Windows.Media.SolidColorBrush", "global::System.Windows.Media.Color")),
                ["system.windows.media.color"] = (s => ConvertToColor(s, "global::System.Windows.Media.Color")),
                ["system.windows.media.doublecollection"] = (s => ConvertToDoubleCollection(s, "global::System.Windows.Media.DoubleCollection")),
                ["system.windows.media.fontfamily"] = (s => ConvertToFontFamily(s, "global::System.Windows.Media.FontFamily")),
                ["system.windows.media.geometry"] = (s => ConvertToGeometry(s, "global::System.Windows.Media.Geometry")),
                ["system.windows.media.pathgeometry"] = (s => ConvertToGeometry(s, "global::System.Windows.Media.PathGeometry")),
                ["system.windows.media.matrix"] = (s => ConvertToMatrix(s, "global::System.Windows.Media.Matrix")),
                ["system.windows.media.pointcollection"] = (s => ConvertToPointCollection(s, "global::System.Windows.Media.PointCollection", "global::System.Windows.Point")),
                ["system.windows.media.transform"] = (s => ConvertToTransform(s, "global::System.Windows.Media.MatrixTransform", "global::System.Windows.Media.Matrix")),
                ["system.windows.media.matrixtransform"] = (s => ConvertToTransform(s, "global::System.Windows.Media.MatrixTransform", "global::System.Windows.Media.Matrix")),
                ["system.windows.media.cachemode"] = (s => ConvertToCacheMode(s, "global::System.Windows.Media.CacheMode", "global::System.Windows.Media.BitmapCache")),
                ["system.windows.cornerradius"] = (s => ConvertToCornerRadius(s, "global::System.Windows.CornerRadius")),
                ["system.windows.duration"] = (s => ConvertToDuration(s, "global::System.Windows.Duration")),
                ["system.windows.fontweight"] = (s => ConvertToFontWeight(s, "global::System.Windows.FontWeight", "global::System.Windows.FontWeights")),
                ["system.windows.gridlength"] = (s => ConvertToGridLength(s, "global::System.Windows.GridLength", "global::System.Windows.GridUnitType")),
                ["system.windows.point"] = (s => ConvertToPoint(s, "global::System.Windows.Point")),
                ["system.windows.propertypath"] = (s => ConvertToPropertyPath(s, "global::System.Windows.PropertyPath")),
                ["system.windows.rect"] = (s => ConvertToRect(s, "global::System.Windows.Rect")),
                ["system.windows.size"] = (s => ConvertToSize(s, "global::System.Windows.Size")),
                ["system.windows.thickness"] = (s => ConvertToThickness(s, "global::System.Windows.Thickness")),
                ["system.windows.fontstretch"] = (s => ConvertToFontStretch(s, "global::System.Windows.FontStretch")),
                ["system.windows.fontstyle"] = (s => ConvertToFontStyle(s, "global::System.Windows.FontStyle", "global::System.Windows.FontStyles")),
                ["system.windows.textdecorationcollection"] = (s => ConvertToTextDecorationCollection(s, "global::System.Windows.TextDecorationCollection", "global::System.Windows.TextDecorations")),
                ["system.windows.media.imagesource"] = (s => ConvertToImageSource(s, "global::System.Windows.Media.ImageSource", "global::System.Windows.Media.Imaging.BitmapImage")),
            };
#else
            return new Dictionary<string, Func<string, string>>(25)
            {
                ["system.windows.input.cursor"] = (s => ConvertToCursor(s, "global::System.Windows.Input.Cursor", "global::System.Windows.Input.Cursors")),
                ["windows.ui.xaml.media.animation.keytime"] = (s => ConvertToKeyTime(s, "global::Windows.UI.Xaml.Media.Animation.KeyTime")),
                ["windows.ui.xaml.media.animation.repeatbehavior"] = (s => ConvertToRepeatBehavior(s, "global::Windows.UI.Xaml.Media.Animation.RepeatBehavior")),
                ["windows.ui.xaml.media.brush"] = (s => ConvertToBrush(s, "global::Windows.UI.Xaml.Media.SolidColorBrush", "global::Windows.UI.Color")),
                ["windows.ui.xaml.media.solidcolorbrush"] = (s => ConvertToBrush(s, "global::Windows.UI.Xaml.Media.SolidColorBrush", "global::Windows.UI.Color")),
                ["windows.ui.color"] = (s => ConvertToColor(s, "global::Windows.UI.Color")),
                ["windows.ui.xaml.media.doublecollection"] = (s => ConvertToDoubleCollection(s, "global::Windows.UI.Xaml.Media.DoubleCollection")),
                ["windows.ui.xaml.media.fontfamily"] = (s => ConvertToFontFamily(s, "global::Windows.UI.Xaml.Media.FontFamily")),
                ["windows.ui.xaml.media.geometry"] = (s => ConvertToGeometry(s, "global::Windows.UI.Xaml.Media.Geometry")),
                ["windows.ui.xaml.media.pathgeometry"] = (s => ConvertToGeometry(s, "global::Windows.UI.Xaml.Media.PathGeometry")),
                ["windows.ui.xaml.media.matrix"] = (s => ConvertToMatrix(s, "global::Windows.UI.Xaml.Media.Matrix")),
                ["windows.ui.xaml.media.pointcollection"] = (s => ConvertToPointCollection(s, "global::Windows.UI.Xaml.Media.PointCollection", "global::Windows.Foundation.Point")),
                ["windows.ui.xaml.media.transform"] = (s => ConvertToTransform(s, "global::Windows.UI.Xaml.Media.MatrixTransform", "global::Windows.UI.Xaml.Media.Matrix")),
                ["windows.ui.xaml.media.matrixtransform"] = (s => ConvertToTransform(s, "global::Windows.UI.Xaml.Media.MatrixTransform", "global::Windows.UI.Xaml.Media.Matrix")),
                ["windows.ui.xaml.media.cachemode"] = (s => ConvertToCacheMode(s, "global::Windows.UI.Xaml.Media.CacheMode", "global::Windows.UI.Xaml.Media.BitmapCache")),
                ["windows.ui.xaml.cornerradius"] = (s => ConvertToCornerRadius(s, "global::Windows.UI.Xaml.CornerRadius")),
                ["windows.ui.xaml.duration"] = (s => ConvertToDuration(s, "global::Windows.UI.Xaml.Duration")),
                ["windows.ui.text.fontweight"] = (s => ConvertToFontWeight(s, "global::Windows.UI.Text.FontWeight", "global::Windows.UI.Text.FontWeights")),
                ["windows.ui.xaml.gridlength"] = (s => ConvertToGridLength(s, "global::Windows.UI.Xaml.GridLength", "global::Windows.UI.Xaml.GridUnitType")),
                ["windows.foundation.point"] = (s => ConvertToPoint(s, "global::Windows.Foundation.Point")),
                ["windows.ui.xaml.propertypath"] = (s => ConvertToPropertyPath(s, "global::Windows.UI.Xaml.PropertyPath")),
                ["windows.foundation.rect"] = (s => ConvertToRect(s, "global::Windows.Foundation.Rect")),
                ["windows.foundation.size"] = (s => ConvertToSize(s, "global::Windows.Foundation.Size")),
                ["windows.ui.xaml.thickness"] = (s => ConvertToThickness(s, "global::Windows.UI.Xaml.Thickness")),
                ["windows.ui.xaml.fontstretch"] = (s => ConvertToFontStretch(s, "global::Windows.UI.Xaml.FontStretch")),
                ["windows.ui.xaml.media.imagesource"] = (s => ConvertToImageSource(s, "global::Windows.UI.Xaml.Media.ImageSource", "global::Windows.UI.Xaml.Media.Imaging.BitmapImage")),
            };
#endif
        }
    }
}
