using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetForHtml5.MigrationWizard
{
    public class FileTreeWalker
    {
        private FolderWalker _thisRootFolder;

        public FolderWalker GetRootFolder()
        {
            return _thisRootFolder;
        }

        // Note: In the Tuple, the first item is the file full path, and the second item is any object that accompanies the file.
        public FileTreeWalker(IEnumerable<Tuple<string, object>> filesPathsAndUserState)
        {
            _thisRootFolder = new FolderWalker(".", "");

            foreach (Tuple<string, object> filePathAndUserState in filesPathsAndUserState)
            {
                string filePath = filePathAndUserState.Item1;
                object userState = filePathAndUserState.Item2;

                string normalizedFilePath = filePath.Replace('/', '\\'); //make sure we have a consistent separator between the different elements.
                string[] splittedPath = normalizedFilePath.Split('\\');
                int splittedPathLength = splittedPath.Length;

                //now we loop through the path's elements to create/go through the folders:
                FolderWalker currentFolder = _thisRootFolder;
                for (int i = 0; i < splittedPathLength - 1; ++i)
                {
                    string folderName = splittedPath[i];
                    if (!string.IsNullOrWhiteSpace(folderName))
                    {
                        currentFolder = currentFolder.GetSubFolderOrCreateIfNotExists(folderName);
                    }
                }

                if (!FoldersHelper.IsAFolder(filePath))
                {
                    //create the file.
                    currentFolder.GetFileOrCreateIfNotExists(splittedPath[splittedPathLength - 1], userState);
                }
            }
        }
    }
}
