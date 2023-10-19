
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
    /// Exposes <see cref="HyperlinkButton"/> types to UI automation.
    /// </summary>
    public class HyperlinkButtonAutomationPeer : ButtonBaseAutomationPeer, IInvokeProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HyperlinkButtonAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="HyperlinkButton"/> to associate with the <see cref="HyperlinkButtonAutomationPeer"/>.
        /// </param>
        public HyperlinkButtonAutomationPeer(HyperlinkButton owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Gets an object that supports the requested pattern, based on the patterns supported
        /// by this <see cref="HyperlinkButtonAutomationPeer"/>.
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
        /// Gets the control type for the element that is associated with this <see cref="HyperlinkButtonAutomationPeer"/>.
        /// This method is called by <see cref="AutomationPeer.GetAutomationControlType"/>.
        /// </summary>
        /// <returns>
        /// A value of the enumeration.
        /// </returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
            => AutomationControlType.Hyperlink;

        /// <summary>
        /// Gets the name of the class that is associated with this <see cref="HyperlinkButtonAutomationPeer"/>.
        /// This method is called by <see cref="AutomationPeer.GetClassName"/>.
        /// </summary>
        /// <returns>
        /// The class name.
        /// </returns>
        protected override string GetClassNameCore() => "Hyperlink";

        /// <summary>
        /// Gets a value that indicates whether the element associated with this automation
        /// peer is an element that contains data that is presented to the user.
        /// </summary>
        /// <returns>
        /// true if the element is interactive; otherwise, false.
        /// </returns>
        protected override bool IsControlElementCore() => true;

        //Invoke Pattern implementation
        void IInvokeProvider.Invoke()
        {
            if (!IsEnabled())
            {
                throw new ElementNotEnabledException();
            }

            ((HyperlinkButton)Owner).AutomationButtonBaseClick();
        }
    }
}
