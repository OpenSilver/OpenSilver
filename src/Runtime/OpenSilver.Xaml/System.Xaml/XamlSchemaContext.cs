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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Markup;
using System.Xaml.ComponentModel;
using System.Xaml.Markup;
using System.Xaml.Schema;
using Pair = System.Collections.Generic.KeyValuePair<string, string>;

namespace System.Xaml
{
    internal class XamlSchemaContext
	{
		public XamlSchemaContext()
			: this(null, null)
		{
		}

		public XamlSchemaContext(IEnumerable<Assembly> referenceAssemblies)
			: this(referenceAssemblies, null)
		{
		}

		public XamlSchemaContext(XamlSchemaContextSettings settings)
			: this(null, settings)
		{
		}

		public XamlSchemaContext(IEnumerable<Assembly> referenceAssemblies, XamlSchemaContextSettings settings)
		{
			if (referenceAssemblies != null)
				reference_assemblies = new List<Assembly>(referenceAssemblies);
			/*else
				AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoaded;*/

			if (settings == null)
				return;

			FullyQualifyAssemblyNamesInClrNamespaces = settings.FullyQualifyAssemblyNamesInClrNamespaces;
			SupportMarkupExtensionsWithDuplicateArity = settings.SupportMarkupExtensionsWithDuplicateArity;
			InvokerOptions = settings.InvokerOptions;
		}

		~XamlSchemaContext()
		{
			/*if (reference_assemblies == null)
				AppDomain.CurrentDomain.AssemblyLoad -= OnAssemblyLoaded;*/
		}

		IList<Assembly> reference_assemblies;
		Dictionary<string, List<string>> xaml_nss;
		Dictionary<string, string> prefixes;
		Dictionary<string, string> compat_nss;
		Dictionary<string, List<XamlType>> all_xaml_types;
		XamlType[] empty_xaml_types = new XamlType[0];
		Dictionary<Type, XamlType> run_time_types = new Dictionary<Type, XamlType>();
		Dictionary<Tuple<string, string>, XamlType> type_lookup = new Dictionary<Tuple<string, string>, XamlType>();
		Dictionary<Pair, XamlDirective> xaml_directives = new Dictionary<Pair, XamlDirective>();
		Dictionary<object, XamlMember> member_cache = new Dictionary<object, XamlMember>();
		Dictionary<ParameterInfo, XamlMember> parameter_cache = new Dictionary<ParameterInfo, XamlMember>();
		Dictionary<string, AssemblyInfo> assembly_cache = new Dictionary<string, AssemblyInfo>();

		[EnhancedXaml]
		public XamlInvokerOptions InvokerOptions { get; private set; } = XamlInvokerOptions.DeferCompile;

		public bool SupportMarkupExtensionsWithDuplicateArity { get; private set; }

		public bool FullyQualifyAssemblyNamesInClrNamespaces { get; private set; }

		public IList<Assembly> ReferenceAssemblies => reference_assemblies;

		class AssemblyInfo
		{
			AssemblyName _name;
			public AssemblyName Name => _name ?? (_name = Assembly.GetName());
			public Assembly Assembly;

			public IEnumerable<T> TryGetAttributes<T>()
				where T : Attribute
			{
				try
				{
					return Assembly.GetCustomAttributes<T>();
				}
				catch (Exception ex)
				{
					Debug.WriteLine("Error getting attributes for assembly '{0}': {1}", Name, ex);
					return Enumerable.Empty<T>();
				}
			}
		}

		IList<AssemblyInfo> assembliesInScope;

		IEnumerable<AssemblyInfo> AssembliesInScope
		{
			get
			{
				if (assembliesInScope != null)
					return assembliesInScope;
				var assemblies = reference_assemblies ?? LookupAssembliesInScope();

				assembliesInScope = assemblies.Select(r => new AssemblyInfo { Assembly = r }).ToList();
				return assembliesInScope;
			}
		}

