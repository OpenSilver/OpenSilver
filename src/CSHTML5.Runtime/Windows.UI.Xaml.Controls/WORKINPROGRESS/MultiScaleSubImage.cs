#if WORKINPROGRESS

#if MIGRATION
namespace System.Windows.Controls
{
	//
	// Summary:
	//     This class holds the properties for each sub-image within the System.Windows.Controls.MultiScaleImage.
	public sealed class MultiScaleSubImage : DependencyObject
	{
		//
		// Summary:
		//     Identifies the System.Windows.Controls.MultiScaleSubImage.AspectRatio dependency
		//     property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.MultiScaleSubImage.AspectRatio
		//     dependency property.
		public static readonly DependencyProperty AspectRatioProperty;
		//
		// Summary:
		//     Identifies the System.Windows.Controls.MultiScaleSubImage.Id dependency property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.MultiScaleSubImage.Id dependency
		//     property.
		public static readonly DependencyProperty IdProperty;
		//
		// Summary:
		//     Identifies the System.Windows.Controls.MultiScaleSubImage.Opacity dependency
		//     property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.MultiScaleSubImage.Opacity dependency
		//     property.
		public static readonly DependencyProperty OpacityProperty;
		//
		// Summary:
		//     Identifies the System.Windows.Controls.MultiScaleSubImage.OriginalPixelHeight
		//     dependency property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.MultiScaleSubImage.OriginalPixelHeight
		//     dependency property.
		public static readonly DependencyProperty OriginalPixelHeightProperty;
		//
		// Summary:
		//     Identifies the System.Windows.Controls.MultiScaleSubImage.OriginalPixelWidth
		//     dependency property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.MultiScaleSubImage.OriginalPixelWidth
		//     dependency property.
		public static readonly DependencyProperty OriginalPixelWidthProperty;
		//
		// Summary:
		//     Identifies the System.Windows.Controls.MultiScaleSubImage.ViewportOrigin dependency
		//     property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.MultiScaleSubImage.ViewportOrigin
		//     dependency property.
		public static readonly DependencyProperty ViewportOriginProperty;
		//
		// Summary:
		//     Identifies the System.Windows.Controls.MultiScaleSubImage.ViewportWidth dependency
		//     property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.MultiScaleSubImage.ViewportWidth
		//     dependency property.
		public static readonly DependencyProperty ViewportWidthProperty;
		//
		// Summary:
		//     Identifies the System.Windows.Controls.MultiScaleSubImage.ZIndex dependency property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.MultiScaleSubImage.ZIndex dependency
		//     property.
		public static readonly DependencyProperty ZIndexProperty;

		//
		// Summary:
		//     Initializes a new instance of the System.Windows.Controls.MultiScaleSubImage
		//     class.
		public MultiScaleSubImage()
		{
			
		}

		//
		// Summary:
		//     Gets the aspect ratio of the image used as the source of the System.Windows.Controls.MultiScaleSubImage.
		//     The aspect ratio is the width of the image divided by its height.
		//
		// Returns:
		//     The aspect ratio of the image used as the source of the System.Windows.Controls.MultiScaleSubImage.
		//     The aspect ratio is the width of the image divided by its height.
		public double AspectRatio { get; }
		//
		// Summary:
		//     Gets the Id of the image used as the source of the System.Windows.Controls.MultiScaleSubImage.
		//
		// Returns:
		//     The Id of the image used as the source of the System.Windows.Controls.MultiScaleSubImage.
		public int Id { get; }
		//
		// Summary:
		//     Gets or sets the degree of the System.Windows.Controls.MultiScaleSubImage opacity.
		//
		// Returns:
		//     A value between 0 and 1.0 that declares the opacity, with 1.0 meaning full opacity
		//     and 0 meaning transparent. The default value is 1.0.
		public double Opacity { get; set; }
		//
		// Summary:
		//     Gets the original height of the image to be displayed.
		//
		// Returns:
		//     The original height of the image to be displayed.
		public int OriginalPixelHeight { get; }
		//
		// Summary:
		//     Gets the original width of the image to be displayed.
		//
		// Returns:
		//     The original width of the image to be displayed.
		public int OriginalPixelWidth { get; }
		//
		// Summary:
		//     Gets or sets the top-left corner of the area of the image to be displayed.
		//
		// Returns:
		//     The point of the top-left corner of the rectangular area of the image to be displayed.
		public Point ViewportOrigin { get; set; }
		//
		// Summary:
		//     Gets or sets the width of the area of the image displayed.
		//
		// Returns:
		//     The width of the area of the image displayed.
		public double ViewportWidth { get; set; }
		//
		// Summary:
		//     Gets or sets a value that represents the z-order rendering behavior of the System.Windows.Controls.MultiScaleSubImage.
		//     Z-order determines the relative rendering order of objects (which object is on
		//     top of which other objects).
		//
		// Returns:
		//     The value that represents the z-order rendering behavior of the System.Windows.Controls.MultiScaleSubImage.
		//     The default value is 0.
		public int ZIndex { get; set; }
	}
}
#endif
#endif