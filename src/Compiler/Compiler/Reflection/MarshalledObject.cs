
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
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using OpenSilver.Compiler.Common;

namespace OpenSilver.Compiler
{
    public sealed class MarshalledObject : MarshalledObjectBase, IMarshalledObject
    {
        private const string ASSEMBLY_NOT_IN_LIST_OF_LOADED_ASSEMBLIES = "The specified assembly is not in the list of loaded assemblies.";

        private IMetadata _metadata;
        private bool _enableImplicitAssemblyRedirection;

        public override void Initialize(bool isSLMigration)
        {
            _metadata = isSLMigration ? new SLMetadata() : new UWPMetadata();
            _enableImplicitAssemblyRedirection = isSLMigration;
        }

        public override Type FindType(string namespaceName, string typeName, string assemblyName = null, bool doNotRaiseExceptionIfNotFound = false)
        {
            // Fix the namespace:
            if (namespaceName.StartsWith("using:", StringComparison.CurrentCultureIgnoreCase))
            {
                namespaceName = namespaceName.Substring("using:".Length);
            }
            else if (namespaceName.StartsWith("clr-namespace:", StringComparison.CurrentCultureIgnoreCase))
            {
                GettingInformationAboutXamlTypes.ParseClrNamespaceDeclaration(namespaceName, out var ns, out var assemblyNameIfAny);
                namespaceName = ns;
                if (_enableImplicitAssemblyRedirection)
                {
                    GettingInformationAboutXamlTypes.FixNamespaceForCompatibility(
                        ref assemblyNameIfAny,
                        ref namespaceName);
                }
            }

            // Note: normally in XAML there is no "global::", but we may enter this method passing a
            // C#-style namespace (cf. section that handles Binding in "GeneratingCSharpCode.cs")
            if (namespaceName.StartsWith("global::", StringComparison.CurrentCultureIgnoreCase))
            {
                namespaceName = namespaceName.Substring("global::".Length);
            }

            // Handle special cases:
            if (typeName == "StaticResource")
            {
                typeName = "StaticResourceExtension";
            }

            return base.FindType(namespaceName, typeName, assemblyName, doNotRaiseExceptionIfNotFound);
        }

        public string GetContentPropertyName(string namespaceName, string localTypeName, string assemblyNameIfAny = null)
        {
            var type = FindType(namespaceName, localTypeName, assemblyNameIfAny);

            // Get instance of the attribute:
            Type contentPropertyAttributeType = this.FindType("System.Windows.Markup", "ContentPropertyAttribute");
            var contentProperty = Attribute.GetCustomAttribute(type, contentPropertyAttributeType, true);

            if (contentProperty == null &&
                !IsElementACollection(namespaceName, localTypeName, assemblyNameIfAny) &&
                !IsElementADictionary(namespaceName, localTypeName, assemblyNameIfAny))
            {
                //if the element is a collection, it is possible to add the children directly to this element.
                throw new XamlParseException("No default content property exists for element: " + localTypeName.ToString());
            }

            if (contentProperty == null)
                return null;

            string contentPropertyName = (contentPropertyAttributeType.GetProperty("Name").GetValue(contentProperty) ?? "").ToString();

            if (string.IsNullOrEmpty(contentPropertyName))
                throw new Exception("The ContentPropertyAttribute must have a non-empty Name.");

            return contentPropertyName;
        }

        public bool DoesTypeContainNameMemberOfTypeString(string namespaceName, string localTypeName, string assemblyNameIfAny = null)
        {
            MemberInfo memberInfo;
            memberInfo = GetMemberInfo("Name", namespaceName, localTypeName, assemblyNameIfAny, returnNullIfNotFoundInsteadOfException: true);
            if (memberInfo == null)
                return false;
            if (memberInfo.MemberType == MemberTypes.Field && ((FieldInfo)memberInfo).FieldType == typeof(string) && ((FieldInfo)memberInfo).IsPublic && !((FieldInfo)memberInfo).IsStatic && !((FieldInfo)memberInfo).IsSecurityCritical)
                return true;
            if (memberInfo.MemberType == MemberTypes.Property && ((PropertyInfo)memberInfo).PropertyType == typeof(string))
                return true;
            return false;
        }

