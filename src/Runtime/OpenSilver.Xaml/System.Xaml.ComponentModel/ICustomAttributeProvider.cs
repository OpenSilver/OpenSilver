using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.Xaml.ComponentModel
{
	class TypeAttributeProvider : ICustomAttributeProvider
	{
		readonly Type type;

		public TypeAttributeProvider(Type type)
		{
			this.type = type;
		}

		public object[] GetCustomAttributes(bool inherit)
		{
			var attr = type.GetTypeInfo().GetCustomAttributes(inherit).ToArray();
			return (attr as object[]) ?? attr.ToArray();
		}

		public object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			var attr = type.GetTypeInfo().GetCustomAttributes(attributeType, inherit);
			return (attr as object[]) ?? attr.ToArray();
		}

		public bool IsDefined(Type attributeType, bool inherit)
		{
			return type.GetTypeInfo().IsDefined(attributeType, inherit);
		}
	}

	class MemberAttributeProvider : ICustomAttributeProvider
	{
		readonly MemberInfo info;

		public MemberAttributeProvider(MemberInfo info)
		{
			this.info = info;
		}

		public object[] GetCustomAttributes(bool inherit)
		{
			var attr = info.GetCustomAttributes(inherit).ToArray();
			return (attr as object[]) ?? attr.ToArray();
		}

		public object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			var attr = info.GetCustomAttributes(attributeType, inherit);
			return (attr as object[]) ?? attr.ToArray();
		}

		public bool IsDefined(Type attributeType, bool inherit)
		{
			return info.IsDefined(attributeType, inherit);
		}
	}
}
