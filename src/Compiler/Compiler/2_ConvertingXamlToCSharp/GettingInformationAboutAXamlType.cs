

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



#if !BRIDGE && !CSHTML5BLAZOR
extern alias custom;
#endif
extern alias wpf;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Xml.Linq;
#if !BRIDGE && !CSHTML5BLAZOR
using custom::System.Windows.Markup;
#endif

namespace DotNetForHtml5.Compiler
{
    internal static class GettingInformationAboutXamlTypes
    {
        public static bool IsPropertyAttached(XElement propertyElement, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain)
        {
            string namespaceName, localName, assemblyNameIfAny;
            GetClrNamespaceAndLocalName(propertyElement.Name, out namespaceName, out localName, out assemblyNameIfAny);
            if (localName.Contains("."))
            {
                var split = localName.Split('.');
                var typeLocalName = split[0];
                var propertyOrFieldName = split[1];
                string parentNamespaceName, parentLocalTypeName, parentAssemblyIfAny;
                GetClrNamespaceAndLocalName(propertyElement.Parent.Name, out parentNamespaceName, out parentLocalTypeName, out parentAssemblyIfAny);
                return reflectionOnSeparateAppDomain.IsPropertyAttached(propertyOrFieldName, namespaceName, typeLocalName, parentNamespaceName, parentLocalTypeName, assemblyNameIfAny);
            }
            else
                return false;
        }

        public static bool IsPropertyOrFieldACollection(XElement propertyElement, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain, bool isAttachedProperty)
        {
            if (isAttachedProperty)
            {
                string methodName = "Get" + propertyElement.Name.LocalName.Split('.')[1]; // In case of attached property, we check the return type of the method "GetPROPERTYNAME()". For example, in case of "Grid.Row", we check the return type of the method "Grid.GetRow()".
                XName elementName = propertyElement.Name.Namespace + propertyElement.Name.LocalName.Split('.')[0]; // eg. if the propertyElement is <VisualStateManager.VisualStateGroups>, this will be "DefaultNamespace+VisualStateManager"
                string namespaceName, localName, assemblyNameIfAny;
                GetClrNamespaceAndLocalName(elementName, out namespaceName, out localName, out assemblyNameIfAny);
                return reflectionOnSeparateAppDomain.DoesMethodReturnACollection(methodName, namespaceName, localName, assemblyNameIfAny);
            }
            else
            {
                var propertyOrFieldName = propertyElement.Name.LocalName.Split('.')[1];




                //todo: keep the full local name (propertyElement.Name.LocalName) and pass it to the reflectionOnSeparateAppDomain method for the cases of binding on attached properties --> it will be used to get the type of the attached property and the actual name of the property.




                var parentElement = propertyElement.Parent;
                string parentNamespaceName, parentLocalName, parentAssemblyNameIfAny;
                GetClrNamespaceAndLocalName(parentElement.Name, out parentNamespaceName, out parentLocalName, out parentAssemblyNameIfAny);
                return reflectionOnSeparateAppDomain.IsPropertyOrFieldACollection(propertyOrFieldName, parentNamespaceName, parentLocalName, parentAssemblyNameIfAny);
            }
        }

