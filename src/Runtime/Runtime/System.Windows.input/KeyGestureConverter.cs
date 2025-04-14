
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
using System.Diagnostics;
using System.Globalization;

namespace System.Windows.Input;

/// <summary>
/// Converts a <see cref="KeyGesture"/> object to and from other types.
/// </summary>
public class KeyGestureConverter : TypeConverter
{
    private const char MODIFIERS_DELIMITER = '+';
    internal const char DISPLAYSTRING_SEPARATOR = ',';

    /// <summary>
    /// Determines whether an object of the specified type can be converted to an instance of <see cref="KeyGesture"/>,
    /// using the specified context.
    /// </summary>
    /// <param name="context">
    /// A format context that provides information about the environment from which this converter is being invoked.
    /// </param>
    /// <param name="sourceType">
    /// The type being evaluated for conversion.
    /// </param>
    /// <returns>
    /// true if sourceType is type <see cref="string"/>; otherwise, false.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        // We can only handle string.
        return sourceType == typeof(string);
    }

    /// <summary>
    /// Determines whether an instance of <see cref="KeyGesture"/> can be converted to the specified type, using the 
    /// specified context.
    /// </summary>
    /// <param name="context">
    /// A format context that provides information about the environment from which this converter is being invoked.
    /// </param>
    /// <param name="destinationType">
    /// The type being evaluated for conversion.
    /// </param>
    /// <returns>
    /// true if destinationType is type <see cref="string"/>; otherwise, false.
    /// </returns>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
        // We can convert to an InstanceDescriptor or to a string.
        if (destinationType == typeof(string))
        {
            // When invoked by the serialization engine we can convert to string only for known type
            if (context is not null && context.Instance is not null)
            {
                if (context.Instance is KeyGesture keyGesture)
                {
                    return ModifierKeysConverter.IsDefinedModifierKeys(keyGesture.Modifiers) && KeyConverter.IsDefinedKey(keyGesture.Key);
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Attempts to convert the specified object to a <see cref="KeyGesture"/>, using the specified context.
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
    /// <paramref name="source"/> cannot be converted.
    /// </exception>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object source)
    {
        if (source is string fullName)
        {
            return FromString(context, culture, fullName);
        }
        throw GetConvertFromException(source);
    }

    internal static KeyGesture FromString(ITypeDescriptorContext context, CultureInfo culture, string source)
    {
        Debug.Assert(source is not null);

        string fullName = source.Trim();
        if (fullName.Length == 0)
        {
            return new KeyGesture(Key.None);
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

        Key resultkey = KeyConverter.GetKeyFromString(keyToken);
        ModifierKeys modifiers = ModifierKeysConverter.FromString(context, culture, modifiersToken);
        return new KeyGesture(resultkey, modifiers, displayString);
    }

    /// <summary>
    /// Attempts to convert a <see cref="KeyGesture"/> to the specified type, using the specified context.
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
    /// The converted object, or an empty string if value is null.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="destinationType"/> is null.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// <paramref name="value"/> cannot be converted.
    /// </exception>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        if (destinationType is null)
        {
            throw new ArgumentNullException(nameof(destinationType));
        }

        if (destinationType == typeof(string))
        {
            if (value is null)
            {
                return string.Empty;
            }

            if (value is KeyGesture keyGesture)
            {
                return ToString(keyGesture);
            }
        }
        throw GetConvertToException(value, destinationType);
    }

    internal static string ToString(KeyGesture keyGesture)
    {
        Debug.Assert(keyGesture is not null);

        if (keyGesture.Key == Key.None)
        {
            return string.Empty;
        }

        string strBinding = string.Empty;
        string strKey = KeyConverter.ToString(keyGesture.Key);
        Debug.Assert(strKey is not null); // KeyGesture constructor ensures that Key is defined

        if (strKey != string.Empty)
        {
            strBinding += ModifierKeysConverter.ToString(keyGesture.Modifiers);
            if (strBinding != string.Empty)
            {
                strBinding += MODIFIERS_DELIMITER;
            }
            strBinding += strKey;

            if (!string.IsNullOrEmpty(keyGesture.DisplayString))
            {
                strBinding += DISPLAYSTRING_SEPARATOR + keyGesture.DisplayString;
            }
        }
        return strBinding;
    }
}