		static List<Assembly> cachedAssembliesInScope;
		static IEnumerable<Assembly> LookupAssembliesInScope()
		{
			if (cachedAssembliesInScope != null)
				return cachedAssembliesInScope;

			var assemblies =
				GetAppDomainAssemblies()
				?? GetReferencedAssemblies()
				?? GetUwpAssemblies()
				?? Enumerable.Empty<Assembly>();

			cachedAssembliesInScope = assemblies.Distinct().ToList();
			return cachedAssembliesInScope;
		}

		static IEnumerable<Assembly> GetAppDomainAssemblies()
		{
			return AppDomain.CurrentDomain.GetAssemblies();
		}

		static IEnumerable<Assembly> GetReferencedAssemblies()
		{
			try
			{
				// .NET Core, we get the assemblies from the entry assembly.
				var assemblyType = typeof(Assembly);
				if (assemblyType == null)
					return null;
				var getEntryAssembly = assemblyType.GetRuntimeMethod("GetEntryAssembly", new Type[0]);
				if (getEntryAssembly == null)
					return null;
				var entryAssembly = getEntryAssembly.Invoke(null, null) as Assembly;
				if (entryAssembly == null)
					return null;

				var assemblies = new List<Assembly>();
				assemblies.Add(entryAssembly);

				var getReferencedAssemblies = assemblyType.GetRuntimeMethod("GetReferencedAssemblies", new Type[0]);
				if (getReferencedAssemblies != null)
				{
					var referencedAssemblies = getReferencedAssemblies.Invoke(entryAssembly, null) as AssemblyName[];
					if (referencedAssemblies != null)
					{
						foreach (var assemblyName in referencedAssemblies)
						{
							try
							{
								assemblies.Add(Assembly.Load(assemblyName));
							}
							catch { }
						}
					}
				}
				else
				{
					foreach (var file in Directory.EnumerateFiles(AppContext.BaseDirectory, "*.dll"))
					{
						try
						{
							var assembly = Assembly.Load(new AssemblyName(Path.GetFileNameWithoutExtension(file)));
							if (assembly != entryAssembly)
								assemblies.Add(assembly);
						}
						catch { }
					}
				}

				return assemblies;
			}
			catch
			{
				return null;
			}
		}

		static IEnumerable<Assembly> GetStandardAssemblies()
		{
			yield return typeof(int).GetTypeInfo().Assembly; // System.Private.CoreLib
		}

		static IEnumerable<Assembly> GetUwpAssemblies()
		{
			try
			{
				// if we're running in UWP, get all assemblies in installed locationF
				// an ugly hack, but there's no other option until maybe netstandard 2.0.
				var packageType = Type.GetType("Windows.ApplicationModel.Package,Windows.Foundation.UniversalApiContract,ContentType=WindowsRuntime");
				if (packageType == null)
					return null;
				var current = packageType.GetRuntimeProperty("Current")?.GetValue(null);
				if (current == null)
					return null;
				var installedLocation = current.GetType().GetRuntimeProperty("InstalledLocation")?.GetValue(current);
				if (installedLocation == null)
					return null;
				var getFilesAsync = installedLocation.GetType().GetRuntimeMethod("GetFilesAsync", new Type[0])?.Invoke(installedLocation, null);
				if (getFilesAsync == null)
					return null;

				var awaiterExtensions = Type.GetType("System.WindowsRuntimeSystemExtensions,System.Runtime.WindowsRuntime");
				var interfaceType = Type.GetType("Windows.Foundation.IAsyncOperation`1,Windows.Foundation.UniversalApiContract,ContentType=WindowsRuntime");
				var storageType = Type.GetType("Windows.Storage.StorageFile,Windows.Foundation.UniversalApiContract,ContentType=WindowsRuntime");
				var resultType = typeof(IReadOnlyList<>).MakeGenericType(storageType);
				var interfaceResultType = interfaceType?.MakeGenericType(resultType);
				var getAwaiterMethod = awaiterExtensions.GetRuntimeMethods().First(m => m.Name == "GetAwaiter" && m.IsGenericMethod && m.ReturnType.GetTypeInfo().IsGenericType);
				var awaiter = getAwaiterMethod.MakeGenericMethod(resultType).Invoke(null, new object[] { getFilesAsync });
				var results = awaiter?.GetType().GetRuntimeMethod("GetResult", new Type[0])?.Invoke(awaiter, null);
				var nameProperty = storageType.GetRuntimeProperty("Name");

				var assemblies = new List<Assembly>();
				foreach (var result in (results as IEnumerable))
				{
					var name = (string)nameProperty.GetValue(result);
					if (string.Equals(Path.GetExtension(name), ".dll", StringComparison.OrdinalIgnoreCase)
						|| string.Equals(Path.GetExtension(name), ".exe", StringComparison.OrdinalIgnoreCase))
					{
						try
						{
							assemblies.Add(Assembly.Load(new AssemblyName(Path.GetFileNameWithoutExtension(name))));
						}
						catch { }
					}
				}
				return GetStandardAssemblies().Concat(assemblies);
			}
			catch { }
			return null;
		}

