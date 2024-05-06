
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

namespace OpenSilver.Compiler
{
    internal static class PathsHelper
    {
        /// <summary>
        /// This method converts a path in the end-user application (whether it is
        /// absolute or relative) into an absolute path in the form:
        /// "/AssemblyName;component/Folder/FileName.extension"
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="pathOfTheXamlFileWhereTheUriIsDeclared"></param>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public static string ConvertToAbsolutePathWithComponentSyntax(string uri, string pathOfTheXamlFileWhereTheUriIsDeclared, string assemblyName)
        {
            if (uri == null)
            {
                return null;
            }

            var originalStringLowercase = uri.ToLower();

            if (originalStringLowercase.Contains(@";component/"))
            {
                //----------------
                // Already good.
                //----------------

                return uri;
            }
            else if (originalStringLowercase.StartsWith(@"ms-appx:/"))
            {
                //todo
                throw new NotImplementedException(@"Unsupported URI: " + originalStringLowercase + Environment.NewLine + @"With the current version, the URI specified in the Source property must be either relative or absolute in the format ""/AssemblyName;component/Folder/FileName.extension"".");
            }
            else
            {
                //----------------
                // The path is a relative path. We convert it to an absolute path
                //----------------

                string relativeUri = uri;

                // Fix the slashes:
                relativeUri = relativeUri.Replace('\\', '/');
                pathOfTheXamlFileWhereTheUriIsDeclared = pathOfTheXamlFileWhereTheUriIsDeclared.Replace('\\', '/');

                // Remove the filename from the "pathOfTheXamlFileWhereTheUriIsDeclared":
                string xamlSourcePathWithoutFileName_PossiblyEmpty = RemoveFileNameFromPath(pathOfTheXamlFileWhereTheUriIsDeclared);

                string absolutePath;

                // If the Uri starts with "/", it means that it refers to the root of the assembly, otherwise it is relative to the XAML where it is used (if any):
                if (relativeUri.StartsWith("/"))
                {
                    //================
                    // The path is relative to the root of the assembly:
                    //================

                    // Merge the path and the assembly name to obtain an absolute path:
                    absolutePath = "/" + assemblyName + ";component" + (!relativeUri.StartsWith("/") ? "/" : "") + relativeUri;
                }
                else
                {
                    //================
                    // The path is relative to the XAML where it is used (if any)
                    //================

                    // Handle ".." in a way that, if we reach the root, we ignore any additional ".." (this is the same behavior as in Silverlight):
                    while (relativeUri.StartsWith("../"))
                    {
                        // Remove the "../":
                        relativeUri = relativeUri.Substring(3);

                        // Remove the last folder (if any) in the xamlSourcePath:
                        if (xamlSourcePathWithoutFileName_PossiblyEmpty.Contains('/'))
                        {
                            xamlSourcePathWithoutFileName_PossiblyEmpty = xamlSourcePathWithoutFileName_PossiblyEmpty.Substring(0, xamlSourcePathWithoutFileName_PossiblyEmpty.LastIndexOf('/'));
                        }
                        else
                        {
                            xamlSourcePathWithoutFileName_PossiblyEmpty = "";
                        }
                    }

                    // Merge the path of the .XAML file with the relative URI specified as parameter of this method:
                    absolutePath = "/" + assemblyName + ";component/" + xamlSourcePathWithoutFileName_PossiblyEmpty.TrimStart('/');
                    absolutePath = absolutePath.TrimEnd('/') + "/" + relativeUri.TrimStart('/');
                }

                return absolutePath;
            }
        }

        private static string RemoveFileNameFromPath(string pathWithCorrectSlashes)
        {
            if (pathWithCorrectSlashes.Contains('/'))
            {
                return pathWithCorrectSlashes.Substring(0, pathWithCorrectSlashes.LastIndexOf('/'));
            }
            
            return string.Empty;
        }
    }
}
