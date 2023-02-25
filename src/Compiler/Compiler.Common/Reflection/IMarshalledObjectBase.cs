
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

namespace OpenSilver.Compiler.Common
{
    public interface IMarshalledObjectBase
    {
        string LoadAssembly(string assemblyPath, bool loadReferencedAssembliesToo, bool skipReadingAttributesFromAssemblies);

        void LoadAssemblyAndAllReferencedAssembliesRecursively(
            string assemblyPath,
            bool skipReadingAttributesFromAssemblies,
            out List<string> assemblySimpleNames);

        void LoadAssemblyMscorlib(bool isCoreAssembly);

        void Initialize(bool isSLMigration);
    }
}
