#if WORKINPROGRESS

using System.Security;

namespace System.Windows
{
	//
	// Summary:
	//     Represents a notification area that is displayed in the system area. Notifications
	//     can only be enabled for an out-of-browser application; browser-hosted applications
	//     cannot access this notification area.
	public sealed class NotificationWindow : DependencyObject
	{
		//
		// Summary:
		//     Identifies the System.Windows.NotificationWindow.Content dependency property.
		//
		// Returns:
		//     The identifier for the System.Windows.NotificationWindow.Content dependency property.
		public static readonly DependencyProperty ContentProperty;
		//
		// Summary:
		//     Identifies the System.Windows.NotificationWindow.Height dependency property.
		//
		// Returns:
		//     The identifier for the System.Windows.NotificationWindow.Height dependency property.
		public static readonly DependencyProperty HeightProperty;
		//
		// Summary:
		//     Identifies the System.Windows.NotificationWindow.Width dependency property.
		//
		// Returns:
		//     The identifier for the System.Windows.NotificationWindow.Width dependency property.
		public static readonly DependencyProperty WidthProperty;

		//
		// Summary:
		//     Initializes a new instance of the System.Windows.NotificationWindow class.
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
		public FrameworkElement Content { get; set; }
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
		public double Height { get; set; }
		//
		// Summary:
		//     Gets a value that determines whether this notification is currently being displayed.
		//
		// Returns:
		//     true if the notification is currently displayed; otherwise, false.
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
		public double Width { get; set; }

		//
		// Summary:
		//     Occurs when System.Windows.NotificationWindow.Close is called, or when the notification
		//     window times out and has finished its fadeout animation.
		public event EventHandler Closed;

		//
		// Summary:
		//     Immediately closes the notification window.
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
		public void Show(int durationInMilliseconds)
		{
			
		}
	}
}

#endif