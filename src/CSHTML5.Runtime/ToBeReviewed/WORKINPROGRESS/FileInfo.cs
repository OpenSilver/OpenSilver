
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
    public class FileInfo //: FileSystemInfo
    {
        protected String FullPath;          // fully qualified path of the directory
        protected String OriginalPath;      // path passed in by the user
        private String _displayPath = "";   // path that can be displayed to the user

        public FileInfo(string fileName)
        {
            OriginalPath = fileName;
            FullPath = fileName;
        }

        public String FullName { get { return FullPath; } }
        public String Name { get { return Path.GetFileName(FullPath); } }

        public FileStream Open(FileMode mode, FileAccess access, FileShare share)
        {
            return null;
        }

        public bool Exists
        {
            get
            {
                throw new NotImplementedException("FileInfo error - Exist - ");
            }
        }

        public String Extension
        {
            get
            {
                string[] split = FullPath.Split('.');
                if (split.Length > 0)
                    return split[split.Length - 1];
                return string.Empty;
            }
        }


    }

}