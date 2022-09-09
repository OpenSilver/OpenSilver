
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
using System.Windows.Automation.Peers;
#else
using Windows.UI.Xaml.Automation.Peers;
#endif

#if MIGRATION
namespace System.Windows.Automation
#else
namespace Windows.UI.Xaml.Automation
#endif
{
    /// <summary>
    /// Provides support for getting or setting the value of instance-level values of automation properties.
    /// These property values are set as attached properties (typically in XAML) and supplement or override 
    /// automation property values from a control's <see cref="AutomationPeer" />.
    /// </summary>
    public static class AutomationProperties
    {
        /// <summary>
        /// Identifies the AutomationProperties.AcceleratorKey attached property.
        /// </summary>
        public static readonly DependencyProperty AcceleratorKeyProperty =
            DependencyProperty.RegisterAttached(
                "AcceleratorKey",
                typeof(string),
                typeof(AutomationProperties),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets the value of the <see cref="AcceleratorKeyProperty" /> attached property for 
        /// the specified <see cref="DependencyObject" />.
        /// </summary>
        /// <param name="element">
        /// The <see cref="DependencyObject" /> to check.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="element" /> is invalid value.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> is null.
        /// </exception>
        public static string GetAcceleratorKey(DependencyObject element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (!(element is UIElement uie))
            {
                throw new ArgumentException(nameof(element));
            }

            AutomationPeer peer = uie.CreateAutomationPeer();

            if (peer is null)
            {
                return (string)uie.GetValue(AcceleratorKeyProperty);
            }

            return peer.GetAcceleratorKey();
        }

        /// <summary>
        /// Sets the value of the <see cref="AcceleratorKeyProperty" /> attached property for the 
        /// specified <see cref="DependencyObject" />.
        /// </summary>
        /// <param name="element">
        /// The <see cref="DependencyObject" /> for which to set the property.
        /// </param>
        /// <param name="value">
        /// The accelerator key value to set.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="element" /> is invalid value.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> is null.
        /// </exception>
        public static void SetAcceleratorKey(DependencyObject element, string value)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (!(element is UIElement))
            {
                throw new ArgumentException(nameof(element));
            }

            element.SetValue(AcceleratorKeyProperty, value);
        }

        /// <summary>
        /// Identifies the AutomationProperties.AccessKey attached property.
        /// </summary>
        public static readonly DependencyProperty AccessKeyProperty =
            DependencyProperty.RegisterAttached(
                "AccessKey",
                typeof(string),
                typeof(AutomationProperties),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets the value of the <see cref="AccessKeyProperty" /> attached property for the specified 
        /// <see cref="DependencyObject" />.</summary>
        /// <returns>
        /// The access key, as a string.
        /// </returns>
        /// <param name="element">
        /// The <see cref="DependencyObject" /> to check.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="element" /> is invalid value.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> is null.
        /// </exception>
        public static string GetAccessKey(DependencyObject element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (!(element is UIElement uie))
            {
                throw new ArgumentException(nameof(element));
            }
            
            AutomationPeer peer = uie.CreateAutomationPeer();

            if (peer is null)
            {
                return (string)element.GetValue(AccessKeyProperty);
            }

            return peer.GetAccessKey();
        }

        /// <summary>
        /// Sets the value of the <see cref="AccessKeyProperty" /> attached property for the 
        /// specified <see cref="DependencyObject" />.
        /// </summary>
        /// <param name="element">
        /// The <see cref="DependencyObject" /> for which to set the property.
        /// </param>
        /// <param name="value">
        /// The access key value to set.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="element" /> is invalid value.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> is null.
        /// </exception>
        public static void SetAccessKey(DependencyObject element, string value)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (!(element is UIElement))
            {
                throw new ArgumentException(nameof(element));
            }

            element.SetValue(AccessKeyProperty, value);
        }

        /// <summary>
        /// Identifies the AutomationProperties.AutomationId attached property.
        /// </summary>
        public static readonly DependencyProperty AutomationIdProperty =
            DependencyProperty.RegisterAttached(
                "AutomationId",
                typeof(string),
                typeof(AutomationProperties),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets the value of the <see cref="AutomationIdProperty" /> attached property for the 
        /// specified <see cref="DependencyObject" />.
        /// </summary>
        /// <returns>
        /// The UI Automation identifier for the specified element.
        /// </returns>
        /// <param name="element">
        /// The <see cref="DependencyObject" /> to check.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="element" /> is invalid value.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> is null.
        /// </exception>
        public static string GetAutomationId(DependencyObject element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (!(element is UIElement uie))
            {
                throw new ArgumentException(nameof(element));
            }

            AutomationPeer peer = uie.CreateAutomationPeer();

            if (peer is null)
            {
                return (string)element.GetValue(AutomationIdProperty);
            }
            
            return peer.GetAutomationId();
        }

        /// <summary>
        /// Sets the value of the <see cref="AutomationIdProperty" /> attached property for the 
        /// specified <see cref="DependencyObject" />.
        /// </summary>
        /// <param name="element">
        /// The <see cref="DependencyObject" /> for which to set the property.
        /// </param>
        /// <param name="value">
        /// The UI automation identifier value to set.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="element" /> is invalid value.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> is null.
        /// </exception>
        public static void SetAutomationId(DependencyObject element, string value)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (!(element is UIElement))
            {
                throw new ArgumentException(nameof(element));
            }

            element.SetValue(AutomationIdProperty, value);
        }

        /// <summary>
        /// Identifies the AutomationProperties.HelpText attached property.
        /// </summary>
        public static readonly DependencyProperty HelpTextProperty =
            DependencyProperty.RegisterAttached(
                "HelpText",
                typeof(string),
                typeof(AutomationProperties),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets the value of the <see cref="HelpTextProperty" /> attached property for the 
        /// specified <see cref="DependencyObject" />.
        /// </summary>
        /// <returns>
        /// The help text for the specified element.
        /// </returns>
        /// <param name="element">
        /// The <see cref="DependencyObject" /> to check.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="element" /> is invalid value.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> is null.
        /// </exception>
        public static string GetHelpText(DependencyObject element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (!(element is UIElement uie))
            {
                throw new ArgumentException(nameof(element));
            }

            AutomationPeer peer = uie.CreateAutomationPeer();

            if (peer is null)
            {
                return (string)element.GetValue(HelpTextProperty);
            }

            return peer.GetHelpText();
        }

        /// <summary>
        /// Sets the value of the <see cref="HelpTextProperty" /> attached property for the 
        /// specified <see cref="DependencyObject" />.
        /// </summary>
        /// <param name="element">
        /// The <see cref="DependencyObject" /> for which to set the property.
        /// </param>
        /// <param name="value">
        /// The help text.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="element" /> is invalid value.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> is null.
        /// </exception>
        public static void SetHelpText(DependencyObject element, string value)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (!(element is UIElement))
            {
                throw new ArgumentException(nameof(element));
            }
            
            element.SetValue(HelpTextProperty, value);
        }

        /// <summary>
        /// Identifies the AutomationProperties.IsRequiredForForm attached property.
        /// </summary>
        public static readonly DependencyProperty IsRequiredForFormProperty =
            DependencyProperty.RegisterAttached(
                "IsRequiredForForm",
                typeof(bool),
                typeof(AutomationProperties),
                new PropertyMetadata(false));

        /// <summary>
        /// Gets the value of the <see cref="IsRequiredForFormProperty" /> attached property for the 
        /// specified <see cref="DependencyObject" />.
        /// </summary>
        /// <returns>
        /// true if the specified element is required for completion of a form; otherwise, false.
        /// </returns>
        /// <param name="element">
        /// The <see cref="DependencyObject" /> to check.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="element" /> is invalid value.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> is null.
        /// </exception>
        public static bool GetIsRequiredForForm(DependencyObject element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (!(element is UIElement uie))
            {
                throw new ArgumentException(nameof(element));
            }

            AutomationPeer peer = uie.CreateAutomationPeer();
            
            if (peer is null)
            {
                return (bool)element.GetValue(IsRequiredForFormProperty);
            }

            return peer.IsRequiredForForm();
        }

        /// <summary>
        /// Sets the value of the <see cref="IsRequiredForFormProperty" /> attached property for the 
        /// specified <see cref="DependencyObject" />.
        /// </summary>
        /// <param name="element">
        /// The <see cref="DependencyObject" /> for which to set the property.
        /// </param>
        /// <param name="value">
        /// true to specify that the element is required to be filled out on a form; otherwise, false.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="element" /> is invalid value.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> is null.
        /// </exception>
        public static void SetIsRequiredForForm(DependencyObject element, bool value)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (!(element is UIElement))
            {
                throw new ArgumentException(nameof(element));
            }
            
            element.SetValue(IsRequiredForFormProperty, value);
        }

        /// <summary>
        /// Identifies the AutomationProperties.ItemStatus attached property.
        /// </summary>
        public static readonly DependencyProperty ItemStatusProperty =
            DependencyProperty.RegisterAttached(
                "ItemStatus",
                typeof(string),
                typeof(AutomationProperties),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets the value of the <see cref="ItemStatusProperty" /> attached property for the 
        /// specified <see cref="DependencyObject" />.
        /// </summary>
        /// <returns>
        /// The item status of the element.
        /// </returns>
        /// <param name="element">
        /// The <see cref="DependencyObject" /> to check.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="element" /> is invalid value.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> is null.
        /// </exception>
        public static string GetItemStatus(DependencyObject element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (!(element is UIElement uie))
            {
                throw new ArgumentException(nameof(element));
            }

            AutomationPeer peer = uie.CreateAutomationPeer();
            
            if (peer is null)
            {
                return (string)element.GetValue(AutomationProperties.ItemStatusProperty);
            }

            return peer.GetItemStatus();
        }

        /// <summary>
        /// Sets the value of the <see cref="ItemStatusProperty" /> attached property for the 
        /// specified <see cref="DependencyObject" />.
        /// </summary>
        /// <param name="element">
        /// The <see cref="DependencyObject" /> for which to set the property.
        /// </param>
        /// <param name="value">
        /// The item status.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="element" /> is invalid value.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> is null.
        /// </exception>
        public static void SetItemStatus(DependencyObject element, string value)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (!(element is UIElement))
            {
                throw new ArgumentException(nameof(element));
            }
            
            element.SetValue(ItemStatusProperty, value);
        }

        /// <summary>
        /// Identifies the AutomationProperties.ItemType attached property.
        /// </summary>
        public static readonly DependencyProperty ItemTypeProperty =
            DependencyProperty.RegisterAttached(
                "ItemType",
                typeof(string),
                typeof(AutomationProperties),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets the value of the <see cref="ItemTypeProperty" /> attached property for the 
        /// specified <see cref="DependencyObject" />.
        /// </summary>
        /// <returns>
        /// The item type of the element.
        /// </returns>
        /// <param name="element">
        /// The <see cref="DependencyObject" /> to check.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="element" /> is invalid value.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> is null.
        /// </exception>
        public static string GetItemType(DependencyObject element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (!(element is UIElement uie))
            {
                throw new ArgumentException(nameof(element));
            }

            AutomationPeer peer = uie.CreateAutomationPeer();
            
            if (peer is null)
            {
                return (string)element.GetValue(ItemTypeProperty);
            }
            
            return peer.GetItemType();
        }

        /// <summary>
        /// Sets the value of the <see cref="ItemTypeProperty" /> attached property for the 
        /// specified <see cref="DependencyObject" />.
        /// </summary>
        /// <param name="element">
        /// The <see cref="DependencyObject" /> for which to set the property.
        /// </param>
        /// <param name="value">
        /// The item type.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="element" /> is invalid value.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> is null.
        /// </exception>
        public static void SetItemType(DependencyObject element, string value)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (!(element is UIElement))
            {
                throw new ArgumentException(nameof(element));
            }
            
            element.SetValue(ItemTypeProperty, value);
        }

        /// <summary>
        /// Identifies the AutomationProperties.LabeledBy attached property.
        /// </summary>
        public static readonly DependencyProperty LabeledByProperty =
            DependencyProperty.RegisterAttached(
                "LabeledBy",
                typeof(UIElement),
                typeof(AutomationProperties),
                new PropertyMetadata((object)null));

        /// <summary>
        /// Gets the value of the <see cref="LabeledByProperty" /> attached property for the 
        /// specified <see cref="DependencyObject" />.
        /// </summary>
        /// <returns>
        /// The element that is targeted by the label.
        /// </returns>
        /// <param name="element">
        /// The <see cref="DependencyObject" /> to check.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="element" /> is invalid value.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> is null.
        /// </exception>
        public static UIElement GetLabeledBy(DependencyObject element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (!(element is UIElement uie))
            {
                throw new ArgumentException(nameof(element));
            }

            AutomationPeer peer = uie.CreateAutomationPeer();
            if (peer is null)
            {
                return (UIElement)element.GetValue(LabeledByProperty);
            }

            if (peer.GetLabeledBy() is FrameworkElementAutomationPeer labeledBy)
            {
                return labeledBy.Owner;
            }

            return null;
        }

        /// <summary>
        /// Sets the value of the <see cref="LabeledByProperty" /> attached property for the 
        /// specified <see cref="DependencyObject" />.
        /// </summary>
        /// <param name="element">
        /// The <see cref="DependencyObject" /> for which to set the property.
        /// </param>
        /// <param name="value">
        /// The UI element that represents the label for <paramref name="element" />.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="element" /> is invalid value.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> is null.
        /// </exception>
        public static void SetLabeledBy(DependencyObject element, UIElement value)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (!(element is UIElement))
            {
                throw new ArgumentException(nameof(element));
            }

            element.SetValue(LabeledByProperty, value);
        }

        /// <summary>
        /// Identifies the AutomationProperties.Name attached property.
        /// </summary>
        public static readonly DependencyProperty NameProperty =
             DependencyProperty.RegisterAttached(
                "Name",
                typeof(string),
                typeof(AutomationProperties),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets the value of the <see cref="NameProperty" /> attached property for the 
        /// specified <see cref="DependencyObject" />.
        /// </summary>
        /// <returns>
        /// The name of the specified element.
        /// </returns>
        /// <param name="element">
        /// The <see cref="DependencyObject" /> to check.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="element" /> is invalid value.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> is null.
        /// </exception>
        public static string GetName(DependencyObject element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (!(element is UIElement uie))
            {
                throw new ArgumentException(nameof(element));
            }
            
            AutomationPeer peer = uie.CreateAutomationPeer();

            if (peer is null)
            {
                return (string)element.GetValue(NameProperty);
            }

            return peer.GetName();
        }

        /// <summary>
        /// Sets the value of the <see cref="NameProperty" /> attached property for the 
        /// specified <see cref="DependencyObject" />.
        /// </summary>
        /// <param name="element">
        /// The <see cref="DependencyObject" /> for which to set the property.
        /// </param>
        /// <param name="value">
        /// The object name.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="element" /> is invalid value.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> is null.
        /// </exception>
        public static void SetName(DependencyObject element, string value)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (!(element is UIElement))
            {
                throw new ArgumentException(nameof(element));
            }

            element.SetValue(NameProperty, value);
        }
    }
}
