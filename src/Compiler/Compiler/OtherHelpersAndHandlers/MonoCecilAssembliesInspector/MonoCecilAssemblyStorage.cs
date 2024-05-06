
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mono.Cecil;

#if RESOURCESEXTRACTOR
namespace OpenSilver.Compiler.Resources
#else
namespace OpenSilver.Compiler
#endif
{
    public class MonoCecilAssemblyStorage : IDisposable
    {
        private readonly List<AssemblyDefinition> _assemblies = new();
        private readonly IAssemblyResolver _resolver;

        public MonoCecilAssemblyStorage()
        {
            Assemblies = new(_assemblies);
            _resolver = new AssemblyDefinitionResolver(GetCachedAssembly);
        }

        public ReadOnlyCollection<AssemblyDefinition> Assemblies { get; }

        public AssemblyDefinition LoadAssembly(string assemblyPath)
        {
            var assembly = AssemblyDefinition.ReadAssembly(assemblyPath, new ReaderParameters
            {
                AssemblyResolver = _resolver,
            });

            _assemblies.Add(assembly);

            return assembly;
        }

        public void Dispose()
        {
            foreach (AssemblyDefinition assembly in _assemblies)
            {
                assembly.Dispose();
            }

            _assemblies.Clear();
        }

        private AssemblyDefinition GetCachedAssembly(AssemblyNameReference name) =>
            _assemblies.Find(asm => asm.Name.Name == name.Name);

        private sealed class AssemblyDefinitionResolver : DefaultAssemblyResolver
        {
            private readonly Func<AssemblyNameReference, AssemblyDefinition> _resolvingStrategy;

            public AssemblyDefinitionResolver(Func<AssemblyNameReference, AssemblyDefinition> resolvingStrategy)
            {
                _resolvingStrategy = resolvingStrategy;
            }

            public override AssemblyDefinition Resolve(AssemblyNameReference name) => _resolvingStrategy(name) ?? base.Resolve(name);
        }
    }
}
