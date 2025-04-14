
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

namespace System.Windows.Input;

/// <summary>
/// Represents the method that will handle keyboard-related routed events.
/// </summary>
/// <param name="sender">
/// The object where the event handler is attached.
/// </param>
/// <param name="e">
/// The event data.
/// </param>
public delegate void KeyboardEventHandler(object sender, KeyboardEventArgs e);

/// <summary>
/// Provides data for keyboard-related events.
/// </summary>
public class KeyboardEventArgs : InputEventArgs
{
    /// <summary>
    /// Invokes event handlers in a type-specific way, which can increase event system efficiency.
    /// </summary>
    /// <param name="genericHandler">
    /// The generic handler to call in a type-specific way.
    /// </param>
    /// <param name="genericTarget">
    /// The target to call the handler on.
    /// </param>
    protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget) =>
        ((KeyboardEventHandler)genericHandler)(genericTarget, this);
}
