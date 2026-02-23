
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

using Microsoft.CodeAnalysis;
using System;

namespace OpenSilver.CodeAnalysis;

internal static class Helpers
{
    public static IAssemblySymbol FindOpenSilverAssembly(Compilation compilation)
    {
        foreach (var assembly in compilation.SourceModule.ReferencedAssemblySymbols)
        {
            if (IsOpenSilverAssembly(assembly))
            {
                return assembly;
            }
        }

        // In unit tests the stub types are defined in the test project itself.
        if (compilation.Assembly.Name.Equals("TestProject", StringComparison.Ordinal))
        {
            return compilation.Assembly;
        }

        return null;
    }

    private static bool IsOpenSilverAssembly(IAssemblySymbol assembly) => assembly.Name.Equals("OpenSilver", StringComparison.Ordinal);
}
