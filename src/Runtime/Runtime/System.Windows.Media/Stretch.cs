
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
    /// Describes how content is resized to fill its allocated space.
    /// </summary>
    public enum Stretch
    {
        /// <summary>
        /// The content preserves its original size.
        /// </summary>
        None = 0,

        /// <summary>
        /// The content is resized to fill the destination dimensions. The aspect ratio is not preserved.
        /// </summary>
        Fill = 1,

        /// <summary>
        /// The content is resized to fit in the destination dimensions while it preserves its native aspect ratio.
        /// </summary>
        Uniform = 2,

        /// <summary>
        /// The content is resized to fill the destination dimensions while it preserves
        /// its native aspect ratio. If the aspect ratio of the destination rectangle
        /// differs from the source, the source content is clipped to fit in the destination
        /// dimensions.
        /// </summary>
        UniformToFill = 3,
    }
}