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
    [OpenSilver.NotImplemented]
    public partial class FrameworkElementAutomationPeer : AutomationPeer
    {
        [OpenSilver.NotImplemented]
        public FrameworkElementAutomationPeer(FrameworkElement owner)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        [OpenSilver.NotImplemented]
        public UIElement Owner
        {
            get;
            private set;
        }

        [OpenSilver.NotImplemented]
        public static AutomationPeer FromElement(UIElement element)
        {
            return null;
        }

        [OpenSilver.NotImplemented]
        public static AutomationPeer CreatePeerForElement(UIElement element)
        {
            return null;
        }

        [OpenSilver.NotImplemented]
        public override object GetPattern(PatternInterface patternInterface)
        {
            return null;
        }

        [OpenSilver.NotImplemented]
        protected override List<AutomationPeer> GetChildrenCore()
        {
            return null;
        }

        [OpenSilver.NotImplemented]
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Window;
        }

        [OpenSilver.NotImplemented]
        protected override string GetClassNameCore()
        {
            return null;
        }

        [OpenSilver.NotImplemented]
        protected override string GetHelpTextCore()
        {
            return null;
        }

        [OpenSilver.NotImplemented]
        protected override string GetLocalizedControlTypeCore()
        {
            return null;
        }

        [OpenSilver.NotImplemented]
        protected override string GetItemStatusCore()
        {
            return null;
        }

        [OpenSilver.NotImplemented]
        protected override AutomationOrientation GetOrientationCore()
        {
            return AutomationOrientation.None;
        }

        [OpenSilver.NotImplemented]
        protected override bool IsControlElementCore()
        {
            return false;
        }

        [OpenSilver.NotImplemented]
        protected override string GetNameCore()
        {
            return null;
        }

        [OpenSilver.NotImplemented]
        protected override Point GetClickablePointCore()
        {
            return new Point();
        }

        [OpenSilver.NotImplemented]
        protected override string GetAutomationIdCore()
        {
            return null;
        }

        [OpenSilver.NotImplemented]
        protected override AutomationPeer GetLabeledByCore()
        {
            return null;
        }

        [OpenSilver.NotImplemented]
        protected override string GetItemTypeCore()
        {
            return null;
        }

        [OpenSilver.NotImplemented]
        protected override bool IsContentElementCore()
        {
            return false;
        }

        [OpenSilver.NotImplemented]
        protected override string GetAccessKeyCore()
        {
            return null;
        }

        [OpenSilver.NotImplemented]
        protected override bool IsKeyboardFocusableCore()
        {
            return false;
        }

        [OpenSilver.NotImplemented]
        protected override string GetAcceleratorKeyCore()
        {
            return null;
        }

        [OpenSilver.NotImplemented]
        protected override Rect GetBoundingRectangleCore()
        {
            return new Rect();
        }

        [OpenSilver.NotImplemented]
        protected override bool HasKeyboardFocusCore()
        {
            return false;
        }

        protected override bool IsEnabledCore()
        {
            var fe = Owner as FrameworkElement;
            return fe?.IsEnabled ?? false;
        }

        [OpenSilver.NotImplemented]
        protected override bool IsOffscreenCore()
        {
            return false;
        }

        [OpenSilver.NotImplemented]
        protected override bool IsPasswordCore()
        {
            return false;
        }

        [OpenSilver.NotImplemented]
        protected override bool IsRequiredForFormCore()
        {
            return false;
        }

        [OpenSilver.NotImplemented]
        protected override void SetFocusCore()
        {
        }
    }
}
