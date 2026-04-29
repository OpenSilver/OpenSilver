
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
using System.Xml;
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
                    ResolveTypeExtensionAttribute(targetType, element, settings);
                }
            }
            else if (settings.Inspector.IsControlTemplate(type))
            {
                if (element.Attribute("TargetType") is XAttribute targetType)
                {
                    ResolveTypeExtensionAttribute(targetType, element, settings);
                }
            }
            else if (settings.Inspector.IsDataTemplate(type))
            {
                if (element.Attribute("DataType") is XAttribute targetType)
                {
                    ResolveTypeExtensionAttribute(targetType, element, settings);
                }
            }
        }

        foreach (var child in element.Elements())
        {
            ResolveTypeExtensionAttributes(child, settings);
        }
    }

    private static void ResolveTypeExtensionAttribute(XAttribute attribute, XElement xmlnsResolver, ConversionSettings settings)
    {
        if (MarkupExtensionDescriptor.TryParse(attribute.Value, out MarkupExtensionDescriptor markupExtension) &&
            TryExtractTypeName(markupExtension, xmlnsResolver, attribute, settings, out string typeName))
        {
            attribute.Value = typeName;
        }
    }

    private static bool TryExtractTypeName(
        MarkupExtensionDescriptor markupExtension,
        XElement xmlnsResolver,
        IXmlLineInfo lineInfo,
        ConversionSettings settings,
        out string typeName)
    {
        var type = ResolveTypeDefinition(markupExtension.Name, xmlnsResolver, lineInfo, settings);
        if (!settings.Inspector.IsTypeExtension(type))
        {
            typeName = null;
            return false;
        }

        if (markupExtension.ContentProperty is string content && markupExtension.Properties.Count == 0)
        {
            typeName = content;
            return true;
        }

        if (markupExtension.ContentProperty is null && markupExtension.Properties.Count == 1)
        {
            (string propertyName, object propertyValue) = markupExtension.Properties[0];

            if ((propertyName == "TypeName" || propertyName == "Type") && propertyValue is string value)
            {
                typeName = value;
                return true;
            }
        }

        typeName = null;
        return false;
    }

    private static TypeDefinition ResolveTypeDefinition(string value, XElement xmlnsResolver, IXmlLineInfo lineInfo, ConversionSettings settings)
    {
        settings.XamlNameParser.GetClrNamespaceAndLocalName(
            value, xmlnsResolver, out string namespaceName, out string typeName, out string assemblyName);

        return settings.Inspector.GetMarkupExtensionTypeDefinition(namespaceName, typeName, assemblyName, lineInfo);
    }
}
