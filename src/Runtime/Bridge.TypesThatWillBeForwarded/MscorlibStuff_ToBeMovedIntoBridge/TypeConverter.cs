using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel
{
    public class TypeConverter
    {
        public bool CanConvertFrom(Type sourceType)
        {
            return CanConvertFrom(null, sourceType);
        }

        public virtual bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public virtual bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public object ConvertFrom(object value)
        {
            return ConvertFrom(null, CultureInfo.CurrentCulture, value);
        }

        public virtual object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            throw new NotImplementedException();
        }

        public object ConvertTo(object value, Type destinationType)
        {
            return ConvertTo(null, null, value, destinationType);
        }

        public virtual object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }

            if (destinationType == typeof(string))
            {
                if (value == null)
                {
                    return String.Empty;
                }

                // Pre-whidbey we just did a ToString() here.  To minimize the chance of a breaking change we
                // still send requests for the CurrentCulture to ToString() (which should return the same).
                if (culture != null && culture != CultureInfo.CurrentCulture)
                {
                    // VSWhidbey 75433 - If the object is IFormattable, use this interface to convert to string
                    // so we use the specified culture rather than the CurrentCulture like object.ToString() does.
                    IFormattable formattable = value as IFormattable;
                    if (formattable != null)
                    {
                        return formattable.ToString(/* format = */ null, /* formatProvider = */ culture);
                    }
                }
                return value.ToString();
            }
            throw GetConvertToException(value, destinationType);
        }

        public string ConvertToString(object value)
        {
            return (string)ConvertTo(null, CultureInfo.CurrentCulture, value, typeof(string));
        }

        public string ConvertToString(ITypeDescriptorContext context, object value)
        {
            return (string)ConvertTo(context, CultureInfo.CurrentCulture, value, typeof(string));
        }

        public string ConvertToString(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return (string)ConvertTo(context, culture, value, typeof(string));
        }

        protected Exception GetConvertFromException(object value)
        {
            string valueTypeName;

            if (value == null)
            {
                //valueTypeName = SR.GetString(SR.ToStringNull);
                throw new NotImplementedException();
            }
            else
            {
                valueTypeName = value.GetType().FullName;
            }

            throw new NotSupportedException("");
        }

        protected Exception GetConvertToException(object value, Type destinationType)
        {
            string valueTypeName;

            if (value == null)
            {
                //valueTypeName = SR.GetString(SR.ToStringNull);
                throw new NotImplementedException();
            }
            else
            {
                valueTypeName = value.GetType().FullName;
            }

            throw new NotSupportedException("");
        }
    }
}
