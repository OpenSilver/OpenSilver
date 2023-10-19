//
// Copyright (C) 2010 Novell Inc. http://novell.com
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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Markup;
using System.Xaml.ComponentModel;
using System.Xaml.Markup;
using System.Xaml.Schema;
using System.Xml.Serialization;

namespace System.Xaml
{
	internal class ReferenceComparer : IEqualityComparer<object>
	{
		public static readonly ReferenceComparer Instance = new ReferenceComparer();
		bool IEqualityComparer<object>.Equals(object x, object y) => ReferenceEquals(x, y);

		public int GetHashCode(object obj) => obj.GetHashCode();
	}

    internal class XamlType : IEquatable<XamlType>
	{
		FlagValue flags;
		static class TypeFlags
		{
			public const int IsAmbient = 1 << 0;
			public const int IsConstructible = 1 << 1;
			public const int IsNullable = 1 << 2;
			public const int IsMarkupExtension = 1 << 3;
			public const int IsNameScope = 1 << 4;
			public const int IsNameValid = 1 << 5;
			public const int IsPublic = 1 << 6;
			public const int IsUnknown = 1 << 7;
			public const int IsUsableDuringInitialization = 1 << 8;
			public const int IsWhitespaceSignificantCollection = 1 << 9;
			public const int IsXData = 1 << 10;
			public const int TrimSurroundingWhitespace = 1 << 11;
			public const int ConstructionRequiresArguments = 1 << 12;
			public const int IsImmutable = 1 << 13;
			public const int IsImmutableCollection = 1 << 14;
			public const int ShouldSerialize = 1 << 15;
		}

		Type type, underlying_type;
		XamlTypeInvoker invoker;
		string explicit_ns;
		string internalXmlName;
		string preferredXamlNamespace;
		Dictionary<XamlDirective, XamlMember> aliased_property_cache;
		XamlCollectionKind? collectionKind;
		ReferenceValue<IList<XamlType>> allowedContentTypes;
		ReferenceValue<XamlType> baseType;
		ReferenceValue<XamlMember> contentProperty;
		ReferenceValue<IList<XamlType>> contentWrappers;
		ReferenceValue<XamlValueConverter<XamlDeferringLoader>> deferringLoader;
		ReferenceValue<XamlType> itemType;
		ReferenceValue<XamlType> keyType;
		ReferenceValue<XamlType> markupExtensionReturnType;
		ReferenceValue<XamlValueConverter<TypeConverter>> typeConverter;
		ReferenceValue<XamlValueConverter<ValueSerializer>> valueSerializer;
		ReferenceValue<ICustomAttributeProvider> attributeProvider;
		IList<XamlMember> attachableMembers;
		IList<XamlMember> allMembers;
		IList<string> xamlNamespaces;
		IList<XamlMember> constructorArguments;
		Dictionary<string, XamlMember> memberLookup;
		ReferenceValue<MethodInfo> shouldSerializeMethod;

		public XamlType(Type underlyingType, XamlSchemaContext schemaContext)
			: this(underlyingType, schemaContext, null)
		{
		}


		static readonly Type[] mscorlib_types = {
			Type.GetType("System.Collections.Generic.IList`1", false),
			typeof(bool),
			Type.GetType("System.Collections.ArrayList", false)
		};

		internal static readonly string[] mscorlib_assemblies = new string[] {
			/*"System.Collections.Generic",
			"System.Collections.NonGeneric",*/
			"System.Private.CoreLib"
		};

		/*mscorlib_types.Where(r => r != null)
																   .Select(r => r.GetTypeInfo().Assembly)
																   .Distinct()
																   .ToArray();*/
		//		static readonly Type [] predefined_types = {
		//				typeof (XData), typeof (Uri), typeof (TimeSpan), typeof (PropertyDefinition), typeof (MemberDefinition), typeof (Reference)
		//			};

		public XamlType(Type underlyingType, XamlSchemaContext schemaContext, XamlTypeInvoker invoker)
			: this(schemaContext, invoker)
		{
			if (underlyingType == null)
				throw new ArgumentNullException("underlyingType");
			type = underlyingType;
			underlying_type = type;

			XamlType xt;
			if (XamlLanguage.InitializingTypes)
			{
				// These are special. Only XamlLanguage members are with shorthand name.
				if (type == typeof(PropertyDefinition))
					Name = "Property";
				else if (type == typeof(MemberDefinition))
					Name = "Member";
				else
					Name = GetXamlName(type);
				preferredXamlNamespace = XamlLanguage.Xaml2006Namespace;
			}
			else if (!ReferenceEquals(xt = XamlLanguage.AllTypes.FirstOrDefault(t => t.UnderlyingType == type), null))
			{
				Name = xt.Name;
				preferredXamlNamespace = XamlLanguage.Xaml2006Namespace;
			}
			else
			{
				Name = GetXamlName(type);
			}
			if (type.GetTypeInfo().IsGenericType)
			{
				TypeArguments = type.GetTypeInfo().GetGenericArguments().Select(r => schemaContext.GetXamlType(r)).ToReadOnly();
			}
		}

