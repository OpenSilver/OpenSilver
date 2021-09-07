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
	public partial class ElementNotAvailableException : Exception
	{
        [OpenSilver.NotImplemented]
		public ElementNotAvailableException()
		{
		}
	}
}