		internal string GetXamlNamespace(string clrNamespace)
		{
			if (clrNamespace == null) // could happen on nested generic type (see bug #680385-comment#4). Not sure if null is correct though.
				return null;
			if (xaml_nss == null) // fill it first
				FillAllXamlNamespaces();
			List<string> ret;
			return xaml_nss.TryGetValue(clrNamespace, out ret) ? ret.FirstOrDefault() : null;
		}

		public virtual IEnumerable<string> GetAllXamlNamespaces()
		{
			if (xaml_nss == null)
				FillAllXamlNamespaces();
			return xaml_nss.Values.SelectMany(r => r).Distinct();
		}

		void FillAllXamlNamespaces()
		{
			xaml_nss = new Dictionary<string, List<string>>();
			foreach (var ass in AssembliesInScope)
				FillXamlNamespaces(ass);
		}

		public virtual ICollection<XamlType> GetAllXamlTypes(string xamlNamespace)
		{
			if (xamlNamespace == null)
				throw new ArgumentNullException("xamlNamespace");
			if (all_xaml_types == null)
			{
				var types = new Dictionary<string, List<XamlType>>();
				foreach (var ass in AssembliesInScope)
					FillAllXamlTypes(types, ass);
				all_xaml_types = types;
			}

			List<XamlType> l;
			if (all_xaml_types.TryGetValue(xamlNamespace, out l))
				return l;
			else
				return empty_xaml_types;
		}

		public virtual string GetPreferredPrefix(string xmlns)
		{
			if (xmlns == null)
				throw new ArgumentNullException("xmlns");
			if (xmlns == XamlLanguage.Xaml2006Namespace)
				return "x";
			if (prefixes == null)
			{
				prefixes = new Dictionary<string, string>();
				foreach (var ass in AssembliesInScope)
					FillPrefixes(ass);
			}
			string ret;
			return prefixes.TryGetValue(xmlns, out ret) ? ret : "p"; // default
		}

		protected internal XamlValueConverter<TConverterBase> GetValueConverter<TConverterBase>(Type converterType, XamlType targetType)
			where TConverterBase : class
		{
			return new XamlValueConverter<TConverterBase>(converterType, targetType);
		}

		public virtual XamlDirective GetXamlDirective(string xamlNamespace, string name)
		{
			XamlDirective t;
			var p = new Pair(xamlNamespace, name);
			if (!xaml_directives.TryGetValue(p, out t))
			{
				t = new XamlDirective(xamlNamespace, name);
				xaml_directives.Add(p, t);
			}
			return t;
		}

		Dictionary<string, XamlType> typename_lookup = new Dictionary<string, XamlType>();

		internal XamlType GetXamlType(string fullTypeName)
		{
			if (typename_lookup.TryGetValue(fullTypeName, out XamlType xamlType))
				return xamlType;
			var idx = fullTypeName.IndexOf(',');
			if (idx == -1)
				return null;
			var name = fullTypeName.Substring(0, idx).Trim();
			var assemblyName = fullTypeName.Substring(idx + 1).Trim();
			var assembly = OnAssemblyResolve(assemblyName);
			if (assembly == null)
				return null;

			var type = assembly.GetType(name);
			if (type == null)
				return null;
			xamlType = GetXamlType(type);
			if (ReferenceEquals(xamlType, null) || xamlType.IsUnknown)
				return null;

			if (!ReferenceEquals(xamlType, null))
				typename_lookup[fullTypeName] = xamlType;
			return xamlType;
		}

