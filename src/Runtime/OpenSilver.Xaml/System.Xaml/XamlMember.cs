//
// Copyright (C) 2011 Novell Inc. http://novell.com
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xaml.ComponentModel;
using System.Xaml.Markup;
using System.Xaml.Schema;

namespace System.Xaml
{
    internal class XamlMember : IEquatable<XamlMember>
	{
		FlagValue flags;
		static class MemberFlags
		{
			public const int IsAmbient = 1 << 0;
			public const int IsEvent = 1 << 1;
			public const int IsReadOnly = 1 << 2;
			public const int IsReadPublic = 1 << 3;
			public const int IsUnknown = 1 << 4;
			public const int IsWriteOnly = 1 << 5;
			public const int IsWritePublic = 1 << 6;
			public const int IsNameValid = 1 << 7;
			public const int IsConstructorArgument = 1 << 8;
			// direct value (do not use a lookup method)
			public const int IsAttachable = 1 << 9;
			public const int IsDefaultEvent = 1 << 10;
			public const int IsDirective = 1 << 11;
			public const int ShouldSerialize = 1 << 12;
			public const int RequiresChildNode = 1 << 13;
			public const int IgnoreDataMember = 1 << 14;
		}
		XamlType target_type;
		MemberInfo underlying_member;
		MethodInfo underlying_getter, underlying_setter;
		XamlSchemaContext context;
		XamlMemberInvoker invoker;
		ReferenceValue<string> ns;
		ReferenceValue<XamlType> targetType;
		ReferenceValue<XamlType> type;
		ReferenceValue<XamlValueConverter<TypeConverter>> typeConverter;
		ReferenceValue<XamlValueConverter<ValueSerializer>> valueSerializer;
		ReferenceValue<ICustomAttributeProvider> customAttributeProvider;
		ReferenceValue<XamlValueConverter<XamlDeferringLoader>> deferringLoader;
		ReferenceValue<MethodInfo> shouldSerializeMethod;

		internal XamlSchemaContext SchemaContext => context; // should we expose this as public?

		public XamlMember(EventInfo eventInfo, XamlSchemaContext schemaContext)
			: this(eventInfo, schemaContext, null)
		{
		}

		public XamlMember(EventInfo eventInfo, XamlSchemaContext schemaContext, XamlMemberInvoker invoker)
			: this(schemaContext, invoker)
		{
			if (eventInfo == null)
				throw new ArgumentNullException("eventInfo");
			Name = eventInfo.Name;
			underlying_member = eventInfo;
			DeclaringType = schemaContext.GetXamlType(eventInfo.DeclaringType);
			target_type = DeclaringType;
			underlying_setter = eventInfo.GetAddMethod();
			flags.Set(MemberFlags.IsDefaultEvent, true);
		}

		public XamlMember(PropertyInfo propertyInfo, XamlSchemaContext schemaContext)
			: this(propertyInfo, schemaContext, null)
		{
		}

		public XamlMember(PropertyInfo propertyInfo, XamlSchemaContext schemaContext, XamlMemberInvoker invoker)
			: this(schemaContext, invoker)
		{
			if (propertyInfo == null)
				throw new ArgumentNullException("propertyInfo");
			Name = propertyInfo.Name;
			underlying_member = propertyInfo;
			DeclaringType = schemaContext.GetXamlType(propertyInfo.DeclaringType);
			target_type = DeclaringType;
			underlying_getter = propertyInfo.GetPrivateGetMethod();
			underlying_setter = propertyInfo.GetPrivateSetMethod();
		}

		public XamlMember(string attachableEventName, MethodInfo adder, XamlSchemaContext schemaContext)
			: this(attachableEventName, adder, schemaContext, null)
		{
		}

		public XamlMember(string attachableEventName, MethodInfo adder, XamlSchemaContext schemaContext, XamlMemberInvoker invoker)
			: this(schemaContext, invoker)
		{
			if (attachableEventName == null)
				throw new ArgumentNullException("attachableEventName");
			if (adder == null)
				throw new ArgumentNullException("adder");
			Name = attachableEventName;
			VerifyAdderSetter(adder);
			underlying_member = adder;
			DeclaringType = schemaContext.GetXamlType(adder.DeclaringType);
			target_type = schemaContext.GetXamlType(typeof(object));
			underlying_setter = adder;
			flags.Set(MemberFlags.IsDefaultEvent | MemberFlags.IsAttachable, true);
		}

