
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Input
#else
namespace Windows.System
#endif
{
    /// <summary>
    /// Specifies the virtual key used to modify another keypress.
    /// </summary>
    [Flags]
#if MIGRATION
    public enum ModifierKeys
#else
    public enum VirtualKeyModifiers
#endif
    {
        /// <summary>
        /// No virtual key modifier.
        /// </summary>
        None = 0,

        /// <summary>
        /// The Ctrl (control) virtual key.
        /// </summary>
        Control = 1,

        /// <summary>
        /// The Menu (Alt) virtual key.
        /// </summary>
#if MIGRATION
        Alt = 2,
#else
        Menu = 2, //this corresponds to the Alt key
#endif

        /// <summary>
        /// The Shift virtual key.
        /// </summary>
        Shift = 4,

        ///// <summary>
        ///// The Windows virtual key.
        ///// </summary>
        //Windows = 8,
    }
}