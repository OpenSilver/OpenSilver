namespace System.Windows.Controls
{
	//
	// Summary:
	//     Contains state information and event data associated with the System.Windows.Controls.MultiScaleImage.SubImageOpenSucceeded
	//     and the System.Windows.Controls.MultiScaleImage.SubImageOpenFailed routed events.
    [OpenSilver.NotImplemented]
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
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty SubImageProperty =
			DependencyProperty.Register("SubImage",
										typeof(MultiScaleSubImage),
										typeof(SubImageRoutedEventArgs),
										null);

		//
		// Summary:
		//     Initializes a new instance of the System.Windows.Controls.SubImageRoutedEventArgs
		//     class.
        [OpenSilver.NotImplemented]
		public SubImageRoutedEventArgs()
		{
			
		}

		//
		// Summary:
		//     Gets the sub-image that is opened.
		//
		// Returns:
		//     The sub-image that is opened.
        [OpenSilver.NotImplemented]
		public MultiScaleSubImage SubImage
		{
			get { return null; }
		}
	}
}
