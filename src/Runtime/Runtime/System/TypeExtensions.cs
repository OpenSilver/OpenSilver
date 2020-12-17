#if !NETSTANDARD

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace System
{
    public static class TypeExtensions
    {
        private const BindingFlags DefaultLookup = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
        private const BindingFlags DeclaredOnlyLookup = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

        // EmptyTypes is used to indicate that we are looking for someting without any parameters.
        public readonly static Type[] EmptyTypes = EmptyArray<Type>.Value;

        public static Type MakeGenericType(this Type type, params Type[] typeArguments)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return type.MakeGenericType(typeArguments);
        }

        internal static MemberInfo[] GetDefaultMembers(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            object[] attributes = type.GetCustomAttributes(true);
            List<object> dmAttributes = new List<object>();
            foreach (object attr in attributes)
            {
                Type t = attr.GetType();
                if (t.Name == "DefaultMemberAttribute" &&
                    t.Namespace == "System.Reflection")
                {
                    dmAttributes.Add(attr);
                }
            }
            if (dmAttributes != null && dmAttributes.Count == 1)
            {
                string name = (string)dmAttributes[0].GetType()
                                                     .GetProperty("MemberName")
                                                     .GetValue(dmAttributes[0]);
                return (name == null ? new MemberInfo[0] : type.GetMember(name));
            }

            return new MemberInfo[0];
        }

        internal static PropertyInfo GetProperty(this Type type, string name, Type returnType)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            PropertyInfo match = null;
            for (Type baseType = type; baseType != null; baseType = baseType.BaseType)
            {
                PropertyInfo[] properties = baseType.GetProperties(DeclaredOnlyLookup)
                                                    .Where(p => p.Name == name && p.PropertyType == returnType)
                                                    .ToArray();
                if (properties.Length == 0)
                {
                    continue;
                }
                else if (properties.Length == 1 && match == null)
                {
                    match = properties[0];
                }
                else // Multiple matches
                {
                    throw new AmbiguousMatchException("Ambiguous match found.");
                }
            }

            return match;
        }
    }

    // Useful in number of places that return an empty byte array to avoid unnecessary memory allocation.
    internal static class EmptyArray<T>
    {
        public static readonly T[] Value = new T[0];
    }
}

#endif
