
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq
{
    public class GroupedEnumerable<TSource, TKey, TElement> : IEnumerable<IGrouping<TKey, TElement>>
    {
        Dictionary<TKey, Grouping<TKey, TElement>> groups = new Dictionary<TKey, Grouping<TKey, TElement>>();

        public GroupedEnumerable(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
#if !BRIDGE
            if (source == null) throw new ArgumentNullException("source");
            if (keySelector == null) throw new ArgumentNullException("keySelector");
            if (elementSelector == null) throw new ArgumentNullException("elementSelector");


            FillGroups(source, keySelector, elementSelector, comparer);
#endif
        }

        private void FillGroups(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
#if !BRIDGE
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
#endif
        }

        public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
        {
            foreach (TKey key in groups.Keys)
            {
                yield return groups[key];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }



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
#if !BRIDGE
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
#endif
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

