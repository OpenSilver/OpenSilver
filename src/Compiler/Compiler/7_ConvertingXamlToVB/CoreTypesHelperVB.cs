
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

using Mono.Cecil;
using OpenSilver.Compiler.OtherHelpersAndHandlers.MonoCecilAssembliesInspector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Xml.Linq;

namespace OpenSilver.Compiler
{
    internal sealed class CoreTypesConverterVB : CoreTypesConverterBase
    {
        public const string RuntimeHelperClass = "Global.OpenSilver.Internal.Xaml.RuntimeHelpers";
        private static readonly char[] _separators = [',', ' '];
        private static readonly char[] _repeatBehaviorConverterIterationCharacter = ['x', 'X'];

        private readonly AssembliesInspector _inspector;
        private readonly string _assemblyName;

        public CoreTypesConverterVB(AssembliesInspector inspector, string assemblyName)
        {
            _inspector = inspector;
            _assemblyName = assemblyName;
            SupportedCoreTypes = GetSupportedCoreTypes();
        }

        protected override Dictionary<string, Func<XElement, string, string>> SupportedCoreTypes { get; }

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
        private Dictionary<string, Func<XElement, string, string>> GetSupportedCoreTypes()
        {
            return new Dictionary<string, Func<XElement, string, string>>(33, StringComparer.OrdinalIgnoreCase)
            {
                ["system.windows.input.cursor"] = ConvertToCursor,
                ["system.windows.input.modifierkeys"] = ConvertToModifierKeys,
                ["system.windows.input.key"] = ConvertToKey,
                ["system.windows.media.animation.keytime"] = ConvertToKeyTime,
                ["system.windows.media.animation.repeatbehavior"] = ConvertToRepeatBehavior,
                ["system.windows.media.animation.keyspline"] = ConvertToKeySpline,
                ["system.windows.media.brush"] = ConvertToBrush,
                ["system.windows.media.solidcolorbrush"] = ConvertToBrush,
                ["system.windows.media.color"] = ConvertToColor,
                ["system.windows.media.doublecollection"] = ConvertToDoubleCollection,
                ["system.windows.media.fontfamily"] = ConvertToFontFamily,
                ["system.windows.media.geometry"] = ConvertToGeometry,
                ["system.windows.media.pathgeometry"] = ConvertToPathGeometry,
                ["system.windows.media.matrix"] = ConvertToMatrix,
                ["system.windows.media.pointcollection"] = ConvertToPointCollection,
                ["system.windows.media.transform"] = ConvertToTransform,
                ["system.windows.media.matrixtransform"] = ConvertToTransform,
                ["system.windows.media.cachemode"] = ConvertToCacheMode,
                ["system.windows.cornerradius"] = ConvertToCornerRadius,
                ["system.windows.duration"] = ConvertToDuration,
                ["system.windows.fontweight"] = ConvertToFontWeight,
                ["system.windows.gridlength"] = ConvertToGridLength,
                ["system.windows.point"] = ConvertToPoint,
                ["system.windows.propertypath"] = ConvertToPropertyPath,
                ["system.windows.rect"] = ConvertToRect,
                ["system.windows.size"] = ConvertToSize,
                ["system.windows.thickness"] = ConvertToThickness,
                ["system.windows.fontstretch"] = ConvertToFontStretch,
                ["system.windows.fontstyle"] = ConvertToFontStyle,
                ["system.windows.textdecorationcollection"] = ConvertToTextDecorationCollection,
                ["system.windows.media.imagesource"] = ConvertToImageSource,
                ["system.windows.vector"] = ConvertToVector,
                ["system.windows.routedevent"] = ConvertToRoutedEvent,
            };
        }

        public static string ConvertFromInvariantStringHelper(string source, string destinationType)
        {
            return $"{RuntimeHelperClass}.ConvertFromInvariantString(Of {destinationType})({Escape(source)})";
        }

        private static string ConvertToCursor(XElement context, string source)
        {
            return $"Global.System.Windows.Input.Cursors.{source}";
        }

