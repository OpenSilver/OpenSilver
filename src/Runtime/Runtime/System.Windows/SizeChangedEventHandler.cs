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
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Represents the method that will handle the SizeChanged event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">Provides data related to the SizeChanged event.</param>
    public delegate void SizeChangedEventHandler(object sender, SizeChangedEventArgs e);
}
