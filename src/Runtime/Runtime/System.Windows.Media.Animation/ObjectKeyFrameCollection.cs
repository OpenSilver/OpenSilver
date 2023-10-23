
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
/// Represents a collection of <see cref="ObjectKeyFrame"/> objects that can be 
/// individually accessed by index.
/// </summary>
public sealed class ObjectKeyFrameCollection : PresentationFrameworkCollection<ObjectKeyFrame>, IKeyFrameCollection<object>
{
    public ObjectKeyFrameCollection()
        : base(false)
    {
    }

    internal ObjectKeyFrameCollection(ObjectAnimationUsingKeyFrames owner)
        : base(false)
    {
        Debug.Assert(owner != null);
        owner.ProvideSelfAsInheritanceContext(this, null);
    }

    internal override void AddOverride(ObjectKeyFrame keyFrame) => AddDependencyObjectInternal(keyFrame);

    internal override void ClearOverride() => ClearDependencyObjectInternal();

    internal override void InsertOverride(int index, ObjectKeyFrame keyFrame) => InsertDependencyObjectInternal(index, keyFrame);

    internal override void RemoveAtOverride(int index) => RemoveAtDependencyObjectInternal(index);

    internal override ObjectKeyFrame GetItemOverride(int index) => GetItemInternal(index);

    internal override void SetItemOverride(int index, ObjectKeyFrame keyFrame) => SetItemDependencyObjectInternal(index, keyFrame);

    IKeyFrame<object> IKeyFrameCollection<object>.this[int index] => GetItemInternal(index);
}
