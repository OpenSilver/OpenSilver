
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
    /// Exposes methods and properties to support access by a UI automation client to controls 
    /// that visually expand to display content and that collapse to hide content.
    /// </summary>
    public interface IExpandCollapseProvider
    {
        /// <summary>
        /// Hides all nodes, controls, or content that are descendants of the control.
        /// </summary>
        void Collapse();

        /// <summary>
        /// Displays all child nodes, controls, or content of the control.
        /// </summary>
        void Expand();

        /// <summary>
        /// Gets the state (expanded or collapsed) of the control.
        /// </summary>
        /// <returns>
        /// The state (expanded or collapsed) of the control.
        /// </returns>
        ExpandCollapseState ExpandCollapseState { get; }
    }
}
