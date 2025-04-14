
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

using System.ComponentModel;
using System.Globalization;
using OpenSilver.Internal;

namespace System.Windows.Input;

/// <summary>
/// Converts a <see cref="Key"/> object to and from other types.
/// </summary>
public class KeyConverter : TypeConverter
{
    /// <summary>
    /// Determines whether an object of the specified type can be converted to an instance of <see cref="Key"/>, using the specified context.
    /// </summary>
    /// <param name="context">
    /// A format context that provides information about the environment from which this converter is being invoked.
    /// </param>
    /// <param name="sourceType">
    /// The type being evaluated for conversion.
    /// </param>
    /// <returns>
    /// true if sourceType is of type <see cref="string"/>; otherwise, false.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        // We can only handle string
        return sourceType == typeof(string);
    }

    /// <summary>
    /// Determines whether an instance of <see cref="Key"/> can be converted to the specified type, using the specified context.
    /// </summary>
    /// <param name="context">
    /// A format context that provides information about the environment from which this converter is being invoked.
    /// </param>
    /// <param name="destinationType">
    /// The type being evaluated for conversion.
    /// </param>
    /// <returns>
    /// true if destinationType is of type <see cref="string"/>; otherwise, false.
    /// </returns>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
        // We can convert to a string
        if (destinationType != typeof(string))
        {
            return false;
        }

        // When invoked by the serialization engine we can convert to string only for known type
        if (context is null || context.Instance is null)
        {
            return false;
        }

        return IsDefinedKey((Key)context.Instance);
    }

    /// <summary>
    /// Attempts to convert the specified object to a <see cref="Key"/>, using the specified context.
    /// </summary>
    /// <param name="context">
    /// A format context that provides information about the environment from which this converter is being invoked.
    /// </param>
    /// <param name="culture">
    /// Culture specific information.
    /// </param>
    /// <param name="source">
    /// The object to convert.
    /// </param>
    /// <returns>
    /// The converted object.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// source cannot be converted.
    /// </exception>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object source)
    {
        if (source is not string stringSource)
        {
            throw GetConvertFromException(source);
        }

        return GetKeyFromString(stringSource);
    }

    internal static Key GetKeyFromString(string keyToken)
    {
        keyToken = keyToken.Trim();

        if (keyToken.Length == 0)
        {
            return Key.None;
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
                return Key.D0 + firstChar - '0';
            }
            else if (firstChar >= 'A' && firstChar <= 'Z')
            {
                return Key.A + firstChar - 'A';
            }
            else
            {
                throw new ArgumentException(string.Format(Strings.CannotConvertStringToType, keyToken, typeof(Key)));
            }
        }

        return keyToken switch
        {
            _ when keyToken.Equals("ENTER", StringComparison.OrdinalIgnoreCase) => Key.Return,
            _ when keyToken.Equals("ESC", StringComparison.OrdinalIgnoreCase) => Key.Escape,
            _ when keyToken.Equals("PGUP", StringComparison.OrdinalIgnoreCase) => Key.PageUp,
            _ when keyToken.Equals("PGDN", StringComparison.OrdinalIgnoreCase) => Key.PageDown,
            //_ when keyToken.Equals("PRTSC", StringComparison.OrdinalIgnoreCase) => Key.PrintScreen,
            _ when keyToken.Equals("INS", StringComparison.OrdinalIgnoreCase) => Key.Insert,
            _ when keyToken.Equals("DEL", StringComparison.OrdinalIgnoreCase) => Key.Delete,
            _ when keyToken.Equals("WINDOWS", StringComparison.OrdinalIgnoreCase) => Key.LWin,
            _ when keyToken.Equals("WIN", StringComparison.OrdinalIgnoreCase) => Key.LWin,
            _ when keyToken.Equals("LEFTWINDOWS", StringComparison.OrdinalIgnoreCase) => Key.LWin,
            _ when keyToken.Equals("RIGHTWINDOWS", StringComparison.OrdinalIgnoreCase) => Key.RWin,
            _ when keyToken.Equals("APPS", StringComparison.OrdinalIgnoreCase) => Key.Apps,
            _ when keyToken.Equals("APPLICATION", StringComparison.OrdinalIgnoreCase) => Key.Apps,
            _ when keyToken.Equals("BREAK", StringComparison.OrdinalIgnoreCase) => Key.Cancel,
            _ when keyToken.Equals("BACKSPACE", StringComparison.OrdinalIgnoreCase) => Key.Back,
            _ when keyToken.Equals("BKSP", StringComparison.OrdinalIgnoreCase) => Key.Back,
            _ when keyToken.Equals("BS", StringComparison.OrdinalIgnoreCase) => Key.Back,
            _ when keyToken.Equals("SHIFT", StringComparison.OrdinalIgnoreCase) => Key.Shift, // Key.LeftShift,
            _ when keyToken.Equals("LEFTSHIFT", StringComparison.OrdinalIgnoreCase) => Key.Shift, // Key.LeftShift,
            //_ when keyToken.Equals("RIGHTSHIFT", StringComparison.OrdinalIgnoreCase) => Key.RightShift,
            _ when keyToken.Equals("CONTROL", StringComparison.OrdinalIgnoreCase) => Key.Ctrl, // Key.LeftCtrl,
            _ when keyToken.Equals("CTRL", StringComparison.OrdinalIgnoreCase) => Key.Ctrl, // Key.LeftCtrl,
            _ when keyToken.Equals("LEFTCTRL", StringComparison.OrdinalIgnoreCase) => Key.Ctrl, // Key.LeftCtrl,
            //_ when keyToken.Equals("RIGHTCTRL", StringComparison.OrdinalIgnoreCase) => Key.RightCtrl,
            _ when keyToken.Equals("ALT", StringComparison.OrdinalIgnoreCase) => Key.Alt, // Key.LeftAlt,
            _ when keyToken.Equals("LEFTALT", StringComparison.OrdinalIgnoreCase) => Key.Alt, // Key.LeftAlt,
            //_ when keyToken.Equals("RIGHTALT", StringComparison.OrdinalIgnoreCase) => Key.RightAlt,
            //_ when keyToken.Equals("SEMICOLON", StringComparison.OrdinalIgnoreCase) => Key.OemSemicolon,
            //_ when keyToken.Equals("PLUS", StringComparison.OrdinalIgnoreCase) => Key.OemPlus,
            //_ when keyToken.Equals("COMMA", StringComparison.OrdinalIgnoreCase) => Key.OemComma,
            //_ when keyToken.Equals("MINUS", StringComparison.OrdinalIgnoreCase) => Key.OemMinus,
            //_ when keyToken.Equals("PERIOD", StringComparison.OrdinalIgnoreCase) => Key.OemPeriod,
            //_ when keyToken.Equals("QUESTION", StringComparison.OrdinalIgnoreCase) => Key.OemQuestion,
            //_ when keyToken.Equals("TILDE", StringComparison.OrdinalIgnoreCase) => Key.OemTilde,
            //_ when keyToken.Equals("OPENBRACKETS", StringComparison.OrdinalIgnoreCase) => Key.OemOpenBrackets,
            //_ when keyToken.Equals("PIPE", StringComparison.OrdinalIgnoreCase) => Key.OemPipe,
            //_ when keyToken.Equals("CLOSEBRACKETS", StringComparison.OrdinalIgnoreCase) => Key.OemCloseBrackets,
            //_ when keyToken.Equals("QUOTES", StringComparison.OrdinalIgnoreCase) => Key.OemQuotes,
            //_ when keyToken.Equals("BACKSLASH", StringComparison.OrdinalIgnoreCase) => Key.OemBackslash,
            //_ when keyToken.Equals("FINISH", StringComparison.OrdinalIgnoreCase) => Key.OemFinish,
            //_ when keyToken.Equals("ATTN", StringComparison.OrdinalIgnoreCase) => Key.Attn,
            //_ when keyToken.Equals("CRSEL", StringComparison.OrdinalIgnoreCase) => Key.CrSel,
            //_ when keyToken.Equals("EXSEL", StringComparison.OrdinalIgnoreCase) => Key.ExSel,
            //_ when keyToken.Equals("ERASEEOF", StringComparison.OrdinalIgnoreCase) => Key.EraseEof,
            //_ when keyToken.Equals("PLAY", StringComparison.OrdinalIgnoreCase) => Key.Play,
            //_ when keyToken.Equals("ZOOM", StringComparison.OrdinalIgnoreCase) => Key.Zoom,
            //_ when keyToken.Equals("PA1", StringComparison.OrdinalIgnoreCase) => Key.Pa1,
            _ => (Key)Enum.Parse(typeof(Key), keyToken, true),
        };
    }

    /// <summary>
    /// Attempts to convert a <see cref="Key"/> to the specified type, using the specified context.
    /// </summary>
    /// <param name="context">
    /// A format context that provides information about the environment from which this converter is being invoked.
    /// </param>
    /// <param name="culture">
    /// Culture specific information.
    /// </param>
    /// <param name="value">
    /// The object to convert.
    /// </param>
    /// <param name="destinationType">
    /// The type to convert the object to.
    /// </param>
    /// <returns>
    /// The converted object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="destinationType"/> is null.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// <paramref name="value"/> cannot be converted to <paramref name="destinationType"/>.
    /// </exception>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        if (destinationType is null)
        {
            throw new ArgumentNullException(nameof(destinationType));
        }

        if (value is null || destinationType != typeof(string))
        {
            throw GetConvertToException(value, destinationType);
        }

        Key key = (Key)value;
        return ToString(key) ?? throw GetConvertToException(value, destinationType);
    }

    internal static string ToString(Key key)
    {
        return key switch
        {
            Key.None => string.Empty,
            // This is a fast path for common keys before resort to Enum<Key>.ToString()
            >= Key.D0 and <= Key.D9 => char.ToString((char)(key - Key.D0 + '0')),
            >= Key.A and <= Key.Z => char.ToString((char)(key - Key.A + 'A')),
            // We format some keys differently than defined in the enum
            Key.Back => "Backspace",
            //Key.LineFeed => "Clear",
            Key.Escape => "Esc",
            Key.Return => "Return",
            // We will add some heavily used interned strings (F10-F12)
            Key.F10 => "F10",
            Key.F11 => "F11",
            Key.F12 => "F12",
            // Last resort, use Enum<Key>.ToString() if the range is defined
            _ when IsDefinedKey(key) => key.ToString(),
            // Everything else failed, we throw an exception
            _ => null,
        };
    }

    internal static bool IsDefinedKey(Key key) => Enum.IsDefined(typeof(Key), key);
}