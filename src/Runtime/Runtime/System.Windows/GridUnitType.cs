

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

namespace System.Windows
{
    /// <summary>
    /// Describes the kind of value that a Windows.UI.Xaml.GridLength
    /// object is holding.
    /// </summary>
    public enum GridUnitType
    {
        /// <summary>
        /// [SECURITY CRITICAL] The size is determined by the size properties of the
        /// content object.
        /// </summary>
        Auto = 0,
        /// <summary>
        /// [SECURITY CRITICAL] The value is expressed in pixels.
        /// </summary>
        Pixel = 1,
        /// <summary>
        /// [SECURITY CRITICAL] The value is expressed as a weighted proportion of available
        /// </summary>
        Star = 2,
    }
}