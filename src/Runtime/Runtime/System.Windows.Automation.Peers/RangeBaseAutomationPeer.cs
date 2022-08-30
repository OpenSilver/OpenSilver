
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
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls.Primitives;
#endif

#if MIGRATION
namespace System.Windows.Automation.Peers
#else
namespace Windows.UI.Xaml.Automation.Peers
#endif
{
    /// <summary>
    /// Exposes <see cref="RangeBase"/> types to UI automation.
    /// </summary>
    public class RangeBaseAutomationPeer : FrameworkElementAutomationPeer, IRangeValueProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RangeBaseAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="RangeBase"/> to associate with the <see cref="RangeBaseAutomationPeer"/>.
        /// </param>
        public RangeBaseAutomationPeer(RangeBase owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Gets an object that supports the requested pattern, based on the patterns implemented
        /// by this <see cref="RangeBaseAutomationPeer"/>.
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
            if (patternInterface == PatternInterface.RangeValue)
            {
                return this;
            }
            
            return base.GetPattern(patternInterface);
        }

        /// <summary>
        /// Helper function for <see cref="IRangeValueProvider.SetValue"/> to provide a way for 
        /// drive classes to have custom way of implementing it.
        /// </summary>
        /// <param name="val"></param>
        internal virtual void SetValueCore(double val)
        {
            RangeBase owner = (RangeBase)Owner;
            if (val < owner.Minimum || val > owner.Maximum)
            {
                throw new ArgumentOutOfRangeException(nameof(val));
            }

            owner.Value = (double)val;
        }

        /// <summary>
        /// Request to set the value that this UI element is representing
        /// </summary>
        /// <param name="val">Value to set the UI to, as an object</param>
        /// <returns>true if the UI element was successfully set to the specified value</returns>
        void IRangeValueProvider.SetValue(double val)
        {
            if (!IsEnabled())
            {
                throw new ElementNotEnabledException();
            }

            SetValueCore(val);
        }

        /// <summary>Value of a value control, as an object</summary>
        double IRangeValueProvider.Value => ((RangeBase)Owner).Value;

        /// <summary>
        /// Indicates that the value can only be read, not modified.
        /// returns True if the control is read-only
        /// </summary>
        bool IRangeValueProvider.IsReadOnly => !IsEnabled();

        /// <summary>maximum value </summary>
        double IRangeValueProvider.Maximum => ((RangeBase)Owner).Maximum;

        /// <summary>minimum value</summary>
        double IRangeValueProvider.Minimum => ((RangeBase)Owner).Minimum;

        /// <summary>Value of a Large Change</summary>
        double IRangeValueProvider.LargeChange => ((RangeBase)Owner).LargeChange;

        /// <summary>Value of a Small Change</summary>
        double IRangeValueProvider.SmallChange => ((RangeBase)Owner).SmallChange;
    }
}
