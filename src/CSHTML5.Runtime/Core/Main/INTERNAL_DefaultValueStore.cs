using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSHTML5.Internal
{
    internal class INTERNAL_DefaultValueStore
    {

        // Singleton
        internal static readonly INTERNAL_DefaultValueStore Instance;

        static INTERNAL_DefaultValueStore()
        {
            Instance = new INTERNAL_DefaultValueStore();
        }

        private INTERNAL_DefaultValueStore()
        {
            _defaultValuesCache = new Dictionary<Type, object>();
        }

        private Dictionary<Type, object> _defaultValuesCache;
        // Cache for default values.
        private Dictionary<Type, object> DefaultValuesCache
        {
            get
            {
                return _defaultValuesCache;
            }
        }

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

        private bool ValidateDefaultValue(object value, Type type)
        {
            if (value == INTERNAL_NoValue.NoValue)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        internal bool EnsureDefaultValueIsValid(object unCheckedDefaultValue, Type defaultValueType, out object defaultValue)
        {
            if (ValidateDefaultValue(unCheckedDefaultValue, defaultValueType))
            {
                defaultValue = unCheckedDefaultValue;
                return true;
            }
            else
            {
                defaultValue = CreateDefaultValueOfType(defaultValueType);
                return false;
            }
        }

        internal object CreateDefaultValue(Type defaultValueType)
        {
            return CreateDefaultValueOfType(defaultValueType);
        }
    }
}