        public XName GetCSharpEquivalentOfXamlTypeAsXName(string namespaceName, string localTypeName, string assemblyNameIfAny = null, bool ifTypeNotFoundTryGuessing = false)
        {
            //todo: in this method, we assume that the alias will be global, which will be false if the user chose something else --> find the right alias.
            // Find the type:
            Type type = FindType(namespaceName, localTypeName, assemblyNameIfAny, doNotRaiseExceptionIfNotFound: ifTypeNotFoundTryGuessing);
            if (type == null)
            {
                if (ifTypeNotFoundTryGuessing)
                {
                    // Try guessing:

                    return XName.Get(localTypeName, string.IsNullOrEmpty(namespaceName) ? "" : namespaceName);
                }
                else
                {
                    throw new XamlParseException(string.Format("Type \"{0}\" not found in namespace \"{1}\".", localTypeName, namespaceName));
                }
            }
            else
            {
                // Use information from the type:
                return XName.Get(type.Name, namespaceName);
            }
        }

        public string GetCSharpEquivalentOfXamlTypeAsString(
            string namespaceName,
            string localTypeName,
            string assemblyNameIfAny = null,
            bool ifTypeNotFoundTryGuessing = false)
        {
            // todo: in this method, we assume that the alias will be global,
            // which will be false if the user chose something else --> find
            // the right alias.

            // Distinguish between system types (String, Double...) and other types
            if (SystemTypesHelper.IsSupportedSystemType($"{namespaceName}.{localTypeName}", assemblyNameIfAny))
            {
                return SystemTypesHelper.GetFullTypeName(namespaceName, localTypeName, assemblyNameIfAny);
            }
            else
            {
                // Find the type:
                Type type = FindType(
                    namespaceName, localTypeName, assemblyNameIfAny, ifTypeNotFoundTryGuessing
                );

                if (type == null)
                {
                    if (ifTypeNotFoundTryGuessing)
                    {
                        // Try guessing

                        if (isNamespaceAnXmlNamespace(namespaceName))
                        {
                            // Attempt to find the type in the current namespace
                            return localTypeName;
                        }
                        else
                        {
                            return string.Format(
                                "global::{0}{1}{2}",
                                namespaceName,
                                string.IsNullOrEmpty(namespaceName) ? string.Empty : ".",
                                localTypeName
                            );
                        }
                    }
                    else
                    {
                        throw new XamlParseException(
                            $"Type '{localTypeName}' not found in namespace '{namespaceName}'."
                        );
                    }
                }
                else
                {
                    // Use information from the type
                    return $"global::{type}";
                }
            }
        }

        public string GetAssemblyQualifiedNameOfXamlType(
            string namespaceName,
            string localTypeName,
            string assemblyNameIfAny)
        {
            Type type = FindType(namespaceName, localTypeName, assemblyNameIfAny);

            if (type != null)
            {
                // Note: we do not use 'AssemblyQualifiedName' because we do not want
                // to include the version of the assembly
                return type.FullName + ", " + type.Assembly.GetName().Name;
            }

            return null;
        }

        public Type GetCSharpEquivalentOfXamlType(string namespaceName, string localTypeName, string assemblyIfAny = null, bool ifTypeNotFoundTryGuessing = false)
        {
            Type type = FindType(namespaceName, localTypeName, assemblyIfAny, doNotRaiseExceptionIfNotFound: ifTypeNotFoundTryGuessing);
            if (type == null)
            {
                if (ifTypeNotFoundTryGuessing)
                {
                    return null;
                }
                else
                {
                    throw new XamlParseException(string.Format("Type \"{0}\" not found in namespace \"{1}\".", localTypeName, namespaceName));
                }
            }
            else
            {
                return type;
            }
        }

        public string GetKeyNameOfProperty(string namespaceName, string localTypeName, string assemblyNameIfAny, string propertyName)
        {
            Type type = FindType(namespaceName, localTypeName, assemblyNameIfAny);
            if (type.GetProperty(propertyName) != null)
            {
                // Look for the static dependency property field in the type and its ancestors:
                string fieldName = propertyName + "Property";
                while (type != null)
                {
                    if (type.GetField(fieldName, BindingFlags.Static | BindingFlags.Public) != null)
                    {
                        return "global::" + type.ToString() + "." + fieldName;
                    }
                    type = type.BaseType;
                }
            }

            return null;
        }

