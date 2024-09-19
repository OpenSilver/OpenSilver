
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
using System.ComponentModel;
using System.Reflection;
using OpenSilver.Internal;

namespace CSHTML5.Internal;

public static class StartupAssemblyInfo
{
    private static Assembly _startupAssembly;

    internal static Assembly StartupAssembly
    {
        get => _startupAssembly;
        set
        {
            _startupAssembly = value;
            StartupAssemblyShortName = value?.GetName().Name;
        }
    }

    public static string StartupAssemblyShortName { get; private set; }

    // This is populated at the startup of the application // (cf. "codeToPutInTheInitializeComponentOfTheApplicationClass" in the "Compiler" project)
    [Obsolete(Helper.ObsoleteMemberMessage)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string OutputRootPath;

    [Obsolete(Helper.ObsoleteMemberMessage)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string OutputAppFilesPath;

    [Obsolete(Helper.ObsoleteMemberMessage)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string OutputLibrariesPath;

    public static string OutputResourcesPath = "resources/";
}