		public XamlType(string unknownTypeNamespace, string unknownTypeName, IList<XamlType> typeArguments, XamlSchemaContext schemaContext)
			: this(schemaContext, null)
		{
			if (unknownTypeNamespace == null)
				throw new ArgumentNullException("unknownTypeNamespace");
			if (unknownTypeName == null)
				throw new ArgumentNullException("unknownTypeName");
			if (schemaContext == null)
				throw new ArgumentNullException("schemaContext");

			type = typeof(object);
			Name = unknownTypeName;
			preferredXamlNamespace = unknownTypeNamespace;
			TypeArguments = typeArguments == null || typeArguments.Count == 0 ? null : typeArguments.ToReadOnly();
			explicit_ns = unknownTypeNamespace;
		}

		protected XamlType(string typeName, IList<XamlType> typeArguments, XamlSchemaContext schemaContext)
			: this(String.Empty, typeName, typeArguments, schemaContext)
		{
		}

		XamlType(XamlSchemaContext schemaContext, XamlTypeInvoker invoker)
		{
			if (schemaContext == null)
				throw new ArgumentNullException("schemaContext");
			SchemaContext = schemaContext;
			this.invoker = invoker;
		}

		// populated properties

		internal EventHandler<XamlSetMarkupExtensionEventArgs> SetMarkupExtensionHandler
		{
			get { return LookupSetMarkupExtensionHandler(); }
		}

		internal EventHandler<XamlSetTypeConverterEventArgs> SetTypeConverterHandler
		{
			get { return LookupSetTypeConverterHandler(); }
		}

		public IList<XamlType> AllowedContentTypes => allowedContentTypes.HasValue ? allowedContentTypes.Value : allowedContentTypes.Set(LookupAllowedContentTypes());

		public XamlType BaseType => baseType.HasValue ? baseType.Value : baseType.Set(LookupBaseType());

		public bool ConstructionRequiresArguments => flags.Get(TypeFlags.ConstructionRequiresArguments) ?? flags.Set(TypeFlags.ConstructionRequiresArguments, LookupConstructionRequiresArguments());

		public XamlMember ContentProperty => contentProperty.HasValue ? contentProperty.Value : contentProperty.Set(LookupContentProperty());

		public IList<XamlType> ContentWrappers => contentWrappers.HasValue ? contentWrappers.Value : contentWrappers.Set(LookupContentWrappers());

		public XamlValueConverter<XamlDeferringLoader> DeferringLoader => deferringLoader.HasValue ? deferringLoader.Value : deferringLoader.Set(LookupDeferringLoader());

		public XamlTypeInvoker Invoker => invoker ?? (invoker = LookupInvoker());

		internal XamlCollectionKind CollectionKind => collectionKind ?? (collectionKind = LookupCollectionKind()).Value;

		public bool IsAmbient => flags.Get(TypeFlags.IsAmbient) ?? flags.Set(TypeFlags.IsAmbient, LookupIsAmbient());

		public bool IsArray => CollectionKind == XamlCollectionKind.Array;

		// it somehow treats array as not a collection...
		public bool IsCollection => CollectionKind == XamlCollectionKind.Collection;

		public bool IsConstructible => flags.Get(TypeFlags.IsConstructible) ?? flags.Set(TypeFlags.IsConstructible, LookupIsConstructible());

		public bool IsDictionary => CollectionKind == XamlCollectionKind.Dictionary;

		public bool IsGeneric => type.GetTypeInfo().IsGenericType;

		public bool IsMarkupExtension => flags.Get(TypeFlags.IsMarkupExtension) ?? flags.Set(TypeFlags.IsMarkupExtension, LookupIsMarkupExtension());

		public bool IsNameScope => flags.Get(TypeFlags.IsNameScope) ?? flags.Set(TypeFlags.IsNameScope, LookupIsNameScope());

		public bool IsNameValid => flags.Get(TypeFlags.IsNameValid) ?? flags.Set(TypeFlags.IsNameValid, XamlLanguage.IsValidXamlName(Name));

		public bool IsNullable => flags.Get(TypeFlags.IsNullable) ?? flags.Set(TypeFlags.IsNullable, LookupIsNullable());

		public bool IsPublic => flags.Get(TypeFlags.IsPublic) ?? flags.Set(TypeFlags.IsPublic, LookupIsPublic());

		public bool IsUnknown => flags.Get(TypeFlags.IsUnknown) ?? flags.Set(TypeFlags.IsUnknown, LookupIsUnknown());

		public bool IsUsableDuringInitialization => flags.Get(TypeFlags.IsUsableDuringInitialization) ?? flags.Set(TypeFlags.IsUsableDuringInitialization, LookupUsableDuringInitialization());

		public bool IsWhitespaceSignificantCollection => flags.Get(TypeFlags.IsWhitespaceSignificantCollection) ?? flags.Set(TypeFlags.IsWhitespaceSignificantCollection, LookupIsWhitespaceSignificantCollection());

		public bool IsXData => flags.Get(TypeFlags.IsXData) ?? flags.Set(TypeFlags.IsXData, LookupIsXData());

		public XamlType ItemType => itemType.HasValue ? itemType.Value : itemType.Set(LookupItemType());

		public XamlType KeyType => keyType.HasValue ? keyType.Value : keyType.Set(LookupKeyType());

		public XamlType MarkupExtensionReturnType => markupExtensionReturnType.HasValue ? markupExtensionReturnType.Value : markupExtensionReturnType.Set(LookupMarkupExtensionReturnType());

		public string Name { get; private set; }

