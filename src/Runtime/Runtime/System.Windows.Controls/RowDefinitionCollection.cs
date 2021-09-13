

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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <exclude/>
    public sealed partial class RowDefinitionCollection : PresentationFrameworkCollection<RowDefinition>
    {
        #region Data

        internal Grid _parentGrid;
        private SimpleMonitor _monitor = new SimpleMonitor();

        #endregion

        #region Constructors

        internal RowDefinitionCollection()
        {

        }

        internal RowDefinitionCollection(Grid parent)
        {
            this._parentGrid = parent;
            parent.ProvideSelfAsInheritanceContext(this, null);
        }

        #endregion

        #region Events (CollectionChanged)

        internal event NotifyCollectionChangedEventHandler CollectionChangedInternal;

        #endregion

        #region Overriden Methods

        internal override void AddOverride(RowDefinition value)
        {
            this.CheckReentrancy();
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            this.AddDependencyObjectInternal(value);
            value.Parent = this._parentGrid;
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, this.CountInternal - 1));
        }

        internal override void ClearOverride()
        {
            this.CheckReentrancy();
            if (this._parentGrid != null)
            {
                foreach (RowDefinition column in this)
                {
                    column.Parent = null;
                }
            }
            this.ClearDependencyObjectInternal();
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        internal override void InsertOverride(int index, RowDefinition value)
        {
            this.CheckReentrancy();
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            value.Parent = this._parentGrid;
            this.InsertDependencyObjectInternal(index, value);
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
        }

        internal override void RemoveAtOverride(int index)
        {
            this.CheckReentrancy();
            RowDefinition removedRow = this.GetItemInternal(index);
            removedRow.Parent = null;
            this.RemoveAtDependencyObjectInternal(index);
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedRow, index));
        }

        internal override bool RemoveOverride(RowDefinition value)
        {
            this.CheckReentrancy();
            int index = this.IndexOf(value);
            if (index > -1)
            {
                this.GetItemInternal(index).Parent = null;
                this.RemoveAtDependencyObjectInternal(index);
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index));
                return true;
            }
            return false;
        }

        internal override RowDefinition GetItemOverride(int index)
        {
            return this.GetItemInternal(index);
        }

        internal override void SetItemOverride(int index, RowDefinition value)
        {
            this.CheckReentrancy();
            RowDefinition originalItem = this.GetItemInternal(index);
            originalItem.Parent = null;
            this.SetItemDependencyObjectInternal(index, value);
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, originalItem, index));
        }

        #endregion

        #region Private Methods

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.CollectionChangedInternal != null)
            {
                using (this.BlockReentrancy())
                {
                    this.CollectionChangedInternal(this, e);
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
                if ((CollectionChangedInternal != null) && (CollectionChangedInternal.GetInvocationList().Length > 1))
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
