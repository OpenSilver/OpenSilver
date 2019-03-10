
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
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