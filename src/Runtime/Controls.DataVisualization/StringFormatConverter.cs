// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Globalization;


#if MIGRATION
using System.Windows.Data;
namespace System.Windows.Controls.DataVisualization
#else
using Windows.UI.Xaml.Data;
namespace Windows.UI.Xaml.Controls.DataVisualization
#endif
{
    /// <summary>
    /// Converts a value to a string using a format string.
    /// </summary>
    public class StringFormatConverter : IValueConverter
    {

#if MIGRATION
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
#else
        public object Convert(object value, Type targetType, object parameter, string language)
#endif
        {
            if (value == null)
            {
                return string.Empty;
            }

            return string.Format(CultureInfo.CurrentCulture, (parameter as string) ?? "{0}", value);
        }

#if MIGRATION
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
#else
        public object ConvertBack(object value, Type targetType, object parameter, string language)
#endif
        {
            throw new NotSupportedException();
        }
    }
}