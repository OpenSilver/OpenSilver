

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
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
    /// <summary>
    /// Converts a <see cref="T:System.Windows.Media.Animation.KeyTime" /> object to and from other types.
    /// </summary>
    public partial class KeyTimeConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether an object of the specified type can be converted to an instance of <see cref="T:System.Windows.Media.Animation.KeyTime" />.
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
        /// Determines whether an instance of <see cref="T:System.Windows.Media.Animation.KeyTime" /> can be converted to the specified type.
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

        /// <summary>Attempts to convert a given object to an instance of <see cref="T:System.Windows.Media.Animation.KeyTime" />.</summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The object being converted.</param>
        /// <returns>A new instance of <see cref="T:System.Windows.Media.Animation.KeyTime" />, based on the supplied <paramref name="value" />.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            object result;

            if (value is null)
            {
                throw GetConvertFromException(value);
            }
            else if (value is string)
            {
                var keyTimeCode = value.ToString();

                try
                {
                    if (keyTimeCode == "Uniform")
                    {
                        throw new NotImplementedException("The Value \"Uniform\" for keyTime is not supported yet.");
                    }
                    else if (keyTimeCode == "Paced")
                    {
                        throw new NotImplementedException("The Value \"Paced\" for keyTime is not supported yet.");
                    }
                    else if (keyTimeCode.EndsWith("%"))
                    {
                        throw new NotImplementedException("The percentage values for keyTime are not supported yet.");
                    }
                    else
                    {
#if BRIDGE
                    TimeSpan timeSpan = INTERNAL_BridgeWorkarounds.TimeSpanParse(keyTimeCode, false);
#else
                        TimeSpan timeSpan = TimeSpan.Parse(keyTimeCode);
#endif
                        result = new KeyTime(timeSpan);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Invalid KeyTime: " + keyTimeCode, ex);
                }
            }
            else
            {
                result = base.ConvertFrom(context, culture, value);
            }

            return result;
        }

        /// <summary>Attempts to convert a <see cref="T:System.Windows.Media.Animation.KeyTime" /> to a specified type. </summary>
        /// <param name="typeDescriptorContext">Describes the context information of a type.</param>
        /// <param name="cultureInfo">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The <see cref="T:System.Windows.Media.Animation.KeyTime" /> to convert.</param>
        /// <param name="destinationType">The type to convert this <see cref="T:System.Windows.Media.Animation.KeyTime" /> to.</param>
        /// <returns>The object created from converting this <see cref="T:System.Windows.Media.Animation.KeyTime" />.</returns>
        public override object ConvertTo(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object value, Type destinationType)
        {
            object result = null;

            if (value != null && value is KeyTime keyTime)
            {
                if (destinationType == typeof(InstanceDescriptor))
                {
                    var mi = typeof(KeyTime).GetMethod("FromTimeSpan", new Type[] { typeof(TimeSpan) });

                    result = new InstanceDescriptor(mi, new object[] { keyTime.TimeSpan });
                }
                else if (destinationType == typeof(string))
                {
                    result = TypeDescriptor.GetConverter(typeof(TimeSpan)).ConvertTo(
                       typeDescriptorContext,
                       cultureInfo,
                       keyTime.TimeSpan,
                       destinationType);
                }
            }

            if (result is null)
            {
                result = base.ConvertTo(typeDescriptorContext, cultureInfo, value, destinationType);
            }

            return result;
        }
    }
}