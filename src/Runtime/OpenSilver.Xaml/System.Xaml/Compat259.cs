using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.Xaml
{
	static class Compat259
	{
		public static IEnumerable<Type> GetInterfaces(this TypeInfo typeInfo)
		{
			return typeInfo.ImplementedInterfaces;
		}

		public static IEnumerable<ConstructorInfo> GetConstructors(this TypeInfo typeInfo)
		{
			return typeInfo.DeclaredConstructors.Where(r => !r.IsStatic);
		}

		public static Type[] GetGenericArguments(this TypeInfo typeInfo)
		{
			return typeInfo.GenericTypeArguments;
		}

		public static Type[] GetGenericParameters(this TypeInfo typeInfo)
		{
			return typeInfo.GenericTypeParameters;
		}

		public static MethodInfo GetAddMethod(this EventInfo methodInfo)
		{
			return methodInfo.AddMethod;
		}

		public static MethodInfo GetRemoveMethod(this EventInfo methodInfo)
		{
			return methodInfo.RemoveMethod;
		}

		public static MethodInfo GetPrivateSetMethod(this PropertyInfo propertyInfo)
		{
			return propertyInfo.SetMethod;
		}

		public static MethodInfo GetPrivateGetMethod(this PropertyInfo propertyInfo)
		{
			return propertyInfo.GetMethod;
		}

		public static IEnumerable<Type> GetExportedTypes(this Assembly assembly)
		{
			return assembly.ExportedTypes;
		}

		public static bool HasDefaultValue(this ParameterInfo parameterInfo)
		{
			return parameterInfo.HasDefaultValue;
		}
	}
}