		public string PreferredXamlNamespace => preferredXamlNamespace ?? (preferredXamlNamespace = LookupPreferredXamlNamespace());

		string LookupPreferredXamlNamespace()
		{
			var ns = SchemaContext.GetXamlNamespace(type.Namespace);
			if (ns != null)
				return ns;

			var assembly = type.GetTypeInfo().Assembly;
			var forwarded = assembly.GetCustomAttributes<TypeForwardedToAttribute>();
			var forwarded2 = assembly.GetCustomAttributes<TypeForwardedFromAttribute>();
			string assemblyName = assembly.GetName().Name;
			if (mscorlib_assemblies.Contains(assemblyName))
				assemblyName = "mscorlib";
			return $"clr-namespace:{type.Namespace};assembly={assemblyName}";
		}

		public XamlSchemaContext SchemaContext { get; private set; }

		public bool TrimSurroundingWhitespace => flags.Get(TypeFlags.TrimSurroundingWhitespace) ?? flags.Set(TypeFlags.TrimSurroundingWhitespace, LookupTrimSurroundingWhitespace());

		public IList<XamlType> TypeArguments { get; private set; }


		public XamlValueConverter<TypeConverter> TypeConverter => typeConverter.HasValue ? typeConverter.Value : typeConverter.Set(LookupTypeConverter());

		public Type UnderlyingType => underlying_type ?? (underlying_type = LookupUnderlyingType());

		public XamlValueConverter<ValueSerializer> ValueSerializer => valueSerializer.HasValue ? valueSerializer.Value : valueSerializer.Set(LookupValueSerializer());

		internal string InternalXmlName => internalXmlName ?? (internalXmlName = LookupInternalXmlName());

		string LookupInternalXmlName()
		{
			if (IsMarkupExtension && Name.EndsWith("Extension", StringComparison.Ordinal))
				return Name.Substring(0, Name.Length - 9);
			else
			{
				var stn = XamlLanguage.SpecialNames.FirstOrDefault(s => s.Type == this);
				return stn?.Name ?? Name;
			}
		}

		public static bool operator ==(XamlType left, XamlType right)
		{
			return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.Equals(right);
		}

		public static bool operator !=(XamlType left, XamlType right)
		{
			return !(left == right);
		}

		public bool Equals(XamlType other)
		{
			// It does not compare XamlSchemaContext.
			return !ReferenceEquals(other, null) &&
				UnderlyingType == other.UnderlyingType &&
				Name == other.Name &&
				PreferredXamlNamespace == other.PreferredXamlNamespace && TypeArguments.ListEquals(other.TypeArguments);
		}

		public override bool Equals(object obj)
		{
			var a = obj as XamlType;
			return Equals(a);
		}

		public override int GetHashCode()
		{
			if (UnderlyingType != null)
				return UnderlyingType.GetHashCode();
			int x = Name.GetHashCode() << 7 + PreferredXamlNamespace.GetHashCode();
			if (TypeArguments != null)
				foreach (var t in TypeArguments)
					x = t.GetHashCode() + x << 5;
			return x;
		}

		public override string ToString()
		{
			return new XamlTypeName(this).ToString();
			//return String.IsNullOrEmpty (PreferredXamlNamespace) ? Name : String.Concat ("{", PreferredXamlNamespace, "}", Name);
		}

		/// <summary>
		/// Gets a value indicating this type can be assigned or converted from the specified type.
		/// </summary>
		/// <returns><c>true</c>, if it can be assigned or converted from the input type , <c>false</c> otherwise.</returns>
		/// <param name="xamlType">Type to convert from.</param>
		[EnhancedXaml]
		public bool CanConvertFrom(XamlType xamlType)
		{
			if (xamlType.CanAssignTo(this))
				return true;

			var tc = TypeConverter?.ConverterInstance;
			if (tc != null)
			{
				return tc.CanConvertFrom(xamlType?.UnderlyingType ?? typeof(object));
			}

			return false;
		}

		/// <summary>
		/// Gets a value indicating this type can be assigned or converted to the specified type
		/// </summary>
		/// <returns><c>true</c>, if it can be assigned or converted to the input type , <c>false</c> otherwise.</returns>
		/// <param name="xamlType">Type to convert to.</param>
		[EnhancedXaml]
		public bool CanConvertTo(XamlType xamlType)
		{
			if (CanAssignTo(xamlType))
				return true;

			var tc = TypeConverter?.ConverterInstance;
			if (tc != null)
			{
				return tc.CanConvertTo(xamlType?.UnderlyingType ?? typeof(object));
			}

			return false;
		}

		[EnhancedXaml]
		public bool IsImmutable => flags.Get(TypeFlags.IsImmutable) ?? flags.Set(TypeFlags.IsImmutable, LookupIsImmutable());

		[EnhancedXaml]
		protected virtual bool LookupIsImmutable()
		{
			return IsImmutableCollection;
		}

		internal bool IsImmutableCollection => flags.Get(TypeFlags.IsImmutableCollection) ?? flags.Set(TypeFlags.IsImmutableCollection, LookupIsImmutableCollection());

