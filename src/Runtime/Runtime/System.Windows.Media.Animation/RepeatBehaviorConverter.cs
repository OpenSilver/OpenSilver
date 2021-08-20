

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
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;

#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
    /// <summary>
    /// Converts a <see cref="T:System.Windows.Media.Animation.RepeatBehaviorConverter" /> object to and from other types.
    /// </summary>
    public sealed partial class RepeatBehaviorConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether an object of the specified type can be converted to an instance of <see cref="T:System.Windows.Media.Animation.RepeatBehaviorConverter" />.
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
        /// Determines whether an instance of <see cref="T:System.Windows.Media.Animation.RepeatBehaviorConverter" /> can be converted to the specified type.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="destinationType">The type being evaluated for conversion.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="destinationType" /> is of type <see cref="T:System.String" />; 
        /// otherwise, <see langword="false" />.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        /// <summary>Converts a given string value to an instance of <see cref="T:System.Windows.Media.Animation.RepeatBehaviorConverter" />.</summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The object being converted.</param>
        /// <returns>A new <see cref="T:System.Windows.Media.Animation.RepeatBehavior" /> object based on <paramref name="value" />.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            object result;

            if (value is null)
            {
                throw GetConvertFromException(value);
            }
            else if (value is string)
            {
                var arg = value.ToString();

                //BRIDGETODO : verify the code below matchs
#if NETSTANDARD
                var loweredArg = arg.ToLowerInvariant();
#else
                var loweredArg = arg.ToLower();
#endif
                if (loweredArg == "forever")
                {
                    result = RepeatBehavior.Forever;
                }
                else if (loweredArg.EndsWith("x"))
                {
                    var repeatCount = double.Parse(loweredArg.Substring(0, loweredArg.Length - 1));
                    result = new RepeatBehavior(repeatCount);
                }
                else
                {
                    throw new FormatException("The string: \"" + arg + "\" could not be parsed into a RepeatBehavior. Note: The duration is not supported yet as a RepeatBehavior.");
                }
                //TODO: else duration.
            }
            else
            {
                result = base.ConvertFrom(context, culture, value);
            }

            return result;
        }

        /// <summary>Attempts to convert a <see cref="T:System.Windows.Media.Animation.RepeatBehavior" /> to a specified type. </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="cultureInfo">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The <see cref="T:System.Windows.Media.Animation.RepeatBehavior" /> to convert.</param>
        /// <param name="destinationType">The type to convert this <see cref="T:System.Windows.Media.Animation.RepeatBehavior" /> to.</param>
        /// <returns>The object created from converting this <see cref="T:System.Windows.Media.Animation.RepeatBehavior" /> (a string).</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo cultureInfo, object value, Type destinationType)
        {
            object result = null;

            if (value is RepeatBehavior behavior && destinationType != null)
            {
                if (destinationType == typeof(string))
                {
                    switch (behavior.Type)
                    {
                        case RepeatBehaviorType.Forever:
                            result = "Forever";
                            break;
                        case RepeatBehaviorType.Count:
                            var sb = new StringBuilder();
#if BRIDGE
                            sb.AppendFormat("{0:}x", behavior.Count);
#else
                            sb.AppendFormat(cultureInfo, "{0:}x", behavior.Count);
#endif
                            result = sb.ToString();
                            break;
                        default:
                            Debug.Fail("Unhandled RepeatBehaviorType.");
                            result = null;
                            break;
                    }
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