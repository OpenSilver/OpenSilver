
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
/// Provides event data for mouse button input events, for example <see cref="UIElement.MouseLeftButtonDown"/>
/// and <see cref="UIElement.MouseRightButtonUp"/>.
/// </summary>
public class MouseButtonEventArgs : MouseEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MouseButtonEventArgs"/> class.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public MouseButtonEventArgs()
        : base(Mouse.PrimaryDevice, Environment.TickCount)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MouseButtonEventArgs"/> class by using the 
    /// specified <see cref="MouseDevice"/>, timestamp, and <see cref="MouseButton"/>.
    /// </summary>
    /// <param name="mouse">
    /// The logical Mouse device associated with this event.
    /// </param>
    /// <param name="timestamp">
    /// The time when the input occurred.
    /// </param>
    /// <param name="button">
    /// The mouse button whose state is being described.
    /// </param>
    public MouseButtonEventArgs(MouseDevice mouse, int timestamp, MouseButton button)
        : base(mouse, timestamp)
    {
        ChangedButton = button;
    }

    internal MouseButtonEventArgs(
        MouseDevice mouse,
        int timestamp,
        MouseButton button,
        MouseButtonState buttonState,
        bool isTouchDevice,
        ModifierKeys keyModifiers,
        double x,
        double y) : base(mouse, timestamp, isTouchDevice, keyModifiers, x, y)
    {
        MouseButtonUtilities.Validate(button);
        MouseButtonStateUtilities.Validate(buttonState);

        ChangedButton = button;
        ButtonState = buttonState;
    }

    /// <summary>
    /// Gets the button associated with the event.
    /// </summary>
    /// <returns>
    /// The button which was pressed.
    /// </returns>
    public MouseButton ChangedButton { get; }

    /// <summary>
    /// Gets the state of the button associated with the event.
    /// </summary>
    /// <returns>
    /// The state the button is in.
    /// </returns>
    public MouseButtonState ButtonState { get; }

    /// <inheritdoc />
    protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget) =>
        ((MouseButtonEventHandler)genericHandler)(genericTarget, this);
}
