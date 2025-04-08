
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
/// Defines values that specify the buttons on a mouse device.
/// </summary>
public enum MouseButton
{
    /// <summary>
    /// The left mouse button.
    /// </summary>
    Left = 0,

    /// <summary>
    /// The middle mouse button.
    /// </summary>
    Middle = 1,

    /// <summary>
    /// The right mouse button.
    /// </summary>
    Right = 2,
}

/// <summary>
/// Utility class for MouseButton
/// </summary>
internal static class MouseButtonUtilities
{
    /// <summary>
    /// Ensures MouseButton is set to a valid value.
    /// </summary>
    /// <remarks>
    /// There is a proscription against using Enum.IsDefined().  (it is slow)
    /// So we manually validate using a switch statement.
    /// </remarks>
    internal static void Validate(MouseButton button)
    {
        switch (button)
        {
            case MouseButton.Left:
            case MouseButton.Middle:
            case MouseButton.Right:
                break;
            default:
                throw new InvalidEnumArgumentException(nameof(button), (int)button, typeof(MouseButton));
        }
    }
}
