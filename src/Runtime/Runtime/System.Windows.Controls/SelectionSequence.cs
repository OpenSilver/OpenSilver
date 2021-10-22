

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
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Determines the order in which visual states are set.
    /// </summary>
    public enum SelectionSequence
    {
        /// <summary>
        /// Collapses are set before expansions.
        /// </summary>
        CollapseBeforeExpand,

        /// <summary>
        /// No delays, all states are set immediately.
        /// </summary>
        Simultaneous
    }
}
