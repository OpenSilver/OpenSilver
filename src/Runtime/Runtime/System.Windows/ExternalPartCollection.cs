
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
/// Represents a collection of <see cref="ExternalPart"/> instances that indicate
/// parts of a Silverlight application that are external to the application package
/// (.xap file).
/// </summary>
public sealed class ExternalPartCollection : PresentationFrameworkCollection<ExternalPart>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExternalPartCollection"/> class.
    /// </summary>
    public ExternalPartCollection()
        : base(false)
    {
    }

    internal override void AddOverride(ExternalPart value) => AddInternal(value);

    internal override void ClearOverride() => ClearInternal();

    internal override ExternalPart GetItemOverride(int index) => GetItemInternal(index);

    internal override void InsertOverride(int index, ExternalPart value) => InsertInternal(index, value);

    internal override void RemoveAtOverride(int index) => RemoveAtInternal(index);

    internal override void SetItemOverride(int index, ExternalPart value) => SetItemInternal(index, value);
}
