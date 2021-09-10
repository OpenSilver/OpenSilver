#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
	public sealed partial class QuinticEase : EasingFunctionBase
	{
        [OpenSilver.NotImplemented]
		protected override double EaseInCore(double normalizedTime)
		{
			return default(double);
		}

		[OpenSilver.NotImplemented]
		protected override Freezable CreateInstanceCore()
		{
			return default(Freezable);
		}
	}
}
