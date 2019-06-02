
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



#if !BRIDGE
using JSIL.Meta;
#else
using Bridge;
#endif

using CSHTML5;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Collections;
using CSHTML5.Internal;
using DotNetForHtml5.Core;

namespace System.Runtime.Serialization
{
    internal static class DataContractSerializer_Helpers
    {
        internal const string DATACONTRACTSERIALIZER_OBJECT_DEFAULT_NAMESPACE = "http://schemas.datacontract.org/2004/07/";
        internal const string XMLSCHEMA_NAMESPACE = "http://www.w3.org/2001/XMLSchema-instance"; // Usually associated to the "xsi:" prefix.
        internal const string XMLSCHEMA_NAMESPACE_XSD = "http://www.w3.org/2001/XMLSchema"; // Usually associated to the "xsd:" prefix.
        internal const string XMLSCHEMA_NAMESPACE_TYPES = "http://microsoft.com/wsdl/types/";
        internal const string XMLNS_NAMESPACE = "http://www.w3.org/2000/xmlns/"; //this is the namespace that is meant when writing "xmlns:XXX". This could (should) be moved to XNamespace since it is normally there.

        //Note: the two strings below are used to force IE to keep the Prefixes that are needed to tell where to find the types (i:type="Prefix567333:MyType) in the ToString.
        //      We had to do this since (in js) new XMLSerializer().serializeToString(element.INTERNAL_jsnode) would remove the "useless" attributes from the serialization (useless from the point of vue of the XML, but not from ours).
        internal const string ATTRIBUTE_TO_REMOVE_FROM_XMLSERIALIZATION_NAME = "AttributeToRemoveFromXMLSerialization"; //Note: if we modify this, remember to modify in XNode.ToString as well. We could not use this variable in that Interop since it would add unwanted quotation marks around it.
        internal const string ATTRIBUTE_TO_REMOVE_FROM_XMLSERIALIZATION_VALUE = "Remove this from the XMLSerialization";// Note: this one whould be alright since the thing we want in the Interop needs quotation marks (referring to the note above).
        

        private static HashSet2<Type> _typesWithOmittedSerializableAttribute;//Note: this HashSet is here because we cannot add the SerializableAttribute to certain types because of JSIL limitations (for example structs attributes are ignored).
        //I also do not know how to add it properly to a class declared directly in JS (what I tried didn't work on Tuple).
        internal static HashSet2<Type> INTERNAL_TypesWithOmittedSerializableAttribute
        {
            get
            {
                if (_typesWithOmittedSerializableAttribute == null)
                {
                    _typesWithOmittedSerializableAttribute = new HashSet2<Type>()
                    {
                        typeof(KeyValuePair<,>),
                        typeof(Tuple<>),
                        typeof(Tuple<,>),
                        typeof(Tuple<,,>),
                        typeof(Tuple<,,,>),
                        typeof(Tuple<,,,,>),
                        typeof(Tuple<,,,,,,>)
                    };
                }
                return _typesWithOmittedSerializableAttribute;
            }
        }


        #region Internal Helpers

        internal static bool IsAssignableToGenericEnumerableOrArray(Type type, out Type itemsType)
        {
            if (type.IsArray)
            {
                itemsType = type.GetElementType();
                if(IsNullOrUndefined(itemsType))
                {
                    //this happens (at least) on arrays that are root in JSIL.
                    //we'll simply assume this is an Object Array for now and change it if it doesn't work.
                    itemsType = typeof(object);
                }
                return true;
            }
            else if (type == typeof(string))
            {
                itemsType = null;
                return false;
            }
            else
            {
                Type genericEnumerable = null;

                // Check if the type is itself the generic enumerable "IEnumerable<>":
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    genericEnumerable = type;
                }
                // Check if the type implements "IEnumerable<>":
                else
                {
#if BRIDGE
                    if (Interop.IsRunningInTheSimulator) //todo: fix me (BadImageFormatException when type is from another assembly)
                    {
                        genericEnumerable = INTERNAL_Simulator.SimulatorProxy.GetInterface((object)type, "IEnumerable`1");
                    }
                    else
                    {
                        genericEnumerable = GetInterface(type, typeof(IEnumerable<>).Name);
                    }
#else
                    genericEnumerable = GetInterface(type, typeof(IEnumerable<>).Name);
#endif
                }

                // If success, we get the type of the "T" in "IEnumerable<T>":
                if (genericEnumerable != null)
                {
                    Type[] genericArguments = genericEnumerable.GetGenericArguments();
                    if (genericArguments.Length > 0)
                    {
                        itemsType = genericArguments[0];
                        return true;
                    }
                }
            }

            itemsType = null;
            return false;
        }

        /*
        // Note: the code below was commented and replaced by another method because in some
        // cases the call to "IsAssignableFrom" let to an exception due to an issue in the
        // JS implementation in the JSIL libraries (reference: CSHTML5 tickets #623 and #648).
          
        internal static bool IsAssignableToGenericEnumerable(Type genericType, Type itemsType)
        {
            var enumerableType = typeof(IEnumerable<>);
            var constructedEnumerableType = enumerableType.MakeGenericType(itemsType);
            return constructedEnumerableType.IsAssignableFrom(genericType);
        }
        */

        internal static bool CheckIfObjectIsNullNullable(object obj)
        {
            Type type = obj.GetType();
            if (type.FullName.StartsWith("System.Nullable`1"))
            {
                //I guess we'll have to use reflection here
                return !CheckIfNullableIsNotNull(obj);
            }
            else
            {
                return false;
            }
        }

#if !BRIDGE
        [JSIL.Meta.JSReplacement("$obj.hasValue")]
#else
        [Template("{obj}.hasValue")]
#endif
        internal static bool CheckIfNullableIsNotNull(object obj)
        {
            return (obj != null);
        }