        public static bool IsPropertyOrFieldADictionary(XElement propertyElement, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain, bool isAttachedProperty)
        {
            if (isAttachedProperty)
            {
                string methodName = "Get" + propertyElement.Name.LocalName.Split('.')[1]; // In case of attached property, we check the return type of the method "GetPROPERTYNAME()". For example, in case of "Grid.Row", we check the return type of the method "Grid.GetRow()".
                XName elementName = propertyElement.Name.Namespace + propertyElement.Name.LocalName.Split('.')[0]; // eg. if the propertyElement is <VisualStateManager.VisualStateGroups>, this will be "DefaultNamespace+VisualStateManager"
                string namespaceName, localName, assemblyNameIfAny;
                GetClrNamespaceAndLocalName(elementName, out namespaceName, out localName, out assemblyNameIfAny);
                return reflectionOnSeparateAppDomain.DoesMethodReturnADictionary(methodName, namespaceName, localName, assemblyNameIfAny);
            }
            else
            {
                var propertyOrFieldName = propertyElement.Name.LocalName.Split('.')[1];
                var parentElement = propertyElement.Parent;
                string parentNamespaceName, parentLocalName, parentAssemblyNameIfAny;
                GetClrNamespaceAndLocalName(parentElement.Name, out parentNamespaceName, out parentLocalName, out parentAssemblyNameIfAny);
                return reflectionOnSeparateAppDomain.IsPropertyOrFieldADictionary(propertyOrFieldName, parentNamespaceName, parentLocalName, parentAssemblyNameIfAny);
            }
        }

        internal static bool IsElementADictionary(XElement element, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain)
        {
            string elementLocalName, elementNameSpace, assemblyNameIfAny;
            GetClrNamespaceAndLocalName(element.Name, out elementNameSpace, out elementLocalName, out assemblyNameIfAny);
            return reflectionOnSeparateAppDomain.IsElementADictionary(elementNameSpace, elementLocalName, assemblyNameIfAny);
        }

        internal static bool IsElementACollection(XElement element, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain)
        {
            string elementLocalName, elementNameSpace, assemblyNameIfAny;
            GetClrNamespaceAndLocalName(element.Name, out elementNameSpace, out elementLocalName, out assemblyNameIfAny);
            return reflectionOnSeparateAppDomain.IsElementACollection(elementNameSpace, elementLocalName, assemblyNameIfAny);
        }


        internal static bool IsElementAMarkupExtension(XElement element, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain)
        {
            string elementLocalName, elementNameSpace, assemblyNameIfAny;
            GetClrNamespaceAndLocalName(element.Name, out elementNameSpace, out elementLocalName, out assemblyNameIfAny);
            return reflectionOnSeparateAppDomain.IsElementAMarkupExtension(elementNameSpace, elementLocalName, assemblyNameIfAny);
        }

        //internal static bool IsElementAnUIElement(XElement element, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain)
        //{
        //    string elementLocalName, elementNameSpace, assemblyNameIfAny;
        //    GetClrNamespaceAndLocalName(element.Name, out elementNameSpace, out elementLocalName, out assemblyNameIfAny);
        //    return reflectionOnSeparateAppDomain.IsElementAnUIElement(elementNameSpace, elementLocalName, assemblyNameIfAny);
        //}

        internal static bool IsTypeAssignableFrom(XName elementOfTypeToAssignFrom, XName elementOfTypeToAssignTo, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain, bool isAttached = false)
        {
            string nameOfTypeToAssignFrom, nameSpaceOfTypeToAssignFrom, assemblyNameOfTypeToAssignFrom;
            GetClrNamespaceAndLocalName(elementOfTypeToAssignFrom, out nameSpaceOfTypeToAssignFrom, out nameOfTypeToAssignFrom, out assemblyNameOfTypeToAssignFrom);
            string nameOfTypeToAssignTo, nameSpaceOfTypeToAssignTo, assemblyNameOfTypeToAssignTo;
            GetClrNamespaceAndLocalName(elementOfTypeToAssignTo, out nameSpaceOfTypeToAssignTo, out nameOfTypeToAssignTo, out assemblyNameOfTypeToAssignTo);
            return reflectionOnSeparateAppDomain.IsTypeAssignableFrom(nameSpaceOfTypeToAssignFrom, nameOfTypeToAssignFrom, assemblyNameOfTypeToAssignFrom, nameSpaceOfTypeToAssignTo, nameOfTypeToAssignTo, assemblyNameOfTypeToAssignTo, isAttached);
        }

