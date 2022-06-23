// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;

#if MIGRATION
using System.Windows.Automation.Provider;
using System.Windows.Controls;
#else
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls;
#endif

#if OPENSILVER
using Properties = OpenSilver.Controls.Properties;
#endif

#if MIGRATION
namespace System.Windows.Automation.Peers
#else
namespace Windows.UI.Xaml.Automation.Peers
#endif
{
    /// <summary>
    /// Exposes AccordionItem types to UI Automation.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class AccordionItemAutomationPeer : FrameworkElementAutomationPeer, IExpandCollapseProvider, ISelectionItemProvider
    {
        /// <summary>
        /// Gets the AccordionItem that owns this AccordionItemAutomationPeer.
        /// </summary>
        private AccordionItem OwnerAccordionItem
        {
            get { return (AccordionItem)Owner; }
        }

        /// <summary>
        /// Initializes a new instance of the AccordionAutomationPeer class.
        /// </summary>
        /// <param name="owner">
        /// The Accordion that is associated with this
        /// AccordionAutomationPeer.
        /// </param>
        public AccordionItemAutomationPeer(AccordionItem owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Gets the control type for the AccordionItem that is associated
        /// with this AccordionItemAutomationPeer.  This method is called by
        /// GetAutomationControlType.
        /// </summary>
        /// <returns>Custom AutomationControlType.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.ListItem;
        }

        /// <summary>
        /// Gets the name of the AccordionItem that is associated with this
        /// AccordionItemAutomationPeer.  This method is called by GetClassName.
        /// </summary>
        /// <returns>The name AccordionItem.</returns>
        protected override string GetClassNameCore()
        {
            return "AccordionItem";
        }

        /// <summary>
        /// Gets the control pattern for the AccordionItem that is associated
        /// with this AccordionItemAutomationPeer.
        /// </summary>
        /// <param name="patternInterface">The desired PatternInterface.</param>
        /// <returns>The desired AutomationPeer or null.</returns>
        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.ExpandCollapse ||
                patternInterface == PatternInterface.SelectionItem)
            {
                return this;
            }

            return null;
        }

        /// <summary>
        /// Gets the state (expanded or collapsed) of the Accordion.
        /// </summary>
        /// <remarks>
        /// This API supports the .NET Framework infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        ExpandCollapseState IExpandCollapseProvider.ExpandCollapseState
        {
            get
            {
                return OwnerAccordionItem.IsSelected ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed;
            }
        }

        /// <summary>
        /// Collapses the AccordionItem.
        /// </summary>
        /// <remarks>
        /// This API supports the .NET Framework infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        void IExpandCollapseProvider.Collapse()
        {
            if (!IsEnabled())
            {
                throw new ElementNotEnabledException();
            }

            AccordionItem owner = OwnerAccordionItem;
            if (owner.IsLocked)
            {
                throw new InvalidOperationException(Properties.Resources.Automation_OperationCannotBePerformed);
            }

            owner.IsSelected = false;
        }

        /// <summary>
        /// Expands the AccordionItem.
        /// </summary>
        /// <remarks>
        /// This API supports the .NET Framework infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        void IExpandCollapseProvider.Expand()
        {
            if (!IsEnabled())
            {
                throw new ElementNotEnabledException();
            }

            AccordionItem owner = OwnerAccordionItem;
            if (owner.IsLocked)
            {
                throw new InvalidOperationException(Properties.Resources.Automation_OperationCannotBePerformed);
            }

            owner.IsSelected = true;
        }

        /// <summary>
        /// Adds the AccordionItem to the collection of selected items.
        /// </summary>
        /// <remarks>
        /// This API supports the .NET Framework infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        void ISelectionItemProvider.AddToSelection()
        {
            AccordionItem owner = OwnerAccordionItem;
            Accordion parent = owner.ParentAccordion;
            if (parent == null)
            {
                throw new InvalidOperationException(Properties.Resources.Automation_OperationCannotBePerformed);
            }
            parent.SelectedItems.Add(owner);
        }

        /// <summary>
        /// Gets a value indicating whether the Accordion is selected.
        /// </summary>
        /// <remarks>
        /// This API supports the .NET Framework infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        bool ISelectionItemProvider.IsSelected
        {
            get { return OwnerAccordionItem.IsSelected; }
        }

        /// <summary>
        /// Removes the current Accordion from the collection of selected
        /// items.
        /// </summary>
        /// <remarks>
        /// This API supports the .NET Framework infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        void ISelectionItemProvider.RemoveFromSelection()
        {
            AccordionItem owner = OwnerAccordionItem;
            Accordion parent = owner.ParentAccordion;
            if (parent == null)
            {
                throw new InvalidOperationException(Properties.Resources.Automation_OperationCannotBePerformed);
            }
            parent.SelectedItems.Remove(owner);
        }

        /// <summary>
        /// Clears selection from currently selected items and then proceeds to
        /// select the current Accordion.
        /// </summary>
        /// <remarks>
        /// This API supports the .NET Framework infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        void ISelectionItemProvider.Select()
        {
            OwnerAccordionItem.IsSelected = true;
        }

        /// <summary>
        /// Gets the UI Automation provider that implements ISelectionProvider
        /// and acts as the container for the calling object.
        /// </summary>
        /// <remarks>
        /// This API supports the .NET Framework infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        IRawElementProviderSimple ISelectionItemProvider.SelectionContainer
        {
            get
            {
                Accordion parent = OwnerAccordionItem.ParentAccordion;
                if (parent != null)
                {
                    AutomationPeer peer = FromElement(parent);
                    if (peer != null)
                    {
                        return ProviderFromPeer(peer);
                    }
                }
                return null;
            }
        }
    }
}