        /// <summary>
        /// If the given namespace already has an assigned prefix, returns that prefix,
        /// otherwise, generates a unique, unused prefix, defines it in the element, then returns the prefix.
        /// </summary>
        /// <param name="element">The XElement in which to define the prefix.</param>
        /// <param name="namespaceName">The namespace name for which to define the prefix.</param>
        /// <returns></returns>
        internal static string GenerateUniqueNamespacePrefixIfNeeded(XElement element, string namespaceName)
        {
            //first we check if there is already a prefix for the namespace:
            string prefix = element.GetPrefixOfNamespace(namespaceName);

            if (!string.IsNullOrWhiteSpace(prefix))
            {
                return prefix;
            }
            else
            {
                // Generate a unique prefix by calculating a hash of the namespaceName, and then incrementing an index we find a prefix that is available:
                int index = 0;
                while (true)
                {
                    // Generate a candidate prefix:
                    prefix = "Prefix" + namespaceName.GetHashCode().ToString() + index.ToString();

                    // Check if the prefix is already used or if it is available for use:
                    if (string.IsNullOrWhiteSpace(element.GetNamespaceOfPrefix(prefix).NamespaceName))
                    {
                        //we found an unused prefix:
                        //we add the prefix definition to the XElement:
                        element.SetAttributeValue(XNamespace.Get(DataContractSerializer_Helpers.XMLNS_NAMESPACE).GetName(prefix), namespaceName);
                        if (INTERNAL_HtmlDomManager.IsInternetExplorer()) //this "if" is so that the prefix is not removed from the string version of the xml by IE when calling XNode.ToString().
                        {
                            element.SetAttributeValue(XNamespace.Get(namespaceName).GetName(prefix + ":" + ATTRIBUTE_TO_REMOVE_FROM_XMLSERIALIZATION_NAME), ATTRIBUTE_TO_REMOVE_FROM_XMLSERIALIZATION_VALUE);
                        }
                        //and we return the prefix so the calling method can use it.
                        return prefix;
                    }
                    index++;
                }
            }
        }

        /// <summary>
        /// Produces a string from an input System.DateTime.
        /// </summary>
        /// <param name="value">A System.DateTime to translate to a string.</param>
        /// <returns>A string representation of the System.DateTime that shows the date and time.</returns>
        internal static string ConvertDateTimeToSerializationString(DateTime value)
        {
            // The following is not used because it always produces the UTC date format. Although the resulting date is the same, the fact that it is UTC means that if the client sends a date to the server in UTC, the "DateTime.Kind" of the received date on the server will be UTC, and if the client asks the date back from the server, this leads to a different result when doing "DateTime.ToString", because "DateTime.ToString" does not take the time zone into account.
            /*
            DateTime dateTimeUtc = (value.Kind == DateTimeKind.Local ? value.ToUniversalTime() : value);
            TimeSpan timeSince1970 = (dateTimeUtc - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            double millisecondsSince1970 = timeSince1970.TotalMilliseconds;
            var jsDate = JSIL.Verbatim.Expression("new Date($0)", millisecondsSince1970);
            string serializedDate = JSIL.Verbatim.Expression("$0.toJSON()", jsDate);
            return serializedDate;
            */

            TimeSpan ts = new TimeSpan(0, 0, 0, 0, (value - value.Date).Milliseconds);
            string tsString = ts.Ticks >= 1000000 ? ts.Ticks.ToString() : "0" + ts.Ticks;
            if (tsString.EndsWith("0000"))
            {
                tsString = tsString.Substring(0, tsString.Length - 4);
            }
            string secondString = "" + value.Second;
            if (secondString.Length == 1)
                secondString = 0 + secondString;
            string minuteString = "" + value.Minute;
            if (minuteString.Length == 1)
                minuteString = 0 + minuteString;
            string hourString = "" + value.Hour;
            if (hourString.Length == 1)
                hourString = 0 + hourString;
            string dayString = "" + value.Day;
            if (dayString.Length == 1)
                dayString = 0 + dayString;
            string monthString = "" + value.Month;
            if (monthString.Length == 1)
                monthString = 0 + monthString;
            string yearString = "" + value.Year;
            while (yearString.Length < 4)
                yearString = 0 + yearString;


            string str = yearString + "-" + monthString + "-" + dayString + "T" + hourString + ":" + minuteString + ":" + secondString;
            if (ts.Ticks != 0)
            {
                str += "." + tsString;
            }

            //we add the Timezone if the DateTimeKind is Local
            if (value.Kind == DateTimeKind.Local)
            {
                TimeSpan changeFromUTC = value - value.ToUniversalTime();
                string minuteStringForTimeZone = "" + changeFromUTC.Minutes;
                if (minuteStringForTimeZone.Length == 1)
                    minuteStringForTimeZone = 0 + minuteStringForTimeZone;
                string hourStringForTimeZone = "" + Math.Abs(changeFromUTC.Hours); //getting the absolute value because it allows us to easily add the 0 in case of a 1 digit hour (so that we get 07 instead of 7 and the '-' doesn't get in the way in case of a negative timezone).
                if (hourStringForTimeZone.Length == 1)
                    hourStringForTimeZone = 0 + hourStringForTimeZone;
                string timeZone = hourStringForTimeZone + ":" + minuteStringForTimeZone;
                if (changeFromUTC.Ticks >= 0)
                {
                    timeZone = "+" + timeZone;
                }
                else
                {
                    timeZone = "-" + timeZone;
                }
                str += timeZone;

            }
            else if (value.Kind == DateTimeKind.Utc)
                str += "Z";
            return str;
        }

