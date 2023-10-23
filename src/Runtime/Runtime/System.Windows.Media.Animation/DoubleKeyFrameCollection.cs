
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
/// Represents a collection of <see cref="DoubleKeyFrame"/> objects that can 
/// be individually accessed by index.
/// </summary>
public sealed class DoubleKeyFrameCollection : PresentationFrameworkCollection<DoubleKeyFrame>, IKeyFrameCollection<double>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleKeyFrameCollection"/> class.
    /// </summary>
    public DoubleKeyFrameCollection()
        : base(false)
    {
    }

    internal DoubleKeyFrameCollection(DoubleAnimationUsingKeyFrames owner)
        : base(false)
    {
        Debug.Assert(owner is not null);
        owner.ProvideSelfAsInheritanceContext(this, null);
    }

    internal override void AddOverride(DoubleKeyFrame keyFrame) => AddDependencyObjectInternal(keyFrame);

    internal override void ClearOverride() => ClearDependencyObjectInternal();

    internal override void InsertOverride(int index, DoubleKeyFrame keyFrame) => InsertDependencyObjectInternal(index, keyFrame);

    internal override void RemoveAtOverride(int index) => RemoveAtDependencyObjectInternal(index);

    internal override DoubleKeyFrame GetItemOverride(int index) => GetItemInternal(index);

    internal override void SetItemOverride(int index, DoubleKeyFrame keyFrame) => SetItemDependencyObjectInternal(index, keyFrame);

    IKeyFrame<double> IKeyFrameCollection<double>.this[int index] => GetItemInternal(index);
}
