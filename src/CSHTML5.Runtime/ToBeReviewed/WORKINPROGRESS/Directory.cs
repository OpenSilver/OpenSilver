
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
    public static class Directory
    {
        public static DirectoryInfo CreateDirectory(string path)
        {
            return new DirectoryInfo(path);
        }

        public static bool Exists(string path)
        {
            throw new NotImplementedException("Directory error - Exist - args: " + path);
        }

        public static String[] GetDirectories(String path)
        {
            throw new NotImplementedException("Directory error - GetDirectories - args: " + path);
        }

        public static String[] GetFiles(String path)
        {
            throw new NotImplementedException("Directory error - GetFiles - args: " + path);
        }
    }
}
