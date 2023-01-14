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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using System.Xaml.Markup;

namespace System.Xaml.Schema
{
    internal class XamlTypeInvoker
	{
		static readonly XamlTypeInvoker unknown = new XamlTypeInvoker();
		public static XamlTypeInvoker UnknownInvoker => unknown;

		protected XamlTypeInvoker()
		{
		}

		public XamlTypeInvoker(XamlType type)
		{
			if (ReferenceEquals(type, null))
				throw new ArgumentNullException("type");
			Type = type;
		}

		ConcurrentDictionary<object, object> cache;

		static object s_CreateImmutableFromMutableKey = new object();
		static object s_MutableTypeKey = new object();

		bool TryGetCache<T>(object key, out T value)
			where T : class
		{
			if (cache == null)
				cache = new ConcurrentDictionary<object, object>();

			object obj;
			if (!cache.TryGetValue(key, out obj))
			{
				value = default(T);
				return false;
			}
			value = obj as T;
			return !ReferenceEquals(value, null);
		}

		[EnhancedXaml]
		protected XamlType Type { get; }

		void ThrowIfUnknown()
		{
			if (ReferenceEquals(Type, null) || Type.UnderlyingType == null)
				throw new NotSupportedException(string.Format("Current operation is valid only when the underlying type on a XamlType is known, but it is unknown for '{0}'", Type));
		}

		public EventHandler<XamlSetMarkupExtensionEventArgs> SetMarkupExtensionHandler => Type?.SetMarkupExtensionHandler;

		public EventHandler<XamlSetTypeConverterEventArgs> SetTypeConverterHandler => Type?.SetTypeConverterHandler;


		public virtual void AddToCollection(object instance, object item)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");
			if (item == null)
				throw new ArgumentNullException("item");

			var collectionType = instance.GetType();
			var itemType = item.GetType();
			var key = Tuple.Create(collectionType, itemType);

			var mode = Type?.SchemaContext.InvokerOptions ?? XamlInvokerOptions.None;
			if (mode.HasFlag(XamlInvokerOptions.Compile))
			{
				Action<object, object> addDelegate;
				if (TryGetCache(key, out addDelegate))
				{
					addDelegate(instance, item);
					return;
				}

				addDelegate = CreateAddDelegate(instance, item, collectionType, itemType, key, mode);

				try
				{
					addDelegate(instance, item);
				}
				catch (TargetInvocationException e)
				{
					ExceptionDispatchInfo.Capture(e.InnerException).Throw();
				}
			}
			else
			{
				MethodInfo mi;
				if (TryGetCache(key, out mi))
				{
					mi.Invoke(instance, new object[] { item });
					return;
				}

				mi = LookupAddCollectionMethod(Type, collectionType, itemType);
				if (mi == null)
					throw new InvalidOperationException($"The collection type '{collectionType}' does not have 'Add' method");
				// FIXME: this method lookup should be mostly based on GetAddMethod(). At least iface method lookup must be done there.
				cache[key] = mi;
				mi.Invoke(instance, new object[] { item });
			}
		}

		Action<object, object> CreateAddDelegate(object instance, object item, Type collectionType, Type itemType, Tuple<Type, Type> key, XamlInvokerOptions mode)
		{
			// this is separate as the anonymous types are instantiated at the beginning of the method
			Action<object, object> addDelegate;
			MethodInfo mi = LookupAddCollectionMethod(Type, collectionType, itemType);
			if (mi == null)
				throw new InvalidOperationException($"The collection type '{collectionType}' does not have 'Add' method");

			if (mode.HasFlag(XamlInvokerOptions.DeferCompile))
			{
				cache[key] = addDelegate = (i, v) => mi.Invoke(i, new object[] { v });
				Task.Factory.StartNew(() => cache[key] = addDelegate = mi.BuildCallExpression());
			}
			else
			{
				cache[key] = addDelegate = mi.BuildCallExpression();
			}
			return addDelegate;
		}

