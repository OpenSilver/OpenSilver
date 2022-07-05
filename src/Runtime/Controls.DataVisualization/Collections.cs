using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Linq;


namespace System.Windows.Controls.DataVisualization
{
    internal class NoResetObservableCollection<T> : System.Collections.ObjectModel.ObservableCollection<T>
    {
        /// <summary>
        /// Clears all items in the collection by removing them individually.
        /// </summary>
        protected override void ClearItems()
        {
            foreach (T obj in (IEnumerable<T>)new List<T>((IEnumerable<T>)this))
                this.Remove(obj);
        }
    }

    internal class ReadOnlyObservableCollection<T> : NoResetObservableCollection<T>
    {
        /// <summary>
        /// Gets or sets a value indicating whether the owner is writing to the
        /// collection.
        /// </summary>
        private bool IsMutating { get; set; }

        /// <summary>A method that mutates the collection.</summary>
        /// <param name="action">The action to mutate the collection.</param>
        public void Mutate(Action<ReadOnlyObservableCollection<T>> action)
        {
            this.IsMutating = true;
            try
            {
                action(this);
            }
            finally
            {
                this.IsMutating = false;
            }
        }

        /// <summary>Removes an item from the collection at an index.</summary>
        /// <param name="index">The index to remove.</param>
        protected override void RemoveItem(int index)
        {
            if (!this.IsMutating)
                throw new NotSupportedException("Cann't remove as collection is readonly");
            base.RemoveItem(index);
        }

        /// <summary>
        /// Sets an item at a particular location in the collection.
        /// </summary>
        /// <param name="index">The location to set an item.</param>
        /// <param name="item">The item to set.</param>
        protected override void SetItem(int index, T item)
        {
            if (!this.IsMutating)
                throw new NotSupportedException("Cann't set as collection is readonly");
            base.SetItem(index, item);
        }

        /// <summary>Inserts an item in the collection.</summary>
        /// <param name="index">The index at which to insert the item.</param>
        /// <param name="item">The item to insert.</param>
        protected override void InsertItem(int index, T item)
        {
            if (!this.IsMutating)
                throw new NotSupportedException("Cann't insert as collection is readonly");
            base.InsertItem(index, item);
        }

        /// <summary>Clears the items from the collection.</summary>
        protected override void ClearItems()
        {
            if (!this.IsMutating)
                throw new NotSupportedException("Cann't clear as collection is readonly");
            base.ClearItems();
        }
    }

    internal class AggregatedObservableCollection<T> : ReadOnlyObservableCollection<T>
    {
        /// <summary>
        /// Initializes a new instance of an aggregated observable collection.
        /// </summary>
        public AggregatedObservableCollection()
        {
            this.ChildCollections = (System.Collections.ObjectModel.ObservableCollection<IList>)new NoResetObservableCollection<IList>();
            this.ChildCollections.CollectionChanged += new NotifyCollectionChangedEventHandler(this.ChildCollectionsCollectionChanged);
        }

