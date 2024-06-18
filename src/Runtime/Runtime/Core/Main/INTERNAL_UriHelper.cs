
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
using System.Windows.Browser;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.ComponentModel;
using OpenSilver.Internal;

namespace CSHTML5.Internal
{
    public static class INTERNAL_UriHelper
    {
        private static string OutputResourcesPath
        {
            get
            {
                // Note: this is populated at the startup of the application
                // (cf. "codeToPutInTheInitializeComponentOfTheApplicationClass" in the "Compiler" project)
                string path = StartupAssemblyInfo.OutputResourcesPath.Replace('\\', '/');
                if (!path.EndsWith("/") && path != string.Empty)
                {
                    path += '/';
                }

                return path;
            }
        }

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

            if (AppResourcesManager.IsComponentUri(uri))
            {
                return ConvertComponentUri(uri);
            }
            else if (AppResourcesManager.IsHttpUri(uri) || AppResourcesManager.IsHttpsUri(uri))
            {
                return uri;
            }
            else if (AppResourcesManager.IsMsAppxUri(uri))
            {
                return ConvertMsAppxUri(uri);
            }
            else if (AppResourcesManager.IsPackUri(uri))
            {
                return ConvertPackApplicationUri(uri);
            }
            else
            {
                return ConvertFromRelativePath(uri, elementThatARelativeUriIsRelativeTo);
            }
        }

        private static string ConvertComponentUri(string uri)
        {
            string assemblyName = AppResourcesManager.ExtractAssemblyNameFromComponentUri(uri).ToLowerInvariant();
            string partName = AppResourcesManager.ExtractResourcePartFromComponentUri(uri).ToLowerInvariant();

            return $"{OutputResourcesPath}{assemblyName}/{partName}";
        }

        private static string ConvertMsAppxUri(string uri)
        {
            AppResourcesManager.ExtractPartsFromMsAppxUri(uri, out string assemblyName, out string resourcePart);
            assemblyName ??= StartupAssemblyInfo.StartupAssemblyShortName;

            if (assemblyName is null)
            {
                return $"{OutputResourcesPath}{resourcePart.ToLowerInvariant()}";
            }
            else
            {
                return $"{OutputResourcesPath}{assemblyName.ToLowerInvariant()}/{resourcePart.ToLowerInvariant()}";
            }
        }

        private static string ConvertPackApplicationUri(string uri)
        {
            // https://docs.microsoft.com/en-us/dotnet/framework/wpf/app-development/pack-uris-in-wpf
            // Note that the pack URI syntax for referenced assembly resource files can be used only with the application:/// authority. 
            // For example, the following is not supported in WPF.
            // pack://siteoforigin:,,,/SomeAssembly;component/ResourceFile.xaml

            AppResourcesManager.ExtractPartsFromPackUri(uri, out string assemblyName, out string resourcePart);
            assemblyName ??= StartupAssemblyInfo.StartupAssemblyShortName;

            if (assemblyName is null)
            {
                return $"{OutputResourcesPath}{resourcePart.ToLowerInvariant()}";
            }
            else
            {
                return $"{OutputResourcesPath}{assemblyName.ToLowerInvariant()}/{resourcePart.ToLowerInvariant()}";
            }
        }

