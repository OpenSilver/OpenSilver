
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



