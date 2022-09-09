
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

#if MIGRATION
using System.Windows.Automation.Provider;
using System.Windows.Controls;
#else
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls;
#endif

#if MIGRATION
namespace System.Windows.Automation.Peers
#else
namespace Windows.UI.Xaml.Automation.Peers
#endif
{
    /// <summary>
    /// Exposes <see cref="TextBox"/> types to UI automation.
    /// </summary>
    public class TextBoxAutomationPeer : FrameworkElementAutomationPeer, IValueProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextBoxAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="TextBox"/> that is associated with this <see cref="TextBoxAutomationPeer"/>.
        /// </param>
        public TextBoxAutomationPeer(TextBox owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Gets an object that supports the requested pattern, based on the patterns supported
        /// by this <see cref="TextBoxAutomationPeer"/>.
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
            if (patternInterface == PatternInterface.Value)
            {
                return this;
            }

            return base.GetPattern(patternInterface);
        }

        /// <summary>
        /// Gets the control type for the control that is associated with this <see cref="TextBoxAutomationPeer"/>.
        /// This method is called by <see cref="AutomationPeer.GetAutomationControlType"/>.
        /// </summary>
        /// <returns>
        /// A value of the enumeration.
        /// </returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
            => AutomationControlType.Edit;

        /// <summary>
        /// Gets the name of the class that is associated with this <see cref="TextBoxAutomationPeer"/>.
        /// This method is called by <see cref="AutomationPeer.GetClassName"/>.
        /// </summary>
        /// <returns>
        /// The class name.
        /// </returns>
        protected override string GetClassNameCore() => "TextBox";

        /// <summary>
        /// Gets the UI Automation Name value from the element that is associated with this
        /// <see cref="TextBoxAutomationPeer"/>. Called by <see cref="AutomationPeer.GetName"/>.
        /// </summary>
        /// <returns>
        /// The UI Automation Name of the element that is associated with this automation peer.
        /// </returns>
        protected override string GetNameCore()
        {
            string name = base.GetNameCore();
            if (string.IsNullOrEmpty(name))
            {
                name = ((TextBox)Owner).Text;
            }

            return name;
        }

        bool IValueProvider.IsReadOnly => ((TextBox)Owner).IsReadOnly;

        string IValueProvider.Value => ((TextBox)Owner).Text;

        void IValueProvider.SetValue(string value)
        {
            if (!IsEnabled())
            {
                throw new ElementNotEnabledException();
            }

            TextBox owner = (TextBox)Owner;

            if (owner.IsReadOnly)
            {
                throw new ElementNotEnabledException();
            }

            owner.Text = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
