//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if WORKINPROGRESS && OPENSILVER

using System;

#if MIGRATION
using System.Windows.Controls;
using System.Windows.Automation.Provider;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Automation.Provider;
#endif

#if MIGRATION
namespace System.Windows.Automation.Peers
#else
namespace Windows.UI.Xaml.Automation.Peers
#endif
{
    /// <summary>
    /// Exposes <see cref="ValidationSummary" /> types to UI Automation.
    /// </summary>
    public class ValidationSummaryAutomationPeer : FrameworkElementAutomationPeer, IInvokeProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationSummaryAutomationPeer" /> class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="ValidationSummary" /> that is associated with this <see cref="ValidationSummaryAutomationPeer" />.
        /// </param>
        public ValidationSummaryAutomationPeer(ValidationSummary owner) : base(owner) { }

        /// <summary>
        /// Called by GetClassName that gets a human readable name that, in addition to AutomationControlType, 
        /// differentiates the control represented by this AutomationPeer.
        /// </summary>
        /// <returns>The string that contains the name.</returns>
        protected override string GetClassNameCore()
        {
            return typeof(ValidationSummary).Name;
        }

        /// <summary>
        /// Called by GetName that gets a human readable name that, in addition to AutomationControlType, 
        /// differentiates the control represented by this AutomationPeer.
        /// </summary>
        /// <returns>The string that contains the name.</returns>
        protected override string GetNameCore()
        {
            ValidationSummary vs = Owner as ValidationSummary;
            if (vs != null && vs.HeaderContentControlInternal != null)
            {
                string stringContent = vs.HeaderContentControlInternal.Content as String;
                if (stringContent != null)
                {
                    return stringContent;
                }
            }
            return base.GetNameCore();
        }

        /// <summary>
        /// Gets the control pattern that is associated with the specified System.Windows.Automation.Peers.PatternInterface.
        /// </summary>
        /// <param name="patternInterface">A value from the System.Windows.Automation.Peers.PatternInterface enumeration.</param>
        /// <returns>The object that supports the specified pattern, or null if unsupported.</returns>
        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Invoke)
            {
                return this;
            }
            return base.GetPattern(patternInterface);
        }

        /// <summary>
        /// Gets the control type for the element that is associated with the UI Automation peer.
        /// </summary>
        /// <returns>The control type.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Group;
        }

        #region IInvokeProvider Members

        /// <summary>
        /// Invoke a selection on the current item, simulating a click.
        /// </summary>
        void IInvokeProvider.Invoke()
        {
            ValidationSummary vs = Owner as ValidationSummary;
            if (vs != null)
            {
                vs.ExecuteClickInternal();
            }
        }

        #endregion
    }
}

#endif // WORKINPROGRESS && OPENSILVER