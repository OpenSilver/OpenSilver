
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

namespace System.Windows.Automation.Provider
{
    /// <summary>
    /// Exposes methods and properties to support access by UI automation client to 
    /// individual child controls of containers that implement <see cref="IScrollProvider" />.
    /// </summary>
    public interface IScrollItemProvider
    {
        /// <summary>
        /// Scrolls the content area of a container object in order to display the control 
        /// within the visible region (viewport) of the container.
        /// </summary>
        void ScrollIntoView();
    }
}
