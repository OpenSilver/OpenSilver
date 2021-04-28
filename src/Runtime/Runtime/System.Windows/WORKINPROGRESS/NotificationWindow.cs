#if WORKINPROGRESS

#if MIGRATION

using System.Security;

namespace System.Windows
{
	//
	// Summary:
	//     Represents a notification area that is displayed in the system area. Notifications
	//     can only be enabled for an out-of-browser application; browser-hosted applications
	//     cannot access this notification area.
	[OpenSilver.NotImplemented]
	public sealed class NotificationWindow : DependencyObject
	{
		//
		// Summary:
		//     Identifies the System.Windows.NotificationWindow.Content dependency property.
		//
		// Returns:
		//     The identifier for the System.Windows.NotificationWindow.Content dependency property.
		[OpenSilver.NotImplemented]
#if WORKINPROGRESS
		public static readonly DependencyProperty ContentProperty =
			DependencyProperty.Register("Content",
										typeof(FrameworkElement),
										typeof(NotificationWindow),
										new FrameworkPropertyMetadata(FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));
#else
		public static readonly DependencyProperty ContentProperty =
			DependencyProperty.Register("Content",
										typeof(FrameworkElement),
										typeof(NotificationWindow),
										null);
#endif
		//
		// Summary:
		//     Identifies the System.Windows.NotificationWindow.Height dependency property.
		//
		// Returns:
		//     The identifier for the System.Windows.NotificationWindow.Height dependency property.
		[OpenSilver.NotImplemented]
#if WORKINPROGRESS
		public static readonly DependencyProperty HeightProperty =
			DependencyProperty.Register("Height",
										typeof(double),
										typeof(NotificationWindow),
										new FrameworkPropertyMetadata(FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));
#else
		public static readonly DependencyProperty HeightProperty =
			DependencyProperty.Register("Height",
										typeof(double),
										typeof(NotificationWindow),
										null);
#endif
		//
		// Summary:
		//     Identifies the System.Windows.NotificationWindow.Width dependency property.
		//
		// Returns:
		//     The identifier for the System.Windows.NotificationWindow.Width dependency property.
		[OpenSilver.NotImplemented]
#if WORKINPROGRESS
		public static readonly DependencyProperty WidthProperty =
			DependencyProperty.Register("Width",
										typeof(double),
										typeof(NotificationWindow),
										new FrameworkPropertyMetadata(FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));
#else
		public static readonly DependencyProperty WidthProperty =
			DependencyProperty.Register("Width",
										typeof(double),
										typeof(NotificationWindow),
										null);
#endif
		//
		// Summary:
		//     Initializes a new instance of the System.Windows.NotificationWindow class.
		[OpenSilver.NotImplemented]
		public NotificationWindow()
		{
			
		}

		//
		// Summary:
		//     Gets or sets the root of visual elements that define the visual look of the notification.
		//
		// Returns:
		//     A single System.Windows.FrameworkElement that includes the root of a visual tree.
		//     The visual tree defines the visual look of the notification.
		[OpenSilver.NotImplemented]
		public FrameworkElement Content
		{
			get { return (FrameworkElement)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
		//
		// Summary:
		//     Gets or sets the height, in pixels, of this notification window. See Remarks.
		//
		// Returns:
		//     The height, in pixels, of this notification window when it is displayed.
		//
		// Exceptions:
		//   T:System.ArgumentOutOfRangeException:
		//     Attempted to set value greater than 100, or less than 0.
		//
		//   T:System.InvalidOperationException:
		//     Attempted to set value while notification window is visible.
		[OpenSilver.NotImplemented]
		public double Height
		{
			get { return (double)GetValue(HeightProperty); }
			set { SetValue(HeightProperty, value); }
		}
		//
		// Summary:
		//     Gets a value that determines whether this notification is currently being displayed.
		//
		// Returns:
		//     true if the notification is currently displayed; otherwise, false.
		[OpenSilver.NotImplemented]
		public Visibility Visibility { get; }
		//
		// Summary:
		//     Gets or sets the width, in pixels, of this notification window. See Remarks.
		//
		// Returns:
		//     The width, in pixels, of this notification window when it is displayed.
		//
		// Exceptions:
		//   T:System.ArgumentOutOfRangeException:
		//     Attempted to set value greater than 400, or less than 0.
		//
		//   T:System.InvalidOperationException:
		//     Attempted to set value while notification window is visible.
		[OpenSilver.NotImplemented]
		public double Width
		{
			get { return (double)GetValue(WidthProperty); }
			set { SetValue(WidthProperty, value); }
		}

		//
		// Summary:
		//     Occurs when System.Windows.NotificationWindow.Close is called, or when the notification
		//     window times out and has finished its fadeout animation.
		[OpenSilver.NotImplemented]
		public event EventHandler Closed;

		//
		// Summary:
		//     Immediately closes the notification window.
		[OpenSilver.NotImplemented]
		public void Close()
		{
			
		}
		//
		// Summary:
		//     Displays the notification window for the specified number of milliseconds before
		//     it times out.
		//
		// Parameters:
		//   durationInMilliseconds:
		//     The duration that the notification window should remain displayed in the system
		//     area, specified in milliseconds.
		//
		// Exceptions:
		//   T:System.InvalidOperationException:
		//     A different notification window instance is still visible.
		[SecuritySafeCritical]
		[OpenSilver.NotImplemented]
		public void Show(int durationInMilliseconds)
		{
			
		}
	}
}

#endif
#endif