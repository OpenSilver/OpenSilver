
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
using System.Globalization;

namespace System.Windows.Media.Animation
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class RepeatBehaviorConverter : TypeConverter
    {
        private static readonly char[] _iterationCharacter = new char[] { 'x' };

        /// <summary>
        /// CanConvertFrom - Returns whether or not this class can convert from a given type
        /// </summary>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        /// <summary>
        /// TypeConverter method override.
        /// </summary>
        /// <param name="context">ITypeDescriptorContext</param>
        /// <param name="destinationType">Type to convert to</param>
        /// <returns>true if conversion is possible</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        /// <summary>
        /// ConvertFrom
        /// </summary>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string stringValue = value as string;

            if (stringValue != null)
            {
                stringValue = stringValue.Trim();
                stringValue = stringValue.ToLowerInvariant();

                if (stringValue == "forever")
                {
                    return RepeatBehavior.Forever;
                }
                else if (stringValue.Length > 0 
                    && stringValue[stringValue.Length - 1] == _iterationCharacter[0])
                {
                    string stringDoubleValue = stringValue.TrimEnd(_iterationCharacter);

                    double doubleValue = (double)TypeConverterHelper.GetConverter(
                        typeof(double)).ConvertFrom(
                            context,
                            culture,
                            stringDoubleValue);

                    return new RepeatBehavior(doubleValue);
                }
            }

            // The value is not Forever or an iteration count so it's either a TimeSpan
            // or we'll let the TimeSpanConverter raise the appropriate exception.

            TimeSpan timeSpanValue = (TimeSpan)TypeConverterHelper.GetConverter(
                typeof(TimeSpan)).ConvertFrom(
                    context, 
                    culture, 
                    stringValue);

            return new RepeatBehavior(timeSpanValue);
        }

        /// <summary>
        /// TypeConverter method implementation.
        /// </summary>
        /// <param name="context">ITypeDescriptorContext</param>
        /// <param name="culture">current culture (see CLR specs)</param>
        /// <param name="value">value to convert from</param>
        /// <param name="destinationType">Type to convert to</param>
        /// <returns>converted value</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }

            if (destinationType == typeof(string))
            {
                if (value is RepeatBehavior repeatBehavior)
                {
                    return repeatBehavior.ToString(culture);
                }
            }

            throw GetConvertToException(value, destinationType);
        }
    }
}
