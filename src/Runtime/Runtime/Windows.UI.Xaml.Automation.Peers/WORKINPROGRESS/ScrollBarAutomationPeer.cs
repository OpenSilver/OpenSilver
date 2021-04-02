#if WORKINPROGRESS
using System;
using System.Collections.Generic;
using System.Text;

#if MIGRATION
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives
#endif

#if MIGRATION
namespace System.Windows.Automation.Peers
#else
namespace Windows.UI.Xaml.Automation.Peers
#endif
{
    public class ScrollBarAutomationPeer : RangeBaseAutomationPeer
    {
        public ScrollBarAutomationPeer(ScrollBar owner)
          : base((RangeBase)owner)
        {
        }

        protected override string GetClassNameCore() => "ScrollBar";

        protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.ScrollBar;

        protected override Point GetClickablePointCore() => new Point(double.NaN, double.NaN);

        protected override AutomationOrientation GetOrientationCore() => ((ScrollBar)this.Owner).Orientation != Orientation.Horizontal ? AutomationOrientation.Vertical : AutomationOrientation.Horizontal;
    }
}
#endif
