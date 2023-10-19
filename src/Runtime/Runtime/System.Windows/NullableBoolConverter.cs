
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

using System;
using System.ComponentModel;
using System.Globalization;

namespace System.Windows
{
	/// <summary>
	/// Converts <see cref="Nullable{T}"/> types (using the <see cref="bool"/> type constraint on
	/// the generic) from strings.
	/// </summary>
	public sealed class NullableBoolConverter : TypeConverter
	{
        /// <summary>
        /// Determines whether this converter can convert an object of the specified type
        /// to the <see cref="Nullable{T}" /> type (using the <see cref="bool"/> type constraint on the
        /// generic).
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext"/> that provides a format context.
        /// </param>
        /// <param name="sourceType">
        /// The type that you want to convert from.
        /// </param>
        /// <returns>
        /// true if sourceType is a <see cref="string"/>, <see cref="bool"/>, or a <see cref="Nullable{T}"/>
        /// type (using the <see cref="bool"/> type constraint on the generic) that can be assigned
        /// from sourceType; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string) || sourceType == typeof(bool) || sourceType == typeof(bool?))
            {
                return true;
            }

            return false;
        }

        // Note: we override this method to emulate the behavior of the base.CanConvertTo() from
        // Silverlight, which always returns false.

        /// <summary>
        /// Always returns false.
        /// </summary>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false;
        }

        /// <summary>
        /// Converts the specified object to a <see cref="Nullable{T}"/> type (using the <see cref="bool"/>
        /// type constraint on the generic).
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext"/> that provides a format context.
        /// </param>
        /// <param name="culture">
        /// The <see cref="CultureInfo"/> to use as the current culture.
        /// </param>
        /// <param name="value">
        /// The object to convert to a <see cref="Nullable{T}"/> type (using the <see cref="bool"/> type
        /// constraint on the generic).
        /// </param>
        /// <returns>
        /// A <see cref="Nullable{T}"/> type (using the <see cref="bool"/> type constraint on the generic)
        /// that represents the converted object.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// The conversion attempt failed. value is not a <see cref="string"/>, <see cref="bool"/>,
        /// or <see cref="Nullable{T}"/> (using the <see cref="bool"/> type constraint on the generic)
        /// type.
        /// </exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (null == value)
            {
                return null;
            }

            if (value is string stringValue)
            {
                if (stringValue.Length == 0)
                {
                    return null;
                }

                return bool.Parse(stringValue);
            }

            if (value is bool || value is bool?)
            {
                return value;
            }

            throw GetConvertFromException(value);
        }

        // Note: we override this method to emulate the behavior of the base.ConvertTo() from
        // Silverlight, which always throws a NotImplementedException.

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// Always throws.
        /// </exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new NotImplementedException($"'{typeof(NullableBoolConverter)}' does not implement '{nameof(ConvertTo)}'.");
        }
    }
}
