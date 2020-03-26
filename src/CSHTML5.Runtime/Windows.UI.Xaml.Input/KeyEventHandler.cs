

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
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
    /// <summary>
    /// Represents the method that handles the KeyUp and KeyDown events.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
#if MIGRATION
    public delegate void KeyEventHandler(object sender, KeyEventArgs e);
#else
    public delegate void KeyEventHandler(object sender, KeyRoutedEventArgs e);
#endif
}
