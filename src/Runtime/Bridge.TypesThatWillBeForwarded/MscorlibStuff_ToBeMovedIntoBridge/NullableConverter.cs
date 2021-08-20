using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace System.ComponentModel
{
    public class NullableConverter : TypeConverter
    {
        Type nullableType;
        Type simpleType;
        TypeConverter simpleTypeConverter;

        /// <include file='doc\NullableConverter.uex' path='docs/doc[@for="NullableConverter.NullableConverter"]/*' />
        /// <devdoc>
        /// </devdoc>
        public NullableConverter(Type type)
        {
            this.nullableType = type;

            this.simpleType = Nullable.GetUnderlyingType(type);
            if (this.simpleType == null)
            {
                throw new ArgumentException("The specified type is not a nullable type.", "type");
            }

            this.simpleTypeConverter = TypeDescriptor.GetConverter(this.simpleType);
        }

        /// <include file='doc\NullableConverter.uex' path='docs/doc[@for="NullableConverter.CanConvertFrom"]/*' />
        /// <devdoc>
        /// </devdoc>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == this.simpleType)
            {
                return true;
            }
            else if (this.simpleTypeConverter != null)
            {
                return this.simpleTypeConverter.CanConvertFrom(context, sourceType);
            }
            else
            {
                return base.CanConvertFrom(context, sourceType);
            }
        }

        /// <include file='doc\NullableConverter.uex' path='docs/doc[@for="NullableConverter.ConvertFrom"]/*' />
        /// <devdoc>
        /// </devdoc>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null || value.GetType() == this.simpleType)
            {
                return value;
            }
            else if (value is String && String.IsNullOrEmpty(value as String))
            {
                return null;
            }
            else if (this.simpleTypeConverter != null)
            {
                object convertedValue = this.simpleTypeConverter.ConvertFrom(context, culture, value);
                return convertedValue;
            }
            else
            {
                return base.ConvertFrom(context, culture, value);
            }
        }

        /// <include file='doc\NullableConverter.uex' path='docs/doc[@for="NullableConverter.CanConvertTo"]/*' />
        /// <devdoc>
        /// </devdoc>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == this.simpleType)
            {
                return true;
            }
            else if (this.simpleTypeConverter != null)
            {
                return this.simpleTypeConverter.CanConvertTo(context, destinationType);
            }
            else
            {
                return base.CanConvertTo(context, destinationType);
            }
        }

        /// <include file='doc\NullableConverter.uex' path='docs/doc[@for="NullableConverter.ConvertTo"]/*' />
        /// <devdoc>
        /// </devdoc>
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }

            if (destinationType == this.simpleType && this.nullableType.IsInstanceOfType(value))
            {
                return value;
            }
            else if (value == null)
            {
                // Handle our own nulls here
                if (destinationType == typeof(string))
                {
                    return string.Empty;
                }
            }
            else if (this.simpleTypeConverter != null)
            {
                return this.simpleTypeConverter.ConvertTo(context, culture, value, destinationType);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <include file='doc\NullableConverter.uex' path='docs/doc[@for="NullableConverter.NullableType"]/*' />
        /// <devdoc>
        /// </devdoc>
        public Type NullableType
        {
            get
            {
                return nullableType;
            }
        }

        /// <include file='doc\NullableConverter.uex' path='docs/doc[@for="NullableConverter.UnderlyingType"]/*' />
        /// <devdoc>
        /// </devdoc>
        public Type UnderlyingType
        {
            get
            {
                return simpleType;
            }
        }

        /// <include file='doc\NullableConverter.uex' path='docs/doc[@for="NullableConverter.UnderlyingTypeConverter"]/*' />
        /// <devdoc>
        /// </devdoc>
        public TypeConverter UnderlyingTypeConverter
        {
            get
            {
                return simpleTypeConverter;
            }
        }
    }
}
