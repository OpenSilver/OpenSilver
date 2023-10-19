
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
    /// Represents the callback that is invoked when the effective property value
    /// of a dependency property changes.
    /// </summary>
    /// <param name="d"></param>
    /// <param name="e"></param>
    public delegate void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e);
}
