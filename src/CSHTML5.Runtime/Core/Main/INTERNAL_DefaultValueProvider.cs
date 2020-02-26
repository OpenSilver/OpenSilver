using System;
using System.Collections.Generic;

namespace CSHTML5.Internal
{
    internal class INTERNAL_DefaultValueProvider
    {
        private readonly Dictionary<Type, object> cache; // Cache for default values.

        internal INTERNAL_DefaultValueProvider()
        {
            this.cache = new Dictionary<Type, object>();
        }

        private static object CreateDefaultValueOfType(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        internal object ProvideValue(Type type)
        {
            object defaultValue;
            // First we try to find the default value in the cache.
            if (!this.cache.TryGetValue(type, out defaultValue))
            {
                defaultValue = CreateDefaultValueOfType(type);
                this.cache.Add(type, defaultValue);
            }
            return defaultValue;
        }
    }
}
