
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
    public TappedRoutedEventArgs() { }

    internal TappedRoutedEventArgs(bool isTouchDevice, ModifierKeys keyModifiers, double x, double y)
        : base(isTouchDevice, keyModifiers, x, y)
    {
    }

    /// <inheritdoc />
    protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget) =>
        ((TappedEventHandler)genericHandler)(genericTarget, this);
}
