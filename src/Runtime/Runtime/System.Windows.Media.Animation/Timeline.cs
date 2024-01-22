
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using OpenSilver.Internal.Media.Animation;
using OpenSilver.Internal;

namespace System.Windows.Media.Animation;

/// <summary>
/// Defines a segment of time.
/// </summary>
public abstract partial class Timeline : DependencyObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Timeline"/> class.
    /// </summary>
    protected Timeline() { }

    /// <summary>
    /// Occurs when the <see cref="Storyboard"/> object has completed playing.
    /// </summary>
    public event EventHandler Completed;

    internal void RaiseCompleted() => Completed?.Invoke(this, EventArgs.Empty);

    /// <summary>
    /// Identifies the <see cref="AutoReverse"/> dependency property.
    /// </summary>
    [OpenSilver.NotImplemented]
    public static readonly DependencyProperty AutoReverseProperty =
        DependencyProperty.Register(
            nameof(AutoReverse),
            typeof(bool),
            typeof(Timeline),
            null);

    /// <summary>
    /// Gets or sets a value that indicates whether the timeline plays in reverse after
    /// it completes a forward iteration.
    /// </summary>
    /// <returns>
    /// true if the timeline plays in reverse at the end of each iteration; otherwise,
    /// false. The default value is false.
    /// </returns>
    [OpenSilver.NotImplemented]
    public bool AutoReverse
    {
        get => (bool)GetValue(AutoReverseProperty);
        set => SetValueInternal(AutoReverseProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="BeginTime"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BeginTimeProperty =
        DependencyProperty.Register(
            nameof(BeginTime),
            typeof(TimeSpan?),
            typeof(Timeline),
            new PropertyMetadata(TimeSpan.Zero));

    /// <summary>
    /// Gets or sets the time at which this <see cref="Timeline"/> should begin.
    /// </summary>
    /// <returns>
    /// The start time of the time line. The default value is zero.
    /// </returns>
    public TimeSpan? BeginTime
    {
        get => (TimeSpan?)GetValue(BeginTimeProperty);
        set => SetValueInternal(BeginTimeProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Duration"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DurationProperty =
        DependencyProperty.Register(
            nameof(Duration),
            typeof(Duration),
            typeof(Timeline),
            new PropertyMetadata(Duration.Automatic));

    /// <summary>
    /// Gets or sets the length of time for which this timeline plays, not counting repetitions.
    /// </summary>
    /// <returns>
    /// The timeline's simple duration: the amount of time this timeline takes to complete
    /// a single forward iteration. The default value is <see cref="Duration.Automatic"/>.
    /// </returns>
    public Duration Duration
    {
        get => (Duration)GetValue(DurationProperty);
        set => SetValueInternal(DurationProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="FillBehavior"/> dependency property.
    /// </summary>
    [OpenSilver.NotImplemented]
    public static readonly DependencyProperty FillBehaviorProperty =
        DependencyProperty.Register(
            nameof(FillBehavior),
            typeof(FillBehavior),
            typeof(Timeline),
            null);

    /// <summary>
    /// Gets or sets a value that specifies how the animation behaves after it reaches
    /// the end of its active period.
    /// </summary>
    /// <returns>
    /// A value that specifies how the timeline behaves after it reaches the end of its
    /// active period but its parent is inside its active or fill period. The default
    /// value is <see cref="FillBehavior.HoldEnd"/>.
    /// </returns>
    [OpenSilver.NotImplemented]
    public FillBehavior FillBehavior
    {
        get => (FillBehavior)GetValue(FillBehaviorProperty);
        set => SetValueInternal(FillBehaviorProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="RepeatBehavior"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RepeatBehaviorProperty =
        DependencyProperty.Register(
            nameof(RepeatBehavior),
            typeof(RepeatBehavior),
            typeof(Timeline),
            new PropertyMetadata(new RepeatBehavior(1)));

    /// <summary>
    /// Gets or sets the repeating behavior of this timeline.
    /// </summary>
    /// <returns>
    /// An iteration <see cref="RepeatBehavior.Count"/> that specifies the number of 
    /// times the timeline should play, a <see cref="TimeSpan"/> value that specifies
    /// the total length of this timeline's active period, or the special value 
    /// <see cref="RepeatBehavior.Forever"/>, which specifies that the timeline should 
    /// repeat indefinitely. The default value is a <see cref="RepeatBehavior"/> with 
    /// a <see cref="RepeatBehavior.Count"/> of 1, which indicates that the timeline 
    /// plays once.
    /// </returns>
    public RepeatBehavior RepeatBehavior
    {
        get => (RepeatBehavior)GetValue(RepeatBehaviorProperty);
        set => SetValueInternal(RepeatBehaviorProperty, value);
    }

    /// <summary>
    /// Identifies for the <see cref="SpeedRatio"/> dependency property.
    /// </summary>
    [OpenSilver.NotImplemented]
    public static readonly DependencyProperty SpeedRatioProperty =
        DependencyProperty.Register(
            nameof(SpeedRatio),
            typeof(double),
            typeof(Timeline),
            new PropertyMetadata(1d));

    /// <summary>
    /// Gets or sets the rate, relative to its parent, at which time progresses for this
    /// <see cref="Timeline"/>.
    /// </summary>
    /// <returns>
    /// A finite value greater than 0 that specifies the rate at which time progresses
    /// for this timeline, relative to the speed of the timeline's parent. If this timeline
    /// is a root timeline, specifies the default timeline speed. The value is expressed
    /// as a factor where 1 represents normal speed, 2 is double speed, 0.5 is half speed,
    /// and so on. The default value is 1.
    /// </returns>
    [OpenSilver.NotImplemented]
    public double SpeedRatio
    {
        get => (double)GetValue(SpeedRatioProperty);
        set => SetValueInternal(SpeedRatioProperty, value);
    }

    /// <summary>
    /// Implemented by the class author to provide a custom natural Duration
    /// in the case that the Duration property is set to Automatic.  If the author
    /// cannot determine the Duration, this method should return Automatic.
    /// </summary>
    /// <returns>
    /// A Duration quantity representing the natural duration.
    /// </returns>
    protected virtual Duration GetNaturalDurationCore() => Duration.Automatic;

    internal INameResolver NameResolver { get; set; }

    internal virtual TimelineClock CreateClock(bool isRoot) => null;
}