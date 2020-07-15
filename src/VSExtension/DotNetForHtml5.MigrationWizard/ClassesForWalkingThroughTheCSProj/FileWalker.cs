using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetForHtml5.MigrationWizard
{
    /// <summary>
    /// Not really a walker but it makes the name consistent with FolderWalker and FileTreeWalker
    /// </summary>
    public class FileWalker
    {
        public FileWalker(string name, string fullPath, object userState)
        {
            _name = name;
            _fullPath = fullPath;
            _userState = userState;
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

        private object _userState;
        public object UserState
        {
            get { return _userState; }
            set { _userState = value; }
        }
    }
}
