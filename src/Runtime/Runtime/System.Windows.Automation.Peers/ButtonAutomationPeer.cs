
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

using System.Windows.Automation.Provider;
using System.Windows.Controls;

namespace System.Windows.Automation.Peers
{
    /// <summary>
    /// Exposes <see cref="Button"/> types to UI automation.
    /// </summary>
    public class ButtonAutomationPeer : ButtonBaseAutomationPeer, IInvokeProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">
        /// The element associated with this automation peer.
        /// </param>
        public ButtonAutomationPeer(Button owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Gets an object that supports the specified pattern, based on the patterns supported
        /// by this <see cref="ButtonAutomationPeer"/>.
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
            if (patternInterface == PatternInterface.Invoke)
            {
                return this;
            }
            
            return base.GetPattern(patternInterface);
        }

        /// <summary>
        /// Gets the control type for the element that is associated with this <see cref="ButtonAutomationPeer"/>.
        /// This method is called by <see cref="AutomationPeer.GetAutomationControlType"/>.
        /// </summary>
        /// <returns>
        /// A value of the enumeration.
        /// </returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
            => AutomationControlType.Button;

        /// <summary>
        /// Gets the name of the class that is associated with this UI automation peer.
        /// </summary>
        /// <returns>
        /// The class name.
        /// </returns>
        protected override string GetClassNameCore() => "Button";

        void IInvokeProvider.Invoke()
        {
            if (!IsEnabled())
            {
                throw new ElementNotEnabledException();
            }

            if (Dispatcher.CheckAccess())
            {
                ((Button)Owner).AutomationButtonBaseClick();
            }
            else
            {
                Dispatcher.BeginInvoke(() => ((Button)Owner).AutomationButtonBaseClick());
            }
        }
    }
}