		bool LookupIsImmutableCollection()
		{
			if (UnderlyingType == null)
				return false;

			var ti = UnderlyingType.GetTypeInfo();
			if (!ti.IsGenericType || !ti.Assembly.FullName.StartsWith("System.Collections.Immutable,", StringComparison.Ordinal))
				return false;

			var typeDef = ti.GetGenericTypeDefinition();
			var name = typeDef.FullName;
			if (!name.StartsWith("System.Collections.Immutable.Immutable", StringComparison.Ordinal))
				return false;

			return name.EndsWith("`1", StringComparison.Ordinal) || name.EndsWith("`2", StringComparison.Ordinal);
		}

		public virtual bool CanAssignTo(XamlType xamlType)
		{
			if (ReferenceEquals(xamlType, null))
				return false;

			if (IsUnknown && xamlType.IsUnknown)
				return Equals(xamlType);

			if (UnderlyingType == null)
				return ReferenceEquals(xamlType, XamlLanguage.Object);
			var ut = (xamlType.UnderlyingType ?? typeof(object)).GetTypeInfo();

			// if we are assigning to a nullable type, we allow null
			if (ReferenceEquals(this, XamlLanguage.Null)
				&& ut.IsValueType
				&& ut.IsGenericType
				&& ut.GetGenericTypeDefinition() == typeof(Nullable<>)
				)
				return true;

			if (xamlType.IsImmutableCollection && (IsCollection || IsDictionary))
				return true;

			return ut.IsAssignableFrom(UnderlyingType.GetTypeInfo());
		}

		public XamlMember GetAliasedProperty(XamlDirective directive)
		{
			XamlMember member;
			if (aliased_property_cache == null)
				aliased_property_cache = new Dictionary<XamlDirective, XamlMember>(ReferenceComparer.Instance);
			else if (aliased_property_cache.TryGetValue(directive, out member))
				return member;
			member = LookupAliasedProperty(directive);
			aliased_property_cache[directive] = member;
			return member;
		}

		public ICollection<XamlMember> GetAllAttachableMembers()
		{
			return attachableMembers ?? (attachableMembers = LookupAllAttachableMembers().ToReadOnly());
		}

		internal IList<XamlMember> GetAllMembersAsList() => (IList<XamlMember>)GetAllMembers();

		public ICollection<XamlMember> GetAllMembers()
		{
			if (allMembers != null)
				return allMembers;
			var list = LookupAllMembers().ToList();
			// set allMembers before sorting as it will attempt to lookup a member which causes a stack overflow
			allMembers = list;
			list.Sort(TypeExtensionMethods.MemberComparer);
			allMembers = new ReadOnlyCollection<XamlMember>(list);
			return allMembers;
		}

		public XamlMember GetAttachableMember(string name)
		{
			return LookupAttachableMember(name);
		}

		public XamlMember GetMember(string name)
		{
			XamlMember member;
			if (memberLookup == null)
				memberLookup = new Dictionary<string, XamlMember>();
			else if (memberLookup.TryGetValue(name, out member))
				return member;
			return memberLookup[name] = LookupMember(name, true);
		}

		public IList<XamlType> GetPositionalParameters(int parameterCount)
		{
			return LookupPositionalParameters(parameterCount);
		}

		public virtual IList<string> GetXamlNamespaces()
		{
			if (xamlNamespaces != null)
				return xamlNamespaces;
			xamlNamespaces = DoGetXamlNamespaces().ToReadOnly();
			return xamlNamespaces;
		}

		IEnumerable<string> DoGetXamlNamespaces()
		{
			// not quite sure if this is correct, but matches documentation to get all namespaces that type exists in
			if (!string.IsNullOrEmpty(explicit_ns))
				yield return explicit_ns;

			if (type != null)
			{
				var assemblyName = type.GetTypeInfo().Assembly.GetName().Name;
				if (mscorlib_assemblies.Contains(assemblyName))
					assemblyName = "mscorlib";
				// type always exists in clr namespace
				yield return $"clr-namespace:{type.Namespace};assembly={assemblyName}";

				// check if it's a built-in type
				if (XamlLanguage.AllTypes.Any(r => r.UnderlyingType == type))
				{
					yield return XamlLanguage.Xaml2006Namespace;
				}

				// check all other registered namespaces (may duplicate for built-in types, such as xaml markup extensions)
				foreach (var ns in SchemaContext.GetAllXamlNamespaces())
				{
					if (SchemaContext.GetAllXamlTypes(ns).Any(r => r.UnderlyingType == type))
					{
						yield return ns;
					}
				}
			}
		}

		// lookups

		protected virtual XamlMember LookupAliasedProperty(XamlDirective directive)
		{
			XamlMember member = null;

			if (ReferenceEquals(directive, XamlLanguage.Key))
			{
				var a = this.GetCustomAttribute<DictionaryKeyPropertyAttribute>();
				member = a != null ? GetMember(a.Name) : null;
			}
			else if (ReferenceEquals(directive, XamlLanguage.Name))
			{
				var a = this.GetCustomAttribute<RuntimeNamePropertyAttribute>();
				member = a != null ? GetMember(a.Name) : null;
			}
			else if (ReferenceEquals(directive, XamlLanguage.Uid))
			{
				var a = this.GetCustomAttribute<UidPropertyAttribute>();
				member = a != null ? GetMember(a.Name) : null;
			}
			else if (ReferenceEquals(directive, XamlLanguage.Lang))
			{
				var a = this.GetCustomAttribute<XmlLangPropertyAttribute>();
				member = a != null ? GetMember(a.Name) : null;
			}
			return member;
		}

