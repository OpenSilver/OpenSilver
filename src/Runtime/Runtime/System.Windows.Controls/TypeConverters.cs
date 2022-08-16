// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Common TypeConverter functionality.
    /// </summary>
    internal static partial class TypeConverters
    {
        /// <summary>
        /// Returns a value indicating whether this converter can convert an
        /// object of the given type to an instance of the expected type.
        /// </summary>
        /// <typeparam name="T">Expected type of the converter.</typeparam>
        /// <param name="sourceType">
        /// The type of the source that is being evaluated for conversion.
        /// </param>
        /// <returns>
        /// A value indicating whether the converter can convert the provided
        /// type.
        /// </returns>
        internal static bool CanConvertFrom<T>(Type sourceType)
        {
            if (sourceType == null)
            {
                throw new ArgumentNullException("sourceType");
            }
            return (sourceType == typeof(string)) ||
                typeof(T).IsAssignableFrom(sourceType);
        }

        /// <summary>
        /// Attempts to convert a specified object to an instance of the
        /// expected type.
        /// </summary>
        /// <typeparam name="T">Expected type of the converter.</typeparam>
        /// <param name="converter">TypeConverter instance.</param>
        /// <param name="value">The object being converted.</param>
        /// <returns>
        /// The instance of the expected type created from the converted object.
        /// </returns>
        internal static object ConvertFrom<T>(TypeConverter converter, object value)
        {
            Debug.Assert(converter != null, "converter should not be null!");

            if (value is T)
            {
                // There's nothing to convert if it's already the correct type
                return value;
            }
            else
            {
                // Otherwise throw an error
                throw new NotSupportedException(string.Format(
                    CultureInfo.CurrentCulture,
                    "'{0}' cannot convert from '{1}'.",
                    converter.GetType().Name,
                    value != null ? value.GetType().FullName : "(null)"));
            }
        }

        /// <summary>
        /// Determines whether conversion is possible to a specified type.
        /// </summary>
        /// <typeparam name="T">Expected type of the converter.</typeparam>
        /// <param name="destinationType">
        /// Identifies the data type to evaluate for conversion.
        /// </param>
        /// <returns>
        /// A value indicating whether conversion is possible.
        /// </returns>
        internal static bool CanConvertTo<T>(Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            return (destinationType == typeof(string)) ||
                destinationType.IsAssignableFrom(typeof(T));
        }

        /// <summary>
        /// Attempts to convert a specified object to an instance of the
        /// desired type.
        /// </summary>
        /// <param name="converter">TypeConverter instance.</param>
        /// <param name="value">The object being converted.</param>
        /// <param name="destinationType">
        /// The type to convert the value to.
        /// </param>
        /// <returns>
        /// The value of the conversion to the specified type.
        /// </returns>
        internal static object ConvertTo(TypeConverter converter, object value, Type destinationType)
        {
            Debug.Assert(converter != null, "converter should not be null!");

            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }

            // Just return the value if it is already an instance of the
            // destination type
            if (value == null && !destinationType.IsValueType)
            {
                return null;
            }
            else if (value != null && destinationType.IsAssignableFrom(value.GetType()))
            {
                return value;
            }

            // Otherwise throw an error
            throw new NotSupportedException(string.Format(
                CultureInfo.CurrentCulture,
                "'{0}' is unable to convert '{1}' to '{2}'.",
                converter.GetType().Name,
                value != null ? value.GetType().FullName : "(null)",
                destinationType.GetType().Name));
        }
    }
}