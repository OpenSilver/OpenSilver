using System;
using System.Collections.Generic;
using System.Text;

#if MIGRATION
namespace System.Windows.Automation
#else
namespace Windows.UI.Xaml.Automation
#endif
{
    [OpenSilver.NotImplemented]
	public static partial class AutomationProperties
	{
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty AcceleratorKeyProperty = DependencyProperty.RegisterAttached("AcceleratorKey", typeof(string), typeof(AutomationProperties), null);
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty AccessKeyProperty = DependencyProperty.RegisterAttached("AccessKey", typeof(string), typeof(AutomationProperties), null);
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty AutomationIdProperty = DependencyProperty.RegisterAttached("AutomationId", typeof(string), typeof(AutomationProperties), null);
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty HelpTextProperty = DependencyProperty.RegisterAttached("HelpText", typeof(string), typeof(AutomationProperties), null);
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty IsRequiredForFormProperty = DependencyProperty.RegisterAttached("IsRequiredForForm", typeof(bool), typeof(AutomationProperties), null);
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty ItemStatusProperty = DependencyProperty.RegisterAttached("ItemStatus", typeof(string), typeof(AutomationProperties), null);
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty ItemTypeProperty = DependencyProperty.RegisterAttached("ItemType", typeof(string), typeof(AutomationProperties), null);
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty LabeledByProperty = DependencyProperty.RegisterAttached("LabeledBy", typeof(UIElement), typeof(AutomationProperties), null);
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty NameProperty = DependencyProperty.RegisterAttached("Name", typeof(string), typeof(AutomationProperties), null);
        [OpenSilver.NotImplemented]
		public static string GetAcceleratorKey(DependencyObject element)
		{
			return (string)element.GetValue(AcceleratorKeyProperty);
		}

        [OpenSilver.NotImplemented]
		public static string GetAccessKey(DependencyObject element)
		{
			return (string)element.GetValue(AccessKeyProperty);
		}

        [OpenSilver.NotImplemented]
		public static string GetAutomationId(DependencyObject element)
		{
			return (string)element.GetValue(AutomationIdProperty);
		}

        [OpenSilver.NotImplemented]
		public static string GetHelpText(DependencyObject element)
		{
			return (string)element.GetValue(HelpTextProperty);
		}

        [OpenSilver.NotImplemented]
		public static bool GetIsRequiredForForm(DependencyObject element)
		{
			return (bool)element.GetValue(IsRequiredForFormProperty);
		}

        [OpenSilver.NotImplemented]
		public static string GetItemStatus(DependencyObject element)
		{
			return (string)element.GetValue(ItemStatusProperty);
		}

        [OpenSilver.NotImplemented]
		public static string GetItemType(DependencyObject element)
		{
			return (string)element.GetValue(ItemTypeProperty);
		}

        [OpenSilver.NotImplemented]
		public static UIElement GetLabeledBy(DependencyObject element)
		{
			return (UIElement)element.GetValue(LabeledByProperty);
		}

        [OpenSilver.NotImplemented]
		public static string GetName(DependencyObject element)
		{
			return (string)element.GetValue(NameProperty);
		}

        [OpenSilver.NotImplemented]
		public static void SetAcceleratorKey(DependencyObject element, string value)
		{
			element.SetValue(AcceleratorKeyProperty, value);
		}

        [OpenSilver.NotImplemented]
		public static void SetAccessKey(DependencyObject element, string value)
		{
			element.SetValue(AccessKeyProperty, value);
		}

        [OpenSilver.NotImplemented]
		public static void SetAutomationId(DependencyObject element, string value)
		{
			element.SetValue(AutomationIdProperty, value);
		}

        [OpenSilver.NotImplemented]
		public static void SetHelpText(DependencyObject element, string value)
		{
			element.SetValue(HelpTextProperty, value);
		}

        [OpenSilver.NotImplemented]
		public static void SetIsRequiredForForm(DependencyObject element, bool value)
		{
			element.SetValue(IsRequiredForFormProperty, value);
		}

        [OpenSilver.NotImplemented]
		public static void SetItemStatus(DependencyObject element, string value)
		{
			element.SetValue(ItemStatusProperty, value);
		}

        [OpenSilver.NotImplemented]
		public static void SetItemType(DependencyObject element, string value)
		{
			element.SetValue(ItemTypeProperty, value);
		}

        [OpenSilver.NotImplemented]
		public static void SetLabeledBy(DependencyObject element, UIElement value)
		{
			element.SetValue(LabeledByProperty, value);
		}

        [OpenSilver.NotImplemented]
		public static void SetName(DependencyObject element, string value)
		{
			element.SetValue(NameProperty, value);
		}
	}
}
