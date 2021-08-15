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


using System;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
#if WORKINPROGRESS
    // Summary:
    //     Specifies the method the System.Windows.Controls.VirtualizingStackPanel uses
    //     to manage virtualizing its child items.
    public enum VirtualizationMode
    {
        // Summary:                                                                           
        //     Create and discard the item containers.
        Standard = 0,
        //
        // Summary:
        //     Reuse the item containers.
        Recycling = 1,
    }
#endif
}