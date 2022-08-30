
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
    /// Exposes <see cref="ProgressBar"/> types to UI automation.
    /// </summary>
    public class ProgressBarAutomationPeer : RangeBaseAutomationPeer, IRangeValueProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressBarAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="ProgressBar"/> to associate with the <see cref="ProgressBarAutomationPeer"/>.
        /// </param>
        public ProgressBarAutomationPeer(ProgressBar owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Gets an object that supports the requested pattern, based on the patterns supported
        /// by this <see cref="ProgressBarAutomationPeer"/>.
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
            // Indeterminate ProgressBar should not support RangeValue pattern
            if (patternInterface == PatternInterface.RangeValue && ((ProgressBar)Owner).IsIndeterminate)
            {
                return null;
            }

            return base.GetPattern(patternInterface);
        }

        /// <summary>
        /// Gets the control type for the element that is associated with this <see cref="ProgressBarAutomationPeer"/>.
        /// This method is called by <see cref="AutomationPeer.GetAutomationControlType"/>.
        /// </summary>
        /// <returns>
        /// A value of the enumeration.
        /// </returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
            => AutomationControlType.ProgressBar;

        /// <summary>
        /// Gets the name of the class that is associated with this <see cref="ProgressBarAutomationPeer"/>.
        /// This method is called by <see cref="AutomationPeer.GetClassName"/>.
        /// </summary>
        /// <returns>
        /// The class name.
        /// </returns>
        protected override string GetClassNameCore() => "ProgressBar";

        /// <summary>
        /// Request to set the value that this UI element is representing
        /// </summary>
        /// <param name="val">Value to set the UI to, as an object</param>
        /// <returns>true if the UI element was successfully set to the specified value</returns>
        void IRangeValueProvider.SetValue(double val)
            => throw new InvalidOperationException("Cannot perform operation.");

        /// <summary>
        /// Indicates that the value can only be read, not modified.
        /// returns True if the control is read-only
        /// </summary>
        bool IRangeValueProvider.IsReadOnly => true;

        /// <summary>Value of a Large Change</summary>
        double IRangeValueProvider.LargeChange => double.NaN;

        /// <summary>Value of a Small Change</summary>
        double IRangeValueProvider.SmallChange => double.NaN;
    }
}
