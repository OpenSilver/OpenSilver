
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
    /// Specifies the control pattern that the <see cref="AutomationPeer.GetPattern(PatternInterface)" /> method returns.
    /// </summary>
    public enum PatternInterface
    {
        /// <summary>
        /// The <see cref="IInvokeProvider" /> control pattern interface.
        /// </summary>
        Invoke,
        /// <summary>
        /// The <see cref="ISelectionProvider" /> control pattern interface.
        /// </summary>
        Selection,
        /// <summary>
        /// The <see cref="IValueProvider" /> control pattern interface.
        /// </summary>
        Value,
        /// <summary>
        /// The <see cref="IRangeValueProvider" /> control pattern interface.
        /// </summary>
        RangeValue,
        /// <summary>
        /// The <see cref="IScrollProvider" /> control pattern interface.
        /// </summary>
        Scroll,
        /// <summary>
        /// The <see cref="IScrollItemProvider" /> control pattern interface.
        /// </summary>
        ScrollItem,
        /// <summary>
        /// The <see cref="IExpandCollapseProvider" /> control pattern interface.
        /// </summary>
        ExpandCollapse,
        /// <summary>
        /// The <see cref="IGridProvider" /> control pattern interface.
        /// </summary>
        Grid,
        /// <summary>
        /// The <see cref="IGridItemProvider" /> control pattern interface.
        /// </summary>
        GridItem,
        /// <summary>
        /// The <see cref="IMultipleViewProvider" /> control pattern interface.
        /// </summary>
        MultipleView,
        /// <summary>
        /// The <see cref="IWindowProvider" /> control pattern interface.
        /// </summary>
        Window,
        /// <summary>
        /// The <see cref="ISelectionItemProvider" /> control pattern interface.
        /// </summary>
        SelectionItem,
        /// <summary>
        /// The <see cref="IDockProvider" /> control pattern interface.
        /// </summary>
        Dock,
        /// <summary>
        /// The <see cref="ITableProvider" /> control pattern interface.
        /// </summary>
        Table,
        /// <summary>
        /// The <see cref="ITableItemProvider" /> control pattern interface.
        /// </summary>
        TableItem,
        /// <summary>
        /// The <see cref="IToggleProvider" /> control pattern interface.
        /// </summary>
        Toggle,
        /// <summary>
        /// The <see cref="ITransformProvider" /> control pattern interface.
        /// </summary>
        Transform,
        /// <summary>
        /// The <see cref="ITextProvider" /> control pattern interface.
        /// </summary>
        Text,
    }
}
