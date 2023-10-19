using System.Collections.ObjectModel;
using System.Windows.Automation.Peers;
using System.Windows.Media;

namespace System.Windows.Controls
{
	//
	// Summary:
	//     Enables users to open a multi-resolution image, which can be zoomed in on and
	//     panned across.
    [OpenSilver.NotImplemented]
	public sealed partial class MultiScaleImage : FrameworkElement
	{
		//
		// Summary:
		//     Identifies the System.Windows.Controls.MultiScaleImage.AllowDownloading dependency
		//     property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.MultiScaleImage.AllowDownloading
		//     dependency property.
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty AllowDownloadingProperty =
			DependencyProperty.Register("AllowDownloading",
										typeof(bool),
										typeof(MultiScaleImage),
										null);
		//
		// Summary:
		//     Identifies the System.Windows.Controls.MultiScaleImage.AspectRatio dependency
		//     property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.MultiScaleImage.AspectRatio dependency
		//     property.
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty AspectRatioProperty =
			DependencyProperty.Register("AspectRatio",
										typeof(double),
										typeof(MultiScaleImage),
										null);
		//
		// Summary:
		//     Identifies the System.Windows.Controls.MultiScaleImage.BlurFactor dependency
		//     property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.MultiScaleImage.BlurFactor dependency
		//     property.
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty BlurFactorProperty =
			DependencyProperty.Register("BlurFactor",
										typeof(double),
										typeof(MultiScaleImage),
										new PropertyMetadata(1d));
		//
		// Summary:
		//     Identifies the System.Windows.Controls.MultiScaleImage.IsDownloading dependency
		//     property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.MultiScaleImage.IsDownloading
		//     dependency property.
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty IsDownloadingProperty =
			DependencyProperty.Register("IsDownloading",
										typeof(bool),
										typeof(MultiScaleImage),
										null);
		//
		// Summary:
		//     Identifies the System.Windows.Controls.MultiScaleImage.IsIdle dependency property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.MultiScaleImage.IsIdle dependency
		//     property.
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty IsIdleProperty =
			DependencyProperty.Register("IsIdle",
										typeof(bool),
										typeof(MultiScaleImage),
										null);
		//
		// Summary:
		//     Identifies the System.Windows.Controls.MultiScaleImage.OriginalPixelHeight dependency
		//     property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.MultiScaleImage.OriginalPixelHeight
		//     dependency property.
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty OriginalPixelHeightProperty =
			DependencyProperty.Register("OriginalPixelHeight",
										typeof(int),
										typeof(MultiScaleImage),
										null);
		//
		// Summary:
		//     Identifies the System.Windows.Controls.MultiScaleImage.OriginalPixelWidth dependency
		//     property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.MultiScaleImage.OriginalPixelWidth
		//     dependency property.
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty OriginalPixelWidthProperty =
			DependencyProperty.Register("OriginalPixelWidth",
										typeof(int),
										typeof(MultiScaleImage),
										null);
		//
		// Summary:
		//     Identifies the System.Windows.Controls.MultiScaleImage.SkipLevels dependency
		//     property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.MultiScaleImage.SkipLevels dependency
		//     property.
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty SkipLevelsProperty =
			DependencyProperty.Register("SkipLevels",
										typeof(int),
										typeof(MultiScaleImage),
										null);
		//
		// Summary:
		//     Identifies the System.Windows.Controls.MultiScaleImage.Source dependency property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.MultiScaleImage.Source dependency
		//     property.
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty SourceProperty =
			DependencyProperty.Register("Source",
										typeof(MultiScaleTileSource),
										typeof(MultiScaleImage),
										null);
		//
		// Summary:
		//     Identifies the System.Windows.Controls.MultiScaleImage.SubImages dependency property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.MultiScaleImage.SubImages dependency
		//     property.
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty SubImagesProperty =
			DependencyProperty.Register("SubImages",
										typeof(ReadOnlyCollection<MultiScaleSubImage>),
										typeof(MultiScaleImage),
										null);
		//
		// Summary:
		//     Identifies the System.Windows.Controls.MultiScaleImage.UseSprings dependency
		//     property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.MultiScaleImage.UseSprings dependency
		//     property.
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty UseSpringsProperty =
			DependencyProperty.Register("UseSprings",
										typeof(bool),
										typeof(MultiScaleImage),
										new PropertyMetadata(true));
		//
		// Summary:
		//     Identifies the System.Windows.Controls.MultiScaleImage.ViewportOrigin dependency
		//     property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.MultiScaleImage.ViewportOrigin
		//     dependency property.
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty ViewportOriginProperty =
			DependencyProperty.Register("ViewportOrigin",
										typeof(Point),
										typeof(MultiScaleImage),
										null);
		//
		// Summary:
		//     Identifies the System.Windows.Controls.MultiScaleImage.ViewportWidth dependency
		//     property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.MultiScaleImage.ViewportWidth
		//     dependency property.
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty ViewportWidthProperty =
			DependencyProperty.Register("ViewportWidth",
										typeof(double),
										typeof(MultiScaleImage),
										null);