		protected virtual IEnumerable<XamlMember> LookupAllAttachableMembers()
		{
			if (UnderlyingType == null)
				return BaseType != null ? BaseType.GetAllAttachableMembers() : Enumerable.Empty<XamlMember>();
			return DoLookupAllAttachableMembers().OrderBy(r => r, TypeExtensionMethods.MemberComparer);
		}

		IEnumerable<XamlMember> DoLookupAllAttachableMembers()
		{
			// based on http://msdn.microsoft.com/en-us/library/ff184560.aspx

			var gl = new Dictionary<string, MethodInfo>();
			var sl = new Dictionary<string, MethodInfo>();
			var al = new Dictionary<string, MethodInfo>();
			//var rl = new Dictionary<string,MethodInfo> ();
			var nl = new List<string>();
			foreach (var mi in UnderlyingType.GetMethods())
			{
				if (!mi.IsStatic)
					continue;
				string name = null;
				if (mi.Name.StartsWith("Get", StringComparison.Ordinal))
				{
					if (mi.ReturnType == typeof(void))
						continue;
					var args = mi.GetParameters();
					if (args.Length != 1)
						continue;
					name = mi.Name.Substring(3);
					gl.Add(name, mi);
				}
				else if (mi.Name.StartsWith("Set", StringComparison.Ordinal))
				{
					// looks like the return type is *ignored*
					//if (mi.ReturnType != typeof (void))
					//	continue;
					var args = mi.GetParameters();
					if (args.Length != 2)
						continue;
					name = mi.Name.Substring(3);
					sl.Add(name, mi);
				}
				else if (mi.Name.EndsWith("Handler", StringComparison.Ordinal))
				{
					var args = mi.GetParameters();
					if (args.Length != 2)
						continue;
					if (mi.Name.StartsWith("Add", StringComparison.Ordinal))
					{
						name = mi.Name.Substring(3, mi.Name.Length - 3 - 7);
						al.Add(name, mi);
					}/* else if (mi.Name.StartsWith ("Remove", StringComparison.Ordinal)) {
						name = mi.Name.Substring (6, mi.Name.Length - 6 - 7);
						rl.Add (name, mi);
					}*/
				}
				if (name != null && !nl.Contains(name))
					nl.Add(name);
			}

			foreach (var name in nl)
			{
				MethodInfo m;
				var g = gl.TryGetValue(name, out m) ? m : null;
				var s = sl.TryGetValue(name, out m) ? m : null;
				if (g != null || s != null)
					yield return SchemaContext.GetAttachableProperty(name, g, s);
				var a = al.TryGetValue(name, out m) ? m : null;
				//var r = rl.TryGetValue (name, out m) ? m : null;
				if (a != null)
					yield return SchemaContext.GetAttachableEvent(name, a);
			}
		}

		protected virtual IEnumerable<XamlMember> LookupAllMembers()
		{
			if (UnderlyingType == null)
				return BaseType?.GetAllMembers() ?? Enumerable.Empty<XamlMember>();
			return DoLookupAllMembers();
		}

		IEnumerable<XamlMember> DoLookupAllMembers()
		{
			// This is a hack that is likely required due to internal implementation difference in System.Uri. Our Uri has two readonly collection properties
			if (ReferenceEquals(this, XamlLanguage.Uri))
				yield break;

			//var bf = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

			foreach (var pi in UnderlyingType.GetRuntimeProperties())
			{
				if (pi.GetPrivateGetMethod()?.IsStatic ?? pi.GetPrivateSetMethod()?.IsStatic ?? false)
					continue;
				if (pi.Name.Contains(".")) // exclude explicit interface implementations.
					continue;
				if (/*pi.CanRead &&*/
					(
						pi.CanWrite
						|| IsCollectionType(pi.PropertyType)
						|| typeof(IXmlSerializable).GetTypeInfo().IsAssignableFrom(pi.PropertyType.GetTypeInfo())
						|| pi.GetCustomAttribute<ConstructorArgumentAttribute>() != null
					)
					&& pi.GetIndexParameters().Length == 0)
					yield return SchemaContext.GetProperty(pi);
			}
			foreach (var ei in UnderlyingType.GetRuntimeEvents())
				yield return SchemaContext.GetEvent(ei);
		}

		static bool IsPublicAccessor(MethodInfo mi)
		{
			return mi != null && mi.IsPublic;
		}

		bool IsCollectionType(Type type)
		{
			if (type == null)
				return false;
			var xt = SchemaContext.GetXamlType(type);
			return xt.CollectionKind != XamlCollectionKind.None;
		}

		protected virtual IList<XamlType> LookupAllowedContentTypes()
		{
			// the actual implementation is very different from what is documented :(
			return null;

			/*
			var l = new List<XamlType> ();
			if (ContentWrappers != null)
				l.AddRange (ContentWrappers);
			if (ContentProperty != null)
				l.Add (ContentProperty.Type);
			if (ItemType != null)
				l.Add (ItemType);
			return l.Count > 0 ? l : null;
			*/
		}

		protected virtual XamlMember LookupAttachableMember(string name)
		{
			return GetAllAttachableMembers().FirstOrDefault(m => m.Name == name);
		}

