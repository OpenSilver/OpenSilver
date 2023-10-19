// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Automation.Provider;
using System.Windows.Controls;

namespace System.Windows.Automation.Peers
{
    /// <summary>
    /// Exposes Accordion types to UI Automation.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class AccordionAutomationPeer : ItemsControlAutomationPeer, ISelectionProvider
    {
        /// <summary>
        /// Gets the Accordion that owns this AccordionAutomationPeer.
        /// </summary>
        /// <value>The accordion.</value>
        private Accordion OwnerAccordion { get { return (Accordion)Owner; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccordionAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">The Accordion that is associated with this
        /// AccordionAutomationPeer.</param>
        public AccordionAutomationPeer(Accordion owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Gets the name of the Accordion that is associated with this
        /// AccordionAutomationPeer.  This method is called by GetClassName.
        /// </summary>
        /// <returns>The name Accordion.</returns>
        protected override string GetClassNameCore()
        {
            return "Accordion";
        }

        /// <summary>
        /// Gets the control type for the Accordion that is associated
        /// with this AccordionAutomationPeer.  This method is called by
        /// GetAutomationControlType.
        /// </summary>
        /// <returns>List AutomationControlType.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.List;
        }

        /// <summary>
        /// Gets the control pattern for the Accordion that is associated
        /// with this AccordionAutomationPeer.
        /// </summary>
        /// <param name="patternInterface">The desired PatternInterface.</param>
        /// <returns>The desired AutomationPeer or null.</returns>
        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Selection)
            {
                return this;
            }

            return base.GetPattern(patternInterface);
        }

        /// <summary>
        /// Gets the collection of child elements of 
        /// the <see cref="T:System.Windows.Controls.ItemsControl"/> that is 
        /// associated with this <see cref="T:System.Windows.Automation.Peers.ItemsControlAutomationPeer"/>.
        /// </summary>
        /// <returns>
        /// A collection of AccordionItemAutomationPeer elements, or null if the
        /// Accordion that is associated with this AccordionAutomationPeer is
        /// empty.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Required by automation")]
        protected override List<AutomationPeer> GetChildrenCore()
        {
            Accordion owner = OwnerAccordion;

            ItemCollection items = owner.Items;
            if (items.Count <= 0)
            {
                return null;
            }

            List<AutomationPeer> peers = new List<AutomationPeer>(items.Count);
            for (int i = 0; i < items.Count; i++)
            {
                AccordionItem element = owner.ItemContainerGenerator.ContainerFromIndex(i) as AccordionItem;
                if (element != null)
                {
                    peers.Add(FromElement(element) ?? CreatePeerForElement(element));
                }
            }
            return peers;
        }

        /// <summary>
        /// Gets a value indicating whether the UI Automation provider 
        /// allows more than one child element to be selected concurrently.
        /// </summary>
        /// <returns>true if multiple selection is allowed; otherwise, false.
        /// </returns>
        /// <remarks>
        /// This API supports the .NET Framework infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        public bool CanSelectMultiple
        {
            get
            {
                return OwnerAccordion.SelectionMode == AccordionSelectionMode.OneOrMore ||
                    OwnerAccordion.SelectionMode == AccordionSelectionMode.ZeroOrMore;
            }
        }

        /// <summary>
        /// Retrieves a UI Automation provider for each child element that is 
        /// selected.
        /// </summary>
        /// <returns>An array of UI Automation providers.</returns>
        /// <remarks>
        /// This API supports the .NET Framework infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        public IRawElementProviderSimple[] GetSelection()
        {
            Accordion owner = OwnerAccordion;

            List<IRawElementProviderSimple> selection = new List<IRawElementProviderSimple>(owner.SelectedIndices.Count);

            foreach (int index in owner.SelectedIndices)
            {
                AccordionItem item = owner.ItemContainerGenerator.ContainerFromIndex(index) as AccordionItem;
                if (item != null)
                {
                    AutomationPeer peer = FromElement(item);
                    if (peer != null)
                    {
                        selection.Add(ProviderFromPeer(peer));
                    }
                }
            }
            return selection.ToArray();
        }

        /// <summary>
        /// Gets a value indicating whether the UI Automation provider 
        /// requires at least one child element to be selected.
        /// </summary>
        /// <returns>true if selection is required; otherwise, false.
        /// </returns>
        /// <remarks>
        /// This API supports the .NET Framework infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        public bool IsSelectionRequired
        {
            get
            {
                return OwnerAccordion.SelectionMode == AccordionSelectionMode.One ||
                       OwnerAccordion.SelectionMode == AccordionSelectionMode.OneOrMore;
            }
        }
    }
}
