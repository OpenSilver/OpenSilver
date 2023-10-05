
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
using ILogger = OpenSilver.Compiler.Common.ILogger;

namespace OpenSilver.Compiler
{
    internal static class ConvertingXamlToCSharp
    {
        public static string Convert(
            string xaml,
            string sourceFile,
            string fileNameWithPathRelativeToProjectRoot,
            string assemblyNameWithoutExtension,
            AssembliesInspector reflectionOnSeparateAppDomain,
            bool isFirstPass,
            bool isSLMigration,
            string outputRootPath,
            string outputAppFilesPath,
            string outputLibrariesPath,
            string outputResourcesPath,
            ILogger logger)
        {
            ConversionSettings settings = isSLMigration ? ConversionSettingsCS.Silverlight : ConversionSettingsCS.UWP;

            // Process the "HtmlPresenter" nodes in order to "escape" its content, because the content is HTML and it could be badly formatted and not be parsable using XDocument.Parse.
            xaml = ProcessingHtmlPresenterNodes.Process(xaml);

            // Parse the XML:
            XDocument doc = XDocument.Parse(xaml, LoadOptions.SetLineInfo);

            // Process the "TextBlock" and "Span" nodes in order to surround direct text content with "<Run>" tags:
            ProcessingTextBlockNodes.Process(doc, reflectionOnSeparateAppDomain);

            // Insert implicit nodes in XAML:
            if (!isFirstPass) // Note: we skip this step during the 1st pass because some types are not known yet, so we cannot determine the default "ContentProperty".
            {
                InsertingImplicitNodes.InsertImplicitNodes(doc, reflectionOnSeparateAppDomain, settings, "global::", new SystemTypesHelperCS());

                FixingPropertiesOrder.FixPropertiesOrder(doc, reflectionOnSeparateAppDomain, settings);

                // Process the "ContentPresenter" nodes in order to transform "<ContentPresenter />" into "<ContentPresenter Content="{TemplateBinding Content}" ContentTemplate="{TemplateBinding ContentTemplate}" />"
                ProcessingContentPresenterNodes.Process(doc, reflectionOnSeparateAppDomain);

                // Process the "ColorAnimation" nodes in order to transform "Storyboard.TargetProperty="XXX"" into "Storyboard.TargetProperty="XXX.Color"" if XXX doesn't end on ".Color" or ".Color)"
                ProcessingColorAnimationNodes.Process(doc, reflectionOnSeparateAppDomain);

                // Convert markup extensions into XDocument nodes:
                InsertingMarkupNodesInXaml.InsertMarkupNodes(doc, reflectionOnSeparateAppDomain, settings);
            }

            // Generate unique names for XAML elements:
            GeneratingUniqueNames.ProcessDocument(doc);

            // Prepare the code that will be put in the "InitializeComponent" of the Application class, which means that it will be executed when the application is launched:
            string codeToPutInTheInitializeComponentOfTheApplicationClass = string.Format(@"
global::CSHTML5.Internal.StartupAssemblyInfo.OutputRootPath = @""{0}"";
global::CSHTML5.Internal.StartupAssemblyInfo.OutputAppFilesPath = @""{1}"";
global::CSHTML5.Internal.StartupAssemblyInfo.OutputLibrariesPath = @""{2}"";
global::CSHTML5.Internal.StartupAssemblyInfo.OutputResourcesPath = @""{3}"";
", outputRootPath, outputAppFilesPath, outputLibrariesPath, outputResourcesPath);

            // Generate C# code from the tree:
            return GeneratingCSCode.GenerateCode(
                doc,
                sourceFile,
                fileNameWithPathRelativeToProjectRoot,
                assemblyNameWithoutExtension,
                reflectionOnSeparateAppDomain,
                isFirstPass,
                settings,
                codeToPutInTheInitializeComponentOfTheApplicationClass,
                logger);

        }
    }
}