        //public static bool IsPropertyOrFieldACollection(XElement propertyElement, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain)
        //{
        //    var propertyOrFieldName = propertyElement.Name.LocalName.Split('.')[1];
        //    var parentElement = propertyElement.Parent;
        //    Type propertyOrFieldType = GetPropertyOrFieldType(parentElement.Name, propertyOrFieldName, reflectionOnSeparateAppDomain);
        //    bool typeIsACollection = (typeof(IEnumerable).IsAssignableFrom(propertyOrFieldType) && propertyOrFieldType != typeof(string));
        //    return typeIsACollection;
        //}

        //public static Type GetPropertyOrFieldType(XName xName, string propertyOrFieldName, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain)
        //{
        //    string namespaceName, localName, assemblyNameIfAny;
        //    GetClrNamespaceAndLocalName(xName, out namespaceName, out localName, out assemblyNameIfAny);

        //    throw new NotImplementedException();

        //    var elementType = GetTypeFromElement(xName, reflectionOnSeparateAppDomain);
        //    if (elementType == null)
        //        throw new XamlParseException("Type not found: " + elementType.ToString());
        //    PropertyInfo propertyInfo = elementType.GetProperty(propertyOrFieldName);
        //    if (propertyInfo == null)
        //        throw new XamlParseException("Property \"" + propertyOrFieldName + "\" not found in type \"" + elementType.ToString() + "\".");
        //    Type propertyOrFieldType = propertyInfo.PropertyType;
        //    return propertyOrFieldType;
        //}

        //public static MemberTypes GetMemberType(XName xname, string memberName, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain)
        //{
        //    MemberInfo memberInfo = GetMemberInfo(xname, memberName, reflectionOnSeparateAppDomain);
        //    return memberInfo.MemberType;
        //}

        //public static Type GetMethodReturnValueType(XName xname, string methodName, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain)
        //{
        //    var elementType = GetTypeFromElement(xname, reflectionOnSeparateAppDomain);
        //    if (elementType == null)
        //        throw new XamlParseException("Type not found: " + xname.ToString());
        //    MethodInfo methodInfo = elementType.GetMethod(methodName);
        //    if (methodInfo == null)
        //        throw new XamlParseException("Method \"" + methodName + "\" not found in type \"" + elementType.ToString() + "\".");
        //    Type methodType = methodInfo.ReturnType;
        //    return methodType;
        //}

        //public static string GetContentPropertyName(XElement element, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain)
        //{
        //    var type = GetTypeFromElement(element.Name, reflectionOnSeparateAppDomain);
        //    if (type == null)
        //        throw new XamlParseException("Type not found: " + element.Name.ToString());

        //    // Get instance of the attribute:
        //    var contentProperty = (ContentPropertyAttribute)Attribute.GetCustomAttribute(type, typeof(ContentPropertyAttribute));

        //    if (contentProperty == null)
        //        throw new XamlParseException("No default content property exists for element: " + element.Name.ToString());

        //    return contentProperty.Name;
        //}

        //public static string GetCSharpEquivalentOfXamlType(XName xname, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain)
        //{
        //    var type = GetTypeFromElement(xname, reflectionOnSeparateAppDomain);

        //    if (type == null)
        //        throw new XamlParseException("Type not found: " + xname.ToString());

        //    return "global::" + type.ToString();
        //}

        //static Type GetTypeFromElement(XName xname, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain)
        //{
        //    string namespaceName = xname.Namespace.NamespaceName;
        //    if (namespaceName.ToLower().StartsWith("using:"))
        //    {
        //        string ns = namespaceName.Substring("using:".Length);

        //        // Look for the type in all assemblies:
        //        var typeIfFound = reflectionOnSeparateAppDomain.FindType(ns, xname.LocalName);
        //        return typeIfFound;
        //    }
        //    else if (namespaceName.ToLower().StartsWith("clr-namespace:"))
        //    {
        //        string ns, assemblyNameIfAny;
        //        ParseClrNamespaceDeclaration(namespaceName, out ns, out assemblyNameIfAny);

