
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

using OpenSilver.Internal.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public sealed class ItemCollection : PresentationFrameworkCollection<object>, INotifyCollectionChanged
    {
        #region Data

        private SimpleMonitor _monitor = new SimpleMonitor();

        private bool _isUsingItemsSource;
        private IEnumerable _itemsSource; // base collection

        private bool _isUsingListWrapper;
        private EnumerableWrapper _listWrapper;

        private FrameworkElement _modelParent;

        #endregion Data

        #region Constructor

        internal ItemCollection(FrameworkElement parent)
        {
            this._modelParent = parent;
        }

        #endregion Constructor

        #region Overriden Methods

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
            this.CheckReentrancy();
            if (this.IsUsingItemsSource)
            {
                throw new InvalidOperationException("Operation is not valid while ItemsSource is in use. Access and modify elements with ItemsControl.ItemsSource instead.");
            }
            this.SetModelParent(value);
            this.AddInternal(value);
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, this.CountInternal - 1));
        }

        internal override void ClearOverride()
        {
            this.CheckReentrancy();
            if (this.IsUsingItemsSource)
            {
                throw new InvalidOperationException("Operation is not valid while ItemsSource is in use. Access and modify elements with ItemsControl.ItemsSource instead.");
            }
            foreach (var item in this)
            {
                this.ClearModelParent(item);
            }
            this.ClearInternal();
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        internal override void InsertOverride(int index, object value)
        {
            this.CheckReentrancy();
            if (this.IsUsingItemsSource)
            {
                throw new InvalidOperationException("Operation is not valid while ItemsSource is in use. Access and modify elements with ItemsControl.ItemsSource instead.");
            }
            this.SetModelParent(value);
            this.InsertInternal(index, value);
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
        }

        internal override void RemoveAtOverride(int index)
        {
            this.CheckReentrancy();
            if (this.IsUsingItemsSource)
            {
                throw new InvalidOperationException("Operation is not valid while ItemsSource is in use. Access and modify elements with ItemsControl.ItemsSource instead.");
            }
            object removedItem = this.GetItemInternal(index);
            this.ClearModelParent(removedItem);
            this.RemoveAtInternal(index);
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItem, index));
        }

        internal override bool RemoveOverride(object value)
        {
            this.CheckReentrancy();
            if (this.IsUsingItemsSource)
            {
                throw new InvalidOperationException("Operation is not valid while ItemsSource is in use. Access and modify elements with ItemsControl.ItemsSource instead.");
            }
            int index = this.IndexOf(value);
            if (index > -1)
            {
                object oldItem = this.GetItemInternal(index);
                this.ClearModelParent(oldItem);
                this.RemoveAtInternal(index);
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem, index));
                return true;
            }
            return false;
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
            this.CheckReentrancy();
            if (this.IsUsingItemsSource)
            {
                throw new InvalidOperationException("Operation is not valid while ItemsSource is in use. Access and modify elements with ItemsControl.ItemsSource instead.");
            }
            object originalItem = this.GetItemInternal(index);
            this.ClearModelParent(originalItem);
            this.SetModelParent(value);
            this.SetItemInternal(index, value);
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, originalItem, index));
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

        #endregion Overriden Methods

        #region INotifyCollectionChanged

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.CollectionChanged != null)
            {
                using (this.BlockReentrancy())
                {
                    this.CollectionChanged(this, e);
                }
            }
        }

        /// <summary>
        /// Disallow reentrant attempts to change this collection. E.g. a event handler
        /// of the CollectionChanged event is not allowed to make changes to this collection.
        /// </summary>
        /// <remarks>
        /// typical usage is to wrap e.g. a OnCollectionChanged call with a using() scope:
        /// <code>
        ///         using (BlockReentrancy())
        ///         {
        ///             CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, item, index));
        ///         }
        /// </code>
        /// </remarks>
        private IDisposable BlockReentrancy()
        {
            _monitor.Enter();
            return _monitor;
        }

        /// <summary> Check and assert for reentrant attempts to change this collection. </summary>
        /// <exception cref="InvalidOperationException"> raised when changing the collection
        /// while another collection change is still being notified to other listeners </exception>
        private void CheckReentrancy()
        {
            if (_monitor.Busy)
            {
                // we can allow changes if there's only one listener - the problem
                // only arises if reentrant changes make the original event args
                // invalid for later listeners.  This keeps existing code working
                // (e.g. Selector.SelectedItems).
                if ((CollectionChanged != null) && (CollectionChanged.GetInvocationList().Length > 1))
                    throw new InvalidOperationException("RowDefinitionCollection Reentrancy not allowed");
            }
        }

        #endregion INotifyCollectionChanged

        #region Internal API

        #region Internal Properties

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

        #endregion Internal Properties

        #region Internal Methods

        internal void SetItemsSource(IEnumerable value)
        {
            if (!this.IsUsingItemsSource && this.CountInternal != 0)
            {
                throw new InvalidOperationException("Items collection must be empty before using ItemsSource.");
            }

            this.TryUnsubscribeFromCollectionChangedEvent(this._itemsSource);

            this._itemsSource = value;
            this._isUsingItemsSource = true;

            this.TrySubscribeToCollectionChangedEvent(value);
            
            this.InitializeSourceList(value);

            this.UpdateCountProperty(this.CountInternal);

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        internal void ClearItemsSource()
        {
            if (this.IsUsingItemsSource)
            {
                // return to normal mode
                this.TryUnsubscribeFromCollectionChangedEvent(this._itemsSource);

                this._itemsSource = null;
                this._listWrapper = null;
                this._isUsingItemsSource = false;
                this._isUsingListWrapper = false;

                this.UpdateCountProperty(this.CountInternal);

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
                    case NotifyCollectionChangedAction.Replace:
                        this._listWrapper[e.OldStartingIndex] = e.NewItems[0];
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        this._listWrapper.Refresh();
                        break;
                }
            }

            this.UpdateCountProperty(this.CountInternal);

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
            INotifyCollectionChanged incc = collection as INotifyCollectionChanged;
            if (incc != null)
            {
                incc.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnSourceCollectionChanged);
            }
        }

        private void TryUnsubscribeFromCollectionChangedEvent(IEnumerable collection)
        {
            INotifyCollectionChanged incc = collection as INotifyCollectionChanged;
            if (incc != null)
            {
                incc.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.OnSourceCollectionChanged);
            }
        }

        #endregion Internal Methods

        #endregion Internal API

        #region Private classes

        private class SimpleMonitor : IDisposable
        {
            public void Enter()
            {
                ++_busyCount;
            }

            public void Dispose()
            {
                --_busyCount;
            }

            public bool Busy { get { return _busyCount > 0; } }

            int _busyCount;
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
        }

        #endregion Private classes
    }
}