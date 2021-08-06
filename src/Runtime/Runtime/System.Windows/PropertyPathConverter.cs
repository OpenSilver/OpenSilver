

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

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Converts a <see cref="T:System.Windows.PropertyPath" /> object to and from other types.
    /// </summary>
    public sealed partial class PropertyPathConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether an object of the specified type can be converted to an instance of <see cref="T:System.Windows.PropertyPath" />.
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
        /// Determines whether an instance of <see cref="T:System.Windows.PropertyPath" /> can be converted to the specified type.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="destinationType">The type being evaluated for conversion.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="destinationType" /> is of type <see cref="T:System.String" />; otherwise, <see langword="false" />.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        /// <summary>Converts the specified value to the <see cref="T:System.Windows.PropertyPath" /> type.</summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The object being converted.</param>
        /// <returns>The converted <see cref="T:System.Windows.PropertyPath" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> was provided as <see langword="null." /></exception>
        /// <exception cref="T:System.ArgumentException">The <paramref name="value" /> was not <see langword="null" />, but was not of the expected <see cref="T:System.String" /> type.</exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            object result;

            if (value is null)
            {
                throw GetConvertFromException(value);
            }
            else if (value is string)
            {
                result = new PropertyPath(value.ToString());
            }
            else
            {
                result = base.ConvertFrom(context, culture, value);
            }

            return result;
        }

        /// <summary>Attempts to convert a <see cref="T:System.Windows.PropertyPath" /> to a specified type. </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="cultureInfo">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The <see cref="T:System.Windows.PropertyPath" /> to convert.</param>
        /// <param name="destinationType">The type to convert this <see cref="T:System.Windows.PropertyPath" /> to.</param>
        /// <returns>The converted destination <see cref="T:System.String" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> was provided as <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentException">The <paramref name="value" /> was not <see langword="null" />, but was not of the expected <see cref="T:System.Windows.PropertyPath" /> type 
        /// or the <paramref name="destinationType" /> was not the <see cref="T:System.String" /> type.</exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo cultureInfo, object value, Type destinationType)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            else if (destinationType is null)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }

            if (value is PropertyPath path)
            {
                return path.Path;
            }
            else
            {
                throw new ArgumentException($"Unexpected parameter type {value.GetType().FullName}.");
            }
        }
    }
}