//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Controls
{
    /// <summary>
    /// Type converter for DataForm.CommandButtonsVisibility.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class DataFormCommandButtonsVisibilityTypeConverter : TypeConverter
    {
        /// <summary>
        /// Returns whether we can convert from a given type.
        /// </summary>
        /// <param name="context">The type descriptor context.</param>
        /// <param name="sourceType">The source type.</param>
        /// <returns>Whether we can convert from the given type.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Converts a value from a type.
        /// </summary>
        /// <param name="context">The type descriptor context.</param>
        /// <param name="culture">The culture to use.</param>
        /// <param name="value">The value to convert from.</param>
        /// <returns>The converted value.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string strValue = value as string;

            if (strValue != null)
            {
                return Enum.Parse(typeof(DataFormCommandButtonsVisibility), strValue, true /* ignoreCase */);
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Converts a value to a type.
        /// </summary>
        /// <param name="context">The type descriptor context.</param>
        /// <param name="culture">The culture to use.</param>
        /// <param name="value">The value to convert from.</param>
        /// <param name="destinationType">The destination type.</param>
        /// <returns>The converted value.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return value.ToString();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
