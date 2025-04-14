
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
/// Provides event listening support for classes that expect to receive events through the WeakEvent pattern and
/// a <see cref="WeakEventManager"/>.
/// </summary>
public interface IWeakEventListener
{
    /// <summary>
    /// Receives events from the centralized event manager.
    /// </summary>
    /// <param name="managerType">
    /// The type of the <see cref="WeakEventManager"/> calling this method.
    /// </param>
    /// <param name="sender">
    /// Object that originated the event.
    /// </param>
    /// <param name="e">
    /// Event data.
    /// </param>
    /// <returns>
    /// true if the listener handled the event. It is considered an error by the <see cref="WeakEventManager"/> handling 
    /// in WPF to register a listener for an event that the listener does not handle. Regardless, the method should 
    /// return false if it receives an event that it does not recognize or handle.
    /// </returns>
    bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e);
}
