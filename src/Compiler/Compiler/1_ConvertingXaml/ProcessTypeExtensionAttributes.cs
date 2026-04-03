
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
            GettingInformationAboutXamlTypes.GetClrNamespaceAndLocalName(
            element.Name,
            out string namespaceName,
            out string typeName,
            out string assemblyName);

            if (settings.Inspector.IsStyle(namespaceName, typeName, assemblyName, element))
            {
                if (element.Attribute("TargetType") is XAttribute targetType)
                {
                    ResolveTypeExtensionAttribute(element, targetType);
                }
            }
            else if (settings.Inspector.IsControlTemplate(namespaceName, typeName, assemblyName, element))
            {
                if (element.Attribute("TargetType") is XAttribute targetType)
                {
                    ResolveTypeExtensionAttribute(element, targetType);
                }
            }
            else if (settings.Inspector.IsDataTemplate(namespaceName, typeName, assemblyName, element))
            {
                if (element.Attribute("DataType") is XAttribute targetType)
                {
                    ResolveTypeExtensionAttribute(element, targetType);
                }
            }
        }

        foreach (var child in element.Elements())
        {
            ResolveTypeExtensionAttributes(child, settings);
        }
    }

    private static void ResolveTypeExtensionAttribute(XElement element, XAttribute attribute)
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

        if (IsTypeExtension(markupExtensionName, element))
        {
            attribute.Value = content.Slice(separatorIndex + 1).Trim().ToString();
        }
    }

    private static bool IsTypeExtension(ReadOnlySpan<char> span, XElement element)
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

        return xname is not null && GeneratingCode.IsTypeExtension(xname);
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
