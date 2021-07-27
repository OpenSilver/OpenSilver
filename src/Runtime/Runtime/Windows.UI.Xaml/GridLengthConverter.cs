

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
using System.ComponentModel.Design.Serialization;
using System.Globalization;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Converts a <see cref="T:System.Windows.GridLength" /> object to and from other types.
    /// </summary>
    public class GridLengthConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether an object of the specified type can be converted to an instance of <see cref="T:System.Windows.GridLength" />.
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
        /// Determines whether an instance of <see cref="T:System.Windows.GridLength" /> can be converted to the specified type.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="destinationType">The type being evaluated for conversion.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="destinationType" /> is of type <see cref="T:System.String" />
        /// or <see cref="T:System.ComponentModel.Design.Serialization.InstanceDescriptor" />; otherwise, <see langword="false" />.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(InstanceDescriptor) || destinationType == typeof(string);
        }

        /// <summary>Attempts to convert a specified object to an instance of <see cref="T:System.Windows.GridLength" />. </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The object being converted.</param>
        /// <returns>The instance of <see cref="T:System.Windows.GridLength" /> that is created from the converted <paramref name="value" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///         <paramref name="value" /> object is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentException">
        ///         <paramref name="value" /> object is not <see langword="null" /> and is not a valid type that can be converted to a <see cref="T:System.Windows.GridLength" />.</exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            object result;

            if (value is null)
            {
                throw GetConvertFromException(value);
            }
            else if (value is string)
            {
                var gridLengthAsString = value.ToString();

                var trimmedLowercase = gridLengthAsString.Trim().ToLower();
                if (trimmedLowercase.EndsWith("*"))
                {
                    var valueAsString = trimmedLowercase.Substring(0, trimmedLowercase.Length - 1);
                    if (valueAsString == "")
                    {
                        result = new GridLength(1.0, GridUnitType.Star);
                    }
                    if (double.TryParse(valueAsString, out var length))
                    {
                        result = new GridLength(length, GridUnitType.Star);
                    }
                    else
                    {
                        throw new Exception("Invalid GridLength: " + gridLengthAsString);
                    }
                }
                else if (trimmedLowercase == "auto")
                {
                    result = new GridLength(1.0, GridUnitType.Auto);
                }
                else
                {
                    if (double.TryParse(trimmedLowercase, out var length))
                    {
                        result = new GridLength(length, GridUnitType.Pixel);
                    }
                    else
                    {
                        throw new Exception("Invalid GridLength: " + gridLengthAsString);
                    }
                }
            }
            else
            {
                result = base.ConvertFrom(context, culture, value);
            }

            return result;
        }

        /// <summary>Attempts to convert a <see cref="T:System.Windows.GridLength" /> to a specified type. </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="cultureInfo">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The <see cref="T:System.Windows.GridLength" /> to convert.</param>
        /// <param name="destinationType">The type to convert this <see cref="T:System.Windows.GridLength" /> to.</param>
        /// <returns>The object created from converting this <see cref="T:System.Windows.GridLength" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///         <paramref name="destinationType" /> is not one of the valid types for conversion.</exception>
        /// <exception cref="T:System.ArgumentException">
        ///         <paramref name="value" /> is <see langword="null" />.</exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo cultureInfo, object value, Type destinationType)
        {
            object result = null;

            if (destinationType is null)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }
            else if (value != null && value is GridLength length)
            {
                if (destinationType == typeof(string))
                {
                    switch (length.GridUnitType)
                    {
                        //  for Auto print out "Auto". value is always "1.0"
                        case GridUnitType.Auto:
                            result = "Auto";
                            break;

                        //  Star has one special case when value is "1.0".
                        //  in this case drop value part and print only "Star"
                        case GridUnitType.Star:
                            result = Math.Abs(length.Value - 1.0) < 2.2204460492503131E-15
                                ? "*"
                                : Convert.ToString(length.Value, cultureInfo) + "*";
                            break;

                        //  for Pixel print out the numeric value. "px" can be omitted.
                        default:
                            result = Convert.ToString(length.Value, cultureInfo);
                            break;
                    }
                }

                if (destinationType == typeof(InstanceDescriptor))
                {
                    var ci = typeof(GridLength).GetConstructor(new Type[] { typeof(double), typeof(GridUnitType) });
                    result = new InstanceDescriptor(ci, new object[] { length.Value, length.GridUnitType });
                }
            }

            if (result is null)
            {
                throw GetConvertToException(value, destinationType);
            }

            return result;
        }
    }
}
