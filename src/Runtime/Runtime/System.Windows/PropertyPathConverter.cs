
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
    /// Provides type conversion support for the <see cref="PropertyPath"/> type.
    /// </summary>
    public class PropertyPathConverter : TypeConverter
    {
        /// <summary>
        /// Returns whether this converter can convert an object of one type to the <see cref="PropertyPath"/>
        /// type.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext"/> that provides a format context.
        /// </param>
        /// <param name="sourceType">
        /// A <see cref="Type"/> that represents the type you want to convert from.
        /// </param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
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
        /// Converts the given value to the <see cref="PropertyPath"/> type.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext"/> that provides a format context.
        /// </param>
        /// <param name="culture">
        /// The <see cref="CultureInfo"/> to use as the current culture.
        /// </param>
        /// <param name="value">
        /// The object to convert.
        /// </param>
        /// <returns>
        /// The returned <see cref="PropertyPath"/>.
        /// </returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (null == value || value is string)
            {
                return new PropertyPath((string)value);
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
            throw new NotImplementedException($"'{typeof(PropertyPathConverter)}' does not implement '{nameof(ConvertTo)}'.");
        }
    }
}
