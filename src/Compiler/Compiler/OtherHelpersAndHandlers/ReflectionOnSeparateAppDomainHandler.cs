

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



extern alias wpf;
#if !BRIDGE && !CSHTML5BLAZOR
extern alias custom;
extern alias DotNetForHtml5Core;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
#if !BRIDGE && !CSHTML5BLAZOR
using custom::System.Windows.Markup;
#endif
using System.Xml.Linq;
using DotNetForHtml5.Compiler.Common;
using DotNetForHtml5.Compiler.OtherHelpersAndHandlers;

namespace DotNetForHtml5.Compiler
{
    internal class ReflectionOnSeparateAppDomainHandler : IDisposable
    {
        //Note: we use a new AppDomain so that we can Unload all the assemblies that we have inspected when we have done.

        //----------------------------------------------------------------------
        // We create a static instance in the "BeforeXamlPreprocessor" task.
        // The static instance avoids reloading the assemblies for each XAML file.
        // We dispose it in the "AfterXamlPreprocessor" task.
        //----------------------------------------------------------------------

        public static ReflectionOnSeparateAppDomainHandler Current;

        AppDomain _newAppDomain;
        IMarshalledObject _marshalledObject;

        public ReflectionOnSeparateAppDomainHandler(string typeForwardingAssemblyPath = null)
        {
            // Create a new AppDomain:
            AppDomainSetup setupInformation = AppDomain.CurrentDomain.SetupInformation;
            _newAppDomain = AppDomain.CreateDomain("newAppDomain", AppDomain.CurrentDomain.Evidence, setupInformation);

            // Listen to the "AssemblyResolve" of the current domain so that when we arrive to the "Unwrap" call below, we can locate the "CSharpXamlForHtml5.Compiler.Common.dll" file. // For information: http://forums.codeguru.com/showthread.php?398030-AppDomain-CreateInstanceAndUnwrap(-)-vs-AppDomain-CreateInstanceFrom
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

                // Create an instance of the MarshalledObject class in the new domain:
                string pathOfThisVeryAssembly = PathsHelper.GetPathOfThisVeryAssembly();
                System.Runtime.Remoting.ObjectHandle obj = _newAppDomain.CreateInstanceFrom(pathOfThisVeryAssembly, typeof(MarshalledObject).FullName);
                _marshalledObject = (IMarshalledObject)obj.Unwrap(); // As the object we are creating is from another appdomain hence we will get that object in wrapped format and hence in next step we have unwrappped it
                _marshalledObject.SetTypeForwardingAssemblyPath(typeForwardingAssemblyPath);
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
            }
        }

        Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return Assembly.Load(args.Name); // This is required so that when the "Unwrap" call above is done, we can locate the "CSharpXamlForHtml5.Compiler.Common.dll" file. // For information: http://forums.codeguru.com/showthread.php?398030-AppDomain-CreateInstanceAndUnwrap(-)-vs-AppDomain-CreateInstanceFrom
        }

        public void Dispose()
        {
            // Unload everything:
            AppDomain.Unload(_newAppDomain);
            GC.Collect(); // Collects all unused memory
            GC.WaitForPendingFinalizers(); // Waits until GC has finished its work
            GC.Collect();
        }

        public string LoadAssembly(string assemblyPath, bool loadReferencedAssembliesToo, bool isBridgeBasedVersion, bool isCoreAssembly, string nameOfAssembliesThatDoNotContainUserCode, bool skipReadingAttributesFromAssemblies)
        {
            return _marshalledObject.LoadAssembly(assemblyPath, loadReferencedAssembliesToo, isBridgeBasedVersion, isCoreAssembly, nameOfAssembliesThatDoNotContainUserCode, skipReadingAttributesFromAssemblies: skipReadingAttributesFromAssemblies);
        }

        public void LoadAssemblyAndAllReferencedAssembliesRecursively(string assemblyPath, bool isBridgeBasedVersion, bool isCoreAssembly, string nameOfAssembliesThatDoNotContainUserCode, bool skipReadingAttributesFromAssemblies, out List<string> assemblySimpleNames)
        {
            _marshalledObject.LoadAssemblyAndAllReferencedAssembliesRecursively(assemblyPath, isBridgeBasedVersion: isBridgeBasedVersion, isCoreAssembly: isCoreAssembly, nameOfAssembliesThatDoNotContainUserCode: nameOfAssembliesThatDoNotContainUserCode, skipReadingAttributesFromAssemblies: skipReadingAttributesFromAssemblies, assemblySimpleNames: out assemblySimpleNames);
        }

        public void LoadAssemblyMscorlib(bool isBridgeBasedVersion, bool isCoreAssembly, string nameOfAssembliesThatDoNotContainUserCode)
        {
            _marshalledObject.LoadAssemblyMscorlib(isBridgeBasedVersion, isCoreAssembly, nameOfAssembliesThatDoNotContainUserCode);
        }

        public string GetContentPropertyName(string namespaceName, string localTypeName, string assemblyNameIfAny = null)
        {
            return _marshalledObject.GetContentPropertyName(namespaceName, localTypeName, assemblyNameIfAny);
        }

        public bool DoesTypeContainAttributeToConvertDirectContent(string namespaceName, string localTypeName, string assemblyNameIfAny = null)
        {
            return _marshalledObject.DoesTypeContainAttributeToConvertDirectContent(namespaceName, localTypeName, assemblyNameIfAny);
        }

        public bool IsPropertyAttached(string propertyName, string declaringTypeNamespaceName, string declaringTypeLocalName, string parentNamespaceName, string parentLocalTypeName, string declaringTypeAssemblyIfAny = null)
        {
            return _marshalledObject.IsPropertyAttached(propertyName, declaringTypeNamespaceName, declaringTypeLocalName, parentNamespaceName, parentLocalTypeName, declaringTypeAssemblyIfAny);
        }

        public bool IsPropertyOrFieldACollection(string propertyName, string parentNamespaceName, string parentLocalTypeName, string parentAssemblyNameIfAny = null)
        {
            return _marshalledObject.IsPropertyOrFieldACollection(propertyName, parentNamespaceName, parentLocalTypeName, parentAssemblyNameIfAny);
        }

        public bool IsPropertyOrFieldADictionary(string propertyName, string parentNamespaceName, string parentLocalTypeName, string parentAssemblyNameIfAny = null)
        {
            return _marshalledObject.IsPropertyOrFieldADictionary(propertyName, parentNamespaceName, parentLocalTypeName, parentAssemblyNameIfAny);
        }

        public bool DoesMethodReturnACollection(string methodName, string typeNamespaceName, string localTypeName, string typeAssemblyNameIfAny = null)
        {
            return _marshalledObject.DoesMethodReturnACollection(methodName, typeNamespaceName, localTypeName, typeAssemblyNameIfAny);
        }

        public bool DoesMethodReturnADictionary(string methodName, string typeNamespaceName, string localTypeName, string typeAssemblyNameIfAny = null)
        {
            return _marshalledObject.DoesMethodReturnADictionary(methodName, typeNamespaceName, localTypeName, typeAssemblyNameIfAny);
        }

        public bool IsElementACollection(string parentNamespaceName, string parentLocalTypeName, string parentAssemblyNameIfAny = null)
        {
            return _marshalledObject.IsElementACollection(parentNamespaceName, parentLocalTypeName, parentAssemblyNameIfAny);
        }

        public bool IsElementADictionary(string parentNamespaceName, string parentLocalTypeName, string parentAssemblyNameIfAny = null)
        {
            return _marshalledObject.IsElementADictionary(parentNamespaceName, parentLocalTypeName, parentAssemblyNameIfAny);
        }

        public bool IsElementAMarkupExtension(string parentNamespaceName, string parentLocalTypeName, string parentAssemblyNameIfAny = null)
        {
            return _marshalledObject.IsElementAMarkupExtension(parentNamespaceName, parentLocalTypeName, parentAssemblyNameIfAny);
        }

        //public bool IsElementAnUIElement(string parentNamespaceName, string parentLocalTypeName, string parentAssemblyNameIfAny = null)
        //{
        //    return _marshalledObject.IsElementAnUIElement(parentNamespaceName, parentLocalTypeName, parentAssemblyNameIfAny);
        //}

        public bool IsTypeAssignableFrom(string nameSpaceOfTypeToAssignFrom, string nameOfTypeToAssignFrom, string assemblyNameOfTypeToAssignFrom, string nameSpaceOfTypeToAssignTo, string nameOfTypeToAssignTo, string assemblyNameOfTypeToAssignTo, bool isAttached = false)
        {
            return _marshalledObject.IsTypeAssignableFrom(nameSpaceOfTypeToAssignFrom, nameOfTypeToAssignFrom, assemblyNameOfTypeToAssignFrom, nameSpaceOfTypeToAssignTo, nameOfTypeToAssignTo, assemblyNameOfTypeToAssignTo, isAttached);
        }

        public string GetKeyNameOfProperty(string elementNameSpace, string elementLocalName, string assemblyNameIfAny, string propertyName)
        {
            return _marshalledObject.GetKeyNameOfProperty(elementNameSpace, elementLocalName, assemblyNameIfAny, propertyName);
        }

        public bool DoesTypeContainNameMemberOfTypeString(string namespaceName, string localTypeName, string assemblyNameIfAny = null)
        {
            return _marshalledObject.DoesTypeContainNameMemberOfTypeString(namespaceName, localTypeName, assemblyNameIfAny);
        }

        public XName GetCSharpEquivalentOfXamlTypeAsXName(string namespaceName, string localTypeName, string assemblyNameIfAny = null, bool ifTypeNotFoundTryGuessing = false)
        {
            return _marshalledObject.GetCSharpEquivalentOfXamlTypeAsXName(namespaceName, localTypeName, assemblyNameIfAny, ifTypeNotFoundTryGuessing);
        }

        public Type GetCSharpEquivalentOfXamlType(string namespaceName, string localTypeName, string assemblyIfAny = null, bool ifTypeNotFoundTryGuessing = false)
        {
            return _marshalledObject.GetCSharpEquivalentOfXamlType(namespaceName, localTypeName, assemblyIfAny, ifTypeNotFoundTryGuessing);
        }

        public string GetCSharpEquivalentOfXamlTypeAsString(string namespaceName, string localTypeName, string assemblyNameIfAny = null, bool ifTypeNotFoundTryGuessing = false)
        {
            return _marshalledObject.GetCSharpEquivalentOfXamlTypeAsString(namespaceName, localTypeName, assemblyNameIfAny, ifTypeNotFoundTryGuessing);
        }

        public MemberTypes GetMemberType(string memberName, string namespaceName, string localTypeName, string assemblyNameIfAny = null)
        {
            return _marshalledObject.GetMemberType(memberName, namespaceName, localTypeName, assemblyNameIfAny);
        }

        public string FindCommaSeparatedTypesThatAreSerializable(string assemblySimpleName)
        {
            return _marshalledObject.FindCommaSeparatedTypesThatAreSerializable(assemblySimpleName);
        }

        public bool IsTypeAnEnum(string namespaceName, string localTypeName, string assemblyNameIfAny = null)
        {
            return _marshalledObject.IsTypeAnEnum(namespaceName, localTypeName, assemblyNameIfAny);
        }

        public void GetMethodReturnValueTypeInfo(string methodName, string namespaceName, string localTypeName, out string returnValueNamespaceName, out string returnValueLocalTypeName, out bool isTypeString, out bool isTypeEnum, string assemblyNameIfAny = null)
        {
            _marshalledObject.GetMethodReturnValueTypeInfo(methodName, namespaceName, localTypeName, out returnValueNamespaceName, out returnValueLocalTypeName, out isTypeString, out isTypeEnum, assemblyNameIfAny);
        }

        public void GetMethodInfo(string methodName, string namespaceName, string localTypeName, out string declaringTypeName, out string returnValueNamespaceName, out string returnValueLocalTypeName, out bool isTypeString, out bool isTypeEnum, string assemblyNameIfAny = null)
        {
            _marshalledObject.GetMethodInfo(methodName, namespaceName, localTypeName, out declaringTypeName, out returnValueNamespaceName, out returnValueLocalTypeName, out isTypeString, out isTypeEnum, assemblyNameIfAny);
        }

        public void GetPropertyOrFieldTypeInfo(string propertyOrFieldName, string namespaceName, string localTypeName, out string propertyNamespaceName, out string propertyLocalTypeName, out bool isTypeString, out bool isTypeEnum, string assemblyNameIfAny = null, bool isAttached = false)
        {
            _marshalledObject.GetPropertyOrFieldTypeInfo(propertyOrFieldName, namespaceName, localTypeName, out propertyNamespaceName, out propertyLocalTypeName, out isTypeString, out isTypeEnum, assemblyNameIfAny, isAttached: isAttached);
        }

        public void GetPropertyOrFieldInfo(string propertyOrFieldName, string namespaceName, string localTypeName, out string memberDeclaringTypeName, out string memberTypeNamespace, out string memberTypeName, out bool isTypeString, out bool isTypeEnum, string assemblyNameIfAny = null, bool isAttached = false)
        {
            _marshalledObject.GetPropertyOrFieldInfo(propertyOrFieldName, namespaceName, localTypeName, out memberDeclaringTypeName, out memberTypeNamespace, out memberTypeName, out isTypeString, out isTypeEnum, assemblyNameIfAny, isAttached);
        }

        public string GetFieldName(string fieldNameIgnoreCase, string namespaceName, string localTypeName, string assemblyIfAny = null)
        {
            return _marshalledObject.GetFieldName(fieldNameIgnoreCase, namespaceName, localTypeName, assemblyIfAny);
        }

        public string GetFieldDeclaringTypeName(string fieldName, string namespaceName, string localTypeName, out string assemblyNameOfDeclaringType, string assemblyNameIfAny = null)
        {
            return _marshalledObject.GetFieldDeclaringTypeName(fieldName, namespaceName, localTypeName, out assemblyNameOfDeclaringType, assemblyNameIfAny);
        }

        public string GetPropertyDeclaringTypeName(string propertyName, string namespaceName, string localTypeName, out string assemblyNameOfDeclaringType, string assemblyNameIfAny = null)
        {
            return _marshalledObject.GetPropertyDeclaringTypeName(propertyName, namespaceName, localTypeName, out assemblyNameOfDeclaringType, assemblyNameIfAny);
        }

        public string GetCSharpXamlForHtml5CompilerVersionNumberOrNull(string assemblySimpleName)
        {
            return _marshalledObject.GetCSharpXamlForHtml5CompilerVersionNumberOrNull(assemblySimpleName);
        }

        public string GetCSharpXamlForHtml5CompilerVersionFriendlyNameOrNull(string assemblySimpleName)
        {
            return _marshalledObject.GetCSharpXamlForHtml5CompilerVersionFriendlyNameOrNull(assemblySimpleName);
        }

        public string GetCSharpXamlForHtml5MinimumRequiredCompilerVersionNumberOrNull(string assemblySimpleName)
        {
            return _marshalledObject.GetCSharpXamlForHtml5MinimumRequiredCompilerVersionNumberOrNull(assemblySimpleName);
        }

        public string GetCSharpXamlForHtml5MinimumRequiredCompilerVersionFriendlyNameOrNull(string assemblySimpleName)
        {
            return _marshalledObject.GetCSharpXamlForHtml5MinimumRequiredCompilerVersionFriendlyNameOrNull(assemblySimpleName);
        }

        public Dictionary<string, byte[]> GetManifestResources(string assemblySimpleName, Func<string, bool> filenamePredicate)
        {
            return _marshalledObject.GetManifestResources(assemblySimpleName, filenamePredicate);
        }

        public Dictionary<string, byte[]> GetManifestResources(string assemblySimpleName, HashSet<string> supportedExtensionsLowerCase)
        {
            return _marshalledObject.GetManifestResources(assemblySimpleName, supportedExtensionsLowerCase);
        }

        public Dictionary<string, byte[]> GetResources(string assemblySimpleName, HashSet<string> supportedExtensionsLowercase)
        {
            return _marshalledObject.GetResources(assemblySimpleName, supportedExtensionsLowercase);
        }

        public Type GetTypeInCoreAssemblies(string typeFullName)
        {
            return _marshalledObject.GetTypeInCoreAssemblies(typeFullName);
        }

        public bool TryGenerateCodeForInstantiatingAttributeValue(string xamlValue, out string generatedCSharpCode, string valueNamespaceName, string valueLocalTypeName, string valueAssemblyNameIfAny)
        {
            return _marshalledObject.TryGenerateCodeForInstantiatingAttributeValue(xamlValue, out generatedCSharpCode, valueNamespaceName, valueLocalTypeName, valueAssemblyNameIfAny);
        }

        public bool IsAssignableFrom(string namespaceName, string typeName, string fromNamespaceName, string fromTypeName)
        {
            return _marshalledObject.IsAssignableFrom(namespaceName, typeName, fromNamespaceName, fromTypeName);
        }

        public class MarshalledObject : MarshalByRefObject, IMarshalledObject
        {
            const string ASSEMBLY_NOT_IN_LIST_OF_LOADED_ASSEMBLIES = "The specified assembly is not in the list of loaded assemblies.";

            Dictionary<string, Assembly> _loadedAssemblySimpleNameToAssembly = new Dictionary<string, Assembly>();
            Dictionary<string, Assembly> _loadedAssemblyPathToAssembly = new Dictionary<string, Assembly>();
            Dictionary<string, Type> _typeNameToType = new Dictionary<string, Type>();
            Dictionary<string, Dictionary<string, HashSet<string>>> _assemblyNameToXmlNamespaceToClrNamespaces = new Dictionary<string, Dictionary<string, HashSet<string>>>(); // Used for XAML namespaces mappings.
            HashSet<Assembly> _coreAssemblies = new HashSet<Assembly>();
            Dictionary<string, Type> _cacheForResolvedTypesInCoreAssembly = new Dictionary<string, Type>();
            HashSet<string> _attemptedAssemblyLoads = new HashSet<string>();
#if BRIDGE
            string _typeForwardingAssemblyPath;
            Assembly _typeForwardingAssembly;
#endif

#if CSHTML5BLAZOR
            // this is the dictionnary of all Assemblies loaded for reflection only
            Dictionary<Assembly, bool> _onlyReflectionLoaded = new Dictionary<Assembly, bool>();
#endif


            delegate void ReferencedAssemblyLoadedDelegate(Assembly assembly, string originalAssemblyFolder);

            public MarshalledObject()
            {
                // Subscribe to the "AssemblyResolve" event so that. We do this for multiple reasons, one of them is so that when reading "assembly.GetCustomAttributes(typeof(XmlnsDefinitionAttribute))" we don't get an error that says that the CSHTML5 "Core" assembly could not be found (this happens when the DLL is not located in the CSHTML5 folder, such as for extensions).
                AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve; // Unsubscribe first, in case of re-entrance in this method.
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

#if CSHTML5BLAZOR
                // Subscribe to the "ReflectionOnlyAssemblyResolve" event so that. We do this for multiple reasons, one of them is so that when reading "assembly.GetCustomAttributes(typeof(XmlnsDefinitionAttribute))" we don't get an error that says that the CSHTML5 "Core" assembly could not be found (this happens when the DLL is not located in the CSHTML5 folder, such as for extensions).
                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= CurrentDomain_ReflectionOnlyAssemblyResolve;
                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += CurrentDomain_ReflectionOnlyAssemblyResolve;
#endif
            }

            /// <returns>The assembly simple name</returns>
            public string LoadAssembly(string assemblyPath, bool loadReferencedAssembliesToo, bool isBridgeBasedVersion, bool isCoreAssembly, string nameOfAssembliesThatDoNotContainUserCode, bool skipReadingAttributesFromAssemblies)
            {
                // Load the specified assembly and process it if not already done:
                Assembly assembly = LoadAndProcessAssemblyFromPath(assemblyPath, isBridgeBasedVersion: isBridgeBasedVersion, isCoreAssembly: isCoreAssembly, skipReadingAttributesFromAssemblies: skipReadingAttributesFromAssemblies);

                // Also load the referenced assemblies too if instructed to do so:
                if (loadReferencedAssembliesToo)
                {
                    LoadAndProcessReferencedAssemblies(assembly, Path.GetDirectoryName(assemblyPath), isBridgeBasedVersion, nameOfAssembliesThatDoNotContainUserCode, skipReadingAttributesFromAssemblies);
                }

                string assemblySimpleName = assembly.GetName().Name;

                return assemblySimpleName;
            }

            public void LoadAssemblyAndAllReferencedAssembliesRecursively(string assemblyPath, bool isBridgeBasedVersion, bool isCoreAssembly, string nameOfAssembliesThatDoNotContainUserCode, bool skipReadingAttributesFromAssemblies, out List<string> assemblySimpleNames)
            {
                HashSet<string> simpleNameOfAssembliesProcessedDuringRecursion = new HashSet<string>();

                // Load the specified assembly in memory and process it if not already done:
                Assembly assembly = LoadAndProcessAssemblyFromPath(assemblyPath, isBridgeBasedVersion: isBridgeBasedVersion, isCoreAssembly: isCoreAssembly, skipReadingAttributesFromAssemblies: skipReadingAttributesFromAssemblies);
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
                                LoadAndProcessReferencedAssemblies(referencedAssembly, referencedAssemblyFolder, isBridgeBasedVersion, nameOfAssembliesThatDoNotContainUserCode, skipReadingAttributesFromAssemblies, whatElseToDoWithTheReferencedAssembly);
                            }
                        };
                LoadAndProcessReferencedAssemblies(assembly, Path.GetDirectoryName(assemblyPath), isBridgeBasedVersion, nameOfAssembliesThatDoNotContainUserCode, skipReadingAttributesFromAssemblies, whatElseToDoWithTheReferencedAssembly);

                assemblySimpleNames = new List<string>(simpleNameOfAssembliesProcessedDuringRecursion);
            }

            public void LoadAssemblyMscorlib(bool isBridgeBasedVersion, bool isCoreAssembly, string nameOfAssembliesThatDoNotContainUserCode)
            {
                string assemblyPath = "mscorlib"; //Note: this is a special case, it's not really a path, it's just used for the cache dictionary.
                if (!_loadedAssemblyPathToAssembly.ContainsKey(assemblyPath))
                {
                    Assembly assembly = typeof(string).Assembly;

                    _loadedAssemblyPathToAssembly[assemblyPath] = assembly;
                    _loadedAssemblySimpleNameToAssembly[assembly.GetName().Name] = assembly;
                }
            }

            void LoadAndProcessReferencedAssemblies(Assembly assembly, string originalAssemblyFolder, bool isBridgeBasedVersion, string nameOfAssembliesThatDoNotContainUserCode, bool skipReadingAttributesFromAssemblies, ReferencedAssemblyLoadedDelegate whatElseToDoWithTheReferencedAssembly = null)
            {
                // Skip the assembly if it is not a user assembly:
                HashSet<string> assembliesToSkipLowercase;
                if (nameOfAssembliesThatDoNotContainUserCode != null)
                    assembliesToSkipLowercase = new HashSet<string>(nameOfAssembliesThatDoNotContainUserCode.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.ToLowerInvariant()));
                else
                    assembliesToSkipLowercase = new HashSet<string>();
