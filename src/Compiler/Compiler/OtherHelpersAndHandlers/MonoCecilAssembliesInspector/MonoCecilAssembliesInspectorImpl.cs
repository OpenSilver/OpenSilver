
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

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using Mono.Cecil;

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

        private const string GenericMarkupExtension = "IMarkupExtension`1";
        private const string ContentPropertyAttributeFullName = "System.Windows.Markup.ContentPropertyAttribute";
        private const string DependencyProperty = "DependencyProperty";
        private const string SetPrefix = "Set";
        private const string GetPrefix = "Get";
        private const string Name = "Name";
        private const string PropertySuffix = "Property";
        private const string Using = "using:";
        private const string ClrNamespace = "clr-namespace:";
        private const string StaticRes = "StaticResource";
        private const string StaticResExtension = "StaticResourceExtension";
        private const string FrameworkTemplateName = "FrameworkTemplate";
        private const string ResourceDictionaryName = "ResourceDictionary";

        private readonly MonoCecilAssemblyStorage _storage;
        private readonly Dictionary<AssemblyDefinition, AssemblyData> _assemblies = [];
        private readonly ConcurrentDictionary<TypeKey, TypeDefinition> _typeNameToType = [];
        private readonly Dictionary<AssemblyDefinition, ConcurrentHashSet<TypeKey>> _typesPerAssembly = [];

        private readonly SystemTypesHelper _systemTypesHelper;
        private readonly TypeReferenceHelper _typeReferenceHelper;
        private readonly XamlNameParser _xamlNameParser;

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
        private TypeDefinition _iUIElementType;
        private TypeDefinition _iFrameworkElementType;
        private TypeDefinition _iMarkupExtensionType;

        private TypeDefinition IListType =>
            _iListType ??= FindType(typeof(IList).Namespace, nameof(IList));

        private TypeDefinition IDictionaryType =>
            _iDictionaryType ??= FindType(typeof(IDictionary).Namespace, nameof(IDictionary));

        private TypeDefinition DependencyObjectType =>
            _dependencyObjectType ??= FindType(KnownNamespaces.SystemWindows, "DependencyObject", Constants.OPENSILVER_ASSEMBLY_NAME);

        private TypeDefinition ApplicationType =>
            _applicationType ??= FindType(KnownNamespaces.SystemWindows, "Application", Constants.OPENSILVER_ASSEMBLY_NAME);

        private TypeDefinition ResourceDictionaryType =>
            _resourceDictionaryType ??= FindType(KnownNamespaces.SystemWindows, "ResourceDictionary", Constants.OPENSILVER_ASSEMBLY_NAME);

        private TypeDefinition StyleType =>
            _styleType ??= FindType(KnownNamespaces.SystemWindows, "Style", Constants.OPENSILVER_ASSEMBLY_NAME);

        private TypeDefinition FrameworkTemplateType =>
            _frameworkTemplateType ??= FindType(KnownNamespaces.SystemWindows, "FrameworkTemplate", Constants.OPENSILVER_ASSEMBLY_NAME);

        private TypeDefinition DataTemplateType =>
            _dataTemplateType ??= FindType(KnownNamespaces.SystemWindows, "DataTemplate", Constants.OPENSILVER_ASSEMBLY_NAME);

        private TypeDefinition ControlTemplateType =>
            _controlTemplateType ??= FindType(KnownNamespaces.SystemWindowsControls, "ControlTemplate", Constants.OPENSILVER_ASSEMBLY_NAME);

        private TypeDefinition ContentPresenterType =>
            _contentPresenterType ??= FindType(KnownNamespaces.SystemWindowsControls, "ContentPresenter", Constants.OPENSILVER_ASSEMBLY_NAME);

        private TypeDefinition ContentControlType =>
            _contentControlType ??= FindType(KnownNamespaces.SystemWindowsControls, "ContentControl", Constants.OPENSILVER_ASSEMBLY_NAME);

        private TypeDefinition IUIElementType =>
            _iUIElementType ??= FindType(KnownNamespaces.SystemWindows, "IUIElement", Constants.OPENSILVER_ASSEMBLY_NAME);

        private TypeDefinition IFrameworkElementType =>
            _iFrameworkElementType ??= FindType(KnownNamespaces.SystemWindows, "IFrameworkElement", Constants.OPENSILVER_ASSEMBLY_NAME);

        private TypeDefinition IMarkupExtensionType =>
            _iMarkupExtensionType ??= FindType(KnownNamespaces.SystemXaml, GenericMarkupExtension, Constants.OPENSILVER_ASSEMBLY_NAME);

        public MonoCecilAssembliesInspectorImpl(string assemblyName, SupportedLanguage compilerType)
        {
            _xamlNameParser = new XamlNameParser(assemblyName);

            switch (compilerType)
            {
                case SupportedLanguage.CSharp:
                    _systemTypesHelper = SystemTypesHelper.CSharp;
                    _typeReferenceHelper = TypeReferenceHelper.CSharp;
                    break;

                case SupportedLanguage.VBNet:
                    _systemTypesHelper = SystemTypesHelper.VisualBasic;
                    _typeReferenceHelper = TypeReferenceHelper.VisualBasic;
                    break;

                case SupportedLanguage.FSharp:
                    _systemTypesHelper = SystemTypesHelper.FSharp;
                    _typeReferenceHelper = TypeReferenceHelper.FSharp;
                    break;

                default:
                    throw new InvalidCompilerTypeException();
            }

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
        }

        public void Dispose()
        {
            _storage.Dispose();
            _assemblies.Clear();
        }

        private TypeDefinition FindType(string namespaceName, string typeName) => FindType(namespaceName, typeName, null, null);

        internal TypeDefinition FindType(string namespaceName, string typeName, string assemblyName)
            => FindType(namespaceName, typeName, assemblyName, null);

        private TypeDefinition FindType(string namespaceName, string typeName, IXmlLineInfo lineInfo)
            => FindType(namespaceName, typeName, null, lineInfo);

        internal TypeDefinition FindType(string namespaceName, string typeName, string assemblyName, IXmlLineInfo lineInfo,
            bool doNotRaiseExceptionIfNotFound = false)
        {
            // Fix the namespace:
            if (namespaceName.StartsWith(Using, StringComparison.CurrentCultureIgnoreCase))
            {
                namespaceName = namespaceName.Substring(Using.Length);
            }
            else if (namespaceName.StartsWith(ClrNamespace, StringComparison.CurrentCultureIgnoreCase))
            {
                // Override assemblyName
                _xamlNameParser.ParseClrNamespaceDeclaration(namespaceName, out string ns, out assemblyName);
                namespaceName = ns;
                XamlNameParser.FixNamespaceForCompatibility(ref assemblyName, ref namespaceName);
            }

            // Handle special cases:
            if (typeName == StaticRes)
            {
                typeName = StaticResExtension;
            }

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

            if (doNotRaiseExceptionIfNotFound)
            {
                return null;
            }

            throw new XamlParseException($"Cannot find type '{typeKey}'.", lineInfo);
        }

        private static bool IsNamespaceAnXmlNamespace(string namespaceName)
        {
            return namespaceName.StartsWith("http://"); //todo: are there other conditions possible for XML namespaces declared with xmlnsDefinitionAttribute?
        }

        private IMemberDefinition GetMemberInfo(string memberName, string namespaceName, string localTypeName, string assemblyNameIfAny,
            IXmlLineInfo lineInfo, bool returnNullIfNotFoundInsteadOfException = false)
        {
            var elementType = FindType(namespaceName, localTypeName, assemblyNameIfAny, lineInfo);
            var typeIterator = elementType;
            while (typeIterator != null)
            {
                var prop = typeIterator.Properties.FirstOrDefault(x => x.Name == memberName);
                if (prop != null) return prop;

                var method = FindMethod(typeIterator, memberName);
                if (method != null) return method;

                var ev = typeIterator.Events.FirstOrDefault(x => x.Name == memberName);
                if (ev != null) return ev;

                var field = typeIterator.Fields.FirstOrDefault(x => x.Name == memberName);
                if (field != null) return field;

                typeIterator = typeIterator.BaseType?.ResolveOrThrow();
            }

            if (returnNullIfNotFoundInsteadOfException)
                return null;

            throw new XamlParseException($"Member '{memberName}' not found in type '{elementType}'.", lineInfo);
        }

        internal static PropertyDefinition FindPropertyDeep(TypeDefinition elementType, string propertyName, out TypeReference ownerElementType)
            => FindPropertyDeep(elementType, propertyName, out ownerElementType, false, false, false);

        private static PropertyDefinition FindPropertyDeep(TypeDefinition elementType, string propertyName,
            out TypeReference ownerElementType, bool ignoreCase = false, bool staticOnly = false, bool publicOnly = false)
        {
            ownerElementType = elementType;
            while (ownerElementType != null)
            {
                var resolved = ownerElementType.ResolveOrThrow();
                var propertyDefinition = resolved.Properties.FirstOrDefault(p =>
                    string.Compare(p.Name, propertyName, ignoreCase) == 0 && (!staticOnly || p.GetMethod.IsStatic) &&
                    (!publicOnly || p.GetMethod.IsPublic));
                if (propertyDefinition != null) return propertyDefinition;

                ownerElementType = resolved.BaseType?.PopulateGeneric(elementType, ownerElementType);
            }

            return null;
        }

        internal static PropertyDefinition FindPropertyGetterDeep(TypeDefinition type, string name, out TypeReference ownerElementType,
            bool staticOnly = false, bool publicOnly = false)
        {
            ownerElementType = type;
            while (ownerElementType != null)
            {
                var resolved = ownerElementType.ResolveOrThrow();
                var propertyDefinition = resolved.Properties.FirstOrDefault(p =>
                {
                    if (p.Name != name)
                    {
                        return false;
                    }

                    if (p.GetMethod is MethodDefinition getMethod)
                    {
                        return (!staticOnly || getMethod.IsStatic) && (!publicOnly || getMethod.IsPublic);
                    }

                    return false;
                });

                if (propertyDefinition != null) return propertyDefinition;

                ownerElementType = resolved.BaseType?.PopulateGeneric(type, ownerElementType);
            }

            return null;
        }

        internal static FieldDefinition FindFieldDeep(TypeDefinition elementType, string propertyName,
            out TypeReference ownerElementType, bool ignoreCase = false, bool staticOnly = false, bool publicOnly = false)
        {
            ownerElementType = elementType;
            while (ownerElementType != null)
            {
                var resolved = ownerElementType.ResolveOrThrow();
                var fieldDefinition = resolved.Fields.FirstOrDefault(p =>
                    string.Compare(p.Name, propertyName, ignoreCase) == 0 && (!staticOnly || p.IsStatic) &&
                    (!publicOnly || p.IsPublic));
                if (fieldDefinition != null) return fieldDefinition;

                ownerElementType = resolved.BaseType?.PopulateGeneric(elementType, ownerElementType);
            }

            return null;
        }

        internal static EventDefinition FindEventDeep(TypeDefinition elementType, string eventName,
            out TypeReference ownerElementType, bool publicOnly)
        {
            ownerElementType = elementType;
            while (ownerElementType != null)
            {
                var resolved = ownerElementType.ResolveOrThrow();
                var eventDefinition = resolved.Events.FirstOrDefault(p =>
                    string.Equals(p.Name, eventName, StringComparison.Ordinal) &&
                    !p.AddMethod.IsStatic &&
                    (!publicOnly || p.AddMethod.IsPublic));

                if (eventDefinition != null) return eventDefinition;

                ownerElementType = resolved.BaseType?.PopulateGeneric(elementType, ownerElementType);
            }

            return null;
        }

        public static MethodDefinition FindMethodDeep(TypeDefinition elementType,
            string methodName,
            bool onlyPublic,
            bool onlyStatic,
            out TypeReference ownerElementType)
        {
            ownerElementType = elementType;
            while (ownerElementType != null)
            {
                var resolved = ownerElementType.ResolveOrThrow();
                var methodInfo = FindMethod(resolved, methodName, onlyPublic, onlyStatic);
                if (methodInfo != null)
                {
                    return methodInfo;
                }
                ownerElementType = resolved.BaseType?.PopulateGeneric(elementType, ownerElementType);
            }

            return null;
        }

        private TypeReference GetPropertyOrFieldType(string propertyName, string namespaceName, string localTypeName,
            string assemblyNameIfAny, IXmlLineInfo lineInfo, bool isAttached = false)
        {
            return GetPropertyOrFieldType(propertyName, namespaceName, localTypeName, assemblyNameIfAny, lineInfo,
                out _, isAttached);
        }

        private TypeReference GetPropertyOrFieldType(string propertyName, string namespaceName, string localTypeName, string assemblyNameIfAny,
            IXmlLineInfo lineInfo, out bool hasTypeConverter, bool isAttached = false)
        {
            const string TypeConverterAttributeFullName = "System.ComponentModel.TypeConverterAttribute";

            hasTypeConverter = false;

            if (isAttached)
            {
                return GetMethodReturnValueType(GetPrefix + propertyName, namespaceName, localTypeName, assemblyNameIfAny, lineInfo);
            }

            var elementType = FindType(namespaceName, localTypeName, assemblyNameIfAny, lineInfo);

            if (FindPropertyDeep(elementType, propertyName, out TypeReference ownerElementType) is PropertyDefinition propertyInfo)
            {
                if (propertyInfo.CustomAttributes.Any(p => p.AttributeType.FullName == TypeConverterAttributeFullName))
                {
                    hasTypeConverter = !ShouldIgnoreTypeConverter(propertyInfo);
                }

                var propertyType = propertyInfo.PropertyType;
                var returnType = propertyType.PopulateGeneric(elementType, ownerElementType);
                return returnType;
            }

            if (FindFieldDeep(elementType, propertyName, out TypeReference fieldOwnerElementType) is FieldDefinition fieldInfo)
            {
                var fieldType = fieldInfo.FieldType;
                return fieldType.PopulateGeneric(elementType, fieldOwnerElementType);
            }

            throw new XamlParseException($"Property or field '{propertyName}' not found in type '{elementType}'.", lineInfo);
        }

        private static bool ShouldIgnoreTypeConverter(PropertyDefinition propertyInfo)
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

            static bool ShouldIgnoreFrameworkElementProperty(string propertyName)
            {
                return propertyName is "Width" or "Height";
            }

            static bool ShouldIgnoreInlineImageContainerProperty(string propertyName)
            {
                return propertyName is "Width" or "Height";
            }
        }

        private TypeReference GetMethodReturnValueType(
            string methodName,
            string namespaceName,
            string localTypeName,
            string assemblyNameIfAny,
            IXmlLineInfo lineInfo)
        {
            var elementType = FindType(namespaceName, localTypeName, assemblyNameIfAny, lineInfo);
            var methodInfo = FindMethodDeep(elementType, methodName, false, false, out var ownerElementType);

            if (methodInfo == null)
            {
                throw new XamlParseException($"Method '{methodName}' not found in type '{elementType}'.", lineInfo);
            }

            return methodInfo.ReturnType.PopulateGeneric(elementType, ownerElementType);
        }

        private bool IsCollection(TypeDefinition type) =>
            TypeDefinitionExtensions.Equals(type, IListType) || type.DoesAnySubTypeImplementInterface(IListType);

        private bool IsDictionary(TypeDefinition type) =>
            TypeDefinitionExtensions.Equals(type, IDictionaryType) || type.DoesAnySubTypeImplementInterface(IDictionaryType);

        private bool IsElementACollection(string elementNameSpace, string elementLocalName, string assemblyNameIfAny, IXmlLineInfo lineInfo)
        {
            var elementType = FindType(elementNameSpace, elementLocalName, assemblyNameIfAny, lineInfo);
            return IsCollection(elementType);
        }

        private bool IsDictionary(string elementNameSpace, string elementLocalName, string assemblyNameIfAny, IXmlLineInfo lineInfo)
        {
            var elementType = FindType(elementNameSpace, elementLocalName, assemblyNameIfAny, lineInfo);
            return IsDictionary(elementType);
        }

        private static CustomAttribute GetCustomAttributeDeep(TypeDefinition type, string fullName)
        {
            while (type != null)
            {
                var customAttr = type.CustomAttributes.FirstOrDefault(ca =>
                    ca.AttributeType.FullName == fullName);

                if (customAttr != null) return customAttr;

                type = type.BaseType?.ResolveOrThrow();
            }

            return null;
        }

        public string GetCSharpEquivalentOfXamlTypeAsString(string namespaceName, string localTypeName, string assemblyNameIfAny,
            IXmlLineInfo lineInfo, bool ifTypeNotFoundTryGuessing = false)
        {
            // Distinguish between system types (String, Double...) and other types
            if (_systemTypesHelper.IsKnownType($"{namespaceName}.{localTypeName}", assemblyNameIfAny))
                return _systemTypesHelper.GetFullTypeName(namespaceName, localTypeName, assemblyNameIfAny);

            // Find the type:
            var type = FindType(
                namespaceName, localTypeName, assemblyNameIfAny, lineInfo, ifTypeNotFoundTryGuessing);

            if (type != null)
            {
                // Use information from the type
                return $"{_typeReferenceHelper.Global}{type}";
            }

            if (ifTypeNotFoundTryGuessing)
            {
                // Try guessing
                if (IsNamespaceAnXmlNamespace(namespaceName))
                    // Attempt to find the type in the current namespace
                    return localTypeName;

                return $"{_typeReferenceHelper.Global}{namespaceName}{(string.IsNullOrEmpty(namespaceName) ? string.Empty : ".")}{localTypeName}";
            }

            throw new XamlParseException($"Type '{localTypeName}' not found in namespace '{namespaceName}'.", lineInfo);
        }

        public string GetAssemblyQualifiedNameOfXamlType(
            string namespaceName,
            string localTypeName,
            string assemblyNameIfAny,
            IXmlLineInfo lineInfo)
        {
            var type = FindType(namespaceName, localTypeName, assemblyNameIfAny, lineInfo, true);

            if (type != null)
            {
                return _typeReferenceHelper.ConvertToString(type) + ", " + type.Module.Assembly.Name.Name;
            }

            return null;
        }

        public bool IsDependencyObject(string namespaceName, string typeName, string assemblyName, IXmlLineInfo lineInfo)
        {
            TypeDefinition type = FindType(namespaceName, typeName, assemblyName, lineInfo);
            return TypeDefinitionExtensions.Equals(type, DependencyObjectType) || type.IsSubclassOf(DependencyObjectType);
        }

        public bool IsApplication(string namespaceName, string typeName, string assemblyName, IXmlLineInfo lineInfo)
        {
            TypeDefinition type = FindType(namespaceName, typeName, assemblyName, lineInfo);
            return TypeDefinitionExtensions.Equals(type, ApplicationType) || type.IsSubclassOf(ApplicationType);
        }

        public bool IsResourceDictionary(string namespaceName, string typeName, string assemblyName, IXmlLineInfo lineInfo)
        {
            TypeDefinition type = FindType(namespaceName, typeName, assemblyName, lineInfo);
            return TypeDefinitionExtensions.Equals(type, ResourceDictionaryType) || type.IsSubclassOf(ResourceDictionaryType);
        }

        public bool IsStyle(string namespaceName, string typeName, string assemblyName, IXmlLineInfo lineInfo)
        {
            TypeDefinition type = FindType(namespaceName, typeName, assemblyName, lineInfo);
            return TypeDefinitionExtensions.Equals(type, StyleType) || type.IsSubclassOf(StyleType);
        }

        public bool IsFrameworkTemplate(string namespaceName, string typeName, string assemblyName, IXmlLineInfo lineInfo)
        {
            TypeDefinition type = FindType(namespaceName, typeName, assemblyName, lineInfo);
            return TypeDefinitionExtensions.Equals(type, FrameworkTemplateType) || type.IsSubclassOf(FrameworkTemplateType);
        }

        public bool IsDataTemplate(string namespaceName, string typeName, string assemblyName, IXmlLineInfo lineInfo)
        {
            TypeDefinition type = FindType(namespaceName, typeName, assemblyName, lineInfo);
            return TypeDefinitionExtensions.Equals(type, DataTemplateType) || type.IsSubclassOf(DataTemplateType);
        }

        public bool IsControlTemplate(string namespaceName, string typeName, string assemblyName, IXmlLineInfo lineInfo)
        {
            TypeDefinition type = FindType(namespaceName, typeName, assemblyName, lineInfo);
            return TypeDefinitionExtensions.Equals(type, ControlTemplateType) || type.IsSubclassOf(ControlTemplateType);
        }

        public bool IsContentPresenter(string namespaceName, string typeName, string assemblyName, IXmlLineInfo lineInfo)
        {
            TypeDefinition type = FindType(namespaceName, typeName, assemblyName, lineInfo);
            return TypeDefinitionExtensions.Equals(type, ContentPresenterType) || type.IsSubclassOf(ContentPresenterType);
        }

        public bool IsContentControl(string namespaceName, string typeName, string assemblyName, IXmlLineInfo lineInfo)
        {
            TypeDefinition type = FindType(namespaceName, typeName, assemblyName, lineInfo);
            return TypeDefinitionExtensions.Equals(type, ContentControlType) || type.IsSubclassOf(ContentControlType);
        }

        public bool IsIUIElement(string namespaceName, string typeName, string assemblyName, IXmlLineInfo lineInfo)
        {
            TypeDefinition type = FindType(namespaceName, typeName, assemblyName, lineInfo);
            return TypeDefinitionExtensions.Equals(type, IUIElementType) || type.DoesAnySubTypeImplementInterface(IUIElementType);
        }

        public bool IsIFrameworkElement(string namespaceName, string typeName, string assemblyName, IXmlLineInfo lineInfo)
        {
            TypeDefinition type = FindType(namespaceName, typeName, assemblyName, lineInfo);
            return TypeDefinitionExtensions.Equals(type, IFrameworkElementType) || type.DoesAnySubTypeImplementInterface(IFrameworkElementType);
        }

        public bool IsFrameworkTemplateTemplateProperty(string propertyName, string namespaceName, string typeName, string assemblyName, IXmlLineInfo lineInfo)
        {
            const string TemplatePropertyName = "Template";

            if (propertyName != TemplatePropertyName)
            {
                return false;
            }

            var type = FindType(namespaceName, typeName, assemblyName, lineInfo);

            return FindPropertyDeep(type, TemplatePropertyName, out _) is PropertyDefinition prop &&
                prop.DeclaringType.Name == FrameworkTemplateName &&
                prop.DeclaringType.Namespace == KnownNamespaces.SystemWindows &&
                prop.DeclaringType.Module.Assembly.Name.Name == Constants.OPENSILVER_ASSEMBLY_NAME;
        }

        public bool IsResourceDictionarySourcePropertyVisible(string namespaceName, string typeName, IXmlLineInfo lineInfo)
        {
            var type = FindType(namespaceName, typeName, lineInfo);

            return FindPropertyDeep(type, "Source", out _) is PropertyDefinition prop &&
                prop.DeclaringType.Name == ResourceDictionaryName &&
                prop.DeclaringType.Namespace == KnownNamespaces.SystemWindows &&
                prop.DeclaringType.Module.Assembly.Name.Name == Constants.OPENSILVER_ASSEMBLY_NAME;
        }

        public MemberTypes GetMemberType(string memberName, string namespaceName, string localTypeName, string assemblyNameIfAny, IXmlLineInfo lineInfo)
        {
            var memberInfo = GetMemberInfo(memberName, namespaceName, localTypeName, assemblyNameIfAny, lineInfo);
            switch (memberInfo)
            {
                case PropertyDefinition _:
                    return MemberTypes.Property;
                case MethodDefinition _:
                    return MemberTypes.Method;
                case FieldDefinition _:
                    return MemberTypes.Field;
                case EventDefinition _:
                    return MemberTypes.Event;
                default:
                    return MemberTypes.Custom;
            }
        }

        public (MemberTypes Type, MethodDefinition Method, TypeReference DeclaringType) GetAttachedMemberType(
            string memberName, string ownerTypeNamespace, string ownerTypeName, string ownerTypeAssemblyName, IXmlLineInfo lineInfo)
        {
            TypeDefinition ownerType = FindType(ownerTypeNamespace, ownerTypeName, ownerTypeAssemblyName, lineInfo);

            TypeReference declaringType;

            // First try attached property
            if (FindMethodDeep(ownerType, $"Set{memberName}", true, true, out declaringType) is MethodDefinition setMethod)
            {
                return (MemberTypes.Property, setMethod, declaringType);
            }

            // Then attached event
            if (FindMethodDeep(ownerType, $"Add{memberName}Handler", true, true, out declaringType) is MethodDefinition addMethod)
            {
                return (MemberTypes.Event, addMethod, declaringType);
            }

            return (MemberTypes.Custom, null, null);
        }

        public void GetPropertyOrFieldTypeInfo(string propertyOrFieldName, string namespaceName, string localTypeName, string assemblyNameIfAny, IXmlLineInfo lineInfo,
            out string propertyNamespaceName, out string propertyLocalTypeName, out string propertyAssemblyName,
            out bool isTypeEnum, bool isAttached = false)
        {
            GetPropertyOrFieldTypeInfo(propertyOrFieldName, namespaceName, localTypeName, assemblyNameIfAny, lineInfo,
                out propertyNamespaceName, out propertyLocalTypeName, out propertyAssemblyName,
                out isTypeEnum, out _, isAttached);
        }

        public void GetPropertyOrFieldTypeInfo(string propertyOrFieldName, string namespaceName, string localTypeName, string assemblyNameIfAny,
            IXmlLineInfo lineInfo, out string propertyNamespaceName, out string propertyLocalTypeName, out string propertyAssemblyName,
            out bool isTypeEnum, out bool hasTypeConverter, bool isAttached = false)
        {
            var typeRef = GetPropertyOrFieldType(propertyOrFieldName, namespaceName, localTypeName, assemblyNameIfAny, lineInfo,
                out hasTypeConverter, isAttached);
            propertyNamespaceName = _typeReferenceHelper.BuildFullPath(typeRef);
            propertyLocalTypeName = _typeReferenceHelper.GetTypeNameIncludingGenericArguments(typeRef, false);
            propertyAssemblyName = typeRef.ResolveOrThrow().Module.Assembly.Name.Name;
            isTypeEnum = _typeReferenceHelper.IsEnum(typeRef.ResolveOrThrow());
        }

        public void GetMethodReturnValueTypeInfo(string methodName, string namespaceName, string localTypeName, string assemblyNameIfAny, IXmlLineInfo lineInfo,
            out string returnValueNamespaceName, out string returnValueLocalTypeName,
            out string returnValueAssemblyName, out bool isTypeEnum)
        {
            var typeDef = GetMethodReturnValueType(methodName, namespaceName, localTypeName, assemblyNameIfAny, lineInfo);
            returnValueNamespaceName = _typeReferenceHelper.BuildFullPath(typeDef);
            returnValueLocalTypeName = _typeReferenceHelper.GetTypeNameIncludingGenericArguments(typeDef, false);
            returnValueAssemblyName = typeDef.ResolveOrThrow().Module.Assembly.Name.Name;
            isTypeEnum = typeDef.ResolveOrThrow().IsEnum;
        }

        public bool IsElementAMarkupExtension(string elementNameSpace, string elementLocalName, string assemblyNameIfAny, IXmlLineInfo lineInfo)
        {
            var elementType = FindType(elementNameSpace, elementLocalName, assemblyNameIfAny, lineInfo);
            return elementType.DoesAnySubTypeImplementInterface(IMarkupExtensionType);
        }

        public bool IsTypeAssignableFrom(
            string nameSpaceOfTypeToAssignFrom, string nameOfTypeToAssignFrom, string assemblyNameOfTypeToAssignFrom,
            string nameSpaceOfTypeToAssignTo, string nameOfTypeToAssignTo, string assemblyNameOfTypeToAssignTo,
            IXmlLineInfo lineInfo, bool isAttached = false)
        {
            TypeDefinition typeOfElementToAssignFrom;
            TypeDefinition typeOfElementToAssignTo;

            var indexOfLastDot = nameOfTypeToAssignFrom.LastIndexOf('.');

            if (indexOfLastDot == -1)
            {
                typeOfElementToAssignFrom = FindType(nameSpaceOfTypeToAssignFrom, nameOfTypeToAssignFrom,
                    assemblyNameOfTypeToAssignFrom, lineInfo);
            }
            else
            {
                var localTypeName = nameOfTypeToAssignFrom.Substring(0, indexOfLastDot);
                var propertyName = nameOfTypeToAssignFrom.Substring(indexOfLastDot + 1);
                typeOfElementToAssignFrom = GetPropertyOrFieldType(propertyName, nameSpaceOfTypeToAssignFrom,
                    localTypeName, assemblyNameOfTypeToAssignFrom, lineInfo).ResolveOrThrow();
            }

            indexOfLastDot = nameOfTypeToAssignTo.LastIndexOf('.');
            if (indexOfLastDot == -1)
            {
                typeOfElementToAssignTo = FindType(nameSpaceOfTypeToAssignTo, nameOfTypeToAssignTo,
                    assemblyNameOfTypeToAssignTo, lineInfo);
            }
            else
            {
                var localTypeName = nameOfTypeToAssignTo.Substring(0, indexOfLastDot);
                var propertyName = nameOfTypeToAssignTo.Substring(indexOfLastDot + 1);
                typeOfElementToAssignTo = GetPropertyOrFieldType(propertyName, nameSpaceOfTypeToAssignTo, localTypeName,
                    assemblyNameOfTypeToAssignTo, lineInfo, isAttached).ResolveOrThrow();
            }

            return typeOfElementToAssignTo.IsAssignableFrom(typeOfElementToAssignFrom);
        }

        public string GetContentPropertyName(string namespaceName, string localTypeName, string assemblyNameIfAny, IXmlLineInfo lineInfo)
        {
            var type = FindType(namespaceName, localTypeName, assemblyNameIfAny, lineInfo);

            // Get instance of the attribute:
            var contentPropertyAttr = GetCustomAttributeDeep(type, ContentPropertyAttributeFullName);

            if (contentPropertyAttr == null &&
                !IsElementACollection(namespaceName, localTypeName, assemblyNameIfAny, lineInfo) &&
                !IsDictionary(namespaceName, localTypeName, assemblyNameIfAny, lineInfo))
            {
                //if the element is a collection, it is possible to add the children directly to this element.
                throw new XamlParseException($"No default content property exists for element: '{localTypeName}'.", lineInfo);
            }

            if (contentPropertyAttr == null)
            {
                return null;
            }

            var value = contentPropertyAttr.ConstructorArguments[0].Value.ToString();

            if (string.IsNullOrEmpty(value))
            {
                throw new XamlParseException("The ContentPropertyAttribute must have a non-empty Name.", lineInfo);
            }

            return value;
        }

        public bool IsTypeAnEnum(string namespaceName, string typeName, string assemblyName, IXmlLineInfo lineInfo)
        {
            var elementType = FindType(namespaceName, typeName, assemblyName, lineInfo);
            return elementType.IsEnum;
        }

        public static MethodDefinition FindMethod(TypeDefinition td, string methodName, bool onlyPublic = false,
            bool onlyStatic = false)
        {
            return td.Methods.FirstOrDefault(m =>
                m.Name == methodName && (!onlyPublic || m.IsPublic) && (!onlyStatic || m.IsStatic));
        }

        public bool IsPropertyAttached(string propertyOrFieldName, string namespaceName, string typeName, string assemblyName, IXmlLineInfo lineInfo)
        {
            var elementType = FindType(namespaceName, typeName, assemblyName, lineInfo);

            var field = FindFieldDeep(elementType, propertyOrFieldName + PropertySuffix, out _) ??
                        FindFieldDeep(elementType, propertyOrFieldName + PropertySuffix.ToLower(), out _);

            if (field == null) return false;

            if (field.FieldType.Name != DependencyProperty) return false;

            var nbOfParameters = 2;
            var method = FindMethod(field.DeclaringType, SetPrefix + propertyOrFieldName, true, true);
            if (method == null)
            {
                method = FindMethod(field.DeclaringType, GetPrefix + propertyOrFieldName, true, true);
                nbOfParameters = 1;
            }

            if (method == null) return false;

            return method.Parameters.Count == nbOfParameters;
        }

        public bool IsPropertyOrFieldACollection(string propertyName, string namespaceName, string typeName, string assemblyName, IXmlLineInfo lineInfo)
        {
            var propertyOrFieldType = GetPropertyOrFieldType(propertyName, namespaceName, typeName, assemblyName, lineInfo);
            return IsCollection(propertyOrFieldType.ResolveOrThrow()) || IsDictionary(propertyOrFieldType.ResolveOrThrow());
        }

        public bool IsPropertyOrFieldADictionary(string propertyName, string namespaceName, string typeName, string assemblyName, IXmlLineInfo lineInfo)
        {
            var propertyOrFieldType = GetPropertyOrFieldType(propertyName, namespaceName, typeName, assemblyName, lineInfo);
            return IsDictionary(propertyOrFieldType.ResolveOrThrow());
        }

        public XName GetCSharpEquivalentOfXamlTypeAsXName(string namespaceName, string localTypeName, string assemblyNameIfAny, IXmlLineInfo lineInfo)
        {
            //todo: in this method, we assume that the alias will be global, which will be false if the user chose something else --> find the right alias.
            // Find the type:
            if (FindType(namespaceName, localTypeName, assemblyNameIfAny, lineInfo) is not TypeDefinition type)
            {
                throw new XamlParseException($"Type '{localTypeName}' not found in namespace '{namespaceName}'.", lineInfo);
            }

            // Use information from the type:
            return XName.Get(type.Name, namespaceName);
        }

        public bool DoesTypeContainNameMemberOfTypeString(string namespaceName, string localTypeName, string assemblyNameIfAny, IXmlLineInfo lineInfo)
        {
            var memberInfo = GetMemberInfo(Name, namespaceName, localTypeName, assemblyNameIfAny, lineInfo, true);
            if (memberInfo == null) return false;

            if (memberInfo is FieldDefinition fd && fd.FieldType.IsString() && fd.IsPublic && !fd.IsStatic) return true;

            if (memberInfo is PropertyDefinition pd && pd.PropertyType.IsString()) return true;

            return false;
        }

        public bool DoesMethodReturnACollection(string methodName, string typeNamespaceName, string localTypeName, string assemblyName,
            IXmlLineInfo lineInfo)
        {
            var propertyType = GetMethodReturnValueType(methodName, typeNamespaceName, localTypeName, assemblyName, lineInfo);
            return IsCollection(propertyType.ResolveOrThrow())
                   || IsDictionary(propertyType.ResolveOrThrow());
        }

        public bool DoesMethodReturnADictionary(string methodName, string typeNamespaceName, string localTypeName, string assemblyName,
            IXmlLineInfo lineInfo)
        {
            var propertyType = GetMethodReturnValueType(methodName, typeNamespaceName, localTypeName, assemblyName, lineInfo);
            return IsDictionary(propertyType.ResolveOrThrow());
        }

        public string GetField(string fieldName, string namespaceName, string typeName, string assemblyName, IXmlLineInfo lineInfo)
        {
            var type = FindType(namespaceName, typeName, null, lineInfo, true);

            var field = FindFieldDeep(type, fieldName, out _, false, false, assemblyName != type.Module.Name);
            if (field != null && (field.IsPublic || field.IsAssembly || field.IsFamilyOrAssembly))
                return $"{_typeReferenceHelper.GetTypeNameIncludingGenericArguments(type, true)}.{field.Name}";

            return null;
        }

        public string GetProperty(string fieldName, string namespaceName, string typeName, string assemblyName, IXmlLineInfo lineInfo)
        {
            var type = FindType(namespaceName, typeName, null, lineInfo, true);

            var property = FindPropertyDeep(type, fieldName, out _, false, false, assemblyName != type.Module.Name);
            if (property != null && (property.GetMethod.IsPublic || property.GetMethod.IsAssembly || property.GetMethod.IsFamilyOrAssembly))
            {
                return $"{_typeReferenceHelper.GetTypeNameIncludingGenericArguments(type, true)}.{property.Name}";
            }

            return null;
        }

        public void GetPropertyOrFieldInfo(string propertyOrFieldName, string namespaceName, string localTypeName, string assemblyNameIfAny,
            IXmlLineInfo lineInfo, out string memberDeclaringTypeName, out string memberTypeNamespace, out string memberTypeName)
        {
            var elementType = FindType(namespaceName, localTypeName, assemblyNameIfAny, lineInfo);
            var propertyInfo = FindPropertyDeep(elementType, propertyOrFieldName, out var ownerElementType);
            TypeReference propertyOrFieldType;
            TypeReference propertyOrFieldDeclaringType;

            if (propertyInfo == null)
            {
                var fieldInfo = FindFieldDeep(elementType, propertyOrFieldName, out var fieldOwnerElementType);
                if (fieldInfo == null)
                {
                    throw new XamlParseException($"Property or field '{propertyOrFieldName}' not found in type '{elementType}'.", lineInfo);
                }

                propertyOrFieldType = fieldInfo.FieldType.PopulateGeneric(elementType, fieldOwnerElementType);
                propertyOrFieldDeclaringType = fieldOwnerElementType;
            }
            else
            {
                propertyOrFieldType = propertyInfo.PropertyType.PopulateGeneric(elementType, ownerElementType);
                propertyOrFieldDeclaringType = ownerElementType;
            }


            memberDeclaringTypeName = _typeReferenceHelper.GetTypeNameIncludingGenericArguments(propertyOrFieldDeclaringType, true);
            memberTypeNamespace = _typeReferenceHelper.BuildFullPath(propertyOrFieldType);
            memberTypeName = _typeReferenceHelper.GetTypeNameIncludingGenericArguments(propertyOrFieldType, false);
        }

        public void GetAttachedPropertyGetMethodInfo(
            string methodName,
            string namespaceName,
            string localTypeName,
            string assemblyNameIfAny,
            IXmlLineInfo lineInfo,
            out string declaringTypeName,
            out string returnValueNamespaceName,
            out string returnValueLocalTypeName)
        {
            var dependencyObjectType = DependencyObjectType;

            var elementType = FindType(namespaceName, localTypeName, assemblyNameIfAny, lineInfo);
            TypeReference currentType = elementType;
            while (currentType != null)
            {
                var resolved = currentType.ResolveOrThrow();
                var method = resolved.Methods.FirstOrDefault(m =>
                    m.Name == methodName && m.IsStatic && m.IsPublic && m.Parameters.Count == 1 &&
                    dependencyObjectType.IsAssignableFrom(m.Parameters[0].ParameterType.ResolveOrThrow()));
                if (method != null)
                {
                    declaringTypeName = _typeReferenceHelper.GetTypeNameIncludingGenericArguments(currentType, true);
                    var returnType = method.ReturnType.PopulateGeneric(elementType, currentType);
                    returnValueNamespaceName = _typeReferenceHelper.BuildFullPath(returnType);
                    returnValueLocalTypeName = _typeReferenceHelper.GetTypeNameIncludingGenericArguments(returnType, false);
                    return;
                }
                currentType = resolved.BaseType?.PopulateGeneric(elementType, currentType);
            }
            throw new XamlParseException($"Method '{methodName}' not found in type '{elementType}'.", lineInfo);
        }

        public bool IsElementADictionary(string elementNameSpace, string elementLocalName, string assemblyNameIfAny, IXmlLineInfo lineInfo)
        {
            var elementType = FindType(elementNameSpace, elementLocalName, assemblyNameIfAny, lineInfo);
            return IsDictionary(elementType);
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
            return _typeReferenceHelper.GetEnumValue(enumType, name, ignoreCase, allowIntegerValue);
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