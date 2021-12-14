using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TestApplication.OpenSilver.Converters
{
    /// <summary>
    /// A type converter for visibility and boolean values.
    /// </summary>
    public class BooleanVisibilityConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            var visibility = (bool)value;

            return visibility ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            return ((Visibility)value == Visibility.Visible);
        }
    }
}
