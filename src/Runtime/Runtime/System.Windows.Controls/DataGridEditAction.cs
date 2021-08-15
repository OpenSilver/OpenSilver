﻿

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


#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    ///     Used to specify action to take out of edit mode.
    /// </summary>
    public enum DataGridEditAction
    {
        /// <summary>
        ///     Cancel the changes.
        /// </summary>
        Cancel,

        /// <summary>
        ///     Commit edited value.
        /// </summary>
        Commit
    }
}