
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

#if MIGRATION
namespace System.Windows.Automation.Provider
#else
namespace Windows.UI.Xaml.Automation.Provider
#endif
{
    /// <summary>
    /// Exposes methods and properties to support access by a UI automation client to 
    /// controls that expose their dock properties in a docking container.
    /// </summary>
    public interface IDockProvider
    {
        /// <summary>
        /// Gets the current <see cref="Automation.DockPosition" /> of the control in a 
        /// docking container.
        /// </summary>
        /// <returns>
        /// The <see cref="Automation.DockPosition" /> of the control, relative to the 
        /// boundaries of the docking container and to other elements in the container.
        /// </returns>
        DockPosition DockPosition { get; }

        /// <summary>
        /// Docks the control in a docking container.
        /// </summary>
        /// <param name="dockPosition">
        /// The dock position, relative to the boundaries of the docking container and to 
        /// other elements in the container.
        /// </param>
        void SetDockPosition(DockPosition dockPosition);
    }
}
