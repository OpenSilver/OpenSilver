
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

#if MIGRATION
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
    /// <summary>
    /// Represents a collection of related <see cref="StylusPoint"/> objects.
    /// </summary>
    public sealed class StylusPointCollection : PresentationFrameworkCollection<StylusPoint>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StylusPointCollection" /> class.
        /// </summary>
        public StylusPointCollection()
            : base (true)
        {
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

            foreach (StylusPoint point in stylusPoints)
            {
                Add(point);
            }
        }

        internal override void AddOverride(StylusPoint point)
        {
            AddInternal(point);
        }

        internal override void ClearOverride()
        {
            ClearInternal();
        }

        internal override void RemoveAtOverride(int index)
        {
            RemoveAtInternal(index);
        }

        internal override void InsertOverride(int index, StylusPoint point)
        {
            InsertInternal(index, point);
        }

        internal override StylusPoint GetItemOverride(int index)
        {
            return GetItemInternal(index);
        }

        internal override void SetItemOverride(int index, StylusPoint point)
        {
            SetItemInternal(index, point);
        }
    }
}
