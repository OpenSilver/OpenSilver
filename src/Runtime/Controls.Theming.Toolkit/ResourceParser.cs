// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;

namespace System.Windows.Controls.Theming
{
    /// <summary>
    /// Parse streams containing theme resources.
    /// </summary>
    internal static partial class ResourceParser
    {
        /// <summary>
        /// The default old XAML namespace.
        /// </summary>
        private const string OldXamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";

        /// <summary>
        /// The default new XAML namespace.
        /// </summary>
        private const string NewXamlNamespace = "http://schemas.microsoft.com/client/2007";

        /// <summary>
        /// The XAML markup for an empty Style declaration.
        /// </summary>
        private const string StyleXaml = "<Style xmlns=\"" + NewXamlNamespace + "\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" TargetType=\"{0}\" />";

        /// <summary>
        /// The XAML markup for an empty Style declaration with a TargetType
        /// using an XML prefix.
        /// </summary>
        private const string StyleXamlWithPrefix = "<Style xmlns=\"" + NewXamlNamespace + "\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" xmlns:{1}=\"{2}\" TargetType=\"{0}\" />";

        /// <summary>
        /// Determines whether an XML element is a type in the core Silverlight
        /// namespace.
        /// </summary>
        /// <typeparam name="T">The type to check for.</typeparam>
        /// <param name="name">The name of the element.</param>
        /// <param name="ns">The namespace of the element.</param>
        /// <returns>
        /// A value indicating whether an the element is the Silverlight type.
        /// </returns>
        private static bool IsXamlElement<T>(string name, string ns)
        {
            return name == typeof(T).Name && (ns == OldXamlNamespace || ns == NewXamlNamespace);
        }

        /// <summary>
        /// Retrieves a resource dictionary from an input stream.
        /// </summary>
        /// <param name="stream">
        /// A stream containing the XAML for a  resource dictionary.
        /// </param>
        /// <param name="checkTypes">
        /// A value indicating whether styles in the resource dictionary should
        /// be filtered to types that have been loaded by the application.
        /// </param>
        /// <returns>A resource dictionary.</returns>
        public static ResourceDictionary Parse(Stream stream, bool checkTypes)
        {
            Debug.Assert(stream != null, "stream cannot be null.");

            // Transform a resource dictionary tag into content control tag and
            // nest all its contents inside a ContentControl.Resources tag. This
            // is necessary because a resource dictionary parsed with XamlReader
            // has different semantics than one attached to a control.  A 
            // resource dictionary attached to a control allows lookup by key.
            // A resource dictionary created with a constructor or deserialized
            // with XamlReader does not allow key lookups.

            string xaml;
            using (MemoryStream outputStream = new MemoryStream())
            {
                using (XmlWriter writer = XmlWriter.Create(outputStream))
                {
                    using (XmlReader reader = XmlReader.Create(new StreamReader(stream)))
                    {
                        ParseResources(reader, writer, checkTypes);
                    }
                }
                outputStream.Seek(0, SeekOrigin.Begin);
                xaml = new StreamReader(outputStream).ReadToEnd();
            }

            return XamlReader.Load(xaml) as ResourceDictionary;
        }

        /// <summary>
        /// Transform a ResourceDictionary.
        /// </summary>
        /// <param name="reader">Reader with the resources.</param>
        /// <param name="writer">Writer with the transformed resources.</param>
        /// <param name="checkTypes">
        /// A value indicating whether styles in the resource dictionary should
        /// be filtered to types that have been loaded by the application.
        /// </param>
        private static void ParseResources(XmlReader reader, XmlWriter writer, bool checkTypes)
        {
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        ParseElement(reader, writer, checkTypes);
                        break;
                    case XmlNodeType.Text:
                        writer.WriteString(reader.Value);
                        break;
                    case XmlNodeType.Whitespace:
                    case XmlNodeType.SignificantWhitespace:
                        writer.WriteWhitespace(reader.Value);
                        break;
                    case XmlNodeType.CDATA:
                        writer.WriteCData(reader.Value);
                        break;
                    case XmlNodeType.EntityReference:
                        writer.WriteEntityRef(reader.Name);
                        break;
                    case XmlNodeType.XmlDeclaration:
                    case XmlNodeType.ProcessingInstruction:
                        writer.WriteProcessingInstruction(reader.Name, reader.Value);
                        break;
                    case XmlNodeType.DocumentType:
                        writer.WriteDocType(reader.Name, reader.GetAttribute("PUBLIC"), reader.GetAttribute("SYSTEM"), reader.Value);
                        break;
                    case XmlNodeType.Comment:
                        writer.WriteComment(reader.Value);
                        break;
                    case XmlNodeType.EndElement:
                        writer.WriteFullEndElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Process an element's opening tag.
        /// </summary>
        /// <param name="reader">Reader with the resources.</param>
        /// <param name="writer">Writer with the transformed resources.</param>
        /// <param name="checkTypes">
        /// A value indicating whether styles in the resource dictionary should
        /// be filtered to types that have been loaded by the application.
        /// </param>
        private static void ParseElement(XmlReader reader, XmlWriter writer, bool checkTypes)
        {
            // Skip this element if its type hasn't been loaded
            if (checkTypes && reader.Depth == 1 && !IsStyleTargetTypeLoaded(reader))
            {
                reader.Skip();
                return;
            }

            bool isEmpty = reader.IsEmptyElement;

            if (reader.Depth == 0)
            {
                // Only allow non-Empty ResourceDictionary root elements
                if (!IsXamlElement<ResourceDictionary>(reader.LocalName, reader.NamespaceURI))
                {
                    throw new InvalidOperationException(Properties.Resources.ResourceParser_CanOnlyParseXAMLFilesWithResourceDictionaryAsTheRootElement);
                }
            }

            // Write the element and its attributes
            writer.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
            writer.WriteAttributes(reader, true);

            // Close the element now if empty, otherwise process its content
            if (isEmpty)
            {
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Determine if the current element in the reader represents a Style
        /// whose TargetType has been loaded in the assembly.
        /// </summary>
        /// <param name="reader">Reader with the resources.</param>
        /// <returns>
        /// A value indicating whether the element is not a style or it is a
        /// Style with a TargetType loaded in this assembly.
        /// </returns>
        private static bool IsStyleTargetTypeLoaded(XmlReader reader)
        {
            // If there's no type to load, return true
            if (!IsXamlElement<Style>(reader.Name, reader.NamespaceURI))
            {
                return true;
            }
            string targetType = reader.GetAttribute("TargetType", null);
            if (string.IsNullOrEmpty(targetType))
            {
                return true;
            }

            // Get the optional prefix of the TargetType
            string prefix = null;
            int prefixIndex = targetType.IndexOf(':');
            if (prefixIndex > 0)
            {
                prefix = targetType.Substring(0, prefixIndex);
            }

            // Create the XAML markup for an empty Style declaration
            string xaml = prefix == null ?
                string.Format(
                    CultureInfo.InvariantCulture,
                    StyleXaml,
                    targetType) :
                string.Format(
                    CultureInfo.InvariantCulture,
                    StyleXamlWithPrefix,
                    targetType,
                    prefix,
                    reader.LookupNamespace(prefix));

            try
            {
                // Ensure we can load the Style (which doesn't work if it's
                // TargetType hasn't been loaded)
                Style style = XamlReader.Load(xaml) as Style;
                return style != null;
            }
            catch (XamlParseException)
            {
                return false;
            }
        }
    }
}