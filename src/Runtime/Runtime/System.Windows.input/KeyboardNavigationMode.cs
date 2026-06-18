
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
/// Specifies the possible values for changes in focus when logical and directional navigation occurs.
/// </summary>
public enum KeyboardNavigationMode
{
    /// <summary>
    /// Each element receives keyboard focus, as long as it is a navigation stop. Navigation leaves the 
    /// containing element when an edge is reached.
    /// </summary>
    Continue = 0,

    /// <summary>
    /// The container and all of its child elements as a whole receive focus only once. Either the first 
    /// tree child or the or the last focused element in the group receives focus.
    /// </summary>
    Once = 1,

    /// <summary>
    /// Depending on the direction of the navigation, the focus returns to the first or the last item 
    /// when the end or the beginning of the container is reached. Focus cannot leave the container using 
    /// logical navigation.
    /// </summary>
    Cycle = 2,

    /// <summary>
    /// No keyboard navigation is allowed inside this container.
    /// </summary>
    None = 3,

    /// <summary>
    /// Depending on the direction of the navigation, focus returns to the first or the last item when the 
    /// end or the beginning of the container is reached, but does not move past the beginning or end of 
    /// the container.
    /// </summary>
    Contained = 4,

    /// <summary>
    /// Tab Indexes are considered on local subtree only inside this container and behave like <see cref="Continue"/> 
    /// after that.
    /// </summary>
    Local = 5,
}