        public MemberTypes GetMemberType(string memberName, string namespaceName, string localTypeName, string assemblyNameIfAny = null)
        {
            MemberInfo memberInfo = GetMemberInfo(memberName, namespaceName, localTypeName, assemblyNameIfAny);
            return memberInfo.MemberType;
        }

        public bool IsPropertyOrFieldACollection(string propertyOrFieldName, string parentNamespaceName, string parentLocalTypeName, string parentAssemblyNameIfAny = null)
        {
            Type propertyOrFieldType = GetPropertyOrFieldType(propertyOrFieldName, parentNamespaceName, parentLocalTypeName, parentAssemblyNameIfAny);
            bool typeIsACollection = typeof(IList).IsAssignableFrom(propertyOrFieldType)
                || typeof(IDictionary).IsAssignableFrom(propertyOrFieldType);

            return typeIsACollection;
        }

        public bool IsPropertyOrFieldADictionary(string propertyName, string parentNamespaceName, string parentLocalTypeName, string parentAssemblyNameIfAny = null)
        {
            Type propertyOrFieldType = GetPropertyOrFieldType(propertyName, parentNamespaceName, parentLocalTypeName, parentAssemblyNameIfAny);
            bool typeIsADictionary = typeof(IDictionary).IsAssignableFrom(propertyOrFieldType);

            return typeIsADictionary;
        }

        public bool DoesMethodReturnACollection(string methodName, string typeNamespaceName, string localTypeName, string typeAssemblyNameIfAny = null)
        {
            Type propertyType = GetMethodReturnValueType(methodName, typeNamespaceName, localTypeName, typeAssemblyNameIfAny);
            bool typeIsACollection = typeof(IList).IsAssignableFrom(propertyType) ||
                typeof(IDictionary).IsAssignableFrom(propertyType);

            return typeIsACollection;
        }

        public bool DoesMethodReturnADictionary(string methodName, string typeNamespaceName, string localTypeName, string typeAssemblyNameIfAny = null)
        {
            Type propertyType = GetMethodReturnValueType(methodName, typeNamespaceName, localTypeName, typeAssemblyNameIfAny);
            bool typeIsADictionary = typeof(IDictionary).IsAssignableFrom(propertyType);

            return typeIsADictionary;
        }

        private bool IsElementACollection(string elementNameSpace, string elementLocalName, string assemblyNameIfAny)
        {
            var elementType = FindType(elementNameSpace, elementLocalName, assemblyNameIfAny);
            bool typeIsACollection = typeof(IList).IsAssignableFrom(elementType);

            return typeIsACollection;
        }

        public bool IsElementADictionary(string elementNameSpace, string elementLocalName, string assemblyNameIfAny)
        {
            var elementType = FindType(elementNameSpace, elementLocalName, assemblyNameIfAny);
            bool typeIsADictionary = typeof(IDictionary).IsAssignableFrom(elementType);

            return typeIsADictionary;
        }

        public bool IsElementAMarkupExtension(string elementNameSpace, string elementLocalName, string assemblyNameIfAny)
        {
            var elementType = FindType(elementNameSpace, elementLocalName, assemblyNameIfAny);

            Type markupExtensionGeneric = this.FindType("System.Xaml", "IMarkupExtension`1");
            Type objectType = this.FindType("System", "Object");
            Type markupExtensionOfObject = markupExtensionGeneric.MakeGenericType(objectType);

            bool typeIsAMarkupExtension = (markupExtensionOfObject.IsAssignableFrom(elementType) && elementType != typeof(string));
            return typeIsAMarkupExtension;
        }

        public bool IsElementAnUIElement(string elementNameSpace, string elementLocalName, string assemblyNameIfAny)
        {
            var elementType = FindType(elementNameSpace, elementLocalName, assemblyNameIfAny);
            Type uiElementType = FindType(_metadata.SystemWindowsNS, "UIElement", Constants.NAME_OF_CORE_ASSEMBLY_USING_BLAZOR);
            bool typeIsAMarkupExtension = (uiElementType.IsAssignableFrom(elementType) && elementType != typeof(string));
            return typeIsAMarkupExtension;
        }