#if BRIDGE
                if (assembliesToSkipLowercase.Count == 0)
                    throw new Exception("The 'NameOfAssembliesThatDoNotContainUserCode' parameter cannot be empty.");
#endif

                //-------------------------------------------
                // Iterate through all the referenced assemblies:
                //-------------------------------------------
                AssemblyName[] referencedAssembliesNames = assembly.GetReferencedAssemblies();

                foreach (AssemblyName referencedAssemblyName in referencedAssembliesNames)
                {
                    if (!assembliesToSkipLowercase.Contains(referencedAssemblyName.Name.ToLowerInvariant()))
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
#if BRIDGE
                            referencedAssembly = Assembly.LoadFile(candidateAssemblyPath); // Note: unlike with "Assembly.LoadFrom", with "Assembly.LoadFile" we get into the "AssemblyResolve" event.
                            _loadedAssemblyPathToAssembly[candidateAssemblyPath] = referencedAssembly;
#else
                            // First, attempt to load the referenced assembly from the same folder as the assembly if found there,
                            // otherwise, load based on the AssemblyName:
                            if (File.Exists(candidateAssemblyPath))
                            {
#if CSHTML5BLAZOR
                                // We try to load assemlby
                                try
                                {
                                    referencedAssembly = Assembly.LoadFrom(candidateAssemblyPath);
                                    if (!_onlyReflectionLoaded.ContainsKey(referencedAssembly))
                                        _onlyReflectionLoaded.Add(referencedAssembly, false);
                                }
                                // it may fails because somes .dll of .NET Standard are more like interfaces
                                // so we load them only for reflection 
                                catch (Exception)
                                {
                                    referencedAssembly = Assembly.ReflectionOnlyLoadFrom(candidateAssemblyPath);
                                    if (!_onlyReflectionLoaded.ContainsKey(referencedAssembly))
                                        _onlyReflectionLoaded.Add(referencedAssembly, true);
                                }
#else
                                referencedAssembly = Assembly.LoadFrom(candidateAssemblyPath);
#endif
                                _loadedAssemblyPathToAssembly[candidateAssemblyPath] = referencedAssembly;
                            }
                            else
                            {
#if CSHTML5BLAZOR
                                // We try to load assemlby
                                try
                                {
                                    referencedAssembly = Assembly.Load(referencedAssemblyName);
                                    if (!_onlyReflectionLoaded.ContainsKey(referencedAssembly))
                                        _onlyReflectionLoaded.Add(referencedAssembly, false);
                                }
                                // it may fails because somes .dll of .NET Standard are more like interfaces
                                // so we load them only for reflection 
                                catch (Exception)
                                {
                                    try
                                    {
                                        referencedAssembly = Assembly.ReflectionOnlyLoad(referencedAssemblyName.Name);
                                        if (!_onlyReflectionLoaded.ContainsKey(referencedAssembly))
                                            _onlyReflectionLoaded.Add(referencedAssembly, true);
                                    }
                                    catch (Exception)
                                    {
                                        //todo: see why we sometimes enter here. We should not because apparently VS is able to find some assemblies that we are unable to find. To reproduce: Client_GD when compiling "ServiceLogic_OS" is unable to locate "Telerik.Windows.Data.dll".
                                        //todo: add an MSBuild warning to the output of the compilation using the logger
                                        System.Diagnostics.Debug.WriteLine("Unable to find assembly '" + referencedAssemblyName.Name + "'.");
                                        referencedAssembly = null;
                                    }
                                }
#else
                                referencedAssembly = Assembly.Load(referencedAssemblyName);
#endif
                            }
#endif
                            if (referencedAssembly != null)
                            {
                                if (!skipReadingAttributesFromAssemblies)
                                {
                                    ReadXmlnsDefinitionAttributes(referencedAssembly, isBridgeBasedVersion);
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
                            if (whatElseToDoWithTheReferencedAssembly != null)
                            {
                                whatElseToDoWithTheReferencedAssembly(referencedAssembly, originalAssemblyFolder);
                            }
                        }
                    }
                }
            }

            Assembly LoadAndProcessAssemblyFromPath(string assemblyPath, bool isBridgeBasedVersion, bool isCoreAssembly, bool skipReadingAttributesFromAssemblies)
            {
                Assembly assembly;
                if (!_loadedAssemblyPathToAssembly.ContainsKey(assemblyPath))
                {
#if BRIDGE
                    try
                    {
                        assembly = Assembly.LoadFile(assemblyPath);  // Note: unlike with "Assembly.LoadFrom", with "Assembly.LoadFile" we get into the "AssemblyResolve" event.
                    }
                    catch (FileNotFoundException ex)
                    {
                        throw new FileNotFoundException(ex.Message, assemblyPath, ex);
                    }
#else

#if CSHTML5BLAZOR
                    // We try to load assemlby
                    try
                    {
                        assembly = Assembly.LoadFrom(assemblyPath);
                        if (!_onlyReflectionLoaded.ContainsKey(assembly))
                            _onlyReflectionLoaded.Add(assembly, false);
                    }
                    // it may fails because somes .dll of .NET Standard are more like interfaces
                    // so we load them only for reflection 
                    catch (Exception e)
                    {
                        assembly = Assembly.ReflectionOnlyLoadFrom(assemblyPath);
                        if (!_onlyReflectionLoaded.ContainsKey(assembly))
                            _onlyReflectionLoaded.Add(assembly, true);

                    }
#else
                    assembly = Assembly.LoadFrom(assemblyPath);
#endif

#endif

                    // Remember the assembly if it is a core assembly:
                    if (isCoreAssembly && !_coreAssemblies.Contains(assembly))
                    {
                        _coreAssemblies.Add(assembly);
                    }

                    _loadedAssemblyPathToAssembly[assemblyPath] = assembly;
                    _loadedAssemblySimpleNameToAssembly[assembly.GetName().Name] = assembly; // Note: this line is here in order to be done before "ProcessLoadedAssembly" (though such order may not be necessarily required)
                    if (!skipReadingAttributesFromAssemblies)
                    {
                        ReadXmlnsDefinitionAttributes(assembly, isBridgeBasedVersion);
                    }
                }
                else
                {
                    assembly = _loadedAssemblyPathToAssembly[assemblyPath];
                    _loadedAssemblySimpleNameToAssembly[assembly.GetName().Name] = assembly;
                }

                return assembly;
            }

            void ReadXmlnsDefinitionAttributes(Assembly assembly, bool isBridgeBasedVersion)
            {
                string assemblySimpleName = assembly.GetName().Name;

                // Extract the "XmlnsDefinition" attributes defined in the "AssemblyInfo.cs" files, for use with XAML namespace mappings:
                Dictionary<string, HashSet<string>> xmlNamespaceToClrNamespaces = null;
                if (_assemblyNameToXmlNamespaceToClrNamespaces.ContainsKey(assemblySimpleName))
                    xmlNamespaceToClrNamespaces = _assemblyNameToXmlNamespaceToClrNamespaces[assemblySimpleName];

#if BRIDGE || CSHTML5BLAZOR
                Type xmlnsDefinitionAttributeType = this.FindType("System.Windows.Markup", "XmlnsDefinitionAttribute");
#else
                Type xmlnsDefinitionAttributeType = typeof(XmlnsDefinitionAttribute);
#endif

#if CSHTML5BLAZOR
                IList<CustomAttributeData> attributesData = new List<CustomAttributeData>();
#endif
                IEnumerable<Attribute> attributes = new List<Attribute>();

#if CSHTML5BLAZOR
                /*
                // if assembly is loaded with reflection only we have to use GetCustomAttributesData instead of GetCustomAttributes
                if (_onlyReflectionLoaded.ContainsKey(assembly) && _onlyReflectionLoaded[assembly])
                    attributesData = assembly.GetCustomAttributesData();
                else
                    attributes = assembly.GetCustomAttributes(xmlnsDefinitionAttributeType);
                */

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
#else
                attributes = assembly.GetCustomAttributes(xmlnsDefinitionAttributeType);
#endif
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
                            clrNamespacesAssociatedToThisXmlNamespace = xmlNamespaceToClrNamespaces[xmlNamespace];
                        else
                        {
                            clrNamespacesAssociatedToThisXmlNamespace = new HashSet<string>();
                            xmlNamespaceToClrNamespaces.Add(xmlNamespace, clrNamespacesAssociatedToThisXmlNamespace);
                        }

                        if (!clrNamespacesAssociatedToThisXmlNamespace.Contains(clrNamespace))
                            clrNamespacesAssociatedToThisXmlNamespace.Add(clrNamespace);
                    }
                }

