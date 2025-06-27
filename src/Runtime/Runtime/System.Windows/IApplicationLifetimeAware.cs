
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
/// Defines methods that application extension services can optionally implement in order to respond 
/// to application lifetime events.
/// </summary>
public interface IApplicationLifetimeAware
{
    /// <summary>
    /// Called by an application immediately after the <see cref="Application.Exit"/> event occurs.
    /// </summary>
    void Exited();

    /// <summary>
    /// Called by an application immediately before the <see cref="Application.Exit"/> event occurs.
    /// </summary>
    void Exiting();

    /// <summary>
    /// Called by an application immediately after the <see cref="Application.Startup"/> event occurs.
    /// </summary>
    void Started();

    /// <summary>
    /// Called by an application immediately before the <see cref="Application.Startup"/> event occurs.
    /// </summary>
    void Starting();
}
