
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

using System.ComponentModel;

namespace System.Windows.Input;

/// <summary>
/// Provides event data for the <see cref="UIElement.KeyUp"/> and <see cref="UIElement.KeyDown"/> events.
/// </summary>
public sealed class KeyEventArgs : KeyboardEventArgs
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public KeyEventArgs()
        : base(Keyboard.PrimaryDevice, Environment.TickCount)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyEventArgs"/> class.
    /// </summary>
    /// <param name="keyboard">
    /// The logical keyboard device associated with this event.
    /// </param>
    /// <param name="timestamp">
    /// The time when the input occurred.
    /// </param>
    /// <param name="key">
    /// The key referenced by the event.
    /// </param>
    public KeyEventArgs(KeyboardDevice keyboard, int timestamp, Key key)
        : base(keyboard, timestamp)
    {
        Key = key;
    }

    /// <inheritdoc />
    protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget) =>
        ((KeyEventHandler)genericHandler)(genericTarget, this);

    /// <summary>
    /// Gets or sets a value that marks the routed event as handled. A true value for Handled prevents 
    /// most handlers along the event route from handling the same event again.
    /// </summary>
    /// <returns>
    /// true to mark the routed event handled; false to leave the routed event unhandled, which permits 
    /// the event to potentially route further. The default is false.
    /// </returns>
    public new bool Handled
    {
        get => base.Handled;
        set => base.Handled = value;
    }

    /// <summary>
    /// Gets or sets a value that determines if the routed event will call <i>preventDefault()</i>
    /// if <see cref="Handled"/> is set to true. The default is true.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public new bool Cancellable
    {
        get => base.Cancellable;
        set => base.Cancellable = value;
    }

    /// <summary>
    /// Gets the keyboard key associated with the event.
    /// </summary>
    /// <returns>
    /// One of the enumeration values that indicates the key referenced by the event.
    /// </returns>
    public Key Key { get; internal set; }

    /// <summary>
    /// Gets a value that indicates whether this event represents a key press.
    /// </summary>
    public bool IsDown => RoutedEvent == Keyboard.KeyDownEvent || RoutedEvent == Keyboard.PreviewKeyDownEvent;

    /// <summary>
    /// Gets a value that indicates whether this event represents a key release.
    /// </summary>
    public bool IsUp => RoutedEvent == Keyboard.KeyUpEvent || RoutedEvent == Keyboard.PreviewKeyUpEvent;

    /// <summary>
    /// Gets an integer value that represents the key that is pressed or released (depending on which 
    /// event is raised). This value is the nonportable key code, which is operating system–specific.
    /// </summary>
    /// <returns>
    /// The key code value.
    /// </returns>
    public int PlatformKeyCode { get; internal set; }

    /// <summary>
    /// Gets a value that indicates which key modifiers were active at the time that
    /// the pointer event was initiated.
    /// </summary>
    public ModifierKeys KeyModifiers { get; internal set; }
}
