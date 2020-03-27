#if WORKINPROGRESS

#if !MIGRATION
using System;
#endif

#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
	public partial struct RepeatBehavior
	{
		/// <summary>
		/// Initializes a new instance of the System.Windows.Media.Animation.RepeatBehavior structure with the specified repeat duration.
		/// </summary>
		/// <param name="duration">The total length of time that the System.Windows.Media.Animation.Timeline should play (its active duration).</param>
		/// <exception cref="ArgumentOutOfRangeException">duration evaluates to a negative number.</exception>
		public RepeatBehavior(TimeSpan duration)
		{
			_type = RepeatBehaviorType.Count;
			_count = 0;
			_hasCount = true;
			HasDuration = false;
			Duration = new TimeSpan();
		}
		
		/// <summary>
		/// Gets the total length of time a System.Windows.Media.Animation.Timeline should play.
		/// </summary>
		/// <returns>The total length of time a timeline should play.</returns>
		/// <exception cref="InvalidOperationException">
		/// This System.Windows.Media.Animation.RepeatBehavior describes an iteration count,
		/// not a repeat duration.
		/// </exception>
		public TimeSpan Duration { get; }
		
		/// <summary>
		/// Gets a value that indicates whether the repeat behavior has a specified repeat duration.
		/// </summary>
		/// <returns>true if the instance represents a repeat duration; otherwise, false.</returns>
		public bool HasDuration { get; }
	}
}
#endif