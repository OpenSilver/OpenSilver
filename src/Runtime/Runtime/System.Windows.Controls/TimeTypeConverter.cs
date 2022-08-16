// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Allows time to be set from xaml.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    /// <remarks>This converter is used by xaml and thus uses the 
    /// English formats.</remarks>
    public class TimeTypeConverter : TypeConverter
    {
        /// <summary>
        /// BackingField for the TimeFormats being used.
        /// </summary>
        private static readonly string[] _timeFormats = new[]
                              {
                                  "h:mm tt",
                                  "h:mm:ss tt",
                                  "HH:mm",
                                  "HH:mm:ss",
                                  "H:mm",
                                  "H:mm:ss",
                              };

        /// <summary>
        /// BackingField for the DateFormats being used.
        /// </summary>
        private static readonly string[] _dateFormats = new[]
                              {
                                  "M/d/yyyy",
                              };

        /// <summary>
        /// Determines whether this instance can convert from 
        /// the specified type descriptor context.
        /// </summary>
        /// <param name="typeDescriptorContext">The type descriptor context.</param>
        /// <param name="sourceType">Type of the source.</param>
        /// <returns>
        /// 	<c>True</c> if this instance can convert from the specified type 
        /// descriptor context; otherwise, <c>false</c>.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "Compat with WPF.")]
        public override bool CanConvertFrom(ITypeDescriptorContext typeDescriptorContext, Type sourceType)
        {
            return Type.GetTypeCode(sourceType) == TypeCode.String;
        }

        /// <summary>
        /// Determines whether this instance can convert to the specified 
        /// type descriptor context.
        /// </summary>
        /// <param name="typeDescriptorContext">The type descriptor context.</param>
        /// <param name="destinationType">Type of the destination.</param>
        /// <returns>
        /// 	<c>True</c> if this instance can convert to the specified type 
        /// descriptor context; otherwise, <c>false</c>.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "Compat with WPF.")]
        public override bool CanConvertTo(ITypeDescriptorContext typeDescriptorContext, Type destinationType)
        {
            return Type.GetTypeCode(destinationType) == TypeCode.String ? true : TypeConverters.CanConvertTo<DateTime?>(destinationType);
        }

        /// <summary>
        /// Converts instances of other data types into instances of DateTime that
        /// represent a time.
        /// </summary>
        /// <param name="typeDescriptorContext">
        /// The type descriptor context.
        /// </param>
        /// <param name="cultureInfo">The culture used to convert. This culture
        /// is not used during conversion, but a specific set of formats is used.</param>
        /// <param name="source">
        /// The string being converted to the DateTime.
        /// </param>
        /// <returns>
        /// A DateTime that is the value of the conversion.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "Compat with WPF.")]
        [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "1#", Justification = "Compat with WPF.")]
        [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "2#", Justification = "Compat with WPF.")]
        public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object source)
        {
            if (source == null)
            {
                return null;
            }

            string text = source as string;

            if (text == null)
            {
                string invalidCastMessage = string.Format(
                    CultureInfo.CurrentCulture,
                    "'{0}' is unable to convert '{1}' to '{2}'.",
                    GetType().Name,
                    source,
                    typeof(DateTime).Name);
                throw new InvalidCastException(invalidCastMessage);
            }

            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            DateTime result;
            // test using times
            foreach (string format in _timeFormats)
            {
                if (DateTime.TryParseExact(text, format, CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault, out result))
                {
                    return DateTime.Now.Date.Add(result.TimeOfDay);
                }
            }

            // test using combinations of date and time
            foreach (string dateFormat in _dateFormats)
            {
                foreach (string timeFormat in _timeFormats)
                {
                    if (DateTime.TryParseExact(text, String.Format(CultureInfo.InvariantCulture, "{0} {1}", dateFormat, timeFormat), CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                    {
                        return result;
                    }
                }
            }

            // test using date only
            foreach (string dateFormat in _dateFormats)
            {
                if (DateTime.TryParseExact(text, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault, out result))
                {
                    return result;
                }
            }

            string invalidFormatMessage = string.Format(
                CultureInfo.CurrentCulture,
                "'{0}' is unable to convert '{1}' to '{2}'.",
                GetType().Name,
                text,
                typeof(DateTime).Name);
            throw new FormatException(invalidFormatMessage);
        }

        /// <summary>
        /// Converts a DateTime into a string.
        /// </summary>
        /// <param name="typeDescriptorContext">
        /// The type descriptor context.
        /// </param>
        /// <param name="cultureInfo">The culture used to convert.</param>
        /// <param name="value">
        /// The value that is being converted to a specified type.
        /// </param>
        /// <param name="destinationType">
        /// The type to convert the value to.
        /// </param>
        /// <returns>
        /// The value of the conversion to the specified type.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "Compat with WPF.")]
        [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "1#", Justification = "Compat with WPF.")]
        public override object ConvertTo(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value == null)
                {
                    return String.Empty;
                }
                else if (value is DateTime)
                {
                    DateTime time = (DateTime)value;
                    return time.ToString("HH:mm:ss", new CultureInfo("en-US"));
                }
            }

            return TypeConverters.ConvertTo(this, value, destinationType);
        }
    }
}