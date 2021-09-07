

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


using System.Collections;
using System.Collections.Generic;

namespace System.Windows.Browser
{
	[OpenSilver.NotImplemented]
    public sealed partial class ScriptObjectCollection : ScriptObject, IEnumerable<ScriptObject>, IEnumerable
    {
        #region Fields
        private int _count;
        private ScriptObject _item;
        #endregion

        #region Properties
		[OpenSilver.NotImplemented]
        public int Count
        {
            get { return _count; }
        }

		[OpenSilver.NotImplemented]
        public ScriptObject this[int index]
        {
            get { return _item; }
        }
        #endregion

        #region Methods
        internal ScriptObjectCollection()
        {
            _count = 0;
            _item = null;
        }

        IEnumerator<ScriptObject> IEnumerable<ScriptObject>.GetEnumerator()
        {
            return null;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return null;
        }
        #endregion

    }
}
