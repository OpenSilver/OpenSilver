using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Xaml.Markup;
using System.Xaml.Schema;

namespace System.Xaml
{
	static class TypeExtensionMethods
	{
		#region inheritance search and custom attribute provision

		public static T GetCustomAttribute<T>(this ICustomAttributeProvider provider, bool inherit) where T : Attribute
		{
			foreach (var a in provider.GetCustomAttributes(typeof(T), inherit))
				return (T)(object)a;
			return null;
		}

		public static Attribute GetCustomAttribute(this ICustomAttributeProvider provider, Type type, bool inherit)
		{
			foreach (var a in provider.GetCustomAttributes(type, inherit))
				return (Attribute)(object)a;
			return null;
		}

		public static T GetCustomAttribute<T>(this XamlType type) where T : Attribute
		{
			if (type.UnderlyingType == null)
				return null;

			T ret = type.CustomAttributeProvider.GetCustomAttribute<T>(true);
			if (ret != null)
				return ret;
			if (type.BaseType != null)
				return type.BaseType.GetCustomAttribute<T>();
			return null;
		}

		public static bool ImplementsAnyInterfacesOf(this Type type, params Type[] definitions)
		{
			return definitions.Any(t => ImplementsInterface(type, t));
		}

		public static bool ImplementsInterface(this Type type, Type definition)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (definition == null)
				throw new ArgumentNullException("definition");
			if (type == definition)
				return true;

			if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == definition)
				return true;

