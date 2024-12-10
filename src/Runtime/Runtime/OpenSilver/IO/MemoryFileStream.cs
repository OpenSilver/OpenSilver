using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenSilver.IO
{
    public class MemoryFileStream : MemoryStream
    {
        private readonly Func<byte[], Task> _writeCallback;
        private readonly Action _closeCallback;

        public MemoryFileStream(Func<byte[], Task> writeCallback, Action closeCallback)
        {
            _writeCallback = writeCallback;
            _closeCallback = closeCallback;
        }

        public override async void Write(byte[] buffer, int offset, int count)
        {
            if (_writeCallback != null)
            {
                await _writeCallback.Invoke(buffer.Skip(offset).Take(count).ToArray());
            }
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (_writeCallback != null)
            {
                await _writeCallback.Invoke(buffer.Skip(offset).Take(count).ToArray());
            }
        }

        public override void Close()
        {
            base.Close();

            _closeCallback?.Invoke();
        }
    }
}
