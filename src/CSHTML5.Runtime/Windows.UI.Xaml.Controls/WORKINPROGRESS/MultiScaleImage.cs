#if WORKINPROGRESS

#if MIGRATION
using System.Collections.ObjectModel;
using System.Windows.Automation.Peers;
using System.Windows.Media;

namespace System.Windows.Controls
{
	//
	// Summary:
	//     Enables users to open a multi-resolution image, which can be zoomed in on and
	//     panned across.
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
		public static readonly DependencyProperty AllowDownloadingProperty;
		//
		// Summary:
		//     Identifies the System.Windows.Controls.MultiScaleImage.AspectRatio dependency
		//     property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.MultiScaleImage.AspectRatio dependency
		//     property.
		public static readonly DependencyProperty AspectRatioProperty;
		//
		// Summary:
		//     Identifies the System.Windows.Controls.MultiScaleImage.BlurFactor dependency
		//     property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.MultiScaleImage.BlurFactor dependency
		//     property.
		public static readonly DependencyProperty BlurFactorProperty;
		//
		// Summary:
		//     Identifies the System.Windows.Controls.MultiScaleImage.IsDownloading dependency
		//     property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.MultiScaleImage.IsDownloading
		//     dependency property.
		public static readonly DependencyProperty IsDownloadingProperty;
		//
		// Summary:
		//     Identifies the System.Windows.Controls.MultiScaleImage.IsIdle dependency property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.MultiScaleImage.IsIdle dependency
		//     property.
		public static readonly DependencyProperty IsIdleProperty;
		//
		// Summary:
		//     Identifies the System.Windows.Controls.MultiScaleImage.OriginalPixelHeight dependency
		//     property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.MultiScaleImage.OriginalPixelHeight
		//     dependency property.
		public static readonly DependencyProperty OriginalPixelHeightProperty;
		//
		// Summary:
		//     Identifies the System.Windows.Controls.MultiScaleImage.OriginalPixelWidth dependency
		//     property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.MultiScaleImage.OriginalPixelWidth
		//     dependency property.
		public static readonly DependencyProperty OriginalPixelWidthProperty;
		//
		// Summary:
		//     Identifies the System.Windows.Controls.MultiScaleImage.SkipLevels dependency
		//     property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.MultiScaleImage.SkipLevels dependency
		//     property.
		public static readonly DependencyProperty SkipLevelsProperty;
		//
		// Summary:
		//     Identifies the System.Windows.Controls.MultiScaleImage.Source dependency property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.MultiScaleImage.Source dependency
		//     property.
		public static readonly DependencyProperty SourceProperty;
		//
		// Summary:
		//     Identifies the System.Windows.Controls.MultiScaleImage.SubImages dependency property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.MultiScaleImage.SubImages dependency
		//     property.
		public static readonly DependencyProperty SubImagesProperty;
		//
		// Summary:
		//     Identifies the System.Windows.Controls.MultiScaleImage.UseSprings dependency
		//     property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.MultiScaleImage.UseSprings dependency
		//     property.
		public static readonly DependencyProperty UseSpringsProperty;
		//
		// Summary:
		//     Identifies the System.Windows.Controls.MultiScaleImage.ViewportOrigin dependency
		//     property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.MultiScaleImage.ViewportOrigin
		//     dependency property.
		public static readonly DependencyProperty ViewportOriginProperty;
		//
		// Summary:
		//     Identifies the System.Windows.Controls.MultiScaleImage.ViewportWidth dependency
		//     property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.MultiScaleImage.ViewportWidth
		//     dependency property.
		public static readonly DependencyProperty ViewportWidthProperty;

