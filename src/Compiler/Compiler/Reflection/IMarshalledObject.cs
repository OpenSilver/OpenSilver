
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
    public interface IMarshalledObject : IMarshalledObjectBase
    {
        string GetContentPropertyName(string namespaceName, string localTypeName, string assemblyNameIfAny = null);

        bool DoesTypeContainNameMemberOfTypeString(string namespaceName, string localTypeName, string assemblyNameIfAny = null);

        XName GetCSharpEquivalentOfXamlTypeAsXName(string namespaceName, string localTypeName, string assemblyNameIfAny = null, bool ifTypeNotFoundTryGuessing = false);

        string GetCSharpEquivalentOfXamlTypeAsString(string namespaceName, string localTypeName, string assemblyNameIfAny = null, bool ifTypeNotFoundTryGuessing = false);

        string GetAssemblyQualifiedNameOfXamlType(string namespaceName, string localTypeName, string assemblyName);

        Type GetCSharpEquivalentOfXamlType(string namespaceName, string localTypeName, string assemblyIfAny = null, bool ifTypeNotFoundTryGuessing = false);

        string GetKeyNameOfProperty(string namespaceName, string localTypeName, string assemblyNameIfAny, string propertyName);

        MemberTypes GetMemberType(string memberName, string namespaceName, string localTypeName, string assemblyNameIfAny = null);

        bool IsPropertyAttached(string propertyOrFieldName, string declaringTypeNamespaceName, string declaringTypeLocalName, string parentNamespaceName, string parentLocalTypeName, string parentAssemblyNameIfAny = null);

        bool IsPropertyOrFieldACollection(string propertyOrFieldName, string parentNamespaceName, string parentLocalTypeName, string parentAssemblyNameIfAny = null);

        bool IsPropertyOrFieldADictionary(string propertyOrFieldName, string parentNamespaceName, string parentLocalTypeName, string parentAssemblyNameIfAny = null);

        bool DoesMethodReturnACollection(string methodName, string typeNamespaceName, string localTypeName, string typeAssemblyNameIfAny = null);

        bool DoesMethodReturnADictionary(string methodName, string typeNamespaceName, string localTypeName, string typeAssemblyNameIfAny = null);

        bool IsElementADictionary(string elementNameSpace, string elementLocalName, string assemblyNameIfAny);

        bool IsElementAMarkupExtension(string elementNameSpace, string elementLocalName, string assemblyNameIfAny);

        bool IsElementAnUIElement(string elementNameSpace, string elementLocalName, string assemblyNameIfAny);

        bool IsTypeAnEnum(string namespaceName, string localTypeName, string assemblyNameIfAny = null);

        void GetMethodReturnValueTypeInfo(string methodName, string namespaceName, string localTypeName, out string returnValueNamespaceName, out string returnValueLocalTypeName, out string returnValueAssemblyName, out bool isTypeString, out bool isTypeEnum, string assemblyNameIfAny = null);

        void GetAttachedPropertyGetMethodInfo(string methodName, string namespaceName, string localTypeName, out string declaringTypeName, out string returnValueNamespaceName, out string returnValueLocalTypeName, out bool isTypeString, out bool isTypeEnum, string assemblyNameIfAny = null);

        void GetPropertyOrFieldTypeInfo(string propertyOrFieldName, string namespaceName, string localTypeName, out string propertyNamespaceName, out string propertyLocalTypeName, out string propertyAssemblyName, out bool isTypeString, out bool isTypeEnum, string assemblyNameIfAny = null, bool isAttached = false);

        void GetPropertyOrFieldInfo(string propertyOrFieldName, string namespaceName, string localTypeName, out string memberDeclaringTypeName, out string memberTypeNamespace, out string memberTypeName, out bool isTypeString, out bool isTypeEnum, string assemblyNameIfAny = null, bool isAttached = false);

        string GetEnumValue(string name, string namespaceName, string enumName, string assembly, bool ignoreCase, bool allowIntegerValue);

        bool IsTypeAssignableFrom(string nameSpaceOfTypeToAssignFrom, string nameOfTypeToAssignFrom, string assemblyNameOfTypeToAssignFrom, string nameSpaceOfTypeToAssignTo, string nameOfTypeToAssignTo, string assemblyNameOfTypeToAssignTo, bool isAttached = false);

        bool IsAssignableFrom(string namespaceName, string typeName, string fromNamespaceName, string fromTypeName);

        string GetField(string fieldName, string namespaceName, string typeName, string assemblyName);

        string GetEventHandlerType(string eventName, string namespaceName, string typeName, string assemblyName);
    }
}
