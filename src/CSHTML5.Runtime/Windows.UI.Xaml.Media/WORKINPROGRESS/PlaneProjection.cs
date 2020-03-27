#if WORKINPROGRESS
using System.Windows;
using System;

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
	public sealed partial class PlaneProjection : Projection
	{
		public PlaneProjection()
		{
		}
	}
}
#endif