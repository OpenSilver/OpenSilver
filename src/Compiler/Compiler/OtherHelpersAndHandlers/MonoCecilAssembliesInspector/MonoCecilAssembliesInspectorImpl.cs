
/*===================================================================================
*
*   Copyright (c) Userware/OpenSilver.net
*
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*
\*====================================================================================*/

using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace OpenSilver.Compiler
{
    internal class MonoCecilAssembliesInspectorImpl : IDisposable
    {
        private sealed class AssemblyData
        {
            private const string XmlnsDefinitionAttributeFullName = "System.Windows.Markup.XmlnsDefinitionAttribute";

            private readonly Dictionary<string, List<string>> _xmlToClrNamespaces;

            public AssemblyData(AssemblyDefinition assembly)
            {
                Assembly = assembly;
                _xmlToClrNamespaces = ReadXmlnsDefinitionAttributes(assembly);
            }

            public AssemblyDefinition Assembly { get; }

            public IEnumerable<string> GetClrNamespacesFromXmlNamespace(string xmlNamespace)
            {
                if (_xmlToClrNamespaces.TryGetValue(xmlNamespace, out List<string> clrNamespaces))
                {
                    return clrNamespaces;
                }

                return Enumerable.Empty<string>();
            }

            private Dictionary<string, List<string>> ReadXmlnsDefinitionAttributes(AssemblyDefinition assembly)
            {
                var map = new Dictionary<string, List<string>>();

                foreach (CustomAttribute attribute in assembly.CustomAttributes)
                {
                    if (!IsXmlnsDefinitionAttribute(attribute))
                    {
                        continue;
                    }

                    string xmlNamespace = (attribute.ConstructorArguments[0].Value ?? string.Empty).ToString();
                    string clrNamespace = (attribute.ConstructorArguments[1].Value ?? string.Empty).ToString();

                    if (string.IsNullOrEmpty(xmlNamespace) || string.IsNullOrEmpty(clrNamespace))
                    {
                        continue;
                    }

                    if (!map.TryGetValue(xmlNamespace, out List<string> clrNamespaces))
                    {
                        map[xmlNamespace] = clrNamespaces = new List<string>();
                    }

                    if (!clrNamespaces.Contains(clrNamespace))
                    {
                        clrNamespaces.Add(clrNamespace);
                    }
                }

                return map;
            }

            private static bool IsXmlnsDefinitionAttribute(CustomAttribute attribute) =>
                attribute.AttributeType.FullName == XmlnsDefinitionAttributeFullName;
        }

        private sealed class ConcurrentHashSet<T> : IEnumerable<T>
        {
            private readonly ConcurrentDictionary<T, byte> _table;

            public ConcurrentHashSet()
            {
                _table = [];
            }

            public bool TryAdd(T value) => _table.TryAdd(value, 0);

            public IEnumerator<T> GetEnumerator() => _table.Keys.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private const string TypeConverterAttributeFullName = "System.ComponentModel.TypeConverterAttribute";
        private const string ContentPropertyAttributeFullName = "System.Windows.Markup.ContentPropertyAttribute";

        private readonly MonoCecilAssemblyStorage _storage;
        private readonly Dictionary<AssemblyDefinition, AssemblyData> _assemblies = [];
        private readonly ConcurrentDictionary<TypeKey, TypeDefinition> _typeNameToType = [];
        private readonly Dictionary<AssemblyDefinition, ConcurrentHashSet<TypeKey>> _typesPerAssembly = [];

        // All per-compile memorization caches live in a dedicated helper; UnloadAssembly resets
        // them in one call. See MonoCecilLookupCache for the cache layout and rationale.
        private readonly MonoCecilLookupCache _caches = new();

        // Method-group delegates cached once so the per-call cache wrappers don't allocate a
        // fresh delegate on every invocation. The static Find*Deep helpers stay on this class;
        // only the memorization moved out.
        private static readonly FindMemberDeepFunc<PropertyDefinition> s_findPropertyDeep = FindPropertyDeep;
        private static readonly FindMemberDeepFunc<FieldDefinition> s_findFieldDeep = FindFieldDeep;
        private static readonly FindMemberDeepFunc<EventDefinition> s_findEventDeep = FindEventDeep;
        private static readonly FindMemberDeepFunc<MethodDefinition> s_findMethodDeep = FindMethodDeep;
        private static readonly Func<TypeDefinition, TypeDefinition, bool> s_isSubclassOf = TypeDefinitionExtensions.IsSubclassOf;
        private static readonly Func<TypeDefinition, TypeDefinition, bool> s_doesAnySubTypeImplementInterface = TypeDefinitionExtensions.DoesAnySubTypeImplementInterface;

        // _typeReferenceHelper.GetEnumValue captured once as a delegate so the enum-value cache
        // path doesn't allocate a closure on every call.
        private readonly Func<TypeDefinition, string, bool, bool, string> _getEnumValueFromHelper;

        private readonly TypeReferenceHelper _typeReferenceHelper;

        private TypeDefinition _iListType;
        private TypeDefinition _iDictionaryType;

        private TypeDefinition _dependencyObjectType;
        private TypeDefinition _styleType;
        private TypeDefinition _frameworkTemplateType;
        private TypeDefinition _controlTemplateType;
        private TypeDefinition _dataTemplateType;
        private TypeDefinition _applicationType;
        private TypeDefinition _resourceDictionaryType;
        private TypeDefinition _contentPresenterType;
        private TypeDefinition _contentControlType;
        private TypeDefinition _relativeSourceType;
        private TypeDefinition _bindingBaseType;
        private TypeDefinition _bindingType;
        private TypeDefinition _multiBindingType;
        private TypeDefinition _templateBindingExtensionType;
        private TypeDefinition _nullExtensionType;
        private TypeDefinition _staticExtensionType;
        private TypeDefinition _typeExtensionType;
        private TypeDefinition _staticResourceExtensionType;
        private TypeDefinition _themeResourceExtensionType;
        private TypeDefinition _dynamicResourceExtensionType;
        private TypeDefinition _responsiveExtensionType;
        private TypeDefinition _iUIElementType;
        private TypeDefinition _iFrameworkElementType;
        private TypeDefinition _iMarkupExtensionType;

        private TypeDefinition IListType =>
            _iListType ??= GetKnownType(typeof(IList).Namespace, nameof(IList), null);

        private TypeDefinition IDictionaryType =>
            _iDictionaryType ??= GetKnownType(typeof(IDictionary).Namespace, nameof(IDictionary), null);

        private TypeDefinition DependencyObjectType =>
            _dependencyObjectType ??= GetKnownType(KnownNamespaces.SystemWindows, "DependencyObject", Constants.OPENSILVER_ASSEMBLY_NAME);

        private TypeDefinition ApplicationType =>
            _applicationType ??= GetKnownType(KnownNamespaces.SystemWindows, "Application", Constants.OPENSILVER_ASSEMBLY_NAME);

        private TypeDefinition ResourceDictionaryType =>
            _resourceDictionaryType ??= GetKnownType(KnownNamespaces.SystemWindows, "ResourceDictionary", Constants.OPENSILVER_ASSEMBLY_NAME);

        private TypeDefinition StyleType =>
            _styleType ??= GetKnownType(KnownNamespaces.SystemWindows, "Style", Constants.OPENSILVER_ASSEMBLY_NAME);

        private TypeDefinition FrameworkTemplateType =>
            _frameworkTemplateType ??= GetKnownType(KnownNamespaces.SystemWindows, "FrameworkTemplate", Constants.OPENSILVER_ASSEMBLY_NAME);

        private TypeDefinition DataTemplateType =>
            _dataTemplateType ??= GetKnownType(KnownNamespaces.SystemWindows, "DataTemplate", Constants.OPENSILVER_ASSEMBLY_NAME);

        private TypeDefinition ControlTemplateType =>
            _controlTemplateType ??= GetKnownType(KnownNamespaces.SystemWindowsControls, "ControlTemplate", Constants.OPENSILVER_ASSEMBLY_NAME);

        private TypeDefinition ContentPresenterType =>
            _contentPresenterType ??= GetKnownType(KnownNamespaces.SystemWindowsControls, "ContentPresenter", Constants.OPENSILVER_ASSEMBLY_NAME);

        private TypeDefinition ContentControlType =>
            _contentControlType ??= GetKnownType(KnownNamespaces.SystemWindowsControls, "ContentControl", Constants.OPENSILVER_ASSEMBLY_NAME);

        private TypeDefinition RelativeSourceType =>
            _relativeSourceType ??= GetKnownType(KnownNamespaces.SystemWindowsData, "RelativeSource", Constants.OPENSILVER_ASSEMBLY_NAME);

        private TypeDefinition BindingBaseType =>
            _bindingBaseType ??= GetKnownType(KnownNamespaces.SystemWindowsData, "BindingBase", Constants.OPENSILVER_ASSEMBLY_NAME);

        private TypeDefinition BindingType =>
            _bindingType ??= GetKnownType(KnownNamespaces.SystemWindowsData, "Binding", Constants.OPENSILVER_ASSEMBLY_NAME);

        private TypeDefinition MultiBindingType =>
            _multiBindingType ??= GetKnownType(KnownNamespaces.SystemWindowsData, "MultiBinding", Constants.OPENSILVER_ASSEMBLY_NAME);

        private TypeDefinition TemplateBindingExtensionType =>
            _templateBindingExtensionType ??= GetKnownType(KnownNamespaces.SystemWindows, "TemplateBindingExtension", Constants.OPENSILVER_ASSEMBLY_NAME);

        private TypeDefinition NullExtensionType =>
            _nullExtensionType ??= GetKnownType(KnownNamespaces.SystemWindowsMarkup, "NullExtension", Constants.OPENSILVER_ASSEMBLY_NAME);

        private TypeDefinition StaticExtensionType =>
            _staticExtensionType ??= GetKnownType(KnownNamespaces.SystemWindowsMarkup, "StaticExtension", Constants.OPENSILVER_ASSEMBLY_NAME);

        private TypeDefinition TypeExtensionType =>
            _typeExtensionType ??= GetKnownType(KnownNamespaces.SystemWindowsMarkup, "TypeExtension", Constants.OPENSILVER_ASSEMBLY_NAME);

        private TypeDefinition StaticResourceExtensionType =>
            _staticResourceExtensionType ??= GetKnownType(KnownNamespaces.SystemWindowsMarkup, "StaticResourceExtension", Constants.OPENSILVER_ASSEMBLY_NAME);

        private TypeDefinition ThemeResourceExtensionType =>
            _themeResourceExtensionType ??= GetKnownType(KnownNamespaces.SystemWindowsMarkup, "ThemeResourceExtension", Constants.OPENSILVER_ASSEMBLY_NAME);

        private TypeDefinition DynamicResourceExtensionType =>
            _dynamicResourceExtensionType ??= GetKnownType(KnownNamespaces.SystemWindows, "DynamicResourceExtension", Constants.OPENSILVER_ASSEMBLY_NAME);

        private TypeDefinition ResponsiveExtensionType =>
            _responsiveExtensionType ??= GetKnownType(KnownNamespaces.SystemWindows, "ResponsiveExtension", Constants.OPENSILVER_ASSEMBLY_NAME);

        private TypeDefinition IUIElementType =>
            _iUIElementType ??= GetKnownType(KnownNamespaces.SystemWindows, "IUIElement", Constants.OPENSILVER_ASSEMBLY_NAME);

        private TypeDefinition IFrameworkElementType =>
            _iFrameworkElementType ??= GetKnownType(KnownNamespaces.SystemWindows, "IFrameworkElement", Constants.OPENSILVER_ASSEMBLY_NAME);

        private TypeDefinition IMarkupExtensionType =>
            _iMarkupExtensionType ??= GetKnownType(KnownNamespaces.SystemXaml, "IMarkupExtension`1", Constants.OPENSILVER_ASSEMBLY_NAME);

        public MonoCecilAssembliesInspectorImpl(string assemblyName, SupportedLanguage compilerType)
        {
            _typeReferenceHelper = compilerType switch
            {
                SupportedLanguage.CSharp => TypeReferenceHelper.CSharp,
                SupportedLanguage.VBNet => TypeReferenceHelper.VisualBasic,
                SupportedLanguage.FSharp => TypeReferenceHelper.FSharp,
                _ => throw new InvalidCompilerTypeException(),
            };

            _getEnumValueFromHelper = _typeReferenceHelper.GetEnumValue;
            _storage = new MonoCecilAssemblyStorage();
        }

        private AssemblyDefinition StoreAssembly(AssemblyDefinition assembly)
        {
            _assemblies.Add(assembly, new AssemblyData(assembly));
            _typesPerAssembly.Add(assembly, new ConcurrentHashSet<TypeKey>());
            return assembly;
        }

        public AssemblyDefinition LoadAssembly(string assemblyPath)
        {
            return StoreAssembly(_storage.LoadAssembly(assemblyPath));
        }

        public AssemblyDefinition LoadAssembly(Stream stream)
        {
            return StoreAssembly(_storage.LoadAssembly(stream));
        }

        public void UnloadAssembly(AssemblyDefinition assemblyDefinition)
        {
            _storage.UnloadAssembly(assemblyDefinition);
            _assemblies.Remove(assemblyDefinition);
            foreach (var t in _typesPerAssembly[assemblyDefinition])
            {
                _typeNameToType.TryRemove(t, out _);
            }
            _typesPerAssembly.Remove(assemblyDefinition);

            // Per-compile caches key on TypeDefinition references that may belong to the
            // unloaded assembly. Reset them to avoid stale references and memory leaks; they
            // rebuild quickly during the next compile.
            _caches.Clear();
        }

        public void Dispose()
        {
            _storage.Dispose();
            _assemblies.Clear();
        }

        internal TypeDefinition GetKnownType(string namespaceName, string typeName, string assemblyName)
            => FindType(namespaceName, typeName, assemblyName, null);

        internal TypeDefinition FindType(string namespaceName, string typeName, string assemblyName, IXmlLineInfo lineInfo, bool throwIfNull = true)
        {
            var typeKey = new TypeKey(namespaceName, typeName);

            // Start by looking in the cache dictionary:
            if (_typeNameToType.TryGetValue(typeKey, out TypeDefinition type))
            {
                return type;
            }

            // Look for the type in all loaded assemblies:
            foreach (AssemblyData asmData in _assemblies.Values)
            {
                AssemblyDefinition assembly = asmData.Assembly;

                if (assemblyName != null && !string.Equals(assembly.Name.Name, assemblyName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                IEnumerable<string> namespacesToLookInto;

                // If the namespace is a XML namespace (eg. "{http://schemas.microsoft.com/winfx/2006/xaml/presentation}"),
                // we should iterate through all the corresponding CLR namespaces:
                if (IsNamespaceAnXmlNamespace(namespaceName))
                {
                    namespacesToLookInto = asmData.GetClrNamespacesFromXmlNamespace(namespaceName);
                }
                else
                {
                    namespacesToLookInto = [namespaceName];
                }

                // Search for the type:
                foreach (string ns in namespacesToLookInto)
                {
                    // First, try to get the type from the assembly
                    type = assembly.MainModule.GetType(ns, typeName);

                    // If type is not found, look for an exported type (TypeForwardedToAttribute)
                    if (type is null &&
                        assembly.MainModule.HasExportedTypes &&
                        assembly.MainModule.ExportedTypes.FirstOrDefault(x => x.Name == typeName && x.Namespace == ns) is ExportedType exportedType)
                    {
                        type = exportedType.Resolve();
                    }

                    // Finally, try to find a nested type
                    if (type is null &&
                        assembly.MainModule.GetType(ns) is TypeDefinition containerType)
                    {
                        type = containerType.NestedTypes.FirstOrDefault(x => x.Name == typeName);
                    }

                    if (type != null)
                    {
                        _typeNameToType[typeKey] = type;
                        _typesPerAssembly[assembly].TryAdd(typeKey);
                        return type;
                    }
                }
            }

            if (throwIfNull)
            {
                throw GetTypeNotFoundError(namespaceName, typeName, assemblyName, lineInfo);
            }

            return null;
        }

        internal static XamlParseException GetTypeNotFoundError(string namespaceName, string typeName, string assemblyName, IXmlLineInfo lineInfo)
        {
            if (IsNamespaceAnXmlNamespace(namespaceName))
            {
                return new XamlParseException($"The type '{typeName}' does not exist in XML namespace '{namespaceName}'.", lineInfo);
            }

            if (string.IsNullOrEmpty(assemblyName))
            {
                if (string.IsNullOrEmpty(namespaceName))
                {
                    return new XamlParseException($"The type '{typeName}' does not exist.", lineInfo);
                }
                else
                {
                    return new XamlParseException($"The type '{typeName}' does not exist in CLR namespace '{namespaceName}'.", lineInfo);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(namespaceName))
                {
                    return new XamlParseException($"The type '{typeName}' does not exist in assembly '{assemblyName}'.", lineInfo);
                }
                else
                {
                    return new XamlParseException($"The type '{typeName}' does not exist in CLR namespace '{namespaceName}' in assembly '{assemblyName}'.", lineInfo);
                }
            }
        }

        private static bool IsNamespaceAnXmlNamespace(string namespaceName)
        {
            return namespaceName.StartsWith("http://"); //todo: are there other conditions possible for XML namespaces declared with xmlnsDefinitionAttribute?
        }

        internal static PropertyDefinition FindPropertyDeep(
            TypeDefinition elementType,
            string propertyName,
            MemberFlags flags,
            out TypeReference ownerElementType)
        {
            bool ignoreCaseFlag = TestMemberFlag(flags, MemberFlags.IgnoreCase);
            bool staticFlag = TestMemberFlag(flags, MemberFlags.Static);
            bool instanceFlag = TestMemberFlag(flags, MemberFlags.Instance);
            bool publicFlag = TestMemberFlag(flags, MemberFlags.Public);
            bool nonPublicFlag = TestMemberFlag(flags, MemberFlags.NonPublic);

            if ((!staticFlag && !instanceFlag) || (!publicFlag && !nonPublicFlag))
            {
                ownerElementType = null;
                return null;
            }

            ownerElementType = elementType;

            while (ownerElementType is not null)
            {
                var resolved = ownerElementType.ResolveOrThrow();

                foreach (var property in resolved.Properties)
                {
                    if (string.Compare(property.Name, propertyName, ignoreCaseFlag) != 0)
                    {
                        continue;
                    }

                    if (staticFlag != instanceFlag && staticFlag != IsStatic(property))
                    {
                        continue;
                    }

                    if (publicFlag != nonPublicFlag && publicFlag != IsPublic(property))
                    {
                        continue;
                    }

                    return property;
                }

                ownerElementType = resolved.BaseType?.PopulateGeneric(elementType, ownerElementType);
            }

            return null;

            static bool IsStatic(PropertyDefinition property)
            {
                return property.GetMethod is not null && property.GetMethod.IsStatic ||
                       property.SetMethod is not null && property.SetMethod.IsStatic;
            }

            static bool IsPublic(PropertyDefinition property)
            {
                return property.GetMethod is not null && property.GetMethod.IsPublic ||
                       property.SetMethod is not null && property.SetMethod.IsPublic;
            }
        }

        private static bool TestMemberFlag(MemberFlags flags, MemberFlags value) => (flags & value) == value;

        internal static FieldDefinition FindFieldDeep(
            TypeDefinition elementType,
            string name,
            MemberFlags flags,
            out TypeReference ownerElementType)
        {
            bool ignoreCaseFlag = TestMemberFlag(flags, MemberFlags.IgnoreCase);
            bool staticFlag = TestMemberFlag(flags, MemberFlags.Static);
            bool instanceFlag = TestMemberFlag(flags, MemberFlags.Instance);
            bool publicFlag = TestMemberFlag(flags, MemberFlags.Public);
            bool nonPublicFlag = TestMemberFlag(flags, MemberFlags.NonPublic);

            if ((!staticFlag && !instanceFlag) || (!publicFlag && !nonPublicFlag))
            {
                ownerElementType = null;
                return null;
            }

            ownerElementType = elementType;

            while (ownerElementType is not null)
            {
                var resolved = ownerElementType.ResolveOrThrow();

                foreach (var field in resolved.Fields)
                {
                    if (string.Compare(field.Name, name, ignoreCaseFlag) != 0)
                    {
                        continue;
                    }

                    if (staticFlag != instanceFlag && staticFlag != field.IsStatic)
                    {
                        continue;
                    }

                    if (publicFlag != nonPublicFlag && publicFlag != field.IsPublic)
                    {
                        continue;
                    }

                    return field;
                }

                ownerElementType = resolved.BaseType?.PopulateGeneric(elementType, ownerElementType);
            }

            return null;
        }

        internal static EventDefinition FindEventDeep(
            TypeDefinition elementType,
            string eventName,
            MemberFlags flags,
            out TypeReference ownerElementType)
        {
            bool ignoreCaseFlag = TestMemberFlag(flags, MemberFlags.IgnoreCase);
            bool staticFlag = TestMemberFlag(flags, MemberFlags.Static);
            bool instanceFlag = TestMemberFlag(flags, MemberFlags.Instance);
            bool publicFlag = TestMemberFlag(flags, MemberFlags.Public);
            bool nonPublicFlag = TestMemberFlag(flags, MemberFlags.NonPublic);

            if ((!staticFlag && !instanceFlag) || (!publicFlag && !nonPublicFlag))
            {
                ownerElementType = null;
                return null;
            }

            ownerElementType = elementType;

            while (ownerElementType is not null)
            {
                var resolved = ownerElementType.ResolveOrThrow();

                foreach (var eventDefinition in resolved.Events)
                {
                    if (string.Compare(eventDefinition.Name, eventName, ignoreCaseFlag) != 0)
                    {
                        continue;
                    }

                    if (staticFlag != instanceFlag && staticFlag != IsStatic(eventDefinition))
                    {
                        continue;
                    }

                    if (publicFlag != nonPublicFlag && publicFlag != IsPublic(eventDefinition))
                    {
                        continue;
                    }

                    return eventDefinition;
                }

                ownerElementType = resolved.BaseType?.PopulateGeneric(elementType, ownerElementType);
            }

            return null;

            static bool IsStatic(EventDefinition eventDefinition)
            {
                return eventDefinition.AddMethod is not null && eventDefinition.AddMethod.IsStatic ||
                       eventDefinition.RemoveMethod is not null && eventDefinition.RemoveMethod.IsStatic;
            }

            static bool IsPublic(EventDefinition eventDefinition)
            {
                return eventDefinition.AddMethod is not null && eventDefinition.AddMethod.IsPublic ||
                       eventDefinition.RemoveMethod is not null && eventDefinition.RemoveMethod.IsPublic;
            }
        }

        public static MethodDefinition FindMethodDeep(
            TypeDefinition elementType,
            string methodName,
            MemberFlags flags,
            out TypeReference ownerElementType)
        {
            bool ignoreCaseFlag = TestMemberFlag(flags, MemberFlags.IgnoreCase);
            bool staticFlag = TestMemberFlag(flags, MemberFlags.Static);
            bool instanceFlag = TestMemberFlag(flags, MemberFlags.Instance);
            bool publicFlag = TestMemberFlag(flags, MemberFlags.Public);
            bool nonPublicFlag = TestMemberFlag(flags, MemberFlags.NonPublic);

            if ((!staticFlag && !instanceFlag) || (!publicFlag && !nonPublicFlag))
            {
                ownerElementType = null;
                return null;
            }

            ownerElementType = elementType;

            while (ownerElementType is not null)
            {
                var resolved = ownerElementType.ResolveOrThrow();

                foreach (var method in resolved.Methods)
                {
                    if (string.Compare(method.Name, methodName, ignoreCaseFlag) != 0)
                    {
                        continue;
                    }

                    if (staticFlag != instanceFlag && staticFlag != method.IsStatic)
                    {
                        continue;
                    }

                    if (publicFlag != nonPublicFlag && publicFlag != method.IsPublic)
                    {
                        continue;
                    }

                    return method;
                }

                ownerElementType = resolved.BaseType?.PopulateGeneric(elementType, ownerElementType);
            }

            return null;
        }

        // Instance-cached wrappers around the static Find*Deep helpers. Memoization is delegated
        // to _caches; the inspector just supplies the cached method-group delegate so the
        // generic miss path knows which Find*Deep to call.
        internal PropertyDefinition FindPropertyDeepCached(
            TypeDefinition elementType, string propertyName, MemberFlags flags, out TypeReference ownerElementType)
            => _caches.FindProperty(elementType, propertyName, flags, s_findPropertyDeep, out ownerElementType);

        internal FieldDefinition FindFieldDeepCached(
            TypeDefinition elementType, string name, MemberFlags flags, out TypeReference ownerElementType)
            => _caches.FindField(elementType, name, flags, s_findFieldDeep, out ownerElementType);

        internal EventDefinition FindEventDeepCached(
            TypeDefinition elementType, string eventName, MemberFlags flags, out TypeReference ownerElementType)
            => _caches.FindEvent(elementType, eventName, flags, s_findEventDeep, out ownerElementType);

        internal MethodDefinition FindMethodDeepCached(
            TypeDefinition elementType, string methodName, MemberFlags flags, out TypeReference ownerElementType)
            => _caches.FindMethod(elementType, methodName, flags, s_findMethodDeep, out ownerElementType);

        private bool IsCollection(TypeDefinition type) =>
            TypeDefinitionExtensions.Equals(type, IListType) || DoesAnySubTypeImplementInterfaceCached(type, IListType);

        private bool IsDictionary(TypeDefinition type) =>
            TypeDefinitionExtensions.Equals(type, IDictionaryType) || DoesAnySubTypeImplementInterfaceCached(type, IDictionaryType);

        // Cached wrappers for the two inheritance-walk primitives. Each IsXxx predicate (and a few
        // internal helpers like IsCollection / IsDictionary) ultimately calls one of these against
        // a fixed target type. The walks are deterministic per (source, target) pair, so we memoize
        // through _caches for the lifetime of the inspector.
        private bool IsSubclassOfCached(TypeDefinition type, TypeDefinition target)
            => _caches.IsSubclassOf(type, target, s_isSubclassOf);

        private bool DoesAnySubTypeImplementInterfaceCached(TypeDefinition type, TypeDefinition iface)
            => _caches.DoesAnySubTypeImplementInterface(type, iface, s_doesAnySubTypeImplementInterface);

        private static CustomAttribute GetCustomAttributeDeep(TypeDefinition type, string fullName)
        {
            while (type is not null)
            {
                foreach (var attribute in type.CustomAttributes)
                {
                    if (attribute.AttributeType.FullName == fullName)
                    {
                        return attribute;
                    }
                }

                type = type.BaseType?.ResolveOrThrow();
            }

            return null;
        }

        public bool IsDependencyObject(TypeDefinition type)
        {
            return TypeDefinitionExtensions.Equals(type, DependencyObjectType) || IsSubclassOfCached(type, DependencyObjectType);
        }

        public bool IsApplication(TypeDefinition type)
        {
            return TypeDefinitionExtensions.Equals(type, ApplicationType) || IsSubclassOfCached(type, ApplicationType);
        }

        public bool IsResourceDictionary(TypeDefinition type)
        {
            return TypeDefinitionExtensions.Equals(type, ResourceDictionaryType) || IsSubclassOfCached(type, ResourceDictionaryType);
        }

        public bool IsStyle(TypeDefinition type)
        {
            return TypeDefinitionExtensions.Equals(type, StyleType) || IsSubclassOfCached(type, StyleType);
        }

        public bool IsFrameworkTemplate(TypeDefinition type)
        {
            return TypeDefinitionExtensions.Equals(type, FrameworkTemplateType) || IsSubclassOfCached(type, FrameworkTemplateType);
        }

        public bool IsDataTemplate(TypeDefinition type)
        {
            return TypeDefinitionExtensions.Equals(type, DataTemplateType) || IsSubclassOfCached(type, DataTemplateType);
        }

        public bool IsControlTemplate(TypeDefinition type)
        {
            return TypeDefinitionExtensions.Equals(type, ControlTemplateType) || IsSubclassOfCached(type, ControlTemplateType);
        }

        public bool IsContentPresenter(TypeDefinition type)
        {
            return TypeDefinitionExtensions.Equals(type, ContentPresenterType) || IsSubclassOfCached(type, ContentPresenterType);
        }

        public bool IsContentControl(TypeDefinition type)
        {
            return TypeDefinitionExtensions.Equals(type, ContentControlType) || IsSubclassOfCached(type, ContentControlType);
        }

        public bool IsRelativeSource(TypeDefinition type)
        {
            return type == RelativeSourceType;
        }

        public bool IsBindingBase(TypeReference type)
        {
            return type == BindingBaseType;
        }

        public bool IsBinding(TypeDefinition type)
        {
            return type == BindingType;
        }

        public bool IsMultiBinding(TypeDefinition type)
        {
            return type == MultiBindingType;
        }

        public bool IsTemplateBindingExtension(TypeDefinition type)
        {
            return type == TemplateBindingExtensionType;
        }

        public bool IsNullExtension(TypeDefinition type)
        {
            return type == NullExtensionType;
        }

        public bool IsStaticExtension(TypeDefinition type)
        {
            return type == StaticExtensionType;
        }

        public bool IsTypeExtension(TypeDefinition type)
        {
            return type == TypeExtensionType;
        }

        public bool IsStaticResourceExtension(TypeDefinition type)
        {
            return type == StaticResourceExtensionType;
        }

        public bool IsThemeResourceExtension(TypeDefinition type)
        {
            return type == ThemeResourceExtensionType;
        }

        public bool IsDynamicResourceExtension(TypeDefinition type)
        {
            return type == DynamicResourceExtensionType;
        }

        public bool IsResponsiveExtensionType(TypeDefinition type)
        {
            return type == ResponsiveExtensionType;
        }

        public bool IsIList(TypeDefinition type)
        {
            return IsCollection(type);
        }

        public bool IsIDictionary(TypeDefinition type)
        {
            return IsDictionary(type);
        }

        public bool IsIUIElement(TypeDefinition type)
        {
            return TypeDefinitionExtensions.Equals(type, IUIElementType) || DoesAnySubTypeImplementInterfaceCached(type, IUIElementType);
        }

        public bool IsIFrameworkElement(TypeDefinition type)
        {
            return TypeDefinitionExtensions.Equals(type, IFrameworkElementType) || DoesAnySubTypeImplementInterfaceCached(type, IFrameworkElementType);
        }

        public bool IsFrameworkTemplateTemplateProperty(MemberReference memberReference)
        {
            return memberReference.Name == "Template" &&
                   memberReference is PropertyDefinition propertyDefinition &&
                   propertyDefinition.DeclaringType == FrameworkTemplateType;
        }

        public bool IsElementAMarkupExtension(TypeDefinition type)
        {
            return DoesAnySubTypeImplementInterfaceCached(type, IMarkupExtensionType);
        }

        public string GetContentPropertyName(TypeDefinition type, IXmlLineInfo lineInfo)
        {
            CustomAttribute contentPropertyAttribute = GetCustomAttributeDeep(type, ContentPropertyAttributeFullName);

            if (contentPropertyAttribute is not null && contentPropertyAttribute.HasConstructorArguments)
            {
                string contentPropertyName = contentPropertyAttribute.ConstructorArguments[0].Value?.ToString();

                if (string.IsNullOrEmpty(contentPropertyName))
                {
                    throw new XamlParseException("The ContentPropertyAttribute must have a non-empty Name.", lineInfo);
                }

                return contentPropertyName;
            }

            if (IsCollection(type) || IsDictionary(type))
            {
                return string.Empty;
            }

            return null;
        }

        public IEnumerable<string> GetEnumValues(TypeDefinition enumType, string name, bool ignoreCase, bool allowIntegerValue, IXmlLineInfo lineInfo)
        {
            name = name.Trim();

            if (name.IndexOf(',') != -1)
            {
                foreach (string token in name.Split(','))
                {
                    string fieldName = token.Trim();

                    // integer values are not allowed when we have multiple values
                    yield return GetEnumValue(enumType, fieldName, ignoreCase, false) ??
                        throw new XamlParseException($"Field '{fieldName}' not found in type: '{_typeReferenceHelper.ConvertToString(enumType)}'.", lineInfo);
                }
            }
            else
            {
                yield return GetEnumValue(enumType, name, ignoreCase, allowIntegerValue) ??
                    throw new XamlParseException($"Field '{name}' not found in type: '{_typeReferenceHelper.ConvertToString(enumType)}'.", lineInfo);
            }
        }

        public string GetEnumValue(TypeDefinition enumType, string name, bool ignoreCase, bool allowIntegerValue)
        {
            // Bypass the cache when enumType is null so we don't pollute it with null keys;
            // the helper handles that case itself.
            if (enumType is null)
            {
                return _typeReferenceHelper.GetEnumValue(enumType, name, ignoreCase, allowIntegerValue);
            }

            return _caches.GetEnumValue(enumType, name, ignoreCase, allowIntegerValue, _getEnumValueFromHelper);
        }

        public static bool HasTypeConverter(MemberReference member)
        {
            return member switch
            {
                PropertyDefinition property => HasTypeConverter(property),
                TypeDefinition type => HasTypeConverter(type),
                _ => false,
            };
        }

        private static bool HasTypeConverter(PropertyDefinition property)
        {
            return property.CustomAttributes.Any(p => p.AttributeType.FullName == TypeConverterAttributeFullName) &&
                !ShouldIgnoreTypeConverter(property);

            static bool ShouldIgnoreTypeConverter(PropertyDefinition propertyInfo)
            {
                var declaringType = propertyInfo.DeclaringType;

                if (declaringType.GetAssemblyName() == Constants.OPENSILVER_ASSEMBLY_NAME)
                {
                    return declaringType.FullName switch
                    {
                        $"{KnownNamespaces.SystemWindows}.FrameworkElement" => ShouldIgnoreFrameworkElementProperty(propertyInfo.Name),
                        $"{KnownNamespaces.SystemWindowsDocuments}.InlineImageContainer" => ShouldIgnoreInlineImageContainerProperty(propertyInfo.Name),
                        _ => false,
                    };
                }

                return false;
            }

            static bool ShouldIgnoreFrameworkElementProperty(string propertyName)
            {
                return propertyName is "Width" or "Height";
            }

            static bool ShouldIgnoreInlineImageContainerProperty(string propertyName)
            {
                return propertyName is "Width" or "Height";
            }
        }

        private static bool HasTypeConverter(TypeDefinition type)
        {
            TypeDefinition t = type;

            do
            {
                if (t.CustomAttributes.Any(p => p.AttributeType.FullName == TypeConverterAttributeFullName))
                {
                    return true;
                }

                if (t.BaseType is null)
                {
                    break;
                }

                t = t.BaseType.ResolveOrThrow();
            } while (t is not null);

            return false;
        }

        public MemberReference GetMemberFromType(
            string memberName,
            TypeDefinition fromType,
            MemberKind lookupFlags,
            out TypeReference declaringType,
            out TypeReference memberType,
            out MemberKind memberKind)
        {
            if (TestFlag(lookupFlags, MemberKind.Property))
            {
                PropertyDefinition propertyDefinition = FindPropertyDeepCached(
                    fromType,
                    memberName,
                    MemberFlags.Public | MemberFlags.NonPublic | MemberFlags.Instance,
                    out declaringType);

                if (propertyDefinition is not null)
                {
                    memberKind = MemberKind.Property;
                    memberType = propertyDefinition.PropertyType.PopulateGeneric(fromType, declaringType);
                    return propertyDefinition;
                }
            }

            if (TestFlag(lookupFlags, MemberKind.AttachedPropertyGet))
            {
                MethodDefinition attachedGetter = FindMethodDeepCached(
                    fromType,
                    $"Get{memberName}",
                    MemberFlags.Public | MemberFlags.NonPublic | MemberFlags.Static,
                    out declaringType);

                if (attachedGetter is not null && attachedGetter.Parameters.Count == 1)
                {
                    memberKind = MemberKind.AttachedPropertyGet;
                    memberType = attachedGetter.ReturnType.PopulateGeneric(fromType, declaringType);
                    return attachedGetter;
                }
            }

            if (TestFlag(lookupFlags, MemberKind.AttachedPropertySet))
            {
                MethodDefinition attachedSetter = FindMethodDeepCached(
                    fromType,
                    $"Set{memberName}",
                    MemberFlags.Public | MemberFlags.NonPublic | MemberFlags.Static,
                    out declaringType);

                if (attachedSetter is not null && attachedSetter.Parameters.Count == 2)
                {
                    memberKind = MemberKind.AttachedPropertySet;
                    memberType = attachedSetter.Parameters[1].ParameterType.PopulateGeneric(fromType, declaringType);
                    return attachedSetter;
                }
            }

            if (TestFlag(lookupFlags, MemberKind.Event))
            {
                EventDefinition eventDefinition = FindEventDeepCached(
                    fromType,
                    memberName,
                    MemberFlags.Public | MemberFlags.NonPublic | MemberFlags.Instance,
                    out declaringType);

                if (eventDefinition is not null)
                {
                    memberKind = MemberKind.Event;
                    memberType = eventDefinition.EventType.PopulateGeneric(fromType, declaringType);
                    return eventDefinition;
                }
            }

            if (TestFlag(lookupFlags, MemberKind.AttachedEvent))
            {
                MethodDefinition attachedAddHandler = FindMethodDeepCached(
                    fromType,
                    $"Add{memberName}Handler",
                    MemberFlags.Public | MemberFlags.NonPublic | MemberFlags.Static,
                    out declaringType);

                if (attachedAddHandler is not null && attachedAddHandler.Parameters.Count == 2)
                {
                    memberKind = MemberKind.AttachedEvent;
                    memberType = attachedAddHandler.Parameters[1].ParameterType.PopulateGeneric(fromType, declaringType);
                    return attachedAddHandler;
                }
            }

            if (TestFlag(lookupFlags, MemberKind.Field))
            {
                FieldDefinition fieldDefinition = FindFieldDeepCached(
                    fromType,
                    memberName,
                    MemberFlags.Public | MemberFlags.NonPublic | MemberFlags.Instance,
                    out declaringType);

                if (fieldDefinition is not null)
                {
                    memberKind = MemberKind.Field;
                    memberType = fieldDefinition.FieldType.PopulateGeneric(fromType, declaringType);
                    return fieldDefinition;
                }
            }

            memberKind = MemberKind.Unknown;
            declaringType = null;
            memberType = null;
            return null;

            static bool TestFlag(MemberKind flags, MemberKind value) => (flags & value) == value;
        }

        private readonly struct TypeKey
        {
            private readonly int _hashCode;

            public TypeKey(string namespaceName, string typeName)
            {
                Namespace = namespaceName;
                Type = typeName;

                _hashCode = GetHashCode(typeName) ^ GetHashCode(namespaceName);
            }

            public readonly string Namespace;
            public readonly string Type;

            public override string ToString()
            {
                if (string.IsNullOrEmpty(Namespace))
                {
                    return Type;
                }
                else
                {
                    return $"{Namespace}.{Type}";
                }
            }

            public override bool Equals(object obj) => obj is TypeKey type && this == type;

            public override int GetHashCode() => _hashCode;

            public static bool operator ==(TypeKey type1, TypeKey type2) =>
                type1.Type == type2.Type && type1.Namespace == type2.Namespace;

            public static bool operator !=(TypeKey type1, TypeKey type2) => !(type1 == type2);

            private static int GetHashCode(string s) => s?.GetHashCode() ?? 0;
        }
    }

    public class InvalidCompilerTypeException : Exception
    {
        public InvalidCompilerTypeException() : base("Invalid Compiler Type")
        {

        }
    }
}