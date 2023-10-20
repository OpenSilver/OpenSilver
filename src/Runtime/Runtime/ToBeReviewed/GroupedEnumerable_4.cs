

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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq
{
    public class GroupedEnumerable<TSource, TKey, TElement, TResult> : IEnumerable<TResult>
    {
        Collection<TResult> result = new Collection<TResult>();
        Dictionary<TKey, Grouping<TKey, TElement>> groups = new Dictionary<TKey, Grouping<TKey, TElement>>();

        public GroupedEnumerable(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (keySelector == null) throw new ArgumentNullException("keySelector");
            if (elementSelector == null) throw new ArgumentNullException("elementSelector");
            if (resultSelector == null) throw new ArgumentNullException("resultSelector");

            FillGroups(source, keySelector, elementSelector, comparer);
            
            foreach (TKey groupKey in groups.Keys)
            {
                result.Add(resultSelector(groupKey, groups[groupKey]));
            }
        }

        private void FillGroups(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            foreach (TSource sourceElement in source)
            {
                TElement element = elementSelector(sourceElement);
                TKey key = keySelector(sourceElement);
                if (!groups.ContainsKey(key))
                {
                    groups.Add(key, new Grouping<TKey, TElement>(key));
                }
                var v = groups[key];
                v.Add(element);
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return result.GetEnumerator();
        }

        IEnumerator<TResult> IEnumerable<TResult>.GetEnumerator()
        {
            return result.GetEnumerator();
        }
    }

}

