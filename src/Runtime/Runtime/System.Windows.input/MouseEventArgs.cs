
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
/// Provides event data for pointer message events related to specific user interface
/// elements, such as PointerPressed.
/// </summary>
public class MouseEventArgs : InputEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MouseEventArgs"/> class.
    /// </summary>
    public MouseEventArgs() { }

    internal MouseEventArgs(bool isTouchDevice, ModifierKeys keyModifiers, double x, double y)
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
        => Mouse.GetPosition(new Point(_pointerAbsoluteX, _pointerAbsoluteY), relativeTo);

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
        => Mouse.GetPosition(new Point(_pointerAbsoluteX, _pointerAbsoluteY), relativeTo);
}