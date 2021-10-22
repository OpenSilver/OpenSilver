// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace System.ComponentModel
{
    /// <summary>
    /// Provides a type converter to convert <see cref='System.Enum'/> objects to and
    /// from various other representations
    /// </summary>
    public class EnumConverter : TypeConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref='System.ComponentModel.EnumConverter'/> class for the given
        /// type.
        /// </summary>
        public EnumConverter(Type type)
        {
            EnumType = type;
        }

        protected Type EnumType { get; }

        /// <summary>
        /// Gets a value indicating whether this converter can convert an object in the given
        /// source type to an enumeration object using the specified context.
        /// </summary>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string) || sourceType == typeof(Enum[]))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Gets a value indicating whether this converter can convert an object to the
        /// given destination type using the context.
        /// </summary>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(Enum[]))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        private static long GetEnumValue(bool isUnderlyingTypeUInt64, Enum enumVal, CultureInfo culture)
        {
            return isUnderlyingTypeUInt64 ?
                unchecked((long)Convert.ToUInt64(enumVal, culture)) :
                Convert.ToInt64(enumVal, culture);
        }

        /// <summary>
        /// Converts the specified value object to an enumeration object.
        /// </summary>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string strValue)
            {
                try
                {
                    if (strValue.Contains(","))
                    {
                        bool isUnderlyingTypeUInt64 = false;
                        long convertedValue = 0;
                        string[] values = strValue.Split(',');
                        foreach (string v in values)
                        {
                            convertedValue |= GetEnumValue(isUnderlyingTypeUInt64, (Enum)Enum.Parse(EnumType, v, true), culture);
                        }
                        return Enum.ToObject(EnumType, convertedValue);
                    }
                    else
                    {
                        return Enum.Parse(EnumType, strValue, true);
                    }
                }
                catch (Exception e)
                {
                    throw new FormatException(string.Format("{0} is not a valid value for {1}.", (string)value, EnumType.Name), e);
                }
            }
            else if (value is Enum[])
            {
                bool isUnderlyingTypeUInt64 = false;
                long finalValue = 0;
                foreach (Enum e in (Enum[])value)
                {
                    finalValue |= GetEnumValue(isUnderlyingTypeUInt64, e, culture);
                }
                return Enum.ToObject(EnumType, finalValue);
            }
            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Converts the given value object to the specified destination type.
        /// </summary>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }

            if (destinationType == typeof(string) && value != null)
            {
                // Raise an argument exception if the value isn't defined and if
                // the enum isn't a flags style.
                if (!IsAttributeDefined(EnumType, "System.FlagsAttribute", false) && !Enum.IsDefined(EnumType, value))
                {
                    throw new ArgumentException(string.Format("The value '{0}' is not a valid value for the enum '{1}'.", value, EnumType.Name));
                }

                return Enum.Format(EnumType, value, "G");
            }

            if (destinationType == typeof(Enum[]) && value != null)
            {
                if (IsAttributeDefined(EnumType, "System.FlagsAttribute", false))
                {
                    bool isUnderlyingTypeUInt64 = false;
                    List<Enum> flagValues = new List<Enum>();

                    Array objValues = Enum.GetValues(EnumType);
                    long[] ulValues = new long[objValues.Length];
                    for (int idx = 0; idx < objValues.Length; idx++)
                    {
                        ulValues[idx] = GetEnumValue(isUnderlyingTypeUInt64, (Enum)objValues.GetValue(idx), culture);
                    }

                    long longValue = GetEnumValue(isUnderlyingTypeUInt64, (Enum)value, culture);
                    bool valueFound = true;
                    while (valueFound)
                    {
                        valueFound = false;
                        foreach (long ul in ulValues)
                        {
                            if ((ul != 0 && (ul & longValue) == ul) || ul == longValue)
                            {
                                flagValues.Add((Enum)Enum.ToObject(EnumType, ul));
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
                        flagValues.Add((Enum)Enum.ToObject(EnumType, longValue));
                    }

                    return flagValues.ToArray();
                }
                else
                {
                    return new Enum[] { (Enum)Enum.ToObject(EnumType, value) };
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>
        /// Gets a value indicating whether the given object value is valid for this type.
        /// </summary>
        public override bool IsValid(ITypeDescriptorContext context, object value) => Enum.IsDefined(EnumType, value); 

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