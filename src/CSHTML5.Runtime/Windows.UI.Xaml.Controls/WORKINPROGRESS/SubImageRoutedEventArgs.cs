#if WORKINPROGRESS

#if MIGRATION
namespace System.Windows.Controls
{
	//
	// Summary:
	//     Contains state information and event data associated with the System.Windows.Controls.MultiScaleImage.SubImageOpenSucceeded
	//     and the System.Windows.Controls.MultiScaleImage.SubImageOpenFailed routed events.
	public class SubImageRoutedEventArgs : RoutedEventArgs
	{
		//
		// Summary:
		//     Gets the identifier for the System.Windows.Controls.SubImageRoutedEventArgs.SubImage
		//     dependency property
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.SubImageRoutedEventArgs.SubImage
		//     dependency property.
		public static readonly DependencyProperty SubImageProperty =
			DependencyProperty.Register("SubImage",
										typeof(MultiScaleSubImage),
										typeof(SubImageRoutedEventArgs),
										null);

		//
		// Summary:
		//     Initializes a new instance of the System.Windows.Controls.SubImageRoutedEventArgs
		//     class.
		public SubImageRoutedEventArgs()
		{
			
		}

		//
		// Summary:
		//     Gets the sub-image that is opened.
		//
		// Returns:
		//     The sub-image that is opened.
		public MultiScaleSubImage SubImage
		{
			get { return null; }
		}
	}
}
#endif
#endif