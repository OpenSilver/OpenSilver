using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace System.Windows.Controls.DataVisualization.Charting
{
    internal class ResourceDictionaryDispenser : IResourceDictionaryDispenser
    {
        /// <summary>A linked list of ResourceDictionaries dispensed.</summary>
        private LinkedList<ResourceDictionaryDispensedEventArgs> _resourceDictionariesDispensed = new LinkedList<ResourceDictionaryDispensedEventArgs>();
        /// <summary>
        /// A bag of weak references to connected style enumerators.
        /// </summary>
        private WeakReferenceBag<ResourceDictionaryEnumerator> _resourceDictionaryEnumerators = new WeakReferenceBag<ResourceDictionaryEnumerator>();
        /// <summary>
        /// Value indicating whether to ignore that the enumerator has
        /// dispensed a ResourceDictionary.
        /// </summary>
        private bool _ignoreResourceDictionaryDispensedByEnumerator;
        /// <summary>The list of ResourceDictionaries of rotate.</summary>
        private IList<ResourceDictionary> _resourceDictionaries;
        /// <summary>The parent of the ResourceDictionaryDispenser.</summary>
        private IResourceDictionaryDispenser _parent;

        /// <summary>
        /// Gets or sets the list of ResourceDictionaries to rotate.
        /// </summary>
        public IList<ResourceDictionary> ResourceDictionaries
        {
            get
            {
                return this._resourceDictionaries;
            }
            set
            {
                if (value == this._resourceDictionaries)
                    return;
                INotifyCollectionChanged resourceDictionaries1 = this._resourceDictionaries as INotifyCollectionChanged;
                if (resourceDictionaries1 != null)
                    resourceDictionaries1.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.ResourceDictionariesCollectionChanged);
                this._resourceDictionaries = value;
                INotifyCollectionChanged resourceDictionaries2 = this._resourceDictionaries as INotifyCollectionChanged;
                if (resourceDictionaries2 != null)
                    resourceDictionaries2.CollectionChanged += new NotifyCollectionChangedEventHandler(this.ResourceDictionariesCollectionChanged);
                this.Reset();
            }
        }

        /// <summary>
        /// This method is raised when the ResourceDictionaries collection is changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        private void ResourceDictionariesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && this.ResourceDictionaries.Count - e.NewItems.Count == e.NewStartingIndex)
                return;
            this.Reset();
        }

        /// <summary>
        /// Event that is invoked when the ResourceDictionaryDispenser's contents have changed.
        /// </summary>
        public event EventHandler ResourceDictionariesChanged;

        /// <summary>
        /// Gets or sets the parent of the ResourceDictionaryDispenser.
        /// </summary>
        public IResourceDictionaryDispenser Parent
        {
            get
            {
                return this._parent;
            }
            set
            {
                if (this._parent == value)
                    return;
                if (null != this._parent)
                    this._parent.ResourceDictionariesChanged -= new EventHandler(this.ParentResourceDictionariesChanged);
                this._parent = value;
                if (null != this._parent)
                    this._parent.ResourceDictionariesChanged += new EventHandler(this.ParentResourceDictionariesChanged);
                this.OnParentChanged();
            }
        }

        /// <summary>
        /// Resets the state of the ResourceDictionaryDispenser and its enumerators.
        /// </summary>
        private void Reset()
        {
            this.OnResetting();
            EventHandler dictionariesChanged = this.ResourceDictionariesChanged;
            if (null == dictionariesChanged)
                return;
            dictionariesChanged((object)this, EventArgs.Empty);
        }

        /// <summary>
        /// Unregisters an enumerator so that it can be garbage collected.
        /// </summary>
        /// <param name="enumerator">The enumerator.</param>
        internal void Unregister(ResourceDictionaryEnumerator enumerator)
        {
            this._resourceDictionaryEnumerators.Remove(enumerator);
        }

        /// <summary>
        /// Returns a rotating enumerator of ResourceDictionary objects that coordinates
        /// with the dispenser object to ensure that no two enumerators are on the same
        /// item. If the dispenser is reset or its collection is changed then the
        /// enumerators are also reset.
        /// </summary>
        /// <param name="predicate">A predicate that returns a value indicating
        /// whether to return an item.</param>
        /// <returns>An enumerator of ResourceDictionaries.</returns>
        public IEnumerator<ResourceDictionary> GetResourceDictionariesWhere(Func<ResourceDictionary, bool> predicate)
        {
            ResourceDictionaryEnumerator dictionaryEnumerator = new ResourceDictionaryEnumerator(this, predicate);
            this._ignoreResourceDictionaryDispensedByEnumerator = true;
            try
            {
                foreach (ResourceDictionaryDispensedEventArgs e in this._resourceDictionariesDispensed)
                    dictionaryEnumerator.ResourceDictionaryDispenserResourceDictionaryDispensed((object)this, e);
            }
            finally
            {
                this._ignoreResourceDictionaryDispensedByEnumerator = false;
            }
            this._resourceDictionaryEnumerators.Add(dictionaryEnumerator);
            return (IEnumerator<ResourceDictionary>)dictionaryEnumerator;
        }

        /// <summary>
        /// This method is raised when an enumerator dispenses a ResourceDictionary.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        internal void EnumeratorResourceDictionaryDispensed(object sender, ResourceDictionaryDispensedEventArgs e)
        {
            if (this._ignoreResourceDictionaryDispensedByEnumerator)
                return;
            this.OnEnumeratorResourceDictionaryDispensed((object)this, e);
        }

        /// <summary>Raises the ParentChanged event.</summary>
        private void OnParentChanged()
        {
            foreach (ResourceDictionaryEnumerator dictionaryEnumerator in this._resourceDictionaryEnumerators)
                dictionaryEnumerator.ResourceDictionaryDispenserParentChanged();
        }

        /// <summary>
        /// Raises the EnumeratorResourceDictionaryDispensed event.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="args">Information about the event.</param>
        private void OnEnumeratorResourceDictionaryDispensed(object source, ResourceDictionaryDispensedEventArgs args)
        {
            this._resourceDictionariesDispensed.Remove(args);
            this._resourceDictionariesDispensed.AddLast(args);
            foreach (ResourceDictionaryEnumerator dictionaryEnumerator in this._resourceDictionaryEnumerators)
                dictionaryEnumerator.ResourceDictionaryDispenserResourceDictionaryDispensed(source, args);
        }

        /// <summary>This method raises the EnumeratorsResetting event.</summary>
        private void OnResetting()
        {
            this._resourceDictionariesDispensed.Clear();
            foreach (ResourceDictionaryEnumerator dictionaryEnumerator in this._resourceDictionaryEnumerators)
                dictionaryEnumerator.ResourceDictionaryDispenserResetting();
        }

        /// <summary>
        /// Handles the Parent's ResourceDictionariesChanged event.
        /// </summary>
        /// <param name="sender">Parent instance.</param>
        /// <param name="e">Event args.</param>
        private void ParentResourceDictionariesChanged(object sender, EventArgs e)
        {
            this.Reset();
        }
    }
}
