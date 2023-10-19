
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

namespace System.Windows
{
    /// <summary>
    /// Represents the method that will handle routed events.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void RoutedEventHandler(object sender, RoutedEventArgs e);
}
