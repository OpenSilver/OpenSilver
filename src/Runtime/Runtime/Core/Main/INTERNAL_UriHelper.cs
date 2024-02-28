﻿
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
using System.Linq;
using System.Windows.Browser;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using OpenSilver.Internal;
using System.ComponentModel;

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
            uri = uri.Trim();
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
                string assemblyName;
                string pathAfterAssemblyName;
                if (!DoesPathContainAssemblyName("/" + html5Path, out assemblyName, out pathAfterAssemblyName))
                {
                    // We are supposed to know the startup assembly (it is set by the constructor of the "Application" class):
                    string startupAssemblyShortName = StartupAssemblyInfo.StartupAssemblyShortName;
                    if (!string.IsNullOrEmpty(startupAssemblyShortName))
                    {
                        html5Path = startupAssemblyShortName + "/" + html5Path.ToLower();
                    }
                }
                else
                {
                    // Make sure the portion of the path AFTER the assembly name is lowercase:
                    html5Path = assemblyName + "/" + pathAfterAssemblyName.ToLower();
                }

                // Get the relative path where the resources are located (such as "Resources/"), and ensure that it ends with "/":
                string outputResourcesPath = StartupAssemblyInfo.OutputResourcesPath.Replace('\\', '/'); // Note: this is populated at the startup of the application (cf. "codeToPutInTheInitializeComponentOfTheApplicationClass" in the "Compiler" project)
                if (!outputResourcesPath.EndsWith("/") && outputResourcesPath != "")
                    outputResourcesPath = outputResourcesPath + '/';

                // Add the above relative path to the beginning of the path:
                html5Path = outputResourcesPath + html5Path;

                return html5Path;
            }
            else if (originalStringLowercase.StartsWith(@"http://") || originalStringLowercase.StartsWith(@"https://"))
            {
                //----------------
                // This is the syntax for online resources.
                //----------------

                return uri;
            }
            else if (originalStringLowercase.StartsWith(@"pack://application:,,,/"))
            {
                // https://docs.microsoft.com/en-us/dotnet/framework/wpf/app-development/pack-uris-in-wpf
                // Note that the pack URI syntax for referenced assembly resource files can be used only with the application:/// authority. 
                // For example, the following is not supported in WPF.
                // pack://siteoforigin:,,,/SomeAssembly;component/ResourceFile.xaml

                string html5Path = uri.Substring(23);

                // If the path does not contain an assembly name, we need to add it:
                string assemblyName;
                string pathAfterAssemblyName;
                if (!DoesPathContainAssemblyName("/" + html5Path, out assemblyName, out pathAfterAssemblyName))
                {
                    // We are supposed to know the startup assembly (it is set by the constructor of the "Application" class):
                    string startupAssemblyShortName = StartupAssemblyInfo.StartupAssemblyShortName;
                    if (!string.IsNullOrEmpty(startupAssemblyShortName))
                    {
                        html5Path = startupAssemblyShortName + "/" + html5Path.ToLower();
                    }
                }
                else
                {
                    // Make sure the portion of the path AFTER the assembly name is lowercase:
                    html5Path = assemblyName + "/" + pathAfterAssemblyName.ToLower();
                }

                // Get the relative path where the resources are located (such as "Resources/"), and ensure that it ends with "/":
                string outputResourcesPath = StartupAssemblyInfo.OutputResourcesPath.Replace('\\', '/'); // Note: this is populated at the startup of the application (cf. "codeToPutInTheInitializeComponentOfTheApplicationClass" in the "Compiler" project)
                if (!outputResourcesPath.EndsWith("/") && outputResourcesPath != "")
                    outputResourcesPath = outputResourcesPath + '/';

                // Add the above relative path to the beginning of the path:
                html5Path = outputResourcesPath + html5Path;

                return html5Path;
            }
            else if (originalStringLowercase.Contains(@";component/"))
            {
                //----------------
                // This is the Silverlight/WPF syntax for files in the app package (absolute paths).
                //----------------

                string componentKeyword = @";component/";
                int indexOfComponentKeyword = originalStringLowercase.IndexOf(componentKeyword); //We use the lowercase version of the uri to get the index, so "Component" can work. The index will still be the same.
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

                    string absolutePath;

                    // If the Uri starts with "/", it means that it refers to the root of the assembly, otherwise it is relative to the XAML where it is used (if any):
                    if (uri.StartsWith("/"))
                    {
                        //================
                        // The path is relative to the root of the assembly:
                        //================

                        // Merge the path and the assembly name to obtain an absolute path:
                        absolutePath = "ms-appx:/" + assemblyName + uri;
                    }
                    else
                    {
                        //================
                        // The path is relative to the XAML where it is used (if any)
                        //================

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
                        absolutePath = "ms-appx:/" + assemblyName + (folderWhereXamlFileIsLoated_PossibleEmpty != "" ? "/" + folderWhereXamlFileIsLoated_PossibleEmpty : "") + (!uri.StartsWith("/") ? "/" : "") + uri;
                    }

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
                    current = VisualTreeHelper.GetParent(current) as UIElement ?? (current as FrameworkElement)?.Parent as Popup;
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

        static bool DoesPathContainAssemblyName(string path, out string assemblyName, out string pathAfterAssemblyName)
        {
            //BRIDGETODO: verify "to lower" & "to lower invariant" doesnt change much, otherwise, implement it
            string pathLowercase = path.ToLower();
            string[] listOfAssemblies = GetListOfLoadedAssemblies();
            foreach (string assemblyShortName in listOfAssemblies)
            {
                if (pathLowercase.Contains("/" + assemblyShortName.ToLower() + "/"))
                {
                    assemblyName = assemblyShortName; // Note: here we deliberately do not call "ToLower()".
                    pathAfterAssemblyName = pathLowercase.Substring(assemblyName.Length + 2); // + 2 because we want to remove "/assemblyName/"
                    return true;
                }
            }
            assemblyName = null;
            pathAfterAssemblyName = null;
            return false;
        }

        static string[] GetListOfLoadedAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetName().Name).ToArray();
        }

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

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static string GetJavaScriptCallingAssembly()
        {
            return null;
        }
    }
}
