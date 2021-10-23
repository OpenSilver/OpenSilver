

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
        private static string _startupAssemblyShortName;
        internal static Assembly StartupAssembly
        {
            get
            {
                return _startupAssembly;
            }
            set
            {
                _startupAssembly = value;

                // Remember the short name of the assembly:
                string name = _startupAssembly.FullName; // Note: we do not call "GetName().Name" because it does not appear to work well with JSIL.
                int i = name.IndexOf(',');
                if (i > -1)
                    name = name.Substring(0, i);
                _startupAssemblyShortName = name;
            }
        }

        public static string StartupAssemblyShortName
        {
            get
            {
                return _startupAssemblyShortName;
            }
        }

        public static string OutputRootPath; // This is populated at the startup of the application (cf. "codeToPutInTheInitializeComponentOfTheApplicationClass" in the "Compiler" project)
        public static string OutputAppFilesPath; // This is populated at the startup of the application (cf. "codeToPutInTheInitializeComponentOfTheApplicationClass" in the "Compiler" project)
        public static string OutputLibrariesPath; // This is populated at the startup of the application (cf. "codeToPutInTheInitializeComponentOfTheApplicationClass" in the "Compiler" project)
        public static string OutputResourcesPath; // This is populated at the startup of the application (cf. "codeToPutInTheInitializeComponentOfTheApplicationClass" in the "Compiler" project)
    }
}
