
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
/// Specifies the possible states of a mouse button.
/// </summary>
public enum MouseButtonState
{
    /// <summary>
    /// The button is released.
    /// </summary>
    Released = 0,

    /// <summary>
    /// The button is pressed.
    /// </summary>
    Pressed = 1,
}

/// <summary>
/// Utility class for MouseButtonState
/// </summary>
internal static class MouseButtonStateUtilities
{
    /// <summary>
    /// Ensures MouseButtonState is set to a valid value.
    /// </summary>
    internal static void Validate(MouseButtonState buttonState)
    {
        switch (buttonState)
        {
            case MouseButtonState.Released:
            case MouseButtonState.Pressed:
                break;
            default:
                throw new InvalidEnumArgumentException(nameof(buttonState), (int)buttonState, typeof(MouseButtonState));
        }
    }
}
