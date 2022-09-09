
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
    /// Exposes <see cref="ToggleButton"/> types to UI automation.
    /// </summary>
    public class ToggleButtonAutomationPeer : ButtonBaseAutomationPeer, IToggleProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToggleButtonAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="ToggleButton"/> to associate with this <see cref="ToggleButtonAutomationPeer"/>.
        /// </param>
        public ToggleButtonAutomationPeer(ToggleButton owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Gets an object that supports the requested pattern, based on the patterns supported
        /// by this <see cref="ToggleButtonAutomationPeer"/>.
        /// </summary>
        /// <param name="patternInterface">
        /// One of the enumeration values that indicates the control pattern.
        /// </param>
        /// <returns>
        /// The object that implements the pattern interface, or null if the specified pattern
        /// interface is not implemented by this peer.
        /// </returns>
        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Toggle)
            {
                return this;
            }

            return base.GetPattern(patternInterface);
        }

        /// <summary>
        /// Gets the control type for the element that is associated with this <see cref="ToggleButtonAutomationPeer"/>.
        /// This method is called by <see cref="AutomationPeer.GetAutomationControlType"/>.
        /// </summary>
        /// <returns>
        /// A value of the enumeration.
        /// </returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
            => AutomationControlType.Button;

        /// <summary>
        /// Gets the name of the class that is associated with this <see cref="ToggleButtonAutomationPeer"/>.
        /// This method is called by <see cref="AutomationPeer.GetClassName"/>.
        /// </summary>
        /// <returns>
        /// The name of the associated class.
        /// </returns>
        protected override string GetClassNameCore() => "Button";

        void IToggleProvider.Toggle()
        {
            if (!IsEnabled())
            {
                throw new ElementNotEnabledException();
            }

            ((ToggleButton)Owner).OnToggle();
        }

        ToggleState IToggleProvider.ToggleState
            => ConvertToToggleState(((ToggleButton)Owner).IsChecked);

        private static ToggleState ConvertToToggleState(bool? value)
        {
            switch (value)
            {
                case (true): return ToggleState.On;
                case (false): return ToggleState.Off;
                default: return ToggleState.Indeterminate;
            }
        }
    }
}
