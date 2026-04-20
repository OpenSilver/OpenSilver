
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
    /// The container does not handle the keyboard navigation;
    /// each element receives keyboard focus as long as it is a key navigation stop.
    /// </summary>
    [OpenSilver.NotImplemented]
    Continue,

    /// <summary>
    /// The container and all of its child elements as a whole only receive focus once.
    /// Either the first tree child or the ActiveElement receive focus
    /// </summary>
    Once,

    /// <summary>
    /// Depending on the direction of the navigation,
    /// the focus returns to the first or the last item when the end or
    /// the beginning of the container is reached, respectively.
    /// </summary>
    Cycle,

    /// <summary>
    /// No keyboard navigation is allowed inside this container
    /// </summary>
    [OpenSilver.NotImplemented]
    None,

    /// <summary>
    /// Like cycle but does not move past the beginning or end of the container.
    /// </summary>
    [OpenSilver.NotImplemented]
    Contained,

    /// <summary>
    /// TabIndexes are considered on local subtree only inside this container
    /// </summary>
    Local,

    // NOTE: if you add or remove any values in this enum, be sure to update KeyboardNavigation.IsValidKeyNavigationMode()
}