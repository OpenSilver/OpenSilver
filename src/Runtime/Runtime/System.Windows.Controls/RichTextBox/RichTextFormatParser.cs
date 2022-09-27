
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
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using DotNetForHtml5.Core;

#if MIGRATION
using System.Windows.Documents;
#else
using Windows.UI.Xaml.Documents;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    internal class RichTextXamlParser
    {
        public static IEnumerable<Block> Parse(string xaml)
        {
            List<Block> result = new List<Block>();
            XmlDocument document = new XmlDocument();
            document.LoadXml(xaml);
            XmlNode root = document.SelectSingleNode("*");
            result.AddRange(ParseInternal(root));
            return result;
        }

        private static IEnumerable<Block> ParseInternal(XmlNode node)
        {
            List<Block> result = new List<Block>();
            if (node is XmlElement)
            {
                var currentNode = ProcessNode(node);
                result.Add(currentNode);
                if (node.HasChildNodes)
                {
                    if (currentNode is Section)
                    {
                        foreach (var item in ParseInternal(node.FirstChild))
                        {
                            ((Section)currentNode).Blocks.Add(item);
                        }
                    }
                    else if (currentNode is Paragraph)
                    {
                        foreach (var item in node.ChildNodes)
                        {
                            ((Paragraph)currentNode).Inlines.Add(ProcessNodeInline((XmlNode)item));
                        }
                    }
                }
                if (node.NextSibling != null)
                    result.AddRange(ParseInternal(node.NextSibling));
            }
            return result;
        }

        private static Block ProcessNode(XmlNode node)
        {
            if (node.NodeType == XmlNodeType.Element)
            {
                if (node.Name == nameof(Section))
                {
                    var element = new Section();
                    foreach (XmlAttribute attribute in node.Attributes)
                    {
                        SetProperty(element, attribute.Name, attribute.Value);
                    }
                    return element;
                }
                else if (node.Name == nameof(Paragraph))
                {
                    var element = new Paragraph();
                    foreach (XmlAttribute attribute in node.Attributes)
                    {
                        SetProperty(element, attribute.Name, attribute.Value);
                    }
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
                    foreach (XmlAttribute attribute in node.Attributes)
                    {
                        SetProperty(element, attribute.Name, attribute.Value);
                    }
                    element.Text = node.InnerText;
                    return element;
                }
                else if (node.Name == nameof(Span))
                {
                    var element = new Span();
                    foreach (XmlAttribute attribute in node.Attributes)
                    {
                        SetProperty(element, attribute.Name, attribute.Value);
                    }
                    return element;
                }
                else if (node.Name == nameof(LineBreak))
                {
                    var element = new LineBreak();
                    foreach (XmlAttribute attribute in node.Attributes)
                    {
                        SetProperty(element, attribute.Name, attribute.Value);
                    }
                    return element;
                }
                else if (node.Name == nameof(InlineUIContainer))
                {
                    var element = new InlineUIContainer();
                    foreach (XmlAttribute attribute in node.Attributes)
                    {
                        SetProperty(element, attribute.Name, attribute.Value);
                    }
                    return element;
                }
            }
            return null;
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
}
