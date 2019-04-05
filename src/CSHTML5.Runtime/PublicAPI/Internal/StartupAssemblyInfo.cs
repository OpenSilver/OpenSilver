
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

        internal static string StartupAssemblyShortName
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
