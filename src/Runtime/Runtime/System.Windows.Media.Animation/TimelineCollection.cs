
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
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
    /// <summary>
    /// Represents a collection of <see cref="Timeline"/> objects.
    /// </summary>
    public sealed partial class TimelineCollection : PresentationFrameworkCollection<Timeline>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimelineCollection"/> class.
        /// </summary>
        public TimelineCollection() : base(false) { }

        internal override void AddOverride(Timeline value) => AddDependencyObjectInternal(value);

        internal override void ClearOverride() => ClearDependencyObjectInternal();

        internal override Timeline GetItemOverride(int index) => GetItemInternal(index);

        internal override void InsertOverride(int index, Timeline value) => InsertDependencyObjectInternal(index, value);

        internal override void RemoveAtOverride(int index) => RemoveAtDependencyObjectInternal(index);

        internal override void SetItemOverride(int index, Timeline value) => SetItemDependencyObjectInternal(index, value);
    }
}