		public virtual XamlType GetXamlType(Type type)
		{
			XamlType xt;
			if (run_time_types.TryGetValue(type, out xt))
				return xt;

			xt = new XamlType(type, this);

			run_time_types[type] = xt;
			return xt;
		}

		public XamlType GetXamlType(XamlTypeName xamlTypeName)
		{
			if (xamlTypeName == null)
				throw new ArgumentNullException(nameof(xamlTypeName));

			var n = xamlTypeName;
			if (n.TypeArguments.Count == 0) // non-generic
				return GetXamlType(n.Namespace, n.Name, null);

			// generic
			XamlType[] typeArgs = new XamlType[n.TypeArguments.Count];
			for (int i = 0; i < typeArgs.Length; i++)
				typeArgs[i] = GetXamlType(n.TypeArguments[i]);
			return GetXamlType(n.Namespace, n.Name, typeArgs);
		}

		protected virtual XamlType GetXamlType(string xamlNamespace, string name, params XamlType[] typeArguments)
		{
			XamlType ret;
			var key = Tuple.Create(xamlNamespace, name);
			var useLookup = typeArguments == null || typeArguments.Length == 0;
			if (useLookup && type_lookup.TryGetValue(key, out ret))
				return ret;

			string dummy;
			if (TryGetCompatibleXamlNamespace(xamlNamespace, out dummy))
				xamlNamespace = dummy;

			ret = ResolveXamlTypeName(xamlNamespace, name, typeArguments, false);

			if (useLookup)
				type_lookup[key] = ret;

			// If the type was not found, it just returns null.
			return ret;
		}

		bool TypeMatches(XamlType t, string ns, string name, XamlType[] typeArgs)
		{
			if (t.PreferredXamlNamespace == ns && t.Name == name && t.TypeArguments.ListEquals(typeArgs))
				return true;
			if (t.IsMarkupExtension)
				return t.PreferredXamlNamespace == ns && t.InternalXmlName == name && t.TypeArguments.ListEquals(typeArgs);
			else
				return false;
		}

		protected internal virtual Assembly OnAssemblyResolve(string assemblyName)
		{
			if (assembly_cache.TryGetValue(assemblyName, out AssemblyInfo info))
				return info.Assembly;

			var aname = new AssemblyName(assemblyName);
			foreach (var ainfo in AssembliesInScope)
			{
				if (ainfo.Name.Matches(aname))
				{
					assembly_cache[assemblyName] = ainfo;
					return ainfo.Assembly;
				}
			}

			// fallback if not found
			var assembly = Assembly.Load(aname);
			assembly_cache[assemblyName] = new AssemblyInfo { Assembly = assembly };
			return assembly;
		}

		public virtual bool TryGetCompatibleXamlNamespace(string xamlNamespace, out string compatibleNamespace)
		{
			if (xamlNamespace == null)
				throw new ArgumentNullException("xamlNamespace");
			if (compat_nss == null)
			{
				compat_nss = new Dictionary<string, string>();
				foreach (var ass in AssembliesInScope)
					FillCompatibilities(ass);
			}
			if (compat_nss.TryGetValue(xamlNamespace, out compatibleNamespace))
				return GetAllXamlNamespaces().Contains(compatibleNamespace);
			if (GetAllXamlNamespaces().Contains(xamlNamespace))
			{
				compatibleNamespace = xamlNamespace;
				return true;
			}
			return false;
		}

