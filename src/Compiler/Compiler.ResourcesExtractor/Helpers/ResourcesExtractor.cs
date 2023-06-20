
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


using Mono.Cecil;
using OpenSilver.Compiler.Common.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OpenSilver.Compiler.Resources.Helpers
{
    internal class ResourcesExtractor
    {
        private const string AssemblyNotInListOfLoadedAssemblies = "The specified assembly is not in the list of loaded assemblies.";

        private static string GetExtension(string str)
        {
            try
            {
                return Path.GetExtension(str);
            }
            catch
            {
                //It is possible that resource does not have an extension
                return null;
            }
        }

        public Dictionary<string, byte[]> GetManifestResources(MonoCecilAssemblyStorage storage, string assemblySimpleName)
        {
            if (!storage.LoadedAssemblySimpleNameToAssembly.ContainsKey(assemblySimpleName))
                throw new Exception(AssemblyNotInListOfLoadedAssemblies);

            var assembly = storage.LoadedAssemblySimpleNameToAssembly[assemblySimpleName];

            var manifestResourceNames = assembly.MainModule.Resources.Select(r => r.Name);
            var resourceFiles = manifestResourceNames.Where(x => GetExtension(x.ToLower()) != ".xaml").ToArray();
            var result = new Dictionary<string, byte[]>();

            foreach (var resourceFile in resourceFiles)
            {
                var resource = assembly.MainModule.Resources.FirstOrDefault(r => r.Name == resourceFile);
                if (resource == null)
                    throw new FileNotFoundException("No manifest resource stream named " + resourceFile);

                result[resourceFile] = ((EmbeddedResource)resource).GetResourceData();
            }
            return result;
        }
    }
}
