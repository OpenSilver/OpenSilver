
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
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Describes the direction that content is scaled.
    /// </summary>
    public enum StretchDirection
    {
        /// <summary>
        /// The content scales upward only when it is smaller than the parent. If the
        /// content is larger, no scaling downward is performed.
        /// </summary>
        UpOnly = 0,

        /// <summary>
        /// The content scales downward only when it is larger than the parent. If the
        /// content is smaller, no scaling upward is performed.
        /// </summary>
        DownOnly = 1,

        /// <summary>
        /// The content stretches to fit the parent according to the Stretch property.
        /// </summary>
        Both = 2,
    }
}
