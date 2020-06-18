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
	public static partial class SelectionItemPatternIdentifiers
	{
		public static readonly AutomationProperty IsSelectedProperty;
		public static readonly AutomationProperty SelectionContainerProperty;
	}
}
#endif