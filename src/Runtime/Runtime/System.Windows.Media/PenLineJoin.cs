
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
    /// Describes the shape that joins two lines or segments.
    /// </summary>
    public enum PenLineJoin
    {
        /// <summary>
        /// Line joins use regular angular vertices.
        /// </summary>
        Miter = 0,
        /// <summary>
        /// Line joins use beveled vertices.
        /// </summary>
        Bevel = 1,
        /// <summary>
        /// Line joins use rounded vertices.
        /// </summary>
        Round = 2,
    }
}