
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

using System.Windows.Controls;

namespace System.Windows.Automation.Peers
{
    /// <summary>
    /// Exposes <see cref="CheckBox"/> types to UI automation.
    /// </summary>
    public class CheckBoxAutomationPeer : ToggleButtonAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckBoxAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="CheckBox"/> to associate with this <see cref="CheckBoxAutomationPeer"/>.
        /// </param>
        public CheckBoxAutomationPeer(CheckBox owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Gets the <see cref="AutomationControlType"/> for the class associated with this 
        /// <see cref="CheckBoxAutomationPeer"/>.
        /// Called by <see cref="AutomationPeer.GetAutomationControlType"/>.
        /// </summary>
        /// <returns>
        /// A value of the enumeration.
        /// </returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
            => AutomationControlType.CheckBox;

        /// <summary>
        /// Gets the name of the class that is associated with this <see cref="CheckBoxAutomationPeer"/>.
        /// Called by <see cref="AutomationPeer.GetClassName"/>.
        /// </summary>
        /// <returns>
        /// The class name.
        /// </returns>
        protected override string GetClassNameCore() => "CheckBox";
    }
}