#if CSHTML5BLAZOR
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
                                clrNamespacesAssociatedToThisXmlNamespace = xmlNamespaceToClrNamespaces[xmlNamespace];
                            else
                            {
                                clrNamespacesAssociatedToThisXmlNamespace = new HashSet<string>();
                                xmlNamespaceToClrNamespaces.Add(xmlNamespace, clrNamespacesAssociatedToThisXmlNamespace);
                            }

                            if (!clrNamespacesAssociatedToThisXmlNamespace.Contains(clrNamespace))
                                clrNamespacesAssociatedToThisXmlNamespace.Add(clrNamespace);
                        }
                    }
                }
#endif
            }

            Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
            {
                string assemblyName = args.Name;
                string assemblyLocalName = assemblyName.IndexOf(',') >= 0 ? assemblyName.Substring(0, assemblyName.IndexOf(',')) : assemblyName;

#if BRIDGE
                if (assemblyLocalName.ToLower() == "bridge"
                || assemblyLocalName.ToLower() == "cshtml5.stubs") // Note: this corresponds to the assembly produced by the project "DotNetForHtml5.Bridge.TypesThatWillBeForwarded"
                {
                    if (_typeForwardingAssembly == null)
                    {
                        if (string.IsNullOrEmpty(_typeForwardingAssemblyPath))
                            throw new Exception("'TypeForwardingAssemblyPath' was not properly set.");

                        _typeForwardingAssembly = Assembly.LoadFile(_typeForwardingAssemblyPath);

                        if (_typeForwardingAssembly == null)
                            throw new Exception("Could not load TypeForwarding assembly.");
                    }

                    return _typeForwardingAssembly;

                }

                // Look in the same folder as the requesting assembly:
                var requestingAssembly = args.RequestingAssembly;
                string pathOfRequestingAssembly = requestingAssembly.CodeBase;
                string folder = Path.GetDirectoryName(pathOfRequestingAssembly);
                string candidateAssemblyPath = Path.Combine(folder, assemblyLocalName + ".dll");
                if (candidateAssemblyPath.ToLower().StartsWith(@"file:\"))
                    candidateAssemblyPath = candidateAssemblyPath.Substring(6);

                Assembly assembly;
                if (_loadedAssemblySimpleNameToAssembly.ContainsKey(assemblyLocalName))
                    assembly = _loadedAssemblySimpleNameToAssembly[assemblyLocalName];
                else if (_loadedAssemblyPathToAssembly.ContainsKey(candidateAssemblyPath))
                    assembly = _loadedAssemblyPathToAssembly[candidateAssemblyPath];
                else
                {
                    assembly = Assembly.LoadFile(candidateAssemblyPath);
                    _loadedAssemblyPathToAssembly[candidateAssemblyPath] = assembly;
                }
                return assembly;
#else
                if (!_attemptedAssemblyLoads.Contains(assemblyName)) // Check if we already tried loading this.
                {
                    _attemptedAssemblyLoads.Add(assemblyName);
                    return Assembly.Load(assemblyName); //Note: this line was added when referencing a user-made extension DLL located in its own folder, because we got an error when reading "assembly.GetCustomAttributes(typeof(XmlnsDefinitionAttribute))" saying that the CSHTML5 "Core" assembly could not be found. //todo: make sure that this is the right solution.
                }
                else
                {
                    return null; // We will then trigger a "ReflectionOnlyLoad"
                }
#endif
            }


#if CSHTML5BLAZOR
            private Assembly CurrentDomain_ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
            {
                return Assembly.ReflectionOnlyLoad(args.Name);
            }
#endif

            IEnumerable<string> GetClrNamespacesFromXmlNamespace(string assemblySimpleName, string xmlNamespace)
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

            Type FindType(string namespaceName, string localTypeName, string filterAssembliesAndRetainOnlyThoseThatHaveThisName = null, bool doNotRaiseExceptionIfNotFound = false)
            {
                Type type;

                // Fix the namespace:
                if (namespaceName.ToLower().StartsWith("using:"))
                {
                    namespaceName = namespaceName.Substring("using:".Length);
                }
                else if (namespaceName.ToLower().StartsWith("clr-namespace:"))
                {
                    string ns;
                    string assemblyNameIfAny;
                    GettingInformationAboutXamlTypes.ParseClrNamespaceDeclaration(namespaceName, out ns, out assemblyNameIfAny);
                    namespaceName = ns;
                    GettingInformationAboutXamlTypes.FixNamespaceForCompatibility(ref assemblyNameIfAny, ref namespaceName);
                }

                if (namespaceName.ToLower().StartsWith("global::")) // Note: normally in XAML there is no "global::", but we may enter this method passing a C#-style namespace (cf. section that handles Binding in "GeneratingCSharpCode.cs")
                {
                    namespaceName = namespaceName.Substring("global::".Length);
                }

                // Handle special cases:
                if (localTypeName == "StaticResource")
                {
                    localTypeName = "StaticResourceExtension";
                }

                // Generate string representing the type:
                string fullTypeNameWithNamespaceInsideBraces = !string.IsNullOrEmpty(namespaceName) ? "{" + namespaceName + "}" + localTypeName : localTypeName;

                // Start by looking in the cache dictionary:
                if (!_typeNameToType.TryGetValue(fullTypeNameWithNamespaceInsideBraces, out type))
                {
                    // Look for the type in all loaded assemblies:
                    string assemblyNameWhereTheTypeWasFound = string.Empty;
                    string fullTypeNameFound = string.Empty;
                    foreach (var assemblyKeyValuePair in _loadedAssemblySimpleNameToAssembly)
                    {
                        string assemblySimpleName = assemblyKeyValuePair.Key;
                        Assembly assembly = assemblyKeyValuePair.Value;
                        if (filterAssembliesAndRetainOnlyThoseThatHaveThisName == null
                            || assemblySimpleName == filterAssembliesAndRetainOnlyThoseThatHaveThisName)
                        {
                            List<string> namespacesToLookInto = new List<string>();

                            // If the namespace is a XML namespace (eg. "{http://schemas.microsoft.com/winfx/2006/xaml/presentation}"), we should iterate through all the corresponding CLR namespaces:
                            if (isNamespaceAnXmlNamespace(namespaceName))
                            {
                                foreach (var clrNamespace in GetClrNamespacesFromXmlNamespace(assemblySimpleName, namespaceName))
                                    namespacesToLookInto.Add(clrNamespace);
                            }
                            else
                                namespacesToLookInto.Add(namespaceName);

                            // Search for the type:
                            foreach (var namespaceToLookInto in namespacesToLookInto)
                            {
                                string fullTypeNameToFind = namespaceToLookInto + "." + localTypeName;
                                var typeIfFound = assembly.GetType(fullTypeNameToFind);
                                if (typeIfFound != null)
                                {
                                    if (type != null)
                                    {
                                        //throw new Exception(string.Format("Ambiguous type declaration: the type \"{0}\" is declared at multiple locations. The type \"{1}\" defined in the assembly \"{2}\" appears to have the same identifier as the type \"{3}\" defined in the assembly \"{4}\".", fullTypeNameWithNamespaceInsideBraces, fullTypeNameToFind, assemblySimpleName, fullTypeNameFound, assemblyNameWhereTheTypeWasFound));
                                        if (string.CompareOrdinal(typeIfFound.Namespace, type.Namespace) == -1)
                                            type = typeIfFound;
                                    }

                                    type = typeIfFound;
                                    assemblyNameWhereTheTypeWasFound = assemblySimpleName;
                                    fullTypeNameFound = fullTypeNameToFind;
                                }
                                else
                                {
                                    //try to find a matching nested type.
                                    fullTypeNameToFind = namespaceToLookInto + "+" + localTypeName;
                                    typeIfFound = assembly.GetType(fullTypeNameToFind);
                                    {
                                        if (typeIfFound != null)
                                        {
                                            if (type != null)
                                            {
                                                //throw new Exception(string.Format("Ambiguous type declaration: the type \"{0}\" is declared at multiple locations. The type \"{1}\" defined in the assembly \"{2}\" appears to have the same identifier as the type \"{3}\" defined in the assembly \"{4}\".", fullTypeNameWithNamespaceInsideBraces, fullTypeNameToFind, assemblySimpleName, fullTypeNameFound, assemblyNameWhereTheTypeWasFound));
                                                if (string.CompareOrdinal(typeIfFound.Namespace, type.Namespace) == -1)
                                                    type = typeIfFound;
                                            }
                                            type = typeIfFound;
                                            assemblyNameWhereTheTypeWasFound = assemblySimpleName;
                                            fullTypeNameFound = fullTypeNameToFind;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // Add the type to the cache for later reuse:
                    if (type != null)
                        _typeNameToType.Add(fullTypeNameWithNamespaceInsideBraces, type);
                }

                if (type == null && !doNotRaiseExceptionIfNotFound)
                    throw new wpf::System.Windows.Markup.XamlParseException(
                        "Type not found: \"" + localTypeName + "\""
                        + (!string.IsNullOrEmpty(namespaceName) ? " in namespace: \"" + namespaceName + "\"" : "")
                        + (!string.IsNullOrEmpty(filterAssembliesAndRetainOnlyThoseThatHaveThisName) ? " in assembly: \"" + filterAssembliesAndRetainOnlyThoseThatHaveThisName + "\"" : "")
                        + ".");

                return type;
            }

            public bool DoesTypeContainAttributeToConvertDirectContent(string namespaceName, string localTypeName, string assemblyNameIfAny = null)
            {
                var type = FindType(namespaceName, localTypeName, assemblyNameIfAny);

                // Attempt to get the instance of the attribute if any:
#if BRIDGE || CSHTML5BLAZOR
                Type supportsDirectContentViaTypeFromStringConvertersAttributeType = this.FindType("System.Windows.Markup", "SupportsDirectContentViaTypeFromStringConvertersAttribute");
#else
                Type supportsDirectContentViaTypeFromStringConvertersAttributeType = typeof(DotNetForHtml5Core::System.Windows.Markup.SupportsDirectContentViaTypeFromStringConvertersAttribute);
#endif
                var attribute = Attribute.GetCustomAttribute(type, supportsDirectContentViaTypeFromStringConvertersAttributeType);

                if (attribute != null)
                    return true;
                else
                    return false;
            }

            public string GetContentPropertyName(string namespaceName, string localTypeName, string assemblyNameIfAny = null)
            {
                var type = FindType(namespaceName, localTypeName, assemblyNameIfAny);

                // Get instance of the attribute:
#if BRIDGE || CSHTML5BLAZOR
                Type contentPropertyAttributeType = this.FindType("System.Windows.Markup", "ContentPropertyAttribute");
#else
                Type contentPropertyAttributeType = typeof(ContentPropertyAttribute);
#endif
                var contentProperty = Attribute.GetCustomAttribute(type, contentPropertyAttributeType, true);

                if (contentProperty == null && !IsElementACollection(namespaceName, localTypeName, assemblyNameIfAny)) //if the element is a collection, it is possible to add the children directly to this element.
                    throw new wpf::System.Windows.Markup.XamlParseException("No default content property exists for element: " + localTypeName.ToString());

                if (contentProperty == null)
                    return null;

                string contentPropertyName = (contentPropertyAttributeType.GetProperty("Name").GetValue(contentProperty) ?? "").ToString();

                if (string.IsNullOrEmpty(contentPropertyName))
                    throw new Exception("The ContentPropertyAttribute must have a non-empty Name.");

                return contentPropertyName;
            }

            public bool DoesTypeContainNameMemberOfTypeString(string namespaceName, string localTypeName, string assemblyNameIfAny = null)
            {
                MemberInfo memberInfo;
                memberInfo = GetMemberInfo("Name", namespaceName, localTypeName, assemblyNameIfAny, returnNullIfNotFoundInsteadOfException: true);
                if (memberInfo == null)
                    return false;
                if (memberInfo.MemberType == MemberTypes.Field && ((FieldInfo)memberInfo).FieldType == typeof(string) && ((FieldInfo)memberInfo).IsPublic && !((FieldInfo)memberInfo).IsStatic && !((FieldInfo)memberInfo).IsSecurityCritical)
                    return true;
                if (memberInfo.MemberType == MemberTypes.Property && ((PropertyInfo)memberInfo).PropertyType == typeof(string))
                    return true;
                return false;
            }


            public XName GetCSharpEquivalentOfXamlTypeAsXName(string namespaceName, string localTypeName, string assemblyNameIfAny = null, bool ifTypeNotFoundTryGuessing = false)
            {
                //todo: in this method, we assume that the alias will be global, which will be false if the user chose something else --> find the right alias.

                // Ensure that "ifTypeNotFoundTryGuessing" is always false if the namespace is not a CLR namespace. In fact, in that case, we are unable to guess:
                if (ifTypeNotFoundTryGuessing && isNamespaceAnXmlNamespace(namespaceName))
                    ifTypeNotFoundTryGuessing = false;

                // Find the type:
                Type type = FindType(namespaceName, localTypeName, assemblyNameIfAny, doNotRaiseExceptionIfNotFound: ifTypeNotFoundTryGuessing);
                if (type == null)
                {
                    if (ifTypeNotFoundTryGuessing)
                    {
                        // Try guessing:
                        return XName.Get(!string.IsNullOrEmpty(namespaceName) ? namespaceName + "." + localTypeName : localTypeName, "global::");
                    }
                    else
                    {
                        throw new wpf::System.Windows.Markup.XamlParseException(string.Format("Type \"{0}\" not found in namespace \"{1}\".", localTypeName, namespaceName));
                    }
                }
                else
                {
                    // Use information from the type:
                    return XName.Get(type.Name, namespaceName);
                }
            }

            public string GetCSharpEquivalentOfXamlTypeAsString(string namespaceName, string localTypeName, string assemblyNameIfAny = null, bool ifTypeNotFoundTryGuessing = false)
            {
                //todo: in this method, we assume that the alias will be global, which will be false if the user chose something else --> find the right alias.

                // Ensure that "ifTypeNotFoundTryGuessing" is always false if the namespace is not a CLR namespace. In fact, in that case, we are unable to guess:
                if (isNamespaceAnXmlNamespace(namespaceName))
                    ifTypeNotFoundTryGuessing = false;

                // Distinguish between system types (String, Double...) and other types:
                if (SystemTypesHelper.IsSupportedSystemType(namespaceName, localTypeName, assemblyNameIfAny))
                {
                    return SystemTypesHelper.GetCSharpEquivalentOfXamlType(namespaceName, localTypeName, assemblyNameIfAny);
                }
                else
                {
                    // Find the type:
                    Type type = FindType(namespaceName, localTypeName, assemblyNameIfAny, doNotRaiseExceptionIfNotFound: ifTypeNotFoundTryGuessing);
                    if (type == null)
                    {
                        if (ifTypeNotFoundTryGuessing)
                        {
                            // Try guessing:
                            return "global::" + (!string.IsNullOrEmpty(namespaceName) ? namespaceName + "." + localTypeName : localTypeName);
                        }
                        else
                        {
                            throw new wpf::System.Windows.Markup.XamlParseException(string.Format("Type \"{0}\" not found in namespace \"{1}\".", localTypeName, namespaceName));
                        }
                    }
                    else
                    {
                        // Use information from the type:
                        return "global::" + type.ToString();
                    }
                }
            }

            public Type GetCSharpEquivalentOfXamlType(string namespaceName, string localTypeName, string assemblyIfAny = null, bool ifTypeNotFoundTryGuessing = false)
            {
                Type type = FindType(namespaceName, localTypeName, assemblyIfAny, doNotRaiseExceptionIfNotFound: ifTypeNotFoundTryGuessing);
                if (type == null)
                {
                    if (ifTypeNotFoundTryGuessing)
                    {
                        return null;
                    }
                    else
                    {
                        throw new wpf::System.Windows.Markup.XamlParseException(string.Format("Type \"{0}\" not found in namespace \"{1}\".", localTypeName, namespaceName));
                    }
                }
                else
                {
                    return type;
                }
            }

            public string GetKeyNameOfProperty(string namespaceName, string localTypeName, string assemblyNameIfAny, string propertyName)
            {
                Type type = FindType(namespaceName, localTypeName, assemblyNameIfAny);
                if (type.GetProperty(propertyName) != null)
                {
                    // Look for the static dependency property field in the type and its ancestors:
                    string fieldName = propertyName + "Property";
                    while (type != null)
                    {
                        if (type.GetField(fieldName, BindingFlags.Static | BindingFlags.Public) != null)
                        {
                            return "global::" + type.ToString() + "." + fieldName;
                        }
                        type = type.BaseType;
                    }
                }

                return null;
            }

            public MemberTypes GetMemberType(string memberName, string namespaceName, string localTypeName, string assemblyNameIfAny = null)
            {
                MemberInfo memberInfo = GetMemberInfo(memberName, namespaceName, localTypeName, assemblyNameIfAny);
                return memberInfo.MemberType;
            }

            public string FindCommaSeparatedTypesThatAreSerializable(string assemblySimpleName)
            {
#if BRIDGE || CSHTML5BLAZOR
                throw new NotSupportedException();
#else
                List<string> output = new List<string>();
                if (_loadedAssemblySimpleNameToAssembly.ContainsKey(assemblySimpleName))
                {
                    var assembly = _loadedAssemblySimpleNameToAssembly[assemblySimpleName];

                    foreach (Type type in assembly.GetTypes())
                    {
                        if (type.GetCustomAttributes(typeof(custom::System.Runtime.Serialization.DataContractAttribute), true).Length > 0)
                        {
                            output.Add(type.FullName);
                        }
                    }
                    return string.Join(",", output.ToArray());
                }
                else
                    throw new Exception(ASSEMBLY_NOT_IN_LIST_OF_LOADED_ASSEMBLIES);
#endif
            }

            //public bool IsPropertyAttached(string propertyOrFieldName, string declaringTypeNamespaceName, string declaringTypeLocalName, string parentNamespaceName, string parentLocalTypeName, string parentAssemblyNameIfAny = null)
            //{
            //    var elementType = FindType(declaringTypeNamespaceName, declaringTypeLocalName, parentAssemblyNameIfAny);
            //    var parentElementType = FindType(parentNamespaceName, parentLocalTypeName);
            //    if (elementType == parentElementType || parentElementType.IsSubclassOf(elementType))
            //    {
            //        return false;
            //    }

            //    PropertyInfo propertyInfo = null;

            //    try
            //    {
            //        propertyInfo = elementType.GetProperty(propertyOrFieldName);
            //    }
            //    catch (AmbiguousMatchException) // Can happen if a property hides a property defined in a parent class (with the keyword new)
            //    {
            //        return false;
            //    }
            //    if (propertyInfo != null)
            //    {
            //        return false;
            //    }
            //    FieldInfo fieldInfo = elementType.GetField(propertyOrFieldName + "Property");//todo: if we somehow allow property names to be different than the name + Property, handle this case here.
            //    if (fieldInfo != null)
            //    {
            //        var dp = fieldInfo.GetValue(null);
            //        var dpType = dp.GetType();
            //        PropertyInfo isAttachedProperty = ((Type)dpType).GetProperty("IsAttached");
            //        return (bool)isAttachedProperty.GetValue(dp);
            //    }
            //    else
            //        return false; // This is the case for example for "HtmlCanvas.Children", which is a simple field instead of a dependency property.
            //}

            public bool IsPropertyOrFieldACollection(string propertyOrFieldName, string parentNamespaceName, string parentLocalTypeName, string parentAssemblyNameIfAny = null)
            {
                Type propertyOrFieldType = GetPropertyOrFieldType(propertyOrFieldName, parentNamespaceName, parentLocalTypeName, parentAssemblyNameIfAny);
                bool typeIsACollection = (typeof(IEnumerable).IsAssignableFrom(propertyOrFieldType) && propertyOrFieldType != typeof(string));
                return typeIsACollection;
            }

            public bool IsPropertyOrFieldADictionary(string propertyName, string parentNamespaceName, string parentLocalTypeName, string parentAssemblyNameIfAny = null)
            {
                Type propertyOrFieldType = GetPropertyOrFieldType(propertyName, parentNamespaceName, parentLocalTypeName, parentAssemblyNameIfAny);
                bool isTypeTheIDictionayType = propertyOrFieldType.IsGenericType && propertyOrFieldType.GetGenericTypeDefinition() == typeof(IDictionary<,>);
                bool typeIsADictionary = isTypeTheIDictionayType
                    || (propertyOrFieldType.GetInterface("IDictionary`2") != null && propertyOrFieldType != typeof(string));
                //bool typeIsADictionary = (typeof(IDictionary).IsAssignableFrom(propertyOrFieldType) && propertyOrFieldType != typeof(string));
                return typeIsADictionary;
            }

            public bool DoesMethodReturnACollection(string methodName, string typeNamespaceName, string localTypeName, string typeAssemblyNameIfAny = null)
            {
                Type propertyType = GetMethodReturnValueType(methodName, typeNamespaceName, localTypeName, typeAssemblyNameIfAny);
                bool typeIsACollection = (typeof(IEnumerable).IsAssignableFrom(propertyType) && propertyType != typeof(string));
                return typeIsACollection;
            }

            public bool DoesMethodReturnADictionary(string methodName, string typeNamespaceName, string localTypeName, string typeAssemblyNameIfAny = null)
            {
                Type propertyType = GetMethodReturnValueType(methodName, typeNamespaceName, localTypeName, typeAssemblyNameIfAny);
                bool typeIsADictionary = (propertyType.GetInterface("IDictionary`2") != null && propertyType != typeof(string));
                //bool typeIsADictionary = (typeof(IDictionary).IsAssignableFrom(propertyOrFieldType) && propertyOrFieldType != typeof(string));
                return typeIsADictionary;
            }

            public bool IsElementACollection(string elementNameSpace, string elementLocalName, string assemblyNameIfAny)
            {
                var elementType = FindType(elementNameSpace, elementLocalName, assemblyNameIfAny);

                bool typeIsACollection = (typeof(IEnumerable).IsAssignableFrom(elementType) && elementType != typeof(string));
                return typeIsACollection;
            }

            public bool IsElementADictionary(string elementNameSpace, string elementLocalName, string assemblyNameIfAny)
            {
                var elementType = FindType(elementNameSpace, elementLocalName, assemblyNameIfAny);

                bool typeIsADictionary = (elementType.GetInterface("IDictionary`2") != null && elementType != typeof(string));
                return typeIsADictionary;

            }

            public bool IsElementAMarkupExtension(string elementNameSpace, string elementLocalName, string assemblyNameIfAny)
            {
                var elementType = FindType(elementNameSpace, elementLocalName, assemblyNameIfAny);

                Type markupExtensionGeneric = this.FindType("System.Xaml", "IMarkupExtension`1");
                Type objectType = this.FindType("System", "Object");
                Type markupExtensionOfObject = markupExtensionGeneric.MakeGenericType(objectType);

                bool typeIsAMarkupExtension = (markupExtensionOfObject.IsAssignableFrom(elementType) && elementType != typeof(string));
                return typeIsAMarkupExtension;
            }

            public bool IsElementAnUIElement(string elementNameSpace, string elementLocalName, string assemblyNameIfAny)
            {
                var elementType = FindType(elementNameSpace, elementLocalName, assemblyNameIfAny);

#if BRIDGE || CSHTML5BLAZOR
#if SILVERLIGHTCOMPATIBLEVERSION
                Type uiElementType = this.FindType("System.Windows", "UIElement");
#else
                Type uiElementType = this.FindType("Windows.UI.Xaml", "UIElement");
#endif
#else
#if SILVERLIGHTCOMPATIBLEVERSION
                Type uiElementType = typeof(DotNetForHtml5Core::System.Windows.UIElement);
#else
                Type uiElementType = typeof(DotNetForHtml5Core::Windows.UI.Xaml.UIElement);
#endif
#endif
                bool typeIsAMarkupExtension = (uiElementType.IsAssignableFrom(elementType) && elementType != typeof(string));
                return typeIsAMarkupExtension;
            }

            public bool IsTypeAnEnum(string namespaceName, string localTypeName, string assemblyNameIfAny = null)
            {
                var elementType = FindType(namespaceName, localTypeName, assemblyNameIfAny);
                return elementType.IsEnum;
            }

            public void GetMethodReturnValueTypeInfo(string methodName, string namespaceName, string localTypeName, out string returnValueNamespaceName, out string returnValueLocalTypeName, out bool isTypeString, out bool isTypeEnum, string assemblyNameIfAny = null)
            {
                var type = GetMethodReturnValueType(methodName, namespaceName, localTypeName, assemblyNameIfAny);
                returnValueNamespaceName = this.BuildPropertyPathRecursively(type);
                returnValueLocalTypeName = GetTypeNameIncludingGenericArguments(type);
                isTypeString = (type == typeof(string));
                isTypeEnum = (type.IsEnum);
            }

            public void GetMethodInfo(string methodName, string namespaceName, string localTypeName, out string declaringTypeName, out string returnValueNamespaceName, out string returnValueLocalTypeName, out bool isTypeString, out bool isTypeEnum, string assemblyNameIfAny = null)
            {
                var elementType = FindType(namespaceName, localTypeName, assemblyNameIfAny);
                MethodInfo methodInfo = elementType.GetMethod(methodName);
                if (methodInfo == null)
                {
                    throw new wpf::System.Windows.Markup.XamlParseException("Method \"" + methodName + "\" not found in type \"" + elementType.ToString() + "\".");
                }
                declaringTypeName = "global::" + (!string.IsNullOrEmpty(methodInfo.DeclaringType.Namespace) ? methodInfo.DeclaringType.Namespace + "." : "") + GetTypeNameIncludingGenericArguments(methodInfo.DeclaringType);
                returnValueNamespaceName = this.BuildPropertyPathRecursively(methodInfo.ReturnType);
                returnValueLocalTypeName = GetTypeNameIncludingGenericArguments(methodInfo.ReturnType);
                isTypeString = methodInfo.ReturnType == typeof(string);
                isTypeEnum = methodInfo.ReturnType.IsEnum;
            }

            public void GetPropertyOrFieldTypeInfo(string propertyOrFieldName, string namespaceName, string localTypeName, out string propertyNamespaceName, out string propertyLocalTypeName, out bool isTypeString, out bool isTypeEnum, string assemblyNameIfAny = null, bool isAttached = false)
            {
                var type = GetPropertyOrFieldType(propertyOrFieldName, namespaceName, localTypeName, assemblyNameIfAny, isAttached: isAttached);
                propertyNamespaceName = this.BuildPropertyPathRecursively(type);
                propertyLocalTypeName = GetTypeNameIncludingGenericArguments(type);
                isTypeString = (type == typeof(string));
                isTypeEnum = (type.IsEnum);
            }

            public void GetPropertyOrFieldInfo(string propertyOrFieldName, string namespaceName, string localTypeName, out string memberDeclaringTypeName, out string memberTypeNamespace, out string memberTypeName, out bool isTypeString, out bool isTypeEnum, string assemblyNameIfAny = null, bool isAttached = false)
            {
                var elementType = FindType(namespaceName, localTypeName, assemblyNameIfAny);
                PropertyInfo propertyInfo;
                Type propertyOrFieldType;
                Type propertyOrFieldDeclaringType;
                try
                {
                    propertyInfo = elementType.GetProperty(propertyOrFieldName);
                }
                catch (AmbiguousMatchException)
                {
                    propertyInfo = this.GetPropertyLastImplementationIfMultipleMatches(propertyOrFieldName, elementType);
                }
                if (propertyInfo == null)
                {
                    FieldInfo fieldInfo = elementType.GetField(propertyOrFieldName);
                    if (fieldInfo == null)
                    {
                        throw new wpf::System.Windows.Markup.XamlParseException("Property or field \"" + propertyOrFieldName + "\" not found in type \"" + elementType.ToString() + "\".");
                    }
                    else
                    {
                        propertyOrFieldType = fieldInfo.FieldType;
                        propertyOrFieldDeclaringType = fieldInfo.DeclaringType;
                    }
                }
                else
                {
                    propertyOrFieldType = propertyInfo.PropertyType;
                    propertyOrFieldDeclaringType = propertyInfo.DeclaringType;
                }
                memberDeclaringTypeName = "global::" + (!string.IsNullOrEmpty(propertyOrFieldDeclaringType.Namespace) ? propertyOrFieldDeclaringType.Namespace + "." : "") + GetTypeNameIncludingGenericArguments(propertyOrFieldDeclaringType);
                memberTypeNamespace = this.BuildPropertyPathRecursively(propertyOrFieldType);
                memberTypeName = GetTypeNameIncludingGenericArguments(propertyOrFieldType);
                isTypeString = (propertyOrFieldType == typeof(string));
                isTypeEnum = (propertyOrFieldType.IsEnum);
            }

            private string BuildPropertyPathRecursively(Type type)
            {
                string fullPath = string.Empty;
                Type parentType = type;
                while ((parentType = parentType.DeclaringType) != null)
                {
                    if (!string.IsNullOrEmpty(fullPath))
                    {
                        fullPath = "." + fullPath;
                    }
                    fullPath = parentType.Name + fullPath;
                }
                fullPath = type.Namespace + (!string.IsNullOrEmpty(type.Namespace) && !string.IsNullOrEmpty(fullPath) ? "." : string.Empty) + fullPath;
                return fullPath;
            }

            static string GetTypeNameIncludingGenericArguments(Type type)
            {
                string result = type.Name;
                if (type.IsGenericType)
                {
                    result = result.Split('`')[0];
                    result += "<" + string.Join(", ", type.GenericTypeArguments.Select(x => "global::" + (!string.IsNullOrEmpty(x.Namespace) ? x.Namespace + "." : "") + GetTypeNameIncludingGenericArguments(x))) + ">";
                }
                return result;
            }


            public string GetFieldDeclaringTypeName(string fieldName, string namespaceName, string localTypeName, out string assemblyNameOfDeclaringType, string assemblyNameIfAny = null)
            {
                var elementType = FindType(namespaceName, localTypeName, assemblyNameIfAny);

                FieldInfo fieldInfo = elementType.GetField(fieldName);
                Type declaringType = fieldInfo.DeclaringType;
                assemblyNameOfDeclaringType = declaringType.Assembly.GetName().Name;
                return "global::" + (!string.IsNullOrEmpty(declaringType.Namespace) ? (declaringType.Namespace + ".") : "") + GetTypeNameIncludingGenericArguments(declaringType);
            }

            public string GetPropertyDeclaringTypeName(string propertyName, string namespaceName, string localTypeName, out string assemblyNameOfDeclaringType, string assemblyNameIfAny = null)
            {
                var elementType = FindType(namespaceName, localTypeName, assemblyNameIfAny);

                PropertyInfo propertyInfo;
                try
                {
                    propertyInfo = elementType.GetProperty(propertyName);
                }
                catch (AmbiguousMatchException)
                {
                    propertyInfo = this.GetPropertyLastImplementationIfMultipleMatches(propertyName, elementType);
                }
                Type declaringType = propertyInfo.DeclaringType;
                assemblyNameOfDeclaringType = declaringType.Assembly.GetName().Name;
                return "global::" + (!string.IsNullOrEmpty(declaringType.Namespace) ? (declaringType.Namespace + ".") : "") + GetTypeNameIncludingGenericArguments(declaringType);
            }
            MemberInfo GetMemberInfo(string memberName, string namespaceName, string localTypeName, string assemblyNameIfAny = null, bool returnNullIfNotFoundInsteadOfException = false)
            {
                var elementType = FindType(namespaceName, localTypeName, assemblyNameIfAny);
                MemberInfo[] membersFound = elementType.GetMember(memberName);
                if (membersFound == null || membersFound.Length < 1)
                {
                    if (returnNullIfNotFoundInsteadOfException)
                        return null;
                    else
                        throw new wpf::System.Windows.Markup.XamlParseException("Member \"" + memberName + "\" not found in type \"" + elementType.ToString() + "\".");
                }
                MemberInfo memberInfo = membersFound[0];
                return memberInfo;
            }

            Type GetPropertyOrFieldType(string propertyName, string namespaceName, string localTypeName, string assemblyNameIfAny = null, bool isAttached = false)
            {
                if (isAttached)
                {
                    return GetMethodReturnValueType("Get" + propertyName, namespaceName, localTypeName, assemblyNameIfAny);
                }
                else
                {
                    var elementType = FindType(namespaceName, localTypeName, assemblyNameIfAny);
                    PropertyInfo propertyInfo = null;
                    try
                    {
                        propertyInfo = elementType.GetProperty(propertyName);
                    }
                    catch (AmbiguousMatchException)
                    {
                        propertyInfo = GetPropertyLastImplementationIfMultipleMatches(propertyName, elementType);
                    }
                    if (propertyInfo == null)
                    {
                        FieldInfo fieldInfo = elementType.GetField(propertyName);
                        if (fieldInfo == null)
                        {
                            throw new wpf::System.Windows.Markup.XamlParseException("Property or field \"" + propertyName + "\" not found in type \"" + elementType.ToString() + "\".");
                        }
                        else
                        {
                            Type fieldType = fieldInfo.FieldType;
                            return fieldType;
                        }
                    }
                    else
                    {
                        Type propertyType = propertyInfo.PropertyType;
                        return propertyType;
                    }
                }
            }

            PropertyInfo GetPropertyLastImplementationIfMultipleMatches(string propertyName, Type type)
            {
                Type currentType = type;
                while (currentType != null)
                {
                    foreach (PropertyInfo property in currentType.GetProperties())
                    {
                        if (property.Name == propertyName)
                        {
                            return property;
                        }
                    }
                    currentType = currentType.BaseType;
                }
                return null;
            }

            Type GetMethodReturnValueType(string methodName, string namespaceName, string localTypeName, string assemblyNameIfAny = null)
            {
                Type elementType = FindType(namespaceName, localTypeName, assemblyNameIfAny);
                Type currentType = elementType;
                MethodInfo methodInfo = null;

                while (methodInfo == null && currentType != null)
                {
                    MethodInfo[] methods = currentType.GetMethods();
                    methodInfo = methods.FirstOrDefault(m => m.Name == methodName);
                    currentType = currentType.BaseType;
                }

                if (methodInfo == null)
                    throw new wpf::System.Windows.Markup.XamlParseException("Method \"" + methodName + "\" not found in type \"" + elementType.ToString() + "\".");
                Type methodType = methodInfo.ReturnType;
                return methodType;
            }

            public bool IsTypeAssignableFrom(string nameSpaceOfTypeToAssignFrom, string nameOfTypeToAssignFrom, string assemblyNameOfTypeToAssignFrom, string nameSpaceOfTypeToAssignTo, string nameOfTypeToAssignTo, string assemblyNameOfTypeToAssignTo, bool isAttached = false)
            {
                Type typeOfElementToAssignFrom;
                Type typeOfElementToAssignTo;

                int indexOfLastDot = nameOfTypeToAssignFrom.LastIndexOf('.');

                if (indexOfLastDot == -1)
                {
                    typeOfElementToAssignFrom = FindType(nameSpaceOfTypeToAssignFrom, nameOfTypeToAssignFrom, assemblyNameOfTypeToAssignFrom);
                }
                else
                {
                    string localTypeName = nameOfTypeToAssignFrom.Substring(0, indexOfLastDot);
                    string propertyName = nameOfTypeToAssignFrom.Substring(indexOfLastDot + 1);
                    typeOfElementToAssignFrom = GetPropertyOrFieldType(propertyName, nameSpaceOfTypeToAssignFrom, localTypeName, assemblyNameOfTypeToAssignFrom);
                }

                indexOfLastDot = nameOfTypeToAssignTo.LastIndexOf('.');
                if(indexOfLastDot == -1)
                {
                    typeOfElementToAssignTo = FindType(nameSpaceOfTypeToAssignTo, nameOfTypeToAssignTo, assemblyNameOfTypeToAssignTo);
                }
                else
                {
                    string localTypeName = nameOfTypeToAssignTo.Substring(0, indexOfLastDot);
                    string propertyName = nameOfTypeToAssignTo.Substring(indexOfLastDot + 1);
                    typeOfElementToAssignTo = GetPropertyOrFieldType(propertyName, nameSpaceOfTypeToAssignTo, localTypeName, assemblyNameOfTypeToAssignTo, isAttached);
                }

                return typeOfElementToAssignTo.IsAssignableFrom(typeOfElementToAssignFrom);
            }

            bool isNamespaceAnXmlNamespace(string namespaceName)
            {
                return namespaceName.StartsWith("http://"); //todo: are there other conditions possible for XML namespaces declared with xmlnsDefinitionAttribute?
            }

            public string GetCSharpXamlForHtml5CompilerVersionNumberOrNull(string assemblySimpleName)
            {
                if (_loadedAssemblySimpleNameToAssembly.ContainsKey(assemblySimpleName))
                {
                    var assembly = _loadedAssemblySimpleNameToAssembly[assemblySimpleName];

#if BRIDGE || CSHTML5BLAZOR
                    Type attributeType = this.FindType("CSHTML5.Internal.Attributes", "CompilerVersionNumberAttribute");
#else
                    Type attributeType = typeof(DotNetForHtml5Core::CompilerVersionNumberAttribute);
#endif
                    var attribute = assembly.GetCustomAttributes(attributeType, true).SingleOrDefault();

                    if (attribute != null)
                    {
                        string result = (attributeType.GetProperty("VersionNumber").GetValue(attribute) ?? "").ToString();
                        if (string.IsNullOrEmpty(result))
                            throw new Exception("Incorrect CompilerVersionNumberAttribute.VersionNumber");
                        return result;
                    }
                    else
                        return null;
                }
                else
                    throw new Exception(ASSEMBLY_NOT_IN_LIST_OF_LOADED_ASSEMBLIES);
            }

            public string GetCSharpXamlForHtml5CompilerVersionFriendlyNameOrNull(string assemblySimpleName)
            {
                if (_loadedAssemblySimpleNameToAssembly.ContainsKey(assemblySimpleName))
                {
                    var assembly = _loadedAssemblySimpleNameToAssembly[assemblySimpleName];

#if BRIDGE || CSHTML5BLAZOR
                    Type attributeType = this.FindType("CSHTML5.Internal.Attributes", "CompilerVersionFriendlyNameAttribute");
#else
                    Type attributeType = typeof(DotNetForHtml5Core::CompilerVersionFriendlyNameAttribute);
#endif
                    var attribute = assembly.GetCustomAttributes(attributeType, true).SingleOrDefault();

                    if (attribute != null)
                    {
                        string result = (attributeType.GetProperty("VersionFriendlyName").GetValue(attribute) ?? "").ToString();
                        if (string.IsNullOrEmpty(result))
                            throw new Exception("Incorrect CompilerVersionFriendlyNameAttribute.VersionFriendlyName");
                        return result;
                    }
                    else
                        return null;
                }
                else
                    throw new Exception(ASSEMBLY_NOT_IN_LIST_OF_LOADED_ASSEMBLIES);
            }

            public string GetCSharpXamlForHtml5MinimumRequiredCompilerVersionNumberOrNull(string assemblySimpleName)
            {
                if (_loadedAssemblySimpleNameToAssembly.ContainsKey(assemblySimpleName))
                {
                    var assembly = _loadedAssemblySimpleNameToAssembly[assemblySimpleName];

#if BRIDGE || CSHTML5BLAZOR
                    Type attributeType = this.FindType("CSHTML5.Internal.Attributes", "MinimumRequiredCompilerVersionNumberAttribute");
#else
                    Type attributeType = typeof(DotNetForHtml5Core::MinimumRequiredCompilerVersionNumberAttribute);
#endif
                    var attribute = assembly.GetCustomAttributes(attributeType, true).SingleOrDefault();

                    if (attribute != null)
                    {
                        string result = (attributeType.GetProperty("VersionNumber").GetValue(attribute) ?? "").ToString();
                        if (string.IsNullOrEmpty(result))
                            throw new Exception("Incorrect MinimumRequiredCompilerVersionNumberAttribute.VersionNumber");
                        return result;
                    }
                    else
                        return null;
                }
                else
                    throw new Exception(ASSEMBLY_NOT_IN_LIST_OF_LOADED_ASSEMBLIES);
            }

            public string GetCSharpXamlForHtml5MinimumRequiredCompilerVersionFriendlyNameOrNull(string assemblySimpleName)
            {
                if (_loadedAssemblySimpleNameToAssembly.ContainsKey(assemblySimpleName))
                {
                    var assembly = _loadedAssemblySimpleNameToAssembly[assemblySimpleName];

#if BRIDGE || CSHTML5BLAZOR
                    Type attributeType = this.FindType("CSHTML5.Internal.Attributes", "MinimumRequiredCompilerVersionFriendlyNameAttribute");
#else
                    Type attributeType = typeof(DotNetForHtml5Core::MinimumRequiredCompilerVersionFriendlyNameAttribute);
#endif
                    var attribute = assembly.GetCustomAttributes(attributeType, true).SingleOrDefault();

                    if (attribute != null)
                    {
                        string result = (attributeType.GetProperty("VersionFriendlyName").GetValue(attribute) ?? "").ToString();
                        if (string.IsNullOrEmpty(result))
                            throw new Exception("Incorrect MinimumRequiredCompilerVersionFriendlyNameAttribute.VersionFriendlyName");
                        return result;
                    }
                    else
                        return null;
                }
                else
                    throw new Exception(ASSEMBLY_NOT_IN_LIST_OF_LOADED_ASSEMBLIES);
            }

            public Dictionary<string, byte[]> GetManifestResources(string assemblySimpleName, Func<string, bool> filenamePredicate)
            {
                if (_loadedAssemblySimpleNameToAssembly.ContainsKey(assemblySimpleName))
                {
                    var assembly = _loadedAssemblySimpleNameToAssembly[assemblySimpleName];

                    var manifestResourceNames = assembly.GetManifestResourceNames();
                    var resourceFiles = (from fn in manifestResourceNames where filenamePredicate(fn) select fn).ToArray();
                    var result = new Dictionary<string, byte[]>();

                    foreach (var resourceFile in resourceFiles)
                    {
                        var stream = assembly.GetManifestResourceStream(resourceFile);
                        if (stream == null)
                            throw new FileNotFoundException("No manifest resource stream named " + resourceFile);

                        using (stream)
                        {
                            var buffer = new byte[stream.Length];
                            stream.Read(buffer, 0, buffer.Length);
                            result[resourceFile] = buffer;
                        }
                    }

                    return result;
                }
                else
                    throw new Exception(ASSEMBLY_NOT_IN_LIST_OF_LOADED_ASSEMBLIES);
            }


            public Dictionary<string, byte[]> GetManifestResources(string assemblySimpleName, HashSet<string> supportedExtensionsLowerCase)
            {
                if (_loadedAssemblySimpleNameToAssembly.ContainsKey(assemblySimpleName))
                {
                    var assembly = _loadedAssemblySimpleNameToAssembly[assemblySimpleName];

                    var manifestResourceNames = assembly.GetManifestResourceNames();
                    var resourceFiles = (from fn in manifestResourceNames where supportedExtensionsLowerCase.Contains(Path.GetExtension(fn.ToLower())) select fn).ToArray();
                    var result = new Dictionary<string, byte[]>();

                    foreach (var resourceFile in resourceFiles)
                    {
                        var stream = assembly.GetManifestResourceStream(resourceFile);
                        if (stream == null)
                            throw new FileNotFoundException("No manifest resource stream named " + resourceFile);

                        using (stream)
                        {
                            var buffer = new byte[stream.Length];
                            stream.Read(buffer, 0, buffer.Length);
                            result[resourceFile] = buffer;
                        }
                    }

                    return result;
                }
                else
                    throw new Exception(ASSEMBLY_NOT_IN_LIST_OF_LOADED_ASSEMBLIES);
            }

            public Dictionary<string, byte[]> GetResources(string assemblySimpleName, HashSet<string> supportedExtensionsLowercase)
            {
                if (_loadedAssemblySimpleNameToAssembly.ContainsKey(assemblySimpleName))
                {
                    //---------------------------------------------
                    // All the resources (ie. all the files with a "BuildAction" set to "Resource) are located inside the manifest resource named "AssemblyName.g.resources"
                    //---------------------------------------------

                    var assembly = _loadedAssemblySimpleNameToAssembly[assemblySimpleName];
                    string resName = assembly.GetName().Name + ".g.resources";
                    var result = new Dictionary<string, byte[]>();
                    using (var stream = assembly.GetManifestResourceStream(resName))
                    {
                        if (stream != null)
                        {
                            using (var reader = new System.Resources.ResourceReader(stream))
                            {
                                //--------------------------
                                // Get the name of the files:
                                //--------------------------

                                string[] resourceNames = reader.Cast<DictionaryEntry>().Select(entry => (string)entry.Key).ToArray();

                                //--------------------------
                                // Read the files content:
                                //--------------------------

                                foreach (string fileName in resourceNames)
                                {
                                    // Unescape the fileName (for example, replacing "%20" with " "):
                                    string fileNameUnescaped = Uri.UnescapeDataString(fileName);

                                    // Check if the extension is in the list of supported extensions:
                                    if (IsExtensionSupported(fileNameUnescaped, supportedExtensionsLowercase))
                                    {
                                        // Get the data:
                                        string resourceType;
                                        byte[] data;
                                        reader.GetResourceData(fileName, out resourceType, out data);

                                        if (data != null && resourceType == "ResourceTypeCode.Stream")
                                        {
                                            // Remove the first 4 bytes // cf. http://stackoverflow.com/questions/32891004/why-resourcereader-getresourcedata-return-data-of-type-resourcetypecode-stream
                                            const int OFFSET = 4;
                                            int newLength = data.Length - OFFSET;
                                            byte[] fileContent = new byte[newLength];
                                            Array.Copy(data, OFFSET, fileContent, 0, newLength);

                                            // Remember the result:
                                            result[fileNameUnescaped] = fileContent; // Not using the "Add" method so that if 2 keys are the same, we don't get an error.
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            // The manifest resource does not exist. This is normal in case that there are no files with a BuildAction set to "Resource".
                            // We ignore and continue.
                        }
                        return result;
                    }

                }
                else
                    throw new Exception(ASSEMBLY_NOT_IN_LIST_OF_LOADED_ASSEMBLIES);
            }

            public Type GetTypeInCoreAssemblies(string typeFullName)
            {
                if (_cacheForResolvedTypesInCoreAssembly.ContainsKey(typeFullName))
                    return _cacheForResolvedTypesInCoreAssembly[typeFullName];
                else
                {
#if BRIDGE
                    if (_coreAssemblies.Count == 0)
                        throw new Exception("The list of CoreAssemblies has not been initialized.");
#endif
                    foreach (var coreAssembly in _coreAssemblies)
                    {
                        var type = coreAssembly.GetType(typeFullName, throwOnError: false);
                        if (type != null)
                        {
                            _cacheForResolvedTypesInCoreAssembly[typeFullName] = type;
                            return type;
                        }
                    }
                    throw new Exception("Type not found '" + typeFullName + "' in core assemblie(s).");
                }
            }

            public void SetTypeForwardingAssemblyPath(string typeForwardingAssemblyPath)
            {
#if BRIDGE
                _typeForwardingAssemblyPath = typeForwardingAssemblyPath;
#endif
            }

            public bool TryGenerateCodeForInstantiatingAttributeValue(string xamlValue, out string generatedCSharpCode, string valueNamespaceName, string valueLocalTypeName, string valueAssemblyNameIfAny)
            {
                //todo: handle built-in types here (Enum, string, int, double, etc.)

                Type type = FindType(valueNamespaceName, valueLocalTypeName, valueAssemblyNameIfAny);

                if (type.FullName == "System.String")
                {
                    generatedCSharpCode = "@\"" + xamlValue.Replace("\"", "\"\"") + "\"";
                    return true;
                }

                if (type.IsEnum)
                {
                    FieldInfo xamlValueToEnumValue = type.GetField(xamlValue, BindingFlags.IgnoreCase);
                    if (xamlValueToEnumValue == null)
                    {
                        generatedCSharpCode = String.Format("{0}.{1}", "global::" + type.FullName, xamlValue);
                    }
                    else
                    {
                        generatedCSharpCode = String.Format("{0}.{1}", "global::" + type.FullName, xamlValueToEnumValue.Name);
                    }
                    return true;
                }

                // Attempt to get the isntance of the attribute if any
#if BRIDGE || CSHTML5BLAZOR
                Type methodToTranslateXamlValueToCSharpAttribute = this.FindType("System.Windows.Markup", "MethodToTranslateXamlValueToCSharpAttribute");
#else
                Type methodToTranslateXamlValueToCSharpAttribute = typeof(DotNetForHtml5Core::System.Windows.Markup.MethodToTranslateXamlValueToCSharpAttribute);
#endif
                var attribute = Attribute.GetCustomAttribute(type, methodToTranslateXamlValueToCSharpAttribute);
                if (attribute == null)
                {
                    generatedCSharpCode = "";
                    return false;
                }
                string methodName = (methodToTranslateXamlValueToCSharpAttribute.GetProperty("MethodName").GetValue(attribute) ?? "").ToString();
                if (string.IsNullOrEmpty(methodName))
                {
                    throw new Exception("Property 'MethodName' not found in type '" + methodToTranslateXamlValueToCSharpAttribute.FullName + "'");
                }
                // throw clear exception if the method is not found.
                generatedCSharpCode = type.GetMethod(methodName).Invoke(null, new object[] { xamlValue }).ToString();
                return true;
            }

            public string GetFieldName(string fieldNameIgnoreCase, string namespaceName, string localTypeName, string assemblyIfAny = null)
            {
                Type type = FindType(namespaceName, localTypeName, assemblyIfAny);

                if (type == null) throw new wpf::System.Windows.Markup.XamlParseException($"Type '{localTypeName}' not found in namepsace '{namespaceName}'.");

                FieldInfo field;
                if (type.IsEnum)
                {
                    field = type.GetField(fieldNameIgnoreCase, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Static);
                    if (field == null)
                    {
                        // If the field isn't found "as is", we try to interpret it as the int corresponding to a field
                        if (int.TryParse(fieldNameIgnoreCase, out int value))
                        {
                            string trueFieldName = Enum.GetName(type, Enum.ToObject(type, value));
                            field = type.GetField(trueFieldName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Static);
                        }
                    }
                }
                else
                {
                    field = type.GetField(fieldNameIgnoreCase, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Static);
                }

                return field.Name ?? throw new wpf::System.Windows.Markup.XamlParseException($"Field '{fieldNameIgnoreCase}' not found in type: '{type.FullName}'.");
            }

            // note: this method has been abandonned because fieldInfo.GetValue(null) can 
            // crash if the class constructor has not been called. (to reproduce build 
            // project T_UI_OS_RichTextBoxUI)
            // Adding a try catch can lead to a StackOverflowException.
            //public bool IsPropertyAttached(string propertyOrFieldName, string declaringTypeNamespaceName, string declaringTypeLocalName, string parentNamespaceName, string parentLocalTypeName, string parentAssemblyNameIfAny = null)
            //{
            //    var elementType = FindType(declaringTypeNamespaceName, declaringTypeLocalName, parentAssemblyNameIfAny);
            //    FieldInfo fieldInfo = elementType.GetField(propertyOrFieldName + "Property");//todo: if we somehow allow property names to be different than the name + Property, handle this case here.
            //    if (fieldInfo != null)
            //    {
            //      object dp = fieldInfo.GetValue(null);
            //      PropertyInfo isAttachedProperty = dp.GetType().GetProperty("IsAttached");
            //      return (bool)isAttachedProperty.GetValue(dp);
            //    }
            //    return false;
            //}

            public bool IsPropertyAttached(string propertyOrFieldName, string declaringTypeNamespaceName, string declaringTypeLocalName, string parentNamespaceName, string parentLocalTypeName, string parentAssemblyNameIfAny = null)
            {
                Type elementType = FindType(declaringTypeNamespaceName, declaringTypeLocalName, parentAssemblyNameIfAny);
                Type currentType = elementType;
                FieldInfo fieldInfo = null;

                while (currentType != typeof(object))
                {
                    fieldInfo = currentType.GetField(propertyOrFieldName + "Property"); //todo: if we somehow allow property names to be different than the name + Property, handle this case here.
                    if (fieldInfo != null) break;
                    currentType = currentType.BaseType;
                }

                if (fieldInfo != null)
                {
                    if (fieldInfo.FieldType.Name == "DependencyProperty")
                    {
                        int nbOfParameters = 2;
                        MethodInfo method = currentType.GetMethod("Set" + propertyOrFieldName, BindingFlags.Public | BindingFlags.Static);
                        if (method == null)
                        {
                            method = currentType.GetMethod("Get" + propertyOrFieldName, BindingFlags.Public | BindingFlags.Static);
                            nbOfParameters = 1;
                        }
                        if (method != null)
                        {
                            if (method.GetParameters().Length == nbOfParameters)
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }

            public bool IsAssignableFrom(string namespaceName, string typeName, string fromNamespaceName, string fromTypeName)
            {
                Type type = this.FindType(namespaceName, typeName);
                Type fromType = this.FindType(fromNamespaceName, fromTypeName);

                return type.IsAssignableFrom(fromType);
            }
        }

        static bool IsExtensionSupported(string fileName, HashSet<string> supportedExtensionsLowercase)
        {
            int lastIndexOfDot = fileName.LastIndexOf('.');
            if (lastIndexOfDot > -1)
            {
                string extension = fileName.Substring(lastIndexOfDot);
                return supportedExtensionsLowercase.Contains(extension.ToLowerInvariant());
            }
            else
            {
                return false;
            }
        }
    }
}
