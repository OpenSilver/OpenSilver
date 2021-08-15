

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
using System.Collections.Specialized;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <exclude/>
    public class UIElementCollection : PresentationFrameworkCollection<UIElement>
    {
        #region Data

        private readonly UIElement _visualParent;
        private readonly FrameworkElement _logicalParent;
        private SimpleMonitor _monitor = new SimpleMonitor();

        #endregion Data

        #region Constructor

        internal UIElementCollection(UIElement visualParent, FrameworkElement logicalParent)
        {
            if (visualParent == null)
            {
                throw new ArgumentNullException(string.Format("'{0}' must be provided when instantiating '{1}'", "visualParent", this.GetType()));
            }

            this._visualParent = visualParent;
            this._logicalParent = logicalParent;
        }

        #endregion Constructor

        #region Override methods

        internal override void AddOverride(UIElement value)
        {
            this.CheckReentrancy();
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            this.SetLogicalParent(value);
            this.AddInternal(value);

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, this.CountInternal - 1));
        }

        internal override void ClearOverride()
        {
            this.CheckReentrancy();
            int count = this.CountInternal;
            UIElement[] uies = new UIElement[count];
            for (int i = 0; i < count; ++i)
            {
                uies[i] = this.GetItemInternal(i);
            }

            for (int i = 0; i < count; ++i)
            {
                this.ClearLogicalParent(uies[i]);
            }

            this.ClearInternal();

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        internal override UIElement GetItemOverride(int index)
        {
            if (index < 0 || index >= this.CountInternal)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            return this.GetItemInternal(index);
        }

        internal override void InsertOverride(int index, UIElement value)
        {
            this.CheckReentrancy();
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (index < 0 || index > this.CountInternal)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            this.SetLogicalParent(value);
            this.InsertInternal(index, value);

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
        }

        internal override void RemoveAtOverride(int index)
        {
            this.CheckReentrancy();
            if (index < 0 || index >= this.CountInternal)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            UIElement oldChild = this.GetItemInternal(index);
            this.ClearLogicalParent(oldChild);
            this.RemoveAtInternal(index);

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldChild, index));
        }

        internal override bool RemoveOverride(UIElement value)
        {
            this.CheckReentrancy();
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            int index = this.IndexOf(value);
            if (index > -1)
            {
                this.ClearLogicalParent(value);
                this.RemoveAtInternal(index);
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index));
                return true;
            }
            return false;
        }

        internal override void SetItemOverride(int index, UIElement value)
        {
            this.CheckReentrancy();
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (index < 0 || index >= this.CountInternal)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            UIElement oldChild = this.GetItemInternal(index);
            if (oldChild != value)
            {
                this.ClearLogicalParent(oldChild);

                this.SetItemInternal(index, value);

                this.SetLogicalParent(value);

                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldChild, index));
            }
        }

        #endregion Override methods

        #region CollectionChanged event

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

        #endregion CollectionChanged event

        #region Internal methods

        internal void AddRange(IEnumerable<UIElement> children)
        {
            this.CheckReentrancy();

            if (children == null)
            {
                throw new ArgumentNullException("children");
            }

            foreach (UIElement child in children)
            {
                if (child != null)
                {
                    this.SetLogicalParent(child);
                    this.AddInternal(child);
                }
            }

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        internal void RemoveRange(IEnumerable<UIElement> children)
        {
            this.CheckReentrancy();

            if (children == null)
            {
                throw new ArgumentNullException("children");
            }

            foreach (UIElement child in children)
            {
                if (child != null)
                {
                    this.ClearLogicalParent(child);
                    this.RemoveInternal(child);
                }
            }

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void SetLogicalParent(UIElement child)
        {
            if (this._logicalParent != null)
            {
                this._logicalParent.AddLogicalChild(child);
            }
        }

        private void ClearLogicalParent(UIElement child)
        {
            if (this._logicalParent != null)
            {
                this._logicalParent.RemoveLogicalChild(child);
            }
        }

        #endregion Internal methods

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

        #endregion Private classes
    }
}
