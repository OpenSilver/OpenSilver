
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

using System.Windows.Automation.Provider;

namespace System.Windows.Automation
{
    /// <summary>
    /// Contains values that specify the <see cref="IExpandCollapseProvider.ExpandCollapseState" /> 
    /// automation property value of a UI automation element.
    /// </summary>
    public enum ExpandCollapseState
    {
        /// <summary>
        /// No child nodes, controls, or content of the UI automation element are displayed.
        /// </summary>
        Collapsed,
        /// <summary>
        /// All child nodes, controls or content of the UI automation element are displayed.
        /// </summary>
        Expanded,
        /// <summary>
        /// Some, but not all, child nodes, controls, or content of the UI automation element are displayed.
        /// </summary>
        PartiallyExpanded,
        /// <summary>
        /// The UI automation element has no child nodes, controls, or content to display.
        /// </summary>
        LeafNode,
    }
}
