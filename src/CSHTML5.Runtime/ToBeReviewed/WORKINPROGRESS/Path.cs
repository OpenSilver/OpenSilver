
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