
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

namespace System.Windows.Media
{
    /// <summary>
    ///Represents a collection of <see cref="TimelineMarker"/> objects that can be individually accessed by index.
    /// </summary>
    public sealed class TimelineMarkerCollection : PresentationFrameworkCollection<TimelineMarker>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimelineMarkerCollection"/> class.
        /// </summary>
        public TimelineMarkerCollection() : base(false) { }

        internal override void AddOverride(TimelineMarker value)
        {
            this.AddDependencyObjectInternal(value);
        }

        internal override void ClearOverride()
        {
            this.ClearDependencyObjectInternal();
        }

        internal override TimelineMarker GetItemOverride(int index)
        {
            return this.GetItemInternal(index);
        }

        internal override void InsertOverride(int index, TimelineMarker value)
        {
            this.InsertDependencyObjectInternal(index, value);
        }

        internal override void RemoveAtOverride(int index)
        {
            this.RemoveAtDependencyObjectInternal(index);
        }

        internal override void SetItemOverride(int index, TimelineMarker value)
        {
            this.SetItemDependencyObjectInternal(index, value);
        }
    }
}
