
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using System;

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
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
        ///// <summary>
        ///// The gradient is repeated in the reverse direction until the space is filled.
        ///// </summary>
        //Reflect = 1,  
        /// <summary>
        /// The gradient is repeated in the original direction until the space is filled.
        /// </summary>
        Repeat = 2,
    }
}
