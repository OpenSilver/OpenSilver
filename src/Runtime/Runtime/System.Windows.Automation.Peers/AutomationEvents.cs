
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

namespace System.Windows.Automation.Peers
{
    /// <summary>
    /// Specifies the event that is raised by the element through the associated <see cref="AutomationPeer" />.
    /// </summary>
    public enum AutomationEvents
    {
        /// <summary>
        /// The event that is raised when a tooltip is opened.
        /// </summary>
        ToolTipOpened,
        /// <summary>
        /// The event that is raised when a tooltip is closed.
        /// </summary>
        ToolTipClosed,
        /// <summary>
        /// The event that is raised when a menu is opened.
        /// </summary>
        MenuOpened,
        /// <summary>
        /// The event that is raised when a menu is closed.
        /// </summary>
        MenuClosed,
        /// <summary>
        /// The event that is raised when the focus has changed. See <see cref="AutomationPeer.SetFocus" />.
        /// </summary>
        AutomationFocusChanged,
        /// <summary>
        /// The event that is raised when a control is activated. See <see cref="IInvokeProvider.Invoke" />.
        /// </summary>
        InvokePatternOnInvoked,
        /// <summary>
        /// The event that is raised when an item is added to a collection of selected items. 
        /// See <see cref="ISelectionItemProvider.AddToSelection" />.
        /// </summary>
        SelectionItemPatternOnElementAddedToSelection,
        /// <summary>
        /// The event that is raised when an item is removed from a collection of selected items. 
        /// See <see cref="ISelectionItemProvider.RemoveFromSelection" />.
        /// </summary>
        SelectionItemPatternOnElementRemovedFromSelection,
        /// <summary>
        /// The event that is raised when a single item is selected (which clears any previous selection). 
        /// See <see cref="ISelectionItemProvider.Select" />.
        /// </summary>
        SelectionItemPatternOnElementSelected,
        /// <summary>
        /// The event that is raised when a selection in a container has changed significantly.
        /// </summary>
        SelectionPatternOnInvalidated,
        /// <summary>
        /// The event that is raised when the text selection is modified.
        /// </summary>
        TextPatternOnTextSelectionChanged,
        /// <summary>
        /// The event that is raised when textual content is modified.
        /// </summary>
        TextPatternOnTextChanged,
        /// <summary>
        /// The event that is raised when content is loaded asynchronously.
        /// </summary>
        AsyncContentLoaded,
        /// <summary>
        /// The event that is raised when a property has changed.
        /// </summary>
        PropertyChanged,
        /// <summary>
        /// The event that is raised when the UI Automation tree structure is changed.
        /// </summary>
        StructureChanged,
    }
}
