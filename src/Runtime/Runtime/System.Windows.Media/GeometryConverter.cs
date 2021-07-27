

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
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Converts a <see cref="T:System.Windows.Media.Geometry" /> object to and from other types.
    /// </summary>
    public sealed partial class GeometryConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether an object of the specified type can be converted to an instance of <see cref="T:System.Windows.Media.Geometry" />.
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
        /// Determines whether an instance of <see cref="T:System.Windows.Media.Geometry" /> can be converted to the specified type.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="destinationType">The type being evaluated for conversion.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="destinationType" /> is of type <see cref="T:System.String" /> or <see cref="T:System.Windows.Media.Geometry" />; otherwise, <see langword="false" />.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                // When invoked by the serialization engine we can convert to string only for some instances
                if (context != null && context.Instance != null)
                {
                    if (!(context.Instance is Geometry))
                    {
                        throw new ArgumentException($"Expected type of {nameof(Geometry)}.");
                    }

                    return false;
                }

                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>Converts the specified object to a <see cref="T:System.Windows.Media.Geometry" />.</summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The object being converted.</param>
        /// <returns>The <see cref="T:System.Windows.Media.Geometry" /> created from converting <paramref name="value" />.</returns>
        /// <exception cref="T:System.NotSupportedException">Thrown if <paramref name="value" /> is <see langword="null" /> or is not a valid type which can be converted to a <see cref="T:System.Windows.Media.Geometry" />.</exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            object result;

            if (value is null)
            {
                throw GetConvertFromException(value);
            }
            else if (value is string)
            {
                result = GeometryParser.ParseGeometry(value.ToString());
            }
            else
            {
                result = base.ConvertFrom(context, culture, value);
            }

            return result;

        }

        /// <summary>Attempts to convert a <see cref="T:System.Windows.Media.Geometry" /> to a specified type. </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The <see cref="T:System.Windows.Media.Geometry" /> to convert.</param>
        /// <param name="destinationType">The type to convert this <see cref="T:System.Windows.Media.Geometry" /> to.</param>
        /// <returns>The object created from converting this <see cref="T:System.Windows.Media.Geometry" />.</returns>
        /// <exception cref="T:System.NotSupportedException">
        /// <paramref name="value" /> is <see langword="null" />, <paramref name="value" /> is not a <see cref="T:System.Windows.Media.Geometry" />, or <paramref name="destinationType" /> is not a string.</exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            object result = null;

            if (destinationType != null && value is Geometry geometry)
            {
                if (destinationType == typeof(string))
                {
                    if (context != null && context.Instance != null)
                    {
                        throw new NotSupportedException($"Conversion to {destinationType.FullName} is not supported.");
                    }

                    result = geometry.ToString();
                }
            }

            if (result is null)
            {
                result = base.ConvertTo(context, culture, value, destinationType);
            }

            return result;
        }
    }
}