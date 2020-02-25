﻿
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
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

