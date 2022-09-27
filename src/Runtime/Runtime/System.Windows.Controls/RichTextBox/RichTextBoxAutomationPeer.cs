
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
using System.Collections.Generic;

#if MIGRATION
using System.Windows.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

#if MIGRATION
namespace System.Windows.Automation.Peers
#else
namespace Windows.UI.Xaml.Automation.Peers
#endif
{
    /// <summary>
    /// Exposes <see cref="RichTextBox"/> types to UI automation.
    /// </summary>
    public class RichTextBoxAutomationPeer : FrameworkElementAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RichTextBoxAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="RichTextBox"/> owner of the automation peer.
        /// </param>
        public RichTextBoxAutomationPeer(RichTextBox owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Gets an object that supports the requested pattern, based on the patterns supported
        /// by this <see cref="RichTextBoxAutomationPeer"/>.
        /// </summary>
        /// <param name="patternInterface">
        /// One of the enumeration values.
        /// </param>
        /// <returns>
        /// The object that implements the pattern interface; or null, if the specified pattern
        /// interface is not implemented by this peer.
        /// </returns>
        [OpenSilver.NotImplemented]
        public override object GetPattern(PatternInterface patternInterface)
        {
            return null;
        }

        /// <summary>
        /// Gets the control type for the control that is associated with this <see cref="RichTextBoxAutomationPeer"/>.
        /// This method is called by <see cref="AutomationPeer.GetAutomationControlType"/>.
        /// </summary>
        /// <returns>
        /// A value of the enumeration.
        /// </returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
            => AutomationControlType.Document;

        /// <summary>
        /// Returns automation peers for the collection of child elements of the <see cref="RichTextBox"/>
        /// that is associated with this <see cref="FrameworkElementAutomationPeer"/>.
        /// This method is called by <see cref="AutomationPeer.GetChildren"/>.
        /// </summary>
        /// <returns>
        /// A list of <see cref="AutomationPeer"/> objects for child elements.
        /// </returns>
        [OpenSilver.NotImplemented]
        protected override List<AutomationPeer> GetChildrenCore()
        {
            return null;
        }

        /// <summary>
        /// Gets the name of the class that is associated with this <see cref="RichTextBoxAutomationPeer"/>.
        /// This method is called by <see cref="AutomationPeer.GetClassName"/>.
        /// </summary>
        /// <returns>
        /// The class name.
        /// </returns>
        protected override string GetClassNameCore() => "RichTextBox";
    }
}
