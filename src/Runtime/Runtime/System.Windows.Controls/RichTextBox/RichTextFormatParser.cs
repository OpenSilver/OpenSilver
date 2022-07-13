#if MIGRATION
using DotNetForHtml5.Core;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Documents;
using System.Xml;

namespace System.Windows.Controls
#else
using DotNetForHtml5.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using Windows.UI.Xaml.Documents;

namespace Windows.UI.Xaml.Controls
#endif
{
    internal class RichTextFormatParser
    {
        public IEnumerable<Block> Parse(string xaml)
        {
            List<Block> result = new List<Block>();
            Paragraph _currentParagraph = null;

            using (var textReader = new StringReader(xaml))
            using (XmlReader reader = XmlReader.Create(textReader))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == nameof(Section))
                        {
                            continue;//Section is not supported in SilverLight
                        }
                        else if (reader.Name == nameof(Paragraph))
                        {
                            if (reader.IsStartElement())
                            {
                                var paragraph = new Paragraph();
                                if (reader.HasAttributes)
                                {
                                    while (reader.MoveToNextAttribute())
                                    {
                                        //SetProperty(paragraph, reader.Name, reader.Value);
                                    }

                                    reader.MoveToElement();
                                }

                                result.Add(paragraph);
                                _currentParagraph = paragraph;
                            }
                            else
                            {
                                _currentParagraph = null;
                                continue;
                            }
                        }
                        else if (reader.Name == nameof(Run))
                        {
                            if (_currentParagraph == null)//Run must belong to a paragraph
                                continue;

                            if (reader.IsStartElement())
                            {
                                var run = new Run();
                                if (reader.HasAttributes)
                                {
                                    while (reader.MoveToNextAttribute())
                                    {
                                        //SetProperty(run, reader.Name, reader.Value);
                                    }

                                    reader.MoveToElement();
                                }

                                reader.Read();//Read content
                                run.Text = reader.Value;
                                _currentParagraph.Inlines.Add(run);
                            }
                        }
                    }
                }
            }

            return result;
        }

        private void SetProperty(object obj, string propertyName, object value)
        {
            var propInfo = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            if (propInfo != null)
            {
                try
                {
                    var val = value == null ? null : TypeFromStringConverters.ConvertFromInvariantString(propInfo.PropertyType, value.ToString());
                    propInfo.SetValue(obj, val);
                }
                catch (Exception ex)
                {
                    return;
                }
            }
        }
    }
}
