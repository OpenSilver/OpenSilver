
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

using System.Windows.Automation.Peers;

namespace System.Windows
{
    public partial class UIElement
    {
        private AutomationPeer _peer;

        /// <summary>
        /// When implemented in a derived class, returns class-specific <see cref="AutomationPeer"/>
        /// implementations for the Silverlight automation infrastructure.
        /// </summary>
        /// <returns>
        /// The class-specific <see cref="AutomationPeer"/> subclass to
        /// return.
        /// </returns>
        protected virtual AutomationPeer OnCreateAutomationPeer() => null;

        internal bool HasAutomationPeer
        {
            get => ReadFlag(CoreFlags.HasAutomationPeer);
            set => WriteFlag(CoreFlags.HasAutomationPeer, value);
        }

        /// <summary>
        /// Called by the Automation infrastructure or Control author
        /// to make sure the AutomationPeer is created. The element may
        /// create AP or return null, depending on OnCreateAutomationPeer override.
        /// </summary>
        internal AutomationPeer CreateAutomationPeer()
        {
            AutomationPeer ap = _peer;

            if (ap is null)
            {
                ap = OnCreateAutomationPeer();

                if (ap != null)
                {
                    _peer = ap;
                    HasAutomationPeer = true;
                }
            }

            return ap;
        }

        /// <summary>
        /// Returns AutomationPeer if one exists.
        /// The AutomationPeer may not exist if not yet created by Automation infrastructure
        /// or if this element is not supposed to have one.
        /// </summary>
        internal AutomationPeer GetAutomationPeer() => _peer;
    }
}
