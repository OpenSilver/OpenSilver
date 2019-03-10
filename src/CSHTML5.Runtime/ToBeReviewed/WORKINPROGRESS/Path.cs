
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



namespace System.IO
{
    public static class Path
    {
        public static readonly char DirectorySeparatorChar = '\\';
        internal const string DirectorySeparatorCharAsString = "\\";

        public static readonly char AltDirectorySeparatorChar = '/';

        public static readonly char VolumeSeparatorChar = ':';

        public static string GetDirectoryName(string path)
        {

            string[] paths = path.Split('\\');

            path = "";

            if (paths.Length > 0)
                path = paths[0];

            for (int i = 1; i < paths.Length - 1; i++)
            {
                path += '\\' + paths[i];
            }

            return path;
        }

        public static string GetFileName(string path)
        {
            string[] paths = path.Split('\\');

            path = "";

            if (paths.Length > 0)
                path = paths[paths.Length- 1];

            return path;
        }
    }
}