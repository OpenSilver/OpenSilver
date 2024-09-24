
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
using System.ComponentModel;
using System.Globalization;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml;
using OpenSilver.Internal;

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
            if (node.Name == nameof(InlineImageContainer))
            {
                var element = new InlineImageContainer();
                SetProperties(element, node);
                return element;
            }
            if (node.Name == nameof(InlineUIContainer))
            {
                var element = new InlineUIContainer();
                SetProperties(element, node);
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

    private static void SetProperty(object obj, string propertyName, string xamlValue)
    {
        if (TypeConverterHelper.GetProperties(obj.GetType())[propertyName] is ReflectedPropertyData property)
        {
            if (property.Converter is TypeConverter converter)
            {
                object value = converter.ConvertFromInvariantString(xamlValue);
                property.PropertyInfo.SetValue(obj, value);
            }
        }
    }

    public static string ToXaml(BlockCollection blockCollection)
    {
        var blocks = blockCollection.InternalItems;

        if (blocks.Count == 0)
        {
            return string.Empty;
        }

        var document = new XmlDocument();
        document.LoadXml("<Section xml:space=\"preserve\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"></Section>");

        bool isEmpty = true;

        for (int i = 0; i < blocks.Count - 1; i++)
        {
            if (ProcessBlock(blocks[i], document, true))
            {
                isEmpty = false;
            }
        }

        if (ProcessBlock(blocks[blocks.Count - 1], document, false))
        {
            isEmpty = false;
        }

        return isEmpty ? string.Empty : document.OuterXml;
    }

    /// <summary>
    /// Add a block's content to a xml document.
    /// </summary>
    /// <returns>
    /// true if the block is not empty (i.e. it contains at least one non-empty Inline if it is a 
    /// Paragraph, or at least one non empty paragraph if it is a Section), false otherwise.
    /// </returns>
    private static bool ProcessBlock(Block block, XmlDocument document, bool allowEmpty)
    {
        switch (block)
        {
            case Paragraph paragraph:
                var xmlParagraph = XmlParagraph(paragraph, document);
                if (allowEmpty || !xmlParagraph.IsEmpty)
                {
                    document.DocumentElement.AppendChild(xmlParagraph.Paragraph);
                    return true;
                }
                break;

            case Section section:
                var blocks = section.Blocks.InternalItems;
                bool isEmpty = true;
                if (blocks.Count > 0)
                {
                    for (int i = 0; i < blocks.Count - 1; i++)
                    {
                        if (ProcessBlock(blocks[i], document, true))
                        {
                            isEmpty = false;
                        }
                    }

                    if (ProcessBlock(blocks[blocks.Count - 1], document, allowEmpty))
                    {
                        isEmpty = false;
                    }
                }
                return isEmpty;
        }

        return false;
    }

    private static (XmlElement Paragraph, bool IsEmpty) XmlParagraph(Paragraph paragraph, XmlDocument document)
    {
        var xmlBlock = document.CreateElement(nameof(Paragraph), document.DocumentElement.NamespaceURI);
        bool isEmpty = true;

        xmlBlock.SetAttribute(nameof(Block.TextAlignment), paragraph.TextAlignment.ToString());
        xmlBlock.SetAttribute(nameof(Block.LineHeight), paragraph.LineHeight.ToInvariantString());

        foreach (Inline inline in paragraph.Inlines)
        {
            if (ProcessInline(inline, xmlBlock))
            {
                isEmpty = false;
            }
        }

        return (xmlBlock, isEmpty);
    }

    /// <summary>
    /// Add a inline's content to a xml document.
    /// </summary>
    /// <returns>
    /// true if the inline is not empty (i.e. it is a Run with non empty text or a LineBreak
    /// or a Span that contains at least one non empty Inline), false otherwise.
    /// </returns>
    private static bool ProcessInline(Inline inline, XmlNode parent)
    {
        XmlDocument document = parent.OwnerDocument;

        switch (inline)
        {
            case Run run:
                if (!string.IsNullOrEmpty(run.Text))
                {
                    parent.AppendChild(XmlRun(run, document));
                    return true;
                }
                break;

            case LineBreak:
                parent.AppendChild(XmlLineBreak(document));
                return true;

            case Span span:
                var xmlSpan = XmlSpan(span, document);
                parent.AppendChild(xmlSpan.Span);
                return !xmlSpan.IsEmpty;
        }

        return false;
    }

    private static XmlElement XmlRun(Run run, XmlDocument document)
    {
        var xmlRun = document.CreateElement(nameof(Run), document.DocumentElement.NamespaceURI);

        xmlRun.SetAttribute(nameof(Run.Text), run.Text);
        xmlRun.SetAttribute(nameof(TextElement.CharacterSpacing), run.CharacterSpacing.ToInvariantString());
        xmlRun.SetAttribute(nameof(TextElement.FontFamily), run.FontFamily.Source);
        xmlRun.SetAttribute(nameof(TextElement.FontSize), run.FontSize.ToInvariantString());
        xmlRun.SetAttribute(nameof(TextElement.FontStyle), run.FontStyle.ToString());
        xmlRun.SetAttribute(nameof(TextElement.FontWeight), run.FontWeight.ToString());
        xmlRun.SetAttribute(nameof(Inline.TextDecorations), TextDecorationCollection.ToString(run.TextDecorations));
        if (run.Foreground is SolidColorBrush foreground)
        {
            xmlRun.SetAttribute(nameof(TextElement.Foreground), foreground.Color.ToString(CultureInfo.InvariantCulture));
        }

        return xmlRun;
    }

    private static XmlElement XmlLineBreak(XmlDocument document)
        => document.CreateElement(nameof(LineBreak), document.DocumentElement.NamespaceURI);

    private static (XmlElement Span, bool IsEmpty) XmlSpan(Span span, XmlDocument document)
    {
        var xmlSpan = document.CreateElement(span.GetType().Name, document.DocumentElement.NamespaceURI);
        bool isEmpty = true;

        foreach (Inline inline in span.Inlines)
        {
            if (ProcessInline(inline, xmlSpan))
            {
                isEmpty = false;
            }
        }

        return (xmlSpan, isEmpty);
    }
}
