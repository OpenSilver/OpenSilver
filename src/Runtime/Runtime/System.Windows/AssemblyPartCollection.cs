
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
/// Stores a collection of <see cref="AssemblyPart"/> objects. Provides collection
/// support for the <see cref="Deployment.Parts"/> property.
/// </summary>
public sealed class AssemblyPartCollection : PresentationFrameworkCollection<AssemblyPart>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblyPartCollection"/> class.
    /// </summary>
    public AssemblyPartCollection()
        : base(false)
    {
    }

    internal override void AddOverride(AssemblyPart value) => AddInternal(value);

    internal override void ClearOverride() => ClearInternal();

    internal override void InsertOverride(int index, AssemblyPart value) => InsertInternal(index, value);

    internal override void RemoveAtOverride(int index) => RemoveAtInternal(index);

    internal override AssemblyPart GetItemOverride(int index) => GetItemInternal(index);

    internal override void SetItemOverride(int index, AssemblyPart value) => SetItemInternal(index, value);
}
