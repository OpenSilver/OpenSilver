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

using System.Windows.Markup;
using OpenSilver.Internal.Media.Animation;

namespace System.Windows.Media.Animation;

/// <summary>
/// Abstract class that, when implemented represents a <see cref="Timeline"/> that may contain 
/// a collection of child <see cref="Timeline"/> objects.
/// </summary>
[ContentProperty(nameof(Children))]
public abstract class TimelineGroup : Timeline
{
    private TimelineCollection _children;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimelineGroup"/> class, with default properties.
    /// </summary>
    protected TimelineGroup()
        : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TimelineGroup"/> class with the specified 
    /// <see cref="Timeline.BeginTime"/>.
    /// </summary>
    /// <param name="beginTime">
    /// The <see cref="Timeline.BeginTime"/> for this <see cref="TimelineGroup"/>.
    /// </param>
    protected TimelineGroup(TimeSpan? beginTime)
        : base(beginTime)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TimelineGroup"/> class with the specified 
    /// <see cref="Timeline.BeginTime"/> and <see cref="Timeline.Duration"/>.
    /// </summary>
    /// <param name="beginTime">
    /// The <see cref="Timeline.BeginTime"/> for this <see cref="TimelineGroup"/>.
    /// </param>
    /// <param name="duration">
    /// The <see cref="Timeline.Duration"/> for this <see cref="TimelineGroup"/>.
    /// </param>
    protected TimelineGroup(TimeSpan? beginTime, Duration duration)
        : base(beginTime, duration)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TimelineGroup"/> class with the specified 
    /// <see cref="Timeline.BeginTime"/>, <see cref="Timeline.Duration"/>, and <see cref="Timeline.RepeatBehavior"/>.
    /// </summary>
    /// <param name="beginTime">
    /// The <see cref="Timeline.BeginTime"/> for this <see cref="TimelineGroup"/>.
    /// </param>
    /// <param name="duration">
    /// The <see cref="Timeline.Duration"/> for this <see cref="TimelineGroup"/>.
    /// </param>
    /// <param name="repeatBehavior">
    /// The <see cref="Timeline.RepeatBehavior"/> for this <see cref="TimelineGroup"/>.
    /// </param>
    protected TimelineGroup(TimeSpan? beginTime, Duration duration, RepeatBehavior repeatBehavior)
        : base(beginTime, duration, repeatBehavior)
    {
    }

    /// <summary>
    /// Gets the collection of direct child <see cref="Timeline"/> objects of the <see cref="TimelineGroup"/>.
    /// </summary>
    /// <returns>
    /// Child <see cref="Timeline"/> objects of the <see cref="TimelineGroup"/>.
    /// </returns>
    public TimelineCollection Children => _children ??= new TimelineCollection(this);

    internal override TimelineClock CreateClock() => new ClockGroup(this);
}
