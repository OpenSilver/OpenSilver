
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


using DotNetForHtml5;
using System;
using System.Text;

namespace Runtime.OpenSilver.PublicAPI.Interop
{
    internal class PendingJavascript
    {
        private static readonly Encoding DefaultEncoding = Encoding.Unicode;
        private static readonly byte[] Delimiter = DefaultEncoding.GetBytes(";\n");
        private readonly object _syncObj = new ();
        private byte[] _buffer;
        private int _currentLength;

        public PendingJavascript(int bufferSize)
        {
            if (bufferSize <= 0)
            {
                throw new ArgumentException("Buffer size can not be less or equal to 0");
            }
            _buffer = new byte[bufferSize];
        }

        private void IncreaseBuffer()
        {
            var currentBuffer = _buffer;
            _buffer = new byte[currentBuffer.Length * 2];
            currentBuffer.CopyTo(_buffer, 0);
        }

        public void AddJavascript(string javascript)
        {
            lock (_syncObj)
            {
                var maxByteCount = DefaultEncoding.GetMaxByteCount(javascript.Length);
                while (maxByteCount + _currentLength > _buffer.Length)
                {
                    IncreaseBuffer();
                }

                _currentLength += DefaultEncoding.GetBytes(javascript, 0, javascript.Length, _buffer, _currentLength);

                Buffer.BlockCopy(Delimiter, 0, _buffer, _currentLength, Delimiter.Length);
                _currentLength += Delimiter.Length;
            }
        }

        public object ExecutePending(IWebAssemblyExecutionHandler executionHandler)
        {
            if (_currentLength == 0)
            {
                return null;
            }

            var curLength = _currentLength;
            _currentLength = 0;
            //Here we pass a reference to _buffer object and current length
            //Js will read data from the heap
            return executionHandler.InvokeUnmarshalled<byte[], int, object>("callJSUnmarshalledHeap",
                _buffer, curLength);
        }

        public string TakeJsOut()
        {
            lock (_syncObj)
            {
                var res = DefaultEncoding.GetString(_buffer, 0, _currentLength);
                _currentLength = 0;
                return res;
            }
        }
    }
}
