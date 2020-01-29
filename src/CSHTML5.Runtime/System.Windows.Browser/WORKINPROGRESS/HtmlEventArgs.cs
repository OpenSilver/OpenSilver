using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Windows.Browser
{
#if WORKINPROGRESS
    public class HtmlEventArgs : EventArgs
    {
        #region Fields
        private int _offsetX;
        private int _offsetY;
        private ScriptObject _eventObject;
        #endregion

        #region Properties
        public int OffsetX
        {
            get { return _offsetX; }
        }
        public int OffsetY
        {
            get { return _offsetY; }
        }
        public ScriptObject EventObject
        {
            get { return _eventObject; }
        }
        #endregion

        #region Methods
        internal HtmlEventArgs()
        {
            _offsetX = 0;
            _offsetY = 0;
            _eventObject = null;
        }
        public void PreventDefault()
        {
        }
        #endregion
    }
#endif
}
