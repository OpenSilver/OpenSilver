
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
/// Represents the method that will handle the Keyboard.KeyboardInputProviderAcquireFocus event.
/// </summary>
/// <param name="sender">
/// The source of the event.
/// </param>
/// <param name="e">
/// A <see cref="KeyboardInputProviderAcquireFocusEventArgs"/> that contains the event data.
/// </param>
public delegate void KeyboardInputProviderAcquireFocusEventHandler(object sender, KeyboardInputProviderAcquireFocusEventArgs e);

/// <summary>
/// Provides data for the Keyboard.KeyboardInputProviderAcquireFocus event.
/// </summary>
public class KeyboardInputProviderAcquireFocusEventArgs : KeyboardEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KeyboardInputProviderAcquireFocusEventArgs"/> class.
    /// </summary>
    /// <param name="keyboard">
    /// The logical keyboard device associated with this event.
    /// </param>
    /// <param name="timestamp">
    /// The time when the input occurred.
    /// </param>
    /// <param name="focusAcquired">
    /// true to indicate that interoperation focus was acquired; otherwise, false.
    /// </param>
    public KeyboardInputProviderAcquireFocusEventArgs(KeyboardDevice keyboard, int timestamp, bool focusAcquired)
        : base(keyboard, timestamp)
    {
        FocusAcquired = focusAcquired;
    }

    /// <summary>
    /// Gets a value that indicates whether interoperation focus was acquired.
    /// </summary>
    /// <returns>
    /// true if interoperation focus was acquired; otherwise, false.
    /// </returns>
    public bool FocusAcquired { get; }

    /// <inheritdoc />
    protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget) =>
        ((KeyboardInputProviderAcquireFocusEventHandler)genericHandler)(genericTarget, this);
}
