

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
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
{
    /// <summary>
    /// Converts a <see cref="T:System.Windows.Data.RelativeSource" /> object to and from other types.
    /// </summary>
    public class RelativeSourceConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether an object of the specified type can be converted to an instance of <see cref="T:System.Windows.Data.RelativeSource" />.
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
        /// Determines whether an instance of <see cref="T:System.Windows.Data.RelativeSource" /> can be converted to the specified type.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="destinationType">The type being evaluated for conversion.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="destinationType" /> is of type <see cref="T:System.ComponentModel.Design.Serialization.InstanceDescriptor" />; otherwise, <see langword="false" />.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        /// <summary>Attempts to convert the specified object to a <see cref="T:System.Windows.Data.RelativeSource" />.</summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The object being converted.</param>
        /// <returns>The <see cref="T:System.Windows.Data.RelativeSource" /> created from converting <paramref name="value" />.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            object result;

            if (value is null)
            {
                throw GetConvertFromException(value);
            }
            else if (value is string)
            {
                var relativeSourceAsString = value.ToString();

                switch (relativeSourceAsString)
                {
                    case "None":
                        result = new RelativeSource() { Mode = RelativeSourceMode.None };
                        break;
                    case "Self":
                        result = new RelativeSource() { Mode = RelativeSourceMode.Self };
                        break;
                    case "TemplatedParent":
                        result = new RelativeSource() { Mode = RelativeSourceMode.TemplatedParent };
                        break;
                    default:
                        throw new FormatException(relativeSourceAsString + " is not an eligible value for a RelativeSource");
                }
            }
            else
            {
                result = base.ConvertFrom(context, culture, value);
            }

            return result;
        }

        /// <summary>Attempts to convert a <see cref="T:System.Windows.Data.RelativeSource" /> to a specified type. </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The <see cref="T:System.Windows.Data.RelativeSource" /> to convert.</param>
        /// <param name="destinationType">The type to convert this <see cref="T:System.Windows.Data.RelativeSource" /> to.</param>
        /// <returns>The object created from converting this <see cref="T:System.Windows.Data.RelativeSource" />.</returns>
        /// <exception cref="T:System.ArgumentException">The <paramref name="value" /> object is invalid <see cref="T:System.Windows.Data.RelativeSource" /> .</exception>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> or <paramref name="destinationType" /> are <see langword="null" />.</exception>
        /// <exception cref="T:System.NotSupportedException">The <paramref name="value" /> is not of type <see cref="T:System.Windows.Data.RelativeSource" /> or <paramref name="destinationType" /> is not of type <see langword="string" />.</exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var result = string.Empty;

            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            else if (destinationType is null)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }
            else if (destinationType != typeof(string) || !(value is RelativeSource source))
            {
                throw GetConvertToException(value, destinationType);
            }
            else
            {
                switch (source.Mode)
                {
                    case RelativeSourceMode.None:
                        result = RelativeSourceMode.None.ToString();
                        break;
                    case RelativeSourceMode.Self:
                        result = RelativeSourceMode.Self.ToString();
                        break;
                    case RelativeSourceMode.TemplatedParent:
                        result = RelativeSourceMode.TemplatedParent.ToString();
                        break;
                    default:
                        throw new ArgumentException($"Invalid {nameof(RelativeSource)}.");
                }
            }

            return result;
        }
    }
}
