// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#if OPENSILVER

using System.Diagnostics.CodeAnalysis;

#if MIGRATION
using System.Windows.Automation.Provider;
using System.Windows.Controls;
#else
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls;
#endif

[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.ExpanderAutomationPeer.#System.Windows.Automation.Provider.IExpandCollapseProvider.Collapse()", Justification = "Required for subset compat with WPF")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.ExpanderAutomationPeer.#System.Windows.Automation.Provider.IExpandCollapseProvider.Expand()", Justification = "Required for subset compat with WPF")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.ExpanderAutomationPeer.#System.Windows.Automation.Provider.IExpandCollapseProvider.ExpandCollapseState", Justification = "Required for subset compat with WPF")]

#if MIGRATION
namespace System.Windows.Automation.Peers
#else
namespace Windows.UI.Xaml.Automation.Peers
#endif
{
    /// <summary>
    /// Exposes <see cref="Expander" /> types to UI
    /// automation.
    /// </summary>
    /// <QualityBand>Stable</QualityBand>
    public partial class ExpanderAutomationPeer
        : FrameworkElementAutomationPeer, IExpandCollapseProvider
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ExpanderAutomationPeer" />
        /// class.
        /// </summary>
        /// <param name="owner">
        /// The element associated with this automation peer.
        /// </param>
        public ExpanderAutomationPeer(Expander owner)
            : base(owner)
        {
        }

        #region AutomationPeer overrides
        /// <summary>
        /// Gets the control type for the
        /// <see cref="Expander" /> that is associated
        /// with this
        /// <see cref="ExpanderAutomationPeer" />.
        /// This method is called by
        /// <see cref="AutomationPeer.GetAutomationControlType" />.
        /// </summary>
        /// <returns>
        /// The
        /// <see cref="AutomationControlType.Group" />
        /// enumeration value.
        /// </returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Group;
        }

        /// <summary>
        /// Gets the name of the
        /// <see cref="Expander" /> that is associated
        /// with this
        /// <see cref="ExpanderAutomationPeer" />.
        /// This method is called by
        /// <see cref="AutomationPeer.GetClassName" />.
        /// </summary>
        /// <returns>A string that contains Expander.</returns>
        protected override string GetClassNameCore()
        {
            return "Expander";
        }

        /// <summary>
        /// Gets the control pattern for the
        /// <see cref="Expander" /> that is associated
        /// with this
        /// <see cref="ExpanderAutomationPeer" />.
        /// </summary>
        /// <param name="pattern">One of the enumeration values.</param>
        /// <returns>
        /// The object that implements the pattern interface, or null if the
        /// specified pattern interface is not implemented by this peer.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "Compat with WPF.")]
        public override object GetPattern(PatternInterface pattern)
        {
            if (pattern == PatternInterface.ExpandCollapse)
            {
                return this;
            }

            return null;
        }
        #endregion

        #region Implement IExpandCollapseProvider
        /// <summary>
        /// Displays all child nodes, controls, or content of the control.
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

            Expander owner = (Expander)Owner;
            owner.IsExpanded = true;
        }

        /// <summary>
        /// Hides all nodes, controls, or content that are descendants of the
        /// control.
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

            Expander owner = (Expander)Owner;
            owner.IsExpanded = false;
        }

        /// <summary>
        /// Gets the state (expanded or collapsed) of the control.
        /// </summary>
        /// <value>
        /// The state (expanded or collapsed) of the control.
        /// </value>
        /// <remarks>
        /// This API supports the .NET Framework infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        ExpandCollapseState IExpandCollapseProvider.ExpandCollapseState
        {
            get
            {
                Expander owner = (Expander)Owner;
                return owner.IsExpanded ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed;
            }
        }
        #endregion

        /// <summary>
        /// Raise an automation event when a Expander is expanded or collapsed.
        /// </summary>
        /// <param name="oldValue">
        /// A value indicating whether the Expander was expanded.
        /// </param>
        /// <param name="newValue">
        /// A value indicating whether the Expander is expanded.
        /// </param>
        internal void RaiseExpandCollapseAutomationEvent(bool oldValue, bool newValue)
        {
            RaisePropertyChangedEvent(
                ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
                oldValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed,
                newValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed);
        }
    }
}

#endif