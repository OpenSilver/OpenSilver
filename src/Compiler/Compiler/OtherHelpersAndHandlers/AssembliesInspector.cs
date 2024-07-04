
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
using System.Reflection;
using System.Xml.Linq;
using OpenSilver.Compiler.OtherHelpersAndHandlers.MonoCecilAssembliesInspector;

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

        public void LoadAssembly(string assemblyPath) => _monoCecilVersion.LoadAssembly(assemblyPath);

        public string GetContentPropertyName(string namespaceName, string localTypeName, string assemblyNameIfAny = null)
            => _monoCecilVersion.GetContentPropertyName(namespaceName, localTypeName, assemblyNameIfAny);

        public bool IsPropertyAttached(string propertyName, string declaringTypeNamespaceName, string declaringTypeLocalName, string parentNamespaceName, string parentLocalTypeName, string declaringTypeAssemblyIfAny = null)
            => _monoCecilVersion.IsPropertyAttached(propertyName, declaringTypeNamespaceName, declaringTypeLocalName, parentNamespaceName, parentLocalTypeName, declaringTypeAssemblyIfAny);

        public bool IsPropertyOrFieldACollection(string propertyName, string parentNamespaceName, string parentLocalTypeName, string parentAssemblyNameIfAny = null)
            => _monoCecilVersion.IsPropertyOrFieldACollection(propertyName, parentNamespaceName, parentLocalTypeName, parentAssemblyNameIfAny);

        public bool IsPropertyOrFieldADictionary(string propertyName, string parentNamespaceName, string parentLocalTypeName, string parentAssemblyNameIfAny = null)
            => _monoCecilVersion.IsPropertyOrFieldADictionary(propertyName, parentNamespaceName, parentLocalTypeName, parentAssemblyNameIfAny);

        public bool DoesMethodReturnACollection(string methodName, string typeNamespaceName, string localTypeName, string typeAssemblyNameIfAny = null)
            => _monoCecilVersion.DoesMethodReturnACollection(methodName, typeNamespaceName, localTypeName, typeAssemblyNameIfAny);

        public bool DoesMethodReturnADictionary(string methodName, string typeNamespaceName, string localTypeName, string typeAssemblyNameIfAny = null)
            => _monoCecilVersion.DoesMethodReturnADictionary(methodName, typeNamespaceName, localTypeName, typeAssemblyNameIfAny);

        public bool IsElementADictionary(string parentNamespaceName, string parentLocalTypeName, string parentAssemblyNameIfAny = null)
            => _monoCecilVersion.IsElementADictionary(parentNamespaceName, parentLocalTypeName, parentAssemblyNameIfAny);

        public bool IsElementAMarkupExtension(string parentNamespaceName, string parentLocalTypeName, string parentAssemblyNameIfAny = null)
            => _monoCecilVersion.IsElementAMarkupExtension(parentNamespaceName, parentLocalTypeName, parentAssemblyNameIfAny);

        public bool IsTypeAssignableFrom(string nameSpaceOfTypeToAssignFrom, string nameOfTypeToAssignFrom, string assemblyNameOfTypeToAssignFrom, string nameSpaceOfTypeToAssignTo, string nameOfTypeToAssignTo, string assemblyNameOfTypeToAssignTo, bool isAttached = false)
            => _monoCecilVersion.IsTypeAssignableFrom(nameSpaceOfTypeToAssignFrom, nameOfTypeToAssignFrom, assemblyNameOfTypeToAssignFrom, nameSpaceOfTypeToAssignTo, nameOfTypeToAssignTo, assemblyNameOfTypeToAssignTo, isAttached);

        public string GetKeyNameOfProperty(string elementNameSpace, string elementLocalName, string assemblyNameIfAny, string propertyName)
            => _monoCecilVersion.GetKeyNameOfProperty(elementNameSpace, elementLocalName, assemblyNameIfAny, propertyName);

        public bool DoesTypeContainNameMemberOfTypeString(string namespaceName, string localTypeName, string assemblyNameIfAny = null)
            => _monoCecilVersion.DoesTypeContainNameMemberOfTypeString(namespaceName, localTypeName, assemblyNameIfAny);

        public XName GetCSharpEquivalentOfXamlTypeAsXName(string namespaceName, string localTypeName, string assemblyNameIfAny = null)
            => _monoCecilVersion.GetCSharpEquivalentOfXamlTypeAsXName(namespaceName, localTypeName, assemblyNameIfAny);

        public string GetCSharpEquivalentOfXamlTypeAsString(string namespaceName, string localTypeName, string assemblyNameIfAny = null, bool ifTypeNotFoundTryGuessing = false)
            => _monoCecilVersion.GetCSharpEquivalentOfXamlTypeAsString(namespaceName, localTypeName,
                assemblyNameIfAny, ifTypeNotFoundTryGuessing);

        public string GetAssemblyQualifiedNameOfXamlType(string namespaceName, string localTypeName, string assemblyName)
            => _monoCecilVersion.GetAssemblyQualifiedNameOfXamlType(namespaceName, localTypeName, assemblyName);

        public MemberTypes GetMemberType(string memberName, string namespaceName, string localTypeName, string assemblyNameIfAny = null)
            => _monoCecilVersion.GetMemberType(memberName, namespaceName, localTypeName, assemblyNameIfAny);

        public bool IsTypeAnEnum(string namespaceName, string localTypeName, string assemblyNameIfAny = null)
            => _monoCecilVersion.IsTypeAnEnum(namespaceName, localTypeName, assemblyNameIfAny);

        public void GetMethodReturnValueTypeInfo(string methodName, string namespaceName, string localTypeName, out string returnValueNamespaceName, out string returnValueLocalTypeName, out string returnValueAssemblyName, out bool isTypeEnum, string assemblyNameIfAny = null)
            => _monoCecilVersion.GetMethodReturnValueTypeInfo(methodName, namespaceName, localTypeName, out returnValueNamespaceName, out returnValueLocalTypeName, out returnValueAssemblyName, out isTypeEnum, assemblyNameIfAny);

        public void GetAttachedPropertyGetMethodInfo(string methodName, string namespaceName, string localTypeName, out string declaringTypeName, out string returnValueNamespaceName, out string returnValueLocalTypeName, string assemblyNameIfAny = null)
            => _monoCecilVersion.GetAttachedPropertyGetMethodInfo(methodName, namespaceName, localTypeName,
                out declaringTypeName, out returnValueNamespaceName, out returnValueLocalTypeName, assemblyNameIfAny);

        public void GetPropertyOrFieldTypeInfo(string propertyOrFieldName, string namespaceName, string localTypeName, out string propertyNamespaceName, out string propertyLocalTypeName, out string propertyAssemblyName, out bool isTypeEnum, string assemblyNameIfAny = null, bool isAttached = false)
            => _monoCecilVersion.GetPropertyOrFieldTypeInfo(propertyOrFieldName, namespaceName, localTypeName,
                out propertyNamespaceName, out propertyLocalTypeName, out propertyAssemblyName,
                out isTypeEnum, assemblyNameIfAny, isAttached: isAttached);

        public void GetPropertyOrFieldInfo(string propertyOrFieldName, string namespaceName, string localTypeName, out string memberDeclaringTypeName, out string memberTypeNamespace, out string memberTypeName, string assemblyNameIfAny = null, bool isAttached = false)
            => _monoCecilVersion.GetPropertyOrFieldInfo(propertyOrFieldName, namespaceName, localTypeName, out memberDeclaringTypeName, out memberTypeNamespace, out memberTypeName, assemblyNameIfAny, isAttached);

        public string GetEnumValue(string name, string namespaceName, string enumName, string assembly, bool ignoreCase, bool allowIntegerValue)
            => _monoCecilVersion.GetEnumValue(name, namespaceName, enumName, assembly, ignoreCase, allowIntegerValue);

        public bool IsAssignableFrom(string namespaceName, string typeName, string fromNamespaceName, string fromTypeName)
            => _monoCecilVersion.IsAssignableFrom(namespaceName, typeName, fromNamespaceName, fromTypeName);

        public bool IsFrameworkTemplateTemplateProperty(string propertyName, string namespaceName, string typeName)
            => _monoCecilVersion.IsFrameworkTemplateTemplateProperty(propertyName, namespaceName, typeName);

        public bool IsResourceDictionarySourcePropertyVisible( string namespaceName, string typeName)
            => _monoCecilVersion.IsResourceDictionarySourcePropertyVisible(namespaceName, typeName);

        public string GetField(string fieldName, string namespaceName, string typeName, string assemblyName)
            => _monoCecilVersion.GetField(fieldName, namespaceName, typeName, assemblyName);
    }
}
