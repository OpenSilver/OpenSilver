// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

#if MIGRATION
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
#else
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Exposes ListTimePickerPopup types to UI Automation.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [OpenSilver.NotImplemented]
    public class ListTimePickerPopupAutomationPeer : TimePickerPopupAutomationPeer, ISelectionProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListTimePickerPopupAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public ListTimePickerPopupAutomationPeer(ListTimePickerPopup owner) : base(owner)
        {
        }

        /// <summary>
        /// Gets the ListTimePickerPopup that owns this AutomationPeer.
        /// </summary>
        /// <returns>The ListTimePickerPopup that owns this AutomationPeer.</returns>
        protected override TimePickerPopup TimePickerPopupOwner
        {
            get
            {
                return (ListTimePickerPopup)Owner;
            }
        }

        /// <summary>
        /// Gets the ListTimePickerPopup that owns this AutomationPeer.
        /// </summary>
        private ListTimePickerPopup OwnerListTimePickerPopup
        {
            get
            {
                return (ListTimePickerPopup)TimePickerPopupOwner;
            }
        }

        /// <summary>
        /// Gets the type of the automation control.
        /// </summary>
        /// <returns>The Calendar AutomationControlType.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Calendar;
        }

        /// <summary>
        /// Returns the control pattern for the <see cref="T:System.Windows.UIElement"/>
        /// that is associated with this <see cref="T:System.Windows.Automation.Peers.FrameworkElementAutomationPeer"/>.
        /// </summary>
        /// <param name="patternInterface">One of the enumeration values.</param>
        /// <returns>
        /// Returns an AutomationPeer that can handle the the pattern,
        /// or null.
        /// </returns>
        public override object GetPattern(PatternInterface patternInterface)
        {
            ListBox lb = OwnerListTimePickerPopup.ListBoxPart;

            if (patternInterface == PatternInterface.Selection && lb != null)
            {
                AutomationPeer peer = CreatePeerForElement(lb);
                if (peer != null)
                {
                    return peer.GetPattern(patternInterface);
                }
            }
            return base.GetPattern(patternInterface);
        }

        /// <summary>
        /// Returns the name of the <see cref="T:System.Windows.UIElement"/> 
        /// that is associated with this <see cref="T:System.Windows.Automation.Peers.FrameworkElementAutomationPeer"/>. 
        /// This method is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetClassName"/>.
        /// </summary>
        /// <returns>The string ListTimePickerPopup.</returns>
        protected override string GetClassNameCore()
        {
            return "ListTimePickerPopup";
        }

        /// <summary>
        /// Gets a value indicating whether the UI Automation provider 
        /// allows more than one child element to be selected concurrently.
        /// </summary>
        /// <returns>False.</returns>
        public bool CanSelectMultiple
        {
            get { return false; }
        }

        /// <summary>
        /// Retrieves a UI Automation provider for each child element that is 
        /// selected.
        /// </summary>
        /// <returns>An array of UI Automation providers.</returns>
        public IRawElementProviderSimple[] GetSelection()
        {
            ListBox lb = OwnerListTimePickerPopup.ListBoxPart;

            if (lb != null)
            {
                object selectedItem = lb.SelectedItem;
                if (selectedItem != null)
                {
                    AutomationPeer peer = null;
                    // check if the selecteditem is an UIElement
                    UIElement uie = selectedItem as UIElement;
                    if (uie != null)
                    {
                        peer = CreatePeerForElement(uie);
                    }
                    else
                    {
                        ListBoxAutomationPeer lbpeer = CreatePeerForElement(lb) as ListBoxAutomationPeer;
                        if (lbpeer != null)
                        {
                            List<AutomationPeer> children = lbpeer.GetChildren();
                            if (children != null)
                            {
                                peer = children.FirstOrDefault(child =>
                                                                   {
                                                                       // ISelectionItemProvider.IsSelected does not yet give the correct information
                                                                       ListBoxItemAutomationPeer listboxitemPeer = child as ListBoxItemAutomationPeer;
                                                                       if (listboxitemPeer != null)
                                                                       {
                                                                           ListBoxItem item = listboxitemPeer.Owner as ListBoxItem;
                                                                           return item != null && item.DataContext.Equals(selectedItem);
                                                                       }
                                                                       return false;
                                                                   });
                            }
                        }
                    }

                    // we only ever expect to return an array of one peer.
                    if (peer != null)
                    {
                        return new[] { ProviderFromPeer(peer) };
                    }
                }
            }
            return new IRawElementProviderSimple[] { };
        }

        /// <summary>
        /// Returns the collection of child elements of the <see cref="T:System.Windows.UIElement"/> 
        /// that is associated with this <see cref="T:System.Windows.Automation.Peers.FrameworkElementAutomationPeer"/>. 
        /// This method is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetChildren"/>.
        /// </summary>
        /// <returns>
        /// A list of child <see cref="T:System.Windows.Automation.Peers.AutomationPeer"/> elements.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Baseclass does not follow guidelines.")]
        protected override List<AutomationPeer> GetChildrenCore()
        {
            ListBox lb = OwnerListTimePickerPopup.ListBoxPart;
            if (lb != null)
            {
                // if possible, delegate to ListBox.
                ListBoxAutomationPeer peer = (ListBoxAutomationPeer)CreatePeerForElement(lb);
                return peer.GetChildren();
            }
            else
            {
                return base.GetChildrenCore();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the UI Automation provider 
        /// requires at least one child element to be selected.
        /// </summary>
        /// <returns>False.</returns>
        public bool IsSelectionRequired
        {
            get { return false; }
        }
    }
}