		protected virtual XamlType LookupBaseType()
		{
			if (UnderlyingType == null)
				return SchemaContext.GetXamlType(typeof(object));
			else
				return type.GetTypeInfo().BaseType == null || type.GetTypeInfo().BaseType == typeof(object) ? null : SchemaContext.GetXamlType(type.GetTypeInfo().BaseType);
		}

		// This implementation is not verified. (No place to use.)
		protected virtual XamlCollectionKind LookupCollectionKind()
		{
			if (UnderlyingType == null)
				return BaseType != null ? BaseType.CollectionKind : XamlCollectionKind.None;
			else if (type.IsArray)
				return XamlCollectionKind.Array;

			else if (type.ImplementsAnyInterfacesOf(typeof(IDictionary), typeof(IDictionary<,>)))
				return XamlCollectionKind.Dictionary;

			else if (type.ImplementsAnyInterfacesOf(typeof(IList), typeof(ICollection<>)))
				return XamlCollectionKind.Collection;
			else if (IsImmutableCollection && type.ImplementsAnyInterfacesOf(typeof(IEnumerable))) // stack/queue
				return XamlCollectionKind.Collection;
			else
				return XamlCollectionKind.None;
		}

		protected virtual bool LookupConstructionRequiresArguments()
		{
			if (UnderlyingType == null)
				return false;

			var typeInfo = UnderlyingType.GetTypeInfo();

			if (typeInfo.IsValueType)
				return false;

			// not sure if it is required, but MemberDefinition return true while they are abstract and it makes no sense.
			if (typeInfo.IsAbstract)
				return true;

			// FIXME: probably some primitive types are treated as special.
			if (typeof(string).GetTypeInfo().IsAssignableFrom(typeInfo))
				return true;
			if (typeof(TimeSpan) == UnderlyingType)
				return false;


			return typeInfo.GetConstructors().Where(r => r.IsPublic).All(r => r.GetParameters().Length > 0);
		}

		protected virtual XamlMember LookupContentProperty()
		{
			var a = this.GetCustomAttribute<ContentPropertyAttribute>();
			return a != null && a.Name != null ? GetMember(a.Name) : null;
		}

		protected virtual IList<XamlType> LookupContentWrappers()
		{
			if (CustomAttributeProvider == null)
				return null;

			var arr = CustomAttributeProvider.GetCustomAttributes(typeof(ContentWrapperAttribute), false);
			if (arr == null || arr.Length == 0)
				return null;
			var l = new XamlType[arr.Length];
			for (int i = 0; i < l.Length; i++)
				l[i] = SchemaContext.GetXamlType(((ContentWrapperAttribute)arr[i]).ContentWrapper);
			return l;
		}

		internal ICustomAttributeProvider CustomAttributeProvider => attributeProvider.HasValue ? attributeProvider.Value : attributeProvider.Set(LookupCustomAttributeProvider());

		protected internal virtual ICustomAttributeProvider LookupCustomAttributeProvider()
		{
			return UnderlyingType != null ? SchemaContext.GetCustomAttributeProvider(UnderlyingType) : null;
		}

		protected virtual XamlValueConverter<XamlDeferringLoader> LookupDeferringLoader()
		{
			if (UnderlyingType == null)
				return null;
			var attr = this.GetCustomAttribute<XamlDeferLoadAttribute>();
			if (attr == null)
				return null;
			var loaderType = attr.GetLoaderType();
			var contentType = attr.GetContentType();
			if (loaderType == null || contentType == null)
				throw new XamlSchemaException("Invalid metadata for attribute XamlDeferLoadAttribute");
			return new XamlValueConverter<XamlDeferringLoader>(loaderType, null); // Why is the targetType null in System.Xaml?
		}

		protected virtual XamlTypeInvoker LookupInvoker()
		{
			return new XamlTypeInvoker(this);
		}

		protected virtual bool LookupIsAmbient()
		{
			return this.GetCustomAttribute<AmbientAttribute>() != null;
		}

		// It is documented as if it were to reflect spec. section 5.2,
		// but the actual behavior shows it is *totally* wrong.
		// Here I have implemented this based on the nunit test results. sigh.
		protected virtual bool LookupIsConstructible()
		{
			if (UnderlyingType == null)
				return true;
			if (IsMarkupExtension)
				return true;
			if (UnderlyingType.GetTypeInfo().IsAbstract)
				return false;
			if (!IsNameValid)
				return false;
			return true;
		}

		protected virtual bool LookupIsMarkupExtension()
		{
			return UnderlyingType != null && typeof(MarkupExtension).GetTypeInfo().IsAssignableFrom(UnderlyingType.GetTypeInfo());
		}

		protected virtual bool LookupIsNameScope()
		{
			return UnderlyingType != null && typeof(INameScope).GetTypeInfo().IsAssignableFrom(UnderlyingType.GetTypeInfo());
		}

