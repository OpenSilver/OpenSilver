

/*===================================================================================
* 
*   Copyright (c) Userware (OpenSilver.net, CSHTML5.com)
*      
*   This file is part of both the OpenSilver Compiler (https://opensilver.net), which
*   is licensed under the MIT license (https://opensource.org/licenses/MIT), and the
*   CSHTML5 Compiler (http://cshtml5.com), which is dual-licensed (MIT + commercial).
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/



using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetForHtml5.Compiler
{
    internal static class FileHelpers
    {
        public static string CreateTemporaryFolder()
        {
            string tempFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + Path.DirectorySeparatorChar);
            Directory.CreateDirectory(tempFolder);
            return tempFolder;
        }

        public static void DeleteTemporaryFolder(string tempFolder)
        {
            Directory.Delete(tempFolder, true);
        }

        public static void DeleteAllFilesAndFoldersInDirectory(string directoryPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
            foreach (System.IO.FileInfo file in directoryInfo.GetFiles()) file.Delete();
            foreach (System.IO.DirectoryInfo subDirectory in directoryInfo.GetDirectories()) subDirectory.Delete(true);
        }

        /*
        public static void DeleteAllFilesAndFoldersInDirectory(string directoryPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
            DeleteAllFilesAndFoldersInDirectory(directoryPath);
        }

        static void DeleteAllFilesAndFoldersInDirectory(DirectoryInfo directoryInfo)
        {
            foreach (System.IO.FileInfo file in directoryInfo.GetFiles())
            {
                file.Delete();
            }
            foreach (System.IO.DirectoryInfo subDirectory in directoryInfo.GetDirectories())
            {
                // Recursion:
                DeleteAllFilesAndFoldersInDirectory(subDirectory);

                // Then, delete folder:
                subDirectory.Delete(false);
            }
        }
         */
    }
}
