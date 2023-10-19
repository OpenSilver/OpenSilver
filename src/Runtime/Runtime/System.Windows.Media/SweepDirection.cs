
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

namespace System.Windows.Media
{
    /// <summary>
    /// Specifies the direction in which an elliptical arc is drawn.
    /// </summary>
    public enum SweepDirection
    {
        /// <summary>
        /// Arcs are drawn in a counterclockwise (negative-angle) direction.
        /// </summary>
        Counterclockwise = 0,

        /// <summary>
        /// Arcs are drawn in a clockwise (positive-angle) direction.
        /// </summary>
        Clockwise = 1,
    }
}
