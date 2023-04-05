
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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Mono.Cecil;
using OpenSilver.Compiler.Common;
using OpenSilver.Compiler.Common.Helpers;

namespace OpenSilver.Compiler.OtherHelpersAndHandlers.MonoCecilAssembliesInspector
{
    internal class MonoCecilAssembliesInspectorImpl : IDisposable
    {
        private const string GlobalPrefix = "global::";
        private const string SystemXamlNamespace = "System.Xaml";
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
        private const string DependencyObj = "DependencyObject";

        private readonly Dictionary<string, TypeDefinition> _typeNameToType = new();

        private readonly IMetadata _metadata;

        private readonly MonoCecilAssemblyStorage _storage = new();

        public MonoCecilAssembliesInspectorImpl(bool isSlMigration)
        {
            _metadata = isSlMigration ? Metadatas.Silverlight : Metadatas.UWP;
        }

        public void Dispose()
        {
            _storage.Dispose();
        }

        private TypeDefinition FindType(string namespaceName, string localTypeName,
            string filterAssembliesAndRetainOnlyThoseThatHaveThisName = null,
            bool doNotRaiseExceptionIfNotFound = false)
        {
            // Fix the namespace:
            if (namespaceName.StartsWith(Using, StringComparison.CurrentCultureIgnoreCase))
            {
                namespaceName = namespaceName.Substring(Using.Length);
            }
            else if (namespaceName.StartsWith(ClrNamespace, StringComparison.CurrentCultureIgnoreCase))
            {
                GettingInformationAboutXamlTypes.ParseClrNamespaceDeclaration(namespaceName, out var ns,
                    out var assemblyNameIfAny);
                namespaceName = ns;
                GettingInformationAboutXamlTypes.FixNamespaceForCompatibility(ref assemblyNameIfAny, ref namespaceName);
            }

            if (namespaceName.StartsWith(GlobalPrefix,
                    StringComparison
                        .CurrentCultureIgnoreCase)) // Note: normally in XAML there is no "global::", but we may enter this method passing a C#-style namespace (cf. section that handles Binding in "GeneratingCSharpCode.cs")
                namespaceName = namespaceName.Substring(GlobalPrefix.Length);

            // Handle special cases:
            if (localTypeName == StaticRes) localTypeName = StaticResExtension;

            // Generate string representing the type:
            var fullTypeNameWithNamespaceInsideBraces = !string.IsNullOrEmpty(namespaceName)
                ? "{" + namespaceName + "}" + localTypeName
                : localTypeName;

            // Start by looking in the cache dictionary:
            if (_typeNameToType.TryGetValue(fullTypeNameWithNamespaceInsideBraces, out var type)) return type;

            // Look for the type in all loaded assemblies:
            foreach (var assemblyKeyValuePair in _storage.LoadedAssemblySimpleNameToAssembly)
            {
                var assemblySimpleName = assemblyKeyValuePair.Key;
                var assembly = assemblyKeyValuePair.Value;
                if (filterAssembliesAndRetainOnlyThoseThatHaveThisName == null
                    || assemblySimpleName == filterAssembliesAndRetainOnlyThoseThatHaveThisName)
                {
                    var namespacesToLookInto = new List<string>();

                    // If the namespace is a XML namespace (eg. "{http://schemas.microsoft.com/winfx/2006/xaml/presentation}"), we should iterate through all the corresponding CLR namespaces:
                    if (IsNamespaceAnXmlNamespace(namespaceName))
                        namespacesToLookInto.AddRange(
                            GetClrNamespacesFromXmlNamespace(assemblySimpleName, namespaceName));
                    else
                        namespacesToLookInto.Add(namespaceName);

                    // Search for the type:
                    foreach (var namespaceToLookInto in namespacesToLookInto)
                    {
                        var fullTypeNameToFind = string.IsNullOrEmpty(namespaceToLookInto) ? localTypeName : $"{namespaceToLookInto}.{localTypeName}";

                        var typeIfFound =
                            assembly.MainModule.Types.FirstOrDefault(x => x.FullName == fullTypeNameToFind);
                        if (typeIfFound == null)
                        {
                            //try to find a matching nested type.
                            var containerType = assembly.MainModule.Types.FirstOrDefault(x => x.FullName == namespaceToLookInto);
                            typeIfFound = containerType?.NestedTypes.FirstOrDefault(x => x.Name == localTypeName);
                        }

                        if (typeIfFound != null)
                        {
                            _typeNameToType[fullTypeNameWithNamespaceInsideBraces] = typeIfFound;
                            return typeIfFound;
                        }
                    }
                }
            }

            if (doNotRaiseExceptionIfNotFound)
                return null;
            throw new Exception("Type not found: " + fullTypeNameWithNamespaceInsideBraces);
        }

