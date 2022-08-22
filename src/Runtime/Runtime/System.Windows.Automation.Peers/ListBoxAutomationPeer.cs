
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

using System;

#if MIGRATION
using System.Windows.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

#if MIGRATION
namespace System.Windows.Automation.Peers
#else
namespace Windows.UI.Xaml.Automation.Peers
#endif
{
    /// <summary>
    /// Exposes <see cref="ListBox" /> types to UI automation.
    /// </summary>
    [OpenSilver.NotImplemented]
    public class ListBoxAutomationPeer : SelectorAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListBoxAutomationPeer" /> class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="ListBox" /> to associate with the <see cref="ListBoxAutomationPeer" />.
        /// </param>
        public ListBoxAutomationPeer(ListBox owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Gets the control type for the <see cref="ListBox" /> that is associated 
        /// with this <see cref="ListBoxAutomationPeer" />. This method is called by 
        /// <see cref="AutomationPeer.GetAutomationControlType" />.
        /// </summary>
        /// <returns>
        /// A value of the enumeration.
        /// </returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.List;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ItemAutomationPeer" /> for a data item. 
        /// The item peer is aligned with an item in the <see cref="ItemsControl.Items" /> 
        /// collection of the associated <see cref="ListBox" />.
        /// </summary>
        /// <param name="item">
        /// The data item to use as basis for the created <see cref="ItemAutomationPeer" />.
        /// </param>
        /// <returns>
        /// A peer object that provides UI automation support for the specified data item in 
        /// the owner <see cref="ListBox" />.
        /// </returns>
        protected override ItemAutomationPeer CreateItemAutomationPeer(object item)
        {
            return new ListBoxItemAutomationPeer(item, this);
        }

        /// <summary>
        /// Gets the class name of the class that is associated with this 
        /// <see cref="ListBoxAutomationPeer" />. This method is called by 
        /// <see cref="AutomationPeer.GetClassName" />.
        /// </summary>
        /// <returns>
        /// The associated class name.
        /// </returns>
        protected override string GetClassNameCore()
        {
            return nameof(ListBox);
        }
    }
}