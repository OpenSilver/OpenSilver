
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
using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace OpenSilver.Compiler;

internal sealed class CoreTypesConverterFS : CoreTypesConverter
{
    public const string RuntimeHelperClass = "global.OpenSilver.Internal.Xaml.RuntimeHelpers";
    private static readonly char[] _separators = [',', ' '];
    private static readonly char[] _repeatBehaviorConverterIterationCharacter = ['x', 'X'];

    private readonly AssembliesInspector _inspector;
    private readonly XamlNameParser _xamlNameParser;
    private readonly CommandConverter _commandConverter;

    public CoreTypesConverterFS(AssembliesInspector inspector, string assemblyName)
    {
        _inspector = inspector;
        _xamlNameParser = new XamlNameParser(assemblyName);
        _commandConverter = new CommandConverter(_inspector, TypeReferenceHelper.FSharp, _xamlNameParser);
    }

    public override string ConvertFromInvariantString(string source, string destinationType)
    {
        return $"{RuntimeHelperClass}.ConvertFromInvariantString<global.{destinationType}>({Escape(source)})";
    }

    public override string ConvertToCursor(XObject context, string source)
    {
        return $"global.System.Windows.Input.Cursors.{source}";
    }

    public override string ConvertToModifierKeys(XObject context, string source)
    {
        char[] separator = ['+'];

        string modifiersToken = source.Trim();

        // Empty token means there were no modifiers, exit early
        if (modifiersToken.Length == 0 || modifiersToken.Equals("None", StringComparison.OrdinalIgnoreCase))
        {
            return "global.System.Windows.Input.ModifierKeys.None";
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

            TypeDefinition modifierKeysType = _inspector.GetKnownTypeDefinition("System.Windows.Input", "ModifierKeys", "OpenSilver");
            return string.Join(" ||| ", _inspector.GetEnumValues(modifierKeysType, modifiersToken, true, true, context));
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
                throw GetConvertException(source, "System.Windows.Input.ModifierKeys", context);
            }

            if (sb.Length > 0)
            {
                sb.Append(" | ");
            }

            sb.Append(key);
        }

        if (sb.Length == 0)
        {
            return "global.System.Windows.Input.ModifierKeys.None";
        }

        return sb.ToString();

