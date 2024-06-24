
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

using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Windows.Documents;
using DotNetForHtml5.Core;

namespace System.Windows.Controls;

internal static class RichTextXamlParser
{
    public static IEnumerable<Block> Parse(string xaml)
    {
        var document = new XmlDocument();
        document.LoadXml(xaml);

        return ParseInternal(document.SelectSingleNode("*"));
    }

    private static IEnumerable<Block> ParseInternal(XmlNode node)
    {
        if (node is XmlElement)
        {
            var currentNode = ProcessNode(node);

            if (node.HasChildNodes)
            {
                if (currentNode is Section section)
                {
                    foreach (var item in ParseInternal(node.FirstChild))
                    {
                        section.Blocks.Add(item);
                    }
                }
                else if (currentNode is Paragraph paragraph)
                {
                    foreach (XmlNode item in node.ChildNodes)
                    {
                        var inline = ProcessNodeInline(item);
                        if (inline != null)
                        {
                            paragraph.Inlines.Add(inline);
                        }
                    }
                }
            }

            yield return currentNode;

            if (node.NextSibling != null)
            {
                foreach (var sibling in ParseInternal(node.NextSibling))
                {
                    yield return sibling;
                }
            }
        }
    }

    private static Block ProcessNode(XmlNode node)
    {
        if (node.NodeType == XmlNodeType.Element)
        {
            if (node.Name == nameof(Section))
            {
                var element = new Section();
                SetProperties(element, node);
                return element;
            }
            if (node.Name == nameof(Paragraph))
            {
                var element = new Paragraph();
                SetProperties(element, node);
                return element;
            }
        }

        return null;
    }

    private static Inline ProcessNodeInline(XmlNode node)
    {
        if (node.NodeType == XmlNodeType.Element)
        {
            if (node.Name == nameof(Run))
            {
                var element = new Run();
                element.Text = node.InnerText;
                SetProperties(element, node);
                return element;
            }
            if (node.Name == nameof(LineBreak))
            {
                var element = new LineBreak();
                SetProperties(element, node);
                return element;
            }
            if (node.Name == nameof(InlineUIContainer))
            {
                var element = new InlineUIContainer();
                SetProperties(element, node);
                return element;
            }
            if (node.Name == nameof(Span))
            {
                var element = new Span();
                SetProperties(element, node);
                ProcessChildInlines(element, node);
                return element;
            }
            if (node.Name == nameof(Bold))
            {
                var element = new Bold();
                SetProperties(element, node);
                ProcessChildInlines(element, node);
                return element;
            }
            if (node.Name == nameof(Italic))
            {
                var element = new Italic();
                SetProperties(element, node);
                ProcessChildInlines(element, node);
                return element;
            }
            if (node.Name == nameof(Underline))
            {
                var element = new Underline();
                SetProperties(element, node);
                ProcessChildInlines(element, node);
                return element;
            }
            if (node.Name == nameof(Hyperlink))
            {
                var element = new Hyperlink();
                SetProperties(element, node);
                ProcessChildInlines(element, node);
                return element;
            }
        }
        else if (node.NodeType == XmlNodeType.Text)
        {
            return new Run { Text = node.InnerText };
        }

        return null;
    }

    private static void ProcessChildInlines(Span element, XmlNode node)
    {
        foreach (XmlNode item in node.ChildNodes)
        {
            var inline = ProcessNodeInline(item);
            if (inline != null)
            {
                element.Inlines.Add(inline);
            }
        }
    }

    private static void SetProperties(TextElement element, XmlNode node)
    {
        foreach (XmlAttribute attribute in node.Attributes)
        {
            SetProperty(element, attribute.Name, attribute.Value);
        }
    }

    private static void SetProperty(object obj, string propertyName, object value)
    {
        var propInfo = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        if (propInfo != null)
        {
            try
            {
                var val = value == null ? null : TypeFromStringConverters.ConvertFromInvariantString(propInfo.PropertyType, value.ToString());
                propInfo.SetValue(obj, val);
            }
            catch { }
        }
    }
}
