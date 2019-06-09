
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



#if !BRIDGE
using JSIL.Meta;
#else
using Bridge;
#endif
using CSHTML5.Internal;
using DotNetForHtml5.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Browser;
#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace CSHTML5.Internal
{
    public static class INTERNAL_UriHelper
    {
        /// <summary>
        /// Converts to an URI suitable to use in the HTML5 "src" property.
        /// </summary>
        /// <param name="uri">The URI with the C#/XAML syntax.</param>
        /// <param name="elementThatARelativeUriIsRelativeTo">This is an optional parameter that is helpful
        /// in case of relative URI's. In fact, when an URI is relative, we need to transform it into an
        /// absolute URI in order to locate the resources. A relative URI is usually relative to the place
        /// where the .XAML file is located. For example, when using an Image control, relative image paths
        /// are relative to the location of the .XAML file that contains the Image control.</param>
        /// <returns>The URI suitable to use in the HTML5 "src" property</returns>
        public static string ConvertToHtml5Path(string uri, UIElement elementThatARelativeUriIsRelativeTo = null)
        {
            if (uri == null)
            {
                return null;
            }

            var originalStringLowercase = uri.ToLower();

            if (originalStringLowercase.StartsWith(@"ms-appx:/"))
            {
                //----------------
                // This is the WinRT/UWP syntax for files in the app package.
                //----------------

                string html5Path;

                // Remove "ms-appx:/", "ms-appx://", and "ms-appx:///":
                html5Path = uri.Replace("://", ":/").Replace("://", ":/").Replace("://", ":/").Substring(9); // Note: We use "Substring" instead of "String.Replace" to remove "ms-appx:/" in order to support any case (lowercase/uppercase).

                // Fix slashes:
                html5Path = html5Path.Replace('\\', '/');

                // If the path does not contain an assembly name, we need to add it:
                if (!DoesPathContainAssemblyName("/" + html5Path))
                {
                    // We are supposed to know the startup assembly (it is set by the constructor of the "Application" class):
                    string startupAssemblyShortName = StartupAssemblyInfo.StartupAssemblyShortName;
                    if (!string.IsNullOrEmpty(startupAssemblyShortName))
                    {
                        html5Path = startupAssemblyShortName + "/" + html5Path.ToLower();
                    }
                }

                // Get the relative path where the resources are located (such as "Resources/"), and ensure that it ends with "/":
                string outputResourcesPath = StartupAssemblyInfo.OutputResourcesPath.Replace('\\', '/'); // Note: this is populated at the startup of the application (cf. "codeToPutInTheInitializeComponentOfTheApplicationClass" in the "Compiler" project)
                if (!outputResourcesPath.EndsWith("/") && outputResourcesPath != "")
                    outputResourcesPath = outputResourcesPath + '/';

                // Add the above relative path to the beginning of the path:
                html5Path = outputResourcesPath + html5Path;

#if !CSHTML5NETSTANDARD
                // Support running in the simulator (note: the following method is not translated to JavaScript, so it only runs in the Simulator):
                html5Path = GetAbsolutePathIfRunningInCSharp(html5Path);
#endif

                return html5Path;
            }
            else if (originalStringLowercase.StartsWith(@"http://") || originalStringLowercase.StartsWith(@"https://"))
            {
                //----------------
                // This is the syntax for online resources.
                //----------------

                return uri;
            }
            else if (uri.Contains(@";component/"))
            {
                //----------------
                // This is the Silverlight/WPF syntax for files in the app package (absolute paths).
                //----------------

                string componentKeyword = @";component/";
                int indexOfComponentKeyword = uri.IndexOf(componentKeyword);
                string assemblyName = uri.Substring(0, indexOfComponentKeyword);
                assemblyName = TrimStartChars(TrimStartChars(assemblyName, '/'), '\\');
                assemblyName = TrimEndChars(TrimEndChars(assemblyName, '/'), '\\');
                string relativeFolderAndFileName = uri.Substring(indexOfComponentKeyword + componentKeyword.Length);
                relativeFolderAndFileName = TrimStartChars(TrimStartChars(relativeFolderAndFileName, '/'), '\\');

                // Get the relative path where the resources are located (such as "Resources/"), and ensure that it ends with "/":
                string outputResourcesPath = StartupAssemblyInfo.OutputResourcesPath.Replace('\\', '/'); // Note: this is populated at the startup of the application (cf. "codeToPutInTheInitializeComponentOfTheApplicationClass" in the "Compiler" project)
                if (!outputResourcesPath.EndsWith("/") && outputResourcesPath != "")
                    outputResourcesPath = outputResourcesPath + '/';

                // Combine the path:
                string html5Path = outputResourcesPath + assemblyName + "/" + relativeFolderAndFileName.ToLower();

#if !CSHTML5NETSTANDARD
                // Support running in the simulator:
                html5Path = GetAbsolutePathIfRunningInCSharp(html5Path);
#endif

                return html5Path;
            }
            else
            {
                //----------------
                // The path is a relative path. We convert it to an absolute path, and call this very method again.
                //----------------

                // Try to determine the location of the .XAML file so as to transform the relative path into an absolute path:
                string xamlSourcePath = null;
                if (elementThatARelativeUriIsRelativeTo != null
                    && INTERNAL_VisualTreeManager.IsElementInVisualTree(elementThatARelativeUriIsRelativeTo)
                    && TryGetLocationOfXamlFile(elementThatARelativeUriIsRelativeTo, out xamlSourcePath)
                    && xamlSourcePath.Contains(@"\"))
                {
                    // Note: the "XamlSourcePath" is always in the following format: AssemblyName\Folder1\Folder2\FileName.xaml

                    // Fix the slashes:
                    uri = uri.Replace('\\', '/');
                    xamlSourcePath = xamlSourcePath.Replace('\\', '/');

                    // Remove the filename from the "XamlSourcePath":
                    string xamlSourcePathWithoutFileName = xamlSourcePath.Substring(0, xamlSourcePath.LastIndexOf('/'));

                    // Remove the assembly name from the "XamlSourcePath", so as to keep only the folder:
                    string assemblyName;
                    string folderWhereXamlFileIsLoated_PossibleEmpty;
                    if (xamlSourcePathWithoutFileName.Contains('/'))
                    {
                        assemblyName = xamlSourcePathWithoutFileName.Substring(0, xamlSourcePathWithoutFileName.IndexOf('/'));
                        folderWhereXamlFileIsLoated_PossibleEmpty = xamlSourcePathWithoutFileName.Substring(assemblyName.Length + 1);
                    }
                    else
                    {
                        assemblyName = xamlSourcePathWithoutFileName;
                        folderWhereXamlFileIsLoated_PossibleEmpty = "";
                    }

                    // Handle ".." in a way that, if we reach the root, we ignore any additional ".." (this is the same behavior as in Silverlight):
                    while (uri.StartsWith("../"))
                    {
                        // Remove the "../":
                        uri = uri.Substring(3);

                        // Remove the last folder (if any) in the xamlSourcePath:
                        if (folderWhereXamlFileIsLoated_PossibleEmpty.Contains('/'))
                        {
                            folderWhereXamlFileIsLoated_PossibleEmpty = folderWhereXamlFileIsLoated_PossibleEmpty.Substring(0, folderWhereXamlFileIsLoated_PossibleEmpty.LastIndexOf('/'));
                        }
                        else
                        {
                            folderWhereXamlFileIsLoated_PossibleEmpty = "";
                        }
                    }

                    // Merge the path of the .XAML file with the relative URI specified as parameter of this method:
                    string absolutePath = "ms-appx:/" + assemblyName + (folderWhereXamlFileIsLoated_PossibleEmpty != "" ? "/" + folderWhereXamlFileIsLoated_PossibleEmpty : "") + (!uri.StartsWith("/") ? "/" : "") + uri;

                    // Call again this very method (re-entrance), but this time pass the absolute path instead of the relative path:
                    string result = ConvertToHtml5Path(absolutePath, null);
                    return result;
                }
                else
                {
                    throw new Exception(@"Unless you specify the URI in XAML, the current version only supports absolute URIs that start with http:// or https:// or that are in the form of ""ms-appx:///AssemblyName/Folder/FileName"" or ""/AssemblyName;component/Folder/FileName"""
                            + (!originalStringLowercase.Contains(":") ? "    - Try adding  ms-appx:///YourAssemblyName/  to the beginning of your path." : string.Empty));
                }
            }
        }

        private static bool TryGetLocationOfXamlFile(UIElement element, out string xamlSourcePath)
        {
            // Walk up the visual tree until we find the root of the .XAML file (if any):
            UIElement current = element;
            while (current != null)
            {
                if (!string.IsNullOrEmpty(current.XamlSourcePath))
                {
                    xamlSourcePath = current.XamlSourcePath;
                    return true;
                }
                else
                {
                    current = current.INTERNAL_VisualParent as UIElement;
                }
            }
            xamlSourcePath = null;
            return false;
        }

        //todo: Replace with "TrimStart" when it works in JSIL:
        static string TrimStartChars(string value, char charToRemove)
        {
            int removeLength = 0;
            for (int i = 0; i < value.Length; i++)
            {
                char let = value[i];
                if (let == charToRemove)
                {
                    removeLength++;
                }
                else
                {
                    break;
                }
            }
            if (removeLength > 0)
            {
                return value.Substring(removeLength, value.Length - removeLength);
            }
            return value;
        }

        //todo: Replace with "TrimEnd" when it works in JSIL:
        static string TrimEndChars(string value, char charToRemove)
        {
            int removeLength = 0;
            for (int i = value.Length - 1; i >= 0; i--)
            {
                char let = value[i];
                if (let == charToRemove)
                {
                    removeLength++;
                }
                else
                {
                    break;
                }
            }
            if (removeLength > 0)
            {
                return value.Substring(0, value.Length - removeLength);
            }
            return value;
        }

        static bool DoesPathContainAssemblyName(string path)
        {
#if !BRIDGE
            string pathLowercase = path.ToLowerInvariant();
            string[] listOfAssemblies = GetListOfLoadedAssemblies();
            foreach (string assemblyShortName in listOfAssemblies)
            {
                if (pathLowercase.Contains("/" + assemblyShortName.ToLowerInvariant() + "/"))
                    return true;
            }
            return false;
#else
            //BRIDGETODO :
            // verify "to lower" & "to lower invariant" doesnt change much, otherwise, implement it
            string pathLowercase = path.ToLower();
            string[] listOfAssemblies = GetListOfLoadedAssemblies();
            foreach (string assemblyShortName in listOfAssemblies)
            {
                if (pathLowercase.Contains("/" + assemblyShortName.ToLower() + "/"))
                    return true;
            }
            return false;
#endif
        }

        static string[] GetListOfLoadedAssemblies()
        {
            string[] listOfAssemblies;
            if (CSHTML5.Interop.IsRunningInTheSimulator)
            {
#if !BRIDGE
                listOfAssemblies = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetName().Name).ToArray();
#else
                listOfAssemblies = AppDomain.CurrentDomain.GetAssemblies().Select(a => INTERNAL_BridgeWorkarounds.GetAssemblyNameWithoutCallingGetNameMethod(a)).ToArray();
#endif
            }
            else
            {
#if !BRIDGE
                listOfAssemblies = JSIL.Verbatim.Expression("Object.keys(JSIL.AssemblyShortNames)");
#else
                listOfAssemblies = AppDomain.CurrentDomain.GetAssemblies().Select(a => GetAssemblyName(a.FullName)).ToArray();//Script.Write<string[]>("Object.keys(JSIL.AssemblyShortNames);");
#endif
            }
            return listOfAssemblies;
        }

        //we use this function in order to get the assembly to Bridge
        private static string GetAssemblyName(string fullName)
        {
            //syntax is "assemblyName,version,..."
            //we want assemblyName

            int tmpIndex = fullName.IndexOf(',');

            if (tmpIndex == -1)
                return fullName;
            else
                return fullName.Substring(0, tmpIndex);
        }

#if !BRIDGE
        [JSReplacement("$relativePath")]
#else
        [Template("{relativePath}")]
#endif
        static string GetAbsolutePathIfRunningInCSharp(string relativePath) //todo: test what happens if the path is very long.
        {
            // This method is skipped when translated to JavaScript (due to the attributes above).
            // Therefore it is used only in the simulator.
            // Note: in this method we sometimes use the "dynamic" keyword so that the code can be compiled also on the environments that do not have those methods.
            var assembly = (StartupAssemblyInfo.StartupAssembly ?? Assembly.GetExecutingAssembly());

            string assemblyLocation = Path.GetDirectoryName((((dynamic)assembly).Location)).Replace('/', '\\');

            if (!assemblyLocation.EndsWith("\\") && assemblyLocation != "")
                assemblyLocation = assemblyLocation + '\\';

            string outputRootPath = StartupAssemblyInfo.OutputRootPath.Replace('/', '\\');  // Note: this is populated at the startup of the application (cf. "codeToPutInTheInitializeComponentOfTheApplicationClass" in the "Compiler" project)

            if (!outputRootPath.EndsWith("\\") && outputRootPath != "")
                outputRootPath = outputRootPath + '\\';

            string outputAbsolutePath = INTERNAL_Simulator.SimulatorProxy.PathCombine(assemblyLocation, outputRootPath); // Note: previously, when the path was hard-coded, it was: Path.Combine(assemblyLocation, @"Output\");
            string finalAbsolutePath = INTERNAL_Simulator.SimulatorProxy.PathCombine(outputAbsolutePath, relativePath);
            finalAbsolutePath = @"file:///" + finalAbsolutePath.Replace('\\', '/');
            return finalAbsolutePath;
        }

#if WORKINPROGRESS
        public static Uri EnsureAbsoluteUri(string uriString)
        {
            if (uriString.ToLower().StartsWith("http://") || uriString.ToLower().StartsWith("https://"))
            {
                return new Uri(uriString, UriKind.Absolute);
            }
            else if (uriString.StartsWith("/"))
            {
                string originalString = HtmlPage.Document.DocumentUri.OriginalString;
                int firstIndexOfSlashAfterNameOfDomain = -1;
                string httpPrefix = "";
                string nameOfDomain = originalString;
                if (originalString.ToLower().StartsWith("https://"))
                {
                    httpPrefix = "https://";
                    nameOfDomain = nameOfDomain.Substring(8);
                    firstIndexOfSlashAfterNameOfDomain = nameOfDomain.IndexOf('/');
                }
                else if (originalString.ToLower().StartsWith("http://"))
                {
                    httpPrefix = "http://";
                    nameOfDomain = nameOfDomain.Substring(7);
                    firstIndexOfSlashAfterNameOfDomain = nameOfDomain.IndexOf('/');
                }
                if (firstIndexOfSlashAfterNameOfDomain > -1)
                {
                    nameOfDomain = nameOfDomain.Substring(0, firstIndexOfSlashAfterNameOfDomain);
                }
                return new Uri(httpPrefix + nameOfDomain + uriString, UriKind.Absolute);
            }
            else
            {
                string originalString = HtmlPage.Document.DocumentUri.OriginalString;
                int lastIndexOfSlash = originalString.LastIndexOf('/');
                if (lastIndexOfSlash > -1)
                {
                    if (originalString.Substring(0, lastIndexOfSlash + 1).ToLower() == "https://"
                        || originalString.Substring(0, lastIndexOfSlash + 1).ToLower() == "http://")
                    {
                        return new Uri(originalString + "/" + uriString, UriKind.Absolute);
                    }
                    else
                    {
                        string absoluteUriAsString = originalString.Substring(0, lastIndexOfSlash + 1) + uriString;
                        return new Uri(absoluteUriAsString, UriKind.Absolute);
                    }
                }
                else
                {
                    throw new Exception("Can't create an absolute URI with relative URI : " + uriString);
                }
            }
        }
#endif

        public static string GetJavaScriptCallingAssembly()
        {
            return null;

#if false //Disabled because causes IE10 to crash with "Number Expected" error in Bootstrap.Text line 1882
            string jsStackTrace = Convert.ToString(JSIL.Verbatim.Expression(@"getStackTrace()"));

            // Get all the files listed in the stack trace:
            List<string> assemblyNames = new List<string>();
            foreach (Match match in Regex.Matches(jsStackTrace, @"[^\/]*\.js\?"))
            {
                // "match" here is in the form of "filename.js?", so we need to remove the 4 last characters to get the actual file name without extension:
                string assemblyName = match.Value.Substring(0, match.Length - 4);

                assemblyNames.Add(assemblyName);
            }
            /* Example of output:
             * JSIL.Host.js?
             * JSIL.Host.js?
             * JSIL.Core.js?
             * JSIL.Core.js?
             * JSIL.Core.js?
             * JSIL.Core.js?
             * CSharpXamlForHtml5.js?
             * CSharpXamlForHtml5.js?
             * SmallTests.js?
             * SmallTests.js?
             */

            // Find out the current assembly name,
            // which is the name of the assembly
            // that contains Core:
            string currentAssemblyName = "CSharpXamlForHtml5"; //System.Reflection.Assembly.GetAssembly(typeof(INTERNAL_JSObjectReference)).GetName().Name;

            // Find out the first filename that is after the current assembly:
            bool currentAssemblyFound = false;
            foreach (string assemblyName in assemblyNames)
            {
                if (currentAssemblyFound)
                {
                    if (assemblyName != currentAssemblyName)
                    {
                        return assemblyName;
                    }
                }
                else
                {
                    if (assemblyName == currentAssemblyName)
                    {
                        currentAssemblyFound = true;
                    }
                }
            }

            // If not found, return an empty string:
            return string.Empty;
#endif
        }
    }
}
