
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

namespace System.Windows.Browser
{
    /// <summary>
    /// Specifies constants that indicate which mouse button was clicked.
    /// </summary>
    [Flags]
    public enum MouseButtons
    {
        /// <summary>
        /// No mouse button was clicked.
        /// </summary>
        None = 0,

        /// <summary>
        /// The left mouse button was clicked.
        /// </summary>
        Left = 1,

        /// <summary>
        /// The right mouse button was clicked.
        /// </summary>
        Right = 2,

        /// <summary>
        /// The wheel button was clicked.
        /// </summary>
        Middle = 4
    }
}