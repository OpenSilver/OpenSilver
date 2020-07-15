using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetForHtml5.VisualStudioExtension.Editor.SplittedXamlEditor
{
    class TextLineEventListener : IVsTextLinesEvents, IDisposable
    {
        public event EventHandler TextChanged;

        private IVsTextLines _buffer;
        private IConnectionPoint _connectionPoint;
        private uint _connectionCookie;

        public TextLineEventListener(IVsTextLines buffer)
        {
            this._buffer = buffer;
            IConnectionPointContainer container = buffer as IConnectionPointContainer;
            if (null != container)
            {
                Guid eventsGuid = typeof(IVsTextLinesEvents).GUID;
                container.FindConnectionPoint(ref eventsGuid, out _connectionPoint);
                _connectionPoint.Advise(this as IVsTextLinesEvents, out _connectionCookie);
            }
        }

        #region IVsTextLinesEvents Members

        void IVsTextLinesEvents.OnChangeLineAttributes(int iFirstLine, int iLastLine)
        {
            // Do Nothing
        }

        void IVsTextLinesEvents.OnChangeLineText(TextLineChange[] pTextLineChange, int fLast)
        {
            if (TextChanged != null)
            {
                TextChanged(this, new EventArgs());
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if ((null != _connectionPoint) && (0 != _connectionCookie))
            {
                _connectionPoint.Unadvise(_connectionCookie);
                System.Diagnostics.Debug.WriteLine("\n\tUnadvised from TextLinesEvents\n");
            }
            _connectionCookie = 0;
            _connectionPoint = null;

            this._buffer = null;
        }

        #endregion
    }
}