        internal static IEnumerable<MemberInformation> GetDataContractMembers(Type type, SerializationType serializationType, bool useXmlSerializerFormat) //todo: remove that note, or change it so that it fits the new situation (isTypeMarkedWithAttributes was replaced with serializationtype)// Note: to better understand "isTypeMarkedWithAttributes", read "Using DataContracts" at: https://msdn.microsoft.com/en-us/library/ms733127(v=vs.100).aspx
        {
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic; // Note: further selection is done in the "ShouldMemberBeConsideredForSerialization" method.

            //todo-perfs: use a cache.

            //-------------------------
            // Get the properties
            //-------------------------
            PropertyInfo[] properties = type.GetProperties(bindingFlags);

            //-------------------------
            // Get the fields
            //-------------------------
            FieldInfo[] fields = type.GetFields(bindingFlags);

            //-------------------------
            // Get the members type and information
            //-------------------------
            Dictionary<Type, List<MemberInformation>> typesToMembersInformations = new Dictionary<Type, List<MemberInformation>>(); //This is to put the properties that come from the same types together since it is important for ordering them (see notes below DataMember2Attribute.Order).
            foreach (PropertyInfo propertyInfo in properties)
            {
                MemberInformation memberInformation = GetDataMemberInformation(propertyInfo);
                memberInformation.MemberType = propertyInfo.PropertyType;
                if (!IsNullOrUndefined(memberInformation.MemberType))
                {
                    MemberInfo memberInfo = memberInformation.MemberInfo;
                    if (ShouldMemberBeConsideredForSerialization(memberInfo, serializationType, useXmlSerializerFormat)) //only keep the members that will actually be serialized
                    {
                        //Add the member to its corresponding declaring type:
                        Type propertyDeclaringType = propertyInfo.DeclaringType;
                        if (!typesToMembersInformations.ContainsKey(propertyDeclaringType))
                        {
                            typesToMembersInformations.Add(propertyDeclaringType, new List<MemberInformation>());
                        }
                        typesToMembersInformations[propertyDeclaringType].Add(memberInformation);
                    }
                }
                else
                {
                    //todo: investigate why we sometimes arrive here in JSIL.
                    Debug.WriteLine("Cannot read 'PropertyType' from the property: " + propertyInfo.DeclaringType.ToString() + "." + propertyInfo.Name);
                }
            }
            foreach (FieldInfo fieldInfo in fields)
            {
                MemberInformation memberInformation = GetDataMemberInformation(fieldInfo);
                memberInformation.MemberType = fieldInfo.FieldType;
                if (!IsNullOrUndefined(memberInformation.MemberType))
                {
                    MemberInfo memberInfo = memberInformation.MemberInfo;
                    if (ShouldMemberBeConsideredForSerialization(memberInfo, serializationType, useXmlSerializerFormat)) //only keep the members that will actually be serialized
                    {
                        //Add the member to its corresponding declaring type:
                        Type fieldDeclaringType = fieldInfo.DeclaringType;
                        if (!typesToMembersInformations.ContainsKey(fieldDeclaringType))
                        {
                            typesToMembersInformations.Add(fieldDeclaringType, new List<MemberInformation>());
                        }
                        typesToMembersInformations[fieldDeclaringType].Add(memberInformation);
                    }
                }
                else
                {
                    //todo: investigate why we sometimes arrive here in JSIL.
                    Debug.WriteLine("Cannot read 'FieldType' from the field: " + fieldInfo.DeclaringType.ToString() + "." + fieldInfo.Name);
                }
            }

            //-------------------------
            // Sort the members and return the result:
            //-------------------------
            return SortMembers(typesToMembersInformations);
        }

#region to help sort the members for the Serialization
        /// <summary>
        /// returns a List&lt;MemberInformation&gt; with the memberInformations ordered to fit the requirements of Microsoft's DataContractSerializer (see notes below DataMember2Attribute.Order).
        /// </summary>
        /// <param name="membersUnitedByDeclaringType">A dictionary containing the memberInformations with their DeclaringType as Keys.</param>
        /// <returns>A List&lt;MemberInformation&gt; with the memberInformations ordered to fit the requirements of Microsoft's DataContractSerializer.</returns>
        static IEnumerable<MemberInformation> SortMembers(Dictionary<Type, List<MemberInformation>> membersUnitedByDeclaringType)
        {
            //first we need to order the types:
            List<Type> orderedTypes = membersUnitedByDeclaringType.Keys.ToList();
            orderedTypes.Sort((t1, t2) => t1.IsAssignableFrom(t2) ? -1 : 1);

            //we add the members to a new list following the order of the types:
            List<MemberInformation> resultList = new List<MemberInformation>();
            foreach (Type currentType in orderedTypes)
            {
                //sort the elements in the dictionary and add them to the result list:
                var sortedResultForType = membersUnitedByDeclaringType[currentType].ToList();
                sortedResultForType.Sort(CompareMembers);
                resultList.AddRange(sortedResultForType);
            }
            return resultList;
        }

        static int CompareMembers(MemberInformation member1, MemberInformation member2)
        {
            if(member1.Order > member2.Order)
            {
                return 1;
            }
            else if (member1.Order < member2.Order)
            {
                return -1;
            }
            else
            {
                return member1.Name.CompareTo(member2.Name);
            }
        }

#endregion