		//
		// Summary:
		//     Initializes a new instance of the System.Windows.Controls.MultiScaleImage class.
        [OpenSilver.NotImplemented]
		public MultiScaleImage()
		{
			
		}

		//
		// Summary:
		//     Gets or sets a value that indicates whether downloading is permitted by this
		//     System.Windows.Controls.MultiScaleImage.
		//
		// Returns:
		//     true if downloading is permitted by this System.Windows.Controls.MultiScaleImage.
		//     false if downloading is not permitted by this System.Windows.Controls.MultiScaleImage.
        [OpenSilver.NotImplemented]
		public bool AllowDownloading
		{
			get { return (bool)GetValue(AllowDownloadingProperty); }
			set { SetValue(AllowDownloadingProperty, value); }
		}
		//
		// Summary:
		//     Gets the aspect ratio of the image used as the source of the System.Windows.Controls.MultiScaleImage.
		//     The aspect ratio is the width of the image divided by its height.
		//
		// Returns:
		//     The aspect ratio of the image used as the source of the System.Windows.Controls.MultiScaleImage.
		//     The aspect ratio is the width of the image divided by its height.
        [OpenSilver.NotImplemented]
		public double AspectRatio
		{
			get { return (double)GetValue(AspectRatioProperty); }
		}
		//
		// Summary:
		//     Gets or sets the extent that data is blurred while rendering.
		//
		// Returns:
		//     The extent that data is blurred while rendering. A value of 2 means that data
		//     is twice as blurry (one level lower), while a value of 0.5 means that data is
		//     sharper (one level higher). The default is 1.
        [OpenSilver.NotImplemented]
		public double BlurFactor
		{
			get { return (double)GetValue(BlurFactorProperty); }
			set { SetValue(BlurFactorProperty, value); }
		}
		//
		// Summary:
		//     Gets a value that indicates whether the image is still downloading.
		//
		// Returns:
		//     true if the image is still downloading. false if all the needed tiles have been
		//     downloaded. If the image is moved, System.Windows.Controls.MultiScaleImage.IsDownloading
		//     may become true again.
        [OpenSilver.NotImplemented]
		public bool IsDownloading
		{
			get { return (bool)GetValue(IsDownloadingProperty); }
		}
		//
		// Summary:
		//     Gets a value that indicates whether Deep Zoom is done downloading, decoding,
		//     blending, and animating (if springs are being used) images.
		//
		// Returns:
		//     true, if Deep Zoom is done downloading, decoding, blending, and animating (if
		//     springs are being used) images. Otherwise, false.
        [OpenSilver.NotImplemented]
		public bool IsIdle
		{
			get { return (bool)GetValue(IsIdleProperty); }
		}
		//
		// Summary:
		//     Gets the original height of the image used as the source of the System.Windows.Controls.MultiScaleImage.
		//
		// Returns:
		//     The height of the image used as the source of the System.Windows.Controls.MultiScaleImage.
        [OpenSilver.NotImplemented]
		public int OriginalPixelHeight
		{
			get { return (int)GetValue(OriginalPixelHeightProperty); }
		}
		//
		// Summary:
		//     Gets the original width of the image used as the source of the System.Windows.Controls.MultiScaleImage.
		//
		// Returns:
		//     Returns System.Int32.
        [OpenSilver.NotImplemented]
		public int OriginalPixelWidth
		{
			get { return (int)GetValue(OriginalPixelWidthProperty); }
		}
		//
		// Summary:
		//     Gets or sets a value that indicates levels to be skipped while loading a System.Windows.Controls.MultiScaleImage.
		//
		// Returns:
		//     An integer that represents the levels that are skipped while loading a System.Windows.Controls.MultiScaleImage.
        [OpenSilver.NotImplemented]
		public int SkipLevels
		{
			get { return (int)GetValue(SkipLevelsProperty); }
			set { SetValue(SkipLevelsProperty, value); }
		}
		//
		// Summary:
		//     Gets or sets the System.Windows.Media.MultiScaleTileSource object that is used
		//     as the source for the System.Windows.Controls.MultiScaleImage.
		//
		// Returns:
		//     The System.Windows.Media.MultiScaleTileSource object that is used as the source
		//     for the System.Windows.Controls.MultiScaleImage.
        [OpenSilver.NotImplemented]
		public MultiScaleTileSource Source
		{
			get { return (MultiScaleTileSource)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}
		//
		// Summary:
		//     Gets the collection of System.Windows.Controls.MultiScaleSubImage objects within
		//     the multiresolution image that is used by the System.Windows.Controls.MultiScaleImage.
		//
		// Returns:
		//     The collection of System.Windows.Controls.MultiScaleSubImage objects within the
		//     multiresolution image that is used by the System.Windows.Controls.MultiScaleImage.
        [OpenSilver.NotImplemented]
		public ReadOnlyCollection<MultiScaleSubImage> SubImages
		{
			get { return (ReadOnlyCollection<MultiScaleSubImage>)GetValue(SubImagesProperty); }
		}
		//
		// Summary:
		//     Gets or sets a value that indicates whether the System.Windows.Controls.MultiScaleImage
		//     uses spring animations.
		//
		// Returns:
		//     true if the System.Windows.Controls.MultiScaleImage uses spring animations; otherwise,
		//     false. The default value is true.
        [OpenSilver.NotImplemented]
		public bool UseSprings
		{
			get { return (bool)GetValue(UseSpringsProperty); }
			set { SetValue(UseSpringsProperty, value); }
		}
		//
		// Summary:
		//     Gets or sets the top-left corner of the area of the image to be displayed.
		//
		// Returns:
		//     The top-left corner of the rectangular area of the image to be displayed.
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
		//     Occurs if the download of a tile times out or fails for another reason.
        [OpenSilver.NotImplemented]
		public event RoutedEventHandler ImageFailed;
		//
		// Summary:
		//     Occurs if the first piece of metadata used to open the image fails. If this event
		//     occurs no parts of the image will open successfully.
        [OpenSilver.NotImplemented]
		public event EventHandler<ExceptionRoutedEventArgs> ImageOpenFailed;
		//
		// Summary:
		//     Occurs when the first piece of metadata that is needed to load the rest of the
		//     tiles opens.
        [OpenSilver.NotImplemented]
		public event RoutedEventHandler ImageOpenSucceeded;
		//
		// Summary:
		//     Occurs when the zoom or pan animation ends.
        [OpenSilver.NotImplemented]
		public event RoutedEventHandler MotionFinished;
		//
		// Summary:
		//     Occurs when the collection of System.Windows.Controls.MultiScaleSubImage objects
		//     within the multiresolution image that is used by the System.Windows.Controls.MultiScaleImage
		//     fails to open.
        [OpenSilver.NotImplemented]
		public event SubImageEventHandler SubImageOpenFailed;
		//
		// Summary:
		//     Occurs when the collection of System.Windows.Controls.MultiScaleSubImage objects
		//     within the multiresolution image that is used by the System.Windows.Controls.MultiScaleImage
		//     opens successfully.
        [OpenSilver.NotImplemented]
		public event SubImageEventHandler SubImageOpenSucceeded;
		//
		// Summary:
		//     Occurs when the viewport (the area of the image displayed) changes.
        [OpenSilver.NotImplemented]
		public event RoutedEventHandler ViewportChanged;

		//
		// Summary:
		//     Gets a point with logical coordinates (values between 0 and 1) from a point of
		//     the System.Windows.Controls.MultiScaleImage.
		//
		// Parameters:
		//   elementPoint:
		//     The point on the System.Windows.Controls.MultiScaleImage to translate into a
		//     point with logical coordinates (values between 0 and 1).
		//
		// Returns:
		//     The logical point translated from the elementPoint.
        [OpenSilver.NotImplemented]
		public Point ElementToLogicalPoint(Point elementPoint)
		{
			return default(Point);
		}
		//
		// Summary:
		//     Gets a point with pixel coordinates relative to the System.Windows.Controls.MultiScaleImage
		//     from a logical point (values between 0 and 1).
		//
		// Parameters:
		//   logicalPoint:
		//     The logical point to translate into pixel coordinates relative to the System.Windows.Controls.MultiScaleImage.
		//
		// Returns:
		//     A point with pixel coordinates relative to the System.Windows.Controls.MultiScaleImage
		//     translated from logicalPoint.
        [OpenSilver.NotImplemented]
		public Point LogicalToElementPoint(Point logicalPoint)
		{
			return default(Point);
		}
		//
		// Summary:
		//     Enables a user to zoom in on a point of the System.Windows.Controls.MultiScaleImage.
		//
		// Parameters:
		//   zoomIncrementFactor:
		//     Specifies the zoom. This number is greater than 0. A value of 1 specifies that
		//     the image fit the allotted page size exactly. A number greater than 1 specifies
		//     to zoom in. If a value of 0 or less is used, failure is returned and no zoom
		//     changes are applied.
		//
		//   zoomCenterLogicalX:
		//     X coordinate for the point on the System.Windows.Controls.MultiScaleImage that
		//     is zoomed in on. This is a logical point (between 0 and 1).
		//
		//   zoomCenterLogicalY:
		//     Y coordinate for the point on the System.Windows.Controls.MultiScaleImage that
		//     is zoomed in on. This is a logical point (between 0 and 1).
        [OpenSilver.NotImplemented]
		public void ZoomAboutLogicalPoint(double zoomIncrementFactor, double zoomCenterLogicalX, double zoomCenterLogicalY)
		{
			
		}

        [OpenSilver.NotImplemented]
		protected override AutomationPeer OnCreateAutomationPeer()
		{
			return default(AutomationPeer);
		}
	}
}
