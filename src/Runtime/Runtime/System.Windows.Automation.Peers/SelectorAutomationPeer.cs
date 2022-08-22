
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
using System.Collections.Generic;

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
    /// Exposes <see cref="Selector" /> types to UI automation.
    /// </summary>
    [OpenSilver.NotImplemented]
    public abstract class SelectorAutomationPeer : ItemsControlAutomationPeer, ISelectionProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectorAutomationPeer" /> class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="Selector" /> instance that is associated with this <see cref="SelectorAutomationPeer" />.
        /// </param>
        protected SelectorAutomationPeer(Selector owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Gets the control type for the element that is associated with this <see cref="SelectorAutomationPeer" />.
        /// This method is called by <see cref="AutomationPeer.GetAutomationControlType" />.
        /// </summary>
        /// <returns>
        /// A value of the enumeration.
        /// </returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.List;
        }

        /// <summary>
        /// Gets an object that supports the requested pattern, based on the 
        /// patterns that are supported by this <see cref="SelectorAutomationPeer" />.
        /// </summary>
        /// <param name="patternInterface">
        /// One of the enumeration values.
        /// </param>
        /// <returns>
        /// The object that implements the pattern interface, or null if the 
        /// specified pattern interface is not implemented by this peer.
        /// </returns>
        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Selection)
            {
                return this;
            }

            return base.GetPattern(patternInterface); // ItemsControlAutomationPeer support Scroll pattern
        }

        //-------------------------------------------------------------------
        //
        //  ISelectionProvider
        //
        //-------------------------------------------------------------------

#region ISelectionProvider

        IRawElementProviderSimple[] ISelectionProvider.GetSelection()
        {
            Selector owner = (Selector)Owner;

            int count = owner.SelectedItemsInternal.Count;
            int itemsCount = (owner as ItemsControl).Items.Count;

            if (count > 0 && itemsCount > 0)
            {
                List<IRawElementProviderSimple> selectedProviders = new List<IRawElementProviderSimple>(count);

                for (int i = 0; i < count; i++)
                {
                    //SelectorItemAutomationPeer peer = FindOrCreateItemAutomationPeer(owner.SelectedItemsInternal[i].Item) as SelectorItemAutomationPeer;
                    SelectorItemAutomationPeer peer = null;
                    if (peer != null)
                    {
                        selectedProviders.Add(ProviderFromPeer(peer));
                    }
                }
                return selectedProviders.ToArray();
            }
            return null;
        }

        bool ISelectionProvider.CanSelectMultiple
        {
            get
            {
                Selector owner = (Selector)Owner;
                return owner.CanSelectMultiple;
            }
        }

        bool ISelectionProvider.IsSelectionRequired
        {
            get
            {
                return false;
            }
        }

#endregion

    }
}