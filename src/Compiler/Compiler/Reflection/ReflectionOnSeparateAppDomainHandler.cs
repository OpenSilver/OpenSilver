
/*===================================================================================
* 
*   Copyright (c) Userware (OpenSilver.net, CSHTML5.com)
*      
*   This file is part of both the OpenSilver Compiler (https://opensilver.net), which
*   is licensed under the MIT license (https://opensource.org/licenses/MIT), and the
*   CSHTML5 Compiler (http://cshtml5.com), which is dual-licensed (MIT + commercial).
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System;
using System.Reflection;
using System.Xml.Linq;
using OpenSilver.Compiler.Common;

namespace OpenSilver.Compiler
{
    public sealed class ReflectionOnSeparateAppDomainHandler : ReflectionOnSeparateAppDomainHandlerBase<IMarshalledObject, MarshalledObject>
    {
        //Note: we use a new AppDomain so that we can Unload all the assemblies that we have inspected when we have done.

        //----------------------------------------------------------------------
        // We create a static instance in the "BeforeXamlPreprocessor" task.
        // The static instance avoids reloading the assemblies for each XAML file.
        // We dispose it in the "AfterXamlPreprocessor" task.
        //----------------------------------------------------------------------

        public static ReflectionOnSeparateAppDomainHandler Current;

        public ReflectionOnSeparateAppDomainHandler(bool isSLMigration)
            : base(isSLMigration)
        {
        }

        public string GetContentPropertyName(string namespaceName, string localTypeName, string assemblyNameIfAny = null)
            => MarshalledObject.GetContentPropertyName(namespaceName, localTypeName, assemblyNameIfAny);

        public bool IsPropertyAttached(string propertyName, string declaringTypeNamespaceName, string declaringTypeLocalName, string parentNamespaceName, string parentLocalTypeName, string declaringTypeAssemblyIfAny = null)
            => MarshalledObject.IsPropertyAttached(propertyName, declaringTypeNamespaceName, declaringTypeLocalName, parentNamespaceName, parentLocalTypeName, declaringTypeAssemblyIfAny);

        public bool IsPropertyOrFieldACollection(string propertyName, string parentNamespaceName, string parentLocalTypeName, string parentAssemblyNameIfAny = null)
            => MarshalledObject.IsPropertyOrFieldACollection(propertyName, parentNamespaceName, parentLocalTypeName, parentAssemblyNameIfAny);

        public bool IsPropertyOrFieldADictionary(string propertyName, string parentNamespaceName, string parentLocalTypeName, string parentAssemblyNameIfAny = null)
            => MarshalledObject.IsPropertyOrFieldADictionary(propertyName, parentNamespaceName, parentLocalTypeName, parentAssemblyNameIfAny);

        public bool DoesMethodReturnACollection(string methodName, string typeNamespaceName, string localTypeName, string typeAssemblyNameIfAny = null)
            => MarshalledObject.DoesMethodReturnACollection(methodName, typeNamespaceName, localTypeName, typeAssemblyNameIfAny);

        public bool DoesMethodReturnADictionary(string methodName, string typeNamespaceName, string localTypeName, string typeAssemblyNameIfAny = null)
            => MarshalledObject.DoesMethodReturnADictionary(methodName, typeNamespaceName, localTypeName, typeAssemblyNameIfAny);

        public bool IsElementADictionary(string parentNamespaceName, string parentLocalTypeName, string parentAssemblyNameIfAny = null)
            => MarshalledObject.IsElementADictionary(parentNamespaceName, parentLocalTypeName, parentAssemblyNameIfAny);

        public bool IsElementAMarkupExtension(string parentNamespaceName, string parentLocalTypeName, string parentAssemblyNameIfAny = null)
            => MarshalledObject.IsElementAMarkupExtension(parentNamespaceName, parentLocalTypeName, parentAssemblyNameIfAny);

        public bool IsTypeAssignableFrom(string nameSpaceOfTypeToAssignFrom, string nameOfTypeToAssignFrom, string assemblyNameOfTypeToAssignFrom, string nameSpaceOfTypeToAssignTo, string nameOfTypeToAssignTo, string assemblyNameOfTypeToAssignTo, bool isAttached = false)
            => MarshalledObject.IsTypeAssignableFrom(nameSpaceOfTypeToAssignFrom, nameOfTypeToAssignFrom, assemblyNameOfTypeToAssignFrom, nameSpaceOfTypeToAssignTo, nameOfTypeToAssignTo, assemblyNameOfTypeToAssignTo, isAttached);

        public string GetKeyNameOfProperty(string elementNameSpace, string elementLocalName, string assemblyNameIfAny, string propertyName)
            => MarshalledObject.GetKeyNameOfProperty(elementNameSpace, elementLocalName, assemblyNameIfAny, propertyName);

        public bool DoesTypeContainNameMemberOfTypeString(string namespaceName, string localTypeName, string assemblyNameIfAny = null)
            => MarshalledObject.DoesTypeContainNameMemberOfTypeString(namespaceName, localTypeName, assemblyNameIfAny);

        public XName GetCSharpEquivalentOfXamlTypeAsXName(string namespaceName, string localTypeName, string assemblyNameIfAny = null, bool ifTypeNotFoundTryGuessing = false)
            => MarshalledObject.GetCSharpEquivalentOfXamlTypeAsXName(namespaceName, localTypeName, assemblyNameIfAny, ifTypeNotFoundTryGuessing);

        public Type GetCSharpEquivalentOfXamlType(string namespaceName, string localTypeName, string assemblyIfAny = null, bool ifTypeNotFoundTryGuessing = false)
            => MarshalledObject.GetCSharpEquivalentOfXamlType(namespaceName, localTypeName, assemblyIfAny, ifTypeNotFoundTryGuessing);

        public string GetCSharpEquivalentOfXamlTypeAsString(string namespaceName, string localTypeName, string assemblyNameIfAny = null, bool ifTypeNotFoundTryGuessing = false)
            => MarshalledObject.GetCSharpEquivalentOfXamlTypeAsString(namespaceName, localTypeName, assemblyNameIfAny, ifTypeNotFoundTryGuessing);

        public string GetAssemblyQualifiedNameOfXamlType(string namespaceName, string localTypeName, string assemblyName)
            => MarshalledObject.GetAssemblyQualifiedNameOfXamlType(namespaceName, localTypeName, assemblyName);

        public MemberTypes GetMemberType(string memberName, string namespaceName, string localTypeName, string assemblyNameIfAny = null)
            => MarshalledObject.GetMemberType(memberName, namespaceName, localTypeName, assemblyNameIfAny);

        public bool IsTypeAnEnum(string namespaceName, string localTypeName, string assemblyNameIfAny = null)
            => MarshalledObject.IsTypeAnEnum(namespaceName, localTypeName, assemblyNameIfAny);

        public void GetMethodReturnValueTypeInfo(string methodName, string namespaceName, string localTypeName, out string returnValueNamespaceName, out string returnValueLocalTypeName, out string returnValueAssemblyName, out bool isTypeString, out bool isTypeEnum, string assemblyNameIfAny = null)
            => MarshalledObject.GetMethodReturnValueTypeInfo(methodName, namespaceName, localTypeName, out returnValueNamespaceName, out returnValueLocalTypeName, out returnValueAssemblyName, out isTypeString, out isTypeEnum, assemblyNameIfAny);

        public void GetAttachedPropertyGetMethodInfo(string methodName, string namespaceName, string localTypeName, out string declaringTypeName, out string returnValueNamespaceName, out string returnValueLocalTypeName, out bool isTypeString, out bool isTypeEnum, string assemblyNameIfAny = null)
            => MarshalledObject.GetAttachedPropertyGetMethodInfo(methodName, namespaceName, localTypeName, out declaringTypeName, out returnValueNamespaceName, out returnValueLocalTypeName, out isTypeString, out isTypeEnum, assemblyNameIfAny);

        public void GetPropertyOrFieldTypeInfo(string propertyOrFieldName, string namespaceName, string localTypeName, out string propertyNamespaceName, out string propertyLocalTypeName, out string propertyAssemblyName, out bool isTypeString, out bool isTypeEnum, string assemblyNameIfAny = null, bool isAttached = false)
            => MarshalledObject.GetPropertyOrFieldTypeInfo(propertyOrFieldName, namespaceName, localTypeName, out propertyNamespaceName, out propertyLocalTypeName, out propertyAssemblyName, out isTypeString, out isTypeEnum, assemblyNameIfAny, isAttached: isAttached);

        public void GetPropertyOrFieldInfo(string propertyOrFieldName, string namespaceName, string localTypeName, out string memberDeclaringTypeName, out string memberTypeNamespace, out string memberTypeName, out bool isTypeString, out bool isTypeEnum, string assemblyNameIfAny = null, bool isAttached = false)
            => MarshalledObject.GetPropertyOrFieldInfo(propertyOrFieldName, namespaceName, localTypeName, out memberDeclaringTypeName, out memberTypeNamespace, out memberTypeName, out isTypeString, out isTypeEnum, assemblyNameIfAny, isAttached);

        public string GetFieldName(string fieldNameIgnoreCase, string namespaceName, string localTypeName, string assemblyIfAny = null)
            => MarshalledObject.GetFieldName(fieldNameIgnoreCase, namespaceName, localTypeName, assemblyIfAny);

        public bool IsAssignableFrom(string namespaceName, string typeName, string fromNamespaceName, string fromTypeName)
            => MarshalledObject.IsAssignableFrom(namespaceName, typeName, fromNamespaceName, fromTypeName);

        public string GetField(string fieldName, string namespaceName, string typeName, string assemblyName)
            => MarshalledObject.GetField(fieldName, namespaceName, typeName, assemblyName);

        public string GetEventHandlerType(string eventName, string namespaceName, string typeName, string assemblyName)
            => MarshalledObject.GetEventHandlerType(eventName, namespaceName, typeName, assemblyName);
    }
}
