
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
    /// Property value coercion callback.
    /// </summary>
    /// <param name="d">The DependencyObject on which the value changed.</param>
    /// <param name="baseValue">The value before coercion.</param>
    /// <returns>The value after coercion.</returns>
    public delegate object CoerceValueCallback(DependencyObject d, object baseValue);
}