			foreach (var iface in type.GetTypeInfo().GetInterfaces())
				if (iface == definition || (iface.GetTypeInfo().IsGenericType && iface.GetGenericTypeDefinition() == definition))
					return true;
			return false;
		}

		#endregion

		#region type conversion and member value retrieval

		static readonly NullExtension null_value = new NullExtension();

		public static object GetExtensionWrapped(object o)
		{
			// FIXME: should this manually checked, or is there any way to automate it?
			// Also XamlSchemaContext might be involved but this method signature does not take it consideration.
			if (o == null)
				return null_value;
			if (o is Array)
				return new ArrayExtension((Array)o);
			if (o is Type)
				return new TypeExtension((Type)o);
			return o;
		}

		public static string GetStringValue(XamlType xt, XamlMember xm, object obj, IValueSerializerContext vsctx)
		{
			if (obj == null)
				return String.Empty;
			if (obj is Type)
				return new XamlTypeName(xt.SchemaContext.GetXamlType((Type)obj)).ToString(vsctx?.GetService(typeof(INamespacePrefixLookup)) as INamespacePrefixLookup);

			var vs = xm?.ValueSerializer ?? xt.ValueSerializer;
			if (vs != null)
				return vs.ConverterInstance.ConvertToString(obj, vsctx);

			// FIXME: does this make sense?
			var vc = xm?.TypeConverter ?? xt.TypeConverter;
			var tc = vc?.ConverterInstance;
			if (tc != null && tc.CanConvertTo((ITypeDescriptorContext)vsctx, typeof(string)))
				return (string)tc.ConvertTo((ITypeDescriptorContext)vsctx, CultureInfo.InvariantCulture, obj, typeof(string));
			if (obj is string || obj == null)
				return (string)obj;
			throw new InvalidCastException(String.Format("Cannot cast object '{0}' to string", obj.GetType()));
		}

		public static string GetTypeConverterName(this ICustomAttributeProvider member, bool inherit)
		{
			if (member == null)
				return null;
			var typeConverterName = member.GetCustomAttribute<TypeConverterAttribute>(inherit)?.ConverterTypeName;
			return typeConverterName;
		}

		public static string GetTypeConverterName(this MemberInfo member, bool inherit)
		{
			if (member == null)
				return null;
			var typeConverterName = member.GetCustomAttribute<TypeConverterAttribute>(inherit)?.ConverterTypeName;
			return typeConverterName;
		}

		public static string GetTypeConverterName(this TypeInfo type, bool inherit)
		{
			if (type == null)
				return null;
			var typeConverterName = type.GetCustomAttribute<TypeConverterAttribute>(inherit)?.ConverterTypeName;
			return typeConverterName;
		}

		public static bool IsBaseTypeConverter(this TypeConverter typeConverter)
		{
			var type = typeConverter.GetType();
			return type == typeof(TypeConverter)
				|| type == typeof(CollectionConverter)
				|| type.FullName == "System.ComponentModel.ReferenceConverter";
		}

		public static TypeConverter GetTypeConverter(this MemberInfo member)
		{
			var typeConverterName = GetTypeConverterName(member, true);
			if (string.IsNullOrEmpty(typeConverterName))
				return null;

			var tcType = Type.GetType(typeConverterName);

			var tc = Activator.CreateInstance(tcType);
			return tc as TypeConverter;
		}

		public static TypeConverter GetTypeConverter(this Type type)
		{
			return TypeDescriptor.GetConverter(type);
		}

		#endregion

		public static bool IsContentValue(this XamlMember member, IValueSerializerContext vsctx)
		{
			if (ReferenceEquals(member, XamlLanguage.Initialization))
				return true;
			if (ReferenceEquals(member, XamlLanguage.PositionalParameters) || ReferenceEquals(member, XamlLanguage.Arguments))
				return false; // it's up to the argument (no need to check them though, as IList<object> is not of value)
			var typeConverter = member.TypeConverter;
			if (typeConverter != null && typeConverter.ConverterInstance != null && typeConverter.ConverterInstance.CanConvertTo((ITypeDescriptorContext)vsctx, typeof(string)))
				return true;
			return IsContentValue(member.Type, vsctx);
		}

		public static bool IsContentValue(this XamlType type, IValueSerializerContext vsctx)
		{
			var typeConverter = type.TypeConverter;
			if (typeConverter != null && typeConverter.ConverterInstance != null && typeConverter.ConverterInstance.CanConvertTo((ITypeDescriptorContext)vsctx, typeof(string)))
				return true;
			return false;
		}

		public static bool ListEquals(this IList<XamlType> a1, IList<XamlType> a2)
		{
			if (a1 == null || a1.Count == 0)
				return a2 == null || a2.Count == 0;
			if (a2 == null || a2.Count == 0)
				return false;
			if (a1.Count != a2.Count)
				return false;
			for (int i = 0; i < a1.Count; i++)
				if (a1[i] != a2[i])
					return false;
			return true;
		}

		public static bool HasPositionalParameters(this XamlType type, IValueSerializerContext vsctx)
		{
			// FIXME: find out why only TypeExtension and StaticExtension yield this directive. Seealso XamlObjectReaderTest.Read_CustomMarkupExtension*()
			return type == XamlLanguage.Type ||
			type == XamlLanguage.Static ||
				(type.ConstructionRequiresArguments && ExaminePositionalParametersApplicable(type, vsctx));
		}

		static bool ExaminePositionalParametersApplicable(this XamlType type, IValueSerializerContext vsctx)
		{
			if (!type.IsMarkupExtension || type.UnderlyingType == null)
				return false;

			var args = type.GetSortedConstructorArguments();
			if (args == null)
				return false;

			foreach (var arg in args)
				if (arg.Type != null && !arg.Type.IsContentValue(vsctx))
					return false;

			Type[] argTypes = (from arg in args
							   select arg.Type.UnderlyingType).ToArray();
			if (argTypes.Any(at => at == null))
				return false;
			var ci = type.UnderlyingType
				.GetTypeInfo()
				.GetConstructors().FirstOrDefault(c =>
				  c.GetParameters().Select(r => r.ParameterType).SequenceEqual(argTypes)
					 );
			return ci != null;
		}

		public static IEnumerable<XamlWriterInternalBase.MemberAndValue> GetSortedConstructorArguments(this XamlType type, IList<XamlWriterInternalBase.MemberAndValue> members)
		{
			var constructors = type.UnderlyingType.GetTypeInfo().GetConstructors();
			var preferredParameterCount = 0;
			ConstructorInfo preferredConstructor = null;
			foreach (var constructor in constructors)
			{
				var parameters = constructor.GetParameters();
				var matchedParameterCount = 0;
				bool mismatch = false;
				for (int i = 0; i < parameters.Length; i++)
				{
					var parameter = parameters[i];
					var member = members.FirstOrDefault(r => r.Member.ConstructorArgumentName() == parameter.Name);
					if (member == null)
					{
						// allow parameters with a default value to be omitted
						mismatch = !parameter.HasDefaultValue();
						if (mismatch)
							break;
						continue;
					}
					var paramXamlType = type.SchemaContext.GetXamlType(parameter.ParameterType);

					// check if type input type can be converted to the parameter type
					mismatch = !paramXamlType.CanConvertFrom(member.Member.Type);
					if (mismatch)
						break;
					matchedParameterCount++;
				}
				// prefer the constructor that accepts the most parameters
				if (!mismatch && matchedParameterCount > preferredParameterCount)
				{
					preferredConstructor = constructor;
					preferredParameterCount = matchedParameterCount;
				}
			}
			if (preferredConstructor == null)
				return null;
			return preferredConstructor
				.GetParameters()
				.Select(p => {
					var mem = members.FirstOrDefault(r => r.Member.ConstructorArgumentName() == p.Name);
					if (mem == null && p.HasDefaultValue())
					{
						mem = new XamlWriterInternalBase.MemberAndValue(type.SchemaContext.GetParameter(p));
						mem.Value = p.DefaultValue;
					}
					return mem;
				});
		}


		public static IEnumerable<XamlMember> GetSortedConstructorArguments(this XamlType type, IList<object> contents = null)
		{
			var constructors = type.UnderlyingType.GetTypeInfo().GetConstructors();
			if (contents != null && contents.Count > 0)
			{
				var context = type.SchemaContext;

				// find constructor that matches content type directly first, then by ones that can be converted by type
				var constructorArguments =
					FindConstructorArguments(context, constructors, contents, (type1, type2) => type1.UnderlyingType.GetTypeInfo().IsAssignableFrom(type2.UnderlyingType.GetTypeInfo()))
					?? FindConstructorArguments(context, constructors, contents, (type1, type2) => type1.CanConvertFrom(type2));

				if (constructorArguments != null)
					return constructorArguments;
			}

			// find constructor based on ConstructorArgumentAttribute
			var args = type.GetConstructorArguments();
			foreach (var ci in constructors)
			{
				var pis = ci.GetParameters();
				if (args.Count != pis.Length)
					continue;
				bool mismatch = false;
				foreach (var pi in pis)
					mismatch |= args.All(a => a.ConstructorArgumentName() != pi.Name);
				if (mismatch)
					continue;
				return args.OrderBy(c => pis.FindParameterWithName(c.ConstructorArgumentName()).Position);
			}

			return null;
		}

		static ParameterInfo FindParameterWithName(this IEnumerable<ParameterInfo> pis, string name)
		{
			foreach (var pi in pis)
			{
				if (pi.Name == name)
					return pi;
			}
			return null;
		}

		static IEnumerable<XamlMember> FindConstructorArguments(XamlSchemaContext context, IEnumerable<ConstructorInfo> constructors, IList<object> contents, Func<XamlType, XamlType, bool> compare)
		{
			foreach (var constructor in constructors)
			{
				var parameters = constructor.GetParameters();
				if (contents.Count > parameters.Length)
					continue;

				bool mismatch = false;
				for (int i = 0; i < parameters.Length; i++)
				{
					var parameter = parameters[i];
					if (i >= contents.Count)
					{
						// allow parameters with a default value to be omitted
						mismatch = !parameter.HasDefaultValue();
						if (mismatch)
							break;
						continue;
					}
					// check if the parameter value can be assigned to the required type
					var posParameter = contents[i];
					var paramXamlType = context.GetXamlType(parameter.ParameterType);

					// check if type input type can be converted to the parameter type
					var inputType = posParameter == null ? XamlLanguage.Null : context.GetXamlType(posParameter.GetType());
					mismatch = !compare(paramXamlType, inputType);
					if (mismatch)
						break;
				}
				if (mismatch)
					continue;

				// matches constructor arguments
				return constructor
					.GetParameters()
					.Select(p => context.GetParameter(p));
			}
			return null;
		}

		public static string ConstructorArgumentName(this XamlMember xm)
		{
			var caa = xm.CustomAttributeProvider.GetCustomAttribute<ConstructorArgumentAttribute>(false);
			return caa.ArgumentName;
		}

		class InternalMemberComparer : IComparer<XamlMember>
		{
			public int Compare(XamlMember x, XamlMember y)
			{
				return CompareMembers(x, y);
			}
		}

		internal static IComparer<XamlMember> MemberComparer = new InternalMemberComparer();

		internal static int CompareMembers(XamlMember m1, XamlMember m2)
		{
			if (ReferenceEquals(m1, null))
				return ReferenceEquals(m2, null) ? 0 : -1;
			if (ReferenceEquals(m2, null))
				return 1;

			// these come before non-content properties

			// 1. PositionalParameters comes first
			if (ReferenceEquals(m1, XamlLanguage.PositionalParameters))
				return ReferenceEquals(m2, XamlLanguage.PositionalParameters) ? 0 : -1;
			if (ReferenceEquals(m2, XamlLanguage.PositionalParameters))
				return 1;

			// 2. constructor arguments
			if (m1.IsConstructorArgument)
			{
				if (!m2.IsConstructorArgument)
					return -1;
			}
			else if (m2.IsConstructorArgument)
				return 1;


			// these come AFTER non-content properties

			// 1. initialization
			if (ReferenceEquals(m1, XamlLanguage.Initialization))
				return ReferenceEquals(m2, XamlLanguage.Initialization) ? 0 : 1;
			if (ReferenceEquals(m2, XamlLanguage.Initialization))
				return -1;

			// 2. key
			if (ReferenceEquals(m1, XamlLanguage.Key))
				return ReferenceEquals(m2, XamlLanguage.Key) ? 0 : 1;
			if (ReferenceEquals(m2, XamlLanguage.Key))
				return -1;

			// 3. Name
			if (ReferenceEquals(m1, XamlLanguage.Name))
				return ReferenceEquals(m2, XamlLanguage.Name) ? 0 : 1;
			if (ReferenceEquals(m2, XamlLanguage.Name))
				return -1;


			// 4. Properties that (probably) require a child node should come last so we can
			// put as many in the start tag as possible.
			if (m1.RequiresChildNode)
			{
				if (!m2.RequiresChildNode)
					return 1;
			}
			else if (m2.RequiresChildNode)
				return -1;

			// then, compare names.
			return String.CompareOrdinal(m1.Name, m2.Name);
		}

		internal static string GetInternalXmlName(this XamlMember xm)
		{
			return xm.IsAttachable ? String.Concat(xm.DeclaringType.InternalXmlName, ".", xm.Name) : xm.Name;
		}

		internal static ICustomAttributeProvider GetCustomAttributeProvider (this XamlType type)
		{
			return type.UnderlyingType;
		}
		
		internal static ICustomAttributeProvider GetCustomAttributeProvider (this XamlMember member)
		{
			return member.UnderlyingMember;
		}
	}
}