        internal static IEnumerable<MemberInformationAndValue> GetDataContractMembersAndValues(object instance, SerializationType serializationType, bool useXmlSerializerFormat) // Note: to better understand "isTypeMarkedWithAttributes", read "Using DataContracts" at: https://msdn.microsoft.com/en-us/library/ms733127(v=vs.100).aspx
        {
            Type instanceType = instance.GetType();
            IEnumerable<MemberInformation> membersInformation = GetDataContractMembers(instanceType, serializationType, useXmlSerializerFormat);

            //-------------------------
            // Read the values
            //-------------------------
            List<MemberInformationAndValue> resultList = new List<MemberInformationAndValue>();
            foreach (MemberInformation memberInformation in membersInformation)
            {
                object value = null;
                bool successGettingTheValue = false;
                if (memberInformation.MemberInfo is PropertyInfo)
                {
                    //------------------
                    // Read the property
                    //------------------
                    PropertyInfo propertyInfo = (PropertyInfo)memberInformation.MemberInfo;
                    if (propertyInfo.CanRead)
                    {
                        value = propertyInfo.GetValue(instance, null);
                        successGettingTheValue = true;
                    }
                }
                else if (memberInformation.MemberInfo is FieldInfo)
                {
                    //------------------
                    // Read the field
                    //------------------
                    value = ((FieldInfo)memberInformation.MemberInfo).GetValue(instance);
                    successGettingTheValue = true;
                }
                else if (memberInformation.MemberInfo == null)
                {
                    throw new Exception("MemberInfo is null.");
                }
                else
                {
                    throw new Exception("The MemberInfo type is not supported.");
                }
                if (successGettingTheValue)
                {
                    // Check if the value is the default value, in which case the memmber should be emitted only if "EmitDefaultValue" is true.
                    if (memberInformation.EmitDefaultValue
                        || !IsValueTheDefault(value, memberInformation.MemberType))
                    {
                        MemberInformationAndValue memberInformationAndValue = new MemberInformationAndValue(memberInformation, value);
                        resultList.Add(memberInformationAndValue);
                    }
                }
            }

            return resultList;
        }

        internal static void SetMemberValue(object instance, MemberInformation memberInformation, object memberValue)
        {
            if (memberInformation.MemberInfo is PropertyInfo)
            {
                PropertyInfo propertyInfo = (PropertyInfo)memberInformation.MemberInfo;
                if (propertyInfo.CanWrite)
                {
#if BRIDGE
                    propertyInfo.SetValue(instance, memberValue); //Note: in Bridge, keeping null as the third parameter breaks (at least) arrays of strings: the value of the property becomes the first element of the array instead of the array itself.
#else
                    propertyInfo.SetValue(instance, memberValue, null); 
#endif
                }
            }
            else if (memberInformation.MemberInfo is FieldInfo)
            {
                ((FieldInfo)memberInformation.MemberInfo).SetValue(instance, memberValue);
            }
            else if (memberInformation.MemberInfo == null)
            {
                throw new Exception("MemberInfo is null.");
            }
            else
            {
                throw new Exception("The MemberInfo type is not supported.");
            }
        }

        internal static object GetMemberValue(object instance, MemberInformation memberInformation)
        {
            if (memberInformation.MemberInfo is PropertyInfo)
            {
                PropertyInfo propertyInfo = (PropertyInfo)memberInformation.MemberInfo;
                if (propertyInfo.CanRead)
                {
                    return propertyInfo.GetValue(instance);
                }
            }
            else if (memberInformation.MemberInfo is FieldInfo)
            {
                return ((FieldInfo)memberInformation.MemberInfo).GetValue(instance);
            }
            else if (memberInformation.MemberInfo == null)
            {
                throw new Exception("MemberInfo is null.");
            }
            else
            {
                throw new Exception("The MemberInfo type is not supported.");
            }

            return null;
        }

        internal static MemberInformation GetDataMemberInformation(MemberInfo memberInfo1)
        {
            bool emitDefaultValue = true;
            bool isRequired = false;
            string name = null;
            int order = -1;
            bool hasXmlElementAttribute = false;
            bool hasXmlAttributeAttribute = false; // Note: repetition of the word "Attribute" is intended.
            foreach (var memberInfo in GetMemberInheritanceChain(memberInfo1))
            {
                foreach (Attribute attr in memberInfo.GetCustomAttributes(false))
                {
                    if (attr is DataMember2Attribute)
                    {
                        DataMember2Attribute attribute = (DataMember2Attribute)attr;
                        emitDefaultValue = attribute.EmitDefaultValue;
                        isRequired = attribute.IsRequired;
                        if (!string.IsNullOrWhiteSpace(attribute.Name))
                        {
                            name = attribute.Name;
                        }
                        order = attribute.Order;
                    }
                    else if (attr is XmlElementAttribute)
                    {
                        XmlElementAttribute attribute = (XmlElementAttribute)attr;
                        hasXmlElementAttribute = true;
                        if (!string.IsNullOrWhiteSpace(attribute.ElementName))
                        {
                            name = attribute.ElementName;
                        }
                        order = attribute.Order;
                    }
                    else if (attr is XmlAttributeAttribute)
                    {
                        XmlAttributeAttribute attribute = (XmlAttributeAttribute)attr;
                        hasXmlAttributeAttribute = true;
                        if (!string.IsNullOrWhiteSpace(attribute.AttributeName))
                        {
                            name = attribute.AttributeName;
                        }
                    }
                }
            }
            if (name == null)
            {
                name = memberInfo1.Name;
            }
            return new MemberInformation(memberInfo1, emitDefaultValue: emitDefaultValue, isRequired: isRequired, name: name, order: order, hasXmlElementAttribute: hasXmlElementAttribute, hasXmlAttributeAttribute: hasXmlAttributeAttribute);
        }

