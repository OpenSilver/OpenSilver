using System;
using System.Globalization;

namespace System.ComponentModel
{
    public class UriTypeConverter : TypeConverter
    {
        private UriKind m_UriKind;


        public UriTypeConverter() : this(UriKind.RelativeOrAbsolute) { }

        internal UriTypeConverter(UriKind uriKind)
        {
            m_UriKind = uriKind;
        }


        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == null)
                throw new ArgumentNullException("sourceType");

            if (sourceType == typeof(string))
                return true;

            if (typeof(Uri).IsAssignableFrom(sourceType))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }
        //
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;

            if (destinationType == typeof(Uri))
                return true;

            return base.CanConvertTo(context, destinationType);
        }
        //
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string uriString = value as string;
            if (uriString != null)
                return new Uri(uriString, m_UriKind);

            Uri uri = value as Uri;
            if (uri != null)
                return new Uri(uri.OriginalString,
                    m_UriKind == UriKind.RelativeOrAbsolute ? uri.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative : m_UriKind);

            return base.ConvertFrom(context, culture, value);
        }
        //
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            Uri uri = value as Uri;

            if (uri != null && destinationType == typeof(string))
                return uri.OriginalString;

            if (uri != null && destinationType == typeof(Uri))
                return new Uri(uri.OriginalString,
                    m_UriKind == UriKind.RelativeOrAbsolute ? uri.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative : m_UriKind);

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
