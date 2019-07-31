
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
    public class DependencyProperty
    {
        public static readonly object UnsetValue = INTERNAL_NoValue.NoValue;

        internal static INTERNAL_DefaultValueStore DefaultValueStore = INTERNAL_DefaultValueStore.Instance;

        //internal INTERNAL_PropertyStore Store { get; set; }
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

        private static void EnsureDefaultValue(PropertyMetadata typeMetadata, Type propertyType)
        {
            if (typeMetadata != null)
            {
                if (typeMetadata.DefaultValueWasSet())
                {
                    object defaultValue;
                    if (!DefaultValueStore.EnsureDefaultValueIsValid(typeMetadata.DefaultValue, propertyType, out defaultValue))
                    {
                        typeMetadata.DefaultValue = defaultValue;
                    }
                }
                else
                {
                    typeMetadata.DefaultValue = DefaultValueStore.CreateDefaultValue(propertyType);
                }
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
            // Make sure typeMetadata default value is valid.
            EnsureDefaultValue(typeMetadata, propertyType);

            var newDependencyProperty = new DependencyProperty()
            {
                Name = name,
                PropertyType = propertyType,
                OwnerType = ownerType,
                _typeMetadata = typeMetadata
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

        public void OverrideMetadata(Type newOwnerType, PropertyMetadata typeMetadata)
        {
            // Make sure typeMetadata default value is not set to INTERNAL_NoValue.NoValue.
            EnsureDefaultValue(typeMetadata, PropertyType);

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
