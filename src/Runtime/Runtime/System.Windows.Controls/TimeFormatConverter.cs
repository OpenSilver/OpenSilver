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
    /// Converts from a string value to an ITimeFormat instance. Converts "Short" 
    /// to ShortTimeFormat, "Long" to LongTimeFormat and any other strings to 
    /// CustomTimeFormat. 
    /// </summary>
    public class TimeFormatConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether this instance can convert from the specified type 
        /// descriptor context.
        /// </summary>
        /// <param name="typeDescriptorContext">The type descriptor context.</param>
        /// <param name="sourceType">Type of the source.</param>
        /// <returns>
        /// 	<c>True</c> if this instance can convert from the specified 
        /// type descriptor context; otherwise, <c>false</c>.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "Compat with WPF.")]
        public override bool CanConvertFrom(ITypeDescriptorContext typeDescriptorContext, Type sourceType)
        {
            return Type.GetTypeCode(sourceType) == TypeCode.String;
        }

        /// <summary>
        /// Determines whether this instance can convert to the specified type 
        /// descriptor context.
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
        /// Converts instances of type string to an instance of type ITimeFormat.
        /// </summary>
        /// <param name="typeDescriptorContext">
        /// The type descriptor context.
        /// </param>
        /// <param name="cultureInfo">The culture used to convert.</param>
        /// <param name="source">
        /// The string that is converted.
        /// </param>
        /// <returns>
        /// An instance of ITimeFormat that is the value of the conversion.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "Compat with WPF.")]
        [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "1#", Justification = "Compat with WPF.")]
        [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "2#", Justification = "Compat with WPF.")]
        public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object source)
        {
            if (source == null)
            {
                string message = string.Format(
                    CultureInfo.CurrentCulture,
                    "'{0}' cannot convert from '{1}'.",
                    GetType().Name,
                    "null");
                throw new NotSupportedException(message);
            }

            string text = source as string;

            if (text != null)
            {
                if (string.Compare(text, "short", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return new ShortTimeFormat();
                }
                else if (string.Compare(text, "long", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return new LongTimeFormat();
                }
                else if (text.IndexOfAny(new[] { 'h', 'm', 's' }) < 0)
                {
                    string message = string.Format(
                        CultureInfo.CurrentCulture,
                        "'{0}' is unable to convert '{1}' to '{2}'.",
                        GetType().Name,
                        text,
                        typeof(ITimeFormat).Name);
                    throw new FormatException(message);
                }
                else
                {
                    return new CustomTimeFormat(text);
                }
            }
            else
            {
                string message = string.Format(
                    CultureInfo.CurrentCulture,
                    "'{0}' is unable to convert '{1}' to '{2}'.",
                    GetType().Name,
                    source,
                    typeof(ITimeFormat).Name);
                throw new InvalidCastException(message);
            }
        }

        /// <summary>
        /// Converts an known instance of type ITimeFormat to a string.
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
                if (value is ShortTimeFormat)
                {
                    return "Short";
                }

                if (value is LongTimeFormat)
                {
                    return "Long";
                }

                CustomTimeFormat customTimeFormat = value as CustomTimeFormat;
                if (customTimeFormat != null)
                {
                    return customTimeFormat.Format;
                }
            }

            return TypeConverters.ConvertTo(this, value, destinationType);
        }
    }
}
