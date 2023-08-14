
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

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Determines how the colors in a gradient are interpolated.
    /// </summary>
    public enum ColorInterpolationMode
    {
        /// <summary>
        /// Colors are interpolated in the scRGB color space
        /// </summary>
        ScRgbLinearInterpolation = 0,

        /// <summary>
        /// Colors are interpolated in the sRGB color space
        /// </summary>
        SRgbLinearInterpolation = 1
    }
}
