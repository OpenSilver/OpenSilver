

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
    public abstract partial class HtmlObject : ScriptObject
    {
        #region Methods
        protected HtmlObject()
        {
        }
        public bool AttachEvent(string @eventName, EventHandler @handler)
        {
            return false;
        }
        public bool AttachEvent(string @eventName, EventHandler<HtmlEventArgs> @handler)
        {
            return false;
        }
        #endregion
    }
#endif
}
