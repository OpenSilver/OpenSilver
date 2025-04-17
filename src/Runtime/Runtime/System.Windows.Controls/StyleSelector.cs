
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

namespace System.Windows.Controls;

/// <summary>
/// Provides a way to apply styles based on custom logic.
/// </summary>
public class StyleSelector
{
    /// <summary>
    /// When overridden in a derived class, returns a <see cref="Style"/> based on custom logic.
    /// </summary>
    /// <param name="item">
    /// The content.
    /// </param>
    /// <param name="container">
    /// The element to which the style will be applied.
    /// </param>
    /// <returns>
    /// Returns an application-specific style to apply; otherwise, null.
    /// </returns>
    public virtual Style SelectStyle(object item, DependencyObject container) => null;
}
