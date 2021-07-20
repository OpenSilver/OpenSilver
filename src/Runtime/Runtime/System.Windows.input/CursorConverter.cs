

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

namespace System.Windows.Input
{
    /// <summary>
    /// Converts a <see cref="T:System.Windows.Input.Cursor" /> object to and from other types.
    /// </summary>
    public partial class CursorConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether an object of the specified type can be converted to an instance of <see cref="T:System.Windows.Input.Cursor" />.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="sourceType">The type being evaluated for conversion.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="sourceType" /> is of type <see cref="T:System.String" />; otherwise, <see langword="false" />.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        /// <summary>
        /// Determines whether an instance of <see cref="T:System.Windows.Input.Cursor" /> can be converted to the specified type.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="destinationType">
        /// The desired type this System.Windows.Input.Cursor is being evaluated to be
        /// converted to.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="destinationType" /> is of type <see cref="T:System.String" />; otherwise, <see langword="false" />.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        /// <summary>
        /// Attempts to convert the specified object to a <see cref="T:System.Windows.Input.Cursor" />.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Culture specific information.</param>
        /// <param name="value">The object to convert.</param>
        /// <returns>The converted object, or <see langword="null" /> if <paramref name="value" /> is an empty string.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            object result;

            if (value is null)
            {
                throw GetConvertFromException(value);
            }
            else if (value is string)
            {
                if (Enum.TryParse(value.ToString(), out CursorType cursorType) && (int)cursorType >= (int)CursorType.None && (int)cursorType <= (int)CursorType.Eraser)
                {
                    result = Cursors.EnsureCursor(cursorType);
                }
                else
                {
                    throw new ArgumentException(string.Format("'{0}' cursor type is not valid.", value.ToString()));
                }
            }
            else
            {
                result = base.ConvertFrom(context, culture, value);
            }

            return result;
        }

        /// <summary>
        /// Attempts to convert a <see cref="T:System.Windows.Input.Cursor" /> to the specified type.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Culture specific information.</param>
        /// <param name="value">The object to convert.</param>
        /// <param name="destinationType">The type to convert the object to.</param>
        /// <returns>The converted object, or an empty string if <paramref name="value" /> is <see langword="null" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="destinationType" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.NotSupportedException">
        /// <paramref name="value" /> cannot be converted.</exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var result = string.Empty;

            if (destinationType is null)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }
            else if (destinationType != typeof(string))
            {
                throw GetConvertToException(value, destinationType);
            }
            else
            {
                if (value is Cursor cursor)
                {
                    result = cursor.ToString();
                }
            }

            return result;
        }
    }
}
