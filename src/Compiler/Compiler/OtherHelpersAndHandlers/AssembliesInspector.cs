
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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace OpenSilver.Compiler
{
    public class AssembliesInspector : IDisposable
    {
        private readonly MonoCecilAssembliesInspectorImpl _monoCecilVersion;

        public AssembliesInspector(SupportedLanguage compilerType)
        {
            _monoCecilVersion = new MonoCecilAssembliesInspectorImpl(compilerType);
        }

        public void Dispose() => _monoCecilVersion.Dispose();

        public AssemblyDefinition LoadAssembly(string assemblyPath) => _monoCecilVersion.LoadAssembly(assemblyPath);

        public AssemblyDefinition LoadAssembly(Stream stream) => _monoCecilVersion.LoadAssembly(stream);

        public void UnloadAssembly(AssemblyDefinition assembly) => _monoCecilVersion.UnloadAssembly(assembly);

        public string GetContentPropertyName(string namespaceName, string localTypeName, string assemblyNameIfAny, IXmlLineInfo lineInfo)
            => _monoCecilVersion.GetContentPropertyName(namespaceName, localTypeName, assemblyNameIfAny, lineInfo);

        public bool IsPropertyAttached(string propertyName, string namespaceName, string typeName, string assemblyName, IXmlLineInfo lineInfo)
            => _monoCecilVersion.IsPropertyAttached(propertyName, namespaceName, typeName, assemblyName, lineInfo);

        public bool IsPropertyOrFieldACollection(string propertyName, string namespaceName, string typeName, string assemblyName, IXmlLineInfo lineInfo)
            => _monoCecilVersion.IsPropertyOrFieldACollection(propertyName, namespaceName, typeName, assemblyName, lineInfo);

        public bool IsPropertyOrFieldADictionary(string propertyName, string namespaceName, string typeName, string assemblyName, IXmlLineInfo lineInfo)
            => _monoCecilVersion.IsPropertyOrFieldADictionary(propertyName, namespaceName, typeName, assemblyName, lineInfo);

        public bool DoesMethodReturnACollection(string methodName, string typeNamespaceName, string localTypeName, string assemblyName, IXmlLineInfo lineInfo)
            => _monoCecilVersion.DoesMethodReturnACollection(methodName, typeNamespaceName, localTypeName, assemblyName, lineInfo);

        public bool DoesMethodReturnADictionary(string methodName, string typeNamespaceName, string localTypeName, string assemblyName, IXmlLineInfo lineInfo)
            => _monoCecilVersion.DoesMethodReturnADictionary(methodName, typeNamespaceName, localTypeName, assemblyName, lineInfo);

        public bool IsElementADictionary(string namespaceName, string typeName, string assemblyName, IXmlLineInfo lineInfo)
            => _monoCecilVersion.IsElementADictionary(namespaceName, typeName, assemblyName, lineInfo);

        public bool IsElementAMarkupExtension(string namespaceName, string typeName, string assemblyName, IXmlLineInfo lineInfo)
            => _monoCecilVersion.IsElementAMarkupExtension(namespaceName, typeName, assemblyName, lineInfo);

        public bool IsTypeAssignableFrom(string nameSpaceOfTypeToAssignFrom, string nameOfTypeToAssignFrom, string assemblyNameOfTypeToAssignFrom, string nameSpaceOfTypeToAssignTo, string nameOfTypeToAssignTo, string assemblyNameOfTypeToAssignTo, IXmlLineInfo lineInfo, bool isAttached = false)
            => _monoCecilVersion.IsTypeAssignableFrom(nameSpaceOfTypeToAssignFrom, nameOfTypeToAssignFrom, assemblyNameOfTypeToAssignFrom, nameSpaceOfTypeToAssignTo, nameOfTypeToAssignTo, assemblyNameOfTypeToAssignTo, lineInfo, isAttached);

        public bool DoesTypeContainNameMemberOfTypeString(string namespaceName, string localTypeName, string assemblyNameIfAny, IXmlLineInfo lineInfo)
            => _monoCecilVersion.DoesTypeContainNameMemberOfTypeString(namespaceName, localTypeName, assemblyNameIfAny, lineInfo);

        public XName GetCSharpEquivalentOfXamlTypeAsXName(string namespaceName, string localTypeName, string assemblyNameIfAny, IXmlLineInfo lineInfo)
            => _monoCecilVersion.GetCSharpEquivalentOfXamlTypeAsXName(namespaceName, localTypeName, assemblyNameIfAny, lineInfo);

        public string GetCSharpEquivalentOfXamlTypeAsString(string namespaceName, string localTypeName, string assemblyNameIfAny,
            IXmlLineInfo lineInfo, bool ifTypeNotFoundTryGuessing = false)
            => _monoCecilVersion.GetCSharpEquivalentOfXamlTypeAsString(namespaceName, localTypeName, assemblyNameIfAny, lineInfo, ifTypeNotFoundTryGuessing);

        public string GetAssemblyQualifiedNameOfXamlType(string namespaceName, string localTypeName, string assemblyName, IXmlLineInfo lineInfo)
            => _monoCecilVersion.GetAssemblyQualifiedNameOfXamlType(namespaceName, localTypeName, assemblyName, lineInfo);

        public MemberTypes GetMemberType(string memberName, string namespaceName, string localTypeName, string assemblyNameIfAny, IXmlLineInfo lineInfo)
            => _monoCecilVersion.GetMemberType(memberName, namespaceName, localTypeName, assemblyNameIfAny, lineInfo);

        public (MemberTypes Type, MethodDefinition Method, TypeReference DeclaringType) GetAttachedMemberType(string memberName, string ownerTypeNamespace, string ownerTypeName, string ownerTypeAssemblyName, IXmlLineInfo lineInfo)
            => _monoCecilVersion.GetAttachedMemberType(memberName, ownerTypeNamespace, ownerTypeName, ownerTypeAssemblyName, lineInfo);

        public bool IsTypeAnEnum(string namespaceName, string typeName, string assemblyName, IXmlLineInfo lineInfo)
            => _monoCecilVersion.IsTypeAnEnum(namespaceName, typeName, assemblyName, lineInfo);

        public void GetMethodReturnValueTypeInfo(string methodName, string namespaceName, string localTypeName, string assemblyNameIfAny, IXmlLineInfo lineInfo, out string returnValueNamespaceName, out string returnValueLocalTypeName, out string returnValueAssemblyName, out bool isTypeEnum)
            => _monoCecilVersion.GetMethodReturnValueTypeInfo(methodName, namespaceName, localTypeName, assemblyNameIfAny, lineInfo, out returnValueNamespaceName, out returnValueLocalTypeName, out returnValueAssemblyName, out isTypeEnum);

        public void GetAttachedPropertyGetMethodInfo(string methodName, string namespaceName, string localTypeName, string assemblyNameIfAny, IXmlLineInfo lineInfo, out string declaringTypeName, out string returnValueNamespaceName, out string returnValueLocalTypeName)
            => _monoCecilVersion.GetAttachedPropertyGetMethodInfo(methodName, namespaceName, localTypeName, assemblyNameIfAny, lineInfo,
                out declaringTypeName, out returnValueNamespaceName, out returnValueLocalTypeName);

        public void GetPropertyOrFieldTypeInfo(string propertyOrFieldName, string namespaceName, string localTypeName, string assemblyNameIfAny, IXmlLineInfo lineInfo, out string propertyNamespaceName, out string propertyLocalTypeName, out string propertyAssemblyName, out bool isTypeEnum, bool isAttached = false)
            => _monoCecilVersion.GetPropertyOrFieldTypeInfo(propertyOrFieldName, namespaceName, localTypeName, assemblyNameIfAny, lineInfo,
                out propertyNamespaceName, out propertyLocalTypeName, out propertyAssemblyName,
                out isTypeEnum, isAttached: isAttached);

        public void GetPropertyOrFieldTypeInfo(string propertyOrFieldName, string namespaceName, string localTypeName, string assemblyNameIfAny, IXmlLineInfo lineInfo, out string propertyNamespaceName, out string propertyLocalTypeName, out string propertyAssemblyName, out bool isTypeEnum, out bool hasTypeConverter, bool isAttached = false)
            => _monoCecilVersion.GetPropertyOrFieldTypeInfo(propertyOrFieldName, namespaceName, localTypeName, assemblyNameIfAny, lineInfo,
                out propertyNamespaceName, out propertyLocalTypeName, out propertyAssemblyName,
                out isTypeEnum, out hasTypeConverter, isAttached: isAttached);

        public void GetPropertyOrFieldInfo(string propertyOrFieldName, string namespaceName, string localTypeName, string assemblyNameIfAny, IXmlLineInfo lineInfo, out string memberDeclaringTypeName, out string memberTypeNamespace, out string memberTypeName)
            => _monoCecilVersion.GetPropertyOrFieldInfo(propertyOrFieldName, namespaceName, localTypeName, assemblyNameIfAny, lineInfo, out memberDeclaringTypeName, out memberTypeNamespace, out memberTypeName);

        public bool IsAssignableFrom(string namespaceName, string typeName, string fromNamespaceName, string fromTypeName, IXmlLineInfo lineInfo)
            => _monoCecilVersion.IsAssignableFrom(namespaceName, typeName, fromNamespaceName, fromTypeName, lineInfo);

        public bool IsFrameworkTemplateTemplateProperty(string propertyName, string namespaceName, string typeName, IXmlLineInfo lineInfo)
            => _monoCecilVersion.IsFrameworkTemplateTemplateProperty(propertyName, namespaceName, typeName, lineInfo);

        public bool IsResourceDictionarySourcePropertyVisible(string namespaceName, string typeName, IXmlLineInfo lineInfo)
            => _monoCecilVersion.IsResourceDictionarySourcePropertyVisible(namespaceName, typeName, lineInfo);

        public string GetField(string fieldName, string namespaceName, string typeName, string assemblyName, IXmlLineInfo lineInfo)
            => _monoCecilVersion.GetField(fieldName, namespaceName, typeName, assemblyName, lineInfo);

        public string GetProperty(string fieldName, string namespaceName, string typeName, string assemblyName, IXmlLineInfo lineInfo)
            => _monoCecilVersion.GetProperty(fieldName, namespaceName, typeName, assemblyName, lineInfo);

        public TypeDefinition GetTypeDefinition(string namespaceName, string typeName, string assemblyName, IXmlLineInfo lineInfo, bool throwIfNull = true)
            => _monoCecilVersion.FindType(namespaceName, typeName, assemblyName, lineInfo, !throwIfNull);

        public string GetEnumValue(TypeDefinition enumType, string name, bool ignoreCase, bool allowIntegerValue)
            => _monoCecilVersion.GetEnumValue(enumType, name, ignoreCase, allowIntegerValue);

        public IEnumerable<string> GetEnumValues(TypeDefinition enumType, string name, bool ignoreCase, bool allowIntegerValue, IXmlLineInfo lineInfo)
            => _monoCecilVersion.GetEnumValues(enumType, name, ignoreCase, allowIntegerValue, lineInfo);

        public (FieldDefinition Field, TypeReference DeclaringType) GetField(TypeDefinition type, string name, bool staticOnly, bool publicOnly)
        {
            FieldDefinition field = MonoCecilAssembliesInspectorImpl.FindFieldDeep(
                type, name, out TypeReference declaringType, false, staticOnly, publicOnly);

            return (field, declaringType);
        }

        public (PropertyDefinition Property, TypeReference DeclaringType) GetProperty(TypeDefinition type, string name, bool staticOnly, bool publicOnly)
        {
            PropertyDefinition property = MonoCecilAssembliesInspectorImpl.FindPropertyGetterDeep(
                type, name, out TypeReference declaringType, staticOnly, publicOnly);

            return (property, declaringType);
        }

        public EventDefinition GetEvent(TypeDefinition type, string eventName, bool publicOnly)
            => MonoCecilAssembliesInspectorImpl.FindEventDeep(type, eventName, out _, publicOnly);

        public MethodDefinition GetMethod(TypeDefinition type, string methodName, bool publicOnly, bool staticOnly)
            => MonoCecilAssembliesInspectorImpl.FindMethodDeep(type, methodName, publicOnly, staticOnly, out _);
    }
}
