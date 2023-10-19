
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

namespace Microsoft.Windows
{
    /// <summary>
    /// Represents the method that will handle the UIElement.DragEnter,
    /// UIElement.DragLeave, UIElement.DragOver, and
    /// UIElement.Drop events of a UIElement.
    /// </summary>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event data.</param>
    public delegate void DragEventHandler(object sender, DragEventArgs e);
}
