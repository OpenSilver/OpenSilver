
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
    /// Exposes <see cref="PasswordBox"/> types to UI automation.
    /// </summary>
    public class PasswordBoxAutomationPeer : FrameworkElementAutomationPeer, IValueProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordBoxAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="PasswordBox"/> that is associated with this <see cref="PasswordBoxAutomationPeer"/>.
        /// </param>
        public PasswordBoxAutomationPeer(PasswordBox owner)
            : base(owner)
        {
        }
        
        /// <summary>
        /// Gets an object that supports the requested pattern, based on the patterns supported
        /// by this <see cref="PasswordBoxAutomationPeer"/>.
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
        /// Gets the control type for the element that is associated with this <see cref="PasswordBoxAutomationPeer"/>.
        /// This method is called by <see cref="AutomationPeer.GetAutomationControlType"/>.
        /// </summary>
        /// <returns>
        /// A value of the enumeration.
        /// </returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
            => AutomationControlType.Edit;

        /// <summary>
        /// Gets the name of the class that is associated with this <see cref="PasswordBoxAutomationPeer"/>.
        /// This method is called by <see cref="AutomationPeer.GetClassName"/>.
        /// </summary>
        /// <returns>
        /// The class name.
        /// </returns>
        protected override string GetClassNameCore() => "PasswordBox";

        /// <summary>
        /// Gets a value that indicates whether the <see cref="PasswordBox"/> associated
        /// with this peer is understood by the user as interactive or as contributing to
        /// the logical structure of the UI.
        /// </summary>
        /// <returns>
        /// false if the element is part of an applied template; otherwise, true.
        /// </returns>
        protected override bool IsPasswordCore() => true;

        bool IValueProvider.IsReadOnly => false;

        string IValueProvider.Value => throw new InvalidOperationException();

        void IValueProvider.SetValue(string value)
        {
            if (!IsEnabled())
            {
                throw new ElementNotEnabledException();
            }

            ((PasswordBox)Owner).Password = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
