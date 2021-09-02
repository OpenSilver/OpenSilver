

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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DotNetForHtml5.Compiler
{
    internal static class AssembliesLoadHelper
    {
        /// <summary>
        /// This collection represents the Core assemblies in the loading order.
        /// </summary>
        internal static IReadOnlyCollection<string> CoreAssembliesNames { get; } = new Collection<string>()
        {
            Constants.OPENSILVER_XAML_ASSEMBLY_NAME,
            GetCoreAssemblyName(),
        };

        internal static bool IsCoreAssembly(string assemblyPath)
        {
            var fileName = Path.GetFileNameWithoutExtension(assemblyPath);

            return CoreAssembliesNames.Any(assemblyName => assemblyName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase));
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
