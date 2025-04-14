
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

namespace System.Windows.Input;

/// <summary>
/// Converts a <see cref="MouseGesture"/> object to and from other types.
/// </summary>
public class MouseGestureConverter : TypeConverter
{
    private const char MODIFIERS_DELIMITER = '+';

    /// <summary>
    /// Determines whether an object of the specified type can be converted to an instance of <see cref="MouseGesture"/>,
    /// using the specified context.
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
        // We can only handle string.
        return sourceType == typeof(string);
    }

    /// <summary>
    /// Determines whether an instance of <see cref="MouseGesture"/> can be converted to the specified type, using 
    /// the specified context.
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
        // We can convert to an InstanceDescriptor or to a string.
        if (destinationType == typeof(string))
        {
            // When invoked by the serialization engine we can convert to string only for known type
            if (context is not null && context.Instance is not null)
            {
                if (context.Instance is MouseGesture mouseGesture)
                {
                    return ModifierKeysConverter.IsDefinedModifierKeys(mouseGesture.Modifiers) &&
                           MouseActionConverter.IsDefinedMouseAction(mouseGesture.MouseAction);
                }
            }
        }
        return false;
    }

    /// <summary>
    ///  Attempts to convert the specified object to a <see cref="MouseGesture"/>, using the specified context.
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
    /// <paramref name="source"/> cannot be converter.
    /// </exception>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object source)
    {
        if (source is string stringSource)
        {
            ReadOnlySpan<char> fullName = stringSource.AsSpan().Trim();

            if (fullName.Length == 0)
            {
                return new MouseGesture(MouseAction.None, ModifierKeys.None);
            }

            // break apart LocalName and Prefix
            int offset = fullName.LastIndexOf(MODIFIERS_DELIMITER);
            if (offset >= 0)
            {
                // modifiers exists
                ReadOnlySpan<char> modifiersToken = fullName.Slice(0, offset);
                ReadOnlySpan<char> mouseActionToken = fullName.Slice(offset + 1);

                MouseAction mouseAction = MouseActionConverter.FromString(mouseActionToken);
                ModifierKeys modifierKeys = ModifierKeysConverter.FromString(context, culture, modifiersToken.ToString());
                return new MouseGesture(mouseAction, modifierKeys);
            }
            else
            {
                MouseAction mouseAction = MouseActionConverter.FromString(fullName);
                return new MouseGesture(mouseAction);
            }
        }
        throw GetConvertFromException(source);
    }

    /// <summary>
    /// Attempts to convert a <see cref="MouseGesture"/> to the specified type, using the specified context.
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

            if (value is MouseGesture mouseGesture)
            {
                string modifiersKey = ModifierKeysConverter.ToString(mouseGesture.Modifiers);
                string mouseAction = MouseActionConverter.ToString(mouseGesture.MouseAction);
                return modifiersKey switch
                {
                    "" => mouseAction,
                    _ => $"{modifiersKey}{MODIFIERS_DELIMITER}{mouseAction}",
                };
            }
        }
        throw GetConvertToException(value, destinationType);
    }
}
