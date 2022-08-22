
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
using System.Windows.Automation.Provider;
using System.Windows.Controls;
#else
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls;
#endif

#if MIGRATION
namespace System.Windows.Automation.Peers
#else
namespace Windows.UI.Xaml.Automation.Peers
#endif
{
    /// <summary>
    /// Exposes the items in the <see cref="ItemsControl.Items" /> collection 
    /// of a <see cref="ListBox" /> to UI automation.
    /// </summary>
    [OpenSilver.NotImplemented]
    public class ListBoxItemAutomationPeer : SelectorItemAutomationPeer, IScrollItemProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListBoxItemAutomationPeer" /> class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="ListBoxItem" /> instance that is associated with this <see cref="ListBoxItemAutomationPeer" />.
        /// </param>
        public ListBoxItemAutomationPeer(ListBoxItem owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListBoxItemAutomationPeer" /> class using the specified 
        /// selector automation peer.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="ListBoxItem" /> instance that is associated with this <see cref="ListBoxItemAutomationPeer" />.
        /// </param>
        /// <param name="selectorAutomationPeer">
        /// The <see cref="SelectorAutomationPeer" /> that is associated with the <see cref="ListBox" /> that holds the 
        /// <see cref="ItemsControl.Items" /> collection.
        /// </param>
        public ListBoxItemAutomationPeer(object owner, SelectorAutomationPeer selectorAutomationPeer)
            : base(owner, selectorAutomationPeer)
        {
        }

        /// <summary>
        /// Gets the name of the class that is associated with this <see cref="ListBoxItemAutomationPeer" />.
        /// This method is called by <see cref="AutomationPeer.GetClassName" />.
        /// </summary>
        /// <returns>
        /// The class name.
        /// </returns>
        protected override string GetClassNameCore()
        {
            return nameof(ListBoxItem);
        }

        /// <summary>
        /// Gets the control type for the element that is associated with this <see cref="ListBoxItemAutomationPeer" />.
        /// This method is called by <see cref="AutomationPeer.GetAutomationControlType" />.
        /// </summary>
        /// <returns>
        /// A value of the enumeration.
        /// </returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.ListItem;
        }

        /// <summary>
        /// Gets an object that supports the requested pattern, based on the patterns supported by this 
        /// <see cref="ListBoxItemAutomationPeer" />.
        /// </summary>
        /// <param name="patternInterface">
        /// One of the enumeration values.
        /// </param>
        /// <returns>
        /// The object that implements the pattern interface, or null if the specified pattern interface 
        /// is not implemented by this peer.
        /// </returns>
        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.ScrollItem)
            {
                return this;
            }
            return base.GetPattern(patternInterface);
        }

        void IScrollItemProvider.ScrollIntoView()
        {
            if (ItemsControlAutomationPeer.Owner is ListBox parent)
            {
                parent.ScrollIntoView(Item);
            }
        }
    }
}