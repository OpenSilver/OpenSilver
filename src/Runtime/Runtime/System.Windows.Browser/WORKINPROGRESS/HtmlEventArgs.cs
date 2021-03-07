

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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Windows.Browser
{
#if WORKINPROGRESS
    public partial class HtmlEventArgs : EventArgs
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
        public bool AltKey { get; }
        public bool CtrlKey { get; }
        public int ClientX { get; }
        public int ClientY { get; }
        public bool ShiftKey { get; }

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
