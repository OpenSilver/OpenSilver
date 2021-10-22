using System;
using System.Collections.Generic;
using System.Text;

#if MIGRATION
namespace System.Windows.Automation.Provider
#else
namespace Windows.UI.Xaml.Automation.Provider
#endif
{
    [OpenSilver.NotImplemented]
	public sealed partial class IRawElementProviderSimple
	{
	}
}
