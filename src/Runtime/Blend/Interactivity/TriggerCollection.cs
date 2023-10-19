// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

namespace System.Windows.Interactivity
{
    ///<summary>
    /// Represents a collection of triggers with a shared AssociatedObject and provides change notifications to its contents when that AssociatedObject changes.
    /// </summary>
    public sealed class TriggerCollection : AttachableCollection<TriggerBase>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerCollection"/> class.
        /// </summary>
        /// <remarks>Internal, because this should not be inherited outside this assembly.</remarks>
        internal TriggerCollection()
        {
        }

        /// <summary>
        /// Called immediately after the collection is attached to an AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            foreach (TriggerBase trigger in this)
            {
                trigger.Attach(this.AssociatedObject);
            }
        }

        /// <summary>
        /// Called when the collection is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        protected override void OnDetaching()
        {
            foreach (TriggerBase trigger in this)
            {
                trigger.Detach();
            }
        }

        /// <summary>
        /// Called when a new item is added to the collection.
        /// </summary>
        /// <param name="item">The new item.</param>
        internal override void ItemAdded(TriggerBase item)
        {
            if (this.AssociatedObject != null)
            {
                item.Attach(this.AssociatedObject);
            }
        }

        /// <summary>
        /// Called when an item is removed from the collection.
        /// </summary>
        /// <param name="item">The removed item.</param>
        internal override void ItemRemoved(TriggerBase item)
        {
            if (((IAttachedObject)item).AssociatedObject != null)
            {
                item.Detach();
            }
        }

#if __WPF__
		/// <summary>
		/// Creates a new instance of the <see cref="TriggerCollection"/>.
		/// </summary>
		/// <returns>The new instance.</returns>
		protected override Freezable CreateInstanceCore()
		{
			return new TriggerCollection();
		}
#endif
    }
}
