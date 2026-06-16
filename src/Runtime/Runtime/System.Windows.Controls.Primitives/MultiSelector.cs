
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

using OpenSilver.Internal;
using System.Collections;

namespace System.Windows.Controls.Primitives;

/// <summary>
/// Provides an abstract class for controls that allow multiple items to be selected.
/// </summary>
public abstract class MultiSelector : Selector
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MultiSelector"/> class.
    /// </summary>
    protected MultiSelector() { }

    /// <summary>
    /// Gets the items in the <see cref="MultiSelector"/> that are selected.
    /// </summary>
    /// <returns>
    /// The items in the <see cref="MultiSelector"/> that are selected.
    /// </returns>
    public IList SelectedItems => SelectedItemsImpl;

    /// <summary>
    /// Gets or sets a value that indicates whether the multiple items in the <see cref="MultiSelector"/> can 
    /// be selected at a time.
    /// </summary>
    /// <returns>
    /// true if multiple items in the <see cref="MultiSelector"/> can be selected at a time; otherwise, false.
    /// </returns>
    protected bool CanSelectMultipleItems
    {
        get => CanSelectMultiple;
        set => CanSelectMultiple = value;
    }

    /// <summary>
    /// Gets a value that indicates whether the <see cref="MultiSelector"/> is currently performing a bulk update 
    /// to the <see cref="SelectedItems"/> collection.
    /// </summary>
    /// <returns>
    /// true if the <see cref="MultiSelector"/> is currently performing a bulk update to the <see cref="SelectedItems"/>
    /// collection; otherwise, false.
    /// </returns>
    protected bool IsUpdatingSelectedItems => SelectedItemsImpl.IsUpdatingSelectedItems;

    /// <summary>
    /// Selects all of the items in the <see cref="MultiSelector"/>.
    /// </summary>
    /// <exception cref="NotSupportedException">
    /// <see cref="CanSelectMultipleItems"/> is false.
    /// </exception>
    public void SelectAll()
    {
        if (!CanSelectMultipleItems)
        {
            throw new NotSupportedException(Strings.MultiSelectorSelectAll);
        }

        SelectAllImpl();
    }

    /// <summary>
    /// Unselects all of the items in the <see cref="MultiSelector"/>.
    /// </summary>
    public void UnselectAll() => UnselectAllImpl();

    /// <summary>
    /// Starts a new selection transaction.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// <see cref="IsUpdatingSelectedItems"/> is true when this method is called.
    /// </exception>
    protected void BeginUpdateSelectedItems() => SelectedItemsImpl.BeginUpdateSelectedItems();

    /// <summary>
    /// Commits the selected items to the <see cref="MultiSelector"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// <see cref="IsUpdatingSelectedItems"/> is false when this method is called.
    /// </exception>
    protected void EndUpdateSelectedItems() => SelectedItemsImpl.EndUpdateSelectedItems();
}
