
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
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
#endif

#if MIGRATION
namespace System.Windows.Automation.Peers
#else
namespace Windows.UI.Xaml.Automation.Peers
#endif
{
    /// <summary>
    /// Exposes the items in a <see cref="Selector" /> to UI automation.
    /// </summary>
    [OpenSilver.NotImplemented]
    public abstract class SelectorItemAutomationPeer : ItemAutomationPeer, ISelectionItemProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectorItemAutomationPeer" /> class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="UIElement" /> instance to associate with this <see cref="SelectorItemAutomationPeer" />.
        /// </param>
        protected SelectorItemAutomationPeer(UIElement owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectorItemAutomationPeer" />  using the 
        /// specified selector automation peer.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="UIElement" /> instance to associate with this <see cref="SelectorItemAutomationPeer" />.
        /// </param>
        /// <param name="selectorAutomationPeer">
        /// The <see cref="SelectorAutomationPeer" /> that is associated with the control that holds the 
        /// <see cref="ItemsControl.Items" /> collection.
        /// </param>
        protected SelectorItemAutomationPeer(object owner, SelectorAutomationPeer selectorAutomationPeer)
          : base(owner, selectorAutomationPeer)
        {
        }

        /// <summary>
        /// Gets a object that supports the requested pattern, based on the patterns supported 
        /// by this <see cref="SelectorItemAutomationPeer" />.
        /// </summary>
        /// <param name="patternInterface">
        /// One of the enumeration values.
        /// </param>
        /// <returns>
        /// The object that implements the pattern interface, or null if the specified pattern 
        /// interface is not implemented by this peer.
        /// </returns>
        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.SelectionItem)
            {
                return this;
            }
            return base.GetPattern(patternInterface);
        }

        /// <summary>
        /// Sets the current element as the selection
        /// This clears the selection from other elements in the container
        /// </summary>
        void ISelectionItemProvider.Select()
        {
            if (!IsEnabled())
                throw new ElementNotEnabledException();

            Selector parentSelector = (Selector)(ItemsControlAutomationPeer.Owner);
            if (parentSelector == null)
            {
                throw new InvalidOperationException("Cannot perform operation.");
            }

            parentSelector.SelectionChange.SelectJustThisItem(parentSelector.NewItemInfo(Item), true /* assumeInItemsCollection */);
        }


        /// <summary>
        /// Adds current element to selection
        /// </summary>
        void ISelectionItemProvider.AddToSelection()
        {
            if (!IsEnabled())
                throw new ElementNotEnabledException();

            Selector parentSelector = (Selector)(ItemsControlAutomationPeer.Owner);
            if ((parentSelector == null) || (!parentSelector.CanSelectMultiple && parentSelector.SelectedItem != null && parentSelector.SelectedItem != Item))
            {
                // Parent must exist and be multi-select
                // in single-select mode the selected item should be null or Owner
                throw new InvalidOperationException("Cannot perform operation.");
            }

            parentSelector.SelectionChange.Begin();
            parentSelector.SelectionChange.Select(parentSelector.NewItemInfo(Item), true);
            parentSelector.SelectionChange.End();
        }


        /// <summary>
        /// Removes current element from selection
        /// </summary>
        void ISelectionItemProvider.RemoveFromSelection()
        {
            if (!IsEnabled())
                throw new ElementNotEnabledException();

            Selector parentSelector = (Selector)(ItemsControlAutomationPeer.Owner);

            parentSelector.SelectionChange.Begin();
            parentSelector.SelectionChange.Unselect(parentSelector.NewItemInfo(Item));
            parentSelector.SelectionChange.End();
        }


        /// <summary>
        /// Check whether an element is selected
        /// </summary>
        /// <value>returns true if the element is selected</value>
        bool ISelectionItemProvider.IsSelected
        {
            get
            {
                Selector parentSelector = (Selector)(ItemsControlAutomationPeer.Owner);
                return parentSelector.SelectedItemsInternal.Contains(parentSelector.NewItemInfo(Item));
            }
        }


        /// <summary>
        /// The logical element that supports the SelectionPattern for this Item
        /// </summary>
        /// <value>returns an IRawElementProviderSimple</value>
        IRawElementProviderSimple ISelectionItemProvider.SelectionContainer
        {
            get
            {
                return ProviderFromPeer(ItemsControlAutomationPeer);
            }
        }
    }
}