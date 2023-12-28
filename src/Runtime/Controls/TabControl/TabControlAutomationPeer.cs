// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Windows.Automation.Provider;
using System.Windows.Controls;

namespace System.Windows.Automation.Peers
{
    /// <summary>
    /// Exposes <see cref="TabControl" /> types to UI automation.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    public class TabControlAutomationPeer : ItemsControlAutomationPeer, ISelectionProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TabControlAutomationPeer" /> class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="TabControl" /> that is associated with this <see cref="TabControlAutomationPeer" />.
        /// </param>
        public TabControlAutomationPeer(TabControl owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Creates a new <see cref="TabItemAutomationPeer"/>.
        /// </summary>
        /// <param name="item">
        /// The <see cref="TabItem"/> that is associated with the new <see cref="TabItemAutomationPeer"/>.
        /// </param>
        /// <returns>The <see cref="TabItemAutomationPeer"/> that is created.</returns>
        private static new ItemAutomationPeer CreateItemAutomationPeer(object item)
        {
            return new TabItemAutomationPeer(item);
        }

        /// <summary>
        /// Gets the control type for the element that is associated with the UI
        /// Automation peer.
        /// </summary>
        /// <returns>The control type.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Tab;
        }

        /// <summary>
        /// Called by GetClassName that gets a human readable name that, in
        /// addition to AutomationControlType, differentiates the control
        /// represented by this AutomationPeer.
        /// </summary>
        /// <returns>The string that contains the name.</returns>
        protected override string GetClassNameCore()
        {
            return Owner.GetType().Name;
        }

        /// <summary>
        /// This method is called by <see cref="AutomationPeer.GetClickablePoint"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Point"/> containing <see cref="double.NaN"/>, <see cref="double.NaN"/>;
        /// the only clickable points in a <see cref="TabControl"/> are the child <see cref="TabItem"/> 
        /// elements.
        /// </returns>
        protected override Point GetClickablePointCore()
        {
            return new Point(double.NaN, double.NaN);
        }

        /// <summary>
        /// Gets the control pattern for the <see cref="TabControl" /> that is
        /// associated with this <see cref="TabControlAutomationPeer" />.
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
            return base.GetPattern(patternInterface);
        }

        /// <summary>
        /// Gets a value that indicates whether the element can accept keyboard focus.
        /// </summary>
        /// <returns>True if the element can accept keyboard focus; otherwise, false.</returns>
        protected override bool IsKeyboardFocusableCore()
        {
            TabControl tabControl = this.Owner as TabControl;
            System.Diagnostics.Debug.Assert(tabControl != null, "Owner should be a non-null TabControl");
            return tabControl.IsEnabled && tabControl.IsTabStop;
        }

        /// <summary>
        /// Gets a value indicating whether the UI automation provider
        /// allows more than one child element to be selected concurrently.
        /// </summary>
        /// <value>
        /// True if multiple selection is allowed; otherwise, false.
        /// </value>
        bool ISelectionProvider.CanSelectMultiple
        {
            get { return false; }
        }

        /// <summary>
        /// Retrieves a UI automation provider for each child element that is
        /// selected.
        /// </summary>
        /// <returns>An array of UI automation providers.</returns>
        IRawElementProviderSimple[] ISelectionProvider.GetSelection()
        {
            TabControl tabControl = Owner as TabControl;
            if (tabControl.SelectedItem == null)
            {
                return null;
            }
            List<IRawElementProviderSimple> list = new List<IRawElementProviderSimple>(1);

            ItemAutomationPeer peer = CreateItemAutomationPeer(tabControl.SelectedItem);

            if (peer != null)
            {
                list.Add(ProviderFromPeer(peer));
            }
            return list.ToArray();
        }

        /// <summary>
        /// Gets a value indicating whether the UI automation provider
        /// requires at least one child element to be selected.
        /// </summary>
        /// <value>
        /// True if selection is required; otherwise, false.
        /// </value>
        bool ISelectionProvider.IsSelectionRequired
        {
            get { return true; }
        }
    }
}
