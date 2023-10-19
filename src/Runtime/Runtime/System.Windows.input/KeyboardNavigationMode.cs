
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

namespace System.Windows.Input;

/// <summary>
/// Specifies the tabbing behavior across tab stops for a Silverlight tabbing sequence
/// within a container.
/// </summary>
public enum KeyboardNavigationMode
{
    /// <summary>
    /// Tab indexes are considered on the local subtree only inside this container.
    /// </summary>
    Local = 0,
    /// <summary>
    /// Focus returns to the first or the last keyboard navigation stop inside of a container
    /// when the first or last keyboard navigation stop is reached.
    /// </summary>
    Cycle = 1,
    /// <summary>
    /// The container and all of its child elements as a whole receive focus only once.
    /// </summary>
    Once = 2
}