

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
using Windows.Foundation;
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
    /// Converts a <see cref="T:System.Windows.Media.PointCollection" /> object to and from other types.
    /// </summary>
    public class PointCollectionConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether an object of the specified type can be converted to an instance of <see cref="T:System.Windows.Media.PointCollection" />.
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
        /// Determines whether an instance of <see cref="T:System.Windows.Media.PointCollection" /> can be converted to the specified type.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="destinationType">The type being evaluated for conversion.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="destinationType" /> is of type <see cref="T:System.String" />; otherwise, <see langword="false" />.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        /// <summary>Attempts to convert the specified object to a <see cref="T:System.Windows.Media.PointCollection" />.</summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The object being converted.</param>
        /// <returns>The <see cref="T:System.Windows.Media.PointCollection" /> that is created from converting <paramref name="value" />.</returns>
        /// <exception cref="T:System.NotSupportedException">The specified object is null or is a type that cannot be converted to a <see cref="T:System.Windows.Media.PointCollection" />.</exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            object result;

            if (value is null)
            {
                throw GetConvertFromException(value);
            }
            else if (value is string)
            {
                var pointsAsString = value.ToString();

                var splittedString = pointsAsString.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var collection = new PointCollection();

                // Points count needs to be even number
                if (splittedString.Length % 2 == 0)
                {
                    for (int i = 0; i < splittedString.Length; i += 2)
                    {
#if OPENSILVER
                        if (double.TryParse(splittedString[i], NumberStyles.Any, CultureInfo.InvariantCulture, out var x) &&
                            double.TryParse(splittedString[i + 1], NumberStyles.Any, CultureInfo.InvariantCulture, out var y))
#else
                    if (double.TryParse(splittedString[i], out var x) &&
                        double.TryParse(splittedString[i + 1], out var y))
#endif
                        {
                            collection.Add(new Point(x, y));
                        }
                    }

                    result = collection;
                }
                else
                {
                    throw new FormatException(pointsAsString + " is not an eligible value for a PointCollection");
                }
            }
            else
            {
                result = base.ConvertFrom(context, culture, value);
            }

            return result;
        }

        /// <summary>Attempts to convert a <see cref="T:System.Windows.Media.PointCollection" /> to a specified type. </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The <see cref="T:System.Windows.Media.PointCollection" /> to convert.</param>
        /// <param name="destinationType">The type to convert this <see cref="T:System.Windows.Media.PointCollection" /> to.</param>
        /// <returns>The object created from converting this <see cref="T:System.Windows.Media.PointCollection" />.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            object result = null;

            if (destinationType != null && value is PointCollection collection)
            {
                if (destinationType == typeof(string))
                {
                    result = collection.ToString(null, culture);
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
