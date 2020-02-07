using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if WORKINPROGRESS

#if MIGRATION
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
    public sealed class StylusDevice
    {
        public TabletDeviceType DeviceType { get; private set; }
    }
}
#endif