		MethodInfo LookupAddCollectionMethod(XamlType type, Type collectionType, Type itemType)
		{
			// FIXME: this method lookup should be mostly based on GetAddMethod(). At least iface method lookup must be done there.
			MethodInfo mi = null;
			if (type != null && type.UnderlyingType != null)
			{
				var xct = type.SchemaContext.GetXamlType(collectionType);
				if (!xct.IsCollection) // not sure why this check is done only when UnderlyingType exists...
					throw new NotSupportedException(String.Format("Non-collection type '{0}' does not support this operation", xct));
				if (typeof(IList).GetTypeInfo().IsAssignableFrom(collectionType.GetTypeInfo()))
					mi = typeof(IList).GetTypeInfo().GetDeclaredMethod("Add");
				else if (collectionType.GetTypeInfo().IsAssignableFrom(type.UnderlyingType.GetTypeInfo()))
					mi = GetAddMethod(type.SchemaContext.GetXamlType(itemType));
			}

			if (mi == null)
			{
				var baseCollection = collectionType.GetTypeInfo().GetInterfaces()
												   .FirstOrDefault(r => r.GetTypeInfo().IsGenericType
																   && r.GetTypeInfo().GetGenericTypeDefinition() == typeof(ICollection<>));
				if (baseCollection != null)
				{
					mi = collectionType.GetRuntimeMethod("Add", baseCollection.GetTypeInfo().GetGenericArguments());
					if (mi == null)
						mi = LookupAddMethod(collectionType, baseCollection);
				}
				else
				{
					mi = collectionType.GetRuntimeMethod("Add", new Type[] { typeof(object) });
					if (mi == null)
						mi = LookupAddMethod(collectionType, typeof(IList));
				}
			}

			return mi;

		}

		public virtual void AddToDictionary(object instance, object key, object item)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");

			var instanceType = instance.GetType();

			var lookupKey = Tuple.Create(instanceType, key?.GetType(), item?.GetType());

