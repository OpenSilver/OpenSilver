

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

        internal static bool TryDetermineTypeThatInheritsFromApplication(Assembly entryPointAssembly, out Type typeThatInheritsFromApplication, out Assembly coreAssembly)
        {
            coreAssembly = null;
            string errorMessage = "No entry point was found. Please make sure that the application contains a class that inherits from \"Application\". The default constructor of that class will be used as the entry point of the application.\r\n\r\nTo see an example, under Visual Studio, click File -> New -> Project, and create a project of type \"C#/XAML for HTML5\" application";
            typeThatInheritsFromApplication = null;
            try
            {
                // Get the list of types in the entryPointAssembly:
                //--------------
                // IMPORTANT: we must do this before calling "AppDomain.CurrentDomain.GetAssemblies" because this will raise the "AssemblyResolve" event which is used to load the Core assembly.
                //--------------
                Type[] typesInEntryPointAssembly = null;

                try
                {
                    typesInEntryPointAssembly = entryPointAssembly.GetTypes();
                }
                catch (Exception ex)
                {
                    // The following lines provide no functionality but they make it easier to obtain the LoaderExceptions information when doing step-by-step debugging on the compiler.
                    if (ex is System.Reflection.ReflectionTypeLoadException)
                    {
                        var typeLoadException = ex as ReflectionTypeLoadException;
                        var loaderExceptions = typeLoadException.LoaderExceptions;
                    }
                    throw;
                }

                // Find the type "Application" in Core:
                Type baseApplicationType;
                if (TryGetTypeInCoreAssembly(
                    typeNamespace: "Windows.UI.Xaml",
                    typeAlternativeNamespaceOrNull: "System.Windows",
                    typeName: "Application",
                    type: out baseApplicationType,
                    coreAssembly: out coreAssembly))
                {
                    // Find the type that inherits from "Application" (note: we don't use "linq" because performance is better with the "return" statement that breaks the foreach).
                    foreach (Type type in typesInEntryPointAssembly)
                    {
                        if (baseApplicationType.IsAssignableFrom(type))
                        {
                            typeThatInheritsFromApplication = type;
                            return true;
                        }
                    }
                }
                else
                {
                    // A message box has already been displayed.
                    return false;
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                var errorMessages = string.Join("\r\n", from loaderException in ex.LoaderExceptions select (loaderException.Message));
                MessageBox.Show(errorMessages);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(errorMessage + "\r\n\r\n" + ex.Message);
                return false;
            }

            MessageBox.Show(errorMessage);
            return false;
        }

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

        internal static bool WasAssemblyWasCompiledInSLMigrationMode()
        {
            Assembly coreAssembly;
            if (TryGetCoreAssembly(out coreAssembly))
            {
                if (coreAssembly.GetName().Name.StartsWith("SLMigration."))
                    return true;
                else
                    return false;
            }
            else
            {
                // A message box has already been displayed by the method "TryGetCoreAssembly".
                return false;
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
#if OPENSILVER
            // todo: see if the path can be not hard-coded
            // In the OpenSilver version, the app use the wwwroot folder to store the libs and resources
            // This folder is not inside the build dir (bin\Debug\netstandard2.0) but at the root level
            outputRootPath = @"..\..\..\wwwroot\"; 
            outputLibrariesPath = @"\app-cshtml5\libs\";
            outputResourcesPath = @"\app-cshtml5\res\";
            outputAppFilesPath = @"";
            intermediateOutputAbsolutePath = @"";
#elif BRIDGE
            // In the Bridge-based version, the paths are hard-coded because they cannot be configured by the user at this time:
            outputRootPath = @"Output\";
            outputLibrariesPath = @"app-cshtml5\libs\";
            outputResourcesPath = @"app-cshtml5\res\";
            outputAppFilesPath = null; // Not used in the Bridge-based version
            intermediateOutputAbsolutePath = null; // Not used in the Bridge-based version because the Simulator does not do the compilation.
#else
            outputRootPath = null;
            outputAppFilesPath = null;
            outputLibrariesPath = null;
            outputResourcesPath = null;
            intermediateOutputAbsolutePath = null;

            foreach (Attribute attribute in entryPointAssembly.GetCustomAttributes())
            {
                Type attributeType = attribute.GetType();
                string attributeName = attributeType.Name;

                if (attributeName == "OutputRootPathAttribute")
                    outputRootPath = GetPropertyValue<string>(attributeType, attribute, attributeName, "OutputRootPath");

                if (attributeName == "OutputAppFilesPathAttribute")
                    outputAppFilesPath = GetPropertyValue<string>(attributeType, attribute, attributeName, "OutputAppFilesPath");

                if (attributeName == "OutputLibrariesPathAttribute")
                    outputLibrariesPath = GetPropertyValue<string>(attributeType, attribute, attributeName, "OutputLibrariesPath");

                if (attributeName == "OutputResourcesPathAttribute")
                    outputResourcesPath = GetPropertyValue<string>(attributeType, attribute, attributeName, "OutputResourcesPath");

                if (attributeName == "IntermediateOutputAbsolutePathAttribute")
                    intermediateOutputAbsolutePath = GetPropertyValue<string>(attributeType, attribute, attributeName, "IntermediateOutputAbsolutePath");
            }

            if (outputRootPath == null
                || outputAppFilesPath == null
                || outputLibrariesPath == null
                || outputResourcesPath == null
                || intermediateOutputAbsolutePath == null
                )
                throw new Exception("One of the output path attributes could not be found. Please contact support.");
#endif
        }

        internal static void GetSettingsByReadingAssemblyAttributes(
            Assembly entryPointAssembly,
            out bool generateJavaScriptDuringBuild)
        {
            // Default values:
            generateJavaScriptDuringBuild = false;

            // Find the "SettingsAttribute" and read its properties:
            foreach (Attribute attribute in entryPointAssembly.GetCustomAttributes())
            {
                Type attributeType = attribute.GetType();

                if (attributeType.Name == "SettingsAttribute")
                {
                    // Read the properties of the "SettingsAttribute":
                    generateJavaScriptDuringBuild = GetPropertyValue<bool>(attributeType, attribute, "SettingsAttribute", "GenerateJavaScriptDuringBuild");
                }
            }
        }

        static T GetPropertyValue<T>(Type attributeType, Attribute attribute, string attributeName, string propertyName)
        {
            PropertyInfo propertyInfo = attributeType.GetProperty(propertyName);

            if (propertyInfo == null)
                throw new Exception(string.Format("Could not find the property '{1}' of the attribute '{0}'. Please contact support.", attributeName, propertyName));

            object value = propertyInfo.GetValue(attribute);

            if (value == null)
                throw new Exception(string.Format("The value of the property '{1}' of the attribute '{0}' is null. Please contact support.", attributeName, propertyName));

            if (typeof(T) == typeof(string))
            {
                string valueAsString = value.ToString();

                if (string.IsNullOrWhiteSpace(valueAsString))
                    throw new Exception(string.Format("The value of the property '{1}' of the attribute '{0}' is empty or whitespace. Please contact support.", attributeName, propertyName));

                return (T)(object)valueAsString;
            }
            else
            {
                T castedValue;
                try
                {
                    castedValue = (T)value;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("The value of the property '{1}' of the attribute '{0}' is not a valid {2}. Please contact support.", attributeName, propertyName, typeof(T).FullName));
                }
                return castedValue;
            }
        }
    }
}
