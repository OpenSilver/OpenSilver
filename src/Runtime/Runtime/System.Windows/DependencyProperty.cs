
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

using System;
using System.Collections.Generic;
using CSHTML5.Internal;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Represents a dependency property that is registered with the dependency property
    /// system. Dependency properties provide support for value expressions, data
    /// binding, animation, and property change notification.
    /// </summary>
    public partial class DependencyProperty
    {
        private static readonly Type nullableType = typeof(Nullable<>);

        public static readonly object UnsetValue = new INTERNAL_NamedObject("DependencyProperty.UnsetValue");

        internal string Name { get; set; }
        internal Type PropertyType { get; set; }
        internal Type OwnerType { get; set; }
        private bool _isAttached = false;
        public bool IsAttached
        {
            get { return _isAttached; }
        }

        private PropertyMetadata _defaultMetadata;

        Dictionary<Type, PropertyMetadata> _typesToOverridenMetadatas = null; //this is the same as Optimization_typesToOverrides except that this one contains only the types that called the OverrideMetaData method.
        Dictionary<Type, bool> Optimization_typesWithoutOverride = null; //todo: replace with a hashset when possible.
        Dictionary<Type, PropertyMetadata> Optimization_typesToOverrides = null; //note: see comment on _typesToOverridenMetadatas

        public PropertyMetadata GetMetadata(Type type)
        {
            return this.GetTypeMetaData(type);
        }

        internal PropertyMetadata GetTypeMetaData(Type typeOfOwner)
        {
#if PERFSTAT
            var t0 = Performance.now();
#endif
            List<Type> typesWentThrough = null; //we want to keep the types through which we went so that we can add them to the corresponding dictionary (Optimization_typesToOverrides or Optimization_typesWithoutOverride) when finished.
            if (Optimization_typesToOverrides != null) //note: if this dictionary is null, it means that the given Metadata has not been overriden by anything.
            {
                //we check if the metadata is overriden for the current type:
                if (Optimization_typesToOverrides.ContainsKey(typeOfOwner))
                {
#if PERFSTAT
                    Performance.Counter("DependencyProperty.GetTypeMetaData", t0);
#endif
                    //the type is already known for having an override on this property:
                    return Optimization_typesToOverrides[typeOfOwner];
                }
                else if (Optimization_typesWithoutOverride != null && Optimization_typesWithoutOverride.ContainsKey(typeOfOwner))
                {
#if PERFSTAT
                    Performance.Counter("DependencyProperty.GetTypeMetaData", t0);
#endif
                    //the type is already known for NOT having an override on this property:
                    return _defaultMetadata;
                }
                //if we arrive here, it means that we currently do not know if the type has an override for the dependency property.
                Type currentType = typeOfOwner;
                typesWentThrough = new List<Type>(); //we want to keep the types through which we went so that we can add them to the corresponding dictionary (Optimization_typesToOverrides or Optimization_typesWithoutOverride) when finished.
                while (currentType != OwnerType && !Optimization_typesToOverrides.ContainsKey(currentType))
                {
                    typesWentThrough.Add(currentType);
                    currentType = currentType.BaseType;
                }
                if (currentType != OwnerType)
                {
                    //a parent type has been found in the types overriding the metadata.
                    //we add the types through which we went to the dictionary containing the correspondance between types and metadata:
                    foreach (Type type in typesWentThrough)
                    {
                        Optimization_typesToOverrides.Add(type, Optimization_typesToOverrides[currentType]);
                    }
#if PERFSTAT
                    Performance.Counter("DependencyProperty.GetTypeMetaData", t0);
#endif
                    //we return the Metadata:
                    return Optimization_typesToOverrides[currentType];
                }
            }
            //the metadata has either not been overriden by anything or not by a parent of the asked type.
            if (typesWentThrough != null)
            {
                if (Optimization_typesWithoutOverride == null)
                {
                    Optimization_typesWithoutOverride = new Dictionary<Type, bool>();
                }
                //we add the types through which we went to the dictionary containing the types with no override on the metadata:
                foreach (Type type in typesWentThrough)
                {
                    if (!Optimization_typesWithoutOverride.ContainsKey(type)) //todo: there has been an error when without this test but I haven't really looked into it so it might come from an error somewhere else.
                    {
                        Optimization_typesWithoutOverride.Add(type, false);// false because why not?
                    }
                }
            }
#if PERFSTAT
            Performance.Counter("DependencyProperty.GetTypeMetaData", t0);
#endif
            return _defaultMetadata;
        }

        public static DependencyProperty RegisterAttached(string name, Type propertyType, Type ownerType, PropertyMetadata typeMetadata)
        {
            DependencyProperty property = Register(name, propertyType, ownerType, typeMetadata);
            property._isAttached = true;
            return property;
        }

        private static void EnsureDefaultValue(PropertyMetadata typeMetadata, Type propertyType, string name, Type ownerType)
        {
#if BRIDGE || NETSTANDARD // We exclude the following code in the JSIL version, because of issues in JSIL comparing "1.0" and "System.Double" (it says that they are not the same type when running in the browser)
            if (typeMetadata.IsDefaultValueModified)
            {
                if (!IsValueTypeValid(typeMetadata.DefaultValue, propertyType))
                {
                    string message = $"Default value type ({(typeMetadata.DefaultValue == null ? "null" : typeMetadata.DefaultValue.GetType().FullName)}) does not match property type ({propertyType.FullName}). To fix this issue, please change the default value of the dependency property named '{name}' in the type '{ownerType}' so that it matches the type of the property.";
                    if (Application.Current.Host.Settings.EnableInvalidPropertyMetadataDefaultValueExceptions)
                    {
                        throw new ArgumentException(message);
                    }
                    else
                    {
                        var defaultValue = CreateDefaultValue(propertyType);
                        typeMetadata.DefaultValue = defaultValue;
                        Console.WriteLine(message + Environment.NewLine + string.Format("The default value has been automatically set to '{0}'.", defaultValue));
                    }
                }
            }
            else
            {
                typeMetadata.DefaultValue = CreateDefaultValue(propertyType);
            }
#endif
        }

        internal static bool IsValueTypeValid(object value, Type type)
        {
            if (object.ReferenceEquals(value, DependencyProperty.UnsetValue))
            {
                return false;
            }
            else
            {
                if (value == null)
                {
                    // Null values are invalid for value-types
                    if (type.IsValueType && !(type.IsGenericType && type.GetGenericTypeDefinition() == nullableType))
                    {
                        return false;
                    }
                }
                else
                {
                    // Non-null default value, ensure its the correct type
#if !BRIDGE && !NETSTANDARD // This is the JSIL version
                    if (!value.GetType().IsAssignableFrom(type))
#else
                    if (!type.IsInstanceOfType(value))
#endif
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Registers a dependency property with the specified property name, property type, owner type, and property metadata.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="propertyType">The type of the property.</param>
        /// <param name="ownerType">The type of the property's owner.</param>
        /// <param name="typeMetadata">The property metadata.</param>
        /// <returns>The DependencyProperty.</returns>
        public static DependencyProperty Register(string name, Type propertyType, Type ownerType, PropertyMetadata typeMetadata)
        {
#if PERFSTAT
            var t = Performance.now();
#endif
            PropertyMetadata defaultMetadata = typeMetadata;
            if (defaultMetadata == null)
            {
                //Create metadata if not set
                defaultMetadata = new PropertyMetadata();
            }
            // Make sure typeMetadata default value is valid.
            EnsureDefaultValue(defaultMetadata, propertyType, name, ownerType);

            defaultMetadata.Seal();

            var newDependencyProperty = new DependencyProperty()
            {
                Name = name,
                PropertyType = propertyType,
                OwnerType = ownerType,
                _defaultMetadata = defaultMetadata
            };

            INTERNAL_TypeToStringsToDependencyProperties.Register(newDependencyProperty);

            // Add the dependency property to the list that is used to know whether to always call "PropertyChanged" when the UI element is loaded into the Visual Tree:
            if (typeMetadata != null && typeMetadata.CallPropertyChangedWhenLoadedIntoVisualTree == WhenToCallPropertyChangedEnum.Always)
            {
                INTERNAL_TypeToDependencyPropertiesThatRequirePropertyChanged.Add(ownerType, newDependencyProperty);
            }

#if PERFSTAT
            Performance.Counter("DependencyProperty.Register", t);
#endif
            return newDependencyProperty;
        }

        /// <summary>
        /// Returns the DependencyProperty as a string.
        /// </summary>
        /// <returns>The DependencyProperty as a string.</returns>
        public override string ToString()
        {
            // Useful for easier debugging:
            if (OwnerType != null && Name != null)
                return OwnerType.Name + "." + Name + "Property";
            else
                return base.ToString();
        }

        private void PrepareOverrideMetadata(Type newOwnerType, PropertyMetadata typeMetadata)
        {
            if (newOwnerType == null)
            {
                throw new ArgumentNullException("newOwnerType");
            }
            if (typeMetadata == null)
            {
                throw new ArgumentNullException("typeMetadata");
            }
            //Default value has been specified when creating the typeMetadata.
            //We need to make sure it's type is correct.
            if (typeMetadata.IsDefaultValueModified)
            {
                if (!IsValueTypeValid(typeMetadata.DefaultValue, PropertyType))
                {
                    throw new ArgumentException(string.Format("Default value type does not match type of property. To fix this issue, please change the default value of the dependency property named '{0}' in the type '{1}' so that it matches the type of the property.", this.Name, this.OwnerType.ToString()));
                }
            }
            //todo: check that newOnwerType inherit from the base metadata owner type.
        }


        public void OverrideMetadata(Type newOwnerType, PropertyMetadata typeMetadata)
        {
            // Validate parameters.
            PrepareOverrideMetadata(newOwnerType, typeMetadata);

            // Make sure typeMetadata default value is set.
            if (!typeMetadata.IsDefaultValueModified)
            {
                typeMetadata.DefaultValue = CreateDefaultValue(PropertyType);
            }

            typeMetadata.Seal();

            if (_typesToOverridenMetadatas == null)
            {
                _typesToOverridenMetadatas = new Dictionary<Type, PropertyMetadata>();
            }
            _typesToOverridenMetadatas.Add(newOwnerType, typeMetadata);

            //We add the element in the dictionary used for optimizations:
            if (Optimization_typesToOverrides == null)
            {
                Optimization_typesToOverrides = new Dictionary<Type, PropertyMetadata>();
            }
            Optimization_typesToOverrides.Add(newOwnerType, typeMetadata);
        }

        private static object CreateDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}
