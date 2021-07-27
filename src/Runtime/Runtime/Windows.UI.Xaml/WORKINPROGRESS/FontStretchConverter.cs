

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
    #if WORKINPROGRESS
    /// <summary>
    /// Converts a <see cref="T:System.Windows.FontStretch" /> object to and from other types.
    /// </summary>
    public class FontStretchConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether an object of the specified type can be converted to an instance of <see cref="T:System.Windows.FontStretch" />.
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
        /// Determines whether an instance of <see cref="T:System.Windows.FontStretch" /> can be converted to the specified type.
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

        /// <summary>Attempts to convert a specified object to an instance of <see cref="T:System.Windows.FontStretch" />.</summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The object being converted.</param>
        /// <returns>The instance of <see cref="T:System.Windows.FontStretch" /> created from the converted <paramref name="value" />.</returns>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="value" /> is <see langword="null" /> or is not a valid type for conversion.</exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            object result;

            if (value is null)
            {
                throw GetConvertFromException(value);
            }
            else if (value is string)
            {
                result = new FontStretch();
            }
            else
            {
                result = base.ConvertFrom(context, culture, value);
            }

            return result;
        }

        /// <summary>Attempts to convert a <see cref="T:System.Windows.FontStretch" /> to a specified type. </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The <see cref="T:System.Windows.FontStretch" /> to convert.</param>
        /// <param name="destinationType">The type to convert this <see cref="T:System.Windows.FontStretch" /> to.</param>
        /// <returns>The object created from converting this <see cref="T:System.Windows.FontStretch" />.</returns>
        /// <exception cref="T:System.NotSupportedException">
        /// Thrown if <paramref name="value" /> is <see langword="null" /> or not a <see cref="T:System.Windows.FontStretch" />,
        /// or if the <paramref name="destinationType" /> is not one of the valid types for conversion.</exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            object result = null;

            if (destinationType != null && value is FontStretch fontStretch)
            {
                if (destinationType == typeof(InstanceDescriptor))
                {
                    var mi = typeof(FontStretch).GetMethod("FromOpenTypeStretch", new Type[] { typeof(int) });
                    result = new InstanceDescriptor(mi, new object[] { fontStretch.ToOpenTypeStretch() });
                }
                else if (destinationType == typeof(string))
                {
                    result = ((IFormattable)fontStretch).ToString(null, culture);
                }
            }

            if (result is null)
            {
                result = base.ConvertTo(context, culture, value, destinationType);
            }

            return result;
        }
    }
#endif
}
