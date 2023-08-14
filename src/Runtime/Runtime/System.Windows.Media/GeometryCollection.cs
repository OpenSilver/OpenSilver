
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
    /// Represents a collection of <see cref="Geometry"/> objects.
    /// </summary>
    public sealed class GeometryCollection : PresentationFrameworkCollection<Geometry>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryCollection"/> class.
        /// </summary>
        public GeometryCollection()
            : base(true)
        {
        }

        internal override void AddOverride(Geometry value) => AddDependencyObjectInternal(value);

        internal override void ClearOverride() => ClearDependencyObjectInternal();

        internal override void InsertOverride(int index, Geometry value) => InsertDependencyObjectInternal(index, value);

        internal override void RemoveAtOverride(int index) => RemoveAtDependencyObjectInternal(index);

        internal override Geometry GetItemOverride(int index) => GetItemInternal(index);

        internal override void SetItemOverride(int index, Geometry value) => SetItemDependencyObjectInternal(index, value);
    }
}
