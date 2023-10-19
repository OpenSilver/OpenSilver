//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics;
using System.Reflection;

namespace System.Windows.Controls.Common
{
    /// <summary>
    /// Utility class for Type related operations
    /// </summary>
    internal static class TypeHelper
    {
#region Internal Fields
        internal const char PropertyNameSeparator = '.';
#endregion

        /// <summary>
        /// Helper for SortList to handle nested properties (e.g. Address.Street)
        /// </summary>
        /// <param name="item">parent object</param>
        /// <param name="propertyPath">property names path</param>
        /// <returns>child object</returns>
        internal static object InvokePath(object item, string propertyPath)
        {
            object newItem = item;
            string[] propertyNames = propertyPath.Split(PropertyNameSeparator);
            for (int i = 0; i < propertyNames.Length; i++)
            {
                if (newItem == null || string.IsNullOrEmpty(propertyNames[i]))
                {
                    throw new InvalidOperationException(string.Format(
                        global::System.Globalization.CultureInfo.InvariantCulture,
                        OpenSilver.Internal.Controls.Data.DataForm.Toolkit.CommonResources.InvalidPropertyName,
                        propertyNames[i]));
                    ;
                }
                newItem = newItem.GetType().InvokeMember(propertyNames[i], global::System.Reflection.BindingFlags.GetProperty, null, newItem, null);
            }
            return newItem;
        }

        internal static Type GetNonNullableType(this Type type)
        {
            return IsNullableType(type) ? type.GetGenericArguments()[0] : type;
        }

        /// <summary>
        /// Returns the PropertyInfo corresponding to the provided propertyPath. The propertyPath can be a dotted
        /// path where each section is a public property name. Only public instance properties are searched for.
        /// </summary>
        /// <param name="type">The root type.</param>
        /// <param name="propertyPath">The property path.</param>
        /// <returns>The found PropertyInfo or null otherwise</returns>
        internal static PropertyInfo GetPropertyInfo(this Type type, string propertyPath)
        {
            Debug.Assert(type != null, "Unexpected null type in TypeHelper.GetPropertyOrFieldInfo");
            if (!String.IsNullOrEmpty(propertyPath))
            {
                string[] propertyNames = propertyPath.Split(PropertyNameSeparator);
                for (int i = 0; i < propertyNames.Length; i++)
                {
                    PropertyInfo propertyInfo = type.GetProperty(propertyNames[i]);
                    if (propertyInfo == null || propertyInfo.GetIndexParameters().Length > 0)
                    {
                        return null;
                    }
                    if (i == propertyNames.Length - 1)
                    {
                        return propertyInfo;
                    }
                    else
                    {
                        type = propertyInfo.PropertyType;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the friendly name for a type
        /// </summary>
        /// <param name="type">The type for which to return the name.</param>
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

        /// <summary>
        /// Gets whether or not a type is editable by default.
        /// </summary>
        /// <param name="type">The type in question.</param>
        /// <returns>Whether or not the type is editable by default.</returns>
        public static bool IsEditable(this Type type)
        {
            return
                type.IsPrimitive ||
                type.IsEnum ||
                type == typeof(String) ||
                type == typeof(DateTime) ||
                type == typeof(Decimal);
        }

        internal static bool IsNullableType(this Type type)
        {
            return (((type != null) && type.IsGenericType) && (type.GetGenericTypeDefinition() == typeof(Nullable<>)));
        }

        /// <summary>
        /// Returns whether or not the type is a primitive type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>Whether or not the type is a primitive type.</returns>
        internal static bool TypeIsPrimitive(Type type)
        {
            // If the type is a nullable type, get the base type.
            if (type.IsNullableType())
            {
                type = type.GetGenericArguments()[0];
            }

            return type.IsPrimitive || type == typeof(string) || type == typeof(DateTime) || type == typeof(Decimal);
        }
    }
}
