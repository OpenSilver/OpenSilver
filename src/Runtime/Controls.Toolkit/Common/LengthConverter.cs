// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Controls;
using Resource = OpenSilver.Controls.Toolkit.Resources;

namespace System.Windows
{
    /// <summary>
    /// Converts instances of other types to and from instances of a double that
    /// represent an object measurement such as a height or width.
    /// </summary>
    /// <QualityBand>Stable</QualityBand>
    public partial class LengthConverter : TypeConverter
    {
        /// <summary>
        /// Conversions from units to pixels.
        /// </summary>
        private static Dictionary<string, double> UnitToPixelConversions = new Dictionary<string, double>
        {
            { "px", 1.0 },
            { "in", 96.0 },
            { "cm", 37.795275590551178 },
            { "pt", 1.3333333333333333 }
        };

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.LengthConverter" /> class.
        /// </summary>
        public LengthConverter()
        {
        }

        /// <summary>
        /// Determines whether conversion is possible from a specified type to a
        /// <see cref="T:System.Double" /> that represents an object
        /// measurement.
        /// </summary>
        /// <param name="typeDescriptorContext">
        /// An <see cref="T:System.ComponentModel.ITypeDescriptorContext" />
        /// that provides a format context.
        /// </param>
        /// <param name="sourceType">
        /// A <see cref="T:System.Type" /> that represents the type you want to
        /// convert from.
        /// </param>
        /// <returns>
        /// True if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext typeDescriptorContext, Type sourceType)
        {
            // Convert numeric types and strings
            switch (Type.GetTypeCode(sourceType))
            {
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.String:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Converts from the specified value to values of the
        /// <see cref="T:System.Double" /> type.
        /// </summary>
        /// <param name="typeDescriptorContext">
        /// An <see cref="T:System.ComponentModel.ITypeDescriptorContext" />
        /// that provides a format context.
        /// </param>
        /// <param name="cultureInfo">
        /// The <see cref="T:System.Globalization.CultureInfo" /> to use as the
        /// current culture.
        /// </param>
        /// <param name="source">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object source)
        {
            if (source == null)
            {
                string message = string.Format(
                    CultureInfo.CurrentCulture,
                    Resource.TypeConverters_ConvertFrom_CannotConvertFromType,
                    GetType().Name,
                    "null");
                throw new NotSupportedException(message);
            }

            string text = source as string;
            if (text != null)
            {
                // Convert Auto to NaN
                if (string.Compare(text, "Auto", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return double.NaN;
                }

                // Get the unit conversion factor
                string number = text;
                double conversionFactor = 1.0;
                foreach (KeyValuePair<string, double> conversion in UnitToPixelConversions)
                {
                    if (number.EndsWith(conversion.Key, StringComparison.Ordinal))
                    {
                        conversionFactor = conversion.Value;
                        number = text.Substring(0, number.Length - conversion.Key.Length);
                        break;
                    }
                }

                // Convert the value
                try
                {
                    return conversionFactor * Convert.ToDouble(number, cultureInfo);
                }
                catch (FormatException)
                {
                    string message = string.Format(
                        CultureInfo.CurrentCulture,
                        Resource.TypeConverters_Convert_CannotConvert,
                        GetType().Name,
                        text,
                        typeof(double).Name);
                    throw new FormatException(message);
                }
            }

            return Convert.ToDouble(source, cultureInfo);
        }

        /// <summary>
        /// Returns whether the type converter can convert a measurement to the
        /// specified type.
        /// </summary>
        /// <param name="typeDescriptorContext">
        /// An <see cref="T:System.ComponentModel.ITypeDescriptorContext" />
        /// that provides a format context.
        /// </param>
        /// <param name="destinationType">
        /// A <see cref="T:System.Type" /> that represents the type you want to
        /// convert to.
        /// </param>
        /// <returns>
        /// True if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext typeDescriptorContext, Type destinationType)
        {
            return TypeConverters.CanConvertTo<double>(destinationType);
        }

        /// <summary>
        /// Converts the specified measurement to the specified type.
        /// </summary>
        /// <param name="typeDescriptorContext">
        /// An object that provides a format context.
        /// </param>
        /// <param name="cultureInfo">
        /// The <see cref="T:System.Globalization.CultureInfo" /> to use as the
        /// current culture.
        /// </param>
        /// <param name="value">The value to convert.</param>
        /// <param name="destinationType">
        /// A <see cref="T:System.Type" /> that represents the type you want to
        /// convert to.
        /// </param>
        /// <returns>The converted value.</returns>
        public override object ConvertTo(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object value, Type destinationType)
        {
            // Convert the length to a String
            if (value is double)
            {
                double length = (double) value;
                if (destinationType == typeof(string))
                {
                    return double.IsNaN(length) ?
                        "Auto" :
                        Convert.ToString(length, cultureInfo);
                }
            }

            return TypeConverters.ConvertTo(this, value, destinationType);
        }
    }
}