
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
/// Provides event data for the Tapped event.
/// </summary>
public sealed class TappedRoutedEventArgs : MouseEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TappedRoutedEventArgs"/> class.
    /// </summary>
    public TappedRoutedEventArgs()
        : base(Mouse.PrimaryDevice, Environment.TickCount)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TappedRoutedEventArgs"/> class.
    /// </summary>
    /// <param name="mouse">
    /// The mouse device associated with this event.
    /// </param>
    /// <param name="timestamp">
    /// The time when the input occurred.
    /// </param>
    public TappedRoutedEventArgs(MouseDevice mouse, int timestamp)
        : base(mouse, timestamp)
    {
    }

    internal TappedRoutedEventArgs(MouseDevice mouse, int timestamp, bool isTouchDevice, ModifierKeys keyModifiers, double x, double y)
        : base(mouse, timestamp, isTouchDevice, keyModifiers, x, y)
    {
    }

    /// <inheritdoc />
    protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget) =>
        ((TappedEventHandler)genericHandler)(genericTarget, this);
}
