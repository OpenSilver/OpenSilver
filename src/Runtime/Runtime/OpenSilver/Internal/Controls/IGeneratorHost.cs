
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

using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace OpenSilver.Internal.Controls;

/// <summary>
///     Interface through which an ItemContainerGenerator
///     communicates with its host.
/// </summary>
internal interface IGeneratorHost
{
    /// <summary>
    /// The view of the data
    /// </summary>
    ItemCollection View { get; }

    /// <summary>
    /// The AlternationCount
    /// </summary>
    int AlternationCount { get; }

    /// <summary>
    /// Return true if the item is (or should be) its own item container
    /// </summary>
    bool IsItemItsOwnContainer(object item);

    /// <summary>
    /// Return the element used to display the given item
    /// </summary>
    DependencyObject GetContainerForItem(object item, DependencyObject recycledContainer);

    /// <summary>
    /// Prepare the element to act as the ItemUI for the corresponding item.
    /// </summary>
    void PrepareItemContainer(DependencyObject container, object item);

    /// <summary>
    /// Undo any initialization done on the element during GetContainerForItem and PrepareItemContainer
    /// </summary>
    void ClearContainerForItem(DependencyObject container, object item);

    /// <summary>
    /// Determine if the given element was generated for this host as an ItemUI.
    /// </summary>
    bool IsHostForItemContainer(DependencyObject container);

    /// <summary>
    /// Return the GroupStyle (if any) to use for the given group at the given level.
    /// </summary>
    GroupStyle GetGroupStyle(CollectionViewGroup group, int level);

    /// <summary>
    /// Communicates to the host that the generator is using grouping.
    /// </summary>
    void SetIsGrouping(bool isGrouping);
}