        public bool IsTypeAnEnum(string namespaceName, string localTypeName, string assemblyNameIfAny = null)
        {
            var elementType = FindType(namespaceName, localTypeName, assemblyNameIfAny);
            return elementType.IsEnum;
        }

        public void GetMethodReturnValueTypeInfo(string methodName, string namespaceName, string localTypeName, out string returnValueNamespaceName, out string returnValueLocalTypeName, out string returnValueAssemblyName, out bool isTypeString, out bool isTypeEnum, string assemblyNameIfAny = null)
        {
            var type = GetMethodReturnValueType(methodName, namespaceName, localTypeName, assemblyNameIfAny);
            returnValueNamespaceName = this.BuildPropertyPathRecursively(type);
            returnValueLocalTypeName = GetTypeNameIncludingGenericArguments(type, false);
            returnValueAssemblyName = type.Assembly.GetName().Name;
            isTypeString = (type == typeof(string));
            isTypeEnum = (type.IsEnum);
        }

        private Type GetDependencyObjectType()
        {
            return FindType(_metadata.SystemWindowsNS, "DependencyObject", Constants.NAME_OF_CORE_ASSEMBLY_USING_BLAZOR);
        }

        public void GetAttachedPropertyGetMethodInfo(string methodName, string namespaceName, string localTypeName, out string declaringTypeName, out string returnValueNamespaceName, out string returnValueLocalTypeName, out bool isTypeString, out bool isTypeEnum, string assemblyNameIfAny = null)
        {
            Type dependencyObjectType = GetDependencyObjectType();

            Type elementType = FindType(namespaceName, localTypeName, assemblyNameIfAny);
            MethodInfo methodInfo = null;
            for (Type t = elementType; t != null; t = t.BaseType)
            {
                methodInfo = t.GetMethods(BindingFlags.Static | BindingFlags.Public)
                    .FirstOrDefault(m =>
                    {
                        if (m.Name != methodName)
                        {
                            return false;
                        }

                        ParameterInfo[] parameterInfos = m.GetParameters();
                        if (parameterInfos.Length != 1)
                        {
                            return false;
                        }

                        return dependencyObjectType.IsAssignableFrom(parameterInfos[0].ParameterType);
                    });

                if (methodInfo != null)
                {
                    break;
                }
            }

            if (methodInfo == null)
            {
                throw new XamlParseException("Method \"" + methodName + "\" not found in type \"" + elementType.ToString() + "\".");
            }

            declaringTypeName = GetTypeNameIncludingGenericArguments(methodInfo.DeclaringType, true);
            returnValueNamespaceName = this.BuildPropertyPathRecursively(methodInfo.ReturnType);
            returnValueLocalTypeName = GetTypeNameIncludingGenericArguments(methodInfo.ReturnType, false);
            isTypeString = methodInfo.ReturnType == typeof(string);
            isTypeEnum = methodInfo.ReturnType.IsEnum;
        }

        public void GetPropertyOrFieldTypeInfo(string propertyOrFieldName, string namespaceName, string localTypeName, out string propertyNamespaceName, out string propertyLocalTypeName, out string propertyAssemblyName, out bool isTypeString, out bool isTypeEnum, string assemblyNameIfAny = null, bool isAttached = false)
        {
            var type = GetPropertyOrFieldType(propertyOrFieldName, namespaceName, localTypeName, assemblyNameIfAny, isAttached: isAttached);
            propertyNamespaceName = this.BuildPropertyPathRecursively(type);
            propertyLocalTypeName = GetTypeNameIncludingGenericArguments(type, false);
            propertyAssemblyName = type.Assembly.GetName().Name;
            isTypeString = (type == typeof(string));
            isTypeEnum = (type.IsEnum);
        }

