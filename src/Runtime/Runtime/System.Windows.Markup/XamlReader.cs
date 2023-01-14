
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
using System.IO;
using System.Xaml;

namespace System.Windows.Markup
{
    /// <summary>
    /// Provides a XAML processor engine for parsing XAML and creating corresponding Silverlight object trees.
    /// </summary>
    public static class XamlReader
    {
        private static readonly Lazy<XamlSchemaContext> _xamlSharedContext = new(() => new XamlSchemaContext());

        internal static XamlSchemaContext XamlSharedContext => _xamlSharedContext.Value;

        /// <summary>
        /// Parses a well-formed XAML fragment and creates a corresponding Silverlight object tree, 
        /// and returns the root of the object tree.
        /// </summary>
        /// <returns>The root object of the Silverlight object tree.</returns>
        /// <param name="xaml">A string that contains a valid XAML fragment.</param>
        public static object Load(string xaml)
        {
            var textReader = new StringReader(xaml);

            var xamlReader = new XamlXmlReader(textReader, XamlSharedContext);
            var xamlWriter = new XamlObjectWriter(xamlReader.SchemaContext);

            if (xamlReader.NodeType == XamlNodeType.None)
                xamlReader.Read();

            var xamlLineInfo = xamlReader as IXamlLineInfo;
            var xamlLineConsumer = xamlWriter as IXamlLineInfoConsumer;
            var shouldSetLineInfo = xamlLineInfo != null && xamlLineConsumer != null && xamlLineConsumer.ShouldProvideLineInfo && xamlLineInfo.HasLineInfo;

            while (!xamlReader.IsEof)
            {
                if (shouldSetLineInfo)
                {
                    xamlLineConsumer.SetLineInfo(xamlLineInfo.LineNumber, xamlLineInfo.LinePosition);
                }
                xamlWriter.WriteNode(xamlReader);
                xamlReader.Read();
            }

            return xamlWriter.Result;
        }
    }
}