        //        // Look for the type:
        //        var typeIfFound = reflectionOnSeparateAppDomain.FindType(ns, xname.LocalName, assemblyNameIfAny);
        //        return typeIfFound;
        //    }
        //    else
        //    {
        //        // If namespace is empty, use the default XAML namespace:
        //        if (string.IsNullOrEmpty(namespaceName))
        //            //ns = "http://schemas.microsoft.com/netfx/2007/xaml/presentation";
        //            namespaceName = "http://schemas.microsoft.com/winfx/2006/xaml/presentation"; //todo: instead of hard-coding this, use the default namespace that applies to the current XML node.

        //        // Look for the type:
        //        var typeIfFound = reflectionOnSeparateAppDomain.FindType(namespaceName, xname.LocalName);
        //        return typeIfFound;
        //    }
        //}

        public static void GetClrNamespaceAndLocalName(string typeAsStringInsideAXamlAttribute, XElement elementWhereTheTypeIsUsed, out string namespaceName, out string localName, out string assemblyNameIfAny)
        {
            XNamespace xNamespace = null;
            if (typeAsStringInsideAXamlAttribute.Contains(':'))
            {
                string[] splitted = typeAsStringInsideAXamlAttribute.Split(':');
                string prefix = splitted[0];
                typeAsStringInsideAXamlAttribute = splitted[1];
                xNamespace = elementWhereTheTypeIsUsed.GetNamespaceOfPrefix(prefix);
            }
            if (xNamespace == null)
                xNamespace = elementWhereTheTypeIsUsed.GetDefaultNamespace();

            XName name = xNamespace + typeAsStringInsideAXamlAttribute;

            GetClrNamespaceAndLocalName(name, out namespaceName, out localName, out assemblyNameIfAny);
        }

        internal static void FixNamespaceForCompatibility(ref string assemblyName, ref string namespaceName)
        {
#if SILVERLIGHTCOMPATIBLEVERSION
            if (assemblyName != null)
            {
#if CSHTML5BLAZOR
                switch (assemblyName)
                {
                    case "System.Windows.Controls.Data.Input":
                        assemblyName = "OpenSilver.Controls.Data.Input";
                        return;
                    case "System.Windows.Controls.Data":
                        assemblyName = "OpenSilver.Controls.Data";
                        return;
                    case "System.Windows.Controls.Data.DataForm.Toolkit":
                        assemblyName = "OpenSilver.Controls.Data.DataForm.Toolkit";
                        return;
                    case "System.Windows.Controls.Navigation":
                        assemblyName = "OpenSilver.Controls.Navigation";
                        return;
                    default:
                        if (assemblyName == "System" || assemblyName.StartsWith("System."))
                        {
                            assemblyName = Constants.NAME_OF_CORE_ASSEMBLY_SLMIGRATION_USING_BLAZOR;
                        }
                        return;
                }
#elif BRIDGE
                switch (assemblyName)
                {
                    case "System.Windows.Controls.Data.Input":
                        assemblyName = "CSHTML5.Controls.Data.Input";
                        return;
                    default:
                        if (assemblyName == "System" || assemblyName.StartsWith("System."))
                        {
                            assemblyName = Constants.NAME_OF_CORE_ASSEMBLY_SLMIGRATION_USING_BRIDGE;
                        }
                        return;
                }                    
#else // JSIL, Obsolete, remove this
                switch (assemblyName)
                {
                    default:
                        if (assemblyName == "System" || assemblyName.StartsWith("System."))
                        {
                            assemblyName = Constants.NAME_OF_CORE_ASSEMBLY_SLMIGRATION;
                        }
                        return;
                }
#endif
            }
#endif
        }

