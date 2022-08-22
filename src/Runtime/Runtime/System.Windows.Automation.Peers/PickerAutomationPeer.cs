// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

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
    /// Exposes Picker types to UI Automation.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [OpenSilver.NotImplemented]
    public abstract class PickerAutomationPeer : FrameworkElementAutomationPeer, IExpandCollapseProvider
    {
        /// <summary>
        /// Gets the Picker that owns this AutomationPeer.
        /// </summary>
        private Picker OwnerPicker
        {
            get
            {
                return (Picker)Owner;
            }
        }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="PickerAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">The Picker that is associated to this 
        /// AutomationPeer.</param>
        protected PickerAutomationPeer(Picker owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Gets the control type for the Picker that is associated
        /// with this PickerAutomationPeer. This method is called by
        /// GetAutomationControlType.
        /// </summary>
        /// <returns>ComboBox AutomationControlType.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.ComboBox;
        }

        /// <summary>
        /// Gets the name of the Picker that is associated with this
        /// PickerAutomationPeer. This method is called by
        /// GetClassName.
        /// </summary>
        /// <returns>The name Picker.</returns>
        protected override string GetClassNameCore()
        {
            return "Picker";
        }

        /// <summary>
        /// Returns the control pattern for the <see cref="UIElement"/> 
        /// that is associated with this <see cref="FrameworkElementAutomationPeer"/>.
        /// </summary>
        /// <param name="patternInterface">One of the enumeration values.</param>
        /// <returns>Returns an AutomationPeer that can handle the the pattern,
        /// or null.</returns>
        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.ExpandCollapse)
            {
                return this;
            }
            return base.GetPattern(patternInterface);
        }

        #region ExpandCollapse
        /// <summary>
        /// Blocking method that returns after the element has been expanded.
        /// </summary>
        /// <remarks>
        /// This API supports the .NET Framework infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        public void Expand()
        {
            if (!IsEnabled())
            {
                throw new ElementNotEnabledException();
            }

            OwnerPicker.IsDropDownOpen = true;
        }

        /// <summary>
        /// Blocking method that returns after the element has been collapsed.
        /// </summary>
        /// <remarks>
        /// This API supports the .NET Framework infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        public void Collapse()
        {
            if (!IsEnabled())
            {
                throw new ElementNotEnabledException();
            }

            OwnerPicker.IsDropDownOpen = false;
        }

        /// <summary>
        /// Gets an element's current Collapsed or Expanded state.
        /// </summary>
        /// <remarks>
        /// This API supports the .NET Framework infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        public ExpandCollapseState ExpandCollapseState
        {
            get
            {
                return OwnerPicker.IsDropDownOpen ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed;
            }
        }

        /// <summary>
        /// Raises the ExpandCollapse automation event.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        internal void RaiseExpandCollapseAutomationEvent(bool oldValue, bool newValue)
        {
            RaisePropertyChangedEvent(
                ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
                oldValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed,
                newValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed);
        }
        #endregion ExpandCollapse
    }
}
