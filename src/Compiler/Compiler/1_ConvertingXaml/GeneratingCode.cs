
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

using Mono.Cecil;
using System;
using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;

namespace OpenSilver.Compiler
{
    internal interface ICodeGenerator
    {
        string Generate();
    }

    internal static class GeneratingCode
    {
        internal const string DefaultXamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        internal const string LegacyXamlNamespace = "http://schemas.microsoft.com/client/2007"; // XAML namespace used for Silverlight 1.0 application
        internal const string SdkXamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk";

        internal static readonly XNamespace[] DefaultXamlNamespaces = [DefaultXamlNamespace, LegacyXamlNamespace];
        internal static readonly XNamespace xNamespace = "http://schemas.microsoft.com/winfx/2006/xaml"; // Used for example for "x:Name" attributes and {x:Null} markup extensions.

        internal static readonly XName XKeyAttribute = xNamespace.GetName("Key");
        internal static readonly XName XNameAttribute = xNamespace.GetName("Name");
        internal static readonly XName XFieldModifierAttribute = xNamespace.GetName("FieldModifier");

        internal static string GetAttributeValue(XAttribute attribute)
        {
            string value = attribute.Value;

            if (value is not null && value.StartsWith("{}"))
            {
                return value.Substring(2);
            }

            return value;
        }

