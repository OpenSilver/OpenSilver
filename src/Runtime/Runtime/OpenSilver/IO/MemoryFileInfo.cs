
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System.IO;

namespace OpenSilver.IO
{
    /// <summary>
    /// This is an alternative to having a <see cref="FileInfo"/> in the browser.
    /// To accomodate for file system restrictions in the browser, this behaves like an in-memory file.
    /// 
    /// Important: This must follow the System.IO.FileInfo class interface.
    /// Then no compilation errors are expected when migrating from Silverlight.
    /// </summary>
    public class MemoryFileInfo
    {
        public override string ToString()
        {
            return Name;
        }

        string _extension = null;
        public string Extension
        {
            get
            {
                if (_extension == null)
                    _extension = Path.GetExtension(Name);
                return _extension;
            }
        }

        [OpenSilver.NotImplemented]
        public bool Exists { get; }

        [OpenSilver.NotImplemented]
        public FileAttributes Attributes { get; set; }

        [OpenSilver.NotImplemented]
        public virtual string FullName { get { return Name; } }

        private byte[] _content;

        public MemoryFileInfo(string filename, byte[] content)
        {
            Name = filename;
            _content = content;
        }

        public string Name { get; }

        public Stream OpenRead()
        {
            MemoryStream memoryStream = new MemoryStream();
            memoryStream.Write(_content, 0, _content.Length);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }
    }
}