        private static IEnumerable<MemberInfo> GetMemberInheritanceChain(MemberInfo memberInfo)
        {
            // Note: this method is here to fix the issue that was that it was not possible
            // to deserialize properties that where defined as "overridden" where the
            // [DataMember2] attribute was in the base abstract class, because the
            // DataContractSerializer did not see that attribute. The "inherits" flag of
            // the "MethodInfo.GetCustomAttributes" method does not work with properties and
            // fields, as also documented by MS:
            // https://msdn.microsoft.com/fr-fr/library/kff8s254(v=vs.110).aspx
            // We could not use "Attribute.GetCustomAttributes(inherits)" because JSIL did
            // not support it. So we wrote the method above, which works fine in the
            // Simulator. We then realized that a bug of JSIL caused the "Type.GetProperties"
            // to also return the base properties, so in JS this method is not very useful.
            //todo: this method will become useful when JSIL will be replaced.

            yield return memberInfo;

            //Note: The following block is only needed in the Simulator because "GetProperties" in JSIL already returns parent properties too.
            if (Interop.IsRunningInTheSimulator)
            {
                Type baseType = memberInfo.DeclaringType;
                Type objectType = typeof(object);
                string memberName = memberInfo.Name;
                bool isProperty = (memberInfo is PropertyInfo);
                bool isField = (memberInfo is FieldInfo);

                // Walk up the inheritance chain:
                while ((baseType = baseType.BaseType) != null
                    && (baseType != objectType))
                {
                    MemberInfo baseMemberInfo;
                    if (isProperty)
                    {
                        baseMemberInfo = baseType.GetProperty(memberName);
                    }
                    else if (isField)
                    {
                        baseMemberInfo = baseType.GetField(memberName);
                    }
                    else
                    {
                        break;
                    }
                    if (!object.Equals(baseMemberInfo, null))
                    {
                        yield return baseMemberInfo;
                    }
                }
            }
        }