        private static bool IsNamespaceAnXmlNamespace(string namespaceName)
        {
            return namespaceName.StartsWith("http://"); //todo: are there other conditions possible for XML namespaces declared with xmlnsDefinitionAttribute?
        }

        private IEnumerable<string> GetClrNamespacesFromXmlNamespace(string assemblySimpleName, string xmlNamespace)
        {
            // Note: This method returns an empty enumeration if no result was found.
            if (!_storage.AssemblyNameToXmlNamespaceToClrNamespaces.ContainsKey(assemblySimpleName))
                return Enumerable.Empty<string>();

            var xmlNamespaceToClrNamespaces = _storage.AssemblyNameToXmlNamespaceToClrNamespaces[assemblySimpleName];
            return xmlNamespaceToClrNamespaces.ContainsKey(xmlNamespace)
                ? xmlNamespaceToClrNamespaces[xmlNamespace]
                : Enumerable.Empty<string>();
        }

        private IMemberDefinition GetMemberInfo(string memberName, string namespaceName, string localTypeName,
            string assemblyNameIfAny = null, bool returnNullIfNotFoundInsteadOfException = false)
        {
            var elementType = FindType(namespaceName, localTypeName, assemblyNameIfAny);
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

            throw new XamlParseException("Member \"" + memberName + "\" not found in type \"" + elementType);
        }

        private static PropertyDefinition FindPropertyDeep(TypeDefinition elementType, string propertyName, out TypeReference ownerElementType)
        {
            ownerElementType = elementType;
            while (ownerElementType != null)
            {
                var resolved = ownerElementType.ResolveOrThrow();
                var propertyDefinition = resolved.Properties.FirstOrDefault(p => p.Name == propertyName);
                if (propertyDefinition != null) return propertyDefinition;

                ownerElementType = resolved.BaseType;
            }

            return null;
        }

        private static FieldDefinition FindFieldDeep(TypeDefinition elementType, string propertyName,
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

                ownerElementType = resolved.BaseType;
            }

            return null;
        }

        private static MethodDefinition FindMethodDeep(TypeDefinition elementType, string methodName, out TypeReference ownerElementType)
        {
            ownerElementType = elementType;
            while (ownerElementType != null)
            {
                var resolved = ownerElementType.ResolveOrThrow();
                var methodInfo = FindMethod(resolved, methodName);
                if (methodInfo != null)
                {
                    return methodInfo;
                }
                ownerElementType = resolved.BaseType;
            }

            return null;
        }

        private TypeReference GetPropertyOrFieldType(string propertyName, string namespaceName, string localTypeName,
            string assemblyNameIfAny = null, bool isAttached = false)
        {
            if (isAttached)
                return GetMethodReturnValueType(GetPrefix + propertyName, namespaceName, localTypeName,
                    assemblyNameIfAny);

            var elementType = FindType(namespaceName, localTypeName, assemblyNameIfAny);
            var propertyInfo = FindPropertyDeep(elementType, propertyName, out var ownerElementType);

            if (propertyInfo == null)
            {
                var fieldInfo = FindFieldDeep(elementType, propertyName, out var fieldOwnerElementType);
                if (fieldInfo == null)
                    throw new XamlParseException("Property or field \"" + propertyName + "\" not found in type \"" +
                                                 elementType + "\".");

                var fieldType = fieldInfo.FieldType;
                return fieldType.PopulateGeneric(elementType, fieldOwnerElementType);
            }

            var propertyType = propertyInfo.PropertyType;
            var returnType = propertyType.PopulateGeneric(elementType, ownerElementType);
            return returnType;
        }

