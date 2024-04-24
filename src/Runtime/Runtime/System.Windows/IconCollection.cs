
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

namespace System.Windows;

/// <summary>
/// Represents a collection of <see cref="Icon"/> instances.
/// </summary>
public sealed class IconCollection : PresentationFrameworkCollection<Icon>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IconCollection"/> class.
    /// </summary>
    public IconCollection()
        : base(false)
    {
    }

    internal override void AddOverride(Icon value) => AddInternal(value);

    internal override void ClearOverride() => ClearInternal();

    internal override Icon GetItemOverride(int index) => GetItemInternal(index);

    internal override void InsertOverride(int index, Icon value) => InsertInternal(index, value);

    internal override void RemoveAtOverride(int index) => RemoveAtInternal(index);

    internal override void SetItemOverride(int index, Icon value) => SetItemInternal(index, value);
}
