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
	public static partial class SelectionItemPatternIdentifiers
	{
        [OpenSilver.NotImplemented]
		public static readonly AutomationProperty IsSelectedProperty;
        [OpenSilver.NotImplemented]
		public static readonly AutomationProperty SelectionContainerProperty;
	}
}
