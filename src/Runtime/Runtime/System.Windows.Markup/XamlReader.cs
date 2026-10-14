
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

using OpenSilver.Internal.Xaml;
using System.IO;
using System.Xaml;
using System.Xml;

namespace System.Windows.Markup;

/// <summary>
/// Provides a XAML processor engine for parsing XAML and creating corresponding Silverlight object trees.
/// </summary>
public class XamlReader
{
    private static readonly Lazy<XamlSchemaContext> _sharedXamlContext = new(() => new OpenSilverXamlSchemaContext());

    internal static XamlSchemaContext SharedXamlContext => _sharedXamlContext.Value;

    /// <summary>
    /// Reads the XAML input in the specified text string and returns an object that corresponds 
    /// to the root of the specified markup.
    /// </summary>
    /// <param name="xamlText">
    /// The input XAML, as a single text string.
    /// </param>
    /// <returns>
    /// The root of the created object tree.
    /// </returns>
    public static object Parse(string xamlText)
    {
        var textReader = new StringReader(xamlText);
        var xamlReader = new XamlXmlReader(textReader, SharedXamlContext);
        return Load(xamlReader);
    }

    /// <summary>
    /// Parses a well-formed XAML fragment and creates a corresponding Silverlight object tree, 
    /// and returns the root of the object tree.
    /// </summary>
    /// <param name="xaml">
    /// A string that contains a valid XAML fragment.
    /// </param>
    /// <returns>
    /// The root object of the Silverlight object tree.
    /// </returns>
    public static object Load(string xaml) => Parse(xaml);

    /// <summary>
    /// Reads the XAML input in the specified <see cref="Stream"/> and returns an 
    /// <see cref="object"/> that is the root of the corresponding object tree.
    /// </summary>
    /// <param name="stream">
    /// The XAML to load, in stream form.
    /// </param>
    /// <returns>
    /// The object at the root of the created object tree.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="stream"/> is null.
    /// </exception>
    public static object Load(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        var streamReader = new StreamReader(stream);
        var xamlReader = new XamlXmlReader(streamReader, SharedXamlContext);
        var tree = Load(xamlReader);
        stream.Close();
        return tree;
    }

    /// <summary>
    /// Reads the XAML input in the specified <see cref="XmlReader"/> and returns an object 
    /// that is the root of the corresponding object tree.
    /// </summary>
    /// <param name="reader">
    /// The <see cref="XmlReader"/> that has already loaded the XAML input to load in XML form.
    /// </param>
    /// <returns>
    /// The object that is the root of the created object tree.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="reader"/> is null.
    /// </exception>
    public static object Load(XmlReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);

        var xamlReader = new XamlXmlReader(reader, SharedXamlContext);
        var tree = Load(xamlReader);
        reader.Close();
        return tree;
    }

    /// <summary>
    /// Reads the XAML input through a provided <see cref="XamlReader"/> and returns an object 
    /// that is the root of the corresponding object tree.
    /// </summary>
    /// <param name="reader">
    /// A <see cref="XamlReader"/> object. This is expected to be initialized with input XAML.
    /// </param>
    /// <returns>
    /// The object that is the root of the created object tree.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="reader"/> is null.
    /// </exception>
    public static object Load(Xaml.XamlReader reader)
    {
        var nameScope = new NameScope();
        var xamlWriter = new XamlObjectWriter(reader.SchemaContext, new XamlObjectWriterSettings
        {
            ExternalNameScope = nameScope,
            RegisterNamesOnExternalNamescope = true,
        });

        if (reader.NodeType == XamlNodeType.None)
        {
            reader.Read();
        }

        var xamlLineInfo = reader as IXamlLineInfo;
        var xamlLineConsumer = xamlWriter as IXamlLineInfoConsumer;
        var shouldSetLineInfo = xamlLineInfo != null && xamlLineConsumer != null && xamlLineConsumer.ShouldProvideLineInfo && xamlLineInfo.HasLineInfo;

        while (!reader.IsEof)
        {
            if (shouldSetLineInfo)
            {
                xamlLineConsumer.SetLineInfo(xamlLineInfo.LineNumber, xamlLineInfo.LinePosition);
            }
            xamlWriter.WriteNode(reader);
            reader.Read();
        }

        if (xamlWriter.Result is FrameworkElement rootElement)
        {
            if (NameScope.GetNameScope(rootElement) is INameScope rootNameScope)
            {
                foreach (var pair in nameScope)
                {
                    rootNameScope.RegisterName(pair.Key, pair.Value);
                }
            }
            else
            {
                NameScope.SetNameScope(rootElement, nameScope);
            }
        }

        return xamlWriter.Result;
    }
}