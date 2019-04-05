
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
    public static partial class CSHTML5_File
    {
        public static bool Exists(String path)
        {
            throw new NotImplementedException("File error - Exists - args: " + path);
        }

        public static void Copy(String sourceFileName, String destFileName)
        {
            throw new NotImplementedException("File error - Copy - args: " + sourceFileName + " " + sourceFileName);
        }

        public static void Copy(String sourceFileName, String destFileName, bool overwrite)
        {
            throw new NotImplementedException("File error - Copy - args: " + sourceFileName + " " + sourceFileName + (overwrite?"   True" : "    False") );
        }

        public static void Delete(String path)
        {
            throw new NotImplementedException("File error - Delete - args: " + path);
        }

        public static void Move(String sourceFileName, String destFileName)
        {
            throw new NotImplementedException("File error - Move - args: " + sourceFileName + " " + sourceFileName);
        }

        public static FileStream Open(String path, FileMode mode)
        {
            return Open(path, mode, (mode == FileMode.Append ? FileAccess.Write : FileAccess.ReadWrite), FileShare.None);
        }

        public static FileStream Open(String path, FileMode mode, FileAccess access)
        {
            return Open(path, mode, access, FileShare.None);
        }

        public static FileStream Open(String path, FileMode mode, FileAccess access, FileShare share)
        {
            throw new NotImplementedException("File error - Open - args: " + path + " " + mode + " " + access + " " + share);
            // return new File.OpenRead(path);
        }

        public static FileStream Create(String path)
        {
            throw new NotImplementedException("File error - Open - args: " + path);
            /*
            return Create(path, FileStream.DefaultBufferSize);
            */
        }

        public static FileStream Create(String path, int bufferSize)
        {
            throw new NotImplementedException("File error - Open - args: " + path + " " + bufferSize);
            /*
            return new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None, bufferSize);
            */
        }

        public static FileStream Create(String path, int bufferSize, FileOptions options)
        {
            throw new NotImplementedException("File error - Open - args: " + path + " " + bufferSize + " " + options);
            /*
            return new FileStream(path, FileMode.Create, FileAccess.ReadWrite,
                                  FileShare.None, bufferSize, options);
            */
        }
    }
}



