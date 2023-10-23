
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

using System.Diagnostics;
using OpenSilver.Internal.Media.Animation;

namespace System.Windows.Media.Animation;

/// <summary>
/// Represents a collection of <see cref="PointKeyFrame"/> objects that can 
/// be individually accessed by index.
/// </summary>
public sealed class PointKeyFrameCollection : PresentationFrameworkCollection<PointKeyFrame>, IKeyFrameCollection<Point>
{
    public PointKeyFrameCollection()
        : base(false)
    {
    }

    internal PointKeyFrameCollection(PointAnimationUsingKeyFrames owner)
        : base(false)
    {
        Debug.Assert(owner is not null);
        owner.ProvideSelfAsInheritanceContext(this, null);
    }

    internal override void AddOverride(PointKeyFrame value) => AddDependencyObjectInternal(value);

    internal override void ClearOverride() => ClearDependencyObjectInternal();

    internal override PointKeyFrame GetItemOverride(int index) => GetItemInternal(index);

    internal override void InsertOverride(int index, PointKeyFrame value) => InsertDependencyObjectInternal(index, value);

    internal override void RemoveAtOverride(int index) => RemoveAtDependencyObjectInternal(index);

    internal override void SetItemOverride(int index, PointKeyFrame value) => SetItemDependencyObjectInternal(index, value);

    IKeyFrame<Point> IKeyFrameCollection<Point>.this[int index] => GetItemInternal(index);
}
