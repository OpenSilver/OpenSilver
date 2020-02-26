
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
#if MIGRATION
using CSHTML5.Internal;
#endif

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
#if FOR_DESIGN_TIME
    [TypeConverter(typeof(DependencyPropertyConverter))]
#endif
    public partial class DependencyProperty
    {
        private static readonly Type nullableType;
        private static readonly INTERNAL_DefaultValueProvider defaultValueProvider;

        public static readonly object UnsetValue = INTERNAL_NoValue.NoValue;

        internal string Name { get; set; }
        internal Type PropertyType { get; set; }
        internal Type OwnerType { get; set; }
        private bool _isAttached = false;
        public bool IsAttached
        {
            get { return _isAttached; }
        }

        private PropertyMetadata _typeMetadata { get; set; } //note: this is now private because we now use GetTypeMetadata

        Dictionary<Type, PropertyMetadata> _typesToOverridenMetadatas = null; //this is the same as Optimization_typesToOverrides except that this one contains only the types that called the OverrideMetaData method.
        Dictionary<Type, bool> Optimization_typesWithoutOverride = null; //todo: replace with a hashset when possible.
        Dictionary<Type, PropertyMetadata> Optimization_typesToOverrides = null; //note: see comment on _typesToOverridenMetadatas

        static DependencyProperty()
        {
            nullableType = typeof(Nullable<>);
            defaultValueProvider = new INTERNAL_DefaultValueProvider();
        }

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
                    return _typeMetadata;
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
            return _typeMetadata;
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
                    string message = string.Format("Default value type does not match type of property. To fix this issue, please change the default value of the dependency property named '{0}' in the type '{1}' so that it matches the type of the property.", name, ownerType.ToString());
                    if (Application.Current.Host.Settings.EnableInvalidPropertyMetadataDefaultValueExceptions)
                    {
                        throw new ArgumentException(message);
                    }
                    else
                    {
                        var defaultValue = defaultValueProvider.ProvideValue(propertyType);
                        typeMetadata.DefaultValue = defaultValue;
                        Console.WriteLine(message + Environment.NewLine + string.Format("The default value has been automatically set to '{0}'.", defaultValue));
                    }
                }
            }
            else
            {
                typeMetadata.DefaultValue = defaultValueProvider.ProvideValue(propertyType);
            }
#endif
        }

        private static bool IsValueTypeValid(object value, Type type)
        {
            if (object.ReferenceEquals(value, INTERNAL_NoValue.NoValue))
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

            var newDependencyProperty = new DependencyProperty()
            {
                Name = name,
                PropertyType = propertyType,
                OwnerType = ownerType,
                _typeMetadata = defaultMetadata
                //Store = INTERNAL_PropertyStore.Instance
            };

            // Add the dependency property to the list of all the dependency properties of the object:
            INTERNAL_TypeToDependencyProperties.Add(ownerType, newDependencyProperty);

            // Add the dependency property to the list that is used to know whether to always call "PropertyChanged" when the UI element is loaded into the Visual Tree:
            if (typeMetadata != null && typeMetadata.CallPropertyChangedWhenLoadedIntoVisualTree == WhenToCallPropertyChangedEnum.Always)
            {
                INTERNAL_TypeToDependencyPropertiesThatRequirePropertyChanged.Add(ownerType, newDependencyProperty);
            }

            //Add the dependencyProperty's name to the dictionary that allows to get the dependencyProperty from its name:
            Dictionary<string, DependencyProperty> stringsToDependencyProperties = INTERNAL_TypeToStringsToDependencyProperties.GetDictionaryForType(ownerType);
            if (stringsToDependencyProperties.ContainsKey(name))
            {
#if !MIGRATION
                // THE FOLLOWING CHECK IS DISABLED IN THE SILVERLIGHT COMPATIBLE VERSION
                // BECAUSE IT APPEARS THAT SILVERLIGHT IS TOLERANT TO DECLARING TWICE
                // THE SAME DEPENDENCY PROPERTY OR ATTACHED PROPERTY. FOR AN EXAMPLE OF
                // USE, SEE THE CLASS "RatingsView" IN THE CLIENT APPLICATION "STAR".
                if (stringsToDependencyProperties[name] != null)
                {
                    throw new Exception("Cannot register multiple properties with the same PropertyName");
                }
#endif
                stringsToDependencyProperties[name] = newDependencyProperty;
            }
            else
            {
                stringsToDependencyProperties.Add(name, newDependencyProperty);
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
                typeMetadata.DefaultValue = defaultValueProvider.ProvideValue(PropertyType);
            }
            //EnsureDefaultValue(typeMetadata, PropertyType);

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
    }
}
