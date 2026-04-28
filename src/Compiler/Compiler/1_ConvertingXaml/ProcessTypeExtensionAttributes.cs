
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

using Mono.Cecil;
using System;
using System.Xml.Linq;

namespace OpenSilver.Compiler;

internal static class ProcessTypeExtensionAttributes
{
    internal static void Process(XDocument doc, ConversionSettings settings)
    {
        ResolveTypeExtensionAttributes(doc.Root, settings);
    }

    private static void ResolveTypeExtensionAttributes(XElement element, ConversionSettings settings)
    {
        if (!XamlParser.IsMemberNode(element))
        {
            // Normalize <x:Type> element to <x:TypeExtension> so the compiler handles it the same way:
            if (element.Name.LocalName == "Type")
            {
                XName typeExtName = element.Name.Namespace + "TypeExtension";
                if (GeneratingCode.IsTypeExtension(typeExtName, settings))
                {
                    element.Name = typeExtName;
                }
            }

            settings.XamlNameParser.GetClrNamespaceAndLocalName(
                element.Name,
                out string namespaceName,
                out string typeName,
                out string assemblyName);

            TypeDefinition type = settings.Inspector.GetTypeDefinition(namespaceName, typeName, assemblyName, element);

            if (settings.Inspector.IsStyle(type))
            {
                if (element.Attribute("TargetType") is XAttribute targetType)
                {
                    ResolveTypeExtensionAttribute(element, targetType, settings);
                }
            }
            else if (settings.Inspector.IsControlTemplate(type))
            {
                if (element.Attribute("TargetType") is XAttribute targetType)
                {
                    ResolveTypeExtensionAttribute(element, targetType, settings);
                }
            }
            else if (settings.Inspector.IsDataTemplate(type))
            {
                if (element.Attribute("DataType") is XAttribute targetType)
                {
                    ResolveTypeExtensionAttribute(element, targetType, settings);
                }
            }
        }

        foreach (var child in element.Elements())
        {
            ResolveTypeExtensionAttributes(child, settings);
        }
    }

    private static void ResolveTypeExtensionAttribute(XElement element, XAttribute attribute, ConversionSettings settings)
    {
        string value = attribute.Value;

        if (value.StartsWith("{}") || !(value.StartsWith("{") && value.EndsWith("}")))
        {
            return;
        }

        ReadOnlySpan<char> content = value.AsSpan(1, value.Length - 2).Trim();

        int separatorIndex = IndexOfWhiteSpace(content);
        if (separatorIndex == -1)
        {
            return;
        }

        ReadOnlySpan<char> markupExtensionName = content.Slice(0, separatorIndex);

        if (IsTypeExtension(markupExtensionName, element, settings))
        {
            attribute.Value = content.Slice(separatorIndex + 1).Trim().ToString();
        }
    }

    private static bool IsTypeExtension(ReadOnlySpan<char> span, XElement element, ConversionSettings settings)
    {
        XName xname = null;

        XNamespace xmlns;
        ReadOnlySpan<char> name;

        int columnIndex = span.IndexOf(':');
        if (columnIndex == -1)
        {
            xmlns = element.GetDefaultNamespace();
            name = span;
        }
        else
        {
            string prefix = span.Slice(0, columnIndex).ToString();
            xmlns = element.GetNamespaceOfPrefix(prefix);
            name = span.Slice(columnIndex + 1);
        }

        if (xmlns is not null)
        {
            if (name.Equals("Type", StringComparison.Ordinal) || name.Equals("TypeExtension", StringComparison.Ordinal))
            {
                xname = xmlns.GetName("TypeExtension");
            }
        }

        return xname is not null && GeneratingCode.IsTypeExtension(xname, settings);
    }

    private static int IndexOfWhiteSpace(ReadOnlySpan<char> span)
    {
        for (int i = 0; i < span.Length; i++)
        {
            if (char.IsWhiteSpace(span[i]))
            {
                return i;
            }
        }

        return -1;
    }
}