		public XamlMember(string attachablePropertyName, MethodInfo getter, MethodInfo setter, XamlSchemaContext schemaContext)
			: this(attachablePropertyName, getter, setter, schemaContext, null)
		{
		}

		public XamlMember(string attachablePropertyName, MethodInfo getter, MethodInfo setter, XamlSchemaContext schemaContext, XamlMemberInvoker invoker)
			: this(schemaContext, invoker)
		{
			if (attachablePropertyName == null)
				throw new ArgumentNullException("attachablePropertyName");
			if (getter == null && setter == null)
				throw new ArgumentNullException("getter", "Either property getter or setter must be non-null.");
			Name = attachablePropertyName;
			VerifyGetter(getter);
			VerifyAdderSetter(setter);
			underlying_member = getter ?? setter;
			DeclaringType = schemaContext.GetXamlType(underlying_member.DeclaringType);
			target_type = schemaContext.GetXamlType(typeof(object));
			underlying_getter = getter;
			underlying_setter = setter;
			flags.Set(MemberFlags.IsAttachable, true);
		}

		[EnhancedXaml]
		public XamlMember(ParameterInfo parameterInfo, XamlSchemaContext schemaContext)
			: this(parameterInfo, schemaContext, null)
		{
		}

		[EnhancedXaml]
		public XamlMember(ParameterInfo parameterInfo, XamlSchemaContext schemaContext, XamlMemberInvoker invoker)
			: this(schemaContext, invoker)
		{
			var declaringType = schemaContext.GetXamlType(parameterInfo.Member.DeclaringType);
			Name = parameterInfo.Name;
			context = declaringType.SchemaContext;
			DeclaringType = declaringType;
			target_type = DeclaringType;
			type = schemaContext.GetXamlType(parameterInfo.ParameterType);
		}

		public XamlMember(string name, XamlType declaringType, bool isAttachable)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (declaringType == null)
				throw new ArgumentNullException("declaringType");
			Name = name;
			context = declaringType.SchemaContext;
			DeclaringType = declaringType;
			target_type = DeclaringType;
			flags.Set(MemberFlags.IsAttachable, isAttachable);
		}

