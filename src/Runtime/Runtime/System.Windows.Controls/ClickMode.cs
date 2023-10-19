
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

namespace System.Windows.Controls
{
    /// <summary>
    /// Specifies when the Click event should be raised for a control.
    /// </summary>
    public enum ClickMode
    {
        // Summary:
        //     Specifies that the System.Windows.Controls.Primitives.ButtonBase.Click event
        //     should be raised when the left mouse button is pressed and released, and
        //     the mouse pointer is over the control. If you are using the keyboard, specifies
        //     that the System.Windows.Controls.Primitives.ButtonBase.Click event should
        //     be raised when the SPACEBAR or ENTER key is pressed and released, and the
        //     control has keyboard focus.
        /// <summary>
        /// Specifies that the Click event
        /// should be raised when the left mouse button is pressed and released, and
        /// the mouse pointer is over the control.
        /// </summary>
        Release = 0,

        /// <summary>
        /// Specifies that the Click event
        /// should be raised when the mouse button is pressed and the mouse pointer is
        /// over the control.
        /// </summary>
        Press = 1,

        /// <summary>
        /// Specifies that the Click event
        /// should be raised when the mouse pointer moves over the control.
        /// </summary>
        Hover = 2,
    }
}
