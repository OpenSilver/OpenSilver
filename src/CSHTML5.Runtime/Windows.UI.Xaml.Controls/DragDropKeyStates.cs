
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



#if unsupported
using System;

#if MIGRATION
namespace Microsoft.Windows
#else
namespace System.Windows
#endif
{
    /// <summary>
    /// Specifies the current state of the modifier keys (SHIFT, CTRL, and ALT),
    /// as well as the state of the mouse buttons.
    /// </summary>
    [Flags]
    public enum DragDropKeyStates
    {
        /// <summary>
        /// No modifier keys or mouse buttons are pressed.
        /// </summary>
        None = 0,

        /// <summary>
        /// The left mouse button is pressed.
        /// </summary>
        LeftMouseButton = 1,

        /// <summary>
        /// The right mouse button is pressed.
        /// </summary>
        RightMouseButton = 2,

        /// <summary>
        /// The shift (SHIFT) key is pressed.
        /// </summary>
        ShiftKey = 4,

        /// <summary>
        /// The control (CTRL) key is pressed.
        /// </summary>
        ControlKey = 8,

        /// <summary>
        /// The middle mouse button is pressed.
        /// </summary>
        MiddleMouseButton = 16,

        /// <summary>
        /// The ALT key is pressed.
        /// </summary>
        AltKey = 32,
    }
}
#endif