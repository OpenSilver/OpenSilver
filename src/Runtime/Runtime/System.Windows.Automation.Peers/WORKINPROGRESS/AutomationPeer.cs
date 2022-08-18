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
    [OpenSilver.NotImplemented]
    public abstract partial class AutomationPeer : DependencyObject
    {
        [OpenSilver.NotImplemented]
        public static bool ListenerExists(AutomationEvents eventId)
        {
            return false;
        }

        [OpenSilver.NotImplemented]
        public AutomationPeer EventsSource
        {
            get;
            set;
        }

        [OpenSilver.NotImplemented]
        public void RaiseAutomationEvent(AutomationEvents eventId)
        {
        }

        [OpenSilver.NotImplemented]
        public abstract object GetPattern(PatternInterface patternInterface);
        [OpenSilver.NotImplemented]
        protected abstract List<AutomationPeer> GetChildrenCore();
        [OpenSilver.NotImplemented]
        protected abstract AutomationControlType GetAutomationControlTypeCore();
        [OpenSilver.NotImplemented]
        protected abstract string GetClassNameCore();
        [OpenSilver.NotImplemented]
        protected abstract string GetHelpTextCore();
        [OpenSilver.NotImplemented]
        protected abstract string GetLocalizedControlTypeCore();
        [OpenSilver.NotImplemented]
        protected abstract string GetItemStatusCore();
        [OpenSilver.NotImplemented]
        protected abstract AutomationOrientation GetOrientationCore();
        [OpenSilver.NotImplemented]
        protected abstract bool IsControlElementCore();
        [OpenSilver.NotImplemented]
        protected abstract string GetNameCore();
        [OpenSilver.NotImplemented]
        protected abstract Point GetClickablePointCore();
        [OpenSilver.NotImplemented]
        protected abstract string GetAutomationIdCore();
        [OpenSilver.NotImplemented]
        protected abstract AutomationPeer GetLabeledByCore();
        [OpenSilver.NotImplemented]
        protected abstract string GetItemTypeCore();
        [OpenSilver.NotImplemented]
        protected abstract bool IsContentElementCore();
        [OpenSilver.NotImplemented]
        protected abstract string GetAccessKeyCore();
        [OpenSilver.NotImplemented]
        protected abstract bool IsKeyboardFocusableCore();
        [OpenSilver.NotImplemented]
        protected abstract string GetAcceleratorKeyCore();
        [OpenSilver.NotImplemented]
        protected abstract Rect GetBoundingRectangleCore();
        [OpenSilver.NotImplemented]
        protected abstract bool HasKeyboardFocusCore();
        [OpenSilver.NotImplemented]
        protected abstract bool IsEnabledCore();
        [OpenSilver.NotImplemented]
        protected abstract bool IsOffscreenCore();
        [OpenSilver.NotImplemented]
        protected abstract bool IsPasswordCore();
        [OpenSilver.NotImplemented]
        protected abstract bool IsRequiredForFormCore();
        [OpenSilver.NotImplemented]
        protected abstract void SetFocusCore();
        [OpenSilver.NotImplemented]
        public string GetAcceleratorKey()
        {
            return null;
        }

        [OpenSilver.NotImplemented]
        public string GetAccessKey()
        {
            return null;
        }

        [OpenSilver.NotImplemented]
        public Rect GetBoundingRectangle()
        {
            return new Rect();
        }

        [OpenSilver.NotImplemented]
        public string GetLocalizedControlType()
        {
            return null;
        }

        [OpenSilver.NotImplemented]
        public Point GetClickablePoint()
        {
            return new Point();
        }

        [OpenSilver.NotImplemented]
        public string GetItemType()
        {
            return null;
        }

        [OpenSilver.NotImplemented]
        public AutomationPeer GetLabeledBy()
        {
            return null;
        }

        [OpenSilver.NotImplemented]
        public bool HasKeyboardFocus()
        {
            return false;
        }

        [OpenSilver.NotImplemented]
        public bool IsContentElement()
        {
            return false;
        }

        [OpenSilver.NotImplemented]
        public bool IsControlElement()
        {
            return false;
        }

        [OpenSilver.NotImplemented]
        public bool IsKeyboardFocusable()
        {
            return false;
        }

        [OpenSilver.NotImplemented]
        public bool IsOffscreen()
        {
            return false;
        }

        [OpenSilver.NotImplemented]
        public bool IsPassword()
        {
            return false;
        }

        [OpenSilver.NotImplemented]
        public bool IsRequiredForForm()
        {
            return false;
        }

        [OpenSilver.NotImplemented]
        public void SetFocus()
        {
        }

        [OpenSilver.NotImplemented]
        public string GetItemStatus()
        {
            return null;
        }

        public bool IsEnabled()
        {
            return IsEnabledCore();
        }

        [OpenSilver.NotImplemented]
        public string GetName()
        {
            return null;
        }

        [OpenSilver.NotImplemented]
        public AutomationOrientation GetOrientation()
        {
            return AutomationOrientation.None;
        }

        [OpenSilver.NotImplemented]
        public AutomationPeer GetParent()
        {
            return null;
        }

        [OpenSilver.NotImplemented]
        public List<AutomationPeer> GetChildren()
        {
            return null;
        }

        [OpenSilver.NotImplemented]
        public void RaisePropertyChangedEvent(AutomationProperty property, object oldValue, object newValue)
        {
        }

        [OpenSilver.NotImplemented]
        protected IRawElementProviderSimple ProviderFromPeer(AutomationPeer peer)
        {
            return null;
        }

        [OpenSilver.NotImplemented]
        public void InvalidatePeer()
        {
        }

        [OpenSilver.NotImplemented]
        public string GetClassName() => this.GetClassNameCore();

        [OpenSilver.NotImplemented]
        public string GetHelpText() => this.GetHelpTextCore();

        [OpenSilver.NotImplemented]
        public string GetAutomationId() => this.GetAutomationIdCore();

        [OpenSilver.NotImplemented]
        public AutomationControlType GetAutomationControlType() => this.GetAutomationControlTypeCore();
    }
}
