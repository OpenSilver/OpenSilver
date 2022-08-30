
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
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Xaml.Controls.Primitives;
#endif

#if MIGRATION
namespace System.Windows.Automation.Peers
#else
namespace Windows.UI.Xaml.Automation.Peers
#endif
{
    internal class PopupRootAutomationPeer : FrameworkElementAutomationPeer
    {
        public PopupRootAutomationPeer(Popup owner)
            : base(owner)
        {
        }

        protected override string GetClassNameCore() => "Popup";

        protected override AutomationControlType GetAutomationControlTypeCore()
            => AutomationControlType.Window;
    }
}