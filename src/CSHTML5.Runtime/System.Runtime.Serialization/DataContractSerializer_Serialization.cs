
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
    internal static class DataContractSerializer_Serialization
    {
        static Dictionary<Type, MethodInfo> TypeToOnSerializingMethodCache = new Dictionary<Type, MethodInfo>();
        static Dictionary<Type, MethodInfo> TypeToOnSerializedMethodCache = new Dictionary<Type, MethodInfo>();

        /// <summary>
        /// Serializes the given object.
        /// </summary>
        /// <param name="obj">The instance of the object to serialize</param>
        /// <param name="objExpectedType">The type that the serialization should expect (it can be different from the type of the instance if the instance is from a child class of this type)</param>
        /// <param name="knownTypes">The types that should be known by the DataContractSerializer to allow serialization.</param>
        /// <param name="useXmlSerializerFormat">True if the DataContractSerializer should use the format normally used by the XmlSerializer.</param>
        /// <param name="isRoot">True if the current node we want to Serialize is the Root (it is the object that is put as the parameter when calling DataContractSerializer.SerializeToXXX.</param>
        /// <param name="isContainedInsideEnumerable">True if the current element is contained inside an Enumerable. It means that we should add a Node that contains the name of the class expected by the Enumerable (I think, this is old stuff for me)</param>
        /// <param name="parentTypeInformation">This contains informations on the type that contains the current object; or in the case of the root, on the type expected by the DataContractSerializer</param>
        /// <param name="nodeDefaultNamespaceIfAny">The default namespace that is applied on the node (not sure exactly since it is old stuff for me).</param>
        /// <returns>A List&lt;XNode&gt; of the nodes for the current element (a list in case of an Enumerable I think).</returns>
        internal static List<XNode> SerializeToXNodes(object obj, Type objExpectedType, IReadOnlyList<Type> knownTypes, bool useXmlSerializerFormat, bool isRoot, bool isContainedInsideEnumerable, TypeInformation parentTypeInformation, string nodeDefaultNamespaceIfAny)
        {
            //Note: objExpectedtype was added because we needed to know if obj was a string or a char. It is now also used for when the root is of a child class
            //      of the one expected by the DataContractSerializer. In that case, the DataContractSerializer should add a i:type="XXX" attribute to the root.

            //namespace for type (as in i:type): xmlns:i="http://www.w3.org/2001/XMLSchema-instance"
            //when it's used: <Objet xmlns:d2p1="http://www.w3.org/2001/XMLSchema" i:type="d2p1:string">babab</Objet>

            Type objectType = obj.GetType();

            if(objectType.IsEnum || DataContractSerializer_ValueTypesHandler.TypesToNames.ContainsKey(objectType)) //todo: make sure this does not cause issues with the enum types compared to what is below commented.
            {
            //if (objectType.IsValueType || obj is string)
            //{
                //-------------------------
                // VALUE TYPE OR STRING (more or less a primitive type)
                //-------------------------

                return SerializeToXNodes_ValueTypeOrString(obj, objExpectedType, objectType, knownTypes, useXmlSerializerFormat, nodeDefaultNamespaceIfAny, isRoot, isContainedInsideEnumerable, parentTypeInformation);
            }
            else if (obj.GetType() == typeof(Byte[]))
            {
                //-------------------------
                // BYTE ARRAY
                //-------------------------

                return SerializeToXNodes_ByteArray(obj, isRoot, isContainedInsideEnumerable);
            }
            else if (obj is IEnumerable) //todo: should we be more precise and check if it implements IEnumerable<T> or is an Array, like we do during the Deserialization?
            {
                //-------------------------
                // ENUMERABLE (WITH RECURSION)
                //-------------------------

                return SerializeToXNodes_Enumerable_WithRecursion(obj, objectType, knownTypes, useXmlSerializerFormat, nodeDefaultNamespaceIfAny, isRoot, isContainedInsideEnumerable);
            }
            else
            {
                //-------------------------
                // OTHER OBJECT TYPE (WITH RECURSION)
                //-------------------------

                return SerializeToXNodes_Object_WithRecursion(obj, objectType, objExpectedType, knownTypes, useXmlSerializerFormat, nodeDefaultNamespaceIfAny, isRoot, isContainedInsideEnumerable, parentTypeInformation);
            }
        }

        //todo: those additional parameter are probably useless now.
        static List<XNode> SerializeToXNodes_ValueTypeOrString(object obj, Type objExpectedType, Type objectType, IReadOnlyList<Type> knownTypes, bool useXmlSerializerFormat, string nodeDefaultNamespaceIfAny, bool isRoot, bool isContainedInsideEnumerable, TypeInformation parentTypeInformation)
        {
            string str;
            if (obj is Double)
            {
#if SILVERLIGHT
                str = ((Double)obj).ToString(CultureInfo.InvariantCulture);
#else
                str = ((Double)obj).ToString();
#endif
            }
            else if (obj is char && objExpectedType != typeof(string)) //the second part is required because JSIL is bad at differentiating between char and string.
            {
                str = ((int)((char)obj)).ToString(); //that looks really bad but it works...
            }
            else if (obj is DateTime)
            {
                DateTime dt = (DateTime)obj;
                str = INTERNAL_DateTimeHelpers.FromDateTime(dt);
            }
            else if (obj is bool)
            {
                str = obj.ToString().ToLower(); //this is required for WCF calls.
            }
            else
            {
                str = obj.ToString();
            }

            XText xtext = new XText(str);

            // If the value is the root or if it is inside an Enumerable, we add an XElement to surround it:
            if (isRoot || isContainedInsideEnumerable)
                return AddSurroundingXElement(xtext, DataContractSerializer_ValueTypesHandler.TypesToNames[obj.GetType()], isRoot, isContainedInsideEnumerable);
            else
                return new List<XNode>() { xtext };
        }

        static List<XNode> SerializeToXNodes_ByteArray(object obj, bool isRoot, bool isContainedInsideEnumerable)
        {
            XText xtext = new XText(Convert.ToBase64String((Byte[])obj));

            // If the value is the root or if it is inside an Enumerable, we add an XElement to surround it:
            if (isRoot || isContainedInsideEnumerable)
                return AddSurroundingXElement(xtext, "base64Binary", isRoot, isContainedInsideEnumerable);
            else
                return new List<XNode>() { xtext };
        }

        static List<XNode> SerializeToXNodes_Enumerable_WithRecursion(object obj, Type objectType, IReadOnlyList<Type> knownTypes, bool useXmlSerializerFormat, string nodeDefaultNamespaceIfAny, bool isRoot, bool isContainedInsideEnumerable)
        {
            List<XNode> result = new List<XNode>();

            // Get the type information (namespace, etc.) by reading the DataContractAttribute and similar attributes, if present:
            TypeInformation typeInformation = DataContractSerializer_Helpers.GetTypeInformationByReadingAttributes(objectType, nodeDefaultNamespaceIfAny);

            // Traverse the collection:
            foreach (object item in (IEnumerable)obj)
            {
                //todo: USEMETHODTOGETTYPE: make a method that returns the actual type of object and replace all USEMETHODTOGETTYPE with a call to this method. (also find other places where we could use it)
                Type itemType = item.GetType();
                if (item is char) //special case because JSIL thinks a variable of type object containing a char contains a string.
                {
                    itemType = typeof(char);
                }

                //********** RECURSION **********
                //WILD GUESS: Since we are going to the content of an IEnumerable, the namespace of the IEnumerable becomes irrelevant for its content so we give them null instead.
                List<XNode> xnodesForItem = SerializeToXNodes(item, itemType, knownTypes, useXmlSerializerFormat, isRoot: false, isContainedInsideEnumerable: true, parentTypeInformation: typeInformation, nodeDefaultNamespaceIfAny: typeInformation.NamespaceName);

                // Keep only the first node:
                if (xnodesForItem.Count > 1)
                    throw new Exception("When serializing an IEnumerable, we do not expect an item of the enumerable to be serialized as multiple XNodes.");
                XNode nodeForItem = xnodesForItem.First();

                // If it's a value type, add an XElement to surround the node:
                if (nodeForItem is XText)
                {
                    //todo: do we still ever enter this block? In fact, we now add the surrounding XElement in other places.

                    //------------
                    // Value type
                    //------------

                    //we assume namespace of the element is "http://schemas.microsoft.com/2003/10/Serialization/Arrays" as that is what it is for both an array and a list of string
                    //the name of the element should be found in the Dictionary Type --> XName with the item's type
                    //Note: We shoud not arrive here if we cannot find the name in the Dictionary mentioned (except maybe for structs but we'll see when we'll deal with them).
                    XName elementName = XNamespace.Get("http://schemas.microsoft.com/2003/10/Serialization/Arrays").GetName(DataContractSerializer_ValueTypesHandler.TypesToNames[item.GetType()]); //in this line, we create a XName with the namespace that seems to be used for all Enumerables and a name associated with the type.
                    XElement element = new XElement(elementName);
                    //  name of the element: use the Dictionary Type --> XName with the item's type
                    element.Add(nodeForItem);
                    nodeForItem = element;
                }

                // Add the node to the resulting collection of nodes:
                result.Add(nodeForItem);
            }

            // If the value is the root, add an XElement "ArrayOf..." to surround the nodes:
            if (isRoot)
            {
                Type itemsType;
                if (DataContractSerializer_Helpers.IsAssignableToGenericEnumerableOrArray(objectType, out itemsType))
                {
                    string elementName = "Array";

                    if (DataContractSerializer_ValueTypesHandler.TypesToNames.ContainsKey(itemsType))
                    {
                        elementName = "ArrayOf" + DataContractSerializer_ValueTypesHandler.TypesToNames[itemsType];
                    }
                    else
                    {
                        // In case of nested types, replace the '+' with '.', and do other changes to obtain the type name to use in the serialization:
                        string itemsTypeName = DataContractSerializer_Helpers.GetTypeNameSafeForSerialization(itemsType);

                        elementName = "ArrayOf" + itemsTypeName;
                    }
                    XElement xElement = new XElement(XNamespace.Get("http://schemas.microsoft.com/2003/10/Serialization/Arrays").GetName(elementName), result);
                    xElement.Add(new XAttribute(XNamespace.Get(DataContractSerializer_Helpers.XMLNS_NAMESPACE).GetName("xmlns:i"), DataContractSerializer_Helpers.XMLSCHEMA_NAMESPACE));
                    return new List<XNode>() { xElement };
                }
                else
                {
                    throw new SerializationException(string.Format("The type '{0}' cannot be serialized because it does implement IEnumerable<T> nor is it an Array.", objectType.ToString())); //todo: see if we can avoid this limitation.
                }
            }

            return result;
        }

        static List<XNode> SerializeToXNodes_Object_WithRecursion(object obj, Type objectType, Type resultExpectedType, IReadOnlyList<Type> knownTypes, bool useXmlSerializerFormat, string nodeDefaultNamespaceIfAny, bool isRoot, bool isContainedInsideEnumerable, TypeInformation parentTypeInformation)
        {
            // Call the "OnSerializing" method if any:
            CallOnSerializingMethod(obj, objectType);

            // Get the type information (namespace, etc.) by reading the DataContractAttribute and similar attributes, if present:
            TypeInformation typeInformation = DataContractSerializer_Helpers.GetTypeInformationByReadingAttributes(objectType, nodeDefaultNamespaceIfAny);

            // Process each member of the object:
            List<XNode> childrenNodes = new List<XNode>();
            IEnumerable<MemberInformationAndValue> membersAndValues = DataContractSerializer_Helpers.GetDataContractMembersAndValues(obj, serializationType: typeInformation.serializationType, useXmlSerializerFormat: useXmlSerializerFormat); //todo: normally, checking if a data contract is present is not enough to know if the type if "marked with attributes". According to the MSND article "Using Data Contracts", we should check for any of the following attributes: "DataContractAttribute, SerializableAttribute, CollectionDataContractAttribute, or EnumMemberAttribute attributes, or marked as serializable by any other means (such as IXmlSerializable)". cf. https://msdn.microsoft.com/en-us/library/ms733127(v=vs.100).aspx
            foreach (MemberInformationAndValue memberInfoAndValue in membersAndValues)
            {
                //------------------------------------
                // Process each member of the object
                //------------------------------------

                // Get the member information:
                MemberInfo memberInfo = memberInfoAndValue.MemberInformation.MemberInfo;
                object memberValue = memberInfoAndValue.MemberValue;
                string memberName = memberInfoAndValue.MemberInformation.Name;

                // Create the XNode for the member:
                XName xnameForMember;
                if (objectType.FullName.StartsWith("System.Collections.Generic.KeyValuePair"))
                {
                    if(memberName == "key")
                    {
                        memberName = "Key";
                    }
                    else if (memberName == "value")
                    {
                        memberName = "Value";
                    }
                    xnameForMember = memberName;
                }
                else
                {
                    if (typeInformation.NamespaceName == null) //todo: make sure we want to use this typeInformation since it comes from the type that contains of the current member and not the member itself.
                    {
                        xnameForMember = memberName;
                    }
                    else
                    {
                        xnameForMember = XNamespace.Get(typeInformation.NamespaceName).GetName(memberName);
                    }
                }
                XElement xElementForMember = new XElement(xnameForMember);

                bool isNull = ((memberValue == null) || DataContractSerializer_Helpers.CheckIfObjectIsNullNullable(memberValue));

                if (!isNull)
                {
                    Type expectedType = memberInfoAndValue.MemberInformation.MemberType;
                    Type memberType = memberValue.GetType();

                    // Work around JSIL issues:
                    if (memberValue is char && expectedType == typeof(char)) //Note: this test is required because JSIL thinks that object testobj = 'c'; testobj.GetType() should return System.String...
                    {
                        memberType = typeof(char);
                    }
                    else if (expectedType == typeof(double)) //Note: this test is required because, with JSIL, "GetType" on a double returns "int" instead of "double". //todo: see if other workarounds to JSIL bugs like this one are required.
                    {
                        memberType = typeof(double);
                    }
                    else if(expectedType == typeof(byte)) //same as above.
                    {
                        memberType = typeof(byte);
                    }
                    else if (expectedType == typeof(float)) //same as above.
                    {
                        memberType = typeof(float);
                    }

                    bool isValueEnumerableDifferentThanString = (memberValue is IEnumerable) && !(memberValue is string);

                    Type nonNullableMemberType = memberType;
                    if (memberType.FullName.StartsWith("System.Nullable`1"))
                    {
                        nonNullableMemberType = Nullable.GetUnderlyingType(memberType);
                    }
                    Type nonNullableExpectedType = expectedType;
                    if (expectedType.FullName.StartsWith("System.Nullable`1"))
                    {
                        nonNullableExpectedType = Nullable.GetUnderlyingType(expectedType);
                    }

                    if (nonNullableMemberType != nonNullableExpectedType && !isValueEnumerableDifferentThanString)
                    {
                        //we want to add a type attribute to be able to know when deserializing that this is another type that the property's
                        if (DataContractSerializer_ValueTypesHandler.TypesToNames.ContainsKey(nonNullableMemberType)) //todo: should we add the nullable versions?
                        {
                            string prefixForType = xElementForMember.GetPrefixOfNamespace("http://www.w3.org/2001/XMLSchema");
                            if (string.IsNullOrWhiteSpace(prefixForType))
                            {
                                //we need to create a prefix for that
                                //todo:see if it would be ok to use d2p1 like silverlight seems to do, and if so, replace the following with just that.

                                prefixForType = DataContractSerializer_Helpers.GenerateUniqueNamespacePrefixIfNeeded(xElementForMember, "http://www.w3.org/2001/XMLSchema");

                                //we can finally add the type attribute:
                                xElementForMember.Add(new XAttribute(XNamespace.Get(DataContractSerializer_Helpers.XMLSCHEMA_NAMESPACE).GetName("type"), prefixForType + ":" + DataContractSerializer_ValueTypesHandler.TypesToNames[nonNullableMemberType]));
                            }
                            else
                            {
                                xElementForMember.Add(new XAttribute(XNamespace.Get(DataContractSerializer_Helpers.XMLSCHEMA_NAMESPACE).GetName("type"), prefixForType + ":" + DataContractSerializer_ValueTypesHandler.TypesToNames[nonNullableMemberType]));
                            }
                        }
                        else
                        {
                            bool isTypeOk = DataContractSerializer_KnownTypes.CheckIfItIsAKnownType(obj, objectType, knownTypes, memberType);
                            if (isTypeOk)
                            {
                                if (memberType.IsEnum) //enums are a special case because JSIL is not capable of handling Custom attributes on them.
                                {
                                    xElementForMember.Add(new XAttribute(XNamespace.Get(DataContractSerializer_Helpers.XMLSCHEMA_NAMESPACE).GetName("type"), memberType.Name));
                                }
                                else
                                {
                                    string defaultNamespace = DataContractSerializer_Helpers.DATACONTRACTSERIALIZER_OBJECT_DEFAULT_NAMESPACE + memberType.Namespace;

                                    // Get the type information (namespace, etc.) by reading the DataContractAttribute and similar attributes, if present:
                                    TypeInformation childTypeInformation = DataContractSerializer_Helpers.GetTypeInformationByReadingAttributes(memberType, defaultNamespace);

                                    string prefixForTypeName = "";
                                    if (childTypeInformation.NamespaceName != null) //when the namespaceName is null I guess it means we don't need something like type="sth:ChildTypeName" but type="ChildTypeName" is sufficient.
                                    {
                                        prefixForTypeName = DataContractSerializer_Helpers.GenerateUniqueNamespacePrefixIfNeeded(xElementForMember, childTypeInformation.NamespaceName) + ":"; // note: we added : directly because whenever we have a prefix, we have ':' right after.
                                    }

                                    xElementForMember.Add(new XAttribute(XNamespace.Get(DataContractSerializer_Helpers.XMLSCHEMA_NAMESPACE).GetName("type"), prefixForTypeName + childTypeInformation.Name));
                                }
                            }
                            else
                            {
                                string namespaceNameToPutInException = "";
                                if (typeInformation.NamespaceName != null)
                                {
                                    namespaceNameToPutInException = ":" + typeInformation.NamespaceName;
                                }
                                throw new SerializationException("Type \"" + memberType.FullName + "\" with data contract name \"" + memberType.Name + namespaceNameToPutInException + "\" is not expected. Add any types not known statically to the list of known types - for example, by using the KnownTypeAttribute attribute or by adding them to the list of known types passed to DataContractSerializer.");
                            }
                        }
                    }

                    //********** RECURSION **********
                    List<XNode> xnodesForMemberValue = SerializeToXNodes(memberValue, memberType, knownTypes, useXmlSerializerFormat, isRoot: false, isContainedInsideEnumerable: false, parentTypeInformation: typeInformation, nodeDefaultNamespaceIfAny: typeInformation.NamespaceName);
                    foreach (XNode xnodeForMemberValue in xnodesForMemberValue) // Note: the collection usually contains only 1 node, but there may be multiple nodes if for example we are serializing an Enumerable.
                    {
                        xElementForMember.Add(xnodeForMemberValue);
                    }
                }
                else
                {
                    //------------------------------------
                    // The value of the member is "null"
                    //------------------------------------

                    XName nilName = XNamespace.Get(DataContractSerializer_Helpers.XMLSCHEMA_NAMESPACE).GetName("nil");
                    xElementForMember.Add(new XAttribute(nilName, "true"));
                }
                childrenNodes.Add(xElementForMember);
            }
            if (isRoot || isContainedInsideEnumerable)
            {
                //we add a XElement with the type name as its name.
                //we create the XName for the node containing the type.

                //----------------------------------
                // Determine the name of the object to use in the XML:
                //----------------------------------

                //we get the type expected by the parent Enumerable:
                Type typeExpectedByParentEnumerable = isRoot ? resultExpectedType : DataContractSerializer_Helpers.GetInterface(parentTypeInformation.Type, "IEnumerable`1").GetGenericArguments()[0];

                string objectTypeName;

                // If the parent of the object is a collection that has the
                // "CollectionDataContractAttribute", we use the name specified
                // by that attribute, if any. Otherwise we determine the name
                // from the Type of the object

                if (parentTypeInformation != null
                    && !string.IsNullOrEmpty(parentTypeInformation.ItemName))
                {
                    objectTypeName = parentTypeInformation.ItemName;
                }
                else
                {
                    objectTypeName = DataContractSerializer_Helpers.GetTypeNameSafeForSerialization(typeExpectedByParentEnumerable); // Note: in case of nested types, this method replaces the '+' with '.', and does other changes to obtain the type name to use in the serialization.
                }

                XName elementName;
                if(typeInformation.NamespaceName == null)
                {
                    elementName = objectTypeName;
                }
                else
                {
                    elementName = XNamespace.Get(typeInformation.NamespaceName).GetName(objectTypeName);
                }

                XElement xelement = new XElement(elementName);

                if (objectType.IsEnum) //enums are a special case because JSIL is not capable of handling Custom attributes on them.
                {
                    xelement.Add(new XAttribute(XNamespace.Get(DataContractSerializer_Helpers.XMLSCHEMA_NAMESPACE).GetName("type"), objectTypeName));
                }
                else if (typeInformation.NamespaceName != null && !objectType.IsGenericType && typeExpectedByParentEnumerable != obj.GetType())
                {
                    string prefixForTypeName = DataContractSerializer_Helpers.GenerateUniqueNamespacePrefixIfNeeded(xelement, typeInformation.NamespaceName);

                    xelement.Add(new XAttribute(XNamespace.Get(DataContractSerializer_Helpers.XMLSCHEMA_NAMESPACE).GetName("type"), prefixForTypeName + ":" + typeInformation.Name));
                }

                foreach (XNode node in childrenNodes)
                {
                    xelement.Add(node);
                }
                childrenNodes = new List<XNode>() { xelement };
            }

            // Call the "OnSerialized" method if any:
            CallOnSerializedMethod(obj, objectType);

            return childrenNodes;
        }

        static List<XNode> AddSurroundingXElement(XText xtext, string surroundingElementName, bool isRoot, bool isContainedInsideEnumerable)
        {
            string elementNamespace = isContainedInsideEnumerable ? "http://schemas.microsoft.com/2003/10/Serialization/Arrays" : "http://schemas.microsoft.com/2003/10/Serialization/";
            XElement xElement = new XElement(XNamespace.Get(elementNamespace).GetName(surroundingElementName), xtext);
            return new List<XNode>() { xElement };
        }

        static void CallOnSerializingMethod(object obj, Type objType)
        {
            DataContractSerializer_Helpers.CallMethod(obj, objType, typeof(OnSerializingAttribute), TypeToOnSerializingMethodCache);
        }

        static void CallOnSerializedMethod(object obj, Type objType)
        {
            DataContractSerializer_Helpers.CallMethod(obj, objType, typeof(OnSerializedAttribute), TypeToOnSerializedMethodCache);
        }
    }
}
