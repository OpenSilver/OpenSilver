
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
    /// Exposes <see cref="ComboBox"/> types to UI automation.
    /// </summary>
    public class ComboBoxAutomationPeer : SelectorAutomationPeer, IValueProvider, IExpandCollapseProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComboBoxAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="ComboBox"/> instance to associate with this <see cref="ComboBoxAutomationPeer"/>.
        /// </param>
        public ComboBoxAutomationPeer(ComboBox owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Gets an object that supports the requested pattern, based on the patterns supported
        /// by this <see cref="ComboBoxAutomationPeer"/>.
        /// </summary>
        /// <param name="pattern">
        /// One of the enumeration values.
        /// </param>
        /// <returns>
        /// The object that implements the pattern interface, or null if the specified pattern
        /// interface is not implemented by this peer.
        /// </returns>
        public override object GetPattern(PatternInterface pattern)
        {
            object iface = null;

            if (pattern == PatternInterface.Value)
            {
                if (((ComboBox)Owner).IsEditable)
                {
                    iface = this;
                }
            }
            else if (pattern == PatternInterface.ExpandCollapse)
            {
                iface = this;
            }
            else
            {
                iface = base.GetPattern(pattern);
            }

            return iface;
        }

        /// <summary>
        /// Creates an automation peer for an individual item in the <see cref="ComboBox"/>
        /// (or other element) associated with this <see cref="ComboBoxAutomationPeer"/>.
        /// </summary>
        /// <param name="item">
        /// The item to create the automation peer for.
        /// </param>
        /// <returns>
        /// The new <see cref="ListBoxItemAutomationPeer"/> for the specified
        /// item in the children collection.
        /// </returns>
        protected override ItemAutomationPeer CreateItemAutomationPeer(object item)
            => new ListBoxItemAutomationPeer(item, this);

        /// <summary>
        /// Gets the control type for the element associated with this <see cref="ComboBoxAutomationPeer"/>.
        /// This method is called by <see cref="AutomationPeer.GetAutomationControlType"/>.
        /// </summary>
        /// <returns>
        /// The control type as a value of the enumeration.
        /// </returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
            => AutomationControlType.ComboBox;

        /// <summary>
        /// Gets the name of the class that is associated with this <see cref="ComboBoxAutomationPeer"/>.
        /// This method is called by <see cref="AutomationPeer.GetClassName"/>.
        /// </summary>
        /// <returns>
        /// The class name.
        /// </returns>
        protected override string GetClassNameCore() => "ComboBox";

        /// <summary>
        /// Request to set the value that this UI element is representing
        /// </summary>
        /// <param name="val">Value to set the UI to, as an object</param>
        /// <returns>true if the UI element was successfully set to the specified value</returns>
        void IValueProvider.SetValue(string val)
            => throw new InvalidOperationException("Cannot perform operation.");

        ///<summary>Value of a value control, as a a string.</summary>
        string IValueProvider.Value
            => ContentControl.ContentObjectToString(((ComboBox)Owner).SelectionBoxItem);

        ///<summary>Indicates that the value can only be read, not modified.
        ///returns True if the control is read-only</summary>
        bool IValueProvider.IsReadOnly
        {
            get
            {
                ComboBox owner = (ComboBox)Owner;
                return !owner.IsEnabled || !owner.IsEditable;
            }
        }

        /// <summary>
        /// Blocking method that returns after the element has been expanded.
        /// </summary>
        /// <returns>true if the node was successfully expanded</returns>
        void IExpandCollapseProvider.Expand()
        {
            if (!IsEnabled())
            {
                throw new ElementNotEnabledException();
            }

            ((ComboBox)Owner).SetCurrentValue(ComboBox.IsDropDownOpenProperty, true);
        }

        /// <summary>
        /// Blocking method that returns after the element has been collapsed.
        /// </summary>
        /// <returns>true if the node was successfully collapsed</returns>
        void IExpandCollapseProvider.Collapse()
        {
            if (!IsEnabled())
            {
                throw new ElementNotEnabledException();
            }

            ((ComboBox)Owner).SetCurrentValue(ComboBox.IsDropDownOpenProperty, false);
        }

        ///<summary>indicates an element's current Collapsed or Expanded state</summary>
        ExpandCollapseState IExpandCollapseProvider.ExpandCollapseState
            => ((ComboBox)Owner).IsDropDownOpen ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed;
    }
}
