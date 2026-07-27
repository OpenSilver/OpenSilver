
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
/// Represents the method that will handle the <see cref="UIElement.QueryCursor"/> events, as well 
/// as the <b>Mouse.QueryCursor</b> attached event.
/// </summary>
/// <param name="sender">
/// The object where the event handler is attached.
/// </param>
/// <param name="e">
/// The event data.
/// </param>
public delegate void QueryCursorEventHandler(object sender, QueryCursorEventArgs e);

/// <summary>
/// Provides data for the <b>Mouse.QueryCursor</b> event.
/// </summary>
public class QueryCursorEventArgs : MouseEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QueryCursorEventArgs"/> class, by using the 
    /// specified mouse device and the specified timestamp.
    /// </summary>
    /// <param name="mouse">
    /// The logical mouse device associated with this event.
    /// </param>
    /// <param name="timestamp">
    /// The time when the input occurred.
    /// </param>
    public QueryCursorEventArgs(MouseDevice mouse, int timestamp)
        : base(mouse, timestamp)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryCursorEventArgs"/> class, by using the 
    /// specified mouse device, timestamp, and stylus device.
    /// </summary>
    /// <param name="mouse">
    /// The logical mouse device associated with this event.
    /// </param>
    /// <param name="timestamp">
    /// The time when the input occurred.
    /// </param>
    /// <param name="stylusDevice">
    /// The stylus pointer associated with this event.
    /// </param>
    public QueryCursorEventArgs(MouseDevice mouse, int timestamp, StylusDevice stylusDevice)
        : base(mouse, timestamp, stylusDevice)
    {
    }

    internal QueryCursorEventArgs(MouseDevice mouse, int timestamp, bool isTouchDevice, ModifierKeys keyModifiers, double x, double y)
        : base(mouse, timestamp, isTouchDevice, keyModifiers, x, y)
    {
    }

    /// <summary>
    /// Gets or sets the cursor associated with this event.
    /// </summary>
    /// <returns>
    /// The cursor.
    /// </returns>
    public Cursor Cursor
    {
        get;
        set => field = value ?? Cursors.None;
    }

    /// <inheritdoc />
    protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget) =>
        ((QueryCursorEventHandler)genericHandler)(genericTarget, this);
}