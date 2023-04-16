
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
using System.IO;
using System.Linq;
using Mono.Cecil;

namespace OpenSilver.Compiler.Common.Helpers
{
    public class MonoCecilAssemblyStorage : IDisposable
    {
        private readonly DefaultAssemblyResolver _defaultResolver = new();
        private const string XmlnsDefinitionAttributeFullName = "System.Windows.Markup.XmlnsDefinitionAttribute";

        private readonly Dictionary<string, Dictionary<string, HashSet<string>>>
            _assemblyNameToXmlNamespaceToClrNamespaces = new();

        private Dictionary<string, AssemblyDefinition> _loadedAssemblySimpleNameToAssembly =
            new();

        private CustomAssemblyDefinitionResolver _customAssemblyDefinitionResolver;

        public MonoCecilAssemblyStorage()
        {
            _customAssemblyDefinitionResolver = new CustomAssemblyDefinitionResolver(assemblyName =>
            {
                if (!_loadedAssemblySimpleNameToAssembly.ContainsKey(assemblyName))
                {
                    return null;
                }
                var res = _loadedAssemblySimpleNameToAssembly[assemblyName];
                return res;
            });
        }

        public void Dispose()
        {
            foreach (var kvp in _loadedAssemblySimpleNameToAssembly) kvp.Value.Dispose();

            _loadedAssemblySimpleNameToAssembly.Clear();
            _customAssemblyDefinitionResolver.Dispose();
            _loadedAssemblySimpleNameToAssembly = null;
            _customAssemblyDefinitionResolver = null;
        }

        public AssemblyDefinition ReadAssembly(string assemblyPath)
        {
            return AssemblyDefinition.ReadAssembly(assemblyPath, new ReaderParameters
            {
                AssemblyResolver = _customAssemblyDefinitionResolver
            });
        }

        public Dictionary<string, AssemblyDefinition> LoadedAssemblySimpleNameToAssembly => _loadedAssemblySimpleNameToAssembly;

        public Dictionary<string, Dictionary<string, HashSet<string>>> AssemblyNameToXmlNamespaceToClrNamespaces =>
            _assemblyNameToXmlNamespaceToClrNamespaces;

        private void ReadXmlnsDefinitionAttributes(AssemblyDefinition assembly)
        {
            var assemblySimpleName = assembly.Name.Name;

            // Extract the "XmlnsDefinition" attributes defined in the "AssemblyInfo.cs" files, for use with XAML namespace mappings:
            Dictionary<string, HashSet<string>> xmlNamespaceToClrNamespaces = null;
            if (_assemblyNameToXmlNamespaceToClrNamespaces.ContainsKey(assemblySimpleName))
                xmlNamespaceToClrNamespaces = _assemblyNameToXmlNamespaceToClrNamespaces[assemblySimpleName];

            var attributes = assembly.CustomAttributes.Where(x =>
                x.AttributeType.FullName == XmlnsDefinitionAttributeFullName).ToList();

            foreach (var attribute in attributes)
            {
                var xmlNamespace = (attribute.ConstructorArguments[0].Value ?? "").ToString();
                var clrNamespace = (attribute.ConstructorArguments[1].Value ?? "").ToString();

                if (string.IsNullOrEmpty(xmlNamespace) || string.IsNullOrEmpty(clrNamespace)) continue;

                if (xmlNamespaceToClrNamespaces == null)
                {
                    xmlNamespaceToClrNamespaces = new Dictionary<string, HashSet<string>>();
                    _assemblyNameToXmlNamespaceToClrNamespaces.Add(assemblySimpleName,
                        xmlNamespaceToClrNamespaces);
                }

                HashSet<string> clrNamespacesAssociatedToThisXmlNamespace;
                if (xmlNamespaceToClrNamespaces.ContainsKey(xmlNamespace))
                {
                    clrNamespacesAssociatedToThisXmlNamespace = xmlNamespaceToClrNamespaces[xmlNamespace];
                }
                else
                {
                    clrNamespacesAssociatedToThisXmlNamespace = new HashSet<string>();
                    xmlNamespaceToClrNamespaces.Add(xmlNamespace, clrNamespacesAssociatedToThisXmlNamespace);
                }

                if (!clrNamespacesAssociatedToThisXmlNamespace.Contains(clrNamespace))
                    clrNamespacesAssociatedToThisXmlNamespace.Add(clrNamespace);
            }
        }

        public HashSet<string> LoadAssembly(string assemblyPath, bool loadReferencedAssembliesToo = false,
            bool skipReadingAttributesFromAssemblies = false)
        {
            var loadedAssemblyNames = new HashSet<string>();
            var queue = new Queue<AssemblyDefinition>();
            queue.Enqueue(ReadAssembly(assemblyPath));
            while (queue.Any())
            {
                var assembly = queue.Dequeue();
                _loadedAssemblySimpleNameToAssembly[assembly.Name.Name] = assembly;
                loadedAssemblyNames.Add(assembly.Name.Name);
                if (!skipReadingAttributesFromAssemblies)
                {
                    ReadXmlnsDefinitionAttributes(assembly);
                }

                if (!loadReferencedAssembliesToo)
                {
                    return loadedAssemblyNames;
                }
                var referencedAssemblies = assembly.MainModule.AssemblyReferences;

                foreach (var referencedAssembly in referencedAssemblies)
                {
                    if (_loadedAssemblySimpleNameToAssembly.ContainsKey(referencedAssembly.Name))
                    {
                        continue;
                    }

                    var assemblyFullPath = Path.Combine(Path.GetDirectoryName(assemblyPath) ?? "",
                        referencedAssembly.Name + ".dll");
                    if (File.Exists(assemblyFullPath))
                    {
                        queue.Enqueue(ReadAssembly(assemblyFullPath));
                    }
                    else
                    {
                        //There is not the assembly in the output folder.
                        //Maybe it is netstandard.dll or any another core library.
                        //Let's try to load via default resolver.
                        try
                        {
                            var ns = _defaultResolver.Resolve(referencedAssembly);
                            if (ns != null)
                            {
                                queue.Enqueue(ns);
                            }
                        } catch (AssemblyResolutionException) { }
                    }
                }
            }

            return loadedAssemblyNames;
        }
    }
}
