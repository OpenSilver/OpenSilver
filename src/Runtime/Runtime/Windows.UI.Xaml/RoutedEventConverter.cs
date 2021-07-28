

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
    /// Converts a <see cref="T:System.Windows.RoutedEvent" /> object to and from other types.
    /// </summary>
    public sealed class RoutedEventConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether an object of the specified type can be converted to an instance of  <see cref="T:System.Windows.RoutedEvent" />.
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
        /// Determines whether an instance of <see cref="T:System.Windows.RoutedEvent" /> can be converted to the specified type.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="destinationType">The type being evaluated for conversion.</param>
        /// <returns>Always returns <see langword="false" />.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false;
        }

        /// <summary>Attempts to convert the specified object to a <see cref="T:System.Windows.RoutedEvent" /> object, using the specified context.</summary>
        /// <param name="typeDescriptorContext">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The object being converted.</param>
        /// <returns>The <see cref="T:System.Windows.RoutedEvent" /> created from converting <paramref name="value" />.</returns>
        /// <exception cref="T:System.NotSupportedException">
        ///         <paramref name="value" /> is not a string or cannot be converted.</exception>
        public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext, CultureInfo culture, object value)
        {
            RoutedEvent result = null;

            if (value is null)
            {
                throw GetConvertFromException(value);
            }
            else if (value is string routedEventName && typeDescriptorContext is IServiceProvider serviceProvider)
            {
                throw new NotImplementedException();
            }

            if (result is null)
            {
                throw GetConvertFromException(value);
            }

            return result;
        }

        /// <summary>Attempts to convert a <see cref="T:System.Windows.RoutedEvent" /> to the specified type. Throws an exception in all cases.</summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The <see cref="T:System.Windows.Media.RoutedEvent" /> to convert.</param>
        /// <param name="destinationType">The type to convert this <see cref="T:System.Windows.Media.RoutedEvent" /> to.</param>
        /// <exception cref="T:System.NotSupportedException">
        ///         <paramref name="value" /> cannot be converted. This is not a functioning converter for a save path..</exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///         <paramref name="value" /> or <paramref name="destinationType" /> are <see langword="null" />.</exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            else if (destinationType is null)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }
            else
            {
                throw GetConvertToException(value, destinationType);
            }
        }
    }
}
