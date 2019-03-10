using CSHTML5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.IO.IsolatedStorage
{
    public class IsolatedStorageFile : IDisposable
    {
        string _assemblyName;
        public static IsolatedStorageFile GetUserStoreForAssembly()
        {
            return new IsolatedStorageFile() { _assemblyName = System.Reflection.Assembly.GetExecutingAssembly().FullName };
        }

        public IsolatedStorageFileStream CreateFile(string fileName)
        {
            return new IsolatedStorageFileStream(GetFilePath(_assemblyName, fileName), FileMode.Create);
        }

        public void DeleteFile(string fileName)
        {
            string filePath_lower = GetFilePath(_assemblyName, fileName).ToLower();
            Interop.ExecuteJavaScript("localStorage.removeItem({0})", filePath_lower + "ǀǀCaseSensitivePath");
            Interop.ExecuteJavaScript("localStorage.removeItem({0})", filePath_lower);
        }

        public bool FileExists(string fileName)
        {
            string filePath_lower = GetFilePath(_assemblyName, fileName).ToLower();
            return Convert.ToBoolean(Interop.ExecuteJavaScript("localStorage.getItem({0}) != null", filePath_lower));
        }

        public IsolatedStorageFileStream OpenFile(string fileName, FileMode fileMode)
        {
            return new IsolatedStorageFileStream(GetFilePath(_assemblyName, fileName), FileMode.Open);
        }

        public void Dispose()
        {
            //do nothing?
        }

        /// <summary>
        /// This method is here just to have the definition of the file path in a single place.
        /// </summary>
        /// <param name="fileName">The path to the file</param>
        /// <returns>a string as follows: &quot;storage_ASSEMBLYNAME/fileName&quot;</returns>
        [Bridge.Template("(\"storage_\" + {assemblyName} + \"/\" + {fileName})")] //Just for perf-purposes.
        private static string GetFilePath(string assemblyName, string fileName)
        {
            return "storage_" + assemblyName + "/" + fileName;
        }
    }
}
