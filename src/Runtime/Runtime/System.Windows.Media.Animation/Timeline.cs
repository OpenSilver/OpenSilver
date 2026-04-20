
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
public abstract class Timeline : DependencyObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Timeline"/> class.
    /// </summary>
    protected Timeline() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Timeline"/> class with the specified 
    /// <see cref="BeginTime"/>.
    /// </summary>
    /// <param name="beginTime">
    /// The time at which this <see cref="Timeline"/> should begin. See the <see cref="BeginTime"/> 
    /// property for more information.
    /// </param>
    protected Timeline(TimeSpan? beginTime)
    {
        BeginTime = beginTime;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Timeline"/> class with the specified 
    /// <see cref="BeginTime"/> and <see cref="Duration"/>.
    /// </summary>
    /// <param name="beginTime">
    /// The time at which this <see cref="Timeline"/> should begin. See the <see cref="BeginTime"/> 
    /// property for more information.
    /// </param>
    /// <param name="duration">
    /// The length of time for which this timeline plays, not counting repetitions. See the 
    /// <see cref="Duration"/> property for more information.
    /// </param>
    protected Timeline(TimeSpan? beginTime, Duration duration)
    {
        BeginTime = beginTime;
        Duration = duration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Timeline"/> class with the specified 
    /// <see cref="BeginTime"/>, <see cref="Duration"/>, and <see cref="RepeatBehavior"/>.
    /// </summary>
    /// <param name="beginTime">
    /// The time at which this <see cref="Timeline"/> should begin. See the <see cref="BeginTime"/> 
    /// property for more information.
    /// </param>
    /// <param name="duration">
    /// The length of time for which this timeline plays, not counting repetitions. See the 
    /// <see cref="Duration"/> property for more information.
    /// </param>
    /// <param name="repeatBehavior">
    /// The repeating behavior of this timeline, either as an iteration <see cref="RepeatBehavior.Count"/> 
    /// or a repeat <see cref="RepeatBehavior.Duration"/>. See the <see cref="RepeatBehavior"/> property 
    /// for more information.
    /// </param>
    protected Timeline(TimeSpan? beginTime, Duration duration, RepeatBehavior repeatBehavior)
    {
        BeginTime = beginTime;
        Duration = duration;
        RepeatBehavior = repeatBehavior;
    }

    /// <summary>
    /// Occurs when the <see cref="Storyboard"/> object has completed playing.
    /// </summary>
    public event EventHandler Completed;

    internal void RaiseCompleted() => Completed?.Invoke(this, EventArgs.Empty);

    /// <summary>
    /// Identifies the <see cref="AutoReverse"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AutoReverseProperty =
        DependencyProperty.Register(
            nameof(AutoReverse),
            typeof(bool),
            typeof(Timeline),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Gets or sets a value that indicates whether the timeline plays in reverse after
    /// it completes a forward iteration.
    /// </summary>
    /// <returns>
    /// true if the timeline plays in reverse at the end of each iteration; otherwise,
    /// false. The default value is false.
    /// </returns>
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
    public static readonly DependencyProperty FillBehaviorProperty =
        DependencyProperty.Register(
            nameof(FillBehavior),
            typeof(FillBehavior),
            typeof(Timeline),
            new PropertyMetadata(FillBehavior.HoldEnd));

    /// <summary>
    /// Gets or sets a value that specifies how the animation behaves after it reaches
    /// the end of its active period.
    /// </summary>
    /// <returns>
    /// A value that specifies how the timeline behaves after it reaches the end of its
    /// active period but its parent is inside its active or fill period. The default
    /// value is <see cref="FillBehavior.HoldEnd"/>.
    /// </returns>
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
    public static readonly DependencyProperty SpeedRatioProperty =
        DependencyProperty.Register(
            nameof(SpeedRatio),
            typeof(double),
            typeof(Timeline),
            new PropertyMetadata(1.0),
            ValidateSpeedRatio);

    /// <summary>
    /// Gets or sets the rate, relative to its parent, at which time progresses for this
    /// <see cref="Timeline"/>.
    /// </summary>
    /// <returns>
    /// A finite value greater than 0 that describes the rate at which time progresses for
    /// this timeline, relative to the speed of the timeline's parent or, if this is a root 
    /// timeline, the default timeline speed. The default value is 1.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// <see cref="SpeedRatio"/> is less than 0 or is not a finite value.
    /// </exception>
    /// <remarks>
    /// A timeline's <see cref="SpeedRatio"/> setting does not have an effect on its <see cref="BeginTime"/>;
    /// that time is relative to the timeline's parent or, if the timeline is a root timeline,
    /// the moment at which the timeline's clock was begun.
    /// </remarks>
    public double SpeedRatio
    {
        get => (double)GetValue(SpeedRatioProperty);
        set => SetValueInternal(SpeedRatioProperty, value);
    }

    /// <summary>
    /// Gets or sets the desired frame rate for this timeline and its child timelines.
    /// </summary>
    [OpenSilver.NotImplemented]
    public static readonly DependencyProperty DesiredFrameRateProperty =
        DependencyProperty.RegisterAttached(
            "DesiredFrameRate",
            typeof(int?),
            typeof(Timeline),
            null,
            new ValidateValueCallback(ValidateDesiredFrameRate));

    private static bool ValidateDesiredFrameRate(object value)
    {
        var desiredFrameRate = (int?)value;

        return !desiredFrameRate.HasValue || desiredFrameRate.Value > 0;
    }

    /// <summary>
    /// Gets the desired frame rate of the specified <see cref="Timeline"/>.
    /// </summary>
    /// <param name="timeline">The timeline from which to retrieve the desired frame rate.</param>
    /// <returns>The desired frame rate of this timeline. The default value is <see langword="null"/>.</returns>
    /// <seealso cref="DesiredFrameRateProperty"/>
    [OpenSilver.NotImplemented]
    public static int? GetDesiredFrameRate(Timeline timeline)
    {
        ArgumentNullException.ThrowIfNull(timeline);

        return (int?)timeline.GetValue(DesiredFrameRateProperty);
    }

    /// <summary>
    /// Sets the desired frame rate of the specified <see cref="Timeline"/>.
    /// </summary>
    /// <param name="timeline">The <see cref="Timeline"/> to which <paramref name="desiredFrameRate"/> is assigned.</param>
    /// <param name="desiredFrameRate">
    /// The maximum number of frames this timeline should generate each second,
    /// or <see langword="null"/> if the system should control the number of frames.
    /// </param>
    /// <seealso cref="DesiredFrameRateProperty"/>
    [OpenSilver.NotImplemented]
    public static void SetDesiredFrameRate(Timeline timeline, int? desiredFrameRate)
    {
        ArgumentNullException.ThrowIfNull(timeline);

        timeline.SetValueInternal(DesiredFrameRateProperty, desiredFrameRate);
    }

    private static bool ValidateSpeedRatio(object value)
    {
        double newValue = (double)value;

        if (newValue <= 0 || newValue > double.MaxValue || double.IsNaN(newValue))
        {
            throw new ArgumentException(string.Format(Strings.Timing_InvalidArgFinitePositive), nameof(value));
        }

        return true;
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

    internal virtual TimelineClock CreateClock() => null;
}