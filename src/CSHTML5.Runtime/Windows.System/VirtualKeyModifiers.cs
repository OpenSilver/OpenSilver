
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