        public static void GetClrNamespaceAndLocalName(XName xName, out string namespaceName, out string localName, out string assemblyNameIfAny)
        {
            namespaceName = xName.Namespace.NamespaceName;
            localName = xName.LocalName;
            assemblyNameIfAny = null;
            if (namespaceName.ToLower().StartsWith("using:"))
            {
                string ns = namespaceName.Substring("using:".Length);
                namespaceName = ns;
            }
            else if (namespaceName.ToLower().StartsWith("clr-namespace:"))
            {
                string ns;
                ParseClrNamespaceDeclaration(namespaceName, out ns, out assemblyNameIfAny);
                namespaceName = ns;
                FixNamespaceForCompatibility(ref assemblyNameIfAny, ref namespaceName);
            }
            else
            {
                // If namespace is empty, use the default XAML namespace:
                if (string.IsNullOrEmpty(namespaceName))
                    //ns = "http://schemas.microsoft.com/netfx/2007/xaml/presentation";
                    namespaceName = "http://schemas.microsoft.com/winfx/2006/xaml/presentation"; //todo: instead of hard-coding this, use the default namespace that applies to the current XML node.
            }
        }

        internal static string GetKeyNameOfProperty(XElement element, string propertyName, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain)
        {
            string elementLocalName, elementNameSpace, assemblyNameIfAny;
            GetClrNamespaceAndLocalName(element.Name, out elementNameSpace, out elementLocalName, out assemblyNameIfAny);
            return reflectionOnSeparateAppDomain.GetKeyNameOfProperty(elementNameSpace, elementLocalName, assemblyNameIfAny, propertyName);
        }


        public static void ParseClrNamespaceDeclaration(string input, out string ns, out string assemblyNameIfAny)
        {
            assemblyNameIfAny = null;
            var str = input.Substring("clr-namespace:".Length);
            int indexOfSemiColons = str.IndexOf(';');
            if (indexOfSemiColons > -1)
            {
                ns = str.Substring(0, indexOfSemiColons);
                var str2 = str.Substring(indexOfSemiColons);
                if (str2.StartsWith(";assembly="))
                    assemblyNameIfAny = str2.Substring(";assembly=".Length);
            }
            else
            {
                ns = str;
            }
        }

        //static MemberInfo GetMemberInfo(XName xname, string memberName, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain)
        //{
        //    var elementType = GetTypeFromElement(xname, reflectionOnSeparateAppDomain);
        //    if (elementType == null)
        //        throw new XamlParseException("Type not found: " + elementType.ToString());
        //    MemberInfo[] membersFound = elementType.GetMember(memberName);
        //    if (membersFound == null || membersFound.Length < 1)
        //        throw new XamlParseException("Member \"" + memberName + "\" not found in type \"" + elementType.ToString() + "\".");
        //    MemberInfo memberInfo = membersFound[0];
        //    return memberInfo;
        //}

        //public static bool DoesTypeContainNameMemberOfTypeString(XName xname, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain)
        //{
        //    MemberInfo memberInfo;
        //    try 
        //    {	        
        //        memberInfo = GetMemberInfo(xname, "Name", reflectionOnSeparateAppDomain);
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //    if (memberInfo.MemberType == MemberTypes.Field && ((FieldInfo)memberInfo).FieldType == typeof(string) && ((FieldInfo)memberInfo).IsPublic && !((FieldInfo)memberInfo).IsStatic && !((FieldInfo)memberInfo).IsSecurityCritical)
        //        return true;
        //    if (memberInfo.MemberType == MemberTypes.Property && ((PropertyInfo)memberInfo).PropertyType == typeof(string))
        //        return true;
        //    return false;
        //}

        //static IEnumerable<Assembly> GetReferenceAssemblies()
        //{
        //    //todo-performance: for performance reason, cache the result instead of doing a "Assembly.LoadWithPartialName" at every call?
        //    return new[] { "WindowsBase", "PresentationCore", "PresentationFramework" }.Select(an => Assembly.LoadWithPartialName(an));
        //}


    }
}
