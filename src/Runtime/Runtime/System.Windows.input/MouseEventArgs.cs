
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
/// Provides data for mouse related routed events that do not specifically involve mouse 
/// buttons or the mouse wheel, for example <see cref="UIElement.MouseMove"/>.
/// </summary>
public class MouseEventArgs : InputEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MouseEventArgs"/> class.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public MouseEventArgs()
        : base(Mouse.PrimaryDevice, Environment.TickCount)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MouseEventArgs"/> class using the specified 
    /// <see cref="MouseDevice"/> and timestamp.
    /// </summary>
    /// <param name="mouse">
    /// The mouse device associated with this event.
    /// </param>
    /// <param name="timestamp">
    /// The time when the input occurred.
    /// </param>
    public MouseEventArgs(MouseDevice mouse, int timestamp)
        : base(mouse, timestamp)
    {
    }

    internal MouseEventArgs(MouseDevice mouse, int timestamp, bool isTouchDevice, ModifierKeys keyModifiers, double x, double y)
        : base(mouse, timestamp)
    {
        IsTouchEvent = isTouchDevice;
        KeyModifiers = keyModifiers;
        _pointerAbsoluteX = x;
        _pointerAbsoluteY = y;
    }

    /// <inheritdoc />
    protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget) =>
        ((MouseEventHandler)genericHandler)(genericTarget, this);

    internal double _pointerAbsoluteX;
    internal double _pointerAbsoluteY;

    internal bool IsTouchEvent { get; private set; }

    /// <summary>
    /// Gets the mouse device associated with this event.
    /// </summary>
    /// <returns>
    /// The mouse device associated with this event. There is no default value.
    /// </returns>
    public MouseDevice MouseDevice => (MouseDevice)Device;

    /// <summary>
    /// Gets the current state of the left mouse button.
    /// </summary>
    /// <returns>
    /// The current state of the left mouse button, which is either <see cref="MouseButtonState.Pressed"/> 
    /// or <see cref="MouseButtonState.Released"/>. There is no default value.
    /// </returns>
    public MouseButtonState LeftButton => MouseDevice.LeftButton;

    /// <summary>
    /// Gets the current state of the middle mouse button.
    /// </summary>
    /// <returns>
    /// The current state of the middle mouse button, which is either <see cref="MouseButtonState.Pressed"/> 
    /// or <see cref="MouseButtonState.Released"/>. There is no default value.
    /// </returns>
    public MouseButtonState MiddleButton => MouseDevice.MiddleButton;

    /// <summary>
    /// Gets the current state of the right mouse button.
    /// </summary>
    /// <returns>
    /// The current state of the right mouse button, which is either <see cref="MouseButtonState.Pressed"/> 
    /// or <see cref="MouseButtonState.Released"/>. There is no default value.
    /// </returns>
    public MouseButtonState RightButton => MouseDevice.RightButton;

    /// <summary>
    /// Gets or sets a value that marks the routed event as handled, and prevents
    /// most handlers along the event route from handling the same event again.
    /// </summary>
    public new bool Handled
    {
        get => base.Handled;
        set => base.Handled = value;
    }

    /// <summary>
    /// Gets a value that indicates which key modifiers were active at the time that
    /// the pointer event was initiated.
    /// </summary>
    public ModifierKeys KeyModifiers { get; }

    /// <summary>
    /// Gets an object that reports stylus device information, such as the collection
    /// of stylus points associated with the input.
    /// </summary>
    /// <returns>
    /// The stylus device information object.
    /// </returns>
    public StylusDevice StylusDevice => new StylusDevice(this);

    /// <summary>
    /// Gets a reference to a pointer token.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Pointer Pointer { get; internal set; }

    /// <summary>
    /// Gets the number of times the button was clicked.
    /// </summary>
    public int ClickCount { get; internal set; }

    /// <summary>
    /// Returns the position of the mouse pointer relative to the specified element.
    /// </summary>
    /// <param name="relativeTo">
    /// The element to use as the frame of reference for calculating the position of the mouse pointer.
    /// </param>
    /// <returns>
    /// The x- and y-coordinates of the mouse pointer position relative to the specified object.
    /// </returns>
    public Point GetPosition(UIElement relativeTo)
        => MouseDevice.GetPosition(new Point(_pointerAbsoluteX, _pointerAbsoluteY), relativeTo);

    /// <summary>
    /// Returns the position of the mouse pointer relative to the specified element.
    /// </summary>
    /// <param name="relativeTo">
    /// The element to use as the frame of reference for calculating the position of the mouse pointer.
    /// </param>
    /// <returns>
    /// The x- and y-coordinates of the mouse pointer position relative to the specified object.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Point GetPosition(IInputElement relativeTo)
        => MouseDevice.GetPosition(new Point(_pointerAbsoluteX, _pointerAbsoluteY), relativeTo);
}