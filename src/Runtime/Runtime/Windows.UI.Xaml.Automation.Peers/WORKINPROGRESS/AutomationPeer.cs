#if WORKINPROGRESS
using System;
using System.Collections.Generic;
using System.Text;
#if MIGRATION
using System.Windows.Automation.Provider;
#else
using Windows.UI.Xaml.Automation.Provider;
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Automation.Peers
#else
namespace Windows.UI.Xaml.Automation.Peers
#endif
{
	public abstract partial class AutomationPeer : DependencyObject
	{
		public static bool ListenerExists(AutomationEvents eventId)
		{
			return false;
		}

		public AutomationPeer EventsSource
		{
			get;
			set;
		}

		public void RaiseAutomationEvent(AutomationEvents eventId)
		{
		}

		public abstract object GetPattern(PatternInterface patternInterface);
		protected abstract List<AutomationPeer> GetChildrenCore();
		protected abstract AutomationControlType GetAutomationControlTypeCore();
		protected abstract string GetClassNameCore();
		protected abstract string GetHelpTextCore();
		protected abstract string GetLocalizedControlTypeCore();
		protected abstract string GetItemStatusCore();
		protected abstract AutomationOrientation GetOrientationCore();
		protected abstract bool IsControlElementCore();
		protected abstract string GetNameCore();
		protected abstract Point GetClickablePointCore();
		protected abstract string GetAutomationIdCore();
		protected abstract AutomationPeer GetLabeledByCore();
		protected abstract string GetItemTypeCore();
		protected abstract bool IsContentElementCore();
		protected abstract string GetAccessKeyCore();
		protected abstract bool IsKeyboardFocusableCore();
		protected abstract string GetAcceleratorKeyCore();
		protected abstract Rect GetBoundingRectangleCore();
		protected abstract bool HasKeyboardFocusCore();
		protected abstract bool IsEnabledCore();
		protected abstract bool IsOffscreenCore();
		protected abstract bool IsPasswordCore();
		protected abstract bool IsRequiredForFormCore();
		protected abstract void SetFocusCore();
		public string GetAcceleratorKey()
		{
			return null;
		}

		public string GetAccessKey()
		{
			return null;
		}

		public Rect GetBoundingRectangle()
		{
			return new Rect();
		}

		public string GetLocalizedControlType()
		{
			return null;
		}

		public Point GetClickablePoint()
		{
			return new Point();
		}

		public string GetItemType()
		{
			return null;
		}

		public AutomationPeer GetLabeledBy()
		{
			return null;
		}

		public bool HasKeyboardFocus()
		{
			return false;
		}

		public bool IsContentElement()
		{
			return false;
		}

		public bool IsControlElement()
		{
			return false;
		}

		public bool IsKeyboardFocusable()
		{
			return false;
		}

		public bool IsOffscreen()
		{
			return false;
		}

		public bool IsPassword()
		{
			return false;
		}

		public bool IsRequiredForForm()
		{
			return false;
		}

		public void SetFocus()
		{
		}

		public string GetItemStatus()
		{
			return null;
		}

		public bool IsEnabled()
		{
			return false;
		}

		public string GetName()
		{
			return null;
		}

		public AutomationOrientation GetOrientation()
		{
			return AutomationOrientation.None;
		}

		public AutomationPeer GetParent()
		{
			return null;
		}

		public List<AutomationPeer> GetChildren()
		{
			return null;
		}

		public void RaisePropertyChangedEvent(AutomationProperty property, object oldValue, object newValue)
		{
		}

		protected IRawElementProviderSimple ProviderFromPeer(AutomationPeer peer)
		{
			return null;
		}

		public void InvalidatePeer()
		{
		}

		public string GetClassName() => this.GetClassNameCore();

		public string GetHelpText() => this.GetHelpTextCore();

		public string GetAutomationId() => this.GetAutomationIdCore();
	}
}
#endif