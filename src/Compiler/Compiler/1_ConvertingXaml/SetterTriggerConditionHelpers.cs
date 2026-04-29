
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
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace OpenSilver.Compiler;

internal static class SetterTriggerConditionHelpers
{
    internal static (TypeDefinition DeclaringType, string PropertyName) GetSetterOrTriggerOrConditionProperty(
        XAttribute property,
        string targetPropertyName,
        ConversionSettings settings)
    {
        XElement element = property.Parent;

        int index = property.Value.IndexOf('.');
        if (index > -1)
        {
            string typeString = property.Value.Substring(0, index);
            string propertyName = property.Value.Substring(index + 1);

            settings.XamlNameParser.GetClrNamespaceAndLocalName(
                typeString,
                element,
                out string namespaceName,
                out string typeName,
                out string assemblyName);

            return (settings.Inspector.GetTypeDefinition(namespaceName, typeName, assemblyName, property), propertyName);
        }
        else
        {
            TypeDefinition declaringType = null;

            if (element.Attribute(targetPropertyName) is XAttribute targetNameAttr)
            {
                if (GetElementFromTargetName(targetNameAttr, settings) is not XElement targetElement)
                {
                    throw new XamlParseException(
                        $"Cannot find the Trigger target '{targetNameAttr.Value}'. (The target must appear before any Setters, Triggers, or Conditions that use it.)",
                        targetNameAttr);
                }

                settings.XamlNameParser.GetClrNamespaceAndLocalName(
                    targetElement.Name,
                    out string namespaceName,
                    out string typeName,
                    out string assemblyName);

                declaringType = settings.Inspector.GetTypeDefinition(namespaceName, typeName, assemblyName, targetElement);
            }
            else
            {
                for (XElement parent = element.Parent; parent is not null; parent = parent.Parent)
                {
                    if (XamlParser.IsMemberNode(parent))
                    {
                        continue;
                    }

                    settings.XamlNameParser.GetClrNamespaceAndLocalName(
                        parent.Name,
                        out string namespaceName,
                        out string typeName,
                        out string assemblyName);

                    TypeDefinition type = settings.Inspector.GetTypeDefinition(namespaceName, typeName, assemblyName, parent);

                    if (settings.Inspector.IsStyle(type))
                    {
                        declaringType = GetStyleTargetType(parent, settings);
                        break;
                    }

                    if (settings.Inspector.IsControlTemplate(type))
                    {
                        declaringType = GetControlTemplateTargetType(parent, settings);
                        break;
                    }

                    if (settings.Inspector.IsDataTemplate(type))
                    {
                        declaringType = GetDataTemplateTargetType(settings);
                        break;
                    }
                }
            }

            return (declaringType, property.Value);
        }
    }

    private static XElement GetElementFromTargetName(XAttribute targetNameAttr, ConversionSettings settings)
    {
        XElement setter = targetNameAttr.Parent;
        string targetName = GeneratingCode.GetAttributeValue(targetNameAttr);
        XElement template = null;

        for (XElement parent = setter.Parent; parent is not null; parent = parent.Parent)
        {
            if (XamlParser.IsMemberNode(parent))
            {
                continue;
            }

            settings.XamlNameParser.GetClrNamespaceAndLocalName(parent.Name,
                out string namespaceName, out string typeName, out string assemblyName);

            TypeDefinition type = settings.Inspector.GetTypeDefinition(namespaceName, typeName, assemblyName, parent);

            if (settings.Inspector.IsStyle(type))
            {
                throw new XamlParseException("TargetName property cannot be set on a Style Setter.", targetNameAttr);
            }

            if (settings.Inspector.IsFrameworkTemplate(type))
            {
                template = parent;
                break;
            }
        }

        if (template is not null)
        {
            XElement templateContent = GetTemplateContentElement(template, settings);
            return GetNamedElementInTemplateContent(templateContent, targetName, settings);
        }

        return null;
    }

