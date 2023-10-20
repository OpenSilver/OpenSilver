

/*===================================================================================
* 
*   Copyright (c) Userware (OpenSilver.net, CSHTML5.com)
*      
*   This file is part of both the OpenSilver Simulator (https://opensilver.net), which
*   is licensed under the MIT license (https://opensource.org/licenses/MIT), and the
*   CSHTML5 Simulator (http://cshtml5.com), which is dual-licensed (MIT + commercial).
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Linq;
using System.Collections.Generic;
using OpenSilver;

namespace DotNetForHtml5.EmulatorWithoutJavascript
{
    static class ReflectionInUserAssembliesHelper
    {
        static Assembly _coreAssembly;
        static Dictionary<string, Type> _typesCacheForPerformance = new Dictionary<string, Type>();

        internal static bool TryGetPathOfAssemblyThatContainsEntryPoint(out string path)
        {
            path = null;
            string[] commandLineArgs = Environment.GetCommandLineArgs();
            if (commandLineArgs.Length >= 2 && !string.IsNullOrEmpty(commandLineArgs[1]))
            {
                path = commandLineArgs[1];

                // If path is not absolute, make it absolute:
                string currentDirectory = null;
                bool isPathRelative =
                    !Path.IsPathRooted(path)
                    && !string.IsNullOrEmpty(path)
                    && !string.IsNullOrEmpty(currentDirectory = Directory.GetCurrentDirectory());
                if (isPathRelative)
                {
                    path = Path.Combine(currentDirectory, path);
                }

                return true;
            }
            else
                return false;
        }

        internal static bool TryGetCustomBaseUrl(out string customBaseUrl)
        {
            const string prefix = "/baseurl:";
            customBaseUrl = null;
            string[] commandLineArgs = Environment.GetCommandLineArgs();
            if (commandLineArgs.Length >= 3
                && !string.IsNullOrEmpty(commandLineArgs[2])
                && commandLineArgs[2].StartsWith(prefix))
            {
                // Remove the prefix:
                customBaseUrl = commandLineArgs[2].Substring(prefix.Length);

                // Remove the quotes if any:
                customBaseUrl = customBaseUrl.Trim('"');

                return true;
            }
            else
                return false;
        }

        internal static bool TryGetCoreAssembly(out Assembly coreAssembly)
        {
            // Read from cache if previously found:
            if (_coreAssembly != null)
            {
                coreAssembly = _coreAssembly;
                return true;
            }
            else
            {
                coreAssembly =
                    (from a in AppDomain.CurrentDomain.GetAssemblies()
                     where (string.Equals(a.GetName().Name, Constants.NAME_OF_CORE_ASSEMBLY, StringComparison.CurrentCultureIgnoreCase)
                     || string.Equals(a.GetName().Name, Constants.NAME_OF_CORE_ASSEMBLY_USING_BRIDGE, StringComparison.CurrentCultureIgnoreCase)
                     || string.Equals(a.GetName().Name, Constants.NAME_OF_CORE_ASSEMBLY_SLMIGRATION, StringComparison.CurrentCultureIgnoreCase)
                     || string.Equals(a.GetName().Name, Constants.NAME_OF_CORE_ASSEMBLY_SLMIGRATION_USING_BRIDGE, StringComparison.CurrentCultureIgnoreCase)
                     || string.Equals(a.GetName().Name, Constants.NAME_OF_CORE_ASSEMBLY_USING_BLAZOR, StringComparison.CurrentCultureIgnoreCase))
                     select a).FirstOrDefault();
                if (coreAssembly != null)
                {
                    _coreAssembly = coreAssembly;
                    return true;
                }
                else
                {
                    MessageBox.Show("Could not find the core assembly among the loaded assemblies.");
                    return false;
                }
            }
        }

        internal static bool TryGetTypeInCoreAssembly(string typeNamespace, string typeAlternativeNamespaceOrNull, string typeName, out Type type, out Assembly coreAssembly)
        {
            if (_typesCacheForPerformance.ContainsKey(typeNamespace + "." + typeName))
            {
                type = _typesCacheForPerformance[typeNamespace + "." + typeName];
                coreAssembly = _coreAssembly;
                return true;
            }
            else if (!string.IsNullOrEmpty(typeAlternativeNamespaceOrNull) && _typesCacheForPerformance.ContainsKey(typeAlternativeNamespaceOrNull + "." + typeName))
            {
                type = _typesCacheForPerformance[typeAlternativeNamespaceOrNull + "." + typeName];
                coreAssembly = _coreAssembly;
                return true;
            }
            else
            {
                // Get the "Core" assembly:
                if (TryGetCoreAssembly(out coreAssembly))
                {
                    // Look for the type in the core assembly:
                    type = (
                        from t
                        in coreAssembly.GetTypes()
                        where (t.Namespace == typeNamespace && t.Name == typeName)
                        select t).FirstOrDefault();
                    if (type != null)
                    {
                        _typesCacheForPerformance[typeNamespace + "." + typeName] = type;
                        return true;
                    }

                    // Look for the alternative namespace if it was specified:
                    if (!string.IsNullOrEmpty(typeAlternativeNamespaceOrNull))
                    {
                        type = (
                        from t
                        in coreAssembly.GetTypes()
                        where (t.Namespace == typeAlternativeNamespaceOrNull && t.Name == typeName)
                        select t).FirstOrDefault();
                        if (type != null)
                        {
                            _typesCacheForPerformance[typeAlternativeNamespaceOrNull + "." + typeName] = type;
                            return true;
                        }
                    }

                    // Otherwise, failure:
                    MessageBox.Show(string.Format("Could not find the type \"{1}\" in the core assembly.", typeName));
                    return false;
                }
                else
                {
                    // A message box has already been displayed by the method "TryGetCoreAssembly".
                    type = null;
                    return false;
                }
            }
        }

        internal static void GetOutputPathsByReadingAssemblyAttributes(
            Assembly entryPointAssembly,
            out string outputRootPath,
            out string outputAppFilesPath,
            out string outputLibrariesPath,
            out string outputResourcesPath,
            out string intermediateOutputAbsolutePath)
        {
            // todo: see if the path can be not hard-coded
            // In the OpenSilver version, the app use the wwwroot folder to store the libs and resources
            // This folder is not inside the build dir (bin\Debug\netstandard2.0) but at the root level
            outputRootPath = @"..\..\..\wwwroot\"; 
            outputLibrariesPath = @"\app-cshtml5\libs\";
            outputResourcesPath = @"\app-cshtml5\res\";
            outputAppFilesPath = @"";
            intermediateOutputAbsolutePath = @"";
        }
    }
}