        private static string ConvertFromRelativePath(string uri, UIElement relativeTo)
        {
            if (!TryGetLocationOfXamlFile(relativeTo, out string xamlSourcePath))
            {
                throw InvalidUriException(uri);
            }

            // Note: the "XamlSourcePath" is always in the following format: AssemblyName\Folder1\Folder2\FileName.xaml

            int index1 = xamlSourcePath.AsSpan().LastIndexOfAny('/', '\\');
            if (index1 == -1)
            {
                throw InvalidUriException(uri);
            }

            // Remove file name
            ReadOnlySpan<char> basePath = xamlSourcePath.AsSpan(0, index1);
            ReadOnlySpan<char> xamlFilePath;

            string assemblyName;

            int index2 = basePath.IndexOfAny('\\', '/');
            if (index2 != -1)
            {
                assemblyName = basePath.Slice(0, index2).ToString();
                xamlFilePath = basePath.Slice(index2 + 1);
            }
            else
            {
                assemblyName = basePath.ToString();
                xamlFilePath = ReadOnlySpan<char>.Empty;
            }

            // If the Uri starts with "/", it means that it refers to the root of the assembly,
            // otherwise it is relative to the XAML where it is used (if any):
            if (uri.StartsWith("/") || uri.StartsWith("\\"))
            {
                return ConvertComponentUri($"/{assemblyName}{AppResourcesManager.Component}{uri.Substring(1)}");
            }

            //================
            // The path is relative to the XAML where it is used (if any)
            //================

            ReadOnlySpan<char> path = uri.AsSpan();

            // Handle ".." in a way that, if we reach the root, we ignore any additional ".." (this is the same behavior as in Silverlight)
            while (StartsWithCollapseSeparator(path))
            {
                // Remove the "../"
                path = path.Slice(3);

                // Remove the last folder (if any) in the xamlSourcePath
                int i = xamlFilePath.LastIndexOfAny('/', '\\');
                if (i != -1)
                {
                    xamlFilePath = xamlFilePath.Slice(0, i);
                }
                else
                {
                    xamlFilePath = ReadOnlySpan<char>.Empty;
                }
            }

            string separator = (path.StartsWith("/".AsSpan()) || path.StartsWith("\\".AsSpan())) ? string.Empty : "/";

            // Merge the path of the .XAML file with the relative URI specified as parameter of this method:
            return ConvertComponentUri($"/{assemblyName}{AppResourcesManager.Component}{xamlFilePath.ToString()}{separator}{path.ToString()}");

            static bool TryGetLocationOfXamlFile(UIElement element, out string xamlSourcePath)
            {
                UIElement current = element;

                while (current is not null)
                {
                    if (!string.IsNullOrEmpty(current.XamlSourcePath))
                    {
                        xamlSourcePath = current.XamlSourcePath;
                        return true;
                    }

                    current = VisualTreeHelper.GetParent(current) as UIElement ?? (current as FrameworkElement)?.Parent as Popup;
                }

                xamlSourcePath = null;
                return false;
            }

            static bool StartsWithCollapseSeparator(ReadOnlySpan<char> input)
            {
                if (input.Length < 3)
                {
                    return false;
                }

                return input[0] == '.' && input[1] == '.' && (input[2] == '/' || input[2] == '\\');
            }

            static Exception InvalidUriException(string uri)
            {
                return new Exception(
                    @"Unless you specify the URI in XAML, the current version only supports absolute URIs that start with http:// or https:// or that are in the form of ""/AssemblyName;component/Folder/FileName"" or ""ms-appx:///AssemblyName/Folder/FileName"""
                    + (!uri.Contains(":") ? @" - Try adding ""ms-appx:///YourAssemblyName/"" to the beginning of your path." : string.Empty));
            }
        }

        public static Uri EnsureAbsoluteUri(string uriString)
        {
            if (AppResourcesManager.IsHttpUri(uriString) || AppResourcesManager.IsHttpsUri(uriString))
            {
                return new Uri(uriString, UriKind.Absolute);
            }
            else if (uriString.StartsWith("/"))
            {
                string originalString = HtmlPage.Document.DocumentUri.OriginalString;
                int firstIndexOfSlashAfterNameOfDomain = -1;
                string httpPrefix = string.Empty;
                string nameOfDomain = originalString;
                if (AppResourcesManager.IsHttpsUri(originalString))
                {
                    httpPrefix = AppResourcesManager.Https;
                    nameOfDomain = nameOfDomain.Substring(AppResourcesManager.Https.Length);
                    firstIndexOfSlashAfterNameOfDomain = nameOfDomain.IndexOf('/');
                }
                else if (AppResourcesManager.IsHttpUri(originalString))
                {
                    httpPrefix = AppResourcesManager.Http;
                    nameOfDomain = nameOfDomain.Substring(AppResourcesManager.Http.Length);
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
                    if (AppResourcesManager.IsHttpsUri(originalString) || AppResourcesManager.IsHttpUri(originalString))
                    {
                        return new Uri(originalString + "/" + uriString, UriKind.Absolute);
                    }
                    else
                    {
                        return new Uri(originalString.Substring(0, lastIndexOfSlash + 1) + uriString, UriKind.Absolute);
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
