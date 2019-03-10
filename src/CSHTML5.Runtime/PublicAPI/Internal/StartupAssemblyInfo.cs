
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
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
