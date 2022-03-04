using System;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
	/// <summary>
	/// Provides data for the <see cref="System.Windows.Controls.Calendar.DisplayModeChanged" /> event. 
	/// </summary>	
	public class CalendarModeChangedEventArgs : RoutedEventArgs
	{
        /// <summary>
        /// Gets the previous mode of the <see cref="System.Windows.Controls.Calendar" />.
        /// /summary>
        /// <returns>
        /// A <see cref="System.Windows.Controls.CalendarMode" /> representing the previous mode. 
        /// </returns>		
        public CalendarMode OldMode { get; private set; }

		/// <summary>
		/// Gets the new mode of the <see cref="System.Windows.Controls.Calendar" />.
		/// </summary>
		/// <returns>A <see cref="System.Windows.Controls.CalendarMode" /> representing the new mode. 
		/// </returns>		
		public CalendarMode NewMode { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="System.Windows.Controls.CalendarModeChangedEventArgs" /> class. 
		/// </summary>
		/// <param name="oldMode">The previous mode.</param>
		/// <param name="newMode">The new mode.</param>		
		public CalendarModeChangedEventArgs(CalendarMode oldMode, CalendarMode newMode)
		{
			this.OldMode = oldMode;
			this.NewMode = newMode;
		}
	}
}
