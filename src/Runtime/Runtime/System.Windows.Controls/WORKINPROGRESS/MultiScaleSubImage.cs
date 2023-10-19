namespace System.Windows.Controls
{
	//
	// Summary:
	//     This class holds the properties for each sub-image within the System.Windows.Controls.MultiScaleImage.
    [OpenSilver.NotImplemented]
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
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty AspectRatioProperty =
			DependencyProperty.Register("AspectRatio",
										typeof(double),
										typeof(MultiScaleSubImage),
										null);
		//
		// Summary:
		//     Identifies the System.Windows.Controls.MultiScaleSubImage.Id dependency property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.MultiScaleSubImage.Id dependency
		//     property.
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty IdProperty =
			DependencyProperty.Register("Id",
										typeof(int),
										typeof(MultiScaleSubImage),
										null);
		//
		// Summary:
		//     Identifies the System.Windows.Controls.MultiScaleSubImage.Opacity dependency
		//     property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.MultiScaleSubImage.Opacity dependency
		//     property.
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty OpacityProperty =
			DependencyProperty.Register("Opacity",
										typeof(double),
										typeof(MultiScaleSubImage),
										new PropertyMetadata(1d));
		//
		// Summary:
		//     Identifies the System.Windows.Controls.MultiScaleSubImage.OriginalPixelHeight
		//     dependency property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.MultiScaleSubImage.OriginalPixelHeight
		//     dependency property.
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty OriginalPixelHeightProperty =
			DependencyProperty.Register("OriginalPixelHeight",
										typeof(int),
										typeof(MultiScaleSubImage),
										null);
		//
		// Summary:
		//     Identifies the System.Windows.Controls.MultiScaleSubImage.OriginalPixelWidth
		//     dependency property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.MultiScaleSubImage.OriginalPixelWidth
		//     dependency property.
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty OriginalPixelWidthProperty =
			DependencyProperty.Register("OriginalPixelWidth",
										typeof(int),
										typeof(MultiScaleSubImage),
										null);
		//
		// Summary:
		//     Identifies the System.Windows.Controls.MultiScaleSubImage.ViewportOrigin dependency
		//     property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.MultiScaleSubImage.ViewportOrigin
		//     dependency property.
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty ViewportOriginProperty =
			DependencyProperty.Register("ViewportOrigin",
										typeof(Point),
										typeof(MultiScaleSubImage),
										null);
		//
		// Summary:
		//     Identifies the System.Windows.Controls.MultiScaleSubImage.ViewportWidth dependency
		//     property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.MultiScaleSubImage.ViewportWidth
		//     dependency property.
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty ViewportWidthProperty =
			DependencyProperty.Register("ViewportWidth",
										typeof(double),
										typeof(MultiScaleSubImage),
										null);
		//
		// Summary:
		//     Identifies the System.Windows.Controls.MultiScaleSubImage.ZIndex dependency property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.MultiScaleSubImage.ZIndex dependency
		//     property.
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty ZIndexProperty =
			DependencyProperty.Register("ZIndex",
										typeof(int),
										typeof(MultiScaleSubImage),
										null);

		//
		// Summary:
		//     Initializes a new instance of the System.Windows.Controls.MultiScaleSubImage
		//     class.
        [OpenSilver.NotImplemented]
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
        [OpenSilver.NotImplemented]
		public double AspectRatio
		{
			get { return (double)GetValue(AspectRatioProperty); }
		}
		//
		// Summary:
		//     Gets the Id of the image used as the source of the System.Windows.Controls.MultiScaleSubImage.
		//
		// Returns:
		//     The Id of the image used as the source of the System.Windows.Controls.MultiScaleSubImage.
        [OpenSilver.NotImplemented]
		public int Id
		{
			get { return (int)GetValue(IdProperty); }
		}
		//
		// Summary:
		//     Gets or sets the degree of the System.Windows.Controls.MultiScaleSubImage opacity.
		//
		// Returns:
		//     A value between 0 and 1.0 that declares the opacity, with 1.0 meaning full opacity
		//     and 0 meaning transparent. The default value is 1.0.
        [OpenSilver.NotImplemented]
		public double Opacity
		{
			get { return (double)GetValue(OpacityProperty); }
			set { SetValue(OpacityProperty, value); }
		}
		//
		// Summary:
		//     Gets the original height of the image to be displayed.
		//
		// Returns:
		//     The original height of the image to be displayed.
        [OpenSilver.NotImplemented]
		public int OriginalPixelHeight
		{
			get { return (int)GetValue(OriginalPixelHeightProperty); }
		}
		//
		// Summary:
		//     Gets the original width of the image to be displayed.
		//
		// Returns:
		//     The original width of the image to be displayed.
        [OpenSilver.NotImplemented]
		public int OriginalPixelWidth
		{
			get { return (int)GetValue(OriginalPixelWidthProperty); }
		}
		//
		// Summary:
		//     Gets or sets the top-left corner of the area of the image to be displayed.
		//
		// Returns:
		//     The point of the top-left corner of the rectangular area of the image to be displayed.
        [OpenSilver.NotImplemented]
		public Point ViewportOrigin
		{
			get { return (Point)GetValue(ViewportOriginProperty); }
			set { SetValue(ViewportOriginProperty, value); }
		}
		//
		// Summary:
		//     Gets or sets the width of the area of the image displayed.
		//
		// Returns:
		//     The width of the area of the image displayed.
        [OpenSilver.NotImplemented]
		public double ViewportWidth
		{
			get { return (double)GetValue(ViewportWidthProperty); }
			set { SetValue(ViewportWidthProperty, value); }
		}
		//
		// Summary:
		//     Gets or sets a value that represents the z-order rendering behavior of the System.Windows.Controls.MultiScaleSubImage.
		//     Z-order determines the relative rendering order of objects (which object is on
		//     top of which other objects).
		//
		// Returns:
		//     The value that represents the z-order rendering behavior of the System.Windows.Controls.MultiScaleSubImage.
		//     The default value is 0.
        [OpenSilver.NotImplemented]
		public int ZIndex
		{
			get { return (int)GetValue(ZIndexProperty); }
			set { SetValue(ZIndexProperty, value); }
		}
	}
}
