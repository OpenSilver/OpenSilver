

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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
#if MIGRATION

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

#else

    /// <summary>
    /// Represents the method that will handle pointer message events such as PointerPressed.
    /// </summary>
    /// <param name="sender">The object that fired the event</param>
    /// <param name="e">The infos on the event</param>
    public delegate void PointerEventHandler(object sender, PointerRoutedEventArgs e);

#endif
}