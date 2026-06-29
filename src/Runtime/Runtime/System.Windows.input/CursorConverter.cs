
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

using OpenSilver.Internal;
using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace System.Windows.Input;

/// <summary>
/// Converts a <see cref="Cursor"/> object to and from other types.
/// </summary>
public class CursorConverter : TypeConverter
{
    private StandardValuesCollection _standardValues;

    /// <summary>
    /// Determines whether an object of the specified type can be converted to an instance of <see cref="Cursor"/>, using the specified context.
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
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string);

    /// <summary>
    /// Determines whether an instance of <see cref="Cursor"/> can be converted to the specified type, using the specified context.
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
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(string);

    /// <summary>
    /// Attempts to convert the specified object to a <see cref="Cursor"/>, using the specified context.
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
    /// <returns>
    /// The converted object, or null if value is an empty string.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// value cannot be converted.
    /// </exception>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string s)
        {
            string text = s.Trim();

            if (text == string.Empty)
            {
                return null;
            }

            if (text.LastIndexOf('.') == -1)
            {
                if (Enum.TryParse(text, true, out CursorType cursorType) &&
                    (int)cursorType >= (int)CursorType.None && (int)cursorType <= (int)CursorType.Eraser)
                {
                    return Cursors.EnsureCursor(cursorType);
                }
                else
                {
                    throw new FormatException($"'{value}' is not a valid token.");
                }
            }
            else
            {
                if (text.EndsWith(".cur", StringComparison.OrdinalIgnoreCase) && AppResourcesManager.GetResourceStream(text) is Stream stream)
                {
                    return new Cursor(stream);
                }
            }
        }

        throw GetConvertFromException(value);
    }

    /// <summary>
    /// Attempts to convert a <see cref="Cursor"/> to the specified type, using the specified context.
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
    /// destinationType is null.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// source cannot be converted.
    /// </exception>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        ArgumentNullException.ThrowIfNull(destinationType);

        if (destinationType == typeof(string))
        {
            if (value is Cursor cursor)
            {
                return cursor.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        throw GetConvertToException(value, destinationType);
    }

    /// <summary>
    /// Gets a collection of standard cursor values, using the specified context.
    /// </summary>
    /// <param name="context">
    /// A format context that provides information about the environment from which this converter is being invoked.
    /// </param>
    /// <returns>
    /// A collection that holds a standard set of valid values.
    /// </returns>
    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
    {
        if (_standardValues is null)
        {
            var cursorTypes = (CursorType[])Enum.GetValues(typeof(CursorType));
            Cursor[] cursors = new Cursor[cursorTypes.Length];
            for (int i = 0; i < cursorTypes.Length; i++)
            {
                cursors[i] = Cursors.EnsureCursor(cursorTypes[i]);
            }
            _standardValues = new StandardValuesCollection(cursors);
        }

        return _standardValues;
    }

    /// <summary>
    /// Determines whether this object supports a standard set of values that can be picked from a list, using the specified context.
    /// </summary>
    /// <param name="context">
    /// A format context that provides information about the environment from which this converter is being invoked.
    /// </param>
    /// <returns>
    /// Always returns true.
    /// </returns>
    public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;
}
