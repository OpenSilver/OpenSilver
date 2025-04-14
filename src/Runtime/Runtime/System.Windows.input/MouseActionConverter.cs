
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
/// Converts a <see cref="MouseAction"/> object to and from other types.
/// </summary>
public class MouseActionConverter : TypeConverter
{
    /// <summary>
    /// Determines whether an object of the specified type can be converted to an instance of <see cref="MouseAction"/>,
    /// using the specified context.
    /// </summary>
    /// <param name="context">
    /// A format context that provides information about the environment from which this converter is being invoked.
    /// </param>
    /// <param name="sourceType">
    /// The type being evaluated for conversion.
    /// </param>
    /// <returns>
    /// true if this converter can perform the operation; otherwise, false.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        // We can only handle string
        return sourceType == typeof(string);
    }

    /// <summary>
    /// Determines whether an instance of <see cref="MouseAction"/> can be converted to the specified type, using the 
    /// specified context.
    /// </summary>
    /// <param name="context">
    /// A format context that provides information about the environment from which this converter is being invoked.
    /// </param>
    /// <param name="destinationType">
    /// The type being evaluated for conversion.
    /// </param>
    /// <returns>
    /// true if this converter can perform the operation; otherwise, false.
    /// </returns>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
        // We can convert to an InstanceDescriptor or to a string
        if (destinationType != typeof(string))
        {
            return false;
        }

        // When invoked by the serialization engine we can convert to string only for known type
        if (context is null || context.Instance is null)
        {
            return false;
        }

        // Make sure the value falls within defined set
        return IsDefinedMouseAction((MouseAction)context.Instance);
    }

    /// <summary>
    /// Attempts to convert the specified object to a <see cref="MouseAction"/>, using the specified context.
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
        if (source is not string mouseAction)
        {
            throw GetConvertFromException(source);
        }

        return FromString(mouseAction.AsSpan());
    }

    internal static MouseAction FromString(ReadOnlySpan<char> source)
    {
        ReadOnlySpan<char> mouseActionToken = source.Trim();
        return mouseActionToken switch
        {
            _ when mouseActionToken.IsEmpty => MouseAction.None, // Special casing as produced by "ConvertTo"
            _ when mouseActionToken.Equals("None".AsSpan(), StringComparison.OrdinalIgnoreCase) => MouseAction.None,
            _ when mouseActionToken.Equals("LeftClick".AsSpan(), StringComparison.OrdinalIgnoreCase) => MouseAction.LeftClick,
            _ when mouseActionToken.Equals("RightClick".AsSpan(), StringComparison.OrdinalIgnoreCase) => MouseAction.RightClick,
            _ when mouseActionToken.Equals("MiddleClick".AsSpan(), StringComparison.OrdinalIgnoreCase) => MouseAction.MiddleClick,
            _ when mouseActionToken.Equals("WheelClick".AsSpan(), StringComparison.OrdinalIgnoreCase) => MouseAction.WheelClick,
            _ when mouseActionToken.Equals("LeftDoubleClick".AsSpan(), StringComparison.OrdinalIgnoreCase) => MouseAction.LeftDoubleClick,
            _ when mouseActionToken.Equals("RightDoubleClick".AsSpan(), StringComparison.OrdinalIgnoreCase) => MouseAction.RightDoubleClick,
            _ when mouseActionToken.Equals("MiddleDoubleClick".AsSpan(), StringComparison.OrdinalIgnoreCase) => MouseAction.MiddleDoubleClick,
            _ => throw new NotSupportedException(string.Format(Strings.Unsupported_MouseAction, mouseActionToken.ToString()))
        };
    }

    /// <summary>
    /// Attempts to convert a <see cref="MouseAction"/> to the specified type, using the specified context.
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
    /// <exception cref="InvalidEnumArgumentException">
    /// <paramref name="value"/> does not map to a valid <see cref="MouseAction"/>.
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

        if (value is null || destinationType != typeof(string))
        {
            throw GetConvertToException(value, destinationType);
        }

        MouseAction mouseAction = (MouseAction)value;
        return ToString(mouseAction);
    }

    internal static string ToString(MouseAction mouseAction)
    {
        return mouseAction switch
        {
            MouseAction.None => string.Empty,
            MouseAction.LeftClick => "LeftClick",
            MouseAction.RightClick => "RightClick",
            MouseAction.MiddleClick => "MiddleClick",
            MouseAction.WheelClick => "WheelClick",
            MouseAction.LeftDoubleClick => "LeftDoubleClick",
            MouseAction.RightDoubleClick => "RightDoubleClick",
            MouseAction.MiddleDoubleClick => "MiddleDoubleClick",
            _ => throw new InvalidEnumArgumentException(nameof(mouseAction), (int)mouseAction, typeof(MouseAction))
        };
    }

    internal static bool IsDefinedMouseAction(MouseAction mouseAction) => mouseAction >= MouseAction.None && mouseAction <= MouseAction.MiddleDoubleClick;
}
