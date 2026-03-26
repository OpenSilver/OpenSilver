
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
/// Specifies the panel that the <see cref="ItemsPresenter"/> creates for the layout of 
/// the items of an <see cref="ItemsControl"/>.
/// </summary>
public class ItemsPanelTemplate : FrameworkTemplate
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ItemsPanelTemplate"/> class.
    /// </summary>
    public ItemsPanelTemplate() { }

    internal override Type TargetTypeInternal => typeof(ItemsPresenter);
}
