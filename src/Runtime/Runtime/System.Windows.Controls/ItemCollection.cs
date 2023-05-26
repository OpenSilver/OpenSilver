
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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using OpenSilver.Internal;
using OpenSilver.Internal.Controls;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public sealed class ItemCollection : PresentationFrameworkCollection<object>, INotifyCollectionChanged
    {
        private bool _isUsingItemsSource;
        private IEnumerable _itemsSource; // base collection
        private WeakEventListener<ItemCollection, INotifyCollectionChanged, NotifyCollectionChangedEventArgs> _collectionChangedListener;

        private bool _isUsingListWrapper;
        private EnumerableWrapper _listWrapper;

        private IInternalFrameworkElement _modelParent;

        internal ItemCollection(IInternalFrameworkElement parent) : base(true)
        {
            this._modelParent = parent;
        }

        internal override bool IsFixedSizeImpl
        {
            get { return this.IsUsingItemsSource; }
        }

        internal override bool IsReadOnlyImpl
        {
            get { return this.IsUsingItemsSource; }
        }

        internal override void AddOverride(object value)
        {
            if (this.IsUsingItemsSource)
            {
                throw new InvalidOperationException("Operation is not valid while ItemsSource is in use. Access and modify elements with ItemsControl.ItemsSource instead.");
            }

            this.SetModelParent(value);
            this.AddInternal(value);
        }

        internal override void ClearOverride()
        {
            if (this.IsUsingItemsSource)
            {
                throw new InvalidOperationException("Operation is not valid while ItemsSource is in use. Access and modify elements with ItemsControl.ItemsSource instead.");
            }

            foreach (var item in this)
            {
                this.ClearModelParent(item);
            }

            this.ClearInternal();
        }

        internal override void InsertOverride(int index, object value)
        {
            if (this.IsUsingItemsSource)
            {
                throw new InvalidOperationException("Operation is not valid while ItemsSource is in use. Access and modify elements with ItemsControl.ItemsSource instead.");
            }

            this.SetModelParent(value);
            this.InsertInternal(index, value);
        }

        internal override void RemoveAtOverride(int index)
        {
            if (this.IsUsingItemsSource)
            {
                throw new InvalidOperationException("Operation is not valid while ItemsSource is in use. Access and modify elements with ItemsControl.ItemsSource instead.");
            }

            object removedItem = this.GetItemInternal(index);
            this.ClearModelParent(removedItem);
            this.RemoveAtInternal(index);
        }

        internal override object GetItemOverride(int index)
        {
            if (this.IsUsingItemsSource)
            {
                return this.SourceList[index];
            }

            return this.GetItemInternal(index);
        }

        internal override void SetItemOverride(int index, object value)
        {
            if (this.IsUsingItemsSource)
            {
                throw new InvalidOperationException("Operation is not valid while ItemsSource is in use. Access and modify elements with ItemsControl.ItemsSource instead.");
            }

            object originalItem = this.GetItemInternal(index);
            this.ClearModelParent(originalItem);
            this.SetModelParent(value);
            this.SetItemInternal(index, value);
        }

        internal override bool ContainsImpl(object value)
        {
            if (this.IsUsingItemsSource)
            {
                return this.SourceList.Contains(value);
            }

            return base.ContainsImpl(value);
        }

        internal override int IndexOfImpl(object value)
        {
            if (this.IsUsingItemsSource)
            {
                return this.SourceList.IndexOf(value);
            }

            return base.IndexOfImpl(value);
        }

        internal override IEnumerator<object> GetEnumeratorImpl()
        {
            if (this.IsUsingItemsSource)
            {
                return this.GetEnumeratorPrivateItemsSourceOnly();
            }

            return base.GetEnumeratorImpl();
        }

        private IEnumerator<object> GetEnumeratorPrivateItemsSourceOnly()
        {
            IEnumerator enumerator = this.SourceList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }

        public new event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                base.CollectionChanged += value;
            }
            remove
            {
                base.CollectionChanged -= value;
            }
        }

        internal IEnumerator LogicalChildren
        {
            get
            {
                if (this.IsUsingItemsSource)
                {
                    return EmptyEnumerator.Instance;
                }

                return this.GetEnumerator();
            }
        }

        internal bool IsUsingItemsSource
        {
            get
            {
                return this._isUsingItemsSource;
            }
        }

        internal IEnumerable SourceCollection
        {
            get
            {
                return this._itemsSource;
            }
        }

        internal IList SourceList
        {
            get
            {
                if (this._isUsingListWrapper)
                {
                    return this._listWrapper;
                }

                return (IList)this._itemsSource;
            }
        }

        internal override int CountInternal
        {
            get
            {
                if (this.IsUsingItemsSource)
                {
                    return this.SourceList.Count;
                }
                else
                {
                    return base.CountInternal;
                }
            }
        }

        internal void SetItemsSource(IEnumerable value)
        {
            if (!this.IsUsingItemsSource && this.CountInternal != 0)
            {
                throw new InvalidOperationException("Items collection must be empty before using ItemsSource.");
            }

            this.TryUnsubscribeFromCollectionChangedEvent();

            this._itemsSource = value;
            this._isUsingItemsSource = true;

            this.TrySubscribeToCollectionChangedEvent(value);

            this.InitializeSourceList(value);

            this.UpdateCountProperty();

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        internal void ClearItemsSource()
        {
            if (this.IsUsingItemsSource)
            {
                // return to normal mode
                this.TryUnsubscribeFromCollectionChangedEvent();

                this._itemsSource = null;
                this._listWrapper = null;
                this._isUsingItemsSource = false;
                this._isUsingListWrapper = false;

                this.UpdateCountProperty();

                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        private void InitializeSourceList(IEnumerable sourceCollection)
        {
            IList sourceAsList = sourceCollection as IList;
            if (sourceAsList == null)
            {
                this._listWrapper = new EnumerableWrapper(sourceCollection, this);
                this._isUsingListWrapper = true;
            }
            else
            {
                this._listWrapper = null;
                this._isUsingListWrapper = false;
            }
        }

        private void ValidateCollectionChangedEventArgs(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems.Count != 1)
                    {
                        throw new NotSupportedException("Range actions are not supported.");
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems.Count != 1)
                    {
                        throw new NotSupportedException("Range actions are not supported.");
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                    if (e.NewItems.Count != 1 || e.OldItems.Count != 1)
                    {
                        throw new NotSupportedException("Range actions are not supported.");
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    break;

                default:
                    throw new NotSupportedException(string.Format("Unexpected collection change action '{0}'.", e.Action));
            }
        }

        private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.ValidateCollectionChangedEventArgs(e);

            // Update list wrapper
            if (this._isUsingListWrapper)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        this._listWrapper.Insert(e.NewStartingIndex, e.NewItems[0]);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        this._listWrapper.RemoveAt(e.OldStartingIndex);
                        break;
                    case NotifyCollectionChangedAction.Move:
                        this._listWrapper.Move(e.OldStartingIndex, e.NewStartingIndex);
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        this._listWrapper[e.OldStartingIndex] = e.NewItems[0];
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        this._listWrapper.Refresh();
                        break;
                }
            }

            this.UpdateCountProperty();

            // Raise collection changed
            this.OnCollectionChanged(e);
        }

        private void SetModelParent(object item)
        {
            if (this._modelParent != null)
            {
                this._modelParent.AddLogicalChild(item);
            }
        }

        private void ClearModelParent(object item)
        {
            if (this._modelParent != null)
            {
                this._modelParent.RemoveLogicalChild(item);
            }
        }

        private void TrySubscribeToCollectionChangedEvent(IEnumerable collection)
        {
            if (collection is INotifyCollectionChanged incc)
            {
                _collectionChangedListener = new(this, incc)
                {
                    OnEventAction = static (instance, source, args) => instance.OnSourceCollectionChanged(source, args),
                    OnDetachAction = static (listener, source) => source.CollectionChanged -= listener.OnEvent,
                };
                incc.CollectionChanged += _collectionChangedListener.OnEvent;
            }
        }

        private void TryUnsubscribeFromCollectionChangedEvent()
        {
            if (_collectionChangedListener != null)
            {
                _collectionChangedListener.Detach();
                _collectionChangedListener = null;
            }
        }

        private class EnumerableWrapper : List<object>
        {
            private IEnumerable _sourceCollection;
            private ItemCollection _owner; // unused

            public EnumerableWrapper(IEnumerable source, ItemCollection owner)
            {
                Debug.Assert(source != null);
                Debug.Assert(owner != null);
                this._sourceCollection = source;
                this._owner = owner;

                this.Refresh();
            }

            public void Refresh()
            {
                this.Clear();

                IEnumerator enumerator = this._sourceCollection.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    this.Add(enumerator.Current);
                }
            }

            public void Move(int oldIndex, int newIndex)
            {
                if (oldIndex == newIndex)
                {
                    return;
                }

                var item = this[oldIndex];

                this.RemoveAt(oldIndex);
                this.Insert(newIndex, item);
            }
        }
    }
}