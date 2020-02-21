#if WORKINPROGRESS
using System.Windows;
using System;

#if MIGRATION
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
	public sealed partial class TouchPointCollection : PresentationFrameworkCollection<TouchPoint>
	{
		internal TouchPointCollection()
		{
		}
	}
}
#endif