		protected virtual bool LookupIsNullable()
		{
			return !type.GetTypeInfo().IsValueType || type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		protected virtual bool LookupIsPublic()
		{
			return underlying_type == null || underlying_type.GetTypeInfo().IsPublic || underlying_type.GetTypeInfo().IsNestedPublic;
		}

		protected virtual bool LookupIsUnknown()
		{
			return UnderlyingType == null;
		}

		protected virtual bool LookupIsWhitespaceSignificantCollection()
		{
			// probably for unknown types, it should preserve whitespaces.
			return IsUnknown || this.GetCustomAttribute<WhitespaceSignificantCollectionAttribute>() != null;
		}

		protected virtual bool LookupIsXData()
		{
			return ReflectionHelpers.IXmlSerializableType != null && CanAssignTo(SchemaContext.GetXamlType(ReflectionHelpers.IXmlSerializableType));
		}

		protected virtual XamlType LookupItemType()
		{
			XamlType itemType;

			var kind = CollectionKind;
			if (kind == XamlCollectionKind.Array)
				itemType = SchemaContext.GetXamlType(type.GetElementType());
			else if (kind == XamlCollectionKind.Dictionary)
			{
				if (!IsGeneric)
					itemType = SchemaContext.GetXamlType(typeof(object));
				else
					itemType = SchemaContext.GetXamlType(type.GetTypeInfo().GetGenericArguments()[1]);
			}
			else if (kind != XamlCollectionKind.Collection)
				return null;
			else if (!IsGeneric)
			{
				// support custom collections that inherit ICollection<T>
				var collectionType = type.GetTypeInfo().GetInterfaces().FirstOrDefault(r => r.GetTypeInfo().IsGenericType && r.GetGenericTypeDefinition() == typeof(ICollection<>));
				if (collectionType != null)
					itemType = SchemaContext.GetXamlType(collectionType.GetTypeInfo().GetGenericArguments()[0]);
				else
					itemType = SchemaContext.GetXamlType(typeof(object));
			}
			else
				itemType = SchemaContext.GetXamlType(type.GetTypeInfo().GetGenericArguments()[0]);
			return itemType;
		}

		protected virtual XamlType LookupKeyType()
		{
			if (!IsDictionary)
				return null;
			if (!IsGeneric)
				return SchemaContext.GetXamlType(typeof(object));
			return SchemaContext.GetXamlType(type.GetTypeInfo().GetGenericArguments()[0]);
		}

		protected virtual XamlType LookupMarkupExtensionReturnType()
		{
			var a = this.GetCustomAttribute<MarkupExtensionReturnTypeAttribute>();
			return a != null ? SchemaContext.GetXamlType(a.ReturnType) : null;
		}

		protected virtual XamlMember LookupMember(string name, bool skipReadOnlyCheck)
		{
			// FIXME: verify if this does not filter out events.
			return GetAllMembers().FirstOrDefault(m => m.Name == name && (skipReadOnlyCheck || !m.IsReadOnly || m.Type.IsCollection || m.Type.IsDictionary || m.Type.IsArray));
		}

		protected virtual IList<XamlType> LookupPositionalParameters(int parameterCount)
		{
			if (UnderlyingType == null/* || !IsMarkupExtension*/) // see nunit tests...
				return null;

			// check if there is applicable ConstructorArgumentAttribute.
			// If there is, then return its type.
			if (parameterCount == 1)
			{
				foreach (var xm in GetAllMembers())
				{
					var ca = xm.CustomAttributeProvider.GetCustomAttribute<ConstructorArgumentAttribute>(false);
					if (ca != null)
						return new XamlType[] { xm.Type };
				}
			}

			var methods = (from m in UnderlyingType.GetTypeInfo().GetConstructors() where m.GetParameters().Length == parameterCount select m).ToList();
			if (methods.Count == 1)
				return (from p in methods[0].GetParameters() select SchemaContext.GetXamlType(p.ParameterType)).ToList();

			if (SchemaContext.SupportMarkupExtensionsWithDuplicateArity)
				throw new NotSupportedException("The default LookupPositionalParameters implementation does not allow duplicate arity of markup extensions");
			return null;
		}

		//static readonly BindingFlags flags_get_static = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

		protected virtual EventHandler<XamlSetMarkupExtensionEventArgs> LookupSetMarkupExtensionHandler()
		{
			var a = this.GetCustomAttribute<XamlSetMarkupExtensionAttribute>();
			if (a == null)
				return null;
			var mi = type.GetRuntimeMethods().FirstOrDefault(r => r.Name == a.XamlSetMarkupExtensionHandler && r.IsStatic);
			if (mi == null)
				throw new ArgumentException("Binding to XamlSetMarkupExtensionHandler failed");
			return (EventHandler<XamlSetMarkupExtensionEventArgs>)mi.CreateDelegate(typeof(EventHandler<XamlSetMarkupExtensionEventArgs>));
		}

		protected virtual EventHandler<XamlSetTypeConverterEventArgs> LookupSetTypeConverterHandler()
		{
			var a = this.GetCustomAttribute<XamlSetTypeConverterAttribute>();
			if (a == null)
				return null;
			var mi = type.GetRuntimeMethods().FirstOrDefault(r => r.Name == a.XamlSetTypeConverterHandler && r.IsStatic);
			if (mi == null)
				throw new ArgumentException("Binding to XamlSetTypeConverterHandler failed");
			return (EventHandler<XamlSetTypeConverterEventArgs>)mi.CreateDelegate(typeof(EventHandler<XamlSetTypeConverterEventArgs>));
		}

		protected virtual bool LookupTrimSurroundingWhitespace()
		{
			return this.GetCustomAttribute<TrimSurroundingWhitespaceAttribute>() != null;
		}

		protected virtual XamlValueConverter<TypeConverter> LookupTypeConverter()
		{
			var t = UnderlyingType;
			if (t == null)
				return null;

			var tc = TypeConverterHelper.GetConverter(t);

			if (tc != null)
			{
                var vc = SchemaContext.GetValueConverter<TypeConverter>(tc.GetType(), this);
                vc.ConverterInstance = tc;
                return vc;
            }

			return null;
		}

		protected virtual Type LookupUnderlyingType()
		{
			return underlying_type;
		}

		protected virtual bool LookupUsableDuringInitialization()
		{
			var a = this.GetCustomAttribute<UsableDuringInitializationAttribute>();
			return a != null && a.Usable;
		}

		protected virtual XamlValueConverter<ValueSerializer> LookupValueSerializer()
		{
			return LookupValueSerializer(this, CustomAttributeProvider);
		}

		internal static XamlValueConverter<ValueSerializer> LookupValueSerializer(XamlType targetType, ICustomAttributeProvider provider)
		{
			if (provider == null)
				return null;

			var a = provider.GetCustomAttribute<ValueSerializerAttribute>(true);
			if (a != null)
				return new XamlValueConverter<ValueSerializer>(a.ValueSerializerType ?? Type.GetType(a.ValueSerializerTypeName), targetType);

			if (targetType.BaseType != null)
			{
				var ret = targetType.BaseType.ValueSerializer;
				if (ret != null)
					return ret;
			}

			if (targetType.UnderlyingType == typeof(string))
				return new XamlValueConverter<ValueSerializer>(typeof(StringValueSerializer), targetType);

			if (targetType.UnderlyingType == typeof(DateTime))
				return new XamlValueConverter<ValueSerializer>(typeof(DateTimeValueSerializer), targetType);

			return null;
		}

		/// <summary>
		/// Visability object during serialization. This property lookup attribute <see cref="System.ComponentModel.DesignerSerializationVisibilityAttribute"/>
		/// </summary>
		[EnhancedXaml]
		public DesignerSerializationVisibility SerializationVisibility
		{
			get
			{
				var c = this.CustomAttributeProvider;
				var a = c == null ? null : c.GetCustomAttribute<DesignerSerializationVisibilityAttribute>(false);
				return a != null ? a.Visibility : DesignerSerializationVisibility.Visible;
			}
		}

		/// <summary>
		/// Check instance can it serialize using <see cref="LookupShouldSerialize"/> and check the ShouldSerialize method in underlaing class 
		/// </summary>
		/// <param name="instance">The instance of the object (instance type must equals <see cref="UnderlyingType"/>)</param>
		internal bool ShouldSerialize(object instance)
		{
			var shouldSerialize = flags.Get(TypeFlags.ShouldSerialize) ?? flags.Set(TypeFlags.ShouldSerialize, LookupShouldSerialize());

			if (!shouldSerialize)
				return false;

			if (!shouldSerializeMethod.HasValue)
				shouldSerializeMethod.Set(LookupShouldSerializeMethod());

			if (shouldSerializeMethod.Value != null)
			{
				return (bool)shouldSerializeMethod.Value.Invoke(instance, null);
			}

			return true;
		}

		/// <summary>
		/// Lookup the method with name "ShouldSerialize" with return type is boolean and 0 arguments
		/// </summary>
		/// <returns>Method info if method found and null if not</returns>
		MethodInfo LookupShouldSerializeMethod()
		{
			var attr = this.CustomAttributeProvider.GetCustomAttribute<ShouldSerializeAttribute>(false);
			if (attr != null)
			{
				var methods = UnderlyingType?.GetTypeInfo().GetDeclaredMethods(attr.MethodName);
				if (methods != null)
					foreach (var method in methods)
					{
						if (method.GetParameters().Length == 0 && method.ReturnType == typeof(bool))
						{
							return method;
						}
					}
			}

			return null;
		}

		/// <summary>
		/// Lookup SerializationVisability attribute 
		/// </summary>
		bool LookupShouldSerialize()
		{
			bool shouldSerialize = true;
			shouldSerialize &= SerializationVisibility != DesignerSerializationVisibility.Hidden;
			return shouldSerialize;
		}

		static string GetXamlName(Type type)
		{
			string n;
			if (!type.GetTypeInfo().IsNestedPublic && !type.GetTypeInfo().IsNestedAssembly && !type.GetTypeInfo().IsNestedPrivate)
				n = type.Name;
			else
				n = GetXamlName(type.DeclaringType) + "+" + type.Name;
			if (type.GetTypeInfo().IsGenericType && !type.GetTypeInfo().ContainsGenericParameters) // the latter condition is to filter out "nested non-generic type within generic type".
				return n.Substring(0, n.IndexOf('`'));
			else
				return n;
		}

		internal IList<XamlMember> GetConstructorArguments()
		{
			if (constructorArguments != null)
				return constructorArguments;

			constructorArguments = GetAllMembers()
				.Where(m =>
				   m.UnderlyingMember != null
				   && m.CustomAttributeProvider.GetCustomAttribute<ConstructorArgumentAttribute>(false) != null
				)
				.ToList();
			return constructorArguments;
		}

		internal bool IsMutableDefault(object instance)
		{
			if (!IsImmutableCollection)
				return false;
			var isDefaultProperty = UnderlyingType?.GetRuntimeProperty("IsDefault");
			if (isDefaultProperty != null)
			{
				var isDefault = isDefaultProperty.GetValue(instance, null) as bool?;
				if (isDefault == true)
					return true;
			}
			return false;
		}
	}
}
