
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
    /// Exposes <see cref="MediaElement"/> types to UI automation.
    /// </summary>
    public class MediaElementAutomationPeer : FrameworkElementAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaElementAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="MediaElement"/> to associate with the <see cref="MediaElementAutomationPeer"/>.
        /// </param>
        public MediaElementAutomationPeer(MediaElement owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Gets the control type for the element that is associated with this <see cref="MediaElementAutomationPeer"/>.
        /// This method is called by <see cref="AutomationPeer.GetAutomationControlType"/>.
        /// </summary>
        /// <returns>
        /// A value of the enumeration
        /// </returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
            => AutomationControlType.Custom;

        /// <summary>
        /// Gets the name of the class that is associated with this <see cref="MediaElementAutomationPeer"/>.
        /// This method is called by <see cref="AutomationPeer.GetClassName"/>.
        /// </summary>
        /// <returns>
        /// The class name.
        /// </returns>
        protected override string GetClassNameCore() => "MediaElement";
    }
}
