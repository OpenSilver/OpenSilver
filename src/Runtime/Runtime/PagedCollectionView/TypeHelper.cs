//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace System.Windows.Common
{
    /// <summary>
    /// Utility class for Type related operations
    /// </summary>
    internal static class TypeHelper
    {
#region Internal Fields
        internal const char IndexParameterSeparator = ',';
        internal const char LeftIndexerToken = '[';
        internal const char PropertyNameSeparator = '.';
        internal const char RightIndexerToken = ']';
#endregion

        private static Type FindGenericType(Type definition, Type type)
        {
            while ((type != null) && (type != typeof(object)))
            {
                if (type.IsGenericType && (type.GetGenericTypeDefinition() == definition))
                {
                    return type;
                }
                if (definition.IsInterface)
                {
                    foreach (Type type2 in type.GetInterfaces())
                    {
                        Type type3 = FindGenericType(definition, type2);
                        if (type3 != null)
                        {
                            return type3;
                        }
                    }
                }
                type = type.BaseType;
            }
            return null;
        }

        /// <summary>
        /// Gets the default member name that is used for an indexer (e.g. "Item").
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <returns>Default member name.</returns>
        private static string GetDefaultMemberName(this Type type)
        {
            object[] attributes = type.GetCustomAttributes(typeof(DefaultMemberAttribute), true);
            if (attributes != null && attributes.Length == 1)
            {
                DefaultMemberAttribute defaultMemberAttribute = attributes[0] as DefaultMemberAttribute;
                return defaultMemberAttribute.MemberName;
            }
            else
            {
                return null;
            }
        }

        internal static Type GetEnumerableItemType(this Type enumerableType)
        {
            Type type = FindGenericType(typeof(IEnumerable<>), enumerableType);
            if (type != null)
            {
                return type.GetGenericArguments()[0];
            }
            return enumerableType;
        }

        /// <summary>
        /// Retrieves the value and type of a property. That property can be nested and its path
        /// can include indexers. Each element of the path needs to be a public instance property.
        /// </summary>
        /// <param name="parentType">The parent Type</param>
        /// <param name="propertyPath">Property path</param>
        /// <param name="exception">Potential exception</param>
        /// <param name="item">Parent item which will be set to the property value if non-null.</param>
        /// <returns></returns>
        private static PropertyInfo GetNestedProperty(this Type parentType, string propertyPath, out Exception exception, ref object item)
        {
            exception = null;
            Type type = parentType;
            PropertyInfo propertyInfo = null;
            List<string> propertyNames = SplitPropertyPath(propertyPath);
            for (int i = 0; i < propertyNames.Count; i++)
            {
                // if we can't find the property or it is not of the correct type,
                // treat it as a null value
                object[] index = null;
                propertyInfo = type.GetPropertyOrIndexer(propertyNames[i], out index);
                if (propertyInfo == null)
                {
                    item = null;
                    return null;
                }

                if (!propertyInfo.CanRead)
                {
                    exception = new InvalidOperationException(string.Format(
                        CultureInfo.InvariantCulture,
                        PagedCollectionViewResources.PropertyNotReadable,
                        propertyNames[i],
                        type.GetTypeName()));
                    item = null;
                    return null;
                }

                if (item != null)
                {
                    item = propertyInfo.GetValue(item, index);
                }
                type = propertyInfo.PropertyType.GetNonNullableType();
            }

            return propertyInfo;
        }

        /// <summary>
        /// Extension method that returns the type of a property. That property can be nested and
        /// its path can include indexers. Each element of the path needs to be a public instance property.
        /// </summary>
        /// <param name="parentType">Type that exposes that property</param>
        /// <param name="propertyPath">Property path</param>
        /// <returns>Property type</returns>
        internal static Type GetNestedPropertyType(this Type parentType, string propertyPath)
        {
            if (string.IsNullOrEmpty(propertyPath))
            {
                return parentType;
            }

            object item = null;
            Exception exception = null;
            PropertyInfo propertyInfo = parentType.GetNestedProperty(propertyPath, out exception, ref item);
            return propertyInfo == null ? null : propertyInfo.PropertyType;
        }

        /// <summary>
        /// Retrieves the value of a property. That property can be nested and its path can
        /// include indexers. Each element of the path needs to be a public instance property.
        /// The return value will either be of type propertyType or it will be null.
        /// </summary>
        /// <param name="item">Object that exposes the property</param>
        /// <param name="propertyPath">Property path</param>
        /// <param name="propertyType">Property type</param>
        /// <param name="exception">Potential exception</param>
        /// <returns>Property value</returns>
        internal static object GetNestedPropertyValue(object item, string propertyPath, Type propertyType, out Exception exception)
        {
            exception = null;

            // if the item is null, treat the property value as null
            if (item == null)
            {
                return null;
            }

            // if the propertyPath is null or empty, return the item
            if (String.IsNullOrEmpty(propertyPath))
            {
                return item;
            }

            object propertyValue = item;
            Type itemType = item.GetType();
            if (itemType != null)
            {
                PropertyInfo propertyInfo = itemType.GetNestedProperty(propertyPath, out exception, ref propertyValue);
                if (propertyInfo != null && propertyInfo.PropertyType != propertyType)
                {
                    return null;
                }
            }
            return propertyValue;
        }

        internal static Type GetNonNullableType(this Type type)
        {
            return IsNullableType(type) ? type.GetGenericArguments()[0] : type;
        }

        /// <summary>
        /// Returns the PropertyInfo for the specified property path.  If the property path
        /// refers to an indexer (e.g. "[abc]"), then the index out parameter will be set to the value
        /// specified in the property path.  This method only supports indexers with a single parameter
        /// that is either an int or a string.  Int parameters take priority over string parameters.
        /// </summary>
        /// <param name="type">Type to search.</param>
        /// <param name="propertyPath">Property path.</param>
        /// <param name="index">Set to the index if return value is an indexer, otherwise null.</param>
        /// <returns>PropertyInfo for either a property or an indexer.</returns>
        private static PropertyInfo GetPropertyOrIndexer(this Type type, string propertyPath, out object[] index)
        {
            index = null;
            if (string.IsNullOrEmpty(propertyPath) || propertyPath[0] != LeftIndexerToken)
            {
                // Return the default value of GetProperty if the first character is not an indexer token.
                return type.GetProperty(propertyPath);
            }

            if (propertyPath.Length < 3 || propertyPath[propertyPath.Length - 1] != RightIndexerToken)
            {
                // Return null if the indexer does not meet the standard format (i.e. "[x]").
                return null;
            }

            string stringIndex = propertyPath.Substring(1, propertyPath.Length - 2);
            if (stringIndex.IndexOf(IndexParameterSeparator) != -1)
            {
                // Return null if there are multiple parameters specified for this indexer (e.g. "[a,b]").
                return null;
            }

            string propertyName = type.GetDefaultMemberName();
            if (string.IsNullOrEmpty(propertyName))
            {
                // Return null if there is no default member name for the indexer.
                return null;
            }

            PropertyInfo indexer = null;
            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
                // Find a property with the indexer's name.  We shouldn't use GetProperty because
                // there could be an AmbiguousMatchException if there are multiple indexers with
                // different parameter or return types.
                if (string.Equals(propertyInfo.Name, propertyName))
                {
                    // We only support indexers with a single parameter.
                    ParameterInfo[] parameterInfos = propertyInfo.GetIndexParameters();
                    if (parameterInfos != null && parameterInfos.Length == 1)
                    {
                        // First check if there is an int indexer and whether or not the index can be parsed.
                        if (parameterInfos[0].ParameterType == typeof(int))
                        {
                            int intIndex = -1;
                            if (Int32.TryParse(stringIndex.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out intIndex))
                            {
                                indexer = propertyInfo;
                                index = new object[] { intIndex };
                                break;
                            }
                        }

                        // Also check if there is a string indexer, but don't break out if there is one, because there
                        // might be an int indexer later in the properties list that would take priority.
                        if (parameterInfos[0].ParameterType == typeof(string))
                        {
                            indexer = propertyInfo;
                            index = new object[] { stringIndex };
                        }
                    }
                }
            }
            return indexer;
        }

        /// <summary>
        /// Returns the friendly name for a type
        /// </summary>
        /// <param name="type">The type to get the name from</param>
        /// <returns>Textual representation of the input type</returns>
        internal static string GetTypeName(this Type type)
        {
            Type baseType = type.GetNonNullableType();
            string s = baseType.Name;
            if (type != baseType)
            {
                s += '?';
            }
            return s;
        }

        internal static bool IsEnumerableType(this Type enumerableType)
        {
            return FindGenericType(typeof(IEnumerable<>), enumerableType) != null;
        }

        internal static bool IsNullableType(this Type type)
        {
            return (((type != null) && type.IsGenericType) && (type.GetGenericTypeDefinition() == typeof(Nullable<>)));
        }

        /// <summary>
        /// Returns a list of substrings where each one represents a single property within a nested
        /// property path which may include indexers.  For example, the string "abc.d[efg][h].ijk"
        /// would return the substrings: "abc", "d", "[efg]", "[h]", and "ijk".
        /// </summary>
        /// <param name="propertyPath">Path to split.</param>
        /// <returns>List of property substrings.</returns>
        private static List<string> SplitPropertyPath(string propertyPath)
        {
            List<string> propertyPaths = new List<string>();
            if (!string.IsNullOrEmpty(propertyPath))
            {
                int startIndex = 0;
                for (int index = 0; index < propertyPath.Length; index++)
                {
                    if (propertyPath[index] == PropertyNameSeparator)
                    {
                        propertyPaths.Add(propertyPath.Substring(startIndex, index - startIndex));
                        startIndex = index + 1;
                    }
                    else if (startIndex != index && propertyPath[index] == LeftIndexerToken)
                    {
                        propertyPaths.Add(propertyPath.Substring(startIndex, index - startIndex));
                        startIndex = index;
                    }
                    else if (index == propertyPath.Length - 1)
                    {
                        propertyPaths.Add(propertyPath.Substring(startIndex));
                    }
                }
            }
            return propertyPaths;
        }
    }
}
