
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

namespace System.Windows.Input
{
    /// <summary>
    /// Represents the method that handles the System.Windows.UIElement.MouseLeftButtonDown
    /// and System.Windows.UIElement.MouseLeftButtonUp events.
    /// </summary>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event data.</param>
    public delegate void MouseButtonEventHandler(object sender, MouseButtonEventArgs e);

    /// <summary>
    /// Represents the method that will handle mouse related routed events that do not
    /// specifically involve mouse buttons; for example, System.Windows.UIElement.MouseMove.
    /// </summary>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event data.</param>
    public delegate void MouseEventHandler(object sender, MouseEventArgs e);
}