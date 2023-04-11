
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
using System.IO;
using System.Linq;
using System.Reflection;

namespace OpenSilver
{
    public static class PathsHelper
    {
        public static string GetProgramFilesX86Path()
        {
            // Credits: http://stackoverflow.com/questions/194157/c-sharp-how-to-get-program-files-x86-on-windows-vista-64-bit

            if (8 == IntPtr.Size
                || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
            {
                return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            }

            return Environment.GetEnvironmentVariable("ProgramFiles");
        }

        public static string GetCshtml5RootPath()
        {
            return Path.Combine(GetProgramFilesX86Path(), @"MSBuild\CSharpXamlForHtml5");
        }

        public static string GetActivationAppPath()
        {
            return Path.Combine(GetCshtml5RootPath(), @"InternalStuff\Activation\CSharpXamlForHtml5.Activation.exe");
        }

        public static string GetCompilerPath()
        {
            return Path.Combine(GetCshtml5RootPath(), @"InternalStuff\Compiler");
        }

        public static string GetLibrariesPath()
        {
            return Path.Combine(GetCshtml5RootPath(), @"InternalStuff\Libraries");
        }

        public static string GetPathOfAssembly(Assembly assembly)
        {
            // cf. http://stackoverflow.com/questions/52797/how-do-i-get-the-path-of-the-assembly-the-code-is-in
            return Uri.UnescapeDataString(new UriBuilder(assembly.CodeBase).Path);
        }

        /// <summary>
        /// This method processes a path to ensure that it contains no forward slash and that it ends with a backslash.
        /// </summary>
        /// <param name="path">The path to process</param>
        /// <returns>The resulting path</returns>
        public static string EnsureNoForwardSlashAndEnsureItEndsWithABackslash(string path)
        {
            // Replace any forward slash with a backslash:
            path = path.Replace('/', '\\');

            // Make sure that the path ends with "\":
            if (!path.EndsWith(@"\") && path != "")
                path = path + @"\";

            return path;
        }

        /// <summary>
        /// This method processes a path to ensure that it contains no backslash and that it ends with a forward backslash.
        /// </summary>
        /// <param name="path">The path to process</param>
        /// <returns>The resulting path</returns>
        public static string EnsureNoBackslashAndEnsureItEndsWithAForwardSlash(string path)
        {
            // Replace any forward slash with a backslash:
            path = path.Replace('\\', '/');

            // Make sure that the path ends with "/":
            if (!path.EndsWith("/") && path != "")
                path = path + "/";

            return path;
        }

        /// <summary>
        /// This method combines two paths while ensuring that no forward slash is used and that the combined path ends with a backslash.
        /// </summary>
        /// <param name="path1">First path</param>
        /// <param name="path2">Second path</param>
        /// <returns>The combined path</returns>
        public static string CombinePathsWhileEnsuringEndingBackslashAndMore(string path1, string path2)
        {
            var separator = Path.DirectorySeparatorChar;

            // Replace any incorrect path separators with the correct one:
            path1 = path1.Replace('/', separator).Replace('\\', separator);
            path2 = path2.Replace('/', separator).Replace('\\', separator);

            // Combine the paths:
            var combinedPath = Path.Combine(path1, path2);

            // Make sure that the combined path ends with the correct separator:
            if (!combinedPath.EndsWith(separator.ToString()))
                combinedPath += separator;

            return combinedPath;
        }

        /// <summary>
        /// Returns the path of the Output folder where the HTML app is generated.
        /// </summary>
        /// <param name="assemblyFullNameAndPath">The name (including the full path) of the assembly that is being compiled to HTML/JS.</param>
        /// <param name="outputRootPath">The "OutputRootPath" as specified in the ".target" file or in the CSPROJ (for example: "Output/"). It can be absolute or relative to the assembly "bin\Debug" path.</param>
        /// <returns>The path of the Output folder where the HTML app is generated</returns>
        public static string GetOutputPathAbsolute(string assemblyFullNameAndPath, string outputRootPath)
        {
            //--------------------------
            // Note: this method is similar to the one in the Simulator.
            // IMPORTANT: If you update this method, make sure to update the other one as well.
            //--------------------------

            var separator = Path.DirectorySeparatorChar;
            var outputRootPathFixed = outputRootPath.Replace('/', separator).Replace('\\', separator);
            if (!outputRootPathFixed.EndsWith(separator.ToString()) && outputRootPathFixed != "")
                outputRootPathFixed += separator;

            // If the path is already ABSOLUTE, we return it directly, otherwise we concatenate it to the path of the assembly:
            string outputPathAbsolute;
            if (Path.IsPathRooted(outputRootPathFixed))
            {
                outputPathAbsolute = outputRootPathFixed;
            }
            else
            {
                outputPathAbsolute = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(assemblyFullNameAndPath)), outputRootPathFixed);

                outputPathAbsolute = outputPathAbsolute.Replace('/', separator).Replace('\\', separator);

                if (!outputPathAbsolute.EndsWith(separator.ToString()) && outputPathAbsolute != "")
                    outputPathAbsolute += separator;
            }

            return outputPathAbsolute;
        }

        /// <summary>
        /// This method converts a path in the end-user application (whether it is
        /// absolute or relative) into an absolute path in the form:
        /// "/AssemblyName;component/Folder/FileName.extension"
        /// </summary>
        /// <param name="uri"></param>
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

        static string RemoveFileNameFromPath(string pathWithCorrectSlashes)
        {
            if (pathWithCorrectSlashes.Contains('/'))
                return pathWithCorrectSlashes.Substring(0, pathWithCorrectSlashes.LastIndexOf('/'));
            else
                return "";
        }
    }
}