        /// <summary>Rebuilds the list if a collection changes.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        private void ChildCollectionsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            System.Diagnostics.Debug.Assert(e.Action != NotifyCollectionChangedAction.Reset, "Reset is not supported.");
            if (e.Action == NotifyCollectionChangedAction.Add)
                e.NewItems.OfType<IList>().ForEachWithIndex<IList>((Action<IList, int>)((newCollection, index) =>
                {
                    int startingIndex = this.GetStartingIndexOfCollectionAtIndex(e.NewStartingIndex + index);
                    using (IEnumerator<T> enumerator = newCollection.OfType<T>().Reverse<T>().GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            T item = enumerator.Current;
                            this.Mutate((Action<ReadOnlyObservableCollection<T>>)(items => items.Insert(startingIndex, item)));
                        }
                    }
                    INotifyCollectionChanged collectionChanged = newCollection as INotifyCollectionChanged;
                    if (collectionChanged == null)
                        return;
                    collectionChanged.CollectionChanged += new NotifyCollectionChangedEventHandler(this.ChildCollectionCollectionChanged);
                }));
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (IList oldItem in (IEnumerable)e.OldItems)
                {
                    INotifyCollectionChanged collectionChanged = oldItem as INotifyCollectionChanged;
                    if (collectionChanged != null)
                        collectionChanged.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.ChildCollectionCollectionChanged);
                    IEnumerator enumerator = oldItem.GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            T item = (T)enumerator.Current;
                            this.Mutate((Action<ReadOnlyObservableCollection<T>>)(items => items.Remove(item)));
                        }
                    }
                    finally
                    {
                        IDisposable disposable = enumerator as IDisposable;
                        if (disposable != null)
                            disposable.Dispose();
                    }
                }
            }
            else
            {
                if (e.Action != NotifyCollectionChangedAction.Replace)
                    return;
                foreach (IList oldItem in (IEnumerable)e.OldItems)
                {
                    INotifyCollectionChanged collectionChanged = oldItem as INotifyCollectionChanged;
                    if (collectionChanged != null)
                        collectionChanged.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.ChildCollectionCollectionChanged);
                }
                foreach (IList newItem in (IEnumerable)e.NewItems)
                {
                    INotifyCollectionChanged collectionChanged = newItem as INotifyCollectionChanged;
                    if (collectionChanged != null)
                        collectionChanged.CollectionChanged += new NotifyCollectionChangedEventHandler(this.ChildCollectionCollectionChanged);
                }
                this.Rebuild();
            }
        }

        /// <summary>
        /// Synchronizes the collection with changes made in a child collection.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        private void ChildCollectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            System.Diagnostics.Debug.Assert(e.Action != NotifyCollectionChangedAction.Reset, "Reset is not supported.");
            IList list = sender as IList;
            // ISSUE: reference to a compiler-generated field
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                int startingIndex = this.GetStartingIndexOfCollectionAtIndex(this.ChildCollections.IndexOf(list));
                // ISSUE: reference to a compiler-generated field
                e.NewItems.OfType<T>().ForEachWithIndex<T>((Action<T, int>)((item, index) =>
                {
                    this.Mutate((Action<ReadOnlyObservableCollection<T>>)(that => that.Insert(startingIndex + e.NewStartingIndex + index, item)));
                }));
            }
            else
            {
                // ISSUE: reference to a compiler-generated field
                if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    // ISSUE: reference to a compiler-generated field
                    using (IEnumerator<T> enumerator = e.OldItems.OfType<T>().GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            T item = enumerator.Current;
                            this.Mutate((Action<ReadOnlyObservableCollection<T>>)(that => that.Remove(item)));
                        }
                    }
                }
                else
                {
                    // ISSUE: reference to a compiler-generated field
                    if (e.Action != NotifyCollectionChangedAction.Replace)
                        return;
                    // ISSUE: reference to a compiler-generated field
                    for (int index = 0; index < e.NewItems.Count; ++index)
                    {
                        // ISSUE: reference to a compiler-generated field
                        T oldItem = (T)e.OldItems[index];
                        // ISSUE: reference to a compiler-generated field
                        T newItem = (T)e.NewItems[index];
                        int oldItemIndex = this.IndexOf(oldItem);
                        this.Mutate((Action<ReadOnlyObservableCollection<T>>)(that => that[oldItemIndex] = newItem));
                    }
                }
            }
        }

        /// <summary>
        /// Returns the starting index of a collection in the aggregate
        /// collection.
        /// </summary>
        /// <param name="index">The starting index of a collection.</param>
        /// <returns>The starting index of the collection in the aggregate
        /// collection.</returns>
        private int GetStartingIndexOfCollectionAtIndex(int index)
        {
            return CollectionHelper.Count(this.ChildCollections.OfType<IEnumerable>().Select<IEnumerable, IEnumerable<T>>((Func<IEnumerable, IEnumerable<T>>)(collection => collection.CastWrapper<T>())).Take<IEnumerable<T>>(index).SelectMany<IEnumerable<T>, T>((Func<IEnumerable<T>, IEnumerable<T>>)(collection => collection)));
        }

        /// <summary>
        /// Rebuild the list in the correct order when a child collection
        /// changes.
        /// </summary>
        private void Rebuild()
        {
            this.Mutate((Action<ReadOnlyObservableCollection<T>>)(that => that.Clear()));
            this.Mutate((Action<ReadOnlyObservableCollection<T>>)(that =>
            {
                foreach (T obj in (IEnumerable<T>)this.ChildCollections.OfType<IEnumerable>().Select<IEnumerable, IEnumerable<T>>((Func<IEnumerable, IEnumerable<T>>)(collection => collection.CastWrapper<T>())).SelectMany<IEnumerable<T>, T>((Func<IEnumerable<T>, IEnumerable<T>>)(collection => collection)).ToList<T>())
                    that.Add(obj);
            }));
        }

        /// <summary>Gets child collections of the aggregated collection.</summary>
        public System.Collections.ObjectModel.ObservableCollection<IList> ChildCollections { get; private set; }
    }

    

    internal class ObservableCollectionListAdapter<T> where T : class
    {
        /// <summary>The collection to synchronize with a list.</summary>
        private IEnumerable _collection;

        /// <summary>
        /// Gets or sets the collection to synchronize with a list.
        /// </summary>
        public IEnumerable Collection
        {
            get
            {
                return this._collection;
            }
            set
            {
                INotifyCollectionChanged collection = this._collection as INotifyCollectionChanged;
                INotifyCollectionChanged collectionChanged = value as INotifyCollectionChanged;
                this._collection = value;
                if (collection != null)
                    collection.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.OnCollectionChanged);
                if (value == null && this.TargetList != null)
                    this.TargetList.Clear();
                if (collectionChanged == null)
                    return;
                collectionChanged.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnCollectionChanged);
            }
        }

        /// <summary>
        /// Gets or sets the panel to synchronize with the collection.
        /// </summary>
        public IList TargetList { get; set; }

        /// <summary>
        /// Method that synchronizes the panel's child collection with the
        /// contents of the observable collection when it changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        public void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.TargetList == null)
                return;
            if (e.Action == NotifyCollectionChangedAction.Reset)
                this.TargetList.Clear();
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                for (int index1 = 0; index1 < e.OldItems.Count; ++index1)
                {
                    T oldItem = e.OldItems[index1] as T;
                    T newItem = e.NewItems[index1] as T;
                    int index2 = this.TargetList.IndexOf((object)oldItem);
                    if (index2 != -1)
                    {
                        this.TargetList[index2] = (object)newItem;
                    }
                    else
                    {
                        this.TargetList.Remove((object)oldItem);
                        this.TargetList.Add((object)newItem);
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
            {
                foreach (T oldItem in (IEnumerable)e.OldItems)
                    this.TargetList.Remove((object)oldItem);
            }
            else if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                int startingIndex = e.NewStartingIndex;
                if (startingIndex != 0)
                {
                    int num = this.TargetList.IndexOf((object)this.Collection.FastElementAt<T>(startingIndex - 1));
                    if (num != -1)
                        startingIndex = num + 1;
                }
                else if (this.Collection.FastCount() > 1)
                {
                    int num = this.TargetList.IndexOf((object)this.Collection.FastElementAt<T>(startingIndex + 1));
                    startingIndex = num != -1 ? num : 0;
                }
                e.NewItems.OfType<T>().ForEachWithIndex<T>((Action<T, int>)((item, index) => this.TargetList.Insert(startingIndex + index, (object)item)));
            }
        }

        /// <summary>
        /// A method that populates a panel with the items in the collection.
        /// </summary>
        public void Populate()
        {
            if (this.TargetList == null)
                return;
            if (this.Collection != null)
            {
                foreach (T obj in this.Collection)
                    this.TargetList.Add((object)obj);
            }
            else
                this.TargetList.Clear();
        }

        /// <summary>
        /// Removes the items in the adapted list from the target list.
        /// </summary>
        public void ClearItems()
        {
            foreach (T obj in this.Collection)
                this.TargetList.Remove((object)obj);
        }
    }

    internal static class EnumerableFunctions
    {
        /// <summary>
        /// Attempts to cast IEnumerable to a list in order to retrieve a count
        /// in order one.  It attempts to cast fail the sequence is enumerated.
        /// </summary>
        /// <param name="that">The sequence.</param>
        /// <returns>The number of elements in the sequence.</returns>
        public static int FastCount(this IEnumerable that)
        {
            IList list = that as IList;
            if (list != null)
                return list.Count;
            return CollectionHelper.Count(that.CastWrapper<object>());
        }

        /// <summary>
        /// Returns the minimum value in the stream based on the result of a
        /// project function.
        /// </summary>
        /// <typeparam name="T">The stream type.</typeparam>
        /// <param name="that">The stream.</param>
        /// <param name="projectionFunction">The function that transforms the
        /// item.</param>
        /// <returns>The minimum value or null.</returns>
        public static T MinOrNull<T>(this IEnumerable<T> that, Func<T, IComparable> projectionFunction) where T : class
        {
            T obj1 = default(T);
            if (!that.Any<T>())
                return obj1;
            T obj2 = that.First<T>();
            IComparable comparable1 = projectionFunction(obj2);
            foreach (T obj3 in that.Skip<T>(1))
            {
                IComparable comparable2 = projectionFunction(obj3);
                if (comparable1.CompareTo((object)comparable2) > 0)
                {
                    comparable1 = comparable2;
                    obj2 = obj3;
                }
            }
            return obj2;
        }

        /// <summary>
        /// Returns the sum of all values in the sequence or the default value.
        /// </summary>
        /// <param name="that">The stream.</param>
        /// <returns>The sum of all values or the default value.</returns>
        public static double SumOrDefault(this IEnumerable<double> that)
        {
            if (!that.Any<double>())
                return 0.0;
            return that.Sum();
        }

        /// <summary>
        /// Returns the maximum value in the stream based on the result of a
        /// project function.
        /// </summary>
        /// <typeparam name="T">The stream type.</typeparam>
        /// <param name="that">The stream.</param>
        /// <param name="projectionFunction">The function that transforms the
        /// item.</param>
        /// <returns>The maximum value or null.</returns>
        public static T MaxOrNull<T>(this IEnumerable<T> that, Func<T, IComparable> projectionFunction) where T : class
        {
            T obj1 = default(T);
            if (!that.Any<T>())
                return obj1;
            T obj2 = that.First<T>();
            IComparable comparable1 = projectionFunction(obj2);
            foreach (T obj3 in that.Skip<T>(1))
            {
                IComparable comparable2 = projectionFunction(obj3);
                if (comparable1.CompareTo((object)comparable2) < 0)
                {
                    comparable1 = comparable2;
                    obj2 = obj3;
                }
            }
            return obj2;
        }

        /// <summary>
        /// Accepts two sequences and applies a function to the corresponding
        /// values in the two sequences.
        /// </summary>
        /// <typeparam name="T0">The type of the first sequence.</typeparam>
        /// <typeparam name="T1">The type of the second sequence.</typeparam>
        /// <typeparam name="R">The return type of the function.</typeparam>
        /// <param name="enumerable0">The first sequence.</param>
        /// <param name="enumerable1">The second sequence.</param>
        /// <param name="func">The function to apply to the corresponding values
        /// from the two sequences.</param>
        /// <returns>A sequence of transformed values from both sequences.</returns>
        public static IEnumerable<R> Zip<T0, T1, R>(IEnumerable<T0> enumerable0, IEnumerable<T1> enumerable1, Func<T0, T1, R> func)
        {
            IEnumerator<T0> enumerator0 = enumerable0.GetEnumerator();
            IEnumerator<T1> enumerator1 = enumerable1.GetEnumerator();
            while (enumerator0.MoveNext() && enumerator1.MoveNext())
                yield return func(enumerator0.Current, enumerator1.Current);
        }

        /// <summary>
        /// Creates a sequence of values by accepting an initial value, an
        /// iteration function, and apply the iteration function recursively.
        /// </summary>
        /// <typeparam name="T">The type of the sequence.</typeparam>
        /// <param name="value">The initial value.</param>
        /// <param name="nextFunction">The function to apply to the value.</param>
        /// <returns>A sequence of the iterated values.</returns>
        public static IEnumerable<T> Iterate<T>(T value, Func<T, T> nextFunction)
        {
            yield return value;
            while (true)
            {
                value = nextFunction(value);
                yield return value;
            }
        }

        /// <summary>Returns the index of an item in a sequence.</summary>
        /// <param name="that">The sequence.</param>
        /// <param name="value">The item to search for.</param>
        /// <returns>The index of the item or -1 if not found.</returns>
        public static int IndexOf(this IEnumerable that, object value)
        {
            int num = 0;
            foreach (object objB in that)
            {
                if (object.ReferenceEquals(value, objB) || value.Equals(objB))
                    return num;
                ++num;
            }
            return -1;
        }

        /// <summary>
        /// Executes an action for each item and a sequence, passing in the
        /// index of that item to the action procedure.
        /// </summary>
        /// <typeparam name="T">The type of the sequence.</typeparam>
        /// <param name="that">The sequence.</param>
        /// <param name="action">A function that accepts a sequence item and its
        /// index in the sequence.</param>
        public static void ForEachWithIndex<T>(this IEnumerable<T> that, Action<T, int> action)
        {
            int num = 0;
            foreach (T obj in that)
            {
                action(obj, num);
                ++num;
            }
        }

        /// <summary>
        /// Attempts to retrieve an element at an index by testing whether a
        /// sequence is randomly accessible.  If not, performance degrades to a
        /// linear search.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="that">The sequence.</param>
        /// <param name="index">The index of the element in the sequence.</param>
        /// <returns>The element at the given index.</returns>
        public static T FastElementAt<T>(this IEnumerable that, int index)
        {
            IList<T> objList = that as IList<T>;
            if (objList != null)
                return objList[index];
            IList list = that as IList;
            if (list != null)
                return (T)list[index];
            return that.CastWrapper<T>().ElementAt<T>(index);
        }

        /// <summary>
        /// Applies an accumulator function over a sequence and returns each intermediate result.
        /// </summary>
        /// <typeparam name="T">Type of elements in source sequence.</typeparam>
        /// <typeparam name="S">Type of elements in result sequence.</typeparam>
        /// <param name="that">Sequence to scan.</param>
        /// <param name="seed">Initial accumulator value.</param>
        /// <param name="accumulator">Function used to generate the result sequence.</param>
        /// <returns>Sequence of intermediate results.</returns>
        public static IEnumerable<S> Scan<T, S>(this IEnumerable<T> that, S seed, Func<S, T, S> accumulator)
        {
            S value = seed;
            yield return seed;
            foreach (T obj in that)
            {
                value = accumulator(value, obj);
                yield return value;
            }
        }

        /// <summary>
        /// Converts the elements of an System.Collections.IEnumerable to the specified type.
        /// </summary>
        /// <remarks>
        /// A wrapper for the Enumerable.Cast(T) method that works around a limitation on some platforms.
        /// </remarks>
        /// <typeparam name="TResult">The type to convert the elements of source to.</typeparam>
        /// <param name="source">The System.Collections.IEnumerable that contains the elements to be converted.</param>
        /// <returns>
        /// An System.Collections.Generic.IEnumerable(T) that contains each element of the source sequence converted to the specified type.
        /// </returns>
        public static IEnumerable<TResult> CastWrapper<TResult>(this IEnumerable source)
        {
            return source.OfType<TResult>();
        }
    }


    /// <summary>A bag of weak references to items.</summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    internal class WeakReferenceBag<T> : IEnumerable<T>, IEnumerable where T : class
    {
        /// <summary>Gets or sets the list of event listeners.</summary>
        private IList<WeakReference> Items { get; set; }

        /// <summary>Initializes a new instance of the WeakEvent class.</summary>
        public WeakReferenceBag()
        {
            this.Items = (IList<WeakReference>)new List<WeakReference>();
        }

        /// <summary>Adds an item to the bag.</summary>
        /// <param name="item">The item to add to the bag.</param>
        public void Add(T item)
        {
            System.Diagnostics.Debug.Assert((object)item != null, "listener must not be null.");
            this.Items.Add(new WeakReference((object)item));
        }

        /// <summary>Removes an item from the bag.</summary>
        /// <param name="item">The item to remove.</param>
        public void Remove(T item)
        {
            System.Diagnostics.Debug.Assert((object)item != null, "listener must not be null.");
            int index = 0;
            while (index < this.Items.Count)
            {
                object target = this.Items[index].Target;
                if (!this.Items[index].IsAlive || object.ReferenceEquals(target, (object)item))
                    this.Items.RemoveAt(index);
                else
                    ++index;
            }
        }

        /// <summary>Returns a sequence of the elements in the bag.</summary>
        /// <returns>A sequence of the elements in the bag.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            int count = 0;
            while (count < this.Items.Count)
            {
                object target = this.Items[count].Target;
                if (!this.Items[count].IsAlive)
                {
                    this.Items.RemoveAt(count);
                }
                else
                {
                    yield return (T)target;
                    ++count;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this.GetEnumerator();
        }
    }

    /// <summary>
    /// An observable collection that does not allow duplicates.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    internal class UniqueObservableCollection<T> : System.Collections.ObjectModel.ObservableCollection<T>
    {
        /// <summary>
        /// Inserts an item at an index. Throws if the item already exists in the collection.
        /// </summary>
        /// <param name="index">The index at which to insert the item.</param>
        /// <param name="item">The item to insert.</param>
        protected override void InsertItem(int index, T item)
        {
            if (this.Contains(item))
                throw new InvalidOperationException("Can't insert as it violates uniqueness");
            base.InsertItem(index, item);
        }

        /// <summary>
        /// Sets an item at a given index. Throws if the item already exists at another index.
        /// </summary>
        /// <param name="index">The index at which to insert the item.</param>
        /// <param name="item">The item to be inserted.</param>
        protected override void SetItem(int index, T item)
        {
            int num = this.IndexOf(item);
            if (num != -1 && num != index)
                throw new InvalidOperationException("Can't insert as it violates uniqueness");
            base.SetItem(index, item);
        }

        /// <summary>
        /// Clears all items in the collection by removing them individually.
        /// </summary>
        protected override void ClearItems()
        {
            foreach (T obj in (IEnumerable<T>)new List<T>((IEnumerable<T>)this))
                this.Remove(obj);
        }
    }
}