        internal static bool SkipAttribute(XAttribute attribute)
        {
            if (IsReservedAttribute(attribute.Name) || attribute.IsNamespaceDeclaration)
            {
                return true;
            }

            int index = attribute.Name.LocalName.IndexOf('.');

            if (index == -1)
            {
                XElement element = attribute.Parent;

                if (!string.IsNullOrEmpty(attribute.Name.NamespaceName) && attribute.Name.Namespace != element.Name.Namespace)
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool IsReservedAttribute(XName attributeName)
        {
            return attributeName == GeneratingUniqueNames.UniqueNameAttribute ||
                   attributeName == InsertingImplicitNodes.InitializedFromStringAttribute ||
                   attributeName == InsertingMarkupNodesInXaml.GeneratedMarkupExtensionAttribute ||
                   attributeName == GeneratingPathInXaml.PathInXamlAttribute;
        }

        internal static bool IsXNameAttribute(XAttribute attr) =>
            attr.Name.LocalName == "Name" && attr.Name.Namespace == xNamespace;

        internal static bool IsNameAttribute(XAttribute attr) =>
            attr.Name.LocalName == "Name" && string.IsNullOrEmpty(attr.Name.NamespaceName);

        internal static string GetUniqueName(XElement element) =>
            element.Attribute(GeneratingUniqueNames.UniqueNameAttribute).Value;

        public static bool IsDataTemplate(XElement element, ConversionSettings settings) =>
            IsXElementOfType(element, "DataTemplate", KnownNamespaces.SystemWindows, settings);

        public static bool IsItemsPanelTemplate(XElement element, ConversionSettings settings) =>
            IsXElementOfType(element, "ItemsPanelTemplate", KnownNamespaces.SystemWindowsControls, settings);

        public static bool IsControlTemplate(XElement element, ConversionSettings settings) =>
            IsXElementOfType(element, "ControlTemplate", KnownNamespaces.SystemWindowsControls, settings);

        public static bool IsBinding(XElement element, ConversionSettings settings) =>
            IsXElementOfType(element, "Binding", KnownNamespaces.SystemWindowsData, settings);

        public static bool IsStyle(XElement element, ConversionSettings settings) =>
            IsXElementOfType(element, "Style", KnownNamespaces.SystemWindows, settings);

        public static bool IsTextBlock(XElement element, ConversionSettings settings) =>
            IsXElementOfType(element, "TextBlock", KnownNamespaces.SystemWindowsControls, settings);

        public static bool IsRun(XElement element, ConversionSettings settings) =>
            IsXElementOfType(element, "Run", KnownNamespaces.SystemWindowsDocuments, settings);

        public static bool IsSpan(XElement element, ConversionSettings settings) =>
            IsXElementOfType(element, "Span", KnownNamespaces.SystemWindowsDocuments, settings);

        public static bool IsItalic(XElement element, ConversionSettings settings) =>
            IsXElementOfType(element, "Italic", KnownNamespaces.SystemWindowsDocuments, settings);

        public static bool IsUnderline(XElement element, ConversionSettings settings) =>
            IsXElementOfType(element, "Underline", KnownNamespaces.SystemWindowsDocuments, settings);

        public static bool IsBold(XElement element, ConversionSettings settings) =>
            IsXElementOfType(element, "Bold", KnownNamespaces.SystemWindowsDocuments, settings);

        public static bool IsHyperlink(XElement element, ConversionSettings settings) =>
            IsXElementOfType(element, "Hyperlink", KnownNamespaces.SystemWindowsDocuments, settings);

        public static bool IsParagraph(XElement element, ConversionSettings settings) =>
            IsXElementOfType(element, "Paragraph", KnownNamespaces.SystemWindowsDocuments, settings);

        private static bool IsXElementOfType(XElement element, string typeName, string namespaceName, ConversionSettings settings)
        {
            XName name = element.Name;
            if (name.LocalName == typeName)
            {
                for (int i = 0; i < DefaultXamlNamespaces.Length; i++)
                {
                    if (name.Namespace == DefaultXamlNamespaces[i])
                    {
                        return true;
                    }
                }

                settings.XamlNameParser.ParseClrNamespaceDeclaration(name.NamespaceName, out string ns, out string assemblyName);
                if (ns == namespaceName)
                {
                    return assemblyName == "OpenSilver";
                }
            }

            return false;
        }

        public static bool IsNullExtension(XElement element, ConversionSettings settings)
        {
            if (element.Name.LocalName != "NullExtension")
            {
                return false;
            }

            if (element.Name.NamespaceName == xNamespace.NamespaceName ||
                element.Name.NamespaceName == DefaultXamlNamespace ||
                element.Name.NamespaceName == LegacyXamlNamespace)
            {
                return true;
            }

            (string ns, string assemblyName) = settings.XamlNameParser.GetClrNamespaceAndAssembly(element.Name.NamespaceName);

            return ns == "System.Windows.Markup" && assemblyName == "OpenSilver";
        }

        public static bool IsStaticExtension(XElement element, ConversionSettings settings)
        {
            if (element.Name.LocalName != "StaticExtension")
            {
                return false;
            }

            if (element.Name.NamespaceName == xNamespace.NamespaceName ||
                element.Name.NamespaceName == DefaultXamlNamespace ||
                element.Name.NamespaceName == LegacyXamlNamespace)
            {
                return true;
            }

            (string ns, string assemblyName) = settings.XamlNameParser.GetClrNamespaceAndAssembly(element.Name.NamespaceName);

            return ns == "System.Windows.Markup" && assemblyName == "OpenSilver";
        }

        public static bool IsTypeExtension(XElement element, ConversionSettings settings) => IsTypeExtension(element.Name, settings);

        public static bool IsTypeExtension(XName name, ConversionSettings settings)
        {
            if (name.LocalName != "TypeExtension")
            {
                return false;
            }

            if (name.NamespaceName == xNamespace.NamespaceName ||
                name.NamespaceName == DefaultXamlNamespace ||
                name.NamespaceName == LegacyXamlNamespace)
            {
                return true;
            }

            (string ns, string assemblyName) = settings.XamlNameParser.GetClrNamespaceAndAssembly(name.NamespaceName);

            return ns == "System.Windows.Markup" && assemblyName == "OpenSilver";
        }

        public static bool IsUriAbsolute(string path)
        {
            if (path.StartsWith("~"))
            {
                return true;
            }

            int index = path.IndexOf(':');
            if (index >= 0)
            {
                string scheme = path.Substring(0, index);
                return Uri.CheckSchemeName(scheme);
            }

            return false;
        }

        public static bool IsComponentUri(string value) => value.Contains(";component/", StringComparison.OrdinalIgnoreCase);

        public static bool IsApplicationStartupUriProperty(string propertyName, TypeReference type)
        {
            if (propertyName != "StartupUri")
            {
                return false;
            }

            return type.Name == "Application" &&
                   type.Namespace == KnownNamespaces.SystemWindows &&
                   type.GetAssemblyName() == Constants.OPENSILVER_ASSEMBLY_NAME;
        }

        internal static bool ShouldConvertUri(string value, string memberName, string memberTypeName, TypeReference elementType, TypeReference declaringType)
        {
            string assemblyName = elementType.GetAssemblyName();

            if (assemblyName == Constants.OPENSILVER_ASSEMBLY_NAME)
            {
                if (elementType.Name == "Hyperlink" && elementType.Namespace == KnownNamespaces.SystemWindowsDocuments)
                {
                    return false;
                }

                if (elementType.Name == "HyperlinkButton" && elementType.Namespace == KnownNamespaces.SystemWindowsControls)
                {
                    return false;
                }
            }

            if (assemblyName == Constants.OPENSILVER_CONTROLS_NAVIGATION_ASSEMBLY_NAME)
            {
                if (elementType.Name == "UriMapping" && elementType.Namespace == KnownNamespaces.SystemWindowsNavigation)
                {
                    return false;
                }

                if (elementType.Name == "Frame" && elementType.Namespace == KnownNamespaces.SystemWindowsControls)
                {
                    return false;
                }
            }

            if (IsUriAbsolute(value))
            {
                return false;
            }

            if (IsComponentUri(value))
            {
                return false;
            }

            if (IsApplicationStartupUriProperty(memberName, declaringType))
            {
                return true;
            }

            if (value.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (memberTypeName != "System.Uri" &&
                memberTypeName != $"{KnownNamespaces.SystemWindowsMedia}.ImageSource" &&
                (memberName != "FontFamily" || !value.Contains(".")))
            {
                return false;
            }

            return true;
        }

        internal static string GetCSharpEquivalentOfXamlTypeAsString(XElement element, ConversionSettings settings)
        {
            settings.XamlNameParser.GetClrNamespaceAndLocalName(
                element.Name,
                out string namespaceName,
                out string typeName,
                out string assemblyName);

            if (settings.Inspector.GetTypeDefinition(namespaceName, typeName, assemblyName, element, false) is TypeDefinition type)
            {
                return $"{settings.TypeReferenceHelper.Global}{settings.TypeReferenceHelper.ConvertToString(type)}";
            }

            if (XamlNameParser.IsXmlNamespace(element.Name.NamespaceName))
            {
                return typeName;
            }

            if (string.IsNullOrEmpty(namespaceName))
            {
                return $"{settings.TypeReferenceHelper.Global}{typeName}";
            }
            else
            {
                return $"{settings.TypeReferenceHelper.Global}{namespaceName}.{typeName}";
            }
        }

        internal static TypeDefinition GetTypeDefinitionFromString(string value, XElement xmlnsResolver, IXmlLineInfo lineInfo, ConversionSettings settings, bool throwIfNull = true)
        {
            Debug.Assert(value is not null);

            settings.XamlNameParser.GetClrNamespaceAndLocalName(
                value, xmlnsResolver, out string namespaceName, out string typeName, out string assemblyName);

            return settings.Inspector.GetTypeDefinition(namespaceName, typeName, assemblyName, lineInfo, throwIfNull);
        }
    }
}