    private static XElement GetTemplateContentElement(XElement template, ConversionSettings settings)
    {
        foreach (XElement element in template.Elements())
        {
            if (!XamlParser.IsMemberNode(element))
            {
                continue;
            }

            int idx = element.Name.LocalName.IndexOf('.');

            string typeName = element.Name.LocalName.Substring(0, idx);
            (string namespaceName, string assemblyName) = settings.XamlNameParser.GetClrNamespaceAndAssembly(
                element.Name.NamespaceName);

            string propertyName = element.Name.LocalName.Substring(idx + 1);

            if (settings.Inspector.IsFrameworkTemplateTemplateProperty(propertyName, namespaceName, typeName, assemblyName, element))
            {
                return element.Elements().FirstOrDefault();
            }
        }

        return null;
    }

    private static XElement GetNamedElementInTemplateContent(XElement element, string targetName, ConversionSettings settings)
    {
        if (element is null)
        {
            return null;
        }

        if (!XamlParser.IsMemberNode(element))
        {
            settings.XamlNameParser.GetClrNamespaceAndLocalName(element.Name,
                out string namespaceName, out string typeName, out string assemblyName);

            TypeDefinition type = settings.Inspector.GetTypeDefinition(
                namespaceName,
                typeName,
                assemblyName,
                element);

            // Skips types that create a new scope
            if (settings.Inspector.IsStyle(type) ||
                settings.Inspector.IsFrameworkTemplate(type) ||
                settings.Inspector.IsResourceDictionary(type))
            {
                return null;
            }

            XAttribute nameAttr = element
                .Attributes()
                .FirstOrDefault(attr => GeneratingCode.IsXNameAttribute(attr) || GeneratingCode.IsNameAttribute(attr));

            if (nameAttr is not null)
            {
                string name = GeneratingCode.GetAttributeValue(nameAttr);
                if (name == targetName)
                {
                    return element;
                }
            }
        }

        foreach (XElement child in element.Elements())
        {
            if (GetNamedElementInTemplateContent(child, targetName, settings) is XElement namedElement)
            {
                return namedElement;
            }
        }

        return null;
    }

    private static TypeDefinition GetStyleTargetType(XElement style, ConversionSettings settings)
    {
        IXmlLineInfo lineInfo;
        string namespaceName, typeName, assemblyName;

        if (style.Attribute("TargetType") is XAttribute targetType)
        {
            lineInfo = targetType;

            settings.XamlNameParser.GetClrNamespaceAndLocalName(
                targetType.Value, style, out namespaceName, out typeName, out assemblyName);
        }
        else
        {
            lineInfo = style;

            namespaceName = KnownNamespaces.SystemWindows;
            typeName = "FrameworkElement";
            assemblyName = Constants.OPENSILVER_ASSEMBLY_NAME;
        }

        return settings.Inspector.GetTypeDefinition(namespaceName, typeName, assemblyName, lineInfo);
    }

    private static TypeDefinition GetControlTemplateTargetType(XElement template, ConversionSettings settings)
    {
        IXmlLineInfo lineInfo;
        string namespaceName, typeName, assemblyName;

        if (template.Attribute("TargetType") is XAttribute targetType)
        {
            lineInfo = targetType;

            settings.XamlNameParser.GetClrNamespaceAndLocalName(
                targetType.Value, template, out namespaceName, out typeName, out assemblyName);
        }
        else
        {
            lineInfo = template;

            namespaceName = KnownNamespaces.SystemWindowsControls;
            typeName = "Control";
            assemblyName = Constants.OPENSILVER_ASSEMBLY_NAME;
        }

        return settings.Inspector.GetTypeDefinition(namespaceName, typeName, assemblyName, lineInfo);
    }

    private static TypeDefinition GetDataTemplateTargetType(ConversionSettings settings)
    {
        string namespaceName = KnownNamespaces.SystemWindowsControls;
        string typeName = "ContentPresenter";
        string assemblyName = Constants.OPENSILVER_ASSEMBLY_NAME;

        return settings.Inspector.GetKnownTypeDefinition(namespaceName, typeName, assemblyName);
    }
}