        public void GetPropertyOrFieldInfo(string propertyOrFieldName, string namespaceName, string localTypeName, out string memberDeclaringTypeName, out string memberTypeNamespace, out string memberTypeName, out bool isTypeString, out bool isTypeEnum, string assemblyNameIfAny = null, bool isAttached = false)
        {
            var elementType = FindType(namespaceName, localTypeName, assemblyNameIfAny);
            PropertyInfo propertyInfo;
            Type propertyOrFieldType;
            Type propertyOrFieldDeclaringType;
            try
            {
                propertyInfo = elementType.GetProperty(propertyOrFieldName);
            }
            catch (AmbiguousMatchException)
            {
                propertyInfo = this.GetPropertyLastImplementationIfMultipleMatches(propertyOrFieldName, elementType);
            }
            if (propertyInfo == null)
            {
                FieldInfo fieldInfo = elementType.GetField(propertyOrFieldName);
                if (fieldInfo == null)
                {
                    throw new XamlParseException("Property or field \"" + propertyOrFieldName + "\" not found in type \"" + elementType.ToString() + "\".");
                }
                else
                {
                    propertyOrFieldType = fieldInfo.FieldType;
                    propertyOrFieldDeclaringType = fieldInfo.DeclaringType;
                }
            }
            else
            {
                propertyOrFieldType = propertyInfo.PropertyType;
                propertyOrFieldDeclaringType = propertyInfo.DeclaringType;
            }
            memberDeclaringTypeName = GetTypeNameIncludingGenericArguments(propertyOrFieldDeclaringType, true);
            memberTypeNamespace = this.BuildPropertyPathRecursively(propertyOrFieldType);
            memberTypeName = GetTypeNameIncludingGenericArguments(propertyOrFieldType, false);
            isTypeString = (propertyOrFieldType == typeof(string));
            isTypeEnum = (propertyOrFieldType.IsEnum);
        }

        private string BuildPropertyPathRecursively(Type type)
        {
            string fullPath = string.Empty;
            Type parentType = type;
            while ((parentType = parentType.DeclaringType) != null)
            {
                if (!string.IsNullOrEmpty(fullPath))
                {
                    fullPath = "." + fullPath;
                }
                fullPath = parentType.Name + fullPath;
            }
            fullPath = type.Namespace + (!string.IsNullOrEmpty(type.Namespace) && !string.IsNullOrEmpty(fullPath) ? "." : string.Empty) + fullPath;
            return fullPath;
        }

        private static string GetTypeNameIncludingGenericArguments(Type type, bool appendNamespace)
        {
            string result = "";
            if (appendNamespace)
            {
                result += "global::";
                if (!string.IsNullOrEmpty(type.Namespace))
                {
                    result += type.Namespace + ".";
                }
            }

            result += type.Name;

            if (type.IsGenericType)
            {
                result = result.Split('`')[0];
                result += $"<{string.Join(", ", type.GenericTypeArguments.Select(x => GetTypeNameIncludingGenericArguments(x, true)))}>";
            }
            return result;
        }

        MemberInfo GetMemberInfo(string memberName, string namespaceName, string localTypeName, string assemblyNameIfAny = null, bool returnNullIfNotFoundInsteadOfException = false)
        {
            var elementType = FindType(namespaceName, localTypeName, assemblyNameIfAny);
            MemberInfo[] membersFound = elementType.GetMember(memberName);
            if (membersFound == null || membersFound.Length < 1)
            {
                if (returnNullIfNotFoundInsteadOfException)
                    return null;
                else
                    throw new XamlParseException("Member \"" + memberName + "\" not found in type \"" + elementType.ToString() + "\".");
            }
            MemberInfo memberInfo = membersFound[0];
            return memberInfo;
        }

        Type GetPropertyOrFieldType(string propertyName, string namespaceName, string localTypeName, string assemblyNameIfAny = null, bool isAttached = false)
        {
            if (isAttached)
            {
                return GetMethodReturnValueType("Get" + propertyName, namespaceName, localTypeName, assemblyNameIfAny);
            }
            else
            {
                var elementType = FindType(namespaceName, localTypeName, assemblyNameIfAny);
                PropertyInfo propertyInfo = null;
                try
                {
                    propertyInfo = elementType.GetProperty(propertyName);
                }
                catch (AmbiguousMatchException)
                {
                    propertyInfo = GetPropertyLastImplementationIfMultipleMatches(propertyName, elementType);
                }
                if (propertyInfo == null)
                {
                    FieldInfo fieldInfo = elementType.GetField(propertyName);
                    if (fieldInfo == null)
                    {
                        throw new XamlParseException("Property or field \"" + propertyName + "\" not found in type \"" + elementType.ToString() + "\".");
                    }
                    else
                    {
                        Type fieldType = fieldInfo.FieldType;
                        return fieldType;
                    }
                }
                else
                {
                    Type propertyType = propertyInfo.PropertyType;
                    return propertyType;
                }
            }
        }

