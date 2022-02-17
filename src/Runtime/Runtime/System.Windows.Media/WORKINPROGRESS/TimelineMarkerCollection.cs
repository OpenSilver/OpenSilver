
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
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    ///Represents a collection of <see cref="TimelineMarker"/> objects that can be individually accessed by index.
    /// </summary>
    [OpenSilver.NotImplemented]
    public sealed class TimelineMarkerCollection : PresentationFrameworkCollection<TimelineMarker>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimelineMarkerCollection"/> class.
        /// </summary>
        [OpenSilver.NotImplemented]
        public TimelineMarkerCollection() : base(true) { }

        [OpenSilver.NotImplemented]
        internal override void AddOverride(TimelineMarker value) { }

        [OpenSilver.NotImplemented]
        internal override void ClearOverride() { }

        [OpenSilver.NotImplemented]
        internal override TimelineMarker GetItemOverride(int index)
        {
            throw new NotImplementedException();
        }

        [OpenSilver.NotImplemented]
        internal override void InsertOverride(int index, TimelineMarker value) { }

        [OpenSilver.NotImplemented]
        internal override void RemoveAtOverride(int index) { }

        [OpenSilver.NotImplemented]
        internal override void SetItemOverride(int index, TimelineMarker value) { }
    }
}
