
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

// Derived from WPF (dotnet/wpf, MIT license).

using System.Windows.Controls;

namespace System.Windows.Automation.Peers
{
    /// <summary>
    /// Exposes <see cref="GroupBox"/> types to UI automation.
    /// </summary>
    public class GroupBoxAutomationPeer : FrameworkElementAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupBoxAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="GroupBox"/> to associate with this <see cref="GroupBoxAutomationPeer"/>.
        /// </param>
        public GroupBoxAutomationPeer(GroupBox owner)
            : base(owner)
        {
        }

        protected override string GetClassNameCore() => "GroupBox";

        protected override AutomationControlType GetAutomationControlTypeCore()
            => AutomationControlType.Group;
    }
}
