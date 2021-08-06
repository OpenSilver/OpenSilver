using System;
using System.Collections.Generic;
using System.Globalization;

namespace System.ComponentModel
{
    public class EnumConverter : TypeConverter
    {
        /// <devdoc>
        ///    <para>
        ///       Specifies
        ///       the
        ///       type of the enumerator this converter is
        ///       associated with.
        ///    </para>
        /// </devdoc>
        private Type type;

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.ComponentModel.EnumConverter'/> class for the given
        ///       type.
        ///    </para>
        /// </devdoc>
        public EnumConverter(Type type)
        {
            this.type = type;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        protected Type EnumType
        {
            get
            {
                return type;
            }
        }

        /// <internalonly/>
        /// <devdoc>
        ///    <para>Gets a value indicating whether this converter
        ///       can convert an object in the given source type to an enumeration object using
        ///       the specified context.</para>
        /// </devdoc>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string) || sourceType == typeof(Enum[]))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        /// <devdoc>
        ///    <para>Gets a value indicating whether this converter can
        ///       convert an object to the given destination type using the context.</para>
        /// </devdoc>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(Enum[]))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        /// <internalonly/>
        /// <devdoc>
        ///    <para>Converts the specified value object to an enumeration object.</para>
        /// </devdoc>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                try
                {
                    string strValue = (string)value;
                    if (strValue.IndexOf(',') != -1)
                    {
                        long convertedValue = 0;
                        string[] values = strValue.Split(new char[] { ',' });
                        foreach (string v in values)
                        {
                            convertedValue |= Convert.ToInt64((Enum)Enum.Parse(type, v, true), culture);
                        }
                        return Enum.ToObject(type, convertedValue);
                    }
                    else
                    {
                        return Enum.Parse(type, strValue, true);
                    }
                }
                catch (Exception e)
                {
                    throw new FormatException(string.Format("{0} is not a valid value for {1}.", (string)value, type.Name), e);
                }
            }
            else if (value is Enum[])
            {
                long finalValue = 0;
                foreach (Enum e in (Enum[])value)
                {
                    finalValue |= Convert.ToInt64(e, culture);
                }
                return Enum.ToObject(type, finalValue);
            }
            return base.ConvertFrom(context, culture, value);
        }

        /// <internalonly/>
        /// <devdoc>
        ///    <para>Converts the given
        ///       value object to the
        ///       specified destination type.</para>
        /// </devdoc>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1803:AvoidCostlyCallsWherePossible")]  // Keep call to Enum.IsDefined
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }

            if (destinationType == typeof(string) && value != null)
            {
                if (!IsAttributeDefined(type, "System.FlagsAttribute", false) && !Enum.IsDefined(type, value))
                {
                    throw new ArgumentException(string.Format("The value '{0}' is not a valid value for the enum '{1}'.", value.ToString(), type.Name));
                }

                return Enum.Format(type, value, "G");
            }
            if (destinationType == typeof(Enum[]) && value != null)
            {
                if (IsAttributeDefined(type, "System.FlagsAttribute", false))
                {
                    List<Enum> flagValues = new List<Enum>();

                    Array objValues = Enum.GetValues(type);
                    long[] ulValues = new long[objValues.Length];
                    for (int idx = 0; idx < objValues.Length; idx++)
                    {
                        ulValues[idx] = Convert.ToInt64((Enum)objValues.GetValue(idx), culture);
                    }

                    long longValue = Convert.ToInt64((Enum)value, culture);
                    bool valueFound = true;
                    while (valueFound)
                    {
                        valueFound = false;
                        foreach (long ul in ulValues)
                        {
                            if ((ul != 0 && (ul & longValue) == ul) || ul == longValue)
                            {
                                flagValues.Add((Enum)Enum.ToObject(type, ul));
                                valueFound = true;
                                longValue &= ~ul;
                                break;
                            }
                        }

                        if (longValue == 0)
                        {
                            break;
                        }
                    }

                    if (!valueFound && longValue != 0)
                    {
                        flagValues.Add((Enum)Enum.ToObject(type, longValue));
                    }

                    return flagValues.ToArray();
                }
                else
                {
                    return new Enum[] { (Enum)Enum.ToObject(type, value) };
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        private static bool IsAttributeDefined(Type type, string attributeTypeFullName, bool inherit)
        {
            return IsAttributeDefined(type, Type.GetType(attributeTypeFullName), inherit);
        }

        private static bool IsAttributeDefined(Type type, Type attributeType, bool inherit)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (attributeType == null)
            {
                throw new ArgumentNullException(nameof(attributeType));
            }

            var attrs = type.GetCustomAttributes(attributeType, inherit);
            return attrs != null && attrs.Length > 0;
        }
    }
}
