
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
    /// Exposes <see cref="TextBlock"/> types to UI automation.
    /// </summary>
    public class TextBlockAutomationPeer : FrameworkElementAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextBlockAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="TextBlock"/> to associate with the <see cref="TextBlockAutomationPeer"/>.
        /// </param>
        public TextBlockAutomationPeer(TextBlock owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Gets the control type for the <see cref="TextBlock"/> that is associated 
        /// with this <see cref="TextBlockAutomationPeer"/>. This method is called 
        /// by <see cref="AutomationPeer.GetAutomationControlType"/>.
        /// </summary>
        /// <returns>
        /// A value of the enumeration.
        /// </returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
            => AutomationControlType.Text;

        /// <summary>
        /// Gets the name of the class that is associated with this <see cref="TextBlockAutomationPeer"/>.
        /// This method is called by <see cref="AutomationPeer.GetClassName"/>.
        /// </summary>
        /// <returns>
        /// The class name.
        /// </returns>
        protected override string GetClassNameCore() => "TextBlock";

        /// <summary>
        /// Gets the UI Automation Name for the <see cref="TextBlock"/> that is associated with 
        /// this <see cref="TextBlockAutomationPeer"/>.
        /// Called by <see cref="AutomationPeer.GetName"/>.
        /// </summary>
        /// <returns>
        /// The UI Automation Name of the object that is associated with this automation peer.
        /// </returns>
        protected override string GetNameCore()
        {
            string name = base.GetNameCore();
            if (string.IsNullOrEmpty(name))
            {
                name = ((TextBlock)Owner).GetPlainText();
            }

            return name;
        }

        /// <summary>
        /// Gets a value that indicates whether the <see cref="TextBlock"/> associated
        /// with this peer is understood by the user as interactive or as contributing to
        /// the logical structure of the UI.
        /// </summary>
        /// <returns>
        /// false if the element is part of an applied template; otherwise, true.
        /// </returns>
        protected override bool IsControlElementCore()
        {
            if (((TextBlock)Owner).TemplatedParent == null)
            {
                return base.IsControlElementCore();
            }

            return false;
        }
    }
}
