

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



using System.Collections.Generic;

namespace DotNetForHtml5.Compiler.Common
{
    // This is referenced by both the Simulator and the MSBUILD-based compiler so that the two can communicate even though they are in separate AppDomains.

    public interface ICompilationFromSimulator
    {
        string StartCompilation(
            string sourceAssembly,
            string outputRootPath,
            string outputAppFilesPath,
            string outputLibrariesPath,
            string outputResourcesPath,
            string intermediateOutputAbsolutePath,
            string assemblyNameWithoutExtension,
            string flags,
            bool isBridgeBasedVersion
            );

        bool IsFirstTimeJavaScriptCompilation(string intermediateOutputAbsolutePath);
    }
}