		// cache updater methods
		void FillXamlNamespaces(AssemblyInfo ass)
		{
			try
			{
				foreach (XmlnsDefinitionAttribute xda in ass.TryGetAttributes<XmlnsDefinitionAttribute>())
				{
					List<string> namespaces;
					if (!xaml_nss.TryGetValue(xda.ClrNamespace, out namespaces))
					{
						namespaces = new List<string>();
						xaml_nss.Add(xda.ClrNamespace, namespaces);
					}
					namespaces.Add(xda.XmlNamespace);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Error getting namespaces for assembly '{0}': {1}", ass.Name, ex);
			}
		}

		void FillPrefixes(AssemblyInfo ass)
		{
			foreach (XmlnsPrefixAttribute xpa in ass.TryGetAttributes<XmlnsPrefixAttribute>())
				prefixes.Add(xpa.XmlNamespace, xpa.Prefix);
		}

		void FillCompatibilities(AssemblyInfo ass)
		{
			foreach (XmlnsCompatibleWithAttribute xca in ass.TryGetAttributes<XmlnsCompatibleWithAttribute>())
				compat_nss.Add(xca.OldNamespace, xca.NewNamespace);
		}

		Type FindType(string xamlNamespace, string name, Type[] genArgs)
		{
			if (genArgs != null)
				name += "`" + genArgs.Length;
			foreach (var ass in AssembliesInScope)
				foreach (XmlnsDefinitionAttribute xda in ass.TryGetAttributes<XmlnsDefinitionAttribute>())
				{
					if (xamlNamespace != xda.XmlNamespace)
						continue;

					var assembly = ass.Assembly;
					if (!string.IsNullOrEmpty(xda.AssemblyName))
						assembly = Assembly.Load(new AssemblyName(xda.AssemblyName));
					var n = xda.ClrNamespace + "." + name;
					var t = assembly.GetType(n);
					if (t == null && genArgs == null)
					{
						t = assembly.GetType(n + "Extension");
						if (t != null && !GetXamlType(t).IsMarkupExtension)
							continue;
					}
					if (t != null && t.Namespace == xda.ClrNamespace)
					{
						var ti = t.GetTypeInfo();
						if (!ti.IsNested)
						{
							return t;
						}
					}
				}
			return null;
		}

		void FillAllXamlTypes(Dictionary<string, List<XamlType>> types, AssemblyInfo ass)
		{
			try
			{
				foreach (XmlnsDefinitionAttribute xda in ass.TryGetAttributes<XmlnsDefinitionAttribute>())
				{
					List<XamlType> l;
					if (!types.TryGetValue(xda.XmlNamespace, out l))
					{
						l = new List<XamlType>();
						types.Add(xda.XmlNamespace, l);
					}
					var assembly = ass.Assembly;
					if (!string.IsNullOrEmpty(xda.AssemblyName))
						assembly = Assembly.Load(new AssemblyName(xda.AssemblyName));
					foreach (var t in assembly.GetExportedTypes())
						if (t.Namespace == xda.ClrNamespace && !t.GetTypeInfo().IsNested)
							l.Add(GetXamlType(t));
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Error getting xaml types for assembly '{0}': {1}", ass.Name, ex);
			}
		}

		// XamlTypeName -> Type resolution

		const string clr_ns = "clr-namespace:";
		static readonly int clr_ns_len = clr_ns.Length;
		static readonly int clr_ass_len = "assembly=".Length;

		XamlType ResolveXamlTypeName(string xmlNamespace, string xmlLocalName, XamlType[] typeArguments, bool required)
		{
			if (xmlNamespace == XamlLanguage.Xaml2006Namespace)
			{
				var xt = XamlLanguage.SpecialNames.Find(xmlLocalName, xmlNamespace);
				if (ReferenceEquals(xt, null))
					xt = XamlLanguage.AllTypes.FirstOrDefault(t => TypeMatches(t, xmlNamespace, xmlLocalName, typeArguments));
				if (!ReferenceEquals(xt, null))
					return xt;
			}

			Type[] genArgs = null;
			if (typeArguments != null && typeArguments.Length > 0)
			{
				genArgs = typeArguments.Select(t => t?.UnderlyingType).ToArray();
				if (genArgs.Any(t => t == null))
					return null;
			}

			Type ret;
			if (!xmlNamespace.StartsWith(clr_ns, StringComparison.Ordinal))
			{
				ret = FindType(xmlNamespace, xmlLocalName, genArgs);
				if (ret == null)
					return null;
			}
			else
			{
				// convert xml namespace to clr namespace and assembly
				string[] split = xmlNamespace.Split(';');
				if (split.Length != 2 || split[0].Length < clr_ns_len || split[1].Length <= clr_ass_len)
					throw new XamlParseException(string.Format("Cannot resolve runtime namespace from XML namespace '{0}'", xmlNamespace));
				string tns = split[0].Substring(clr_ns_len);
				string aname = split[1].Substring(clr_ass_len);

				string taqn = GetTypeName(tns, xmlLocalName, genArgs);
				var ass = OnAssemblyResolve(aname);
				// MarkupExtension type could omit "Extension" part in XML name.
				ret = ass?.GetType(taqn) ?? ass?.GetType(taqn + "Extension");

				if (ret == null && aname == "mscorlib")
				{
					//foreach (var asmName in XamlType.mscorlib_assemblies)
					{
						ass = typeof(int).GetTypeInfo().Assembly;
						//.NET Core hack to get type from correct assembly
						ret = ass?.GetType(taqn) ?? ass?.GetType(taqn + "Extension");
						////if (ret != null)
						//break;
					}
				}
				if (required && ret == null)
					throw new XamlParseException(string.Format("Cannot resolve runtime type from XML namespace '{0}', local name '{1}' with {2} type arguments ({3})", xmlNamespace, xmlLocalName, typeArguments != null ? typeArguments.Length : 0, taqn));
			}


			// ensure only the referenced types are allowed
			if ((reference_assemblies != null && !reference_assemblies.Contains(ret.GetTypeInfo().Assembly)))
				return null;

			// we need to return the unknown type instead of null for more readable errors
			if (ret == null)
				return new XamlType(xmlNamespace, xmlLocalName, null, this);

			return GetXamlType(genArgs == null ? ret : ret.MakeGenericType(genArgs));
		}

		static string GetTypeName(string tns, string name, Type[] genArgs)
		{
			string tfn = tns.Length > 0 ? tns + '.' + name : name;
			if (genArgs != null)
				tfn += "`" + genArgs.Length;
			return tfn;
		}

        [EnhancedXaml]
        protected internal virtual XamlMember GetParameter(ParameterInfo parameterInfo)
        {
            XamlMember member;
            if (parameter_cache.TryGetValue(parameterInfo, out member))
                return member;
            return parameter_cache[parameterInfo] = new XamlMember(parameterInfo, this);
        }

        [EnhancedXaml]
        protected internal virtual XamlMember GetProperty(PropertyInfo propertyInfo)
        {
            var key = Tuple.Create(propertyInfo.DeclaringType, propertyInfo.Name);
            XamlMember member;
            if (member_cache.TryGetValue(key, out member))
                return member;
            return member_cache[key] = new XamlMember(propertyInfo, this);
        }

        [EnhancedXaml]
        protected internal virtual XamlMember GetEvent(EventInfo eventInfo)
        {
            var key = Tuple.Create(eventInfo.DeclaringType, eventInfo.Name);
            XamlMember member;
            if (member_cache.TryGetValue(key, out member))
                return member;
            return member_cache[key] = new XamlMember(eventInfo, this);
        }

        [EnhancedXaml]
        protected internal virtual XamlMember GetAttachableProperty(string attachablePropertyName, MethodInfo getter, MethodInfo setter)
        {
            var key = Tuple.Create(getter?.DeclaringType ?? setter.DeclaringType, getter?.Name, setter?.Name);
            XamlMember member;
            if (member_cache.TryGetValue(key, out member))
                return member;
            return member_cache[key] = new XamlMember(attachablePropertyName, getter, setter, this);
        }

        [EnhancedXaml]
        protected internal virtual XamlMember GetAttachableEvent(string attachablePropertyName, MethodInfo adder)
        {
            var key = Tuple.Create(adder.DeclaringType, adder.Name);
            XamlMember member;
            if (member_cache.TryGetValue(key, out member))
                return member;
            return member_cache[key] = new XamlMember(attachablePropertyName, adder, this);
        }

        [EnhancedXaml]
        internal virtual ICustomAttributeProvider GetCustomAttributeProvider(Type type)
        {
            return new TypeAttributeProvider(type);
        }

        [EnhancedXaml]
        internal virtual ICustomAttributeProvider GetCustomAttributeProvider(MemberInfo member)
        {
            return new MemberAttributeProvider(member);
        }
    }
}