		//
		// Summary:
		//     Initializes a new instance of the System.Windows.Controls.MultiScaleImage class.
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
		public bool AllowDownloading { get; set; }
		//
		// Summary:
		//     Gets the aspect ratio of the image used as the source of the System.Windows.Controls.MultiScaleImage.
		//     The aspect ratio is the width of the image divided by its height.
		//
		// Returns:
		//     The aspect ratio of the image used as the source of the System.Windows.Controls.MultiScaleImage.
		//     The aspect ratio is the width of the image divided by its height.
		public double AspectRatio { get; }
		//
		// Summary:
		//     Gets or sets the extent that data is blurred while rendering.
		//
		// Returns:
		//     The extent that data is blurred while rendering. A value of 2 means that data
		//     is twice as blurry (one level lower), while a value of 0.5 means that data is
		//     sharper (one level higher). The default is 1.
		public double BlurFactor { get; set; }
		//
		// Summary:
		//     Gets a value that indicates whether the image is still downloading.
		//
		// Returns:
		//     true if the image is still downloading. false if all the needed tiles have been
		//     downloaded. If the image is moved, System.Windows.Controls.MultiScaleImage.IsDownloading
		//     may become true again.
		public bool IsDownloading { get; }
		//
		// Summary:
		//     Gets a value that indicates whether Deep Zoom is done downloading, decoding,
		//     blending, and animating (if springs are being used) images.
		//
		// Returns:
		//     true, if Deep Zoom is done downloading, decoding, blending, and animating (if
		//     springs are being used) images. Otherwise, false.
		public bool IsIdle { get; }
		//
		// Summary:
		//     Gets the original height of the image used as the source of the System.Windows.Controls.MultiScaleImage.
		//
		// Returns:
		//     The height of the image used as the source of the System.Windows.Controls.MultiScaleImage.
		public int OriginalPixelHeight { get; }
		//
		// Summary:
		//     Gets the original width of the image used as the source of the System.Windows.Controls.MultiScaleImage.
		//
		// Returns:
		//     Returns System.Int32.
		public int OriginalPixelWidth { get; }
		//
		// Summary:
		//     Gets or sets a value that indicates levels to be skipped while loading a System.Windows.Controls.MultiScaleImage.
		//
		// Returns:
		//     An integer that represents the levels that are skipped while loading a System.Windows.Controls.MultiScaleImage.
		public int SkipLevels { get; set; }
		//
		// Summary:
		//     Gets or sets the System.Windows.Media.MultiScaleTileSource object that is used
		//     as the source for the System.Windows.Controls.MultiScaleImage.
		//
		// Returns:
		//     The System.Windows.Media.MultiScaleTileSource object that is used as the source
		//     for the System.Windows.Controls.MultiScaleImage.
		public MultiScaleTileSource Source { get; set; }
		//
		// Summary:
		//     Gets the collection of System.Windows.Controls.MultiScaleSubImage objects within
		//     the multiresolution image that is used by the System.Windows.Controls.MultiScaleImage.
		//
		// Returns:
		//     The collection of System.Windows.Controls.MultiScaleSubImage objects within the
		//     multiresolution image that is used by the System.Windows.Controls.MultiScaleImage.
		public ReadOnlyCollection<MultiScaleSubImage> SubImages { get; }
		//
		// Summary:
		//     Gets or sets a value that indicates whether the System.Windows.Controls.MultiScaleImage
		//     uses spring animations.
		//
		// Returns:
		//     true if the System.Windows.Controls.MultiScaleImage uses spring animations; otherwise,
		//     false. The default value is true.
		public bool UseSprings { get; set; }
		//
		// Summary:
		//     Gets or sets the top-left corner of the area of the image to be displayed.
		//
		// Returns:
		//     The top-left corner of the rectangular area of the image to be displayed.
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
		//     Occurs if the download of a tile times out or fails for another reason.
		public event RoutedEventHandler ImageFailed;
		//
		// Summary:
		//     Occurs if the first piece of metadata used to open the image fails. If this event
		//     occurs no parts of the image will open successfully.
		public event EventHandler<ExceptionRoutedEventArgs> ImageOpenFailed;
		//
		// Summary:
		//     Occurs when the first piece of metadata that is needed to load the rest of the
		//     tiles opens.
		public event RoutedEventHandler ImageOpenSucceeded;
		//
		// Summary:
		//     Occurs when the zoom or pan animation ends.
		public event RoutedEventHandler MotionFinished;
		//
		// Summary:
		//     Occurs when the collection of System.Windows.Controls.MultiScaleSubImage objects
		//     within the multiresolution image that is used by the System.Windows.Controls.MultiScaleImage
		//     fails to open.
		public event SubImageEventHandler SubImageOpenFailed;
		//
		// Summary:
		//     Occurs when the collection of System.Windows.Controls.MultiScaleSubImage objects
		//     within the multiresolution image that is used by the System.Windows.Controls.MultiScaleImage
		//     opens successfully.
		public event SubImageEventHandler SubImageOpenSucceeded;
		//
		// Summary:
		//     Occurs when the viewport (the area of the image displayed) changes.
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
		public void ZoomAboutLogicalPoint(double zoomIncrementFactor, double zoomCenterLogicalX, double zoomCenterLogicalY)
		{
			
		}

		protected override AutomationPeer OnCreateAutomationPeer()
		{
			return default(AutomationPeer);
		}
	}
}
#endif
#endif