			var mode = Type?.SchemaContext.InvokerOptions ?? XamlInvokerOptions.None;
			if (mode.HasFlag(XamlInvokerOptions.Compile))
			{
				Action<object, object, object> addDelegate;
				if (TryGetCache(lookupKey, out addDelegate))
				{
					addDelegate(instance, key, item);
					return;
				}

				MethodInfo mi = LookupAddDictionaryMethod(instanceType);
				if (mi == null)
					throw new InvalidOperationException($"The dictionary type '{instanceType}' does not have 'Add' method");
				if (mode.HasFlag(XamlInvokerOptions.DeferCompile))
				{
					cache[key] = addDelegate = (i, k, v) => mi.Invoke(i, new object[] { k, v });
					Task.Factory.StartNew(() => cache[key] = addDelegate = mi.BuildCall2Expression());
				}
				else
				{
					cache[key] = addDelegate = mi.BuildCall2Expression();
				}
				addDelegate(instance, key, item);
			}
			else
			{
				MethodInfo mi;
				if (TryGetCache(lookupKey, out mi))
				{
					mi.Invoke(instance, new object[] { key, item });
					return;
				}

				mi = LookupAddDictionaryMethod(instanceType);
				if (mi == null)
					throw new InvalidOperationException($"The dictionary type '{instanceType}' does not have 'Add' method");
				cache[lookupKey] = mi;
				mi.Invoke(instance, new object[] { key, item });
			}
		}

		MethodInfo LookupAddDictionaryMethod(Type dictionaryType)
		{
			MethodInfo mi;
			if (dictionaryType.GetTypeInfo().IsGenericType)
			{
				mi = dictionaryType.GetRuntimeMethod("Add", dictionaryType.GetTypeInfo().GetGenericArguments());
				if (mi == null)
					mi = LookupAddMethod(dictionaryType, typeof(IDictionary<,>).MakeGenericType(dictionaryType.GetTypeInfo().GetGenericArguments()));
			}
			else
			{
				mi = dictionaryType.GetRuntimeMethod("Add", new Type[] { typeof(object), typeof(object) });
				if (mi == null)
					mi = LookupAddMethod(dictionaryType, typeof(IDictionary));
			}
			return mi;
		}

		MethodInfo LookupAddMethod(Type ct, Type iface)
		{
			var map = ct.GetTypeInfo().GetRuntimeInterfaceMap(iface);
			for (int i = 0; i < map.TargetMethods.Length; i++)
				if (map.InterfaceMethods[i].Name == "Add")
					return map.TargetMethods[i];
			return null;
		}

		public virtual object CreateInstance(object[] arguments)
		{
			ThrowIfUnknown();
			if (arguments == null)
				return Activator.CreateInstance(Type.UnderlyingType);
			else
				return Activator.CreateInstance(Type.UnderlyingType, arguments);
		}

		[EnhancedXaml]
		public virtual object ToMutable(object instance)
		{
			if (!Type.IsImmutableCollection)
				return instance;

			Type mutableType;
			// Use a List<> or Dictionary<,> to collect values for immutable collections
			if (!TryGetCache<Type>(s_MutableTypeKey, out mutableType))
			{
				var typeArgs = Type.UnderlyingType.GetTypeInfo().GetGenericArguments();
				var listType = typeArgs.Length == 2 ? typeof(Dictionary<,>) : typeof(List<>);
				mutableType = listType.MakeGenericType(typeArgs);
				cache[s_MutableTypeKey] = mutableType;
			}
			if (instance == null || Type.UnderlyingType.GetTypeInfo().IsValueType)
				return Activator.CreateInstance(mutableType);

			return Activator.CreateInstance(mutableType, instance);
		}

		[EnhancedXaml]
		public virtual object ToImmutable(object instance)
		{
			if (!Type.IsImmutableCollection)
				return instance;

			MethodInfo createImmutableFromMutable;
			// create immutable collection from List<> or Dictionary<,> using the Immutable[Type].CreateRange static method
			if (TryGetCache(s_CreateImmutableFromMutableKey, out createImmutableFromMutable))
				return createImmutableFromMutable.Invoke(null, new[] { instance });

			var ti = Type.UnderlyingType.GetTypeInfo();
			var typeArgs = ti.GetGenericArguments();
			var assembly = ti.Assembly;
			var name = ti.GetGenericTypeDefinition().FullName;
			var builderType = assembly.GetType(name.Substring(0, name.Length - 2)); // remove `1 or `2
			var mi = builderType.GetRuntimeMethods().FirstOrDefault(r => r.Name == "CreateRange" && r.GetParameters().Length == 1);
			createImmutableFromMutable = mi.MakeGenericMethod(typeArgs);
			cache[s_CreateImmutableFromMutableKey] = createImmutableFromMutable;
			return createImmutableFromMutable.Invoke(null, new[] { instance });
		}

		public virtual MethodInfo GetAddMethod(XamlType contentType)
		{
			return Type == null || Type.UnderlyingType == null || Type.ItemType == null || Type.CollectionKind == XamlCollectionKind.None
				? null
				: Type.UnderlyingType.GetRuntimeMethod("Add", new Type[] { contentType.UnderlyingType });
		}

		public virtual MethodInfo GetEnumeratorMethod()
		{
			return Type == null || Type.UnderlyingType == null || Type.CollectionKind == XamlCollectionKind.None
				? null
				: Type.UnderlyingType.GetRuntimeMethod("GetEnumerator", new Type[0]);
		}

		public virtual IEnumerator GetItems(object instance)
		{
			if (instance == null)
				throw new ArgumentNullException(nameof(instance));

			// cannot get enumerator of immutable collections
			if (Type?.IsMutableDefault(instance) == true)
				return Enumerable.Empty<object>().GetEnumerator();

			return ((IEnumerable)instance).GetEnumerator();
		}
	}
}