        PropertyInfo GetPropertyLastImplementationIfMultipleMatches(string propertyName, Type type)
        {
            Type currentType = type;
            while (currentType != null)
            {
                foreach (PropertyInfo property in currentType.GetProperties())
                {
                    if (property.Name == propertyName)
                    {
                        return property;
                    }
                }
                currentType = currentType.BaseType;
            }
            return null;
        }

        Type GetMethodReturnValueType(string methodName, string namespaceName, string localTypeName, string assemblyNameIfAny = null)
        {
            Type elementType = FindType(namespaceName, localTypeName, assemblyNameIfAny);
            Type currentType = elementType;
            MethodInfo methodInfo = null;

            while (methodInfo == null && currentType != null)
            {
                MethodInfo[] methods = currentType.GetMethods();
                methodInfo = methods.FirstOrDefault(m => m.Name == methodName);
                currentType = currentType.BaseType;
            }

            if (methodInfo == null)
                throw new XamlParseException("Method \"" + methodName + "\" not found in type \"" + elementType.ToString() + "\".");
            Type methodType = methodInfo.ReturnType;
            return methodType;
        }

        public bool IsTypeAssignableFrom(string nameSpaceOfTypeToAssignFrom, string nameOfTypeToAssignFrom, string assemblyNameOfTypeToAssignFrom, string nameSpaceOfTypeToAssignTo, string nameOfTypeToAssignTo, string assemblyNameOfTypeToAssignTo, bool isAttached = false)
        {
            Type typeOfElementToAssignFrom;
            Type typeOfElementToAssignTo;

            int indexOfLastDot = nameOfTypeToAssignFrom.LastIndexOf('.');

            if (indexOfLastDot == -1)
            {
                typeOfElementToAssignFrom = FindType(nameSpaceOfTypeToAssignFrom, nameOfTypeToAssignFrom, assemblyNameOfTypeToAssignFrom);
            }
            else
            {
                string localTypeName = nameOfTypeToAssignFrom.Substring(0, indexOfLastDot);
                string propertyName = nameOfTypeToAssignFrom.Substring(indexOfLastDot + 1);
                typeOfElementToAssignFrom = GetPropertyOrFieldType(propertyName, nameSpaceOfTypeToAssignFrom, localTypeName, assemblyNameOfTypeToAssignFrom);
            }

            indexOfLastDot = nameOfTypeToAssignTo.LastIndexOf('.');
            if (indexOfLastDot == -1)
            {
                typeOfElementToAssignTo = FindType(nameSpaceOfTypeToAssignTo, nameOfTypeToAssignTo, assemblyNameOfTypeToAssignTo);
            }
            else
            {
                string localTypeName = nameOfTypeToAssignTo.Substring(0, indexOfLastDot);
                string propertyName = nameOfTypeToAssignTo.Substring(indexOfLastDot + 1);
                typeOfElementToAssignTo = GetPropertyOrFieldType(propertyName, nameSpaceOfTypeToAssignTo, localTypeName, assemblyNameOfTypeToAssignTo, isAttached);
            }

            return typeOfElementToAssignTo.IsAssignableFrom(typeOfElementToAssignFrom);
        }

        bool isNamespaceAnXmlNamespace(string namespaceName)
        {
            return namespaceName.StartsWith("http://"); //todo: are there other conditions possible for XML namespaces declared with xmlnsDefinitionAttribute?
        }

