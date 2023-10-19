
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
    /// Specifies how to draw the gradient outside a gradient brush's gradient vector
    /// or space.
    /// </summary>
    public enum GradientSpreadMethod
    {
        /// <summary>
        /// The color values at the ends of the gradient vector fill the remaining space.
        /// </summary>
        Pad = 0,

        /// <summary>
        /// The gradient is repeated in the reverse direction until the space is filled.
        /// </summary>
        Reflect = 1,

        /// <summary>
        /// The gradient is repeated in the original direction until the space is filled.
        /// </summary>
        Repeat = 2,
    }
}
