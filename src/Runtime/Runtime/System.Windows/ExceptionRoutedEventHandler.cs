

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

namespace System.Windows
{
    /// <summary>
    /// Represents the method that will handle certain events that report exceptions.
    /// These exceptions generally come from asynchronous operations.</summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ExceptionRoutedEventHandler(object sender, ExceptionRoutedEventArgs e);
}