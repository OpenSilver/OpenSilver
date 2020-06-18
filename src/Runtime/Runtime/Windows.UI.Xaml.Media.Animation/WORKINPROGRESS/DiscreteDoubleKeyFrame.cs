#if WORKINPROGRESS
using System.Windows;
using System;

#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
	public sealed partial class DiscreteDoubleKeyFrame : DoubleKeyFrame
	{
		public DiscreteDoubleKeyFrame()
		{
		}
	}
}
#endif