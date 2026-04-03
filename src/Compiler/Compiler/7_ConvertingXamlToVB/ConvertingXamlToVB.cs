
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

using System.Xml.Linq;

namespace OpenSilver.Compiler
{
    internal static class ConvertingXamlToVB
    {
        public static string Convert(
            string xaml,
            string sourceFile,
            string fileNameWithPathRelativeToProjectRoot,
            string rootNamespace,
            ConversionSettings settings,
            bool isFirstPass)
        {
            // Process the "HtmlPresenter" nodes in order to "escape" its content, because the content is HTML and it
            // could be badly formatted and not be parsable using XDocument.Parse.
            xaml = ProcessingHtmlPresenterNodes.Process(xaml);

            XDocument doc = XDocumentHelper.Parse(xaml, LoadOptions.SetLineInfo);

            if (!isFirstPass)
            {
                // Generate paths for XAML elements before any manipulations:
                GeneratingPathInXaml.ProcessDocument(doc);

                // Process the "TextBlock" and "Span" nodes in order to surround direct text content with "<Run>" tags:
                ProcessingTextBlockNodes.Process(doc, settings);

                InsertingImplicitNodes.InsertImplicitNodes(doc, settings);

                // Resolve {x:Type ...} in TargetType/DataType attributes to plain type names
                // before markup extension processing converts them to child elements
                ProcessTypeExtensionAttributes.Process(doc, settings);

                // Process the "ContentPresenter" nodes in order to transform "<ContentPresenter />" into
                // "<ContentPresenter Content="{TemplateBinding Content}" ContentTemplate="{TemplateBinding ContentTemplate}" />"
                ProcessingContentPresenterNodes.Process(doc, settings);

                // Convert markup extensions into XDocument nodes:
                InsertingMarkupNodesInXaml.InsertMarkupNodes(doc, settings);
            }

            // Generate unique names for XAML elements:
            GeneratingUniqueNames.ProcessDocument(doc);

            // Generate Vb code from the tree:
            return GeneratingVBCode.GenerateCode(
                doc,
                sourceFile,
                fileNameWithPathRelativeToProjectRoot,
                rootNamespace,
                isFirstPass,
                settings);
        }
    }
}