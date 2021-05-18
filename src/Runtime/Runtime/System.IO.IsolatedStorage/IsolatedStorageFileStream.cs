
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



using CSHTML5;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSilver.IO.IsolatedStorage
{
    public partial class IsolatedStorageFileStream : FileStream, IDisposable
    {
        internal string _filePath;
        MemoryStream _fs;

        // Summary:
        //     Initializes a new instance of an System.IO.IsolatedStorage.IsolatedStorageFileStream
        //     object giving access to the file designated by path in the specified mode.
        //
        // Parameters:
        //   path:
        //     The relative path of the file within isolated storage.
        //
        //   mode:
        //     One of the System.IO.FileMode values.
        //
        // Exceptions:
        //   System.ArgumentException:
        //     The path is badly formed.
        //
        //   System.ArgumentNullException:
        //     The path is null.
        //
        //   System.IO.DirectoryNotFoundException:
        //     The directory in path does not exist.
        //
        //   System.IO.FileNotFoundException:
        //     No file was found and the mode is set to System.IO.FileMode.Open
        public IsolatedStorageFileStream(string path, FileMode mode)
            : base(path, mode)
        {
            _filePath = path;

            _fs = new MemoryStream();
            if (mode == FileMode.Open || mode == FileMode.OpenOrCreate)
            {
                //attempt to read the file:
                Object obj = Interop.ExecuteJavaScript("localStorage.getItem($0);", _filePath.ToLower() + "ǀǀCaseSensitivePath");
                bool isNullOrUndefined = false;
                if (Interop.IsRunningInTheSimulator)
                {
                    isNullOrUndefined = Convert.ToBoolean(CSHTML5.Interop.ExecuteJavaScript("$0 == undefined || $0 == null", obj));
                }
                else
                {
                    isNullOrUndefined = Interop.IsNull(obj) || Interop.IsUndefined(obj);
                }

                if (!isNullOrUndefined)
                {
                    string fileContent = Convert.ToString(obj);
                    byte[] bytes = Convert.FromBase64String(fileContent);
                    _fs.Write(bytes, 0, bytes.Length);
                    _fs.Position = 0;
                }
            }
        }

        // Exceptions:
        //   System.IO.IsolatedStorage.IsolatedStorageException:
        //     The write attempt exceeds the quota for the System.IO.IsolatedStorage.IsolatedStorageFileStream
        //     object.
        /// <summary>
        /// Writes a block of bytes to the System.IO.IsolatedStorage.IsolatedStorageFileStream
        /// object using data read from a byte array.
        /// </summary>
        /// <param name="buffer">The buffer to write.</param>
        /// <param name="offset">The byte offset in buffer from which to begin.</param>
        /// <param name="count">The maximum number of bytes to write.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            //write in the stream:
            _fs.Write(buffer, offset, count);
            _fs.Position = 0;

            //read the result and update the localStorage:
            long msLenght = this.Length;
            byte[] bytes = new byte[msLenght];
            _fs.Read(bytes, 0, (int)msLenght); //todo: fix if _ms.Length > int.MaxValue (_ms.Length is an Int64)

            //update the localStorage:
            string filePath_lowered = _filePath.ToLower();
            Interop.ExecuteJavaScript("localStorage.setItem($0, $1); localStorage.setItem($0, $2)", filePath_lowered + "ǀǀCaseSensitivePath", _filePath, System.Convert.ToBase64String(bytes)); //Note: this is not a pipe (the thing we get with ctrl+alt+6), it is U+01C0
        }

        /// <summary>
        /// Copies bytes from the current buffered System.IO.IsolatedStorage.IsolatedStorageFileStream
        /// object to an array.
        /// </summary>
        /// <param name="buffer">The buffer to read.</param>
        /// <param name="offset">The offset in the buffer at which to begin writing.</param>
        /// <param name="count">The maximum number of bytes to read.</param>
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than the
        /// number of bytes requested if that many bytes are not currently available,
        /// or zero if the end of the stream is reached.
        /// </returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            //_fs.Position = 0;
            return _fs.Read(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            if (_fs != null)
                _fs.Dispose();
            base.Dispose(disposing);
        }

        public override void Flush()
        {
            _fs.Flush();
            base.Flush();
        }

        public override long Position
        {
            get { return _fs.Position; }
            set { _fs.Position = value; }
        }

        public override long Length
        {
            get { return _fs.Length; }
        }

        public override int ReadByte()
        {
            return _fs.ReadByte();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _fs.Seek(offset, origin);
        }

        public override void WriteByte(byte value)
        {
            _fs.WriteByte(value);
        }

        public override void Close()
        {
            _fs.Close();
            base.Close();
        }

        public void Dispose()
        {
            this.Close();
        }
    }
}
