#if WORKINPROGRESS
using System;
using System.Collections.Generic;
using System.Text;

#if !MIGRATION
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Automation.Peers
#else
namespace Windows.UI.Xaml.Automation.Peers
#endif
{
	public partial class FrameworkElementAutomationPeer : AutomationPeer
	{
		public FrameworkElementAutomationPeer(FrameworkElement owner)
		{
		}

		public UIElement Owner
		{
			get;
			private set;
		}

		public static AutomationPeer FromElement(UIElement element)
		{
			return null;
		}

		public static AutomationPeer CreatePeerForElement(UIElement element)
		{
			return null;
		}

		public override object GetPattern(PatternInterface patternInterface)
		{
			return null;
		}

		protected override List<AutomationPeer> GetChildrenCore()
		{
			return null;
		}

		protected override AutomationControlType GetAutomationControlTypeCore()
		{
			return AutomationControlType.Window;
		}

		protected override string GetClassNameCore()
		{
			return null;
		}

		protected override string GetHelpTextCore()
		{
			return null;
		}

		protected override string GetLocalizedControlTypeCore()
		{
			return null;
		}

		protected override string GetItemStatusCore()
		{
			return null;
		}

		protected override AutomationOrientation GetOrientationCore()
		{
			return AutomationOrientation.None;
		}

		protected override bool IsControlElementCore()
		{
			return false;
		}

		protected override string GetNameCore()
		{
			return null;
		}

		protected override Point GetClickablePointCore()
		{
			return new Point();
		}

		protected override string GetAutomationIdCore()
		{
			return null;
		}

		protected override AutomationPeer GetLabeledByCore()
		{
			return null;
		}

		protected override string GetItemTypeCore()
		{
			return null;
		}

		protected override bool IsContentElementCore()
		{
			return false;
		}

		protected override string GetAccessKeyCore()
		{
			return null;
		}

		protected override bool IsKeyboardFocusableCore()
		{
			return false;
		}

		protected override string GetAcceleratorKeyCore()
		{
			return null;
		}

		protected override Rect GetBoundingRectangleCore()
		{
			return new Rect();
		}

		protected override bool HasKeyboardFocusCore()
		{
			return false;
		}

		protected override bool IsEnabledCore()
		{
			return false;
		}

		protected override bool IsOffscreenCore()
		{
			return false;
		}

		protected override bool IsPasswordCore()
		{
			return false;
		}

		protected override bool IsRequiredForFormCore()
		{
			return false;
		}

		protected override void SetFocusCore()
		{
		}
	}
}
#endif