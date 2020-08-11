

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



using DotNetForHtml5.Compiler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetForHtml5.Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
#if NO_LONGER_USED
            // Create the logger:
            ILogger logger = new LoggerThatUsesConsoleWriteLine();

            // Define main configuration:
            string csprojFilePath = @"..\..\..\InputProject\InputProject.csproj";
            string outputPath = @"C:\DotNetForHtml5\TestOuptut2";
            List<string> coreAssembliesForUseByXamlToCSharpConverter = new List<string>()
                {
                    //@"C:\Program Files (x86)\Windows Kits\8.0\References\CommonConfiguration\Neutral\Windows.winmd"
                    Path.GetFullPath(@"..\..\..\DotNetForHtml5.Core\bin\ForUseByXamlToCSharpConverter\CSharpXamlForHtml5.dll")
                };
            List<string> coreAssembliesForUseByCSharpToExeOrDllCompiler = new List<string>()
                {
                    Path.GetFullPath(@"..\..\..\DotNetForHtml5.Core\bin\Debug\CSharpXamlForHtml5.dll"),
                    //Path.GetFullPath(@"..\..\..\DotNetForHtml5.Proxies\bin\Debug\DotNetForHtml5.Proxies.dll"),
                };
            List<string> librariesFolders = new List<string>()
            {
                Path.GetFullPath(@"..\..\..\JSIL\Libraries"),
                Path.GetFullPath(@"..\..\..\OtherLibraries")
            };
            using (var reflectionOnSeparateAppDomain = new ReflectionOnSeparateAppDomainHandler())
            {
                reflectionOnSeparateAppDomain.LoadAssembly(Path.GetFullPath(@"..\..\..\DotNetForHtml5.Core\bin\ForUseByXamlToCSharpConverter\CSharpXamlForHtml5.dll"));

                //todo: call reflectionOnSeparateAppDomain.LoadAssembly to also load the assemblies that are referenced by the entry-point project

                // Start compilation:
                StandaloneCompilerTest.ProcessProject(
                    Path.GetFullPath(csprojFilePath),
                    outputPath,
                    librariesFolders,
                    reflectionOnSeparateAppDomain,
                    coreAssembliesForUseByXamlToCSharpConverter,
                    coreAssembliesForUseByCSharpToExeOrDllCompiler,
                    logger);
            }
            // Finish:
            logger.WriteMessage("");
            logger.WriteMessage("-------- COMPILATION FINISHED: Press any key to close --------");
            Console.ReadLine();
#endif
        }
    }
}
