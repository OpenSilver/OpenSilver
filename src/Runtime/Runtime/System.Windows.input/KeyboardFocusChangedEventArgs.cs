
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

namespace System.Windows.Input;

/// <summary>
/// Represents the method that will handle the <see cref="UIElement.LostKeyboardFocus"/> and 
/// <see cref="UIElement.GotKeyboardFocus"/> routed events, as well as related attached and 
/// Preview events. 
/// </summary>
/// <param name="sender">
/// The object where the event handler is attached.
/// </param>
/// <param name="e">
/// The event data.
/// </param>
public delegate void KeyboardFocusChangedEventHandler(object sender, KeyboardFocusChangedEventArgs e);

/// <summary>
/// Provides data for <see cref="UIElement.LostKeyboardFocus"/> and <see cref="UIElement.GotKeyboardFocus"/>
/// routed events, as well as related attached and Preview events.
/// </summary>
public class KeyboardFocusChangedEventArgs : KeyboardEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KeyboardFocusChangedEventArgs"/> class.
    /// </summary>
    /// <param name="keyboard">
    /// The logical keyboard device associated with this event.
    /// </param>
    /// <param name="timestamp">
    /// The time when the input occurred.
    /// </param>
    /// <param name="oldFocus">
    /// The element that previously had focus.
    /// </param>
    /// <param name="newFocus">
    /// The element that now has focus.
    /// </param>
    public KeyboardFocusChangedEventArgs(KeyboardDevice keyboard, int timestamp, IInputElement oldFocus, IInputElement newFocus)
        : base(keyboard, timestamp)
    {
        if (oldFocus is not null && !InputElement.IsValid(oldFocus))
        {
            throw new InvalidOperationException(string.Format(Strings.Invalid_IInputElement, oldFocus.GetType()));
        }

        if (newFocus is not null && !InputElement.IsValid(newFocus))
        {
            throw new InvalidOperationException(string.Format(Strings.Invalid_IInputElement, newFocus.GetType()));
        }

        OldFocus = oldFocus;
        NewFocus = newFocus;
    }

    /// <summary>
    /// Gets the element that focus has moved to.
    /// </summary>
    /// <returns>
    /// The element with focus.
    /// </returns>
    public IInputElement NewFocus { get; }

    /// <summary>
    /// Gets the element that previously had focus.
    /// </summary>
    /// <returns>
    /// The previously focused element.
    /// </returns>
    public IInputElement OldFocus { get; }

    /// <inheritdoc />
    protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget) =>
        ((KeyboardFocusChangedEventHandler)genericHandler)(genericTarget, this);
}
