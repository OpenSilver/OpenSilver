
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
    //We exclude this class when we use Bridge, because it is already used by it (and it creates conflict with CSHTML5)
#if !BRIDGE
    public class Grouping<TKey, TElement> : IGrouping<TKey, TElement>, IEnumerable<TElement>
    {
        Collection<TElement> elements;

        private TKey _key;
        public TKey Key
        {
            get { return _key; }
        }

        internal Grouping(TKey key)
        {
            elements = new Collection<TElement>();
            _key = key;
        }

        internal void Add(TElement element)
        {
            elements.Add(element);
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            return elements.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return elements.GetEnumerator();
        }
    }
#endif
}
