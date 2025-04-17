
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
/// Provides a way to choose a <see cref="DataTemplate"/> based on the data object and the data-bound element.
/// </summary>
public class DataTemplateSelector
{
    /// <summary>
    /// When overridden in a derived class, returns a <see cref="DataTemplate"/> based on custom logic.
    /// </summary>
    /// <param name="item">
    /// The data object for which to select the template.
    /// </param>
    /// <param name="container">
    /// The data-bound object.
    /// </param>
    /// <returns>
    /// Returns a <see cref="DataTemplate"/> or null. The default value is null.
    /// </returns>
    public virtual DataTemplate SelectTemplate(object item, DependencyObject container) => null;
}