        public string GetFieldName(string fieldNameIgnoreCase, string namespaceName, string localTypeName, string assemblyIfAny = null)
        {
            Type type = FindType(namespaceName, localTypeName, assemblyIfAny);

            if (type == null) throw new XamlParseException($"Type '{localTypeName}' not found in namepsace '{namespaceName}'.");

            FieldInfo field;
            if (type.IsEnum)
            {
                field = type.GetField(fieldNameIgnoreCase, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Static);
                if (field == null)
                {
                    // If the field isn't found "as is", we try to interpret it as the int corresponding to a field
                    if (int.TryParse(fieldNameIgnoreCase, out int value))
                    {
                        string trueFieldName = Enum.GetName(type, Enum.ToObject(type, value));
                        field = type.GetField(trueFieldName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Static);
                    }
                }
            }
            else
            {
                field = type.GetField(fieldNameIgnoreCase, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Static);
            }

            return field?.Name ?? throw new XamlParseException($"Field '{fieldNameIgnoreCase}' not found in type: '{type.FullName}'.");
        }

        public bool IsPropertyAttached(string propertyOrFieldName, string declaringTypeNamespaceName, string declaringTypeLocalName, string parentNamespaceName, string parentLocalTypeName, string parentAssemblyNameIfAny = null)
        {
            Type elementType = FindType(declaringTypeNamespaceName, declaringTypeLocalName, parentAssemblyNameIfAny);
            Type currentType = elementType;
            FieldInfo fieldInfo = null;

            while (currentType != typeof(object))
            {
                fieldInfo = currentType.GetField(propertyOrFieldName + "Property"); //todo: if we somehow allow property names to be different than the name + Property, handle this case here.
                if (fieldInfo != null) break;
                currentType = currentType.BaseType;
            }

            if (fieldInfo != null)
            {
                if (fieldInfo.FieldType.Name == "DependencyProperty")
                {
                    int nbOfParameters = 2;
                    MethodInfo method = currentType.GetMethod("Set" + propertyOrFieldName, BindingFlags.Public | BindingFlags.Static);
                    if (method == null)
                    {
                        method = currentType.GetMethod("Get" + propertyOrFieldName, BindingFlags.Public | BindingFlags.Static);
                        nbOfParameters = 1;
                    }
                    if (method != null)
                    {
                        if (method.GetParameters().Length == nbOfParameters)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool IsAssignableFrom(string namespaceName, string typeName, string fromNamespaceName, string fromTypeName)
        {
            Type type = this.FindType(namespaceName, typeName);
            Type fromType = this.FindType(fromNamespaceName, fromTypeName);

            return type.IsAssignableFrom(fromType);
        }

        public string GetField(string fieldName, string namespaceName, string typeName, string assemblyName)
        {
            Type type = this.FindType(namespaceName, typeName, null, true);

            FieldInfo field;
            for (; type != null; type = type.BaseType)
            {
                var lookup = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;
                if (assemblyName == type.Assembly.GetName().Name)
                {
                    lookup |= BindingFlags.NonPublic;
                }

                if ((field = type.GetField(fieldName, lookup)) != null &&
                    (field.IsPublic || field.IsAssembly || field.IsFamilyOrAssembly))
                {
                    return $"{GetTypeNameIncludingGenericArguments(field.DeclaringType, true)}.{field.Name}";
                }
            }

            return null;
        }

        public string GetEventHandlerType(string eventName, string namespaceName, string typeName, string assemblyName)
        {
            Type type = FindType(namespaceName, typeName, assemblyName);

            EventInfo eventInfo = type.GetEvent(eventName);
            if (eventInfo == null)
            {
                throw new XamlParseException($"'{type}' does not contain an event named '{eventName}'.");
            }

            return GetTypeNameIncludingGenericArguments(eventInfo.EventHandlerType, true);
        }

        private static string GetExtension(string str)
        {
            try
            {
                return Path.GetExtension(str);
            }
            catch
            {
                //It is possible that resource does not have an extension
                return null;
            }
        }

        private static bool IsExtensionSupported(string fileName, HashSet<string> supportedExtensionsLowercase)
        {
            int lastIndexOfDot = fileName.LastIndexOf('.');
            if (lastIndexOfDot > -1)
            {
                string extension = fileName.Substring(lastIndexOfDot);
                return supportedExtensionsLowercase.Contains(extension.ToLowerInvariant());
            }
            else
            {
                return false;
            }
        }
    }
}
