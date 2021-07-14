using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.Foundation
#endif
{
    /// <summary>
    /// 
    /// </summary>
    public class PointTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var result = default(Point);

            if (value is string exp)
            {
                var split = exp.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (split.Length == 2)
                {
                    double.TryParse(split[0], out var x);
                    double.TryParse(split[1], out var y);
                    
                    result = new Point(x, y);
                }
                else
                {
                    throw new FormatException($"The {nameof(value)} was not in the expected format: \"x, y\"");
                }

            }
            else
            {
                throw new NotSupportedException($"Conversion from {value.GetType().FullName} is not supported.");
            }

            return result;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(string))
            {
                throw new NotSupportedException($"Conversion to {destinationType.FullName} is not supported.");
            }
            
            return value.ToString();
        }
    }
}
