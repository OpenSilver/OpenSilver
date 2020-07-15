using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetForHtml5.MigrationWizard
{
    public class FolderWalker
    {
        private Dictionary<string, FolderWalker> _folderNameToFolder = new Dictionary<string, FolderWalker>();
        private Dictionary<string, FileWalker> _fileNameToFile = new Dictionary<string, FileWalker>();

        public FolderWalker(string name, string fullPath)
        {
            _fullPath = fullPath;
            _name = name;
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _fullPath;
        public string FullPath
        {
            get { return _fullPath; }
            set { _fullPath = value; }
        }

        /// <summary>
        /// Adds a subfolder to this folder and returns it. If the subfolder already exists, it is returned.
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        internal FolderWalker GetSubFolderOrCreateIfNotExists(string folderName, bool raiseExceptionIfFolderAlreadyExists = false)
        {
            if (!_folderNameToFolder.ContainsKey(folderName))
            {
                string folderFullPath = Path.Combine(_fullPath, folderName);
                if (!folderFullPath.EndsWith("\\"))
                    folderFullPath = folderFullPath + "\\";
                FolderWalker folder = new FolderWalker(folderName, folderFullPath);
                _folderNameToFolder.Add(folderName, folder);
                return folder;
            }
            else
            {
                if (raiseExceptionIfFolderAlreadyExists)
                    throw new Exception(string.Format("A folder with the name '{0}' already exists.", folderName));

                return _folderNameToFolder[folderName];
            }
        }

        /// <summary>
        /// Adds a file to this folder and returns it. If the file already exists, it is returned.
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        internal FileWalker GetFileOrCreateIfNotExists(string fileName, object userState, bool raiseExceptionIfFileAlreadyExists = false)
        {
            if (!_fileNameToFile.ContainsKey(fileName))
            {
                FileWalker file = new FileWalker(fileName, Path.Combine(_fullPath, fileName), userState);
                _fileNameToFile.Add(fileName, file);
                return file;
            }
            else
            {
                if (raiseExceptionIfFileAlreadyExists)
                    throw new Exception(string.Format("A file with the name '{0}' already exists.", fileName));

                return _fileNameToFile[fileName];
            }
        }

        public IEnumerable<FileWalker> GetFiles(bool sortByName = false)
        {
            IEnumerable<FileWalker> files = _fileNameToFile.Values;

            if (sortByName)
            {
                files = files.OrderBy(fileWalker => fileWalker.Name);
            }

            return files;
        }

        public IEnumerable<FolderWalker> GetFolders(bool sortByName = false)
        {
            IEnumerable<FolderWalker> folders = _folderNameToFolder.Values;

            if (sortByName)
            {
                folders = folders.OrderBy(folderWalker => folderWalker.Name);
            }

            return folders;
        }
    }
}
