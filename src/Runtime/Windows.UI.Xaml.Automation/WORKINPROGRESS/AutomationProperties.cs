#if WORKINPROGRESS
using System;
using System.Collections.Generic;
using System.Text;

#if MIGRATION
namespace System.Windows.Automation
#else
namespace Windows.UI.Xaml.Automation
#endif
{
	public static partial class AutomationProperties
	{
		public static readonly DependencyProperty AcceleratorKeyProperty = DependencyProperty.RegisterAttached("AcceleratorKey", typeof(string), typeof(AutomationProperties), null);
		public static readonly DependencyProperty AccessKeyProperty = DependencyProperty.RegisterAttached("AccessKey", typeof(string), typeof(AutomationProperties), null);
		public static readonly DependencyProperty AutomationIdProperty = DependencyProperty.RegisterAttached("AutomationId", typeof(string), typeof(AutomationProperties), null);
		public static readonly DependencyProperty HelpTextProperty = DependencyProperty.RegisterAttached("HelpText", typeof(string), typeof(AutomationProperties), null);
		public static readonly DependencyProperty IsRequiredForFormProperty = DependencyProperty.RegisterAttached("IsRequiredForForm", typeof(bool), typeof(AutomationProperties), null);
		public static readonly DependencyProperty ItemStatusProperty = DependencyProperty.RegisterAttached("ItemStatus", typeof(string), typeof(AutomationProperties), null);
		public static readonly DependencyProperty ItemTypeProperty = DependencyProperty.RegisterAttached("ItemType", typeof(string), typeof(AutomationProperties), null);
		public static readonly DependencyProperty LabeledByProperty = DependencyProperty.RegisterAttached("LabeledBy", typeof(UIElement), typeof(AutomationProperties), null);
		public static readonly DependencyProperty NameProperty = DependencyProperty.RegisterAttached("Name", typeof(string), typeof(AutomationProperties), null);
		public static string GetAcceleratorKey(DependencyObject element)
		{
			return (string)element.GetValue(AcceleratorKeyProperty);
		}

		public static string GetAccessKey(DependencyObject element)
		{
			return (string)element.GetValue(AccessKeyProperty);
		}

		public static string GetAutomationId(DependencyObject element)
		{
			return (string)element.GetValue(AutomationIdProperty);
		}

		public static string GetHelpText(DependencyObject element)
		{
			return (string)element.GetValue(HelpTextProperty);
		}

		public static bool GetIsRequiredForForm(DependencyObject element)
		{
			return (bool)element.GetValue(IsRequiredForFormProperty);
		}

		public static string GetItemStatus(DependencyObject element)
		{
			return (string)element.GetValue(ItemStatusProperty);
		}

		public static string GetItemType(DependencyObject element)
		{
			return (string)element.GetValue(ItemTypeProperty);
		}

		public static UIElement GetLabeledBy(DependencyObject element)
		{
			return (UIElement)element.GetValue(LabeledByProperty);
		}

		public static string GetName(DependencyObject element)
		{
			return (string)element.GetValue(NameProperty);
		}

		public static void SetAcceleratorKey(DependencyObject element, string value)
		{
			element.SetValue(AcceleratorKeyProperty, value);
		}

		public static void SetAccessKey(DependencyObject element, string value)
		{
			element.SetValue(AccessKeyProperty, value);
		}

		public static void SetAutomationId(DependencyObject element, string value)
		{
			element.SetValue(AutomationIdProperty, value);
		}

		public static void SetHelpText(DependencyObject element, string value)
		{
			element.SetValue(HelpTextProperty, value);
		}

		public static void SetIsRequiredForForm(DependencyObject element, bool value)
		{
			element.SetValue(IsRequiredForFormProperty, value);
		}

		public static void SetItemStatus(DependencyObject element, string value)
		{
			element.SetValue(ItemStatusProperty, value);
		}

		public static void SetItemType(DependencyObject element, string value)
		{
			element.SetValue(ItemTypeProperty, value);
		}

		public static void SetLabeledBy(DependencyObject element, UIElement value)
		{
			element.SetValue(LabeledByProperty, value);
		}

		public static void SetName(DependencyObject element, string value)
		{
			element.SetValue(NameProperty, value);
		}
	}
}
#endif