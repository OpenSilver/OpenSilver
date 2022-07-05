using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace System.Windows.Controls.DataVisualization.Charting
{
    internal class ResourceDictionaryEnumerator : IEnumerator<ResourceDictionary>, IDisposable, IEnumerator
    {
        /// <summary>
        /// The index of current item in the ResourceDictionaryDispenser's list.
        /// </summary>
        private int? index;
        /// <summary>The parent enumerator.</summary>
        private IEnumerator<ResourceDictionary> _parentEnumerator;

        /// <summary>Gets or sets the current ResourceDictionary.</summary>
        private ResourceDictionary CurrentResourceDictionary { get; set; }

        /// <summary>Gets the parent enumerator.</summary>
        private IEnumerator<ResourceDictionary> ParentEnumerator
        {
            get
            {
                if (this._parentEnumerator == null && this.ResourceDictionaryDispenser.Parent != null)
                    this._parentEnumerator = this.ResourceDictionaryDispenser.Parent.GetResourceDictionariesWhere(this.Predicate);
                return this._parentEnumerator;
            }
        }

        /// <summary>
        /// Initializes a new instance of a ResourceDictionaryEnumerator.
        /// </summary>
        /// <param name="dispenser">The dispenser that dispensed this
        /// ResourceDictionaryEnumerator.</param>
        /// <param name="predicate">A predicate used to determine which
        /// ResourceDictionaries to return.</param>
        public ResourceDictionaryEnumerator(ResourceDictionaryDispenser dispenser, Func<ResourceDictionary, bool> predicate)
        {
            this.ResourceDictionaryDispenser = dispenser;
            this.Predicate = predicate;
        }

        /// <summary>Called when the parent has changed.</summary>
        internal void ResourceDictionaryDispenserParentChanged()
        {
            this._parentEnumerator = (IEnumerator<ResourceDictionary>)null;
        }

        /// <summary>
        /// Returns the index of the next suitable style in the list.
        /// </summary>
        /// <param name="startIndex">The index at which to start looking.</param>
        /// <returns>The index of the next suitable ResourceDictionary.</returns>
        private int? GetIndexOfNextSuitableResourceDictionary(int startIndex)
        {
            if (this.ResourceDictionaryDispenser.ResourceDictionaries == null || this.ResourceDictionaryDispenser.ResourceDictionaries.Count == 0)
                return new int?();
            if (startIndex >= this.ResourceDictionaryDispenser.ResourceDictionaries.Count)
                startIndex = 0;
            int index = startIndex;
            while (!this.Predicate(this.ResourceDictionaryDispenser.ResourceDictionaries[index]))
            {
                index = (index + 1) % this.ResourceDictionaryDispenser.ResourceDictionaries.Count;
                if (startIndex == index)
                    return new int?();
            }
            return new int?(index);
        }

        /// <summary>Resets the dispenser.</summary>
        internal void ResourceDictionaryDispenserResetting()
        {
            if (this.ShouldRetrieveFromParentEnumerator)
                return;
            this.index = new int?();
        }

        /// <summary>
        /// Gets or sets a predicate that returns a value indicating whether a
        /// ResourceDictionary should be returned by this enumerator.
        /// </summary>
        /// <returns>A value indicating whether a ResourceDictionary can be returned by this
        /// enumerator.</returns>
        private Func<ResourceDictionary, bool> Predicate { get; set; }

        /// <summary>
        /// This method is invoked when one of the related enumerator's
        /// dispenses.  The enumerator checks to see if the item
        /// dispensed would've been the next item it would have returned.  If
        /// so it updates it's index to the position after the previously
        /// returned item.
        /// </summary>
        /// <param name="sender">The ResourceDictionaryDispenser.</param>
        /// <param name="e">Information about the event.</param>
        internal void ResourceDictionaryDispenserResourceDictionaryDispensed(object sender, ResourceDictionaryDispensedEventArgs e)
        {
            if (this.ShouldRetrieveFromParentEnumerator || !this.Predicate(e.ResourceDictionary))
                return;
            int? nullable = this.index;
            nullable = this.GetIndexOfNextSuitableResourceDictionary(nullable ?? 0);
            if ((nullable ?? -1) == e.Index)
                this.index = new int?((e.Index + 1) % this.ResourceDictionaryDispenser.ResourceDictionaries.Count);
        }

        /// <summary>Raises the EnumeratorResourceDictionaryDispensed.</summary>
        /// <param name="args">Information about the ResourceDictionary dispensed.</param>
        protected virtual void OnStyleDispensed(ResourceDictionaryDispensedEventArgs args)
        {
            this.ResourceDictionaryDispenser.EnumeratorResourceDictionaryDispensed((object)this, args);
        }

        /// <summary>Gets the dispenser that dispensed this enumerator.</summary>
        public ResourceDictionaryDispenser ResourceDictionaryDispenser { get; private set; }

        /// <summary>Gets the current ResourceDictionary.</summary>
        public ResourceDictionary Current
        {
            get
            {
                return this.CurrentResourceDictionary;
            }
        }

        object IEnumerator.Current => (object)this.CurrentResourceDictionary;

        /// <summary>Moves to the next ResourceDictionary.</summary>
        /// <returns>A value indicating whether there are any more suitable
        /// ResourceDictionary.</returns>
        public bool MoveNext()
        {
            if (this.ShouldRetrieveFromParentEnumerator && this.ParentEnumerator != null)
            {
                bool flag = this.ParentEnumerator.MoveNext();
                if (flag)
                    this.CurrentResourceDictionary = this.ParentEnumerator.Current;
                return flag;
            }
            this.index = this.GetIndexOfNextSuitableResourceDictionary(this.index ?? 0);
            if (!this.index.HasValue)
            {
                this.CurrentResourceDictionary = (ResourceDictionary)null;
                this.Dispose();
                return false;
            }
            this.CurrentResourceDictionary = this.ResourceDictionaryDispenser.ResourceDictionaries[this.index.Value];
            this.OnStyleDispensed(new ResourceDictionaryDispensedEventArgs(this.index.Value, this.CurrentResourceDictionary));
            return true;
        }

        /// <summary>
        /// Gets a value indicating whether a enumerator should return ResourceDictionaries
        /// from its parent enumerator.
        /// </summary>
        private bool ShouldRetrieveFromParentEnumerator
        {
            get
            {
                return this.ResourceDictionaryDispenser.ResourceDictionaries == null;
            }
        }

        /// <summary>Resets the enumerator.</summary>
        public void Reset()
        {
            throw new NotSupportedException("Can't reset enumerator, reset dispenser instead");
        }

        /// <summary>Stops listening to the dispenser.</summary>
        public void Dispose()
        {
            if (this._parentEnumerator != null)
                this._parentEnumerator.Dispose();
            this.ResourceDictionaryDispenser.Unregister(this);
            GC.SuppressFinalize((object)this);
        }
    }
}