        private TypeReference GetMethodReturnValueType(string methodName, string namespaceName, string localTypeName,
            string assemblyNameIfAny = null)
        {
            var elementType = FindType(namespaceName, localTypeName, assemblyNameIfAny);
            var methodInfo = FindMethodDeep(elementType, methodName, out var ownerElementType);

            if (methodInfo == null)
            {
                throw new XamlParseException("Method \"" + methodName + "\" not found in type \"" +
                                             elementType + "\".");
            }

            return methodInfo.ReturnType.PopulateGeneric(elementType, ownerElementType);
        }

        private bool IsCollection(TypeDefinition elementType)
        {
            var iList = FindType(typeof(IList).Namespace, nameof(IList));
            return iList.IsAssignableFrom(elementType);
        }

        private bool IsDictionary(TypeDefinition elementType)
        {
            var iDictionary = FindType(typeof(IDictionary).Namespace, nameof(IDictionary));
            return iDictionary.IsAssignableFrom(elementType);
        }

        private bool IsElementACollection(string elementNameSpace, string elementLocalName, string assemblyNameIfAny)
        {
            var elementType = FindType(elementNameSpace, elementLocalName, assemblyNameIfAny);
            return IsCollection(elementType);
        }

        private bool IsDictionary(string elementNameSpace, string elementLocalName, string assemblyNameIfAny)
        {
            var elementType = FindType(elementNameSpace, elementLocalName, assemblyNameIfAny);
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

        private TypeDefinition GetDependencyObjectType()
        {
            return FindType(_metadata.SystemWindowsNS, DependencyObj, Constants.NAME_OF_CORE_ASSEMBLY_USING_BLAZOR);
        }

        public HashSet<string> LoadAssembly(string assemblyPath, bool loadReferencedAssembliesToo = false,
            bool skipReadingAttributesFromAssemblies = false)
        {
            return _storage.LoadAssembly(assemblyPath, loadReferencedAssembliesToo, skipReadingAttributesFromAssemblies);
        }

        public string GetCSharpEquivalentOfXamlTypeAsString(string namespaceName, string localTypeName,
            string assemblyNameIfAny = null, bool ifTypeNotFoundTryGuessing = false)
        {
            // Distinguish between system types (String, Double...) and other types
            if (SystemTypesHelper.IsSupportedSystemType($"{namespaceName}.{localTypeName}", assemblyNameIfAny))
                return SystemTypesHelper.GetFullTypeName(namespaceName, localTypeName, assemblyNameIfAny);

            // Find the type:
            var type = FindType(
                namespaceName, localTypeName, assemblyNameIfAny, ifTypeNotFoundTryGuessing
            );

            if (type != null)
                // Use information from the type
                return $"{GlobalPrefix}{type}";

            if (ifTypeNotFoundTryGuessing)
            {
                // Try guessing
                if (IsNamespaceAnXmlNamespace(namespaceName))
                    // Attempt to find the type in the current namespace
                    return localTypeName;

                return
                    $"{GlobalPrefix}{namespaceName}{(string.IsNullOrEmpty(namespaceName) ? string.Empty : ".")}{localTypeName}";
            }

            throw new XamlParseException(
                $"Type '{localTypeName}' not found in namespace '{namespaceName}'."
            );
        }

        public string GetAssemblyQualifiedNameOfXamlType(
            string namespaceName,
            string localTypeName,
            string assemblyNameIfAny)
        {
            var type = FindType(namespaceName, localTypeName, assemblyNameIfAny);

            if (type != null)
            {
                return type.ConvertToString() + ", " + type.Module.Assembly.Name.Name;
            }

            return null;
        }

        public bool IsAssignableFrom(string namespaceName, string typeName, string fromNamespaceName,
            string fromTypeName)
        {
            var type = FindType(namespaceName, typeName);
            var fromType = FindType(fromNamespaceName, fromTypeName);

            return type.IsAssignableFrom(fromType);
        }

        public MemberTypes GetMemberType(string memberName, string namespaceName, string localTypeName,
            string assemblyNameIfAny = null)
        {
            var memberInfo = GetMemberInfo(memberName, namespaceName, localTypeName, assemblyNameIfAny);
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

        public void GetPropertyOrFieldTypeInfo(string propertyOrFieldName, string namespaceName, string localTypeName,
            out string propertyNamespaceName, out string propertyLocalTypeName, out string propertyAssemblyName,
            out bool isTypeString, out bool isTypeEnum, string assemblyNameIfAny = null, bool isAttached = false)
        {
            var typeRef = GetPropertyOrFieldType(propertyOrFieldName, namespaceName, localTypeName,
                assemblyNameIfAny, isAttached);
            propertyNamespaceName = typeRef.BuildFullPath();
            propertyLocalTypeName = typeRef.GetTypeNameIncludingGenericArguments(false);
            propertyAssemblyName = typeRef.ResolveOrThrow().Module.Assembly.Name.Name;
            isTypeString = typeRef.IsString();
            isTypeEnum = typeRef.ResolveOrThrow().IsEnum;
        }

        public void GetMethodReturnValueTypeInfo(string methodName, string namespaceName, string localTypeName,
            out string returnValueNamespaceName, out string returnValueLocalTypeName,
            out string returnValueAssemblyName, out bool isTypeString, out bool isTypeEnum,
            string assemblyNameIfAny = null)
        {
            var typeDef = GetMethodReturnValueType(methodName, namespaceName, localTypeName, assemblyNameIfAny);
            returnValueNamespaceName = typeDef.BuildFullPath();
            returnValueLocalTypeName = typeDef.GetTypeNameIncludingGenericArguments(false);
            returnValueAssemblyName = typeDef.ResolveOrThrow().Module.Assembly.Name.Name;
            isTypeString = typeDef.IsString();
            isTypeEnum = typeDef.ResolveOrThrow().IsEnum;
        }

        public string GetEventHandlerType(string eventName, string namespaceName, string typeName, string assemblyName)
        {
            var type = FindType(namespaceName, typeName, assemblyName);
            var typeIterator = type;
            while (typeIterator != null)
            {
                var eventInfo = typeIterator.Events.FirstOrDefault(n => n.Name == eventName);
                if (eventInfo != null)
                {
                    var eventType = eventInfo.EventType.PopulateGeneric(type, typeIterator);
                    return eventType.GetTypeNameIncludingGenericArguments(true);
                }

                typeIterator = typeIterator.BaseType?.ResolveOrThrow();
            }

            throw new XamlParseException($"'{type}' does not contain an event named '{eventName}'.");
        }

        public bool IsElementAMarkupExtension(string elementNameSpace, string elementLocalName,
            string assemblyNameIfAny)
        {
            var elementType = FindType(elementNameSpace, elementLocalName, assemblyNameIfAny);

            var markupExtensionGeneric = FindType(SystemXamlNamespace, GenericMarkupExtension);

            var isAssignableFrom = markupExtensionGeneric.IsAssignableFrom(elementType);
            var typeIsAMarkupExtension = isAssignableFrom && !elementType.IsString();
            return typeIsAMarkupExtension;
        }

        public bool IsTypeAssignableFrom(string nameSpaceOfTypeToAssignFrom, string nameOfTypeToAssignFrom,
            string assemblyNameOfTypeToAssignFrom, string nameSpaceOfTypeToAssignTo, string nameOfTypeToAssignTo,
            string assemblyNameOfTypeToAssignTo, bool isAttached = false)
        {
            TypeDefinition typeOfElementToAssignFrom;
            TypeDefinition typeOfElementToAssignTo;

            var indexOfLastDot = nameOfTypeToAssignFrom.LastIndexOf('.');

            if (indexOfLastDot == -1)
            {
                typeOfElementToAssignFrom = FindType(nameSpaceOfTypeToAssignFrom, nameOfTypeToAssignFrom,
                    assemblyNameOfTypeToAssignFrom);
            }
            else
            {
                var localTypeName = nameOfTypeToAssignFrom.Substring(0, indexOfLastDot);
                var propertyName = nameOfTypeToAssignFrom.Substring(indexOfLastDot + 1);
                typeOfElementToAssignFrom = GetPropertyOrFieldType(propertyName, nameSpaceOfTypeToAssignFrom,
                    localTypeName, assemblyNameOfTypeToAssignFrom).ResolveOrThrow();
            }

            indexOfLastDot = nameOfTypeToAssignTo.LastIndexOf('.');
            if (indexOfLastDot == -1)
            {
                typeOfElementToAssignTo = FindType(nameSpaceOfTypeToAssignTo, nameOfTypeToAssignTo,
                    assemblyNameOfTypeToAssignTo);
            }
            else
            {
                var localTypeName = nameOfTypeToAssignTo.Substring(0, indexOfLastDot);
                var propertyName = nameOfTypeToAssignTo.Substring(indexOfLastDot + 1);
                typeOfElementToAssignTo = GetPropertyOrFieldType(propertyName, nameSpaceOfTypeToAssignTo, localTypeName,
                    assemblyNameOfTypeToAssignTo, isAttached).ResolveOrThrow();
            }

            return typeOfElementToAssignTo.IsAssignableFrom(typeOfElementToAssignFrom);
        }

        public string GetContentPropertyName(string namespaceName, string localTypeName,
            string assemblyNameIfAny = null)
        {
            var type = FindType(namespaceName, localTypeName, assemblyNameIfAny);

            // Get instance of the attribute:
            var contentPropertyAttr = GetCustomAttributeDeep(type, ContentPropertyAttributeFullName);

            if (contentPropertyAttr == null &&
                !IsElementACollection(namespaceName, localTypeName, assemblyNameIfAny) &&
                !IsDictionary(namespaceName, localTypeName, assemblyNameIfAny))
                //if the element is a collection, it is possible to add the children directly to this element.
                throw new XamlParseException("No default content property exists for element: " + localTypeName);

            if (contentPropertyAttr == null)
                return null;

            var value = contentPropertyAttr.ConstructorArguments[0].Value.ToString();

            if (string.IsNullOrEmpty(value))
                throw new Exception("The ContentPropertyAttribute must have a non-empty Name.");

            return value;
        }

        public bool IsTypeAnEnum(string namespaceName, string localTypeName, string assemblyNameIfAny = null)
        {
            var elementType = FindType(namespaceName, localTypeName, assemblyNameIfAny);
            return elementType.IsEnum;
        }

        public static MethodDefinition FindMethod(TypeDefinition td, string methodName, bool onlyPublic = false,
            bool onlyStatic = false)
        {
            return td.Methods.FirstOrDefault(m =>
                m.Name == methodName && (!onlyPublic || m.IsPublic) && (!onlyStatic || m.IsStatic));
        }

        public bool IsPropertyAttached(string propertyOrFieldName, string declaringTypeNamespaceName,
            string declaringTypeLocalName, string parentNamespaceName, string parentLocalTypeName,
            string parentAssemblyNameIfAny = null)
        {
            var elementType = FindType(declaringTypeNamespaceName, declaringTypeLocalName, parentAssemblyNameIfAny);

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

        public bool IsPropertyOrFieldACollection(string propertyOrFieldName, string namespaceName,
            string localTypeName, string assemblyNameIfAny = null)
        {
            var propertyOrFieldType = GetPropertyOrFieldType(propertyOrFieldName, namespaceName,
                localTypeName, assemblyNameIfAny);
            return IsCollection(propertyOrFieldType.ResolveOrThrow())
                   || IsDictionary(propertyOrFieldType.ResolveOrThrow());
        }

        public bool IsPropertyOrFieldADictionary(string propertyName, string namespaceName,
            string localTypeName, string assemblyNameIfAny = null)
        {
            var propertyOrFieldType = GetPropertyOrFieldType(propertyName, namespaceName, localTypeName,
                assemblyNameIfAny);
            return IsDictionary(propertyOrFieldType.ResolveOrThrow());
        }

        public XName GetCSharpEquivalentOfXamlTypeAsXName(string namespaceName, string localTypeName,
            string assemblyNameIfAny = null)
        {
            //todo: in this method, we assume that the alias will be global, which will be false if the user chose something else --> find the right alias.
            // Find the type:
            var type = FindType(namespaceName, localTypeName, assemblyNameIfAny);
            if (type == null)
                throw new XamlParseException(
                    $"Type \"{localTypeName}\" not found in namespace \"{namespaceName}\".");

            // Use information from the type:
            return XName.Get(type.Name, namespaceName);
        }

        public bool DoesTypeContainNameMemberOfTypeString(string namespaceName, string localTypeName,
            string assemblyNameIfAny = null)
        {
            var memberInfo = GetMemberInfo(Name, namespaceName, localTypeName, assemblyNameIfAny,
                true);
            if (memberInfo == null) return false;

            if (memberInfo is FieldDefinition fd && fd.FieldType.IsString() && fd.IsPublic && !fd.IsStatic) return true;

            if (memberInfo is PropertyDefinition pd && pd.PropertyType.IsString()) return true;

            return false;
        }

        public bool DoesMethodReturnACollection(string methodName, string typeNamespaceName, string localTypeName,
            string typeAssemblyNameIfAny = null)
        {
            var propertyType =
                GetMethodReturnValueType(methodName, typeNamespaceName, localTypeName, typeAssemblyNameIfAny);
            return IsCollection(propertyType.ResolveOrThrow())
                   || IsDictionary(propertyType.ResolveOrThrow());
        }

        public bool DoesMethodReturnADictionary(string methodName, string typeNamespaceName, string localTypeName,
            string typeAssemblyNameIfAny = null)
        {
            var propertyType =
                GetMethodReturnValueType(methodName, typeNamespaceName, localTypeName, typeAssemblyNameIfAny);
            return IsDictionary(propertyType.ResolveOrThrow());
        }

        public string GetField(string fieldName, string namespaceName, string typeName, string assemblyName)
        {
            var type = FindType(namespaceName, typeName, null, true);

            var field = FindFieldDeep(type, fieldName, out _, false, false, assemblyName != type.Module.Name);
            if (field != null &&
                (field.IsPublic || field.IsAssembly || field.IsFamilyOrAssembly))
                return $"{type.GetTypeNameIncludingGenericArguments(true)}.{field.Name}";

            return null;
        }

        public string GetKeyNameOfProperty(string namespaceName, string localTypeName, string assemblyNameIfAny,
            string propertyName)
        {
            var type = FindType(namespaceName, localTypeName, assemblyNameIfAny);

            var property = FindPropertyDeep(type, propertyName, out _);
            if (property == null) return null;

            // Look for the static dependency property field in the type and its ancestors:
            var fieldName = propertyName + PropertySuffix;
            var field = FindFieldDeep(type, fieldName, out _, true, true, true);
            if (field != null) return GlobalPrefix + type + "." + fieldName;

            return null;
        }

        public void GetPropertyOrFieldInfo(string propertyOrFieldName, string namespaceName, string localTypeName,
            out string memberDeclaringTypeName, out string memberTypeNamespace, out string memberTypeName,
            out bool isTypeString, out bool isTypeEnum, string assemblyNameIfAny = null, bool isAttached = false)
        {
            var elementType = FindType(namespaceName, localTypeName, assemblyNameIfAny);
            var propertyInfo = FindPropertyDeep(elementType, propertyOrFieldName, out var ownerElementType);
            TypeReference propertyOrFieldType;
            TypeReference propertyOrFieldDeclaringType;

            if (propertyInfo == null)
            {
                var fieldInfo = FindFieldDeep(elementType, propertyOrFieldName, out var fieldOwnerElementType);
                if (fieldInfo == null)
                    throw new XamlParseException("Property or field \"" + propertyOrFieldName +
                                                 "\" not found in type \"" + elementType + "\".");

                propertyOrFieldType = fieldInfo.FieldType.PopulateGeneric(elementType, fieldOwnerElementType);
                propertyOrFieldDeclaringType = fieldOwnerElementType;
            }
            else
            {
                propertyOrFieldType = propertyInfo.PropertyType.PopulateGeneric(elementType, ownerElementType);
                propertyOrFieldDeclaringType = ownerElementType;
            }


            memberDeclaringTypeName = propertyOrFieldDeclaringType.GetTypeNameIncludingGenericArguments(true);
            memberTypeNamespace = propertyOrFieldType.BuildFullPath();
            memberTypeName = propertyOrFieldType.GetTypeNameIncludingGenericArguments(false);
            isTypeString = propertyOrFieldType.IsString();
            isTypeEnum = propertyOrFieldType.ResolveOrThrow().IsEnum;
        }

        public void GetAttachedPropertyGetMethodInfo(string methodName, string namespaceName, string localTypeName, out string declaringTypeName, out string returnValueNamespaceName, out string returnValueLocalTypeName, out bool isTypeString, out bool isTypeEnum, string assemblyNameIfAny = null)
        {
            var dependencyObjectType = GetDependencyObjectType();

            var elementType = FindType(namespaceName, localTypeName, assemblyNameIfAny);
            TypeReference currentType = elementType;
            while (currentType != null)
            {
                var resolved = currentType.ResolveOrThrow();
                var method = resolved.Methods.FirstOrDefault(m =>
                    m.Name == methodName && m.IsStatic && m.IsPublic && m.Parameters.Count == 1 &&
                    dependencyObjectType.IsAssignableFrom(m.Parameters[0].ParameterType.ResolveOrThrow()));
                if (method != null)
                {
                    declaringTypeName = currentType.GetTypeNameIncludingGenericArguments(true);
                    returnValueNamespaceName = method.ReturnType.BuildFullPath();
                    returnValueLocalTypeName = method.ReturnType.PopulateGeneric(elementType, currentType).GetTypeNameIncludingGenericArguments(false);
                    isTypeString = method.ReturnType.IsString();
                    isTypeEnum = method.ReturnType.ResolveOrThrow().IsEnum;
                    return;
                }
                currentType = resolved.BaseType;
            }
            throw new XamlParseException("Method \"" + methodName + "\" not found in type \"" + elementType + "\".");
        }

        public bool IsElementADictionary(string elementNameSpace, string elementLocalName, string assemblyNameIfAny)
        {
            var elementType = FindType(elementNameSpace, elementLocalName, assemblyNameIfAny);
            return IsDictionary(elementType);
        }

        public string GetEnumValue(string name, string namespaceName, string enumName, string assembly, bool ignoreCase, bool allowIntegerValue)
        {
            name = name.Trim();

            var type = FindType(namespaceName, enumName, assembly)
                        ?? throw new XamlParseException($"Type '{enumName}' not found in namespace '{namespaceName}'.");

            var field = FindFieldDeep(type, name, out _, ignoreCase, true, true);
            if (field is not null)
            {
                return $"{GlobalPrefix}{type.ConvertToString()}.{field.Name}";
            }

            if (allowIntegerValue)
            {
                if (long.TryParse(name, out var l))
                {
                    return $"({GlobalPrefix}{type.ConvertToString()}){l}";
                }
                if (ulong.TryParse(name, out var ul))
                {
                    return $"({GlobalPrefix}{type.ConvertToString()}){ul}";
                }
            }

            return null;
        }
    }
}