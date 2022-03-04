using System;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
	/// <summary>
	/// Provides data for the <see cref="System.Windows.Controls.Calendar.DisplayDateChanged" /> event. 
	/// </summary>	
	public class CalendarDateChangedEventArgs : RoutedEventArgs
	{
		/// <summary>
		/// Getsor sets the date that was previously displayed.
		/// </summary>
		/// <returns>
		/// The date previously displayed. 
		/// </returns>		
		public DateTime? RemovedDate { get; private set; }

		/// <summary>
		/// Gets or sets the date to be newly displayed.</summary>
		/// <returns>
		/// The new date to display.
		/// </returns>		
		public DateTime? AddedDate { get; private set; }
		
		internal CalendarDateChangedEventArgs(DateTime? removedDate, DateTime? addedDate)
		{
			this.RemovedDate = removedDate;
			this.AddedDate = addedDate;
		}
	}
}
