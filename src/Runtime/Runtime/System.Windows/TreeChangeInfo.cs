
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
///     This is the data that is passed through the DescendentsWalker 
///     during an AncestorChange tree-walk.
/// </summary>
internal readonly struct TreeChangeInfo
{
    public TreeChangeInfo(DependencyObject root, DependencyObject parent, bool isAddOperation)
    {
        Root = root;
        IsAddOperation = isAddOperation;
    }

    // Indicates if this is a add child tree operation
    internal bool IsAddOperation { get; }

    // This is the element at the root of the sub-tree that had a parent change.
    internal DependencyObject Root { get; }
}
