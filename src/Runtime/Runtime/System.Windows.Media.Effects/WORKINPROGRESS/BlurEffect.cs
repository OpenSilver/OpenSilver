#if !MIGRATION
using Windows.UI.Xaml;
#endif

namespace System.Windows.Media.Effects
{
	//
	// Summary:
	//     Represents an effect that you can apply to an object that simulates looking at
	//     the object through an out-of-focus lens.
	[OpenSilver.NotImplemented]
	public sealed partial class BlurEffect : Effect
	{
		//
		// Summary:
		//     Identifies the System.Windows.Media.Effects.BlurEffect.Radius dependency property.
		//
		// Returns:
		//     The identifier for the System.Windows.Media.Effects.BlurEffect.Radius dependency
		//     property.
		[OpenSilver.NotImplemented]
		public static readonly DependencyProperty RadiusProperty =
			DependencyProperty.Register("Radius",
										typeof(double),
										typeof(BlurEffect),
										new PropertyMetadata(5d));

		//
		// Summary:
		//     Initializes a new instance of the System.Windows.Media.Effects.BlurEffect class.
		[OpenSilver.NotImplemented]
		public BlurEffect()
		{
			
		}

		//
		// Summary:
		//     Gets or sets the amount of blurring applied by the System.Windows.Media.Effects.BlurEffect.
		//
		// Returns:
		//     The radius used in the blur, as a pixel-based factor. The default is 5.
		[OpenSilver.NotImplemented]
		public double Radius
		{
			get { return (double)GetValue(RadiusProperty); }
			set { SetValue(RadiusProperty, value); }
		}
	}
}
