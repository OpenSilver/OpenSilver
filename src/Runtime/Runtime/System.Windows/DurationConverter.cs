

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
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Converts a <see cref="T:System.Windows.Duration" /> object to and from other types.
    /// </summary>
    public partial class DurationConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether an object of the specified type can be converted to an instance of <see cref="T:System.Windows.Duration" />.
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
        /// Determines whether an instance of <see cref="T:System.Windows.Duration" /> can be converted to the specified type.
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

        /// <summary>Converts a given string value to an instance of <see cref="T:System.Windows.Duration" />.</summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The object being converted.</param>
        /// <returns>A new instance of <see cref="T:System.Windows.Duration" />.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            object result;

            if (value is null)
            {
                throw GetConvertFromException(value);
            }
            else if (value is string)
            {
                var duration = value.ToString();

                if (duration.ToLower() == "forever")
                    return Duration.Forever;
                if (duration.ToLower() == "automatic")
                    return Duration.Automatic;
#if BRIDGE
            TimeSpan timeSpan = INTERNAL_BridgeWorkarounds.TimeSpanParse(duration);
#else
                TimeSpan timeSpan = TimeSpan.Parse(duration);
#endif
                result = new Duration(timeSpan);
            }
            else
            {
                result = base.ConvertFrom(context, culture, value);
            }

            return result;
        }

        /// <summary>Attempts to convert a <see cref="T:System.Windows.Duration" /> to a specified type. </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="cultureInfo">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The <see cref="T:System.Windows.Duration" /> to convert.</param>
        /// <param name="destinationType">The type to convert this <see cref="T:System.Windows.Duration" /> to.</param>
        /// <returns>A new instance of the <paramref name="destinationType" />.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo cultureInfo, object value, Type destinationType)
        {
            object result = null;

            if (destinationType != null && value is Duration dur)
            {
                if (destinationType == typeof(InstanceDescriptor))
                {
                    MemberInfo mi;

                    if (dur.HasTimeSpan)
                    {
                        mi = typeof(Duration).GetConstructor(new Type[] { typeof(TimeSpan) });

                        result = new InstanceDescriptor(mi, new object[] { dur.TimeSpan });
                    }
                    else if (dur == Duration.Forever)
                    {
                        mi = typeof(Duration).GetProperty("Forever");

                        result = new InstanceDescriptor(mi, null);
                    }
                    else
                    {
                        Debug.Assert(dur == Duration.Automatic);  // Only other legal duration type

                        mi = typeof(Duration).GetProperty("Automatic");

                        result = new InstanceDescriptor(mi, null);
                    }
                }
                else if (destinationType == typeof(string))
                {
                    result = dur.ToString();
                }
            }

            if (result is null)
            {
                result = base.ConvertTo(context, cultureInfo, value, destinationType);
            }

            return result;
        }
    }
}