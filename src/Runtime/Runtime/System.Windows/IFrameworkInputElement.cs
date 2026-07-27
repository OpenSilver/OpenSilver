
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

namespace System.Windows;

/// <summary>
/// Declares a namescope contract for framework elements.
/// </summary>
public interface IFrameworkInputElement : IInputElement
{
    /// <summary>
    /// Gets or sets the name of an element.
    /// </summary>
    /// <returns>
    /// The element name, which is unique in the namescope and can be used as an identifier for certain operations.
    /// </returns>
    string Name { get; set; }
}