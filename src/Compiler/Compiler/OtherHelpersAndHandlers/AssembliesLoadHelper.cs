

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



using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DotNetForHtml5.Compiler
{
    internal static class AssembliesLoadHelper
    {
        /// <summary>
        /// Ensures that the core CSHTML5 assembly is the first among the provided list of assemblies.
        /// This is useful to ensure that types such as "XmlnsDefinitionAttribute" are resolved.
        /// </summary>
        /// <param name="assemblyPaths">An enumerable that contains the paths of some assemblies</param>
        /// <returns>An enumerable that contains the same items as the original enumerable but in a different order</returns>
        internal static IEnumerable<string> EnsureCoreAssemblyIsFirstInList(IEnumerable<string> assemblyPaths)
        {
            List<string> result = new List<string>();
            string coreAssemblyNameLowercase = GetCoreAssemblyName().ToLower();
            foreach (string assemblyPath in assemblyPaths)
            {
                string fileName = Path.GetFileNameWithoutExtension(assemblyPath);
                if (fileName.ToLower() == coreAssemblyNameLowercase)
                {
                    result.Insert(0, assemblyPath);
                }
                else
                {
                    result.Add(assemblyPath);
                }
            }
            return result;
        }

        internal static string GetCoreAssemblyName()
        {
#if SILVERLIGHTCOMPATIBLEVERSION
#if CSHTML5BLAZOR
            return Constants.NAME_OF_CORE_ASSEMBLY_SLMIGRATION_USING_BLAZOR;
#elif BRIDGE
            return Constants.NAME_OF_CORE_ASSEMBLY_SLMIGRATION_USING_BRIDGE;
#else
            return Constants.NAME_OF_CORE_ASSEMBLY_SLMIGRATION;
#endif
#else
#if CSHTML5BLAZOR
            return Constants.NAME_OF_CORE_ASSEMBLY_USING_BLAZOR;
#elif BRIDGE
            return Constants.NAME_OF_CORE_ASSEMBLY_USING_BRIDGE;
#else
            return Constants.NAME_OF_CORE_ASSEMBLY;
#endif
#endif
        }
    }
}
