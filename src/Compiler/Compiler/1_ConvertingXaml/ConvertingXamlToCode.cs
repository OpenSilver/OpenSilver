
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
    internal abstract class ConvertingXamlToCode
    {
        public abstract string Convert(
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
            ILogger logger);
    }

    internal abstract class ConversionSettings
    {
        public IMetadata Metadata { get; set; }

        public ICoreTypesConverter CoreTypesConverter { get; set; }

        public bool EnableImplicitAssemblyRedirection { get; set; }
    }
}