

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

#if BRIDGE
using System;
#endif
using System.ComponentModel;
using System.Globalization;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Converts a <see cref="T:System.Windows.DependencyProperty" /> object to and from other types.
    /// </summary>
    public sealed partial class DependencyPropertyConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether an object of the specified type can be converted to an instance of <see cref="T:System.Windows.DependencyProperty" />.
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
        /// Determines whether an instance of <see cref="T:System.Windows.DependencyProperty" /> can be converted to the specified type.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="destinationType">The type being evaluated for conversion.</param>
        /// <returns>
        /// <see langword="true" /> if this converter can perform the operation; otherwise, <see langword="false" />.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false;
        }

        /// <summary>
        /// Attempts to convert a specified object to an instance of Windows.UI.Xaml.DependencyProperty.
        /// </summary>
        /// <param name="context">Context information of a type.</param>
        /// <param name="culture">System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The object being converted.</param>
        /// <returns>The instance of Windows.UI.Xaml.DependencyProperty created from the converted value.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is null)
            {
                throw GetConvertFromException(value);
            }

            return value is string ? null : base.ConvertFrom(context, culture, value);
        }

        /// <summary>Attempts to convert a <see cref="T:System.Windows.DependencyProperty" /> to a specified type. </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The <see cref="T:System.Windows.DependencyProperty" /> to convert.</param>
        /// <param name="destinationType">The type to convert this <see cref="T:System.Windows.DependencyProperty" /> to.</param>
        /// <returns>Always throws an exception.</returns>
        /// <exception cref="T:System.NotSupportedException">In all cases.</exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw GetConvertToException(value, destinationType);
        }
    }
}