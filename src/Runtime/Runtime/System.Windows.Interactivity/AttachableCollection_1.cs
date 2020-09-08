

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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if !MIGRATION
using Windows.UI.Xaml;
#endif

namespace System.Windows.Interactivity
{
    /// <summary>
    /// Represents a collection of IAttachedObject with a shared AssociatedObject
    /// and provides change notifications to its contents when that AssociatedObject
    /// changes.
    /// </summary>
    public abstract partial class AttachableCollection<T> : DependencyObjectCollection<T>, IAttachedObject where T : DependencyObject, IAttachedObject
    {
        private DependencyObject _associatedObject;
        private List<T> _unorderedCopy;

        internal AttachableCollection()
        {
            this._unorderedCopy = new List<T>();
            this.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnCollectionChanged);
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove ||
                e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (T item in e.OldItems)
                {
                    this._unorderedCopy.Remove(item);
                    item.Detach();
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Add ||
                e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (T item in e.NewItems)
                {
                    this._unorderedCopy.Add(item);
                    if (this._associatedObject != null)
                    {
                        item.Attach(this._associatedObject);
                    }
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (T item in this._unorderedCopy)
                {
                    item.Detach();
                }
                this._unorderedCopy.Clear();
                foreach (T item in this)
                {
                    this._unorderedCopy.Add(item);
                    if (this._associatedObject != null)
                    {
                        item.Attach(this._associatedObject);
                    }
                }
            }
        }

        /// <summary>
        /// The object on which the collection is hosted.
        /// </summary>
        public DependencyObject AssociatedObject
        {
            get { return _associatedObject; }
        }

        DependencyObject IAttachedObject.AssociatedObject
        {
            get { return this.AssociatedObject; }
        }

        /// <summary>
        /// Attaches to the specified object.
        /// </summary>
        /// <param name="dependencyObject">The object to attach to.</param>
        /// <exception cref="InvalidOperationException">
        /// The IAttachedObject is already attached to a different object.
        /// </exception>
        public void Attach(DependencyObject dependencyObject)
        {
            if (_associatedObject != null)
            {
                throw new InvalidOperationException("The AttachableCollection is already attached to a different object.");
            }
            _associatedObject = dependencyObject;
            foreach (T element in this)
            {
                element.Attach(dependencyObject);
            }
            OnAttached();
        }

        /// <summary>
        /// Detaches this instance from its associated object.
        /// </summary>
        public void Detach()
        {
            OnDetaching();
            foreach (T element in this)
            {
                element.Detach();
            }
            _associatedObject = null;
        }

        /// <summary>
        /// Called immediately after the collection is attached to an AssociatedObject.
        /// </summary>
        protected abstract void OnAttached();

        /// <summary>
        /// Called when the collection is being detached from its AssociatedObject, but
        /// before it has actually occurred.
        /// </summary>
        protected abstract void OnDetaching();
    }
}
