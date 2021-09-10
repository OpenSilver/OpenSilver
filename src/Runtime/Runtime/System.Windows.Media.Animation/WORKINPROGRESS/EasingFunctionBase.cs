#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
	//
	// Summary:
	//     Provides the base class for all the easing functions. You can create your own
	//     custom easing functions by inheriting from this class.
	public abstract partial class EasingFunctionBase : Freezable, IEasingFunction
	{
#if OPENSILVER
		//
		// Summary:
		//     Initializes a new instance of the System.Windows.Media.Animation.EasingFunctionBase
		//     class.
		protected EasingFunctionBase()
		{
			
		}
		
		//
		// Summary:
		//     Provides the logic portion of the easing function that you can override to produce
		//     the System.Windows.Media.Animation.EasingMode.EaseIn mode of the custom easing
		//     function.
		//
		// Parameters:
		//   normalizedTime:
		//     Normalized time (progress) of the animation.
		//
		// Returns:
		//     A double that represents the transformed progress.
        [OpenSilver.NotImplemented]
		protected abstract double EaseInCore(double normalizedTime);
#endif
		
		//
		// Summary:
		//     Transforms normalized time to control the pace of an animation.
		//
		// Parameters:
		//   normalizedTime:
		//     Normalized time (progress) of the animation.
		//
		// Returns:
		//     A double that represents the transformed progress.
        [OpenSilver.NotImplemented]
		public double Ease(double normalizedTime)
		{
			return default(double);
		}
	}
}
