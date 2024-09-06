
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System.Reflection;

namespace System.ComponentModel
{
    internal sealed class ReflectedPropertyData
    {
        private readonly string _name;                     // the property name
        private readonly Type _componentClass;             // used to determine if we should all on us or on the designer
        private readonly Type _type;                       // the data type of the property
        private readonly PropertyInfo _propInfo;           // the property info
        private readonly MethodInfo _getMethod;            // the property get method
        private readonly MethodInfo _setMethod;            // the property set method
        private Attribute[] _attributes;                   // the property attributes

        private TypeConverter _converter;                  // the type converter for this property. Do not include the converter of this property's type
        private TypeConverter _propertyTypeConverter;      // the type converter for this property's type. remains null if we have converter from custom attributes

        private readonly object lockCookie = new object();

        public ReflectedPropertyData(
            Type componentClass,
            string name,
            Type type,
            PropertyInfo propInfo,
            MethodInfo getMethod,
            MethodInfo setMethod)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (name.Length == 0)
            {
                throw new ArgumentException(nameof(name));
            }
            if (componentClass == null)
            {
                throw new ArgumentNullException(nameof(componentClass));
            }
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            _name = name;
            _componentClass = componentClass;
            _type = type;
            _propInfo = propInfo;
            _getMethod = getMethod;
            _setMethod = setMethod;
        }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        public string Name => _name ?? "";

        /// <summary>
        /// Retrieves the type of the component this PropertyDescriptor is bound to.
        /// </summary>
        public Type ComponentType => _componentClass;

        /// <summary>
        /// Retrieves the type of the property.
        /// </summary>
        public Type PropertyType => _type;

        public Attribute[] Attributes
        {
            get
            {
                if (_attributes == null)
                {
                    lock (lockCookie)
                    {
                        _attributes = Attribute.GetCustomAttributes(_propInfo, false);
                    }
                }

                return _attributes;
            }
        }

        /// <summary>
        /// Gets the type converter for this property.
        /// </summary>
        public TypeConverter Converter
        {
            get
            {
                TypeConverter converter = InternalConverter;
                if (converter != null)
                {
                    return converter;
                }

                _propertyTypeConverter ??= TypeConverterHelper.GetConverter(PropertyType) ?? TypeConverterHelper.NullConverter;

                return _propertyTypeConverter == TypeConverterHelper.NullConverter ? null : _propertyTypeConverter;
            }
        }

        internal PropertyInfo PropertyInfo => _propInfo;

        /// <summary>
        /// Gets the type converter for this property.
        /// Do not include any type converter of the property type.
        /// </summary>
        /// <remarks>
        /// This property is used by the compiler to avoid doing too many
        /// string conversions at runtime and should not be used in most
        /// cases. Use <see cref="Converter"/> instead.
        /// </remarks>
        internal TypeConverter InternalConverter
        {
            get
            {
                if (_converter == null)
                {
                    if (TypeConverterHelper.FindAttributeByType<TypeConverterAttribute>(Attributes) is TypeConverterAttribute typeAttr)
                    {
                        Type converterType = GetTypeFromName(typeAttr.ConverterTypeName);
                        if (converterType != null && typeof(TypeConverter).IsAssignableFrom(converterType))
                        {
                            _converter = (TypeConverter)Activator.CreateInstance(converterType);
                        }
                    }

                    _converter ??= TypeConverterHelper.NullConverter;
                }

                return _converter == TypeConverterHelper.NullConverter ? null : _converter;
            }
        }

        private Type GetTypeFromName(string typeName)
        {
            if (typeName == null || typeName.Length == 0)
            {
                return null;
            }

            //  try the generic method.
            Type typeFromGetType = Type.GetType(typeName);

            // If we didn't get a type from the generic method, or if the assembly we found the type
            // in is the same as our Component's assembly, use or Component's assembly instead. This is
            // because the CLR may have cached an older version if the assembly's version number didn't change
            Type typeFromComponent = null;
            if (ComponentType != null)
            {
                if ((typeFromGetType == null) ||
                    (ComponentType.Assembly.FullName.Equals(typeFromGetType.Assembly.FullName)))
                {
                    int comma = typeName.IndexOf(',');

                    if (comma != -1)
                        typeName = typeName.Substring(0, comma);

                    typeFromComponent = ComponentType.Assembly.GetType(typeName);
                }
            }

            return typeFromComponent ?? typeFromGetType;
        }
    }
}