		internal XamlMember(string name, string preferredNamespace)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			Name = name;
			flags.Set(MemberFlags.IsUnknown, true);
			ns.Set(preferredNamespace);
		}

		XamlMember(XamlSchemaContext schemaContext, XamlMemberInvoker invoker)
		{
			if (schemaContext == null)
				throw new ArgumentNullException("schemaContext");
			context = schemaContext;
			this.invoker = invoker;
		}

		internal XamlMember(bool isDirective, string ns, string name)
		{
			this.ns = ns;
			Name = name;
			flags.Set(MemberFlags.IsDirective, isDirective);
		}

		internal MethodInfo UnderlyingGetter => LookupUnderlyingGetter();

		internal MethodInfo UnderlyingSetter => LookupUnderlyingSetter();

		public XamlType DeclaringType { get; private set; }

		public string Name { get; private set; }

		public string PreferredXamlNamespace => ns.HasValue ? ns.Value : ns.Set(DeclaringType?.PreferredXamlNamespace);

		public DesignerSerializationVisibility SerializationVisibility
		{
			get
			{
				var c = this.CustomAttributeProvider;
				var a = c == null ? null : c.GetCustomAttribute<DesignerSerializationVisibilityAttribute>(false);
				return a != null ? a.Visibility : DesignerSerializationVisibility.Visible;
			}
		}

		internal bool LookupIgnoreDataMember()
		{
			var c = this.CustomAttributeProvider;
			var a = c == null ? null : c.GetCustomAttribute<IgnoreDataMemberAttribute>(false);
			return a != null;
		}

		internal bool ShouldSerialize(object instance, bool useIgnoreDataMemberAttribute)
		{
			var shouldSerialize = flags.Get(MemberFlags.ShouldSerialize) ?? flags.Set(MemberFlags.ShouldSerialize, LookupShouldSerialize());

			if (!shouldSerialize)
				return false;

			if (useIgnoreDataMemberAttribute)
			{
				var ignoreDataMember = flags.Get(MemberFlags.IgnoreDataMember) ?? flags.Set(MemberFlags.IgnoreDataMember, LookupIgnoreDataMember());
				if (ignoreDataMember)
					return false;
			}

			if (!shouldSerializeMethod.HasValue)
				shouldSerializeMethod.Set(LookupShouldSerializeMethod());

			if (shouldSerializeMethod.Value != null)
			{
				return (bool)shouldSerializeMethod.Value.Invoke(instance, null);
			}

			return true;
		}

		MethodInfo LookupShouldSerializeMethod()
		{
			foreach (var method in DeclaringType.UnderlyingType?.GetTypeInfo().GetDeclaredMethods("ShouldSerialize" + Name))
			{
				if (method.GetParameters().Length == 0 && method.ReturnType == typeof(bool))
				{
					return method;
				}
			}
			return null;
		}

		bool LookupShouldSerialize()
		{
			if (SerializationVisibility == DesignerSerializationVisibility.Hidden)
				return false;

			if (!IsReadPublic)
				return false;

			if (!IsWritePublic
				&& !Type.IsXData
				&& !Type.IsArray
				&& !Type.IsCollection
				&& !Type.IsDictionary)
				return false;

			return true;
		}

		public bool IsAttachable => flags.Get(MemberFlags.IsAttachable) ?? false;

		public bool IsDirective => flags.Get(MemberFlags.IsDirective) ?? false;

		public bool IsNameValid => flags.Get(MemberFlags.IsNameValid) ?? flags.Set(MemberFlags.IsNameValid, XamlLanguage.IsValidXamlName(Name));

		public XamlValueConverter<XamlDeferringLoader> DeferringLoader => deferringLoader.HasValue ? deferringLoader.Value : deferringLoader.Set(LookupDeferringLoader());

		static readonly XamlMember[] empty_members = new XamlMember[0];

		public IList<XamlMember> DependsOn => LookupDependsOn() ?? empty_members;

		public XamlMemberInvoker Invoker => invoker ?? (invoker = LookupInvoker());

		public bool IsAmbient => flags.Get(MemberFlags.IsAmbient) ?? flags.Set(MemberFlags.IsAmbient, LookupIsAmbient());

		public bool IsEvent => flags.Get(MemberFlags.IsEvent) ?? flags.Set(MemberFlags.IsEvent, LookupIsEvent());

		public bool IsReadOnly => flags.Get(MemberFlags.IsReadOnly) ?? flags.Set(MemberFlags.IsReadOnly, LookupIsReadOnly());

		public bool IsReadPublic => flags.Get(MemberFlags.IsReadPublic) ?? flags.Set(MemberFlags.IsReadPublic, LookupIsReadPublic());

		public bool IsUnknown => flags.Get(MemberFlags.IsUnknown) ?? flags.Set(MemberFlags.IsUnknown, LookupIsUnknown());

		public bool IsWriteOnly => flags.Get(MemberFlags.IsWriteOnly) ?? flags.Set(MemberFlags.IsWriteOnly, LookupIsWriteOnly());

		public bool IsWritePublic => flags.Get(MemberFlags.IsWritePublic) ?? flags.Set(MemberFlags.IsWritePublic, LookupIsWritePublic());

		public XamlType TargetType => targetType.HasValue ? targetType.Value : targetType.Set(LookupTargetType());

		public XamlType Type => type.HasValue ? type.Value : type.Set(LookupType());

		public XamlValueConverter<TypeConverter> TypeConverter => typeConverter.HasValue ? typeConverter.Value : typeConverter.Set(LookupTypeConverter());

		public MemberInfo UnderlyingMember => underlying_member ?? (underlying_member = LookupUnderlyingMember());

		public XamlValueConverter<ValueSerializer> ValueSerializer => valueSerializer.HasValue ? valueSerializer.Value : valueSerializer.Set(LookupValueSerializer());

		public static bool operator ==(XamlMember left, XamlMember right)
		{
			return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.Equals(right);
		}

		public static bool operator !=(XamlMember left, XamlMember right)
		{
			return !(left == right);
		}

		public override bool Equals(object other)
		{
			var x = other as XamlMember;
			return Equals(x);
		}

		bool MemberEquals(MemberInfo member1, MemberInfo member2)
		{
			if (ReferenceEquals(member1, null))
				return ReferenceEquals(member2, null);
			return !ReferenceEquals(member2, null)
				&& member1.DeclaringType == member2.DeclaringType
				&& string.Equals(member1.Name, member2.Name, StringComparison.Ordinal);
		}


		public bool Equals(XamlMember other)
		{
			// this should be in general correct; XamlMembers are almost not comparable.
			if (ReferenceEquals(this, other))
				return true;
			// It does not compare XamlSchemaContext.
			return !ReferenceEquals(other, null) &&
				MemberEquals(underlying_member, other.underlying_member) &&
				MemberEquals(underlying_getter, other.underlying_getter) &&
				MemberEquals(underlying_setter, other.underlying_setter) &&
				Name == other.Name &&
				PreferredXamlNamespace == other.PreferredXamlNamespace &&
				ns == other.ns &&
				IsAttachable == other.IsAttachable;
		}

		public override int GetHashCode()
		{
			if (IsAttachable || string.IsNullOrEmpty(PreferredXamlNamespace))
			{
				if (DeclaringType == null)
					return Name.GetHashCode();
				else
					return DeclaringType.UnderlyingType.FullName.GetHashCode() ^ Name.GetHashCode();
			}
			else
				return PreferredXamlNamespace.GetHashCode() ^ DeclaringType.Name.GetHashCode() ^ Name.GetHashCode();
		}

		public override string ToString()
		{
			if (IsAttachable || String.IsNullOrEmpty(PreferredXamlNamespace))
			{
				if (DeclaringType == null)
					return Name;
				else
					return String.Concat(DeclaringType.UnderlyingType.FullName, ".", Name);
			}
			else
				return String.Concat("{", PreferredXamlNamespace, "}", DeclaringType?.Name, ".", Name);
		}

		public virtual IList<string> GetXamlNamespaces()
		{
			if (DeclaringType == null)
				return null;
			return DeclaringType.GetXamlNamespaces();
		}

		// lookups

		internal ICustomAttributeProvider CustomAttributeProvider => customAttributeProvider.HasValue ? customAttributeProvider.Value : customAttributeProvider.Set(LookupCustomAttributeProvider());

		protected internal virtual ICustomAttributeProvider LookupCustomAttributeProvider()
		{
			return UnderlyingMember != null ? context.GetCustomAttributeProvider(UnderlyingMember) : null;
		}

		protected virtual XamlValueConverter<XamlDeferringLoader> LookupDeferringLoader()
		{
			var attr = CustomAttributeProvider?.GetCustomAttribute<XamlDeferLoadAttribute>(true);
			if (attr == null)
				return Type?.DeferringLoader;
			var loaderType = attr.GetLoaderType();
			var contentType = attr.GetContentType();
			if (loaderType == null || contentType == null)
				throw new XamlSchemaException("Invalid metadata for attribute XamlDeferLoadAttribute");
			return new XamlValueConverter<XamlDeferringLoader>(loaderType, null); // why is the target type null here? thought it would be the contentType.
		}

		static readonly XamlMember[] empty_list = new XamlMember[0];

		protected virtual IList<XamlMember> LookupDependsOn()
		{
			return empty_list;
		}

		protected virtual XamlMemberInvoker LookupInvoker()
		{
			return new XamlMemberInvoker(this);
		}

		protected virtual bool LookupIsAmbient()
		{
			var ambientAttribute = CustomAttributeProvider?.GetCustomAttribute<AmbientAttribute>(true);
			return ambientAttribute != null;
		}

		protected virtual bool LookupIsEvent()
		{
			return flags.Get(MemberFlags.IsDefaultEvent) ?? false;
		}

		protected virtual bool LookupIsReadOnly()
		{
			return UnderlyingGetter != null && UnderlyingSetter == null;
		}

		protected virtual bool LookupIsReadPublic()
		{
			if (underlying_member == null)
				return true;
			if (UnderlyingGetter != null)
				return UnderlyingGetter.IsPublic;
			return false;
		}

		protected virtual bool LookupIsUnknown()
		{
			return underlying_member == null;
		}

		protected virtual bool LookupIsWriteOnly()
		{
			var pi = underlying_member as PropertyInfo;
			if (pi != null)
				return !pi.CanRead && pi.CanWrite;
			return UnderlyingGetter == null && UnderlyingSetter != null;
		}

		protected virtual bool LookupIsWritePublic()
		{
			if (underlying_member == null)
				return true;
			if (UnderlyingSetter != null)
				return UnderlyingSetter.IsPublic;
			return false;
		}

		protected virtual XamlType LookupTargetType()
		{
			return target_type;
		}

		protected virtual XamlType LookupType()
		{
			return context.GetXamlType(DoGetType());
		}

		Type DoGetType()
		{
			var pi = underlying_member as PropertyInfo;
			if (pi != null)
				return pi.PropertyType;
			var ei = underlying_member as EventInfo;
			if (ei != null)
				return ei.EventHandlerType;
			if (UnderlyingSetter != null)
				return UnderlyingSetter.GetParameters()[1].ParameterType;
			if (UnderlyingGetter != null)
			{
				if (IsAttachable)
					return UnderlyingGetter.ReturnType;
				else
					return UnderlyingGetter.GetParameters()[0].ParameterType;
			}
			return typeof(object);
		}


		protected virtual XamlValueConverter<TypeConverter> LookupTypeConverter()
		{
			var t = Type.UnderlyingType;
			if (t == null)
				return null;

			var converterName = CustomAttributeProvider.GetTypeConverterName(false);
			if (converterName != null)
				return context.GetValueConverter<TypeConverter>(System.Type.GetType(converterName), Type);
			if (IsEvent)
				return context.GetValueConverter<TypeConverter>(typeof(EventConverter), Type);

			return Type.TypeConverter;
		}

		protected virtual MethodInfo LookupUnderlyingGetter()
		{
			return underlying_getter;
		}

		protected virtual MemberInfo LookupUnderlyingMember()
		{
			return underlying_member;
		}

		protected virtual MethodInfo LookupUnderlyingSetter()
		{
			return underlying_setter;
		}

		protected virtual XamlValueConverter<ValueSerializer> LookupValueSerializer()
		{
			if (Type == null)
				return null;

			return XamlType.LookupValueSerializer(Type, CustomAttributeProvider) ?? Type.ValueSerializer;
		}

		void VerifyGetter(MethodInfo method)
		{
			if (method == null)
				return;
			if (method.GetParameters().Length != 1 || method.ReturnType == typeof(void))
				throw new ArgumentException(String.Format("Property getter for {0} must have exactly one argument and must have non-void return type.", Name));
		}

		void VerifyAdderSetter(MethodInfo method)
		{
			if (method == null)
				return;
			if (method.GetParameters().Length != 2)
				throw new ArgumentException(String.Format("Property getter or event adder for {0} must have exactly one argument and must have non-void return type.", Name));
		}

		ReferenceValue<DefaultValueAttribute> defaultValue;
		internal DefaultValueAttribute DefaultValue => defaultValue.HasValue ? defaultValue.Value : defaultValue.Set(CustomAttributeProvider?.GetCustomAttribute<DefaultValueAttribute>(true));

		internal bool IsConstructorArgument => flags.Get(MemberFlags.IsConstructorArgument) ?? flags.Set(MemberFlags.IsConstructorArgument, LookupIsConstructorArgument());

		bool LookupIsConstructorArgument()
		{
			var ap = CustomAttributeProvider;
			return ap != null && ap.GetCustomAttributes(typeof(ConstructorArgumentAttribute), false).Length > 0;
		}

		internal virtual bool RequiresChildNode => flags.Get(MemberFlags.RequiresChildNode) ?? flags.Set(MemberFlags.RequiresChildNode, LookupRequiresChildNode());

		bool LookupRequiresChildNode()
		{
			// if we can convert to & from string, we can write it as an attribute
			if (DeclaringType?.ContentProperty == this)
				return true;
			var canConvert = Type.CanConvertFrom(XamlLanguage.String) && Type.CanConvertTo(XamlLanguage.String);
			if (IsWritePublic && canConvert)
				return false;
			return Type.IsCollection
				|| Type.IsDictionary
				|| (IsWritePublic && !canConvert);
		}
	}
}
