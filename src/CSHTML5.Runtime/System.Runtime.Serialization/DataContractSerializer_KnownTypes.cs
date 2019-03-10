
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace System.Runtime.Serialization
{
    internal static class DataContractSerializer_KnownTypes
    {
        internal static bool CheckIfItIsAKnownType(object obj, Type objectType, IReadOnlyList<Type> knownTypes, Type memberType)
        {
            // Search in the list of known types passed to the DataContractSerializer:
            if (knownTypes.Contains(memberType))
            {
                return true;
            }

            // Search in the [KnownType] attribute(s) of the class:
            foreach (Type knownType in GetKnownTypesByReadingKnownTypeAttributes(objectType))
            {
                if (knownType == memberType)
                {
                    return true;
                }
            }

            return false;
        }

        internal static Type GetCSharpTypeForNode(XElement xElement, Type parentType, Type memberType, IReadOnlyList<Type> knownTypes, MemberInformation parentMemberInformationIfNotRoot)
        {
            // Look for the XML attribute named "type":
            XAttribute xmlTypeAttribute = null;
            foreach (XAttribute attribute in xElement.Attributes(XNamespace.Get(DataContractSerializer_Helpers.XMLSCHEMA_NAMESPACE).GetName("type")))
            {
                xmlTypeAttribute = attribute;
                break;
            }

            //todo: if no "type" attribute was found, verify that the expected type ("memberType") has the same name and namespace of the XElement. If not, raise a SerializationException that says something like: Expecting element 'MainPage.Person' from namespace 'http://schemas.datacontract.org/2004/07/TestSilverlightDataContractSerializer1'. Encountered 'Element' with name 'MainPage.Person', namespace 'http://schemas.datacontract.org/2004/07/TestCshtml5DataContractSerializer1'.

            // If found, read it and process it:
            string typeNameInXmlTypeAttribute = null;
            string typeNamespaceInXmlTypeAttribute = null;
            if (xmlTypeAttribute != null)
            {
                GetNameAndNamespaceFromXmlTypeAttribute(xmlTypeAttribute, xElement, out typeNameInXmlTypeAttribute, out typeNamespaceInXmlTypeAttribute);
            }

            // If no type information was found, but the return type is "object", it means that we should consider the name of the XElement:
            if (string.IsNullOrWhiteSpace(typeNameInXmlTypeAttribute)
                && memberType == typeof(object))
            {
                typeNameInXmlTypeAttribute = xElement.Name.LocalName;
                typeNamespaceInXmlTypeAttribute = (xElement.Name.Namespace != null ? xElement.Name.Namespace.NamespaceName : null);
                if (string.IsNullOrEmpty(typeNamespaceInXmlTypeAttribute))
                {
                    typeNamespaceInXmlTypeAttribute = xElement.GetDefaultNamespace().NamespaceName;
                }
            }

            // Look for a C# type that has the name and namespace specified by that "type" attribute:
            if (!string.IsNullOrWhiteSpace(typeNamespaceInXmlTypeAttribute) && !string.IsNullOrWhiteSpace(typeNameInXmlTypeAttribute))
            {
                // Check if the type specified by the "type" attribute is the same as the expected member type:
                if (IsTypeSameAsTheOneSpecifiedInXmlTypeAttribute(memberType, typeNameInXmlTypeAttribute, typeNamespaceInXmlTypeAttribute))
                {
                    //----------------------------
                    // The actual type is same as the memberActualType.
                    //----------------------------

                    // We found the type, so there is nothing to do.
                }
                // Check if the type specified by the "type" attribute is a built-in type:
                else if ((typeNamespaceInXmlTypeAttribute == DataContractSerializer_Helpers.XMLSCHEMA_NAMESPACE
                        || typeNamespaceInXmlTypeAttribute == DataContractSerializer_Helpers.XMLSCHEMA_NAMESPACE_XSD
                        || typeNamespaceInXmlTypeAttribute == DataContractSerializer_Helpers.XMLSCHEMA_NAMESPACE_TYPES)
                        && DataContractSerializer_ValueTypesHandler.NamesToTypes.ContainsKey(typeNameInXmlTypeAttribute))
                {
                    //----------------------------
                    // It is a built-in type:
                    //----------------------------

                    memberType = DataContractSerializer_ValueTypesHandler.NamesToTypes[typeNameInXmlTypeAttribute];
                }
                // Look in the KnownTypes:
                else
                {
                    //----------------------------
                    // We try to find the type with the specified name and namespace
                    // by looking in the global known types, as well as the known
                    // types of the parent type:
                    //----------------------------

                    // First, look in the list of known types passed to the DataContractSerializer:
                    bool typeWasFound = false;
                    foreach (Type knownType in knownTypes)
                    {
                        if (IsTypeSameAsTheOneSpecifiedInXmlTypeAttribute(knownType, typeNameInXmlTypeAttribute, typeNamespaceInXmlTypeAttribute))
                        {
                            memberType = knownType;
                            typeWasFound = true;
                            break;
                        }
                    }

                    // If not found three, look in the [KnownType] attribute(s) of the parent type:
                    if (!typeWasFound)
                    {
                        if (parentType != null)
                        {
                            foreach (Type knownType in GetKnownTypesByReadingKnownTypeAttributes(parentType))
                            {
                                if (IsTypeSameAsTheOneSpecifiedInXmlTypeAttribute(knownType, typeNameInXmlTypeAttribute, typeNamespaceInXmlTypeAttribute))
                                {
                                    memberType = knownType;
                                    typeWasFound = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (!typeWasFound)
                    {
                        throw new SerializationException(
                            string.Format(
                                "Element '{0}' contains data of the '{1}' data contract. The deserializer has no knowledge of any type that maps to this contract. Add the type corresponding to '{2}' to the list of known types - for example, by using the KnownTypeAttribute attribute or by adding it to the list of known types passed to DataContractSerializer.",
                                parentMemberInformationIfNotRoot != null ? parentMemberInformationIfNotRoot.Name : "",
                                typeNamespaceInXmlTypeAttribute + ":" + typeNameInXmlTypeAttribute,
                                typeNameInXmlTypeAttribute
                            ));
                    }

                    ////we try to get the type from the namespace and type name:
                    //memberActualType = memberType.Assembly.GetType(ns + "." + typeName);
                }
            }

            return memberType;
        }

        static bool IsTypeSameAsTheOneSpecifiedInXmlTypeAttribute(Type typeToCompare, string typeNameInXmlTypeAttribute, string typeNamespaceInXmlTypeAttribute)
        {
            string typeName = DataContractSerializer_Helpers.GetTypeNameSafeForSerialization(typeToCompare);

            string typeNamespaceName = DataContractSerializer_Helpers.DATACONTRACTSERIALIZER_OBJECT_DEFAULT_NAMESPACE + typeToCompare.Namespace;
            //todo: verify that the namespace is OK or if we should get the namespace specified by the DataContract attribute by calling "GetTypeInformationByReadingAttributes".
            TypeInformation referenceTypeInfo = DataContractSerializer_Helpers.GetTypeInformationByReadingAttributes(typeToCompare, null);
            typeNamespaceName = referenceTypeInfo.NamespaceName;
            typeName = referenceTypeInfo.Name;

            // Compare the KnownType with the type name and namespace that we are looking for:
            if (typeName == typeNameInXmlTypeAttribute
                && typeNamespaceName == typeNamespaceInXmlTypeAttribute)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static IEnumerable<Type> GetKnownTypesByReadingKnownTypeAttributes(Type typeThatHasTheKnownTypeAttributes)
        {
            foreach (Attribute attr in typeThatHasTheKnownTypeAttributes.GetCustomAttributes(typeof(KnownTypeAttribute), true))
            {
                if (attr is KnownTypeAttribute)
                {
                    KnownTypeAttribute knownTypeAttribute = (KnownTypeAttribute)attr;

                    //------------------------------------
                    // Return the type defined using the [KnownType(type)] constructor overload:
                    //------------------------------------
                    if (knownTypeAttribute.Type != null)
                    {
                        yield return knownTypeAttribute.Type;
                    }

                    //------------------------------------
                    // Return the types returned by the method specified using the [KnownType(methodName)] constructor overload:
                    //------------------------------------
                    if (!string.IsNullOrWhiteSpace(knownTypeAttribute.MethodName))
                    {
                        string methodName = knownTypeAttribute.MethodName;
                        MethodInfo methodInfo = typeThatHasTheKnownTypeAttributes.GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                        if (methodInfo != null)
                        {
                            var types = methodInfo.Invoke(null, null);
                            if (types is IEnumerable<Type>)
                            {
                                foreach (Type type in (IEnumerable<Type>)types)
                                {
                                    yield return type;
                                }
                            }
                        }
                        else
                        {
                            throw new InvalidDataContractException(
                                string.Format(
                                    "KnownTypeAttribute attribute on type '{0}' specifies a method named '{1}' to provide known types. Static method '{1}()' was not found on this type. Ensure that the method exists and is marked as static.",
                                    typeThatHasTheKnownTypeAttributes.ToString(),
                                    methodName
                                ));
                        }
                    }
                }
            }
        }

        static void GetNameAndNamespaceFromXmlTypeAttribute(XAttribute xmlTypeAttribute, XElement xElement, out string typeNameInXmlTypeAttribute, out string typeNamespaceInXmlTypeAttribute)
        {
            typeNameInXmlTypeAttribute = null;
            typeNamespaceInXmlTypeAttribute = null;
            if (xmlTypeAttribute != null)
            {
                typeNameInXmlTypeAttribute = xmlTypeAttribute.Value;
                int indexOfColon = typeNameInXmlTypeAttribute.IndexOf(':');

                typeNamespaceInXmlTypeAttribute = xElement.GetDefaultNamespace().NamespaceName;// xElement.Name.NamespaceName; //todo: might be that, might be something else.
                if (indexOfColon != -1)
                {
                    typeNamespaceInXmlTypeAttribute = xElement.GetNamespaceOfPrefix(typeNameInXmlTypeAttribute.Substring(0, indexOfColon)).NamespaceName;
                }
                typeNameInXmlTypeAttribute = typeNameInXmlTypeAttribute.Substring(indexOfColon + 1); //+ 1 because we do not want the colon itself (and it also works when we had -1).
            }
        }
    }
}
