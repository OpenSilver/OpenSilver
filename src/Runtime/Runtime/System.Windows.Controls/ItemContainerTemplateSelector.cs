
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

namespace System.Windows.Controls;

/// <summary>
/// Enables you to select an <see cref="ItemContainerTemplate"/> for each item within an 
/// <see cref="ItemsControl"/>.
/// </summary>
public abstract class ItemContainerTemplateSelector
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ItemContainerTemplateSelector"/> class.
    /// </summary>
    protected ItemContainerTemplateSelector() { }

    /// <summary>
    /// When overridden in a derived class, returns an <see cref="ItemContainerTemplate"/> based 
    /// on custom logic.
    /// </summary>
    /// <param name="item">
    /// The object for which to select the template.
    /// </param>
    /// <param name="parentItemsControl">
    /// The container for the items.
    /// </param>
    /// <returns>
    /// The template. The default implementation returns null.
    /// </returns>
    public virtual DataTemplate SelectTemplate(object item, ItemsControl parentItemsControl) => null;
}

internal sealed class DefaultItemContainerTemplateSelector : ItemContainerTemplateSelector
{
    public override DataTemplate SelectTemplate(object item, ItemsControl parentItemsControl)
    {
        // Do an implicit type lookup for an ItemContainerTemplate
        return ContentPresenter.FindTemplateResourceInternal(parentItemsControl, item, typeof(ItemContainerTemplate)) as DataTemplate;
    }
}