
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
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
#endif

#if MIGRATION
namespace System.Windows.Automation.Peers
#else
namespace Windows.UI.Xaml.Automation.Peers
#endif
{
    /// <summary>
    /// Exposes <see cref="RadioButton"/> types to UI automation.
    /// </summary>
    public class RadioButtonAutomationPeer : ToggleButtonAutomationPeer, ISelectionItemProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RadioButtonAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="RadioButton" /> to associate with the <see cref="RadioButtonAutomationPeer"/>.
        /// </param>
        public RadioButtonAutomationPeer(RadioButton owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Gets an object that supports the requested pattern, based on the patterns supported
        /// by this <see cref="RadioButtonAutomationPeer"/>.
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
            if (patternInterface == PatternInterface.SelectionItem)
            {
                return this;
            }

            return null;
        }

        /// <summary>
        /// This method is called by <see cref="AutomationPeer.GetAutomationControlType"/>.
        /// </summary>
        /// <returns>
        /// A value of the enumeration.
        /// </returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
            => AutomationControlType.RadioButton;

        /// <summary>
        /// Gets the name of the class that is associated with this <see cref="RadioButtonAutomationPeer"/>.
        /// This method is called by <see cref="AutomationPeer.GetClassName"/>.
        /// </summary>
        /// <returns>
        /// The class name.
        /// </returns>
        protected override string GetClassNameCore() => nameof(RadioButton);

        /// <summary>
        /// Sets the current element as the selection
        /// This clears the selection from other elements in the container
        /// </summary>
        void ISelectionItemProvider.Select()
        {
            if (!IsEnabled())
            {
                throw new ElementNotEnabledException();
            }

            ((RadioButton)Owner).SetCurrentValue(ToggleButton.IsCheckedProperty, true);
        }

        /// <summary>
        /// Adds current element to selection
        /// </summary>
        void ISelectionItemProvider.AddToSelection()
        {
            if (((RadioButton)Owner).IsChecked != true)
            {
                throw new InvalidOperationException("Cannot perform operation.");
            }
        }

        /// <summary>
        /// Removes current element from selection
        /// </summary>
        void ISelectionItemProvider.RemoveFromSelection()
        {
            if (((RadioButton)Owner).IsChecked == true)
            {
                throw new InvalidOperationException("Cannot perform operation.");
            }
        }

        /// <summary>
        /// Check whether an element is selected
        /// </summary>
        /// <value>returns true if the element is selected</value>
        bool ISelectionItemProvider.IsSelected => ((RadioButton)Owner).IsChecked == true;

        /// <summary>
        /// The logical element that supports the SelectionPattern for this Item
        /// </summary>
        /// <value>returns an IRawElementProviderSimple</value>
        IRawElementProviderSimple ISelectionItemProvider.SelectionContainer => null;
    }
}
