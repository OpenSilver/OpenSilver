

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
using System.ComponentModel.Design.Serialization;
using System.Globalization;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Converts a <see cref="T:System.Windows.DataGridLength" /> object to and from other types.
    /// </summary>
    public sealed partial class DataGridLengthConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether an object of the specified type can be converted to an instance of <see cref="T:System.Windows.DataGridLength" />.
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
        /// Determines whether an instance of <see cref="T:System.Windows.DataGridLength" /> can be converted to the specified type.
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
        //     source is not null and is not a valid type which can be converted to a System.Windows.Controls.DataGridLength.
        /// <summary>
        /// Converts the specified object to a System.Windows.Controls.DataGridLength.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The object being converted.</param>
        /// <returns>The System.Windows.Controls.DataGridLength created from converting source.</returns>
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

            return DataGridLength.INTERNAL_ConvertFromString((string)value);
        }

        /// <summary>Attempts to convert a <see cref="T:System.Windows.Controls.DataGridLength" /> to a specified type. </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The <see cref="T:System.Windows.Controls.DataGridLength" /> to convert.</param>
        /// <param name="destinationType">The type to convert this <see cref="T:System.Windows.Controls.DataGridLength" /> to.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///         <paramref name="destinationType" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentException">
        ///         <paramref name="value" /> is not a <see cref="T:System.Windows.Controls.DataGridLength" /> or <paramref name="destinationType" /> is not a valid conversion type.</exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            object result = null;

            if (destinationType is null)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }
            else if ((value != null) && (value is DataGridLength length))
            {
                if (destinationType == typeof(string))
                {
                    switch (length.UnitType)
                    {
                        case DataGridLengthUnitType.Auto:
                            result = length.UnitType.ToString();
                            break;

                        // Star has one special case when value is "1.0" in which the value can be dropped.
                        case DataGridLengthUnitType.Star:
                            result = (Math.Abs(length.Value - 1.0) < 2.2204460492503131E-15) ? "*" : Convert.ToString(length.Value, culture) + "*";
                            break;

                        // Print out the numeric value. "px" can be omitted.
                        default:
                            result = Convert.ToString(length.Value, culture);
                            break;
                    }
                }
                else if (destinationType == typeof(InstanceDescriptor))
                {
                    var ci = typeof(DataGridLength).GetConstructor(new Type[] { typeof(double), typeof(DataGridLengthUnitType) });
                    result = new InstanceDescriptor(ci, new object[] { length.Value, length.UnitType });
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