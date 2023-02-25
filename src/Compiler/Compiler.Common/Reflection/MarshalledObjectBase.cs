
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OpenSilver.Compiler.Common
{
    public abstract class MarshalledObjectBase : MarshalByRefObject, IMarshalledObjectBase
    {
        private delegate void ReferencedAssemblyLoadedDelegate(Assembly assembly, string originalAssemblyFolder);

        private readonly Dictionary<string, Assembly> _loadedAssemblySimpleNameToAssembly = new();
        private readonly Dictionary<string, Assembly> _loadedAssemblyPathToAssembly = new();
        private readonly Dictionary<string, Type> _typeNameToType = new();
        private readonly Dictionary<string, Dictionary<string, HashSet<string>>> _assemblyNameToXmlNamespaceToClrNamespaces = new();
        private readonly HashSet<string> _attemptedAssemblyLoads = new();
        private readonly Dictionary<Assembly, bool> _onlyReflectionLoaded = new();

        protected MarshalledObjectBase()
        {
            // Subscribe to the "AssemblyResolve" event so that. We do this for multiple reasons,
            // one of them is so that when reading "assembly.GetCustomAttributes(typeof(XmlnsDefinitionAttribute))"
            // we don't get an error that says that the CSHTML5 "Core" assembly could not be found (this
            // happens when the DLL is not located in the CSHTML5 folder, such as for extensions).
            AppDomain.CurrentDomain.AssemblyResolve -= OnAssemblyResolve; // Unsubscribe first, in case of re-entrance in this method.
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;

            // Subscribe to the "ReflectionOnlyAssemblyResolve" event so that. We do this for multiple reasons,
            // one of them is so that when reading "assembly.GetCustomAttributes(typeof(XmlnsDefinitionAttribute))"
            // we don't get an error that says that the CSHTML5 "Core" assembly could not be found (this happens
            // when the DLL is not located in the CSHTML5 folder, such as for extensions).
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= OnReflectionOnlyAssemblyResolve;
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += OnReflectionOnlyAssemblyResolve;
        }

        public virtual void Initialize(bool isSLMigration) { }

        public virtual Type FindType(
            string namespaceName,
            string typeName,
            string assemblyName = null,
            bool doNotRaiseExceptionIfNotFound = false)
        {
            // Generate string representing the type:
            string fullTypeNameWithNamespaceInsideBraces = !string.IsNullOrEmpty(namespaceName) ? "{" + namespaceName + "}" + typeName : typeName;

            // Start by looking in the cache dictionary:
            if (_typeNameToType.TryGetValue(fullTypeNameWithNamespaceInsideBraces, out var type))
            {
                return type;
            }

            // Look for the type in all loaded assemblies:
            foreach (var assemblyKeyValuePair in _loadedAssemblySimpleNameToAssembly)
            {
                string assemblySimpleName = assemblyKeyValuePair.Key;
                Assembly assembly = assemblyKeyValuePair.Value;
                if (assemblyName == null || assemblySimpleName == assemblyName)
                {
                    var namespacesToLookInto = new List<string>();

                    // If the namespace is a XML namespace (eg. "{http://schemas.microsoft.com/winfx/2006/xaml/presentation}"), we should iterate through all the corresponding CLR namespaces:
                    if (IsNamespaceAnXmlNamespace(namespaceName))
                    {
                        namespacesToLookInto.AddRange(GetClrNamespacesFromXmlNamespace(assemblySimpleName, namespaceName));
                    }
                    else
                    {
                        namespacesToLookInto.Add(namespaceName);
                    }

                    // Search for the type:
                    foreach (var namespaceToLookInto in namespacesToLookInto)
                    {
                        string fullTypeNameToFind = namespaceToLookInto + "." + typeName;
                        var typeIfFound = assembly.GetType(fullTypeNameToFind);
                        if (typeIfFound == null)
                        {
                            //try to find a matching nested type.
                            fullTypeNameToFind = namespaceToLookInto + "+" + typeName;
                            typeIfFound = assembly.GetType(fullTypeNameToFind);
                        }

                        if (typeIfFound != null)
                        {
                            _typeNameToType.Add(fullTypeNameWithNamespaceInsideBraces, typeIfFound);
                            return typeIfFound;
                        }
                    }
                }
            }

            if (!doNotRaiseExceptionIfNotFound)
                throw new XamlParseException(
                    "Type not found: \"" + typeName + "\""
                    + (!string.IsNullOrEmpty(namespaceName) ? " in namespace: \"" + namespaceName + "\"" : "")
                    + (!string.IsNullOrEmpty(assemblyName) ? " in assembly: \"" + assemblyName + "\"" : "")
                    + ".");

            return type;
        }

        public bool TryGetAssembly(string assemblyName, out Assembly assembly)
            => _loadedAssemblySimpleNameToAssembly.TryGetValue(assemblyName, out assembly);

        public string LoadAssembly(string assemblyPath, bool loadReferencedAssembliesToo, bool skipReadingAttributesFromAssemblies)
        {
            bool alreadyLoaded = _loadedAssemblyPathToAssembly.ContainsKey(assemblyPath);

            // Load the specified assembly and process it if not already done:
            Assembly assembly = LoadAndProcessAssemblyFromPath(assemblyPath);

            if (loadReferencedAssembliesToo)
            {
                LoadAndProcessReferencedAssemblies(
                    assembly,
                    Path.GetDirectoryName(assemblyPath));
            }

            // Also load the referenced assemblies too if instructed to do so:
            if (!skipReadingAttributesFromAssemblies && !alreadyLoaded)
            {
                ReadXmlnsDefinitionAttributes(assembly);
                if (loadReferencedAssembliesToo)
                {
                    ReadXmlnsAttributesFromReferencedAssemblies(assembly);
                }
            }

            string assemblySimpleName = assembly.GetName().Name;

            return assemblySimpleName;
        }

        public void LoadAssemblyAndAllReferencedAssembliesRecursively(
            string assemblyPath,
            bool skipReadingAttributesFromAssemblies,
            out List<string> assemblySimpleNames)
        {
            HashSet<string> simpleNameOfAssembliesProcessedDuringRecursion = new HashSet<string>();

            // Load the specified assembly in memory and process it if not already done:
            Assembly assembly = LoadAndProcessAssemblyFromPath(assemblyPath);
            simpleNameOfAssembliesProcessedDuringRecursion.Add(assembly.GetName().Name);

            // Start the recursion:
            ReferencedAssemblyLoadedDelegate whatElseToDoWithTheReferencedAssembly = null;
            whatElseToDoWithTheReferencedAssembly = (referencedAssembly, referencedAssemblyFolder) =>
            {
                string referencedAssemblySimpleName = referencedAssembly.GetName().Name;

                if (!simpleNameOfAssembliesProcessedDuringRecursion.Contains(referencedAssemblySimpleName)) // This prevents processing multiple times the same assembly in case that it is referenced by multiple assemblies.
                {
                    // Remember that we processed this assembly:
                    simpleNameOfAssembliesProcessedDuringRecursion.Add(referencedAssemblySimpleName);

                    // Recursion:
                    LoadAndProcessReferencedAssemblies(
                        referencedAssembly,
                        referencedAssemblyFolder,
                        whatElseToDoWithTheReferencedAssembly);
                    if (!skipReadingAttributesFromAssemblies)
                    {
                        ReadXmlnsAttributesFromReferencedAssemblies(referencedAssembly);
                    }
                }
            };

            LoadAndProcessReferencedAssemblies(
                assembly,
                Path.GetDirectoryName(assemblyPath),
                whatElseToDoWithTheReferencedAssembly);
            if (!skipReadingAttributesFromAssemblies)
            {
                ReadXmlnsAttributesFromReferencedAssemblies(assembly);
            }

            assemblySimpleNames = new List<string>(simpleNameOfAssembliesProcessedDuringRecursion);
        }

        public void LoadAssemblyMscorlib(bool isCoreAssembly)
        {
            string assemblyPath = "mscorlib"; //Note: this is a special case, it's not really a path, it's just used for the cache dictionary.
            if (!_loadedAssemblyPathToAssembly.ContainsKey(assemblyPath))
            {
                Assembly assembly = typeof(string).Assembly;

                _loadedAssemblyPathToAssembly[assemblyPath] = assembly;
                _loadedAssemblySimpleNameToAssembly[assembly.GetName().Name] = assembly;
            }
        }

        private Assembly LoadAndProcessAssemblyFromPath(string assemblyPath)
        {
            Assembly assembly;
            if (!_loadedAssemblyPathToAssembly.ContainsKey(assemblyPath))
            {
                // We try to load assemlby
                try
                {
                    assembly = Assembly.LoadFrom(assemblyPath);
                    if (!_onlyReflectionLoaded.ContainsKey(assembly))
                    {
                        _onlyReflectionLoaded.Add(assembly, false);
                    }
                }
                catch
                {
                    // it may fails because somes .dll of .NET Standard are more like interfaces
                    // so we load them only for reflection 

                    assembly = Assembly.ReflectionOnlyLoadFrom(assemblyPath);
                    if (!_onlyReflectionLoaded.ContainsKey(assembly))
                    {
                        _onlyReflectionLoaded.Add(assembly, true);
                    }

                }

                _loadedAssemblyPathToAssembly[assemblyPath] = assembly;
            }
            else
            {
                assembly = _loadedAssemblyPathToAssembly[assemblyPath];
            }

            // Note: this line is here in order to be done before "ProcessLoadedAssembly"
            // (though such order may not be necessarily required)
            _loadedAssemblySimpleNameToAssembly[assembly.GetName().Name] = assembly;

            return assembly;
        }

        private void ReadXmlnsDefinitionAttributes(Assembly assembly)
        {
            string assemblySimpleName = assembly.GetName().Name;

            // Extract the "XmlnsDefinition" attributes defined in the "AssemblyInfo.cs" files, for use with XAML namespace mappings:
            Dictionary<string, HashSet<string>> xmlNamespaceToClrNamespaces = null;
            if (_assemblyNameToXmlNamespaceToClrNamespaces.ContainsKey(assemblySimpleName))
                xmlNamespaceToClrNamespaces = _assemblyNameToXmlNamespaceToClrNamespaces[assemblySimpleName];

            Type xmlnsDefinitionAttributeType = this.FindType("System.Windows.Markup", "XmlnsDefinitionAttribute");

            IList<CustomAttributeData> attributesData = new List<CustomAttributeData>();
            IEnumerable<Attribute> attributes = new List<Attribute>();

            // Instead of the commented code above, we now try both "GetCustomAttributes" and "GetCustomAttributesData"
            // to fix the compilation issue experienced with Client_REP (with the delivery dated Dec 22, 2020)
            try
            {
                attributes = assembly.GetCustomAttributes(xmlnsDefinitionAttributeType);
            }
            catch
            {
                try
                {
                    attributesData = assembly.GetCustomAttributesData();
                }
                catch
                {
                    // Fails silently
                }
            }

            foreach (var attribute in attributes)
            {
                string xmlNamespace = (xmlnsDefinitionAttributeType.GetProperty("XmlNamespace").GetValue(attribute) ?? "").ToString();
                string clrNamespace = (xmlnsDefinitionAttributeType.GetProperty("ClrNamespace").GetValue(attribute) ?? "").ToString();
                if (!string.IsNullOrEmpty(xmlNamespace) && !string.IsNullOrEmpty(clrNamespace))
                {
                    if (xmlNamespaceToClrNamespaces == null)
                    {
                        xmlNamespaceToClrNamespaces = new Dictionary<string, HashSet<string>>();
                        _assemblyNameToXmlNamespaceToClrNamespaces.Add(assemblySimpleName, xmlNamespaceToClrNamespaces);
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
                    {
                        clrNamespacesAssociatedToThisXmlNamespace.Add(clrNamespace);
                    }
                }
            }

            // we have to go through attributesData from only reflection loaded assemblies
            foreach (var attributeData in attributesData)
            {
                if (attributeData.AttributeType == xmlnsDefinitionAttributeType) //note: should we use IsAssignableFrom instead? (I'd say no because I wouldn't see the point of inheriting from thit type.)
                {
                    string xmlNamespace = attributeData.ConstructorArguments[0].ToString();
                    string clrNamespace = attributeData.ConstructorArguments[1].ToString();
                    if (!string.IsNullOrEmpty(xmlNamespace) && !string.IsNullOrEmpty(clrNamespace))
                    {
                        if (xmlNamespaceToClrNamespaces == null)
                        {
                            xmlNamespaceToClrNamespaces = new Dictionary<string, HashSet<string>>();
                            _assemblyNameToXmlNamespaceToClrNamespaces.Add(assemblySimpleName, xmlNamespaceToClrNamespaces);
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
                        {
                            clrNamespacesAssociatedToThisXmlNamespace.Add(clrNamespace);
                        }
                    }
                }
            }
        }

        private void ReadXmlnsAttributesFromReferencedAssemblies(Assembly assembly)
        {
            foreach (var refAssemblyName in assembly.GetReferencedAssemblies())
            {
                if (_loadedAssemblySimpleNameToAssembly.TryGetValue(refAssemblyName.Name, out Assembly refAssembly))
                {
                    ReadXmlnsDefinitionAttributes(refAssembly);
                }
            }
        }

        private void LoadAndProcessReferencedAssemblies(
                Assembly assembly,
                string originalAssemblyFolder,
                ReferencedAssemblyLoadedDelegate whatElseToDoWithTheReferencedAssembly = null)
        {
            //-------------------------------------------
            // Iterate through all the referenced assemblies:
            //-------------------------------------------
            AssemblyName[] referencedAssembliesNames = assembly.GetReferencedAssemblies();

            foreach (AssemblyName referencedAssemblyName in referencedAssembliesNames)
            {
                string candidateAssemblyPath = Path.Combine(originalAssemblyFolder + "\\", referencedAssemblyName.Name + ".dll");

                Assembly referencedAssembly;

                //-------------------------------------------
                // Load the referenced assembly in memory if not already loaded:
                //-------------------------------------------
                if (_loadedAssemblySimpleNameToAssembly.ContainsKey(referencedAssemblyName.Name))
                {
                    referencedAssembly = _loadedAssemblySimpleNameToAssembly[referencedAssemblyName.Name];
                }
                else if (_loadedAssemblyPathToAssembly.ContainsKey(candidateAssemblyPath))
                {
                    referencedAssembly = _loadedAssemblyPathToAssembly[candidateAssemblyPath];
                }
                else
                {
                    // First, attempt to load the referenced assembly from the same folder as the assembly if found there,
                    // otherwise, load based on the AssemblyName:
                    if (File.Exists(candidateAssemblyPath))
                    {
                        // We try to load assemlby
                        try
                        {
                            referencedAssembly = Assembly.LoadFrom(candidateAssemblyPath);
                            if (!_onlyReflectionLoaded.ContainsKey(referencedAssembly))
                            {
                                _onlyReflectionLoaded.Add(referencedAssembly, false);
                            }
                        }
                        catch
                        {
                            // it may fails because somes .dll of .NET Standard are more like interfaces
                            // so we load them only for reflection 

                            referencedAssembly = Assembly.ReflectionOnlyLoadFrom(candidateAssemblyPath);
                            if (!_onlyReflectionLoaded.ContainsKey(referencedAssembly))
                            {
                                _onlyReflectionLoaded.Add(referencedAssembly, true);
                            }
                        }
                        _loadedAssemblyPathToAssembly[candidateAssemblyPath] = referencedAssembly;
                    }
                    else
                    {
                        // We try to load assemlby
                        try
                        {
                            referencedAssembly = Assembly.Load(referencedAssemblyName);
                            if (!_onlyReflectionLoaded.ContainsKey(referencedAssembly))
                            {
                                _onlyReflectionLoaded.Add(referencedAssembly, false);
                            }
                        }
                        // it may fails because somes .dll of .NET Standard are more like interfaces
                        // so we load them only for reflection 
                        catch (Exception)
                        {
                            try
                            {
                                referencedAssembly = Assembly.ReflectionOnlyLoad(referencedAssemblyName.Name);
                                if (!_onlyReflectionLoaded.ContainsKey(referencedAssembly))
                                {
                                    _onlyReflectionLoaded.Add(referencedAssembly, true);
                                }
                            }
                            catch
                            {
                                // todo: see why we sometimes enter here. We should not because apparently VS is able to find some
                                // assemblies that we are unable to find.
                                // todo: add an MSBuild warning to the output of the compilation using the logger
                                Debug.WriteLine("Unable to find assembly '" + referencedAssemblyName.Name + "'.");
                                referencedAssembly = null;
                            }
                        }
                    }
                }

                if (referencedAssembly != null)
                {
                    // Remember the assembly simple name:
                    string assemblySimpleName = referencedAssemblyName.Name;
                    _loadedAssemblySimpleNameToAssembly[assemblySimpleName] = referencedAssembly;

                    //-------------------------------------------
                    // Do something else with the referenced assembly (useful for recursion):
                    //-------------------------------------------
                    whatElseToDoWithTheReferencedAssembly?.Invoke(referencedAssembly, originalAssemblyFolder);
                }
            }
        }

        private Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            string assemblyName = args.Name;

            if (!_attemptedAssemblyLoads.Contains(assemblyName)) // Check if we already tried loading this.
            {
                _attemptedAssemblyLoads.Add(assemblyName);
                return Assembly.Load(assemblyName);
            }

            return null; // We will then trigger a "ReflectionOnlyLoad"
        }

        private Assembly OnReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
            => Assembly.ReflectionOnlyLoad(args.Name);

        private IEnumerable<string> GetClrNamespacesFromXmlNamespace(string assemblySimpleName, string xmlNamespace)
        {
            // Note: This method returns an empty enumeration if no result was found.
            if (_assemblyNameToXmlNamespaceToClrNamespaces.ContainsKey(assemblySimpleName))
            {
                var xmlNamespaceToClrNamespaces = _assemblyNameToXmlNamespaceToClrNamespaces[assemblySimpleName];
                if (xmlNamespaceToClrNamespaces.ContainsKey(xmlNamespace))
                {
                    return xmlNamespaceToClrNamespaces[xmlNamespace];
                }
            }
            return Enumerable.Empty<string>();
        }

        private bool IsNamespaceAnXmlNamespace(string namespaceName) => namespaceName.StartsWith("http://");
    }
}
