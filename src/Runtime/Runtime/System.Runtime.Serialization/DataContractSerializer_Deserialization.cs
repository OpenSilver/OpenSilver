

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


using CSHTML5;
using DotNetForHtml5.Core;
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
    internal static class DataContractSerializer_Deserialization
    {
        static Dictionary<Type, MethodInfo> TypeToOnDeserializingMethodCache = new Dictionary<Type, MethodInfo>();
        static Dictionary<Type, MethodInfo> TypeToOnDeserializedMethodCache = new Dictionary<Type, MethodInfo>();

        internal static object DeserializeToCSharpObject(IEnumerable<XNode> content, Type resultType, XElement parentElement, IReadOnlyList<Type> knownTypes, bool ignoreErrors, bool useXmlSerializerFormat)
        {
            if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                resultType = resultType.GetGenericArguments()[0];
            }

            Type itemsType;

            //if (resultType.IsValueType || resultType == typeof(string))
            if (resultType.IsEnum || DataContractSerializer_ValueTypesHandler.TypesToNames.ContainsKey(resultType))
            {
                //-------------------------
                // VALUE TYPE OR STRING
                //-------------------------

                return DeserializeToCSharpObject_ValueTypeOrString(content, resultType);

            }
            else if (resultType.IsArray && resultType.GetElementType() == typeof(Byte))
            {
                //-------------------------
                // BYTE ARRAY
                //-------------------------

                return DeserializeToCSharpObject_ByteArray(content);
            }
            else if (DataContractSerializer_Helpers.IsAssignableToGenericEnumerableOrArray(resultType, out itemsType))
            {
                //-------------------------
                // IEnumerable<T> or array (WITH RECURSION)
                //-------------------------

                return DeserializeToCSharpObject_Enumerable_WithRecursion(content, resultType, knownTypes, ignoreErrors, itemsType, useXmlSerializerFormat);

            }
            else
            {
                //-------------------------
                // OTHER OBJECT TYPE (WITH RECURSION)
                //-------------------------

                return DeserializeToCSharpObject_Object_WithRecursion(content, resultType, parentElement, knownTypes, ignoreErrors, useXmlSerializerFormat);
            }

            throw new Exception("Unable to create the type '" + resultType.ToString() + "'.");
        }

        static object DeserializeToCSharpObject_ValueTypeOrString(IEnumerable<XNode> content, Type resultType)
        {
            if (content.Any())
            {
                foreach (XNode node in content)
                {
                    if (node is XText) // Normally value types were serialized as direct text content
                    {
                        XText text = (XText)node;
                        string str = text.Value;

                        return DataContractSerializer_ValueTypesHandler.ConvertStringToValueType(str, resultType);
                    }
                }
            }
            else
            {
                //-------------------------
                // We have arrived to an empty node, such as in: <MyObject><MyStringProperty /></MyObject>
                //-------------------------
                if (resultType == typeof(string))
                {
                    return string.Empty;
                }
            }
            throw new Exception("Unable to create the type '" + resultType.ToString() + "'.");
        }

        static object DeserializeToCSharpObject_ByteArray(IEnumerable<XNode> content)
        {
            //we get the string that represents the byte array:
            foreach (XNode node in content)
            {
                if (node is XText)
                {
                    return Convert.FromBase64String(((XText)node).Value);
                }
            }
            return new byte[] { };
        }

        static object DeserializeToCSharpObject_Enumerable_WithRecursion(IEnumerable<XNode> content, Type resultType, IReadOnlyList<Type> knownTypes, bool ignoreErrors, Type itemsType, bool useXmlSerializerFormat)
        {
            // Here is how it works:
            // 1. First we create a new instance of List<T> to add child items recursively
            // 2. Then:
            //   - if the result type is an array or just "IEnumerable", we convert the list to an array and return it
            //   - if the result type is List<T> or a compatible interface such as IList<> ,IEnumerable<>, ICollection<>, IReadOnlyCollection<>, IReadOnlyList<>, we just return the list "as is".
            //   - otherwise we attempt to create a new instance of the result type by passing the list to the constructor and return it
            //   - if it fails (because no such constructor exists), we attempt to create a new instance and call the "Add(T)" method to add the items, if found, and return the new instance.


            //--------------------------------
            // Create a temporary List<T> in order to add items to it. Later we will convert it to the final type if needed.
            //--------------------------------

#if !BRIDGE
            var methodToCreateNewInstanceOfGenericList = typeof(DataContractSerializer_Deserialization).GetMethod("CreateNewInstanceOfGenericList", BindingFlags.NonPublic | BindingFlags.Static);
            if (methodToCreateNewInstanceOfGenericList == null)
                throw new Exception("Cannot find the private static method named 'CreateNewInstanceOfGenericList'.");
            object list = methodToCreateNewInstanceOfGenericList.MakeGenericMethod(itemsType).Invoke(null, new object[] { });

            // Note: in the code above, we call the method "CreateNewInstanceOfGenericList" instead of
            // calling "var list = Activator.CreateInstance(typeof(List<>).MakeGenericType(itemsType))"
            // because the latter does not appear to properly initialize the underlying JavaScript "_items"
            // collection that is inside the List<T> implementation in JSIL as of July 14, 2017, thus
            // resulting in an exception when calling the Add method (reference: CSHTML5 tickets #623 and
            // #648).

#else
            // TODOBRIDGE: verify if the two code are similar
            Type[] arg = { itemsType };
            object list;

            if (Interop.IsRunningInTheSimulator)
            {
                list = INTERNAL_Simulator.SimulatorProxy.MakeInstanceOfGenericType(typeof(List<>), arg);
            }
            else
            {
                list = Activator.CreateInstance(typeof(List<>).MakeGenericType(arg));
            }
#endif


            // Get a reference to the "Add" method of the generic list that we just created:
            var listAddMethod = list.GetType().GetMethod("Add");

            int count = 0; // Note: this is for the case where we have an array, to be able to create an array of the correct size.

            //--------------------------------
            // Add the items to the list:
            //--------------------------------
            foreach (XNode node in content)
            {
                if (node is XElement) // Normally the elements of a IEnumerable were serialized as XElements
                {
                    XElement xElement = (XElement)node;
                    object childObject;
                    if (DataContractSerializer_Helpers.IsElementNil(xElement))
                    {
                        if (itemsType.IsValueType && !itemsType.FullName.StartsWith("System.Nullable`1"))
                        {
                            childObject = Activator.CreateInstance(itemsType);
                        }
                        else
                        {
                            childObject = null;
                        }
                    }
                    else
                    {
                        IEnumerable<XNode> elementChildNodes = xElement.Nodes();

                        Type childObjectActualType = DataContractSerializer_KnownTypes.GetCSharpTypeForNode(xElement, itemsType, itemsType, knownTypes, null, useXmlSerializerFormat); //Note: the two "null" values are used only in the case where we couldn't find the type.


                        //********** RECURSION **********
                        childObject = DeserializeToCSharpObject(elementChildNodes, childObjectActualType, xElement, knownTypes, ignoreErrors, useXmlSerializerFormat);
                    }

                    // Add the child to the resulting enumerable:
                    listAddMethod.Invoke(list, new object[] { childObject });
                }
                ++count;
            }

            //--------------------------------
            // Convert the list to the result type:
            //--------------------------------

            Type genericTypeDefinition;
            if (resultType.IsArray || resultType == typeof(IEnumerable)) // Note: here we deliberately use "==" instead of "IsAssignableFrom".
            {
                //------------------------
                // If the result type is an array T[] or the type is exactly "IEnumerable"
                //------------------------

                //create an array with the correct number of elements:
                Array result = Array.CreateInstance(itemsType, count);

                //now we need to fill the array:
                ((IList)list).CopyTo(result, 0);

                return result;
            }
            else if (resultType.IsGenericType
                    && (genericTypeDefinition = resultType.GetGenericTypeDefinition()) != null
                    && (genericTypeDefinition == typeof(List<>)
                        || genericTypeDefinition == typeof(IList<>)
                        || genericTypeDefinition == typeof(IEnumerable<>)
                        || genericTypeDefinition == typeof(ICollection<>)
                        //|| genericTypeDefinition == typeof(IReadOnlyCollection<>)
                        //|| genericTypeDefinition == typeof(IReadOnlyList<>)
                        ))
            {
                //------------------------
                // If the result type is List<T> or a compatible interface such as: IList<> ,IEnumerable<>, ICollection<>, IReadOnlyCollection<>, IReadOnlyList<>
                //------------------------

                // We directly return the List<> that we created above:
                return list;
            }
            else
            {
                //------------------------
                // Otherwise, we attempt to create a new instance of the result type by passing the items as first argument of the constructor (this works for example with ObservableCollection<T> and other common collections):
                //------------------------

                try
                {
                    //todo: check if the type inherits from Dictionary here and if so, we do nothing here and it will be handled later.
#if !BRIDGE
                    object result1 = Activator.CreateInstance(resultType, args: new object[] { list });
#else
                    object result1 = Activator.CreateInstance(resultType, arguments: new object[] { list });
#endif


                    //we check if the elements have correctly been added to the result. If they have, we return the result, otherwise, we try to add the elements through a Add method.
                    //result1 is an IEnumerable<> or an Array
                    bool isOk = false;
                    if (count == 0) //if there was no elements to add anyway, the result is good as it is.
                    {
                        isOk = true;
                    }
                    else
                    {
                        if (resultType.IsArray)
                        {
                            if (((Array)result1).Length == count)
                            {
                                isOk = true;
                            }
                        }
                        else //result1 is an IEnumerable<>
                        {
                            foreach (object item in (IEnumerable)result1)
                            {
                                isOk = true;
                                break;//if there is at least one element, we assume all elements were added correctly.
                            }
                        }
                    }
                    if (isOk)
                    {
                        return result1;
                    }
                }
                catch
                {
                    // If it failed, we attempt something else (see below).
                }

                //------------------------
                // Otherwise, we search for the method "Add(T)" in the result type, so that we can create a new instance of the type and then we call "Add(T)" to add the items:
                //------------------------

                // Get a reference to the "Add(T)" method:
                var addMethod = resultType.GetMethod("Add", new Type[] { itemsType });
                bool isDictionary = false;
                if (addMethod == null)
                {
                    //if we couldn't find an Add method, we check if it is a Dictionary, and if so we call the Add(Key, Value) method instead:

                    //note: Another way to try and make it work could be to look if itemsType is a GenericType and try to find the elements of the types in its genericArguments and find a Add ethod with those arguments types?

                    Type currentType = resultType;
                    while (currentType != null && !(currentType.FullName.StartsWith("System.Collections.Generic.Dictionary"))) //this isn't great but I couldn't find a better way at the moment.
                    {
                        currentType = currentType.BaseType;
                    }
                    if (currentType != null)
                    {
                        currentType = itemsType;
                        while (currentType != null && !(currentType.FullName.StartsWith("System.Collections.Generic.KeyValuePair"))) //this isn't great but I couldn't find a better way at the moment.
                        {
                            currentType = currentType.BaseType;
                        }

                        if (currentType != null)
                        {
                            isDictionary = true;
                            Type keyType = itemsType.GetGenericArguments()[0];
                            Type valueType = itemsType.GetGenericArguments()[1];
                            addMethod = resultType.GetMethod("Add", new Type[] { keyType, valueType });
                        }
                    }
                    if (!isDictionary)
                    {
                        if (!ignoreErrors)
                        {
                            throw new InvalidDataContractException(
                                string.Format(
                                    "Type '{0}' is an invalid collection type since it does not have a valid Add method with parameter of type '{1}'.",
                                    resultType.ToString(),
                                    itemsType.ToString()
                                ));
                        }
                        else
                        {
                            return null;
                        }
                    }
                }

                // Create a new instance of the result type:
                object result2 = null;
                try
                {
                    result2 = Activator.CreateInstance(resultType); //todo: support creating the object without calling the constructor? (in that case, make sure to change the message of the exception in case of failure)
                }
                catch
                {
                }
                if (result2 == null)
                {
                    if (!ignoreErrors)
                    {
                        throw new SerializationException(
                            string.Format(
                                "Cannot create a new instance of the type '{0}'. Please verify that the type has a public parameterless constructor.",
                                resultType.ToString()
                                ));
                    }
                    else
                    {
                        return null;
                    }
                }

                // Add the items to the collection:
                if (!isDictionary)
                {
                    foreach (var item in (IList)list)
                    {
                        addMethod.Invoke(result2, new object[] { item });
                    }
                }
                else
                {
                    foreach (var item in (IList)list)
                    {
                        dynamic dynamicItem = item;// using a dynamic here since it it much simpler but if it causes issues, we should use reflection to get Key and Value.
#if BRIDGE
                        if (Interop.IsRunningInTheSimulator)
                        {
                            addMethod.Invoke(result2, new object[] { dynamicItem.Key, dynamicItem.Value });
                        }
                        else
                        {
                            addMethod.Invoke(result2, new object[] { dynamicItem.key, dynamicItem.value });
                        }
#else
                        addMethod.Invoke(result2, new object[] { dynamicItem.Key, dynamicItem.Value });
#endif
                    }
                }

                return result2;
            }
        }

        static object DeserializeToCSharpObject_Enumerable_WithRecursion_SpecialCase(string name, IEnumerable<XNode> content, Type resultType, IReadOnlyList<Type> knownTypes, bool ignoreErrors, Type itemsType, bool useXmlSerializerFormat)
        {
            List<XNode> nodesThatCorrespondToTheEnumerableContent = new List<XNode>();

            foreach (XNode node in content)
            {
                if (node is XElement) // Normally an object property was serialized as an XElement.
                {
                    XElement xElement = (XElement)node;
                    XName elementName = xElement.Name;
                    string elementNameWithoutNamespace = elementName.LocalName;

                    if (elementNameWithoutNamespace == name) //todo: should we compare the namespace as well?
                    {
                        nodesThatCorrespondToTheEnumerableContent.Add(node);
                    }
                }
            }

            object result = DeserializeToCSharpObject_Enumerable_WithRecursion(nodesThatCorrespondToTheEnumerableContent, resultType, knownTypes, ignoreErrors, itemsType, useXmlSerializerFormat);

            return result;
        }

        static object DeserializeToCSharpObject_Object_WithRecursion(IEnumerable<XNode> content, Type resultType, XElement parentElement, IReadOnlyList<Type> knownTypes, bool ignoreErrors, bool useXmlSerializerFormat)
        {
            // Quit if attempting to deserialize to an interface: //todo: investigate why this may happen.
            if (!resultType.IsInterface)
            {
                string resultTypeFullName = resultType.FullName; // For debugging only, can be removed.

                // Create the resulting class:
                object resultInstance = Activator.CreateInstance(resultType); //todo: replace with "System.Runtime.Serialization.FormatterServices.GetUninitializedObject(type)" so that the type does not require a parameterless constructor.

                // Call the "OnDeserializing" method if any:
                CallOnDeserializingMethod(resultInstance, resultType);

                // Get the type information (namespace, etc.) by reading the DataContractAttribute and similar attributes, if present:
                TypeInformation typeInformation = DataContractSerializer_Helpers.GetTypeInformationByReadingAttributes(resultType, null, useXmlSerializerFormat);

                // Read the members of the target type:
                IEnumerable<MemberInformation> membersInformation = DataContractSerializer_Helpers.GetDataContractMembers(resultType, typeInformation.serializationType, useXmlSerializerFormat);

                // Make a dictionary of the members of the target type for faster lookup:
                Dictionary<string, MemberInformation> memberNameToMemberInformation = new Dictionary<string, MemberInformation>();
                foreach (var memberInformation in membersInformation)
                {
                    string memberName = memberInformation.Name;
                    if (resultType.FullName.StartsWith("System.Collections.Generic.KeyValuePair"))
                    {
                        if (memberName == "key")
                        {
                            memberName = "Key";
                        }
                        else if (memberName == "value")
                        {
                            memberName = "Value";
                        }
                    }
                    if (!memberNameToMemberInformation.ContainsKey(memberName))
                    {
                        memberNameToMemberInformation.Add(memberName, memberInformation);
                    }
                    else
                    {
                        MemberInformation collidingMemberInformation = memberNameToMemberInformation[memberName];
                        throw new InvalidDataContractException(
                            string.Format(
                                "Type '{0}' contains two members '{1}' 'and '{2}' with the same data member name '{3}'. Multiple members with the same name in one type are not supported. Consider changing one of the member names using DataMemberAttribute attribute.",
                                resultType.ToString(),
                                memberInformation.MemberInfo.Name,
                                collidingMemberInformation.MemberInfo.Name,
                                memberName
                            ));
                    }
                }

                // Populate the values of the properties/members of the class:
                HashSet<string> membersForWhichWeSuccessfullSetTheValue = new HashSet<string>();
                foreach (XNode node in content)
                {
                    if (node is XElement) // Normally an object property was serialized as an XElement.
                    {
                        XElement xElement = (XElement)node;
                        XName elementName = xElement.Name;
                        string elementNameWithoutNamespace = elementName.LocalName;

                        // Find the member that has the name of the XNode:
                        MemberInformation memberInformation;
                        if (memberNameToMemberInformation.TryGetValue(elementNameWithoutNamespace, out memberInformation))
                        {
                            // Avoid processing nodes that have the same name as other nodes already processed (this can happen in case of [XmlElement] attribute on enumerable members - cf. "special case" below - but it is handled differently):
                            if (!membersForWhichWeSuccessfullSetTheValue.Contains(memberInformation.Name))
                            {
                                object memberValue = null;
                                Type itemsType = null;
                                Type memberActualType = memberInformation.MemberType; // Note: this is the initial value. It may be modified below.
                                // Handle "Nil" case:
                                if (DataContractSerializer_Helpers.IsElementNil(xElement))
                                {
                                    //----------------------
                                    // XNode is "Nil", so we return the default value of the result type.
                                    //----------------------

                                    memberValue = DataContractSerializer_Helpers.GetDefault(memberInformation.MemberType);
                                }
                                // Handle the special case where there is an [XmlElement] attribute on an enumerable member (XmlSerializer compatibility mode only):
                                else if (useXmlSerializerFormat
                                    && memberInformation.HasXmlElementAttribute
                                    && DataContractSerializer_Helpers.IsAssignableToGenericEnumerableOrArray(memberActualType, out itemsType))
                                {
                                    //---------------------------------
                                    // Special case where there is an [XmlElement] attribute on an enumerable member (XmlSerializer compatibility mode only):
                                    //
                                    // Example:
                                    //      <MyObject>
                                    //         <MyEnumerablePropertyName/>
                                    //         <MyEnumerablePropertyName/>
                                    //         <MyEnumerablePropertyName/>
                                    //      </MyObject>
                                    //
                                    // obtained via:
                                    //      class MyObject
                                    //      {
                                    //          [XmlElement]
                                    //          List<MyType> MyEnumerablePropertyName { get; set; }
                                    //      }
                                    //
                                    // cf. https://docs.microsoft.com/en-us/dotnet/standard/serialization/controlling-xml-serialization-using-attributes
                                    //---------------------------------

                                    object deserializedEnumerable = DeserializeToCSharpObject_Enumerable_WithRecursion_SpecialCase(
                                        memberInformation.Name,
                                        content, memberActualType, knownTypes, ignoreErrors, itemsType, useXmlSerializerFormat);
                                    memberValue = deserializedEnumerable;
                                }
                                // Handle all other cases:
                                else
                                {
                                    //----------------------
                                    // All other cases: the XElement is not "Nil" and needs to be deserialized.
                                    //----------------------

                                    memberActualType = DataContractSerializer_KnownTypes.GetCSharpTypeForNode(xElement, memberInformation.MemberInfo.DeclaringType, memberActualType, knownTypes, memberInformation, useXmlSerializerFormat);

                                    //if the type is nullable, we get the undelying type:
                                    Type nonNullableMemberType = memberActualType;
                                    if (memberActualType.FullName.StartsWith("System.Nullable`1"))
                                    {
                                        nonNullableMemberType = Nullable.GetUnderlyingType(memberActualType);
                                    }

                                    // Recursively create the value for the property:
                                    IEnumerable<XNode> propertyChildNodes = xElement.Nodes();

                                    //********** RECURSION **********
                                    memberValue = DeserializeToCSharpObject(propertyChildNodes, nonNullableMemberType, xElement, knownTypes, ignoreErrors, useXmlSerializerFormat);
                                }

                                //---------------------------------
                                // Set the value of the member:
                                //---------------------------------

                                DataContractSerializer_Helpers.SetMemberValue(resultInstance, memberInformation, memberValue);
                                membersForWhichWeSuccessfullSetTheValue.Add(memberInformation.Name);
                            }
                        }
                        else
                        {
                            //-----------
                            // We ignore missing members, to mimic the behavior of the .NET DataContractSerializer.
                            //-----------
                            //throw new Exception("Member '" + memberName + "' not found in type '" + resultType.Name + "'.");
                        }
                    }
                }

                // In case of XmlSerializer compatibility mode, and [XmlAttribute] attribute on a class member, we also need to deserialize the XAttributes (cf. https://msdn.microsoft.com/en-us/library/system.xml.serialization.xmlattributeattribute(v=vs.110).aspx ):
                if (useXmlSerializerFormat)
                {
                    foreach (XAttribute attribute in parentElement.Attributes())
                    {
                        XName attributeName = attribute.Name;
                        // We assume that the object properties have no namespace //todo: fix this assumption, cf. "XmlAttributeAttribute.Namespace" for example (note: repetition of "Attribute" is intended)
                        if (string.IsNullOrEmpty(attributeName.NamespaceName))
                        {
                            string attributeNameWithoutNamespace = attributeName.LocalName;

                            // Find the member that has the name of the XAttribute:
                            MemberInformation memberInformation;
                            if (memberNameToMemberInformation.TryGetValue(attributeNameWithoutNamespace, out memberInformation)
                                && memberInformation.HasXmlAttributeAttribute)
                            {
                                // Avoid processing members that have already been processed (just in case):
                                if (!membersForWhichWeSuccessfullSetTheValue.Contains(memberInformation.Name))
                                {
                                    string attributeValue = attribute.Value;

                                    // Check to see if the expected type is a value type:
                                    if (DataContractSerializer_ValueTypesHandler.TypesToNames.ContainsKey(memberInformation.MemberType))
                                    {
                                        // Attempt to deserialize the string:
                                        object memberValue = DataContractSerializer_ValueTypesHandler.ConvertStringToValueType(attributeValue, memberInformation.MemberType);

                                        // Set the value of the member:
                                        DataContractSerializer_Helpers.SetMemberValue(resultInstance, memberInformation, memberValue);
                                        membersForWhichWeSuccessfullSetTheValue.Add(memberInformation.Name);
                                    }
                                    else
                                    {
                                        //todo: report the error?
                                        if (memberInformation.MemberType == typeof(List<int>))
                                        {
                                            string[] splittedElements = attributeValue.Split(' ');

#if !BRIDGE
                                            List<int> listint = splittedElements.Select(Int32.Parse).ToList();
#else
                                            List<int> listint = new List<int>();

                                            foreach (string str in splittedElements)
                                            {
                                                listint.Add(Int32.Parse(str));
                                            }
#endif


                                            DataContractSerializer_Helpers.SetMemberValue(resultInstance, memberInformation, listint);
                                            membersForWhichWeSuccessfullSetTheValue.Add(memberInformation.Name);
                                        }
                                    }
                                }
                                else
                                {
                                    //todo: report the error?
                                }
                            }
                            else
                            {
                                //todo: report the error?
                            }
                        }
                    }
                }

                // Verify that the values of all the members marked as "IsRequired" have been set:
                foreach (var memberInformation in membersInformation)
                {
                    if (memberInformation.IsRequired
                        && !membersForWhichWeSuccessfullSetTheValue.Contains(memberInformation.Name))
                    {
                        throw new SerializationException(string.Format("The member '{0}' is required but it was not found in the document being deserialized.", memberInformation.Name));
                    }
                }

                // Call the "OnDeserialized" method if any:
                CallOnDeserializedMethod(resultInstance, resultType);

                return resultInstance;
            }
            else
            {
                return null;
            }
        }

        static void CallOnDeserializingMethod(object obj, Type objType)
        {
            DataContractSerializer_Helpers.CallMethod(obj, objType, typeof(OnDeserializingAttribute), TypeToOnDeserializingMethodCache);
        }

        static void CallOnDeserializedMethod(object obj, Type objType)
        {
            DataContractSerializer_Helpers.CallMethod(obj, objType, typeof(OnDeserializedAttribute), TypeToOnDeserializedMethodCache);
        }

        // DO NOT MOVE!!! This method is called by reflection. If you move it, make sure to move the caller.
        internal static IList CreateNewInstanceOfGenericList<T>()
        {
            //-----------------
            // IMPORTANT: This method is called by reflection. If you move it, make sure to move the caller.
            //-----------------

            return new List<T>();
        }
    }
}
