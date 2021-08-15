﻿

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


#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Describes the shape at the end of a line or segment.
    /// </summary>
    public enum PenLineCap
    {
        /// <summary>
        /// A cap that does not extend past the last point of the line. Comparable to
        /// no line cap.
        /// </summary>
        Flat = 0,
        /// <summary>
        /// A rectangle that has a height equal to the line thickness and a length equal
        /// to half the line thickness.
        /// </summary>
        Square = 1,
        /// <summary>
        /// A semicircle that has a diameter equal to the line thickness.
        /// </summary>
        Round = 2,
#if WORKINPROGRESS
        /// <summary>
        ///     Triangle - Triangle line cap.
        /// </summary>
        Triangle = 3,
#endif
    }
}