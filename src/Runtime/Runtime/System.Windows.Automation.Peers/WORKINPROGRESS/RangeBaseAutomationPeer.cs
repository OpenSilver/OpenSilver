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
using Windows.UI.Xaml.Controls.Primitives;
#endif

#if MIGRATION
namespace System.Windows.Automation.Peers
#else
namespace Windows.UI.Xaml.Automation.Peers
#endif
{
    [OpenSilver.NotImplemented]
    public partial class RangeBaseAutomationPeer : FrameworkElementAutomationPeer, IRangeValueProvider
    {
        [OpenSilver.NotImplemented]
        public RangeBaseAutomationPeer(RangeBase owner)
          : base((FrameworkElement)owner)
        {
        }

        [OpenSilver.NotImplemented]
        public override object GetPattern(PatternInterface patternInterface) => patternInterface == PatternInterface.RangeValue ? (object)this : (object)null;

        double IRangeValueProvider.Value => ((RangeBase)this.Owner).Value;

        bool IRangeValueProvider.IsReadOnly => !this.IsEnabled();

        double IRangeValueProvider.Maximum => ((RangeBase)this.Owner).Maximum;

        double IRangeValueProvider.Minimum => ((RangeBase)this.Owner).Minimum;

        double IRangeValueProvider.LargeChange => ((RangeBase)this.Owner).LargeChange;

        double IRangeValueProvider.SmallChange => ((RangeBase)this.Owner).SmallChange;

        [OpenSilver.NotImplemented]
        public void SetValue(double val)
        {
            if (!this.IsEnabled())
                throw new ElementNotEnabledException();
            RangeBase owner = (RangeBase)this.Owner;
            if (val < owner.Minimum || val > owner.Maximum)
                throw new ArgumentOutOfRangeException(nameof(val));
            owner.Value = val;
        }
    }
}
