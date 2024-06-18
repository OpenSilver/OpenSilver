

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


using System.Reflection;

namespace CSHTML5.Internal
{
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

        public static string OutputRootPath; // This is populated at the startup of the application (cf. "codeToPutInTheInitializeComponentOfTheApplicationClass" in the "Compiler" project)
        public static string OutputAppFilesPath; // This is populated at the startup of the application (cf. "codeToPutInTheInitializeComponentOfTheApplicationClass" in the "Compiler" project)
        public static string OutputLibrariesPath; // This is populated at the startup of the application (cf. "codeToPutInTheInitializeComponentOfTheApplicationClass" in the "Compiler" project)
        public static string OutputResourcesPath; // This is populated at the startup of the application (cf. "codeToPutInTheInitializeComponentOfTheApplicationClass" in the "Compiler" project)
    }
}
