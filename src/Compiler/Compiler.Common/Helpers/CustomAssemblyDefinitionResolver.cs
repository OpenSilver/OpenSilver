
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
using Mono.Cecil;

namespace OpenSilver.Compiler.Common.Helpers
{
    internal class CustomAssemblyDefinitionResolver : BaseAssemblyResolver
    {
        private readonly Func<string, AssemblyDefinition> _resolvingStrategy;
        private readonly DefaultAssemblyResolver _defaultResolver = new ();

        public CustomAssemblyDefinitionResolver(Func<string, AssemblyDefinition> resolvingStrategy)
        {
            _resolvingStrategy = resolvingStrategy;
        }

        public override AssemblyDefinition Resolve(AssemblyNameReference name)
        {
            return _resolvingStrategy(name.Name) ?? _defaultResolver.Resolve(name);
        }
    }
}