        static bool TryParseModifier(string source, out string modifier)
        {
            modifier = source switch
            {
                _ when source.Equals("Ctrl", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.ModifierKeys.Control",
                _ when source.Equals("Control", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.ModifierKeys.Control",
                _ when source.Equals("Win", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.ModifierKeys.Windows",
                _ when source.Equals("Windows", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.ModifierKeys.Windows",
                _ when source.Equals("Apple", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.ModifierKeys.Apple",
                _ when source.Equals("Alt", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.ModifierKeys.Alt",
                _ when source.Equals("Shift", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.ModifierKeys.Shift",
                _ => null,
            };

            return modifier is not null;
        }
    }

    public override string ConvertToKey(XObject context, string source)
    {
        string keyToken = source.Trim();

        if (keyToken.Length == 0)
        {
            return "global.System.Windows.Input.Key.None";
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
                return $"global.System.Windows.Input.Key.D{firstChar}";
            }
            else if (firstChar >= 'A' && firstChar <= 'Z')
            {
                return $"global.System.Windows.Input.Key.{firstChar}";
            }
            else
            {
                throw GetConvertException(source, "System.Windows.Input.Key", context);
            }
        }

        return keyToken switch
        {
            _ when keyToken.Equals("ENTER", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.Return",
            _ when keyToken.Equals("ESC", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.Escape",
            _ when keyToken.Equals("PGUP", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.PageUp",
            _ when keyToken.Equals("PGDN", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.PageDown",
            //_ when keyToken.Equals("PRTSC", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.PrintScreen",
            _ when keyToken.Equals("INS", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.Insert",
            _ when keyToken.Equals("DEL", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.Delete",
            _ when keyToken.Equals("WINDOWS", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.LWin",
            _ when keyToken.Equals("WIN", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.LWin",
            _ when keyToken.Equals("LEFTWINDOWS", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.LWin",
            _ when keyToken.Equals("RIGHTWINDOWS", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.RWin",
            _ when keyToken.Equals("APPS", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.Apps",
            _ when keyToken.Equals("APPLICATION", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.Apps",
            _ when keyToken.Equals("BREAK", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.Cancel",
            _ when keyToken.Equals("BACKSPACE", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.Back",
            _ when keyToken.Equals("BKSP", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.Back",
            _ when keyToken.Equals("BS", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.Back",
            _ when keyToken.Equals("SHIFT", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.Shift", // Key.LeftShift,
            _ when keyToken.Equals("LEFTSHIFT", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.Shift", // Key.LeftShift,
            //_ when keyToken.Equals("RIGHTSHIFT", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.RightShift",
            _ when keyToken.Equals("CONTROL", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.Ctrl", // Key.LeftCtrl,
            _ when keyToken.Equals("CTRL", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.Ctrl", // Key.LeftCtrl,
            _ when keyToken.Equals("LEFTCTRL", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.Ctrl", // Key.LeftCtrl,
            //_ when keyToken.Equals("RIGHTCTRL", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.RightCtrl",
            _ when keyToken.Equals("ALT", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.Alt", // Key.LeftAlt,
            _ when keyToken.Equals("LEFTALT", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.Alt", // Key.LeftAlt,
            //_ when keyToken.Equals("RIGHTALT", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.RightAlt",
            //_ when keyToken.Equals("SEMICOLON", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.OemSemicolon",
            //_ when keyToken.Equals("PLUS", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.OemPlus",
            //_ when keyToken.Equals("COMMA", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.OemComma",
            //_ when keyToken.Equals("MINUS", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.OemMinus",
            //_ when keyToken.Equals("PERIOD", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.OemPeriod",
            //_ when keyToken.Equals("QUESTION", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.OemQuestion",
            //_ when keyToken.Equals("TILDE", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.OemTilde",
            //_ when keyToken.Equals("OPENBRACKETS", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.OemOpenBrackets",
            //_ when keyToken.Equals("PIPE", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.OemPipe",
            //_ when keyToken.Equals("CLOSEBRACKETS", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.OemCloseBrackets",
            //_ when keyToken.Equals("QUOTES", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.OemQuotes",
            //_ when keyToken.Equals("BACKSLASH", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.OemBackslash",
            //_ when keyToken.Equals("FINISH", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.OemFinish",
            //_ when keyToken.Equals("ATTN", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.Attn",
            //_ when keyToken.Equals("CRSEL", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.CrSel",
            //_ when keyToken.Equals("EXSEL", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.ExSel",
            //_ when keyToken.Equals("ERASEEOF", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.EraseEof",
            //_ when keyToken.Equals("PLAY", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.Play",
            //_ when keyToken.Equals("ZOOM", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.Zoom",
            //_ when keyToken.Equals("PA1", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.Key.Pa1",
            _ => Parse(keyToken, _inspector) ?? throw GetConvertException(source, "System.Windows.Input.Key", context),
        };

        static string Parse(string source, AssembliesInspector inspector)
        {
            TypeDefinition keyType = inspector.GetKnownTypeDefinition("System.Windows.Input", "Key", "OpenSilver");
            return inspector.GetEnumValue(keyType, source, true, true);
        }
    }

    public override string ConvertToMouseAction(XObject context, string source)
    {
        string mouseActionToken = source.Trim();
        return mouseActionToken switch
        {
            _ when mouseActionToken == string.Empty => "global.System.Windows.Input.MouseAction.None",
            _ when mouseActionToken.Equals("None", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.MouseAction.None",
            _ when mouseActionToken.Equals("LeftClick", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.MouseAction.LeftClick",
            _ when mouseActionToken.Equals("RightClick", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.MouseAction.RightClick",
            _ when mouseActionToken.Equals("MiddleClick", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.MouseAction.MiddleClick",
            _ when mouseActionToken.Equals("WheelClick", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.MouseAction.WheelClick",
            _ when mouseActionToken.Equals("LeftDoubleClick", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.MouseAction.LeftDoubleClick",
            _ when mouseActionToken.Equals("RightDoubleClick", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.MouseAction.RightDoubleClick",
            _ when mouseActionToken.Equals("MiddleDoubleClick", StringComparison.OrdinalIgnoreCase) => "global.System.Windows.Input.MouseAction.MiddleDoubleClick",
            _ => throw new XamlParseException($"Unsupported MouseAction '{mouseActionToken}'.", context),
        };
    }

    public override string ConvertToKeyGesture(XObject context, string source)
    {
        const char MODIFIERS_DELIMITER = '+';
        const char DISPLAYSTRING_SEPARATOR = ',';

        string fullName = source.Trim();
        if (fullName.Length == 0)
        {
            return "new global.System.Windows.Input.KeyGesture(global.System.Windows.Input.Key.None)";
        }

        string keyToken;
        string modifiersToken;
        string displayString;

        // break apart display string
        int index = fullName.IndexOf(DISPLAYSTRING_SEPARATOR);
        if (index >= 0)
        {
            displayString = fullName.Substring(index + 1).Trim();
            fullName = fullName.Substring(0, index).Trim();
        }
        else
        {
            displayString = string.Empty;
        }

        // break apart key and modifiers
        index = fullName.LastIndexOf(MODIFIERS_DELIMITER);
        if (index >= 0)
        {
            // modifiers exists
            modifiersToken = fullName.Substring(0, index);
            keyToken = fullName.Substring(index + 1);
        }
        else
        {
            modifiersToken = string.Empty;
            keyToken = fullName;
        }

        string resultkey = ConvertToKey(context, keyToken);
        string modifiers = ConvertToModifierKeys(context, modifiersToken);
        return $"new global.System.Windows.Input.KeyGesture({resultkey}, {modifiers}, {Escape(displayString)})";
    }

    public override string ConvertToMouseGesture(XObject context, string source)
    {
        const char MODIFIERS_DELIMITER = '+';

        string fullName = source.Trim();

        if (fullName.Length == 0)
        {
            return "new global.System.Windows.Input.MouseGesture(global.System.Windows.Input.MouseAction.None, global.System.Windows.Input.ModifierKeys.None)";
        }

        // break apart LocalName and Prefix
        int offset = fullName.LastIndexOf(MODIFIERS_DELIMITER);
        if (offset >= 0)
        {
            // modifiers exists
            string modifiersToken = fullName.Substring(0, offset);
            string mouseActionToken = fullName.Substring(offset + 1);

            string mouseAction = ConvertToMouseAction(context, mouseActionToken);
            string modifierKeys = ConvertToModifierKeys(context, modifiersToken);
            return $"new global.System.Windows.Input.MouseGesture({mouseAction}, {modifierKeys})";
        }
        else
        {
            string mouseAction = ConvertToMouseAction(context, fullName);
            return $"new global.System.Windows.Input.MouseGesture({mouseAction})";
        }
    }

    public override string ConvertToCommand(XObject context, string source)
    {
        return _commandConverter.Convert(context, source);
    }

    public override string ConvertToKeyTime(XObject context, string source)
    {
        string stringValue = source.Trim();

        if (stringValue == "Paced")
        {
            throw new XamlParseException("The 'System.Windows.Media.Animation.KeyTime.Placed' property is not supported yet.", context);
        }
        else if (stringValue.Length > 0 &&
                 stringValue[stringValue.Length - 1] == '%')
        {
            throw new XamlParseException("Percentage values for 'System.Windows.Media.Animation.KeyTime' are not supported yet.", context);
        }
        else if (stringValue == "Uniform")
        {
            return "global.System.Windows.Media.Animation.KeyTime.Uniform";
        }
        else
        {
            string timeSpanValue = SystemTypesHelper.FSharp.ConvertToTimeSpan(stringValue);
            return $"global.System.Windows.Media.Animation.KeyTime.FromTimeSpan({timeSpanValue})";
        }
    }

    public override string ConvertToRepeatBehavior(XObject context, string source)
    {
        string stringValue = source.Trim();

        if (string.Equals(stringValue, "Forever", StringComparison.OrdinalIgnoreCase))
        {
            return "global.System.Windows.Media.Animation.RepeatBehavior.Forever";
        }
        else if (stringValue.Length > 0 &&
                 char.ToLowerInvariant(stringValue[stringValue.Length - 1]) == _repeatBehaviorConverterIterationCharacter[0])
        {
            string stringDoubleValue = stringValue.TrimEnd(_repeatBehaviorConverterIterationCharacter);

            return $"new global.System.Windows.Media.Animation.RepeatBehavior({SystemTypesHelper.FSharp.ConvertToDouble(stringDoubleValue)})";
        }

        string timeSpanValue = SystemTypesHelper.FSharp.ConvertToTimeSpan(stringValue);

        return $"new global.System.Windows.Media.Animation.RepeatBehavior({timeSpanValue})";
    }

    public override string ConvertToKeySpline(XObject context, string source)
    {
        if (string.IsNullOrEmpty(source))
        {
            return "new global.System.Windows.Media.Animation.KeySpline()";
        }

        string[] split = source.Split(_separators, StringSplitOptions.RemoveEmptyEntries);
        if (split.Length == 4)
        {
            return $"new global.System.Windows.Media.Animation.KeySpline({split[0]}, {split[1]}, {split[2]}, {split[3]})";
        }

        throw GetConvertException(source, "System.Windows.Media.Animation.KeySpline", context);
    }

    public override string ConvertToBrush(XObject context, string source)
    {
        return $"new global.System.Windows.Media.SolidColorBrush({ConvertToColor(context, source)})";
    }

    public override string ConvertToColor(XObject context, string source)
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

        static int ParseHexChar(char c, XObject context)
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
            throw new XamlParseException("Token is not valid.", context);
        }

        static string ParseHexColor(string trimmedColor, XObject context)
        {
            int a, r, g, b;
            a = 255;

            if (trimmedColor.Length > 7)
            {
                a = ParseHexChar(trimmedColor[1], context) * 16 + ParseHexChar(trimmedColor[2], context);
                r = ParseHexChar(trimmedColor[3], context) * 16 + ParseHexChar(trimmedColor[4], context);
                g = ParseHexChar(trimmedColor[5], context) * 16 + ParseHexChar(trimmedColor[6], context);
                b = ParseHexChar(trimmedColor[7], context) * 16 + ParseHexChar(trimmedColor[8], context);
            }
            else if (trimmedColor.Length > 5)
            {
                r = ParseHexChar(trimmedColor[1], context) * 16 + ParseHexChar(trimmedColor[2], context);
                g = ParseHexChar(trimmedColor[3], context) * 16 + ParseHexChar(trimmedColor[4], context);
                b = ParseHexChar(trimmedColor[5], context) * 16 + ParseHexChar(trimmedColor[6], context);
            }
            else if (trimmedColor.Length > 4)
            {
                a = ParseHexChar(trimmedColor[1], context);
                a = a + a * 16;
                r = ParseHexChar(trimmedColor[2], context);
                r = r + r * 16;
                g = ParseHexChar(trimmedColor[3], context);
                g = g + g * 16;
                b = ParseHexChar(trimmedColor[4], context);
                b = b + b * 16;
            }
            else
            {
                r = ParseHexChar(trimmedColor[1], context);
                r = r + r * 16;
                g = ParseHexChar(trimmedColor[2], context);
                g = g + g * 16;
                b = ParseHexChar(trimmedColor[3], context);
                b = b + b * 16;
            }

            string A = a.ToString(CultureInfo.InvariantCulture);
            string R = r.ToString(CultureInfo.InvariantCulture);
            string G = g.ToString(CultureInfo.InvariantCulture);
            string B = b.ToString(CultureInfo.InvariantCulture);
            return $"global.System.Windows.Media.Color.FromArgb((byte){A}, (byte){R}, (byte){G}, (byte){B})";
        }

        static string ParseScRgbColor(string trimmedColor, XObject context)
        {
            if (!trimmedColor.StartsWith("sc#", StringComparison.Ordinal))
            {
                throw new XamlParseException("Token is not valid.", context);
            }

            string tokens = trimmedColor.Substring(3, trimmedColor.Length - 3);

            string[] split = tokens.Split(_separators, StringSplitOptions.RemoveEmptyEntries);

            if (split.Length == 3)
            {
                string r = Convert.ToSingle(split[0], CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
                string g = Convert.ToSingle(split[1], CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
                string b = Convert.ToSingle(split[2], CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
                return $"global.System.Windows.Media.Color.FromScRgb(1F, {r}F, {g}F, {b}F)";
            }
            else if (split.Length == 4)
            {
                string a = Convert.ToSingle(split[0], CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
                string r = Convert.ToSingle(split[1], CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
                string g = Convert.ToSingle(split[2], CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
                string b = Convert.ToSingle(split[3], CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
                return $"global.System.Windows.Media.Color.FromScRgb({a}F, {r}F, {g}F, {b}F)";
            }

            throw new XamlParseException("Token is not valid.", context);
        }

        static string ParseColor(string colorString, XObject context)
        {
            string trimmedColor = MatchColor(
                colorString, out bool isPossibleKnowColor, out bool isNumericColor, out bool isScRgbColor
            );

            //Is it a number?
            if (isNumericColor)
            {
                return ParseHexColor(trimmedColor, context);
            }
            else if (isScRgbColor)
            {
                return ParseScRgbColor(trimmedColor, context);
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
                    return $"global.System.Windows.Media.Color.FromArgb((byte){a}, (byte){r}, (byte){g}, (byte){b})";
                }
            }

            throw GetConvertException(colorString, "System.Windows.Media.Color", context);
        }

        return ParseColor(source, context);
    }

    public override string ConvertToDoubleCollection(XObject context, string source)
    {
        string[] split = source.Split(_separators, StringSplitOptions.RemoveEmptyEntries);

        var sb = new StringBuilder();

        sb.AppendLine();
        sb.AppendLine("                let collection = global.System.Windows.Media.DoubleCollection()");
        if (split != null && split.Length > 0)
        {
            foreach (string d in split)
            {
                sb.AppendLine($"                collection.Add({d})");
            }
        }

        sb.Append("                collection");

        return sb.ToString();
    }

    public override string ConvertToFontFamily(XObject context, string source)
    {
        string fontName = Escape(source.Trim());

        return $"new global.System.Windows.Media.FontFamily({fontName})";
    }

    public override string ConvertToGeometry(XObject context, string source)
    {
        return $"global.System.Windows.Media.Geometry.Parse({Escape(source)})";
    }

    public override string ConvertToPathFigureCollection(XObject context, string source)
    {
        return $"global.System.Windows.Media.PathFigureCollection.Parse({Escape(source)})";
    }

    public override string ConvertToMatrix(XObject context, string source)
    {
        if (source == "Identity")
        {
            return "global.System.Windows.Media.Matrix.Identity";
        }

        string[] split = source.Split(_separators, StringSplitOptions.RemoveEmptyEntries);

        if (split.Length == 6)
        {
            return $"new global.System.Windows.Media.Matrix({split[0]}, {split[1]}, {split[2]}, {split[3]}, {split[4]}, {split[5]})";
        }

        throw GetConvertException(source, "System.Windows.Media.Matrix", context);
    }

    public override string ConvertToPointCollection(XObject context, string source)
    {
        string[] split = source.Split(_separators, StringSplitOptions.RemoveEmptyEntries);

        // Points count needs to be an even number
        if (split.Length % 2 == 1)
        {
            throw GetConvertException(source, "System.Windows.Media.PointCollection", context);
        }

        var sb = new StringBuilder();

        sb.AppendLine();
        sb.AppendLine("                let collection = global.System.Windows.Media.PointCollection()");
        for (int i = 0; i < split.Length; i += 2)
        {
            sb.AppendLine($"                collection.Add({ConvertPointHelper(split[i], split[i + 1])})");
        }

        sb.Append("                collection");

        return sb.ToString();
    }

    public override string ConvertToTransform(XObject context, string source)
    {
        return $"new global.System.Windows.Media.MatrixTransform({ConvertToMatrix(context, source)})";
    }

    public override string ConvertToCacheMode(XObject context, string source)
    {
        if (source.Equals("BitmapCache", StringComparison.OrdinalIgnoreCase))
        {
            return "new global.System.Windows.Media.BitmapCache()";
        }

        throw GetConvertException(source, "System.Windows.Media.CacheMode", context);
    }

    public override string ConvertToMatrix3D(XObject context, string source)
    {
        if (source == "Identity")
        {
            return "global.System.Windows.Media.Media3D.Matrix3D.Identity";
        }

        string[] split = source.Split(_separators, StringSplitOptions.RemoveEmptyEntries);

        if (split.Length == 16)
        {
            return $"new global.System.Windows.Media.Media3D.Matrix3D({split[0]}, {split[1]}, {split[2]}, {split[3]}, {split[4]}, {split[5]}, {split[6]}, {split[7]}, {split[8]}, {split[9]}, {split[10]}, {split[11]}, {split[12]}, {split[13]}, {split[14]}, {split[15]})";
        }

        throw GetConvertException(source, "System.Windows.Media.Media3D.Matrix3D", context);
    }

    public override string ConvertToXmlLanguage(XObject context, string source)
    {
        return $"global.System.Windows.Markup.XmlLanguage.GetLanguage({Escape(source)})";
    }

    public override string ConvertToCornerRadius(XObject context, string source)
    {
        string[] split = source.Split(_separators, StringSplitOptions.RemoveEmptyEntries);

        switch (split.Length)
        {
            case 1:
                return $"new global.System.Windows.CornerRadius({split[0]})";

            case 4:
                return $"new global.System.Windows.CornerRadius({split[0]}, {split[1]}, {split[2]}, {split[3]})";
        }

        throw GetConvertException(source, "System.Windows.CornerRadius", context);
    }

    public override string ConvertToDuration(XObject context, string source)
    {
        string stringValue = source.Trim();

        if (stringValue.Equals("Automatic", StringComparison.OrdinalIgnoreCase))
        {
            return "global.System.Windows.Duration.Automatic";
        }
        else if (stringValue.Equals("Forever", StringComparison.OrdinalIgnoreCase))
        {
            return "global.System.Windows.Duration.Forever";
        }
        else
        {
            return $"new global.System.Windows.Duration({SystemTypesHelper.FSharp.ConvertToTimeSpan(stringValue)})";
        }
    }

    public override string ConvertToFontWeight(XObject context, string source)
    {
        if (Enum.TryParse(source, true, out FontWeightsCode fontCode))
        {
            return $"global.System.Windows.FontWeights.{fontCode}";
        }
        else if (ushort.TryParse(source, out ushort code))
        {
            string fontName = Enum.GetName(typeof(FontWeightsCode), code);
            if (fontName != null)
            {
                return $"global.System.Windows.FontWeights.{fontName}";
            }
        }

        throw GetConvertException(source, "System.Windows.FontWeight", context);
    }

    public override string ConvertToGridLength(XObject context, string source)
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
                return "0.0";
            }

            return seq.Substring(0, i);
        }

        string value;
        string unit;

        string stringValue = source.Trim().ToLower();
        if (stringValue == "auto")
        {
            value = "1.0";
            unit = "global.System.Windows.GridUnitType.Auto";
        }
        else if (stringValue.EndsWith("*"))
        {
            value = ReadDouble(stringValue, "1.0");
            unit = "global.System.Windows.GridUnitType.Star";
        }
        else
        {
            value = ReadDouble(stringValue, "0.0");
            unit = "global.System.Windows.GridUnitType.Pixel";
        }

        return $"new global.System.Windows.GridLength({value}, {unit})";
    }

    public override string ConvertToPoint(XObject context, string source)
    {
        string[] split = source.Split(_separators, StringSplitOptions.RemoveEmptyEntries);

        if (split.Length == 2)
        {
            // F# doesn't support "new Point(.5, .5)"
            if (split[0].StartsWith("."))
                split[0] = "0" + split[0];
            if (split[1].StartsWith("."))
                split[1] = "0" + split[1];
            return ConvertPointHelper(split[0], split[1]);
        }

        throw GetConvertException(source, "System.Windows.Point", context);
    }

    private static string ConvertPointHelper(string x, string y)
    {
        return $"new global.System.Windows.Point({x}, {y})";
    }

    public override string ConvertToPropertyPath(XObject context, string source)
    {
        return $"new global.System.Windows.PropertyPath({Escape(source)})";
    }

    public override string ConvertToRect(XObject context, string source)
    {
        string[] split = source.Split(_separators, StringSplitOptions.RemoveEmptyEntries);

        if (split.Length == 4)
        {
            return $"new global.System.Windows.Rect({split[0]}, {split[1]}, {split[2]}, {split[3]})";
        }

        throw GetConvertException(source, "System.Windows.Rect", context);
    }

    public override string ConvertToSize(XObject context, string source)
    {
        string[] split = source.Split(_separators, StringSplitOptions.RemoveEmptyEntries);

        if (split.Length == 2)
        {
            return $"new global.System.Windows.Size({split[0]}, {split[1]})";
        }

        throw GetConvertException(source, "System.Windows.Size", context);
    }

    public override string ConvertToThickness(XObject context, string source)
    {
        string[] split = source.Split(_separators, StringSplitOptions.RemoveEmptyEntries);

        switch (split.Length)
        {
            case 1:
                return $"new global.System.Windows.Thickness({split[0]})";

            case 2:
                return $"new global.System.Windows.Thickness({split[0]}, {split[1]}, {split[0]}, {split[1]})";

            case 4:
                return $"new global.System.Windows.Thickness({split[0]}, {split[1]}, {split[2]}, {split[3]})";
        }

        throw GetConvertException(source, "System.Windows.Thickness", context);
    }

    public override string ConvertToFontStretch(XObject context, string source)
    {
        string stringValue = source.Trim();
        if (stringValue.Equals("UltraCondensed", StringComparison.OrdinalIgnoreCase))
        {
            return "global.System.Windows.FontStretches.UltraCondensed";
        }
        else if (stringValue.Equals("ExtraCondensed", StringComparison.OrdinalIgnoreCase))
        {
            return "global.System.Windows.FontStretches.ExtraCondensed";
        }
        else if (stringValue.Equals("Condensed", StringComparison.OrdinalIgnoreCase))
        {
            return "global.System.Windows.FontStretches.Condensed";
        }
        else if (stringValue.Equals("SemiCondensed", StringComparison.OrdinalIgnoreCase))
        {
            return "global.System.Windows.FontStretches.SemiCondensed";
        }
        else if (stringValue.Equals("Normal", StringComparison.OrdinalIgnoreCase))
        {
            return "global.System.Windows.FontStretches.Normal";
        }
        else if (stringValue.Equals("SemiExpanded", StringComparison.OrdinalIgnoreCase))
        {
            return "global.System.Windows.FontStretches.SemiExpanded";
        }
        else if (stringValue.Equals("Expanded", StringComparison.OrdinalIgnoreCase))
        {
            return "global.System.Windows.FontStretches.Expanded";
        }
        else if (stringValue.Equals("ExtraExpanded", StringComparison.OrdinalIgnoreCase))
        {
            return "global.System.Windows.FontStretches.ExtraExpanded";
        }
        else if (stringValue.Equals("UltraExpanded", StringComparison.OrdinalIgnoreCase))
        {
            return "global.System.Windows.FontStretches.UltraExpanded";
        }

        throw GetConvertException(source, "System.Windows.FontStretch", context);
    }

    public override string ConvertToFontStyle(XObject context, string source)
    {
        if (source.Equals("Normal", StringComparison.OrdinalIgnoreCase))
        {
            return "global.System.Windows.FontStyles.Normal";
        }
        else if (source.Equals("Oblique", StringComparison.OrdinalIgnoreCase))
        {
            return "global.System.Windows.FontStyles.Oblique";
        }
        else if (source.Equals("Italic", StringComparison.OrdinalIgnoreCase))
        {
            return "global.System.Windows.FontStyles.Italic";
        }

        throw GetConvertException(source, "System.Windows.FontStyle", context);
    }

    public override string ConvertToTextDecorationCollection(XObject context, string source)
    {
        switch (source.Trim().ToLower())
        {
            case "underline":
                return "global.System.Windows.TextDecorations.Underline";
            case "strikethrough":
                return "global.System.Windows.TextDecorations.Strikethrough";
            case "overline":
                return "global.System.Windows.TextDecorations.OverLine";
            //case "baseline":
            //    return $"{textDecorationsTypeFullName}.Baseline";
            case "none":
                return "null";

            default:
                throw GetConvertException(source, "System.Windows.TextDecorationCollection", context);
        }
    }

    public override string ConvertToImageSource(XObject context, string source)
    {
        string uriKind;
        if (source.Contains(":/"))
        {
            uriKind = "global.System.UriKind.Absolute";
        }
        else
        {
            uriKind = "global.System.UriKind.Relative";
        }

        return $"new global.System.Windows.Media.Imaging.BitmapImage(new global.System.Uri({Escape(source)}, {uriKind}))";
    }

    public override string ConvertToVector(XObject context, string source)
    {
        string[] split = source.Split(_separators, StringSplitOptions.RemoveEmptyEntries);

        if (split.Length == 2)
        {
            // F# doesn't support "new Vector(.5, .5)"
            if (split[0].StartsWith("."))
            {
                split[0] = "0" + split[0];
            }
            if (split[1].StartsWith("."))
            {
                split[1] = "0" + split[1];
            }

            return $"new global.System.Windows.Vector({split[0]}, {split[1]})";
        }

        throw GetConvertException(source, "System.Windows.Vector", context);
    }

    public override string ConvertToRoutedEvent(XObject context, string source)
    {
        string eventName, namespaceName, typeName, assemblyName;
        IXmlLineInfo lineInfo = context;

        int index = source.IndexOf('.');
        if (index >= 0)
        {
            eventName = source.Substring(index + 1);
            _xamlNameParser.GetClrNamespaceAndLocalName(
                source.Substring(0, index), GetClosestXElement(context), out namespaceName, out typeName, out assemblyName);
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

            XElement style = null;

            for (XElement element = GetClosestXElement(context); element is not null; element = element.Parent)
            {
                if (XamlParser.IsMemberNode(element))
                {
                    continue;
                }

                if (IsStyle(element, _inspector, _xamlNameParser))
                {
                    style = element;
                    break;
                }
            }

            if (style is not null && style.Attribute("TargetType") is XAttribute targetType)
            {
                lineInfo = targetType;

                _xamlNameParser.GetClrNamespaceAndLocalName(
                    targetType.Value, style, out namespaceName, out typeName, out assemblyName);
            }
            else
            {
                namespaceName = KnownNamespaces.SystemWindows;
                typeName = "FrameworkElement";
                assemblyName = "OpenSilver";
            }
        }

        TypeDefinition ownerType = _inspector.GetTypeDefinition(namespaceName, typeName, assemblyName, lineInfo);
        string ownerTypeString = TypeReferenceHelper.FSharp.ConvertToString(ownerType);

        return $"{RuntimeHelperClass}.RoutedEventFromName(\"{eventName}\", typeof<global.{ownerTypeString}>)";

        static bool IsStyle(XElement element, AssembliesInspector inspector, XamlNameParser xamlNameParser)
        {
            xamlNameParser.GetClrNamespaceAndLocalName(element.Name,
                out string namespaceName, out string typeName, out string assemblyName);

            TypeDefinition type = inspector.GetTypeDefinition(namespaceName, typeName, assemblyName, element);

            return inspector.IsStyle(type);
        }
    }

    public override string ConvertToResponsiveThreshold(XObject context, string source)
    {
        string[] split = source.Split(_separators, StringSplitOptions.RemoveEmptyEntries);

        return split.Length switch
        {
            1 => $"new global.System.Windows.ResponsiveThreshold({split[0]})",
            2 => $"new global.System.Windows.ResponsiveThreshold({split[0]}, {split[1]})",
            _ => throw GetConvertException(source, "System.Windows.ResponsiveThreshold", context),
        };
    }

    public override string ConvertToRowDefinitionCollection(XObject context, string source)
    {
        string[] split = source.Split(_separators, StringSplitOptions.RemoveEmptyEntries);

        var sb = new StringBuilder();

        sb.AppendLine();
        sb.AppendLine("                let collection = global.System.Windows.Controls.RowDefinitionCollection()");
        if (split != null && split.Length > 0)
        {
            for (int i = 0; i < split.Length; i++)
            {
                string value = split[i];
                sb.AppendLine($"                let row{i} = global.System.Windows.Controls.RowDefinition()");
                sb.AppendLine($"                row{i}.Height <- {ConvertToGridLength(context, value)}");
                sb.AppendLine($"                collection.Add(row{i})");
            }
        }

        sb.Append("                collection");

        return sb.ToString();
    }

    public override string ConvertToColumnDefinitionCollection(XObject context, string source)
    {
        string[] split = source.Split(_separators, StringSplitOptions.RemoveEmptyEntries);

        var sb = new StringBuilder();

        sb.AppendLine();
        sb.AppendLine("                let collection = global.System.Windows.Controls.ColumnDefinitionCollection()");
        if (split != null && split.Length > 0)
        {
            for (int i = 0; i < split.Length; i++)
            {
                string value = split[i];
                sb.AppendLine($"                let column{i} = global.System.Windows.Controls.ColumnDefinition()");
                sb.AppendLine($"                column{i}.Width <- {ConvertToGridLength(context, value)}");
                sb.AppendLine($"                collection.Add(column{i})");
            }
        }

        sb.Append("                collection");

        return sb.ToString();
    }

    private static string Escape(string s)
    {
        return string.Concat("@\"", s.Replace("\"", "\"\""), "\"");
    }
}
