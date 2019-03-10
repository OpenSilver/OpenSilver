
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq
{
      //// Summary:
      //  //     Represents a sorted sequence.
      //  //
      //  // Type parameters:
      //  //   TElement:
      //  //     The type of the elements of the sequence.
      //  public interface IOrderedEnumerable<TElement> : IEnumerable<TElement>, IEnumerable
      //  {
      //      // Summary:
      //      //     Performs a subsequent ordering on the elements of an System.Linq.IOrderedEnumerable<TElement>
      //      //     according to a key.
      //      //
      //      // Parameters:
      //      //   keySelector:
      //      //     The System.Func<T,TResult> used to extract the key for each element.
      //      //
      //      //   comparer:
      //      //     The System.Collections.Generic.IComparer<T> used to compare keys for placement
      //      //     in the returned sequence.
      //      //
      //      //   descending:
      //      //     true to sort the elements in descending order; false to sort the elements
      //      //     in ascending order.
      //      //
      //      // Type parameters:
      //      //   TKey:
      //      //     The type of the key produced by keySelector.
      //      //
      //      // Returns:
      //      //     An System.Linq.IOrderedEnumerable<TElement> whose elements are sorted according
      //      //     to a key.
      //      IOrderedEnumerable<TElement> CreateOrderedEnumerable<TKey>(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending);
      //  }
    

    //public abstract class OrderedEnumerable<TElement> : IOrderedEnumerable<TElement>
    //{
    //    internal IEnumerable<TElement> source;

    //    public IEnumerator<TElement> GetEnumerator()
    //    {
    //        return GetOrderedEnumerableEnumerator();
    //    }

    //    internal abstract IEnumerator<TElement> GetOrderedEnumerableEnumerator();
    //    //internal abstract EnumerableSorter<TElement> GetEnumerableSorter(EnumerableSorter<TElement> next);

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return GetEnumerator();
    //    }

    //    IOrderedEnumerable<TElement> IOrderedEnumerable<TElement>.CreateOrderedEnumerable<TKey>(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending)
    //    {
    //        OrderedEnumerable<TElement, TKey> result = new OrderedEnumerable<TElement, TKey>(source, keySelector, comparer, descending);
    //        result.parent = this;
    //        return result;
    //    }
    //}

    //public class OrderedEnumerable<TElement, TKey> : OrderedEnumerable<TElement>
    //{
    //    internal OrderedEnumerable<TElement> parent;
    //    internal Func<TElement, TKey> keySelector;
    //    internal IComparer<TKey> comparer;
    //    internal bool descending;

    //    public OrderedEnumerable(IEnumerable<TElement> source, Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending)
    //    {
    //        if (source == null) throw new ArgumentNullException("The source is null.");
    //        if (keySelector == null) throw new ArgumentNullException("The keySelector is null");
    //        this.source = source;
    //        this.parent = null;
    //        this.keySelector = keySelector;
    //        this.comparer = comparer != null ? comparer : Comparer<TKey>.Default;
    //        this.descending = descending;
    //    }

    //    internal override IEnumerator<TElement> GetOrderedEnumerableEnumerator()
    //    {
    //        List<TElement> buffer = new List<TElement>(source);
    //        int descendingAdapter = descending ? -1 : 1;
    //        while (buffer.Count > 0)
    //        {
    //            int index = 0;
    //            int currentIndex = 0;
    //            foreach (TElement element in buffer)
    //            {
    //                int elementsComparison = comparer.Compare(keySelector(element), keySelector(buffer.ElementAt(index)));
    //                if ((descendingAdapter * elementsComparison) < 0)
    //                {
    //                    index = currentIndex;
    //                }
    //                ++currentIndex;
    //            }
    //            yield return buffer.ElementAt(index);
    //            buffer.RemoveAt(index);
    //        }
    //    }

    //    //internal IOrderedEnumerable<TElement> MyMethod(IOrderedEnumerable<TElement> source, Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending)
    //    //{
    //    //    return source.CreateOrderedEnumerable<TKey>(keySelector, comparer, descending);
    //    //}

    //    //internal override EnumerableSorter<TElement> GetEnumerableSorter(EnumerableSorter<TElement> next)
    //    //{
    //    //    EnumerableSorter<TElement> sorter = new EnumerableSorter<TElement, TKey>(keySelector, comparer, descending, next);
    //    //    if (parent != null) sorter = parent.GetEnumerableSorter(sorter);
    //    //    return sorter;
    //    //}
    //}







    //public class OrderedEnumerable<TElement, TKey> : List<TElement>, IOrderedEnumerable<TElement>
    //{
    //    Func<TElement, TKey> _keySelector;
    //    IComparer<TKey> _comparer;

    //    public OrderedEnumerable(IEnumerable<TElement> collection, Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending)
    //        : base()
    //    {
    //        _keySelector = keySelector;
    //        _comparer = comparer;
    //        if (collection is OrderedEnumerable<TElement, TKey>)
    //        {
    //            //In this case, we do not want to sort the collection directly, we can only sort the elements that were considered to have the same "value" by the previous OrderedEnumerable.
    //            //              Example: previous one ordered a List<string> by the length of the strings, then we want to order alphabetically --> we still have to keep the length of the strings sorted properly and then sort the ones that have the same amout of characters among themselves.
    //            OrderedEnumerable<TElement, TKey> collectionAsIOrderedEnumerable = (OrderedEnumerable<TElement, TKey>)collection;
    //            var parentComparer = collectionAsIOrderedEnumerable._comparer;
    //            var parentKeySelector = collectionAsIOrderedEnumerable._keySelector;
    //            if (parentComparer == null)
    //            {
    //                parentComparer = Comparer<TKey>.Default;
    //            }
    //            TKey lastValue = keySelector(collectionAsIOrderedEnumerable.ElementAt(0));
    //            // We add the elements to this one by one from the collection. When we find one that is considered "equal" by the comparer, we will need to sort them using the comparer of this instance.

    //            List<TElement> tempList = null; //we keep a list of the elements considered equal by the parent's comparer.
    //            TKey parentKeyInTempList = default(TKey); //value here doesn't matter since at the first iteration we won't read it but we will set it.
    //            foreach (TElement testedElement in collectionAsIOrderedEnumerable)
    //            {
    //                TKey parentKey = parentKeySelector(testedElement);

    //                if (tempList == null)
    //                {
    //                    tempList = new List<TElement>();
    //                    tempList.Add(testedElement);
    //                    parentKeyInTempList = parentKey;
    //                }
    //                else
    //                {
    //                    int parentComparison = parentComparer.Compare(parentKeyInTempList, parentKey); //Note: We only need to test this because the collection is already sorted to fit the order of the parent OrderedEnumerable (no need to redo the sorting for the parent).
    //                    if (parentComparison != 0) //we have been through all the elements that have the same parentKey's value --> we sort them according to this instance's comparer and add them to this instance.
    //                    {
    //                        if (_comparer != null)
    //                            tempList.Sort(CustomComparer);
    //                        else
    //                            tempList.Sort(DefaultComparer);
    //                        if (descending)
    //                            tempList.Reverse();

    //                        this.AddRange(tempList);
    //                        parentKeyInTempList = parentKey;
    //                        tempList.Clear();
    //                        tempList.Add(testedElement);
    //                    }
    //                    else //the key is considered equal to the previous one by the parent's comparer.
    //                    {
    //                        tempList.Add(testedElement);
    //                    }
    //                }
    //            }
    //            //we add the last templist:
    //            if (_comparer != null)
    //                tempList.Sort(CustomComparer);
    //            else
    //                tempList.Sort(DefaultComparer);
    //            if (descending)
    //                tempList.Reverse();

    //            this.AddRange(tempList);
    //        }
    //        else
    //        {
    //            this.AddRange(collection);

    //            if (_comparer != null)
    //                this.Sort(CustomComparer);
    //            else
    //                this.Sort(DefaultComparer);
    //            if (descending)
    //                this.Reverse();
    //        }
    //    }

    //    int DefaultComparer(TElement a, TElement b)
    //    {
    //        return Comparer<TKey>.Default.Compare(_keySelector(a), _keySelector(b));
    //    }

    //    int CustomComparer(TElement a, TElement b)
    //    {
    //        return _comparer.Compare(_keySelector(a), _keySelector(b));
    //    }

    //    public IOrderedEnumerable<TElement> CreateOrderedEnumerable<TKey>(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

}