        internal static bool IsElementNil(XElement element) //todo: what if it is not nullable?
        {
            foreach (XAttribute attribute in element.Attributes())
            {
                if (attribute.Name.NamespaceName == XMLSCHEMA_NAMESPACE
                    && attribute.Name.LocalName == "nil"
                    && attribute.Value.ToLower() == "true")
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Useful to call the methods "OnDeserializing, OnDeserialized, OnSerializing and OnSerialized.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="objType"></param>
        /// <param name="attributeType"></param>
        /// <param name="methodsCache"></param>
        internal static void CallMethod(object obj, Type objType, Type attributeType, Dictionary<Type, MethodInfo> methodsCache)
        {
            MethodInfo methodIfAny;

            // We look for the method in the cache for performance optimization:
            if (methodsCache.ContainsKey(objType))
            {
                methodIfAny = methodsCache[objType];
            }
            else
            {
                BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
#if BRIDGE
                MethodInfo[] instanceMethods = INTERNAL_BridgeWorkarounds.TypeGetMethods_SimulatorCompatible(objType, bindingFlags);
                methodIfAny =
                    instanceMethods
                    .Where(m => m.GetCustomAttributes(attributeType, false).Length > 0)
                    .FirstOrDefault();
#else
                methodIfAny = objType
                    .GetMethods(bindingFlags)
                    .Where(m => m.GetCustomAttributes(attributeType, false).Length > 0)
                    .FirstOrDefault();
#endif
                // We add it to the cache, even if it was not found, to avoid future lookup for performance optimization:
                methodsCache.Add(objType, methodIfAny);
            }

            // Call the method:
            if (methodIfAny != null)
            {
                methodIfAny.Invoke(obj, new object[] { null });
            }
        }

        internal static string GetTypeNameSafeForSerialization(Type type)
        {
#if !BRIDGE
            bool isRunningUnderJSIL = !CSHTML5.Interop.IsRunningInTheSimulator; //todowasm: fix this when running in WebAssembly
#else
            bool isRunningUnderJSIL = false;
#endif

            if (isRunningUnderJSIL)
            {
                // Workaround for JSIL because JSIL does not handle Type.IsNested and Type.DeclaringType (at least in the case of nested types)
                return JSIL_Workaround_GetTypeNameSafeForSerialization(type);
            }
            else
            {
                //In case of nested types, replace the '+' with '.'
                string typeName = MakeGenericTypeReadyForSerialization(type);
                Type currentType = type;

                while (currentType.IsNested)
                {
                    currentType = currentType.DeclaringType;

                    typeName = MakeGenericTypeReadyForSerialization(currentType) + "." + typeName;
                }

                return typeName;
            }
        }

        static string MakeGenericTypeReadyForSerialization(Type type)
        {
            string readableTypeName;
            string typeName = type.Name;
            
            //with JSIL, objectType.Name for nested types returns "ParentType+NestedType" while in c#, we only get "NestedType", so we correct it:
            int lastIndexOfPlusSign = typeName.LastIndexOf('+');
            if (lastIndexOfPlusSign > -1)
            {
                typeName = typeName.Substring(lastIndexOfPlusSign + 1);
            }

            //now we are certain to only have the current type and not its nested parent(s) in the string.
            if (type.IsGenericType)
            {
                //if the type is generic, we remove the "`N" part and put "Of" instead:
                readableTypeName = typeName.Substring(0, typeName.IndexOf('`')) + "Of";

#if !BRIDGE
                foreach (Type typeInGenericTypeArguments in type.GenericTypeArguments)
#else
                // TODOBRIDGE: verify if the two code are similar
                foreach (Type typeInGenericTypeArguments in type.GetGenericArguments())
#endif
                {
                    if (DataContractSerializer_ValueTypesHandler.TypesToNames.ContainsKey(typeInGenericTypeArguments))
                    {
                        readableTypeName += DataContractSerializer_ValueTypesHandler.TypesToNames[typeInGenericTypeArguments];
                    }
                    else
                    {
                        readableTypeName += MakeGenericTypeReadyForSerialization(typeInGenericTypeArguments);
                    }
                }
            }
            else
            {
                readableTypeName = type.Name;
            }
            return readableTypeName;
        }

        static string JSIL_Workaround_GetTypeNameSafeForSerialization(Type type)
        {
            string readableTypeName = "";
            string typeName = type.Name;
            if (!typeName.Contains('`'))
            {
                //Note: this is the easy one, there is no Generic type in the whole thing.
                readableTypeName = typeName.Replace('+', '.');
            }
            else
            {
                dynamic currentType = type;
                //we separate all the DeclaringTypes:
                string currentTypeFullName = type.FullName;

                //we get the assembly (directly in JS because it is probably more efficient that way) from the type:
                object assembly = CSHTML5.Interop.ExecuteJavaScript("$0.Assembly", type); //Note: we will not update this as we go through the declaring types since there is no way it is a different assembly.
                string separatorToAdd = "";
                while (!string.IsNullOrWhiteSpace(currentTypeFullName))
                {
                    //make the type readable:
                    string currentTypeName = currentType.Name;
                    if (currentType.IsGenericType)
                    {
                        currentTypeName = currentTypeName.Substring(currentTypeName.IndexOf('+') + 1); //to get rid of everything that precedes the last '+'
                        currentTypeName = currentTypeName.Substring(0, currentTypeName.IndexOf('`')) + "Of"; //to change MyType`N into MyTypeOf (and then add the types)
                        //we add the generic type arguments to the name:

#if !BRIDGE
                        foreach (Type typeInGenericTypeArguments in type.GenericTypeArguments)
#else
                        foreach (Type typeInGenericTypeArguments in type.GetGenericArguments())
#endif
                        {
                            if (DataContractSerializer_ValueTypesHandler.TypesToNames.ContainsKey(typeInGenericTypeArguments))
                            {
                                currentTypeName += DataContractSerializer_ValueTypesHandler.TypesToNames[typeInGenericTypeArguments];
                            }
                            else
                            {

                                currentTypeName += JSIL_Workaround_GetTypeNameSafeForSerialization(typeInGenericTypeArguments);
                            }
                        }
                        readableTypeName = currentTypeName + separatorToAdd + readableTypeName;
                    }
                    else
                    {
                        //the current type is not generic so we only need to copy it into readableTypeName
                        readableTypeName = currentTypeName.Substring(currentTypeName.IndexOf('+') + 1) + separatorToAdd + readableTypeName;
                    }

                    //go to the next type:
                    //if the type is generic, it's type.FullName is: what we want, followed by brackets containing the Generic types names.
                    //we remove the brackets part:
                    currentTypeFullName = currentTypeFullName.Substring(0, currentTypeFullName.IndexOf('['));

                    //get the fullName of the next type from the string:
                    int indexOfLastPlusSign = currentTypeFullName.LastIndexOf('+');

                    //we remove the part we just did from the type full name string:
                    currentTypeFullName = currentTypeFullName.Substring(0, indexOfLastPlusSign);

                    //get the type from the assembly:
                    if (string.IsNullOrWhiteSpace(currentTypeFullName))
                    {
                        break;
                    }
                    currentType = CSHTML5.Interop.ExecuteJavaScript("$0.GetType($1)", assembly, currentTypeFullName);
                    separatorToAdd = ".";
                }
            }
            return readableTypeName;
        }

        internal static TypeInformation GetTypeInformationByReadingAttributes(Type objectType, string nodeDefaultNamespaceIfAny)
        {
            //todo-perfs: add a cache.

            // Set defaults:
            string namespaceName;// = nodeDefaultNamespaceIfAny ?? DATACONTRACTSERIALIZER_OBJECT_DEFAULT_NAMESPACE + objectType.Namespace;
            if (objectType.Namespace == "System.Collections.Generic" || objectType.Namespace == "") //todo: whenever we meet a new thing that doesn't put its namespace in the serialization, try to find what they have in common and change this.
            {
                namespaceName = nodeDefaultNamespaceIfAny;
            }
            else
            {
                namespaceName = DATACONTRACTSERIALIZER_OBJECT_DEFAULT_NAMESPACE + objectType.Namespace;
            }
            string name = objectType.Name; //note: we might need to make the name safe for serialization by calling DataContractSerializer_Helpers.GetTypeNameSafeForSerialization(objectType);
            string itemName = null;
            string keyName = null;
            string valueName = null;
            SerializationType serializationType = SerializationType.Default;

            Type objectTypeForComparison = objectType.IsGenericType ? objectType.GetGenericTypeDefinition() : objectType;
            //we check if the type is one with a broken SerializableAttribute:
            if (INTERNAL_TypesWithOmittedSerializableAttribute.Contains(objectTypeForComparison))
            {
                //this means we should consider that the type has the SerializableAttribute attribute.
                serializationType = SerializationType.Serializable;
            }
            else
            {

                // Look for the attributes:
                foreach (Attribute attribute in objectType.GetCustomAttributes(true))
                {
                    if (attribute is DataContractAttribute)
                    {
                        serializationType = SerializationType.DataContract;

                        DataContractAttribute attributeAsDataContractAttribute = (DataContractAttribute)attribute;
                        if (!string.IsNullOrWhiteSpace(attributeAsDataContractAttribute.Namespace))
                        {
                            namespaceName = attributeAsDataContractAttribute.Namespace;
                        }
                        if (!string.IsNullOrWhiteSpace(attributeAsDataContractAttribute.Name))
                        {
                            name = attributeAsDataContractAttribute.Name;
                        }
                        //Note: line below commented because we need to find DataContract2Attribute since DataContractAttribute does not have the values set in JSIL
                        //break; //break here because DataContract takes precedence over Serializable.
                    }
                    else if (attribute is DataContract2Attribute) //todo: remove "DataContract2Attribute" and keep only "DataContractAttribute" when JSIL will support setting attribute properties such as Name and Namespace.
                    {
                        serializationType = SerializationType.DataContract;

                        DataContract2Attribute attributeAsDataContractAttribute = (DataContract2Attribute)attribute;
                        if (!string.IsNullOrWhiteSpace(attributeAsDataContractAttribute.Namespace))
                        {
                            namespaceName = attributeAsDataContractAttribute.Namespace;
                        }
                        if (!string.IsNullOrWhiteSpace(attributeAsDataContractAttribute.Name))
                        {
                            name = attributeAsDataContractAttribute.Name;
                        }
                        break; //break here because DataContract takes precedence over Serializable.
                    }
                    else if (attribute is CollectionDataContractAttribute)
                    {
                        serializationType = SerializationType.DataContract;

                        CollectionDataContractAttribute collectionDataContractAttribute = (CollectionDataContractAttribute)attribute;
                        if (!string.IsNullOrWhiteSpace(collectionDataContractAttribute.Namespace))
                        {
                            namespaceName = collectionDataContractAttribute.Namespace;
                        }
                        if (!string.IsNullOrWhiteSpace(collectionDataContractAttribute.Name))
                        {
                            name = collectionDataContractAttribute.Name;
                        }
                        if (!string.IsNullOrWhiteSpace(collectionDataContractAttribute.ItemName))
                        {
                            itemName = collectionDataContractAttribute.ItemName;
                        }
                        if (!string.IsNullOrWhiteSpace(collectionDataContractAttribute.KeyName))
                        {
                            itemName = collectionDataContractAttribute.KeyName;
                        }
                        if (!string.IsNullOrWhiteSpace(collectionDataContractAttribute.ValueName))
                        {
                            itemName = collectionDataContractAttribute.ValueName;
                        }
                        //Note: line below commented because we need to find CollectionDataContract2Attribute since CollectionDataContractAttribute does not have the values set in JSIL
                        //break; //break here because DataContract takes precedence over Serializable.
                    }
                    else if (attribute is CollectionDataContract2Attribute) //todo: remove "CollectionDataContract2Attribute" and keep only "CollectionDataContractAttribute" when JSIL will support setting attribute properties such as Name and Namespace.
                    {
                        serializationType = SerializationType.DataContract;

                        CollectionDataContract2Attribute collectionDataContractAttribute = (CollectionDataContract2Attribute)attribute;
                        if (!string.IsNullOrWhiteSpace(collectionDataContractAttribute.Namespace))
                        {
                            namespaceName = collectionDataContractAttribute.Namespace;
                        }
                        if (!string.IsNullOrWhiteSpace(collectionDataContractAttribute.Name))
                        {
                            name = collectionDataContractAttribute.Name;
                        }
                        if (!string.IsNullOrWhiteSpace(collectionDataContractAttribute.ItemName))
                        {
                            itemName = collectionDataContractAttribute.ItemName;
                        }
                        if (!string.IsNullOrWhiteSpace(collectionDataContractAttribute.KeyName))
                        {
                            itemName = collectionDataContractAttribute.KeyName;
                        }
                        if (!string.IsNullOrWhiteSpace(collectionDataContractAttribute.ValueName))
                        {
                            itemName = collectionDataContractAttribute.ValueName;
                        }
                        break; //break here because DataContract takes precedence over Serializable.
                    }
                    else if (attribute is SerializableAttribute)
                    {
                        if (serializationType == SerializationType.Default)
                        {
                            serializationType = SerializationType.Serializable;
                        }
                    }
                    else if (attribute is Serializable2Attribute) //todo: remove "Serializable2Attribute" and keep only "SerializableAttribute" when JSIL will support setting attribute properties such as Name and Namespace.
                    {
                        if (serializationType == SerializationType.Default)
                        {
                            serializationType = SerializationType.Serializable;
                        }
                    }
                }
            }

            return new TypeInformation(objectType, name, namespaceName, itemName, keyName, valueName, serializationType);
        }

        internal static object GetDefault(Type type)
        {
            if (type.IsValueType && !IsNullable(type))
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        /*
        internal static bool TryGetCustomAttribute<ATTRIBUTE_TYPE>(Type typeToLookInto, bool inherit, out ATTRIBUTE_TYPE attribute)
            where ATTRIBUTE_TYPE : Attribute
        {
            foreach (Attribute attr in typeToLookInto.GetCustomAttributes(typeof(ATTRIBUTE_TYPE), inherit))
            {
                if (attr is ATTRIBUTE_TYPE)
                {
                    attribute = (ATTRIBUTE_TYPE)attr;
                    return true;
                }
            }
            attribute = null;
            return false;
        }
        */

        internal static bool DoesTypeHaveConstructorWithParameter(Type type, Type typeOfTheParameter)
        {

#if !BRIDGE
            ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
#else

            // TODOBRIDGE: verify if the two code are similar (especially constructorInfo.IsPublic && !constructorInfo.IsStatic )

            ConstructorInfo[] constructors = type.GetConstructors();

            List<ConstructorInfo> constructorsList = new List<ConstructorInfo>();

            foreach (ConstructorInfo constructorInfo in constructors)
            {
                if (
#if BRIDGE
                    INTERNAL_BridgeWorkarounds.ConstructorInfoIsPublic_SimulatorCompatible(constructorInfo)
#else
                    constructorInfo.IsPublic
#endif
                    &&
#if BRIDGE
                    !INTERNAL_BridgeWorkarounds.ConstructorInfoIsStatic_SimulatorCompatible(constructorInfo)
#else
                    !constructorInfo.IsStatic
#endif
                    )
                    constructorsList.Add(constructorInfo);
            }

            constructors = constructorsList.ToArray();
#endif

                    foreach (ConstructorInfo constructorInfo in constructors)
            {
                ParameterInfo[] parameters = constructorInfo.GetParameters();
                if (parameters.Length == 1)
                {
                    ParameterInfo parameterInfo = parameters[0];
                    if (parameterInfo.ParameterType.IsAssignableFrom(typeOfTheParameter))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal static Type GetInterface(Type type, string name)
        {
            // Note: this method is here because "Type.GetInterface(name)" is not yet
            // available in the JSIL libraries as of July 14, 2017.
            // When available, this method can be replaced with "Type.GetInterface(name)".

            foreach (Type theInterface in type.GetInterfaces())
            {
                if (theInterface.Name == name)
                    return theInterface;
            }
            return null;
        }

#endregion


#region Private Helpers

        static bool ShouldMemberBeConsideredForSerialization(MemberInfo memberInfo, SerializationType serializationType, bool useXmlSerializerFormat)
        {
            // Reference: https://msdn.microsoft.com/en-us/library/ms733127(v=vs.100).aspx

            bool shouldSerialize;

            if (serializationType == SerializationType.Serializable) //todo: see if the XmlSerialier takes that attribute into consideration, if not, we may want to add something to differentiate between Silverlight and XmlSerializer (in the project StratX star, useXmlSerializerFormat is set to true for some reason when the original thing uses the DataContractSerializer).
            {
                //the class we are serializing has the SerializableAttribute attribute:
                //we accept any field that is not static and that hasn't the NonSerialized attribute. (Note: FIELDS are accepted, PROPERTIES are not).
                if (memberInfo is FieldInfo)
                {
                    shouldSerialize = true;
                    var fieldInfo = (FieldInfo)memberInfo;
#if BRIDGE
                    if (INTERNAL_BridgeWorkarounds.FieldInfoIsStatic_SimulatorCompatible(fieldInfo))
#else
                    if (fieldInfo.IsStatic)
#endif
                    {
                        shouldSerialize = false;
                    }
                    if (shouldSerialize) //if the field is not static
                    {
                        foreach (Attribute attribute in fieldInfo.GetCustomAttributes(false))
                        {

                            // for some reason, Bridge can't use the NonSerializedAttribute type 
                            // (Bridge.Contract.BridgeTypes.ToJsName : Type System.NonSerializedAttribute is marked as not usable from script  )
#if !BRIDGE
                            if (attribute is NonSerializedAttribute)
#else
                            if (attribute.GetType().Name == "NonSerializedAttribute")
#endif
                            {
                                shouldSerialize = false;
                                break;
                            }

                            //todo: we probably also need a NonSerialized2Attribute class (because I think it doesn't work either)
                            //      and to change the INTERNAL_TypesWithOmittedSerializableAttribute into a Dictionary<Type,List<FieldInfo>> or something like that
                            //          to get an access to which fields should be serialized or not (probably better to have the fields we do not want to serialize
                            //          in the dictionary so that we do not have to list all the fields all the time).
                        }
                    }
                }
                else
                {
                    shouldSerialize = false;
                }
            }
            else if (serializationType == SerializationType.Default || useXmlSerializerFormat)
            {
                //------------------------------------------
                // If we arrive here, it means that either there is no [DataContract] attribute on the type, or we are using the XmlSerializer instead of the DataContractSerializer.
                // In both cases, we should only serialize public members.
                //------------------------------------------

                if (memberInfo is PropertyInfo)
                {
                    //we have either no serialization-related attributes on the class or we are using the XmlSerializer.
                    var propertyInfo = (PropertyInfo)memberInfo;

#if !BRIDGE
                    var getter = propertyInfo.GetGetMethod();
                    var setter = propertyInfo.GetSetMethod();
#else
                    var getter = propertyInfo.GetMethod;
                    var setter = propertyInfo.SetMethod;
#endif

                    shouldSerialize =
                        getter != null // When "GetGetMethod" is not null, it means that the Getter exists, and it is public.
#if BRIDGE
                        && !INTERNAL_BridgeWorkarounds.MethodInfoIsStatic_SimulatorCompatible(getter)
#else
                        && !getter.IsStatic
#endif
                        && setter != null // When "GetSetMethod" is not null, it means that the Setter exists, and it is public.
#if BRIDGE
                        && !INTERNAL_BridgeWorkarounds.MethodInfoIsStatic_SimulatorCompatible(setter);
#else
                        && !setter.IsStatic;
#endif
                }
                else if (memberInfo is FieldInfo)
                {
                    var fieldInfo = (FieldInfo)memberInfo;
                    shouldSerialize =
#if BRIDGE
                        INTERNAL_BridgeWorkarounds.FieldInfoIsPublic_SimulatorCompatible(fieldInfo) && !INTERNAL_BridgeWorkarounds.FieldInfoIsStatic_SimulatorCompatible(fieldInfo);
#else
                        fieldInfo.IsPublic && !fieldInfo.IsStatic;
#endif
                }
                else
                    throw new Exception("Unsupported member type.");
            }
            else
            {
                //------------------------------------------
                // If we arrive here, it means that the class has the [DataContract] attribute.
                // In that case, we should only serialize members with the [DataMember] attribute.
                //------------------------------------------

                shouldSerialize = DoesMemberHaveTheDataMemberAttribute(memberInfo);
            }


            return shouldSerialize;
        }

        static bool DoesMemberHaveTheDataMemberAttribute(MemberInfo memberInfo1)
        {
            //todo: when supported by JSIL, replace the code with the method "IsDefined", such as: "memberInfo.IsDefined(typeof(DataMemberAttribute), false))"  IMPORTANT: verify that it still works fine with abstract inherited properties though.

            foreach (MemberInfo memberInfo in GetMemberInheritanceChain(memberInfo1))
            {
                foreach (Attribute attribute in memberInfo.GetCustomAttributes(false))
                {
                    if (attribute is DataMemberAttribute
                        || attribute is DataMember2Attribute)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        static bool IsValueTheDefault(object value, Type type)
        {
            if (type.IsValueType && !IsNullable(type))
            {
                if (value == null)
                {
                    return false;
                }
                else
                {
                    return value.Equals(Activator.CreateInstance(type));
                }
            }
            else
            {
                return (value == null);
            }
        }

        static bool IsNullable(Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        static bool IsNullOrUndefined(object obj)
        {
            if (Interop.IsRunningInTheSimulator)
            {
                return obj == null;
            }
            else
            {
                return Convert.ToBoolean(Interop.ExecuteJavaScript("(typeof $0 === 'undefined' || $0 === null)", obj));
            }
        }

#endregion

    }
}
