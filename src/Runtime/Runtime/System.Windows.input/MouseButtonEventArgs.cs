
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
/// Provides event data for mouse button input events, for example <see cref="UIElement.MouseLeftButtonDown"/>
/// and <see cref="UIElement.MouseRightButtonUp"/>.
/// </summary>
public class MouseButtonEventArgs : MouseEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MouseButtonEventArgs"/> class.
    /// </summary>
    public MouseButtonEventArgs() { }

    internal MouseButtonEventArgs(MouseButton button, bool isTouchDevice, ModifierKeys keyModifiers, double x, double y)
        : base(isTouchDevice, keyModifiers, x, y)
    {
        MouseButtonUtilities.Validate(button);

        ChangedButton = button;
    }

    /// <summary>
    /// Gets the button associated with the event.
    /// </summary>
    /// <returns>
    /// The button which was pressed.
    /// </returns>
    public MouseButton ChangedButton { get; }

    /// <inheritdoc />
    protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget) =>
        ((MouseButtonEventHandler)genericHandler)(genericTarget, this);
}
