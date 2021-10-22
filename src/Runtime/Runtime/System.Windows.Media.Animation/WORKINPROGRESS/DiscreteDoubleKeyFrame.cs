using System.Windows;
using System;

#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
    [OpenSilver.NotImplemented]
	public sealed partial class DiscreteDoubleKeyFrame : DoubleKeyFrame
	{
        [OpenSilver.NotImplemented]
		public DiscreteDoubleKeyFrame()
		{
		}
	}
}
