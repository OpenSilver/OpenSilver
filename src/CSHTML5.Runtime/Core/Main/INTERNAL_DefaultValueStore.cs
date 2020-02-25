using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSHTML5.Internal
{
    internal class INTERNAL_DefaultValueStore
    {
        private readonly Type nullableType;

        // Singleton
        internal static readonly INTERNAL_DefaultValueStore Instance;

        static INTERNAL_DefaultValueStore()
        {
            Instance = new INTERNAL_DefaultValueStore();
        }

        private INTERNAL_DefaultValueStore()
        {
            DefaultValuesCache = new Dictionary<Type, object>();
            nullableType = typeof(Nullable<>);
        }

        // Cache for default values.
        private Dictionary<Type, object> DefaultValuesCache { get; set; }

        private object CreateDefaultValueOfType(Type propertyType)
        {
            object defaultValue;
            // First we try to find the default value in the cache.
            if (!DefaultValuesCache.TryGetValue(propertyType, out defaultValue))
            {
                // otherwise we compute the default value and store it in the cache.
                defaultValue = propertyType.IsValueType ? Activator.CreateInstance(propertyType) : null;
                DefaultValuesCache.Add(propertyType, defaultValue);
            }
            return defaultValue;
        }

        internal bool ValidateDefaultValue(object value, Type type)
        {
            if (value == INTERNAL_NoValue.NoValue)
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

        internal object CreateDefaultValue(Type defaultValueType)
        {
            return CreateDefaultValueOfType(defaultValueType);
        }
    }
}
