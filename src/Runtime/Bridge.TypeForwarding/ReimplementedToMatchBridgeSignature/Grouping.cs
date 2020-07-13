using System.Collections;
using System.Collections.Generic;

namespace System.Linq
{
    //todo: implement this class
    public class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        public TKey Key
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}