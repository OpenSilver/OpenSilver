
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

using System;

namespace Microsoft.Windows
{
    /// <summary>
    /// Specifies the current state of the modifier keys (SHIFT, CTRL, and ALT),
    /// as well as the state of the mouse buttons.
    /// </summary>
    [Flags]
    public enum DragDropKeyStates
    {
        /// <summary>
        /// No modifier keys or mouse buttons are pressed.
        /// </summary>
        None = 0,

        /// <summary>
        /// The left mouse button is pressed.
        /// </summary>
        LeftMouseButton = 1,

        /// <summary>
        /// The right mouse button is pressed.
        /// </summary>
        RightMouseButton = 2,

        /// <summary>
        /// The shift (SHIFT) key is pressed.
        /// </summary>
        ShiftKey = 4,

        /// <summary>
        /// The control (CTRL) key is pressed.
        /// </summary>
        ControlKey = 8,

        /// <summary>
        /// The middle mouse button is pressed.
        /// </summary>
        MiddleMouseButton = 16,

        /// <summary>
        /// The ALT key is pressed.
        /// </summary>
        AltKey = 32,
    }
}
