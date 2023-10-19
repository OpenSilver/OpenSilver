
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
    /// Exposes <see cref="Image"/> types to UI automation.
    /// </summary>
    public class ImageAutomationPeer : FrameworkElementAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="Image"/> to associate with this <see cref="ImageAutomationPeer"/>.
        /// </param>
        public ImageAutomationPeer(Image owner)
            : base(owner)
        {
        }

        protected override string GetClassNameCore() => "Image";

        protected override AutomationControlType GetAutomationControlTypeCore()
            => AutomationControlType.Image;
    }
}
