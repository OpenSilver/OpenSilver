// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;

namespace OpenSilver.Compiler;

/// <summary>
/// TaskHelper which implements some helper methods.
/// </summary>
internal static class TaskHelper
{
    // <summary>
    // Create the full file path with the right root path
    // </summary>
    // <param name="thePath">The original file path</param>
    // <param name="rootPath">The root path</param>
    // <returns>The new fullpath</returns>
    internal static string CreateFullFilePath(string thePath, string rootPath)
    {
        // make it an absolute path if not already so
        if (!Path.IsPathRooted(thePath))
        {
            thePath = rootPath + thePath;
        }

        // get rid of '..' and '.' if any
        thePath = Path.GetFullPath(thePath);
        return thePath;
    }

    // <summary>
    // This helper returns the "relative" portion of a path
    // to a given "root"
    // - both paths need to be rooted
    // - if no match > return empty string
    // E.g.: path1 = C:\foo\bar\
    //       path2 = C:\foo\bar\baz
    //
    //       return value = "baz"
    // </summary>
    internal static string GetRootRelativePath(string path1, string path2)
    {
        string relPath = "";
        string fullpath1;
        string fullpath2;

        string sourceDir = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar;

        // make sure path1 and Path2 are both full path
        // so that they can be compared on right base.

        fullpath1 = CreateFullFilePath(path1, sourceDir);
        fullpath2 = CreateFullFilePath(path2, sourceDir);

        if (fullpath2.StartsWith(fullpath1, StringComparison.OrdinalIgnoreCase))
        {
            relPath = fullpath2.Substring(fullpath1.Length);
        }

        return relPath;
    }
}
