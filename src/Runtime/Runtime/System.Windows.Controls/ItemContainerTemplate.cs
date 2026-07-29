
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

using System.Windows.Markup;

namespace System.Windows.Controls;

/// <summary>
/// Provides the template for producing a container for an <see cref="ItemsControl"/> object.
/// </summary>
[DictionaryKeyProperty(nameof(ItemContainerTemplateKey))]
public class ItemContainerTemplate : DataTemplate
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ItemContainerTemplate"/> class.
    /// </summary>
    public ItemContainerTemplate() { }

    /// <summary>
    /// Gets the default key of the <see cref="ItemContainerTemplate"/>.
    /// </summary>
    /// <returns>
    /// The default key of the <see cref="ItemContainerTemplate"/>.
    /// </returns>
    public object ItemContainerTemplateKey => DataType is not null ? new ItemContainerTemplateKey(DataType) : null;
}