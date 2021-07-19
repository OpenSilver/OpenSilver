

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

#if MIGRATION
namespace System.Windows
#else
namespace Windows.Foundation
#endif
{
    /// <summary>
    /// Converts a <see cref="T:System.Windows.Size" /> object to and from other types.
    /// </summary>
    public sealed partial class SizeConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether an object of the specified type can be converted to an instance of <see cref="T:System.Windows.Size" />.
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
        /// Determines whether an instance of <see cref="T:System.Windows.Size" /> can be converted to the specified type.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="destinationType">The type being evaluated for conversion.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="destinationType" /> is of type <see cref="T:System.String" />; otherwise, <see langword="false" />.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        // Exceptions:
        //   System.ArgumentNullException:
        //     source is null.
        //
        //   System.NotSupportedException:
        //     source is not null and is not a valid type which can be converted to a System.Windows.Size.
        /// <summary>
        /// Converts the specified object to a System.Windows.Size.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The object being converted.</param>
        /// <returns>The System.Windows.Size created from converting source.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            else if (value.GetType() != typeof(string))
            {
                throw GetConvertFromException(value);
            }

            return Size.Parse((string)value);
        }

        /// <summary>Attempts to convert a <see cref="T:System.Windows.Size" /> to a specified type. </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The <see cref="T:System.Windows.Size" /> to convert.</param>
        /// <param name="destinationType">The type to convert this <see cref="T:System.Windows.Size" /> to.</param>
        /// <returns>The object created from converting this <see cref="T:System.Windows.Size" />.</returns>
        /// <exception cref="T:System.NotSupportedException">
        /// Thrown if <paramref name="value" /> is <see langword="null" /> or not an instance of <see cref="T:System.Windows.Size" />,
        /// or if the <paramref name="destinationType" /> is not one of the valid types for conversion.</exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            object result;

            if (destinationType != null && destinationType == typeof(string) && value is Size size)
            {
                result = size.ToString();
            }
            else
            {
                result = base.ConvertTo(context, culture, value, destinationType);
            }

            return result;
        }
    }
}
