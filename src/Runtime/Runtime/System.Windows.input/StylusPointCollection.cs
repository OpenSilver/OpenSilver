
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

using System.Collections.Specialized;
using OpenSilver.Internal;

namespace System.Windows.Input
{
    /// <summary>
    /// Represents a collection of related <see cref="StylusPoint"/> objects.
    /// </summary>
    public sealed class StylusPointCollection : PresentationFrameworkCollection<StylusPoint>
    {
        private readonly CollectionChangedHelper _collectionChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="StylusPointCollection" /> class.
        /// </summary>
        public StylusPointCollection()
        {
            _collectionChanged = new(this);
        }

        /// <summary>
        /// Adds a collection of <see cref="StylusPoint"/> objects to the collection.
        /// </summary>
        /// <param name="stylusPoints">
        /// The collection of <see cref="StylusPoint"/> objects to add to the collection.
        /// </param>
        public void Add(StylusPointCollection stylusPoints)
        {
            if (stylusPoints is null)
            {
                throw new ArgumentNullException(nameof(stylusPoints));
            }

            foreach (StylusPoint point in stylusPoints.InternalItems)
            {
                Add(point);
            }
        }

        internal event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add => _collectionChanged.CollectionChanged += value;
            remove => _collectionChanged.CollectionChanged -= value;
        }

        internal override void AddOverride(StylusPoint point)
        {
            _collectionChanged.CheckReentrancy();
            AddInternal(point);
            _collectionChanged.OnCollectionChanged(NotifyCollectionChangedAction.Add, point, InternalCount - 1);
        }

        internal override void ClearOverride()
        {
            _collectionChanged.CheckReentrancy();
            ClearInternal();
            _collectionChanged.OnCollectionReset();
        }

        internal override void RemoveAtOverride(int index)
        {
            _collectionChanged.CheckReentrancy();
            StylusPoint removedItem = GetItemInternal(index);
            RemoveAtInternal(index);
            _collectionChanged.OnCollectionChanged(NotifyCollectionChangedAction.Remove, removedItem, index);
        }

        internal override void InsertOverride(int index, StylusPoint point)
        {
            _collectionChanged.CheckReentrancy();
            InsertInternal(index, point);
            _collectionChanged.OnCollectionChanged(NotifyCollectionChangedAction.Add, point, index);
        }

        internal override StylusPoint GetItemOverride(int index) => GetItemInternal(index);

        internal override void SetItemOverride(int index, StylusPoint point)
        {
            _collectionChanged.CheckReentrancy();
            StylusPoint originalItem = GetItemInternal(index);
            SetItemInternal(index, point);
            _collectionChanged.OnCollectionChanged(NotifyCollectionChangedAction.Replace, originalItem, point, index);
        }
    }
}
