using System.Collections;
using System.Collections.Generic;

namespace System.Windows.Browser
{
#if WORKINPROGRESS
    public sealed class ScriptObjectCollection : ScriptObject, IEnumerable<ScriptObject>, IEnumerable
    {
        #region Fields
        private int _count;
        private ScriptObject _item;
        #endregion

        #region Properties
        public int Count
        {
            get { return _count; }
        }

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
#endif
}
