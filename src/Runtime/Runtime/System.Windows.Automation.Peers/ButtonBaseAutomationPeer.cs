
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

using System.Windows.Controls.Primitives;

namespace System.Windows.Automation.Peers
{
    /// <summary>
    /// Represents a base class for exposing classes derived from <see cref="ButtonBase"/> to UI automation.
    /// </summary>
    public abstract class ButtonBaseAutomationPeer : FrameworkElementAutomationPeer
    {
        /// <summary>
        /// Provides initialization for base class values when called by the constructor
        /// of a derived class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="ButtonBase"/> to associate with this peer.
        /// </param>
        protected ButtonBaseAutomationPeer(ButtonBase owner)
            : base(owner)
        {
        }
        
        /// <summary>
        /// Gets the UI Automation Name for the element associated with this <see cref="ButtonBaseAutomationPeer"/>.
        /// Called by <see cref="AutomationPeer.GetName"/>.
        /// </summary>
        /// <returns>
        /// The UI Automation Name for the associated element.
        /// </returns>
        protected override string GetNameCore()
        {
            string name = base.GetNameCore();
            if (string.IsNullOrEmpty(name))
            {
                name = ((ButtonBase)Owner).GetPlainText();
            }

            return name;
        }
    }
}