        private string ConvertToModifierKeys(XElement context, string source)
        {
            char[] separator = ['+'];

            string modifiersToken = source.Trim();

            // Empty token means there were no modifiers, exit early
            if (modifiersToken.Length == 0 || modifiersToken.Equals("None", StringComparison.OrdinalIgnoreCase))
            {
                return "Global.System.Windows.Input.ModifierKeys.None";
            }

            // Silverlight uses the default enum converter. To remain compatible, in case there is no + in the source string, we
            // use the default converter to support the comma based syntax ("Alt,Control,Windows" instead of "Alt+Control+Windows"
            // for instance).
            if (modifiersToken.IndexOfAny(separator) == -1)
            {
                if (TryParseModifier(modifiersToken, out string modifier))
                {
                    return modifier;
                }

                TypeDefinition modifierKeysType = _inspector.GetTypeDefinition("System.Windows.Input", "ModifierKeys", "OpenSilver");
                return string.Join(" | ", _inspector.GetEnumValues(modifierKeysType, modifiersToken, true, true));
            }

            var sb = new StringBuilder();

            // Split modifier keys by the delimiter
            string[] modifiers = modifiersToken.Split(separator);

            for (int i = 0; i < modifiers.Length; i++)
            {
                string modifier = modifiers[i].Trim();

                // This would be a case where we have a token like "Ctrl + " for example,
                // which itself is invalid but we choose to support this malformed behaviour.
                if (modifier.Length == 0)
                {
                    break;
                }

                if (!TryParseModifier(modifier, out string key))
                {
                    throw GetConvertException(source, "System.Windows.Input.ModifierKeys");
                }

                if (sb.Length > 0)
                {
                    sb.Append(" | ");
                }

                sb.Append(key);
            }

            if (sb.Length == 0)
            {
                return "Global.System.Windows.Input.ModifierKeys.None";
            }

            return sb.ToString();

            static bool TryParseModifier(string source, out string modifier)
            {
                modifier = source switch
                {
                    _ when source.Equals("Ctrl", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.ModifierKeys.Control",
                    _ when source.Equals("Control", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.ModifierKeys.Control",
                    _ when source.Equals("Win", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.ModifierKeys.Windows",
                    _ when source.Equals("Windows", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.ModifierKeys.Windows",
                    _ when source.Equals("Apple", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.ModifierKeys.Apple",
                    _ when source.Equals("Alt", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.ModifierKeys.Alt",
                    _ when source.Equals("Shift", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.ModifierKeys.Shift",
                    _ => null,
                };

                return modifier is not null;
            }
        }

        private string ConvertToKey(XElement context, string source)
        {
            string keyToken = source.Trim();

            if (keyToken.Length == 0)
            {
                return "Global.System.Windows.Input.Key.None";
            }

            // In case we're dealing with a lowercase character, we uppercase it
            char firstChar = keyToken[0];
            if (firstChar >= 'a' && firstChar <= 'z')
            {
                firstChar = char.ToUpper(firstChar);
            }

            // If this is a single-character we're dealing with, match digits/letters
            if (keyToken.Length == 1 && char.IsLetterOrDigit(firstChar))
            {
                if (firstChar >= '0' && firstChar <= '9')
                {
                    return $"Global.System.Windows.Input.Key.D{firstChar}";
                }
                else if (firstChar >= 'A' && firstChar <= 'Z')
                {
                    return $"Global.System.Windows.Input.Key.{firstChar}";
                }
                else
                {
                    throw GetConvertException(source, "System.Windows.Input.Key");
                }
            }

            return keyToken switch
            {
                _ when keyToken.Equals("ENTER", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.Return",
                _ when keyToken.Equals("ESC", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.Escape",
                _ when keyToken.Equals("PGUP", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.PageUp",
                _ when keyToken.Equals("PGDN", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.PageDown",
                //_ when keyToken.Equals("PRTSC", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.PrintScreen",
                _ when keyToken.Equals("INS", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.Insert",
                _ when keyToken.Equals("DEL", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.Delete",
                _ when keyToken.Equals("WINDOWS", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.LWin",
                _ when keyToken.Equals("WIN", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.LWin",
                _ when keyToken.Equals("LEFTWINDOWS", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.LWin",
                _ when keyToken.Equals("RIGHTWINDOWS", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.RWin",
                _ when keyToken.Equals("APPS", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.Apps",
                _ when keyToken.Equals("APPLICATION", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.Apps",
                _ when keyToken.Equals("BREAK", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.Cancel",
                _ when keyToken.Equals("BACKSPACE", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.Back",
                _ when keyToken.Equals("BKSP", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.Back",
                _ when keyToken.Equals("BS", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.Back",
                _ when keyToken.Equals("SHIFT", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.Shift", // Key.LeftShift,
                _ when keyToken.Equals("LEFTSHIFT", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.Shift", // Key.LeftShift,
                //_ when keyToken.Equals("RIGHTSHIFT", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.RightShift",
                _ when keyToken.Equals("CONTROL", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.Ctrl", // Key.LeftCtrl,
                _ when keyToken.Equals("CTRL", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.Ctrl", // Key.LeftCtrl,
                _ when keyToken.Equals("LEFTCTRL", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.Ctrl", // Key.LeftCtrl,
                //_ when keyToken.Equals("RIGHTCTRL", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.RightCtrl",
                _ when keyToken.Equals("ALT", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.Alt", // Key.LeftAlt,
                _ when keyToken.Equals("LEFTALT", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.Alt", // Key.LeftAlt,
                //_ when keyToken.Equals("RIGHTALT", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.RightAlt",
                //_ when keyToken.Equals("SEMICOLON", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.OemSemicolon",
                //_ when keyToken.Equals("PLUS", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.OemPlus",
                //_ when keyToken.Equals("COMMA", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.OemComma",
                //_ when keyToken.Equals("MINUS", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.OemMinus",
                //_ when keyToken.Equals("PERIOD", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.OemPeriod",
                //_ when keyToken.Equals("QUESTION", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.OemQuestion",
                //_ when keyToken.Equals("TILDE", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.OemTilde",
                //_ when keyToken.Equals("OPENBRACKETS", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.OemOpenBrackets",
                //_ when keyToken.Equals("PIPE", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.OemPipe",
                //_ when keyToken.Equals("CLOSEBRACKETS", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.OemCloseBrackets",
                //_ when keyToken.Equals("QUOTES", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.OemQuotes",
                //_ when keyToken.Equals("BACKSLASH", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.OemBackslash",
                //_ when keyToken.Equals("FINISH", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.OemFinish",
                //_ when keyToken.Equals("ATTN", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.Attn",
                //_ when keyToken.Equals("CRSEL", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.CrSel",
                //_ when keyToken.Equals("EXSEL", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.ExSel",
                //_ when keyToken.Equals("ERASEEOF", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.EraseEof",
                //_ when keyToken.Equals("PLAY", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.Play",
                //_ when keyToken.Equals("ZOOM", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.Zoom",
                //_ when keyToken.Equals("PA1", StringComparison.OrdinalIgnoreCase) => "Global.System.Windows.Input.Key.Pa1",
                _ => Parse(keyToken, _inspector) ?? throw GetConvertException(source, "System.Windows.Input.Key"),
            };

            static string Parse(string source, AssembliesInspector inspector)
            {
                TypeDefinition keyType = inspector.GetTypeDefinition("System.Windows.Input", "Key", "OpenSilver");
                return inspector.GetEnumValue(keyType, source, true, true);
            }
        }

        private static string ConvertToKeyTime(XElement context, string source)
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
                return $"Global.System.Windows.Media.Animation.KeyTime.Uniform";
            }
            else
            {
                string timeSpanValue = SystemTypesHelper.VisualBasic.ConvertFromInvariantString(stringValue, "system.timespan");
                return $"Global.System.Windows.Media.Animation.KeyTime.FromTimeSpan({timeSpanValue})";
            }
        }

        private static string ConvertToRepeatBehavior(XElement context, string source)
        {
            string stringValue = source.Trim();

            if (string.Equals(stringValue, "Forever", StringComparison.OrdinalIgnoreCase))
            {
                return "Global.System.Windows.Media.Animation.RepeatBehavior.Forever";
            }
            else if (stringValue.Length > 0 &&
                     char.ToLowerInvariant(stringValue[stringValue.Length - 1]) == _repeatBehaviorConverterIterationCharacter[0])
            {
                string stringDoubleValue = stringValue.TrimEnd(_repeatBehaviorConverterIterationCharacter);

                return $"New Global.System.Windows.Media.Animation.RepeatBehavior({SystemTypesHelper.VisualBasic.ConvertFromInvariantString(stringDoubleValue, "system.double")})";
            }

            string timeSpanValue = SystemTypesHelper.VisualBasic.ConvertFromInvariantString(stringValue, "system.timespan");

            return $"New Global.System.Windows.Media.Animation.RepeatBehavior({timeSpanValue})";
        }

        private static string ConvertToKeySpline(XElement context, string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return "New Global.System.Windows.Media.Animation.KeySpline()";
            }

            string[] split = source.Split(_separators, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length == 4)
            {
                return $"New Global.System.Windows.Media.Animation.KeySpline({split[0]}, {split[1]}, {split[2]}, {split[3]})";
            }

            throw GetConvertException(source, "System.Windows.Media.Animation.KeySpline");
        }

        private static string ConvertToBrush(XElement context, string source)
        {
            return $"New Global.System.Windows.Media.SolidColorBrush({ConvertToColor(context, source)})";
        }

        private static string ConvertToColor(XElement context, string source)
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

                string A = a.ToString(CultureInfo.InvariantCulture);
                string R = r.ToString(CultureInfo.InvariantCulture);
                string G = g.ToString(CultureInfo.InvariantCulture);
                string B = b.ToString(CultureInfo.InvariantCulture);
                return $"Global.System.Windows.Media.Color.FromArgb(CByte({A}), CByte({R}), CByte({G}), CByte({B}))";
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
                    string r = Convert.ToSingle(split[0], CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
                    string g = Convert.ToSingle(split[1], CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
                    string b = Convert.ToSingle(split[2], CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
                    return $"Global.System.Windows.Media.Color.FromScRgb(1F, {r}F, {g}F, {b}F)";
                }
                else if (split.Length == 4)
                {
                    string a = Convert.ToSingle(split[0], CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
                    string r = Convert.ToSingle(split[1], CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
                    string g = Convert.ToSingle(split[2], CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
                    string b = Convert.ToSingle(split[3], CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
                    return $"Global.System.Windows.Media.Color.FromScRgb({a}F, {r}F, {g}F, {b}F)";
                }

                throw new FormatException("Token is not valid.");
            }

            static string ParseColor(string colorString)
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

                        string a = ((color >> 0x18) & 0xff).ToString(CultureInfo.InvariantCulture);
                        string r = ((color >> 0x10) & 0xff).ToString(CultureInfo.InvariantCulture);
                        string g = ((color >> 8) & 0xff).ToString(CultureInfo.InvariantCulture);
                        string b = (color & 0xff).ToString(CultureInfo.InvariantCulture);
                        return $"Global.System.Windows.Media.Color.FromArgb(CByte({a}), CByte({r}), CByte({g}), CByte({b}))";
                    }
                }

                throw GetConvertException(colorString, "System.Windows.Media.Color");
            }

            return ParseColor(source);
        }

        private static string ConvertToDoubleCollection(XElement context, string source)
        {
            string[] split = source.Split(_separators, StringSplitOptions.RemoveEmptyEntries);

            var sb = new StringBuilder();

            sb.Append("New Global.System.Windows.Media.DoubleCollection() From ");
            sb.Append("{");

            if (split != null && split.Length > 0)
                sb.Append(string.Join(",", split));

            sb.Append("}");

            return sb.ToString();
        }

        private static string ConvertToFontFamily(XElement context, string source)
        {
            string fontName = Escape(source.Trim());

            return $"New Global.System.Windows.Media.FontFamily({fontName})";
        }

        private static string ConvertToGeometry(XElement context, string source)
        {
            return ConvertFromInvariantStringHelper(source, "Global.System.Windows.Media.Geometry");
        }

        private static string ConvertToPathGeometry(XElement context, string source)
        {
            return ConvertFromInvariantStringHelper(source, "Global.System.Windows.Media.PathGeometry");
        }

        private static string ConvertToMatrix(XElement context, string source)
        {
            if (source == "Identity")
            {
                return "Global.System.Windows.Media.Matrix.Identity";
            }

            string[] split = source.Split(_separators, StringSplitOptions.RemoveEmptyEntries);

            if (split.Length == 6)
            {
                return $"New Global.System.Windows.Media.Matrix({split[0]}, {split[1]}, {split[2]}, {split[3]}, {split[4]}, {split[5]})";
            }

            throw GetConvertException(source, "System.Windows.Media.Matrix");
        }

        private static string ConvertToPointCollection(XElement context, string source)
        {
            string[] split = source.Split(_separators, StringSplitOptions.RemoveEmptyEntries);

            // Points count needs to be an even number
            if (split.Length % 2 == 1)
            {
                throw GetConvertException(source, "System.Windows.Media.PointCollection");
            }

            if (split.Length == 0)
            {
                return "New Global.System.Windows.Media.PointCollection()";
            }

            var sb = new StringBuilder();

            sb.Append("New Global.System.Windows.Media.PointCollection() From ");
            sb.Append("{");

            sb.Append(ConvertPointHelper(split[0], split[1]));
            for (int i = 2; i < split.Length; i += 2)
            {
                sb.Append(", ")
                  .Append(ConvertPointHelper(split[i], split[i + 1]));
            }

            sb.Append("}");

            return sb.ToString();
        }

        private static string ConvertToTransform(XElement context, string source)
        {
            return $"New Global.System.Windows.Media.MatrixTransform({ConvertToMatrix(context, source)})";
        }

        private static string ConvertToCacheMode(XElement context, string source)
        {
            if (source.Equals("BitmapCache", StringComparison.OrdinalIgnoreCase))
            {
                return "New Global.System.Windows.Media.BitmapCache()";
            }

            throw GetConvertException(source, "System.Windows.Media.CacheMode");
        }

        private static string ConvertToCornerRadius(XElement context, string source)
        {
            string[] split = source.Split(_separators, StringSplitOptions.RemoveEmptyEntries);

            switch (split.Length)
            {
                case 1:
                    return $"New Global.System.Windows.CornerRadius({split[0]})";

                case 4:
                    return $"New Global.System.Windows.CornerRadius({split[0]}, {split[1]}, {split[2]}, {split[3]})";
            }

            throw GetConvertException(source, "System.Windows.CornerRadius");
        }

        private static string ConvertToDuration(XElement context, string source)
        {
            string stringValue = source.Trim();

            if (stringValue.Equals("Automatic", StringComparison.OrdinalIgnoreCase))
            {
                return "Global.System.Windows.Duration.Automatic";
            }
            else if (stringValue.Equals("Forever", StringComparison.OrdinalIgnoreCase))
            {
                return "Global.System.Windows.Duration.Forever";
            }
            else
            {
                return $"New Global.System.Windows.Duration({SystemTypesHelper.VisualBasic.ConvertFromInvariantString(stringValue, "system.timespan")})";
            }
        }

        private static string ConvertToFontWeight(XElement context, string source)
        {
            if (Enum.TryParse(source, true, out FontWeightsCode fontCode))
            {
                return $"Global.System.Windows.FontWeights.{fontCode}";
            }
            else if (ushort.TryParse(source, out ushort code))
            {
                string fontName = Enum.GetName(typeof(FontWeightsCode), code);
                if (fontName != null)
                {
                    return $"Global.System.Windows.FontWeights.{fontName}";
                }
            }

            throw GetConvertException(source, "System.Windows.FontWeight");
        }

        private static string ConvertToGridLength(XElement context, string source)
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
                unit = "Global.System.Windows.GridUnitType.Auto";
            }
            else if (stringValue.EndsWith("*"))
            {
                value = ReadDouble(stringValue, "1D");
                unit = "Global.System.Windows.GridUnitType.Star";
            }
            else
            {
                value = ReadDouble(stringValue, "0D");
                unit = "Global.System.Windows.GridUnitType.Pixel";
            }

            return $"New Global.System.Windows.GridLength({value}, {unit})";
        }

        private static string ConvertToPoint(XElement context, string source)
        {
            string[] split = source.Split(_separators, StringSplitOptions.RemoveEmptyEntries);

            if (split.Length == 2)
            {
                return ConvertPointHelper(split[0], split[1]);
            }

            throw GetConvertException(source, "System.Windows.Point");
        }

        private static string ConvertPointHelper(string x, string y)
        {
            return $"New Global.System.Windows.Point({x}, {y})";
        }

        private static string ConvertToPropertyPath(XElement context, string source)
        {
            return $"New Global.System.Windows.PropertyPath({Escape(source)})";
        }

        private static string ConvertToRect(XElement context, string source)
        {
            string[] split = source.Split(_separators, StringSplitOptions.RemoveEmptyEntries);

            if (split.Length == 4)
            {
                return $"New Global.System.Windows.Rect({split[0]}, {split[1]}, {split[2]}, {split[3]})";
            }

            throw GetConvertException(source, "System.Windows.Rect");
        }

        private static string ConvertToSize(XElement context, string source)
        {
            string[] split = source.Split(_separators, StringSplitOptions.RemoveEmptyEntries);

            if (split.Length == 2)
            {
                return $"New System.Windows.Size({split[0]}, {split[1]})";
            }

            throw GetConvertException(source, "System.Windows.Size");
        }

        private static string ConvertToThickness(XElement context, string source)
        {
            string[] split = source.Split(_separators, StringSplitOptions.RemoveEmptyEntries);

            switch (split.Length)
            {
                case 1:
                    return $"New Global.System.Windows.Thickness({split[0]})";

                case 2:
                    return $"New Global.System.Windows.Thickness({split[0]}, {split[1]}, {split[0]}, {split[1]})";

                case 4:
                    return $"New Global.System.Windows.Thickness({split[0]}, {split[1]}, {split[2]}, {split[3]})";
            }

            throw GetConvertException(source, "System.Windows.Thickness");
        }

        private static string ConvertToFontStretch(XElement context, string source)
        {
            string stringValue = source.Trim();
            if (stringValue.Equals("UltraCondensed", StringComparison.OrdinalIgnoreCase))
            {
                return "Global.System.Windows.FontStretches.UltraCondensed";
            }
            else if (stringValue.Equals("ExtraCondensed", StringComparison.OrdinalIgnoreCase))
            {
                return "Global.System.Windows.FontStretches.ExtraCondensed";
            }
            else if (stringValue.Equals("Condensed", StringComparison.OrdinalIgnoreCase))
            {
                return "Global.System.Windows.FontStretches.Condensed";
            }
            else if (stringValue.Equals("SemiCondensed", StringComparison.OrdinalIgnoreCase))
            {
                return "Global.System.Windows.FontStretches.SemiCondensed";
            }
            else if (stringValue.Equals("Normal", StringComparison.OrdinalIgnoreCase))
            {
                return "Global.System.Windows.FontStretches.Normal";
            }
            else if (stringValue.Equals("SemiExpanded", StringComparison.OrdinalIgnoreCase))
            {
                return "Global.System.Windows.FontStretches.SemiExpanded";
            }
            else if (stringValue.Equals("Expanded", StringComparison.OrdinalIgnoreCase))
            {
                return "Global.System.Windows.FontStretches.Expanded";
            }
            else if (stringValue.Equals("ExtraExpanded", StringComparison.OrdinalIgnoreCase))
            {
                return "Global.System.Windows.FontStretches.ExtraExpanded";
            }
            else if (stringValue.Equals("UltraExpanded", StringComparison.OrdinalIgnoreCase))
            {
                return "Global.System.Windows.FontStretches.UltraExpanded";
            }

            throw GetConvertException(source, "System.Windows.FontStretch");
        }

        private static string ConvertToFontStyle(XElement context, string source)
        {
            if (source.Equals("Normal", StringComparison.OrdinalIgnoreCase))
            {
                return "Global.System.Windows.FontStyles.Normal";
            }
            else if (source.Equals("Oblique", StringComparison.OrdinalIgnoreCase))
            {
                return "Global.System.Windows.FontStyles.Oblique";
            }
            else if (source.Equals("Italic", StringComparison.OrdinalIgnoreCase))
            {
                return "Global.System.Windows.FontStyles.Italic";
            }

            throw GetConvertException(source, "System.Windows.FontStyle");
        }

        private static string ConvertToTextDecorationCollection(XElement context, string source)
        {
            switch (source.Trim().ToLower())
            {
                case "underline":
                    return "Global.System.Windows.TextDecorations.Underline";
                case "strikethrough":
                    return "Global.System.Windows.TextDecorations.Strikethrough";
                case "overline":
                    return "Global.System.Windows.TextDecorations.OverLine";
                //case "baseline":
                //    return $"{textDecorationsTypeFullName}.Baseline";
                case "none":
                    return "Nothing";

                default:
                    throw GetConvertException(source, "System.Windows.TextDecorationCollection");
            }
        }

        private static string ConvertToImageSource(XElement context, string source)
        {
            string uriKind;
            if (source.Contains(":/"))
            {
                uriKind = "Global.System.UriKind.Absolute";
            }
            else
            {
                uriKind = "Global.System.UriKind.Relative";
            }

            return $"New Global.System.Windows.Media.Imaging.BitmapImage(New Global.System.Uri({Escape(source)}, {uriKind}))";
        }

        private static string ConvertToVector(XElement context, string source)
        {
            string[] split = source.Split(_separators, StringSplitOptions.RemoveEmptyEntries);

            if (split.Length == 2)
            {
                return $"New Global.System.Windows.Vector({split[0]}, {split[1]})";
            }

            throw GetConvertException(source, "System.Windows.Vector");
        }

        private string ConvertToRoutedEvent(XElement context, string source)
        {
            string eventName, namespaceName, typeName, assemblyName;

            int index = source.IndexOf('.');
            if (index >= 0)
            {
                eventName = source.Substring(index + 1);
                GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(
                    source.Substring(0, index), context, out namespaceName, out typeName, out assemblyName);
            }
            else
            {
                index = source.IndexOf(':');
                if (index >= 0)
                {
                    // WPF ignore everything before the ':'
                    eventName = source.Substring(index + 1);
                }
                else
                {
                    eventName = source;
                }

                XElement style = context;
                while (style is not null && !GeneratingCode.IsStyle(style, _assemblyName))
                {
                    style = style.Parent;
                }

                if (style is not null && style.Attribute("TargetType") is XAttribute targetType)
                {
                    GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(
                        targetType.Value, style, out namespaceName, out typeName, out assemblyName);
                }
                else
                {
                    namespaceName = KnownNamespaces.SystemWindows;
                    typeName = "FrameworkElement";
                    assemblyName = "OpenSilver";
                }
            }

            TypeDefinition ownerType = _inspector.GetTypeDefinition(namespaceName, typeName, assemblyName);
            string ownerTypeString = ownerType.ConvertToString(SupportedLanguage.VBNet);

            return $"{RuntimeHelperClass}.RoutedEventFromName(\"{eventName}\", GetType(Global.{ownerTypeString}))";
        }

        private static Exception GetConvertException(string value, string destinationTypeFullName)
        {
            return new XamlParseException($"Cannot convert '{value}' to '{destinationTypeFullName}'.");
        }

        private static string Escape(string s)
        {
            return string.Concat("\"", s.Replace("\"", "\"\""), "\"");
        }
    }
}
