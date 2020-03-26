

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

        /// <summary>
        /// The Windows virtual key.
        /// </summary>
        Windows = 8,

        /// <summary>
        /// The Apple key (also known as the Open Apple key) is pressed.
        /// </summary>
        Apple = 8
    }
}