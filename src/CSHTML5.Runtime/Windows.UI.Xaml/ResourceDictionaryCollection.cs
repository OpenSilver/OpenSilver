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
using System.Collections.Specialized;
using System.Diagnostics;


#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    internal class ResourceDictionaryCollection : PresentationFrameworkCollection<ResourceDictionary>
    {
        #region Data

        private ResourceDictionary _owner;
        private SimpleMonitor _monitor = new SimpleMonitor();

        #endregion

        #region Constructor

        internal ResourceDictionaryCollection(ResourceDictionary owner)
        {
            Debug.Assert(owner != null, "ResourceDictionaryCollection's owner cannot be null");
            this._owner = owner;
        }

        #endregion

        #region Overriden Methods

        internal override void AddOverride(ResourceDictionary value)
        {
            this.CheckReentrancy();
            this.CheckValue(value);
            this.AddInternal(value);
            value._parentDictionary = this._owner;
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, this.CountInternal));
        }

        internal override void ClearOverride()
        {
            this.CheckReentrancy();
            foreach (ResourceDictionary dictionary in this)
            {
                dictionary._parentDictionary = null;
                this._owner.RemoveParentOwners(dictionary);
            }
            this.ClearInternal();
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        internal override void InsertOverride(int index, ResourceDictionary value)
        {
            this.CheckReentrancy();
            this.CheckValue(value);
            this.InsertInternal(index, value);
            value._parentDictionary = this._owner;
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
        }

        internal override void RemoveAtOverride(int index)
        {
            this.CheckReentrancy();
            ResourceDictionary removedItem = this.GetItemInternal(index);
            this.RemoveAtInternal(index);
            removedItem._parentDictionary = null;
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItem, index));
        }

        internal override bool RemoveOverride(ResourceDictionary value)
        {
            this.CheckReentrancy();
            int index = this.IndexOf(value);
            if (index > -1)
            {
                ResourceDictionary removedItem = this.GetItemInternal(index);
                this.RemoveAtInternal(index);
                removedItem._parentDictionary = null;
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItem, index));
                return true;
            }
            return false;
        }

        internal override void SetItemOverride(int index, ResourceDictionary value)
        {
            this.CheckReentrancy();
            this.CheckValue(value);
            ResourceDictionary originalItem = this.GetItemInternal(index);
            this.SetItemInternal(index, value);
            originalItem._parentDictionary = null;
            value._parentDictionary = this._owner;
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, originalItem, index));
        }

        #endregion

        #region Events (CollectionChanged)

        internal event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region Private Methods

        private void CheckValue(ResourceDictionary value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (value._parentDictionary != null)
            {
                throw new InvalidOperationException("Element is already the child of another element.");
            }
        }

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

        #endregion

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

        #endregion
    }
}
