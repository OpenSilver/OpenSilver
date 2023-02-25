
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
using System.Collections.Generic;
using System.Reflection;

namespace OpenSilver.Compiler.Common
{
    public abstract class ReflectionOnSeparateAppDomainHandlerBase<TInterface, TImpl> : IDisposable
        where TInterface : class, IMarshalledObjectBase
        where TImpl : TInterface
    {
        private readonly AppDomain _newAppDomain;

        protected ReflectionOnSeparateAppDomainHandlerBase(bool isSLMigration)
        {
            _newAppDomain = AppDomain.CreateDomain(
                "newAppDomain",
                AppDomain.CurrentDomain.Evidence,
                AppDomain.CurrentDomain.SetupInformation);

            // Listen to the "AssemblyResolve" of the current domain so that when we arrive to the
            // "Unwrap" call below, we can locate the "CSharpXamlForHtml5.Compiler.Common.dll" file.
            // For information: http://forums.codeguru.com/showthread.php?398030-AppDomain-CreateInstanceAndUnwrap(-)-vs-AppDomain-CreateInstanceFrom
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve -= OnAssemblyResolve;
                AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;

                MarshalledObject = (TInterface)_newAppDomain.CreateInstanceFromAndUnwrap(
                    PathsHelper.GetPathOfAssembly(typeof(TImpl).Assembly),
                    typeof(TImpl).FullName);
                MarshalledObject.Initialize(isSLMigration);
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= OnAssemblyResolve;
            }
        }

        protected TInterface MarshalledObject { get; }

        public string LoadAssembly(string assemblyPath, bool loadReferencedAssembliesToo, bool skipReadingAttributesFromAssemblies)
            => MarshalledObject.LoadAssembly(assemblyPath, loadReferencedAssembliesToo, skipReadingAttributesFromAssemblies);

        public void LoadAssemblyAndAllReferencedAssembliesRecursively(string assemblyPath, bool skipReadingAttributesFromAssemblies, out List<string> assemblySimpleNames)
            => MarshalledObject.LoadAssemblyAndAllReferencedAssembliesRecursively(assemblyPath, skipReadingAttributesFromAssemblies, out assemblySimpleNames);

        public void LoadAssemblyMscorlib(bool isCoreAssembly) => MarshalledObject.LoadAssemblyMscorlib(isCoreAssembly);

        public void Dispose()
        {
            // Unload everything:
            AppDomain.Unload(_newAppDomain);
            GC.Collect(); // Collects all unused memory
            GC.WaitForPendingFinalizers(); // Waits until GC has finished its work
            GC.Collect();
        }

        private Assembly OnAssemblyResolve(object sender, ResolveEventArgs args) => Assembly.Load(args.Name);
    }
}
