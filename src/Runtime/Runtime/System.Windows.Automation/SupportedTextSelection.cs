
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
namespace System.Windows.Automation
#else
namespace Windows.UI.Xaml.Automation
#endif
{
    /// <summary>
    /// Contains values that specify whether a text provider supports selection and, if so, 
    /// whether it supports a single, continuous selection or multiple, disjoint selections.
    /// </summary>
    [Flags]
    public enum SupportedTextSelection
    {
        /// <summary>
        /// Does not support text selections.
        /// </summary>
        None = 0,
        /// <summary>
        /// Supports a single, continuous text selection.
        /// </summary>
        Single = 1,
        /// <summary>
        /// Supports multiple, disjoint text selections.
        /// </summary>
        Multiple = 2,
    }
}
